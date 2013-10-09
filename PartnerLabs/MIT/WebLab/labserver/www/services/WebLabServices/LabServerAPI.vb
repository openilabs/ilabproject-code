Imports System
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports WebLabDataManagers
Imports WebLabCustomDataTypes
'Imports validation_engine.WebLabSystemComponents
Imports WebLabSystemComponents
Imports ILabStandardTypes

'Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
'Please see license.txt in top level directory for full license. 

'Date Created: 6/20/2003
'Date Modified: 9/23/2004
'This class implements the Lab Server API for use in the iLab Shared Architecture.  Here, Lab Server methods are defined and made available 
'via web services.  Users of this interface must have registered with the Lab Server first in order to pass its authentication checks.
'
'Dependency List
'Used By:
'   WebLab Lab Server Web Service Interface File (/services/WebLabService.asmx)
'   Lab Server Web Service Interface Library (/bin/WebLabServices.dll)
'
'Uses:
'   WebLab Custom Data Type Library (/bin/WebLabCustomDataTypes.dll, /controls/WebLabCustomDataTypes/WebLAbCustomDataTypes.vb)
'   iLab Standard Types Library (built from wsdl) (/bin/ILabStandardTypes.dll, /services/ILabStandardTypes.asmx)
'   Lab Server Experiment Validation component library (/bin/validation_engine.dll, /controls/validation_engine/ValidationEngine.vb)
'   Lab Server Resource Permission Manager Library (/bin/ResourcePermissionManager.dll, /controls/WebLabDataManagers/ResourcePermissionManager.vb)
'   Lab Server Record Manager Library (/bin/RecordManager.dll, /controls/WebLabDataManagers/RecordManager.vb)
'   Lab Server Queue Manager Library (/bin/QueueManager.dll, /controls/WebLabDataManagers/QueueManager.vb)

Public Class AuthHeader
    Inherits SoapHeader
    'This class defines the Authentication Header object.  For each Web Method call, an instance of this class, containing the caller's 
    'server ID and passkey will be passed in the header of the SOAP Request.  
    Public identifier As String
    Public passKey As String
End Class

<XmlType(Namespace:="http://ilab.mit.edu"), WebService(Description:="WebLab Lab Server/Service Broker Interface", Namespace:="http://ilab.mit.edu")> _
Public Class LabServerAPI : Inherits WebService
    'This class defines the body of the WebLab LAb Server API.

    'member variable from SoapHeader, instantiated so as to accept and interpret AuthHeader objects in incoming SOAP Requests. 
    Public BrokerCred As AuthHeader

    <WebMethod(Description:="WebLab Lab Server Method: Returns the current status of the lab.  Members of struct LabStatus are 'online' (boolean) and 'labStatusMessage' (string)."), SoapHeader("BrokerCred", Direction:=SoapHeaderDirection.In), SoapDocumentMethod(RequestNamespace:="http://ilab.mit.edu")> _
    Public Function GetLabStatus() As LabStatus
        'This method returns the current status of the lab to the caller.  In the future, labStatusMessage should incorporate data 
        'from system notices.
        Dim lsOutput As LabStatus = New LabStatus()
        Dim rmObject As RecordManager = New RecordManager()
        Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()
        Dim intBrokerID As Integer
        Dim blnHasPermission As Boolean = False

        'attempt to read AuthHeader object from SOAP header
        If Not BrokerCred Is Nothing Then
            Try
                intBrokerID = AuthenticateCaller(BrokerCred.identifier, BrokerCred.passKey)
                BrokerCred.DidUnderstand = True
            Catch
                BrokerCred.DidUnderstand = False
                intBrokerID = -1
            End Try

            'once identity is determined, check caller is authorized to use this interface
            blnHasPermission = AuthorizeBroker(intBrokerID, "canview")


            If blnHasPermission Then
                Try
                    'core method body
                    If GetInterfaceStatus() Then

                        lsOutput.online = True

                    Else
                        lsOutput.online = False
                    End If

                    lsOutput.labStatusMessage = rpmObject.GetLabStatusMessage() & "  Current local time: " & CStr(Now())
                Catch
                    'error case
                    rmObject.LogIncomingWebRequest(intBrokerID, "GetLabStatus", blnHasPermission, False, "Method Error: " & Err.Description() & ": " & Err.Source())
                    'Return lsOutput
                    Throw New Protocols.SoapException("Method Error: " & Err.Description() & ": " & Err.Source(), Protocols.SoapException.ServerFaultCode)
                    Exit Function
                End Try
                'success case
                rmObject.LogIncomingWebRequest(intBrokerID, "GetLabStatus", blnHasPermission, True, "Completed Successfully.")
            Else
                'insufficient permission case
                rmObject.LogIncomingWebRequest(intBrokerID, "GetLabStatus", blnHasPermission, False, "Insufficient permission for method execution.")
                Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                Exit Function
            End If
        Else
            'case where no credentials are supplied
            rmObject.LogIncomingWebRequest(intBrokerID, "GetLabStatus", blnHasPermission, False, "Insufficient permission for method execution.")
            Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
            Exit Function
        End If

        Return lsOutput
    End Function

    <WebMethod(Description:="WebLab Lab Server Method: Returns the current length of the experiment queue based on a hypothetical priority, group and broker membership.  Members of struct WaitEstimate are 'effectiveQueueLength' (int) and 'estWait' (double)."), SoapHeader("BrokerCred", Direction:=SoapHeaderDirection.In), SoapDocumentMethod(RequestNamespace:="http://ilab.mit.edu")> _
    Public Function GetEffectiveQueueLength(ByVal userGroup As String, ByVal priorityHint As Integer) As WaitEstimate
        'This is a method used to return the the current length of the experiment queue based on the callers permissions and priority.  
        Dim weOutput As WaitEstimate = New WaitEstimate()
        Dim qmObject As QueueManager = New QueueManager()
        Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()
        Dim rmObject As RecordManager = New RecordManager()
        Dim queueLength As QueueLengthObject
        Dim intBrokerID, intPriority, intGroupID As Integer
        Dim blnHasPermission As Boolean = False

        'attempt to read AuthHeader object from SOAP header
        If Not BrokerCred Is Nothing Then
            Try
                intBrokerID = AuthenticateCaller(BrokerCred.identifier, BrokerCred.passKey)
                BrokerCred.DidUnderstand = True
            Catch
                BrokerCred.DidUnderstand = False
                intBrokerID = -1
            End Try

            'once identity is determined, check caller is authorized to use this interface
            blnHasPermission = AuthorizeBroker(intBrokerID, "canview")


            If blnHasPermission Then 'if caller has permission

                If GetInterfaceStatus() Then 'if interface is enabled.
                    Try
                        'core method body
                        intGroupID = rpmObject.GetGroupID(intBrokerID, userGroup)

                        If intGroupID > 0 Then
                            intPriority = rpmObject.GetGroupAvgPriority(CStr(intGroupID))
                        Else
                            intPriority = rpmObject.GetBrokerAvgPriority(intBrokerID)
                        End If

                        queueLength = qmObject.QueueLength(intPriority)

                        weOutput.effectiveQueueLength = queueLength.QueueLength()
                        weOutput.estWait = queueLength.EstTimeToRun()
                    Catch
                        'method error case
                        rmObject.LogIncomingWebRequest(intBrokerID, "GetEffectiveQueueLength", blnHasPermission, False, "Method Error: " & Err.Description() & ": " & Err.Source())
                        Throw New Protocols.SoapException("Method Error: " & Err.Description() & ": " & Err.Source(), Protocols.SoapException.ServerFaultCode)
                        'Return weOutput
                        Exit Function
                    End Try
                    'success case
                    rmObject.LogIncomingWebRequest(intBrokerID, "GetEffectiveQueueLength", blnHasPermission, True, "Completed Successfully.")
                Else
                    'case where interface is disabled by admin.
                    Throw New Protocols.SoapException("This interface is temporarily unavailable.  Please try again later.", Protocols.SoapException.ServerFaultCode)
                    Exit Function
                End If
            Else
                'insufficient permission case
                rmObject.LogIncomingWebRequest(intBrokerID, "GetEffectiveQueueLength", blnHasPermission, False, "Insufficient permission for method execution.")
                Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                Exit Function

            End If
        Else
            'insufficient permission case
            rmObject.LogIncomingWebRequest(intBrokerID, "GetEffectiveQueueLength", blnHasPermission, False, "Insufficient permission for method execution.")
            Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
            Exit Function
        End If

        Return weOutput

    End Function

    <WebMethod(Description:="WebLab Lab Server Method: Returns a URL where information about that lab may be found."), SoapHeader("BrokerCred", Direction:=SoapHeaderDirection.In), SoapDocumentMethod(RequestNamespace:="http://ilab.mit.edu")> _
    Public Function GetLabInfo() As String
        'This method returns a pre-defined URL which points to information on the lab server.  For current version, this will be hardcoded.  
        'For full release, this should be part of system config.
        Dim rmObject As RecordManager = New RecordManager()
        Dim intBrokerID As Integer
        Dim blnHasPermission As Boolean = False

        'attempt to read AuthHeader object from SOAP header
        If Not BrokerCred Is Nothing Then
            Try
                intBrokerID = AuthenticateCaller(BrokerCred.identifier, BrokerCred.passKey)
                BrokerCred.DidUnderstand = True
            Catch
                BrokerCred.DidUnderstand = False
                intBrokerID = -1
            End Try

            'once identity is determined, check caller is authorized to use this interface
            blnHasPermission = AuthorizeBroker(intBrokerID, "canview")


            If blnHasPermission Then 'has sufficient permission

                If GetInterfaceStatus() Then 'interface is enabled
                    rmObject.LogIncomingWebRequest(intBrokerID, "GetLabInfo", blnHasPermission, True, "Completed Successfully.")
                    Return "http://weblab.mit.edu"
                Else
                    'case where interface is disabled by admin.
                    Throw New Protocols.SoapException("This interface is temporarily unavailable.  Please try again later.", Protocols.SoapException.ServerFaultCode)
                    Exit Function
                End If
            Else
                'insufficient permission case
                rmObject.LogIncomingWebRequest(intBrokerID, "GetLabInfo", blnHasPermission, False, "Insufficient permission for method execution.")
                Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                Exit Function

            End If
        Else
            'insufficient permission case
            rmObject.LogIncomingWebRequest(intBrokerID, "GetLabInfo", blnHasPermission, False, "Insufficient permission for method execution.")
            Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
            Exit Function
        End If
    End Function

    <WebMethod(Description:="WebLab Lab Server Method: Returns the valid XML lab configuration document 'labConfiguration'.  Used by the lab client to determine device availability."), SoapHeader("BrokerCred", Direction:=SoapHeaderDirection.In), SoapDocumentMethod(RequestNamespace:="http://ilab.mit.edu")> _
    Public Function GetLabConfiguration(ByVal userGroup As String) As String
        'This method returns a valid XML lab configuration document.  This document describes the devices available on the lab server for the 
        'specified Group.  
        Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()
        Dim rmObject As RecordManager = New RecordManager()
        Dim intBrokerID, intClassID, intGroupID As Integer
        Dim strXMLOutput As String
        Dim blnHasPermission As Boolean = False

        'attempt to read AuthHeader object from SOAP header
        If Not BrokerCred Is Nothing Then
            Try
                intBrokerID = AuthenticateCaller(BrokerCred.identifier, BrokerCred.passKey)
                BrokerCred.DidUnderstand = True
            Catch
                BrokerCred.DidUnderstand = False
                intBrokerID = -1
            End Try

            'once identity is determined, check caller is authorized to use this interface
            blnHasPermission = AuthorizeBroker(intBrokerID, "canview")


            If blnHasPermission Then 'sufficient permission case

                If GetInterfaceStatus() Then 'interface is enabled
                    Try
                        intGroupID = rpmObject.GetGroupID(intBrokerID, userGroup)

                        'core method body
                        If intGroupID > 0 Then
                            'use group id
                            intClassID = CInt(rpmObject.GroupIsMemberOf(intGroupID, "id"))
                            strXMLOutput = rpmObject.GetLabConfig(intClassID)
                        Else
                            'use broker id
                            intClassID = CInt(rpmObject.BrokerIsMemberOf(intBrokerID, "id"))
                            strXMLOutput = rpmObject.GetLabConfig(intClassID)
                        End If
                    Catch
                        'method error case
                        rmObject.LogIncomingWebRequest(intBrokerID, "GetLabConfiguration", blnHasPermission, False, "Method Error: " & Err.Description() & ": " & Err.Source())
                        'strXMLOutput = Err.Description() & "<>" & Err.Source()
                        Throw New Protocols.SoapException("Method Error: " & Err.Description() & ": " & Err.Source(), Protocols.SoapException.ServerFaultCode)
                        'Return strXMLOutput
                        Exit Function
                    End Try
                    'success case
                    rmObject.LogIncomingWebRequest(intBrokerID, "GetLabConfiguration", blnHasPermission, True, "Completed Successfully.")
                Else
                    'case where interface is disabled by admin.
                    Throw New Protocols.SoapException("This interface is temporarily unavailable.  Please try again later.", Protocols.SoapException.ServerFaultCode)
                    Exit Function
                End If
            Else
                'insufficient permission case
                rmObject.LogIncomingWebRequest(intBrokerID, "GetLabConfiguration", blnHasPermission, False, "Insufficient permission for method execution.")
                Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                'strXMLOutput = ""
                Exit Function
            End If
        Else
            'insufficient permission case
            rmObject.LogIncomingWebRequest(intBrokerID, "GetLabConfiguration", blnHasPermission, False, "Insufficient permission for method execution.")
            Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
            'strXMLOutput = ""
            Exit Function
        End If

        Return strXMLOutput
    End Function

    <WebMethod(Description:="WebLab Lab Server Method: Checks that the submitted experiment specification is valid and executable by the specified caller.  Member of struct ValidationReport are 'accepted' (boolean), 'warningMessages' (string()), 'errorMessage' (string) and 'estRuntime' (double)"), SoapHeader("BrokerCred", Direction:=SoapHeaderDirection.In), SoapDocumentMethod(RequestNamespace:="http://ilab.mit.edu")> _
    Public Function Validate(ByVal experimentSpecification As String, ByVal userGroup As String) As ValidationReport
        'This method validates the submitted experiment specification given the specified user group membership and broker id.  
        Dim vrOutput As ValidationReport = New ValidationReport()
        Dim valObject As ValidationObject
        Dim valEngine As ExperimentValidator = New ExperimentValidator()
        Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()
        Dim rmObject As RecordManager = New RecordManager()
        Dim intBrokerID, intGroupID As Integer
        Dim blnHasPermission As Boolean = False

        'attempt to read AuthHeader object from SOAP header
        If Not BrokerCred Is Nothing Then
            Try
                intBrokerID = AuthenticateCaller(BrokerCred.identifier, BrokerCred.passKey)
                BrokerCred.DidUnderstand = True
            Catch
                BrokerCred.DidUnderstand = False
                intBrokerID = -1
            End Try

            'once identity is determined, check caller is authorized to use this interface
            blnHasPermission = AuthorizeBroker(intBrokerID, "canview")


            If blnHasPermission Then 'caller has sufficient permission

                If GetInterfaceStatus() Then 'interface is enabled
                    Try
                        intGroupID = rpmObject.GetGroupID(intBrokerID, userGroup)

                        If intGroupID > 0 Then
                            'use group id
                            valObject = valEngine.validate(experimentSpecification, intBrokerID, intGroupID, HttpContext.Current.Server.MapPath("/"))
                        Else
                            'use broker id
                            valObject = valEngine.validate(experimentSpecification, intBrokerID, HttpContext.Current.Server.MapPath("/"))
                        End If

                        vrOutput.accepted = valObject.isValid
                        vrOutput.errorMessage = valObject.errorMessage
                        If valObject.isValid Then
                            vrOutput.estRuntime = CDbl(rmObject.ExperimentRuntimeEst(valObject.dataPoints, valObject.deviceProfileID))
                        Else
                            vrOutput.estRuntime = 0
                        End If

                        'If strValReport <> "SUCCESS" Then
                        '    vrOutput.accepted = False
                        '    vrOutput.errorMessage = strValReport
                        '    vrOutput.estRuntime = 0
                        'Else
                        '    vrOutput.accepted = True
                        '    vrOutput.errorMessage = ""
                        '    vrOutput.estRuntime = CDbl(rmObject.ExperimentRuntimeEst(valEngine.getTotalDataPoints(), valEngine.getDeviceProfileID()))
                        'End If

                    Catch
                        'error case
                        rmObject.LogIncomingWebRequest(intBrokerID, "Validate", blnHasPermission, False, "Method Error: " & Err.Description() & ": " & Err.Source())
                        Throw New Protocols.SoapException("Method Error: " & Err.Description() & ": " & Err.Source(), Protocols.SoapException.ServerFaultCode)
                        'Return vrOutput
                        Exit Function
                    End Try
                    'success case
                    rmObject.LogIncomingWebRequest(intBrokerID, "Validate", blnHasPermission, True, "Completed Successfully.")
                Else
                    'case where interface is disabled by admin.
                    Throw New Protocols.SoapException("This interface is temporarily unavailable.  Please try again later.", Protocols.SoapException.ServerFaultCode)
                    Exit Function
                End If
            Else
                'insufficient permission case
                rmObject.LogIncomingWebRequest(intBrokerID, "Validate", blnHasPermission, False, "Insufficient permission for method execution.")
                Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                Exit Function
            End If
        Else
            'insufficient permission case
            rmObject.LogIncomingWebRequest(intBrokerID, "Validate", blnHasPermission, False, "Insufficient permission for method execution.")
            Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
            Exit Function
        End If

        Return vrOutput
    End Function

    <WebMethod(Description:="WebLab Lab Server Method: This method validates an incoming experiment specification, calculates the approriate execution priority and, if no errors are thrown, enters the job into the execution queue.  Members of the struct SubmissionReport are 'vReport' (ValidationReport), 'minTimeToLive' (double) and 'wait' (WaitEstimate)"), SoapHeader("BrokerCred", Direction:=SoapHeaderDirection.In), SoapDocumentMethod(RequestNamespace:="http://ilab.mit.edu")> _
    Public Function Submit(ByVal experimentID As Integer, ByVal experimentSpecification As String, ByVal userGroup As String, ByVal priorityHint As Integer) As SubmissionReport
        'This method validates the submitted experiment specification given the specified user group membership and broker id.  If validation
        'completes successfully, the proper job priority is calculated and the job is submitted to the experiment queue.  
        Dim vrOutput As ValidationReport = New ValidationReport()
        Dim weOutput As WaitEstimate = New WaitEstimate()
        Dim srOutput As SubmissionReport = New SubmissionReport()
        Dim valObject As ValidationObject
        Dim valEngine As ExperimentValidator = New ExperimentValidator()
        Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()
        Dim rmObject As RecordManager = New RecordManager()
        Dim qmObject As QueueManager = New QueueManager()
        Dim queueLength As QueueLengthObject
        Dim intBrokerID, intPriority, intGroupID, intEstRunTime, intExpIDCheck As Integer
        Dim blnUseGroup As Boolean
        Dim blnHasPermission As Boolean = False

        Try
            'attempt to read AuthHeader object from SOAP header
            If Not BrokerCred Is Nothing Then
                Try
                    intBrokerID = AuthenticateCaller(BrokerCred.identifier, BrokerCred.passKey)
                    BrokerCred.DidUnderstand = True
                Catch
                    BrokerCred.DidUnderstand = False
                    intBrokerID = -1
                End Try

                'once identity is determined, check caller is authorized to use this interface
                blnHasPermission = AuthorizeBroker(intBrokerID, "canview")


                If blnHasPermission Then 'caller has sufficient permission

                    If GetInterfaceStatus() Then 'if interface is enabled
                        intExpIDCheck = rmObject.LocalExperimentIDLookup(intBrokerID, experimentID)
                        If intExpIDCheck = -1 Then
                            Try
                                intGroupID = rpmObject.GetGroupID(intBrokerID, userGroup)

                                'validation
                                If intGroupID > 0 Then
                                    'use group id
                                    blnUseGroup = True
                                    valObject = valEngine.validate(experimentSpecification, intBrokerID, intGroupID, HttpContext.Current.Server.MapPath("/"))
                                Else
                                    'use broker id
                                    blnUseGroup = False
                                    valObject = valEngine.validate(experimentSpecification, intBrokerID, HttpContext.Current.Server.MapPath("/"))
                                End If

                                vrOutput.accepted = valObject.isValid
                                vrOutput.errorMessage = valObject.errorMessage

                                If valObject.isValid Then
                                    vrOutput.estRuntime = CDbl(rmObject.ExperimentRuntimeEst(valObject.dataPoints, valObject.deviceProfileID))
                                    srOutput.vReport = vrOutput
                                Else
                                    vrOutput.estRuntime = 0
                                    srOutput.vReport = vrOutput
                                    Return srOutput
                                    Exit Function
                                End If

                                'If strValReport <> "SUCCESS" Then
                                '    vrOutput.accepted = False
                                '    vrOutput.errorMessage = strValReport
                                '    vrOutput.estRuntime = 0

                                '    srOutput.vReport = vrOutput
                                '    Return srOutput
                                '    Exit Function
                                'Else
                                '    vrOutput.accepted = True
                                '    vrOutput.errorMessage = ""
                                '    vrOutput.estRuntime = CDbl(rmObject.ExperimentRuntimeEst(valEngine.getTotalDataPoints(), valEngine.getDeviceProfileID()))

                                '    srOutput.vReport = vrOutput
                                'End If

                                'validation step complete, begin submission
                                If blnUseGroup Then

                                    'priority determination
                                    intPriority = rpmObject.GetGroupDevicePriority(intGroupID, valObject.resourceID)

                                    If intPriority > priorityHint And priorityHint >= -20 Then
                                        intPriority = priorityHint
                                    End If

                                    'determine estimated Runtime of job
                                    intEstRunTime = rmObject.ExperimentRuntimeEst(valObject.dataPoints, valObject.deviceProfileID)

                                    'determine wait estimate
                                    queueLength = qmObject.QueueLength(intPriority)

                                    weOutput.effectiveQueueLength = queueLength.QueueLength()
                                    weOutput.estWait = queueLength.EstTimeToRun()

                                    'submit job
                                    Dim intSubmitReturn As Integer = qmObject.EnqueueJob(experimentSpecification, intPriority, userGroup, intBrokerID, experimentID, intEstRunTime, valObject.dataPoints, valObject.deviceProfileID)

                                    srOutput.minTimeToLive = CDbl(43200)       'may want to make this a admin-definable setting
                                    'weOutput.effectiveQueueLength = intSubmitReturn  'want to maintain previoudly written value
                                    srOutput.wait = weOutput

                                    'Return srOutput
                                    'Exit Function

                                Else

                                    'priority determination
                                    intPriority = rpmObject.GetBrokerDevicePriority(intBrokerID, valObject.resourceID)

                                    If intPriority > priorityHint And priorityHint >= -20 Then
                                        intPriority = priorityHint
                                    End If

                                    'determine estimated Runtime of job
                                    intEstRunTime = rmObject.ExperimentRuntimeEst(valObject.dataPoints, valObject.deviceProfileID)

                                    'determine wait estimate
                                    queueLength = qmObject.QueueLength(intPriority)

                                    weOutput.effectiveQueueLength = queueLength.QueueLength()
                                    weOutput.estWait = queueLength.EstTimeToRun()

                                    'submit job
                                    Dim intSubmitReturn As Integer = qmObject.EnqueueJob(experimentSpecification, intPriority, "", intBrokerID, experimentID, intEstRunTime, valObject.dataPoints, valObject.deviceProfileID)

                                    srOutput.minTimeToLive = CDbl(43200)       'may want to make this a admin-definable setting
                                    'weOutput.effectiveQueueLength = intSubmitReturn   'want to maintain previously written value
                                    srOutput.wait = weOutput

                                    'Return srOutput
                                    'Exit Function

                                End If
                            Catch
                                'error case
                                rmObject.LogIncomingWebRequest(intBrokerID, "Submit", blnHasPermission, False, "Method Error: " & Err.Description() & ": " & Err.Source())
                                Throw New Protocols.SoapException("Method Error: " & Err.Description() & ": " & Err.Source(), Protocols.SoapException.ServerFaultCode)
                                'Return srOutput
                                Exit Function
                            End Try
                            'success case
                            rmObject.LogIncomingWebRequest(intBrokerID, "Submit", blnHasPermission, True, "Completed Successfully.")
                        Else
                            'broker id/experimentid pair not unique
                            rmObject.LogIncomingWebRequest(intBrokerID, "Submit", blnHasPermission, False, "Supplied experimentID has already been used.")
                            Throw New Protocols.SoapException("Supplied experimentID has already been used.", Protocols.SoapException.ClientFaultCode)
                            Exit Function
                        End If

                    Else
                        'case where interface is disabled by admin.
                        Throw New Protocols.SoapException("This interface is temporarily unavailable.  Please try again later.", Protocols.SoapException.ServerFaultCode)
                        Exit Function
                    End If
                Else
                    'insufficient permission case
                    rmObject.LogIncomingWebRequest(intBrokerID, "Submit", blnHasPermission, False, "Insufficient permission for method execution.")
                    Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                    Exit Function
                End If
            Else
                'insufficient permission case
                rmObject.LogIncomingWebRequest(intBrokerID, "Submit", blnHasPermission, False, "Insufficient permission for method execution.")
                Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                Exit Function
            End If

            'temp
        Catch
            'error case
            rmObject.LogIncomingWebRequest(intBrokerID, "Submit", blnHasPermission, False, "Method Error: " & Err.Description() & ": " & Err.Source())
            Throw New Protocols.SoapException("Method Error: " & Err.Description() & ": " & Err.Source(), Protocols.SoapException.ServerFaultCode)
            'Return srOutput
            Exit Function
        End Try

        Return srOutput
    End Function

    <WebMethod(Description:="WebLab Lab Server Method: This method cancels the specified spubmitted experiment.  A single boolean return value indicates whether or not the cancel operation completed successfully."), SoapHeader("BrokerCred", Direction:=SoapHeaderDirection.In), SoapDocumentMethod(RequestNamespace:="http://ilab.mit.edu")> _
    Public Function Cancel(ByVal experimentID As Integer) As Boolean
        'This method allows the caller to attempt to cancel a previously submitted job.  Cancel will only succeed if the caller is from the same 
        'service broker that submitted the job and the job has not yet been dequeued.  
        Dim qmObject As QueueManager = New QueueManager()
        Dim rmObject As RecordManager = New RecordManager()
        Dim intBrokerID As Integer
        Dim expStat As ExpStatusObject
        Dim blnOutput As Boolean = False
        Dim blnHasPermission As Boolean = False

        'attempt to read AuthHeader object from SOAP header
        If Not BrokerCred Is Nothing Then
            Try
                intBrokerID = AuthenticateCaller(BrokerCred.identifier, BrokerCred.passKey)
                BrokerCred.DidUnderstand = True
            Catch
                BrokerCred.DidUnderstand = False
                intBrokerID = -1
            End Try

            'once identity is determined, check caller is authorized to use this interface
            blnHasPermission = AuthorizeBroker(intBrokerID, "canview")


            If blnHasPermission Then 'caller has sufficient permission

                If GetInterfaceStatus() Then 'if interface is active
                    Try
                        expStat = qmObject.ExperimentStatus(intBrokerID, experimentID)

                        If expStat.QueuePosition() < 1 Then
                            blnOutput = False
                            'Exit Function
                        Else
                            blnOutput = qmObject.CancelJob(intBrokerID, experimentID)
                            'Return blnOutput
                        End If
                    Catch
                        'error case
                        rmObject.LogIncomingWebRequest(intBrokerID, "Cancel", blnHasPermission, False, "Method Error: " & Err.Description() & ": " & Err.Source())
                        Throw New Protocols.SoapException("Method Error: " & Err.Description() & ": " & Err.Source(), Protocols.SoapException.ServerFaultCode)
                        'Return blnOutput
                        Exit Function
                    End Try
                    'success case
                    rmObject.LogIncomingWebRequest(intBrokerID, "Cancel", blnHasPermission, True, "Completed Successfully.")
                Else
                    'case where interface is disabled by admin.
                    Throw New Protocols.SoapException("This interface is temporarily unavailable.  Please try again later.", Protocols.SoapException.ServerFaultCode)
                    Exit Function
                End If

            Else
                'insufficient permission case
                rmObject.LogIncomingWebRequest(intBrokerID, "Cancel", blnHasPermission, False, "Insufficient permission for method execution.")
                Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                Exit Function
            End If
        Else
            'insufficient permission case
            rmObject.LogIncomingWebRequest(intBrokerID, "Cancel", blnHasPermission, False, "Insufficient permission for method execution.")
            Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
            Exit Function
        End If

        Return blnOutput
    End Function

    <WebMethod(Description:="WebLab Lab Server Method: This method returns the status of a previously submitted experiment.  Members of the struct LabExperimentStatus are 'statusReport' (struct ExperimentStatus) and 'minTimeToLive' (double).  Members of the struct ExperimentStatus are 'statusCode' (int), 'wait' (struct waitEstimate), 'estRuntime' (double) and 'estRemainingRuntime' (double)."), SoapHeader("BrokerCred", Direction:=SoapHeaderDirection.In), SoapDocumentMethod(RequestNamespace:="http://ilab.mit.edu")> _
    Public Function GetExperimentStatus(ByVal experimentID As Integer) As LabExperimentStatus
        'This method returns the status of the specified experiment.  Data will only be returned if the specified job was submitted by the same
        'service broker as the caller. 
        Dim lesOutput As LabExperimentStatus = New LabExperimentStatus()
        Dim esOutput As ExperimentStatus = New ExperimentStatus()
        Dim weOutput As WaitEstimate = New WaitEstimate()
        Dim qmObject As QueueManager = New QueueManager()
        Dim rmObject As RecordManager = New RecordManager()
        Dim expStatus As ExpStatusObject
        Dim intBrokerID, intExpStatCode As Integer
        Dim blnHasPermission As Boolean = False

        'attempt to read AuthHeader object from SOAP header
        If Not BrokerCred Is Nothing Then
            Try
                intBrokerID = AuthenticateCaller(BrokerCred.identifier, BrokerCred.passKey)
                BrokerCred.DidUnderstand = True
            Catch
                BrokerCred.DidUnderstand = False
                intBrokerID = -1
            End Try

            'once identity is determined, check caller is authorized to use this interface
            blnHasPermission = AuthorizeBroker(intBrokerID, "canview")


            If blnHasPermission Then 'caller has sufficient permission

                If GetInterfaceStatus() Then 'if interface is active
                    Try
                        intExpStatCode = qmObject.GetExperimentStatusCode(intBrokerID, experimentID)

                        If intExpStatCode = 6 Then  'case where expID/brokerID combo is invalid.
                            esOutput.statusCode = intExpStatCode
                            esOutput.estRemainingRuntime = 0
                            esOutput.wait = weOutput
                            lesOutput.statusReport = esOutput
                            lesOutput.minTimeToLive = 43200

                            'success case
                            rmObject.LogIncomingWebRequest(intBrokerID, "GetExperimentStatus", blnHasPermission, True, "Completed Successfully: Specified job is invalid. (BrokerID = " & intBrokerID & ", RemoteID = " & experimentID & ")")

                            Return lesOutput
                            Exit Function
                        End If

                        esOutput.statusCode = intExpStatCode

                        expStatus = qmObject.ExperimentStatus(intBrokerID, experimentID)
                        weOutput.effectiveQueueLength = expStatus.QueuePosition()
                        weOutput.estWait = expStatus.EstTimeToRun
                        esOutput.estRuntime = expStatus.EstExecTime()
                        esOutput.estRemainingRuntime = expStatus.EstRemainExecTime()
                        esOutput.wait = weOutput
                        lesOutput.minTimeToLive = 43200
                        lesOutput.statusReport = esOutput
                    Catch
                        'error case
                        rmObject.LogIncomingWebRequest(intBrokerID, "GetExperimentStatus", blnHasPermission, False, "Method Error: " & Err.Description() & ": " & Err.Source())
                        Throw New Protocols.SoapException("Method Error: " & Err.Description() & ": " & Err.Source(), Protocols.SoapException.ServerFaultCode)
                        'Return lesOutput
                        Exit Function
                    End Try
                    'success case
                    rmObject.LogIncomingWebRequest(intBrokerID, "GetExperimentStatus", blnHasPermission, True, "Completed Successfully.")
                Else
                    'case where interface is disabled by admin.
                    Throw New Protocols.SoapException("This interface is temporarily unavailable.  Please try again later.", Protocols.SoapException.ServerFaultCode)
                    Exit Function

                End If
            Else
                'insufficient permission case
                rmObject.LogIncomingWebRequest(intBrokerID, "GetExperimentStatus", blnHasPermission, False, "Insufficient permission for method execution.")
                Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                Exit Function
            End If
        Else
            'insufficient permission case
            rmObject.LogIncomingWebRequest(intBrokerID, "GetExperimentStatus", blnHasPermission, False, "Insufficient permission for method execution.")
            Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
            Exit Function
        End If

        Return lesOutput
    End Function

    <WebMethod(Description:="WebLab Lab Server Method: This method returns the results of the specified experiment.  Memebers of the struct ResultReport are 'statusCode' (int), 'experimentResult' (string), 'xmlResultExtension' (string), 'xmlBlobExtension' (string), 'warningMessages' (string()) and 'errorMessage' (string)."), SoapHeader("BrokerCred", Direction:=SoapHeaderDirection.In), SoapDocumentMethod(RequestNamespace:="http://ilab.mit.edu")> _
    Public Function RetrieveResult(ByVal experimentID As Integer) As ResultReport
        'This method returns the results of the specified experiment.  Data will only be returned if the specified job was submitted by the same
        'service broker as the caller. 
        Dim rrOutput As ResultReport = New ResultReport()
        Dim rmObject As RecordManager = New RecordManager()
        Dim qmObject As QueueManager = New QueueManager()
        Dim resultObject As ResultObject
        Dim eriObject As ExpRecordInfoObject
        Dim intBrokerID, intExpStatCode As Integer
        Dim strResultExtension As String
        Dim blnHasPermission As Boolean = False

        intBrokerID = -1

        'attempt to read AuthHeader object from SOAP header
        If Not BrokerCred Is Nothing Then
            Try
                intBrokerID = AuthenticateCaller(BrokerCred.identifier, BrokerCred.passKey)
                BrokerCred.DidUnderstand = True
            Catch
                BrokerCred.DidUnderstand = False
                'intBrokerID = -1
            End Try

            'once identity is determined, check caller is authorized to use this interface
            blnHasPermission = AuthorizeBroker(intBrokerID, "canview")


            If blnHasPermission Then 'caller has sufficient permission

                If GetInterfaceStatus() Then 'if interface is active
                    Try
                        intExpStatCode = qmObject.GetExperimentStatusCode(intBrokerID, experimentID)

                        If intExpStatCode <> 3 And intExpStatCode <> 4 And intExpStatCode <> 5 Then  'case where expID/brokerID combo is invalid or job is still in the queue.
                            rrOutput.statusCode = intExpStatCode

                            'success case
                            rmObject.LogIncomingWebRequest(intBrokerID, "RetrieveResult", blnHasPermission, True, "Completed Successfully: Job invalid or not yet executed. (BrokerID = " & intBrokerID & ", RemoteID = " & experimentID & ")")

                            Return rrOutput
                            Exit Function
                        End If


                        resultObject = rmObject.RetrieveResult(intBrokerID, experimentID)
                        eriObject = rmObject.GetExperimentRecordInfo(intBrokerID, experimentID)

                        rrOutput.statusCode = resultObject.ExperimentStatus()
                        rrOutput.experimentResults = resultObject.ExperimentResults()
                        'rrOutput.labConfiguration = resultObject.LabConfig()  - lab configuration removed from ResultReport object (2/16/05)

                        Dim strWarning() As String = Split(resultObject.WarningMessages(), ";;")
                        rrOutput.warningMessages = strWarning

                        rrOutput.errorMessage = resultObject.ErrorMessages()

                        If eriObject.SubmitTime() <> "" Then 'checks submit time field for data
                            strResultExtension = "Group=" & eriObject.UserGroup()
                            strResultExtension = strResultExtension & ",SubmitTime=" & eriObject.SubmitTime()
                            strResultExtension = strResultExtension & ",ExecutionTime=" & eriObject.ExecTime()
                            strResultExtension = strResultExtension & ",EndTime=" & eriObject.EndTime()
                            strResultExtension = strResultExtension & ",ElapsedExecutionTime=" & eriObject.ExecElapsed()
                            strResultExtension = strResultExtension & ",ElapsedJobTime=" & eriObject.JobElapsed()
                            strResultExtension = strResultExtension & ",DeviceName=" & eriObject.DeviceName()

                            rrOutput.xmlResultExtension = strResultExtension
                        End If
                    Catch e As Exception
                        'error case
                        rmObject.LogIncomingWebRequest(intBrokerID, "RetrieveResult", blnHasPermission, False, "Method Error: " & e.Message() & ": " & e.Source())
                        Throw New Protocols.SoapException("Method Error: " & e.Message() & ": " & e.Source(), Protocols.SoapException.ServerFaultCode)
                        'Return rrOutput
                        Exit Function
                        'rrOutput.executionErrorMessage = Err.Description() & "<>" & Err.Source()
                    End Try
                    'success case
                    rmObject.LogIncomingWebRequest(intBrokerID, "RetrieveResult", blnHasPermission, True, "Completed Successfully.")
                Else
                    'case where interface is disabled by admin.
                    Throw New Protocols.SoapException("This interface is temporarily unavailable.  Please try again later.", Protocols.SoapException.ServerFaultCode)
                    Exit Function

                End If
            Else
                'insufficient permission case
                rmObject.LogIncomingWebRequest(intBrokerID, "RetrieveResult", blnHasPermission, False, "Insufficient permission for method execution.")
                Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
                Exit Function
            End If
        Else
            'insufficient permission case
            rmObject.LogIncomingWebRequest(intBrokerID, "RetrieveResult", blnHasPermission, False, "Insufficient permission for method execution.")
            Throw New Protocols.SoapException("Insufficient permission for method execution.", Protocols.SoapException.ClientFaultCode)
            Exit Function
        End If

        Return rrOutput
    End Function


    '=========================================>SUPPORTING FUNCTIONS<======================================

    Private Function GetInterfaceStatus() As Boolean
        'This method checks the System Configuration record to determine the current activity status of this interface.  Caller methods
        'should complete execution only if the output of this method is True.
        Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()

        Return rpmObject.GetWSInterfaceStatus()
    End Function

    Private Function AuthenticateCaller(ByVal strIdentifier As String, ByVal strPassKey As String) As Integer
        'This method checks a caller's Authentication Header information against locally stored Service Broker records.  The inputs to this
        'method are the supplied Server ID and passkey.  If the credentials are valid, a value of True is returned.
        Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()

        Return rpmObject.AuthenticateBroker(strIdentifier, strPassKey)
    End Function

    Private Function AuthorizeBroker(ByVal intBrokerID As Integer, ByVal strAccessLevel As String) As Boolean
        'This method checks that the specified, authenticated Broker has the specified permission on this interface.  WS Interface use only 
        'requires Read access on the "WSInterface" resource.  If the specified Broker has the specified leve of permission on this interface,
        'a value of True is returned.  Valid values for strAccessLevel are as follows:
        '
        '"canview" - Read Permission
        '"canedit" - Edit Permission
        '"cangrant" - Grant Permission
        '"candelete" - Delete Permission
        Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()

        Return rpmObject.GetBrokerResourcePermission(intBrokerID, "WSInterface", strAccessLevel)
    End Function

    Private Function AuthorizeGroup(ByVal intGroupID As Integer, ByVal strAccessLevel As String) As Boolean
        'This method checks that the specified, authenticated Broker Group has the specified permission on this interface.  WS Interface 
        'use only requires Read access on the "WSInterface" resource.  If the specifired Group has the specified level of permission on this
        'interface, a value of True is returned.  Valid values for strAccessLevel are as follows:
        '
        '"canview" - Read Permission
        '"canedit" - Edit Permission
        '"cangrant" - Grant Permission
        '"candelete" - Delete Permission
        Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()

        Return rpmObject.GetGroupResourcePermission(intGroupID, "WSInterface", strAccessLevel)
    End Function

End Class
