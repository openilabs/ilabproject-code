<%@ Control Language="c#" Inherits="iLabs.LabServer.LabView.LVRemotePanel" CodeFile="LVRemotePanel.ascx.cs" %>
<div>
<!-- LVRemotePanel -->
<OBJECT ID="LabVIEWControl" BORDER=<% =border %>
    CLASSID="<% =classId %>"
    CODEBASE="<% =codebase %>"
    WIDTH="<% =width %>" HEIGHT="<% =height %>" >
    <PARAM name="server" value="<% =serverURL %>" />
	<PARAM name="LVFPPVINAME" value="<% =viName %>" />
	<param name="REQCTRL" value="<% =hasControl %>" /> 
	<EMBED SRC="<% =serverURL  %>/.LV_FrontPanelProtocol.rpvi82" LVFPPVINAME="<% =viName %>" 
	REQCTRL="<% =hasControl %>"  WIDTH=<% =width %> HEIGHT=<% =height %> TYPE="application/x-labviewrpvi82" 
	PLUGINSPAGE="http://digital.ni.com/express.nsf/express?openagent&code=ex3e33&">
	 </EMBED> 
</OBJECT> 
</div>