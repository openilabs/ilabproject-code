/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.util.Collections;
import java.util.Iterator;
import java.util.List;
import java.util.LinkedList;
import java.util.ArrayList;

import weblab.toolkit.serverInterface.Server;
import weblab.toolkit.serverInterface.ServerException;
import weblab.toolkit.serverInterface.ExperimentStatus;
import weblab.toolkit.serverInterface.ExperimentInformation;

import weblab.toolkit.graphing.Graph;

import weblab.toolkit.util.ChangeTrackingObservable;
import weblab.toolkit.util.EngUnits;

import weblab.toolkit.xml.Parser;


/**
 * A facade through which the UI layer accesses all functional
 * operations of the client.  The WeblabClient manages a Server and
 * the active LabConfiguration (the one that reflects the actual
 * current state of the Lab Server).  It contains many methods for
 * operating on Experiments, but does not store references to
 * Experiments.
 *
 * Important note on Observable behavior: UNLIKE all other classes in
 * the weblab.client package, WeblabClient proactively calls
 * notifyObservers on itself when it considers itself to have changed!
 */
public class WeblabClient extends ChangeTrackingObservable
{
  /**
   * The default name prefix for new setups.
   */
  public static final String UNTITLED = "Untitled";

  /**
   * Flag value to inform Observers that there is a new active Lab
   * Configuration associated with this.
   */
  public static final String ACTIVE_LAB_CONFIGURATION_CHANGE =
    "activeLabConfiguration";

  // Types of EngUnits used in this client (defined once globally)
  public static final EngUnits VOLTAGE_UNITS =
    new EngUnits("V", 0, -15);
  public static final EngUnits CURRENT_UNITS =
    new EngUnits("A", 0, -15);



  // minimum time in seconds to wait between calls to check on the
  // status of a running experiment
  private static final int EXECUTE_MIN_POLL_DELAY = 3;

  // fudge factor to add to "intelligent" delay times calculated from
  // lab server's wait time and run time estimates
  private static final int EXECUTE_POLL_DELAY_EPSILON = 2;

  // delay time to use if lab server does not provide estimates
  private static final int EXECUTE_DEFAULT_POLL_DELAY = 5;


  // written by execute, read by getExecutionStatus
  private volatile String executionStatus;

  // estimated LOCAL COMPUTER TIME (milliseconds) when execute will
  // complete, or -1 for unknown.  written by execute, read by
  // getExecutionEstimatedTimeRemaining
  private volatile long estCompletionTime;

  // time in seconds to add on to server's estimate in order to take
  // into account that we still have to retrieve the results (and do a
  // bit of other bookkeeping) after the execution finishes
  private static final int ESTIMATED_RESULT_RETRIEVE_SECONDS = 3;

  // signal flag to cancel execution, written by cancelExecution and
  // read by execute
  private volatile boolean cancelFlag;


  private static final int MEMORIZED_EXP_ID_LIMIT = 100;
  private List memorizedIDs;

  private Server theServer;
  private LabConfiguration activeLabConfiguration;


  /**
   * Creates a new WeblabClient with the specified Server and an empty
   * active LabConfiguration.
   */
  public WeblabClient(Server weblabServer)
  {
    theServer = weblabServer;
    activeLabConfiguration = new LabConfiguration();

    this.executionStatus = "";
    this.estCompletionTime = -1;
  }



  /**
   * Returns the active LabConfiguration.
   */
  public LabConfiguration getActiveLabConfiguration()
  {
    return activeLabConfiguration;
  }



  /**
   * Returns true iff the specified Experiment is active.  An active
   * Experiment is one whose selected Device is present in the active
   * LabConfiguration, meaning that it can be safely submitted for
   * execution on the Lab Server (among other things).
   */
  public boolean isActive(Experiment exp)
  {
    Device d = exp.getExperimentSpecification().getDevice();
    if (d == null)
      return false;

    return activeLabConfiguration.getDevices().contains(d);
  }



  /**
   * Loads a fresh lab configuration from the server.
   */
  public void loadLabConfiguration()
    throws ServerException, InvalidLabConfigurationException
  {
    String xmlLC = theServer.getLabConfiguration();

    // log to console
    System.out.println("LabConfiguration retrieved:");
    System.out.println(Parser.xmlPrettyPrint(xmlLC) + "\n");

    activeLabConfiguration = LabConfiguration.parseXMLLabConfiguration(xmlLC);
    this.setChanged(ACTIVE_LAB_CONFIGURATION_CHANGE);
    this.notifyObservers();
  }



  /**
   * Selects a new device for the specified Experiment from the active
   * Lab Configuration.
   *
   * If a new device will actually be selected, i.e. not -1 and not
   * the same device that's already selected (according to
   * Device.equals), additionally ensures that the experiment's
   * LabConfiguration is set to the active LabConfiguration, resets
   * the name of the experiment's setup to WeblabClient.UNTITLED if it
   * does not already start with WeblabClient.UNTITLED, and ensures
   * that the setup is not quicksaveable.
   *
   * @param exp the Experiment to select a device for
   * @param index the position of the new device in the array of
   * devices from the active LabConfiguration in this (note: NOT the
   * LabConfiguration in exp), or -1 to select no device.
   * @param requestConfirmation whether to throw ConfirmationRequests
   * @throws ConfirmationRequest if the current setup defines ports
   * that are not connected to the new device [only thrown if
   * requestConfirmation is true]
   */
  public void selectDevice(Experiment exp, int index,
			   boolean requestConfirmation)
    throws ConfirmationRequest
  {
    ExperimentSpecification theExpSpec = exp.getExperimentSpecification();
    if (index == -1)
    {
      theExpSpec.setDevice(null);
      if (! theExpSpec.getName().startsWith(UNTITLED))
	theExpSpec.setName(UNTITLED);
      theExpSpec.setQuickSaveable(false);
      exp.setExperimentID(null);
      exp.setSubmissionTime(null);
    }
    else
    {
      Device d = (Device) activeLabConfiguration.getDevices().get(index);

      if (! d.equals(theExpSpec.getDevice()))
      {
	// check compatibility
	if (requestConfirmation && !theExpSpec.checkDeviceCompatibility(d))
	  throw new ConfirmationRequest();

	// proceed, updating exp with active LC
	theExpSpec.setDevice(null);
	exp.setLabConfiguration(activeLabConfiguration);
	theExpSpec.setDevice(d);
	if (! theExpSpec.getName().startsWith(UNTITLED))
	  theExpSpec.setName(UNTITLED);
	theExpSpec.setQuickSaveable(false);
	exp.setExperimentID(null);
	exp.setSubmissionTime(null);
      }
    }

    // notify observers that exp and theExpSpec may have changed
    exp.notifyObservers();
    theExpSpec.notifyObservers();
  }



  /**
   * Resets the setup of the specified Experiment, preserving the
   * currently selected device.
   *
   * @param exp the Experiment to reset
   */
  public void reset(Experiment exp)
  {
    exp.resetSetup(UNTITLED);
    exp.setExperimentID(null);
    exp.setSubmissionTime(null);
    exp.notifyObservers();
  }



  /**
   * Clears the results of the specified Experiment.
   *
   * @param exp the Experiment to clear results from
   */
  public void clearResults(Experiment exp)
  {
    exp.setExperimentResult(new ExperimentResult());
    exp.notifyObservers();
  }



  /**
   * Returns a read-only Iterator over the names of existing saved
   * setups.
   *
   * @return Iterator of String
   * @throws ServerException if a problem occurs
   */
  public Iterator getSetupNames()
    throws ServerException
  {
    List setups = new ArrayList();

    List clientItems = theServer.listAllClientItems();
    for (int i = 0, n = clientItems.size(); i < n; i++)
    {
      String next = (String) clientItems.get(i);
      if (next.startsWith("savedSetup_"))
	setups.add(next.substring(11));
    }

    return setups.iterator();
  }



  /**
   * Loads a setup from the server and applies it to the specified
   * Experiment, preserving the currently selected device.
   *
   * @param exp the Experiment to apply the setup to
   * @param name the name of the setup to load
   * @param requestConfirmation whether to throw ConfirmationRequests
   * @throws ServerException if a problem occurs communicating with
   * the server
   * @throws InvalidExperimentSpecificationException if the setup
   * received from the server is not a valid XML experiment
   * specification
   * @throws ConfirmationRequest if the new setup defines ports that
   * are not connected to the currently selected device [only thrown
   * if requestConfirmation is true]
   */
  public void loadSetup(Experiment exp, String name,
			boolean requestConfirmation)
    throws ServerException, InvalidExperimentSpecificationException,
	   ConfirmationRequest
  {
    String xmlES = theServer.loadClientItem("savedSetup_" + name);
    exp.specifySetup(xmlES, name, requestConfirmation);
    exp.getExperimentSpecification().setQuickSaveable(true);
    exp.setExperimentID(null);
    exp.setSubmissionTime(null);
    exp.notifyObservers();
  }



  /**
   * Saves a setup to the server.
   *
   * @param exp the Experiment to extract the setup from
   * @param name the name under which to save the setup (if a setup of
   * this name already exists, it will be overwritten)
   * @throws ServerException if a problem occurs
   */
  public void saveSetup(Experiment exp, String name)
    throws ServerException
  {
    ExperimentSpecification theExpSpec = exp.getExperimentSpecification();
    String xmlES = theExpSpec.toXMLString();
    theServer.saveClientItem("savedSetup_" + name, xmlES);
    theExpSpec.setName(name);
    theExpSpec.setQuickSaveable(true);
    theExpSpec.notifyObservers();
  }



  /**
   * Deletes a setup from the server.
   *
   * @param name the name of the setup to delete
   * @throws ServerException if a problem occurs
   */
  public void deleteSetup(String name)
    throws ServerException
  {
    theServer.deleteClientItem("savedSetup_" + name);
  }



  /**
   * Returns an immutable List of the experiment IDs of recently-run
   * experiments (by this user, from this client, on this Service
   * Broker).
   *
   * @return List of Integer
   * @throws ServerException if a problem occurs
   */
  public List getRecentExperimentIDs()
    throws ServerException
  {
    if (memorizedIDs == null)
      initializeMemorizedIDs();

    return Collections.unmodifiableList(new ArrayList(memorizedIDs));
  }



  /**
   * Retrieves the experiment metadata for multiple experimentIDs at
   * once.  The returned List may contain null elements (to indicate
   * experimentIDs for which no metadata was available).
   *
   * @param experimentIDs List of Integer containing experimentIDs of
   * one or more previously executed experiments
   * @return immutable List of ExperimentInformation
   * @throws ServerException if a problem occurs
   */
  public List getExperimentInformation(List experimentIDs)
    throws ServerException
  {
    return theServer.getExperimentInformation(experimentIDs);
  }



  /**
   * Submits an experiment specification for execution, waits for
   * execution to finish, and then terminates.  Does not currently
   * perform any client-side validation.
   *
   * @param exp the Experiment to execute
   * @throws ServerException if a problem occurs
   * @throws InvalidExperimentResultException if the result returned
   * by the server is invalid
   */
  public void execute(Experiment exp)
    throws ServerException, InvalidExperimentResultException
  {
    try
    {
      // create and execute the measurement specification
      ExperimentSpecification expSpec = exp.getExperimentSpecification();

      String xmlExpSpec = expSpec.toXMLString();

      this.executionStatus = "submitting";

      // ensure that cancelFlag and estCompletionTime are clear
      this.cancelFlag = false;
      this.estCompletionTime = -1;

      ExperimentStatus status = new ExperimentStatus();

      int expID = theServer.submit(xmlExpSpec, status);

      //XXX store expID in Experiment object, and clear existing
      //Result if any?  Currently not done until successful
      //termination -- question is what we want to happen in the event
      //of unsuccessful termination

      this.executionStatus = "accepted by lab server";

      //
      // Begin the "execution loop".  It lasts until we get a result
      // or throw an exception, or until the cancelFlag is raised.
      //

      String xmlResult = null;

      while (true)
      {
	// time in seconds to wait before polling again (if needed)
	long pollDelay = EXECUTE_DEFAULT_POLL_DELAY;

	switch (status.statusCode)
	{
	case ExperimentStatus.STATUS_WAITING:
	  // update status and estCompletionTime and loop

	  if (status.estWait < 0 || status.estRuntime < 0)
	  {
	    this.estCompletionTime = -1;  // unknown
	  }
	  else
	  {
	    this.estCompletionTime = System.currentTimeMillis() +
	      (long) (status.estWait + status.estRuntime) * 1000;

	    // Poll for status again once we are reasonably likely to
	    // have advanced one position in the queue (or, if
	    // effectiveQueueLength is 0 and we're currently first in
	    // the queue, once we can reasonably expect to have
	    // started running).  In both cases this is given by:
	    //
	    // delay = estWait / (effectiveQueueLength+1)
	    //
	    // but we'll round the division up and add on a small
	    // extra fudge factor, because if we end up polling just
	    // *before* the queue advances then the user's view of the
	    // queue status will appear to progress less smoothly.
	    //
	    // Note however that we don't want to wait *much* longer
	    // than this, in case the experiments ahead of us get
	    // cancelled and we get to start executing sooner than
	    // anticipated.
	    pollDelay = ((long) status.estWait + status.effectiveQueueLength)
	      / (status.effectiveQueueLength+1)
	      + EXECUTE_POLL_DELAY_EPSILON;
	  }

	  this.executionStatus =
	    "waiting in queue (currently at position " +
	    (status.effectiveQueueLength+1) + ")";
	  break;

	case ExperimentStatus.STATUS_RUNNING:
	  // update status and estCompletionTime and loop

	  if (status.estRemainingRuntime < 0)
	  {
	    this.estCompletionTime = -1;  // unknown
	  }
	  else
	  {
	    this.estCompletionTime = System.currentTimeMillis() +
	      (long) status.estRemainingRuntime * 1000;

	    // Poll for status again when we expect to be finished.
	    pollDelay = (long) status.estRemainingRuntime
	      + EXECUTE_POLL_DELAY_EPSILON;
	  }

	  this.executionStatus = "running";
	  break;

	case ExperimentStatus.STATUS_SUCCESS:
	case ExperimentStatus.STATUS_ERROR:
	  // invoke RetrieveResult to either obtain the result or
	  // process the error (as appropriate)
	  this.executionStatus = "retrieving results";
	  xmlResult = theServer.retrieveResult(expID);
	  break;

	case ExperimentStatus.STATUS_CANCELLED:
	  throw new ServerException
	    ("Server reports experiment cancelled by user");

	case ExperimentStatus.STATUS_UNKNOWN:
	  throw new ServerException
	    ("Server failed to recognize experiment ID");
	}

	// if we got a result, stop looping
	if (xmlResult != null)
	  break;

	// sleep for pollDelay seconds (or minimum poll delay) unless
	// interrupted or cancelled first, checking for cancellation
	// once every second.
	if (pollDelay < EXECUTE_MIN_POLL_DELAY)
	  pollDelay = EXECUTE_MIN_POLL_DELAY;
	for (int i = 0; i < pollDelay && !cancelFlag; i++)
	{
	  // sleep for 1 second at a time
	  try {
	    Thread.sleep(1000);
	  }
	  catch (InterruptedException e) {
	    break;
	  }
	}

	// perform cancellation if requested
	if (cancelFlag)
	{
	  // invoke Cancel (ignore result -- this is a best effort)
	  theServer.cancel(expID);
	  this.cancelFlag = false;
	  throw new ServerException("Execution cancelled");
	}

	// poll for status again and loop
	status = theServer.getExperimentStatus(expID);
      }

      // log to console
      System.out.println("ExperimentResult retrieved:");
      System.out.println(Parser.xmlPrettyPrint(xmlResult) + "\n");

      // auto-annotate this experimentID with the name of the device
      // and (if meaningful) the setup name
      String annotation = expSpec.getDevice().getName();
      if (expSpec.isQuickSaveable())
	annotation += ": " + expSpec.getName();
      theServer.saveAnnotation(expID, annotation);

      // memorize this experimentID for future retrieval
      memorizeExperimentID(expID);

      // load the ExperimentInformation
      ExperimentInformation expInfo = (ExperimentInformation)
	theServer.getExperimentInformation
	(Collections.singletonList(new Integer(expID)))
	.get(0);

      // set ExperimentID and submissionTime, clear the current
      // results and store the new ones
      exp.setExperimentID(new Integer(expID));
      exp.setSubmissionTime(expInfo.submissionTime);
      exp.setExperimentResult
	(ExperimentResult.parseXMLExperimentResult(xmlResult));
      exp.notifyObservers();
    }
    finally {
      this.estCompletionTime = -1;
      this.executionStatus = "";
    }
  }



  /**
   * Indicates the status of the execute() operation currently in
   * progress.
   *
   * @return a human-readable status summary, suitable for displaying
   * in a dialog box.
   */
  public String getExecutionStatus()
  {
    if (this.cancelFlag)
      return "cancelling, please wait...";

    return this.executionStatus;
  }



  /**
   * Provides an estimate of the remaining time before the execute()
   * operation currently in progress will be finished.
   *
   * @return estimated time in seconds, or -1 if time is unknown or if
   * there is no execute() operation currently in progress
   */
  public int getExecutionEstimatedTimeRemaining()
  {
    if (this.cancelFlag || this.estCompletionTime == -1)
      return -1;

    long seconds = (this.estCompletionTime - System.currentTimeMillis())
      / 1000 + 1 // +1 to round up instead of truncating down
      + ESTIMATED_RESULT_RETRIEVE_SECONDS;

    // never estimate less than 1 second
    if (seconds <= 0)
      seconds = 1;

    return (int) seconds;
  }



  /**
   * Cancels the execute() operation currently in progress.
   */
  public void cancelExecution()
    throws ServerException
  {
    this.cancelFlag = true;
  }



  /**
   * Returns the current results in CSV (comma-separated value)
   * format, suitable for displaying to the user or exporting to a
   * spreadsheet program.
   *
   * @param exp the Experiment to view data for
   */
  public String viewData(Experiment exp)
  {
    return exp.getExperimentResult().toCSVString();
  }



  /**
   * Loads a previously executed experiment from the service broker's
   * experiment archive and writes the LabConfiguration,
   * ExperimentSpecification, and ExperimentResult to the specified
   * Experiment object.  The components are replaced in that order,
   * which is important because if an exception is thrown while
   * accessing one of the parts, other parts may have already been
   * rewritten.
   *
   * If the experiment contains a validation error message or an
   * execution error message, a ServerException will be thrown when
   * processing the Result (similarly to WeblabClient.execute).  Note,
   * however, that the LabConfiguration and ExperimentSpecification
   * will have already been rewritten by this time, so this represents
   * useful data.
   *
   * Note: the reason the Experiment object is a parameter rather than
   * a return value is so that exceptions thrown because of validation
   * and execution errors don't prevent the ExpSpec and LC from being
   * loaded.  It is intended that this method be called with a newly
   * created, "blank" Experiment object (doing otherwise can lead to
   * possible confusing scenarios if some parts do not get rewritten).
   *
   * @param exp the Experiment to write to
   * @param expID the experiment ID to load
   * @throws ServerException if a problem occurs communicating with
   * the server
   * @throws InvalidLabConfigurationException if an invalid XML lab
   * configuration is received from the server
   * @throws InvalidExperimentSpecificationException if an invalid XML
   * experiment specification is received from the server
   * @throws InvalidExperimentResultException if an invalid XML
   * experiment result is received from the server
   */
  public void loadArchivedExperiment(Experiment exp, int expID)
    throws ServerException, InvalidLabConfigurationException,
	   InvalidExperimentSpecificationException,
	   InvalidExperimentResultException
  {
    try
    {
      // note: log to console, but don't pretty-print XML (to save space)
      System.out.println("Loading archived experiment " + expID + "...\n");

      //
      // load the ExperimentInformation
      //

      ExperimentInformation expInfo = (ExperimentInformation)
	theServer.getExperimentInformation
	(Collections.singletonList(new Integer(expID)))
	.get(0);

      // set ExperimentID
      exp.setExperimentID(new Integer(expID));
      exp.setSubmissionTime(expInfo.submissionTime);

      //
      // load the lab configuration.
      //

      String xmlLC = theServer.retrieveLabConfiguration(expID);
      if (xmlLC == null)
	throw new ServerException
	  ("Could not load archived lab configuration");
      else
	System.out.println
	  ("Lab configuration retrieved: " + xmlLC + "\n");

      LabConfiguration labConf = LabConfiguration
	.parseXMLLabConfiguration(xmlLC);

      exp.setLabConfiguration(labConf);

      //
      // load the experiment specification (selecting the appropriate
      // device from the lab configuration)
      //

      String xmlES = theServer.retrieveSpecification(expID);
      if (xmlES == null)
	throw new ServerException
	  ("Could not load archived experiment specification");
      else
	System.out.println
	  ("Experiment specification retrieved: " + xmlES + "\n");

      ExperimentSpecification expSpec = ExperimentSpecification
	.parseXMLExperimentSpecification(xmlES, labConf);
      expSpec.setName(expInfo.annotation);

      exp.setExperimentSpecification(expSpec);

      //
      // check for validation errors in the ExperimentInformation
      // (RetrieveResult will find execution errors, but implicitly
      // assumes that the experiment has passed validation)
      //

      if (expInfo.validationErrorMessage != null)
	throw new ServerException
	  ("Lab server rejected experiment as invalid: " +
	   expInfo.validationErrorMessage);

      //
      // load the experiment result (note: Server method checks for
      // execution errors, etc and throws ServerException similarly to
      // execute method)
      //

      String xmlResult = theServer.retrieveResult(expID);
      if (xmlResult == null)
	throw new ServerException
	  ("Could not load archived experiment result");
      else
	System.out.println
	  ("Experiment result retrieved: " + xmlResult + "\n");

      ExperimentResult expResult = ExperimentResult
	.parseXMLExperimentResult(xmlResult);

      exp.setExperimentResult(expResult);
    }
    finally
    {
      exp.notifyObservers();
    }
  }



  /**
   * "Pings" the Server with a minor web service request.  Call this
   * method every so often to keep the user's session from timing out
   * due to inactivity.
   */
  public void pingServer()
    throws ServerException
  {
    theServer.saveClientItem("ping", "ping");
  }


  ////////////////////
  // Helper Methods //
  ////////////////////


  private void initializeMemorizedIDs()
    throws ServerException
  {
    memorizedIDs = new LinkedList();

    List clientItems = theServer.listAllClientItems();
    if (! clientItems.contains("recentExperimentIDs"))
	return;

    String idsString = theServer.loadClientItem("recentExperimentIDs");

    // read space-separated list of integer values
    String[] ids = idsString.split(" ");
    for (int i = 0; i < ids.length; i++)
    {
      memorizedIDs.add(Integer.valueOf(ids[i]));
    }
  }



  private void memorizeExperimentID(int expID)
    throws ServerException
  {
    if (memorizedIDs == null)
      initializeMemorizedIDs();

    // never memorize more than MEMORIZED_EXP_ID_LIMIT expIDs,
    // including the new one we're about to add
    while (memorizedIDs.size() >= MEMORIZED_EXP_ID_LIMIT)
      memorizedIDs.remove(0);

    memorizedIDs.add(new Integer(expID));

    StringBuffer sb = new StringBuffer();
    for (Iterator i = memorizedIDs.iterator(); i.hasNext(); )
    {
      sb.append(((Integer) i.next()).toString());
      sb.append(" ");//XXX extra space at very end might be confusing
    }

    theServer.saveClientItem("recentExperimentIDs", sb.toString());
  }

} // end class WeblabClient
