package weblab.client.serverInterface;

// JAVA 1.1 COMPLIANT

import java.util.Enumeration;
import java.util.Vector;

import java.io.IOException;

import weblab.xml.SOAPRequest;
import weblab.xml.Element;
import weblab.xml.InvalidXMLException;
import weblab.xml.SOAPFaultException;


/**
 * Provides Server functionality by communicating with an iLab Shared
 * Architecture Service Broker.
 */
public class SBServer implements Server
{
  private final String serviceURL;
  private final String labServerID;

  // written by execute, read by getExecutionStatus
  private volatile String executionStatus;

  // estimated LOCAL COMPUTER TIME (milliseconds) when execute will
  // complete, or -1 for unknown.  written by execute, read by
  // getExecutionEstimatedTimeRemaining
  private volatile long estCompletionTime;

  // signal flag to cancel execution, written by cancelExecution and
  // read by execute
  private volatile boolean cancelFlag;

  /**
   * the well-known XML namespace associated with the iLab Shared
   * Architecture
   */
  public static final String serviceNamespace = "http://ilab.mit.edu";

  // number of seconds to wait between calls to check on the status of
  // a running experiment
  private static final int EXECUTE_LOOP_DELAY = 3;

  private static final String NULLPOINTER_MESSAGE =
    "SOAP response missing expected element";
  private static final String IO_MESSAGE =
    "An I/O error occurred";



  /**
   * Constructs a new SBServer for the specified serviceURL and
   * labServerID.
   *
   * @param serviceURL the URL of the Service Broker's web services
   * interface, to which SOAP RPC calls will be directed
   * @param labServerID the labServerID by which the Service Broker
   * knows our lab server, which will be included as an argument of
   * most of our web service calls.
   */
  public SBServer(String serviceURL, String labServerID)
  {
    this.serviceURL = serviceURL;
    this.labServerID = labServerID;

    this.executionStatus = "";
    this.estCompletionTime = -1;
  }

/**
   * This method relays the SB coupon info read from the loader script parameter
   * list to the SOAPRequest object
   * 
   * @param couponID The ID value of the current lab session coupon as provided
   * by the Service Broker.
   * @param couponPassKey The passkey value of the current lab session coupon
   * as provided by the Service Broker.
   */
  
  public void addCouponInformation(long couponID, String couponPassKey) {
        SOAPRequest.addSBHeader(couponID, couponPassKey);
    }

  public final String getLabInfo()
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "GetLabInfo");
      request.addParameter("labServerID", labServerID);

      return request.invoke(serviceURL)
	.getChild("GetLabInfoResponse")
	.getChild("GetLabInfoResult").getData();
    }
    catch (SOAPFaultException ex)
    {
      throw new ServerException("SOAP Fault: " + ex.getMessage());
    }
    catch (InvalidXMLException ex)
    {
      ex.printStackTrace();
      throw new ServerException("received invalid SOAP response");
    }
    catch (IOException ex)
    {
      ex.printStackTrace();
      throw new ServerException(IO_MESSAGE);
    }
    catch (NullPointerException ex)
    {
      ex.printStackTrace();
      throw new ServerException(NULLPOINTER_MESSAGE);
    }
  }



  public final String getLabConfiguration()
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "GetLabConfiguration");
      request.addParameter("labServerID", labServerID);

      return request.invoke(serviceURL)
	.getChild("GetLabConfigurationResponse")
	.getChild("GetLabConfigurationResult").getData();
    }
    catch (SOAPFaultException ex)
    {
      throw new ServerException("SOAP Fault: " + ex.getMessage());
    }
    catch (InvalidXMLException ex)
    {
      ex.printStackTrace();
      throw new ServerException("received invalid SOAP response");
    }
    catch (IOException ex)
    {
      ex.printStackTrace();
      throw new ServerException(IO_MESSAGE);
    }
    catch (NullPointerException ex)
    {
      ex.printStackTrace();
      throw new ServerException(NULLPOINTER_MESSAGE);
    }
  }



  public final String execute(String xmlExperimentSpecification)
    throws ServerException
  {
    try
    {
      this.executionStatus = "submitting";

      // ensure that cancelFlag and estCompletionTime are clear
      this.cancelFlag = false;
      this.estCompletionTime = -1;

      //
      // invoke Submit
      //

      SOAPRequest submitRequest =
	new SOAPRequest(serviceNamespace, "Submit");
      submitRequest.addParameter("labServerID", labServerID);
      submitRequest.addParameter("experimentSpecification",
				 xmlExperimentSpecification);
      submitRequest.addParameter("priorityHint", "0");
      submitRequest.addParameter("emailNotification", "false");

      Element submitResult = submitRequest.invoke(serviceURL)
	.getChild("SubmitResponse")
	.getChild("SubmitResult");

      boolean accepted = Boolean.valueOf
	(submitResult
	 .getChild("vReport")
	 .getChild("accepted")
	 .getData())
	.booleanValue();

      if (! accepted)
      {
	String error = submitResult.getChild("vReport")
	  .getChild("errorMessage").getData();
	throw new ServerException
	  ("Lab server rejected experiment as invalid: " + error);
      }

      this.executionStatus = "accepted by lab server";

      // experimentID is actually an int, but there's no point in
      // parsing it because I'm just going to turn around and use it
      // as a String again
      String experimentID = submitResult.getChild("experimentID").getData();

      //
      // Set up requests for GetExperimentStatus and RetrieveResult
      //

      SOAPRequest getExperimentStatusRequest =
	new SOAPRequest(serviceNamespace, "GetExperimentStatus");
      getExperimentStatusRequest.addParameter("experimentID", experimentID);

      SOAPRequest retrieveResultRequest =
	new SOAPRequest(serviceNamespace, "RetrieveResult");
      retrieveResultRequest.addParameter("experimentID", experimentID);

      //
      // Begin the "execution loop".  It lasts until we return or
      // throw an exception, or until the cancelFlag is raised.
      //

      while(! this.cancelFlag)
      {
	// invoke GetExperimentStatus and extract statusReport

	Element statusReport =
	  getExperimentStatusRequest.invoke(serviceURL)
	  .getChild("GetExperimentStatusResponse")
	  .getChild("GetExperimentStatusResult")
	  .getChild("statusReport");

	int statusCode = Integer.parseInt
	  (statusReport.getChild("statusCode").getData());

	switch (statusCode)
	{
	case 1: // waiting in queue
	  // update status and estCompletionTime and loop

	  Element waitEstimate = statusReport.getChild("wait");
	  int qlength = Integer.parseInt
	    (waitEstimate.getChild("effectiveQueueLength").getData());

	  // estimated wait in seconds before experiment will begin running
	  int estWait = Double.valueOf
	    (waitEstimate.getChild("estWait").getData()).intValue();

	  // estimated runtime in seconds
	  int estRuntime = Double.valueOf
	    (statusReport.getChild("estRuntime").getData()).intValue();

	  // TEMPORARY DEBUGGING HACK!  REMOVE  <dmrz>
	  System.out.println("estWait: " + estWait);
	  System.out.println("estRuntime: " + estRuntime);
	  System.out.println();

	  if (estWait < 0 || estRuntime < 0)
	    this.estCompletionTime = -1;  // unknown
	  else
	    this.estCompletionTime =
	      System.currentTimeMillis() + (estWait + estRuntime) * 1000;

	  this.executionStatus = "waiting in queue (" + qlength + " left)";
	  break;

	case 2: // currently running
	  // update status and estCompletionTime and loop

	  int estRemainingRuntime = Double.valueOf
	    (statusReport.getChild("estRemainingRuntime").getData())
	    .intValue();

	  // TEMPORARY DEBUGGING HACK!  REMOVE  <dmrz>
	  System.out.println("estRemainingRuntime: " + estRemainingRuntime);
	  System.out.println();

	  if (estRemainingRuntime < 0)
	    this.estCompletionTime = -1;  // unknown
	  else
	    this.estCompletionTime =
	      System.currentTimeMillis() + estRemainingRuntime * 1000;

	  this.executionStatus = "running";
	  break;

	case 3: // terminated normally
	  // invoke RetrieveResult and return the result
	  return retrieveResultRequest.invoke(serviceURL)
	    .getChild("RetrieveResultResponse")
	    .getChild("RetrieveResultResult")
	    .getChild("experimentResults")
	    .getData();

	case 4: // terminated with errors
	  // invoke RetrieveResult and process the error
	  String errorMessage = retrieveResultRequest.invoke(serviceURL)
	    .getChild("RetrieveResultResponse")
	    .getChild("RetrieveResultResult")
	    .getChild("errorMessage")
	    .getData();
	  throw new ServerException
	    ("Experiment terminated with errors: " + errorMessage);

	case 5: // cancelled by user before execution began
	  throw new ServerException
	    ("Server reports experiment cancelled by user");

	case 6: // unknown experimentID
	  throw new ServerException
	    ("Server failed to recognize experiment ID");

	default: // illegal status code
	  throw new ServerException
	    ("Server returned illegal status code: " + statusCode);
	}

	// sleep for EXECUTE_LOOP_DELAY seconds, unless we get
	// interrupted first
	try {
	  Thread.sleep(EXECUTE_LOOP_DELAY * 1000);
	}
	catch (InterruptedException e) {
	  // do nothing
	}
      }

      // if we get past the while loop, it's because we got cancelled
      this.cancelFlag = false;

      //
      // invoke Cancel (ignore result -- this is a best effort)
      //

      SOAPRequest cancelRequest =
	new SOAPRequest(serviceNamespace, "Cancel");
      cancelRequest.addParameter("experimentID", experimentID);
      cancelRequest.invoke(serviceURL);

      throw new ServerException("Execution cancelled");
    }
    catch (SOAPFaultException ex)
    {
      throw new ServerException("SOAP Fault: " + ex.getMessage());
    }
    catch (InvalidXMLException ex)
    {
      ex.printStackTrace();
      throw new ServerException("received invalid SOAP response");
    }
    catch (IOException ex)
    {
      ex.printStackTrace();
      throw new ServerException(IO_MESSAGE);
    }
    catch (NullPointerException ex)
    {
      ex.printStackTrace();
      throw new ServerException(NULLPOINTER_MESSAGE);
    }
    finally {
      this.estCompletionTime = -1;
      this.executionStatus = "";
    }
  }



  public final String getExecutionStatus()
  {
    if (this.cancelFlag)
      return "cancelling, please wait...";

    return this.executionStatus;
  }



  public final String getExecutionEstimatedTimeRemaining()
  {
    if (this.cancelFlag)
      return "";

    if (this.estCompletionTime == -1)
      return "unknown";

    long seconds = (this.estCompletionTime - System.currentTimeMillis())
      / 1000;
    String et = seconds + " seconds";
    return et;
  }



  public void cancelExecution()
    throws ServerException
  {
    this.cancelFlag = true;
  }



  public final Enumeration getSavedExpConfigurationNames()
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "ListAllClientItems");

      Enumeration enumSaved = request.invoke(serviceURL)
      							.getChild("ListAllClientItemsResponse")
      							.getChild("ListAllClientItemsResult")
      							.getChildren("string");

      Vector setups = new Vector();

      while (enumSaved.hasMoreElements())
      {
	String next = ((Element) enumSaved.nextElement()).getData();
	if (next.startsWith("savedSetup_"))
	  setups.addElement(next.substring(11));
      }

      return setups.elements();
    }
    catch (SOAPFaultException ex)
    {
      throw new ServerException("SOAP Fault: " + ex.getMessage());
    }
    catch (InvalidXMLException ex)
    {
      ex.printStackTrace();
      throw new ServerException("received invalid SOAP response");
    }
    catch (IOException ex)
    {
      ex.printStackTrace();
      throw new ServerException(IO_MESSAGE);
    }
    catch (NullPointerException ex)
    {
      ex.printStackTrace();
      throw new ServerException(NULLPOINTER_MESSAGE);
    }
  }



  public final String loadExpConfiguration(String name)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "LoadClientItem");
      request.addParameter("name", "savedSetup_" + name);

      return request.invoke(serviceURL)
	.getChild("LoadClientItemResponse")
	.getChild("LoadClientItemResult").getData();
    }
    catch (SOAPFaultException ex)
    {
      throw new ServerException("SOAP Fault: " + ex.getMessage());
    }
    catch (InvalidXMLException ex)
    {
      ex.printStackTrace();
      throw new ServerException("received invalid SOAP response");
    }
    catch (IOException ex)
    {
      ex.printStackTrace();
      throw new ServerException(IO_MESSAGE);
    }
    catch (NullPointerException ex)
    {
      ex.printStackTrace();
      throw new ServerException(NULLPOINTER_MESSAGE);
    }
  }



  public final void saveExpConfiguration(String name, String setup)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "SaveClientItem");
      request.addParameter("name", "savedSetup_" + name);
      request.addParameter("itemValue", setup);
      request.invoke(serviceURL);
    }
    catch (SOAPFaultException ex)
    {
      throw new ServerException("SOAP Fault: " + ex.getMessage());
    }
    catch (InvalidXMLException ex)
    {
      ex.printStackTrace();
      throw new ServerException("received invalid SOAP response");
    }
    catch (IOException ex)
    {
      ex.printStackTrace();
      throw new ServerException(IO_MESSAGE);
    }
  }



  public final void deleteExpConfiguration(String name)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "DeleteClientItem");
      request.addParameter("name", "savedSetup_" + name);
      request.invoke(serviceURL);
    }
    catch (SOAPFaultException ex)
    {
      throw new ServerException("SOAP Fault: " + ex.getMessage());
    }
    catch (InvalidXMLException ex)
    {
      ex.printStackTrace();
      throw new ServerException("received invalid SOAP response");
    }
    catch (IOException ex)
    {
      ex.printStackTrace();
      throw new ServerException(IO_MESSAGE);
    }
  }

} // end class SBServer
