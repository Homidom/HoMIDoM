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

    Private MemAdrWrite As Integer

    Private FirstStart As Boolean = True

    Private adressReadMW As Integer
    Private longReadMW As Integer
    Private adressReadIW As Integer
    Private longReadIW As Integer
    Private adressReadMX As Integer
    Private longReadMX As Integer
    Private adressReadIX As Integer
    Private longReadIX As Integer
    Private adressReadQX As Integer
    Private longReadQX As Integer
    Private adressReadQW As Integer
    Private longReadQW As Integer
    Private adressWriteMW As Integer
    Private adressWriteMX As Integer
    Private adressWriteQW As Integer
    Private adressWriteQX As Integer
    Private unit As Integer
    Private affTrame As Boolean

    Private nbsendMW As Integer
    Private nbsendIW As Integer
    Private nbsendQW As Integer
    Private nbsendMX As Integer
    Private nbsendIX As Integer
    Private nbsendQX As Integer

    Private dataMdbRMW() As UInt16
    Private dataMdbRIW() As UInt16
    Private dataMdbRMX() As Boolean
    Private dataMdbRIX() As Boolean

    Private memdataMdbRMW() As UInt16
    Private memdataMdbRIW() As UInt16
    Private memdataMdbRMX() As Boolean
    Private memdataMdbRIX() As Boolean

    Private dataMdbWMW() As UInt16
    Private dataMdbWQW() As UInt16
    Private dataMdbWMX() As Boolean
    Private dataMdbWQX() As Boolean

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
                    If Value > longReadMX Then

                    End If
                    If Value > longReadMW Then

                    End If
                    If Value > longReadIW Then

                    End If
                    If Value > longReadIX Then

                    End If


                Case "ADRESSE2" 'Adresse Ecriture 
                    If adressWriteMX + Value > 20479 Then

                    End If
                    If adressWriteMX + Value > 12287 Then

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
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, "Driver non démarré : " & retour)
            Else
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, retour)

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
                If TypeOf _Parametres.Item(12).Valeur Is Boolean Then
                    _Parametres.Item(12).Valeur = 0
                End If

                If _Parametres.Item(2).Valeur > 20479 Then _Parametres.Item(2).Valeur = 20479
                If _Parametres.Item(1).Valeur > 20479 Then _Parametres.Item(1).Valeur = 20478
                If (CInt(_Parametres.Item(2).Valeur) + CInt(_Parametres.Item(1).Valeur)) > 20479 Then _Parametres.Item(2).Valeur = 20479 - _Parametres.Item(1).Valeur
                If _Parametres.Item(4).Valeur > 12287 Then _Parametres.Item(4).Valeur = 12287
                If _Parametres.Item(3).Valeur > 12287 Then _Parametres.Item(3).Valeur = 12286
                If (CInt(_Parametres.Item(4).Valeur) + CInt(_Parametres.Item(3).Valeur)) > 12287 Then _Parametres.Item(4).Valeur = 12287 - _Parametres.Item(3).Valeur
                If _Parametres.Item(6).Valeur > 255 Then _Parametres.Item(6).Valeur = 255
                If _Parametres.Item(5).Valeur > 255 Then _Parametres.Item(5).Valeur = 254
                If (CInt(_Parametres.Item(6).Valeur) + CInt(_Parametres.Item(5).Valeur)) > 255 Then _Parametres.Item(6).Valeur = 255 - _Parametres.Item(5).Valeur
                If _Parametres.Item(7).Valeur > 20479 Then _Parametres.Item(7).Valeur = 20479
                If _Parametres.Item(8).Valeur > 12287 Then _Parametres.Item(8).Valeur = 12287
                If _Parametres.Item(12).Valeur > 255 Then _Parametres.Item(12).Valeur = 255
                If _Parametres.Item(13).Valeur > 255 Then _Parametres.Item(13).Valeur = 254
                If _Parametres.Item(14).Valeur > 255 Then _Parametres.Item(14).Valeur = 255
                If _Parametres.Item(15).Valeur > 255 Then _Parametres.Item(15).Valeur = 254
                If _Parametres.Item(16).Valeur > 255 Then _Parametres.Item(16).Valeur = 255
                If _Parametres.Item(17).Valeur > 255 Then _Parametres.Item(17).Valeur = 254
                If _Parametres.Item(18).Valeur > 255 Then _Parametres.Item(18).Valeur = 255
                If _Parametres.Item(19).Valeur > 255 Then _Parametres.Item(19).Valeur = 255

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
                affTrame = _Parametres.Item(11).Valeur
                adressReadIX = _Parametres.Item(12).Valeur
                longReadIX = _Parametres.Item(13).Valeur
                adressReadQX = _Parametres.Item(14).Valeur
                longReadQX = _Parametres.Item(15).Valeur
                adressReadQW = _Parametres.Item(16).Valeur
                longReadQW = _Parametres.Item(17).Valeur
                adressWriteQX = _Parametres.Item(18).Valeur
                adressWriteQW = _Parametres.Item(19).Valeur

                Dim ConfigTxt As String = " Conf ModbusTCP = Modele : " & Modele & " Interval de lecture : " & _Parametres.Item(0).Valeur _
                                          & " Unit : " & _Parametres.Item(9).Valeur _
                                          & " Affichage Debug : " & _Parametres.Item(10).Valeur _
                                          & " Affichage Trame : " & _Parametres.Item(11).Valeur & vbCrLf _
                                          & " Adresse lecture MX : " & _Parametres.Item(1).Valeur _
                                          & " Longueur table MX : " & _Parametres.Item(2).Valeur _
                                          & " Adresse lecture MW : " & _Parametres.Item(3).Valeur _
                                          & " Longueur table MW : " & _Parametres.Item(4).Valeur & vbCrLf _
                                          & " Adresse lecture IW : " & _Parametres.Item(5).Valeur _
                                          & " Longueur table IW : " & _Parametres.Item(6).Valeur _
                                          & " Adresse ecriture MX : " & _Parametres.Item(7).Valeur _
                                          & " Adresse ecriture MW : " & _Parametres.Item(8).Valeur _
                                          & " Adresse lecture IX : " & _Parametres.Item(12).Valeur _
                                          & " Longueur table IX : " & _Parametres.Item(13).Valeur & vbCrLf _
                                          & " Adresse lecture QX : " & _Parametres.Item(14).Valeur _
                                          & " Longueur table QX : " & _Parametres.Item(15).Valeur _
                                          & " Adresse lecture QW : " & _Parametres.Item(16).Valeur _
                                          & " Longueur table QW : " & _Parametres.Item(17).Valeur _
                                          & " Adresse ecriture QX : " & _Parametres.Item(18).Valeur _
                                          & " Adresse ecriture QW : " & _Parametres.Item(19).Valeur

                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start", ConfigTxt)

                If longReadMW > 0 Then
                    If longReadMW < 76 Then
                        nbsendMW = 1
                    Else
                        nbsendMW = Math.Ceiling((longReadMW - 1) / 75)
                    End If
                Else
                    nbsendMW = 0
                End If

                If longReadIW > 0 Then
                    If longReadIW < 76 Then
                        nbsendIW = 1
                    Else
                        nbsendIW = Math.Ceiling((longReadIW - 1) / 75)
                    End If
                Else
                    nbsendIW = 0
                End If

                If longReadQW > 0 Then
                    If longReadQW < 76 Then
                        nbsendQW = 1
                    Else
                        nbsendQW = Math.Ceiling((longReadQW - 1) / 75)
                    End If
                Else
                    nbsendQW = 0
                End If

                If longReadMX > 0 Then
                    If longReadMX < 256 Then
                        nbsendMX = 1
                    Else
                        nbsendMX = Math.Ceiling((longReadMX - 1) / 255)
                    End If
                Else
                    nbsendMX = 0
                End If

                If longReadIX > 0 Then
                    If longReadIX < 256 Then
                        nbsendIX = 1
                    Else
                        nbsendIX = Math.Ceiling((longReadIX - 1) / 255)
                    End If
                Else
                    nbsendIX = 0
                End If

                If longReadQX > 0 Then
                    If longReadQX < 256 Then
                        nbsendQX = 1
                    Else
                        nbsendQX = Math.Ceiling((longReadQX - 1) / 255)
                    End If
                Else
                    nbsendQX = 0
                End If

                ReDim dataMdbRMW(nbsendMW * 75)
                ReDim dataMdbRIW(nbsendIW * 75)
                ReDim dataMdbRMX(nbsendMX * 255)
                ReDim dataMdbRIX(nbsendIX * 255)

                ReDim dataMdbWMW(nbsendMW * 75)
                ReDim dataMdbWQW(nbsendQW * 75)
                ReDim dataMdbWMX(nbsendMX * 255)
                ReDim dataMdbWQX(nbsendQX * 255)
                MyTimer.Interval = _Parametres.Item(0).Valeur '2000
                MyTimer.Enabled = True
                typeRead = "MW"
                cptsend = 0
                breading = False

                AddHandler MyTimer.Elapsed, AddressOf TimerTick
                AddHandler _Server.DeviceChanged, AddressOf DeviceChange

            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", ex.Message)
            _IsConnect = False
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Dim retour As String
        Try

            MyTimer.Enabled = False
            RemoveHandler MyTimer.Elapsed, AddressOf TimerTick

            retour = fermer()
            RemoveHandler _Server.DeviceChanged, AddressOf DeviceChange

            If STRGS.Left(retour, 4) = "ERR:" Then
                retour = STRGS.Right(retour, retour.Length - 5)
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & "", retour)
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & "", retour)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ReStart", ex.Message)
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
                If (Objet.Modele = "MX" And CInt(Objet.Adresse1) < adressReadMX + longReadMX And CInt(Objet.Adresse1) >= adressReadMX) Then

                    If TypeOf Objet.Value Is Boolean And dataMdbRMX(Objet.Adresse1) > -1 And dataMdbRMX(Objet.Adresse1) < 2 Then
                        Objet.Value = CBool(dataMdbRMX(Objet.Adresse1 - 1))
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "L'adresse du composant n'est pas dans la plage de lecture du driver")

                End If

                If (Objet.Modele = "IX" And CInt(Objet.Adresse1) < adressReadIX + longReadIX And CInt(Objet.Adresse1) >= adressReadIX) Then

                    If TypeOf Objet.Value Is Boolean And dataMdbRIX(Objet.Adresse1) > -1 And dataMdbRIX(Objet.Adresse1) < 2 Then
                        Objet.Value = CBool(dataMdbRIX(Objet.Adresse1 - 1))
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "L'adresse du composant n'est pas dans la plage de lecture du driver")

                End If

                If (Objet.Modele = "MW" And CInt(Objet.Adresse1) < adressReadMW + longReadMW And CInt(Objet.Adresse1) >= adressReadMW) Then

                    If TypeOf Objet.Value Is Integer Or TypeOf Objet.Value Is Double Then
                        Objet.Value = dataMdbRMW(Objet.Adresse1 - 1)
                    End If
                    If TypeOf Objet.Value Is Boolean And dataMdbRMW(Objet.Adresse1) > -1 And dataMdbRMW(Objet.Adresse1) < 2 Then
                        Objet.Value = CBool(dataMdbRMW(Objet.Adresse1 - 1))
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "L'adresse du composant n'est pas dans la plage de lecture du driver")

                End If

                If (Objet.Modele = "IW" And CInt(Objet.Adresse1) < adressReadIW + longReadIW And CInt(Objet.Adresse1) >= adressReadIW) Then

                    If TypeOf Objet.Value Is Integer Or TypeOf Objet.Value Is Double Then
                        Objet.Value = dataMdbRIW(Objet.Adresse1 - 1)
                    End If
                    If TypeOf Objet.Value Is Boolean And dataMdbRIW(Objet.Adresse1) > -1 And dataMdbRIW(Objet.Adresse1) < 2 Then
                        Objet.Value = CBool(dataMdbRIW(Objet.Adresse1 - 1))
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

        Dim sendtwice As Boolean = False
        Try
            If _Enable = False Then Exit Sub
            If _IsConnect = False Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Write", "Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Ecriture de " & Objet.Name)
            If Parametre1 Is Nothing Then Parametre1 = 0

            If Objet.Adresse2 <> "-1" And Objet.Adresse2 <> "" Then

                Select Case Objet.Modele
                    Case "MX"
                        Parametre2 = 1
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Debut d'ecriture MX ")

                    Case "QX"
                        Parametre2 = 2
                    Case "MW"
                        Parametre2 = 3
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Debut d'ecriture MW ")
                    Case "QW"
                        Parametre2 = 4
                End Select

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

                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Valeur a ecrire : " & Parametre1 & " et memoire d'écriture = " & Objet.Value & " et memoire de lecture = " & dataMdbRMW(Objet.Adresse2))

                End If

            End If
            Select Case Commande
                'Case "ON", "OFF"
                'If Parametre1 = 0 Then traitement("OFF", Objet.adresse2, Commande, True) Else traitement("ON", Objet.adresse2, Commande, True)
                'Case "OPEN", "CLOSE"
                'If Parametre1 = 0 Then traitement("CLOSE", Objet.adresse2, Commande, True) Else traitement("OPEN", Objet.adresse2, Commande, True)
                Case "DIM", "OUVERTURE"
                    traitement(CStr(Parametre1), Objet.Adresse2, Commande, Objet.Modele, True)
                Case Else
                    traitement(Commande, Objet.Adresse2, Commande, Objet.Modele, True)
            End Select
            If Not flagWrite Then
                flagWrite = True
                If Not breading Then TestWrite()
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>

    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " NewDevice", ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
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
            add_paramavance("Longueur table de bits interne à lire", "Longueur table de bits interne MX à lire dans l'automate", 0)
            add_paramavance("Premier mot interne de lecture", "Adresse du premier mot interne MW à lire dans l'automate", 256)
            add_paramavance("Longueur table de mots interne à lire", "Longueur table de mots interne MW à lire dans l'automate", 225)
            add_paramavance("Premier mot d'entrée de lecture", "Adresse du premier mot d'entrée IW à lire dans l'automate", 0)
            add_paramavance("Longueur table de mots d'entrée à lire", "Longueur table de mots d'entrée IW à lire dans l'automate", 0)
            add_paramavance("Premier bit interne d'ecriture", "Adresse du premier bit interne MX à écrire dans l'automate", 0)
            add_paramavance("Premier mot interne d'ecriture", "Adresse du premier mot interne MW à écrire dans l'automate", 700)
            add_paramavance("Numéro Unit", "Numéro d'identification de l'unité a accéder", 0)
            add_paramavance("Debug", "Activer le Debug complet (True/False)", False)
            add_paramavance("Trame", "Activer l'affichage des trames (True/False)", False)
            add_paramavance("Premier bit d'entrée de lecture", "Adresse du premier bit d'entrée IX à lire dans l'automate", 0)
            add_paramavance("Longueur table de bits d'entrée à lire", "Longueur table de bits d'entrée IX à lire dans l'automate", 0)
            add_paramavance("Premier bit de sortie de lecture", "Adresse du premier bit de sortie QX à lire dans l'automate", 0)
            add_paramavance("Longueur table de bits de sortie à lire", "Longueur table de bits de sortie QX à lire dans l'automate", 0)
            add_paramavance("Premier mot de sortie de lecture", "Adresse du premier mot de sortie QW à lire dans l'automate", 0)
            add_paramavance("Longueur table de mots de sortie à lire", "Longueur table de mots de sortie QW à lire dans l'automate", 0)
            add_paramavance("Premier bit de sortie d'ecriture", "Adresse du premier bit de sortie QX à écrire dans l'automate", 0)
            add_paramavance("Premier mot de sortie d'ecriture", "Adresse du premier mot de sortie QW à écrire dans l'automate", 0)

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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        Dim StartAddress As UShort = 0
        Dim Length As UShort = 0
        If _Enable = False Then Exit Sub

        Try
            If Not breading Then

                If Not flagWrite Then

                    cptsend += 1

                    Select Case typeRead

                        Case "MW"
                            If nbsendMW > 0 And cptsend <= nbsendMW Then

                                breading = True
                                StartAddress = ReadStartAdr(offsetModele + adressReadMW + ((cptsend - 1) * 75)) '%MW0 = 12288 
                                Length = ReadStartAdr(75)
                                MBmaster.ReadHoldingRegister(3, unit, StartAddress, Length)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Demande de lecture MW" & StartAddress & " , Envoi n°" & cptsend & "/" & nbsendMW)

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
                                MBmaster.ReadInputRegister(4, unit, StartAddress, Length)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Demande de lecture IW" & StartAddress & " , Envoi n°" & cptsend & "/" & nbsendIW)

                            Else
                                typeRead = "MX"
                                cptsend = 0
                                breading = False
                            End If

                        Case "MX"
                            If nbsendMX > 0 And cptsend <= nbsendMX Then
                                breading = True
                                StartAddress = ReadStartAdr(offsetModele + adressReadMX + ((cptsend - 1) * 255)) '%MX0.0 = 12288
                                If nbsendMX = 1 Then Length = ReadStartAdr(longReadMX) Else Length = ReadStartAdr(255)
                                MBmaster.ReadCoils(1, unit, StartAddress, Length)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Demande de lecture MX" & StartAddress & " , Envoi n°" & cptsend & "/" & nbsendMX)

                            Else
                                typeRead = "IX"
                                cptsend = 0
                                breading = False
                            End If

                        Case "IX"
                            If nbsendIX > 0 And cptsend <= nbsendIX Then

                                breading = True
                                StartAddress = ReadStartAdr(adressReadIX + ((cptsend - 1) * 255)) '%MX0.0 = 12288
                                If nbsendIX = 1 Then Length = ReadStartAdr(longReadIX) Else Length = ReadStartAdr(255)
                                MBmaster.ReadDiscreteInputs(2, unit, StartAddress, Length)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Demande de lecture IX" & StartAddress & " , Envoi n°" & cptsend & "/" & nbsendIX)

                            Else
                                typeRead = "MW"
                                cptsend = 0
                                breading = False
                            End If

                    End Select


                    cptWaitReponse = 0
                Else
                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Tentative de lecture échoué car ecriture en cours")
                    If Not breading Then TestWrite()
                End If
            Else
                If cptWaitReponse = 10 Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Time Out lecture deja en cours")

                    cptWaitReponse = 0
                    breading = False
                Else
                    cptWaitReponse += 1
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read " & " startadress=" & StartAddress & " et length=" & Length, ex.Message)
        End Try

    End Sub

    Sub DeviceChange(ByVal DeviceID, ByVal valeurString)
        Try
            Dim genericDevice As templateDevice = _Server.ReturnDeviceById(_IdSrv, DeviceID)
            If genericDevice.DriverID = _ID Then
                If genericDevice.Modele = "MW" Then
                    If testDatatableW(genericDevice) And genericDevice.Adresse2 <> "-1" And genericDevice.Adresse2 <> "" Then
                        Threading.Thread.Sleep(500)
                        If genericDevice.Value <> dataMdbWMW(genericDevice.Adresse2) And Not flagWrite Then '  
                            flagWrite = True
                            'ecrire(genericDevice.Adresse2, "", CInt(valeurString), 3)
                            If Not breading Then TestWrite()
                        End If
                    End If
                End If
                If genericDevice.Modele = "MX" Then
                    If testDatatableB(genericDevice) And genericDevice.Adresse2 <> "-1" And genericDevice.Adresse2 <> "" Then
                        Threading.Thread.Sleep(500)
                        If genericDevice.Value <> dataMdbWMX(genericDevice.Adresse2) And Not flagWrite Then '  
                            flagWrite = True
                            'ecrire(genericDevice.Adresse2, "", CInt(valeurString), 3)
                            If Not breading Then TestWrite()
                        End If
                    End If
                End If
                If genericDevice.Modele = "QW" Then
                    If testDatatableW(genericDevice) And genericDevice.Adresse2 <> "-1" And genericDevice.Adresse2 <> "" Then
                        Threading.Thread.Sleep(500)
                        If genericDevice.Value <> dataMdbWQW(genericDevice.Adresse2) And Not flagWrite Then '  
                            flagWrite = True
                            'ecrire(genericDevice.Adresse2, "", CInt(valeurString), 3)
                            If Not breading Then TestWrite()
                        End If
                    End If
                End If
                If genericDevice.Modele = "QX" Then
                    If testDatatableB(genericDevice) And genericDevice.Adresse2 <> "-1" And genericDevice.Adresse2 <> "" Then
                        Threading.Thread.Sleep(500)
                        If genericDevice.Value <> dataMdbWQX(genericDevice.Adresse2) And Not flagWrite Then '  
                            flagWrite = True
                            'ecrire(genericDevice.Adresse2, "", CInt(valeurString), 3)
                            If Not breading Then TestWrite()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Device Change", ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Wait", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As String, ByVal adresse As String, ByVal commande As String, ByVal modele As String, ByVal erreursidevicepastrouve As Boolean)
        If valeur <> "" Then
            Try

                'Recherche si un device affecté
                Dim listedevicesfind As New ArrayList
                listedevicesfind = _Server.ReturnDeviceByAdresse2TypeDriver(_IdSrv, adresse, "", Me._ID, True)
                If IsNothing(listedevicesfind) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Traitement", "Communication impossible avec le serveur, l'IDsrv est peut être erroné : " & _IdSrv)
                    Exit Sub
                End If
                'un device trouvé on maj la value
                If (listedevicesfind.Count > 0) Then
                    For Each Device In listedevicesfind
                        If Device.Modele = modele Then
                            If TypeOf Device.Value Is Integer Then
                                If valeur = "ON" Or valeur = "OPEN" Then
                                    Device.Value = Device.ValueMax
                                ElseIf valeur = "OFF" Or valeur = "CLOSE" Then
                                    Device.Value = Device.ValueMin
                                Else
                                    Device.Value = valeur
                                End If
                            ElseIf TypeOf Device.Value Is Boolean Then
                                If valeur = "ON" Or valeur = "OPEN" Then
                                    Device.Value = True
                                ElseIf valeur = "OFF" Or valeur = "CLOSE" Then
                                    Device.Value = False
                                Else
                                    Device.Value = True
                                End If
                            Else
                                Device.Value = valeur
                            End If
                        End If
                    Next
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Traitement", "Device non trouvé : " & adresse & ":" & valeur)

                    'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)

                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & commande & "-" & valeur)
            End Try
        End If
    End Sub

    ''' <summary>Tester la possiblité d'écrire sur le port ModbusTCP</summary>
    ''' <remarks></remarks>

    Private Sub TestWrite()

        Dim listedevices As New ArrayList
        Dim adresse As Integer

        Try
            'Recherche si un device affecté

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, "", "", Me._ID, True)

            For Each j As Object In listedevices
                If j.Adresse2 <> "-1" And j.Adresse2 <> "" Then
                    adresse = j.Adresse2
                    Select Case j.modele
                        Case "MW"
                            If testDatatableW(j) Then
                                If nbsendMW > 0 Then
                                    If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                        If j.Value <> dataMdbWMW(j.adresse2) Then
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Match TestWrite MW ", "adress= " & j.adresse2 & " ; mWrite= " & dataMdbWMW(j.adresse2) & " ; value= " & j.Value & ") ; ")
                                            dataMdbWMW(j.adresse2) = j.Value
                                            flagWrite = True
                                            ecrire(j.adresse2, "DIM", j.value, 3)
                                            Exit Sub
                                        End If
                                    End If
                                    If TypeOf j.Value Is Boolean Then
                                        If j.Value <> CBool(dataMdbWMW(j.adresse2)) Then
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Match TestWrite MW ", "adress= " & j.adresse2 & " ; mWrite= " & CBool(dataMdbWMW(j.adresse2)) & " ; value= " & j.Value & ") ; ")
                                            If j.Value = False Then
                                                dataMdbWMW(j.adresse2) = 0
                                            Else
                                                dataMdbWMW(j.adresse2) = 1
                                            End If
                                            flagWrite = True
                                            If j.value = False Then ecrire(j.adresse2, "OFF", 0, 3)
                                            If j.value = True Then ecrire(j.adresse2, "ON", 1, 3)
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            End If
                        Case "MX"
                            If testDatatableB(j) Then
                                If nbsendMX > 0 Then
                                    If TypeOf j.Value Is Boolean Then
                                        If j.Value <> dataMdbWMX(j.adresse2) Then
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Match TestWrite MX ", "adress= " & j.adresse2 & " ; mWrite= " & dataMdbWMX(j.adresse2) & " ; value= " & j.Value & ") ; ")
                                            dataMdbWMX(j.adresse2) = j.Value
                                            flagWrite = True
                                            If j.value = False Then ecrire(j.adresse2, "OFF", 0, 1)
                                            If j.value = True Then ecrire(j.adresse2, "ON", 1, 1)
                                            Exit Sub
                                        End If
                                    End If
                                    If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                        If j.Value <> CInt(dataMdbWMX(j.adresse2)) Then
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Match TestWrite MX ", "adress= " & j.adresse2 & " ; mWrite= " & CUShort(dataMdbWMX(j.adresse2)) & " ; value= " & j.Value & ") ; ")
                                            If j.Value = 0 Then
                                                dataMdbWMX(j.adresse2) = False
                                            Else
                                                dataMdbWMX(j.adresse2) = True
                                            End If
                                            flagWrite = True
                                            If j.value = 0 Then
                                                ecrire(j.adresse2, "OFF", 0, 1)
                                            Else
                                                ecrire(j.adresse2, "ON", 1, 1)
                                            End If
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            End If
                        Case "QW"
                            If testDatatableW(j) Then
                                If nbsendQW > 0 Then
                                    If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                        If j.Value <> dataMdbWQW(j.adresse2) Then
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Match TestWrite QW ", "adress= " & j.adresse2 & " ; mWrite= " & dataMdbWQW(j.adresse2) & " ; value= " & j.Value & ") ; ")
                                            dataMdbWQW(j.adresse2) = j.Value
                                            flagWrite = True
                                            ecrire(j.adresse2, "DIM", j.value, 4)
                                            Exit Sub
                                        End If
                                    End If
                                    If TypeOf j.Value Is Boolean Then
                                        If j.Value <> CBool(dataMdbWQW(j.adresse2)) Then
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Match TestWrite QW ", "adress= " & j.adresse2 & " ; mWrite= " & CBool(dataMdbWQW(j.adresse2)) & " ; value= " & j.Value & ") ; ")
                                            If j.Value = False Then
                                                dataMdbWQW(j.adresse2) = 0
                                            Else
                                                dataMdbWQW(j.adresse2) = 1
                                            End If
                                            flagWrite = True
                                            If j.value = False Then ecrire(j.adresse2, "OFF", 0, 4)
                                            If j.value = True Then ecrire(j.adresse2, "ON", 1, 4)
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            End If
                        Case "QX"
                            If testDatatableB(j) Then
                                If nbsendQX > 0 Then
                                    If TypeOf j.Value Is Boolean Then
                                        If j.Value <> dataMdbWQX(j.adresse2) Then
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Match TestWrite QX ", "adress= " & j.adresse2 & " ; mWrite= " & dataMdbWQX(j.adresse2) & " ; value= " & j.Value & ") ; ")
                                            dataMdbWQX(j.adresse2) = j.Value
                                            flagWrite = True
                                            If j.value = False Then ecrire(j.adresse2, "OFF", 0, 2)
                                            If j.value = True Then ecrire(j.adresse2, "ON", 1, 2)
                                            Exit Sub
                                        End If
                                    End If
                                    If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                        If j.Value <> CInt(dataMdbWQX(j.adresse2)) Then
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Match TestWrite QX ", "adress= " & j.adresse2 & " ; mWrite= " & CUShort(dataMdbWQX(j.adresse2)) & " ; value= " & j.Value & ") ; ")
                                            If j.Value = 0 Then
                                                dataMdbWQX(j.adresse2) = False
                                            Else
                                                dataMdbWQX(j.adresse2) = True
                                            End If
                                            flagWrite = True

                                            If j.value = 0 Then
                                                ecrire(j.adresse2, "OFF", 0, 2)
                                            Else
                                                ecrire(j.adresse2, "ON", 1, 2)
                                            End If

                                            Exit Sub
                                        End If
                                    End If
                                End If
                            End If

                    End Select
                End If
            Next

            flagWrite = False
            breading = False
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Test Write", "Fin du traitement d'ecriture")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Test Write", "Exception : " & ex.Message & " adresse --> " & adresse)
        End Try

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
                        StartAddress = ReadStartAdr(CInt(adresse) + adressWriteMW + offsetModele)
                    End If
                    If data2 = 1 Then
                        StartAddress = ReadStartAdr(CInt(adresse) + adressWriteMX + offsetModele)
                    End If
                    If data2 = 4 Then
                        StartAddress = ReadStartAdr(CInt(adresse) + adressWriteQW)
                    End If
                    If data2 = 2 Then
                        StartAddress = ReadStartAdr(CInt(adresse) + adressWriteQX)
                    End If

                    If data2 = 3 Or data2 = 4 Then 'MW ou QW

                        DataM(0) = data1
                        Data = GetData(DataM)
                        MBmaster.WriteSingleRegister(7, unit, StartAddress, Data)
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Ecrire", "MBmaster_WriteSingleRegister : à l'adresse MW ou QW" & StartAddress - offsetModele & " a la valeur " & data1)

                    End If
                    If data2 = 1 Or data2 = 2 Then 'MX ou QX

                        DataB = CBool(data1)
                        MBmaster.WriteSingleCoils(5, unit, StartAddress, DataB)
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Ecrire", "MBmaster_WriteSingleCoils : à l'adresse MX ou QX" & StartAddress - offsetModele & " a la valeur " & CBool(data1))

                    End If

                    Threading.Thread.Sleep(200)
                    If Not breading Then TestWrite()

                Catch ex As Exception
                    Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Ecrire Exception", "Ecriture startadress=" & StartAddress)
                End Try

                'renvoie la valeur ecrite
                Return "VALUE"

            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Ecrire", "Port Fermé, impossible d ecrire : " & adresse & " : " & commande & " " & data1 & "-" & data2)
                Return ""
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Ecrire", "exception : " & ex.Message)
            Return "" 'on renvoi rien car il y a eu une erreur
        End Try
    End Function

    Private Function testDatatableW(ByVal device As Object) As Boolean
        Dim retour = True
        If device.modele = "MW" Then
            If dataMdbRMW.Count <= device.adresse1 Then
                retour = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " testDatatableW ", "L'adresse de lecture est superieur au parametre du driver --> Nombre de mots internes à lire")
            End If
            If dataMdbWMW.Count <= device.adresse2 Then
                retour = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " testDatatableW ", "L'adresse d'ecriture est superieur au parametre du driver --> Nombre de mots internes à ecrire")
            End If
        End If
        If device.modele = "IW" Then
            If dataMdbRIW.Count <= device.adresse1 Then
                retour = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " testDatatableW ", "L'adresse de lecture est superieur au parametre du driver --> Nombre de mots d'entrée à lire")
            End If
        End If
        If device.modele = "QW" Then
            If dataMdbWQW.Count <= device.adresse1 Then
                retour = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " testDatatableW ", "L'adresse de lecture est superieur au parametre du driver --> Nombre de mots de sortie a ecrire")
            End If
        End If
        Return retour
    End Function

    Private Function testDatatableB(ByVal device As Object) As Boolean
        Dim retour = True
        If device.modele = "MX" Then
            If dataMdbRMX.Count <= device.adresse1 Then
                retour = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " testDatatableB ", "L'adresse de lecture est superieur au parametre du driver --> Nombre de bits internes à lire")
            End If
            If dataMdbWMX.Count <= device.adresse2 Then
                retour = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " testDatatableB ", "L'adresse d'ecriture est superieur au parametre du driver --> Nombre de bits internes à ecrire")
            End If
        End If
        If device.modele = "IX" Then
            If dataMdbRIX.Count <= device.adresse1 Then
                retour = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " testDatatableB ", "L'adresse de lecture est superieur au parametre du driver --> Nombre de bits d'entrée à lire")
            End If
        End If
        If device.modele = "QX" Then
            If dataMdbWQX.Count <= device.adresse1 Then
                retour = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " testDatatableB ", "L'adresse de lecture est superieur au parametre du driver --> Nombre de bits de sortie à lire")
            End If
        End If
        Return retour
    End Function

#End Region

#Region "ModbusTCP"

    ' ------------------------------------------------------------------------
    ' Event for response data
    ' ------------------------------------------------------------------------
    Private Sub MBmaster_OnResponseData(ByVal ID As UShort, ByVal unit As Byte, ByVal [function] As Byte, ByVal values As Byte()) Handles MBmaster.OnResponseData

        Dim msg As String = ""
        'Dim adresse As Integer = 0
        ' ------------------------------------------------------------------
        Try

            ' Ignore watchdog response data
            If ID = &HFF Then
                Return
            End If

            ' ------------------------------------------------------------------------
            ' Identify requested data

            msg = "ID = " & ID & " et Fonction = " & [function]

            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " slave receive", "MBmaster_OnResponseData : " & msg)
            If Not flagWrite Then
                If Not breading Then TestWrite()
            Else
                Exit Sub
            End If

            If Not [function] = 6 Then

                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, "", "", Me._ID, True)
                Dim deb As Boolean = False
                If typeRead = "MW" Then
                    memdataMdbRMW = dataMdbRMW
                    dataMdbRMW = ShowAsW(values, dataMdbRMW, 75 * (cptsend - 1))
                    If _DEBUG Then
                        For i = 0 To dataMdbRMW.Length - 1
                            If memdataMdbRMW(i) <> dataMdbRMW(i) Then
                                If deb = False Then msg += " Debut Read MW" & adressReadMW + (75 * (cptsend - 1)) & " et data change : " : deb = True
                                msg += "adresse homidom = " & i & " , adresse automate = MW" & adressReadMW + i + (75 * (cptsend - 1)) & " , valeur = " & CStr(dataMdbRMW(i)) & "; "
                            End If
                        Next
                    End If
                    If cptsend = nbsendMW Then FirstStart = False

                End If

                If typeRead = "IW" Then
                    memdataMdbRIW = dataMdbRIW
                    dataMdbRIW = ShowAsW(values, dataMdbRIW, 75 * (cptsend - 1))

                    For i = 0 To dataMdbRIW.Length - 1
                        If memdataMdbRIW(i) <> dataMdbRIW(i) Then
                            If deb = False Then msg += " Debut Read IW" & adressReadIW + (75 * (cptsend - 1)) & " et data change : " : deb = True
                            msg += "adresse homidom = " & i & " , adresse automate = IW" & adressReadIW + i + (75 * (cptsend - 1)) & " , valeur = " & CStr(dataMdbRIW(i)) & "; "
                        End If
                    Next
                End If

                If typeRead = "MX" Then
                    memdataMdbRMX = dataMdbRMX
                    dataMdbRMX = ShowAsB(values, dataMdbRMX, 255 * (cptsend - 1))

                    For i = 0 To dataMdbRMX.Length - 1
                        If memdataMdbRMX(i) <> dataMdbRMX(i) Then
                            If deb = False Then msg += " Debut Read MX" & Math.Floor(adressReadMX / 16) + (255 * (cptsend - 1)) & "." & adressReadMX Mod 16 & " et data change : " : deb = True
                            msg += "adresse homidom = " & i & " , adresse automate = MX" & Math.Floor((adressReadMX + i) / 16) + (255 * (cptsend - 1)) & "." & (adressReadMX + i) Mod 16 & " , valeur = " & CStr(dataMdbRMX(i)) & "; "
                        End If
                    Next
                End If

                If typeRead = "IX" Then
                    memdataMdbRIX = dataMdbRIX
                    dataMdbRIX = ShowAsB(values, dataMdbRIX, 255 * (cptsend - 1))

                    For i = 0 To dataMdbRIX.Length - 1
                        If deb = False Then msg += " Debut Read IX" & Math.Floor(adressReadIX / 16) + (255 * (cptsend - 1)) & "." & adressReadIX Mod 16 & " et data change : " : deb = True
                        If memdataMdbRIX(i) <> dataMdbRIX(i) Then
                            msg += "adresse homidom = " & i & " , adresse automate = IX" & Math.Floor((adressReadIX + i) / 16) + (255 * (cptsend - 1)) & "." & (adressReadIX + i) Mod 16 & " , valeur = " & CStr(dataMdbRIX(i)) & "; "
                        End If
                    Next
                End If

                If _DEBUG And msg <> "" Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " slave receive", "MBmaster_OnResponseData : " & msg)
                msg = ""

                For Each j As Object In listedevices
                    If j.Adresse1 <> "-1" And j.Adresse1 <> "" Then

                        If j.modele = typeRead And j.Refresh = 0 Then

                            If j.modele = "MW" Then
                                If j.adresse1 >= 76 * (cptsend - 1) And j.adresse1 < 76 * cptsend And testDatatableW(j) Then
                                    If j.Value <> dataMdbRMW(j.adresse1) And Not flagWrite Then
                                        If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                            j.Value = dataMdbRMW(j.adresse1)
                                        End If
                                        If TypeOf j.Value Is Boolean Then
                                            If dataMdbRMW(j.adresse1) > 0 Then
                                                j.Value = True
                                            Else
                                                j.Value = False
                                            End If
                                        End If
                                        msg += "MW : ( adress= " & j.adresse1 & " ; value= " & j.Value & " ; mRead= " & dataMdbRMW(j.adresse1)
                                        If j.adresse2 > -1 Then
                                            dataMdbWMW(j.adresse2) = dataMdbRMW(j.adresse1)
                                            msg += " ; mWrite= " & dataMdbWMW(j.adresse2) & ") ; "
                                        Else
                                            msg += ") ; "
                                        End If
                                    End If
                                End If
                            End If

                            If j.modele = "IW" Then
                                If j.adresse1 >= 76 * (cptsend - 1) And j.adresse1 < 76 * cptsend And testDatatableW(j) Then
                                    If j.Value <> dataMdbRIW(j.adresse1) Then
                                        msg += "IW : ( adress= " & j.adresse1 & " ; value= " & j.Value & " ; mRead= " & dataMdbRIW(j.adresse1) & ") ; "
                                        If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                            j.Value = dataMdbRIW(j.adresse1)
                                        End If
                                        If TypeOf j.Value Is Boolean Then
                                            If dataMdbRIW(j.adresse1) > 0 Then
                                                j.Value = True
                                            Else
                                                j.Value = False
                                            End If
                                        End If
                                    End If
                                End If
                            End If


                            If j.modele = "MX" Then
                                If j.adresse1 >= 256 * (cptsend - 1) And j.adresse1 < 256 * cptsend And testDatatableB(j) Then
                                    If j.Value <> dataMdbRMX(j.adresse1) And Not flagWrite Then
                                        If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                            If dataMdbRMX(j.adresse1) = False Then
                                                j.Value = 0
                                            Else
                                                j.Value = 1
                                            End If
                                        End If
                                        If TypeOf j.Value Is Boolean Then
                                            j.Value = dataMdbRMX(j.adresse1)
                                        End If
                                        msg += "MX : ( adress= " & j.adresse1 & " ; value= " & j.Value & " ; mRead= " & dataMdbRMX(j.adresse1)
                                        If j.adresse2 > -1 Then
                                            dataMdbWMX(j.adresse2) = dataMdbRMX(j.adresse1)
                                            msg += " ; mWrite= " & dataMdbWMX(j.adresse2) & ") ; "
                                        Else
                                            msg += ") ; "
                                        End If
                                    End If
                                End If
                            End If

                            If j.modele = "IX" Then
                                If j.adresse1 >= 256 * (cptsend - 1) And j.adresse1 < 256 * cptsend And testDatatableB(j) Then
                                    If j.Value <> dataMdbRIX(j.adresse1) Then
                                        msg += "IX : ( adress= " & j.adresse1 & " ; value= " & j.Value & " ; mRead= " & dataMdbRIX(j.adresse1) & ") ; "
                                        If TypeOf j.Value Is Integer Or TypeOf j.Value Is Double Then
                                            If dataMdbRIX(j.adresse1) = False Then
                                                j.Value = 0
                                            Else
                                                j.Value = 1
                                            End If
                                        End If
                                        If TypeOf j.Value Is Boolean Then
                                            j.Value = dataMdbRIX(j.adresse1)
                                        End If
                                    End If
                                End If
                            End If

                        End If

                    End If

                Next

                If _DEBUG And msg <> "" Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " slave receive", "MBmaster_OnResponseData : Composants pris en compte " & msg)
                Threading.Thread.Sleep(100)
                breading = False

            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " slave exception", "MBmaster_OnResponseData : " & msg)
            breading = False
        End Try
    End Sub

    ' ------------------------------------------------------------------------
    ' Modbus TCP slave exception
    ' ------------------------------------------------------------------------
    Private Sub MBmaster_OnException(ByVal id As UShort, ByVal unit As Byte, ByVal [function] As Byte, ByVal exception As Byte) Handles MBmaster.OnException
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

        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " slave exception", exc)
        Restart()
    End Sub


    ' ------------------------------------------------------------------------
    ' Modbus TCP slave data sent
    ' ------------------------------------------------------------------------
    Private Sub MBmaster_OnSendData(ByVal id As UShort, ByVal data() As Byte) Handles MBmaster.OnSendData
        ' ------------------------------------------------------------------

        If affTrame And _DEBUG Then

            Dim msg As String = "ID = " & id & " et data = "
            For Each i As Byte In data
                msg &= i & " "
            Next

            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " slave send", "MBmaster_OnSendData , trame : " & msg)

        End If

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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " slave exception", "ReadStartAdr")
        End Try
    End Function

    ' ------------------------------------------------------------------------
    ' Read start address
    ' ------------------------------------------------------------------------
    Private Function Compare(ByVal comp1 As UShort(), ByVal comp2 As UShort()) As Boolean
        Try
            For Index = 0 To comp1.Length - 1
                If comp1(Index) <> comp2(Index) Then
                    Return False
                    Exit Function
                End If
            Next
            Return True
        Catch ex As Exception
            Return False
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " slave exception", "Compare")
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
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GetData", num.ToString)

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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " slave exception", "GetData")
        End Try
    End Function

    ' ------------------------------------------------------------------------
    ' Show values in selected way
    ' ------------------------------------------------------------------------
    Private Function ShowAsW(ByVal dataR() As Byte, ByVal orig As UInt16(), ByVal StartAdr As Integer) As UInt16()

        Dim word As UInt16() = New UInt16(0) {}
        Try
            ' Convert data to selected data type
            If dataR.Length < 2 Then
                Return Nothing
                Exit Function
            End If
            word = orig '= New UInt16(CInt(dataR.Length / 2 - 1)) {}
            Dim x As Integer = 0
            While x < dataR.Length
                word(CInt(x / 2) + StartAdr) = CType(dataR(x) * 256 + dataR(x + 1), UInt16)
                x = x + 2
            End While

            Return word
        Catch ex As Exception
            Return word
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " slave exception", "ShowAs")
        End Try
    End Function

    Private Function ShowAsB(ByVal dataR() As Byte, ByVal orig As Boolean(), ByVal StartAdr As Integer) As Boolean()

        Dim bool As Boolean() = New Boolean(0) {}
        Try
            ' Convert data to selected data type
            If dataR.Length < 1 Then
                Return Nothing
                Exit Function
            End If
            bool = orig '= New Boolean(CInt(dataR.Length * 8 - 1)) {}
            Dim x As Integer = 0

            While x < dataR.Length
                bool(CInt(x * 8) + StartAdr) = CType(dataR(x) And Hex(("&h" & 1)), Boolean)
                bool(CInt(x * 8) + StartAdr + 1) = CType(dataR(x) And Hex(("&h" & 2)), Boolean)
                bool(CInt(x * 8) + StartAdr + 2) = CType(dataR(x) And Hex(("&h" & 4)), Boolean)
                bool(CInt(x * 8) + StartAdr + 3) = CType(dataR(x) And Hex(("&h" & 8)), Boolean)
                bool(CInt(x * 8) + StartAdr + 4) = CType(dataR(x) And Hex(("&h" & 16)), Boolean)
                bool(CInt(x * 8) + StartAdr + 5) = CType(dataR(x) And Hex(("&h" & 32)), Boolean)
                bool(CInt(x * 8) + StartAdr + 6) = CType(dataR(x) And Hex(("&h" & 64)), Boolean)
                bool(CInt(x * 8) + StartAdr + 7) = CType(dataR(x) And Hex(("&h" & 128)), Boolean)
                x = x + 1
            End While

            Return bool
        Catch ex As Exception
            Return bool
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " slave exception", "ShowAs")
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
        Private Shared _timeout As UShort = 500
        Private Shared _refresh As UShort = 10
        Private Shared _connected As Boolean = False

        Private tcpAsyCl As Socket
        Private tcpAsyClBuffer(2047) As Byte

        Private tcpSynCl As Socket
        Private tcpSynClBuffer(2047) As Byte

        ' ------------------------------------------------------------------------
        ''' <summary>Response data event. This event is called when new data arrives</summary>
        Public Delegate Sub ResponseData(ByVal id As UShort, ByVal unit As Byte, ByVal [function] As Byte, ByVal data() As Byte)
        ''' <summary>Response data event. This event is called when new data arrives</summary>
        Public Event OnResponseData As ResponseData
        ''' <summary>Exception data event. This event is called when the data is incorrect</summary>
        Public Delegate Sub ExceptionData(ByVal id As UShort, ByVal unit As Byte, ByVal [function] As Byte, ByVal exception As Byte)
        ''' <summary>Exception data event. This event is called when the data is incorrect</summary>
        Public Event OnException As ExceptionData
        ''' <summary>Response data event. This event is called when new data arrives</summary>
        Public Delegate Sub WriteData(ByVal id As UShort, ByVal data() As Byte)
        ''' <summary>Exception data event. This event is called when the data is incorrect</summary>
        Public Event OnSendData As WriteData

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
        ''' <summary>Refresh timer for slave answer. The class is polling for answer every X ms.</summary>
        ''' <value>The default value is 10ms.</value>
        Public Property refresh() As UShort
            Get
                Return _refresh
            End Get
            Set(ByVal value As UShort)
                _refresh = value
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
                tcpAsyCl = New Socket(IPAddress.Parse(ip).AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                tcpAsyCl.Connect(New IPEndPoint(IPAddress.Parse(ip), port))
                tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout)
                tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout)
                tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1)
                ' ----------------------------------------------------------------
                ' Connect synchronous client
                tcpSynCl = New Socket(IPAddress.Parse(ip).AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                tcpSynCl.Connect(New IPEndPoint(IPAddress.Parse(ip), port))
                tcpSynCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout)
                tcpSynCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout)
                tcpSynCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1)
                _connected = True
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
            Dispose()
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Destroy master instance</summary>
        Public Sub Dispose()
            If tcpAsyCl IsNot Nothing Then
                If tcpAsyCl.Connected Then
                    Try
                        tcpAsyCl.Shutdown(SocketShutdown.Both)
                    Catch
                    End Try
                    tcpAsyCl.Close()
                End If
                tcpAsyCl = Nothing
            End If
            If tcpSynCl IsNot Nothing Then
                If tcpSynCl.Connected Then
                    Try
                        tcpSynCl.Shutdown(SocketShutdown.Both)
                    Catch
                    End Try
                    tcpSynCl.Close()
                End If
                tcpSynCl = Nothing
            End If
        End Sub

        Friend Sub CallException(ByVal id As UShort, ByVal unit As Byte, ByVal [function] As Byte, ByVal exception As Byte)
            If (tcpAsyCl Is Nothing) OrElse (tcpSynCl Is Nothing) Then
                Return
            End If
            If exception = excExceptionConnectionLost Then
                tcpSynCl = Nothing
                tcpAsyCl = Nothing
            End If
            RaiseEvent OnException(id, unit, [function], exception)
        End Sub

        Friend Shared Function SwapUInt16(ByVal inValue As UInt16) As UInt16
            Return CUShort(((inValue And &HFF00) >> 8) Or ((inValue And &HFF) << 8))
        End Function

        ' ------------------------------------------------------------------------
        ''' <summary>Read coils from slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        Public Sub ReadCoils(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numInputs As UShort)
            WriteAsyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadCoil), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read coils from slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="values">Contains the result of function.</param>
        Public Sub ReadCoils(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numInputs As UShort, ByRef values() As Byte)
            values = WriteSyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadCoil), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read discrete inputs from slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        Public Sub ReadDiscreteInputs(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numInputs As UShort)
            WriteAsyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadDiscreteInputs), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read discrete inputs from slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="values">Contains the result of function.</param>
        Public Sub ReadDiscreteInputs(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numInputs As UShort, ByRef values() As Byte)
            values = WriteSyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadDiscreteInputs), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read holding registers from slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        Public Sub ReadHoldingRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numInputs As UShort)
            WriteAsyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadHoldingRegister), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read holding registers from slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="values">Contains the result of function.</param>
        Public Sub ReadHoldingRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numInputs As UShort, ByRef values() As Byte)
            values = WriteSyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadHoldingRegister), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read input registers from slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        Public Sub ReadInputRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numInputs As UShort)
            WriteAsyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadInputRegister), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read input registers from slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="values">Contains the result of function.</param>
        Public Sub ReadInputRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numInputs As UShort, ByRef values() As Byte)
            values = WriteSyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadInputRegister), id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write single coil in slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="OnOff">Specifys if the coil should be switched on or off.</param>
        Public Sub WriteSingleCoils(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal OnOff As Boolean)
            Dim data() As Byte
            data = CreateWriteHeader(id, unit, startAddress, 1, 1, fctWriteSingleCoil)
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
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="OnOff">Specifys if the coil should be switched on or off.</param>
        ''' <param name="result">Contains the result of the synchronous write.</param>
        Public Sub WriteSingleCoils(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal OnOff As Boolean, ByRef result() As Byte)
            Dim data() As Byte
            data = CreateWriteHeader(id, unit, startAddress, 1, 1, fctWriteSingleCoil)
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
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numBits">Specifys number of bits.</param>
        ''' <param name="values">Contains the bit information in byte format.</param>
        Public Sub WriteMultipleCoils(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numBits As UShort, ByVal values() As Byte)
            Dim numBytes As Byte = Convert.ToByte(values.Length)
            Dim data() As Byte
            data = CreateWriteHeader(id, unit, startAddress, numBits, CByte(numBytes + 2), fctWriteMultipleCoils)
            Array.Copy(values, 0, data, 13, numBytes)
            WriteAsyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write multiple coils in slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address from where the data read begins.</param>
        ''' <param name="numBits">Specifys number of bits.</param>
        ''' <param name="values">Contains the bit information in byte format.</param>
        ''' <param name="result">Contains the result of the synchronous write.</param>
        Public Sub WriteMultipleCoils(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numBits As UShort, ByVal values() As Byte, ByRef result() As Byte)
            Dim numBytes As Byte = Convert.ToByte(values.Length)
            Dim data() As Byte
            data = CreateWriteHeader(id, unit, startAddress, numBits, CByte(numBytes + 2), fctWriteMultipleCoils)
            Array.Copy(values, 0, data, 13, numBytes)
            result = WriteSyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write single register in slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        Public Sub WriteSingleRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal values() As Byte)
            Dim data() As Byte
            data = CreateWriteHeader(id, unit, startAddress, 1, 1, fctWriteSingleRegister)
            data(10) = values(0)
            data(11) = values(1)
            WriteAsyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write single register in slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        ''' <param name="result">Contains the result of the synchronous write.</param>
        Public Sub WriteSingleRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal values() As Byte, ByRef result() As Byte)
            Dim data() As Byte
            data = CreateWriteHeader(id, unit, startAddress, 1, 1, fctWriteSingleRegister)
            data(10) = values(0)
            data(11) = values(1)
            result = WriteSyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write multiple registers in slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        Public Sub WriteMultipleRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal values() As Byte)
            Dim numBytes As UShort = Convert.ToUInt16(values.Length)
            If numBytes Mod 2 > 0 Then
                numBytes += CUShort(1)
            End If
            Dim data() As Byte

            data = CreateWriteHeader(id, unit, startAddress, Convert.ToUInt16(numBytes \ 2), Convert.ToUInt16(numBytes + 2), fctWriteMultipleRegister)
            Array.Copy(values, 0, data, 13, values.Length)
            WriteAsyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Write multiple registers in slave synchronous.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        ''' <param name="result">Contains the result of the synchronous write.</param>
        Public Sub WriteMultipleRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal values() As Byte, ByRef result() As Byte)
            Dim numBytes As UShort = Convert.ToUInt16(values.Length)
            If numBytes Mod 2 > 0 Then
                numBytes += CUShort(1)
            End If
            Dim data() As Byte

            data = CreateWriteHeader(id, unit, startAddress, Convert.ToUInt16(numBytes \ 2), Convert.ToUInt16(numBytes + 2), fctWriteMultipleRegister)
            Array.Copy(values, 0, data, 13, values.Length)
            result = WriteSyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read/Write multiple registers in slave asynchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startReadAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="startWriteAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        Public Sub ReadWriteMultipleRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startReadAddress As UShort, ByVal numInputs As UShort, ByVal startWriteAddress As UShort, ByVal values() As Byte)
            Dim numBytes As UShort = Convert.ToUInt16(values.Length)
            If numBytes Mod 2 > 0 Then
                numBytes += CUShort(1)
            End If
            Dim data() As Byte

            data = CreateReadWriteHeader(id, unit, startReadAddress, numInputs, startWriteAddress, Convert.ToUInt16(numBytes \ 2))
            Array.Copy(values, 0, data, 17, values.Length)
            WriteAsyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ''' <summary>Read/Write multiple registers in slave synchronous. The result is given in the response function.</summary>
        ''' <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        ''' <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        ''' <param name="startReadAddress">Address from where the data read begins.</param>
        ''' <param name="numInputs">Length of data.</param>
        ''' <param name="startWriteAddress">Address to where the data is written.</param>
        ''' <param name="values">Contains the register information.</param>
        ''' <param name="result">Contains the result of the synchronous command.</param>
        Public Sub ReadWriteMultipleRegister(ByVal id As UShort, ByVal unit As Byte, ByVal startReadAddress As UShort, ByVal numInputs As UShort, ByVal startWriteAddress As UShort, ByVal values() As Byte, ByRef result() As Byte)
            Dim numBytes As UShort = Convert.ToUInt16(values.Length)
            If numBytes Mod 2 > 0 Then
                numBytes += CUShort(1)
            End If
            Dim data() As Byte

            data = CreateReadWriteHeader(id, unit, startReadAddress, numInputs, startWriteAddress, Convert.ToUInt16(numBytes \ 2))
            Array.Copy(values, 0, data, 17, values.Length)
            result = WriteSyncData(data, id)
        End Sub

        ' ------------------------------------------------------------------------
        ' Create modbus header for read action
        Private Function CreateReadHeader(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal length As UShort, ByVal [function] As Byte) As Byte()
            Dim data(11) As Byte

            Dim _id() As Byte = BitConverter.GetBytes(CShort(id))
            data(0) = _id(1) ' Slave id high byte
            data(1) = _id(0) ' Slave id low byte
            data(5) = 6 ' Message size
            data(6) = unit ' Slave address
            data(7) = [function] ' Function code
            Dim _adr() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(startAddress))))
            data(8) = _adr(0) ' Start address
            data(9) = _adr(1) ' Start address
            Dim _length() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(length))))
            data(10) = _length(0) ' Number of data to read
            data(11) = _length(1) ' Number of data to read
            Return data
        End Function

        ' ------------------------------------------------------------------------
        ' Create modbus header for write action
        Private Function CreateWriteHeader(ByVal id As UShort, ByVal unit As Byte, ByVal startAddress As UShort, ByVal numData As UShort, ByVal numBytes As UShort, ByVal [function] As Byte) As Byte()
            Dim data((numBytes + 11) - 1) As Byte

            Dim _id() As Byte = BitConverter.GetBytes(CShort(id))
            data(0) = _id(1) ' Slave id high byte
            data(1) = _id(0) ' Slave id low byte
            Dim _size() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(5 + numBytes))))
            data(4) = _size(0) ' Complete message size in bytes
            data(5) = _size(1) ' Complete message size in bytes
            data(6) = unit ' Slave address
            data(7) = [function] ' Function code
            Dim _adr() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(startAddress))))
            data(8) = _adr(0) ' Start address
            data(9) = _adr(1) ' Start address
            If [function] >= fctWriteMultipleCoils Then
                Dim _cnt() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(numData))))
                data(10) = _cnt(0) ' Number of bytes
                data(11) = _cnt(1) ' Number of bytes
                data(12) = CByte(numBytes - 2)
            End If
            Return data
        End Function

        ' ------------------------------------------------------------------------
        ' Create modbus header for read/write action
        Private Function CreateReadWriteHeader(ByVal id As UShort, ByVal unit As Byte, ByVal startReadAddress As UShort, ByVal numRead As UShort, ByVal startWriteAddress As UShort, ByVal numWrite As UShort) As Byte()
            Dim data((numWrite * 2 + 17) - 1) As Byte

            Dim _id() As Byte = BitConverter.GetBytes(CShort(id))
            data(0) = _id(1) ' Slave id high byte
            data(1) = _id(0) ' Slave id low byte
            Dim _size() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(11 + numWrite * 2))))
            data(4) = _size(0) ' Complete message size in bytes
            data(5) = _size(1) ' Complete message size in bytes
            data(6) = unit ' Slave address
            data(7) = fctReadWriteMultipleRegister ' Function code
            Dim _adr_read() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(startReadAddress))))
            data(8) = _adr_read(0) ' Start read address
            data(9) = _adr_read(1) ' Start read address
            Dim _cnt_read() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(numRead))))
            data(10) = _cnt_read(0) ' Number of bytes to read
            data(11) = _cnt_read(1) ' Number of bytes to read
            Dim _adr_write() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(startWriteAddress))))
            data(12) = _adr_write(0) ' Start write address
            data(13) = _adr_write(1) ' Start write address
            Dim _cnt_write() As Byte = BitConverter.GetBytes(CShort(IPAddress.HostToNetworkOrder(CShort(numWrite))))
            data(14) = _cnt_write(0) ' Number of bytes to write
            data(15) = _cnt_write(1) ' Number of bytes to write
            data(16) = CByte(numWrite * 2)

            Return data
        End Function

        ' ------------------------------------------------------------------------
        ' Write asynchronous data
        Private Sub WriteAsyncData(ByVal write_data() As Byte, ByVal id As UShort)
            If (tcpAsyCl IsNot Nothing) AndAlso (tcpAsyCl.Connected) Then
                Try
                    RaiseEvent OnSendData(id, write_data)

                    tcpAsyCl.BeginSend(write_data, 0, write_data.Length, SocketFlags.None, New AsyncCallback(AddressOf OnSend), Nothing)
                    tcpAsyCl.BeginReceive(tcpAsyClBuffer, 0, tcpAsyClBuffer.Length, SocketFlags.None, New AsyncCallback(AddressOf OnReceive), tcpAsyCl)
                Catch e1 As SystemException
                    CallException(id, write_data(6), write_data(7), excExceptionConnectionLost)
                End Try
            Else
                CallException(id, write_data(6), write_data(7), excExceptionConnectionLost)
            End If
        End Sub

        ' ------------------------------------------------------------------------
        ' Write asynchronous data acknowledge
        Private Sub OnSend(ByVal result As System.IAsyncResult)
            If result.IsCompleted = False Then
                CallException(&HFFFF, &HFF, &HFF, excSendFailt)
            End If
        End Sub

        ' ------------------------------------------------------------------------
        ' Write asynchronous data response
        Private Sub OnReceive(ByVal result As System.IAsyncResult)
            If result.IsCompleted = False Then
                CallException(&HFF, &HFF, &HFF, excExceptionConnectionLost)
            End If

            Dim id As UShort = SwapUInt16(BitConverter.ToUInt16(tcpAsyClBuffer, 0))
            Dim unit As Byte = tcpAsyClBuffer(6)
            Dim [function] As Byte = tcpAsyClBuffer(7)
            Dim data() As Byte

            ' ------------------------------------------------------------
            ' Write response data
            If ([function] >= fctWriteSingleCoil) AndAlso ([function] <> fctReadWriteMultipleRegister) Then
                data = New Byte(1) {}
                Array.Copy(tcpAsyClBuffer, 10, data, 0, 2)
                ' ------------------------------------------------------------
                ' Read response data
            Else
                data = New Byte(tcpAsyClBuffer(8) - 1) {}
                Array.Copy(tcpAsyClBuffer, 9, data, 0, tcpAsyClBuffer(8))
            End If
            ' ------------------------------------------------------------
            ' Response data is slave exception
            If [function] > excExceptionOffset Then
                [function] -= excExceptionOffset
                CallException(id, unit, [function], tcpAsyClBuffer(8))
                ' ------------------------------------------------------------
                ' Response data is regular data
            ElseIf OnResponseDataEvent IsNot Nothing Then
                RaiseEvent OnResponseData(id, unit, [function], data)
            End If
        End Sub

        ' ------------------------------------------------------------------------
        ' Write data and and wait for response
        Private Function WriteSyncData(ByVal write_data() As Byte, ByVal id As UShort) As Byte()

            If tcpSynCl.Connected Then
                Try
                    tcpSynCl.Send(write_data, 0, write_data.Length, SocketFlags.None)
                    Dim result As Integer = tcpSynCl.Receive(tcpSynClBuffer, 0, tcpSynClBuffer.Length, SocketFlags.None)

                    Dim unit As Byte = tcpSynClBuffer(6)
                    Dim [function] As Byte = tcpSynClBuffer(7)
                    Dim data() As Byte

                    If result = 0 Then
                        CallException(id, unit, write_data(7), excExceptionConnectionLost)
                    End If

                    ' ------------------------------------------------------------
                    ' Response data is slave exception
                    If [function] > excExceptionOffset Then
                        [function] -= excExceptionOffset
                        CallException(id, unit, [function], tcpSynClBuffer(8))
                        Return Nothing
                        ' ------------------------------------------------------------
                        ' Write response data
                    ElseIf ([function] >= fctWriteSingleCoil) AndAlso ([function] <> fctReadWriteMultipleRegister) Then
                        data = New Byte(1) {}
                        Array.Copy(tcpSynClBuffer, 10, data, 0, 2)
                        ' ------------------------------------------------------------
                        ' Read response data
                    Else
                        data = New Byte(tcpSynClBuffer(8) - 1) {}
                        Array.Copy(tcpSynClBuffer, 9, data, 0, tcpSynClBuffer(8))
                    End If
                    Return data
                Catch e1 As SystemException
                    CallException(id, write_data(6), write_data(7), excExceptionConnectionLost)
                End Try
            Else
                CallException(id, write_data(6), write_data(7), excExceptionConnectionLost)
            End If
            Return Nothing
        End Function
    End Class
End Namespace

