using System;
using System.Collections.Generic;
using System.Text;

namespace iLabs.Core
{
    class iLabGlobal
    {
        static private string date = "$Date$";
        static private string revision = "$Revision$";
        static private string release = "2.1";
        static string Revision
        {
            get
            {
                return date + " -- " + revision;
            }
        }

        static string Release
        {
            get
            {
                return release;
            }
        }
    }
}
