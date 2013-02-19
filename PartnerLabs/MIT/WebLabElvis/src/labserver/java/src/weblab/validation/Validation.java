package weblab.validation;

import weblab.expressionParser.*;
import java.util.StringTokenizer;
import java.util.Vector;

public class Validation {

  /**
   * Command-line executable to validate the body of a user-defined
   * function.  It can be used in two different ways:
   *
   * java Validation -v "VX,IX,VY,IY" "LOG(VX)+VY"
   *
   * reports an error if the body refers to any variable not listed in
   * the -v parameter
   *
   * java Validation "LOG(VX)+VY"
   *
   * assumes that any syntactically correct variable name is valid
   *
   *
   * Prints "OK" to stdout if the function body is okay, otherwise
   * prints an error message to stdout.
   */
  public static void main(String [] args) {

    String fxn; // the function body supplied
    Vector vars = null; // the set of valid variable names
    boolean varcheck; // whether to do variable checking

    // parse command-line arguments
    switch(args.length) {
    case 1:
      // usage without -v
      fxn = args[0];
      varcheck = false;
      break;
    case 3:
      // usage with -v
      if (! args[0].equals("-v")) {
	// first arg must be -v
	usage();
	return;
      }
      fxn = args[2];
      varcheck = true;
      vars = new Vector();
      StringTokenizer vstr = new StringTokenizer(args[1], ",");
      while(vstr.hasMoreTokens())
	vars.add(vstr.nextToken());
      break;
    default:
      // erroneous usage
      usage();
      return;
    }

    Expression e; // the expression object parsed from the function body
    SemanticCheckingVisitor v; // does semantic checking on the expression

    // parse body into an expression object
    try {
      e = Parser.parseExpression(fxn);
    }
    catch (InvalidExpressionSyntaxException ex) {
      System.out.println("Syntax error: " + ex.getMessage());
      return;
    }

    // do semantic checking on it
    try {
      if (varcheck)
	v = new SemanticCheckingVisitor(vars);
      else
	v = new SemanticCheckingVisitor();
      e.accept(v);
    }
    catch (ExpressionVisitorException ex) {
      System.out.println("Semantic error: " + ex.getMessage());
      return;
    }

    // if we get here, the function is okay
    System.out.println("OK");
    return;
  }


  /**
   * Prints the usage message (command-line help)
   */
  private static void usage() {
    System.out.println("Usage: java -classpath <bin> weblab.validation.Validation [-v <vars>] <exp>");
    System.out.println();
    System.out.println("<bin>\t path to the bin directory which contains class files");
    System.out.println("<vars>\t (optional) comma-separated list of valid variables");
    System.out.println("<exp>\t the function body to check");
    System.out.println();
    System.out.println("You should probably enclose <exp> in quotes to protect it from the shell.");
    return;
  }

}// end class Validation
