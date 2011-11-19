<?php
	if(isset($_SESSION['user_id'])){
		include("./include_php/config.php");
		$resultat = mysql_query("select config_valeur from config where config_nom='menu_seticone'");
		$menuset = mysql_result($resultat,0,"config_valeur");

		$action=isset($_GET["action"])?$_GET["action"]:(isset($_POST["action"])?$_POST["action"]:"voir");
?>
<div class="main">
 <table border="0" width="100%">
  <tbody>
    <tr>
      <td align="center" valign="middle">

	<!-- TITRE -->
        <table border="0" width="100%">
            <tr>
              <td class="titre" align="left"><img src="images/menu/<?php echo "$menuset"; ?>/logs.png" width="32" height="32" border="0"> Logs </td>
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
	case "voir" :
		$logs_page=isset($_GET["logs_page"])?$_GET["logs_page"]:1;
		$resultat = mysql_query("select config_valeur from config where config_nom='logs_nbparpage'");
		$limite = mysql_result($resultat,0,"config_valeur");
		
		$nblogs_jour = mysql_fetch_row(mysql_query("select count(1) from logs where logs_date>'".date ('Y-m-d', strtotime (date("Y/m/d")))."'"));
		$nblogs_jour = $nblogs_jour[0];
		$nblogs_semaine = mysql_fetch_row(mysql_query("select count(1) from logs where logs_date>'".date ('Y-m-d', strtotime (date("Y/m/d"))-((7-1)*24*60*60))."'"));
		$nblogs_semaine = $nblogs_semaine[0];
		$nblogs_mois = mysql_fetch_row(mysql_query("select count(1) from logs where logs_date>'".date ('Y-m-d', strtotime (date("Y/m/d"))-((31-1)*24*60*60))."'"));
		$nblogs_mois = $nblogs_mois[0];
		$nblogs_tous = mysql_fetch_row(mysql_query("select count(1) from logs"));
		$nblogs_tous = $nblogs_tous[0];
		
		echo "
			<tr><td align=center><div class=\"plan\" id=\"plan\" style=\"padding:5px;\">
				<link rel=\"stylesheet\" type=\"text/css\" href=\"dhtmlx/dhtmlx.css\">
				<script src=\"dhtmlx/dhtmlx.js\"></script>
				<div id=\"gridbox\" style=\"width:920px;height:510px;overflow:hidden\"></div> 
				<input type=\"button\" name=\"a1\" value=\"Ajouter\" onClick=\"mygrid.addRow('WEB',(new Date()).valueOf(),['2000-01-01 00:00:00',''],0)\" class=\"formsubmit\">
				<input type=\"button\" name=\"a1\" value=\"Supprimer\" onClick=\"deletee()\" class=\"formsubmit\">
				<input type=\"button\" name=\"a1\" value=\"Purger\" onClick=\"javascript:window.location.href='logs-purger.html'\" class=\"formsubmit\">
				&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				<a href='javascript:refresh(1,$nblogs_jour,1)' title='$nblogs_jour Logs'> Jour </a> |
				<a href='javascript:refresh(7,$nblogs_semaine,1)' title='$nblogs_semaine Logs'> Semaine </a> |
				<a href='javascript:refresh(30,$nblogs_mois,1)' title='$nblogs_mois Logs'> Mois </a> |
				<a href='javascript:refresh(0,$nblogs_tous,1)' title='$nblogs_tous Logs'> Tous </a>
				&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				<a href='javascript:refresh2(\"logs_source\",\"2\")' title='$limite dernières Erreurs'> Err </a> |
				<a href='javascript:refresh2(\"logs_description\",\"Alert\")' title='$limite dernières Alertes'> Alert </a>
				&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				<span id='pages' style='display: inline-block;width:250px;align:center;text-align:center;'> </span>
		
				<script>
					var mygrid;
					var limitmin;
					var limitmax;
					var nblogs;
					var nbjour;
					var numpage;
					var timeoutHnd;
					limitmin=0;
					limitmax=$limite;
					nblogs=$nblogs_jour;
					nbjour=1;
					numpage=1;
					nompage=1;
					numpageaffiche=7;
					timeoutHnd='';
					function setpages() {
						var span = document.getElementById('pages');
						if (limitmax < nblogs) {
							texte='';
							if (numpage!=1) {texte='<a href=javascript:refresh('+nbjour+','+nblogs+',' + (numpage-1) + ')><</a>';}
							else {texte='<';}
							//Affichage numero 1
							if (numpage==1) {texte=texte + ' <b>1</b>';}
							else {texte=texte + ' <a href=javascript:refresh('+nbjour+','+nblogs+',1)>1</a>';}
	
							if (numpage>(numpageaffiche-2)) {
								minpage=(numpage-((numpageaffiche-1)/2))*limitmax;
								if (numpage>=(Math.ceil(nblogs/limitmax)-((numpageaffiche-1)/2))) {minpage=(Math.ceil(nblogs/limitmax)-numpageaffiche)*limitmax}
								texte=texte + ' .';
								if (numpage>=(Math.ceil(nblogs/limitmax)-((numpageaffiche-1)/2)-1)) {texte=texte + '..';}
							} else {minpage=2*limitmax;}
							if (numpage<((nblogs/limitmax)-((numpageaffiche-1)/2)-1)) {
								maxpage=((numpage+((numpageaffiche-1)/2)+1)*limitmax);
								if (numpage<=((numpageaffiche-1)/2)+2) {maxpage=(numpageaffiche+2)*limitmax}
							} else {maxpage=nblogs;}
							for (i=minpage; i<maxpage; i=i+limitmax) {
								nompage=i/limitmax;
								if (nompage<=9) {nompage2='&nbsp;'+nompage;} else {nompage2=nompage;}
								if (nompage==numpage) {texte=texte + ' <b>' + nompage2 + '</b>';}
								else {texte=texte + ' <a href=javascript:refresh('+nbjour+','+nblogs+','+nompage+')>' + nompage2 + '</a>';}
							}
							if (numpage<(Math.ceil(nblogs/limitmax)-((numpageaffiche-1)/2)-1)) {
								texte=texte + ' .';
								if (numpage<=(numpageaffiche-2)) {texte=texte + '..';}
							}
	
							//Affichage dernier numero
							numpagemax=Math.ceil(nblogs/limitmax);
							if (numpage==numpagemax) {texte=texte + ' <b>'+numpagemax+'</b>';}
							else {texte=texte + ' <a href=javascript:refresh('+nbjour+','+nblogs+','+numpagemax+')>'+numpagemax+'</a>';}
		
							if (numpage!=(nompage+1)) {texte=texte + ' <a href=javascript:refresh('+nbjour+','+nblogs+','+(numpage+1)+')>></a>';}
							else {texte=texte + ' >';}
							span.innerHTML = texte;
						} else {
							span.innerHTML = '';
						}
					}

					function refresh(nbjou,nblog,numpag) {
						nbjour=nbjou;
						numpage=numpag;
						limitmin=(numpage-1) * limitmax;
						nblogs=nblog;
						mygrid.clearAll();
						mygrid.loadXML('pages/dhtmlx_get.php?action=logs&nbjour='+nbjour+'&limitmin='+limitmin+'&limitmax='+limitmax);
						setpages();
					}
					function refresh2(champ,texte) {
						window.clearTimeout(timeoutHnd);
						mygrid.clearAll();
						mygrid.loadXML('pages/dhtmlx_get.php?action=logs2&champ='+champ+'&texte='+texte+'&limite='+limitmax);
						var span = document.getElementById('pages');
						span.innerHTML = '';
					}
					function deletee() {
						x = mygrid.getSelectedId();
						//mygrid.deleteRow(mygrid.getSelectedId());
						mygrid.deleteSelectedRows();
						if (x>1) {x=x-1;}
						mygrid.selectid(x);
					}
					mygrid = new dhtmlXGridObject('gridbox');
					mygrid.setImagePath('./dhtmlx/imgs/');
					mygrid.setSkin('dhx_skyblue');
					mygrid.setHeader('Src,Date,Description');
					mygrid.attachHeader('&nbsp;,&nbsp;,#text_filter');
					mygrid.setInitWidths('50,130,*');
					mygrid.setColAlign('center,center,left');
					mygrid.setColTypes('ed,ed,ed'); 
					mygrid.setColSorting('str,str,str');
					mygrid.enableMultiselect(true);
					mygrid.init();
					mygrid.enableSmartRendering(true,50);
					mygrid.loadXML('pages/dhtmlx_get.php?action=logs&nbjour=1&limitmin='+limitmin+'&limitmax='+limitmax);
					myDataProcessor = new dataProcessor('pages/dhtmlx_update_logs.php');
					myDataProcessor.init(mygrid);
					myDataProcessor.enableUTFencoding(false);
					timeoutHnd = setTimeout(setpages, 200);
					timeoutHnd = setTimeout(setpages, 500);
					timeoutHnd = setTimeout(setpages, 1000);
					timeoutHnd = setTimeout(setpages, 10000);
					timeoutHnd = setTimeout(setpages, 30000);
				</script>
			</div></td></tr>
		";
		break;
		
case "purger" :
	$mois=date("m")-2;
	if ($mois>0) {
		if ($mois<=9) {$mois="0".$mois;}
		$logs_date=date("Y")."/".$mois."/".date("d");
	}
	else {
		$mois=12+$mois;
		if ($mois<=9) {$mois="0".$mois;}
		$annee=date("Y")-1;
		$logs_date=$annee."/".$mois."/".date("d");
	}
	echo "<tr height=\"23\" bgcolor=\"#5680CB\"><td align=left class=\"titrecolonne\"> &nbsp;..:: Purger les LOGS ::..</td><td align=right class=\"titrecolonne\"> &nbsp;<a href='logs.html'><img src='./images/plus.gif' border='0'> Liste</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>\n";
	
	echo "<form name=purger_offres action=\"logs-purgerlogs.html\" method=\"post\">
		<tr>
		  <td align=left colspan=2><br/><br/>Pour information, les logs datant de plus d'un mois sont purgés automatiquement tous les jours.<br/><br/></td>
		</tr>
		<tr>
		  <td align=left colspan=2><br/><br/>Supprimer les logs datant d'avant le :<br/><br/></td>
		</tr>
		<tr>
		  <td colspan=2>&nbsp;&nbsp;&nbsp;<INPUT name=logs_date size=20 type=text value=\"$logs_date\"></td>
		</tr>
		<tr>
		  <td colspan=2>&nbsp;&nbsp;&nbsp;<input type=submit value=purger class=formsubmit><br/><br/><br/><br/></td>
		</tr></form>";
	break;

case "purgerlogs" :
	$logs_date=@$_POST["logs_date"];
	$resultat=mysql_query("delete from logs where logs_date<'$logs_date'");
	echo "<tr height=\"23\" bgcolor=\"#5680CB\"><td align=left class=\"titrecolonne\"> &nbsp;..:: Purger les LOGS ::..</td><td align=right class=\"titrecolonne\"> &nbsp;<a href='logs.html'><img src='./images/plus.gif' border='0'> Liste</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>\n";
	echo "<tr><td colspan=2><br /><br /><br /><br />Les logs ont été purgés !<br /><br /><br /><br /><br /></tr></td>";
	break;
	
default :
	echo "<tr><td>Choisis une action ci dessus !</td></tr>";
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
