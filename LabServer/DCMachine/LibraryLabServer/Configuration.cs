using System;
using System.Xml;
using Library.Lab;
using Library.LabServerEngine;

namespace Library.LabServer
{
    public class Configuration : LabConfiguration
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "Configuration";

        //
        // String constants for the XML lab configuration
        //

        //
        // String constants for logfile messages
        //

        #endregion

        #region Properties

        //
        // YOUR CODE HERE
        //

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public Configuration()
            : this(null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public Configuration(string xmlLabConfiguration)
            : base(xmlLabConfiguration)
        {
            const string STRLOG_MethodName = "Configuration";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            try
            {
                //
                // Nothing to do here
                //
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                throw;
            }

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

    }
}
