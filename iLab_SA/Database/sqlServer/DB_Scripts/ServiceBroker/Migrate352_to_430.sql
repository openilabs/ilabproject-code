/******
	$Id$
	
Note this does not compact or change primary keys for services & clients! 
Pruning of Services, Clients and users may be done by changing the 
where clauses of the Wrk table loaders.

Restore a backup of your current 3.5.2 database as i352_ISB on the 
same machine as a the target 4.3.x version database that has 
only had the SQL scripts run on it. Create a connection to the target database,
modify the select statements for the xxxWrk tables to prune servers, 
clients and users. 
Then run this script and be sure to check the results.

Existing ProcessAgents and Batch Lab Servers should continue to be able 
to comunicate using the original DomainCredientials. 
Please note the if there are updated ProcessAgent services they will not have had 
DomainCredientials installed, you will have either have to exclude them from 
this script and register them using the standard process or
manually install the required coupons and database records.
This process does not create relationships between the different services 
including scheduling and Experiment storage services.

Migrating groups is not supported in this script. By default users 
are placed in the New User Group

**************************************************/


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[paWrk]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[paWrk]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[clientWrk]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[clientWrk]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[userWrk]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[userWrk]
GO

create Table paWrk(
 id int,
 paguid varchar(50)
 )
 go

 create Table clientWrk(
 id int,
 guid varchar(50)
 )
 go

create Table userWrk(
 user_ID int,
 new_id int null,
 userName nvarchar(256)
 )
GO


insert into paWrk (id,paguid)
select agent_ID,Agent_Guid from [i352_ISB].[dbo].ProcessAgent

/** Replace WHERE Clause with the service IDs that you want to migrate
where Agent_ID in (10,15,16,17,18,19,20,21,22,24,25,30,31,32)
**/
go

insert into clientWrk (id,guid)
select Client_ID,Client_Guid from [i352_ISB].[dbo].Lab_Clients

/** Replace WHERE Clause with the client IDs that you want to migrate
where Client_ID in (2,3,4,5,6,7,8,9,10,11,12,16,17,18)
**/
go

insert into userWrk(user_ID,userName)
select user_ID, user_Name from [i352_ISB].[dbo].[Users]
 
/** Replace WHERE Clause with the users that you want to migrate
*** Note using email address will move all accounts that share that address
where user_name 
  in ('pbailey','kirky','alamo','hardison','jud','aiakinwande',
  'DAAntoniadis','lang')
  or Email in ('fonstad@MIT.EDU','jingkong@MIT.EDU','gauss@dei.unipd.it',
 'giuseppe.martini@unipv.it', 'dtsouk@central.ntua.gr')
 **/
 
-- Insert Issued Coupons
SET IDENTITY_INSERT IssuedCoupon ON;
go
Insert into IssuedCoupon (Coupon_ID,Cancelled,Passkey)
Select Coupon_ID,Cancelled,Passkey
FROM i352_ISB.dbo.IssuedCoupon where Coupon_ID IN
(Select IdentIn_ID from   i352_ISB.dbo.ProcessAgent 
where Agent_ID in (select id from pawrk));
go
Insert into IssuedCoupon (Coupon_ID,Cancelled,Passkey)
Select Coupon_ID,Cancelled,Passkey
FROM i352_ISB.dbo.IssuedCoupon where Coupon_ID IN
(Select IdentOut_ID from   i352_ISB.dbo.ProcessAgent
where Agent_ID in (select id from pawrk));
go
SET IDENTITY_INSERT IssuedCoupon OFF;
DBCC CHECKIDENT (IssuedCoupon, RESEED);
go

-- insert Active coupons
BEGIN
declare @domainguid varchar(50);
 set @domainguid = (select agent_Guid 
 from i352_ISB.dbo.ProcessAgent
 where Self = 1 and Retired = 0);
 
Insert into Coupon (Coupon_ID,Cancelled,Issuer_Guid,Passkey)
Select Coupon_ID,Cancelled,@domainguid, Passkey
FROM i352_ISB.dbo.IssuedCoupon where Coupon_ID IN
(Select IdentIn_ID from   i352_ISB.dbo.ProcessAgent 
where Agent_ID in (select id from pawrk));

Insert into Coupon (Coupon_ID,Cancelled,Issuer_Guid,Passkey)
Select Coupon_ID,Cancelled,@domainguid,Passkey
FROM i352_ISB.dbo.IssuedCoupon where Coupon_ID IN
(Select IdentOut_ID from   i352_ISB.dbo.ProcessAgent
where Agent_ID in (select id from pawrk));
END
go


-- Do the services
SET IDENTITY_INSERT ProcessAgent ON;
insert into ProcessAgent (agent_ID,retired,self,ProcessAgent_Type_ID,
IdentIn_ID,IdentOut_ID,Issuer_Guid,Domain_Guid,Agent_Guid, Agent_Name,
WebService_URL,Codebase_URL,Info_URL,Contact_Email,Bug_Email,Location,Description)
Select agent_ID,retired,self,ProcessAgent_Type_ID,
IdentIn_ID,IdentOut_ID,Issuer_Guid,Domain_Guid,Agent_Guid, Agent_Name,
WebService_URL,Codebase_URL,Info_URL,Contact_Email,Bug_Email,Location,Description
 from i352_ISB.dbo.ProcessAgent
 where Agent_ID in (select id from pawrk);
SET IDENTITY_INSERT ProcessAgent OFF;
DBCC CHECKIDENT (ProcessAgent, RESEED);

insert into AdminURLs (ProcessAgentID,Ticket_Type_ID,AdminURL)
select ProcessAgentID,Ticket_Type_ID,AdminURL
from i352_ISB.dbo.AdminUrls where processAgentID in (select id from pawrk);
GO

-- set the deault Authority to match the ServiceBroker
update Authority set Authority_Guid = p.Agent_Guid,
	Authority_Name = p.Agent_Name, Authority_URL= p.Codebase_URL, 
	Contact_Email = p.Contact_Email, Bug_Email = p.Bug_Email, 
	Location = p.Location, Description= p.Description
	from (select Agent_Guid,Agent_Name, Codebase_URL, 
	contact_Email, Bug_Email,location, Description
	from ProcessAgent where Self =1) as p
	where Authority_ID = 0
GO

-- Do the clients
SET IDENTITY_INSERT Lab_Clients ON;

insert into Lab_Clients (Client_ID,Client_Type_ID,NeedsScheduling,NeedsESS,
IsReentrant,Loader_Script,Long_Description,Contact_Email,Short_Description,
Version,Contact_First_Name,Contact_Last_Name,Date_Created,Client_Guid,
Lab_Client_Name,Notes,Documentation_URL)
Select Client_ID,Client_Type_ID,NeedsScheduling,NeedsESS,
IsReentrant,Loader_Script,Long_Description,Contact_Email,Short_Description,
Version,Contact_First_Name,Contact_Last_Name,Date_Created,Client_Guid,
Lab_Client_Name,Notes,Documentation_URL 
from i352_ISB.dbo.Lab_Clients where Client_ID in (select id from clientwrk);
SET IDENTITY_INSERT Lab_Clients OFF;
DBCC CHECKIDENT (Lab_Clients, RESEED);
GO

-- migrate Qualifiers for Severs and Clients
SET IDENTITY_INSERT Qualifiers ON;
go
insert into Qualifiers ( Qualifier_ID, Qualifier_Type_ID,
 Qualifier_Reference_ID, Date_Created, Qualifier_Name)
 select  Qualifier_ID, Qualifier_Type_ID,
 Qualifier_Reference_ID, Date_Created, Qualifier_Name
from i352_ISB.dbo.Qualifiers
where Qualifier_Type_ID = 3 
and Qualifier_Reference_ID in (select id from paWrk)
go
insert into Qualifiers ( Qualifier_ID, Qualifier_Type_ID,
 Qualifier_Reference_ID, Date_Created, Qualifier_Name)
 select  Qualifier_ID, Qualifier_Type_ID,
 Qualifier_Reference_ID, Date_Created, Qualifier_Name
from i352_ISB.dbo.Qualifiers
where Qualifier_Type_ID = 2 
and Qualifier_Reference_ID in (select id from clientWrk)
go
SET IDENTITY_INSERT Qualifiers OFF;
DBCC CHECKIDENT (Qualifiers, RESEED);
GO

insert into Qualifier_Hierarchy (Qualifier_ID, Parent_Qualifier_ID)
select Qualifier_ID, Parent_Qualifier_ID
from i352_ISB.dbo.Qualifier_Hierarchy
where Qualifier_ID 
in (Select Qualifier_ID from Qualifiers where Qualifier_ID >100)
go

-- migrate client to server mapping only if both server & client have been included
insert into Lab_Server_To_Client_Map (Client_ID,Agent_ID, Display_Order)
select Client_ID,Agent_ID, Display_Order 
from  [i352_ISB].[dbo].Lab_Server_To_Client_Map
where Client_ID in (select id from clientwrk)
AND Agent_ID in (select id from pawrk);
GO

insert into Client_Info (Client_ID,Display_Order,Info_URL,Info_Name,Description)
select Client_ID,Display_Order,Info_URL,Info_Name,Description
from  [i352_ISB].[dbo].Client_Info where Client_ID in (select id from clientwrk);
GO

/****** Script for MoveUsers  
This will use the list of users and generate new user_IDs
******/
insert into users (Lock_User,Auth_ID,Date_Created,Xml_Extension,Password,User_Name
      ,First_Name,Last_Name,Email,Affiliation,Signup_Reason)
SELECT Lock_User,0,Date_Created,Xml_Extension,Password,User_Name
      ,First_Name,Last_Name,Email,Affiliation,Signup_Reason
  FROM [i352_ISB].[dbo].[Users] u,[i352_ISB].[dbo].[Agents] a
  where  USER_ID in (select User_ID from userWrk) and user_Name != 'superUser' AND u.User_ID = a.Agent_ID
  GO
  
  update userWrk set new_ID = u.User_ID 
  from userWrk w,Users u  where w.UserName = u.User_name
  GO
  
  insert into Principals (Auth_Type_ID, User_ID, Principal_String)
  select 1, USER_ID, USER_NAME 
  From Users where USER_NAME != 'superUser'
  GO
  
  -- By default all users into NewUserGroup
  insert into User_Groups (group_ID,User_ID)
  select 2,User_ID
  From Users where USER_NAME != 'superUser'
  GO
  

-- Assign client _Items using the original client_ID and the new user_ID
  insert into Client_Items (User_ID,Client_ID,Item_Name,Item_Value)
  select w.new_ID,Client_ID, Item_Name, Item_Value
  from [i352_ISB].[dbo].Client_Items ci, userwrk w
  Where client_id in (select id from clientWrk)
  AND Item_Name like 'save%'
  AND ci.User_ID in (select user_ID from userWrk)
  and w.user_id = ci.user_id
  GO
