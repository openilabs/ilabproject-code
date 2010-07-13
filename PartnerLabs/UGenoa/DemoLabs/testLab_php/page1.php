<?php
session_start();
require_once ('lib/labproxyclient.php');

echo "<h1>This is a demo page of a laboratory</h1>";
echo "<hr>";
echo "|   ";
echo '<a href="index.php">Back to startpage</a>';
echo "   |   ";
echo '<a href="javascript:location.reload(true)">Refresh</a>';
echo "   |   ";
echo "<hr>";
echo "<br>";
if(LabProxyClient::isReservationValid())
{
echo "<p>Hello User - u can continue with experimenting....</p>";
echo "<img width=\"618\" height=\"464\" src=\"http://impulseportal.com/images/repair%20lab.jpg\">";
echo "<p>Your registration expires at:".LabProxyClient::reservationExpiration()."</p>";
echo "<p>Time Now:".time()."</p>";

}
else
{
echo "<p>Sorry - session already expired!.</p>";
echo '<br/></br><br/></br> <a href="' . $_SESSION['sbUrl'] . '">Back to SB </a>';
session_destroy();
}
?>

