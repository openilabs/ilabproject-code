package weblab.client;

// JAVA 1.1 COMPATIBLE

import java.math.BigDecimal;

/**
 * Represents a single terminal of an experiment Setup.
 * A Terminal has an instrumentType (one of Instrument.FGEN_TYPE or instrument.SCOPE_TYPE) 
 * and an instrumentNumber, a label, an xPixelLocation and yPixelLocation to
 * identify where the terminal is located in the setup image, and a
 * maxVoltage, maxCurrent and maxFrequency indicating the maximum values that should
 * be applied to this terminal.
 *
 * Terminal is immutable.
 */
public class Terminal
{
  private final int instrumentType; // Instrument.FGEN_TYPE, Instrument.SCOPE_TYPE
  private final int instrumentNumber;
  private final String label;
  private final int xPixelLocation;
  private final int yPixelLocation;
  

  /**
   * Constructs a new Terminal with the specified values.
   */
  public Terminal(int instrumentType, int instrumentNumber, String label,
		  int xPixelLocation, int yPixelLocation)
  {
    this.instrumentType = instrumentType;
    this.instrumentNumber = instrumentNumber;
    this.label = label;
    this.xPixelLocation = xPixelLocation;
    this.yPixelLocation = yPixelLocation;
 
  }

  /**
   * Returns the type of the instrument connected to this
   * (Instrument.FGEN_TYPE or Instrument.SCOPE_TYPE).
   */
  public final int getInstrumentType()
  {
    return instrumentType;
  }

  /**
   * Returns the number of the instrument connected to this.
   */
  public final int getInstrumentNumber()
  {
    return instrumentNumber;
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
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitTerminal(this);
  }

} // end class Terminal
