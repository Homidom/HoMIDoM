Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Web.Http.SelfHost
Imports System.Web.Http
Imports System.Net.Http.Formatting
Imports System.Globalization
Imports System.Threading

' Auteur : domomath sur une base HoMIDoM
' Date : 04/10/2015

''' <summary>Driver ImperiHome API d'interface avec l'IHM ImperiHome</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_ImperiHome
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "7BC5AD0A-6A9C-11E5-9EAE-802E1D5D46B0"
    Dim _Nom As String = "ImperiHome"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Interface ImperiHome"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "WEB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "ImperiHome"
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
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True

#End Region

#Region "Variables internes"

    Private Shared serverhttp As HttpSelfHostServer
    Private Shared _allDevImperi As DeviceList
    Public Shared Property ServerKey() As String
    Public Shared Property driverDebug As Boolean
    Private Shared homidom As IHoMIDom

    'param avancé
    Dim _DEBUG As Boolean = False

#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements homidom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
        End Set
    End Property

    Public Property COM() As String Implements homidom.HoMIDom.IDriver.COM
        Get
            Return _Com
        End Get
        Set(ByVal value As String)
            _Com = value
        End Set
    End Property
    Public ReadOnly Property Description() As String Implements homidom.HoMIDom.IDriver.Description
        Get
            Return _Description
        End Get
    End Property
    Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements homidom.HoMIDom.IDriver.DeviceSupport
        Get
            Return _DeviceSupport
        End Get
    End Property
    Public Property Parametres() As System.Collections.ArrayList Implements homidom.HoMIDom.IDriver.Parametres
        Get
            Return _Parametres
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _Parametres = value
        End Set
    End Property

    Public Property LabelsDriver() As System.Collections.ArrayList Implements homidom.HoMIDom.IDriver.LabelsDriver
        Get
            Return _LabelsDriver
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDriver = value
        End Set
    End Property
    Public Property LabelsDevice() As System.Collections.ArrayList Implements homidom.HoMIDom.IDriver.LabelsDevice
        Get
            Return _LabelsDevice
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDevice = value
        End Set
    End Property



    Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements homidom.HoMIDom.IDriver.DriverEvent

    Public Property Enable() As Boolean Implements homidom.HoMIDom.IDriver.Enable
        Get
            Return _Enable
        End Get
        Set(ByVal value As Boolean)
            _Enable = value
        End Set
    End Property
    Public ReadOnly Property ID() As String Implements homidom.HoMIDom.IDriver.ID
        Get
            Return _ID
        End Get
    End Property
    Public Property IP_TCP() As String Implements homidom.HoMIDom.IDriver.IP_TCP
        Get
            Return _IP_TCP
        End Get
        Set(ByVal value As String)
            _IP_TCP = value
        End Set
    End Property
    Public Property IP_UDP() As String Implements homidom.HoMIDom.IDriver.IP_UDP
        Get
            Return _IP_UDP
        End Get
        Set(ByVal value As String)
            _IP_UDP = value
        End Set
    End Property
    Public ReadOnly Property IsConnect() As Boolean Implements homidom.HoMIDom.IDriver.IsConnect
        Get
            Return _IsConnect
        End Get
    End Property
    Public Property Modele() As String Implements homidom.HoMIDom.IDriver.Modele
        Get
            Return _Modele
        End Get
        Set(ByVal value As String)
            _Modele = value
        End Set
    End Property
    Public ReadOnly Property Nom() As String Implements homidom.HoMIDom.IDriver.Nom
        Get
            Return _Nom
        End Get
    End Property
    Public Property Picture() As String Implements homidom.HoMIDom.IDriver.Picture
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property
    Public Property Port_TCP() As String Implements homidom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
            _Port_TCP = value
        End Set
    End Property
    Public Property Port_UDP() As String Implements homidom.HoMIDom.IDriver.Port_UDP
        Get
            Return _Port_UDP
        End Get
        Set(ByVal value As String)
            _Port_UDP = value
        End Set
    End Property
    Public ReadOnly Property Protocol() As String Implements homidom.HoMIDom.IDriver.Protocol
        Get
            Return _Protocol
        End Get
    End Property
    Public Property Refresh() As Integer Implements homidom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property
    Public Property Server() As HoMIDom.HoMIDom.Server Implements homidom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property
    Public ReadOnly Property Version() As String Implements homidom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property
    Public ReadOnly Property OsPlatform() As String Implements homidom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
        End Get
    End Property
    Public Property StartAuto() As Boolean Implements homidom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property
    Public Property AutoDiscover() As Boolean Implements homidom.HoMIDom.IDriver.AutoDiscover
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
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements homidom.HoMIDom.IDriver.VerifChamp

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
    Public Sub Start() Implements homidom.HoMIDom.IDriver.Start
        Try
            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
                driverDebug = _DEBUG
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            Dim fileName = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\system.json"

            If System.IO.File.Exists(fileName) Then
                CurrentServer = Global.HoMIDom.HoMIDom.Server.Instance
                ServerKey = _IdSrv
                Dim webApiUrl As String = "http://" & _Server.GetIPSOAP & ":" & _Server.GetPortSOAP & "/imperihome/"
                Dim config = New HttpSelfHostConfiguration(webApiUrl)

                config.Routes.MapHttpRoute("DefaultApi", "{key}/{controller}/{id}", New With { _
                 .id = RouteParameter.[Optional]
                })
                config.Routes.MapHttpRoute("CommandApi", "{key}/{controller}/{id}/action/{command}/{param}", New With { _
                .param = RouteParameter.[Optional], _
                .action = "ExecuteCommand"
                }) 'DevicesController
                config.Routes.MapHttpRoute("ValueApi", "{key}/{controller}", New With { _
                 .action = "GetValue"
                }) 'DevicesController / RoomsController / SystemController
                config.Routes.MapHttpRoute("HistoApi", "{key}/{controller}/{id}/{param}/histo/{startdate}/{enddate}", New With { _
                 .action = "GetHisto"
                }) 'DevicesController

                config.Formatters.Add(config.Formatters.JsonFormatter)
                config.Formatters(0) = New JsonMediaTypeFormatter
                config.Formatters(0).SupportedMediaTypes.Add(New System.Net.Http.Headers.MediaTypeHeaderValue("application/json"))
                
                fileName = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\devices.json"
                If System.IO.File.Exists(fileName) Then
                    Dim stream = System.IO.File.ReadAllText(fileName)
                    _allDevImperi = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(DeviceList))
                End If
                WriteLog("Driver " & Me.Nom & " connecté avec l'adresse :  " & webApiUrl)
                serverhttp = New HttpSelfHostServer(config)
                serverhttp.OpenAsync()

                _IsConnect = True
                WriteLog("Driver " & Me.Nom & " démarré")
            Else
                _IsConnect = False
                WriteLog("Driver " & Me.Nom & " non démarré")
                WriteLog("ERR: Start Driver " & Me.Nom & " Configurez les composants que vous souhaitez voir dans ImperiHome, allez dans HoMIAdmin, Configuration, Onglet Configuration, Bouton 'Export Imperihome' ")
            End If

        Catch ex As Exception
            _IsConnect = False
            WriteLog("Driver " & Me.Nom & " non démarré")
            WriteLog("ERR: Start Driver " & Me.Nom & " Erreur démarrage " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements homidom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            WriteLog("Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            WriteLog("ERR: STOP Driver " & Me.Nom & " Erreur arrêt " & ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements homidom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    ''' <summary>Intérroger un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <remarks>pas utilisé</remarks>
    Public Sub Read(ByVal Objet As Object) Implements homidom.HoMIDom.IDriver.Read

        Try

            If _Enable = False Then Exit Sub

            If _IsConnect = False Then
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

            'récupération des paramétres avancés pour rafraichir l'affichage
            Try
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

        Catch ex As Exception
            WriteLog("ERR: READ, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représentant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements homidom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then Exit Sub

            If _IsConnect = False Then
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

        Catch ex As Exception
            WriteLog("ERR: Write, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements homidom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            WriteLog("ERR: DeleteDevice, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements homidom.HoMIDom.IDriver.NewDevice
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
            WriteLog("ERR: add_devicecommande, Exception :" & ex.Message)
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
            WriteLog("ERR: add_devicecommande, Exception : " & ex.Message)
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
            WriteLog("ERR: add_devicecommande, Exception : " & ex.Message)
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
            WriteLog("ERR: add_devicecommande, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "@", "")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("REFRESH", "@", "")
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
        ' Attente de 3s pour eviter le relancement de la procedure dans le laps de temps
        'System.Threading.Thread.Sleep(3000)

    End Sub

#End Region

#Region "Fonctions internes"

    Public Shared Property CurrentServer() As IHoMIDom
        Get
            Return homidom
        End Get
        Set(ByVal value As IHoMIDom)
            homidom = value
        End Set
    End Property

    Public Shared Property AllDevImperi As DeviceList
        Get
            Return _allDevImperi
        End Get
        Set(ByVal value As DeviceList)
            _allDevImperi = value
        End Set
    End Property

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

#Region "Class externe"
Public MustInherit Class BaseHoMIdomController
    Inherits ApiController

    'Public Property ServerKey As String
    Public ReadOnly Property ServerKey() As String
        Get
            Return Me.ControllerContext.RouteData.Values("key")
        End Get
    End Property

End Class

Public Class DevicesController
    Inherits BaseHoMIdomController

    <HttpGet()>
    Public Function ExecuteCommand(ByVal id As String, ByVal command As String, Optional ByVal param As String = "") As Boolean
        Me.ExecuteCommandWithParams(id, command, param)
        Return True
    End Function

    <HttpGet()>
    Public Function GetHisto(ByVal id As String, ByVal param As String, ByVal startDate As String, ByVal enddate As String) As Object
        Dim allHistImperi As ValueList = New ValueList

        Dim fileName = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\devices.json"

        If System.IO.File.Exists(fileName) Then
            Dim stream = System.IO.File.ReadAllText(fileName)

            Dim allDev = Driver_ImperiHome.CurrentServer.GetAllDevices(Me.ServerKey)
            Dim allDevImperi As DeviceList = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(DeviceList))
            Dim allHisto As Object = ""
            Dim epoch As New DateTime(1970, 1, 1)
            Dim dtstart As String = Format(epoch.AddSeconds(startDate / 1000), "yyyy-MM-dd HH:mm:ss")
            Dim dtend As String = Format(epoch.AddSeconds(enddate / 1000), "yyyy-MM-dd HH:mm:ss")

            For Each devImperi In allDevImperi.devices
                If id = devImperi.id Then
                    For Each dev In allDev
                        If dev.Name = devImperi.name Then
                            allHisto = Driver_ImperiHome.CurrentServer.GetHistoDeviceSource(Me.ServerKey, dev.ID, "Value", dtstart, dtend)
                        End If
                    Next
                End If
            Next

            If allHisto.count > 0 Then
                For Each histo In allHisto
                    Dim Val As Value = New Value
                    Val.date = date_to_long(histo.DateTime)
                    Val.value = histo.Value
                    allHistImperi.values.Add(Val)
                Next
            Else
                Dim Val As Value = New Value
                Val.Date = date_to_long(Now)
                Val.Value = 0
                allHistImperi.values.Add(Val)
            End If

            If Driver_ImperiHome.driverDebug Then
                stream = Newtonsoft.Json.JsonConvert.SerializeObject(allHistImperi)
                System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\Drivers\Imperihome\histo.json", stream)
            End If

            Return allHistImperi
        Else
            Return "error"
        End If
    End Function
    Private Function date_to_long(ByVal dt As Date) As Long
        'converti de  date et heure vers temps unix
        Dim origin As New Date(1970, 1, 1)
        Dim span As TimeSpan = dt - origin
        Dim seconds As Double = span.TotalMilliseconds
        Return CType(seconds, Long)
    End Function

    <HttpGet()>
    Public Function GetValue() As Object

        Dim allDev = Driver_ImperiHome.CurrentServer.GetAllDevices(Me.ServerKey)
        Dim allMac = Driver_ImperiHome.CurrentServer.GetAllMacros(Me.ServerKey)

        Dim allDevImperiTemp As DeviceList = Nothing
        Dim fileName = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\devices.json"
        If System.IO.File.Exists(fileName) Then
            Dim stream = System.IO.File.ReadAllText(fileName)
            allDevImperiTemp = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(DeviceList))
        End If

        Dim find As Boolean
        For Each devImperi In allDevImperiTemp.devices
            find = False
            For Each dev In allDev
                If dev.Name = devImperi.name Then
                    devImperi.params = ParamByType(dev, devImperi)
                    find = True
                    Exit For
                End If
            Next
            If Not find Then
                For Each dev In allMac
                    If dev.Nom = devImperi.name Then
                        Dim params As List(Of DeviceParam) = New List(Of DeviceParam)
                        params.Add(New DeviceParam)
                        params(0).key = "LastRun"
                        params(0).value = ""
                        devImperi.params = params
                        Exit For
                    End If
                Next
            End If
        Next
        If Driver_ImperiHome.driverDebug Then
            Dim stream = Newtonsoft.Json.JsonConvert.SerializeObject(allDevImperiTemp)
            System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\Drivers\Imperihome\devices_send.json", stream)
        End If

        Return allDevImperiTemp

    End Function

    Private Function ExecuteCommandWithParams(ByVal id As String, ByVal command As String, ByVal param As String) As Object

        Try
            Dim fileName = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\devices.json"

            If System.IO.File.Exists(fileName) Then
                Dim stream = System.IO.File.ReadAllText(fileName)
                Dim allDevImperi As DeviceList = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(DeviceList))

                Select Case command

                    Case "launchScene"

                        Dim Mac As Macro
                        Dim allMac = Driver_ImperiHome.CurrentServer.GetAllMacros(Me.ServerKey)
                        For Each devImperi In allDevImperi.devices
                            If id = devImperi.id Then
                                For Each Mac1 In allMac
                                    If devImperi.name = Mac1.Nom Then
                                        Mac = Mac1
                                        Driver_ImperiHome.CurrentServer.RunMacro(Me.ServerKey, Mac.ID)
                                        Exit For
                                    End If
                                Next
                                Exit For
                            End If
                        Next
                    Case Else

                        Dim convCommand As Dictionary(Of String, String) = New Dictionary(Of String, String)
                        convCommand.Add("1", "ON")
                        convCommand.Add("0", "OFF")
                        Dim convVolet As Dictionary(Of String, String) = New Dictionary(Of String, String)
                        convVolet.Add("up", "OPEN")
                        convVolet.Add("down", "CLOSE")
                        Dim action As DeviceAction
                        Dim Dev As TemplateDevice
                        Dim allDev = Driver_ImperiHome.CurrentServer.GetAllDevices(Me.ServerKey)
                        For Each devImperi In allDevImperi.devices
                            If id = devImperi.id Then

                                Select Case command
                                    Case "setLevel"
                                        For Each devparam In devImperi.params
                                            If devparam.key = "Level" Then
                                                Dev = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                                If Dev.Type = ListeDevices.VOLET Then
                                                    action = New DeviceAction() With {.Nom = "OUVERTURE"}
                                                Else
                                                    action = New DeviceAction() With {.Nom = "DIM"}
                                                End If
                                                Dim devActionParameter As DeviceAction.Parametre = New DeviceAction.Parametre With {.Nom = "Value", .Value = param}
                                                'devActionParameter = action.Parametres.Where(Function(t) t.Nom = param).FirstOrDefault()
                                                action.Parametres.Add(devActionParameter)
                                                Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev.ID, action)

                                            End If
                                        Next
                                    Case "setMode", "setChoice"

                                        action = New DeviceAction() With {.Nom = "SETVALUE"}
                                        Dim Dev2 As HoMIDom.HoMIDom.TemplateDevice = Nothing
                                        Dim Dev3 As HoMIDom.HoMIDom.TemplateDevice = Nothing
                                        For Each devparam In devImperi.params
                                            If devparam.key = "availablemodes" Then
                                                Dev2 = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                            End If
                                            If devparam.key = "curmode" Then
                                                Dev3 = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                            End If
                                            If devparam.key = "Choices" Then
                                                Dev2 = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                            End If
                                            If devparam.key = "Value" Then
                                                Dev3 = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                            End If
                                        Next
                                        Dim devActionParameter As DeviceAction.Parametre = Nothing
                                        For Each Var In Dev2.VariablesOfDevice
                                            If param = Var.Key Then
                                                devActionParameter = New DeviceAction.Parametre With {.Nom = "Value", .Value = Var.Value}
                                            End If
                                        Next
                                        action.Parametres.Add(devActionParameter)
                                        Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev3.ID, action)

                                    Case "setStatus", "setArmed"

                                        For Each devparam In devImperi.params
                                            If devparam.key = "Status" Or devparam.key = "Armed" Then
                                                Dev = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                                action = New DeviceAction() With {.Nom = convCommand(param)}
                                                Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev.ID, action)
                                            End If
                                        Next

                                    Case "setSetPoint"

                                        For Each devparam In devImperi.params
                                            If devparam.key = "cursetpoint" Then
                                                Dev = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                                action = New DeviceAction() With {.Nom = "SETVALUE"}
                                                Dim devActionParameter As DeviceAction.Parametre = New DeviceAction.Parametre With {.Nom = "Value", .Value = param}
                                                'devActionParameter = action.Parametres.Where(Function(t) t.Nom = param).FirstOrDefault()
                                                action.Parametres.Add(devActionParameter)
                                                Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev.ID, action)
                                            End If
                                        Next

                                    Case "pulseShutter"

                                        For Each devparam In devImperi.params
                                            If devparam.key = "Level" Then
                                                Dev = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                                action = New DeviceAction() With {.Nom = convVolet(param)}
                                                Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev.ID, action)
                                            End If
                                        Next

                                    Case "pulse"

                                        For Each devparam In devImperi.params
                                            If devparam.key = "Status" Then
                                                Dev = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                                action = New DeviceAction() With {.Nom = "TOGGLE"}
                                                Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev.ID, action)
                                            End If
                                        Next

                                    Case "setAck", "stopShutter"

                                                For Each devparam In devImperi.params
                                                    If devparam.key = "Level" Or devparam.key = "Tripped" Then
                                                        Dev = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                                        action = New DeviceAction() With {.Nom = "OFF"}
                                                        Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev.ID, action)
                                                    End If
                                                Next

                                    Case "setColor"

                                        For Each devparam In devImperi.params
                                            If devparam.key = "color" Then
                                                Dev = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, devparam.value)
                                                action = New DeviceAction() With {.Nom = "red"}
                                                Dim devActionParameter As DeviceAction.Parametre = New DeviceAction.Parametre With {.Nom = "Value", .Value = Convert.ToInt32(Mid(param, 3, 2), 16)}
                                                action.Parametres.Add(devActionParameter)
                                                Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev.ID, action)

                                                action = New DeviceAction() With {.Nom = "green"}
                                                devActionParameter = New DeviceAction.Parametre With {.Nom = "Value", .Value = Convert.ToInt32(Mid(param, 5, 2), 16)}
                                                action.Parametres.Add(devActionParameter)
                                                Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev.ID, action)

                                                action = New DeviceAction() With {.Nom = "blue"}
                                                devActionParameter = New DeviceAction.Parametre With {.Nom = "Value", .Value = Convert.ToInt32(Mid(param, 7, 2), 16)}
                                                action.Parametres.Add(devActionParameter)
                                                Driver_ImperiHome.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, Dev.ID, action)
                                            End If
                                        Next
                                    Case Else

                                End Select
                                Exit For
                            End If
                        Next
                End Select
            End If

            fileName = My.Application.Info.DirectoryPath & "\config\Imperihome\action_ret.json"

            If System.IO.File.Exists(fileName) Then
                Dim stream = System.IO.File.ReadAllText(fileName)
                Dim action_ret As action_ret = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(action_ret))
                Return action_ret
            Else
                Return "error"
            End If

        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Function ParamByType(ByVal comp0 As HoMIDom.HoMIDom.TemplateDevice, ByVal dev As Device) As List(Of DeviceParam)
        Try
            Dim params As List(Of DeviceParam) = New List(Of DeviceParam)

            Dim bComp0 As Boolean = True
            Dim Comp As HoMIDom.HoMIDom.TemplateDevice = Nothing

            If dev.params IsNot Nothing Then
                For Each param In dev.params
                    If bComp0 Then
                        Comp = comp0
                        bComp0 = False
                    Else
                        Comp = Driver_ImperiHome.CurrentServer.ReturnDeviceByID(ServerKey, param.value)
                    End If

                    If Comp IsNot Nothing And param.value <> "" Then
                        Dim tempparam As DeviceParam = param
                        If param.value.Length > 1 Then
                            If TypeOf (Comp.Value) Is Boolean Then
                                If Comp.Value = True Then
                                    tempparam.value = "1"
                                Else
                                    tempparam.value = "0"
                                End If
                            Else
                                If param.key = "Status" Then
                                    If Comp.Value > 0 Then
                                        tempparam.value = "1"
                                    Else
                                        tempparam.value = "0"
                                    End If
                                Else
                                    tempparam.value = Comp.Value
                                End If
                            End If
                            If Comp.VariablesOfDevice.Count > 1 Then
                                If (param.key = "availablemodes" Or param.key = "Choices") Then
                                    tempparam.value = ""
                                    Dim i = 0
                                    For Each Var In Comp.VariablesOfDevice
                                        i += 1
                                        tempparam.value &= Var.Key
                                        If i < Comp.VariablesOfDevice.Count Then tempparam.value &= ","
                                    Next
                                End If
                                If param.key = "curmode" Or (param.key = "value" And dev.type = "DevMultiSwitch") Then
                                    For Each Var In Comp.VariablesOfDevice
                                        If tempparam.value = Var.Value Then tempparam.value = Var.Key
                                    Next
                                End If
                            End If

                            If param.unit <> "" Then tempparam.unit = Comp.Unit
                            If param.graphable = True Then tempparam.graphable = Comp.IsHisto
                        End If
                        params.Add(tempparam)
                    End If
                Next
            End If
            For i = 0 To params.Count - 1
                If params(i) IsNot Nothing Then
                    If params(i).value.Contains(","c) Then
                        If (params(i).key <> "availablemodes" And params(i).key <> "curmode" And params(i).key <> "Choices") Then
                            Dim tmpstr(2) As String
                            tmpstr = Split(params(i).value, ",")
                            params(i).value = tmpstr(0) & "." & tmpstr(1)
                        End If
                    End If
                End If
            Next

            Return params

        Catch ex As Exception
            Driver_ImperiHome.CurrentServer.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Driver Imperihome", dev.name & " " & ex.Message)
            Return Nothing
        End Try

    End Function

End Class

''' <summary>Class DeviceParam, Défini le type parametre de composant pour le client Imperihome</summary>
<Serializable()> Public Class DeviceParam

    Public key As String = ""
    Public value As String = ""
    Public unit As String = ""
    Public graphable As Boolean = False

End Class

''' <summary>Class Device, Défini le type composant pour le client Imperihome</summary>
<Serializable()> Public Class Device

    Public id As String = ""
    Public name As String = ""
    Public type As String = ""
    Public room As String = ""
    Public params As List(Of DeviceParam)

End Class

''' <summary>Class DeviceList, Défini le type liste des composants pour le client Imperihome</summary>
<Serializable()> Public Class DeviceList

    Public devices As List(Of Device)

End Class

''' <summary>Class Value, Défini le type valeur pour le client Imperihome</summary>
<Serializable()> Public Class Value

    Public [date] As Long
    Public value As Double

End Class

''' <summary>Class ValueList, Défini le type liste des valeur pour le client Imperihome</summary>
<Serializable()> Public Class ValueList

    Public values As List(Of Value)
    Sub New()
        values = New List(Of Value)
    End Sub

End Class

''' <summary>Class action_ret, Défini le type retour pour le client Imperihome</summary>
<Serializable()> Public Class action_ret

    Public success As Boolean = True
    Public errormsg As String = "ok"

End Class

Public Class RoomsController
    Inherits BaseHoMIdomController

    <HttpGet()>
    Public Function GetValue() As Object
        Try

            Dim fileName = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\rooms.json"

            If System.IO.File.Exists(fileName) Then
                Dim stream = System.IO.File.ReadAllText(fileName)
                Dim allZoneImperi As RoomsList = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(RoomsList))

                Return allZoneImperi
            Else
                Return "Le fichier rooms n'éxiste pas"
            End If

        Catch ex As Exception
            Return "Error"
        End Try

    End Function

End Class

''' <summary>Class Room, Défini le type zone pour le client Imperihome</summary>
<Serializable()> Public Class Room

    Public id As String = ""
    Public name As String = ""

End Class

''' <summary>Class RoomList, Défini le type liste des zones pour le client Imperihome</summary>
<Serializable()> Public Class RoomsList

    Public rooms As List(Of Room)

End Class

Public Class SystemController
    Inherits BaseHoMIdomController

    <HttpGet()>
    Public Function GetValue() As Object
        Try
            Dim fileName = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\system.json"

            If System.IO.File.Exists(fileName) Then
                Dim stream = System.IO.File.ReadAllText(fileName)
                Dim system2 As System1 = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(System1))
                system2.id = ServerKey
                Return system2
            Else
                Return "Le fichier system n'éxiste pas"
            End If

        Catch ex As Exception
            Return "Error"
        End Try

    End Function

End Class

''' <summary>Class DeviceList, Défini le type liste des composants pour le client Imperihome</summary>
<Serializable()> Public Class System1

    Public id As String
    Public apiversion As Integer

End Class

#End Region