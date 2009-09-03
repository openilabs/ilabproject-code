set wsdlpath="C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin"
%wsdlpath%\wsdl /v /sharetypes /f /n:Library.LabServer.Drivers.Equipment /o:LibraryLabServer\Drivers\Equipment\ProxyRadioactivityService.cs http://localhost:8087/Radioactivity.asmx?WSDL
