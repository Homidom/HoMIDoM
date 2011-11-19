<?php
	if(isset($_SESSION['user_id'])){
?>
<div class="main">
 <table border="0" width="100%">
  <tbody>
    <tr>
      <td align="center" valign="middle">

	<!-- TITRE -->
        <table border="0" width="100%">
            <tr>
              <td class="titre" align="left"><img src="images/_composants.gif" width="32" height="32" border="0"> Divers </td>
              <td align="right"> </td>
            </tr>
        </table>

	<!-- CONTENU -->
        <table width="92%" class="adminform">
          <tbody>
            <tr>
              <td width="100%" align="left" valign="top">
				<table border="0" cellspacing="0" cellpadding="0" width="100%">
				  <tr><td align=center>
				  	<div class="plan" id="plan" style="padding:5px;">
<?php

include("./include_php/config.php");
$resultat = mysql_query("select config_valeur from config where config_nom='socket_ip'");
$adresse = mysql_result($resultat,0,"config_valeur");
$resultat = mysql_query("select config_valeur from config where config_nom='socket_port'");
$port = mysql_result($resultat,0,"config_valeur");
$resultat = mysql_query("select config_valeur from config where config_nom='socket_portgui'");
$portgui = mysql_result($resultat,0,"config_valeur");

echo "<script type=\"text/javascript\">
	function afficherResultats_socket(obj) {
		document.getElementById('ack').innerHTML=obj.responseText;
	}
	function sendsocket(message) {
		document.getElementById('ack').innerHTML='In progress';
		var XHR = new XHRConnection();
		XHR.appendData('tache', \"socket\");
		XHR.appendData('message', message);
		XHR.appendData('adresse', \"$adresse\");
		XHR.appendData('port', \"$port\");
		XHR.sendAndLoad('pages/actions.php', 'POST', afficherResultats_socket);
	}
	function sendsocketgui(message) {
		document.getElementById('ack').innerHTML='In progress';
		var XHR = new XHRConnection();
		XHR.appendData('tache', \"socket\");
		XHR.appendData('message', message);
		XHR.appendData('adresse', \"$adresse\");
		XHR.appendData('port', \"$portgui\");
		XHR.sendAndLoad('pages/actions.php', 'POST', afficherResultats_socket);
	}
	function sendtexte() {
		document.getElementById('ack').innerHTML='In progress';
		sendsocket(document.sendformaction.texte.value);
	}
	function sendsql() {
		document.getElementById('ack').innerHTML='In progress';
		var XHR = new XHRConnection();
		XHR.appendData('tache', \"sql\");
		XHR.appendData('message', document.sendformsql.texte.value);
		XHR.sendAndLoad('pages/actions.php', 'POST', afficherResultats_socket);
	}
	function senddumpreleve(compress) {
		document.getElementById('ack').innerHTML='In progress';
		var XHR = new XHRConnection();
		XHR.appendData('tache', \"dump\");
		XHR.appendData('detail', \"composantreleve\");
		XHR.appendData('composants_id', document.sendformreleve.texte.value);
		XHR.appendData('compression', compress);
		XHR.sendAndLoad('pages/actions.php', 'POST', afficherResultats_socket);
	}
	function senddump(message,compress) {
		document.getElementById('ack').innerHTML='In progress';
		var XHR = new XHRConnection();
		XHR.appendData('tache', \"dump\");
		XHR.appendData('detail', message);
		XHR.appendData('compression', compress);
		XHR.sendAndLoad('pages/actions.php', 'POST', afficherResultats_socket);
	}
	function refreshselectdump() {
		document.getElementById('ack').innerHTML='In progress';
		var XHR = new XHRConnection();
		XHR.appendData('tache', \"refreshselectdump\");
		XHR.sendAndLoad('pages/actions.php', 'POST', refreshselectdump_post);
	}
	function refreshselectdump_post(obj) {
		var tabResult = obj.responseXML.getElementsByTagName('resultat');
		result='<select name=selecteddump>';
		if (tabResult.length > 0) {
			for (var i = 0; i < tabResult.length; i++) {
				resultat = tabResult.item(i);
				result=result + '<option>' + resultat.getAttribute('fichier') +  '</option>';
			}
		}
		result=result+'</select>';
		document.getElementById('selectdump').innerHTML=result;
		document.getElementById('ack').innerHTML='OK';
	}
	function restaurerbackup() {
		document.getElementById('ack').innerHTML='In progress';
		var XHR = new XHRConnection();
		XHR.appendData('tache', \"restaurerbackup\");
		XHR.appendData('fichier', document.restobackup.selecteddump.options[document.restobackup.selecteddump.selectedIndex].value);
		XHR.sendAndLoad('pages/plan_actions.php', 'POST', afficherResultats_socket);
	}

</script>";

echo "
Arrêt du service : <a href=javascript:sendsocket(\"([AS#stop])\")>STOP</a> <br /><br />
Redémarrage du service : <a href=javascript:sendsocket(\"([AS#restart])\")>RESTART</a> <br /><br />
GUI :  Domos service : <a href=javascript:sendsocketgui(\"start\")>Start</a> / <a href=javascript:sendsocketgui(\"stop\")>Stop</a> / <a href=javascript:sendsocketgui(\"restart\")>Restart</a> <br /><br />
Maj des tables : <a href=javascript:sendsocket(\"([AS#maj_all])\")>ALL</a> - <a href=javascript:sendsocket(\"([AS#maj_composants])\")>Composants</a> - <a href=javascript:sendsocket(\"([AS#maj_composants_bannis])\")>Cmp bannis</a> - <a href=javascript:sendsocket(\"([AS#maj_macro])\")>Macros</a> - <a href=javascript:sendsocket(\"([AS#maj_timer])\")>Timers</a> <br /><br />
Affichage des tables : <a href=javascript:sendsocket(\"([AS#afftables])\")>AFFTABLES</a> <br /><br />
Envoyer une action : <form name='sendformaction'><input type='text' name='texte' value='([AL#test])' style='width: 500px;'/> <input type=button value='Send' class=\"formsubmit\" onClick='sendtexte();'></form>
Envoyer une requete SQL : <form name='sendformsql'><input type='text' name='texte' value=\"delete from logs where logs_description like '%empty%'\" style='width: 500px;'/> <input type=button value='Send' class=\"formsubmit\" onClick='sendsql();'></form>
<br /><br />
Dump SQL complet : <a href=javascript:senddump(\"complet\",\"true\")>Compréssé</a> | <a href=javascript:senddump(\"complet\",\"false\")>Normal</a> <br /><br />
Dump SQL avec insertion complete : <a href=javascript:senddump(\"complet_insertioncomplete\",\"true\")>Compréssé</a> | <a href=javascript:senddump(\"complet_insertioncomplete\",\"false\")>Normal</a> <br /><br />
Dump SQL complet sans les logs : <a href=javascript:senddump(\"completsanslogs\",\"true\")>Compréssé</a> | <a href=javascript:senddump(\"completsanslogs\",\"false\")>Normal</a> <br /><br />
Dump SQL complet sans les logs et relevés : <a href=javascript:senddump(\"completsanslogsreleves\",\"true\")>Compréssé</a> | <a href=javascript:senddump(\"completsanslogsreleves\",\"false\")>Normal</a> <br /><br />
Restaurer un backup SQL : <form name='restobackup'><div id='selectdump' class='selectdump'><select name='selecteddump' id='selecteddump'><option>Aucun</option></select></div><input type=button value='Restaurer' class=\"formsubmit\"  onClick='restaurerbackup()'> <input type='button' value='Refresh' class='formsubmit' onClick='refreshselectdump();'></form>
<br /><br />
Dump des relevés du composant ID : <form name='sendformreleve'><input type='text' name='texte' value='1' style='width: 100px;'/> <input type=button value='Send Compress' class=\"formsubmit\" onClick='senddumpreleve(true);'><input type=button value='Send Normal' class=\"formsubmit\" onClick='senddumpreleve(false);'></form>";

//Affichage du cadre ack
echo "<div class=\"ack\" id=\"ack\">RAS</div>";
echo "<script type=\"text/javascript\">
refreshselectdump();
</script>";
?>
				  	</div>
				  </td></tr>
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
