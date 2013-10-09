<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Web.Mail" %>

<Script Runat="Server">
'Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
'Please see license.txt in top level directory for full license. 

Sub Session_Start()
	If User.Identity.IsAuthenticated
		Response.redirect("/index.aspx")
	end if
End Sub

Sub Application_Start()
	Dim strDBQuery As String
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	
	Dim conWebLabLS As SqlConnection = new SqlConnection(ConfigurationSettings.AppSettings("conString"))
	conWebLabLS.Open()
	
	strDBQuery = "SELECT s.HP4155_ID, s.HPE5250A_ID, s.HPE5250A_present, s.HP34970A_ID, s.HP34970A_present, s.HP34970A_chan, s.VISA_Name, s.homepage, s.ws_int_is_active, s.exp_eng_is_active, s.lab_server_id, u.first_name, u.last_name, u.email FROM LSSystemConfig s JOIN SiteUsers u ON s.Admin_ID = u.user_id;"
	cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
	dtrDBQuery = cmdDBQuery.ExecuteReader()

	If dtrDBQuery.Read() Then
		application("HP4155_ID") = dtrDBQuery("HP4155_ID")
		application("HPE5250A_present") = dtrDBQuery("HPE5250A_present")
		application("HPE5250A_ID") = dtrDBQuery("HPE5250A_ID")
		application("HP34970A_present") = dtrDBQuery("HP34970A_present")
		application("HP34970A_ID") = dtrDBQuery("HP34970A_ID")
		application("HP34970A_chan") = dtrDBQuery("HP34970A_chan")
		application("VISA_Name") = dtrDBQuery("VISA_Name")
		application("Admin_Name") = trim(dtrDBQuery("first_name")) & " " & trim(dtrDBQuery("last_name"))
		application("Admin_email") = dtrDBQuery("email")
		application("homepage") = dtrDBQuery("homepage")
		application("ws_int_is_active") = dtrDBQuery("ws_int_is_active")
		application("exp_eng_is_active") = dtrDBQuery("exp_eng_is_active")
		application("lab_server_id") = dtrDBQuery("lab_server_id")
	End If
	
	dtrDBQuery.Close()
	conWebLabLS.Close()

End Sub

'Sub Application_Error
	' top level error handler
	' any error that occurs on this site that is not caught by a lower level handler
	' will be caught by this method. This method sends an email about the error
	' to the site's technical team.

'	Dim mailObj AS new MailMessage
'		
	' Set the from and to address on the email
'	mailObj.From = "errorhandler@teampsycho.com"
	'mailObj.To = "info@eyearchitect.com,hardison@mit.edu,saunders@mit.edu"
'	mailObj.To = "saunders@mit.edu"

'	mailObj.Subject = "Error in page at teampsycho.com"
	
'	mailObj.Body = ""
	
'	mailObj.Body &= "The page requested was:" & vbNewLine
'	mailObj.Body &= Request.Path & vbNewLine & vbNewLine
	
'	mailObj.Body &= "Time of error: " & DateTime.Now.ToString("f") & vbNewLine & vbNewLine
	
'	mailObj.Body &= "The query string was:" & vbNewLine
'	If len(trim(Request.QueryString.ToString)) > 0
'		mailObj.Body &= Request.QueryString.ToString & vbNewLine & vbNewLine
'	Else
'		mailObj.Body &= "no query string" & vbNewLine & vbNewLine
'	End If
	
'	mailObj.Body &= "The form variables were:" & vbNewLine
'	If len(trim(Request.Form.ToString)) > 0
'		mailObj.Body &= Request.Form.ToString & vbNewLine & vbNewLine
'	Else
'		mailObj.Body &= "no form variables" & vbNewLine & vbNewLine
'	End If
	
	' the following statements have been commented out because the
	' information they provide is completely useless. The error messages are too general--they
	' do not describe the specific error that happened. There is probably a different
	' way to access the correct error information, but we don't currently know how to
	' do any better than the code below.
	
	'mailObj.Body &= "The error message was:" & vbNewLine
	'mailObj.Body &= Context.Server.GetLastError.Message & vbNewLine & vbNewLine
	
	'mailObj.Body &= "The error source was:" & vbNewLine
	'mailObj.Body &= Context.Server.GetLastError.Source & vbNewLine & vbNewLine
	
	'mailObj.Body &= "The error stack trace was:" & vbNewLine
	'mailObj.Body &= Context.Server.GetLastError.StackTrace & vbNewLine & vbNewLine
	
	'mailObj.Body &= "The error stack target site (the method in which the error occured) was:" & vbNewLine
	'mailObj.Body &= Context.Server.GetLastError.TargetSite.ToString & vbNewLine & vbNewLine
	
'	SmtpMail.SmtpServer = ConfigurationSettings.AppSettings("TeamPsychoMailServer")
'	SmtpMail.Send(mailObj)
'	Context.Server.ClearError
	
'	Response.Redirect("/error.html")
'End Sub

'Sub Application_EndRequest
	' at the end of each request of a logged on user, update their last_visited timestamp
	' this timestamp is used for the whos-online utility
	' If the site ever begins to receive large amounts of traffic, you will probably
	' want to either remove this functionality, or look at redoing it in a more efficient
	' way that does not generate a database hit on every page request. The whos-online
	' function is only available to admins and is not at all essential to the successful
	' operation of the site.
'	If Not Request.Cookies("WhosOnUserID") Is Nothing
		' update last_visited timestamp for distributed problem set
'		UpdateLastVisitedTimestamp(Request.Cookies("WhosOnUserID").value)
'	End If
'End Sub

'Sub UpdateLastVisitedTimestamp(TheEmail As String)
'	Dim theConnection As SqlConnection
'	Dim strUpdate As String
'	Dim cmdUpdate AS SqlCommand
'	Dim strConString As String

'	strConString = ConfigurationSettings.AppSettings("connectionString")
'	theConnection = new SqlConnection(strConString)
	
'	strUpdate = "UPDATE AllUsers SET last_visit = getdate() WHERE UserEmail = @UserEmail"

'	cmdUpdate = New SqlCommand(strUpdate, theConnection)
'	cmdUpdate.Parameters.Add( "@UserEmail", TheEmail )
	
'	theConnection.Open()
'	cmdUpdate.ExecuteNonQuery()
'	theConnection.Close()
'End Sub

</SCRIPT>

