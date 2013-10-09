/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.util;

import javax.swing.*;
import javax.swing.plaf.basic.BasicComboPopup;
import java.awt.*;

/**
 * CompactComboBox is a customized JComboBox that allows the user to
 * select between objects with long string descriptions, even if the
 * CompactComboBox itself is not wide enough to display the
 * descriptions.
 *
 * The important difference is that CompactComboBox's popup window is
 * allowed to exceed the width of the main component.
 *
 * Note: by default, CompactComboBox still calculates its preferred
 * dimensions based on its contents just like a JComboBox (so it will
 * try to be very wide if it contains objects with long string
 * descriptions).  Calling setPrototypeDisplayValue is a convenient
 * way to override this behavior.
 */
public class CompactComboBox extends JComboBox
{
  private boolean layoutDone = false;


  /**
   * Constructs a new CompactComboBox.
   */
  public CompactComboBox()
  {
    super();
  }



  // overrides JComboBox.firePopupMenuWillBecomeVisible()
  public void firePopupMenuWillBecomeVisible()
  {
    super.firePopupMenuWillBecomeVisible();

    //
    // resize the popup window based on the size of its contents
    //

    try
    {
      BasicComboPopup popup = (BasicComboPopup)
	this.getUI().getAccessibleChild(this, 0);

      JScrollPane js = (JScrollPane) popup.getComponent(0);

      if (! layoutDone)
      {
	// one-time setup: reinstall the JScrollPane in the popup
	// using BorderLayout instead of the default BoxLayout, so
	// that it will take up the entire size of the popup
	popup.setLayout(new BorderLayout());
	popup.add(js, BorderLayout.CENTER);

	layoutDone = true;
      }

      // calculate actual width needed to display contents, plus
      // vertical scrollbar, plus an extra 2 pixels by which
      // js.preferredSize() and popup.preferredSize() normally differ
      // for some reason
      int actualWidth = popup.getList().getPreferredSize().width
	+ js.getVerticalScrollBar().getPreferredSize().width
	+ 2;

      Dimension d = new Dimension(popup.getMinimumSize());

      // substitute actual width if it's larger than the default width
      // (but not if it's smaller)
      if (actualWidth > d.width)
	d.width = actualWidth;

      popup.setPreferredSize(d);
    }
    catch (RuntimeException ex)
    {
      // If the assumptions on which the above voodoo is based don't
      // turn to be accurate, it may generate NullPointerExceptions,
      // ClassCastExceptions, etc.  Print a warning and a stack trace
      // to the console if this happens, but don't let it interfere
      // with the normal operation of the JComboBox.
      System.out.println("WARNING: CompactComboBox.resizePopup() failed");
      ex.printStackTrace();
    }
  }

} // end class CompactComboBox
