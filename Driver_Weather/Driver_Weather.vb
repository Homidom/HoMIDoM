﻿Imports HoMIDom
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
    Dim _OsPlatform As String = "3264" 'Plateforme compatible 32 64 ou 3264
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
            ' Request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; fr; rv:1.8.0.7) Gecko/20060909 Firefox/1.5.0.7"
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
                                'Select Case idx
                                '    Case 0
                                '        objet.IconToday = _child.FirstChild.Value
                                '    Case 1
                                '        objet.IconJ1 = _child.FirstChild.Value
                                '    Case 2
                                '        objet.IconJ2 = _child.FirstChild.Value
                                '    Case 3
                                '        objet.IconJ3 = _child.FirstChild.Value
                                'End Select
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

            objet.LastChange = Now

            doc = Nothing
            nodes = Nothing

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

    Private Function Traduire(ByVal txt)

        Traduire = txt

        If txt = "A Few Clouds" Then
            Traduire = "Quelques nuages"
        End If
        If txt = "A Few Clouds and Breezy" Then
            Traduire = "Quelques nuages et Frais"
        End If
        If txt = "A Few Clouds and Windy" Then
            Traduire = "Quelques nuages et  et grand vent"
        End If
        If txt = "A Few Clouds with Haze" Then
            Traduire = "Quelques nuages et brume"
        End If
        If txt = "AM Clouds / PM Sun" Then
            Traduire = "Matin nuages / Après-Midi Soleil"
        End If
        If txt = "AM Fog / PM Sun" Then
            Traduire = "Matin Brouillard / Après-Midi Soleil"
        End If
        If txt = "AM Rain / Snow Showers" Then
            Traduire = "Matin pluie / averses neigeuses"
        End If
        If txt = "AM Showers" Then
            Traduire = "Averses matinales"
        End If
        If txt = "AM Snow Showers" Then
            Traduire = "Averses de neige le matin"
        End If
        If txt = "AM T-Storms" Then
            Traduire = "Orages le matin"
        End If
        If txt = "Blowing Dust" Then
            Traduire = "Vent de poussière"
        End If
        If txt = "Blowing Sand" Then
            Traduire = "Vent de sable"
        End If
        If txt = "Blowing Snow" Then
            Traduire = "Vent de neige"
        End If
        If txt = "Blowing Snow in Vicinity" Then
            Traduire = "Vent de neige dans les environs"
        End If
        If txt = "Clear" Then
            Traduire = "Clair"
        End If
        If txt = "Clear / Wind" Then
            Traduire = "Clair / vent"
        End If
        If txt = "Clear and Breezy" Then
            Traduire = "Clair et frais"
        End If
        If txt = "Clear with Haze" Then
            Traduire = "Clair avec brume légère"
        End If
        If txt = "Cloudy Clair" Then
            Traduire = "Nuageux clair"
        End If
        If txt = "Clouds Early / Clearing Late" Then
            Traduire = "Nuages matinaux suivis d'éclaircies"
        End If
        If txt = "Cloudy" Then
            Traduire = "Nuageux"
        End If
        If txt = "Cloudy / Wind" Then
            Traduire = "Nuageux / Vent"
        End If
        If txt = "Cloudy / Windy" Then
            Traduire = ""
        End If
        If txt = "Drifting Snow" Then
            Traduire = "Amoncellement de neige"
        End If
        If txt = "Drizzle" Then
            Traduire = "Crachin"
        End If
        If txt = "Drizzle Fog/Mist" Then
            Traduire = "Crachin brouillard/brume"
        End If
        If txt = "Drizzle Fog" Then
            Traduire = "Crachin brouillard"
        End If
        If txt = "Drizzle Ice Pellets" Then
            Traduire = "Crachin grêlons"
        End If
        If txt = "Drizzle Snow" Then
            Traduire = "Crachin neige"
        End If
        If txt = "Dust" Then
            Traduire = "Poussière"
        End If
        If txt = "Dust Storm in Vicinity" Then
            Traduire = "Tempête de poussière dans les environs"
        End If
        If txt = "Dust Storm" Then
            Traduire = "Tempête de poussière"
        End If
        If txt = "Dust/Sand Whirls" Then
            Traduire = "Tourbillons de poussière/sable"
        End If
        If txt = "Dust/Sand Whirls in Vicinity" Then
            Traduire = "Tourbillons de poussière/sable dans les environs"
        End If
        If txt = "Fair" Then
            Traduire = "Ciel dégagé"
        End If
        If txt = "Fair/ Windy" Then
            Traduire = "Ciel dégagé / grand vent"
        End If
        If txt = "Fair and Breezy" Then
            Traduire = "Ciel dégagé et frais"
        End If
        If txt = "Fair and Windy" Then
            Traduire = "Ciel dégagé et grand vent"
        End If
        If txt = "Fair with Haze" Then
            Traduire = "Ciel dégagé avec brume légère"
        End If
        If txt = "Few Showers" Then
            Traduire = "Quelques averses"
        End If
        If txt = "Few Snow Showers" Then
            Traduire = "Quelques averses de neige"
        End If
        If txt = "Few Snow Showers / Wind" Then
            Traduire = "Quelques averses de neige / Vent"
        End If
        If txt = "Fog" Then
            Traduire = "Brouillard"
        End If
        If txt = "Fog in Vicinity" Then
            Traduire = "Brouillard dans les environs"
        End If
        If txt = "Fog/Mist" Then
            Traduire = "Brouillard/brume"
        End If
        If txt = "Freezing Drizzle" Then
            Traduire = "Crachin givrant"
        End If
        If txt = "Freezing Drizzle in Vicinity" Then
            Traduire = "Crachin givrant dans les environs"
        End If
        If txt = "Freezing Drizzle Rain" Then
            Traduire = "Crachin pluie givrant "
        End If
        If txt = "Freezing Drizzle Snow" Then
            Traduire = "Crachin neige givrant"
        End If
        If txt = "Freezing Fog" Then
            Traduire = "Brouillard givrant"
        End If
        If txt = "Freezing Fog in Vicinity" Then
            Traduire = "Brouillard givrant dans les environs"
        End If
        If txt = "Freezing Rain" Then
            Traduire = "Pluie givrante"
        End If
        If txt = "Freezing Rain in Vicinity" Then
            Traduire = "Pluie givrante dans les environs"
        End If
        If txt = "Freezing Rain Snow" Then
            Traduire = "Pluie neige givrante dans les environs"
        End If
        If txt = "Frigid" Then
            Traduire = "Grand froid"
        End If
        If txt = "Funnel Cloud in Vicinity" Then
            Traduire = "Nuage en entonnoir dans les environs"
        End If
        If txt = "Funnel Cloud" Then
            Traduire = "Nuage en entonnoir"
        End If
        If txt = "Hail" Then
            Traduire = "Grêle"
        End If
        If txt = "Hail Showers" Then
            Traduire = "Averses de grêle"
        End If
        If txt = "Haze" Then
            Traduire = "Brume légère"
        End If
        If txt = "Heavy Drizzle" Then
            Traduire = "Gros Crachin "
        End If
        If txt = "Heavy Drizzle Fog/Mist" Then
            Traduire = "Gros Crachin Brouillard/brume"
        End If
        If txt = "Heavy Drizzle Fog" Then
            Traduire = "Gros Crachin Brouillard"
        End If
        If txt = "Heavy Drizzle Ice Pellets" Then
            Traduire = "Gros Crachin Grêlons"
        End If
        If txt = "Heavy Drizzle Snow" Then
            Traduire = "Gros Crachin Neige"
        End If
        If txt = "Heavy Dust Storm" Then
            Traduire = "Grosse tempête de poussière"
        End If
        If txt = "Heavy Freezing Drizzle Rain" Then
            Traduire = "Gros Crachin verglassant Pluie"
        End If
        If txt = "Heavy Freezing Drizzle Snow" Then
            Traduire = "Gros Crachin verglassant Neige"
        End If
        If txt = "Heavy Freezing Drizzle" Then
            Traduire = "Gros Crachin verglassant"
        End If
        If txt = "Heavy Freezing Fog" Then
            Traduire = "Gros brouillard verglassant"
        End If
        If txt = "Heavy Freezing Rain Snow" Then
            Traduire = "Grosse pluie verglassante Neige"
        End If
        If txt = "Heavy Freezing Rain" Then
            Traduire = "Grosse pluie verglassante"
        End If
        If txt = "Heavy Ice Pellets" Then
            Traduire = "Gros Grêlons"
        End If
        If txt = "Heavy Ice Pellets Drizzle" Then
            Traduire = "Gros Grêlons Crachin"
        End If
        If txt = "Heavy Ice Pellets Rain" Then
            Traduire = "Gros Grêlons Pluie"
        End If
        If txt = "Heavy Rain" Then
            Traduire = "Grosse pluie"
        End If
        If txt = "Heavy Rain Fog/Mist" Then
            Traduire = "Grosse pluie Brouillard/brume"
        End If
        If txt = "Heavy Rain Fog" Then
            Traduire = "Grosse pluie Brouillard"
        End If
        If txt = "Heavy Rain Freezing Drizzle" Then
            Traduire = "Grosse pluie Crachin verglassant"
        End If
        If txt = "Heavy Rain Freezing Rain" Then
            Traduire = "Grosse pluie Pluie verglassante"
        End If
        If txt = "Heavy Rain Ice Pellets" Then
            Traduire = "Grosse pluie Grêlons"
        End If
        If txt = "Heavy Rain Icy" Then
            Traduire = "Grosse pluie verglassante"
        End If
        If txt = "Heavy Rain Showers Fog/Mist" Then
            Traduire = "Grosse pluie Averses Brouillard/brume"
        End If
        If txt = "Heavy Rain Showers" Then
            Traduire = "Grosse pluie Averses"
        End If
        If txt = "Heavy Rain Snow" Then
            Traduire = "Grosse pluie Neige"
        End If
        If txt = "Heavy Sand Storm" Then
            Traduire = "Grosse tempête de sable "
        End If
        If txt = "Heavy Showers Rain Fog/Mist" Then
            Traduire = "Grosses averses Pluie Brouillard/brume"
        End If
        If txt = "Heavy Showers Rain" Then
            Traduire = "Grosses averses Pluie"
        End If
        If txt = "Heavy Showers Snow Fog/Mist" Then
            Traduire = "Grosses Neige Pluie Brouillard/brume"
        End If
        If txt = "Heavy Showers Snow Fog" Then
            Traduire = "Grosses averses Neige Brouillard"
        End If
        If txt = "Heavy Showers Snow" Then
            Traduire = "Grosses averses Neige"
        End If
        If txt = "Heavy Snow" Then
            Traduire = "Beaucoup de neige"
        End If
        If txt = "Heavy Snow Drizzle" Then
            Traduire = "Beaucoup de neige Crachin"
        End If
        If txt = "Heavy Snow Fog/Mist" Then
            Traduire = "Beaucoup de neige Brouillard/brume"
        End If
        If txt = "Heavy Snow Fog" Then
            Traduire = "Beaucoup de neige Brouillard"
        End If
        If txt = "Heavy Snow Freezing Drizzle" Then
            Traduire = "Beaucoup de neige Crachin verglassant"
        End If
        If txt = "Heavy Snow Freezing Rain" Then
            Traduire = "Beaucoup de neige Pluie verglassante"
        End If
        If txt = "Heavy Snow Rain" Then
            Traduire = "Beaucoup de neige Pluie"
        End If
        If txt = "Heavy Snow Showers Fog/Mist" Then
            Traduire = "Beaucoup de neige Averses Brouillard/brume"
        End If
        If txt = "Heavy Snow Showers Fog" Then
            Traduire = "Beaucoup de neige Averses Brouillard"
        End If
        If txt = "Heavy Snow Showers" Then
            Traduire = "Beaucoup de neige Averses"
        End If
        If txt = "Heavy T-Storm" Then
            Traduire = "Gros orage"
        End If
        If txt = "Heavy T-Storms Rain Fog/Mist" Then
            Traduire = "Gros orage Brouillard/brume"
        End If
        If txt = "Heavy T-Storms Rain Fog" Then
            Traduire = "Gros orage Brouillard"
        End If
        If txt = "Heavy T-Storms Rain Hail Fog/Mist" Then
            Traduire = "Gros orage Grêle Brouillard/brume"
        End If
        If txt = "Heavy T-Storms Rain Hail Fog" Then
            Traduire = "Gros orage Grêle  Brouillard"
        End If
        If txt = "Heavy T-Storms Rain Hail Haze" Then
            Traduire = "Gros orage Grêle Brume légère"
        End If
        If txt = "Heavy T-Storms Rain Hail" Then
            Traduire = "Gros orage Grêle"
        End If
        If txt = "Heavy T-Storms Rain Haze" Then
            Traduire = "Gros orage Brume légère"
        End If
        If txt = "Heavy T-Storms Rain" Then
            Traduire = "Gros orage Pluie"
        End If
        If txt = "Heavy T-Storms Snow" Then
            Traduire = "Gros orage Neige"
        End If
        If txt = "Ice Crystals" Then
            Traduire = "Cristaux de glace"
        End If
        If txt = "Ice Pellets Drizzle" Then
            Traduire = "Grêlons Crachin"
        End If
        If txt = "Ice Pellets in Vicinity" Then
            Traduire = "Grêlons dans les environs "
        End If
        If txt = "Ice Pellets Rain" Then
            Traduire = "Grêlons Pluie"
        End If
        If txt = "Ice Pellets" Then
            Traduire = "Grêlons"
        End If
        If txt = "Isolated T-Storms" Then
            Traduire = "Orages isolés"
        End If
        If txt = "Light Drizzle" Then
            Traduire = "Léger crachin"
        End If
        If txt = "Light Drizzle Fog/Mist" Then
            Traduire = "Léger crachin Brouillard/brume"
        End If
        If txt = "Light Drizzle Fog" Then
            Traduire = "Léger crachin Brouillard"
        End If
        If txt = "Light Drizzle Ice Pellets" Then
            Traduire = "Léger crachin Grêlons"
        End If
        If txt = "Light Drizzle Snow" Then
            Traduire = "Léger crachin Neige"
        End If
        If txt = "Light Freezing Drizzle" Then
            Traduire = "Léger crachin verglassant"
        End If
        If txt = "Light Freezing Drizzle Rain" Then
            Traduire = "Léger crachin verglassant Pluie"
        End If
        If txt = "Light Freezing Drizzle Snow" Then
            Traduire = "Léger crachin verglassant Neige"
        End If
        If txt = "Light Freezing Fog" Then
            Traduire = "Léger brouillard verglassant"
        End If
        If txt = "Light Freezing Rain" Then
            Traduire = "Légère pluie verglassante"
        End If
        If txt = "Light Freezing Rain Snow" Then
            Traduire = "Légère pluie verglassante Neige"
        End If
        If txt = "Light Ice Pellets Drizzle" Then
            Traduire = "Petit Grêlons Crachin"
        End If
        If txt = "Light Ice Pellets Rain" Then
            Traduire = "Petit Grêlons Pluie"
        End If
        If txt = "Light Ice Pellets" Then
            Traduire = "Petit Grêlons"
        End If
        If txt = "Light Rain" Then
            Traduire = "Légère Pluie"
        End If
        If txt = "Light Rain Early" Then
            Traduire = "Légère Pluie matinal"
        End If
        If txt = "Light Rain Fog/Mist" Then
            Traduire = "Légère Pluie Brouillard/brume"
        End If
        If txt = "Light Rain Fog" Then
            Traduire = "Légère Pluie Brouillard"
        End If
        If txt = "Light Rain Freezing Drizzle" Then
            Traduire = "Légère Pluie Crachin verglassant"
        End If
        If txt = "Light Rain Freezing Rain" Then
            Traduire = "Légère Pluie Pluie verglassante"
        End If
        If txt = "Light Rain Ice Pellets" Then
            Traduire = "Légère Pluie Grêlons"
        End If
        If txt = "Light Rain Icy" Then
            Traduire = "Légère pluie verglassante"
        End If
        If txt = "Light Rain Late" Then
            Traduire = "Légère Pluie tardive"
        End If
        If txt = "Light Rain Shower" Then
            Traduire = "Légère pluie Averse"
        End If
        If txt = "Light Rain Shower and Windy" Then
            Traduire = "Légère pluie Averses et Vent"
        End If
        If txt = "Light Rain Showers" Then
            Traduire = "Légères pluie Averses"
        End If
        If txt = "Light Rain Snow" Then
            Traduire = "Légères pluie Neige"
        End If
        If txt = "Light Rain with Thunder" Then
            Traduire = "Légère Pluie avec tonnerre"
        End If
        If txt = "Light Showers Rain" Then
            Traduire = "Légère averses Pluie"
        End If
        If txt = "Light Showers Rain Fog/Mist" Then
            Traduire = "Légère averses Pluie Brouillard/brume"
        End If
        If txt = "Light Showers Snow" Then
            Traduire = "Légère averses Neige"
        End If
        If txt = "Light Showers Snow Fog" Then
            Traduire = "Légère averses Neige Brouillard"
        End If
        If txt = "Light Showers Snow Fog/Mist" Then
            Traduire = "Légère averses Neige Brouillard/brume"
        End If
        If txt = "Light Snow" Then
            Traduire = "Peu de neige"
        End If
        If txt = "Light Snow Drizzle" Then
            Traduire = "Peu de neige Crachin"
        End If
        If txt = "Light Snow Fall" Then
            Traduire = "Légère chutes de neige"
        End If
        If txt = "Light Snow Fog/Mist" Then
            Traduire = "Peu de neige Brouillard/brume"
        End If
        If txt = "Light Snow Fog" Then
            Traduire = "Peu de neige Brouillard"
        End If
        If txt = "Light Snow Freezing Drizzle" Then
            Traduire = "Peu de neige Crachin verglassant"
        End If
        If txt = "Light Snow Freezing Rain" Then
            Traduire = "Peu de neige Pluie Verglassante"
        End If
        If txt = "Light Snow Grains" Then
            Traduire = "Quelques flocons de neige"
        End If
        If txt = "Light Snow Rain" Then
            Traduire = "Peu de neige Pluie"
        End If
        If txt = "Light Snow Shower" Then
            Traduire = "Légère averse de neige"
        End If
        If txt = "Light Snow Showers Fog/Mist" Then
            Traduire = "Légère averses de neige Brouillard/brume"
        End If
        If txt = "Light Snow Showers Fog" Then
            Traduire = "Légère averses de neige Brouillard"
        End If
        If txt = "Light T-Storms Rain Fog/Mist" Then
            Traduire = "Léger orages Pluie Brouillard/brume"
        End If
        If txt = "Light T-Storms Rain Fog" Then
            Traduire = "Léger orages Pluie Brouillard"
        End If
        If txt = "Light T-Storms Rain Hail Fog/Mist" Then
            Traduire = "Léger orages Pluie Grêle Brouillard/brume"
        End If
        If txt = "Light T-Storms Rain Hail Fog" Then
            Traduire = "Léger orages Pluie Grêle Brouillard"
        End If
        If txt = "Light T-Storms Rain Hail Haze" Then
            Traduire = "Léger orages Pluie Grêle Brume légère"
        End If
        If txt = "Light T-Storms Rain Hail" Then
            Traduire = "Léger orages Pluie Grêle"
        End If
        If txt = "Light T-Storms Rain Haze" Then
            Traduire = "Léger orages Pluie Brume légère"
        End If
        If txt = "Light T-Storms Rain" Then
            Traduire = "Léger orages Pluie"
        End If
        If txt = "Light T-Storms Snow" Then
            Traduire = "Léger orages Neige"
        End If
        If txt = "Lightening" Then
            Traduire = "Eclairs"
        End If
        If txt = "Lightenings" Then
            Traduire = "Eclairs"
        End If
        If txt = "Mostly Clear" Then
            Traduire = "Ciel plutôt dégagé"
        End If
        If txt = "Mostly Cloudy" Then
            Traduire = "Plutôt nuageux"
        End If
        If txt = "Mostly Cloudy and Breezy" Then
            Traduire = "Plutôt nuageux et Venteux"
        End If
        If txt = "Mostly Cloudy and Windy" Or txt = "Mostly Cloudy/Wind" Then
            Traduire = "Plutôt nuageux et Grand vent"
        End If
        If txt = "Mostly Cloudy with Haze" Then
            Traduire = "Plutôt nuageux avec Légère Brume"
        End If
        If txt = "Mostly Sunny" Then
            Traduire = "Plutôt ensoleillé"
        End If
        If txt = "Mostly Sunny / Wind" Then
            Traduire = "Plutôt ensoleillé / vent"
        End If
        If txt = "Overcast" Then
            Traduire = "Couvert"
        End If
        If txt = "Overcast and Breezy" Then
            Traduire = "Couvert et Venteux"
        End If
        If txt = "Overcast and Windy" Then
            Traduire = "Couvert et Grand vent"
        End If
        If txt = "Overcast with Haze" Then
            Traduire = "Couvert avec légère brume"
        End If
        If txt = "Partial Fog" Then
            Traduire = "Banc de Brouillard"
        End If
        If txt = "Partial Fog in Vicinity" Then
            Traduire = "Banc de Brouillard dans les environs"
        End If
        If txt = "P Cloudy" Then
            Traduire = "Partiellement nuageux"
        End If
        If txt = "Partly Cloudy" Then
            Traduire = "Partiellement nuageux"
        End If
        If txt = "Partly Cloudy and Breezy" Then
            Traduire = "Partiellement nuageux et Venteux"
        End If
        If txt = "Partly Cloudy and Windy" Then
            Traduire = "Partiellement nuageux et Grand vent"
        End If
        If txt = "Partly Cloudy / Wind" Then
            Traduire = "Partiellement nuageux / Vent"
        End If
        If txt = "Partly Cloudy/ Windy" Or txt = "Partly Cloudy/Wind" Then
            Traduire = "Partiellement nuageux / Venteux"
        End If
        If txt = "Party Cloudy with Haze" Then
            Traduire = "Partiellement nuageux avec légère brume"
        End If
        If txt = "Partly Sunny" Then
            Traduire = "Partiellement ensoleillé"
        End If
        If txt = "Patches of Fog" Then
            Traduire = "Nappes de Brouillard"
        End If
        If txt = "Patches of Fog in Vicinity" Then
            Traduire = "Nappes de Brouillard dans les environs"
        End If
        If txt = "PM light rain" Then
            Traduire = "PM Légère Pluie"
        End If
        If txt = "PM Rain / Snow" Then
            Traduire = "PM Pluie / Neige"
        End If
        If txt = "PM Rain / Wind" Then
            Traduire = "PM Pluie / Vent"
        End If
        If txt = "PM Showers" Then
            Traduire = "PM Averses"
        End If
        If txt = "PM Snow Showers" Then
            Traduire = "PM averses neigeuses"
        End If
        If txt = "PM T-Storms" Then
            Traduire = "Orages l après-midi"
        End If
        If txt = "Rain" Then
            Traduire = "Pluie"
        End If
        If txt = "Rain / Snow" Then
            Traduire = "Pluie / neige"
        End If
        If txt = "Rain / Snow Showers" Then
            Traduire = "Pluie / averses neigeuses"
        End If
        If txt = "Rain / Snow Showers Early" Then
            Traduire = "Pluie / averses neigeuses matinales"
        End If
        If txt = "Rain / Thunder" Then
            Traduire = "Pluie / Tonnerre"
        End If
        If txt = "Rain / Wind" Then
            Traduire = "Pluie / Vent"
        End If
        If txt = "Rain and Snow" Then
            Traduire = "Pluie et Neige"
        End If
        If txt = "Rain Early" Then
            Traduire = "Pluie matinale"
        End If
        If txt = "Rain Fog/Mist" Then
            Traduire = "Pluie Brouillard/brume"
        End If
        If txt = "Rain Fog" Then
            Traduire = "Pluie Brouillard"
        End If
        If txt = "Rain Freezing Drizzle" Then
            Traduire = "Pluie Crachin verglassant"
        End If
        If txt = "Rain Freezing Rain" Then
            Traduire = "Pluie Pluie Verglassante"
        End If
        If txt = "Rain Ice Pellets" Then
            Traduire = "Pluie Grêlons"
        End If
        If txt = "Rain Shower" Then
            Traduire = "Pluie Averses"
        End If
        If txt = "Rain Showers Fog/Mist" Then
            Traduire = "Pluie Averses Brouillard/brume"
        End If
        If txt = "Rain Showers in Vicinity Fog/Mist" Then
            Traduire = "Pluie Averses dans les environs Brouillard/brume"
        End If
        If txt = "Rain Showers in Vicinity" Then
            Traduire = "Pluie Averses dans les environs"
        End If
        If txt = "Rain Snow" Then
            Traduire = "Pluie Neige"
        End If
        If txt = "Rain to Snow" Then
            Traduire = "Pluie vers Neige"
        End If
        If txt = "Sand Storm" Then
            Traduire = "Tempête de Sable"
        End If
        If txt = "Sand Storm in Vicinity" Then
            Traduire = "Tempête de Sable dans les environs"
        End If
        If txt = "Sand" Then
            Traduire = "Sable"
        End If
        If txt = "Shallow Fog" Then
            Traduire = "Brouillard superficiel"
        End If
        If txt = "Shallow Fog in Vicinity" Then
            Traduire = "Brouillard superficiel dans les environs"
        End If
        If txt = "Scattered Showers" Then
            Traduire = "Averses éparses"
        End If
        If txt = "Scattered Showers / Wind" Then
            Traduire = "Averses éparses / Vent"
        End If
        If txt = "Scattered Snow Showers" Then
            Traduire = "Averses neigeuses éparses"
        End If
        If txt = "Scattered Snow Showers / Wind" Then
            Traduire = "Averses neigeuses éparses / Vent"
        End If
        If txt = "Scattered Strong Storms" Then
            Traduire = "Violents orages locals"
        End If
        If txt = "Scattered T-Storms" Then
            Traduire = "Orages éparses"
        End If
        If txt = "Showers" Then
            Traduire = "Averses"
        End If
        If txt = "Showers / Wind" Then
            Traduire = "Averses / Vent"
        End If
        If txt = "Showers Hail" Then
            Traduire = "Averses Grêle"
        End If
        If txt = "Showers Ice Pellets" Then
            Traduire = "Averses Grêlons"
        End If
        If txt = "Showers in the Vicinity" Then
            Traduire = "Averses dans les environs"
        End If
        If txt = "Showers in Vicinity Fog/Mist" Then
            Traduire = "Averses dans les environs Brouillard/brume"
        End If
        If txt = "Showers in Vicinity Fog" Then
            Traduire = "Averses dans les environs Brouillard"
        End If
        If txt = "Showers in Vicinity Haze" Then
            Traduire = "Averses dans les environs Brume légère"
        End If
        If txt = "Showers in Vicinity Snow" Then
            Traduire = "Averses dans les environs Neige"
        End If
        If txt = "Showers Early" Then
            Traduire = "Averses matinales"
        End If
        If txt = "Showers Late" Then
            Traduire = "Averses tardives"
        End If
        If txt = "Showers Rain" Then
            Traduire = "Averses Pluie"
        End If
        If txt = "Showers Rain Fog/Mist" Then
            Traduire = "Averses Pluie Brouillard/brume"
        End If
        If txt = "Showers Rain in Vicinity" Then
            Traduire = "Averses Pluie dans les environs"
        End If
        If txt = "Showers Rain in Vicinity Fog/Mist" Then
            Traduire = "Averses Pluie dans les environs Brouillard/brume"
        End If
        If txt = "Showers Snow" Then
            Traduire = "Averses Neige"
        End If
        If txt = "Showers Snow Fog" Then
            Traduire = "Averses Neige Brouillard"
        End If
        If txt = "Showers Snow Fog/Mist" Then
            Traduire = "Averses Neige Brouillard/brume"
        End If
        If txt = "Smoke" Then
            Traduire = "fumée"
        End If
        If txt = "Snow" Then
            Traduire = "Neige"
        End If
        If txt = "Snow / Rain Icy Mix" Then
            Traduire = "Mélange Neige / Pluie Verglassante"
        End If
        If txt = "Snow and Fog" Then
            Traduire = "Neige et Brouillard"
        End If
        If txt = "Snow Drizzle" Then
            Traduire = "Neige Crachin"
        End If
        If txt = "Snow Fog/Mist" Then
            Traduire = "Neige Brouillard/brume"
        End If
        If txt = "Snow Freezing Drizzle" Then
            Traduire = "Neige Crachin verglassant"
        End If
        If txt = "Snow Freezing Rain" Then
            Traduire = "Neige Pluie verglassante"
        End If
        If txt = "Snow Rain" Then
            Traduire = "Neige Pluie"
        End If
        If txt = "Snow Shower" Then
            Traduire = "Averses de neige"
        End If
        If txt = "Snow Shower / Wind" Then
            Traduire = "Averses de neige / vent"
        End If
        If txt = "Snow Shower Early" Then
            Traduire = "Averses de neige matinales"
        End If
        If txt = "Snow Showers Fog/Mist" Then
            Traduire = "Averses de neige Brouillard/brume"
        End If
        If txt = "Snow Showers Fog" Then
            Traduire = "Averses de neige Brouillard"
        End If
        If txt = "Snow Showers in Vicinity" Then
            Traduire = "Averses de neige dans les environs"
        End If
        If txt = "Snow Showers in Vicinity Fog" Then
            Traduire = "Averses de neige dans les environs Brouillard"
        End If
        If txt = "Snow Showers in Vicinity Fog/Mist" Then
            Traduire = "Averses de neige dans les environs Brouillard/brume"
        End If
        If txt = "Snow Showers Late" Then
            Traduire = "Averses de neige tardives"
        End If
        If txt = "Snow to Rain" Then
            Traduire = "Neige vers Pluie"
        End If
        If txt = "Snowflakes" Then
            Traduire = "Flocons de neige"
        End If
        If txt = "Sunny" Then
            Traduire = "Ensoleillé"
        End If
        If txt = "Sunny / Wind" Then
            Traduire = "Ensoleillé / Vent"
        End If
        If txt = "Sunny Day" Then
            Traduire = "Journée ensoleillé"
        End If
        If txt = "Thunder" Then
            Traduire = "Tonnerre"
        End If
        If txt = "Thunder in the Vicinity" Then
            Traduire = "Tonnerre dans les environs"
        End If
        If txt = "T-Storms" Then
            Traduire = "Orages"
        End If
        If txt = "T-Storms Early" Then
            Traduire = "Orages matinaux"
        End If
        If txt = "T-Storms Fog" Then
            Traduire = "Orages Brouillard"
        End If
        If txt = "T-Storms Hail Fog" Then
            Traduire = "Orages Grêle Brouillard"
        End If
        If txt = "T-Storms Hail" Then
            Traduire = "Orages Grêle"
        End If
        If txt = "T-Storms Haze in Vicinity Hail" Then
            Traduire = "Orages brume dans les environs Grêle"
        End If
        If txt = "T-Storms Haze in Vicinity" Then
            Traduire = "Orages brume dans les environs"
        End If
        If txt = "T-Storms Heavy Rain" Then
            Traduire = "Orages Grosse Pluie"
        End If
        If txt = "T-Storms Heavy Rain Fog" Then
            Traduire = "Orages Grosse Pluie Brouillard"
        End If
        If txt = "T-Storms Heavy Rain Fog/Mist" Then
            Traduire = "Orages Grosse pluie Brouillard/brume"
        End If
        If txt = "T-Storms Heavy Rain Hail Fog" Then
            Traduire = "Orages Grosse Pluie Grêle Brouillard"
        End If
        If txt = "T-Storms Heavy Rain Hail Fog/Mist" Then
            Traduire = "Orages Grosse Pluie Grêle Brouillard/brume"
        End If
        If txt = "T-Storms Heavy Rain Hail Haze" Then
            Traduire = "Orages Grosse Pluie Grêle Brume légère"
        End If
        If txt = "T-Storms Heavy Rain Hail" Then
            Traduire = "Orages Grosse Pluie Grêle"
        End If
        If txt = "T-Storms Heavy Rain Haze" Then
            Traduire = "Orages Grosse Pluie Brume légère"
        End If
        If txt = "T-Storms Ice Pellets" Then
            Traduire = "Orages Grêlons"
        End If
        If txt = "T-Storms in Vicinity" Then
            Traduire = "Orages dans les environs"
        End If
        If txt = "T-Storms in Vicinity Fog" Then
            Traduire = "Orages dans les environs Brouillard"
        End If
        If txt = "T-Storms in Vicinity Fog/Mist" Then
            Traduire = "Orages dans les environs Brouillard/brume"
        End If
        If txt = "T-Storms in Vicinity Hail Fog/Mist" Then
            Traduire = "Orages dans les environs Grêle Brouillard/brume"
        End If
        If txt = "T-Storms in Vicinity Hail Haze" Then
            Traduire = "Orages dans les environs Grêle Brume légère"
        End If
        If txt = "T-Storms in Vicinity Hail" Then
            Traduire = "Orages dans les environs Grêle"
        End If
        If txt = "T-Storms in Vicinity Haze" Then
            Traduire = "Orages dans les environs Brume légère"
        End If
        If txt = "T-Storms Light Rain" Then
            Traduire = "Orages Légère pluie"
        End If
        If txt = "T-Storms Light Rain Fog" Then
            Traduire = "Orages Légère pluie Brouillard"
        End If
        If txt = "T-Storms Light Rain Fog/Mist" Then
            Traduire = "Orages Légère pluie Brouillard/brume"
        End If
        If txt = "T-Storms Light Rain Hail" Then
            Traduire = "Orages Légère pluie Grêle"
        End If
        If txt = "T-Storms Light Rain Hail Fog" Then
            Traduire = "Orages Légère pluie Grêle Brouillard"
        End If
        If txt = "T-Storms Light Rain Hail Fog/Mist" Then
            Traduire = "Orages Légère pluie Grêle Brouillard/brume"
        End If
        If txt = "T-Storms Light Rain Hail Haze" Then
            Traduire = "Orages Légère pluie Grêle Brume légère"
        End If
        If txt = "T-Storms Light Rain Haze" Then
            Traduire = "Orages Légère pluie Brume légère"
        End If
        If txt = "T-Storms Rain Fog/Mist" Then
            Traduire = "Orages Pluie Brouillard/brume"
        End If
        If txt = "T-Storms Rain Hail Fog/Mist" Then
            Traduire = "Orages Pluie Grêle Brouillard/brume"
        End If
        If txt = "T-Storms Showers in Vicinity" Then
            Traduire = "Orages Averses dans les environs"
        End If
        If txt = "T-Storms Showers in Vicinity Hail" Then
            Traduire = "Orages Averses dans les environs Grêle"
        End If
        If txt = "T-Storms Snow" Then
            Traduire = "Orages Neige"
        End If
        If txt = "Windy" Then
            Traduire = "Grand vent"
        End If
        If txt = "Windy / Snowy" Then
            Traduire = "Grand vent / neigeux"
        End If
        If txt = "Windy Rain" Then
            Traduire = "giboulées"
        End If
        If txt = "Wintry Mix" Then
            Traduire = "Mélanges pluie/neige"
        End If
        If txt = "Wintry Mix / Wind" Then
            Traduire = "Mélanges pluie/neige / vent"
        End If

    End Function

#End Region

End Class