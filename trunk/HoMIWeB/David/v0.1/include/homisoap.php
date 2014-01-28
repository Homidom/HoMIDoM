<?php

class HomidomSoap {
    public $_port = "7999";
    public $_ip = "localhost";
    public $_idserver = "123456789";
    public $_trace = false;
    private $_client;
    private $_extension = false;
    private $_connecte = false;
    
	
	/*--------------------------
	------- CONSTUCTEUR --------
	--------------------------*/
    public function __construct($ip,$port,$idserver,$trace) {
        $this->_port=$port;
        $this->_ip=$ip; 
        $this->_idserver=$idserver;
        $this->_trace=$trace;
        if(!extension_loaded('soap')) {
            die('Impossible d\'utiliser l\'extension SOAP');
        }
        $this->_extension = true;
    }
	
	
	/*--------------------------
	-  CONNEXION / DECONNEXION -
	--------------------------*/
    public function connect() {
        if($this->_extension) {
            ini_set('soap.wsdl_cache_enabled', 0);
			ini_set('default_socket_timeout', 20);

            if ($this->_trace) {
                try {
					$this->_client = @new SoapClient('http://'.$this->_ip.':'.$this->_port.'/ServiceModelSamples/service?wsdl', array('trace' => 1));
					$this->_connecte = true;
					return $this->_connecte;
					} catch(SoapFault $fault)
					{
					$this->_connecte = false;
					return $this->_connecte;
					}
					catch(Exception $e)
					{
					$this->_connecte = false;
					return $this->_connecte;
					}
            } else {
                try {
					$this->_client = @new SoapClient('http://'.$this->_ip.':'.$this->_port.'/ServiceModelSamples/service?wsdl');
					$this->_connecte = true;
					return $this->_connecte;
					} catch(SoapFault $fault)
					{
					$this->_connecte = false;
					return $this->_connecte;
					}
					
            }
        } else {
            $this->_connecte = false;
            return $this->_connecte;
        }
    } 

	/************ ANCIENNE FONCTION : BACKUP = 06/02/2013  ****************/
/*	 public function connect() {
        if($this->_extension) {
            ini_set('soap.wsdl_cache_enabled', 0);
            if ($this->_trace) {
                $this->_client = new SoapClient('http://'.$this->_ip.':'.$this->_port.'/ServiceModelSamples/service?wsdl', array('trace' => 1));
            } else {
                $this->_client = new SoapClient('http://'.$this->_ip.':'.$this->_port.'/ServiceModelSamples/service?wsdl');
            }
            $this->_connecte = true;
        } else {
            $this->_connecte = false;
        }
    }  */

  
    public function disconnect() {
        // $_client = ;
        $this->_connecte = false;
    }
	
	
	/*--------------------------
	----------- TRACE ----------
	--------------------------*/
    public function tracer() {
        if ($this->_trace) {
            echo "<pre>\n";
            echo "Request :".htmlspecialchars($this->_client->__getLastRequest()) ."\n";
            echo "Response:".htmlspecialchars($this->_client->__getLastResponse())."\n";
            echo "</pre>";
        }
    }
	
	
	/*--------------------------
	----------- SERVEUR --------
	--------------------------*/
	public function GetTimeSave(){
        if($this->_connecte) {
            $reponse= $this->_client->GetTimeSave(array('IdSrv'=>"$this->_idserver"));
            return $reponse->GetTimeSaveResult;
        } else {
            return "";
        }
    }
	
	public function SetTimeSave($value){
        if($this->_connecte) {
            $reponse= $this->_client->SetTimeSave(array('IdSrv'=>"$this->_idserver", 'Value'=>"$value"));
            return $reponse->SetTimeSaveResult;
        } else {
            return "";
        }
    }
	
	public function GetIdServer(){
        if($this->_connecte) {
            $reponse= $this->_client->GetIdServer(array('IdSrv'=>"$this->_idserver"));
            return $reponse->GetIdServerResult;
        } else {
            return "";
        }
    }
	
	public function SetIdServer($newidserver){
        if($this->_connecte) {
            $reponse= $this->_client->SetIdServer(array('IdSrv'=>"$this->_idserver", 'Value'=>"$newidserver"));
            return $reponse->SetIdServerResult;
        } else {
            return "";
        }
    }
	
	public function GetServerVersion(){
        if($this->_connecte) {
            $reponse= $this->_client->GetServerVersion();
            return $reponse->GetServerVersionResult;
        } else {
            return "";
        }
    }
	
	public function GetTime(){
        if($this->_connecte) {
            $reponse= $this->_client->GetTime();
            return $reponse->GetTimeResult;
        } else {
            return "";
        }
    }
	
	public function MessageToServer($message){
		if($this->_connecte) {
            $reponse= $this->_client->MessageToServer(array('Message'=>"$message"));
            return $reponse;
//            return 0;
        } else {
            return "";
        }
	}
	
	public function SaveConfig(){
		if($this->_connecte) {
            $reponse= $this->_client->SaveConfig(array('IdSrv'=>"$this->_idserver"));
            return 0;
        } else {
            return "";
        }
	}
	
	public function Start(){
		if($this->_connecte) {
            $reponse= $this->_client->Start();
            return 0;
        } else {
            return "";
        }
	}
	
	public function Stop(){
		if($this->_connecte) {
            $reponse= $this->_client->Stop(array('IdSrv'=>"$this->_idserver"));
            return 0;
        } else {
            return "";
        }
	}
	
	public function SetPortSOAP($portsoap){
		if($this->_connecte) {
            $reponse= $this->_client->SetPortSOAP(array('IdSrv'=>"$this->_idserver", 'Value'=>"$portsoap"));
            return 0;
        } else {
            return "";
        }
	}
	
	public function GetPortSOAP(){
		if($this->_connecte) {
            $reponse= $this->_client->GetPortSOAP();
            return $reponse->GetPortSOAPResult;
        } else {
            return "";
        }
	}
	
	public function GetLastStartTime(){
        if($this->_connecte) {
            $reponse= $this->_client->GetLastStartTime();
            return $reponse->GetLastStartTimeResult;
        } else {
            return "";
        }
    }
	
	public function GetHeureLeverSoleil(){
        if($this->_connecte) {
            $reponse= $this->_client->GetHeureLeverSoleil();
            return $reponse->GetHeureLeverSoleilResult;
        } else {
            return "";
        }
    }
	
	public function GetHeureCoucherSoleil(){
        if($this->_connecte) {
            $reponse= $this->_client->GetHeureCoucherSoleil();
            return $reponse->GetHeureCoucherSoleilResult;
        } else {
            return "";
        }
    }
	
	public function GetLongitude(){
        if($this->_connecte) {
            $reponse= $this->_client->GetLongitude();
            return $reponse->GetLongitudeResult;
        } else {
            return "";
        }
    }
	
	public function SetLongitude($longitude){
        if($this->_connecte) {
            $reponse= $this->_client->SetLongitude(array('IdSrv'=>"$this->_idserver", 'Value'=>"$longitude"));
            return 0;
        } else {
            return "";
        }
    }
	
	public function GetLatitude(){
        if($this->_connecte) {
            $reponse= $this->_client->GetLatitude();
            return $reponse->GetLatitudeResult;
        } else {
            return "";
        }
    }
	
	public function SetLatitude($latitude){
        if($this->_connecte) {
            $reponse= $this->_client->SetLatitude(array('IdSrv'=>"$this->_idserver", 'Value'=>"$latitude"));
            return 0;
        } else {
            return "";
        }
    }
	
	public function GetHeureCorrectionCoucher(){
        if($this->_connecte) {
            $reponse= $this->_client->GetHeureCorrectionCoucher();
            return $reponse->GetHeureCorrectionCoucherResult;
        } else {
            return "";
        }
    }
	
	public function SetHeureCorrectionCoucher($correctiontime){
        if($this->_connecte) {
            $reponse= $this->_client->SetHeureCorrectionCoucher(array('IdSrv'=>"$this->_idserver", 'Value'=>"$correctiontime"));
			return 0;
        } else {
			return "";
        }
    }
	
	public function GetHeureCorrectionLever(){
        if($this->_connecte) {
            $reponse= $this->_client->GetHeureCorrectionLever();
            return $reponse->GetHeureCorrectionLeverResult;
        } else {
            return "";
        }
    }
	
	public function SetHeureCorrectionLever($correctiontime){
        if($this->_connecte) {
            $reponse= $this->_client->SetHeureCorrectionLever(array('IdSrv'=>"$this->_idserver", 'Value'=>"$correctiontime"));
			return 0;
        } else {
			return "";
        }
    }
	
	public function GetByteFromImage($file){
        if($this->_connecte) {
            $reponse= $this->_client->GetByteFromImage(array('file'=>"$file"));
			return $reponse->GetByteFromImageResult;
        } else {
			return "";
        }
    }
	
	public function GetListOfImage(){
        if($this->_connecte) {
            $reponse= $this->_client->GetListOfImage();
			return $reponse->GetListOfImageResult->ImageFile;
        } else {
			return "";
        }
    }
	
	
	/*--------------------------
	------- HISTORISATION ------
	--------------------------*/
	public function GetAllListHisto(){
        if($this->_connecte) {
            $reponse= $this->_client->GetAllListHisto(array('IdSrv'=>"$this->_idserver"));
			return $reponse->GetAllListHistoResult->Historisation;
        } else {
			return "";
        }
    }
	
	public function GetHisto($source, $id){
        if($this->_connecte) {
            $reponse= $this->_client->GetHisto(array('IdSrv'=>"$this->_idserver", 'Source'=>"$source", 'idDevice'=>"$id"));
			return $reponse->GetHistoResult;
        } else {
			return "";
        }
    }
	
	
	/*--------------------------
	----------- AUDIO ----------
	--------------------------*/
	public function DeleteExtensionAudio($extension){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteExtensionAudio(array('IdSrv'=>"$this->_idserver", 'NomExtension'=>"$extension"));
			return $reponse->DeleteExtensionAudioResult;
        } else {
			return "";
        }
    }
	
	public function NewExtensionAudio($extension, $enable){
        if($this->_connecte) {
            $reponse= $this->_client->NewExtensionAudio(array('IdSrv'=>"$this->_idserver", 'NomExtension'=>"$extension", 'Enable'=>"$enable"));
			return $reponse->NewExtensionAudioResult;
        } else {
			return "";
        }
    }
	
	public function EnableExtensionAudio($extension, $enable){
        if($this->_connecte) {
            $reponse= $this->_client->EnableExtensionAudio(array('IdSrv'=>"$this->_idserver", 'NomExtension'=>"$extension", 'Enable'=>"$enable"));
			return $reponse->EnableExtensionAudioResult;
        } else {
			return "";
        }
    }
	
	public function DeleteRepertoireAudio($repertoire){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteRepertoireAudio(array('IdSrv'=>"$this->_idserver", 'NomRepertoire'=>"$repertoire"));
			return $reponse->DeleteRepertoireAudioResult;
        } else {
			return "";
        }
    }
	
	public function NewRepertoireAudio($repertoire, $enable){
        if($this->_connecte) {
            $reponse= $this->_client->NewRepertoireAudio(array('IdSrv'=>"$this->_idserver", 'NomRepertoire'=>"$repertoire", 'Enable'=>"$enable"));
			return $reponse->NewRepertoireAudioResult;
        } else {
			return "";
        }
    }
	
	public function EnableRepertoireAudio($repertoire, $enable){
        if($this->_connecte) {
            $reponse= $this->_client->EnableRepertoireAudio(array('IdSrv'=>"$this->_idserver", 'NomRepertoire'=>"$repertoire", 'Enable'=>"$enable"));
			return $reponse->EnableRepertoireAudioResult;
        } else {
			return "";
        }
    }
	
	public function GetAllRepertoiresAudio(){
        if($this->_connecte) {
            $reponse= $this->_client->GetAllRepertoiresAudio(array('IdSrv'=>"$this->_idserver"));
			return $reponse->GetAllRepertoiresAudioResult;
        } else {
			return "";
        }
    }
	
	public function GetAllExtensionsAudio(){
        if($this->_connecte) {
            $reponse= $this->_client->GetAllExtensionsAudio(array('IdSrv'=>"$this->_idserver"));
			return $reponse->GetAllExtensionsAudioResult;
        } else {
			return "";
        }
    }
	
	
	/*--------------------------
	----------- USER -----------
	--------------------------*/
	public function DeleteUser($iduser){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteUser(array('IdSrv'=>"$this->_idserver", 'userId'=>"$iduser"));
			return $reponse->DeleteUserResult;
        } else {
			return "";
        }
    }
	
	public function VerifLogin($username, $password){
        if($this->_connecte) {
            $reponse= $this->_client->VerifLogin(array('Username'=>"$username", 'Password'=>"$password"));
			return $reponse->VerifLoginResult;
        } else {
			return "";
        }
    }
	
	public function ChangePassword($username, $oldpassword, $confirmnewpassword, $password){
        if($this->_connecte) {
            $reponse= $this->_client->ChangePassword(array('IdSrv'=>"$this->_idserver", 'Username'=>"$username", 'OldPassword'=>"$oldpassword", 'ConfirmNewPassword'=>"$confirmnewpassword", 'Password'=>"$password"));
			return $reponse->ChangePasswordResult;
        } else {
			return "";
        }
    }
	
	public function ReturnUserByUsername($username){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnUserByUsername(array('IdSrv'=>"$this->_idserver", 'Username'=>"$username"));
			return $reponse->ReturnUserByUsernameResult;
        } else {
			return "";
        }
    }
	
	public function GetAllUsers(){
        if($this->_connecte) {
            $reponse= $this->_client->GetAllUsers(array('IdSrv'=>"$this->_idserver"));
			return $reponse->GetAllUsersResult;
        } else {
			return "";
        }
    }
	
	public function ReturnUserById($userid){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnUserById(array('IdSrv'=>"$this->_idserver", 'UserId'=>"$userid"));
			return $reponse->ReturnUserByIdResult;
        } else {
			return "";
        }
    }
	
	public function SaveUser($userid = "", $username, $password, $profil, $nom, $prenom, $numberidentification = "", $image = "", $email = "", $emailautre = "", $telfix = "", $telautre = "", $adresse = "", $ville = "", $codepostal = ""){
        if($this->_connecte) {
            $reponse= $this->_client->SaveUser(array('IdSrv'=>"$this->_idserver", 'userId'=>"$userid", 'UserName'=>"$username", 'Password'=>"$password", 'Profil'=>"$profil", 'Nom'=>"$nom", 'Prenom'=>"$prenom", 'NumberIdentification'=>"$numberidentification", 'Image'=>"$image", 'eMail'=>"$email", 'eMailAutre'=>"$emailautre", 'TelFixe'=>"$telfix", 'TelAutre'=>"$telautre", 'Adresse'=>"$adresse", 'Ville'=>"$ville", 'CodePostal'=>"$codepostal"));
			return $reponse->SaveUserResult;
        } else {
			return "";
        }
    }
	
	
	/*--------------------------
	---------- DEVICE ----------
	--------------------------*/
	public function GetAllDevices(){
        if($this->_connecte) {
            $reponse= $this->_client->GetAllDevices(array('IdSrv'=>"$this->_idserver"));
            /*$this->tracer();*/
            return $reponse->GetAllDevicesResult->TemplateDevice;  
        } else {
            return "";
        }
    }
	
	public function ExecuteDeviceCommand($deviceid, $action){
        if($this->_connecte) {
            $reponse= $this->_client->ExecuteDeviceCommand(array('IdSrv'=>"$this->_idserver", 'DeviceId'=>"$deviceid", 'Action'=>$action));
            /*return $reponse->ExecuteDeviceCommandResult;  */
            return "OK";
        } else {
            return "";
        }
    }
	
    public function ExecuteDeviceCommandSimple($deviceid, $action){
        if($this->_connecte) {
            $reponse= $this->_client->ExecuteDeviceCommand(array('IdSrv'=>"$this->_idserver", 'DeviceId'=>"$deviceid", 'Action'=>$action));
            /*return $reponse->ExecuteDeviceCommandResult;  */
            return "OK";
        } else {
            return "";
        }
    }
	
	public function DeleteDevice($deviceid){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteDevice(array('IdSrv'=>"$this->_idserver", 'deviceId'=>"$deviceid"));
            return $reponse->DeleteDeviceResult;  
        } else {
            return "";
        }
    }
	
	public function DeleteDeviceCommandIR($deviceid, $cmdname){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteDeviceCommandIR(array('IdSrv'=>"$this->_idserver", 'deviceId'=>"$deviceid", 'CmdName'=>"$cmdname"));
            return $reponse->DeleteDeviceCommandIRResult;  
        } else {
            return "";
        }
    }
	
	public function ReturnDeviceByID($id){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnDeviceByID(array('IdSrv'=>"$this->_idserver", 'Id'=>"$id"));
            return $reponse->ReturnDeviceByIDResult;  
        } else {
            return "";
        }
    }
	
	public function ReturnDeviceByAdresse1TypeDriver($deviceadresse, $devicetype, $devicedriver, $enable){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnDeviceByAdresse1TypeDriver(array('IdSrv'=>"$this->_idserver", 'DeviceAdresse'=>"$deviceadresse", 'DeviceType'=>"$devicetype", 'DeviceDriver'=>"$devicedriver", 'Enable'=>"$enable"));
            return $reponse->ReturnDeviceByAdresse1TypeDriverResult;  
        } else {
            return "";
        }
    }
	
	public function SaveDevice($deviceid, $name, $address1, $enable, $solo, $driverid, $type, $refresh, $address2, $image, $modele, $description, $lastchangeduree, $lastetat, $correction, $formatage, $precision, $valuemax, $valuemin, $valuedef){
        if($this->_connecte) {
            $reponse= $this->_client->SaveDevice(array('IdSrv'=>"$this->_idserver", 'deviceId'=>"$deviceid", 'name'=>"$name", 'address1'=>"$address1", 'enable'=>"$enable", 'solo'=>"$solo", 'driverid'=>"$driverid", 'type'=>"$type", 'refresh'=>"$refresh", 'address2'=>"$address2", 'image'=>"$image", 'modele'=>"$modele", 'description'=>"$description", 'lastchangeduree'=>"$lastchangeduree", 'lastEtat'=>"$lastetat", 'correction'=>"$correction", 'formatage'=>"$formatage", 'precision'=>"$precision", 'valuemax'=>"$valuemax", 'valuemin'=>"$valuemin", 'valuedef'=>"$valuedef"));
            return $reponse->SaveDeviceResult;  
        } else {
            return "";
        }
    }
	
	public function SaveDeviceCommandIR($deviceid, $cmdname, $cmddata, $cmdrepeat){
        if($this->_connecte) {
            $reponse= $this->_client->SaveDeviceCommandIR(array('IdSrv'=>"$this->_idserver", 'deviceId'=>"$deviceid", 'CmdName'=>"$cmdname", 'CmdData'=>"$cmddata", 'CmdRepeat'=>"$cmdrepeat", 'solo'=>"$solo", 'driverid'=>"$driverid", 'type'=>"$type", 'refresh'=>"$refresh", 'address2'=>"$address2", 'image'=>"$image", 'modele'=>"$modele", 'description'=>"$description", 'lastchangeduree'=>"$lastchangeduree", 'lastEtat'=>"$lastetat", 'correction'=>"$correction", 'formatage'=>"$formatage", 'precision'=>"$precision", 'valuemax'=>"$valuemax", 'valuemin'=>"$valuemin", 'valuedef'=>"$valuedef"));
            return $reponse->SaveDeviceCommandIRResult;  
        } else {
            return "";
        }
    }
	
	public function StartIrLearning(){
        if($this->_connecte) {
            $reponse= $this->_client->StartIrLearning(array('IdSrv'=>"$this->_idserver"));
            return $reponse->StartIrLearningResult;  
        } else {
            return "";
        }
    }
	
	
	/*--------------------------
	---------- DRIVER ----------
	--------------------------*/
	public function GetAllDrivers(){
        if($this->_connecte) {
            $reponse= $this->_client->GetAllDrivers(array('IdSrv'=>"$this->_idserver"));
            return $reponse->GetAllDriversResult;  
        } else {
            return "";
        }
    }
	
	public function ExecuteDriverCommand($driverid, $action){
        if($this->_connecte) {
            $reponse= $this->_client->ExecuteDriverCommand(array('IdSrv'=>"$this->_idserver", 'DriverId'=>"$driverid", 'Action'=>"$action"));
            return $reponse->ExecuteDriverCommandResult;  
        } else {
            return "";
        }
    }
	
	public function ReturnDriverByID($id){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnDriverByID(array('IdSrv'=>"$this->_idserver", 'Id'=>"$id"));
            return $reponse->ReturnDriverByIDResult;  
        } else {
            return "";
        }
    }
	
	public function DeleteDriver($driverid){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteDriver(array('IdSrv'=>"$this->_idserver", 'driverId'=>"$driverid"));
            return $reponse->DeleteDriverResult;  
        } else {
            return "";
        }
    }
	
	public function ReturnDriverByNom($name){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnDriverByNom(array('IdSrv'=>"$this->_idserver", 'name'=>"$name"));
            return $reponse->ReturnDriverByNomResult;  
        } else {
            return "";
        }
    }
	
	public function SaveDriver($driveid, $name, $enable, $startauto, $iptcp, $porttcp, $ipudp, $portudp, $com, $refresh, $picture, $modele, $parametres){
        if($this->_connecte) {
            $reponse= $this->_client->SaveDriver(array('IdSrv'=>"$this->_idserver", 'driverId'=>"$driveid", 'name'=>"$name", 'enable'=>"$enable", 'startauto'=>"$startauto", 'iptcp'=>"$iptcp", 'porttcp'=>"$porttcp", 'ipudp'=>"$ipudp", 'portudp'=>"$portudp", 'com'=>"$com", 'refresh'=>"$refresh", 'picture'=>"$picture", 'modele'=>"$modele", 'Parametres'=>"$parametres"));
            return $reponse->SaveDriverResult;  
        } else {
            return "";
        }
    }
	
	public function StopDriver($driverid){
        if($this->_connecte) {
            $reponse= $this->_client->StopDriver(array('IdSrv'=>"$this->_idserver", 'DriverId'=>"$driverid"));
            return $reponse->StopDriverResult;  
        } else {
            return "";
        }
    }
	
	public function StartDriver($driverid){
        if($this->_connecte) {
            $reponse= $this->_client->StartDriver(array('IdSrv'=>"$this->_idserver", 'DriverId'=>"$driverid"));
            return $reponse->StartDriverResult;  
        } else {
            return "";
        }
    }
	
	
	/*--------------------------
	----------- ZONE -----------
	--------------------------*/
	public function ZoneIsEmpty($zoneid){
        if($this->_connecte) {
            $reponse= $this->_client->ZoneIsEmpty(array('IdSrv'=>"$this->_idserver", 'zoneId'=>"$zoneid"));
            return $reponse->ZoneIsEmptyResult;  
        } else {
            return "";
        }
    }
	
	public function DeleteZone($zoneid){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteZone(array('IdSrv'=>"$this->_idserver", 'zoneId'=>"$zoneid"));
            return $reponse->DeleteZoneResult;  
        } else {
            return "";
        }
    }
	
	
	public function objectToArray($object)
	{
			 if(!is_object($object) && !is_array($object))
				  return $object;

			 $array=array();
			 foreach($object as $member=>$data)
			 {
				  $array[$member]=$this->objectToArray($data);
			}
			return $array;
	}
	
	
	public function GetAllZones(){
		if($this->_connecte) {
			$reponse= $this->_client->GetAllZones(array('IdSrv'=>"$this->_idserver"));
			return $reponse->GetAllZonesResult;  
		} else {
			return "";
		}
	}
	public function GetDeviceInZone($zoneid){
        if($this->_connecte) {
            $reponse= $this->_client->GetDeviceInZone(array('IdSrv'=>"$this->_idserver", 'zoneId'=>"$zoneid"));
            return $reponse->GetDeviceInZoneResult;  
        } else {
            return "";
        }
    }
	
	
	public function ReturnZoneByID($id){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnZoneByID(array('IdSrv'=>"$this->_idserver", 'Id'=>"$id"));
            return $reponse->ReturnZoneByIDResult;  
        } else {
            return "";
        }
    }
	
	
	public function SaveZone($id, $zoneid, $name, $listelement, $icon, $image){
        if($this->_connecte) {
            $reponse= $this->_client->SaveZone(array('IdSrv'=>"$this->_idserver", 'zoneId'=>"$zoneid", 'name'=>"$name", 'ListElement'=>"$listelement", 'icon'=>"$icon", 'image'=>"$image"));
            return $reponse->SaveZoneResult;  
        } else {
            return "";
        }
    }
	
	public function AddDeviceToZone($zoneid){
        if($this->_connecte) {
            $reponse= $this->_client->AddDeviceToZone(array('IdSrv'=>"$this->_idserver", 'ZoneId'=>"$zoneid"));
            return $reponse->AddDeviceToZoneResult;  
        } else {
            return "";
        }
    }
	
	public function DeleteDeviceToZone($zoneid, $deviceid){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteDeviceToZone(array('IdSrv'=>"$this->_idserver", 'ZoneId'=>"$zoneid", 'DeviceId'=>"$deviceid"));
            return $reponse->DeleteDeviceToZoneResult;  
        } else {
            return "";
        }
    }
	
	
	/*--------------------------
	----------- MACRO -----------
	--------------------------*/
	public function DeleteMacro($macroId){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteMacro(array('IdSrv'=>"$this->_idserver", 'macroId'=>"$macroid"));
            return $reponse->DeleteMacroResult;  
        } else {
            return "";
        }
    }
	
	public function RunMacro($Id){
        if($this->_connecte) {
            $reponse= $this->_client->RunMacro(array('IdSrv'=>"$this->_idserver", 'Id'=>"$id"));
            return $reponse->RunMacroResult;  
        } else {
            return "";
        }
    }
	
	public function GetAllMacros(){
        if($this->_connecte) {
            $reponse= $this->_client->GetAllMacros(array('IdSrv'=>"$this->_idserver"));
            return $reponse->GetAllMacrosResult;  
        } else {
            return "";
        }
    }
	
	public function ReturnMacroById($macroid){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnMacroById(array('IdSrv'=>"$this->_idserver", 'MacroId'=>"$macroid"));
            return $reponse->ReturnMacroByIdResult;  
        } else {
            return "";
        }
    }
	
	public function SaveMacro($macroid){
        if($this->_connecte) {
            $reponse= $this->_client->SaveMacro(array('IdSrv'=>"$this->_idserver", 'macroId'=>"$macroId", 'nom'=>"$nom", 'enable'=>"$enable", 'description'=>"$description", 'listactions'=>"$listactions"));
            return $reponse->SaveMacroResult;  
        } else {
            return "";
        }
    }
	
	
	/*--------------------------
	--------- TRIGGER ----------
	--------------------------*/
	public function GetAllTriggers(){
        if($this->_connecte) {
            $reponse= $this->_client->GetAllTriggers(array('IdSrv'=>"$this->_idserver"));
            return $reponse->GetAllTriggersResult;  
        } else {
            return "";
        }
    }
	
	public function ReturnTriggerById($triggerid){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnTriggerById(array('IdSrv'=>"$this->_idserver", 'TriggerId'=>"$triggerid"));
            return $reponse->ReturnTriggerByIdResult;  
        } else {
            return "";
        }
    }
	
	public function DeleteTrigger($triggerid){
        if($this->_connecte) {
            $reponse= $this->_client->DeleteTrigger(array('IdSrv'=>"$this->_idserver", 'triggerId'=>"$triggerid"));
            return $reponse->DeleteTriggerResult;  
        } else {
            return "";
        }
    }
	
	public function SaveTrigger($triggerid, $nom, $enable, $typetrigger, $description, $conditiontimer, $deviceid, $deviceproperty, $macro){
        if($this->_connecte) {
            $reponse= $this->_client->SaveTrigger(array('IdSrv'=>"$this->_idserver", 'triggerId'=>"$triggerid", 'nom'=>"$nom", 'enable'=>"$enable", 'TypeTrigger'=>"$typetrigger", 'description'=>"$description", 'conditiontimer'=>"$conditiontimer", 'deviceid'=>"$deviceid", 'deviceproperty'=>"$deviceproperty", 'macro'=>"$macro"));
            return $reponse->SaveTriggerResult;  
        } else {
            return "";
        }
    }
	
	
	/*--------------------------
	---------- DIVERS ----------
	--------------------------*/	
	public function ListMethod($deviceid){
        if($this->_connecte) {
            $reponse= $this->_client->ListMethod(array('DeviceId'=>"$deviceid"));
            return $reponse->ListMethodResult;  
        } else {
            return "";
        }
    }
	
	
	/*--------------------------
	------------ LOG -----------
	--------------------------*/	
	public function GetTypeLogEnable(){
        if($this->_connecte) {
            $reponse= $this->_client->GetTypeLogEnable();
            return $reponse->GetTypeLogEnableResult;  
        } else {
            return "";
        }
    }
	
	public function SetTypeLogEnable($listtypelogenable){
        if($this->_connecte) {
            $reponse= $this->_client->SetTypeLogEnable(array('ListTypeLogEnable'=>"$listtypelogenable"));
            return $reponse->SetTypeLogEnableResult;  
        } else {
            return "";
        }
    }
	
	public function SetMaxMonthLog($month){
        if($this->_connecte) {
            $reponse= $this->_client->SetMaxMonthLog(array('Month'=>"$month"));
            return $reponse->SetMaxMonthLogResult;  
        } else {
            return "";
        }
    }
	
	public function GetMaxMonthLog(){
        if($this->_connecte) {
            $reponse= $this->_client->GetMaxMonthLog();
            return $reponse->GetMaxMonthLogResult;  
        } else {
            return "";
        }
    }
	
	public function Log($typlog, $source, $fonction, $message){
        if($this->_connecte) {
            $reponse= $this->_client->Log(array('TypLog'=>"$typlog", 'Source'=>"$source", 'Fonction'=>"$fonction", 'Message'=>"$message"));
            return $reponse->LogResult;  
        } else {
            return "";
        }
    }
	
	public function ReturnLog($requete){
        if($this->_connecte) {
            $reponse= $this->_client->ReturnLog(array('Requete'=>"$requete"));
            return $reponse->ReturnLogResult;  
        } else {
            return "";
        }
    }
	
	public function SetMaxFileSizeLog($value){
        if($this->_connecte) {
            $reponse= $this->_client->SetMaxFileSizeLog(array('Value'=>"$value"));
            return $reponse->SetMaxFileSizeLogResult;  
        } else {
            return "";
        }
    }
	
	public function GetMaxFileSizeLog(){
        if($this->_connecte) {
            $reponse= $this->_client->GetMaxFileSizeLog();
            return $reponse->GetMaxFileSizeLogResult;  
        } else {
            return "";
        }
    }
	
	
	/*--------------------------
	------------ SMTP ----------
	--------------------------*/
	
	public function GetSMTPServeur(){
        if($this->_connecte) {
            $reponse= $this->_client->GetSMTPServeur(array('IdSrv'=>"$this->_idserver"));
            return $reponse->GetSMTPServeurResult;  
        } else {
            return "";
        }
    }
	
	public function SetSMTPServeur($value){
        if($this->_connecte) {
            $reponse= $this->_client->SetSMTPServeur(array('IdSrv'=>"$this->_idserver", 'Value'=>"$value"));
            return $reponse->SetSMTPServeurResult;  
        } else {
            return "";
        }
    }
	
	public function GetSMTPLogin(){
        if($this->_connecte) {
            $reponse= $this->_client->GetSMTPLogin(array('IdSrv'=>"$this->_idserver"));
            return $reponse->GetSMTPLoginResult;  
        } else {
            return "";
        }
    }
	
	public function SetSMTPLogin($value){
        if($this->_connecte) {
            $reponse= $this->_client->SetSMTPLogin(array('IdSrv'=>"$this->_idserver", 'Value'=>"$value"));
            return $reponse->SetSMTPLoginResult;  
        } else {
            return "";
        }
    }
	
	public function GetSMTPPassword(){
        if($this->_connecte) {
            $reponse= $this->_client->GetSMTPPassword(array('IdSrv'=>"$this->_idserver"));
            return $reponse->GetSMTPPasswordResult;  
        } else {
            return "";
        }
    }
	
	public function SetSMTPPassword($value){
        if($this->_connecte) {
            $reponse= $this->_client->SetSMTPPassword(array('IdSrv'=>"$this->_idserver", 'Value'=>"$value"));
            return $reponse->SetSMTPPasswordResult;  
        } else {
            return "";
        }
    }
	
	public function GetSMTPMailServeur(){
        if($this->_connecte) {
            $reponse= $this->_client->GetSMTPMailServeur(array('IdSrv'=>"$this->_idserver"));
            return $reponse->GetSMTPMailServeurResult;  
        } else {
            return "";
        }
    }
	
	public function SetSMTPMailServeur($value){
        if($this->_connecte) {
            $reponse= $this->_client->SetSMTPMailServeur(array('IdSrv'=>"$this->_idserver", 'Value'=>"$value"));
            return $reponse->SetSMTPMailServeurResult;  
        } else {
            return "";
        }
    }	
}