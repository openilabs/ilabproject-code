/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import weblab.toolkit.util.ChangeTrackingObservable;

/**
 * Abstract superclass for any of the ports on the HP4155 analyzer.
 */
public abstract class Port extends ChangeTrackingObservable
  implements Cloneable
{
  public static final int SMU_TYPE = 1;
  public static final int VSU_TYPE = 2;
  public static final int VMU_TYPE = 3;

  // the Terminal connected to this Port.  Can be null!  (since an
  // ExperimentSpecification need not always be connected to a Device)
  private Terminal myTerminal;

  /**
   * Factory method that creates a port with the given type and
   * number (and a null Terminal).
   */
  public static Port createPort(int type, int number)
  {
    switch(type)
    {
    case SMU_TYPE:
      return new SMU(number);
    case VSU_TYPE:
      return new VSU(number);
    case VMU_TYPE:
      return new VMU(number);
    default:
      throw new Error("illegal port type in Port.createPort");
    }
  }

  /**
   * Returns the port number of this.
   */
  public abstract int getNumber();

  /**
   * Returns the port type of this (SMU_TYPE, VSU_TYPE, or VMU_TYPE).
   */
  public abstract int getType();

  /**
   * Returns the name of this port.
   */
  public String getName()
  {
    switch(this.getType()) {
    case SMU_TYPE:
      return "SMU" + this.getNumber();
    case VSU_TYPE:
      return "VSU" + this.getNumber();
    case VMU_TYPE:
      return "VMU" + this.getNumber();
    default:
      throw new Error("illegal port type: " + this.getType());
    }
  }

  /**
   * Returns the Terminal connected to this (or null if none).
   */
  public Terminal getTerminal()
  {
    return myTerminal;
  }

  /**
   * Sets the Terminal connected to this.
   *
   * @param newTerminal the new Terminal connected to this, or null
   * for none.
   */
  public void setTerminal(Terminal newTerminal)
  {
    if (newTerminal == null)
    {
      if (myTerminal != null)
      {
	myTerminal = null;
	this.setChanged();
      }
    }
    else if (! newTerminal.equals(myTerminal))
    {
      myTerminal = newTerminal;
      this.setChanged();
    }
  }

  /**
   * Returns true iff this port has been configured (i.e. changed from
   * its default values).
   *
   * Note: this is meant to detect changes made by the user, so
   * setting a Terminal doesn't count.
   */
  public abstract boolean isConfigured();

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public abstract void accept(Visitor v);

  public Object clone()
  {
    try
    {
      Port clone = (Port) super.clone();
      clone.deleteObservers();
      return clone;
    }
    catch (CloneNotSupportedException ex)
    {
      throw new Error(ex);
    }
  }

} // end class Port
