<?xml version='1.0'?>
<Project Type="Project" LVVersion="8208000">
   <Item Name="My Computer" Type="My Computer">
      <Property Name="server.app.propertiesEnabled" Type="Bool">true</Property>
      <Property Name="server.control.propertiesEnabled" Type="Bool">true</Property>
      <Property Name="server.tcp.enabled" Type="Bool">false</Property>
      <Property Name="server.tcp.port" Type="Int">0</Property>
      <Property Name="server.tcp.serviceName" Type="Str">My Computer/VI Server</Property>
      <Property Name="server.tcp.serviceName.default" Type="Str">My Computer/VI Server</Property>
      <Property Name="server.vi.callsEnabled" Type="Bool">true</Property>
      <Property Name="server.vi.propertiesEnabled" Type="Bool">true</Property>
      <Property Name="specify.custom.address" Type="Bool">false</Property>
      <Item Name="elvisv1_2.vi" Type="VI" URL="elvisv1_2.vi"/>
      <Item Name="NI ELVIS - Close Wrap.vi" Type="VI" URL="NI ELVIS - Close Wrap.vi"/>
      <Item Name="FGEN-SCOPE.vi" Type="VI" URL="FGEN-SCOPE.vi"/>
      <Item Name="NI ELVIS - Initialize Wrap.vi" Type="VI" URL="NI ELVIS - Initialize Wrap.vi"/>
      <Item Name="Dependencies" Type="Dependencies"/>
      <Item Name="Build Specifications" Type="Build">
         <Item Name="elvisv1.2" Type="DLL">
            <Property Name=".NET" Type="Bool">false</Property>
            <Property Name="Absolute[0]" Type="Bool">false</Property>
            <Property Name="Absolute[1]" Type="Bool">false</Property>
            <Property Name="Absolute[2]" Type="Bool">false</Property>
            <Property Name="AliasID" Type="Str">{599CC4DD-329E-4491-9C7C-99B9608A0C8E}</Property>
            <Property Name="AliasName" Type="Str">Project.aliases</Property>
            <Property Name="AutoIncrement" Type="Bool">false</Property>
            <Property Name="BuildName" Type="Str">elvisv1.2</Property>
            <Property Name="CopyErrors" Type="Bool">false</Property>
            <Property Name="DebuggingDLL" Type="Bool">false</Property>
            <Property Name="DebugServerWaitOnLaunch" Type="Bool">true</Property>
            <Property Name="DefaultLanguage" Type="Str">English</Property>
            <Property Name="DelayOSMsg" Type="Bool">true</Property>
            <Property Name="DependencyApplyDestination" Type="Bool">true</Property>
            <Property Name="DependencyApplyInclusion" Type="Bool">true</Property>
            <Property Name="DependencyApplyProperties" Type="Bool">true</Property>
            <Property Name="DependencyFolderDestination" Type="Int">0</Property>
            <Property Name="DependencyFolderInclusion" Type="Str">As Needed</Property>
            <Property Name="DependencyFolderPropertiesItemCount" Type="Int">0</Property>
            <Property Name="DestinationID[0]" Type="Str">{24EDC973-A1E7-477A-B3B6-6095C462B6FC}</Property>
            <Property Name="DestinationID[1]" Type="Str">{24EDC973-A1E7-477A-B3B6-6095C462B6FC}</Property>
            <Property Name="DestinationID[2]" Type="Str">{8A8FDB74-7AE3-4870-B7D6-1C5C515A9E89}</Property>
            <Property Name="DestinationItemCount" Type="Int">3</Property>
            <Property Name="DestinationName[0]" Type="Str">elvisv1_2.dll</Property>
            <Property Name="DestinationName[1]" Type="Str">Destination Directory</Property>
            <Property Name="DestinationName[2]" Type="Str">Support Directory</Property>
            <Property Name="Disconnect" Type="Bool">true</Property>
            <Property Name="EmbeddedRTE" Type="Bool">false</Property>
            <Property Name="HID" Type="Str">{31C3D7E8-1BE8-4E66-9EC2-0C46E9F8E51C}</Property>
            <Property Name="IncludeHeaders" Type="Bool">true</Property>
            <Property Name="IncludeHWConfig" Type="Bool">false</Property>
            <Property Name="IncludeSCC" Type="Bool">true</Property>
            <Property Name="INIID" Type="Str">{E0FF7050-3F9D-4BB2-AA12-D702C820BC44}</Property>
            <Property Name="LIBID" Type="Str">{F45EE7C7-6C23-4D38-9880-B0D14891A436}</Property>
            <Property Name="ManagedClassName" Type="Str"></Property>
            <Property Name="ManagedClassNamespace" Type="Str"></Property>
            <Property Name="MathScript" Type="Bool">false</Property>
            <Property Name="Path[0]" Type="Path">../build/internal.llb</Property>
            <Property Name="Path[1]" Type="Path">../build</Property>
            <Property Name="Path[2]" Type="Path">../build/data</Property>
            <Property Name="SharedLibraryID" Type="Str">{661E56DF-56FF-4BF3-A442-F11C0BE4D87F}</Property>
            <Property Name="SharedLibraryName" Type="Str">elvisv1_2.dll</Property>
            <Property Name="ShowHWConfig" Type="Bool">false</Property>
            <Property Name="SourceInfoItemCount" Type="Int">4</Property>
            <Property Name="SourceItem[0].FolderInclusion" Type="Str">Exported VI</Property>
            <Property Name="SourceItem[0].Inclusion" Type="Str">Exported VI</Property>
            <Property Name="SourceItem[0].ItemID" Type="Ref">/My Computer/elvisv1_2.vi</Property>
            <Property Name="SourceItem[0].VIPropertiesItemCount" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIPropertiesSettingOption[0]" Type="Str">Remove panel</Property>
            <Property Name="SourceItem[0].VIPropertiesVISetting[0]" Type="Bool">false</Property>
            <Property Name="SourceItem[0].VIProtoInfo[0]VIProtoDir" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[0]VIProtoInputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[0]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[0]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[0]VIProtoName" Type="Str">return value</Property>
            <Property Name="SourceItem[0].VIProtoInfo[0]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[0]VIProtoPassBy" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[1]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[1]VIProtoInputIdx" Type="Int">18</Property>
            <Property Name="SourceItem[0].VIProtoInfo[1]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[1]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[1]VIProtoName" Type="Str">SCOPET_Slope</Property>
            <Property Name="SourceItem[0].VIProtoInfo[1]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[1]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[10]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[10]VIProtoInputIdx" Type="Int">15</Property>
            <Property Name="SourceItem[0].VIProtoInfo[10]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[10]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[10]VIProtoName" Type="Str">SCOPEChB_Offset</Property>
            <Property Name="SourceItem[0].VIProtoInfo[10]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[10]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[11]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[11]VIProtoInputIdx" Type="Int">11</Property>
            <Property Name="SourceItem[0].VIProtoInfo[11]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[11]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[11]VIProtoName" Type="Str">SCOPEChB_Coupling</Property>
            <Property Name="SourceItem[0].VIProtoInfo[11]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[11]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[12]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[12]VIProtoInputIdx" Type="Int">6</Property>
            <Property Name="SourceItem[0].VIProtoInfo[12]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[12]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[12]VIProtoName" Type="Str">SCOPEH_RecordLength</Property>
            <Property Name="SourceItem[0].VIProtoInfo[12]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[12]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[13]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[13]VIProtoInputIdx" Type="Int">5</Property>
            <Property Name="SourceItem[0].VIProtoInfo[13]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[13]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[13]VIProtoName" Type="Str">SCOPEH_SampleRateHz</Property>
            <Property Name="SourceItem[0].VIProtoInfo[13]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[13]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[14]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[14]VIProtoInputIdx" Type="Int">7</Property>
            <Property Name="SourceItem[0].VIProtoInfo[14]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[14]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[14]VIProtoName" Type="Str">SCOPEH_Acquire</Property>
            <Property Name="SourceItem[0].VIProtoInfo[14]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[14]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[15]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[15]VIProtoInputIdx" Type="Int">4</Property>
            <Property Name="SourceItem[0].VIProtoInfo[15]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[15]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[15]VIProtoName" Type="Str">FGEN_WaveformType</Property>
            <Property Name="SourceItem[0].VIProtoInfo[15]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[15]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[16]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[16]VIProtoInputIdx" Type="Int">3</Property>
            <Property Name="SourceItem[0].VIProtoInfo[16]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[16]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[16]VIProtoName" Type="Str">FGEN_Frequency</Property>
            <Property Name="SourceItem[0].VIProtoInfo[16]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[16]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[17]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[17]VIProtoInputIdx" Type="Int">2</Property>
            <Property Name="SourceItem[0].VIProtoInfo[17]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[17]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[17]VIProtoName" Type="Str">FGEN_DCOffset</Property>
            <Property Name="SourceItem[0].VIProtoInfo[17]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[17]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[18]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[18]VIProtoInputIdx" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[18]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[18]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[18]VIProtoName" Type="Str">FGEN_Amplitude</Property>
            <Property Name="SourceItem[0].VIProtoInfo[18]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[18]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[19]VIProtoDir" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[19]VIProtoInputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[19]VIProtoLenInput" Type="Int">20</Property>
            <Property Name="SourceItem[0].VIProtoInfo[19]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[19]VIProtoName" Type="Str">interleavedArray</Property>
            <Property Name="SourceItem[0].VIProtoInfo[19]VIProtoOutputIdx" Type="Int">22</Property>
            <Property Name="SourceItem[0].VIProtoInfo[19]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[2]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[2]VIProtoInputIdx" Type="Int">17</Property>
            <Property Name="SourceItem[0].VIProtoInfo[2]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[2]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[2]VIProtoName" Type="Str">SCOPET_TriggerType</Property>
            <Property Name="SourceItem[0].VIProtoInfo[2]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[2]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[20]CallingConv" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[20]Name" Type="Str">runExperiment</Property>
            <Property Name="SourceItem[0].VIProtoInfo[20]VIProtoDir" Type="Int">3</Property>
            <Property Name="SourceItem[0].VIProtoInfo[20]VIProtoInputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[20]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[20]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[20]VIProtoName" Type="Str">len</Property>
            <Property Name="SourceItem[0].VIProtoInfo[20]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[20]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[3]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[3]VIProtoInputIdx" Type="Int">16</Property>
            <Property Name="SourceItem[0].VIProtoInfo[3]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[3]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[3]VIProtoName" Type="Str">SCOPE_TriggerSource</Property>
            <Property Name="SourceItem[0].VIProtoInfo[3]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[3]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[4]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[4]VIProtoInputIdx" Type="Int">8</Property>
            <Property Name="SourceItem[0].VIProtoInfo[4]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[4]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[4]VIProtoName" Type="Str">SCOPEChA_Source</Property>
            <Property Name="SourceItem[0].VIProtoInfo[4]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[4]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[5]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[5]VIProtoInputIdx" Type="Int">12</Property>
            <Property Name="SourceItem[0].VIProtoInfo[5]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[5]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[5]VIProtoName" Type="Str">SCOPEChA_Range</Property>
            <Property Name="SourceItem[0].VIProtoInfo[5]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[5]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[6]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[6]VIProtoInputIdx" Type="Int">14</Property>
            <Property Name="SourceItem[0].VIProtoInfo[6]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[6]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[6]VIProtoName" Type="Str">SCOPEChA_Offset</Property>
            <Property Name="SourceItem[0].VIProtoInfo[6]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[6]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[7]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[7]VIProtoInputIdx" Type="Int">10</Property>
            <Property Name="SourceItem[0].VIProtoInfo[7]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[7]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[7]VIProtoName" Type="Str">SCOPEChA_Coupling</Property>
            <Property Name="SourceItem[0].VIProtoInfo[7]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[7]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[8]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[8]VIProtoInputIdx" Type="Int">9</Property>
            <Property Name="SourceItem[0].VIProtoInfo[8]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[8]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[8]VIProtoName" Type="Str">SCOPEChB_Source</Property>
            <Property Name="SourceItem[0].VIProtoInfo[8]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[8]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[9]VIProtoDir" Type="Int">0</Property>
            <Property Name="SourceItem[0].VIProtoInfo[9]VIProtoInputIdx" Type="Int">13</Property>
            <Property Name="SourceItem[0].VIProtoInfo[9]VIProtoLenInput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[9]VIProtoLenOutput" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[9]VIProtoName" Type="Str">SCOPEChB_Range</Property>
            <Property Name="SourceItem[0].VIProtoInfo[9]VIProtoOutputIdx" Type="Int">-1</Property>
            <Property Name="SourceItem[0].VIProtoInfo[9]VIProtoPassBy" Type="Int">1</Property>
            <Property Name="SourceItem[0].VIProtoInfoCPTM" Type="Bin">###!!A!!!"=!&amp;E!X`````Q!-#U2F&gt;GFD:3"/97VF!!Z!#AF"&lt;8"M;82V:'5!$E!+#52$)%^G:H.F&gt;!!/1!I*2H*F=86F&lt;G.Z!#R!&amp;A!$"&amp;.J&lt;G5+6(*J97ZH&gt;7RB=A:4=86B=G5!$6&gt;B&gt;G6G&lt;X*N)&amp;2Z='5!&amp;E!+%&amp;.B&lt;8"M:3"3982F)#B)?CE!!"*!!QV3:7.P=G1A4'6O:X2I!!R!"A&gt;"9X&amp;V;8*F!$]!]1!!!!!!!!!")E6-6EF4)&amp;.$4V"&amp;)#UA1WBB&lt;GZF&lt;#"")&amp;.P&gt;8*D:3ZD&gt;'Q!%U!'#E.I16^4&lt;X6S9W5!!$]!]1!!!!!!!!!")E6-6EF4)&amp;.$4V"&amp;)#UA1WBB&lt;GZF&lt;#"#)&amp;.P&gt;8*D:3ZD&gt;'Q!%U!'#E.I1F^4&lt;X6S9W5!!"*!"AR$;%%A1W^V='RJ&lt;G=!!"*!"AV$;%*@)%.P&gt;8"M;7ZH!!Z!#AF$;%%A5G&amp;O:W5!$E!+#5.I1C"397ZH:1!11!I+1WB")%^G:H.F&gt;!!!%%!+#E.I1C"0:G:T:81!!"2!"AZ5=GFH:W6S)&amp;.P&gt;8*D:1!!%E!'$&amp;2S;7&gt;H:8)A6(FQ:1!!#E!$"6.M&lt;X"F!!1!!!!-1!I(:7RF&lt;76O&gt;!!?1%!!!@````]!&amp;"&amp;J&lt;H2F=GRF98:F:#"B=H*B?1"]!0!!(!!!!!%!!A!$!!1!"1!'!!=!#!!*!!I!#Q!-!!U!$A!0!"!!%1!3!"-!%Q!4!"5!%Q!4!"-!%Q!4!Q!"'!I!#!!)!!A!#!!)!!A!#!!)!!A!#!!)!!A!#!!)!!A!#!!)!!A!!!!!!!!!#3!!!!!!!!!!!!!!!!!!!1!7</Property>
            <Property Name="SourceItem[0].VIProtoInfoVIProtoItemCount" Type="Int">21</Property>
            <Property Name="SourceItem[1].FolderInclusion" Type="Str">Exported VI</Property>
            <Property Name="SourceItem[1].Inclusion" Type="Str">Always Included</Property>
            <Property Name="SourceItem[1].ItemID" Type="Ref">/My Computer/NI ELVIS - Close Wrap.vi</Property>
            <Property Name="SourceItem[1].VIPropertiesItemCount" Type="Int">1</Property>
            <Property Name="SourceItem[1].VIPropertiesSettingOption[0]" Type="Str">Remove panel</Property>
            <Property Name="SourceItem[1].VIPropertiesVISetting[0]" Type="Bool">false</Property>
            <Property Name="SourceItem[2].FolderInclusion" Type="Str">Exported VI</Property>
            <Property Name="SourceItem[2].Inclusion" Type="Str">Always Included</Property>
            <Property Name="SourceItem[2].ItemID" Type="Ref">/My Computer/FGEN-SCOPE.vi</Property>
            <Property Name="SourceItem[2].VIPropertiesItemCount" Type="Int">1</Property>
            <Property Name="SourceItem[2].VIPropertiesSettingOption[0]" Type="Str">Remove panel</Property>
            <Property Name="SourceItem[2].VIPropertiesVISetting[0]" Type="Bool">false</Property>
            <Property Name="SourceItem[3].FolderInclusion" Type="Str">Exported VI</Property>
            <Property Name="SourceItem[3].Inclusion" Type="Str">Always Included</Property>
            <Property Name="SourceItem[3].ItemID" Type="Ref">/My Computer/NI ELVIS - Initialize Wrap.vi</Property>
            <Property Name="StripLib" Type="Bool">true</Property>
            <Property Name="SupportedLanguageCount" Type="Int">0</Property>
            <Property Name="UseFFRTE" Type="Bool">false</Property>
            <Property Name="VersionInfoCompanyName" Type="Str">MIT</Property>
            <Property Name="VersionInfoFileDescription" Type="Str"></Property>
            <Property Name="VersionInfoFileType" Type="Int">2</Property>
            <Property Name="VersionInfoFileVersionBuild" Type="Int">0</Property>
            <Property Name="VersionInfoFileVersionMajor" Type="Int">1</Property>
            <Property Name="VersionInfoFileVersionMinor" Type="Int">0</Property>
            <Property Name="VersionInfoFileVersionPatch" Type="Int">0</Property>
            <Property Name="VersionInfoInternalName" Type="Str">MIT-ELVISv1.2</Property>
            <Property Name="VersionInfoLegalCopyright" Type="Str">Copyright © 2007 MIT</Property>
            <Property Name="VersionInfoProductName" Type="Str">MIT-ELVISv1.2</Property>
         </Item>
      </Item>
   </Item>
</Project>
