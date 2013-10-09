/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.util.StringTokenizer;

import java.util.Collections;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

import java.math.BigDecimal;

import weblab.toolkit.graphing.Variable;

import weblab.toolkit.util.EngMath;
import weblab.toolkit.util.EngValue;
import weblab.toolkit.util.EngUnits;

import weblab.toolkit.xml.Parser;
import weblab.toolkit.xml.Element;
import weblab.toolkit.xml.InvalidXMLException;



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

  // unmodifiable, contains ExperimentResult.WeblabVariable
  private List vars;


  /**
   * Constructs a new "empty" ExperimentResult with no variables and
   * no temperature.
   */
  public ExperimentResult()
  {
    this.vars = Collections.EMPTY_LIST;
    this.temp = null;
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
   * Returns an unmodifiable List of the data variables contained in
   * this result.
   *
   * @return List of Variable.
   */
  public final List getVariables()
  {
    return vars;
  }



  /**
   * Returns the Variable from this result that has the specified
   * name, or null if the result does not contain a variable with the
   * specified name.
   */
  public final Variable getVariable(String variableName)
  {
    for (int i = 0, n = vars.size(); i < n; i++)
    {
      Variable v = (Variable) vars.get(i);
      if (v.getName().equals(variableName))
	return v;
    }

    return null;
  }



  /**
   * Returns true iff the result contains a variable with the
   * specified name.
   */
  public final boolean containsVariable(String name)
  {
    return (this.getVariable(name) != null);
  }



  /**
   * If the Variable <code>v</code> came from an ExperimentResult,
   * returns the ExperimentResult that it came from, otherwise returns
   * null.
   */
  public final static ExperimentResult
    getExperimentResultForVariable(Variable v)
  {
    if (! (v instanceof WeblabVariable))
      return null;

    WeblabVariable wv = (WeblabVariable) v;
    return wv.getExperimentResult();
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

    // parse XML and make sure the root element is experimentResult
    try
    {
      xmlExperimentResult = Parser.parse(xmlString);

      if (! xmlExperimentResult.getName().equals("experimentResult"))
	throw new InvalidExperimentResultException
	  ("illegal root element " + xmlExperimentResult.getName());
    }
    catch (InvalidXMLException ex) {
      throw new InvalidExperimentResultException(ex);
    }

    // construct ExperimentResult based on XML content
    try
    {
      ExperimentResult er = new ExperimentResult();

      // read optional <temp units='X'>N</temp>
      Element xmlTemp = xmlExperimentResult.getChild("temp");
      if (xmlTemp != null)
      {
	String units = xmlTemp.getRequiredAttributeValue("units");
	BigDecimal value =
	  EngMath.parseBigDecimal(xmlTemp.getRequiredData());
	er.temp = new Temperature(value, units);
      }

      List newVars = new ArrayList();

      // read <datavector name='X' units='X'>N N N N N ...  and
      // generate WeblabVariable objects
      for (Iterator i = xmlExperimentResult.getChildren("datavector")
	     .iterator();
	   i.hasNext(); )
      {
	Element xmlDatavector = (Element) i.next();

	String name = xmlDatavector.getRequiredAttributeValue("name");
	if (name.equals(""))
	  throw new InvalidExperimentResultException
	    ("datavector name attribute may not be blank");

	String units = xmlDatavector.getRequiredAttributeValue("units");

	List decimalData = new ArrayList();

	// read space-separated list of BigDecimal values
	String stringData = xmlDatavector.getData();
	for (StringTokenizer st = new StringTokenizer(stringData, " ");
	     st.hasMoreTokens(); )
	{
	  decimalData.add(EngMath.parseBigDecimal(st.nextToken()));
	}

	// ... </datavector>
	newVars.add
	  (new WeblabVariable
	   (er, name, units, Collections.unmodifiableList(decimalData)));
      }

      er.vars = Collections.unmodifiableList(newVars);

      return er;
    }
    catch (InvalidXMLException ex) {
      // it's not really invalid XML per se when this comes from
      // Element.getRequiredFOO, so just copy the message
      throw new InvalidExperimentResultException(ex.getMessage());
    }
    catch(NumberFormatException ex) {
      throw new InvalidExperimentResultException(ex);
    }
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

    int nvars = vars.size();
    if (nvars == 0)
      sb.append("no variable data");

    // write row with variable names
    for(int i = 0; i < nvars; i++)
    {
      // write next variable name
      Variable v = (Variable) vars.get(i);
      sb.append(v.getName());
	// if not the last one, write a comma and space
      if (i < nvars - 1)
	sb.append(", ");
    }
    sb.append("\n");

    // write row with variable units
    for(int i = 0; i < nvars; i++)
    {
      // write next variable unit string
      Variable v = (Variable) vars.get(i);
      sb.append(v.getUnits());
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
      Variable v = (Variable) vars.get(i);
      int newSize = v.getData().size();
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
	Variable v = (Variable) vars.get(i);
	List data = v.getData();
	
	// write xth element of ith variable, if it exists
	if (x < data.size())
	  sb.append(new EngValue((BigDecimal) data.get(x), new EngUnits())
		    .toString());
	
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



  // a Variable that comes from a particular ExperimentResult
  private static class WeblabVariable implements Variable
  {
    private ExperimentResult myResult;
    private String name;
    private String units;
    private List data; // unmodifiable, contains BigDecimal

    // note: data should be an unmodifiable List of BigDecimal
    public WeblabVariable(ExperimentResult expResult, String name,
			  String units, List data)
    {
      this.myResult = expResult;
      this.name = name;
      this.units = units;
      this.data = data;
    }

    public String getName()
    {
      return name;
    }

    public String getUnits()
    {
      return units;
    }

    public List getData()
    {
      return data;
    }

    public List getDataSet()
    {
      return myResult.getVariables();
    }

    public ExperimentResult getExperimentResult()
    {
      return myResult;
    }

    // make WeblabVariables suitable for displaying in JComboBoxes, etc
    public String toString()
    {
      return name;
    }
  } // end nested class WeblabVariable

} // end class ExperimentResult
