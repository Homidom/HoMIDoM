<?php
		include ("./include_php/fonctions.php");
		include ("./include_php/homisoap.php");
		include ("./include_php/homiclass.php");
	
	$zoneid=$_GET['zoneid'];
		
	$dom = new DomDocument();
	$dom->load('homidom-web-param.xml');
	$ListeServer = $dom->getElementsByTagName('SERVER'); // on récupère les paramètres du serveur
	$server = $ListeServer->item(0);
	
	$homidom = new HomidomSoap($server->getAttribute("IP"), $server->getAttribute("PORT"), $server->getAttribute("ID"),true);
	$homidom->connect();	
	$homizone=$homidom->ReturnZoneByID($zoneid);
	
	
	$MonImage = $homidom->GetByteFromImage($homizone->_Image);
	header("Content-Type: image/jpeg");
	echo $MonImage;
?>
