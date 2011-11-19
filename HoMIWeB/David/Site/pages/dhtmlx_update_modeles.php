<?php
require_once('../include_php/config.php');
include("../include_php/fonctions.php");

function add_row(){
	global $newId;
	$sql = "INSERT INTO composants_modele(composants_modele_norme,composants_modele_nom,composants_modele_graphe,composants_modele_description)
			VALUES ('".$_GET["c0"]."','".$_GET["c1"]."','".$_GET["c2"]."','".$_GET["c3"]."')";	
	$res = mysql_query($sql);
	$newId = mysql_insert_id();
	return "insert";
}

function update_row(){
	$sql = "UPDATE composants_modele SET composants_modele_norme='".$_GET["c0"]."',composants_modele_nom='".$_GET["c1"]."',composants_modele_graphe='".$_GET["c2"]."',composants_modele_description='".$_GET["c3"]."' 
					WHERE composants_modele_id=".$_GET["gr_id"];
	$res = mysql_query($sql);
	return "update";
}

function delete_row(){
	$d_sql = "DELETE FROM composants_modele WHERE composants_modele_id=".$_GET["gr_id"];
	$resDel = mysql_query($d_sql);
	return "delete";
}

header("Content-type: text/xml");
echo('<?xml version="1.0" encoding="iso-8859-1"?>'); 

$mode = $_GET["!nativeeditor_status"]; //get request mode
$rowId = $_GET["gr_id"]; //id or row which was updated 
$newId = $_GET["gr_id"]; //will be used for insert operation

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