/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import javax.swing.event.*;

import java.util.Iterator;

import weblab.toolkit.util.ConvenientDialog;

import weblab.toolkit.serverInterface.ServerException;

import weblab.client.WeblabClient;
import weblab.client.Experiment;
import weblab.client.ConfirmationRequest;
import weblab.client.InvalidExperimentSpecificationException;

/**
 * Allows the user to load, save, or delete setups (but only one
 * operation at a time).  Created by MainFrame.
 *
 * SetupDialogs are disposable (create, use once, get rid of it) so
 * they do not need to keep track of changes in the WeblabClient or
 * Experiment.
 */
public class SetupDialog extends JDialog
  implements ActionListener, ListSelectionListener, WindowListener, KeyListener
{
  private Frame theMainFrame;
  private WeblabClient theWeblabClient;
  private Experiment theExperiment;
  
  private JList setupList;
  private JTextField setupTextField;


  // function is "Save", "Load", or "Delete"
  public SetupDialog(MainFrame theMainFrame, WeblabClient wc, Experiment exp,
		     String function, String initialNameText)
  {
    this(theMainFrame, wc, exp, function);
    setupTextField.setText(initialNameText);
  }



  // function is "Save", "Load", or "Delete"
  public SetupDialog(Frame theMainFrame, WeblabClient wc, Experiment exp,
		     String function)
  {
    // title = function + " Setup"
    super(theMainFrame, function + " Setup", true);

    if (! (function.equals("Save") ||
	   function.equals("Load") ||
	   function.equals("Delete")))
      throw new Error("illegal SetupDialog function: " + function);

    this.theMainFrame = theMainFrame;
    theWeblabClient = wc;
    theExperiment = exp;

    // initialize elements

    DefaultListModel setupListModel = new DefaultListModel();
    try
    {
      for (Iterator i = theWeblabClient.getSetupNames(); i.hasNext(); )
      {
	setupListModel.addElement((String) i.next());
      }
    }
    catch (ServerException ex)
    {
      ex.printStackTrace();
      ConvenientDialog.showExceptionDialog(theMainFrame, ex);
      this.dispose();
    }

    setupList = new JList(setupListModel);
    setupList.setVisibleRowCount(10);
    setupList.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
    // use prototype to set fixed cell height so that "" will be
    // rendered correctly (otherwise "" gets rendered as a very thin
    // horizontal line that's almost impossible to notice)
    setupList.setPrototypeCellValue("a nice long name for a saved setup");
    setupList.addListSelectionListener(this);

    JScrollPane setupScrollPane = new JScrollPane(setupList);

    setupTextField = new JTextField(20); // 20 columns
    // editable only for "Save" function, not for "Load" or "Delete"
    if (function.equals("Save")) {
      setupTextField.setEditable(true);
      setupTextField.addKeyListener(this);
    }
    else {
      setupTextField.setEditable(false);
    }

    JButton functionButton = new JButton(function);
    functionButton.setActionCommand(function);
    functionButton.addActionListener(this);

    JButton cancelButton = new JButton("Cancel");
    cancelButton.setActionCommand("Cancel");
    cancelButton.addActionListener(this);

    // layout elements

    Container c = this.getContentPane();
    c.setLayout(new GridBagLayout());

    GridBagConstraints gbc = new GridBagConstraints();
    gbc.insets = new Insets(1, 2, 1, 2); // top, left, bottom, right
    gbc.gridx = 0;
    gbc.gridy = 0;
    gbc.gridwidth = 3;

    c.add(new JLabel("Available setups:"), gbc);

    gbc.gridy = 1;
    gbc.weighty = 1.0;
    gbc.fill = GridBagConstraints.HORIZONTAL;
    c.add(setupScrollPane, gbc);

    gbc.gridy = 2;
    gbc.weighty = 0.0;
    gbc.fill = GridBagConstraints.NONE;
    gbc.gridwidth = 1;
    gbc.gridheight = 2;
    c.add(new JLabel("Setup name:"), gbc);

    gbc.gridx = 1;
    gbc.weightx = 1.0;
    gbc.fill = GridBagConstraints.HORIZONTAL;
    c.add(setupTextField, gbc);

    gbc.gridx = 2;
    gbc.weightx = 0.0;
    gbc.gridheight = 1;
    c.add(functionButton, gbc);

    gbc.gridy = 3;
    c.add(cancelButton, gbc);

    this.pack();

    this.addWindowListener(this);
  }



  //////////////////////
  // Listener methods //
  //////////////////////


  /**
   * When this is first displayed, if setupTextField is editable
   * (i.e. if this is a Save dialog), select the contents of
   * setupTextField and give it the keyboard focus.
   */
  public final void windowOpened(WindowEvent evt)
  {
    if (setupTextField.isEditable())
    {
      setupTextField.selectAll();
      setupTextField.requestFocus();
    }
  }

  // obligatory irrelevant methods of WindowListener
  public final void windowClosing(WindowEvent evt) {}
  public final void windowClosed(WindowEvent evt) {}
  public final void windowIconified(WindowEvent evt) {}
  public final void windowDeiconified(WindowEvent evt) {}
  public final void windowActivated(WindowEvent evt) {}
  public final void windowDeactivated(WindowEvent evt) {}



  /**
   * When user types in the text field, clear List selection.
   */
  public final void keyTyped(KeyEvent evt)
  {
    setupList.clearSelection();
  }

  // obligatory irrelevant methods of KeyListener
  public final void keyPressed(KeyEvent evt) {}
  public final void keyReleased(KeyEvent evt) {}



  /**
   * When the user selects a setup name from the list, put it in the
   * text field.  If the text field is editable (i.e. if this is a
   * Save dialog), also select the contents of the text field and give
   * it the input focus.
   */
  public void valueChanged(ListSelectionEvent event)
  {
    if (! setupList.isSelectionEmpty())
    {
      setupTextField.setText((String) setupList.getSelectedValue());

      if (setupTextField.isEditable())
      {
	setupTextField.selectAll();
	setupTextField.requestFocus();
      }
    }
  }



  /**
   * Handle ActionEvents
   */
  public void actionPerformed(ActionEvent event)
  {
    String cmd = event.getActionCommand();

    if (cmd.equals("Save"))
    {
      String name = setupTextField.getText();

      // don't allow saving a setup named ""
      if (! name.equals(""))
      {
	try
	{
	  theWeblabClient.saveSetup(theExperiment, name);
	  this.dispose();
	}
	catch (ServerException e) {
	  e.printStackTrace();
	  ConvenientDialog.showExceptionDialog(theMainFrame, e);
	}
      }
    }
    else if (cmd.equals("Load"))
    {
      String name = (String) setupList.getSelectedValue();
      if (name != null)
      {
	try
	{
	  try
	  {
	    theWeblabClient.loadSetup(theExperiment, name, true);
	    this.dispose();
	  }
	  catch (ConfirmationRequest cr)
	  {
	    String message =
	      "The setup you have chosen defines ports that are not connected to the currently selected device.\nIf you choose to load the setup anyway, the setup information for these ports will be discarded.";
	    boolean confirmed = ConvenientDialog.showConfirmDialog
	      (theMainFrame, message, "Warning",
	       "Load the setup anyway", "Cancel");
	    if (confirmed)
	    {
	      theWeblabClient.loadSetup(theExperiment, name, false);
	      this.dispose();
	    }
	  }
	}
	catch (ServerException e) {
	  e.printStackTrace();
	  ConvenientDialog.showExceptionDialog(theMainFrame, e);
	}
	catch (InvalidExperimentSpecificationException e) {
	  e.printStackTrace();
	  ConvenientDialog.showExceptionDialog(theMainFrame, e);
	}
      }
    }
    else if (cmd.equals("Delete"))
    {
      String name = (String) setupList.getSelectedValue();
      if (name != null)
      {
	try
	{
	  theWeblabClient.deleteSetup(name);
	  this.dispose();
	}
	catch (ServerException e) {
	  e.printStackTrace();
	  ConvenientDialog.showExceptionDialog(theMainFrame, e);
	}
      }
    }
    else if (cmd.equals("Cancel"))
    {
      this.dispose();
    }
  }

} // end class SetupDialog
