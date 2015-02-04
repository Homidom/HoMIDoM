'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.IO
Imports System.Threading
'Imports System.Threading.Tasks

Imports Google.Apis.Calendar.v3
Imports Google.Apis.Calendar.v3.Data
Imports Google.Apis.Calendar.v3.EventsResource
Imports Google.Apis.Util.Store
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Auth.OAuth2.Flows
Imports Google.Apis.Services
Imports Google.Apis.Json


Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


' Auteur : Laurent/Mathieu
' Date : 19/04/2013  Creation du driver
' Date : 30/11/2013  Maj pour API V3


''' <summary>Class Driver_GoogleCalendar, permet de communiquer avec le un Calendrier Google.</summary>
''' <remarks>Nécessite un compte Google </remarks>
<Serializable()> Public Class Driver_GoogleCalendar
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "C19296A0-B00D-11E2-94A2-7B126288709B"
    Dim _Nom As String = "GoogleCalendar"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Calendrier Google Calendar"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "HTTP"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Google"
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

   
#End Region

#Region "Variables Internes"

    'Ajoutés dans les ppt avancés dans New()
    Dim _DEBUG As Boolean = False
    Dim _CalHomidomName As String
    Dim _CalFeriesName As String

    Dim Service As CalendarService ' = New CalendarService("HoMIDoM")

    '' Calendar scopes which is initialized on the main method.
    Dim scopes As IList(Of String) = New List(Of String)()

    Dim CalendarHomidom As New Data.CalendarListEntry
    Dim Calendarferies As New Data.CalendarListEntry

    Dim str_to_week As New Dictionary(Of String, String)
    Dim firstScan As Boolean = True
    'Dim gCal As Gcal_API_V3.OAuth2
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "exception : " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Permet de vérifier si un champ est valide
    ''' </summary>
    ''' <param name="Champ"></param>
    ''' 
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)

            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        'Dim queryCal As New CalendarQuery()
        'Dim CalendarFeed As CalendarFeed
        Try
            ' Récuperation des parametres avancés
            _DEBUG = _Parametres.Item(0).Valeur
            _CalHomidomName = _Parametres.Item(1).Valeur
            _CalFeriesName = _Parametres.Item(2).Valeur

            ' Positionne le compte Gmail à utiliser
            'Service.setUserCredentials(_UserName, _PassWord)
            'queryCal.Uri = New Uri("https://www.google.com/calendar/feeds/default/allcalendars/full")

            ' Recupere la liste des calendriers du compte 
            'CalendarFeed = Service.Query(queryCal)
            ' Add the calendar specific scope to the scopes list.
            'scopes.Add(CalendarService.Scope.Calendar)

            ' Add the calendar specific scope to the scopes list.
            scopes.Add(CalendarService.Scope.Calendar)
            Dim stream As New FileStream(My.Application.Info.DirectoryPath & "\config\client_secrets.json", FileMode.Open, FileAccess.Read)
            Dim X = GoogleClientSecrets.Load(stream).Secrets
            'Dim Y As FileDataStore = New FileDataStore("Calendar.VB.Sample")

            'Using stream As New FileStream("client_secrets.json", FileMode.Open, FileAccess.Read)

            If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar ", "ClientId : " & X.ClientId & " // ClientSecret : " & X.ClientSecret)
            If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar ", "Emplacement client_secrets.json : " & My.Application.Info.DirectoryPath & "\config\")

            'Dim initializer1 = New GoogleAuthorizationCodeFlow.Initializer
            'initializer1.Scopes = scopes
            'initializer1.ClientSecrets = X
            'initializer1.DataStore = New FileDownloadData(Y.FolderPath, Y.ToString)
            'Dim flow = New GoogleAuthorizationCodeFlow(initializer1)

            'If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar ", "Flow Ok : " & initializer1.ToString)

            Dim fileName = My.Application.Info.DirectoryPath & "\config\Google.Apis.Auth.OAuth2.Responses.TokenResponse-user"

            If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar ", "File Path TOKENRESPONSE = " & fileName)

            Dim credential As UserCredential
            If System.IO.File.Exists(fileName) Then
                Dim Temp1 As String = System.IO.File.ReadAllText(fileName).Replace(",", "," & ControlChars.CrLf)
                Dim position1 As Integer = InStr(Temp1, "refresh_token") + 15
                Dim Temp2 As String = Right(Temp1, Temp1.Length - position1)
                Dim position2 As Integer = InStr(Temp2, ",") - 2

                If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar ", "File = " & Temp1)

                Dim RefreshToken As String = Left(Temp2, position2)
                If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar ", "Refresh token = " & RefreshToken)

                Dim myStoredResponse As New GoogleOauthAPI.StoredResponse(RefreshToken)
                'credential = New AuthorizationCodeInstalledApp(flow,  New LocalServerCodeReceiver()).AuthorizeAsync(
                '    "user", CancellationToken.None).Result

                GoogleWebAuthorizationBroker.Folder = "Tasks.Auth.Store"
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(X, scopes, "user", CancellationToken.None, New GoogleOauthAPI.SavedDataStore(myStoredResponse)).Result
            Else

                System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\Drivers\GoogleOauthAPI.exe")

                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar ", "File Path TOKENRESPONSE n'a pas été trouvé ")
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar", "Driver " & Me.Nom & " non démarré")
                Exit Sub
            End If

            'Dim credential As UserCredential
            'Using stream As New FileStream("client_secrets.json", FileMode.Open, FileAccess.Read)
            'credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            '       New ClientSecrets With {.ClientId = _UserName, .ClientSecret = _PassWord}, scopes, "user", CancellationToken.None
            '       ).Result 'New FileDataStore("Calendar.VB.Sample")
            'End Using
            'New ClientSecrets With {.ClientId = _UserName, .ClientSecret = _PassWord}
            'GoogleClientSecrets.Load(stream).Secrets
            If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar ", "Credential Ok : ")

            ' Create the calendar service using an initializer instance
            Dim initializer As New BaseClientService.Initializer()
            initializer.HttpClientInitializer = credential
            initializer.ApplicationName = "HoMIDoM"
            Service = New CalendarService(initializer)

            ' Fetch the list of calendar list
            Dim list As IList(Of CalendarListEntry) = Service.CalendarList.List().Execute().Items()

            ' Display all calendars

            Dim lstLog As String = "Lists of calendars: " & vbCrLf
            For Each item As CalendarListEntry In list
                lstLog &= item.Summary & ". Location: " & item.Location & ", TimeZone: " & item.TimeZone & vbCrLf
            Next
            If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", lstLog)

            For Each calendar As Data.CalendarListEntry In list
                ' Display calendar's events
                If calendar.Summary.ToUpper = _CalHomidomName.ToUpper Then
                    CalendarHomidom = calendar
                    _IsConnect = True
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", "Calendrier " & calendar.Summary.ToUpper & " trouvé dans le compte ")
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", "Driver " & Me.Nom & " démarré")

                    ' Recherche le calendrier des jours Fériés  
                ElseIf calendar.Summary.ToUpper = _CalFeriesName.ToUpper Then
                    Calendarferies = calendar
                    If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", "Calendrier Jours fériés trouvé dans le compte ")
                End If
            Next

            'Recherche le calendrier Homidom 
            'For Each entry In CalendarFeed.Entries
            'If entry.Title.Text.ToUpper = _CalHomidomName.ToUpper Then
            'CalendarHomidom = entry
            '_IsConnect = True
            '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", "Calendrier HoMIDoM trouvé dans le compte " & _UserName)
            '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", "Driver " & Me.Nom & " démarré")

            ' Recherche le calendrier des jours Fériés  
            'ElseIf entry.Title.Text.ToUpper = _CalFeriesName.ToUpper Then
            'Calendarferies = entry
            'If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", "Calendrier Jours fériés trouvé dans le compte " & _UserName)
            'End If
            'Next

            ' Traitement en cas de calendrier non trouvé
            If CalendarHomidom Is Nothing And Calendarferies Is Nothing Then
                ' pas de demarrage du drives
                _IsConnect = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar", "Calendrier " & _CalFeriesName.ToUpper & " non trouvé dans le compte ")
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar", "Driver " & Me.Nom & " non démarré")
            End If

            If _Refresh > 0 Then
                MyTimer.Interval = _Refresh * 1000
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar Start", ex.Message & ex.Data.ToString)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop

        Try
            If _Refresh > 0 Then
                MyTimer.Enabled = False
                RemoveHandler MyTimer.Elapsed, AddressOf TimerTick
            End If
            _IsConnect = False
            ' Service = Nothing

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
    ''' <remarks>pas utilisé</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>

    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write

        ' Creation de l'evenement
        '   Dim entryNew As New Data.Event
        ' Postionnement de l'heure de l'evenement à NOW avec un durée de 5 min 
        '   Dim eventTime As New [When](DateTime.Now, DateTime.Now.AddMinutes(5))

        Try
            '   If Not (IsNothing(Objet)) Then
            '   entryNew.Title.Text = Objet.Name & ":" & Objet.value.ToString

            ' Positionne l'heure de l'evenement 
            '   entryNew.Times.Add(eventTime)

            ' Envoie la requete de creation de l'evenement         
            '   Dim PostUri As New Uri(CalendarHomidom.Content.AbsoluteUri)
            '   Dim insertedEntry As AtomEntry = Service.Insert(PostUri, entryNew)
            '   End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & "Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & "DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & "NewDevice", ex.Message)
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

            ' liste des devices compatibles    
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            ' _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)

            ' Défintion des Paramètres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("Calendrier Homidom", "Nom du calendrier réservé à HoMIDoM", "homidom")
            Add_ParamAvance("Calendrier Jours Fériés", "Nom du calendrier réservé aux jours fériés du pays", "")

            Add_LibelleDevice("ADRESSE1", "Valeur à rechercher", "Titre de l'evenement à rechercher")
            Add_LibelleDevice("ADRESSE2", "Element à recuperer (Titre,Lieu,Description)", "Information de l'événement à retourner")

            str_to_week.Add("MO", 1)
            str_to_week.Add("TU", 2)
            str_to_week.Add("WE", 3)
            str_to_week.Add("TH", 4)
            str_to_week.Add("FR", 5)
            str_to_week.Add("SA", 6)
            str_to_week.Add("SU", 0)
            str_to_week.Add(1, "MO")
            str_to_week.Add(2, "TU")
            str_to_week.Add(3, "WE")
            str_to_week.Add(4, "TH")
            str_to_week.Add(5, "FR")
            str_to_week.Add(6, "SA")
            str_to_week.Add(0, "SU")

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        ' Attente de 3s pour eviter le relancement de la procedure dans le laps de temps
        'System.Threading.Thread.Sleep(3000)
        ScanCalendar()
    End Sub

#End Region

#Region "Fonctions internes"

    Public Sub CreateEvent(ByVal Objet As Object, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing)

        '   Dim entryNew As New EventEntry()

        Try
            '   If _Enable = False Then Exit Sub

            '   _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write ", "Creation d'un événement " & Objet)

            ' Positionne le titre et le commentaire de l'evenement 
            '   If Not (IsNothing(Objet)) Then entryNew.Title.Text = Objet.name.ToString & ":" & Objet.value.ToString
            '   If Not (IsNothing(Parametre1)) Then entryNew.Content.Content = Parametre1

            ' Positionne le lieu de l'evenement 
            '   Dim eventLocation As New Where()
            '   If Not (IsNothing(Parametre2)) Then
            '   eventLocation.ValueString = Parametre2
            '   entryNew.Locations.Add(eventLocation)
            '   End If

            ' Positionne l'heure de l'evenement 
            '   Dim eventTime As New [When](DateTime.Now, DateTime.Now.AddMinutes(5))
            '   entryNew.Times.Add(eventTime)

            ' Envoie la requete de creation de l'evenement         
            '   Dim PostUri As New Uri(CalendarHomidom.Content.AbsoluteUri)
            '   Dim insertedEntry As AtomEntry = Service.Insert(PostUri, entryNew)

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & "Write", ex.Message)
        End Try
    End Sub

    Public Sub ScanCalendar()

        Try
            Dim request As ListRequest = Service.Events.List(CalendarHomidom.Id)

            ' Creation de la requete sur le calendrier reservé 
            'Dim queryCal As New EventQuery(CalendarHomidom.Content.AbsoluteUri.ToString)

            'queryCal.ExtraParameters = "orderby=starttime&sortorder=ascending"
            'queryCal.NumberToRetrieve = 20

            'Recherche le calendrier Homidom 
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Il est : " & Now.ToShortTimeString)

            If Not firstScan Then
                request.MaxResults = 20
                request.TimeMin = New System.DateTime(Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Year, Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Month, Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Day, Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Hour, Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Minute, 0)
                request.TimeMax = New System.DateTime(Now.AddMinutes(Math.Ceiling(Refresh / 60)).Year, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Month, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Day, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Hour, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Minute, 0)

                'queryCal.StartDate = New System.DateTime(Now.Year, Now.Month, Now.Day)
                'queryCal.StartTime = New System.DateTime(Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, 0)
                'queryCal.EndTime = New System.DateTime(Now.AddMinutes(Math.Ceiling(Refresh / 60)).Year, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Month, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Day, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Hour, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Minute, 0)
            End If
            firstScan = False

            ' Lancement de la requete de recherches des événements
            'Dim calFeed As AtomFeed = Service.Query(queryCal)
            'Dim feedEntry As Data.Event
            Dim calFeed As IList(Of Data.Event) = request.Execute().Items
            Dim EntryFind As New Data.Event
            Dim elementFound As Boolean = False
            Dim commandFound As Boolean = False

            If calFeed.Count > 0 Then

                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", " Nombres d'évenements trouvés: " & calFeed.Count)

                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, "", "", Me._ID, True)

                ' Fetch the list of events
                For Each feedEntry As Data.Event In calFeed

                    'Parcours tous les evenements 
                    'For Each feedEntry In calFeed.Entries
                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Titre : " & feedEntry.Summary & " Compte: " & feedEntry.Id)

                    Dim StartTime, EndTime As Date
                    Dim Fini As Boolean = False
                    
                    If feedEntry.Recurrence IsNot Nothing Then
                        If feedEntry.Recurrence(0) IsNot Nothing Then
                            Dim position As Integer
                            If InStr(feedEntry.Recurrence(0).ToString, "DTSTART;TZID") Then
                                position = InStr(InStr(feedEntry.Recurrence(0).ToString, "DTSTART;TZID") + 12, feedEntry.Recurrence(0).ToString, vbCrLf) - 15
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "StartTime Recurrence : " & Mid(feedEntry.Recurrence(0).ToString, position, 15) & " et position= " & position)
                                StartTime = New System.DateTime(Mid(feedEntry.Recurrence(0).ToString, position, 4), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 4, 2), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 6, 2), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 9, 2), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 11, 2), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 13, 2))
                            End If
                            If InStr(feedEntry.Recurrence(0).ToString, "DTEND;TZID") Then
                                position = InStr(InStr(feedEntry.Recurrence(0).ToString, "DTEND;TZID") + 10, feedEntry.Recurrence(0).ToString, vbCrLf) - 15
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "EndTime Recurrence : " & Mid(feedEntry.Recurrence(0).ToString, position, 15) & " et position= " & position)
                                EndTime = New System.DateTime(Mid(feedEntry.Recurrence(0).ToString, position, 4), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 4, 2), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 6, 2), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 9, 2), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 11, 2), _
                                                               Mid(feedEntry.Recurrence(0).ToString, position + 13, 2))
                            End If
                            StartTime = feedEntry.Start.DateTime
                            EndTime = feedEntry.End.DateTime
                            If InStr(feedEntry.Recurrence(0).ToString, "RRULE:") Then
                                position = InStr(InStr(feedEntry.Recurrence(0).ToString, "RRULE:") + 6, feedEntry.Recurrence(0).ToString, vbCrLf)

                                'Dim Chaine = Mid(feedEntry.Recurrence(0).ToString, InStr(feedEntry.Recurrence(0).ToString, "RRULE:"), position - InStr(feedEntry.Recurrence(0).ToString, "RRULE:")) & ";"
                                Dim Chaine = Right(feedEntry.Recurrence(0).ToString, feedEntry.Recurrence(0).Length - 6) & ";"

                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Chaine Recurrence : " & Chaine)

                                Dim Frequence As String = ""
                                Dim Interval As String = ""
                                Dim Compteur As String = ""
                                Dim Jusque As String = ""
                                Dim EndDate As String = ""
                                Dim ByDay As String = ""
                                Dim ByMonth As String = ""
                                Dim ByMonthDay As String = ""

                                If InStr(Chaine, "INTERVAL=") > 0 Then
                                    position = InStr(InStr(Chaine, "INTERVAL=") + 9, Chaine, ";")
                                    Interval = Mid(Chaine, InStr(Chaine, "INTERVAL=") + 9, position - InStr(Chaine, "INTERVAL=") - 9)
                                Else
                                    Interval = "1"
                                End If

                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Interval Recurrence : " & Interval & " Position= " & position)

                                If InStr(Chaine, "BYDAY=") > 0 Then
                                    position = InStr(InStr(Chaine, "BYDAY=") + 6, Chaine, ";")
                                    ByDay = Mid(Chaine, InStr(Chaine, "BYDAY=") + 6, position - InStr(Chaine, "BYDAY=") - 6)
                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "ByDay Recurrence : " & ByDay & " Position= " & position)
                                End If

                                If InStr(Chaine, "BYMONTHDAY=") > 0 Then
                                    position = InStr(InStr(Chaine, "BYMONTHDAY=") + 11, Chaine, ";")
                                    ByMonthDay = Mid(Chaine, InStr(Chaine, "BYMONTHDAY=") + 11, position - InStr(Chaine, "BYMONTHDAY=") - 11)
                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "ByMonthDay Recurrence : " & ByMonthDay & " Position= " & position)
                                End If

                                If InStr(Chaine, "BYMONTH=") > 0 Then
                                    position = InStr(InStr(Chaine, "BYMONTH=") + 8, Chaine, ";")
                                    ByMonth = Mid(Chaine, InStr(Chaine, "BYMONTH=") + 8, position - InStr(Chaine, "BYMONTH=") - 8)

                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "ByMonth Recurrence : " & ByMonth & " Position= " & position)

                                End If

                                If InStr(Chaine, "FREQ=") > 0 Then
                                    position = InStr(InStr(Chaine, "FREQ=") + 5, Chaine, ";")
                                    Frequence = Mid(Chaine, InStr(Chaine, "FREQ=") + 5, position - InStr(Chaine, "FREQ=") - 5)

                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Frequence Recurrence : " & Frequence & " Position= " & position)
                                    Dim Sdatesearch As Date
                                    Dim Edatesearch As Date

                                    If Frequence = "DAILY" Then
                                        Sdatesearch = StartTime
                                        Edatesearch = EndTime
                                        While Sdatesearch.Date <= Now.Date
                                            StartTime = Sdatesearch
                                            EndTime = Edatesearch
                                            Sdatesearch = Sdatesearch.AddDays(CInt(Interval))
                                            Edatesearch = Edatesearch.AddDays(CInt(Interval))
                                        End While
                                    End If

                                    If Frequence = "WEEKLY" Then
                                        If ByDay <> "" Then
                                            Sdatesearch = StartTime
                                            Edatesearch = EndTime
                                            While Sdatesearch.DayOfWeek <> Now.DayOfWeek
                                                Sdatesearch = Sdatesearch.AddDays(1)
                                                Edatesearch = Edatesearch.AddDays(1)
                                            End While
                                            If Now.DayOfWeek = Sdatesearch.DayOfWeek Then
                                                If InStr(ByDay, str_to_week(Now.DayOfWeek)) Then
                                                    While Sdatesearch.Date <= Now.Date
                                                        StartTime = Sdatesearch
                                                        EndTime = Edatesearch
                                                        Sdatesearch = Sdatesearch.AddDays(CInt(Interval) * 7)
                                                        Edatesearch = Edatesearch.AddDays(CInt(Interval) * 7)
                                                    End While
                                                End If
                                            End If
                                        End If
                                    End If

                                    If Frequence = "MONTHLY" Then
                                        Dim nb, dow, dday As Integer
                                        If ByDay <> "" Then
                                            nb = CInt(Left(ByDay, 1))
                                            dow = str_to_week(Right(ByDay, 2))
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Monthly Recurrence : Tout les " & nb & " et " & dow)

                                            Dim firstday As Date = New System.DateTime(Now.Year, Now.Month, 1)
                                            While firstday.DayOfWeek <> dow
                                                firstday = firstday.AddDays(1)
                                            End While
                                            dday = firstday.Day + ((nb - 1) * 7)
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Monthly Recurrence : DDay= " & dday)
                                        End If
                                        If ByMonthDay <> "" Then
                                            dday = CInt(ByMonthDay)
                                        End If

                                        Sdatesearch = StartTime
                                        Edatesearch = EndTime
                                        While Sdatesearch.Day <> dday
                                            Sdatesearch = Sdatesearch.AddDays(1)
                                            Edatesearch = Edatesearch.AddDays(1)
                                        End While

                                        While Sdatesearch.Date <= Now.Date
                                            StartTime = Sdatesearch
                                            EndTime = Edatesearch
                                            Sdatesearch = Sdatesearch.AddMonths(CInt(Interval))
                                            Edatesearch = Edatesearch.AddMonths(CInt(Interval))
                                        End While
                                    End If

                                    If Frequence = "YEARLY" Then
                                        Sdatesearch = StartTime
                                        Edatesearch = EndTime
                                        While Sdatesearch.Date <= Now.Date
                                            StartTime = Sdatesearch
                                            EndTime = Edatesearch
                                            Sdatesearch = Sdatesearch.AddYears(CInt(Interval))
                                            Edatesearch = Edatesearch.AddYears(CInt(Interval))
                                        End While
                                    End If

                                End If

                                If InStr(Chaine, "COUNT=") > 0 Then
                                    position = InStr(InStr(Chaine, "COUNT=") + 6, Chaine, ";")
                                    Compteur = Mid(Chaine, InStr(Chaine, "COUNT=") + 6, position - InStr(Chaine, "COUNT=") - 6)
                                    If (Frequence = "DAILY" And StartTime.AddDays(CInt(Compteur) * CInt(Interval)) > Now) Or _
                                         (Frequence = "WEEKLY" And StartTime.AddDays(CInt(Compteur) * CInt(Interval) * 7) > Now) Or _
                                         (Frequence = "MONTHLY" And StartTime.AddMonths(CInt(Compteur) * CInt(Interval)) > Now) Or _
                                         (Frequence = "YEARLY" And StartTime.AddYears(CInt(Compteur) * CInt(Interval)) > Now) Then
                                        Fini = True
                                    End If
                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Compteur Recurrence : " & Compteur)
                                End If
                                If InStr(Chaine, "UNTIL=") > 0 Then
                                    position = InStr(InStr(Chaine, "UNTIL=") + 6, Chaine, ";")
                                    Jusque = Mid(Chaine, InStr(Chaine, "UNTIL=") + 6, position - InStr(Chaine, "UNTIL=") - 7)
                                    EndDate = New System.DateTime(Mid(Jusque, 1, 4), _
                                                                                          Mid(Jusque, 5, 2), _
                                                                                          Mid(Jusque, 7, 2), _
                                                                                          Mid(Jusque, 10, 2), _
                                                                                          Mid(Jusque, 12, 2), _
                                                                                          Mid(Jusque, 14, 2))
                                    If EndDate > Now Then
                                        Fini = True
                                    End If
                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Jusqu'au Recurrence : " & Jusque)
                                End If
                            End If


                            '---------------TESTé POUR:

                            'DTSTART;TZID=Europe/Paris:20140526T080000
                            'DTEND;TZID=Europe/Paris:20140526T180000
                            'RRULE:FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR

                            'DTSTART;TZID=Europe/Paris:20140607T080000
                            'DTEND;TZID=Europe/Paris:20140607T090000
                            'RRULE:FREQ=MONTHLY;INTERVAL=2;BYMONTHDAY=7

                            'DTSTART;TZID=Europe/Paris:20140608T080000
                            'DTEND;TZID=Europe/Paris:20140608T090000
                            'RRULE:FREQ=YEARLY

                            'DTSTART;TZID=Europe/Paris:20140608T080000
                            'DTEND;TZID=Europe/Paris:20140608T090000
                            'RRULE:FREQ=DAILY;UNTIL=20140612T060000Z;INTERVAL=2

                            'DTSTART;TZID=Europe/Paris:20140607T080000
                            'DTEND;TZID=Europe/Paris:20140607T090000
                            'RRULE:FREQ=MONTHLY;COUNT=35;BYDAY=1SA
                            
                        End If
                    End If

                    If Not Fini And StartTime = System.DateTime.Today.ToShortDateString & " " & System.DateTime.Now.ToShortTimeString Then
                        commandFound = True
                        EntryFind = feedEntry

                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", " Start: " & StartTime & " End: " & EndTime & " Now: " & System.DateTime.Today.ToShortDateString & " " & System.DateTime.Now.ToShortTimeString)

                    End If


                    If feedEntry.Start.DateTime = System.DateTime.Today.ToShortDateString & " " & System.DateTime.Now.ToShortTimeString Then
                        commandFound = True
                        EntryFind = feedEntry

                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", " Start: " & feedEntry.Start.DateTime & " End: " & feedEntry.End.DateTime & " Now: " & System.DateTime.Today.ToShortDateString & " " & System.DateTime.Now.ToShortTimeString)

                    End If


                    If commandFound Then
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", EntryFind.Summary & ":" & EntryFind.Id)
                        If InStr(EntryFind.Description, ":") Then
                            Dim ParaAdr2 = Split(EntryFind.Description, ":")

                            Select Case ParaAdr2(0).ToUpper
                                Case "COMPOSANT"
                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Composant d'ID : " & ParaAdr2(1) & " L'action est : " & ParaAdr2(2))
                                    Dim TempDevice As TemplateDevice = _Server.ReturnDeviceById(_IdSrv, ParaAdr2(1))
                                    If TempDevice IsNot Nothing Then
                                        Select Case ParaAdr2(2).ToUpper
                                            Case "ON", "OFF"
                                                Dim x As DeviceAction = New DeviceAction
                                                x.Nom = ParaAdr2(2)
                                                _Server.ExecuteDeviceCommand(_IdSrv, ParaAdr2(1), x)

                                            Case "DIM", "OUVERTURE"
                                                Dim x As DeviceActionSimple = New DeviceActionSimple
                                                x.Nom = ParaAdr2(2)
                                                x.Param1 = ParaAdr2(3)
                                                _Server.ExecuteDeviceCommandSimple(_IdSrv, ParaAdr2(1), x)
                                            Case Else
                                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Commande non trouvée : " & ParaAdr2(2).ToUpper)
                                        End Select
                                    Else
                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Composant non trouvée : " & ParaAdr2(1).ToUpper)
                                    End If

                                Case "MACRO"
                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Passage par la partie Macro d'ID : " & ParaAdr2(1))
                                    Dim TempMacro As Macro = _Server.ReturnMacroById(_IdSrv, ParaAdr2(1))
                                    If TempMacro IsNot Nothing And TempMacro.Enable = True Then
                                        ' Analyse de la commande 
                                        Select Case ParaAdr2(2).ToUpper
                                            Case "START"
                                                _Server.RunMacro(_IdSrv, ParaAdr2(1))
                                            Case "STOP"

                                            Case Else
                                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Macro non trouvée : " & ParaAdr2(2).ToUpper)
                                        End Select
                                    Else
                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Composant non trouvée : " & ParaAdr2(1).ToUpper)
                                    End If

                                Case Else
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Type non trouvée : " & ParaAdr2(0).ToUpper)
                            End Select
                        Else
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & "ScanCalendar - Le nombre de parametre n'est pas correct", feedEntry.Summary)
                        End If
                    Else
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Evenement non retenu pour une commande : " & feedEntry.Summary)
                    End If

                    For Each objet As Object In listedevices

                        Dim Minut As Integer = 0
                        If InStr(objet.Adresse2, ":") Then
                            Dim Adr2 = Split(objet.Adresse2, ":")
                            If Adr2(1) <> "" Then
                                Minut = CInt(Adr2(1))
                            End If
                        End If
                        If feedEntry.Summary IsNot Nothing Then
                            If ((feedEntry.Summary.ToUpper = objet.Adresse1.ToString.ToUpper) Or (feedEntry.Summary.ToUpper = "Jours fériés en France")) Then

                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Le composant " & objet.name & " est valide pour cette évenement")

                                If Not Fini And StartTime < System.DateTime.Today.AddMinutes(Minut).ToShortDateString & " " & System.DateTime.Now.AddMinutes(Minut).ToShortTimeString And _
                                       EndTime > System.DateTime.Today.AddMinutes(Minut).ToShortDateString & " " & System.DateTime.Now.AddMinutes(Minut).ToShortTimeString Then
                                    ' Un element etre trouvé 
                                    elementFound = True
                                    EntryFind = feedEntry
                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Titre : " & feedEntry.Summary & " Compte: " & feedEntry.Id _
                                    & " Start: " & StartTime & " End: " & EndTime & " Now: " & System.DateTime.Today.ToShortDateString & " " & System.DateTime.Now.ToShortTimeString)

                                End If

                                'For Each Times In feedEntry.Times

                                If feedEntry.Start.DateTime < System.DateTime.Today.AddMinutes(Minut).ToShortDateString & " " & System.DateTime.Now.AddMinutes(Minut).ToShortTimeString And _
                                   feedEntry.End.DateTime > System.DateTime.Today.AddMinutes(Minut).ToShortDateString & " " & System.DateTime.Now.AddMinutes(Minut).ToShortTimeString Then
                                    ' Un element etre trouvé 
                                    elementFound = True
                                    EntryFind = feedEntry
                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Titre : " & feedEntry.Summary & " Compte: " & feedEntry.Id _
                                    & "Start: " & feedEntry.Start.DateTime & " End: " & feedEntry.End.DateTime & " Now: " & System.DateTime.Today.ToShortDateString & " " & System.DateTime.Now.ToShortTimeString)

                                End If
                                'Next

                                Select Case objet.Type
                                    Case "GENERIQUESTRING"
                                        If elementFound Then
                                            Select Case objet.adresse2.ToString.ToUpper
                                                Case "TITRE"
                                                    objet.setValue(EntryFind.Summary)

                                                Case "DESCRIPTION"
                                                    objet.setValue(EntryFind.Description)

                                                Case "LIEU"
                                                    objet.setValue(EntryFind.Location)

                                            End Select

                                        Else
                                            objet.setValue("Evénement non trouvé")
                                        End If


                                    Case "GENERIQUEBOOLEEN"
                                        objet.setValue(elementFound)

                                    Case Else
                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur du type du composant de " & objet.Adresse1)
                                End Select

                            Else
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Le composant " & objet.name & " n'est pas valide pour cette évenement")
                            End If
                        Else
                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Le composant " & objet.name & " n'est pas valide pour cette évenement")
                        End If

                    Next

                    elementFound = False
                    commandFound = False
                Next

            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ScanCalendar", ex.Message)
        End Try
    End Sub

#End Region

End Class

Namespace GoogleOauthAPI
    Public Class StoredResponse
        Public Property access_token() As String
        Public Property token_type() As String
        Public Property expires_in() As Long?
        Public Property refresh_token() As String
        Public Property Issued() As String


        Public Sub New(ByVal pRefreshToken As String)
            'this.access_token = pAccessToken;
            ' this.token_type = pTokenType;
            'this.expires_in = pExpiresIn;
            Me.refresh_token = pRefreshToken
            'this.Issued = pIssued;
            Me.Issued = Date.MinValue.ToString()
        End Sub
        Public Sub New()
            Me.Issued = Date.MinValue.ToString()
        End Sub


    End Class

    ''' <summary>
    ''' Saved data store that implements <seealso cref="IDataStore"/>. 
    ''' This Saved data store stores a StoredResponse object.
    ''' </summary>
    Friend Class SavedDataStore
        Implements IDataStore

        Public Property _storedResponse() As StoredResponse

        ''' <summary>
        ''' Constructs Load perviously saved StoredResponse.
        ''' </summary>

        Public Sub New(ByVal pResponse As StoredResponse)
            Me._storedResponse = pResponse
        End Sub
        Public Sub New()
            Me._storedResponse = New StoredResponse()
        End Sub
        ''' <summary>
        ''' Stores the given value. into storedResponse
        ''' </summary>
        ''' <typeparam name="T">The type to store in the data store</typeparam>
        ''' <param name="key">The key</param>
        ''' <param name="value">The value to store in the data store</param>
        Public Function StoreAsync(Of T)(ByVal key As String, ByVal value As T) As Task Implements IDataStore.StoreAsync

            Dim serialized = NewtonsoftJsonSerializer.Instance.Serialize(value)

            Dim jObject As jObject = Newtonsoft.Json.Linq.jObject.Parse(serialized)

            ' storing access token
            Dim test = jObject.SelectToken("access_token")
            If test IsNot Nothing Then
                Me._storedResponse.access_token = CStr(test)
            End If
            ' storing token type
            test = jObject.SelectToken("token_type")
            If test IsNot Nothing Then
                Me._storedResponse.token_type = CStr(test)
            End If
            test = jObject.SelectToken("expires_in")
            If test IsNot Nothing Then
                Me._storedResponse.expires_in = CType(test, Long?)
            End If
            test = jObject.SelectToken("refresh_token")
            If test IsNot Nothing Then
                Me._storedResponse.refresh_token = CStr(test)
            End If
            test = jObject.SelectToken("Issued")
            If test IsNot Nothing Then
                Me._storedResponse.Issued = CStr(test)
            End If

            Return TaskEx.Delay(0)
        End Function

        ''' <summary>
        ''' Deletes StoredResponse.
        ''' </summary>
        ''' <param name="key">The key to delete from the data store</param>
        Public Function DeleteAsync(Of T)(ByVal key As String) As Task Implements IDataStore.DeleteAsync
            Me._storedResponse = New StoredResponse()

            Return TaskEx.Delay(0)
        End Function


        ''' Returns the stored value for_storedResponse      
        ''' <typeparam name="T">The type to retrieve</typeparam>
        ''' <param name="key">The key to retrieve from the data store</param>
        ''' <returns>The stored object</returns>
        Public Function GetAsync(Of T)(ByVal key As String) As Task(Of T) Implements IDataStore.GetAsync
            Dim tcs As New TaskCompletionSource(Of T)()
            Try

                Dim JsonData As String = Newtonsoft.Json.JsonConvert.SerializeObject(Me._storedResponse)

                tcs.SetResult(Google.Apis.Json.NewtonsoftJsonSerializer.Instance.Deserialize(Of T)(JsonData))
            Catch ex As Exception
                tcs.SetException(ex)
            End Try

            Return tcs.Task
        End Function

        ''' <summary>
        ''' Clears all values in the data store. 
        ''' </summary>
        Public Function ClearAsync() As Task Implements IDataStore.ClearAsync
            Me._storedResponse = New StoredResponse()
            Return TaskEx.Delay(0)
        End Function

        '/// <summary>Creates a unique stored key based on the key and the class type.</summary>
        '/// <param name="key">The object key</param>
        '/// <param name="t">The type to store or retrieve</param>
        'public static string GenerateStoredKey(string key, Type t)
        '{
        '    return string.Format("{0}-{1}", t.FullName, key);
        '}
    End Class
End Namespace