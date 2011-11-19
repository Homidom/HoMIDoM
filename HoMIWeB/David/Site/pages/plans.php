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
              <td class="titre" align="left"><img src="images/menu/<?php echo "$menuset"; ?>/plans.png" width="32" height="32" border="0"> Gestion des Plans </td>
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
				mygrid.setHeader('Composant,X,Y,Visible,Plan');
				mygrid.setInitWidths('250,50,50,50,*');
				mygrid.setColAlign('left,left,center,center,left');
				mygrid.setColTypes('co,ed,ed,ch,co'); 
				mygrid.setColSorting('str,str,int,int,str');
				";
				$resultat_tmp = mysql_query("SELECT composants_id,composants_nom from composants order by composants_nom");
				while($row=mysql_fetch_array($resultat_tmp)){echo "mygrid.getCombo(0).put(".$row['composants_id'].",\"".dequote($row['composants_nom'])."\");";}
				$resultat_tmp = mysql_query("SELECT menu_lien from menu order by menu_nom");
				while($row=mysql_fetch_array($resultat_tmp)){
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_div\",\"".$row['menu_lien']."_div\");";
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_hyg\",\"".$row['menu_lien']."_hyg\");";
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_jour\",\"".$row['menu_lien']."_jour\");";
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_lampe\",\"".$row['menu_lien']."_lampe\");";
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_lampeaction\",\"".$row['menu_lien']."_lampeaction\");";
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_mvt\",\"".$row['menu_lien']."_mvt\");";
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_temp\",\"".$row['menu_lien']."_temp\");";
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_voletaction\",\"".$row['menu_lien']."_voletaction\");";
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_voletactionetat\",\"".$row['menu_lien']."_voletactionetat\");";
					echo "mygrid.getCombo(4).put(\"".$row['menu_lien']."_voletetat\",\"".$row['menu_lien']."_voletetat\");";
				}
				echo "
				mygrid.init();
				mygrid.loadXML('pages/dhtmlx_get.php?action=plans');
				myDataProcessor = new dataProcessor('pages/dhtmlx_update_plans.php');
				myDataProcessor.init(mygrid);
				myDataProcessor.enableUTFencoding(false);
				function deletee() {
					if (confirm('Supprimer ?')) {mygrid.deleteRow(mygrid.getSelectedId());}
				}
			</script>
			<input type=\"button\" name=\"a1\" value=\"Ajouter\" onClick=\"mygrid.addRow((new Date()).valueOf(),['1','0','0','0',''],0)\" class=\"formsubmit\">
			<input type=\"button\" name=\"a1\" value=\"Supprimer\" onClick=\"deletee()\" class=\"formsubmit\"><br /><br />
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
