package weblab.client;

// JAVA 1.1 COMPLIANT

import weblab.client.serverInterface.Server;
import weblab.client.serverInterface.ServerException;

import weblab.xml.Parser;

import weblab.util.EngUnits;

import java.util.Enumeration;

/**
 * A facade through which the UI layer accesses all functional
 * operations of the client.  The WeblabClient manages a Server, a
 * LabConfiguration, an ExperimentSpecification, an ExperimentResult,
 * and three instances of WeblabAxis (for the X, Y1, and Y2 axes).
 *
 * Important note on Observable behavior: UNLIKE all other classes in
 * the weblab.client package, WeblabClient proactively calls
 * notifyObservers on itself when it considers itself to have changed!
 */
public class WeblabClient extends weblab.util.ChangeTrackingObservable
{
  /**
   * The default name for new experiment configuration not loaded from the server.
   */
  public static final String UNTITLED = "Untitled";

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
   * Flag value to inform Observers that there is a new Experiment
   * Specification associated with this.
   */
  public static final String EXPERIMENT_SPECIFICATION_CHANGE = "experimentSpecification";


  // Types of EngUnits used in this client (defined once globally)
  public static final EngUnits VOLTAGE_UNITS =
    new EngUnits("V", 0, -15);
  public static final EngUnits FREQUENCY_UNITS =
    new EngUnits("Hz", 3, 0);
  public static final EngUnits TIME_UNITS =
    new EngUnits("sec", 0, -15);


  private Server theServer;

  private ExperimentSpecification theExpSpec;
  private LabConfiguration theLabConf;
  private ExperimentResult theResult;

  private WeblabAxis xAxis, y1Axis, y2Axis;


  /**
   * Creates a new WeblabClient with the specified Server, a new blank
   * ExperimentSpecification, an empty LabConfiguration, and an empty
   * ExperimentResult.
   */
  public WeblabClient(Server weblabServer)
  {
    theServer = weblabServer;
    theExpSpec = new ExperimentSpecification(UNTITLED);
    theLabConf = new LabConfiguration();
    theResult = new ExperimentResult();

    xAxis = new WeblabAxis(this, "X");
    xAxis.setContinuousAutoscale(true);
    y1Axis = new WeblabAxis(this, "Y1");
    y1Axis.setContinuousAutoscale(true);
    y2Axis = new WeblabAxis(this, "Y2");
    y2Axis.setContinuousAutoscale(true);
  }



  /**
   * Returns the x axis.
   */
  public final WeblabAxis getXAxis()
  {
    return xAxis;
  }



  /**
   * Returns the y1 axis.
   */
  public final WeblabAxis getY1Axis()
  {
    return y1Axis;
  }



  /**
   * Returns the y2 axis.
   */
  public final WeblabAxis getY2Axis()
  {
    return y2Axis;
  }



  /**
   * Returns the current ExperimentSpecification.
   */
  public final ExperimentSpecification getExperimentSpecification()
  {
    return theExpSpec;
  }



  /**
   * Returns the current LabConfiguration.
   */
  public final LabConfiguration getLabConfiguration()
  {
    return theLabConf;
  }



  /**
   * Returns the current ExperimentResult.
   */
  public final ExperimentResult getExperimentResult()
  {
    return theResult;
  }



  /**
   * Loads a fresh lab configuration from the server.  If an experiment setup was
   * selected before, deselects it.
   */
  public final void loadLabConfiguration()
    throws ServerException, InvalidLabConfigurationException
  {
    String xmlLC = theServer.getLabConfiguration();

    // log to console
    System.out.println("LabConfiguration retrieved:");
    System.out.println(Parser.xmlPrettyPrint(xmlLC) + "\n");

    theLabConf = LabConfiguration.parseXMLLabConfiguration(xmlLC);
    // select no setup
    this.selectSetup(-1, false);
    this.setChanged(LAB_CONFIGURATION_CHANGE);
    this.notifyObservers();
  }



  /**
   * Selects the experiment setup with the specified index (in the array of
   * setups from the lab configuration) by assigning that setup to
   * the experiment specification.  -1 means select no setup.
   *
   * If a new setup is selected, also resets the name of the current
   * ExperimentSpeciication to WeblabClient.UNTITLED.
   *
   * @param requestConfirmation whether to throw ConfirmationRequests
   * @throws ConfirmationRequest if the current  defines instruments
   * that are not connected to the new setup [only thrown if
   * requestConfirmation is true]
   */
  public final void selectSetup(int index, boolean requestConfirmation)
    throws ConfirmationRequest
  {
    if (index == -1)
    {
      theExpSpec.setSetup(null);
      theExpSpec.setName(UNTITLED);
    }
    else
    {
      Setup s = theLabConf.getSetups()[index];

      // check compatibility
      if (requestConfirmation && !theExpSpec.checkSetupCompatibility(s))
    	  throw new ConfirmationRequest();

      // proceed
      if (! s.equals(theExpSpec.getSetup()))
    	  theExpSpec.setName(UNTITLED);
      theExpSpec.setSetup(s);
    }
    // notify observers that theExpSpec may have changed
    theExpSpec.notifyObservers();
  }



  /**
   * Creates a new blank experiment specification for the currently
   * selected setup.
   */
  public final void reset()
  {
    Setup s = theExpSpec.getSetup();
    theExpSpec = new ExperimentSpecification(UNTITLED);
    theExpSpec.setSetup(s);
    theExpSpec.notifyObservers();
    this.setChanged(EXPERIMENT_SPECIFICATION_CHANGE);
    this.notifyObservers();
  }



  /**
   * Creates a new experiment specification (for the currently
   * selected setup) based on the specified XML string.
   *
   * @param xmlExperimentSpecification the XML experiment
   * specification string to be used as the new experiment configuration
   * @param requestConfirmation whether to throw ConfirmationRequests
   * @throws InvalidExperimentSpecificationException if
   * xmlExperimentSpecification is not a valid XML experiment
   * specification
   * @throws ConfirmationRequest if the new experiment configuration defines instruments that
   * are not connected to the currently selected setup [only thrown
   * if requestConfirmation is true]
   */
  public final void specifyExpConfiguration(String xmlExperimentSpecification,
				 boolean requestConfirmation)
    throws InvalidExperimentSpecificationException, ConfirmationRequest
  {
	  // parse the XML
	  ExperimentSpecification newExpSpec =
      ExperimentSpecification.parseXMLExperimentSpecification
      (xmlExperimentSpecification);

	  // obtain current selected setup
	  Setup s = theExpSpec.getSetup();

	  // check compatibility
	  if (requestConfirmation && !newExpSpec.checkSetupCompatibility(s))
		  throw new ConfirmationRequest();

	  // proceed
	  newExpSpec.setSetup(s);
	  newExpSpec.setName(UNTITLED);
	  newExpSpec.notifyObservers();
	  this.theExpSpec = newExpSpec;
	  this.setChanged(EXPERIMENT_SPECIFICATION_CHANGE);
	  this.notifyObservers();
  }


  /**
   * Returns an enumeration of the names of existing saved setups.
   *
   * @return Enumeration of String
   * @throws ServerException if a problem occurs
   */
  public final Enumeration getSavedExpConfigurationNames()
    throws ServerException
  {
    return theServer.getSavedExpConfigurationNames();
  }

  /**
   * Loads an experiment configuration from the server.
   *
   * @param name the name of the configuration to load
   * @param requestConfirmation whether to throw ConfirmationRequests
   * @throws ServerException if a problem occurs communicating with
   * the server
   * @throws InvalidExperimentSpecificationException if the configuration
   * received from the server is not a valid XML experiment
   * specification
   * @throws ConfirmationRequest if the new experiment configuration defines instruments that
   * are not connected to the currently selected setup [only thrown
   * if requestConfirmation is true]
   */
  public final void loadExpConfiguration(String name, boolean requestConfirmation)
    throws ServerException, InvalidExperimentSpecificationException,
	   ConfirmationRequest
  {
    String xmlES = theServer.loadExpConfiguration(name);
    this.specifyExpConfiguration(xmlES, requestConfirmation);
    theExpSpec.setName(name);
    theExpSpec.notifyObservers();
  }



  /**
   * Saves an experiment configuration to the server.
   *
   * @param name the name under which to save the configuration (if a configuration of
   * this name already exists, it will be overwritten)
   * @throws ServerException if a problem occurs
   */
  public final void saveExpConfiguration(String name)
    throws ServerException
  {
    String xmlES = theExpSpec.toXMLString();
    theServer.saveExpConfiguration(name, xmlES);
    theExpSpec.setName(name);
    theExpSpec.notifyObservers();
  }



  /**
   * Deletes an experiment configuration from the server.
   *
   * @param name the name of the experiment configuratgion  to delete
   * @throws ServerException if a problem occurs
   */
  public final void deleteExpConfiguration(String name)
    throws ServerException
  {
    theServer.deleteExpConfiguration(name);
  }



  /**
   * Submits an experiment specification for execution, waits for
   * execution to finish, and then terminates.  Does not currently
   * perform any client-side validation.
   *
   * Also implements some heuristics for choosing which variables to
   * graph.
   *
   * @throws ServerException if a problem occurs
   * @throws InvalidExperimentResultException if the result returned
   * by the server is invalid
   */
  public final void execute()
    throws ServerException, InvalidExperimentResultException
  {
    // find out which variables are var1, var2, var1p or Waveform
    ExperimentSpecificationVisitor ev = new ExperimentSpecificationVisitor();
    theExpSpec.accept(ev);

    // create and execute the measurement specification
    String xmlExpSpec = theExpSpec.toXMLString();
    
    System.out.println("The experiment specification is:");
    System.out.println(Parser.xmlPrettyPrint(xmlExpSpec) + "\n");

    String xmlResult = theServer.execute(xmlExpSpec);
    
    
    // log to console
    System.out.println("ExperimentResult retrieved:");
    System.out.println(Parser.xmlPrettyPrint(xmlResult) + "\n");

    // clear the current results and store the new ones
    theResult = ExperimentResult.parseXMLExperimentResult(xmlResult);
    this.setChanged(EXPERIMENT_RESULT_CHANGE);
    this.notifyObservers();

    //
    // graphing heuristics for the user's convenience
    //

    // if x axis has no selected variable, select TIME if WAVEFORM exists. Otherwise, try to select VAR1.  If
    // this is not possible, select any variable.
    if (xAxis.getVariable() == null)
    {
      String xVar = null;
      if (ev.waveformName != null && theResult.containsVariable(ev.waveformName)){
    	  xVar = "TIME";
      }else{
	      if (ev.var1Name != null && theResult.containsVariable(ev.var1Name))
	    	  xVar = ev.var1Name;
	      else
	      {
	    	  Enumeration varNames = theResult.getVariableNames();
	    	  if (varNames.hasMoreElements())
	    		  xVar = (String) varNames.nextElement();
	      }
      }
      xAxis.setVariable(xVar);
      xAxis.notifyObservers();
    }

    // if y1 axis has no selected variable, select WAVEFORM if it exists. 
    // Otherwise, try to select any variable
    // that is not VAR2, VAR1P, or the x variable.  If this is not
    // possible, select any variable other than the x variable.
    if (y1Axis.getVariable() == null)
    {
      String y1Var = null;
      String backupChoice = null;
      if (ev.waveformName != null && theResult.containsVariable(ev.waveformName)){
    	  y1Var = ev.waveformName;
      }else{
      
	      String xVar = xAxis.getVariable();
	      	
	      Enumeration varNames = theResult.getVariableNames();
	      while (y1Var == null && varNames.hasMoreElements())
	      {
	    	  String next = (String) varNames.nextElement();
	    	  if (! next.equals(xVar))
	    	  {
	    		  if (! next.equals(ev.var2Name) && ! next.equals(ev.var1pName))
	    		  {
	    			  // found one
	    			  y1Var = next;
	    		  }
	    		  else
	    		  {
	    			  // not our first choice, but an acceptable backup choice
	    			  // since it's not xVar
	    			  backupChoice = next;
	    		  }
	    	  }
	      }
	
	      if (y1Var == null && backupChoice != null)
	    	  y1Var = backupChoice;
	
	      y1Axis.setVariable(y1Var);
	      y1Axis.notifyObservers();
	    }
    }
  }



  /**
   * Indicates the status of the execute() operation currently in
   * progress.
   *
   * @return a human-readable status summary, suitable for displaying
   * in a dialog box.
   */
  public final String getExecutionStatus()
  {
    return theServer.getExecutionStatus();
  }



  /**
   * Provides an estimate of the remaining time before the execute()
   * operation currently in progress will be finished.
   *
   * @return a human-readable time estimate, suitable for displaying
   * in a dialog box.
   */
  public final String getExecutionEstimatedTimeRemaining()
  {
    return theServer.getExecutionEstimatedTimeRemaining();
  }



  /**
   * Cancels the execute() operation currently in progress.
   */
  public final void cancelExecution()
    throws ServerException
  {
    theServer.cancelExecution();
  }



  /**
   * Returns the current results in CSV (comma-separated value)
   * format, suitable for displaying to the user or exporting to a
   * spreadsheet program.
   */
  public final String viewData()
  {
    return theResult.toCSVString();
  }



  /**
   * Clears the current ExperimentResult.
   */
  public final void clearResults()
  {
    theResult = new ExperimentResult();
    this.setChanged(EXPERIMENT_RESULT_CHANGE);
    this.notifyObservers();
  }

} // end class WeblabClient
