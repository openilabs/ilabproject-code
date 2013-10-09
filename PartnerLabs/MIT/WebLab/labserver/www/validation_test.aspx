<%@ Page validateRequest="false" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="WebLabSystemComponents" %>
<%@ Import Namespace="WebLabCustomDataTypes" %>
<!--Import Namespace="validation_engine.WebLabSystemComponents" -->

<!-- 
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->


<script Runat="server">
Dim strPageState, strValidateResponse as String
Dim intDataPoints As Integer
'Dim valEngine As ValidationEngine = New ValidationEngine()
Dim valEngine As ExperimentValidator = New ExperimentValidator()
Dim parser As ExperimentParser = New ExperimentParser()
Dim valObject As ValidationObject
Dim expObject As ExperimentObject


Sub Page_Load
	If not Page.IsPostBack then
		strPageState = "NEWLOAD"
	End If
End Sub

Sub btnValidate_Click(s As Object, e As EventArgs)

	If Trim(txtExpSpec.Text) = "" Then
		strValidateResponse = "You must enter an experiment specification."
	ElseIf Trim(txtBrokerID.Text) = "" Or Not IsNumeric(txtBrokerID.Text) Then
		expObject = parser.parseXMLExpSpec(txtExpSpec.Text)
		If expObject.Item("DeviceNum") Is Nothing Then
			strValidateResponse = "Failure on specification parsing (return object empty)"
		Else
			strValidateResponse = "Specification successfully parsed (Device Number is " & expObject.Item("DeviceNum") & ")."
		'strValidateResponse = "Specification parsed - The following items were found " & expObject.EnumerateItems()
		End If
		intDataPoints = 0
		'strValidateResponse = "You must enter a numeric Broker ID."
	ElseIf Trim(txtGroupID.Text) = "" Then
		valObject = valEngine.validate(txtExpSpec.Text, CInt(txtBrokerID.Text), MapPath("/"))
		strValidateResponse = valObject.errorMessage
		If valObject.isValid Then
			intDataPoints = valObject.dataPoints
		Else
			intDataPoints = 0
		End If
		
	ElseIf IsNumeric(txtGroupID.Text) Then
		valObject = valEngine.validate(txtExpSpec.Text, CInt(txtBrokerID.Text), CInt(txtGroupID.Text), MapPath("/"))
		strValidateResponse = valObject.errorMessage
		If valObject.isValid Then
			intDataPoints = valObject.dataPoints
		Else
			intDataPoints = 0
		End If
	
	Else
		intDataPoints = 0
		strValidateResponse = "Error - no good case."
	
	End If
	
	strPageState = "VALIDATED"

End Sub

 Private Function parseXMLSpec(ByVal strExpSpec As String)
            Dim tempXPath, udfXPath, termXPath, portType, termName, udfName, listItem, listUnits As String
            Dim tempNode As XmlNode
            Dim termNodeList, udfNodeList As XmlNodeList
            Dim xmlExpSpec, xmlTemp As XmlDocument
            Dim termNodeListLength, udfNodeListLength, intDeviceNumber, intPriority As Integer
            Dim portConstant, loopIdx As Integer
            listItem = ""
            listUnits = ""
            
             Dim SMU1_record As Integer = -1 '-1 is default, unassigned value, dependency: parseXMLSpec
			Dim SMU2_record As Integer = -1
			Dim SMU3_record As Integer = -1
			Dim SMU4_record As Integer = -1
			Dim VSU1_record As Integer = -1
			Dim VSU2_record As Integer = -1
			Dim VMU1_record As Integer = -1
			Dim VMU2_record As Integer = -1
            
             'terminal info fields
			Const TERM_PORT As Integer = 0
			Const TERM_VNAME As Integer = 1
			Const TERM_INAME As Integer = 2
			Const TERM_MODE As Integer = 3
			Const TERM_FUNCTION_TYPE As Integer = 4
			Const TERM_COMPLIANCE As Integer = 5

			'port labels for function info
			Const SMU1_FUNCT As Integer = 0
			Const SMU2_FUNCT As Integer = 1
			Const SMU3_FUNCT As Integer = 2
			Const SMU4_FUNCT As Integer = 3
			Const VSU1_FUNCT As Integer = 4
			Const VSU2_FUNCT As Integer = 5
			'function information fields
			Const FUNCT_SCALE As Integer = 0
			Const FUNCT_START As Integer = 1
			Const FUNCT_STOP As Integer = 2
			Const FUNCT_STEP As Integer = 3
			Const FUNCT_OFFSET As Integer = 4
			Const FUNCT_RATIO As Integer = 5
			Const FUNCT_VALUE As Integer = 6

			'user defined function info fields
			Const UDF_NAME As Integer = 0
			Const UDF_UNITS As Integer = 1
			Const UDF_BODY As Integer = 2

            Debug.WriteLine("begin parsing Exp Spec")

            xmlExpSpec = New XmlDocument()
            xmlExpSpec.LoadXml(strExpSpec)

            'loads terminal nodes 
            termXPath = "/experimentSpecification/terminal"
            termNodeList = xmlExpSpec.SelectNodes(termXPath)
            termNodeListLength = termNodeList.Count()

            'loads udf nodes
            udfXPath = "/experimentSpecification/userDefinedFunction"
            udfNodeList = xmlExpSpec.SelectNodes(udfXPath)
            udfNodeListLength = udfNodeList.Count()

            'builds 2D array, by # of terminals and then by field (reinitiallized)
            Dim termInfoTable((termNodeListLength - 1), 5)
            'builds 2D array, by # of udfs and then by field (reinitiallized)
            Dim udfInfoTable((udfNodeListLength - 1), 2)
            'builds 2D array, by port and then by field (reinitiallized)
            Dim functInfoTable(5, 6)

            tempXPath = "/experimentSpecification/deviceID"
            tempNode = xmlExpSpec.SelectSingleNode(tempXPath)
            intDeviceNumber = CInt(tempNode.InnerXml()) - 1 'start is at zero (for switching matrix)
            Debug.WriteLine("device number=" & CStr(intDeviceNumber))

            For loopIdx = 0 To termNodeListLength - 1
                xmlTemp = New XmlDocument()
                xmlTemp.LoadXml(termNodeList.Item(loopIdx).OuterXml())

                'port
                tempXPath = "/terminal/@portType"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                portType = Trim(tempNode.InnerXml())
                Debug.WriteLine("port type=" & Trim(tempNode.InnerXml()))
                tempXPath = "/terminal/@portNumber"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                termInfoTable(loopIdx, TERM_PORT) = portType & Trim(tempNode.InnerXml())
                Debug.WriteLine("port number=" & Trim(tempNode.InnerXml()))

                Select Case termInfoTable(loopIdx, TERM_PORT)
                    Case "SMU1"
                        portConstant = SMU1_FUNCT
                        SMU1_record = loopIdx
                    Case "SMU2"
                        portConstant = SMU2_FUNCT
                        SMU2_record = loopIdx
                    Case "SMU3"
                        portConstant = SMU3_FUNCT
                        SMU3_record = loopIdx
                    Case "SMU4"
                        portConstant = SMU4_FUNCT
                        SMU4_record = loopIdx
                    Case "VSU1"
                        portConstant = VSU1_FUNCT
                        VSU1_record = loopIdx
                    Case "VSU2"
                        portConstant = VSU2_FUNCT
                        VSU2_record = loopIdx
                    Case "VMU1"
                        VMU1_record = loopIdx
                    Case "VMU2"
                        VMU2_record = loopIdx
                End Select

                'vname & download attribute
                tempXPath = "/terminal/vname"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                termName = Trim(tempNode.InnerXml())
                Debug.WriteLine("Vname=" & Trim(tempNode.InnerXml()))
                termInfoTable(loopIdx, TERM_VNAME) = termName

                tempXPath = "/terminal/vname/@download"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                If Trim(tempNode.InnerXml()) = "true" Then
                    listItem = listItem & "'" & termName & "'" & ", "
                    listUnits = listUnits & "V, "
                    Debug.WriteLine(listItem)
                    Debug.WriteLine(listUnits)
                End If

                'iname & download attribute
                tempXPath = "/terminal/iname"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                termName = Trim(tempNode.InnerXml())
                Debug.WriteLine("Iname=" & Trim(tempNode.InnerXml()))
                termInfoTable(loopIdx, TERM_INAME) = termName

                tempXPath = "/terminal/iname/@download"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                If Trim(tempNode.InnerXml()) = "true" Then
                    listItem = listItem & "'" & termName & "'" & ", "
                    listUnits = listUnits & "A, "
                    Debug.WriteLine(listItem)
                    Debug.WriteLine(listUnits)
                End If

                'mode
                tempXPath = "/terminal/mode"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                termInfoTable(loopIdx, TERM_MODE) = Trim(tempNode.InnerXml())
                Debug.WriteLine("mode=" & Trim(tempNode.InnerXml()))

                'compliance
                If termInfoTable(loopIdx, TERM_MODE) <> "COMM" Then					'added conditional
                tempXPath = "/terminal/compliance"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                termInfoTable(loopIdx, TERM_COMPLIANCE) = Trim(tempNode.InnerXml())
                Debug.WriteLine("compliance=" & Trim(tempNode.InnerXml()))
                End If

                'function
                If (portType <> "VMU") And (termInfoTable(loopIdx, TERM_MODE) <> "COMM") Then 'should be and <> "COMM" ?
                    tempXPath = "/terminal/function/@type"
                    tempNode = xmlTemp.SelectSingleNode(tempXPath)
                    termInfoTable(loopIdx, TERM_FUNCTION_TYPE) = Trim(tempNode.InnerXml())
                    Debug.WriteLine("function type=" & Trim(tempNode.InnerXml()))

                    Select Case termInfoTable(loopIdx, TERM_FUNCTION_TYPE)
                        Case "VAR1"
                            'scale
                            tempXPath = "/terminal/function/scale"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(portConstant, FUNCT_SCALE) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("Scale=" & Trim(tempNode.InnerXml()))

                            'start
                            tempXPath = "/terminal/function/start"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(portConstant, FUNCT_START) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("Start=" & Trim(tempNode.InnerXml()))

                            'stop
                            tempXPath = "/terminal/function/stop"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(portConstant, FUNCT_STOP) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("Stop=" & Trim(tempNode.InnerXml()))

                            'step (only present if in linear mode)
                            If UCase(functInfoTable(portConstant, FUNCT_SCALE)) = "LIN" Then

                                tempXPath = "/terminal/function/step"
                                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                                functInfoTable(portConstant, FUNCT_STEP) = Trim(tempNode.InnerXml())
                                Debug.WriteLine("Step=" & Trim(tempNode.InnerXml()))
                            End If

                        Case "VAR2"
                            'start
                            tempXPath = "/terminal/function/start"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(portConstant, FUNCT_START) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("Start=" & Trim(tempNode.InnerXml()))

                            'stop
                            tempXPath = "/terminal/function/stop"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(portConstant, FUNCT_STOP) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("Stop=" & Trim(tempNode.InnerXml()))

                            'step
                            tempXPath = "/terminal/function/step"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(portConstant, FUNCT_STEP) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("Step=" & Trim(tempNode.InnerXml()))

                        Case "VAR1P"
                            'ratio
                            tempXPath = "/terminal/function/ratio"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(portConstant, FUNCT_RATIO) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("ratio=" & Trim(tempNode.InnerXml()))

                            'offset
                            tempXPath = "/terminal/function/offset"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(portConstant, FUNCT_OFFSET) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("offset=" & Trim(tempNode.InnerXml()))

                        Case "CONS"
                            'value
                            tempXPath = "/terminal/function/value"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(portConstant, FUNCT_VALUE) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("value=" & Trim(tempNode.InnerXml()))
                    End Select
                End If
            Next

            For loopIdx = 0 To udfNodeListLength - 1
                xmlTemp = New XmlDocument()
                xmlTemp.LoadXml(udfNodeList.Item(loopIdx).OuterXml())

                'units 
                tempXPath = "/userDefinedFunction/units"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                udfInfoTable(loopIdx, UDF_UNITS) = Trim(tempNode.InnerXml())
                Debug.WriteLine("udf units=" & Trim(tempNode.InnerXml()))

                'name and download attribute
                tempXPath = "/userDefinedFunction/name"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                udfName = Trim(tempNode.InnerXml())
                Debug.WriteLine("udf name=" & Trim(tempNode.InnerXml()))
                udfInfoTable(loopIdx, UDF_NAME) = udfName

                tempXPath = "/userDefinedFunction/name/@download"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                If Trim(tempNode.InnerXml()) = "true" Then
                    listItem = listItem & "'" & udfName & "'" & ", "
                    listUnits = listUnits & udfInfoTable(loopIdx, UDF_UNITS) & ", "
                    Debug.WriteLine(listItem)
                    Debug.WriteLine(listUnits)
                End If

                'body
                tempXPath = "/userDefinedFunction/body"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                udfInfoTable(loopIdx, UDF_BODY) = Trim(tempNode.InnerXml())
                Debug.WriteLine("udf body=" & Trim(tempNode.InnerXml()))
            Next
        End Function

</script>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title>WebLab Lab Server Validation Engine Test Page</title>
  </head>
  
  <body>
    <form Runat="server">
    <%
    Select Case strPageState
		Case "NEWLOAD"
    %>
		<p>
		This is a script used to test the Lab Server Experiment Validation module.  To use, paste or type a valid XML Experiment Specification in the textbox below,
		fill in a broker ID (required) and a group ID (optional) and then click on the validate button.  This will submit the spec for validation and return the result of the process.
		<p>
		<table border=0 cellpadding=0 cellspacing=0>
			<tr>
				<th>Enter your brokerID (required):</th>
				<td>
					<asp:TextBox
						id="txtBrokerID"
						Columns="5"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<th>Enter your groupID (optional):</th>
				<td>
					<asp:TextBox
						id="txtGroupID"
						Columns="5"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<th>Insert XML Experiment Specification here (required):</th>
				<td>
					<asp:TextBox
						id="txtExpSPec"
						TextMode="Multiline"
						Columns="70"
						Rows="30"
						Wrap="True"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<td colspan=2>
					<asp:Button
						id="btnValidate"
						text="Validate"
						OnClick="btnValidate_Click"
						Runat="Server" />
				</td>
			</tr>
				
				
				
		</table>
	<%
		Case "VALIDATED"
			Response.write(strValidateResponse & "<p>")
			Response.write("DataPoints: " & intdataPoints)
	%>
		
    
    
    <%
    End Select
    %>
    

    </form>
  </body>
</html>
