/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.math.BigDecimal;

/**
 * Represents VAR1P, the synchronous sweep source.  VAR1PFunction
 * consists of an offset and a ratio; the value of VAR1P at each point
 * is determined by the relation:
 *
 * VAR1P = VAR1 * ratio + offset
 *
 * VAR1PFunction is immutable.
 */

public class VAR1PFunction extends SourceFunction
{
  private BigDecimal ratio;
  private BigDecimal offset;

  /**
   * Creates a new VAR1PFunction with default values.
   */
  public VAR1PFunction()
  {
    this(BigDecimal.valueOf(0), BigDecimal.valueOf(0));
  }

  /**
   * Creates a new VAR1PFunction with the specified values.
   */
  public VAR1PFunction(BigDecimal ratio, BigDecimal offset)
  {
    this.ratio = ratio;
    this.offset = offset;
  }

  public final int getType()
  {
    return VAR1P_TYPE;
  }

  /**
   * Returns the ratio value of this.
   */
  public final BigDecimal getRatio()
  {
    return this.ratio;
  }

  /**
   * Returns the offset value of this.
   */
  public final BigDecimal getOffset()
  {
    return this.offset;
  }

  /**
   * Compares the specified object with this VAR1PFunction for
   * equality.  Returns true iff the specified object is also a
   * VAR1PFunction and both VAR1PFunctions have equal ratio and offset
   * values.
   *
   * note: BigDecimal comparison is performed using
   * BigDecimal.compareTo, not BigDecimal.equals.
   */
  public final boolean equals(Object obj)
  {
    if (! (obj instanceof VAR1PFunction))
      return false;
    VAR1PFunction f = (VAR1PFunction) obj;

    return (this.ratio.compareTo(f.ratio) == 0 &&
	    this.offset.compareTo(f.offset) == 0);
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitVAR1PFunction(this);
  }

} // end class VAR1PFunction
