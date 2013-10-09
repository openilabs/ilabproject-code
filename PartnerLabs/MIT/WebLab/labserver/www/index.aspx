<%@ Page Language="VBscript"%>

<!-- 
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->

<html>
	<head>
		<title>MIT Microelectronics Weblab</title></head>
	<frameset cols="225,*" frameborder="0" framespacing="0" framepadding="0" border="0">
		<!--<frame src="left.html?offset=37" scrolling=no noresize marginwidth=0 marginheight=0>-->
		<frameset rows="205,*" frameborder="0" framespacing="0" framepadding="0" border="0" marginheight="0" marginwidth="0">
			<frame src="logo.html?offset=7" scrolling="no" noresize marginheight="0" marginwidth="0">
			<frameset cols="41,183,*" frameborder="0" framespacing="0" framepadding="0" border="0" marginheight="0" marginwidth="0">
				<frame src="menu_border.html?offset=41" scrolling="no" noresize>
				<frame src="login.aspx?offset=0" name="menu">
				<frame src="menu_border.html?offset=0" scrolling="no" noresize>
			</frameset>
		</frameset>
		<frameset rows="135,*" frameborder="0" framespacing="0" framepadding="0" border="0">
			<frame src="top.html" scrolling="no" noresize>
			<frameset cols="610,65,*" frameborder="0" framespacing="0" framepadding="0" border="0">
				<frameset rows="*,39,1" frameborder="0" framespacing="0" framepadding="0" border="0"><!--20-->
					<frame src="main.aspx" name="main" noresize>
					<frame src="bottom.aspx" scrolling="no" noresize>
					<frame src="load_area_default.aspx" name="loadArea" scrolling="no" noresize>
				</frameset>
				<frame src="decoration_vertical.html" scrolling="no" noresize>
				<frame src="empty.html?offset=0" scrolling="no" noresize>
			</frameset>
		</frameset>
	</frameset>
</html>