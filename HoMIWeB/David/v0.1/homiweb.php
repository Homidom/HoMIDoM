<?php
	session_start();

	if(isset($_SESSION['server_valid'])){
		include ("./include/fonctions.php");
		include ("./include/homiclass.php");
		include ("./include/homisoap.php");

?>
<!DOCTYPE html>
<html lang="fr-FR">

<head>
	<meta charset="UTF-8">
	<title>HoMIWeB</title>
	<link href="bundle.css" media="screen" rel="stylesheet" type="text/css" />
	<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript"></script>
	<script src="javascripts/vendor/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui.js" type="text/javascript"></script>
	<script src="javascripts/vendor/chosen.jquery.js" type="text/javascript"></script>
	<script src="javascripts/vendor/jquery.placeholder.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/Equalizer.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/appreciate.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/commutator.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/datepicker.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/pagination.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/scrollbox.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/slideshow.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/tabbox.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/starbar.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/checkbox.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/radio.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/player.js" type="text/javascript"></script>
	<script src="javascripts/SCF.ui/currentlyPlaying.js" type="text/javascript"></script>
</head>

<body>

	<img alt="" class="background-light" src="images/background-light.png" />
	<div class="container">

<?php
	$homidom = new HomidomSoap($_SESSION['server_ip'], $_SESSION['server_port'], $_SESSION['server_id'],true);
	//on se connecte au serveur SOAP
	if(!($homidom->connect())) {
		//non connecté : ERREUR
		?>
			<div class="header">
				<div class="wrapper by-center">
					<img src="images/homidom_title.png" />
				</div>
			</div>
			<div class="wrapper by-center">
				<div class="crosslink">
				<img alt="Warning" src="images/warning.png" /><br /><br />
				Connexion impossible au serveur <?php echo $_SESSION['server_ip'].":".$_SESSION['server_port'] ?> !<br />Veuillez v&eacute;rifier si celui-ci est d&eacute;marr&eacute;.<br /><br />
				<a href="index.html" alt="Retour"> RETOUR </a>
				</div>
			</div>';
			<?php
	} else {

?>

<div class="content">
	<div class="wrapper clearfix">
		<div class="colonne1">
			<?php
				$dom = new DomDocument();
				$FicExist = @$dom->load('include/zones.xml');
				//si la bdd existe, on recupere les infos des zones 
				if($FicExist) {
					$ListeZones = $dom->getElementsByTagName('ZONE');
					foreach($ListeZones as $zone) {
						echo "<A HREF=\"zone_" . $zone->getAttribute("ID") . ".html\"><CENTER><IMG SRC=\"".$zone->getAttribute("IMAGE")."\" HEIGHT=\"50\" WIDTH=\"50\" BORDER=\"0\" TITLE=\"".$zone->getAttribute("NAME")."\"></CENTER></A><br>";
					}
				} else {
					//le fichier xml n'existe pas, on récupére les infos en SOAP et on enregistre le XML
					$ListeHomiZone=$homidom->GetAllZones(); // On récupère la liste des zones dans Homidom (SOAP)
					//$ListeHomiZone->Zone = array($ListeHomiZone->Zone);
					$dom = new DOMDocument();
					$NewZones = $dom->createElement('ZONES');
					$dom->appendChild($NewZones);
					foreach($ListeHomiZone->Zone as $k=>$v) {
						// On affiche l'icone de la zone
						//echo "<A HREF=\"zone-" . str_replace("-","",$v->_Id) . ".html\">";
						echo "<A HREF=\"zone_" . $v->_Id . ".html\"><CENTER><IMG SRC=\"images/zone-defaut.png\" HEIGHT=\"50\" WIDTH=\"50\" BORDER=\"0\" TITLE=\"".html_entity_decode($v->_Name)."\"></CENTER></A><br>";
						// On ajoute la zone dans le XML
						$NewZone = $dom->createElement('ZONE');
						$NewZone->setAttribute("ID",$v->_Id);
						$NewZone->setAttribute("NAME",$v->_Name);
						$NewZone->setAttribute("IMAGE","images/zone-defaut.png");
						$NewZones->appendChild($NewZone);
					}
					$dom->save('include/zones.xml');
				}

			?>
			<!-- Menu Parametre -->
			<A HREF="parametres.html"><CENTER><IMG SRC="images/param.png" HEIGHT="50" WIDTH="50" BORDER="0" ALT="PARAMETRES"></CENTER></A><br>
			
		</div>
		<div class="last-col">
<?php
	if(isset($_GET['page']) || isset($_POST['page'])){$page=isset($_GET['page'])?$_GET['page']:$_POST['page'];}else{$page="accueil";};
	if(file_exists("./pages/".$page.".php")){
		include("./pages/".$page.".php");
	}else{
		?>
		<div class="crosslink">
		<img alt="Warning" src="images/warning.png" /><br /><br />
		La page demand&eacute;e n'est pas disponible
		</div>
		<?php
	};
?>
		</div>
	</div>
</div>

</body>
</html>

<?php
	}
}else{header("location:index.html");}
?>
