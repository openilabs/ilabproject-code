<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Web.Mail" %>

<Script Runat="Server">

Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
        Dim strDBQuery As String
        Dim cmdDBQuery As SqlCommand
        Dim dtrDBQuery As SqlDataReader

        Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("conString"))
        conWebLabLS.Open()

        strDBQuery = "SELECT s.homepage, s.ws_int_is_active, s.exp_eng_is_active, s.lab_server_id, s.elvis_dev_name, u.first_name, u.last_name, u.email FROM LSSystemConfig s JOIN SiteUsers u ON s.Admin_ID = u.user_id;"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        dtrDBQuery = cmdDBQuery.ExecuteReader()

        If dtrDBQuery.Read() Then
            Application("Admin_Name") = Trim(dtrDBQuery("first_name")) & " " & Trim(dtrDBQuery("last_name"))
            Application("Admin_email") = dtrDBQuery("email")
            Application("homepage") = dtrDBQuery("homepage")
            Application("ws_int_is_active") = dtrDBQuery("ws_int_is_active")
            Application("exp_eng_is_active") = dtrDBQuery("exp_eng_is_active")
            Application("lab_server_id") = dtrDBQuery("lab_server_id")
            Application("device_name") = dtrDBQuery("elvis_dev_name")
        End If

        dtrDBQuery.Close()
        conWebLabLS.Close()
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
        If User.Identity.IsAuthenticated Then
            Response.Redirect("index.aspx")
        End If
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub
    

</Script>
