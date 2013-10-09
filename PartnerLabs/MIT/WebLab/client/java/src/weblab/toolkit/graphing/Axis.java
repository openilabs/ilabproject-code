/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.graphing;

import weblab.toolkit.util.EngMath;
import weblab.toolkit.util.ChangeTrackingObservable;

import java.math.BigDecimal;

import java.util.Observer;
import java.util.Observable;

import java.util.Collections;
import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.Set;
import java.util.Map;
import java.util.HashSet;
import java.util.WeakHashMap;

/**
 * Represents one axis of a graph.
 *
 * Axis has one special invariant: in LOG_SCALE, both min and max must
 * be greater than zero (since the log of a negative number is not
 * defined).  Set methods enforce this invariant by changing whatever
 * else they need to (e.g. if the scale is LOG_SCALE and you try to
 * set the min to zero, setMin will first set the scale to
 * LINEAR_SCALE and then set the min to zero).
 *
 * Note: it's perfectly okay to have min > max if you want.
 *
 * Note: in LOG_SCALE, signs of data points are ignored and absolute
 * values are used.  Also, any value smaller than LOG_SCALE_MIN_VALUE
 * is replaced by LOG_SCALE_MIN_VALUE.
 */
public class Axis extends ChangeTrackingObservable
  implements Observer, Cloneable
{
  // scale can be linear or logarithmic
  public static final int LINEAR_SCALE = 1;
  public static final int LOG_SCALE = 2;

  /**
   * Minimum acceptable value in log scale (1e-15 by default).  Used
   * whenever we would otherwise want to set something to zero.
   */
  public static BigDecimal LOG_SCALE_MIN_VALUE =
    BigDecimal.valueOf(1, 15); // 1e-15

  /**
   * Flag value to inform Observers that the set of Graphs to which
   * this Axis is currently assigned has changed.
   */
  public static final String GRAPHS_CHANGE = "graphs";

  /**
   * Flag value to inform Observers that the units of this Axis have
   * changed.
   */
  public static final String UNITS_CHANGE = "units";


  private boolean continuousAutoscale;

  private String name;
  private BigDecimal min, max;
  private int scale;

  // contains Graph
  //
  // keeps track of all the Graphs that are using this Axis (and whose
  // corresponding Variables should therefore be taken into
  // consideration for autoscaling and determining axis units)
  private Set graphs;

  // maps Variable to BigDecimal[4]: {min, max, logMin, logMax}
  //
  // Caches the minimum and maximum data values of each variable for
  // purposes of autoscaling in linear and logarithmic mode, to avoid
  // unnecessary continual recalculation.
  private static Map varStatsCache = new WeakHashMap();

  // keeps track of the most recently computed unit symbol for this
  // Axis, and whether the units are consistent.  See getUnits()
  // method specification for detailed semantics.  Note that the value
  // of unitSymbol is only meaningful when unitsConsistent is true.
  private String unitSymbol;
  private boolean unitsConsistent;


  /**
   * Creates a new Axis with default values.
   */
  public Axis(String name)
  {
    this.name = name;
    this.scale = LINEAR_SCALE;
    this.min = this.max = BigDecimal.valueOf(0);
    this.continuousAutoscale = true;

    this.graphs = new HashSet();
  }



  /**
   * Returns the name of this axis.
   */
  public String getName()
  {
    return this.name;
  }



  /**
   * Returns the min value for this axis.
   */
  public BigDecimal getMin()
  {
    return this.min;
  }



  /**
   * Sets the min value for this axis.
   *
   * Note: has no effect when continuousAutoscale is enabled
   */
  public void setMin(BigDecimal newMin)
  {
    if (! continuousAutoscale)
      this.actuallySetMin(newMin);
  }



  /**
   * Returns the max value for this axis.
   */
  public BigDecimal getMax()
  {
    return this.max;
  }



  /**
   * Sets the max value for this axis.
   *
   * Note: has no effect when continuousAutoscale is enabled
   */
  public void setMax(BigDecimal newMax)
  {
    if (! continuousAutoscale)
      actuallySetMax(newMax);
  }



  /**
   * Returns the scale of this axis (LINEAR_SCALE or LOG_SCALE).
   */
  public int getScale()
  {
    return this.scale;
  }



  /**
   * Sets the scale of this axis.
   *
   * requires: newScale must be either LINEAR_SCALE or LOG_SCALE
   */
  public void setScale(int newScale)
  {
    if (newScale != LINEAR_SCALE && newScale != LOG_SCALE)
      throw new Error("illegal scale: " + newScale);

    if (newScale != this.scale)
    {
      // preserve invariant
      if (newScale == LOG_SCALE && this.min.signum() <= 0)
      {
	this.actuallySetMin(LOG_SCALE_MIN_VALUE);
      }
      if (newScale == LOG_SCALE && this.max.signum() <= 0)
      {
	this.actuallySetMax(LOG_SCALE_MIN_VALUE);
      }

      // set the scale
      this.scale = newScale;
      this.setChanged();

      // re-autoscale if appropriate
      if (continuousAutoscale)
	autoscale();
    }
  }



  /**
   * Returns true if this Axis is actively being used for graphing
   * (i.e. at least one variable is currently being graphed on it).
   */
  public synchronized boolean isActive()
  {
    for (Iterator i = graphs.iterator(); i.hasNext(); )
    {
      Graph g = (Graph) i.next();
      if (g.isActive())
	return true;
    }
    return false;
  }



  /**
   * Returns an unmodifiable Set of the Graphs that are using this
   * Axis.
   */
  public synchronized Set getGraphs()
  {
    return Collections.unmodifiableSet(this.graphs);
  }



  /**
   * Returns the unit symbol for this Axis (i.e. the unit symbol of
   * all the Variables currently being graphed on this Axis, assuming
   * they match).  Null may be used to indicate dimensionless
   * quantities, and is also the default return value if no Variables
   * are currently being graphed on this Axis.
   *
   * @throws InconsistentUnitsException if the Variables currently
   * being graphed on this Axis do not all have the same unit symbol.
   */
  public String getUnits() throws InconsistentUnitsException
  {
    if (! this.unitsConsistent)
      throw new InconsistentUnitsException();

    return this.unitSymbol;
  }



  /**
   * Automatically sets Min and Max based on the data of all Variables
   * currently being graphed on this, and the scale of this
   * (preserving the LOG_SCALE invariant if in LOG_SCALE).
   *
   * Note: if continuousAutoscale is enabled, this method will
   * automatically be called whenever something else is changed that
   * might affect the values it generates.
   */
  public synchronized void autoscale()
  {
    BigDecimal newMin = null, newMax = null, tmpMin, tmpMax;
    BigDecimal tmpStats[];

    // Iterate over all variables being graphed by this...
    for (Iterator i = graphs.iterator(); i.hasNext(); )
    {
      Variable v = ((Graph) i.next()).getActiveVariableForAxis(this);
      if (v == null)
	continue;

      // find the min and max values for this variable that are
      // applicable to the current scale
      //
      tmpStats = findVarStats(v); // {min, max, logMin, logMax}
      if (this.scale == LOG_SCALE)
      {
	tmpMin = tmpStats[2];
	tmpMax = tmpStats[3];
      }
      else
      {
	tmpMin = tmpStats[0];
	tmpMax = tmpStats[1];
      }
      
      // update newMin and newMax if appropriate (note: first
      // variable found sets the standard for newMin and newMax)
      if (tmpMin != null && (newMin == null || tmpMin.compareTo(newMin) < 0))
	newMin = tmpMin;
      
      if (tmpMax != null && (newMax == null || tmpMax.compareTo(newMax) > 0))
	newMax = tmpMax;
    }

    // if we didn't find any values, do nothing
    if (newMin == null || newMax == null)
      return;

    // set Min and Max (using the private methods that don't silently
    // fail to work when continuousAutoscale is on)
    this.actuallySetMin(newMin);
    this.actuallySetMax(newMax);
  }



  /**
   * Returns true iff continuousAutoscale is enabled.
   */
  public boolean getContinuousAutoscale()
  {
    return this.continuousAutoscale;
  }



  /**
   * Sets whether this should automatically autoscale itself whenever
   * something happens that might change the autoscaled min and max.
   */
  public void setContinuousAutoscale(boolean autoscale)
  {
    if (autoscale && ! this.continuousAutoscale)
    {
      this.continuousAutoscale = true;
      this.setChanged();
      // autoscale now
      this.autoscale();
    }
    else if (!autoscale && this.continuousAutoscale)
    {
      this.continuousAutoscale = false;
      this.setChanged();
    }
  }



  /**
   * Maps a data value from the BigDecimal-valued interval [Min, Max]
   * associated with this axis onto the specified integer-valued
   * interval [rescaledMin, rescaledMax].  Thus a value equal to Min
   * is mapped to rescaledMin, a value equal to Max is mapped to
   * rescaledMax, and values in between are scaled appropriately.
   *
   * The scale of this axis is taken into account.  In LOG_SCALE, a
   * return value halfway between rescaledMin and rescaledMax
   * corresponds to the data value whose logarithm is halfway between
   * log(Min) and log(Max).
   *
   * Data values not between Min and Max can be rescaled, but note
   * that in this case the result will not be between rescaledMin and
   * rescaledMax.
   *
   * This method makes no assumptions about rescaledMin and
   * rescaledMax except that they fit comfortably within the size
   * bounds of the Java integer type.  It is perfectly okay to specify
   * a target interval where rescaledMin > rescaledMax.
   *
   * If Min and Max are currently equal, returns Integer.MAX_VALUE,
   * Integer.MIN_VALUE, or (rescaledMin+rescaledMax)/2, depending on
   * whether dataValue is bigger, smaller, or also the same.
   */
  public int rescaleDataValue(BigDecimal dataValue,
			      int rescaledMin, int rescaledMax)
  {
    BigDecimal rescaledValue;

    // special cases (to prevent dividing by zero later)
    if (this.min.compareTo(this.max) == 0)
    {
      if (this.scale == LOG_SCALE)
	dataValue = fixDataForLogscale(dataValue);

      switch(dataValue.compareTo(this.min))
      {
      case 1:
	return Integer.MAX_VALUE;
      case -1:
	return Integer.MIN_VALUE;
      case 0:
	return (rescaledMin + rescaledMax) / 2;
      }
    }
    if (rescaledMin == rescaledMax)
      return rescaledMin;

    // perform rescaling in BigDecimal domain
    switch(this.scale)
    {
    case LINEAR_SCALE:
      // rescaledValue = (dataValue - this.min) / (this.max - this.min)
      // * (rescaledMax - rescaledMin) + rescaledMin;
      rescaledValue = dataValue
	.subtract(this.min)
	.multiply(BigDecimal.valueOf(rescaledMax - rescaledMin))
	.divide(this.max.subtract(this.min), 0, BigDecimal.ROUND_HALF_UP)
	.add(BigDecimal.valueOf(rescaledMin));
      break;

    case LOG_SCALE:
      // rescaledValue = (log(dataValue) - log(this.min)) /
      // (log(this.max) - log(this.min)) * (rescaledMax - rescaledMin)
      // + rescaledMin;
      rescaledValue =
	EngMath.log(fixDataForLogscale(dataValue))
	.subtract(EngMath.log(this.min))
	.multiply(BigDecimal.valueOf(rescaledMax - rescaledMin))
	.divide
	(EngMath.log(this.max).subtract(EngMath.log(this.min)),
	 0, BigDecimal.ROUND_HALF_UP)
	.add(BigDecimal.valueOf(rescaledMin));
      break;

    default:
      throw new Error("illegal scale: " + this.scale);
    }

    // convert to integer, being careful to avoid overflow
    if (rescaledValue.compareTo(BigDecimal.valueOf(Integer.MAX_VALUE)) > 0)
      return Integer.MAX_VALUE;
    else if (rescaledValue.compareTo(BigDecimal.valueOf(Integer.MIN_VALUE))
	     < 0)
      return Integer.MIN_VALUE;
    else
      return rescaledValue.intValue();
  }



  /**
   * unscaleToDataValue is the inverse of rescaleDataValue.
   *
   * Maps an integer value from the specified interval [scaledMin,
   * scaledMax] to a data value in the BigDecimal-valued interval
   * [Min, Max] associated with this axis.  Thus a scaledValue equal
   * to scaledMin is mapped to Min, a scaledValue equal to scaledMax
   * is mapped to Max, and scaledValues in between are (un)scaled
   * appropriately.  The resulting data value is rounded to numsigfigs
   * significant figures.
   *
   * The scale of this axis is taken into account.  In LOG_SCALE, a
   * scaledValue halfway between scaledMin and scaledMax maps to the
   * data value whose logarithm is halfway between log(Min) and
   * log(Max).
   *
   * scaledValues not between scaledMin and scaledMax can be
   * (un)scaled, but note that in this case the resulting data value
   * will not be between Min and Max.
   *
   * This method makes no assumptions about scaledMin and scaledMax
   * except that they are not equal and fit comfortably within the
   * size bounds of the Java integer type.  It is perfectly okay to
   * specify a target interval where scaledMin > scaledMax.
   *
   * requires: scaledMin != scaledMax
   */
  public BigDecimal unscaleToDataValue(int scaledValue,
				       int scaledMin, int scaledMax,
				       int numsigfigs)
  {
    BigDecimal value;

    if (scaledMin == scaledMax)
      throw new Error("failed requirement: scaledMin != scaledMax");
    if (this.min.compareTo(this.max) == 0)
      return this.min;


    switch(this.scale)
    {
    case LINEAR_SCALE:
      // value = (scaledValue - scaledMin) / (scaledMax - scaledMin) *
      // (this.max - this.min) + this.min
      value =
	new BigDecimal(((double)scaledValue - scaledMin)
		       / ((double)scaledMax - scaledMin))
	.multiply(this.max.subtract(this.min))
	.add(this.min);
      break;


    case LOG_SCALE:
      // log10(value) = (scaledValue - scaledMin) / (scaledMax -
      // scaledMin) * (log10(this.max) - log10(this.min)) +
      // log10(this.min)
      //
      // so
      //
      // value = this.min * 10**[(scaledValue - scaledMin) / (scaledMax -
      // scaledMin) * (log10(this.max) - log10(this.min))]
      value =
	this.min.multiply
	(EngMath.pow10
	 (new BigDecimal(((double)scaledValue - scaledMin)
			 / ((double)scaledMax - scaledMin))
	  .multiply(EngMath.log10(this.max)
		    .subtract(EngMath.log10(this.min)))));
      break;

    default:
      throw new Error("illegal scale: " + this.scale);
    }

    return EngMath.round(value, numsigfigs);
  }



  /**
   * Returns the size of each division if this axis is split into
   * nDivs divisions (note: in LOG_SCALE, the answer returned is in
   * decades), rounded to numsigfigs significant figures.
   *
   * requires: nDivs > 0
   */
  public BigDecimal getDivSize(int nDivs, int numsigfigs)
  {
    if (nDivs <= 0)
      throw new Error("illegal nDivs: " + nDivs);

    switch(this.scale)
    {
    case LINEAR_SCALE:
      // (max - min) / nDivs
      return EngMath.round
	(this.max
	 .subtract(this.min)
	 .multiply(new BigDecimal((double)1.0 / nDivs)),
	 numsigfigs);

    case LOG_SCALE:
      // (log10(max) - log10(min)) / nDivs
      return EngMath.round
	(EngMath.log10(this.max)
	 .subtract(EngMath.log10(this.min))
	 .multiply(new BigDecimal((double)1.0 / nDivs)),
	 numsigfigs);

    default:
      throw new Error("illegal scale: " + this.scale);
    }
  }



  /**
   * Returns the preferred number of divisions for graphing this axis.
   * Currently this is fixed at 10 for LINEAR_SCALE, and equal to the
   * number of decades between min and max for LOG_SCALE (with a
   * mininum of 2 and maximum of 30).
   */
  public int getPreferredDivs()
  {
    switch(this.getScale())
    {
    case LINEAR_SCALE:
      return 10;

    case LOG_SCALE:
      int divs = EngMath.log10ceiling(this.getMax()) -
	EngMath.log10floor(this.getMin());
      if (divs < 2)
	divs = 2;
      else if (divs > 30)
	divs = 30;
      return divs;

    default:
      throw new Error("illegal scale: " + this.getScale());
    }
  }



  /**
   * Handle updates from Observables
   */
  public void update(Observable o, Object arg)
  {
    // when a graph's variable or axis is changed...
    if (graphs.contains(o))
    {
      Set changes = ChangeTrackingObservable.extractChanges(arg);

      if (changes.contains(Graph.VARIABLE_CHANGE) ||
	  changes.contains(Graph.AXIS_CHANGE))
      {
	// units may have changed (from nothing to something, or from
	// something to inconsistent)
	this.updateUnits();

	// data may have changed -> re-autoscale if appropriate
	if (continuousAutoscale)
	  autoscale();

	// notify observers that this may have changed
	this.notifyObservers();
      }
    }
  }



  // make Axis suitable for displaying e.g. in a JComboBox
  public String toString()
  {
    return name;
  }



  public Object clone()
  {
    try
    {
      Axis clone = (Axis) super.clone();
      clone.deleteObservers();

      // no Graphs are using the newly cloned Axis yet
      clone.graphs = new HashSet();

      return clone;
    }
    catch (CloneNotSupportedException ex)
    {
      throw new Error(ex);
    }
  }



  ///////////////////////
  // Protected Methods //
  ///////////////////////


  /**
   * Adds g to the set of Graphs that are using this Axis (and whose
   * corresponding Variables should therefore be taken into
   * consideration for autoscaling and determining units).
   *
   * Note: care should be taken to call removeGraph when g is no
   * longer using this Axis.
   */
  protected synchronized void addGraph(Graph g)
  {
    graphs.add(g);
    g.addWeakReferenceObserver(this);

    this.setChanged(GRAPHS_CHANGE);

    // Since we may have just added a Variable to this Axis, recheck
    // units and re-autoscale (if appropriate).

    this.updateUnits();

    if (continuousAutoscale)
      autoscale();
  }



  /**
   * Removes g from the set of graphs that are using this Axis.
   */
  protected synchronized void removeGraph(Graph g)
  {
    graphs.remove(g);
    g.deleteObserver(this);

    this.setChanged(GRAPHS_CHANGE);

    // Since we may have just removed a Variable from this Axis,
    // recheck units and re-autoscale (if appropriate).

    this.updateUnits();

    if (continuousAutoscale)
      autoscale();
  }



  ////////////////////
  // Helper Methods //
  ////////////////////


  // returns BigDecimal[4]: {min, max, logMin, logMax}
  //
  // These are the min and max values of the variable data of v, for
  // purposes of autoscaling in linear scale and log scale
  // respectively (in log scale all data is treated using absolute
  // values, the LOG_SCALE_MIN_VALUE limit applies, and the desired
  // min and max are rounded to the nearest powers of 10)
  //
  // note: in the degenerate case of a Variable with an empty data
  // list, this method returns {null, null, null, null}.
  private static BigDecimal[] findVarStats(Variable v)
  {
    BigDecimal[] stats = (BigDecimal[]) varStatsCache.get(v);

    if (stats == null)
    {
      List data = v.getData();

      // find minimum and maximum data values
      //
      BigDecimal newMin = null, newMax = null,
	newLogMin = null, newLogMax = null, tmp;

      for (int i = 0, n = data.size(); i < n; i++)
      {
	tmp = (BigDecimal) data.get(i);
	if (newMin == null || tmp.compareTo(newMin) < 0)
	  newMin = tmp;
	if (newMax == null || tmp.compareTo(newMax) > 0)
	  newMax = tmp;

	tmp = fixDataForLogscale(tmp);
	if (newLogMin == null || tmp.compareTo(newLogMin) < 0)
	  newLogMin = tmp;
	if (newLogMax == null || tmp.compareTo(newLogMax) > 0)
	  newLogMax = tmp;
      }

      // In log scale, round min and max to nearest powers of 10
      //
      if (newLogMin != null)
	newLogMin = BigDecimal.valueOf(1).movePointRight
	  (EngMath.log10floor(newLogMin));
      if (newLogMax != null)
	newLogMax = BigDecimal.valueOf(1).movePointRight
	  (EngMath.log10ceiling(newLogMax));

      stats = new BigDecimal[] {newMin, newMax, newLogMin, newLogMax};
      varStatsCache.put(v, stats);
    }

    return stats;
  }



  // updates this.unitsConsistent and this.unitSymbol based on all the
  // Variables currently being graphed on this Axis by active Graphs.
  // See getUnits() method specification for detailed semantics.
  private synchronized void updateUnits()
  {
    String newUnits = null;
    boolean foundVar = false;

    for (Iterator i = graphs.iterator(); i.hasNext(); )
    {
      Variable v = ((Graph) i.next()).getActiveVariableForAxis(this);
      if (v == null)
	continue;

      if (! foundVar)
      {
	// first variable found sets the standard
	newUnits = v.getUnits();
	foundVar = true;
      }
      else if (! (newUnits == null ? v.getUnits() == null :
		  newUnits.equals(v.getUnits())))
      {
	// set new units: inconsistent!
	if (this.unitsConsistent)
	{
	  this.unitsConsistent = false;
	  this.setChanged(UNITS_CHANGE);
	}
	return;
      }
    }

    // set new units
    if (! (this.unitsConsistent &&
	   (newUnits == null ? this.unitSymbol == null :
	    newUnits.equals(this.unitSymbol))))
    {
      this.unitsConsistent = true;
      this.unitSymbol = newUnits;
      this.setChanged(UNITS_CHANGE);
    }
  }



  // does the actual work of setMin.  This is a separate method so
  // that it can be called from within other methods (e.g. autoscale)
  // in cases where it *does* need to work even when
  // continuousAutoscale is enabled.
  private void actuallySetMin(BigDecimal newMin)
  {
    if (newMin.compareTo(this.min) != 0)
    {
      // preserve invariant
      if (newMin.signum() <= 0 && this.scale == LOG_SCALE)
      {
	this.setScale(LINEAR_SCALE);
      }

      // set the min
      this.min = newMin;
      this.setChanged();
    }
  }



  // does the actual work of setMax.  This is a separate method so
  // that it can be called from within other methods (e.g. autoscale)
  // in cases where it *does* need to work even when
  // continuousAutoscale is enabled.
  private void actuallySetMax(BigDecimal newMax)
  {
    if (newMax.compareTo(this.max) != 0)
    {
      // preserve invariant
      if (newMax.signum() <= 0 && this.scale == LOG_SCALE)
      {
	this.setScale(LINEAR_SCALE);
      }

      // set the max
      this.max = newMax;
      this.setChanged();
    }
  }



  // "fixes" a data point for processing in log scale by taking its
  // absolute value and enforcing a minimum magnitude of
  // LOG_SCALE_MIN_VALUE
  private static BigDecimal fixDataForLogscale(BigDecimal x)
  {
    x = x.abs();
    if (x.compareTo(LOG_SCALE_MIN_VALUE) < 0)
      x = LOG_SCALE_MIN_VALUE;
    return x;
  }

} // end class Axis
