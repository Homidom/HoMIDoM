<?php


if(!empty($_POST)) {

	/* On se crée un nouveau XML avec les informations passé dans le post */
	$dom = new DOMDocument();

	$SERVER_IP = $_POST['SERVER_IP']; 
	$SERVER_PORT = $_POST['SERVER_PORT']; 
	$SERVER_ID = $_POST['SERVER_ID']; 
		
	$NewParam = $dom->createElement('PARAM');
	$NewServer = $dom->createElement('SERVER');
	
	$NewServer->setAttribute("IP",$SERVER_IP);
	$NewServer->setAttribute("PORT",$SERVER_PORT);
	$NewServer->setAttribute("ID",$SERVER_ID);
	
	$dom->appendChild($NewParam);
	$NewParam->appendChild($NewServer);
	
	
	

if(!empty($_POST['ZONE_ID'])) {
	foreach($_POST['ZONE_ID'] as $ZoneId)
		{
			$NewZone = $dom->createElement('ZONE');
			$NewZone->setAttribute("ID",$ZoneId);
			$NewServer->appendChild($NewZone);
			foreach($_POST[$ZoneId . 'DEVICE_ID'] as $DeviceId)
			{
				$NewDevice = $dom->createElement('HOMIDEVICE');
				$NewDevice->setAttribute('ID',$DeviceId);

				// On récupère tous les attibuts de l'objet Device
				$POS_X = $_POST[$ZoneId . '_' . $DeviceId . '_X'];
				$NewDevice->setAttribute('POSX',utf8_encode($POS_X));
				$POS_Y = $_POST[$ZoneId . '_' . $DeviceId . '_Y'];
				$NewDevice->setAttribute('POSY',utf8_encode($POS_Y));
				$ICON_L = $_POST[$ZoneId . '_' . $DeviceId . '_ICON_L'];
				$NewDevice->setAttribute('WIDTH',utf8_encode($ICON_L));
				$ICON_H = $_POST[$ZoneId . '_' . $DeviceId . '_ICON_H'];
				$NewDevice->setAttribute('HEIGHT',utf8_encode($ICON_H));
				$ICON = $_POST[$ZoneId . '_' . $DeviceId . '_ICON'];
				$NewDevice->setAttribute('ICON',utf8_encode($ICON));

				if($POS_X and $POS_Y) // Si POS_X et POS_Y alors on peut enregistrer le device
				{
					$NewZone->appendChild($NewDevice);
				
					$Compteur = 0;
					foreach($_POST[$ZoneId . '_' . $DeviceId . '_ETAT_VALUE'] as $EtatValue)
					{
						$NewEtat = $dom->createElement('ETAT');
						$NewEtat->setAttribute('VALUE',$EtatValue);

						// On récupère tous les attibuts de l'objet Etat
						$ETAT_H = $_POST[$ZoneId . '_' . $DeviceId . '_ETAT_H'][$Compteur];
						$NewEtat->setAttribute('HEIGHT',utf8_encode($ETAT_H));
						$ETAT_L = $_POST[$ZoneId . '_' . $DeviceId . '_ETAT_L'][$Compteur];
						$NewEtat->setAttribute('WIDTH',utf8_encode($ETAT_L));
						$IMG = $_POST[$ZoneId . '_' . $DeviceId . '_ETAT_IMG'][$Compteur];
						$NewEtat->setAttribute('IMG',utf8_encode($IMG));

						if($EtatValue or $ETAT_H or $ETAT_L or $IMG) // on teste si des données sont renseignées
							$NewDevice->appendChild($NewEtat);
						$Compteur = $Compteur + 1;
					}
				}
			}
		}
	}

/* On enregistre le tout */
$dom->save('homidom-web-param.xml');

/* On retourne à index.php ; le nouveau fichier sera lu */
header('Location: index.php?zoneid=Parametres'); 
	
}
?>
