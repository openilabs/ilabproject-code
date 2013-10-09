package weblab.expressionParser;

/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/**
 * <b>ExpressionVisitor</b> is the interface for objects that will visit the
 * parse tree, according to the Visitor design pattern.
 */
public interface ExpressionVisitor 
{
	
  /**
   * Process a BinaryOpExpression node.
   * @param expr the expression to process
   * @throws ExpressionVisitorException if an error condition occurs
   */
  public void visitBinaryOpExpression(BinaryOpExpression expr)
    throws ExpressionVisitorException;

  /**
   * Process a BuiltinFunctionExpression node.
   * @param expr the expression to process
   * @throws ExpressionVisitorException if an error condition occurs
   */
  public void visitBuiltinFunctionExpression(BuiltinFunctionExpression expr)
    throws ExpressionVisitorException;
  /**
   * Process a NumericConstantExpression node.
   * @param expr the expression to process
   * @throws ExpressionVisitorException if an error condition occurs
   */
  public void visitNumericConstantExpression(NumericConstantExpression expr)
    throws ExpressionVisitorException;

  /**
   * Process a ScientificConstantExpression node.
   * @param expr the expression to process
   * @throws ExpressionVisitorException if an error condition occurs
   */
  public void visitScientificConstantExpression(ScientificConstantExpression expr)
    throws ExpressionVisitorException;

  /**
   * Process a UnaryOpExpression node.
   * @param expr the expression to process
   * @throws ExpressionVisitorException if an error condition occurs
   */
  public void visitUnaryOpExpression(UnaryOpExpression expr)
    throws ExpressionVisitorException;

  /**
   * Process a VariableExpression node.
   * @param expr the expression to process
   * @throws ExpressionVisitorException if an error condition occurs
   */
  public void visitVariableExpression(VariableExpression expr)
    throws ExpressionVisitorException;
}
