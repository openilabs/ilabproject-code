<%@ Page validateRequest="false" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="validation_engine.WebLabSystemComponents" %>
<%@ Import Namespace="WebLabDataManagers" %>

<script Runat="server">
Dim strPageState, strValidateResponse as String
Dim valEngine As ValidationEngine = New ValidationEngine()


Sub Page_Load
	If not Page.IsPostBack then
		strPageState = "NEWLOAD"
	End If
End Sub

Sub btnValidate_Click(s As Object, e As EventArgs)

	If Trim(txtExpSpec.Text) = "" Then
		strValidateResponse = "You must enter an experiment specification."
	ElseIf Trim(txtBrokerID.Text) = "" Or Not IsNumeric(txtBrokerID.Text) Then
		parseXMLSpec(txtExpSpec.Text)
		'strValidateResponse = "You must enter a numeric Broker ID."
	ElseIf Trim(txtGroupID.Text) = "" Then
		strValidateResponse = valEngine.validate(txtExpSpec.Text, CInt(txtBrokerID.Text))
	ElseIf IsNumeric(txtGroupID.Text) Then
		strValidateResponse = valEngine.validate(txtExpSpec.Text, CInt(txtBrokerID.Text), CInt(txtGroupID.Text))
	Else
		strValidateResponse = "Error - no good case."
	
	End If
	
	strPageState = "VALIDATED"

End Sub

 Private Function parseXMLSpec(ByVal strExpSpec As String)
        Dim termInfoTable(,), udfInfoTable(,), functInfoTable(,), listItem, listUnits As String
        Dim termNodeListLength, udfNodeListLength, intSetupID, intPriority As Integer
        Dim xmlExpSpec, xmlTemp As XmlDocument
        Dim rpmObject As New ResourcePermissionManager

        Dim intVar1Points As Integer = 0
        Dim intVar2Points As Integer = 0

        Dim intTotalPoints As Integer = 0
        Dim intPointLimit As Integer = 0
        Dim intResourceID As Integer = 0


        Dim FGEN_record As Integer = -1 '-1 is default, unassigned value, dependency: parseXMLSpec
        Dim SCOPE_record As Integer = -1
        
        'terminal info fields
        Const TERM_INSTRUMENT As Integer = 0
        Const TERM_VNAME As Integer = 1
        Const TERM_INAME As Integer = 2
        Const TERM_MODE As Integer = 3
        Const TERM_FUNCTION_TYPE As Integer = 4
        Const TERM_COMPLIANCE As Integer = 5

        'instrument labels for function info
        Const FGEN_FUNCT As Integer = 0
        Const SCOPE_FUNCT As Integer = 1
        
        'function information fields
        Const FUNCT_SCALE As Integer = 0
        Const FUNCT_START As Integer = 1
        Const FUNCT_STOP As Integer = 2
        Const FUNCT_STEP As Integer = 3
        Const FUNCT_OFFSET As Integer = 4
        Const FUNCT_RATIO As Integer = 5
        Const FUNCT_VALUE As Integer = 6
        Const FUNCT_WAVEFORMTYPE As Integer = 7
        Const FUNCT_FREQUENCY As Integer = 8
        Const FUNCT_AMPLITUDE As Integer = 9

        'user defined function info fields
        Const UDF_NAME As Integer = 0
        Const UDF_UNITS As Integer = 1
        Const UDF_BODY As Integer = 2
        
        Dim tempXPath, udfXPath, termXPath, instrumentType, termName, udfName As String
        Dim tempNode As XmlNode
        Dim termNodeList, udfNodeList As XmlNodeList
        Dim instrumentConstant, loopIdx As Integer
        listItem = ""
        listUnits = ""

        Debug.WriteLine("begin parsing Exp Spec")

        xmlExpSpec = New XmlDocument
        xmlExpSpec.LoadXml(strExpSpec)

        'loads terminal nodes 
        termXPath = "/experimentSpecification/terminal"
        termNodeList = xmlExpSpec.SelectNodes(termXPath)
        termNodeListLength = termNodeList.Count()

        'loads User Defined Function (udf) nodes
        udfXPath = "/experimentSpecification/userDefinedFunction"
        udfNodeList = xmlExpSpec.SelectNodes(udfXPath)
        udfNodeListLength = udfNodeList.Count()

        'builds 2D array, by # of terminals and then by field (reinitiallized)
        ReDim termInfoTable((termNodeListLength - 1), 5)

        'builds 2D array, by # of udfs and then by field (reinitiallized)
        ReDim udfInfoTable((udfNodeListLength - 1), 2)

        'builds 2D array, by function and then by field (reinitiallized)
        ReDim functInfoTable(5, 10)

        tempXPath = "/experimentSpecification/setupID"
        tempNode = xmlExpSpec.SelectSingleNode(tempXPath)
        intSetupID = CInt(tempNode.InnerXml())
        Debug.WriteLine("setup ID =" & CStr(intSetupID))

        'processes terminal subtrees
        For loopIdx = 0 To termNodeListLength - 1
            xmlTemp = New XmlDocument
            xmlTemp.LoadXml(termNodeList.Item(loopIdx).OuterXml())

            'instrument
            tempXPath = "/terminal/@instrumentType"
            tempNode = xmlTemp.SelectSingleNode(tempXPath)
            instrumentType = Trim(tempNode.InnerXml())
            Debug.WriteLine("instrument type=" & Trim(tempNode.InnerXml()))
            tempXPath = "/terminal/@instrumentNumber"
            tempNode = xmlTemp.SelectSingleNode(tempXPath)
            termInfoTable(loopIdx, TERM_INSTRUMENT) = instrumentType '& Trim(tempNode.InnerXml())
            Debug.WriteLine("instrument number=" & Trim(tempNode.InnerXml()))

            Select Case termInfoTable(loopIdx, TERM_INSTRUMENT)
                Case "FGEN"
                    instrumentConstant = FGEN_FUNCT
                    FGEN_record = loopIdx
                Case "SCOPE"
                    instrumentConstant = SCOPE_FUNCT
                    SCOPE_record = loopIdx
            End Select

            'load vname & download attribute
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

            'load iname & download attribute
            'tempXPath = "/terminal/iname"
            'tempNode = xmlTemp.SelectSingleNode(tempXPath)
            'termName = Trim(tempNode.InnerXml())
			'Debug.WriteLine("Iname=" & Trim(tempNode.InnerXml()))
			'termInfoTable(loopIdx, TERM_INAME) = termName
		
			'tempXPath = "/terminal/iname/@download"
			'tempNode = xmlTemp.SelectSingleNode(tempXPath)
			'If Trim(tempNode.InnerXml()) = "true" Then
			'	listItem = listItem & "'" & termName & "'" & ", "
			'	listUnits = listUnits & "A, "
			'	Debug.WriteLine(listItem)
			'	Debug.WriteLine(listUnits)
			'End If
			'load mode setting
            'tempXPath = "/terminal/mode"
            'tempNode = xmlTemp.SelectSingleNode(tempXPath)
            'termInfoTable(loopIdx, TERM_MODE) = Trim(tempNode.InnerXml())
            'Debug.WriteLine("mode=" & Trim(tempNode.InnerXml()))

            'load compliance value
            'If termInfoTable(loopIdx, TERM_MODE) <> "COMM" Then     'added conditional
            '    tempXPath = "/terminal/compliance"
            '    tempNode = xmlTemp.SelectSingleNode(tempXPath)
            '    termInfoTable(loopIdx, TERM_COMPLIANCE) = Trim(tempNode.InnerXml())
            '    Debug.WriteLine("compliance=" & Trim(tempNode.InnerXml()))
            'End If

            'load function type & process sub-nodes. Change this condition as soon as we determine how the modes will
            'work for ELVIS
            If (instrumentType <> "VMU") And (termInfoTable(loopIdx, TERM_MODE) <> "COMM") Then 'should be <> "COMM" ?
                tempXPath = "/terminal/function/@type"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                termInfoTable(loopIdx, TERM_FUNCTION_TYPE) = Trim(tempNode.InnerXml())
                Debug.WriteLine("function type=" & Trim(tempNode.InnerXml()))

                Select Case termInfoTable(loopIdx, TERM_FUNCTION_TYPE)
                    Case "WAVEFORM"
                        'load waveformTYpe value
                        tempXPath = "/terminal/function/waveformType"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_WAVEFORMTYPE) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("waveformType=" & Trim(tempNode.InnerXml()))

                        'load frequency value
                        tempXPath = "/terminal/function/frequency"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_FREQUENCY) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Frequency=" & Trim(tempNode.InnerXml()))

                        'load amplitude value
                        tempXPath = "/terminal/function/amplitude"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_AMPLITUDE) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Amplitude=" & Trim(tempNode.InnerXml()))

                        'load offset value
                        tempXPath = "/terminal/function/offset"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_OFFSET) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Offset=" & Trim(tempNode.InnerXml()))

                    Case "VAR1"
                        'load scale value
                        tempXPath = "/terminal/function/scale"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_SCALE) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Scale=" & Trim(tempNode.InnerXml()))

                        'load start value
                        tempXPath = "/terminal/function/start"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_START) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Start=" & Trim(tempNode.InnerXml()))

                        'load stop value
                        tempXPath = "/terminal/function/stop"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_STOP) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Stop=" & Trim(tempNode.InnerXml()))

                        'load step interval (only present if in linear mode)
                        If UCase(functInfoTable(instrumentConstant, FUNCT_SCALE)) = "LIN" Then

                            tempXPath = "/terminal/function/step"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            functInfoTable(instrumentConstant, FUNCT_STEP) = Trim(tempNode.InnerXml())
                            Debug.WriteLine("Step=" & Trim(tempNode.InnerXml()))
                        End If

                    Case "VAR2"
                        'load start value
                        tempXPath = "/terminal/function/start"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_START) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Start=" & Trim(tempNode.InnerXml()))

                        'load stop value
                        tempXPath = "/terminal/function/stop"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_STOP) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Stop=" & Trim(tempNode.InnerXml()))

                        'load step interval
                        tempXPath = "/terminal/function/step"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_STEP) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Step=" & Trim(tempNode.InnerXml()))

                    Case "VAR1P"
                        'load ratio value
                        tempXPath = "/terminal/function/ratio"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_RATIO) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("ratio=" & Trim(tempNode.InnerXml()))

                        'load offset value
                        tempXPath = "/terminal/function/offset"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_OFFSET) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("offset=" & Trim(tempNode.InnerXml()))

                    Case "CONS"
                        'load value 
                        tempXPath = "/terminal/function/value"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_VALUE) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("value=" & Trim(tempNode.InnerXml()))
                End Select
            End If
            Next

            'process udf subtree
            For loopIdx = 0 To udfNodeListLength - 1
                xmlTemp = New XmlDocument
                xmlTemp.LoadXml(udfNodeList.Item(loopIdx).OuterXml())

                'load units 
                tempXPath = "/userDefinedFunction/units"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                udfInfoTable(loopIdx, UDF_UNITS) = Trim(tempNode.InnerXml())
                Debug.WriteLine("udf units=" & Trim(tempNode.InnerXml()))

                'load name and download attribute
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

                'load function body
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
			Response.write("DataPoints: " & valEngine.getTotalDataPoints())
	%>
		
    
    
    <%
    End Select
    %>
    

    </form>
  </body>
</html>
