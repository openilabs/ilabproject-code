package weblab.client;

// JAVA 1.1 COMPLIANT

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

  // two CONSFunctions are equal if they have equal values.
  public final boolean equals(Object obj)
  {
    if (obj instanceof CONSFunction)
    {
      CONSFunction f = (CONSFunction) obj;

      return (this.value.compareTo(f.value) == 0);
    }
    else
      return false;
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitCONSFunction(this);
  }

} // end class CONSFunction
