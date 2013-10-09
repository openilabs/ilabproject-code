/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.serverInterface;

import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.ArrayList;
import java.util.HashMap;

/**
 * <b>StubServer</b> is an abstract stub implementation of the server
 * interface to facilitate testing of the rest of the applet code.
 *
 * Lab clients should subclass StubServer to return lab
 * configurations, experiment specifications, and experiment results
 * that are suitable for the lab domain.
 */
public abstract class StubServer implements Server
{
  /**
   * The number of dummy experiments that will be simulated to be in
   * the queue ahead of any submitted experiment.  This value can be
   * customized in the subclass constructor.
   */
  protected int QUEUE_LENGTH = 0;

  /**
   * The number of seconds that each experiment will be simulated to
   * run (must be > 0).  This value can be customized in the subclass
   * constructor.
   */
  protected int RUNTIME_SECONDS = 1;

  /**
   * The next experimentID that will be returned by Submit.
   */
  protected int nextExperimentID = 12345;



  private List clientItemNames;
  private List clientItemValues;

  private Map annotations; // maps Integer to String

  private long submitTime;



  public StubServer()
  {
    this.clientItemNames = new ArrayList();
    this.clientItemValues = new ArrayList();
    this.annotations = new HashMap();
  }


  ////////////////////
  // Server methods //
  ////////////////////


  public String getLabInfo()
    throws ServerException
  {
    return "http://www.homestarrunner.com/sbemail.html";
  }



  public int submit(String xmlExperimentSpecification)
    throws ServerException
  {
    return submit(xmlExperimentSpecification, null);
  }



  public int submit(String xmlExperimentSpecification, ExperimentStatus status)
    throws ServerException
  {
    // pretend the web service call takes 1 second
    try
    {
      Thread.sleep(1000);
    }
    catch (InterruptedException e) {}

    submitTime = System.currentTimeMillis();

    // write status fields
    if (status != null)
    {
      writeStatus(status, submitTime);
    }

    return nextExperimentID++;
  }



  public ExperimentStatus getExperimentStatus(int experimentID)
    throws ServerException
  {
    ExperimentStatus status = new ExperimentStatus();
    writeStatus(status, System.currentTimeMillis());
    return status;
  }



  public boolean cancel(int experimentID)
    throws ServerException
  {
    return true;
  }



  public List listAllClientItems()
    throws ServerException
  {
    return Collections.unmodifiableList(new ArrayList(clientItemNames));
  }



  public String loadClientItem(String name)
    throws ServerException
  {
    int index = clientItemNames.indexOf(name);
    if (index == -1) {
      throw new ServerException("no such clientItem");
    }
    else {
      return (String) clientItemValues.get(index);
    }
  }



  public void saveClientItem(String name, String value)
    throws ServerException
  {
    int index = clientItemNames.indexOf(name);
    if (index == -1) {
      // new CI name
      clientItemNames.add(name);
      clientItemValues.add(value);
    }
    else {
      // CI name already exists
      clientItemValues.set(index, value);
    }
  }



  public void deleteClientItem(String name)
    throws ServerException
  {
    int index = clientItemNames.indexOf(name);
    if (index == -1) {
      throw new ServerException("no such clientItem");
    }
    else {
      clientItemNames.remove(index);
      clientItemValues.remove(index);
    }
  }



  public String saveAnnotation(int experimentID, String annotation)
  {
    String s = (String) annotations.put(new Integer(experimentID), annotation);

    return (s == null ? "" : s);
  }



  public String retrieveAnnotation(int experimentID)
  {
    String s = (String) annotations.get(new Integer(experimentID));

    return (s == null ? "" : s);
  }


  // Server methods NOT implemented by this class:
  //
  // public String getLabConfiguration()
  //   throws ServerException;
  // public String retrieveResult(int experimentID)
  //   throws ServerException;
  // public String retrieveSpecification(int experimentID)
  //   throws ServerException;
  // public String retrieveLabConfiguration(int experimentID)
  //   throws ServerException;
  // public List getExperimentInformation(List experimentIDs)
  //   throws ServerException;



  // helper method for status generation
  private void writeStatus(ExperimentStatus status, long currentTime)
  {
    // pretend that there were QUEUE_LENGTH experiments in the queue
    // ahead of this one, and that each experiment takes
    // RUNTIME_SECONDS seconds to run.

    double elapsedTime = currentTime - submitTime;

    int numFinishedExperiments = (int) elapsedTime / 1000 / RUNTIME_SECONDS;

    int currentExperimentRemainingSeconds =
      RUNTIME_SECONDS - (int) (elapsedTime / 1000) % RUNTIME_SECONDS;

    if (numFinishedExperiments < QUEUE_LENGTH)
    {
      status.statusCode = ExperimentStatus.STATUS_WAITING;
      status.effectiveQueueLength = QUEUE_LENGTH - numFinishedExperiments - 1;
      status.estWait = status.effectiveQueueLength * RUNTIME_SECONDS
	+ currentExperimentRemainingSeconds;
      status.estRuntime = RUNTIME_SECONDS;
    }
    else if (numFinishedExperiments == QUEUE_LENGTH)
    {
      status.statusCode = ExperimentStatus.STATUS_RUNNING;
      status.estRemainingRuntime = currentExperimentRemainingSeconds;
    }
    else
    {
      status.statusCode = ExperimentStatus.STATUS_SUCCESS;
    }
  }



} // end class StubServer
