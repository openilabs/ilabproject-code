/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.serverInterface;

import java.util.Collections;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

import java.util.Date;
import java.text.SimpleDateFormat;
import java.text.ParseException;

import java.io.IOException;

import weblab.toolkit.xml.*;


/**
 * Implements Server functionality by communicating with an iLab
 * Shared Architecture Service Broker.
 */
public class SBServer implements Server
{
  private String serviceURL;
  private String labServerID;

  /**
   * the well-known XML namespace associated with the iLab Shared
   * Architecture
   */
  public static final String serviceNamespace = "http://ilab.mit.edu";



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

  public String getLabInfo()
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace, "GetLabInfo");
      request.addParameter("labServerID", labServerID);

      return request.invoke(serviceURL)
	.getRequiredChild("GetLabInfoResponse")
	.getRequiredChild("GetLabInfoResult").getData();
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public String getLabConfiguration()
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "GetLabConfiguration");
      request.addParameter("labServerID", labServerID);

      return request.invoke(serviceURL)
	.getRequiredChild("GetLabConfigurationResponse")
	.getRequiredChild("GetLabConfigurationResult").getData();
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public int submit(String xmlExperimentSpecification)
    throws ServerException
  {
    return submit(xmlExperimentSpecification, null);
  }



  public int submit(String xmlExperimentSpecification, ExperimentStatus status)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace, "Submit");
      request.addParameter("labServerID", labServerID);
      request.addParameter("experimentSpecification",
			   xmlExperimentSpecification);
      request.addParameter("priorityHint", "0");
      request.addParameter("emailNotification", "false");

      Element result = request.invoke(serviceURL)
	.getRequiredChild("SubmitResponse")
	.getRequiredChild("SubmitResult");

      Element vReport = result.getRequiredChild("vReport");

      boolean accepted = Boolean.valueOf
	(vReport.getRequiredChild("accepted").getRequiredData())
	.booleanValue();

      if (! accepted)
      {
	String error = vReport.getRequiredChild("errorMessage").getData();
	throw new ServerException
	  ("Lab server rejected experiment as invalid: " + error);
      }

      // write status fields
      if (status != null)
      {
	status.statusCode = ExperimentStatus.STATUS_WAITING;

	Element waitEstimate = result.getRequiredChild("wait");
	status.effectiveQueueLength = Integer.parseInt
	  (waitEstimate.getRequiredChild("effectiveQueueLength")
	   .getRequiredData());
	status.estWait = Double.parseDouble
	  (waitEstimate.getRequiredChild("estWait").getRequiredData());

	status.estRuntime = Double.parseDouble
	  (vReport.getRequiredChild("estRuntime").getRequiredData());
      }

      return Integer.parseInt
	(result.getRequiredChild("experimentID").getRequiredData());
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public ExperimentStatus getExperimentStatus(int experimentID)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "GetExperimentStatus");
      request.addParameter("experimentID", Integer.toString(experimentID));

      Element statusReport =
	request.invoke(serviceURL)
	.getRequiredChild("GetExperimentStatusResponse")
	.getRequiredChild("GetExperimentStatusResult")
	.getRequiredChild("statusReport");

      ExperimentStatus status = new ExperimentStatus();

      status.statusCode = Integer.parseInt
	(statusReport.getRequiredChild("statusCode").getRequiredData());

      switch (status.statusCode)
      {
      case ExperimentStatus.STATUS_WAITING:
	// update effectiveQueueLength, estWait, and estRuntime
	Element waitEstimate = statusReport.getRequiredChild("wait");
	status.effectiveQueueLength = Integer.parseInt
	  (waitEstimate.getRequiredChild("effectiveQueueLength")
	   .getRequiredData());
	status.estWait = Double.parseDouble
	  (waitEstimate.getRequiredChild("estWait").getRequiredData());
	status.estRuntime = Double.parseDouble
	  (statusReport.getRequiredChild("estRuntime").getRequiredData());
	break;

      case ExperimentStatus.STATUS_RUNNING:
	// update estRemainingRuntime
	status.estRemainingRuntime = Double.parseDouble
	  (statusReport.getRequiredChild("estRemainingRuntime")
	   .getRequiredData());
	break;

      case ExperimentStatus.STATUS_SUCCESS:
      case ExperimentStatus.STATUS_ERROR:
      case ExperimentStatus.STATUS_CANCELLED:
      case ExperimentStatus.STATUS_UNKNOWN:
	break;

      default: // illegal status code
	throw new ServerException
	  ("Server returned illegal status code: " + status.statusCode);
      }

      return status;
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public String retrieveResult(int experimentID)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "RetrieveResult");
      request.addParameter("experimentID", Integer.toString(experimentID));

      Element result = request.invoke(serviceURL)
	.getRequiredChild("RetrieveResultResponse")
	.getRequiredChild("RetrieveResultResult");

      int statusCode = Integer.parseInt
	(result.getRequiredChild("statusCode").getRequiredData());

      switch (statusCode)
      {
      case ExperimentStatus.STATUS_WAITING:
      case ExperimentStatus.STATUS_RUNNING:
	throw new ServerException
	  ("Experiment has not finished running yet");

      case ExperimentStatus.STATUS_SUCCESS:
	// terminated normally
	return result.getRequiredChild("experimentResults").getData();

      case ExperimentStatus.STATUS_ERROR:
	// terminated with errors
	throw new ServerException
	  ("Experiment terminated with errors: " +
	   result.getRequiredChild("errorMessage").getData());

      case ExperimentStatus.STATUS_CANCELLED:
	throw new ServerException
	  ("Server reports experiment cancelled by user");

      case ExperimentStatus.STATUS_UNKNOWN:
	throw new ServerException
	  ("Server failed to recognize experiment ID");

      default: // illegal status code
	throw new ServerException
	  ("Server returned illegal status code: " + statusCode);
      }
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public boolean cancel(int experimentID)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace, "Cancel");
      request.addParameter("experimentID", Integer.toString(experimentID));
      request.invoke(serviceURL);

      return Boolean.valueOf
	(request.invoke(serviceURL)
	 .getRequiredChild("CancelResponse")
	 .getRequiredChild("CancelResult").getRequiredData())
	.booleanValue();
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public List listAllClientItems()
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "ListAllClientItems");

      List clientItemElts = request.invoke(serviceURL)
	.getRequiredChild("ListAllClientItemsResponse")
	.getRequiredChild("ListAllClientItemsResult")
	.getChildren("string");

      List clientItems = new ArrayList();

      for (int i = 0, n = clientItemElts.size(); i < n; i++)
	clientItems.add(((Element) clientItemElts.get(i)).getData());

      return Collections.unmodifiableList(clientItems);
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public String loadClientItem(String name)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "LoadClientItem");
      request.addParameter("name", name);

      return request.invoke(serviceURL)
	.getRequiredChild("LoadClientItemResponse")
	.getRequiredChild("LoadClientItemResult").getData();
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public void saveClientItem(String name, String value)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "SaveClientItem");
      request.addParameter("name", name);
      request.addParameter("itemValue", value);
      request.invoke(serviceURL);
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public void deleteClientItem(String name)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "DeleteClientItem");
      request.addParameter("name", name);
      request.invoke(serviceURL);
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public String retrieveSpecification(int expID)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "RetrieveSpecification");
      request.addParameter("experimentID", Integer.toString(expID));

      Element result = request.invoke(serviceURL)
	.getRequiredChild("RetrieveSpecificationResponse")
	.getChild("RetrieveSpecificationResult");

      if (result == null)
	return null;
      else
	return result.getData();
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public String retrieveLabConfiguration(int expID)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "RetrieveLabConfiguration");
      request.addParameter("experimentID", Integer.toString(expID));

      Element result = request.invoke(serviceURL)
	.getRequiredChild("RetrieveLabConfigurationResponse")
	.getChild("RetrieveLabConfigurationResult");

      if (result == null)
	return null;
      else
	return result.getData();
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public List getExperimentInformation(List experimentIDs)
    throws ServerException
  {
    try
    {
      int[] expIDs = new int[experimentIDs.size()];
      for (int i = 0, n = experimentIDs.size(); i < n; i++)
	expIDs[i] = ((Integer) experimentIDs.get(i)).intValue();

      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "GetExperimentInformation");
      request.addParameter("experimentIDs", expIDs);

      List xmlExpInfoList = request.invoke(serviceURL)
	.getRequiredChild("GetExperimentInformationResponse")
	.getRequiredChild("GetExperimentInformationResult")
	.getChildren("ExperimentInformation");

      List expInfoList = new ArrayList();

      for (Iterator i = xmlExpInfoList.iterator(); i.hasNext(); )
      {
	Element xmlExpInfo = (Element) i.next();

	ExperimentInformation expInfo = new ExperimentInformation();

	Element elt;

	expInfo.experimentID = Integer.valueOf
	  (xmlExpInfo.getRequiredChild("experimentID").getRequiredData());

	expInfo.labServerID = Integer.valueOf
	  (xmlExpInfo.getRequiredChild("labServerID").getRequiredData());

	expInfo.submissionTime = parseDateTime
	  (xmlExpInfo.getRequiredChild("submissionTime").getRequiredData());

	expInfo.completionTime = parseDateTime
	  (xmlExpInfo.getRequiredChild("completionTime").getRequiredData());

	expInfo.priorityHint = Integer.parseInt
	  (xmlExpInfo.getRequiredChild("priorityHint").getRequiredData());

	expInfo.statusCode = Integer.parseInt
	  (xmlExpInfo.getRequiredChild("statusCode").getRequiredData());

    //checking for validation warning messages element
	elt = xmlExpInfo.getChild("validationWarningMessages");
    if (elt != null)
    {
        List xmlMessages = xmlExpInfo
          .getRequiredChild("validationWarningMessages")
          .getChildren("string");
        expInfo.validationWarningMessages = new String[xmlMessages.size()];
        for (int j = 0, n = xmlMessages.size(); j < n; j++)
        {
          expInfo.validationWarningMessages[j] =
            ((Element) xmlMessages.get(j)).getData();
        }
    }

    //checking for validation error message element
	elt = xmlExpInfo.getChild("validationErrorMessage");
	if (elt != null)
	  expInfo.validationErrorMessage = elt.getData();

    //checking for execution warning messages element
    elt = xmlExpInfo.getChild("executionWarningMessages");
    if (elt != null)
    {
        List xmlMessages = xmlExpInfo
          .getRequiredChild("executionWarningMessages")
          .getChildren("string");
        expInfo.executionWarningMessages = new String[xmlMessages.size()];
        for (int j = 0, n = xmlMessages.size(); j < n; j++)
        {
          expInfo.executionWarningMessages[j] =
            ((Element) xmlMessages.get(j)).getData();
        }
    }

    //checking for execution error message element
	elt = xmlExpInfo.getChild("executionErrorMessage");
	if (elt != null)
	  expInfo.executionErrorMessage = elt.getData();

    //checking for experiment annotation (required)
	expInfo.annotation =
	  xmlExpInfo.getRequiredChild("annotation").getData();

    //checking for xml result extensions element
	elt = xmlExpInfo.getChild("xmlResultExtension");
	if (elt != null)
	  expInfo.xmlResultExtension = elt.getData();

    //checking for xml blob extension element
	elt = xmlExpInfo.getChild("xmlBlobExtension");
	if (elt != null)
	  expInfo.xmlBlobExtension = elt.getData();

        expInfoList.add(expInfo);
      }

      return Collections.unmodifiableList(expInfoList);
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public String saveAnnotation(int experimentID, String annotation)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "SaveAnnotation");
      request.addParameter("experimentID", Integer.toString(experimentID));
      request.addParameter("annotation", annotation);

      return request.invoke(serviceURL)
	.getRequiredChild("SaveAnnotationResponse")
	.getRequiredChild("SaveAnnotationResult").getData();
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }



  public String retrieveAnnotation(int experimentID)
    throws ServerException
  {
    try
    {
      SOAPRequest request = new SOAPRequest(serviceNamespace,
					    "RetrieveAnnotation");
      request.addParameter("experimentID", Integer.toString(experimentID));

      return request.invoke(serviceURL)
	.getRequiredChild("RetrieveAnnotationResponse")
	.getRequiredChild("RetrieveAnnotationResult").getData();
    }
    catch (Exception ex)
    {
      throw toServerException(ex);
    }
  }


  ////////////////////
  // Helper Methods //
  ////////////////////


  /**
   * Parses a W3C date/time string in long format (complete date plus
   * hours, minutes, seconds and an optional decimal fraction of a
   * second, e.g. "2007-06-27T13:25:12.5000000-04:00"), truncating the
   * result to the nearest second.
   *
   * http://www.w3.org/TR/NOTE-datetime
   */
  public Date parseDateTime(String dateTime) throws ServerException
  {
    // fix timezone designator so Java will understand it: "Z" means
    // GMT+00:00, "+hh:mm" or "-hh:mm" needs "GMT" prepended to it
    if (dateTime.endsWith("Z"))
      dateTime = dateTime.substring(0, dateTime.length() - 1)
	.concat("GMT+00:00");
    else
      dateTime = dateTime.replaceFirst("[+-]\\d{2}:\\d{2}", "GMT$0");

    // discard fractional parts of a second (we don't care)
    dateTime = dateTime.replaceFirst("\\.\\d*", "");

    try
    {
      // parse using SimpleDateFormat
      return (new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ssz"))
	.parse(dateTime);
    }
    catch (ParseException ex)
    {
      throw new ServerException("error parsing date/time", ex);
    }
  }



  // reusable code to take the standard exceptions encountered while
  // calling Web Service methods and wrap them in a ServerException so
  // they can be thrown by other methods of this class.  The resulting
  // ServerException is RETURNED by this method, not thrown.
  //
  // if ex is not of a type that we are expecting to catch, this
  // method will THROW an unchecked Throwable: either ex itself if
  // it's a RuntimeException, or an Error wrapping ex if it's a
  // checked Exception of an unexpected type (this case indicates a
  // serious logic problem that would have been caught by the compiler
  // if I were checking all expected exceptions individually in each
  // method instead of taking this shortcut).
  private ServerException toServerException(Exception ex)
  {
    if (ex instanceof ServerException)
      return (ServerException) ex;

    if (ex instanceof SOAPFaultException)
      return new ServerException("SOAP Fault: " + ex.getMessage(), ex);

    if (ex instanceof InvalidXMLException)
      return new ServerException("received invalid SOAP response", ex);

    if (ex instanceof NumberFormatException)
      return new ServerException("received invalid SOAP response", ex);

    if (ex instanceof IOException)
      return new ServerException
	("An I/O error occurred: " + ex.getMessage(), ex);

    // if we get here, we're dealing with an UNEXPECTED Exception

    if (ex instanceof RuntimeException)
      throw (RuntimeException) ex;
    else
      throw new Error("Encountered unexpected checked Exception type: "
		      + ex.getClass().getName(), ex);
  }

} // end class SBServer
