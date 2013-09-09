package weblab.client;

// JAVA 1.1 COMPLIANT

/**
 * Abstract superclass for any of the instrument connected to an experiment setup.
 */
public abstract class Instrument extends java.util.Observable
{
  public static final int FGEN_TYPE = 1;
  public static final int SCOPE_TYPE = 2;

  // the Terminal connected to this Setup.  Can be null!  (since an
  // ExperimentSpecification need not always be connected to a Setup)
  private Terminal myTerminal;

  /**
   * Factory method that creates an instrument with the given type and
   * number (and a null Terminal).
   */
  public static Instrument createInstrument(int type, int number)
  {
    switch(type)
    {
    	case FGEN_TYPE:
    		return new FGEN(number);
    	case SCOPE_TYPE:
    		return new SCOPE(number);
    	default:
    		throw new Error("illegal instrument type in Instrument.createInstrument");
    }
  }

  /**
   * Returns the instrument number of this.
   */
  public abstract int getNumber();

  /**
   * Returns the instrument type of this (FGEN_TYPE or SCOPE_TYPE).
   */
  public abstract int getType();

  /**
   * Returns the name of this instrument.
   */
  public String getName()
  {
    switch(this.getType()) {
    case FGEN_TYPE:
      return "Function Generator" + this.getNumber();
    case SCOPE_TYPE:
      return "Oscilloscope" + this.getNumber();
    default:
      throw new Error("illegal instrument type: " + this.getType());
    }
  }

  /**
   * Returns the Terminal connected to this (or null if none).
   */
  public Terminal getTerminal()
  {
    return myTerminal;
  }

  /**
   * Sets the Terminal connected to this.
   *
   * @param newTerminal the new Terminal connected to this, or null
   * for none.
   */
  public void setTerminal(Terminal newTerminal)
  {
    if (newTerminal == null)
    {
      if (myTerminal != null)
      {
    	  myTerminal = null;
    	  this.setChanged();
      }
    }
    else if (! newTerminal.equals(myTerminal))
    {
      myTerminal = newTerminal;
      this.setChanged();
    }
  }

  /**
   * Returns true iff this instrument has been configured (i.e. changed from
   * its default values).
   *
   * Note: this is meant to detect changes made by the user, so
   * setting a Terminal doesn't count.
   */
  public abstract boolean isConfigured();

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public abstract void accept(Visitor v);

} // end class Instrument
