package weblab.expressionParser;

/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/**
 * <b>ScientificConstantExpression</b> represents a scientific constant in
 * string form, such as <code>"e"</code>.
 */
public class ScientificConstantExpression extends Expression 
{

  /** the scientific constant */
  private String sciconst;

  /**
   * Creates a new ScientificConstantExpression.
   * 
   * @param scientificConstant the scientific constant in string form
   */
  public ScientificConstantExpression(String scientificConstant)
  {
    this.sciconst = scientificConstant;
  }
	
  /**
   * @return the scientific constant in string form
   */
  public String getScientificConstant() {
    return sciconst;
  }
	
  /** 
   * Accepts the visitor <code>visitor</code>, according to the
   * Visitor design pattern, by calling
   * <code>visitor.visitScientificConstantExpression(this)</code>.
   * 
   * @param visitor the visitor visiting this node
   * @throws ExpressionVisitorException if the visitor throws a
   * ExpressionVisitorException while visiting this node
   */
  public void accept(ExpressionVisitor visitor)
    throws ExpressionVisitorException
  {
    visitor.visitScientificConstantExpression(this);
  }
  
  public String toString() {
    return sciconst;
  }
}
