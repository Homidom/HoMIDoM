Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Text.RegularExpressions
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Net
Imports System.IO
Imports System.Xml

' Auteur : jphomi 
' Date : 01/06/2015

''' <summary>Driver IPX800 recption/envoi commandes</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_IPX800
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "9A2B3B2E-FBFD-11E4-948F-69CE1D5D46B0"
    Dim _Nom As String = "IPX800"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Pilotage IPX800"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "WEB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "IPX800"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Dim _LabelsDevice As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _urlIPX As String = "http://192.168.0.0:80/"
    Dim _IPXVersion As String = "3.0.0"

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _IPAdress As String = "192.168.0.0"
    Dim _IPPort As String = "80"
    Dim _Username As String = ""
    Dim _Password As String = ""


#End Region

#Region "Variables internes"
    Dim _Obj As Object = Nothing
#End Region

#Region "Propriétés génériques"
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
                    'Write(deviceobject, Command, Param(0), Param(1))
                    WriteLog("DBG: ExecuteCommandAdvance : " & MyDevice.ToString & " : " & Command & "/" & Param(0) & "-" & Param(1))
                    Select Case UCase(Command)
                        Case ""
                        Case Else
                    End Select
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            WriteLog("ERR: ExecuteCommand exception : " & ex.Message)
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
            Dim result As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1"
                    If String.IsNullOrEmpty(Value) Then
                        result = "Veuillez saisir l'adresse de lecture sous la forme A:x (1..9) pour Analog entreex, x pour les autres entrées/sorties (0..31)"
                    End If
            End Select
            Return result
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            If My.Computer.Network.IsAvailable = False Then
                _IsConnect = False
                WriteLog("ERR: Pas d'accés réseau! Vérifiez votre connection")
                WriteLog("Driver non démarré")
                Exit Sub
            End If

            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _IPAdress = _Parametres.Item(1).Valeur
                _IPPort = _Parametres.Item(2).Valeur
                _Username = _Parametres.Item(3).Valeur
                _Password = _Parametres.Item(4).Valeur

            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            _urlIPX = "http://" & _IPAdress & ":" & _IPPort & "/"

            Get_Config(_urlIPX & "globalstatus.xml", _Username, _Password)

            Dim elmt As String

            Try
                elmt = "version"
                _IPXVersion = valueconfig.version
                If _IPXVersion <> "" Then
                    WriteLog("Nom : " & valueconfig.config_hostname)
                    WriteLog("Version logicielle : " & _IPXVersion)
                    _IsConnect = True
                    WriteLog("Driver démarré")
                Else
                    _IsConnect = False
                    WriteLog("ERR: Driver " & Me.Nom & " Erreur démarrage, matériel introuvable ")
                    WriteLog("Driver non démarré")
                End If
            Catch ex As Exception
                _IsConnect = False
                WriteLog("ERR: Matériel non trouvé à l'adresse : " & _urlIPX)
                WriteLog("Driver non démarré")
            End Try
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
    ''' <remarks>pas utilisé</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read

        Try

            If _Enable = False Then Exit Sub

            If _IsConnect = False Then
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'écrire sur le port")
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

            Dim valeur As Object = Nothing

            Select Case Objet.Type
                Case "LAMPE", "APPAREIL"
                    Objet.value = GET_VALUE(_urlIPX & "globalstatus.xml", _Username, _Password, "led" & (Objet.Adresse1 - 1))
                Case "CONTACT"
                    Objet.value = GET_VALUE(_urlIPX & "globalstatus.xml", _Username, _Password, "btn" & (Objet.Adresse1 - 1))
                Case "COMPTEUR"
                    If String.IsNullOrEmpty(Objet.adresse2) Then
                        Objet.Value = GET_VALUE(_urlIPX & "globalstatus.xml", _Username, _Password, "count" & (Objet.Adresse1 - 1))
                    Else
                        'permet de reinitialier un compteur
                        Dim numpage As String = Int(Objet.adresse1 / 2) + 1
                        If (InStr(Objet.adresse2, "inc=") > 0) Or (InStr(Objet.adresse2, "dec=") > 0) Then
                            SEND_IPX800(_urlIPX & "protect/assignio/counter" & numpage & ".htm?num=" & Objet.adresse1 - 1 & "&" & Objet.adresse2, _Username, _Password)
                        Else
                            SEND_IPX800(_urlIPX & "protect/assignio/counter" & numpage & ".htm?num=" & Objet.adresse1 - 1 & "&counter=" & Objet.adresse2, _Username, _Password)
                        End If
                        Objet.Value = Objet.adresse2
                    End If
                Case "TEMPERATURE", "HUMIDITE", "ENERGIEINSTANTANEE"
                    valeur = GET_VALUE(_urlIPX & "globalstatus.xml", _Username, _Password, "analog" & Mid(Objet.Adresse1, InStr(Objet.Adresse1, ":") + 1, 1) - 1)
                    Objet.Value = Regex.Replace(CStr(valeur), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                Case "GENERIQUEVALUE"
                    If InStr(Objet.Adresse1, "A:") = 0 Then
                        Objet.Value = GET_VALUE(_urlIPX & "globalstatus.xml", _Username, _Password, Objet.Adresse1)
                    Else
                        valeur = GET_VALUE(_urlIPX & "globalstatus.xml", _Username, _Password, "analog" & Mid(Objet.Adresse1, InStr(Objet.Adresse1, ":") + 1, 1) - 1)
                        Objet.Value = Regex.Replace(CStr(valeur), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End If
                Case "GENERIQUESTRING"
                    If String.IsNullOrEmpty(Objet.adresse2) Then
                        Objet.Value = GET_VALUE(_urlIPX & "globalstatus.xml", _Username, _Password, Objet.Adresse1)
                    Else
                        SEND_IPX800(_urlIPX & Objet.adresse2, _Username, _Password)
                        Objet.Value = 1
                    End If
            End Select

            WriteLog("DBG: Valeur enregistrée : " & Objet.Type & " -> " & Objet.value)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", ex.Message)
            WriteLog("ERR: Read, Exception : " & ex.Message)
            WriteLog("ERR: Read, adresse1 : " & Objet.adresse1 & " - adresse2 : " & Objet.adresse2)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représentant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then Exit Sub

            If _IsConnect = False Then
                WriteLog("ERR: WRITE, Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

            WriteLog("DBG: WRITE Device " & Objet.Name & " --> " & Command)

            Try
                Select Case Objet.Type
                    Case "APPAREIL"
                        Select Case Command
                            Case "ON"
                                If _Version < "3.05.33" Then
                                    SEND_IPX800(_urlIPX & "preset.htm?led" & Objet.adresse1 & "=1", _Username, _Password)
                                Else
                                    SEND_IPX800(_urlIPX & "preset.htm?set" & Objet.adresse1 & "=1", _Username, _Password)
                                End If
                                Objet.value = True
                                WriteLog("DBG: Write " & Objet.Type & " Adr : " & Objet.adresse1 & " -> ON")
                                Exit Sub
                            Case "OFF"
                                If _Version < "3.05.33" Then
                                    SEND_IPX800(_urlIPX & "preset.htm?led" & Objet.adresse1 & "=0", _Username, _Password)
                                Else
                                    SEND_IPX800(_urlIPX & "preset.htm?set" & Objet.adresse1 & "=0", _Username, _Password)
                                End If
                                Objet.value = False
                                WriteLog("DBG: Write " & Objet.Type & " Adr : " & Objet.adresse1 & " -> OFF")
                                Exit Sub
                        End Select
                    Case "SWITCH"
                        Select Case Command
                            Case "ON"
                                SEND_IPX800(_urlIPX & "preset.htm?RLY" & Objet.adresse1 & "=1", _Username, _Password)
                                WriteLog("DBG: Write " & Objet.Type & " Adr : " & Objet.adresse1 & " -> Impulsion 0.1.0")
                            Case "OFF"
                                SEND_IPX800(_urlIPX & "preset.htm?RLY" & Objet.adresse1 & "=0", _Username, _Password)
                                WriteLog("DBG: Write " & Objet.Type & " Adr : " & Objet.adresse1 & " -> Impulsion 1.0.1")
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
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("IPAdress", "Adresse IP", "192.168.0.0")
            Add_ParamAvance("IPPort", "Port IP", "80")
            Add_ParamAvance("Username", "Nom utilisateur", "homidom@homidom.com")
            Add_ParamAvance("Password", "Mot de passe", "homi123456")

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Numéro entrée/sortie/compteur", "Num. entrée/sortie/compteur indiqué sur l'IPX800", "")
            Add_LibelleDevice("ADRESSE2", "Nvlle valeur ou cmd. prise littéralement", "Cmd sans url de l'IPX800", "")
            Add_LibelleDevice("REFRESH", "Refresh en sec", "Minimum 600, valeur rafraicissement station", "600")
            ' Libellés Device inutiles
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "@", "")
        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

#End Region

#Region "Fonctions internes"

    Dim obj As Object
    Dim valueinputs As listInputs
    Dim valueoutputs As listOutputs
    Dim valueanalog As listAnalog
    Dim valuecount As listCount
    Dim valueconfig As New IpxConfig

    Public Class valeur
        Public _id As String
        Public val As Object
    End Class

    Public Class listInputs
        Public _IPX800_V3 As String
        Public IN1 As Integer
        Public IN2 As Integer
        Public IN3 As Integer
        Public IN4 As Integer
        Public IN5 As Integer
        Public IN6 As Integer
        Public IN7 As Integer
        Public IN8 As Integer
        Public IN9 As Integer
        Public IN10 As Integer
        Public IN11 As Integer
        Public IN12 As Integer
        Public IN13 As Integer
        Public IN14 As Integer
        Public IN15 As Integer
        Public IN16 As Integer
        Public IN17 As Integer
        Public IN18 As Integer
        Public IN19 As Integer
        Public IN20 As Integer
        Public IN21 As Integer
        Public IN22 As Integer
        Public IN23 As Integer
        Public IN24 As Integer
        Public IN25 As Integer
        Public IN26 As Integer
        Public IN27 As Integer
        Public IN28 As Integer
        Public IN29 As Integer
        Public IN30 As Integer
        Public IN31 As Integer
        Public IN32 As Integer
        Public IN33 As Integer
    End Class

    Public Class listOutputs
        Public _IPX800_V3 As String
        Public OUT1 As Integer
        Public OUT2 As Integer
        Public OUT3 As Integer
        Public OUT4 As Integer
        Public OUT5 As Integer
        Public OUT6 As Integer
        Public OUT7 As Integer
        Public OUT8 As Integer
        Public OUT9 As Integer
        Public OUT10 As Integer
        Public OUT11 As Integer
        Public OUT12 As Integer
        Public OUT13 As Integer
        Public OUT14 As Integer
        Public OUT15 As Integer
        Public OUT16 As Integer
        Public OUT17 As Integer
        Public OUT18 As Integer
        Public OUT19 As Integer
        Public OUT20 As Integer
        Public OUT21 As Integer
        Public OUT22 As Integer
        Public OUT23 As Integer
        Public OUT24 As Integer
        Public OUT25 As Integer
        Public OUT26 As Integer
        Public OUT27 As Integer
        Public OUT28 As Integer
        Public OUT29 As Integer
        Public OUT30 As Integer
        Public OUT31 As Integer
        Public OUT32 As Integer
    End Class

    Public Class listAnalog
        Public _IPX800_V3 As String
        Public AN1 As Integer
        Public AN2 As Integer
        Public AN3 As Integer
        Public AN4 As Integer
        Public AN5 As Integer
        Public AN6 As Integer
        Public AN7 As Integer
        Public AN8 As Integer
        Public AN9 As Integer
        Public AN10 As Integer
        Public AN11 As Integer
        Public AN12 As Integer
        Public AN13 As Integer
        Public AN14 As Integer
        Public AN15 As Integer
        Public AN16 As Integer
    End Class

    Public Class listCount
        Public _IPX800_V3 As String
        Public C1 As Integer
        Public C2 As Integer
        Public C3 As Integer
        Public C4 As Integer
        Public C5 As Integer
        Public C6 As Integer
        Public C7 As Integer
        Public C8 As Integer
    End Class

    Public Class IpxConfig
        Public config_hostname As String
        Public version As String
        Public http_port As String
        Public dnsstatus As String
        Public day As String
        Public time0 As String
        Public pingmsg As String
        Public psel As String
        Public pingip As String
        Public pingretry As String
        Public pingtime As String
    End Class


    Private Sub Get_Config(adrs As String, user As String, password As String)

        Dim webclient As New WebClient
        Dim cache As New CredentialCache
        Dim str As String

        Try
            cache.Add(New Uri(adrs), "Basic", New NetworkCredential(user, password))
            webclient.Credentials = cache

            Dim reader As XmlTextReader = New XmlTextReader(webclient.OpenRead(adrs))
            reader.WhitespaceHandling = WhitespaceHandling.Significant
            WriteLog("DBG: Fichier xml -> " & adrs & " acquis")
            While reader.Read()
                str = reader.ReadString
                WriteLog("DBG: " & reader.Name & " -> " & str)
                Select Case reader.Name
                    Case "version"
                        valueconfig.version = str
                    Case "config_hostname"
                        valueconfig.config_hostname = str
                    Case "http_port"
                        valueconfig.http_port = str
                    Case "dnsstatus"
                        valueconfig.dnsstatus = str
                    Case "day"
                        valueconfig.day = str
                    Case "time0"
                        valueconfig.time0 = str
                    Case "pingmsg"
                        valueconfig.pingmsg = str
                    Case "psel"
                        valueconfig.psel = str
                    Case "pingip"""
                        valueconfig.pingip = str
                    Case "pingretry"
                        valueconfig.pingretry = str
                    Case "pingtime"
                        valueconfig.pingtime = str
                End Select
            End While
        Catch ex As Exception
            WriteLog("ERR: " & ex.Message)
            WriteLog("ERR: " & "GET_Config Url: " & adrs)
        End Try
    End Sub

    Public Function GET_VALUE(adrs As String, user As String, password As String, ByVal Element As String) As Object

        WriteLog("DBG: " & "GET Url: " & adrs)
        Dim webclient As New WebClient
        Dim cache As New CredentialCache

        Try
            Dim result As Object = Nothing
            cache.Add(New Uri(adrs), "Basic", New NetworkCredential(user, password))
            webclient.Credentials = cache

            Dim reader As XmlTextReader = New XmlTextReader(webclient.OpenRead(adrs))
            WriteLog("DBG: Acquisition fichier xml -> " & adrs)
            reader.WhitespaceHandling = WhitespaceHandling.Significant
            While reader.Read()
                If reader.Name = Element Then
                    Dim valeurreader As String = reader.ReadString
                    WriteLog("DBG: " & "Valeur trouvée  pour " & Element & " -> " & valeurreader)
                    If Not IsNumeric(valeurreader) Then
                        Select Case valeurreader
                            Case "up"
                                result = False
                            Case "dn"
                                result = True
                        End Select
                    Else
                        result = Val(valeurreader)
                    End If
                    Exit While
                End If
            End While
            GET_VALUE = result

        Catch ex As Exception
            WriteLog("ERR: " & ex.Message)
            WriteLog("ERR: " & "GET Url: " & adrs)
            Return ""
        End Try
    End Function

    Private Sub GET_VALUES(adrs As String, user As String, password As String)

        Dim webclient As New WebClient
        Dim cache As New CredentialCache

        Try
            Dim client As New Net.WebClient
            cache.Add(New Uri(adrs), "Basic", New NetworkCredential(user, password))
            webclient.Credentials = cache

            adrs = _urlIPX & "api/xdevices.json?cmd=10"
            Dim responsebody = client.DownloadString(Adrs)
            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody)
            valueinputs = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(listInputs))
            WriteLog("DBG: " & "GET_VALUES Url: " & Adrs)
            WriteLog("DBG: GetValue inputs : " & responsebody.ToString)

            Adrs = _urlIPX & "api/xdevices.json?cmd=20"
            responsebody = client.DownloadString(Adrs)
            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody)
            valueoutputs = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(listOutputs))
            WriteLog("DBG: " & "GET_VALUES Url: " & Adrs)
            WriteLog("DBG: GetValue outputs : " & responsebody.ToString)

            Adrs = _urlIPX & "api/xdevices.json?cmd=30"
            responsebody = client.DownloadString(Adrs)
            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody)
            valueanalog = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(listAnalog))
            WriteLog("DBG: " & "GET_VALUES Url: " & Adrs)
            WriteLog("DBG: GetValue analog : " & responsebody.ToString)

            Adrs = _urlIPX & "api/xdevices.json?cmd=40"
            responsebody = client.DownloadString(Adrs)
            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody)
            valuecount = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(listCount))
            WriteLog("DBG: " & "GET_VALUES Url: " & Adrs)
            WriteLog("DBG: GetValue count : " & responsebody.ToString)

        Catch ex As Exception
            WriteLog("ERR: " & ex.Message)
            WriteLog("ERR: " & "GET_VALUES Url: " & Adrs)
        End Try
    End Sub

    Public Sub SEND_IPX800(ByVal Command As String, user As String, password As String)
        Try
            WriteLog("DBG: " & "SEND Url: " & Command)

            Dim reader As StreamReader
            Dim str As String = ""

            Dim cache As New CredentialCache
            cache.Add(New Uri(Command), "Basic", New NetworkCredential(user, password))

            Dim request As WebRequest = WebRequest.Create(Command)
            request.Credentials = cache

            Dim response As WebResponse = request.GetResponse()
            reader = New StreamReader(response.GetResponseStream())
            str = reader.ReadToEnd
            reader.Close()
        Catch ex As Exception
            WriteLog("ERR: " & ex.Message)
            WriteLog("ERR: " & "SEND Url: " & Command)
        End Try
    End Sub

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
