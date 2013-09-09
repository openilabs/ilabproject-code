package weblab.client.serverInterface;

// JAVA 1.1 COMPLIANT

import java.util.Enumeration;

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
   * Submits an experiment specification for execution, waits for
   * execution to finish, and returns the results.
   *
   * @param xmlExperimentSpecification the XML experiment
   * specification string to submit to the server
   * @return the XML result string from the server
   * @throws ServerException if a problem occurs
   */
  public String execute(String xmlExperimentSpecification)
    throws ServerException;

  /**
   * Indicates the status of the execute() operation currently in
   * progress.
   *
   * @return a human-readable status summary, suitable for displaying
   * in a dialog box.
   */
  public String getExecutionStatus();

  /**
   * Provides an estimate of the remaining time before the execute()
   * operation currently in progress will be finished.
   *
   * @return a human-readable time estimate, suitable for displaying
   * in a dialog box.
   */
  public String getExecutionEstimatedTimeRemaining();

  /**
   * Cancels the execute() operation currently in progress.
   */
  public void cancelExecution()
    throws ServerException;

  /**
   * Returns an enumeration of the names of existing saved configurations.
   *
   * @return Enumeration of String
   * @throws ServerException if a problem occurs
   */
  public Enumeration getSavedExpConfigurationNames()
    throws ServerException;

  /**
   * Loads a configuration from the server.
   *
   * @param name the name of the configuration to load
   * @return the configuration string retrieved from the server
   * @throws ServerException if a problem occurs
   */
  public String loadExpConfiguration(String name)
    throws ServerException;

  /**
   * Saves a configuration on the server.
   *
   * @param name the name under which to save the configuration (if a configuaration of
   * this name already exists, it will be overwritten)
   * @param configuration the configuration string to be saved
   * @throws ServerException if a problem occurs
   */
  public void saveExpConfiguration(String name, String configuration)
    throws ServerException;

  /**
   * Deletes a configuration from the server.
   *
   * @param name the name of the configuration to delete
   * @throws ServerException if a problem occurs
   */
  public void deleteExpConfiguration(String name)
    throws ServerException;

} // end interface Server
