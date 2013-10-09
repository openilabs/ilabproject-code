/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.serverInterface;

/**
 * A structure to hold information about the execution status of an
 * Experiment.
 *
 * This class corresponds to the ExperimentStatus struct defined in
 * the iLab Service Broker to Lab Server API.
 */
public class ExperimentStatus
{
  /**
   * Indicates that the experiment is waiting in the execution queue.
   */
  public static final int STATUS_WAITING = 1;

  /**
   * Indicates that the experiment is currently running.
   */
  public static final int STATUS_RUNNING = 2;

  /**
   * Indicates that the experiment has terminated normally.
   */
  public static final int STATUS_SUCCESS = 3;

  /**
   * Indicates that the experiment has terminated with errors (this
   * includes cancellation by user in mid-execution).
   */
  public static final int STATUS_ERROR = 4;

  /**
   * Indicates that the experiment was cancelled by the user before
   * execution had begun.
   */
  public static final int STATUS_CANCELLED = 5;

  /**
   * Indicates that status was requested for an unknown experimentID
   */
  public static final int STATUS_UNKNOWN = 6;


  /**
   * one of STATUS_WAITING, STATUS_RUNNING, STATUS_SUCCESS,
   * STATUS_ERROR, STATUS_CANCELLED, or STATUS_UNKNOWN
   */
  public int statusCode = STATUS_UNKNOWN;

  /**
   * Number of experiments currently in the execution queue that will
   * run before this experiment.
   *
   * This field is only relevant if statusCode == STATUS_WAITING
   */
  public int effectiveQueueLength;

  /**
   * [OPTIONAL, < 0 if not implemented by Lab Server] estimated wait
   * (in seconds) until this experiment will begin running, based on
   * the other experiments currently in the execution queue.
   *
   * This field is only relevant if statusCode == STATUS_WAITING
   */
  public double estWait;

  /**
   * [OPTIONAL, < 0 if not implemented by Lab Server] estimated
   * runtime (in seconds) of this experiment once it begins running.
   *
   * This field is only relevant if statusCode == STATUS_WAITING
   */
  public double estRuntime;

  /**
   * [OPTIONAL, < 0 if not implemented by Lab Server] estimated
   * remaining runtime (in seconds) of this experiment, if the
   * experiment is currently running.
   *
   * This field is only relevant if statusCode == STATUS_RUNNING
   */
  public double estRemainingRuntime;


} // end class ExperimentStatus
