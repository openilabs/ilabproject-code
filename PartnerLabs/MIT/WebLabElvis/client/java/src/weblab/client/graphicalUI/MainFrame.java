package weblab.client.graphicalUI;

import weblab.client.*;
import weblab.client.serverInterface.ServerException;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.BufferedReader;
import java.net.URL;
import java.net.MalformedURLException;

import java.util.Observable;
import java.util.Observer;
import java.util.Vector;

import java.applet.AppletContext;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import javax.swing.border.*;

import weblab.util.ChangeTrackingObservable;


public class MainFrame extends JFrame implements Observer
{
  // reference to this for use within inner classes
  private final MainFrame self;

  private WeblabClient theWeblabClient;

  private ExperimentSpecification theExpSpec;

  private AppletContext theAppletContext;
  private String helpURL;

  private boolean initDone = false;

  //private UserDefinedFunctionsDialog udfDialog;

  private volatile Thread statusThread, executeThread;

  private int selectedSetupIndex = -1;
  private Vector setupMenuItems; // contains JRadioButtonMenuItem

  private ResultsPanel theResultsPanel;



  /**
   * Closes the applet.
   */
  private final Action EXIT = new AbstractAction("Exit")
    {
      public void actionPerformed(ActionEvent evt) {
    	  self.onExit();
      }
    };


  /**
   * Brings up online help.
   */
  private final Action HELP = new AbstractAction("Help")
    {
      public void actionPerformed(ActionEvent evt)
      {
    	  // instruct browser to open helpURL in a new window
    	  try {
    		  theAppletContext.showDocument(new URL(helpURL), "helpWindow");
    	  }
    	  catch(MalformedURLException ex) {}
    	  	String message = "Online help should automatically open in a new browser window.\nIf it does not, try manually directing your browser to\n" + helpURL;
    	  	JOptionPane.showMessageDialog(self, message, "Help", JOptionPane.INFORMATION_MESSAGE);
      	}
    };


  /**
   * Executes the measurement.
   */
  private final Action EXECUTE = new AbstractAction("Run Experiment")
    {
      public void actionPerformed(ActionEvent evt)
      {
    	  // create a modal dialog
    	  final JDialog executeDialog = new JDialog(self, "Please Wait...", true);
    	  // don't allow to user to force-close the dialog if they get
    	  // impatient (because then they'll click Run again and swamp
    	  // the system worse)
    	  executeDialog.setDefaultCloseOperation(JDialog.DO_NOTHING_ON_CLOSE);
    	  // create text area
    	  JLabel mesg = new JLabel("  Your experiment is being submitted to the lab server for execution.  ");
    	  final JLabel status = new JLabel("  Status: unknown");
    	  final JLabel estTime = new JLabel("  Estimated time remaining: unknown");

    	  // create cancel button
    	  final JButton cancelButton = new JButton("Cancel");
    	  cancelButton.addActionListener(new ActionListener() {
    		  public void actionPerformed(ActionEvent e) {
    			  cancelButton.setEnabled(false);
    			  try {
    				  theWeblabClient.cancelExecution();
    			  }
    			  catch(ServerException ex) {
    				  String error = ex.getMessage();
    				  ex.printStackTrace();
    				  JOptionPane.showMessageDialog(self, error, "Error",JOptionPane.ERROR_MESSAGE);
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
    	  // TEMPORARILY DISABLED FOR MAIN DEPLOYMENT <dmrz>
    	  //c.add(estTime);
    	  c.add(Box.createVerticalStrut(10));
    	  c.add(buttonPanel);

    	  // this thread updates the status text (and the time
    	  // remaining) every half-second until its variable is set to
    	  // null, then terminates
    	  statusThread = new Thread(new Runnable () {
    	 public final void run() {
    		 while (statusThread != null)
    		 {
    			 status.setText("  Status: " + theWeblabClient.getExecutionStatus());
    			 estTime.setText("  Estimated time remaining: " + theWeblabClient.getExecutionEstimatedTimeRemaining());
    			 try {
    				 Thread.sleep(500);
    			 }
    			 catch (InterruptedException e) {
    				 statusThread = null;
    			 }
    		 }
    		 statusThread = null;
    	 }});

    	  // this thread calls WeblabClient.execute, which must be run in
    	  // the background because it takes a long time.  When finished,
    	  // it signals the status thread to terminate, and closes the
    	  // dialog box.
    	  executeThread = new Thread(new Runnable () {
    		  public final void run() {
    			  Exception ex = null;
    			  try {
    				  theWeblabClient.execute();
    			  }
    			  catch (ServerException e) {
    				  ex = e;
    			  }
    			  catch (InvalidExperimentResultException e) {
    				  ex = e;
    			  }
    			  catch (RuntimeException e)
    			  {
					// if we get an unexpected runtime exception, stop the
					// status thread and close the dialog box before
					// rethrowing it
					statusThread = null;
					executeDialog.setVisible(false);
					executeDialog.dispose();
					throw e;
    			  }

			      statusThread = null;
			      
			      executeDialog.setVisible(false);
			      executeDialog.dispose();

			      if (ex != null)
			      {
			    	  String error = ex.getMessage();
			    	  ex.printStackTrace();
			    	  JOptionPane.showMessageDialog(self, error, "Error", JOptionPane.ERROR_MESSAGE);
			      }

			      executeThread = null;
    		  }});

    	  // start background threads and display dialog box
    	  executeDialog.pack();
    	  self.centerDialog(executeDialog);
    	  statusThread.start();
    	  executeThread.start();

    	  executeDialog.setVisible(true); // blocks until dialog is closed
      	}
    };

  /**
   * Resets the client.
   */
  private final Action RESET = new AbstractAction("New Configuration (reset)")
    {
      public void actionPerformed(ActionEvent evt) {
    	  theWeblabClient.reset();
      }
    };

  /**
   * Opens a dialog box to allow the user to load a configuration.
   */
    private final Action LOAD_CONFIGURATION = new AbstractAction("Load Configuration...")
    {
      public void actionPerformed(ActionEvent evt) {
    	  ConfigurationDialog d = new ConfigurationDialog(self, theWeblabClient, "Load");
    	  d.setVisible(true);
      }
    };

  /**
   * Saves the current configuration under its existing name.  If UNTITLED,
   * does a SAVE_CONFIGURATION_AS instead.
   */
  private final Action SAVE_CONFIGURATION = new AbstractAction("Save Configuration")
    {
      public void actionPerformed(ActionEvent evt) {
    	  String name = theWeblabClient.getExperimentSpecification().getName();

    	  if (name.equals("") || name.equals(WeblabClient.UNTITLED))
    	  {
    		  // do a Save As instead
    		  ConfigurationDialog d = new ConfigurationDialog(self, theWeblabClient, "Save");
    		  d.setVisible(true);
    	  }
    	  else
    	  {
			  try {
			    theWeblabClient.saveExpConfiguration(name);
			  }
			  catch (ServerException e)
			  {
			    String error = "Server error: " + e.getMessage();
			    e.printStackTrace();
			    JOptionPane.showMessageDialog(self, error, "Error", JOptionPane.ERROR_MESSAGE);
			  }
	}
      }
    };

  /**
   * Opens a dialog box to allow the user to save the current configuration.
   */
  private final Action SAVE_CONFIGURATION_AS = new AbstractAction("Save Configuration As...")
    {
      public void actionPerformed(ActionEvent evt) {
    	  ConfigurationDialog d = new ConfigurationDialog(self, theWeblabClient, "Save");
    	  d.setVisible(true);
      }
    };

  /**
   * Opens a dialog box to allow the user to delete an experiment configuration.
   */
  private final Action DELETE_CONFIGURATION = new AbstractAction("Delete Configuration...")
    {
      public void actionPerformed(ActionEvent evt) {
    	  ConfigurationDialog d = new ConfigurationDialog(self, theWeblabClient, "Delete");
    	  d.setVisible(true);
      }
    };

  /**
   * Opens the User-Defined Functions dialog box.
   *
  private final Action USER_DEFINED_FUNCTIONS = new AbstractAction("Show User-Defined Functions")
    {
      public void actionPerformed(ActionEvent evt) {
	udfDialog.setVisible(true);
      }
    };

  /**
   * Displays a non-modal dialog containing the current result data.
   */
  private final Action VIEW_DATA = new AbstractAction("View Data")
    {
      public void actionPerformed(ActionEvent evt)
      {
		// create dialog
		final JDialog viewDataDialog = new JDialog(self, "View Data", false);
		// create text area
		JTextArea dataTextArea = new JTextArea(theWeblabClient.viewData());
		dataTextArea.setEditable(false);
		dataTextArea.setRows(20);
		dataTextArea.setColumns(50);
		// put text area in scroll pane
		JScrollPane scrollPane = new JScrollPane(dataTextArea);
		// create close button
		JButton closeButton = new JButton("Close");
		closeButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				viewDataDialog.dispose();
			}
		});
		// layout dialog
		Container c = viewDataDialog.getContentPane();
		c.setLayout(new BoxLayout(c, BoxLayout.Y_AXIS));
		c.add(scrollPane);
		c.add(closeButton);
		// show dialog
		viewDataDialog.pack();
		viewDataDialog.setVisible(true);
      }
    };

  /**
   * Opens a dialog to allow the user to save the results to a local
   * CSV file.  If the user clicks Save, saves.
   */
  private final Action DOWNLOAD_DATA = new AbstractAction("Download Data")
    {
      public void actionPerformed(ActionEvent evt)
      {
		// if there is no result, display an error dialog and do nothing
		if (! theWeblabClient.getExperimentResult()
		    .getVariableNames().hasMoreElements())
		{
		  String error = "No data to download!\n" +
		    "You must execute a measurement first.";
		  JOptionPane.showMessageDialog(self, error, "Error",
						JOptionPane.ERROR_MESSAGE);
		  return;
		}

		// prompt user for name of a .csv file to save
		JFileChooser chooser = new JFileChooser();
		CSVFileFilter filter = new CSVFileFilter();
		chooser.addChoosableFileFilter(filter);
		int returnVal = chooser.showSaveDialog(self);
		
		// if the user clicked on Save...
		if (returnVal == JFileChooser.APPROVE_OPTION)
		{
		  try {
		    File f = chooser.getSelectedFile();
	
		    // make sure file destination ends with .csv
		    if (! f.getName().endsWith(".csv"))
		      f = new File(f.getPath() + ".csv");
	
		    FileWriter w = new FileWriter(f);
		    w.write(theWeblabClient.viewData());
		    w.close();
		  }
		  catch (IOException e) {
		    String error = "Couldn't save CSV file.\n" + e.getMessage();
		    JOptionPane.showMessageDialog(self, error, "Error",
						  JOptionPane.ERROR_MESSAGE);
		  }
		}
      }
    };

  /**
   * Clears the current ExperimentResult.
   */
  private final Action CLEAR_DATA = new AbstractAction("Clear Data")
    {
      public void actionPerformed(ActionEvent evt) {
    	  theWeblabClient.clearResults();
      }
    };

  /**
   * Displays a dialog with version information about the client applet.
   */  
  private final Action ABOUT = new AbstractAction("About the Weblab Client")
    {
      public void actionPerformed(ActionEvent evt)
      {
      // try to read build-version.txt from archive
      String message;
      try 
      {
    	  StringBuffer sb = new StringBuffer();
    	  BufferedReader br = new BufferedReader
    	  	(new InputStreamReader(this.getClass().getResourceAsStream("/build-version.txt")));
	
    	  for (String next = br.readLine();next != null;next = br.readLine())
    	  {
    		  sb.append(next);
			  sb.append('\n');
    	  }

    	  message = sb.toString();
      	}
      	catch(Exception ex) {
			System.err.println("Error reading build-version.txt from archive...");
			ex.printStackTrace();
			message = "version info not available";
      	}

      	JOptionPane.showMessageDialog(self, message, "About the Weblab Client", JOptionPane.INFORMATION_MESSAGE);
      	}
    };



  /**
   * Creates the MainFrame.
   */
  public MainFrame(WeblabClient wc, AppletContext appletContext,
		   String helpURL)
  {
    self = this;

    this.theWeblabClient = wc;
    this.theAppletContext = appletContext;
    this.helpURL = helpURL;

    this.setupMenuItems = new Vector();
  }



  // This method does the main work of setting up MainFrame, which has
  // been moved out of the constructor so that the constructor won't
  // take so long to execute.  This method is invoked when the dialog
  // is made visible.
  private final void init()
  {
    theExpSpec = theWeblabClient.getExperimentSpecification();

    //
    // pacify the user in case load takes a long time
    //

    this.setTitle("Initializing, please wait...");
    // this is a more or less random size that's big enough for the
    // user to notice (actual size is set at end by pack)
    this.setSize(500, 500);
    super.setVisible(true);

    //
    // attempt to load lab configuration
    //

    boolean tryAgain = true;
    while (tryAgain)
    {
      try {
    	  theWeblabClient.loadLabConfiguration();
    	  // notify observers that theWeblabClient has changed
    	  theWeblabClient.notifyObservers();
    	  tryAgain = false;
      }
      catch (InvalidLabConfigurationException ex)
      {
    	  ex.printStackTrace();
    	  String error = "Invalid Lab Configuration: " + ex.getMessage();
    	  JOptionPane.showMessageDialog(this, error, "Error", JOptionPane.ERROR_MESSAGE);
    	  tryAgain = false;
      }
      catch (ServerException ex)
      {
    	  ex.printStackTrace();
    	  String message = "Server error: " + ex.getMessage() + "\nFailed to load lab configuration.  Try again?";
    	  tryAgain = (JOptionPane.showConfirmDialog
	   (this, message, "Error", JOptionPane.YES_NO_OPTION)
	   == JOptionPane.YES_OPTION);
      }
    }

    // try to load image icons for actions
    try {
      EXECUTE.putValue
      (Action.SMALL_ICON, new ImageIcon(this.getClass().getResource
					  ("/img/icon_Run.gif")));
      LOAD_CONFIGURATION.putValue
      (Action.SMALL_ICON, new ImageIcon(this.getClass().getResource
					  ("/img/icon_Load.gif")));
      SAVE_CONFIGURATION.putValue
      (Action.SMALL_ICON, new ImageIcon(this.getClass().getResource
					  ("/img/icon_Save.gif")));
      /**USER_DEFINED_FUNCTIONS.putValue
      (Action.SMALL_ICON,
    		  new ImageIcon(this.getClass().getResource
		       ("/img/icon_UserDefinedFunctions.gif")));
	 **/
      HELP.putValue
	(Action.SMALL_ICON, new ImageIcon(this.getClass().getResource
					  ("/img/icon_Help.gif")));
    }
    catch (Exception e) {
      String error = "Unable to load image icons for actions";
      JOptionPane.showMessageDialog(this, error, "Error",
    		  JOptionPane.ERROR_MESSAGE);
      e.printStackTrace();
    }


    // respond to window close by exiting
    this.addWindowListener(new WindowAdapter() {
	public void windowClosing(WindowEvent e) {onExit();}
      });


    // create user-defined functions dialog
    //udfDialog = new UserDefinedFunctionsDialog(this, theWeblabClient);


    createMenuBar();

    // Create toolbar
    JToolBar theToolbar = new JToolBar(JToolBar.VERTICAL);
    theToolbar.setOpaque(false);
    theToolbar.add(new WeblabToolbarButton(EXECUTE));
    theToolbar.add(new WeblabToolbarButton(LOAD_CONFIGURATION));
    theToolbar.add(new WeblabToolbarButton(SAVE_CONFIGURATION));
    //theToolbar.add(new WeblabToolbarButton(USER_DEFINED_FUNCTIONS));
    theToolbar.add(new WeblabToolbarButton(HELP));

    // create schematic panel (after menubar, so that it doesn't have
    // to switch devices when menubar is created)
    SchematicPanel theSchematicPanel =
      new SchematicPanel(this, theWeblabClient, theAppletContext);

    theResultsPanel = new ResultsPanel(this, theWeblabClient);


    // layout elements

    JPanel leftPanel = new JPanel();
    leftPanel.setOpaque(false);
    leftPanel.setLayout(new BorderLayout());
    leftPanel.add(theToolbar, BorderLayout.EAST);
    leftPanel.add(theSchematicPanel, BorderLayout.CENTER);
    
    JPanel mainPanel = new JPanel();
    mainPanel.setBackground(Color.white);
    mainPanel.setLayout(new GridBagLayout());
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.fill = GridBagConstraints.BOTH;
    gbc.weightx = 1;
    gbc.weighty = 1;
    gbc.gridwidth = GridBagConstraints.REMAINDER;
    mainPanel.add(leftPanel, gbc);
    mainPanel.add(theResultsPanel, gbc);

    JScrollPane scrollPane = new JScrollPane(mainPanel);

    this.getContentPane().add(scrollPane);

    //
    // start observing theWeblabClient and theExpSpec
    //

    theWeblabClient.notifyObservers();
    theWeblabClient.addObserver(this);
    theExpSpec.notifyObservers();
    theExpSpec.addObserver(this);

    //
    // Stop pacifying the user, initialize title
    //

    updateTitle();
    this.pack();
    this.initDone = true;
  }



  // override setVisible to perform init if it has not yet been done
  public void setVisible(boolean b)
  {
    // if init has not been done, do it now
    if (b && !initDone)
      init();

    super.setVisible(b);
  }



  /**
   * Creates and assembles the menu bar, menus, and menu items.
   */
  private void createMenuBar()
  {
    JMenuBar theMenuBar;

    JMenu menu_Configuration, menu_Measurement, menu_Setups, menu_Results, menu_Results_GridSetup, menu_Help;

    // Create menubar
    theMenuBar = new JMenuBar();

    // Populate menubar
    menu_Configuration = new JMenu("Configuration");
    theMenuBar.add(menu_Configuration);
    menu_Measurement = new JMenu("Measurement");
    theMenuBar.add(menu_Measurement);
    menu_Setups = new JMenu("Setups");
    theMenuBar.add(menu_Setups);
    //menu_UserDefinedFunctions = new JMenu("User Defined Functions");
    //theMenuBar.add(menu_UserDefinedFunctions);
    menu_Results = new JMenu("Results");
    theMenuBar.add(menu_Results);
    menu_Help = new JMenu("Help");
    theMenuBar.add(menu_Help);


    // Populate Setup Menu
    menu_Configuration.add(RESET);
    menu_Configuration.add(LOAD_CONFIGURATION);
    menu_Configuration.add(SAVE_CONFIGURATION);
    menu_Configuration.add(SAVE_CONFIGURATION_AS);
    menu_Configuration.add(DELETE_CONFIGURATION);
    menu_Configuration.addSeparator();
    menu_Configuration.add(EXIT);

    // Populate Measurement menu
    menu_Measurement.add(EXECUTE);

    // Populate Setupss menu
    ButtonGroup setupGroup = new ButtonGroup();
    Setup[] setups = theWeblabClient.getLabConfiguration().getSetups();
    for (int i = 0; i < setups.length; i++)
    {
      Setup d = setups[i];
      String menuText =
    	  d.getSetupID() + ": " + d.getName();

      JRadioButtonMenuItem setupButton = new JRadioButtonMenuItem(menuText);
      final int setupIndex = i;
      setupButton.addActionListener(new ActionListener() {
	  public void actionPerformed(ActionEvent e) {
	    onSetupSelect(setupIndex);
	  }
	});
      setupGroup.add(setupButton);
      menu_Setups.add(setupButton);
      setupMenuItems.add(setupButton);

      // if this is the first setup, select it now
      if (i == 0)
    	  setupButton.doClick();
    }
    // if no setups, add a grayed-out explanatory item
    if (setups.length == 0)
    {
      JMenuItem noSetupItem = new JMenuItem("(no experiment setups available)");
      noSetupItem.setEnabled(false);
      menu_Setups.add(noSetupItem);
    }

    // Populate User Defined Functions menu
    //menu_UserDefinedFunctions.add(USER_DEFINED_FUNCTIONS);

    // Populate Results menu
    menu_Results_GridSetup = new JMenu("Grid Setup");
    menu_Results.add(menu_Results_GridSetup);
    menu_Results.addSeparator();
    menu_Results.add(VIEW_DATA);
    menu_Results.add(DOWNLOAD_DATA);
    menu_Results.add(CLEAR_DATA);

    // Populate Results->GridSetup submenu
    JRadioButtonMenuItem y1gridlines = new JRadioButtonMenuItem
      ("follow Y1 Axis");
    y1gridlines.addActionListener(new ActionListener() {
	public void actionPerformed(ActionEvent e) {
	  theResultsPanel.setY1Gridlines(true);
	}
      });
    JRadioButtonMenuItem y2gridlines = new JRadioButtonMenuItem
      ("follow Y2 Axis");
    y2gridlines.addActionListener(new ActionListener() {
	public void actionPerformed(ActionEvent e) {
	  theResultsPanel.setY1Gridlines(false);
	}
      });
    menu_Results_GridSetup.add(y1gridlines);
    menu_Results_GridSetup.add(y2gridlines);
    ButtonGroup gridGroup = new ButtonGroup();
    gridGroup.add(y1gridlines);
    gridGroup.add(y2gridlines);
    y1gridlines.setSelected(true);

    // Populate Help menu
    menu_Help.add(HELP);
    menu_Help.add(ABOUT);

    // Activate menubar
    setJMenuBar(theMenuBar);
  }



  /**
   * Handle updates from Observables.
   */
  public final void update(Observable o, Object arg)
  {
    if (o == theWeblabClient && ChangeTrackingObservable.containsChange
	(arg, WeblabClient.EXPERIMENT_SPECIFICATION_CHANGE))
    {
	theExpSpec.deleteObserver(this);
	theExpSpec = theWeblabClient.getExperimentSpecification();
	theExpSpec.addObserver(this);
	updateTitle();
    }

    // When ExperimentSpecification changes internally, update title
    // and (if device was changed) device menu state
    if (o == theExpSpec)
    {
      updateTitle();
    }
  }



  private final void updateTitle()
  {
    this.setTitle(theExpSpec.getName() + " - MIT NI-ELVIS Weblab");
  }


  /**
   * Exits the applet.
   *
   * Invoked from the menubar: File->Exit
   *
   * Invoked by closing the applet window
   */
  public void onExit() {
    this.dispose();
  }

  /**
   * Invoked from the menubar: Setup->[item n]
   */
  public void onSetupSelect(int index)
  {
    try {
      theWeblabClient.selectSetup(index, true);
      this.selectedSetupIndex = index;
    }
    catch (ConfirmationRequest cr)
    {
      String message = "Your current configuration defines instruments " +
      				   "that are not connected to the new experiment setup " +
      				   "you have chosen.\nIf you choose to select the experiment " +
      				   "setup anyway, this information will be lost" +
      				   ".\nSelect the experiment setup anyway?";
      boolean confirmed =
    	  (JOptionPane.showConfirmDialog
    			  (this, message, "Warning", JOptionPane.YES_NO_OPTION)
    			  == JOptionPane.YES_OPTION);
      	if (confirmed)
      	{
      		theWeblabClient.selectSetup(index, false);
      		this.selectedSetupIndex = index;
      	}
      	else
      	{
      		// reset menu state to reflect the currently selected item
      		((JRadioButtonMenuItem)setupMenuItems
    			  .elementAt(this.selectedSetupIndex))
    			  .setSelected(true);
      	}
    }
  }


  // sets the location of d so that it is centered over this
  private final void centerDialog(Dialog d)
  {
    Point parentLocation = this.getLocation();
    Dimension parentSize = this.getSize();
    Dimension dialogSize = d.getSize();
    int x = parentLocation.x + (parentSize.width - dialogSize.width) / 2;
    int y = parentLocation.y + (parentSize.height - dialogSize.height) / 2;
    d.setLocation(x, y);
  }

} // end class MainFrame
