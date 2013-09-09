package weblab.client.graphicalUI;

import weblab.client.serverInterface.Server;
import weblab.client.serverInterface.StubServer;

/**
 * GraphicalAppletDebug is just like GraphicalApplet, except that it uses
 * a StubServer instead of an SBServer.
 */
public class GraphicalAppletDEBUG extends GraphicalApplet
{
  protected Server getServer()
  {
    return new StubServer();
  }

} // end class GraphicalAppletDEBUG
