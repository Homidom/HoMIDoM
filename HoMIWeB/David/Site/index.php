<?php
	include("./include_php/config.php");
	session_start();
	session_unset();
	if(getenv("HTTP_HOST")=="192.168.1.2" || getenv("HTTP_HOST")=="localhost") {
		$sql_verif_ident="select id from users where login='administrateur'";
		$req_verif_ident=mysql_query($sql_verif_ident)or die("Erreur d'identification.<br>".mysql_error()."");
		$nb_trouve=mysql_num_rows($req_verif_ident);
		if($nb_trouve==0){Header("location:./index.html?msg=".$msg_erreur."");}
		elseif($nb_trouve==1) {
			$admin_id=mysql_result($req_verif_ident,0);	
			//session_start();
			$_SESSION['user_id']=$admin_id;
			Header("location:homiweb.html");
		}
	} 
?>
<html>
<head>
	<title>HOMIWEB</title>
	<link href="./include_cssjs/style.css" rel="stylesheet" type="text/css" />
	<link rel="SHORTCUT ICON" href="favicon.ico" />
    <link rel="icon" type="image/ico" href="favicon.ico" />
</head>
<body>
<center>
<table width="950" style="margin:0;padding:0;border:1px solid #53555F;background-color:#FFFFFF;">
	<tr><td height="60" align="left" bgcolor="#30569D"><img src="./images/bandeau.jpg" width="950" height="60"></td></tr>
	<tr><td align="center">
	<br><br><br>
		<?php if(isset($_GET['msg'])) echo "<center><font color=#FF9C00><b>".$_GET['msg']."</b></font></center>"; ?>
	<br><br>  
	<form name="authentification" method="post" action="connexion.html">
		<div class="authentification">
        <table border="0" width="400">
          <tbody>
            <tr>
              <td align="center" valign="middle" width="45%"><img src="images/keys.gif"><br /><br />Veuillez vous identifier<br />pour acc&eacute;der &agrave; <br />DOMOS<br /></td>
			  <td align="left" valign="middle" width="55%">
                <table border="0">
                    <tr>
                      <td class="titre" align="left"> Identification </td>
                    </tr>
                </table>
                <table width="80%" cellpadding="2" class="adminform">
                    <tr><td width="100%" align="left" valign="middle"><br /><b> Login : </b></td></tr>
					<tr><td><input type="text" name="login" class="identification"></td></tr>
					<tr><td width="100%" align="left" valign="middle"><br /><b> Mot de passe : </b></td></tr>
					<tr><td><input type="password" name="password" class="identification"></td></tr>
					<tr><td width="100%" height="51" colspan="2" align="right" valign="middle"><input type="submit" value="Se connecter" class="bouton"></td></tr>
                </table>
			  </td>
            </tr>
          </tbody>
        </table>
      </div>
	<input type="hidden" name="etape" value="identification">
	</form>
	<br /><br /><br /><br /><br /><br />
	</td>
  </tr>
</table>
</center>
</body>
</html>