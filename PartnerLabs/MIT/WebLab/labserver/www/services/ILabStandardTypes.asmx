<!--
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license.
-->

<%@ WebService Class="ILabStandardTypes" %>
Imports System
Imports System.Web.Services
Imports Microsoft.VisualBasic

<WebService(Description:="ILab Standard Types Definition", Namespace:="http://ilab.mit.edu")> _
Public Class ILabStandardTypes

    Public Structure LabStatus
        Public online As Boolean
        Public labStatusMessage As String
    End Structure

    <WebMethod()> Public Function EatLabStatus(ByVal labStatusObject As LabStatus)
       
    End Function


    Public Structure WaitEstimate
        Public effectiveQueueLength As Integer
        Public estWait As Double
    End Structure

    <WebMethod()> Public Function EatWaitEstimate(ByVal waitEstimateObject As WaitEstimate)

    End Function




    Public Structure ValidationReport
        Public accepted As Boolean
        Public warningMessages As String()
        Public errorMessage As String
        Public estRuntime As Double
    End Structure

    <WebMethod()> Public Function EatValidationReport(ByVal validationReportObject As ValidationReport)

    End Function

   ' Public Structure SubmissionReport
   '     Public vReport As ValidationReport
   '     Public minTimeToLive As Double
   '     Public wait As WaitEstimate
   ' End Structure

    '<WebMethod()> Public Function EatSubmissionReport(ByVal submissionReportObject As SubmissionReport)

    'End Function

    Public Structure ExperimentStatus
        Public statusCode As Integer
        Public wait As WaitEstimate
        Public estRuntime As Double
        Public estRemainingRuntime As Double
    End Structure

    <WebMethod()> Public Function EatExperimentStatus(ByVal experimentStatusObject As ExperimentStatus)

    End Function

    Public Structure LabExperimentStatus
        Public statusReport As ExperimentStatus
        Public minTimeToLive As Double
    End Structure

    <WebMethod()> Public Function EatLabExperimentStatus(ByVal labExperimentStatusObject As LabExperimentStatus)

    End Function

    Public Structure ResultReport
        Public statusCode As Integer
        Public experimentResults As String
        Public xmlResultExtension As String
        Public xmlBlobExtension As String
        Public warningMessages As String()
        Public errorMessage As String
    End Structure

    <WebMethod()> Public Function EatResultReport(ByVal resultReportObject As ResultReport)

    End Function
        
    Public Structure Experiment
		Public information As ExperimentInformation
		Public labConfiguration As String
		Public experimentSpecification As String
		Public experimentResults As String
	End Structure
	
	<WebMethod()> Public Function EatExperiment(ByVal experimentObject As Experiment)

    End Function
    
	Public Structure ExperimentInformation
		Public experimentID As Integer
		Public labServerID As String
		Public userID As String
		Public effectiveGroupName As String
		Public submissionTime As DateTime
		Public completionTime As DateTime
		Public expirationTime As DateTime
		Public minTimeToLive As Double
		Public priorityHint As Integer
		Public statusCode As Integer
		Public validationWarningMessages As String()
		Public validationErrorMessage As String
		Public executionWarningMessages As String()
		Public executionErrorMessage As String
		Public annotation As String
		Public xmlResultExtension As String
		Public xmlBlobExtension As String
	End Structure
	
	<WebMethod()> Public Function EatExperimentInformation(ByVal experimentInformationObject As ExperimentInformation)

    End Function
	
	Public Structure SubmissionReport
		Public vReport As ValidationReport
		Public experimentID As Integer
		Public minTimeToLive As Double
		Public wait As WaitEstimate
	End Structure
	
	<WebMethod()> Public Function EatSubmissionReport(ByVal submissionReportObject As SubmissionReport)

    End Function


End Class

