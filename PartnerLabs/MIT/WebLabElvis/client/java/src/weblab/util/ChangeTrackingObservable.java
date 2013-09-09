package weblab.util;

// JAVA 1.1 COMPLIANT

import java.util.Vector;

/**
 * Improves upon java.util.Observable by allowing subclasses to
 * specify the nature of a change in their calls to setChanged.  This
 * information is propagated to the observers when notifyObservers is
 * called.
 */
public class ChangeTrackingObservable extends java.util.Observable
{
  private Vector changes; // contains String



  /**
   * Constructs a new ChangeTrackingObservable with no Observers.
   */
  public ChangeTrackingObservable()
  {
    changes = new Vector();
  }



  /**
   * Indicates that this object has changed, and adds newChange to the
   * set of specific changes made since the last call to
   * clearChanged().
   */
  protected synchronized void setChanged(String newChange)
  {
    if (! changes.contains(newChange))
      changes.addElement(newChange);
    this.setChanged();
  }



  // overrides Observable.clearChanged
  protected synchronized void clearChanged()
  {
    changes.removeAllElements();
    super.clearChanged();
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
   * 2. a String[] listing any specific changes that have been
   * identified using setChanged(String) since the last call to
   * clearChanged().
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
   * 2. a String[] listing any specific changes that have been
   * identified using setChanged(String) since the last call to
   * clearChanged().
   */
  public void notifyObservers(Object arg)
  {
    Object[] updateArg = new Object[2];
    updateArg[0] = arg;
    updateArg[1] = listRecentChanges();
    super.notifyObservers(updateArg);
  }



  /**
   * Extracts the set of specific changes from updateArg.  If a set of
   * changes cannot be found (e.g. if updateArg does not have the
   * expected structure), returns an empty array.
   *
   * @param updateArg an argument provided to an Observable.update
   * method by one of the notifyObservers methods of this
   */
  public static String[] extractChanges(Object updateArg)
  {
    String[] result = null;

    try {
      result = (String[]) (((Object[]) updateArg)[1]);
    }
    catch (RuntimeException ex)
    {
      // do nothing, the case below will handle it
    }

    if (result == null)
      result = new String[0];

    return result;
  }



  /**
   * Returns true iff the list of specific changes in updateArg
   * includes the specified change.
   *
   * @param updateArg an argument provided to an Observable.update
   * method by one of the notifyObservers methods of this
   * @param change the change to search for
   */
  public static boolean containsChange(Object updateArg, String change)
  {
    String[] theChanges = extractChanges(updateArg);
    for (int i = 0; i < theChanges.length; i++)
      if (theChanges[i].equals(change))
	return true;

    return false;
  }



  // this is a separate method because it needs to be synchronized
  private synchronized String[] listRecentChanges()
  {
    String[] recentChanges = new String[changes.size()];
    changes.copyInto(recentChanges);
    return recentChanges;
  }

} // end class ChangeTrackingObservable
