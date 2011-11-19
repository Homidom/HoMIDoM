<?php
	if(isset($_SESSION['user_id'])){
		include("./include_php/config.php");
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
              <td class="titre" align="left"><img src="images/menu/<?php echo "$menuset"; ?>/webcam.png" width="32" height="32" border="0"> Gestion des Webcams </td>
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
	echo "<tr height=\"23\" bgcolor=\"#5680CB\">
		<td align=right class=\"titrecolonne\"><a href=\"javascript:history.go(-1);\"><img src=\"./images/plus.gif\" border=\"0\"> Retour</a>&nbsp;&nbsp;&nbsp;</td>
	     </tr>\n";

	echo "
		<tr><td align=center><div class=\"plan\" id=\"plan\" style=\"padding:5px;\">
			<link rel=\"stylesheet\" type=\"text/css\" href=\"dhtmlx/dhtmlx.css\">
			<script  src=\"dhtmlx/dhtmlx.js\"></script>
			<div id=\"gridbox\" style=\"width:915px;height:515px;overflow:hidden\"></div> 
			<script>
				mygrid = new dhtmlXGridObject('gridbox');
				mygrid.setImagePath('./dhtmlx/imgs/');
				mygrid.setSkin('dhx_skyblue');
				mygrid.setHeader('Id,Nom,Description,Lien,Accueil');
				mygrid.setInitWidths('50,150,200,*,50');
				mygrid.setColAlign('center,left,left,left,center');
				mygrid.setColTypes('ro,ed,ed,ed,ed'); 
				mygrid.setColSorting('str,str,str,str,str');
				mygrid.init();
				mygrid.loadXML('pages/dhtmlx_get.php?action=webcams');
				myDataProcessor = new dataProcessor('pages/dhtmlx_update_webcams.php');
				myDataProcessor.init(mygrid);
				myDataProcessor.enableUTFencoding(false);
				function deletee() {
					if (confirm('Supprimer ?')) {mygrid.deleteRow(mygrid.getSelectedId());}
				}
			</script>
			<input type=\"button\" name=\"a1\" value=\"Ajouter\" onClick=\"mygrid.addRow((new Date()).valueOf(),['','nom','description','http://lien','0'],0)\" class=\"formsubmit\">
			<input type=\"button\" name=\"a1\" value=\"Supprimer\" onClick=\"deletee()\" class=\"formsubmit\">
		</div></td></tr>
	";
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
