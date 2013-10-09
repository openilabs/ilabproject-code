/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

/**
 * Abstract superclass for any source function that can be assigned to
 * an SMU or VSU.
 *
 * All subclasses of SourceFunction should be immutable.
 */
public abstract class SourceFunction
{
  public static final int CONS_TYPE = 1;
  public static final int VAR1_TYPE = 2;
  public static final int VAR2_TYPE = 3;
  public static final int VAR1P_TYPE = 4;

  /**
   * Returns the function type of this (CONS_TYPE, VAR1_TYPE,
   * VAR2_TYPE, or VAR1P_TYPE).
   */
  public abstract int getType();

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public abstract void accept(Visitor v);

} // end class SourceFunction
