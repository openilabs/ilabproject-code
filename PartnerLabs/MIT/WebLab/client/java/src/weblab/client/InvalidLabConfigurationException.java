/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

/**
 * Used to indicate an invalid XML Lab Configuration.
 */
public class InvalidLabConfigurationException extends Exception
{
  public InvalidLabConfigurationException(String mesg)
  {
    super(mesg);
  }

  public InvalidLabConfigurationException(Throwable cause)
  {
    super(cause);
  }
}
