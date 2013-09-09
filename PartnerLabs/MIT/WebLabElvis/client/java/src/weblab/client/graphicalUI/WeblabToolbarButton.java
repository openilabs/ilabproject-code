package weblab.client.graphicalUI;

import javax.swing.*;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;


/**
 * WeblabToolbarButton differs from JButton in the way it acquires
 * properties from an Action.  Text is not set unless icon is null (so
 * the button displays only an icon, rather than an icon and text),
 * and tooltip text is set to the action name.
 */
public class WeblabToolbarButton extends JButton
{
  public WeblabToolbarButton(Action a)
  {
    super(a);
  }

  protected void configurePropertiesFromAction(Action a)
  {
    // configure properties according to JButton's rules
    super.configurePropertiesFromAction(a);

    // if this button has an icon...
    if (getIcon() != null) {
      // set tooltip to the name of the button (which
      // JButton.configurePropertiesFromAction puts in the Text
      // field), unless tooltip is already set
      if (getToolTipText() == null) {
	setToolTipText(getText());
      }
      // clear the Text field
      setText(null);
    }
  }

  protected PropertyChangeListener
    createActionPropertyChangeListener(Action a)
  {
    // the easy way out: return a PropertyChangeListener that just
    // calls configurePropertiesFromAction on a

    final Action myAction = a;

    return new PropertyChangeListener() {
	public void propertyChange(PropertyChangeEvent evt) {
	  configurePropertiesFromAction(myAction);
	}
      };
  }

} // end class WeblabToolbarButton
