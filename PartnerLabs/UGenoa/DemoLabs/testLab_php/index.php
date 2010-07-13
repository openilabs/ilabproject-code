<?php
session_start();

require_once ('lib/labproxyclient.php');
echo "<hr>";
echo "<h1>This is a demo lab startpage</h1>";
echo "<hr>";
echo "| ";
echo '<a href="page1.php">ENTER LABORATORY</a>';
echo "   |   ";
echo '<a href="'.$_SESSION['sbUrl'].'">Back to SB </a>';
echo "   |   ";
echo '<a href="javascript:location.reload(true)">Refresh </a>';
echo "   |   ";
echo "<hr>";
echo "<br/> <b>The current time of the webserver is : " .date("Y-m-dTH:i:sZ")."</b><br/>or ".time()."<br/><br/>";
echo "Here is some Reservation Info retrieved from the LabProxy WS: <br/><br/>";
	
if(!LabProxyClient::isReservationValid())
{
echo "Make a new WS Call: <br/>";
//parameters are the current post and the URL of the webservice
LabProxyClient::RetrieveReservation('http://remlab-esng.dibe.unige.it/LPS_MIT/LabProxy.asmx');
}

//echo "EMPTY: ".$_SESSION['reservationExpiration']."<br/><br/>";

if(isset($_SESSION['reservationExpiration'])&& !empty($_SESSION['reservationExpiration']))
{
	echo '<b>Expiration of Reservation: </b>'.LabProxyClient::reservationExpiration()."<br/>";
	echo '<b>ExperimentID: </b>'.LabProxyClient::experimentID()."<br/>";
	echo '<b>GroupName: </b>'.LabProxyClient::groupName()."<br/>";	
	echo '<b>Session Valid: </b>'.LabProxyClient::isReservationValid()."<br/>";	
	
}
else
{
echo "<br/><b>Session not valid anymore</b>";
session_destroy();
}

?>


