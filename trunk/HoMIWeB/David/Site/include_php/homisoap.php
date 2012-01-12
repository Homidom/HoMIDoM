<?php

class HomidomSoap {
	public $_port = "7999";
	public $_ip = "localhost";
	public $_idserver = "123456789";
	public $_trace = false;
	private $_client;
	private $_extension = false;
	private $_connecte = false;
	
	public function __construct($ip,$port,$idserver,$trace) {
		$this->_port=$port; //7999
		$this->_ip=$ip; //localhost ou 192.168.1.X
		$this->_idserver=$idserver;
		$this->_trace=$trace;
		if(!extension_loaded('soap')) {
			// Cette dernière n'est pas chargée, tentative de chargement
			if(!@dl('soap')) {
				// Le chargement n'est pas réalisée, exit
				die('Impossible d\'utiliser l\'extension SOAP');
			}
		}
		$this->_extension = true;
	}

	public function connect() {
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
	}

	public function disconnect() {
		// $_client = ;
		$this->_connecte = false;
	}
 
	private function tracer() {
		if ($this->_trace) {
			echo "<pre>\n";
			echo "Request :".htmlspecialchars($this->_client->__getLastRequest()) ."\n";
			echo "Response:".htmlspecialchars($this->_client->__getLastResponse())."\n";
			echo "</pre>";
		}
	}

	public function GetAllDevices(){
		if($this->_connecte) {
			$reponse= $this->_client->GetAllDevices(array('IdSrv'=>"$this->_idserver"));
			$this->tracer();
			$x=$reponse->GetAllDevicesResult->TemplateDevice;
			return $x;
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
}

?>