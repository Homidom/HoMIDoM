
Namespace HoMIDom

    '***********************************************
    '** INTERFACE SOAP HTTP
    '** version 1.0
    '** Date de création: 12/01/2011
    '** Historique (SebBergues): 12/01/2011: Création 
    '***********************************************

    Public Interface IHoMIDom
        'Fonctions
        Function DeleteDevice(ByVal deviceId As String) As Integer 'Supprimer un device
        Function DeleteDriver(ByVal driverId As String) As Integer 'Supprimer un driver de la config
        Function ReturnDeviceByID(ByVal Id As String) As Object 'Retourne l'objet d'un device par son ID
        Function ReturnDriverByID(ByVal Id As String) As Object 'Retourne l'objet d'un driver par son ID
        Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal address2 As String, ByVal image As String, ByVal enable As Boolean, ByVal adapter As String, ByVal Parametres As String) As String 'Créer un nouveau device ou sauvegarder la modif (si ID est complété)
        Function SaveDriver(ByVal driverId As String, ByVal name As String, ByVal enable As Boolean, ByVal startauto As Boolean, ByVal iptcp As String, ByVal porttcp As String, ByVal ipudp As String, ByVal portudp As String, ByVal com As String, ByVal refresh As Integer, ByVal picture As String) As String 'Créer un nouveau driver ou sauvegarder la modif (si ID est complété)
        Function StartIrLearning() As String 'Commencer l'apprentissage d'un commande IR
        Function HeureLeverSoleil() As String 'Valeur du couché du soleil
        Function HeureCoucherSoleil() As String 'valeur du levé du soleil
        'Propriétés
        Property Devices() As ArrayList 'Liste des devices
        Property Drivers() As ArrayList 'Liste des drivers
    End Interface

End Namespace