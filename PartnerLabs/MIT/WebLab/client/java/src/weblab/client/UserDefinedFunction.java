/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client;

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

  /**
   * Compares the specified object with this UserDefinedFunction for
   * equality.  Returns true iff the specified object is also a
   * UserDefinedFunction and both UserDefinedFunctions have equal
   * values for all fields.
   */
  public final boolean equals(Object obj)
  {
    // check object type
    if (! (obj instanceof UserDefinedFunction))
      return false;
    UserDefinedFunction udf = (UserDefinedFunction) obj;

    // check all fields
    return (udf.name.equals(this.name) &&
	    udf.download == this.download &&
	    udf.units.equals(this.units) &&
	    udf.body.equals(this.body));
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitUserDefinedFunction(this);
  }

} // end class UserDefinedFunction
