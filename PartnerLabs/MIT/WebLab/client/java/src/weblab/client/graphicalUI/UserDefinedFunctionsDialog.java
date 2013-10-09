/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

import java.util.Observer;
import java.util.Observable;

import weblab.toolkit.util.Spacer;

import weblab.client.ExperimentSpecification;
import weblab.client.UserDefinedFunction;


public class UserDefinedFunctionsDialog extends JDialog
  implements ActionListener, Observer
{
  private ExperimentSpecification theExpSpec;

  private JPanel functionsPanel;
  private JButton addFunctionButton;

  private List udfDisplays;

  private static final int WIDTH = 500;
  private static final int HEIGHT = 250;



  public UserDefinedFunctionsDialog(Frame owner, ExperimentSpecification es)
  {
    // create a non-modal dialog
    super(owner, "User-Defined Functions", false);
    
    this.theExpSpec = es;
    this.udfDisplays = new ArrayList();

    // setup components

    addFunctionButton = new JButton("Add New Function");
    addFunctionButton.setActionCommand("add");

    JButton okButton = new JButton("OK");
    okButton.setActionCommand("ok");

    JButton applyButton = new JButton("Apply");
    applyButton.setActionCommand("apply");

    JButton cancelButton = new JButton("Cancel");
    cancelButton.setActionCommand("cancel");

    functionsPanel = new JPanel(new GridBagLayout());

    // layout button panel

    JPanel buttonPanel = new JPanel();
    buttonPanel.add(okButton);
    buttonPanel.add(applyButton);
    buttonPanel.add(cancelButton);

    // layout dialog

    Container c = this.getContentPane();
    c.setLayout(new GridBagLayout());
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.gridwidth = GridBagConstraints.REMAINDER;
    gbc.weightx = 1;

    gbc.fill = GridBagConstraints.BOTH;
    gbc.weighty = 1;
    // top, left, bottom, right
    gbc.insets = new Insets(3, 2, 0, 2);

    JScrollPane scrollPane = new JScrollPane(functionsPanel);
    scrollPane.setBorder(BorderFactory.createEmptyBorder());
    // make the scrollbar scroll at a reasonable rate
    scrollPane.getVerticalScrollBar().setUnitIncrement(10);
    c.add(scrollPane, gbc);

    gbc.fill = GridBagConstraints.HORIZONTAL;
    gbc.weighty = 0;
    // top, left, bottom, right
    gbc.insets = new Insets(1, 2, 1, 2);

    JSeparator js = new JSeparator();
    js.setMinimumSize(js.getPreferredSize());
    c.add(js, gbc);

    c.add(buttonPanel, gbc);

    this.setSize(new Dimension(WIDTH, HEIGHT));

    // set up event listeners

    addFunctionButton.addActionListener(this);
    okButton.addActionListener(this);
    applyButton.addActionListener(this);
    cancelButton.addActionListener(this);
    theExpSpec.addWeakReferenceObserver(this);

    // initialize

    revert();
  }



  /**
   * Changes the ExperimentSpecification to which this
   * UserDefinedFunctionsDialog is assigned.
   */
  public void setExperimentSpecification(ExperimentSpecification
					 newExperimentSpecification)
  {
    // stop observing the old ExpSpec, update our reference, and start
    // observing the new one
    theExpSpec.deleteObserver(this);
    theExpSpec = newExperimentSpecification;
    theExpSpec.addWeakReferenceObserver(this);

    // import data from the new ExpSpec
    this.revert();
  }



  /**
   * Handle ActionEvents
   */
  public final void actionPerformed(ActionEvent evt)
  {
    String cmd = evt.getActionCommand();

    // when Add is clicked, add a new UDFDisplay
    if (cmd.equals("add"))
    {
      udfDisplays.add(new UDFDisplay());
      updateFunctionsPanel();
    }

    // when OK is clicked, export values and then hide the dialog
    if (cmd.equals("ok"))
    {
      apply();
      revert(); // to remove extraneous blank UDFdisplays
      setVisible(false);
    }

    // when Apply is clicked, export values
    if (cmd.equals("apply"))
    {
      apply();
      revert(); // to remove extraneous blank UDFdisplays
    }

    // when Cancel is clicked, re-import values and then hide the dialog
    if (cmd.equals("cancel"))
    {
      revert();
      setVisible(false);
    }
  }



  /**
   * Handle changes in Observables
   */
  public final void update(Observable o, Object arg)
  {
    // when ExpSpec changes, re-import values
    revert();
  }



  private void updateFunctionsPanel()
  {
    GridBagConstraints gbc;

    // clear the functions panel of all components
    functionsPanel.removeAll();

    //
    // Add column labels and separator
    //

    gbc = new GridBagConstraints();
    // top, left, bottom, right
    gbc.insets = new Insets(0, 5, 0, 5);
    functionsPanel.add(new JLabel("Download"), gbc);
    functionsPanel.add(new JLabel("Name"), gbc);
    functionsPanel.add(new JLabel("Units"), gbc);
    // body should take up extra available horizontal space
    gbc.weightx = 1;
    functionsPanel.add(new JLabel("Body"), gbc);
    gbc.weightx = 0;
    // leave room for Delete buttons
    gbc.gridwidth = GridBagConstraints.REMAINDER;
    functionsPanel.add(Spacer.createHorizontalDoppelganger
		       (new JButton("Delete"), false), gbc);

    gbc.fill = GridBagConstraints.HORIZONTAL;
    // top, left, bottom, right
    gbc.insets = new Insets(2, 0, 3, 0);
    functionsPanel.add(new JSeparator(), gbc);
    gbc.fill = GridBagConstraints.NONE;

    //
    // Add a row for each UDFDisplay in udfDisplays
    //

    // top, left, bottom, right
    gbc.insets = new Insets(2, 2, 2, 2);

    for (Iterator i = udfDisplays.iterator(); i.hasNext(); )
    {
      UDFDisplay d = (UDFDisplay) i.next();

      gbc.gridwidth = 1;

      functionsPanel.add(d.download, gbc);
      functionsPanel.add(d.name, gbc);
      functionsPanel.add(d.units, gbc);
	
      // body should take up extra available horizontal space
      gbc.fill = GridBagConstraints.HORIZONTAL;
      gbc.weightx = 1;
      functionsPanel.add(d.body, gbc);
      gbc.fill = GridBagConstraints.NONE;
      gbc.weightx = 0;
	
      // deleteButton should be the end of a row
      gbc.gridwidth = GridBagConstraints.REMAINDER;
	
      functionsPanel.add(d.deleteButton, gbc);      
    }

    // Add button to add a new function
    gbc.anchor = GridBagConstraints.WEST;
    functionsPanel.add(addFunctionButton, gbc);

    // Add a blank row to take up the remaining vertical space (so
    // that the other components will appear top-aligned rather than
    // vertically centered)
    gbc = new GridBagConstraints();
    gbc.weighty = 1.0;
    functionsPanel.add(new JPanel(), gbc);

    // revalidate (whole dialog box instead of just functionsPanel
    // because scrollPane may also need it)
    this.validate();
    this.repaint();
  }



  private void apply()
  {
    // generate new set of UDFs from UDFDisplays
    List newUDFs = new ArrayList();
    for (Iterator i = udfDisplays.iterator(); i.hasNext(); )
    {
      UDFDisplay next = (UDFDisplay) i.next();

      UserDefinedFunction u = next.toUserDefinedFunction();

      if (! u.getName().equals(""))
	newUDFs.add(u);
    }

    // update expSpec's UDFs to match these
    theExpSpec.setUserDefinedFunctions(newUDFs);

    // notify other observers that expSpec may have changed
    theExpSpec.notifyObservers();
  }



  public void revert()
  {
    // clear existing UDFDisplays
    udfDisplays.clear();

    // add a UDFDisplay for each udf in expSpec
    for (Iterator i = theExpSpec.getUserDefinedFunctions();
	 i.hasNext(); )
    {
      udfDisplays.add
	(new UDFDisplay((UserDefinedFunction) i.next()));
    }

    // update functions panel
    updateFunctionsPanel();
  }



  private class UDFDisplay implements ActionListener
  {
    JTextField name, units, body;
    JCheckBox download;
    JButton deleteButton;

    public UDFDisplay()
    {
      this.name = new JTextField(6);
      this.units = new JTextField(6);
      this.body = new JTextField(20);
      this.download = new JCheckBox();

      deleteButton = new JButton("Delete");
      deleteButton.addActionListener(this);
    }

    public UDFDisplay(UserDefinedFunction udf)
    {
      this();

      this.name.setText(udf.getName());
      this.units.setText(udf.getUnits());
      this.body.setText(udf.getBody());
      this.download.setSelected(udf.getDownload());
    }

    public final void actionPerformed(ActionEvent evt)
    {
      // when Delete is clicked, remove this from the dialog
      udfDisplays.remove(this);
      updateFunctionsPanel();
    }

    public UserDefinedFunction toUserDefinedFunction()
    {
      return new UserDefinedFunction
	(name.getText(), download.isSelected(),
	 units.getText(), body.getText());
    }
  } // end inner class UDFDisplay

} // end class UserDefinedFunctionsDialog
