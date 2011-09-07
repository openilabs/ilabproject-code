/* Copyright (c) 2008 The Massachusetts Institute of Technology. All rights reserved. */
/* $Id: iLabGlobal.cs 443 2011-08-29 21:35:54Z phbailey $ */

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
        static private string headUrl = "$HeadURL: https://ilabproject.svn.sourceforge.net/svnroot/ilabproject/trunk/iLab_SA/dotNet/Libraries/UtilLibrary/iLabGlobal.cs $";
        static private string info = "$Id: iLabGlobal.cs 443 2011-08-29 21:35:54Z phbailey $";
        static private string date = "$Date: 2011-08-29 17:35:54 -0400 (Mon, 29 Aug 2011) $";
        static private string revision = "$Revision: 443 $";
        static private string iLabRelease = "$ilab:Release$";
	static private string release = "Release 3.5.1";
        static private string buildDate = "$ilab:BuildDate$";
        /// <summary>
        /// Returns the date and svn revision last set, still not auto setting..
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
        /// returns the build date of the release.
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
