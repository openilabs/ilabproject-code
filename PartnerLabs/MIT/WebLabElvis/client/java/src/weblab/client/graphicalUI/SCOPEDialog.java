package weblab.client.graphicalUI;

import weblab.client.WeblabClient;
import weblab.client.SCOPE;
import weblab.client.SourceFunction;
import weblab.client.SAMPLINGFunction;
import weblab.client.DefaultVisitor;

import weblab.util.EngValue;
import weblab.util.EngValueField;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import javax.swing.border.EmptyBorder;

import java.util.Observer;
import java.util.Observable;

import java.math.BigDecimal;


public class SCOPEDialog extends JDialog implements Observer
{
  protected SCOPE mySCOPE;

  //JTextField vName;
  //JCheckBox vDownload;

  private JLabel points;

  private JComboBox samplingRate; // contains type EngValue

  //private EngValueField samplingRate;
  private EngValueField samplingTime, interval;

  private JPanel functionPanel;
  private CardLayout functionPanelCardLayout;

  /**
   * 
   */
  private final Action OK = new AbstractAction("OK")
    {
      public void actionPerformed(ActionEvent evt) {
    	  // first Apply, then close the dialog
    	  exportValues(mySCOPE);
    	  mySCOPE.accept(new ImportValuesVisitor());
    	  setVisible(false);
      }
    };

  /**
   * 
   */
  private final Action APPLY = new AbstractAction("Apply")
    {
      public void actionPerformed(ActionEvent evt) {
    	  exportValues(mySCOPE);
    	  mySCOPE.accept(new ImportValuesVisitor());
      }
    };

  /**
   * 
   */
  private final Action CANCEL = new AbstractAction("Cancel")
    {
      public void actionPerformed(ActionEvent evt) {
    	  mySCOPE.accept(new ImportValuesVisitor());
    	  setVisible(false);
      }
    };


  public SCOPEDialog(Frame owner, SCOPE mySCOPE)
  {
    // create a non-modal dialog
    super(owner, "SCOPE Configuration", false);

    this.mySCOPE = mySCOPE;

    GridBagConstraints gbc;

    //vName = new JTextField(6);
    //vDownload = new JCheckBox("Download");

    // initialize value fields
    samplingTime = new EngValueField(WeblabClient.TIME_UNITS);
    //samplingRate = new EngValueField(WeblabClient.FREQUENCY_UNITS);

    interval = new EngValueField(WeblabClient.TIME_UNITS);
    interval.setEditable(false);
    interval.setHorizontalAlignment(SwingConstants.CENTER);

    // initialize samplingRate and populate with three choices per
    // decade: 10, 20, 50, 100, 200, 500, ... 10k, 20k, 50k, 100k
    samplingRate = new JComboBox();
    samplingRate.addItem
      (new EngValue(BigDecimal.valueOf(100000),
		    WeblabClient.FREQUENCY_UNITS, 1));
    for (int n = 4; n >= 1; n--)
    {
      samplingRate.addItem
	(new EngValue(BigDecimal.valueOf(5).movePointRight(n),
		      WeblabClient.FREQUENCY_UNITS, 1));
      samplingRate.addItem
	(new EngValue(BigDecimal.valueOf(2).movePointRight(n),
		      WeblabClient.FREQUENCY_UNITS, 1));
      samplingRate.addItem
	(new EngValue(BigDecimal.valueOf(1).movePointRight(n),
		      WeblabClient.FREQUENCY_UNITS, 1));
    }

    points = new JLabel();
    points.setHorizontalAlignment(SwingConstants.CENTER);

    // set up layout
    //functionPanelCardLayout = new CardLayout();
    functionPanel = new JPanel(functionPanelCardLayout);
    functionPanel.setLayout(new BoxLayout(functionPanel, BoxLayout.Y_AXIS));
    functionPanel.add(makeSpecRow("Sampling Rate", samplingRate));
    functionPanel.add(makeSpecRow("Duration", samplingTime));
    functionPanel.add(makeSpecRow("Number of points", points));
    functionPanel.add(makeSpecRow("Interval between points", interval));
    // top, left, bottom, right
    functionPanel.setBorder(new EmptyBorder(2,2,2,0));
    functionPanel.add(Box.createVerticalGlue());

    JPanel vPanel = new JPanel(new GridLayout(1,1));
    vPanel.add(new JLabel("Output Voltage (Vout)\n Please specify sampling parameters at the output:"));
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
    mySCOPE.accept(new ImportValuesVisitor());
    // start observing mySMU for changes
    mySCOPE.addObserver(this);
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
   * Exports values from the SCOPEDialog and writes them to SCOPE.
   */
  protected void exportValues(SCOPE f)
  {
    f.setVName("Vout");
    f.setVDownload(true);

    // export source function
    EngValue selectedRate = (EngValue) samplingRate.getSelectedItem();
    f.setFunction
      (new SAMPLINGFunction(selectedRate.toBigDecimal(),
			    samplingTime.getValue().toBigDecimal()));
    // notify observers that myFGEN has changed
    mySCOPE.notifyObservers();
  }



  /**
   * When instrument data changes, re-import values
   */
  public void update(Observable o, Object arg)
  {
    mySCOPE.accept(new ImportValuesVisitor());
  }



  /**
   * Reads values from the object visited and imports them into the
   * FGENDialog.
   */
  protected class ImportValuesVisitor extends DefaultVisitor
  {

    public void visitSCOPE(SCOPE s)
    {
      // set all source-function-related values to defaults, so that any
      // values we don't write over will be in a reasonable state
      
      s.getFunction().accept(this);
      
      //vName.setText(f.getVName());
      //vDownload.setSelected(f.getVDownload());
    }

    public void visitSAMPLINGFunction(SAMPLINGFunction f)
    {
      BigDecimal rate = f.getRate();

      // try to select this rate from the list of available choices
      boolean foundIt = false;
      for (int i = 0; i < samplingRate.getItemCount(); i++)
      {
	EngValue potentialRate = (EngValue) samplingRate.getItemAt(i);
	if (potentialRate.toBigDecimal().compareTo(rate) == 0)
	{
	  foundIt = true;
	  samplingRate.setSelectedIndex(i);
	}
      }
      if (! foundIt)
      {
	// this rate is not a choice, so just select the top choice.
	samplingRate.setSelectedIndex(0);
      }

      samplingTime.setValue(f.getTime());
      points.setText(Integer.toString(f.calculatePoints()));
      interval.setValue(f.calculateInterval());
    }

  } // end inner class ImportValuesVisitor

} // end class FGENDialog
