<?php
if($FicExist and $ConExist) {
	if(property_exists($ListeHomiZone,'Zone')) {
		if(!is_array($ListeHomiZone->Zone)) 
			$ListeHomiZone->Zone = array($ListeHomiZone->Zone);
		foreach($ListeHomiZone->Zone as $k=>$v) 
			{
						echo "<BR><A HREF=\"index.php?zoneid=" . $v->_Id . "\">\n";
						echo "<CENTER><FONT style=\"font-size:12px ;color: #FFFFFF\"\">" . utf8_decode($v->_Name) . "</CENTER><br>\n";
						echo "<CENTER><IMG SRC=\"images/zone-defaut.png\" HEIGHT=\"50\" WIDTH=\"50\" BORDER=\"0\"></CENTER></A><br>\n";
		  }
/*		else
		{
					echo "<BR><A HREF=\"index.php?zoneid=" . $ListeHomiZone->Zone->_Id . "\">\n";
					echo "<CENTER><FONT style=\"font-size:12px ;color: #FFFFFF\"\">" . utf8_decode($ListeHomiZone->Zone->_Name) . "</CENTER><br>\n";
					echo "<CENTER><IMG SRC=\"images/zone-defaut.png\" HEIGHT=\"50\" WIDTH=\"50\" BORDER=\"0\"></CENTER></A><br>\n";
	  } */
	}
}
?>
<CENTER>
<BR><A HREF="index.php?zoneid=Parametres">
<CENTER><FONT style="font-size:12px ;color: #FFFFFF">Paramètres</CENTER><br>
<CENTER><IMG SRC="images/param.png" WIDTH="25" HEIGHT="25" BORDER="0"><CENTER></A><BR>
</CENTER>
