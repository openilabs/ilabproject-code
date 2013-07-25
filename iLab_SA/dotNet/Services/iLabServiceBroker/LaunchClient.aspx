<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LaunchClient.aspx.cs" Inherits="iLabs.ServiceBroker.iLabSB.LaunchClient" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Launch Client</title>
    <script type="text/javascript" language="javascript" >
        function setTZ(){
         var obj = hdnUserTZ;
         var visitortime = new Date();
         if(visitortime) {
            document.getElementById(obj).value = -visitortime.getTimezoneOffset();
         }
         else {
             document.getElementById(obj).value = "JavaScript not Date() enabled";
         }
        }
    </script>
</head>
<body onload="setTZ()">
    <form id="form1" runat="server">
    <asp:HiddenField ID="hdnUserTZ" runat="server" />
    <div>
    
    </div>
    </form>
</body>
</html>
