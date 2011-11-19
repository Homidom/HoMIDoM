<?php
	include("./include_php/config.php");

	//récupération du login
	if(get_magic_quotes_gpc()){$login=trim(htmlentities($_POST['login']));}
	else{$login=trim(htmlentities(addslashes($_POST['login'])));}
	//récupération du mot de passe
	if(get_magic_quotes_gpc()){$password=trim(htmlentities($_POST['password']));}
	else{$password=trim(htmlentities(addslashes($_POST['password'])));}
	//codage md5 du mot de passe
	//$password=md5($password);
	
	//recherche dans la base de données 
	$sql_verif_ident="select id from users where login='".$login."' and pwd='".$password."'";
	$req_verif_ident=mysql_query($sql_verif_ident)or die("Erreur d'identification.<br>".mysql_error()."");
	$nb_trouve=mysql_num_rows($req_verif_ident);
	//message d'erreur en cas d'identifiants non valides
	$msg_erreur="Login ou mot de passe incorrect : $login $password";
	//si aucun résultat, on redirige vers la page d'accueil en affichant l'erreur	
	if($nb_trouve==0){Header("location:./index.html?msg=".$msg_erreur."");}
	//sinon ouverture d'une session et enregistrement de l'id de l'utilisateur
	elseif($nb_trouve==1)
	{
		$admin_id=mysql_result($req_verif_ident,0);	
		session_start();
		$_SESSION['user_id']=$admin_id;
		Header("location:homiweb.html");
	}
?>