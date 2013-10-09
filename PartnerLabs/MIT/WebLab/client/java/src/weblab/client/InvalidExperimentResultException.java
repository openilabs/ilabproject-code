/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

/**
 * Used to indicate an invalid XML Experiment Result.
 */
public class InvalidExperimentResultException extends Exception
{
  public InvalidExperimentResultException(String mesg)
  {
    super(mesg);
  }

  public InvalidExperimentResultException(Throwable cause)
  {
    super(cause);
  }
}
