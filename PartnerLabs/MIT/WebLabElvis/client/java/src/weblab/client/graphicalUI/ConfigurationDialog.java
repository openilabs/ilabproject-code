package weblab.client.graphicalUI;

import weblab.client.WeblabClient;
import weblab.client.serverInterface.ServerException;
import weblab.client.ConfirmationRequest;
import weblab.client.InvalidExperimentSpecificationException;

import javax.swing.*;
import javax.swing.event.*;
import java.awt.*;
import java.awt.event.*;

import java.util.Enumeration;

/**
 * This dialog is used to load, save, and delete experiment configurations.
 */
public class ConfigurationDialog extends JDialog implements ActionListener
{
  WeblabClient theWeblabClient;
  
  JList configurationList;
  JTextField configurationTextField;

  // function is "Save", "Load", or "Delete"
  public ConfigurationDialog(Frame owner, WeblabClient wc, String function)
  {
    // title = function + " Configuration"
    super(owner, function + " Configuration", true);

    theWeblabClient = wc;

    // initialize elements

    DefaultListModel configurationListModel = new DefaultListModel();
    try
    {
      Enumeration names_enum = theWeblabClient.getSavedExpConfigurationNames();
      while(names_enum.hasMoreElements())
      {
    	  configurationListModel.addElement((String) names_enum.nextElement());
      }
    }
    catch (ServerException e)
    {
      String error = e.getMessage();
      e.printStackTrace();
      JOptionPane.showMessageDialog(owner, error, "Error",
				    JOptionPane.ERROR_MESSAGE);
      this.dispose();
    }

    configurationList = new JList(configurationListModel);
    configurationList.setVisibleRowCount(10);
    // use prototype to set fixed cell height so that "" will be
    // rendered correctly (otherwise "" gets rendered as a very thin
    // horizontal line that's almost impossible to notice)
    configurationList.setPrototypeCellValue("a nice long name for a saved configuration");
    // when the user selects something, put it in the text field
    configurationList.addListSelectionListener(new ListSelectionListener() {
	public void valueChanged(ListSelectionEvent event) {
	  if (! configurationList.isSelectionEmpty()) {
	    configurationTextField.setText((String)configurationList.getSelectedValue());
	  }
	}
      });

    JScrollPane configurationScrollPane= new JScrollPane(configurationList);

    configurationTextField = new JTextField(20); // 20 columns
    // editable only for "Save" function, not for "Load" or "Delete"
    if (function.equals("Save")) {
      configurationTextField.setEditable(true);
    }
    else {
      configurationTextField.setEditable(false);
    }

    JButton functionButton = new JButton(function);
    functionButton.setActionCommand(function);
    functionButton.addActionListener(this);

    JButton cancelButton = new JButton("Cancel");
    cancelButton.setActionCommand("Cancel");
    cancelButton.addActionListener(this);

    // layout elements

    JPanel buttonPanel = new JPanel();
    buttonPanel.setLayout(new GridLayout(2,1));
    buttonPanel.add(functionButton);
    buttonPanel.add(cancelButton);

    JPanel configurationNamePanel = new JPanel();
    configurationNamePanel.setLayout(new FlowLayout());
    configurationNamePanel.add(new JLabel("Configuration name:"));
    configurationNamePanel.add(configurationTextField);

    JPanel bottomPanel = new JPanel();
    bottomPanel.setLayout(new FlowLayout());
    bottomPanel.add(configurationNamePanel);
    bottomPanel.add(buttonPanel);

    JPanel topPanel = new JPanel();
    topPanel.setLayout(new FlowLayout());
    topPanel.add(new JLabel("Available configurations:"));

    Container c = this.getContentPane();
    c.setLayout(new BorderLayout());
    c.add(BorderLayout.NORTH, topPanel);
    c.add(BorderLayout.CENTER, configurationScrollPane);
    c.add(BorderLayout.SOUTH, bottomPanel);

    this.pack();
  }

  public void actionPerformed(ActionEvent event)
  {
    if (event.getActionCommand().equals("Save")) {
      String name = configurationTextField.getText();
      if (name != null) {
	try {
	  theWeblabClient.saveExpConfiguration(name);
	  this.dispose();
	}
	catch (ServerException e) {
	  String error = "Server error: " + e.getMessage();
	  e.printStackTrace();
	  JOptionPane.showMessageDialog(this, error, "Error",
					JOptionPane.ERROR_MESSAGE);
	}
      }
    }
    else if (event.getActionCommand().equals("Load")) {
      String name = configurationTextField.getText();
      if (name != null)
      {
	try {
	  try {
	    theWeblabClient.loadExpConfiguration(name, true);
	    this.dispose();
	  }
	  catch (ConfirmationRequest cr)
	  {
	    String message =
	      "The configuration you have chosen defines instruments that are not connected to the currently selected experiment configuration.\nIf you choose to load the configuration anyway, this information will be lost.\nLoad this configuration anyway?";
	    boolean confirmed = 
	      (JOptionPane.showConfirmDialog
	       (this, message, "Warning", JOptionPane.YES_NO_OPTION,
		JOptionPane.WARNING_MESSAGE)
	       == JOptionPane.YES_OPTION);
	    // (owner, message, "Warning", "Load the configuration anyway", "Cancel");
	    if (confirmed)
	    {
	      theWeblabClient.loadExpConfiguration(name, false);
	      this.dispose();
	    }
	  }
	}
	catch (ServerException e) {
	  String error = "Server error: " + e.getMessage();
	  e.printStackTrace();
	  JOptionPane.showMessageDialog(this, error, "Error",
					JOptionPane.ERROR_MESSAGE);
	}
	catch (InvalidExperimentSpecificationException e) {
	  String error = "Invalid experiment specification: " + e.getMessage();
	  e.printStackTrace();
	  JOptionPane.showMessageDialog(this, error, "Error",
					JOptionPane.ERROR_MESSAGE);
	}
      }
    }
    else if (event.getActionCommand().equals("Delete")) {
      String name = configurationTextField.getText();
      if (name != null) {
	try {
	  theWeblabClient.deleteExpConfiguration(name);
	  this.dispose();
	}
	catch (ServerException e) {
	  String error = "Server error: " + e.getMessage();
	  e.printStackTrace();
	  JOptionPane.showMessageDialog(this, error, "Error",
					JOptionPane.ERROR_MESSAGE);
	}
      }
    }
    else if (event.getActionCommand().equals("Cancel")) {
      this.dispose();
    }
  }

} // end class ConfigurationDialog
