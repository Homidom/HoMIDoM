<?php
require_once('../include_php/config.php');
include("../include_php/fonctions.php");

function add_row(){
	global $newId;
	$resultat = mysql_query("select menu_ordre from menu order by menu_ordre desc");
	$menu_ordre = mysql_result($resultat,0,"menu_ordre")+1;
	$sql = "INSERT INTO menu(menu_nom,menu_lien,menu_ordre)
			VALUES ('".$_GET["c1"]."','".$_GET["c2"]."','".$menu_ordre."')";
	$res = mysql_query($sql);
	$newId = mysql_insert_id();
	return "insert";
}

function update_row(){
	$sql = "UPDATE menu SET menu_nom='".quote($_GET["c1"])."',menu_lien='".$_GET["c2"]."' WHERE menu_id=".$_GET["gr_id"];
	$res = mysql_query($sql);
	return "update";
}

function delete_row(){
	$d_sql = "DELETE FROM menu WHERE menu_id=".$_GET["gr_id"];
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