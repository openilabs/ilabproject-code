/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.Iterator;
import java.util.ArrayList;
import java.util.HashMap;

import java.math.BigDecimal;

import weblab.toolkit.xml.Parser;
import weblab.toolkit.xml.Element;
import weblab.toolkit.xml.InvalidXMLException;

import weblab.toolkit.util.EngMath;

/**
 * Contains information about the current setup of the lab, obtained
 * from the lab server.  A LabConfiguration consists of a list of
 * Devices, and an optional cache of image data that can be used in
 * place of trying to load the imageURLs over the web (as this may be
 * forbidden by the Java security sandbox).
 *
 * LabConfiguration is immutable.
 */
public class LabConfiguration
{
  // unmodifiable, contains Device
  private List devices;

  // maps String (a URL) to String (the image located at that URL,
  // encoded in base64)
  private final Map imageCache;


  /**
   * Constructs a new "empty" LabConfiguration with no devices and no
   * image data.
   */
  public LabConfiguration()
  {
    devices = Collections.EMPTY_LIST;
    imageCache = new HashMap();
  }



  /**
   * Returns the list of available Devices.
   *
   * @return unmodifiable List of Device
   */
  public final List getDevices()
  {
    return this.devices;
  }



  /**
   * Returns the base64-encoded image data for the image located at
   * <code>url</code>, or <code>null</code> if this lab configuration
   * does not include image data for <code>url</code>.
   */
  public final String getImageData(String url)
  {
    return (String) imageCache.get(url);
  }



  /**
   * Generates a new LabConfiguration from the specified XML string
   * (which was presumably obtained from the lab server).
   *
   * @throws InvalidLabConfigurationException if xmlString is not a
   * valid XML lab configuration
   */
  public final static LabConfiguration
    parseXMLLabConfiguration(String xmlString)
    throws InvalidLabConfigurationException
  {
    Element xmlLabConfiguration;

    // parse XML and make sure the root element is labConfiguration
    try
    {
      xmlLabConfiguration = Parser.parse(xmlString);

      if (! xmlLabConfiguration.getName().equals("labConfiguration"))
	throw new InvalidLabConfigurationException
	  ("illegal root element " + xmlLabConfiguration.getName());
    }
    catch (InvalidXMLException ex) {
      throw new InvalidLabConfigurationException(ex);
    }

    // construct LabConfiguration based on XML content
    try
    {
      LabConfiguration lc = new LabConfiguration();

      // read <imageData url='X'>X</imageData> and add entries to
      // imageCache
      for (Iterator i = xmlLabConfiguration.getChildren("imageData")
	     .iterator();
	   i.hasNext(); )
      {
	Element xmlImageData = (Element) i.next();

	String url = xmlImageData.getRequiredAttributeValue("url");
	String imageData = xmlImageData.getRequiredData();

	lc.imageCache.put(url, imageData);
      }

      List newDevices = new ArrayList();

      // read <device id='N' type='X'> ... and generate Device objects
      for (Iterator i_devs = xmlLabConfiguration.getChildren("device")
	     .iterator();
	   i_devs.hasNext(); )
      {
	Element xmlDevice = (Element) i_devs.next();

	int deviceID = Integer.parseInt
	  (xmlDevice.getRequiredAttributeValue("id"));

	String deviceType = xmlDevice.getRequiredAttributeValue("type");

	// read <name>X</name>
	String name = xmlDevice.getRequiredChild("name").getRequiredData();

	// read <description>X</description>
	String description =
	  xmlDevice.getRequiredChild("description").getData();

	// read <imageURL>X</imageURL>
	String imageURL =
	  xmlDevice.getRequiredChild("imageURL").getRequiredData();

	// read <maxDataPoints>N</maxDataPoints>
	int maxDataPoints = Integer.parseInt
	  (xmlDevice.getRequiredChild("maxDataPoints").getRequiredData());

	List newDeviceTerminals = new ArrayList();

	// read <terminal portType='X' portNumber='N'> ... and
	// generate Terminal objects
	for (Iterator i_terms = xmlDevice.getChildren("terminal")
	       .iterator();
	     i_terms.hasNext(); )
	{
	  Element xmlTerminal = (Element) i_terms.next();

	  int portType;
	  String portTypeName =
	    xmlTerminal.getRequiredAttributeValue("portType");

	  if (portTypeName.equals("SMU"))
	  {
	    portType = Port.SMU_TYPE;
	  }
	  else if (portTypeName.equals("VSU"))
	  {
	    portType = Port.VSU_TYPE;
	  }
	  else if (portTypeName.equals("VMU"))
	  {
	    portType = Port.VMU_TYPE;
	  }
	  else
	  {
	    throw new InvalidLabConfigurationException
	      ("illegal port type: " + portTypeName);
	  }

	  int portNumber = Integer.parseInt
	    (xmlTerminal.getRequiredAttributeValue("portNumber"));

	  // read <label>X</label>
	  String label =
	    xmlTerminal.getRequiredChild("label").getRequiredData();

	  // read <pixelLocation> ...
	  Element xmlPixelLocation =
	    xmlTerminal.getRequiredChild("pixelLocation");

	  // read <x>N</x>
	  int xPixelLocation = Integer.parseInt
	    (xmlPixelLocation.getRequiredChild("x").getRequiredData());

	  // read <y>N</y>
	  int yPixelLocation = Integer.parseInt
	    (xmlPixelLocation.getRequiredChild("y").getRequiredData());

	  // ... </pixelLocation>

	  // read <maxVoltage>N</maxVoltage>
	  BigDecimal maxVoltage = EngMath.parseBigDecimal
	    (xmlTerminal.getRequiredChild("maxVoltage").getRequiredData());

	  // read <maxCurrent>N</maxCurrent>
	  BigDecimal maxCurrent = EngMath.parseBigDecimal
	    (xmlTerminal.getRequiredChild("maxCurrent").getRequiredData());

	  // ... </terminal>
	  newDeviceTerminals.add
	    (new Terminal(portType, portNumber, label, xPixelLocation,
			  yPixelLocation, maxVoltage, maxCurrent));
	}

	// ... </device>
	newDevices.add
	  (new Device(deviceID, deviceType, name, description,
		      imageURL, (String) lc.imageCache.get(imageURL),
		      maxDataPoints, newDeviceTerminals));
      }

      lc.devices = Collections.unmodifiableList(newDevices);

      return lc;
    }
    catch (InvalidXMLException ex) {
      // it's not really invalid XML per se when this comes from
      // Element.getRequiredFOO, so just copy the message
      throw new InvalidLabConfigurationException(ex.getMessage());
    }
    catch (NumberFormatException ex) {
      throw new InvalidLabConfigurationException(ex);
    }
  }



  // This method is not currently needed by the Weblab client, and has
  // therefore been left unimplemented to save space:
  //
  // public final String toXMLString()



  /**
   * Compares the specified object with this LabConfiguration for
   * equality.  Returns true iff the specified object is also a
   * LabConfiguration and both LabConfigurations have equal lists of
   * Devices (according to Device.equals) and the same cache of image
   * data (i.e. the getImageData method of each will return equal
   * results for equal url arguments).
   */
  public boolean equals(Object obj)
  {
    // check object type
    if (! (obj instanceof LabConfiguration))
      return false;
    LabConfiguration lc = (LabConfiguration) obj;

    // check devices and imageCache (hooray for List.equals and
    // Map.equals!)
    return (this.devices.equals(lc.devices) &&
	    this.imageCache.equals(lc.imageCache));
  }



  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitLabConfiguration(this);
  }

} // end class LabConfiguration
