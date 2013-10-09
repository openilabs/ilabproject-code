/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.graphing;

/**
 * Used to indicate an inconsistency in units.
 */
public class InconsistentUnitsException extends Exception
{
  public InconsistentUnitsException()
  {
    super();
  }

  public InconsistentUnitsException(String mesg)
  {
    super(mesg);
  }
}
