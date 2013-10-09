/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

/**
 * Used to indicate that completing the throwing method call might not
 * be desirable for some reason, and that the UI should ask the user
 * for confirmation to make sure.  If the user wants to go through
 * with it anyway, the UI should call the method again with
 * requestConfirmation = false.
 */
public class ConfirmationRequest extends RuntimeException
{
  public ConfirmationRequest()
  {
    super();
  }
}
