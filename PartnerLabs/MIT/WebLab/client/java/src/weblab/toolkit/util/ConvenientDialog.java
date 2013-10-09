/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.util;

// JAVA 1.1 COMPLIANT

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

import java.util.StringTokenizer;

/**
 * A convenient way of creating quick dialog boxes for confirmation,
 * info, errors, etc.
 */
public class ConvenientDialog extends Dialog implements ActionListener
{
  private static Frame defaultParent;

  private TextField inputField;
  private boolean result;


  // if cancelText is null, shows only the OK button
  private ConvenientDialog(Frame parent,
			   String message,
			   String title,
			   String okText,
			   String cancelText,
			   boolean allowInput)
  {
    super(parent == null ?
	  (defaultParent == null ? new Frame() : defaultParent)
	  : parent, title, true);

    Panel buttonPanel = new Panel();

    Button okButton = new Button(okText);
    okButton.setActionCommand("ok");
    okButton.addActionListener(this);
    buttonPanel.add(okButton);

    if (cancelText != null)
    {
      Button cancelButton = new Button(cancelText);
      cancelButton.setActionCommand("cancel");
      cancelButton.addActionListener(this);
      buttonPanel.add(cancelButton);
    }

    inputField = new TextField(10);

    this.setLayout(new GridBagLayout());
    GridBagConstraints gbc = new GridBagConstraints();
    gbc.gridx = 0;
    gbc.anchor = GridBagConstraints.CENTER;
    gbc.insets = new Insets(5, 20, 0, 20); // top, left, bottom, right
    Insets loopInsets = new Insets(0, 20, 0, 20);
    StringTokenizer st = new StringTokenizer(message, "\n");
    while(st.hasMoreTokens())
    {
      this.add(new Label(st.nextToken()), gbc);
      gbc.insets = loopInsets;
    }

    gbc.fill = GridBagConstraints.HORIZONTAL;
    if (allowInput)
      this.add(inputField, gbc);

    gbc.insets = new Insets(5, 0, 0, 0);
    this.add(buttonPanel, gbc);

    this.pack();

    // center the dialog over its parent
    Component theParent = this.getParent();
    Point parentLocation = theParent.getLocation();
    Dimension parentSize = theParent.getSize();
    Dimension dialogSize = this.getSize();
    int x = parentLocation.x + (parentSize.width - dialogSize.width) / 2;
    int y = parentLocation.y + (parentSize.height - dialogSize.height) / 2;
    if (x < 0)
      x = 0;
    if (y < 0)
      y = 0;
    this.setLocation(x, y);
  }

  // handle button clicks
  public void actionPerformed(ActionEvent evt)
  {
    if (evt.getActionCommand().equals("ok"))
      this.result = true;
    else
      this.result = false;
    this.setVisible(false);
    this.dispose();
  }



  /**
   * Sets a "default parent" that will be used from now on whenever a
   * showDialog method is called with parent = null.
   */
  public static void setDefaultParent(Frame parent)
  {
    defaultParent = parent;
  }



  /**
   * Displays an exception dialog for the specified exception (this is
   * like a message dialog with title = name of exception, message =
   * exception message plus a note to check the Java Console output
   * for more details, and buttonText = OK).  Waits for the user to
   * click OK before returning.
   */
  public static void showExceptionDialog(Frame parent, Throwable t)
  {
    String title = t.getClass().getName();
    // clever trick: if lastIndexOf returns -1, then substring(0) will
    // leave the whole string intact
    title = title.substring(title.lastIndexOf('.') + 1);

    String message = t.getMessage() + "\n" +
      "See Java Console output for more details";

    JOptionPane.showMessageDialog(parent, message, title,
				  JOptionPane.ERROR_MESSAGE);
    /*XXX
    ConvenientDialog d = new ConvenientDialog
      (parent, message, title, "OK", null, false);
    d.show(); // blocks until dialog is closed by user
    */
  }



  /**
   * Displays a message dialog with the specified message and title,
   * and a button labelled with buttonText.  Waits for the user to
   * click on the button before returning.
   */
  public static void showMessageDialog(Frame parent,
				       String message,
				       String title,
				       String buttonText)
  {
    ConvenientDialog d = new ConvenientDialog
      (parent, message, title, buttonText, null, false);
    d.show(); // blocks until dialog is closed by user
  }



  /**
   * Displays a confirmation dialog with the specified message and
   * title, and two buttons: one labelled with okText and one labelled
   * with cancelText.  Waits for the user to click on one of the
   * buttons, and returns true iff the user chose the ok button.
   */
  public static boolean showConfirmDialog(Frame parent,
					  String message,
					  String title,
					  String okText,
					  String cancelText)
  {
    /*XXX
    ConvenientDialog d = new ConvenientDialog
      (parent, message, title, okText, cancelText, false);
    d.show(); // blocks until dialog is closed by user
    return d.result;
    */
    Object[] options = { okText, cancelText };

    return (JOptionPane.showOptionDialog
	    (parent, message, title, JOptionPane.DEFAULT_OPTION,
	     JOptionPane.QUESTION_MESSAGE, null, options, okText)
	    == 0);
  }



  /**
   * Displays a dialog with the specified message and title, OK and
   * Cancel buttons, and a text field for the user to input a string.
   * Waits for the user to click on one of the buttons, and returns
   * the contents of the input field (if the user chose the ok button)
   * or null (if the user chose the cancel button).
   */
  public static String showInputDialog(Frame parent,
				       String message,
				       String title)
  {
    ConvenientDialog d = new ConvenientDialog
      (parent, message, title, "OK", "Cancel", true);
    d.show(); // blocks until dialog is closed by user
    if (d.result)
      return d.inputField.getText();
    else
      return null;
  }

} // end class ConvenientDialog
