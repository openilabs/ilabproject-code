package weblab.client;

// JAVA 1.1 COMPLIANT

/**
 * Represents a user-defined function on the analyzer.  A
 * UserDefinedFunction has a name, a type of units, and a body (the
 * expression which gives the function its actual meaning).
 *
 * UserDefinedFunction is immutable.
 */
public class UserDefinedFunction
{
  private String name;
  private boolean download;
  private String units;
  private String body;

  /**
   * Creates a new UserDefinedFunction with default (blank) values
   */
  public UserDefinedFunction()
  {
    this("", false, "", "");
  }

  /**
   * Creates a new UserDefinedFunction with the specified values.
   */
  public UserDefinedFunction(String name, boolean download,
			     String units, String body)
  {
    this.name = name;
    this.download = download;
    this.units = units;
    this.body = body;
  }

  /**
   * Returns the name of this user-defined function.
   */
  public final String getName()
  {
    return this.name;
  }

  /**
   * Returns whether the results from this user-defined function will
   * be downloaded.
   */
  public final boolean getDownload()
  {
    return this.download;
  }

  /**
   * Returns the units for this user-defined function.
   */
  public final String getUnits()
  {
    return this.units;
  }

  /**
   * Returns the body of this user-defined function.
   */
  public final String getBody()
  {
    return this.body;
  }

  // overrides Object.equals(o)
  public final boolean equals(Object obj)
  {
    if (obj != null && obj instanceof UserDefinedFunction)
    {
      UserDefinedFunction udf = (UserDefinedFunction) obj;

      if (udf.name.equals(this.name) &&
	  udf.download == this.download &&
	  udf.units.equals(this.units) &&
	  udf.body.equals(this.body))
      {
	return true;
      }
    }

    return false;
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitUserDefinedFunction(this);
  }

} // end class UserDefinedFunction
