<?php
	if(isset($_SESSION['user_id'])){ 
?>
<div class="main">
 <table border="0" width="100%">
  <tbody>
    <tr>
      <td align="center" valign="middle">
        <table width="92%" class="adminform">
          <tbody>
            <tr>
              <td width="100%" align="left" valign="top">
				<table border="0" cellspacing="0" cellpadding="0" width="100%">
    
<?php
include("./include_php/config.php");
$msg=isset($_GET["msg"])?$_GET["msg"]:(isset($_POST["msg"])?$_POST["msg"]:"ERREUR !");

echo "<tr><td align='center'><div class='plan' id='plan'><div class='image'>";

If ($msg="erreur404") {warning("Erreur 404<br /><br /> Page non trouvé !");}
else {warning($msg);}

echo "</div></div></td></tr>";
?>
				</table>
			  </td>
            </tr>
          </tbody>
        </table>
      </td>
    </tr>
  </tbody>
 </table>
</div>
<?php
	}else{
		header("location:../index.html");
	}
?>