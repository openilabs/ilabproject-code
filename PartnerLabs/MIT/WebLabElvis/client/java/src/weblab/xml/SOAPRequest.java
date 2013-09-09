/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.xml;

// NO LONGER JAVA 1.1 COMPLIANT (HttpURLConnection.getErrorStream)

import java.util.Vector;

import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.BufferedReader;
import java.io.OutputStreamWriter;
import java.io.IOException;

import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLConnection;


/**
 * Represents a SOAP RPC request.
 */
public class SOAPRequest
{
  private String methodName;
  private String xmlNamespace;

  private Vector paramNames; // contains String
  private Vector paramValues; // contains String

  //SB coupon info 
  private static long couponID = -1L;
  private static String couponPassKey;

  /**
   * Constructs a new SOAP RPC request for the method methodName in
   * the namespace xmlNamespace, with no parameters.
   */
  public SOAPRequest(String xmlNamespace, String methodName)
  {
    this.xmlNamespace = xmlNamespace;
    this.methodName = methodName;

    this.paramNames = new Vector();
    this.paramValues = new Vector();
  }


    //Provides SB coupon info to be placed in teh request's SOAP Header
    public static void addSBHeader(long id, String passcode) {
          SOAPRequest.couponID = id;
          SOAPRequest.couponPassKey = passcode;
      }


  /**
   * Adds a parameter to this SOAP RPC request.
   */
  public void addParameter(String name, String value)
  {
    paramNames.addElement(name);
    paramValues.addElement(value);
  }



  /**
   * Returns the XML representation of this SOAP RPC request.
   */
  public String toString()
  {
    StringBuffer sb = new StringBuffer(1000);

    // write XML preamble stuff, opening tags for SOAP elements
    sb.append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
    sb.append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
    
    //also write SB coupon info into SOAP Header
	if (couponID > 0) {
	        sb.append("<soap:Header><sbAuthHeader xmlns=\"http://ilab.mit.edu\"><couponID>"
	                + couponID + "</couponID><couponPassKey>" + couponPassKey
	                + "</couponPassKey></sbAuthHeader></soap:Header>");

       		}
    
    sb.append("<soap:Body>");

    // write opening tag for method element
    sb.append("<" + methodName + " xmlns=\"" + xmlNamespace + "\">");

    // write parameter elements
    for (int i = 0; i < paramNames.size(); i++)
    {
      String name = (String) paramNames.elementAt(i);
      String value = Parser.xmlEscape
	((String) paramValues.elementAt(i));

      sb.append("<" + name + ">" + value + "</" + name + ">");
    }

    // write closing tag for method element
    sb.append("</" + methodName + ">");

    // write closing tags for SOAP elements
    sb.append("</soap:Body>");
    sb.append("</soap:Envelope>");

    return sb.toString();
  }



  /**
   * Sends this request to the specified serviceURL, parses the
   * response, and returns the SOAP Body element.
   *
   * @throws SOAPFaultException if the response from the server
   * contains a SOAP Fault element
   * @throws InvalidXMLException if the response from the server is
   * not a well-formed XML SOAP response
   * @throws IOException if an I/O error occurs
   */
  public Element invoke(String serviceURL)
    throws SOAPFaultException, IOException, InvalidXMLException
  {
    String request = this.toString();
    String response;

    System.out.println("Invoking web method " + this.methodName);
    System.out.println("SOAP Request:\n" + Parser.xmlPrettyPrint(request)
		       + "\n");

    URL theURL = new URL(serviceURL);

    try
    {
      URLConnection urlConnection = theURL.openConnection();

      if (! (urlConnection instanceof HttpURLConnection))
	throw new IOException
	  ("openConnection did not return an HttpURLConnection");

      HttpURLConnection httpConnection = (HttpURLConnection) urlConnection;

      httpConnection.setRequestMethod("POST");
      httpConnection.setRequestProperty
	("Content-Type", "text/xml; charset=utf-8");
      httpConnection.setRequestProperty
	("Content-Length", String.valueOf(request.length()));
      httpConnection.setRequestProperty
	("SOAPAction", "\"" + xmlNamespace + "/" + methodName + "\"");
      httpConnection.setDoOutput(true);
      httpConnection.setDoInput(true);

      // Send request
      OutputStreamWriter out =
	new OutputStreamWriter(httpConnection.getOutputStream());
      out.write(request, 0, request.length());
      out.close();

      // Check for error data in response; if any is present, log to
      // console.  In case of HTTP response code 500 with error data,
      // also check for presence of a SOAP Fault (which according to
      // W3C's SOAP spec must be accompanied by HTTP 500), and throw
      // SOAPFaultException if one is found.
      //
      int responseCode = httpConnection.getResponseCode();
      InputStream errorStream = httpConnection.getErrorStream();
      if (errorStream != null)
      {
	System.out.println
	  ("Server returned HTTP response code " + responseCode +
	   " (" + httpConnection.getResponseMessage() + ") for URL: "
	   + serviceURL);
	System.out.println
	  ("Server response included accompanying error data:");
	response = readStream(errorStream);
	System.out.println(Parser.xmlPrettyPrint(response) + "\n");

	if (responseCode == 500)
	{
	  // attempt to parse error data as a SOAP response to see if
	  // it contains a SOAP Fault.  Hopefully this call will throw
	  // a SOAPFaultException which we will allow to propagate up.
	  //
	  // InvalidXMLException, or no exception at all, means it
	  // wasn't a SOAP Fault but some other type of HTTP 500
	  // error.  Easiest thing to do in this case is proceed with
	  // trying to read the input stream, and let Java throw its
	  // standard IOException.
	  try {
	    parseSOAPResponse(response);
	  }
	  catch (InvalidXMLException e) {
	    System.out.println
	      ("Attempting to parse error data as a SOAPFault " +
	       "generated InvalidXMLException: " + e.getMessage());
	  }
	  System.out.println
	    ("HTTP 500 does not appear to be indicating a SOAP fault.");
	}
      }

      // Receive response
      //
      response = readStream(httpConnection.getInputStream());
      System.out.println("SOAP Response:\n" +
			 Parser.xmlPrettyPrint(response.toString()) + "\n");

      // parse SOAP response and return Body element
      return parseSOAPResponse(response);
    }
    catch(SecurityException e) {
      throw new IOException
	("Security manager forbids access to " + serviceURL);
    }
  }



  // helper method to read all lines from an InputStream into a String
  private String readStream(InputStream is) throws IOException
  {
    StringBuffer data = new StringBuffer(1000);

    BufferedReader in = new BufferedReader(new InputStreamReader(is));
    String nextLine;
    while( (nextLine = in.readLine()) != null)
    {
      data.append(nextLine + "\n");
    }
    in.close();

    return data.toString();
  }



  // helper method to parse a SOAP response and return the Body
  // element, or throw SOAPFaultException if there is a SOAP Fault.
  private Element parseSOAPResponse(String response)
    throws SOAPFaultException, InvalidXMLException
  {
    // Root element should be Envelope, and should contain Body.  Body
    // may or may not contain Fault.

    Element envelope = Parser.parse(response);
    if (! envelope.getName().equals("Envelope"))
      throw new InvalidXMLException
	("SOAP response has unexpected root element "
	 + envelope.getName() + " (expected: Envelope)");

    Element body = envelope.getChild("Body");
    if (body == null)
      throw new InvalidXMLException
	("SOAP response missing Body element");

    // Check if there's a Fault.  If so, throw SOAPFaultException.

    Element fault = body.getChild("Fault");
    if (fault != null)
    {
      Element faultstring = fault.getChild("faultstring");
      if (faultstring != null)
	throw new SOAPFaultException(faultstring.getData());
      else
	throw new SOAPFaultException("(couldn't extract faultstring)");
    }

    // return Body element
    return body;
  }

} // end class SOAPRequest
