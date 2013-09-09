package weblab.graphing;

// JAVA 1.1 COMPLIANT

import java.awt.Graphics;
import java.awt.Component;
import java.awt.Color;


/**
 * Represents a rectangular grid that can be drawn behind plotted
 * graphs to help users interpret the data values.
 */
public class Grid extends java.util.Observable
{
  private Color color;
  private int xDivs, yDivs;

  /**
   * Constructs a new Grid with default values for color (gray), x
   * divisions (10), and y divisions (10).
   */
  public Grid()
  {
    // set default values
    this.color = Color.gray;
    this.xDivs = 10;
    this.yDivs = 10;
  }



  /**
   * Returns the number of horizontal divisions.
   */
  public int getXDivs()
  {
    return this.xDivs;
  }



  /**
   * Sets the number of horizontal divisions (which corresponds to the
   * number of vertical lines).
   */
  public void setXDivs(int newXDivs)
  {
    if (newXDivs != this.xDivs)
    {
      this.xDivs = newXDivs;
      this.setChanged();
    }
  }



  /**
   * Returns the number of vertical divisions.
   */
  public int getYDivs()
  {
    return this.yDivs;
  }



  /**
   * Sets the number of vertical divisions (which corresponds to the
   * number of horizontal lines).
   */
  public void setYDivs(int newYDivs)
  {
    if (newYDivs != this.yDivs)
    {
      this.yDivs = newYDivs;
      this.setChanged();
    }
  }



  /**
   * Returns the current Color.
   */
  public Color getColor()
  {
    return this.color;
  }



  /**
   * Sets the Color that should be used to paint this grid.
   */
  public void setColor(Color newColor)
  {
    if (! newColor.equals(this.color))
    {
      this.color = newColor;
      this.setChanged();
    }
  }



  /**
   * Paints this grid onto the specified canvas using the provided
   * Graphics object.
   */
  public void paint(Component canvas, Graphics g)
  {
    g.setColor(this.color);

    // subtract 1 so that grid lines will fit *within* the canvas
    int width = canvas.getSize().width - 1;
    int height = canvas.getSize().height - 1;

    // draw yDivs+1 horizontal lines (including very top and very
    // bottom), evenly spaced along height
    for (int n = 0; n <= this.yDivs; n++)
    {
      int y = n * height / this.yDivs;
      g.drawLine(0, y, width, y);
    }

    // draw xDivs+1 vertical lines (including far left and far right),
    // evenly spaced along width
    for (int n = 0; n <= this.xDivs; n++)
    {
      int x = n * width / this.xDivs;
      g.drawLine(x, 0, x, height);
    }
  }

} // end class Grid
