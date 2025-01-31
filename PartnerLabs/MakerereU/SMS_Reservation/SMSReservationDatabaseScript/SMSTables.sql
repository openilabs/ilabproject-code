/****** Object:  ForeignKey [FK_InComingMessages_LabConfiguration]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_InComingMessages_LabConfiguration]') AND type = 'F')
ALTER TABLE [dbo].[InComingMessages] DROP CONSTRAINT [FK_InComingMessages_LabConfiguration]
GO
/****** Object:  ForeignKey [FK_OutGoingMessages_LabConfiguration]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_OutGoingMessages_LabConfiguration]') AND type = 'F')
ALTER TABLE [dbo].[OutGoingMessages] DROP CONSTRAINT [FK_OutGoingMessages_LabConfiguration]
GO
/****** Object:  ForeignKey [FK_ProcessAgent_ProcessAgent_Type]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_ProcessAgent_ProcessAgent_Type]') AND type = 'F')
ALTER TABLE [dbo].[ProcessAgent] DROP CONSTRAINT [FK_ProcessAgent_ProcessAgent_Type]
GO
/****** Object:  ForeignKey [FK_Ticket_Coupon]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Ticket_Coupon]') AND type = 'F')
ALTER TABLE [dbo].[Ticket] DROP CONSTRAINT [FK_Ticket_Coupon]
GO
/****** Object:  ForeignKey [FK_Ticket_Ticket_Type]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Ticket_Ticket_Type]') AND type = 'F')
ALTER TABLE [dbo].[Ticket] DROP CONSTRAINT [FK_Ticket_Ticket_Type]
GO
/****** Object:  StoredProcedure [dbo].[SetIdentInCouponID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetIdentInCouponID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SetIdentInCouponID]
GO
/****** Object:  StoredProcedure [dbo].[SetIdentCoupons]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetIdentCoupons]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SetIdentCoupons]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagByGuid]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagByGuid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentTagByGuid]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTags]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTags]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentTags]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagsByTypeID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagsByTypeID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentTagsByTypeID]
GO
/****** Object:  StoredProcedure [dbo].[GetDomainProcessAgentTags]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetDomainProcessAgentTags]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetDomainProcessAgentTags]
GO
/****** Object:  StoredProcedure [dbo].[GetSelfProcessAgentID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetSelfProcessAgentID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetSelfProcessAgentID]
GO
/****** Object:  StoredProcedure [dbo].[SetSelfState]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetSelfState]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SetSelfState]
GO
/****** Object:  StoredProcedure [dbo].[UpdateDomainGuid]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateDomainGuid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateDomainGuid]
GO
/****** Object:  StoredProcedure [dbo].[SaveSystemSupportById]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SaveSystemSupportById]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveSystemSupportById]
GO
/****** Object:  StoredProcedure [dbo].[SaveSystemSupport]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SaveSystemSupport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveSystemSupport]
GO
/****** Object:  StoredProcedure [dbo].[RetrieveSystemSupportById]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[RetrieveSystemSupportById]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RetrieveSystemSupportById]
GO
/****** Object:  StoredProcedure [dbo].[RetrieveSystemSupport]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[RetrieveSystemSupport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RetrieveSystemSupport]
GO
/****** Object:  StoredProcedure [dbo].[RemoveDomainCredentials]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[RemoveDomainCredentials]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RemoveDomainCredentials]
GO
/****** Object:  StoredProcedure [dbo].[SetProcessAgentRetired]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetProcessAgentRetired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SetProcessAgentRetired]
GO
/****** Object:  StoredProcedure [dbo].[IsProcessAgentIDRetired]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[IsProcessAgentIDRetired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[IsProcessAgentIDRetired]
GO
/****** Object:  StoredProcedure [dbo].[IsProcessAgentRetired]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[IsProcessAgentRetired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[IsProcessAgentRetired]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentURLbyID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentURLbyID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentURLbyID]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentURL]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentURL]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentURL]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentIDsByTypeID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentIDsByTypeID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentIDsByTypeID]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentID]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentName]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentName]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentName]
GO
/****** Object:  StoredProcedure [dbo].[SetIdentOutCouponID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetIdentOutCouponID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SetIdentOutCouponID]
GO
/****** Object:  StoredProcedure [dbo].[GetCouponCollectionCount]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetCouponCollectionCount]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetCouponCollectionCount]
GO
/****** Object:  StoredProcedure [dbo].[DeleteTicketByID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteTicketByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteTicketByID]
GO
/****** Object:  StoredProcedure [dbo].[CancelTicketByID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CancelTicketByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CancelTicketByID]
GO
/****** Object:  StoredProcedure [dbo].[DeleteIssuerTickets]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteIssuerTickets]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteIssuerTickets]
GO
/****** Object:  StoredProcedure [dbo].[DeleteTickets]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteTickets]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteTickets]
GO
/****** Object:  StoredProcedure [dbo].[GetTicketIDs]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketIDs]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetTicketIDs]
GO
/****** Object:  StoredProcedure [dbo].[InsertTicket]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InsertTicket]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[InsertTicket]
GO
/****** Object:  StoredProcedure [dbo].[DeleteTicket]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteTicket]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteTicket]
GO
/****** Object:  StoredProcedure [dbo].[RetrieveTicketTypes]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[RetrieveTicketTypes]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RetrieveTicketTypes]
GO
/****** Object:  StoredProcedure [dbo].[GetTicketByID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetTicketByID]
GO
/****** Object:  StoredProcedure [dbo].[GetTicketsByType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetTicketsByType]
GO
/****** Object:  StoredProcedure [dbo].[CancelTicket]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CancelTicket]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CancelTicket]
GO
/****** Object:  StoredProcedure [dbo].[GetTicket]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicket]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetTicket]
GO
/****** Object:  StoredProcedure [dbo].[GetTicketID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetTicketID]
GO
/****** Object:  StoredProcedure [dbo].[GetTicketsByCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketsByCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetTicketsByCoupon]
GO
/****** Object:  StoredProcedure [dbo].[WriteSelfProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[WriteSelfProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WriteSelfProcessAgent]
GO
/****** Object:  StoredProcedure [dbo].[ReadSelfProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ReadSelfProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ReadSelfProcessAgent]
GO
/****** Object:  StoredProcedure [dbo].[GetDomainProcessAgentTagsByType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetDomainProcessAgentTagsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetDomainProcessAgentTagsByType]
GO
/****** Object:  StoredProcedure [dbo].[InsertProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InsertProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[InsertProcessAgent]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTypeID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTypeID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentTypeID]
GO
/****** Object:  StoredProcedure [dbo].[ProcessAgentTypeExists]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ProcessAgentTypeExists]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ProcessAgentTypeExists]
GO
/****** Object:  StoredProcedure [dbo].[GetDomainProcessAgentTagsWithType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetDomainProcessAgentTagsWithType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetDomainProcessAgentTagsWithType]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagsByType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentTagsByType]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagsWithTypeById]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagsWithTypeById]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentTagsWithTypeById]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagsWithType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagsWithType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentTagsWithType]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagByGuidWithType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagByGuidWithType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentTagByGuidWithType]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentNameWithType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentNameWithType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentNameWithType]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgent]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgents]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgents]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgents]
GO
/****** Object:  StoredProcedure [dbo].[UpdateProcessAgentByID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateProcessAgentByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateProcessAgentByID]
GO
/****** Object:  StoredProcedure [dbo].[UpdateProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateProcessAgent]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentsByType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentsByType]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentByID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentByID]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentIDsByType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentIDsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentIDsByType]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfoByOutCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfoByOutCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentInfoByOutCoupon]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfoByType]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfoByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentInfoByType]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfoByID]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfoByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentInfoByID]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfo]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfo]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentInfo]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfosForDomain]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfosForDomain]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentInfosForDomain]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentsInfo]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentsInfo]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentsInfo]
GO
/****** Object:  StoredProcedure [dbo].[GetExpiredTickets]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetExpiredTickets]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetExpiredTickets]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfoByInCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfoByInCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetProcessAgentInfoByInCoupon]
GO
/****** Object:  StoredProcedure [dbo].[CancelCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CancelCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CancelCoupon]
GO
/****** Object:  StoredProcedure [dbo].[GetIdentOutCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetIdentOutCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetIdentOutCoupon]
GO
/****** Object:  StoredProcedure [dbo].[GetIdentInCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetIdentInCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetIdentInCoupon]
GO
/****** Object:  StoredProcedure [dbo].[DeleteCoupons]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteCoupons]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteCoupons]
GO
/****** Object:  StoredProcedure [dbo].[GetCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetCoupon]
GO
/****** Object:  StoredProcedure [dbo].[AuthenticateCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[AuthenticateCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AuthenticateCoupon]
GO
/****** Object:  StoredProcedure [dbo].[InsertCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InsertCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[InsertCoupon]
GO
/****** Object:  StoredProcedure [dbo].[DeleteCoupon]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteCoupon]
GO
/****** Object:  Table [dbo].[ProcessAgent]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ProcessAgent]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[ProcessAgent]
GO
/****** Object:  Table [dbo].[Ticket]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Ticket]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Ticket]
GO
/****** Object:  Table [dbo].[OutGoingMessages]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[OutGoingMessages]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[OutGoingMessages]
GO
/****** Object:  Table [dbo].[InComingMessages]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InComingMessages]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[InComingMessages]
GO
/****** Object:  StoredProcedure [dbo].[GetSelfState]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetSelfState]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetSelfState]
GO
/****** Object:  Table [dbo].[Ticket_Type]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Ticket_Type]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Ticket_Type]
GO
/****** Object:  UserDefinedFunction [dbo].[toIntArray]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[toIntArray]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].[toIntArray]
GO
/****** Object:  UserDefinedFunction [dbo].[toLongArray]    Script Date: 05/11/2011 12:29:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[toLongArray]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].[toLongArray]
GO
/****** Object:  Table [dbo].[ProcessAgent_Type]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ProcessAgent_Type]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[ProcessAgent_Type]
GO
/****** Object:  Table [dbo].[Coupon]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Coupon]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Coupon]
GO
/****** Object:  Table [dbo].[ErrorDescriptions]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ErrorDescriptions]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[ErrorDescriptions]
GO
/****** Object:  Table [dbo].[LabConfiguration]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[LabConfiguration]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[LabConfiguration]
GO
/****** Object:  Default [DF_ProcessAgent_Retired]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ProcessAgent_Retired]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProcessAgent] DROP CONSTRAINT [DF_ProcessAgent_Retired]

END
GO
/****** Object:  Default [DF_ProcessAgent_Self]    Script Date: 05/11/2011 12:29:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ProcessAgent_Self]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProcessAgent] DROP CONSTRAINT [DF_ProcessAgent_Self]

END
GO
/****** Object:  Table [dbo].[LabConfiguration]    Script Date: 05/11/2011 12:29:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[LabConfiguration]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[LabConfiguration](
	[LabConfigurationID] [int] IDENTITY(1,1) NOT NULL,
	[applicationCallName] [nchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LabName] [nchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LabDescription] [nchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExperimentGroupName] [nchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ServiceBrokerGUID] [nchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ServiceBrokerURL] [nchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ClientName] [nchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ClientGuid] [nchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LabServerName] [nchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LabServerGuid] [nchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[MaximumLabDuration] [int] NULL,
	[DateCreated] [datetime] NULL,
 CONSTRAINT [PK_LabConfiguration_1] PRIMARY KEY CLUSTERED 
(
	[LabConfigurationID] ASC
)
)
END
GO
SET IDENTITY_INSERT [dbo].[LabConfiguration] ON
INSERT [dbo].[LabConfiguration] ([LabConfigurationID], [applicationCallName], [LabName], [LabDescription], [ExperimentGroupName], [ServiceBrokerGUID], [ServiceBrokerURL], [ClientName], [ClientGuid], [LabServerName], [LabServerGuid], [MaximumLabDuration], [DateCreated]) VALUES (1, N'none      ', N'no lab                                                                ', N'this path shows that there is no lab of this name                                                                                                                                                                                                                                                                                                                                                                                                                                                                   ', N'no group                                                              ', N'no serviceGuid                                                        ', N'no serviceGuid                                                        ', N'no client name                                                        ', N'no client Guid                                                        ', N'no lab server                                                         ', N'no lab server Guid                                                    ', 0, CAST(0x00009EDC00A6CD96 AS DateTime))
SET IDENTITY_INSERT [dbo].[LabConfiguration] OFF
/****** Object:  Table [dbo].[ErrorDescriptions]    Script Date: 05/11/2011 12:29:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ErrorDescriptions]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[ErrorDescriptions](
	[ErrorID] [int] IDENTITY(1,1) NOT NULL,
	[nameError] [nchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[codeError] [nchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShortDescription] [nchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LongDescription] [nchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PossibleSoln] [nchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ErrorDescriptions] PRIMARY KEY CLUSTERED 
(
	[ErrorID] ASC
)
)
END
GO
SET IDENTITY_INSERT [dbo].[ErrorDescriptions] ON
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (1, N'Start DateTime Format Error   ', N'SDT01     ', N'The Start time range provided is wrongly formatted.         ', N'The Start Time Range requested by the user is wrongly formatted                                                                                                                                                                                                                                                                                                                                                                                                                                                     ', N'Use "DD/MM/YYYY" format for date and HH:MM for Time                             ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (2, N'Start DateTime outdated       ', N'SDT02     ', N'The Start Time range provided is past                       ', N'The Start Time Range is already past                                                                                                                                                                                                                                                                                                                                                                                                                                                                                ', N'Ensure that date time is uptodate of the form "DD/MM/YYYY" and time "HH:MM"     ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (3, N'End Date Time Format Error    ', N'EDT01     ', N'The End time range provided is wrongly formatted.           ', N'The End Time Range requested by the user is wrongly formatted                                                                                                                                                                                                                                                                                                                                                                                                                                                       ', N'Use "DD/MM/YYYY" format for date and HH:MM for Time                             ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (4, N'End Date Time outdated        ', N'EDT02     ', N'The End Time range provided is past                         ', N'The End Time Range is already past                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  ', N'Ensure that date time is uptodate of the form "DD/MM/YYYY" and time "HH:MM"     ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (5, N'Negative Time Range           ', N'SDT00     ', N'Start Time Range provided is after the end time range.      ', N'When the Start time Range provided is later than the End Time Range provided                                                                                                                                                                                                                                                                                                                                                                                                                                        ', N'Ensure that the end time range is after the start time range.                   ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (6, N'Small Time Range              ', N'STR01     ', N'Time Range provided is smaller than minimum duration        ', N'When the time Range provided by doing the lab is much smaller than the actual minimum time required by the user to do a lab as specified by the administrator                                                                                                                                                                                                                                                                                                                                                       ', N'The system has provided you with the minimum time duration                      ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (7, N'Database Connection Failure   ', N'DBF01     ', N'Failed to connect to database  for lab config values        ', N'Internal Error when the application fails to connect to the database to retrieve the required values for scheduling the user into the system                                                                                                                                                                                                                                                                                                                                                                        ', N'Please try again later                                                          ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (8, N'Message Format Error          ', N'MFE01     ', N'The message is not well formatted.                          ', N'Message sent by the user is not formatted as the default description earlier explained.                                                                                                                                                                                                                                                                                                                                                                                                                             ', N'Please Format Message as "Username Labname Date StartTimeRange EndTimeRange".   ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (9, N'Default Error                 ', N'DEF00     ', N'Instant scheduling failed.                                  ', N'This is the default error when the application cannot resolve the possible cause of the error                                                                                                                                                                                                                                                                                                                                                                                                                       ', N'Cannot resolve possible cause of error. Please try again later                  ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (10, N'username unavailable          ', N'SBE01     ', N'Username provided has not been registered                   ', N'The username provided has not been registered by the in the service broker that holds the lab being scheduled.                                                                                                                                                                                                                                                                                                                                                                                                      ', N' Please first register with the Service broker.                                 ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (11, N'user not group mem            ', N'SBE02     ', N'Username given not a member of lab group                    ', N'The username provided is present in the service broker database but is not a member of the group authorized to do the lab                                                                                                                                                                                                                                                                                                                                                                                           ', N'Contact administrator for group membership.                                     ')
INSERT [dbo].[ErrorDescriptions] ([ErrorID], [nameError], [codeError], [ShortDescription], [LongDescription], [PossibleSoln]) VALUES (12, N'labname unavailable           ', N'LRU01     ', N'labname acronym given is not recoganized.                   ', N'The labname or labname acronym given by the user does not belong to any lab that supports this scheduling system                                                                                                                                                                                                                                                                                                                                                                                                    ', N'Ensure the labname given is accurate.                                           ')
SET IDENTITY_INSERT [dbo].[ErrorDescriptions] OFF
/****** Object:  Table [dbo].[Coupon]    Script Date: 05/11/2011 12:29:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Coupon]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Coupon](
	[Coupon_ID] [bigint] NOT NULL,
	[Cancelled] [bit] NOT NULL,
	[Issuer_GUID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Passkey] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [IX_Coupon] UNIQUE NONCLUSTERED 
(
	[Coupon_ID] ASC,
	[Issuer_GUID] ASC
)
)
END
GO
/****** Object:  Table [dbo].[ProcessAgent_Type]    Script Date: 05/11/2011 12:29:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ProcessAgent_Type]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[ProcessAgent_Type](
	[ProcessAgent_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Short_Name] [char](4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar](512) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ProcessAgent_Type] PRIMARY KEY CLUSTERED 
(
	[ProcessAgent_Type_ID] ASC
)
)
END
GO
SET IDENTITY_INSERT [dbo].[ProcessAgent_Type] ON
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (1, convert(text, N'NOTA' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'NOT A PA' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (4, convert(text, N' ISB' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'SERVICE BROKER' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (5, convert(text, N' BSB' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'BATCH SERVICE BROKER' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (6, convert(text, N' RSB' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'REMOTE SERVICE BROKER' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (8, convert(text, N' ILS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'LAB SERVER' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (9, convert(text, N' BLS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'BATCH LAB SERVER' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (16, convert(text, N' ESS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'EXPERIMENT STORAGE SERVER' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (32, convert(text, N' USS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'SCHEDULING SERVER' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (64, convert(text, N' LSS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'LAB SCHEDULING SERVER' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (128, convert(text, N'AUTH' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'AUTHORIZATION SERVICE' collate SQL_Latin1_General_CP1_CI_AS))
INSERT [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID], [Short_Name], [Description]) VALUES (256, convert(text, N' GPA' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'GENERIC PA' collate SQL_Latin1_General_CP1_CI_AS))
SET IDENTITY_INSERT [dbo].[ProcessAgent_Type] OFF
/****** Object:  UserDefinedFunction [dbo].[toLongArray]    Script Date: 05/11/2011 12:29:12 ******/
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[toLongArray]') AND xtype in (N'FN', N'IF', N'TF'))
BEGIN
execute dbo.sp_executesql @statement = N'
CREATE FUNCTION [dbo].[toLongArray]
(
	@list varchar(8000)
)
RETURNS @array TABLE(lid bigint)
AS
BEGIN
DECLARE @tempList table(id BigInt)

	DECLARE @idStr varchar(100), @Pos int

	SET @list = LTRIM(RTRIM(@list))+ '',''
	SET @Pos = CHARINDEX('','', @list, 1)

	IF REPLACE(@list, '','', '''') <> ''''
	BEGIN
		WHILE @Pos > 0
		BEGIN
			SET @idStr = LTRIM(RTRIM(LEFT(@list, @Pos - 1)))
			IF @idStr <> ''''
			BEGIN
				INSERT INTO @tempList (ID) VALUES (CAST(@idStr AS bigint)) --Use Appropriate conversion
			END
			SET @list = RIGHT(@list, LEN(@list) - @Pos)
			SET @Pos = CHARINDEX('','', @list, 1)

		END
	END	

	insert @array SELECT ID FROM @tempList
RETURN	
END

' 
END
GO
/****** Object:  UserDefinedFunction [dbo].[toIntArray]    Script Date: 05/11/2011 12:29:11 ******/
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[toIntArray]') AND xtype in (N'FN', N'IF', N'TF'))
BEGIN
execute dbo.sp_executesql @statement = N'
CREATE FUNCTION [dbo].[toIntArray]
(
	@list varchar(8000)
)
RETURNS @array TABLE(lid int)
AS
BEGIN
DECLARE @tempList table(id Int)

	DECLARE @idStr varchar(100), @Pos int

	SET @list = LTRIM(RTRIM(@list))+ '',''
	SET @Pos = CHARINDEX('','', @list, 1)

	IF REPLACE(@list, '','', '''') <> ''''
	BEGIN
		WHILE @Pos > 0
		BEGIN
			SET @idStr = LTRIM(RTRIM(LEFT(@list, @Pos - 1)))
			IF @idStr <> ''''
			BEGIN
				INSERT INTO @tempList (ID) VALUES (CAST(@idStr AS int)) --Use Appropriate conversion
			END
			SET @list = RIGHT(@list, LEN(@list) - @Pos)
			SET @Pos = CHARINDEX('','', @list, 1)

		END
	END	

	insert @array SELECT ID FROM @tempList
RETURN	
END

' 
END
GO
/****** Object:  Table [dbo].[Ticket_Type]    Script Date: 05/11/2011 12:29:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Ticket_Type]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Ticket_Type](
	[Ticket_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Short_Description] [varchar](512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Abstract] [bit] NOT NULL,
 CONSTRAINT [PK_Ticket_Type] PRIMARY KEY CLUSTERED 
(
	[Ticket_Type_ID] ASC
)
)
END
GO
SET IDENTITY_INSERT [dbo].[Ticket_Type] ON
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (1, convert(text, N'ADMINISTER PA' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Administer Process Agent' collate SQL_Latin1_General_CP1_CI_AS), 1)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (2, convert(text, N'MANAGE PA' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Manage Process Agent' collate SQL_Latin1_General_CP1_CI_AS), 1)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (3, convert(text, N'AUTHENTICATE' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Authenticate Ticket' collate SQL_Latin1_General_CP1_CI_AS), 1)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (4, convert(text, N'LS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Lab Server Ticket' collate SQL_Latin1_General_CP1_CI_AS), 1)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (5, convert(text, N'LSS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Lab Scheduling Server Ticket' collate SQL_Latin1_General_CP1_CI_AS), 1)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (6, convert(text, N'USS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'User Scheduling Server Ticket' collate SQL_Latin1_General_CP1_CI_AS), 1)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (7, convert(text, N'ESS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Experiment Storage Server Ticket' collate SQL_Latin1_General_CP1_CI_AS), 1)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (8, convert(text, N'AUTHENTICATE SERVICE BROKER' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Authenticate Service Broker' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (9, convert(text, N'AUTHENTICATE AGENT' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Authenticate Process Agent' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (10, convert(text, N'REDEEM SESSION' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Redeem Session' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (11, convert(text, N'ADMINISTER ESS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Administer ESS' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (12, convert(text, N'ADMINISTER EXPERIMENT' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Administer Experiment' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (13, convert(text, N'STORE RECORDS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Store Records' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (14, convert(text, N'RETRIEVE RECORDS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Retrieve Records' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (15, convert(text, N'ADMINISTER USS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Administer USS' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (16, convert(text, N'MANAGE USS GROUP' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'USS Manage Group' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (17, convert(text, N'SCHEDULE SESSION' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Schedule Session' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (18, convert(text, N'REVOKE RESERVATION' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Revoke Reservation' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (19, convert(text, N'ALLOW EXPERIMENT EXECUTION' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Allow Experiment Execution' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (20, convert(text, N'ADMINISTER LSS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Administer LSS' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (21, convert(text, N'MANAGE LAB' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Manage Lab' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (22, convert(text, N'REQUEST RESERVATION' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Request Reservation' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (23, convert(text, N'REGISTER LS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Register LS' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (24, convert(text, N'ADMINISTER LS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Administer LS' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (25, convert(text, N'EXECUTE EXPERIMENT' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Execute Experiment' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (26, convert(text, N'CREATE EXPERIMENT' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Create Experiment' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (27, convert(text, N'REDEEM RESERVATION' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Redeem Reservation' collate SQL_Latin1_General_CP1_CI_AS), 0)
INSERT [dbo].[Ticket_Type] ([Ticket_Type_ID], [Name], [Short_Description], [Abstract]) VALUES (28, convert(text, N'AUTHORIZE_ACCESS' collate SQL_Latin1_General_CP1_CI_AS), convert(text, N'Authorize Access' collate SQL_Latin1_General_CP1_CI_AS), 0)
SET IDENTITY_INSERT [dbo].[Ticket_Type] OFF
/****** Object:  StoredProcedure [dbo].[GetSelfState]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetSelfState]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
Create Procedure [dbo].[GetSelfState]
@guid varchar(50)
as
select self from ProcesAgent  where Agent_Guid =@guid
' 
END
GO
/****** Object:  Table [dbo].[InComingMessages]    Script Date: 05/11/2011 12:29:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InComingMessages]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[InComingMessages](
	[IncomingMessageID] [int] IDENTITY(1,1) NOT NULL,
	[Telephone] [nchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[username] [nchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LabConfigurationID] [int] NULL,
	[StartTimeRange] [datetime] NULL,
	[EndTimeRange] [datetime] NULL,
	[TimeReceived] [datetime] NULL,
	[MessageKey] [nchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[RwRecievedMsg] [nchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_InComingMessages] PRIMARY KEY CLUSTERED 
(
	[IncomingMessageID] ASC
)
)
END
GO
/****** Object:  Table [dbo].[OutGoingMessages]    Script Date: 05/11/2011 12:29:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[OutGoingMessages]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[OutGoingMessages](
	[OutGoingMessageID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[LabConfigurationID] [int] NULL,
	[IsScheduled] [bit] NULL,
	[GivenStartTime] [datetime] NULL,
	[GivenEndTime] [datetime] NULL,
	[RwSentMsg] [nchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[MessageKey] [nchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TimeAndDateSent] [datetime] NULL,
	[codeError] [nchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_OutGoingMessages] PRIMARY KEY CLUSTERED 
(
	[OutGoingMessageID] ASC
)
)
END
GO
/****** Object:  Table [dbo].[Ticket]    Script Date: 05/11/2011 12:29:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Ticket]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Ticket](
	[Ticket_ID] [bigint] NOT NULL,
	[Ticket_Type_ID] [int] NOT NULL,
	[Creation_Time] [datetime] NOT NULL,
	[Duration] [bigint] NOT NULL,
	[Cancelled] [bit] NOT NULL,
	[Payload] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Coupon_ID] [bigint] NOT NULL,
	[Issuer_GUID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Redeemer_GUID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Sponsor_GUID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [IX_Ticket] UNIQUE NONCLUSTERED 
(
	[Ticket_ID] ASC,
	[Issuer_GUID] ASC
),
 CONSTRAINT [IX_Ticket_Value] UNIQUE NONCLUSTERED 
(
	[Ticket_Type_ID] ASC,
	[Redeemer_GUID] ASC,
	[Issuer_GUID] ASC,
	[Coupon_ID] ASC
)
)
END
GO
/****** Object:  Table [dbo].[ProcessAgent]    Script Date: 05/11/2011 12:29:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ProcessAgent]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[ProcessAgent](
	[Agent_ID] [int] IDENTITY(10,1) NOT NULL,
	[Retired] [bit] NOT NULL,
	[Self] [bit] NOT NULL,
	[ProcessAgent_Type_ID] [int] NOT NULL,
	[IdentIn_ID] [bigint] NULL,
	[IdentOut_ID] [bigint] NULL,
	[Issuer_GUID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Domain_GUID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Agent_GUID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Agent_Name] [nvarchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[WebService_URL] [nvarchar](512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Codebase_URL] [nvarchar](512) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Info_URL] [nvarchar](512) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Contact_Email] [nvarchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Bug_Email] [nvarchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Location] [nvarchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ProcessAgent] PRIMARY KEY CLUSTERED 
(
	[Agent_ID] ASC
),
 CONSTRAINT [IX_ProcessAgent] UNIQUE NONCLUSTERED 
(
	[Agent_GUID] ASC
)
)
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[DeleteCoupon]
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

' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InsertCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'-- Inserts a coupon
CREATE PROCEDURE [dbo].[InsertCoupon]
@couponID bigint,
@issuerGUID varchar (50),
@passKey varchar(100),
@cancelled  bit = 0

 AS

insert into Coupon(coupon_ID, issuer_GUID, Passkey, Cancelled)
values(@couponID,@issuerGUID, @passKey, @cancelled)

' 
END
GO
/****** Object:  StoredProcedure [dbo].[AuthenticateCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[AuthenticateCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

-- This version expects all coupons to be in the Coupon Table, 
-- may be over written in TicketIssuer database,
CREATE PROCEDURE [dbo].[AuthenticateCoupon]
@couponID bigint,
@issuerGUID varchar (50),
@passKey varchar(100)

 AS

select coupon_ID from Coupon 
where EXISTS ( SELECT * Where coupon_ID=@couponID AND issuer_GUID=@issuerGUID AND passkey = @passKey AND cancelled = 0)

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetCoupon] 
@issuerGUID varchar(50),
@couponID bigint

 AS

select Cancelled, Passkey
 from Coupon
 where Coupon_ID = @couponID AND Issuer_GUID = @issuerGUID
' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteCoupons]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteCoupons]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[DeleteCoupons]
@issuerGUID varchar (50)
 AS
declare @ticketCount int

select @ticketCount = COUNT(ticket_ID) from Ticket where Issuer_GUID = @issuerGUID
if (@ticketCount > 0)
GOTO hasTickets

delete from coupon where Issuer_GUID = @issuerGUID
if (@@rowcount = 0)
goto on_error
if (@@error>0)
goto on_error

return 1

on_error:
return 0

hasTickets:
return -1

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetIdentInCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetIdentInCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetIdentInCoupon] 
@agentGUID varchar(50)

AS
select cancelled,Coupon_ID,issuer_Guid,passkey
from Coupon
where Coupon_ID = (Select identIn_ID from ProcessAgent  where agent_Guid = @agentGUID)
AND Issuer_Guid = (Select issuer_GUID from ProcessAgent where agent_Guid = @agentGUID)

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetIdentOutCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetIdentOutCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetIdentOutCoupon] 
@agentGUID varchar(50)

AS


select cancelled,Coupon_ID,issuer_Guid,passkey
from Coupon
where Coupon_ID = (Select identOut_ID from ProcessAgent  where agent_Guid = @agentGUID)
AND Issuer_Guid = (Select issuer_GUID from ProcessAgent where agent_Guid = @agentGUID)


' 
END
GO
/****** Object:  StoredProcedure [dbo].[CancelCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CancelCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[CancelCoupon]
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

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfoByInCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfoByInCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentInfoByInCoupon]
@couponID bigint, 
@issuer varchar (50)
 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID,
Codebase_URL, WebService_URL,  Domain_Guid, issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID),
retired
from ProcessAgent
where IdentIn_ID = @couponID AND issuer_GUID = @issuer



' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetExpiredTickets]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetExpiredTickets]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'-- Returns the TicketID and coupon for an expired ticket
CREATE PROCEDURE [dbo].[GetExpiredTickets]
AS
select t.ticket_id, t.coupon_id, t.Issuer_guid, c.passkey
from ticket t, coupon c
where t.cancelled = 1 OR
(t.cancelled = 0 and duration != -1 and DATEDIFF(second,creation_Time,GETUTCDATE()) > duration
and t.coupon_id = c.Coupon_id and t.issuer_guid = c.issuer_guid)

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentsInfo]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentsInfo]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'



-- AgentInfo, a utility structure for processing service requests.
CREATE PROCEDURE [dbo].[GetProcessAgentsInfo] 

 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID, Codebase_URL, WebService_URL, Domain_Guid,
issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where ProcessAgent.Issuer_GUID != null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_GUID != null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID),
retired
from ProcessAgent

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfosForDomain]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfosForDomain]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetProcessAgentInfosForDomain] 
@guid varchar(50)
 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID, Codebase_URL, WebService_URL, Domain_Guid,
issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where ProcessAgent.Issuer_GUID != null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_GUID != null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID),
retired
from ProcessAgent where Domain_Guid = @guid

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfo]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfo]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetProcessAgentInfo] 
@guid varchar (50)
 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID, Codebase_URL, WebService_URL, Domain_Guid,
issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID),
retired
from ProcessAgent
where Agent_GUID = @guid



' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfoByID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfoByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentInfoByID] 
@id int
 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID,
Codebase_URL, WebService_URL,  Domain_Guid, issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where ProcessAgent.Issuer_Guid != Null AND  coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID),
retired
from ProcessAgent
where Agent_ID = @id



' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfoByType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfoByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentInfoByType] 
@agentType varchar (100)
 AS
declare @typeID int

select @typeID = ProcessAgent_Type_ID from  ProcessAgent_Type where ProcessAgent_Type.Description = upper(@agentType)
select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID,
Codebase_URL,  WebService_URL,  Domain_Guid, issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID),
retired
from ProcessAgent
where ProcessAgent_Type_ID = @typeID



' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentInfoByOutCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentInfoByOutCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentInfoByOutCoupon]
@couponID bigint, 
@issuer varchar (50)
 AS

select Agent_ID, Agent_GUID,  Agent_Name, ProcessAgent_Type_ID,
Codebase_URL, WebService_URL,  Domain_Guid, issuer_GUID, 
IdentIn_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentIn_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID), 
IdentOut_ID, (Select passkey from coupon where  ProcessAgent.Issuer_Guid != Null AND coupon_id = IdentOut_ID AND ProcessAgent.Issuer_GUID = Coupon.Issuer_GUID),
retired
from ProcessAgent
where IdentOut_ID = @couponID AND issuer_GUID = @issuer



' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentIDsByType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentIDsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentIDsByType]
@type varchar (100)
 AS
declare @typeID int
select @typeID= processAgent_type_id from ProcessAgent_type where description = @type
select Agent_ID from ProcessAgent
where ProcessAgent_type_id= @typeid  AND retired = 0
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentByID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentByID] 

@ID int

 AS

SELECT Agent_ID, Agent_GUID, Agent_Name, ProcessAgent_Type.Description,
Domain_Guid, Codebase_URL, WebService_URL
FROM ProcessAgent JOIN ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
WHERE Agent_ID= @ID  AND retired = 0


' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentsByType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetProcessAgentsByType]

@agentType varchar(100)

 AS

DECLARE @agentTypeID int 

select @agentTypeID = (select ProcessAgent_Type_ID from  ProcessAgent_Type 
where ProcessAgent_Type.Description = upper(@agentType))

select Agent_ID, Agent_GUID, Agent_Name, upper(@agentType),
Domain_Guid, Codebase_URL, WebService_URL
from ProcessAgent where ProcessAgent_Type_ID= @agentTypeID  AND retired = 0



' 
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[UpdateProcessAgent]
@guid varchar(50),
@domain varchar (50), 
@name nvarchar(256),
@type varchar(100),
@codeBaseURL nvarchar(512),
@webServiceURL nvarchar(512)

AS
Declare @processAgentTypeID int 

select @processAgentTypeID = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@type )))
update ProcessAgent set ProcessAgent_Type_ID = @processAgentTypeID,
 Agent_name = @name, Codebase_URL = @codeBaseURL, WebService_URL = @webServiceURL, Domain_Guid = @domain
where Agent_GUID = @guid
select @@rowcount

' 
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateProcessAgentByID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateProcessAgentByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[UpdateProcessAgentByID]
@id int,
@guid varchar(50),
@domain varchar (50), 
@name nvarchar(256),
@type varchar(100),
@codeBaseURL nvarchar(512) = null,
@webServiceURL nvarchar(512)

AS
Declare @processAgentTypeID int 

select @processAgentTypeID = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@type )))
update ProcessAgent set Agent_GUID = @guid, ProcessAgent_Type_ID = @processAgentTypeID,
 Agent_name = @name, Codebase_URL = @codeBaseURL, WebService_URL = @webServiceURL, Domain_Guid = @domain
where Agent_ID = @id
select @@rowcount

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgents]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgents]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetProcessAgents] 
 AS

select Agent_ID, Agent_GUID, Agent_Name, ProcessAgent_Type.Description,
Domain_guid, Codebase_URL, WebService_URL
from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
where retired = 0
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgent] 

@agentGUID varchar(50)

 AS

SELECT Agent_ID, Agent_GUID, Agent_Name, ProcessAgent_Type.Description,
 Domain_Guid, Codebase_URL, WebService_URL
FROM ProcessAgent JOIN ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
WHERE Agent_GUID= @agentGUID AND retired = 0

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentNameWithType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentNameWithType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentNameWithType]
@agentID int
 AS
select ProcessAgent_Type.Short_Name, Agent_Name

from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)

where agent_ID = @agentID AND retired = 0

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagByGuidWithType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagByGuidWithType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentTagByGuidWithType]
@guid varchar(50)
 AS
select agent_ID, ProcessAgent_Type.Short_Name, Agent_Name
from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
where agent_Guid = @guid AND retired = 0

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagsWithType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagsWithType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentTagsWithType]
 AS
select agent_ID, ProcessAgent_Type.Short_Name, Agent_Name

from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
where retired = 0
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagsWithTypeById]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagsWithTypeById]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentTagsWithTypeById]
@agentID int
 AS
select agent_ID, ProcessAgent_Type.Short_Name, Agent_Name

from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
where agent_ID = @agentID  AND retired = 0
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagsByType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetProcessAgentTagsByType]

@type varchar(100)

 AS

select agent_ID, Agent_Name from ProcessAgent
 where retired = 0 AND ProcessAgent_type_id = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@type )))


' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetDomainProcessAgentTagsWithType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetDomainProcessAgentTagsWithType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetDomainProcessAgentTagsWithType]
@domain varchar(50)
 AS
select agent_ID, ProcessAgent_Type.Short_Name, Agent_Name

from ProcessAgent join ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
where domain_guid = @domain  AND retired = 0
' 
END
GO
/****** Object:  StoredProcedure [dbo].[ProcessAgentTypeExists]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ProcessAgentTypeExists]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[ProcessAgentTypeExists]

@type varchar(100)

 AS

select ProcessAgent_Type_ID from ProcessAgent_Type where ProcessAgent_Type.Description = @type

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTypeID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTypeID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentTypeID]

@type varchar(100)

 AS

select ProcessAgent_Type_ID from ProcessAgent_Type where ProcessAgent_Type.Description = @type

' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InsertProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[InsertProcessAgent] 
@processAgentType varchar(100),
@guid varchar(50),
@name nvarchar(256),
@codeBaseURL nvarchar(512) = null,
@webServiceURL nvarchar(512),
@issuer varchar(50) = null,
@inID bigint = null,
@outID bigint = null,
@domain varchar (50)
AS
Declare @processAgentTypeID int 

select @processAgentTypeID = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@processAgentType )))
insert into ProcessAgent( Agent_GUID,domain_Guid, Agent_Name,ProcessAgent_Type_ID, Codebase_URL, WebService_URL, Issuer_GUID, IdentIn_ID,IdentOut_ID)
values ( @guid, @domain, @name, @processAgentTypeID,  @codeBaseURL, @webServiceURL, @issuer, @inID, @outID )
select ident_current(''ProcessAgent'')

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetDomainProcessAgentTagsByType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetDomainProcessAgentTagsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetDomainProcessAgentTagsByType]

@type varchar(100),
@domain varchar(50)

 AS

select agent_ID, Agent_Name from ProcessAgent
 where retired = 0 AND ProcessAgent_type_id = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@type )))
and domain_Guid=@domain


' 
END
GO
/****** Object:  StoredProcedure [dbo].[ReadSelfProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ReadSelfProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
Create PROCEDURE [dbo].[ReadSelfProcessAgent] 

AS
Declare @processAgentTypeID int 
declare @selfID int


SELECT Agent_ID, Agent_GUID, Agent_Name, ProcessAgent_Type.Description,
 Domain_Guid, Codebase_URL, WebService_URL
FROM ProcessAgent JOIN ProcessAgent_Type on (ProcessAgent.ProcessAgent_Type_ID= ProcessAgent_Type.ProcessAgent_Type_ID)
WHERE self = 1 AND retired = 0

' 
END
GO
/****** Object:  StoredProcedure [dbo].[WriteSelfProcessAgent]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[WriteSelfProcessAgent]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
Create PROCEDURE [dbo].[WriteSelfProcessAgent] 
@processAgentType varchar(100),
@guid varchar(50),
@domain varchar(50),
@name nvarchar(256),
@codeBaseURL nvarchar(512) = null,
@webServiceURL nvarchar(512)

AS
Declare @processAgentTypeID int 
declare @selfID int

select @selfID=Agent_id from ProcessAgent where self = 1

if @selfID > 0
  begin
    update ProcessAgent set Agent_GUID=@guid, Agent_Name=@name, Codebase_URL=@codeBaseURL, WebService_URL=@webServiceURL,
      Domain_GUID = @domain
    where agent_id = @selfID
    select  @selfID
  end
else
  begin
    select @processAgentTypeID = ( select ProcessAgent_Type_ID  from ProcessAgent_Type where (upper( Description) = upper(@processAgentType )))
    insert into ProcessAgent( self, Agent_GUID, Agent_Name,ProcessAgent_Type_ID, Codebase_URL, WebService_URL, Domain_GUID)
    values (1, @guid, @name, @processAgentTypeID,  @codeBaseURL, @webServiceURL, @domain )
    select ident_current(''ProcessAgent'')
    
end

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetTicketsByCoupon]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketsByCoupon]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetTicketsByCoupon]
	@couponID bigint,
	@issuer varchar(50)

  AS
        DECLARE @ticketTypeID int

        select  Ticket_ID, Ticket_Type.Name, Coupon_ID, Issuer_GUID, Redeemer_GUID, Sponsor_GUID, 
	Creation_Time, duration, Payload, Cancelled
        from Ticket join Ticket_Type on (Ticket.Ticket_Type_ID= Ticket_Type.Ticket_Type_ID)
	where coupon_ID = @couponID and Issuer_GUID = @issuer

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetTicketID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetTicketID] 
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

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetTicket]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicket]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetTicket]
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

' 
END
GO
/****** Object:  StoredProcedure [dbo].[CancelTicket]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CancelTicket]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'--
-- Note: Ticket use in the ProcessAgent may be optional, 
-- Tickets are created by an external ServiceBroker.
--

CREATE PROCEDURE [dbo].[CancelTicket]

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


' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetTicketsByType]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketsByType]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetTicketsByType]
	@ticketType varchar(100)
  AS
        DECLARE @ticketTypeID int
        select @ticketTypeID = (select Ticket_Type_ID  from Ticket_Type where upper( Name ) = upper(@ticketType) )
        select  Ticket_ID, upper(@ticketType), Coupon_ID, Issuer_GUID, Redeemer_GUID, Sponsor_GUID, 
	Creation_Time, duration, Payload, Cancelled
        from Ticket  where Ticket_Type_ID = @ticketTypeID

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetTicketByID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'


CREATE PROCEDURE [dbo].[GetTicketByID]
    @ticketID bigint,
    @issuer varchar (50)
 AS
 	
	select Ticket_ID, Ticket_Type.Name, Coupon_ID, Issuer_GUID, Redeemer_GUID, Sponsor_GUID, 
	Creation_Time, duration, Payload, Cancelled
        from Ticket join Ticket_Type on (Ticket.Ticket_Type_ID= Ticket_Type.Ticket_Type_ID)
        where Ticket_ID = @ticketID  AND Issuer_Guid = @issuer

' 
END
GO
/****** Object:  StoredProcedure [dbo].[RetrieveTicketTypes]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[RetrieveTicketTypes]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[RetrieveTicketTypes]
  AS
        select Ticket_Type_ID, Name, Short_Description, Abstract  from Ticket_Type
' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteTicket]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteTicket]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[DeleteTicket]

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


' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertTicket]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InsertTicket]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
-- Inserting a ticket in the general ticketing tables does not generate the ticket_id,
-- If the ServiceBroker also needs to deal with tickets it has not issued
-- it will have to maintain the full structure.
CREATE PROCEDURE [dbo].[InsertTicket]
	@ticketID bigint,
	@ticketType varchar(100),
	@couponID bigint,
	@issuerGUID varchar(50),
	@redeemerGUID varchar(50),
  	@sponsorGUID varchar(50),
	@creationTime DateTime,
  	@duration bigint,
  	@payload ntext,
  	@cancelled bit=0
   AS
	DECLARE @ticketTypeID int
	select @ticketTypeID = (select Ticket_Type_ID  from Ticket_Type where upper( Name ) =upper(@ticketType) )
	insert into Ticket (Ticket_ID, Ticket_Type_ID, creation_Time, duration, 	payload,cancelled,Coupon_ID,Issuer_GUID, sponsor_GUID, Redeemer_GUID) 
	values (@ticketID, @ticketTypeID,@creationTime,@duration,@payload,@cancelled, @couponID, 	@issuerGUID, @sponsorGUID, @redeemerGUID)

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetTicketIDs]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTicketIDs]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetTicketIDs] 
@couponID  bigint,
@issuerGuid varchar(50)
 AS

select Ticket_ID from Ticket 
where Coupon_ID= @couponID AND Issuer_GUID = @issuerGuid
' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteTickets]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteTickets]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[DeleteTickets]
@guid  varchar(50)

AS

DELETE from Ticket
where redeemer_GUID=@guid or sponsor_GUID = @guid
select @@rowcount
return
' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteIssuerTickets]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteIssuerTickets]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[DeleteIssuerTickets]
@guid  varchar(50)

AS

DELETE from Ticket
where Issuer_GUID=@guid
select @@rowcount
return
' 
END
GO
/****** Object:  StoredProcedure [dbo].[CancelTicketByID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CancelTicketByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[CancelTicketByID]
@ticketID  bigint,
@issuer varchar (50)

AS

update Ticket set Cancelled = 1 
where ticket_ID = @ticketID AND Issuer_Guid = @issuer

select @@rowcount
return


' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteTicketByID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteTicketByID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeleteTicketByID]
@ticketID  bigint,
@issuer varchar (50)
AS

delete from ticket
where ticket_ID = @ticketID  AND Issuer_Guid = @issuer

select @@rowcount
return


' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetCouponCollectionCount]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetCouponCollectionCount]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetCouponCollectionCount]
	@couponID BigInt,
	@guid varchar (50)
AS
	select count(ticket_ID) from Ticket where coupon_ID = @couponID and issuer_guid = @guid
' 
END
GO
/****** Object:  StoredProcedure [dbo].[SetIdentOutCouponID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetIdentOutCouponID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[SetIdentOutCouponID] 
@agentGUID varchar(50),
@id bigint
AS

update ProcessAgent set identOut_ID=@id where agent_Guid = @agentGUID
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentName]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentName]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentName]
@agentID int
 AS
select Agent_Name from ProcessAgent
where agent_ID = @agentID AND retired = 0

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetProcessAgentID]
@guid varchar (50)
 AS

select Agent_ID from ProcessAgent
where Agent_GUID = @guid  AND retired = 0
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentIDsByTypeID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentIDsByTypeID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentIDsByTypeID]
@typeid int
 AS

select Agent_ID from ProcessAgent
where ProcessAgent_type_id = @typeid AND retired = 0



' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentURL]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentURL]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetProcessAgentURL] 

@guid varchar(50)

 AS

select WebService_URL
 from ProcessAgent
 where  Agent_GUID = @guid AND retired = 0


' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentURLbyID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentURLbyID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetProcessAgentURLbyID] 

@agent_id int

 AS

select WebService_URL
 from ProcessAgent
 where  Agent_ID = @agent_id AND retired = 0


' 
END
GO
/****** Object:  StoredProcedure [dbo].[IsProcessAgentRetired]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[IsProcessAgentRetired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[IsProcessAgentRetired]
@guid varchar (50)
AS
declare @status int
Set @status = -1
if Exists (select* from processAgent where Agent_Guid = @guid)
BEGIN
	select @status=retired from processAgent where Agent_Guid = @guid
END
select @status
' 
END
GO
/****** Object:  StoredProcedure [dbo].[IsProcessAgentIDRetired]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[IsProcessAgentIDRetired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[IsProcessAgentIDRetired]
@id int
AS
declare @status int
Set @status = -1
if Exists (select* from processAgent where Agent_ID = @id)
BEGIN
	select @status=retired from processAgent where Agent_id = @id
END
select @status
' 
END
GO
/****** Object:  StoredProcedure [dbo].[SetProcessAgentRetired]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetProcessAgentRetired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[SetProcessAgentRetired]
@guid varchar (50),
@state bit
AS
update ProcessAgent Set retired=@state where Agent_Guid = @guid
select @@rowcount
' 
END
GO
/****** Object:  StoredProcedure [dbo].[RemoveDomainCredentials]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[RemoveDomainCredentials]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[RemoveDomainCredentials]
    @domainGuid varchar(50),
    @agentGuid varchar (50)
 AS
 	
update ProcessAgent set IdentIn_ID= null, IdentOut_ID= null,Issuer_Guid= null,Domain_Guid = null
where Domain_Guid = @domainGuid and Agent_Guid = @agentGuid
select @@rowcount

' 
END
GO
/****** Object:  StoredProcedure [dbo].[RetrieveSystemSupport]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[RetrieveSystemSupport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[RetrieveSystemSupport]
@guid varchar(50)
AS
select agent_GUID, Contact_Email, bug_email, Info_URL, description , location
from ProcessAgent where Agent_Guid=@guid
' 
END
GO
/****** Object:  StoredProcedure [dbo].[RetrieveSystemSupportById]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[RetrieveSystemSupportById]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[RetrieveSystemSupportById]
@id int
AS
select agent_GUID, Contact_Email, Bug_Email, Info_URL, description , location
from ProcessAgent where Agent_id=@id
' 
END
GO
/****** Object:  StoredProcedure [dbo].[SaveSystemSupport]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SaveSystemSupport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[SaveSystemSupport]
@guid varchar(50),
@contactEmail nvarchar(256),
@bugEmail nvarchar(256),
@info nvarchar(512),
@desc ntext,
@location nvarchar(256)
AS

UPDATE ProcessAgent set Contact_Email=@contactEmail,Bug_Email=@bugEmail,Info_Url=@info,Description=@desc, location=@location
WHERE Agent_GUID =@guid
select @@rowcount
' 
END
GO
/****** Object:  StoredProcedure [dbo].[SaveSystemSupportById]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SaveSystemSupportById]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[SaveSystemSupportById]
@id int,
@contactEmail nvarchar(256),
@bugEmail nvarchar(256),
@info nvarchar(512),
@desc ntext,
@location nvarchar(256)
AS

UPDATE PRocessAgent set Contact_Email=@contactEmail,Bug_Email=@bugEmail,Info_Url=@info,Description=@desc, location=@location
WHERE Agent_ID =@id
select @@rowcount
' 
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateDomainGuid]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateDomainGuid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
Create procedure [dbo].[UpdateDomainGuid]
@guid varchar(50)

as
declare @found int

select @found = count(agent_ID) from ProcessAgent where self = 1
if @found > 0
  BEGIN
    Update ProcessAgent set Domain_GUID = @guid where self = 1
    return 1
  END
ELSE
  begin
    return 0
  end

' 
END
GO
/****** Object:  StoredProcedure [dbo].[SetSelfState]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetSelfState]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
Create Procedure [dbo].[SetSelfState]
@guid varchar(50),
@state bit
as
update ProcessAgent set self=@state where Agent_Guid =@guid
select @@rowcount
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetSelfProcessAgentID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetSelfProcessAgentID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
Create Procedure [dbo].[GetSelfProcessAgentID]
as
select agent_id from ProcessAgent
where self = 1 and retired = 0;
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetDomainProcessAgentTags]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetDomainProcessAgentTags]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetDomainProcessAgentTags]
@domain varchar(50)
 AS
select agent_ID, Agent_Name from ProcessAgent
where domain_guid = @domain  AND retired = 0

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagsByTypeID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagsByTypeID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GetProcessAgentTagsByTypeID]

@typeID int

 AS

select agent_ID, Agent_Name from ProcessAgent
 where ProcessAgent_Type_id = @typeID  AND retired = 0

' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTags]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTags]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

--
-- ProcessAgent  the detailed information about a service
--

CREATE PROCEDURE [dbo].[GetProcessAgentTags]
 AS
select agent_ID, Agent_Name from ProcessAgent
where retired = 0
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetProcessAgentTagByGuid]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetProcessAgentTagByGuid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[GetProcessAgentTagByGuid]
@guid varchar(50)
 AS
select agent_ID, Agent_Name from ProcessAgent
where agent_Guid = @guid AND retired = 0

' 
END
GO
/****** Object:  StoredProcedure [dbo].[SetIdentCoupons]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetIdentCoupons]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[SetIdentCoupons] 
@agentGUID varchar(50),
@inId bigint,
@outId bigint,
@issuerGUID varchar(50)
AS

update ProcessAgent set identIn_ID=@inId,identOut_ID=@outId,issuer_Guid= @issuerGuid
where agent_Guid = @agentGUID

' 
END
GO
/****** Object:  StoredProcedure [dbo].[SetIdentInCouponID]    Script Date: 05/11/2011 12:29:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SetIdentInCouponID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
EXEC dbo.sp_executesql @statement = N'


CREATE PROCEDURE [dbo].[SetIdentInCouponID] 
@agentGUID varchar(50),
@id bigint
AS

update ProcessAgent set identIn_ID=@id where agent_Guid = @agentGUID
' 
END
GO
/****** Object:  Default [DF_ProcessAgent_Retired]    Script Date: 05/11/2011 12:29:11 ******/
IF Not EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ProcessAgent_Retired]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProcessAgent] ADD  CONSTRAINT [DF_ProcessAgent_Retired]  DEFAULT (0) FOR [Retired]

END
GO
/****** Object:  Default [DF_ProcessAgent_Self]    Script Date: 05/11/2011 12:29:11 ******/
IF Not EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ProcessAgent_Self]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProcessAgent] ADD  CONSTRAINT [DF_ProcessAgent_Self]  DEFAULT (0) FOR [Self]

END
GO
/****** Object:  ForeignKey [FK_InComingMessages_LabConfiguration]    Script Date: 05/11/2011 12:29:11 ******/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_InComingMessages_LabConfiguration]') AND type = 'F')
ALTER TABLE [dbo].[InComingMessages]  WITH NOCHECK ADD  CONSTRAINT [FK_InComingMessages_LabConfiguration] FOREIGN KEY([LabConfigurationID])
REFERENCES [dbo].[LabConfiguration] ([LabConfigurationID])
GO
ALTER TABLE [dbo].[InComingMessages] CHECK CONSTRAINT [FK_InComingMessages_LabConfiguration]
GO
/****** Object:  ForeignKey [FK_OutGoingMessages_LabConfiguration]    Script Date: 05/11/2011 12:29:11 ******/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_OutGoingMessages_LabConfiguration]') AND type = 'F')
ALTER TABLE [dbo].[OutGoingMessages]  WITH NOCHECK ADD  CONSTRAINT [FK_OutGoingMessages_LabConfiguration] FOREIGN KEY([LabConfigurationID])
REFERENCES [dbo].[LabConfiguration] ([LabConfigurationID])
GO
ALTER TABLE [dbo].[OutGoingMessages] CHECK CONSTRAINT [FK_OutGoingMessages_LabConfiguration]
GO
/****** Object:  ForeignKey [FK_ProcessAgent_ProcessAgent_Type]    Script Date: 05/11/2011 12:29:11 ******/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_ProcessAgent_ProcessAgent_Type]') AND type = 'F')
ALTER TABLE [dbo].[ProcessAgent]  WITH CHECK ADD  CONSTRAINT [FK_ProcessAgent_ProcessAgent_Type] FOREIGN KEY([ProcessAgent_Type_ID])
REFERENCES [dbo].[ProcessAgent_Type] ([ProcessAgent_Type_ID])
GO
ALTER TABLE [dbo].[ProcessAgent] CHECK CONSTRAINT [FK_ProcessAgent_ProcessAgent_Type]
GO
/****** Object:  ForeignKey [FK_Ticket_Coupon]    Script Date: 05/11/2011 12:29:11 ******/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Ticket_Coupon]') AND type = 'F')
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Coupon] FOREIGN KEY([Coupon_ID], [Issuer_GUID])
REFERENCES [dbo].[Coupon] ([Coupon_ID], [Issuer_GUID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_Coupon]
GO
/****** Object:  ForeignKey [FK_Ticket_Ticket_Type]    Script Date: 05/11/2011 12:29:11 ******/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Ticket_Ticket_Type]') AND type = 'F')
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Ticket_Type] FOREIGN KEY([Ticket_Type_ID])
REFERENCES [dbo].[Ticket_Type] ([Ticket_Type_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_Ticket_Type]
GO
