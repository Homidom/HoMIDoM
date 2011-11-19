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

//Recup de la config meteo
$resultat = mysql_query("select config_valeur from config where config_nom='meteo_codeville'");
$meteo_codeville = mysql_result($resultat,0,"config_valeur");
$resultat = mysql_query("select config_valeur from config where config_nom='meteo_codevillereleve'");
$meteo_codevillereleve = mysql_result($resultat,0,"config_valeur");
$resultat = mysql_query("select config_valeur from config where config_nom='meteo_icone'");
$meteo_icone = mysql_result($resultat,0,"config_valeur");

echo "<tr height=20><td colspan=5 align=center><div class=\"plan\" id=\"plan\">";

//Affichage du cadre prévision meteo à 10 jours
echo "<div class=\"meteo\">";
require_once("./include_php/wdweather.php");
$weather = new WdWeather();
$days = $weather->getWeather($meteo_codeville, 10);
if ($days) {
	foreach ($days as $day) {
	    echo '<div class="cadre">';
	    echo $day['jour'].' '.$day['date'].' <br />';
	    if ($day['day']['icon']<10) {$day['day']['icon']="0".$day['day']['icon'];}
	    if ($day['night']['icon']<10) {$day['night']['icon']="0".$day['night']['icon'];}
	    echo '<img src="./images/wdweather/'.$meteo_icone.'/'  . $day['day']['icon'] . '.png" alt="' . $weather->tempstoFR($day['day']['t']) . '" title="' . $weather->tempstoFR($day['day']['t']) . '" class="icone"/>';
	    echo '<img src="./images/wdweather/'.$meteo_icone.'/' . $day['night']['icon'] . '.png" alt="' . $weather->tempstoFR($day['night']['t']) . '" title="' . $weather->tempstoFR($day['night']['t']) . '" class="icone"/><br />';
	    echo $day['low'].'°C  /  '.$day['hi'].'°C <br />';
	    echo 'Soleil : ' . $day['sunr'] . ' - ' . $day['suns'] . '<br />';
	    echo 'Pluie : ' . $day['day']['ppcp'] . '% - '.$day['night']['ppcp'].'%<br />';
	    echo 'Hum. : ' . $day['day']['hmid'] . '% - '.$day['night']['hmid'].'%<br />';
	    echo 'Vent : ' . $day['day']['wind']['t'] . ' à ' . $day['day']['wind']['s'] . ' km/h <br />';
	    echo 'Vent : ' . $day['night']['wind']['t'] . ' à ' . $day['night']['wind']['s'] . ' km/h <br /><br />';
	    echo '</div>';
	}
} else {echo "Erreur Récupération information météo";}
echo "</div><div class=\"meteo\">";
echo "Matin &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Aprés-midi <br />";
echo "<img src='http://www.pleinchamp.com/images/meteo/national/frsymb1.png' class='imagesimple'><img src='http://www.pleinchamp.com/images/meteo/national/frtemp1.png' class='imagesimple'>";
echo "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
echo "<img src='http://www.pleinchamp.com/images/meteo/national/frsymb2.png' class='imagesimple'><img src='http://www.pleinchamp.com/images/meteo/national/frtemp2.png' class='imagesimple'>";
echo "<br /></div><div class=\"meteo\"><br />";
echo "Algrange <a href='http://fr.weather.com/weather/today-Algrange-57440'><img src='./images/weathercom.gif' title='Weather.com' border='0' style='vertical-align:middle;' /></a> <a href='http://www.pleinchamp.com/meteo/meteoDept.aspx?dpt_id=57&menu_id=35'><img src='./images/pleinchamp.gif' title='pleinchamp.com' border='0' style='vertical-align:middle;' /></a> | ";
echo "Gap <a href='http://fr.weather.com/weather/today-Gap-05000'><img src='./images/weathercom.gif' title='Weather.com' border='0' style='vertical-align:middle;' /></a> <a href='http://www.pleinchamp.com/meteo/meteoDept.aspx?dpt_id=05&menu_id=35'><img src='./images/pleinchamp.gif' title='pleinchamp.com' border='0' style='vertical-align:middle;' /></a> | ";
echo "Paris <a href='http://fr.weather.com/weather/today-Paris-75000'><img src='./images/weathercom.gif' title='Weather.com' border='0' style='vertical-align:middle;' /></a> <a href='http://www.pleinchamp.com/meteo/meteoDept.aspx?dpt_id=75&menu_id=35'><img src='./images/pleinchamp.gif' title='pleinchamp.com' border='0' style='vertical-align:middle;' /></a> | ";
echo "Guadeloupe <a href='http://fr.weather.com/weather/today-Pointe-%E0-Pitre-GPXX0003'><img src='./images/weathercom.gif' title='Weather.com' border='0' style='vertical-align:middle;' /></a> <a href='http://www.pleinchamp.com/meteo/meteoDept.aspx?dpt_id=971&menu_id=35'><img src='./images/pleinchamp.gif' title='pleinchamp.com' border='0' style='vertical-align:middle;' /></a><br />";
echo "<br /></div></div></td></tr>";
echo "<script type=\"text/javascript\">loadData();</script>";

?>

        </table>
      </td>
    </tr>
  </tbody>
 </table>
</div>
<?php
	}else{
		header("location:../index.php");
	}
?>