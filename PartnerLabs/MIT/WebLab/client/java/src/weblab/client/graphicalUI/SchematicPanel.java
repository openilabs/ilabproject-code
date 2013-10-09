/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

import java.applet.AppletContext;

import java.util.Observer;
import java.util.Observable;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

import java.net.URL;
import java.net.MalformedURLException;

import weblab.toolkit.util.Base64;
import weblab.toolkit.util.ChangeTrackingObservable;

import weblab.client.*;


/**
 * The SchematicPanel displays a circuit schematic of the currently
 * selected device and the analyzer ports to which it is connected.
 */
public class SchematicPanel extends JPanel implements Observer
{
  private MainFrame theMainFrame;

  private AppletContext theAppletContext;

  private Experiment theExperiment;
  private ExperimentSpecification theExpSpec;

  // contains PortLabel, parallels theExpSpec.getPorts()
  private List portLabels;

  // the device image (if any) for the current selected device of
  // theExpSpec (if any)
  private Image deviceImage;


  final int WIDTH = 500;
  final int HEIGHT = 300;
  final int DEVICEPANEL_HEIGHT = 200;
  final int PORTPANEL_HEIGHT = HEIGHT - DEVICEPANEL_HEIGHT;

  // pixel location of analyzer port's positive terminal (relative to
  // the top-left corner of the icon)
  final int PORT_POSITIVE_X = 25;
  final int PORT_POSITIVE_Y = 0;


  private JLabel deviceLabel;

  // a panel with the schematic icons for the analyzer ports
  private JPanel portPanel;

  private JPanel drawingLayer;


  /**
   * Creates a new SchematicPanel.
   */	
  public SchematicPanel(MainFrame theMainFrame, Experiment exp)
  {
    this.theAppletContext = theMainFrame.getAppletContext();
    this.theMainFrame = theMainFrame;
    this.theExperiment = exp;

    this.theExpSpec = theExperiment.getExperimentSpecification();

    theExperiment.addWeakReferenceObserver(this);
    theExpSpec.addWeakReferenceObserver(this);

    // create device label
    deviceLabel = new JLabel();

    // create portLabels
    this.portLabels = new ArrayList();

    // create portPanel
    portPanel = new JPanel();
    portPanel.setOpaque(false);
    portPanel.setPreferredSize(new Dimension(WIDTH, PORTPANEL_HEIGHT));
    portPanel.setLayout(new BoxLayout(portPanel, BoxLayout.X_AXIS));

    // do layout

    // create a panel with the schematic icon for the device
    JPanel devicePanel = new JPanel();
    devicePanel.setOpaque(false);
    devicePanel.setPreferredSize(new Dimension(WIDTH, DEVICEPANEL_HEIGHT));
    // use GridBagLayout because it centers things by default
    devicePanel.setLayout(new GridBagLayout());
    // add the device label to the panel
    devicePanel.add(deviceLabel);

    // layout schematic panel
    JPanel mainLayer = new JPanel();
    mainLayer.setOpaque(false);
    mainLayer.setLayout(new BoxLayout(mainLayer, BoxLayout.Y_AXIS));
    mainLayer.add(Box.createVerticalGlue());
    mainLayer.add(devicePanel);
    mainLayer.add(portPanel);
    mainLayer.add(Box.createVerticalGlue());
    // mainLayer.setBounds(0, 0, mainLayer.getWidth(), mainLayer.getHeight());

    // create drawingLayer: a panel that is transparent except for the
    // wires, terminal labels, and variable names we draw on it
    drawingLayer = new JPanel() {
	// do special painting stuff
	public void paintComponent(Graphics g) {
	  // paint background (except not, since it's transparent)
	  super.paintComponent(g);

	  drawDeviceName(g);
	  drawTerminalLabelsAndWires(g);
	  drawVariableNames(g);
	}
      };
    drawingLayer.setOpaque(false);
    // mouse events should ignore drawingLayer and be passed through
    // to components in mainLayer
    drawingLayer.setEnabled(false);

    JLayeredPane layeredPane = new JLayeredPane();
    //layeredPane.setOpaque(false);

    layeredPane.setPreferredSize(new Dimension(WIDTH, HEIGHT));
    layeredPane.setLayout(new OverlayLayout(layeredPane));

    // make sure the origins overlap so our drawing algorithms will
    // work
    mainLayer.setAlignmentX(Component.LEFT_ALIGNMENT);
    mainLayer.setAlignmentY(Component.TOP_ALIGNMENT);
    drawingLayer.setAlignmentX(Component.LEFT_ALIGNMENT);
    drawingLayer.setAlignmentY(Component.TOP_ALIGNMENT);

    layeredPane.add(mainLayer, new Integer(1));
    layeredPane.add(drawingLayer, new Integer(2));

    this.add(layeredPane);
    this.setOpaque(false);

    updatePorts();
    updateDeviceLabel();

    repaint();
  }



  /**
   * Changes the Experiment to which this SchematicPanel is assigned.
   */
  public void setExperiment(Experiment newExperiment)
  {
    // stop observing the old Experiment, update our reference, and
    // start observing the new one
    theExperiment.deleteObserver(this);
    theExperiment = newExperiment;
    theExperiment.addWeakReferenceObserver(this);

    // when there's a new Experiment, there's a new ExpSpec too
    setExpSpec(theExperiment.getExperimentSpecification());
  }



  // call this when ExpSpec is replaced
  private void setExpSpec(ExperimentSpecification newExpSpec)
  {
    // stop observing the old ExpSpec, update our reference, and
    // start observing the new one
    theExpSpec.deleteObserver(this);
    theExpSpec = newExpSpec;
    theExpSpec.addWeakReferenceObserver(this);

    // update for new ExpSpec
    updatePorts();
    updateDeviceLabel();

    // repaint this
    repaint();
  }



  /**
   * Paints the device name in the upper left corner.
   */
  public void drawDeviceName(Graphics g)
  {
    Device d = theExpSpec.getDevice();

    // if there's no device, don't paint anything
    if (d == null)
      return;

    int fontAscent = g.getFontMetrics().getAscent();

    // paint the device name in the upper-left corner of the
    // drawingLayer.  For inactive devices, use gray instead of black
    // and also paint "(inactive)" underneath the device name.

    if (theMainFrame.theWeblabClient.isActive(theExperiment))
      g.setColor(Color.black);
    else
    {
      g.setColor(Color.gray);
      g.drawString("(inactive)", 5, 10 + fontAscent * 2);
    }

    g.drawString(d.getName(), 5, 5 + fontAscent);
  }



  /**
   * Paints the name of each terminal next to the terminal, and paints
   * a wire from the associated port to the terminal.
   *
   * Note: the word "label" is used here in the sense of labelling a
   * diagram; it does not refer to an AWT or Swing label component.
   */
  public void drawTerminalLabelsAndWires(Graphics g)
  {
    // if there's no device, don't paint any terminal names or wires
    if (theExpSpec.getDevice() == null)
      return;


    // the point within the device image that should be centered in the
    // panel (though it may or may not actually *be* centered)
    Point centeringPoint = new Point(0, 0);

    if (deviceImage != null)
    {
      // by default, use the point in the middle of the image as the
      // centering point
      centeringPoint = new Point
	(deviceImage.getWidth(this) / 2, deviceImage.getHeight(this) / 2);
    }

    // convert into drawingLayer coordinates
    centeringPoint = SwingUtilities.convertPoint
      (deviceLabel, centeringPoint, drawingLayer);


    FontMetrics fm = g.getFontMetrics();
    int fontAscent = fm.getAscent();

    for(int i = 0, n = portLabels.size(); i < n; i++)
    {
      PortLabel pl = (PortLabel)portLabels.get(i);
      Terminal t = pl.getPort().getTerminal();

      String label = t.getLabel();
      int textwidth = fm.stringWidth(label);

      // location of terminal relative to drawingLayer
      Point terminalPoint = SwingUtilities.convertPoint
	(deviceLabel, t.getXPixelLocation(), t.getYPixelLocation(),
	 drawingLayer);

      // location of port relative to drawingLayer
      Point portPoint = SwingUtilities.convertPoint
	(pl, PORT_POSITIVE_X, PORT_POSITIVE_Y, drawingLayer);

      // position for lower-left hand corner of the label
      Point labelPosition = new Point(terminalPoint);

      // whether the wire from port to terminal should be drawn "up
      // and over" (true) or "over and up" (false)
      boolean upAndOver;

      // find displacement of terminal point from centering point in
      // each direction (positive if terminal is below/right of
      // center, negative for above/left)
      //
      // HACK: several existing device images have *all* their
      // terminals to the left of the centering point because of the
      // magic numbers for port icon locations in the graphical
      // applet, so use a 25-pixel threshold as a workaround.  <dmrz>
      int xdelta = terminalPoint.x - (centeringPoint.x - 25);
      int ydelta = terminalPoint.y - centeringPoint.y;

      /*
      // For debugging location of centering point.
      System.out.println("center x:" + centeringPoint.x +
			 "   y:" + centeringPoint.y);
      System.out.println("i: "+i+"  x: "+xdelta+"  y: "+ydelta);
      g.drawOval(centeringPoint.x - 25 - 4, centeringPoint.y - 4, 8, 8);
      */

      // determine which dimension (horizontal or vertical) has the
      // more significant displacement
      if (Math.abs(xdelta) > Math.abs(ydelta))
      {
	if (xdelta > 0)
	{
	  // terminal is on "right side" of image.

	  if (portPoint.x > terminalPoint.x)
	  {
	    // port is further right of center than terminal is, so
	    // wire should be drawn "up and over"
	    upAndOver = true;

	    // label should be 5 pixels to right of and above terminal
	    // point
	    labelPosition.translate(5, -5);
	  }
	  else
	  {
	    // port is nearer center than terminal is (or on the
	    // opposite side of center), so wire should be drawn "over
	    // and up"
	    upAndOver = false;

	    // label should be 5 pixels to right of terminal point and
	    // vertically centered
	    labelPosition.translate(5, fontAscent / 2);
	  }
	}
	else
	{
	  // terminal is on "left side" of image.

	  if (portPoint.x < terminalPoint.x)
	  {
	    // port is further left of center than terminal is, so
	    // wire should be drawn "up and over"
	    upAndOver = true;

	    // label should be 5 pixels to left of and above terminal
	    // point
	    labelPosition.translate(-5 - textwidth, -5);
	  }
	  else
	  {
	    // port is nearer center than terminal is (or on the
	    // opposite side of center), so wire should be drawn "over
	    // and up"
	    upAndOver = false;

	    // label should be 5 pixels to left of terminal
	    // point and vertically centered
	    labelPosition.translate(-5 - textwidth, fontAscent / 2);
	  }
	}
      }
      else
      {
	if (ydelta > 0)
	{
	  // terminal is at "bottom" of image.

	  // wire should be drawn "over and up"
	  upAndOver = false;

	  if (xdelta > 0)
	  {
	    // label should be 5 pixels to right of terminal point and
	    // vertically centered
	    labelPosition.translate(5, fontAscent / 2);
	  }
	  else
	  {
	    // label should be 5 pixels to left of terminal point and
	    // vertically centered
	    labelPosition.translate(-5 - textwidth, fontAscent / 2);
	  }
	}
	else
	{
	  // terminal is at "top" of image.

	  // wire should be drawn "up and over"
	  upAndOver = true;

	  if (portPoint.x > terminalPoint.x)
	  {
	    // wire will be coming in from right, so label should be 5
	    // pixels above and to right of terminal point
	    labelPosition.translate(5, -5);
	  }
	  else
	  {
	    // wire will be coming in from left, so label should be 5
	    // pixels above and to left of terminal point
	    labelPosition.translate(-5 - textwidth, -5);
	  }
	}
      }

      // paint the label
      g.setColor(Color.black);
      g.drawString(label, labelPosition.x, labelPosition.y);

      // paint the wire
      g.setColor(Color.blue);
      if (upAndOver)
      {
	g.drawLine(portPoint.x, portPoint.y,
		   portPoint.x, terminalPoint.y);
	g.drawLine(portPoint.x, terminalPoint.y,
		   terminalPoint.x, terminalPoint.y);
      }
      else
      {
	g.drawLine(portPoint.x, portPoint.y,
		   terminalPoint.x, portPoint.y);
	g.drawLine(terminalPoint.x, portPoint.y,
		   terminalPoint.x, terminalPoint.y);
      }
    }
  }



  /**
   * Draws VNames and INames, with symbols, next to each port.
   */
  public void drawVariableNames(final Graphics g)
  {
    g.setColor(Color.magenta);

    for(int i = 0; i < portLabels.size(); i++)
    {
      final PortLabel pl = (PortLabel)portLabels.get(i);

      // Start drawing at a point a bit below and to the right of the
      // top of the port (so we won't run into the wire), and convert
      // this point into the coordinate system of drawingLayer.
      final Point origin = SwingUtilities.convertPoint
	(pl, pl.getWidth() / 2 + 3, 2, drawingLayer);

      pl.getPort().accept(new DefaultVisitor() {
	  public void visitSMU(SMU u)
	  {
	    drawVName(u.getVName());
	    drawIName(u.getIName());
	  }
	  
	  public void visitVSU(VSU u)
	  {
	    drawVName(u.getVName());
	  }
	  
	  public void visitVMU(VMU u)
	  {
	    drawVName(u.getVName());
	  }

	  private void drawVName(String vName)
	  {
	    if (! vName.equals(""))
	    {
	      // draw a plus sign for voltage
	      
	      Point plusLeft = new Point(0, 19);
	      Point plusRight = new Point(8, 19);
	      Point plusTop = new Point(4, 15);
	      Point plusBottom = new Point(4, 23);
	
	      plusLeft.translate(origin.x, origin.y);
	      plusRight.translate(origin.x, origin.y);
	      plusTop.translate(origin.x, origin.y);
	      plusBottom.translate(origin.x, origin.y);
	
	      g.drawLine(plusLeft.x, plusLeft.y, plusRight.x, plusRight.y);
	      g.drawLine(plusTop.x, plusTop.y, plusBottom.x, plusBottom.y);
	
	      // draw the vname
      
	      Point vNamePoint = new Point(10, 23);
	      vNamePoint.translate(origin.x, origin.y);
	      g.drawString(vName, vNamePoint.x, vNamePoint.y);
	    }
	  }

	  private void drawIName(String iName)
	  {
	    if (! iName.equals(""))
	    {
	      // draw an arrow for current
	      
	      Point arrowTop = new Point(4, 0);
	      Point arrowBottom = new Point(4, 10);
	      Point arrowLeft = new Point(1, 5);
	      Point arrowRight = new Point(7, 5);
	      
	      arrowTop.translate(origin.x, origin.y);
	      arrowBottom.translate(origin.x, origin.y);
	      arrowLeft.translate(origin.x, origin.y);
	      arrowRight.translate(origin.x, origin.y);
	
	      g.drawLine(arrowTop.x, arrowTop.y, arrowBottom.x, arrowBottom.y);
	      g.drawLine(arrowTop.x, arrowTop.y, arrowLeft.x, arrowLeft.y);
	      g.drawLine(arrowTop.x, arrowTop.y, arrowRight.x, arrowRight.y);
	
	      // draw the iname
	      
	      Point iNamePoint = new Point(10, 10);
	      iNamePoint.translate(origin.x, origin.y);
	      g.drawString(iName, iNamePoint.x, iNamePoint.y);
	    }
	  }
	});
    }
  }



  /**
   * Handle updates from Observables
   */
  public final void update(Observable o, Object arg)
  {
    if (o == theExperiment &&
	ChangeTrackingObservable.extractChanges(arg).contains
	(Experiment.EXPERIMENT_SPECIFICATION_CHANGE))
      setExpSpec(theExperiment.getExperimentSpecification());

    if (o == theExpSpec &&
	ChangeTrackingObservable.extractChanges(arg).contains
	(ExperimentSpecification.DEVICE_CHANGE))
    {
      updateDeviceLabel();
      repaint();
    }

    if (o == theExpSpec &&
	ChangeTrackingObservable.extractChanges(arg).contains
	(ExperimentSpecification.PORTS_CHANGE))
    {
      updatePorts();
      repaint();
    }

    // when port data changes, just repaint the drawing layer
    if (o instanceof Port)
      drawingLayer.repaint();
  }



  // setup portPanel, portLabels based on expSpec
  private void updatePorts()
  {
    // clear contents of portPanel
    portPanel.removeAll();

    // clean up old portLabels and stop observing old Ports
    for (Iterator i = portLabels.iterator(); i.hasNext(); )
    {
      PortLabel pl = (PortLabel) i.next();
      pl.getPort().deleteObserver(this);
      // XXX replaces pl.cleanup().  Stupid problem: in some cases
      // this actually creates new PDs just to dispose them!
      theMainFrame.getPortDialog(pl.getPort()).dispose();
    }
    portLabels.clear();

    // add new port labels, separated and surrounded by horizontal
    // glue, and start observing new Ports
    portPanel.add(Box.createHorizontalGlue());
    for (Iterator i = theExpSpec.getPorts(); i.hasNext(); )
    {
      Port p = (Port) i.next();
      p.addWeakReferenceObserver(this);

      // add an SMU, VSU, or VMU icon label
      PortLabel pl = new PortLabel(theMainFrame, p);

      portLabels.add(pl);
      portPanel.add(pl);

      portPanel.add(Box.createHorizontalGlue());
    }

    portPanel.validate();
  }



  // set up device label based on device
  private void updateDeviceLabel()
  {
    Device d = theExpSpec.getDevice();

    if (d == null) {
      // if there is no device, display a placeholder text label
      deviceLabel.setText("(no device selected)");
      deviceLabel.setIcon(null);
      deviceLabel.validate();
      return;
    }

    //
    // try to load device image from lab configuration
    //

    String imageURL = d.getImageURL();
    String imageData = theExperiment.getLabConfiguration()
      .getImageData(imageURL);
    if (imageData != null)
    {
      byte[] imageBytes = Base64.decode(imageData);
      deviceImage = Toolkit.getDefaultToolkit().createImage(imageBytes);
      deviceLabel.setText(null);
      deviceLabel.setIcon(new ImageIcon(deviceImage));
      deviceLabel.validate();
      return;
    }

    // try to load the device image from the web
    try
    {
      deviceImage = theAppletContext.getImage(new URL(imageURL));
      if (deviceImage == null)
	System.out.println("AppletContext silently failed to load " +
			   imageURL + " from the web");
      else
      {
	deviceLabel.setText(null);
	deviceLabel.setIcon(new ImageIcon(deviceImage));
	deviceLabel.validate();
	return;
      }
    }
    catch(MalformedURLException e) {
      e.printStackTrace();
    }
    catch(SecurityException e) {
      System.out.println
	("Security manager won't let us load " + imageURL + " from the web");
    }

    //
    // give up
    //

    deviceLabel.setText("(device image not available)");
    deviceLabel.setIcon(null);
    deviceLabel.validate();

    String error = "Failed to load device image " + imageURL;
    JOptionPane.showMessageDialog(this, error, "Error",
				  JOptionPane.ERROR_MESSAGE);
  }

} // end class SchematicPanel
