-- Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
-- $Id:$

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Blobs_Access_Experiment_Blobs]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Blobs_Access] DROP CONSTRAINT FK_Blobs_Access_Experiment_Blobs
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Experiment_Blobs_Experiment_Records]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Experiment_Blobs] DROP CONSTRAINT FK_Experiment_Blobs_Experiment_Records
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Record_Attributes_Experiment_Records]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Record_Attributes] DROP CONSTRAINT FK_Record_Attributes_Experiment_Records
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Record_Attributes_Experiment_ID]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Record_Attributes] DROP CONSTRAINT FK_Record_Attributes_Experiment_ID
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Experiment_Blobs_Experiments]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Experiment_Blobs] DROP CONSTRAINT FK_Experiment_Blobs_Experiments
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Experiment_Blobs_Mime_Type]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Experiment_Blobs] DROP CONSTRAINT FK_Experiment_Blobs_Mime_Type
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Experiment_Records_Experiments]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Experiment_Records] DROP CONSTRAINT FK_Experiment_Records_Experiments
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Blobs_Access]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Blobs_Access]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_Blobs]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Experiment_Blobs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_Records]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Experiment_Records]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiments]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Experiments]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Mime_type]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Mime_Type]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Record_Attributes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Record_Attributes]
GO

CREATE TABLE [dbo].[Blobs_Access] (
	[Blob_Access_ID] [bigint] IDENTITY (1, 1) NOT NULL ,
	[Blob_ID] [bigint] NOT NULL ,
	[Blob_URL] [nvarchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Expiration_Time] [datetime] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Experiment_Blobs] (
	[Blob_ID] [bigint] IDENTITY (1, 1) NOT NULL ,
	[The_Blob] [image] NULL ,
	[EssExp_ID] [bigint] NOT NULL ,
	[Record_ID] [bigint] NULL ,
	[Blob_Status] [int] DEFAULT 0 NOT NULL,
	[Mime_ID] [int] DEFAULT 0 NOT NULL,
	[Description] [nvarchar] (2048) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Byte_Count] [int] NOT NULL ,
	[Check_Sum] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Check_Sum_Algorithm] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Date_Created] [datetime] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Experiment_Records] (
	[Record_ID] [bigint] IDENTITY (1, 1) NOT NULL ,
	[EssExp_ID] [bigint] NOT NULL ,
	[Sequence_No] [int] NULL ,
	[Record_Type] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Submitter_Name] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Contents] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Time_Stamp] [datetime] NOT NULL ,
	[Is_XML_Searchable] [bit] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE UNIQUE INDEX [Sequence_IDX] ON [dbo].[Experiment_Records] ([ESSExp_ID], [Sequence_No])
GO

CREATE TABLE [dbo].[Experiments] (
	[EssExp_ID] [bigint] IDENTITY (1, 1) NOT NULL ,
	[Experiment_ID] [bigint] NOT NULL ,
	[Issuer_GUID] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Last_Modified] [datetime] NOT NULL ,
	[Date_Created] [datetime] NOT NULL ,
	[Close_Time] [datetime] NULL ,
	[When_Close] [datetime] NULL ,
	[Current_Sequence_No] [int] NOT NULL ,
	[status_code] [int] NOT NULL,
	[User_ID] [int] NOT NULL DEFAULT 0,
	[Group_ID] [int] NOT NULL DEFAULT 0,
	[Client_ID] [int] NOT NULL DEFAULT 0
) ON [PRIMARY]
GO

CREATE 
  INDEX [SB_Experiment_IDX] ON [dbo].[Experiments] ([Experiment_ID], [Issuer_GUID])
GO

CREATE TABLE [DBO].[Mime_Type](
	[Mime_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Type] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Extension] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO

SET IDENTITY_INSERT Mime_Type ON
	INSERT INTO Mime_Type(Mime_ID, Type) VALUES (0,'unknown');
SET IDENTITY_INSERT Mime_Type OFF
GO

CREATE TABLE [dbo].[Record_Attributes] (
	[Attribute_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EssExp_ID] [bigint] NOT NULL ,
	[Record_ID] [bigint] NOT NULL ,
	[Attribute_Name] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Attribute_Value] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Blobs_Access] WITH NOCHECK ADD 
	CONSTRAINT [PK_Blobs_Access] PRIMARY KEY  CLUSTERED 
	(
		[Blob_Access_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Experiment_Blobs] WITH NOCHECK ADD 
	CONSTRAINT [PK_Experiment_Blobs] PRIMARY KEY  CLUSTERED 
	(
		[Blob_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Experiment_Records] WITH NOCHECK ADD 
	CONSTRAINT [PK_Experiment_Records] PRIMARY KEY  CLUSTERED 
	(
		[Record_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Experiments] WITH NOCHECK ADD 
	CONSTRAINT [PK_Experiments] PRIMARY KEY  CLUSTERED 
	(
		[EssExp_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Mime_Type] WITH NOCHECK ADD 
	CONSTRAINT [PK_Mime_Type] PRIMARY KEY  CLUSTERED 
	(
		[Mime_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Record_Attributes] WITH NOCHECK ADD 
	CONSTRAINT [PK_Record_Attributes] PRIMARY KEY  CLUSTERED 
	(
		[Attribute_ID]
	)  ON [PRIMARY] 
GO


ALTER TABLE [dbo].[Experiment_Blobs] ADD 
	CONSTRAINT [DF_Experiment_Blobs_Date_Created] DEFAULT (getUTCdate()) FOR [Date_Created]
GO

ALTER TABLE [dbo].[Experiment_Records] ADD 
	CONSTRAINT [DF_Experiment_Records_Time_Stamp] DEFAULT (getUTCdate()) FOR [Time_Stamp]
GO

ALTER TABLE [dbo].[Experiments] ADD 
	CONSTRAINT [DF_Experiments_Date_Created] DEFAULT (getUTCdate()) FOR [Date_Created],
	CONSTRAINT [DF_Experiments_status_code] DEFAULT (1) FOR [status_code]
GO

ALTER TABLE [dbo].[Blobs_Access] ADD 
	CONSTRAINT [FK_Blobs_Access_Experiment_Blobs] FOREIGN KEY 
	(
		[Blob_ID]
	) REFERENCES [dbo].[Experiment_Blobs] (
		[Blob_ID]
	) ON DELETE CASCADE 
GO

ALTER TABLE [dbo].[Experiment_Blobs] ADD 
	CONSTRAINT [FK_Experiment_Blobs_Experiment_Records] FOREIGN KEY 
	(
		[Record_ID]
	) REFERENCES [dbo].[Experiment_Records] (
		[Record_ID]
	),
	CONSTRAINT [FK_Experiment_Blobs_Experiments] FOREIGN KEY 
	(
		[EssExp_ID]
	) REFERENCES [dbo].[Experiments] (
		[EssExp_ID]
	) ON DELETE CASCADE,
	CONSTRAINT [FK_Experiment_Blobs_Mime_Type] FOREIGN KEY 
	(
		[Mime_ID]
	) REFERENCES [dbo].[Mime_Type] (
		[Mime_ID]
	) ON DELETE CASCADE  
GO

ALTER TABLE [dbo].[Experiment_Records] ADD 
	CONSTRAINT [FK_Experiment_Records_Experiments] FOREIGN KEY 
	(
		[EssExp_ID]
	) REFERENCES [dbo].[Experiments] (
		[EssExp_ID]
	) ON DELETE CASCADE 
GO

ALTER TABLE [dbo].[Record_Attributes] ADD 
CONSTRAINT [FK_Record_Attributes_Experiment_Records] FOREIGN KEY 
	(
		[Record_ID]
	) REFERENCES [dbo].[Experiment_Records] (
		[Record_ID]
	) ON DELETE CASCADE
	
GO

