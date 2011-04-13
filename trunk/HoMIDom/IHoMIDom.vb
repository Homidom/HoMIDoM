Imports System
Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports System.Linq

Namespace HoMIDom

    '***********************************************
    '** INTERFACE SOAP HTTP
    '** Liste toutes les functions et propriétés accessibles par les clients
    '** version 1.0
    '** Date de création: 12/01/2011
    '** Historique (SebBergues): 12/01/2011: Création 
    '***********************************************



    ''' <summary>
    ''' Liste toutes les functions et propriétés accessibles par les clients
    ''' </summary>
    ''' <remarks></remarks>
    <ServiceContract(Namespace:="http://HoMIDom/")> Public Interface IHoMIDom
        '---- Subs --------------------------------------------

        <OperationContract()> Sub LoadDocument(ByVal Fichier As String, ByVal TablBytes() As Byte)

        ''' <summary>
        ''' Retourne l'heure du serveur
        ''' </summary>
        ''' <remarks></remarks>
        <OperationContract()> Function GetTime() As String

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
        ''' Obtient la liste des devices
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllDevices() As List(Of TemplateDevice)

        ''' <summary>
        ''' Obtient la liste des drivers
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllDrivers() As List(Of TemplateDriver)

        ''' <summary>
        ''' Obtient la liste des zones
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllZones() As List(Of Zone)

        ''' <summary>
        ''' Obtient la liste des users
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllUsers() As List(Of Users.User)

        ''' <summary>
        ''' Execute une commande (COMMAND) d'un device (DeviceID) associés à des paramètres (Param)
        ''' </summary>
        ''' <param name="DeviceId"></param>
        ''' <param name="Action"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub ExecuteDeviceCommand(ByVal DeviceId As String, ByVal Action As DeviceAction)

        ''' <summary>
        ''' Retourne la liste des devices d'une zone (par son id)
        ''' </summary>
        ''' <param name="zoneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetDeviceInZone(ByVal zoneId) As List(Of TemplateDevice)

        ''' <summary>
        ''' Execute une commande (COMMAND) d'un driver (DriverID) associés à des paramètres (Param)
        ''' </summary>
        ''' <param name="DriverId"></param>
        ''' <param name="Action"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub ExecuteDriverCommand(ByVal DriverId As String, ByVal Action As DeviceAction)

        ''' <summary>
        ''' Liste les méthodes (actions) dispo pour un device (par son id)
        ''' Retourne pour chaque élément de la liste NOMDELAMETHODE|Parametre1:TypeParametre1|Parametre2:TypeParametre2...
        ''' '' ex pour la classe lampe cela retourne: DIM|Variation:Int32
        ''' </summary>
        ''' <param name="DeviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ListMethod(ByVal DeviceId As String) As List(Of String)

        '---- Fonctions ---------------------------------------
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
        ''' Supprimer un driver de la config
        ''' </summary>
        ''' <param name="driverId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteDriver(ByVal driverId As String) As Integer

        ''' <summary>
        ''' Supprimer une zone de la config
        ''' </summary>
        ''' <param name="zoneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteZone(ByVal zoneId As String) As Integer

        ''' <summary>
        ''' Supprime un user
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteUser(ByVal userId As String) As Integer

        ''' <summary>
        ''' Retourne l'objet d'un device par son ID
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDeviceByID(ByVal Id As String) As TemplateDevice

        ''' <summary>
        ''' Retourne l'objet d'un driver par son ID
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDriverByID(ByVal Id As String) As TemplateDriver

        ''' <summary>
        ''' Retourne l'objet d'une zone par son ID
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnZoneByID(ByVal Id As String) As Zone

        ''' <summary>
        ''' Retourne l'objet d'un user par son ID
        ''' </summary>
        ''' <param name="UserId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnUserById(ByVal UserId As String) As Users.User

        ''' <summary>
        ''' Retourne l'objet d'un driver par son nom
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDriverByNom(ByVal name As String) As Object

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

        <OperationContract()> Function SaveDriver(ByVal driverId As String, ByVal name As String, ByVal enable As Boolean, ByVal startauto As Boolean, ByVal iptcp As String, ByVal porttcp As String, ByVal ipudp As String, ByVal portudp As String, ByVal com As String, ByVal refresh As Integer, ByVal picture As String, Optional ByVal Parametres As ArrayList = Nothing) As String 'Créer un nouveau driver ou sauvegarder la modif (si ID est complété)

        ''' <summary>
        ''' Créer un nouveau zone ou sauvegarder la modif (si ID est complété)
        ''' </summary>
        ''' <param name="zoneId"></param>
        ''' <param name="name"></param>
        ''' <param name="ListDevice"></param>
        ''' <param name="icon"></param>
        ''' <param name="image"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveZone(ByVal zoneId As String, ByVal name As String, Optional ByVal ListDevice As List(Of Zone.Device_Zone) = Nothing, Optional ByVal icon As String = "", Optional ByVal image As String = "") As String

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

        ''' <summary>
        ''' Commencer l'apprentissage d'un commande IR
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function StartIrLearning() As String

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

        ''' <summary>
        ''' Renvoi le fichier log suivant une requête xml si besoin
        ''' </summary>
        ''' <param name="Requete"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnLog(Optional ByVal Requete As String = "") As String

        '---- Propriétés --------------------------------------
        '''' <summary>
        '''' Liste des devices
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Property Devices() As ArrayList 'Liste des devices

        '''' <summary>
        '''' Liste des drivers
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Property Drivers() As ArrayList 'Liste des drivers

        '''' <summary>
        '''' Liste des zones
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Property Zones() As ArrayList 'Liste des zones

        '''' <summary>
        '''' Longitude du serveur pour calcul des heures de lever et coucher du soleil
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Property Longitude() As Double 'Longitude

        '''' <summary>
        '''' Latitude du serveur pour calcul des heures de lever et coucher du soleil
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Property Latitude() As Double 'Latitude

        '''' <summary>
        '''' Valeur de correction de l'heure de couché du soleil
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Property HeureCorrectionCoucher() As Integer

        '''' <summary>
        '''' Valeur de correction de l'heure de levé du soleil
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Property HeureCorrectionLever() As Integer

        '---- Variables ---------------------------------------

        '---- Events ---------------------------------------

    End Interface

End Namespace