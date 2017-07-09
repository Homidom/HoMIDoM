
'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Xml
Imports System.Net
Imports System.Threading
Imports System.Text
Imports System.Text.RegularExpressions
Imports STRGS = Microsoft.VisualBasic.Strings

''' <summary>Driver OpenWeatherMap, le device doit indique sa ville dans son Adresse 1</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_OpenWeatherMap
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "1E169F60-1C54-11E6-A54F-E39A490D5663"
    Dim _Nom As String = "Meteo_OpenWeatherMap"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Meteo provenant de OpenWeatherMap"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "WEB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "OpenWeatherMap"
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
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _AppID As String = ""
    Dim _OrgIcon As Boolean = False

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
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "exception : " & ex.Message)
            WriteLog("ERR: ExecuteCommand, " & ex.Message)
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
                            retour = "Veuillez saisir le nom de la ville(remplacer les espaces par _)"
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
            Try
                _AppID = _Parametres.Item(1).Valeur
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " Lecture du paramètre AppID : " & _AppID)
                WriteLog("Lecture du paramètre AppID : " & _AppID)
            Catch ex As Exception
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " Une erreur est apparue lors de la lecture du paramètre AppID")
                WriteLog("Une erreur est apparue lors de la lecture du paramètre AppID")
            End Try
            Try
                _OrgIcon = _Parametres.Item(2).Valeur
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " Lecture du paramètre OrgIcon : " & _OrgIcon)
                WriteLog("Lecture du paramètre OrgIcon : " & _OrgIcon)
            Catch ex As Exception
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " Une erreur est apparue lors de la lecture du paramètre OrgIcon")
                WriteLog("Une erreur est apparue lors de la lecture du paramètre OrgIcon")
            End Try

            _IsConnect = True
            '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " démarré")
            WriteLog("Driver " & Me.Nom & " démarré")

            If _IsConnect Then
                For Each _Dev As HoMIDom.HoMIDom.TemplateDevice In _Server.GetAllDevices(_IdSrv)
                    If _Dev.Type = ListeDevices.METEO And _Dev.Enable Then
                        Read(_Server.ReturnRealDeviceById(_Dev.ID))
                        Thread.Sleep(2000)
                    End If
                Next
            End If

            If System.IO.File.Exists(Server.GetRepertoireOfServer & "\Fichiers\OpenWeatherMap.xml") = False Then

                Dim xmlWriter As New XmlTextWriter(Server.GetRepertoireOfServer & "\Fichiers\OpenWeatherMap.xml", Encoding.UTF8)

                With xmlWriter
                    .Formatting = Formatting.Indented
                    .WriteStartDocument()
                    .WriteStartElement("Weather")
                    .WriteAttributeString("Version", "1.3.0.2")
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "200")
                    .WriteElementString("WPF_Icon", "thunderstorm")
                    .WriteElementString("Condition_en", "thunderstorm with light rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "201")
                    .WriteElementString("WPF_Icon", "thunderstorm")
                    .WriteElementString("Condition_en", "thunderstorm with rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "202")
                    .WriteElementString("WPF_Icon", "thunderstorm")
                    .WriteElementString("Condition_en", "thunderstorm with heavy rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "210")
                    .WriteElementString("WPF_Icon", "thunderstorm")
                    .WriteElementString("Condition_en", "light thunderstorm")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "211")
                    .WriteElementString("WPF_Icon", "thunderstorm")
                    .WriteElementString("Condition_en", "thunderstorm")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "212")
                    .WriteElementString("WPF_Icon", "thunderstorm")
                    .WriteElementString("Condition_en", "heavy thunderstorm")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "221")
                    .WriteElementString("WPF_Icon", "thunderstorm")
                    .WriteElementString("Condition_en", "ragged thunderstorm")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "230")
                    .WriteElementString("WPF_Icon", "17")
                    .WriteElementString("Condition_en", "thunderstorm with light drizzle")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "231")
                    .WriteElementString("WPF_Icon", "17")
                    .WriteElementString("Condition_en", "thunderstorm with drizzle")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "232")
                    .WriteElementString("WPF_Icon", "17")
                    .WriteElementString("Condition_en", "thunderstorm with heavy drizzle")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "300")
                    .WriteElementString("WPF_Icon", "sleet")
                    .WriteElementString("Condition_en", "light intensity drizzle")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "301")
                    .WriteElementString("WPF_Icon", "sleet")
                    .WriteElementString("Condition_en", "drizzle")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "302")
                    .WriteElementString("WPF_Icon", "sleet")
                    .WriteElementString("Condition_en", "heavy intensity drizzle")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "310")
                    .WriteElementString("WPF_Icon", "sleet")
                    .WriteElementString("Condition_en", "light intensity drizzle rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "311")
                    .WriteElementString("WPF_Icon", "sleet")
                    .WriteElementString("Condition_en", "drizzle rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "312")
                    .WriteElementString("WPF_Icon", "sleet")
                    .WriteElementString("Condition_en", "heavy intensity drizzle rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "313")
                    .WriteElementString("WPF_Icon", "sleet")
                    .WriteElementString("Condition_en", "shower rain and drizzle")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "314")
                    .WriteElementString("WPF_Icon", "sleet")
                    .WriteElementString("Condition_en", "heavy shower rain and drizzle")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "321")
                    .WriteElementString("WPF_Icon", "sleet")
                    .WriteElementString("Condition_en", "shower drizzle")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "500")
                    .WriteElementString("WPF_Icon", "11")
                    .WriteElementString("Condition_en", "light rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "501")
                    .WriteElementString("WPF_Icon", "11")
                    .WriteElementString("Condition_en", "moderate rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "502")
                    .WriteElementString("WPF_Icon", "rain")
                    .WriteElementString("Condition_en", "heavy intensity rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "503")
                    .WriteElementString("WPF_Icon", "rain")
                    .WriteElementString("Condition_en", "very heavy rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "504")
                    .WriteElementString("WPF_Icon", "storm")
                    .WriteElementString("Condition_en", "extreme rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "511")
                    .WriteElementString("WPF_Icon", "14")
                    .WriteElementString("Condition_en", "freezing rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "520")
                    .WriteElementString("WPF_Icon", "11")
                    .WriteElementString("Condition_en", "light intensity shower rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "521")
                    .WriteElementString("WPF_Icon", "11")
                    .WriteElementString("Condition_en", "shower rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "522")
                    .WriteElementString("WPF_Icon", "11")
                    .WriteElementString("Condition_en", "heavy intensity shower rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "531")
                    .WriteElementString("WPF_Icon", "11")
                    .WriteElementString("Condition_en", "ragged shower rain")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "600")
                    .WriteElementString("WPF_Icon", "14")
                    .WriteElementString("Condition_en", "light snow")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "601")
                    .WriteElementString("WPF_Icon", "13")
                    .WriteElementString("Condition_en", "snow")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "602")
                    .WriteElementString("WPF_Icon", "14")
                    .WriteElementString("Condition_en", "heavy snow")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "611")
                    .WriteElementString("WPF_Icon", "snow")
                    .WriteElementString("Condition_en", "sleet")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "612")
                    .WriteElementString("WPF_Icon", "14")
                    .WriteElementString("Condition_en", "shower sleet")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "615")
                    .WriteElementString("WPF_Icon", "10")
                    .WriteElementString("Condition_en", "light rain and snow")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "616")
                    .WriteElementString("WPF_Icon", "07")
                    .WriteElementString("Condition_en", "rain and snow")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "620")
                    .WriteElementString("WPF_Icon", "14")
                    .WriteElementString("Condition_en", "light shower snow")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "621")
                    .WriteElementString("WPF_Icon", "14")
                    .WriteElementString("Condition_en", "shower snow")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "622")
                    .WriteElementString("WPF_Icon", "snow")
                    .WriteElementString("Condition_en", "heavy shower snow")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "701")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "mist")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "711")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "smoke")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "721")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "haze")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "731")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "sand, dust whirls")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "741")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "fog")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "751")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "sand")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "761")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "dust")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "762")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "volcanic ash")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "771")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "squalls")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "781")
                    .WriteElementString("WPF_Icon", "19")
                    .WriteElementString("Condition_en", "tornado")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "800")
                    .WriteElementString("WPF_Icon", "sunny")
                    .WriteElementString("Condition_en", "clear sky")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "801")
                    .WriteElementString("WPF_Icon", "partly_cloudy")
                    .WriteElementString("Condition_en", "few clouds")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "802")
                    .WriteElementString("WPF_Icon", "mostly_cloudy")
                    .WriteElementString("Condition_en", "scattered clouds")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "803")
                    .WriteElementString("WPF_Icon", "26")
                    .WriteElementString("Condition_en", "broken clouds")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "804")
                    .WriteElementString("WPF_Icon", "cloudy")
                    .WriteElementString("Condition_en", "overcast clouds")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "900")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "tornado")
                    .WriteElementString("Condition_fr", "tornade")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "901")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "tropical storm")
                    .WriteElementString("Condition_fr", "tempête tropicale")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "902")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "hurricane")
                    .WriteElementString("Condition_fr", "ouragan")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "903")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "cold")
                    .WriteElementString("Condition_fr", "froid")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "904")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "hot")
                    .WriteElementString("Condition_fr", "chaud")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "905")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "windy")
                    .WriteElementString("Condition_fr", "venteux")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "906")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "hail")
                    .WriteElementString("Condition_fr", "hail")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "951")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "calm")
                    .WriteElementString("Condition_fr", "calme")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "952")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "light breeze")
                    .WriteElementString("Condition_fr", "légère brise")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "953")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "gentle breeze")
                    .WriteElementString("Condition_fr", "petite brise")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "954")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "moderate breeze")
                    .WriteElementString("Condition_fr", "brise modérée")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "955")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "fresh breeze")
                    .WriteElementString("Condition_fr", "brise")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "956")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "strong breeze")
                    .WriteElementString("Condition_fr", "forte brise")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "957")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "high wind, near gale")
                    .WriteElementString("Condition_fr", "vent fort, proche tempête")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "958")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "gale")
                    .WriteElementString("Condition_fr", "tempête")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "959")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "severe gale")
                    .WriteElementString("Condition_fr", "tempête sévère")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "960")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "storm")
                    .WriteElementString("Condition_fr", "orage")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "961")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "violent storm")
                    .WriteElementString("Condition_fr", "orage violent")
                    .WriteEndElement()
                    .WriteStartElement("label")
                    .WriteElementString("Driver", "962")
                    .WriteElementString("WPF_Icon", "")
                    .WriteElementString("Condition_en", "hurricane")
                    .WriteElementString("Condition_fr", "ouragan")
                    .WriteEndElement()
                    .WriteEndElement()
                    .Close()
                End With
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Fichier OpenWeatherMap.xml a été créé !")
                WriteLog("Fichier OpenWeatherMap.xml a été créé !")
            End If

        Catch ex As Exception
            _IsConnect = False
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", ex.Message)
            WriteLog("ERR: Start, " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", ex.Message)
            WriteLog("ERR: Stop, " & ex.Message)
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
                '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Seul les composants de type meteo peuvent etre mis à jour. Les composants dépendants sont mis à jour automatiquement en même temps que la météo.")
                WriteLog("ERR: Read, Seul les composants de type meteo peuvent etre mis à jour. Les composants dépendants sont mis à jour automatiquement en même temps que la météo.")
            End If

        Catch ex As Exception
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", ex.Message)
            WriteLog("ERR: Read, " & ex.Message)
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
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", ex.Message)
            WriteLog("ERR: Write, " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " DeleteDevice", ex.Message)
            WriteLog("ERR: DeleteDevice, " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " NewDevice", ex.Message)
            WriteLog("ERR: NewDevice, " & ex.Message)
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
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
            WriteLog("ERR: Add_DeviceCommande, " & ex.Message)
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
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
            WriteLog("ERR: Add_LibelleDriver, " & ex.Message)
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
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
            WriteLog("ERR: Add_LibelleDevice, " & ex.Message)
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
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
            WriteLog("ERR: Add_ParamAvance, " & ex.Message)
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
            Add_ParamAvance("AppID", "ID du Compte OpenWeatherMap.org", "")
            Add_ParamAvance("OrgIcon", "Utiliser les icones météo d'OpenWeatherMap", False)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Ville", "Ville dans OpenWeatherMap.org", "")
            Add_LibelleDevice("ADRESSE2", "@", "", "")
            Add_LibelleDevice("MODELE", "@", "", "")
            Add_LibelleDevice("SOLO", "@", "")
            'Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
        Catch ex As Exception
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
            WriteLog("ERR: New, " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        Try
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
            WriteLog("ERR: TimerTick, " & ex.ToString)
        End Try
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
            Dim url As New Uri("http://api.openweathermap.org/data/2.5/find?q=" & _Obj.adresse1 & "&APPID=" & _AppID & "&units=metric&lang=fr&mode=xml")
            Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
            ' Request.UserAgent = "Mozilla/5.0 (windows; U; windows NT 5.1; fr; rv:1.8.0.7) Gecko/20060909 Firefox/1.5.0.7"
            Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)

            doc.Load(response.GetResponseStream)

            nodes = doc.SelectNodes("/cities/list/item")

            For Each node As XmlNode In nodes
                If node.HasChildNodes = True Then
                    For Each _child As XmlNode In node
                        Select Case _child.Name
                            Case "temperature"
                                '_Obj.TemperatureActuel = Int(Replace(_child.Attributes.GetNamedItem("value").Value, ".", ","))
                                _Obj.TemperatureActuel = Regex.Replace(_child.Attributes.GetNamedItem("value").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                '_StrTemp = Int(Replace(_child.Attributes.GetNamedItem("value").Value, ".", ","))
                                _StrTemp = Regex.Replace(_child.Attributes.GetNamedItem("value").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Lecture du paramètre TEMPERATURE : " & _StrTemp)
                                WriteLog("DBG: MAJ, Lecture du paramètre TEMPERATURE : " & _StrTemp)
                            Case "weather"
                                _Obj.ConditionActuel = _child.Attributes.GetNamedItem("value").Value
                                '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Lecture du paramètre ConditionActuel : " & _Obj.ConditionActuel)
                                WriteLog("DBG: MAJ, Lecture du paramètre ConditionActuel : " & _Obj.ConditionActuel)
                                If _OrgIcon = True Then
                                    _Obj.IconActuel = _child.Attributes.GetNamedItem("icon").Value
                                Else
                                    _Obj.IconActuel = GetIcon(_child.Attributes.GetNamedItem("number").Value)
                                End If
                                DonwloadIcon(_child.Attributes.GetNamedItem("icon").Value)
                                '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Lecture du paramètre IconActuel : " & _Obj.IconActuel)
                                WriteLog("DBG: MAJ, Lecture du paramètre IconActuel : " & _Obj.IconActuel)
                            Case "humidity"
                                '  _Obj.HumiditeActuel = Int(Replace(_child.Attributes.GetNamedItem("value").Value, ".", ","))
                                _Obj.HumiditeActuel = Regex.Replace(_child.Attributes.GetNamedItem("value").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                '_StrHum = Int(Replace(_child.Attributes.GetNamedItem("value").Value, ".", ","))
                                _StrHum = Regex.Replace(_child.Attributes.GetNamedItem("value").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Lecture du paramètre HUMIDITE : " & _StrHum)
                                WriteLog("DBG: MAJ, Lecture du paramètre HUMIDITE : " & _StrHum)
                            Case "pressure"
                                _StrBar = _child.Attributes.GetNamedItem("value").Value
                                ' _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Lecture du paramètre BAROMETRE : " & _StrBar)
                                WriteLog("DBG: MAJ, Lecture du paramètre BAROMETRE : " & _StrBar)
                            Case "wind"
                                For Each _child2 As XmlNode In _child
                                    Select Case _child2.Name
                                        Case "speed"
                                            If _Obj IsNot Nothing Then
                                                '_Obj.VentActuel = Int(Replace(_child2.Attributes.GetNamedItem("value").Value, ".", ",") * 3600 / 1000)
                                                _Obj.VentActuel = (Regex.Replace(_child2.Attributes.GetNamedItem("value").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) * 3600 / 1000)
                                                ' _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Lecture du paramètre VentActuel : " & _Obj.VentActuel)
                                                WriteLog("DBG: MAJ, Lecture du paramètre VentActuel : " & _Obj.VentActuel)
                                            End If
                                            '_StrVitVent = Int(Replace(_child2.Attributes.GetNamedItem("value").Value, ".", ",") * 3600 / 1000)
                                            _StrVitVent = (Regex.Replace(_child2.Attributes.GetNamedItem("value").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) * 3600 / 1000)
                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Lecture du paramètre VITESSEVENT : " & _StrVitVent)
                                            WriteLog("DBG: MAJ, Lecture du paramètre VITESSEVENT : " & _StrVitVent)
                                        Case "direction"
                                            _StrDirVent = Replace(_child2.Attributes.GetNamedItem("code").Value, "W", "O")
                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Lecture du paramètre DIRECTIONVENT : " & _StrDirVent)
                                            WriteLog("DBG: MAJ, Lecture du paramètre DIRECTIONVENT : " & _StrDirVent)
                                    End Select
                                Next
                        End Select
                    Next
                End If
            Next

            ' Create a new XmlDocument   
            doc = New XmlDocument()
            Dim url2 As New Uri("http://api.openweathermap.org/data/2.5/forecast/daily?q=" & _Obj.adresse1 & "&APPID=" & _AppID & "&units=metric&lang=fr&mode=xml&cnt=4")
            Dim Request2 As HttpWebRequest = CType(HttpWebRequest.Create(url2), System.Net.HttpWebRequest)
            ' Request.UserAgent = "Mozilla/5.0 (windows; U; windows NT 5.1; fr; rv:1.8.0.7) Gecko/20060909 Firefox/1.5.0.7"
            Dim response2 As Net.HttpWebResponse = CType(Request2.GetResponse(), Net.HttpWebResponse)

            doc.Load(response2.GetResponseStream)

            nodes = doc.SelectNodes("/weatherdata/forecast")

            Dim idx As Integer = -1

            For Each node As XmlNode In nodes
                If node.HasChildNodes = True Then
                    For Each _child As XmlNode In node
                        If _child.HasChildNodes Then
                            idx = idx + 1

                            Select Case idx
                                Case 0 : If _Obj IsNot Nothing Then _Obj.JourToday = TraduireJour(Mid(Now.DayOfWeek.ToString, 1, 3))
                                Case 1 : If _Obj IsNot Nothing Then _Obj.JourJ1 = TraduireJour(Mid(Now.AddDays(1).DayOfWeek.ToString, 1, 3))
                                Case 2 : If _Obj IsNot Nothing Then _Obj.JourJ2 = TraduireJour(Mid(Now.AddDays(2).DayOfWeek.ToString, 1, 3))
                                Case 3 : If _Obj IsNot Nothing Then _Obj.JourJ3 = TraduireJour(Mid(Now.AddDays(3).DayOfWeek.ToString, 1, 3))
                            End Select

                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Idx = " & idx & " Time : " & _child.Attributes.GetNamedItem("day").Value)
                            WriteLog("DBG: MAJ, Idx = " & idx & " Time : " & _child.Attributes.GetNamedItem("day").Value)
                            Select Case _child.Name
                                Case "time"
                                    For Each _child2 As XmlNode In _child
                                        Select Case _child2.Name
                                            Case "temperature"
                                                Select Case idx
                                                    Case 0
                                                        If _Obj IsNot Nothing Then
                                                            '_Obj.MaxToday = Int(Replace(_child2.Attributes.GetNamedItem("max").Value, ".", ","))
                                                            _Obj.MaxToday = Regex.Replace(_child2.Attributes.GetNamedItem("max").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                            '_Obj.MinToday = Int(Replace(_child2.Attributes.GetNamedItem("min").Value, ".", ","))
                                                            _Obj.MinToday = Regex.Replace(_child2.Attributes.GetNamedItem("min").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temperature Max : " & _child2.Attributes.GetNamedItem("max").Value)
                                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temperature Min : " & _child2.Attributes.GetNamedItem("min").Value)
                                                            WriteLog("DBG: MAJ, Temperature Max : " & _child2.Attributes.GetNamedItem("max").Value)
                                                            WriteLog("DBG: MAJ, Temperature Min : " & _child2.Attributes.GetNamedItem("min").Value)
                                                        End If
                                                    Case 1
                                                        If _Obj IsNot Nothing Then
                                                            '_Obj.MaxJ1 = Int(Replace(_child2.Attributes.GetNamedItem("max").Value, ".", ","))
                                                            _Obj.MaxJ1 = Regex.Replace(_child2.Attributes.GetNamedItem("max").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                            '_Obj.MinJ1 = Int(Replace(_child2.Attributes.GetNamedItem("min").Value, ".", ","))
                                                            _Obj.MinJ1 = Regex.Replace(_child2.Attributes.GetNamedItem("min").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temperature Max : " & _child2.Attributes.GetNamedItem("max").Value)
                                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temperature Min : " & _child2.Attributes.GetNamedItem("min").Value)
                                                            WriteLog("DBG: MAJ, Temperature Max : " & _child2.Attributes.GetNamedItem("max").Value)
                                                            WriteLog("DBG: MAJ, Temperature Min : " & _child2.Attributes.GetNamedItem("min").Value)
                                                        End If
                                                    Case 2
                                                        If _Obj IsNot Nothing Then
                                                            '_Obj.MaxJ2 = Int(Replace(_child2.Attributes.GetNamedItem("max").Value, ".", ","))
                                                            _Obj.MaxJ2 = Regex.Replace(_child2.Attributes.GetNamedItem("max").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                            '_Obj.MinJ2 = Int(Replace(_child2.Attributes.GetNamedItem("min").Value, ".", ","))
                                                            _Obj.MinJ2 = Regex.Replace(_child2.Attributes.GetNamedItem("min").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temperature Max : " & _child2.Attributes.GetNamedItem("max").Value)
                                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temperature Min : " & _child2.Attributes.GetNamedItem("min").Value)
                                                            WriteLog("DBG: MAJ, Temperature Max : " & _child2.Attributes.GetNamedItem("max").Value)
                                                            WriteLog("DBG: MAJ, Temperature Min : " & _child2.Attributes.GetNamedItem("min").Value)
                                                        End If
                                                    Case 3
                                                        If _Obj IsNot Nothing Then
                                                            '_Obj.MaxJ3 = Int(Replace(_child2.Attributes.GetNamedItem("max").Value, ".", ","))
                                                            _Obj.MaxJ3 = Regex.Replace(_child2.Attributes.GetNamedItem("max").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                            '_Obj.MinJ3 = Int(Replace(_child2.Attributes.GetNamedItem("min").Value, ".", ","))
                                                            _Obj.MinJ3 = Regex.Replace(_child2.Attributes.GetNamedItem("min").Value, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                            ' _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temperature Max : " & _child2.Attributes.GetNamedItem("max").Value)
                                                            '  _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temperature Min : " & _child2.Attributes.GetNamedItem("min").Value)
                                                            WriteLog("DBG: MAJ, Temperature Max : " & _child2.Attributes.GetNamedItem("max").Value)
                                                            WriteLog("DBG: MAJ, Temperature Min : " & _child2.Attributes.GetNamedItem("min").Value)
                                                        End If
                                                End Select
                                            Case "symbol"
                                                'My.Computer.Network.DownloadFile("http://www.cohowinery.com/downloads/WineList.txt","C:\Documents and Settings\All Users\Documents\WineList.txt")
                                                'If My.Computer.FileSystem.FileExists("c://Check.txt") Then
                                                'MsgBox("File found.")
                                                'Else
                                                'MsgBox("File not found.")
                                                'End If

                                                Select Case idx
                                                    Case 0
                                                        If _Obj IsNot Nothing Then
                                                            _Obj.ConditionToday = _child2.Attributes.GetNamedItem("name").Value
                                                            If _OrgIcon = True Then
                                                                _Obj.IconToday = _child2.Attributes.GetNamedItem("var").Value
                                                            Else
                                                                _Obj.IconToday = GetIcon(_child2.Attributes.GetNamedItem("number").Value)
                                                            End If
                                                            DonwloadIcon(_child2.Attributes.GetNamedItem("var").Value)
                                                            '  _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Symbol : " & _child2.Attributes.GetNamedItem("var").Value)
                                                            '  _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temp : " & _child2.Attributes.GetNamedItem("name").Value)
                                                            WriteLog("DBG: MAJ, Symbol : " & _child2.Attributes.GetNamedItem("var").Value)
                                                            WriteLog("DBG: MAJ, Temp : " & _child2.Attributes.GetNamedItem("name").Value)
                                                        End If
                                                    Case 1
                                                        If _Obj IsNot Nothing Then
                                                            _Obj.ConditionJ1 = _child2.Attributes.GetNamedItem("name").Value
                                                            If _OrgIcon = True Then
                                                                _Obj.IconJ1 = _child2.Attributes.GetNamedItem("var").Value
                                                            Else
                                                                _Obj.IconJ1 = GetIcon(_child2.Attributes.GetNamedItem("number").Value)
                                                            End If
                                                            DonwloadIcon(_child2.Attributes.GetNamedItem("var").Value)
                                                            '  _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Symbol : " & _child2.Attributes.GetNamedItem("var").Value)
                                                            '   _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temp : " & _child2.Attributes.GetNamedItem("name").Value)
                                                            WriteLog("DBG: MAJ, Symbol : " & _child2.Attributes.GetNamedItem("var").Value)
                                                            WriteLog("DBG: MAJ, Temp : " & _child2.Attributes.GetNamedItem("name").Value)
                                                        End If
                                                    Case 2
                                                        If _Obj IsNot Nothing Then
                                                            _Obj.ConditionJ2 = _child2.Attributes.GetNamedItem("name").Value
                                                            If _OrgIcon = True Then
                                                                _Obj.IconJ2 = _child2.Attributes.GetNamedItem("var").Value
                                                            Else
                                                                _Obj.IconJ2 = GetIcon(_child2.Attributes.GetNamedItem("number").Value)
                                                            End If
                                                            DonwloadIcon(_child2.Attributes.GetNamedItem("var").Value)
                                                            '  _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Symbol : " & _child2.Attributes.GetNamedItem("var").Value)
                                                            '  _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temp : " & _child2.Attributes.GetNamedItem("name").Value)
                                                            WriteLog("DBG: MAJ, Symbol : " & _child2.Attributes.GetNamedItem("var").Value)
                                                            WriteLog("DBG: MAJ, Temp : " & _child2.Attributes.GetNamedItem("name").Value)
                                                        End If
                                                    Case 3
                                                        If _Obj IsNot Nothing Then
                                                            _Obj.ConditionJ3 = _child2.Attributes.GetNamedItem("name").Value
                                                            If _OrgIcon = True Then
                                                                _Obj.IconJ3 = _child2.Attributes.GetNamedItem("var").Value
                                                            Else
                                                                _Obj.IconJ3 = GetIcon(_child2.Attributes.GetNamedItem("number").Value)
                                                            End If
                                                            DonwloadIcon(_child2.Attributes.GetNamedItem("var").Value)
                                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Symbol : " & _child2.Attributes.GetNamedItem("var").Value)
                                                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Temp : " & _child2.Attributes.GetNamedItem("name").Value)
                                                            WriteLog("DBG: MAJ, Symbol : " & _child2.Attributes.GetNamedItem("var").Value)
                                                            WriteLog("DBG: MAJ, Temp : " & _child2.Attributes.GetNamedItem("name").Value)
                                                        End If
                                                End Select
                                        End Select
                                    Next
                            End Select

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

            'If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "MAJ Meteo effectuée pour " & _Obj.name)
            WriteLog("DBG: MAJ, Meteo effectuée pour " & _Obj.name)
        Catch ex As Exception
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Erreur Lors de la MaJ de " & _Obj.name & " : " & ex.ToString)
            WriteLog("ERR: MAJ, Erreur Lors de la MaJ de " & _Obj.name & " : " & ex.ToString)
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
            WriteLog("ERR: ExtractFile, " & ex.ToString)
            Return Nothing
        End Try
    End Function

    Private Function TraduireJour(ByVal Jour As String) As String
        Try
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
        Catch ex As Exception
            WriteLog("ERR: TraduireJour, " & ex.ToString)
            Return "?"
        End Try
    End Function

    Private Function Traduire(ByVal txt As String) As String
        Try
            Return Traduct(txt)
        Catch ex As Exception
            ' _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Traduire: " & ex.Message)
            WriteLog("ERR: Traduire, " & ex.ToString)
            Return txt
        End Try
    End Function

    Public Function Traduct(Txt As String) As String
        Try
            Dim _txt As String = Trim(Txt).ToLower.Replace("  ", " ")
            Dim _return As String = ""
            Dim xReader As XmlReader
            Dim xDoc As XElement

            If System.IO.File.Exists(Server.GetRepertoireOfServer & "\Fichiers\OpenWeatherMap.xml") = True Then
                xReader = XmlReader.Create(Server.GetRepertoireOfServer & "\Fichiers\OpenWeatherMap.xml")
                xDoc = XElement.Load(xReader)
                xReader.Close()

                Dim employees As IEnumerable(Of XElement) = xDoc.Elements()

                For Each employee In employees
                    If employee.Element("Condition_en").Value.ToLower = _txt Then
                        _return = employee.Element("Condition_fr").Value
                        Exit For
                    End If
                Next employee
                Return _return
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Fichier OpenWeatherMap.xml introuvable !")
                WriteLog("ERR: Traduct, Fichier OpenWeatherMap.xml introuvable !")
                Return ""
            End If
        Catch ex As Exception
            '     _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Erreur Traduct: " & ex.ToString)
            WriteLog("ERR: Traduct, " & ex.ToString)
            Return ""
        End Try
    End Function

    Public Function GetIcon(Txt As String) As String
        Try
            Dim _txt As String = Trim(Txt).ToLower.Replace("  ", " ")
            Dim _return As String = ""
            Dim xReader As XmlReader
            Dim xDoc As XElement
            If System.IO.File.Exists(Server.GetRepertoireOfServer & "\Fichiers\OpenWeatherMap.xml") = True Then
                xReader = XmlReader.Create(Server.GetRepertoireOfServer & "\Fichiers\OpenWeatherMap.xml")
                xDoc = XElement.Load(xReader)
                xReader.Close()

                Dim employees As IEnumerable(Of XElement) = xDoc.Elements()
                For Each employee In employees
                    If employee.Element("Driver").Value.ToLower = _txt Then
                        _return = employee.Element("WPF_Icon").Value
                        Exit For
                    End If
                Next employee
                Return _return
            Else
                '                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Fichier OpenWeatherMap.xml introuvable !")
                WriteLog("ERR: GetIcon, Fichier OpenWeatherMap.xml introuvable !")
                Return ""
            End If

        Catch ex As Exception
            '           _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "METEO_OpenWeatherMap", "Erreur GetIcon : " & ex.ToString)
            WriteLog("ERR: GetIcon, " & ex.ToString)
            Return ""
        End Try
    End Function

    Public Function DonwloadIcon(Icon As String) As String
        Try
            If Not System.IO.File.Exists(Server.GetRepertoireOfServer & "\Images\Meteo\" & Icon & ".png") Then
                My.Computer.Network.DownloadFile("http://openweathermap.org/img/w/" & Icon & ".png", Server.GetRepertoireOfServer & "\Images\Meteo\" & Icon & ".png")
                WriteLog("Téléchargement de http://openweathermap.org/img/w/" & Icon & ".png dans " & Server.GetRepertoireOfServer & "\Images\Meteo\" & Icon & ".png")
            End If
        Catch ex As Exception
            WriteLog("ERR: DonwloadIcon, Téléchargement de http://openweathermap.org/img/w/" & Icon & ".png dans " & Server.GetRepertoireOfServer & "\Images\Meteo\" & Icon & ".png")
        End Try
        Return ""
    End Function

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
