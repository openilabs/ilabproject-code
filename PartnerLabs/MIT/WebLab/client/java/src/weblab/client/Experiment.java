/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.util.Iterator;
import java.util.Date;

import java.text.DateFormat;

import weblab.toolkit.graphing.Graph;
import weblab.toolkit.graphing.Variable;
import weblab.toolkit.graphing.ConnectPattern;

import weblab.toolkit.util.ChangeTrackingObservable;

/**
 * Encapsulates a Microelectronics Weblab experiment, which includes
 * an ExperimentSpecification, a LabConfiguration (representing the
 * state of the Lab Server at the time of execution), and an
 * ExperimentResult.
 *
 * note: it is expected (though not strictly enforced in the code)
 * that when the ExperimentSpecification of this contains a non-null
 * Device, that Device should exist in the LabConfiguration of this.
 * If you want to select a Device from a different LabConfiguration,
 * first select a null device, then update the LabConfiguration of
 * this, and finally select the new Device.
 */
public class Experiment extends ChangeTrackingObservable implements Cloneable
{
  /**
   * Flag value to inform Observers that there is a new Experiment
   * Specification associated with this.
   */
  public static final String EXPERIMENT_SPECIFICATION_CHANGE =
    "experimentSpecification";

  /**
   * Flag value to inform Observers that there is a new Lab
   * Configuration associated with this.
   */
  public static final String LAB_CONFIGURATION_CHANGE = "labConfiguration";

  /**
   * Flag value to inform Observers that there is a new Experiment
   * Result associated with this.
   */
  public static final String EXPERIMENT_RESULT_CHANGE = "experimentResult";

  /**
   * Flag value to inform Observers that the experimentID and/or
   * submissionTime of this has changed.
   */
  public static final String EXPERIMENT_INFO_CHANGE = "experimentID";


  private ExperimentSpecification expSpec;
  private LabConfiguration labConf;
  private ExperimentResult expResult;
  private Integer experimentID;
  private Date submissionTime;

  private static DateFormat dateFormatter =
    DateFormat.getDateTimeInstance(DateFormat.SHORT, DateFormat.MEDIUM);


  /**
   * Creates a new Experiment with a new blank
   * ExperimentSpecification, an empty LabConfiguration, an empty
   * ExperimentResult, and a null experimentID.
   */
  public Experiment(String setupName)
  {
    expSpec = new ExperimentSpecification(setupName);
    labConf = new LabConfiguration();
    expResult = new ExperimentResult();
    experimentID = null;
  }



  /**
   * Returns the ExperimentSpecification of this.
   */
  public ExperimentSpecification getExperimentSpecification()
  {
    return expSpec;
  }



  /**
   * Sets the ExperimentSpecification of this.
   *
   * @param expSpec the new ExperimentSpecification for this.
   */
  public void setExperimentSpecification(ExperimentSpecification expSpec)
  {
    if (! this.expSpec.equals(expSpec))
    {
      this.expSpec = expSpec;
      this.setChanged(EXPERIMENT_SPECIFICATION_CHANGE);
    }
  }



  /**
   * Returns the LabConfiguration of this.
   */
  public LabConfiguration getLabConfiguration()
  {
    return labConf;
  }



  /**
   * Sets the LabConfiguration of this.
   *
   * @param labConf the new LabConfiguration for this.
   */
  public void setLabConfiguration(LabConfiguration labConf)
  {
    if (! this.labConf.equals(labConf))
    {
      this.labConf = labConf;
      this.setChanged(LAB_CONFIGURATION_CHANGE);
    }
  }



  /**
   * Returns the ExperimentResult of this.
   */
  public ExperimentResult getExperimentResult()
  {
    return expResult;
  }



  /**
   * Sets the ExperimentResult of this.
   *
   * @param expResult the new ExperimentResult for this.
   */
  public void setExperimentResult(ExperimentResult expResult)
  {
    if (! this.expResult.equals(expResult))
    {
      this.expResult = expResult;
      this.setChanged(EXPERIMENT_RESULT_CHANGE);
    }
  }



  /**
   * Returns the experimentID of this Experiment (may be null to
   * indicate that the Experiment has not yet been executed).
   */
  public Integer getExperimentID()
  {
    return experimentID;
  }



  /**
   * Sets the experimentID of this Experiment to the specified value
   * (may be null to indicate that the Experiment has not yet been
   * executed).
   */
  public void setExperimentID(Integer expID)
  {
    this.experimentID = expID;
    this.setChanged(EXPERIMENT_INFO_CHANGE);
  }



  /**
   * Returns the submission time of this Experiment (may be null to
   * indicate that the Experiment has not yet been executed).
   */
  public Date getSubmissionTime()
  {
    return submissionTime;
  }



  /**
   * Sets the submission time of this Experiment to the specified
   * value (may be null to indicate that the Experiment has not yet
   * been executed).
   */
  public void setSubmissionTime(Date submissionTime)
  {
    this.submissionTime = submissionTime;
    this.setChanged(EXPERIMENT_INFO_CHANGE);
  }



  /**
   * Creates a new blank experiment specification for the currently
   * selected device.
   *
   * @param name a name for the new ExperimentSpecification
   */
  public void resetSetup(String name)
  {
    Device d = expSpec.getDevice();
    expSpec = new ExperimentSpecification(name);
    expSpec.setDevice(d);
    expSpec.notifyObservers();
    this.setChanged(EXPERIMENT_SPECIFICATION_CHANGE);
  }



  /**
   * Creates a new ExperimentSpecification for this based on the
   * specified XML string, preserving the currently selected device.
   *
   * @param xmlExperimentSpecification the XML experiment
   * specification string to be used as the new setup
   * @param name a name for the new ExperimentSpecification
   * @param requestConfirmation whether to throw ConfirmationRequests
   * @throws InvalidExperimentSpecificationException if
   * xmlExperimentSpecification is not a valid XML experiment
   * specification
   * @throws ConfirmationRequest if the new setup defines ports that
   * are not connected to the currently selected device [only thrown
   * if requestConfirmation is true]
   */
  public void specifySetup(String xmlExperimentSpecification, String name,
			   boolean requestConfirmation)
    throws InvalidExperimentSpecificationException, ConfirmationRequest
  {
    // parse the XML (ignoring deviceID tag)
    ExperimentSpecification newExpSpec =
      ExperimentSpecification.parseXMLExperimentSpecification
      (xmlExperimentSpecification, null);

    // obtain current selected device
    Device d = expSpec.getDevice();

    // check compatibility
    if (requestConfirmation && !newExpSpec.checkDeviceCompatibility(d))
      throw new ConfirmationRequest();

    // proceed
    newExpSpec.setDevice(d);
    newExpSpec.setName(name);
    newExpSpec.notifyObservers();
    this.expSpec = newExpSpec;
    this.setChanged(EXPERIMENT_SPECIFICATION_CHANGE);
  }



  /**
   * Returns a ConnectPattern suitable for graphing the results of
   * this Experiment (based on the number of points in the VAR1 loop
   * of the ExperimentSpecification).
   */
  public ConnectPattern getConnectPattern()
  {
    ExperimentSpecificationVisitor ev = new ExperimentSpecificationVisitor();
    expSpec.accept(ev);

    if (ev.var1Function != null)
    {
      // number of points in the VAR1 loop
      final int var1points = ev.var1Function.calculatePoints();

      // Connect all pairs of successive points EXCEPT: every
      // (var1points) points, don't connect the next point.
      return new ConnectPattern() {
	  public final boolean isConnected(int n)
	  {
	    return (n % var1points != 0);
	  }
	};
    }
    else
    {
      return Graph.ALWAYS_CONNECT;
    }
  }



  /**
   * Automatically selects new X and Y variables for the Graph
   * <code>g</code> from the ExperimentResult of this, according to
   * the following heuristics (which depend on the
   * ExperimentSpecification of this).
   *
   * X: a Variable with the same name as the current X variable of
   * <code>g</code>, else VAR1, else any variable, else null.
   *
   * Y: a Variable with the same name as the current Y variable of
   * <code>g</code>, else any variable except VAR2, VAR1P, or the
   * variable we chose for X, else any variable except the X variable,
   * else null.
   *
   * modifies: <code>g</code>
   */
  public void autoselectGraphVariables(Graph g)
  {
    // find out which variables are var1, var2, var1p
    ExperimentSpecificationVisitor ev = new ExperimentSpecificationVisitor();
    expSpec.accept(ev);

    //
    // Choose a new X variable
    //

    Variable newX = null, oldX = g.getXVariable();

    // first choice: same name as oldX
    if (oldX != null)
      newX = expResult.getVariable(oldX.getName());

    // second choice: VAR1
    if (newX == null && ev.var1Name != null)
      newX = expResult.getVariable(ev.var1Name);

    // third choice: any variable
    if (newX == null)
    {
      Iterator vars = expResult.getVariables().iterator();
      if (vars.hasNext())
	newX = (Variable) vars.next();
    }

    // otherwise, give up and leave newX null

    g.setXVariable(newX);

    //
    // Choose a new Y variable
    //

    Variable newY = null, oldY = g.getYVariable();

    // first choice: same name as oldY
    if (oldY != null)
      newY = expResult.getVariable(oldY.getName());

    // second choice: any variable that is not VAR2, VAR1P, or the
    // variable we just chose for newX.
    //
    // third choice: any variable other than newX
    if (newY == null)
    {
      Variable thirdChoice = null;

      Iterator vars = expResult.getVariables().iterator();
      while (newY == null && vars.hasNext())
      {
	Variable v = (Variable) vars.next();
	if (! v.equals(newX))
	{
	  String name = v.getName();
	  if (! name.equals(ev.var2Name) && ! name.equals(ev.var1pName))
	  {
	    // found a second choice
	    newY = v;
	  }
	  else if (thirdChoice == null)
	  {
	    // not a second choice, but remember as an acceptable
	    // third choice since at least it's not newX
	    thirdChoice = v;
	  }
	}
      }

      // if we didn't find a second choice but did find a third
      // choice, use the third choice
      if (newY == null && thirdChoice != null)
	newY = thirdChoice;
    }

    // otherwise, give up and leave newY null

    g.setYVariable(newY);

    //
    // notify observers that g may have changed
    //

    g.notifyObservers();
  }


  public Object clone()
  {
    try
    {
      Experiment clone = (Experiment) super.clone();
      clone.deleteObservers();
      clone.expSpec = (ExperimentSpecification) expSpec.clone();
      return clone;
    }
    catch (CloneNotSupportedException ex)
    {
      throw new Error(ex);
    }
  }



  // make Experiments suitable for displaying in JComboBoxes, etc
  public String toString()
  {
    StringBuffer sb = new StringBuffer();

    if (experimentID != null)
    {
      sb.append("#");
      sb.append(experimentID.toString());

      if (submissionTime != null)
      {
	sb.append("  (");
	sb.append(dateFormatter.format(submissionTime));
	sb.append(")");
      }

      sb.append("  ");
    }

    sb.append(expSpec.getName());

    return sb.toString();
  }

} // end class Experiment
