package weblab.expressionParser;

import java.util.HashMap;

/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/**
 * <b>Expression</b> is the base class representing any expression in
 * the HP4155 system.  All expressions can be annotated with key/value
 * pairs.
 */
public abstract class Expression 
{

  /** a mapping from keys to attributes */
  private HashMap map = new HashMap();
	

  /** 
   * Accepts the visitor <code>visitor</code>, according to the Visitor
   * design pattern.
   * 
   * @param visitor the visitor visiting this node
   * @throws ExpressionVisitorException if the visitor throws a
   * ExpressionVisitorException while visiting this node
   */
  public abstract void accept(ExpressionVisitor visitor)
    throws ExpressionVisitorException;
  
  /**
   * Sets the value of the given key. <code>null</code> removes the
   * mapping.
   * 
   * @param key the key to set
   * @param value the value of the key
   */
  public void annotate(Object key, Object value) {
    map.put(key, value);
  }
	
  /**
   * Gets the value the given key maps to, or <code>null</code> if no
   * such mapping exists.
   * 
   * @param key the key to retrieve
   * @return the value the key maps to, or <code>null<code> if the key does
   * not map to anything
   */
  public Object getAnnotation(Object key) {
    return map.get(key);
  }
}
