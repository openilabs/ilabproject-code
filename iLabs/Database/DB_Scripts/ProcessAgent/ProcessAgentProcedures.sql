-- $Id: ProcessAgentProcedures.sql,v 1.20 2008/03/11 19:10:40 pbailey Exp $
--
--
-- Ticketing support these procedures implement the common Ticketing database methods.
-- The entries in the Coupon and Ticket table only contain data from an external ServiceBroker or Agent.
-- This is also the case when these tables are part of a ServiceBroker database.
-- Use of the ProcessAgent table should be the same for ServiceBrokers and ProcessAgents.
--
--
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[toLongArray]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[toLongArray]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[toIntArray]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[toIntArray]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[InsertTicket]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[InsertTicket]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AuthenticateCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AuthenticateCoupon]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CancelCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[CancelCoupon]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteCoupon]
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CancelTicket]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[CancelTicket]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteTicket]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteTicket]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CancelTicketByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[CancelTicketByID]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteTicketByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteTicketByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[InsertCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[InsertCoupon]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[InsertProcessAgent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[InsertProcessAgent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UpdateProcessAgent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UpdateProcessAgent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetCoupon]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetCouponCollectionCount]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetCouponCollectionCount]
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetIdentInCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetIdentInCoupon]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetIdentOutCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetIdentOutCoupon]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SetIdentInCouponID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SetIdentInCouponID]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SetIdentOutCouponID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SetIdentOutCouponID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentInfoByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentInfoByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentsInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentsInfo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentIDsByType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentIDsByType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentIDsByTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentIDsByTypeID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentInfos]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetPRocessAgentInfos]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentInfo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcesAgentInfoByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentInfoByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentInfoByType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentInfoByType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentInfoByInCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentInfoByInCoupon]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentName]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentNameWithType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentNameWithType]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentTagByGuid]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentTagByGuid]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentTagByGuidWithType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentTagByGuidWithType]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentTags]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentTags]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentTagsWithType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentTagsWithType]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentTagsWithTypeById]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentTagsWithTypeById]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentTagsByTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentTagsByTypeID]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentTagsByType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentTagsByType]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetDomainProcessAgentTags]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetDomainProcessAgentTags]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetDomainProcessAgentTagsWithType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetDomainProcessAgentTagsWithType]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetDomainProcessAgentTagsByType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetDomainProcessAgentTagsByType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentInfoByOutCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentInfoByOutCoupon]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[InsertCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[InsertCoupon]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[InsertProcessAgent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[InsertProcessAgent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WriteSelfProcessAgent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WriteSelfProcessAgent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[InsertTicket]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[InsertTicket]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ProcessAgentTypeExists]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ProcessAgentTypeExists]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentTypeID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgents]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgents]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentsByType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentsByType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentURL]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentURL]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetProcessAgentURLbyID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetProcessAgentURLbyID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetTicket]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetTicket]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetExpiredTickets]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetExpiredTickets]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetTicketsByCoupon]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetTicketsByCoupon]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetTicketIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetTicketIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetTicketByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetTicketByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetTicketID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetTicketID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetTickets]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetTickets]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetTicketsByType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetTicketsByType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveTicketTypes]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveTicketTypes]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UpdateDomainGuid]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UpdateDomainGuid]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE FUNCTION dbo.toLongArray
(
	@list varchar(8000)
)
RETURNS @array TABLE(lid bigint)
AS
BEGIN
DECLARE @tempList table(id BigInt)

	DECLARE @idStr varchar(100), @Pos int

	SET @list = LTRIM(RTRIM(@list))+ ','
	SET @Pos = CHARINDEX(',', @list, 1)

	IF REPLACE(@list, ',', '') <> ''
	BEGIN
		WHILE @Pos > 0
		BEGIN
			SET @idStr = LTRIM(RTRIM(LEFT(@list, @Pos - 1)))
			IF @idStr <> ''
			BEGIN
				INSERT INTO @tempList (ID) VALUES (CAST(@idStr AS bigint)) --Use Appropriate conversion
			END
			SET @list = RIGHT(@list, LEN(@list) - @Pos)
			SET @Pos = CHARINDEX(',', @list, 1)

		END
	END	

	insert @array SELECT ID FROM @tempList
RETURN	
END

go

CREATE FUNCTION dbo.toIntArray
(
	@list varchar(8000)
)
RETURNS @array TABLE(lid int)
AS
BEGIN
DECLARE @tempList table(id Int)

	DECLARE @idStr varchar(100), @Pos int

	SET @list = LTRIM(RTRIM(@list))+ ','
	SET @Pos = CHARINDEX(',', @list, 1)

	IF REPLACE(@list, ',', '') <> ''
	BEGIN
		WHILE @Pos > 0
		BEGIN
			SET @idStr = LTRIM(RTRIM(LEFT(@list, @Pos - 1)))
			IF @idStr <> ''
			BEGIN
				INSERT INTO @tempList (ID) VALUES (CAST(@idStr AS int)) --Use Appropriate conversion
			END
			SET @list = RIGHT(@list, LEN(@list) - @Pos)
			SET @Pos = CHARINDEX(',', @list, 1)

		END
	END	

	insert @array SELECT ID FROM @tempList
RETURN	
END

go


CREATE PROCEDURE CancelCoupon
@issuerGUID varchar (50),
@couponID bigint
 AS

BEGIN TRANSACTION
begin
update Coupon set Cancelled = 1 where Coupon_ID = @couponID AND Issuer_GUID = @issuerGUID AND Cancelled = 0
if (@@rowcount = 0)
goto on_error
if (@@error>0)
goto on_error
update Ticket set Cancelled =1 where Coupon_ID = @couponID AND Issuer_GUID = @issuerGUID
if (@@error>0)
goto on_error
end
COMMIT TRANSACTION
select 1
return

on_error:
ROLLBACK TRANSACTION
select 0
return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE DeleteCoupon
@issuerGUID varchar (50),
@couponID bigint
 AS
declare @ticketCount int

select @ticketCount = COUNT(ticket_ID) from Ticket where Coupon_ID = @couponID AND Issuer_GUID = @issuerGUID
if (@ticketCount > 0)
GOTO hasTickets

delete from coupon where Coupon_ID = @couponID AND Issuer_GUID = @issuerGUID
if (@@rowcount = 0)
goto on_error
if (@@error>0)
goto on_error

return 1

on_error:
return 0

hasTickets:
return -1

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Inserts a coupon
CREATE PROCEDURE InsertCoupon
@couponID bigint,
@issuerGUID varchar (50),
@passKey varchar(100),
@cancelled  bit = 0

 AS

insert into Coupon(coupon_ID, issuer_GUID, Passkey, Cancelled)
values(@couponID,@issuerGUID, @passKey, @cancelled)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


-- This version expects all coupons to be in the Coupon Table, 
-- may be over written in TicketIssuer database,
CREATE PROCEDURE AuthenticateCoupon
@couponID bigint,
@issuerGUID varchar (50),
@passKey varchar(100)

 AS

select coupon_ID from Coupon 
where EXISTS ( SELECT * Where coupon_ID=@couponID AND issuer_GUID=@issuerGUID AND passkey = @passKey AND cancelled = 0)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetCoupon 
@issuerGUID varchar(50),
@couponID bigint

 AS

select Cancelled, Passkey
 from Coupon
 where Coupon_ID = @couponID AND Issuer_GUID = @issuerGUID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetCouponCollectionCount
	@couponID BigInt,
	@guid varchar (50)
AS
	select count(ticket_ID) from Ticket where coupon_ID = @couponID and issuer_guid = @guid
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetIdentInCoupon 
@agentGUID varchar(50)

AS
select cancelled,Coupon_ID,issuer_Guid,passkey
from Coupon
where Coupon_ID = (Select identIn_ID from ProcessAgent  where agent_Guid = @agentGUID)
AND Issuer_Guid = (Select issuer_GUID from ProcessAgent where agent_Guid = @agentGUID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE GetIdentOutCoupon 
@agentGUID varchar(50)

AS


select cancelled,Coupon_ID,issuer_Guid,passkey
from Coupon
where Coupon_ID = (Select identOut_ID from ProcessAgent  where agent_Guid = @agentGUID)
AND Issuer_Guid = (Select issuer_GUID from ProcessAgent where agent_Guid = @agentGUID)


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE SetIdentInCouponID 
@agentGUID varchar(50),
@id bigint
AS

update ProcessAgent set identIn_ID=@id where agent_Guid = @agentGUID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE SetIdentOutCouponID 
@agentGUID varchar(50),
@id bigint
AS

update ProcessAgent set identOut_ID=@id where agent_Guid = @agentGUID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE GetProcessAgentName
@agentID int
 AS
select Agent_Name from ProcessAgent
where agent_ID = @agentID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE GetProcessAgentNameWithType
@agentID int
 AS
select ProcessAgent_Type.Short_Name, Agent_Name

from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)

where agent_ID = @agentID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE GetProcessAgentTagByGuid
@guid varchar(50)
 AS
select agent_ID, Agent_Name from ProcessAgent
where agent_Guid = @guid

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE GetProcessAgentTagByGuidWithType
@guid varchar(50)
 AS
select agent_ID, ProcessAgent_Type.Short_Name, Agent_Name
from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
where agent_Guid = @guid

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


--
-- ProcessAgent  the detailed information about a service
--

CREATE PROCEDURE GetProcessAgentTags
 AS
select agent_ID, Agent_Name from ProcessAgent

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentTagsWithType
 AS
select agent_ID, ProcessAgent_Type.Short_Name, Agent_Name

from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentTagsWithTypeById
@agentID int
 AS
select agent_ID, ProcessAgent_Type.Short_Name, Agent_Name

from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
where agent_ID = @agentID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentTagsByTypeID

@typeID int

 AS

select agent_ID, Agent_Name from ProcessAgent
 where ProcessAgent_Type_id = @typeID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetProcessAgentTagsByType

@type varchar(100)

 AS

select agent_ID, Agent_Name from ProcessAgent
 where ProcessAgent_type_id = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@type )))


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetDomainProcessAgentTags
@domain varchar(50)
 AS
select agent_ID, Agent_Name from ProcessAgent
where domain_guid = @domain

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetDomainProcessAgentTagsWithType
@domain varchar(50)
 AS
select agent_ID, ProcessAgent_Type.Short_Name, Agent_Name

from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
where domain_guid = @domain
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetDomainProcessAgentTagsByType

@type varchar(100),
@domain varchar(50)

 AS

select agent_ID, Agent_Name from ProcessAgent
 where ProcessAgent_type_id = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@type )))
and domain_Guid=@domain


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE ProcessAgentTypeExists

@type varchar(100)

 AS

select ProcessAgent_Type_ID from ProcessAgent_Type where ProcessAgent_Type.Description = @type

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentTypeID

@type varchar(100)

 AS

select ProcessAgent_Type_ID from ProcessAgent_Type where ProcessAgent_Type.Description = @type

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE InsertProcessAgent 
@processAgentType varchar(100),
@guid varchar(50),
@name varchar(100),
@codeBaseURL varchar(256) = null,
@webServiceURL varchar(256),
@issuer varchar(50) = null,
@inID bigint = null,
@outID bigint = null,
@domain varchar (50)
AS
Declare @processAgentTypeID int 

select @processAgentTypeID = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@processAgentType )))
insert into ProcessAgent( Agent_GUID,domain_Guid, Agent_Name,ProcessAgent_Type_ID, Codebase_URL, WebService_URL, Issuer_GUID, IdentIn_ID,IdentOut_ID)
values ( @guid, @domain, @name, @processAgentTypeID,  @codeBaseURL, @webServiceURL, @issuer, @inID, @outID )
select ident_current('ProcessAgent')

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

Create PROCEDURE WriteSelfProcessAgent 
@processAgentType varchar(100),
@guid varchar(50),
@domain varchar(50),
@name varchar(100),
@codeBaseURL varchar(256) = null,
@webServiceURL varchar(256)

AS
Declare @processAgentTypeID int 
declare @selfID int

select @selfID=Agent_id from ProcessAgent where EXISTS ( select * from processAgent where agent_id = 1)

if @selfID = 1
  begin
    update ProcessAgent set Agent_GUID=@guid, Agent_Name=@name, Codebase_URL=@codeBaseURL, WebService_URL=@webServiceURL,
      Domain_GUID = @domain
    where agent_id = 1
    select @@rowcount
  end
else
  begin
    select @processAgentTypeID = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@processAgentType )))
    SET IDENTITY_INSERT ProcessAgent ON
    insert into ProcessAgent(agent_id, Agent_GUID, Agent_Name,ProcessAgent_Type_ID, Codebase_URL, WebService_URL, Domain_GUID)
    values (1, @guid, @name, @processAgentTypeID,  @codeBaseURL, @webServiceURL, @domain )
    select 1
    SET IDENTITY_INSERT ProcessAgent OFF
end

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

Create procedure UpdateDomainGuid
@guid varchar(50)

as
declare @found int

select @found = count(agent_ID) from ProcessAgent where agent_id = 1
if @found > 0
  BEGIN
    Update ProcessAgent set Domain_GUID = @guid where agent_id = 1
    return 1
  END
ELSE
  begin
    return 0
  end

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
CREATE PROCEDURE UpdateProcessAgent
@id int,
@guid varchar(50),
@domain varchar (50), 
@name varchar(100),
@type varchar(100),
@codeBaseURL varchar(256) = null,
@webServiceURL varchar(256)



AS
Declare @processAgentTypeID int 

select @processAgentTypeID = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@type )))
update ProcessAgent set Agent_GUID = @guid, ProcessAgent_Type_ID = @processAgentTypeID,
 Agent_name = @name, Codebase_URL = @codeBaseURL, WebService_URL = @webServiceURL, Domain_Guid = @domain
where Agent_ID = @id
select @@rowcount

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



CREATE PROCEDURE GetProcessAgents 
 AS

select Agent_ID, Agent_GUID, Agent_Name, ProcessAgent_Type.Description,
Domain_guid, Codebase_URL, WebService_URL
from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgent 

@agentGUID varchar(50)

 AS

SELECT Agent_ID, Agent_GUID, Agent_Name, ProcessAgent_Type.Description,
 Domain_Guid, Codebase_URL, WebService_URL
FROM ProcessAgent JOIN ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
WHERE Agent_GUID= @agentGUID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentByID 

@ID int

 AS

SELECT Agent_ID, Agent_GUID, Agent_Name, ProcessAgent_Type.Description,
Domain_Guid, Codebase_URL, WebService_URL
FROM ProcessAgent JOIN ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
WHERE Agent_ID= @ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetProcessAgentsByType

@agentType varchar(100)

 AS

DECLARE @agentTypeID int 

select @agentTypeID = (select ProcessAgent_Type_ID from  ProcessAgent_Type 
where ProcessAgent_Type.Description = upper(@agentType))

select Agent_ID, Agent_GUID, Agent_Name, upper(@agentType),
Domain_Guid, Codebase_URL, WebService_URL
from ProcessAgent where ProcessAgent_Type_ID= @agentTypeID


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




-- AgentInfo, a utility structure for processing service requests.
CREATE PROCEDURE GetProcessAgentsInfo 

 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID, Codebase_URL, WebService_URL, Domain_Guid,
issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where ProcessAgent.Issuer_GUID != null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_GUID != null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID)
from ProcessAgent

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetProcessAgentID
@guid varchar (50)
 AS

select Agent_ID from ProcessAgent
where Agent_GUID = @guid


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentIDs
@guid varchar (50)
 AS

select Agent_ID from ProcessAgent
where Agent_GUID = @guid


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE GetProcessAgentIDsByType
@type varchar (100)
 AS
declare @typeID int
select @typeID= processAgent_type_id from ProcessAgent_type where description = @type
select Agent_ID from ProcessAgent
where ProcessAgent_type_id= @typeid


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentIDsByTypeID
@typeid int
 AS

select Agent_ID from ProcessAgent
where ProcessAgent_type_id = @typeid


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetProcessAgentInfo 
@guid varchar (50)
 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID, Codebase_URL, WebService_URL, Domain_Guid,
issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID)
from ProcessAgent
where Agent_GUID = @guid


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentInfoByID 
@id int
 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID,
Codebase_URL, WebService_URL,  Domain_Guid, issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where ProcessAgent.Issuer_Guid != Null AND  coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID)
from ProcessAgent
where Agent_ID = @id


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentInfoByType 
@agentType varchar (100)
 AS
declare @typeID int

select @typeID = ProcessAgent_Type_ID from  ProcessAgent_Type where ProcessAgent_Type.Description = upper(@agentType)
select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID,
Codebase_URL,  WebService_URL,  Domain_Guid, issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID)
from ProcessAgent
where ProcessAgent_Type_ID = @typeID


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentInfoByInCoupon
@couponID bigint, 
@issuer varchar (50)
 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID,
Codebase_URL, WebService_URL,  Domain_Guid, issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID)
from ProcessAgent
where IdentIn_ID = @couponID AND issuer_GUID = @issuer


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetProcessAgentInfoByOutCoupon
@couponID bigint, 
@issuer varchar (50)
 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID,
Codebase_URL, WebService_URL,  Domain_Guid, issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID)
from ProcessAgent
where IdentOut_ID = @couponID AND issuer_GUID = @issuer


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetProcessAgentURL 

@guid varchar(50)

 AS

select WebService_URL
 from ProcessAgent
 where  Agent_GUID = @guid

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetProcessAgentURLbyID 

@agent_id int

 AS

select WebService_URL
 from ProcessAgent
 where  Agent_ID = @agent_id

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

--
-- Note: Ticket use in the ProcessAgent may be optional, 
-- Tickets are created by an external ServiceBroker.
--

CREATE PROCEDURE CancelTicket

@ticketType  varchar(100),
@redeemerGuid  varchar(50),
@couponID  bigint,
@issuerGuid  varchar(50)

AS

DECLARE

@ticketTypeID int

select @ticketTypeID= (select Ticket_Type_ID from Ticket_Type where  upper(Name) = upper(@ticketType))

update Ticket set Cancelled = 1 
where Coupon_ID = @couponID and Ticket_Type_ID = @ticketTypeID 
and Issuer_GUID=@issuerGuid and redeemer_GUID = @redeemerGuid

select @@rowcount
return


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE DeleteTicket

@ticketType  varchar(100),
@redeemerGuid  varchar(50),
@couponID  bigint,
@issuerGuid  varchar(50)

AS

DECLARE

@ticketTypeID int

select @ticketTypeID= (select Ticket_Type_ID from Ticket_Type where  upper(Name) = upper(@ticketType))

DELETE from Ticket
where Coupon_ID = @couponID and Ticket_Type_ID = @ticketTypeID 
and Issuer_GUID=@issuerGuid and redeemer_GUID = @redeemerGuid

select @@rowcount
return


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE CancelTicketByID
@ticketID  bigint,
@issuer varchar (50)

AS

update Ticket set Cancelled = 1 
where ticket_ID = @ticketID AND Issuer_Guid = @issuer

select @@rowcount
return


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO
CREATE PROCEDURE DeleteTicketByID
@ticketID  bigint,
@issuer varchar (50)
AS

delete from ticket
where ticket_ID = @ticketID  AND Issuer_Guid = @issuer

select @@rowcount
return


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO
-- Returns the TicketID and coupon for an expired ticket
CREATE PROCEDURE GetExpiredTickets
AS
select t.ticket_id, t.coupon_id, t.Issuer_guid, c.passkey
from ticket t, coupon c
where t.cancelled = 1 OR
(t.cancelled = 0 and duration != -1 and DATEDIFF(second,creation_Time,GETUTCDATE()) > duration
and t.coupon_id = c.Coupon_id and t.issuer_guid = c.issuer_guid)

go
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

-- Inserting a ticket in the general ticketing tables does not generate the ticket_id,
-- If the ServiceBroker also needs to deal with tickets it has not issued
-- it will have to maintain the full structure.
CREATE PROCEDURE InsertTicket
	@ticketID bigint,
	@ticketType varchar(100),
	@couponID bigint,
	@issuerGUID varchar(50),
	@redeemerGUID varchar(50),
  	@sponsorGUID varchar(50),
	@creationTime DateTime,
  	@duration bigint,
  	@payload text,
  	@cancelled bit=0
   AS
	DECLARE @ticketTypeID int
	select @ticketTypeID = (select Ticket_Type_ID  from Ticket_Type where upper( Name ) =upper(@ticketType) )
	insert into Ticket (Ticket_ID, Ticket_Type_ID, creation_Time, duration, 	payload,cancelled,Coupon_ID,Issuer_GUID, sponsor_GUID, Redeemer_GUID) 
	values (@ticketID, @ticketTypeID,@creationTime,@duration,@payload,@cancelled, @couponID, 	@issuerGUID, @sponsorGUID, @redeemerGUID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetTicketID 
@couponID bigint,
@issuerGuid varchar(50),
@redeemerID varchar(50),
@ticketType varchar(100)

AS
DECLARE @ticketTypeID int

select @ticketTypeID=(select Ticket_Type_ID from Ticket_Type where (upper(Name) =upper( @ticketType) )) 
select Ticket_ID from Ticket 
where Coupon_ID = @couponID  AND Ticket_Type_ID = @ticketTypeID 
AND Issuer_GUID = @issuerGuid AND Redeemer_GUID = @redeemerID 

return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetTicketIDs 
@couponID  bigint,
@issuerGuid varchar(50)
 AS

select Ticket_ID from Ticket 
where Coupon_ID= @couponID AND Issuer_GUID = @issuerGuid
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetTicket
	@couponID bigint,
	@issuer varchar(50),
	@redeemer varchar(50),
        @ticketType varchar(100)
  AS
        DECLARE @ticketTypeID int

        select @ticketTypeID = (select Ticket_Type_ID  from Ticket_Type where upper( Name ) = upper(@ticketType) )
        select  Ticket_ID, upper(@ticketType), Coupon_ID, Issuer_GUID, Redeemer_GUID, Sponsor_GUID, 
	Creation_Time, duration, Payload, Cancelled
        from Ticket  
	where coupon_ID = @couponID and Ticket_Type_ID = @ticketTypeID and Redeemer_GUID = @redeemer and Issuer_GUID = @issuer

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetTicketsByCoupon
	@couponID bigint,
	@issuer varchar(50)

  AS
        DECLARE @ticketTypeID int

        select  Ticket_ID, Ticket_Type.Name, Coupon_ID, Issuer_GUID, Redeemer_GUID, Sponsor_GUID, 
	Creation_Time, duration, Payload, Cancelled
        from Ticket join Ticket_Type on (Ticket.Ticket_Type_ID= Ticket_Type.Ticket_Type_ID)
	where coupon_ID = @couponID and Issuer_GUID = @issuer

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE GetTicketsByType
	@ticketType varchar(100)
  AS
        DECLARE @ticketTypeID int
        select @ticketTypeID = (select Ticket_Type_ID  from Ticket_Type where upper( Name ) = upper(@ticketType) )
        select  Ticket_ID, upper(@ticketType), Coupon_ID, Issuer_GUID, Redeemer_GUID, Sponsor_GUID, 
	Creation_Time, duration, Payload, Cancelled
        from Ticket  where Ticket_Type_ID = @ticketTypeID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE GetTicketByID
    @ticketID bigint,
    @issuer varchar (50)
 AS
 	
	select Ticket_ID, Ticket_Type.Name, Coupon_ID, Issuer_GUID, Redeemer_GUID, Sponsor_GUID, 
	Creation_Time, duration, Payload, Cancelled
        from Ticket join Ticket_Type on (Ticket.Ticket_Type_ID= Ticket_Type.Ticket_Type_ID)
        where Ticket_ID = @ticketID  AND Issuer_Guid = @issuer

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE RetrieveTicketTypes
  AS
        select Ticket_Type_ID, Name, Short_Description, Abstract  from Ticket_Type
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

