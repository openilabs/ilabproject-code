using System;
using Library.Lab;
using Library.LabServerEngine;
using Library.LabServer.Drivers.Setup;

namespace Library.LabServer
{
    public class Specification : ExperimentSpecification
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "Specification";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_accepted = " accepted: ";
        private const string STRLOG_executionTime = " executionTime: ";
        private const string STRLOG_seconds = " seconds";

        //
        // Local variables
        //
        private Configuration configuration;

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public Specification(Configuration configuration)
            : base(configuration)
        {
            const string STRLOG_MethodName = "Specification";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            // Save these for use by the Parse() method
            this.configuration = configuration;

            //
            // Check that the specification template is valid. This is used by the LabClient to submit
            // the experiment specification to the LabServer for execution.
            //
            try
            {
                //
                // Check that all required XML nodes exist
                //
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

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Parse the XML specification string to check its validity. No exceptions are thrown back to the
        /// calling method. If an error occurs, 'accepted' is set to false and the error message is placed
        /// in 'errorMessage' where it can be examined by the calling method.
        /// </summary>
        /// <param name="xmlSpecification"></param>
        public override ValidationReport Parse(string xmlSpecification)
        {
            const string STRLOG_MethodName = "Parse";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Catch all exceptions and log errors, don't throw back to caller
            //
            ValidationReport validationReport = null;
            try
            {
                //
                // Call the base class to parse its part
                //
                validationReport = base.Parse(xmlSpecification);
                if (validationReport.accepted == false)
                {
                    throw new Exception(validationReport.errorMessage);
                }

                // Create new validation report
                validationReport = new ValidationReport();

                //
                // Create an instance of the driver for the specified setup and get specific values
                //
                DriverGeneric driver = null;
                if (this.SetupId.Equals(Consts.STRXML_SetupId_LockedRotor))
                {
                    driver = new DriverLockedRotor(0);
                }
                else if (this.SetupId.Equals(Consts.STRXML_SetupId_NoLoad))
                {
                    driver = new DriverNoLoad(0);
                }
                else if (this.SetupId.Equals(Consts.STRXML_SetupId_SynchronousSpeed))
                {
                    driver = new DriverSynchronousSpeed(0);
                }
                else if (this.SetupId.Equals(Consts.STRXML_SetupId_FullLoad))
                {
                    driver = new DriverFullLoad(0);
                }

                // Get the driver's execution time for this specification
                int executionTime = driver.GetExecutionTime(this);

                // Add on 10% to the execution time for good measure
                validationReport.estRuntime = executionTime + executionTime / 10;

                // Specification is valid
                validationReport.accepted = true;
            }
            catch (Exception ex)
            {
                validationReport.errorMessage = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            Logfile.Write(STRLOG_accepted + validationReport.accepted.ToString());
            if (validationReport.accepted == true)
            {
                Logfile.Write(STRLOG_executionTime + validationReport.estRuntime.ToString() + STRLOG_seconds);
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return validationReport;
        }

    }
}
