<?php
require_once('../include_php/config.php');
include("../include_php/fonctions.php");

function add_row(){
	global $newId;
	$sql = 	"INSERT INTO composants(composants_nom,composants_modele,composants_adresse,composants_description,composants_polling,composants_actif,composants_etat,composants_correction,composants_precision,composants_divers,composants_maj)
					VALUES ('".$_GET["c2"]."','".$_GET["c3"]."','".quote($_GET["c4"])."','".$_GET["c5"]."','".$_GET["c6"]."','".$_GET["c7"]."','".$_GET["c8"]."','".$_GET["c10"]."','".$_GET["c11"]."','".$_GET["c12"]."','".quote($_GET["c13"])."')";
	$res = mysql_query($sql);
	$newId = mysql_insert_id();
	return "insert";
}

function update_row(){
	//recuperation de l'ancienne adresse et etat
	$resultat = mysql_query("select composants_adresse, composants_etat from composants where composants_id='".$_GET["gr_id"]."'");
	$num = mysql_num_rows($resultat);
	if ($num<>0) {
		$composants_adresse = mysql_result($resultat,0,"composants_adresse");
		$composants_etat = mysql_result($resultat,0,"composants_etat");
	}
	//update du composant
	$sql = 	"UPDATE composants SET 
		composants_nom='".quote($_GET["c2"])."',composants_modele='".$_GET["c3"]."',composants_adresse='".$_GET["c4"]."',
		composants_description='".quote($_GET["c5"])."',composants_polling='".$_GET["c6"]."',composants_actif=	'".$_GET["c7"]."',composants_etat=	'".$_GET["c8"]."',
		composants_correction='".$_GET["c10"]."',composants_precision='".$_GET["c11"]."',composants_divers='".$_GET["c12"]."' ,composants_maj='".$_GET["c13"]."' 
		WHERE composants_id=".$_GET["gr_id"];
	$res = mysql_query($sql);
	//envoi par socket de la nouvelle adresse ou etat
	$resultat = mysql_query("select config_valeur from config where config_nom='socket_ip'");
	$adresse = mysql_result($resultat,0,"config_valeur");
	$resultat = mysql_query("select config_valeur from config where config_nom='socket_port'");
	$port = mysql_result($resultat,0,"config_valeur");
	if ($composants_adresse<>$_GET["c4"]) { // adresse a été modifié on update
		socket_simple("([MA#".$_GET["gr_id"]."#".$_GET["c4"]."])",$adresse,$port);
	}
	if ($composants_etat<>$_GET["c8"]) { // etat a été modifié on update
		socket_simple("([ME#".$_GET["gr_id"]."#".$_GET["c8"]."])",$adresse,$port);
	}
	return "update";
}

function delete_row(){
	$d_sql = "DELETE FROM composants WHERE composants_id=".$_GET["gr_id"];
	$resDel = mysql_query($d_sql);
	return "delete";	
}

header("Content-type: text/xml");
echo('<?xml version="1.0" encoding="iso-8859-1"?>'); 

$mode = @$_GET["!nativeeditor_status"]; //get request mode
$rowId = @$_GET["gr_id"]; //id or row which was updated 
$newId = @$_GET["gr_id"]; //will be used for insert operation

switch($mode){
	case "inserted":
		$action = add_row();
	break;
	case "deleted":
		$action = delete_row();
	break;
	default:
		$action = update_row();
	break;
}

echo "<data>";
echo "<action type='".$action."' sid='".$rowId."' tid='".$newId."'/>";
echo "</data>";

?>