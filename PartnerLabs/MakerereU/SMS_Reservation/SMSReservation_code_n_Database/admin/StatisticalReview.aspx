<%@ Page Language="C#" MasterPageFile="~/admin/masterPages/MasterPage.master" AutoEventWireup="true" CodeFile="StatisticalReview.aspx.cs" Inherits="StatisticalReview" Title="Statistical Review" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
    .style12
    {}
    .style13
    {
        font-weight: bold;
        font-size: x-large;
    }
</style>
<script language="javascript" type="text/javascript">
// <!CDATA[

function check1_onclick() {

}

// ]]>
</script>
</asp:Content>

<asp:Content ID="Content2" runat="server" 
    contentplaceholderid="ContentPlaceHolder1">
    <p class="style13">Statistical Review</p>
<p class="style12">
   This page displays how the Instant Scheduling application has been used by students.</p>
<p class="style12">
    &nbsp;</p>
    <div style="width: 936px; background-color: #CCCCFF; padding-top: 50px"> 
    
    <div style="float:left"><asp:ListBox ID="lstUsers" runat="server" Height="313px" Width="303px"
        onselectedindexchanged="lstUsers_SelectedIndexChanged" AutoPostBack="true">
    </asp:ListBox></div>
 <b></b>
<div style="border-color: #333333; border-style: solid; width: 442px; height: 359px; margin-left: 475px; ">  
<h3> Selected User </h3> 
<b></b>
Username: <asp:TextBox id="txtUserName" runat="server" style="margin-left: 51px"/>
<br /><br />
Labname: <asp:TextBox id="txtLabName" runat="server" style="margin-left: 57px" />
<br /><br />
Date:    <asp:TextBox id="txtDate" runat="server" style="margin-left: 85px" />
<br /><br />
StartTime range:<asp:TextBox id="txtStartTime" runat="server" 
        style="margin-left: 18px" />
<br /><br />
EndTime range : <asp:TextBox id="txtEndTime" runat="server" 
        style="margin-left: 19px" />
<br /><br />
MessageKey : <asp:TextBox id="txtMsgkey" runat="server" 
        style="margin-left: 29px" />  
        </div>      
<br /><br /> 

<p> </p>
<div style="border-color: #333333; border-style: solid; width: 429px; height: 213px; margin-left: 483px"> 
 <h3>Message from system</h3>

GivenStartTime: <asp:TextBox id="txtGivenStartTime" runat="server" 
        style="margin-left: 37px" Width="129px" />
<br /><br />
GivenEndTime: <asp:TextBox id="txtGiveEndTime" runat="server" style="margin-left: 42px" />
<br /><br />
TimeAndDate:    <asp:TextBox id="txtTimeAndDate" runat="server" 
        style="margin-left: 45px" />
</div>
    
<br />
        <br />
  &nbsp;
 </div>
     
</asp:Content>


