Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Net
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Text

<Serializable()> Public Class Driver_Jeedom
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "0AAD7310-AB5F-11E5-85E4-E8561E5D46B0" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "Jeedom" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver Jeedom" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "Http" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "@" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "" 'Modèle du driver/interface
    Dim _Version As String = My.Application.Info.Version.ToString 'Version du driver
    Dim _OsPlatform As String = "3264" 'plateforme compatible 32 64 ou 3264 bits
    Dim _Picture As String = "" 'Image du driver (non utilisé actuellement)
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

    'A ajouter dans les ppt du driver

    'param avancé
    Dim _DEBUG As Boolean = False

    Dim _IPAdressJeedom As String = "localhost"
    Dim _IPPortJeedom As String = "80"
    Dim _APIKeyJeedom As String = "abc123"


#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes

    Dim _urlAPIJeedom As String = ""
    Dim _UrlCommandeJeedom As String = ""
    Dim _UrlMacroJeedom As String = ""

    Dim ObjetListe As ObjetList
    Dim eqLogicListe As List(Of eqLogicList)
    Dim eqLogicCommandListe As New eqLogicCommandList
    Dim eqLogicCommandListeTotal As New List(Of eqLogicCommandList)
    Dim ScenarioListe As List(Of ScenarioList)

    Public Class ResultObjet
        Public jsonrpc As String
        Public id As String
        Public result As List(Of ObjetList)
    End Class
    Public Class ResulteqLogic
        Public jsonrpc As String
        Public id As String
        Public result As List(Of eqLogicList)
    End Class

    Public Class ResultCommand
        Public jsonrpc As String
        Public id As String
        Public result As eqLogicCommandList
    End Class

    Public Class ResultScenario
        Public jsonrpc As String
        Public id As String
        Public result As List(Of ScenarioList)
    End Class

    Public Class ResultError
        Public jsonrpc As String
        Public id As String
        Public [error] As ErrorRpc
    End Class


    Public Class ObjetList
        Public id As String
        Public name As String
        Public father_id As String
        Public isVisible As String
    End Class

    Public Class eqLogicList
        Public id As String
        Public name As String
        Public eqType_name As String
        Public isVisible As String
        Public isEnable As String
    End Class

    Public Class eqLogicCommandList
        Public id As String
        Public name As String
        Public eqType_name As String
        Public isVisible As String
        Public isEnable As String
        Public cmds As List(Of eqLogicCommand)
    End Class

    Public Class eqLogicCommand
        Public id As String
        Public logicalId As String
        Public eqType_name As String
        Public name As String
        Public type As String
        Public subType As String
        Public eqLogic_id As String
        Public unite As String
        Public isVisible As String
        Public value As String
        Public currentvalue As String
        Public state As String
    End Class
    Public Class ScenarioList
        Public id As String
        Public name As String
        Public state As String
        Public isActive As String
        Public isEnable As String
    End Class
    Public Class ErrorRpc
        Public code As Long
        Public message As String
    End Class


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

    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
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
    ''' <param name="MyDevice">Objet représentant le Device </param>
    ''' <param name="Command">Nom de la commande avancée à éxécuter</param>
    ''' <param name="Param">tableau de paramétres</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False
        Try
            If MyDevice IsNot Nothing Then
                If Command = "" Then
                    Return False
                Else
                    'mise a jour de la configuration
                    Return Get_Config()
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            WriteLog("ERR: ExecuteCommand exception : " & ex.Message)
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
                    If Value IsNot Nothing Then
                        If String.IsNullOrEmpty(Value) Or IsNumeric(Value) Then
                            retour = "Veuillez choisir l'équipement"
                        End If
                    End If
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
            If My.Computer.Network.IsAvailable = False Then
                _IsConnect = False
                WriteLog("ERR: Pas d'accés réseau! Vérifiez votre connection")
                WriteLog("Driver non démarré")
                Exit Sub
            End If

            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _IPAdressJeedom = _Parametres.Item(1).Valeur
                _IPPortJeedom = _Parametres.Item(2).Valeur
                _APIKeyJeedom = Trim(_Parametres.Item(3).Valeur)
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            _urlApiJeedom = "http://" & _IPAdressJeedom & ":" & _IPPortJeedom & "/core/api/jeeApi.php?"""
            WriteLog("Start, connection au serveur " & _urlAPIJeedom)

            ' teste 2 cas car suivant les installation, pas la même url
            If Get_Config() Then
                _IsConnect = True
                WriteLog("Driver " & Me.Nom & " démarré avec succés à l'adresse " & _urlAPIJeedom)
            Else
                _urlApiJeedom = "http://" & _IPAdressJeedom & ":" & _IPPortJeedom & "/jeedom/core/api/jeeApi.php?"""
                If Get_Config() Then
                    _IsConnect = True
                    WriteLog("Driver " & Me.Nom & " démarré avec succés à l'adresse " & _urlAPIJeedom)
                Else
                    _IsConnect = False
                    WriteLog("ERR: Driver " & Me.Nom & " Erreur démarrage ")
                End If
            End If
        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Driver " & Me.Nom & " Erreur démarrage " & ex.Message)
            WriteLog("Driver non démarré")
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            MyTimer.Enabled = False
            WriteLog("Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            WriteLog("ERR: Driver " & Me.Nom & " Erreur arrêt " & ex.Message)
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
                WriteLog("ERR: Read, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If
            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: READ, Pas de réseau! Lecture du périphérique impossible")
                Exit Sub
            End If

            Dim ideqLogic As String = Trim(Mid(Objet.adresse1, 1, InStr(Objet.adresse1, "#") - 1))
            Dim boolscenario As Boolean = InStr(Objet.adresse1, "scénario") > 0
            Dim idCom As String = ""
            If Objet.adresse2 <> "" Then ' cas ou saisi dans la zone texte
                If InStr(Objet.adresse2, "#") > 0 Then
                    idCom = (Trim(Mid(Objet.adresse2, 1, InStr(Objet.adresse2, "#") - 1)))
                Else
                    idCom = Objet.adresse2
                End If
            Else
                idCom = Get_IdCmd(ideqLogic, idCom)
            End If
            WriteLog("DBG: Read, lecture de l'info " & idCom & " pour l'appareil numero " & ideqLogic)
            'recupere les commandes de l'appareil pour avoir les dernières valeurs

            Dim valeur As String = Nothing

            Select Case Objet.Type
                Case "LAMPE", "APPAREIL", "CONTACT", "DETECTEUR", "SWITCH"
                    valeur = Get_Value(ideqLogic, idCom)
                    If valeur <> "" Then Objet.Value = (valeur = 1)
                Case "BATTERIE", "TEMPERATURE", "HUMIDITE", "ENERGIEINSTANTANEE", "GENERIQUEVALUE", "UV", "BAROMETRE", "PLUIETOTAL", "VITESSEVENT", "COMPTEUR"
                    valeur = Get_Value(ideqLogic, idCom)
                    If valeur <> "" Then Objet.Value = Regex.Replace(CStr(valeur), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                Case "GENERIQUESTRING", "DIRECTIONVENT"
                    If Not boolscenario Then
                        valeur = Get_Value(ideqLogic, idCom)
                    Else
                        valeur = Get_StateScenario(ideqLogic)
                    End If
                    If valeur <> "" Then Objet.Value = valeur
                Case "GENERIQUEBOOLEEN"
                    valeur = Get_Value(ideqLogic, idCom)
                    If valeur <> "" Then Objet.Value = valeur = 1
                Case "SCENARIO"
            End Select
        Catch ex As Exception
            WriteLog("ERR: Read, adresse1 : " & Objet.adresse1 & " - adresse2 : " & Objet.adresse2)
            WriteLog("ERR: Read, Exception : " & ex.Message)
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
                WriteLog("ERR: Read, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If
            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: READ, Pas de réseau! Lecture du périphérique impossible")
                Exit Sub
            End If

            Dim ideqLogic As String = Trim(Mid(Objet.adresse1, 1, InStr(Objet.adresse1, "#") - 1))
            Dim idCmd As String = Get_IdCmd(ideqLogic, Command)

            If InStr(Objet.adresse1, "Scénario") > 0 Then
                If (InStr(UCase(Objet.adresse2), "RUN") > 0) Or (Command = "ON") Then EXEC_Sce(idCmd, "run")
                If (InStr(UCase(Objet.adresse2), "STOP") > 0) Or (Command = "OFF") Then EXEC_Sce(idCmd, "stop")
                Exit Sub
            End If

            WriteLog("DBG: Write, commande Jeedom numéro " & idCmd & " pour commande " & Command & " sur " & Objet.adresse1)

            Try
                Select Case Objet.Type
                    Case "LAMPE", "APPAREIL", "SWITCH"
                        Select Case Command
                            Case "ON", "OFF"
                                If Not EXEC_Cmd(idCmd) Then Exit Sub
                                Objet.value = Command = "ON"
                        End Select
                    Case Else
                        WriteLog("ERR: WRITE Erreur Write Type de composant non géré : " & Objet.Type.ToString)
                End Select

            Catch ex As Exception
                WriteLog("ERR: WRITE Erreur commande " & Command & " : " & ex.ToString)
            End Try
        Catch ex As Exception
            WriteLog("ERR: WRITE, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            WriteLog("ERR: DeleteDevice, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try
        Catch ex As Exception
            WriteLog("ERR: NewDevice, Exception : " & ex.Message)
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
            WriteLog("ERR: add_DeviceCommande, Exception :" & ex.Message)
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
            WriteLog("ERR: add_LibelleDriver, Exception : " & ex.Message)
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
            WriteLog("ERR: add_LibelleDevice, Exception : " & ex.Message)
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
            WriteLog("ERR: add_ParamAvance, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL)
            _DeviceSupport.Add(ListeDevices.BATTERIE)
            _DeviceSupport.Add(ListeDevices.DETECTEUR)
            _DeviceSupport.Add(ListeDevices.COMPTEUR)
            _DeviceSupport.Add(ListeDevices.CONTACT)
            _DeviceSupport.Add(ListeDevices.LAMPE)
            _DeviceSupport.Add(ListeDevices.SWITCH)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE)
            _DeviceSupport.Add(ListeDevices.BAROMETRE)
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.PLUIETOTAL)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE)
            _DeviceSupport.Add(ListeDevices.UV)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("IPAdress", "Adresse IP", "127.0.0.1")
            Add_ParamAvance("IPPort", "Port IP", "80")
            Add_ParamAvance("APIKey", "APIKey", "abc123")

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            'add_devicecommande("PRESETDIM", "permet de paramétrer le DIM : param1=niveau, param2=timer", 2)
            '            Add_DeviceCommande("RUN", "Run scenario", 0)
            '            Add_DeviceCommande("STOP", "Run scenario", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Numéro de l'équipement", "Numéro de l'équipement")
            Add_LibelleDevice("ADRESSE2", "Numéro de la commande", "Numéro de la commande")
            '           Add_LibelleDevice("SOLO", "si décoché, les autres composants eco-device seront mis à jour en même temps que celui-ci automatiquement.", "")
            '            Add_LibelleDevice("MODELE", "Modele", "modèle du device: IPX800,RSS,Arduino,Eco Device,Vigilance", "Arduino|EcoDevice|IPX800|RSS|VIGILANCE")
            Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
    End Sub

#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver et nom communes (ex: start)
    Sub Get_AllConfig() ' lancement de la lecture des config
        Get_Config()
    End Sub
    Function Get_Config() As Boolean
        ' recupere les configarions des equipements et scenarios de jeedom

        Try
            Dim response As String = ""
            'recherche des equipements
            response = Get_RPC(_urlAPIJeedom, "eqLogic::all", "", "")
            If response = "" Then Return False
            WriteLog("DBG: " & "GET_Config, response: " & response.ToString)
            Dim ResulteqLogic = Newtonsoft.Json.JsonConvert.DeserializeObject(response, GetType(ResulteqLogic))
            Me.eqLogicListe = ResulteqLogic.result
            WriteLog("GET_Config, equipements récupérés : " & Me.eqLogicListe.Count)

            'recherche des scenario
            response = Get_RPC(_urlAPIJeedom & "core/api/jeeApi.php?", "scenario::all", "", "")
            If response = "" Then Return False
            WriteLog("DBG: " & "GET_Config, response: " & response.ToString)
            Dim ResultScenarion = Newtonsoft.Json.JsonConvert.DeserializeObject(response, GetType(ResultScenario))
            Me.ScenarioListe = ResultScenarion.result
            WriteLog("GET_Config, scénarios récupérés : " & Me.ScenarioListe.Count)

            Get_ConfigLibelleDevice()

            Return True
        Catch ex As Exception

            WriteLog("ERR: " & "GET_Config, " & ex.Message)
            WriteLog("ERR: " & "GET_Config, Url: " & _urlAPIJeedom)
            Return False
        End Try
    End Function
    Function Get_ConfigLibelleDevice()
        Dim eqlogicname As String = ""
        Try
            ' mise en forme des libellés equipement actifs pour adresse1
            Dim IdLib As String = ""

            Me.eqLogicCommandListeTotal.Clear()
            For i = 0 To eqLogicListe.Count - 1
                If eqLogicListe.Item(i).isEnable = "0" Then Continue For
                IdLib += eqLogicListe.Item(i).id & " # " & eqLogicListe.Item(i).name & " > " & eqLogicListe.Item(i).eqType_name & "|"
                If Not Get_ConfigCmd(eqLogicListe.Item(i).id) Then Continue For
                eqlogicname = eqLogicListe.Item(i).id & " => " & eqLogicListe.Item(i).name
                WriteLog("DBG: " & "GET_ConfigLibelleDevice, equipement récupéré id : " & eqLogicListe.Item(i).id & " => " & eqLogicListe.Item(i).name)
            Next

            For i = 0 To ScenarioListe.Count - 1
                If ScenarioListe.Item(i).isActive = "0" Then Continue For
                IdLib += ScenarioListe.Item(i).id & " # " & ScenarioListe.Item(i).name & " > scénario" & "|"
                eqlogicname = ScenarioListe.Item(i).id & " => " & ScenarioListe.Item(i).name
                WriteLog("DBG: " & "GET_ConfigLibelleDevice, scénario récupéré id : " & ScenarioListe.Item(i).id & " => " & ScenarioListe.Item(i).name)
            Next

            IdLib = Mid(IdLib, 1, Len(IdLib) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
            Add_LibelleDevice("ADRESSE1", "Nom de l'équipement", "Nom de l'équipement", IdLib)

            WriteLog("GET_ConfigLibelleDevice, nbre de commandes récupérées : " & Me.eqLogicCommandListeTotal.Count)

            ' mise en forme des libellés commandes pour adresse2
            Dim idCom As String = ""
            For i = 0 To Me.eqLogicCommandListeTotal.Count - 1
                For j = 0 To Me.eqLogicCommandListeTotal.Item(i).cmds.Count - 1
                    If eqLogicCommandListeTotal.Item(i).cmds.Item(j).type = "info" Then
                        idCom += eqLogicCommandListeTotal.Item(i).cmds.Item(j).eqLogic_id & " #; " & eqLogicCommandListeTotal.Item(i).cmds.Item(j).id & " # " & eqLogicCommandListeTotal.Item(i).cmds.Item(j).name & " > " & eqLogicCommandListeTotal.Item(i).cmds.Item(j).type & "|"
                    End If
                Next
            Next

            ' mise en forme des libellés scénarios pour adresse2
            For i = 0 To ScenarioListe.Count - 1
                If ScenarioListe.Item(i).isActive = "0" Then Continue For
                idCom += ScenarioListe.Item(i).id & " #; " & "1 # RUN > scénario" & "|"
                idCom += ScenarioListe.Item(i).id & " #; " & "2 # STOP > scénario" & "|"
            Next
            idCom = Mid(idCom, 1, Len(idCom) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
            Add_LibelleDevice("ADRESSE2", "Nom de la commande", "Nom de la commande", idCom)
            Return True
        Catch ex As Exception

            WriteLog("ERR: " & "Get_ConfigLibelleDevice, " & ex.Message & "idlogic " & eqlogicname)
            WriteLog("ERR: " & "Get_ConfigLibelleDevice, Url: " & _urlAPIJeedom)
            Return False
        End Try

    End Function
    Function Get_ConfigCmd(ideqLogic As String) As Boolean
        ' retourne toutes les commandes pour un equipement

        Try
            If ideqLogic = "" Then
                WriteLog("ERR: " & "GET_ConfigCmd, recherche des équipement impossible, pas de ideqlogic")
                Return False
            End If
            Dim response As String = ""
            Dim resultcommand As Object
            'recherche des equipements
            response = Get_RPC(_urlAPIJeedom, "eqLogic::fullById", ideqLogic, "")
            If response = "" Then Return False
            WriteLog("DBG: " & "GET_ConfigCmd, response: " & response.ToString)
            resultcommand = Newtonsoft.Json.JsonConvert.DeserializeObject(response, GetType(ResultCommand))
            Me.eqLogicCommandListeTotal.Add(resultcommand.result)
            WriteLog("DBG: GET_ConfigCmd, " & Me.eqLogicCommandListeTotal.Item(Me.eqLogicCommandListeTotal.Count - 1).cmds.Count & " commandes pour " & Me.eqLogicCommandListeTotal.Item(Me.eqLogicCommandListeTotal.Count - 1).name & " récupérées ")
            Return True
        Catch ex As Exception
            WriteLog("ERR: " & "GET_ConfigCmd, " & ex.Message)
            WriteLog("ERR: " & "EXEC_Cmd, Url: " & _urlAPIJeedom & " eqLogic::fullById " & "ideqlogic=" & ideqLogic)
            Return False
        End Try
    End Function
    Function Get_IdCmd(ideqLogic As String, cmd As String) As String
        ' retourne l'id d'une commande à partir de son nom
        Try
            For i = 0 To Me.eqLogicCommandListeTotal.Count - 1
                If Me.eqLogicCommandListeTotal.Item(i).id = ideqLogic Then
                    For j = 0 To Me.eqLogicCommandListeTotal.Item(i).cmds.Count - 1
                        If Trim(UCase(Me.eqLogicCommandListeTotal.Item(i).cmds.Item(j).name)) = Trim(UCase(cmd)) Then
                            Return Me.eqLogicCommandListeTotal.Item(i).cmds.Item(j).id
                        End If
                    Next
                End If
            Next
            Return ""
        Catch ex As Exception
            WriteLog("ERR: " & "GET_IdCmd, " & ex.Message)
            WriteLog("ERR: " & "GET_IdCmd, Url: " & _urlAPIJeedom)
            Return ""
        End Try
    End Function
    Function Get_Value(ideqLogic As String, idcmd As String) As String
        'retourne l'état de la commande d'un equipement
        Try
            'recherche de la valeur
            Dim val As String = ""
            For i = 0 To Me.eqLogicCommandListeTotal.Count - 1
                If Me.eqLogicCommandListeTotal.Item(i).id = ideqLogic Then
                    For j = 0 To Me.eqLogicCommandListeTotal.Item(i).cmds.Count - 1
                        If Me.eqLogicCommandListeTotal.Item(i).cmds.Item(j).id = idcmd Then
                            Dim response As String = ""
                            ' recherche des objets
                            response = Get_RPC(_urlAPIJeedom & "core/api/jeeApi.php?", "eqLogic::fullById", Me.eqLogicCommandListeTotal.Item(i).cmds.Item(j).eqLogic_id, "")
                            WriteLog("DBG: " & "GET_Value, response: " & response.ToString)
                            If InStr(response, "error"":") > 0 Then
                                Continue For
                            End If
                            Dim resultcommand = Newtonsoft.Json.JsonConvert.DeserializeObject(response, GetType(ResultCommand))
                            Me.eqLogicCommandListe = resultcommand.result
                            For k = 0 To eqLogicCommandListe.cmds.Count - 1
                                If Me.eqLogicCommandListe.cmds.Item(j).id = idcmd Then
                                    Return Me.eqLogicCommandListe.cmds.Item(j).currentvalue
                                    Exit Function
                                End If
                            Next
                        End If
                    Next
                End If
            Next
            Return ""
        Catch ex As Exception
            WriteLog("ERR: " & "GET_Value, " & ex.Message)
            WriteLog("ERR: " & "GET_Value, " & idcmd)
            Return ""
        End Try
    End Function
    Function Get_StateScenario(idcmd As String) As String
        'retourne l'état de la commande d'un scénario

        Try
            'recherche de la valeur
            For i = 0 To ScenarioListe.Count - 1
                If Trim(UCase(ScenarioListe(i).id)) = Trim(UCase(idcmd)) Then
                    WriteLog("DBG: Get_StateScenario, valeur " & idcmd & " " & i & " => " & ScenarioListe(i).name & " = " & ScenarioListe(i).state)
                    Return ScenarioListe(i).state
                    Exit For
                End If
            Next
            Return ""
        Catch ex As Exception
            WriteLog("ERR: " & "GET_StateScenario, " & ex.Message)
            WriteLog("ERR: " & "GET_StateScenario, " & idcmd)
            Return ""
        End Try
    End Function
    Function EXEC_Cmd(id As String) As Boolean
        'execute une commande

        Try
            Return Get_RPC(_urlAPIJeedom, "cmd::execCmd", id, "") <> ""
        Catch ex As Exception

            WriteLog("ERR: " & "EXEC_Cmd, " & ex.Message)
            WriteLog("ERR: " & "EXEC_Cmd, Url: " & _urlAPIJeedom & "core/api/jeeApi.php? " & " cmd::execCmd " & "id=" & id)
            Return False
        End Try
    End Function
    Function EXEC_Sce(id As String, newstate As String) As Boolean
        'execute un scenario

        Try
            Return Get_RPC(_urlAPIJeedom, "scenario::changeState", id, newstate) <> ""
        Catch ex As Exception
            WriteLog("ERR: " & "EXEC_Sce, " & ex.Message)
            WriteLog("ERR: " & "EXEC_Sce, Url: " & _urlAPIJeedom & "core/api/jeeApi.php? " & " scenario::changeState " & "id=" & id)
            Return False
        End Try
    End Function
    Function Get_RPC(url As String, method As String, id As String, newstate As String) As String
        ' lance la commande données en parametre. Retourne la réponse de jeedom

        Try
            If method = "" Then Return ""

            Dim parts As String = ""
            If id = "" Then
                parts = "{ ""jsonrpc"":""2.0"", ""id"":""1"", ""method"":""" & method & """, ""params"": {""apikey"":""" & _APIKeyJeedom & """}}"
            Else
                If newstate = "" Then
                    parts = "{ ""jsonrpc"":""2.0"", ""id"":""1"", ""method"":""" & method & """, ""params"": {""apikey"":""" & _APIKeyJeedom & """, ""id"":""" & id & """}}"
                Else
                    parts = "{ ""jsonrpc"":""2.0"", ""id"":""1"", ""method"":""" & method & """, ""params"": {""apikey"":""" & _APIKeyJeedom & """, ""id"":""" & id & """,""string state : [" & newstate & "]""}}"
                End If
            End If

            Dim postBytes As Byte() = Encoding.UTF8.GetBytes(parts)
            WriteLog("DBG: " & "GET_RPC, response: " & parts)
            Dim Request As HttpWebRequest = HttpWebRequest.Create(url)
            Request.ContentType = "application/x-www-form-urlencoded"
            Request.Method = "POST"
            Request.ContentLength = postBytes.Length
            Request.Timeout = 10000

            Dim requestStream As Stream = Request.GetRequestStream()
            requestStream.Write(postBytes, 0, postBytes.Length)
            requestStream.Close()

            Try
                Dim Response As HttpWebResponse = Request.GetResponse()
                Dim responsereader = New StreamReader(Response.GetResponseStream())
                Dim reponse = responsereader.ReadToEnd()
                responsereader.Close()
                If InStr(reponse, "error"":") Then
                    Dim ResultCommand = Newtonsoft.Json.JsonConvert.DeserializeObject(reponse, GetType(ResultError))
                    WriteLog("ERR: " & "GET_RPC, Url: " & url & " Commande : " & method)
                    WriteLog("ERR: GET_ConfigCmd, Retour erreur " & reponse)
                    WriteLog("ERR: GET_ConfigCmd, Code erreur : " & ResultCommand.error.code & ", message : " & ResultCommand.error.message)
                    Return ""
                Else
                    Return reponse
                End If
            Catch ex As Exception
                WriteLog("ERR: " & "GET_RPC, " & ex.Message)
                WriteLog("ERR: " & "GET_RPC, Url: " & url & " Commande : " & method)
                Return ""
            End Try
        Catch ex As Exception
            WriteLog("ERR: " & "GET_RPC, " & ex.Message)
            WriteLog("ERR: " & "GET_RPC, Url: " & url & " Commande : " & method)
            Return ""
        End Try
    End Function

    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                If _DEBUG Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
                End If
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " WriteLog", ex.Message)
        End Try
    End Sub
#End Region
End Class
