/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.util;

import java.math.BigDecimal;
import java.math.BigInteger;

/**
 * Provides useful methods for doing math with BigDecimals.
 */
public class EngMath
{
  /**
   * Exponent of the smallest acceptable magnitude for a BigDecimal in
   * scientific "E" notation.  Smaller values will be silently
   * replaced with 1e[MIN_BIG_DECIMAL_EXPONENT] to prevent massively
   * overloading the client machine's CPU with BigDecimal
   * computations.
   */
  public static final int MIN_BIGDECIMAL_EXPONENT = -999;

  /**
   * Exponent of the largest acceptable magnitude for a BigDecimal in
   * scientific "E" notation.  Larger values will be silently replaced
   * with 1e[MAX_BIGDECIMAL_EXPONENT] to prevent massively overloading
   * the client machine's CPU with BigDecimal computations.
   */
  public static final int MAX_BIGDECIMAL_EXPONENT = 999;



  /**
   * Returns the largest n such that 10**n <= |x|.  Note that this
   * method is exact and does not suffer from floating-point rounding
   * errors.
   *
   * If x is equal to zero, returns 0.
   *
   * @throws Error if the answer is greater than or equal to
   * Integer.MAX_VALUE
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
    {
      if (n == Integer.MAX_VALUE)
	throw new Error
	  ("log10floor does not fit in an int for value: " + x.toString());
      n++;
    }

    return n-1;
  }



  /**
   * Returns the smallest n such that 10**n >= |x|.  Note that this
   * method is exact and does not suffer from floating-point rounding
   * errors.
   *
   * If x is equal to zero, returns 0.
   *
   * @throws Error if the answer is greater than Integer.MAX_VALUE
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
    {
      if (n == Integer.MAX_VALUE)
	throw new Error
	  ("log10ceiling does not fit in an int for value: " + x.toString());
      n++;
    }

    return n;
  }



  /**
   * Returns the natural log of x, calculated to double-precision
   * using <code>Math.log()</code>.  However, this method is capable
   * of handling values larger than Double.MAX_VALUE, so long as the
   * exponent when x is written in scientific notation fits
   * comfortably within an integer.
   *
   * requires: x > 0
   */
  public static BigDecimal log(BigDecimal x)
  {
    if (x.signum() != 1)
      throw new Error("tried to compute log of zero or a negative number: "
		      + x.toString());

    if (x.compareTo(new BigDecimal(Double.MAX_VALUE)) <= 0)
    {
      // If x fits in a double, the naive approach is fine.
      return new BigDecimal(Math.log(x.doubleValue()));
    }
    else
    {
      // If x is too large to fit in a double, let x = m * 10**n.
      // Then log x = log m + n log 10
      int n = log10floor(x);
      double m = x.movePointLeft(n).doubleValue();
      return new BigDecimal(Math.log(m)).add
	(BigDecimal.valueOf(n).multiply(new BigDecimal(Math.log(10))));
    }
  }



  /**
   * Returns the base 10 logarithm of x, calculated to
   * double-precision using <code>Math.log()</code>.  However, this
   * method is capable of handling values larger than
   * Double.MAX_VALUE, so long as the exponent when x is written in
   * scientific notation fits comfortably within an integer.
   *
   * requires: x > 0
   */
  public static BigDecimal log10(BigDecimal x)
  {
    if (x.signum() != 1)
      throw new Error("tried to compute log10 of zero or a negative number: "
		      + x.toString());

    if (x.compareTo(new BigDecimal(Double.MAX_VALUE)) <= 0)
    {
      // If x fits in a double, the naive approach is fine.
      return new BigDecimal(Math.log(x.doubleValue())/Math.log(10.0));
    }
    else
    {
      // If x is too large to fit in a double, let x = m * 10**n.
      // Then log10(x) = log10(m) + n
      int n = log10floor(x);
      double m = x.movePointLeft(n).doubleValue();
      return new BigDecimal(Math.log(m)/Math.log(10.0))
	.add(BigDecimal.valueOf(n));
    }
  }



  /**
   * Returns the value of 10 raised to the power of x, calculated to
   * double-precision using <code>Math.pow()</code>.  However, this
   * method is capable of handling arbitrarily large values of x.
   */
  public static BigDecimal pow10(BigDecimal x)
  {
    BigDecimal result;

    // for convenience
    BigInteger MAXINT = BigInteger.valueOf(Integer.MAX_VALUE);

    // let x = n + m where n is a possibly large, possibly negative
    // integer and m is a small, possibly negative decimal number.
    // Then result = 10**m * 10**n

    BigInteger n = x.toBigInteger(); // truncates fractional part
    BigDecimal m = x.subtract(new BigDecimal(n));

    // compute 10**m in double precision using Math.pow (unless m is
    // zero, in which case be sure to get 1 precisely)
    if (m.signum() == 0)
      result = BigDecimal.valueOf(1);
    else
      result = new BigDecimal(Math.pow(10.0, m.doubleValue()));

    // multiply by 10**n by shifting decimal point to the right n
    // times, taking into account that n might be too big to fit in an
    // int all at once (but it probably won't be, so we don't care too
    // much about bad performance in that case)
    while (n.compareTo(MAXINT) > 0)
    {
      result = result.movePointRight(Integer.MAX_VALUE);
      n = n.subtract(MAXINT);
    }
    result = result.movePointRight(n.intValue());

    return result;
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
   * If the magnitude of a number written in scientific "E" notation
   * is larger than 1e[MAX_BIGDECIMAL_EXPONENT] or smaller than
   * 1e[MIN_BIGDECIMAL_EXPONENT], this method returns the appropriate
   * boundary value instead of the true value.
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
    // entire BigDecimal
    if (ePosition == -1)
      return new BigDecimal(text);

    // otherwise just parse the mantissa for now
    BigDecimal mantissa = new BigDecimal(text.substring(0, ePosition));


    String exponentText = text.substring(ePosition + 1);

    // BigInteger constructor can't handle leading '+' either.  If one
    // is present, throw it away.
    if (text.charAt(ePosition + 1) == '+')
      exponentText = text.substring(ePosition + 2);
    else
      exponentText = text.substring(ePosition + 1);

    // parse the exponent (use a BigInteger until we've range-checked
    // it, just to be safe)
    BigInteger exponent = new BigInteger(exponentText);


    // ensure that the overall magnitude will be within acceptable
    // limits (i.e. between 1e[MIN] and 1e[MAX]).
    BigInteger magnitudeFloor = exponent.add
      (BigInteger.valueOf(EngMath.log10floor(mantissa)));
    if (magnitudeFloor.compareTo
	(BigInteger.valueOf(MIN_BIGDECIMAL_EXPONENT))
	< 0)
    {
      // magnitude is too small.  Return +-1e[MIN] instead.
      return BigDecimal.valueOf(mantissa.signum())
	.movePointRight(MIN_BIGDECIMAL_EXPONENT);
    }
    else if (magnitudeFloor.compareTo
	     (BigInteger.valueOf(MAX_BIGDECIMAL_EXPONENT))
	     >= 0)
    {
      // magnitude is too big (or exactly on the borderline).  Return
      // +-1e[MAX] instead.
      return BigDecimal.valueOf(mantissa.signum())
	.movePointRight(MAX_BIGDECIMAL_EXPONENT);
    }


    // if we make it here, we know it's okay to return the true value
    return mantissa.movePointRight(exponent.intValue());
  }

} // end class EngMath
