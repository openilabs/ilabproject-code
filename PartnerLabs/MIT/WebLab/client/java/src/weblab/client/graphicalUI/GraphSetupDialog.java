/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import java.awt.*;
import java.awt.event.*;
import javax.swing.*;
import javax.swing.event.*;

import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.Set;
import java.util.WeakHashMap;
import java.util.ArrayList;

import java.util.Observer;
import java.util.Observable;

import weblab.toolkit.graphing.*;
import weblab.toolkit.util.ChangeTrackingObservable;
import weblab.toolkit.util.Spacer;
import weblab.toolkit.util.CompactComboBox;

import weblab.client.Experiment;
import weblab.client.ExperimentResult;


//XXX pretty much the entire class, for now.

public class GraphSetupDialog extends JDialog
  implements ActionListener, ChangeListener, Observer
{
  private MainFrame theMainFrame;

  private GraphCanvasManager gcm;

  private JRadioButton normalExperimentMode, appendExperimentMode;

  private JSpinner numYAxes;

  private JSpinner numXDivs, numYDivs;
  private JCheckBox xDivsAuto, yDivsAuto;
  private JComboBox yDivsAxis;

  private JPanel graphsPanel;

  private JButton addGraphButton;

  private static final int WIDTH = 500;
  private static final int HEIGHT = 500;

  //XXX add comments
  private List graphDisplays;

  // Graphs currently in the GCM [but no longer represented in
  // graphDisplays] that will be deleted when the user clicks OK/Apply
  private List deletedGraphs;

  // the X Axis
  private Axis xAxis;

  // Y Axes that are presumed to exist within the scope of this
  // dialog, based on the current value of numYAxes.  This may include
  // some new Y Axes that will not be added to the GCM until the user
  // clicks OK/Apply.
  private List yAxes;

  // Y Axes currently in the GCM [but no longer in yAxes] that will be
  // deleted when the user clicks OK/Apply
  private List deletedYAxes;

  private ResultsPanel theResultsPanel;//XXX

  // keys are JComboBoxes whose contents should always be kept in sync
  // with that of yAxes
  private WeakHashMap yAxisChoosers;



  public GraphSetupDialog(MainFrame theMainFrame, GraphCanvasManager gcm,
			  ResultsPanel theResultsPanel)
  {
    // create a non-modal dialog
    super(theMainFrame, "Advanced Graph Setup", false);

    this.theMainFrame = theMainFrame;

    this.theResultsPanel = theResultsPanel;//XXX

    this.graphDisplays = new ArrayList();
    this.deletedGraphs = new ArrayList();

    this.yAxes = new ArrayList();
    this.deletedYAxes = new ArrayList();

    yAxisChoosers = new WeakHashMap();

    this.gcm = gcm;
    gcm.addWeakReferenceObserver(this);
    gcm.getGrid().addWeakReferenceObserver(this);

    // create functional components

    normalExperimentMode = new JRadioButton
      ("<html><b>Replace</b> existing Current Result graphs with graphs from the new experiment.</html>");
    normalExperimentMode.setVerticalTextPosition(SwingConstants.TOP);

    appendExperimentMode = new JRadioButton
      ("<html><b>Preserve</b> existing Current Result graphs by converting them to Custom graphs, and graph the new experiment using different colors.</html>");
    appendExperimentMode.setVerticalTextPosition(SwingConstants.TOP);

    ButtonGroup experimentModeGroup = new ButtonGroup();
    experimentModeGroup.add(normalExperimentMode);
    experimentModeGroup.add(appendExperimentMode);

    numXDivs = new JSpinner(new SpinnerNumberModel(10, 2, 30, 1));

    numYDivs = new JSpinner(new SpinnerNumberModel(10, 2, 30, 1));

    numYAxes = new JSpinner(new SpinnerNumberModel(1, 1, 10, 1));

    xDivsAuto = new JCheckBox("follow X axis");
    xDivsAuto.setActionCommand("xDivsMode");

    yDivsAuto = new JCheckBox("follow");
    yDivsAuto.setActionCommand("yDivsMode");

    yDivsAxis = new JComboBox();
    yDivsAxis.setEditable(false);
    yAxisChoosers.put(yDivsAxis, null);

    // create buttons

    addGraphButton = new JButton("Add Custom Graph");
    addGraphButton.setActionCommand("addGraph");

    JButton okButton = new JButton("OK");
    okButton.setActionCommand("ok");

    JButton applyButton = new JButton("Apply");
    applyButton.setActionCommand("apply");

    JButton cancelButton = new JButton("Cancel");
    cancelButton.setActionCommand("cancel");

    // layout Experiments panel

    JPanel experimentsPanel = new JPanel(new GridBagLayout());
    experimentsPanel.setBorder
      (BorderFactory.createTitledBorder("Experiments"));
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.gridx = 0;
    gbc.weightx = 1;
    gbc.fill = GridBagConstraints.HORIZONTAL;
    gbc.anchor = GridBagConstraints.WEST;
    gbc.insets = new Insets(2, 2, 2, 2); // top, left, bottom, right
    experimentsPanel.add
      (new JLabel("When I run a new experiment in this tab:"), gbc);
    gbc.insets = new Insets(2, 12, 2, 2); // top, left, bottom, right
    experimentsPanel.add(normalExperimentMode, gbc);
    experimentsPanel.add(appendExperimentMode, gbc);

    // layout Axes panel

    JPanel axesPanel = new JPanel(new GridBagLayout());
    axesPanel.setBorder(BorderFactory.createTitledBorder("Axes"));
    axesPanel.add(new JLabel("Y Axes: "));
    axesPanel.add(numYAxes);

    // layout Grid panel

    JPanel yDivsAutoPanel = new JPanel(new GridBagLayout());
    gbc = new GridBagConstraints();
    yDivsAutoPanel.add(yDivsAuto, gbc);
    yDivsAutoPanel.add(yDivsAxis, gbc);
    yDivsAutoPanel.add(new JLabel(" axis"), gbc);

    JPanel gridPanel = new JPanel(new GridBagLayout());
    gridPanel.setBorder(BorderFactory.createTitledBorder("Grid"));
    gbc.insets = new Insets(2, 2, 2, 2); // top, left, bottom, right
    gbc.gridx = 0;
    gbc.weightx = 1;
    gridPanel.add(Box.createGlue(), gbc);

    gbc.gridx = 1;
    gbc.weightx = 0;
    gridPanel.add(new JLabel("Horizontal divisions: "), gbc);
    gridPanel.add(new JLabel("Vertical divisions: "), gbc);

    gbc.gridx = 2;
    gbc.weightx = 2;
    gridPanel.add(numXDivs, gbc);
    gridPanel.add(numYDivs, gbc);

    gbc.gridx = 3;
    gbc.weightx = 1;
    gbc.anchor = GridBagConstraints.WEST;
    gridPanel.add(xDivsAuto, gbc);
    gridPanel.add(yDivsAutoPanel, gbc);

    // layout Graphs panel

    graphsPanel = new JPanel(new GridBagLayout());

    JScrollPane graphsPanelScrollPane = new JScrollPane(graphsPanel);
    graphsPanelScrollPane.setBorder
      (BorderFactory.createTitledBorder("Graphs"));
    // make the scrollbar scroll at a reasonable rate
    graphsPanelScrollPane.getVerticalScrollBar().setUnitIncrement(10);

    // layout button panel

    JPanel buttonPanel = new JPanel();
    buttonPanel.add(okButton);
    buttonPanel.add(applyButton);
    buttonPanel.add(cancelButton);

    // layout dialog

    Container c = this.getContentPane();
    c.setLayout(new GridBagLayout());

    gbc = new GridBagConstraints();
    gbc.fill = GridBagConstraints.BOTH;
    gbc.gridwidth = GridBagConstraints.REMAINDER;
    gbc.weightx = 1;
    gbc.insets = new Insets(2, 2, 2, 2); // top, left, bottom, right

    c.add(experimentsPanel, gbc);

    gbc.gridwidth = 1;
    gbc.weightx = 0.3;
    c.add(axesPanel, gbc);

    gbc.weightx = 0.7;
    gbc.gridwidth = GridBagConstraints.REMAINDER;
    c.add(gridPanel, gbc);

    gbc.weightx = 1;
    gbc.weighty = 1;
    c.add(graphsPanelScrollPane, gbc);

    gbc.weighty = 0;
    c.add(buttonPanel, gbc);

    this.setSize(new Dimension(WIDTH, HEIGHT));

    // initialize

    revert();

    // set up event listeners

    numYAxes.addChangeListener(this);
    xDivsAuto.addActionListener(this);
    yDivsAuto.addActionListener(this);
    addGraphButton.addActionListener(this);
    okButton.addActionListener(this);
    applyButton.addActionListener(this);
    cancelButton.addActionListener(this);
  }



  // XXX may not need this once Experiments collection stuff is
  // working properly
  public void updateExperiments()
  {
    // update Experiments in all GraphDisplays
    for (int i = 0, n = graphDisplays.size(); i < n; i++)
    {
      ((GraphDisplay) graphDisplays.get(i)).updateExperiments();
    }
  }



  // call after modifying yAxes to propagate the changes to all
  // registered yAxisChoosers
  private void updateYAxisChoosers()
  {
    for (Iterator i = yAxisChoosers.keySet().iterator(); i.hasNext(); )
    {
      JComboBox jc = (JComboBox) i.next();

      setJComboBoxContents(jc, yAxes);
    }
  }



  // XXX spec
  //
  // returns true iff we were successfully able to reselect the
  // previously selected item
  //
  // required: target is not editable
  private static boolean setJComboBoxContents(JComboBox target,
					      Collection contents)
  {
    // remember currently selected item
    Object o = target.getSelectedItem();

    target.removeAllItems();

    // add an item for each element of contents
    for (Iterator i = contents.iterator(); i.hasNext(); )
      target.addItem(i.next());

    // try to reselect previously selected item.  If none, or if that
    // item isn't a choice anymore, this will just leave the
    // selection at index 0 (where it goes by default when items are
    // added to an empty JComboBox).
    if (o != null)
      target.setSelectedItem(o);

    target.revalidate();

    return (o == null ?
	    target.getSelectedItem() == null :
	    o.equals(target.getSelectedItem()));
  }



  public void updateAxes()
  {
    // add or delete Y Axes (and associated current-experiment graphs)
    // as necessary to match the spinner
    int newN = ((Integer) numYAxes.getValue()).intValue();
    if (newN > yAxes.size())
      for (int i = yAxes.size(); i < newN; i++)
      {
	Axis a;
	if (deletedYAxes.isEmpty())
	{
	  // create a brand new Y Axis
	  a = new Axis("Y" + (i+1));
	  yAxes.add(a);
	  // create GraphDisplay for new associated current-experiment graph
	  graphDisplays.add(i, new GraphDisplay(null, true, a));//XXX
	}
	else
	{
	  // "undelete" the last Y Axis scheduled for deletion
	  a = (Axis) deletedYAxes.remove(deletedYAxes.size() - 1);
	  yAxes.add(a);
	  // find the current-experiment graph from GCM and undelete
	  // that too
	  Graph g = (Graph) gcm.getGraphs().get(i);//XXX
	  deletedGraphs.remove(g);
	  graphDisplays.add(i, new GraphDisplay(g, true));
	}
      }
    else
      for (int i = yAxes.size()-1; i >= newN; i--)
      {
	// schedule highest-numbered remaining Y Axis for deletion
	Axis a = (Axis) yAxes.remove(i);
	//XXX if it's already in the GCM, schedule it to be deleted
	if (gcm.getYAxes().contains(a))
	  deletedYAxes.add(a);
	// delete associated current-experiment graph and remove its
	// GraphDisplay
	((GraphDisplay) graphDisplays.get(i)).delete();
      }

    updateYAxisChoosers();

    updateGraphsPanel();//XXX because we might have added graphDisplays
  }



  /**
   * Handle changes in Observables
   */
  public void update(Observable o, Object arg)
  {
    // when GraphCanvasManager, import values
    if (o == gcm)
      revert();//XXX???  something more sophisticated?

    // when Grid changes, import values only from Grid (don't do a
    // full revert here, since Grid might send a change in response to
    // something as irrelevant to us as selecting a different variable
    // for graphing on a logscale axis)
    else if (o == gcm.getGrid())
      importGridValues();
  }



  /**
   * Handle ActionEvents
   */
  public final void actionPerformed(ActionEvent evt)
  {
    String cmd = evt.getActionCommand();

    // when Add Graph is clicked, add a new GraphDisplay
    if (cmd.equals("addGraph"))
    {
      graphDisplays.add(new GraphDisplay(null, false));
      updateGraphsPanel();
    }

    // when OK is clicked, export values and then hide the dialog
    else if (cmd.equals("ok"))
    {
      apply();
      revert(); // to remove extraneous blank UDFdisplays
      setVisible(false);
    }

    // when Apply is clicked, export values
    else if (cmd.equals("apply"))
    {
      apply();
      revert(); // to remove extraneous blank UDFdisplays
    }

    // when Cancel is clicked, re-import values and then hide the dialog
    else if (cmd.equals("cancel"))
    {
      revert();
      setVisible(false);
    }

    else if (cmd.equals("xDivsMode"))
      reconcile();

    else if (cmd.equals("yDivsMode"))
      reconcile();

    else
      throw new Error("invalid action command: " + cmd);
  }



  /**
   * Handle ChangeEvents
   */
  public final void stateChanged(ChangeEvent evt)
  {
    if (evt.getSource() == numYAxes)
      updateAxes();
  }



  private void apply()
  {
    theResultsPanel.setAppendMode(appendExperimentMode.isSelected());

    // add new Y Axes
    for (Iterator i = yAxes.iterator(); i.hasNext(); )
    {
      // note: method automatically ignores axes that are already
      // present in GCM
      gcm.addYAxis((Axis) i.next());
    }

    // apply all changes made to all GraphDisplays (includes modified
    // Graphs and newly added Graphs)
    for (int i = 0, n = graphDisplays.size(); i < n; i++)
    {
      ((GraphDisplay) graphDisplays.get(i)).exportValues();
    }

    // delete all Graphs that have been scheduled for deletion (by
    // removing their GraphDisplays)
    for (Iterator i = deletedGraphs.iterator(); i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      gcm.removeGraph(g);
    }
    deletedGraphs.clear();

    // remove deleted Y Axes
    for (Iterator i = deletedYAxes.iterator(); i.hasNext(); )
      gcm.removeYAxis((Axis) i.next());
    deletedYAxes.clear();

    // notify observers that the Graphs may have changed (all at once,
    // to avoid temporary states of inconsistent units on an axis)
    for (int i = 0, n = graphDisplays.size(); i < n; i++)
    {
      //XXX a bit kludgey
      ((GraphDisplay) graphDisplays.get(i)).theGraph.notifyObservers();
    }

    // apply changes to grid settings
    Grid theGrid = gcm.getGrid();

    if (xDivsAuto.isSelected())
    {
      theGrid.setXFollow(xAxis);
    }
    else
    {
      theGrid.setXFollow(null);
      theGrid.setXDivs(((Integer) numXDivs.getValue()).intValue());
    }

    if (yDivsAuto.isSelected())
    {
      theGrid.setYFollow((Axis) yDivsAxis.getSelectedItem());
    }
    else
    {
      theGrid.setYFollow(null);
      theGrid.setYDivs(((Integer) numYDivs.getValue()).intValue());
    }

    // notify other observers that gcm may have changed
    gcm.notifyObservers();

    // notify other observers that Grid may have changed
    theGrid.notifyObservers();
  }



  private void reconcile()
  {
    numXDivs.setEnabled(! xDivsAuto.isSelected());

    numYDivs.setEnabled(! yDivsAuto.isSelected());
    yDivsAxis.setEnabled(yDivsAuto.isSelected());
  }



  private void importGridValues()
  {
    Grid theGrid = gcm.getGrid();

    numXDivs.setValue(new Integer(theGrid.getXDivs()));

    xDivsAuto.setSelected(theGrid.getXFollow() != null);

    numYDivs.setValue(new Integer(theGrid.getYDivs()));

    Axis yFollow = theGrid.getYFollow();
    yDivsAuto.setSelected(yFollow != null);

    if (yFollow != null)
      yDivsAxis.setSelectedItem(yFollow);

    reconcile();
  }



  public void revert()
  {
    if (theResultsPanel.getAppendMode())//XXX
      appendExperimentMode.setSelected(true);
    else
      normalExperimentMode.setSelected(true);

    // clear graph deletion queue
    deletedGraphs.clear();

    // reset X Axis
    this.xAxis = (Axis) gcm.getXAxes().get(0);

    // reset Y Axes
    this.yAxes = new ArrayList(gcm.getYAxes());
    deletedYAxes.clear();

    numYAxes.setValue(new Integer(yAxes.size()));

    updateYAxisChoosers();

    // clear existing GraphDisplays
    for (Iterator i = graphDisplays.iterator(); i.hasNext(); )
      ((GraphDisplay) i.next()).destroy();

    graphDisplays.clear();

    importGridValues();

    // add a GraphDisplay for each Graph in GraphCanvasManager
    for (Iterator i = gcm.getGraphs().iterator(); i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      graphDisplays.add
	(new GraphDisplay(g, theResultsPanel.isCurrentExperimentGraph(g)));
    }

    // update graphs panel
    updateGraphsPanel();
  }



  private void updateGraphsPanel()
  {
    GridBagConstraints gbc;

    // clear the graphs panel of all components
    graphsPanel.removeAll();

    // Add column labels
    gbc = new GridBagConstraints();
    gbc.anchor = GridBagConstraints.CENTER;
    // top, left, bottom, right
    gbc.insets = new Insets(0, 5, 0, 5);

    graphsPanel.add(new JLabel("Y Axis"), gbc);
    graphsPanel.add(new JLabel("Color"), gbc);

    // experiment should take up extra available horizontal space
    gbc.weightx = 1;
    graphsPanel.add(new JLabel("Experiment"), gbc);
    gbc.weightx = 0;

    graphsPanel.add(new JLabel("Y Var"), gbc);
    graphsPanel.add(new JLabel("X Var"), gbc);
    gbc.gridwidth = GridBagConstraints.REMAINDER;
    graphsPanel.add(Spacer.createHorizontalDoppelganger
		    (new JButton("Remove"), false), gbc);

    gbc.fill = GridBagConstraints.HORIZONTAL;
    // top, left, bottom, right
    gbc.insets = new Insets(2, 0, 3, 0);
    graphsPanel.add(new JSeparator(), gbc);
    gbc.fill = GridBagConstraints.NONE;

    // Add a row for each GraphDisplay in graphDisplays
    for (Iterator i = graphDisplays.iterator(); i.hasNext(); )
    {
      GraphDisplay d = (GraphDisplay) i.next();

      gbc = new GridBagConstraints();
      // top, left, bottom, right
      gbc.insets = new Insets(2, 2, 2, 2);

      if (d.isCurrentExperimentGraph)
      {
	graphsPanel.add(d.yAxisLabel, gbc);
	gbc.fill = GridBagConstraints.VERTICAL;
	graphsPanel.add(d.colorButton, gbc);
	gbc.fill = GridBagConstraints.NONE;
	graphsPanel.add(d.experimentLabel, gbc);
	graphsPanel.add(d.yVarLabel, gbc);
	graphsPanel.add(d.xVarLabel, gbc);
	gbc.gridwidth = GridBagConstraints.REMAINDER;
	graphsPanel.add(Box.createGlue(), gbc);
      }
      else
      {
	graphsPanel.add(d.yAxis, gbc);

	gbc.fill = GridBagConstraints.VERTICAL;
	graphsPanel.add(d.colorButton, gbc);

	// experiment should take up extra available horizontal space
	gbc.fill = GridBagConstraints.HORIZONTAL;
	gbc.weightx = 1;
	graphsPanel.add(d.experiment, gbc);
	gbc.fill = GridBagConstraints.NONE;
	gbc.weightx = 0;

	graphsPanel.add(d.yVar, gbc);
	graphsPanel.add(d.xVar, gbc);
	
	// deleteButton should be the end of a row
	gbc.gridwidth = GridBagConstraints.REMAINDER;
	graphsPanel.add(d.deleteButton, gbc);      
      }
    }

    // Add button to add a new graph
    gbc.anchor = GridBagConstraints.WEST;
    graphsPanel.add(Box.createVerticalStrut(5), gbc);
    graphsPanel.add(addGraphButton, gbc);

    // Add a blank row to take up the remaining vertical space (so
    // that the other components will appear top-aligned rather than
    // vertically centered)
    gbc = new GridBagConstraints();
    gbc.weighty = 1.0;
    graphsPanel.add(new JPanel(), gbc);

    // revalidate (whole dialog box instead of just graphsPanel
    // because scrollPane may also need it)
    this.validate();
    this.repaint();
  }



  static int gdSerial=0;//XXX

  // contains the GUI components for one line of the Graph table
  // (i.e. everything we need to edit a single Graph)
  private class GraphDisplay implements Observer, ActionListener
  {
    Graph theGraph;
    boolean isCurrentExperimentGraph;
    Axis myYAxis; // for current-experiment GD's only

    JComboBox experiment;
    JComboBox xVar, yVar, yAxis;
    JButton colorButton, deleteButton;

    JLabel experimentLabel, xVarLabel, yVarLabel, yAxisLabel;

    int mySerialNum;//XXX for debugging only

    // XXX these constructor method signatures feel kludgey

    // Set g to null if this GraphDisplay should create a new Graph
    // (and add it to the GraphCanvasManager) when exportValues is
    // called, instead of modifying an existing one.
    public GraphDisplay(Graph g, boolean isCurrentExperimentGraph)
    { this(g, isCurrentExperimentGraph, null); }

    public GraphDisplay(Graph g, boolean isCurrentExperimentGraph, Axis targetAxis)
    {
      theGraph = g;
      this.isCurrentExperimentGraph = isCurrentExperimentGraph;

      if (isCurrentExperimentGraph)
      {
	if (theGraph != null)
	  myYAxis = theGraph.getYAxis();
	else
	  myYAxis = targetAxis; //XXX what about null?
      }

      mySerialNum = gdSerial++;//XXX

      experiment = new CompactComboBox();
      experiment.setEditable(false);
      experiment.addItem("None ");
      experiment.addItem("<< Load Archived Experiment... >>");
      experiment.addActionListener(this);
      // use "None " to calculate minimum/preferred size for
      // experiment.  GridBagLayout will then expand it to fill all
      // available extra space (but no more than that)
      experiment.setPrototypeDisplayValue("None ");

      experimentLabel = new JLabel("Current Result");

      xVar = new JComboBox();
      xVar.setEditable(false);
      xVar.addItem("None ");

      xVarLabel = new JLabel("None");

      yVar = new JComboBox();
      yVar.setEditable(false);
      yVar.addItem("None ");

      yVarLabel = new JLabel("None");

      yAxis = new JComboBox();
      yAxis.setEditable(false);
      yAxisChoosers.put(yAxis, null);

      yAxisLabel = new JLabel(myYAxis == null ? "None" : myYAxis.getName());

      colorButton = new JButton(new ColorIcon());
      colorButton.setActionCommand("color");
      colorButton.addActionListener(this);
      colorButton.setMargin(new Insets(1, 1, 1, 1));

      deleteButton = new JButton("Remove");
      deleteButton.setActionCommand("delete");
      deleteButton.addActionListener(this);

      updateExperiments();
      setJComboBoxContents(yAxis, yAxes);

      if (theGraph != null)
      {
	theGraph.addWeakReferenceObserver(this);
	importValues();
      }
      else
      {
	colorButton.setForeground(theResultsPanel.nextColor());
	if (this.isCurrentExperimentGraph)
	  setVisible(false);//XXX?
      }
    }



    public void setVisible(boolean visible)
    {
      experiment.setVisible(visible);
      xVar.setVisible(visible);
      yVar.setVisible(visible);
      yAxis.setVisible(visible);
      colorButton.setVisible(visible);
      deleteButton.setVisible(visible);
      experimentLabel.setVisible(visible);
      xVarLabel.setVisible(visible);
      yVarLabel.setVisible(visible);
      yAxisLabel.setVisible(visible);
    }



    // apply all changes made to this GraphDisplay (except the delete
    // button, which is handled separately), but do NOT notify
    // observers yet
    public void exportValues()
    {
      if (theGraph == null)
      {
	theGraph = new Graph();
	theGraph.setXAxis(xAxis);

	if (isCurrentExperimentGraph)
	{
	  theGraph.setYAxis(myYAxis);
	  //XXX??
	  int yAxisNumber = Integer.parseInt(myYAxis.getName().substring(1));
	  gcm.addGraph(yAxisNumber - 1, theGraph);

	  theResultsPanel.setCurrentExperimentGraph(theGraph, true);
	}
	else
	  gcm.addGraph(theGraph);
      }

      // set ConnectPattern (based on the selected Experiment)

      Object objExp = experiment.getSelectedItem();
      if (objExp instanceof Experiment)
      {
	Experiment exp = (Experiment) objExp;
	theGraph.setConnectPattern(exp.getConnectPattern());
      }

      // set color

      theGraph.setColor(colorButton.getForeground());

      if (! isCurrentExperimentGraph)
      {
	// set X and Y variables

	int i = xVar.getSelectedIndex();
	theGraph.setXVariable(i == 0 ? null : (Variable) xVar.getItemAt(i));
	i = yVar.getSelectedIndex();
	theGraph.setYVariable(i == 0 ? null : (Variable) yVar.getItemAt(i));

	// set Y axis

	theGraph.setYAxis((Axis) yAxis.getSelectedItem());
      }
    }



    // import values from theGraph (but do NOT update choices for
    // Experiments or Axes; those depend on other things)
    public void importValues()
    {
      if (theGraph == null)
	return;

      this.setVisible(true);

      ExperimentResult er = ExperimentResult
	.getExperimentResultForVariable(theGraph.getXVariable());

      if (er == null)
	er = ExperimentResult
	.getExperimentResultForVariable(theGraph.getYVariable());

      selectExperimentWithResult(er); // will automatically update
				      // variable choices

      Variable v = theGraph.getXVariable();
      xVar.setSelectedIndex(0);
      if (v != null)
	xVar.setSelectedItem(v); // won't change selection if not found

      xVarLabel.setText(v == null ? "None" : v.toString());

      if (this.isCurrentExperimentGraph && v == null)
	this.setVisible(false); // don't display non-active
				// current-experiment graphs

      v = theGraph.getYVariable();
      yVar.setSelectedIndex(0);
      if (v != null)
	yVar.setSelectedItem(v); // won't change selection if not found

      yVarLabel.setText(v == null ? "None" : v.toString());

      if (this.isCurrentExperimentGraph && v == null)
	this.setVisible(false); // don't display non-active
				// current-experiment graphs

      Axis yA = theGraph.getYAxis();
      yAxis.setSelectedItem(yA);

      yAxisLabel.setText(yA == null ? "None" : yA.toString());


      colorButton.setForeground(theGraph.getColor());
    }



    public void delete()
    {
      // if the Graph already existed, clicking OK/Apply should
      // cause it to be removed from the GraphCanvasManager
      if (theGraph != null)
	deletedGraphs.add(theGraph);

      // remove this GraphDisplay from the dialog
      graphDisplays.remove(this);
      updateGraphsPanel();
      this.destroy();
    }

    // call when finished with this GraphDisplay
    public void destroy()
    {
      if (theGraph != null)
	theGraph.deleteObserver(this);
    }



    // handle updates from Observables
    public void update(Observable o, Object arg)
    {
      // when theGraph changes an Axis, Variable, Color, or other
      // miscellaneous changes, importValues
      if (o == theGraph)
      {
	Set changes = ChangeTrackingObservable.extractChanges(arg);

	if (changes.contains(Graph.AXIS_CHANGE) ||
	    changes.contains(Graph.VARIABLE_CHANGE) ||
	    changes.contains(Graph.COLOR_CHANGE) ||
	    changes.contains(Graph.UNSPECIFIED_CHANGE))
	  importValues();
      }

      //XXX GraphCanvasManager, Experiments?
    }



    // handle ActionEvents
    public final void actionPerformed(ActionEvent evt)
    {
      String cmd = evt.getActionCommand();

      // when Delete is clicked, remove this GraphDisplay
      if (cmd.equals("delete"))
      {
	this.delete();
      }

      // when color button is clicked, choose a new color
      else if (cmd.equals("color"))
      {
	Color newColor = JColorChooser.showDialog
	  (theMainFrame, "Select a Color", colorButton.getForeground());
	if (newColor != null)
	  colorButton.setForeground(newColor);
      }

      // when a new experiment is selected, update variable choices
      // accordingly
      else if (cmd.equals("experimentSelect"))
      {
	// check for selection of the Load item
	if (experiment.getSelectedIndex() == experiment.getItemCount() - 1)
	{
	  // open a LoadExperimentDialog that loads only into the
	  // recently executed experiments list, not into any current
	  // tab
	  LoadExperimentDialog led =
	    new LoadExperimentDialog(theMainFrame, null);
	  theMainFrame.centerDialog(led);
	  Experiment exp = led.loadExperiment();

	  // XXX this really should be auto-triggered but currently
	  // isn't (note: isn't ever, not just isn't in this
	  // particular case)
	  GraphSetupDialog.this.updateExperiments();

	  // select the experiment we just loaded, or None (if LED was
	  // cancelled by user, the experiment contained an error, etc)
	  if (exp != null)
	    experiment.setSelectedItem(exp);
	  else
	    experiment.setSelectedIndex(0);
	}

	updateVariables(xVar);
	updateVariables(yVar);
      }
    }



    // repopulate the Experiment selection box
    public void updateExperiments()
    {
      // temporarily disable the action command so that we won't wind
      // up updating variables incorrectly / unnecessarily
      experiment.setActionCommand("DISABLED");

      List contents = new ArrayList(theMainFrame.getRecentExperiments());
      contents.add(0, "None");
      contents.add("<< Load Archived Experiment... >>");
      boolean ok = setJComboBoxContents(experiment, contents);

      // re-enable action command
      experiment.setActionCommand("experimentSelect");

      if (! ok) 
      {
	updateVariables(xVar);
	updateVariables(yVar);
      }
    }



    // repopulate the specified variable selection box
    private void updateVariables(JComboBox var)
    {
      // remember current selection
      Object o = var.getSelectedItem();

      // if current selection is a Variable, remember its name too.
      String name = null;
      if (o instanceof Variable)
	name = ((Variable) o).getName();

      // remove all items except the first one ("None")
      while (var.getItemCount() > 1)
	var.removeItemAt(1);

      // add an item for each Variable in the ExperimentResult of the
      // currently selected Experiment, if any.  As we're looping
      // through, also keep track of the first variable we encounter
      // with the same name as the previously selected variable.

      Variable sameNameVar = null;

      Object objExp = experiment.getSelectedItem();
      if (objExp instanceof Experiment)
      {
	Experiment exp = (Experiment) objExp;
	for (Iterator i = exp.getExperimentResult().getVariables().iterator();
	     i.hasNext(); )
	{
	  Variable v = (Variable) i.next();
	  var.addItem(v);
	  if (sameNameVar == null && v.getName().equals(name))
	    sameNameVar = v;
	}
      }

      // try to reselect previous selection
      var.setSelectedItem(o);

      // If that didn't work, but we found a variable with the same
      // name as the previous selection, select that one.  Otherwise
      // select index 0 as a fallback.
      if (var.getSelectedItem() != o)
      {
	if (sameNameVar != null)
	  var.setSelectedItem(sameNameVar);
	else
	  var.setSelectedIndex(0);
      }

      var.revalidate();
    }



    // selects the Experiment containing the specified
    // ExperimentResult, if possible.  Otherwise select index 0.
    private void selectExperimentWithResult(ExperimentResult er)
    {
      // skip index 0
      for (int i = 1, n = experiment.getItemCount(); i < n; i++)
      {
	Object o = experiment.getItemAt(i);
	if (o instanceof Experiment &&
	    ((Experiment) o).getExperimentResult() == er)
	{
	  experiment.setSelectedIndex(i);
	  return;
	}
      }

      // didn't find it
      experiment.setSelectedIndex(0);
    }

  } // end inner class GraphDisplay



  // A rectangular icon that takes its color from the foreground color
  // of the JButton it's on
  private class ColorIcon implements Icon
  {
    public int getIconHeight()
    {
      return 15;
    }

    public int getIconWidth()
    {
      return 25;
    }

    public void paintIcon(Component c, Graphics g, int x, int y)
    {
      g.setColor(c.getForeground());
      g.fillRect(x, y, getIconWidth(), getIconHeight());
    }
  } // end inner class ColorIcon

} // end class GraphSetupDialog
