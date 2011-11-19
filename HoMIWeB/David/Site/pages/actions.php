<?php
include("../include_php/config.php");
include("../include_php/fonctions.php");
$zones=!empty($_REQUEST["zones"])?$_REQUEST["zones"]:"1";
$tache=!empty($_REQUEST["tache"])?$_REQUEST["tache"]:"releve";

switch ($tache) {
case "releve" :
	header("Pragma: no-cache");
	header("Expires: 0");
	header("Last-Modified: " . gmdate("D, d M Y H:i:s") . " GMT");
	header("Cache-Control: no-cache, must-revalidate");
	header("Content-type: application/xml"); 
	$xml="<resultats>\n";
	$request="select * from devices, zones, devices_zones where zones_id='$zones' and devices_visible=\"1\" and devices_id=devices_zones_devicesid and zones_id==devices_zones_zonesid";
	$resultat = mysql_query($request);
	$num = mysql_num_rows($resultat);
	for ($i=0;$i<$num;$i++) {
		$comp = mysql_result($resultat,$i,"devices_id");
		$compnom = mysql_result($resultat,$i,"devices_nom");
		$type = mysql_result($resultat,$i,"devices_type");
		$pos_top = mysql_result($resultat,$i,"devices_zones_coordy");
		$pos_left = mysql_result($resultat,$i,"devices_zones_coordx");
		$valeur = mysql_result($resultat,$i,"devices_valeur");
		$datemaj = mysql_result($resultat,$i,"devices_datemaj");
		$xml.= "<resultat type=\"".$type."\" comp=\"$comp\" compnom=\"".dequote($compnom)."\" top=\"$pos_top\" left=\"$pos_left\" valeur=\"$valeur\" datemaj=\"$datemaj\" />\n";
	}
	$xml.="</resultats>";
	echo utf8_encode($xml);
	break;

case "socket" :
	$message=!empty($_REQUEST["message"])?$_REQUEST["message"]:"([AL#Erreurtransmission])";
	$adresse=!empty($_REQUEST["adresse"])?$_REQUEST["adresse"]:"192.168.1.20";
	$port=!empty($_REQUEST["port"])?$_REQUEST["port"]:"3852";
	socket($message,$adresse,$port);
	break;

case "sql" :
	$message=!empty($_REQUEST["message"])?$_REQUEST["message"]:"([AL#Erreurtransmission])";
	mysql_query($message);
	echo "OK";
	break;

case "dump" :
	$detail=!empty($_REQUEST["detail"])?$_REQUEST["detail"]:"";
	$compression=!empty($_REQUEST["compression"])?$_REQUEST["compression"]:"false";
	switch ($detail) {
		case "composantreleve" :
			$composants_id=!empty($_REQUEST["composants_id"])?$_REQUEST["composants_id"]:"1";
			if ($compression=="true") {
				$backup_file = '../backup/SQL_domos_releve_composant_'.$composants_id.'_'.date("Ymd").'.sql.gz';
				$fp = gzopen($backup_file, 'w');
			} else {
				$backup_file = '../backup/SQL_domos_'.$composants_id.'_'.date("Ymd").'.sql';
				$fp = fopen($backup_file, 'w');
			}
			$result=mysql_query("SHOW COLUMNS FROM releve");
			$num_results=mysql_numrows($result);
			$insertions_debut = "INSERT INTO `releve` (";
			while($row=mysql_fetch_array($result)){if ($row[0]<>"releve_id") {$insertions_debut .= "`".$row[0]."`, ";};}
			$insertions_debut = substr($insertions_debut, 0, -2);
			$insertions_debut .= ") VALUES";
			$req_table = mysql_query("SELECT * FROM releve WHERE releve_composants=".$composants_id) or die(mysql_error());
			$nbr_champs = mysql_num_fields($req_table);
			$nbr_ligne = mysql_num_rows($req_table);
			$j=1;
			while ($ligne = mysql_fetch_array($req_table)) {
				$insertions = "(";
				for ($i=1; $i<$nbr_champs; $i++) {$insertions .= "`" . mysql_real_escape_string($ligne[$i]) . "`, ";}
				$insertions = substr($insertions, 0, -2);
				$insertions .= ");\n";
				if ($compression=="true") {gzwrite($fp, $insertions_debut.$insertions);} else {fwrite($fp, $insertions_debut.$insertions);}
				$j=$j+1;
			}
			if ($compression=="true") {gzclose($fp);} else {fclose($fp);}
			echo "OK";
			break;

		case "complet" :
			//exec("mysqldump –opt –host=localhost –databases=domos –user=domos –password=domos > ../Domos.sql");
			$tables = mysql_query('SHOW TABLES FROM domos') or die(mysql_error());
			if ($compression=="true") {$backup_file = '../backup/SQL_domos_complet_'.date("Ymd").'.sql.gz';$fp = gzopen($backup_file, 'w');} else {$backup_file = '../backup/SQL_domos_complet_'.date("Ymd").'.sql';$fp = fopen($backup_file, 'w');}
			while ($donnees = mysql_fetch_array($tables)) {
				$table = $donnees[0];
				if ($compression=="true") {gzwrite($fp, "DROP TABLE $table;\n");} else {fwrite($fp, "DROP TABLE $table;\n");}
				$sql = 'SHOW CREATE TABLE '.$table;
				$res = mysql_query($sql) or die(mysql_error().$sql);
				if ($res) {
					$tableau = mysql_fetch_array($res);
					$tableau[1] .= ";\n";
					$insertions = $tableau[1];
					if ($compression=="true") {gzwrite($fp, $insertions);} else {fwrite($fp, $insertions);}
					$result=mysql_query('SHOW COLUMNS FROM '.$table);
					$num_results=mysql_numrows($result);
					$insertions_debut = 'INSERT INTO `'.$table.'` (';
					while($row=mysql_fetch_array($result)){$insertions_debut .= '`'.$row[0].'`, ';}
					$insertions_debut = substr($insertions_debut, 0, -2);
					$insertions_debut .= ") VALUES\n";
					if ($compression=="true") {gzwrite($fp, $insertions_debut);} else {fwrite($fp, $insertions_debut);}
					$req_table = mysql_query('SELECT * FROM '.$table) or die(mysql_error());
					$nbr_champs = mysql_num_fields($req_table);
					$nbr_ligne = mysql_num_rows($req_table);
					$j=1;
					while ($ligne = mysql_fetch_array($req_table)) {
						$insertions = '(';
						for ($i=0; $i<$nbr_champs; $i++) {$insertions .= '\'' . mysql_real_escape_string($ligne[$i]) . '\', ';}
						$insertions = substr($insertions, 0, -2);
						if ($j<$nbr_ligne) {$insertions .= "),\n";} else {$insertions .= ");\n";};
						if ($compression=="true") {gzwrite($fp, $insertions);} else {fwrite($fp, $insertions);}
						$j=$j+1;
					}
				}
				mysql_free_result($res);
			}
			if ($compression=="true") {gzclose($fp);} else {fclose($fp);}
			echo "OK";
			break;
		case "complet_insertioncomplete" :
			//exec("mysqldump –opt –host=localhost –databases=domos –user=domos –password=domos > ../Domos.sql");
			$tables = mysql_query('SHOW TABLES FROM domos') or die(mysql_error());
			if ($compression=="true") {$backup_file = '../backup/SQL_domos_insertcomplet_'.date("Ymd").'.sql.gz';$fp = gzopen($backup_file, 'w');} else {$backup_file = '../backup/SQL_domos_insertcomplet_'.date("Ymd").'.sql';$fp = fopen($backup_file, 'w');}
			while ($donnees = mysql_fetch_array($tables)) {
				$table = $donnees[0];
				if ($compression=="true") {gzwrite($fp, "DROP TABLE $table;\n");} else {fwrite($fp, "DROP TABLE $table;\n");}
				$sql = 'SHOW CREATE TABLE '.$table;
				$res = mysql_query($sql) or die(mysql_error().$sql);
				if ($res) {
					$tableau = mysql_fetch_array($res);
					$tableau[1] .= ";\n";
					$insertions = $tableau[1];
					if ($compression=="true") {gzwrite($fp, $insertions);} else {fwrite($fp, $insertions);}
					$req_table = mysql_query('SELECT * FROM '.$table) or die(mysql_error());
					$nbr_champs = mysql_num_fields($req_table);
					while ($ligne = mysql_fetch_array($req_table)) {
						$insertions = 'INSERT INTO '.$table.' VALUES (';
						for ($i=0; $i<$nbr_champs; $i++) {$insertions .= '\'' . mysql_real_escape_string($ligne[$i]) . '\', ';}
						$insertions = substr($insertions, 0, -2);
						$insertions .= ");\n";
						if ($compression=="true") {gzwrite($fp, $insertions);} else {fwrite($fp, $insertions);}
					}
				}
				mysql_free_result($res);
			}
			if ($compression=="true") {gzclose($fp);} else {fclose($fp);}
			echo "OK";
			break;
		case "completsanslogs" :
			$tables = mysql_query('SHOW TABLES FROM domos') or die(mysql_error());
			if ($compression=="true") {$backup_file = '../backup/SQL_domos_complet_sanslogs_'.date("Ymd").'.sql.gz';$fp = gzopen($backup_file, 'w');} else {$backup_file = '../backup/SQL_domos_complet_sanslogs_'.date("Ymd").'.sql';$fp = fopen($backup_file, 'w');}
			while ($donnees = mysql_fetch_array($tables)) {
				$table = $donnees[0];
				if ($compression=="true") {gzwrite($fp, "DROP TABLE $table;\n");} else {fwrite($fp, "DROP TABLE $table;\n");}
				$sql = 'SHOW CREATE TABLE '.$table;
				$res = mysql_query($sql) or die(mysql_error().$sql);
				if ($res) {
					$tableau = mysql_fetch_array($res);
					$tableau[1] .= ";\n";
					$insertions = $tableau[1];
					if ($compression=="true") {gzwrite($fp, $insertions);} else {fwrite($fp, $insertions);}
					$result=mysql_query('SHOW COLUMNS FROM '.$table);
					$num_results=mysql_numrows($result);
					$insertions_debut = 'INSERT INTO `'.$table.'` (';
					while($row=mysql_fetch_array($result)){$insertions_debut .= '`'.$row[0].'`, ';}
					$insertions_debut = substr($insertions_debut, 0, -2);
					$insertions_debut .= ") VALUES\n";
					if ($compression=="true") {gzwrite($fp, $insertions_debut);} else {fwrite($fp, $insertions_debut);}
					if ($table<>'logs') {
						$req_table = mysql_query('SELECT * FROM '.$table) or die(mysql_error());
						$nbr_champs = mysql_num_fields($req_table);
						$nbr_ligne = mysql_num_rows($req_table);
						$j=1;
						while ($ligne = mysql_fetch_array($req_table)) {
							$insertions = '(';
							for ($i=0; $i<$nbr_champs; $i++) {$insertions .= '\'' . mysql_real_escape_string($ligne[$i]) . '\', ';}
							$insertions = substr($insertions, 0, -2);
							if ($j<$nbr_ligne) {$insertions .= "),\n";} else {$insertions .= ");\n";};
							if ($compression=="true") {gzwrite($fp, $insertions);} else {fwrite($fp, $insertions);}
							$j=$j+1;
						}
					}
				}
				mysql_free_result($res);
			}
			if ($compression=="true") {gzclose($fp);} else {fclose($fp);}
			echo "OK";
			break;
		case "completsanslogsreleves" :
			$tables = mysql_query('SHOW TABLES FROM domos') or die(mysql_error());
			if ($compression=="true") {$backup_file = '../backup/SQL_domos_complet_sanslogsreleves_'.date("Ymd").'.sql.gz';$fp = gzopen($backup_file, 'w');} else {$backup_file = '../backup/SQL_domos_complet_sanslogsreleves_'.date("Ymd").'.sql';$fp = fopen($backup_file, 'w');}
			while ($donnees = mysql_fetch_array($tables)) {
				$table = $donnees[0];
				if ($compression=="true") {gzwrite($fp, "DROP TABLE $table;\n");} else {fwrite($fp, "DROP TABLE $table;\n");}
				$sql = 'SHOW CREATE TABLE '.$table;
				$res = mysql_query($sql) or die(mysql_error().$sql);
				if ($res) {
					$tableau = mysql_fetch_array($res);
					$tableau[1] .= ";\n";
					$insertions = $tableau[1];
					if ($compression=="true") {gzwrite($fp, $insertions);} else {fwrite($fp, $insertions);}
					$result=mysql_query('SHOW COLUMNS FROM '.$table);
					$num_results=mysql_numrows($result);
					$insertions_debut = 'INSERT INTO `'.$table.'` (';
					while($row=mysql_fetch_array($result)){$insertions_debut .= '`'.$row[0].'`, ';}
					$insertions_debut = substr($insertions_debut, 0, -2);
					$insertions_debut .= ") VALUES\n";
					if ($compression=="true") {gzwrite($fp, $insertions_debut);} else {fwrite($fp, $insertions_debut);}
					if ($table<>'logs' && $table<>'releve') {
						$req_table = mysql_query('SELECT * FROM '.$table) or die(mysql_error());
						$nbr_champs = mysql_num_fields($req_table);
						$nbr_ligne = mysql_num_rows($req_table);
						$j=1;
						while ($ligne = mysql_fetch_array($req_table)) {
							$insertions = '(';
							for ($i=0; $i<$nbr_champs; $i++) {$insertions .= '\'' . mysql_real_escape_string($ligne[$i]) . '\', ';}
							$insertions = substr($insertions, 0, -2);
							if ($j<$nbr_ligne) {$insertions .= "),\n";} else {$insertions .= ");\n";};
							if ($compression=="true") {gzwrite($fp, $insertions);} else {fwrite($fp, $insertions);}
							$j=$j+1;
						}
					}
				}
				mysql_free_result($res);
			}
			if ($compression=="true") {gzclose($fp);} else {fclose($fp);}
			echo "OK";
	}
	break;
case "refreshselectdump" :
	header("Pragma: no-cache");
	header("Expires: 0");
	header("Last-Modified: " . gmdate("D, d M Y H:i:s") . " GMT");
	header("Cache-Control: no-cache, must-revalidate");
	header("Content-type: application/xml"); 
	$xml="<resultats>\n";
	$Dir = opendir("../backup/");
	while ($x=readdir($Dir)) {
		if ($x<>"." && $x<>"..") {
			if (preg_match('/^SQL_domos/',$x)) {
				$xml.= "<resultat fichier=\"$x\" />\n";
			}
		}
	}
	$xml.="</resultats>";
	echo utf8_encode($xml);
	break;
case "restaurerbackup" :
	$fichier=!empty($_REQUEST["fichier"])?$_REQUEST["fichier"]:"";
	//if (preg_match('/.sql.gz/',$fichier)) {$file = readgzfile("../backup/$fichier");} else {$file = file_get_contents("../backup/$fichier");}
// 	if ($file==false) {echo "ERR : fichier non trouvé";}
// 	else {
// 		$requetes = explode(';\n', $file);
// 		foreach($requetes as $requete){
// 			//$result=mysql_query($requete);
// 			echo $requete;
// 			}
// 		echo "Resto OK";
// 	}
	$file = gzfile("../backup/$fichier");
	$commande="";
	foreach ($file as $line) {
		if (preg_match('/;\n/',$line)) {
			$commande.=$line;
			$resultat = mysql_query($commande);
			echo $commande."<br/>";
			$commande="";
		} else {$commande.=$line;}
	}
	echo "Resto OK";
	break;
}
?>