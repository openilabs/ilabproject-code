﻿using Library.Lab;
using Library.LabServerEngine;
using Library.LabServerEngine.Drivers.Setup;
using Library.LabServerEngine.Drivers.Equipment;

namespace Library.LabServer.Drivers.Setup
{
    public partial class DriverSpeedVsField : DriverEquipmentGeneric
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "DriverSpeedVsField";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_FieldMin = " Field Min: ";
        private const string STRLOG_FieldMax = " Field Max: ";
        private const string STRLOG_FieldStep = " Field Step: ";

        //
        // String constants for error messages
        //

        //
        // Local variables
        //
        private int measurementCount;

        #endregion

        //---------------------------------------------------------------------------------------//

        public DriverSpeedVsField(EquipmentService equipmentServiceProxy, Configuration configuration)
            : this(equipmentServiceProxy, configuration, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public DriverSpeedVsField(EquipmentService equipmentServiceProxy, Configuration configuration, CancelExperiment cancelExperiment)
            : base(equipmentServiceProxy, configuration, cancelExperiment)
        {
            const string STRLOG_MethodName = "DriverSpeedVsField";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Save for use by this driver
            //
            this.measurementCount = configuration.MeasurementCount;

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

    }
}
