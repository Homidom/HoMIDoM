Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.ComponentModel
Imports Lextm.SharpSnmpLib
Imports Lextm.SharpSnmpLib.Messaging
Imports System.Net
Imports System.Net.Sockets

' Auteur : Marc Froidevaux
' Date : 16.12.2012

''' <summary>Class Driver_Teracom, permet de communiquer avec les boîtiers Teracom TCW</summary>
''' <remarks>Nécessite l'installation des pilotes fournis sur le site </remarks>
<Serializable()> Public Class Driver_Teracom
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "220B6A0C-2F6C-11E2-8C2F-4A186188709B"
    Dim _Nom As String = "Teracom"
    Dim _Enable As String = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver pour boîtiers Teracom TCW" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "ETHERNET" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "@" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "Teracom" 'Modèle du driver/interface
    Dim _Version As String = My.Application.Info.Version.ToString 'Version du driver
    Dim _OsPlatform As String = "3264" 'Plateforme compatible 32 64 ou 3264
    Dim _Picture As String = "teracom.png" 'Image du driver (non utilisé actuellement)
    Dim _Server As HoMIDom.HoMIDom.Server 'Objet Reflètant le serveur
    Dim _DeviceSupport As New ArrayList 'Type de Device supporté par le driver
    Dim _Device As HoMIDom.HoMIDom.Device 'Image reflétant un device
    Dim _Parametres As New ArrayList 'Paramètres supplémentaires associés au driver
    Dim _LabelsDriver As New ArrayList 'Libellés, tooltip associés au driver
    Dim _LabelsDevice As New ArrayList 'Libellés, tooltip des devices associés au driver
    Dim MyTimer As New Timers.Timer 'Timer du driver
    Dim _IdSrv As String 'Id du Serveur (pour autoriser à utiliser des commandes)
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande) 'Liste des commandes avancées du driver
    Dim _AutoDiscover As Boolean = False

#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes
    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _AFFICHELOG As Boolean = False

    Enum LogLevel
        LogLevel_None
        LogLevel_Always
        LogLevel_Fatal
        LogLevel_Error
        LogLevel_Warning
        LogLevel_Alert
        LogLevel_Info
        LogLevel_Detail
        LogLevel_Debug
        LogLevel_Internal
    End Enum

    Private Const NbControls As Integer = 38
    Dim ControlType(NbControls) As String
    Dim ValueType(NbControls) As String

#End Region

#Region "Propriétés génériques"
    ''' <summary>
    ''' Evènement déclenché par le driver au serveur
    ''' </summary>
    ''' <param name="DriveName"></param>
    ''' <param name="TypeEvent"></param>
    ''' <param name="Parametre"></param>
    ''' <remarks></remarks>
    Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements HoMIDom.HoMIDom.IDriver.DriverEvent

    ''' <summary>
    ''' ID du serveur
    ''' </summary>
    ''' <value>ID du serveur</value>
    ''' <remarks>Permet d'accéder aux commandes du serveur pour lesquels il faut passer l'ID du serveur</remarks>
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
        End Set
    End Property

    ''' <summary>
    ''' Port COM du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' Retourne la liste des devices supportés par le driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Voir Sub New</remarks>
    Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
        Get
            Return _DeviceSupport
        End Get
    End Property

    ''' <summary>
    ''' Liste des paramètres avancés du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Voir Sub New</remarks>
    Public Property Parametres() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.Parametres
        Get
            Return _Parametres
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _Parametres = value
        End Set
    End Property

    ''' <summary>
    ''' Liste les libellés et tooltip des champs associés au driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LabelsDriver() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDriver
        Get
            Return _LabelsDriver
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDriver = value
        End Set
    End Property

    ''' <summary>
    ''' Liste les libellés et tooltip des champs associés au device associé au driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LabelsDevice() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDevice
        Get
            Return _LabelsDevice
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDevice = value
        End Set
    End Property

    ''' <summary>
    ''' Active/Désactive le driver
    ''' </summary>
    ''' <value>True si actif</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Enable() As Boolean Implements HoMIDom.HoMIDom.IDriver.Enable
        Get
            Return _Enable
        End Get
        Set(ByVal value As Boolean)
            _Enable = value
        End Set
    End Property

    ''' <summary>
    ''' ID du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ID() As String Implements HoMIDom.HoMIDom.IDriver.ID
        Get
            Return _ID
        End Get
    End Property

    ''' <summary>
    ''' Adresse IP TCP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IP_TCP() As String Implements HoMIDom.HoMIDom.IDriver.IP_TCP
        Get
            Return _IP_TCP
        End Get
        Set(ByVal value As String)
            _IP_TCP = value
        End Set
    End Property

    ''' <summary>
    ''' Adresse IP UDP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IP_UDP() As String Implements HoMIDom.HoMIDom.IDriver.IP_UDP
        Get
            Return _IP_UDP
        End Get
        Set(ByVal value As String)
            _IP_UDP = value
        End Set
    End Property

    ''' <summary>
    ''' Permet de savoir si le driver est actif
    ''' </summary>
    ''' <value>Retourne True si le driver est démarré</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsConnect() As Boolean Implements HoMIDom.HoMIDom.IDriver.IsConnect
        Get
            Return _IsConnect
        End Get
    End Property

    ''' <summary>
    ''' Modèle du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
        Get
            Return _Modele
        End Get
        Set(ByVal value As String)
            _Modele = value
        End Set
    End Property

    ''' <summary>
    ''' Nom du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Nom() As String Implements HoMIDom.HoMIDom.IDriver.Nom
        Get
            Return _Nom
        End Get
    End Property

    ''' <summary>
    ''' Image du driver (non utilisé)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Picture() As String Implements HoMIDom.HoMIDom.IDriver.Picture
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property

    ''' <summary>
    ''' Port TCP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port_TCP() As String Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
            _Port_TCP = value
        End Set
    End Property

    ''' <summary>
    ''' Port UDP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port_UDP() As String Implements HoMIDom.HoMIDom.IDriver.Port_UDP
        Get
            Return _Port_UDP
        End Get
        Set(ByVal value As String)
            _Port_UDP = value
        End Set
    End Property

    ''' <summary>
    ''' Type de protocole utilisé par le driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Protocol() As String Implements HoMIDom.HoMIDom.IDriver.Protocol
        Get
            Return _Protocol
        End Get
    End Property

    ''' <summary>
    ''' Valeur de rafraichissement des devices
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property

    ''' <summary>
    ''' Objet représentant le serveur
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property

    ''' <summary>
    ''' Version du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property

    ''' <summary>
    ''' True si le driver doit démarrer automatiquement
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property
    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
        End Get
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

    ''' <summary>Retourne la liste des Commandes avancées de type DeviceCommande</summary>
    ''' <remarks></remarks>
    Public Function GetCommandPlus() As List(Of DeviceCommande)
        Return _DeviceCommandPlus
    End Function

    ''' <summary>Execute une commande avancée</summary>
    ''' <param name="Objet">Objet représentant le Device </param>
    ''' <param name="Command">Nom de la commande avancée à éxécuter</param>
    ''' <param name="Param">tableau de paramétres</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal Objet As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False
        Try
            If Objet IsNot Nothing Then

                Dim myHost As String = Objet.Adresse1.ToString
                Dim myPort As String = Objet.Adresse2.ToString
                Dim myControl As String = Objet.Modele.ToString
                Dim myControlNum As Integer = 0

                If (myHost = "") Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand ", "Erreur: l'adresse IP du device (Adresse2) n'est pas définie.")
                    ExecuteCommand = False
                    Exit Function
                End If
                If (myPort = "") Then
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " ExecuteCommand ", "Info: port SNMP du device non spécifié (Adresse2), utilisation du port TCP 161.")
                End If

                Select Case Command
                    Case "SET"
                        For i As Integer = 1 To NbControls
                            If myControl = ControlType(i) Then
                                myControlNum = i
                                Exit For
                            End If
                        Next
                        If (myControlNum = 0) Then
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand ", "Erreur: le composant n'est pas d'un type défini!")
                            ExecuteCommand = False
                            Exit Function
                        End If

                        Dim myTCW As Teracom_TCW
                        myTCW = New Teracom_TCW
                        myTCW.Host = myHost
                        myTCW.Port = myPort
                        myTCW.Control = myControlNum

                        Select Case myControlNum
                            Case 27
                                'ControlType(27) = "TCW12x_TEMP1_MIN"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_1
                                myTCW.Min = Param(0)
                                myTCW.SetMinMaxHyst(True, False, False)
                            Case 28
                                'ControlType(28) = "TCW12x_TEMP1_MAX"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_1
                                myTCW.Max = Param(0)
                                myTCW.SetMinMaxHyst(False, True, False)
                            Case 29
                                'ControlType(29) = "TCW12x_TEMP1_HYST"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_1
                                myTCW.Hyst = Param(0)
                                myTCW.SetMinMaxHyst(False, False, True)
                            Case 30
                                'ControlType(30) = "TCW12x_HUMID1_MIN"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_1
                                myTCW.Min = Param(0)
                                myTCW.SetMinMaxHyst(True, False, False)
                            Case 31
                                'ControlType(31) = "TCW12x_HUMID1_MAX"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_1
                                myTCW.Max = Param(0)
                                myTCW.SetMinMaxHyst(False, True, False)
                            Case 32
                                'ControlType(32) = "TCW12x_HUMID1_HYST"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_1
                                myTCW.Hyst = Param(0)
                                myTCW.SetMinMaxHyst(False, False, True)
                            Case 33
                                'ControlType(33) = "TCW12x_TEMP2_MIN"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                                myTCW.Min = Param(0)
                                myTCW.SetMinMaxHyst(True, False, False)
                            Case 34
                                'ControlType(34) = "TCW12x_TEMP2_MAX"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                                myTCW.Max = Param(0)
                                myTCW.SetMinMaxHyst(False, True, False)
                            Case 35
                                'ControlType(35) = "TCW12x_TEMP2_HYST"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                                myTCW.Hyst = Param(0)
                                myTCW.SetMinMaxHyst(False, False, True)
                            Case 36
                                'ControlType(36) = "TCW12x_HUMID2_MIN"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_2
                                myTCW.Min = Param(0)
                                myTCW.SetMinMaxHyst(True, False, False)
                            Case 37
                                'ControlType(37) = "TCW12x_HUMID2_MAX"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_2
                                myTCW.Max = Param(0)
                                myTCW.SetMinMaxHyst(False, True, False)
                            Case 38
                                'ControlType(38) = "TCW12x_HUMID2_HYST"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_2
                                myTCW.Hyst = Param(0)
                                myTCW.SetMinMaxHyst(False, False, True)
                            Case Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & "ExecuteCommand ", "Commande impossible pour ce type de contrôle.")
                                ExecuteCommand = False
                                Exit Function
                        End Select
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand ", "Commande SET exécutée pour " & ControlType(myControlNum) & " avec paramètre: " & Param(0).ToString)
                        myTCW = Nothing
                        ' Mise à jour de la valeur par relecture du device
                        Read(Objet)
                    Case "SAVE_SETTINGS"
                        For i As Integer = 1 To NbControls
                            If myControl = ControlType(i) Then
                                myControlNum = i
                                Exit For
                            End If
                        Next
                        If (myControlNum = 0) Then
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand ", "Erreur: le composant n'est pas d'un type défini!")
                            ExecuteCommand = False
                            Exit Function
                        End If

                        Dim myTCW As Teracom_TCW
                        myTCW = New Teracom_TCW
                        myTCW.Host = myHost
                        myTCW.Port = myPort
                        myTCW.Control = myControlNum
                        myTCW.SaveSettings()
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand ", "Commande SAVE_SETTINGS exécutée pour " & ControlType(myControlNum))
                    Case Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand ", "La commande " & Command & " n'existe pas")
                End Select
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "exception : " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>Permet de vérifier si un champ est valide</summary>
    ''' <param name="Champ">Nom du champ à vérifier, ex ADRESSE1</param>
    ''' <param name="Value">Valeur à vérifier</param>
    ''' <returns>Retourne 0 si OK, sinon un message d'erreur</returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1"

                Case "ADRESSE2"

            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _AFFICHELOG = Parametres.Item(1).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teracom Start", "Erreur dans les paramétres avancés. utilisation des valeurs par défaut" & ex.Message)
            End Try


            _IsConnect = True
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Driver " & Me.Nom & " démarré")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    ''' <summary>Intérroger un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <remarks>Le device demande au driver d'aller le lire suivant son adresse</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read

        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read ", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read ", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté")
                Exit Sub
            End If

            Dim myHost As String = Objet.Adresse1.ToString
            Dim myPort As String = Objet.Adresse2.ToString
            Dim myControl As String = Objet.Modele.ToString
            Dim myControlNum As Integer = 0
            If (myHost = "") Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read ", "Erreur: l'adresse IP du device (Adresse2) n'est pas définie.")
                Exit Sub
            End If
            If (myPort = "") Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Read ", "Info: port SNMP du device non spécifié (Adresse2), utilisation du port TCP 161.")
            End If

            If Objet IsNot Nothing Then
                For i As Integer = 1 To NbControls
                    If myControl = ControlType(i) Then
                        myControlNum = i
                        Exit For
                    End If
                Next
                If (myControlNum = 0) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read ", "Erreur: le composant n'est pas d'un type défini!")
                    Exit Sub
                End If

                Dim myTCW As Teracom_TCW
                myTCW = New Teracom_TCW
                myTCW.Host = myHost
                myTCW.Port = myPort
                myTCW.Control = myControlNum

                Dim result As String
                Select Case myControlNum
                    Case 27
                        'ControlType(27) = "TCW12x_TEMP1_MIN"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_1
                        myTCW.GetMinMaxHyst(True, False, False)
                        result = myTCW.Min
                    Case 28
                        'ControlType(28) = "TCW12x_TEMP1_MAX"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_1
                        myTCW.GetMinMaxHyst(False, True, False)
                        result = myTCW.Max
                    Case 29
                        'ControlType(29) = "TCW12x_TEMP1_HYST"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_1
                        myTCW.GetMinMaxHyst(False, False, True)
                        result = myTCW.Hyst
                    Case 30
                        'ControlType(30) = "TCW12x_HUMID1_MIN"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_1
                        myTCW.GetMinMaxHyst(True, False, False)
                        result = myTCW.Min
                    Case 31
                        'ControlType(31) = "TCW12x_HUMID1_MAX"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_1
                        myTCW.GetMinMaxHyst(False, True, False)
                        result = myTCW.Max
                    Case 32
                        'ControlType(32) = "TCW12x_HUMID1_HYST"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_1
                        myTCW.GetMinMaxHyst(False, False, True)
                        result = myTCW.Hyst
                    Case 33
                        'ControlType(33) = "TCW12x_TEMP2_MIN"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                        myTCW.GetMinMaxHyst(True, False, False)
                        result = myTCW.Min
                    Case 34
                        'ControlType(34) = "TCW12x_TEMP2_MAX"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                        myTCW.GetMinMaxHyst(False, True, False)
                        result = myTCW.Max
                    Case 35
                        'ControlType(35) = "TCW12x_TEMP2_HYST"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                        myTCW.GetMinMaxHyst(False, False, True)
                        result = myTCW.Hyst
                    Case 36
                        'ControlType(36) = "TCW12x_HUMID2_MIN"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_2
                        myTCW.GetMinMaxHyst(True, False, False)
                        result = myTCW.Min
                    Case 37
                        'ControlType(37) = "TCW12x_HUMID2_MAX"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_2
                        myTCW.GetMinMaxHyst(False, True, False)
                        result = myTCW.Max
                    Case 38
                        'ControlType(38) = "TCW12x_HUMID2_HYST"
                        myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_2
                        myTCW.GetMinMaxHyst(False, False, True)
                        result = myTCW.Hyst
                    Case Else
                        result = myTCW.GetValue()
                End Select

                If InStr(result, "timeout") > 0 Then
                    If _DEBUG Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read ", "Le périphérique n'a pas répondu pour " & Objet.modele)
                    End If
                    Exit Sub
                End If
                Select Case ValueType(myControlNum)
                    Case "Boolean"
                        If result = 0 Then
                            Objet.value = False
                        Else
                            Objet.value = True
                        End If
                    Case "Single"
                        Objet.value = result
                End Select
                myTCW = Nothing
                If _DEBUG Then
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Read ", Objet.modele & ": " & result)
                End If
            Else
                If _DEBUG Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read ", "Pas de valeur trouvée pour " & Objet.modele)
                End If
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read ", ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à commander</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1">parametre 1 de la commande, optionnel</param>
    ''' <param name="Parametre2">parametre 2 de la commande, optionnel</param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write ", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write ", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté")
                Exit Sub
            End If

            Dim myHost As String = Objet.Adresse1.ToString
            Dim myPort As String = Objet.Adresse2.ToString
            Dim myControl As String = Objet.Modele.ToString
            Dim myControlNum As Integer = 0
            If (myHost = "") Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write ", "Erreur: l'adresse IP du device (Adresse2) n'est pas définie.")
                Exit Sub
            End If
            If (myPort = "") Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Write ", "Info: port SNMP du device non spécifié (Adresse2), utilisation du port TCP 161.")
            End If

            If Objet IsNot Nothing Then
                For i As Integer = 1 To NbControls
                    If myControl = ControlType(i) Then
                        myControlNum = i
                        Exit For
                    End If
                Next
                If (myControlNum = 0) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write ", "Erreur: le composant n'est pas d'un type défini!")
                    Exit Sub
                End If

                Dim myTCW As Teracom_TCW
                myTCW = New Teracom_TCW
                myTCW.Host = myHost
                myTCW.Port = myPort
                myTCW.Control = myControlNum

                Dim result As String = ""

                Select Case ValueType(myControlNum)
                    Case "Boolean"
                        If UCase(Command) = "ON" Then
                            result = myTCW.SetValue("1")
                        End If
                        If UCase(Command) = "OFF" Then
                            result = myTCW.SetValue("0")
                        End If
                        If result = "1" Then
                            Objet.value = True
                        ElseIf result = "0" Then
                            Objet.value = False
                        Else
                            If _DEBUG Then
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write ", "Erreur: " & result)
                            End If
                            Exit Sub
                        End If
                    Case "Single"
                        Select Case myControlNum
                            Case 27
                                'ControlType(27) = "TCW12x_TEMP1_MIN"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_1
                                myTCW.Min = Parametre1
                                myTCW.SetMinMaxHyst(True, False, False)
                            Case 28
                                'ControlType(28) = "TCW12x_TEMP1_MAX"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_1
                                myTCW.Max = Parametre1
                                myTCW.SetMinMaxHyst(False, True, False)
                            Case 29
                                'ControlType(29) = "TCW12x_TEMP1_HYST"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_1
                                myTCW.Hyst = Parametre1
                                myTCW.SetMinMaxHyst(False, False, True)
                            Case 30
                                'ControlType(30) = "TCW12x_HUMID1_MIN"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_1
                                myTCW.Min = Parametre1
                                myTCW.SetMinMaxHyst(True, False, False)
                            Case 31
                                'ControlType(31) = "TCW12x_HUMID1_MAX"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_1
                                myTCW.Max = Parametre1
                                myTCW.SetMinMaxHyst(False, True, False)
                            Case 32
                                'ControlType(32) = "TCW12x_HUMID1_HYST"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_1
                                myTCW.Hyst = Parametre1
                                myTCW.SetMinMaxHyst(False, False, True)
                            Case 33
                                'ControlType(33) = "TCW12x_TEMP2_MIN"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                                myTCW.Min = Parametre1
                                myTCW.SetMinMaxHyst(True, False, False)
                            Case 34
                                'ControlType(34) = "TCW12x_TEMP2_MAX"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                                myTCW.Max = Parametre1
                                myTCW.SetMinMaxHyst(False, True, False)
                            Case 35
                                'ControlType(35) = "TCW12x_TEMP2_HYST"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                                myTCW.Hyst = Parametre1
                                myTCW.SetMinMaxHyst(False, False, True)
                            Case 36
                                'ControlType(36) = "TCW12x_HUMID2_MIN"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_2
                                myTCW.Min = Parametre1
                                myTCW.SetMinMaxHyst(True, False, False)
                            Case 37
                                'ControlType(37) = "TCW12x_HUMID2_MAX"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_2
                                myTCW.Max = Parametre1
                                myTCW.SetMinMaxHyst(False, True, False)
                            Case 38
                                'ControlType(38) = "TCW12x_HUMID2_HYST"
                                myTCW.Control = Teracom_TCW.ControlTypes.TYPE_TCW12x_HUMIDITY_2
                                myTCW.Hyst = Parametre1
                                myTCW.SetMinMaxHyst(False, False, True)
                            Case Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write ", "Commande impossible pour ce type de contrôle.")
                                Exit Sub
                        End Select
                        'myTCW.SaveSettings()
                    Case Else
                        result = "Erreur: le composant n'est pas commandable."
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write ", "Erreur: le composant n'est pas un relais commandable!")
                End Select
                myTCW = Nothing
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Write ", Objet.modele & ": " & result)
            Else
                If _DEBUG Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write ", "Pas de valeur trouvée")
                End If
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write ", ex.Message)
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
    ''' <param name="nom">Nom de la commande avancée</param>
    ''' <param name="description">Description qui sera affichée dans l'admin</param>
    ''' <param name="nbparam">Nombre de parametres attendus</param>
    ''' <remarks></remarks>
    Private Sub Add_DeviceCommande(ByVal Nom As String, ByVal Description As String, ByVal NbParam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = Nom
            x.DescriptionCommand = Description
            x.CountParam = NbParam
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
    Private Sub Add_ParamAvance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
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

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.SWITCH)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.DETECTEUR)
            _DeviceSupport.Add(ListeDevices.CONTACT)
            _DeviceSupport.Add(ListeDevices.APPAREIL)
            _DeviceSupport.Add(ListeDevices.LAMPE)
            _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE)

            'Paramétres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("AfficheLog", "Afficher Log à l'écran (True/False)", True)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            Add_DeviceCommande("SET", "Paramètre: valeur Min/Max/Hyst à envoyer", 1)
            Add_DeviceCommande("SAVE_SETTINGS", "Sauvegarde des réglages résistante au reboot du boîtier", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Ce module permet de contrôler un composant d'un boîtier Teracom. Attention, le mode SNMP doit être activé dans le boîtier!")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse IP", "Adresse IP ou nom d'hôte du boîtier Teracom")
            Add_LibelleDevice("ADRESSE2", "Port SNMP", "Port SNMP utilisé pour accéder au boîtier")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "Contrôle", "Composant du boîtier à interroger", "TCW12x_TEMPERATURE_1|TCW12x_TEMPERATURE_2|TCW12x_HUMIDITY_1|TCW12x_HUMIDITY_2|TCW12x_DIGITAL_IN_1|TCW12x_DIGITAL_IN_2|TCW12x_ANALOG_IN_1|TCW12x_ANALOG_IN_2|TCW12x_RELAY_1|TCW12x_RELAY_2|TCW18x_RELAY_1|TCW18x_RELAY_2|TCW18x_RELAY_3|TCW18x_RELAY_4|TCW18x_RELAY_5|TCW18x_RELAY_6|TCW18x_RELAY_7|TCW18x_RELAY_8|TCW18x_DIGITAL_IN|TCW120_TEMPERATURE|TCW120_DIGITAL_IN_1|TCW120_DIGITAL_IN_2|TCW120_ANALOG_IN_1|TCW120_ANALOG_IN_2|TCW120_RELAY_1|TCW120_RELAY_2|TCW12x_TEMP1_MIN|TCW12x_TEMP1_MAX|TCW12x_TEMP1_HYST|TCW12x_HUMID1_MIN|TCW12x_HUMID1_MAX|TCW12x_HUMID1_HYST|TCW12x_TEMP2_MIN|TCW12x_TEMP2_MAX|TCW12x_TEMP2_HYST|TCW12x_HUMID2_MIN|TCW12x_HUMID2_MAX|TCW12x_HUMID2_HYST")
            Add_LibelleDevice("REFRESH", "Refresh", "Intervalle de rafraîchissement des données")
            Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

            ' Initialisation des arrays
            ControlType(1) = "TCW12x_TEMPERATURE_1"
            ControlType(2) = "TCW12x_TEMPERATURE_2"
            ControlType(3) = "TCW12x_HUMIDITY_1"
            ControlType(4) = "TCW12x_HUMIDITY_2"
            ControlType(5) = "TCW12x_DIGITAL_IN_1"
            ControlType(6) = "TCW12x_DIGITAL_IN_2"
            ControlType(7) = "TCW12x_ANALOG_IN_1"
            ControlType(8) = "TCW12x_ANALOG_IN_2"
            ControlType(9) = "TCW12x_RELAY_1"
            ControlType(10) = "TCW12x_RELAY_2"
            ControlType(11) = "TCW18x_RELAY_1"
            ControlType(12) = "TCW18x_RELAY_2"
            ControlType(13) = "TCW18x_RELAY_3"
            ControlType(14) = "TCW18x_RELAY_4"
            ControlType(15) = "TCW18x_RELAY_5"
            ControlType(16) = "TCW18x_RELAY_6"
            ControlType(17) = "TCW18x_RELAY_7"
            ControlType(18) = "TCW18x_RELAY_8"
            ControlType(19) = "TCW18x_DIGITAL_IN"
            ControlType(20) = "TCW120_TEMPERATURE"
            ControlType(21) = "TCW120_DIGITAL_IN_1"
            ControlType(22) = "TCW120_DIGITAL_IN_2"
            ControlType(23) = "TCW120_ANALOG_IN_1"
            ControlType(24) = "TCW120_ANALOG_IN_2"
            ControlType(25) = "TCW120_RELAY_1"
            ControlType(26) = "TCW120_RELAY_2"
            ControlType(27) = "TCW12x_TEMP1_MIN"
            ControlType(28) = "TCW12x_TEMP1_MAX"
            ControlType(29) = "TCW12x_TEMP1_HYST"
            ControlType(30) = "TCW12x_HUMID1_MIN"
            ControlType(31) = "TCW12x_HUMID1_MAX"
            ControlType(32) = "TCW12x_HUMID1_HYST"
            ControlType(33) = "TCW12x_TEMP2_MIN"
            ControlType(34) = "TCW12x_TEMP2_MAX"
            ControlType(35) = "TCW12x_TEMP2_HYST"
            ControlType(36) = "TCW12x_HUMID2_MIN"
            ControlType(37) = "TCW12x_HUMID2_MAX"
            ControlType(38) = "TCW12x_HUMID2_HYST"

            ValueType(0) = "Boolean"
            ValueType(1) = "Single"
            ValueType(2) = "Single"
            ValueType(3) = "Single"
            ValueType(4) = "Single"
            ValueType(5) = "Boolean"
            ValueType(6) = "Boolean"
            ValueType(7) = "Single"
            ValueType(8) = "Single"
            ValueType(9) = "Boolean"
            ValueType(10) = "Boolean"
            ValueType(11) = "Boolean"
            ValueType(12) = "Boolean"
            ValueType(13) = "Boolean"
            ValueType(14) = "Boolean"
            ValueType(15) = "Boolean"
            ValueType(16) = "Boolean"
            ValueType(17) = "Boolean"
            ValueType(18) = "Boolean"
            ValueType(19) = "Boolean"
            ValueType(20) = "Single"
            ValueType(21) = "Boolean"
            ValueType(22) = "Boolean"
            ValueType(23) = "Single"
            ValueType(24) = "Single"
            ValueType(25) = "Boolean"
            ValueType(26) = "Boolean"
            ValueType(27) = "Single"
            ValueType(28) = "Single"
            ValueType(29) = "Single"
            ValueType(30) = "Single"
            ValueType(31) = "Single"
            ValueType(32) = "Single"
            ValueType(33) = "Single"
            ValueType(34) = "Single"
            ValueType(35) = "Single"
            ValueType(36) = "Single"
            ValueType(37) = "Single"
            ValueType(38) = "Single"


        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick()

    End Sub

#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver et nom communes (ex: start)
 
#End Region


End Class

' TCW12x/TCW18x/TCW120 Controller Class
' Copyright (C) 2012-2013 Marc Froidevaux - marc@froidevaux.org
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program.  If not, see <http://www.gnu.org/licenses/>.

Friend Class Teracom_TCW

    Private myDesc(27) As String
    Private myHost As String
    Private myPort As Integer
    Private myControl As ControlTypes
    Private myLabel As String
    Private myValue As String
    Private myMin As String
    Private myMax As String
    Private myHyst As String
    Private myPublicCommunity As String
    Private myPrivateCommunity As String
    Private MIB(27) As String
    Private MIBMin(27) As String
    Private MIBMax(27) As String
    Private MIBHyst(27) As String
    Private Const BaseOID As String = "1.3.6.1.4.1.38783."
    Private Const TCW12x_SaveSettings As String = "3.13.0"
    Private Const TCW18x_SaveSettings As String = "6.0"
    Private Const TCW120_SaveSettings As String = "8.0"
    Public Const TYPE_COUNT As Integer = 26

    Public Property Host As String
        Get
            Host = myHost
        End Get
        Set(value As String)
            myHost = value
        End Set
    End Property

    Public Property Port As Integer
        Get
            Port = myPort
        End Get
        Set(value As Integer)
            myPort = value
        End Set
    End Property

    Public Property Control() As ControlTypes
        Get
            Return myControl
        End Get
        Set(value As ControlTypes)
            myControl = value
        End Set
    End Property

    Public Property Description As String
        Get
            Description = myDesc(myControl)
        End Get
        Set(value As String)
        End Set
    End Property

    Public Property Label As String
        Get
            Label = myLabel
        End Get
        Set(value As String)
            myLabel = value
        End Set
    End Property

    Public Property Value As String
        Get
            Value = myValue
        End Get
        Set(value As String)
        End Set
    End Property

    Public Property Min As String
        Get
            Min = myMin
        End Get
        Set(value As String)
            myMin = value
        End Set
    End Property

    Public Property Max As String
        Get
            Max = myMax
        End Get
        Set(value As String)
            myMax = value
        End Set
    End Property

    Public Property Hyst As String
        Get
            Hyst = myHyst
        End Get
        Set(value As String)
            myHyst = value
        End Set
    End Property

    Public Property PublicCommunity As String
        Get
            PublicCommunity = myPublicCommunity
        End Get
        Set(value As String)
            myPublicCommunity = value
        End Set
    End Property

    Public Property PrivateCommunity As String
        Get
            PrivateCommunity = myPrivateCommunity
        End Get
        Set(value As String)
            myPrivateCommunity = value
        End Set
    End Property


    Public Sub New()

        MIB(0) = ""
        MIB(1) = "3.9.0"
        MIB(2) = "3.10.0"
        MIB(3) = "3.11.0"
        MIB(4) = "3.12.0"
        MIB(5) = "3.1.0"
        MIB(6) = "3.2.0"
        MIB(7) = "3.7.0"
        MIB(8) = "3.8.0"
        MIB(9) = "3.3.0"
        MIB(10) = "3.5.0"
        MIB(11) = "3.2.0"
        MIB(12) = "3.3.0"
        MIB(13) = "3.4.0"
        MIB(14) = "3.5.0"
        MIB(15) = "3.6.0"
        MIB(16) = "3.7.0"
        MIB(17) = "3.8.0"
        MIB(18) = "3.9.0"
        MIB(19) = "3.1.0"
        MIB(20) = "3.4.0"
        MIB(21) = "3.3.1.0"
        MIB(22) = "3.3.2.0"
        MIB(23) = "3.2.1.0"
        MIB(24) = "3.2.3.0"
        MIB(25) = "3.1.1.0"
        MIB(26) = "3.1.2.0"

        MIBMin(1) = "2.5.1.1.0"
        MIBMax(1) = "2.5.1.2.0"
        MIBHyst(1) = "2.5.1.3.0"
        MIBMin(2) = "2.6.2.1.0"
        MIBMax(2) = "2.6.2.2.0"
        MIBHyst(2) = "2.6.2.3.0"
        MIBMin(3) = "2.5.2.1.0"
        MIBMax(3) = "2.5.2.2.0"
        MIBHyst(3) = "2.5.2.3.0"
        MIBMin(4) = "2.6.2.1.0"
        MIBMax(4) = "2.6.2.2.0"
        MIBHyst(4) = "2.6.2.3.0"
        MIBMin(7) = "2.7.1.1.0"
        MIBMax(7) = "2.7.1.2.0"
        MIBHyst(7) = "2.7.1.3.0"
        MIBMin(8) = "2.7.2.1.0"
        MIBMax(8) = "2.7.2.2.0"
        MIBHyst(8) = "2.7.2.3.0"
        MIBMin(20) = "2.1.10.1.0"
        MIBMax(20) = "2.1.10.2.0"
        MIBHyst(20) = "0"

        myPort = 161
        myHost = String.Empty

        myDesc(0) = "None"
        myDesc(1) = "TCW12x Temperature Sensor 1"
        myDesc(2) = "TCW12x Temperature Sensor 2"
        myDesc(3) = "TCW12x Humidity Sensor 1"
        myDesc(4) = "TCW12x Humidity Sensor 2"
        myDesc(5) = "TCW12x Digital Input 1"
        myDesc(6) = "TCW12x Digital Input 2"
        myDesc(7) = "TCW12x Analog Input 1"
        myDesc(8) = "TCW12x Analog Input 2"
        myDesc(9) = "TCW12x Relay 1"
        myDesc(10) = "TCW12x Relay 2"
        myDesc(11) = "TCW18x Relay 1"
        myDesc(12) = "TCW18x Relay 2"
        myDesc(13) = "TCW18x Relay 3"
        myDesc(14) = "TCW18x Relay 4"
        myDesc(15) = "TCW18x Relay 5"
        myDesc(16) = "TCW18x Relay 6"
        myDesc(17) = "TCW18x Relay 7"
        myDesc(18) = "TCW18x Relay 8"
        myDesc(19) = "TCW18x Digital Input"
        myDesc(20) = "TCW120 Temperature Sensor"
        myDesc(21) = "TCW120 Digital Input 1"
        myDesc(22) = "TCW120 Digital Input 2"
        myDesc(23) = "TCW120 Analog Input 1"
        myDesc(24) = "TCW120 Analog Input 2"
        myDesc(25) = "TCW120 Relay 1"
        myDesc(26) = "TCW120 Relay 2"



    End Sub

    Public Function GetValue() As String
        Dim result As String
        result = GetSnmp(myHost, BaseOID & MIB(myControl))
        Select Case myControl
            Case ControlTypes.TYPE_TCW12x_ANALOG_IN_1, ControlTypes.TYPE_TCW12x_ANALOG_IN_2, ControlTypes.TYPE_TCW12x_HUMIDITY_1, ControlTypes.TYPE_TCW12x_HUMIDITY_2, ControlTypes.TYPE_TCW12x_TEMPERATURE_1, ControlTypes.TYPE_TCW12x_TEMPERATURE_2
                If Val(result) <> 0 Then
                    result = Format$(Val(result) / 10, "0.0")
                ElseIf result = "0" Then
                    result = "0"
                End If
            Case ControlTypes.TYPE_TCW12x_DIGITAL_IN_1, ControlTypes.TYPE_TCW12x_DIGITAL_IN_2, ControlTypes.TYPE_TCW12x_RELAY_1, ControlTypes.TYPE_TCW12x_RELAY_2, ControlTypes.TYPE_TCW18x_DIGITAL_IN, ControlTypes.TYPE_TCW18x_RELAY_1, ControlTypes.TYPE_TCW18x_RELAY_2, ControlTypes.TYPE_TCW18x_RELAY_3, ControlTypes.TYPE_TCW18x_RELAY_4, ControlTypes.TYPE_TCW18x_RELAY_5, ControlTypes.TYPE_TCW18x_RELAY_6, ControlTypes.TYPE_TCW18x_RELAY_7, ControlTypes.TYPE_TCW18x_RELAY_8, ControlTypes.TYPE_TCW120_DIGITAL_IN_1, ControlTypes.TYPE_TCW120_DIGITAL_IN_2, ControlTypes.TYPE_TCW120_RELAY_1, ControlTypes.TYPE_TCW120_RELAY_2
                If Val(result) = 1 Then
                    result = "1"
                ElseIf result = "0" Then
                    result = "0"
                End If
            Case ControlTypes.TYPE_TCW120_ANALOG_IN_1, ControlTypes.TYPE_TCW120_ANALOG_IN_2, ControlTypes.TYPE_TCW120_TEMPERATURE
                If Val(result) <> 0 Then
                    result = Format$(Val(result), "0.0")
                ElseIf result = "0" Then
                    result = "0"
                End If
            Case Else
                result = String.Empty
        End Select
        GetValue = result
        myValue = result

    End Function


    Public Function SetValue(Value As String) As String
        Dim result As String = String.Empty
        Select Case myControl
            Case ControlTypes.TYPE_TCW12x_RELAY_1, ControlTypes.TYPE_TCW12x_RELAY_2, ControlTypes.TYPE_TCW18x_RELAY_1, ControlTypes.TYPE_TCW18x_RELAY_2, ControlTypes.TYPE_TCW18x_RELAY_3, ControlTypes.TYPE_TCW18x_RELAY_4, ControlTypes.TYPE_TCW18x_RELAY_5, ControlTypes.TYPE_TCW18x_RELAY_6, ControlTypes.TYPE_TCW18x_RELAY_7, ControlTypes.TYPE_TCW18x_RELAY_8, ControlTypes.TYPE_TCW120_RELAY_1, ControlTypes.TYPE_TCW120_RELAY_2
                result = SetSnmp(myHost, BaseOID & MIB(myControl), Value, True)
            Case Else
                result = "Not set (read-only)."
        End Select
        SetValue = result
        myValue = result
    End Function

    Public Function SaveSettings() As String
        Dim result As String = String.Empty
        Select Case myControl
            Case ControlTypes.TYPE_TCW18x_RELAY_1, ControlTypes.TYPE_TCW18x_RELAY_2, ControlTypes.TYPE_TCW18x_RELAY_3, ControlTypes.TYPE_TCW18x_RELAY_4, ControlTypes.TYPE_TCW18x_RELAY_5, ControlTypes.TYPE_TCW18x_RELAY_6, ControlTypes.TYPE_TCW18x_RELAY_7, ControlTypes.TYPE_TCW18x_RELAY_8
                ' TCW18x
                result = SetSnmp(myHost, BaseOID & TCW18x_SaveSettings, Value, True)
            Case ControlTypes.TYPE_TCW120_RELAY_1, ControlTypes.TYPE_TCW120_RELAY_2
                ' TCW120
                result = SetSnmp(myHost, BaseOID & TCW120_SaveSettings, Value, True)
            Case Else
                ' TCW12x
                result = SetSnmp(myHost, BaseOID & TCW12x_SaveSettings, Value, True)
        End Select
        SaveSettings = result
    End Function

    Public Property About As String
        Get
            Return "Teracom TCW controller class by Marc Froidevaux"
        End Get
        Set(value As String)

        End Set
    End Property

    Private Function GetSnmp(ByVal host As String, oid As String) As String
        Dim community As String
        Dim showHelp__1 As Boolean = False
        Dim showVersion As Boolean = False
        Dim version As VersionCode = VersionCode.V1
        Dim timeout As Integer = 1000
        Dim retry As Integer = 0

        If myPublicCommunity = String.Empty Then
            community = "public"
        Else
            community = myPublicCommunity
        End If

        Dim ip As IPAddress
        ip = Nothing
        GetSnmp = Nothing

        Dim parsed As Boolean = IPAddress.TryParse(host, ip)
        If Not parsed Then
            For Each address As IPAddress In Dns.GetHostAddresses(host)
                If address.AddressFamily <> AddressFamily.InterNetwork Then
                    Continue For
                End If

                ip = address
                Exit For
            Next

            If ip Is Nothing Then
                GetSnmp = "invalid host or wrong IP address found: " & host
                Exit Function
            End If
        End If

        Try
            Dim vList As New List(Of Variable)()
            Dim test As New Variable(New ObjectIdentifier(oid))
            vList.Add(test)

            Dim receiver As New IPEndPoint(ip, myPort)
            For Each variable As Variable In Messenger.[Get](version, receiver, New OctetString(community), vList, timeout)
                GetSnmp = variable.Data.ToString
            Next

        Catch ex As SnmpException
            GetSnmp = ex.ToString
        Catch ex As SocketException
            GetSnmp = ex.ToString
        End Try
    End Function

    Private Function SetSnmp(ByVal host As String, oid As String, value As String, ValueInteger As Boolean) As String
        Dim community As String
        Dim showHelp__1 As Boolean = False
        Dim showVersion As Boolean = False
        Dim version As VersionCode = VersionCode.V1
        Dim timeout As Integer = 1000
        Dim retry As Integer = 0

        If myPrivateCommunity = String.Empty Then
            community = "private"
        Else
            community = myPrivateCommunity
        End If

        Dim ip As IPAddress
        ip = Nothing
        SetSnmp = Nothing

        Dim parsed As Boolean = IPAddress.TryParse(host, ip)
        If Not parsed Then
            For Each address As IPAddress In Dns.GetHostAddresses(host)
                If address.AddressFamily <> AddressFamily.InterNetwork Then
                    Continue For
                End If

                ip = address
                Exit For
            Next

            If ip Is Nothing Then
                SetSnmp = "invalid host or wrong IP address found: " & host
                Exit Function
            End If
        End If

        Try
            Dim vList As New List(Of Variable)()
            If ValueInteger = True Then
                Dim test As New Variable(New ObjectIdentifier(oid), New Integer32(CInt(value)))
                vList.Add(test)
            Else
                Dim test As New Variable(New ObjectIdentifier(oid), New OctetString(value))
                vList.Add(test)
            End If

            Dim receiver As New IPEndPoint(ip, myPort)
            For Each variable As Variable In Messenger.[Set](version, receiver, New OctetString(community), vList, timeout)
                SetSnmp = variable.Data.ToString
            Next

        Catch ex As SnmpException
            SetSnmp = ex.ToString
        Catch ex As SocketException
            SetSnmp = ex.ToString
        End Try
    End Function

    Public Sub GetMinMaxHyst(SetMin As Boolean, SetMax As Boolean, setHyst As Boolean)

        Dim result As String
        myMin = String.Empty
        myMax = String.Empty
        myHyst = String.Empty

        Try
            If SetMin = True Then
                result = GetSnmp(myHost, BaseOID & MIBMin(myControl))
                myMin = Format$(Val(result) / 10, "0.0")
                If Val(result) <> Val(myMin) Then
                    result = "Error (min)"
                End If
            End If
            If SetMax = True Then
                result = GetSnmp(myHost, BaseOID & MIBMax(myControl))
                myMax = Format$(Val(result) / 10, "0.0")
                If Val(result) <> Val(myMin) Then
                    result = "Error (max)"
                End If
            End If
            If setHyst = True Then
                result = GetSnmp(myHost, BaseOID & MIBHyst(myControl))
                myHyst = Format$(Val(result) / 10, "0.0")
                If Val(result) <> Val(myMin) Then
                    result = "Error (hyst)"
                End If
            End If

        Catch ex As Exception
            result = "Error"
        End Try

    End Sub

    Public Sub SetMinMaxHyst(SetMin As Boolean, SetMax As Boolean, setHyst As Boolean)

        Dim result As String
        Dim myValue As String

        If SetMin = True Then
            myValue = CStr(Val(myMin) * 10)
            result = SetSnmp(myHost, BaseOID & MIBMin(myControl), myValue, True)
        End If
        If SetMax = True Then
            myValue = CStr(Val(myMax) * 10)
            result = SetSnmp(myHost, BaseOID & MIBMax(myControl), myValue, True)
        End If
        If setHyst = True Then
            myValue = CStr(Val(myHyst) * 10)
            result = SetSnmp(myHost, BaseOID & MIBHyst(myControl), myValue, True)
        End If

    End Sub

    Public Function GetDescription(myControl As Integer) As String

        GetDescription = myDesc(myControl)

    End Function

    Public Enum ControlTypes As Integer
        TYPE_NONE = 0
        TYPE_TCW12x_TEMPERATURE_1 = 1
        TYPE_TCW12x_TEMPERATURE_2 = 2
        TYPE_TCW12x_HUMIDITY_1 = 3
        TYPE_TCW12x_HUMIDITY_2 = 4
        TYPE_TCW12x_DIGITAL_IN_1 = 5
        TYPE_TCW12x_DIGITAL_IN_2 = 6
        TYPE_TCW12x_ANALOG_IN_1 = 7
        TYPE_TCW12x_ANALOG_IN_2 = 8
        TYPE_TCW12x_RELAY_1 = 9
        TYPE_TCW12x_RELAY_2 = 10
        TYPE_TCW18x_RELAY_1 = 11
        TYPE_TCW18x_RELAY_2 = 12
        TYPE_TCW18x_RELAY_3 = 13
        TYPE_TCW18x_RELAY_4 = 14
        TYPE_TCW18x_RELAY_5 = 15
        TYPE_TCW18x_RELAY_6 = 16
        TYPE_TCW18x_RELAY_7 = 17
        TYPE_TCW18x_RELAY_8 = 18
        TYPE_TCW18x_DIGITAL_IN = 19
        TYPE_TCW120_TEMPERATURE = 20
        TYPE_TCW120_DIGITAL_IN_1 = 21
        TYPE_TCW120_DIGITAL_IN_2 = 22
        TYPE_TCW120_ANALOG_IN_1 = 23
        TYPE_TCW120_ANALOG_IN_2 = 24
        TYPE_TCW120_RELAY_1 = 25
        TYPE_TCW120_RELAY_2 = 26
    End Enum

End Class
