<%@ Page Language="C#" MasterPageFile="~/MasterPagePopupWindow.master" AutoEventWireup="true" CodeFile="PopupLabSetup.aspx.cs" Inherits="PopupLabSetup" Title="New Configuration" %>



<asp:Content ID="Content4" runat="server" 
    contentplaceholderid="PopupWindowLabels">
    <p> &nbsp;</p>
    
    
    <p> <asp:Label ID="LabName" runat="server" Text="Lab Name"></asp:Label> </p>
   <p> &nbsp;</p>
   <p> &nbsp;</p>
   <p>  <asp:Label ID="LabDescriptionID" runat="server" Text="Lab Description"></asp:Label> </p>
   
    <p> &nbsp;</p>
    
    <p> <asp:Label ID="ExperimentGroupID" runat="server" Text="Experiment Group Name"></asp:Label> </p>
    <p> <asp:Label ID="ServiceBrokerGuid" runat="server" Text="Service Broker GUID"></asp:Label> </p>
    <p> <asp:Label ID="InPasskeyID" runat="server" Text="In Passkey ID"></asp:Label> </p>
    <p> <asp:Label ID="OutPasskeyID" runat="server" Text="Out Passkey ID"></asp:Label> </p>
    <p> <asp:Label ID="InPasskeyName" runat="server" Text="In-Passkey Name"></asp:Label> </p>
    <p> <asp:Label ID="OutPasskeyName" runat="server" Text="Out-Passkey Name"></asp:Label> </p>
    <p> <asp:Label ID="ServiceBrokerURL" runat="server" Text="Service Broker URL"></asp:Label> </p>                           
    <p> <asp:Label ID="ClientName" runat="server" Text="Client Name"></asp:Label> </p>
    <p> <asp:Label ID="ClientGuid" runat="server" Text="Client GUID"></asp:Label> </p>
    <p> <asp:Label ID="LabServerName" runat="server" Text="Lab Server Name"></asp:Label> </p>
    <p> <asp:Label ID="LabServerGuid" runat="server" Text="Lab Server GUID"></asp:Label> </p>
    <p> <asp:Label ID="InstantMessageGuid" runat="server" Text="Instant Message GUID"></asp:Label> </p>
    <p> <asp:Label ID="MinimumLabDuration" runat="server" Text="Minimum Lab Duration"></asp:Label> </p>
    <p> <asp:Label ID="MaximumLabDuration" runat="server" Text="Maximum Lab Duration"></asp:Label> </p>
</asp:Content>
<asp:Content ID="Content5" runat="server" 
    contentplaceholderid="PopUpWindowTextBoxes">
    <p> &nbsp;</p>
    

    
                                    <p><asp:TextBox ID="TxBLabName" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    
                                    
                                    
                                    <p><asp:TextBox ID="TxBLabDescription" runat="server" Height="110px" Width="471px" 
                                            TextMode="MultiLine" style="margin-left: 0px; margin-top: 13px"></asp:TextBox></p>
                                        
                                            
                                    <p><asp:TextBox ID="TxBExptGroup" runat="server" Width="469px" 
                                            Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBSbGuid" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBInPasskey" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBOutPasskey" runat="server" Width="469px" Height="14px"></asp:TextBox></p>    
                                    <p><asp:TextBox ID="TxBInPasskeyName" runat="server" Width="469px" Height="14px"></asp:TextBox></p>    
                                    <p><asp:TextBox ID="TxBOutPasskeyName" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBSbUrl" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBClientName" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBClientGuid" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBLsName" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBLsGuid" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBMessageGuid" runat="server" Width="469px" 
                                            Height="14px"></asp:TextBox></p>
                                    <p><asp:TextBox ID="TxBmaxLabDurn" runat="server" Width="469px" Height="14px"></asp:TextBox></p> 
                                    <p><asp:TextBox ID="TxBminLabDurn" runat="server" Width="469px" Height="14px"></asp:TextBox></p>
                                    <p style="margin-left: 160px">
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="CreateBtn" runat="server" Text="Create Instant Schedule" 
                                            onclick="CreateBtn_Click" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:Button ID="ClosePopupButton" runat="server" Text="Close" 
                                            onclick="ClosePopupButton_Click" />
                                    </p>
                                    </asp:Content>





<asp:Content ID="Content6" runat="server" 
    contentplaceholderid="ContentPlaceHolder3">
    <p class="style26">
        This page configures the Instant schedule application to support SMS scheduling 
        of&nbsp; Interactive Laboratories</p>
    <p class="style26">
        <asp:Label ID="Confirmation" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>&nbsp;</p>
</asp:Content>






<asp:Content ID="Content7" runat="server" contentplaceholderid="head">

    <style type="text/css">
        .style26
        {
            width: 662px;
            margin-left: 40px;
        }
    </style>

</asp:Content>







