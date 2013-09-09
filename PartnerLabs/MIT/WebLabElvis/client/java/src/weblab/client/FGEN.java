package weblab.client;

// JAVA 1.1 COMPLIANT

/**
 * Represents a Function generator  (FGEN).
 * An FGEN has a VName, an instrument number and a source function. 
 */
public class FGEN extends Instrument
{
  private int number;
  private String vName;
  private boolean vDownload;
  private SourceFunction function;

  /**
   * Creates a new FGEN with default values.
   */
  public FGEN(int number)
  {
    this.number = number;
    // pre-initialize things that might be null
    this.vName = "";
    this.function = new WAVEFORMFunction();
    // initialize everything here
    this.reset();
  }

  /**
   * Resets this FGEN to default values.
   */
  public final void reset()
  {
    this.setVName("");
    this.setVDownload(false);
    this.setFunction(new WAVEFORMFunction());
  }
	
  public final int getNumber()
  {
    return this.number;
  }

  public final int getType()
  {
    return FGEN_TYPE;
  }

  public final boolean isConfigured()
  {
    return (! this.matches(new FGEN(this.number)));
  }

  /**
   * Returns the name of the voltage variable for this unit.
   */
  public final String getVName()
  {
    return this.vName;
  }

  /**
   * Sets the name of the voltage variable for this unit.
   *
   * @param name the new VName for this
   */
  public final void setVName(String name)
  {
    if (! this.vName.equals(name))
    {
      this.vName = name;
      this.setChanged();
    }
  }

  /**
   * Returns whether the voltage variable for this unit will be
   * downloaded.
   */
  public final boolean getVDownload()
  {
    return this.vDownload;
  }

  /**
   * Sets whether the voltage variable for this unit will be
   * downloaded.
   *
   * @param download true to download the voltage variable
   */
  public final void setVDownload(boolean download)
  {
    if (this.vDownload != download)
    {
      this.vDownload = download;
      this.setChanged();
    }
  }

  /**
   * Returns the source function of this.
   */
  public final SourceFunction getFunction()
  {
    return this.function;
  }

  /**
   * Sets the source function of this.
   *
   * @param function the new source function for this.
   */
  public final void setFunction(SourceFunction function)
  {
    if (! this.function.equals(function))
    {
      this.function = function;
      this.setChanged();
    }
  }

  /**
   * Returns true iff the settings of this match the settings of
   * i. This is like an equals() method, but not quite because two
   * FGENs should not really be considered equal unless actually
   * identical.
   */
  public final boolean matches(FGEN i)
  {
    return (i.number == number &&
	    i.vName.equals(vName) &&
	    i.vDownload == vDownload &&
	    i.function.equals(function));
  }

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public final void accept(Visitor v)
  {
    v.visitFGEN(this);
  }

} // end class VSU
