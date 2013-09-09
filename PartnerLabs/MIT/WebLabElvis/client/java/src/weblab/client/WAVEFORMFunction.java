package weblab.client;


import java.math.BigDecimal;

/**
 * Represents a waveform type specification function. A waveform 
 * contains the type of the waveform, the amplitude,
 * the frequency and the offset values
 * 
 * WAVEFORMFunction is immutable
 * @author gikandi
 *
 */
public class WAVEFORMFunction extends SourceFunction {

	public static final int SINE_WAVE = 1;
	public static final int SQUARE_WAVE = 2;
	public static final int TRIANGULAR_WAVE = 3;
	
	private int waveformType;
	private BigDecimal frequency;
	private BigDecimal amplitude;
	private BigDecimal offset;
	
	/**
	 * Constructs a WAVEFORMFunction with the default values
	 */
	public WAVEFORMFunction(){
		this(SINE_WAVE, BigDecimal.valueOf(0), BigDecimal.valueOf(0), BigDecimal.valueOf(0));
	}
	
	/**
	 * Constructs a WAVEFORMFunction using the values passed in 
	 */
	public WAVEFORMFunction(int waveformType, BigDecimal frequency, BigDecimal amplitude, BigDecimal offset){
		this.waveformType = waveformType;
		this.frequency = frequency;
		this.amplitude = amplitude;
		this.offset = offset;
	}
	
	public int getType() {
		return WAVEFORM_TYPE;
	}

	public int getWaveformType() {
		return waveformType;
	}
	
	public BigDecimal getFrequency() {
		return frequency;
	}
	
	public BigDecimal getAmplitude() {
		return amplitude;
	}
	
	public BigDecimal getOffset() {
		return offset;
	}
   // two WAVEFORMFunctions are equal if they have equal values.
   public final boolean equals(Object obj)
  {
    if (obj instanceof WAVEFORMFunction)
    {
      WAVEFORMFunction f = (WAVEFORMFunction) obj;

      return (this.waveformType == f.waveformType && 
    		  this.frequency.compareTo(f.frequency) == 0 &&
    		  this.amplitude.compareTo(f.amplitude) == 0 &&
    		  this.offset.compareTo(f.offset) == 0 );
    }
    else
      return false;
  }
    /**
    * Accepts a Visitor, according to the Visitor design pattern.
    */
	public void accept(Visitor v) {
		// TODO Auto-generated method stub
		v.visitWAVEFORMFunction(this);
	}

}
