/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

/**
 * Interface for all objects that will visit the Weblab client data
 * hierarchy, according to the Visitor design pattern.
 */
public interface Visitor
{
  public void visitLabConfiguration(LabConfiguration lc);

  public void visitDevice(Device d);

  public void visitTerminal(Terminal t);

  public void visitExperimentSpecification(ExperimentSpecification e);

  public void visitSMU(SMU u);

  public void visitVSU(VSU u);

  public void visitVMU(VMU u);

  public void visitCONSFunction(CONSFunction f);

  public void visitVAR1Function(VAR1Function f);

  public void visitVAR2Function(VAR2Function f);

  public void visitVAR1PFunction(VAR1PFunction f);

  public void visitUserDefinedFunction(UserDefinedFunction udf);

  public void visitExperimentResult(ExperimentResult r);

  public void visitTemperature(Temperature t);

} // end interface Visitor
