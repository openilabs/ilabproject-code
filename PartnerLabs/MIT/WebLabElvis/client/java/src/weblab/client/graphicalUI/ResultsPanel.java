package weblab.client.graphicalUI;

import weblab.client.*;

import weblab.graphing.Graph;
import weblab.graphing.Grid;
import weblab.graphing.ConnectPattern;

import weblab.util.ChangeTrackingObservable;
import weblab.util.EngMath;
import weblab.util.EngValue;
import weblab.util.EngUnits;
import weblab.util.EngValueField;

import java.awt.*;
import javax.swing.*;
import java.awt.event.*;

import java.util.Observable;
import java.util.Observer;


public class ResultsPanel extends JPanel implements Observer
{
  private ResultsPanel self;

  // number of sigfigs to display (for temperature)
  private static final int NUM_SIGFIGS = 4;

  protected Frame theMainFrame;
  protected WeblabClient theWeblabClient;
  private final Grid theGrid;

  private boolean y1Gridlines;

  private WeblabAxis xAxis, y1Axis, y2Axis;

  protected Graph Y1vsX, Y2vsX;

  protected JPanel graphPanel;

  private final int GRAPH_WIDTH = 360;
  private final int GRAPH_HEIGHT = 270;

  // snap tracking will only activate if mouse is within this number
  // of pixels of the data point
  private final double SNAP_TRACKING_THRESHOLD = 20;

  private JLabel temp;

  private JDialog trackingDialog;
  private JButton trackingButton;
  private JCheckBox snapTracking;
  private JLabel xTracking, y1Tracking, y2Tracking;
  private boolean trackingInit = false; // whether we've opened the
					// tracking dialog yet


  //////////////////
  // Constructors //
  //////////////////



  public ResultsPanel(Frame theMainFrame, WeblabClient wc)
  {
    self = this;

    this.theMainFrame = theMainFrame;
    this.theWeblabClient = wc;

    xAxis = wc.getXAxis();
    y1Axis = wc.getY1Axis();
    y2Axis = wc.getY2Axis();

    this.theGrid = new Grid();

    Y1vsX = new Graph(y1Axis, xAxis);
    Y1vsX.setColor(Color.blue);
    Y2vsX = new Graph(y2Axis, xAxis);
    Y2vsX.setColor(Color.red);

    // grid follows Y1 by default
    y1Gridlines = true;

    //
    // setup components
    //

    temp = new JLabel();

    graphPanel = new JPanel() {
	public void paintComponent(Graphics g)
	{
	  // paint background
	  super.paintComponent(g);

	  // paint grid
	  theGrid.paint(this, g);

	  // plot graphs
	  Y2vsX.plot(this, g);
	  Y1vsX.plot(this, g);
	}

      };
    graphPanel.setMinimumSize(new Dimension(GRAPH_WIDTH, GRAPH_HEIGHT));
    graphPanel.setPreferredSize(new Dimension(GRAPH_WIDTH, GRAPH_HEIGHT));
    graphPanel.setBackground(Color.white);

    final AxisPanel xAxisPanel = new AxisPanel
      (xAxis, true, Color.black, theGrid);
    final AxisPanel y1AxisPanel = new AxisPanel
      (y1Axis, false, Color.blue, theGrid);
    final AxisPanel y2AxisPanel = new AxisPanel
      (y2Axis, false, Color.red, theGrid);

    trackingButton = new JButton("Tracking");
    trackingButton.addActionListener(new ActionListener () {
	public void actionPerformed(ActionEvent evt) {
	  if (! trackingInit)
	  {
	    trackingInit = true;

	    // set default position for tracking dialog: bottom-left
	    // corner aligned with top-left corner of ResultsPanel
	    Point parentLocation = self.getLocationOnScreen();
	    Dimension dialogSize = trackingDialog.getSize();
	    int x = parentLocation.x;
	    int y = parentLocation.y - dialogSize.height;
	    trackingDialog.setLocation(x, y);
	  }

	  trackingButton.setEnabled(false);
	  trackingDialog.setVisible(true);
	}
      });

    trackingDialog = new JDialog(theMainFrame, "Tracking", false);

    snapTracking = new JCheckBox("Snap to data points", true);

    xTracking = new JLabel("---");
    xTracking.setForeground(Color.black);
    //xTracking.setAlignment(Label.LEFT);
    y1Tracking = new JLabel("---");
    y1Tracking.setForeground(Color.blue);
    //y1Tracking.setAlignment(Label.LEFT);
    y2Tracking = new JLabel("---");
    y2Tracking.setForeground(Color.red);
    //y2Tracking.setAlignment(Label.LEFT);

    JButton closeButton = new JButton("Close");
    closeButton.addActionListener(new ActionListener () {
	public void actionPerformed(ActionEvent evt) {
	  trackingButton.setEnabled(true);
	  trackingDialog.setVisible(false);
	}
      });


    //
    // setup listeners
    //

    xAxis.addObserver(this);
    y1Axis.addObserver(this);
    y2Axis.addObserver(this);

    Y1vsX.addObserver(this);
    Y2vsX.addObserver(this);

    theGrid.addObserver(this);
    theWeblabClient.addObserver(this);

    // When mouse moves within the graph panel, update point
    // highlighting for all graphs and update tracking for all axes.
    graphPanel.addMouseMotionListener(new MouseMotionListener () {
	public void mouseMoved(MouseEvent evt)
	{
	  // mouse position
	  Point mouseP = evt.getPoint();

	  // new point to highlight (or -1 for none)
	  int newN = -1;

	  if (trackingDialog.isVisible() && snapTracking.isSelected())
	  {
	    //
	    // If tracking dialog box is open and snap tracking is enabled
	    // by user, set newN to the index of the nearest data point to
	    // mouseP (but only if the distance is less than
	    // SNAP_TRACKING_THRESHOLD).
	    //
	    double distance = SNAP_TRACKING_THRESHOLD;

	    int nextN;
	    double nextDistance;

	    nextN = Y1vsX.findNearestPoint(graphPanel, mouseP);
	    if (nextN != -1)
	    {
	      nextDistance = Graph.distanceBetweenPoints
		(Y1vsX.getPoint(graphPanel, nextN), mouseP);
	      if (nextDistance < distance)
	      {
		newN = nextN;
		distance = nextDistance;
	      }
	    }

	    nextN = Y2vsX.findNearestPoint(graphPanel, mouseP);
	    if (nextN != -1)
	    {
	      nextDistance = Graph.distanceBetweenPoints
		(Y2vsX.getPoint(graphPanel, nextN), mouseP);
	      if (nextDistance < distance)
	      {
		newN = nextN;
		distance = nextDistance;
	      }
	    }
	  }

	  // update highlighting

	  Y1vsX.setHighlightedPoint(newN);
	  Y1vsX.notifyObservers();

	  Y2vsX.setHighlightedPoint(newN);
	  Y2vsX.notifyObservers();

	  // update tracking

	  if (newN == -1)
	  {
	    // Continuous tracking: set tracking fields to the values of the
	    // current mouse location.

	    xTracking.setText(xAxisPanel.trackMouseLocation(graphPanel, mouseP));
	    y1Tracking.setText(y1AxisPanel.trackMouseLocation(graphPanel, mouseP));
	    y2Tracking.setText(y2AxisPanel.trackMouseLocation(graphPanel, mouseP));
	  }
	  else
	  {
	    // Snap tracking: set tracking fields to the values of point
	    // newN.

	    xTracking.setText(xAxisPanel.trackDataPoint(newN));
	    y1Tracking.setText(y1AxisPanel.trackDataPoint(newN));
	    y2Tracking.setText(y2AxisPanel.trackDataPoint(newN));
	  }
	}

	// obligatory irrelevant methods of MouseMotionListener
	public void mouseDragged(MouseEvent evt) {}
      });

    // when mouse pointer leaves the graph panel, clear highlighting
    // for all graphs and clear tracking for all axes
    graphPanel.addMouseListener(new MouseAdapter () {
	public void mouseExited(MouseEvent evt)
	{
	  Y1vsX.setHighlightedPoint(-1);
	  Y1vsX.notifyObservers();

	  Y2vsX.setHighlightedPoint(-1);
	  Y2vsX.notifyObservers();

	  xTracking.setText("---");
	  y1Tracking.setText("---");
	  y2Tracking.setText("---");
	}
      });

    //
    // setup layout
    //

    GridBagConstraints gbc = new GridBagConstraints();
    gbc.fill = GridBagConstraints.NONE;

    JPanel trackingPanel = new JPanel(new GridBagLayout());
    gbc.gridx = 0;
    gbc.gridwidth = 2;
    trackingPanel.add(snapTracking, gbc);
    gbc.gridwidth = 1;
    trackingPanel.add(new Label("X:"), gbc);
    trackingPanel.add(new Label("Y1:"), gbc);
    trackingPanel.add(new Label("Y2:"), gbc);
    gbc.gridx = 1;
    gbc.weightx = 1;
    gbc.fill = GridBagConstraints.HORIZONTAL;
    trackingPanel.add(xTracking, gbc);
    trackingPanel.add(y1Tracking, gbc);
    trackingPanel.add(y2Tracking, gbc);

    Container trackingDialogCP = trackingDialog.getContentPane();
    trackingDialogCP.setLayout(new BorderLayout());
    trackingDialogCP.add(trackingPanel, BorderLayout.CENTER);
    trackingDialogCP.add(closeButton, BorderLayout.SOUTH);

    trackingDialog.pack();
    

    JPanel temperaturePanel = new JPanel(new GridBagLayout());
    temperaturePanel.add(new JLabel("Temperature: "));
    temperaturePanel.add(temp);
    // ensure that this is at least as tall as an EngDisplay
    temperaturePanel.add
      (Box.createVerticalStrut
       ((int) (new EngValueField()).getPreferredSize().getHeight()));

    this.setLayout(new GridBagLayout());

    gbc = new GridBagConstraints();
    gbc.fill = GridBagConstraints.BOTH;

    gbc.gridx = 1;
    gbc.gridy = 0;
    gbc.weightx = 1;
    gbc.weighty = 0;
    this.add(temperaturePanel, gbc);

    gbc.gridx = 1;
    gbc.gridy = 2;
    gbc.weightx = 1;
    gbc.weighty = 0;
    this.add(xAxisPanel, gbc);

    gbc.gridx = 0;
    gbc.gridy = 1;
    gbc.weightx = 0;
    gbc.weighty = 1;
    this.add(y1AxisPanel, gbc);

    gbc.gridx = 2;
    gbc.gridy = 1;
    gbc.weightx = 0;
    gbc.weighty = 1;
    this.add(y2AxisPanel, gbc);

    gbc.gridx = 1;
    gbc.gridy = 1;
    gbc.weightx = 1;
    gbc.weighty = 1;
    this.add(graphPanel, gbc);

    gbc.gridx = 0;
    gbc.gridy = 2;
    gbc.weightx = 0;
    gbc.weighty = 0;
    gbc.fill = GridBagConstraints.NONE;
    this.add(trackingButton, gbc);

    //
    // initialize
    //

    updateTemperature();
    updateConnectPatterns();
  }



  /**
   * true -> draw Y axis gridlines based on y1 axis
   * false -> draw Y axis gridlines based on y2 axis
   *
   * Immediately redraws the grid if necessary.
   */
  public void setY1Gridlines(boolean newState)
  {
    this.y1Gridlines = newState;
    updateGrid();
  }



  //////////////////////
  // Listener methods //
  //////////////////////


  /**
   * Handle updates from Observables
   */
  public final void update(Observable o, Object arg)
  {
    // When the data from an axis changes, update the number of
    // divisions on the grid and repaint the graphPanel.
    if (o == xAxis || o == y1Axis || o == y2Axis)
    {
      updateGrid();
      graphPanel.repaint();
    }

    // When a graph changes, repaint graphPanel as necessary
    if (o == Y1vsX || o == Y2vsX)
    {
      String[] changes = ChangeTrackingObservable.extractChanges(arg);
      for (int i = 0; i < changes.length; i++)
      {
	//
	// For highlighting changes, we can get away with repainting
	// only one or two small regions of the graph canvas
	//
	if (changes[i].startsWith(Graph.HIGHLIGHT_CHANGE_PREFIX))
	{
	  Point p;

	  // the rest of the change string indicates the number of the
	  // point that is no longer highlighted (or -1 for none)
	  int oldN = Integer.parseInt
	    (changes[i].substring(Graph.HIGHLIGHT_CHANGE_PREFIX.length(),
				  changes[i].length()));

	  if (oldN != -1)
	  {
	    // repaint old point location for each graph
	    try {
	      p = Y1vsX.getPoint(graphPanel, oldN);
	      graphPanel.repaint(p.x - Graph.HIGHLIGHT_RADIUS,
				 p.y - Graph.HIGHLIGHT_RADIUS,
				 2 * Graph.HIGHLIGHT_RADIUS + 1,
				 2 * Graph.HIGHLIGHT_RADIUS + 1);
	    }
	    catch (IndexOutOfBoundsException ex) {
	      // ignore (there's no such point on this graph to
	      // worry about)
	    }

	    try {
	      p = Y2vsX.getPoint(graphPanel, oldN);
	      graphPanel.repaint(p.x - Graph.HIGHLIGHT_RADIUS,
				 p.y - Graph.HIGHLIGHT_RADIUS,
				 2 * Graph.HIGHLIGHT_RADIUS + 1,
				 2 * Graph.HIGHLIGHT_RADIUS + 1);
	    }
	    catch (IndexOutOfBoundsException ex) {
	      // ignore (there's no such point on this graph to
	      // worry about)
	    }
	  }

	  // the new highlighted point (or -1 for none)
	  int newN = Y1vsX.getHighlightedPoint();

	  if (newN != -1)
	  {
	    // repaint new point location for each graph
	    try {
	      p = Y1vsX.getPoint(graphPanel, newN);
	      graphPanel.repaint(p.x - Graph.HIGHLIGHT_RADIUS,
				 p.y - Graph.HIGHLIGHT_RADIUS,
				 2 * Graph.HIGHLIGHT_RADIUS + 1,
				 2 * Graph.HIGHLIGHT_RADIUS + 1);
	    }
	    catch (IndexOutOfBoundsException ex) {
	      // ignore (there's no such point on this graph to
	      // worry about)
	    }

	    try {
	      p = Y2vsX.getPoint(graphPanel, newN);
	      graphPanel.repaint(p.x - Graph.HIGHLIGHT_RADIUS,
				 p.y - Graph.HIGHLIGHT_RADIUS,
				 2 * Graph.HIGHLIGHT_RADIUS + 1,
				 2 * Graph.HIGHLIGHT_RADIUS + 1);
	    }
	    catch (IndexOutOfBoundsException ex) {
	      // ignore (there's no such point on this graph to
	      // worry about)
	    }
	  }
	}
	else
	{
	  //
	  // For changes other than highlight changes, we must repaint
	  // the entire graphPanel.
	  //
	  graphPanel.repaint();
	}
      }
    }

    // When the grid changes, repaint graphPanel
    if (o == theGrid)
      graphPanel.repaint();

    // When results change, update temperature, update connect
    // patterns, and repaint graphPanel.
    if (o == theWeblabClient && ChangeTrackingObservable.containsChange
	(arg, WeblabClient.EXPERIMENT_RESULT_CHANGE))
    {
      updateTemperature();
      updateConnectPatterns();
      graphPanel.repaint();
    }
  }



  ////////////////////
  // Helper Methods //
  ////////////////////



  private void updateTemperature()
  {
    Temperature t =
      theWeblabClient.getExperimentResult().getTemperature();

    if (t == null) {
      temp.setText("unknown");
    }
    else {
      // HACK: trim() below is a workaround for Lab Server reporting
      // units as "K\n" rather than "K"
      temp.setText
	((new EngValue(t.getValue(), new EngUnits(t.getUnits().trim()), NUM_SIGFIGS))
	 .toString());
    }
  }



  private final void updateConnectPatterns()
  {
    ExperimentSpecificationVisitor ev = new ExperimentSpecificationVisitor();
    theWeblabClient.getExperimentSpecification().accept(ev);

    if (ev.var1Function != null)
    {
      // number of points in the VAR1 loop
      final int var1points = ev.var1Function.calculatePoints();

      // Connect all pairs of successive points EXCEPT: every
      // (var1points) points, don't connect the next point.
      ConnectPattern cp = new ConnectPattern() {
	  public final boolean isConnected(int n)
	  {
	    return (n % var1points != 0);
	  }
	};
      Y1vsX.setConnectPattern(cp);
      Y2vsX.setConnectPattern(cp);
    }
    else
    {
      Y1vsX.setConnectPattern(Graph.ALWAYS_CONNECT);
      Y2vsX.setConnectPattern(Graph.ALWAYS_CONNECT);
    }
  }



  private final void updateGrid()
  {
    theGrid.setXDivs(xAxis.getPreferredDivs());
    if (this.y1Gridlines)
      theGrid.setYDivs(y1Axis.getPreferredDivs());
    else
      theGrid.setYDivs(y2Axis.getPreferredDivs());

    theGrid.notifyObservers();
  }

} // end class ResultsPanel
