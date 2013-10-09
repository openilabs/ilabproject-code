/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.graphing;

import weblab.toolkit.util.ChangeTrackingObservable;

import java.awt.Component;
import java.awt.Graphics;
import java.awt.Point;

import javax.swing.event.MouseInputListener;
import java.awt.event.MouseEvent;

import java.util.Observer;
import java.util.Observable;

import java.util.Collections;
import java.util.Collection;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

import java.math.BigDecimal;

// TODO: incorporate parts of tracking code from old ResultsPanel.java?

/**
 * TODO
 */
public class GraphCanvasManager extends ChangeTrackingObservable
  implements Observer, MouseInputListener
{
  /**
   * Flag value to inform Observers that the set of X Axes associated
   * with this has changed.
   */
  public static final String X_AXES_CHANGE = "xAxes";

  /**
   * Flag value to inform Observers that the set of Y Axes associated
   * with this has changed.
   */
  public static final String Y_AXES_CHANGE = "yAxes";

  /**
   * Flag value to inform Observers that the set of Graphs associated
   * with this has changed.
   */
  public static final String GRAPHS_CHANGE = "graphs";

  //XXX
  public static final int NUM_SIGFIGS = 4;

  private Grid theGrid;

  private List xAxes, yAxes, graphs, trackingListeners;

  protected Component canvas;


  private boolean trackingEnabled, snapTracking;

  //XXX
  private int snapTrackingThreshold = 20; // pixels


  /**
   * Constructs a new GraphCanvasManager for the specified canvas,
   * with a default Grid and no axes or graphs.
   */
  public GraphCanvasManager(Component canvas)
  {
    this.canvas = canvas;

    theGrid = new Grid();
    theGrid.addWeakReferenceObserver(this);

    xAxes = new ArrayList();
    yAxes = new ArrayList();
    graphs = new ArrayList();

    canvas.addMouseMotionListener(this);
    canvas.addMouseListener(this);

    trackingListeners = new ArrayList();
  }



  /**
   * Returns the Grid of this.
   */
  public Grid getGrid()
  {
    return theGrid;
  }



  /**
   * Returns an unmodifiable List of the Graphs of this.
   */
  public List getGraphs()
  {
    return Collections.unmodifiableList(graphs);
  }



  /**
   * Adds the specified Graph to this GraphCanvasManager (at the end
   * of the list of Graphs).  If the specified Graph is already on
   * this GraphCanvasManager, does nothing.
   */
  public void addGraph(Graph g)
  {
    if (! graphs.contains(g))
    {
      graphs.add(g);
      g.addWeakReferenceObserver(this);

      this.setChanged(GRAPHS_CHANGE);

      canvas.repaint();
    }
  }



  /**
   * Adds the specified Graph to this GraphCanvasManager (at the
   * specified index position in the list of Graphs).  If the
   * specified Graph is already on this GraphCanvasManager, does
   * nothing.
   */
  public void addGraph(int index, Graph g)
  {
    if (! graphs.contains(g))
    {
      graphs.add(index, g);
      g.addWeakReferenceObserver(this);

      this.setChanged(GRAPHS_CHANGE);

      canvas.repaint();
    }
  }



  /**
   * Removes the specified Graph from this GraphCanvasManager.  Also
   * sets the Graph's Axes to null for cleanup purposes (see Graph
   * class specification).
   */
  public void removeGraph(Graph g)
  {
    if (graphs.remove(g))
    {
      g.deleteObserver(this);

      // set Graph's axes to null so old axes won't continue to keep
      // track of extraneous Variables
      g.setXAxis(null);
      g.setYAxis(null);

      this.setChanged(GRAPHS_CHANGE);

      // notify observers that g has changed
      g.notifyObservers();

      canvas.repaint();
    }
  }



  /**
   * Returns an unmodifiable List of the X Axes of this.
   */
  public List getXAxes()
  {
    return Collections.unmodifiableList(xAxes);
  }



  /**
   * Adds the specified Axis to the list of X Axes associated with
   * this.  If the specified Axis is already an X Axis of this, does
   * nothing.
   */
  public void addXAxis(Axis a)
  {
    if (! xAxes.contains(a))
    {
      xAxes.add(a);
      a.addWeakReferenceObserver(this);

      this.setChanged(X_AXES_CHANGE);
    }
  }



  /**
   * Removes the specified Axis from the list of X Axes associated
   * with this.
   */
  public void removeXAxis(Axis a)
  {
    if (xAxes.remove(a))
    {
      // stop observing a, unless it is also a Y axis of this
      if (! yAxes.contains(a))
	a.deleteObserver(this);

      this.setChanged(X_AXES_CHANGE);
    }
  }



  /**
   * Returns an unmodifiable List of the Y Axes of this.
   */
  public List getYAxes()
  {
    return Collections.unmodifiableList(yAxes);
  }



  /**
   * Adds the specified Axis to the list of Y Axes associated with
   * this.  If the specified Axis is already a Y Axis of this, does
   * nothing.
   */
  public void addYAxis(Axis a)
  {
    if (! yAxes.contains(a))
    {
      yAxes.add(a);
      a.addWeakReferenceObserver(this);

      this.setChanged(Y_AXES_CHANGE);
    }
  }



  /**
   * Removes the specified Axis from the list of Y Axes associated
   * with this.
   */
  public void removeYAxis(Axis a)
  {
    if (yAxes.remove(a))
    {
      // stop observing a, unless it is also an X axis of this
      if (! xAxes.contains(a))
	a.deleteObserver(this);

      this.setChanged(Y_AXES_CHANGE);
    }
  }



  /**
   * Paints the grid and graphs onto the canvas.  This method should
   * be called by the paintComponent method of the canvas.
   */
  public void paintCanvas(Graphics g)
  {
    // paint grid
    theGrid.paint(canvas, g);

    // plot graphs (in reverse order so that earliest ones are in front)
    for (int i = graphs.size() - 1; i >= 0; i--)
    {
      ((Graph) graphs.get(i)).plot(canvas, g);
    }
  }



  // XXX SPECS
  // highlight given point of specified Graphs.  Clear highlighting on
  // all Graphs in this that are not in graphsToHighlight.
  public void highlightPoint(Collection graphsToHighlight, int n)
  {
    for (Iterator i = graphsToHighlight.iterator(); i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      g.setHighlightedPoint(n);
      g.notifyObservers();
    }

    ArrayList graphsToClear = new ArrayList(graphs);
    graphsToClear.removeAll(graphsToHighlight);

    for (Iterator i = graphsToClear.iterator(); i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      g.setHighlightedPoint(-1);
      g.notifyObservers();
    }
  }


  // XXX SPECS (and maybe method name?)
  // dataSet = Collection of Variable
  public List getGraphsForDataSet(Collection dataSet)
  {
    List result = new ArrayList();
    for (Iterator i = graphs.iterator(); i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      if (dataSet.contains(g.getXVariable()) &&
	  dataSet.contains(g.getYVariable()))
	result.add(g);
    }

    return result;
  }


  /**
   * XXX
   *
   * Note: this method is closely related to Axis.getGraphs(), but
   * preserves the ordering of the returned Graphs by returning them
   * in a List rather than a Set.
   */
  public List getGraphsForXAxis(Axis a)
  {
    List result = new ArrayList();
    for (Iterator i = graphs.iterator(); i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      if (g.getXAxis() == a)
	result.add(g);
    }
    return Collections.unmodifiableList(result);
  }

  public List getGraphsForYAxis(Axis a)
  {
    List result = new ArrayList();
    for (Iterator i = graphs.iterator(); i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      if (g.getYAxis() == a)
	result.add(g);
    }
    return Collections.unmodifiableList(result);
  }


  //XXX all these method names and signatures.  Especially
  //isSnapTracking.  Ngggh.  Sort out later.
  public boolean isTrackingEnabled()
  {
    return trackingEnabled;
  }

  public void setTrackingEnabled(boolean value)
  {
    if (trackingEnabled && ! value)
      clearTrackingInfo();
    trackingEnabled = value;
  }

  public boolean isSnapTracking()
  {
    return snapTracking;
  }

  public void setSnapTracking(boolean value)
  {
    snapTracking = value;
  }


  public void addTrackingListener(TrackingListener l)
  {
    trackingListeners.add(l);
  }


  //////////////////////
  // Listener methods //
  //////////////////////


  /**
   * Handle updates from Observables
   */
  public final void update(Observable o, Object arg)
  {
    // When the grid changes, repaint the canvas.
    if (o == theGrid)
      canvas.repaint();

    // When an axis changes, repaint the canvas.
    if (xAxes.contains(o) || yAxes.contains(o))
      canvas.repaint();

    // When a graph changes, repaint canvas as necessary
    if (graphs.contains(o))
    {
      Graph changedGraph = (Graph) o;

      for (Iterator i = ChangeTrackingObservable.extractChanges(arg)
	     .iterator();
	   i.hasNext(); )
      {
	String changeFlag = (String) i.next();

	//
	// For highlighting changes, we can get away with repainting
	// only one or two small regions of the graph canvas
	//
	if (changeFlag.startsWith(Graph.HIGHLIGHT_CHANGE_PREFIX))
	{
	  Point p;

	  // the rest of the change string indicates the number of the
	  // point that is no longer highlighted (or -1 for none)
	  int oldN = Integer.parseInt
	    (changeFlag.substring(Graph.HIGHLIGHT_CHANGE_PREFIX.length()));

	  if (oldN != -1)
	  {
	    // repaint vicinity of old point location
	    try {
	      p = changedGraph.getPoint(canvas, oldN);
	      canvas.repaint(p.x - Graph.HIGHLIGHT_RADIUS,
			     p.y - Graph.HIGHLIGHT_RADIUS,
			     2 * Graph.HIGHLIGHT_RADIUS + 1,
			     2 * Graph.HIGHLIGHT_RADIUS + 1);
	    }
	    catch (IndexOutOfBoundsException ex) {
	      // ignore (there's no such point on this graph to
	      // worry about)
	    }
	  }

	  // the new highlighted point (or -1 for none)
	  int newN = changedGraph.getHighlightedPoint();

	  if (newN != -1)
	  {
	    // repaint vicinity of new point location
	    try {
	      p = changedGraph.getPoint(canvas, newN);
	      canvas.repaint(p.x - Graph.HIGHLIGHT_RADIUS,
			     p.y - Graph.HIGHLIGHT_RADIUS,
			     2 * Graph.HIGHLIGHT_RADIUS + 1,
			     2 * Graph.HIGHLIGHT_RADIUS + 1);
	    }
	    catch (IndexOutOfBoundsException ex) {
	      // ignore (there's no such point on this graph to
	      // worry about)
	    }
	  }
	}
	else
	{
	  //
	  // For changes other than highlight changes, we must repaint
	  // the entire canvas.
	  //
	  canvas.repaint();
	}
      }
    }
  }



  // When mouse moves within the canvas, update point highlighting for
  // all graphs and update tracking info
  public void mouseMoved(MouseEvent evt)
  {
    if (! trackingEnabled)
      return;

    // mouse position
    Point mouseP = evt.getPoint();

    if (snapTracking)
    {
      // Snap tracking: identify the data point closest to the current
      // mouse position, and display the value of each Variable in the
      // data set that corresponds to this data point.

      // find nearest point to current mouse position and keep track
      // of which graph it is, which point it is on that graph, and
      // the pixel distance from mouseP

      Graph nearestGraph = null;
      int nearestPoint = -1;
      double distanceSq = 0;

      for (int i = 0, n = graphs.size(); i < n; i++)
      {
	Graph g = (Graph) graphs.get(i);
	if (! g.isActive())
	  continue;

	int pt = g.findNearestPoint(canvas, mouseP);
	if (pt != -1)
	{
	  double nextDSq = mouseP.distanceSq(g.getPoint(canvas, pt));
	  if (nearestGraph == null || nextDSq < distanceSq)
	  {
	    nearestGraph = g;
	    nearestPoint = pt;
	    distanceSq = nextDSq;
	  }
	}
      }

      // if we found a point, and it was within snapTrackingThreshold
      // pixels of mouseP, proceed with snap tracking
      if (nearestGraph != null &&
	  distanceSq <= snapTrackingThreshold * snapTrackingThreshold)
      {
	// XXX check for null (shouldn't need to, though) and/or check
	// for matching X Var and Y Var dataSets?
	List dataSet = nearestGraph.getXVariable().getDataSet();

	Collection graphsForDataSet = getGraphsForDataSet(dataSet);

	// update highlighting
	highlightPoint(graphsForDataSet, nearestPoint);

	List values = new ArrayList(dataSet.size());
	for (int i = 0, n = dataSet.size(); i < n; i++)
	{
	  Variable v = (Variable) dataSet.get(i);
	  values.add((BigDecimal) v.getData().get(nearestPoint));
	}

	for (int i = 0, n = trackingListeners.size(); i < n; i++)
	  ((TrackingListener) trackingListeners.get(i))
	    .displaySnapTrackingInfo(dataSet, values);
      }
      else
      {
	// snap tracking failed!  clear highlighting and tracking info
	highlightPoint(Collections.EMPTY_SET, -1);
	clearTrackingInfo();
      }
    }
    else
    {
      // Continuous tracking: display a value for each Axis that
      // corresponds to the current mouse position.

      // clear highlighting
      highlightPoint(Collections.EMPTY_SET, -1);

      int numXAxes = xAxes.size();
      int numAxes = numXAxes + yAxes.size();

      // populate Axes list from GCM.  Keep track of number of X Axes
      // as well as total number of Axes.
      List axes = new ArrayList(numAxes);
      axes.addAll(xAxes);
      axes.addAll(yAxes);

      // initialize Values list
      List values = new ArrayList(numAxes);

      int xMax = canvas.getWidth() - 1;
      int yMax = canvas.getHeight() - 1;

      for (int i = 0; i < numAxes; i++)
      {
	Axis a = (Axis) axes.get(i);

	if (a.isActive())
	{
	  if (i < numXAxes)
	    values.add(a.unscaleToDataValue(mouseP.x, 0, xMax, NUM_SIGFIGS));
	  else
	    values.add(a.unscaleToDataValue(mouseP.y, yMax, 0, NUM_SIGFIGS));
	}
	else
	  values.add(null);
      }

      for (int i = 0, n = trackingListeners.size(); i < n; i++)
	((TrackingListener) trackingListeners.get(i))
	  .displayContinuousTrackingInfo(axes, values);
    }
  }



  // when mouse pointer leaves the canvas, clear highlighting for all
  // graphs and clear tracking info
  public void mouseExited(MouseEvent evt)
  {
    if (! trackingEnabled)
      return;

    // clear highlighting
    highlightPoint(Collections.EMPTY_SET, -1);

    // clear tracking info
    clearTrackingInfo();
  }



  // obligatory irrelevant methods of MouseListener
  public void mouseClicked(MouseEvent evt) {}
  public void mouseEntered(MouseEvent evt) {}
  public void mousePressed(MouseEvent evt) {}
  public void mouseReleased(MouseEvent evt) {}

  // obligatory irrelevant methods of MouseMotionListener
  public void mouseDragged(MouseEvent evt) {}


  ////////////////////
  // Helper Methods //
  ////////////////////


  // calls clearTrackingInfo on all trackingListeners
  private void clearTrackingInfo()
  {
    for (int i = 0, n = trackingListeners.size(); i < n; i++)
      ((TrackingListener) trackingListeners.get(i))
	.clearTrackingInfo();
  }

} // end class GraphCanvasManager
