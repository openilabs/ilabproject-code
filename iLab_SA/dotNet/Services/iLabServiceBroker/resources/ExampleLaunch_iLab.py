""" Example iLabClientMetadata for Use with gateway4Labs

<iLabClientMetadata xmlns:ns="http://ilab.mit.edu/iLab">
    <authCouponId>246</authCouponId>                                #Authenticate_Client Ticket collection coupon
    <authIssuer>ISB-247A4591CA1443485D85657CF357</authIssuer>
    <authPasskey>C518F6E888A44E68B583C46520A27679</authPasskey>
    <host>http://ludi.mit.edu</host>
    <webService>/iLabServiceBroker/iLabServiceBroker.asmx</webService>
    <webMethod>LanuchLabClient</webMethod>
    <clientName>MIT Microelectronics Device Characterization iLab Client v7.1</clientName>
    <clientGuid>6BD4E985-4967-4742-854B-D44B8B844A21</clientGuid>
    <groupName>Experiment_Group</groupName>
</iLabClientMetadata>

"""

# Python method to request WebLab client & experiment

import sys, httplib
import xml.etree.ElementTree as ET

# from scorm or other format provided by the ServiceBroker
# see example metadata above

# ServuiceBroker Info
host= "ludi.mit.edu"
url = "/iLabServiceBroker/iLabServiceBroker.asmx"
sbGuid = "ISB-247A4591CA1443485D85657CF357"

# Client specific Info
# WebLab Micro-electronics -- Batch Applet lab
webLabGuid = "6BD4E985-4967-4742-854B-D44B8B844A21"
webLabCouponID = "246" 
webLabPassCode = "C518F6E888A44E68B583C46520A27679"
groupName = "Experiment_Group"

#Specific to LMS or other SerrviceBroker registered authority
# This requires that an Authority has been registered on the target Servicebroker
authorityGuid = "fakeGUIDforRMLStest-12345"
authorityPassCode = "193BE47E31C4E3BADA155B19A6EB528"

#from LMS/Gateway runtime-environment
userName = "lmsuser"

#########

duration = "-1"

couponID = webLabCouponID
passkey = webLabPassCode
clientGuid = webLabGuid

soapXml = '''<?xml version="1.0" encoding="utf-8"?>
        <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
        xmlns:xsd="http://www.w3.org/2001/XMLSchema"
        xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
            <soap12:Header>
                <OperationAuthHeader xmlns="http://ilab.mit.edu/iLabs/type">
                    <coupon  xmlns="http://ilab.mit.edu/iLabs/type">
                        <couponId>''' + couponID + '''</couponId>
                        <issuerGuid>''' + sbGuid + '''</issuerGuid>
                        <passkey>''' + passkey +'''</passkey>
                    </coupon>
                </OperationAuthHeader>
            </soap12:Header>
            <soap12:Body>
                <LaunchLabClient xmlns="http://ilab.mit.edu/iLabs/Services">
                    <clientGuid>''' + clientGuid +'''</clientGuid>
                    <groupName>''' + groupName + '''</groupName>
                    <userName>''' + userName +'''</userName>
                    <authorityKey>''' + authorityGuid + '''</authorityKey>
                    <duration>''' + duration + '''</duration><autoStart>1</autoStart>
                </LaunchLabClient>
            </soap12:Body>
        </soap12:Envelope>'''

#make connection
ws = httplib.HTTP(host)
ws.putrequest("POST", url)

#add headers
ws.putheader("Content-Type", "application/soap+xml; charset=utf-8")
ws.putheader("Content-Length", "%d"%(len(soapXml),))
ws.endheaders()

#send request
ws.send(soapXml)

#get response
statuscode, statusmessage, header = ws.getreply()
print "Response: ", statuscode, statusmessage
print "headers: ", header
res = ws.getfile().read()
print res, "\n"
root = ET.fromstring(res)
tag = root[0][0][0][1].text
print tag

#<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><soap:Body><LaunchLabClientResponse xmlns="http://ilab.mit.edu/iLabs/Services"><LaunchLabClientResult xmlns="http://ilab.mit.edu/iLabs/type"><id>1</id><tag>http://ceci-pc-29.mit.edu/iLabServiceBroker/default.aspx?sso=t&amp;usr=test&amp;cid=AB3904BAF6345D5979C6D85EDB5460E&amp;grp=Experiment_Group&amp;auth=fakeGUIDforRMLStest-12345&amp;key=8F930961E4084153BE40AD9844AC5C69&amp;auto=t</tag></LaunchLabClientResult></LaunchLabClientResponse></soap:Body></soap:Envelope>
