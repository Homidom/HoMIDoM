<?php
	include ("./include/fonctions.php");
	include ("./include/homiclass.php");
	include ("./include/homisoap.php");
	session_start();
	session_unset();
	
	//Validation Extension SOAP
	$validsoap=false;
	if(extension_loaded('soap')) {
		$validsoap=true;
		//on charge la BDD connexion.xml
		$dom = new DomDocument();
		$FicExist = @$dom->load('include/connexion.xml');
		//si la bdd existe, on recupere les infos de connexion 
		if($FicExist) {
			$ListeServer = $dom->getElementsByTagName('SERVER');
			$server = $ListeServer->item(0);
			$_SESSION['server_ip']=$server->getAttribute("IP");
			$_SESSION['server_port']=$server->getAttribute("PORT");
			$_SESSION['server_id']=$server->getAttribute("ID");
			//on verifie si il n'y a pas de message d'erreur (cela veut dire qu'on a déjà tenté une connexion qui a échoué)
			If(!isset($_GET['msg'])) {
				//On teste si on accéde au site via le reseau local
				if(substr(getenv("HTTP_HOST"),0,8)=="192.168." || getenv("HTTP_HOST")=="localhost") {
					//si reseau local, on va direct à la bonne page
					Header("location:connexion.html");
				}
			}
			//sinon on affiche la fenetre de connexion
		}
	}
        

?>

<!DOCTYPE html>
<html lang="fr-FR">

<head>
	<meta charset="UTF-8" />
	<title>HoMIWeB</title>
	<link href="bundle.css" media="screen" rel="stylesheet" type="text/css" />
	<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript"></script>
	<script src="javascripts/vendor/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
</head>

<body>

	<img alt="" class="background-light" src="images/background-light.png" />
	<div class="container">
		<div class="header">
			<div class="wrapper by-center">
				<img src="images/homidom_title.png" />
			</div>
		</div>
		<?php
		//l'extension SOAP est OK
		If ($validsoap) {
			//On verifie si on a un message provenant d'une autre page (erreur)
			if(isset($_GET['msg'])) {
				?>
					<div class="wrapper by-center">
						<div class="crosslink">
						<img alt="Warning" src="images/warning.png" /><br /><br />
						<?php echo html_entity_decode($_GET['msg']); ?>
						</div>
					</div>
				<?php
			}
			?>
			<div class="wrapper by-center" style="width:300px;">
				<form name="authentification" id="authentification" method="post" action="connexion.html">
					<div class="text dark mbl"><input name="ident_ip" width="200" placeholder="<?php if(isset($_SESSION['server_ip'])){echo $_SESSION['server_ip'];}else{echo "Adresse IP (localhost)";}; ?>" tabindex="1" type="text" /></div>
					<div class="text dark mbl"><input name="ident_port" width="200" placeholder="<?php if(isset($_SESSION['server_port'])){echo $_SESSION['server_port'];}else{echo "Port (7999)";} ?>" tabindex="2" type="text" /></div>
					<div class="text dark mbl"><input name="ident_id" width="200" placeholder="Serveur ID" tabindex="3" type="password" /></div>
					<div class="right"><input type="submit" class="button positive" value="Seconnecter" /></div>
				</form>
			</div>
			<?php
		} else {
			?>
				<div class="wrapper by-center">
					<div class="crosslink">
					<img alt="Warning" src="images/warning.png" /><br /><br />
					L'extension PHP SOAP n'est pas chargéecute;e, impossible d'utiliser HoMIWeB sans celle-ci !
					</div>
				</div>
			<?php
		}
		?>
	</div>
</body>

</html>
