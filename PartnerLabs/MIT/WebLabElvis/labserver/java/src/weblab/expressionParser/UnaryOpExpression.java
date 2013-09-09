package weblab.expressionParser;

/**
 * <b>UnaryOpExpression</b> represents an operation on one
 * subexpression, such as <code>-a</code>.  A UnaryOpExpression
 * consists of an operand and an operator (represented by its opcode,
 * which should be either PLUS or MINUS).
 */
public class UnaryOpExpression extends Expression {

  public static final int PLUS = 1;
  public static final int MINUS = 2;

  /** the opcode */
  private int op;
  /** the operand */
  private Expression operand;

  /**
   * Creates a new UnaryOpExpression.
   * 
   * @param opcode the opcode
   * @param operand the operand
   */
  public UnaryOpExpression(int opcode, Expression operand) {
    this.op = opcode;
    this.operand = operand;
  }
	
  /**
   * @return the opcode of this
   */
  public int getOperator() {
    return op;
  }
	
  /**
   * @return the operand of this
   */
  public Expression getOperand() {
    return operand;
  }
	
  /** 
   * Accepts the visitor <code>visitor</code>, according to the
   * Visitor design pattern, by calling
   * <code>visitor.visitUnaryOpExpression(this)</code>.
   * 
   * @param visitor the visitor visiting this node
   * @throws ExpressionVisitorException if the visitor throws a
   * ExpressionVisitorException while visiting this node
   */
  public void accept(ExpressionVisitor visitor)
    throws ExpressionVisitorException
  {
    visitor.visitUnaryOpExpression(this);
  }
  
  public String toString() {
    StringBuffer buffer = new StringBuffer();
    buffer.append("(");
    buffer.append(opToString(op));
    buffer.append(", ");
    buffer.append(operand.toString());
    buffer.append(")");
    return buffer.toString();
  }
	
  // UTILITY FUNCTIONS
	
  /**
   * Return a String representation of the opcode.
   * 
   * @param op the opcode to convert
   * @return the String representation for the operation
   */
  protected String opToString(int op) {
    switch(op) {
    case PLUS:
      return "+";
    case MINUS:
      return "-";
    default:
      return "UNKNOWN";
    }
  }
}
