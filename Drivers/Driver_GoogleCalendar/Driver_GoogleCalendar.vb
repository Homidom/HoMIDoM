'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports Google.GData.Client
Imports Google.GData.Extensions
Imports Google.GData.Calendar


Public Class Driver_GoogleCalendar

    ' Auteur : Laurent
    ' Date : 19/04/2013  Creation du driver


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

        'Ajoutés dans les ppt avancés dans New()
        Dim _DEBUG As Boolean = False
        Dim _UserName As String
        Dim _PassWord As String
        Dim _CalHomidomName As String
        Dim _CalFeriesName As String

#End Region

#Region "Variables Internes"
        Dim Service As CalendarService = New CalendarService("HoMIDoM")
        Dim CalendarHomidom As New CalendarEntry
        Dim Calendarferies As New CalendarEntry

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
                If value >= 1 Then
                    _Refresh = value

                End If
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
            Dim queryCal As New CalendarQuery()
            Dim CalendarFeed As CalendarFeed

            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _UserName = _Parametres.Item(1).Valeur
                _PassWord = _Parametres.Item(2).Valeur
                _CalHomidomName = _Parametres.Item(3).Valeur
                _CalFeriesName = _Parametres.Item(4).Valeur

                Service.setUserCredentials(_UserName, _PassWord)        ' Positionne le compte Gmail à utiliser
                queryCal.Uri = New Uri("https://www.google.com/calendar/feeds/default/allcalendars/full")
                ' Recupere la liste des calendriers du compte 
                CalendarFeed = Service.Query(queryCal)

                'Recherche le calendrier Homidom 
                For Each entry In CalendarFeed.Entries
                    If entry.Title.Text.ToUpper = _CalHomidomName.ToUpper Then
                        CalendarHomidom = entry
                        _IsConnect = True
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", "Calendrier HoMIDoM trouvé dans le compte " & _UserName)
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", "Driver " & Me.Nom & " démarré")

                    ElseIf entry.Title.Text.ToUpper = _CalFeriesName.ToUpper Then
                        Calendarferies = entry
                        If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GoogleCalendar", "Calendrier Jours fériés trouvé dans le compte " & _UserName)
                    End If
                Next

                If IsNothing(CalendarHomidom.SelfUri) Then
                    ' pas de calendrier trouvé 
                    _IsConnect = False
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar", "Calendrier HoMIDoM non trouvé dans le compte " & _UserName)
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar", "Driver " & Me.Nom & " non démarré")
                End If

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar Start", ex.Message)
            End Try
        End Sub

        ''' <summary>Arrêter le du driver</summary>
        ''' <remarks></remarks>
        Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop

            Try
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

            Dim elementFound As Boolean = False
            Dim query As EventQuery = New EventQuery
            If Objet.Adresse1.ToString.ToUpper = "FERIE" Then
                query.Uri = New Uri(Calendarferies.Content.AbsoluteUri)
            Else
                query.Uri = New Uri(CalendarHomidom.Content.AbsoluteUri)
            End If

            Dim calFeed As AtomFeed = Service.Query(query)
            Dim feedEntry As EventEntry
            Dim EntryFind As New EventEntry

            Try
                If _Enable = False Then Exit Sub
                If _IsConnect = False Then Exit Sub

                ' Une requete a trouvé un événement
                If Not IsNothing(calFeed) Then
                    ' L'objet est valide 
                    If Objet IsNot Nothing Then

                        'If _DEBUG Then Console.WriteLine("la date du jour est : " & System.DateTime.Today.ToShortDateString)
                        'Parcourt toutes les entrees 
                        For Each feedEntry In calFeed.Entries
                            If _DEBUG Then Console.WriteLine(feedEntry.Title.Text & vbTab & feedEntry.Content.Content & vbTab & feedEntry.Authors.FirstOrDefault.Name _
                                              & vbTab & "#" & feedEntry.Times.Item(0).StartTime & "#" & vbCrLf & feedEntry.Times.Item(0).AllDay)


                            If ((feedEntry.Title.Text.ToUpper = Objet.Adresse1.ToString.ToUpper) Or (Objet.Adresse1.ToString.ToUpper = "FERIE")) And feedEntry.Times.Item(0).StartTime = System.DateTime.Today.ToShortDateString Then
                                ' Un element etre trouvé 
                                elementFound = True
                                EntryFind = feedEntry
                                Exit For
                            End If

                        Next

                        Select Case Objet.Type
                            Case "GENERIQUESTRING"
                                If elementFound Then
                                    Select Case Objet.adresse2.ToString.ToUpper
                                        Case "TITRE"
                                            Objet.Value = EntryFind.Title.ToString

                                        Case "DESCRIPTION"
                                            Objet.Value = EntryFind.Content.ToString

                                        Case "LIEU"
                                            Objet.Value = EntryFind.Locations.ToString

                                    End Select

                                Else
                                    Objet.Value = "Evénement non trouvé"
                                End If


                            Case "GENERIQUEBOOLEEN"
                                Objet.Value = elementFound

                            Case Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur du type du composant de " & Objet.Adresse1)
                        End Select



                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "L'objet est invalide")
                    End If
                End If





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
            Try
                If _Enable = False Then Exit Sub
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

                'Paramétres avancés
                Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
                Add_ParamAvance("Compte Google", "Adresse de messagerie Google", "")
                Add_ParamAvance("Mot de passe", "Mot de passe du compte Google", "")
                Add_ParamAvance("Calendrier Homidom", "Nom du calendrier réservé à HoMIDoM", "homidom")
                Add_ParamAvance("Calendrier Jours Fériés", "Nom du calendrier réservé aux jours fériés du pays", "")



                Add_LibelleDevice("ADRESSE1", "Valeur à rechercher", "Titre de l'evenement à rechercher")
                Add_LibelleDevice("ADRESSE2", "Element à recuperer (Titre,Lieu,Description)", "Information de l'événement à retourner")

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
            End Try
        End Sub

        ''' <summary>Si refresh >0 gestion du timer</summary>
        ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
        Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

        End Sub

#End Region

#Region "Fonctions internes"


#End Region

    End Class

End Class
