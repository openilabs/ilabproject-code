package weblab.expressionParser;

/**
 * <b>BinaryOpExpression</b> represents an operation on two
 * subexpressions, such as <code>a+b</code>.  A BinaryOpExpression
 * consists of a left operand, a right operand, and an operator
 * (represented by its opcode, which should be one of PLUS, MINUS,
 * TIMES, DIVIDE, and POW).
 */
public class BinaryOpExpression extends Expression {

  public static final int PLUS = 1;
  public static final int MINUS = 2;
  public static final int TIMES = 3;
  public static final int DIVIDE = 4;
  public static final int POW = 5;

  /** the opcode */
  private int op;
  /** the left operand */
  private Expression left;
  /** the right operand */
  private Expression right;

  /**
   * Creates a new BinaryOpExpression.
   * 
   * @param opcode the opcode
   * @param left the left operand
   * @param right the right operand
   */
  public BinaryOpExpression(int opcode, Expression left, Expression right) {
    this.op = opcode;
    this.left = left;
    this.right = right;
  }
	
  /**
   * @return the opcode of this
   */
  public int getOperator() {
    return op;
  }
	
  /**
   * @return the left operand of this
   */
  public Expression getLeft() {
    return left;
  }
	
  /**
   * @return the right operand of this
   */
  public Expression getRight() {
    return right;
  }

  /** 
   * Accepts the visitor <code>visitor</code>, according to the
   * Visitor design pattern, by calling
   * <code>visitor.visitBinaryOpExpression(this)</code>.
   * 
   * @param visitor the visitor visiting this node
   * @throws ExpressionVisitorException if the visitor throws a
   * ExpressionVisitorException while visiting this node
   */
  public void accept(ExpressionVisitor visitor)
    throws ExpressionVisitorException
  {
    visitor.visitBinaryOpExpression(this);
  }
  
  public String toString() {
    StringBuffer buffer = new StringBuffer();
    buffer.append("(");
    buffer.append(opToString(op));
    buffer.append(", ");
    buffer.append(left.toString());
    buffer.append(", ");
    buffer.append(right.toString());
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
    case TIMES:
      return "*";
    case DIVIDE:
      return "/";
    case POW:
      return "^";
    default:
      return "UNKNOWN";
    }
  }
}
