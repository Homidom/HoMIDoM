<?php
require_once('../include_php/config.php');
include("../include_php/fonctions.php");

header("Content-type: text/xml");
echo('<?xml version="1.0" encoding="iso-8859-1"?>'); 
echo '<rows id="0">';

$action=isset($_GET["action"])?$_GET["action"]:(isset($_POST["action"])?$_POST["action"]:"composants");

switch ($action) {
case "composants" :
	$sql = "SELECT composants.*,composants_modele.* from composants,composants_modele where composants.composants_modele=composants_modele.composants_modele_id order by composants_nom";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			$resultat_tmp = mysql_query("select releve_id from releve where releve_composants='".$row['composants_id']."'");
	    	$num_releve = mysql_num_rows($resultat_tmp);
	    	//create xml tag for grid's row
			echo ("<row id='".$row['composants_id']."'>");
			print ("<cell><![CDATA[");
			//if ($num_releve>0) {print("<a href='composants-relever-".$row['composants_id'].".html' title='$num_releve Relevés'><img src='images/_releve.png' width=16 style='border:0;'></a> ");}
			print("<a href='composants-relever-".$row['composants_id'].".html' title='$num_releve Relevés'><img src='images/releve.png' width=16 style='border:0;'></a> ");
			if ($num_releve>0 && $row['composants_modele_graphe']>0) {print("<a href='composants-grapher-".$row['composants_id'].".html' title='Graphe'><img src='images/graphe.gif' width=16 style='border:0;'></a>");}
			print ("]]></cell>");
			print("<cell><![CDATA[".$row['composants_id']."]]></cell>");
			print("<cell><![CDATA[".dequote($row['composants_nom'])."]]></cell>");
			print("<cell><![CDATA[".$row['composants_modele']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_adresse']."]]></cell>");
			print("<cell><![CDATA[".dequote($row['composants_description'])."]]></cell>");
			print("<cell><![CDATA[".$row['composants_polling']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_actif']."]]></cell>");
			//if ($num_releve>0) {print("<cell>".$row['composants_etat']."^test-relever-".$row['composants_id'].".html^_self</cell>");} else {print("<cell>".$row['composants_etat']."</cell>");}
			print("<cell>".$row['composants_etat']."</cell>");
			print("<cell><![CDATA[".$row['composants_etatdate']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_correction']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_precision']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_divers']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_maj']."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "composants_bannis" :
	$sql = "SELECT composants_bannis.* from composants_bannis";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			//create xml tag for grid's row
			echo ("<row id='".$row['composants_bannis_id']."'>");
			print("<cell><![CDATA[".$row['composants_bannis_id']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_bannis_norme']."]]></cell>");
			print("<cell><![CDATA[".dequote($row['composants_bannis_adresse'])."]]></cell>");
			print("<cell><![CDATA[".dequote($row['composants_bannis_description'])."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "webcams" :
	$sql = "SELECT * from webcams";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			//create xml tag for grid's row
			echo ("<row id='".$row['webcams_id']."'>");
			print("<cell><![CDATA[".$row['webcams_id']."]]></cell>");
			print("<cell><![CDATA[".dequote($row['webcams_nom'])."]]></cell>");
			print("<cell><![CDATA[".dequote($row['webcams_description'])."]]></cell>");
			print("<cell><![CDATA[".dequote($row['webcams_lien'])."]]></cell>");
			print("<cell><![CDATA[".dequote($row['webcams_accueil'])."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "menu" :
	$sql = "SELECT * from menu";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			//create xml tag for grid's row
			echo ("<row id='".$row['menu_id']."'>");
			print("<cell><![CDATA[".$row['menu_id']."]]></cell>");
			print("<cell><![CDATA[".dequote($row['menu_nom'])."]]></cell>");
			print("<cell><![CDATA[".$row['menu_lien']."]]></cell>");
			print("<cell><![CDATA[".dequote($row['menu_ordre'])."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "macros" :
	$sql = "SELECT * from macro order by macro_nom";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			echo ("<row id='".$row['macro_id']."'>");
			print ("<cell><![CDATA[");
			print("<a href='macros-modifier-".$row['macro_id'].".html' title='Conditions/Actions'><img src='images/modif.gif' width=16 style='border:0;'></a> ");
			print ("]]></cell>");
			print("<cell><![CDATA[".$row['macro_actif']."]]></cell>");
			print("<cell><![CDATA[".dequote($row['macro_nom'])."]]></cell>");
			print("<cell><![CDATA[".dequote($row['macro_description'])."]]></cell>");
			print("<cell><![CDATA[".$row['macro_conditions']."]]></cell>");
			print("<cell><![CDATA[".$row['macro_actions']."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "relever" :
	$compid=isset($_GET["compid"])?$_GET["compid"]:"";
	$limitmin=isset($_GET["limitmin"])?$_GET["limitmin"]:0;
	$limitmax=isset($_GET["limitmax"])?$_GET["limitmax"]:10000;
	$sql = "SELECT * from releve where releve_composants='$compid' order by releve_dateheure desc limit ".$limitmin.",".$limitmax." ";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			echo ("<row id='".$row['releve_id']."'>");
			print("<cell><![CDATA[".$row['releve_composants']."]]></cell>");
			print("<cell><![CDATA[".$row['releve_dateheure']."]]></cell>");
			print("<cell><![CDATA[".$row['releve_valeur']."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "modeles" :
	$sql = "SELECT * from composants_modele order by composants_modele_norme, composants_modele_nom";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			echo ("<row id='".$row['composants_modele_id']."'>");
			print("<cell><![CDATA[".$row['composants_modele_norme']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_modele_nom']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_modele_graphe']."]]></cell>");
			print("<cell><![CDATA[".$row['composants_modele_description']."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "plans" :
	$sql = "SELECT plan.* from plan,composants where plan_composant=composants_id order by composants_nom";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			echo ("<row id='".$row['plan_id']."'>");
			print("<cell><![CDATA[".$row['plan_composant']."]]></cell>");
			print("<cell><![CDATA[".$row['plan_left']."]]></cell>");
			print("<cell><![CDATA[".$row['plan_top']."]]></cell>");
			print("<cell><![CDATA[".$row['plan_visible']."]]></cell>");
			print("<cell><![CDATA[".$row['plan_nomplan']."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "config" :
	$sql = "SELECT * from config order by config_nom";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			echo ("<row id='".$row['config_id']."'>");
			print("<cell><![CDATA[".$row['config_nom']."]]></cell>");
			print("<cell><![CDATA[".$row['config_valeur']."]]></cell>");
			print("<cell><![CDATA[".$row['config_description']."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "users" :
	$sql = "SELECT * from users order by login";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			echo ("<row id='".$row['id']."'>");
			print("<cell><![CDATA[".$row['login']."]]></cell>");
			print("<cell><![CDATA[".$row['droits']."]]></cell>");
			print("<cell><![CDATA[".$row['pwd']."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "logs" :
	$limitmin=isset($_GET["limitmin"])?$_GET["limitmin"]:0;
	$limitmax=isset($_GET["limitmax"])?$_GET["limitmax"]:10000;
	$nbjour=isset($_GET["nbjour"])?$_GET["nbjour"]:1;
	$Timestamp = strtotime (date("Y/m/d"))-(($nbjour-1)*24*60*60); // on enleve logs_jours jours a la date actuelle
	$datejours = date ('Y-m-d', $Timestamp); // on reconverti en date = date actuelle - 7 jours

	if ($nbjour==0) {$sql = "SELECT * from logs order by logs_date desc, logs_id desc limit ".$limitmin.",".$limitmax." ";}
	else {$sql = "SELECT * from logs where logs_date>'".$datejours."' order by logs_date desc, logs_id desc  limit ".$limitmin.",".$limitmax." ";}
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			echo ("<row id='".$row['logs_id']."'>");
			print("<cell><![CDATA[".$row['logs_source']."]]></cell>");
			print("<cell><![CDATA[".$row['logs_date']."]]></cell>");
			print("<cell><![CDATA[");
			$description=quote(dequote($row['logs_description']));
			If (strpos(strtoupper($row['logs_description']),"ALERT")>0) {print("<b style='color:red;'>".$description."</b>");}
			ElseIf ($row['logs_source']=="2" || $row['logs_source']=="1") {print("<b>".$description."</b>");}
			else {print($description);}
			print("]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

case "logs2" :
	$champ=isset($_GET["champ"])?$_GET["champ"]:"";
	$texte=isset($_GET["texte"])?$_GET["texte"]:"";
	$limite=isset($_GET["limite"])?$_GET["limite"]:"";
	$sql = "SELECT * from logs where $champ like '%$texte%' order by logs_date desc limit 0,$limite ";
	$res = mysql_query ($sql);
	if($res){
		while($row=mysql_fetch_array($res)){
			echo ("<row id='".$row['logs_id']."'>");
			print("<cell><![CDATA[".$row['logs_source']."]]></cell>");
			print("<cell><![CDATA[".$row['logs_date']."]]></cell>");
			print("<cell><![CDATA[".$row['logs_description']."]]></cell>");
			print("</row>");
		}
	}else{echo mysql_errno().": ".mysql_error()." at ".__LINE__." line in ".__FILE__." file<br>";}
	break;

}
echo '</rows>';

?>