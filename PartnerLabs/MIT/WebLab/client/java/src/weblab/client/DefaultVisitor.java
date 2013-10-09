/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

/**
 * A Visitor with empty implementations of all visit methods.
 */
public class DefaultVisitor implements Visitor
{
  public void visitLabConfiguration(LabConfiguration lc)
  {
    // do nothing
  }

  public void visitDevice(Device d)
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

  public void visitSMU(SMU u)
  {
    // do nothing
  }

  public void visitVSU(VSU u)
  {
    // do nothing
  }

  public void visitVMU(VMU u)
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
