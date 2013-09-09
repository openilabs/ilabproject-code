/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.xml;

// JAVA 1.1 COMPLIANT

/**
 * An XML parser (and related convenient utilities).
 *
 * Note: many comments in this class refer to syntactic constructs
 * defined in the XML 1.0 specification recommended by W3C, which can
 * be found at http://www.w3.org/TR/REC-xml/
 */
public class Parser
{
  // possible parser states
  private static final int BEFORE_ROOT_ELEMENT = 1;
  private static final int INSIDE_OPEN_TAG     = 2;
  private static final int INSIDE_ELEMENT      = 3;
  private static final int AFTER_ROOT_ELEMENT  = 4;

  // current parser state
  private int state;

  // the text being parsed
  private String text;

  // the parser's current position in the text
  private int pointer;

  // the element that the parser is working on right now
  private Element currentElement;

  // the root element of the XML document
  private Element rootElement;



  /**
   * Parses an XML document from a String and returns the root Element.
   *
   * @throws InvalidXMLException if xmlText is not a well-formed XML
   * document
   */
  public static Element parse(String xmlText)
    throws InvalidXMLException
  {
    return (new Parser()).parseText(xmlText);
  }



  /**
   * Replaces xml markup characters &, <, >, ', " in <code>text</code>
   * with corresponding predefined entity references, and returns the
   * result.
   */
  public static String xmlEscape(String text)
  {
    StringBuffer sb = new StringBuffer();

    for (int pointer = 0; pointer < text.length(); pointer++)
    {
      switch(text.charAt(pointer))
      {
      case '&':
	if (text.startsWith("&amp;", pointer) ||
	    text.startsWith("&lt;", pointer) ||
	    text.startsWith("&gt;", pointer) ||
	    text.startsWith("&apos;", pointer) ||
	    text.startsWith("&quot;", pointer))
	{
	  sb.append("&");
	}
	else
	  sb.append("&amp;");
	break;

      case '<':
	sb.append("&lt;");
	break;

      case '>':
	sb.append("&gt;");
	break;

      case '\'':
	sb.append("&apos;");
	break;

      case '"':
	sb.append("&quot;");
	break;

      default:
	sb.append(text.charAt(pointer));
      }
    }

    return sb.toString();
  }



  /**
   * Inserts newlines into xmlText so that it will look nicer (i.e. be
   * more human-readable) when printed to console, and returns the
   * result.
   */
  public static String xmlPrettyPrint(String xmlText)
  {
    // insert a newline inside each occurrence of "><".  Insert them in
    // reverse order of appearance so that each insertion doesn't change
    // the indexes for the rest of the insertions.
    StringBuffer sb = new StringBuffer(xmlText);
    for (int i = xmlText.lastIndexOf("><");
	 i != -1;
	 i = xmlText.lastIndexOf("><", i-1)) {
      sb.insert(i+1, '\n');
    }
    return sb.toString();
  }



  // private constructor prevents object construction outside of this
  // class
  private Parser()
  {
    // do nothing
  }



  // called when the parser encounters an open tag for a new element
  private void openElement(String name)
  {
    Element e = new Element(name, currentElement);
    currentElement = e;
    this.state = INSIDE_OPEN_TAG;
  }



  // called when the parser encounters a regular close tag (not an
  // open-close tag, but the separate kind that specifies the name of
  // the element being closed)
  private void closeElement(String name)
    throws InvalidXMLException
  {
    String expected = currentElement.getName();
    if (! name.equals(expected))
      throw new InvalidXMLException
	("unexpected close tag </" + name +
	 "> at " + pointer + " (expected: </" + expected + ">)");

    closeElement();
  }



  // called when the parser encounters the end of an open-close tag
  // (where the element being closed is not specified explicitly
  // because it's the one that was just opened in this same tag)
  private void closeElement()
  {
    if (currentElement == rootElement)
    {
      // closing the root element
      currentElement = null;
      this.state = AFTER_ROOT_ELEMENT;
    }
    else
    {
      // closing an element other than the root -> we're back inside
      // that element's parent
      currentElement = currentElement.getParent();
      this.state = INSIDE_ELEMENT;
    }
  }



  // the method that does all the work
  private Element parseText(String xmlText)
    throws InvalidXMLException
  {
    // initialize state variables in preparating for parsing
    this.state = BEFORE_ROOT_ELEMENT;
    this.text = xmlText;
    this.pointer = 0;
    this.currentElement = this.rootElement = null;

    // go!
    while(true)
    {
      // behavior of parser is determined by current state
      switch(this.state)
      {
      case BEFORE_ROOT_ELEMENT:
	// We are in the opening section of the XML document, and have
	// not yet begun to parse the root element.
	//
	// expected possibilities: "<?", "<!--", "<!DOCTYPE", or root
	// Element

	parseWhitespace(); // skip over whitespace

	if (pointer >= text.length())
	  throw new InvalidXMLException
	    ("unexpected EOF before root element");

	if (text.startsWith("<?", pointer))
	{
	  // XML declaration or processing instruction -> ignore
	  skipAheadPast("?>");
	}
	else if (text.startsWith("<!--", pointer))
	{
	  // comment -> ignore
	  skipAheadPast("-->");
	}
	else if (text.startsWith("<!DOCTYPE", pointer))
	{
	  // DOCTYPE declaration -> ignore
	  skipAheadPast(">");
	}
	else
	{
	  // must be open tag for the root element
	  parseExactChar('<');
	  openElement(parseName());
	  rootElement = currentElement;
	}
	break;

      case INSIDE_OPEN_TAG:
	// We are currently inside an opening tag, after "<TAGNAME"
	//
	// expected possibilities: ">", "/>", or Attribute

	parseWhitespace(); // skip over whitespace

	if (pointer >= text.length())
	  throw new InvalidXMLException
	    ("unexpected EOF while inside a tag");

	if (text.charAt(pointer) == '>')
	{
	  // end of an open tag -> subsequent text is inside the
	  // element itself

	  pointer++;
	  this.state = INSIDE_ELEMENT;
	}
	else if (text.startsWith("/>", pointer))
	{
	  // end of an open-and-close tag -> close the element

	  pointer += 2;
	  closeElement();
	}
	else
	{
	  // must be an Attribute

	  // parse name of attribute
	  String name = parseName();

	  // parse '=', with optional surrounding whitespace
	  parseWhitespace();
	  parseExactChar('=');
	  parseWhitespace();

	  // parse value for attribute (may be blank)
	  String value = parseAttributeValue();

	  currentElement.addAttribute(name, value);
	}
	break;

      case INSIDE_ELEMENT:
	// We are currently inside an element (after the '>' of the
	// opening tag).
	//
	// expected possibilities: "</", CDATA, "<", Reference, or
	// CharData
	//
	// note: do NOT skip whitespace here; it's valid CharData!

	if (pointer >= text.length())
	  throw new InvalidXMLException
	    ("unexpected EOF while inside an element");

	if (text.startsWith("</", pointer))
	{
	  // a close tag -> close the element

	  pointer += 2;
	  String name = parseName();

	  parseWhitespace();
	  parseExactChar('>');

	  closeElement(name);
	}
	else if (text.startsWith("<![CDATA[", pointer))
	{
	  // a CDSect (section of CDATA)
	  pointer += 9;
	  currentElement.addData(parseCDATA());
	}
	else if (text.charAt(pointer) == '<')
	{
	  // an open tag -> open a new element

	  pointer++;
	  openElement(parseName());
	}
	else if (text.charAt(pointer) == '&')
	{
	  // a Reference
	  currentElement.addData(parseReference());
	}
	else
	{
	  // must be CharData
	  currentElement.addData(parseCharData());
	}
	break;

      case AFTER_ROOT_ELEMENT:
	// We are in the closing section of the XML document, having
	// already parsed the root element in its entirety.
	//
	// expected possibilities: EOF, "<?", "<!--"

	parseWhitespace(); // skip over whitespace

	if (pointer >= text.length())
	{
	  // valid EOF -> stop parsing and return the rootElement
	  return rootElement;
	}

	if (text.startsWith("<?", pointer))
	{
	  // XML declaration or processing instruction -> ignore
	  skipAheadPast("?>");
	}
	else if (text.startsWith("<!--", pointer))
	{
	  // comment -> ignore
	  skipAheadPast("-->");
	}
	else
	  throw new InvalidXMLException
	    ("unexpected character \"" + text.charAt(pointer) +
	     "\" at " + pointer + " (expected: EOF)");
	break;

      default:
	throw new Error("illegal parser state: " + this.state);
      }
    }
  }



  // Consumes and returns a Name (approximately as specified in the
  // W3C spec).
  //
  // For simplicity, totally ignores and throws away namespaces,
  // returning only the part of the name after the last ':'.  Also
  // ignores the fact that some NameChars cannot be the first
  // character of a Name, and ignores CombiningChar and Extender as
  // possible NameChars.
  private String parseName()
    throws InvalidXMLException
  {
    StringBuffer result = new StringBuffer();

    while (pointer < text.length())
    {
      char c = text.charAt(pointer);
      if (Character.isLetterOrDigit(c))
      {
	result.append(c);
      }
      else
	switch(c)
	{
	case ':':
	  // everything up to and including this colon is just a
	  // namespace; throw it away and start over
	  result = new StringBuffer();
	  break;
	case '.':
	case '-':
	case '_':
	  result.append(c);
	  break;
	default:
	  // Name ends here; the next character doesn't belong
	  if (result.length() == 0)
	    throw new InvalidXMLException
	      ("unexpected character \"" + c + "\" at " + pointer +
	       " (expected: Name)");
	  else
	    return result.toString();
	}
      pointer++;
    }

    // no more characters
    if (result.length() == 0)
      throw new InvalidXMLException
	("unexpected EOF (expected: Name)");
    else
      return result.toString();
  }



  // Consumes and returns an AttValue (approximately as specified in
  // the W3C spec), replacing character-escape References with their
  // corresponding real characters.  The return value does NOT include
  // the enclosing apostrophes or quotes.
  //
  // Only handles References that are understood by the parseReference
  // method.
  private String parseAttributeValue()
    throws InvalidXMLException
  {
    StringBuffer result = new StringBuffer();

    if (pointer >= text.length())
      throw new InvalidXMLException
	("unexpected EOF (expected: Attribute Value)");

    // read the quote char: ' or "
    char quote = text.charAt(pointer);
    if (quote != '"' && quote != '\'')
      throw new InvalidXMLException
	("unexpected character \"" + quote + "\" at " + pointer +
	 " (expected: '\"' or \"'\")");
    pointer++;

    while (pointer < text.length())
    {
      char c = text.charAt(pointer);
      if (c == quote)
      {
	pointer++;
	return result.toString();
      }
      else
	switch(c)
	{
	case '&':
	  result.append(parseReference());
	  break;
	case '<':
	  throw new InvalidXMLException
	    ("unexpected \"<\" at " + pointer + " (expected: \""
	     + quote + "\")");
	default:
	  result.append(c);
	  pointer++;
	}
    }
    throw new InvalidXMLException
      ("unexpected EOF while parsing attribute value (expected: \""
       + quote + "\")");
  }



  // Consumes a Reference to one of the predefined entities (amp, lt,
  // gt, apos, quot) and returns a String consisting of the
  // corresponding character (&, <, >, ', ").
  //
  // Does NOT understand the other types of References defined in the
  // W3C spec.
  private String parseReference()
    throws InvalidXMLException
  {
    if (text.startsWith("&amp;", pointer))
    {
      pointer += 5;
      return "&";
    }
    else if (text.startsWith("&lt;", pointer))
    {
      pointer += 4;
      return "<";
    }
    else if (text.startsWith("&gt;", pointer))
    {
      pointer += 4;
      return ">";
    }
    else if (text.startsWith("&apos;", pointer))
    {
      pointer += 6;
      return "'";
    }
    else if (text.startsWith("&quot;", pointer))
    {
      pointer += 6;
      return "\"";
    }
    else
      throw new InvalidXMLException
	("unexpected text after & at " + pointer +
	 " (expected: Reference to a predefined entity)");
  }



  // Consumes and returns CharData (as specified in the W3C spec).
  private String parseCharData()
  {
    StringBuffer result = new StringBuffer();

    while (pointer < text.length())
    {
      char c = text.charAt(pointer);
      switch(c)
      {
      case '&':
      case '<':
	// CharData ends here; the next character doesn't belong
	return result.toString();
      case ']':
	if (text.startsWith("]]>", pointer))
	  // CharData ends here; cannot include CDEnd
	  return result.toString();
	// (else fall through to default case)
      default:
	result.append(c);
	pointer++;
      }
    }

    return result.toString();
  }



  // Consumes and returns CDATA (as specified in the W3C spec).
  private String parseCDATA()
    throws InvalidXMLException
  {
    int endP = text.indexOf("]]>", pointer);
    if (endP == -1)
      throw new InvalidXMLException
	("unexpected EOF while parsing CDATA (expected: \"]]>\")");

    String result = text.substring(pointer, endP);
    pointer = endP + 3;
    return result;
  }



  // Consumes the character c.
  private void parseExactChar(char c)
    throws InvalidXMLException
  {
    if (pointer >= text.length())
      throw new InvalidXMLException
	("unexpected EOF (expected: \"" + c + "\")");

    if (text.charAt(pointer) != c)
      throw new InvalidXMLException
	("unexpected character \"" + text.charAt(pointer) + "\" at "
	 + pointer + " (expected: \"" + c + "\")");
    pointer++;
  }



  // Consumes any amount of whitespace (including none).
  private void parseWhitespace()
  {
    while (pointer < text.length() &&
	   (text.charAt(pointer) == ' ' ||
	    text.charAt(pointer) == '\t' ||
	    text.charAt(pointer) == '\r' ||
	    text.charAt(pointer) == '\n'))
      pointer++;
  }



  // Skips ahead in the text to the next occurrence of goal, consuming
  // everything in between.
  private void skipAheadPast(String goal)
    throws InvalidXMLException
  {
    int newP = text.indexOf(goal, pointer);
    if (newP == -1)
      throw new InvalidXMLException
	("unexpected EOF while looking for next occurrence of \"" + goal
	 + "\" after " + pointer);
    pointer = newP + goal.length();
  }

} // end class Parser
