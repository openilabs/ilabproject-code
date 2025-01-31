-- ESS_Procedures.sql
--
-- Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
-- $Id:$

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddBlobAccess]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddBlobAccess]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddBlobData]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddBlobData]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddBlobToExperimentRecord]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddBlobToExperimentRecord]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddExperimentRecord]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddExperimentRecord]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddRecordAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddRecordAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CloseExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[CloseExperiment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CreateBlob]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[CreateBlob]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CreateExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[CreateExperiment]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[OpenExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[OpenExperiment]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteExperiment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteRecordAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteRecordAttribute]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetMimeTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetMimeTypeID]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveBlob]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveBlob]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveBlobAccess]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveBlobAccess]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveBlobAssociation]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveBlobAssociation]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveBlobData]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveBlobData]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveBlobDataWithMime]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveBlobDataWithMime]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveBlobExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveBlobExperiment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveBlobsForExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveBlobsForExperiment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveBlobsForExperimentRecord]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveBlobsForExperimentRecord]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperiment]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentIDsCriteria]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentIDsCriteria]
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveEssExperimentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveEssExperimentID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentIdleTime]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentIdleTime]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentRecord]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentRecord]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveRecordAttributeByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveRecordAttributeByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveRecordAttributeByName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveRecordAttributeByName]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveRecordsForExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveRecordsForExperiment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentRecords]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentRecords]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentRecordNumbers]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentRecordNumbers]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentRecordIdsCriteria]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentRecordIdsCriteria]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentRecordNumbersCriteria]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentRecordNumbersCriteria]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentRecordsCriteria]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentRecordsCriteria]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveSBExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveSBExperiment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetExperimentStatus]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetExperimentStatus]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SetExperimentStatus]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SetExperimentStatus]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UpdateExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UpdateExperiment]
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE AddBlobAccess
	@blobId bigint,
	@blobUrl nvarchar(1024)
	-- should add expiration time
AS
	insert into Blobs_Access (blob_id, blob_url)
	values (@blobId, @blobUrl)

	--select ident_current('blobs_access')
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE GetMimeTypeID
	@mimeType varchar (1024),
	@mimeID int OUTPUT
AS

if (select count(*) from mime_type where type = @mimeType) > 0
BEGIN
select @mimeID = mime_ID from mime_type where type = @mimeType
END
ELSE
BEGIN
insert into mime_type (type) values (@mimeType)
select @mimeID = ident_current('mime_type');
END
RETURN 0
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE AddBlobData
	@blobID bigint,
	@blobStatus int,
	@mimeType varchar( 1024),
	@byteCount int,
	@blobData image

 AS
	declare @mimeTypeId int

	exec GetMimeTypeID @mimeType, @mimeTypeId OUTPUT

	-- there should be a "last_modified" field

	update Experiment_Blobs set The_Blob = @blobData, Blob_Status = @blobStatus, mime_ID= @mimeTypeID, Byte_Count = @byteCount
	where Blob_ID = @blobID and the_blob is null
	return @@rowcount
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE AddBlobToExperimentRecord
	@blobID bigint,
	@experimentID bigint,
	@issuerGuid varchar (50),
	@sequenceNo int
AS
	declare @recordID bigint
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and issuer_Guid = @issuerGuid )
	select @recordID = (select record_id from experiment_records where EssExp_ID = @essExpID
					and sequence_no = @sequenceNo)
	
	if (@recordID>0)
		update Experiment_Blobs set record_id = @recordID
		where blob_id = @blobID and record_id is null
	
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


CREATE PROCEDURE AddExperimentRecord
	@experimentID bigint,
	@issuerGuid varchar (50),
	@type nvarchar(50),
	@xmlSearchable bit,
	@contents text,
	@submitter nvarchar(256)
AS
BEGIN TRANSACTION

	declare @essExpID bigint
	declare @closeTime datetime
	declare @seqNo int
	select @essExpID =  EssExp_id, @closeTime = close_Time,  @seqNo = Current_Sequence_No from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid


	--declare @essExpID bigint
	--select @essExpID = ( select EssExp_id from Experiments 
	--	where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
	--declare @closeTime datetime
	--select @closeTime = (select close_time from experiments where EssExp_ID = @essExpID);

	IF (@closeTime <> null)
	goto on_closed_experiment

	--declare @seqNo int
	--select @seqNo = (select  Current_Sequence_No from experiments where EssExp_ID=@essExpID);	

	update experiments set current_sequence_no = @seqNo+1, last_modified=getUTCDate() where EssExp_ID=@essExpID;
	IF (@@ERROR <> 0) goto on_error

	--insert into Experiment_Records (EssExp_ID, record_type,  is_xml_searchable, contents, submitter_Name, sequence_no, sponsor_GUID)
		--values (@essExpID, @type, @xmlSearchable, @contents, @submitterName,@seqNo+1,@sponsorGuid);

	insert into Experiment_Records (EssExp_ID, sequence_no,submitter_Name, record_type,  is_xml_searchable, contents)
		values (@essExpID,@seqNo+1,@submitter, @type, @xmlSearchable, @contents);	
	IF (@@ERROR <> 0) goto on_error

	select @seqNo+1;

COMMIT TRANSACTION	
return
	on_error: 
	ROLLBACK TRANSACTION

	on_closed_experiment:
	ROLLBACK TRANSACTION
	select -1;
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

CREATE PROCEDURE AddRecordAttribute
	@experimentID bigint,
	@issuerGuid varchar (50),
	@attributeName nvarchar(256),
	@attributeValue nvarchar(256),	
	@sequenceNo int
	
AS
	declare @recordID bigint
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
	select @recordID = (select record_id from experiment_records where EssExp_ID =@essExpID
				and sequence_no = @sequenceNo)

	insert into Record_Attributes ( EssExp_ID,record_id, attribute_name, attribute_value)
		values (@essExpID,@recordID, @attributeName, @attributeValue);	

	select ident_current('Record_Attributes');
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

CREATE PROCEDURE CloseExperiment 
	@experimentID bigint,
	@issuerGuid varchar (50),
	@statusCode int
AS
BEGIN TRANSACTION
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and issuer_Guid = @issuerGuid )
	update experiments set close_time = getUTCdate(),status_code = @statusCode where EssExp_ID = @essExpID and close_time is null
	IF (@@ERROR <> 0) goto on_error

	delete from experiment_blobs 
	where EssExp_ID = @essExpID and record_id is null
	IF (@@ERROR <> 0) goto on_error

COMMIT TRANSACTION	
return
	on_error: 
	ROLLBACK TRANSACTION
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


CREATE PROCEDURE GetExperimentStatus 
	@experimentID bigint,
	@issuerGuid varchar (50)
	
AS
	select experiment_ID,status_code,Current_Sequence_No,Date_Created,Close_Time,Last_Modified,Issuer_Guid
	from experiments where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid 

	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE SetExperimentStatus 
	@experimentID bigint,
	@issuerGuid varchar (50),
	@statusCode int
AS

	update experiments set status_code = @statusCode 
	where Experiment_ID = @experimentID and issuer_GUID = @issuerGuid
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE CreateBLOB
	@experimentID bigint,
	@issuerGuid varchar (50),
	@description nvarchar(2048) ,
	@byteCount int,
	@checksum varchar(256),
	@checksumAlgorithm varchar(256)
AS
declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and issuer_Guid = @issuerGuid )
	insert into Experiment_Blobs (EssExp_ID, description, byte_count, check_sum, check_sum_algorithm)
	values (@essExpID, @description, @byteCount,@checksum, @checksumAlgorithm)

	select ident_current('experiment_blobs')
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE CreateExperiment
	
	@experimentID bigint,
	@issuerGuid varchar(50),
	@scheduledClose datetime,
	@status int,
	@userID int,
	@groupID int,
	@clientID int

	
AS
	insert into Experiments (Experiment_ID, Issuer_GUID, status_code, User_ID, Group_ID, Client_ID, last_modified, when_close, current_sequence_no)
	values (@experimentID,@issuerGuid,@status, @userID, @groupID, @clientID, getUtcdate(),@scheduledClose, 0)

	select ident_current('experiments')
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE DeleteExperiment
	@experimentID bigint,
	@issuerGuid varchar(50)
AS
	delete from Experiments
	where Experiment_ID = @experimentID and Issuer_GUID = @issuerGuid;

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

CREATE PROCEDURE OpenExperiment
	
	@experimentID bigint,
	@issuerGuid varchar(50),
	@scheduledClose datetime

	
AS
	select experiment_Id from experiments where experiment_ID = @experimentID and Issuer_Guid= @issuerGuid
	if @@rowcount > 0
	update Experiments set status_code = 6, when_close=@scheduledClose, Close_Time = NULL
	where Experiment_ID=@experimentID and Issuer_Guid= @issuerGuid
	else
	insert into Experiments (Experiment_ID, Issuer_Guid, status_code,last_modified, when_close, current_sequence_no)
	values (@experimentID,@issuerGuid, 4, getUtcdate(),@scheduledClose, 0)

select experiment_ID,status_code,Current_Sequence_No,Date_Created,Close_Time,Last_Modified,Issuer_Guid
	from experiments where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE UpdateExperiment
	
	@experimentID bigint,
	@issuerGuid varchar(50),
	@status int,
	@scheduledClose datetime,
	@closeTime datetime

	
AS
	update Experiments set status_code = @status, when_close=@scheduledClose, Close_Time =@closeTime
	where Experiment_ID=@experimentID and Issuer_Guid= @issuerGuid

	select @status
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE DeleteRecordAttribute
	@attributeID int,
	@experimentID bigint,
	@issuerGuid varchar (50),
	@sequenceNo int
	
AS
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
	declare @recordID bigint
	select @recordID = (select record_id from experiment_records where EssExp_ID = @essExpID
				and sequence_no = @sequenceNo)

	select attribute_name, attribute_value from Record_Attributes 
		where attribute_id = @attributeID and record_id = @recordID

	delete from Record_Attributes 
		where attribute_id = @attributeID and record_id = @recordID

return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE RetrieveBlob
	@blobID bigint

AS
	
select  (select experiment_ID from experiments e where e.EssExp_ID = eb.EssExp_ID) experiment_ID,
	 description, date_created, byte_count, check_sum, check_sum_algorithm,
	(select sequence_no from experiment_records er  where er.record_id = eb.record_id) seq_num,
	(select type from Mime_Type mt where  mt.Mime_ID = eb.Mime_ID) mime_type
	from Experiment_Blobs eb where blob_id = @blobID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveBlobAccess
	@blobID bigint,
	@duration int
AS


	update Blobs_Access set expiration_time = dateadd(minute, @duration,getdate())
	where blob_id=@blobID
	
	select blob_url from blobs_access where blob_id=@blobID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveBLOBAssociation
	@blobID bigint
AS
	
	select sequence_no from Experiment_Records er, Experiment_Blobs eb
	where (eb.blob_id = @blobID) and (eb.record_ID = er.record_ID)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveBlobData
	@blobID bigint
AS
	
	select Byte_Count, the_Blob
	from Experiment_Blobs
	where blob_id = @blobID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveBlobDataWithMime
	@blobID bigint
AS
	
	select mt.type, eb.Byte_count, eb.the_Blob
	from Experiment_Blobs eb, MIME_TYPE mt
	where (eb.blob_id = @blobID) and (eb.MIME_ID = mt.MIME_ID)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveBlobExperiment
	@blobID bigint
AS
	declare @essExpID bigint
	select @essExpID = ( select  EssExp_ID
	from Experiment_Blobs where blob_id = @blobID )
	select experiment_ID,Issuer_Guid from experiments where EssExp_ID = @essExpID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveBlobsForExperiment
	@experimentID bigint,
	@issuerGuid varchar (50)
AS
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )

	if (@essExpID > 0)
		select  blob_id, description, date_created, byte_count, check_sum, check_sum_algorithm,
		(select sequence_no from experiment_records er  where er.record_id = eb.record_id) seq_num,
		(select type from Mime_Type mt where  mt.Mime_ID = eb.Mime_ID) mime_type
		from Experiment_Blobs eb where record_id in (select record_ID from experiment_records
		       	      where EssExp_ID = @essExpID)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveBlobsForExperimentRecord
	@experimentID bigint,
	@issuerGuid varchar (50),
	@sequenceNo int
AS
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
	declare @recordID bigint
	select @recordID = (select record_id
			      from experiment_records
		       	      where EssExp_ID = @essExpID and sequence_no = @sequenceNo);

	if (@recordID > 0)
	   select  blob_id, description, date_created, byte_count, check_sum, check_sum_algorithm,
		(select sequence_no from experiment_records er  where er.record_id = eb.record_id) seq_num,
		(select type from Mime_Type mt where  mt.Mime_ID = eb.Mime_ID) mime_type
		from Experiment_Blobs eb where record_id = @recordID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveExperiment
	@experimentID bigint,
	@issuerGuid varchar (50)
AS

	select EssExp_ID, current_sequence_no, Experiment_ID, Issuer_Guid  from experiments 
	where Experiment_ID = @experimentID and Issuer_Guid = @issuerGuid
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveSBExperiment
	@sbExpID bigint,
	@issuerGuid varchar (50)
AS

	select EssExp_ID,current_sequence_no  from experiments 
	where Experiment_ID = @sbExpID and Issuer_Guid = @issuerGuid
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveExperimentIdleTime
	@experimentID bigint,
	@issuerGuid varchar (50)
AS
	
	select last_modified from experiments
	where Experiment_ID = @experimentID and Issuer_Guid = @issuerGuid
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveExperimentRecord
	@experimentID bigint,
	@issuerGuid varchar (50),
	@sequenceNo	int
AS
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
	select  record_type, submitter_Name, contents, time_stamp, is_xml_searchable, sequence_no
	from experiment_records
	where EssExp_ID = @essExpID and sequence_no = @sequenceNo
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE PROCEDURE RetrieveExperimentRecords
	@experimentID bigint,
	@issuerGuid varchar (50)
AS
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
	select  sequence_no, record_type, submitter_Name, contents, time_stamp, is_xml_searchable
	from experiment_records
	where EssExp_ID = @essExpID Order by sequence_no
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

Create PROCEDURE RetrieveExperimentRecordNumbers
@experimentID bigint,
@issuerGuid varchar (50)
AS
declare @essExpID bigint

select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )

select DISTINCT sequence_no from Experiment_Records 
where essExp_id = @essExpId
order by sequence_no


RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

Create PROCEDURE RetrieveExperimentIDsCriteria
@expIDs varchar(7000),
@issuerGuid varchar (50),
@fieldQuery nvarchar(3000) = NULL,
@attributeQuery nvarchar(3000) = NULL

AS
BEGIN
CREATE TABLE #essExps ( ids bigint )
Create Table #expids  ( ids bigint )

--exec @exps =dbo.GetLongList @expIDs
 INSERT into #eesExps SELECT EssExp_id from Experiments 
		where experiment_ID in (dbo.toLongArray( @expIDs )) and Issuer_Guid = @issuerGuid 
--print @fieldQuery
--print @attributeQuery

if @fieldQuery IS NOT NULL
insert #expids Exec('select distinct essExp_id from Experiment_records where essExp_ID IN SELECT ids from #essExps AND '
	+ @fieldQuery)

if @attributeQuery IS NOT NULL
insert #expids Exec('select distinct essExp_id from record_Attributes where essExp_ID IN SELECT ids from #essExps AND '
	+ @attributeQuery)

select DISTINCT experiment_ID from experiments where essExp_ID IN (select ids from #expids)
drop table #expids
drop table #essExps
END
RETURN
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

Create PROCEDURE RetrieveExperimentRecordIDsCriteria
@experimentID bigint,
@issuerGuid varchar (50),
@fieldQuery nvarchar(3000),
@attributeQuery nvarchar(3000)

AS
BEGIN
declare @essExpID bigint
Create Table #recids  (recId bigint)

select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
--print @fieldQuery
--print @attributeQuery

if @fieldQuery IS NOT NULL
insert #recids Exec('select distinct record_id from Experiment_records where essExp_ID = '
	+ @essExpID + ' AND ' +@fieldQuery)

if @attributeQuery IS NOT NULL
insert #recids Exec('select distinct record_id from Experiment_records where essExp_ID = '
	+ @essExpID + ' AND '+ @attributeQuery)

select DISTINCT recId from #recids
drop table #recids
END
RETURN
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

Create PROCEDURE RetrieveExperimentRecordNumbersCriteria
@experimentID bigint,
@issuerGuid varchar (50),
@fieldQuery nvarchar(3000),
@attributeQuery nvarchar(3000)
AS

BEGIN
declare @essExpID bigint
Create Table #recids  (recId bigint)

select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
--print @fieldQuery
--print @attributeQuery

if @fieldQuery IS NOT NULL
insert #recids Exec('select distinct record_id from Experiment_records where essExp_ID = '
	+ @essExpID + ' AND ' +@fieldQuery)

if @attributeQuery IS NOT NULL
insert #recids Exec('select distinct record_id from Experiment_records where essExp_ID = '
	+ @essExpID + ' AND '+ @attributeQuery)

select sequence_no from experiment_records 
where record_id in (select DISTINCT recId from #recids)
order by sequence_no
drop table #recids
END
RETURN
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

Create PROCEDURE RetrieveExperimentRecordsCriteria
@experimentID bigint,
@issuerGuid varchar (50),
@fieldQuery nvarchar(3000),
@attributeQuery nvarchar(3000)
AS

BEGIN
declare @essExpID bigint
Create Table #recids  (recId bigint)

select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )

if @fieldQuery IS NOT NULL
insert #recids Exec('select distinct record_id from Experiment_records where essExp_ID = '
	+ @essExpID + ' AND ' +@fieldQuery)

if @attributeQuery IS NOT NULL
insert #recids Exec('select distinct record_id from Experiment_records where essExp_ID = '
	+ @essExpID + ' AND '+ @attributeQuery)

select  sequence_no, record_type, submitter_Name, contents, time_stamp, is_xml_searchable
	from experiment_records 
	where record_id in (select DISTINCT recId from #recids)
	order by sequence_no
drop table #recids
END
RETURN
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrieveRecordAttributeByID
	@attributeID int,
	@experimentID bigint,
	@issuerGuid varchar (50),
	@sequenceNo int
	
AS
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
	declare @recordID bigint
	select @recordID = (select record_id from experiment_records where EssExp_ID =@essExpID
				and sequence_no = @sequenceNo)

	select  attribute_name, attribute_value from Record_Attributes 
		where attribute_id = @attributeID and record_id = @recordID

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

CREATE PROCEDURE RetrieveRecordAttributeByName
	@attributeName nvarchar(256),
	@experimentID bigint,
	@issuerGuid varchar (50),
	@sequenceNo int
	
AS
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
	declare @recordID bigint
	select @recordID = (select record_id from experiment_records where EssExp_ID =@essExpID
				and sequence_no = @sequenceNo)

	select attribute_value from Record_Attributes 
		where attribute_name = @attributeName and record_id = @recordID

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

Create PROCEDURE RetrieveRecordsForExperiment
	@experimentID bigint,
	@issuerGuid varchar (50)
AS
	declare @essExpID bigint
	select @essExpID = ( select EssExp_id from Experiments 
		where experiment_ID = @experimentID and Issuer_Guid = @issuerGuid )
	select  record_type, submitter_name, contents, time_stamp, is_xml_searchable, sequence_no
	from experiment_records 
	where EssExp_ID = @essExpID
	order by sequence_no
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE RetrieveEssExperimentID
	@sbExpID bigint,
	@issuerGuid varchar (50)
AS
	select EssExp_ID from experiments
	where Experiment_ID = @sbExpID AND Issuer_Guid = @issuerGuid
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

