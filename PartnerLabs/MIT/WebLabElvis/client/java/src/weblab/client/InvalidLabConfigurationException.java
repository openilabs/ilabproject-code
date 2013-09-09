package weblab.client;

// JAVA 1.1 COMPLIANT

/**
 * Used to indicate an invalid XML Lab Configuration.
 */
public class InvalidLabConfigurationException extends Exception
{
  public InvalidLabConfigurationException(String mesg)
  {
    super(mesg);
  }
}
