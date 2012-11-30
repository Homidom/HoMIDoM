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
    Dim _Enable As String = False
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
    Public Property Port_TCP() As Object Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As Object)
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
                        If Value = "" Or Value = " " Or IsNumeric(Value) Then
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
            _Obj = Objet
            Dim y As New Thread(AddressOf MAJ)
            y.Start()
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

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Code Ville", "Code de la ville dans weather channel", "")
            Add_LibelleDevice("ADRESSE2", "@", "", "")
            Add_LibelleDevice("MODELE", "@", "", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("REFRESH", "Refresh", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
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

    Private Sub MAJ()
        Dim objet As Object = _Obj
        If objet Is Nothing Then Exit Sub

        Try
            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                Exit Sub
            End If

            Dim doc As New XmlDocument
            Dim nodes As XmlNodeList

            ' Create a new XmlDocument   
            doc = New XmlDocument()
            Dim url As New Uri("http://xml.weather.com/weather/local/" & objet.adresse1 & "?cc=*&unit=m&dayf=4")
            Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
            ' Request.UserAgent = "Mozilla/5.0 (windows; U; windows NT 5.1; fr; rv:1.8.0.7) Gecko/20060909 Firefox/1.5.0.7"
            Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)

            doc.Load(response.GetResponseStream)

            nodes = doc.SelectNodes("/weather/cc")
            For Each node As XmlNode In nodes
                If node.HasChildNodes = True Then
                    For Each _child As XmlNode In node
                        Select Case _child.Name
                            Case "tmp"
                                If IsNumeric(_child.FirstChild.Value) Then objet.TemperatureActuel = _child.FirstChild.Value
                            Case "t"
                                objet.ConditionActuel = Traduire(_child.FirstChild.Value)
                            Case "icon"
                                objet.IconActuel = _child.FirstChild.Value
                            Case "hmid"
                                If IsNumeric(_child.FirstChild.Value) Then objet.HumiditeActuel = _child.FirstChild.Value
                            Case "s"
                                objet.VentActuel = _child.FirstChild.Value
                        End Select
                        If _child.HasChildNodes = True Then
                            For Each _child2 As XmlNode In _child
                                ' If _child2.Name.StartsWith("#text") = False Then Console.WriteLine(_child2.Name & ":" & _child2.InnerText)
                                Select Case _child2.Name
                                    Case "tmp"
                                        If IsNumeric(_child.FirstChild.Value) Then objet.TemperatureActuel = _child2.InnerText
                                    Case "t"
                                        'objet.ConditionActuel = Traduire(_child2.InnerText)
                                    Case "icon"
                                        'objet.IconActuel = _child2.InnerText
                                    Case "hmid"
                                        If IsNumeric(_child.FirstChild.Value) Then objet.HumiditeActuel = _child2.InnerText
                                    Case "s"
                                        objet.VentActuel = _child2.InnerText
                                End Select
                            Next
                        End If
                    Next

                End If
            Next

            nodes = doc.SelectNodes("/weather/dayf/day")

            Dim idx As Integer = -1

            For Each node As XmlNode In nodes
                idx = idx + 1

                Select Case idx
                    Case 0
                        objet.JourToday = TraduireJour(Mid(Now.DayOfWeek.ToString, 1, 3))
                    Case 1
                        objet.JourJ1 = TraduireJour(Mid(Now.AddDays(1).DayOfWeek.ToString, 1, 3))
                    Case 2
                        objet.JourJ2 = TraduireJour(Mid(Now.AddDays(2).DayOfWeek.ToString, 1, 3))
                    Case 3
                        objet.JourJ3 = TraduireJour(Mid(Now.AddDays(3).DayOfWeek.ToString, 1, 3))
                End Select

                If node.HasChildNodes = True Then
                    For Each _child As XmlNode In node

                        Select Case _child.Name
                            Case "hi"
                                Select Case idx
                                    Case 0
                                        If IsNumeric(_child.FirstChild.Value) Then objet.MaxToday = _child.FirstChild.Value
                                    Case 1
                                        If IsNumeric(_child.FirstChild.Value) Then objet.MaxJ1 = _child.FirstChild.Value
                                    Case 2
                                        If IsNumeric(_child.FirstChild.Value) Then objet.MaxJ2 = _child.FirstChild.Value
                                    Case 3
                                        If IsNumeric(_child.FirstChild.Value) Then objet.MaxJ3 = _child.FirstChild.Value
                                End Select
                            Case "low"
                                Select Case idx
                                    Case 0
                                        If IsNumeric(_child.FirstChild.Value) Then objet.MinToday = _child.FirstChild.Value
                                    Case 1
                                        If IsNumeric(_child.FirstChild.Value) Then objet.MinJ1 = _child.FirstChild.Value
                                    Case 2
                                        If IsNumeric(_child.FirstChild.Value) Then objet.MinJ2 = _child.FirstChild.Value
                                    Case 3
                                        If IsNumeric(_child.FirstChild.Value) Then objet.MinJ3 = _child.FirstChild.Value
                                End Select
                            Case "icon"
                            Case "t"
                                Select Case idx
                                    Case 0
                                        objet.ConditionToday = Traduire(_child.FirstChild.Value)
                                    Case 1
                                        objet.ConditionJ1 = Traduire(_child.FirstChild.Value)
                                    Case 2
                                        objet.ConditionJ2 = Traduire(_child.FirstChild.Value)
                                    Case 3
                                        objet.ConditionJ3 = Traduire(_child.FirstChild.Value)
                                End Select
                        End Select
                        If _child.HasChildNodes = True Then
                            For Each _child2 As XmlNode In _child
                                Select Case _child2.Name
                                    Case "hi"
                                        Select Case idx
                                            Case 0
                                                If IsNumeric(_child2.InnerText) Then objet.MaxToday = _child2.InnerText
                                            Case 1
                                                If IsNumeric(_child2.InnerText) Then objet.MaxJ1 = _child2.InnerText
                                            Case 2
                                                If IsNumeric(_child2.InnerText) Then objet.MaxJ2 = _child2.InnerText
                                            Case 3
                                                If IsNumeric(_child2.InnerText) Then objet.MaxJ3 = _child2.InnerText
                                        End Select
                                    Case "low"
                                        Select Case idx
                                            Case 0
                                                If IsNumeric(_child2.InnerText) Then objet.MinToday = _child2.InnerText
                                            Case 1
                                                If IsNumeric(_child2.InnerText) Then objet.MinJ1 = _child2.InnerText
                                            Case 2
                                                If IsNumeric(_child2.InnerText) Then objet.MinJ2 = _child2.InnerText
                                            Case 3
                                                If IsNumeric(_child2.InnerText) Then objet.MinJ3 = _child2.InnerText
                                        End Select
                                    Case "icon"
                                        Select Case idx
                                            Case 0
                                                objet.IconToday = _child2.InnerText
                                            Case 1
                                                objet.IconJ1 = _child2.InnerText
                                            Case 2
                                                objet.IconJ2 = _child2.InnerText
                                            Case 3
                                                objet.IconJ3 = _child2.InnerText
                                        End Select
                                    Case "t"
                                        Select Case idx
                                            Case 0
                                                objet.ConditionToday = Traduire(_child2.InnerText)
                                            Case 1
                                                objet.ConditionJ1 = Traduire(_child2.InnerText)
                                            Case 2
                                                objet.ConditionJ2 = Traduire(_child2.InnerText)
                                            Case 3
                                                objet.ConditionJ3 = Traduire(_child2.InnerText)
                                        End Select
                                End Select
                                'If _child2.Name.StartsWith("#text") = False Then Console.WriteLine(_child2.Name & ":" & _child2.InnerText)
                            Next
                        End If
                    Next

                End If
            Next

            doc = Nothing
            nodes = Nothing
            response = Nothing
            Request = Nothing
            url = Nothing
            objet.LastChange = Now
            objet = Nothing

            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GOOGLEMETEO", "MAJ Meteo effectuée pour " & objet.name)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "METEOWeather", "Erreur Lors de la MaJ de " & objet.name & " : " & ex.Message)
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
            Dim _retour As String = txt
            Dim _txt = LCase(Trim(txt))
            _txt = _txt.Replace("  ", " ")

            If _txt = "a few clouds" Then
                _retour = "Quelques nuages"
            End If
            If _txt = "a few clouds and breezy" Then
                _retour = "Quelques nuages et Frais"
            End If
            If _txt = "a few clouds and windy" Then
                _retour = "Quelques nuages Grand vent"
            End If
            If _txt = "a few clouds with haze" Then
                _retour = "Quelques nuages et brume"
            End If
            If _txt = "am clouds / pm sun" Then
                _retour = "Matin nuages / Après-Midi Soleil"
            End If
            If _txt = "am fog / pm sun" Then
                _retour = "Matin Brouillard / Après-Midi Soleil"
            End If
            If _txt = "am rain / snow showers" Then
                _retour = "Matin pluie / averses neigeuses"
            End If
            If _txt = "am showers" Then
                _retour = "Averses matinales"
            End If
            If _txt = "am snow showers" Then
                _retour = "Averses de neige le matin"
            End If
            If _txt = "am t-storms" Then
                _retour = "Orages le matin"
            End If
            If _txt = "blowing dust" Then
                _retour = "Vent de poussière"
            End If
            If _txt = "blowing sand" Then
                _retour = "Vent de sable"
            End If
            If _txt = "blowing snow" Then
                _retour = "Vent de neige"
            End If
            If _txt = "blowing snow in vicinity" Then
                _retour = "Vent de neige dans les environs"
            End If
            If _txt = "clear" Then
                _retour = "Clair"
            End If
            If _txt = "clear / wind" Then
                _retour = "Clair / vent"
            End If
            If _txt = "clear and breezy" Then
                _retour = "Clair et frais"
            End If
            If _txt = "clear with haze" Then
                _retour = "Clair avec brume légère"
            End If
            If _txt = "cloudy clair" Then
                _retour = "Nuageux clair"
            End If
            If _txt = "clouds early / clearing kate" Then
                _retour = "Nuages matinaux suivis d'éclaircies"
            End If
            If _txt = "cloudy" Then
                _retour = "Nuageux"
            End If
            If _txt = "cloudy / wind" Then
                _retour = "Nuageux / Vent"
            End If
            If _txt = "cloudy / windy" Then
                _retour = "Nuageux et venteux"
            End If
            If _txt = "cloudy and windy" Then
                _retour = "Nuageux et venteux"
            End If
            If _txt = "drifting snow" Then
                _retour = "Amoncellement de neige"
            End If
            If _txt = "drizzle" Then
                _retour = "crachin"
            End If
            If _txt = "drizzle fog/mist" Then
                _retour = "Crachin brouillard/brume"
            End If
            If _txt = "drizzle fog" Then
                _retour = "Crachin brouillard"
            End If
            If _txt = "drizzle ice pellets" Then
                _retour = "Crachin grêlons"
            End If
            If _txt = "drizzle snow" Then
                _retour = "Crachin neige"
            End If
            If _txt = "dust" Then
                _retour = "Poussière"
            End If
            If _txt = "dust storm in vicinity" Then
                _retour = "Tempête de poussière dans les environs"
            End If
            If _txt = "dust storm" Then
                _retour = "Tempête de poussière"
            End If
            If _txt = "dust/sand whirls" Then
                _retour = "Tourbillons de poussière/sable"
            End If
            If _txt = "dust/sand whirls in vicinity" Then
                _retour = "Tourbillons de poussière/sable dans les environs"
            End If
            If _txt = "fair" Then
                _retour = "Ciel dégagé"
            End If
            If _txt = "fair/ windy" Then
                _retour = "Ciel dégagé / grand vent"
            End If
            If _txt = "fair and breezy" Then
                _retour = "Ciel dégagé et frais"
            End If
            If _txt = "fair and windy" Then
                _retour = "Ciel dégagé et grand vent"
            End If
            If _txt = "fair with haze" Then
                _retour = "Ciel dégagé avec brume légère"
            End If
            If _txt = "few showers" Then
                _retour = "Quelques averses"
            End If
            If _txt = "few snow showers" Then
                _retour = "Quelques averses de neige"
            End If
            If _txt = "few snow showers / wind" Then
                _retour = "Quelques averses de neige / Vent"
            End If
            If _txt = "fog" Then
                _retour = "Brouillard"
            End If
            If _txt = "fog in vicinity" Then
                _retour = "Brouillard dans les environs"
            End If
            If _txt = "fog/mist" Then
                _retour = "Brouillard/brume"
            End If
            If _txt = "freezing drizzle" Then
                _retour = "Crachin givrant"
            End If
            If _txt = "freezing drizzle in vicinity" Then
                _retour = "Crachin givrant dans les environs"
            End If
            If _txt = "freezing drizzle rain" Then
                _retour = "Crachin pluie givrant "
            End If
            If _txt = "freezing drizzle snow" Then
                _retour = "Crachin neige givrant"
            End If
            If _txt = "freezing fog" Then
                _retour = "Brouillard givrant"
            End If
            If _txt = "freezing fog in vicinity" Then
                _retour = "Brouillard givrant dans les environs"
            End If
            If _txt = "freezing rain" Then
                _retour = "Pluie givrante"
            End If
            If _txt = "freezing rain in vicinity" Then
                _retour = "Pluie givrante dans les environs"
            End If
            If _txt = "freezing rain snow" Then
                _retour = "Pluie neige givrante dans les environs"
            End If
            If _txt = "frigid" Then
                _retour = "Grand froid"
            End If
            If _txt = "funnel cloud in vicinity" Then
                _retour = "Nuage en entonnoir dans les environs"
            End If
            If _txt = "funnel cloud" Then
                _retour = "Nuage en entonnoir"
            End If
            If _txt = "hail" Then
                _retour = "Grêle"
            End If
            If _txt = "hail showers" Then
                _retour = "Averses de grêle"
            End If
            If _txt = "haze" Then
                _retour = "Brume légère"
            End If
            If _txt = "heavy drizzle" Then
                _retour = "Gros Crachin "
            End If
            If _txt = "heavy drizzle gog/mist" Then
                _retour = "Gros Crachin Brouillard/brume"
            End If
            If _txt = "heavy drizzle fog" Then
                _retour = "Gros Crachin Brouillard"
            End If
            If _txt = "heavy drizzle ice pellets" Then
                _retour = "Gros Crachin Grêlons"
            End If
            If _txt = "heavy drizzle snow" Then
                _retour = "Gros Crachin Neige"
            End If
            If _txt = "heavy Dust storm" Then
                _retour = "Grosse tempête de poussière"
            End If
            If _txt = "heavy freezing drizzle rain" Then
                _retour = "Gros Crachin verglassant Pluie"
            End If
            If _txt = "heavy freezing drizzle snow" Then
                _retour = "Gros Crachin verglassant Neige"
            End If
            If _txt = "heavy freezing drizzle" Then
                _retour = "Gros Crachin verglassant"
            End If
            If _txt = "heavy freezing fog" Then
                _retour = "Gros brouillard verglassant"
            End If
            If _txt = "heavy freezing rain snow" Then
                _retour = "Grosse pluie verglassante Neige"
            End If
            If _txt = "heavy freezing rain" Then
                _retour = "Grosse pluie verglassante"
            End If
            If _txt = "heavy ice pellets" Then
                _retour = "Gros Grêlons"
            End If
            If _txt = "heavy ice pellets drizzle" Then
                _retour = "Gros Grêlons Crachin"
            End If
            If _txt = "heavy ice pellets rain" Then
                _retour = "Gros Grêlons Pluie"
            End If
            If _txt = "heavy rain" Then
                _retour = "Grosse pluie"
            End If
            If _txt = "heavy rain fog/mist" Then
                _retour = "Grosse pluie Brouillard/brume"
            End If
            If _txt = "heavy rain fog" Then
                _retour = "Grosse pluie Brouillard"
            End If
            If _txt = "heavy rain freezing drizzle" Then
                _retour = "Grosse pluie Crachin verglassant"
            End If
            If _txt = "heavy rain freezing rain" Then
                _retour = "Grosse pluie Pluie verglassante"
            End If
            If _txt = "heavy rain ice pellets" Then
                _retour = "Grosse pluie Grêlons"
            End If
            If _txt = "heavy rain icy" Then
                _retour = "Grosse pluie verglassante"
            End If
            If _txt = "heavy rain showers fog/mist" Then
                _retour = "Grosse pluie Averses Brouillard/brume"
            End If
            If _txt = "heavy rain showers" Then
                _retour = "Grosse pluie Averses"
            End If
            If _txt = "heavy rain snow" Then
                _retour = "Grosse pluie Neige"
            End If
            If _txt = "heavy Sand storm" Then
                _retour = "Grosse tempête de sable "
            End If
            If _txt = "heavy showers rain fog/mist" Then
                _retour = "Grosses averses Pluie Brouillard/brume"
            End If
            If _txt = "heavy showers rain" Then
                _retour = "Grosses averses Pluie"
            End If
            If _txt = "heavy showers snow fog/mist" Then
                _retour = "Grosses Neige Pluie Brouillard/brume"
            End If
            If _txt = "heavy showers snow fog" Then
                _retour = "Grosses averses Neige Brouillard"
            End If
            If _txt = "heavy showers snow" Then
                _retour = "Grosses averses Neige"
            End If
            If _txt = "heavy snow" Then
                _retour = "Beaucoup de neige"
            End If
            If _txt = "heavy snow drizzle" Then
                _retour = "Beaucoup de neige Crachin"
            End If
            If _txt = "heavy snow fog/mist" Then
                _retour = "Beaucoup de neige Brouillard/brume"
            End If
            If _txt = "heavy snow fog" Then
                _retour = "Beaucoup de neige Brouillard"
            End If
            If _txt = "heavy snow freezing drizzle" Then
                _retour = "Beaucoup de neige Crachin verglassant"
            End If
            If _txt = "heavy snow freezing rain" Then
                _retour = "Beaucoup de neige Pluie verglassante"
            End If
            If _txt = "heavy snow rain" Then
                _retour = "Beaucoup de neige Pluie"
            End If
            If _txt = "heavy snow showers fog/mist" Then
                _retour = "Beaucoup de neige Averses Brouillard/brume"
            End If
            If _txt = "heavy snow showers fog" Then
                _retour = "Beaucoup de neige Averses Brouillard"
            End If
            If _txt = "heavy snow showers" Then
                _retour = "Beaucoup de neige Averses"
            End If
            If _txt = "heavy t-storm" Then
                _retour = "Gros orage"
            End If
            If _txt = "heavy t-storms rain fog/mist" Then
                _retour = "Gros orage Brouillard/brume"
            End If
            If _txt = "heavy t-storms rain fog" Then
                _retour = "Gros orage Brouillard"
            End If
            If _txt = "heavy t-storms rain hail fog/mist" Then
                _retour = "Gros orage Grêle Brouillard/brume"
            End If
            If _txt = "heavy t-storms rain hail fog" Then
                _retour = "Gros orage Grêle  Brouillard"
            End If
            If _txt = "heavy t-storms rain hail haze" Then
                _retour = "Gros orage Grêle Brume légère"
            End If
            If _txt = "heavy t-storms rain hail" Then
                _retour = "Gros orage Grêle"
            End If
            If _txt = "heavy t-storms rain haze" Then
                _retour = "Gros orage Brume légère"
            End If
            If _txt = "heavy t-storms rain" Then
                _retour = "Gros orage Pluie"
            End If
            If _txt = "heavy t-storms snow" Then
                _retour = "Gros orage Neige"
            End If
            If _txt = "ice crystals" Then
                _retour = "Cristaux de glace"
            End If
            If _txt = "ice pellets drizzle" Then
                _retour = "Grêlons Crachin"
            End If
            If _txt = "ice pellets in vicinity" Then
                _retour = "Grêlons dans les environs "
            End If
            If _txt = "ice pellets rain" Then
                _retour = "Grêlons Pluie"
            End If
            If _txt = "ice pellets" Then
                _retour = "Grêlons"
            End If
            If _txt = "Isokated t-storms" Then
                _retour = "Orages isolés"
            End If
            If _txt = "light drizzle" Then
                _retour = "Léger crachin"
            End If
            If _txt = "light drizzle fog/mist" Then
                _retour = "Léger crachin Brouillard/brume"
            End If
            If _txt = "light drizzle fog" Then
                _retour = "Léger crachin Brouillard"
            End If
            If _txt = "light drizzle ice pellets" Then
                _retour = "Léger crachin Grêlons"
            End If
            If _txt = "light drizzle snow" Then
                _retour = "Léger crachin Neige"
            End If
            If _txt = "light freezing drizzle" Then
                _retour = "Léger crachin verglassant"
            End If
            If _txt = "light freezing drizzle rain" Then
                _retour = "Léger crachin verglassant Pluie"
            End If
            If _txt = "light freezing drizzle snow" Then
                _retour = "Léger crachin verglassant Neige"
            End If
            If _txt = "light freezing fog" Then
                _retour = "Léger brouillard verglassant"
            End If
            If _txt = "light freezing rain" Then
                _retour = "Légère pluie verglassante"
            End If
            If _txt = "light freezing rain snow" Then
                _retour = "Légère pluie verglassante Neige"
            End If
            If _txt = "light ice pellets drizzle" Then
                _retour = "Petit Grêlons Crachin"
            End If
            If _txt = "light ice pellets rain" Then
                _retour = "Petit Grêlons Pluie"
            End If
            If _txt = "light ice pellets" Then
                _retour = "Petit Grêlons"
            End If
            If _txt = "light rain" Then
                _retour = "Légère Pluie"
            End If
            If _txt = "light rain Early" Then
                _retour = "Légère Pluie matinal"
            End If
            If _txt = "light rain fog/mist" Then
                _retour = "Légère Pluie Brouillard/brume"
            End If
            If _txt = "light rain fog" Then
                _retour = "Légère Pluie Brouillard"
            End If
            If _txt = "light rain freezing drizzle" Then
                _retour = "Légère Pluie Crachin verglassant"
            End If
            If _txt = "light rain freezing rain" Then
                _retour = "Légère Pluie Pluie verglassante"
            End If
            If _txt = "light rain ice pellets" Then
                _retour = "Légère Pluie Grêlons"
            End If
            If _txt = "light rain icy" Then
                _retour = "Légère pluie verglassante"
            End If
            If _txt = "light rain kate" Then
                _retour = "Légère Pluie tardive"
            End If
            If _txt = "light rain shower" Then
                _retour = "Légère pluie Averse"
            End If
            If _txt = "light rain shower and windy" Then
                _retour = "Légère pluie Averses et Vent"
            End If
            If _txt = "light rain showers" Then
                _retour = "Légères pluie Averses"
            End If
            If _txt = "light rain snow" Then
                _retour = "Légères pluie Neige"
            End If
            If _txt = "light rain with Thunder" Then
                _retour = "Légère Pluie avec tonnerre"
            End If
            If _txt = "light showers rain" Then
                _retour = "Légère averses Pluie"
            End If
            If _txt = "light showers rain fog/mist" Then
                _retour = "Légère averses Pluie Brouillard/brume"
            End If
            If _txt = "light showers snow" Then
                _retour = "Légère averses Neige"
            End If
            If _txt = "light showers snow fog" Then
                _retour = "Légère averses Neige Brouillard"
            End If
            If _txt = "light showers snow fog/mist" Then
                _retour = "Légère averses Neige Brouillard/brume"
            End If
            If _txt = "light snow" Then
                _retour = "Peu de neige"
            End If
            If _txt = "light snow drizzle" Then
                _retour = "Peu de neige Crachin"
            End If
            If _txt = "light snow Fall" Then
                _retour = "Légère chutes de neige"
            End If
            If _txt = "light snow fog/mist" Then
                _retour = "Peu de neige Brouillard/brume"
            End If
            If _txt = "light snow fog" Then
                _retour = "Peu de neige Brouillard"
            End If
            If _txt = "light snow freezing drizzle" Then
                _retour = "Peu de neige Crachin verglassant"
            End If
            If _txt = "light snow freezing rain" Then
                _retour = "Peu de neige Pluie Verglassante"
            End If
            If _txt = "light snow grains" Then
                _retour = "Quelques flocons de neige"
            End If
            If _txt = "light snow rain" Then
                _retour = "Peu de neige Pluie"
            End If
            If _txt = "light snow shower" Then
                _retour = "Légère averse de neige"
            End If
            If _txt = "light snow showers fog/mist" Then
                _retour = "Légère averses de neige Brouillard/brume"
            End If
            If _txt = "light snow showers fog" Then
                _retour = "Légère averses de neige Brouillard"
            End If
            If _txt = "light t-storms rain fog/mist" Then
                _retour = "Léger orages Pluie Brouillard/brume"
            End If
            If _txt = "light t-storms rain fog" Then
                _retour = "Léger orages Pluie Brouillard"
            End If
            If _txt = "light t-storms rain hail fog/mist" Then
                _retour = "Léger orages Pluie Grêle Brouillard/brume"
            End If
            If _txt = "light t-storms rain hail fog" Then
                _retour = "Léger orages Pluie Grêle Brouillard"
            End If
            If _txt = "light t-storms rain hail haze" Then
                _retour = "Léger orages Pluie Grêle Brume légère"
            End If
            If _txt = "light t-storms rain hail" Then
                _retour = "Léger orages Pluie Grêle"
            End If
            If _txt = "light t-storms rain haze" Then
                _retour = "Léger orages Pluie Brume légère"
            End If
            If _txt = "light t-storms rain" Then
                _retour = "Léger orages Pluie"
            End If
            If _txt = "light t-storms snow" Then
                _retour = "Léger orages Neige"
            End If
            If _txt = "lightening" Then
                _retour = "Eclairs"
            End If
            If _txt = "lightenings" Then
                _retour = "Eclairs"
            End If
            If _txt = "mostly clear" Then
                _retour = "Ciel plutôt dégagé"
            End If
            If _txt = "mostly cloudy" Then
                _retour = "Plutôt nuageux"
            End If
            If _txt = "mostly cloudy and breezy" Then
                _retour = "Plutôt nuageux et Venteux"
            End If
            If _txt = "mostly cloudy and windy" Or txt = "mostly cloudy/wind" Then
                _retour = "Plutôt nuageux et Grand vent"
            End If
            If _txt = "mostly cloudy with haze" Then
                _retour = "Plutôt nuageux avec Légère Brume"
            End If
            If _txt = "mostly sunny" Then
                _retour = "Plutôt ensoleillé"
            End If
            If _txt = "mostly sunny / wind" Then
                _retour = "Plutôt ensoleillé / vent"
            End If
            If _txt = "overcast" Then
                _retour = "Couvert"
            End If
            If _txt = "overcast and Breezy" Then
                _retour = "Couvert et Venteux"
            End If
            If _txt = "overcast and windy" Then
                _retour = "Couvert et Grand vent"
            End If
            If _txt = "overcast with haze" Then
                _retour = "Couvert avec légère brume"
            End If
            If _txt = "partial fog" Then
                _retour = "Banc de Brouillard"
            End If
            If _txt = "partial fog in vicinity" Then
                _retour = "Banc de Brouillard dans les environs"
            End If
            If _txt = "p cloudy" Then
                _retour = "Partiellement nuageux"
            End If
            If _txt = "partly cloudy" Then
                _retour = "Partiellement nuageux"
            End If
            If _txt = "partly cloudy and breezy" Then
                _retour = "Partiellement nuageux et Venteux"
            End If
            If _txt = "partly cloudy and windy" Then
                _retour = "Partiellement nuageux et Grand vent"
            End If
            If _txt = "partly cloudy / wind" Then
                _retour = "Partiellement nuageux / Vent"
            End If
            If _txt = "partly cloudy/ windy" Or txt = "partly cloudy/wind" Then
                _retour = "Partiellement nuageux / Venteux"
            End If
            If _txt = "partly cloudy with haze" Then
                _retour = "Partiellement nuageux avec légère brume"
            End If
            If _txt = "partly sunny" Then
                _retour = "Partiellement ensoleillé"
            End If
            If _txt = "patches of fog" Then
                _retour = "Nappes de Brouillard"
            End If
            If _txt = "patches of fog in vicinity" Then
                _retour = "Nappes de Brouillard dans les environs"
            End If
            If _txt = "pm light rain" Then
                _retour = "PM Légère Pluie"
            End If
            If _txt = "pm rain / snow" Then
                _retour = "PM Pluie / Neige"
            End If
            If _txt = "pm rain / wind" Then
                _retour = "PM Pluie / Vent"
            End If
            If _txt = "pm showers" Then
                _retour = "PM Averses"
            End If
            If _txt = "pm snow showers" Then
                _retour = "PM averses neigeuses"
            End If
            If _txt = "pm t-storms" Then
                _retour = "Orages l après-midi"
            End If
            If _txt = "rain" Then
                _retour = "Pluie"
            End If
            If _txt = "rain / snow" Then
                _retour = "Pluie / neige"
            End If
            If _txt = "rain / snow showers" Then
                _retour = "Pluie / averses neigeuses"
            End If
            If _txt = "rain / snow showers early" Then
                _retour = "Pluie / averses neigeuses matinales"
            End If
            If _txt = "rain / thunder" Then
                _retour = "Pluie / Tonnerre"
            End If
            If _txt = "rain / wind" Then
                _retour = "Pluie / Vent"
            End If
            If _txt = "rain and snow" Then
                _retour = "Pluie et Neige"
            End If
            If _txt = "rain early" Then
                _retour = "Pluie matinale"
            End If
            If _txt = "rain fog/mist" Then
                _retour = "Pluie Brouillard/brume"
            End If
            If _txt = "rain fog" Then
                _retour = "Pluie Brouillard"
            End If
            If _txt = "rain freezing drizzle" Then
                _retour = "Pluie Crachin verglassant"
            End If
            If _txt = "rain freezing rain" Then
                _retour = "Pluie Pluie Verglassante"
            End If
            If _txt = "rain ice pellets" Then
                _retour = "Pluie Grêlons"
            End If
            If _txt = "rain shower" Then
                _retour = "Pluie Averses"
            End If
            If _txt = "rain showers fog/mist" Then
                _retour = "Pluie Averses Brouillard/brume"
            End If
            If _txt = "rain showers in vicinity fog/mist" Then
                _retour = "Pluie Averses dans les environs Brouillard/brume"
            End If
            If _txt = "rain showers in vicinity" Then
                _retour = "Pluie Averses dans les environs"
            End If
            If _txt = "rain snow" Then
                _retour = "Pluie Neige"
            End If
            If _txt = "rain to snow" Then
                _retour = "Pluie vers Neige"
            End If
            If _txt = "sand storm" Then
                _retour = "Tempête de Sable"
            End If
            If _txt = "sand storm in vicinity" Then
                _retour = "Tempête de Sable dans les environs"
            End If
            If _txt = "sand" Then
                _retour = "Sable"
            End If
            If _txt = "shallow fog" Then
                _retour = "Brouillard superficiel"
            End If
            If _txt = "shallow fog in vicinity" Then
                _retour = "Brouillard superficiel dans les environs"
            End If
            If _txt = "scattered showers" Then
                _retour = "Averses éparses"
            End If
            If _txt = "scattered showers / wind" Then
                _retour = "Averses éparses / Vent"
            End If
            If _txt = "scattered snow showers" Then
                _retour = "Averses neigeuses éparses"
            End If
            If _txt = "scattered snow showers / wind" Then
                _retour = "Averses neigeuses éparses / Vent"
            End If
            If _txt = "scattered strong storms" Then
                _retour = "Violents orages locals"
            End If
            If _txt = "scattered t-storms" Then
                _retour = "Orages éparses"
            End If
            If _txt = "showers" Then
                _retour = "Averses"
            End If
            If _txt = "showers / wind" Then
                _retour = "Averses / Vent"
            End If
            If _txt = "showers hail" Then
                _retour = "Averses Grêle"
            End If
            If _txt = "showers ice pellets" Then
                _retour = "Averses Grêlons"
            End If
            If _txt = "showers in the vicinity" Then
                _retour = "Averses dans les environs"
            End If
            If _txt = "showers in vicinity fog/mist" Then
                _retour = "Averses dans les environs Brouillard/brume"
            End If
            If _txt = "showers in vicinity fog" Then
                _retour = "Averses dans les environs Brouillard"
            End If
            If _txt = "showers in vicinity haze" Then
                _retour = "Averses dans les environs Brume légère"
            End If
            If _txt = "showers in vicinity snow" Then
                _retour = "Averses dans les environs Neige"
            End If
            If _txt = "showers Early" Then
                _retour = "Averses matinales"
            End If
            If _txt = "showers kate" Then
                _retour = "Averses tardives"
            End If
            If _txt = "showers rain" Then
                _retour = "Averses Pluie"
            End If
            If _txt = "showers rain fog/mist" Then
                _retour = "Averses Pluie Brouillard/brume"
            End If
            If _txt = "showers rain in vicinity" Then
                _retour = "Averses Pluie dans les environs"
            End If
            If _txt = "showers rain in vicinity fog/mist" Then
                _retour = "Averses Pluie dans les environs Brouillard/brume"
            End If
            If _txt = "showers snow" Then
                _retour = "Averses Neige"
            End If
            If _txt = "showers snow fog" Then
                _retour = "Averses Neige Brouillard"
            End If
            If _txt = "showers snow fog/mist" Then
                _retour = "Averses Neige Brouillard/brume"
            End If
            If _txt = "smoke" Then
                _retour = "fumée"
            End If
            If _txt = "snow" Then
                _retour = "Neige"
            End If
            If _txt = "snow / rain icy mix" Then
                _retour = "Mélange Neige / Pluie Verglassante"
            End If
            If _txt = "snow and fog" Then
                _retour = "Neige et Brouillard"
            End If
            If _txt = "snow drizzle" Then
                _retour = "Neige Crachin"
            End If
            If _txt = "snow fog/mist" Then
                _retour = "Neige Brouillard/brume"
            End If
            If _txt = "snow freezing drizzle" Then
                _retour = "Neige Crachin verglassant"
            End If
            If _txt = "snow freezing rain" Then
                _retour = "Neige Pluie verglassante"
            End If
            If _txt = "snow rain" Then
                _retour = "Neige Pluie"
            End If
            If _txt = "snow shower" Then
                _retour = "Averses de neige"
            End If
            If _txt = "snow shower / wind" Then
                _retour = "Averses de neige / vent"
            End If
            If _txt = "snow shower early" Then
                _retour = "Averses de neige matinales"
            End If
            If _txt = "snow showers fog/mist" Then
                _retour = "Averses de neige Brouillard/brume"
            End If
            If _txt = "snow showers fog" Then
                _retour = "Averses de neige Brouillard"
            End If
            If _txt = "snow showers in vicinity" Then
                _retour = "Averses de neige dans les environs"
            End If
            If _txt = "snow showers in vicinity fog" Then
                _retour = "Averses de neige dans les environs Brouillard"
            End If
            If _txt = "snow showers in vicinity fog/mist" Then
                _retour = "Averses de neige dans les environs Brouillard/brume"
            End If
            If _txt = "snow showers kate" Then
                _retour = "Averses de neige tardives"
            End If
            If _txt = "snow to rain" Then
                _retour = "Neige vers Pluie"
            End If
            If _txt = "snowflakes" Then
                _retour = "Flocons de neige"
            End If
            If _txt = "sunny" Then
                _retour = "Ensoleillé"
            End If
            If _txt = "sunny / wind" Then
                _retour = "Ensoleillé / Vent"
            End If
            If _txt = "sunny day" Then
                _retour = "Journée ensoleillé"
            End If
            If _txt = "thunder" Then
                _retour = "Tonnerre"
            End If
            If _txt = "thunder in the vicinity" Then
                _retour = "Tonnerre dans les environs"
            End If
            If _txt = "t-storms" Then
                _retour = "Orages"
            End If
            If _txt = "t-storms early" Then
                _retour = "Orages matinaux"
            End If
            If _txt = "t-storms fog" Then
                _retour = "Orages Brouillard"
            End If
            If _txt = "t-storms hail fog" Then
                _retour = "Orages Grêle Brouillard"
            End If
            If _txt = "t-storms hail" Then
                _retour = "Orages Grêle"
            End If
            If _txt = "t-storms haze in vicinity hail" Then
                _retour = "Orages brume dans les environs Grêle"
            End If
            If _txt = "t-storms haze in vicinity" Then
                _retour = "Orages brume dans les environs"
            End If
            If _txt = "t-storms heavy rain" Then
                _retour = "Orages Grosse Pluie"
            End If
            If _txt = "t-storms heavy rain fog" Then
                _retour = "Orages Grosse Pluie Brouillard"
            End If
            If _txt = "t-storms heavy rain fog/mist" Then
                _retour = "Orages Grosse pluie Brouillard/brume"
            End If
            If _txt = "t-storms heavy rain hail fog" Then
                _retour = "Orages Grosse Pluie Grêle Brouillard"
            End If
            If _txt = "t-storms heavy rain hail fog/mist" Then
                _retour = "Orages Grosse Pluie Grêle Brouillard/brume"
            End If
            If _txt = "t-storms heavy rain hail haze" Then
                _retour = "Orages Grosse Pluie Grêle Brume légère"
            End If
            If _txt = "t-storms heavy rain hail" Then
                _retour = "Orages Grosse Pluie Grêle"
            End If
            If _txt = "t-storms heavy rain haze" Then
                _retour = "Orages Grosse Pluie Brume légère"
            End If
            If _txt = "t-storms ice pellets" Then
                _retour = "Orages Grêlons"
            End If
            If _txt = "t-storms in vicinity" Then
                _retour = "Orages dans les environs"
            End If
            If _txt = "t-storms in vicinity fog" Then
                _retour = "Orages dans les environs Brouillard"
            End If
            If _txt = "t-storms in vicinity fog/mist" Then
                _retour = "Orages dans les environs Brouillard/brume"
            End If
            If _txt = "t-storms in vicinity hail fog/mist" Then
                _retour = "Orages dans les environs Grêle Brouillard/brume"
            End If
            If _txt = "t-storms in vicinity hail haze" Then
                _retour = "Orages dans les environs Grêle Brume légère"
            End If
            If _txt = "t-storms in vicinity hail" Then
                _retour = "Orages dans les environs Grêle"
            End If
            If _txt = "t-storms in vicinity haze" Then
                _retour = "Orages dans les environs Brume légère"
            End If
            If _txt = "t-storms light rain" Then
                _retour = "Orages Légère pluie"
            End If
            If _txt = "t-storms light rain fog" Then
                _retour = "Orages Légère pluie Brouillard"
            End If
            If _txt = "t-storms light rain fog/mist" Then
                _retour = "Orages Légère pluie Brouillard/brume"
            End If
            If _txt = "t-storms light rain hail" Then
                _retour = "Orages Légère pluie Grêle"
            End If
            If _txt = "t-storms light rain hail fog" Then
                _retour = "Orages Légère pluie Grêle Brouillard"
            End If
            If _txt = "t-storms light rain hail fog/mist" Then
                _retour = "Orages Légère pluie Grêle Brouillard/brume"
            End If
            If _txt = "t-storms light rain hail haze" Then
                _retour = "Orages Légère pluie Grêle Brume légère"
            End If
            If _txt = "t-storms light rain haze" Then
                _retour = "Orages Légère pluie Brume légère"
            End If
            If _txt = "t-storms rain fog/mist" Then
                _retour = "Orages Pluie Brouillard/brume"
            End If
            If _txt = "t-storms rain hail fog/mist" Then
                _retour = "Orages Pluie Grêle Brouillard/brume"
            End If
            If _txt = "t-storms showers in vicinity" Then
                _retour = "Orages Averses dans les environs"
            End If
            If _txt = "t-storms showers in vicinity hail" Then
                _retour = "Orages Averses dans les environs Grêle"
            End If
            If _txt = "t-storms snow" Then
                _retour = "Orages Neige"
            End If
            If _txt = "windy" Then
                _retour = "Grand vent"
            End If
            If _txt = "windy / snowy" Then
                _retour = "Grand vent / neigeux"
            End If
            If _txt = "windy rain" Then
                _retour = "giboulées"
            End If
            If _txt = "wintry mix" Then
                _retour = "Mélanges pluie/neige"
            End If
            If _txt = "wintry mix / wind" Then
                _retour = "Mélanges pluie/neige / vent"
            End If

            Return _retour

            _retour = Nothing
            _txt = Nothing
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "METEOWeather", "Traduire: " & ex.Message)
            Return txt
        End Try
    End Function

#End Region

End Class
