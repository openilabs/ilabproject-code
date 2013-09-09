package weblab.expressionParser;

public class InvalidExpressionSyntaxException extends Exception {
  public InvalidExpressionSyntaxException(String mesg) {
    super(mesg);
  }
}
