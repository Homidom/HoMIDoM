<?php
	if(isset($_SESSION['user_id'])){

		include("./include_php/config.php");
		$action=isset($_GET["action"])?$_GET["action"]:(isset($_POST["action"])?$_POST["action"]:"gerer");
		$resultat = mysql_query("select config_valeur from config where config_nom='menu_seticone'");
		$menuset = mysql_result($resultat,0,"config_valeur");

?>
<div class="main">
 <table border="0" width="100%">
  <tbody>
    <tr>
      <td align="center" valign="middle">

	<!-- TITRE -->
        <table border="0" width="100%">
            <tr>
              <td class="titre" align="left"><img src="images/menu/<?php echo "$menuset"; ?>/menu.png" width="32" height="32" border="0"> Gestion du menu </td>
              <td align="right"> </td>
            </tr>
        </table>

	<!-- CONTENU -->
        <table width="92%" class="adminform">
          <tbody>
            <tr>
              <td width="100%" align="left" valign="top">
				<table border="0" cellspacing="0" cellpadding="0" width="100%">
				
<?php

switch ($action) {
case "gerer" :
	echo "<tr height=\"23\" bgcolor=\"#5680CB\">
		<td align=right class=\"titrecolonne\"><a href=\"menu-ordre.html\"><img src=\"./images/plus.gif\" border=\"0\"> Modifier l'ordre</a>&nbsp;&nbsp;&nbsp;</td>
	     </tr>\n";
	echo "
		<tr><td align=center><div class=\"plan\" id=\"plan\" style=\"padding:5px;\">
			<link rel=\"stylesheet\" type=\"text/css\" href=\"dhtmlx/dhtmlx.css\">
			<script  src=\"dhtmlx/dhtmlx.js\"></script>
			<div id=\"gridbox\" style=\"width:915px;height:515px;overflow:hidden\"></div> 
			<script>
				mygrid = new dhtmlXGridObject('gridbox');
				mygrid.setImagePath(\"./dhtmlx/imgs/\");
				mygrid.setSkin(\"dhx_skyblue\");
				mygrid.setHeader(\"ID,Nom,Lien,Ordre\");
				mygrid.setInitWidths(\"45,150,150,*\");
				mygrid.setColAlign(\"center,center,center,left\");
				mygrid.setColTypes(\"ro,ed,ed,ro\"); 
				mygrid.setColSorting(\"int,str,str,int\");
				mygrid.init();
				mygrid.loadXML(\"pages/dhtmlx_get.php?action=menu\");
				myDataProcessor = new dataProcessor(\"pages/dhtmlx_update_menu.php\");
				myDataProcessor.init(mygrid);
				myDataProcessor.enableUTFencoding(false);
				function deletee() {
					if (confirm('Supprimer ?')) {mygrid.deleteRow(mygrid.getSelectedId());}
				}
				function modif_ordre() {
					window.location.href='menu-ordre.html'
				}
			</script>
			<input type=\"button\" name=\"a1\" value=\"Ajouter\" onClick=\"mygrid.addRow((new Date()).valueOf(),['','nom','lien','999'],0)\" class=\"formsubmit\">
			<input type=\"button\" name=\"a1\" value=\"Supprimer\" onClick=\"deletee()\" class=\"formsubmit\">
			<input type=\"button\" name=\"a1\" value=\"Modif ORDRE\" onClick=\"modif_ordre()\" class=\"formsubmit\">
		</div></td></tr>
	";
	break;

case "ordre" :
	echo "<tr height=\"23\" bgcolor=\"#5680CB\">
		<td align=right class=\"titrecolonne\"><a href=\"javascript:history.go(-1);\"><img src=\"./images/plus.gif\" border=\"0\"> Retour</a>&nbsp;&nbsp;&nbsp;</td>
	     </tr>\n";
	echo "<tr><td>Modifier l'ordre du menu : <br />";
	echo "<div id=\"contentLeft\"><ul>";
	$resultat = mysql_query("select * from menu order by menu_ordre");
	if($resultat){
		while($row=mysql_fetch_array($resultat)){
			echo "<li id=\"recordsArray_".$row['menu_id']."\">".$row['menu_nom']." -> ".$row['menu_lien']."</li>";
		}
	}   
	echo "</ul></div>";
	echo "
		<script language=\"javascript\" type=\"text/javascript\" src=\"./include_cssjs/jquery-ui-1.8.4.custom.min.js\"></script>
		<script type=\"text/javascript\">
		$(document).ready(function(){ 
			$(function() {
				$(\"#contentLeft ul\").sortable({ opacity: 0.6, cursor: 'move', update: function() {
					var order = $(this).sortable(\"serialize\") + '&action=updateRecordsListings';
					$.post(\"pages/menu_update.php\", order, function(theResponse){
					});
				}
				});
			});
		
		});
		</script>
	</td></tr>";
	break;

default : echo "<tr><td>choisis une action ci dessus</td></tr>";
	break;
}

?>
		</table>
 	      </td>
            </tr>
          </tbody>
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
