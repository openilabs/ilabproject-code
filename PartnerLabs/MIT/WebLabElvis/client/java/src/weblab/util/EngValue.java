/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.util;

import java.math.BigDecimal;

/**
 * Represents a number expressed in engineering notation: X.XXX *
 * 10**(3n), using unit prefixes to represent the order of magnitude
 * for any quantity that has units (e.g. 10**-3 V = 1 mV).
 *
 * EngValue is immutable.
 */
public class EngValue
{
  private BigDecimal engMantissa;
  private int engExponent;
  private EngUnits units;



  /**
   * Constructs a new EngValue with the specified value and units.
   */
  public EngValue(BigDecimal value, EngUnits units)
  {
    this.units = units;

    // exponent = largest n such that 10**n <= |value|
    int exponent = EngMath.log10floor(value);

    // engExponent = 3 * floor(exponent / 3), i.e. the largest 3n such
    // that 10**(3n) <= value
    this.engExponent = BigDecimal.valueOf(exponent)
      .divide(BigDecimal.valueOf(3), 0, BigDecimal.ROUND_FLOOR)
      .intValue()
      * 3;

    // set mantissa to the part of the value that's left over
    this.engMantissa = value.movePointLeft(this.engExponent);
  }



  /**
   * Constructs a new EngValue with the specified value and units,
   * rounded to numsigfigs significant figures.
   */
  public EngValue(BigDecimal value, EngUnits units, int numsigfigs)
  {
    // Round the value before dispatching to standard constructor in
    // case this affects the exponent.  Otherwise "999.99" rounded to
    // 4 sigfigs would end up with engExponent 0 instead of 3.
    this(EngMath.round(value, numsigfigs), units);

    // Now round the mantissa again to get rid of any extra trailing
    // zeros after the decimal point (due to the original rounded
    // value being greater than or equal to 10**numsigfigs).
    // Otherwise "87654321" rounded to 4 sigfigs would end up as
    // "87.650000e+6" instead of "87.65e+6".
    this.engMantissa = EngMath.round(this.engMantissa, numsigfigs);
  }



  /**
   * Returns the units of this.
   */
  public EngUnits getUnits()
  {
    return this.units;
  }



  /**
   * Returns the mantissa of this (the part of the value that's left
   * over after you divide by 10**engExponent).
   */
  public BigDecimal getEngMantissa()
  {
    return this.engMantissa;
  }



  /**
   * Returns the engineering notation exponent of this (the maximum 3n
   * such that 10**3n is less than or equal to the value of this).
   */
  public int getEngExponent()
  {
    return this.engExponent;
  }



  /**
   * Returns the entire numerical value of this as a BigDecimal.
   */
  public BigDecimal toBigDecimal()
  {
    return this.engMantissa.movePointRight(this.engExponent);
  }



  /**
   * Returns a string representation of this in engineering notation.
   */
  public String toString()
  {
    return
      this.getEngMantissa().toString() +
      this.getUnits().writeEngSuffixString(this.getEngExponent(), true);
  }

} // end class EngValue
