/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

/**
 * Represents a particular Voltage Monitor Unit (VMU) on the HP4155
 * analyzer.  A VMU has a VName and a mode (which must be V_MODE
 * because weblab doesn't currently support any other modes for VMUs).
 * A VMU also has a port number that indicates which analyzer unit it
 * corresponds to (VMU1, VMU2, etc).
 */
public class VMU extends Port
{
  // modes
  public static final int V_MODE = 1;


  private int number;
  private String vName;
  private boolean vDownload;
  private int mode;

  /**
   * Creates a new VMU with default values.
   */
  public VMU(int number)
  {
    this.number = number;
    // pre-initialize things that might be null
    this.vName = "";
    // initialize everything here
    this.reset();
  }
  
  /**
   * Resets this VMU to default values.
   */
  public final void reset()
  {
    this.setVName("");
    this.setVDownload(false);
    this.setMode(V_MODE);
  }
	
  public final int getNumber()
  {
    return this.number;
  }

  public final int getType()
  {
    return VMU_TYPE;
  }

  public final boolean isConfigured()
  {
    return (! this.matches(new VMU(this.number)));
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
   * Returns the mode of this (V_MODE).
   */
  public final int getMode()
  {
    return this.mode;
  }

  /**
   * Sets the mode of this.
   *
   * requires: mode is V_MODE
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
   * Returns true iff the settings of this match the settings of
   * u. This is like an equals() method, but not quite because two
   * VMUs should not really be considered equal unless actually
   * identical.
   */
  public final boolean matches(VMU u)
  {
    return (u.number == number &&
	    u.vName.equals(vName) &&
	    u.vDownload == vDownload &&
	    u.mode == mode);
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitVMU(this);
  }

} // end class VMU
