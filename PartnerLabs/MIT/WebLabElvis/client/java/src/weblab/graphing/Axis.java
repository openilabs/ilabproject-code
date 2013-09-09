package weblab.graphing;

// JAVA 1.1 COMPLIANT

import java.math.BigDecimal;

import weblab.util.EngMath;

/**
 * Represents one axis of a graph.
 *
 * Subclasses must provide the methods for obtaining the axis's data
 * values.  Note that the Observable behavior of Axis should be used
 * by subclasses to update observers whenever the data changes.
 *
 * Axis has one special invariant: in LOG_SCALE, both min and max must
 * be greater than zero (the log of a negative number is not defined).
 * Set methods enforce this invariant by changing whatever else they
 * need to (e.g. if the scale is LOG_SCALE and you try to set the min
 * to zero, setMin will first set the scale to LINEAR_SCALE and then
 * set the min to zero).
 *
 * Note: it's perfectly okay to have min > max if you want.
 *
 * Note: in LOG_SCALE, signs of data points are ignored and absolute
 * values are used.  Also, any value smaller than LOG_SCALE_MIN_VALUE
 * is replaced by LOG_SCALE_MIN_VALUE.
 */
public abstract class Axis extends weblab.util.ChangeTrackingObservable
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

  private boolean continuousAutoscale;

  private String name;
  private BigDecimal min, max;
  private int scale;



  /**
   * Creates a new Axis with default values.
   */
  public Axis(String name)
  {
    this.name = name;
    this.scale = LINEAR_SCALE;
    this.min = this.max = BigDecimal.valueOf(0);
    // note: continuousAutoscale *must* default to false because we
    // can't perform an autoscale within the constructor (the data
    // won't be available until the subclass constructor finishes!)
    this.continuousAutoscale = false;
  }



  /**
   * Returns the data for this axis.
   */
  public abstract BigDecimal[] getData();



  /**
   * Returns the units for this axis.
   */
  public abstract String getUnits();



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
   * Automatically sets Min and Max based on the data and the scale of
   * this (preserving the LOG_SCALE invariant if in LOG_SCALE).
   *
   * Note: if continuousAutoscale is enabled, this method will
   * automatically be called whenever something else is changed that
   * might affect the values it generates.
   */
  public void autoscale()
  {
    BigDecimal[] data = this.getData();

    // if data contains no elements, do nothing
    if (data.length == 0)
      return;

    // find minimum and maximum data values
    //
    BigDecimal newMin, newMax;
    newMin = newMax = fixData(data[0]);
    for (int i = 1; i < data.length; i++)
    {
      BigDecimal d = fixData(data[i]);
      if (d.compareTo(newMin) < 0)
	newMin = d;
      if (d.compareTo(newMax) > 0)
	newMax = d;
    }

    // In log scale, round min and max to nearest powers of 10
    //
    if (this.scale == LOG_SCALE)
    {
      newMax = BigDecimal.valueOf(1).movePointRight
	(EngMath.log10ceiling(newMax));
      newMin = BigDecimal.valueOf(1).movePointRight
	(EngMath.log10floor(newMin));
    }

    // set min and max (using the private methods that don't silently
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
   * rescaledMax.  It is perfectly okay to specify a target interval
   * where rescaledMin > rescaledMax.
   *
   * If Min and Max are currently equal, returns Integer.MAX_VALUE,
   * Integer.MIN_VALUE, or (rescaledMin+rescaledMax)/2, depending on
   * whether dataValue is bigger, smaller, or also the same.
   */
  public int rescaleDataValue(BigDecimal dataValue,
			      int rescaledMin, int rescaledMax)
  {
    // special cases (to prevent dividing by zero later)
    if (this.min.compareTo(this.max) == 0) {
      switch(fixData(dataValue).compareTo(this.min))
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

    switch(this.scale)
    {
    case LINEAR_SCALE:
      // (dataValue - this.min) / (this.max - this.min)
      // * (rescaledMax - rescaledMin) + rescaledMin;
      return dataValue
	.subtract(this.min)
	.multiply(BigDecimal.valueOf(rescaledMax - rescaledMin))
	.divide(this.max.subtract(this.min), 0, BigDecimal.ROUND_HALF_UP)
	.intValue()
	+ rescaledMin;

    case LOG_SCALE:
      // log(dataValue / this.min) / log(this.max / this.min)
      // * (rescaledMax - rescaledMin) + rescaledMin;
      double d_val = fixData(dataValue).doubleValue();
      double d_min = this.min.doubleValue();
      double d_max = this.max.doubleValue();
      double d_result =
	Math.log(d_val / d_min) * (rescaledMax - rescaledMin)
	/ Math.log(d_max / d_min) + rescaledMin;

      return (new BigDecimal(d_result))
	.setScale(0, BigDecimal.ROUND_HALF_UP)
	.intValue();

    default:
      throw new Error("illegal scale: " + this.scale);
    }
  }



  /**
   * scaleToDataValue is the inverse of rescaleDataValue.
   *
   * Maps an integer value from the specified interval [scaledMin,
   * scaledMax] to a data value in the BigDecimal-valued interval
   * [Min, Max] associated with this axis.  Thus a value equal to
   * scaledMin is mapped to Min, a value equal to scaledMax is mapped
   * to Max, and values in between are scaled appropriately.  The
   * resulting data value is rounded to numsigfigs significant
   * figures.
   *
   * The scale of this axis is taken into account.  In LOG_SCALE, a
   * value halfway between scaledMin and scaledMax maps to the
   * data value whose logarithm is halfway between log(Min) and
   * log(Max).
   *
   * Values not between scaledMin and scaledMax can be scaled, but
   * note that in this case the resulting data value will not be
   * between Min and Max.
   *
   * This method makes no assumptions about scaledMin and
   * scaledMax.  It is perfectly okay to specify a target interval
   * where scaledMin > scaledMax.
   *
   * requires: scaledMin != scaledMax
   */
  public BigDecimal scaleToDataValue(int scaledValue,
				     int scaledMin, int scaledMax,
				     int numsigfigs)
  {
    if (scaledMin == scaledMax)
      throw new Error("failed requirement: scaledMin != scaledMax");
    if (this.min.compareTo(this.max) == 0)
      return this.min;

    double d_min = this.min.doubleValue();
    double d_max = this.max.doubleValue();
    double d_result;

    switch(this.scale)
    {
    case LINEAR_SCALE:
      // (scaledValue - scaledMin) / (scaledMax - scaledMin) *
      // (this.max - this.min) + this.min
      d_result = (scaledValue - scaledMin) * (d_max - d_min)
	/ (scaledMax - scaledMin) + d_min;
      break;

    case LOG_SCALE:
      // this.min * ((this.max / this.min)**
      // [(scaledValue - scaledMin) / (scaledMax - scaledMin)])
      d_result = d_min * Math.pow
	((d_max / d_min),
	 ((double)scaledValue - scaledMin) / (scaledMax - scaledMin));
      break;

    default:
      throw new Error("illegal scale: " + this.scale);
    }

    return EngMath.round(new BigDecimal(d_result), numsigfigs);
  }



  /**
   * Returns the size of each division if this axis is split into
   * nDivs divisions (note: in LOG_SCALE, the answer returned is in
   * decades).
   *
   * requires: nDivs > 0
   */
  public float getDivSize(int nDivs)
  {
    if (nDivs <= 0)
      throw new Error("illegal nDivs: " + nDivs);

    double d_max = this.max.doubleValue();
    double d_min = this.min.doubleValue();

    switch(this.scale)
    {
    case LINEAR_SCALE:
      return (float) ((d_max - d_min) / nDivs);

    case LOG_SCALE:
      return (float) (Math.log(d_max / d_min) / Math.log(10) / nDivs);

    default:
      throw new Error("illegal scale: " + this.scale);
    }
  }


  ////////////////////
  // Helper Methods //
  ////////////////////


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



  // "fixes" a data point if necessary for processing in log scale.
  private BigDecimal fixData(BigDecimal x)
  {
    if (this.scale == LOG_SCALE)
    {
      x = x.abs();
      if (x.compareTo(LOG_SCALE_MIN_VALUE) < 0)
	x = LOG_SCALE_MIN_VALUE;
    }
    return x;
  }

} // end class Axis
