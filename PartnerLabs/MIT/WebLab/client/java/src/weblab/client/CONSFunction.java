/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.math.BigDecimal;

/**
 * Represents a constant-valued source function.  A CONSFunction
 * consists of a single value.
 *
 * CONSFunction is immutable.
 */

public class CONSFunction extends SourceFunction
{
  private BigDecimal value;

  /**
   * Creates a new CONSFunction with default value.
   */
  public CONSFunction()
  {
    this(BigDecimal.valueOf(0));
  }

  /**
   * Creates a new CONSFunction with the specified value.
   */
  public CONSFunction(BigDecimal value)
  {
    this.value = value;
  }

  public final int getType()
  {
    return CONS_TYPE;
  }

  /**
   * Returns the value of this.
   */
  public final BigDecimal getValue()
  {
    return this.value;
  }

  /**
   * Compares the specified object with this CONSFunction for
   * equality.  Returns true iff the specified object is also a
   * CONSFunction and both CONSFunctions have equal values.
   *
   * note: BigDecimal comparison is performed using
   * BigDecimal.compareTo, not BigDecimal.equals.
   */
  public final boolean equals(Object obj)
  {
    // check object type
    if (! (obj instanceof CONSFunction))
      return false;
    CONSFunction f = (CONSFunction) obj;

    // check value
    return (this.value.compareTo(f.value) == 0);
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitCONSFunction(this);
  }

} // end class CONSFunction
