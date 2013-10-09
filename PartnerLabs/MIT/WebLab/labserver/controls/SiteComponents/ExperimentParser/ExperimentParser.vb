Imports System
Imports System.Xml
Imports Microsoft.VisualBasic
Imports WebLabCustomDataTypes

'Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
'Please see license.txt in top level directory for full license. 


Namespace WebLabSystemComponents

    'Author(s): James Hardison (hardison@alum.mit.edu)
    'Date Created: 4/7/2005
    'Date Modified: 4/7/2005
    'This class contains the methods/logic for the parsing of incoming ExperimentSpecification XML documents.  The main method of this class will be called by
    'external objects and passed an ExperimentSpecification document.  This method will parse that document into an ExperimentObject (Defined in WebLabCustom
    'DataTypes) in a form that is well known to the calling agent.  A completed ExperimentObject will be the output of this method.  In it's first 
    'implementation, this class will be used by the Web Process' Validation Engine as well as the independent Experiment Execution Engine.

    'Dependency List
    'Used By:
    '<FILL THIS IN>
    '
    'Uses:
    '   WebLab Custom Data Types (/controls/WebLabCustomDataTypes/WebLabCustomDataTypes.vb, /bin/WebLabCustomDataTypes.dll)

    Public Class ExperimentParser

        Public Function parseXMLExpSpec(ByVal strExpSpec As String) As ExperimentObject
            'This method takes as input a string representation of an Experiment Specification XML document.  Based on the known structure of this 
            'document, this method parses the input document and places the data into an ExperimentObject.  See WebLabCustomDataTypes for more 
            'information on this structure.  This method will organize it's output into a single ExperimentObject that contains the following objects:
            '1. An ExpUnitSetObject (keyed as TermTable) that contains all of the terminal/function specifications, keyed first on an integer index 
            '   and then on individual data item.
            '2. An ExpUnitSetObject (keyed as UDFTable) that contains all of the user defined function specifications, keyed first on and integer index
            '   and then on individual data item (units and body).
            '3. An string (keyed as DeviceNum) that contains the device location (on Switching Matrix hardware) this specification is intended to run on.
            '
            'Errors encountered in this method should be caught by the caller.

            Dim tempXPath, udfXPath, termXPath, strDeviceNum, portType As String
            Dim xmlExpSpec, xmlTemp As XmlDocument
            Dim tempNode As XmlNode
            Dim termNodeList, udfNodeList As XmlNodeList
            Dim loopIdx As Integer

            Dim expParsedSpec As New ExperimentObject()
            Dim eusTerminalTable As New ExpUnitSetObject()
            Dim eusUDFTable As New ExpUnitSetObject()


            'Begin specification parsing
            xmlExpSpec = New XmlDocument()
            xmlExpSpec.LoadXml(strExpSpec)

            'loads terminal nodes
            termXPath = "/experimentSpecification/terminal"
            termNodeList = xmlExpSpec.SelectNodes(termXPath)

            'loads User Defined Function (udf) nodes
            udfXPath = "/experimentSpecification/userDefinedFunction"
            udfNodeList = xmlExpSpec.SelectNodes(udfXPath)

            'parse out device number
            tempXPath = "/experimentSpecification/deviceID"
            tempNode = xmlExpSpec.SelectSingleNode(tempXPath)
            strDeviceNum = CInt(tempNode.InnerXml()) - 1  'index for switching matrix is zero-based

            'process terminal subtrees
            For loopIdx = 0 To termNodeList.Count() - 1
                xmlTemp = New XmlDocument()
                xmlTemp.LoadXml(termNodeList.Item(loopIdx).OuterXml())

                'create teminal in table
                eusTerminalTable.CreateUnit(CStr(loopIdx))

                'port
                tempXPath = "/terminal/@portType"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                portType = Trim(tempNode.InnerXml())
                tempXPath = "/terminal/@portNumber"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                eusTerminalTable.UnitItem(CStr(loopIdx), "port") = portType & Trim(tempNode.InnerXml())

                'load vname & download attribute
                tempXPath = "terminal/vname"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                eusTerminalTable.UnitItem(CStr(loopIdx), "vname") = Trim(tempNode.InnerXml())

                tempXPath = "terminal/vname/@download"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                eusTerminalTable.UnitItem(CStr(loopIdx), "dlVTerm") = Trim(tempNode.InnerXml())


                'begin loading conditional parameters
                If portType = "SMU" Then
                    'load iname & download attribute (only SMU's have inames)
                    tempXPath = "terminal/iname"
                    tempNode = xmlTemp.SelectSingleNode(tempXPath)
                    eusTerminalTable.UnitItem(CStr(loopIdx), "iname") = Trim(tempNode.InnerXml())

                    tempXPath = "terminal/iname/@download"
                    tempNode = xmlTemp.SelectSingleNode(tempXPath)
                    eusTerminalTable.UnitItem(CStr(loopIdx), "dlITerm") = Trim(tempNode.InnerXml())

                    'load mode setting (only for SMU)
                    tempXPath = "/terminal/mode"
                    tempNode = xmlTemp.SelectSingleNode(tempXPath)
                    eusTerminalTable.UnitItem(CStr(loopIdx), "mode") = Trim(tempNode.InnerXml())

                    If eusTerminalTable.UnitItem(CStr(loopIdx), "mode") <> "COMM" Then
                        'load compliance value (only valid when SMU type terminal is not set to common mode)
                        tempXPath = "/terminal/compliance"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        eusTerminalTable.UnitItem(CStr(loopIdx), "compliance") = Trim(tempNode.InnerXml())
                    End If
                End If

                If (portType = "SMU" And eusTerminalTable.UnitItem(CStr(loopIdx), "mode") <> "COMM") Or portType = "VSU" Then
                    'load terminal function type
                    tempXPath = "/terminal/function/@type"
                    tempNode = xmlTemp.SelectSingleNode(tempXPath)
                    eusTerminalTable.UnitItem(CStr(loopIdx), "functionType") = Trim(tempNode.InnerXml())

                    Select Case eusTerminalTable.UnitItem(CStr(loopIdx), "functionType")
                        Case "VAR1"
                            'load scale value
                            tempXPath = "/terminal/function/scale"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            eusTerminalTable.UnitItem(CStr(loopIdx), "scale") = Trim(tempNode.InnerXml())

                            'load start value
                            tempXPath = "/terminal/function/start"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            eusTerminalTable.UnitItem(CStr(loopIdx), "start") = Trim(tempNode.InnerXml())

                            'load stop value
                            tempXPath = "/terminal/function/stop"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            eusTerminalTable.UnitItem(CStr(loopIdx), "stop") = Trim(tempNode.InnerXml())

                            'load step interval (only present if scale is linear)
                            If UCase(eusTerminalTable.UnitItem(CStr(loopIdx), "scale")) = "LIN" Then
                                tempXPath = "/terminal/function/step"
                                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                                eusTerminalTable.UnitItem(CStr(loopIdx), "step") = Trim(tempNode.InnerXml())
                            End If

                        Case "VAR2"
                            'load start value
                            tempXPath = "/terminal/function/start"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            eusTerminalTable.UnitItem(CStr(loopIdx), "start") = Trim(tempNode.InnerXml())

                            'load stop value
                            tempXPath = "/terminal/function/stop"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            eusTerminalTable.UnitItem(CStr(loopIdx), "stop") = Trim(tempNode.InnerXml())

                            'load step interval 
                            tempXPath = "/terminal/function/step"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            eusTerminalTable.UnitItem(CStr(loopIdx), "step") = Trim(tempNode.InnerXml())

                        Case "VAR1P"
                            'load ratio value
                            tempXPath = "/terminal/function/ratio"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            eusTerminalTable.UnitItem(CStr(loopIdx), "ratio") = Trim(tempNode.InnerXml())

                            'load offset value
                            tempXPath = "/terminal/function/offset"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            eusTerminalTable.UnitItem(CStr(loopIdx), "offset") = Trim(tempNode.InnerXml())

                        Case "CONS"
                            'load value
                            tempXPath = "/terminal/function/value"
                            tempNode = xmlTemp.SelectSingleNode(tempXPath)
                            eusTerminalTable.UnitItem(CStr(loopIdx), "value") = Trim(tempNode.InnerXml())

                    End Select
                End If

            Next

            'process udf subtree
            For loopIdx = 0 To udfNodeList.Count() - 1
                xmlTemp = New XmlDocument()
                xmlTemp.LoadXml(udfNodeList.Item(loopIdx).OuterXml())

                'create udf in table
                eusUDFTable.CreateUnit(CStr(loopIdx))

                'load name and download attribute
                tempXPath = "/userDefinedFunction/name"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                eusUDFTable.UnitItem(CStr(loopIdx), "name") = Trim(tempNode.InnerXml())


                tempXPath = "/userDefinedFunction/name/@download"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                eusUDFTable.UnitItem(CStr(loopIdx), "dlUDF") = Trim(tempNode.InnerXml())

                'load units
                tempXPath = "/userDefinedFunction/units"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                eusUDFTable.UnitItem(CStr(loopIdx), "units") = Trim(tempNode.InnerXml())

                'load function body
                tempXPath = "/userDefinedFunction/body"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                eusUDFTable.UnitItem(CStr(loopIdx), "body") = Trim(tempNode.InnerXml())
            Next

            'load tables into return object
            expParsedSpec.AddItem("TermTable", eusTerminalTable)
            expParsedSpec.AddItem("UDFTable", eusUDFTable)
            expParsedSpec.AddItem("DeviceNum", strDeviceNum)

            Return expParsedSpec

        End Function
    End Class

End Namespace


