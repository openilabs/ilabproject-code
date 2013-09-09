package weblab.client;

// JAVA 1.1 COMPLIANT

/**
 * Abstract superclass for any source function that can be assigned to
 * an SMU or VSU.
 */
public abstract class SourceFunction
{
  public static final int CONS_TYPE = 1;
  public static final int VAR1_TYPE = 2;
  public static final int VAR2_TYPE = 3;
  public static final int VAR1P_TYPE = 4;
  public static final int WAVEFORM_TYPE = 5;
  public static final int SAMPLING_TYPE = 6;

  /**
   * Returns the function type of this (CONS_TYPE, VAR1_TYPE,
   * VAR2_TYPE, VAR1P_TYPE, WAVEFORM_TYPE or SAMPLING_TYPE).
   */
  public abstract int getType();

  /**
   * Accepts a Visitor, according to the Visitor design pattern.
   */
  public abstract void accept(Visitor v);

} // end class SourceFunction
