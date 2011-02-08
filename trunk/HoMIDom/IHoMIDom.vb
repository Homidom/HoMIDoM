
Namespace HoMIDom

    '***********************************************
    '** INTERFACE SOAP HTTP
    '** Liste toutes les functions et propriétés accessibles par les clients
    '** version 1.0
    '** Date de création: 12/01/2011
    '** Historique (SebBergues): 12/01/2011: Création 
    '***********************************************

    Public Interface IHoMIDom
        '---- Subs --------------------------------------------
        Sub SaveConfig() 'Sauvegarde de la configuration

        '---- Fonctions ---------------------------------------
        Function DeleteDevice(ByVal deviceId As String) As Integer 'Supprimer un device
        Function DeleteDriver(ByVal driverId As String) As Integer 'Supprimer un driver de la config
        Function ReturnDeviceByID(ByVal Id As String) As Object 'Retourne l'objet d'un device par son ID
        Function ReturnDriverByID(ByVal Id As String) As Object 'Retourne l'objet d'un driver par son ID
        Function ReturnDriverByNom(ByVal name As String) As Object 'Retourne l'objet d'un driver par son nom
        Function ReturnDeviceByAdresse1TypeDriver(ByVal DeviceAdresse As String, ByVal DeviceType As String, ByVal DeviceDriver As String) As arraylist 'retourne une liste de device suivant l'addresse1 et/ou son type et/ou le driver
        Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal enable As Boolean, ByVal solo As Boolean, ByVal driverid As String, ByVal type As String, ByVal refresh As Integer, Optional ByVal address2 As String = "", Optional ByVal image As String = "", Optional ByVal modele As String = "", Optional ByVal description As String = "", Optional ByVal lastchangeduree As Integer = 0) As String 'Créer un nouveau device ou sauvegarder la modif (si ID est complété)
        Function SaveDriver(ByVal driverId As String, ByVal name As String, ByVal enable As Boolean, ByVal startauto As Boolean, ByVal iptcp As String, ByVal porttcp As String, ByVal ipudp As String, ByVal portudp As String, ByVal com As String, ByVal refresh As Integer, ByVal picture As String) As String 'Créer un nouveau driver ou sauvegarder la modif (si ID est complété)
        Function StartIrLearning() As String 'Commencer l'apprentissage d'un commande IR
        Function HeureLeverSoleil() As String 'Valeur du couché du soleil
        Function HeureCoucherSoleil() As String 'valeur du levé du soleil
        Function ReturnLog(Optional ByVal Requete As String = "") As String 'renvoi le fichier log suivant une requête xml si besoin

        '---- Propriétés --------------------------------------
        Property Devices() As ArrayList 'Liste des devices
        Property Drivers() As ArrayList 'Liste des drivers
        Property Longitude() As Double 'Longitude
        Property Latitude() As Double 'Latitude
        Property HeureCorrectionCoucher() As Integer
        Property HeureCorrectionLever() As Integer

        '---- Variables ---------------------------------------

        '---- Events ---------------------------------------

    End Interface

End Namespace