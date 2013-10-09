Imports System
Imports System.Net
Imports System.Security.Cryptography.x509Certificates


Namespace SiteComponents

    'Author(s): James Hardison (hardison@alum.mit.edu)
    'Date: 1/22/2004
    'This class defines an ICertificatePolicy to be used with modules acting as a client to the WebLab Lab Server.  
    'Specifically, this policy validates the server certificate installed on the Lab Server (issued to weblab2.mit.edu
    'from the MIT Certification Authority) so that the client module can establish a trusted SSL connection with the Lab Server;s
    'Web Service interface.  When applying to a client module, this policy should be set in place of the default policy 
    'at System.Net.ServicePointManager.CertificatePolicy

Public Class WebLabCertPolicy 
    Implements ICertificatePolicy

	Public Function CheckValidationResult(ByVal srvPoint As ServicePoint, ByVal certificate As X509Certificate, ByVal request As Webrequest, ByVal certificateProblem As Integer) As Boolean Implements ICertificatePolicy.CheckValidationResult
        Dim strIssuer, strSubject As String

        strIssuer = certificate.GetIssuerName()
        strSubject = certificate.GetName()

        If strIssuer = "C=US, S=Massachusetts, O=Massachusetts Institute of Technology, OU=MIT Certification Authority" And strSubject = "C=US, S=Massachusetts, L=Cambridge, O=Massachusetts Institute of Technology, OU=Microsystems Technology Labs, CN=weblab.mit.edu" Then
            Return True
        Else
            Return False    
        End If
	End Function

End Class

End Namespace