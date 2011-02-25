
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
    Public Interface IHoMIDom
        '---- Subs --------------------------------------------
        ''' <summary>
        ''' 'Sauvegarde de la configuration
        ''' </summary>
        ''' <remarks></remarks>
        Sub SaveConfig()

        ''' <summary>
        ''' Execute une commande d'un device
        ''' </summary>
        ''' <param name="DeviceId"></param>
        ''' <param name="Command"></param>
        ''' <param name="Param"></param>
        ''' <remarks></remarks>
        Sub ExecuteDeviceCommand(ByVal DeviceId As String, ByVal Command As String, ByVal Param As ArrayList)


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
        Function AddDeviceToZone(ByVal ZoneId As String, ByVal DeviceId As String, ByVal Visible As Boolean, Optional ByVal X As Double = 0, Optional ByVal Y As Double = 0) As String

        ''' <summary>
        ''' Supprimer un device à une zone
        ''' </summary>
        ''' <param name="ZoneId"></param>
        ''' <param name="DeviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Function DeleteDeviceToZone(ByVal ZoneId As String, ByVal DeviceId As String) As String

        ''' <summary>
        ''' Supprimer un device
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DeleteDevice(ByVal deviceId As String) As Integer

        ''' <summary>
        ''' Supprime une commande IR d'un device
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <param name="CmdName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DeleteDeviceCommandIR(ByVal deviceId As String, ByVal CmdName As String) As Integer

        ''' <summary>
        ''' Supprimer un driver de la config
        ''' </summary>
        ''' <param name="driverId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DeleteDriver(ByVal driverId As String) As Integer

        ''' <summary>
        ''' Supprimer une zone de la config
        ''' </summary>
        ''' <param name="zoneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DeleteZone(ByVal zoneId As String) As Integer

        ''' <summary>
        ''' Retourne l'objet d'un device par son ID
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ReturnDeviceByID(ByVal Id As String) As Object

        ''' <summary>
        ''' Retourne l'objet d'un driver par son ID
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ReturnDriverByID(ByVal Id As String) As Object

        ''' <summary>
        ''' Retourne l'objet d'une zone par son ID
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ReturnZoneByID(ByVal Id As String) As Object

        ''' <summary>
        ''' Retourne l'objet d'un driver par son nom
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ReturnDriverByNom(ByVal name As String) As Object

        ''' <summary>
        ''' Retourne une liste de device suivant l'addresse1 et/ou son type et/ou le driver
        ''' </summary>
        ''' <param name="DeviceAdresse"></param>
        ''' <param name="DeviceType"></param>
        ''' <param name="DeviceDriver"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ReturnDeviceByAdresse1TypeDriver(ByVal DeviceAdresse As String, ByVal DeviceType As String, ByVal DeviceDriver As String) As ArrayList

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
        Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal enable As Boolean, ByVal solo As Boolean, ByVal driverid As String, ByVal type As String, ByVal refresh As Integer, Optional ByVal address2 As String = "", Optional ByVal image As String = "", Optional ByVal modele As String = "", Optional ByVal description As String = "", Optional ByVal lastchangeduree As Integer = 0) As String

        ''' <summary>
        ''' 'Ajouter ou modifier une commande IR à un device (utilisé pour usbuirt)
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <param name="CmdName"></param>
        ''' <param name="CmdData"></param>
        ''' <param name="CmdRepeat"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SaveDeviceCommandIR(ByVal deviceId As String, ByVal CmdName As String, ByVal CmdData As String, ByVal CmdRepeat As String) As String

        Function SaveDriver(ByVal driverId As String, ByVal name As String, ByVal enable As Boolean, ByVal startauto As Boolean, ByVal iptcp As String, ByVal porttcp As String, ByVal ipudp As String, ByVal portudp As String, ByVal com As String, ByVal refresh As Integer, ByVal picture As String) As String 'Créer un nouveau driver ou sauvegarder la modif (si ID est complété)

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
        Function SaveZone(ByVal zoneId As String, ByVal name As String, Optional ByVal ListDevice As ArrayList = Nothing, Optional ByVal icon As String = "", Optional ByVal image As String = "") As String

        ''' <summary>
        ''' Commencer l'apprentissage d'un commande IR
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function StartIrLearning() As String

        ''' <summary>
        ''' Valeur du levé du soleil
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HeureLeverSoleil() As String

        ''' <summary>
        ''' Valeur du couché du soleil
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HeureCoucherSoleil() As String

        ''' <summary>
        ''' Renvoi le fichier log suivant une requête xml si besoin
        ''' </summary>
        ''' <param name="Requete"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ReturnLog(Optional ByVal Requete As String = "") As String

        '---- Propriétés --------------------------------------
        ''' <summary>
        ''' Liste des devices
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Devices() As ArrayList 'Liste des devices

        ''' <summary>
        ''' Liste des drivers
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Drivers() As ArrayList 'Liste des drivers

        ''' <summary>
        ''' Liste des zones
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Zones() As ArrayList 'Liste des zones

        ''' <summary>
        ''' Longitude du serveur pour calcul des heures de lever et coucher du soleil
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Longitude() As Double 'Longitude

        ''' <summary>
        ''' Latitude du serveur pour calcul des heures de lever et coucher du soleil
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Latitude() As Double 'Latitude

        ''' <summary>
        ''' Valeur de correction de l'heure de couché du soleil
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property HeureCorrectionCoucher() As Integer

        ''' <summary>
        ''' Valeur de correction de l'heure de levé du soleil
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property HeureCorrectionLever() As Integer

        '---- Variables ---------------------------------------

        '---- Events ---------------------------------------

    End Interface

End Namespace