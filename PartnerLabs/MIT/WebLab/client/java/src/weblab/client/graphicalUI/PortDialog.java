/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

import java.util.Observer;
import java.util.Observable;

import weblab.toolkit.util.EngUnits;
import weblab.toolkit.util.EngValueField;
import weblab.toolkit.util.Spacer;

import weblab.client.*;

/**
 * Abstract superclass of SMUDialog, VSUDialog, and VMUDialog.
 * Contains functionality common to all port dialogs for purposes of
 * reducing code size.
 */
public abstract class PortDialog extends JDialog
  implements Observer, ItemListener, ActionListener
{
  protected Port myPort;

  protected boolean initDone = false;

  // functional components
  protected JTextField vName, iName;
  protected JCheckBox vDownload, iDownload;
  protected JComboBox mode, function, scale;
  protected EngValueField value, start, stop, step, offset,
    ratio, iCompliance, vCompliance;

  protected JLabel points;

  // misc components for layout
  protected JPanel buttonPanel, functionPanel, scalePanel, valuesPanel,
    compliancePanel;
  protected CardLayout functionCardLayout, scaleCardLayout, valuesCardLayout,
    complianceCardLayout;

  // column width markers: must be sure to have width large enough for
  // everything we need to fit in the column, and height of 1 so that
  // GridBagLayout will resize their actual widths (which it
  // apparently doesn't bother doing for zero-height components)
  protected Component leftWidthMarker, rightWidthMarker;



  /**
   * Creates a new PortDialog of the appropriate subtype for the
   * specified Port.
   */
  public static PortDialog createPortDialog(MainFrame theMainFrame, Port p)
  {
    switch(p.getType())
    {
    case Port.SMU_TYPE:
      return new SMUDialog(theMainFrame, (SMU)p);
    case Port.VSU_TYPE:
      return new VSUDialog(theMainFrame, (VSU)p);
    case Port.VMU_TYPE:
      return new VMUDialog(theMainFrame, (VMU)p);
    default:
      throw new Error("illegal port type");
    }
  }



  /**
   * Creates a new PortDialog.
   */
  protected PortDialog(Frame owner, Port p)
  {
    // create a non-modal dialog
    super(owner, false);

    this.myPort = p;

    String title = "Configuring " + p.getName();

    Terminal t = p.getTerminal();
    if (t != null)
      title += " (" + t.getLabel() + ")";

    this.setTitle(title);
  }



  /**
   * This method handles the main work of setting up the PortDialog so
   * that the constructor won't take so long to execute.  This method
   * is automatically invoked when the dialog is made visible for the
   * first time.
   */
  protected void init()
  {
    // create functional components

    vName = new JTextField(6);
    vDownload = new JCheckBox("Download");

    iName = new JTextField(6);
    iDownload = new JCheckBox("Download");

    mode = new JComboBox();
    mode.setEditable(false);
    mode.addItem("V");
    mode.addItem("I");
    mode.addItem("COMM");

    function = new JComboBox();
    function.setEditable(false);
    function.addItem("CONS");
    function.addItem("VAR1");
    function.addItem("VAR2");
    function.addItem("VAR1P");

    scale = new JComboBox();
    scale.setEditable(false);
    scale.addItem("Linear");
    scale.addItem("Log10");
    scale.addItem("Log25");
    scale.addItem("Log50");

    // initialize these with no units, the units will be set later
    // (except for ratio, which is actually dimensionless)
    value = new EngValueField();
    start = new EngValueField();
    stop = new EngValueField();
    step = new EngValueField();
    offset = new EngValueField();
    ratio = new EngValueField();
    // initialize compliance fields with appropriate units
    iCompliance = new EngValueField(WeblabClient.CURRENT_UNITS);
    vCompliance = new EngValueField(WeblabClient.VOLTAGE_UNITS);

    points = new JLabel();
    points.setHorizontalAlignment(SwingConstants.CENTER);

    // create buttons

    JButton okButton = new JButton("OK");
    okButton.setActionCommand("ok");

    JButton applyButton = new JButton("Apply");
    applyButton.setActionCommand("apply");

    JButton cancelButton = new JButton("Cancel");
    cancelButton.setActionCommand("cancel");

    // create column width markers

    leftWidthMarker = Spacer.createHorizontalDoppelganger
      (new JLabel("Compliance"), false, 1);
    // not worried about right column minwidth because the widest
    // thing is an XNameRow which we'll always have at least one of in
    // the main layout
    rightWidthMarker = Box.createVerticalStrut(1);

    // create functionPanel and CardLayout

    functionCardLayout = new CardLayout();
    functionPanel = new JPanel(functionCardLayout);

    JPanel functionCard = new JPanel(new GridBagLayout());
    addRow(functionCard, "Function", function);
    addDoppelgangerRow(functionCard);

    functionPanel.add(functionCard, "function");
    functionPanel.add(new JPanel(), "blank");

    // create scalePanel and CardLayout

    scaleCardLayout = new CardLayout();
    scalePanel = new JPanel(scaleCardLayout);
    scalePanel.add(scale, "scale");
    scalePanel.add(new JLabel("Linear"), "linear");
    scalePanel.add(new JPanel(), "blank");

    // create compliancePanel and CardLayout

    complianceCardLayout = new CardLayout();
    compliancePanel = new JPanel(complianceCardLayout);

    JPanel iComplianceCard = new JPanel(new GridBagLayout());
    addRow(iComplianceCard, "Compliance", iCompliance);
    addDoppelgangerRow(iComplianceCard);

    JPanel vComplianceCard = new JPanel(new GridBagLayout());
    addRow(vComplianceCard, "Compliance", vCompliance);
    addDoppelgangerRow(vComplianceCard);

    compliancePanel.add(iComplianceCard, "iCompliance");
    compliancePanel.add(vComplianceCard, "vCompliance");
    compliancePanel.add(new JPanel(), "blank");

    // create valuesPanel and CardLayout

    valuesCardLayout = new CardLayout();
    valuesPanel = new JPanel(valuesCardLayout);

    JPanel var1or2Card = new JPanel(new GridBagLayout());
    addRow(var1or2Card, "Scale", scalePanel);
    addRow(var1or2Card, "Start", start);
    addRow(var1or2Card, "Stop", stop);
    addRow(var1or2Card, "Step", step);
    addRow(var1or2Card, "Points", points);
    addDoppelgangerRow(var1or2Card);

    JPanel var1pCard = new JPanel(new GridBagLayout());
    addRow(var1pCard, "Ratio", ratio);
    addRow(var1pCard, "Offset", offset);
    addDoppelgangerRow(var1pCard);

    JPanel consCard = new JPanel(new GridBagLayout());
    addRow(consCard, "Value", value);
    addDoppelgangerRow(consCard);

    valuesPanel.add(consCard, "cons");
    valuesPanel.add(var1or2Card, "var1or2");
    valuesPanel.add(var1pCard, "var1p");
    valuesPanel.add(new JPanel(), "blank");

    // layout button panel

    buttonPanel = new JPanel();
    buttonPanel.add(okButton);
    buttonPanel.add(applyButton);
    buttonPanel.add(cancelButton);

    // layout dialog

    Container c = this.getContentPane();
    c.setLayout(new GridBagLayout());
    addInvisibleRow(c, leftWidthMarker, rightWidthMarker);

    this.layoutDialog();

    // set up event listeners

    mode.addItemListener(this);
    function.addItemListener(this);
    scale.addItemListener(this);
    okButton.addActionListener(this);
    applyButton.addActionListener(this);
    cancelButton.addActionListener(this);
    myPort.addWeakReferenceObserver(this);

    // intialize

    revert();
    reconcile();

    this.initDone = true;
  }



  // call when finished with this
  public void destroy()
  {
    myPort.deleteObserver(this);
    this.dispose();
  }



  /**
   * Automatically called by init() to lay out the components of the
   * dialog box.  Implementation varies depending on port type.
   */
  protected abstract void layoutDialog();

  //
  // helper methods for layoutDialog
  //

  protected void addXNameRow(String labelText, Component xName,
			     Component xDownload)
  {
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.anchor = GridBagConstraints.WEST;
    gbc.insets = new Insets(1, 2, 1, 2); // top, left, bottom, right

    Container c = this.getContentPane();

    gbc.gridx = 0;
    gbc.weightx = 1.0;
    gbc.fill = GridBagConstraints.HORIZONTAL;
    c.add(new JLabel(labelText), gbc);
    gbc.gridx++;
    gbc.weightx = 2.0;
    c.add(xName, gbc);
    gbc.gridx++;
    gbc.weightx = 0;
    c.add(Box.createHorizontalStrut(10), gbc);
    gbc.gridx++;
    c.add(xDownload, gbc);
  }

  protected void addRow(Container c, String labelText, Component spec)
  {
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.anchor = GridBagConstraints.WEST;
    gbc.insets = new Insets(1, 2, 1, 2); // top, left, bottom, right

    gbc.gridx = 0;
    gbc.weightx = 1.0;
    gbc.fill = GridBagConstraints.HORIZONTAL;
    c.add(new JLabel(labelText), gbc);
    gbc.gridx++;
    gbc.gridwidth = GridBagConstraints.REMAINDER;
    gbc.weightx = 2.0;
    c.add(spec, gbc);
  }

  protected void addSeparator()
  {
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.gridx = 0;
    gbc.gridy = GridBagConstraints.RELATIVE;
    gbc.insets = new Insets(1, 2, 1, 2); // top, left, bottom, right
    gbc.gridwidth = GridBagConstraints.REMAINDER;
    gbc.fill = GridBagConstraints.HORIZONTAL;

    this.getContentPane().add(new JSeparator(), gbc);
  }

  protected void addVSpace(int pixels)
  {
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.gridx = 0;
    gbc.gridy = GridBagConstraints.RELATIVE;
    gbc.gridwidth = GridBagConstraints.REMAINDER;

    this.getContentPane().add(Box.createVerticalStrut(pixels), gbc);
  }

  protected void addInvisibleRow(Container c, Component i1, Component i2)
  {
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.anchor = GridBagConstraints.WEST;
    gbc.insets = new Insets(0, 2, 0, 2); // top, left, bottom, right

    gbc.gridx = 0;
    gbc.weightx = 1.0;
    gbc.fill = GridBagConstraints.HORIZONTAL;
    c.add(i1, gbc);
    gbc.gridx++;
    gbc.gridwidth = GridBagConstraints.REMAINDER;
    gbc.weightx = 2.0;
    c.add(i2, gbc);
  }

  protected void addDoppelgangerRow(Container c)
  {
    addInvisibleRow
      (c, 
       Spacer.createHorizontalDoppelganger(leftWidthMarker, true),
       Spacer.createHorizontalDoppelganger(rightWidthMarker, true));
  }

  // override setVisible to perform init if it has not yet been done
  public void setVisible(boolean b)
  {
    // if init has not been done, do it now
    if (b && !initDone)
      init();

    super.setVisible(b);
  }



  /**
   * Handle ActionEvents
   */
  public void actionPerformed(ActionEvent evt)
  {
    String cmd = evt.getActionCommand();

    // when OK is clicked, export values and then hide the dialog
    if (cmd.equals("ok"))
    {
      exportValues();
      setVisible(false);
      // re-importing values is not strictly necessary from a logical
      // point of view (the export will cause a change notification if
      // the values have changed), but helps prevent possible
      // weirdness if the user changed a text field in a way that does
      // not actually change the value Weblab gleans from it
      // (i.e. adding an extra digit less than 5 onto the end of a
      // value)
      revert();
    }

    // when Apply is clicked, export values but leave the dialog open
    if (cmd.equals("apply"))
    {
      exportValues();
      // re-importing values is not strictly necessary from a logical
      // point of view (the export will cause a change notification if
      // the values have changed), but helps prevent possible
      // weirdness if the user changed a text field in a way that does
      // not actually change the value Weblab gleans from it
      // (i.e. adding an extra digit less than 5 onto the end of a
      // value)
      revert();
    }

    // when Cancel is clicked, re-import values and then hide the dialog
    if (cmd.equals("cancel"))
    {
      revert();
      setVisible(false);
    }
  }



  /**
   * When a new mode, function, or scale is selected, reconcile.
   */
  public void itemStateChanged(ItemEvent evt)
  {
    reconcile();
  }



  /**
   * When port data changes, re-import values
   */
  public void update(Observable o, Object arg)
  {
    revert();
  }



  /**
   * Updates the status of the CardLayout components and the units of
   * the EngValueFields based on the current mode, function, and scale
   * selections.
   */
  protected abstract void reconcile();

  //
  // helper methods for reconcile
  //

  protected void reconcileUnits(EngUnits modeUnits)
  {
    value.setUnits(modeUnits);
    start.setUnits(modeUnits);
    stop.setUnits(modeUnits);
    step.setUnits(modeUnits);
    offset.setUnits(modeUnits);
  }

  protected void reconcileFunction(boolean showFunction)
  {
    // If appropriate, show Function and show other things depending
    // on which function is selected
    if (showFunction)
    {
      functionCardLayout.show(functionPanel, "function");

      // CONS: show Value
      if (function.getSelectedItem().equals("CONS"))
      {
	valuesCardLayout.show(valuesPanel, "cons");
      }

      // VAR1P: show Ratio and Offset
      else if (function.getSelectedItem().equals("VAR1P"))
      {
	valuesCardLayout.show(valuesPanel, "var1p");
      }

      // VAR1: show scale, show start, stop, and points, and show step
      // iff scale is Linear.
      else if (function.getSelectedItem().equals("VAR1"))
      {
	valuesCardLayout.show(valuesPanel, "var1or2");
	scaleCardLayout.show(scalePanel, "scale");
	step.setHidden(! scale.getSelectedItem().equals("Linear"));
      }

      // VAR2: show scale=linear, show start, stop, step, and points.
      else if (function.getSelectedItem().equals("VAR2"))
      {
	valuesCardLayout.show(valuesPanel, "var1or2");
	scaleCardLayout.show(scalePanel, "linear");
	step.setHidden(false);
      }
    }
    else
    {
      functionCardLayout.show(functionPanel, "blank");
      valuesCardLayout.show(valuesPanel, "blank");
    }
  }



  /**
   * Exports values from this to the port.
   */
  protected abstract void exportValues();

  //
  // helper method for exportValues
  //

  protected SourceFunction exportFunction()
  {
    String function_str = (String) function.getSelectedItem();
    if (function_str.equals("CONS"))
      return new CONSFunction(value.getValue().toBigDecimal());
    else if (function_str.equals("VAR1"))
    {
      int n_scale;
      String scale_str = (String) scale.getSelectedItem();
      if (scale_str.equals("Linear"))
	n_scale = VAR1Function.LIN_SCALE;
      else if (scale_str.equals("Log10"))
	n_scale = VAR1Function.LOG10_SCALE;
      else if (scale_str.equals("Log25"))
	n_scale = VAR1Function.LOG25_SCALE;
      else if (scale_str.equals("Log50"))
	n_scale = VAR1Function.LOG50_SCALE;
      else
	throw new Error("illegal scale choice: " + scale_str);

      return new VAR1Function(n_scale,
			      start.getValue().toBigDecimal(),
			      stop.getValue().toBigDecimal(),
			      step.getValue().toBigDecimal());
    }
    else if (function_str.equals("VAR2"))
      return new VAR2Function(start.getValue().toBigDecimal(),
			      stop.getValue().toBigDecimal(),
			      step.getValue().toBigDecimal());
    else if (function_str.equals("VAR1P"))
      return new VAR1PFunction(ratio.getValue().toBigDecimal(),
			       offset.getValue().toBigDecimal());
    else
      throw new Error("illegal function choice: " + function_str);
  }



  /**
   * Imports values from the port to this.
   */
  public void revert()
  {
    // set all source-function-related values to defaults, so that any
    // values we don't write over will be in a reasonable state
    (new VAR1Function()).accept(importValuesVisitor);
    (new VAR2Function()).accept(importValuesVisitor);
    (new VAR1PFunction()).accept(importValuesVisitor);
    (new CONSFunction()).accept(importValuesVisitor);

    myPort.accept(importValuesVisitor);

    reconcile();
  }



  // helper Visitor: reads values from the object visited and imports
  // them into the SMUDialog.
  protected Visitor importValuesVisitor = new DefaultVisitor()
    {
      public final void visitSMU(SMU u)
      {
	vName.setText(u.getVName());
	vDownload.setSelected(u.getVDownload());
	iName.setText(u.getIName());
	iDownload.setSelected(u.getIDownload());
	switch (u.getMode())
	{
	case SMU.V_MODE:
	  mode.setSelectedItem("V");
	  u.getFunction().accept(this);
	  break;
	case SMU.I_MODE:
	  mode.setSelectedItem("I");
	  u.getFunction().accept(this);
	  break;
	case SMU.COMM_MODE:
	  mode.setSelectedItem("COMM");
	  break;
	default:
	  throw new Error("illegal mode: " + u.getMode());
	}
	iCompliance.setValue(u.getICompliance());
	vCompliance.setValue(u.getVCompliance());
      }
      
      public final void visitVSU(VSU u)
      {
	vName.setText(u.getVName());
	vDownload.setSelected(u.getVDownload());
	u.getFunction().accept(this);
      }

      public final void visitVMU(VMU u)
      {
	vName.setText(u.getVName());
	vDownload.setSelected(u.getVDownload());
	switch (u.getMode())
	{
	case VMU.V_MODE:
	  mode.setSelectedItem("V");
	  break;
	default:
	  throw new Error("illegal mode: " + u.getMode());
	}
      }

      public final void visitCONSFunction(CONSFunction f)
      {
	function.setSelectedItem("CONS");
	value.setValue(f.getValue());
      }
      
      public final void visitVAR1Function(VAR1Function f)
      {
	function.setSelectedItem("VAR1");
	switch(f.getScale())
	{
	case VAR1Function.LIN_SCALE:
	  scale.setSelectedItem("Linear");
	  step.setValue(f.getStep());
	  break;
	case VAR1Function.LOG10_SCALE:
	  scale.setSelectedItem("Log10");
	  break;
	case VAR1Function.LOG25_SCALE:
	  scale.setSelectedItem("Log25");
	  break;
	case VAR1Function.LOG50_SCALE:
	  scale.setSelectedItem("Log50");
	  break;
	default:
	  throw new Error("illegal scale: " + f.getScale());
	}
	start.setValue(f.getStart());
	stop.setValue(f.getStop());
	points.setText(Integer.toString(f.calculatePoints()));
      }

      public final void visitVAR2Function(VAR2Function f)
      {
	function.setSelectedItem("VAR2");
	start.setValue(f.getStart());
	stop.setValue(f.getStop());
	step.setValue(f.getStep());
	points.setText(Integer.toString(f.calculatePoints()));
      }

      public final void visitVAR1PFunction(VAR1PFunction f)
      {
	function.setSelectedItem("VAR1P");
	ratio.setValue(f.getRatio());
	offset.setValue(f.getOffset());
      }
    }; // end importValuesVisitor

} // end class PortDialog
