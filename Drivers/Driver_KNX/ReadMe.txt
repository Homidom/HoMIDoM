Driver KNX/EIBD pour HoMIDom
----------------------------
Permet d'interragir avec un BUS KNX à travers une passerelle IP/KNX (EIBD)

Basé sur .NET KNXFramework : http://knxframework.codeplex.com/ (Gianni Tagliarino)


Types de Devices pris en charge:
-----------------------------------
* ListeDevices.SWITCH

Types de Commandes pris en charge:
-------------------------------------
* ON
* OFF

Configuration des Objets KNX
----------------------------

	Les objets KNX doivent être configurés dans le fichier ~\Drivers\KNX\knxobjects.xml
	Il s'agit des objets GA (GroupAddress)

	<?xml version="1.0" encoding="utf-8" ?>
	<KNXObjects>
	  <KNXObject id="ECL_CHAMBRE1_ECLAIRAGE_IE" gad="1/1/11" name="Eclairage Chambre 1 - Plafond IE" type="1.001" />
	  <KNXObject id="ECL_CHAMBRE1_ECLAIRAGE_TOR" gad="1/1/10" name="Eclairage Chambre 1 - Plafond ON/OFF" type="1.001" flags="cwrtu"/>
	</KNXObjects>

	* Id : Identifiant de l'objet (texte libre, doit être unique)
	* gad : Adresse du groupe KNX
	* name : Nom de l'objet (text libre, non utilisé)
	* type : type de donnée du groupe. (KNX datapoint types)
		* 1.001: switching (on/off) (EIS1)
		* 3.007: dimming (control of dimmer using up/down/stop) (EIS2)
		* 3.008: blinds (control of blinds using close/open/stop)
		* 10.001: time (EIS3)
		* 11.001: date (EIS4)
	* flags : flags 
		* c : Communication (allow the object to interact with the KNX bus)
		* w : Write (update the object's internal value with the one received in write telegram if they are different)
		* r : Read (allow the object to answer to a read request from another participant)
		* t : Transmit (allow the object to transmit it's value on the bus if it's modified internally by a rule or via XML protocol)
		* u : Update (update the object's internal value with the one received in "read response" telegram if they are different)
		* f : Force (force the object value to be transmitted on the bus, even if it didn't change).


	sources: 
	* LinKNX wiki : http://sourceforge.net/apps/mediawiki/linknx/index.php?title=Object_Definition_section

Configuration des objets HMD (Homidom)
--------------------------------------

	Les objets KNX doivent être configurés dans le fichier ~\Drivers\KNX\config.xml
	ils servent regrouper les objets KNX.

	<?xml version="1.0" encoding="utf-8" ?>
	<HMDObjects>
		<HMDObject id="ECL_CHAMBRE1" name="Eclairage Chambre 1 - Plafond" type="switch"> 
			<read id="ECL_CHAMBRE1_ECLAIRAGE_IE" />
			<write id="ECL_CHAMBRE1_ECLAIRAGE_TOR" />
		</HMDObject>
	</HMDObjects>

	* id : Identifiant uniquemt de l'objet => Correspond à l'adresse1 de du composant HoMIDom
	* name : Nom de l'objet (texte libre)
	* type : Type de le l'objet :
		* switch
	* read : liste des objets KNX utilisés pour lire l'état 
	* write : liste des objets KNX utilisé pour définir l'état

Comment déterminer les objets GA KNX à utiliser pour un composant HoMIDoM ?
---------------------------------------------------------------------------

* Installez, configurez et démarrez le driver KNX
* Activee le moniteur de bus KNX
* Pour un switch (ex. lumiere ON/OFF)
	* Allumer et éteindre la lumière concerné avec la bouton KNX (physique)
	* Consultez le journal du service HoMIDoM, vous devrier voir plusieurs lignes de DEBUG ressamblant à ça : 

		KNX/EIBD:BusMonitorEvent	LPDU: BC 10 5A 0F 0A E1 00 81 6C :L_Data low from 1.0.90 to 1/7/10 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 01 
		=> le bouton "1.0.90" ecrit la valeur "1" dans l'adresse de groupe "1/7/10" => donne l'ordre d'allumer la lumière
		KNX/EIBD:BusMonitorEvent	LPDU: BC 10 01 0F 0B E1 00 81 36 :L_Data low from 1.0.1 to 1/7/11 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 01
		=> l'actionneur "1.0.1" ecrit la valeur "1" dans le l'adresse de groupe "1/7/11" => inique au autres membres du groupe que sa valeur est "1" (indicateur d'etat)
		KNX/EIBD:BusMonitorEvent	LPDU: BC 10 5A 0F 0A E1 00 80 6D :L_Data low from 1.0.90 to 1/7/10 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 00
		=> le bouton "1.0.90" ecrit la valeur "0" dans l'adresse de groupe "1/7/10" => donne l'ordre d'éteindre la lumière
		KNX/EIBD:BusMonitorEvent	LPDU: BC 10 01 0F 0B E1 00 80 37 :L_Data low from 1.0.1 to 1/7/11 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 00
		=> l'actionneur "1.0.1" ecrit la valeur "0" dans le l'adresse de groupe "1/7/11" => inique au autres membres du groupe que sa valeur est "0" (indicateur d'etat)

	L'adresse de groupe "1/7/10" est utilisé pour écrire la valeur
	L'adresse de groupe "1/7/11" est utilisé pour lire la valeur

	Créez deux objets KNX dans le fichier ~\Drivers\KNX\knxobjects.xml

		* Un pour l'adresse de groupe "1/7/10" de type "1.001" : switching (on/off) (EIS1)
		<KNXObject id="ECL_PIECE1_TOR" gad="1/1/10" name="Eclairage Piece 1 - Plafond ON/OFF" type="1.001" flags="cwrtu"/>

		* Un pour l'adresse de groupe "1/7/11" de type "1.001" : switching (on/off) (EIS1)
		<KNXObject id="ECL_PIECE1_IE" gad="1/1/11" name="Eclairage Piece 1 - Plafond IE" type="1.001" />

	Puis créez un objet HMD dans le fichier ~\Drivers\KNX\config.xml

		<HMDObject id="ECL_PIECE1" name="Eclairage Piece 1 - Plafond" type="switch"> 
			<read id="ECL_PIECE1_IE" />
			<write id="ECL_PIECE1_TOR" />
		</HMDObject>


Ressources:
-----------
	* EIBD : http://www.auto.tuwien.ac.at/~mkoegler/index.php/eibd
	* BCU SDK : http://www.auto.tuwien.ac.at/~mkoegler/index.php/bcusdk
	* Installation EIBD sur Raspberry Pi : 
		* http://blog.hbprod.fr/2013/01/31/eibd-installation-sur-raspberry-pi/
		* http://wiki.knx-fr.com/doku.php?id=installation_sur_raspberry_pi
		* http://www.internetmosquito.com/2012/12/learning-how-to-control-existing-knx.html
		* http://ekblad.org/knx/pi.html
	
	* Falcon KNX SDK : http://www.knx.org/knx-tools/falcon/features/


