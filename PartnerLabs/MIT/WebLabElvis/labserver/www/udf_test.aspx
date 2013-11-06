<%@ Page validateRequest="false" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="WebLab.Elvis" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title>WebLab Lab Server User Defined Function Validation Module Test Page</title>
  </head>
  
  <body>

<script Runat="server">
Dim strPageState, strValidateResponse as String
Dim valEngine As ValidationEngine = New ValidationEngine()


Sub Page_Load
	If not Page.IsPostBack then
		strPageState = "NEWLOAD"
	End If
End Sub

Sub btnValidate_Click(s As Object, e As EventArgs)
	If Trim(txtUDFBody.Text) = "" OR Trim(txtVarList.Text) = "" Then
		strValidateResponse = "All fields are required."
	Else
		Dim UDFCheckProcess As Process = New Process()
        Dim strChkOut, strTermOut, strVarList, strUDFBody As String
        Dim loopIdx, innerLoopIdx, intIterations As Integer
        
        If Not IsNumeric(txtIterations.Text) OR Trim(txtIterations.Text) = "" Then
			intIterations = 1
		Else
			intIterations = CInt(txtIterations.Text)
		End If
        

		strVarList = txtVarList.Text
		strUDFBody = txtUDFBody.Text

        UDFCheckProcess.StartInfo.FileName = "cmd.exe"
        UDFCheckProcess.StartInfo.UseShellExecute = False
        UDFCheckProcess.StartInfo.CreateNoWindow = True
        UDFCheckProcess.StartInfo.RedirectStandardInput = True
        UDFCheckProcess.StartInfo.RedirectStandardOutput = True
        UDFCheckProcess.StartInfo.RedirectStandardError = True
        UDFCheckProcess.Start()
        Dim sIn As StreamWriter = UDFCheckProcess.StandardInput
        Dim sOut As StreamReader = UDFCheckProcess.StandardOutput
        sIn.AutoFlush = True

        sIn.Write("cd c:\weblab\labser~1\java" & Environment.NewLine)
		
		strTermOut = ""
						
		For loopIdx = 0 To intIterations - 1
		Thread.Sleep(1000)
		sOut.ReadLine()
		sOut.DiscardBufferedData()
        sIn.Write("java -classpath bin weblab.validation.Validation -v """ & strVarList & """ """ & strUDFBody & """" & Environment.NewLine)
        
        Thread.Sleep(1000)
        
        
   
        sOut.ReadLine()
       
        
		strChkOut = sOut.ReadLine()
		
		
		If Trim(strChkOut) <> "OK" Then
            strValidateResponse = "Error Parsing User-defined function - " & strChkOut & "<br>"
            'sIn.Close()
            'sOut.Close()
            'UDFCheckProcess.Close()
        Else
			strValidateResponse = "UDF OK <br>"
        End If
			Response.write("<b> Iteration" & loopIdx + 1 & ": </b><br>" & strValidateResponse & "<p>")
        
		Next
        
        sIn.Write("exit" & Environment.NewLine)
    
		
        If Not UDFCheckProcess.HasExited Then
            UDFCheckProcess.Kill()
        End If
        
        Response.Write("<b>Terminal Output:</b><br>" & strTermOut & "<p>")
		
        'While sOut.Peek() > -1
            
           
            
       ' End While

        sIn.Close()
        sOut.Close()
        UDFCheckProcess.Close()

	
	End If
	
	strPageState = "VALIDATED"

End Sub


</script>



    <form Runat="server">
    <%
    Select Case strPageState
		Case "NEWLOAD"
    %>
		<p>
		This is a script used to test the Lab Server User Defined Function Validation Component.  To use, paste or type a comma-separated list of mock variable names and user-defined function in the textboxes below,
		and then click on the validate button.  This will submit the function for validation and return the result of the process.
		<p>
		<table border=0 cellpadding=0 cellspacing=0>
			<tr>
				<th>Enter a comma-separated list of variable names (required):</th>
				<td>
					<asp:TextBox
						id="txtVarList"
						Columns="20"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<th>Enter your user defined function (required):</th>
				<td>
					<asp:TextBox
						id="txtUDFBody"
						Columns="20"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<th>Enter the numebr of times validation should be repeated (optional):</th>
				<td>
					<asp:TextBox
						id="txtIterations"
						Columns="5"
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
			'Response.write(strValidateResponse)
	%>
		
    
    
    <%
    End Select
    %>
    

    </form>
  </body>
</html>
