using System;
using System.Drawing;
using System.IO;
using System.Xml;
using Library.Lab;
using Library.LabClient;

namespace LabClientHtml.LabControls
{
    public partial class LabSetup : System.Web.UI.UserControl
    {
        #region Class Constants and Variables

        //
        // String constants
        //
        private const string STR_Range = "Range: ";
        private const string STR_to = " to ";
        private const string STR_TotalTime = " - Total Time: ";
        private const string STR_seconds = " seconds";

        #endregion

        #region Properties

        private XmlNode xmlNodeConfiguration;
        private XmlNode xmlNodeValidation;
        private XmlNode xmlNodeSelectedSetup;

        public XmlNode XmlNodeConfiguration
        {
            get { return this.xmlNodeConfiguration; }
            set { this.xmlNodeConfiguration = value; }
        }

        public XmlNode XmlNodeValidation
        {
            get { return this.xmlNodeValidation; }
            set { this.xmlNodeValidation = value; }
        }

        public XmlNode XmlNodeSelectedSetup
        {
            get { return this.xmlNodeSelectedSetup; }
            set { this.xmlNodeSelectedSetup = value; }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulatePageControls();
            }
        }

        //-------------------------------------------------------------------------------------------------//

        private void ClearPageControls()
        {
            txbDistanceList.Text = null;
            txbDuration.Text = null;
            txbRepeat.Text = null;
        }

        //-------------------------------------------------------------------------------------------------//

        private void PopulatePageControls()
        {
            //
            // Cannot do anything without a configuration
            //
            if (this.xmlNodeConfiguration == null)
            {
                return;
            }

            //
            // Get a list of all sources and add to the dropdownlist
            //
            XmlNode xmlNode = XmlUtilities.GetXmlNode(this.xmlNodeConfiguration, LabConsts.STRXML_sources, false);
            XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(xmlNode, LabConsts.STRXML_source, false);
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlNode xmlNodeTemp = xmlNodeList.Item(i);

                string sourceName = XmlUtilities.GetXmlValue(xmlNodeTemp, LabConsts.STRXML_name, false);
                ddlSource.Items.Add(sourceName);
            }

            //
            // Get a list of all absorbers and add to the dropdownlist
            //
            xmlNode = XmlUtilities.GetXmlNode(this.xmlNodeConfiguration, LabConsts.STRXML_absorbers, false);
            xmlNodeList = XmlUtilities.GetXmlNodeList(xmlNode, LabConsts.STRXML_absorber, false);
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlNode xmlNodeTemp = xmlNodeList.Item(i);

                string absorberName = XmlUtilities.GetXmlValue(xmlNodeTemp, LabConsts.STRXML_name, false);
                ddlAbsorber.Items.Add(absorberName);
            }

            //
            // Get a list of distances and add to the dropdownlist
            //
            xmlNode = XmlUtilities.GetXmlNode(this.xmlNodeConfiguration, LabConsts.STRXML_distances, true);
            if (xmlNode != null)
            {
                // Get minimum distance
                int minimum = XmlUtilities.GetIntValue(xmlNode, LabConsts.STRXML_minimum);

                // Get maximum distance
                int maximum = XmlUtilities.GetIntValue(xmlNode, LabConsts.STRXML_maximum);

                // Get distance stepsize
                int stepsize = XmlUtilities.GetIntValue(xmlNode, LabConsts.STRXML_stepsize);

                //
                // Add numbers to the Distance dropdownlist if range is valid
                //
                if (minimum >= 0 && maximum > 0 && stepsize > 0)
                {
                    for (int i = minimum; i <= maximum; i += stepsize)
                    {
                        ddlDistance.Items.Add(i.ToString());
                    }
                }
            }

            //
            // Update controls for selected setup
            //
            UpdatePageControls();
        }

        //-------------------------------------------------------------------------------------------------//

        private void UpdatePageControls()
        {
            //
            // Cannot do anything without a configuration
            //
            if (this.xmlNodeConfiguration == null)
            {
                return;
            }

            //
            // Set default selection for source
            //
            XmlNode xmlNode = XmlUtilities.GetXmlNode(this.xmlNodeConfiguration, LabConsts.STRXML_sources, true);
            string defaultSource = XmlUtilities.GetXmlValue(xmlNode, LabConsts.STRXMLPARAM_default, true);
            if (defaultSource.Length > 0)
            {
                ddlSource.SelectedValue = defaultSource;
            }

            //
            // Set default selection for absorber
            //
            xmlNode = XmlUtilities.GetXmlNode(this.xmlNodeConfiguration, LabConsts.STRXML_absorbers, true);
            string defaultAbsorber = XmlUtilities.GetXmlValue(xmlNode, LabConsts.STRXMLPARAM_default, true);
            if (defaultAbsorber.Length > 0)
            {
                ddlAbsorber.SelectedValue = defaultAbsorber;
            }

            //
            // Get the ID of the selected setup
            //
            string setupId = XmlUtilities.GetXmlValue(this.xmlNodeSelectedSetup, Consts.STRXMLPARAM_id, false);

            //
            // Show/hide the page controls for the specified setup
            //
            if (setupId.Equals(LabConsts.STRXML_SetupId_RadioactivityVsTime) ||
                setupId.Equals(LabConsts.STRXML_SetupId_SimActivityVsTime))
            {
                //
                // Hide DistanceList controls
                //
                lblDistanceList.Visible = false;
                btnDistanceListAdd.Visible = false;
                btnDistanceListClear.Visible = false;
                txbDistanceList.Visible = false;

                //
                // Set default distance
                //
                ddlDistance.SelectedValue = XmlUtilities.GetXmlValue(this.xmlNodeSelectedSetup, LabConsts.STRXML_distance, true);
            }
            else if (setupId.Equals(LabConsts.STRXML_SetupId_RadioactivityVsDistance) ||
                setupId.Equals(LabConsts.STRXML_SetupId_SimActivityVsDistance))
            {
                //
                // Show DistanceList controls
                //
                lblDistanceList.Visible = true;
                btnDistanceListAdd.Visible = true;
                btnDistanceListClear.Visible = true;
                txbDistanceList.Visible = true;

                //
                // Set default distance list
                //
                txbDistanceList.Text = XmlUtilities.GetXmlValue(this.xmlNodeSelectedSetup, LabConsts.STRXML_distance, true);
            }

            //
            // Boundary values for total time
            //
            XmlNode xmlNodeTemp = XmlUtilities.GetXmlNode(this.xmlNodeValidation, LabConsts.STRXML_vdnTotaltime);
            int totalTimeMin = XmlUtilities.GetIntValue(xmlNodeTemp, LabConsts.STRXML_minimum);
            int totalTimeMax = XmlUtilities.GetIntValue(xmlNodeTemp, LabConsts.STRXML_maximum);

            //
            // Set default duration
            //
            int duration = XmlUtilities.GetIntValue(this.xmlNodeSelectedSetup, LabConsts.STRXML_duration);
            txbDuration.Text = duration.ToString();

            //
            // Boundary values and tooltips for duration
            //
            xmlNodeTemp = XmlUtilities.GetXmlNode(this.xmlNodeValidation, LabConsts.STRXML_vdnDuration);
            int durationMin = XmlUtilities.GetIntValue(xmlNodeTemp, LabConsts.STRXML_minimum);
            int durationMax = XmlUtilities.GetIntValue(xmlNodeTemp, LabConsts.STRXML_maximum);
            txbDuration.ToolTip = STR_Range + durationMin.ToString() + STR_to + durationMax.ToString()
                + STR_TotalTime + totalTimeMin.ToString() + STR_to + totalTimeMax.ToString() + STR_seconds;

            //
            // Set default repeat count
            //
            int repeat = XmlUtilities.GetIntValue(this.xmlNodeSelectedSetup, LabConsts.STRXML_repeat);
            txbRepeat.Text = repeat.ToString();

            //
            // Boundary values and tooltips for repeat count
            //
            xmlNodeTemp = XmlUtilities.GetXmlNode(this.xmlNodeValidation, LabConsts.STRXML_vdnRepeat);
            int repeatMin = XmlUtilities.GetIntValue(xmlNodeTemp, LabConsts.STRXML_minimum);
            int repeatMax = XmlUtilities.GetIntValue(xmlNodeTemp, LabConsts.STRXML_maximum);
            txbRepeat.ToolTip = STR_Range + repeatMin.ToString() + STR_to + repeatMax.ToString()
                + STR_TotalTime + totalTimeMin.ToString() + STR_to + totalTimeMax.ToString() + STR_seconds;
        }

        //-------------------------------------------------------------------------------------------------//

        public void ddlExperimentSetups_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear page controls before selecting another setup
            ClearPageControls();

            // Update page controls for the selected index
            UpdatePageControls();
        }

        //---------------------------------------------------------------------------------------//

        public XmlNode BuildSpecification(XmlNode xmlNodeSpecification, string setupId)
        {
            //
            // Fill in specification information only for selected setup
            //
            XmlUtilities.SetXmlValue(xmlNodeSpecification, LabConsts.STRXML_sourceName, ddlSource.SelectedValue, false);
            XmlUtilities.SetXmlValue(xmlNodeSpecification, LabConsts.STRXML_absorberName, ddlAbsorber.SelectedValue, false);
            if (setupId.Equals(LabConsts.STRXML_SetupId_RadioactivityVsTime) ||
                setupId.Equals(LabConsts.STRXML_SetupId_SimActivityVsTime))
            {
                XmlUtilities.SetXmlValue(xmlNodeSpecification, LabConsts.STRXML_distance, ddlDistance.SelectedValue, false);
            }
            else if (setupId.Equals(LabConsts.STRXML_SetupId_RadioactivityVsDistance) ||
                setupId.Equals(LabConsts.STRXML_SetupId_SimActivityVsDistance))
            {
                XmlUtilities.SetXmlValue(xmlNodeSpecification, LabConsts.STRXML_distance, txbDistanceList.Text, false);
            }
            XmlUtilities.SetXmlValue(xmlNodeSpecification, LabConsts.STRXML_duration, txbDuration.Text, false);
            XmlUtilities.SetXmlValue(xmlNodeSpecification, LabConsts.STRXML_repeat, txbRepeat.Text, false);

            return xmlNodeSpecification;
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnDistanceListAdd_Click(object sender, EventArgs e)
        {
            // Check if distance list is empty
            string strDistanceList = txbDistanceList.Text.Trim();
            if (strDistanceList.Length == 0)
            {
                // Add value to list and return
                txbDistanceList.Text = ddlDistance.SelectedValue;
                return;
            }

            string strValue = ddlDistance.SelectedValue;

            // Split the distance list csv string
            string[] strSplit = strDistanceList.Split(new char[] { ',' });

            // Check that the selected distance doesn't exist in the list
            for (int i = 0; i < strSplit.Length; i++)
            {
                if (strSplit[i].Equals(strValue) == true)
                {
                    // Already in the list
                    return;
                }
            }

            //
            // Selected distance doesn't exist in the list yet.
            // Add to comma-seperated value string in sorted order
            //
            try
            {
                string csvString = "";

                // Convert the selected distance to a number
                int value = Convert.ToInt16(strValue);

                // Find index to insert selected distance
                int index = 0;
                while (index < strSplit.Length)
                {
                    // Convert the current distance to a number and compare
                    int listValue = Convert.ToInt16(strSplit[index]);
                    if (value < listValue)
                    {
                        break;
                    }

                    // Try next one
                    index++;
                }

                //
                // Create csv string
                //
                for (int i = 0; i < index; i++)
                {
                    if (i > 0)
                    {
                        csvString += ",";
                    }
                    int listValue = Convert.ToInt16(strSplit[i]);
                    csvString += listValue.ToString();
                }

                //
                // Insert selected value into the csv string
                //
                if (index > 0)
                {
                    csvString += ",";
                }
                csvString += value.ToString();

                for (int i = index; i < strSplit.Length; i++)
                {
                    csvString += ",";

                    int listValue = Convert.ToInt16(strSplit[i]);
                    csvString += listValue.ToString();
                }

                // csv string is complete
                txbDistanceList.Text = csvString;
            }
            catch
            {
            }
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnDistanceListClear_Click(object sender, EventArgs e)
        {
            txbDistanceList.Text = null;
        }

    }
}