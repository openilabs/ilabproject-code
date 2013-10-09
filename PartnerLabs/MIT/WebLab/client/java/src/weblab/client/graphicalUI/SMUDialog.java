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
 * PortDialog that allows user to change the configuration of an SMU.
 */
public class SMUDialog extends PortDialog
{
  private final SMU mySMU;


  /**
   * Creates a new SMUDialog.
   */
  public SMUDialog(Frame owner, SMU mySMU)
  {
    super(owner, mySMU);

    this.mySMU = mySMU;
  }



  protected void layoutDialog()
  {
    Container c = this.getContentPane();

    GridBagConstraints gbc = new GridBagConstraints();
    gbc.gridx = 0;
    gbc.gridwidth = GridBagConstraints.REMAINDER;

    addXNameRow("VName", vName, vDownload);
    addXNameRow("IName", iName, iDownload);

    addVSpace(3);
    addSeparator();
    addVSpace(3);

    addRow(c, "Mode", mode);

    gbc.fill = GridBagConstraints.HORIZONTAL;
    c.add(functionPanel, gbc);
    c.add(valuesPanel, gbc);
    c.add(compliancePanel, gbc);

    addVSpace(3);
    addSeparator();

    gbc.fill = GridBagConstraints.NONE;
    c.add(buttonPanel, gbc);

    this.pack();
  }



  protected void reconcile()
  {
    if (mode.getSelectedItem().equals("V"))
    {
      // V mode: show function and iCompliance, and set units to V

      complianceCardLayout.show(compliancePanel, "iCompliance");
      reconcileUnits(WeblabClient.VOLTAGE_UNITS);
      reconcileFunction(true);
    }
    else if (mode.getSelectedItem().equals("I"))
    {
      // I mode: show function and vCompliance, and set units to A

      complianceCardLayout.show(compliancePanel, "vCompliance");
      reconcileUnits(WeblabClient.CURRENT_UNITS);
      reconcileFunction(true);
    }
    else if (mode.getSelectedItem().equals("COMM"))
    {
      // COMM mode: don't show function or compliance

      complianceCardLayout.show(compliancePanel, "blank");
      reconcileFunction(false);
    }

    // repaint
    repaint();
  }



  protected void exportValues()
  {
    // export vname, vdownload, iname, idownload
    mySMU.setVName(vName.getText());
    mySMU.setVDownload(vDownload.isSelected());
    mySMU.setIName(iName.getText());
    mySMU.setIDownload(iDownload.isSelected());

    String mode_str = (String) mode.getSelectedItem();
    if (mode_str.equals("V"))
    {
      // export mode
      mySMU.setMode(SMU.V_MODE);

      // export source function
      mySMU.setFunction(exportFunction());

      // export iCompliance
      mySMU.setICompliance(iCompliance.getValue().toBigDecimal());
    }
    else if (mode_str.equals("I"))
    {
      // export mode
      mySMU.setMode(SMU.I_MODE);

      // export source function
      mySMU.setFunction(exportFunction());

      // export vCompliance
      mySMU.setVCompliance(vCompliance.getValue().toBigDecimal());
    }
    else if (mode_str.equals("COMM"))
    {
      // export mode
      mySMU.setMode(SMU.COMM_MODE);
    }

    // notify
    mySMU.notifyObservers();
  }

} // end class SMUDialog
