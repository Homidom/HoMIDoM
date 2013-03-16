<?php

class HomidomClass {
	public $_port = "7999";
	public $_ip = "localhost";
	public $_idserver = "123456789";
	public $_trace = false;
	public $_homidomsoap;
	
	/*--------------------------
	------- CONSTUCTEUR --------
	--------------------------*/
	public function __construct($ip,$port,$idserver,$trace) {
		$this->_port=$port; 
		$this->_ip=$ip; 
		$this->_idserver=$idserver;
		$this->_trace=$trace;
		$this->_homidomsoap = new HomidomSoap($this->_ip, $this->_port, $this->_idserver,$this->_trace);
		$this->_homidomsoap->connect();
	}

	public function GetTime() {
		return $this->_homidomsoap->GetTime();
	}

	public function ExecuteDeviceCommand($deviceid, $actionnom, $actionparametre){
		
		//echo "methodes: <br />";
		//$x=$this->_homidomsoap->ListMethod($deviceid);
		//print_r($x);
		If ($actionparametre=="") {
			return $this->_homidomsoap->ExecuteDeviceCommand($deviceid,array('Nom'=>"$actionnom",'Parametres'=>""));
		} else {
			$Parametres=array('DeviceAction'=>array('Nom'=>"",'Type'=>"",'Value'=>"$actionparametre"));
			return $this->_homidomsoap->ExecuteDeviceCommandSimple($deviceid,array('Nom'=>"$actionnom",'Param1'=>"$actionparametre"));
			//$x=$this->_homidomsoap->ExecuteDeviceCommand($deviceid,array('Nom'=>"$actionnom",'Parametres'=>array($Parametres)));
			//$this->_homidomsoap->tracer();
			return $x;
		}
		
	}
	
	public function RefreshBddDevice(){
		$x=$this->_homidomsoap->GetAllDevices();
		for($i = 0; $i < count($x); $i++){
			//pour chaque device, on vérifie si déjà présent dans la BDD : modifie sinon ajoute
// 			$x[$i]->_Name
// 			$x[$i]->_Type
// 			$x[$i]->_Adresse1
// 			$x[$i]->_Adresse2
// 			$x[$i]->_Value
// 			$x[$i]->_Refresh
// 			$x[$i]->_Description
// 			$x[$i]->_DriverID
// 			$x[$i]->_Enable
// 			$x[$i]->_Correction
// 			$x[$i]->_DateCreated
// 			$x[$i]->_Formatage
// 			$x[$i]->_LastChange
// 			$x[$i]->_LastChangeDuree
// 			$x[$i]->_LastEtat
// 			$x[$i]->_Modele
// 			$x[$i]->_Picture
// 			$x[$i]->_Precision
// 			$x[$i]->_Solo
// 			$x[$i]->_ValueDef
// 			$x[$i]->_ValueLast
// 			$x[$i]->_ValueMax
// 			$x[$i]->_ValueMin
// 			$x[$i]->_ConditionActuel
// 			$x[$i]->_HumActuel
// 			$x[$i]->_IconActuel
// 			$x[$i]->_TempActuel
// 			$x[$i]->_VentActuel
			
			$sql = "SELECT * from devices where ssid='".$x[$i]->_ID."'";
			$res = mysql_query ($sql);
			if(mysql_num_rows($res)>0){
				//Trouvé : mise à jour
				$sql = "UPDATE devices SET name='".$x[$i]->_Name."',type='".$x[$i]->_Type."',adresse1='".$x[$i]->_Adresse1."',adresse2='".$x[$i]->_Adresse2."',value='".$x[$i]->_Value."' WHERE id=".$x[$i]->_ID;
				$res = mysql_query($sql);
			} else {
				//Non trouvé : Ajoute
				$sql = "INSERT INTO devices(ssid,name,type,adresse1,adresse2,value) VALUES ('".$x[$i]->_ID."','".$x[$i]->_Name."','".$x[$i]->_Type."','".$x[$i]->_Adresse1."','".$x[$i]->_Adresse2."','".$x[$i]->_Value."')";
				$res = mysql_query($sql);
			}
		}
		return "OK";
	}

}

?>