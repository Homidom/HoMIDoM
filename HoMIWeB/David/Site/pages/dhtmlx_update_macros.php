<?php
require_once('../include_php/config.php');
include("../include_php/fonctions.php");

function add_row(){
	global $newId;
	$sql = 	"INSERT INTO macro(macro_nom,macro_description,macro_actif)
					VALUES ('".quote($_GET["c2"])."','".$_GET["c3"]."','".$_GET["c1"]."')";
	$res = mysql_query($sql);
	$newId = mysql_insert_id();
	return "insert";			
}

function update_row(){
	$sql = 	"UPDATE macro SET 
		macro_nom='".quote($_GET["c2"])."',macro_description='".$_GET["c3"]."',macro_actif='".$_GET["c1"]."'
		WHERE macro_id=".$_GET["gr_id"];
	$res = mysql_query($sql);
	return "update";
}

function delete_row(){
	$d_sql = "DELETE FROM macro WHERE macro_id=".$_GET["gr_id"];
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