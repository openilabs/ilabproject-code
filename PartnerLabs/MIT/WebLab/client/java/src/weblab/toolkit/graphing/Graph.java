/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.graphing;

import weblab.toolkit.util.ChangeTrackingObservable;

import java.awt.Graphics;
import java.awt.Component;
import java.awt.Color;
import java.awt.Point;

import java.math.BigDecimal;

import java.util.List;

/**
 * Represents a relationship between two Variables (an independent
 * xVariable and a dependent yVariable) that can be plotted on a
 * canvas using a pair of Axes (independent xAxis and dependent
 * yAxis).
 *
 * Note: Graph automatically updates the Axis->Graph relationships for
 * its X and Y Axes whenever a new Axis is set.  When you're done
 * using a Graph object, you should ensure that both of its Axes are
 * set to null so that these relationships will be properly cleared
 * (otherwise, units and autoscaling behavior of the remaining Axes
 * may continue to be affected by the Variables they are associated
 * with in this Graph).
 */
public class Graph extends ChangeTrackingObservable implements Cloneable
{
  private Axis xAxis;
  private Axis yAxis;

  private Variable xVar;
  private Variable yVar;

  private Color color;
  private boolean accentPoints;

  private ConnectPattern connectPattern;

  private int highlightedPoint;

  /**
   * Flag value to inform Observers that either the x or y Axis
   * associated with this has changed.
   */
  public static final String AXIS_CHANGE = "axis";

  /**
   * Flag value to inform Observers that either the x or y Variable
   * associated with this has changed.
   */
  public static final String VARIABLE_CHANGE = "variable";

  /**
   * Flag value to inform Observers that the color of this has
   * changed.
   */
  public static final String COLOR_CHANGE = "color";

  /**
   * Flag value to inform Observers that the accent state of this has
   * changed.
   */
  public static final String ACCENT_CHANGE = "accent";

  /**
   * Flag value to inform Observers that the ConnectPattern associated
   * with this has changed.
   */
  public static final String CONNECT_PATTERN_CHANGE = "connectPattern";

  /**
   * Flag value to inform Observers that the highlighted point of this
   * (or lack thereof) has changed.  The actual change string will be
   * this prefix plus the number of the previously highlighted point
   * (or -1 for none), e.g. "highlightChangedFrom35" or
   * "highlightChangedFrom-1".
   */
  public static final String HIGHLIGHT_CHANGE_PREFIX = "highlightChangedFrom";

  /**
   * The radius of a highlighted point in pixels.
   */
  public static final int HIGHLIGHT_RADIUS = 5;

  /**
   * Always connect successive points.
   */
  public static final ConnectPattern ALWAYS_CONNECT =
    new ConnectPattern() {
      public boolean isConnected(int n)
      {
	return true;
      }
    };

  /**
   * Never connect successive points.
   */
  public static final ConnectPattern NEVER_CONNECT =
    new ConnectPattern() {
      public boolean isConnected(int n)
      {
	return false;
      }
    };



  /**
   * Constructs a new Graph with default values for color (black),
   * accent points (true), highlighted point (none), and connect
   * pattern (always).  The axes and variables of the new Graph are
   * initially null.
   */
  public Graph()
  {
    // set default values
    this.color = Color.black;
    this.accentPoints = true;
    this.highlightedPoint = -1;
    this.connectPattern = ALWAYS_CONNECT;

    xAxis = yAxis = null;
    xVar = yVar = null;
  }



  /**
   * Returns the X Axis of this (may be null).
   */
  public Axis getXAxis()
  {
    return this.xAxis;
  }



  /**
   * Returns the Y Axis of this (may be null).
   */
  public Axis getYAxis()
  {
    return this.yAxis;
  }



  /**
   * Sets the X Axis of this to newXAxis (may be null).
   *
   * Automatically registers this Graph with the new Axis (and
   * unregisters it from the previous X Axis).
   */
  public void setXAxis(Axis newXAxis)
  {
    if (this.xAxis != newXAxis)
    {
      Axis oldXAxis = this.xAxis;

      // change axis
      this.xAxis = newXAxis;
      this.setChanged(AXIS_CHANGE);

      // unregister from old axis
      if (oldXAxis != null)
      {
	oldXAxis.removeGraph(this);
	oldXAxis.notifyObservers();
      }

      // register with new axis
      if (newXAxis != null)
      {
	newXAxis.addGraph(this);
	newXAxis.notifyObservers();
      }
    }
  }



  /**
   * Sets the Y Axis of this to newYAxis (may be null).
   *
   * Automatically registers this Graph with the new Axis (and
   * unregisters it from the previous Y Axis).
   */
  public void setYAxis(Axis newYAxis)
  {
    if (this.yAxis != newYAxis)
    {
      Axis oldYAxis = this.yAxis;

      // change axis
      this.yAxis = newYAxis;
      this.setChanged(AXIS_CHANGE);

      // unregister from old axis
      if (oldYAxis != null)
      {
	oldYAxis.removeGraph(this);
	oldYAxis.notifyObservers();
      }

      // register with new axis
      if (newYAxis != null)
      {
	newYAxis.addGraph(this);
	newYAxis.notifyObservers();
      }
    }
  }



  /**
   * Returns the X Variable of this (may be null).
   */
  public Variable getXVariable()
  {
    return this.xVar;
  }



  /**
   * Returns the Y Variable of this (may be null).
   */
  public Variable getYVariable()
  {
    return this.yVar;
  }



  /**
   * Sets the X Variable of this to newX (may be null).
   */
  public void setXVariable(Variable newX)
  {
    if (xVar != newX)
    {
      xVar = newX;
      this.setChanged(VARIABLE_CHANGE);
    }
  }



  /**
   * Sets the Y Variable of this to newY (may be null).
   */
  public void setYVariable(Variable newY)
  {
    if (yVar != newY)
    {
      yVar = newY;
      this.setChanged(VARIABLE_CHANGE);
    }
  }



  /**
   * Returns the Variable actively being graphed on the specified Axis
   * by this Graph, if any.  Returns null if this Graph is not active
   * (i.e. if any of its Axes or Variables is null) or if the
   * specified Axis is not an Axis of this.
   */
  public Variable getActiveVariableForAxis(Axis a)
  {
    if (this.isActive())
    {
      if (a == xAxis)
	return xVar;

      if (a == yAxis)
	return yVar;
    }

    return null;
  }



  /**
   * Returns true if this Graph is actively being plotted (i.e. both
   * Axes and both Variables are non-null).
   */
  public boolean isActive()
  {
    return (getXAxis() != null && getYAxis() != null &&
	    getXVariable() != null && getYVariable() != null);
  }



  /**
   * Returns the current Color.
   */
  public Color getColor()
  {
    return this.color;
  }



  /**
   * Sets the Color that should be used to plot this graph.
   */
  public void setColor(Color c)
  {
    if (! c.equals(this.color))
    {
      this.color = c;
      this.setChanged(COLOR_CHANGE);
    }
  }



  /**
   * Returns true iff accentPoints is enabled.
   */
  public boolean getAccentPoints()
  {
    return this.accentPoints;
  }



  /**
   * Sets whether to visually accent each data point of the graph when
   * plotting.
   */
  public void setAccentPoints(boolean accentPoints)
  {
    if (accentPoints != this.accentPoints)
    {
      this.accentPoints = accentPoints;
      this.setChanged(ACCENT_CHANGE);
    }
  }



  /**
   * Sets the ConnectPattern for this graph.
   */
  public void setConnectPattern(ConnectPattern cp)
  {
    if (! cp.equals(this.connectPattern))
    {
      this.connectPattern = cp;
      this.setChanged(CONNECT_PATTERN_CHANGE);
    }
  }



  /**
   * Returns the currently highlighted point, or -1 if no point is
   * currently highlighted.
   */
  public int getHighlightedPoint()
  {
    return this.highlightedPoint;
  }



  /**
   * Sets the point that should be specially highlighted when this is
   * plotted.  A value of -1 means don't highlight any point.
   */
  public void setHighlightedPoint(int n)
  {
    if (n != this.highlightedPoint)
    {
      this.setChanged(HIGHLIGHT_CHANGE_PREFIX + this.highlightedPoint);
      this.highlightedPoint = n;
    }
  }



  /**
   * Plots this graph on the specified canvas using the provided
   * Graphics object.
   *
   * Does nothing if any Axis or Variable of this is currently null.
   */
  public void plot(Component canvas, Graphics g)
  {
    try
    {
      g.setColor(this.color);

      int numPoints = this.getNumberOfPoints();
      Point prevPoint = null, nextPoint;

      for (int i = 0; i < numPoints; i++, prevPoint = nextPoint)
      {
	nextPoint = this.getPoint(canvas, i);

	// draw accent circle around next point, if applicable
	if (accentPoints)
	  g.drawOval(nextPoint.x - 2, nextPoint.y - 2, 4, 4);

	// draw line segment connecting previous point to next point, if
	// applicable (note: it's never applicable if this is the very
	// first point)
	if (i != 0 && connectPattern.isConnected(i))
	  g.drawLine(prevPoint.x, prevPoint.y, nextPoint.x, nextPoint.y);

	// highlight next point, if applicable
	if (i == this.getHighlightedPoint())
	{
	  int r = HIGHLIGHT_RADIUS - 1;
	  g.drawOval(nextPoint.x - r, nextPoint.y - r, 2*r, 2*r);
	  r++;
	  g.drawOval(nextPoint.x - r, nextPoint.y - r, 2*r, 2*r);
	}
      }
    }
    catch (IndexOutOfBoundsException ex)
    {
      // Do nothing.
      //
      // This exception probably means that one of the Axes is null.
      // Alternatively, due to threading issues it's possible that the
      // data may suddenly change or vanish in the middle of
      // plotting... in this case we abort, and trust the application
      // will make sure that plot gets called again once the data is
      // back in a consistent state.
    }
  }



  /**
   * Returns the pixel location where the nth data point in this graph
   * would be plotted on the specified canvas.
   *
   * @throws IndexOutOfBoundsException if this graph does not have an
   * nth point (note: this includes all cases where any of the Axes or
   * Variables of this is currently null).
   */
  public Point getPoint(Component canvas, int n)
  {
    // subtract 1 so that everything actually fits *within* the canvas
    int width = canvas.getSize().width - 1;
    int height = canvas.getSize().height - 1;

    if (xAxis == null || yAxis == null)
      throw new IndexOutOfBoundsException();

    int x = xAxis.rescaleDataValue(this.getXValue(n), 0, width);
    int y = yAxis.rescaleDataValue(this.getYValue(n), height, 0);

    return new Point(x, y);
  }



  /**
   * Returns the index of the data point in this graph which would be
   * plotted nearest to the specified point on the specified canvas.
   * If this graph contains no data points, returns -1.
   */
  public int findNearestPoint(Component canvas, Point position)
  {
    int nearestPoint = -1;
    double distanceSq = 0;

    for (int i = 0, n = this.getNumberOfPoints(); i < n; i++)
    {
      double nextDSq = position.distanceSq(this.getPoint(canvas, i));
      if (nearestPoint == -1 || nextDSq < distanceSq)
      {
	nearestPoint = i;
	distanceSq = nextDSq;
      }
    }

    return nearestPoint;
  }



  public Object clone()
  {
    try
    {
      Graph clone = (Graph) super.clone();
      clone.deleteObservers();

      if (xAxis != null)
	xAxis.addGraph(clone);
      if (yAxis != null)
	yAxis.addGraph(clone);

      return clone;
    }
    catch (CloneNotSupportedException ex)
    {
      throw new Error(ex);
    }
  }



  ////////////////////
  // Helper Methods //
  ////////////////////


  // returns the number of points in this graph (equal to the
  // *maximum* length of either variable's data, or zero if at least
  // one of the variables is null or has no data)
  private int getNumberOfPoints()
  {
    if (xVar == null || yVar == null)
      return 0;

    int xSize = xVar.getData().size();
    int ySize = yVar.getData().size();

    if (xSize == 0 || ySize == 0) {
      return 0;
    }
    else if (xSize > ySize) {
      return xSize;
    }
    else {
      return ySize;
    }
  }

  // returns the X value of the nth point in this graph.  If the x
  // variable does not have an nth point but the graph does, uses the
  // last data value in the x variable.  If the entire graph does not
  // have an nth point (note: this includes cases where the x variable
  // is null or has no data), throws an IndexOutOfBoundsException.
  private BigDecimal getXValue(int n)
  {
    if (n >= this.getNumberOfPoints() || n < 0)
      throw new IndexOutOfBoundsException();

    List data = xVar.getData();

    if (n < data.size())
      return (BigDecimal) data.get(n);
    else
      return (BigDecimal) data.get(data.size() - 1);
  }

  // returns the Y value of the nth point in this graph.  If the y
  // variable does not have an nth point but the graph does, uses the
  // last data value in the y variable.  If the entire graph does not
  // have an nth point (note: this includes cases where the y variable
  // is null or has no data), throws an IndexOutOfBoundsException.
  private BigDecimal getYValue(int n)
  {
    if (n >= this.getNumberOfPoints() || n < 0)
      throw new IndexOutOfBoundsException();

    List data = yVar.getData();

    if (n < data.size())
      return (BigDecimal) data.get(n);
    else
      return (BigDecimal) data.get(data.size() - 1);
  }

} // end class Graph
