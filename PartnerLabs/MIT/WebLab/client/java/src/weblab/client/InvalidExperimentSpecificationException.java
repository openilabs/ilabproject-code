/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

/**
 * Used to indicate an invalid XML Experiment Specification.
 */
public class InvalidExperimentSpecificationException extends Exception
{
  public InvalidExperimentSpecificationException(String mesg)
  {
    super(mesg);
  }

  public InvalidExperimentSpecificationException(Throwable cause)
  {
    super(cause);
  }
}
