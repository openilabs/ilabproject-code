package weblab.client.graphicalUI;

/**
 * A file filter that accepts .csv files.
 */
class CSVFileFilter extends javax.swing.filechooser.FileFilter {

  /**
   * Accepts a file if it is a directory or if it ends with .csv
   */
  public boolean accept(java.io.File f)
  {
    if (f == null) {
      return false;
    }
    if (f.isDirectory()) {
      return true;
    }
    if (f.getName().endsWith(".csv")) {
      return true;
    }
    return false;
  }

  /**
   * Returns a description for this filter
   */
  public String getDescription()
  {
    return "Comma-Separated Value Files (*.csv)";
  }
}
