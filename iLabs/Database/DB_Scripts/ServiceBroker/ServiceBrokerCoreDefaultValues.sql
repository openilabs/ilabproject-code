/* created by Charu. Last Modified 12/16/2004 */

/* ENTERING DEFAULT VALUES IN TABLES */

/* AUTHENTICATION_TYPES */
INSERT INTO Authentication_Types(description) VALUES ('Native');
INSERT INTO Authentication_Types(description) VALUES ('Kerberos_MIT');

/* CLIENT_TYPES */
INSERT INTO Client_Types(description) VALUES ('Batch Applet');
INSERT INTO Client_Types(description) VALUES ('Batch Redirect');
INSERT INTO Client_Types(description) VALUES ('Interactive Applet');
INSERT INTO Client_Types(description) VALUES ('Interactive Redirect');

/* DEFAULT CLIENT */
SET IDENTITY_INSERT Lab_Clients ON
INSERT INTO Lab_Clients(Client_ID, Client_Type_ID, NeedsScheduling, NeedsESS, IsReentrant, Loader_Script,Client_GUID, Lab_Client_Name)
values (0,1,0,0,0,' ','0','No Client');
SET IDENTITY_INSERT Lab_Clients OFF
DBCC CHECKIDENT (Lab_Clients, RESEED);

/* FUNCTIONS */
INSERT INTO Functions (Function_Name, Description) VALUES ('addMember', 'allows one to add members to a group');
INSERT INTO Functions (Function_Name, Description) VALUES ('administerGroup', 'allows one to administer a group');
INSERT INTO Functions (Function_Name, Description) VALUES ('anyone', 'anyone can execute the method');
INSERT INTO Functions (Function_Name, Description) VALUES ('owner', 'only the owner of the designated item may execute the method');
INSERT INTO Functions (Function_Name, Description) VALUES ('readExperiment', 'allows one to read an experiment');
INSERT INTO Functions (Function_Name, Description) VALUES ('trusted', 'method only called during the execution of trusted administrative code');
INSERT INTO Functions (Function_Name, Description) VALUES ('useLabClient', 'allows one to use a lab client');
INSERT INTO Functions (Function_Name, Description) VALUES ('useLabServer', 'allows one to use a lab server');
INSERT INTO Functions (Function_Name, Description) VALUES ('writeExperiment', 'allows one to write to an experiment');
INSERT INTO Functions (Function_Name, Description) VALUES ('createExperiment', 'allows one to create an experiment');
INSERT INTO Functions (Function_Name, Description) VALUES ('useLabScheduling', 'allows one to configure a Lab Scheduing Service');
INSERT INTO Functions (Function_Name, Description) VALUES ('useUserScheduling', 'allows one to Schdule an experiment');
/* FUNCTIONS which are also TICKET TYPES */
/* ADMINISTER_PA Ticket Types*/
INSERT INTO Functions (Function_Name, Description) VALUES ('ADMINISTER LSS','Administer LSS');
INSERT INTO Functions (Function_Name, Description) VALUES ('ADMINISTER ESS', 'Administer ESS');
INSERT INTO Functions (Function_Name, Description) VALUES ('ADMINISTER LS','Administer LS');
INSERT INTO Functions (Function_Name, Description) VALUES ('ADMINISTER USS', 'Administer USS');
/*MANAGE_PA Ticket Types*/
INSERT INTO Functions (Function_Name, Description) VALUES ('MANAGE LAB','Manage LAB');
INSERT INTO Functions (Function_Name, Description) VALUES ('ADMINISTER EXPERIMENT', 'Administer Experiment');
INSERT INTO Functions (Function_Name, Description) VALUES ('MANAGE USS GROUP', 'USS Manage Group');
INSERT INTO Functions (Function_Name, Description) VALUES ('REQUEST RESERVATION','Request Reservation');

/* GROUP_TYPES */
SET IDENTITY_INSERT Group_Types ON
INSERT INTO Group_Types(Group_Type_ID, description) VALUES (0,'Non-existent Group');
INSERT INTO Group_Types(Group_Type_ID, description) VALUES (1,'Regular Group');
INSERT INTO Group_Types(Group_Type_ID, description) VALUES (2,'Request Group');
INSERT INTO Group_Types(Group_Type_ID, description) VALUES (3,'Course Staff Group');
INSERT INTO Group_Types(Group_Type_ID, description) VALUES (4,'Service Administration Group');
INSERT INTO Group_Types(Group_Type_ID, description) VALUES (5,'Built-in Group');
SET IDENTITY_INSERT Group_Types OFF
DBCC CHECKIDENT (GROUP_TYPES, RESEED);

/* GROUPS & CORRESPONDING AGENTS*/
SET IDENTITY_INSERT Agents ON
INSERT INTO Agents (Agent_ID,Agent_Name, Is_Group) VALUES (0,'Group not assigned', 1);
INSERT INTO Groups(Group_ID, Group_Name, description, group_type_ID) VALUES (0, 'Group not assigned','If a groupID does not exist. This is an illegal group.',0);
SET IDENTITY_INSERT Agents OFF
DBCC CHECKIDENT (AGENTS, RESEED);

BEGIN
DECLARE @Agent_ID NUMERIC
DECLARE @Parent_Group_ID NUMERIC

INSERT INTO Agents (Agent_Name, Is_Group) VALUES ('ROOT', 1);
SELECT @Agent_ID = (SELECT ident_current('Agents'));
INSERT INTO Groups(Group_ID, Group_Name, description, group_type_ID) VALUES (@Agent_ID, 'ROOT','Root Group', 5);

INSERT INTO Agents (Agent_Name, Is_Group) VALUES ('NewUserGroup', 1);
SELECT @Agent_ID = (SELECT ident_current('Agents'));
SELECT @Parent_Group_ID = (SELECT Group_ID FROM Groups WHERE Group_Name = 'ROOT');
INSERT INTO Groups(Group_ID, Group_Name, description, group_type_ID) VALUES (@Agent_ID, 'NewUserGroup','New registered users who have not been moved to any group yet', 5);
INSERT INTO Agent_Hierarchy (Agent_ID, Parent_Group_ID) VALUES(@Agent_ID, @Parent_Group_ID);

INSERT INTO Agents (Agent_Name, Is_Group) VALUES ('OrphanedUserGroup', 1);
SELECT @Agent_ID = (SELECT ident_current('Agents'));
SELECT @Parent_Group_ID = (SELECT Group_ID FROM Groups WHERE Group_Name = 'ROOT');
INSERT INTO Groups(Group_ID, Group_Name, description, group_type_ID) VALUES (@Agent_ID,'OrphanedUserGroup','Users who no longer belong to any group',5);
INSERT INTO Agent_Hierarchy (Agent_ID, Parent_Group_ID) VALUES(@Agent_ID, @Parent_Group_ID);

INSERT INTO Agents (Agent_Name, Is_Group) VALUES ('SuperUserGroup', 1);
SELECT @Agent_ID = (SELECT ident_current('Agents'));
SELECT @Parent_Group_ID = (SELECT Group_ID FROM Groups WHERE Group_Name = 'ROOT');
INSERT INTO Groups(Group_ID, Group_Name, description, group_type_ID) VALUES (@Agent_ID,'SuperUserGroup','Administrators',5);
INSERT INTO Agent_Hierarchy (Agent_ID, Parent_Group_ID) VALUES(@Agent_ID, @Parent_Group_ID);

DBCC CHECKIDENT (AGENTS, RESEED);

END

UPDATE GROUPS SET associated_group_id = 0
DBCC CHECKIDENT (AGENTS, RESEED, 10) ;



/* MESSAGE_TYPES */
INSERT INTO Message_Types(description) VALUES ('Lab');
INSERT INTO Message_Types(description) VALUES ('Group');
INSERT INTO Message_Types(description) VALUES ('System');

/* QUALIFIER_TYPES */
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (0,'Null Qualifier Type');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (1,'Root');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (2,'Lab Client');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (3,'Lab Server');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (4,'Experiment');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (5,'Group');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (6,'Experiment Collection');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (7,'Lab Scheduling');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (8,'User Scheduling');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (9,'Service Broker');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (10,'Storage Server');
INSERT INTO Qualifier_Types(Qualifier_Type_ID, description) VALUES (11,'Resource Mapping');


/* QUALIFIERS & QUALIFIER HIERARCHY */
DBCC CHECKIDENT (QUALIFIERS, RESEED, 0) ;
INSERT INTO Qualifiers(Qualifier_Type_ID, Qualifier_Reference_ID, Qualifier_Name) VALUES (0,0, 'Null Qualifier'); 
INSERT INTO Qualifiers(Qualifier_Type_ID, Qualifier_Reference_ID, Qualifier_Name) VALUES (1, 0,'ROOT');

BEGIN
DECLARE @GroupReference int

SELECT  @GroupReference = (SELECT Group_ID FROM Groups where Group_Name='SuperUserGroup')
INSERT INTO Qualifiers(Qualifier_Type_ID, Qualifier_Reference_ID, Qualifier_Name) VALUES (5,@GroupReference ,'SuperUserGroup'); 

SELECT  @GroupReference = (SELECT Group_ID FROM Groups where Group_Name='NewUserGroup')
INSERT INTO Qualifiers(Qualifier_Type_ID, Qualifier_Reference_ID, Qualifier_Name) VALUES (5,@GroupReference,'NewUserGroup'); 

SELECT  @GroupReference = (SELECT Group_ID FROM Groups where Group_Name='OrphanedUserGroup')
INSERT INTO Qualifiers(Qualifier_Type_ID, Qualifier_Reference_ID, Qualifier_Name) VALUES (5,@GroupReference,'OrphanedUserGroup');

DBCC CHECKIDENT (QUALIFIERS, RESEED, 100) ;

END
BEGIN
DECLARE @qualID int
DECLARE @parentid INT 
/* Orphaned User group is a member of New User Group */
SELECT @qualID =(select qualifier_id from Qualifiers where qualifier_name = 'OrphanedUserGroup')
SELECT @parentid =(select qualifier_id from Qualifiers where qualifier_name = 'NewUserGroup')
INSERT INTO Qualifier_Hierarchy (Qualifier_ID, Parent_Qualifier_ID) VALUES (@qualID, @parentid)
/*Super User group is a member of Root */
SELECT @qualID =(select qualifier_id from Qualifiers where qualifier_name = 'SuperUserGroup')
SELECT @parentid =(select qualifier_id from Qualifiers where qualifier_name = 'ROOT')
INSERT INTO Qualifier_Hierarchy (Qualifier_ID, Parent_Qualifier_ID) VALUES (@qualID, @parentid)
END

/* USERS & CORRESPONDING AGENTS & PRINCIPALS */
/* Default SuperUser password is ilab */
INSERT INTO Agents (Agent_Name, Is_Group) VALUES ('superUser', 0);
select @Agent_ID = (select ident_current('Agents'))
INSERT INTO Users (User_ID, User_Name, First_Name, Last_Name, Email, Affiliation, password, signup_reason) VALUES
(@Agent_ID, 'superUser', 'Super', 'User', 'ilab-debug@mit.edu', 'Other', '3759F4FF14D8494DF3B58671FF9251A9D0C41D54', 'Default Value');

INSERT INTO Principals (User_ID, Principal_String, Auth_Type_ID) VALUES (@Agent_ID, 'superUser', 1);

SELECT @Parent_Group_ID = (SELECT Group_ID FROM Groups WHERE Group_Name = 'SuperUserGroup');
INSERT INTO Agent_Hierarchy (Agent_ID, Parent_Group_ID) VALUES (@Agent_ID, @Parent_Group_ID);


/*ReSourceMappingTypes*/
SET IDENTITY_INSERT ResourceMappingTypes ON
INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (1,'PROCESS_AGENT','Process Agent');
INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (2,'CLIENT','Client');
INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (3,'RESOURCE_MAPPING','Resource Mapping');
INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (4,'STRING','String');
INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (5,'TICKET_TYPE','Ticket Type');
INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (6,'GROUP','Group');
INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (7,'RESOURCE_TYPE', 'Resource Type');

SET IDENTITY_INSERT ResourceMappingTypes OFF
DBCC CHECKIDENT (ResourceMappingTypes, RESEED);
GO

/*ReSourceMapTypes*/
--SET IDENTITY_INSERT ResourceMappingTypes ON
--INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (1,'GROUP','Group');
--INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (2,'PROCESS_AGENT','Process Agent');
--INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (3,'CLIENT','Client');
--INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (4,'RESOURCE_MAPPING','Resource Mapping');
--INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (5,'TICKET_TYPE','Ticket Type');
--INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (6,'RESOURCE_TYPE', 'Resource Type');
--INSERT INTO ResourceMappingTypes(Type_ID, Type_Name, Description) VALUES (7,'STRING','String');

--SET IDENTITY_INSERT ResourceMappingTypes OFF
GO