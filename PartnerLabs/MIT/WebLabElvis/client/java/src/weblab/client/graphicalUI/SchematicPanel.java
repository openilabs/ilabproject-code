package weblab.client.graphicalUI;

import weblab.client.WeblabClient;
import weblab.client.ExperimentSpecification;
import weblab.client.Setup;
import weblab.client.Terminal;
import weblab.client.Instrument;
import weblab.client.FGEN;
import weblab.client.SCOPE;

import weblab.client.DefaultVisitor;

import weblab.util.Base64;
import weblab.util.ChangeTrackingObservable;

import java.awt.*;
import javax.swing.*;
import java.awt.event.*;

import java.applet.AppletContext;

import java.util.Observer;
import java.util.Observable;
import java.util.Enumeration;
import java.util.Iterator;

import java.net.URL;
import java.net.MalformedURLException;

/**
 * The SchematicPanel displays a circuit schematic of the currently
 * selected experiment setup and the instruments to which it is connected.
 */
public class SchematicPanel extends JPanel implements Observer
{
  private AppletContext theAppletContext;

  protected Frame theMainFrame;
  protected WeblabClient theWeblabClient;
  protected ExperimentSpecification theExpSpec;

  private Setup theSetup;

  final int WIDTH = 500;
  final int HEIGHT = 300;
  final int SETUPPANEL_HEIGHT = 200;
  final int INSTRUMENTPANEL_HEIGHT = HEIGHT - SETUPPANEL_HEIGHT;

  // pixel location of instrument's positive terminal (relative to
  // the top-left corner of the icon)
  final int INSTRUMENT_POSITIVE_X = 25;
  final int INSTRUMENT_POSITIVE_Y = 0;

  JPanel drawingLayer;
  JLabel setupLabel;
  java.util.List instrumentLabels; // List of InstrumentLabel, order corresponds
			     // to order of theExpSpec.getInstruments()

  // a panel with the schematic icons for the instruments
  JPanel instrumentPanel;


	
  public SchematicPanel(Frame theMainFrame, WeblabClient theWeblabClient,
			AppletContext appletContext)
  {
    this.theMainFrame = theMainFrame;
    this.theWeblabClient = theWeblabClient;
    this.theAppletContext = appletContext;

    theExpSpec = theWeblabClient.getExperimentSpecification();

    theWeblabClient.addObserver(this);
    theExpSpec.addObserver(this);


    // create setup label
    setupLabel = new JLabel();

    // create instrumentLabels
    this.instrumentLabels = new java.util.ArrayList();

    // create instrumentPanel
    instrumentPanel = new JPanel();
    instrumentPanel.setOpaque(false);
    instrumentPanel.setPreferredSize(new Dimension(WIDTH, INSTRUMENTPANEL_HEIGHT));
    instrumentPanel.setLayout(new BoxLayout(instrumentPanel, BoxLayout.X_AXIS));

    // do layout

    // create a panel with the schematic icon for the setup
    JPanel setupPanel = new JPanel();
    setupPanel.setOpaque(false);
    setupPanel.setPreferredSize(new Dimension(WIDTH, SETUPPANEL_HEIGHT));
    // use GridBagLayout because it centers things by default
    setupPanel.setLayout(new GridBagLayout());
    // add the setup label to the panel
    setupPanel.add(setupLabel);

    // layout schematic panel
    JPanel mainLayer = new JPanel();
    mainLayer.setOpaque(false);
    mainLayer.setLayout(new BoxLayout(mainLayer, BoxLayout.Y_AXIS));
    mainLayer.add(Box.createVerticalGlue());
    mainLayer.add(setupPanel);
    mainLayer.add(instrumentPanel);
    mainLayer.add(Box.createVerticalGlue());
    //    mainLayer.setBounds(0, 0, mainLayer.getWidth(), mainLayer.getHeight());

    // create drawingLayer: a panel that is transparent except for the
    // wires, terminal labels, and variable names we draw on it
    drawingLayer = new JPanel() {
	// do special painting stuff
	public void paintComponent(Graphics g) {
	  // paint background (except not, since it's transparent)
	  super.paintComponent(g);

	  g.setColor(Color.black);
	  drawSetupName(g);
	  g.setColor(Color.blue);
	  drawWires(g);
	  //g.setColor(Color.black);
	  //drawTerminalLabels(g);
	  g.setColor(Color.magenta);
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

    initForSetup();
  }


  public void drawSetupName(Graphics g)
  {
    // if there's no setup, don't paint anything
    if (theSetup == null) {
      return;
    }

    // paint the setup name in the upper-left corner of the drawingLayer
    g.drawString(theSetup.getName(), 5, 5 + g.getFontMetrics().getAscent());
  }


  public void drawWires(Graphics g)
  {
    // if there's no setup, don't paint any wires
    if (theSetup == null) {
      return;
    }

    Terminal[] terminals = theSetup.getTerminals();
    //assert (terminals.length == instrumentLabels.size());

    // for all i, connect terminal i to instrumentLabel i
    for(int i = 0; i < terminals.length; i++) {
      Terminal t = terminals[i];
      InstrumentLabel pl = (InstrumentLabel)instrumentLabels.get(i);

      // find location of terminal relative to drawingLayer
      Point terminalPoint = SwingUtilities.convertPoint
	(setupLabel, t.getXPixelLocation(), t.getYPixelLocation(),
	 drawingLayer);

      // find location of instrument wire relative to drawingLayer
      Point instrumentPoint = SwingUtilities.convertPoint
	(pl, INSTRUMENT_POSITIVE_X, INSTRUMENT_POSITIVE_Y, drawingLayer);

      // paint the wire
      //g.drawLine(terminalPoint.x, terminalPoint.y,
      //	 instrumentPoint.x, terminalPoint.y);
      // g.drawLine(instrumentPoint.x, terminalPoint.y,
      //	 instrumentPoint.x, instrumentPoint.y);
      g.drawLine(instrumentPoint.x, instrumentPoint.y,
    	      terminalPoint.x, instrumentPoint.y);
      g.drawLine(terminalPoint.x, instrumentPoint.y,
    		  terminalPoint.x, terminalPoint.y);
    }
  }


  /**
   * Paints the name of each terminal next to the terminal.
   *
   * Note: the word "label" is used here in the sense of labelling a
   * diagram; it does not refer to an AWT or Swing label component.
   */
  public void drawTerminalLabels(Graphics g)
  {
    // if there's no setup, don't paint any terminal names
    if (theSetup == null) {
      return;
    }

    Terminal[] terminals = theSetup.getTerminals();

    for(int i = 0; i < terminals.length; i++) {
      Terminal t = terminals[i];

      // find location of terminal relative to drawingLayer
      Point terminalPoint = SwingUtilities.convertPoint
	(setupLabel, t.getXPixelLocation(), t.getYPixelLocation(),
	 drawingLayer);

      // terminal name should be 10 pixels away from the actual
      // terminal location in each dimension
      terminalPoint.translate(-90, 0);

      // paint the name
      g.drawString(t.getLabel(), terminalPoint.x, terminalPoint.y);
    }
  }

  /**
   * Draws VNames and INames, with symbols, next to each instrument.
   */
  public void drawVariableNames(final Graphics g)
  {
    for(int i = 0; i < instrumentLabels.size(); i++) {
      final InstrumentLabel pl = (InstrumentLabel)instrumentLabels.get(i);

      // Start drawing at a point a bit to the right of pl's center
      // (so we don't run into the wire), and convert this point into
      // the coordinate system of drawingLayer.
      final Point origin = SwingUtilities.convertPoint
	(pl, pl.getWidth() / 2 + 3, 0, drawingLayer);

      pl.getInstrument().accept(new DefaultVisitor() {
	  public void visitFGEN(FGEN f)
	  {
	    drawVName(f.getVName());
	  }
	 
	  public void visitSCOPE(SCOPE s)
	  {
	    drawVName(s.getVName());
	  }
	  
	  public void drawVName(String vName)
	  {
	    if (! vName.equals("")) {
	      // draw a plus sign for voltage
	      
	      Point plusLeft = new Point(0, 21);
	      Point plusRight = new Point(8, 21);
	      Point plusTop = new Point(4, 17);
	      Point plusBottom = new Point(4, 25);
	
	      plusLeft.translate(origin.x, origin.y);
	      plusRight.translate(origin.x, origin.y);
	      plusTop.translate(origin.x, origin.y);
	      plusBottom.translate(origin.x, origin.y);
	
	      g.drawLine(plusLeft.x, plusLeft.y, plusRight.x, plusRight.y);
	      g.drawLine(plusTop.x, plusTop.y, plusBottom.x, plusBottom.y);
	
	      // draw the vname
      
	      Point vNamePoint = new Point(10, 25);
	      vNamePoint.translate(origin.x, origin.y);
	      g.drawString(vName, vNamePoint.x, vNamePoint.y);
	    }
	  }

	  public void drawIName(String iName)
	  {
	    if (! iName.equals("")) {
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
    if (o == theWeblabClient && ChangeTrackingObservable.containsChange
	(arg, WeblabClient.EXPERIMENT_SPECIFICATION_CHANGE))
    {
      theExpSpec.deleteObserver(this);
      theExpSpec = theWeblabClient.getExperimentSpecification();
      theExpSpec.addObserver(this);
      initForSetup();
    }

    if (o == theExpSpec && ChangeTrackingObservable.containsChange
	(arg, ExperimentSpecification.SETUP_CHANGE))
    {
      initForSetup();
    }
  }



  private void initForSetup()
  {
    // update pointer to setup
    theSetup = theExpSpec.getSetup();

    updateInstruments();
    updateSetupLabel();

    // repaint this
    repaint();
  }



  // setup instrumentPanel, instrumentLabels based on setup
  private void updateInstruments()
  {
    // clear contents of instrumentPanel
    instrumentPanel.removeAll();

    // clean up instrumentLabels
    for (Iterator i = instrumentLabels.iterator(); i.hasNext();)
    {
      InstrumentLabel pl = (InstrumentLabel) i.next();
      pl.cleanup();
    }
    instrumentLabels.clear();

    // add the new instrument labels, separated and surrounded by horizontal glue
    instrumentPanel.add(Box.createHorizontalGlue());
    if (theSetup == null) {
      // no instruments, so do nothing
    }
    else
    {
      Enumeration instruments_enum = theExpSpec.getInstruments();
      while(instruments_enum.hasMoreElements())
      {
    	  Instrument p = (Instrument) instruments_enum.nextElement();

    	  // add a listener to repaint the drawing layer when the instrument
    	  // data changes
    	  p.addObserver(new Observer() {
    		  public final void update(Observable o, Object arg)
    		  {
    			  drawingLayer.repaint();
    		  }
	  });

    	  // add an FGEN icon label
    	  if (p instanceof FGEN) {
    		  InstrumentLabel pl = new FGENLabel(theMainFrame, (FGEN)p);
    		  instrumentLabels.add(pl);
    		  instrumentPanel.add(pl);
    	  }
    	  else if (p instanceof SCOPE) {
    		  InstrumentLabel pl = new SCOPELabel(theMainFrame, (SCOPE)p);
    		  instrumentLabels.add(pl);
    		  instrumentPanel.add(pl);
    	  }
    	  else
    	  {
    		  // shouldn't ever get here
    		  throw new Error("Instrument type not recognized");
		 //assert false : "Instrument is not an instance of FGEN";
	}
	instrumentPanel.add(Box.createHorizontalGlue());
      }
    }
    instrumentPanel.validate();
  }



  // set up setup label based on setup
  private void updateSetupLabel()
  {
    if (theSetup == null) {
      // if there is no setup, display a placeholder text label
      setupLabel.setText("(no experiment setup selected)");
      setupLabel.setIcon(null);
      setupLabel.validate();
      return;
    }

    //
    // try to load setup image from lab configuration
    //

    String imageURL = theSetup.getImageURL();
    String imageData = theWeblabClient.getLabConfiguration().getImageData(imageURL);
    if (imageData != null)
    {
      byte[] imageBytes = Base64.decode(imageData);
      Image setupImage = Toolkit.getDefaultToolkit().createImage(imageBytes);
      setupLabel.setText(null);
      setupLabel.setIcon(new ImageIcon(setupImage));
      setupLabel.validate();
      return;
    }

    // try to load the setup image from the web
    try
    {
      Image setupImage = theAppletContext.getImage(new URL(imageURL));
      if (setupImage == null)
    	  System.out.println("AppletContext silently failed to load " +
    			  			  imageURL + " from the web");
      else
      {
    	  setupLabel.setText(null);
    	  setupLabel.setIcon(new ImageIcon(setupImage));
    	  setupLabel.validate();
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

    setupLabel.setText("(experiment setup image not available)");
    setupLabel.setIcon(null);
    setupLabel.validate();

    String error = "Failed to load experiment setup image " + imageURL;
    JOptionPane.showMessageDialog(this, error, "Error",
				  JOptionPane.ERROR_MESSAGE);
  }

} // end class SchematicPanel
