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

import java.util.List;
import java.util.SortedSet;
import java.util.Iterator;
import java.util.Comparator;
import java.util.ArrayList;
import java.util.TreeSet;

import java.text.DateFormat;

import weblab.toolkit.util.ConvenientDialog;

import weblab.toolkit.serverInterface.ExperimentInformation;
import weblab.toolkit.serverInterface.ServerException;

import weblab.client.WeblabClient;
import weblab.client.Experiment;
import weblab.client.InvalidLabConfigurationException;
import weblab.client.InvalidExperimentSpecificationException;
import weblab.client.InvalidExperimentResultException;

/**
 * XXX spec
 * Allows the user to load, save, or delete setups (but only one
 * operation at a time).  Created by MainFrame.
 *
 * LoadExperimentDialogs are disposable (create, use once, get rid of it) so
 * they do not need to keep track of changes in the WeblabClient or
 * Experiment.
 */
public class LoadExperimentDialog extends JDialog
  implements ActionListener, ListSelectionListener, KeyListener
{
  private MainFrame theMainFrame;
  private WeblabClient theWeblabClient;
  private ExperimentTab targetTab;
  
  private JList expList;
  private DefaultListModel expListModel;

  private JTextField expIDTextField;

  private Experiment experiment; // the Experiment loaded by the user, or null

  private static DateFormat dateFormat =
    DateFormat.getDateTimeInstance(DateFormat.SHORT, DateFormat.MEDIUM);


  public LoadExperimentDialog(MainFrame theMainFrame, ExperimentTab targetTab)
  {
    //XXX title = function + " Setup"
    super(theMainFrame, "Load Experiment", true);

    this.theMainFrame = theMainFrame;
    theWeblabClient = theMainFrame.getWeblabClient();
    this.targetTab = targetTab;

    experiment = null;

    // initialize elements

    expListModel = new DefaultListModel();

    expList = new JList(expListModel);
    expList.setVisibleRowCount(10);
    expList.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
    // use prototype to set fixed cell height so that "" will be
    // rendered correctly (otherwise "" gets rendered as a very thin
    // horizontal line that's almost impossible to notice)
    expList.setPrototypeCellValue("a nice long name for a list item string");
    expList.addListSelectionListener(this);

    // populate expList in a background thread (may take some time)
    Thread populateThread = new Thread(new Runnable () {
	public final void run()
	{
	  setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));
	  populateExpList();
	  setCursor(null);
	}
      });
    populateThread.start();

    JScrollPane scrollPane = new JScrollPane(expList);

    expIDTextField = new JTextField(20); // 20 columns

    //XXX
    // editable only for "Save" function, not for "Load" or "Delete"
    /*
    if (function.equals("Save")) {
      expIDTextField.setEditable(true);
      expIDTextField.addKeyListener(this);
    }
    else {
      expIDTextField.setEditable(false);
    }
    */

    // allow user to enter own ExpIDs not in list (e.g. from the My
    // Experiments page)
    expIDTextField.setEditable(true);
    expIDTextField.addKeyListener(this);

    JButton functionButton = new JButton("Load");
    functionButton.setActionCommand("Load");
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

    c.add(new JLabel("Recent experiments:"), gbc);

    gbc.gridy = 1;
    gbc.weighty = 1.0;
    gbc.fill = GridBagConstraints.HORIZONTAL;
    c.add(scrollPane, gbc);

    gbc.gridy = 2;
    gbc.weighty = 0.0;
    gbc.fill = GridBagConstraints.NONE;
    gbc.gridwidth = 1;
    gbc.gridheight = 2;
    c.add(new JLabel("Experiment ID:"), gbc);

    gbc.gridx = 1;
    gbc.weightx = 1.0;
    gbc.fill = GridBagConstraints.HORIZONTAL;
    c.add(expIDTextField, gbc);

    gbc.gridx = 2;
    gbc.weightx = 0.0;
    gbc.gridheight = 1;
    c.add(functionButton, gbc);

    gbc.gridy = 3;
    c.add(cancelButton, gbc);

    this.pack();
  }



  /**
   * Displays this dialog to allow the user to load an experiment.  If
   * an Experiment is successfully loaded, returns it, otherwise
   * returns null.
   */
  public Experiment loadExperiment()
  {
    this.setVisible(true);
    return experiment;
  }



  private void populateExpList()
  {
    try
    {
      expList.setEnabled(false);
      expListModel.addElement(" loading, please wait...");

      List experimentIDs = new ArrayList();
      //XXX experimentIDs.addAll(theWeblabClient.getAnnotatedExperimentIDs());
      experimentIDs.addAll(theWeblabClient.getRecentExperimentIDs());

      // get ExperimentInformations and sort in REVERSE order of
      // submissionTime (breaking ties by reverse order of
      // experimentID, then by hashCode)
      SortedSet experimentInformations = new TreeSet
	(new Comparator() {
	    public int compare(Object o1, Object o2)
	    {
	      ExperimentInformation ei1 = (ExperimentInformation) o1;
	      ExperimentInformation ei2 = (ExperimentInformation) o2;

	      int answer = ei2.submissionTime.compareTo(ei1.submissionTime);

	      if (answer == 0)
		answer = ei2.experimentID.compareTo(ei1.experimentID);

	      if (answer == 0)
		answer = ei1.hashCode() - ei2.hashCode();

	      return answer;
	    }
	  });
      experimentInformations.addAll
	(theWeblabClient.getExperimentInformation(experimentIDs));

      for (Iterator i = experimentInformations.iterator(); i.hasNext(); )
      {
	expListModel.addElement
	  (new ExperimentInformationDisplay
	   ((ExperimentInformation) i.next()));
      }

      expListModel.removeElementAt(0);
      expList.setEnabled(true);
    }
    catch (ServerException ex)
    {
      ex.printStackTrace();
      ConvenientDialog.showExceptionDialog(theMainFrame, ex);
      this.dispose();
    }
  }



  //////////////////////
  // Listener methods //
  //////////////////////


  /**
   * When user types in the text field, clear List selection.
   */
  public final void keyTyped(KeyEvent evt)
  {
    expList.clearSelection();
  }

  // obligatory irrelevant methods of KeyListener
  public final void keyPressed(KeyEvent evt) {}
  public final void keyReleased(KeyEvent evt) {}



  /**
   * When the user selects an experiment from the list, put its expID
   * in the text field.
   */
  public void valueChanged(ListSelectionEvent event)
  {
    if (! expList.isSelectionEmpty())
    {
      expIDTextField.setText
	(((ExperimentInformationDisplay) expList.getSelectedValue())
	 .theExperimentInformation.experimentID.toString());
    }
  }



  /**
   * Handle ActionEvents
   */
  public void actionPerformed(ActionEvent event)
  {
    String cmd = event.getActionCommand();

    if (cmd.equals("Load"))
    {
      try
      {
	int expID = Integer.parseInt(expIDTextField.getText());

	setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));

	try
	{
	  this.experiment = null;

	  // check to see if we already have this experiment loaded
	  for (Iterator i = theMainFrame.getRecentExperiments().iterator();
	       i.hasNext(); )
	  {
	    Experiment e = (Experiment) i.next();
	    if (e.getExperimentID().intValue() == expID)
	    {
	      this.experiment = e;
	      break;
	    }
	  }

	  // if not, load it now
	  if (this.experiment == null)
	  {
	    this.experiment = new Experiment("XXX");
	    theWeblabClient.loadArchivedExperiment(experiment, expID);
	    theMainFrame.getRecentExperiments().add(experiment);
	  }

	  // if a targetTab was provided, put the experiment in its
	  // results panel, and start editing a fresh clone of it in
	  // the schematic panel
	  if (targetTab != null)
	  {
	    targetTab.setResultsExperiment(experiment);
	    targetTab.setCurrentExperiment((Experiment) experiment.clone());
	  }

	  this.dispose();
	}
	catch (ServerException e) {
	  e.printStackTrace();
	  if (expList.isSelectionEmpty())
	    JOptionPane.showMessageDialog
	      (this, "Failed to retrieve archived experiment " + expID + ".\nPlease double-check that the Experiment ID is correct and that you have permission to access this experiment record.\nSee Java Console output for more details.", "Error", JOptionPane.ERROR_MESSAGE);
	  else
	    ConvenientDialog.showExceptionDialog(theMainFrame, e);
	}
	catch (InvalidLabConfigurationException e) {
	  e.printStackTrace();
	  ConvenientDialog.showExceptionDialog(theMainFrame, e);
	}
	catch (InvalidExperimentSpecificationException e) {
	  e.printStackTrace();
	  ConvenientDialog.showExceptionDialog(theMainFrame, e);
	}
	catch (InvalidExperimentResultException e) {
	  e.printStackTrace();
	  ConvenientDialog.showExceptionDialog(theMainFrame, e);
	}

	setCursor(null);
      }
      catch (NumberFormatException ex)
      {
	JOptionPane.showMessageDialog
	  (this, "Please enter a valid Experiment ID or select an experiment from the list.", "Error", JOptionPane.ERROR_MESSAGE);
      }
    }
    else if (cmd.equals("Cancel"))
    {
      this.dispose();
    }
  }



  private class ExperimentInformationDisplay
  {
    public ExperimentInformation theExperimentInformation;

    public ExperimentInformationDisplay(ExperimentInformation expInfo)
    {
      theExperimentInformation = expInfo;
    }

    public String toString()
    {
      StringBuffer sb = new StringBuffer();

      sb.append("#");
      sb.append(theExperimentInformation.experimentID.toString());
      sb.append("  (");
      sb.append(dateFormat.format(theExperimentInformation.submissionTime));
      sb.append(")  ");

      if (theExperimentInformation.annotation.equals(""))
	sb.append("???");
      else
	sb.append(theExperimentInformation.annotation);

      return sb.toString();
    }
  } // end inner class ExperimentInformationDisplay

} // end class LoadExperimentDialog
