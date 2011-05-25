Imports System
Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports System.Linq

Namespace HoMIDom

    ''' <summary>
    ''' Liste toutes les functions et propriétés accessibles par les clients
    ''' </summary>
    ''' <remarks></remarks>
    <ServiceContract(Namespace:="http://HoMIDom/")> Public Interface IHoMIDom

#Region "Serveur"
        ''' <summary>
        ''' Retourne la version du serveur
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetServerVersion() As String

        ''' <summary>
        ''' Retourne l'heure du serveur
        ''' </summary>
        ''' <remarks></remarks>
        <OperationContract()> Function GetTime() As String

        ''' <summary>
        ''' Permet d'envoyer un message d'un client vers le server
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub MessageToServer(ByVal Message As String)

        ''' <summary>
        ''' 'Sauvegarde de la configuration
        ''' </summary>
        ''' <remarks></remarks>
        <OperationContract()> Sub SaveConfig()

        ''' <summary>
        ''' Démarre le service et charge la config
        ''' </summary>
        ''' <remarks></remarks>
        <OperationContract()> Sub Start()

        ''' <summary>
        ''' Arrête le service et charge la config
        ''' </summary>
        ''' <remarks></remarks>
        <OperationContract()> Sub [Stop]()

        ''' <summary>
        ''' Fixe la valeur de port SOAP
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetPortSOAP(ByVal Value As Double)

        ''' <summary>
        ''' Retourne la valeur de port SOAP
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetPortSOAP() As Double

        ''' <summary>
        ''' Retourne la date et heure du dernier démarrage du serveur
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetLastStartTime() As Date

        ''' <summary>
        ''' Valeur du levé du soleil
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetHeureLeverSoleil() As String

        ''' <summary>
        ''' Valeur du couché du soleil
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetHeureCoucherSoleil() As String

        ''' <summary>
        ''' Obtenir la valeur de longitude
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetLongitude() As Double

        ''' <summary>
        ''' Appliquer une valeur de longitude
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetLongitude(ByVal Value As Double)

        ''' <summary>
        ''' Obtenir la valeur de latitude
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetLatitude() As Double

        ''' <summary>
        ''' Appliquer la valeur de latitude
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetLatitude(ByVal Value As Double)

        ''' <summary>
        ''' Obtenir la correction sur l'heure du coucher du soleil
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetHeureCorrectionCoucher() As Integer

        ''' <summary>
        ''' Appliquer une correction sur l'heure du coucher du soleil
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetHeureCorrectionCoucher(ByVal Value As Integer)

        ''' <summary>
        ''' Obtenir la correction sur l'heure de lever du soleil
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetHeureCorrectionLever() As Integer

        ''' <summary>
        ''' Appliquer une correction sur l'heure de lever du soleil
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetHeureCorrectionLever(ByVal Value As Integer)

        ''' <summary>
        '''  Convert a file on a byte array.
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetByteFromImage(ByVal file As String) As Byte()
#End Region

#Region "Audio"
        ''' <summary>
        ''' Supprimer une extension Audio
        ''' </summary>
        ''' <param name="NomExtension"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteExtensionAudio(ByVal NomExtension As String) As Integer

        ''' <summary>
        ''' Ajouter une nouvelle extension audio
        ''' </summary>
        ''' <param name="NomExtension"></param>
        ''' <param name="Enable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function NewExtensionAudio(ByVal NomExtension As String, Optional ByVal Enable As Boolean = False) As Integer

        ''' <summary>
        ''' Active ou désactive une extension Audio
        ''' </summary>
        ''' <param name="NomExtension"></param>
        ''' <param name="Enable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function EnableExtensionAudio(ByVal NomExtension As String, ByVal Enable As Boolean) As Integer

        ''' <summary>
        ''' Supprimer un répertoire Audio
        ''' </summary>
        ''' <param name="NomRepertoire"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteRepertoireAudio(ByVal NomRepertoire As String) As Integer

        ''' <summary>
        ''' Ajouter un nouveau répertoire audio
        ''' </summary>
        ''' <param name="NomRepertoire"></param>
        ''' <param name="Enable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function NewRepertoireAudio(ByVal NomRepertoire As String, Optional ByVal Enable As Boolean = False) As Integer

        ''' <summary>
        ''' Active ou désactive un répertoire Audio
        ''' </summary>
        ''' <param name="NomRepertoire"></param>
        ''' <param name="Enable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function EnableRepertoireAudio(ByVal NomRepertoire As String, ByVal Enable As Boolean) As Integer

        ''' <summary>
        ''' Obtient la liste des répertoires audio
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllRepertoiresAudio() As List(Of Audio.RepertoireAudio)

        ''' <summary>
        ''' Obtient la liste des extensions audio
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllExtensionsAudio() As List(Of Audio.ExtensionAudio)
#End Region

#Region "User"
        ''' <summary>
        ''' Supprime un user
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteUser(ByVal userId As String) As Integer

        ''' <summary>
        ''' Vérifie le couple username + login  renvoi true si ok
        ''' </summary>
        ''' <param name="Username"></param>
        ''' <param name="Password"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function VerifLogin(ByVal Username As String, ByVal Password As String) As Boolean

        ''' <summary>
        ''' Permet de changer le mot de passe d'un user
        ''' </summary>
        ''' <param name="Username"></param>
        ''' <param name="OldPassword"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ChangePassword(ByVal Username As String, ByVal OldPassword As String, ByVal ConfirmNewPassword As String, ByVal Password As String) As Boolean

        ''' <summary>
        ''' Retourne un user par son username
        ''' </summary>
        ''' <param name="Username"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnUserByUsername(ByVal Username As String) As Users.User

        ''' <summary>
        ''' Obtient la liste des users
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllUsers() As List(Of Users.User)

        ''' <summary>
        ''' Retourne l'objet d'un user par son ID
        ''' </summary>
        ''' <param name="UserId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnUserById(ByVal UserId As String) As Users.User

        ''' <summary>
        ''' Créer ou modifier un user
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <param name="UserName"></param>
        ''' <param name="Password"></param>
        ''' <param name="Profil"></param>
        ''' <param name="Nom"></param>
        ''' <param name="Prenom"></param>
        ''' <param name="NumberIdentification"></param>
        ''' <param name="Image"></param>
        ''' <param name="eMail"></param>
        ''' <param name="eMailAutre"></param>
        ''' <param name="TelFixe"></param>
        ''' <param name="TelMobile"></param>
        ''' <param name="TelAutre"></param>
        ''' <param name="Adresse"></param>
        ''' <param name="Ville"></param>
        ''' <param name="CodePostal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveUser(ByVal userId As String, ByVal UserName As String, ByVal Password As String, ByVal Profil As Users.TypeProfil, ByVal Nom As String, ByVal Prenom As String, Optional ByVal NumberIdentification As String = "", Optional ByVal Image As String = "", Optional ByVal eMail As String = "", Optional ByVal eMailAutre As String = "", Optional ByVal TelFixe As String = "", Optional ByVal TelMobile As String = "", Optional ByVal TelAutre As String = "", Optional ByVal Adresse As String = "", Optional ByVal Ville As String = "", Optional ByVal CodePostal As String = "") As String
#End Region

#Region "Device"
        ''' <summary>
        ''' Obtient la liste des devices
        ''' </summary>
        ''' <returns>List de TemplateDevice</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllDevices() As List(Of TemplateDevice)

        ''' <summary>
        ''' Execute une commande (COMMAND) d'un device (DeviceID) associés à des paramètres (Param)
        ''' </summary>
        ''' <param name="DeviceId"></param>
        ''' <param name="Action"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub ExecuteDeviceCommand(ByVal DeviceId As String, ByVal Action As DeviceAction)

        ''' <summary>
        ''' Supprimer un device
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteDevice(ByVal deviceId As String) As Integer

        ''' <summary>
        ''' Supprime une commande IR d'un device
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <param name="CmdName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteDeviceCommandIR(ByVal deviceId As String, ByVal CmdName As String) As Integer

        ''' <summary>
        ''' Retourne l'objet d'un device par son ID
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDeviceByID(ByVal Id As String) As TemplateDevice

        ''' <summary>
        ''' Retourne une liste de device suivant l'addresse1 et/ou son type et/ou le driver
        ''' </summary>
        ''' <param name="DeviceAdresse"></param>
        ''' <param name="DeviceType"></param>
        ''' <param name="DeviceDriver"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDeviceByAdresse1TypeDriver(ByVal DeviceAdresse As String, ByVal DeviceType As String, ByVal DeviceDriver As String) As ArrayList

        ''' <summary>
        ''' Créer un nouveau device ou sauvegarder la modif (si ID est complété)
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <param name="name"></param>
        ''' <param name="address1"></param>
        ''' <param name="enable"></param>
        ''' <param name="solo"></param>
        ''' <param name="driverid"></param>
        ''' <param name="type"></param>
        ''' <param name="refresh"></param>
        ''' <param name="address2"></param>
        ''' <param name="image"></param>
        ''' <param name="modele"></param>
        ''' <param name="description"></param>
        ''' <param name="lastchangeduree"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal enable As Boolean, ByVal solo As Boolean, ByVal driverid As String, ByVal type As String, ByVal refresh As Integer, Optional ByVal address2 As String = "", Optional ByVal image As String = "", Optional ByVal modele As String = "", Optional ByVal description As String = "", Optional ByVal lastchangeduree As Integer = 0) As String

        ''' <summary>
        ''' 'Ajouter ou modifier une commande IR à un device (utilisé pour usbuirt)
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <param name="CmdName"></param>
        ''' <param name="CmdData"></param>
        ''' <param name="CmdRepeat"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveDeviceCommandIR(ByVal deviceId As String, ByVal CmdName As String, ByVal CmdData As String, ByVal CmdRepeat As String) As String

        ''' <summary>
        ''' Commencer l'apprentissage d'un commande IR
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function StartIrLearning() As String
#End Region

#Region "Driver"
        ''' <summary>
        ''' Obtient la liste des drivers
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllDrivers() As List(Of TemplateDriver)

        ''' <summary>
        ''' Execute une commande (COMMAND) d'un driver (DriverID) associés à des paramètres (Param)
        ''' </summary>
        ''' <param name="DriverId"></param>
        ''' <param name="Action"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub ExecuteDriverCommand(ByVal DriverId As String, ByVal Action As DeviceAction)

        ''' <summary>
        ''' Retourne l'objet d'un driver par son ID
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDriverByID(ByVal Id As String) As TemplateDriver

        ''' <summary>
        ''' Supprimer un driver de la config
        ''' </summary>
        ''' <param name="driverId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteDriver(ByVal driverId As String) As Integer

        ''' <summary>
        ''' Retourne l'objet d'un driver par son nom
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDriverByNom(ByVal name As String) As Object

        ''' <summary>
        ''' Créer un nouveau driver ou sauvegarder la modif (si ID est complété)
        ''' </summary>
        ''' <param name="driverId"></param>
        ''' <param name="name"></param>
        ''' <param name="enable"></param>
        ''' <param name="startauto"></param>
        ''' <param name="iptcp"></param>
        ''' <param name="porttcp"></param>
        ''' <param name="ipudp"></param>
        ''' <param name="portudp"></param>
        ''' <param name="com"></param>
        ''' <param name="refresh"></param>
        ''' <param name="picture"></param>
        ''' <param name="Parametres"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveDriver(ByVal driverId As String, ByVal name As String, ByVal enable As Boolean, ByVal startauto As Boolean, ByVal iptcp As String, ByVal porttcp As String, ByVal ipudp As String, ByVal portudp As String, ByVal com As String, ByVal refresh As Integer, ByVal picture As String, Optional ByVal Parametres As ArrayList = Nothing) As String


        ''' <summary>
        ''' Arrêter un driver
        ''' </summary>
        ''' <param name="DriverId"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub StopDriver(ByVal DriverId As String)

        ''' <summary>
        ''' Démarrer un driver
        ''' </summary>
        ''' <param name="DriverId"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub StartDriver(ByVal DriverId As String)
#End Region

#Region "Zone"
        ''' <summary>
        ''' Indique si la zone ne contient aucun device (exemple à vérifier avant de supprimer une zone)
        ''' </summary>
        ''' <param name="zoneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ZoneIsEmpty(ByVal zoneId) As Boolean

        ''' <summary>
        ''' Supprimer une zone de la config
        ''' </summary>
        ''' <param name="zoneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteZone(ByVal zoneId As String) As Integer

        ''' <summary>
        ''' Obtient la liste des zones
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllZones() As List(Of Zone)

        ''' <summary>
        ''' Retourne la liste des devices d'une zone (par son id)
        ''' </summary>
        ''' <param name="zoneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetDeviceInZone(ByVal zoneId) As List(Of TemplateDevice)

        ''' <summary>
        ''' Retourne l'objet d'une zone par son ID
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnZoneByID(ByVal Id As String) As Zone

        ''' <summary>
        ''' Créer un nouveau zone ou sauvegarder la modif (si ID est complété)
        ''' </summary>
        ''' <param name="zoneId"></param>
        ''' <param name="name"></param>
        ''' <param name="ListElement"></param>
        ''' <param name="icon"></param>
        ''' <param name="image"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveZone(ByVal zoneId As String, ByVal name As String, Optional ByVal ListElement As List(Of Zone.Element_Zone) = Nothing, Optional ByVal icon As String = "", Optional ByVal image As String = "") As String

        ''' <summary>
        ''' Ajouter un device à une zone
        ''' </summary>
        ''' <param name="ZoneId"></param>
        ''' <param name="DeviceId"></param>
        ''' <param name="Visible"></param>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function AddDeviceToZone(ByVal ZoneId As String, ByVal DeviceId As String, ByVal Visible As Boolean, Optional ByVal X As Double = 0, Optional ByVal Y As Double = 0) As String

        ''' <summary>
        ''' Supprimer un device à une zone
        ''' </summary>
        ''' <param name="ZoneId"></param>
        ''' <param name="DeviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <OperationContract()> Function DeleteDeviceToZone(ByVal ZoneId As String, ByVal DeviceId As String) As String
#End Region

#Region "Macro"
        ''' <summary>
        ''' Supprimer une macro de la config
        ''' </summary>
        ''' <param name="macroId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteMacro(ByVal macroId As String) As Integer

        ''' <summary>Retourne la liste de toutes les macros</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllMacros() As List(Of Macro)

        ''' <summary>Retourne la macro par son ID</summary>
        ''' <param name="MacroId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnMacroById(ByVal MacroId As String) As Macro

        ''' <summary>
        ''' Permet de créer ou modifier une macro
        ''' </summary>
        ''' <param name="macroId"></param>
        ''' <param name="nom"></param>
        ''' <param name="enable"></param>
        ''' <param name="description"></param>
        ''' <param name="listactions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveMacro(ByVal macroId As String, ByVal nom As String, ByVal enable As Boolean, Optional ByVal description As String = "", Optional ByVal listactions As List(Of TemplateAction) = Nothing) As String
#End Region

#Region "Trigger"
        ''' <summary>Retourne la liste de toutes les triggers</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllTriggers() As List(Of Trigger)

        ''' <summary>Retourne le trigger par son ID</summary>
        ''' <param name="TriggerId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnTriggerById(ByVal TriggerId As String) As Trigger

        ''' <summary>
        ''' Supprimer un trigger de la config
        ''' </summary>
        ''' <param name="triggerId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteTrigger(ByVal triggerId As String) As Integer

        ''' <summary>
        ''' Permet de créer ou modifier un trigger
        ''' </summary>
        ''' <param name="triggerId"></param>
        ''' <param name="nom"></param>
        ''' <param name="enable"></param>
        ''' <param name="description"></param>
        ''' <param name="conditiontimer"></param>
        ''' <param name="macro"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveTrigger(ByVal triggerId As String, ByVal nom As String, ByVal enable As Boolean, ByVal TypeTrigger As Trigger.TypeTrigger, Optional ByVal description As String = "", Optional ByVal conditiontimer As String = "", Optional ByVal deviceid As String = "", Optional ByVal deviceproperty As String = "", Optional ByVal macro As ArrayList = Nothing) As String
#End Region

#Region "Divers"
        ''' <summary>
        ''' Liste les méthodes (actions) dispo pour un device (par son id)
        ''' Retourne pour chaque élément de la liste NOMDELAMETHODE|Parametre1:TypeParametre1|Parametre2:TypeParametre2...
        ''' '' ex pour la classe lampe cela retourne: DIM|Variation:Int32
        ''' </summary>
        ''' <param name="DeviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ListMethod(ByVal DeviceId As String) As List(Of String)

#End Region

#Region "Log"

        ''' <summary>Ecrit un log dans le fichier log au format xml</summary>
        ''' <param name="TypLog"></param>
        ''' <param name="Source"></param>
        ''' <param name="Fonction"></param>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub Log(ByVal TypLog As HoMIDom.Server.TypeLog, ByVal Source As HoMIDom.Server.TypeSource, ByVal Fonction As String, ByVal Message As String)

        ''' <summary>
        ''' Renvoi le fichier log suivant une requête xml si besoin
        ''' </summary>
        ''' <param name="Requete"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnLog(Optional ByVal Requete As String = "") As String

        ''' <summary>
        ''' Fixe la taille max du fichier log en Ko avant d'en créer un nouveau
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetMaxFileSizeLog(ByVal Value As Long)

        ''' <summary>
        ''' Retourne la taille max du fichier log en Ko
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetMaxFileSizeLog() As Long
#End Region

#Region "SMTP"
        ''' <summary>
        ''' Retourne l'adresse SMTP du serveur
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetSMTPServeur() As String

        ''' <summary>
        ''' Fixe l'adresse SMTP du serveur
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetSMTPServeur(ByVal Value As String)

        ''' <summary>
        ''' Retourne le login du serveur SMTP
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetSMTPLogin() As String

        ''' <summary>
        ''' Fixe le login du serveur SMTP
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetSMTPLogin(ByVal Value As String)

        ''' <summary>
        ''' Retourne le password du serveur SMTP
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetSMTPPassword() As String

        ''' <summary>
        ''' Fixe le password du serveur SMTP
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetSMTPPassword(ByVal Value As String)

        ''' <summary>
        ''' Retourne l'adresse mail du serveur SMTP
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetSMTPMailServeur() As String

        ''' <summary>
        ''' Fixe l'adresse mail du serveur SMTP
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetSMTPMailServeur(ByVal Value As String)
#End Region

    End Interface

End Namespace