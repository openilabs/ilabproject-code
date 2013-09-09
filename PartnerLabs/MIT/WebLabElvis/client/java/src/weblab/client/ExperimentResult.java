package weblab.client;

// JAVA 1.1 COMPLIANT

import java.util.Vector;
import java.util.Enumeration;
import java.util.StringTokenizer;

import java.math.BigDecimal;

import weblab.xml.Parser;
import weblab.xml.Element;
import weblab.xml.InvalidXMLException;

import weblab.util.EngMath;
import weblab.util.EngValue;
import weblab.util.EngUnits;


/**
 * Contains information about the outcome of an experiment, obtained
 * from the lab server.  An ExperimentResult consists of a collection
 * of data variables (each with a name, units, and data) and an
 * optional temperature.
 *
 * ExperimentResult is immutable.
 */
public class ExperimentResult
{
  private Temperature temp;

  // three parallel vectors to store <name, units, data> triples
  private final Vector varNames; // contains String
  private final Vector varUnits; // contains String
  private final Vector varData;  // contains BigDecimal[]



  /**
   * Constructs a new "empty" ExperimentResult with no variables.
   */
  public ExperimentResult()
  {
    this.varNames = new Vector();
    this.varUnits = new Vector();
    this.varData = new Vector();
  }



  /**
   * Returns the Temperature from this result, or null if the result
   * did not include a temperature.
   */
  public final Temperature getTemperature()
  {
    return this.temp;
  }



  /**
   * Returns true iff the result contains a variable with the
   * specified name.
   */
  public final boolean containsVariable(String name)
  {
    return varNames.contains(name);
  }



  /**
   * Returns an Enumeration of the names of the data variables
   * contained in this result.
   *
   * @return Enumeration of String.
   */
  public final Enumeration getVariableNames()
  {
    return varNames.elements();
  }



  /**
   * Returns the units of the variable with the specified name, or
   * null if the result does not contain a variable with the specified
   * name.
   */
  public final String getVariableUnits(String variableName)
  {
    if (! this.containsVariable(variableName))
	return null;

    return (String) varUnits.elementAt(varNames.indexOf(variableName));
  }



  /**
   * Returns the data for the variable with the specified name, or
   * null if the result does not contain a variable with the specified
   * name.
   */
  public final BigDecimal[] getVariableData(String variableName)
  {
    if (! this.containsVariable(variableName))
      return null;

    return (BigDecimal[])
      ((BigDecimal[]) varData.elementAt(varNames.indexOf(variableName)))
      .clone();
  }



  /**
   * Generates a new ExperimentResult from the specified XML string
   * (which was presumably obtained from the lab server).
   *
   * @throws InvalidExperimentResultException if xmlString is not a
   * valid XML experiment result
   */
  public final static ExperimentResult
    parseXMLExperimentResult(String xmlString)
    throws InvalidExperimentResultException
  {
    Element xmlExperimentResult;

    try {
      xmlExperimentResult = Parser.parse(xmlString);
    }
    catch (InvalidXMLException ex) {
      throw new InvalidExperimentResultException
	(ex.getMessage());
    }

    if (! xmlExperimentResult.getName().equals("experimentResult"))
    {
      throw new InvalidExperimentResultException
	("illegal root element " + xmlExperimentResult.getName());
    }

    ExperimentResult er = new ExperimentResult();

    // read optional <temp units='X'> if present
    Element xmlTemp = xmlExperimentResult.getChild("temp");
    if (xmlTemp != null)
    {
      BigDecimal value = EngMath.parseBigDecimal(xmlTemp.getData());
      String units = xmlTemp.getAttributeValue("units");
      er.temp = new Temperature(value, units);
    }

    // read <datavector name='X' units='X'>
    for (Enumeration enum_dvs = xmlExperimentResult
	   .getChildren("datavector");
	 enum_dvs.hasMoreElements(); )
    {
      // get next <datavector> element
      Element xmlDatavector = (Element) enum_dvs.nextElement();

      String name = xmlDatavector.getAttributeValue("name");

      String units = xmlDatavector.getAttributeValue("units");

      // String representing list of values (separated by spaces)
      String stringData = xmlDatavector.getData();
      // Split up into actual list of numbers
      Vector vec_decimalData = new Vector();
      StringTokenizer st = new StringTokenizer(stringData, " ");
      while(st.hasMoreTokens())
      {
	BigDecimal nextPoint = EngMath.parseBigDecimal(st.nextToken());
	vec_decimalData.addElement(nextPoint);
      }
      BigDecimal[] decimalData = new BigDecimal[vec_decimalData.size()];
      vec_decimalData.copyInto(decimalData);

      er.varNames.addElement(name);
      er.varUnits.addElement(units);
      er.varData.addElement(decimalData);
    }

    return er;
  }



  /**
   * Returns a string representation of this in CSV (comma-separated
   * value) format, suitable for displaying to the user or exporting
   * to a spreadsheet program.
   */
  public final String toCSVString()
  {
    StringBuffer sb = new StringBuffer();

    // write temperature line if temperature is known
    if (temp != null) {
      sb.append("T(" + temp.getUnits() + ")=, " + temp.getValue() + "\n");
    }

    int nvars = varNames.size();
    if (nvars == 0)
      sb.append("no variable data");

    // write row with variable names
    for(int i = 0; i < nvars; i++)
    {
      // write next variable name
      sb.append((String) varNames.elementAt(i));
	// if not the last one, write a comma and space
      if (i < nvars - 1)
	sb.append(", ");
    }
    sb.append("\n");

    // write row with variable units
    for(int i = 0; i < nvars; i++)
    {
      // write next variable unit string
      sb.append((String) varUnits.elementAt(i));
	// if not the last one, write a comma and space
      if (i < nvars - 1)
	sb.append(", ");
    }
    sb.append("\n");

    // find biggest size of any variable's data (in case they are
    // different lengths)
    int biggestSize = 0;
    for(int i = 0; i < nvars; i++)
    {
      // find size of ith variable data
      int newSize = ((BigDecimal[]) varData.elementAt(i)).length;
      // update biggest size if necesssary
      if (newSize > biggestSize)
	biggestSize = newSize;
    }

    // write the data
    for(int x = 0; x < biggestSize; x++)
    {
      // write the row for the xth data point
      for(int i = 0; i < nvars; i++)
      {
	// retrieve data for ith variable
	BigDecimal[] data = (BigDecimal[]) varData.elementAt(i);
	
	// write xth element of ith variable, if it exists
	if (x < data.length)
	  sb.append(new EngValue(data[x], new EngUnits()).toString());
	
	  // if not the last variable, write a comma and space
	if (i < nvars - 1)
	  sb.append(", ");
      }
      sb.append("\n");
    }

    return sb.toString();
  }


  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitExperimentResult(this);
  }

} // end class ExperimentResult
