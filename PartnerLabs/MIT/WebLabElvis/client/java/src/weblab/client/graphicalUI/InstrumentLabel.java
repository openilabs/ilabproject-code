package weblab.client.graphicalUI;

import weblab.client.WeblabClient;
import weblab.client.SourceFunction;
import weblab.client.WAVEFORMFunction;
import weblab.client.SAMPLINGFunction;
import weblab.client.CONSFunction;
import weblab.client.VAR1Function;
import weblab.client.VAR2Function;
import weblab.client.VAR1PFunction;
import weblab.client.Instrument;
import weblab.client.FGEN;
import weblab.client.SCOPE;
import weblab.client.DefaultVisitor;

import weblab.util.EngMath;
import weblab.util.EngValue;
import weblab.util.EngUnits;

import java.math.BigDecimal;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

import java.util.Observable;
import java.util.Observer;


public abstract class InstrumentLabel extends JPanel implements Observer
{
  protected Frame theMainFrame;
  protected JLabel imageLabel;

  protected Instrument myInstrument;

  final int HEIGHT = 100;
  final int WIDTH = 50;

  final int NUM_SIGFIGS = 4;

  public InstrumentLabel(Frame theMainFrame, Instrument myInstrument, String name)
  {
    this.theMainFrame = theMainFrame;
    this.myInstrument = myInstrument;

    this.setLayout(new BoxLayout(this, BoxLayout.Y_AXIS));
 
    Dimension d = new Dimension(WIDTH, HEIGHT);
    this.setMinimumSize(d);
    this.setPreferredSize(d);
    this.setMaximumSize(d);

    // note: can't actually set image in this constructor, because we
    // won't know which image to use until subclass constructor
    // finishes!
    this.imageLabel = new JLabel();
    this.add(imageLabel);
    imageLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

    JLabel nameLabel = new JLabel(name);
    nameLabel.setHorizontalAlignment(SwingConstants.CENTER);
    this.add(nameLabel);
    nameLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

    // add a listener to update the image and tooltip when the instrument
    // data changes
    myInstrument.addObserver(this);

    this.setOpaque(false);
  }

  public Instrument getInstrument()
  {
    return myInstrument;
  }



  /**
   * When instrument data changes, update image and tooltip.
   */
  public void update(Observable o, Object arg)
  {
    updateImage();
    updateToolTip();
  }



  protected String chooseImageNameForCurrentSourceFunction(SourceFunction f)
  {
    if (f instanceof CONSFunction) {
      return "instrument_ConstantCurrentSource.gif";
    }
    else {
      // must be variable source function
      return "instrument_VariableCurrentSource.gif";
    }
  }

  protected String chooseImageNameForVoltageSourceFunction(SourceFunction f)
  {
    if (f instanceof CONSFunction) {
      return "instrument_ConstantVoltageSource.gif";
    }
    else {
      // must be variable source function
      return "instrument_VariableVoltageSource.gif";
    }
  }

  protected String chooseImageNameForVoltageOutputFunction(SourceFunction f)
  {
      // use oscilloscope image 
      return "instrument_Oscilloscope.gif";
  }
  protected abstract String chooseImageName();

  /**
   * Performs cleanup of UI elements associated with this
   * (i.e. closing the instrument dialog if it's open) in preparation for
   * this being removed from the UI.
   */
  public abstract void cleanup();


  /**
   * Sets the tooltip text to a multi-line summary of the instrument setup.
   */
  public void updateToolTip()
  {
    this.setToolTipText
      ((new DefaultVisitor()
	{
	  private StringBuffer summary;
	  private EngUnits units;
	
	  public String go(Instrument p)
	  {
	    summary = new StringBuffer();
	    p.accept(this);
	    return summary.toString();
	  }

	  public void visitFGEN(FGEN f)
	  {
	    summary.append("ELVIS FGEN\n");
	    // visit function
	    f.getFunction().accept(this);
	  }

	  public void visitSCOPE(SCOPE s)
	  {
	    summary.append("ELVIS SCOPE\n");
	    // visit function
	    s.getFunction().accept(this);
	  }
	  
	  public void visitWAVEFORMFunction(WAVEFORMFunction f)
	  {
	    summary.append("Waveform: ");
	    switch(f.getWaveformType()) {
	    case WAVEFORMFunction.SINE_WAVE:
	      summary.append("SINE");
	      break;
	    case WAVEFORMFunction.SQUARE_WAVE:
	      summary.append("SQUARE");
	      break;
	    case WAVEFORMFunction.TRIANGULAR_WAVE:
	      summary.append("TRIANGULAR");
	      break;
	    default:
	    	throw new Error("impossible waveform");
	    	//assert false; // impossible scale
	    }
	    summary.append("\n");
	    summary.append
	      ("Frequency: " + engString
	       (f.getFrequency(), WeblabClient.FREQUENCY_UNITS) + "\n");
	    summary.append
	      ("Amplitude: " + engString
	       (f.getAmplitude(), WeblabClient.VOLTAGE_UNITS) + "\n");
	    summary.append
	      ("Offset: " + engString
	       (f.getOffset(), WeblabClient.VOLTAGE_UNITS));
	  }

	  public void visitSAMPLINGFunction(SAMPLINGFunction s)
	  {
	    summary.append
	      ("Sampling Rate: " + engString
	       (s.getRate(), WeblabClient.FREQUENCY_UNITS) + "\n");
	    summary.append
	      ("Duration: " + engString
	       (s.getTime(), WeblabClient.TIME_UNITS) + "\n");
	    summary.append("Number of points: " + s.calculatePoints() + "\n");
	    summary.append
	      ("Interval between points: " + engString
	       (s.calculateInterval(), WeblabClient.TIME_UNITS));
	  }

	  
	  public void visitCONSFunction(CONSFunction f)
	  {
	    summary.append("Function: CONS\n");
	    summary.append("Value: " +
			   engString(f.getValue(), units));
	  }

	  public void visitVAR1Function(VAR1Function f)
	  {
	    summary.append("Function: VAR1\n");

	    summary.append("Scale: ");
	    switch(f.getScale()) {
	    case VAR1Function.LIN_SCALE:
	      summary.append("LIN");
	      break;
	    case VAR1Function.LOG10_SCALE:
	      summary.append("LOG10");
	      break;
	    case VAR1Function.LOG25_SCALE:
	      summary.append("LOG25");
	      break;
	    case VAR1Function.LOG50_SCALE:
	      summary.append("LOG50");
	      break;
	    default:
	    	throw new Error("impossible scale");
	    	//assert false; // impossible scale
	    }
	    summary.append("\n");

	    summary.append("Start: " +
			   engString(f.getStart(), units) + "\n");
	    summary.append("Stop: " +
			   engString(f.getStop(), units) + "\n");

	    if (f.getScale() == VAR1Function.LIN_SCALE) {
	      summary.append("Step: " +
			     engString(f.getStep(), units) + "\n");
	    }

	    summary.append("Points: " + f.calculatePoints());
	  }

	  public void visitVAR2Function(VAR2Function f)
	  {
	    summary.append("Function: VAR2\n");
	    summary.append("Scale: Linear\n");
	    summary.append("Start: " +
			   engString(f.getStart(), units) + "\n");
	    summary.append("Stop: " +
			   engString(f.getStop(), units) + "\n");
	    summary.append("Step: " +
			   engString(f.getStep(), units) + "\n");
	    summary.append("Points: " + f.calculatePoints());
	  }

	  public void visitVAR1PFunction(VAR1PFunction f)
	  {
	    summary.append("Function: VAR1P\n");
	    summary.append("Ratio: " + f.getRatio()+ "\n");
	    summary.append("Offset: " +
			   engString(f.getOffset(), units));
	  }

	  // helper method for applying units to numerical values
	  private final String engString(BigDecimal val, EngUnits units)
	  {
	    return (new EngValue(val, units, NUM_SIGFIGS)).toString();
	  }
	}).go(myInstrument));
  }



  public void updateImage()
  {
    // figure out which image to use
    String imageName = this.chooseImageName();

    // load the image
    try {
      ImageIcon instrumentImageIcon = new ImageIcon(this.getClass().getResource
					      ("/img/" + imageName));

      // note: it's important to do setSize *before* setIcon;
      // otherwise the icon takes on the size of the label rather than
      // dictating the size of the label.  <dmrz>
      imageLabel.setSize(instrumentImageIcon.getIconWidth(),
    		  instrumentImageIcon.getIconHeight());
      imageLabel.setIcon(instrumentImageIcon);

    }
    catch (Exception e) {
      String error ="Unable to load image icon: " + imageName;
      e.printStackTrace();
      JOptionPane.showMessageDialog(this, error, "Error", JOptionPane.ERROR_MESSAGE);
    }

    // repaint
    repaint();
  }

} // end class InstrumentLabel
