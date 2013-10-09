/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.graphing;

import java.util.List;


/**
 * XXX
 * 
 * TrackingListener provides an easy way to implement tracking
 * functionality.  Don't want this class to include specific display
 * details, but to take care of as much as possible of the
 * functionality in a general way.
 */
public interface TrackingListener
{
  /**
   * Display handler for continuous tracking: for each Axis, a value
   * is displayed that corresponds to the current mouse position on
   * that Axis.  The arguments to this method are a pair of parallel
   * Lists XXX.
   *
   * requires: axes.size() == values.size()
   *
   * @param axes contains Axis
   * @param values contains BigDecimal, null if the Axis is not active
   */
  public abstract void displayContinuousTrackingInfo(List axes, List values);

  public abstract void displaySnapTrackingInfo(List variables, List values);

  /**
   * Clears all tracking info.
   */
  public abstract void clearTrackingInfo();

} // end interface TrackingListener
