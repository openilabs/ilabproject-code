package weblab.util;

// JAVA 1.1 COMPLIANT

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

  private boolean result;


  // if cancelText is null, shows only the OK button
  private ConvenientDialog(Frame parent,
			   String message,
			   String title,
			   String okText,
			   String cancelText)
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
  public static void showExceptionDialog(Frame parent, Exception ex)
  {
    String title = ex.getClass().getName();
    // clever trick: if lastIndexOf returns -1, then substring(0) will
    // leave the whole string intact
    title = title.substring(title.lastIndexOf('.') + 1);

    String message = ex.getMessage() + "\n" +
      "See Java Console output for more details";

    ConvenientDialog d = new ConvenientDialog
      (parent, message, title, "OK", null);
    d.show(); // blocks until dialog is closed by user
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
      (parent, message, title, buttonText, null);
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
    ConvenientDialog d = new ConvenientDialog
      (parent, message, title, okText, cancelText);
    d.show(); // blocks until dialog is closed by user
    return d.result;
  }

} // end class ConvenientDialog
