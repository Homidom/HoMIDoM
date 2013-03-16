<?php
	session_start();
	include ("./include/fonctions.php");
	include ("./include/homiclass.php");
	include ("./include/homisoap.php");
		
	//on verifie si on arrive depuis le formulaire
	if(isset($_POST['ident_ip'])){
		$server_ip=$_POST['ident_ip'];
		$server_port=$_POST['ident_port'];
		$server_id=$_POST['ident_id'];
	} else {
		$server_ip=$_SESSION['server_ip'];
		$server_port=$_SESSION['server_port'];
		$server_id=$_SESSION['server_id'];
	}

	$homidom = new HomidomSoap($server_ip, $server_port, $server_id,true);
	//on se connecte au serveur SOAP puis on verifie l'ID
	if(!($homidom->connect())) {
		//non connecté : ERREUR
		$msg=htmlentities("Connexion impossible au serveur (".$server_ip." ".$server_port.")");
		Header("location:./index.html?msg=$msg");
	} else {
		//connecté, on vérifie l'ID
		if($homidom->GetIdServer() == '99') {
			$homidom->disconnect();
			//Id Incorrect : ERREUR
			$msg=htmlentities("L'ID du serveur est incorrect : ".$server_id." ");
			Header("location:./index.html?msg=$msg");
		} else {
			session_start();

			//on stocke dans la session les infos
			$_SESSION['server_valid']="OK";
			$_SESSION['server_ip']=$server_ip;
			$_SESSION['server_port']=$server_port;
			$_SESSION['server_id']=$server_id;

			//on enregistre la bdd de connexion.xml
			$dom = new DOMDocument();
			$NewParam = $dom->createElement('PARAM');
			$NewServer = $dom->createElement('SERVER');
			$NewServer->setAttribute("IP",$server_ip);
			$NewServer->setAttribute("PORT",$server_port);
			$NewServer->setAttribute("ID",$server_id);
			$dom->appendChild($NewParam);
			$NewParam->appendChild($NewServer);
			$dom->save('include/connexion.xml');

			//On gere les zones et leurs composants
			$dom = new DomDocument();
			$FicExist = @$dom->load('include/zones.xml');
			//si le fichier xml n'existe pas déjà, on recupere les infos des zones en SOAP
			if(!$FicExist) {
				//le fichier xml n'existe pas, on récupére les infos en SOAP et on enregistre le XML
				$ListeZones=$homidom->GetAllZones(); // On récupère la liste des zones dans Homidom (SOAP)
				//$ListeHomiZone->Zone = array($ListeHomiZone->Zone);
				$dom = new DOMDocument();
				$NewZones = $dom->createElement('ZONES');
				$dom->appendChild($NewZones);
				foreach($ListeZones->Zone as $k=>$v) {
					// On ajoute la zone dans le XML
					$NewZone = $dom->createElement('ZONE');
					$NewZone->setAttribute("IMAGE","images/zone-defaut.png");
					$NewZone->setAttribute("NAME",$v->_Name);
					$NewZone->setAttribute("ID",$v->_Id);
					$NewZones->appendChild($NewZone);
					//Pour Chaque zone, on recupere ces composants
					$ListeComposants=$homidom->GetDeviceInZone($v->_Id)->TemplateDevice;
					for($i = 0; $i < count($ListeComposants); $i++){
						$NewComposant = $dom->createElement('COMPOSANT');
						$NewComposant->setAttribute("LASTCHANGE",$ListeComposants[$i]->_LastChange);
						$NewComposant->setAttribute("VALUE",$ListeComposants[$i]>_Value);
						$NewComposant->setAttribute("ADRESSE1",$ListeComposants[$i]->_Adresse1);
						$NewComposant->setAttribute("IMAGE"," ");
						$NewComposant->setAttribute("TYPE",$ListeComposants[$i]->_Type);
						$NewComposant->setAttribute("ENABLE",$ListeComposants[$i]->_Enable);
						$NewComposant->setAttribute("NAME",$ListeComposants[$i]->_Name);
						$NewComposant->setAttribute("POSY","0");
						$NewComposant->setAttribute("POSX","0");
						$NewComposant->setAttribute("ID",$ListeComposants[$i]->_ID);
						$NewZone->appendChild($NewComposant);
					}
				}
				$dom->save('include/zones.xml');
			} else {
				//on met à jour le XML pour supprimer les composants qui n'existe plus et ajouter les nouveaux
				
				
				
				
				
				
				
				
			}
			//on va vers la bonne page
			Header("location:./homiweb.html");
		}
	}

?>