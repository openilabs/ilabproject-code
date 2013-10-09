/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.serverInterface;

/**
 * Used to indicate a problem invoking a Server method.
 */
public class ServerException extends Exception
{
  public ServerException(String mesg)
  {
    super(mesg);
  }

  public ServerException(String mesg, Throwable cause)
  {
    super(mesg, cause);
  }
}
