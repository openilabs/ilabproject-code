/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.util;

/**
 * A helper object used by EngValue and EngValueField to represent the
 * units associated with numerical data values.  This class
 * automatically provides appropriate unit prefix strings (such as
 * "m", "u", "n", "p") to represent order of magnitude.
 *
 * An instance of EngUnits generally corresponds to a particular
 * semantic category of data values (e.g. voltages, currents,
 * frequencies).  In addition to a symbol naming the units (which can
 * be null to indicate a dimensionless quantity), it keeps track of
 * the maximum and minimum exponent values that should be conveniently
 * represented using engineering prefix strings for this type of
 * numerical data.  For example, most applications would not want to
 * make it convenient for the user to input current values with
 * exponents higher than 0, or frequency values with exponents lower
 * than 0.
 *
 * Constraints: the max and min exponent values must be integer
 * multiples of 3, and min must be less than or equal to max
 * (i.e. there must be at least one exponent in the acceptable range).
 *
 * EngUnits is immutable.
 */
public class EngUnits
{
  private String unitSymbol;
  private int max;
  private int min;


  /**
   * Constructs a new EngUnits for dimensionless quantities (i.e. with
   * a null unitSymbol), using default max and min exponent values
   * which span the entire range of exponents for which engineering
   * prefixes are available.
   */
  public EngUnits()
  {
    this(null);
  }



  /**
   * Constructs a new EngUnits with the specified unit symbol, using
   * default max and min exponent values which span the entire range
   * of exponents for which engineering prefixes are available.
   */
  public EngUnits(String unitSymbol)
  {
    this(unitSymbol, 12, -15);
  }



  /**
   * Constructs a new EngUnits with the specified parameters.  If a
   * specified maxExponent and/or minExponent is not an integer
   * multiple of 3, it will be rounded (toward zero) to the next
   * multiple of 3.  If minExponent is larger than maxExponent, both
   * parameters will be set to the value of maxExponent.
   *
   * @param unitSymbol a symbol naming the units, or <code>null</code>
   * to indicate a dimensionless quantity
   * @param maxExponent the maximum exponent that should be
   * represented using an engineering prefix string (must be a
   * multiple of 3)
   * @param minExponent the minimum exponent that should be
   * represented using an engineering prefix string (must be a
   * multiple of 3, and less than or equal to maxExponent)
   */
  public EngUnits(String unitSymbol, int maxExponent, int minExponent)
  {
    this.unitSymbol = unitSymbol;

    // note: since Java division truncates, the remainder from a
    // negative dividend will be negative, e.g. -5 % 3 gives -2 not
    // +1.  Therefore subtracting the remainder will always make max
    // and min into multiples of 3 (by rounding toward zero).
    this.max = maxExponent - (maxExponent % 3);
    this.min = minExponent - (minExponent % 3);

    if (this.min > this.max)
      this.min = this.max;
  }



  /**
   * Returns the symbol for the units this represents (null if this
   * represents dimensionless quantities).
   */
  public String getUnitSymbol()
  {
    return this.unitSymbol;
  }



  /**
   * Returns the maximum exponent that should be expressed using an
   * engineering prefix.
   */
  public int getMaxExponent()
  {
    return max;
  }



  /**
   * Returns the minimum exponent that should be expressed using an
   * engineering prefix.
   */
  public int getMinExponent()
  {
    return min;
  }



  /**
   * Returns the suffix string that would be used to write the
   * specified exponent with these units in engineering notation.  The
   * suffix string includes the order of magnitude (represented using
   * a prefix character if the exponent is within the acceptable range
   * for representation in this way, or using E notation otherwise)
   * and the unit symbol.  For dimensionless quantities, the suffix
   * string will consist solely of E notation e.g. "e+6" (unless the
   * exponent is zero, in which case an empty suffix "" is returned).
   *
   * If precedingSpace is true, suffix strings that do NOT involve E
   * notation will include a leading space character " " so that the
   * output will be suitable for concatenating onto the end of
   * BigDecimal.toString().
   */
  public String writeEngSuffixString(int exponent, boolean precedingSpace)
  {
    String eNotation = "e" + (exponent >= 0 ? "+" : "") + exponent;

    if (unitSymbol == null)
    {
      if (exponent == 0)
	return ""; // don't suffix unitless quantites with "e+0"
      else
	return eNotation;
    }
    else
    {
      String prefix = this.getUnitPrefix(exponent);

      if (prefix == null)
	return eNotation + " " + unitSymbol;

      return (precedingSpace ? " " : "") + prefix + unitSymbol;
    }
  }



  /**
   * Returns the prefix that should be used to express the specified
   * exponent (an exponent of zero is represented using the empty
   * prefix "").  If the exponent is outside of the acceptable range
   * (i.e. bigger than max or smaller than min), or if no prefix is
   * available for the specified exponent, returns null.
   */
  protected String getUnitPrefix(int exponent)
  {
    if (exponent > this.getMaxExponent() ||
	exponent < this.getMinExponent())
      return null;

    switch(exponent) {
    case 12:
      return "T";
    case 9:
      return "G";
    case 6:
      return "M";
    case 3:
      return "k";
    case 0:
      return "";
    case -3:
      return "m";
    case -6:
      return "u";
    case -9:
      return "n";
    case -12:
      return "p";
    case -15:
      return "f";
    default:
      // our exponent doesn't match any of the standard prefixes
      return null;
    }
  }



  // two EngUnits are equal if they have equal unit name, max, and min.
  public final boolean equals(Object obj)
  {
    if (obj instanceof EngUnits)
    {
      EngUnits x = (EngUnits) obj;

      if ((x.unitSymbol == null && this.unitSymbol != null) ||
	  (x.unitSymbol != null && !x.unitSymbol.equals(this.unitSymbol)))
	return false;

      if (x.max != this.max || x.min != this.min)
	return false;

      return true;
    }
    else
      return false;
  }

} // end class EngUnits
