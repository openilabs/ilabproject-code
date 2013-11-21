/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.xml;

// JAVA 1.1 COMPLIANT

import java.util.Hashtable;
import java.util.Vector;
import java.util.Enumeration;

/**
 * Represents a single XML element.  Elements are generated by the
 * Parser.parse method.
 */
public class Element
{
  private String name;
  private Hashtable attributes = new Hashtable(); // maps String to String
  private StringBuffer data = new StringBuffer();
  private Element parent;
  private Vector children = new Vector(); // contains Element



  /**
   * Returns the name of this element.
   */
  public String getName()
  {
    return name;
  }



  /**
   * Returns the value for the named attribute in this element, or
   * null if no such attribute exists.
   */
  public String getAttributeValue(String name)
  {
    return (String) attributes.get(name);
  }



  /**
   * Returns the simple string data in this element, or "" if it
   * contains no string data.
   */
  public String getData()
  {
    return data.toString();
  }



  /**
   * Returns the parent element of this element. If no parent then
   * null is returned.
   */
  public Element getParent()
  {
    return parent;
  }



  /**
   * Returns an enumeration (of Element) over the subelements of this
   * element that have the given name.
   */
  public Enumeration getChildren(String name)
  {
    Vector r = new Vector();
    Enumeration enumChildren = children.elements();
    while(enumChildren.hasMoreElements())
    {
      Element e = (Element) enumChildren.nextElement();
      if (e.getName().equals(name))
      {
	r.addElement(e);
      }
    }
    return r.elements();
  }



  /**
   * Returns a SINGLE subelement of this with the given name (null if
   * none exists).
   */
  public Element getChild(String name)
  {
    Enumeration enumChildren = this.getChildren(name);
    if (! enumChildren.hasMoreElements())
      return null;
    return (Element) enumChildren.nextElement();
  }



  ////////////////////////////////////////////
  // Package-visible methods used by Parser //
  ////////////////////////////////////////////

  // parent may be null
  Element(String name, Element parent)
  {
    this.name = name;
    if (parent != null)
    {
      this.parent = parent;
      this.parent.children.addElement(this);
    }
  }

  void addAttribute(String name, String value)
  {
    attributes.put(name, value);
  }

  void addData(String s)
  {
    data.append(s);
  }

} // end class Element
