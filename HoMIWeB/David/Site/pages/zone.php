<?php
	if(isset($_SESSION['user_id'])){ 
		include("./include_php/config.php");
		$zones_id=isset($_GET["zones_id"])?$_GET["zones_id"]:(isset($_POST["zones_id"])?$_POST["zones_id"]:"1");
		$resultat = mysql_query("select zones_plan from zones where zones_id='$zones_id'");
		$zones_plan = mysql_result($resultat,0,"zones_plan");
		
		$resultat = mysql_query("select config_valeur from config where config_nom='serveur_ip'");
		$adresse = mysql_result($resultat,0,"config_valeur");
		$resultat = mysql_query("select config_valeur from config where config_nom='serveur_port'");
		$port = mysql_result($resultat,0,"config_valeur");

?>
<div class="main">
 <table border="0" width="100%">
  <tbody>
    <tr>
      <td align="center" valign="middle">
        <table border="0" width="100%">

<!--Context menu-->
    <div class="contextMenu" id="myMenuplan">
      <ul>
        <li id="grapher"><img src="images/graphe.gif" width=16/> Graphes</li>
        <li id="relever"><img src="images/releve.png" width=16 /> Relevés</li>
        <li id="editer"><img src="images/modif.gif" width=16 /> Edition</li>
      </ul>
    </div>
    
<?php

echo "<script type=\"text/javascript\">
	function afficherResultats_releve(obj) {
		document.getElementById('valeurs').innerHTML='';
		var tabResult = obj.responseXML.getElementsByTagName('resultat');
		if (tabResult.length > 0) {
			for (var i = 0; i < tabResult.length; i++) {
				resultat = tabResult.item(i);
				
				var monspan = document.createElement('span');
				switch (resultat.getAttribute('type')) {
					case \"temp\":
						monspan.className = 'spanun';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-grapher-' + resultat.getAttribute('comp') + '.html');
						mona.setAttribute('title',resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						var monimg = document.createElement('img');
						monimg.setAttribute('src',\"./images/temp.png\");
						monimg.style.width = '12px';
						monimg.style.height = '12px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						texte=resultat.getAttribute('valeur')+\"°\";
						monaText = document.createTextNode(texte);
						mona.appendChild(monaText);
						monspan.appendChild(mona);
						break;

					case \"hyg\":
						monspan.className = 'spanun';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-grapher-' + resultat.getAttribute('comp') + '.html');
						mona.setAttribute('title',resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						var monimg = document.createElement('img');
						monimg.setAttribute('src',\"./images/water.png\");
						monimg.style.width = '12px';
						monimg.style.height = '12px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						texte=resultat.getAttribute('valeur')+\"%\";
						monaText = document.createTextNode(texte);
						mona.appendChild(monaText);
						monspan.appendChild(mona);
						break;

					case \"div\":
						monspan.className = 'spanun';
						monspan.style.width = '150px';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-relever-' + resultat.getAttribute('comp') + '.html');
						mona.setAttribute('title',resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						texte=resultat.getAttribute('valeur');
						monaText = document.createTextNode(texte);
						mona.appendChild(monaText);
						monspan.appendChild(mona);
						break;

					case \"voletaction\":
						monspan.className = 'spandeux';
						monspan.style.width = '12px';
						var mona = document.createElement('a');
						mona.setAttribute('href','javascript:sendsocket(\"([AC#' + resultat.getAttribute('comp') + '#OFF])\")');
						mona.setAttribute('title','Ouvrir ' + resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						var monimg = document.createElement('img');
						monimg.setAttribute('src',\"./images/s_up.png\");
						monimg.style.width = '12px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						var monbr = document.createElement('br');
						monspan.appendChild(monbr);
						var mona = document.createElement('a');
						mona.setAttribute('href','javascript:sendsocket(\"([AC#' + resultat.getAttribute('comp') + '#ON])\")');
						mona.setAttribute('title','Fermer ' + resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						var monimg = document.createElement('img');
						monimg.setAttribute('src',\"./images/s_down.png\");
						monimg.style.width = '12px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						break;

					case \"voletactionetat\":
						monspan.className = 'spandeux';
						monspan.style.width = '35px';
						var mona = document.createElement('a');
						mona.setAttribute('href','javascript:sendsocket(\"([AC#' + resultat.getAttribute('comp') + '#OFF])\")');
						mona.setAttribute('title','Ouvrir ' + resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						var monimg = document.createElement('img');
						monimg.setAttribute('src',\"./images/s_up.png\");
						monimg.style.width = '12px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						var monbr = document.createElement('br');
						monspan.appendChild(monbr);	
						var mona = document.createElement('a');
						mona.setAttribute('href','javascript:sendsocket(\"([AC#' + resultat.getAttribute('comp') + '#ON])\")');
						mona.setAttribute('title','Fermer ' + resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						var monimg = document.createElement('img');
						monimg.setAttribute('src',\"./images/s_down.png\");
						monimg.style.width = '12px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						monspan.id = resultat.getAttribute('comp');
						monspan.style.top = (resultat.getAttribute('top')-1)+'px';
						monspan.style.left = (resultat.getAttribute('left')-20)+'px';
						document.getElementById('valeurs').appendChild(monspan);
						//span avec l'etat
						var monspan = document.createElement('span');
						monspan.className = 'spanun';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-relever-' + resultat.getAttribute('comp') + '.html');
						var monimg = document.createElement('img');
						if (resultat.getAttribute('valeur')==0) {
							monimg.setAttribute('src',\"./images/volet_ferme.gif\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' Fermé' + ' - ' + resultat.getAttribute('etatdate'));
						} else {
							monimg.setAttribute('src',\"./images/volet_ouvert.gif\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' Ouvert' + ' - ' + resultat.getAttribute('etatdate'));
						}
						monimg.style.width = '19px';
						monimg.style.height = '24px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);

						break;

					case \"voletetat\":
						monspan.className = 'spanun';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-relever-' + resultat.getAttribute('comp') + '.html');
						var monimg = document.createElement('img');
						if (resultat.getAttribute('valeur')==0) {
							monimg.setAttribute('src',\"./images/volet_ferme.gif\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' Fermé' + ' - ' + resultat.getAttribute('etatdate'));
						} else {
							monimg.setAttribute('src',\"./images/volet_ouvert.gif\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' Ouvert' + ' - ' + resultat.getAttribute('etatdate'));
						}
						monimg.style.width = '19px';
						monimg.style.height = '24px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						break;

					case \"jour\":
						monspan.className = 'spanun';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-relever-' + resultat.getAttribute('comp') + '.html');
						var monimg = document.createElement('img');
						if ((resultat.getAttribute('valeur')==0) || (resultat.getAttribute('valeur')=='ON')) {
							monimg.setAttribute('src',\"./images/jour_nuit.png\");
							mona.setAttribute('title','Nuit - ' + resultat.getAttribute('etatdate'));
						} else {
							monimg.setAttribute('src',\"./images/jour_jour.png\");
							mona.setAttribute('title','Jour - ' + resultat.getAttribute('etatdate'));
						}
						monimg.style.width = '60px';
						monimg.style.height = '60px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						break;

					case \"lampe\":
						monspan.className = 'spanun';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-relever-' + resultat.getAttribute('comp') + '.html');
						var monimg = document.createElement('img');
						if (resultat.getAttribute('valeur')=='OFF') {
							monimg.setAttribute('src',\"./images/ampoule_off.png\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' OFF' + ' - ' + resultat.getAttribute('etatdate'));
						} else if (resultat.getAttribute('valeur')=='ON') {
							monimg.setAttribute('src',\"./images/ampoule_on.png\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' ON' + ' - ' + resultat.getAttribute('etatdate'));
						} else if (resultat.getAttribute('valeur') = 0){
							monimg.setAttribute('src',\"./images/ampoule_off.png\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' OFF' + ' - ' + resultat.getAttribute('etatdate'));
						} else {
							monimg.setAttribute('src',\"./images/ampoule_on.png\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' ON-' + resultat.getAttribute('valeur') + ' - ' + resultat.getAttribute('etatdate'));
						}
						monimg.style.width = '16px';
						monimg.style.height = '16px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						break;

					case \"lampeaction\":
						//span INTER
						monspan.className = 'spandeux';
						monspan.style.width = '35px';
						var mona = document.createElement('a');
						mona.setAttribute('href','javascript:sendsocket(\"([AC#' + resultat.getAttribute('comp') + '#ON])\")');
						mona.setAttribute('title','Allumer ' + resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						var monimg = document.createElement('img');
						monimg.setAttribute('src',\"./images/s_on.gif\");
						monimg.style.width = '12px';
						monimg.style.height = '12px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						var monbr = document.createElement('br');
						monspan.appendChild(monbr);
						var mona = document.createElement('a');
						mona.setAttribute('href','javascript:sendsocket(\"([AC#' + resultat.getAttribute('comp') + '#OFF])\")');
						mona.setAttribute('title','Eteindre ' + resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						var monimg = document.createElement('img');
						monimg.setAttribute('src',\"./images/s_off.gif\");
						monimg.style.width = '12px';
						monimg.style.height = '12px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						monspan.id = resultat.getAttribute('comp');
						monspan.style.top = (resultat.getAttribute('top')-7)+'px';
						monspan.style.left = (resultat.getAttribute('left')-20)+'px';
						document.getElementById('valeurs').appendChild(monspan);
						//span avec l'ampoule
						var monspan = document.createElement('span');
						monspan.className = 'spanun';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-relever-' + resultat.getAttribute('comp') + '.html');
						var monimg = document.createElement('img');
						if (resultat.getAttribute('valeur')=='OFF') {
							monimg.setAttribute('src',\"./images/ampoule_off.png\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' OFF' + ' - ' + resultat.getAttribute('etatdate'));
						} else if (resultat.getAttribute('valeur')=='ON') {
							monimg.setAttribute('src',\"./images/ampoule_on.png\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' ON' + ' - ' + resultat.getAttribute('etatdate'));
						} else if (resultat.getAttribute('valeur') == 0){
							monimg.setAttribute('src',\"./images/ampoule_off.png\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' OFF' + ' - ' + resultat.getAttribute('etatdate'));
						} else {
							monimg.setAttribute('src',\"./images/ampoule_on.png\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' ON-' + resultat.getAttribute('valeur') + ' - ' + resultat.getAttribute('etatdate'));
						}
						monimg.style.width = '16px';
						monimg.style.height = '16px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						break;

					case \"mvt\":
						monspan.className = 'spanun';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-relever-' + resultat.getAttribute('comp') + '.html');
						var monimg = document.createElement('img');
						if (resultat.getAttribute('valeur')=='OFF') {
							monimg.setAttribute('src',\"./images/motion_off.gif\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' OFF' + ' - ' + resultat.getAttribute('etatdate'));
						} else {
							monimg.setAttribute('src',\"./images/motion_on.gif\");
							mona.setAttribute('title',resultat.getAttribute('compnom') + ' ON' + ' - ' + resultat.getAttribute('etatdate'));
						}
						monimg.style.width = '16px';
						monimg.style.height = '16px';
						monimg.style.border = '0px';
						mona.appendChild(monimg);
						monspan.appendChild(mona);
						break;

					default:
						monspan.className = 'spanun';
						var mona = document.createElement('a');
						mona.setAttribute('href','devices-relever-' + resultat.getAttribute('comp') + '.html');
						mona.setAttribute('title',resultat.getAttribute('compnom') + ' - ' + resultat.getAttribute('etatdate'));
						texte=resultat.getAttribute('valeur');
						monaText = document.createTextNode(texte);
						mona.appendChild(monaText);
						monspan.appendChild(mona);
						break;
				}
				monspan.id = resultat.getAttribute('comp');
				monspan.style.top = resultat.getAttribute('top')+'px';
				monspan.style.left = resultat.getAttribute('left')+'px';
				document.getElementById('valeurs').appendChild(monspan);
			}
		}
		timeoutID = window.setTimeout(\"window.loadData()\",5000);
		$(document).ready(function() {
			$('span').contextMenu('myMenuplan', {
				menuStyle: {width: '85px'},
				bindings: {
					'grapher': function(t) {contextmenu(t.id,'grapher');},
					'relever': function(t) {contextmenu(t.id,'relever');},
					'editer': function(t) {contextmenu(t.id,'editer');}
				}
			});
			$('span').live('click', function(e) {
				if( (!$.browser.msie && e.button == 0) || ($.browser.msie && e.button == 1) ) {
					//leftclick
					$(function() {
					  $('mymenuplan').display();
					});
				}
			});

		});
	}
	function contextmenu(compid,action) {
		if (action=='grapher') {window.location.href='devices-grapher-'+compid+'.html';}
		if (action=='relever') {window.location.href='devices-relever-'+compid+'.html';}
		if (action=='editer') {window.location.href='devices.html';}
	}
	function afficherResultats_socket(obj) {
		document.getElementById('ack').innerHTML=obj.responseText;
	}
	function loadData() {
		var XHR = new XHRConnection();
		XHR.appendData('tache', \"releve\");
		XHR.appendData('zones', \"$zones_id\");
		XHR.sendAndLoad('pages/actions.php', 'POST', afficherResultats_releve);	
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
	
</script>";

echo "<tr height=20><td colspan=5 align=center><div class=\"plan\" id=\"plan\">";

// Affichage du plan
echo "<img src=\"images/zones/plan/".$zones_plan."\" / class=\"image\"><div class=\"valeurs\" id=\"valeurs\"></div>";

//Affichage du cadre ack
echo "<div class=\"ack\" id=\"ack\">RAS</div>";

echo "</div></td></tr>";
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