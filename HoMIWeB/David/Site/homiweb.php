<?php
	session_start();
	if(isset($_SESSION['user_id'])){
		include("./include_php/config.php");
		include ("./include_php/fonctions.php");
		$resultat = mysql_query("select config_valeur from config where config_nom='menu_seticone'");
		$menuset = mysql_result($resultat,0,"config_valeur");
?>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
	<title>HOMIWEB</title>
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
	<link href="./include_cssjs/style.css" rel="stylesheet" type="text/css" />
	<link rel="SHORTCUT ICON" href="favicon.ico" />
    <link rel="icon" type="image/ico" href="favicon.ico" />
    <link href="./include_cssjs/prettyPhoto.css" rel="stylesheet" type="text/css" media="all" />
	<script language="javascript" type="text/javascript" src="./include_cssjs/script_XHRConnection.js"></script>
	<script language="javascript" type="text/javascript" src="./include_cssjs/jquery-1.4.4.min.js"></script>
	<script language="javascript" type="text/javascript" src="./include_cssjs/contextmenu.js"></script>
	<script language="javascript" type="text/javascript" src="./include_cssjs/jquery.prettyPhoto.js"></script>
</head>
<body>
<!--PrettyPhoto-->
	<script type="text/javascript" charset="utf-8">
		$(document).ready(function(){
			$("a[rel^='prettyPhoto']").prettyPhoto({allow_resize:true,default_width:900,default_height:500,overlay_gallery:false,keyboard_shortcuts:true,show_title:true,animationSpeed:'slow',theme:'dark_rounded',slideshow:3000});
		});
	</script>

<!--Context menu-->
    <div class="contextMenu" id="myMenufisheye" style="display:none;">
      <ul>
      <?php
      echo "
        <li id=\"devices\"><img src=\"images/menu/$menuset/devices.png\" width=28/> devices</li>
        <li id=\"drivers\"><img src=\"images/menu/$menuset/drivers.png\" width=28/> drivers</li>
        <li id=\"zones\"><img src=\"images/menu/$menuset/zones.png\" width=28/> Zones</li>
        <li id=\"plans\"><img src=\"images/menu/$menuset/zones.png\" width=28 /> Plans</li>
        <li id=\"menu\"><img src=\"images/menu/$menuset/menu.png\" width=28 /> Menu</li>
        <li id=\"users\"><img src=\"images/menu/$menuset/users.png\" width=28 /> Users</li>
        <li id=\"webcamsadmin\"><img src=\"images/menu/$menuset/webcam.png\" width=28 /> Webcams</li>
        <li id=\"configuration\"><img src=\"images/menu/$menuset/config.png\" width=28 /> Configuration</li>
        <li id=\"phpinfo\"><img src=\"images/menu/$menuset/info.png\" width=28 /> phpinfo</li>
        ";
      ?>
      </ul>
    </div>
	<script type="text/javascript">
			$(document).ready(function() {
					$('a.contextItem').contextMenu('myMenufisheye', {
						menuStyle: {width: '120px'},
						itemStyle: {padding: '0px'},
						bindings: {
							'devices': function(t) {window.location.href='devices.html';},
							'drivers': function(t) {window.location.href='drivers.html';},
							'zones': function(t) {window.location.href='zones.html';},
							'plans': function(t) {window.location.href='plans.html';},
							'menu': function(t) {window.location.href='menu.html';},
							'users': function(t) {window.location.href='users.html';},
							'webcamsadmin': function(t) {window.location.href='webcamsadmin.html';},
							'configuration': function(t) {window.location.href='config.html';},
							'phpinfo': function(t) {window.location.href='phpinfo.html';}
						}
					});
			});
	</script>
<center>
<table width="950" style="margin:0;padding:0;border:1px solid #53555F;background-color:#FFFFFF;">
	<tr align="center" valign="middle" colspan="2"><td width="100%" height="5" style="background-image:url('images/bandeau_2.jpg');background-position:top left;background-repeat:no-repeat;">
		<div style="text-align:right;border:0px;width:600px">
		<ul class="thumb">
			<?php
				echo '<li><a href="homiweb.html" class="contextItem"><img src="images/menu/'.$menuset.'/homiweb.png" alt="Accueil"/></a></li>';
				$resultat = mysql_query("select * from zones where zones_ordre<>'' order by zones_ordre");
				if($resultat){
					while($row=mysql_fetch_array($resultat)){
						echo '<li><a href="zone-'.$row['zones_id'].'.html" class="contextItem"><img src="images/zones/icone/'.$row['zones_icone'].'" alt="'.$row['zones_nom'].'"/></a></li>';
					}
				}
				echo '
				<li><a href="meteo.html" class="contextItem"><img src="images/menu/'.$menuset.'/meteo.png" alt="Météo"/></a></li>
				<li><a href="calendrier.html" class="contextItem"><img src="images/menu/'.$menuset.'/calendrier.png" alt="Calendrier"/></a></li>
				<li><a href="logs.html" class="contextItem"><img src="images/menu/'.$menuset.'/logs.png" alt="Logs"/></a></li>
				<li><a href="webcams.html" class="contextItem"><img src="images/menu/'.$menuset.'/webcam.png" alt="Webcams"/></a></li>
				<li><a href="divers.html" class="contextItem"><img src="images/menu/'.$menuset.'/display.png" alt="Divers"/></a></li>
				';
			?>
		</ul>
		</div>
	</td>
  </tr>
<script type="text/javascript">
	$("ul.thumb li").hover(function() {
		$(this).css({'z-index' : '10'}); /*Add a higher z-index value so this image stays on top*/ 
		$(this).find('img').addClass("hover").stop() /* Add class of "hover", then stop animation queue buildup*/
			.animate({
				marginTop: '-40px', /* The next 4 lines will vertically align this image */ 
				marginLeft: '-60px',
				top: '100%',
				left: '100%',
				width: '70px', /* Set new width */
				height: '70px', /* Set new height */
				padding: '0px'
			}, 100); /* this value of "200" is the speed of how fast/slow this hover animates */
		} , function() {
		$(this).css({'z-index' : '10'}); /* Set z-index back to 0 */
		$(this).find('img').removeClass("hover").stop()  /* Remove the "hover" class , then stop animation queue buildup*/
			.animate({
				marginTop: '0', /* Set alignment back to default */
				marginLeft: '0',
				top: '0',
				left: '0',
				width: '40px', /* Set width back to default */
				height: '40px', /* Set height back to default */
				padding: '0px'
			}, 200);
	});
</script>
  <tr>
    <td colspan="2" align="center">
	<?php	//Inclusion des modules
	if(isset($_GET['page']) || isset($_POST['page'])){
		$page=isset($_GET['page'])?$_GET['page']:$_POST['page'];
		if(file_exists("./pages/".$page.".php")){include("./pages/".$page.".php");}else{echo"<center><b><br><br>La page demandée n'est pas disponible.<br><br></b></center>";};
	}
	//module par défaut
	else{
		include("./pages/accueil.php");
	} ?>
	</td>
  </tr>
</table>
</center>
</body>
</html>
<?php
	}else{
		header("location:index.html");
	}
?>
