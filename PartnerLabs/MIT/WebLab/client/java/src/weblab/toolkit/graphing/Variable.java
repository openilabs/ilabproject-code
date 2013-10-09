/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.graphing;

import java.util.List;

/**
 * Variable is the basic building block of graphable data.  It has a
 * name, units, and a list of numerical (BigDecimal) data values.
 * Each Variable also belongs to a "dataset", which is simply a group
 * of Variables that are semantically related to each other
 * (e.g. because they came from the same experiment).  Datasets are
 * represented as Lists rather than Sets because the ordering may be
 * important for some applications.
 *
 * Variables are considered immutable; implementing classes should
 * make sure that the values returned by its methods will not change
 * over time.
 */
public interface Variable
{
  /**
   * Returns the name of this variable.  May not be blank or null.
   */
  public String getName();

  /**
   * Returns the unit symbol for this variable.  May be null to
   * represent a dimensionless quantity.
   */
  public String getUnits();

  /**
   * Returns the numerical data for this variable.
   *
   * @return an immutable List of BigDecimal
   */
  public List getData();

  /**
   * Returns the dataset to which this Variable belongs.
   *
   * @return an immutable List of Variable
   */
  public List getDataSet();
}
