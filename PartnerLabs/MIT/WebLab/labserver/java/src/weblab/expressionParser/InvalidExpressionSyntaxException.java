package weblab.expressionParser;

/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

public class InvalidExpressionSyntaxException extends Exception 
{
  public InvalidExpressionSyntaxException(String mesg) {
    super(mesg);
  }
}
