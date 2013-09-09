package weblab.client;

// JAVA 1.1 COMPLIANT

/**
 * Used to indicate an invalid XML Experiment Specification.
 */
public class InvalidExperimentSpecificationException extends Exception
{
  public InvalidExperimentSpecificationException(String mesg)
  {
    super(mesg);
  }
}
