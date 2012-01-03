Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports ActiveHomeScriptLib

Public Class Driver_X10_CM15
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "1DA44C90-353F-11E1-9B3D-78FC4824019B"
    Dim _Nom As String = "X10 CM15"
    Dim _Enable As String = False
    Dim _Description As String = "Driver X10 CM15"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "USB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "CM15"
    Dim _Version As String = "1.0"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    Dim WithEvents ActiveHomeObj As ActiveHome
#End Region

#Region "Fonctions génériques"

    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _idsrv = value
        End Set
    End Property

    Public Property COM() As String Implements HoMIDom.HoMIDom.IDriver.COM
        Get
            Return _Com
        End Get
        Set(ByVal value As String)
            _Com = value
        End Set
    End Property

    Public ReadOnly Property Description() As String Implements HoMIDom.HoMIDom.IDriver.Description
        Get
            Return _Description
        End Get
    End Property

    Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
        Get
            Return _DeviceSupport
        End Get
    End Property

    Public Property Parametres() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.Parametres
        Get
            Return _Parametres
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _Parametres = value
        End Set
    End Property

    Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements HoMIDom.HoMIDom.IDriver.DriverEvent

    Public Property Enable() As Boolean Implements HoMIDom.HoMIDom.IDriver.Enable
        Get
            Return _Enable
        End Get
        Set(ByVal value As Boolean)
            _Enable = value
        End Set
    End Property

    Public ReadOnly Property ID() As String Implements HoMIDom.HoMIDom.IDriver.ID
        Get
            Return _ID
        End Get
    End Property

    Public Property IP_TCP() As String Implements HoMIDom.HoMIDom.IDriver.IP_TCP
        Get
            Return _IP_TCP
        End Get
        Set(ByVal value As String)
            _IP_TCP = value
        End Set
    End Property

    Public Property IP_UDP() As String Implements HoMIDom.HoMIDom.IDriver.IP_UDP
        Get
            Return _IP_UDP
        End Get
        Set(ByVal value As String)
            _IP_UDP = value
        End Set
    End Property

    Public ReadOnly Property IsConnect() As Boolean Implements HoMIDom.HoMIDom.IDriver.IsConnect
        Get
            Return _IsConnect
        End Get
    End Property

    Public Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
        Get
            Return _Modele
        End Get
        Set(ByVal value As String)
            _Modele = value
        End Set
    End Property

    Public ReadOnly Property Nom() As String Implements HoMIDom.HoMIDom.IDriver.Nom
        Get
            Return _Nom
        End Get
    End Property

    Public Property Picture() As String Implements HoMIDom.HoMIDom.IDriver.Picture
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property

    Public Property Port_TCP() As Object Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As Object)
            _Port_TCP = value
        End Set
    End Property

    Public Property Port_UDP() As String Implements HoMIDom.HoMIDom.IDriver.Port_UDP
        Get
            Return _Port_UDP
        End Get
        Set(ByVal value As String)
            _Port_UDP = value
        End Set
    End Property

    Public ReadOnly Property Protocol() As String Implements HoMIDom.HoMIDom.IDriver.Protocol
        Get
            Return _Protocol
        End Get
    End Property

    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        If _Enable = False Then Exit Sub

        Try
            'ActiveHomeObj.SendAction(
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: " & ex.ToString)
        End Try
        ' Objet.Value = ReadBinaireChannel(Objet.Adresse1)
    End Sub

    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property

    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property

    ''' <summary>
    ''' Retourne la liste des Commandes avancées
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCommandPlus() As List(Of DeviceCommande)
        Return _DeviceCommandPlus
    End Function

    ''' <summary>
    ''' Execute une commande avancée
    ''' </summary>
    ''' <param name="Command"></param>
    ''' <param name="Param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False

        If Command = "" Then
            Return False
            Exit Function
        End If

        Select Case UCase(Command)
            Case ""
            Case Else
        End Select

        Return retour
    End Function

    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        'cree l'objet
        Try
            ActiveHomeObj = CreateObject("X10.ActiveHome")
            If ActiveHomeObj IsNot Nothing Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Connectée")
                _IsConnect = True
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur lors du démarrage du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property

    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        'cree l'objet
        Try
            ActiveHomeObj = Nothing
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver arrêté")
            _IsConnect = False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", "Erreur lors de l'arrêt du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        If _Enable = False Then Exit Sub
        If _IsConnect = False Then Exit Sub
        Try
            Dim TypeDev As String

            Select Case Objet.Adresse2.ToString
                Case "0" Or " " Or ""
                    TypeDev = "sendplc"
                Case 1
                    TypeDev = "sendrf"
                Case 2
                    TypeDev = "sendsecurerf"
                Case 3
                    TypeDev = "sendsecurehomecontrolrf"
                Case Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur le type de device (adresse2) est inconnu (0--3): " & Objet.adresse2)
                    Exit Sub
            End Select
            If Commande = "ON" Then
                ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " on")
            End If
            If Commande = "OFF" Then
                ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " off")
            End If
            If Commande = "DIM" Then
                ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " dim " & Parametre1)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: " & ex.ToString)
        End Try
    End Sub

    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

    End Sub

    Public Sub New()
        _DeviceSupport.Add(ListeDevices.SWITCH)
        _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
        _DeviceSupport.Add(ListeDevices.CONTACT)
        _DeviceSupport.Add(ListeDevices.APPAREIL)

        'ajout des commandes avancées pour les devices
        'Ci-dessous un exemple
        'Dim x As New DeviceCommande
        'x.NameCommand = "Test"
        'x.DescriptionCommand = "Ceci est une commande avancée de test"
        'x.CountParam = 1
        '_DeviceCommandPlus.Add(x)
    End Sub
#End Region

#Region "Fonctions propres au driver"
    'events from ActiveHome: write out received event
    Sub ActiveHome_RecvAction(ByVal bszRecv As Object _
                            , ByVal vParm1 As Object _
                            , ByVal vParm2 As Object _
                            , ByVal vParm3 As Object _
                            , ByVal vParm4 As Object _
                            , ByVal vParm5 As Object _
                            , ByVal vReserved As Object) Handles ActiveHomeObj.RecvAction

        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " RecvAction", "RecvAction: " & bszRecv & " " & vParm1 & " " & vParm2 & " " & vParm3 & " " & vParm4 & " " & vParm5)

        If vParm3 <> "" Then
            Try
                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, vParm2, "", Me._ID, True)
                'un device trouvé on maj la value
                If (listedevices.Count = 1) Then
                    'correction valeur pour correspondre au type de value
                    If TypeOf listedevices.Item(0).Value Is Integer Then
                        If UCase(vParm3) = "ON" Then
                            listedevices.Item(0).Value = 100
                        ElseIf UCase(vParm3) = "OFF" Then
                            listedevices.Item(0).Value = 0
                        End If
                    ElseIf TypeOf listedevices.Item(0).Value Is Boolean Then
                        If UCase(vParm3) = "ON" Then
                            listedevices.Item(0).Value = True
                        ElseIf UCase(vParm3) = "OFF" Then
                            listedevices.Item(0).Value = False
                        Else
                            listedevices.Item(0).Value = True
                        End If
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " RecvAction", "Aucun ou Plusieurs devices correspondent à : " & vParm2 & ":" & vParm3)
                End If

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " RecvAction", "Erreur : " & ex.ToString)
            End Try
        End If
    End Sub
#End Region

End Class
