package weblab.client.graphicalUI;

import weblab.client.SCOPE;

import java.awt.*;
import javax.swing.*;
import java.awt.event.*;


/**
 * Label that represents a SCOPE
 */
public class SCOPELabel extends InstrumentLabel
{
  private SCOPE mySCOPE;
  private final SCOPEDialog mySCOPEDialog;

  public SCOPELabel(Frame theMainFrame, SCOPE mySCOPE)
  {
    super(theMainFrame, mySCOPE, "SCOPE");

    this.mySCOPE = mySCOPE;

    // create a SCOPE dialog that can be opened by clicking on this
    mySCOPEDialog = new SCOPEDialog(theMainFrame, mySCOPE);
    this.addMouseListener(new MouseAdapter() {
	public void mouseClicked(MouseEvent e) {
	  // Point location = new Point(e.getX()+10, e.getY()+10);
	  // mySMUDialog.setLocation(getMousePointerAbsolute(location));
	  mySCOPEDialog.setVisible(true);
	}
      });

    // display the hand cursor whenever the mouse is over this
    this.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));

    this.updateImage();
    this.updateToolTip();
  }


  protected String chooseImageName()
  {
    if (mySCOPE.getVName().equals("")) {
      // considered "unconfigured"
      return "instrument_SCOPE.gif";
    }
    else return chooseImageNameForVoltageOutputFunction(mySCOPE.getFunction());
  }

  public void cleanup()
  {
    mySCOPEDialog.dispose();    
  }

} // end class SCOPELabel
