
'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Xml
Imports System.Net
Imports System.Threading


''' <summary>Driver Google Meteo, le device doit indique sa ville dans son Adresse 1</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_Weather
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "8B8EFC7E-F66D-11E1-AD40-71AF6188709B"
    Dim _Nom As String = "MeteoWeather"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Meteo provenant de Weather Channel"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "WEB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Google Weather"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264" 'Pkateforme compatible 32 64 ou 3264
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
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True

    'param avancé
    Dim _DEBUG As Boolean = False
#End Region

#Region "Variables internes"
    Dim _Obj As Object
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
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1"
                    If Value IsNot Nothing Then
                        If String.IsNullOrEmpty(Value) Or IsNumeric(Value) Then
                            retour = "Veuillez saisir le code de la ville, ex: FRXX0151"
                        End If
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
        Try
            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
            End Try

            _IsConnect = True
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " démarré")

            If _IsConnect Then
                For Each _Dev As HoMIDom.HoMIDom.TemplateDevice In _Server.GetAllDevices(_IdSrv)
                    If _Dev.Type = ListeDevices.METEO Then
                        Read(_Server.ReturnRealDeviceById(_Dev.ID))
                        Thread.Sleep(5000)
                    End If
                Next
            End If
        Catch ex As Exception
            _IsConnect = False
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " arrêté")
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
            If _Enable = False Then Exit Sub
            If Objet.type = "METEO" Then
                _Obj = Objet
                Dim y As New Thread(AddressOf MAJ)
                y.Start()
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Seul les composants de type meteo peuvent etre mis à jour. Les composants dépendants sont mis à jour automatiquement en même temps que la météo.")
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
            _DeviceSupport.Add(ListeDevices.METEO)
            _DeviceSupport.Add(ListeDevices.BAROMETRE)
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.UV)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Code Ville", "Code de la ville dans weather channel", "")
            Add_LibelleDevice("ADRESSE2", "@", "", "")
            Add_LibelleDevice("MODELE", "@", "", "")
            Add_LibelleDevice("SOLO", "@", "")
            'Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
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

    Private Sub MAJ()
        If _Obj Is Nothing Then Exit Sub

        Try
            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                Exit Sub
            End If

            Dim doc As New XmlDocument
            Dim nodes As XmlNodeList
            Dim _StrBar As String = ""
            Dim _StrDirVent As String = ""
            Dim _StrHum As String = ""
            Dim _StrUv As String = ""
            Dim _StrVitVent As String = ""
            Dim _StrTemp As String = ""

            ' Create a new XmlDocument   
            doc = New XmlDocument()
            Dim url As New Uri("http://xml.weather.com/weather/local/" & _Obj.adresse1 & "?cc=*&unit=m&dayf=4")
            Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
            ' Request.UserAgent = "Mozilla/5.0 (windows; U; windows NT 5.1; fr; rv:1.8.0.7) Gecko/20060909 Firefox/1.5.0.7"
            Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)

            doc.Load(response.GetResponseStream)

            nodes = doc.SelectNodes("/weather/cc")

            For Each node As XmlNode In nodes
                If node.HasChildNodes = True Then
                    For Each _child As XmlNode In node
                        Select Case _child.Name
                            Case "tmp" : If IsNumeric(_child.FirstChild.Value) Then
                                    _Obj.TemperatureActuel = _child.FirstChild.Value
                                    _StrTemp = _child.FirstChild.Value
                                End If
                            Case "t" : _Obj.ConditionActuel = Traduire(_child.FirstChild.Value)
                            Case "icon" : _Obj.IconActuel = _child.FirstChild.Value
                            Case "hmid" : If IsNumeric(_child.FirstChild.Value) Then
                                    _Obj.HumiditeActuel = _child.FirstChild.Value
                                    _StrHum = _child.FirstChild.Value
                                End If
                                'Case "i" : _StrUv = _child.FirstChild.Value
                            Case "bar"
                                For Each _child2 As XmlNode In _child
                                    Select Case _child2.Name
                                        Case "r" : _StrBar = _child2.FirstChild.Value
                                    End Select
                                Next
                            Case "wind"
                                For Each _child2 As XmlNode In _child
                                    Select Case _child2.Name
                                        Case "s"
                                            _Obj.VentActuel = _child2.FirstChild.Value
                                            _StrVitVent = _child2.FirstChild.Value
                                        Case "t" : _StrDirVent = _child2.FirstChild.Value
                                    End Select
                                Next
                            Case "uv"
                                For Each _child2 As XmlNode In _child
                                    Select Case _child2.Name
                                        Case "i" : _StrUv = _child2.InnerText
                                    End Select
                                Next
                        End Select
                    Next
                End If
            Next

            nodes = doc.SelectNodes("/weather/dayf/day")

            Dim idx As Integer = -1

            For Each node As XmlNode In nodes
                idx = idx + 1

                Select Case idx
                    Case 0 : _Obj.JourToday = TraduireJour(Mid(Now.DayOfWeek.ToString, 1, 3))
                    Case 1 : _Obj.JourJ1 = TraduireJour(Mid(Now.AddDays(1).DayOfWeek.ToString, 1, 3))
                    Case 2 : _Obj.JourJ2 = TraduireJour(Mid(Now.AddDays(2).DayOfWeek.ToString, 1, 3))
                    Case 3 : _Obj.JourJ3 = TraduireJour(Mid(Now.AddDays(3).DayOfWeek.ToString, 1, 3))
                End Select

                If node.HasChildNodes = True Then
                    For Each _child As XmlNode In node
                        If _child.HasChildNodes Then
                            Select Case _child.Name
                                Case "hi"
                                    If IsNumeric(_child.FirstChild.Value) Then
                                        Select Case idx
                                            Case 0 : _Obj.MaxToday = _child.FirstChild.Value
                                            Case 1 : _Obj.MaxJ1 = _child.FirstChild.Value
                                            Case 2 : _Obj.MaxJ2 = _child.FirstChild.Value
                                            Case 3 : _Obj.MaxJ3 = _child.FirstChild.Value
                                        End Select
                                    End If
                                Case "low"
                                    If IsNumeric(_child.FirstChild.Value) Then
                                        Select Case idx
                                            Case 0 : _Obj.MinToday = _child.FirstChild.Value
                                            Case 1 : _Obj.MinJ1 = _child.FirstChild.Value
                                            Case 2 : _Obj.MinJ2 = _child.FirstChild.Value
                                            Case 3 : _Obj.MinJ3 = _child.FirstChild.Value
                                        End Select
                                    End If
                                Case "icon"
                                Case "t"
                                    Select Case idx
                                        Case 0 : _Obj.ConditionToday = Traduire(_child.FirstChild.Value)
                                        Case 1 : _Obj.ConditionJ1 = Traduire(_child.FirstChild.Value)
                                        Case 2 : _Obj.ConditionJ2 = Traduire(_child.FirstChild.Value)
                                        Case 3 : _Obj.ConditionJ3 = Traduire(_child.FirstChild.Value)
                                    End Select
                            End Select
                        End If

                        If _child.HasChildNodes = True Then
                            For Each _child2 As XmlNode In _child
                                Select Case _child2.Name
                                    Case "hi"
                                        If IsNumeric(_child.FirstChild.InnerText) Then
                                            Select Case idx
                                                Case 0 : _Obj.MaxToday = _child2.InnerText
                                                Case 1 : _Obj.MaxJ1 = _child2.InnerText
                                                Case 2 : _Obj.MaxJ2 = _child2.InnerText
                                                Case 3 : _Obj.MaxJ3 = _child2.InnerText
                                            End Select
                                        End If
                                    Case "low"
                                        If IsNumeric(_child.FirstChild.InnerText) Then
                                            Select Case idx
                                                Case 0 : _Obj.MinToday = _child2.InnerText
                                                Case 1 : _Obj.MinJ1 = _child2.InnerText
                                                Case 2 : _Obj.MinJ2 = _child2.InnerText
                                                Case 3 : _Obj.MinJ3 = _child2.InnerText
                                            End Select
                                        End If
                                    Case "icon"
                                        Select Case idx
                                            Case 0 : _Obj.IconToday = _child2.InnerText
                                            Case 1 : _Obj.IconJ1 = _child2.InnerText
                                            Case 2 : _Obj.IconJ2 = _child2.InnerText
                                            Case 3 : _Obj.IconJ3 = _child2.InnerText
                                        End Select
                                    Case "t"
                                        Select Case idx
                                            Case 0 : _Obj.ConditionToday = Traduire(_child2.InnerText)
                                            Case 1 : _Obj.ConditionJ1 = Traduire(_child2.InnerText)
                                            Case 2 : _Obj.ConditionJ2 = Traduire(_child2.InnerText)
                                            Case 3 : _Obj.ConditionJ3 = Traduire(_child2.InnerText)
                                        End Select
                                End Select
                                'If _child2.Name.StartsWith("#text") = False Then Console.WriteLine(_child2.Name & ":" & _child2.InnerText)

                            Next
                        End If

                    Next

                End If
            Next


            Dim listedevices As New ArrayList
            Dim i As Integer = 0
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, _Obj.Adresse1, "HUMIDITE", Me._ID, True)
            For i = 0 To listedevices.Count - 1
                listedevices.Item(i).Value = Replace(_StrHum, ".", ",")
            Next
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, _Obj.Adresse1, "BAROMETRE", Me._ID, True)
            For i = 0 To listedevices.Count - 1
                listedevices.Item(i).Value = Replace(_StrBar, ".", ",")
            Next
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, _Obj.Adresse1, "DIRECTIONVENT", Me._ID, True)
            For i = 0 To listedevices.Count - 1
                listedevices.Item(i).Value = Replace(_StrDirVent, ".", ",")
            Next
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, _Obj.Adresse1, "UV", Me._ID, True)
            For i = 0 To listedevices.Count - 1
                listedevices.Item(i).Value = Replace(_StrUv, ".", ",")
            Next
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, _Obj.Adresse1, "VITESSEVENT", Me._ID, True)
            For i = 0 To listedevices.Count - 1
                listedevices.Item(i).Value = Replace(_StrVitVent, ".", ",")
            Next
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, _Obj.Adresse1, "TEMPERATURE", Me._ID, True)
            For i = 0 To listedevices.Count - 1
                listedevices.Item(i).Value = Replace(_StrTemp, ".", ",")
            Next

            _StrBar = ""
            _StrDirVent = ""
            _StrHum = ""
            _StrUv = ""
            _StrVitVent = ""
            _StrTemp = ""
            doc = Nothing
            nodes = Nothing
            response = Nothing
            Request = Nothing
            url = Nothing
            _Obj.LastChange = Now

            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEOWeather", "MAJ Meteo effectuée pour " & _Obj.name)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "METEOWeather", "Erreur Lors de la MaJ de " & _Obj.name & " : " & ex.ToString)
        End Try
    End Sub

    Private Function ExtractFile(ByVal File As String) As String
        Try
            Dim result As String = ""
            Dim j As Integer = Len(File)
            Dim tmp As Integer = 0

            For i As Integer = 1 To Len(File)
                If Mid(File, i, 1) = "/" Then
                    tmp = i
                End If
            Next

            If tmp > 0 Then
                result = Mid(File, tmp + 1, j - tmp)
            End If

            result = result.Replace(".gif", "")
            Return result
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function TraduireJour(ByVal Jour As String) As String
        TraduireJour = "?"
        Select Case Jour
            Case "Thu"
                TraduireJour = "Jeu"
            Case "Fri"
                TraduireJour = "Ven"
            Case "Sat"
                TraduireJour = "Sam"
            Case "Sun"
                TraduireJour = "Dim"
            Case "Mon"
                TraduireJour = "Lun"
            Case "Tue"
                TraduireJour = "Mar"
            Case "Wed"
                TraduireJour = "Mer"
        End Select
    End Function

    Private Function Traduire(ByVal txt As String) As String
        Try
            Return Traduct(txt)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "METEOWeather", "Traduire: " & ex.Message)
            Return txt
        End Try
    End Function

    Public Function Traduct(Txt As String) As String
        Try
            Dim _txt As String = Trim(Txt).ToLower.Replace("  ", " ")
            Dim _return As String = ""
            Dim xReader As XmlReader
            Dim xDoc As XElement

            If System.IO.File.Exists(Server.GetRepertoireOfServer & "\Fichiers\Traduc_FR.xml") = False Then
                Dim xmlName2 As String = ".Traduc_FR.xml"
                ' get the current executing assembly, and append the resources name
                Dim strResources2 As String = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & xmlName2
                ' use stream to load up the resources
                Using s As IO.Stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(strResources2)

                    'use xmlreader to create in-memory xml structured
                    xReader = XmlReader.Create(s)

                    xDoc = XElement.Load(xReader)
                    xDoc.Save(Server.GetRepertoireOfServer & "\Fichiers\Traduc_FR.xml")
                    xReader.Close()
                End Using
            Else
                xReader = XmlReader.Create(Server.GetRepertoireOfServer & "\Fichiers\Traduc_FR.xml")
                xDoc = XElement.Load(xReader)
                xReader.Close()
            End If


            'Dim xmlName As String = ".Traduc_FR.xml"
            '' get the current executing assembly, and append the resources name
            'Dim strResources As String = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & xmlName
            '' use stream to load up the resources
            'Using s As IO.Stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(strResources)

            'use xmlreader to create in-memory xml structured


            'xDoc = XElement.Load(xReader)
            Dim employees As IEnumerable(Of XElement) = xDoc.Elements()

            For Each employee In employees
                If employee.Element("eng").Value.ToLower = _txt Then
                    _return = employee.Element("fr").Value
                    Exit For
                End If
            Next employee

            'End Using
            Return _return
        Catch ex As Exception
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEOWeather", "Erreur Traduct: " & ex.ToString)
            Return ""
        End Try
    End Function
#End Region

End Class
