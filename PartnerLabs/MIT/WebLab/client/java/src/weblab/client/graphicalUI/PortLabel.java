/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

import java.math.BigDecimal;

import java.util.Observable;
import java.util.Observer;

import weblab.toolkit.util.EngUnits;
import weblab.toolkit.util.EngValue;

import weblab.client.*;


/**
 * Graphical representation of a Port.
 */
public class PortLabel extends JPanel implements Observer, MouseListener
{
  private MainFrame theMainFrame;
  private JLabel imageLabel;

  private Port myPort;

  private static final int HEIGHT = 100;
  private static final int WIDTH = 50;

  private static final int NUM_SIGFIGS = 4;



  /**
   * Creates a new PortLabel.
   */
  public PortLabel(MainFrame theMainFrame, Port myPort)
  {
    this.theMainFrame = theMainFrame;
    this.myPort = myPort;

    // setup components

    this.imageLabel = new JLabel();
    imageLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

    JLabel nameLabel = new JLabel(myPort.getName());
    nameLabel.setHorizontalAlignment(SwingConstants.CENTER);
    nameLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

    // setup layout

    this.setLayout(new BoxLayout(this, BoxLayout.Y_AXIS));
    this.add(imageLabel);
    this.add(nameLabel);

    Dimension d = new Dimension(WIDTH, HEIGHT);
    this.setMinimumSize(d);
    this.setPreferredSize(d);
    this.setMaximumSize(d);

    this.setOpaque(false);

    // display the hand cursor whenever the mouse is over this
    this.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));

    // setup listeners

    myPort.addWeakReferenceObserver(this);
    this.addMouseListener(this);

    // initialize

    this.updateImage();
    this.updateToolTip();
  }



  public Port getPort()
  {
    return myPort;
  }


  //////////////////////
  // Listener Methods //
  //////////////////////


  /**
   * When port data changes, update image and tooltip.
   */
  public void update(Observable o, Object arg)
  {
    updateImage();
    updateToolTip();
  }



  // clicking on this opens the port dialog
  public void mouseClicked(MouseEvent evt)
  {
    // Point location = new Point(e.getX()+10, e.getY()+10);
    // mySMUDialog.setLocation(getMousePointerAbsolute(location));
    theMainFrame.getPortDialog(myPort).setVisible(true);
  }



  // obligatory irrelevant methods of MouseListener
  public void mouseEntered(MouseEvent evt) {}
  public void mouseExited(MouseEvent evt) {}
  public void mousePressed(MouseEvent evt) {}
  public void mouseReleased(MouseEvent evt) {}


  ////////////////////
  // Helper Methods //
  ////////////////////


  private void updateImage()
  {
    // figure out which image to use
    String imageName = this.chooseImageName();

    // load the image
    try {
      ImageIcon portImageIcon = new ImageIcon(this.getClass().getResource
					      ("/img/" + imageName));

      // note: it's important to do setSize *before* setIcon;
      // otherwise the icon takes on the size of the label rather than
      // dictating the size of the label.  <dmrz>
      imageLabel.setSize(portImageIcon.getIconWidth(),
      			 portImageIcon.getIconHeight());
      imageLabel.setIcon(portImageIcon);

    }
    catch (Exception e) {
      String error =
	"Unable to load image icon: " + imageName;
      e.printStackTrace();
      JOptionPane.showMessageDialog(this, error, "Error",
				    JOptionPane.ERROR_MESSAGE);
    }

    // repaint
    repaint();
  }



  protected String chooseImageName()
  {
    switch(myPort.getType())
    {
    case Port.SMU_TYPE:
      SMU mySMU = (SMU) myPort;

      if (mySMU.getVName().equals("") && mySMU.getIName().equals(""))
      {
	// considered "unconfigured"
	return "port_SMU.gif";
      }
      else
	switch(mySMU.getMode())
	{
	case SMU.COMM_MODE:
	  return "port_Ground.gif";
	case SMU.V_MODE:
	  return chooseImageNameForVoltageSourceFunction(mySMU.getFunction());
	case SMU.I_MODE:
	  return chooseImageNameForCurrentSourceFunction(mySMU.getFunction());
	default:
	  throw new Error("illegal mode");
	}

    case Port.VSU_TYPE:
      VSU myVSU = (VSU) myPort;

      if (myVSU.getVName().equals(""))
      {
	// considered "unconfigured"
	return "port_VSU.gif";
      }
      else
      {
	// V mode
	return chooseImageNameForVoltageSourceFunction(myVSU.getFunction());
      }

    case Port.VMU_TYPE:
      VMU myVMU = (VMU) myPort;

      if (myVMU.getVName().equals(""))
      {
	// considered "unconfigured"
	return "port_VMU.gif";
      }
      else
	return "port_VoltageMeter.gif";

    default:
      throw new Error("illegal port type");
    }
  }



  private String chooseImageNameForCurrentSourceFunction(SourceFunction f)
  {
    if (f instanceof CONSFunction) {
      return "port_ConstantCurrentSource.gif";
    }
    else {
      // must be variable source function
      return "port_VariableCurrentSource.gif";
    }
  }



  private String chooseImageNameForVoltageSourceFunction(SourceFunction f)
  {
    if (f instanceof CONSFunction) {
      return "port_ConstantVoltageSource.gif";
    }
    else {
      // must be variable source function
      return "port_VariableVoltageSource.gif";
    }
  }



  /**
   * Sets the tooltip text to a multi-line summary of the port setup.
   */
  private void updateToolTip()
  {
    this.setToolTipText
      ((new DefaultVisitor()
	{
	  private StringBuffer summary;
	  private EngUnits units;
	
	  public String go(Port p)
	  {
	    summary = new StringBuffer();
	    p.accept(this);
	    return summary.toString();
	  }

	  public void visitSMU(SMU u)
	  {
	    summary.append("SMU" + u.getNumber() + "\n");

	    EngUnits complianceUnits;

	    // write VName line
	    summary.append("VName: ");
	    if (u.getVDownload()) {
	      summary.append("*");
	    }
	    summary.append(u.getVName());
	    if (u.getVDownload()) {
	      summary.append("*");
	    }
	    summary.append("\n");

	    // write IName line
	    summary.append("IName: ");
	    if (u.getIDownload()) {
	      summary.append("*");
	    }
	    summary.append(u.getIName());
	    if (u.getIDownload()) {
	      summary.append("*");
	    }
	    summary.append("\n");

	    // write Mode line
	    summary.append("Mode: ");
	    switch (u.getMode())
	    {
	    case SMU.V_MODE:
	      summary.append("V");
	      units = WeblabClient.VOLTAGE_UNITS;
	      complianceUnits = WeblabClient.CURRENT_UNITS;
	      break;
	    case SMU.I_MODE:
	      summary.append("I");
	      units = WeblabClient.CURRENT_UNITS;
	      complianceUnits = WeblabClient.VOLTAGE_UNITS;
	      break;
	    case SMU.COMM_MODE:
	      // this is it, we're done here
	      summary.append("COMM");
	      return;
	    default:
	      throw new Error("illegal mode");
	    }
	    summary.append("\n");

	    // visit function
	    u.getFunction().accept(this);
	    summary.append("\n");

	    summary.append("Compliance: " + engString
			   (u.getCompliance(), complianceUnits));
	  }

	  public void visitVMU(VMU u)
	  {
	    // write VName line
	    summary.append("VName: ");
	    if (u.getVDownload()) {
	      summary.append("*");
	    }
	    summary.append(u.getVName());
	    if (u.getVDownload()) {
	      summary.append("*");
	    }
	    summary.append("\n");

	    // Write Mode line
	    //
	    // currently weblab only supports V mode for VMUs   
	    summary.append("Mode: ");
	    switch (u.getMode()) {
	    case VMU.V_MODE:
	      summary.append("V");
	      break;
	    default:
	      assert false; // impossible mode
	    }
	  }

	  public void visitVSU(VSU u)
	  {
	    // write VName line
	    summary.append("VName: ");
	    if (u.getVDownload()) {
	      summary.append("*");
	    }
	    summary.append(u.getVName());
	    if (u.getVDownload()) {
	      summary.append("*");
	    }
	    summary.append("\n");

	    units = WeblabClient.VOLTAGE_UNITS;

	    // visit function
	    u.getFunction().accept(this);
	  }
	
	  public void visitCONSFunction(CONSFunction f)
	  {
	    summary.append("Function: CONS\n");
	    summary.append("Value: " +
			   engString(f.getValue(), units));
	  }

	  public void visitVAR1Function(VAR1Function f)
	  {
	    summary.append("Function: VAR1\n");

	    summary.append("Scale: ");
	    switch(f.getScale())
	    {
	    case VAR1Function.LIN_SCALE:
	      summary.append("LIN");
	      break;
	    case VAR1Function.LOG10_SCALE:
	      summary.append("LOG10");
	      break;
	    case VAR1Function.LOG25_SCALE:
	      summary.append("LOG25");
	      break;
	    case VAR1Function.LOG50_SCALE:
	      summary.append("LOG50");
	      break;
	    default:
	      throw new Error("impossible scale");
	    }
	    summary.append("\n");

	    summary.append("Start: " +
			   engString(f.getStart(), units) + "\n");
	    summary.append("Stop: " +
			   engString(f.getStop(), units) + "\n");

	    if (f.getScale() == VAR1Function.LIN_SCALE) {
	      summary.append("Step: " +
			     engString(f.getStep(), units) + "\n");
	    }

	    summary.append("Points: " + f.calculatePoints());
	  }

	  public void visitVAR2Function(VAR2Function f)
	  {
	    summary.append("Function: VAR2\n");
	    summary.append("Scale: Linear\n");
	    summary.append("Start: " +
			   engString(f.getStart(), units) + "\n");
	    summary.append("Stop: " +
			   engString(f.getStop(), units) + "\n");
	    summary.append("Step: " +
			   engString(f.getStep(), units) + "\n");
	    summary.append("Points: " + f.calculatePoints());
	  }

	  public void visitVAR1PFunction(VAR1PFunction f)
	  {
	    summary.append("Function: VAR1P\n");
	    summary.append("Ratio: " + f.getRatio()+ "\n");
	    summary.append("Offset: " +
			   engString(f.getOffset(), units));
	  }

	  // helper method for applying units to numerical values
	  private final String engString(BigDecimal val, EngUnits units)
	  {
	    return (new EngValue(val, units, NUM_SIGFIGS)).toString();
	  }
	}).go(myPort));
  }

} // end class PortLabel
