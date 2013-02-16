<?php
if($FicExist) {
	$ListeServer = $dom->getElementsByTagName('SERVER'); // on récupère les paramètres du serveur
	$XMLServer = $ListeServer->item(0);
	$SERVER_IP = $server->getAttribute("IP");
	$SERVER_PORT = $server->getAttribute("PORT");
	$SERVER_ID = $server->getAttribute("ID");
}

?>
<script language="JavaScript"> 
document.getElementById('MyBody').style.height = 'auto';
</script>	
<CENTER>
<H1>Statut</H1>
</CENTER></BR>
<CENTER><TABLE WIDTH="50%" BORDER="1">
<TR><TD><H3 STYLE="color:#FFFFFF">Fichier de paramètres chargé</H3></TD>
<TD>
<?php 
if($FicExist) echo "<IMG SRC=\"images/check.png\">";
		else echo "<IMG SRC=\"images/cross.png\">"; ?>
</TD></TR>
<TR><TD><H3 STYLE="color:#FFFFFF">Connexion au serveur SOAP</H3></TD>
<TD>
<?php if($ConExist) echo "<IMG SRC=\"images/check.png\">";
		else echo "<IMG SRC=\"images/cross.png\">"; ?>
</TD></TR>
</TABLE>
</CENTER>
<CENTER>
<H1>Paramètres</H1>
</CENTER></BR>
<CENTER>
<FORM ACTION="save-param.php" METHOD="POST">
<TABLE BORDER="1">
<TR>
<TD><H2>Serveur SOAP</H2></TD>
<TD><TABLE>
<TR><TD valign="BOTTOM"><H4>Adresse IP</H4></TD><TD valign="TOP"><INPUT TYPE="TEXT" NAME="SERVER_IP" VALUE="<?php if($FicExist) echo $XMLServer->getAttribute("IP");?>" SIZE="15"></TD></TR>
<TR><TD valign="BOTTOM"><H4>Numéro de port</H4></TD><TD valign="TOP"><INPUT TYPE="TEXT" NAME="SERVER_PORT" VALUE="<?php if($FicExist) echo $XMLServer->getAttribute("PORT");?>" SIZE="15"></TD></TR>
<TR><TD valign="BOTTOM"><H4>Identifiant</H4></TD><TD valign="TOP"><INPUT TYPE="TEXT" NAME="SERVER_ID" VALUE="<?php if($FicExist) echo $XMLServer->getAttribute("ID");?>" SIZE="15"></TD></TR>
</TABLE></TD>
</TR>
</TABLE>
<?php
if($ConExist) {
?>
<TABLE BORDER="1">
<?php
	$XMLZone = false;
	$x=$homidom->GetAllZones();
	foreach($x->Zone as $k=>$v) {
		$XMLListZones = $XMLServer->getElementsByTagName('ZONE');
		foreach($XMLListZones as $tmpXMLZone) {
			if($tmpXMLZone->getAttribute("ID") == $v->_Id) {
				$XMLZone = $tmpXMLZone;
			}
		}
?>
<TR>
<TD colspan="2" VALIGN="TOP"><H2>ZONE : <?php echo utf8_decode($v->_Name) ?></H2>
<TABLE BORDER="0">
<TR><TD>
<FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">
ID : <?php echo utf8_decode($v->_Id) ?><BR>
Name : <?php echo utf8_decode($v->_Name) ?></FONT><BR><BR>
</TD>
<TD>
<CENTER><IMG SRC="<?php echo "imagezone.php?zoneid=" . utf8_decode($v->_Id) ?>" WIDTH="25%" HEIGHT="25%"></CENTER>
<INPUT TYPE="HIDDEN" NAME="ZONE_ID[]" VALUE="<?php echo utf8_decode($v->_Id)?>">
</TD>
</TR>
</TABLE>
</TD>
</TR>
<TR>
<TD></TD>
<TD width="5%">
<?php
	$x=$homidom->GetDeviceInZone($v->_Id)->TemplateDevice;
	for($i = 0; $i < count($x); $i++){
		if($XMLZone) {
			$XMLListDevices = $XMLZone->getElementsByTagName('HOMIDEVICE');
			$XMLDevice = false;
			foreach($XMLListDevices as $tmpXMLDevice) {
				if($tmpXMLDevice->getAttribute("ID") == $x[$i]->_ID)
					$XMLDevice = $tmpXMLDevice;
			}
		}
?><H3>Composant : <?php echo utf8_decode($x[$i]->_Name) ?></H3>
<H4>ID : <?php echo utf8_decode($x[$i]->_ID) ?><BR>
Name : <?php echo utf8_decode($x[$i]->_Name) ?><BR>
Valeur : <?php echo utf8_decode($x[$i]->_Value) ?></H4>
<INPUT TYPE="HIDDEN" NAME="<?php echo utf8_decode($v->_Id) . "DEVICE_ID[]"?>" VALUE="<?php echo utf8_decode($x[$i]->_ID)?>">
<TABLE border=0>
<TR>
<TD valign="MIDDLE" COLSPAN="9" align="LEFT"><FONT STYLE="color:#D87801;font-family:Times New Roman,Times,Serif;font-size : 10pt">Coordonnées</FONT></TD>
</TR>
<TR>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">X</FONT></TD>
<TD valign="MIDDLE"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_X"?>" VALUE="<?php if($XMLDevice) echo utf8_decode($XMLDevice->getAttribute("POSX"));?>" SIZE="4"></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Y</FONT></TD>
<TD valign="MIDDLE"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_Y"?>" VALUE="<?php if($XMLDevice) echo utf8_decode($XMLDevice->getAttribute("POSY"));?>" SIZE="4"></TD>
<TD></TD>
<TD></TD>
<TD></TD>
<TD></TD>
<TD></TD>
</TR>
<TR>
<TD valign="MIDDLE" COLSPAN="9" align="LEFT"><FONT STYLE="color:#D87801;font-family:Times New Roman,Times,Serif;font-size : 10pt">Icone fixe</FONT></TD>
</TR>
<TR>
<TD></TD>
<TD></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Hauteur</FONT></TD>
<TD valign="MIDDLE"  COLSPAN="1"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ICON_H"?>" VALUE="<?php if($XMLDevice) echo utf8_decode($XMLDevice->getAttribute("HEIGHT"));?>" SIZE="3"></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Largeur</FONT></TD>
<TD valign="MIDDLE"  COLSPAN="1"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ICON_L"?>" VALUE="<?php if($XMLDevice) echo utf8_decode($XMLDevice->getAttribute("WIDTH"));?>" SIZE="3"></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Fichier</FONT></TD>
<TD valign="MIDDLE"  COLSPAN="1"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ICON"?>" VALUE="<?php if($XMLDevice) echo utf8_decode($XMLDevice->getAttribute("ICON"));?>" SIZE="10"></TD>
<TD><IMG SRC="<?php if($XMLDevice) echo utf8_decode($XMLDevice->getAttribute("ICON")) ?>" WIDTH="<?php if($XMLDevice) echo utf8_decode($XMLDevice->getAttribute("WIDTH"));?>" HEIGHT="<?php if($XMLDevice) echo utf8_decode($XMLDevice->getAttribute("HEIGHT"));?>"></TD>
</TR>
<TR>
<TD valign="MIDDLE" COLSPAN="9" align="LEFT"><FONT STYLE="color:#D87801;font-family:Times New Roman,Times,Serif;font-size : 10pt">Icone dynamique</FONT></TD>
</TR>
<?php
	if($XMLDevice) {
		$XMLListEtats = $XMLDevice->getElementsByTagName('ETAT');
		foreach($XMLListEtats as $tmpXMLEtat) // Début liste des états
			{
?>
<TR>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Valeur</FONT></TD>
<TD valign="MIDDLE"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ETAT_VALUE[]"?>" VALUE="<?php if($tmpXMLEtat) echo utf8_decode($tmpXMLEtat->getAttribute("VALUE"));?>" SIZE="5"></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Hauteur</FONT></TD>
<TD valign="MIDDLE"  COLSPAN="1"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ETAT_H[]"?>" VALUE="<?php if($tmpXMLEtat) echo utf8_decode($tmpXMLEtat->getAttribute("HEIGHT"));?>" SIZE="3"></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Largeur</FONT></TD>
<TD valign="MIDDLE"  COLSPAN="1"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ETAT_L[]"?>" VALUE="<?php if($tmpXMLEtat) echo utf8_decode($tmpXMLEtat->getAttribute("WIDTH"));?>" SIZE="3"></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Fichier</FONT></TD>
<TD valign="MIDDLE"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ETAT_IMG[]"?>" VALUE="<?php if($tmpXMLEtat) echo utf8_decode($tmpXMLEtat->getAttribute("IMG"));?>" SIZE="10"></TD>
<TD><IMG SRC="<?php if($tmpXMLEtat) echo utf8_decode($tmpXMLEtat->getAttribute("IMG")) ?>" WIDTH="<?php if($tmpXMLEtat) echo utf8_decode($tmpXMLEtat->getAttribute("WIDTH"));?>" HEIGHT="<?php if($tmpXMLEtat) echo utf8_decode($tmpXMLEtat->getAttribute("HEIGHT"));?>"></TD>
</TR>
<?php 		} // Fin liste des états 
		} // Fin du test sur $XMLDevice ?>
<TR>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Valeur</FONT></TD>
<TD valign="MIDDLE"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ETAT_VALUE[]"?>" VALUE="" SIZE="5"></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Hauteur</FONT></TD>
<TD valign="MIDDLE"  COLSPAN="1"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ETAT_H[]"?>" VALUE="" SIZE="3"></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Largeur</FONT></TD>
<TD valign="MIDDLE"  COLSPAN="1"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ETAT_L[]"?>" VALUE="" SIZE="3"></TD>
<TD valign="MIDDLE"><FONT STYLE="color:#FFFFFF;font-family:Times New Roman,Times,Serif;font-size : 10pt">Fichier</FONT></TD>
<TD valign="MIDDLE"><INPUT TYPE="TEXT" NAME="<?php echo utf8_decode($v->_Id) . "_" . utf8_decode($x[$i]->_ID) . "_ETAT_IMG[]"?>" VALUE="" SIZE="10"></TD>
<TD><IMG SRC=""></TD>
</TR>
</TABLE>
<HR>
<?php
}
?>
</TD>
</TR>
<?php
}
?>
</TABLE>
<?php
	}
?>
<INPUT type="SUBMIT" value="Enregistrer">
</FORM>


