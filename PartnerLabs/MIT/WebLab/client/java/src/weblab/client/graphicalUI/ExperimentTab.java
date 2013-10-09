/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import java.util.Observable;
import java.util.Observer;

import javax.swing.*;
import java.awt.*;

import weblab.toolkit.util.ChangeTrackingObservable;

import weblab.client.*;


/**
 * A Tab within MainFrame that holds one Experiment.
 */
public class ExperimentTab extends JPanel implements Observer
{
  private MainFrame theMainFrame;

  private Experiment currentExperiment;
  private ExperimentSpecification theExpSpec;

  private JToolBar myToolBar;

  private UserDefinedFunctionsDialog udfDialog;
  private SchematicPanel schematicPanel;
  protected /*XXX private*/ ResultsPanel resultsPanel;


  public ExperimentTab(MainFrame theMainFrame, Experiment exp)
  {
    this.theMainFrame = theMainFrame;
    this.currentExperiment = exp;

    this.theExpSpec = currentExperiment.getExperimentSpecification();

    myToolBar = theMainFrame.createToolBar();

    //
    // set up components
    //

    udfDialog = new UserDefinedFunctionsDialog(theMainFrame, theExpSpec);
    schematicPanel = new SchematicPanel(theMainFrame, currentExperiment);
    resultsPanel = new ResultsPanel(theMainFrame);


    //
    // setup layout (but don't add scrollPane to this yet)
    //

    JPanel setupPanel = new JPanel();
    setupPanel.setOpaque(false);
    setupPanel.setLayout(new BorderLayout());
    setupPanel.add(myToolBar, BorderLayout.EAST);
    setupPanel.add(schematicPanel, BorderLayout.CENTER);

    JScrollPane resultsScrollPane = new JScrollPane(resultsPanel);
    resultsScrollPane.setHorizontalScrollBarPolicy
      (JScrollPane.HORIZONTAL_SCROLLBAR_ALWAYS);
    //    resultsScrollPane.setVerticalScrollBarPolicy
    //    (JScrollPane.VERTICAL_SCROLLBAR_NEVER);
    resultsScrollPane.setBorder(BorderFactory.createEmptyBorder());
    resultsScrollPane.getHorizontalScrollBar().setUnitIncrement(20);

    this.setBackground(Color.white);

    this.setLayout(new BorderLayout());
    this.add(setupPanel, BorderLayout.NORTH);
    this.add(resultsScrollPane, BorderLayout.CENTER);

    //
    // setup listeners
    //

    currentExperiment.addWeakReferenceObserver(this);
    theExpSpec.addWeakReferenceObserver(this);
  }

  public void showUserDefinedFunctionsDialog()
  {
    udfDialog.setVisible(true);
  }

  public Experiment getCurrentExperiment()
  {
    return currentExperiment;
  }

  public void setCurrentExperiment(Experiment newExperiment)
  {
    currentExperiment.deleteObserver(this);
    currentExperiment = newExperiment;
    currentExperiment.addWeakReferenceObserver(this);

    // update subcomponents tied to Experiment
    schematicPanel.setExperiment(newExperiment);

    // when there's a new Experiment, there's a new ExpSpec and a new Result
    updateExpSpec();

    // update Devices menu, in case either the new or old experiment
    // was an inactive Experiment.
    if (isActiveTab())
      theMainFrame.updateDevicesMenu();
  }


  // XXX XXX
  public Experiment getResultsExperiment()
  {
    return resultsPanel.mostRecentExperiment;
  }


  // XXX rename
  public void setResultsExperiment(Experiment newResultsExperiment)
  {
    resultsPanel.setExperiment(newResultsExperiment);
  }


  /**
   * Closes any UDF, Tracking, and GraphSetup dialogs of oldTab that
   * are currently open, and opens the corresponding dialogs of this.
   */
  public void takeOverFrom(ExperimentTab oldTab)
  {
    if (oldTab == null || oldTab == this)
      return;

    if (oldTab.udfDialog.isVisible())
    {
      this.udfDialog.setLocation(oldTab.udfDialog.getLocation());
      this.udfDialog.setVisible(true);
      oldTab.udfDialog.revert();
      oldTab.udfDialog.setVisible(false);
    }

    if (oldTab.resultsPanel.graphSetupDialog.isVisible())
    {
      this.resultsPanel.graphSetupDialog.setLocation
	(oldTab.resultsPanel.graphSetupDialog.getLocation());
      this.resultsPanel.graphSetupDialog.setVisible(true);
      oldTab.resultsPanel.graphSetupDialog.revert();
      oldTab.resultsPanel.graphSetupDialog.setVisible(false);
    }

    if (oldTab.resultsPanel.trackingDialog.isVisible())
    {
      this.resultsPanel.trackingDialog.setLocation
	(oldTab.resultsPanel.trackingDialog.getLocation());
      this.resultsPanel.onTrackingOpen();
      oldTab.resultsPanel.onTrackingClose();
    }
  }



  // call when ExpSpec is replaced
  private void updateExpSpec()
  {
    if (theExpSpec != null)
      theExpSpec.deleteObserver(this);
    theExpSpec = currentExperiment.getExperimentSpecification();
    theExpSpec.addWeakReferenceObserver(this);

    // update subcomponents tied to ExpSpec
    udfDialog.setExperimentSpecification(theExpSpec);
    /*XXX    channelPanel.updateExpSpec(theExpSpec);
    */

    theMainFrame.updateTabTitle(this);
  }

  /**
   * Handle updates from Observables.
   */
  public final void update(Observable o, Object arg)
  {
    if (o == currentExperiment &&
	ChangeTrackingObservable.extractChanges(arg).contains
	(Experiment.EXPERIMENT_SPECIFICATION_CHANGE))
      updateExpSpec();

    // When the current Experiment's lab configuration changes, update
    // Devices menu (in case the Experiment has become active)
    if (o == currentExperiment &&
	ChangeTrackingObservable.extractChanges(arg).contains
	(Experiment.LAB_CONFIGURATION_CHANGE))
      if (isActiveTab())
	theMainFrame.updateDevicesMenu();

    if (o == currentExperiment &&
	ChangeTrackingObservable.extractChanges(arg).contains
	(Experiment.EXPERIMENT_INFO_CHANGE))
      theMainFrame.updateTabTitle(this);

    // When ExperimentSpecification changes internally, update title
    // and (if device was changed) device menu state
    if (o == theExpSpec)
    {
      theMainFrame.updateTabTitle(this);

      if (isActiveTab())
	if (ChangeTrackingObservable.extractChanges(arg).contains
	    (ExperimentSpecification.DEVICE_CHANGE))
	  theMainFrame.updateDeviceMenuItemState();
    }
  }


  private boolean isActiveTab()
  {
    return theMainFrame.isActiveTab(this);
  }

} // end class ExperimentTab
