/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

import java.util.Iterator;

/**
 * Traverses an ExperimentSpecification looking for VAR1, VAR2, and
 * VAR1P.  Records information about them in public member fields
 * which can be subsequently read back.
 */
public class ExperimentSpecificationVisitor extends DefaultVisitor
{
  // traversal state
  private Port currentPort;
  private String currentSourceName;

  // RESULTS FIELDS

  /**
   * A Port whose source function is a VAR1Function, or null if none
   * has been found.
   */
  public Port var1Port;

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
   * A Port whose source function is a VAR2Function, or null if none
   * has been found.
   */
  public Port var2Port;

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
   * A Port whose source function is a VAR1PFunction, or null if none
   * has been found.
   */
  public Port var1pPort;

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
    for (Iterator i = e.getPorts(); i.hasNext(); )
    {
      currentPort = (Port) i.next();
      currentPort.accept(this);
    }
  }

  public final void visitSMU(SMU u)
  {
    switch (u.getMode())
    {
    case SMU.V_MODE:
      currentSourceName = u.getVName();
      u.getFunction().accept(this);
      break;
    case SMU.I_MODE:
      currentSourceName = u.getIName();
      u.getFunction().accept(this);
      break;
    case SMU.COMM_MODE:
    default:
    }
  }

  public final void visitVSU(VSU u)
  {
    currentSourceName = u.getVName();
    u.getFunction().accept(this);
  }

  public final void visitVAR1Function(VAR1Function f)
  {
    var1Port = currentPort;
    var1Name = currentSourceName;
    var1Function = f;
  }

  public final void visitVAR2Function(VAR2Function f)
  {
    var2Port = currentPort;
    var2Name = currentSourceName;
    var2Function = f;
  }

  public final void visitVAR1PFunction(VAR1PFunction f)
  {
    var1pPort = currentPort;
    var1pName = currentSourceName;
    var1pFunction = f;
  }
} // end class ExperimentSpecificationVisitor
