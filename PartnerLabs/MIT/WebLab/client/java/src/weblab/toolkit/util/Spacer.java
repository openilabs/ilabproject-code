/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.util;

import javax.swing.*;
import java.awt.*;

/**
 * An invisible Swing Component used to take up space in container
 * layouts.  Picking up where javax.swing.Box left off, this class
 * contains factory methods for producing a variety of useful
 * "doppelganger" components.
 */
public class Spacer extends JComponent
{
  private Component target;
  private boolean active;

  // -1 for variable, -2 for mimic, or a nonnegative fixed value
  private int width;
  private int height;

  // private constructor
  private Spacer(Component target, boolean active,
			int width, int height)
  {
    this.target = target;
    this.active = active;
    this.width = width;
    this.height = height;
  }

  public Dimension getPreferredSize()
  {
    return determineSize(target.getPreferredSize(), 0);
  }

  public Dimension getMinimumSize()
  {
    return determineSize(target.getMinimumSize(), 0);
  }

  public Dimension getMaximumSize()
  {
    return determineSize(target.getMaximumSize(), Integer.MAX_VALUE);
  }

  // helper method for getXSize.  d is a Dimension containing the
  // passive mimic mode width and height; variableValue is the value
  // to use for variable mode width or height (either 0 or
  // Integer.MAX_VALUE).
  private Dimension determineSize(Dimension d, int variableValue)
  {
    int w, h;

    switch (this.width) {
    case -2: // mimic
      if (this.active)
	w = target.getSize().width;
      else
	w = d.width;
      break;
    case -1: // variable
      w = variableValue;
      break;
    default: // fixed
      w = this.width;
    }

    switch (this.height) {
    case -2: // mimic
      if (this.active)
	h = target.getSize().height;
      else
	h = d.height;
      break;
    case -1: // variable
      h = variableValue;
      break;
    default: // fixed
      h = this.height;
    }

    return new Dimension(w, h);
  }



  /**
   * Creates a new doppelganger component -- an invisible Swing
   * Component that determines its minimum, maximum, and preferred
   * sizes by mimicking the dimensions of another Component.
   *
   * @param target the Component whose dimensions the returned
   * doppelganger will mimic
   * @param active determines whether the minimum, maximum, and
   * preferred size dimensions of this will be based on the current
   * size of target (active = true) or on the minimum, maximum, and
   * preferred sizes of target (active = false).
   */
  public static JComponent
    createDoppelganger(Component target, boolean active)
  {
    return new Spacer(target, active, -2, -2);
  }

  /**
   * Creates a new doppelganger component which mimics the width of
   * its target but can have any height (zero to Integer.MAX_VALUE).
   */
  public static JComponent
    createHorizontalDoppelganger(Component target, boolean active)
  {
    return new Spacer(target, active, -2, -1);
  }

  /**
   * Creates a new doppelganger component which mimics the width of
   * its target but has fixed height.
   *
   * requires: height >= 0
   */
  public static JComponent
    createHorizontalDoppelganger(Component target, boolean active, int height)
  {
    return new Spacer(target, active, -2, height);
  }

  /**
   * Creates a new doppelganger component which mimics the height of
   * its target but can have any width (zero to Integer.MAX_VALUE).
   */
  public static JComponent
    createVerticalDoppelganger(Component target, boolean active)
  {
    return new Spacer(target, active, -1, -2);
  }

  /**
   * Creates a new doppelganger component which mimics the height of
   * its target but has fixed width.
   *
   * requires: width >= 0
   */
  public static JComponent
    createVerticalDoppelganger(Component target, boolean active, int width)
  {
    return new Spacer(target, active, width, -2);
  }

} // end class Spacer
