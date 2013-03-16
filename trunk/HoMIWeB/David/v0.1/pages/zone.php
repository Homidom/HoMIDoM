<?php
	if(isset($_SESSION['server_valid'])){ 
		$zoneid=$_GET["zoneid"];
?>
<div>
<H1>ZONE</H1>
<?php

	//Recherche de la zone dans le XML
	$dom = new DomDocument();
	$dom->load('include/zones.xml');
	$XML_ListZone = $dom->getElementsByTagName('ZONE');
	$XML_Zone_Composants="";
	foreach($XML_ListZone as $XML_Zone) {
		if($XML_Zone->getAttribute("ID") == $zoneid) {
			$XML_Zone_Composants = $XML_Zone->getElementsByTagName('COMPOSANT');
			break;
		}
	}

	//On recupere les devices de la zone en SOAP
	$x=$homidom->GetDeviceInZone($zoneid)->TemplateDevice;
	for($i = 0; $i < count($x); $i++){
		//on recherche chaque composant recupéré en SOAP dans la base xml
		$XML_Composant="";
		foreach($XML_Zone_Composants as $XML_Zone_Composant) {
			if($XML_Zone_Composant->getAttribute("ID") == $x[$i]->_ID) {
				$XML_Composant = $XML_Zone_Composant;
				break;
			}
		}
		If ($XML_Composant<>"") {
			//On a trouvé le composant dans notre XML
			echo "<DIV>";
			//echo $XML_Composant->getAttribute("ID")." --> ".$XML_Composant->getAttribute("Name")
			//$x[$i]->_ID;_Name;_Type;_Adresse1;_Adresse2;_Value;_Refresh;_Description;_DriverID;_Enable;_Correction;_DateCreated;_Formatage;_LastChange;_LastChangeDuree;_LastEtat;_Modele;_Picture;_Precision;_Solo;_ValueDef;_ValueLast;_ValueMax;_ValueMin;_ConditionActuel;_HumActuel;_IconActuel;_TempActuel;_VentActuel;
			echo "".$x[$i]->_ID." --> ".$x[$i]->_Name." (".$x[$i]->_Type."-".$x[$i]->_Adresse1.") = ".$x[$i]->_Value." ==> ".$XML_Composant->getAttribute("POSX")."/".$XML_Composant->getAttribute("POSY")."---".$x[$i]->_ValueMin."*".$x[$i]->_ValueMax;
			echo "</DIV>";
		} else {
			//C'est un nouveau composant, on l'ajoute à la zone (sauf si on gere déjà la maj de la liste dans connexion.php

		}
	}
?>
</div>
<?php
	}else{
		header("location:../index.html");
	}
?>