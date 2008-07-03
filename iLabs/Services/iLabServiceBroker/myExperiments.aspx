<%@ Register TagPrefix="uc1" TagName="footer" Src="footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="userNav" Src="userNav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="banner" Src="banner.ascx" %>
<%@ Page language="c#" Inherits="iLabs.ServiceBroker.iLabSB.myExperiments" validateRequest="false" CodeFile="myExperiments.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
	<head>
		<title>MIT iLab Service Broker - My Experiments</title> 
		<!-- 
Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
		<!-- $Id: myExperiments.aspx,v 1.9 2008/04/11 19:52:49 pbailey Exp $ -->
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
						<h1>My Experiments</h1>
						<p>View your experiment records by entering a time range and then selecting an 
							experiment below.
						</p>
						<asp:label id="lblResponse" Runat="server" Visible="False"></asp:label>
					</div>
					<!-- end pageintro div -->
					<div id="pagecontent">
					<table cols="2" >
					<tr>
					    <td valign="top">
						<div class="simpleform">
							<table cols="4" width="430px">
								<tbody>
									<tr>
										<th>
											<label for="timeis">Time </label>
										</th>
										<td colspan="3"><asp:dropdownlist id="ddlTimeAttribute" Runat="server" Width="128px" AutoPostBack="True" onselectedindexchanged="ddlTimeAttribute_SelectedIndexChanged">
												<asp:ListItem Value="Select before">-- Select One --</asp:ListItem>
												<asp:ListItem Value="before">before</asp:ListItem>
												<asp:ListItem Value="after">after</asp:ListItem>
												<asp:ListItem Value="between">between</asp:ListItem>
												<asp:ListItem Value="on date">on date</asp:ListItem>
											</asp:dropdownlist>
										</td>
									</tr>
									<tr>
										<th>
											&nbsp;</th>
										<td style="width: 310px"><asp:textbox id="txtTime1" Runat="server" width="160px"></asp:textbox>
										</td>
										<!-- the following field uses the class "noneditable" if the user does not select between from the drop-down list -->
										<td style="width: 225px"><asp:textbox id="txtTime2" Runat="server" width="160px" Enabled="False"></asp:textbox>
										</td>
										<td>&nbsp;</td>
									</tr>
									<tr>
										<th>&nbsp;</th>
										<td colspan="3"><asp:button id="btnGo" Runat="server" Text="Search Experiments" CssClass="button" onclick="btnGo_Click"></asp:button>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
						<div class="simpleform">
							<label for="selectexperiment">Select Experiment</label><br />
							<asp:listbox id="lbxSelectExperiment" Runat="server" Width="430px" Height="156px" AutoPostBack="True" onselectedindexchanged="lbxSelectExperiment_SelectedIndexChanged"></asp:listbox>
						</div>
						</td>
						<td valign="top">
						<div id="itemdisplay" >
							<h4 >Experiment&nbsp;Summary</h4>
							<div class="simpleform">
								<table class="button">
									<tbody>
										<tr id="trExperimentID" runat="server" visible="false">
											<th>
												<label for="experimentid">Experiment ID</label></th>
											<td style="width: 206px">
                                                &nbsp;<asp:textbox id="txtExperimentID" Runat="server" ReadOnly="True" Width="191px"></asp:textbox></td>
										</tr>
										<tr>
											<th>
												<label for="labclientname">Lab Client Name</label></th>
											<td style="width: 206px"><asp:textbox id="txtClientName" Runat="server" ReadOnly="True" Width="197px"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="labservername">Lab Server Name</label></th>
											<td style="width: 206px"><asp:textbox id="txtLabServerName" Runat="server" ReadOnly="True" Width="197px"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="username">User Name </label>
											</th>
											<td style="width: 206px"><asp:textbox id="txtUsername" Runat="server" ReadOnly="True" Width="198px"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th style="HEIGHT: 23px">
												<label for="groupname">Effective Group Name </label>
											</th>
											<td style="HEIGHT: 23px; width: 206px;"><asp:textbox id="txtGroupName" Runat="server" ReadOnly="True" Width="197px"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="status">Status</label></th>
											<td style="width: 206px"><asp:textbox id="txtStatus" Runat="server" ReadOnly="True"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="subtime">Submission Time </label>
											</th>
											<td style="width: 206px"><asp:textbox id="txtSubmissionTime" Runat="server" ReadOnly="True"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="comtime">Completion Time </label>
											</th>
											<td style="width: 206px"><asp:textbox id="txtCompletionTime" Runat="server" ReadOnly="True"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="recordCount">Total Records </label>
											</th>
											<td style="width: 206px"><asp:textbox id="txtRecordCount" Runat="server" ReadOnly="True"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th>
												<label for="annotation">Annotation</label></th>
											<td style="width: 206px"><asp:textbox id="txtAnnotation" Runat="server" Columns="20" Rows="5" TextMode="MultiLine"></asp:textbox>
											</td>
										</tr>
										<tr>
											<th colspan="2">
												<asp:button id="btnSaveAnnotation" Runat="server" Text="Save Annotation" CssClass="buttonright" onclick="btnSaveAnnotation_Click"></asp:button>
											</th>
										</tr>
										<tr id="trShowExperiment" runat="server" visible="false">
											<th colspan="2">
												<asp:button id="btnShowExperiment" Runat="server" Text="Display Experiment Data" Enabled="false" CssClass="buttonright" onclick="btnShowExperiment_Click"></asp:button>
											</th>
										</tr>
										<tr id="trDeleteExperiment" runat="server" visible="false">
											<th colspan="2">
												<asp:button id="btnDeleteExperiment" Runat="server" Text="Delete Experiment" CssClass="buttonright" onclick="btnDeleteExperiment_Click"></asp:button>
											</th>
										</tr>
									</tbody>
								</table>
							</div>
						</div>
						</td>
					</tr>
					</table>
						<p>&nbsp;</p>
					</div><!-- end pagecontent div -->					
					<br clear="all" />
					</div>
				<!-- end innerwrapper div -->
				<uc1:footer id="Footer1" runat="server"></uc1:footer>
			</div>
		</form>
		
	</body>
</html>
