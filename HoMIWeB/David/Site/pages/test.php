<?php
	if(isset($_SESSION['user_id'])){ 
		$action=isset($_GET["action"])?$_GET["action"]:(isset($_POST["action"])?$_POST["action"]:"rdc");
		$compid=isset($_GET["compid"])?$_GET["compid"]:(isset($_POST["compid"])?$_POST["compid"]:"");
?>
<div class="main">
 <table border="0" width="100%">
  <tbody>
	<tr>
	  <td align="center" valign="middle">
		<table border="0" width="100%">
		  <tr height=20><td colspan=5 align=center>
			<div class=\"plan\" id=\"plan\">

			<!- XGRID -->
				<link rel="stylesheet" type="text/css" href="dhtmlx/dhtmlx.css">
				<script  src="dhtmlx/dhtmlx.js"></script>
				
				<div id="gridbox" style="width:920px;height:510px;overflow:hidden"></div> 
				<script>
				mygrid = new dhtmlXGridObject('gridbox');
				mygrid.setImagePath("./dhtmlx/imgs/");
				mygrid.setSkin("dhx_skyblue");
				
				<?php			
				switch ($action) {
					case "composants" :
							echo "
								mygrid.setHeader(\"Nom,Modele,Adresse,Description,Polling,Actif,Valeur,Graphe,Etat Date,Correction,Divers\");
								mygrid.setInitWidths(\"190,105,105,*,40,30,45,45,110,50,45\");
								mygrid.setColAlign(\"left,left,left,left,center,center,center,center,center,center,left\");
								mygrid.setColTypes(\"ed,co,ed,txt,ed,ch,link,link,ro,ed,ed\"); 
								mygrid.setColSorting(\"str,str,str,str,int,int,str,na,date,str,str\");
							";
							$resultat_tmp = mysql_query("SELECT * from composants_modele order by composants_modele_norme,composants_modele_nom");
    						while($row=mysql_fetch_array($resultat_tmp)){echo "mygrid.getCombo(1).put(".$row['composants_modele_id'].",\"".$row['composants_modele_norme']."-".$row['composants_modele_nom']."\");";}
							echo "
								mygrid.init();
								mygrid.loadXML(\"pages_admin/dhtmlx_get.php?action=composants\");
								myDataProcessor = new dataProcessor(\"pages_admin/dhtmlx_update_composants.php\");
								myDataProcessor.init(mygrid);
								myDataProcessor.enableUTFencoding(false);
								</script>
								<input type=\"button\" name=\"a1\" value=\"Ajouter\" onClick=\"mygrid.addRow((new Date()).valueOf(),['nom',1,'','',0,0,'',' ','2000-01-01 00:00:00','',''],0)\" class=\"formsubmit\">
								<input type=\"button\" name=\"a1\" value=\"Supprimer\" onClick=\"mygrid.deleteRow(mygrid.getSelectedId())\" class=\"formsubmit\">
							";
						break;
					case "relever" :
							echo "
								mygrid.setHeader(\"Composant,Date Heure,Valeur\");
								mygrid.setInitWidths(\"0,150,*\");
								mygrid.setColAlign(\"center,center,left\");
								mygrid.setColTypes(\"ro,ed,ed\"); 
								mygrid.setColSorting(\"str,date,str\");
								mygrid.enableSmartRendering(true,50);
								mygrid.init();
								mygrid.loadXML(\"pages_admin/dhtmlx_get.php?action=relever&compid=$compid\");
								myDataProcessor = new dataProcessor(\"pages_admin/dhtmlx_update_relever.php\");
								myDataProcessor.init(mygrid);
								myDataProcessor.enableUTFencoding(false);
								</script>
								<input type=\"button\" name=\"a1\" value=\"Ajouter\" onClick=\"mygrid.addRow((new Date()).valueOf(),[$compid,'2000-01-01 00:00:00',''],0)\" class=\"formsubmit\">
								<input type=\"button\" name=\"a1\" value=\"Supprimer\" onClick=\"mygrid.deleteRow(mygrid.getSelectedId())\" class=\"formsubmit\">
							";
					
						break;
				}
				
				?>

				<!--
				<a href="javascript:void(0)" onclick="mygrid.addRow((new Date()).valueOf(),[1,'','',3,1,1,1,0],-1)">add </a>/<a href="javascript:void(0)" onclick="if (mygrid.getSelectedId()) mygrid.deleteRow(mygrid.getSelectedId())"> delete selected</a>
		 		-->

			</div>
		  </td></tr>
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