/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.math.BigDecimal;

/**
 * Represents a single terminal of a microelectronics device connected
 * to the analyzer.  A Terminal has a portType (one of Port.SMU_TYPE,
 * Port.VSU_TYPE, or Port.VMU_TYPE) and portNumber to indicate which
 * analyzer port it is physically connected to, a label, an
 * xPixelLocation and yPixelLocation to identify where the terminal is
 * located in the device image, and a maxVoltage and maxCurrent
 * indicating the maximum values that should be applied to this
 * terminal.
 *
 * Terminal is immutable.
 */
public class Terminal
{
  private final int portType; // Port.SMU_TYPE, Port.VSU_TYPE, or Port.VMU_TYPE
  private final int portNumber;
  private final String label;
  private final int xPixelLocation;
  private final int yPixelLocation;
  private final BigDecimal maxVoltage;
  private final BigDecimal maxCurrent;

  /**
   * Constructs a new Terminal with the specified values.
   */
  public Terminal(int portType, int portNumber, String label,
		  int xPixelLocation, int yPixelLocation,
		  BigDecimal maxVoltage, BigDecimal maxCurrent)
  {
    this.portType = portType;
    this.portNumber = portNumber;
    this.label = label;
    this.xPixelLocation = xPixelLocation;
    this.yPixelLocation = yPixelLocation;
    this.maxVoltage = maxVoltage;
    this.maxCurrent = maxCurrent;
  }

  /**
   * Returns the type of the analyzer port connected to this
   * (Port.SMU_TYPE, Port.VSU_TYPE, or Port.VMU_TYPE).
   */
  public final int getPortType()
  {
    return portType;
  }

  /**
   * Returns the number of the analyzer port connected to this.
   */
  public final int getPortNumber()
  {
    return portNumber;
  }

  /**
   * Returns the label for this.
   */
  public final String getLabel()
  {
    return label;
  }

  /**
   * Returns the x pixel location of this in the device image.
   */
  public final int getXPixelLocation()
  {
    return xPixelLocation;
  }

  /**
   * Returns the y pixel location of this in the device image.
   */
  public final int getYPixelLocation()
  {
    return yPixelLocation;
  }

  /**
   * Returns the maximum voltage that should be applied to this
   * terminal.
   */
  public final BigDecimal getMaxVoltage()
  {
    return maxVoltage;
  }

  /**
   * Returns the maximum current that should be applied to this
   * terminal.
   */
  public final BigDecimal getMaxCurrent()
  {
    return maxCurrent;
  }

  /**
   * Compares the specified object with this Terminal for equality.
   * Returns true iff the specified object is also a Terminal and both
   * Terminals have equal values for all fields.
   *
   * note: BigDecimal comparison is performed using
   * BigDecimal.compareTo, not BigDecimal.equals.
   */
  public boolean equals(Object obj)
  {
    // check object type
    if (! (obj instanceof Terminal))
      return false;
    Terminal t = (Terminal) obj;

    // check all fields
    return (this.portType == t.portType &&
	    this.portNumber == t.portNumber &&
	    this.label.equals(t.label) &&
	    this.xPixelLocation == t.xPixelLocation &&
	    this.yPixelLocation == t.yPixelLocation &&
	    this.maxVoltage.compareTo(t.maxVoltage) == 0 &&
	    this.maxCurrent.compareTo(t.maxCurrent) == 0);
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitTerminal(this);
  }

} // end class Terminal
