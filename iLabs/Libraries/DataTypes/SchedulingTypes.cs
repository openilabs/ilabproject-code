using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace iLabs.DataTypes.SchedulingTypes
{
    
    


    /// <remarks/>
   
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public class TimePeriod
    {

        private System.DateTime startTimeField;

        private System.DateTime endTimeField;

        /// <remarks/>
        public System.DateTime startTime
        {
            get
            {
                return this.startTimeField;
            }
            set
            {
                this.startTimeField = value;
            }
        }

        /// <remarks/>
        public System.DateTime endTime
        {
            get
            {
                return this.endTimeField;
            }
            set
            {
                this.endTimeField = value;
            }
        }

        public static ArrayList SortByTime(ArrayList timePeriods)
        {
            for (int i = timePeriods.Count; --i >= 0; )
            {
                bool flipped = false;
                for (int j = 0; j < i; j++)
                {

                    if (((TimePeriod)timePeriods[j]).startTime > ((TimePeriod)timePeriods[j + 1]).startTime)
                    {
                        TimePeriod T = (TimePeriod)timePeriods[j];
                        timePeriods[j] = timePeriods[j + 1];
                        timePeriods[j + 1] = T;
                        flipped = true;
                    }
                }

                if (!flipped)
                {
                    return timePeriods;
                }
            }
            return timePeriods;
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public class Reservation
    {


        private string userNameField;
        private System.DateTime startTimeField;
        private System.DateTime endTimeField;

      

        /// <remarks/>
        public string userName
        {
            get
            {
                return this.userNameField;
            }
            set
            {
                this.userNameField = value;
            }
        }

        /// <remarks/>
        public System.DateTime startTime
        {
            get
            {
                return this.startTimeField;
            }
            set
            {
                this.startTimeField = value;
            }
        }

        /// <remarks/>
        public System.DateTime endTime
        {
            get
            {
                return this.endTimeField;
            }
            set
            {
                this.endTimeField = value;
            }
        }

    }

    /// <summary>
    /// a structure which holds time block
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public struct TimeBlock
    {
   
        /// <summary>
        /// the start time of the time block
        /// </summary>
        public DateTime startTime;
        /// <summary>
        /// the end time of the time block
        /// </summary>
        public DateTime endTime;
        /// <summary>
        /// the GUID of the lab server that the time block belongs to
        /// </summary>
        public String labServerGuid;
      
    }

 

}
