package weblab.client;

// JAVA 1.1 COMPLIANT

/**
 * A Visitor with empty implementations of all visit methods.
 */
public class DefaultVisitor implements Visitor
{
  public void visitLabConfiguration(LabConfiguration lc)
  {
    // do nothing
  }

  public void visitSetup(Setup s)
  {
    // do nothing
  }

  public void visitTerminal(Terminal t)
  {
    // do nothing
  }

  public void visitExperimentSpecification(ExperimentSpecification e)
  {
    // do nothing
  }


  public void visitFGEN(FGEN f)
  {
    // do nothing
  }
  
  public void visitSCOPE(SCOPE s)
  {
    // do nothing
  }

  public void visitCONSFunction(CONSFunction f)
  {
    // do nothing
  }

  public void visitVAR1Function(VAR1Function f)
  {
    // do nothing
  }

  public void visitVAR2Function(VAR2Function f)
  {
    // do nothing
  }

  public void visitVAR1PFunction(VAR1PFunction f)
  {
    // do nothing
  }
  
  public void visitWAVEFORMFunction(WAVEFORMFunction f)
  {
    // do nothing
  }
  
  public void visitSAMPLINGFunction(SAMPLINGFunction f)
  {
    // do nothing
  }
  
  public void visitUserDefinedFunction(UserDefinedFunction udf)
  {
    // do nothing
  }

  public void visitExperimentResult(ExperimentResult r)
  {
    // do nothing
  }

  public void visitTemperature(Temperature t)
  {
    // do nothing
  }

} // end class DefaultVisitor
