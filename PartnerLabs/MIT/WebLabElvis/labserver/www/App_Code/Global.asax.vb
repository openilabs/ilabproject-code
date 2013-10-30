Imports System.Web
Imports System.Web.SessionState
Imports System.Data
Imports System.Data.SqlClient


Namespace LabServer


Public Class [Global]
    Inherits System.Web.HttpApplication

#Region " Component Designer Generated Code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

#End Region

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
        Dim strDBQuery As String
        Dim cmdDBQuery As SqlCommand
        Dim dtrDBQuery As SqlDataReader

        Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
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
            Response.Redirect("/index.aspx")
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

End Class

End Namespace
