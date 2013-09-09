package weblab.client.serverInterface;

// JAVA 1.1 COMPLIANT

/**
 * Used to indicate a problem invoking a Server method.
 */
public class ServerException extends Exception
{
  public ServerException(String mesg)
  {
    super(mesg);
  }
}
