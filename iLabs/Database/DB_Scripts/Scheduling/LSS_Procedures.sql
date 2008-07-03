/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: LSSstoreprocedure.sql,v 1.12 2007/06/28 14:08:05 pbailey Exp $
 */

/****** Object:  Stored Procedure dbo.AddCredentialSet    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddCredentialSet]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddCredentialSet]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetCredentialSetID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetCredentialSetID]
GO
/****** Object:  Stored Procedure dbo.RemoveCredentialSet    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RemoveCredentialSet]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RemoveCredentialSet]
GO

/****** Object:  Stored Procedure dbo.AddExperimentInfo    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddExperimentInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddExperimentInfo]
GO

/****** Object:  Stored Procedure dbo.AddLSSPolicy    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddLSSPolicy]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddLSSPolicy]
GO

/****** Object:  Stored Procedure dbo.AddPermittedExperiment    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddPermittedExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddPermittedExperiment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[IsPermittedExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[IsPermittedExperiment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedExperimentIDsForExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedExperimentIDsForExperiment]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedExperimentIDsForRecurrence]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedExperimentIDsForRecurrence]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedExperimentIDsForTimeBLock]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedExperimentIDsForTimeBLock]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[IsPermittedGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[IsPermittedGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedGroupIDsForRecurrence]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedGroupIDsForRecurrence]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedGroupIDsForTimeBlock]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedGroupIDsForTimeBlock]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetRecurenceIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetRecurenceIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Resource_AddGetID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Resource_AddGetID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Resource_Get]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Resource_Get]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Resource_GetByGuid]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Resource_GetByGuid]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Resource_GetTags]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Resource_GetTags]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Resource_GetTagsByGuid]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Resource_GetTagsByGuid]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Resource_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Resource_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Resource_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Resource_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Resource_SetDescription]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Resource_SetDescription]
GO



/****** Object:  Stored Procedure dbo.AddPermittedExperiment    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddPermittedGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddPermittedGroup]
GO
/****** Object:  Stored Procedure dbo.AddPermittedExperiment    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeletePermittedGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeletePermittedGroup]
GO
/****** Object:  Stored Procedure dbo.AddPermittedExperiment    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedGroupIDByRecur]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedGroupIDByRecur]
GO
/****** Object:  Stored Procedure dbo.AddRecurrence    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddRecurrence]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddRecurrence]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddReservation]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddReservation]
GO

/****** Object:  Stored Procedure dbo.AddReservationInfo    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddReservationInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddReservationInfo]
GO

/****** Object:  Stored Procedure dbo.AddTimeBlock    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddTimeBlock]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddTimeBlock]
GO

/****** Object:  Stored Procedure dbo.AddUSSInfo    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AddUSSInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AddUSSInfo]
GO

/****** Object:  Stored Procedure dbo.DeleteCredentialSet    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteCredentialSet]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteCredentialSet]
GO

/****** Object:  Stored Procedure dbo.DeleteExperimentInfo    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteExperimentInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteExperimentInfo]
GO

/****** Object:  Stored Procedure dbo.DeleteLSSPolicy    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteLSSPolicy]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteLSSPolicy]
GO

/****** Object:  Stored Procedure dbo.DeletePermittedExperiment    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeletePermittedExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeletePermittedExperiment]
GO


/****** Object:  Stored Procedure dbo.DeleteRecurrence    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteRecurrence]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteRecurrence]
GO

/****** Object:  Stored Procedure dbo.DeleteReservationInfo    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteReservationInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteReservationInfo]
GO

/****** Object:  Stored Procedure dbo.DeleteReservationInfoByID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteReservationInfoByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteReservationInfoByID]
GO

/****** Object:  Stored Procedure dbo.DeleteTimeBlock    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteTimeBlock]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteTimeBlock]
GO

/****** Object:  Stored Procedure dbo.DeleteUSSInfo    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteUSSInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteUSSInfo]
GO

/****** Object:  Stored Procedure dbo.ModifyCredentialSet    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ModifyCredentialSet]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ModifyCredentialSet]
GO

/****** Object:  Stored Procedure dbo.ModifyExperimentInfo    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ModifyExperimentInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ModifyExperimentInfo]
GO

/****** Object:  Stored Procedure dbo.ModifyLSSPolicy    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ModifyLSSPolicy]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ModifyLSSPolicy]
GO

/****** Object:  Stored Procedure dbo.ModifyTimeBlock    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ModifyTimeBlock]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ModifyTimeBlock]
GO

/****** Object:  Stored Procedure dbo.ModifyUSSInfo    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ModifyUSSInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ModifyUSSInfo]
GO

/****** Object:  Stored Procedure dbo.RetrieveCredentialSetByID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveCredentialSetByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveCredentialSetByID]
GO

/****** Object:  Stored Procedure dbo.RetrieveCredentialSetIDs    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveCredentialSetIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveCredentialSetIDs]
GO

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoByID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentInfoByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentInfoByID]
GO

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoIDByExperiment    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentInfoIDByExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentInfoIDByExperiment]
GO

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoIDs    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentInfoIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentInfoIDs]
GO

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoIDsByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveExperimentInfoIDsByLabServer]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveExperimentInfoIDsByLabServer]
GO

/****** Object:  Stored Procedure dbo.RetrieveLSSPolicyByID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveLSSPolicyByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveLSSPolicyByID]
GO

/****** Object:  Stored Procedure dbo.RetrieveLSSPolicyIDsByExperiment    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveLSSPolicyIDsByExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveLSSPolicyIDsByExperiment]
GO

/****** Object:  Stored Procedure dbo.RetrieveLabServerName    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveLabServerName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveLabServerName]
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentByID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedExperimentByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedExperimentByID]
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedExperimentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedExperimentID]
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentIDByRecur    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedExperimentIDByRecur]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedExperimentIDByRecur]
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentInfoIDsByRecurrence    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedExperimentInfoIDsByRecurrence]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedExperimentInfoIDsByRecurrence]
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentInfoIDsByTimeBlock    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrievePermittedExperimentInfoIDsByTimeBlock]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrievePermittedExperimentInfoIDsByTimeBlock]
GO

/****** Object:  Stored Procedure dbo.RetrieveRecurrenceByID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveRecurrenceByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveRecurrenceByID]
GO

/****** Object:  Stored Procedure dbo.RetrieveRecurrenceIDs    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveRecurrenceIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveRecurrenceIDs]
GO

/****** Object:  Stored Procedure dbo.RetrieveRecurrenceIDsByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveRecurrenceIDsByLabServer]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveRecurrenceIDsByLabServer]
GO
/****** Object:  Stored Procedure dbo.RetrieveRecurrenceIDsByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveRecurrenceIDsByLabServerAndTime]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveRecurrenceIDsByLabServerAndTime]
GO
/****** Object:  Stored Procedure dbo.RetrieveRecurrenceIDsByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveRecurrenceIDsByResourceAndTime]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveRecurrenceIDsByResourceAndTime]
GO


/****** Object:  Stored Procedure dbo.RetrieveReservationInfoByID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveReservationInfoByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveReservationInfoByID]
GO

/****** Object:  Stored Procedure dbo.RetrieveReservationInfoIDByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveReservationInfoIDByLabServer]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveReservationInfoIDByLabServer]
GO

/****** Object:  Stored Procedure dbo.RetrieveReservationInfoIDs    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveReservationInfoIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveReservationInfoIDs]
GO

/****** Object:  Stored Procedure dbo.RetrieveReserveInfoIDsByExperiment    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveReserveInfoIDsByExperiment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveReserveInfoIDsByExperiment]
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockByID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveTimeBlockByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveTimeBlockByID]
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDs    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveTimeBlockIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveTimeBlockIDs]
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDsByGroup    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveTimeBlockIDsByGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveTimeBlockIDsByGroup]
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDsByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveTimeBlockIDsByLabServer]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveTimeBlockIDsByLabServer]
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDsByTimeChunk    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveTimeBlockIDsByTimeChunk]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveTimeBlockIDsByTimeChunk]
GO

/****** Object:  Stored Procedure dbo.RetrieveUSSInfoByID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveUSSInfoByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveUSSInfoByID]
GO

/****** Object:  Stored Procedure dbo.RetrieveUSSInfoID    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveUSSInfoID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveUSSInfoID]
GO

/****** Object:  Stored Procedure dbo.RetrieveUSSInfoIDs    Script Date: 5/2/2006 5:51:11 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RetrieveUSSInfoIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RetrieveUSSInfoIDs]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.AddCredentialSet    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.AddCredentialSet    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE AddCredentialSet

@serviceBrokerGUID varchar(50),
@serviceBrokerName varchar(256),
@groupName varchar(128),
@ussGUID varchar(50)

AS

insert into Credential_Sets(Service_Broker_GUID,Service_Broker_Name, Group_Name, USS_GUID) 
values (@serviceBrokerGUID,@serviceBrokerName,@groupName,@ussGUID)
select ident_current('Credential_Sets')

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.AddCredentialSet    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.GetCredentialSetID    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE GetCredentialSetID

@serviceBrokerGUID varchar(50),
@groupName varchar(128),
@ussGUID varchar(50)

AS
select Credential_Set_ID from Credential_Sets
where Service_Broker_GUID = @serviceBrokerGuid AND Group_Name = @groupName
AND USS_GUID = @ussGUID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO
/****** Object:  Stored Procedure dbo.RemoveCredentialSet    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RemoveCredentialSet    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RemoveCredentialSet

@serviceBrokerGUID varchar(50),
@serviceBrokerName varchar(256),
@groupName varchar(128),
@ussGUID varchar(50)

AS

delete  from Credential_Sets 
where Service_Broker_GUID=@serviceBrokerGUID and Service_Broker_Name=@serviceBrokerName 
and Group_Name=@groupName and USS_GUID=@ussGUID
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.AddExperimentInfo    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.AddExperimentInfo    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE AddExperimentInfo 
@labClientGUID varchar(50),
@labServerGUID varchar(50),
@labServerName varchar(256),
@labClientVersion varchar(50),
@labClientName varchar(256),
@providerName varchar(256),
@quantum int,
@prepareTime int,
@recoverTime int,
@minimumTime int,
@earlyArriveTime int

AS

insert into Experiment_Info(Lab_Client_GUID,Lab_Server_GUID, Lab_Server_Name, Lab_Client_Version, Lab_Client_Name,
 Provider_Name, Quantum, Prepare_Time, Recover_Time, Minimum_Time, Early_Arrive_Time) 
values (@labClientGuid, @labServerGUID, @labServerName, @labClientVersion,@labClientName, 
 @providerName, @quantum, @prepareTime, @recoverTime, @minimumTime,@earlyArriveTime)
select ident_current('Experiment_Info')

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.AddLSSPolicy    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.AddLSSPolicy    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE AddLSSPolicy 

@rule varchar(1024),
@experimentInfoID int,
@credentialSetID int

AS

insert into LSS_Policy([Rule], Experiment_Info_ID, Credential_Set_ID) values (@rule, @experimentInfoID,@credentialSetID)
select ident_current('LSS_Policy')

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.AddPermittedExperiment    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.AddPermittedExperiment    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE AddPermittedExperiment

@experimentInfoID int,
@recurrenceID int

AS

insert into Permitted_Experiments(Experiment_Info_ID, Recurrence_ID) values (@experimentInfoID, @recurrenceID)
select ident_current('Permitted_Experiments')
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE IsPermittedExperiment 

@experimentID int,
@recurrenceID int

AS

if (select count(experiment_Info_ID) from Permitted_Experiments 
where Experiment_Info_ID= @experimentID and  Recurrence_ID = @recurrenceID) > 0
return 1
else
return 0

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrievePermittedExperimentIDsForRecurrence 
@recurrenceID int
AS

select Experiment_Info_ID from Permitted_Experiments 
where Recurrence_ID = @recurrenceID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrievePermittedExperimentIDsForTimeBLock 
@timeBlockID int
AS

select Experiment_Info_ID 
from Permitted_Experiments pe, Time_Blocks tb
where pe.Recurrence_ID = tb.Recurrence_ID and tb.Time_Block_ID = @timeBlockID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeletePermittedExperiment    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.DeletePermittedExperiment    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE DeletePermittedExperiment 

@experimentID int,
@recurrenceID int

AS

delete from Permitted_Experiments 
where Experiment_Info_ID= @experimentID
and  Recurrence_ID = @recurrenceID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.AddPermittedGroup    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE AddPermittedGroup

@credentialSetID int,
@recurrenceID int

AS

insert into Permitted_Groups(Credential_Set_ID, Recurrence_ID) values (@credentialSetID, @recurrenceID)
select ident_current('Permitted_Groups')
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeletePermittedExperiment    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.DeletePermittedExperiment    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE DeletePermittedGroup 

@groupID int,
@recurrenceID int

AS

delete from Permitted_Groups 
where Credential_Set_ID= @groupID
and  Recurrence_ID = @recurrenceID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE IsPermittedGroup 

@groupID int,
@recurrenceID int

AS

if (select count(Credential_Set_ID) from Permitted_Groupss 
where Credential_Seto_ID= @groupID and  Recurrence_ID = @recurrenceID) > 0
return 1
else
return 0

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrievePermittedGroupIDsForRecurrence 
@recurrenceID int
AS

select Credential_Set_ID from Permitted_Groups 
where Recurrence_ID = @recurrenceID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE RetrievePermittedGroupIDsForTimeBlock 
@timeBlockID int
AS

select Credential_Set_ID from Permitted_Groups pg, Time_Blocks tb 
where pg.Recurrence_ID = tb.Recurrence_ID and tb.Time_Block_ID = @timeblockID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

/****** Object:  Stored Procedure dbo.AddRecurrence    Script Date: 5/2/2006 5:51:11 PM ******/
CREATE PROCEDURE AddRecurrence

@recurrenceStartDate datetime,
@recurrenceEndDate datetime,
@recurrenceType varchar(50),
@recurrenceStartTime int,
@recurrenceEndTime int,
@labServerGUID varchar(50),
@resourceID int,
@dayMask tinyint

AS

insert into Recurrence (Recurrence_Start_Date, Recurrence_End_Date, Recurrence_Type,Recurrence_Start_Time, Recurrence_End_Time, Lab_Server_GUID, Resource_ID,Day_Mask) 
values (@recurrenceStartDate, @recurrenceEndDate, @recurrenceType, @recurrenceStartTime, @recurrenceEndTime, @labServerGUID, @resourceID,@dayMask)
select ident_current('Recurrence')
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE GetRecurenceIDs
@credentialSetId int,
@experimentInfoId int
AS

select distinct recurrence_id from permitted_Groups where credential_set_id = @credentialSetId
  and recurrence_id in 
  (Select recurrence_ID from permitted_experiments where experiment_info_id = @experimentInfoID)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE PROCEDURE AddReservation
@experimentInfoID int,
@credentialSetID int,
@startTime datetime,
@endTime datetime


AS
declare 
@resourceID int

select @resourceID = resource_ID from recurrence where recurrence_ID in (select distinct recurrence_id from permitted_Groups where credential_set_id = @credentialSetId
  and recurrence_id in 
  (Select recurrence_ID from permitted_experiments where experiment_info_id = @experimentInfoID))

insert into Reservation_Info(Start_Time, End_Time, Experiment_Info_ID, Credential_Set_ID, resource_id) values (@startTime,@endTime,@experimentInfoID,@credentialSetID, @resourceID)
select ident_current('Reservation_Info')

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.AddReservationInfo    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.AddReservationInfo    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE AddReservationInfo
@serviceBrokerGUID varchar(50),
@groupName varchar(128),
@ussGUID varchar(50),
@clientGuid varchar(50),
@labServerGuid varchar(50),
@startTime datetime,
@endTime datetime


AS
declare 
@credentialSetID int,
@experimentInfoID int

select @credentialSetID=(select Credential_Set_ID from Credential_Sets where Service_Broker_GUID=@serviceBrokerGUID and Group_Name=@groupName and USS_GUID=@ussGUID)
select @experimentInfoID = (select Experiment_Info_ID from Experiment_Info where Lab_Client_Guid = @clientGuid and Lab_Server_Guid = @labServerGuid)
EXEC AddReservation @experimentInfoID,@credentialSetId,@startTime,@endTime

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.AddTimeBlock    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.AddTimeBlock    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE AddTimeBlock

@startTime datetime,
@endTime datetime,
@labServerGUID varchar(50),
@resourceID int,
@recurrenceID int

AS

insert into Time_Blocks(Start_Time, End_Time, Lab_Server_GUID, resource_ID, Recurrence_ID) 
values (@startTime,@endTime, @labServerGUID,@resourceID,@recurrenceID)
select ident_current('Time_Blocks')
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.AddUSSInfo    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.AddUSSInfo    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE AddUSSInfo

@ussGUID varchar(50),
@ussName varchar(256),
@ussURL varchar(256),
@couponId bigint,
@domainGuid varchar(50)


AS
insert into USS_Info(USS_GUID, USS_Name, USS_URL,coupon_id,domain_Guid) 
values (@ussGUID,@ussName, @ussURL,@couponID,@domainGuid) 
select ident_current('USS_Info')

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeleteCredentialSet    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.DeleteCredentialSet    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE DeleteCredentialSet

@credentialSetID int

AS

delete from Credential_Sets where Credential_Set_ID=@credentialSetID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeleteExperimentInfo    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.DeleteExperimentInfo    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE DeleteExperimentInfo 

@experimentInfoID int

AS

delete from Experiment_Info where Experiment_Info_ID= @experimentInfoID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeleteLSSPolicy    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.DeleteLSSPolicy    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE DeleteLSSPolicy

@lssPolicyID int

AS

delete from LSS_Policy where LSS_Policy_ID = @lssPolicyID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeleteRecurrence    Script Date: 5/2/2006 5:51:11 PM ******/
CREATE PROCEDURE DeleteRecurrence

@recurrenceID int

AS

delete from Recurrence where Recurrence_ID= @recurrenceID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeleteReservationInfo    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.DeleteReservationInfo    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE DeleteReservationInfo

@serviceBrokerGUID varchar(50),
@groupName varchar(128),
@ussGUID varchar(50),
@clientGuid varchar(50),
@labServerGuid varchar(50),
@startTime DateTime,
@endTime DateTime

 AS
declare
@experimentInfoID int,
@credentialSetID int

select @experimentInfoID=(select Experiment_Info_ID from Experiment_Info 
where Lab_Client_Guid=@clientGuid and Lab_Server_GUID=@labServerGuid)

select @credentialSetID=(select Credential_Set_ID from Credential_Sets 
where Service_Broker_GUID=@serviceBrokerGUID and Group_Name=@groupName and USS_GUID=@ussGUID)

delete from Reservation_Info 
where Start_Time=@startTime and End_Time=@endTime and Experiment_Info_ID=@experimentInfoID 
and Credential_Set_ID=@credentialSetID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeleteReservationInfoByID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.DeleteReservationInfoByID    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE DeleteReservationInfoByID

@reservationInfoID int 

AS

delete from Reservation_Info where Reservation_Info_ID= @reservationInfoID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeleteTimeBlock    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.DeleteTimeBlock    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE DeleteTimeBlock 

@timeBlockID bigint

AS

delete from Time_Blocks where Time_Block_ID= @timeBlockID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.DeleteUSSInfo    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.DeleteUSSInfo    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE DeleteUSSInfo

@ussInfoID int

AS

delete from USS_Info where USS_Info_ID=@ussInfoID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.ModifyCredentialSet    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.ModifyCredentialSet    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE ModifyCredentialSet
@credentialSetID int,
@serviceBrokerGUID varchar(50),
@serviceBrokerName varchar(256),
@groupName varchar(50),
@ussGUID varchar(50)

 AS

update Credential_Sets set Service_Broker_GUID=@serviceBrokerGUID, Service_Broker_Name=@serviceBrokerName, 
Group_Name=@groupName, USS_GUID=@ussGUID where Credential_Set_ID=@credentialSetID
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

/****** Object:  Stored Procedure dbo.ModifyExperimentInfo    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.ModifyExperimentInfo    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE ModifyExperimentInfo

@experimentInfoID int,
@labClientGUID varchar(50),
@labServerGUID varchar(50),
@labServerName varchar(256),
@labClientVersion varchar(50),
@labClientName varchar(256),
@providerName varchar(256),
@quantum int,
@prepareTime int,
@recoverTime int,
@minimumTime int,
@earlyArriveTime int

 AS

update Experiment_Info set Lab_Client_GUID=@labClientGUID,Lab_Server_GUID=@labServerGUID, 
Lab_Server_Name=@labServerName, Lab_Client_Version=@labClientVersion,Lab_Client_Name=@labClientName, 
Provider_Name=@providerName, Quantum=@quantum, Prepare_Time=@prepareTime, Recover_Time=@recoverTime, 
Minimum_Time=@minimumTime, Early_Arrive_Time=@earlyArriveTime 
where Experiment_Info_ID=@experimentInfoID

select @@rowcount

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

/****** Object:  Stored Procedure dbo.ModifyLSSPolicy    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.ModifyLSSPolicy    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE ModifyLSSPolicy

@lssPolicyID int,
@credentialSetID int,
@experimentInfoID int,
@rule varchar(1024)

AS

update LSS_Policy set Credential_Set_ID = @credentialSetID,Experiment_Info_ID=@experimentInfoID, [Rule]=@rule where LSS_Policy_ID= @lssPolicyID

select @@rowcount

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

/****** Object:  Stored Procedure dbo.ModifyTimeBlock    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.ModifyTimeBlock    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE ModifyTimeBlock

@timeBlockID bigint,
@labServerGUID varchar(50),
@resourceID int,
@startTime datetime,
@endTime datetime

AS

update Time_blocks set Lab_Server_GUID=@labServerGUID, Resource_ID=@resourceID, 
Start_Time=@startTime, End_Time=@endTime where Time_Block_ID=@timeBlockID
select @@rowcount

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.ModifyUSSInfo    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.ModifyUSSInfo    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE ModifyUSSInfo
@ussInfoID int,
@ussGUID varchar(50),
@ussName varchar(256),
@ussURL varchar(256),
@couponId bigint,
@domainGuid varchar(50)


AS

update USS_Info set USS_GUID=@ussGUID, USS_Name=@ussName, USS_URL=@ussURL,
coupon_id=@couponId, domain_guid=@domainGuid
where USS_Info_ID=@ussInfoID

select @@rowcount

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

/****** Object:  Stored Procedure dbo.RetrieveCredentialSetByID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveCredentialSetByID    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveCredentialSetByID
@credentialSetID int

AS

select Service_Broker_GUID, Service_Broker_Name, Group_Name, USS_GUID from Credential_Sets 
where Credential_Set_ID=@credentialSetID

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

/****** Object:  Stored Procedure dbo.RetrieveCredentialSetIDs    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveCredentialSetIDs    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveCredentialSetIDs

 AS

select Credential_Set_ID from Credential_Sets

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoByID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoByID    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveExperimentInfoByID 
@experimentInfoID int

AS

select Lab_Client_GUID, Lab_Server_GUID, Lab_Server_Name, Lab_Client_Version, Lab_Client_Name,
Provider_Name, Quantum, Prepare_Time, Recover_Time, Minimum_Time, Early_Arrive_Time 
from Experiment_Info where Experiment_Info_ID=@experimentInfoID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoIDByExperiment    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoIDByExperiment    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveExperimentInfoIDByExperiment

@clientGuid varchar(50),
@labServerGuid varchar(50)

AS

select Experiment_Info_ID from Experiment_Info 
where Lab_Client_Guid=@clientGuid and Lab_Server_Guid=@labServerGuid

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoIDs    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoIDs    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE RetrieveExperimentInfoIDs
 AS

select Experiment_Info_ID from Experiment_Info

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoIDsByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveExperimentInfoIDsByLabServer    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveExperimentInfoIDsByLabServer

@labServerGUID varchar(50)

AS

select Experiment_Info_ID from Experiment_info where Lab_Server_GUID = @labServerGUID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveLSSPolicyByID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveLSSPolicyByID    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE RetrieveLSSPolicyByID
@lssPolicyID int
 
AS

select [Rule], Experiment_Info_ID, Credential_Set_ID from LSS_Policy where LSS_Policy_ID=@lssPolicyID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveLSSPolicyIDsByExperiment    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveLSSPolicyIDsByExperiment    Script Date: 4/11/2006 6:19:41 PM ******/
CREATE PROCEDURE RetrieveLSSPolicyIDsByExperiment
@experimentInfoID int
AS

select LSS_Policy_ID from LSS_Policy where Experiment_Info_ID=@experimentInfoID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveLabServerName    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveLabServerName    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveLabServerName

@labServerGUID varchar(50)

AS

select DISTINCT Lab_Server_Name from Experiment_Info where Lab_Server_GUID = @labServerGUID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentByID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentByID    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrievePermittedExperimentByID 

@permittedExperimentID int

AS

select Experiment_Info_ID, Recurrence_ID from Permitted_Experiments where Permitted_Experiment_ID=@permittedExperimentID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentID    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrievePermittedExperimentID

@experimentInfoID int,
@timeBlockID bigint

AS

if (select count(*) from Permitted_Experiments 
where Experiment_Info_ID=@experimentInfoID 
and Recurrence_ID= ( select Recurrence_ID from Time_Blocks where Time_Block_ID=@timeBlockID)) ! = 0

select Permitted_Experiment_ID from Permitted_Experiments where Experiment_Info_ID=@experimentInfoID and  Recurrence_ID= ( select Recurrence_ID from Time_Blocks where Time_Block_ID=@timeBlockID)

else
select 'Return Status' = -1
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentIDByRecur    Script Date: 5/2/2006 5:51:11 PM ******/
CREATE PROCEDURE RetrievePermittedExperimentIDByRecur

@experimentInfoID int,
@recurrenceID int

AS

if (select count(*) from Permitted_Experiments where Experiment_Info_ID=@experimentInfoID and Recurrence_ID=@recurrenceID) ! = 0
select Permitted_Experiment_ID from Permitted_Experiments where Experiment_Info_ID=@experimentInfoID and  Recurrence_ID= @recurrenceID

else
select 'Return Status' = -1
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentIDByRecur    Script Date: 5/2/2006 5:51:11 PM ******/
CREATE PROCEDURE RetrievePermittedGroupIDByRecur

@groupID int,
@recurrenceID int

AS

if (select count(*) from Permitted_Groups where Credential_Set_ID=@groupID and Recurrence_ID=@recurrenceID) ! = 0
select Credential_Set_ID from Permitted_Groups where Credential_Set_ID=@groupID and  Recurrence_ID= @recurrenceID

else
select 'Return Status' = -1
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentInfoIDsByRecurrence    Script Date: 5/2/2006 5:51:11 PM ******/

CREATE PROCEDURE RetrievePermittedExperimentInfoIDsByRecurrence
@recurrenceID int

AS

select Experiment_Info_ID from Permitted_Experiments where   Recurrence_ID= @recurrenceID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentInfoIDsByTimeBlock    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrievePermittedExperimentInfoIDsByTimeBlock    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrievePermittedExperimentInfoIDsByTimeBlock
@timeBlockID bigint

AS

select Experiment_Info_ID from Permitted_Experiments where   Recurrence_ID= ( select Recurrence_ID from Time_Blocks where Time_Block_ID=@timeBlockID)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveRecurrenceByID    Script Date: 5/2/2006 5:51:11 PM ******/

CREATE PROCEDURE RetrieveRecurrenceByID
@recurrenceID int

AS

select Recurrence_Start_Date, Recurrence_End_Date, Recurrence_Type,  Recurrence_Start_Time, Recurrence_End_Time, Lab_Server_GUID, Resource_ID, Day_Mask from Recurrence where Recurrence_ID=@recurrenceID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveRecurrenceIDs    Script Date: 5/2/2006 5:51:11 PM ******/
CREATE PROCEDURE RetrieveRecurrenceIDs

AS

select Recurrence_ID from Recurrence order by Recurrence_Start_Date asc
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveRecurrenceIDsByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/
CREATE PROCEDURE RetrieveRecurrenceIDsByLabServer

@labServerGUID varchar(50)

AS

select Recurrence_ID from Recurrence where Lab_Server_GUID= @labServerGUID order by Recurrence_Start_Date asc
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveRecurrenceIDsByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/
CREATE PROCEDURE RetrieveRecurrenceIDsByLabServerAndTime

@labServerGUID varchar(50),
@start DateTime,
@end DateTime

AS

select Recurrence_ID from Recurrence 
where Lab_Server_GUID= @labServerGUID
AND(
 (DateAdd(ss,Recurrence_Start_Time,Recurrence_Start_Date ) <= @end)
  AND (DateAdd(ss,Recurrence_End_Time,Recurrence_End_Date ) >= @start)
)
order by Recurrence_Start_Date asc
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE RetrieveRecurrenceIDsByResourceAndTime

@resourceID int,
@start DateTime,
@end DateTime

AS

select Recurrence_ID from Recurrence 
where resource_ID= @resourceID
AND(
 (DateAdd(ss,Recurrence_Start_Time,Recurrence_Start_Date ) <= @end)
  AND (DateAdd(ss,Recurrence_End_Time,Recurrence_End_Date ) >= @start)
)
order by Recurrence_Start_Date asc
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveReservationInfoByID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveReservationInfoByID    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveReservationInfoByID
@reservationInfoID numeric
 AS
select Start_Time, End_Time, Experiment_Info_ID, Credential_Set_ID from Reservation_Info where Reservation_Info_ID=@reservationInfoID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveReservationInfoIDByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveReservationInfoIDByLabServer    Script Date: 4/11/2006 6:19:42 PM ******/


CREATE PROCEDURE RetrieveReservationInfoIDByLabServer

@labServerGUID varchar(50),
@startTime DateTime,
@endTime DateTime

AS
select R. Reservation_Info_ID from Reservation_Info AS R Join Experiment_Info AS E on (R.Experiment_Info_ID = E.Experiment_Info_ID) where E.Lab_server_GUID = @labServerGUID and R.End_Time>@startTime and R.Start_Time<@endTime ORDER BY R.Start_Time asc

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveReservationInfoIDs    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveReservationInfoIDs    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveReservationInfoIDs

@serviceBrokerGUID varchar(50),
@groupName varchar(128),
@ussGUID varchar(50),
@clientGuid varchar(50),
@labServerGuid varchar(50),
@startTime datetime,
@endTime datetime

 AS
declare
@credentialSetID int,
@experimentInfoID int

select @credentialSetID=Credential_Set_ID from Credential_Sets where Service_Broker_GUID=@serviceBrokerGUID and Group_Name=@groupName and USS_GUID=@ussGUID
select @experimentInfoID=Experiment_Info_ID from Experiment_Info where Lab_Client_Guid=@clientGuid and Lab_Server_Guid=@labServerGuid
select Reservation_Info_ID from Reservation_Info where Credential_Set_ID=@credentialSetID and Experiment_Info_ID=@experimentInfoID and End_Time>@startTime and Start_Time<@endTime ORDER BY Start_Time asc

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveReserveInfoIDsByExperiment    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveReserveInfoIDsByExperiment    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveReserveInfoIDsByExperiment
@experimentInfoID int

AS 

select Reservation_Info_ID from Reservation_Info where Experiment_Info_ID=@experimentInfoID order by Start_Time asc

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockByID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockByID    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveTimeBlockByID
@timeBlockID bigint

AS

select Start_Time, End_Time, Lab_Server_GUID, resource_ID, Recurrence_ID from Time_Blocks where Time_Block_ID=@timeBlockID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDs    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDs    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveTimeBlockIDs

AS

select Time_Block_ID from Time_Blocks  order by Start_Time asc

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDsByGroup    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDsByGroup    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveTimeBlockIDsByGroup

@labserverGUID varchar(50),
@credentialSetID int

AS

select Time_Block_ID from Time_Blocks where Lab_Server_GUID=@labServerGUID and resource_ID=@credentialSetID  
order by Start_Time asc

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDsByLabServer    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDsByLabServer    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveTimeBlockIDsByLabServer

@labServerGUID varchar(50)

AS

select Time_Block_ID from Time_Blocks 
where Lab_Server_GUID=@labServerGUID order by Start_Time asc

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDsByTimeChunk    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveTimeBlockIDsByTimeChunk    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveTimeBlockIDsByTimeChunk

@serviceBrokerGUID varchar(50),
@groupName varchar(128),
@labClientGuid varchar(50),
@labServerGUID varchar(50),
@ussGUID varchar(50),
@startTime DateTime,
@endTime DateTime

AS
declare @credentialSetID int
declare @expID int

select @credentialSetID=(select Credential_Set_ID from Credential_Sets 
where Service_Broker_GUID=@serviceBrokerGUID and Group_Name=@groupName and USS_GUID=@ussGUID)

select @expID = (select Experiment_info_ID from Experiment_Info
where lab_client_Guid = @labClientGuid and Lab_Server_Guid = @labServerGuid)

select Time_Block_ID from Time_Blocks 
where recurrence_id in (
	select recurrence_id from permitted_Groups where credential_set_id = @credentialSetId
	and recurrence_id in 
	(Select recurrence_ID from permitted_experiments where experiment_info_id = @expID)
	) 
and (End_Time>=@startTime and Start_Time<=@endTime) order by Start_Time asc

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveUSSInfoByID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveUSSInfoByID    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveUSSInfoByID

@ussInfoID int

AS

select USS_GUID, USS_Name, USS_URL, coupon_id, domain_Guid
from USS_Info where USS_Info_ID=@ussInfoID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveUSSInfoID    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveUSSInfoID    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveUSSInfoID
@ussGuid varchar(50)

AS

select USS_Info_ID from USS_Info where USS_GUID= @ussGuid

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.RetrieveUSSInfoIDs    Script Date: 5/2/2006 5:51:11 PM ******/

/****** Object:  Stored Procedure dbo.RetrieveUSSInfoIDs    Script Date: 4/11/2006 6:19:42 PM ******/
CREATE PROCEDURE RetrieveUSSInfoIDs
 AS
select USS_Info_ID from USS_Info

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
--
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE Resource_AddGetID
@guid varchar (50),
@name varchar (256)
AS
if( select count(resource_ID) from LS_Resources where Lab_Server_Guid = @guid) >0
select resource_id from LS_Resources where Lab_Server_Guid = @guid
else
BEGIN
insert into LS_Resources (Lab_Server_Guid, Lab_Server_Name)
values (@guid,@name)
select ident_current('LSS_Resources')
END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

--
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE Resource_Get
@id int
AS
SELECT resource_id,Lab_Server_Guid,Lab_Server_Name,description 
from LS_resources where resource_id = @id
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

--
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE Resource_GetByGuid
@guid varchar(50)
AS
SELECT resource_id,Lab_Server_Guid,Lab_Server_Name,description 
from LS_resources where Lab_Server_Guid = @guid
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


--
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE Resource_GetTags
AS

Select resource_id, Lab_Server_Name + ': ' + isnull(description,'')  from LS_Resources

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
--
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE Resource_GetTagsByGuid
@guid varchar (256)
AS

Select resource_id, Lab_Server_Name + ': ' + isnull(description ,'')
from LS_Resources where lab_Server_Guid = @guid
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

--
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE Resource_Delete
@id int
AS
DELETE from LS_Resources where resource_id = @id
select @@rowcount
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

--
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE Resource_Insert
@guid varchar (50),
@name varchar (256),
@description varchar (1024)
AS
insert into LS_Resources (Lab_Server_Guid, Lab_Server_Name,description)
values (@guid,@name,@description)
select ident_current('LSS_Resources')
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
--
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE Resource_SetDescription
@id int,
@description varchar (1024)
AS
UPDATE LS_Resources set description=@description
where resource_id = @id
select @@rowcount
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
