/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.serverInterface;

import java.util.Date;

/**
 * A structure to hold Service-Broker-transparent information about an
 * Experiment.
 *
 * This class corresponds to the ExperimentInformation struct defined
 * in the iLab Service Broker Experiment Storage API, except that
 * several fields have been removed because their values are either
 * completely meaningless outside the Service Broker database (such as
 * an integer userID) or irrelevant to the client (such as
 * minTimeToLive).
 */
public class ExperimentInformation
{
  public Integer experimentID;
  public Integer labServerID;

  //public int userID;
  //public int effectiveGroupID;

  public Date submissionTime;
  public Date completionTime;

  //public Date expirationTime;
  //public double minTimeToLive;

  public int priorityHint;

  /**
   * one of the following constants from ExperimentStatus:
   * STATUS_WAITING, STATUS_RUNNING, STATUS_SUCCESS, STATUS_ERROR,
   * STATUS_CANCELLED, or STATUS_UNKNOWN
   */
  public int statusCode = ExperimentStatus.STATUS_UNKNOWN;

  public String[] validationWarningMessages;

  /** may be null */
  public String validationErrorMessage;

  public String[] executionWarningMessages;

  /** may be null */
  public String executionErrorMessage;

  /** may be an empty string "" */
  public String annotation;

  /** may be null */
  public String xmlResultExtension;

  /** may be null */
  public String xmlBlobExtension;

} // end class ExperimentInformation
