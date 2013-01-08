<?php
		include ("./include_php/fonctions.php");
		include ("./include_php/homiclass.php");
		include ("./include_php/homisoap.php");


$dom = new DomDocument();
$dom->load('homidom-web-param.xml');
$ListeServer = $dom->getElementsByTagName('SERVER'); // on récupère les paramètres du serveur
$server = $ListeServer->item(0);
$homidom = new HomidomSoap($server->getAttribute("IP"), $server->getAttribute("PORT"), $server->getAttribute("ID"),true);
$homidom->connect(); 
$x=$homidom->GetAllZones(); // On récupère la liste des zones dans Homidom (SOAP)
$ListeZone = $dom->getElementsByTagName('ZONE'); // on récupère la liste des zones du fichier de configuration
  foreach($ListeZone as $zone)
  {
	foreach($x->Zone as $k=>$v) {
		if(utf8_decode($v->_Name) == utf8_decode($zone->getAttribute("NAME"))) {
			echo "<BR><A HREF=\"index.php?zoneid=" . $v->_Id . "\">\n";
			echo "<CENTER><FONT style=\"font-size:12px ;color: #FFFFFF\"\">" . utf8_decode($zone->getAttribute("NAME")) . "</CENTER><br>\n";
			echo "<CENTER><IMG SRC=\"images/zone-defaut.png\" HEIGHT=\"50\" WIDTH=\"50\" BORDER=\"0\"></CENTER></A><br>\n";
			}
		
	}
  }

?>
</BR>
<CENTER>
<!--<A HREF="index.php?zoneid=servercheck"><IMG SRC="images/verify.png" WIDTH="25" HEIGHT="25" BORDER="0"></A><BR><BR>-->
<!--<A HREF="index.php?zoneid=parametre"><IMG SRC="images/param.png" WIDTH="25" HEIGHT="25" BORDER="0"></A><BR>-->
</CENTER>
<?php
/*	
	$ListeHomiDevice = $homidom->GetAllDevices();
	$devicenom='SW-O-0';
	$position = array_search($devicenom,$ListeHomiDevice);
	$deviceid=$ListeHomiDevice[$position]->_ID;
	
	echo "<P>$deviceid</P>";
*/
?>