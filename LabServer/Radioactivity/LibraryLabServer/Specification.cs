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
        // String constants for error messages
        //
        private const string STRERR_InvalidSource = "Invalid source";
        private const string STRERR_InvalidAbsorber = "Invalid absorber";

        //
        // Local variables
        //
        private Configuration configuration;
        private Validation validation;

        #endregion

        #region Properties

        private string sourceName;
        private char sourceLocation;
        private string absorberName;
        private char absorberLocation;
        private int distance;
        private int[] distanceList;
        private int duration;
        private int repeat;

        public string SourceName
        {
            get { return this.sourceName; }
        }

        public char SourceLocation
        {
            get { return this.sourceLocation; }
        }

        public string AbsorberName
        {
            get { return this.absorberName; }
        }

        public char AbsorberLocation
        {
            get { return this.absorberLocation; }
        }

        public int Distance
        {
            get { return this.distance; }
        }

        public int[] DistanceList
        {
            get { return this.distanceList; }
        }

        public int Duration
        {
            get { return this.duration; }
        }

        public int Repeat
        {
            get { return this.repeat; }
        }

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
                XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_sourceName, true);
                XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_absorberName, true);
                XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_distance, true);
                XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_duration, true);
                XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_repeat, true);

                //
                // Create an instance fo the Validation class
                //
                this.validation = new Validation(configuration);
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
                // Validate the specification
                //

                //
                // Get the source name and check that it is valid - search is case-sensitive
                //
                string sourceName = XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_sourceName, false);
                int index = Array.IndexOf(this.configuration.SourceNames, sourceName);
                if (index < 0)
                {
                    throw new ArgumentException(STRERR_InvalidSource, sourceName);
                }
                this.sourceName = this.configuration.SourceNames[index];
                this.sourceLocation = this.configuration.SourceLocations[index];

                //
                // Get the absorber name and check that it is valid - search is case-sensitive
                //
                string absorberName = XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_absorberName, false);
                index = Array.IndexOf(this.configuration.AbsorberNames, absorberName);
                if (index < 0)
                {
                    throw new ArgumentException(STRERR_InvalidAbsorber, absorberName);
                }
                this.absorberName = this.configuration.AbsorberNames[index];
                this.absorberLocation = this.configuration.AbsorberLocations[index];

                //
                // Get duration and validate
                //
                this.duration = XmlUtilities.GetIntValue(this.xmlNodeSpecification, Consts.STRXML_duration);
                this.validation.ValidateDuration(this.duration);

                //
                // Get repeat count and validate
                //
                this.repeat = XmlUtilities.GetIntValue(this.xmlNodeSpecification, Consts.STRXML_repeat);
                this.validation.ValidateRepeat(this.repeat);

                //
                // Get distance or distance list
                //
                if (this.SetupId.Equals(Consts.STRXML_SetupId_RadioactivityVsTime) ||
                    this.SetupId.Equals(Consts.STRXML_SetupId_SimActivityVsTime))
                {
                    // Get distance and validate
                    this.distance = XmlUtilities.GetIntValue(this.xmlNodeSpecification, Consts.STRXML_distance);
                    this.validation.ValidateDistance(this.distance);

                    // Validate total time
                    this.validation.ValidateTotalTime(this.duration * this.repeat);

                    // Not using distance list
                    this.distanceList = null;
                }
                else if (this.SetupId.Equals(Consts.STRXML_SetupId_RadioactivityVsDistance) ||
                    this.SetupId.Equals(Consts.STRXML_SetupId_SimActivityVsDistance))
                {
                    // Get distance list and validate
                    string csvString = XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_distance, false);
                    string[] csvStringSplit = csvString.Split(new char[] { Consts.CHR_CsvSplitter });
                    this.distanceList = new int[csvStringSplit.Length];
                    for (int i = 0; i < csvStringSplit.Length; i++)
                    {
                        try
                        {
                            this.distanceList[i] = Int32.Parse(csvStringSplit[i]);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException(ex.Message, Consts.STRXML_distance);
                        }
                        this.validation.ValidateDistance(this.distanceList[i]);
                    }

                    // Validate total time
                    this.validation.ValidateTotalTime(this.duration * this.repeat * this.distanceList.Length);

                    // Not using distance
                    this.distance = 0;
                }

                //
                // Create an instance of the driver for the specified setup
                //
                DriverGeneric driver = null;
                if (this.SetupId.Equals(Consts.STRXML_SetupId_RadioactivityVsTime))
                {
                    driver = new DriverRadioactivityVsTime();
                }
                else if (this.SetupId.Equals(Consts.STRXML_SetupId_RadioactivityVsDistance))
                {
                    string csvString = XmlUtilities.GetXmlValue(this.xmlNodeSetup, Consts.STRXML_distance, false);

                    driver = new DriverRadioactivityVsDistance();
                }
                else if (this.SetupId.Equals(Consts.STRXML_SetupId_SimActivityVsTime))
                {
                    driver = new DriverSimActivityVsTime();
                }
                else if (this.SetupId.Equals(Consts.STRXML_SetupId_SimActivityVsDistance))
                {
                    driver = new DriverSimActivityVsDistance();
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
