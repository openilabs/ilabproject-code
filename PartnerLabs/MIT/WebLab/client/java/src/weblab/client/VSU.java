/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

/**
 * Represents a particular Voltage Source Unit (VSU) on the HP4155
 * analyzer.  A VSU has a VName and a source function.  A VSU also has
 * a port number that indicates which analyzer unit it corresponds to
 * (VSU1, VSU2, etc).
 */
public class VSU extends Port
{
  private int number;
  private String vName;
  private boolean vDownload;
  private SourceFunction function;

  /**
   * Creates a new VSU with default values.
   */
  public VSU(int number)
  {
    this.number = number;
    // pre-initialize things that might be null
    this.vName = "";
    this.function = new CONSFunction();
    // initialize everything here
    this.reset();
  }

  /**
   * Resets this VSU to default values.
   */
  public final void reset()
  {
    this.setVName("");
    this.setVDownload(false);
    this.setFunction(new CONSFunction());
  }
	
  public final int getNumber()
  {
    return this.number;
  }

  public final int getType()
  {
    return VSU_TYPE;
  }

  public final boolean isConfigured()
  {
    return (! this.matches(new VSU(this.number)));
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
   * Returns true iff the settings of this match the settings of
   * u. This is like an equals() method, but not quite because two
   * VSUs should not really be considered equal unless actually
   * identical.
   */
  public final boolean matches(VSU u)
  {
    return (u.number == number &&
	    u.vName.equals(vName) &&
	    u.vDownload == vDownload &&
	    u.function.equals(function));
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitVSU(this);
  }

} // end class VSU
