package weblab.graphing;

// JAVA 1.1 COMPLIANT

import weblab.util.ChangeTrackingObservable;

import java.awt.Graphics;
import java.awt.Component;
import java.awt.Color;
import java.awt.Point;

import java.math.BigDecimal;

import java.util.Observable;
import java.util.Observer;

/**
 * Represents the graph of a relationship between two Axes: an
 * (independent) xAxis and a (dependent) yAxis.
 */
public class Graph extends ChangeTrackingObservable
  implements Observer
{
  private Axis xAxis;
  private Axis yAxis;

  private Color color;
  private boolean accentPoints;

  private ConnectPattern connectPattern;

  private int highlightedPoint;

  /**
   * Flag value to inform Observers that an Axis associated with this
   * has been changed (note: "changed" in this context does NOT mean
   * "replaced" -- an Axis of a Graph cannot be replaced).
   */
  public static final String AXIS_CHANGE = "axis";

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
  // note: the actual implementation is NOT currently based on this
  // number, it's only here so that it can be referred to in other
  // classes that want to do smart repainting.
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
   * Constructs a new Graph with the given xAxis and yAxis, and
   * default values for color (black), accent points (true),
   * highlighted point (none), and connect pattern (always).
   *
   * Note: it's very easy to accidentally reverse the order of the
   * arguments.  Think of the phrasing "Graph y versus x" or "Graph y
   * as a function of x".
   */
  public Graph(Axis yAxis, Axis xAxis)
  {
    this.xAxis = xAxis;
    this.yAxis = yAxis;

    // set default values
    this.color = Color.black;
    this.accentPoints = true;
    this.highlightedPoint = -1;
    this.connectPattern = ALWAYS_CONNECT;

    // listen for changes to axes
    this.xAxis.addObserver(this);
    this.yAxis.addObserver(this);
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
   * being highlighted.
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
   */
  public void plot(Component canvas, Graphics g)
  {
    try
    {
      g.setColor(this.color);

      int numPoints = this.getNumberOfPoints();
      Point oldp = null, newp;

      for (int i = 0; i < numPoints; i++, oldp = newp)
      {
	newp = this.getPoint(canvas, i);

	// draw accent circle around next point, if applicable
	if (accentPoints)
	  g.drawOval(newp.x - 2, newp.y - 2, 4, 4);

	// draw line segment connecting previous point to next point, if
	// applicable (note: it's never applicable if this is the very
	// first point)
	if (i != 0 && connectPattern.isConnected(i))
	  g.drawLine(oldp.x, oldp.y, newp.x, newp.y);

	// highlight this point, if applicable
	if (i == this.getHighlightedPoint())
	{
	  g.drawOval(newp.x - 4, newp.y - 4, 8, 8);
	  g.drawOval(newp.x - 5, newp.y - 5, 10, 10);
	}
      }
    }
    catch (IndexOutOfBoundsException ex)
    {
      // do nothing.  Due to threading issues, it's possible that the
      // data may suddenly change or even vanish in the middle of
      // plotting.  In this case, we abort, and trust that the
      // application will make sure plot gets called again once the
      // data is back in a consistent state.
    }
  }



  /**
   * Returns the nth point in this graph (coordinates measured in
   * pixels with respect to the canvas provided).
   *
   * @throws IndexOutOfBoundsException if the graph does not have an
   * nth point.
   */
  public Point getPoint(Component canvas, int n)
  {
    // subtract 1 so that everything actually fits *within* the canvas
    int width = canvas.getSize().width - 1;
    int height = canvas.getSize().height - 1;

    int x = xAxis.rescaleDataValue(this.getXValue(n), 0, width);
    int y = yAxis.rescaleDataValue(this.getYValue(n), height, 0);

    return new Point(x, y);
  }



  /**
   * Returns the index of the data point which is nearest to the specified
   * point.  If this graph contains no data, returns -1.
   */
  public int findNearestPoint(Component canvas, Point p)
  {
    int numPoints = this.getNumberOfPoints();

    int nearestPoint = -1;
    double minDistance = Double.POSITIVE_INFINITY;
    double newDistance;

    for (int i = 0; i < numPoints; i++)
    {
      newDistance = distanceBetweenPoints(p, this.getPoint(canvas, i));
      if (newDistance < minDistance)
      {
	nearestPoint = i;
	minDistance = newDistance;
      }
    }

    return nearestPoint;
  }



  /**
   * Calculates the distance between two Points.
   */
  public static double distanceBetweenPoints(Point p, Point q)
  {
    return Math.sqrt((q.x - p.x) * (q.x - p.x) +
		     (q.y - p.y) * (q.y - p.y));
  }



  //////////////////////
  // Listener Methods //
  //////////////////////


  /**
   * When either axis changes, notify observers of this that this has
   * changed.
   */
  public void update(Observable o, Object arg)
  {
    this.setChanged(AXIS_CHANGE);
    this.notifyObservers();
  }



  ////////////////////
  // Helper Methods //
  ////////////////////


  // returns the number of points in this graph (equal to the
  // *maximum* length of an axis, or zero if at least one axis is
  // completely empty)
  private int getNumberOfPoints()
  {
    int xSize = xAxis.getData().length;
    int ySize = yAxis.getData().length;

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
  // axis does not have an nth point but the graph does, uses the last
  // point in the x axis.  If the x axis has no data, or if the entire
  // graph does not have an nth point, throws an
  // IndexOutOfBoundsException.
  private BigDecimal getXValue(int n)
  {
    if (n >= this.getNumberOfPoints())
      throw new IndexOutOfBoundsException();

    BigDecimal[] data = xAxis.getData();

    if (n < data.length)
      return data[n];
    else
      return data[data.length - 1];
  }

  // returns the Y value of the nth point in this graph.  If the y
  // axis does not have an nth point but the graph does, uses the last
  // point in the y axis.  If the y axis has no data, or if the entire
  // graph does not have an nth point, throws an
  // IndexOutOfBoundsException.
  private BigDecimal getYValue(int n)
  {
    if (n >= this.getNumberOfPoints())
      throw new IndexOutOfBoundsException();

    BigDecimal[] data = yAxis.getData();

    if (n < data.length)
      return data[n];
    else
      return data[data.length - 1];
  }

} // end class Graph
