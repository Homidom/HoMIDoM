<?PHP
//------------------------------------------------------------------------------------------
//Connexion à la base de donnée
//------------------------------------------------------------------------------------------

$serveur="localhost";
$user="homiweb";
$password="homiweb";
$dbname="homiweb";
$connexion = mysql_connect("$serveur","$user","$password");
if (!$connexion) {echo "impossible de se connecter au serveur, réessaye plus tard...";exit;}
$db = mysql_select_db("$dbname", $connexion);
if (!$db) {echo "Impossible de sélectionner cette base de données !!!";exit;}
?>
