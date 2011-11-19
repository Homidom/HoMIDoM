<?php
require_once('../include_php/config.php');


$action = $_POST['action'];
$updateRecordsArray = $_POST['recordsArray'];

if ($action == "updateRecordsListings"){
	$listingCounter = 1;
	foreach ($updateRecordsArray as $recordIDValue) {
		$query = "UPDATE menu SET menu_ordre = " . $listingCounter . " WHERE menu_id = " . $recordIDValue;
		mysql_query($query) or die('Error, insert query failed');
		$listingCounter = $listingCounter + 1;
	}
}
?>