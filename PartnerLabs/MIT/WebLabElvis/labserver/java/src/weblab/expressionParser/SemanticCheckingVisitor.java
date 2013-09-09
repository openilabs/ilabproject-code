package weblab.expressionParser;

import java.util.Iterator;
import java.util.Vector;

/**
 * <b>SemanticCheckingVisitor</b> visits an Expression, according to
 * the Visitor design pattern, and performs semantic checks.  At
 * present, the following error conditions can be detected:
 * <ul>
 * <li> wrong number of arguments to a builtin function
 * <li> (optional) reference to a variable not in a [provided] list of
 *      defined variables
 * </ul>
 */
public class SemanticCheckingVisitor implements ExpressionVisitor {

  // a vector of strings listing all valid variable names
  private Vector validVariableNames;
  private boolean checkVariables;

  /**
   * Create a SemanticCheckingVisitor without variable checking.
   */
  public SemanticCheckingVisitor() {
    validVariableNames = new Vector();
    checkVariables = false;
  }

  /**
   * Create a SemanticCheckingVisitor with variable checking
   *
   * @param v a vector of strings representing all valid variable names
   */
  public SemanticCheckingVisitor(Vector v) {
    validVariableNames = v;
    checkVariables = true;
  }
  
  /**
   * Process a BinaryOpExpression node: visit both subexpressions.
   * @param expr the expression to process
   * @throws ExpressionVisitorException if a
   * ExpressionVisitorException is thrown while visiting a
   * subexpression
   */
  public void visitBinaryOpExpression(BinaryOpExpression expr)
    throws ExpressionVisitorException
  {
    // visit subexpressions
    expr.getLeft().accept(this);
    expr.getRight().accept(this);
  }

  /**
   * Process a BuiltinFunctionExpression node: check for the error
   * conditions described below, and visit all argument
   * subexpressions.
   *
   * @param expr the expression to process
   * @throws ExpressionVisitorException if a
   * ExpressionVisitorException is thrown while visiting a
   * subexpression, or if the function is not one of the builtin
   * functions defined in the HP4155 user manual, or if it is invoked
   * with a different number of arguments than its definition
   * requires.
   */
  public void visitBuiltinFunctionExpression(BuiltinFunctionExpression expr)
    throws ExpressionVisitorException
  {
    // extract function name and argument list
    String fxn = expr.getFunction();
    ExpressionList arglist = expr.getArgumentList();

    // set expectedArgCount to the number of arguments this function
    // should have (according to the HP4155 user manual)
    int expectedArgCount;
    if (fxn.equals("ABS")) {
      expectedArgCount = 1;
    }
    else if (fxn.equals("AT")) {
      expectedArgCount = 2;
    }
    else if (fxn.equals("AVG")) {
      expectedArgCount = 1;
    }
    else if (fxn.equals("COND")) {
      expectedArgCount = 4;
    }
    else if (fxn.equals("DELTA")) {
      expectedArgCount = 1;
    }
    else if (fxn.equals("DIFF")) {
      expectedArgCount = 2;
    }
    else if (fxn.equals("EXP")) {
      expectedArgCount = 1;
    }
    else if (fxn.equals("INTEG")) {
      expectedArgCount = 2;
    }
    else if (fxn.equals("LGT")) {
      expectedArgCount = 1;
    }
    else if (fxn.equals("LOG")) {
      expectedArgCount = 1;
    }
    else if (fxn.equals("MAVG")) {
      expectedArgCount = 2;
    }
    else if (fxn.equals("MAX")) {
      expectedArgCount = 1;
    }
    else if (fxn.equals("MIN")) {
      expectedArgCount = 1;
    }
    else if (fxn.equals("SQRT")) {
      expectedArgCount = 1;
    }
    else {
      // not a recognized function
      throw new ExpressionVisitorException("unrecognized function " + fxn);
    }

    // check actual number of arguments against expected number
    if (arglist.size() != expectedArgCount) {
      throw new ExpressionVisitorException("cannot invoke function " + fxn +
				 " with " + arglist.size() +
				 " argument(s); it requires " +
				 expectedArgCount);
    }

    // visit all argument subexpressions.  
    Iterator i = arglist.iterator();
    while (i.hasNext()) {
      Expression e = (Expression)i.next();
      e.accept(this);
    }
  }

  /**
   * Process a NumericConstantExpression node: do nothing.
   * @param expr the expression to process
   */
  public void visitNumericConstantExpression(NumericConstantExpression expr) {
    // do nothing
  }

  /**
   * Process a ScientificConstantExpression node: do nothing.
   * @param expr the expression to process
   */
  public void visitScientificConstantExpression(ScientificConstantExpression expr) {
    // do nothing
  }

  /**
   * Process a UnaryOpExpression node: visit the subexpression.
   * @param expr the expression to process
   * @throws ExpressionVisitorException if a
   * ExpressionVisitorException is thrown while visiting a
   * subexpression
   */
  public void visitUnaryOpExpression(UnaryOpExpression expr)
    throws ExpressionVisitorException
  {
    // visit subexpression
    expr.getOperand().accept(this);
  }

  /**
   * Process a VariableExpression node: check for the error conditions
   * described below.
   *
   * @param expr the expression to process
   * @throws ExpressionVisitorException if variable checking is active
   * and the name of the variable in expr is not a valid variable
   * name.
   */
  public void visitVariableExpression(VariableExpression expr)
    throws ExpressionVisitorException
  {
    if (checkVariables) {
      String varname = expr.getVariableName();
      if (! validVariableNames.contains(varname)) {
	throw new ExpressionVisitorException("illegal reference to [non-existent] variable " + varname);
      }
    }
    else {
    // do nothing
    }
  }
}
