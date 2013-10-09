/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

// search for XXX!

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.BufferedReader;
import java.net.URL;
import java.net.MalformedURLException;

import java.util.Iterator;
import java.util.Observable;
import java.util.Observer;
import java.util.List;
import java.util.Set;
import java.util.Map;
import java.util.Comparator;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.TreeSet;

import java.applet.AppletContext;

import javax.swing.*;
import javax.swing.event.*;
import java.awt.*;
import java.awt.event.*;
import javax.swing.border.*;

import javax.swing.plaf.basic.BasicTabbedPaneUI;

import weblab.toolkit.util.ChangeTrackingObservable;
import weblab.toolkit.util.ConvenientDialog;

import weblab.client.*;
import weblab.toolkit.serverInterface.ServerException;


/**
 * The top-level frame of the Graphical Applet.
 */
public class MainFrame extends JFrame
  implements Observer, ActionListener, WindowListener, ChangeListener
{
  protected WeblabClient theWeblabClient;

  private Set recentExperiments; // List of Experiment

  private AppletContext theAppletContext;
  private String helpURL;


  private JMenuBar theMenuBar;

  private JMenu menu_Setup, menu_Experiment, menu_Devices, menu_Results, menu_Help;

  private List deviceMenuItems; // contains JRadioButtonMenuItem

  private JTabbedPane theTabbedPane;

  private ExperimentTab currentTab;

  private JRadioButton noneButton; // for deselecting all devices

  // serial number for UNTITLED experiments created this session
  private int untitledSerialNumber = 1;

  private Map portDialogs;

  private Timer pingTimer;



  /**
   * Creates the MainFrame.
   */
  public MainFrame(WeblabClient wc, AppletContext appletContext,
		   String helpURL)
  {
    // pacify the user in case load takes a long time
    ProgressMonitor pm = new ProgressMonitor
      (null, "", "Initializing...", 0, 11);
    pm.setMillisToPopup(0);
    pm.setMillisToDecideToPopup(0);
    pm.setProgress(1);

    this.theWeblabClient = wc;
    this.theAppletContext = appletContext;
    this.helpURL = helpURL;

    this.deviceMenuItems = new ArrayList();

    // sorts Experiments in REVERSE order of submissionTime (breaking
    // ties by reverse order of experimentID, then by hashCode)
    recentExperiments = new TreeSet
      (new Comparator() {
	  public int compare(Object o1, Object o2)
	  {
	    Experiment e1 = (Experiment) o1;
	    Experiment e2 = (Experiment) o2;

	    int answer = 0;

	    try {
	      answer = e2.getSubmissionTime().compareTo
		(e1.getSubmissionTime());

	      if (answer == 0)
		answer = e2.getExperimentID().compareTo(e1.getExperimentID());
	    }
	    catch (NullPointerException ex)
	    {
	      ex.printStackTrace();
	    }

	    if (answer == 0)
	      answer = e1.hashCode() - e2.hashCode();

	    return answer;
	  }
	});

    noneButton = new JRadioButton();

    portDialogs = new HashMap();

    //
    // create menubar
    //

    createMenuBar();

    //
    // attempt to load active lab configuration
    //

    pm.setProgress(3);
    pm.setNote("Retrieving lab configuration...");

    boolean tryAgain = true;
    while (tryAgain)
    {
      try {
	theWeblabClient.loadLabConfiguration();
	tryAgain = false;
      }
      catch (InvalidLabConfigurationException ex)
      {
	ex.printStackTrace();
	ConvenientDialog.showExceptionDialog(this, ex);
	tryAgain = false;
      }
      catch (ServerException ex)
      {
	ex.printStackTrace();
	ConvenientDialog.showExceptionDialog(this, ex);
	String message = "Failed to load lab configuration.  Try again?";
	tryAgain = ConvenientDialog.showConfirmDialog
	  (this, message, "Error", "Try again", "Give up");
      }
    }

    pm.setProgress(8);
    pm.setNote("Starting the client...");

    theTabbedPane = new JTabbedPane();

    // customize appearance of JTabbedPane to remove obnoxious borders
    // on bottom and sides
    theTabbedPane.setUI(new BasicTabbedPaneUI ()
      {
	protected void paintContentBorderRightEdge
	  (Graphics g, int tabPlacement, int selectedIndex,
	   int x, int y, int w, int h)
	{
	  // Do nothing
	}
	
	protected void paintContentBorderLeftEdge
	  (Graphics g, int tabPlacement, int selectedIndex,
	   int x, int y, int w, int h)
	{
	  // Do nothing
	}
	
	protected void paintContentBorderBottomEdge
	  (Graphics g, int tabPlacement, int selectedIndex,
	   int x, int y, int w, int h)
	{
	  // Do nothing
	}

	protected Insets getContentBorderInsets(int tabPlacement)
	{
	  return new Insets(2, 0, 0, 0);
	}
      });

    //
    // setup event listeners
    //

    // note: make sure this doesn't get unnecessarily updated from
    // changes that have already happened
    theWeblabClient.notifyObservers();
    theWeblabClient.addWeakReferenceObserver(this);
    theTabbedPane.addChangeListener(this);
    this.addWindowListener(this);

    // initialize first ExperimentTab with default Experiment

    onNewTab();

    //
    // Finalize layout
    //

    this.getContentPane().add(theTabbedPane);
    this.pack();

    // this Timer pings the Server every 5 minutes to prevent the
    // user's session from timing out
    pingTimer = new Timer(300000, new ActionListener() {
	public void actionPerformed(ActionEvent evt)
	{
	  try
	  {
	    theWeblabClient.pingServer();
	  }
	  catch (ServerException ex)
	  {
	    ex.printStackTrace();
	  }
	}
      });
    pingTimer.start();

    // stop pacifying the user, and pretend we actually care about the
    // Cancel button
    if (pm.isCanceled())
      throw new Error("Client initialization cancelled by user.");
    pm.close();
  }



  public AppletContext getAppletContext()
  {
    return theAppletContext;
  }


  public WeblabClient getWeblabClient()
  {
    return theWeblabClient;
  }


  // Set of Experiment
  public Set getRecentExperiments()
  {
    return recentExperiments;//XXX???
  }


  public JToolBar createToolBar()
  {
    JToolBar jtb = new JToolBar(JToolBar.VERTICAL);
    jtb.setOpaque(false);
    jtb.add(makeToolbarButton
	    ("execute", "run_icon_32x32.png", "Run Experiment"));
    jtb.add(makeToolbarButton
	    ("loadSetup", "load_icon_32x32.png", "Load Setup..."));
    jtb.add(makeToolbarButton
	    ("saveSetup", "save_icon_32x32.png", "Save Setup"));
    jtb.add(makeToolbarButton
	    ("showUserDefinedFunctions", "udf_icon_32x32.png",
	     "Edit User-Defined Functions"));
    jtb.add(makeToolbarButton
	    ("help", "help_icon_32x32.png", "Help"));
    return jtb;
  }



  /**
   * Creates and assembles the menu bar, menus, and menu items.
   */
  private void createMenuBar()
  {
    // Create menubar
    theMenuBar = new JMenuBar();

    // Populate menubar
    menu_Experiment = new JMenu("Experiment");
    theMenuBar.add(menu_Experiment);

    menu_Setup = new JMenu("Setup");
    theMenuBar.add(menu_Setup);

    menu_Devices = new JMenu("Device");
    theMenuBar.add(menu_Devices);

    menu_Results = new JMenu("Results");
    theMenuBar.add(menu_Results);

    menu_Help = new JMenu("Help");
    theMenuBar.add(menu_Help);

    JMenu menu_NewTab = new JMenu("New Tab");
    menu_NewTab.add(makeMenuItem("newTab", "Blank"));
    menu_NewTab.add(makeMenuItem("copyTab", "Duplicate Current Tab"));

    // Populate Experiment menu
    menu_Experiment.add(menu_NewTab);
    menu_Experiment.add(makeMenuItem("loadExperiment", "Load Archived Experiment...")); //XXX "Load from Archive..."?  or something else?
    menu_Experiment.add(makeMenuItem("closeTab", "Close Tab"));
    menu_Experiment.addSeparator();
    menu_Experiment.add(makeMenuItem("execute", "Run Experiment"));
    menu_Experiment.addSeparator();
    menu_Experiment.add(makeMenuItem("exit", "Exit"));

    // Populate Setup Menu
    menu_Setup.add(makeMenuItem("reset", "New (reset)"));
    menu_Setup.add(makeMenuItem("loadSetup", "Load..."));
    menu_Setup.add(makeMenuItem("saveSetup", "Save"));
    menu_Setup.add(makeMenuItem("saveSetupAs", "Save As..."));
    menu_Setup.add(makeMenuItem("deleteSetup", "Delete..."));
    menu_Setup.addSeparator();
    menu_Setup.add
      (makeMenuItem
       ("showUserDefinedFunctions", "Edit User-Defined Functions"));

    // Populate Results menu
    menu_Results.add(makeMenuItem("viewData", "View Data"));
    menu_Results.add(makeMenuItem("downloadData", "Export to CSV File..."));
    menu_Results.addSeparator();
    menu_Results.add(makeMenuItem("clearData", "Clear Current Result"));

    // Populate Help menu
    menu_Help.add(makeMenuItem("help", "Help"));
    menu_Help.add(makeMenuItem("about", "About the Client"));

    // Activate menubar
    setJMenuBar(theMenuBar);
  }



  /**
   * Handle updates from Observables.
   */
  public final void update(Observable o, Object arg)
  {
    // When the WeblabClient's active Lab Configuration changes,
    // update Devices menu
    if (o == theWeblabClient &&
	ChangeTrackingObservable.extractChanges(arg).contains
	(WeblabClient.ACTIVE_LAB_CONFIGURATION_CHANGE))
      updateDevicesMenu();
  }



  protected/*XXX private*/ final void updateTitle()
  {
    this.setTitle
      (currentTab.getCurrentExperiment().getExperimentSpecification().getName()
       + " - MIT Microelectronics Device Characterization Lab");
  }



  protected/*XXX private*/ void updateTabTitle(ExperimentTab et)
  {
    int index = theTabbedPane.indexOfComponent(et);
    if (index == -1)
      return;

    Experiment exp = et.getCurrentExperiment();
    Integer expID = exp.getExperimentID();

    String title = (expID == null ? "" : "#" + expID.toString() + ": ");
    title += exp.getExperimentSpecification().getName();

    theTabbedPane.setTitleAt(index, title);

    if (index == theTabbedPane.getSelectedIndex())
      updateTitle();
  }



  protected boolean isActiveTab(ExperimentTab et)
  {
    return (currentTab == et);
  }



  protected/*XXX private*/ final void updateDevicesMenu()
  {
    deviceMenuItems.clear();
    menu_Devices.removeAll();
    ButtonGroup deviceButtonGroup = new ButtonGroup();
    deviceButtonGroup.add(noneButton);

    List devices = theWeblabClient.getActiveLabConfiguration()
      .getDevices();

    // add an entry for each available device in the active lab
    // configuration to menu, deviceMenuItems, and deviceButtonGroup
    // (to easily ensure that only one active device button is
    // selected at a time)
    for (int i = 0, n = devices.size(); i < n; i++)
    {
      Device d = (Device) devices.get(i);
      String menuText =
	d.getDeviceID() + ": " + d.getName() +
	" (" + d.getDeviceType() + ")";

      JRadioButtonMenuItem deviceButton = new JRadioButtonMenuItem(menuText);
      final int deviceIndex = i;
      deviceButton.addActionListener(new ActionListener() {
	  public void actionPerformed(ActionEvent e) {
	    onDeviceSelect(deviceIndex);
	  }
	});

      deviceButtonGroup.add(deviceButton);
      menu_Devices.add(deviceButton);
      deviceMenuItems.add(deviceButton);
    }

    // if no devices in activeLC, add a grayed-out explanatory item
    if (devices.size() == 0)
    {
      JMenuItem noDevicesItem = new JMenuItem("(no active devices available)");
      noDevicesItem.setEnabled(false);
      menu_Devices.add(noDevicesItem);
    }

    // if the current experiment is inactive, add a special entry for
    // the selected inactive device (if any).
    //
    // do NOT add this special entry to deviceMenuItems -- we don't
    // want updateDeviceMenuItemState to be able to deselect it
    Experiment exp = currentTab.getCurrentExperiment();
    if (! theWeblabClient.isActive(exp))
    {
      Device d = exp.getExperimentSpecification().getDevice();

      if (d != null)
      {
	menu_Devices.addSeparator();

	String menuText =
	  "?: " + d.getName() +
	  " (" + d.getDeviceType() + ")";

	JRadioButtonMenuItem inactiveDeviceButton =
	  new JRadioButtonMenuItem(menuText);
	inactiveDeviceButton.setSelected(true);
	inactiveDeviceButton.setEnabled(false);
	menu_Devices.add(inactiveDeviceButton);
      }
    }

    // update menu item state according to currently selected device
    updateDeviceMenuItemState();
  }



  // updates the state of the radio button menu items in the devices
  // menu based on which active device (if any) is selected in the
  // current Experiment.
  //
  // note: semantically this method assumes that the contents of
  // deviceMenuItems are properly in sync with the active lab
  // configuration, but it will not cause an error if this is not the
  // case.
  protected/*XXX private*/ final void updateDeviceMenuItemState()
  {
    // determine the index of the currently selected active device, if
    // any.
    int selectedActiveDeviceIndex = -1;

    Device selectedDevice = currentTab.getCurrentExperiment()
      .getExperimentSpecification().getDevice();

    if (selectedDevice != null)
    {
      List activeDevices =
	theWeblabClient.getActiveLabConfiguration().getDevices();

      selectedActiveDeviceIndex = activeDevices.indexOf(selectedDevice);
    }

    if (selectedActiveDeviceIndex != -1)
    {
      // select the appropriate button, and let ButtonGroup magic take
      // care of deselecting the rest.
      ((JRadioButtonMenuItem) deviceMenuItems.get(selectedActiveDeviceIndex))
	.setSelected(true);
    }
    else
    {
      // select no button
      noneButton.setSelected(true);
    }
  }



  /**
   * Returns the PortDialog for the specified Port.  If the specified
   * Port does not have a PortDialog yet, creates one and returns it.
   */
  public PortDialog getPortDialog(Port p)
  {
    PortDialog pd = (PortDialog) portDialogs.get(p);
    if (pd == null)
    {
      pd = PortDialog.createPortDialog(this, p);
      portDialogs.put(p, pd);
    }
    return pd;
  }



  /**
   * Brings up online help.
   */
  public void onHelp()
  {
    // instruct browser to open helpURL in a new window
    try {
      theAppletContext.showDocument(new URL(helpURL), "helpWindow");
    }
    catch(MalformedURLException ex) {}

    String message = "Online help should automatically open in a new browser window.\nIf it does not, try manually directing your browser to\n" + helpURL;
    JOptionPane.showMessageDialog
      (this, message, "Help", JOptionPane.INFORMATION_MESSAGE);
  }


  /**
   * Executes the measurement.
   */
  public void onExecute()
  {
    final ExperimentTab targetTab = currentTab;
    final Experiment exp = targetTab.getCurrentExperiment();

    // ensure that currentExperiment is active.  If not, display an
    // error dialog and abort
    if (! theWeblabClient.isActive(exp))
    {
      JOptionPane.showMessageDialog
	(this, "The currently selected device is from an old set of lab configuration data that may be out of date.\nPlease select an active device from the Devices menu and try again.",
	 "Error", JOptionPane.ERROR_MESSAGE);
      return;
    }

    // create a modal dialog
    final JDialog executeDialog = new JDialog
      (this, "Please Wait...", true);

    // don't allow to user to force-close the dialog if they get
    // impatient (because then they'll click Run again and swamp
    // the system worse)
    executeDialog.setDefaultCloseOperation(JDialog.DO_NOTHING_ON_CLOSE);

    // create text area

    final JLabel status = new JLabel
      ("  Status: unknown");

    JLabel mesg = new JLabel
      ("  Your experiment is being submitted to the lab server "
       + "for execution.  ");

    // create progress bar

    final JProgressBar progressBar = new JProgressBar(0, 100);
    progressBar.setString("");
    progressBar.setStringPainted(true);
    progressBar.setValue(0);
    progressBar.setIndeterminate(true);

    // create cancel button
    final JButton cancelButton = new JButton("Cancel");
    cancelButton.addActionListener(new ActionListener() {
	public void actionPerformed(ActionEvent e) {
	  cancelButton.setEnabled(false);
	  try {
	    theWeblabClient.cancelExecution();
	  }
	  catch(ServerException ex) {
	    ex.printStackTrace();
	    ConvenientDialog.showExceptionDialog(null/*XXX*/, ex);
	  }
	}
      });

    // layout dialog

    JPanel buttonPanel = new JPanel();
    buttonPanel.add(cancelButton);

    Container c = executeDialog.getContentPane();
    c.setLayout(new BoxLayout(c, BoxLayout.Y_AXIS));
    c.add(Box.createVerticalStrut(3));
    c.add(mesg);
    c.add(status);
    c.add(progressBar);
    c.add(Box.createVerticalStrut(10));
    c.add(buttonPanel);

    final long startTime = System.currentTimeMillis();

    // this Timer updates the status info in the execution dialog box
    // every half second (calling Swing methods safely from the
    // event-dispatching thread) until stopped
    final Timer statusTimer = new Timer(500, new ActionListener() {
	public void actionPerformed(ActionEvent evt)
	{
	  status.setText
	    ("  Status: " + theWeblabClient.getExecutionStatus());

	  int remainingSeconds =
	    theWeblabClient.getExecutionEstimatedTimeRemaining();

	  if (remainingSeconds == -1)
	  {
	    //XXX?  leave progress alone or go back to indeterminate?
	    progressBar.setString("");
	  }
	  else
	  {
	    progressBar.setIndeterminate(false);

	    // calculate completion percentage based on start time,
	    // current time, and estimated remaining time
	    int percentComplete = 100 -
	      (100 * remainingSeconds /
	       ((int) ((System.currentTimeMillis() - startTime) / 1000)
		+ remainingSeconds));

	    // update progress bar only if we've moved forward
	    // (displaying backward progress is depressing)
	    if (percentComplete > progressBar.getValue())
	      progressBar.setValue(percentComplete);

	    // display remaining time in a form that's user-friendly
	    // and not too precise
	    String et;

	    if (remainingSeconds > 90)
	      // round up to nearest minute
	      et = ((remainingSeconds + 59) / 60) + " minutes";
	    else if (remainingSeconds > 0)
	      // round up to nearest 5 seconds
	      et = ((remainingSeconds + 4) / 5) * 5 + " seconds";
	    else
	      et = "1 Second";

	    progressBar.setString("about " + et + " remaining...");
	  }
	}
      });
    statusTimer.setInitialDelay(0);

    // this thread calls WeblabClient.execute, which must be run in
    // the background because it takes a long time.  When finished, it
    // stops the status timer and closes the dialog box.
    Thread executeThread = new Thread(new Runnable () {
	public final void run()
	{
	  try
	  {
	    try
	    {
	      theWeblabClient.execute(exp);

	      // If successful, clone the newly executed experiment.
	      // The clone is sent to the results panel and added to
	      // the list of recently executed experiments, and will
	      // hereafter be treated as read-only; the original copy
	      // remains in the schematic panel where it can be edited
	      // again and subsequently re-submitted for execution as
	      // a new experiment.
	      Experiment expClone = (Experiment) exp.clone();
	      recentExperiments.add(expClone);
	      targetTab.setResultsExperiment(expClone);
	    }
	    finally
	    {
	      // whether successful or not, stop the status timer and
	      // close the dialog box before going on
	      statusTimer.stop();
	      executeDialog.setVisible(false);
	      executeDialog.dispose();
	    }
	  }
	  catch (ServerException ex) {
	    ex.printStackTrace();
	    ConvenientDialog.showExceptionDialog(null/*XXX*/, ex);
	  }
	  catch (InvalidExperimentResultException ex) {
	    ex.printStackTrace();
	    ConvenientDialog.showExceptionDialog(null/*XXX*/, ex);
	  }
	}});

    // start background threads and display dialog box
    executeDialog.pack();
    this.centerDialog(executeDialog);
    executeThread.start();
    statusTimer.start();

    executeDialog.setVisible(true); // blocks until dialog is closed
  }



  /**
   * Resets the client.
   */
  public void onReset()
  {
    theWeblabClient.reset(currentTab.getCurrentExperiment());

    ExperimentSpecification expSpec =
      currentTab.getCurrentExperiment().getExperimentSpecification();
    if (expSpec.getName().equals(WeblabClient.UNTITLED))
    {
      expSpec.setName(WeblabClient.UNTITLED + (untitledSerialNumber++));
      expSpec.notifyObservers();
    }
  }



  /**
   * Opens a dialog box to allow the user to load a setup.
   */
  public void onLoadSetup()
  {
    SetupDialog d = new SetupDialog
      (this, theWeblabClient, currentTab.getCurrentExperiment(), "Load");
    d.setVisible(true);
  }



  /**
   * Saves the current setup under its existing name.  If UNTITLED,
   * does onSaveSetupAs instead.
   */
  public void onSaveSetup()
  {
    Experiment exp = currentTab.getCurrentExperiment();
    ExperimentSpecification expSpec = exp.getExperimentSpecification();
    String name = expSpec.getName();

    if (expSpec.isQuickSaveable() && ! name.equals(""))
    {
      try {
	theWeblabClient.saveSetup(exp, name);
      }
      catch (ServerException ex)
      {
	ex.printStackTrace();
	ConvenientDialog.showExceptionDialog(this, ex);
      }
    }
    else
    {
      // do a Save As instead
      SetupDialog d = new SetupDialog
	(this, theWeblabClient, exp, "Save", name);
      d.setVisible(true);
    }
  }



  /**
   * Opens a dialog box to allow the user to save the current setup.
   */
  public void onSaveSetupAs()
  {
    Experiment exp = currentTab.getCurrentExperiment();

    SetupDialog d = new SetupDialog
      (this, theWeblabClient, exp, "Save",
       exp.getExperimentSpecification().getName());
    d.setVisible(true);
  }



  /**
   * Opens a dialog box to allow the user to delete a setup.
   */
  public void onDeleteSetup()
  {
    SetupDialog d = new SetupDialog
      (this, theWeblabClient, currentTab.getCurrentExperiment(), "Delete");
    d.setVisible(true);
  }



  /**
   * Opens the User-Defined Functions dialog box.
   */
  public void onShowUserDefinedFunctions()
  {
    currentTab.showUserDefinedFunctionsDialog();
  }



  /**
   * Displays a non-modal dialog containing the current result data.
   */
  public void onViewData()
  {
    Experiment exp = currentTab.getResultsExperiment();

    // if there is no result, display an error dialog and do nothing
    if (exp.getExperimentResult().getVariables().isEmpty())
    {
      String error = "No data to view!\n" +
	"You must execute an experiment first.";
      JOptionPane.showMessageDialog(this, error, "Error",
				    JOptionPane.ERROR_MESSAGE);
      return;
    }

    // create non-modal dialog
    final JDialog viewDataDialog = new JDialog
      (this, "View Data: " + exp.toString(), false);

    // create text area
    final JTextArea viewDataTextArea = new JTextArea
      (theWeblabClient.viewData(currentTab.getCurrentExperiment()));
    viewDataTextArea.setEditable(false);
    viewDataTextArea.setRows(20);
    viewDataTextArea.setColumns(50);

    // put text area in scroll pane
    JScrollPane scrollPane = new JScrollPane(viewDataTextArea);

    // create buttons

    JButton selectAllButton = new JButton("Select All");
    selectAllButton.addActionListener(new ActionListener() {
	public void actionPerformed(ActionEvent e) {
	  viewDataTextArea.selectAll();
	  viewDataTextArea.requestFocus();
	}
      });

    JButton closeButton = new JButton("Close");
    closeButton.addActionListener(new ActionListener() {
	public void actionPerformed(ActionEvent e) {
	  viewDataDialog.dispose();
	}
      });

    // layout dialog

    JPanel buttonPanel = new JPanel();
    buttonPanel.add(selectAllButton);
    buttonPanel.add(closeButton);

    Container c = viewDataDialog.getContentPane();
    c.setLayout(new BorderLayout());
    c.add(scrollPane, BorderLayout.CENTER);
    c.add(buttonPanel, BorderLayout.SOUTH);

    viewDataDialog.pack();

    // show dialog
    viewDataDialog.setVisible(true);
  }



  /**
   * Opens a dialog to allow the user to save the results to a local
   * CSV file.  If the user clicks Save, saves.
   */
  public void onDownloadData()
  {
    Experiment exp = currentTab.getResultsExperiment();

    // if there is no result, display an error dialog and do nothing
    if (exp.getExperimentResult().getVariables().isEmpty())
    {
      String error = "No data to export!\n" +
	"You must execute an experiment first.";
      JOptionPane.showMessageDialog(this, error, "Error",
				    JOptionPane.ERROR_MESSAGE);
      return;
    }

    // prompt user for name of a .csv file to save
    JFileChooser chooser = new JFileChooser();
    CSVFileFilter filter = new CSVFileFilter();
    chooser.addChoosableFileFilter(filter);
    int returnVal = chooser.showSaveDialog(this);
	
    // if the user clicked on Save...
    if (returnVal == JFileChooser.APPROVE_OPTION)
    {
      try {
	File f = chooser.getSelectedFile();

	// make sure file destination ends with .csv
	if (! f.getName().endsWith(".csv"))
	  f = new File(f.getPath() + ".csv");

	FileWriter w = new FileWriter(f);
	w.write(theWeblabClient.viewData(exp));
	w.close();
      }
      catch (IOException ex) {
	ex.printStackTrace();
	ConvenientDialog.showExceptionDialog(this, ex);
      }
    }
  }



  /**
   * Clears the current ExperimentResult.
   */
  public void onClearData()
  {
    currentTab.setResultsExperiment(new Experiment("(no current result)"));
  }



  /**
   * Displays a dialog with version information about the client applet.
   */
  public void onAbout()
  {
    // try to read version.txt from archive
    String message;
    try {
      StringBuffer sb = new StringBuffer();
      BufferedReader br = new BufferedReader
	(new InputStreamReader(this.getClass().getResourceAsStream
			       ("/version.txt")));
	
      for (String next = br.readLine();
	   next != null;
	   next = br.readLine())
      {
	sb.append(next);
	sb.append('\n');
      }

      message = sb.toString();
    }
    catch(Exception ex) {
      System.err.println("Error reading version.txt from archive...");
      ex.printStackTrace();
      message = "version info not available";
    }

    JOptionPane.showMessageDialog(this, message, "About the Client",
				  JOptionPane.INFORMATION_MESSAGE);
  }
 


  /**
   * Closes the applet.
   *
   * Invoked from the menubar: File->Exit
   *
   * Invoked by closing the applet window
   */
  public void onExit()
  {
    pingTimer.stop();
    this.dispose();
  }



  public void onNewTab()
  {
    // create a new "blank" Experiment for the new Tab, selecting the
    // first active device if there is one
    Experiment exp = new Experiment
      (WeblabClient.UNTITLED + (untitledSerialNumber++));
    if (! theWeblabClient.getActiveLabConfiguration().getDevices().isEmpty())
      theWeblabClient.selectDevice(exp, 0, false);

    // create the tab and add it to tabbedPane
    ExperimentTab newTab = new ExperimentTab(this, exp);
    theTabbedPane.addTab(null, newTab);
    updateTabTitle(newTab);

    // select the new tab (automatically triggers ChangeListener updates)
    theTabbedPane.setSelectedComponent(newTab);
  }



  public void onCopyTab()
  {
    // clone current Tab's experiment for the new tab
    Experiment exp =
      (Experiment) currentTab.getCurrentExperiment().clone();

    // create the tab and add it to tabbedPane
    ExperimentTab newTab = new ExperimentTab(this, exp);
    theTabbedPane.addTab(null, newTab);
    updateTabTitle(newTab);

    // propagate state of resultsPanel
    newTab.resultsPanel.copySettingsFrom(currentTab.resultsPanel);

    // select the new tab (automatically triggers ChangeListener updates)
    theTabbedPane.setSelectedComponent(newTab);
  }



  /**
   * Invoked from the menubar: Device->[item n]
   */
  public void onDeviceSelect(int index)
  {
    Experiment exp = currentTab.getCurrentExperiment();

    try {
      theWeblabClient.selectDevice(exp, index, true);
    }
    catch (ConfirmationRequest cr)
    {
      String message = "Your current setup defines ports that are not connected to the new device you have chosen.\nIf you choose to select the device anyway, setup information for these ports will be discarded.";
      boolean confirmed = ConvenientDialog.showConfirmDialog
	(this, message, "Warning", "Select the device anyway", "Cancel");
      if (confirmed)
      {
	theWeblabClient.selectDevice(exp, index, false);
      }
      else
      {
	// reset menu state to reflect the currently selected item
	updateDeviceMenuItemState();
      }
    }

    ExperimentSpecification expSpec = exp.getExperimentSpecification();
    if (expSpec.getName().equals(WeblabClient.UNTITLED))
    {
      expSpec.setName(WeblabClient.UNTITLED + (untitledSerialNumber++));
      expSpec.notifyObservers();
    }
  }



  // sets the location of d so that it is centered over this
  public void centerDialog(Dialog d)
  {
    Point parentLocation = this.getLocation();
    Dimension parentSize = this.getSize();
    Dimension dialogSize = d.getSize();
    int x = parentLocation.x + (parentSize.width - dialogSize.width) / 2;
    int y = parentLocation.y + (parentSize.height - dialogSize.height) / 2;
    d.setLocation(x, y);
  }



  //////////////////////
  // Listener methods //
  //////////////////////


  /**
   * Handle ActionEvents
   */
  public void actionPerformed(ActionEvent evt)
  {
    String cmd = evt.getActionCommand();

    if (cmd.equals("exit"))
      this.onExit();

    else if (cmd.equals("newTab"))
      this.onNewTab();

    else if (cmd.equals("copyTab"))
      this.onCopyTab();

    else if (cmd.equals("closeTab"))
    {
      ExperimentTab target = currentTab;

      // There must always be at least one tab.  If we're about to
      // close the last tab, make a new one to replace it.
      if (theTabbedPane.getTabCount() <= 1)
	this.onNewTab();

      theTabbedPane.remove(target);
    }

    else if (cmd.equals("help"))
      this.onHelp();

    else if (cmd.equals("execute"))
      this.onExecute();

    else if (cmd.equals("reset"))
      this.onReset();

    else if (cmd.equals("loadSetup"))
      this.onLoadSetup();

    else if (cmd.equals("saveSetup"))
      this.onSaveSetup();

    else if (cmd.equals("saveSetupAs"))
      this.onSaveSetupAs();

    else if (cmd.equals("deleteSetup"))
      this.onDeleteSetup();

    else if (cmd.equals("loadExperiment"))
    {
      LoadExperimentDialog led = new LoadExperimentDialog(this, currentTab);//XXX
      centerDialog(led);
      led.setVisible(true);
      //XXX
      /*
      try {
      java.util.Date d = new java.text.SimpleDateFormat
	("y-M-d'T'H:m:sz")//.SZ")
	.parse("2006-03-20T14:41:02-0400");
	//	.parse("2007-05-23T14:41:02.3400000-04:00");
      System.out.println("date: "+d.toString());
      }
      catch(java.text.ParseException e) {
	e.printStackTrace();
      }
      */
    }

    else if (cmd.equals("showUserDefinedFunctions"))
      this.onShowUserDefinedFunctions();

    else if (cmd.equals("viewData"))
      this.onViewData();

    else if (cmd.equals("downloadData"))
      this.onDownloadData();

    else if (cmd.equals("clearData"))
      this.onClearData();

    else if (cmd.equals("about"))
      this.onAbout();

    else
      throw new Error("unexpected action command: " + cmd);
  }



  /**
   * When user selects a different Tab, update MainFrame title and
   * devices menu, hand off open dialogs, and close PortDialogs.
   */
  public void stateChanged(ChangeEvent evt)
  {
    ExperimentTab oldTab = this.currentTab;
    this.currentTab = (ExperimentTab) theTabbedPane.getSelectedComponent();

    currentTab.takeOverFrom(oldTab);

    for (Iterator i = portDialogs.values().iterator(); i.hasNext(); )
    {
      PortDialog pd = (PortDialog) i.next();
      if (pd.isVisible())
      {
	pd.revert();
	pd.setVisible(false);
      }
    }

    updateTitle();
    updateDevicesMenu();
  }



  /**
   * Allows user to close this window from its system menu.
   */
  public void windowClosing(WindowEvent evt)
  {
    onExit();
  }

  // obligatory irrelevant methods of WindowListener
  public void windowActivated(WindowEvent evt) {}
  public void windowClosed(WindowEvent evt) {}
  public void windowDeactivated(WindowEvent evt) {}
  public void windowDeiconified(WindowEvent evt) {}
  public void windowIconified(WindowEvent evt) {}
  public void windowOpened(WindowEvent evt) {}


  ////////////////////
  // Helper Methods //
  ////////////////////


  private JMenuItem makeMenuItem(String actionCommand, String text)
  {
    JMenuItem jmi = new JMenuItem(text);
    jmi.setActionCommand(actionCommand);
    jmi.addActionListener(this);
    return jmi;
  }



  private JButton makeToolbarButton(String actionCommand, String imageName,
				    String tooltipText)
  {
    JButton button = new JButton();

    try
    {
      URL imageURL = this.getClass().getResource("/img/"+ imageName);
      button.setIcon(new ImageIcon(imageURL));
    }
    catch (RuntimeException ex) {//XXX just Exception
      ex.printStackTrace();
      ConvenientDialog.showExceptionDialog(this, ex);
    }

    button.setToolTipText(tooltipText);
    button.setActionCommand(actionCommand);
    button.addActionListener(this);
    return button;
  }

} // end class MainFrame
