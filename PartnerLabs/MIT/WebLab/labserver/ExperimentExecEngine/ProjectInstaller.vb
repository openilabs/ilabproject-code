Imports System.ComponentModel
Imports System.Configuration.Install

'Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
'Please see license.txt in top level directory for full license. 

<RunInstaller(True)> Public Class ProjectInstaller
    Inherits System.Configuration.Install.Installer

#Region " Component Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Installer overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents ServiceProcessInstaller1 As System.ServiceProcess.ServiceProcessInstaller
    Friend WithEvents ServiceInstaller1 As System.ServiceProcess.ServiceInstaller
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.ServiceProcessInstaller1 = New System.ServiceProcess.ServiceProcessInstaller()
        Me.ServiceInstaller1 = New System.ServiceProcess.ServiceInstaller()
        '
        'ServiceProcessInstaller1
        '
        Me.ServiceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem
        Me.ServiceProcessInstaller1.Password = Nothing
        Me.ServiceProcessInstaller1.Username = Nothing
        '
        'ServiceInstaller1
        '
        Me.ServiceInstaller1.ServiceName = "ExpExecEngine"
        Me.ServiceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic
        '
        'ProjectInstaller
        '
        Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.ServiceProcessInstaller1, Me.ServiceInstaller1})

    End Sub

#End Region

    Public Overrides Sub Install(ByVal stateServer As IDictionary)
        'wrapper for base Install function.  Used to add custom functionality to install procedure.

        Dim strDesc As String

        'set variables for registry info editing procedure
        Dim system, currentControlSet, services, service, config As Microsoft.Win32.RegistryKey

        'service description
        strDesc = "Experiment Execution Service for the Microelectronics WebLab Lab Server.  This service governs experiment execution for the Lab Server system."

        Try
            MyBase.Install(stateServer)
            'run base Install function



            'BEGIN procedure for editing service info in registry
            'open the HKEY_LOCAL_MACHINE\SYSTEM key
            system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System")

            'Open CurrentControlSet key
            currentControlSet = system.OpenSubKey("CurrentControlSet")

            'Open Services key
            services = currentControlSet.OpenSubKey("Services")

            'Open the key for the installed service for writing
            service = services.OpenSubKey(Me.ServiceInstaller1.ServiceName, True)

            'Add the installed service's description as a REG_SZ value named "Description"
            service.SetValue("Description", strDesc)

            'add service dependency on MSSQLSERVER (to make sure SQL server is running before this process at machine startup)
            service.SetValue("DependOnService", "MSSQLSERVER")




            'can optionally add custom information keys for service use
            'config = service.CreateSubKey("Parameters")
            'config.SetValue("UserName", MyBase.Context.Parameters.Keys.Count())
            Debug.WriteLine(MyBase.Context.Parameters.Count())

            'END procedure for editing service info in registry

        Catch e As Exception
            Console.Write("An exception was thrown during service installation:\n" & e.ToString())
            'where does this dump to in practice?

            Me.Rollback(stateServer) 'rolls back the install program
        End Try
    End Sub

End Class
