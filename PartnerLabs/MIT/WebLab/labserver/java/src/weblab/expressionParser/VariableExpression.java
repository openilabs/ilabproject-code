package weblab.expressionParser;

/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */


/**
 * <b>VariableExpression</b> represents a variable name.
 */
public class VariableExpression extends Expression 
{

  /** the variable name */
  private String varname;

  /**
   * Creates a new VariableExpression.
   * 
   * @param name the variable name
   */
  public VariableExpression(String name) {
    this.varname = name;
  }
	
  /**
   * @return the variable name
   */
  public String getVariableName() {
    return varname;
  }
	
  /*  public Object clone() {
    return new VariableExpression(varname);
    }*/

  /** 
   * Accepts the visitor <code>visitor</code>, according to the
   * Visitor design pattern, by calling
   * <code>visitor.visitVariableExpression(this)</code>.
   * 
   * @param visitor the visitor visiting this node
   * @throws ExpressionVisitorException if the visitor throws a
   * ExpressionVisitorException while visiting this node
   */
  public void accept(ExpressionVisitor visitor)
    throws ExpressionVisitorException
  {
    visitor.visitVariableExpression(this);
  }
  
  public String toString() {
    return varname;
  }
}
