<%@ language="VBSCRIPT"%>
<!--
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->

<%
	const HP4155B_GPIB_BUS_ADDR = "17"
	const E5250A_GPIB_BUS_ADDR = "22"
	const VISA_NAME = "GPIB0"
	Dim startTime 'start time of measurement request (in msec)
	Dim endTime   'likewise for end time
	
	' open database connection
	Set Users = Server.CreateObject("ADODB.Connection")
	Users.Open "DSN=WebLab"

	
	' check whether this is only a test execution
	if request("test") = "Test" Then
		Application.Lock
		returnTest
		Application.Unlock
		
		
	' validate the query string - if it is correct, then run the test
	elseif validateQueryString = "TRUE" then
        
		'print out query string variables for testing
		'CALL printQueryVars()
		
		Application.Lock
		
		deviceNum = Request("deviceNumber")

		' make sure an appropriate device number has been chosen
		'Response.write("Here is the device number you chose : " & deviceNum & "<br>")

		if (deviceNum = "") then
			deviceNum =  0
		end if

		Set matrix = Server.CreateObject("MATRIX.Matrix_Session")
    	CALL deviceConnect(deviceNum)
		set hp4155 = Server.CreateObject("HP4155.HP4155_Session") 
		Set hp34970 = Server.CreateObject("Therm34970.Therm34970_Session")
		'connectTherm							'connects to the datalogger
		'hp34970.SetUnits("K")						'sets units to Kelvin
		CALL runTest(Users)							'runs both hp4155 qurey and datalogger scan request
		'disconnectTherm							'disconnects datalogger
		end if 


		' set the switching matrix back to the default device 0 (disabled for now)
		if deviceNum <> 0 then
				'Response.write("<p>Switching matrix back to default Device 0....<br>")
			Set matrix = Server.CreateObject("MATRIX.Matrix_Session")
   			CALL deviceConnect(0)
		end if
	    
		Application.Unlock	

		' close the database connection
		Users.close

'***************************************************************************
'*********** BEGINNING OF FUNCTIONS AND SUBROUTINES ***********************
'***************************************************************************

' print out all variables passed in by query string
Sub printQueryVars()
	
	Response.write("Here are all the form variables included in your HTTP request: <br>")
	
	count = Request.QueryString.Count
	Response.write("We had a total of  " &  Request.QueryString.Count & " form variables included.<p>")
	Response.write("<p><table border=1")
	Response.write("<tr><th>Variable Name <th>Variable Value")
	for i = 1 to count 
		Response.write("<tr><td>" & Request.QueryString.Key(i) & "<td>" & Request.QueryString.Item(i))
	next

	Response.write("</table>")

	Response.write("----------------------------------------------------------------------------------------<p><p>")
end Sub


Sub connectTherm()
	const VISAADDRESS = 9		'GPIB address of datalogger
	const CHANNEL = 101			'thermocouple address
	
	connect = hp34970.connect(VISAADDRESS, CHANNEL)
	if connect <> "SUCCESS" then
		error_message connect, "Try again.  If this problem persists, contact " & application("system administrator")& " and report this error"
		exit sub
	end if 
end sub

Sub disconnectTherm()
	hp34970.ClosePort
End Sub
	
Function getTemp(Users)
	getTemp = hp34970.GetTemp()
End Function

Function getUnits(Users)
	getUnits = hp34970.GetUnits()
End Function

sub deviceConnect(D)	
	' debug info
	'Response.write("Trying to connect to device number : " & D & "<br>")

	matrixConnect = matrix.connect(VISA_NAME, E5250A_GPIB_BUS_ADDR) 'connects to the switching matrix
	matrix.DeviceChoose(D)  'Selects device number D
	matrix.waitUntilDone
	matrix.close_session
end sub

sub runTest(Users)
	' create a connection to the HP4155
   	connect = hp4155.connect(VISA_NAME, HP4155B_GPIB_BUS_ADDR) 'connect to the HP4155
	if connect <> "SUCCESS" then
		error_message connect,"Try again.  If this problem persists, contact " & application("system administrator")& " and report this error"
		exit sub
	end if
	

	hp4155.setTimeOut 3000

	' set the mode to "SWEEP" or "SAMP"
	'if hp4155.setMode(Request.QueryString.Item("mode")) <> "SUCCESS" then
	'	error_message "Bad Mode","Go back and try again.  Make sure that the correct mode is selected.  If this problem persists, contact " & application("system administrator")
	'	exit sub
	'end if 

	' first delete extraneous settings for SMUs alredy present on the device
	CALL sendCommandWithFeedback(":PAGE:CHAN:ALL:DIS", "deletion of all previous channel settings")

	CALL sendCommandWithFeedback(":PAGE:CHAN:UFUN:DEL:ALL", "deletion of all functions")
	'Response.write("Disabled all previous SMU settings w/ result : " & result & ".<br>")


	

	'Response.write("Started operation at: " & startTime & "<br>")

	' set measurement wait time multiplier
	'waitTime = Request.QueryString.Item("waitTime")

	
	' set up the channels
	' Addition - ADH 5/31/05 : add the high-power SMU
	SMU5 = setSMU(hp4155, 5):   if (SMU5 = "FAILURE") then exit sub

	SMU1 = setSMU(hp4155, 1):	if (SMU1 = "FAILURE") then exit sub		
	SMU2 = setSMU(hp4155, 2):	if (SMU2 = "FAILURE") then exit sub
	SMU3 = setSMU(hp4155, 3):	if (SMU3 = "FAILURE") then exit sub
	SMU4 = setSMU(hp4155, 4):	if (SMU4 = "FAILURE") then exit sub
	

	
	'VSU1 = setVSU(hp4155, 1):	if (VSU1 = "FAILURE") then exit sub
	'VSU2 = setVSU(hp4155, 2):	if (VSU2 = "FAILURE") then exit sub
	'VMU1 = setVMU(hp4155, 1):	if (VMU1 = "FAILURE") then exit sub
	'VMU2 = setVMU(hp4155, 2):	if (VMU2 = "FAILURE") then exit sub

	' setup user-defined functions
	i=0
	uf = Request.Form("UserFunction0")
	do while uf <> ""
		'result = hp4155.command(":PAGE:CHAN:UFUN:DEF " + uf)
		if result <> "SUCCESS" then
			error_message "User Function " & i & " error. "&result, "Change User Function "&i&" and try again.  (Remove any non-alphanumeric symbols from the name and units)."
			exit sub
		end if
		i = i + 1
		uf = Request.Form("UserFunction"&i)
	loop

	'Response.Write("<br><b>Setting measurement specs for all SMUs " & number & "</b><br>")

	'setup the constants, VAR1, and VAR2
	SMU1 = setFnct(hp4155, SMU1): if (SMU1 = "FAILURE") then exit sub
	SMU2 = setFnct(hp4155, SMU2): if (SMU2 = "FAILURE") then exit sub
	SMU3 = setFnct(hp4155, SMU3): if (SMU3 = "FAILURE") then exit sub
	SMU4 = setFnct(hp4155, SMU4): if (SMU4 = "FAILURE") then exit sub
	SMU5 = setFnct(hp4155, SMU5): if (SMU5 = "FAILURE") then exit sub

	' VSU1 = setFnct(hp4155, VSU1): if (VSU1 = "FAILURE") then exit sub
	' VSU2 = setFnct(hp4155, VSU2): if (VSU2 = "FAILURE") then exit sub


	' MEASUREMENT SETUP

	waitTime = Request.QueryString.Item("waittime")
	CALL sendCommandWithFeedback(":PAGE:MEAS:MSET:WTIM " & waittime, "wait time")

	integTime = Request.QueryString.Item("integtime")
	CALL sendCommandWithFeedback(":PAGE:MEAS:MSET:ITIM:SHORT " & integTime & "E-5", "SHORT integration time")


	' debug info
	'Response.write("Here are the variables you have set to download : ' " & list & "<br>")
	
	' SEND USER-DEFINED FUNCTIONS


	Dim userfunctions(4)

	userfunctions(0) = Request.QueryString.Item("userfunc1")
	userfunctions(1) = Request.QueryString.Item("userfunc2")
	userfunctions(2) = Request.QueryString.Item("userfunc3")
	userfunctions(3) = Request.QueryString.Item("userfunc4")

	for count = 0 to 3
		if (userfunctions(count) <> "") then
			CALL sendCommandWithFeedback(":PAGE:CHAN:UFUN:DEF func" & count & ",'V'," & userfunctions(count), _ 
				"user function " & (count + 1))
		end if		
	next
	
	' SET THE VARIABLE RESULTS TO DISPLAY
	'CALL sendCommandWithFeedback(":PAGE:DISP:LIST "+ CStr(list), "variables to be downloaded")


	' start timing the experiment measurement execution
	startTime = Timer()
	
	'Response.write("Here is the result of downloading the variables : <font color=""#ff0000""><b>" & _
	'	result & "</b></font>")

	' EXECUTION OF THE EXPERIMENT ***********************
	hp4155.setTimeOut 60000

	CALL sendCommandWithFeedback(":PAGE:SCON:SING", "measurement execution")
	
	' set TimeOut to 10 minutes while waiting for measurement to finish
	
	'wait for it to finish
	result = hp4155.waitUntilDone	

	' download all the variables
	for i = 0 to 3	
		if Request.Querystring.Item("listItem" & i) = "" then
			exit for
		else
			CALL sendQueryWithFeedback(":DATA? " & Request.QueryString.Item("listItem" & i), "all data variables")
		end if
	next

	'download user-defined variables
	for i = 0 to UBound(userfunctions)
		if (userfunctions(i) = "") then
			exit for
		end if

		CALL sendQueryWithFeedback(":PAGE:CHAN:UFUN:DEF? func" & count, "user-defined function " & i)
	next
		

	endTime = Timer()

	'Response.write("Operation ended at " & endTime & "<br>")
	Response.write("<B>This operation took a total of " & (endTime - startTime) & " seconds.</B><p>")

	Response.write("Result of experiment execution : " & encodeResult(result) & ".<p>&nbsp;<p>")

	' display the variables
	Response.write("Dumping the measurement output to the HP4155...<br> " & _
		"To view on the device, press ""Display"" button before running experiment...<p>")
	CALL sendCommandWithFeedback(":HCOP:SDUM", "screen dump")
	
	
	

	' set TimeOut to back to 1 min
	hp4155.setTimeOut 6000

	'flush the read buffer
	hp4155.flush

	' get the data on the variables for display
	'dataoutput = hp4155.query(":DATA? " & VCE)

	'Response.write("VCE: " & dataoutput & "<br>")

	hp4155.close_session

	


	Response.write("<br>CLOSE OF SESSION<BR>")

end sub

' generate the time to ensure that the ASP processing is correct (time returns a unique value)
sub returnTest()
	Response.write("<p>ASP Test successful; Current Time: ")
	Response.write(Time)
	Response.write("<p>")
end sub

function validateQueryString()


	' get the mode (e.g. SWEEP, SAMPLE, etc.)
	mode = Request.QueryString.Item("mode")
	
	'Response.write("Here is the mode you selected " & mode & "<br>")

	if mode <> "SWEEP" then
		error_message "Wrong Mode Selected" , "Select SWEEP mode and try again"
		validateQueryString = "FALSE"
		'exit function
	end if

	validateQueryString = "TRUE"


end function

sub sendCommandWithFeedBack(command, explanation)
	' PURPOSE:
	'	sends command, writes a natural language explanation to the response, and writes back feedback to the response
	'	A convenience method for reducing code clutter
	
	Response.write("Setting the " & explanation & " w/ the command: <i>" & command & "</i>.<br>")
	result = hp4155.command(command)

	Response.write("Result of setting the " & explanation & ": " & encodeResult(result) & ".<p>")
end sub

sub sendQueryWithFeedback(command, explanation)
	' PURPOSE:
	'	sends a query to the 4155
	
	Response.write("Querying the " & explanation & " w/ the command: <i>" & command & "</i>.<br>")
	
	hp4155.query(command)

	'Response.write("Result of setting the " & explanation & ": <b>" & " " & "</b>.<p>")
end sub

function encodeResult(result)
	' PURPOSE:
	'	convenience method which returns a string representing a string (typically a GPIB command result)
	'	encoded in a particular way in HTML
	if (result <> "SUCCESS") then
		encodeResult = "<b><font color=""#ff0000"">" & result & "</font></b>"
	else
		encodeResult = "<b><font color=""#0000FF"">" & result & "</font></b>"
	end if
end function

'****************************************************************************
'****************************************************************************
function setSMU(hp4155, number) 
	' PURPOSE:
	'	 Sets channel information for the specified SMU (i.e. Vname, Iname, mode, function)
	'	 Based on the function specified, then sets measurement parameters for that SMU.
	'	 See "RETURNS" for specific values returned for different functions.
	' INPUT:
	'	 Takes as an input a COM object representing the interface to the 4155 analyzer,
	'	 and the number SMU which to set up (currently from 1-5)
	' RETURNS:
	'	if vName is "" => ""
	'	else if channel set GPIB command fails => "FAILURE"
	'	else if SMU function is "VAR1" => complete GPIB statement setting all VAR1 parameters 
	'		(scale, step, start, stop, compliance)
	'	else if SMU function is "VAR2" => ditto for VAR2 parameters
	'	else if SMU mode is "COMM" (i.e. among V,I, or COMM) => "COMM"
	'	else if SMU function is "CONS" => complete GPIB statement setting CONS source value and compliance
	
	vName = Request.QueryString.Item("SMU" & number & "VName")
	'Response.write("Recorded the following VName for Device " & number & " : " & vName & "<br>")
	if (vName = "") then 
		setSMU = ""
		exit function
	end if

	'Response.Write("<b>Setting channel information for SMU " & number & "</b><br>")

	iName = Request.QueryString.Item("SMU" & number & "IName")
	mode = Request.QueryString.Item("SMU" & number & "Mode")
	fctn = Request.QueryString.Item("SMU" & number & "Function")
	
	' debug info
	'Response.write("&nbsp;&nbsp;&nbsp;vName : " & vName & "<br>")
	'Response.write("&nbsp;&nbsp;&nbsp;iName : " & iName & "<br>")
	'Response.write("&nbsp;&nbsp;&nbsp;Mode : " & mode & "<br>")
	'Response.write("&nbsp;&nbsp;&nbsp;Function : " & fctn & "<br>")

	' Channel setup GPIB commands
	command1 = "PAGE:CHAN:SMU"& number & ":VNAM '"&vName& "'"
	command2 = ":PAGE:CHAN:SMU"& number &":INAM '"+iname+ "'"
	command3 = ":PAGE:CHAN:SMU" & number & ":MODE " + mode + ""
	command4 = ":PAGE:CHAN:SMU" & number & ":FUNC " + fctn

	' batch commands into a single one
	command = command1 & ";" & command2 & ";" & command3 & ";" & command4
	result = hp4155.command(command)

	if (result <> "SUCCESS") then
		error_message "&nbsp;&nbsp;&nbsp;SMU"&number& " error. (" + result + ")", "Change your SMU"&number&" setup and try again."
		setSMU = "FAILURE"
		exit function
	end if 

	' Debug info
	
	'Response.write("&nbsp;&nbsp;&nbsp;<i>Sending channel info GPIB command : </i><br>&nbsp;&nbsp;&nbsp;" _
	'		& command & "<br>")
	'Response.write("&nbsp;&nbsp;&nbsp;Here is the result of the setting channel info for SMU" & number _ 
	'		& ": <b><font color=""#ff0000"">" & result & "</font></b><p>")
	

	' for SMU in I-mode i.e. measuring voltage
	if (mode = "I") then
		' hard code ranging mode temporarily
		CALL sendCommandWithFeedback(":PAGE:MEAS:MSET:SMU" & number & ":RANG:MODE FIX", "ranging mode for I-Mode SMU" & number)
		CALL sendCommandWithFeedback(":PAGE:MEAS:MSET:SMU" & number & ":RANG 20", "range value for SMU" & number)
	end if

	' for SMU in V-mode , i.e. measuring current
	if (mode = "V") then
		' hard code ranging mode temporarily
		CALL sendCommandWithFeedback(":PAGE:MEAS:MSET:SMU" & number & ":RANG:MODE FIX", "ranging mode for V-Mode SMU" & number)
		CALL sendCommandWithFeedback(":PAGE:MEAS:MSET:SMU" & number & ":RANG 1", "range value for SMU" & number)
	end if

	'setup the VAR1, VAR2, CONS, of COMM command line for this SMU
	if fctn = "VAR1" then
		start = ":PAGE:MEAS:VAR1:STAR "+ Request.QueryString.Item("VAR1Start")
		stopV = ":PAGE:MEAS:VAR1:STOP " + Request.QueryString.Item("VAR1Stop")
		scale = ":PAGE:MEAS:VAR1:SPAC " + Request.QueryString.Item("VAR1Scale")
		if Request.QueryString.Item("VAR1Scale") = "LIN" then 
				steps =  ":PAGE:MEAS:VAR1:STEP "+ Request.QueryString.Item("VAR1Step")
			else steps = ""	
		end if
		compliance = ":PAGE:MEAS:VAR1:COMP "+Request.QueryString.Item("VAR1Compliance")
		
		
		resultV1 = scale + ";" + start + ";" + stopV + ";" + steps + ";" + compliance
		
		'temporarily set result to individual commands for debugging
		'result = scale


		'Debug info
		'Response.write("&nbsp;&nbsp;&nbsp;Here is the GPIB command for VAR1 Info for this SMU : " _ 
		'		& resultV1 & "<br>")
		
		setSMU = resultV1
	
	elseif fctn = "VAR2" then
		start = ":PAGE:MEAS:VAR2:START " +Request.QueryString.Item("VAR2Start")+";"
		step  = ":PAGE:MEAS:VAR2:STEP " + Request.QueryString.Item("VAR2Step") +";"
		points=":PAGE:MEAS:VAR2:POINTS "+Request.QueryString.Item("VAR2Points")+";"
		compliance = ":PAGE:MEAS:VAR2:COMP " + Request.QueryString.Item("VAR2Compliance")
		setSMU = start + step + points + compliance

	elseif fctn = "VARD" then
		offset = ":PAGE:MEAS:VARD:OFFS " + Request.QueryString.Item("VARDOffset")+";"
		ratio = ":PAGE:MEAS:VARD:RAT " + Request.QueryString.Item("VARDRatio")+";"
		compliance = ":PAGE:MEAS:VARD:COMP " + Request.QueryString.Item("VARDCompliance")
		setSMU = offset + ratio + compliance

	elseif mode = "COMM" then 
		setSMU = "COMM"

	elseif fctn = "CONS" then ' the SMU serves as a constant source i.e. fctn = "CONS"
		'Response.write("&nbsp;&nbsp;&nbsp;This SMU is serving as a constant source <br>")
		source = ":PAGE:MEAS:CONS:SMU" & number & " " & Request.QueryString.Item("SMU"&number&"Source") & ";"
		compliance = ":PAGE:MEAS:CONS:SMU"&number&":COMP "& Request.QueryString.Item("SMU"&number&"Compliance")

		' debug info
		'Response.write("This is the GPIB command issued to set this SMU as a constant source : " & source + compliance & "<br>")

		setSMU = source & compliance
	else
		CALL _ 
			error_message("&nbsp;&nbsp;&nbsp;Function/Mode combo not recognized", "Please re-enter your function and/or mode")	
	end if
end function


function setFnct(hp4155, command)
	if command = "COMM" OR command = "" then 
		setFnct = "SUCCESS"
		exit function
	end if

	

	'Response.write("&nbsp;&nbsp;&nbsp;Running the following measurement specification " _ 
	'	& command & "<br>")
	result = hp4155.command(command)
	if result <> "SUCCESS" then
		error_message "Error: (" + result + ")","Go back and try again.  Make sure that your Value, Compliance, VAR1, or VAR2 settings are correct. There was a problem setting the value of it.  If this problem persists, contact " & application("system administrator")
		setFnct = "FAILURE"
		exit Function
	end if

	'Response.write("&nbsp;&nbsp;&nbsp;Here is the result of setting the previous measurement spec: " & _
	'	"<b><font color=""#ff0000""> " & result & "</font></b><p>")
end function

sub error_message(message,suggestion)
	sql = "UPDATE Sessions set Result='"&message&"', EndTime=getdate() where UserName='"&request.form("username")&"' and StartTime = '"& stime & "';"
	Users.Execute(sql)
	Response.write("ERROR MESSAGE " & message & chr(13))
	Response.write("ERROR SUGGESTION " & suggestion & chr(13))
	hp4155.close_session
end sub

%>