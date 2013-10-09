/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.serverInterface;

import java.util.List;

/**
 * A facade through which the rest of the client code performs all
 * server-related operations.
 */
public interface Server
{
  /**
   * Returns a URL that can be used to obtain more info about the lab.
   *
   * @throws ServerException if a problem occurs
   */
  public String getLabInfo()
    throws ServerException;

  /**
   * Retrieves the current lab configuration.
   *
   * @return the XML lab configuration string from the server
   * @throws ServerException if a problem occurs
   */
  public String getLabConfiguration()
    throws ServerException;

  /**
   * Submits an experiment specification for execution and returns the
   * experimentID.
   *
   * @param xmlExperimentSpecification the XML experiment
   * specification string to submit to the server
   * @return the experimentID assigned to this experiment by the server
   * @throws ServerException if a problem occurs
   */
  public int submit(String xmlExperimentSpecification)
    throws ServerException;

  /**
   * Submits an experiment specification for execution, and writes
   * status information from the server's submission report to an
   * ExperimentStatus object in addition to returning the
   * experimentID.
   *
   * @param xmlExperimentSpecification the XML experiment
   * specification string to submit to the server
   * @param status an ExperimentStatus object to be filled in with
   * status information from the submission report
   * @return the experimentID assigned to this experiment by the server
   * @throws ServerException if a problem occurs
   */
  public int submit(String xmlExperimentSpecification, ExperimentStatus status)
    throws ServerException;

  /**
   * Checks on the status of a previously submitted experiment.
   *
   * @param experimentID the experimentID previously assigned to this
   * experiment by the server and returned by the submit() method
   * @return an ExperimentStatus object detailing the current status
   * of the experiment
   * @throws ServerException if a problem occurs
   */
  public ExperimentStatus getExperimentStatus(int experimentID)
    throws ServerException;

  /**
   * Retrieves the result from a previously submitted experiment.
   *
   * @param experimentID the experimentID previously assigned to this
   * experiment by the server and returned by the submit() method
   * @return the XML result string from the server
   * @throws ServerException if a problem occurs, or if the experiment
   * does not have a result to retrieve (i.e. if it hasn't finished
   * running yet, was cancelled, or terminated with errors)
   */
  public String retrieveResult(int experimentID)
    throws ServerException;

  /**
   * Attempts to cancel execution of a previously submitted experiment.
   *
   * @param experimentID the experimentID previously assigned to this
   * experiment by the server and returned by the submit() method
   * @return true iff experiment was successfully removed from the
   * queue before execution had begun
   * @throws ServerException if a problem occurs
   */
  public boolean cancel(int experimentID)
    throws ServerException;

  /**
   * Returns a List of the names of existing ClientItems.
   *
   * @return immutable List of String
   * @throws ServerException if a problem occurs
   */
  public List listAllClientItems()
    throws ServerException;

  /**
   * Loads a ClientItem from the server.
   *
   * @param name the name of the ClientItem to load
   * @return the value of the ClientItem retrieved from the server
   * @throws ServerException if a problem occurs
   */
  public String loadClientItem(String name)
    throws ServerException;

  /**
   * Saves a ClientItem on the server.
   *
   * @param name the name of the ClientItem to save (if a ClientItem
   * with this name already exists, it will be overwritten)
   * @param value the value to be saved
   * @throws ServerException if a problem occurs
   */
  public void saveClientItem(String name, String value)
    throws ServerException;

  /**
   * Deletes a ClientItem from the server.
   *
   * @param name the name of the ClientItem to delete
   * @throws ServerException if a problem occurs
   */
  public void deleteClientItem(String name)
    throws ServerException;

  /**
   * Retrieves the ExperimentSpecification from a previously executed
   * experiment.  Returns null if a record of this experiment exists
   * but does not contain an ExperimentSpecification.
   *
   * @param experimentID the experimentID of the previously executed experiment
   * @return the XML experiment specification string from the server
   * @throws ServerException if a problem occurs
   */
  public String retrieveSpecification(int experimentID)
    throws ServerException;

  /**
   * Retrieves the LabConfiguration from a previously executed
   * experiment.  Returns null if a record of this experiment exists
   * but does not contain a LabConfiguration.
   *
   * @param experimentID the experimentID of the previously executed experiment
   * @return the XML lab configuration string from the server
   * @throws ServerException if a problem occurs
   */
  public String retrieveLabConfiguration(int experimentID)
    throws ServerException;

  /**
   * Retrieves experiment metadata for all of the specified
   * experimentIDs.
   *
   * @param experimentIDs List of Integer containing experimentIDs of
   * one or more previously executed experiments
   * @return immutable List of ExperimentInformation (one for each
   * experimentID)
   * @throws ServerException if a problem occurs
   */
  public List getExperimentInformation(List experimentIDs)
    throws ServerException;

  /**
   * Saves a user-defined annotation for a previously executed
   * experiment.
   *
   * @param experimentID the experimentID of the previously executed experiment
   * @return the previous annotation for this experiment, or "" if none
   * @throws ServerException if a problem occurs
   */
  public String saveAnnotation(int experimentID, String annotation)
    throws ServerException;

  /**
   * Retrieves a previously saved experiment annotation.
   *
   * @param experimentID the experimentID of the previously executed experiment
   * @return the user-defined annotation for this experiment, or "" if none
   * @throws ServerException if a problem occurs
   */
  public String retrieveAnnotation(int experimentID)
    throws ServerException;

} // end interface Server
