<?php

//******************************************************
//                   CREER UN GRAPHE                   *
//******************************************************

$request=isset($_GET["request"])?$_GET["request"]:(isset($_POST["request"])?$_POST["request"]:"select * from releve where releve_composants='1'order by releve_id");
$gnomimage=isset($_GET["gnomimage"])?$_GET["gnomimage"]:(isset($_POST["gnomimage"])?$_POST["gnomimage"]:"composant_dyn");
$gtitre=isset($_GET["gtitre"])?$_GET["gtitre"]:(isset($_POST["gtitre"])?$_POST["gtitre"]:"Graphe Dynamique");
$gtitrex=isset($_GET["gtitrex"])?$_GET["gtitrex"]:(isset($_POST["gtitrex"])?$_POST["gtitrex"]:"Graphe Dynamique");
$glarg=isset($_GET["glarg"])?$_GET["glarg"]:(isset($_POST["glarg"])?$_POST["glarg"]:"900");
$ghaut=isset($_GET["ghaut"])?$_GET["ghaut"]:(isset($_POST["ghaut"])?$_POST["ghaut"]:"300");
$gymin=isset($_GET["gymin"])?$_GET["gymin"]:(isset($_POST["gymin"])?$_POST["gymin"]:"0");
$gymax=isset($_GET["gymax"])?$_GET["gymax"]:(isset($_POST["gymax"])?$_POST["gymax"]:"1");
$gdateformat=isset($_GET["gdateformat"])?$_GET["gdateformat"]:(isset($_POST["gdateformat"])?$_POST["gdateformat"]:"m/d H:i");
$gmarge=isset($_GET["gmarge"])?$_GET["gmarge"]:(isset($_POST["gmarge"])?$_POST["gmarge"]:"80");
$gmoyenne=isset($_GET["gmoyenne"])?$_GET["gmoyenne"]:(isset($_POST["gmoyenne"])?$_POST["gmoyenne"]:"0");
$gnbvaleurreel=isset($_GET["gnbvaleurreel"])?$_GET["gnbvaleurreel"]:(isset($_POST["gnbvaleurreel"])?$_POST["gnbvaleurreel"]:"1");
$gtypegraphe=isset($_GET["gtypegraphe"])?$_GET["gtypegraphe"]:(isset($_POST["gtypegraphe"])?$_POST["gtypegraphe"]:"0");

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
		
include("../include_php/config.php");
include_once ("../jpgraph/jpgraph.php");
include_once ("../jpgraph/jpgraph_line.php");
include_once ("../jpgraph/jpgraph_date.php");
include_once ("../jpgraph/jpgraph_scatter.php");
include_once ("../jpgraph/jpgraph_regstat.php");

$resultat = mysql_query($request);

$num = mysql_num_rows($resultat);
if ($num>2 && $num>$gnbvaleurreel) {
	$date_debut = mysql_result($resultat,0,"releve_dateheure");
	$date_fin = mysql_result($resultat,$num-1,"releve_dateheure");

	if (file_exists("../images/graphes/".$gnomimage.".png")) {unlink ("../images/graphes/".$gnomimage.".png");}
		
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
		if ($x=="ON") {$x=1;} 
		elseif ($x=="OFF") {$x=0;}
		$x=floatval($x);
		if ($x<=$datay_min) {$datay_min=$x;}
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
	if ($gtitrex <> '') {
		$line->SetLegend($gtitrex);
	}
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
	$graph->Stroke("../images/graphes/".$gnomimage.".png");
	//$graph->Stroke();
}

?>