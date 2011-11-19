<?php
include("../include_php/config.php");
$composants_id=!empty($_REQUEST["composants_id"])?$_REQUEST["composants_id"]:"";

//$query = "select * from releve where releve_composants='$composants_id' and releve_dateheure>'".date('Y-m-d H:i:s',time() - (365 * 24 * 60 * 60))."' order by releve_dateheure";
$query = "select * from releve where releve_composants='$composants_id' order by releve_dateheure";
$res = mysql_query($query);
// echo data
while($obj = mysql_fetch_object($res)){  
  $date = $obj->releve_dateheure;
  $visits =  $obj->releve_valeur;
 
  echo "$date,$visits\n";
} 



?>