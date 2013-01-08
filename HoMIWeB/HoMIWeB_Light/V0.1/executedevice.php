<?php
	include ("./include_php/homisoap.php");

	$dom = new DomDocument();
	$dom->load('homidom-web-param.xml');
	$ListeServer = $dom->getElementsByTagName('SERVER'); // on récupère les paramètres du serveur
	$server = $ListeServer->item(0);
	$homidom = new HomidomSoap($server->getAttribute("IP"), $server->getAttribute("PORT"), $server->getAttribute("ID"),true);
	$homidom->connect(); 

	$ListeDevice = $homidom->GetAllDevices();
	$devicenom=$_POST['strNameDevice'];
	$position = array_search($devicenom,$ListeDevice);
	$deviceid=$ListeDevice[$position]->_ID;
	$actionnom=$_POST['strAction'];
	$actionparametre=$_POST['strActionParam'];
	$homidom->ExecuteDeviceCommand($deviceid,array('Nom'=>"$actionnom",'Parametres'=>"$actionparametre"));

	$homidom->disconnect();

?>
