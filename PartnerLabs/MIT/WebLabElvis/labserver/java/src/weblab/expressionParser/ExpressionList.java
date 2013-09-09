package weblab.expressionParser;

import java.util.ArrayList;
import java.util.Iterator;

/**
 * <b>ExpressionList</b> represents a list of Expressions.
 */
public class ExpressionList implements Cloneable {

  private ArrayList list;
	
  /**
   * Create a new empty ExpressionList.
   */
  public ExpressionList() {
    list = new ArrayList();
  }

  /**
   * Returns an iterator over the elements in this list in proper sequence.
   * 
   * @return an iterator over the elements in this list in proper sequence.
   */
  public Iterator iterator() {
    return list.iterator();
  }
	
  /**
   * Inserts the specified element at the specified position in this list.
   * 
   * @param index the index to insert at
   * @param element the expression to add
   * @throws IndexOutOfBoundsException if index is out of range 
   * <code>(index < 0 || index > size())</code>.
   */
  public void add(int index, Expression element) {
    list.add(index, element);
  }

  /**
   * Appends the specified element to the end of this list.
   * 
   * @param o element to be appended to this list
   * @return true (as per the general contract of Collection.add).
   */
  public boolean add(Expression o) {
    return list.add(o);
  }

  /**
   * Removes all of the elements from this list.
   */
  public void clear() {
    list.clear();
  }

  /**
   * Returns true if this list contains the specified element.
   *
   * @param elem element whose presence in this ExpressionList is to be 
   * tested.
   * @return true if the specified element is present; false otherwise.
   */
  public boolean contains(Expression elem) {
    return list.contains(elem);
  }

  /**
   * Searches for the first occurence of the given argument, testing for 
   * equality using the equals method.
   * 
   * @param elem an expression
   * @return the index of the first occurrence of the argument in this list; 
   * returns -1 if the object is not found.
   */
  public int indexOf(Expression elem) {
    return list.indexOf(elem);
  }

  /**
   * Returns the index of the last occurrence of the specified object in this 
   * list.
   * 
   * @param elem - the desired element
   * @return the index of the last occurrence of the specified object in 
   * this list; returns -1 if the object is not found.
   */
  public int lastIndexOf(Expression elem) {
    return list.lastIndexOf(elem);
  }

  /**
   * Returns the element at the specified position in this list.
   * 
   * @param index index of element to return
   * @return the element at the specified position in this list
   * @throws IndexOutOfBoundsException if index is out of range 
   * <code>(index < 0 || index >= size())</code>.
   */
  public Expression get(int index) {
    return (Expression)list.get(index);
  }

  /**
   * Replaces the element at the specified position in this list with the 
   * specified element.
   * 
   * @param index index of element to replace
   * @param element element to be stored at the specified position
   * @return the element previously at the specified position
   * @throws IndexOutOfBoundsException if index out of range 
   * <code>(index < 0 || index >= size())</code>
   */
  public Expression set(int index, Expression element) {
    return (Expression)list.set(index, element);
  }

  /**
   * Tests if this list has no elements.	
   * 
   * @return true if this list has no elements; false otherwise.
   */
  public boolean isEmpty() {
    return list.isEmpty();
  }

  /**
   * Returns the number of elements in this list.
   * 
   * @return the number of elements in this list
   */
  public int size() {
    return list.size();
  }
	
  /**
   * Returns a shallow copy of this ExpressionList instance. (The elements 
   * themselves are not copied.)
   * 
   * @return a clone of this ExpressionList instance.
   */
  public Object clone() {
    try {
      ExpressionList newList = (ExpressionList)super.clone();
      newList.list = (ArrayList)this.list.clone();
      return newList;
    } catch (CloneNotSupportedException ex) {
      return null;
    }
  }
	
  public String toString() {
    StringBuffer buffer = new StringBuffer();
    for(Iterator i = iterator(); i.hasNext(); ) {
      Expression exp = (Expression)i.next();
      buffer.append(exp.toString());
      buffer.append(", ");
    }
    buffer.delete(buffer.length()-2,buffer.length());
    return buffer.toString();
  }
	
}
