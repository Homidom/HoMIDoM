'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports VB = Microsoft.VisualBasic
Imports System.Net
Imports System.Net.Sockets


' Auteur : Mathieu35 sur une base HoMIDoM
' Date : 03/03/2013

''' <summary>Class Driver_ModbusTCP, permet de commander et recevoir des ordres avec les périphériques supportant le protocole ModbusTCP</summary>
''' 
<Serializable()> Public Class Driver_ModbusTCP
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "7B5F099E-85A0-11E2-81FC-248E6188709B"
    Dim _Nom As String = "ModbusTCP"
    Dim _Enable As Boolean = False
    Dim _Description As String = "ModbusTCP"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "TCP"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "192.168.0.40"
    Dim _Port_TCP As String = "502"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Wago"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Dim _LabelsDevice As New ArrayList
    Dim WithEvents MyTimer As New Timers.Timer
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False
    Dim _DEBUG As Boolean = False

#End Region

#Region "Variables internes"

    Private WithEvents MBmaster As ModbusTCP.Master

    Private breading As Boolean = False
    Private cptsend As Integer = 0
    Private typeRead As String = "MW"
    Private flagWrite As Boolean = False
    Private cptWaitReponse As Integer = 0
    Private offsetModele As Integer = 12288
    Private rcvIX As Boolean = False

    Private rID As UShort
    Private MemID As UShort

    Private adressReadMW As Integer
    Private longReadMW As Integer
    Private adressReadIW As Integer
    Private longReadIW As Integer
    Private adressReadMX As Integer
    Private longReadMX As Integer
    Private adressWriteMW As Integer
    Private adressWriteMX As Integer
    Private unit As Integer

    Private nbsendMW As Integer
    Private nbsendIW As Integer
    Private nbsendMX As Integer

    Private dataMdbMW() As UInt16
    Private dataMdbIW() As UInt16
    Private dataMdbMX() As Boolean
    Private dataMdbIX() As Boolean

    Dim str_to_bool As New Dictionary(Of String, Integer)

#End Region

#Region "Propriétés génériques"
    Dim TEMP As String

    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
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

    Public Property LabelsDriver() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDriver
        Get
            Return _LabelsDriver
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDriver = value
        End Set
    End Property
    Public Property LabelsDevice() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDevice
        Get
            Return _LabelsDevice
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDevice = value
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
    Public Property Port_TCP() As String Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
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
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property
    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property
    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property
    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
        End Get
    End Property
    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property
    Public Property AutoDiscover() As Boolean Implements HoMIDom.HoMIDom.IDriver.AutoDiscover
        Get
            Return _AutoDiscover
        End Get
        Set(ByVal value As Boolean)
            _AutoDiscover = value
        End Set
    End Property
#End Region

#Region "Fonctions génériques"

    ''' <summary>
    ''' Retourne la liste des Commandes avancées
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCommandPlus() As List(Of DeviceCommande)
        Return _DeviceCommandPlus
    End Function

    ''' <summary>Execute une commande avancée</summary>
    ''' <param name="MyDevice">Objet représentant le Device </param>
    ''' <param name="Command">Nom de la commande avancée à éxécuter</param>
    ''' <param name="Param">tableau de paramétres</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False
        Try
            If MyDevice IsNot Nothing Then
                'Pas de commande demandée donc erreur
                If Command = "" Then
                    Return False
                Else
                    Write(MyDevice, Command, Param(0), Param(1))
                    'Select Case UCase(Command)
                    '    Case ""
                    '    Case Else
                    'End Select
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "exception : " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Permet de vérifier si un champ est valide
    ''' </summary>
    ''' <param name="Champ"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1" ' Adresse Lecture 
                    If Value > adressReadMX + longReadMX Then

                    End If
                    If Value > adressReadMW + longReadMW Then

                    End If
                    If Value > adressReadIW + longReadIW Then

                    End If
                    If Value > 511 Then

                    End If
                    If Value < adressReadMX Then

                    End If
                    If Value < adressReadMW Then

                    End If
                    If Value < adressReadIW Then

                    End If
                    

                Case "ADRESSE2" 'Adresse Ecriture 
                    If adressWriteMX + Value > 20479 Then

                    End If
                    If adressWriteMX + Value > 12287 Then

                    End If
                    If Value > 511 Then

                    End If
                    If Value > 255 Then

                    End If
                    If Value < adressWriteMX Then

                    End If
                    If Value < adressWriteMW Then

                    End If
            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Dim retour As String = ""
        'récupération des paramétres avancés
       
        'ouverture du port suivant le Port Com ou IP
        Try
            If _IP_TCP <> "" And _Port_TCP <> "" Then
                retour = ouvrir(_IP_TCP, _Port_TCP)
            Else
                retour = "ERR: Port ou Adresse non défini. Impossible de se connecter !"
            End If
            'traitement du message de retour
            If STRGS.Left(retour, 4) = "ERR:" Then
                _IsConnect = False
                retour = STRGS.Right(retour, retour.Length - 5)
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP", "Driver non démarré : " & retour)
            Else
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "ModbusTCP", retour)

                Select Case _Modele.ToUpper
                    Case "WAGO"
                        offsetModele = 12288
                    Case Else
                        offsetModele = 0
                End Select

                If TypeOf _Parametres.Item(4).Valeur Is Boolean Then
                    _Parametres.Item(9).Valeur = _Parametres.Item(3).Valeur
                    _Parametres.Item(10).Valeur = _Parametres.Item(4).Valeur
                    _Parametres.Item(3).Valeur = _Parametres.Item(1).Valeur
                    _Parametres.Item(8).Valeur = _Parametres.Item(2).Valeur
                    _Parametres.Item(1).Valeur = 0
                    _Parametres.Item(2).Valeur = 0
                    _Parametres.Item(4).Valeur = 225
                End If

                If _Parametres.Item(2).Valeur > 20479 Then _Parametres.Item(2).Valeur = 20479
                If _Parametres.Item(1).Valeur > 20479 Then _Parametres.Item(1).Valeur = 20478
                If (CInt(_Parametres.Item(2).Valeur) + CInt(_Parametres.Item(1).Valeur)) > 20479 Then _Parametres.Item(2).Valeur = 20479 - _Parametres.Item(1).Valeur
                If _Parametres.Item(4).Valeur > 12287 Then _Parametres.Item(4).Valeur = 12287
                If _Parametres.Item(3).Valeur > 12287 Then _Parametres.Item(3).Valeur = 12286
                If (CInt(_Parametres.Item(4).Valeur) + CInt(_Parametres.Item(3).Valeur)) > 12287 Then _Parametres.Item(4).Valeur = 12287 - _Parametres.Item(3).Valeur
                If _Parametres.Item(6).Valeur > 255 Then _Parametres.Item(6).Valeur = 255
                If _Parametres.Item(5).Valeur > 255 Then _Parametres.Item(5).Valeur = 254
                If (CInt(_Parametres.Item(6).Valeur) + CInt(_Parametres.Item(5).Valeur)) > 12287 Then _Parametres.Item(6).Valeur = 255 - _Parametres.Item(5).Valeur
                If _Parametres.Item(7).Valeur > 20479 Then _Parametres.Item(7).Valeur = 20479
                If _Parametres.Item(8).Valeur > 12287 Then _Parametres.Item(8).Valeur = 12287

                adressReadMX = _Parametres.Item(1).Valeur
                longReadMX = _Parametres.Item(2).Valeur
                adressReadMW = _Parametres.Item(3).Valeur
                longReadMW = _Parametres.Item(4).Valeur
                adressReadIW = _Parametres.Item(5).Valeur
                longReadIW = _Parametres.Item(6).Valeur
                adressWriteMX = _Parametres.Item(7).Valeur
                adressWriteMW = _Parametres.Item(8).Valeur
                unit = _Parametres.Item(9).Valeur
                _DEBUG = _Parametres.Item(10).Valeur

                Dim ConfigTxt As String = " Conf ModbusTCP= "
                For i = 0 To 10
                    ConfigTxt += i & "=" & _Parametres.Item(i).Valeur & " "
                Next
                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ModbusTCP Start", ConfigTxt)

                MyTimer.Interval = _Parametres.Item(0).Valeur '2000
                MyTimer.Enabled = True
                breading = False
                cptsend = 0

                If longReadMW <> 0 Then nbsendMW = Math.Floor((longReadMW - 1) / 75) + 1 Else nbsendMW = 0
                If longReadIW <> 0 Then nbsendIW = Math.Floor((longReadIW - 1) / 75) + 1 Else nbsendIW = 0
                If longReadMX <> 0 Then nbsendMX = Math.Floor((longReadMX - 1) / 512) + 1 Else nbsendMX = 0

                ReDim dataMdbMW(nbsendMW * 75)
                ReDim dataMdbIW(nbsendIW * 75)
                ReDim dataMdbMX(nbsendMX * 512)
                ReDim dataMdbIX(512)

                AddHandler _Server.DeviceChanged, AddressOf DeviceChange

            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP Start", ex.Message)
            _IsConnect = False
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Dim retour As String
        Try
            MyTimer.Enabled = False
            retour = fermer()
            RemoveHandler _Server.DeviceChanged, AddressOf DeviceChange

            If STRGS.Left(retour, 4) = "ERR:" Then
                retour = STRGS.Right(retour, retour.Length - 5)
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP", retour)
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "ModbusTCP", retour)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP Stop", ex.Message)
        End Try
        _IsConnect = False
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        Try
            [Stop]()
            Start()
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP ReStart", ex.Message)
        End Try
    End Sub

    ''' <summary>Intérroger un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car le driver n'est pas Activé")
                Exit Sub
            End If
            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car le driver n'est pas connecté à la carte")
                Exit Sub
            End If
            If CInt(Objet.Adresse1) > "-1" And Objet.Adresse1 <> "" Then
                If (Objet.Model = "MX" And CInt(Objet.Adresse1) < adressReadMX + longReadMX And CInt(Objet.Adresse1) >= adressReadMX) Then
               
                    If TypeOf Objet.Value Is Boolean And dataMdbMX(Objet.adresse1) > -1 And dataMdbMX(Objet.adresse1) < 2 Then
                        Objet.Value = CBool(dataMdbMX(Objet.adresse1 - 1))
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "L'adresse du composant n'est pas dans la plage de lecture du driver")

                End If

                If (Objet.Model = "IX" And CInt(Objet.Adresse1) < 512 And CInt(Objet.Adresse1) >= 0) Then

                    If TypeOf Objet.Value Is Boolean And dataMdbIX(Objet.adresse1) > -1 And dataMdbIX(Objet.adresse1) < 2 Then
                        Objet.Value = CBool(dataMdbIX(Objet.adresse1 - 1))
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "L'adresse du composant n'est pas dans la plage de lecture du driver")

                End If

                If (Objet.Model = "MW" And CInt(Objet.Adresse1) < adressReadMW + longReadMW And CInt(Objet.Adresse1) >= adressReadMW) Then

                    If TypeOf Objet.Value Is Integer Or TypeOf Objet.Value Is Double Then
                        Objet.Value = dataMdbMW(Objet.adresse1 - 1)
                    End If
                    If TypeOf Objet.Value Is Boolean And dataMdbMW(Objet.adresse1) > -1 And dataMdbMW(Objet.adresse1) < 2 Then
                        Objet.Value = CBool(dataMdbMW(Objet.adresse1 - 1))
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "L'adresse du composant n'est pas dans la plage de lecture du driver")

                End If

                If (Objet.Model = "IW" And CInt(Objet.Adresse1) < adressReadIW + longReadIW And CInt(Objet.Adresse1) >= adressReadIW) Then

                    If TypeOf Objet.Value Is Integer Or TypeOf Objet.Value Is Double Then
                        Objet.Value = dataMdbIW(Objet.adresse1 - 1)
                    End If
                    If TypeOf Objet.Value Is Boolean And dataMdbIW(Objet.adresse1) > -1 And dataMdbIW(Objet.adresse1) < 2 Then
                        Objet.Value = CBool(dataMdbIW(Objet.adresse1 - 1))
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "L'adresse du composant n'est pas dans la plage de lecture du driver")

                End If
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur : " & ex.ToString)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Commande">La commande à passer</param>
    ''' <param name="Parametre1">La valeur à passer</param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        'Parametre1 = data1
        'Parametre2 = data2
        Dim sendtwice As Boolean = False
        Try
            If _Enable = False Then Exit Sub
            If _IsConnect = False Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "ModbusTCP Write", "Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "ModbusTCP Write", "Ecriture de " & Objet.Name)
            If Parametre1 Is Nothing Then Parametre1 = 0

            Select Case Objet.modele
                Case "MX"
                    Parametre2 = 1
                Case "QX"
                    Parametre2 = 2
                Case "MW"
                    Parametre2 = 3
                Case "QW"
                    Parametre2 = 4
            End Select

            If Objet.Adresse2 <> "-1" And Objet.Adresse2 <> "" Then
                If TypeOf Objet.Value Is Integer Or TypeOf Objet.Value Is Double Then

                    Select Case Commande
                        Case "ON"
                            Parametre1 = Objet.ValueMax
                        Case "OFF"
                            Parametre1 = Objet.ValueMin
                        Case "OPEN"
                            Parametre1 = Objet.ValueMax
                        Case "CLOSE"
                            Parametre1 = Objet.ValueMin

                    End Select

                    ecrire(Objet.adresse2, Commande, Parametre1, Parametre2, sendtwice)
                End If

                If TypeOf Objet.Value Is Boolean Or Parametre2 = 1 Or Parametre2 = 2 Then
                    Parametre1 = str_to_bool(Commande)
                    ecrire(Objet.adresse2, Commande, Parametre1, Parametre2, sendtwice)
                End If
            End If
            Select Case Commande
                Case "ON", "OFF"
                    If Parametre1 = 0 Then traitement("OFF", Objet.adresse2, Commande, True) Else traitement("ON", Objet.adresse2, Commande, True)
                Case "DIM", "OUVERTURE"
                    traitement(CStr(Parametre1), Objet.adresse2, Commande, True)
                Case Else
                    traitement(Commande, Objet.adresse2, Commande, True)
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP NewDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>ajout des commandes avancées pour les devices</summary>
    ''' <remarks></remarks>
    Private Sub add_devicecommande(ByVal nom As String, ByVal description As String, ByVal nbparam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = nom
            x.DescriptionCommand = description
            x.CountParam = nbparam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP add_devicecommande", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout Libellé pour le Driver</summary>
    ''' <param name="nom">Nom du champ : HELP</param>
    ''' <param name="labelchamp">Nom à afficher : Aide</param>
    ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
    ''' <remarks></remarks>
    Private Sub Add_LibelleDriver(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
        Try
            Dim y0 As New HoMIDom.HoMIDom.Driver.cLabels
            y0.LabelChamp = Labelchamp
            y0.NomChamp = UCase(Nom)
            y0.Tooltip = Tooltip
            y0.Parametre = Parametre
            _LabelsDriver.Add(y0)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Ajout Libellé pour les Devices</summary>
    ''' <param name="nom">Nom du champ : HELP</param>
    ''' <param name="labelchamp">Nom à afficher : Aide, si = "@" alors le champ ne sera pas affiché</param>
    ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
    ''' <remarks></remarks>
    Private Sub Add_LibelleDevice(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
        Try
            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            ld0.LabelChamp = Labelchamp
            ld0.NomChamp = UCase(Nom)
            ld0.Tooltip = Tooltip
            ld0.Parametre = Parametre
            _LabelsDevice.Add(ld0)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout de parametre avancés</summary>
    ''' <param name="nom">Nom du parametre (sans espace)</param>
    ''' <param name="description">Description du parametre</param>
    ''' <param name="valeur">Sa valeur</param>
    ''' <remarks></remarks>
    Private Sub add_paramavance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
        Try
            Dim x As New HoMIDom.HoMIDom.Driver.Parametre
            x.Nom = nom
            x.Description = description
            x.Valeur = valeur
            _Parametres.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP add_devicecommande", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.BAROMETRE.ToString)
            _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
            _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT.ToString)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
            _DeviceSupport.Add(ListeDevices.ENERGIETOTALE.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIECOURANT.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIETOTAL.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.TELECOMMANDE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)
            _DeviceSupport.Add(ListeDevices.UV.ToString)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT.ToString)
            _DeviceSupport.Add(ListeDevices.VOLET.ToString)
			
            'Parametres avancés
            add_paramavance("Rafraichissement de lecture", "le temps en millisecondes entre les demandes de lecture", 500)
            add_paramavance("Premier bit interne de lecture", "Adresse du premier bit interne MX à lire dans l'automate", 0)
            add_paramavance("Nombre de bits interne à lire", "Nombre de bits interne MX à lire dans l'automate", 0)
            add_paramavance("Premier mot interne de lecture", "Adresse du premier mot interne MW à lire dans l'automate", 256)
            add_paramavance("Nombre de mots interne à lire", "Nombre de mots interne MW à lire dans l'automate", 225)
            add_paramavance("Premier mot d'entrée de lecture", "Adresse du premier mot d'entrée IW à lire dans l'automate", 0)
            add_paramavance("Nombre de mots d'entrée à lire", "Nombre de mots d'entrée IW à lire dans l'automate", 0)
            add_paramavance("Premier bit interne d'ecriture", "Adresse du premier bit interne MX à écrire dans l'automate", 0)
            add_paramavance("Premier mot interne d'ecriture", "Adresse du premier mot interne MW à écrire dans l'automate", 700)
            add_paramavance("Numéro Unit", "Numéro d'identification de l'unité a accéder", 0)
            add_paramavance("Debug", "Activer le Debug complet (True/False)", False)

            'ajout des commandes avancées pour les devices
            add_devicecommande("OFF", "Eteint tous les appareils du meme range que ce device", 0)
            add_devicecommande("ON", "Allume toutes les lampes du meme range que ce device", 0)
            add_devicecommande("DIM", "Variation, parametre = Variation", 1)
            add_devicecommande("CLOSE", "Eteint tous les appareils du meme range que ce device", 0)
            add_devicecommande("OPEN", "Allume toutes les lampes du meme range que ce device", 0)
            add_devicecommande("OUVERTURE", "Variation, parametre = Variation", 1)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")
            Add_LibelleDriver("MODELE", "@", "")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse Lecture", "Adresse de lecture du composant (dans la plage de lecture declaré dans le driver) ou -1 si pas de lecture", "-1")
            Add_LibelleDevice("ADRESSE2", "Adresse Ecriture", "Adresse d'écriture du composant ou -1 si pas de écriture", "-1")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("REFRESH", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "@", "")
            Add_LibelleDevice("MODELE", "Type de variable", "Type de variable à utiliser : MW/IW(lecture seul)/QW(ecriture seul)/MX/IX(lecture seul)/QX(ecriture seul)", "MX|IX|QX|MW|IW|QW")


            'dictionnaire Commande STR -> INT
            str_to_bool.Add("OFF", 0)
            str_to_bool.Add("ON", 1)

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP New", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles MyTimer.Elapsed
        Dim StartAddress As UShort = 0
        Dim Length As UShort = 0
        If _Enable = False Then Exit Sub

        Try
            If Not breading Then
				If Not flagwrite Then
                    cptsend += 1

                    Select Case typeRead
                       

                        Case "MW"
                            If nbsendMW > 0 And cptsend <= nbsendMW Then

                                breading = True
                                StartAddress = ReadStartAdr(offsetModele + adressReadMW + ((cptsend - 1) * 75)) '%MW0 = 12288 
                                Length = ReadStartAdr(75)
                                rID = (Rnd() * 256)
                                MBmaster.ReadHoldingRegister(unit, rID, StartAddress, Length)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "ModbusTCP Read", "Demande de lecture MW" & StartAddress & " , Envoi n°" & cptsend & "/" & nbsendMW)

                            Else
                                typeRead = "IW"
                                cptsend = 0
                                breading = False
                            End If
                       
                        Case "IW"
                            If nbsendIW > 0 And cptsend <= nbsendIW Then

                                breading = True
                                StartAddress = ReadStartAdr(adressReadIW + ((cptsend - 1) * 75))
                                Length = ReadStartAdr(75)
                                rID = (Rnd() * 256)
                                MBmaster.ReadHoldingRegister(unit, rID, StartAddress, Length)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "ModbusTCP Read", "Demande de lecture IW" & StartAddress & " , Envoi n°" & cptsend & "/" & nbsendIW)

                            Else
                                typeRead = "MX"
                                cptsend = 0
                                breading = False
                            End If

                        Case "MX"
                            If nbsendMX > 0 And cptsend <= nbsendMX Then

                                breading = True
                                StartAddress = ReadStartAdr(offsetModele + adressReadMX + ((cptsend - 1) * 512)) '%MX0.0 = 12288
                                Length = ReadStartAdr(512)
                                rID = (Rnd() * 256)
                                MBmaster.ReadDiscreteInputs(unit, rID, StartAddress, Length)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "ModbusTCP Read", "Demande de lecture MX" & StartAddress & " , Envoi n°" & cptsend & "/" & nbsendMX)

                            Else
                                typeRead = "IX"
                                cptsend = 0
                                breading = False
                            End If

                        Case "IX"

                            breading = True
                           
                            If rcvIX Then
                                rcvIX = False
                                typeRead = "MW"
                                cptsend = 0
                                breading = False
                            Else
                                StartAddress = ReadStartAdr(0)
                                Length = ReadStartAdr(512) '64 octet
                                rID = (Rnd() * 256)
                                MBmaster.ReadDiscreteInputs(unit, rID, StartAddress, Length)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "ModbusTCP Read", "Demande de lecture IX" & StartAddress)
                            End If

                    End Select
                Else
                    flagWrite = False
                End If
                cptWaitReponse = 0
            Else
                If cptWaitReponse = 3 Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP Read", "Time Out Reponse")

                    cptWaitReponse = 0
                    breading = False
                Else
                    cptWaitReponse += 1
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP Read " & " startadress=" & StartAddress & " et length=" & Length, ex.Message)
        End Try

    End Sub

    Sub DeviceChange(ByVal DeviceID, ByVal valeurString)
		Try
        Dim genericDevice As templateDevice = _Server.ReturnDeviceById(_IdSrv, DeviceID)
            If genericDevice.DriverID = _ID Then
                If TypeOf genericDevice.Value Is Double Or TypeOf genericDevice.Value Is Integer Then
                    Write(genericDevice, "", CInt(valeurString))
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP Device Change", ex.Message)
        End Try
    End Sub

#End Region

#Region "Fonctions internes"

    ''' <summary>Ouvrir le port ModbusTCP</summary>
    ''' <param name="Port">Nom/Numero du port</param>
    ''' <remarks></remarks>
    Private Function ouvrir(ByVal AdressIP As String, ByVal Port As String) As String
        Try
            'ouverture du port
            If Not _IsConnect Then

                ' Create new modbus master and add event functions
                MBmaster = New ModbusTCP.Master(AdressIP, CUShort(Port))

                Return ("Connecté à l'adresse:" & AdressIP)
            Else
                Return ("Echec de connexion Modbus à l'adresse:" & AdressIP)
            End If

        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try
    End Function

    ''' <summary>Fermer le port ModbusTCP</summary>
    ''' <remarks></remarks>
    Private Function fermer() As String
        Try
            If _IsConnect Then
                If MBmaster IsNot Nothing Then
                    MBmaster.disconnect()
                    MBmaster.Dispose()
                    MBmaster = Nothing
                End If
                Return ("Déconnecté")
            End If
        Catch ex As UnauthorizedAccessException
            Return ("ERR: Port 502 IGNORE")
            ' The port may have been removed. Ignore.
        End Try
        Return "ERR: Not defined"
    End Function

    ''' <summary>Pause pour attendre x msecondes </summary>
    ''' <remarks></remarks>
    Private Sub wait(ByVal msec As Integer)
        '100msec = 1 secondes
        Try
            Dim ticks = Date.Now.Ticks + (msec * 100000) '10000000 = 1 secondes
            Dim limite = 0
            While limite = 0
                If ticks <= Date.Now.Ticks Then limite = 1
            End While
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP Wait", "Exception : " & ex.Message)
        End Try
    End Sub
	
    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As String, ByVal adresse As String, ByVal commande As String, ByVal erreursidevicepastrouve As Boolean)
        If valeur <> "" Then
            Try

                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, "", Me._ID, True)
                If IsNothing(listedevices) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Process", "Communication impossible avec le serveur, l'IDsrv est peut être erroné : " & _IdSrv)
                    Exit Sub
                End If
                'un device trouvé on maj la value
                If (listedevices.Count = 1) Then
                    'correction valeur pour correspondre au type de value
                    If TypeOf listedevices.Item(0).Value Is Integer Then
                        If valeur = "ON" Then
                            listedevices.Item(0).Value = listedevices.Item(0).ValueMax
                        ElseIf valeur = "OFF" Then
                            listedevices.Item(0).Value = listedevices.Item(0).ValueMin
                        Else
                            listedevices.Item(0).Value = valeur
                        End If
                    ElseIf TypeOf listedevices.Item(0).Value Is Boolean Then
                        If valeur = "ON" Then
                            listedevices.Item(0).Value = True
                        ElseIf valeur = "OFF" Then
                            listedevices.Item(0).Value = False
                        Else
                            listedevices.Item(0).Value = True
                        End If
                    Else
                        listedevices.Item(0).Value = valeur
                    End If
                ElseIf (listedevices.Count > 1) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Process", "Plusieurs devices correspondent à : " & adresse & ":" & valeur)
                Else                    
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Process", "Device non trouvé : " & adresse & ":" & valeur)

                    'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)

                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & commande & "-" & valeur)
            End Try
        End If
    End Sub
	
    ''' <summary>Ecrire sur le port ModbusTCP</summary>
    ''' <param name="adresse">Adresse du device : A1...</param>
    ''' <param name="commande">commande à envoyer : ON, OFF...</param>
    ''' <param name="data1">voir description des actions plus haut ou doc ModbusTCP</param>
    ''' <param name="data2">voir description des actions plus haut ou doc ModbusTCP</param>
    ''' <param name="ecriretwice">Booleen : Ecrire l'ordre deux fois</param>
    ''' <remarks></remarks>

    Private Function ecrire(ByVal adresse As String, ByVal commande As String, Optional ByVal data1 As Integer = 0, Optional ByVal data2 As Integer = 0, Optional ByVal ecriretwice As Boolean = False) As String

        Try
            If _IsConnect Then

                Dim StartAddress As UShort
                Dim DataM(1) As UInteger
                Dim DataB As Boolean
                Dim Data() As Byte

                Try
                    If data2 = 3 Then
                        StartAddress = ReadStartAdr(CInt(adresse) + adressWriteMW + offsetModele - 1)
                    End If
                    If data2 = 1 Then
                        StartAddress = ReadStartAdr(CInt(adresse) + adressWriteMX + offsetModele - 1)
                    End If
                    If data2 = 2 Or data2 = 4 Then
                        StartAddress = ReadStartAdr(CInt(adresse) - 1)
                    End If

                    If data2 = 3 Or data2 = 4 Then 'MW ou QW
                        DataM(0) = data1
                        Data = GetData(DataM)
                        MBmaster.WriteSingleRegister(unit, 16, StartAddress, Data)
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Modbus slave receive", "MBmaster_WriteSingleRegister : " & "Debut MW ou QW" & StartAddress & " a la valeur " & data1)

                    End If
                    If data2 = 1 Or data2 = 2 Then 'MX ou QX
                        DataB = CBool(data1)
                        MBmaster.WriteSingleCoils(unit, 16, StartAddress, DataB)
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Modbus slave receive", "MBmaster_WriteSingleCoils : " & "Debut MX ou QX" & StartAddress & " a la valeur " & CBool(data1))

                    End If
                    flagWrite = True
                    
                Catch ex As Exception
                    Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP", "ModbusTCP Ecrire startadress=" & StartAddress)
                End Try

                'renvoie la valeur ecrite
                Return "VALUE"

            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP Ecrire", "Port Fermé, impossible d ecrire : " & adresse & " : " & commande & " " & data1 & "-" & data2)
                Return ""
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ModbusTCP Ecrire", "exception : " & ex.Message)
            Return "" 'on renvoi rien car il y a eu une erreur
        End Try
    End Function

#End Region

#Region "ModbusTCP"

    ' ------------------------------------------------------------------------
    ' Event for response data
    ' ------------------------------------------------------------------------
    Private Sub MBmaster_OnResponseData(ByVal ID As UShort, ByVal [function] As Byte, ByVal values As Byte()) Handles MBmaster.OnResponseData

        Dim msg As String = ""
        Dim adresse As Integer = 0
        ' ------------------------------------------------------------------
        Try

            ' Ignore watchdog response data
            If ID = &HFF Then
                Return
            End If

            ' ------------------------------------------------------------------------
            ' Identify requested data

            msg = "ID = " & ID & " et Fonction = " & [function]

            If ID = rid Then


                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, "", "", Me._ID, True)

                If typeRead = "MW" Then
                    dataMdbMW = ShowAsW(values)
                    msg += " Debut MW" & adressReadMW + (75 * (cptsend - 1)) & " et data = "
                End If

                If typeRead = "IW" Then
                    dataMdbIW = ShowAsW(values)
                    msg += " Debut IW" & adressReadIW + (75 * (cptsend - 1)) & " et data = "
                End If

                If typeRead = "MX" Then
                    dataMdbMX = ShowAsB(values)
                    msg += " Debut MX" & adressReadMX + (512 * (cptsend - 1)) & ".0 et data = "
                    For i = 0 To values.Length - 1
                        msg += CStr(values(i)) & "; "
                    Next
                End If

                If typeRead = "IX" Then
                    dataMdbIX = ShowAsB(values)
                    msg += " Debut IX0.0 et data = "
                    For i = 0 To values.Length - 1
                        msg += CStr(values(i)) & "; "
                    Next
                    rcvIX = True
                End If

                For Each j As Object In listedevices
                    If j.Adresse1 <> "-1" And j.Adresse1 <> "" Then

                        If j.modele = typeRead And j.Refresh = 0 Then

                            If j.modele = "MW" Then
                                adresse = j.adresse1 - ((cptsend - 1) * 75) - 1
                                If j.adresse1 > 75 * (cptsend - 1) And j.adresse1 <= 75 * cptsend Then
                                    msg += j.adresse1 & "=" & dataMdbMW(adresse) & " / " & j.Value & " ;"
                                    If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                        j.Value = dataMdbMW(adresse)
                                    End If
                                    If TypeOf j.Value Is Boolean And dataMdbMW(adresse) > -1 And dataMdbMW(adresse) < 2 Then
                                        j.Value = CBool(dataMdbMW(adresse))
                                    End If
                                End If
                            End If

                            If j.modele = "IW" Then
                                If j.adresse1 > 75 * (cptsend - 1) And j.adresse1 <= 75 * cptsend Then
                                    msg += j.adresse1 & "=" & dataMdbIW(j.adresse1) & " / " & j.Value & " ;"
                                    If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                        j.Value = dataMdbIW(j.adresse1)
                                    End If
                                    If TypeOf j.Value Is Boolean And dataMdbIW(j.adresse1) > -1 And dataMdbIW(j.adresse1) < 2 Then
                                        j.Value = CBool(dataMdbIW(j.adresse1))
                                    End If
                                End If
                            End If

                            If TypeOf j.Value Is Boolean Then
                                If j.modele = "MX" Then
                                    adresse = j.adresse1 - ((cptsend - 1) * 512) - 1
                                    If j.adresse1 > 512 * (cptsend - 1) And j.adresse1 <= 512 * cptsend Then
                                        msg += j.adresse1 & "=" & dataMdbMX(adresse) & " / " & j.Value & " ; "
                                        j.Value = dataMdbMX(adresse)
                                    End If
                                End If
                                If j.modele = "IX" Then
                                    If j.adresse1 > 0 And j.adresse1 <= 512 Then
                                        msg += j.adresse1 & "=" & dataMdbIX(j.adresse1) & " / " & j.Value & " ; "
                                        j.Value = dataMdbIX(j.adresse1)
                                    End If
                                End If
                            End If

                        End If

                    End If
                Next
                MemID = ID
            ElseIf ID = MemID Then
                [Stop]()
                Threading.Thread.Sleep(2000)
                Start()
            End If
            breading = False
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Modbus slave receive", "MBmaster_OnResponseData : " & msg)

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Modbus slave exception", "MBmaster_OnResponseData : " & msg)
            breading = False
        End Try
    End Sub

    ' ------------------------------------------------------------------------
    ' Modbus TCP slave exception
    ' ------------------------------------------------------------------------
    Private Sub MBmaster_OnException(ByVal id As UShort, ByVal [function] As Byte, ByVal exception As Byte) Handles MBmaster.OnException
        ' ------------------------------------------------------------------

        Dim exc As String = "Modbus says error: "
        Select Case exception
            Case ModbusTCP.Master.excIllegalFunction
                exc += "Illegal function!"
                Exit Select
            Case ModbusTCP.Master.excIllegalDataAdr
                exc += "Illegal data adress!"
                Exit Select
            Case ModbusTCP.Master.excIllegalDataVal
                exc += "Illegal data value!"
                Exit Select
            Case ModbusTCP.Master.excSlaveDeviceFailure
                exc += "Slave device failure!"
                Exit Select
            Case ModbusTCP.Master.excAck
                exc += "Acknoledge!"
                Exit Select
            Case ModbusTCP.Master.excSlaveIsBusy
                exc += "Slave is busy!"
                Exit Select
            Case ModbusTCP.Master.excGatePathUnavailable
                exc += "Gateway path unavailbale!"
                Exit Select
            Case ModbusTCP.Master.excExceptionTimeout
                exc += "Slave timed out!"
                Exit Select
            Case ModbusTCP.Master.excExceptionConnectionLost
                exc += "Connection is lost!"
                Exit Select
            Case ModbusTCP.Master.excExceptionNotConnected
                exc += "Not connected!"
                Exit Select
        End Select

        _Server.Log(TypeLog.MESSAGE, TypeSource.DRIVER, "Modbus slave exception", exc)
        Restart()
    End Sub

    ' ------------------------------------------------------------------------
    ' Read start address
    ' ------------------------------------------------------------------------
    Private Function ReadStartAdr(ByVal StartAdress As Integer) As UShort
        Try
            ' Convert hex numbers into decimal
            If CStr(StartAdress).IndexOf("0x", 0, CStr(StartAdress).Length) = 0 Then
                Dim str As String = CStr(StartAdress).Replace("0x", "")
                Dim hex As UShort = Convert.ToUInt16(str, 16)
                Return hex
            Else
                Return Convert.ToUInt16(CStr(StartAdress))
            End If
        Catch ex As Exception
            Return Convert.ToUInt16(CStr(0))
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Modbus slave exception", "ReadStartAdr")
        End Try
    End Function

    ' ------------------------------------------------------------------------
    ' Read values from textboxes
    ' ------------------------------------------------------------------------
    Private Function GetData(ByVal dataE() As UInteger) As Byte()

        Dim num As Integer = dataE.Length - 1
        Dim data As Byte() = New [Byte](num - 1) {}
        Dim word As Integer() = New Integer(num - 1) {}

        Try
            Debug.Write("Modbus GetData : " & num.ToString)
            ' ------------------------------------------------------------------------
            ' Convert data from text boxes
            For x As Integer = 0 To num - 1
                If Not (dataE(x) = 0) Then
                    word(x) = CInt(dataE(x))

                End If
            Next
            data = New [Byte](num * 2 - 1) {}
            For x As Integer = 0 To num - 1
                Dim dat As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(word(x)))))
                data(x * 2) = dat(0)
                data(x * 2 + 1) = dat(1)
            Next

            Return data
        Catch ex As Exception
            Return data
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Modbus slave exception", "GetData")
        End Try
    End Function

    ' ------------------------------------------------------------------------
    ' Show values in selected way
    ' ------------------------------------------------------------------------
    Private Function ShowAsW(ByVal dataR() As Byte) As UInt16()

        Dim word As UInt16() = New UInt16(0) {}
        Try
            ' Convert data to selected data type
            If dataR.Length < 2 Then
                Return Nothing
                Exit Function
            End If
            word = New UInt16(CInt(dataR.Length / 2 - 1)) {}
            Dim x As Integer = 0
            While x < dataR.Length
                word(CInt(x / 2)) = CType(dataR(x) * 256 + dataR(x + 1), UInt16)
                x = x + 2
            End While

            Return word
        Catch ex As Exception
            Return word
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Modbus slave exception", "ShowAs")
        End Try
    End Function

    Private Function ShowAsB(ByVal dataR() As Byte) As Boolean()

        Dim bool As Boolean() = New Boolean(0) {}
        Try
            ' Convert data to selected data type
            If dataR.Length < 1 Then
                Return Nothing
                Exit Function
            End If
            bool = New Boolean(CInt(dataR.Length * 8 - 1)) {}
            Dim x As Integer = 0

            While x < dataR.Length
                bool(CInt(x * 8)) = CType(dataR(x) And Hex(("&h" & 1)), Boolean)
                bool(CInt(x * 8) + 1) = CType(dataR(x) And Hex(("&h" & 2)), Boolean)
                bool(CInt(x * 8) + 2) = CType(dataR(x) And Hex(("&h" & 4)), Boolean)
                bool(CInt(x * 8) + 3) = CType(dataR(x) And Hex(("&h" & 8)), Boolean)
                bool(CInt(x * 8) + 4) = CType(dataR(x) And Hex(("&h" & 16)), Boolean)
                bool(CInt(x * 8) + 5) = CType(dataR(x) And Hex(("&h" & 32)), Boolean)
                bool(CInt(x * 8) + 6) = CType(dataR(x) And Hex(("&h" & 64)), Boolean)
                bool(CInt(x * 8) + 7) = CType(dataR(x) And Hex(("&h" & 128)), Boolean)
                x = x + 1
            End While

            Return bool
        Catch ex As Exception
            Return bool
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Modbus slave exception", "ShowAs")
        End Try
    End Function

#End Region

End Class


Namespace ModbusTCP
    ''' <summary>
    ''' Modbus TCP common driver class. This class implements a modbus TCP master driver.
    ''' It supports the following commands:
    ''' 
    ''' Read coils
    ''' Read discrete inputs
    ''' Write single coil
    ''' Write multiple cooils
    ''' Read holding register
    ''' Read input register
    ''' Write single register
    ''' Write multiple register
    ''' 
    ''' All commands can be sent in synchronous or asynchronous mode. If a value is accessed
    ''' in synchronous mode the program will stop and wait for slave to response. If the 
    ''' slave didn't answer within a specified time a timeout exception is called.
    ''' The class uses multi threading for both synchronous and asynchronous access. For
    ''' the communication two lines are created. This is necessary because the synchronous
    ''' thread has to wait for a previous command to finish.
    ''' 
    ''' </summary>
    Public Class Master
        ' ------------------------------------------------------------------------
        ' Constants for access
        Private Const fctReadCoil As Byte = 1
        Private Const fctReadDiscreteInputs As Byte = 2
        Private Const fctReadHoldingRegister As Byte = 3
        Private Const fctReadInputRegister As Byte = 4
        Private Const fctWriteSingleCoil As Byte = 5
        Private Const fctWriteSingleRegister As Byte = 6
        Private Const fctWriteMultipleCoils As Byte = 15
        Private Const fctWriteMultipleRegister As Byte = 16
        Private Const fctReadWriteMultipleRegister As Byte = 23

        ''' <summary>Constant for exception illegal function.</summary>
        Public Const excIllegalFunction As Byte = 1
        ''' <summary>Constant for exception illegal data address.</summary>
        Public Const excIllegalDataAdr As Byte = 2
        ''' <summary>Constant for exception illegal data value.</summary>
        Public Const excIllegalDataVal As Byte = 3
        ''' <summary>Constant for exception slave device failure.</summary>
        Public Const excSlaveDeviceFailure As Byte = 4
        ''' <summary>Constant for exception acknowledge.</summary>
        Public Const excAck As Byte = 5
        ''' <summary>Constant for exception slave is busy/booting up.</summary>
        Public Const excSlaveIsBusy As Byte = 6
        ''' <summary>Constant for exception gate path unavailable.</summary>
        Public Const excGatePathUnavailable As Byte = 10
        ''' <summary>Constant for exception not connected.</summary>
        Public Const excExceptionNotConnected As Byte = 253
        ''' <summary>Constant for exception connection lost.</summary>
        Public Const excExceptionConnectionLost As Byte = 254
        ''' <summary>Constant for exception response timeout.</summary>
        Public Const excExceptionTimeout As Byte = 255
        ''' <summary>Constant for exception wrong offset.</summary>
        Private Const excExceptionOffset As Byte = 128
        ''' <summary>Constant for exception send failt.</summary>
        Private Const excSendFailt As Byte = 100

        ' ------------------------------------------------------------------------
        ' Private declarations
        Private Shared _timeout As UShort = 5000
        Private Shared _connected As Boolean = False

        Private tcpAsyCl As Sockets.Socket
        Private tcpAsyClBuffer As Byte() = New Byte(2047) {}

        Private tcpSynCl As Sockets.Socket
        Private tcpSynClBuffer As Byte() = New Byte(2047) {}

        ' ------------------------------------------------------------------------
        ''' <summary>Response data event. This event is called when new data arrives</summary>
        Public Delegate Sub ResponseData(ByVal id As UShort, ByVal [function] As Byte, ByVal data As Byte())
        ''' <summary>Response data event. This event is called when new data arrives</summary>
        Public Event OnResponseData As ResponseData
        ''' <summary>Exception data event. This event is called when the data is incorrect</summary>
        Public Delegate Sub ExceptionData(ByVal id As UShort, ByVal [function] As Byte, ByVal exception As Byte)
        ''' <summary>Exception data event. This event is called when the data is incorrect</summary>
        Public Event OnException As ExceptionData

        Public Event statConnect(ByVal Connect As Boolean)
        ' ------------------------------------------------------------------------
        ''' <summary>Response timeout. If the slave didn't answers within in this time an exception is called.</summary>
        ''' <value>The default value is 500ms.</value>
        Public Property timeout() As UShort
            Get
                Return _timeout
            End Get
            Set(ByVal value As UShort)
                _timeout = value
            End Set
        End Property

        ' ------------------------------------------------------------------------
        ''' <summary>Shows if a connection is active.</summary>
        Public ReadOnly Property connected() As Boolean
            Get
                Return _connected
            End Get
        End Property

        ' ------------------------------------------------------------------------
        ''' <summary>Create master instance without parameters.</summary>
        Public Sub New()
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Create master instance with parameters.</summary>
        ''' <param name="ip">IP adress of modbus slave.</param>
        ''' <param name="port">Port number of modbus slave. Usually port 502 is used.</param>
        Public Sub New(ByVal ip As String, ByVal port As UShort)
            connect(ip, port)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Start connection to slave.</summary>
        ''' <param name="ip">IP adress of modbus slave.</param>
        ''' <param name="port">Port number of modbus slave. Usually port 502 is used.</param>
        Public Sub connect(ByVal ip As String, ByVal port As UShort)
            Try
                Dim _ip As IPAddress = Nothing
                If IPAddress.TryParse(ip, _ip) = False Then
                    Dim hst As IPHostEntry = Dns.GetHostEntry(ip)
                    ip = hst.AddressList(0).ToString()
                End If
                ' ----------------------------------------------------------------
                ' Connect asynchronous client
                tcpAsyCl = New Sockets.Socket(IPAddress.Parse(ip).AddressFamily, Sockets.SocketType.Stream, Sockets.ProtocolType.Tcp)

                tcpAsyCl.Connect(New IPEndPoint(IPAddress.Parse(ip), port))
                tcpAsyCl.SetSocketOption(Sockets.SocketOptionLevel.Socket, Sockets.SocketOptionName.SendTimeout, _timeout)
                tcpAsyCl.SetSocketOption(Sockets.SocketOptionLevel.Socket, Sockets.SocketOptionName.ReceiveTimeout, _timeout)
                tcpAsyCl.SetSocketOption(Sockets.SocketOptionLevel.Socket, Sockets.SocketOptionName.NoDelay, 1)


                ' ----------------------------------------------------------------
                ' Connect synchronous client
                tcpSynCl = New Sockets.Socket(IPAddress.Parse(ip).AddressFamily, Sockets.SocketType.Stream, Sockets.ProtocolType.Tcp)
                tcpSynCl.Connect(New IPEndPoint(IPAddress.Parse(ip), port))
                tcpSynCl.SetSocketOption(Sockets.SocketOptionLevel.Socket, Sockets.SocketOptionName.SendTimeout, _timeout)
                tcpSynCl.SetSocketOption(Sockets.SocketOptionLevel.Socket, Sockets.SocketOptionName.ReceiveTimeout, _timeout)
                tcpSynCl.SetSocketOption(Sockets.SocketOptionLevel.Socket, Sockets.SocketOptionName.NoDelay, 1)
                _connected = True
                RaiseEvent statConnect(True)
            Catch [error] As System.IO.IOException
                _connected = False
                Throw ([error])
            End Try
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Stop connection to slave.</summary>
        Public Sub disconnect()
            Dispose()
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Destroy master instance.</summary>
        Protected Overrides Sub Finalize()
            Try
                Dispose()
            Finally
                MyBase.Finalize()
            End Try
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Destroy master instance</summary>
        Public Sub Dispose()
            If tcpAsyCl IsNot Nothing Then
                If tcpAsyCl.Connected Then
                    Try
                        tcpAsyCl.Shutdown(Sockets.SocketShutdown.Both)
                    Catch
                    End Try
                    tcpAsyCl.Close()
                End If
                tcpAsyCl = Nothing
            End If
            If tcpSynCl IsNot Nothing Then
                If tcpSynCl.Connected Then
                    Try
                        tcpSynCl.Shutdown(Sockets.SocketShutdown.Both)
                    Catch
                    End Try
                    tcpSynCl.Close()
                End If
                tcpSynCl = Nothing
            End If
            RaiseEvent statConnect(False)
        End Sub

        Friend Sub CallException(ByVal id As UShort, ByVal [function] As Byte, ByVal exception As Byte)
            If (tcpAsyCl Is Nothing) OrElse (tcpSynCl Is Nothing) Then
                Return
            End If
            If exception = excExceptionConnectionLost Then
                tcpSynCl = Nothing
                tcpAsyCl = Nothing
            End If
            RaiseEvent statConnect(_connected)
            RaiseEvent OnException(id, [function], exception)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read coils from slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        Public Sub ReadCoils(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numInputs As UShort)
            WriteAsyncData(CreateReadHeader(unit, id, startAddress, numInputs, fctReadCoil), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read coils from slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="values">Contains the result of function.</param>
        Public Sub ReadCoils(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numInputs As UShort, ByRef values As Byte())
            values = WriteSyncData(CreateReadHeader(unit, id, startAddress, numInputs, fctReadCoil), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read discrete inputs from slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        Public Sub ReadDiscreteInputs(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numInputs As UShort)
            WriteAsyncData(CreateReadHeader(unit, id, startAddress, numInputs, fctReadDiscreteInputs), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read discrete inputs from slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="values">Contains the result of function.</param>
        Public Sub ReadDiscreteInputs(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numInputs As UShort, ByRef values As Byte())
            values = WriteSyncData(CreateReadHeader(unit, id, startAddress, numInputs, fctReadDiscreteInputs), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read holding registers from slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        Public Sub ReadHoldingRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numInputs As UShort)
            Try
                WriteAsyncData(CreateReadHeader(unit, id, startAddress, numInputs, fctReadHoldingRegister), id)
            Catch ex As Exception
                Debug.WriteLine("erreur read class mdb startadress=" & startAddress & " et length=" & numInputs)
            End Try
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read holding registers from slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="values">Contains the result of function.</param>
        Public Sub ReadHoldingRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numInputs As UShort, ByRef values As Byte())
            values = WriteSyncData(CreateReadHeader(unit, id, startAddress, numInputs, fctReadHoldingRegister), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read input registers from slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        Public Sub ReadInputRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numInputs As UShort)
            WriteAsyncData(CreateReadHeader(unit, id, startAddress, numInputs, fctReadInputRegister), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read input registers from slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="values">Contains the result of function.</param>
        Public Sub ReadInputRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numInputs As UShort, ByRef values As Byte())
            values = WriteSyncData(CreateReadHeader(unit, id, startAddress, numInputs, fctReadInputRegister), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write single coil in slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="OnOff">Specifys if the coil should be switched on or off.</param>
        Public Sub WriteSingleCoils(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal OnOff As Boolean)
            Dim data As Byte()
            data = CreateWriteHeader(unit, id, startAddress, 1, 1, fctWriteSingleCoil)
            If OnOff = True Then
                data(10) = 255
            Else
                data(10) = 0
            End If
            WriteAsyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write single coil in slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="OnOff">Specifys if the coil should be switched on or off.</param>
        ''' <param name="result">Contains the result of the synchronous write.</param>
        Public Sub WriteSingleCoils(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal OnOff As Boolean, ByRef result As Byte())
            Dim data As Byte()
            data = CreateWriteHeader(unit, id, startAddress, 1, 1, fctWriteSingleCoil)
            If OnOff = True Then
                data(10) = 255
            Else
                data(10) = 0
            End If
            result = WriteSyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write multiple coils in slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numBits">Specifys number of bits.</param>
        ''' <param name="values">Contains the bit information in byte format.</param>
        Public Sub WriteMultipleCoils(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numBits As UShort, ByVal values As Byte())
            Dim numBytes As Byte = Convert.ToByte(values.Length)
            Dim data As Byte()
            data = CreateWriteHeader(unit, id, startAddress, numBits, CByte(numBytes + 2), fctWriteMultipleCoils)
            Array.Copy(values, 0, data, 13, numBytes)
            WriteAsyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write multiple coils in slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numBits">Specifys number of bits.</param>
        ''' <param name="values">Contains the bit information in byte format.</param>
        ''' <param name="result">Contains the result of the synchronous write.</param>
        Public Sub WriteMultipleCoils(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numBits As UShort, ByVal values As Byte(), ByRef result As Byte())
            Dim numBytes As Byte = Convert.ToByte(values.Length)
            Dim data As Byte()
            data = CreateWriteHeader(unit, id, startAddress, numBits, CByte(numBytes + 2), fctWriteMultipleCoils)
            Array.Copy(values, 0, data, 13, numBytes)
            result = WriteSyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write single register in slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        Public Sub WriteSingleRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal values As Byte())
            Dim data As Byte()
            data = CreateWriteHeader(unit, id, startAddress, 1, 1, fctWriteSingleRegister)
            data(10) = values(0)
            data(11) = values(1)
            WriteAsyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write single register in slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        ''' <param name="result">Contains the result of the synchronous write.</param>
        Public Sub WriteSingleRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal values As Byte(), ByRef result As Byte())
            Dim data As Byte()
            data = CreateWriteHeader(unit, id, startAddress, 1, 1, fctWriteSingleRegister)
            data(10) = values(0)
            data(11) = values(1)
            result = WriteSyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write multiple registers in slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        Public Sub WriteMultipleRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal values As Byte())
            Dim numBytes As UShort = Convert.ToUInt16(values.Length)
            If numBytes Mod 2 > 0 Then
                numBytes += CUShort(1)
            End If
            Dim data As Byte()

            data = CreateWriteHeader(unit, id, startAddress, Convert.ToUInt16(numBytes / 2), Convert.ToUInt16(numBytes + 2), fctWriteMultipleRegister)
            Array.Copy(values, 0, data, 13, values.Length)
            WriteAsyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write multiple registers in slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        ''' <param name="result">Contains the result of the synchronous write.</param>
        Public Sub WriteMultipleRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal values As Byte(), ByRef result As Byte())
            Dim numBytes As UShort = Convert.ToUInt16(values.Length)
            If numBytes Mod 2 > 0 Then
                numBytes += CUShort(1)
            End If
            Dim data As Byte()

            data = CreateWriteHeader(unit, id, startAddress, Convert.ToUInt16(numBytes / 2), Convert.ToUInt16(numBytes + 2), fctWriteMultipleRegister)
            Array.Copy(values, 0, data, 13, values.Length)
            result = WriteSyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read/Write multiple registers in slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startReadAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="startWriteAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        Public Sub ReadWriteMultipleRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startReadAddress As UShort, ByVal numInputs As UShort, ByVal startWriteAddress As UShort, ByVal values As Byte())
            Dim numBytes As UShort = Convert.ToUInt16(values.Length)
            If numBytes Mod 2 > 0 Then
                numBytes += CUShort(1)
            End If
            Dim data As Byte()

            data = CreateReadWriteHeader(unit, id, startReadAddress, numInputs, startWriteAddress, Convert.ToUInt16(numBytes / 2))
            Array.Copy(values, 0, data, 17, values.Length)
            WriteAsyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read/Write multiple registers in slave synchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="startReadAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="startWriteAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        ''' <param name="result">Contains the result of the synchronous command.</param>
        Public Sub ReadWriteMultipleRegister(ByVal unit As Byte, ByVal id As UShort, ByVal startReadAddress As UShort, ByVal numInputs As UShort, ByVal startWriteAddress As UShort, ByVal values As Byte(), ByRef result As Byte())
            Dim numBytes As UShort = Convert.ToUInt16(values.Length)
            If numBytes Mod 2 > 0 Then
                numBytes += CUShort(1)
            End If
            Dim data As Byte()

            data = CreateReadWriteHeader(unit, id, startReadAddress, numInputs, startWriteAddress, Convert.ToUInt16(numBytes / 2))
            Array.Copy(values, 0, data, 17, values.Length)
            result = WriteSyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ' Create modbus header for read action
        Private Function CreateReadHeader(ByVal unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal length As UShort, ByVal [function] As Byte) As Byte()
            Dim data As Byte() = New Byte(11) {}

            Dim _id As Byte() = BitConverter.GetBytes(CShort(id))
            data(0) = _id(0)
            ' Slave id high byte
            data(1) = _id(1)
            ' Slave id low byte
            data(5) = 6
            ' Message size
            data(6) = unit
            ' Slave address
            data(7) = [function]
            ' Function code
            Dim _adr As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(startAddress))))
            data(8) = _adr(0)
            ' Start address
            data(9) = _adr(1)
            ' Start address
            Dim _length As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(length))))
            data(10) = _length(0)
            ' Number of data to read
            data(11) = _length(1)
            ' Number of data to read
            Return data
        End Function

        ' ------------------------------------------------------------------------
        ' Create modbus header for write action
        Private Function CreateWriteHeader(ByVal Unit As Byte, ByVal id As UShort, ByVal startAddress As UShort, ByVal numData As UShort, ByVal numBytes As UShort, ByVal [function] As Byte) As Byte()
            Dim data As Byte() = New Byte(numBytes + 10) {}

            Dim _id As Byte() = BitConverter.GetBytes(CShort(id))
            data(0) = _id(0)
            ' Slave id high byte
            data(1) = _id(1)
            ' Slave id low byte+
            Dim _size As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(5 + numBytes))))
            data(4) = _size(0)
            ' Complete message size in bytes
            data(5) = _size(1)
            ' Complete message size in bytes
            data(6) = Unit
            ' Slave address
            data(7) = [function]
            ' Function code
            Dim _adr As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(startAddress))))
            data(8) = _adr(0)
            ' Start address
            data(9) = _adr(1)
            ' Start address
            If [function] >= fctWriteMultipleCoils Then
                Dim _cnt As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(numData))))
                data(10) = _cnt(0)
                ' Number of bytes
                data(11) = _cnt(1)
                ' Number of bytes
                data(12) = CByte(numBytes - 2)
            End If
            Return data
        End Function

        ' ------------------------------------------------------------------------
        ' Create modbus header for write action
        Private Function CreateReadWriteHeader(ByVal Unit As Byte, ByVal id As UShort, ByVal startReadAddress As UShort, ByVal numRead As UShort, ByVal startWriteAddress As UShort, ByVal numWrite As UShort) As Byte()
            Dim data As Byte() = New Byte(numWrite * 2 + 16) {}

            Dim _id As Byte() = BitConverter.GetBytes(CShort(id))
            data(0) = _id(0)
            ' Slave id high byte
            data(1) = _id(1)
            ' Slave id low byte+
            Dim _size As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(11 + numWrite * 2))))
            data(4) = _size(0)
            ' Complete message size in bytes
            data(5) = _size(1)
            ' Complete message size in bytes
            data(6) = Unit
            ' Slave address
            data(7) = fctReadWriteMultipleRegister
            ' Function code
            Dim _adr_read As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(startReadAddress))))
            data(8) = _adr_read(0)
            ' Start read address
            data(9) = _adr_read(1)
            ' Start read address
            Dim _cnt_read As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(numRead))))
            data(10) = _cnt_read(0)
            ' Number of bytes to read
            data(11) = _cnt_read(1)
            ' Number of bytes to read
            Dim _adr_write As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(startWriteAddress))))
            data(12) = _adr_write(0)
            ' Start write address
            data(13) = _adr_write(1)
            ' Start write address
            Dim _cnt_write As Byte() = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(numWrite))))
            data(14) = _cnt_write(0)
            ' Number of bytes to write
            data(15) = _cnt_write(1)
            ' Number of bytes to write
            data(16) = CByte(numWrite * 2)

            Return data
        End Function

        ' ------------------------------------------------------------------------
        ' Write asynchronous data
        Private Sub WriteAsyncData(ByVal write_data As Byte(), ByVal id As UShort)
            If (tcpAsyCl IsNot Nothing) AndAlso (tcpAsyCl.Connected) Then
                Try
                    tcpAsyCl.BeginSend(write_data, 0, write_data.Length, Sockets.SocketFlags.None, New AsyncCallback(AddressOf OnSend), Nothing)
                    tcpAsyCl.BeginReceive(tcpAsyClBuffer, 0, tcpAsyClBuffer.Length, Sockets.SocketFlags.None, New AsyncCallback(AddressOf OnReceive), tcpAsyCl)

                Catch generatedExceptionName As SystemException
                    CallException(id, write_data(7), excExceptionConnectionLost)
                End Try
            Else
                CallException(id, write_data(7), excExceptionConnectionLost)
            End If
        End Sub

        ' ------------------------------------------------------------------------
        ' Write asynchronous data acknowledge
        Private Sub OnSend(ByVal result As System.IAsyncResult)
            If result.IsCompleted = False Then
                CallException(&HFFFF, &HFF, excSendFailt)
            End If
        End Sub

        ' ------------------------------------------------------------------------
        ' Write asynchronous data response
        Private Sub OnReceive(ByVal result As System.IAsyncResult)
            Try

                RaiseEvent statConnect(_connected)
                If result.IsCompleted = False Then
                    CallException(&HFF, &HFF, excExceptionConnectionLost)
                End If

                Dim id As UShort = BitConverter.ToUInt16(tcpAsyClBuffer, 0)

                Dim [function] As Byte = tcpAsyClBuffer(7)
                Dim data As Byte()

                ' ------------------------------------------------------------
                ' Write response data
                If ([function] >= fctWriteSingleCoil) AndAlso ([function] <> fctReadWriteMultipleRegister) Then
                    data = New Byte(1) {}
                    Array.Copy(tcpAsyClBuffer, 10, data, 0, 2)
                Else
                    ' ------------------------------------------------------------
                    ' Read response data
                    data = New Byte(tcpAsyClBuffer(8) - 1) {}
                    Array.Copy(tcpAsyClBuffer, 9, data, 0, tcpAsyClBuffer(8))
                End If
                ' ------------------------------------------------------------
                ' Response data is slave exception
                If [function] > excExceptionOffset Then
                    [function] -= excExceptionOffset
                    CallException(id, [function], tcpAsyClBuffer(8))
                    ' ------------------------------------------------------------
                    ' Response data is regular data
                Else

                    RaiseEvent OnResponseData(id, [function], data)

                End If

            Catch ex As Exception
                Debug.WriteLine("erreur on receive mdb")
            End Try
        End Sub

        ' ------------------------------------------------------------------------
        ' Write data and and wait for response
        Private Function WriteSyncData(ByVal write_data As Byte(), ByVal id As UShort) As Byte()

            If tcpSynCl.Connected Then
                Try
                    tcpSynCl.Send(write_data, 0, write_data.Length, Sockets.SocketFlags.None)
                    Dim result As Integer = tcpSynCl.Receive(tcpSynClBuffer, 0, tcpSynClBuffer.Length, Sockets.SocketFlags.None)

                    Dim [function] As Byte = tcpSynClBuffer(7)
                    Dim data As Byte()

                    If result = 0 Then
                        CallException(id, write_data(7), excExceptionConnectionLost)
                    End If

                    ' ------------------------------------------------------------
                    ' Response data is slave exception
                    If [function] > excExceptionOffset Then
                        [function] -= excExceptionOffset
                        CallException(id, [function], tcpSynClBuffer(8))
                        Return Nothing
                        ' ------------------------------------------------------------
                        ' Write response data
                    ElseIf ([function] >= fctWriteSingleCoil) AndAlso ([function] <> fctReadWriteMultipleRegister) Then
                        data = New Byte(1) {}
                        Array.Copy(tcpSynClBuffer, 10, data, 0, 2)
                    Else
                        ' ------------------------------------------------------------
                        ' Read response data
                        data = New Byte(tcpSynClBuffer(8) - 1) {}
                        Array.Copy(tcpSynClBuffer, 9, data, 0, tcpSynClBuffer(8))
                    End If
                    Return data
                Catch generatedExceptionName As SystemException
                    CallException(id, write_data(7), excExceptionConnectionLost)
                End Try
            Else
                CallException(id, write_data(7), excExceptionConnectionLost)
            End If
            Return Nothing
        End Function
       
    End Class
End Namespace