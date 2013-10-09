Imports System
Imports System.Web.Services
Imports Microsoft.VisualBasic

'Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
'Please see license.txt in top level directory for full license. 

<WebService(Description:="ILab Standard Types Definition", Namespace:="http://i-lab.mit.edu")> _
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

    Public Structure SubmissionReport
        Public vReport As ValidationReport
        Public labExperimentID As Integer
        Public minTimetoLive As Double
        Public wait As WaitEstimate
    End Structure

    <WebMethod()> Public Function EatSubmissionReport(ByVal submissionReportObject As SubmissionReport)

    End Function

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
        Public minTimetoLive As Double
    End Structure

    <WebMethod()> Public Function EatLabExperimentStatus(ByVal labExperimentStatusObject As LabExperimentStatus)

    End Function

    Public Structure ResultReport
        Public statusCode As Integer
        Public experimentResult As String
        Public xmlResultExtension As String
        Public xmlBlobExtension As String
        Public warningMessages As String()
        Public errorMessage As String
    End Structure

    <WebMethod()> Public Function EatResultReport(ByVal resultReportObject As ResultReport)

    End Function





End Class


