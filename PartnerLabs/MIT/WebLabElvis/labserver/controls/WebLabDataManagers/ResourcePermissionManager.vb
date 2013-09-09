Imports System
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports Microsoft.VisualBasic
Imports WebLabCustomDataTypes.WebLabCustomDataTypes


'Author(s): James Hardison (hardison@alum.mit.edu)
'Date: 6/3/2003
'Date Modified: 9/21/2004
'This class file contains VB extensions to the RPM methods present in the WebLabServicesLS database.  In particular, the SQL methods are 
'exposed here as a VB.NET component which may be utilized by an ASP.NET page or web service method.  The individual methods will contain
'input validation code and invocation of the appropriate SQL stored procedure.  To use these methods in your ASP.NET page, first, make 
'sure a compiled copy of this file is present in the /bin directory of your web application.  In your ASP.NET page, import the
'WebLabDataManagers namespace (<%import namespace="WebLabDataManagers"%>) and, in your code, instantiate a ResourcePermissionManager object.
'From this object, all of the public methods listed below will be accessible.
'
'Dependency List
'Used By:
'   Site Login Page (/login.aspx)
'   Site Active Setups Managment Page (/admin/active-setups.aspx)
'   Site Setups Management Page (/admin/setups.aspx)
'   Site Job Execution Log (/admin/exec-log.aspx)
'   Site User Login Log (/admin/login-log.aspx)
'   Site Web Service Interface Traffic Log (/admin/wsint-log.aspx)
'   Site Queue Status Page (/admin/queue-stat.aspx)
'   Site Service Broker Management Page (/admin/service-brokers.aspx)
'   Site User Management Page (/admin/site-users.aspx)
'   Lab Server System Configuration Page (/admin/system-config.apsx)
'   Lab Server System Notices Page (/admin/system-notices.aspx)
'   Lab Server System Resource Management Page (/admin/system-resources.aspx)
'   Site Usage Class Management Page (/admin/usage-classes.aspx)
'   Lab Server Experiment Validation Script (/controls/validation_engine/ValidationEngine.vb, /bin/validation_engine.dll)
'   Lab Server Web Services Interface (/services/WebLabServices/LabServerAPI.vb, /bin/WebLabServices.dll)
'   Lab Server Resource Permission Manager Library (/bin/ResourcePermissionManager.dll (this code))
'
'Uses:
'   WebLab Custom Data Types (/controls/WebLabCustomDataTypes/WebLabCustomDataTypes.vb, /bin/WebLabCustomDataTypes.dll)

Namespace WebLabDataManagers

    Public Class ResourcePermissionManager

        Dim conWebLabLS As SqlConnection = New SqlConnection("Database=ELVIS_LS;Server=localhost;Integrated Security=true")

        Public Function AddBroker(ByVal strName As String, ByVal strBrokerServerId As String, ByVal strBrokerPassKey As String, ByVal strServerPassKey As String, ByVal intClassID As Integer, ByVal strDesc As String, ByVal strFirstName As String, ByVal strLastName As String, ByVal strEmail As String, ByVal strNotifyLoc As String, ByVal blnIsActive As Boolean) As NewBrokerConf
            'This method creates a new Service Broker record on the Lab Server.  The inputs comprise the information needed to create a new 
            'service broker record.  These are, in order, the name of the Broker, its authentication Server ID, the passkey generated for the
            'broker, the broker generated passkey for the lab server, the ID of the Usage Class the broker will be assigned to, a text 
            'description of the broker, the first and last names as well as email address of a human contact who administers the Service Broker,
            'the location of the broker's Web Service interface and a boolean value indicating whether it should be active or not.  The output 
            'is a New Broker Confirmation object which contains the broker ID of the new record and any comments returned by the operation.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim nbcObject As NewBrokerConf

            strDBQuery = "EXEC rpm_AddBroker @name, @broker_server_id, @broker_passkey, @server_passkey, @classID, @desc, @firstname, @lastname, @email, @notifyloc, @isactive"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@name", strName)
            cmdDBQuery.Parameters.Add("@broker_server_id", strBrokerServerId)
            cmdDBQuery.Parameters.Add("@broker_passkey", strBrokerPassKey)
            cmdDBQuery.Parameters.Add("@server_passkey", strServerPassKey)
            cmdDBQuery.Parameters.Add("@classID", intClassID)
            cmdDBQuery.Parameters.Add("@desc", strDesc)
            cmdDBQuery.Parameters.Add("@firstname", strFirstName)
            cmdDBQuery.Parameters.Add("@lastname", strLastName)
            cmdDBQuery.Parameters.Add("@email", strEmail)
            cmdDBQuery.Parameters.Add("@notifyloc", strNotifyLoc)
            cmdDBQuery.Parameters.Add("@isactive", blnIsActive)

            dtrDBQuery = cmdDBQuery.ExecuteReader()

            dtrDBQuery.Read()

            nbcObject = New NewBrokerConf(CInt(dtrDBQuery("BrokerID")), dtrDBQuery("Comments"))

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return nbcObject
        End Function

        Public Function ActivateBroker(ByVal intBrokerID As Integer, ByVal blnActivateGroups As Boolean) As String
            'This method activates a deactivated Broker record and, optionally, also activates its affiliated broker groups.  The "is_active" 
            'field of the specified broker id is set to true and, if blnActivateGroups is set to true, any groups that the broker owns will 
            'be activated as well.  The output of this method is a string describing any runtime events or errors.
            conWebLabLS.Open()
            Dim output As String
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_ActivateBroker @brokerID, @activateGroups"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)
            cmdDBQuery.Parameters.Add("@activateGroups", blnActivateGroups)

            output = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()
            Return output
        End Function

        Public Function DeactivateBroker(ByVal intBrokerID As Integer) As String
            'This method deactivates the specified broker along with it's affiliated broker groups.  The "is_active" field of the specified 
            'broker id is set to false and as are those of the groups owned by that broker.  The output of this method is a string describing 
            'any runtime events or errors.
            conWebLabLS.Open()
            Dim output, strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_DeactivateBroker @brokerID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)

            output = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()
            Return output
        End Function

        Public Function RemoveBroker(ByVal intBrokerID As Integer) As String
            'This method removes a Service Broker registration record from the Lab Server  The specified broker is removed from 
            'the lab server database as well as any groups who were owned by that broker.  The return value is a string 
            'describing any runtime events or errors.
            conWebLabLS.Open()
            Dim output, strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_RemoveBroker @brokerID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)

            output = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()
            Return output
        End Function

        Public Function BrokerIsMemberOf(ByVal intBrokerID As Integer) As ClassObject
            'This method looks up the class in which the specified broker is a member of.  The output is of the form of a 
            'single class ID and a single class name.  This data will be placed into a custom data object. 
            'if intBrokerID is not a valid broker reference, both locations will return as NULL values
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim cObject As ClassObject

            Dim strDBQuery = "SELECT ClassID, ClassName FROM dbo.rpm_BrokerIsMemberOf(@brokerID)"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)
            dtrDBQuery = cmdDBQuery.ExecuteReader()
            dtrDBQuery.Read()

            cObject = New ClassObject(CInt(dtrDBQuery("ClassID")), dtrDBQuery("ClassName"))

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return cObject
        End Function

        Public Function BrokerIsMemberOf(ByVal intBrokerID As Integer, ByVal strClassPart As String) As String
            'This method looks up the class in which the specified broker is a member of.  The output is of the form of a 
            'single class ID and a single class name, depending on the value of strClassPart.  If intBrokerID is not a valid 
            'group reference, both locations will return as NULL values.  Valid values for strClassPart are as follows:
            '
            '"id" - returns the class ID
            '"name" - returns the name of the class
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            Select Case LCase(strClassPart)
                Case "id"
                    strDBQuery = "SELECT ClassID FROM dbo.rpm_BrokerIsMemberOf(@brokerID)"
                Case "name"
                    strDBQuery = "SELECT ClassName FROM dbo.rpm_BrokerIsMemberOf(@brokerID)"
                Case Else
                    conWebLabLS.Close()
                    Return "Error - An invalid output field was specified"
            End Select

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function AddGroup(ByVal intOwnerID As Integer, ByVal intClassID As Integer, ByVal strName As String, ByVal strDesc As String, ByVal blnIsActive As Boolean) As String
            'This method creates a broker assigned group on the lab server.  Inputs to this method are, in order, the ID of 
            'the broker the group will belong to, the ID of the class the group will be a memer of, the name of the group, 
            'a text description and a boolean value describing whether it is active or not  The output is of the form of 
            'a string containging any runtime events or errors that may have occurred.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_AddGroup @ownerID, @classID, @name, @desc, @isactive"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@ownerID", intOwnerID)
            cmdDBQuery.Parameters.Add("@classID", intClassID)
            cmdDBQuery.Parameters.Add("@name", strName)
            cmdDBQuery.Parameters.Add("@desc", strDesc)
            cmdDBQuery.Parameters.Add("@isactive", blnIsActive)

            Dim output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return output
        End Function

        Public Function ActivateGroup(ByVal intGroupID As Integer) As String
            'This method activates a deactivated broker Group.  The "is_active" field of the specified group id is set to true and
            'the output of this method is a string describing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_ActivateGroup @groupID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)

            Dim output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return output
        End Function

        Public Function DeactivateGroup(ByVal intGroupID As Integer) As String
            'This method deactivates a broker group.  The "is_active" field of the specified groupd id is set to false and 
            'the output of this method is a string describing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_DeactivateGroup @groupID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)

            Dim output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return output
        End Function

        Public Function RemoveGroup(ByVal intGroupID As Integer) As String
            'This method removes a broker assigned group from the Lab Server.  The specified group is removed from the system 
            'and any runtime events or errors are returned as the output of the function.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_RemoveGroup @groupID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)

            Dim output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return output
        End Function

        Public Function AddSiteUser(ByVal strFirstName As String, ByVal strLastName As String, ByVal strEmail As String, ByVal strUserName As String, ByVal strPassword As String, ByVal intClassID As Integer, ByVal blnIsActive As Boolean) As String
            'This method creates a new site user account on the Lab Server System.  This account is to be used by a person logging on to the Lab Server
            'administration/information site.  The account is added by invoking a similarly named SQL stored procedure.  The output of this function is the 
            'user_id of the new record in the case of successful completion.  Otherwise, an error message is returned.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_AddSiteUser @first_name, @last_name, @email, @username, @password, @classID,	@is_active;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@first_name", strFirstName)
            cmdDBQuery.Parameters.Add("@last_name", strLastName)
            cmdDBQuery.Parameters.Add("@email", strEmail)
            cmdDBQuery.Parameters.Add("@username", strUserName)
            cmdDBQuery.Parameters.Add("@password", strPassword)
            cmdDBQuery.Parameters.Add("@classID", intClassID)
            cmdDBQuery.Parameters.Add("@is_active", blnIsActive)

            Dim Output As String = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()
            Return Output
        End Function

        Public Function ActivateSiteUser(ByVal intUserID As Integer) As String
            'This method toggles the status of the specified site user account to active.  This is accomplished by executing the SQL stored procedure 
            'of the same name.  The output of this function is a string indicating success or failure of the operation.
            conWebLabLS.Open()
            Dim Output As String
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_ActivateSiteUser @userID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@userID", intUserID)

            Output = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()
            Return Output
        End Function

        Public Function DeactivateSiteUser(ByVal intUserID As Integer) As String
            'this method toggles the status of the specified site user accout to inactive.  This is accomplished by executing the SQL stored procedure
            'of the same name.  The output of this function is a string indicating success or failure of the operation.
            conWebLabLS.Open()
            Dim strDBQuery, Output As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_DeactivateSiteUser @userID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@userID", intUserID)

            Output = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()
            Return Output
        End Function

        Public Function RemoveSiteUser(ByVal intUserID As Integer) As String
            'This method removes the specified site user account from the Lab Server system.  Specifically, the usage class of the specified user is 
            'properly removed and the user record itself is removed from the system.  This process is handled by a SQL stored procedure of the same name.
            'The output of this function is a string indicating the success or failure of the operation.
            conWebLabLS.Open()
            Dim Output, strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_RemoveSiteUser @userID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@userID", intUserID)

            Output = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()
            Return Output
        End Function

        Public Function AddClass(ByVal strName As String, ByVal strDesc As String) As String
            'This method creates a Usage Class on the Lab Server  A new usage class record is added with the name and description supplied
            'Any runtime events or errors are returned as a string by this method.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_AddClass @name, @desc"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@name", strName)
            cmdDBQuery.Parameters.Add("@desc", strDesc)

            Dim output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return output
        End Function

        Public Function RemoveClass(ByVal intClassID As Integer) As String
            'This method removes a Usage Class from the Lab Server.  The specified usage class is deleted and any orphaned broker or groups
            'are automatically relocated to the default guest class.  The output of this method will contain any runtime event or error messages.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_RemoveClass @classID, ''"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@classID", intClassID)

            Dim output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return output
        End Function

        Public Function RemoveClass(ByVal intClassID As Integer, ByVal intNewClassID As Integer) As String
            'This method removes a Usage Class from the Lab Server.  The specified usage class is deleted and any orphaned brokers or groups
            'are relocated to the class specified by intNewClassID.  The output of this method will contain any runtime event or error messages.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_RemoveClass @classID, @newClassID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@classID", intClassID)
            cmdDBQuery.Parameters.Add("@newClassID", intNewClassID)

            Dim output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return output
        End Function

        Public Function MapBrokerToClass(ByVal intBrokerID As Integer, ByVal intClassID As Integer) As String
            'This method creates/reassigns a membership mapping between a Service Broker record and a Usage Class.  The specified broker is reassigned to the 
            'specified usage class and stored class membership tallies are updated.  Any runtime events or errors are reported as the 
            'output of this function.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_MapBrokerToClass @brokerID, @classID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)
            cmdDBQuery.Parameters.Add("@classID", intClassID)

            Dim output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return output
        End Function

        Public Function MapGroupToClass(ByVal intGroupID As Integer, ByVal intClassID As Integer) As String
            'This method creates/reassigns a membership mapping between a broker assigned group and a Usage Class.  The specified group is 
            'reassigned to the specified usage class and stored class membership tallies are updated.  
            'Any runtime events or errors are reported as the output of this function.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_MapGroupToClass @groupID, @classID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)
            cmdDBQuery.Parameters.Add("@classID", intClassID)

            Dim output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return output
        End Function

        Public Function MapSiteUserToClass(ByVal intUserID As Integer, ByVal intClassID As Integer) As String
            'this method properly reassigns the class membership for the specified site user account to the specified usage class.  
            'This procedure is handled by a similarly named SQL stored procedure.  The output of this function is a string indicating 
            'the success or failure of the operation.
            conWebLabLS.Open()
            Dim Output, strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_MapSiteUserToClass @userID, @classID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@userID", intUserID)
            cmdDBQuery.Parameters.Add("@classID", intClassID)

            Output = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()
            Return Output
        End Function

        Public Function GroupIsMemberOf(ByVal intGroupID As Integer) As ClassObject
            'This method reports the Usage Class the specified group is a member of.  The output is of the form of a single class ID 
            'and a single class name.  This data will be placed into a custom data object.  If intGroupID is not a valid group 
            'reference, both locations will return as NULL values.
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim cObject As ClassObject

            Dim strDBQuery = "SELECT ClassID, ClassName FROM dbo.rpm_GroupIsMemberOf(@groupID)"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)
            dtrDBQuery = cmdDBQuery.ExecuteReader()
            dtrDBQuery.Read()

            cObject = New ClassObject(CInt(dtrDBQuery("ClassID")), dtrDBQuery("ClassName"))

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return cObject
        End Function

        Public Function GroupIsMemberOf(ByVal intGroupID As Integer, ByVal strClassPart As String) As String
            'This method reports the Usage Class the specified group is a member of.  The output is of the form of a single class ID 
            'and a single class name, depending on the value of strClassPart.  If intGroupID is not a valid group reference.  Valid
            'values for strClassPart are as follows:
            '
            '"id" - returns the record ID of the Usage Class
            '"name" - returns the name of the Usage Class
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            Select Case LCase(strClassPart)
                Case "id"
                    strDBQuery = "SELECT ClassID FROM dbo.rpm_GroupIsMemberOf(@groupID)"
                Case "name"
                    strDBQuery = "SELECT ClassName FROM dbo.rpm_GroupIsMemberOf(@groupID)"
                Case Else
                    Return "Error - An invalid output field was specified"
            End Select

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function GetClassResourcePermission(ByVal intClassID As Integer, ByVal intResourceID As Integer) As PermissionMappingObject
            'This method retrieves a manifest of the permissions the specified usage class has on the specified system resource.  The 
            'output is packaged into a custom data object.  If either intClassID or intResourceID are invalid, the fields of the return 
            'object will be 0/false.
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim dtrDbQuery As SqlDataReader
            Dim pmObject As PermissionMappingObject

            Dim strDBQuery = "SELECT MappingId, CanView, CanEdit, CanGrant, CanDelete, Priority FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@classID", intClassID)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            dtrDbQuery = cmdDBQuery.ExecuteReader()


            If dtrDbQuery.Read() Then
                pmObject = New PermissionMappingObject(CInt(dtrDbQuery("MappingID")), CBool(dtrDbQuery("CanView")), CBool(dtrDbQuery("CanEdit")), CBool(dtrDbQuery("CanDelete")), CBool(dtrDbQuery("CanGrant")), CInt(dtrDbQuery("Priority")))
            Else
                pmObject = New PermissionMappingObject(0, False, False, False, False, 0)
            End If
            dtrDbQuery.Close()
            conWebLabLS.Close()

            Return pmObject
        End Function

        Public Function GetClassResourcePermission(ByVal intClassID As Integer, ByVal strResourceName As String) As PermissionMappingObject
            'This method determines if the specified usage class has permissions on the specified system Resource.  The resource name
            'is first used to find a corresponding resource ID in the system database.  That ID is then used to invoke a SQL UDF to 
            'determine the permissions that the specified class has on the specified resource.  The output is packaged into a custom
            'data object.  If either intClassID or strResourceName are invalid or if there is 
            'no resource mapping between the two, the fields of the return object will be 0/false.
            conWebLabLS.Open()
            Dim objResourceID As Object
            Dim strDBQuery As String
            Dim pmObject As PermissionMappingObject
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader

            strDBQuery = "SELECT resource_id FROM Resources WHERE name = @Name;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Name", strResourceName)

            objResourceID = cmdDBQuery.ExecuteScalar()

            If objResourceID Is DBNull.Value Then
                pmObject = New PermissionMappingObject(0, False, False, False, False, 0)

            Else
                strDBQuery = "SELECT MappingId, CanView, CanEdit, CanGrant, CanDelete, Priority FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@classID", intClassID)
                cmdDBQuery.Parameters.Add("@resourceID", CInt(objResourceID))
                dtrDBQuery = cmdDBQuery.ExecuteReader()
                dtrDBQuery.Read()

                If dtrDBQuery("MappingID") Is DBNull.Value Then
                    pmObject = New PermissionMappingObject(0, False, False, False, False, 0)
                Else
                    pmObject = New PermissionMappingObject(CInt(dtrDBQuery("MappingID")), CBool(dtrDBQuery("CanView")), CBool(dtrDBQuery("CanEdit")), CBool(dtrDBQuery("CanDelete")), CBool(dtrDBQuery("CanGrant")), CInt(dtrDBQuery("Priority")))
                End If

            End If

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return pmObject
        End Function

        Public Function GetClassResourcePermission(ByVal intClassID As Integer, ByVal intResourceID As Integer, ByVal strOutputVal As String) As Boolean
            'This method retrieves the details of permissions the speicified usage class has on the specified system resource.  The output 
            'is in the form of a boolean.  The contents of that output value will depend on the value of strOutputVal.  This input value 
            'should be one of the following: 
            '
            '"canview" - read permission
            '"canedit" - write permission
            '"cangrant" - grant permission
            '"candelete" - Delete permission
            '
            'The specified field will be returned by the function. 
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim objOutput As Object

            Select Case strOutputVal.ToLower()
                Case "canview"
                    strDBQuery = "SELECT CanView FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
                Case "canedit"
                    strDBQuery = "SELECT CanEdit FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
                Case "cangrant"
                    strDBQuery = "SELECT CanGrant FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
                Case "candelete"
                    strDBQuery = "SELECT CanDelete FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
                Case Else
                    Return False
            End Select

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@classID", intClassID)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            objOutput = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            If objOutput Is DBNull.Value Then
                Return False
            ElseIf CStr(objOutput) = "True" Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Function GetClassResourcePermission(ByVal intClassID As Integer, ByVal strResourceName As String, ByVal strOutputVal As String) As Boolean
            'This method determines if the specified usage class has permissions on the specified system Resource.  The resource name
            'is first used to find a corresponding resource ID in the system database.  That ID is then used to invoke a SQL UDF to 
            'determine the permissions that the specified class has on the specified resource.  The output is in the form of a boolean.  
            'The contents of that output value will depend on the value of strOutputVal.  This input value should be one of the following: 
            '
            '"canview" - read permission
            '"canedit" - write permission
            '"cangrant" - grant permission
            '"candelete" - Delete permission
            '
            'The specified field will be returned by the function.  If either intClassID or strResourceName are invalid or if there is no 
            'resource mapping between the two, the return value will be false.
            conWebLabLS.Open()
            Dim objResourceID, objTemp As Object
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim blnOutput As Boolean

            strDBQuery = "SELECT resource_id FROM Resources WHERE name = @Name;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Name", strResourceName)

            objResourceID = cmdDBQuery.ExecuteScalar()

            If objResourceID Is DBNull.Value Then
                blnOutput = False

            Else
                Select Case strOutputVal.ToLower()
                    Case "canview"
                        strDBQuery = "SELECT CanView FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
                    Case "canedit"
                        strDBQuery = "SELECT CanEdit FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
                    Case "cangrant"
                        strDBQuery = "SELECT CanGrant FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
                    Case "candelete"
                        strDBQuery = "SELECT CanDelete FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"
                    Case Else
                        Return False
                End Select

                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@classID", intClassID)
                cmdDBQuery.Parameters.Add("@resourceID", CInt(objResourceID))

                objTemp = cmdDBQuery.ExecuteScalar()

                If objTemp Is DBNull.Value Then
                    blnOutput = False
                ElseIf CStr(objTemp) = "True" Then
                    blnOutput = True
                Else
                    blnOutput = False
                End If

            End If

            conWebLabLS.Close()

            Return blnOutput
        End Function

        Public Function GetClassSetupPriority(ByVal intClassID As Integer, ByVal intResourceID As Integer) As Integer
            'This method checks the priority value assigned to the specified resourceID and classID pair.  If the pair is
            'invalid, a value of zero is returned (this method not a valid form of resource authenticaiton.)
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim objOutput As Object


            strDBQuery = "SELECT Priority FROM dbo.rpm_GetClassResourcePermission(@classID, @resourceID)"

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@classID", intClassID)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            objOutput = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            If objOutput Is DBNull.Value Then
                Return 0
            Else
                Return CInt(objOutput)
            End If

        End Function

        Public Function GetBrokerResourcePermission(ByVal intBrokerID As Integer, ByVal intResourceID As Integer) As PermissionMappingObject
            'This method retrieves the permission set joining the specified broker and the specified system resource.  The 
            'output is of the form of a custom data object.  If either intBrokerID or intResourceID are invalid, the fields 
            'of the return object will be  0/false.
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim dtrDbQuery As SqlDataReader
            Dim pmObject As PermissionMappingObject

            Dim strDBQuery = "SELECT MappingId, CanView, CanEdit, CanGrant, CanDelete, Priority FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)
            cmdDBQuery.Parameters.Add("@resourceId", intResourceID)
            dtrDbQuery = cmdDBQuery.ExecuteReader()

            If dtrDbQuery.Read() Then
                pmObject = New PermissionMappingObject(CInt(dtrDbQuery("MappingID")), CBool(dtrDbQuery("CanView")), CBool(dtrDbQuery("CanEdit")), CBool(dtrDbQuery("CanDelete")), CBool(dtrDbQuery("CanGrant")), CInt(dtrDbQuery("Priority")))
            Else
                pmObject = New PermissionMappingObject(0, False, False, False, False, 0)
            End If

            dtrDbQuery.Close()
            conWebLabLS.Close()

            Return pmObject
        End Function

        Public Function GetBrokerResourcePermission(ByVal intBrokerID As Integer, ByVal strResourceName As String) As PermissionMappingObject
            'This method determines if the specified broker has permissions on the specified system Resource.  The resource name
            'is first used to find a corresponding resource ID in the system database.  That ID is then used to invoke a SQL UDF to 
            'determine the permissions that the specified broker has on the specified resource.  The output is of the form of a custom
            'data object.  If either intClassID or strResourceName are invalid or if there is 
            'no resource mapping between the two, the fields of the return array will be 0/false.
            conWebLabLS.Open()
            Dim objResourceID As Object
            Dim strDBQuery As String
            Dim pmObject As PermissionMappingObject
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader

            strDBQuery = "SELECT resource_id FROM Resources WHERE name = @Name;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Name", strResourceName)

            objResourceID = cmdDBQuery.ExecuteScalar()

            If objResourceID Is DBNull.Value Then
                pmObject = New PermissionMappingObject(0, False, False, False, False, 0)

            Else
                strDBQuery = "SELECT MappingId, CanView, CanEdit, CanGrant, CanDelete, Priority FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)
                cmdDBQuery.Parameters.Add("@resourceID", CInt(objResourceID))
                dtrDBQuery = cmdDBQuery.ExecuteReader()
                dtrDBQuery.Read()

                If dtrDBQuery("MappingID") Is DBNull.Value Then
                    pmObject = New PermissionMappingObject(0, False, False, False, False, 0)
                Else
                    pmObject = New PermissionMappingObject(CInt(dtrDBQuery("MappingID")), CBool(dtrDBQuery("CanView")), CBool(dtrDBQuery("CanEdit")), CBool(dtrDBQuery("CanDelete")), CBool(dtrDBQuery("CanGrant")), CInt(dtrDBQuery("Priority")))
                End If

            End If

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return pmObject
        End Function

        Public Function GetBrokerResourcePermission(ByVal intBrokerID As Integer, ByVal intResourceID As Integer, ByVal strOutputVal As String) As Boolean
            'This method retrieves the details of the specified service broker's permissions on the specified resource.  The output 
            'is in the form of a boolean.  The contents of that output value will depend on the value of strOutputVal.  This input 
            'value should be one of the following:
            '
            '"canview" - read permission
            '"canedit" - write permission
            '"cangrant" - grant permission
            '"candelete" - Delete permission
            ' 
            'The specified field will be returned by the function.  
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim objOutput As Object

            Select Case strOutputVal.ToLower()
                Case "canview"
                    strDBQuery = "SELECT CanView FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
                Case "canedit"
                    strDBQuery = "SELECT CanEdit FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
                Case "cangrant"
                    strDBQuery = "SELECT CanGrant FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
                Case "candelete"
                    strDBQuery = "SELECT CanDelete FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
                Case Else
                    Return False
            End Select

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            objOutput = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            If objOutput Is DBNull.Value Then
                Return False
            ElseIf CStr(objOutput) = "True" Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Function GetBrokerResourcePermission(ByVal intBrokerID As Integer, ByVal strResourceName As String, ByVal strOutputVal As String) As Boolean
            'This method determines if the specified broker has permissions on the specified system Resource.  The resource name
            'is first used to find a corresponding resource ID in the system database.  That ID is then used to invoke a SQL UDF to 
            'determine the permissions that the specified broker has on the specified resource.  The output is in the form of a boolean.  
            'The contents of that output value will depend on the value of strOutputVal.  This input value should be one of the following: 
            '
            '"canview" - read permission
            '"canedit" - write permission
            '"cangrant" - grant permission
            '"candelete" - Delete permission
            '
            'The specified field will be returned by the function.  If either intClassID or strResourceName are invalid or if there is no 
            'resource mapping between the two, the return value will be False.
            conWebLabLS.Open()
            Dim objResourceID, objTemp As Object
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim blnOutput As Boolean

            strDBQuery = "SELECT resource_id FROM Resources WHERE name = @Name;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Name", strResourceName)

            objResourceID = cmdDBQuery.ExecuteScalar()

            If objResourceID Is DBNull.Value Then
                blnOutput = False

            Else
                Select Case strOutputVal.ToLower()
                    Case "canview"
                        strDBQuery = "SELECT CanView FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
                    Case "canedit"
                        strDBQuery = "SELECT CanEdit FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
                    Case "cangrant"
                        strDBQuery = "SELECT CanGrant FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
                    Case "candelete"
                        strDBQuery = "SELECT CanDelete FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"
                    Case Else
                        Return False
                End Select

                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)
                cmdDBQuery.Parameters.Add("@resourceID", CInt(objResourceID))

                objTemp = cmdDBQuery.ExecuteScalar()

                If objTemp Is DBNull.Value Then
                    blnOutput = False
                ElseIf CStr(objTemp) = "True" Then
                    blnOutput = True
                Else
                    blnOutput = False
                End If

            End If

            conWebLabLS.Close()

            Return blnOutput
        End Function

        Public Function GetBrokerSetupPriority(ByVal intBrokerID As Integer, ByVal intResourceID As Integer) As Integer
            'This method checks the priority value assigned to the specified resourceID and brokerID pair.  If the pair is
            'invalid, a value of zero is returned (this method not a valid form of resource authenticaiton.)
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim objOutput As Object

            strDBQuery = "SELECT Priority FROM dbo.rpm_GetBrokerResourcePermission(@brokerID, @resourceID)"

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            objOutput = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            If objOutput Is DBNull.Value Then
                Return 0
            Else
                Return CInt(objOutput)
            End If

        End Function

        Public Function GetGroupResourcePermission(ByVal intGroupID As Integer, ByVal intResourceID As Integer) As PermissionMappingObject
            'This method retrieves the permission set joining the specified broker group to the specified system resource.  The output 
            'is of the form of a custom data object.  If either intGroupID or intResourceID are invalid, the fields of the return object 
            'will be 0/false.
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim dtrDbQuery As SqlDataReader
            Dim pmObject As PermissionMappingObject

            Dim strDBQuery = "SELECT MappingId, CanView, CanEdit, CanGrant, CanDelete, Priority FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)
            cmdDBQuery.Parameters.Add("@resourceId", intResourceID)
            dtrDbQuery = cmdDBQuery.ExecuteReader()

            If dtrDbQuery.Read() Then
                pmObject = New PermissionMappingObject(CInt(dtrDbQuery("MappingID")), CBool(dtrDbQuery("CanView")), CBool(dtrDbQuery("CanEdit")), CBool(dtrDbQuery("CanDelete")), CBool(dtrDbQuery("CanGrant")), CInt(dtrDbQuery("Priority")))
            Else
                pmObject = New PermissionMappingObject(0, False, False, False, False, 0)
            End If

            dtrDbQuery.Close()
            conWebLabLS.Close()

            Return pmObject
        End Function

        Public Function GetGroupResourcePermission(ByVal intGroupID As Integer, ByVal strResourceName As String) As PermissionMappingObject
            'This method determines if the specified group has permissions on the specified system Resource.  The resource name
            'is first used to find a corresponding resource ID in the system database.  That ID is then used to invoke a SQL UDF to 
            'determine the permissions that the specified group has on the specified resource.  The output is of the form of a custom
            'data object.  If either intClassID or strResourceName are invalid or if there is 
            'no resource mapping between the two, the fields of the return array will be 0/false.
            conWebLabLS.Open()
            Dim objResourceID As Object
            Dim strDBQuery As String
            Dim pmObject As PermissionMappingObject
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader

            strDBQuery = "SELECT resource_id FROM Resources WHERE name = @Name;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Name", strResourceName)

            objResourceID = cmdDBQuery.ExecuteScalar()

            If objResourceID Is DBNull.Value Then
                pmObject = New PermissionMappingObject(0, False, False, False, False, 0)

            Else
                strDBQuery = "SELECT MappingId, CanView, CanEdit, CanGrant, CanDelete, Priority FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@groupID", intGroupID)
                cmdDBQuery.Parameters.Add("@resourceID", CInt(objResourceID))
                dtrDBQuery = cmdDBQuery.ExecuteReader()
                dtrDBQuery.Read()

                If dtrDBQuery("MappingID") Is DBNull.Value Then
                    pmObject = New PermissionMappingObject(0, False, False, False, False, 0)
                Else
                    pmObject = New PermissionMappingObject(CInt(dtrDBQuery("MappingID")), CBool(dtrDBQuery("CanView")), CBool(dtrDBQuery("CanEdit")), CBool(dtrDBQuery("CanDelete")), CBool(dtrDBQuery("CanGrant")), CInt(dtrDBQuery("Priority")))
                End If

            End If

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return pmObject
        End Function

        Public Function GetGroupResourcePermission(ByVal intGroupID As Integer, ByVal intResourceID As Integer, ByVal strOutputVal As String) As Boolean
            'This method retrieves the details of the specified group's permissions on the specified system resource.  The output 
            'is in the form of a boolean.  The contents of that output value will depend on the value of strOutputVal.  This input 
            'value should be one of the following: 
            '
            '"canview" - read permission
            '"canedit" - write permission
            '"cangrant" - grant permission
            '"candelete" - Delete permission
            ' 
            'The specified field will be returned by the function.  
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim objOutput As Object

            Select Case strOutputVal.ToLower()
                Case "canview"
                    strDBQuery = "SELECT CanView FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
                Case "canedit"
                    strDBQuery = "SELECT CanEdit FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
                Case "cangrant"
                    strDBQuery = "SELECT CanGrant FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
                Case "candelete"
                    strDBQuery = "SELECT CanDelete FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
                Case Else
                    Return False
            End Select

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            objOutput = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            If objOutput Is DBNull.Value Then
                Return False
            ElseIf CStr(objOutput) = "True" Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Function GetGroupResourcePermission(ByVal intGroupID As Integer, ByVal strResourceName As String, ByVal strOutputVal As String) As Boolean
            'This method determines if the specified group has permissions on the specified system Resource.  The resource name
            'is first used to find a corresponding resource ID in the system database.  That ID is then used to invoke a SQL UDF to 
            'determine the permissions that the specified group has on the specified resource.  The output is in the form of a boolean.  
            'The contents of that output value will depend on the value of strOutputVal.  This input value should be one of the following:
            '
            '"canview" - read permission
            '"canedit" - write permission
            '"cangrant" - grant permission
            '"candelete" - Delete permission
            '
            'The specified field will be returned by the function.  If either intClassID or strResourceName are invalid or if there is 
            'no resource mapping between the two, the return value will be false.
            conWebLabLS.Open()
            Dim objResourceID, objTemp As Object
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim blnOutput As Boolean


            strDBQuery = "SELECT resource_id FROM Resources WHERE name = @Name;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Name", strResourceName)

            objResourceID = cmdDBQuery.ExecuteScalar()

            If objResourceID Is DBNull.Value Then
                blnOutput = False

            Else
                Select Case strOutputVal.ToLower()
                    Case "canview"
                        strDBQuery = "SELECT CanView FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
                    Case "canedit"
                        strDBQuery = "SELECT CanEdit FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
                    Case "cangrant"
                        strDBQuery = "SELECT CanGrant FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
                    Case "candelete"
                        strDBQuery = "SELECT CanDelete FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"
                    Case Else
                        Return False
                End Select


                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@groupID", intGroupID)
                cmdDBQuery.Parameters.Add("@resourceID", CInt(objResourceID))

                objTemp = cmdDBQuery.ExecuteScalar()

                If objTemp Is DBNull.Value Then
                    blnOutput = False
                ElseIf CStr(objTemp) = "True" Then
                    blnOutput = True
                Else
                    blnOutput = False
                End If

            End If

            conWebLabLS.Close()

            Return blnOutput
        End Function

        Public Function GetGroupSetupPriority(ByVal intGroupID As Integer, ByVal intResourceID As Integer) As Integer
            'This method checks the priority value assigned to the specified resourceID and groupID pair.  If the pair is
            'invalid, a value of zero is returned (this method not a valid form of resource authenticaiton.)  
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim objOutput As Object

            strDBQuery = "SELECT Priority FROM dbo.rpm_GetGroupResourcePermission(@groupID, @resourceID)"

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            objOutput = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            If objOutput Is DBNull.Value Then
                Return 0
            Else
                Return CInt(objOutput)
            End If

        End Function

        Public Function GetUserResourcePermission(ByVal intUserID As Integer, ByVal intResourceID As Integer) As PermissionMappingObject
            'This method retrieves the permission set joining the specified user and the specified system resource.  The output 
            'is of the form of a custom data object.  If either intGroupID or intResourceID are invalid, the fields of the return
            'object will be 0/false.
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim dtrDbQuery As SqlDataReader
            Dim pmObject As PermissionMappingObject

            Dim strDBQuery = "SELECT MappingId, CanView, CanEdit, CanGrant, CanDelete, Priority FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@userID", intUserID)
            cmdDBQuery.Parameters.Add("@resourceId", intResourceID)
            dtrDbQuery = cmdDBQuery.ExecuteReader()

            If dtrDbQuery.Read() Then
                pmObject = New PermissionMappingObject(CInt(dtrDbQuery("MappingID")), CBool(dtrDbQuery("CanView")), CBool(dtrDbQuery("CanEdit")), CBool(dtrDbQuery("CanDelete")), CBool(dtrDbQuery("CanGrant")), CInt(dtrDbQuery("Priority")))
            Else
                pmObject = New PermissionMappingObject(0, False, False, False, False, 0)
            End If
            dtrDbQuery.Close()
            conWebLabLS.Close()

            Return pmObject
        End Function

        Public Function GetUserResourcePermission(ByVal intUserID As Integer, ByVal strResourceName As String) As PermissionMappingObject
            'This method determines if the specified user has permissions on the specified system Resource.  The resource name
            'is first used to find a corresponding resource ID in the system database.  That ID is then used to invoke a SQL UDF to 
            'determine the permissions that the specified user has on the specified resource.  The output is of the form of a custom 
            'data object.  If either intClassID or strResourceName are invalid or if there is 
            'no resource mapping between the two, the fields of the return array will be 0/false.
            conWebLabLS.Open()
            Dim objResourceID As Object
            Dim strDBQuery As String
            Dim pmObject As PermissionMappingObject
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader

            strDBQuery = "SELECT resource_id FROM Resources WHERE name = @Name;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Name", strResourceName)

            objResourceID = cmdDBQuery.ExecuteScalar()

            If objResourceID Is DBNull.Value Then
                pmObject = New PermissionMappingObject(0, False, False, False, False, 0)

            Else
                strDBQuery = "SELECT MappingId, CanView, CanEdit, CanGrant, CanDelete, Priority FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@userID", intUserID)
                cmdDBQuery.Parameters.Add("@resourceID", CInt(objResourceID))
                dtrDBQuery = cmdDBQuery.ExecuteReader()
                dtrDBQuery.Read()

                If dtrDBQuery("MappingID") Is DBNull.Value Then
                    pmObject = New PermissionMappingObject(0, False, False, False, False, 0)
                Else
                    pmObject = New PermissionMappingObject(CInt(dtrDBQuery("MappingID")), CBool(dtrDBQuery("CanView")), CBool(dtrDBQuery("CanEdit")), CBool(dtrDBQuery("CanDelete")), CBool(dtrDBQuery("CanGrant")), CInt(dtrDBQuery("Priority")))
                End If

            End If

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return pmObject
        End Function

        Public Function GetUserResourcePermission(ByVal intUserID As Integer, ByVal intResourceID As Integer, ByVal strOutputVal As String) As Boolean
            'This method retrieves the details of the specified user's permissions on the specified system resource.  The output 
            'is in the form of a boolean.  The contents of that output value will depend on the value of strOutputVal.  This input 
            'value should be one of the following: 
            '
            '"canview" - read permission
            '"canedit" - write permission
            '"cangrant" - grant permission
            '"candelete" - Delete permission
            ' 
            'The specified field will be returned by the function.  
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim objOutput As Object

            Select Case strOutputVal.ToLower()
                Case "canview"
                    strDBQuery = "SELECT CanView FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
                Case "canedit"
                    strDBQuery = "SELECT CanEdit FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
                Case "cangrant"
                    strDBQuery = "SELECT CanGrant FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
                Case "candelete"
                    strDBQuery = "SELECT CanDelete FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
                Case Else
                    Return False
            End Select

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@userID", intUserID)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            objOutput = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            If objOutput Is DBNull.Value Then
                Return False
            ElseIf CStr(objOutput) = "True" Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Function GetUserResourcePermission(ByVal intUserID As Integer, ByVal strResourceName As String, ByVal strOutputVal As String) As Boolean
            'This method determines if the specified user has permissions on the specified system Resource.  The resource name
            'is first used to find a corresponding resource ID in the system database.  That ID is then used to invoke a SQL UDF to 
            'determine the permissions that the specified user has on the specified resource.  The output is in the form of a boolean.  
            'The contents of that output value will depend on the value of strOutputVal.  This input value should be one of the following: 
            '
            '"canview" - read permission
            '"canedit" - write permission
            '"cangrant" - grant permission
            '"candelete" - Delete permission
            '
            'The specified field will be returned by the function.  If either intClassID or strResourceName are invalid or if there is 
            'no resource mapping between the two, the return value will be false.
            conWebLabLS.Open()
            Dim objResourceID, objTemp As Object
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim blnOutput As Boolean

            strDBQuery = "SELECT resource_id FROM Resources WHERE name = @Name;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Name", strResourceName)

            objResourceID = cmdDBQuery.ExecuteScalar()

            If objResourceID Is DBNull.Value Then
                blnOutput = False

            Else
                Select Case strOutputVal.ToLower()
                    Case "canview"
                        strDBQuery = "SELECT CanView FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
                    Case "canedit"
                        strDBQuery = "SELECT CanEdit FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
                    Case "cangrant"
                        strDBQuery = "SELECT CanGrant FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
                    Case "candelete"
                        strDBQuery = "SELECT CanDelete FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"
                    Case Else
                        Return False
                End Select

                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@userID", intUserID)
                cmdDBQuery.Parameters.Add("@resourceID", CInt(objResourceID))

                objTemp = cmdDBQuery.ExecuteScalar()

                If objTemp Is DBNull.Value Then
                    blnOutput = False
                ElseIf CStr(objTemp) = "True" Then
                    blnOutput = True
                Else
                    blnOutput = False
                End If

            End If

            conWebLabLS.Close()

            Return blnOutput
        End Function

        Public Function GetUserSetupPriority(ByVal intUserID As Integer, ByVal intResourceID As Integer) As Integer
            'This method checks the priority value assigned to the specified resourceID and userID pair.  If the pair is
            'invalid, a value of zero is returned (this method not a valid form of resource authenticaiton.)  
            conWebLabLS.Open()
            Dim cmdDBQuery As SqlCommand
            Dim strDBQuery As String
            Dim objOutput As Object

            strDBQuery = "SELECT Priority FROM dbo.rpm_GetUserResourcePermission(@userID, @resourceID)"

            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@userID", intUserID)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            objOutput = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            If objOutput Is DBNull.Value Then
                Return 0
            Else
                Return CInt(objOutput)
            End If

        End Function

        Public Function AddResource(ByVal strName As String, ByVal strType As String, ByVal strCategory As String, ByVal strDesc As String) As String
            'This method adds a system resource to the Lab Server  A new resource's name, type, category and description are specified as 
            'inputs to the method.  The effect of execution is that the input data is checked for correctness and, if it passes, the resource is 
            'added to the system by way of a database record being written.  The output of the method is a string detailing any runtime events or 
            'errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            strDBQuery = "EXEC rpm_AddResource @name, @type, @category, @desc"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@name", strName)
            cmdDBQuery.Parameters.Add("@type", strType)
            cmdDBQuery.Parameters.Add("@category", strCategory)
            cmdDBQuery.Parameters.Add("@desc", strDesc)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function RemoveResource(ByVal intResourceID As Integer) As String
            'This method removes a resource from the Lab Server system.  This method takes as input a valid resource record ID and, as it's
            'effect, deletes that record and any associated permission mappings.  If the specified resource is a setup record, the 
            'setup record along with associated terminal information will also be removed.  The output of this method is a string detailing 
            'any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            strDBQuery = "EXEC rpm_RemoveResource @resourceID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function AddSetup(ByVal strName As String, ByVal strIconPath As String, ByVal strCategory As String, ByVal strDesc As String) As String
            'This method adds a Setup definition to the Lab Server.  This method takes as input a name, URL of an associated 
            'applet icon referenced from the lab server web root, category name that is unique and chosen by the user and a description
            ' The effect of this method is to create a new Setup
            'type record in the lab server database.  The output of this method is a string detailing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_AddSetup @name, @iconPath, @category, @description"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@name", strName)
            cmdDBQuery.Parameters.Add("@iconPath", strIconPath)
            cmdDBQuery.Parameters.Add("@category", strCategory)
            cmdDBQuery.Parameters.Add("@description", strDesc)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function AddSetupTerminal(ByVal intSetupID As Integer, ByVal strName As String, ByVal intXPixelLoc As Integer, ByVal intYPixelLoc As Integer, ByVal dblMaxAmplitude As Double, ByVal dblMaxOffset As Double, ByVal dblMaxCurrent As Double, ByVal dblMaxFrequency As Double, ByVal dblMaxSamplingRate As Double, ByVal dblMaxSamplingTime As Double, ByVal dblMaxPoints As Double, ByVal strInstrument As String) As String
            'This method adds a Setup Terminal definition to the Lab Server.  This terminal is associated with the speciied Setup
            'at creation/  .This method takes as input a terminal name, function, x and y location on the appropriate Setup icon file, compliance values as 
            'well as a reference to the setup that the terminal is to be assigned to.  The effect of this method is that a new terminal 
            'record will be written to the database.  
            'The output of this method is a string detailing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_AddSetupTerminal @setupID, @name, @xPixelLoc, @yPixelLoc, @max_amplitude, @max_offset, @max_current, @max_frequency, @max_sampling_rate, @max_sampling_time, @max_points, @instrument"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@setupID", intSetupID)
            cmdDBQuery.Parameters.Add("@name", strName)
            cmdDBQuery.Parameters.Add("@xPixelLoc", intXPixelLoc)
            cmdDBQuery.Parameters.Add("@yPixelLoc", intYPixelLoc)
            cmdDBQuery.Parameters.Add("@max_amplitude", dblMaxAmplitude)
            cmdDBQuery.Parameters.Add("@max_offset", dblMaxOffset)
            cmdDBQuery.Parameters.Add("@max_current", dblMaxCurrent)
            cmdDBQuery.Parameters.Add("@max_frequency", dblMaxFrequency)
            cmdDBQuery.Parameters.Add("@max_sampling_rate", dblMaxSamplingRate)
            cmdDBQuery.Parameters.Add("@max_sampling_time", dblMaxSamplingTime)
            cmdDBQuery.Parameters.Add("@max_points", dblMaxPoints)
            cmdDBQuery.Parameters.Add("@instrument", strInstrument)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function



        Public Function RemoveSetupTerminal(ByVal intSetupTerminalID As Integer) As String
            'This method removes the specified Setup Terminal definition from the Lab Server.  This method takes as input a valid     
            'Setup terminal ID and, as its effect, removes that terminal's record from the system database. 
            'The output of this function is a string describing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_RemoveSetupTerminal @setupTermID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@setupTermID", intSetupTerminalID)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function RemoveSetup(ByVal intSetupID As Integer) As String
            'This method removes the specified Setup from the Lab Server.  This method takes as input a valid setup ID and, as its effect,
            'removes that setup record, as well as it's associated terminals, from the system database.  
            ' The output of this function is a string describing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_RemoveSetup @setupID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@SetupID", intSetupID)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function CopySetup(ByVal intSetupID As Integer, ByVal strName As String) As String
            'This method takes as input a valid setup ID and a new setup name.  The effect of this method is to create a duplicate 
            'of the type specified by intSetupID under the specified new name.  The output of this method is a string that details any runtime 
            'events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_CopySetup @setupID, @newName"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@setupID", intSetupID)
            cmdDBQuery.Parameters.Add("@newName", strName)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function SetActiveSetup(ByVal intSetupID As Integer, ByVal blnIsActive As Boolean) As String
            'The effect of this method is that the referenced setup's status is set to reflect the value of blnIsActive (True = active, False = inactive).  
            'The output is a string detailing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_SetActiveSetup @isActive, @setupID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@isActive", blnIsActive)
            cmdDBQuery.Parameters.Add("@setupID", intSetupID)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function SetActiveSetup(ByVal intActivePosID As Integer, ByVal intSetupID As Integer, ByVal blnIsActive As Boolean) As String
            'This method takes as input the ID of the active setup position record to be edited and the valid setup ID that the active setup 
            'position should be set to.  The effect of this method is that the referenced active setup position is set to reference the specified 
            'device profile record and it's status is updated to reflect the value of blnIsActive (True = active, False = inactive).  The output 
            'is a string detailing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_SetActiveDevice @activeID, @isActive, @setupID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@activeID", intActivePosID)
            cmdDBQuery.Parameters.Add("@isActive", blnIsActive)
            cmdDBQuery.Parameters.Add("@setupID", intSetupID)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function


        Public Function MapClassToResource(ByVal intResourceID As Integer, ByVal intClassID As Integer, ByVal blnCanView As Boolean, ByVal blnCanEdit As Boolean, ByVal blnCanGrant As Boolean, ByVal blnCanDelete As Boolean) As String
            'This method creates a permission mapping between the specified Usage Class and the specified System Resource.  The input to the 
            'function is the resource ID and class Id that are to be mapped together.  Additionally, four permission parameters (CanView, 
            'CanEdit, CanGrant and CanDelete) must be specified.  The effect of this function is to give the specified permissions to manipulate 
            'the specified setup to the specified usage class by adding a record to the permission mapping repository in the system database.  
            'The output of this function is a string detailing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_MapClassToResource @resourceID, @classID, @canView, @canEdit, @canGrant, @canDelete, '0'"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            cmdDBQuery.Parameters.Add("@classID", intClassID)
            cmdDBQuery.Parameters.Add("@canView", blnCanView)
            cmdDBQuery.Parameters.Add("@canEdit", blnCanEdit)
            cmdDBQuery.Parameters.Add("@canGrant", blnCanGrant)
            cmdDBQuery.Parameters.Add("@canDelete", blnCanDelete)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function MapClassToResource(ByVal intResourceID As Integer, ByVal intClassID As Integer, ByVal blnCanView As Boolean, ByVal blnCanEdit As Boolean, ByVal blnCanGrant As Boolean, ByVal blnCanDelete As Boolean, ByVal intPriority As Integer) As String
            'This method creates a permission mapping between the specified Usage Class and the specified System Resource.  The input to the 
            'function is the resource ID and class Id that are to be mapped together.  Additionally, five permission parameters (CanView, CanEdit, 
            'CanGrant, CanDelete and Priority) must be specified.  While the first four are boolean values, priority is an integer value (between 
            '+20 and -20) that is used for determining queue ordering.  As such, priority is only a factor if the specified resource is of type 
            'setup.  The effect of this function is to give the specified permissions to manipulate the specified setup to the specified usage 
            'class by adding a record to the permission mapping repository in the system database.  The output of this function is a string 
            'detailing any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_MapClassToResource @resourceID, @classID, @canView, @canEdit, @canGrant, @canDelete, @priority"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@resourceID", intResourceID)
            cmdDBQuery.Parameters.Add("@classID", intClassID)
            cmdDBQuery.Parameters.Add("@canView", blnCanView)
            cmdDBQuery.Parameters.Add("@canEdit", blnCanEdit)
            cmdDBQuery.Parameters.Add("@canGrant", blnCanGrant)
            cmdDBQuery.Parameters.Add("@canDelete", blnCanDelete)
            cmdDBQuery.Parameters.Add("@priority", intPriority)

            Dim Output As String = cmdDBQuery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function RemoveResourceMapping(ByVal intMappingID As Integer) As String
            'This method removes the specified Resource Permission mapping from the Lab Server.  The input is a mapping id that references a 
            'valid class/resource map record and the effect is the deletion of that record.  The output of this function is a string that 
            'details any runtime events or errors.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBquery As SqlCommand

            strDBQuery = "EXEC rpm_RemoveresourceMapping @mappingID"
            cmdDBquery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBquery.Parameters.Add("@mappingID", intMappingID)

            Dim Output As String = cmdDBquery.ExecuteScalar()
            conWebLabLS.Close()

            Return Output
        End Function

        Public Function EditResourceMapping(ByVal intMappingID As Integer, ByVal blnCanView As Boolean, ByVal blnCanEdit As Boolean, ByVal blnCanGrant As Boolean, ByVal blnCanDelete As Boolean) As String
            'This method is used to update a specified class/resource mapping.  The input to this method, in order, are the record ID of the 
            'resource permission mapping to be edited along with new values for View, Edit, Grant and Delete priviledges.  This particular 
            'signature is used for non-Setup type resources.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_EditResourceMapping @MappingID, @canView, @canEdit, @canGrant, @canDelete, NULL"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@MappingID", intMappingID)
            cmdDBQuery.Parameters.Add("@canView", blnCanView)
            cmdDBQuery.Parameters.Add("@canEdit", blnCanEdit)
            cmdDBQuery.Parameters.Add("@canGrant", blnCanGrant)
            cmdDBQuery.Parameters.Add("@canDelete", blnCanDelete)

            Dim Output As String = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            Return Output
        End Function

        Public Function EditResourceMapping(ByVal intMappingID As Integer, ByVal blnCanView As Boolean, ByVal blnCanEdit As Boolean, ByVal blnCanGrant As Boolean, ByVal blnCanDelete As Boolean, ByVal intPriority As Integer) As String
            'This method is used to update a specified class/resource mapping.  The input to this method, in order, are the record ID of the 
            'resource permission mapping to be edited, new values for View, Edit, Grant and Delete priviledges as well as a job execution 
            'priority value.  This priority value is valid only if it is between +20 and -20.  This particular signature is used for only 
            'for Setup resources.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC rpm_EditResourceMapping @MappingID, @canView, @canEdit, @canGrant, @canDelete, @priority"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@MappingID", intMappingID)
            cmdDBQuery.Parameters.Add("@canView", blnCanView)
            cmdDBQuery.Parameters.Add("@canEdit", blnCanEdit)
            cmdDBQuery.Parameters.Add("@canGrant", blnCanGrant)
            cmdDBQuery.Parameters.Add("@canDelete", blnCanDelete)
            cmdDBQuery.Parameters.Add("@priority", intPriority)

            Dim Output As String = cmdDBQuery.ExecuteScalar()

            conWebLabLS.Close()

            Return Output
        End Function

        Public Function GetGroupID(ByVal intBrokerID As Integer, ByVal strGroupName As String) As Integer
            'This function takes as input a group name and a broker id.  The return value is the group ID that the input values 
            'correspond to (the name of the group and the owner broker record referenced by the broker id).  If either input 
            'is invalid or do not correspond to actual database records, the value '0' will be returned.
            conWebLabLS.Open()
            Dim strDBQuery, strReturnValue As String
            Dim cmdDBQuery As SqlCommand
            Dim intGroupID As Integer

            strDBQuery = "SELECT dbo.rpm_GetGroupID(@BrokerID, @GroupName);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@BrokerID", intBrokerID)
            cmdDBQuery.Parameters.Add("@GroupName", strGroupName)
            intGroupID = CInt(cmdDBQuery.ExecuteScalar())

            conWebLabLS.Close()

            Return intGroupID
        End Function

        Public Function GetBrokerAvgPriority(ByVal intBrokerID As Integer) As Integer
            'This method calculates the average priority for a specified service broker across all of the setups for which that broker has 
            'permission to use.
            conWebLabLS.Open()
            Dim strDBQuery, strOutput As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "SELECT AVG(Priority) FROM dbo.rpm_ListBrokerPermissions(@BrokerID) WHERE ResourceType = 'SETUP';"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@BrokerID", intBrokerID)
            strOutput = cmdDBQuery.ExecuteScalar()

            If IsNumeric(strOutput) Then
                Return CInt(strOutput)
            Else
                Return "-20"
            End If

        End Function

        Public Function GetGroupAvgPriority(ByVal intGroupID As Integer) As Integer
            'This method calculates the average priority for a specified group across all of the setups for which that group has 
            'permission to use.
            conWebLabLS.Open()
            Dim strDBQuery, strOutput As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "SELECT AVG(Priority) FROM dbo.rpm_ListGroupPermissions(@GroupID) WHERE ResourceType = 'SETUP';"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@GroupID", intGroupID)
            strOutput = cmdDBQuery.ExecuteScalar()

            If IsNumeric(strOutput) Then
                Return CInt(strOutput)
            Else
                Return "-20"
            End If

        End Function

        Public Function GetClassAvgPriority(ByVal intClassID As Integer) As Integer
            'This method calculates the average priority for a specified class across all of the setups for which that class has 
            'permission to use.
            conWebLabLS.Open()
            Dim strDBQuery, strOutput As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "SELECT AVG(Priority) FROM dbo.rpm_ListClassPermissions(@ClassID) WHERE ResourceType = 'SETUP';"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@ClassID", intClassID)
            strOutput = cmdDBQuery.ExecuteScalar()

            If IsNumeric(strOutput) Then
                Return CInt(strOutput)
            Else
                Return "-20"
            End If
        End Function

        Public Function GetLabConfig(ByVal intClassID As Integer) As String
            'This method finds the active setups that the specified class has access to and generates an XML labConfig document based on the 
            'results
            Dim conWebLabLS2 As SqlConnection = New SqlConnection("Database=ELVIS_LS;Server=localhost;Integrated Security=true")
            Dim strDBQuery, strXMLOutput, strImgFile As String
            Dim loopIdx As Integer
            Dim cmdDBQuery As SqlCommand
            Dim dtrSetupList, dtrTermList, dtrImageList As SqlDataReader
            conWebLabLS.Open()
            conWebLabLS2.Open()

            'write document header
            strXMLOutput = "<?xml version='1.0' encoding='utf-8' standalone='no' ?><!DOCTYPE labConfiguration SYSTEM 'http://ilab-labview.mit.edu/labserver/xml/labConfiguration.dtd'><labConfiguration lab='MIT ELVIS Weblab' specversion='0.1'>"

            'get available setup list
            strDBQuery = "SELECT setupID, setupName, setupDesc, setupImageLoc FROM dbo.rpm_GetActiveSetupList(@ClassID) ORDER BY setupID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@ClassID", intClassID)
            dtrSetupList = cmdDBQuery.ExecuteReader()

            While dtrSetupList.Read()

                'write experiment setup definitions to the document
                strXMLOutput = strXMLOutput & "<setup id='" & dtrSetupList("setupID") & "'>"
                strXMLOutput = strXMLOutput & "<name>" & dtrSetupList("setupName") & "</name>"
                strXMLOutput = strXMLOutput & "<description>" & dtrSetupList("setupDesc") & "</description>"
                strXMLOutput = strXMLOutput & "<imageURL>" & dtrSetupList("setupImageLoc") & "</imageURL>"

                'get setup terminal information
                strDBQuery = "SELECT termInstrument, termNumber, termName, termXLoc, termYLoc FROM dbo.rpm_GetSetupTerminalInfo(@SetupID) ORDER BY termNumber;"
                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS2)
                cmdDBQuery.Parameters.Add("@SetupID", dtrSetupList("setupID"))
                dtrTermList = cmdDBQuery.ExecuteReader()

                While dtrTermList.Read()
                    'write setup terminal info to document
                    strXMLOutput = strXMLOutput & "<terminal instrumentType='" & dtrTermList("termInstrument") & "' instrumentNumber='" & dtrTermList("termNumber") & "'>"
                    strXMLOutput = strXMLOutput & "<label>" & dtrTermList("termName") & "</label>"
                    strXMLOutput = strXMLOutput & "<pixelLocation><x>" & dtrTermList("termXLoc") & "</x><y>" & dtrTermList("termYLoc") & "</y></pixelLocation>"
                    strXMLOutput = strXMLOutput & "</terminal>"
                End While
                dtrTermList.Close()
                strXMLOutput = strXMLOutput & "</setup>"
            End While

            dtrSetupList.Close()

            'get list of distinct setups used (for image file data inclusion)
            strDBQuery = "SELECT DISTINCT setupImageLoc FROM dbo.rpm_GetActiveSetupList(@ClassID);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@ClassID", intClassID)
            dtrImageList = cmdDBQuery.ExecuteReader()

            While dtrImageList.Read()
                'get image data, encode to Base64 and write to document
                strImgFile = getBase64ImageFile(dtrImageList("setupImageLoc"))

                strXMLOutput = strXMLOutput & "<imageData url='" & dtrImageList("setupImageLoc") & "'>" & strImgFile & "</imageData>"
            End While

            dtrImageList.Close()

            conWebLabLS.Close()
            conWebLabLS2.Close()

            'end document
            strXMLOutput = strXMLOutput & "</labConfiguration>"
            Return strXMLOutput
        End Function


        Public Function GetWSInterfaceStatus() As Boolean
            'This method retuns the status setting of the Lab Server Web Service Interface.  A value of True is returned if the 
            'interfaceis available.  Otherwise a value of false is returned.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "SELECT ws_int_is_active FROM LSSystemConfig WHERE SetupID = '1';"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)

            If cmdDBQuery.ExecuteScalar() = "True" Then
                conWebLabLS.Close()
                Return True
            Else
                conWebLabLS.Close()
                Return False
            End If

        End Function


        Public Function GetLabStatusMessage() As String
            'This method is used bt the GetLabStatus Web Service method to get the administrator defined lab status message.  
            conWebLabLS.Open()
            Dim strDBQuery, strOutput As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader

            strDBQuery = "SELECT lab_status_msg FROM LSSystemConfig WHERE SetupID = '1';"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            dtrDBQuery = cmdDBQuery.ExecuteReader()

            strOutput = "No Message."
            If dtrDBQuery.Read() Then
                If Not (dtrDBQuery("lab_status_msg") Is DBNull.Value Or dtrDBQuery("lab_status_msg") = "") Then
                    strOutput = dtrDBQuery("lab_status_msg")
                End If
            End If

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return strOutput
        End Function


        Public Function CreateServerID() As String
            'Creates a new GUID value for use as the lab server ID.  A text representation (no hyphens, no brackets)of this 
            'guid is then stored in the the appropriate setup record in the LSSystemConfig table of the lab server database 
            'and is also returned as the output of this function.  This method will only complete if there is no lab server 
            'id presently on record.
            conWebLabLS.Open()
            Dim strDBQuery, strOutput As String
            Dim cmdDBQuery As SqlCommand
            Dim newServerID As Guid

            strOutput = ""

            strDBQuery = "SELECT lab_server_id FROM LSSystemConfig WHERE SetupID = '1';"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)

            If cmdDBQuery.ExecuteScalar() Is DBNull.Value Then
                newServerID = Guid.NewGuid()

                strOutput = newServerID.ToString("N")

                strDBQuery = "UPDATE LSSystemConfig SET lab_server_id = @newID WHERE SetupID = '1';"
                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@newID", strOutput)
                cmdDBQuery.ExecuteNonQuery()

            End If

            conWebLabLS.Close()
            Return strOutput
        End Function

        Public Function CreateForeignPasskey(ByVal intBrokerID As Integer) As String
            'This method generates a random, 15 digit number using the dividend of the current system time and the 
            '(broker id + 1) as a seed value.  The resulting value will be the authentication passkey used for the 
            'specified service broker.  This value will be converted to a string before 
            'being passed back to the caller.  Designed to be used with the AddBroker method.
            'conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim sngRanSeed, sngRanVal As Single
            Dim lngPasskey, lngDateVal As Long

            'Obtain seed value
            lngDateVal = Now().Ticks()
            sngRanSeed = CSng(lngDateVal / (intBrokerID + 1))

            VBMath.Randomize()
            sngRanVal = VBMath.Rnd(sngRanSeed)

            'scale to 15 digits
            lngPasskey = CLng((1000000000000000 - 100000000000000) * sngRanVal + 100000000000000)

            Return CStr(lngPasskey)
        End Function

        Public Function RegisterServerPasskey(ByVal intBrokerID As Integer, ByVal strPasskey As String)
            'This method registers a service broker supplied passkey with the lab server.  Specifically, 
            'this takes as input a passkey generated by a given service broker and the local identifier
            'for that service broker.  This method records that passkey in the appropriate broker record as
            'a server passkey.  This new passkey will be used to authenticate the lab server with the 
            'specified service broker whenever the lab server makes web service calls to said broker.

            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "UPDATE Brokers SET server_passkey = @Passkey, date_modified = GETDATE() WHERE broker_id = @BrokerID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Passkey", strPasskey)
            cmdDBQuery.Parameters.Add("@BrokerID", intBrokerID)
            cmdDBQuery.ExecuteNonQuery()

            conWebLabLS.Close()
        End Function


        Public Function AuthenticateBroker(ByVal strIdentifier As String, ByVal strPassKey As String) As Integer
            'This method checks a supplied identifier/passkey pair and determines if that pair corresponds to an 
            'active service broker account.  If there is a correspondence, the lab server internal broker id is returned from
            'the database to the caller.  Otherwise a value of '-1' will be returned, indicating that the agent supplying
            'the credentials to the interface does not have an active account on the system and should not be serviced 
            'by the current request.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim intOutput As Integer
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "SELECT dbo.rpm_AuthenticateBrokerCredentials(@Identifier, @PassKey);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@Identifier", strIdentifier)
            cmdDBQuery.Parameters.Add("@PassKey", strPassKey)

            intOutput = CInt(cmdDBQuery.ExecuteScalar())
            conWebLabLS.Close()

            Return intOutput
        End Function

        Private Function getBase64ImageFile(ByVal strImgUrl As String) As String
            'This method takes as input an image URL, which is loaded and then converted to a Base64 encoded string.  If the 
            'image load or encoding processes fail, an empty string is returned.  This method is designed for use with 
            'GetLabConfiguration.
            Dim objRequest As HttpWebRequest
            Dim objResponse As HttpWebResponse

            Try
                objRequest = CType(WebRequest.Create(strImgUrl), HttpWebRequest)
                objRequest.Method = "GET"

                objResponse = objRequest.GetResponse()

                Return Convert.ToBase64String(ConvertStreamToByteBuffer(objResponse.GetResponseStream()))

            Catch When Err.Number <> 0 'error reading or encoding file
                Return ""
            End Try

        End Function

        Private Function ConvertStreamToByteBuffer(ByVal stmInput As Stream) As Byte()
            'This method converts a stream object to a Byte array.  This is a helper method for getBase64ImageFile.
            Dim intByteVal As Integer
            Dim stmTemp As MemoryStream = New MemoryStream

            intByteVal = stmInput.ReadByte()

            Do While intByteVal <> -1
                stmTemp.WriteByte(CByte(intByteVal))

                intByteVal = stmInput.ReadByte()
            Loop

            Return stmTemp.ToArray()
        End Function

    End Class

End Namespace
