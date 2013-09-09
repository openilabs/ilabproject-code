package weblab.util;

// JAVA 1.1 COMPLIANT

import java.math.BigDecimal;

/**
 * Provides useful methods for doing math with BigDecimals.
 */
public class EngMath
{
  /**
   * Returns the largest n such that 10**n <= |x|
   *
   * If x is equal to zero, returns 0.
   */
  public static int log10floor(BigDecimal x)
  {
    switch(x.signum())
    {
    case 0:
      return 0;
    case -1:
      x = x.negate();
    }

    // minimimum value representable with scale s is 10**(-s)
    int n = - x.scale();
    BigDecimal one = BigDecimal.valueOf(1);

    // while 10**n <= x
    while (one.movePointRight(n).compareTo(x) <= 0)
      n++;

    return n-1;
  }



  /**
   * Returns the smallest n such that 10**n >= |x|
   *
   * If x is equal to zero, returns 0.
   */
  public static int log10ceiling(BigDecimal x)
  {
    switch(x.signum())
    {
    case 0:
      return 0;
    case -1:
      x = x.negate();
    }

    // minimimum value representable with scale s is 10**(-s)
    int n = - x.scale();
    BigDecimal one = BigDecimal.valueOf(1);

    // while 10**n < x
    while (one.movePointRight(n).compareTo(x) < 0)
      n++;

    return n;
  }



  /**
   * Rounds x to numSigfigs significant figures.
   */
  public static BigDecimal round(BigDecimal x, int numSigfigs)
  {
    int exponent = log10floor(x);

    BigDecimal roundedMantissa = x
      .movePointLeft(exponent)
      .setScale(numSigfigs - 1, BigDecimal.ROUND_HALF_UP);

    // If roundedMantissa is equal to 10 (i.e. because we just rounded
    // something of the form 9.9999...), then we now have two digits
    // before the decimal point instead of one, and therefore we have
    // an extra sigfig that we need to get rid of.
    if (roundedMantissa.compareTo(BigDecimal.valueOf(10)) == 0)
      roundedMantissa =
	roundedMantissa.setScale(numSigfigs - 2, BigDecimal.ROUND_HALF_UP);

    BigDecimal result = roundedMantissa.movePointRight(exponent);

    return result;
  }



  /**
   * Parses <code>text</code> as a decimal value and returns a
   * BigDecimal with that value.  Use this in place of the
   * BigDecimal(String) constructor in cases when <code>text</code>
   * might contain constructs like "E+000" that early implementations
   * of BigDecimal cannot handle.
   *
   * @throws NumberFormatException if the string cannot be parsed as a
   * decimal number.
   */
  public static BigDecimal parseBigDecimal(String text)
    throws NumberFormatException
  {
    // BigDecimal constructor can't handle leading '+'.  If one is
    // present, throw it away.
    if (text.charAt(0) == '+')
      text = text.substring(1);

    // find 'e' or, failing that, 'E'
    int ePosition = text.indexOf('e');
    if (ePosition == -1)
      ePosition = text.indexOf('E');

    // if we didn't find 'e' or 'E', it's now safe to construct the
    // BigDecimal
    if (ePosition == -1)
      return new BigDecimal(text);

    String exponentText = text.substring(ePosition + 1);

    // Integer.parseInt can't handle leading '+' either.  If one is
    // present, throw it away.
    if (text.charAt(ePosition + 1) == '+')
      exponentText = text.substring(ePosition + 2);
    else
      exponentText = text.substring(ePosition + 1);

    return new BigDecimal(text.substring(0, ePosition))
      .movePointRight(Integer.parseInt(exponentText));
  }

} // end class EngMath
