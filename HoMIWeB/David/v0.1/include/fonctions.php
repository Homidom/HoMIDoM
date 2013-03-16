<?php


//******************************************************
//            Affiche image + texte WARNING            *
//******************************************************
function warning($msg) {
	echo "<br /><br /><br /><br /><br /><br /><br /><h2><img src=\"./images/warning.gif\" alt=\"\" /><br /><br />$msg</h2><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />";
}
//******************************************************
//            Affiche image + texte INFO               *
//******************************************************
function info($msg) {
	echo "<br /><br /><br /><br /><br /><br /><br /><h2><img src=\"./images/info.gif\" alt=\"\" /><br /><br />$msg</h2><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />";
}
//******************************************************
//             Affiche image + texte OK                *
//******************************************************
function ok($msg) {
	echo "<br /><br /><br /><br /><br /><br /><br /><h2><img src=\"./images/ok.gif\" alt=\"\" /><br /><br />$msg</h2><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />";
}

//******************************************************
//                   CREER UN GRAPHE                   *
//******************************************************
function graphe_numerique($resultat,$gnomimage,$gtitre,$gtitrex,$glarg,$ghaut,$gymin,$gymax,$gdateformat,$gmarge,$gmoyenne,$gnbvaleurreel,$gtypegraphe) {
// Affiche une courbe des valeurs dans le temps
	//$resultat : tableau de valeur+date
	//$gnomimage : nom de l'image
	//$gtitre : titre du graphe
	//$gtitrex : titre de l'axe X
	//$glarg : largeur du graphe
	//$ghaut : hauteur du graphe
	//$gymin : temperature min du graphe (Scale)
	//$gymax : temperature max du graphe (Scale)
	//$gdateformat : Format des dates de l'axe X
	//$gmarge : marge basse
	//$gmoyenne : 1/0 : moyenne par jour ?
	//$gnbvaleurreel : nombre de valeur réelle à prendre en compte : 1 = toutes les valeurs, 3 = 1 sur 3...
	//$gtypegraphe : type de graphe : 0=stepplot, 1=line

	require_once ("./jpgraph/jpgraph.php");
	require_once ("./jpgraph/jpgraph_line.php");
	require_once ("./jpgraph/jpgraph_date.php");
	require_once ("./jpgraph/jpgraph_scatter.php");
	require_once ("./jpgraph/jpgraph_regstat.php");
 
	$num = mysql_num_rows($resultat);
	if ($num>2 && $num>$gnbvaleurreel) {
		$date_debut = mysql_result($resultat,0,"releve_dateheure");
		$date_fin = mysql_result($resultat,$num-1,"releve_dateheure");

		if (file_exists("./images/graphes/".$gnomimage.".png")) {unlink ("./images/graphes/".$gnomimage.".png");}

		$start = strtotime($date_debut);
		$end = strtotime($date_fin);
		$datax=array();
		$datay=array();
		
		$dataxmoy=array(); //tableaux pour les moyennes
		$dataymoy=array();
		$dataxmoy_tmp=substr(mysql_result($resultat,0,"releve_dateheure"),0,10); //date du jour du premier releve
		$dataymoy_tmp=0;
		$nbreleveparmoy_tmp=0;

		$datay_min=$gymin;
		$datay_max=$gymax;
		$nbreleveparmoy_tmp=0;
		for ($i=0;$i<$num;$i++)	{
			$x=mysql_result($resultat,$i,"releve_valeur");
			$x=floatval($x);
			if ($x<$datay_min) {$datay_min=ceil($x-1);}
			if ($x>=$datay_max) {$datay_max=ceil($x+3);}
			$datay[] = $x;
			$datax[] = strtotime(mysql_result($resultat,$i,"releve_dateheure"));
			If ($gmoyenne) { //si on doit faire une courbe de moyenne par jour
				If ($dataxmoy_tmp==substr(mysql_result($resultat,$i,"releve_dateheure"),0,10)) { // si on est le même jour, on ajoute à notre moyenne
					$dataymoy_tmp = $dataymoy_tmp+$x;
					$nbreleveparmoy_tmp++;
				} else { //sinon on calcul la moyenne puis on reprend le compte
					If ($nbreleveparmoy_tmp>0) {
						$dataymoy[] = $dataymoy_tmp/$nbreleveparmoy_tmp;
						$dataxmoy[] = strtotime($dataxmoy_tmp." 12:00");
					} else {
						$dataymoy[] = $x;
						$dataxmoy[] = strtotime($dataxmoy_tmp." 12:00");
					}
					$dataxmoy_tmp = substr(mysql_result($resultat,$i,"releve_dateheure"),0,10);
					$nbreleveparmoy_tmp=0;
					$dataymoy_tmp=0;
				}
			}
			if ($gnbvaleurreel>1) {$i=$i+$gnbvaleurreel-1;} // pour ne prendre un compte que les x elements qu'on veut
		}
		if ($gnbvaleurreel>1) {$end = strtotime(mysql_result($resultat,$i-$gnbvaleurreel,"releve_dateheure"));}
		$graph = new Graph($glarg,$ghaut); // Create the new graph
		$graph->SetMargin(40,40,30,$gmarge);
		$graph->SetScale('datlin',$datay_min,$datay_max,$start,$end);
		$graph->title->Set($gtitre);
		$graph->SetAlphaBlending();
		$graph->xaxis->scale->SetDateFormat($gdateformat);
		$graph->xaxis->SetLabelAngle(90);
		$graph->xaxis->SetPos("min"); //positionne l'axe X au plus bas de la valeur de Y
		$graph->ygrid->Show(true,true);
		$graph->ygrid->SetFill(true,'#EFEFEF@0.5','#BFCFFF@0.8');
		$graph->ygrid->SetLineStyle('dashed'); // Des tirets pour les lignes : dashed / solid
		$graph->xgrid->Show();
		$graph->xgrid->SetLineStyle('dashed');
 		$line = new LinePlot($datay,$datax);
 		$line->SetLegend($gtitrex);
 		If ($gtypegraphe==0) {$line->SetStepStyle();}
 		$graph->legend->SetPos(0.01,0.02,'right','top');
 		$line->SetFillColor('lightblue@0.5');
 		$line->SetColor("blue");
		$graph->Add($line);
		If ($gmoyenne) {
			//$line2 = new LinePlot($dataymoy,$dataxmoy);
			$spline = new Spline($dataxmoy, $dataymoy);
			list($newx,$newy) = $spline->Get($num);
			$line2 = new LinePlot($newy,$newx);
			
			$line2->SetColor("red");
			$graph->Add($line2);
			
			$line3 = new ScatterPlot($dataymoy,$dataxmoy);
			$line3->mark->SetFillColor('red@0.3');
			$line3->mark->SetColor('red@0.5');
			$graph->Add($line3);
		}
		$graph->Stroke("./images/graphes/".$gnomimage.".png");
		//$graph->Stroke();
	}
}

function graphe_cumul($resultat,$gnomimage,$gtitre,$gtitrex,$glarg,$ghaut,$gymin,$gymax,$gdateformat,$gmarge,$gmoyenne,$gnbvaleurreel,$gtypegraphe) {
// Affiche une courbe des valeurs dans le temps
	//$resultat : tableau de valeur+date
	//$gnomimage : nom de l'image
	//$gtitre : titre du graphe
	//$gtitrex : titre de l'axe X
	//$glarg : largeur du graphe
	//$ghaut : hauteur du graphe
	//$gymin : temperature min du graphe (Scale)
	//$gymax : temperature max du graphe (Scale)
	//$gdateformat : Format des dates de l'axe X
	//$gmarge : marge basse
	//$gmoyenne : 1/0 : moyenne par jour ?
	//$gnbvaleurreel : nombre de valeur réelle à prendre en compte : 1 = toutes les valeurs, 3 = 1 sur 3...
	//$gtypegraphe : type de graphe : 0=stepplot, 1=line
		
	require_once ("./jpgraph/jpgraph.php");
	require_once ("./jpgraph/jpgraph_line.php");
	require_once ("./jpgraph/jpgraph_date.php");
	require_once ("./jpgraph/jpgraph_scatter.php");
	require_once ("./jpgraph/jpgraph_regstat.php");
 

	$num = mysql_num_rows($resultat);
	if ($num>2 && $num>$gnbvaleurreel) {
		$date_debut = mysql_result($resultat,0,"releve_dateheure");
		$date_fin = mysql_result($resultat,$num-1,"releve_dateheure");
	
		if (file_exists("./images/graphes/".$gnomimage.".png")) {unlink ("./images/graphes/".$gnomimage.".png");}
			
		$start = strtotime($date_debut);
		$end = strtotime($date_fin);
		$datax=array();
		$datay=array();
		
		$dataxmoy=array(); //tableaux pour les moyennes
		$dataymoy=array();
		$dataxmoy_tmp=substr(mysql_result($resultat,0,"releve_dateheure"),0,10); //date du jour du premier releve
		$dataymoy_tmp=0;
		$nbreleveparmoy_tmp=0;
		
		$datay_min=$gymin;
		$datay_max=$gymax;
		$nbreleveparmoy_tmp=0;
		$x_prev=0;
		for ($i=0;$i<$num;$i++)	{
			$x=mysql_result($resultat,$i,"releve_valeur");
			$x=floatval($x-$x_prev);
			$x_prev=$x;
			if ($x<=$datay_min) {$datay_min=$x-1;}
			if ($x>=$datay_max) {$datay_max=$x+3;}
			$datay[] = $x;
			$datax[] = strtotime(mysql_result($resultat,$i,"releve_dateheure"));
			If ($gmoyenne) { //si on doit faire une courbe de moyenne par jour
				If ($dataxmoy_tmp==substr(mysql_result($resultat,$i,"releve_dateheure"),0,10)) { // si on est le même jour, on ajoute à notre moyenne
					$dataymoy_tmp = $dataymoy_tmp+$x;
					$nbreleveparmoy_tmp++;
				} else { //sinon on calcul la moyenne puis on reprend le compte
					If ($nbreleveparmoy_tmp>0) {
						$dataymoy[] = $dataymoy_tmp/$nbreleveparmoy_tmp;
						$dataxmoy[] = strtotime($dataxmoy_tmp." 12:00");
					} else {
						$dataymoy[] = $x;
						$dataxmoy[] = strtotime($dataxmoy_tmp." 12:00");
					}
					$dataxmoy_tmp = substr(mysql_result($resultat,$i,"releve_dateheure"),0,10);
					$nbreleveparmoy_tmp=0;
					$dataymoy_tmp=0;
				}
			}
			if ($gnbvaleurreel>1) {$i=$i+$gnbvaleurreel-1;} // pour ne prendre un compte que les x elements qu'on veut
		}
		if ($gnbvaleurreel>1) {$end = strtotime(mysql_result($resultat,$i-$gnbvaleurreel,"releve_dateheure"));}
		$graph = new Graph($glarg,$ghaut); // Create the new graph
		$graph->SetMargin(40,40,30,$gmarge);
		$graph->SetScale('datlin',$datay_min,$datay_max,$start,$end);
		$graph->title->Set($gtitre);
		$graph->SetAlphaBlending();
		$graph->xaxis->scale->SetDateFormat($gdateformat);
		$graph->xaxis->SetLabelAngle(90);
		$graph->xaxis->SetPos("min"); //positionne l'axe X au plus bas de la valeur de Y
		$graph->ygrid->Show(true,true);
		$graph->ygrid->SetFill(true,'#EFEFEF@0.5','#BFCFFF@0.8');
		$graph->ygrid->SetLineStyle('dashed'); // Des tirets pour les lignes : dashed / solid
		$graph->xgrid->Show();
		$graph->xgrid->SetLineStyle('dashed');
 		$line = new LinePlot($datay,$datax);
 		$line->SetLegend($gtitrex);
 		If ($gtypegraphe==0) {$line->SetStepStyle();}
 		$graph->legend->SetPos(0.01,0.02,'right','top');
 		$line->SetFillColor('lightblue@0.5');
 		$line->SetColor("blue");
		$graph->Add($line);
		If ($gmoyenne) {
			//$line2 = new LinePlot($dataymoy,$dataxmoy);
			$spline = new Spline($dataxmoy, $dataymoy);
			list($newx,$newy) = $spline->Get($num);
			$line2 = new LinePlot($newy,$newx);
			
			$line2->SetColor("red");
			$graph->Add($line2);
			
			$line3 = new ScatterPlot($dataymoy,$dataxmoy);
			$line3->mark->SetFillColor('red@0.3');
			$line3->mark->SetColor('red@0.5');
			$graph->Add($line3);

			
		}
		$graph->Stroke("./images/graphes/".$gnomimage.".png");
		//$graph->Stroke();
	}
}

function graphe_compteur($resultat,$gnomimage,$gtitre,$gtitrex,$glarg,$ghaut,$gymin,$gymax,$gdateformat,$gmarge) {
//Affiche des barres pour chaque valeur dans le temps
	//$resultat : tableau de valeur+date
	//$gnomimage : nom de l'image
	//$gtitre : titre du graphe
	//$gtitrex : titre de l'axe X
	//$glarg : largeur du graphe
	//$ghaut : hauteur du graphe
	//$gymin : temperature min du graphe (Scale)
	//$gymax : temperature max du graphe (Scale)
	//$gdateformat : Format des dates de l'axe X
	//$gmarge : marge basse
	
	require_once ("./jpgraph/jpgraph.php");
	require_once ("./jpgraph/jpgraph_bar.php");
	require_once ("./jpgraph/jpgraph_date.php");

	$num = mysql_num_rows($resultat);
	if ($num>2) {
		$date_debut = mysql_result($resultat,0,"releve_dateheure");
		$date_fin = mysql_result($resultat,$num-1,"releve_dateheure");
	
		if (file_exists("./images/graphes/".$gnomimage.".png")) {unlink ("./images/graphes/".$gnomimage.".png");}

		$start = strtotime($date_debut);
		$end = strtotime($date_fin);
		$datax=array();
		$datay=array();
		$datay_min=$gymin;
		$datay_max=$gymax;
		$x_first = floatval(mysql_result($resultat,0,"releve_valeur"));
		for ($i=1;$i<$num;$i++)	{
			$x=floatval(mysql_result($resultat,$i,"releve_valeur"));
			if ($x<$datay_min) {$datay_min=$x-2;}
			if ($x>$datay_max) {$datay_max=$x+2;}
			$datay[] = $x-$x_first;
			$datax[] = strtotime(mysql_result($resultat,$i,"releve_dateheure"));
		}

//		$datay_min=-10;
//		$datay_max=50;
//		$start = strtotime($date_debut);
//		$end = $start+400;
//		$datay=array(10,32,23,22,15,15,5,23,25,32);
//		$datax=array($start,$start+110,$start+135,$start+175,$start+225,$start+275,$start+315,$start+350,$start+395,$start+400);

		$graph = new Graph($glarg,$ghaut);
		$graph->SetMargin(40,40,30,$gmarge);
		//$graph->SetScale('datlin',$datay_min,$datay_max,$start,$end);
		$graph->SetScale('datlin',$datay_min,$datay_max);
		$graph->title->Set($gtitre);
		$graph->SetAlphaBlending();

		$graph->xaxis->SetPos("min");
		//$graph->xaxis->scale->SetDateFormat($gdateformat);
		$graph->xaxis->scale->SetDateFormat("d H:i:s");
		//$graph->xaxis->scale->ticks->Set(60,10);
		$graph->xaxis->SetLabelAngle(90);
		$graph->xaxis->SetTickLabels($datax);
		$graph->xaxis->SetTextLabelInterval(1);
		$graph->xaxis->SetTextTickInterval(20);

		$graph->ygrid->Show(true,true);
		$graph->ygrid->SetFill(true,'#EFEFEF@0.5','#BFCFFF@0.8');
		$graph->ygrid->SetLineStyle('dashed'); // Des tirets pour les lignes : dashed / solid

		$bar = new BarPlot($datay);
		//$bar->SetLegend($gtitrex);
		$bar->SetFillColor('#cc1111lightblue@0.1');
		$bar->SetColor("#cc1111lightblue@0.5");
		//$bar->SetWidth(0.4);

		$graph->Add($bar);
		$graph->Stroke("./images/graphes/".$gnomimage.".png");
	}

}


//******************************************************
//                   LANCE TINY MCE                    *
//******************************************************
function tiny_mce() {
	echo "<script type=\"text/javascript\" src=\"./tiny_mce/tiny_mce.js\"></script>
	<script language=\"javascript\" type=\"text/javascript\">
	tinyMCE.init({
		mode : \"textareas\",
		theme : \"advanced\",
		language : \"fr\",
		//plugins : \"table,save,advhr,advlink,emotions,iespell,insertdatetime,preview,zoom,advimage,flash,searchreplace,print,paste,directionality,fullscreen,noneditable,contextmenu\",
		plugins : \"table,save,advhr,advlink,emotions,iespell,insertdatetime,preview,advimage,searchreplace,print,paste,directionality,fullscreen,noneditable,contextmenu\",
		theme_advanced_buttons1 : \"cut,copy,paste,pastetext,pasteword,replace,|,undo,redo,|,print,code,preview,fullscreen,|,link,unlink,image,emotions,|,insertdate,inserttime,hr,charmap,|,visualaid,tablecontrols\",
		theme_advanced_buttons2 : \"bold,italic,underline,strikethrough,sub,sup,|,justifyleft,justifycenter,justifyright,justifyfull,|,outdent,indent,|,bullist,numlist,|,forecolor,backcolor\",
		theme_advanced_buttons3 : \"\",
		theme_advanced_toolbar_location : \"top\",
		theme_advanced_toolbar_align : \"left\",
		theme_advanced_statusbar_location : \"bottom\",
	    plugi2n_insertdate_dateFormat : \"%Y-%m-%d\",
	    plugi2n_insertdate_timeFormat : \"%H:%M:%S\",
		paste_use_dialog : false,
		theme_advanced_resizing : true,
		theme_advanced_resize_horizontal : false,
		paste_auto_cleanup_on_paste : true,
		paste_convert_headers_to_strong : false,
		paste_strip_class_attributes : \"all\",
		paste_remove_spans : false,
		paste_remove_styles : false
	});
	function fileBrowserCallBack(field_name, url, type, win) {
		// This is where you insert your custom filebrowser logic
		alert(\"Filebrowser callback: field_name: \" + field_name + \", url: \" + url + \", type: \" + type);
		// Insert new URL, this would normaly be done in a popup
		win.document.forms[0].elements[field_name].value = \"someurl.htm\";
	}
	</script>";
}


//******************************************************
//     REMPLACE LES ACCENTS ET CARACTERES SPECIAUX     *
//******************************************************
// $chaine : chaine a traiter
function caracteres($chaine){
	if ($chaine<>"") {
		$chaine = str_replace(array(" - ", " -", "- "), "-", $chaine);
		$chaine = str_replace(" ", "-", $chaine);
		$search = array ('@[ÈÉÊËèéêë]@i','@[ÀÁÂÃÄÅàáâãäå]@i','@[ÌÍÎÏìíîï]@i','@[ÙÚÛÜùúûü]@i','@[ÒÓÔÕÖðòóôõö]@i','@[Ýýÿ]@i','@[Çç]@i','@[_]@i','@[ ]+@i','@[^-a-zA-Z0-9_.]@');
		$replace = array ('e','a','i','u','o','y','c','-','-','');
		$chaine = strtolower(preg_replace($search,$replace,html_entity_decode($chaine)));
		return $chaine;
	}
	return "";
}
//******************************************************
//     Vérifie si un gars ne tente pas de schunter une valeur de formulaire contrôlée par des =-* etc     *
//******************************************************
// $chaine : chaine a traiter
function verida($valeur){
	// Vérifie si un champ n'est pas rempli qu'avec des ///   etc
	$caract_verif = array("=", " ", "-", "_", "/", "$", "*", ".", ";",);
	return str_replace($caract_verif, "", $valeur);
}

//******************************************************
//     Récupère comme il faut les données de formulaires
//******************************************************
function quote($champ){
	if(get_magic_quotes_gpc()){return htmlentities($champ);}
	else{return htmlentities(addslashes($champ));}
}
function dequote($champ){
	if(get_magic_quotes_gpc()){return html_entity_decode($champ);}
	else{return html_entity_decode(stripslashes($champ));}
}

//******************************************************
//          FORMATTE UNE DATE EN FRANCAIS              *
//******************************************************
//renvoi mercredi 12 novembre 2006 pour 12/11/2006
function datefr($jj, $mm, $aaaa){
	$userDate = mktime(0,0,0,$mm,$jj,$aaaa);
	$jours = array('dimanche', 'lundi', 'mardi', 'mercredi', 'jeudi', 'vendredi', 'samedi');
	$mois = array('', 'janvier', 'février', 'mars', 'avril', 'mai', 'juin', 'juillet', 'août', 'septembre', 'octobre', 'novembre', 'décembre');
	return $jours[date("w", $userDate)] . " " . $jj . " " .   $mois[date("n", $userDate)] . " " . $aaaa;
}

// Date us to fr
function dateUS2FR($date) { 
$split = split("-",$date); 
$annee = $split[0]; 
$mois = $split[1]; 
$jour = $split[2]; 
return "$jour"."-"."$mois"."-"."$annee"; 
} 


//******************************************************
//                    LISTE OUI/NON                    *
//******************************************************
Function affiche_ouinon($nom,$dom) {
	echo"
	<SELECT length=30 name=$nom>
	<option value='0'";if($dom==0) {echo " selected";} echo ">NON</option>
	<option value='1'";if($dom==1) {echo " selected";} echo ">OUI</option>
	</select>
	";
} 


//******************************************************
//                    SOCKET                   *
//******************************************************
Function socket($message,$adresse,$port) {
	//$adresse = "192.168.0.1";
   	//$port=3852;
	//$message = "([AL#testdesocket][AC#1#ON])";
	$socket = socket_create(AF_INET, SOCK_STREAM, SOL_TCP);
	if ($socket < 0) {
		echo "socket_create() a échoué : raison :  " . socket_strerror ($socket) . "<br />";
	} else {
		$resultat = socket_connect($socket, $adresse, $port);
		if ($resultat < 0) {
			echo "socket_connect() a échoué : raison : ($result) " . socket_strerror($result) . "<br />";
		} else {
			socket_write($socket, $message, strlen($message));
			$reception = '';
			while ($reception = socket_read($socket, 2048)) echo $reception;
			socket_close($socket);
		}
	}
}
Function socket_simple($message,$adresse,$port) {
	//$adresse = "192.168.0.1";
   	//$port=3852;
	//$message = "([AL#testdesocket][AC#1#ON])";
	$socket = socket_create(AF_INET, SOCK_STREAM, SOL_TCP);
	if ($socket < 0) {
		// echo "socket_create() a échoué : raison :  " . socket_strerror ($socket) . "<br />";
	} else {
		$resultat = socket_connect($socket, $adresse, $port);
		if ($resultat < 0) {
			// echo "socket_connect() a échoué : raison : ($result) " . socket_strerror($result) . "<br />";
		} else {
			socket_write($socket, $message, strlen($message));
			$reception = '';
			while ($reception = socket_read($socket, 2048)) {
				//echo $reception;
			}
			socket_close($socket);
		}
	}
}
?>