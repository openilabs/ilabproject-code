<?xml version='1.0' encoding='UTF-8'?>
<Project Type="Project" LVVersion="12008004">
	<Item Name="My Computer" Type="My Computer">
		<Property Name="IOScan.Faults" Type="Str"></Property>
		<Property Name="IOScan.NetVarPeriod" Type="UInt">100</Property>
		<Property Name="IOScan.NetWatchdogEnabled" Type="Bool">false</Property>
		<Property Name="IOScan.Period" Type="UInt">10000</Property>
		<Property Name="IOScan.PowerupMode" Type="UInt">0</Property>
		<Property Name="IOScan.Priority" Type="UInt">9</Property>
		<Property Name="IOScan.ReportModeConflict" Type="Bool">true</Property>
		<Property Name="IOScan.StartEngineOnDeploy" Type="Bool">false</Property>
		<Property Name="server.app.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.control.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.tcp.enabled" Type="Bool">false</Property>
		<Property Name="server.tcp.port" Type="Int">0</Property>
		<Property Name="server.tcp.serviceName" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.tcp.serviceName.default" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.vi.callsEnabled" Type="Bool">true</Property>
		<Property Name="server.vi.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="specify.custom.address" Type="Bool">false</Property>
		<Item Name="ILABW_AppStatus.vi" Type="VI" URL="../../ILABW_AppStatus.vi"/>
		<Item Name="ILABW_CaseHandler.vi" Type="VI" URL="../../ILABW_CaseHandler.vi"/>
		<Item Name="ILABW_CaseHandlerNoPath.vi" Type="VI" URL="../../ILABW_CaseHandlerNoPath.vi"/>
		<Item Name="ILABW_CreateFromTemplate.vi" Type="VI" URL="../../ILABW_CreateFromTemplate.vi"/>
		<Item Name="ILABW_DisplayStatus.vi" Type="VI" URL="../../ILABW_DisplayStatus.vi"/>
		<Item Name="ILABW_ErrorToString.vi" Type="VI" URL="../../ILABW_ErrorToString.vi"/>
		<Item Name="ILABW_GetPath.vi" Type="VI" URL="../../ILABW_GetPath.vi"/>
		<Item Name="ILABW_GetVI.vi" Type="VI" URL="../../ILABW_GetVI.vi"/>
		<Item Name="ILABW_IsLoaded.vi" Type="VI" URL="../../ILABW_IsLoaded.vi"/>
		<Item Name="ILABW_RemoteCommandMgr.vi" Type="VI" URL="../../ILABW_RemoteCommandMgr.vi"/>
		<Item Name="ILABW_SetBounds.vi" Type="VI" URL="../../ILABW_SetBounds.vi"/>
		<Item Name="ILABW_ViStatus.vi" Type="VI" URL="../../ILABW_ViStatus.vi"/>
		<Item Name="Dependencies" Type="Dependencies">
			<Item Name="vi.lib" Type="Folder">
				<Item Name="BuildHelpPath.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/BuildHelpPath.vi"/>
				<Item Name="Check Special Tags.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Check Special Tags.vi"/>
				<Item Name="Clear Errors.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Clear Errors.vi"/>
				<Item Name="Convert property node font to graphics font.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Convert property node font to graphics font.vi"/>
				<Item Name="Details Display Dialog.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Details Display Dialog.vi"/>
				<Item Name="DialogType.ctl" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/DialogType.ctl"/>
				<Item Name="DialogTypeEnum.ctl" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/DialogTypeEnum.ctl"/>
				<Item Name="Error Code Database.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Error Code Database.vi"/>
				<Item Name="ErrWarn.ctl" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/ErrWarn.ctl"/>
				<Item Name="eventvkey.ctl" Type="VI" URL="/&lt;vilib&gt;/event_ctls.llb/eventvkey.ctl"/>
				<Item Name="Find Tag.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Find Tag.vi"/>
				<Item Name="Format Message String.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Format Message String.vi"/>
				<Item Name="General Error Handler CORE.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/General Error Handler CORE.vi"/>
				<Item Name="General Error Handler.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/General Error Handler.vi"/>
				<Item Name="Get String Text Bounds.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Get String Text Bounds.vi"/>
				<Item Name="Get Text Rect.vi" Type="VI" URL="/&lt;vilib&gt;/picture/picture.llb/Get Text Rect.vi"/>
				<Item Name="GetHelpDir.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/GetHelpDir.vi"/>
				<Item Name="GetRTHostConnectedProp.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/GetRTHostConnectedProp.vi"/>
				<Item Name="Longest Line Length in Pixels.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Longest Line Length in Pixels.vi"/>
				<Item Name="LVBoundsTypeDef.ctl" Type="VI" URL="/&lt;vilib&gt;/Utility/miscctls.llb/LVBoundsTypeDef.ctl"/>
				<Item Name="Not Found Dialog.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Not Found Dialog.vi"/>
				<Item Name="Search and Replace Pattern.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Search and Replace Pattern.vi"/>
				<Item Name="Set Bold Text.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Set Bold Text.vi"/>
				<Item Name="Set String Value.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Set String Value.vi"/>
				<Item Name="Simple Error Handler.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Simple Error Handler.vi"/>
				<Item Name="TagReturnType.ctl" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/TagReturnType.ctl"/>
				<Item Name="Three Button Dialog CORE.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Three Button Dialog CORE.vi"/>
				<Item Name="Three Button Dialog.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Three Button Dialog.vi"/>
				<Item Name="Trim Whitespace.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Trim Whitespace.vi"/>
				<Item Name="whitespace.ctl" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/whitespace.ctl"/>
			</Item>
			<Item Name="ILABs_CreateFromTemplate.vi" Type="VI" URL="../../ILABs_CreateFromTemplate.vi"/>
			<Item Name="ILABs_IsLoaded.vi" Type="VI" URL="../../ILABs_IsLoaded.vi"/>
			<Item Name="ILABs_SetBounds.vi" Type="VI" URL="../../ILABs_SetBounds.vi"/>
		</Item>
		<Item Name="Build Specifications" Type="Build">
			<Item Name="My Installer" Type="Installer">
				<Property Name="Destination[0].name" Type="Str">ILabW_REST_Loader</Property>
				<Property Name="Destination[0].parent" Type="Str">{3912416A-D2E5-411B-AFEE-B63654D690C0}</Property>
				<Property Name="Destination[0].tag" Type="Str">{A761572A-746D-49DF-95D4-2B9BF8EC7A5B}</Property>
				<Property Name="Destination[0].type" Type="Str">userFolder</Property>
				<Property Name="DestinationCount" Type="Int">1</Property>
				<Property Name="DistPart[0].flavorID" Type="Str">DefaultFull</Property>
				<Property Name="DistPart[0].productID" Type="Str">{01C0F5DE-BF22-43B9-B7D9-7915B32F71F1}</Property>
				<Property Name="DistPart[0].productName" Type="Str">NI LabVIEW Run-Time Engine 2012 f3</Property>
				<Property Name="DistPart[0].upgradeCode" Type="Str">{20385C41-50B1-4416-AC2A-F7D6423A9BD6}</Property>
				<Property Name="DistPartCount" Type="Int">1</Property>
				<Property Name="INST_author" Type="Str">MIT</Property>
				<Property Name="INST_autoIncrement" Type="Bool">true</Property>
				<Property Name="INST_buildLocation" Type="Path">../ILabW_Deployer/Install WebService</Property>
				<Property Name="INST_buildLocation.type" Type="Str">relativeToCommon</Property>
				<Property Name="INST_buildSpecName" Type="Str">My Installer</Property>
				<Property Name="INST_defaultDir" Type="Str">{A761572A-746D-49DF-95D4-2B9BF8EC7A5B}</Property>
				<Property Name="INST_productName" Type="Str">ILabW_REST_Loader</Property>
				<Property Name="INST_productVersion" Type="Str">1.0.5</Property>
				<Property Name="InstSpecBitness" Type="Str">32-bit</Property>
				<Property Name="InstSpecVersion" Type="Str">12008029</Property>
				<Property Name="MSI_arpCompany" Type="Str">MIT iLab Project</Property>
				<Property Name="MSI_arpURL" Type="Str">https://wikis.mit.edu/confluence/display/ILAB2/Home</Property>
				<Property Name="MSI_distID" Type="Str">{1C7E917B-A392-4885-A363-C95E4FFDA1C4}</Property>
				<Property Name="MSI_osCheck" Type="Int">0</Property>
				<Property Name="MSI_upgradeCode" Type="Str">{F391963A-097E-4DCC-8201-E3103EBB6BCC}</Property>
				<Property Name="RegDest[0].dirName" Type="Str">Software</Property>
				<Property Name="RegDest[0].dirTag" Type="Str">{DDFAFC8B-E728-4AC8-96DE-B920EBB97A86}</Property>
				<Property Name="RegDest[0].parentTag" Type="Str">2</Property>
				<Property Name="RegDestCount" Type="Int">1</Property>
				<Property Name="Source[0].dest" Type="Str">{A761572A-746D-49DF-95D4-2B9BF8EC7A5B}</Property>
				<Property Name="Source[0].File[0].dest" Type="Str">{A761572A-746D-49DF-95D4-2B9BF8EC7A5B}</Property>
				<Property Name="Source[0].File[0].name" Type="Str">ILab_WebService.lvws</Property>
				<Property Name="Source[0].File[0].tag" Type="Str">{1D1A1E23-5E2B-4D9A-BA7D-734DD278F00D}</Property>
				<Property Name="Source[0].FileCount" Type="Int">1</Property>
				<Property Name="Source[0].name" Type="Str">My Web Service</Property>
				<Property Name="Source[0].tag" Type="Ref">/My Computer/Build Specifications/My Web Service</Property>
				<Property Name="Source[0].type" Type="Str">RestfulWS</Property>
				<Property Name="SourceCount" Type="Int">1</Property>
			</Item>
			<Item Name="My Web Service" Type="RESTful WS">
				<Property Name="Bld_buildCacheID" Type="Str">{F7A1EC3F-A6B3-4EC5-9E59-621B6D21666C}</Property>
				<Property Name="Bld_buildSpecName" Type="Str">My Web Service</Property>
				<Property Name="Bld_excludeInlineSubVIs" Type="Bool">true</Property>
				<Property Name="Bld_excludeLibraryItems" Type="Bool">true</Property>
				<Property Name="Bld_excludePolymorphicVIs" Type="Bool">true</Property>
				<Property Name="Bld_localDestDir" Type="Path">../My Web Service</Property>
				<Property Name="Bld_localDestDirType" Type="Str">relativeToProject</Property>
				<Property Name="Bld_modifyLibraryFile" Type="Bool">true</Property>
				<Property Name="Bld_previewCacheID" Type="Str">{58DFAA69-9BD6-4BBA-A303-DB9600E30DFC}</Property>
				<Property Name="Destination[0].destName" Type="Str">ILab_WebService.lvws</Property>
				<Property Name="Destination[0].path" Type="Path">../My Web Service/internal.llb</Property>
				<Property Name="Destination[0].path.type" Type="Str">relativeToProject</Property>
				<Property Name="Destination[0].preserveHierarchy" Type="Bool">true</Property>
				<Property Name="Destination[0].type" Type="Str">App</Property>
				<Property Name="Destination[1].destName" Type="Str">Support Directory</Property>
				<Property Name="Destination[1].path" Type="Path">../My Web Service/data</Property>
				<Property Name="Destination[1].path.type" Type="Str">relativeToProject</Property>
				<Property Name="DestinationCount" Type="Int">2</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[0].template" Type="Str">/ILABW_CaseHandler/:data/:action</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[0].VIName" Type="Str">ILABW_CaseHandler.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[1].template" Type="Str">/ILABW_CaseHandlerNoPath/:data/:action</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[1].VIName" Type="Str">ILABW_CaseHandlerNoPath.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[10].template" Type="Str">/ILABW_DisplayStatus</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[10].VIName" Type="Str">ILABW_DisplayStatus.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[2].template" Type="Str">/ILABW_CreateFromTemplate/:path/:suffix/:templateName</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[2].VIName" Type="Str">ILABW_CreateFromTemplate.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[3].template" Type="Str">/ILABW_GetVI/:path</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[3].VIName" Type="Str">ILABW_GetVI.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[4].template" Type="Str">/ILABW_IsLoaded/:name</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[4].VIName" Type="Str">ILABW_IsLoaded.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[5].template" Type="Str">/ILABW_RemoteCommandMgr/:action/:data</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[5].VIName" Type="Str">ILABW_RemoteCommandMgr.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[6].template" Type="Str">/ILABW_SetBounds/:left/:right/:bottom/:top/:viname</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[6].VIName" Type="Str">ILABW_SetBounds.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[7].template" Type="Str">/ILABW_ViStatus/:name</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[7].VIName" Type="Str">ILABW_ViStatus.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[8].template" Type="Str">/ILABW_GetPath/:name</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[8].VIName" Type="Str">ILABW_GetPath.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[9].template" Type="Str">/ILABW_AppStatus</Property>
				<Property Name="RESTfulWebSrvc_routingTemplate[9].VIName" Type="Str">ILABW_AppStatus.vi</Property>
				<Property Name="RESTfulWebSrvc_routingTemplateCount" Type="Int">11</Property>
				<Property Name="Source[0].itemID" Type="Str">{2558FC7B-A2FC-47CF-AB9B-D9737E2C0C63}</Property>
				<Property Name="Source[0].type" Type="Str">Container</Property>
				<Property Name="Source[1].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[1].itemID" Type="Ref">/My Computer/ILABW_CaseHandler.vi</Property>
				<Property Name="Source[1].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[1].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[10].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[10].itemID" Type="Ref">/My Computer/ILABW_AppStatus.vi</Property>
				<Property Name="Source[10].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[10].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[11].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[11].itemID" Type="Ref">/My Computer/ILABW_DisplayStatus.vi</Property>
				<Property Name="Source[11].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[11].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[2].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[2].itemID" Type="Ref">/My Computer/ILABW_CaseHandlerNoPath.vi</Property>
				<Property Name="Source[2].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[2].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[3].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[3].itemID" Type="Ref">/My Computer/ILABW_CreateFromTemplate.vi</Property>
				<Property Name="Source[3].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[3].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[4].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[4].itemID" Type="Ref">/My Computer/ILABW_GetVI.vi</Property>
				<Property Name="Source[4].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[4].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[5].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[5].itemID" Type="Ref">/My Computer/ILABW_IsLoaded.vi</Property>
				<Property Name="Source[5].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[5].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[6].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[6].itemID" Type="Ref">/My Computer/ILABW_RemoteCommandMgr.vi</Property>
				<Property Name="Source[6].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[6].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[7].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[7].itemID" Type="Ref">/My Computer/ILABW_SetBounds.vi</Property>
				<Property Name="Source[7].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[7].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[8].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[8].itemID" Type="Ref">/My Computer/ILABW_ViStatus.vi</Property>
				<Property Name="Source[8].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[8].type" Type="Str">RESTfulVI</Property>
				<Property Name="Source[9].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[9].itemID" Type="Ref">/My Computer/ILABW_GetPath.vi</Property>
				<Property Name="Source[9].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[9].type" Type="Str">RESTfulVI</Property>
				<Property Name="SourceCount" Type="Int">12</Property>
				<Property Name="TgtF_companyName" Type="Str">MIT</Property>
				<Property Name="TgtF_fileDescription" Type="Str">My Web Service</Property>
				<Property Name="TgtF_internalName" Type="Str">My Web Service</Property>
				<Property Name="TgtF_legalCopyright" Type="Str">Copyright © 2013 MIT</Property>
				<Property Name="TgtF_productName" Type="Str">My Web Service</Property>
				<Property Name="TgtF_targetfileGUID" Type="Str">{1D1A1E23-5E2B-4D9A-BA7D-734DD278F00D}</Property>
				<Property Name="TgtF_targetfileName" Type="Str">ILab_WebService.lvws</Property>
				<Property Name="WebSrvc_standaloneService" Type="Bool">true</Property>
			</Item>
		</Item>
	</Item>
</Project>
