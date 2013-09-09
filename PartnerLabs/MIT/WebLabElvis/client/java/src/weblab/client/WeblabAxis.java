package weblab.client;

// JAVA 1.1 COMPLIANT

import java.math.BigDecimal;
import java.util.Enumeration;
import java.util.Observable;
import java.util.Observer;

import weblab.util.ChangeTrackingObservable;
import weblab.util.EngMath;

/**
 * Implementation of weblab.graphing.Axis that pulls data values from
 * the ExperimentResult associated with a WeblabClient.
 */
public class WeblabAxis extends weblab.graphing.Axis implements Observer
{
  /**
   * Flag value to inform Observers that the set of choices for the
   * variable to be graphed has changed.
   */
  public static final String VARIABLE_CHOICES_CHANGE = "variableChoices";

  private final WeblabClient theWeblabClient;
  private ExperimentResult theResult;

  private String variable;



  /**
   * Constructs a new WeblabAxis for the specified WeblabClient.
   */
  public WeblabAxis(WeblabClient wc, String name)
  {
    super(name);

    this.theWeblabClient = wc;
    this.theResult = theWeblabClient.getExperimentResult();

    theWeblabClient.addObserver(this);
  }



  /**
   * Returns the current variable being graphed on this axis (or null
   * for no variable).
   */
  public final String getVariable()
  {
    return this.variable;
  }



  /**
   * Sets the variable to be graphed on this axis (null means don't
   * graph any variable).
   */
  public final void setVariable(String variable)
  {
    if (variable == null)
    {
      if (this.variable != null)
      {
	this.variable = null;
	this.setChanged();
      }
    }
    else if (! variable.equals(this.variable) &&
	     theResult.containsVariable(variable))
    {
      this.variable = variable;
      this.setChanged();

      // re-autoscale if appropriate
      if (this.getContinuousAutoscale())
	this.autoscale();
    }
  }



  /**
   * Returns an enumeration of all of the possible variables that this
   * can be set to graph.
   *
   * @return Enumeration of String
   */
  public final Enumeration getVariableChoices()
  {
    return theResult.getVariableNames();
  }



  /**
   * Returns the preferred number of divisions for graphing this axis.
   * Currently this is fixed at 10 for LINEAR_SCALE, and equal to the
   * number of decades between min and max for LOG_SCALE.
   */
  public int getPreferredDivs()
  {
    switch(this.getScale())
    {
    case LINEAR_SCALE:
      return 10;

    case LOG_SCALE:
      int divs = EngMath.log10ceiling(this.getMax()) -
	EngMath.log10floor(this.getMin());
      if (divs < 2)
	divs = 2;
      return divs;

    default:
      throw new Error("illegal scale: " + this.getScale());
    }
  }



  public final BigDecimal[] getData()
  {
    if (this.variable == null)
      return new BigDecimal[0];
    else
      return theResult.getVariableData(this.variable);
  }



  public final String getUnits()
  {
    if (this.variable == null)
      return null;
    else
      return theResult.getVariableUnits(this.variable);
  }



  /**
   * Handle updates from Observables
   */
  public final void update(Observable o, Object arg)
  {
    // When the WeblabClient changes the ExperimentResult, (a) update
    // it and (b) the variable choices change
    if (o == theWeblabClient)
    {
      if (ChangeTrackingObservable.containsChange
	  (arg, WeblabClient.EXPERIMENT_RESULT_CHANGE))
      {
	theResult = theWeblabClient.getExperimentResult();

	this.setChanged(VARIABLE_CHOICES_CHANGE);

	// if currently selected variable does not exist in the new
	// result, select no variable
	if (this.variable != null &&
	    ! theResult.containsVariable(this.variable))
	  this.setVariable(null);

	// re-autoscale if appropriate
	if (this.getContinuousAutoscale())
	  this.autoscale();

	// notify observers that this has changed
	this.notifyObservers();
      }
    }
  }

} // end class WeblabAxis
