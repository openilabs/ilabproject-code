package weblab.client.graphicalUI;

import weblab.client.FGEN;

import java.awt.*;
import javax.swing.*;
import java.awt.event.*;


/**
 * Label that represents an FGEN
 */
public class FGENLabel extends InstrumentLabel
{
  private FGEN myFGEN;
  private final FGENDialog myFGENDialog;

  public FGENLabel(Frame theMainFrame, FGEN myFGEN)
  {
    super(theMainFrame, myFGEN, "FGEN");

    this.myFGEN = myFGEN;

    // create an FGEN dialog that can be opened by clicking on this
    myFGENDialog = new FGENDialog(theMainFrame, myFGEN);
    this.addMouseListener(new MouseAdapter() {
	public void mouseClicked(MouseEvent e) {
	  // Point location = new Point(e.getX()+10, e.getY()+10);
	  // mySMUDialog.setLocation(getMousePointerAbsolute(location));
	  myFGENDialog.setVisible(true);
	}
      });

    // display the hand cursor whenever the mouse is over this
    this.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));

    this.updateImage();
    this.updateToolTip();
  }


  protected String chooseImageName()
  {
    if (myFGEN.getVName().equals("")) {
      // considered "unconfigured"
      return "instrument_FGEN.gif";
    }
    else return chooseImageNameForVoltageSourceFunction(myFGEN.getFunction());
  }

  public void cleanup()
  {
    myFGENDialog.dispose();    
  }

} // end class FGENLabel
