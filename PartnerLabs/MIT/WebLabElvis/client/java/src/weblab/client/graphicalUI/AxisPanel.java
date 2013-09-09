package weblab.client.graphicalUI;

import weblab.client.WeblabAxis;

import weblab.graphing.Axis;
import weblab.graphing.Grid;

import weblab.util.ChangeTrackingObservable;
import weblab.util.EngMath;
import weblab.util.EngValue;
import weblab.util.EngUnits;
import weblab.util.EngValueField;

import java.awt.*;
import javax.swing.*;
import java.awt.event.*;

import java.math.BigDecimal;

import java.util.Enumeration;
import java.util.Observable;
import java.util.Observer;



public class AxisPanel extends JPanel implements Observer
{
  // number of sigfigs to display for tracking and ndivs
  private static final int NUM_SIGFIGS = 4;

  private WeblabAxis myAxis;
  private Grid theGrid;

  JComboBox variable;
  JComboBox scale;
  EngValueField min, max;
  JCheckBox autoscale;

  JLabel scaleLabel, axisLabel, divLabel, trackingLabel;
    
  JLabel divValue, trackingValue;

  // true for a horizontal axis, false for vertical
  private boolean isHorizontal;

  private Color color;

  /////////////
  // Actions //
  /////////////

  private Action APPLY = new AbstractAction()
    {
      public void actionPerformed(ActionEvent evt)
      {
	if (variable.getSelectedIndex() == 0) {
	  // no variable selected
	  myAxis.setVariable(null);
	}
	else {
	  // set variable
	  myAxis.setVariable((String)variable.getSelectedItem());
	}

	myAxis.setContinuousAutoscale(autoscale.isSelected());

	myAxis.setMin(min.getValue().toBigDecimal());
	myAxis.setMax(max.getValue().toBigDecimal());

	// set scale
	if (scale.getSelectedItem().equals("Linear")) {
	  myAxis.setScale(Axis.LINEAR_SCALE);
	}
	else if (scale.getSelectedItem().equals("Logarithmic")) {
	  myAxis.setScale(Axis.LOG_SCALE);
	}
	else {
	  throw new Error("illegal scale");
	  //assert false : "illegal scale";
	}

	// notify observers that myAxis may have changed
	myAxis.notifyObservers();

	// make sure everything displayed is accurate
	revert();
      }
    };
  
  //////////////////
  // Constructors //
  //////////////////
  
  public AxisPanel(WeblabAxis theAxis, boolean horizontal, Color color,
		   Grid theGrid)
  {
    this.myAxis = theAxis;
    this.isHorizontal = horizontal;
    this.color = color;

    this.theGrid = theGrid;

    //
    // setup components
    //

    axisLabel = new JLabel(myAxis.getName() + " Axis: ");

    scaleLabel = new JLabel("Scale: ");

    trackingLabel = new JLabel("Tracking: ");

    divLabel = new JLabel(" / division");

    divValue = new JLabel();

    trackingValue = new SmartJLabel();
    trackingValue.setHorizontalAlignment(SwingConstants.CENTER);
    // ensure enough width to (probably) avoid resizing
    trackingValue.setText("00.0000000 mm");
    trackingValue.setMinimumSize(trackingValue.getPreferredSize());

    // initialize to null units, since this requires more space to
    // display than typical Weblab units (which are usually only one
    // character)
    min = new EngValueField();
    min.setAction(APPLY);

    max = new EngValueField();
    max.setAction(APPLY);

    scale = new JComboBox();
    scale.setEditable(false);
    scale.addItem("Linear");
    scale.addItem("Logarithmic");
    scale.setAction(APPLY);

    variable = new JComboBox();
    variable.setEditable(false);
    variable.addItem("None ");
    variable.setAction(APPLY);

    // note: have to set the text *after* setAction because setAction
    // pulls text from the Action
    autoscale = new JCheckBox();
    autoscale.setAction(APPLY);
    autoscale.setText("autoscale");

    //
    // setup listeners
    //

    myAxis.addObserver(this);
    theGrid.addObserver(this);

    //      
    // setup colors
    //

    min.setForeground(color);
    max.setForeground(color);
    divValue.setForeground(color);
    trackingValue.setForeground(color);

    //
    // setup layout
    //

    if (this.isHorizontal)
      // do layout for a horizontal AxisPanel
      {
	min.setHorizontalAlignment(SwingConstants.LEFT);
	max.setHorizontalAlignment(SwingConstants.RIGHT);
	
	JPanel row1 = new JPanel(new GridBagLayout());
	JPanel row2 = new JPanel(new GridBagLayout());

	GridBagConstraints gbc = new GridBagConstraints();
	GridBagConstraints glue_gbc = new GridBagConstraints();
	glue_gbc.weightx = 1;

	JPanel currentRow = row1;
	
	currentRow.add(min, gbc);

	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(divValue, gbc);
	currentRow.add(divLabel, gbc);
	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(max, gbc);
	
	currentRow = row2;
	
	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(axisLabel, gbc);
	currentRow.add(variable, gbc);
	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(scaleLabel, gbc);
	currentRow.add(scale, gbc);
	currentRow.add(Box.createGlue(), glue_gbc);
	currentRow.add(autoscale, gbc);
	currentRow.add(Box.createGlue(), glue_gbc);
	
	this.setLayout(new GridLayout(2,1));
	this.add(row1);
	this.add(row2);
      }
    else
      // layout for a vertical AxisPanel
      {
      min.setHorizontalAlignment(SwingConstants.CENTER);
      max.setHorizontalAlignment(SwingConstants.CENTER);

      this.setLayout(new GridBagLayout());

      GridBagConstraints gbc = new GridBagConstraints();
      gbc.gridx = 0;

      GridBagConstraints glue_gbc = new GridBagConstraints();
      glue_gbc.gridx = 0;
      glue_gbc.weighty = 1;

      this.add(max, gbc);
      // instead of just adding glue here, add a strut to ensure that
      // the AxisPanel always remains at least as wide as the current
      // preferred size of the EngValueField (with null units).  This
      // gets rid of the "GUI rhumba" effect.  <dmrz>
      this.add(Box.createHorizontalStrut(max.getPreferredSize().width),
	       glue_gbc);
      this.add(axisLabel, gbc);
      this.add(variable, gbc);
      this.add(Box.createVerticalStrut(10), gbc);
      this.add(scaleLabel, gbc);
      this.add(scale, gbc);
      this.add(autoscale, gbc);
      this.add(Box.createGlue(), glue_gbc);
      this.add(divValue, gbc);
      this.add(divLabel, gbc);
      this.add(Box.createGlue(), glue_gbc);
      this.add(min, gbc);
    }

    //
    // initialize
    //

    updateVariables();
  }



  // sets tracking to the nth data point, if possible
  public final String trackDataPoint(int n)
  {
    BigDecimal[] data = myAxis.getData();
    String trackingText;
    if (n < data.length)
    {
      trackingText =
	(new EngValue(data[n], new EngUnits(myAxis.getUnits()), NUM_SIGFIGS))
	.toString();
    }
    else
      trackingText = "---";

    return trackingText;
  }



  // sets tracking based on mouse location
  public final String trackMouseLocation(Component canvas, Point p)
  {
    String trackingText;

    if (myAxis.getVariable() == null)
      trackingText = "---";
    else
    {
      BigDecimal value;

      if (isHorizontal)
	value = myAxis.scaleToDataValue
	  (p.x, 0, canvas.getSize().width - 1, 4);
      else
	value = myAxis.scaleToDataValue
	  (p.y, canvas.getSize().height - 1, 0, 4);

      trackingText =
	(new EngValue(value, new EngUnits(myAxis.getUnits()), NUM_SIGFIGS))
	.toString();
    }

    return trackingText;
  }



  /**
   * Handle updates from Observables
   */
  public final void update(Observable o, Object arg)
  {
    // When Grid changes, re-import values
    if (o == theGrid)
      revert();

    // When myAxis changes, re-import values and possibly update
    // variable choices
    if (o == myAxis)
    {
      if (ChangeTrackingObservable.containsChange
	  (arg, WeblabAxis.VARIABLE_CHOICES_CHANGE))
      {
	updateVariables();
      }

      revert();
    }
  }



  ////////////////////
  // Helper Methods //
  ////////////////////

  /**
   * Make sure the state of the components is consistent with respect
   * to which components are displayed, etc.
   */
  private void reconcile()
  {
    if (variable.getSelectedIndex() == 0) {
      // don't show other axis settings
      min.setHidden(true);
      max.setHidden(true);
      //	scaleLabel.setVisible(false);
      //	scale.setVisible(false);
      //	autoscale.setVisible(false);
      divValue.setVisible(false);
      divLabel.setVisible(false);
      trackingLabel.setVisible(false);
      trackingValue.setVisible(false);
    }
    else {
      // show all axis settings
      min.setHidden(false);
      max.setHidden(false);
      //	scaleLabel.setVisible(true);
      //	scale.setVisible(true);
      //	autoscale.setVisible(true);
      divValue.setVisible(true);
      divLabel.setVisible(true);
      trackingLabel.setVisible(true);
      trackingValue.setVisible(true);

      // min and max can be edited manually only if autoscale is not
      // selected
      if (autoscale.isSelected())
      {
	min.setEditable(false);
	max.setEditable(false);
      }
      else
      {
	min.setEditable(true);
	max.setEditable(true);
      }
    }

    // validate (since some components may have just been made visible
    // or invisible) and repaint
    this.validate();
    repaint();
  }



  /**
   * Set component state to accurately reflect the Axis (and
   * Grid).  Does NOT update variable choices.
   */
  private void revert()
  {
    // disable JComboBox Actions so that they won't trigger an APPLY
    // in the middle of revert()
    variable.setAction(null);
    scale.setAction(null);

    String var = myAxis.getVariable();

    if (var == null)
    {
      // no variable selected
      variable.setSelectedIndex(0);
    }
    else
    {
      variable.setSelectedItem(var);
    }

    autoscale.setSelected(myAxis.getContinuousAutoscale());

    min.setValue(myAxis.getMin());
    max.setValue(myAxis.getMax());

    // set units for min and max to match the units of the chosen
    // variable
    String units = myAxis.getUnits();
    min.setUnits(new EngUnits(units));
    max.setUnits(new EngUnits(units));

    int nDivs;
    if (this.isHorizontal)
      nDivs = theGrid.getXDivs();
    else
      nDivs = theGrid.getYDivs();
    BigDecimal divSize = EngMath.round
      (new BigDecimal((double)myAxis.getDivSize(nDivs)), NUM_SIGFIGS);

    // import scale and divValue (which has units based on scale)
    switch(myAxis.getScale())
    {
    case Axis.LINEAR_SCALE:
      scale.setSelectedItem("Linear");
      divValue.setText(new EngValue(divSize, new EngUnits(myAxis.getUnits())).toString());
      break;
    case Axis.LOG_SCALE:
      scale.setSelectedItem("Logarithmic");
      divValue.setText(divSize.toString() + " decades");
      break;
    default:
      throw new Error("illegal scale: " + myAxis.getScale());
    }

    // reenable JComboBox Actions
    variable.setAction(APPLY);
    scale.setAction(APPLY);

    reconcile();
  }



  private void updateVariables()
  {
    // disable JComboBox Action so that it won't trigger an APPLY in
    // the middle of this process
    variable.setAction(null);

    // remove all items except the first one (indicating no variable)
    while (variable.getItemCount() > 1)
      variable.removeItemAt(1);

    Enumeration vars_enum = myAxis.getVariableChoices();
    while(vars_enum.hasMoreElements())
      variable.addItem((String) vars_enum.nextElement());
      
    variable.validate();

    // reenable JComboBox Action
    variable.setAction(APPLY);

    // re-import values to get the currently selected variable right
    revert();
  }



  private class SmartJLabel extends JLabel
  {
    /**
     * Returns the smallest dimension which is at least as big as
     * super.getPreferredSize() and also at least as big as
     * this.getMinimumSize().
     */
    public Dimension getPreferredSize()
    {
      Dimension p = super.getPreferredSize();
      Dimension m = this.getMinimumSize();
      if (m.getWidth() > p.getWidth()) {
	p.setSize(m.getWidth(), p.getHeight());
      }
      if (m.getHeight() > p.getHeight()) {
	p.setSize(p.getWidth(), m.getHeight());
      }
      return p;
    }
  } // end inner class SmartJLabel
    
} // end class AxisPanel
