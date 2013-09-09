/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.util;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

import java.math.BigDecimal;


/**
 * A customized Swing widget for displaying and/or editing EngValues
 * (EngValueField is to EngValue as TextField is to String, sort of).
 */
public class EngValueField extends JPanel
{
  // number of sigfigs to keep from user input
  private static final int NUM_SIGFIGS = 4;

  private EngUnits units;
  private boolean editable, hidden;

  // unitsBox contains choices for powers of 10**3 between
  // 10**(units.getMaxExponent()) and 10**(units.getMinExponent()), so
  // selection index i corresponds to 10**(units.getMaxExponent() -
  // 3i).
  private JComboBox unitsBox;

  private JTextField textField;
  private JLabel label;

  private CardLayout theCardLayout;


  /**
   * Constructs a new EngValueField with default dimensionless units
   * and an initial value of zero.
   */
  public EngValueField()
  {
    this(new EngUnits());
  }



  /**
   * Constructs a new EngValueField with the specified units and an
   * initial value of zero.
   */
  public EngValueField(EngUnits units)
  {
    this.units = units;

    this.editable = true;
    this.hidden  = false;

    // ensure enough horizontal space to display e.g. "-3.275e+235"
    //
    // note: when tweaking this value, be sure to test on solaris and
    // linux as well as on Windows.
    textField = new JTextField(9);
    textField.setMinimumSize(textField.getPreferredSize());

    unitsBox = new JComboBox();

    label = new JLabel();

    JPanel editCard = new JPanel(new GridBagLayout());
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.fill = GridBagConstraints.HORIZONTAL;
    gbc.weightx = 1;
    editCard.add(textField, gbc);
    gbc.fill = GridBagConstraints.NONE;
    gbc.weightx = 0;
    editCard.add(unitsBox, gbc);

    theCardLayout = new CardLayout();
    this.setLayout(theCardLayout);
    this.add(editCard, "edit");
    this.add(label, "label");
    this.add(new JLabel(), "blank");

    this.repopulateUnitsBox();
    this.setValue(BigDecimal.valueOf(0));
  }



  public synchronized EngUnits getUnits()
  {
    return units;
  }



  public synchronized void setUnits(EngUnits units)
  {
    if (! units.equals(this.units))
    {
      BigDecimal savedValue = this.getValue().toBigDecimal();
      this.units = units;
      this.repopulateUnitsBox();
      this.setValue(savedValue);
    }
  }



  public synchronized EngValue getValue()
  {
    BigDecimal value;
    try {
      value = EngMath.parseBigDecimal(textField.getText()).movePointRight
	(units.getMaxExponent() - 3 * unitsBox.getSelectedIndex());
    }
    // if text field contents are not a valid decimal number, reset
    // the value of this to zero
    catch (NumberFormatException ex) {
      value = BigDecimal.valueOf(0);
      this.setValue(value);
    }

    return new EngValue(value, this.units, NUM_SIGFIGS);    
  }



  public synchronized void setValue(EngValue value)
  {
    this.setValue(value.toBigDecimal());
    this.setUnits(value.getUnits());
  }



  public synchronized void setValue(BigDecimal value)
  {
    EngValue ev = new EngValue(value, this.units, NUM_SIGFIGS);
    int engExponent = ev.getEngExponent();

    int min = units.getMinExponent();
    int max = units.getMaxExponent();

    // Most numbers will be represented with mantissa in text box and
    // exponent in units box.
    String text = ev.getEngMantissa().toString();
    int i = (max - engExponent) / 3;

    // If real exponent is too big or too small for the units box,
    // select one that is available (preferably 0, but if 0 is not
    // available either then use whichever of max or min is closest to
    // the real exponent), and represent the extra orders of magnitude
    // in the text box using E notation from a dimensionless EngUnits.
    if (engExponent < min || engExponent > max)
    {
      int selectableExponent;

      if (0 >= min && 0 <= max)
	selectableExponent = 0;

      else if (engExponent > max)
	selectableExponent = max;

      else
	selectableExponent = min;

      i = (max - selectableExponent) / 3;

      // only append E notation if the value is nonzero
      if (ev.toBigDecimal().signum() != 0)
	text += new EngUnits()
	  .writeEngSuffixString(engExponent - selectableExponent, false);
    }

    // set the contents of the text field
    textField.setText(text);

    // temporarily disable unitsBox's Action to prevent it from being
    // triggered by setSelectedIndex
    Action a = unitsBox.getAction();
    unitsBox.setAction(null);

    // select the appropriate choice from the units box
    unitsBox.setSelectedIndex(i);

    // reenable action
    unitsBox.setAction(a);

    reconcile();
  }



  /**
   * Enables or disables the editing of this EngValueField.
   */
  public void setEditable(boolean editable)
  {
    this.editable = editable;
    reconcile();
  }



  /**
   * If true, causes this to display as a blank component.
   */
  public void setHidden(boolean hidden)
  {
    this.hidden = hidden;
    reconcile();
  }



  // somehow this gets called within the constructor, so allow for the
  // fact that components might not be initialized yet.  Not sure why.
  public void setForeground(Color c)
  {
    if (textField != null)
      textField.setForeground(c);
    if (unitsBox != null)
      unitsBox.setForeground(c);
    if (label != null)
      label.setForeground(c);
  }



  /**
   * Sets the Action associated with this.  Note: the action is only
   * triggered when the user enters a new value or selects units.
   * Calling setValue() or setUnits() does not trigger the Action.
   */
  public synchronized void setAction(Action a)
  {
    textField.setAction(a);
    unitsBox.setAction(a);
  }



  /**
   * Sets the alignment to use for displaying this when this is not
   * editable.  For possible values see
   * JLabel.setHorizontalAlignment(int alignment)
   */
  public void setHorizontalAlignment(int alignment)
  {
    label.setHorizontalAlignment(alignment);
  }



  private void reconcile()
  {
    if (hidden) {
      theCardLayout.show(this, "blank");
    }
    else if (editable) {
      theCardLayout.show(this, "edit");
    }
    else {
      theCardLayout.show(this, "label");
      label.setText(this.getValue().toString());
    }

    this.repaint();
  }



  private void repopulateUnitsBox()
  {
    // temporarily disable unitsBox's Action to prevent it from being
    // triggered by what we're about to do
    Action a = unitsBox.getAction();
    unitsBox.setAction(null);

    // clear the unitsBox
    unitsBox.removeAllItems();

    String nextItem;

    // add choices for all powers of 3 between the max exponent and
    // min exponent specified by the EngUnits
    for(int n = units.getMaxExponent(); n >= units.getMinExponent(); n -= 3)
    {
      nextItem = units.writeEngSuffixString(n, false);

      // provide visual feedback instead of the empty suffix "" for
      // dimensionless e+0
      if (nextItem.equals(""))
	nextItem = "--";

      unitsBox.addItem(nextItem);
    }

    // width of contents may have just changed; recalculate width and
    // redo layout
    unitsBox.setSize(unitsBox.getMinimumSize());
    unitsBox.invalidate();

    // reenable Action
    unitsBox.setAction(a);
  }

} // end class EngValueField
