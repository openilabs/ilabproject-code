package weblab.client;

// JAVA 1.1 COMPLIANT

import java.util.Vector;
import java.util.Hashtable;
import java.util.Enumeration;
import java.math.BigDecimal;

import weblab.xml.Parser;
import weblab.xml.Element;
import weblab.xml.InvalidXMLException;

import weblab.util.EngMath;

/**
 * Contains information about the current setup of the lab, obtained
 * from the lab server.  A LabConfiguration consists of a list of
 * experiment Setups, and an optional cache of image data that can be used in
 * place of trying to load the imageURLs over the web (as this may be
 * forbidden by the Java security sandbox).
 *
 * LabConfiguration is immutable.
 */
public class LabConfiguration
{
  private Setup[] setups;

  // maps String (a URL) to String (the image located at that URL,
  // encoded in base64)
  private final Hashtable imageCache;


  /**
   * Constructs a new "empty" LabConfiguration with no setups.
   */
  public LabConfiguration()
  {
	  setups = new Setup[0];
      imageCache = new Hashtable();
  }



  /**
   * Returns the list of available Setups.
   */
  public final Setup[] getSetups()
  {
    return (Setup[]) this.setups.clone();
  }



  /**
   * Returns the base64-encoded image data for the image located at
   * <code>url</code>, or <code>null</code> if the lab configuration
   * did not include image data for <code>url</code>.
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

    try {
      xmlLabConfiguration = Parser.parse(xmlString);
    }
    catch (InvalidXMLException ex) {
      throw new InvalidLabConfigurationException
	(ex.getMessage());
    }

    if (! xmlLabConfiguration.getName().equals("labConfiguration"))
    {
      throw new InvalidLabConfigurationException
	("illegal root element " + xmlLabConfiguration.getName());
    }

    LabConfiguration lc = new LabConfiguration();

    Vector vec_setups = new Vector();

    // populate vec_setups with a Setup object for each <setup> element
    for (Enumeration enum_setups = xmlLabConfiguration.getChildren("setup");
	 enum_setups.hasMoreElements(); )
    {
      // get next <setup> element
      Element xmlSetup = (Element) enum_setups.nextElement();

      int setupID = Integer.parseInt(xmlSetup.getAttributeValue("id"));

      String name = xmlSetup.getChild("name").getData();

      String description = xmlSetup.getChild("description").getData();

      String imageURL = xmlSetup.getChild("imageURL").getData();

      Vector vec_terminals = new Vector();

      // populate vec_terminals with a Terminal object for each
      // <terminal> element within this setup
      for (Enumeration enum_terms = xmlSetup.getChildren("terminal");
	   enum_terms.hasMoreElements(); )
      {
	Element xmlTerminal = (Element) enum_terms.nextElement();

	int instrumentType;
	String instrumentTypeName = xmlTerminal.getAttributeValue("instrumentType");
	if (instrumentTypeName.equals("FGEN"))
	{
	  instrumentType = Instrument.FGEN_TYPE;
	}
	else if (instrumentTypeName.equals("SCOPE"))
	{
		  instrumentType = Instrument.SCOPE_TYPE;
	}
	else
	{
	  throw new InvalidLabConfigurationException
	    ("illegal instrument type: " + instrumentTypeName);
	}

	int instrumentNumber = Integer.parseInt
	  (xmlTerminal.getAttributeValue("instrumentNumber"));

	String label =
	  xmlTerminal.getChild("label").getData();

	Element xmlPixelLocation = xmlTerminal.getChild("pixelLocation");

	int xPixelLocation = Integer.parseInt
	  (xmlPixelLocation.getChild("x").getData());

	int yPixelLocation = Integer.parseInt
	  (xmlPixelLocation.getChild("y").getData());
	
	vec_terminals.addElement
	  (new Terminal(instrumentType, instrumentNumber, label, xPixelLocation, yPixelLocation));
      }

      Terminal[] terminals = new Terminal[vec_terminals.size()];
      vec_terminals.copyInto(terminals);

      vec_setups.addElement
	(new Setup(setupID, name, description,
		    imageURL, terminals));
    }

    // add an entry to imageCache for each <imageData> element
    for (Enumeration enum_imgs = xmlLabConfiguration.getChildren("imageData");
	 enum_imgs.hasMoreElements(); )
    {
      // get next <imageData> element
      Element xmlImageData = (Element) enum_imgs.nextElement();

      String url = xmlImageData.getAttributeValue("url");
      String imageData = xmlImageData.getData();

      lc.imageCache.put(url, imageData);
    }

    lc.setups = new Setup[vec_setups.size()];
    vec_setups.copyInto(lc.setups);

    return lc;
  }

  // This method is not currently needed by the Weblab client:
  //
  // public final String toXMLString()

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitLabConfiguration(this);
  }

} // end class LabConfiguration
