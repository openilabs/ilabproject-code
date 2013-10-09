/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.xml;

/**
 * Used to indicate that a SOAP RPC request resulted in a SOAPFault.
 */
public class SOAPFaultException extends Exception
{
  public SOAPFaultException(String mesg)
  {
    super(mesg);
  }
}
