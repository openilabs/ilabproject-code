/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.client.graphicalUI;

import java.applet.AppletContext;
import javax.swing.*;

import weblab.toolkit.util.MultiLineToolTipUI;
import weblab.toolkit.util.ConvenientDialog;
import weblab.toolkit.serverInterface.Server;
import weblab.toolkit.serverInterface.SBServer;

import weblab.client.WeblabClient;

/**
 * GraphicalApplet launches the weblab graphical UI by creating a
 * MainFrame.
 */
public class GraphicalApplet extends JApplet implements Runnable
{
  // Init method should always return successfully and immediately, so
  // that LoadingApplet will know to go away.  The real work is
  // delegated to a background thread.
  public void init()
  {
    try
    {
      System.out.println("GraphicalApplet started.\n");
      (new Thread(this)).start();
    }
    catch (Throwable t)
    {
      try
      {
	t.printStackTrace();
	ConvenientDialog.showExceptionDialog(null, t);
      }
      catch (Throwable t2)
      {
	// Got another throwable while trying to tell the user about
	// the first one.  Nothing we can do here except avoid doing
	// anything that might raise a third one.
      }
    }
  }



  // Since we're initializing the client in a background thread, we
  // can't count on the JRE to display Errors and RuntimeExceptions
  // for us (even in pathological cases like MainFrame.class being
  // missing from the JAR).  Catch anything we possibly can and try to
  // let the user know what went wrong.
  public void run()
  {
    try
    {
      initHelper();
    }
    catch (Throwable t)
    {
      t.printStackTrace();
      ConvenientDialog.showExceptionDialog(null, t);
    }
  }



  // This method does the real work of initializing the client, which
  // has been separated out to make the try/catch structure of the
  // run() method clearer.
  public void initHelper()
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

    Server theServer = this.getServer();

    //look for coupon information passed from Service Broker
    String strCouponID = getParameter("couponID");
    //System.out.println("Coupon ID: " + strCouponID + "\n");
   
    long couponID = -1L;
    if (strCouponID != null && !strCouponID.equals("") && !strCouponID.equals("${op:couponId}")) couponID = Long.parseLong(strCouponID);

    String couponPassKey = getParameter("couponPassKey");
     //System.out.println("Coupon Passkey: " + couponPassKey + "\n");
    if (couponID > 0) ((SBServer) theServer).addCouponInformation(couponID, couponPassKey);
    
    // try to extract experimentSpecificationDTD from (optional) param tag
    String dtdURL = getParameter("experimentSpecificationDTD");
    if (dtdURL == null)
      dtdURL = "http://weblab2.mit.edu/xml/experimentSpecification.dtd";
    weblab.client.ExperimentSpecification.setDTD(dtdURL);

    // try to extract helpURL from (optional) param tag
    String helpURL = getParameter("helpURL");
    if (helpURL == null)
      helpURL = "http://weblab.mit.edu";

    // create and show MainFrame
    MainFrame theMainFrame = new MainFrame
      (new WeblabClient(theServer), getAppletContext(), helpURL);
    theMainFrame.setVisible(true);

    // bring MainFrame to front, and center it on screen
    theMainFrame.toFront();
    theMainFrame.setLocationRelativeTo(null);

    System.out.println("GraphicalApplet initialization complete.\n");
  }



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

} // end class GraphicalApplet
