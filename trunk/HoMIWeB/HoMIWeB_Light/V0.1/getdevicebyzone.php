<?php

	include ("./include_php/fonctions.php");
	include ("./include_php/homisoap.php");
	include ("./include_php/homiclass.php");

if(!empty($_POST)) {
	$zoneid=$_POST['zoneid'];	
	$dom = new DomDocument();
	$dom->load('homidom-web-param.xml');
	$ListeServer = $dom->getElementsByTagName('SERVER'); // on récupère les paramètres du serveur
	$server = $ListeServer->item(0);
	$homidom = new HomidomSoap($server->getAttribute("IP"), $server->getAttribute("PORT"), $server->getAttribute("ID"),true);
	$homidom->connect();
	
	header('Content-Type: text/xml');
	echo '<?xml version="1.0"?>';
	echo "<DATA>";
	$x=$homidom->GetDeviceInZone($zoneid)->TemplateDevice;
	for($i = 0; $i < count($x); $i++){
			echo "<HOMIDEVICE>";

			echo "<ID>";
			echo $x[$i]->_ID;
			echo "</ID>";

			echo "<NAME>";
			echo $x[$i]->_Name;
			echo "</NAME>";
			
// 			$x[$i]->_Type
// 			$x[$i]->_Adresse1
// 			$x[$i]->_Adresse2
			echo "<VALUE>";
			echo $x[$i]->_Value;
			echo "</VALUE>";
// 			$x[$i]->_Refresh
// 			$x[$i]->_Description
// 			$x[$i]->_DriverID
// 			$x[$i]->_Enable
// 			$x[$i]->_Correction
// 			$x[$i]->_DateCreated
// 			$x[$i]->_Formatage
//echo $x[$i]->_LastChange;
// 			$x[$i]->_LastChangeDuree
			echo "<LASTETAT>";
			echo $x[$i]->_LastEtat;
			echo "</LASTETAT>";
// 			$x[$i]->_Modele
// 			$x[$i]->_Picture
// 			$x[$i]->_Precision
// 			$x[$i]->_Solo
// 			$x[$i]->_ValueDef
//			$x[$i]->_ValueLast;
// 			$x[$i]->_ValueMax
// 			$x[$i]->_ValueMin
// 			$x[$i]->_ConditionActuel
// 			$x[$i]->_HumActuel
// 			$x[$i]->_IconActuel
// 			$x[$i]->_TempActuel
// 			$x[$i]->_VentActuel
			echo "</HOMIDEVICE>";			
		}
// Envoi de l'heure systeme
		echo "<HOMIDEVICE>";
		echo "<ID>"."IDHeureCourante"."</ID>";
		echo "<NAME>"."Heure courante"."</NAME>";
		echo "<VALUE>".$homidom->GetTime()."</VALUE>";
		echo "</HOMIDEVICE>";

		
		echo "</DATA>";
	$homidom->disconnect();
}
?>
