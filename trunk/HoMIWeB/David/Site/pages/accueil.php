<?php
	if(isset($_SESSION['user_id'])){ 
?>
<div class="main">
 <table border="0" width="100%">
  <tbody>
    <tr>
      <td align="center" valign="middle">
        <table border="0" width="100%">
    
<?php
include("./include_php/config.php");
$action=isset($_GET["action"])?$_GET["action"]:(isset($_POST["action"])?$_POST["action"]:"rdc");

//Recup de la config meteo
$resultat = mysql_query("select config_valeur from config where config_nom='meteo_codeville'");
$meteo_codeville = mysql_result($resultat,0,"config_valeur");
$resultat = mysql_query("select config_valeur from config where config_nom='meteo_codevillereleve'");
$meteo_codevillereleve = mysql_result($resultat,0,"config_valeur");
$resultat = mysql_query("select config_valeur from config where config_nom='meteo_icone'");
$meteo_icone = mysql_result($resultat,0,"config_valeur");

echo "<tr height='20'><td colspan='5' align='center'><div class='plan' id='plan'>";

echo "<div class=cadregauche>";




echo "<div class='cadresl'>";


$homidom = new HomidomClass("localhost", "7999", "123456789",true);
echo "Update de la BDD locale : " . $homidom->RefreshBddDevice();
echo "Test execute commande : " . $homidom->ExecuteDeviceCommandSimple("e8e8a771-e20a-4379-963c-f8b380348d91","ON","","");
echo "Test execute commande : " . $homidom->ExecuteDeviceCommandSimple("e8e8a771-e20a-4379-963c-f8b380348d91","DIM","80","");
echo "</div>";



	//Affichage des dernieres Erreurs
	echo "<div class='cadresl'><table id='weather' cellpadding='4' cellspacing='0' width='100%'>";
	echo "<tr><td colspan='2' align='left'><b> Les derniéres erreurs :</b></td></tr>";
	$sql = "SELECT distinct logs_description, logs_date  from logs where logs_source = '2' order by logs_date desc limit 0,9 ";
	$res = mysql_query ($sql);
	if(mysql_num_rows($res)>0){
		while($row=mysql_fetch_array($res)){
			if(strlen($row['logs_description'])>88) {echo ("<tr><td align='center' width='140'>".$row['logs_date']."</td><td align=left>".substr($row['logs_description'],0,88)."...</td></tr>");}
			else {echo ("<tr><td align='center' width='140'>".$row['logs_date']."</td><td align=left>".$row['logs_description']."</td></tr>");}
		}
	} else {echo "<tr><td colspan='2'><br />Aucune erreur pour l'instant. <br /><br /></td></tr>";}
	echo "</table>";
	echo "<div class='lien'><a href=logs.html> ... </a></div>";
	echo "</div>";
	
	//Affichage des dernieres Alertes
	echo "<div class='cadresl'><table id='weather' cellpadding='4' cellspacing='0' width='100%'>";
	echo "<tr><td colspan='2' align='left'><b> Les derniéres alertes :</b></td></tr>";
	$sql = "SELECT DISTINCT logs_date, logs_description from logs where logs_description like '%Alert%' order by logs_date desc limit 0,9 ";
	$sql = "SELECT logs_date, logs_description from logs where logs_description like '%Alert%' and logs_description not like '%->%' order by logs_date desc limit 0,9 ";
	$res = mysql_query ($sql);
	if(mysql_num_rows($res)>0){
		while($row=mysql_fetch_array($res)){
			echo ("<tr><td align='center' width='140'>".$row['logs_date']."</td><td align=left>".$row['logs_description']."</td></tr>");
		}
	
	} else {echo "<tr><td colspan='2'><br />Aucune alerte pour l'instant. <br /><br /></td></tr>";}
	echo "</table>";
	echo "<div class='lien'><a href='logs.html'> ... </a></div>";
	echo "</div>";
	
	//Affichage des composants non à jour
	echo "<div class='cadresl'><table id='weather' cellpadding='4' cellspacing='0' width='100%'>";
	echo "<tr><td colspan='2' align='left'><b> Les composants non à jour :</b></td></tr>";
	echo "<tr><td colspan='2'><br />Aucun composant non à jour. <br /><br /></td></tr>";
	echo "</table>";
	echo "<div class='lien'><a href='devices.html'> ... </a></div>";
	echo "</div>";

echo "</div><div class=cadredroit>";

	//affichage de la météo
	echo "<div class='cadresr'><table id='weather' cellpadding='4' cellspacing='0' width='200'><tr>";
	require_once("./include_php/wdweather.php");
	$weather = new WdWeather();
	$todays = $weather->getWeather2($meteo_codevillereleve, 1);
	if ($todays) {
		foreach ($todays as $today) {
		    echo "<td>";
		    echo "<img src=\"./images/wdweather/$meteo_icone/".$today['icon'].".png\" alt=\"".$weather->tempstoFR($today['t'])."\" title=\"".$weather->tempstoFR($today['t'])."\" /><br />";
		    echo $today['tmp'] == "N/A" ? "" : ($today['tmp'])."°C - ".$today['hmid']."%<br />";
		    echo "Vent du ".$today['wind']['t']." à ".$today['wind']['s']." km/h <br />";
		    echo $today['bar']['r']."mb -> ".$today['bar']['d']."<br />";
		    echo "Visibilité à ".$today['vis']." km<br />";
		    echo "UVs ".$today['uv']['i']."/10 (".$today['uv']['t'].")";
		    echo "</td>";
		}
	} else {echo "<td align=center><br /><br /><br />Erreur<br />météo<br /><br /><br /></td>";}
	echo "</tr></table>";
	echo "<div class='lien'><a href='meteo.html'> ... </a></div>";
	echo "</div>";

	//affichage des webcams
	$sql = "SELECT * from webcams where webcams_accueil>'0' order by webcams_accueil";
	$res = mysql_query($sql);
	if(mysql_num_rows($res)>0){
		while($row=mysql_fetch_array($res)){
			echo "<div class='cadresr'><table id='weather' cellpadding='4' cellspacing='0' width='205'>";
			echo "<tr><td colspan='2' align='left'><b> ".$row['webcams_nom']." :</b></td></tr><tr>";
			echo "<td align=center><a href=\"".$row['webcams_lien']."\" rel=\"prettyPhoto[gallery1]\" title=\"".$row['webcams_nom']."\"><img src=\"".$row['webcams_lien']."\" id=\"webcam\" /></a></td>";
			echo "</tr></table>";
			echo "</div>";
		}
	}

echo "</div>";

echo "</div></td></tr>";
?>
        </table>
      </td>
    </tr>
  </tbody>
 </table>
</div>
<?php
	}else{
		header("location:../index.html");
	}
?>