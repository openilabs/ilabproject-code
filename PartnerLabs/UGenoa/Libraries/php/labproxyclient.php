<?php
session_start();

require_once ('nusoap.php');

/**
* This class can be used to connect to your interactive LabServer which is acting like a proxy.
* Author: Niederstätter Michael
* Date: 28.04.2010
*/

class labProxyClient
{

	var $sbUrl;
	
	var $reservationExpiration;
	var $experimentID;
	var $groupName;

	public static function RetrieveReservation($webServiceUrl)
	{
	
		$_SESSION['passkey'] = $_GET["passkey"];
		$_SESSION['sbUrl'] = $_GET["sbUrl"];
		
		if(isset($_SESSION['passkey']) && isset($webServiceUrl))
		{
		// ############# Call Webservice ###############
		// define the soapaction as found in the wsdl - yournamespase/webservicename
		$soapaction = "http://ilab.mit.edu/iLabs/Services/GetReservationInfo";
		// Path to .asmx file
		$wsdl=$webServiceUrl;

		// Crete a new SOAP client
		$client = new nusoap_client($wsdl);
		$mysoapmsg = $client->serializeEnvelope('
		<GetReservationInfo xmlns="http://ilab.mit.edu/iLabs/Services" >
		<passkey>'.$_SESSION['passkey'].'</passkey>
		<labServerTime>'.gmdate("Y-m-d")."T".gmdate("H:i:s.u")."Z".'</labServerTime>
		</GetReservationInfo>',
		'', array (), 'document', 'literal
		');
		$_SESSION['wscall'] = gmdate("Y-m-d")."T".gmdate("H:i:s.u")."Z";
		$response = $client->send($mysoapmsg, $soapaction);
		//echo $client->responseData;
		// if some failure occure
		if ($client->fault) {
			return false;
		}

		$xmlresponse = $response['GetReservationInfoResult'];
		$reservation = simplexml_load_string($xmlresponse);
		if($reservation->ReservationExpiration!="")
		$_SESSION['reservationExpiration'] = (string) strtotime($reservation->ReservationExpiration);

		$_SESSION['experimentID'] = (string) $reservation->ExperimentID;
		$_SESSION['groupName'] = (string) $reservation->GroupName;
		
		//$data = '2010-04-28T12:20:00.0000000Z';
		//$reservationStart = date('Y-m-dTH:i:sZ',strtotime($data);
		
		return true;
		}
		else
		{
			return false;
		}
	}
		
	public static function isReservationValid()
	{
		//check whether the passkey which was posted is still the same
		//if not destroy session because maybe user changed reservation
		if(isset($_GET["passkey"]) && isset($_SESSION['passkey']))
		{
			if($_SESSION['passkey']!=$_GET["passkey"])
			{
			session_destroy();
			return false;
			}
		}
		// check if session is still in time period
		if(isset($_SESSION['reservationExpiration']))
				{
					if(time()<$_SESSION['reservationExpiration'])
					return true;
					else
					return false;
				}
				else
				return false;
		
	}
	
	public static function reservationExpiration()
	{
		return $_SESSION['reservationExpiration'];
	}
	
	public static function experimentID()
	{
		return $_SESSION['experimentID'];
	}
	
	public static function groupName()
	{
		return $_SESSION['groupName'];
	}
	
	
}
?>	
	