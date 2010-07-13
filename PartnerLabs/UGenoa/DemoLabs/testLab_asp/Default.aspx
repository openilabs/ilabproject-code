<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Laboratory </title>
</head>
<body> 
    <form id="form1" runat="server">
    <div>
    <h1>This is a simple Testpage which shows you the data received from the webservice:</h1>
    <br />
    <p>This is the result retrieved from the proxyWS:</p>
    <p><b>Reservation Expiration:   </b></p>
    <asp:Label ID="ReservationExpirationLbl" runat="server"></asp:Label><br />
    <p><b>Experiment ID:            </b></p>
    <asp:Label ID="ExperimentIdLbl" runat="server"> </asp:Label><br />
    <p><b>Group ID:                 </b></p>
    <asp:Label ID="GroupIdLbl" runat="server"></asp:Label><br />
    <br />
    <p>The call to the method "LabProxyClient.isResgistrationValid()" shows you if the current session is valid</p>
    <b>
    <asp:Label ID="isvalid" runat="server"></asp:Label><br />
    </b>
    <br />
    <a href="javascript:location.reload(true)">Refresh page</a>
    <br />
    <br />
    <asp:HyperLink ID="HyperLinkSB" runat="server" Font-Bold="True"><< Back To SB</asp:HyperLink>
    </div>
    </form>
</body>
</html>
