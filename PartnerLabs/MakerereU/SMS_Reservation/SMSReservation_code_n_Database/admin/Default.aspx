<%@ Page Language="C#" MasterPageFile="~/admin/masterPages/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
       
    .style12
    {
        font-size: x-large;
        font-weight: bold;
    }
    .style13
    {
        font-size: large;
        font-weight: bold;
    }
    .styleButton
    {
        background-color: #006699;
        font-weight:bold;
        color:Black;
        border-color: #0099CC;
        height: 28px;
        width: 175px;
        azimuth:left-side;
        margin-left: 591px;
        margin-top: 0px;}
    
        #Submit1
        {
            width: 61px;
            margin-left: 44px;
            margin-bottom: 0px;
            height: 23px;
        }
        #Submit2
        {
            height: 23px;
            margin-left: 682px;
            margin-top: 4px;
            width: 69px;
        }
    </style>
    
<script language="javascript" type="text/javascript">
// <!CDATA[

function Submit2_onclick() {

}

// ]]>
</script>
</asp:Content>


<asp:Content ID="Content2" runat="server" 
    contentplaceholderid="ContentPlaceHolder1"> 
<div id ="lab configuration" style="border-style: none; border-color: #CCCCCC; height: 276px; width: 906px; background-color: #CCCCFF; 
            visibility: visible; clear: both;"> 
         <h2>  <b> Instant Message Configuration </b> </h2>   
<style= " font-size: 18px; width: 790px; padding-left: 2px;text-align: justify; height: 75px; color: #000066;"> 
    <p> This Page displays the available configurations that the Instant Message 
    application is holding.</p>
 <p> </p>

                                <br />
                                <br />

<asp:Button ID="NewLabConfiguration" CssClass="styleButton" runat="server" 
        onclick="NewLabConfiguration_Click" Text="New Configuration" ForeColor="White" 
                Height="28px" Width="252px"/> </div>
<p style="text-align: right; height: 19px;"> </p> 
    </div>
    
    <div id ="lab label"  style="border-style: none; border-color: #0000FF; vertical-align: baseline; text-align: justify; background-color: #CCCCFF; width: 906px; height: 255px; color: #000066;">
        <br />
 LabName&nbsp;&nbsp;&nbsp; 
   <asp:DropDownList id="drop1" runat="server" 
   Width="274px" style="margin-left: 512px; margin-top: 10px; height: 25px;" 
   onselectedindexchanged="drop1_SelectedIndexChanged" Height="19px" >
<asp:ListItem>Lab 1</asp:ListItem>
<asp:ListItem>Lab 2</asp:ListItem>
<asp:ListItem>Lab 3</asp:ListItem>
<asp:ListItem>Lab 4</asp:ListItem>
<asp:ListItem>Lab 5</asp:ListItem>
<asp:ListItem>Lab 6</asp:ListItem>
</asp:DropDownList> 
 
&nbsp;</h3>
<h4>Description:</h4>
<p><asp:Label ID="LbClientDescription" runat="server" ></asp:Label></p>
<input type="submit" id="Submit2" value="Edit" onclick="return Submit2_onclick()" />&nbsp; 
<input type="submit" id="Submit1" value="Confirm" onclick="return Submit2_onclick()"/>
 </div>
    </asp:Content>



