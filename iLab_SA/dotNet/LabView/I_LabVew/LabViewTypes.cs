/*
 * Copyright (c) 2013 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id$ */

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web;

using System.Runtime.InteropServices;

namespace iLabs.LabView
{

  /// <summary>
  /// A class to define enumerated types to act as proxies for the
  /// types in the different versions of LabVIEW.
  /// </summary>
    public class LabViewTypes
    {
        public enum eExecState : int {
            eUndefined = -10, eNotInMemory = -1, eBad = 0, eIdle = 1, eRunningTopLevel = 2, eRunning = 3
        };

        public enum eFPState : int
        {
            eClosed, eHidden, eInvalid, eMaximized, eMinimized, eVisible
        }
    }
}
