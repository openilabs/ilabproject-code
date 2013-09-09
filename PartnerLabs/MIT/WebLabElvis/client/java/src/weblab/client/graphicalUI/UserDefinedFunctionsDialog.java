package weblab.client.graphicalUI;

import weblab.client.WeblabClient;
import weblab.client.ExperimentSpecification;
import weblab.client.UserDefinedFunction;

import weblab.util.ChangeTrackingObservable;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

import java.util.Collection;
import java.util.List;
import java.util.Set;
import java.util.Iterator;

import java.util.ArrayList;
import java.util.HashSet;

import java.util.Observer;
import java.util.Observable;
import java.util.Enumeration;


public class UserDefinedFunctionsDialog extends JDialog implements Observer
{
  private WeblabClient theWeblabClient;
  private ExperimentSpecification theExpSpec;

  private JPanel functionsPanel;

  private List udfDisplays;

  private final int WIDTH = 500;
  private final int HEIGHT = 200;

  /////////////
  // Actions //
  /////////////

  private final Action APPLY = new AbstractAction("Apply")
    {
      public void actionPerformed(ActionEvent evt)
      {
	// namesToRemove holds names of UDFs that should be removed as
	// part of this action.  Initial value is the set of all UDF
	// names currently present in the analyzer.
	Set namesToRemove = new HashSet();
	Enumeration names_enum = theExpSpec.getUserDefinedFunctionNames();
	while (names_enum.hasMoreElements())
	{
	  namesToRemove.add(names_enum.nextElement());
	}

	// for each UDFDisplay in the dialog box...
	for (Iterator i = udfDisplays.iterator(); i.hasNext();) {
	  UDFDisplay next = (UDFDisplay)i.next();
	  // export field values to a new UDF
	  UserDefinedFunction newUDF = next.toUserDefinedFunction();
	  // if the name isn't empty...
	  if (! newUDF.getName().equals("")) {
	    // add this UDF to the analyzer (or overwrite an existing
	    // one of the same name)
	    theExpSpec.addUserDefinedFunction(newUDF);
	    // do NOT remove this UDF name from the analyzer
	    namesToRemove.remove(newUDF.getName());
	  }
	}

	// remove any UDFs from the analyzer that are not defined in
	// the dialog box
	for (Iterator i = namesToRemove.iterator(); i.hasNext();) {
	  theExpSpec.removeUserDefinedFunction((String)i.next());
	}

	// notify other observers that expSpec may have changed
	theExpSpec.notifyObservers();

	// make sure this dialog is up to date (even if no actual
	// changes were made to the set of UDFs on the ExpSpec)
	REVERT.actionPerformed(evt);
      }
    };

  private final Action REVERT = new AbstractAction("Revert")
    {
      public void actionPerformed(ActionEvent evt)
      {
	// clear existing UDFDisplays
	udfDisplays.clear();

	// add a UDFDisplay for each udf in the analyzer
	Enumeration udfs_enum = theExpSpec.getUserDefinedFunctions();
	while (udfs_enum.hasMoreElements())
	{
	  udfDisplays.add
	    (new UDFDisplay((UserDefinedFunction) udfs_enum.nextElement()));
	}

	// update functions panel
	updateFunctionsPanel();
      }
    };

  private final Action OK = new AbstractAction("OK")
    {
      public void actionPerformed(ActionEvent evt)
      {
	APPLY.actionPerformed(evt);
	setVisible(false);
      }
    };

  private final Action CANCEL = new AbstractAction("Cancel")
    {
      public void actionPerformed(ActionEvent evt)
      {
	REVERT.actionPerformed(evt);
	setVisible(false);
      }
    };

  /////////////////
  // Constructor //
  /////////////////

  public UserDefinedFunctionsDialog(Frame owner, WeblabClient wc)
  {
    // create a non-modal dialog
    super(owner, "User-Defined Functions", false);
    
    GridBagConstraints gbc;

    this.theWeblabClient = wc;
    this.theExpSpec = theWeblabClient.getExperimentSpecification();

    this.udfDisplays = new ArrayList();

    JButton addButton = new JButton("Add new function");
    addButton.addActionListener(new ActionListener() {
	public void actionPerformed(ActionEvent evt)
	{
	  udfDisplays.add(new UDFDisplay(new UserDefinedFunction()));
	  updateFunctionsPanel();
	}
      });

    functionsPanel = new JPanel(new GridBagLayout());

    // observe
    theWeblabClient.addObserver(this);
    theExpSpec.addObserver(this);


    JScrollPane scrollPane = new JScrollPane(functionsPanel);
    scrollPane.setPreferredSize(new Dimension(WIDTH, HEIGHT));

    JPanel buttonPanel = new JPanel();
    buttonPanel.setLayout(new BoxLayout(buttonPanel, BoxLayout.X_AXIS));
    buttonPanel.add(Box.createHorizontalGlue());
    buttonPanel.add(addButton);
    buttonPanel.add(Box.createHorizontalGlue());
    buttonPanel.add(new JButton(OK));
    buttonPanel.add(new JButton(APPLY));
    //    buttonPanel.add(new JButton(REVERT));
    buttonPanel.add(new JButton(CANCEL));
    buttonPanel.add(Box.createHorizontalGlue());

    // layout dialog
    Container c = this.getContentPane();
    c.setLayout(new BorderLayout());
    c.add(scrollPane, BorderLayout.CENTER);
    c.add(buttonPanel, BorderLayout.SOUTH);

    this.pack();

    // initialize
    REVERT.actionPerformed(null);
  }



  /**
   * Handle changes in Observables
   */
  public final void update(Observable o, Object arg)
  {
    if (o == theWeblabClient && ChangeTrackingObservable.containsChange
	(arg, WeblabClient.EXPERIMENT_SPECIFICATION_CHANGE))
    {
      theExpSpec.deleteObserver(this);
      theExpSpec = theWeblabClient.getExperimentSpecification();
      theExpSpec.addObserver(this);

      REVERT.actionPerformed(null);
    }

    // when ExpSpec changes, update UDFs
    if (o == theExpSpec)
    {
      REVERT.actionPerformed(null);
    }
  }



  private void updateFunctionsPanel()
  {
    GridBagConstraints gbc;

    // clear the functions panel of all components
    functionsPanel.removeAll();

    if (udfDisplays.isEmpty()) {
      // if no udfs, just display a centered label
      functionsPanel.add(new JLabel("(no functions defined)"));
    }
    else {
      // Add column labels
      gbc = new GridBagConstraints();
      gbc.anchor = GridBagConstraints.WEST;
      // top, left, bottom, right
      gbc.insets = new Insets(0, 5, 0, 5);
      functionsPanel.add(new JLabel("Download"), gbc);
      functionsPanel.add(new JLabel("Name"), gbc);
      functionsPanel.add(new JLabel("Units"), gbc);
      gbc.gridwidth = GridBagConstraints.REMAINDER;
      functionsPanel.add(new JLabel("Body"), gbc);
      
      // Add a row for each UDFDisplay in udfDisplays
      for (Iterator i = udfDisplays.iterator(); i.hasNext();) {
	UDFDisplay d = (UDFDisplay)i.next();
	
	gbc = new GridBagConstraints();
	// top, left, bottom, right
	gbc.insets = new Insets(2, 2, 2, 2);
	
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

      // Add a blank row to take up the remaining vertical space (so
      // that the other components will appear top-aligned rather than
      // vertically centered)
      gbc = new GridBagConstraints();
      gbc.weighty = 1.0;
      functionsPanel.add(new JLabel(), gbc);
    }

    // revalidate and repaint
    functionsPanel.revalidate();
    functionsPanel.repaint();
  }

  private class UDFDisplay
  {
    UDFDisplay self;

    JTextField name, units, body;
    JCheckBox download;
    JButton deleteButton;

    public UDFDisplay(UserDefinedFunction udf)
    {
      this();

      this.name.setText(udf.getName());
      this.units.setText(udf.getUnits());
      this.body.setText(udf.getBody());
      this.download.setSelected(udf.getDownload());
    }

    public UDFDisplay()
    {
      this.self = this;

      this.name = new JTextField(6);
      this.units = new JTextField(6);
      this.body = new JTextField(20);
      this.download = new JCheckBox();

      deleteButton = new JButton("Delete");
      deleteButton.addActionListener(new ActionListener() {
	  public void actionPerformed(ActionEvent evt)
	  {
	    udfDisplays.remove(self);
	    updateFunctionsPanel();
	  }
	});
    }

    public UserDefinedFunction toUserDefinedFunction()
    {
      return new UserDefinedFunction
	(name.getText(), download.isSelected(),
	 units.getText(), body.getText());
    }
  } // end inner class UDFPanel

} // end class UserDefinedFunctionsDialog
