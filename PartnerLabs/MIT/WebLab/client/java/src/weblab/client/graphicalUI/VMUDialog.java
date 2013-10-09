/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

import weblab.client.*;

/**
 * PortDialog that allows user to change the configuration of a VMU.
 */
public class VMUDialog extends PortDialog
{
  private final VMU myVMU;

  /**
   * Creates a new VMUDialog.
   */
  public VMUDialog(Frame owner, VMU myVMU)
  {
    super(owner, myVMU);

    this.myVMU = myVMU;
  }



  protected void layoutDialog()
  {
    Container c = this.getContentPane();

    GridBagConstraints gbc = new GridBagConstraints();
    gbc.gridx = 0;
    gbc.gridwidth = GridBagConstraints.REMAINDER;

    addXNameRow("VName", vName, vDownload);

    addVSpace(3);
    addSeparator();

    c.add(buttonPanel, gbc);

    this.pack();
  }



  protected void reconcile()
  {
    // do nothing
  }



  protected void exportValues()
  {
    // export vname, vdownload
    myVMU.setVName(vName.getText());
    myVMU.setVDownload(vDownload.isSelected());

    // mode removed from VMUDialog, since Mode is always V  <dmrz>
    /*
    // export mode
    String mode_str = (String) mode.getSelectedItem();
    if (mode_str.equals("V"))
      myVMU.setMode(VMU.V_MODE);
    else
      throw new Error("illegal mode choice: " + mode_str);
    */

    // notify
    myVMU.notifyObservers();
  }

} // end class VMUDialog
