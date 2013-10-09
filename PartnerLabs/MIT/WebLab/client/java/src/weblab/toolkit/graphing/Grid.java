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

import java.util.Observable;
import java.util.Observer;


/**
 * Represents a rectangular grid that can be drawn behind plotted
 * graphs to help users interpret the data values.
 */
public class Grid extends ChangeTrackingObservable implements Observer
{
  private Color color;
  private int xDivs, yDivs;

  private Axis xFollow, yFollow;


  /**
   * Constructs a new Grid with default values for color (gray),
   * horizontal divisions (10), and vertical divisions (10).
   * Automatic xFollow and yFollow are disabled (set to null).
   */
  public Grid()
  {
    // set default values
    this.color = Color.gray;
    this.xDivs = 10;
    this.yDivs = 10;
    xFollow = yFollow = null;
  }



  /**
   * Returns the number of horizontal divisions.
   */
  public int getXDivs()
  {
    return this.xDivs;
  }



  /**
   * Sets the number of horizontal divisions (which determines the
   * number of vertical lines).
   *
   * Note: has no effect if xFollow is enabled
   */
  public void setXDivs(int newXDivs)
  {
    if (xFollow == null)
      this.actuallySetXDivs(newXDivs);
  }



  /**
   * Returns the Axis that is currently used to automatically
   * determine the number of horizontal divisions (vertical lines) on
   * this, or null if the number of horizontal divisions is currently
   * being set manually.
   */
  public Axis getXFollow()
  {
    return xFollow;
  }



  /**
   * Sets an Axis that should be used to automatically determine the
   * number of horizontal divisions (vertical lines) on this.  A null
   * argument disables this feature and allows the number of
   * horizontal divisions to be set manually using setXDivs.
   */
  public void setXFollow(Axis a)
  {
    if (xFollow != a)
    {
      // stop observing old xFollow (if any), unless it's also yFollow
      if (xFollow != null && yFollow != xFollow)
	xFollow.deleteObserver(this);

      // start observing new xFollow
      if (a != null)
	a.addWeakReferenceObserver(this);

      // set xFollow
      this.xFollow = a;

      // update number of divs now
      updateDivs();

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
   * Sets the number of vertical divisions (which determines the
   * number of horizontal lines).
   *
   * Note: has no effect if yFollow is enabled
   */
  public void setYDivs(int newYDivs)
  {
    if (yFollow == null)
      this.actuallySetYDivs(newYDivs);
  }



  /**
   * Returns the Axis that is currently used to automatically
   * determine the number of vertical divisions (horizontal lines) on
   * this, or null if the number of vertical divisions is currently
   * being set manually.
   */
  public Axis getYFollow()
  {
    return yFollow;
  }



  /**
   * Sets an Axis that should be used to automatically determine the
   * number of vertical divisions (horizontal lines) on this.  A null
   * argument disables this feature and allows the number of vertical
   * divisions to be set manually using setYDivs.
   */
  public void setYFollow(Axis a)
  {
    if (yFollow != a)
    {
      // stop observing old yFollow (if any), unless it's also xFollow
      if (yFollow != null && xFollow != yFollow)
	yFollow.deleteObserver(this);

      // start observing new yFollow
      if (a != null)
	a.addWeakReferenceObserver(this);

      // set yFollow
      this.yFollow = a;

      // update number of divs now
      updateDivs();

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


  //////////////////////
  // Listener methods //
  //////////////////////


  /**
   * Handle updates from Observables
   */
  public final void update(Observable o, Object arg)
  {
    // if an Axis we're following has changed, update the grid
    // divisions and notify observers that this has changed
    if (o == xFollow || o == yFollow)
    {
      updateDivs();
      this.notifyObservers();
    }
  }



  ////////////////////
  // Helper Methods //
  ////////////////////


  // does the actual work of setXDivs.  This is a separate method so
  // that it can be called from within other methods in cases where it
  // *does* need to work even when xFollow is enabled.
  private void actuallySetXDivs(int newXDivs)
  {
    if (newXDivs != this.xDivs)
    {
      this.xDivs = newXDivs;
      this.setChanged();
    }
  }

  // does the actual work of setYDivs.  This is a separate method so
  // that it can be called from within other methods in cases where it
  // *does* need to work even when yFollow is enabled.
  private void actuallySetYDivs(int newYDivs)
  {
    if (newYDivs != this.yDivs)
    {
      this.yDivs = newYDivs;
      this.setChanged();
    }
  }

  // updates xDivs and yDivs based on xFollow and yFollow
  private void updateDivs()
  {
    if (xFollow != null)
      this.actuallySetXDivs(xFollow.getPreferredDivs());

    if (yFollow != null)
      this.actuallySetYDivs(yFollow.getPreferredDivs());
  }

} // end class Grid
