package weblab.client.graphicalUI;

import weblab.client.WeblabClient;
import weblab.client.serverInterface.Server;
import weblab.client.serverInterface.SBServer;

import java.applet.AppletContext;
import javax.swing.*;

/**
 * GraphicalApplet launches the weblab graphical UI by creating a
 * MainFrame.
 */
public class GraphicalApplet extends JApplet
{
  protected Server getServer()
  {
    // determine serviceURL (.asmx page location) from param tag
    String serviceURL = getParameter("serviceURL");
    if (serviceURL == null)
      throw new Error
	("GraphicalApplet must be invoked with a serviceURL parameter");

    // determine labServerID from param tag
    String labServerID = getParameter("labServerID");
    if (labServerID == null)
      throw new Error
	("GraphicalApplet must be invoked with a labServerID parameter");

    return new SBServer(serviceURL, labServerID);
  }

  public void init()
  {
    // set look and feel based on client system (i.e. Windows look and
    // feel on Windows machines, Mac on Mac systems, CDE/Motif on UNIX
    // systems...)
    try {
      UIManager.setLookAndFeel
	(UIManager.getSystemLookAndFeelClassName());
    }
    catch (Exception e) {
      // oops, not much we can do about this.  Print a stack trace and
      // proceed (using the Java look and feel).
      e.printStackTrace();
    }

    // initialize multi-line tooltips
    MultiLineToolTipUI.initialize();

    showStatus("Initializing Weblab client.  Please wait...");

    Server theServer = this.getServer();

     //look for coupon information passed from Service Broker
    String strCouponID = getParameter("couponID");
    //System.out.println("Coupon ID: " + strCouponID + "\n");
   
    long couponID = -1L;
    if (strCouponID != null && !strCouponID.equals("") && !strCouponID.equals("${op:couponId}")) couponID = Long.parseLong(strCouponID);

    String couponPassKey = getParameter("couponPassKey");
     //System.out.println("Coupon Passkey: " + couponPassKey + "\n");
    if (couponID > 0) ((SBServer) theServer).addCouponInformation(couponID, couponPassKey);
    
    // try to extract expSpecDTD from (optional) param tag
    String dtdURL = getParameter("expSpecDTD");
    if (dtdURL != null)
      weblab.client.ExperimentSpecification.dtd_url = dtdURL;

    // try to extract helpURL from (optional) param tag
    String helpURL = getParameter("helpURL");
    if (helpURL == null)
      helpURL = "http://weblab.mit.edu";

    MainFrame theMainFrame = new MainFrame
      (new WeblabClient(theServer), getAppletContext(), helpURL);
    theMainFrame.setVisible(true);

    showStatus("");
  }

} // end class GraphicalApplet
