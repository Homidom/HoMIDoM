<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Frameset//EN">

<html>
<head>
<title>Homiweb Light</title>
<style type="text/css"> 
		body { 
			margin: 0; 
			padding: 0; 
			border: 0;
			background-color: #000000;
			overflow: auto; 
		} 
		 
		DIV#Header { 
			position: absolute; 
			margin: 0 0 0 0;
			padding: 0px;
			width: 640px;
			top: 0; 
			left: 250px; 
			background-color: #000000;
			overflow: auto; 
		} 
 
		DIV#Menu { 
			position: absolute; 
			margin: 0 0 0 0;
			padding: 0px;
			width: 110px;
			top: 100px; 
			left: 0; 
			background-color: #000000;
			overflow: auto; 
		} 

		DIV#MyBody { 
			position: absolute; 
			top: 110px; 
			left: 300px; 
			width: 600px;
			height: 600px;
			background-color: #000000;
			overflow: auto; 
		} 
		
		DIV#DEVICENOXML { 
			position: absolute; 
			top: 720px; 
			left: 300px; 
			width: 600px;
			height: 150px;
			background-color: #000000;
			overflow: auto; 
		} 

		H1 {
			font-family:"Times New Roman",Times,Serif;
			color : #FFFFFF;
			font-size : 16pt;
		}

		H2 {
			font-family:"Times New Roman",Times,Serif;
			color : #FFFFFF;
			font-size : 14pt;
		}

		H3 {
			font-family:"Times New Roman",Times,Serif;
			color : #FFFFFF;
			font-size : 12pt;
		}
		
		H4 {
			font-family:"Times New Roman",Times,Serif;
			color : #FFFFFF;
			font-size : 10pt;
		}
		
		.ContextMenu { 
		cursor:default; 
		font:menutext; 
		position:absolute; 
		text-align:left; 
		font-family: Arial, Helvetica, sans-serif; 
		font-size: 10; 
		width:150px; 
		background-color:menu; 
		border:1 solid buttonface; 
		visibility:hidden; 
		border:2 outset buttonhighlight; 
		} 

		.ContextMenuOver{ 
		color:white; 
		font-size:11; 
		font-weight:700; 
		background:midnightblue; 
		} 

		.ContextMenuOut{ 
		color:black; 
		font-size:11; 
		font-weight:500; 
		background:; 
		} 


	</style>

	
	<script language="JavaScript"> 
	function MontrerMenu(strMenu) { 
	// Disance par rapport aux bords de la fenetre 
	var EspaceDroit = document.body.clientWidth-event.clientX; 
	var EspaceBas = document.body.clientHeight-event.clientY; 
	var CMenu = document.getElementById(strMenu);
	// Affichage du menu suivant la position du curseur 
	if (EspaceDroit < CMenu.offsetWidth) 
	CMenu.style.left = document.body.scrollLeft + event.clientX - CMenu.offsetWidth; 
	else 
	CMenu.style.left = document.body.scrollLeft + event.clientX; 

	
	if (EspaceBas < CMenu.offsetHeight){ 
	CMenu.style.top = document.body.scrollTop + event.clientY - CMenu.offsetHeight; } 
	else{ 
	CMenu.style.top = document.body.scrollTop + event.clientY; } 

	// Affichage du menu 
	//CMenu.style.visibility = "visible"; 
	return false; 
	} 

	function MasquerMenu(strMenu){ 
	var CMenu = document.getElementById(strMenu);
//	CMenu.style.visibility = "hidden"; 
	} 
	</script> 

</head>
<body bgcolor="#000000" onclick="MasquerMenu('CMenu_SW-O-0')">
<DIV ID="Header">
<?php include("header.html");?>
</DIV>

<DIV ID="Menu">
<?php include("navigation.php");?>
</DIV>

<DIV ID="MyBody">
<?php include("homidom-web.php");?>
</DIV>

<DIV ID="DEVICENOXML"></DIV>

<?php
// ******************  Construction des menus pour les devices  ************************

if(!empty($_GET)) { // début test sur zoneid
	$zoneid=$_GET['zoneid'];
	if($zoneid) {
		foreach($ListeDevice as $device) {
			$ListeAction = $device->getElementsByTagName("ACTION");
			if($ListeAction->length > 0) {
?>
<span id="<?php echo "CMenu_" . $device->getAttribute("NAME")?>" class="ContextMenu" style="visibility:hidden;"> 
<table width="150" cellpadding="0" cellspacing="0" class="texte" style="display:inline;"> 
<?php	foreach($ListeAction as $action) {  ?>
  <tr height="20" onClick="ExecuteDevice('<?php echo $device->getAttribute("NAME"); ?>','<?php  echo $action->getAttribute("NAME"); ?>','<?php  echo $action->getAttribute("PARAM"); ?>');" 
	onMouseOut="this.className='ContextMenuOut';" onMouseOver="this.className='ContextMenuOver';"> 
  <td width="25"></td><td width="125"><?php echo $action->getAttribute("TEXT"); ?></td></tr> 
<?php	}	?>

</table> 
</span> 
		<?php
				}
			}
		}
	}
$homidom->disconnect(); 
?>
</BODY>
</html>
