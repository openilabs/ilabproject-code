package weblab.expressionParser;

/**
 * <b>NumericConstantExpression</b> represents a numeric constant in
 * string form, such as <code>"1.02e-5"</code>.
 */
public class NumericConstantExpression extends Expression {

  /** the numeric constant */
  private String numconst;

  /**
   * Creates a new NumericConstantExpression.
   * 
   * @param numericConstant the numeric constant in string form
   */
  public NumericConstantExpression(String numericConstant) {
    this.numconst = numericConstant;
  }
	
  /**
   * @return the numeric constant in string form
   */
  public String getNumericConstant() {
    return numconst;
  }
	
  /** 
   * Accepts the visitor <code>visitor</code>, according to the
   * Visitor design pattern, by calling
   * <code>visitor.visitNumericConstantExpression(this)</code>.
   * 
   * @param visitor the visitor visiting this node
   * @throws ExpressionVisitorException if the visitor throws a
   * ExpressionVisitorException while visiting this node
   */
  public void accept(ExpressionVisitor visitor)
    throws ExpressionVisitorException
  {
    visitor.visitNumericConstantExpression(this);
  }
  
  public String toString() {
    return numconst;
  }
}
