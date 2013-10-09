/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.util;

import java.util.Observable;
import java.util.Observer;

import java.util.Collections;
import java.util.Set;
import java.util.List;
import java.util.WeakHashMap;
import java.util.HashSet;
import java.util.ArrayList;

/**
 * Improves upon java.util.Observable by allowing subclasses to
 * specify the nature of a change in their calls to setChanged.  This
 * information is propagated to the observers when notifyObservers is
 * called.  In addition:
 *
 * Subclasses of ChangeTrackingObservable may safely implement the
 * Cloneable interface (unlike direct subclasses of
 * java.util.Observable).
 *
 * Observers can be stored either by strong reference (if added using
 * the normal addObserver method), or by weak reference (if added
 * using the addWeakReferenceObserver method); the latter behavior
 * should be used wherever possible to avoid creating memory leaks.
 */
public class ChangeTrackingObservable extends Observable
{
  // maps each Observer to either itself (to maintain a strong
  // reference) or null (to maintain only a weak reference).
  private WeakHashMap observers;

  // Two different flags for keeping track of whether the object has
  // changed:
  //
  // StrictChanged is cleared as soon as we BEGIN notifying observers
  // of updates, to avoid unnecessary infinite recursion traps when an
  // Observer's update method triggers another call to notifyObservers
  // without actually changing the object again.
  //
  // LazyChanged is not cleared until we FINISH notifying observers of
  // updates, to obey the stated contract of the public hasChanged
  // method.
  private boolean strictChanged, lazyChanged;

  private HashSet changes; // contains String

  /**
   * Flag value to inform Observers that this has changed in an
   * unspecified way
   */
  public static final String UNSPECIFIED_CHANGE = "UNSPECIFIED";


  /**
   * Constructs a new ChangeTrackingObservable with no Observers.
   */
  public ChangeTrackingObservable()
  {
    observers = new WeakHashMap();
    strictChanged = false;
    lazyChanged = false;
    changes = new HashSet();
  }



  public synchronized void addObserver(Observer o)
  {
    // note that if the Observer is already present but stored only as
    // a weak reference, this will convert it to a strong reference.
    observers.put(o, o);
  }



  /**
   * Adds an observer to the set of observers for this object using a
   * weak reference, provided that it is not the same as some observer
   * already in the set.  The order in which notifications will be
   * delivered to multiple observers is not specified.
   *
   * Because an Observer added using this method is stored only
   * indirectly as the referent of a weak reference, its association
   * with the ChangeTrackingObservable will not prevent such an
   * Observer from being discarded by the garbage collector.
   */
  public synchronized void addWeakReferenceObserver(Observer o)
  {
    if (! observers.containsKey(o))
      observers.put(o, null);
  }



  protected synchronized void clearChanged()
  {
    changes.clear();
    strictChanged = false;
    lazyChanged = false;
  }



  public int countObservers()
  {
    return observers.size();
  }



  public synchronized void deleteObserver(Observer o)
  {
    observers.remove(o);
  }



  public synchronized void deleteObservers()
  {
    observers.clear();
  }



  public boolean hasChanged()
  {
    return lazyChanged;
  }



  /**
   * If this object has changed, as indicated by the hasChanged
   * method, then notify all of its observers and then call the
   * clearChanged method to indicate that this object has no longer
   * changed.
   *
   * Each observer has its update method called with two arguments:
   * this observable object and an Object[] of length 2 containing:
   *
   * 1. null
   *
   * 2. an unmodifiable, immutable Set of String containing any change
   * flags that have been set using setChanged(String) since the last
   * call to clearChanged(), plus the UNSPECIFIED_CHANGE flag if
   * setChanged() has been called with no arguments.
   */
  public void notifyObservers()
  {
    notifyObservers(null);
  }



  /**
   * If this object has changed, as indicated by the hasChanged
   * method, then notify all of its observers and then call the
   * clearChanged method to indicate that this object has no longer
   * changed.
   *
   * Each observer has its update method called with two arguments:
   * this observable object and an Object[] of length 2 containing:
   *
   * 1. the <code>arg</code> argument
   *
   * 2. an unmodifiable, immutable Set of String containing any change
   * flags that have been set using setChanged(String) since the last
   * call to clearChanged(), plus the UNSPECIFIED_CHANGE flag if
   * setChanged() has been called with no arguments.
   */
  public synchronized void notifyObservers(Object arg)
  {
    // don't notify unless this object has changed since the last time
    // we BEGAN doing a notify
    if (! strictChanged)
      return;
    strictChanged = false;

    Object[] updateArg = new Object[2];
    updateArg[0] = arg;
    updateArg[1] = Collections.unmodifiableSet(new HashSet(changes));

    List ol = new ArrayList(observers.keySet());
    for (int i = 0, n = ol.size(); i < n; i++)
      ((Observer) ol.get(i)).update(this, updateArg);

    this.clearChanged();
  }



  /**
   * Indicates that this object has changed, and adds UNSPECIFIED_CHANGE
   * to the set of specific changes made since the last call to
   * clearChanged().
   */
  protected synchronized void setChanged()
  {
    this.setChanged(UNSPECIFIED_CHANGE);
  }



  /**
   * Indicates that this object has changed, and adds newChange to the
   * set of specific changes made since the last call to
   * clearChanged().
   */
  protected synchronized void setChanged(String newChange)
  {
    changes.add(newChange);
    strictChanged = true;
    lazyChanged = true;
  }



  /**
   * Extends Object.clone() to ensure that clones of subclasses of
   * ChangeTrackingObservable will have independently mutable sets of
   * Observers.
   *
   * Note that the cloned ChangeTrackingObservable will start out with
   * all the Observers that this currently has; if it is desired that
   * the clone have no observers, subclass clone methods should call
   * deleteObservers() on the clone before returning it.
   */
  protected synchronized Object clone() throws CloneNotSupportedException
  {
    ChangeTrackingObservable clone =
      (ChangeTrackingObservable) super.clone();
    clone.observers = new WeakHashMap(observers);
    clone.changes = (HashSet) changes.clone();
    return clone;
  }



  /**
   * Extracts the set of specific changes from updateArg.  If a set of
   * changes cannot be found (i.e. if updateArg does not have the
   * expected structure), returns an immutable empty set.
   *
   * @param updateArg an argument provided to an Observable.update
   * method by one of the notifyObservers methods of this
   * @return a Set of String
   */
  public static Set extractChanges(Object updateArg)
  {
    Set result = null;

    try {
      result = (Set) (((Object[]) updateArg)[1]);
    }
    catch (RuntimeException ex)
    {
      // do nothing, the case below will handle it
    }

    if (result == null)
      result = Collections.EMPTY_SET;

    return result;
  }

} // end class ChangeTrackingObservable
