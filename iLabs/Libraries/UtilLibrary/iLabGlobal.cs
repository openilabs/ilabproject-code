using System;
using System.Collections.Generic;
using System.Text;

namespace iLabs.UtilLib
{
    /// <summary>
    /// static class to provide release tags.
    /// </summary>
    public class iLabGlobal
    {
        static private string date = "$Date$";
        static private string revision = "$Revision$";
        //static private string release = "$ilab:Release$";
	static private string release = "Release 3.0.4 RC3 after merge with trunk";
        static private string buildDate = "$ilab:BuildDate$";
        /// <summary>
        /// Returns the date and svn revision last set, still not auto setting.
        /// </summary>
        public static string Revision
        {
            get
            {
                return revision + " " + date;
            }
        }
        /// <summary>
        /// returns a release string specified in iLabGlobal
        /// </summary>
        public static string Release
        {
            get
            {
                return release + " ( " + revision.Replace("$", "") + " ) " + date.Replace("$", "");
            }
        }

        /// <summary>
        /// returns the build date of the release, this currently is not set automaticl on each commit.
        /// </summary>
        public static string BuildDate
        {
            get
            {
                return buildDate;
            }
        }
    }
}
