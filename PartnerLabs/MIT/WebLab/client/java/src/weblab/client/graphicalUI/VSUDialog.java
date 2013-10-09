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
 * PortDialog that allows user to change the configuration of a VSU.
 */
public class VSUDialog extends PortDialog
{
  private final VSU myVSU;


  /**
   * Creates a new VSUDialog.
   */
  public VSUDialog(Frame owner, VSU myVSU)
  {
    super(owner, myVSU);

    this.myVSU = myVSU;
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
    addVSpace(3);

    gbc.fill = GridBagConstraints.HORIZONTAL;
    c.add(functionPanel, gbc);
    c.add(valuesPanel, gbc);

    addVSpace(3);
    addSeparator();

    gbc.fill = GridBagConstraints.NONE;
    c.add(buttonPanel, gbc);

    this.pack();
  }



  protected void reconcile()
  {
    // show function, and set units to V

    reconcileUnits(WeblabClient.VOLTAGE_UNITS);
    reconcileFunction(true);

    // repaint
    repaint();
  }



  protected void exportValues()
  {
    // export vname, vdownload, iname, idownload
    myVSU.setVName(vName.getText());
    myVSU.setVDownload(vDownload.isSelected());

    // export source function
    myVSU.setFunction(exportFunction());

    // notify
    myVSU.notifyObservers();
  }

} // end class VSUDialog
