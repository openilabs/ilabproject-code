<%@ Register TagPrefix="uc1" TagName="footer" Src="footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="userNav" Src="userNav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="banner" Src="banner.ascx" %>
<%@ Page language="c#" Inherits="iLabs.ServiceBroker.iLabSB.showExperiment" validateRequest="false" CodeFile="showExperiment.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
	<head>
		<title>MIT iLab Service Broker - Show Experiment</title> 
		<!-- 
Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
		<!-- $Id: showExperiment.aspx,v 1.5 2008/04/07 19:06:23 pbailey Exp $ -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
		<style type="text/css">@import url( css/main.css ); 
		</style>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<a name="top"></a>
			<div id="outerwrapper">
				<uc1:banner id="Banner1" runat="server"></uc1:banner>
				<uc1:userNav id="UserNav1" runat="server"></uc1:userNav>
				<br clear="all" />
				<div id="innerwrapper">
					<div id="pageintro">
						<h1>Show Experiment</h1>
						<p>View your experiment records.
						</p>
						<asp:label id="lblResponse" Runat="server" Visible="False"></asp:label>
					</div>
					<!-- end pageintro div -->
					<div id="pagecontent">
							<h4>Selected Experiment</h4>
							<div class="simpleform">
								<table class="button">
									<tbody>
										<tr>
											<th>
												<label for="experimentid">Experiment ID</label>
											</th>
											<td style="width: 100px" colspan="3">
                                                <asp:textbox id="txtExperimentID" Runat="server" ReadOnly="True" Width="100px"></asp:textbox>
                                            </td>
										</tr>
										<tr>
											<th>
												<label for="username">User Name </label>
											</th>
											<td style="width: 206px"><asp:textbox id="txtUsername"  Runat="server" ReadOnly="True" Width="200px"></asp:textbox>
											</td>
											<th style="HEIGHT: 23px">
												<label for="groupname">Effective Group Name </label>
											</th>
											<td style="HEIGHT: 23px; width: 206px;"><asp:textbox id="txtGroupName" Runat="server" ReadOnly="True" Width="200px"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="labservername">Lab Server Name</label></th>
											<td style="width: 206px"><asp:textbox id="txtLabServerName" Runat="server" ReadOnly="True" Width="200px"></asp:textbox>
											</td>
											<th>
												<label for="labclientname">Lab Client Name</label></th>
											<td style="width: 206px"><asp:textbox id="txtClientName" Runat="server" ReadOnly="True" Width="200px"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="subtime">Submission Time </label>
											</th>
											<td style="width: 206px"><asp:textbox id="txtSubmissionTime" Runat="server" ReadOnly="True" Width="200px"></asp:textbox>
											</td>
											<th>
												<label for="comtime">Completion Time </label>
											</th>
											<td style="width: 206px"><asp:textbox id="txtCompletionTime" Runat="server" ReadOnly="True" Width="200px"></asp:textbox>
											</td>
										</tr>
										<tr>
										    <th>
												<label for="recordCount">Total Records </label>
											</th>
											<td style="width: 206px"><asp:textbox id="txtRecordCount" Runat="server" ReadOnly="True" Width="200px"></asp:textbox>
											</td>
											<th>
												<label for="status">Status</label>
											</th>
											<td style="width: 206px"><asp:textbox id="txtStatus" Runat="server" ReadOnly="True" Width="200px"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="annotation">Annotation</label></th>
											<td colspan="3"><asp:textbox id="txtAnnotation" Runat="server"  Rows="5" TextMode="MultiLine" Width="532px"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<asp:button id="btnSaveAnnotation" Runat="server" Text="Save Annotation" CssClass="buttonleft" onclick="btnSaveAnnotation_Click"></asp:button>
											</th>
										
											<th>
												<asp:button id="btnDeleteExperiment" Runat="server" Text="Delete Experiment" CssClass="buttonright" onclick="btnDeleteExperiment_Click"></asp:button>
											</th>
										</tr>
									</tbody>
								</table>
							</div>
							<div class="simpleform" id="divSelect"	runat="server">
							<asp:CheckBox id="cbxContents" runat="server" Text="Data Only" > </asp:CheckBox>
							<asp:button id="btnDisplayRecords" Runat="server" Text="Get Records" CssClass="buttonleft" onclick="btnDisplayRecords_Click"></asp:button>
							</div>
						<div id="divRecords" runat="server" >
						<p>&nbsp;</p>
							<h4>Experiment Records</h4>
							<asp:textbox id="txtExperimentRecords" Runat="server" Width="700px" Height="156px" TextMode="MultiLine"></asp:textbox>
							<asp:GridView ID="grvExperimentRecords" runat="server" Width="700px" CellPadding="5" AutoGenerateColumns="False" 
							    HeaderStyle-Font-Bold="true" >
                                <Columns>
                                    <asp:BoundField DataField="Seq_Num" HeaderText="Seq_Num" ReadOnly="True" >
                                        <HeaderStyle Font-Bold="True" HorizontalAlign="center" Width="80px" Wrap="False" />
                                        <ItemStyle HorizontalAlign="Right" Wrap="False" />   
                                    </asp:BoundField>                                
                                    <asp:BoundField DataField="Record Type" HeaderText="Record Type" ReadOnly="True" >
                                        <HeaderStyle Font-Bold="True" HorizontalAlign="center" Width="200px" />
                                        <ItemStyle HorizontalAlign="Left" />  
                                    </asp:BoundField>                    
                                    <asp:BoundField DataField="Contents" HeaderText="Data" ReadOnly="True" >
                                        <HeaderStyle Font-Bold="True" HorizontalAlign="left" Width="420px" Wrap="True" />
                                        <ItemStyle HorizontalAlign="Left" />                                       
                                    </asp:BoundField>                    
                                </Columns>     
                            </asp:GridView>
						</div>
						
						<div id="divBlobs" runat="server" visible="false">
						<p>&nbsp;</p>
						<h4>Experiment BLOBS</h4>
							<asp:GridView ID="grvBlobs" runat="server" CellPadding="5" Width="700px" AutoGenerateColumns="False" 
							     HeaderStyle-Font-Bold="True" OnRowDataBound="On_BindBlobRow"  OnRowCommand="On_BlobSelected"  
							      HeaderStyle-HorizontalAlign="Center">
							<Columns>
                                <asp:TemplateField HeaderText="Select"  HeaderStyle-HorizontalAlign="Center">
                                    <HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="80px" Wrap="False" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Button ID="Button1" runat="server" CausesValidation="false" CommandName=""  CommandArgument='<%# Eval("Blob_ID") %>' Text='<%# Eval("Blob_ID") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Seq_Num" HeaderText="Seq_Num" ReadOnly="True" >
                                        <HeaderStyle Font-Bold="True" HorizontalAlign="center" Width="80px" Wrap="False" />
                                        <ItemStyle HorizontalAlign="Right" Wrap="False" />   
                                    </asp:BoundField>                                
                                    <asp:BoundField DataField="MimeType" HeaderText="MimeType" ReadOnly="True" >
                                        <HeaderStyle Font-Bold="True" HorizontalAlign="center" Width="200px" />
                                        <ItemStyle HorizontalAlign="Left" />  
                                    </asp:BoundField>                    
                                    <asp:BoundField DataField="Description" HeaderText="Description" ReadOnly="True" >
                                        <HeaderStyle Font-Bold="True" HorizontalAlign="left" Width="440px" Wrap="True" />
                                        <ItemStyle HorizontalAlign="Left" Wrap="true" />                                       
                                    </asp:BoundField>             
                                </Columns>
                                <HeaderStyle Font-Bold="True" />
							</asp:GridView>
						</div>
						<p>&nbsp;</p>
					
					<br clear="all" />
					</div><!-- end pagecontent div -->
					</div><!-- end innerwrapper div -->
				<uc1:footer id="Footer1" runat="server"></uc1:footer>
			</div><!-- end outerwrapper div -->
		</form>	
	</body>
</html>
