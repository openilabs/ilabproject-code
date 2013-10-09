/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.math.BigDecimal;

/**
 * Represents a particular Source/Monitor Unit (SMU) on the HP4155
 * analyzer.  An SMU has a VName, IName, mode (one of V_MODE, I_MODE,
 * and COMM_MODE), a source function (not applicable in COMM_MODE),
 * and a compliance value (also not applicable in COMM_MODE).
 *
 * Note: actually, the SMU maintains separate values for the current
 * compliance (used in V_MODE) and voltage compliance (used in
 * I_MODE), so that the two can vary independently.  The getCompliance
 * and setCompliance methods automatically apply to the compliance
 * value associated with the SMU's current mode.
 *
 * An SMU also has a port number that indicates which analyzer unit it
 * corresponds to (SMU1, SMU2, etc).
 */
public class SMU extends Port
{
  // modes
  public static final int V_MODE = 1;
  public static final int I_MODE = 2;
  public static final int COMM_MODE = 3;


  private int number;
  private String vName, iName;
  private boolean vDownload, iDownload;
  private int mode;
  private SourceFunction function;
  private BigDecimal iCompliance, vCompliance;

  /**
   * Creates a new SMU with default values.
   */
  public SMU(int number)
  {
    this.number = number;
    // pre-initialize things that might be null
    this.vName = "";
    this.iName = "";
    this.function = new CONSFunction();
    this.iCompliance = BigDecimal.valueOf(0);
    this.vCompliance = BigDecimal.valueOf(0);
    // initialize everything here
    this.reset();
  }

  /**
   * Resets this SMU to default values.
   */
  public final void reset()
  {
    this.setVName("");
    this.setIName("");
    this.setVDownload(false);
    this.setIDownload(false);
    this.setMode(V_MODE);
    this.setFunction(new CONSFunction());
    this.setICompliance(new BigDecimal("0.1"));
    this.setVCompliance(new BigDecimal("10"));
  }

  public final int getNumber()
  {
    return this.number;
  }

  public final int getType()
  {
    return SMU_TYPE;
  }

  public final boolean isConfigured()
  {
    // an SMU has been configured if it doesn't match the settings of
    // a newly created SMU with the same number connected to the same
    // Terminal (the Terminal part is important because of
    // compliance).
    SMU defaultSMU = new SMU(this.number);
    defaultSMU.setTerminal(this.getTerminal());
    return (! this.matches(defaultSMU));
  }

  /**
   * Returns the name of the voltage variable for this unit.
   */
  public final String getVName()
  {
    return this.vName;
  }

  /**
   * Sets the name of the voltage variable for this unit.
   *
   * @param name the new VName for this
   */
  public final void setVName(String name)
  {
    if (! this.vName.equals(name))
    {
      this.vName = name;
      this.setChanged();
    }
  }

  /**
   * Returns whether the voltage variable for this unit will be
   * downloaded.
   */
  public final boolean getVDownload()
  {
    return this.vDownload;
  }

  /**
   * Sets whether the voltage variable for this unit will be
   * downloaded.
   *
   * @param download true to download the voltage variable
   */
  public final void setVDownload(boolean download)
  {
    if (this.vDownload != download)
    {
      this.vDownload = download;
      this.setChanged();
    }
  }

  /**
   * Returns the name of the current variable for this unit.
   */
  public final String getIName()
  {
    return this.iName;
  }

  /**
   * Sets the name of the current variable for this unit.
   *
   * @param name the new IName for this
   */
  public final void setIName(String name)
  {
    if (! this.iName.equals(name))
    {
      this.iName = name;
      this.setChanged();
    }
  }

  /**
   * Returns whether the current variable for this unit will be
   * downloaded.
   */
  public final boolean getIDownload()
  {
    return this.iDownload;
  }

  /**
   * Sets whether the current variable for this unit will be
   * downloaded.
   *
   * @param download true to download the current variable
   */
  public final void setIDownload(boolean download)
  {
    if (this.iDownload != download)
    {
      this.iDownload = download;
      this.setChanged();
    }
  }

  /**
   * Returns the mode of this (V_MODE, I_MODE, or COMM_MODE).
   */
  public final int getMode()
  {
    return this.mode;
  }

  /**
   * Sets the mode of this.
   *
   * requires: mode is one of V_MODE, I_MODE, or COMM_MODE
   *
   * @param mode the new mode for this.
   */
  public final void setMode(int mode)
  {
    if (this.mode != mode)
    {
      this.mode = mode;
      this.setChanged();
    }
  }

  /**
   * Returns the source function of this.
   */
  public final SourceFunction getFunction()
  {
    return this.function;
  }

  /**
   * Sets the source function of this.
   *
   * @param function the new source function for this.
   */
  public final void setFunction(SourceFunction function)
  {
    if (! this.function.equals(function))
    {
      this.function = function;
      this.setChanged();
    }
  }

  /**
   * Returns the active compliance value of this (the current
   * compliance if this is in V_MODE, or the voltage compliance if
   * this is in I_MODE).  If this is in COMM_MODE, returns null.
   */
  public final BigDecimal getCompliance()
  {
    switch (this.mode) {
    case V_MODE:
      return this.getICompliance();
    case I_MODE:
      return this.getVCompliance();
    case COMM_MODE:
      return null;
    default:
      throw new Error("illegal mode: " + this.mode);
    }
  }

  /**
   * Gets the current compliance value of this (note: current
   * compliance is only relevant if this is in V_MODE).
   */
  public BigDecimal getICompliance()
  {
    return this.iCompliance;
  }

  /**
   * Gets the voltage compliance value of this (note: voltage
   * compliance is only relevant if this is in I_MODE).
   */
  public BigDecimal getVCompliance()
  {
    return this.vCompliance;
  }

  /**
   * Sets the active compliance value of this (the current compliance
   * if this is in V_MODE, or the voltage compliance if this is in
   * I_MODE).  If this is in COMM_MODE, does nothing.
   *
   * @param compliance the new compliance value for this.
   */
  public final void setCompliance(BigDecimal compliance)
  {
    switch (this.mode) {
    case V_MODE:
      this.setICompliance(compliance);
      break;
    case I_MODE:
      this.setVCompliance(compliance);
      break;
    case COMM_MODE:
      // do nothing
      break;
    default:
      throw new Error("illegal mode: " + this.mode);
    }
  }

  /**
   * Sets the current compliance value of this (note: current
   * compliance is only relevant if this is in V_MODE).
   *
   * @param compliance the new current compliance value for this.
   */
  public void setICompliance(BigDecimal compliance)
  {
    if (! this.iCompliance.equals(compliance))
    {
      this.iCompliance = compliance;
      this.setChanged();
    }
  }

  /**
   * Sets the voltage compliance value of this (note: voltage
   * compliance is only relevant if this is in I_MODE).
   *
   * @param compliance the new voltage compliance value for this.
   */
  public void setVCompliance(BigDecimal compliance)
  {
    if (! this.vCompliance.equals(compliance))
    {
      this.vCompliance = compliance;
      this.setChanged();
    }
  }

  /**
   * Returns true iff the settings of this match the settings of
   * u. This is like an equals() method, but not quite because two
   * SMUs should not really be considered equal unless actually
   * identical.
   */
  public final boolean matches(SMU u)
  {
    return (u.number == number &&
	    u.vName.equals(vName) &&
	    u.iName.equals(iName) &&
	    u.vDownload == vDownload &&
	    u.iDownload == iDownload &&
	    u.mode == mode &&
	    u.function.equals(function) &&
	    u.vCompliance.compareTo(vCompliance) == 0 &&
	    u.iCompliance.compareTo(iCompliance) == 0);
  }

  /**
   * Sets the Terminal connected to this, and updates compliance
   * values of this to match the max voltage and current of the new
   * terminal.
   *
   * @param newTerminal the new Terminal connected to this, or null
   * for none.
   */
  public void setTerminal(Terminal newTerminal)
  {
    if (newTerminal != null && ! newTerminal.equals(this.getTerminal()))
    {
      // update compliance values to limits of new terminal
      this.setICompliance(newTerminal.getMaxCurrent());
      this.setVCompliance(newTerminal.getMaxVoltage());
    }

    super.setTerminal(newTerminal);
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitSMU(this);
  }

} // end class SMU
