package weblab.client.graphicalUI;

import weblab.client.WeblabClient;
import weblab.client.FGEN;
import weblab.client.SourceFunction;
import weblab.client.WAVEFORMFunction;
import weblab.client.DefaultVisitor;

import weblab.util.EngValueField;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import javax.swing.border.EmptyBorder;

import java.util.Observer;
import java.util.Observable;


public class FGENDialog extends JDialog implements Observer
{
  protected FGEN myFGEN;

  //JTextField vName;
  //JCheckBox vDownload;
  JComboBox waveformType;

  EngValueField frequency, amplitude, offset;

  JPanel functionPanel;
  CardLayout functionPanelCardLayout;

  /**
   * 
   */
  private final Action OK = new AbstractAction("OK")
    {
      public void actionPerformed(ActionEvent evt) {
    	  // first Apply, then close the dialog
    	  exportValues(myFGEN);
    	  myFGEN.accept(new ImportValuesVisitor());
    	  setVisible(false);
      }
    };

  /**
   * 
   */
  private final Action APPLY = new AbstractAction("Apply")
    {
      public void actionPerformed(ActionEvent evt) {
    	  exportValues(myFGEN);
    	  myFGEN.accept(new ImportValuesVisitor());
      }
    };

  /**
   * 
   */
  private final Action CANCEL = new AbstractAction("Cancel")
    {
      public void actionPerformed(ActionEvent evt) {
    	  myFGEN.accept(new ImportValuesVisitor());
    	  setVisible(false);
      }
    };


  public FGENDialog(Frame owner, FGEN myFGEN)
  {
    // create a non-modal dialog
    super(owner, "FGEN Configuration", false);

    this.myFGEN = myFGEN;

    GridBagConstraints gbc;

    //vName = new JTextField(6);
    //vDownload = new JCheckBox("Download");

    waveformType = new JComboBox();
    waveformType.setEditable(true);
    waveformType.addItem("SINE");
    waveformType.addItem("SQUARE");
    waveformType.addItem("TRIANGULAR");
    
    // initialize value fields
    frequency = new EngValueField(WeblabClient.FREQUENCY_UNITS);
    amplitude = new EngValueField(WeblabClient.VOLTAGE_UNITS);
    offset = new EngValueField(WeblabClient.VOLTAGE_UNITS);
    
    // set up layout
    //functionPanelCardLayout = new CardLayout();
    functionPanel = new JPanel(functionPanelCardLayout);
    functionPanel.setLayout(new BoxLayout(functionPanel, BoxLayout.Y_AXIS));
    functionPanel.add(makeSpecRow("WaveForm", waveformType));
    functionPanel.add(makeSpecRow("Frequency", frequency));
    functionPanel.add(makeSpecRow("Amplitude", amplitude));
    functionPanel.add(makeSpecRow("Offset", offset));
    // top, left, bottom, right
    functionPanel.setBorder(new EmptyBorder(2,2,2,0));
    functionPanel.add(Box.createVerticalGlue());

    JPanel vPanel = new JPanel(new GridLayout(1,1));
    vPanel.add(new JLabel("Input Voltage (Vin):"));
    //vPanel.add(vName);
    //vPanel.add(vDownload);
    // top, left, bottom, right
    vPanel.setBorder(new EmptyBorder(0,0,2,2));

    JPanel topPanel1 = new JPanel(new GridLayout(1,1));
    topPanel1.add(vPanel);

    JPanel topPanel2 = new JPanel(new GridLayout(1,1));
    topPanel2.add(functionPanel);

    JPanel mainPanel = new JPanel();
    mainPanel.setLayout(new BoxLayout(mainPanel, BoxLayout.Y_AXIS));
    mainPanel.add(topPanel1);
    mainPanel.add(topPanel2);
    //mainPanel.add(Box.createVerticalStrut(5));
    //mainPanel.add(new JSeparator());
    mainPanel.add(Box.createVerticalStrut(5));
    // top, left, bottom, right
    mainPanel.setBorder(new EmptyBorder(2,2,2,2));

    //    JScrollPane scrollPane = new JScrollPane(mainPanel);

    JPanel buttonPanel = new JPanel();
    buttonPanel.add(new JButton(OK));
    buttonPanel.add(new JButton(APPLY));
    buttonPanel.add(new JButton(CANCEL));

    // layout dialog
    Container c = this.getContentPane();
    c.setLayout(new BoxLayout(c, BoxLayout.Y_AXIS));
    c.add(mainPanel);
    c.add(Box.createVerticalStrut(5));
    c.add(new JSeparator());
    c.add(buttonPanel);

    this.pack();

    // initialize settings
    myFGEN.accept(new ImportValuesVisitor());
    // start observing mySMU for changes
    myFGEN.addObserver(this);
    // fire an update action
    //UPDATE_CARDS.actionPerformed(null);
    repaint();
  }



  private static JPanel makeSpecRow(String labelText, JComponent spec)
  {
    // my attempts to get something where 1/3 of the space is used for
    // the label and 2/3 for spec, *evenly*
    /*
    JPanel result = new JPanel(new GridBagLayout());
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.fill = GridBagConstraints.BOTH;
    gbc.anchor = GridBagConstraints.WEST;
    gbc.gridwidth = 1;
    gbc.weightx = 1;
    result.add(new JLabel(labelText), gbc);
    gbc.fill = GridBagConstraints.BOTH;
    gbc.anchor = GridBagConstraints.WEST;
    gbc.gridwidth = 2;
    gbc.weightx = 1;
    result.add(spec, gbc);
    return result;
    */
    // temporary fix, until I figure out how to do this properly...
    JPanel result = new JPanel(new GridLayout(1,2));
    result.add(new JLabel(labelText));
    result.add(spec);
    // top, left, bottom, right
    result.setBorder(new EmptyBorder(1,0,1,0));
    return result;
  }



  /**
   * Exports values from the FGENDialog and writes them to FGEN.
   */
  protected void exportValues(FGEN f)
  {
    f.setVName("Vin");
    f.setVDownload(true);
    
    // export source function
    // get the waveformType selected
    
    int wfType = 0;
    if (waveformType.getSelectedItem().equals("SINE")) {
    	wfType = WAVEFORMFunction.SINE_WAVE;
     }else if (waveformType.getSelectedItem().equals("SQUARE")) {
    	wfType = WAVEFORMFunction.SQUARE_WAVE;
     }else if (waveformType.getSelectedItem().equals("TRIANGULAR")) {
    	wfType = WAVEFORMFunction.TRIANGULAR_WAVE;
     }else {
        //assert false;
    	throw new Error("impossible waveformType");
     }
    
    f.setFunction(new WAVEFORMFunction(wfType, frequency.getValue().toBigDecimal(), amplitude.getValue().toBigDecimal(), offset.getValue().toBigDecimal()));
    // notify observers that myFGEN has changed
    myFGEN.notifyObservers();
  }



  /**
   * When instrument data changes, re-import values
   */
  public void update(Observable o, Object arg)
  {
    myFGEN.accept(new ImportValuesVisitor());
  }



  /**
   * Reads values from the object visited and imports them into the
   * FGENDialog.
   */
  protected class ImportValuesVisitor extends DefaultVisitor
  {

    public void visitFGEN(FGEN f)
    {
      // set all source-function-related values to defaults, so that any
      // values we don't write over will be in a reasonable state
      
      f.getFunction().accept(this);
      
      //vName.setText(f.getVName());
      //vDownload.setSelected(f.getVDownload());
    }

    public void visitWAVEFORMFunction(WAVEFORMFunction f)
    {
    	switch(f.getWaveformType()) {
    		case WAVEFORMFunction.SINE_WAVE:
    			waveformType.setSelectedItem("SINE");
    			break;
    		case WAVEFORMFunction.SQUARE_WAVE:
    			waveformType.setSelectedItem("SQUARE");
    			break;
    		case WAVEFORMFunction.TRIANGULAR_WAVE:
    			waveformType.setSelectedItem("TRIANGULAR");
    			break;
	    	 default:
	    		 throw new Error("impossible waveformtype");
	    	 	//assert false;
    	}
        frequency.setValue(f.getFrequency());
        amplitude.setValue(f.getAmplitude());
        offset.setValue(f.getOffset());
    }

  } // end inner class ImportValuesVisitor

} // end class FGENDialog
