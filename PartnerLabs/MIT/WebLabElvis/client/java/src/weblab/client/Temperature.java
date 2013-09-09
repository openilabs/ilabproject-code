package weblab.client;

// JAVA 1.1 COMPLIANT

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
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitTemperature(this);
  }

} // end class Temperature
