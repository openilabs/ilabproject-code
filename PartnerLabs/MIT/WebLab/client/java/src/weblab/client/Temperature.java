/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.math.BigDecimal;

/**
 * Represents a temperature.  A Temperature has a value and units.
 * 
 * Temperature is immutable.
 */
public class Temperature
{
  private final BigDecimal value;
  private final String units;

  /**
   * Constructs a new Temperature with the specified values.
   */
  public Temperature(BigDecimal value, String units)
  {
    this.value = value;
    this.units = units;
  }

  /**
   * Returns the value of this.
   */
  public final BigDecimal getValue()
  {
    return value;
  }

  /**
   * Returns the units of this.
   */
  public final String getUnits()
  {
    return units;
  }

  /**
   * Compares the specified object with this Temperature for equality.
   * Returns true iff the specified object is also a Temperature and
   * both Temperatures have equal values and units.
   *
   * note: BigDecimal comparison is performed using
   * BigDecimal.compareTo, not BigDecimal.equals.
   */
  public final boolean equals(Object obj)
  {
    // check object type
    if (! (obj instanceof Temperature))
      return false;
    Temperature t = (Temperature) obj;

    // check all fields
    return (this.value.compareTo(t.value) == 0 &&
	    this.units.equals(t.units));
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitTemperature(this);
  }

} // end class Temperature
