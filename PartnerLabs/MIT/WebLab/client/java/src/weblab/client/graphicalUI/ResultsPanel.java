/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import java.awt.*;
import javax.swing.*;
import java.awt.event.*;

import java.math.BigDecimal;

import java.util.Collections;
import java.util.Collection;
import java.util.Iterator;
import java.util.Map;
import java.util.List;
import java.util.HashMap;
import java.util.WeakHashMap;
import java.util.ArrayList;
import java.util.Observable;
import java.util.Observer;
import java.util.Random;

import weblab.toolkit.graphing.*;

import weblab.toolkit.util.ChangeTrackingObservable;
import weblab.toolkit.util.Spacer;
import weblab.toolkit.util.EngUnits;
import weblab.toolkit.util.EngValue;
import weblab.toolkit.util.EngValueField;

import weblab.client.*;


public class ResultsPanel extends JPanel
  implements Observer, ActionListener, WindowListener, TrackingListener
{
  private ResultsPanel self;

  // number of sigfigs to display (for temperature and tracking)
  private static final int NUM_SIGFIGS = 4;

  protected MainFrame theMainFrame;

  protected Experiment mostRecentExperiment;

  AxisPanel xAxisPanel;
  private List yAxisPanels; // List of AxisPanel
  private List apSeparators; // List of JSeparators between AxisPanels

  protected GraphCanvasManager graphCanvasManager;
  private JPanel graphCanvas;

  private final int GRAPH_WIDTH = 360;
  private final int GRAPH_HEIGHT = 270;

  private static final Dimension TRACKING_DIALOG_SIZE =
    new Dimension(200, 250);

  // snap tracking will only activate if mouse is within this number
  // of pixels of the data point
  private final double SNAP_TRACKING_THRESHOLD = 20;

  private JLabel expLabel, temperature;

  private JEditorPane trackingPane;

  protected JDialog trackingDialog;
  private JButton trackingButton, graphSetupButton;
  private JCheckBox snapTracking;

  // whether we've opened the tracking dialog yet
  private boolean trackingInit = false;

  protected GraphSetupDialog graphSetupDialog;
  private boolean appendMode = false; //XXX kludge!

  private Random random = new Random();
  private Iterator defaultColorIterator; // of Color

  // maps Graph to null.  Presence of Graph g in this map means that g
  // is a current-experiment graph (one of the ones controlled
  // directly from an AxisPanel) as opposed to a Custom graph
  // controlled from the Graph Setup Dialog.
  private WeakHashMap currentExperimentGraphs;


  //////////////////
  // Constructors //
  //////////////////



  public ResultsPanel(MainFrame theMainFrame)
  {
    self = this;

    this.theMainFrame = theMainFrame;

    mostRecentExperiment = new Experiment("(no current result)");

    currentExperimentGraphs = new WeakHashMap();

    Axis xAxis = new Axis("X"),
      y1Axis = new Axis("Y1"),
      y2Axis = new Axis("Y2");

    Graph Y1vsX = new Graph();
    setCurrentExperimentGraph(Y1vsX, true);
    Y1vsX.setXAxis(xAxis);
    Y1vsX.setYAxis(y1Axis);
    Y1vsX.setColor(Color.blue);

    Graph Y2vsX = new Graph();
    setCurrentExperimentGraph(Y2vsX, true);
    Y2vsX.setXAxis(xAxis);
    Y2vsX.setYAxis(y2Axis);
    Y2vsX.setColor(Color.red);

    graphCanvas = new JPanel() {
	public void paintComponent(Graphics g)
	{
	  // paint background
	  super.paintComponent(g);
	  // paint canvas
	  graphCanvasManager.paintCanvas(g);
	}
      };

    graphCanvas.setMinimumSize(new Dimension(GRAPH_WIDTH, GRAPH_HEIGHT));
    graphCanvas.setPreferredSize(new Dimension(GRAPH_WIDTH, GRAPH_HEIGHT));
    graphCanvas.setBackground(Color.white);

    // contract for Weblab usage of GraphCanvas: there is always one X
    // Axis, and at least one Y Axis.  For every Y Axis, there is one
    // "current-experiment graph" of that Y axis versus the X axis.
    // These graphs are placed at the beginning of the list of Graphs
    // in GraphCanvasManager, before any extra graphs.
    graphCanvasManager = new GraphCanvasManager(graphCanvas);
    graphCanvasManager.setSnapTracking(true);
    graphCanvasManager.addXAxis(xAxis);
    graphCanvasManager.addYAxis(y1Axis);
    graphCanvasManager.addGraph(Y1vsX);
    graphCanvasManager.addYAxis(y2Axis);
    graphCanvasManager.addGraph(Y2vsX);
    graphCanvasManager.notifyObservers();

    graphSetupDialog = new GraphSetupDialog
      (theMainFrame, graphCanvasManager, this);//XXX

    //
    // setup components
    //

    expLabel = new JLabel();
    temperature = new JLabel();

    // XXX big fat hack
    ExperimentResult er = mostRecentExperiment.getExperimentResult();

    Grid theGrid = graphCanvasManager.getGrid();

    xAxisPanel = null;
    yAxisPanels = new ArrayList();
    apSeparators = new ArrayList();

    trackingButton = new JButton("Tracking");
    trackingButton.setActionCommand("tracking");
    trackingButton.addActionListener(this);

    trackingPane = new JEditorPane("text/html", "");
    trackingPane.setEditable(false);

    trackingDialog = new JDialog(theMainFrame, "Tracking", false);
    trackingDialog.addWindowListener(this);

    snapTracking = new JCheckBox("Snap to data points",
				 graphCanvasManager.isSnapTracking());
    snapTracking.setActionCommand("snapTracking");
    snapTracking.addActionListener(this);


    JButton closeButton = new JButton("Close");
    closeButton.setActionCommand("closeTracking");
    closeButton.addActionListener(this);

    graphSetupButton = new JButton("Advanced...");
    graphSetupButton.setActionCommand("graphSetup");
    graphSetupButton.addActionListener(this);

    //
    // setup listeners
    //

    graphCanvasManager.addWeakReferenceObserver(this);
    graphCanvasManager.addTrackingListener(this);
    mostRecentExperiment.addWeakReferenceObserver(this);

    //
    // setup layout
    //

    Container trackingDialogCP = trackingDialog.getContentPane();
    trackingDialogCP.setLayout(new BorderLayout());
    trackingDialogCP.add(snapTracking, BorderLayout.NORTH);
    trackingDialogCP.add(trackingPane, BorderLayout.CENTER);
    trackingDialogCP.add(closeButton, BorderLayout.SOUTH);

    trackingDialog.setSize(TRACKING_DIALOG_SIZE);
    
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.gridx = 0;
    // top, left, bottom, right
    gbc.insets = new Insets(2, 0, 2, 0);

    JPanel temperaturePanel = new JPanel(new GridBagLayout());
    temperaturePanel.add(expLabel, gbc);
    temperaturePanel.add(temperature, gbc);

    this.setLayout(new GridBagLayout());

    gbc = new GridBagConstraints();

    gbc.fill = GridBagConstraints.BOTH;

    gbc.gridx = 1;
    gbc.gridy = 0;
    gbc.weightx = 1;
    gbc.weighty = 0;
    this.add(temperaturePanel, gbc);

    gbc.gridx = 1;
    gbc.gridy = 1;
    gbc.weightx = 1;
    gbc.weighty = 1;
    this.add(graphCanvas, gbc);

    gbc.gridx = 0;
    gbc.gridy = 2;
    gbc.weightx = 0;
    gbc.weighty = 0;
    gbc.fill = GridBagConstraints.NONE;
    this.add(trackingButton, gbc);

    gbc.gridx = 2;
    gbc.gridy = 2;
    gbc.weightx = 0;
    gbc.weighty = 0;
    gbc.fill = GridBagConstraints.NONE;
    this.add(graphSetupButton, gbc);

    updateAxisPanels();

    //
    // initialize
    //

    updateTemperature();
    updateConnectPatterns();
    //XXX updateGrid();

    // init grid (XXX HACK!!!)
    Grid grid = graphCanvasManager.getGrid();
    grid.setXFollow(xAxis);
    grid.setYFollow(y1Axis);
    grid.notifyObservers();

    // setup default colors
    ArrayList colors = new ArrayList();
    colors.add(Color.green.darker());
    colors.add(Color.magenta.darker());
    //colors.add(Color.orange);
    colors.add(Color.yellow.darker());
    colors.add(Color.cyan);
    colors.add(Color.pink.darker());
    defaultColorIterator = colors.iterator();
  }



  /**
   * Changes the "current" Experiment to which this ResultsPanel is assigned.
   */
  public void setExperiment(Experiment newExperiment)
  {
    // stop observing the old Experiment, update our reference, and
    // start observing the new one
    mostRecentExperiment.deleteObserver(this);
    mostRecentExperiment = newExperiment;
    mostRecentExperiment.addWeakReferenceObserver(this);

    //XXX big kludge here
    if (appendMode)
    {
      int numCurrentExperimentGraphs = graphCanvasManager.getYAxes().size();

      // clone active current-experiment graphs, and insert the clones
      // just after all current-experiment graphs in the GCM
      List graphs = graphCanvasManager.getGraphs();
      for (int i = numCurrentExperimentGraphs - 1; i >= 0; i--)
      {
	Graph g = (Graph) graphs.get(i);

	if (g.isActive())
	{
	  Graph gclone = (Graph) g.clone();
	  graphCanvasManager.addGraph(numCurrentExperimentGraphs, gclone);
	  graphCanvasManager.notifyObservers();
	  g.setColor(nextColor());
	  g.notifyObservers();
	}
      }
    }

    //XXX
    ExperimentResult er = mostRecentExperiment.getExperimentResult();
    xAxisPanel.setExperimentResult(er);
    for (Iterator i = yAxisPanels.iterator(); i.hasNext(); )
      ((AxisPanel) i.next()).setExperimentResult(er);
    graphSetupDialog.updateExperiments();//XXX??

    // when there's a new Experiment, there's a new ExpResult too
    updateTemperature();
    updateConnectPatterns();

    // XXX should UNSELECT Variables from y2 or try to reselect with
    // same name.  The below is a big kludge, not just because this
    // really should be done with an Experiment method but also
    // because it doesn't select an X variable at all (since we know
    // that the AxisPanel will take care of that part)
    //
    int numCurrentExperimentGraphs = graphCanvasManager.getYAxes().size();

    List graphs = graphCanvasManager.getGraphs();
    mostRecentExperiment.autoselectGraphVariables
      (/*Y1vsX*/ (Graph) graphs.get(0));//XXX
    for (int i = 1; i < numCurrentExperimentGraphs; i++)
    {
      Graph g = (Graph) graphs.get(i);

      Variable newY2 = null, oldY2 = g.getYVariable();
      // first choice: same name as oldY2
      if (oldY2 != null)
	newY2 = mostRecentExperiment.getExperimentResult()
	  .getVariable(oldY2.getName());
      g.setYVariable(newY2);
      g.notifyObservers();
    }
  }



  public boolean getAppendMode() {return appendMode;}
  public void setAppendMode(boolean foo) {appendMode = foo;}

  public void onTrackingOpen()
  {
    trackingInit = true;

    trackingButton.setEnabled(false);
    trackingDialog.setVisible(true);
    graphCanvasManager.setTrackingEnabled(true);
  }

  public void onTrackingClose()
  {
    trackingButton.setEnabled(true);
    trackingDialog.setVisible(false);
    graphCanvasManager.setTrackingEnabled(false);
  }


  //XXX spec and maybe method name
  public void copySettingsFrom(ResultsPanel target)
  {
    HashMap axisMap = new HashMap();

    // propagate current experiment
    this.setExperiment(target.mostRecentExperiment);

    //
    // propagate append mode
    //

    this.setAppendMode(target.getAppendMode());

    //
    // propagate GCM settings
    //

    // XXX clear GCM of all Graphs and Axes
    for (Iterator i = (new ArrayList(graphCanvasManager.getGraphs()))
	   .iterator();
	 i.hasNext(); )
      graphCanvasManager.removeGraph((Graph) i.next());
    for (Iterator i = (new ArrayList(graphCanvasManager.getXAxes()))
	   .iterator();
	 i.hasNext(); )
      graphCanvasManager.removeXAxis((Axis) i.next());
    for (Iterator i = (new ArrayList(graphCanvasManager.getYAxes()))
	   .iterator();
	 i.hasNext(); )
      graphCanvasManager.removeYAxis((Axis) i.next());

    // clone Axes and create logical map from old Axes to new Axes
    for (Iterator i = target.graphCanvasManager.getXAxes().iterator();
	 i.hasNext(); )
    {
      Axis a = (Axis) i.next();
      Axis b = (Axis) a.clone();
      axisMap.put(a, b);
      this.graphCanvasManager.addXAxis(b);
    }
    for (Iterator i = target.graphCanvasManager.getYAxes().iterator();
	 i.hasNext(); )
    {
      Axis a = (Axis) i.next();
      Axis b = (Axis) a.clone();
      axisMap.put(a, b);
      this.graphCanvasManager.addYAxis(b);
    }

    // clone Graphs but point them to new corresponding Axes
    for (Iterator i = target.graphCanvasManager.getGraphs().iterator();
	 i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      Graph h = (Graph) g.clone();
      h.setXAxis((Axis) axisMap.get(g.getXAxis()));
      h.setYAxis((Axis) axisMap.get(g.getYAxis()));
      this.graphCanvasManager.addGraph(h);
      this.setCurrentExperimentGraph(h, target.isCurrentExperimentGraph(g));
    }

    // notify GCM observers
    this.graphCanvasManager.notifyObservers();

    //
    // propagate Grid settings
    //

    Grid theGrid = this.graphCanvasManager.getGrid(),
      targetGrid = target.graphCanvasManager.getGrid();

    theGrid.setColor(targetGrid.getColor());

    Axis xFollow = (Axis) axisMap.get(targetGrid.getXFollow());
    theGrid.setXFollow(xFollow);
    if (xFollow == null)
      theGrid.setXDivs(targetGrid.getXDivs());

    Axis yFollow = (Axis) axisMap.get(targetGrid.getYFollow());
    theGrid.setYFollow(yFollow);
    if (yFollow == null)
      theGrid.setYDivs(targetGrid.getYDivs());

    // notify Grid observers
    theGrid.notifyObservers();
  }



  public boolean isCurrentExperimentGraph(Graph g)
  {
    return currentExperimentGraphs.containsKey(g);
  }



  public void setCurrentExperimentGraph(Graph g, boolean value)
  {
    if (value)
      currentExperimentGraphs.put(g, null);
    else
      currentExperimentGraphs.remove(g);
  }



  //////////////////////
  // Listener methods //
  //////////////////////


  /**
   * Handle ActionEvents
   */
  public final void actionPerformed(ActionEvent evt)
  {
    String cmd = evt.getActionCommand();

    // when Tracking button is clicked, open the dialog, enable
    // tracking in GCM, and disable the button
    if (cmd.equals("tracking"))
    {
      if (! trackingInit)
      {
	trackingInit = true;

	// set default position for tracking dialog: bottom-left
	// corner aligned with top-left corner of ResultsPanel
	Point parentLocation = self.getLocationOnScreen();
	Dimension dialogSize = trackingDialog.getSize();
	int x = parentLocation.x;
	int y = parentLocation.y - dialogSize.height;
	trackingDialog.setLocation(x, y);
      }

      onTrackingOpen();
    }

    // when tracking Close button is clicked, hide the dialog, disable
    // tracking in GCM, and re-enable the button
    if (cmd.equals("closeTracking"))
      onTrackingClose();

    // when snapTracking is toggled, update GCM accordingly
    if (cmd.equals("snapTracking"))
    {
      graphCanvasManager.setSnapTracking(snapTracking.isSelected());
    }

    // when GraphSetup is clicked, open the dialog
    if (cmd.equals("graphSetup"))
    {
      graphSetupDialog.setVisible(true);
    }
  }


  /**
   * Handle updates from Observables
   */
  public final void update(Observable o, Object arg)
  {
    // When results change, update temperature, update connect
    // patterns, and repaint graphPanel.
    if (o == mostRecentExperiment &&
	ChangeTrackingObservable.extractChanges(arg).contains
	(Experiment.EXPERIMENT_RESULT_CHANGE))
    {
      updateTemperature();
      updateConnectPatterns();

      //XXX
      ExperimentResult er = mostRecentExperiment.getExperimentResult();
      xAxisPanel.setExperimentResult(er);
      for (Iterator i = yAxisPanels.iterator(); i.hasNext(); )
	((AxisPanel) i.next()).setExperimentResult(er);
      graphSetupDialog.updateExperiments();
      
      graphCanvas.repaint();//XXX?? not sure if I will need this
    }

    if (o == graphCanvasManager &&
	ChangeTrackingObservable.extractChanges(arg).contains
	(GraphCanvasManager.Y_AXES_CHANGE))
    {
      updateAxisPanels();
      updateConnectPatterns(); // if a new Y axis was just created it
			       // won't have the CP yet
    }
  }



  public void displaySnapTrackingInfo(List variables, List values)
  {
    // construct Variable->Color map to color code each variable name
    // using color of first Graph in GCM that uses that variable as a
    // Y variable.
    Map varColors = new HashMap();
    for (Iterator i = graphCanvasManager.getGraphsForDataSet(variables)
	   .iterator();
	 i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      Variable v = g.getYVariable();
      if (! varColors.containsKey(v))
	varColors.put(v, g.getColor());
    }

    StringBuffer trackingHTML = new StringBuffer();

    trackingHTML.append
      ("<table><tr><th>Variable</th><th>Value</th></tr>");

    for (int i = 0, n = variables.size(); i < n; i++)
    {
      Variable v = (Variable) variables.get(i);
      BigDecimal val = (BigDecimal) values.get(i);

      String valString = "---";
      if (val != null)
	valString =
	  (new EngValue(val, new EngUnits(v.getUnits()), NUM_SIGFIGS))
	  .toString();

      writeTrackingRow
	(trackingHTML, v.getName(), valString, (Color) varColors.get(v));
    }

    trackingHTML.append("</table>");

    if (! variables.isEmpty())
    {
      Experiment exp = findExperimentForResult
	(ExperimentResult.getExperimentResultForVariable
	 ((Variable) variables.get(0)));

      if (exp != null)
      {
	trackingHTML.append("<br>&nbsp;[");
	trackingHTML.append(exp.toString());
	trackingHTML.append("]");
      }
    }

    trackingPane.setText(trackingHTML.toString());
  }



  public void displayContinuousTrackingInfo(List axes, List values)
  {
    // Continuous tracking: display a value for each Axis that
    // corresponds to the current mouse position.

    StringBuffer trackingHTML = new StringBuffer();
    trackingHTML.append
      ("<table><tr><th>Axis</th><th>Value</th></tr>");

    for (int i = 0, n = axes.size(); i < n; i++)
    {
      Axis a = (Axis) axes.get(i);
      BigDecimal val = (BigDecimal) values.get(i);

      String valString = "---";
      if (val != null)
      {
	EngUnits units;
	try {
	  units = new EngUnits(a.getUnits());
	}
	catch(InconsistentUnitsException ex)
	{
	  units = new EngUnits("***", 0, 0);
	}

	valString = new EngValue(val, units, NUM_SIGFIGS).toString();
      }

      // Color coding for Axis name comes from the first Graph using
      // the Axis as a Y axis
      Color color = null;
      Iterator gi = graphCanvasManager.getGraphsForYAxis(a).iterator();
      if (gi.hasNext())
	color = ((Graph) gi.next()).getColor();

      writeTrackingRow(trackingHTML, a.getName(), valString, color);
    }

    trackingHTML.append("</table>");
    trackingPane.setText(trackingHTML.toString());
  }



  public void clearTrackingInfo()
  {
    trackingPane.setText("");
  }



  /**
   * Handle WindowEvents from user force-closing tracking dialog.
   */
  public void windowClosing(WindowEvent evt)
  {
    Object o = evt.getSource();

    if (o == trackingDialog)
      onTrackingClose();
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



  private void updateTemperature()//XXX updateForMostRecentExperiment
  {
    expLabel.setText(mostRecentExperiment.toString());

    Temperature t =
      mostRecentExperiment.getExperimentResult().getTemperature();

    temperature.setText
      ("Temperature: " +
       (t == null ? "unknown" :
	// HACK: trim() below is a workaround for Lab Server reporting
	// units as "K\n" rather than "K"
	(new EngValue(t.getValue(), new EngUnits(t.getUnits().trim()),
		      NUM_SIGFIGS))
	.toString()));
  }



  // XXX
  private void updateAxisPanels()
  {
    //XXX
    ExperimentResult er = mostRecentExperiment.getExperimentResult();

    // keep track of existing AxisPanels for reuse.  Maps Axis to
    // AxisPanel.
    Map axisPanelMap = new HashMap();

    // remove existing yAxisPanels
    for (int i = yAxisPanels.size() - 1; i >= 0; i--)
    {
      AxisPanel ap = (AxisPanel) yAxisPanels.get(i);
      axisPanelMap.put(ap.getAxis(), ap);
      this.remove(ap);
      this.remove(ap.axisLabel);
      yAxisPanels.remove(i);
    }

    // remove existing JSeparators
    for (int i = apSeparators.size() - 1; i >= 0; i--)
    {
      this.remove((JSeparator) apSeparators.get(i));
      apSeparators.remove(i);
    }

    //
    // create new yAxisPanels and add them to layout (along with
    // separators if necessary)
    //

    List yAxes = graphCanvasManager.getYAxes();

    GridBagConstraints gbc = new GridBagConstraints();
    gbc.fill = GridBagConstraints.BOTH;
    gbc.gridy = 1;
    gbc.weightx = 0;
    gbc.weighty = 1;

    GridBagConstraints gbcLabel = new GridBagConstraints();
    gbcLabel.gridy = 0;

    for (int i = 0, n = yAxes.size(); i < n; i++)
    {
      Axis a = (Axis) yAxes.get(i);
      AxisPanel newAP = (AxisPanel) axisPanelMap.get(a);
      if (newAP == null)
	newAP = new AxisPanel(this, graphCanvasManager, er, a, false);
      yAxisPanels.add(newAP);

      switch (i)
      {
      case 0:
	gbc.gridx = 0;
	break;
      case 1:
	gbc.gridx = 2;
	break;
      default:
	gbc.gridx++;
	JSeparator js = new JSeparator(SwingConstants.VERTICAL);
	apSeparators.add(js);
	this.add(js, gbc);
	gbc.gridx++;
      }

      this.add(newAP, gbc);

      gbcLabel.gridx = gbc.gridx;
      this.add(newAP.axisLabel, gbcLabel);
    }

    // remove existing xAxisPanel
    if (xAxisPanel != null)
      this.remove(xAxisPanel);

    //
    // create new xAxisPanel and add it to layout
    //

    List xAxes = graphCanvasManager.getXAxes();
    if (xAxes.size() != 1)
      throw new Error("invalid X axis count: " + xAxes.size());
    Axis xAxis = (Axis) xAxes.get(0);
    if (xAxisPanel == null || xAxisPanel.getAxis() != xAxis)
      xAxisPanel = new AxisPanel(this, graphCanvasManager, er, xAxis, true);

    gbc.gridx = 1;
    gbc.gridy = 2;
    gbc.weightx = 1;
    gbc.weighty = 0;
    this.add(xAxisPanel, gbc);

    // revalidate
    this.revalidate();
  }



  private final void updateConnectPatterns()
  {
    ConnectPattern cp = mostRecentExperiment.getConnectPattern();

    for (Iterator i = graphCanvasManager.getGraphs().iterator();
	 i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      if (isCurrentExperimentGraph(g))
      {
	g.setConnectPattern(cp);
	g.notifyObservers();
      }
    }
  }


  //XXX
  protected Experiment findExperimentForResult(ExperimentResult er)
  {
    for (Iterator i = theMainFrame.getRecentExperiments().iterator();
	 i.hasNext(); )
    {
      Experiment e = (Experiment) i.next();
      if (e.getExperimentResult() == er)
	return e;
    }

    return null;
  }



  public /*XXX private? protected? */ Color nextColor()
  {
    if (defaultColorIterator.hasNext())
      return (Color) defaultColorIterator.next();

    // oops, we're out of default colors.  Make up a random color.
    return new Color(random.nextInt(16777216));
  }



  // helper method for tracking
  private void writeTrackingRow(StringBuffer trackingHTML, String name,
				String valString, Color color)
  {
    trackingHTML.append("<tr><td align=\"center\"");
    if (color != null)
    {
      trackingHTML.append(" style=\"color:");
      trackingHTML.append(toHTMLColor(color));
      trackingHTML.append("\"");
    }
    trackingHTML.append(">");
    trackingHTML.append(name);
    trackingHTML.append("</td><td>");
    trackingHTML.append(valString);
    trackingHTML.append("</td></tr>");
  }



  private String toHTMLColor(Color c)
  {
    String s = Integer.toHexString(c.getRGB() & 0xffffff);
    if (s.length() < 6)
    {
      // pad on left with zeros
      s = "000000".substring(s.length()) + s;
    }
    return s;
  }

} // end class ResultsPanel
