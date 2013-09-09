package weblab.client;

// JAVA 1.1 COMPLIANT

import java.util.Enumeration;

/**
 * Traverses an ExperimentSpecification looking for VAR1, VAR2, and
 * VAR1P.  Records information about them in public member fields
 * which can be subsequently read back.
 */
public class ExperimentSpecificationVisitor extends DefaultVisitor
{
  // traversal state
  private Instrument currentInstrument;
  private String currentSourceName;

  // RESULTS FIELDS

  /**
   * An Instrument whose source function is a WAVEFORMFunction, or null if none
   * has been found.
   */
  public Instrument waveformInstrument;

  /**
   * A variable name (VName or IName) that corresponds directly to a
   * WAVEFORMFunction, or null if none has been found.
   */
  public String waveformName;

  /**
   * A WAVEFORMFunction defined by the experiment specification, or null
   * if none has been found.
   */
  public WAVEFORMFunction waveformFunction;


  /**
   * An Instrument whose source function is a VAR1Function, or null if none
   * has been found.
   */
  public Instrument var1Instrument;

  /**
   * A variable name (VName or IName) that corresponds directly to a
   * VAR1Function, or null if none has been found.
   */
  public String var1Name;

  /**
   * A VAR1Function defined by the experiment specification, or null
   * if none has been found.
   */
  public VAR1Function var1Function;


  /**
   * An Instrument whose source function is a VAR2Function, or null if none
   * has been found.
   */
  public Instrument var2Instrument;

  /**
   * A variable name (VName or IName) that corresponds directly to a
   * VAR2Function, or null if none has been found.
   */
  public String var2Name;

  /**
   * A VAR2Function defined by the experiment specification, or null
   * if none has been found.
   */
  public VAR2Function var2Function;


  /**
   * An Instrument whose source function is a VAR1PFunction, or null if none
   * has been found.
   */
  public Instrument var1pInstrument;

  /**
   * A variable name (VName or IName) that corresponds directly to a
   * VAR1PFunction, or null if none has been found.
   */
  public String var1pName;

  /**
   * A VAR1PFunction defined by the experiment specification, or null
   * if none has been found.
   */
  public VAR1PFunction var1pFunction;



  /**
   * Constructs a new ExperimentSpecificationVisitor.
   */
  public ExperimentSpecificationVisitor()
  {
    // do nothing
  }

  public final void visitExperimentSpecification(ExperimentSpecification e)
  {
    Enumeration instruments_enum = e.getInstruments();
    while (instruments_enum.hasMoreElements())
    {
      currentInstrument = (Instrument) instruments_enum.nextElement();
      currentInstrument.accept(this);
    }
  }

  public final void visitFGEN(FGEN f)
  {
    currentSourceName = f.getVName();
    f.getFunction().accept(this);
  }

  public final void visitWAVEFORMFunction(WAVEFORMFunction f)
  {
    waveformInstrument = currentInstrument;
    waveformName = currentSourceName;
    waveformFunction = f;
  }
  
  public final void visitVAR1Function(VAR1Function f)
  {
    var1Instrument = currentInstrument;
    var1Name = currentSourceName;
    var1Function = f;
  }

  public final void visitVAR2Function(VAR2Function f)
  {
    var2Instrument = currentInstrument;
    var2Name = currentSourceName;
    var2Function = f;
  }

  public final void visitVAR1PFunction(VAR1PFunction f)
  {
    var1pInstrument = currentInstrument;
    var1pName = currentSourceName;
    var1pFunction = f;
  }
} // end class ExperimentSpecificationVisitor
