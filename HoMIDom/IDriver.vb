Namespace HoMIDom

    '***********************************************
    '** INTERFACE DRIVER
    '** Liste toutes les functions et propriétés accessibles par les clients
    '** version 1.0
    '** Date de création: 17/01/2011
    '** Historique (SebBergues): 17/01/2011: Création 
    '***********************************************

    Public Interface IDriver

        Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object)

        Property Server() As Server
        Property ID() As String 'Identification unique du driver
        ReadOnly Property Nom() As String
        Property Enable() As Boolean
        ReadOnly Property Description() As String
        Property StartAuto() As Boolean
        ReadOnly Property Protocol() As String
        ReadOnly Property IsConnect() As Boolean
        Property IP_TCP() As String
        Property Port_TCP()
        Property IP_UDP() As String
        Property Port_UDP() As String
        Property COM() As String
        Property Refresh() As Integer
        ReadOnly Property Modele() As String
        ReadOnly Property Version() As String
        Property Picture() As String
        ReadOnly Property DeviceSupport() As ArrayList

        Sub Start()
        Sub [Stop]()
        Sub Restart()
        Sub Read(ByVal Objet As Object)
        Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing)

        'Fonctions à gérer par driver

    End Interface

End Namespace