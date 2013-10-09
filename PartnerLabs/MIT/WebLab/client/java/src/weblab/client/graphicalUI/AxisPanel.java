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

import java.util.List;
import java.util.Set;
import java.util.Iterator;
import java.util.ArrayList;

import java.util.Observable;
import java.util.Observer;

import weblab.toolkit.graphing.*;

import weblab.toolkit.util.ChangeTrackingObservable;
import weblab.toolkit.util.EngUnits;
import weblab.toolkit.util.EngValue;
import weblab.toolkit.util.EngValueField;

import weblab.client.Experiment;
import weblab.client.ExperimentResult;



public class AxisPanel extends JPanel implements Observer
{
  // number of sigfigs to display for tracking and ndivs
  private static final int NUM_SIGFIGS = 4;

  // maximum number of auxillary graph variables to display on panel
  private static final int NUM_AUX_GRAPHS = 4;

  // ResultsPanel that this belongs to
  private ResultsPanel theResultsPanel;

  private GraphCanvasManager gcm;

  private Axis myAxis;

  // Collection of Graphs whose variable selection should be linked to
  // the Variable widget on this
  private List myGraphs;

  ExperimentResult theExpResult;

  JComboBox mainVar;
  JComponent mainVarColorLine;

  JLabel[] auxVars;
  JComponent[] auxVarColorLines;

  JLabel auxVarEllipsis;

  JComboBox scale;
  EngValueField min, max;
  JCheckBox autoscale;

  JLabel scaleLabel, axisLabel, divLabel, trackingLabel;
    
  JLabel divValue, trackingValue;

  // true for a horizontal axis, false for vertical
  private boolean isHorizontal;

  // true if this has inconsistent units and we've already displayed
  // an error message complaining about it
  private boolean inconsistentUnits;



  /////////////
  // Actions //
  /////////////

  private Action APPLY = new AbstractAction()
    {
      public void actionPerformed(ActionEvent evt)
      {
	Variable var;
	if (mainVar.getSelectedIndex() == 0) {
	  // no variable selected
	  var = null;
	}
	else {
	  var = (Variable) mainVar.getSelectedItem();
	}
	// set variable
	for (Iterator i = myGraphs.iterator(); i.hasNext(); )
	{
	  Graph g = (Graph) i.next();
	  if (theResultsPanel.isCurrentExperimentGraph(g))
	  {
	    if (isHorizontal)
	      g.setXVariable(var);
	    else
	      g.setYVariable(var);
	  }
	}


	myAxis.setContinuousAutoscale(autoscale.isSelected());

	myAxis.setMin(min.getValue().toBigDecimal());
	myAxis.setMax(max.getValue().toBigDecimal());

	// set scale
	if (scale.getSelectedItem().equals("Linear")) {
	  myAxis.setScale(Axis.LINEAR_SCALE);
	}
	else if (scale.getSelectedItem().equals("Logarithmic")) {
	  myAxis.setScale(Axis.LOG_SCALE);
	}
	else {
	  assert false : "illegal scale";
	}

	// notify observers that myGraphs may have changed (note:
	// don't embed this in the loop above, because changing the
	// variables one at a time may cause the axis to temporarily
	// go through a state of inconsistent units when in fact we're
	// changing from one consistent state to another consistent
	// state)
	for (Iterator i = myGraphs.iterator(); i.hasNext(); )
	  ((Graph) i.next()).notifyObservers();

	// notify observers that myAxis may have changed
	myAxis.notifyObservers();

	// make sure everything displayed is accurate
	revert();
      }
    };
  
  //////////////////
  // Constructors //
  //////////////////

  public AxisPanel(ResultsPanel theResultsPanel, GraphCanvasManager gcm,
		   ExperimentResult theExpResult,
		   Axis theAxis, boolean horizontal)
  {
    this.theExpResult = theExpResult;
    this.myAxis = theAxis;

    this.theResultsPanel = theResultsPanel;

    this.gcm = gcm;

    this.isHorizontal = horizontal;

    this.inconsistentUnits = false;

    this.myGraphs = new ArrayList();
    updateGraphs();

    //
    // setup components
    //

    axisLabel = new JLabel(myAxis.getName() + " Axis");
    axisLabel.setFont(axisLabel.getFont().deriveFont(Font.BOLD));

    scaleLabel = new JLabel("Scale: ");

    trackingLabel = new JLabel("Tracking: ");

    divLabel = new JLabel(" / division");

    divValue = new JLabel();

    trackingValue = new SmartJLabel();
    trackingValue.setHorizontalAlignment(SwingConstants.CENTER);
    // ensure enough width to (probably) avoid resizing
    trackingValue.setText("00.0000000 mm");
    trackingValue.setMinimumSize(trackingValue.getPreferredSize());

    // initialize to null units, since this requires more space to
    // display than typical Weblab units (which are usually only one
    // character)
    min = new EngValueField();
    min.setAction(APPLY);

    max = new EngValueField();
    max.setAction(APPLY);

    scale = new JComboBox();
    scale.setEditable(false);
    scale.addItem("Linear");
    scale.addItem("Logarithmic");
    scale.setAction(APPLY);

    mainVar = new JComboBox();
    mainVar.setEditable(false);
    mainVar.addItem("None ");
    mainVar.setAction(APPLY);

    mainVarColorLine = new ColorLine();

    auxVars = new JLabel[NUM_AUX_GRAPHS];
    auxVarColorLines = new JComponent[NUM_AUX_GRAPHS];
    for (int i = 0; i < NUM_AUX_GRAPHS; i++)
    {
      auxVars[i] = new JLabel();
      auxVarColorLines[i] = new ColorLine();
    }

    auxVarEllipsis = new JLabel("[more...]");


    // note: have to set the text *after* setAction because setAction
    // pulls text from the Action
    autoscale = new JCheckBox();
    autoscale.setAction(APPLY);
    autoscale.setText("autoscale");

    //
    // setup listeners
    //

    myAxis.addWeakReferenceObserver(this);
    gcm.addWeakReferenceObserver(this);
    gcm.getGrid().addWeakReferenceObserver(this);

    //
    // setup layout
    //

    if (this.isHorizontal)
      // do layout for a horizontal AxisPanel
      {
	min.setHorizontalAlignment(SwingConstants.LEFT);
	max.setHorizontalAlignment(SwingConstants.RIGHT);
	
	JPanel row1 = new JPanel(new GridBagLayout());
	JPanel row2 = new JPanel(new GridBagLayout());

	GridBagConstraints gbc = new GridBagConstraints();
	GridBagConstraints glue_gbc = new GridBagConstraints();
	glue_gbc.weightx = 1;

	JPanel currentRow = row1;
	
	currentRow.add(min, gbc);

	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(divValue, gbc);
	currentRow.add(divLabel, gbc);
	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(max, gbc);
	
	currentRow = row2;
	
	currentRow.add(axisLabel, gbc);
	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(new JLabel("Var: "), gbc);//XXX
	currentRow.add(mainVar, gbc);
	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(scaleLabel, gbc);
	currentRow.add(scale, gbc);
	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(autoscale, gbc);
	currentRow.add(Box.createGlue(), glue_gbc);
	
	this.setLayout(new GridLayout(2,1));
	this.add(row1);
	this.add(row2);
      }
    else
      // layout for a vertical AxisPanel
      {
      min.setHorizontalAlignment(SwingConstants.CENTER);
      max.setHorizontalAlignment(SwingConstants.CENTER);

      this.setLayout(new GridBagLayout());

      GridBagConstraints gbc = new GridBagConstraints();

      //
      // layout varsPanel
      //

      JPanel varsPanel = new JPanel(new GridBagLayout());
      gbc.gridy = 0;

      gbc.weightx = 1;
      gbc.gridx = 0;
      varsPanel.add(Box.createGlue(), gbc);
      gbc.gridx = 2;
      varsPanel.add(Box.createGlue(), gbc);
      gbc.gridx = 4;
      varsPanel.add(Box.createGlue(), gbc);

      gbc.weightx = 0;
      gbc.gridx = 1;
      varsPanel.add(mainVarColorLine, gbc);
      gbc.gridx = 3;
      varsPanel.add(mainVar, gbc);

      for (int i = 0; i < NUM_AUX_GRAPHS; i++)
      {
	gbc.gridy++;
	gbc.gridx = 1;
	varsPanel.add(auxVarColorLines[i], gbc);
	gbc.gridx = 3;
	varsPanel.add(auxVars[i], gbc);
      }

      gbc.gridy++;
      varsPanel.add(auxVarEllipsis, gbc);

      //
      // main layout
      //

      gbc = new GridBagConstraints();
      gbc.gridwidth = GridBagConstraints.REMAINDER;

      GridBagConstraints glue_gbc = new GridBagConstraints();
      glue_gbc.gridwidth = GridBagConstraints.REMAINDER;
      glue_gbc.weighty = 1;

      this.add(max, gbc);
      // instead of just adding glue here, add a strut to ensure that
      // the AxisPanel always remains at least as wide as the current
      // preferred size of the EngValueField (with null units).  This
      // gets rid of the "GUI rhumba" effect.  <dmrz>
      this.add(Box.createHorizontalStrut(max.getPreferredSize().width),
	       glue_gbc);

      this.add(new JLabel("Variables: "), gbc);//XXX
      gbc.fill = GridBagConstraints.HORIZONTAL;
      this.add(varsPanel, gbc);
      gbc.fill = GridBagConstraints.NONE;

      this.add(Box.createVerticalStrut(10), gbc);
      this.add(scaleLabel, gbc);
      this.add(scale, gbc);
      this.add(autoscale, gbc);
      this.add(Box.createGlue(), glue_gbc);
      this.add(divValue, gbc);
      this.add(divLabel, gbc);
      this.add(Box.createGlue(), glue_gbc);
      this.add(min, gbc);
    }

    //
    // initialize
    //

    updateVariables();
  }


  private void updateColor(Color color)
  {
    min.setForeground(color);
    max.setForeground(color);
    divValue.setForeground(color);
    trackingValue.setForeground(color);
    mainVarColorLine.setForeground(color);
  }



  public Axis getAxis() { return myAxis; } //XXX??

  // automatically finds graphs that use myAxis
  private void updateGraphs()
  {
    for (int i = 0, n = myGraphs.size(); i < n; i++)
      ((Graph) myGraphs.get(i)).deleteObserver(this);

    myGraphs.clear();


    List graphs = gcm.getGraphs();

    // identify graphs that use myAxis.
    for (int i = 0, n = graphs.size(); i < n; i++)
    {
      Graph g = (Graph) graphs.get(i);

      if ((isHorizontal ? g.getXAxis() : g.getYAxis()) == myAxis)
      {
	myGraphs.add(g);
	g.addWeakReferenceObserver(this);
      }
    }
  }



  public void setExperimentResult(ExperimentResult newExpResult)
  {
    this.theExpResult = newExpResult;
    updateVariables();
  }


  /*XXX
  // sets tracking to the nth data point, if possible
  public final String trackDataPoint(int n)
  {
    BigDecimal[] data = myAxis.getData();
    String trackingText;
    if (n < data.length)
    {
      trackingText =
	(new EngValue(data[n], new EngUnits(myAxis.getUnits()), NUM_SIGFIGS))
	.toString();
    }
    else
      trackingText = "---";

    return trackingText;
  }
  */

  /*
  // sets tracking based on mouse location
  public final String trackMouseLocation(Component canvas, Point p)
  {
    String trackingText;

    if (myAxis.isActive())
    {
      BigDecimal value;

      if (isHorizontal)
	value = myAxis.unscaleToDataValue
	  (p.x, 0, canvas.getSize().width - 1, 4);
      else
	value = myAxis.unscaleToDataValue
	  (p.y, canvas.getSize().height - 1, 0, 4);

      EngUnits units;
      try {
	units = new EngUnits(myAxis.getUnits());
      }
      catch(InconsistentUnitsException ex)
      {
	units = new EngUnits("***", 0, 0);
      }

      trackingText =
	(new EngValue(value, units, NUM_SIGFIGS)).toString();
    }
    else
      trackingText = "---";

    return trackingText;
  }
  */


  /**
   * Handle updates from Observables
   */
  public final void update(Observable o, Object arg)
  {
    // When Grid changes, re-import values
    if (o == gcm.getGrid())
      revert();

    // When myAxis changes, re-import values.  If the change was a
    // GRAPHS_CHANGE, also update graphs.
    if (o == myAxis)
    {
      // this is how we find out when a pre-existing graph has been
      // reassigned to use myAxis
      if (ChangeTrackingObservable.extractChanges(arg).contains
	  (Axis.GRAPHS_CHANGE))
	updateGraphs();

      revert();
    }

    // When a new Graph is added to GCM, update graphs.  This is how
    // we find out when a brand new graph is created that uses myAxis
    // (of course, we'll also get an Axis.GRAPHS_CHANGE when the graph
    // is first assigned to the axis, but at that point it may not be
    // in the GCM yet, which means updateGraphs won't find it).
    if (o == gcm && ChangeTrackingObservable.extractChanges(arg).contains
	(GraphCanvasManager.GRAPHS_CHANGE))
    {
      updateGraphs();
      revert();
    }

    // if one of myGraphs changes its Variable or Color or its other
    // Axis, re-import values.
    if (myGraphs.contains(o))
    {
      Set changes = ChangeTrackingObservable.extractChanges(arg);

      if (changes.contains(Graph.AXIS_CHANGE) ||
	  changes.contains(Graph.VARIABLE_CHANGE) ||
	  changes.contains(Graph.COLOR_CHANGE))
	revert();
    }
  }



  ////////////////////
  // Helper Methods //
  ////////////////////

  /**
   * Make sure the state of the components is consistent with respect
   * to which components are displayed, etc.  Note that this consults
   * myAxis.isActive() to determine whether or not to display a bunch
   * of things.
   */
  private void reconcile()
  {
    if (myAxis.isActive()) {
      // show all axis settings
      min.setHidden(false);
      max.setHidden(false);
      //	scaleLabel.setVisible(true);
      //	scale.setVisible(true);
      //	autoscale.setVisible(true);
      divValue.setVisible(true);
      divLabel.setVisible(true);
      trackingLabel.setVisible(true);
      trackingValue.setVisible(true);

      // min and max can be edited manually only if autoscale is not
      // selected
      if (autoscale.isSelected())
      {
	min.setEditable(false);
	max.setEditable(false);
      }
      else
      {
	min.setEditable(true);
	max.setEditable(true);
      }
    }
    else {
      // don't show other axis settings
      min.setHidden(true);
      max.setHidden(true);
      //	scaleLabel.setVisible(false);
      //	scale.setVisible(false);
      //	autoscale.setVisible(false);
      divValue.setVisible(false);
      divLabel.setVisible(false);
      trackingLabel.setVisible(false);
      trackingValue.setVisible(false);
    }

    // validate (since some components may have just been made visible
    // or invisible) and repaint
    this.validate();
    repaint();
  }



  /**
   * Set component state to accurately reflect the Axis, Graph, and
   * Grid.  Does NOT update variable choices.
   */
  private void revert()
  {
    // disable JComboBox Actions so that they won't trigger an APPLY
    // in the middle of revert()
    Action variableAction = mainVar.getAction();
    mainVar.setAction(null);
    Action scaleAction = scale.getAction();
    scale.setAction(null);

    Variable var = null;
    // get variable selection from FIRST current-experiment Graph (XXX
    // if they're not the same, we're in big trouble!).  If it's a Y
    // axis, also get Color from FIRST c-e Graph (maybe XXX).  If it's
    // an X axis, color is black.
    Iterator graphIter = (new ArrayList(myGraphs)).iterator();
    if (graphIter.hasNext())
    {
      Graph g = (Graph) graphIter.next();
      if (isHorizontal)
      {
	var = g.getXVariable();
	updateColor(Color.BLACK);
      }
      else
      {
	var = g.getYVariable();
	updateColor(g.getColor());
      }
    }

    //XXX set other c-e Graphs to match the variable from the first
    //one!
    while (graphIter.hasNext())
    {
      Graph g = (Graph) graphIter.next();
      if (theResultsPanel.isCurrentExperimentGraph(g))
      {
	if (isHorizontal)
	  g.setXVariable(var);
	else
	  g.setYVariable(var);
	g.notifyObservers();
      }
    }


    if (var == null)
    {
      // no variable selected
      mainVar.setSelectedIndex(0);
    }
    else
    {
      mainVar.setSelectedItem(var);
    }


    //XXX overall structure of this method getting a bit messy

    Iterator gi = gcm.getGraphsForYAxis(myAxis).iterator();

    if (gi.hasNext())
      gi.next(); // skip first graph

    for (int i = 0; i < NUM_AUX_GRAPHS; i++)
    {
      boolean visible = false;

      // find next active graph variable for this axis, if any
      Variable gv = null;
      while (gi.hasNext() && gv == null)
      {
	Graph gg = (Graph) gi.next();
	gv = gg.getActiveVariableForAxis(myAxis);

	if (gv != null)
	{
	  Experiment exp = theResultsPanel.findExperimentForResult
	    (ExperimentResult.getExperimentResultForVariable(gv));

	  auxVars[i].setText(exp == null ? gv.getName() :
			     gv.getName() + " [#" + exp.getExperimentID() + "]");

	  Color gc = gg.getColor();
	  auxVars[i].setForeground(gc);
	  auxVarColorLines[i].setForeground(gc);

	  visible = true;
	}
      }

      auxVars[i].setVisible(visible);
      auxVarColorLines[i].setVisible(visible);
    }

    // show ellipsis if there are still more Graphs left over (XXX
    // only if more ACTIVE graphs?)
    auxVarEllipsis.setVisible(gi.hasNext());

    autoscale.setSelected(myAxis.getContinuousAutoscale());

    min.setValue(myAxis.getMin());
    max.setValue(myAxis.getMax());

    // set units for min and max to match the units of the chosen
    // variable
    EngUnits units;
    try {
      units = new EngUnits(myAxis.getUnits());
      inconsistentUnits = false;
    }
    catch(InconsistentUnitsException ex)
    {
      units = new EngUnits("***", 0, 0);
      if (! inconsistentUnits)
      {
	inconsistentUnits = true;
	JOptionPane.showMessageDialog
	  (null/*XXX*/, "The variables you have selected for graphing on the "+myAxis.getName()+" Axis do not all have the same units.\nUnits for this axis will be displayed as *** until the situation is remedied.", "Warning - Inconsistent Units", JOptionPane.WARNING_MESSAGE);
      }
    }

    min.setUnits(units);
    max.setUnits(units);

    int nDivs;
    if (this.isHorizontal)
      nDivs = gcm.getGrid().getXDivs();
    else
      nDivs = gcm.getGrid().getYDivs();
    BigDecimal divSize = myAxis.getDivSize(nDivs, NUM_SIGFIGS);

    // import scale and divValue (which has units based on scale)
    switch(myAxis.getScale())
    {
    case Axis.LINEAR_SCALE:
      scale.setSelectedItem("Linear");
      divValue.setText(new EngValue(divSize, units, NUM_SIGFIGS)
		       .toString());
      break;
    case Axis.LOG_SCALE:
      scale.setSelectedItem("Logarithmic");
      // no rounding necessary here.  Even if numerical values get
      // very large, I'm not worried about the number of logarithmic
      // decades getting overly large.
      divValue.setText(divSize.toString() + " decades");
      break;
    default:
      throw new Error("illegal scale: " + myAxis.getScale());
    }

    // reenable JComboBox Actions
    mainVar.setAction(variableAction);
    scale.setAction(scaleAction);

    reconcile();
  }



  private void updateVariables()
  {
    // disable JComboBox Action so that it won't trigger an APPLY in
    // the middle of this process
    mainVar.setAction(null);

    // remove all items except the first one (indicating no variable)
    while (mainVar.getItemCount() > 1)
      mainVar.removeItemAt(1);

    for (Iterator i = theExpResult.getVariables().iterator(); i.hasNext(); )
    {
      mainVar.addItem((Variable) i.next());
    }

    mainVar.validate();

    // reenable JComboBox Action
    mainVar.setAction(APPLY);

    // re-import values to get the currently selected variable right
    revert();
  }



  private class SmartJLabel extends JLabel
  {
    /**
     * Returns the smallest dimension which is at least as big as
     * super.getPreferredSize() and also at least as big as
     * this.getMinimumSize().
     */
    public Dimension getPreferredSize()
    {
      Dimension p = super.getPreferredSize();
      Dimension m = this.getMinimumSize();
      if (m.getWidth() > p.getWidth()) {
	p.setSize(m.getWidth(), p.getHeight());
      }
      if (m.getHeight() > p.getHeight()) {
	p.setSize(p.getWidth(), m.getHeight());
      }
      return p;
    }
  } // end inner class SmartJLabel

  private static class ColorLine extends JPanel
  {
    private static final Dimension SIZE = new Dimension(30, 5);

    public ColorLine()
    {
      this.setMinimumSize(SIZE);
      this.setPreferredSize(SIZE);
    }

    public void paintComponent(Graphics g)
    {
      // paint background
      super.paintComponent(g);

      // paint colored line and accent point

      g.setColor(this.getForeground());

      Dimension size = this.getSize();
      Point center = new Point((size.width - 1) / 2, (size.height - 1) / 2);

      g.drawLine(0, center.y, size.width - 1, center.y);
      g.drawOval(center.x - 2, center.y - 2, 4, 4);
    }
  } // end inner class ColorLine

} // end class AxisPanel
