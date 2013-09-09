package weblab.client;

// JAVA 1.1 COMPLIANT

/**
 * Interface for all objects that will visit the Weblab client data
 * hierarchy, according to the Visitor design pattern.
 */
public interface Visitor
{
  public void visitLabConfiguration(LabConfiguration lc);

  public void visitSetup(Setup s);

  public void visitTerminal(Terminal t);

  public void visitExperimentSpecification(ExperimentSpecification e);

  public void visitFGEN(FGEN f);
  
  public void visitSCOPE(SCOPE s);

  public void visitCONSFunction(CONSFunction f);

  public void visitVAR1Function(VAR1Function f);

  public void visitVAR2Function(VAR2Function f);

  public void visitVAR1PFunction(VAR1PFunction f);
  
  public void visitWAVEFORMFunction(WAVEFORMFunction f);
  
  public void visitSAMPLINGFunction(SAMPLINGFunction f);
  
  public void visitUserDefinedFunction(UserDefinedFunction udf);

  public void visitExperimentResult(ExperimentResult r);

  public void visitTemperature(Temperature t);

} // end interface Visitor
