package weblab.client.graphicalUI;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import javax.swing.plaf.ToolTipUI;
import javax.swing.plaf.ComponentUI;

import java.util.Collections;
import java.util.List;
import java.util.ArrayList;
import java.util.Iterator;

/**
 * MultiLineToolTipUI is a replacement for ToolTipUI that can display
 * tooltips with multiple lines (lines should be separated by "\n").
 *
 * Just run the initialize method, and it will automatically be used
 * by ALL tooltips in the GUI.
 */
public class MultiLineToolTipUI extends ToolTipUI
{

  static MultiLineToolTipUI SINGLETON = new MultiLineToolTipUI();

  int inset = 3;

  // Ensure that SINGLETON will be the only instance of this class by
  // making the constructor private.
  private MultiLineToolTipUI()
  {}

  /**
   * Installs MultiLineToolTipUI into the UIManager so that it will be
   * used to render all tooltips.
   */
  public static void initialize()
  {
    Class cls = SINGLETON.getClass();
    UIManager.put("ToolTipUI", "MultiLineToolTipUI");
    UIManager.put("MultiLineToolTipUI", cls);
  }

  public static ComponentUI createUI(JComponent c)
  {
    return SINGLETON;
  }

  public void installUI(JComponent c)
  {
    LookAndFeel.installColorsAndFont
      (c, "ToolTip.background", "ToolTip.foreground", "ToolTip.font");
    LookAndFeel.installBorder(c, "ToolTip.border");
  }

  public void uninstallUI(JComponent c)
  {
    LookAndFeel.uninstallBorder(c);
  }

  public Dimension getPreferredSize(JComponent c)
  {
    Font font = c.getFont();
    FontMetrics fontMetrics = c.getFontMetrics(font);

    String tipText = ((JToolTip)c).getTipText();

    if (tipText == null)
      tipText = "";

    List lines = breakLines(tipText);

    // calculate total height of all lines
    int height = lines.size() * fontMetrics.getHeight();

    // calculate max width of any one line
    int width = 0;
    for (Iterator i = lines.iterator(); i.hasNext();) {
      width = Math.max(width, fontMetrics.stringWidth((String)i.next()));
    }

    return new Dimension(width + inset * 2, height + inset * 2);
  }

  public Dimension getMinimumSize(JComponent c)
  {
    return this.getPreferredSize(c);
  }

  public Dimension getMaximumSize(JComponent c)
  {
    return this.getPreferredSize(c);
  }

  public void paint(Graphics g, JComponent c)
  {
    Font font = c.getFont();
    FontMetrics fontMetrics = c.getFontMetrics(font);

    int fontHeight = fontMetrics.getHeight();

    String tipText = ((JToolTip)c).getTipText();

    List lines = breakLines(tipText);

    Dimension dimension = c.getSize();
    g.setColor(c.getBackground());
    g.fillRect(0, 0, dimension.width, dimension.height);

    g.setColor(c.getForeground());

    int height = 2 + fontMetrics.getAscent();
    for (Iterator i = lines.iterator(); i.hasNext();) {
      g.drawString((String)i.next(), inset, height);
      height += fontHeight;
    }
  }

  // returns List of String
  public static List breakLines(String text)
  {
    if (text == null) {
      return Collections.EMPTY_LIST;
    }

    int len = text.length();

    List lines = new ArrayList();

    int start = 0;
    int i = 0;
    while (i < len) {
      if (text.charAt(i) == '\n') {
	lines.add(text.substring(start, i));
	start = i + 1;
	i = start;
      }
      else {
	i++;
      }
    }
    lines.add(text.substring(start));

    return lines;
  }

} // end class MultiLineToolTipUI
