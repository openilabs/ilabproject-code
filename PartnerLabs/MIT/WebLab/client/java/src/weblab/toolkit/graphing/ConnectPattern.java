/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.graphing;

/**
 * Specifies which pairs of successive data points in a graph should
 * be visually connected by line segments when the graph is plotted.
 *
 * ConnectPatterns are considered immutable; implementing classes
 * should make sure that the behavior of isConnected() will not change
 * over time.
 */
public interface ConnectPattern
{
  /**
   * Returns true iff the nth point on the graph should be connected
   * to the point before it.  n is indexed starting from zero, so
   * isConnected(1) actually determines whether the second point
   * should be connected to the first point.  isConnected(0) is
   * meaningless.
   */
  public boolean isConnected(int n);

} // end interface ConnectPattern
