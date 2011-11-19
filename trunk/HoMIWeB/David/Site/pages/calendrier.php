<?php
	if(isset($_SESSION['user_id'])){ 
?>
<div class="main">
 <table border="0" width="100%">
  <tbody>
    <tr>
      <td align="center" valign="middle">
        <table border="0" width="100%">
    
<?php
include("./include_php/config.php");

?>

<iframe src="http://www.google.com/calendar/embed?showTitle=0&amp;showTz=0&amp;height=600&amp;wkst=2&amp;hl=fr&amp;bgcolor=%23ffffff&amp;src=davidinfo05%40gmail.com&amp;color=%232952A3&amp;ctz=Europe%2FParis" style=" border-width:0 " width="930" height="600" frameborder="0" scrolling="no"></iframe>

        </table>
      </td>
    </tr>
  </tbody>
 </table>
</div>

<?php
	}else{
		header("location:../index.php");
	}
?>