<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLabDataManagers" %>

<!-- 
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->

<!--
Author(s): James Hardison (hardison@alum.mit.edu)
Date: 5/9/2003
This page serves as an interface between the Experiment Engine and the Notify service 
supplied by the Service Broker associated with the input experiment ID.  Specifically,
this page receives an experiment ID via the querystring variable "expID".  This expID
will be cross referenced with the database and checked for validity and that the job
is complete.  Ownership of the job is then determined and an attempt is made to call
the "Notify" web service method of the appropriate Service Broker, if available.  

-->
<script Runat="server">
Dim strExpID, strBrokerExpID, strConString, strDBQuery, strReturn, strMethodUrl, strBrokerID as String
Dim conWebLabLS as SqlConnection
Dim cmdDBQuery as SqlCommand
Dim dtrDBQuery As SqlDataReader
Dim rmObject As RecordManager = New RecordManager()

Sub Page_Load
	strExpID = Request.QueryString("expID")
	If strExpID = "" then
		Exit Sub
	End If
	
	'strConString = ConfigurationSettings.AppSettings("conString")
	'conWebLabLS = New SqlConnection(strConString)
	
	'conWebLabLS.Open()
	'checks if the submitted id is valid and the associated broker has a service broker service interface location on file	
	'strDBQuery = "SELECT b.notify_location, b.broker_id, r.broker_assigned_id FROM JobRecord r JOIN Brokers b ON b.broker_id = r.provider_id WHERE (r.job_status = 'COMPLETE' OR r.job_status = 'CANCELLED') AND r.exp_id = @ExpID;"
	'cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
	'cmdDBQuery.Parameters.Add("@ExpID", strExpID)
	
	'dtrDBQuery = cmdDBQuery.ExecuteReader()
	
	strMethodUrl = ""
	
	'If dtrDBQuery.Read() Then
	'	strMethodUrl = dtrDBQuery("notify_location")
	'	strBrokerID = dtrDBQuery("broker_id")
'		strBrokerExpID = dtrDBQuery("broker_assigned_id")
	'End If
	
	'dtrDBQuery.Close()
	
	'strReturn = "Entering exec block"
	'we only issue notify if we have a method location on file
	If Not strMethodUrl = "" Then
		
		'strReturn = "Entered Exec block"
		'instantiate the service broker service class
		Dim SBObj as New ServiceBrokerService()
		
		'set the appropriate URL for this job
		SBObj.URL = "http://ilabsdev.mit.edu/services/servicebrokerservice.aspx"
		
	
		'call notify
		Try
			SBObj.Notify(strExpID)
			
			'
		Catch e As Exception
			strReturn = "An error occurred: " & e.Message() & "<p>" & e.Source() 
			
		End Try
	End if
	
	
End Sub




</script>




<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title></title>
  
  </head>
  <body>
<%=strReturn%>
  </body>
</html>
