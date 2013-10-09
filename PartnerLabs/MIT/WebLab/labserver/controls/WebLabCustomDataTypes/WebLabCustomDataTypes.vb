Imports System
Imports System.Collections

'Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
'Please see license.txt in top level directory for full license. 

'Author: James Hardison (hardison@alum.mit.edu)
'Date: 8/19/2004
'Date Modified: 4/5/2005
'This namespace/class will be used as a library for custom built datatypes used in the Microelectronic WebLab Lab Server.  At its initial writing, this 
'class will contain data structures for use by WebLabDataManager methods which return multiple pieces of data in their result set, so as to eliminate the
'use of string arrays as return types.  These types will be referenced by both the data manager classes and the pieces of code which call them.  
'
'As of 4/5/2005, this namespace will also contain data structures used by the experiment validation and execution components for the storage of parsed 
'experiment specifications.
'
'Dependency List:
'Used by:
'   Resource Permission Mananger (/bin/ResourcePermissionManager.dll, /controls/WebLabDataManagers/ResourcePermissionManager.vb)
'   Record Manager (/bin/RecordManager.dll, /controls/WebLabDataManagers/RecordManager.vb)
'   Queue Manager (/bin/QueueManager.dll, /controls/WebLabDataManagers/QueueManager.vb)
'   Service Broker Managment Page (/admin/service-brokers.aspx)
'   Lab Server Web Services Interface Code (/bin/WebLabServices.dll, /services/WebLabService/LabServerAPI.vb)
'   WebLab Custom DataTypes Library (/bin/WebLabCustomTypes.dll) - this source compiled
'   Experiment Parser (/controls/WebLabSystemComponents/ExperimentParser/ExperimentParser.vb, /bin/ExeperimentParser.dll)
'   Experiment Validator (/controls/WebLabSystemComponents/ExperimentValidator/ExperimentValidator.vb, /bin/ExperimentValidator.dll)
'
'Uses:
'   None

Namespace WebLabCustomDataTypes

    Public Class NewBrokerConf
        'The New Broker COnfirmation Object contains the database record ID of a newly created broker and any comments generated during 
        'creation (success/error messages)
        'Used By:
        '   Resource Permission Mananger (/bin/ResourcePermissionManager.dll, /controls/WebLabDataManagers/ResourcePermissionManager.vb)
        '   Service Broker Managment Page (/admin/service-brokers.aspx)
        Private _brokerID As Integer
        Private _comments As String

        '---constructors (custom overload)---
        Public Sub New(ByVal BrokerID As Integer, ByVal Comments As String)
            _brokerID = BrokerID
            _comments = Comments
        End Sub

        '---Property Declarations---
        Public Property BrokerID() As Integer
            Get
                Return _brokerID
            End Get
            Set(ByVal Value As Integer)
                _brokerID = Value
            End Set
        End Property

        Public Property Comments() As String
            Get
                Return _comments
            End Get
            Set(ByVal Value As String)
                _comments = Value
            End Set
        End Property

    End Class


    Public Class ClassObject
        'The Class Description Object contains the english name and database record ID values of a Lab Server Usage Class
        'Used By:
        '   Resource Permission Mananger (/bin/ResourcePermissionManager.dll, /controls/WebLabDataManagers/ResourcePermissionManager.vb)
        Private _classID As Integer
        Private _className As String

        '--constructors (custom overload)---
        Public Sub New(ByVal ClassID As Integer, ByVal ClassName As String)
            _classID = ClassID
            _className = ClassName
        End Sub

        '---property declarations---
        Public Property ClassID() As Integer
            Get
                Return _classID
            End Get
            Set(ByVal Value As Integer)
                _classID = Value
            End Set
        End Property

        Public Property ClassName() As String
            Get
                Return _className
            End Get
            Set(ByVal Value As String)
                _className = Value
            End Set
        End Property

    End Class


    Public Class PermissionMappingObject
        'The Permission Mapping Object contains information describing a given permission mapping between a Usage Class and a 
        'System Resource.  This object contains the following information.  
        '1. Mapping ID - The database record ID of the mapping described by the object
        '2. CanView - A boolean value describing whether the referenced class can read data in the referenced resource.
        '3. CanEdit - A boolean value describing whether the referenced class can edit data in the referenced resource.
        '4. CanDelete - A  boolean value describing whether the referenced class can delete data in the referenced resource.
        '5. CanGrant - A boolean value describing whether the referenced class can grant access to data in the referenced resource
        '       to other classes.
        '6. Priority - A integer value pertaining only to resources that are device profiles.  This value determines the classes queue
        '       priority when submitting jobs for the referenced device profile resource.  Valid priority values are those between +20
        '       and -20.
        '
        'Used By:
        '   Resource Permission Mananger (/bin/ResourcePermissionManager.dll, /controls/WebLabDataManagers/ResourcePermissionManager.vb)
        Private _mappingID As Integer
        Private _canView As Boolean
        Private _canEdit As Boolean
        Private _canDelete As Boolean
        Private _canGrant As Boolean
        Private _priority As Integer

        '--constructors (custom overloads)---
        Public Sub New(ByVal MappingID As Integer, ByVal CanView As Boolean, ByVal CanEdit As Boolean, ByVal CanDelete As Boolean, ByVal CanGrant As Boolean, ByVal Priority As Integer)
            _mappingID = MappingID
            _canView = CanView
            _canEdit = CanEdit
            _canDelete = CanDelete
            _canGrant = CanGrant
            _priority = Priority
        End Sub

        '---property declarations
        Public Property MappingID() As Integer
            Get
                Return _mappingID
            End Get
            Set(ByVal Value As Integer)
                _mappingID = Value
            End Set
        End Property

        Public Property CanView() As Boolean
            Get
                Return _canView
            End Get
            Set(ByVal Value As Boolean)
                _canView = Value
            End Set
        End Property

        Public Property CanEdit() As Boolean
            Get
                Return _canEdit
            End Get
            Set(ByVal Value As Boolean)
                _canEdit = Value
            End Set
        End Property

        Public Property CanDelete() As Boolean
            Get
                Return _canDelete
            End Get
            Set(ByVal Value As Boolean)
                _canDelete = Value
            End Set
        End Property

        Public Property CanGrant() As Boolean
            Get
                Return _canGrant
            End Get
            Set(ByVal Value As Boolean)
                _canGrant = Value
            End Set
        End Property

        Public Property Priority() As Integer
            Get
                Return _priority
            End Get
            Set(ByVal Value As Integer)
                _priority = Value
            End Set
        End Property

    End Class


    Public Class ExpStatusObject
        'The Experiment Status Object contains information describing the status of a given experiment.  This object contains the 
        'following information.
        '1. Queue Position - An integer value detailing the experiments order in the execution queue at the time of object creation.
        '       If the experiment is valid but not in the queue (executing or completed), this value will be 0.  If the experiment
        '       is invalid (does not exist) this value will be -2.
        '2. Estimated Time to Run - An integer value giving an estimate (in seconds) of how long it will take for the job advance to 
        '       to the front of the queue and complete.
        '3. Estimated Execution Time - An integer value giving an estimate (in seconds) of how long it will take for the job to
        '       execute after it has advanced to the front of the queue.  This is a static value.
        '4. Estimated Remaining Execution Time - An integer value giving an estimate (in seconds) of how long it will take for 
        '       the job to complete once it has begun execution.  This is a dynamic value that counts down over the duration of 
        '       the experiment.
        '
        'Used By:
        '   Queue Manager (/bin/QueueManager.dll, /controls/WebLabDataManagers/QueueManager.vb)
        '   Lab Server Web Services Interface Code (/bin/WebLabServices.dll, /services/WebLabService/LabServerAPI.vb)

        Private _queuePosition As Integer
        Private _estTimeToRun As Integer
        Private _estExecTime As Integer
        Private _estRemainExecTime As Integer

        '--Constructors (custom overloads)---
        Public Sub New(ByVal QueuePosition As Integer, ByVal EstTimeToRun As Integer, ByVal EstExecTime As Integer)
            'legacy constructor (7/11/07, addition of _estRemainExecTime property)
            _queuePosition = QueuePosition
            _estTimeToRun = EstTimeToRun
            _estExecTime = EstExecTime
            _estRemainExecTime = EstExecTime
        End Sub

        Public Sub New(ByVal QueuePosition As Integer, ByVal EstTimeToRun As Integer, ByVal EstExecTime As Integer, ByVal EstRemainExecTime As Integer)
            _queuePosition = QueuePosition
            _estTimeToRun = EstTimeToRun
            _estExecTime = EstExecTime
            _estRemainExecTime = EstRemainExecTime
        End Sub

        '---property declarations---
        Public Property QueuePosition() As Integer
            Get
                Return _queuePosition
            End Get
            Set(ByVal Value As Integer)
                _queuePosition = Value
            End Set
        End Property

        Public Property EstTimeToRun() As Integer
            Get
                Return _estTimeToRun
            End Get
            Set(ByVal Value As Integer)
                _estTimeToRun = Value
            End Set
        End Property

        Public Property EstExecTime() As Integer
            Get
                Return _estExecTime
            End Get
            Set(ByVal Value As Integer)
                _estExecTime = Value
            End Set
        End Property

        Public Property EstRemainExecTime() As Integer
            Get
                Return _estRemainExecTime
            End Get
            Set(ByVal Value As Integer)
                _estRemainExecTime = Value
            End Set
        End Property

    End Class


    Public Class QueueLengthObject
        'The Queue Length Object contains information on the length of the experiment queue.  This object contains the following
        'information.
        '1. Queue Length - An integer value indicating how many jobs are currently in the execution queue.
        '2. Estimated Time To Run - An integer value giving an estimate (in seconds) of how long it will take for the job advance to 
        '       to the front of the queue and complete.

        'Used By:
        '   Queue Manager (/bin/QueueManager.dll, /controls/WebLabDataManagers/QueueManager.vb)
        '   Lab Server Web Services Interface Code (/bin/WebLabServices.dll, /services/WebLabService/LabServerAPI.vb)
        Private _queueLength As Integer
        Private _estTimeToRun As Integer

        '---constructors (custom overloads)---
        Public Sub New(ByVal QueueLength As Integer, ByVal EstTimeToRun As Integer)
            _queueLength = QueueLength
            _estTimeToRun = EstTimeToRun
        End Sub

        '---property declarations---
        Public Property QueueLength() As Integer
            Get
                Return _queueLength
            End Get
            Set(ByVal Value As Integer)
                _queueLength = Value
            End Set
        End Property

        Public Property EstTimeToRun() As Integer
            Get
                Return _estTimeToRun
            End Get
            Set(ByVal Value As Integer)
                _estTimeToRun = Value
            End Set
        End Property

    End Class


    Public Class ResultObject
        'The Result Object contains experiment results and execution notes.  Specifically, this object contains the following information.
        '1. Experiment Status - An integer value detailing the current status of the job.  The following are valid values and their meanings
        '       1 - Job/broker combo is valid and still in the queue.
        '       2 - Job/broker combo is valid and currently executing.
        '       3 - Job/broker combo is valid and terminated normally
        '       4 - Job/broker combo is valid and terminated with errors.
        '       5 - Job/broker combo is valid and was cancelled by broker.
        '       6 - Job/broker combo is invalid.
        '2. Experiment Results - A string value containing the results of the experiment.  If the job has completed, this field contains
        '       an XML document containing the results of the run.  If the job has not yet completed, this field is blank.
        '3. Warning Mesages - a string value containing any warnings returned with the experiment data.  Individual warning messages will be 
        '       terminated by the character string ";;".  If the job has not completed or if no warnings were returned, this field will
        '       be empty.
        '4. Error Massages - A string value containing any fatal error messages generated during job execution.  If the job has not yet 
        '       completed or if it has completed successfully, this field will be empty.
        '5. Lab Config - A string value containing the lab configuration at the time of job execution.  If the job has begin or completed 
        '       execution, this field will contain an XML document describing the configuration of the lab.  Otherwise, this field will be 
        '       empty.
        '
        'Used By:
        '   Record Manager (/bin/RecordManager.dll, /controls/WebLabDataManagers/RecordManager.vb)
        '   Lab Server Web Services Interface Code (/bin/WebLabServices.dll, /services/WebLabService/LabServerAPI.vb)

        Private _experimentStatus As Integer
        Private _experimentResults As String
        Private _warningMessages As String
        Private _errorMessages As String
        Private _labConfig As String

        '---constructors (custom overloads)---
        Public Sub New(ByVal ExperimentStatus As Integer, ByVal ExperimentResults As String, ByVal WarningMessages As String, ByVal ErrorMessages As String, ByVal LabConfig As String)
            _experimentStatus = ExperimentStatus
            _experimentResults = ExperimentResults
            _warningMessages = WarningMessages
            _errorMessages = ErrorMessages
            _labConfig = LabConfig
        End Sub

        '---property declarations---
        Public Property ExperimentStatus() As Integer
            Get
                Return _experimentStatus
            End Get
            Set(ByVal Value As Integer)
                _experimentStatus = Value
            End Set
        End Property

        Public Property ExperimentResults() As String
            Get
                Return _experimentResults
            End Get
            Set(ByVal Value As String)
                _experimentResults = Value
            End Set
        End Property

        Public Property WarningMessages() As String
            Get
                Return _warningMessages
            End Get
            Set(ByVal Value As String)
                _warningMessages = Value
            End Set
        End Property

        Public Property ErrorMessages() As String
            Get
                Return _errorMessages
            End Get
            Set(ByVal Value As String)
                _errorMessages = Value
            End Set
        End Property

        Public Property LabConfig() As String
            Get
                Return _labConfig
            End Get
            Set(ByVal Value As String)
                _labConfig = Value
            End Set
        End Property

    End Class


    Public Class ExpRecordInfoObject
        'The Experiment Record Info Object contains information on an experiment (current or past).  This object contains the following
        'information.
        '1. User Group - A string value detailing the Broker Effective Group associated with the job.  If there is no group supplied, or 
        '       if the group is not recognized, this field is blank.
        '2. Submit Time - A string value detailing the date and time (local to the lab server) when the job was submitted.
        '3. Execution Time - A string value detailing the date and time (local to the lab server) when the job began executing.
        '4. End Time - A string value detailing the date and time (local to the lab server) when the job completed execution.
        '5. Execution Elapsed - An integer value detailing the elapsed time (in seconds) that it took for the job to execute.  This value
        '       does not include the time spent waiting in the execution queue (from exec time to end time).
        '6. Job Elapsed - An integer value detailing the elapsed time (in seconds) that it took for the job to progress through the queue 
        '       and execute (from submit time to end time).
        '
        'Used By:
        '   Record Manager (/bin/RecordManager.dll, /controls/WebLabDataManagers/RecordManager.vb)
        '   Lab Server Web Services Interface Code (/bin/WebLabServices.dll, /services/WebLabService/LabServerAPI.vb)
        Private _userGroup As String
        Private _submitTime As String
        Private _execTime As String
        Private _endTime As String
        Private _execElapsed As Integer
        Private _jobElapsed As Integer
        Private _deviceName As String

        '---constructors (custom overloads)---
        Public Sub New(ByVal UserGroup As String, ByVal SubmitTime As String, ByVal ExecTime As String, ByVal EndTime As String, ByVal ExecElapsed As Integer, ByVal JobElapsed As Integer, ByVal DeviceName As String)
            _userGroup = UserGroup
            _submitTime = SubmitTime
            _execTime = ExecTime
            _endTime = EndTime
            _execElapsed = ExecElapsed
            _jobElapsed = JobElapsed
            _deviceName = DeviceName
        End Sub

        '---property declarations---
        Public Property UserGroup() As String
            Get
                Return _userGroup
            End Get
            Set(ByVal Value As String)
                _userGroup = Value
            End Set
        End Property

        Public Property SubmitTime() As String
            Get
                Return _submitTime
            End Get
            Set(ByVal Value As String)
                _submitTime = Value
            End Set
        End Property

        Public Property ExecTime() As String
            Get
                Return _execTime
            End Get
            Set(ByVal Value As String)
                _execTime = Value
            End Set
        End Property

        Public Property EndTime() As String
            Get
                Return _endTime
            End Get
            Set(ByVal Value As String)
                _endTime = Value
            End Set
        End Property

        Public Property ExecElapsed() As Integer
            Get
                Return _execElapsed
            End Get
            Set(ByVal Value As Integer)
                _execElapsed = Value
            End Set
        End Property

        Public Property JobElapsed() As Integer
            Get
                Return _jobElapsed
            End Get
            Set(ByVal Value As Integer)
                _jobElapsed = Value
            End Set
        End Property

        Public Property DeviceName() As String
            Get
                Return _deviceName
            End Get
            Set(ByVal Value As String)
                _deviceName = Value
            End Set
        End Property

    End Class


    '---------------------Experiment Specification Parsing Data Structures-------------------------

    Public Class ExpUnitSetObject
        'The Experiment Unit Set Object contains configuration information for individual units from a parsed experiment specification.  This 
        'structure is comprised of a hashtable of hashtables.  The top-level table indexes the individual units defined by the experiment specification.
        'In the WebLab case, these are used individually for terminal specifications, keyed on terminal port name and user-defined fuction specificaions, 
        'keyed on the UDF name.  The nested hashtables contain the set of terminal settings for a given terminal.  Those nested hashtables contain keys 
        'defined at runtime (for flexibility/reusability).  Use context is defined by the class using this data structure. 

        'Used By:
        '   Experiment Parser (/controls/WebLabSystemComponents/ExperimentParser/ExperimentParser.vb, /bin/ExeperimentParser.dll)
        '   Experiment Validator (/controls/WebLabSystemComponents/ExperimentValidator/ExperimentValidator.vb, /bin/ExperimentValidator.dll)

        Private _MainTable As New Hashtable()

        '---constructors (custom overload)---
        Public Sub New(ByVal UnitName As String)
            'allows creation of one named, nested unit item hashtable during construction
            _MainTable.Add(UnitName, New Hashtable())
        End Sub

        Public Sub New()
            'default constructor
        End Sub

        '---non-property accessor methods---
        Public Sub CreateUnit(ByVal UnitName As String)
            If _MainTable.ContainsKey(UnitName) Then
                Throw New Exception("The name " & UnitName & " is already used in this structure.")
            Else
                _MainTable.Add(UnitName, New Hashtable())
            End If
        End Sub

        Public Function EnumerateUnits() As String()
            Dim tempCollection As ICollection
            Dim Keys(_MainTable.Count() - 1) As String

            tempCollection = _MainTable.Keys()
            tempCollection.CopyTo(Keys, 0)

            Return Keys
        End Function

        Public Function EnumerateUnitItems(ByVal UnitName As String) As String()
            Dim tempCollection As ICollection
            Dim tempHash As Hashtable
            Dim Keys() As String

            If _MainTable.Contains(UnitName) Then
                tempHash = _MainTable.Item(UnitName)

                ReDim Keys(tempHash.Count() - 1)
                tempCollection = tempHash.Keys()
                tempCollection.CopyTo(Keys, 0)
            Else
                Throw New Exception("The name " & UnitName & " is not a valid terminal in this structure.")
            End If

            Return Keys
        End Function

        Public Function Count() As Integer
            'This method returns an integer value indicating the number of top-level items in this container
            Return _MainTable.Count()
        End Function

        '---Property Declarations---
        Public ReadOnly Property Unit(ByVal UnitName As String) As Hashtable
            Get
                Return _MainTable.Item(UnitName)
            End Get
        End Property

        Public Property UnitItem(ByVal UnitName As String, ByVal ItemName As String) As Object
            Get
                Return _MainTable.Item(UnitName).Item(ItemName)
            End Get
            Set(ByVal Value As Object)
                If _MainTable.Contains(UnitName) Then
                    _MainTable.Item(UnitName).Add(ItemName, Value)
                Else
                    Throw New Exception("Invalid Port Name.  Item not set.")
                End If
            End Set
        End Property
    End Class


    Public Class ExperimentObject
        'The experiment object is a data structure used as a collection for individual, parsed experiment specification information collections.  The 
        'core of this structure will be a hashtable that will have specialized, controlled access methods.  In the WebLab case, this collection will contain
        'two ExpUnitSetObjects (one for experiment terminal specifications and one for UDF specifications) as well as a deviceID object (integer or string).  
        'Specific data types (other than keys, which are strings) are not specified by this class.

        'Used By: 
        '   Experiment Parser (/controls/WebLabSystemComponents/ExperimentParser/ExperimentParser.vb, /bin/ExeperimentParser.dll)
        '   Experiment Validator (/controls/WebLabSystemComponents/ExperimentValidator/ExperimentValidator.vb, /bin/ExperimentValidator.dll)

        Private _MainTable As New Hashtable()

        '---non-property accessor methods---
        Public Sub AddItem(ByVal ItemName As String, ByVal Item As Object)
            If _MainTable.ContainsKey(ItemName) Then
                Throw New Exception("The name " & ItemName & " is already used in this structure.")
            Else
                _MainTable.Add(ItemName, Item)
            End If
        End Sub

        Public Function EnumerateItems() As String()
            Dim tempCollection As ICollection
            Dim Keys(_MainTable.Count() - 1) As String

            tempCollection = _MainTable.Keys()
            tempCollection.CopyTo(Keys, 0)

            Return Keys
        End Function

        '---Property Declarations---
        Public ReadOnly Property Item(ByVal ItemName As String) As Object
            Get
                Return _MainTable.Item(ItemName)
            End Get
        End Property
    End Class

    '-----------Experiment Validation Data Object-----------------
    Public Class ValidationObject
        'The validation object is a data structure used as a collection for individual pieces of data that are the result of the experiment
        'validation process.  These data items are a binary value indicating whether the experiment specification is valid or not, an error
        'message (in case the specification is not valid), the number of datapoints that will be generated by the experiment, the Lab Server
        'Device ID of the device indicated by the experiment specification as well as the system resource ID of the specified device.  After 
        'an instance of this object is created, the individual data properties are immutable by the reader.

        'Used By:
        '   Experiment Validator (/controls/WebLabSystemComponents/ExperimentValidator/ExperimentValidator.vb, /bin/ExperimentValidator.dll)
        '   Lab Server Web Services Interface Code (/bin/WebLabServices.dll, /services/WebLabService/LabServerAPI.vb)

        Private _isValid As Boolean
        Private _errorMessage As String
        Private _dataPoints As Integer
        Private _deviceProfileID As Integer
        Private _resourceID As Integer

        '---Constructor Methods---
        Public Sub New(ByVal isValid As Boolean, ByVal errorMessage As String, ByVal dataPoints As Integer, ByVal deviceProfileID As Integer, ByVal resourceID As Integer)
            _isValid = isValid
            _dataPoints = dataPoints
            _deviceProfileID = deviceProfileID
            _resourceID = resourceID

            If isValid Then
                _errorMessage = ""
            Else
                _errorMessage = errorMessage
            End If
        End Sub

        '---Non-property Accessor Methods---

        '---Property Declarations
        Public ReadOnly Property isValid() As Boolean
            Get
                Return _isValid
            End Get
        End Property

        Public ReadOnly Property errorMessage() As String
            Get
                Return _errorMessage
            End Get
        End Property

        Public ReadOnly Property dataPoints() As Integer
            Get
                Return _dataPoints
            End Get
        End Property

        Public ReadOnly Property deviceProfileID() As Integer
            Get
                Return _deviceProfileID
            End Get
        End Property

        Public ReadOnly Property resourceID() As Integer
            Get
                Return _resourceID
            End Get
        End Property
    End Class

End Namespace

