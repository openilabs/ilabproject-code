/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import weblab.toolkit.serverInterface.Server;
import weblab.client.WeblabStubServer;

/**
 * GraphicalAppletDebug is just like GraphicalApplet, except that it uses
 * a StubServer instead of an SBServer.
 */
public class GraphicalAppletDEBUG extends GraphicalApplet
{
  protected Server getServer()
  {
    return new WeblabStubServer();
  }

} // end class GraphicalAppletDEBUG
