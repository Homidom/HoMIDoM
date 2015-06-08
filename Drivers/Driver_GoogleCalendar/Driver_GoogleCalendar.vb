'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Text.RegularExpressions
Imports STRGS = Microsoft.VisualBasic.Strings

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

    Dim CalendarHomidom As New calendar
    Dim Calendarferies As New calendar

    Dim obj As Object
    Dim Auth As HoMIDom.HoMIDom.Authentication

    Dim str_to_week As New Dictionary(Of String, String)
    Dim firstScan As Boolean = True
    Dim cpt_restart As Integer = 0

    Public Class calendarList
        Public kind As String
        Public items As List(Of calendar)
    End Class

    Public Class calendar
        Public id As String
        Public summary As String
    End Class

    Public Class eventsList
        Public items As List(Of [event])
    End Class

    Public Class [event]
        Public id As String
        Public summary As String
        Public status As String
        Public description As String
        Public location As String
        Public [start] As [Date]
        Public [end] As [Date]
        Public recurrence As List(Of String)
    End Class

    Public Class [Date]
        Public [date] As Date
        Public [dateTime] As DateTime
        Public timeZone As String
    End Class

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
            Try
                ' Récuperation des parametres avancés
                _DEBUG = _Parametres.Item(0).Valeur
                _CalHomidomName = _Parametres.Item(1).Valeur
                _CalFeriesName = _Parametres.Item(2).Valeur

                If String.IsNullOrEmpty(_CalFeriesName) And String.IsNullOrEmpty(_CalHomidomName) Then
                    WriteLog("ERR: Veuillez renseigner un nom de calendrier dans les paramétres avancés.")
                    WriteLog("Driver " & Me.Nom & "non démarré")
                    Exit Sub
                End If

            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
                WriteLog("Driver " & Me.Nom & "non démarré")
                Exit Sub
            End Try

            Dim fileName = My.Application.Info.DirectoryPath & "\config\reponse_accesstoken_GoogleCalendar.json"

            If System.IO.File.Exists(fileName) Then

                Dim stream = System.IO.File.ReadAllText(fileName)
                Auth = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(HoMIDom.HoMIDom.Authentication))
                'va chercher les module que si connecté
                If Auth.expires_in > 0 Then
                    WriteLog("DBG: Token : " & Auth.access_token)

                    Dim client As New Net.WebClient
                    Dim responsebody = client.DownloadString("https://www.googleapis.com/calendar/v3/users/me/calendarList?access_token=" & Auth.access_token)
                    Dim list As calendarList = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(calendarList))
                    ' Display all calendars

                    Dim lstLog As String = "Lists of calendars: " & vbCrLf
                    For Each item As calendar In list.items
                        lstLog &= item.summary & " ID : " & item.id & vbCrLf
                    Next
                    If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, lstLog)

                    For Each calendar As calendar In list.items
                        ' Display calendar's events
                        If calendar.summary.ToUpper = _CalHomidomName.ToUpper Then
                            CalendarHomidom = calendar
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Calendrier " & calendar.summary.ToUpper & " trouvé dans le compte ")

                            ' Recherche le calendrier des jours Fériés  
                        ElseIf calendar.summary.ToUpper = _CalFeriesName.ToUpper Then
                            Calendarferies = calendar
                            If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Calendrier Jours fériés trouvé dans le compte ")
                        End If
                    Next

                    ' Traitement en cas de calendrier non trouvé
                    If CalendarHomidom Is Nothing And Calendarferies Is Nothing Then
                        ' pas de demarrage du drives
                        _IsConnect = False
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, "Calendrier " & _CalFeriesName.ToUpper & " et " & _CalHomidomName.ToUpper & " non trouvé dans le compte ")
                        WriteLog("Driver " & Me.Nom & "non démarré")
                        Exit Sub
                    Else
                        _IsConnect = True
                        If _Refresh > 0 Then
                            MyTimer.Interval = _Refresh * 1000
                            MyTimer.Enabled = True
                            AddHandler MyTimer.Elapsed, AddressOf TimerTick
                        End If
                        cpt_restart = 0
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " démarré")
                    End If
                Else
                    _IsConnect = False
                    WriteLog("Driver " & Me.Nom & "non démarré")
                End If
            Else

                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, "Le fichier : " & fileName & " n'a pas été trouvé ")
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " non démarré")
                Exit Sub
            End If
            cpt_restart = 0

        Catch ex As Exception
            cpt_restart += 1
            If cpt_restart < 4 Then
                RefreshToken("GoogleCalendar", "https://www.googleapis.com/oauth2/v3/token")
                Start()
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar Start", ex.Message & ex.Data.ToString)
                WriteLog("ERR:Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
            End If
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
        scanCalendar()
    End Sub

#End Region

#Region "Fonctions internes"


    Private Sub RefreshToken(ByVal clientOauth As String, ByVal httpsOauth As String)
        Try
            Dim client As New Net.WebClient
            Dim reqparm As New Specialized.NameValueCollection
            reqparm.Add("client_id", _Server.GetClientFile(clientOauth).web.client_id)
            reqparm.Add("client_secret", _Server.GetClientFile(clientOauth).web.client_secret)
            reqparm.Add("refresh_token", Auth.refresh_token)
            reqparm.Add("grant_type", "refresh_token")
            Dim responsebytes = client.UploadValues(httpsOauth, "POST", reqparm)
            Dim responsebody = (New System.Text.UTF8Encoding).GetString(responsebytes)
            Dim Oauth As Authentication = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(Authentication))
            Auth.access_token = Oauth.access_token
            Dim stream = Newtonsoft.Json.JsonConvert.SerializeObject(Auth)
            System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\config\reponse_accesstoken_" & clientOauth & ".json", stream)
            If Oauth.expires_in > 0 Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " RefreshToken : ", "Requête " & httpsOauth & " OK")
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " RefreshToken", "Connect : " & responsebody.ToString)
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " RefreshToken", "Non connecté")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " RefreshToken", "Exception : " & ex.Message)
        End Try
    End Sub


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

    Sub scanCalendar()


        'GET  https://www.googleapis.com/calendar/v3/calendars/calendarId/events?maxResults=10&singleEvents=true&pageToken=CiAKGjBpNDd2Nmp2Zml2cXRwYjBpOXA

        Dim calFeed(2) As eventsList
        Dim responsebody0 As Object = New Object
        Dim responsebody1 As Object = New Object

        Try
            If String.IsNullOrEmpty(CalendarHomidom.id) And String.IsNullOrEmpty(Calendarferies.id) Then
                WriteLog("ERR:Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config et redemarrer le driver")
                [Stop]()
                Exit Sub
            End If

            Dim client As New Net.WebClient

            Dim xTimeMin = New System.DateTime(Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Year, Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Month, Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Day, Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Hour, Now.AddMinutes(Math.Ceiling(Refresh / 60) * (-1)).Minute, 0)
            Dim xTimeMax = New System.DateTime(Now.AddMinutes(Math.Ceiling(Refresh / 60)).Year, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Month, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Day, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Hour, Now.AddMinutes(Math.Ceiling(Refresh / 60)).Minute, 0)

            If Not String.IsNullOrEmpty(CalendarHomidom.id) Then
                responsebody0 = client.DownloadString("https://www.googleapis.com/calendar/v3/calendars/" & CalendarHomidom.id & "/events?" & "maxResults=20&singleEvents=true&TimeMin=" & xTimeMin & "&TimeMax=" & xTimeMax & "&access_token=" & Auth.access_token)
                WriteLog("DBG: GetData : " & responsebody0.ToString)
            End If

            If Not String.IsNullOrEmpty(Calendarferies.id) Then
                responsebody1 = client.DownloadString("https://www.googleapis.com/calendar/v3/calendars/" & Calendarferies.id & "/events?" & "maxResults=20&singleEvents=true&TimeMin=" & xTimeMin & "&TimeMax=" & xTimeMax & "&access_token=" & Auth.access_token)
                WriteLog("DBG: GetData : " & responsebody1.ToString)
            End If

            cpt_restart = 0
        Catch ex As Exception

            cpt_restart += 1
            If cpt_restart < 4 Then
                RefreshToken("GoogleCalendar", "https://www.googleapis.com/oauth2/v3/token")
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ScanCalendar", ex.Message)
                WriteLog("ERR:Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                Restart()
            End If
        End Try

        Try
            ' Lancement de la requete de recherches des événements

            Dim EntryFind As New [event]
            Dim elementFound As Boolean = False
            Dim commandFound As Boolean = False
            Dim compFound As Boolean = False

            If Not String.IsNullOrEmpty(CalendarHomidom.id) Then
                calFeed(0) = (Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody0, GetType(eventsList)))
            End If

            If Not String.IsNullOrEmpty(Calendarferies.id) Then
                calFeed(1) = (Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody1, GetType(eventsList)))
            End If

            For i = 0 To 1
                If calFeed(i) IsNot Nothing Then
                    If calFeed(i).items.Count > 0 Then

                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", " Nombres d'évenements trouvés: " & calFeed(i).items.Count)

                        'Recherche si un device affecté
                        Dim listedevices As New ArrayList
                        listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, "", "", Me._ID, True)

                        ' Fetch the list of events
                        For Each feedEntry As [event] In calFeed(i).items

                            'Parcours tous les evenements 

                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Titre : " & feedEntry.summary & " Compte: " & feedEntry.id)

                            If feedEntry.start.dateTime = System.DateTime.Today.ToShortDateString & " " & System.DateTime.Now.ToShortTimeString Then
                                commandFound = True
                                compFound = True
                                EntryFind = feedEntry

                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", " Start: " & feedEntry.start.dateTime & " End: " & feedEntry.end.dateTime & " Now: " & System.DateTime.Today.ToShortDateString & " " & System.DateTime.Now.ToShortTimeString)

                            End If

                            If commandFound Then
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", EntryFind.summary & ":" & EntryFind.id)
                                If InStr(EntryFind.summary, ":") Then
                                    Dim ParaAdr2 = Split(EntryFind.summary, ":")

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
                                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & "ScanCalendar - Le nombre de parametre n'est pas correct", feedEntry.summary)
                                End If
                            Else
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Evenement non retenu pour une commande : " & feedEntry.summary)
                            End If
                            If listedevices IsNot Nothing Then

                                For Each objet As Object In listedevices
                                    If feedEntry.summary IsNot Nothing Then
                                        If ((feedEntry.summary.ToUpper = objet.Adresse1.ToString.ToUpper) Or (feedEntry.summary.ToUpper = "Jours fériés en France")) Then
                                            compFound = True
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Le composant " & objet.name & " est valide pour cette évenement")

                                            Dim Minut As Integer = 0
                                            Dim Search As String = ""
                                            If InStr(objet.Adresse2, ":") Then
                                                Dim Adr2 = Split(objet.Adresse2, ":")
                                                If Adr2(1) <> "" Then
                                                    Minut = CInt(Adr2(1))
                                                End If
                                                Search = Adr2(0)
                                            Else
                                                Search = objet.Adresse2
                                            End If

                                            If feedEntry.start.dateTime < System.DateTime.Today.AddMinutes(Minut).ToShortDateString & " " & System.DateTime.Now.AddMinutes(Minut).ToShortTimeString And _
                                                   feedEntry.end.dateTime > System.DateTime.Today.AddMinutes(Minut).ToShortDateString & " " & System.DateTime.Now.AddMinutes(Minut).ToShortTimeString Then
                                                ' Un element etre trouvé 
                                                elementFound = True
                                                EntryFind = feedEntry
                                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Titre : " & feedEntry.summary & " Compte: " & feedEntry.id _
                                                & " Start: " & feedEntry.start.dateTime & " End: " & feedEntry.end.dateTime & " Now: " & System.DateTime.Today.ToShortDateString & " " & System.DateTime.Now.ToShortTimeString)

                                            End If

                                            If elementFound Then

                                                Select Case objet.Type
                                                    Case "GENERIQUESTRING"

                                                        Select Case Search.ToString.ToUpper
                                                            Case "TITRE"
                                                                objet.setValue(EntryFind.summary)

                                                            Case "DESCRIPTION"
                                                                objet.setValue(EntryFind.description)

                                                            Case "LIEU"
                                                                objet.setValue(EntryFind.location)

                                                        End Select

                                                    Case "GENERIQUEBOOLEEN"
                                                        objet.setValue(True)

                                                    Case Else
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur du type du composant de " & objet.Adresse1)
                                                End Select
                                            Else
                                                Select Case objet.Type
                                                    Case "GENERIQUESTRING"
                                                        objet.setValue("")

                                                    Case "GENERIQUEBOOLEEN"
                                                        objet.setValue(False)

                                                    Case Else
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur du type du composant de " & objet.Adresse1)
                                                End Select
                                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "La plage horaire du composant " & objet.name & " n'est pas en cours")

                                            End If
                                        Else
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Le composant " & objet.name & " n'est pas valide pour cette évenement")
                                        End If
                                    Else
                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ScanCalendar", "Le composant " & objet.name & " n'est pas valide pour cette évenement")
                                    End If
                                Next

                               
                            End If
                            If (_AutoDiscover Or _Server.GetModeDecouverte) And Not compFound Then

                                WriteLog("DBG:AutoCreation du composant : Titre" & " à l'adresse " & feedEntry.summary)
                                _Server.AddDetectNewDevice(feedEntry.summary, _ID, "GENERIQUEBOOLEEN", "Titre")

                            End If
                            compFound = False
                            elementFound = False
                            commandFound = False
                        Next
                    End If
                End If
            Next

        Catch ex As Exception

            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ScanCalendar", ex.Message)

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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Google Calendar WriteLog", ex.Message)
        End Try
    End Sub

#End Region

End Class
