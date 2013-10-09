/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.xml;

/**
 * Used to indicate invalid or malformed XML.
 */
public class InvalidXMLException extends Exception
{
  public InvalidXMLException(String mesg)
  {
    super(mesg);
  }
}
