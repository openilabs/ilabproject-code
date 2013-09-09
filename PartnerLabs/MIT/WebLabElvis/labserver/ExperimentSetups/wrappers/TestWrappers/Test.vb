Imports OpAmpInverter.InstrumentDriverInterop.Ivi
Imports System.Threading

Public Class Test
    Public Shared Sub Main()
        ''' Create a new instance of this class and call its methods

        Dim results()() As Double
        Dim i, j, k, m As Integer
        Dim frequency As Double
        Dim amplitude As Double
        Dim offset As Double = 0
        Dim waveformType As Short = 0
        Dim samplingRate As Double
        Dim SamplingTime As Double
        For i = 0 To 2
            Dim inverter As New Inverter
            amplitude = 0.1 * (i + 1)
            frequency = 10 ^ (i + 1)
            samplingRate = frequency * 10
            SamplingTime = 0.2
            waveformType = 0
            results = inverter.RunExperiment(frequency, amplitude, offset, waveformType, samplingRate, SamplingTime)
            inverter.Dispose()
            inverter = Nothing
            Console.WriteLine("The params are: F = " & frequency & " \n amplitude = " & amplitude & " offset = " & offset & " waveformType = " & waveformType & " rate = " & samplingRate & " time = " & SamplingTime)
            '' print out the result strings to the console
            'Thread.Sleep(1000)

            Console.WriteLine("The length of the results array is " & UBound(results(0)))
            For j = 0 To UBound(results) - 1
                For k = 0 To UBound(results(j)) - 1
                    Console.Write(results(j)(k) & "  ")
                Next
                Console.WriteLine()
            Next

            'Thread.Sleep(1000)
        Next
    End Sub

End Class
