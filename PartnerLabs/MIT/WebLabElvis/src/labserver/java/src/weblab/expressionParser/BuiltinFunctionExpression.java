package weblab.expressionParser;

/**
 * <b>BuiltinFunctionExpression</b> represents an application of a
 * built-in function of the HP4155.  A BuiltinFunctionExpression
 * consists of a function name and a list of argument expressions.
 */
public class BuiltinFunctionExpression extends Expression {

  /** the function name */
  private String function;
  /** the operand */
  private ExpressionList arglist;

  /**
   * Creates a new BuiltinFunctionExpression.
   * 
   * @param function the function name
   * @param arglist the list of argument expressions
   */
  public BuiltinFunctionExpression(String function, ExpressionList arglist) {
    this.function = function;
    this.arglist = arglist;
  }
	
  /**
   * @return the function name
   */
  public String getFunction() {
    return function;
  }
	
  /**
   * @return the list of argument expressions
   */
  public ExpressionList getArgumentList() {
    return arglist;
  }
  	
  /** 
   * Accepts the visitor <code>visitor</code>, according to the
   * Visitor design pattern, by calling
   * <code>visitor.visitBuiltinFunctionExpression(this)</code>.
   * 
   * @param visitor the visitor visiting this node
   * @throws ExpressionVisitorException if the visitor throws a
   * ExpressionVisitorException while visiting this node
   */
  public void accept(ExpressionVisitor visitor)
    throws ExpressionVisitorException
  {
    visitor.visitBuiltinFunctionExpression(this);
  }
  
  public String toString() {
    StringBuffer buffer = new StringBuffer();
    buffer.append(function);
    buffer.append("(");
    buffer.append(arglist.toString());
    buffer.append(")");
    return buffer.toString();
  }
}
