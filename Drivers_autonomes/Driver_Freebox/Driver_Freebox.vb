Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Net
Imports System.IO
Imports System.Text

' Auteur : jphomi 
' Date : 01/01/2017


''' <summary>Driver Freebox </summary>

<Serializable()> Public Class Driver_Freebox
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "67C1B42E-CE06-11E6-9398-A78E00593AC9"
    Dim _Nom As String = "Freebox"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Configuration Freebox"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "WEB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "@"
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
    Dim _urlFreebox As String = "http://mafreebox.freebox.fr"

    'param avancé
    Dim _DEBUG As Boolean = False

#End Region

#Region "Variables internes"
    Dim _Obj As Object = Nothing
    Dim _UpdateInProcess As Boolean = False
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
            Dim retour As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1"
                    If Value IsNot Nothing Then
                        If String.IsNullOrEmpty(Value) Or IsNumeric(Value) Then
                            retour = "Veuillez saisir le nom du module en respectant la casse ( maj/minuscule )"
                        End If
                    End If
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
            If My.Computer.Network.IsAvailable = False Then
                _IsConnect = False
                WriteLog("ERR: Pas d'accés réseau! Vérifiez votre connection")
                WriteLog("Driver non démarré")
                Exit Sub
            End If

            Try
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            Get_Config(_urlFreebox & "/pub/fbx_info.txt")

            If ConfigBox.Modele <> "" Then
                WriteLog(ConfigBox.Modele & ", version " & ConfigBox.version & ", connectée en " & ConfigBox.ModeConnect & " depuis " & ConfigBox.TmpStart)
                _IsConnect = True

                MyTimer.Interval = _Refresh * 1000
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick
            End If
        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Driver " & Me.Nom & " Erreur démarrage " & ex.Message)
            WriteLog("Driver " & Me.Nom & " non démarré")
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
                Case "GENERIQUEVALUE"
                    Select Case Objet.Adresse1
                        Case "Débit montant"
                            Objet.Value = Regex.Replace(CStr(ConfigBox.DebitAtmUp), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        Case "Débit descendant"
                            Objet.Value = Regex.Replace(CStr(ConfigBox.DebitAtmDown), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End Select
                Case "GENERIQUESTRING"
                    Select Case Objet.Adresse1
                        Case "Protocole"
                            Objet.Value = ConfigBox.Protocole
                    End Select
            End Select
            WriteLog("DBG: Valeur enregistrée : " & Objet.Type & " -> " & Objet.value)
        Catch ex As Exception
            WriteLog("ERR: Read, adresse1 : " & Objet.adresse1)
            WriteLog("ERR: Read, Exception : " & ex.Message)
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
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Nom du paramètre", "", "Débit montant|Débit descendant|Protocole")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("REFRESH", "Rafraichissement valeur en sec", "")
            ' Libellés Device inutiles
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "@", "")
        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub


    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        Get_Config(_urlFreebox)
    End Sub

#End Region

#Region "Fonctions internes"

    Dim ConfigBox As New FreeBoxConfig

    Public Class FreeBoxConfig
        Public Modele As String
        Public version As String
        Public ModeConnect As String
        Public TmpStart As String
        Public Protocole As String
        Public DebitAtmUp As String
        Public DebitAtmDown As String
    End Class

    Private Sub Get_Config(adrs As String)

        Try
            Dim Request As HttpWebRequest = HttpWebRequest.Create(adrs)
            Request.Credentials = CredentialCache.DefaultCredentials
            Dim Response As HttpWebResponse = Request.GetResponse()

            Dim responsereader = New StreamReader(Response.GetResponseStream(), Encoding.GetEncoding(1252))

            Dim responsebody As New List(Of String)
            Dim strbody As String = responsereader.ReadLine
            Dim responsebodystr As String = ""
            Do While (Not strbody Is Nothing)
                If (InStr(vbCrLf, strbody) > 0) Then
                    responsebody.Add(responsebodystr)
                    responsebodystr = ""
                End If
                strbody = responsereader.ReadLine
                responsebodystr = responsebodystr + strbody
            Loop

            responsereader.Close()

            WriteLog("DBG: Get_Config : responsebody.Count " & responsebody.Count)

            ConfigBox.Modele = ""
            Dim i As Integer
            Dim str As String = ""

            For i = 0 To responsebody.Count - 1
                str = responsebody(i)
                ' info generales
                If (InStr(str, "Modèle") > 0) And (InStr(str, "route") > 0) Then
                    ConfigBox.TmpStart = Trim(Mid(str, InStr(str, "route") + 5, Len(str)))
                    str = Trim(Mid(str, 1, InStr(str, "Temps") - 1))
                    ConfigBox.ModeConnect = Trim(Mid(str, InStr(str, "connection") + 11, Len(str)))
                    str = Trim(Mid(str, 1, InStr(str, "Mode") - 1))
                    ConfigBox.version = Trim(Mid(str, InStr(str, "firmware") + 9, Len(str)))
                    str = Trim(Mid(str, 1, InStr(str, "Version") - 1))
                    ConfigBox.Modele = Trim(Mid(str, InStr(str, "Modèle") + 6, Len(str)))
                End If
                'Adsl
                If (InStr(str, "Etat") > 0) And (InStr(str, "Protocole") > 0) Then
                    ConfigBox.Protocole = Trim(Mid(str, InStr(str, "Protocole") + 10, InStr(str, "Mode") - 1))
                End If
                'debits
                If (InStr(str, "Débit") > 0) And (InStr(str, "HEC") > 0) Then
                    str = Trim(Mid(str, 1, InStrRev(str, "kb/s") - 1))
                    ConfigBox.DebitAtmUp = Trim(Mid(str, InStr(str, "kb/s") + 4, Len(str)))
                    str = Trim(Mid(str, 1, InStr(str, "kb/s") - 1))
                    ConfigBox.DebitAtmDown = Trim(Mid(str, InStr(str, "ATM") + 4, Len(str)))
                End If
                WriteLog("DBG: Get_Config, responsebody(" & i & ") : " & responsebody(i))
            Next
            WriteLog("Lecture configuration Freebox effectuée")
        Catch ex As Exception
            WriteLog("ERR: " & ex.Message)
            WriteLog("ERR: " & "GET_Config Url: " & adrs)
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
