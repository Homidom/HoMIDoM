'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Xml
Imports System.Net
Imports System.Threading

' Auteur : Seb
' Date : 10/02/2011

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
                        If CStr(Value) = "" Or CStr(Value) = " " Or IsNumeric(Value) Then
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
        Catch ex As Exception
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
        'Dim objet As Object = _Obj
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

                        Select Case _child.Name
                            Case "hi"
                                Select Case idx
                                    Case 0 : If IsNumeric(_child.FirstChild.Value) Then _Obj.MaxToday = _child.FirstChild.Value
                                    Case 1 : If IsNumeric(_child.FirstChild.Value) Then _Obj.MaxJ1 = _child.FirstChild.Value
                                    Case 2 : If IsNumeric(_child.FirstChild.Value) Then _Obj.MaxJ2 = _child.FirstChild.Value
                                    Case 3 : If IsNumeric(_child.FirstChild.Value) Then _Obj.MaxJ3 = _child.FirstChild.Value
                                End Select
                            Case "low"
                                Select Case idx
                                    Case 0 : If IsNumeric(_child.FirstChild.Value) Then _Obj.MinToday = _child.FirstChild.Value
                                    Case 1 : If IsNumeric(_child.FirstChild.Value) Then _Obj.MinJ1 = _child.FirstChild.Value
                                    Case 2 : If IsNumeric(_child.FirstChild.Value) Then _Obj.MinJ2 = _child.FirstChild.Value
                                    Case 3 : If IsNumeric(_child.FirstChild.Value) Then _Obj.MinJ3 = _child.FirstChild.Value
                                End Select
                            Case "icon"
                            Case "t"
                                Select Case idx
                                    Case 0 : _Obj.ConditionToday = Traduire(_child.FirstChild.Value)
                                    Case 1 : _Obj.ConditionJ1 = Traduire(_child.FirstChild.Value)
                                    Case 2 : _Obj.ConditionJ2 = Traduire(_child.FirstChild.Value)
                                    Case 3 : _Obj.ConditionJ3 = Traduire(_child.FirstChild.Value)
                                End Select
                        End Select
                        If _child.HasChildNodes = True Then
                            For Each _child2 As XmlNode In _child
                                Select Case _child2.Name
                                    Case "hi"
                                        Select Case idx
                                            Case 0 : If IsNumeric(_child2.InnerText) Then _Obj.MaxToday = _child2.InnerText
                                            Case 1 : If IsNumeric(_child2.InnerText) Then _Obj.MaxJ1 = _child2.InnerText
                                            Case 2 : If IsNumeric(_child2.InnerText) Then _Obj.MaxJ2 = _child2.InnerText
                                            Case 3 : If IsNumeric(_child2.InnerText) Then _Obj.MaxJ3 = _child2.InnerText
                                        End Select
                                    Case "low"
                                        Select Case idx
                                            Case 0 : If IsNumeric(_child2.InnerText) Then _Obj.MinToday = _child2.InnerText
                                            Case 1 : If IsNumeric(_child2.InnerText) Then _Obj.MinJ1 = _child2.InnerText
                                            Case 2 : If IsNumeric(_child2.InnerText) Then _Obj.MinJ2 = _child2.InnerText
                                            Case 3 : If IsNumeric(_child2.InnerText) Then _Obj.MinJ3 = _child2.InnerText
                                        End Select
                                    Case "icon"
                                        Select Case idx
                                            Case 0
                                                _Obj.IconToday = _child2.InnerText
                                            Case 1
                                                _Obj.IconJ1 = _child2.InnerText
                                            Case 2
                                                _Obj.IconJ2 = _child2.InnerText
                                            Case 3
                                                _Obj.IconJ3 = _child2.InnerText
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
            'objet = Nothing

            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GOOGLEMETEO", "MAJ Meteo effectuée pour " & _Obj.name)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "METEOWeather", "Erreur Lors de la MaJ de " & _Obj.name & " : " & ex.Message)
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

            Dim _txt As String = Trim(txt).ToLower.Replace("  ", " ")

            If _txt = "a few clouds" Then
                Return "Quelques nuages"
            End If
            If _txt = "a few clouds and breezy" Then
                Return "Quelques nuages et Frais"
            End If
            If _txt = "a few clouds and windy" Then
                Return "Quelques nuages Grand vent"
            End If
            If _txt = "a few clouds with haze" Then
                Return "Quelques nuages et brume"
            End If
            If _txt = "am clouds / pm sun" Then
                Return "Matin nuages / Après-Midi Soleil"
            End If
            If _txt = "am fog / pm sun" Then
                Return "Matin Brouillard / Après-Midi Soleil"
            End If
            If _txt = "am rain / snow showers" Then
                Return "Matin pluie / averses neigeuses"
            End If
            If _txt = "am showers" Then
                Return "Averses matinales"
            End If
            If _txt = "am snow showers" Then
                Return "Averses de neige le matin"
            End If
            If _txt = "am t-storms" Then
                Return "Orages le matin"
            End If
            If _txt = "blowing dust" Then
                Return "Vent de poussière"
            End If
            If _txt = "blowing sand" Then
                Return "Vent de sable"
            End If
            If _txt = "blowing snow" Then
                Return "Vent de neige"
            End If
            If _txt = "blowing snow in vicinity" Then
                Return "Vent de neige dans les environs"
            End If
            If _txt = "clear" Then
                Return "Clair"
            End If
            If _txt = "clear / wind" Then
                Return "Clair / vent"
            End If
            If _txt = "clear and breezy" Then
                Return "Clair et frais"
            End If
            If _txt = "clear with haze" Then
                Return "Clair avec brume légère"
            End If
            If _txt = "clear and windy" Then
                Return "Clair et venteux"
            End If
            If _txt = "cloudy clair" Then
                Return "Nuageux clair"
            End If
            If _txt = "clouds early / clearing kate" Then
                Return "Nuages matinaux suivis d'éclaircies"
            End If
            If _txt = "cloudy" Then
                Return "Nuageux"
            End If
            If _txt = "cloudy / wind" Then
                Return "Nuageux / Vent"
            End If
            If _txt = "cloudy / windy" Then
                Return "Nuageux et venteux"
            End If
            If _txt = "cloudy and windy" Then
                Return "Nuageux et venteux"
            End If
            If _txt = "drifting snow" Then
                Return "Amoncellement de neige"
            End If
            If _txt = "drizzle" Then
                Return "crachin"
            End If
            If _txt = "drizzle fog/mist" Then
                Return "Crachin brouillard/brume"
            End If
            If _txt = "drizzle fog" Then
                Return "Crachin brouillard"
            End If
            If _txt = "drizzle ice pellets" Then
                Return "Crachin grêlons"
            End If
            If _txt = "drizzle snow" Then
                Return "Crachin neige"
            End If
            If _txt = "dust" Then
                Return "Poussière"
            End If
            If _txt = "dust storm in vicinity" Then
                Return "Tempête de poussière dans les environs"
            End If
            If _txt = "dust storm" Then
                Return "Tempête de poussière"
            End If
            If _txt = "dust/sand whirls" Then
                Return "Tourbillons de poussière/sable"
            End If
            If _txt = "dust/sand whirls in vicinity" Then
                Return "Tourbillons de poussière/sable dans les environs"
            End If
            If _txt = "fair" Then
                Return "Ciel dégagé"
            End If
            If _txt = "fair/ windy" Then
                Return "Ciel dégagé / grand vent"
            End If
            If _txt = "fair and breezy" Then
                Return "Ciel dégagé et frais"
            End If
            If _txt = "fair and windy" Then
                Return "Ciel dégagé et grand vent"
            End If
            If _txt = "fair with haze" Then
                Return "Ciel dégagé avec brume légère"
            End If
            If _txt = "few showers" Then
                Return "Quelques averses"
            End If
            If _txt = "few snow showers" Then
                Return "Quelques averses de neige"
            End If
            If _txt = "few snow showers / wind" Then
                Return "Quelques averses de neige / Vent"
            End If
            If _txt = "fog" Then
                Return "Brouillard"
            End If
            If _txt = "fog in vicinity" Then
                Return "Brouillard dans les environs"
            End If
            If _txt = "fog/mist" Then
                Return "Brouillard/brume"
            End If
            If _txt = "freezing drizzle" Then
                Return "Crachin givrant"
            End If
            If _txt = "freezing drizzle in vicinity" Then
                Return "Crachin givrant dans les environs"
            End If
            If _txt = "freezing drizzle rain" Then
                Return "Crachin pluie givrant "
            End If
            If _txt = "freezing drizzle snow" Then
                Return "Crachin neige givrant"
            End If
            If _txt = "freezing fog" Then
                Return "Brouillard givrant"
            End If
            If _txt = "freezing fog in vicinity" Then
                Return "Brouillard givrant dans les environs"
            End If
            If _txt = "freezing rain" Then
                Return "Pluie givrante"
            End If
            If _txt = "freezing rain in vicinity" Then
                Return "Pluie givrante dans les environs"
            End If
            If _txt = "freezing rain snow" Then
                Return "Pluie neige givrante dans les environs"
            End If
            If _txt = "frigid" Then
                Return "Grand froid"
            End If
            If _txt = "funnel cloud in vicinity" Then
                Return "Nuage en entonnoir dans les environs"
            End If
            If _txt = "funnel cloud" Then
                Return "Nuage en entonnoir"
            End If
            If _txt = "hail" Then
                Return "Grêle"
            End If
            If _txt = "hail showers" Then
                Return "Averses de grêle"
            End If
            If _txt = "haze" Then
                Return "Brume légère"
            End If
            If _txt = "heavy drizzle" Then
                Return "Gros Crachin "
            End If
            If _txt = "heavy drizzle gog/mist" Then
                Return "Gros Crachin Brouillard/brume"
            End If
            If _txt = "heavy drizzle fog" Then
                Return "Gros Crachin Brouillard"
            End If
            If _txt = "heavy drizzle ice pellets" Then
                Return "Gros Crachin Grêlons"
            End If
            If _txt = "heavy drizzle snow" Then
                Return "Gros Crachin Neige"
            End If
            If _txt = "heavy Dust storm" Then
                Return "Grosse tempête de poussière"
            End If
            If _txt = "heavy freezing drizzle rain" Then
                Return "Gros Crachin verglassant Pluie"
            End If
            If _txt = "heavy freezing drizzle snow" Then
                Return "Gros Crachin verglassant Neige"
            End If
            If _txt = "heavy freezing drizzle" Then
                Return "Gros Crachin verglassant"
            End If
            If _txt = "heavy freezing fog" Then
                Return "Gros brouillard verglassant"
            End If
            If _txt = "heavy freezing rain snow" Then
                Return "Grosse pluie verglassante Neige"
            End If
            If _txt = "heavy freezing rain" Then
                Return "Grosse pluie verglassante"
            End If
            If _txt = "heavy ice pellets" Then
                Return "Gros Grêlons"
            End If
            If _txt = "heavy ice pellets drizzle" Then
                Return "Gros Grêlons Crachin"
            End If
            If _txt = "heavy ice pellets rain" Then
                Return "Gros Grêlons Pluie"
            End If
            If _txt = "heavy rain" Then
                Return "Grosse pluie"
            End If
            If _txt = "heavy rain fog/mist" Then
                Return "Grosse pluie Brouillard/brume"
            End If
            If _txt = "heavy rain fog" Then
                Return "Grosse pluie Brouillard"
            End If
            If _txt = "heavy rain freezing drizzle" Then
                Return "Grosse pluie Crachin verglassant"
            End If
            If _txt = "heavy rain freezing rain" Then
                Return "Grosse pluie Pluie verglassante"
            End If
            If _txt = "heavy rain ice pellets" Then
                Return "Grosse pluie Grêlons"
            End If
            If _txt = "heavy rain icy" Then
                Return "Grosse pluie verglassante"
            End If
            If _txt = "heavy rain showers fog/mist" Then
                Return "Grosse pluie Averses Brouillard/brume"
            End If
            If _txt = "heavy rain showers" Then
                Return "Grosse pluie Averses"
            End If
            If _txt = "heavy rain snow" Then
                Return "Grosse pluie Neige"
            End If
            If _txt = "heavy Sand storm" Then
                Return "Grosse tempête de sable "
            End If
            If _txt = "heavy showers rain fog/mist" Then
                Return "Grosses averses Pluie Brouillard/brume"
            End If
            If _txt = "heavy showers rain" Then
                Return "Grosses averses Pluie"
            End If
            If _txt = "heavy showers snow fog/mist" Then
                Return "Grosses Neige Pluie Brouillard/brume"
            End If
            If _txt = "heavy showers snow fog" Then
                Return "Grosses averses Neige Brouillard"
            End If
            If _txt = "heavy showers snow" Then
                Return "Grosses averses Neige"
            End If
            If _txt = "heavy snow" Then
                Return "Beaucoup de neige"
            End If
            If _txt = "heavy snow drizzle" Then
                Return "Beaucoup de neige Crachin"
            End If
            If _txt = "heavy snow fog/mist" Then
                Return "Beaucoup de neige Brouillard/brume"
            End If
            If _txt = "heavy snow fog" Then
                Return "Beaucoup de neige Brouillard"
            End If
            If _txt = "heavy snow freezing drizzle" Then
                Return "Beaucoup de neige Crachin verglassant"
            End If
            If _txt = "heavy snow freezing rain" Then
                Return "Beaucoup de neige Pluie verglassante"
            End If
            If _txt = "heavy snow rain" Then
                Return "Beaucoup de neige Pluie"
            End If
            If _txt = "heavy snow showers fog/mist" Then
                Return "Beaucoup de neige Averses Brouillard/brume"
            End If
            If _txt = "heavy snow showers fog" Then
                Return "Beaucoup de neige Averses Brouillard"
            End If
            If _txt = "heavy snow showers" Then
                Return "Beaucoup de neige Averses"
            End If
            If _txt = "heavy t-storm" Then
                Return "Gros orage"
            End If
            If _txt = "heavy t-storms rain fog/mist" Then
                Return "Gros orage Brouillard/brume"
            End If
            If _txt = "heavy t-storms rain fog" Then
                Return "Gros orage Brouillard"
            End If
            If _txt = "heavy t-storms rain hail fog/mist" Then
                Return "Gros orage Grêle Brouillard/brume"
            End If
            If _txt = "heavy t-storms rain hail fog" Then
                Return "Gros orage Grêle  Brouillard"
            End If
            If _txt = "heavy t-storms rain hail haze" Then
                Return "Gros orage Grêle Brume légère"
            End If
            If _txt = "heavy t-storms rain hail" Then
                Return "Gros orage Grêle"
            End If
            If _txt = "heavy t-storms rain haze" Then
                Return "Gros orage Brume légère"
            End If
            If _txt = "heavy t-storms rain" Then
                Return "Gros orage Pluie"
            End If
            If _txt = "heavy t-storms snow" Then
                Return "Gros orage Neige"
            End If
            If _txt = "ice crystals" Then
                Return "Cristaux de glace"
            End If
            If _txt = "ice pellets drizzle" Then
                Return "Grêlons Crachin"
            End If
            If _txt = "ice pellets in vicinity" Then
                Return "Grêlons dans les environs "
            End If
            If _txt = "ice pellets rain" Then
                Return "Grêlons Pluie"
            End If
            If _txt = "ice pellets" Then
                Return "Grêlons"
            End If
            If _txt = "Isokated t-storms" Then
                Return "Orages isolés"
            End If
            If _txt = "light drizzle" Then
                Return "Léger crachin"
            End If
            If _txt = "light drizzle fog/mist" Then
                Return "Léger crachin Brouillard/brume"
            End If
            If _txt = "light drizzle fog" Then
                Return "Léger crachin Brouillard"
            End If
            If _txt = "light drizzle ice pellets" Then
                Return "Léger crachin Grêlons"
            End If
            If _txt = "light drizzle snow" Then
                Return "Léger crachin Neige"
            End If
            If _txt = "light freezing drizzle" Then
                Return "Léger crachin verglassant"
            End If
            If _txt = "light freezing drizzle rain" Then
                Return "Léger crachin verglassant Pluie"
            End If
            If _txt = "light freezing drizzle snow" Then
                Return "Léger crachin verglassant Neige"
            End If
            If _txt = "light freezing fog" Then
                Return "Léger brouillard verglassant"
            End If
            If _txt = "light freezing rain" Then
                Return "Légère pluie verglassante"
            End If
            If _txt = "light freezing rain snow" Then
                Return "Légère pluie verglassante Neige"
            End If
            If _txt = "light ice pellets drizzle" Then
                Return "Petit Grêlons Crachin"
            End If
            If _txt = "light ice pellets rain" Then
                Return "Petit Grêlons Pluie"
            End If
            If _txt = "light ice pellets" Then
                Return "Petit Grêlons"
            End If
            If _txt = "light rain" Then
                Return "Légère Pluie"
            End If
            If _txt = "light rain Early" Then
                Return "Légère Pluie matinal"
            End If
            If _txt = "light rain fog/mist" Then
                Return "Légère Pluie Brouillard/brume"
            End If
            If _txt = "light rain fog" Then
                Return "Légère Pluie Brouillard"
            End If
            If _txt = "light rain freezing drizzle" Then
                Return "Légère Pluie Crachin verglassant"
            End If
            If _txt = "light rain freezing rain" Then
                Return "Légère Pluie Pluie verglassante"
            End If
            If _txt = "light rain ice pellets" Then
                Return "Légère Pluie Grêlons"
            End If
            If _txt = "light rain icy" Then
                Return "Légère pluie verglassante"
            End If
            If _txt = "light rain kate" Then
                Return "Légère Pluie tardive"
            End If
            If _txt = "light rain shower" Then
                Return "Légère pluie Averse"
            End If
            If _txt = "light rain shower and windy" Then
                Return "Légère pluie Averses et Vent"
            End If
            If _txt = "light rain showers" Then
                Return "Légères pluie Averses"
            End If
            If _txt = "light rain snow" Then
                Return "Légères pluie Neige"
            End If
            If _txt = "light rain with thunder" Then
                Return "Légère Pluie avec tonnerre"
            End If
            If _txt = "light showers rain" Then
                Return "Légère averses Pluie"
            End If
            If _txt = "light showers rain fog/mist" Then
                Return "Légère averses Pluie Brouillard/brume"
            End If
            If _txt = "light showers snow" Then
                Return "Légère averses Neige"
            End If
            If _txt = "light showers snow fog" Then
                Return "Légère averses Neige Brouillard"
            End If
            If _txt = "light showers snow fog/mist" Then
                Return "Légère averses Neige Brouillard/brume"
            End If
            If _txt = "light snow" Then
                Return "Peu de neige"
            End If
            If _txt = "light snow drizzle" Then
                Return "Peu de neige Crachin"
            End If
            If _txt = "light snow Fall" Then
                Return "Légère chutes de neige"
            End If
            If _txt = "light snow fog/mist" Then
                Return "Peu de neige Brouillard/brume"
            End If
            If _txt = "light snow fog" Then
                Return "Peu de neige Brouillard"
            End If
            If _txt = "light snow freezing drizzle" Then
                Return "Peu de neige Crachin verglassant"
            End If
            If _txt = "light snow freezing rain" Then
                Return "Peu de neige Pluie Verglassante"
            End If
            If _txt = "light snow grains" Then
                Return "Quelques flocons de neige"
            End If
            If _txt = "light snow rain" Then
                Return "Peu de neige Pluie"
            End If
            If _txt = "light snow shower" Then
                Return "Légère averse de neige"
            End If
            If _txt = "light snow showers fog/mist" Then
                Return "Légère averses de neige Brouillard/brume"
            End If
            If _txt = "light snow showers fog" Then
                Return "Légère averses de neige Brouillard"
            End If
            If _txt = "light t-storms rain fog/mist" Then
                Return "Léger orages Pluie Brouillard/brume"
            End If
            If _txt = "light t-storms rain fog" Then
                Return "Léger orages Pluie Brouillard"
            End If
            If _txt = "light t-storms rain hail fog/mist" Then
                Return "Léger orages Pluie Grêle Brouillard/brume"
            End If
            If _txt = "light t-storms rain hail fog" Then
                Return "Léger orages Pluie Grêle Brouillard"
            End If
            If _txt = "light t-storms rain hail haze" Then
                Return "Léger orages Pluie Grêle Brume légère"
            End If
            If _txt = "light t-storms rain hail" Then
                Return "Léger orages Pluie Grêle"
            End If
            If _txt = "light t-storms rain haze" Then
                Return "Léger orages Pluie Brume légère"
            End If
            If _txt = "light t-storms rain" Then
                Return "Léger orages Pluie"
            End If
            If _txt = "light t-storms snow" Then
                Return "Léger orages Neige"
            End If
            If _txt = "lightening" Then
                Return "Eclairs"
            End If
            If _txt = "lightenings" Then
                Return "Eclairs"
            End If
            If _txt = "mostly clear" Then
                Return "Ciel plutôt dégagé"
            End If
            If _txt = "mist" Then
                Return "Brume"
            End If
            If _txt = "mostly cloudy" Then
                Return "Plutôt nuageux"
            End If
            If _txt = "mostly cloudy and breezy" Then
                Return "Plutôt nuageux et Venteux"
            End If
            If _txt = "mostly cloudy and windy" Or txt = "mostly cloudy/wind" Then
                Return "Plutôt nuageux et Grand vent"
            End If
            If _txt = "mostly cloudy with haze" Then
                Return "Plutôt nuageux avec Légère Brume"
            End If
            If _txt = "mostly sunny" Then
                Return "Plutôt ensoleillé"
            End If
            If _txt = "mostly sunny / wind" Then
                Return "Plutôt ensoleillé / vent"
            End If
            If _txt = "overcast" Then
                Return "Couvert"
            End If
            If _txt = "overcast and Breezy" Then
                Return "Couvert et Venteux"
            End If
            If _txt = "overcast and windy" Then
                Return "Couvert et Grand vent"
            End If
            If _txt = "overcast with haze" Then
                Return "Couvert avec légère brume"
            End If
            If _txt = "partial fog" Then
                Return "Banc de Brouillard"
            End If
            If _txt = "partial fog in vicinity" Then
                Return "Banc de Brouillard dans les environs"
            End If
            If _txt = "p cloudy" Then
                Return "Partiellement nuageux"
            End If
            If _txt = "partly cloudy" Then
                Return "Partiellement nuageux"
            End If
            If _txt = "partly cloudy and breezy" Then
                Return "Partiellement nuageux et Venteux"
            End If
            If _txt = "partly cloudy and windy" Then
                Return "Partiellement nuageux et Grand vent"
            End If
            If _txt = "partly cloudy / wind" Then
                Return "Partiellement nuageux / Vent"
            End If
            If _txt = "partly cloudy/ windy" Or txt = "partly cloudy/wind" Then
                Return "Partiellement nuageux / Venteux"
            End If
            If _txt = "partly cloudy with haze" Then
                Return "Partiellement nuageux avec légère brume"
            End If
            If _txt = "partly sunny" Then
                Return "Partiellement ensoleillé"
            End If
            If _txt = "patches of fog" Then
                Return "Nappes de Brouillard"
            End If
            If _txt = "patches of fog in vicinity" Then
                Return "Nappes de Brouillard dans les environs"
            End If
            If _txt = "pm light rain" Then
                Return "PM Légère Pluie"
            End If
            If _txt = "pm rain / snow" Then
                Return "PM Pluie / Neige"
            End If
            If _txt = "pm rain / wind" Then
                Return "PM Pluie / Vent"
            End If
            If _txt = "pm showers" Then
                Return "PM Averses"
            End If
            If _txt = "pm snow showers" Then
                Return "PM averses neigeuses"
            End If
            If _txt = "pm t-storms" Then
                Return "Orages l après-midi"
            End If
            If _txt = "rain" Then
                Return "Pluie"
            End If
            If _txt = "rain / snow" Then
                Return "Pluie / neige"
            End If
            If _txt = "rain / snow showers" Then
                Return "Pluie / averses neigeuses"
            End If
            If _txt = "rain / snow showers early" Then
                Return "Pluie / averses neigeuses matinales"
            End If
            If _txt = "rain / thunder" Then
                Return "Pluie / Tonnerre"
            End If
            If _txt = "rain / wind" Then
                Return "Pluie / Vent"
            End If
            If _txt = "rain and snow" Then
                Return "Pluie et Neige"
            End If
            If _txt = "rain early" Then
                Return "Pluie matinale"
            End If
            If _txt = "rain fog/mist" Then
                Return "Pluie Brouillard/brume"
            End If
            If _txt = "rain fog" Then
                Return "Pluie Brouillard"
            End If
            If _txt = "rain freezing drizzle" Then
                Return "Pluie Crachin verglassant"
            End If
            If _txt = "rain freezing rain" Then
                Return "Pluie Pluie Verglassante"
            End If
            If _txt = "rain ice pellets" Then
                Return "Pluie Grêlons"
            End If
            If _txt = "rain shower" Then
                Return "Pluie Averses"
            End If
            If _txt = "rain showers fog/mist" Then
                Return "Pluie Averses Brouillard/brume"
            End If
            If _txt = "rain showers in vicinity fog/mist" Then
                Return "Pluie Averses dans les environs Brouillard/brume"
            End If
            If _txt = "rain showers in vicinity" Then
                Return "Pluie Averses dans les environs"
            End If
            If _txt = "rain snow" Then
                Return "Pluie Neige"
            End If
            If _txt = "rain to snow" Then
                Return "Pluie vers Neige"
            End If
            If _txt = "sand storm" Then
                Return "Tempête de Sable"
            End If
            If _txt = "sand storm in vicinity" Then
                Return "Tempête de Sable dans les environs"
            End If
            If _txt = "sand" Then
                Return "Sable"
            End If
            If _txt = "shallow fog" Then
                Return "Brouillard superficiel"
            End If
            If _txt = "shallow fog in vicinity" Then
                Return "Brouillard superficiel dans les environs"
            End If
            If _txt = "scattered showers" Then
                Return "Averses éparses"
            End If
            If _txt = "scattered showers / wind" Then
                Return "Averses éparses / Vent"
            End If
            If _txt = "scattered snow showers" Then
                Return "Averses neigeuses éparses"
            End If
            If _txt = "scattered snow showers / wind" Then
                Return "Averses neigeuses éparses / Vent"
            End If
            If _txt = "scattered strong storms" Then
                Return "Violents orages locals"
            End If
            If _txt = "scattered t-storms" Then
                Return "Orages éparses"
            End If
            If _txt = "showers" Then
                Return "Averses"
            End If
            If _txt = "showers / wind" Then
                Return "Averses / Vent"
            End If
            If _txt = "showers hail" Then
                Return "Averses Grêle"
            End If
            If _txt = "showers ice pellets" Then
                Return "Averses Grêlons"
            End If
            If _txt = "showers in the vicinity" Then
                Return "Averses dans les environs"
            End If
            If _txt = "showers in vicinity fog/mist" Then
                Return "Averses dans les environs Brouillard/brume"
            End If
            If _txt = "showers in vicinity fog" Then
                Return "Averses dans les environs Brouillard"
            End If
            If _txt = "showers in vicinity haze" Then
                Return "Averses dans les environs Brume légère"
            End If
            If _txt = "showers in vicinity snow" Then
                Return "Averses dans les environs Neige"
            End If
            If _txt = "showers Early" Then
                Return "Averses matinales"
            End If
            If _txt = "showers kate" Then
                Return "Averses tardives"
            End If
            If _txt = "showers rain" Then
                Return "Averses Pluie"
            End If
            If _txt = "showers rain fog/mist" Then
                Return "Averses Pluie Brouillard/brume"
            End If
            If _txt = "showers rain in vicinity" Then
                Return "Averses Pluie dans les environs"
            End If
            If _txt = "showers rain in vicinity fog/mist" Then
                Return "Averses Pluie dans les environs Brouillard/brume"
            End If
            If _txt = "showers snow" Then
                Return "Averses Neige"
            End If
            If _txt = "showers snow fog" Then
                Return "Averses Neige Brouillard"
            End If
            If _txt = "showers snow fog/mist" Then
                Return "Averses Neige Brouillard/brume"
            End If
            If _txt = "smoke" Then
                Return "fumée"
            End If
            If _txt = "snow" Then
                Return "Neige"
            End If
            If _txt = "snow / rain icy mix" Then
                Return "Mélange Neige / Pluie Verglassante"
            End If
            If _txt = "snow and fog" Then
                Return "Neige et Brouillard"
            End If
            If _txt = "snow drizzle" Then
                Return "Neige Crachin"
            End If
            If _txt = "snow fog/mist" Then
                Return "Neige Brouillard/brume"
            End If
            If _txt = "snow freezing drizzle" Then
                Return "Neige Crachin verglassant"
            End If
            If _txt = "snow freezing rain" Then
                Return "Neige Pluie verglassante"
            End If
            If _txt = "snow rain" Then
                Return "Neige Pluie"
            End If
            If _txt = "snow shower" Then
                Return "Averses de neige"
            End If
            If _txt = "snow shower / wind" Then
                Return "Averses de neige / vent"
            End If
            If _txt = "snow shower early" Then
                Return "Averses de neige matinales"
            End If
            If _txt = "snow showers fog/mist" Then
                Return "Averses de neige Brouillard/brume"
            End If
            If _txt = "snow showers fog" Then
                Return "Averses de neige Brouillard"
            End If
            If _txt = "snow showers in vicinity" Then
                Return "Averses de neige dans les environs"
            End If
            If _txt = "snow showers in vicinity fog" Then
                Return "Averses de neige dans les environs Brouillard"
            End If
            If _txt = "snow showers in vicinity fog/mist" Then
                Return "Averses de neige dans les environs Brouillard/brume"
            End If
            If _txt = "snow showers kate" Then
                Return "Averses de neige tardives"
            End If
            If _txt = "snow to rain" Then
                Return "Neige vers Pluie"
            End If
            If _txt = "snowflakes" Then
                Return "Flocons de neige"
            End If
            If _txt = "sunny" Then
                Return "Ensoleillé"
            End If
            If _txt = "sunny and widy" Then
                Return "Ensoleillé et vent"
            End If
            If _txt = "sunny / wind" Then
                Return "Ensoleillé / Vent"
            End If
            If _txt = "sunny day" Then
                Return "Journée ensoleillé"
            End If
            If _txt = "thunder" Then
                Return "Tonnerre"
            End If
            If _txt = "thunder in the vicinity" Then
                Return "Tonnerre dans les environs"
            End If
            If _txt = "t-storm" Then
                Return "Orage"
            End If
            If _txt = "t-storms" Then
                Return "Orages"
            End If
            If _txt = "t-storms early" Then
                Return "Orages matinaux"
            End If
            If _txt = "t-storms fog" Then
                Return "Orages Brouillard"
            End If
            If _txt = "t-storms hail fog" Then
                Return "Orages Grêle Brouillard"
            End If
            If _txt = "t-storms hail" Then
                Return "Orages Grêle"
            End If
            If _txt = "t-storms haze in vicinity hail" Then
                Return "Orages brume dans les environs Grêle"
            End If
            If _txt = "t-storms haze in vicinity" Then
                Return "Orages brume dans les environs"
            End If
            If _txt = "t-storms heavy rain" Then
                Return "Orages Grosse Pluie"
            End If
            If _txt = "t-storms heavy rain fog" Then
                Return "Orages Grosse Pluie Brouillard"
            End If
            If _txt = "t-storms heavy rain fog/mist" Then
                Return "Orages Grosse pluie Brouillard/brume"
            End If
            If _txt = "t-storms heavy rain hail fog" Then
                Return "Orages Grosse Pluie Grêle Brouillard"
            End If
            If _txt = "t-storms heavy rain hail fog/mist" Then
                Return "Orages Grosse Pluie Grêle Brouillard/brume"
            End If
            If _txt = "t-storms heavy rain hail haze" Then
                Return "Orages Grosse Pluie Grêle Brume légère"
            End If
            If _txt = "t-storms heavy rain hail" Then
                Return "Orages Grosse Pluie Grêle"
            End If
            If _txt = "t-storms heavy rain haze" Then
                Return "Orages Grosse Pluie Brume légère"
            End If
            If _txt = "t-storms ice pellets" Then
                Return "Orages Grêlons"
            End If
            If _txt = "t-storms in vicinity" Then
                Return "Orages dans les environs"
            End If
            If _txt = "t-storms in vicinity fog" Then
                Return "Orages dans les environs Brouillard"
            End If
            If _txt = "t-storms in vicinity fog/mist" Then
                Return "Orages dans les environs Brouillard/brume"
            End If
            If _txt = "t-storms in vicinity hail fog/mist" Then
                Return "Orages dans les environs Grêle Brouillard/brume"
            End If
            If _txt = "t-storms in vicinity hail haze" Then
                Return "Orages dans les environs Grêle Brume légère"
            End If
            If _txt = "t-storms in vicinity hail" Then
                Return "Orages dans les environs Grêle"
            End If
            If _txt = "t-storms in vicinity haze" Then
                Return "Orages dans les environs Brume légère"
            End If
            If _txt = "t-storms light rain" Then
                Return "Orages Légère pluie"
            End If
            If _txt = "t-storms light rain fog" Then
                Return "Orages Légère pluie Brouillard"
            End If
            If _txt = "t-storms light rain fog/mist" Then
                Return "Orages Légère pluie Brouillard/brume"
            End If
            If _txt = "t-storms light rain hail" Then
                Return "Orages Légère pluie Grêle"
            End If
            If _txt = "t-storms light rain hail fog" Then
                Return "Orages Légère pluie Grêle Brouillard"
            End If
            If _txt = "t-storms light rain hail fog/mist" Then
                Return "Orages Légère pluie Grêle Brouillard/brume"
            End If
            If _txt = "t-storms light rain hail haze" Then
                Return "Orages Légère pluie Grêle Brume légère"
            End If
            If _txt = "t-storms light rain haze" Then
                Return "Orages Légère pluie Brume légère"
            End If
            If _txt = "t-storms rain fog/mist" Then
                Return "Orages Pluie Brouillard/brume"
            End If
            If _txt = "t-storms rain hail fog/mist" Then
                Return "Orages Pluie Grêle Brouillard/brume"
            End If
            If _txt = "t-storms showers in vicinity" Then
                Return "Orages Averses dans les environs"
            End If
            If _txt = "t-storms showers in vicinity hail" Then
                Return "Orages Averses dans les environs Grêle"
            End If
            If _txt = "t-storms snow" Then
                Return "Orages Neige"
            End If
            If _txt = "windy" Then
                Return "Grand vent"
            End If
            If _txt = "windy / snowy" Then
                Return "Grand vent / neigeux"
            End If
            If _txt = "windy rain" Then
                Return "giboulées"
            End If
            If _txt = "wintry mix" Then
                Return "Mélanges pluie/neige"
            End If
            If _txt = "wintry mix / wind" Then
                Return "Mélanges pluie/neige / vent"
            End If

            Return txt

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
