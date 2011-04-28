/**** SERVICE BROKER STORED PROCEDURES ****/

-- Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
-- $Id$

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[IsSuperuser]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[IsSuperuser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[getUserExperimentIDs]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[getUserExperimentIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AdminURL_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AdminURL_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AdminURL_DeletebyID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AdminURL_DeletebyID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AdminURL_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AdminURL_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AdminURL_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AdminURL_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AdminURLs_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AdminURLs_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AdminUrl_UpdateCodebase]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AdminUrl_UpdateCodebase]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AgentGroup_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AgentGroup_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AgentHierarchy_RetrieveTable]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AgentHierarchy_RetrieveTable]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AgentType_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[AgentType_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Agent_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Agent_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Agents_RetrieveTable]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Agents_RetrieveTable]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Authorization_RetrieveExpIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Authorization_RetrieveExpIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Authorization_RetrieveExpIDsCriteria]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Authorization_RetrieveExpIDsCriteria]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Blob_SaveXMLExtensionSchemaURL]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Blob_SaveXMLExtensionSchemaURL]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientIDs_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientIDs_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientInfo_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientInfo_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientInfo_DeleteByClient]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientInfo_DeleteByClient]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientInfo_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientInfo_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientInfo_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientInfo_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientInfo_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientInfo_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientInfo_UpdateOrder]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientInfo_UpdateOrder]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientItem_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientItem_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientItem_RetrieveNames]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientItem_RetrieveNames]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientItem_RetrieveValue]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientItem_RetrieveValue]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientItem_Save]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientItem_Save]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClientTypes_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClientTypes_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Client_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Client_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Client_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Client_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Client_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Client_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Client_RetrieveByGuid]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Client_RetrieveByGuid]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Client_RetrieveID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Client_RetrieveID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Client_RetrieveLoaderScript]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Client_RetrieveLoaderScript]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Client_RetrieveTags]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Client_RetrieveTags]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Client_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Client_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Client_UpdateLoaderScript]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Client_UpdateLoaderScript]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ExperimentCouponID_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ExperimentCouponID_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ExperimentCoupon_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ExperimentCoupon_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ExperimentCoupon_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ExperimentCoupon_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ExperimentCoupon_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ExperimentCoupon_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ExperimentIDs_RetrieveByGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ExperimentIDs_RetrieveByGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ExperimentStatus_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ExperimentStatus_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ExperimentStatus_UpdateCode]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ExperimentStatus_UpdateCode]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ExperimentSummary_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ExperimentSummary_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_Close]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_Close]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_Create]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_Create]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_RetrieveActiveIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_RetrieveActiveIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_RetrieveAdminInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_RetrieveAdminInfo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_RetrieveAdminInfos]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_RetrieveAdminInfos]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_RetrieveAnnotation]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_RetrieveAnnotation]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_RetrieveEssInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_RetrieveEssInfo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_RetrieveGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_RetrieveGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_RetrieveOwner]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_RetrieveOwner]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_RetrieveRawData]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_RetrieveRawData]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_RetrieveStatusCode]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_RetrieveStatusCode]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_UpdateAnnotation]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_UpdateAnnotation]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Experiment_UpdateOwner]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Experiment_UpdateOwner]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Grant_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Grant_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Grant_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Grant_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Grants_RetrieveTable]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Grants_RetrieveTable]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_RetrieveIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_RetrieveIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_AddMember]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_AddMember]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_DeleteMember]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_DeleteMember]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_RetrieveAdminGroupID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_RetrieveAdminGroupID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_RetrieveAdminGroupIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_RetrieveAdminGroupIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_RetrieveAssociatedGroupID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_RetrieveAssociatedGroupID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_RetrieveID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_RetrieveID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_RetrieveMembers]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_RetrieveMembers]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_RetrieveName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_RetrieveName]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_RetrieveRequestGroupID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_RetrieveRequestGroupID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Group_Update]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LabServerClient_ClientIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[LabServerClient_ClientIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LabServerClient_Clients]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[LabServerClient_Clients]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LabServerClient_Count]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[LabServerClient_Count]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LabServerClient_CountScheduledClients]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[LabServerClient_CountScheduledClients]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LabServerClient_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[LabServerClient_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LabServerClient_DeleteAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[LabServerClient_DeleteAll]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LabServerClient_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[LabServerClient_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LabServerClient_RetrieveServerIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[LabServerClient_RetrieveServerIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[NativePassword_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[NativePassword_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[NativePassword_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[NativePassword_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[NativePrincipal_Create]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[NativePrincipal_Create]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[NativePrincipal_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[NativePrincipal_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[NativePrincipals_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[NativePrincipals_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ProcesAgent_RetrieveAdminServiceTags]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ProcesAgent_RetrieveAdminServiceTags]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ProcessAgent_RetrieveAdminGrants]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ProcessAgent_RetrieveAdminGrants]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ProcessAgent_RetrieveAdminTags]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ProcessAgent_RetrieveAdminTags]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[QualifierHierarchy_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[QualifierHierarchy_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[QualifierHierarchy_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[QualifierHierarchy_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[QualifierHierarchy_RetrieveTable]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[QualifierHierarchy_RetrieveTable]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Qualifier_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Qualifier_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Qualifier_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Qualifier_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Qualifier_UpdateName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Qualifier_UpdateName]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Qualifiers_RetrieveTable]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Qualifiers_RetrieveTable]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Registration_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Registration_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Registration_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Registration_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Registration_RetrieveByGuid]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Registration_RetrieveByGuid]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Registration_UpdateStatus]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Registration_UpdateStatus]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Registrations_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Registrations_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Registrations_RetrieveByRange]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Registrations_RetrieveByRange]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Registrations_RetrieveByStatus]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Registrations_RetrieveByStatus]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapId_RetrieveByKeyValue]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapId_RetrieveByKeyValue]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapIds_RetrieveByValue]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapIds_RetrieveByValue]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapKey_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapKey_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapResourceType_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapResourceType_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapStringTag_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapStringTag_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapString_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapString_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapString_RetrieveByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapString_RetrieveByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapString_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapString_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapTypeNames_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapTypeNames_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapTypeString_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapTypeString_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapTypeStrings_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapTypeStrings_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapType_RetrieveByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapType_RetrieveByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapValue_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapValue_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMap_RetrieveByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMap_RetrieveByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMap_RetrieveIDsByKey]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMap_RetrieveIDsByKey]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMap_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMap_delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResourceMapIDs_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResourceMapIDs_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ResultXMLExtensionSchemaURL_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ResultXMLExtensionSchemaURL_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SessionHistory_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SessionHistory_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SessionInfo_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SessionInfo_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SystemMessage_DeleteByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemMessage_DeleteByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SystemMessage_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemMessage_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SystemMessage_RetrieveByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemMessage_RetrieveByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SystemMessage_RetrieveByIDForAdmin]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemMessage_RetrieveByIDForAdmin]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SystemMessage_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemMessage_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SystemMessages_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemMessages_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SystemMessages_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemMessages_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SystemMessages_RetrieveByGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemMessages_RetrieveByGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SystemMessages_RetrieveForAdmin]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[SystemMessages_RetrieveForAdmin]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TicketCollection_Count]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[TicketCollection_Count]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserSession_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UserSession_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserSession_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UserSession_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserSession_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UserSession_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserSession_UpdateClient]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UserSession_UpdateClient]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserSession_UpdateEndTime]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UserSession_UpdateEndTime]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserSession_UpdateGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UserSession_UpdateGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserSession_UpdateKey]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UserSession_UpdateKey]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserSession_UpdateTZOffset]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UserSession_UpdateTZOffset]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserSessions_RetrieveAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UserSessions_RetrieveAll]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_Retrieve]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_Retrieve]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_RetrieveEmail]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_RetrieveEmail]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_RetrieveGroups]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_RetrieveGroups]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_RetrieveID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_RetrieveID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_RetrieveIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_RetrieveIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_RetrieveName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_RetrieveName]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_RetrieveOrphanedIDs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_RetrieveOrphanedIDs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_Update]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/** $Id$ **/


/** CREATE FUNCTIONS **/

create Function IsSuperuser(@userid int,@groupid int)
returns bit
as
BEGIN
declare @status bit 
SET @status = 0 
if (select user_id from users where user_name ='superUser') = @userid
BEGIN
SET @status = 1
END
if (select group_ID from groups where group_name = 'SuperUserGroup') = @groupid
BEGIN
SET @status =1
END
return @status
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


create function getUserExperimentIDs(@userid int,@groupid int)
returns table
AS
Return ( SELECT  experiment_ID from experiments
WHERE (USER_ID = @userID) or group_id in
(select qualifier_reference_ID from qualifiers
where qualifier_type_id = 6 
and Qualifier_id in (select qualifier_id from grants 
where agent_id = @groupid and function_id = 5))

)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



CREATE PROCEDURE AdminURL_Delete
@processAgentID int,
@adminURL nvarchar (512),
@ticketType varchar (100)
AS
DECLARE @ticket_Type_ID int

select @Ticket_Type_ID = (select Ticket_Type_ID  from Ticket_Type where (upper(Name) = upper(@ticketType )))
delete from AdminURLs  where ProcessAgentID = @processAgentID and  AdminURL = @adminURL and Ticket_Type_ID = @ticket_Type_ID;
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


CREATE PROCEDURE AdminURL_DeletebyID

       @adminURLID int

AS
       delete from AdminURLs
        where id= @adminURLID
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



CREATE PROCEDURE AdminURL_Insert

@processAgentID int,
@adminURL nvarchar (512),
@ticketType varchar (100)
AS
DECLARE @ticket_Type_ID int

select @Ticket_Type_ID = (select Ticket_Type_ID  from Ticket_Type where (upper(Name) = upper(@ticketType )))

insert into AdminURLs(ProcessAgentID, AdminURL, Ticket_Type_ID) values (@processAgentID, @adminURL, @ticket_Type_ID)
select ident_current('AdminURLs')

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE AdminURL_Update
@id int,
@url nvarchar(512),
@typeID int
AS
update AdminURLs set AdminURL=@url where ProcessAgentID=@id and Ticket_Type_ID = @typeID
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


CREATE PROCEDURE AdminURLs_Retrieve 

@processAgentID int
AS

SELECT      AdminURLs.ID, AdminURLs.ProcessAgentID, AdminURLs.AdminURL, Ticket_Type.Name
FROM         AdminURLs INNER JOIN
                      Ticket_Type ON AdminURLs.Ticket_Type_ID = Ticket_Type.Ticket_Type_ID
WHERE     (AdminURLs.ProcessAgentID = @processAgentID and AdminURLs.Ticket_Type_ID = Ticket_Type.Ticket_Type_ID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE AdminUrl_UpdateCodebase
@id int,
@old nvarchar(512),
@new nvarchar(512)
AS
update AdminURLs set AdminURL=REPLACE(AdminURL,@old,@new) where ProcessAgentID=@id
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



/****** Object:  Stored Procedure dbo.RetrieveAgentGroup    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE AgentGroup_Retrieve
	@agentID int
AS
	select parent_group_ID 
	from   agent_hierarchy
	where agent_ID = @agentID --and (parent_group_id!= (select group_id from groups where group_name='ROOT'))
	-- don't select root as a group. root is parent for everyone!

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveAgentHierarchyTable    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE AgentHierarchy_RetrieveTable
AS
	select agent_id,parent_group_id
	from agent_hierarchy
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


/****** Object:  Stored Procedure dbo.RetrieveAgentType    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE AgentType_Retrieve
	@agentID int
AS
	select  is_group
	from  agents
	where (agent_id=@agentID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveAgent    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Agent_Retrieve
	@agentID int
AS
	select agent_ID, is_group
	from   agents
	where agent_ID = @agentID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Agents_RetrieveTable
-- Procedure was added by Karim - need this for constructing the tree control
AS
	select agent_id, is_group
	from agents
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


create procedure Authorization_RetrieveExpIDs
@userid int,
@groupId int
AS
 
declare @exps TABLE (experiment_id bigint,ess_id int)

declare @status bit
SET @status = dbo.IsSuperuser(@userid,@groupid)
if @status = 1
BEGIN
insert @exps SELECT  experiment_ID,ess_id from experiments
END
ELSE
BEGIN
insert @exps SELECT  experiment_ID,ess_id from experiments
WHERE (USER_ID = @userID) or group_id in
(select qualifier_reference_ID from qualifiers
where qualifier_type_id = 6 
and Qualifier_id in (select qualifier_id from grants 
where agent_id = @groupid and function_id = 5))
END
select * from @exps

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


create procedure Authorization_RetrieveExpIDsCriteria
@userid int,
@groupId int,
@criteria varchar (7000)
AS

declare @queryStr varchar(7500)

create table #expsFiltered (experiment_id bigint, ess_id int)
declare @status bit
SET @status = dbo.IsSuperuser(@userid,@groupid)
if @status = 0
BEGIN
-- select * from userExperiments(@userid,@groupid)
	create table #exps (experiment_id bigint)
	insert #exps SELECT  experiment_ID from experiments
		WHERE (USER_ID = @userID) or group_id in
		(select qualifier_reference_ID from qualifiers
		where qualifier_type_id = 6 
		and Qualifier_id in (select qualifier_id from grants 
		where agent_id = @groupid and function_id = 5))

	if @criteria is not null
	BEGIN
		select @queryStr = 'insert #expsFiltered select experiment_id,ess_id from experiments where experiment_id in ( '
			 + 'select experiment_id from #exps ) AND ( ' + @criteria + ')'

		EXEC(@queryStr)
	END
	ELSE
	BEGIN
		insert #expsFiltered select experiment_id,ess_id from experiments 
		where experiment_id in ( select experiment_id from #exps )
		
	END
	drop table #exps
END

ELSE
BEGIN

	if @criteria is not null
	BEGIN
		select @queryStr = 'insert #expsFiltered select experiment_id,ess_id from experiments where '
			+  @criteria 
		EXEC(@queryStr)
	END
	ELSE
	BEGIN
		insert #expsFiltered select experiment_id,ess_id from experiments 
	END
END

select experiment_id, ess_id from #expsFiltered
select distinct ess_id from #expsFiltered where ess_id IS NOT NULL

drop table #expsFiltered
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


/****** Object:  Stored Procedure dbo.SaveBlobXMLExtensionSchemaURL    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Blob_SaveXMLExtensionSchemaURL
 	@labserverID varchar(256),
	@URL nvarchar(512)
AS
/* hardcoded account ID*/
	update accounts
	set blob_XML_Extension_schema_URL = @URL
	where account_ID = 2

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveLabClientIDs    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE ClientIDs_Retrieve
AS
	select client_id
	from lab_clients where client_ID > 0

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE ClientInfo_Delete
	@infoID int,
	@clientID int

AS
delete from client_info 
	where client_info_id=@infoID and client_ID = @clientID
select @@rowcount

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE ClientInfo_DeleteByClient
	@labClientID int

AS
	delete from client_info 
		where client_id=@labClientID
	select @@rowcount

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/** CREATE STORED PROCEDURES **/




CREATE PROCEDURE ClientInfo_Insert
	@labClientID int,
	@infoURL nvarchar (512),
	@infoName nvarchar(256),
	@description nvarchar(2048),
	@displayOrder int
AS
	insert into client_info (client_id,info_url,info_name,description, display_order)
		values (@labClientID, @infoURL,  @infoName, @description, @displayOrder)
 	select ident_current('Client_Info')


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




/****** Object:  Stored Procedure dbo.RetrieveClientInfo    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE ClientInfo_Retrieve
	@labClientID int
AS
	select client_info_id, info_URL, info_name, display_order, description
	from client_info
	where client_id=@labClientID
	order by display_order

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE ClientInfo_Update
	@clientInfoID int,
	@clientID int,
	@infoURL nvarchar (512),
	@infoName nvarchar(256),
	@description nvarchar(2048),
	@displayOrder int
AS
	UPDATE Client_Info set info_url=@infoURL,info_name=@infoName,description=@description, display_order=@displayOrder
		where Client_Info_ID = @clientInfoID AND Client_ID = @clientID
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

CREATE PROCEDURE ClientInfo_UpdateOrder
	@clientInfoID int,
	@displayOrder int
AS
	update Client_Info set display_order = @displayOrder
	where Client_Info_ID = @clientInfoID
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


/****** Object:  Stored Procedure dbo.DeleteClientItem    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE ClientItem_Delete
	@clientID int,
	@userID int,
	@itemName nvarchar(256)
AS
		delete from client_items 
		where client_id = @ClientID and user_ID=@userID and item_Name = @itemName

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


/****** Object:  Stored Procedure dbo.RetrieveClientItemNames    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE ClientItem_RetrieveNames
	@clientID int,
	@userID int
AS
	select item_name
	 from client_items
	where client_id = @ClientID and user_ID=@userID;
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

CREATE PROCEDURE ClientItem_RetrieveValue
	@clientID int,
	@userID int,
	@itemName nvarchar(256)
AS
	select item_value 
	from client_items
	where client_id = @ClientID and user_ID=@userID and item_Name = @itemName;
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


/****** Object:  Stored Procedure dbo.SaveClientItem    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE ClientItem_Save
	@clientID int,
	@userID int,
	@itemName nvarchar(256),
	@itemValue ntext
AS
	DECLARE @clientItemID bigint
	select @clientItemID = (select client_item_id from client_items where client_id = @ClientID and user_ID=@userID and item_Name = @itemName);
	if (@clientItemID is not null) 
	begin
		update Client_Items
		set item_Value = @itemValue
		where client_item_ID = @clientItemID;
		if (@@error > 0)
			goto on_error
	end
	else
	begin
	begin transaction
		insert into client_items (user_id,client_id, item_name,item_value)
		values (@userID, @clientID, @itemName,  @itemValue)
		if (@@error > 0)
			goto on_error
	commit transaction
	end
	return
	on_error: 
	ROLLBACK TRANSACTION
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

CREATE PROCEDURE ClientTypes_Retrieve
AS
	select description
	from client_types

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Client_Delete
	@labClientID int
AS
BEGIN TRANSACTION
	delete from Lab_Clients
	where Client_ID = @labClientID;
	if (@@error > 0)
		goto on_error
	
	delete from Qualifiers
	where qualifier_reference_ID = @labClientID and 
	qualifier_type_ID = (select qualifier_type_ID from qualifier_Types 
				where description = 'Lab Client');
	if (@@error > 0)
		goto on_error
COMMIT TRANSACTION	
return 1
	on_error: 
	ROLLBACK TRANSACTION

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.AddLabClient    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Client_Insert
	@guid varchar(50),
	@labClientName nvarchar(256),
	@shortDescription nvarchar(256),
	@longDescription ntext,
	@version nvarchar(50),
	@loaderScript nvarchar(2000),
	@clientType varchar (100),
	@email nvarchar(256),
	@firstName nvarchar(128),
	@lastName nvarchar(128),
	@notes nvarchar(2048),
	@docURL nvarchar (512),
	@needsScheduling bit,
	@needsESS bit,
	@isReentrant bit
AS
		DECLARE @clientTypeID INT
		
		SELECT @clientTypeID = (SELECT client_type_id FROM client_types 
							WHERE upper(description) = upper(@clientType))
							
		INSERT INTO lab_clients (Client_Guid, lab_client_name, short_description, long_description,version, 
				loader_script, client_type_ID, contact_email, contact_first_name, 
				contact_last_name, notes,Documentation_URL,NeedsScheduling,NeedsESS,IsReentrant)
		VALUES (@guid, @labClientName, @shortDescription, @longDescription, @Version,
				@loaderScript,@clientTypeID, @email, @firstName, @lastName, @notes,
				@docURL,@needsScheduling,@needsESS,@isReentrant)
		
		SELECT ident_current('lab_clients');
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


/****** Object:  Stored Procedure dbo.RetrieveLabClient    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Client_Retrieve
	@labClientID int
AS
	select client_guid, lab_client_name, short_description, long_description, version, loader_script, 
		contact_email, contact_first_name, contact_last_name, notes, date_created,
		client_types.description,NeedsScheduling,NeedsESS,IsReentrant,Documentation_URL
	from lab_clients, client_types
	where client_id = @labClientID and lab_clients.client_type_id = client_types.client_type_id

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Client_RetrieveByGuid
	@clientGuid varchar(50)
AS
	select client_id, client_guid, lab_client_name, short_description, long_description, version, loader_script, 
		contact_email, contact_first_name, contact_last_name, notes, date_created,
		client_types.description,NeedsScheduling,NeedsESS,IsReentrant, Documentation_URL
	from lab_clients, client_types
	where client_guid = @clientGuid and lab_clients.client_type_id = client_types.client_type_id

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveLabClientID    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Client_RetrieveID
@guid varchar (50)
AS
	select client_id
	from lab_clients where client_guid = @guid

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE Client_RetrieveLoaderScript
	@id int
AS
	SELECT loader_script from lab_clients where client_ID = @id
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

CREATE PROCEDURE Client_RetrieveTags
AS
	select client_id, Lab_Client_Name
	from lab_clients where client_ID > 0

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




/****** Object:  Stored Procedure dbo.ModifyLabClient    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Client_Update
	@labClientID int,
	@clientGuid varchar (50),
	@labClientName nvarchar(256),
	@shortDescription nvarchar(256),
	@longDescription ntext,
	@version nvarchar(50),
	@loaderScript nvarchar(2000),
	@clientType varchar (128),
	@docURL nvarchar (512),
	@email nvarchar(256),
	@firstName nvarchar(128),
	@lastName nvarchar(128),
	@notes nvarchar(2048),
	@needsScheduling bit,
	@needsESS bit,
	@isReentrant bit
AS
		DECLARE @clientTypeID INT
		
		SELECT @clientTypeID = (SELECT client_type_id FROM client_types 
							WHERE upper(description) = upper(@clientType))
							
		UPDATE Lab_Clients
		SET lab_client_name = @labClientName, short_description=@shortDescription, 
			long_description=@longDescription, Client_Guid=@clientGuid, version = @version, 
			loader_script = @loaderScript, client_type_id=@clientTypeID,
			Documentation_URL=@docURL,contact_email = @email, 
			contact_first_name=@firstName, contact_last_name=@lastName, notes=@notes,
			NeedsScheduling=@needsScheduling,NeedsESS=@needsESS,IsReentrant= @isReentrant
		WHERE client_ID = @labClientID
	select @@rowcount

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE Client_UpdateLoaderScript
	@id int,
	@script nvarchar(2000)
AS
	UPDATE loader_script set loader_script = @script  where client_ID = @id
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

/****** Object:  Stored Procedure dbo.DeleteExperimentCoupon    Script Date: 12/12/2006 P******/

CREATE PROCEDURE ExperimentCouponID_Retrieve
	@experimentID BigInt
	
AS
	select Coupon_ID from ExperimentCoupon
	where Experiment_ID = @experimentID;


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.DeleteExperimentCoupon    Script Date: 12/12/2006 P******/

CREATE PROCEDURE ExperimentCoupon_Delete
	@experimentID BigInt,
	@couponID BigInt
AS
	delete from ExperimentCoupon
	where Experiment_ID = @experimentID and Coupon_ID = @couponID;
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


/****** Object:  Stored Procedure dbo.InsertExperimentCoupon    Script Date: 12/12/2006 P******/

CREATE PROCEDURE ExperimentCoupon_Insert
	@experimentID BigInt,
	@couponID BigInt
AS
	insert into ExperimentCoupon (Experiment_ID, Coupon_ID) 
	values ( @experimentID,@couponID );


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.DeleteExperimentCoupon    Script Date: 12/12/2006 P******/

CREATE PROCEDURE ExperimentCoupon_Retrieve
	@experimentID BigInt
	
AS
declare @couponId bigint
	select @couponId = Coupon_ID from ExperimentCoupon
	where Experiment_ID = @experimentID;
	if @couponId > 0
	begin
	select cancelled, coupon_ID, passkey from IssuedCoupon where coupon_ID = @couponId
	end
else
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


/****** Object:  Stored Procedure dbo.RetrieveGroupExperimentIDs    Script Date: 5/18/2005 4:17:56 PM ******/
CREATE PROCEDURE ExperimentIDs_RetrieveByGroup
	@groupID int
AS
	select Experiment_ID
	from Experiments
	where Group_ID = @groupID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE ExperimentStatus_Update
	@experimentID bigint,
	@status int,
	@closeTime DateTime,
	@recordCount int
AS

update experiments set status=@status, CloseTime=@closeTime,Record_count=@recordCount
where Experiment_ID=@experimentID

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



CREATE PROCEDURE ExperimentStatus_UpdateCode
	@experimentID bigint,
	@status int
AS

update experiments set status=@status
where Experiment_ID=@experimentID
select @@rowcount


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveExperimentSummary    Script Date: 02/16/2007 ******/

CREATE  PROCEDURE ExperimentSummary_Retrieve
	@experimentID BigInt
AS
declare @essID int
Select @essID= ess_id from experiments where experiment_ID = @experimentId
if @essID > 0

	select u.user_Name,g.group_Name,pa.Agent_Guid,pa.Agent_Name,
	 c.Lab_Client_Name,c.version, status, pa2.Agent_Guid, scheduledStart, 
	duration, creationTime, closeTime, annotation, record_count
	from Experiments ei,ProcessAgent pa, ProcessAgent pa2, Groups g, Lab_Clients c, Users u
	where ei.Experiment_ID = @experimentID and ei.agent_ID = pa.Agent_ID 
	and ei.ess_ID = pa2.Agent_ID and ei.user_ID= u.user_id 
	and ei.group_id = g.group_id and ei.client_ID= c.Client_ID
else
select u.user_Name,g.group_Name,pa.Agent_Guid,pa.Agent_Name,
	 c.Lab_Client_Name,c.version, status, null, scheduledStart, 
	duration, creationTime, closeTime, annotation, record_count
	from Experiments ei,ProcessAgent pa, ProcessAgent pa2, Groups g, Lab_Clients c, Users u
	where ei.Experiment_ID = @experimentID and ei.agent_ID = pa.Agent_ID 
	and ei.user_ID= u.user_id 
	and ei.group_id = g.group_id and ei.client_ID= c.Client_ID


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.CloseExperiment    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Experiment_Close
	@experimentID BigInt,
	@status int
AS
BEGIN TRANSACTION
	
	 update experiments set closeTime = getUtcdate(), status = @status where experiment_id = @experimentID
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

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.CreateExperiment    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Experiment_Create

	@status int, 
	@user int,
	@group int,
	@ls int,
	@client int,
	@ess int,
	@start datetime,
	@duration bigint
AS
	insert into Experiments (status, User_ID, Group_ID, Agent_ID,
		Client_ID, ESS_ID, ScheduledStart, duration, CreationTime)
	values (@status, @user, @group, @ls, @client, @ess, @start, @duration, getUtcdate())

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




/****** Object:  Stored Procedure dbo.DeleteExperiment    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Experiment_Delete
	@experimentID BigInt
AS
	delete from Experiments
	where Experiment_ID = @experimentID;

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



/****** Object:  Stored Procedure dbo.RetrieveExperiment    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Experiment_Retrieve
	@experimentID BigInt
AS
	select user_id, group_id, agent_ID, client_ID,status, ess_ID, scheduledStart, duration,creationTime, closeTime, annotation, record_count
	from Experiments 
	where Experiment_ID = @experimentID 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


create procedure Experiment_RetrieveActiveIDs
@userId int,
@groupId int,
@serverId int,
@clientId int
AS
 
select experiment_ID from experiments
where user_ID = @userId and Group_ID = @groupId and agent_ID = @serverId and Client_ID = @clientID
AND DATEADD(second,duration,scheduledStart) > GETUTCDATE()

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


CREATE  PROCEDURE Experiment_RetrieveAdminInfo
@experimentID bigint

AS

select experiment_ID,user_ID,group_ID,agent_ID,client_ID,ess_ID,
status,record_Count,duration,scheduledStart,creationTime,closeTime,annotation
from Experiments where Experiment_ID = @experimentID
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE  PROCEDURE Experiment_RetrieveAdminInfos
@ids varchar(8000)

AS
BEGIN

select experiment_ID,user_ID,group_ID,agent_ID,client_ID,ess_ID,
status,record_Count,duration,scheduledStart,creationTime,closeTime,annotation
from Experiments where Experiment_ID in (SELECT * FROM [dbo].[toLongArray](@ids))
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO






/****** Object:  Stored Procedure dbo.RetrieveExperimentAnnotation    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Experiment_RetrieveAnnotation
	@experimentID BigInt
AS
	select annotation
	from Experiments
	where Experiment_ID = @experimentID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE Experiment_RetrieveEssInfo
@experimentID bigint
AS
declare @essid int
select @essid=ess_id from experiments where experiment_ID = @experimentID
if @essid is Null or @essid <= 0
BEGIN
	return 0
END
ELSE
BEGIN
select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID,
Codebase_URL, WebService_URL,  Domain_Guid, issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where ProcessAgent.Issuer_Guid != Null AND  coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID),
retired
from ProcessAgent
where Agent_ID = @essID
return 0
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveExperimentGroup    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Experiment_RetrieveGroup
	@experimentID BigInt
AS
	select group_id
	from Experiments
	where Experiment_ID = @experimentID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



/****** Object:  Stored Procedure dbo.RetrieveExperimentOwner    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Experiment_RetrieveOwner
	@experimentID BigInt
AS
	select user_id
	from Experiments
	where Experiment_ID = @experimentID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveExperimentRawData    Script Date: 11/27/2007 ******/

CREATE PROCEDURE Experiment_RetrieveRawData
	@experimentID BigInt
AS
	select user_id, Group_ID,Agent_ID,Client_ID,ESS_ID,status
	from Experiments
	where Experiment_ID = @experimentID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE  PROCEDURE Experiment_RetrieveStatusCode
@experimentID bigint

AS
BEGIN

select status
from Experiments where Experiment_ID = @experimentID
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.SaveExperimentAnnotation    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Experiment_UpdateAnnotation
	@experimentID BigInt,
	@annotation ntext 
AS
	update Experiments
	set Annotation=@annotation
	where Experiment_ID = @experimentID;
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


/****** Object:  Stored Procedure dbo.ModifyExperimentOwner    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Experiment_UpdateOwner
	@experimentID BigInt,
	@newUserID int
AS
	update experiments
	set user_Id = @newUserID
	where experiment_ID = @experimentID
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







/****** Object:  Stored Procedure dbo.DeleteGrant    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Grant_Delete
	@grantID int
AS
	delete from Grants
	where Grant_ID = @grantID;
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



CREATE PROCEDURE Grant_Insert
	@agentID int,
	@functionName varchar(128),
	@qualifierID int
AS
	DECLARE @functionID int
	
	select @functionID = (select function_id from functions where 
								upper(function_name)=upper(@functionName))
	insert into grants(agent_ID, function_ID,qualifier_ID)
	values (@agentID,@functionID,@qualifierID)

	select ident_current('grants')

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




/****** Object:  Stored Procedure dbo.RetrieveGrantsTable    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Grants_RetrieveTable
AS
	select agent_id, f.function_name, qualifier_id, grant_id
	from grants g, functions f
	where g.function_id =f.function_id
return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE Group_RetrieveIDs
AS
	select group_id
	from groups

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.AddMemberToGroup    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Group_AddMember
	@groupID int,
	@memberID int
AS
BEGIN TRANSACTION
	DECLARE @isGroup bit;
	DECLARE @orphanedGroupID int;
	DECLARE @newUserGroupID int;
	DECLARE @rootGroupID int

	select @isGroup = (select is_group from agents  where (agent_id=@memberID) );
	select @rootGroupID = (select group_id from groups where group_name = 'ROOT')

	begin
		if (@isGroup = 0)
		begin
			if (@groupID!=@rootGroupID) -- not trying to transfer to root
			begin 
				-- if agent is member of orphaned user group then delete them from it.
				select @orphanedGroupID = (select group_id from groups where group_name = 'OrphanedUserGroup');

				delete from agent_hierarchy 
				where agent_id= @memberID and parent_group_id = @orphanedGroupID;
				if (@@error > 0)
					goto on_error

				-- add to other group
				insert into agent_hierarchy (agent_id, parent_group_id)
				values (@memberID, @groupID);
				if (@@error > 0)
					goto on_error

			end
		end
		else 	/*If group then set qualifiier parents appropriately */
		begin
			DECLARE @groupQualifierID int;
			DECLARE @parentGroupQualifierID int;
			DECLARE @ECQualifierID int;
			DECLARE @parentECQualifierID int;

			DECLARE @rootQualifierID int;
			select @rootQualifierID = (select qualifier_id from Qualifiers where qualifier_name = 'ROOT');


			-- set group qualifiers
			select @groupQualifierID = (select qualifier_id from Qualifiers where Qualifier_reference_ID=@memberID 
							and Qualifier_Type_ID = (select Qualifier_Type_ID from Qualifier_Types where description='Group') );

			-- if added to root group then set parent qualifier to root	
			if (@groupID = @rootGroupID)
				select @parentGroupQualifierID = @rootQualifierID
			else
				select @parentGroupQualifierID = (select qualifier_id from Qualifiers where Qualifier_reference_ID=@groupID 
							and Qualifier_Type_ID = (select Qualifier_Type_ID from Qualifier_Types where description='Group') );

			insert into qualifier_hierarchy (qualifier_ID, parent_qualifier_ID)
			values (@groupQualifierID, @parentGroupQualifierID);
			if (@@error > 0)
				goto on_error
			
			-- set experiment qualifiers
			select @ECQualifierID = (select qualifier_id from Qualifiers where Qualifier_reference_ID=@memberID 
							and Qualifier_Type_ID = (select Qualifier_Type_ID from Qualifier_Types where description='Experiment Collection') );
						
			-- if added to root group then set experiment collection qualifier parent to root	
			if (@groupID = @rootGroupID)
				select @parentECQualifierID = @rootQualifierID
			else
				select @parentECQualifierID = (select qualifier_id from Qualifiers where Qualifier_reference_ID=@groupID 
					and Qualifier_Type_ID = (select Qualifier_Type_ID from Qualifier_Types where description='Experiment Collection') );
			
			insert into qualifier_hierarchy (qualifier_ID, parent_qualifier_ID)
			values (@ECQualifierID, @parentECQualifierID);
			if (@@error > 0)
				goto on_error
			
			insert into  agent_hierarchy (parent_group_ID, agent_ID)
			values (@groupID, @memberID);
			if (@@error > 0)
				goto on_error
		-- This is an insert and  NOT an update because agents can belong to multiple groups
		end
	end

COMMIT TRANSACTION	
return
	on_error: 
	ROLLBACK TRANSACTION

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.DeleteGroup    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Group_Delete
	@groupID int
AS
BEGIN TRANSACTION
	delete from Agents 
	where agent_id = @groupID
	if (@@error > 0)
		goto on_error

	delete from Agent_Hierarchy
	where Parent_Group_ID = @groupID;
	if (@@error > 0)
		goto on_error
-- AgentId is taken care of by referential integrity
	
	delete from Groups
	where Group_ID = @groupID;
	if (@@error > 0)
		goto on_error
		
	delete from Qualifiers
	where Qualifier_Reference_ID = @groupID and 
		Qualifier_Type_ID = (select Qualifier_Type_ID from Qualifier_Types where description='Group')
	if (@@error > 0)
		goto on_error
		
	delete from Qualifiers
	where Qualifier_Reference_ID = @groupID and 
		Qualifier_Type_ID = (select Qualifier_Type_ID from Qualifier_Types where description='Experiment Collection')
		
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

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.DeleteMemberFromGroup    Script Date: 5/18/2005 4:17:55 PM ******/
CREATE PROCEDURE Group_DeleteMember
	@groupID int,
	@memberID int
AS
BEGIN TRANSACTION
	DECLARE @isGroup bit;
	select @isGroup = (select is_group from agents
				 where (agent_id=@memberID) )

	DECLARE @rootGroupID int
	select @rootGroupID = (select group_id from groups where group_name = 'ROOT')
	
	if (@isGroup = 0) /* if user */
	begin
		/* Get Orphaned user group ID */
		DECLARE @orphanedGroupID int;
		select @orphanedGroupID = (select group_id from groups where group_name = 'OrphanedUserGroup');
		
		/* If user only belongs to the Orphaned Users Group - delete from system */
 		if( (@groupID=@orphanedGroupID) 
 			and  ((select count( agent_id) from agent_hierarchy where agent_id=@memberID and parent_group_id=@groupID)  = 1)
 			and  ((select count( agent_id) from agent_hierarchy where agent_id=@memberID)  = 1))

		begin
			delete from users where user_id=@memberID;
			if (@@error > 0)
				goto on_error

			delete from agents where agent_id=@memberID;
			if (@@error > 0)
				goto on_error

		end
		else
			/*check parents of member*/
			if ((select count( parent_group_id) from agent_hierarchy where agent_id=@memberID)>1)
				/* multiple parents*/
				delete from agent_hierarchy where agent_id=@memberID and parent_group_id=@groupID
			else
			begin
				/* if single parent */

				-- single parent cannot be root
				if (@groupID != @rootGroupID)
					update agent_hierarchy 
					set parent_group_ID = @orphanedGroupID
					where parent_group_ID = @groupID and  agent_ID = @memberID;
			end
	end
	else
	if (@isGroup = 1) /* Group */
	begin
		DECLARE @rootQualifierID int;
		select @rootQualifierID = (select qualifier_id from Qualifiers where qualifier_name = 'ROOT');

		/* get group qualifier */
		DECLARE @qualifierID int;
		select @qualifierID = (select qualifier_id from Qualifiers where qualifier_reference_id=@memberID 
					and qualifier_type_id  = (select Qualifier_Type_ID from Qualifier_Types where description='Group'))
		
		
		/* get experiment collection qualifier */
		DECLARE @experimentCollectionQualifierID int;
		select @experimentCollectionqualifierID = (select qualifier_id from Qualifiers where qualifier_reference_id=@memberID 
					and qualifier_type_id  = (select Qualifier_Type_ID from Qualifier_Types where description='Experiment Collection'))

		DECLARE @parentQualifierID int;
		DECLARE @parentECQualifierID int;

		-- if being removed from root
		if (@groupID = @rootGroupID)
		begin
			select @parentQualifierID = @rootQualifierID;
			select @parentECQualifierID = @rootQualifierID;
		end
		else
		begin
			/* get parent qualifier */
			select @parentQualifierID = (select qualifier_id from Qualifiers where qualifier_reference_id=@groupID 
				and qualifier_type_id  = (select Qualifier_Type_ID from Qualifier_Types where description='Group'))

			/* get parent experiment collection qualifier */
			select @parentECQualifierID = (select qualifier_id from Qualifiers where qualifier_reference_id=@groupID 
				and qualifier_type_id  = (select Qualifier_Type_ID from Qualifier_Types where description='Experiment Collection'))
		end
		

		/*check parents of agent*/
		if ((select count( parent_group_id) from agent_hierarchy where agent_id=@memberID)>1)
		
		/* multiple parents - delete relationships*/
		begin
			delete from agent_hierarchy where agent_id=@memberID and parent_group_id=@groupID
			if (@@error > 0)
				goto on_error
				
			delete from qualifier_hierarchy where qualifier_id = @qualifierID and parent_qualifier_id=@parentQualifierID
			if (@@error > 0)
				goto on_error
				
			delete from qualifier_hierarchy where qualifier_id = @experimentCollectionQualifierID and parent_qualifier_id=@parentECQualifierID
			if (@@error > 0)
				goto on_error
		end
		else
		/* single parent - move all to ROOT */
		-- single parent cannot be root
		if (@groupID != @rootGroupID)
		begin
			update agent_hierarchy 
			set parent_group_ID = @rootGroupID
			where parent_group_ID = @groupID and  agent_ID = @memberID;
			if (@@error > 0)
				goto on_error
				
			update qualifier_hierarchy
			set parent_qualifier_ID = @rootQualifierID
			where parent_qualifier_ID = @parentQualifierID and qualifier_ID=@qualifierID
			if (@@error > 0)
				goto on_error
				
			update qualifier_hierarchy
			set parent_qualifier_ID = @rootQualifierID
			where parent_qualifier_ID = @parentECQualifierID and qualifier_ID=@experimentCollectionQualifierID
			if (@@error > 0)
				goto on_error
		end
	end
COMMIT TRANSACTION	
return
	on_error: 
	ROLLBACK TRANSACTION

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Group_Insert
	@groupName nvarchar(256),
	@description nvarchar(2048),
	@email nvarchar(256),
	@parentGroupID int, 
	@groupType varchar(256),
	@associatedGroupID int
AS
BEGIN TRANSACTION
	DECLARE @agentID int
	DECLARE @groupTypeID int

	select @groupTypeID = (select  group_type_id from group_types where description=@groupType);	
	
	insert into agents (agent_name, is_group)
	values (@groupName, 1)
	if (@@error > 0)
		goto on_error
	select @agentID=(select ident_current('Agents'))

	/* Assume that the parent group id here is a legal value. 
	Any corrections for -1 will be done in API code */
	insert into  agent_hierarchy (parent_group_ID, agent_ID)
	values (@parentGroupID, @agentID)
	if (@@error > 0)
		goto on_error
	
	insert into groups (group_id, group_name,description,email, group_type_id, associated_group_id)
	values (@agentID, @groupname, @description,@email, @groupTypeID, @associatedGroupID)
	if (@@error > 0)
		goto on_error

COMMIT TRANSACTION
	select @agentID
return
	on_error: 
	ROLLBACK TRANSACTION

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveGroup    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Group_Retrieve
	@groupID int
AS
	select  group_name, g.description AS description, email, gt.description AS group_type, date_created
	from groups g, group_types gt
	where group_ID= @groupID and g.group_type_id=gt.group_type_id

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveGroupAdminGroupID    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Group_RetrieveAdminGroupID
	@groupID int
AS
	DECLARE @adminGroupType int
	
	select @adminGroupType = (Select group_type_id from group_types where description
								 = 'Course Staff Group');
	select group_ID from groups
	where associated_group_id = @groupID and group_type_id = @adminGroupType

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveAdminGroupIDs    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Group_RetrieveAdminGroupIDs
AS
	DECLARE @adminGroupType int
	
	select @adminGroupType = (Select group_type_id from group_types where description
								 = 'Service Administration Group');
	select group_ID from groups
	where group_type_id = @adminGroupType

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveAssociatedGroupID    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Group_RetrieveAssociatedGroupID
	@groupID int
AS
	select associated_group_ID
	from groups
	where group_id = @groupID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveGroupID    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Group_RetrieveID
	@groupName nvarchar(256)
AS
	select group_ID
	from groups
	where group_name = @groupName

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveGroupMembers    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Group_RetrieveMembers
	@groupID int
AS
	select ah.agent_ID, ag.is_group, ag.agent_name
	from   agent_hierarchy ah, agents ag
	where ah.parent_group_ID = @groupID and ah.agent_id=ag.agent_id
	order by agent_name

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Group_RetrieveName
	@groupID int
AS
	select group_Name
	from groups
	where group_ID = @groupID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveGroupRequestGroupID    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Group_RetrieveRequestGroupID
	@groupID int
AS
	DECLARE @requestGroupType int
	
	select @requestGroupType = (Select group_type_id from group_types where description
								 = 'Request Group');
	select group_ID from groups
	where associated_group_id = @groupID and group_type_id = @requestGroupType

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



/****** Object:  Stored Procedure dbo.ModifyGroup    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE Group_Update
	@groupID int,
	@groupName nvarchar(256),
	@description nvarchar(4000),
	@email nvarchar(256)
AS
BEGIN TRANSACTION
	DECLARE @oldGroupName nvarchar(256)
	DECLARE @qualifierTypeID1 int
	DECLARE @qualifierTypeID2 int
	
	begin
		select @oldGroupName = (select group_name from Groups where group_id=@groupID)
		if (@oldGroupName <> @groupName)
		begin
			update agents
			set agent_name = @groupName
			where agent_name = @oldGroupName;
			if (@@error > 0)
				goto on_error
				
			select @qualifierTypeID1 = (select qualifier_type_id from qualifier_types	
										where description = 'Agent')
			select @qualifierTypeID2 = (select qualifier_type_id from qualifier_types	
								where description = 'Group')
								
			update qualifiers
			set qualifier_name = @groupName
			where qualifier_reference_id = @groupID and qualifier_name = @oldGroupName 
					and (qualifier_type_id = @qualifierTypeID1 or qualifier_type_id = @qualifierTypeID2)
			if (@@error > 0)
				goto on_error
		end
	
		update groups 
		set group_name = @groupName, description = @description, email=@email
		where group_id = @groupID
		if (@@error > 0)
			goto on_error
	end
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

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE LabServerClient_ClientIDs
@serverID int
AS

select Client_ID FROM Lab_CLients
where client_ID in (select client_ID from Lab_Server_To_Client_Map where agent_ID = @serverID)
GO

CREATE PROCEDURE LabServerClient_Count
@groupID int,
@serverID int
AS
if @groupID > 0
select count(client_id) from Lab_Server_To_Client_map where agent_id = @serverID and client_ID in
(select qualifier_reference_ID from Qualifiers where qualifier_id in
(select qualifier_ID from grants where agent_id = @groupID 
and function_id = (select function_ID from functions where Function_Name = 'useLabClient')))
ELSE
select count(client_id) from Lab_Server_To_Client_map where agent_id = @serverID
return
GO


CREATE PROCEDURE LabServerClient_CountScheduledClients
@serverID int
AS
select COUNT(Client_id) FROM Lab_CLients
where client_ID in (select client_ID from Lab_Server_To_Client_Map where agent_ID = @serverID)
AND NeedsScheduling = 1

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


/****** Object:  Stored Procedure dbo.DeleteLabServerClient    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE LabServerClient_Delete
	@labClientID int,
	@labServerID int
AS
	delete from lab_server_to_client_map
	where client_id = @labClientID and Agent_ID = @labServerID
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
CREATE PROCEDURE LabServerClient_DeleteAll
	@labClientID int
AS
	delete from lab_server_to_client_map
	where client_id = @labClientID
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
/****** Object:  Stored Procedure dbo.AddLabServerClient    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE LabServerClient_Insert
	@labClientID int,
	@labServerID int,
	@displayOrder int
AS
	insert into lab_server_to_client_map (client_id,agent_id, display_order)
	values (@labClientID, @labServerID, @displayOrder)
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


/****** Object:  Stored Procedure dbo.RetrieveClientServerIDs    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE LabServerClient_RetrieveServerIDs
	@labClientID int
AS
	select agent_id
	from lab_server_to_client_map
	where client_id=@labClientID
	order by display_order

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



/****** Object:  Stored Procedure dbo.RetrieveNativePassword    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE NativePassword_Retrieve
	@userID int
AS
	select password
	from users
	where user_ID =@userID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO






/****** Object:  Stored Procedure dbo.SaveNativePassword    Script Date: 5/18/2005 4:17:57 PM ******/

CREATE PROCEDURE NativePassword_Update
	@userID int,
	@password nvarchar(256)
AS
BEGIN TRANSACTION
	update users
	set password = @password
	where user_id =@userID
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

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



/****** Object:  Stored Procedure dbo.CreateNativePrincipal    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE NativePrincipal_Create
	@userName nvarchar(256)
AS
	DECLARE @authTypeID int
	DECLARE @userID int
	
	select @authTypeID=(select auth_type_id from authentication_types where
					upper(description) = 'NATIVE')	
	select @userID = (select user_ID from users where user_name=@userName)
	
	-- since no principal string is specified in authen api, inserting the username as principal string
	insert into Principals (user_ID, auth_type_id, principal_string)
	values (@userID,@authTypeID, @userName)
	
	select @userID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



/****** Object:  Stored Procedure dbo.DeleteNativePrincipal    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE NativePrincipal_Delete
	@userID int
AS
	DECLARE @NativeTypeID int;
	
	select @NativeTypeID = (select auth_type_id from Authentication_Types where upper(description)='NATIVE')
	
	delete from Principals
	where user_ID = @userID and auth_type_id=@NativeTypeID;
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


/****** Object:  Stored Procedure dbo.RetrieveNativePrincipals    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE NativePrincipals_Retrieve
AS
	select user_ID
	from   principals 
	where auth_type_id=(select auth_type_id from authentication_types where
					upper(description) = 'NATIVE')

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE procedure ProcesAgent_RetrieveAdminServiceTags
@groupId int

as

select agent_id, Agent_Name from ProcessAgent
where agent_id in
(select distinct mappingvalue from ResourceMappingValues where MappingValue_Type = 1 AND
mapping_ID in (select Qualifier_reference_id  from Qualifiers where Qualifier_Type_id = 11 
and qualifier_id in
( select qualifier_id from grants where agent_id = @groupId and Function_id between 13 and 19 ) ) )


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


Create Procedure ProcessAgent_RetrieveAdminGrants
@agentId int,
@groupID int

as

select g.Agent_id, f.function_Name, g.grant_ID, g.qualifier_ID

FROM Grants g, Functions f

 

where g.Agent_ID = @groupID AND g.qualifier_id 
in ( select Qualifier_id
from Qualifiers where qualifier_Reference_ID = @agentID 
AND qualifier_id 
in ( select Qualifier_id from grants 
where agent_id = @groupID AND function_ID between 13 and 19))
AND g.Function_id = f.Function_id

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


Create Procedure ProcessAgent_RetrieveAdminTags
@groupId int

as

select pa.Agent_id, pa.Agent_Name 

FROM ProcessAgent pa 

where pa.Agent_id 
in ( select Qualifier_Reference_id
from Qualifiers where qualifier_id 
in ( select Qualifier_id from grants 
where agent_id = @groupID AND function_ID between 13 and 19))

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE QualifierHierarchy_Delete
	@parentQualifierID int,
	@qualifierID int
AS
	delete from  qualifier_hierarchy 
	where parent_qualifier_ID = @parentQualifierID and qualifier_ID = @qualifierID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE QualifierHierarchy_Insert
	@parentQualifierID int,
	@qualifierID int
AS
	
	insert into  qualifier_hierarchy (parent_qualifier_ID, qualifier_ID)
	values (@parentQualifierID, @qualifierID)
-- This is an insert and  NOT an update because qualifiers can have multiple parents

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveQualifierHierarchyTable    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE QualifierHierarchy_RetrieveTable
AS
	select qualifier_id, parent_qualifier_id
	from qualifier_hierarchy
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


/****** Object:  Stored Procedure dbo.DeleteQualifier    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Qualifier_Delete
	@qualifierID int
AS
	delete from Qualifiers
	where Qualifier_ID = @qualifierID;

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.AddQualifier    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE Qualifier_Insert
	@qualifierTypeID int,
	@qualifierReferenceID int,
	@qualifierName nvarchar (512)
AS
	/*DECLARE @qualifierTypeID int
	select @qualifierTypeID = (select qualifier_type_id from qualifier_Types where 
				upper(description) = upper(@qualifierType))*/
	insert into qualifiers(qualifier_Type_id, qualifier_reference_id, qualifier_name)
	values (@qualifierTypeID,@qualifierReferenceID, @qualifierName)
	
	select ident_current('qualifiers')

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE Qualifier_UpdateName
@qualifierTypeID int,
@refID int,
@newName nvarchar(512)

AS

UPDATE Qualifiers SET Qualifier_name = @newName
where Qualifier_Type_ID =@qualifierTypeID and Qualifier_Reference_ID = @refID
select @@rowcount


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Qualifiers_RetrieveTable
AS
	select qualifier_id, qualifier_Reference_ID, qualifier_Type_ID, qualifier_name, date_created
	from qualifiers
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


/* Registration Table Procedures */


CREATE PROCEDURE Registration_Insert
@couponId int,
@couponGuid varchar (50),
@registerGuid varchar (50),
@sourceGuid varchar (50),
@status int,
@descriptor NTEXT,
@email nvarchar (256)

AS

 insert into Registration (couponID, couponGuid,registerGuid,sourceGuid,status,descriptor,email)
	values (@couponId,@couponGuid, @registerGuid,@sourceGuid,@status,@descriptor,@email)
select ident_current('Register')


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Registration_Retrieve
@id int
AS
select record_id, couponID, couponGuid, registerGuid,sourceGuid,status,createTime,lastModTime,descriptor,email
from Registration
where record_id = @id


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Registration_RetrieveByGuid
@registerGuid varchar (50)
AS
select record_id,couponID,couponGuid, registerGuid,sourceGuid,status,createTime,lastModTime,descriptor,email
from Registration
where registerGuid = @registerGuid


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Registration_UpdateStatus
@id int,
@status int
AS

update registration set status=@status,lastModTime= getUtcdate()
where record_id=@id
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

CREATE PROCEDURE Registrations_Retrieve
AS
select record_id, couponId,couponGuid, registerGuid,sourceGuid,status,createTime,lastModTime,descriptor,email
from Registration
order by registerGuid


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Registrations_RetrieveByRange
@low int,
@high int
AS
select record_id,couponId,couponGuid,registerGuid,sourceGuid,status,createTime,lastModTime,descriptor,email
from Registration
where status BETWEEN @low and @high
order by registerGuid


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Registrations_RetrieveByStatus
@status int
AS
select record_id,couponID,couponGuid, registerGuid,sourceGuid,status,createTime,lastModTime,descriptor,email
from Registration
where status = @status
order by registerGuid


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




Create Procedure ResourceMapId_RetrieveByKeyValue
@keyId int,
@keyType int,
@valueId int,
@valueType int

as 

select mapping_id from ResourceMappingValues
where mapping_id 
in ( select mapping_id from ResourceMappingKeys
	where MappingKey = @keyId and MappingKey_Type = @keyType )
AND MappingValue = @valueId AND MappingValue_Type = @valueType


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



Create PROCEDURE ResourceMapIds_RetrieveByValue
@type nvarchar(256),
@id int

AS

select distinct Mapping_ID from ResourceMappingVAlues
where mappingValue =@id and MappingValue_Type = (select type_id FROM ResourceMappingTypes where Type_Name =@type)


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE ResourceMapKey_Insert
	@mappingKey_Type int,
	@mappingKey int
 AS
declare @idmap int

	
		insert into ResourceMappingKeys (MappingKey_Type, MappingKey) values (@mappingKey_Type, @mappingKey)
		select ident_current('ResourceMappingKeys')
	

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE ResourceMapResourceType_Insert
	@resourceType_Value nvarchar(256)
 AS
	if(select count(*) from ResourceMappingResourceTypes where ResourceType_Value = @resourceType_Value) = 0
		begin
		insert into ResourceMappingResourceTypes (ResourceType_Value) values (@resourceType_Value)
		select ident_current('ResourceMappingResourceTypes')
		end
	else
		select ID from ResourceMappingResourceTypes where ResourceType_Value = @resourceType_Value

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


Create PROCEDURE ResourceMapStringTag_Retrieve
@mapid int

AS

if (select count(*) from ResourceMappingValues where mapping_ID = @mapid and MappingValue_Type = 4) > 0
	select mv.mappingValue, rs.String_Value
	from ResourceMappingValues mv, ResourceMappingStrings rs
	where mapping_ID = @mapid and MappingValue_Type = 4 
	and mv.MappingValue = rs.ID
else
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


CREATE PROCEDURE ResourceMapString_Insert
	@string_Value nvarchar(2048)
 AS

	insert into ResourceMappingStrings (String_Value) values (@string_Value)
	select ident_current('ResourceMappingStrings')


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE ResourceMapString_RetrieveByID
	@ID int
 AS
	select String_Value from ResourceMappingStrings where ID = @ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


Create  PROCEDURE ResourceMapString_Update
	@id int,
	@string nvarchar(2048)
 AS

	update ResourceMappingStrings set String_Value= @string where ID = @id
	select mapping_ID from ResourceMappingValues where MappingValue = @id and MappingValue_Type = 4


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


Create Procedure ResourceMapTypeNames_Retrieve
@type int,
@target int

as 

select mapping_id, ResourceType_Value
from resourceMappingValues mv, ResourceMappingResourceTypes rt
where mapping_ID 
in ( select mapping_id from ResourceMappingKeys where MappingKey_type = @type and MappingKey = @target)
and MappingValue_Type = 7 
and mv.MappingValue = rt.ID


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

Create Procedure ResourceMapTypeString_Retrieve
@resourceType nvarchar( 256),
@type int,
@target int

as 
declare @mapId int


select @mapId = mapping_id from resourceMappingValues 
where mapping_ID 
in ( select mapping_id from ResourceMappingKeys
where MappingKey_type = @type and MappingKey = @target)
and MappingValue_Type = 7 and MappingValue =(select id from ResourceMappingResourceTypes
where resourceType_value = @resourceType )

if @mapID > 0
select mappingvalue, string_value from resourceMappingValues, ResourceMappingStrings
where mappingValue_Type = 4 and mapping_ID = @mapId and ID = mappingvalue
else
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


Create Procedure ResourceMapTypeStrings_Retrieve
@type int,
@target int

as 

select mapping_id, ResourceType_Value
from resourceMappingValues mv, ResourceMappingResourceTypes rt
where mapping_ID 
in ( select mapping_id from ResourceMappingKeys where MappingKey_type = @type and MappingKey = @target)
and MappingValue_Type = 7 
and mv.MappingValue = rt.ID;


select mapping_id,   mv.MappingValue, String_Value
from resourceMappingValues mv, ResourceMappingStrings rs
where mapping_ID 
in ( select mapping_id from ResourceMappingKeys where MappingKey_type = @type and MappingKey = @target)
and MappingValue_Type = 4 
and mv.MappingValue = rs.ID


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE ResourceMapType_RetrieveByID
	@ID int
 AS
	select ResourceType_Value from ResourceMappingResourceTypes where ID = @ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE ResourceMapValue_Insert
	@mapping_ID int,
	@mappingValue_Type int,
	@mappingValue int
 AS
	insert into ResourceMappingValues (Mapping_ID, MappingValue, MappingValue_Type) values (@mapping_ID, @mappingValue, @mappingValue_Type)
	select ident_current('ResourceMappingValues')

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE ResourceMap_RetrieveByID
	@mappingID int
AS 
	DECLARE @mappingKey_Type int
	DECLARE @mappingKey int

	/* retrieve map key type and key */
	select @mappingKey_Type  = ( select MappingKey_Type FROM ResourceMappingKeys where (Mapping_ID = @mappingID))
	select @mappingKey  = ( select MappingKey FROM ResourceMappingKeys where (Mapping_ID = @mappingID))

	/* retrieve corresponding map value type and value */
	select @mappingKey_Type, @mappingKey union all select MappingValue_Type, MappingValue FROM ResourceMappingValues where (Mapping_ID = @mappingID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE ResourceMap_RetrieveIDsByKey
	@mappingKey_Type int,
	@mappingKey int
 AS
	select Mapping_ID from ResourceMappingKeys where (MappingKey_Type = @mappingKey_Type AND MappingKey = @mappingKey)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




/****** Object:  Stored Procedure dbo.DeleteREsourceMapping    Script Date: 10/4/2005 ******/

CREATE PROCEDURE ResourceMap_delete
	@mapping_ID int
AS
BEGIN TRANSACTION

	delete from ResourceMappingValues
	where Mapping_ID = @mapping_ID
	if (@@error > 0)
		goto on_error

	delete from ResourceMappingKeys
	where Mapping_ID = @mapping_ID
	if (@@error > 0)
		goto on_error

	delete from Qualifiers
	where qualifier_reference_ID = @mapping_ID and 
	qualifier_type_ID = (select qualifier_type_ID from qualifier_Types 
				where description = 'Resource Mapping');
	if (@@error > 0)
		goto on_error

COMMIT TRANSACTION	
return
	on_error: 
	ROLLBACK TRANSACTION

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE ResourceMapIDs_Retrieve
 AS
	select Mapping_ID from ResourceMappingKeys

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/* This procedure is probably not being used anywhere */
CREATE PROCEDURE ResultXMLExtensionSchemaURL_Update
 	@labserverID int,
	@URL varchar(512)
AS
/* hardcoded account ID*/
	update accounts
	set result_XML_Extension_schema_URL = @URL
	where account_ID = 2

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE SessionHistory_Retrieve
AS
select s.Session_ID,u.User_name, h.modify_time,g.Group_Name, c.Lab_Client_Name,s.Session_Start_Time,s.Session_End_Time,h.Session_key
from user_sessions s, session_history h, Users u, Groups g, Lab_Clients c
where s.session_id = h.session_id and s.user_ID = u.user_id and h.group_ID = g.Group_ID and h.CLient_ID = c.client_ID
order by s.session_id, h.modify_time


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



-- Only return user sessions that have not been closed
CREATE PROCEDURE SessionInfo_Retrieve
	@sessionID BigInt
AS 
	
	select s.user_id, s.effective_group_id, s.client_ID, u.user_Name, g.group_Name,s.TZ_Offset
	 from user_sessions s,users u, groups g where session_ID = @sessionID
	AND s.Session_End_Time = NULL
	AND s.user_id = u.user_id and s.effective_group_ID=g.group_ID 
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

CREATE PROCEDURE SystemMessage_DeleteByID
	@messageID int
AS
	delete from System_Messages
	where System_Message_ID = @messageID;
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

CREATE PROCEDURE SystemMessage_Insert
	@messageType varchar(256),
	@messageTitle nvarchar(256),
	@toBeDisplayed bit,
	@groupID int,
	@clientID int,
	@agentID int,
	@messageBody nvarchar(3000)

AS 
	DECLARE @messageTypeID int

	select @messageTypeID = (select message_type_id from message_types 
							where upper(description) = upper(@messageType))
	insert into System_Messages (message_type_id, to_be_displayed, agent_id, client_ID, group_id, 
		message_title, message_body, date_created,last_modified)
		values (@messageTypeID, @toBeDisplayed, @agentID, @clientID, @groupID, @messageTitle, @messageBody,
			getUtcdate(),getUtcdate())
	
	select ident_current('system_messages')
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


/****** Object:  Stored Procedure dbo.RetrieveSystemMessageByID    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE SystemMessage_RetrieveByID
	@messageID int
/*Retrieve by message ID*/
AS
	select  description, to_be_displayed, last_modified, agent_ID, client_ID, group_id, 
			message_title, message_body
	from system_messages sm, message_types mt
	where sm.system_message_ID= @messageID and to_be_displayed = 1
		and sm.message_type_id=mt.message_type_id 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE SystemMessage_RetrieveByIDForAdmin
	@messageID int
/*Retrieve by message ID for admin pages (all messages)*/
AS
	select  message_body, to_be_displayed, last_modified, client_ID, group_id, 
			agent_id,message_title, description
	from system_messages sm, message_types mt
	where sm.system_message_ID= @messageID and sm.message_type_id=mt.message_type_id

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE SystemMessage_Update
	@messageID	int,
	@messageType varchar(256),
	@messageTitle nvarchar(256),
	@toBeDisplayed bit,
	@groupID int,
	@agentID int,
	@clientID int,
	@messageBody nvarchar (3000) 
AS
	DECLARE @messageTypeID int

	select @messageTypeID = (select message_type_id from message_types 
							where upper(description) = upper(@messageType))
							
	update System_Messages
	set message_type_id = @messageTypeID, to_be_displayed=@toBeDisplayed, 
	group_ID = @groupID, agent_id=@agentID, client_ID = @clientID, message_title = @messageTitle,
	message_body = @messageBody, last_modified = getUtcdate()
	where system_message_ID = @messageID;
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


/****** Object:  Stored Procedure dbo.DeleteSystemMessages    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE SystemMessages_Delete
	@messageType varchar(256),
	@clientID int,
	@groupID int,
	@agentID int

/* Delete by message type & group/lab server */
AS
	DECLARE @messageTypeID int

	select @messageTypeID = (select message_type_id from message_types 
							where upper(description) = upper(@messageType))
							
	delete from System_Messages
	where message_type_id = @messageTypeID and group_ID = @groupID and agent_ID=@agentID and client_ID = @clientID;
select @@rowcount


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveSystemMessages    Script Date: 5/18/2005 4:17:56 PM ******/
CREATE PROCEDURE SystemMessages_Retrieve
/*Retrieves by message type and options */
	@messageType varchar(256),
	@agentID int,
	@clientID int,
	@groupID int
AS
	DECLARE @messageTypeID int
	
	select @messageTypeID = (select message_type_id from message_types 
						where upper(description) = upper(@messageType))
	IF @agentID >0 AND @clientID >0 AND @groupID > 0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID and to_be_displayed =1 
			and group_ID=@groupID and agent_ID=@agentID and client_ID =@clientID
	order by last_modified DESC
	END
	ELSE IF @agentID >0 AND @clientID >0 
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID and to_be_displayed =1 
			and agent_ID=@agentID and client_ID =@clientID
	order by last_modified DESC
	END
	ELSE IF @agentID >0 AND @groupID > 0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID and to_be_displayed =1 
			and group_ID=@groupID and agent_ID=@agentID
	order by last_modified DESC
	END
	ELSE IF @clientID >0 AND @groupID > 0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID and to_be_displayed =1 
			and group_ID=@groupID and client_ID =@clientID
	order by last_modified DESC
	END
	
	ELSE IF @agentID >0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID and to_be_displayed =1 
			and agent_ID=@agentID
	order by last_modified DESC
	END
	ELSE IF @clientID >0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID and to_be_displayed =1 
			and client_ID =@clientID
	order by last_modified DESC
	END
	ELSE IF @groupID > 0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID and to_be_displayed =1 
			and group_ID=@groupID
	order by last_modified DESC
	END
	ELSE
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID and to_be_displayed =1
	order by last_modified DESC 
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE SystemMessages_RetrieveByGroup
	@groupIds varchar(4000)
AS
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages
	where to_be_displayed =1 
		and group_ID in (select * from [dbo].[toIntArray](@groupIds))
order by last_modified DESC

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.RetrieveSystemMessagesForAdmin    Script Date: 5/18/2005 4:17:56 PM ******/

CREATE PROCEDURE SystemMessages_RetrieveForAdmin
/*Retrieves by message type and group */
	@messageType varchar(256),
	@clientID int,
	@groupID int,
	@agentID int
AS
	DECLARE @messageTypeID int
	
	select @messageTypeID = (select message_type_id from message_types 
						where upper(description) = upper(@messageType))
	IF @agentID >0 AND @clientID >0 AND @groupID > 0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID
			and group_ID=@groupID and agent_ID=@agentID and client_ID =@clientID
	order by last_modified DESC
	END
	ELSE IF @agentID >0 AND @clientID >0 
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID
			and agent_ID=@agentID and client_ID =@clientID
	order by last_modified DESC
	END
	ELSE IF @agentID >0 AND @groupID > 0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID 
			and group_ID=@groupID and agent_ID=@agentID
	order by last_modified DESC
	END
	ELSE IF @clientID >0 AND @groupID > 0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID 
			and group_ID=@groupID and client_ID =@clientID
	order by last_modified DESC
	END
	
	ELSE IF @agentID >0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID 
			and agent_ID=@agentID
	order by last_modified DESC
	END
	ELSE IF @clientID >0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID 
			and client_ID =@clientID
	order by last_modified DESC
	END
	ELSE IF @groupID > 0
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID 
			and group_ID=@groupID
	order by last_modified DESC
	END
	ELSE
	BEGIN
	select system_message_ID, message_body, to_be_displayed, last_modified, message_title
	from system_messages sm
	where sm.message_type_id=@messageTypeID
	order by last_modified DESC 
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE TicketCollection_Count
	@couponID BigInt
AS
	select count(ticket_ID) from IssuedTickets where coupon_ID = @couponID


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/****** Object:  Stored Procedure dbo.AddUserSession    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE UserSession_Insert
	@userID int,
	@groupID int,
	@clientID int,
	@tzOffset int,
	@sessionKey varchar(512)
AS 
	DECLARE @sessionID bigint;
	insert into user_sessions (modify_Time, user_ID, Effective_group_ID,  client_ID, TZ_Offset, session_key)
		values (getutcdate(), @userID, @groupID, @clientID, @tzOffset, @sessionKey )
	
	select @sessionID =  ident_current('user_sessions')
	insert into Session_History (session_ID,Group_ID,Client_ID, Session_Key)
		values ( @sessionID,@groupID,@clientID,@sessionKey);
	select @sessionID
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


/****** Object:  Stored Procedure dbo.SelectUserSession    Script Date: 5/18/2005 4:17:57 PM ******/

CREATE PROCEDURE UserSession_Retrieve
	@sessionID BigInt
AS 
	
	select user_id, effective_group_id, client_ID, tz_Offset, session_start_time, session_end_time, session_key
	 from user_sessions where session_ID = @sessionID
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


CREATE PROCEDURE UserSession_Update
	@sessionID bigint,
	@groupID int,
	@clientID int,
	@sessionKey varchar(512)
AS 
	update user_sessions set modify_Time = GETUTCDATE(),Effective_Group_ID=@groupID,
	client_ID=@clientID,session_Key=@sessionKey
	where session_ID= @sessionID
	insert into session_history (session_ID,Group_ID,Client_ID, session_key)
		values (@sessionID,@groupID,@clientID, @sessionKey)
	
	return 1

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE UserSession_UpdateClient
	@sessionid bigint,
	@clientID int
AS 
	update user_sessions set modify_time =getutcdate(),
 		client_ID=@clientID WHERE session_ID = @sessionID
return @@rowcount

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE UserSession_UpdateEndTime
	@sessionID BigInt

AS 
	update user_sessions set session_end_time = getutcdate()
	where session_id=@sessionID
	select @@rowcount
	--select session_end_time from user_sessions where session_ID = @sessionID
	--return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE UserSession_UpdateGroup
	@sessionid bigint,
	@groupID int
	
AS 
	update user_sessions set modify_time =getutcdate(), effective_group_id = @groupID
 		WHERE session_ID = @sessionID
return @@rowcount

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE UserSession_UpdateKey
	@sessionID bigint,
	@sessionKey varchar(512)
AS 
	update user_sessions set modify_time =getutcdate(),
 		session_key=@sessionKey WHERE session_ID = @sessionID
return @@rowcount

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE UserSession_UpdateTZOffset
	@sessionid bigint,
	@tzOffset int
AS 
	update user_sessions set modify_time =getutcdate(),
 		TZ_Offset=@tzOffset WHERE session_ID = @sessionID
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

CREATE PROCEDURE UserSessions_RetrieveAll
	@userID int,
	@groupID int,
	@TimeBefore DateTime,
	@TimeAfter DateTime

AS 
	select session_ID, session_start_time, session_end_time, effective_group_ID, tz_Offset,session_key
	from user_sessions 
	where user_ID=@userID
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

CREATE PROCEDURE User_Delete
	@userID int
AS
BEGIN TRANSACTION
	DECLARE @userName nvarchar(256)
	DECLARE @qualifierTypeID int
	DECLARE @agentID int
	
	select @agentID = (select agent_ID from Agents where agent_id = @userID)
	select @userName = (select user_name from Users where user_ID = @userID)
	delete from Agents 
	where agent_name = @userName
	if (@@error > 0)
		goto on_error
		
	/* Must update this logic if a qualifier type user is added */
	select @qualifierTypeID = (select qualifier_type_id from qualifier_types	
										where description = 'Agent')
	delete from qualifiers
	where qualifier_reference_ID = @agentID and qualifier_type_id=@qualifierTypeID
	if (@@error > 0)
		goto on_error
		
	delete from users
	where user_ID = @userID
	if (@@error > 0)
		goto on_error
COMMIT TRANSACTION	
return 1
	on_error: 
	ROLLBACK TRANSACTION
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


/****** Object:  Stored Procedure dbo.AddUser    Script Date: 5/18/2005 4:17:55 PM ******/

CREATE PROCEDURE User_Insert
	@userName nvarchar(256),
	@firstName nvarchar(256),
	@lastName nvarchar(256),
	@email nvarchar(256),
	@affiliation nvarchar(256),
	@reason nvarchar(2048),
	@XMLExtension text,
	@lockUser bit,
	@principalString nvarchar (256),
	@authenType varchar(256),
	@initialGroupID int
AS
	DECLARE @authTypeID int
	DECLARE @agentID int
	--DECLARE @parentGroupID int
	DECLARE @userID int

BEGIN TRANSACTION
	
	begin
		insert into agents (agent_name, is_group)
		values (@userName, 0)
		if (@@error > 0)
			goto on_error
		select @agentID=(select ident_current('Agents'))

		--select @parentGroupID = (select group_id from groups 
		--						where upper(group_name)=upper(@initialGroupID))
		insert into  agent_hierarchy (parent_group_ID, agent_ID)
		values (@initialGroupID, @agentID)
		if (@@error > 0)
			goto on_error
	
		insert into users (user_id, user_name,first_name,last_name,email, 
							affiliation, signup_reason, XML_Extension, lock_user)
		values (@agentID, @userName, @firstName,  @lastName, @email, 
				@affiliation, @reason, @XMLExtension, @lockUser)
		if (@@error > 0)
			goto on_error
		
		select @authTypeID = (select auth_type_id from authentication_types 
							where upper(description)=upper(@authenType))
		insert into principals (user_id, principal_string, auth_type_ID)
		values (@agentID,@principalString, @authTypeID)
		if (@@error > 0)
			goto on_error
	end
COMMIT TRANSACTION	
		
		select @agentID
return
	on_error: 
	ROLLBACK TRANSACTION
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


CREATE PROCEDURE User_Retrieve
	@userID int
AS
	select user_name, first_name, last_name, affiliation, XML_extension, signup_reason, 
			email, date_created, lock_user, password
	from users u 
	where u.user_id = @userID 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE User_RetrieveEmail
	@userName nvarchar(256)
AS
	select email
	from users
	where user_name = @userName

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE User_RetrieveGroups
	@userID int
AS
	select parent_group_ID 
	from   agent_hierarchy
	where agent_ID = @userID and parent_group_id NOT 
	in (select group_id from groups where  group_type_id = 2 or group_name='ROOT' or group_name='Group not assigned'
	or group_name='NewUserGroup' or group_name='OrphanedUserGroup')


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE User_RetrieveID
	@userName nvarchar(256)
AS
	select user_ID
	from users
	where user_name = @userName

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE User_RetrieveIDs
AS
	select user_id
	from users

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE User_RetrieveName
	@userID int
AS
	select user_Name
	from users
	where user_id = @userID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE User_RetrieveOrphanedIDs
AS
	DECLARE @orphanedGroupID int
	
	select @orphanedGroupID = (select group_ID from Groups where group_name = 'OrphanedUserGroup')
	
	select user_id
	from users, agent_hierarchy ah, agents
	where ah.parent_group_id=@orphanedGroupID and ah.agent_id=agents.agent_id and agents.agent_name = users.user_name

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE User_Update
	@userID int,
	@userName nvarchar(256),
	@firstName nvarchar(256),
	@lastName nvarchar(256),
	@email varchar(256),
	@affiliation nvarchar(256),
	@reason nvarchar(2048),
	@XMLExtension text,
	@lockUser bit,
	@principalString nvarchar (256),
	@authenType varchar(256)
AS
BEGIN TRANSACTION
	DECLARE @authTypeID int
	DECLARE @oldUserName nvarchar(256)
	DECLARE @qualifierTypeID int
		
	begin
		select @oldUserName = (select user_name from Users where user_id=@userID)
		if (@oldUserName != @userName)
		begin
			update agents
			set agent_name = @userName
			where agent_name = @oldUserName;
			if (@@error > 0)
				goto on_error
				
		/* Must update this logic if a qualifier type user is added */
			select @qualifierTypeID = (select qualifier_type_id from qualifier_types	
										where description = 'Agent')
			update qualifiers
			set qualifier_name = @userName
			where qualifier_reference_id = @userID and qualifier_name = @oldUserName 
					and qualifier_type_id = @qualifierTypeID
			if (@@error > 0)
				goto on_error
		end
		
		update users
		set user_name=@userName, first_name = @firstName, last_name=@lastName, 
			affiliation = @affiliation,  signup_reason=@reason, 
			XML_Extension = @XMLExtension, email = @email, lock_user=@lockUser
		where user_ID = @userID;
		if (@@error > 0)
			goto on_error
	
		select @authTypeID = (select auth_type_id from authentication_types where upper(description)=upper(@authenType))		
		update Principals 
		set principal_string=@principalString, auth_type_id = @authTypeID
		where user_id = @userID;
		if (@@error > 0)
			goto on_error
	end
COMMIT TRANSACTION	
return 1
	on_error: 
	ROLLBACK TRANSACTION
return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

