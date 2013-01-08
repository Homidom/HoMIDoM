<?php
$dom = new DomDocument();
$dom->load('homidom-web-param.xml');
$ListeServer = $dom->getElementsByTagName('SERVER'); // on récupère les paramètres du serveur
$server = $ListeServer->item(0);
?>

<CENTER>
<H1>Paramètres</H1>
</CENTER></BR>
<CENTER>
<FORM ACTION="homidom-web-param.php">
<TABLE BORDER="1">
<TR>
<TD><H2>Serveur SOAP</H2></TD>
<TD><TABLE>
<TR><TD valign="BOTTOM"><H4>Adresse IP</H4></TD><TD valign="TOP"><INPUT TYPE="TEXT" NAME="SERVER_IP" VALUE="<?php echo $server->getAttribute("IP");?>"></TD></TR>
<TR><TD valign="BOTTOM"><H4>Numéro de port</H4></TD><TD valign="TOP"><INPUT TYPE="TEXT" NAME="SERVEUR_PORT" VALUE="<?php echo $server->getAttribute("PORT");?>"></TD></TR>
<TR><TD valign="BOTTOM"><H4>Identifiant</H4></TD><TD valign="TOP"><INPUT TYPE="TEXT" NAME="SERVEUR_ID" VALUE="<?php echo $server->getAttribute("ID");?>"></TD></TR>
</TABLE></TD>
</TR>
</TABLE>
</FORM>

<?php
/*$homidom = new HomidomSoap($server->getAttribute("IP"), $server->getAttribute("PORT"), $server->getAttribute("ID"),true);
echo $homidom->connect() . "</BR>";
if($homidom->GetTime())
	echo "OK à " . $homidom->GetTime() . "</BR>";
else 
	echo "KO";*/
?>
<?php
/*$ListeZone = $dom->getElementsByTagName('ZONE'); // on récupère les zones
foreach($ListeZone as $zone) {
	echo "Nom :" . (utf8_decode($zone->getAttribute("NAME"))) . "</BR>\n";
	echo "Image :" . (utf8_decode($zone->getAttribute("FILENAME"))) . "</BR>\n";
	echo "Icone :" . (utf8_decode($zone->getAttribute("ICON"))) . "</BR>\n";
	echo "</BR>\n";
	}*/
?>
<?php
/*
$x=$homidom->GetAllZones();
foreach($x->Zone as $k=>$v) {
	
	echo "Nom :" . (utf8_decode($v->_Name)) . "</BR>\n";
	echo "Id :" . (utf8_decode($v->_Id)) . "</BR>\n";
	echo "Image :" . (utf8_decode($v->_Image)) . "</BR>\n";
//			$ListeDevice = $zone->getElementsByTagName("HOMIDEVICE");
	echo "</BR>";
	}*/
?>
