<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

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
        
        .style14
        {
        	background: #F6FFFF;
        	
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
        }
        
       

    
    </style>
</asp:Content>

<asp:Content ID="Content2" runat="server" 
    contentplaceholderid="ContentPlaceHolder1">
    <p class="style13">
        &nbsp;<h2>Instatnt Message Configuration</h2></p>
<p>
    This Page displays the available configurations that the Instant Message 
    application is holding.</p>
    <p>
        &nbsp;</p>
<p style="text-align: right">
    
    <asp:Button ID="NewLabConfiguration" CssClass="styleButton" runat="server" 
        onclick="NewLabConfiguration_Click" Text="New Configuration"  />
    &nbsp;</p>
    <div class ="style14">
    <h3>
    Lab Name:<asp:Label ID="LabelClientName" runat="server" ></asp:Label>
    </h3>
        <h4>Description:</h4>
        <p> 
            <asp:Label ID="LbClientDescription" runat="server" ></asp:Label>
        </p>
        <p style="text-align: right"> 
            
    </div>

</asp:Content>


