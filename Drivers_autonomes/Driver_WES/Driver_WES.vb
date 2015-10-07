Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Xml.Serialization

Imports System.Text.RegularExpressions
Imports STRGS = Microsoft.VisualBasic.Strings
Imports HoMIDom.HoMIDom
Imports System.Net
Imports System.IO

' Auteur : DomoMath sur une base HoMIDoM
' Date : 01/10/2015

''' <summary>Driver WES Reception des datas base du serveur WES Cartelectronic.fr</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_WES
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "92CBC2D9-B072-11E4-A32C-93901D5D46B0"
    Dim _Nom As String = "WES"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Données station météo WES"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "XML"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "WES"
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

    Dim data1 As data
    Dim form1 As form

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim First As Boolean = True
    Dim tabAdresse As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)

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
            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur

            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            data1 = New data

            If ScanData() Then


                If _Refresh > 0 Then
                    If _Refresh < 60 Then
                        _Refresh = 60
                    End If

                    MyTimer.Interval = _Refresh * 1000
                    MyTimer.Enabled = True
                    AddHandler MyTimer.Elapsed, AddressOf TimerTick
                End If

            Else
                _IsConnect = False
                WriteLog("Driver " & Me.Nom & " non démarré")
                Exit Sub
            End If

            _IsConnect = True
            WriteLog("Driver " & Me.Nom & " démarré")

        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: START Driver " & Me.Nom & " Erreur démarrage " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            WriteLog("Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            WriteLog("ERR: STOP Driver " & Me.Nom & " Erreur arrêt " & ex.Message)
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

            'récupération des paramétres avancés pour rafraichir l'affichage
            Try
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            If data1 IsNot Nothing Then GetData(Objet)

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
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
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

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.COMPTEUR)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE)
            _DeviceSupport.Add(ListeDevices.ENERGIETOTALE)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE)
            _DeviceSupport.Add(ListeDevices.UV)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
            _DeviceSupport.Add(ListeDevices.LAMPE)
            _DeviceSupport.Add(ListeDevices.SWITCH)
            _DeviceSupport.Add(ListeDevices.CONTACT)
            _DeviceSupport.Add(ListeDevices.APPAREIL)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device

            Add_LibelleDevice("ADRESSE1", "Info à recupérer", "Info à recupérer", _
                                   "ADCO" & "|" & _
                                   "OPTARIF" & "|" & _
                                   "ISOUSC" & "|" & _
                                   "PTEC" & "|" & _
                                   "PAP" & "|" & _
                                   "IINST" & "|" & _
                                   "IINST1" & "|" & _
                                   "IINST2" & "|" & _
                                   "IINST3" & "|" & _
                                   "IMAX" & "|" & _
                                   "IMAX1" & "|" & _
                                   "IMAX2" & "|" & _
                                   "IMAX3" & "|" & _
                                   "PEJP" & "|" & _
                                   "DEMAIN" & "|" & _
                                   "BASE" & "|" & _
                                   "HCHC" & "|" & _
                                   "HCHP" & "|" & _
                                   "EJPHN" & "|" & _
                                   "EJPHPM" & "|" & _
                                   "BBRHCJB" & "|" & _
                                   "BBRHPJB" & "|" & _
                                   "BBRHCJW" & "|" & _
                                   "BBRHPJW" & "|" & _
                                   "BBRHCJR" & "|" & _
                                   "BBRHPJR" & "|" & _
                                   "PINCE" & "|" & _
                                   "INDEX PINCE" & "|" & _
                                   "PULSE" & "|" & _
                                   "INDEX PULSE" & "|" & _
                                   "SONDE" & "|" & _
                                   "RELAIS" & "|" & _
                                   "ENTREE DIGITAL" & "|" & _
                                   "ENTREE ANALOGIQUE" & "|" & _
                                   "VIRTUEL")

            Add_LibelleDevice("ADRESSE2", "Voie à recupérer", "Adresse de la voie à recupérer", "")
            Add_LibelleDevice("REFRESH", "Refresh en sec", "Valeur rafraicissement du composant", "60")
            ' Libellés Device inutiles
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "@", "")

            tabAdresse.Add(1, "http://WES/tic1.cgx")
            tabAdresse.Add(2, "http://WES/tic2.cgx")
            tabAdresse.Add(3, "http://WES/temp.cgx")
            tabAdresse.Add(4, "http://WES/pince.cgx")
            tabAdresse.Add(5, "http://WES/pulse.cgx")


        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        ' Attente de 3s pour eviter le relancement de la procedure dans le laps de temps
        'System.Threading.Thread.Sleep(3000)
        ScanData()
    End Sub

#End Region

#Region "Fonctions internes"

    Private Function ScanData() As Boolean

        Try
            Dim datatemp As data
            Dim request As WebRequest
            Dim response As WebResponse
            For Each adress In tabAdresse

                request = WebRequest.Create(adress.Value)
                request.Credentials = New System.Net.NetworkCredential("admin", "wes")
                response = request.GetResponse()

                If CType(response, HttpWebResponse).StatusCode = HttpStatusCode.OK Then
                    ' Get the stream containing content returned by the server.
                    Dim dataStream As Stream = response.GetResponseStream()
                    ' Open the stream using a StreamReader.
                    Dim reader As New StreamReader(dataStream)

                    Dim serializer As New XmlSerializer(GetType(data))
                    datatemp = CType(serializer.Deserialize(reader), data)

                    Select Case adress.Key
                        Case 1
                            data1.tic1 = datatemp.tic1
                        Case 2
                            data1.tic2 = datatemp.tic2
                        Case 3
                            data1.temp = datatemp.temp
                        Case 4
                            data1.pince = datatemp.pince
                        Case 5
                            data1.impulsion = datatemp.impulsion
                    End Select
                End If
            Next

            request = WebRequest.Create("http://WES/inout.cgx")
            request.Credentials = New System.Net.NetworkCredential("admin", "wes")
            response = request.GetResponse()

            If CType(response, HttpWebResponse).StatusCode = HttpStatusCode.OK Then
                ' Get the stream containing content returned by the server.
                Dim dataStream As Stream = response.GetResponseStream()
                ' Open the stream using a StreamReader.
                Dim reader As New StreamReader(dataStream)

                Dim serializer As New XmlSerializer(GetType(form))
                form1 = CType(serializer.Deserialize(reader), form)
            End If

            Dim stream = Newtonsoft.Json.JsonConvert.SerializeObject(form1)
            stream &= Newtonsoft.Json.JsonConvert.SerializeObject(data1)
            Dim text = stream.Replace("}", vbCrLf)
            WriteLog("DBG: ScanData : " & text)

            Return True

        Catch ex As Exception
            WriteLog("ERR: ScanData, Exception : " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub GetData(ByVal objet As Object)

        Try
            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                Exit Sub
            End If
            Select UCase(objet.adresse2)
                Case "1"
                    Select Case UCase(objet.adresse1)
                        Case "ADCO"
                            objet.value = data1.tic1.ADCO
                        Case "BASE"
                            objet.value = data1.tic1.BASE
                        Case "BBRHCJB"
                            objet.value = data1.tic1.BBRHCJB
                        Case "BBRHCJR"
                            objet.value = data1.tic1.BBRHCJR
                        Case "BBRHCJW"
                            objet.value = data1.tic1.BBRHCJW
                        Case "BBRHPJB"
                            objet.value = data1.tic1.BBRHPJB
                        Case "BBRHPJR"
                            objet.value = data1.tic1.BBRHPJR
                        Case "BBRHPJW"
                            objet.value = data1.tic1.BBRHPJW
                        Case "DEMAIN"
                            objet.value = data1.tic1.DEMAIN
                        Case "EJPHN"
                            objet.value = data1.tic1.EJPHN
                        Case "EJPHPM"
                            objet.value = data1.tic1.EJPHPM
                        Case "HCHC"
                            objet.value = data1.tic1.HCHC
                        Case "HCHP"
                            objet.value = data1.tic1.HCHP
                        Case "IINST"
                            objet.value = data1.tic1.IINST
                        Case "IINST1"
                            objet.value = data1.tic1.IINST1
                        Case "IINST2"
                            objet.value = data1.tic1.IINST2
                        Case "IINST3"
                            objet.value = data1.tic1.IINST3
                        Case "IMAX"
                            objet.value = data1.tic1.IMAX
                        Case "IMAX1"
                            objet.value = data1.tic1.IMAX1
                        Case "IMAX2"
                            objet.value = data1.tic1.IMAX2
                        Case "IMAX3"
                            objet.value = data1.tic1.IMAX3
                        Case "ISOUSC"
                            objet.value = data1.tic1.ISOUSC
                        Case "OPTARIF"
                            objet.value = data1.tic1.OPTARIF
                        Case "PAP"
                            objet.value = data1.tic1.PAP
                        Case "PEJP"
                            objet.value = data1.tic1.PEJP
                        Case "PTEC"
                            objet.value = data1.tic1.PTEC
                        Case "PINCE"
                            objet.value = data1.pince.I1
                        Case "INDEX PINCE"
                            objet.value = data1.pince.INDEX1
                        Case "PULSE"
                            objet.value = data1.impulsion.PULSE1
                        Case "INDEX PULSE"
                            objet.value = data1.impulsion.INDEX1
                        Case "SONDE"
                            objet.value = data1.temp.SONDE1
                        Case "RELAIS"
                            objet.value = form1.RL(1).value
                        Case "ENTREE DIGITAL"
                            objet.value = form1.IN(1).value
                        Case "ENTREE ANALOGIQUE"
                            objet.value = form1.AD(1).value
                        Case "VIRTUEL"
                            objet.value = form1.VS(1).value
                    End Select

                Case "2"
                    Select Case UCase(objet.adresse1)
                        Case "ADCO"
                            objet.value = data1.tic2.ADCO
                        Case "BASE"
                            objet.value = data1.tic2.BASE
                        Case "BBRHCJB"
                            objet.value = data1.tic2.BBRHCJB
                        Case "BBRHCJR"
                            objet.value = data1.tic2.BBRHCJR
                        Case "BBRHCJW"
                            objet.value = data1.tic2.BBRHCJW
                        Case "BBRHPJB"
                            objet.value = data1.tic2.BBRHPJB
                        Case "BBRHPJR"
                            objet.value = data1.tic2.BBRHPJR
                        Case "BBRHPJW"
                            objet.value = data1.tic2.BBRHPJW
                        Case "DEMAIN"
                            objet.value = data1.tic2.DEMAIN
                        Case "EJPHN"
                            objet.value = data1.tic2.EJPHN
                        Case "EJPHPM"
                            objet.value = data1.tic2.EJPHPM
                        Case "HCHC"
                            objet.value = data1.tic2.HCHC
                        Case "HCHP"
                            objet.value = data1.tic2.HCHP
                        Case "IINST"
                            objet.value = data1.tic2.IINST
                        Case "IINST1"
                            objet.value = data1.tic2.IINST1
                        Case "IINST2"
                            objet.value = data1.tic2.IINST2
                        Case "IINST3"
                            objet.value = data1.tic2.IINST3
                        Case "IMAX"
                            objet.value = data1.tic2.IMAX
                        Case "IMAX1"
                            objet.value = data1.tic2.IMAX1
                        Case "IMAX2"
                            objet.value = data1.tic2.IMAX2
                        Case "IMAX3"
                            objet.value = data1.tic2.IMAX3
                        Case "ISOUSC"
                            objet.value = data1.tic2.ISOUSC
                        Case "OPTARIF"
                            objet.value = data1.tic2.OPTARIF
                        Case "PAP"
                            objet.value = data1.tic2.PAP
                        Case "PEJP"
                            objet.value = data1.tic2.PEJP
                        Case "PTEC"
                            objet.value = data1.tic2.PTEC
                        Case "PINCE"
                            objet.value = data1.pince.I2
                        Case "INDEX PINCE"
                            objet.value = data1.pince.INDEX2
                        Case "PULSE"
                            objet.value = data1.impulsion.PULSE2
                        Case "INDEX PULSE"
                            objet.value = data1.impulsion.INDEX2
                        Case "SONDE"
                            objet.value = data1.temp.SONDE2
                        Case "RELAIS"
                            objet.value = form1.RL(2).value
                        Case "ENTREE DIGITAL"
                            objet.value = form1.IN(2).value
                        Case "ENTREE ANALOGIQUE"
                            objet.value = form1.AD(2).value
                        Case "VIRTUEL"
                            objet.value = form1.VS(2).value
                    End Select

                Case "3"
                    Select Case UCase(objet.adresse1)
                        Case "PINCE"
                            objet.value = data1.pince.I3
                        Case "INDEX PINCE"
                            objet.value = data1.pince.INDEX3
                        Case "PULSE"
                            objet.value = data1.impulsion.PULSE3
                        Case "INDEX PULSE"
                            objet.value = data1.impulsion.INDEX3
                        Case "SONDE"
                            objet.value = data1.temp.SONDE3
                        Case "VIRTUEL"
                            objet.value = form1.VS(3).value
                    End Select

                Case "4"
                    Select Case UCase(objet.adresse1)
                        Case "PINCE"
                            objet.value = data1.pince.I4
                        Case "INDEX PINCE"
                            objet.value = data1.pince.INDEX4
                        Case "PULSE"
                            objet.value = data1.impulsion.PULSE4
                        Case "INDEX PULSE"
                            objet.value = data1.impulsion.INDEX4
                        Case "SONDE"
                            objet.value = data1.temp.SONDE4
                        Case "VIRTUEL"
                            objet.value = form1.VS(4).value
                    End Select

                Case "5"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE5
                        Case "VIRTUEL"
                            objet.value = form1.VS(5).value
                    End Select

                Case "6"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE6
                        Case "VIRTUEL"
                            objet.value = form1.VS(6).value
                    End Select

                Case "7"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE7
                        Case "VIRTUEL"
                            objet.value = form1.VS(7).value
                    End Select
                    
                Case "8"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE8
                        Case "VIRTUEL"
                            objet.value = form1.VS(8).value
                    End Select
                    
                Case "9"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE9
                    End Select

                Case "10"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE10
                    End Select

                Case "11"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE11
                    End Select

                Case "12"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE12
                    End Select

                Case "13"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE13
                    End Select

                Case "14"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE14
                    End Select

                Case "15"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE15
                    End Select

                Case "16"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE16
                    End Select

                Case "17"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE17
                    End Select

                Case "18"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE18
                    End Select

                Case "19"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE19
                    End Select

                Case "20"
                    Select Case UCase(objet.adresse1)
                        Case "SONDE"
                            objet.value = data1.temp.SONDE20
                    End Select

            End Select

        Catch ex As Exception
            WriteLog("ERR: GetData=> Read, Exception : " & ex.Message)
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

    Public Class data
        <XmlElement>
        Public tic1 As tic = New tic
        Public tic2 As tic = New tic
        Public pince As pince = New pince
        Public impulsion As impulsion = New impulsion
        Public temp As temp = New temp
    End Class

    Public Class tic
        Public ADCO As String
        Public OPTARIF As String
        Public ISOUSC As Double
        Public PTEC As String
        Public PAP As Double
        Public IINST As Double
        Public IINST1 As Double
        Public IINST2 As Double
        Public IINST3 As Double
        Public IMAX As Double
        Public IMAX1 As Double
        Public IMAX2 As Double
        Public IMAX3 As Double
        Public PEJP As Double
        Public DEMAIN As String
        Public BASE As Double
        Public HCHC As Double
        Public HCHP As Double
        Public EJPHN As Double
        Public EJPHPM As Double
        Public BBRHCJB As Double
        Public BBRHPJB As Double
        Public BBRHCJW As Double
        Public BBRHPJW As Double
        Public BBRHCJR As Double
        Public BBRHPJR As Double
    End Class

    Public Class pince
        Public I1 As Double = 0
        Public INDEX1 As Double = 0
        Public I2 As Double = 0
        Public INDEX2 As Double = 0
        Public I3 As Double = 0
        Public INDEX3 As Double = 0
        Public I4 As Double = 0
        Public INDEX4 As Double = 0
    End Class

    Public Class temp
        Public SONDE1 As Double = 0
        Public SONDE2 As Double = 0
        Public SONDE3 As Double = 0
        Public SONDE4 As Double = 0
        Public SONDE5 As Double = 0
        Public SONDE6 As Double = 0
        Public SONDE7 As Double = 0
        Public SONDE8 As Double = 0
        Public SONDE9 As Double = 0
        Public SONDE10 As Double = 0
        Public SONDE11 As Double = 0
        Public SONDE12 As Double = 0
        Public SONDE13 As Double = 0
        Public SONDE14 As Double = 0
        Public SONDE15 As Double = 0
        Public SONDE16 As Double = 0
        Public SONDE17 As Double = 0
        Public SONDE18 As Double = 0
        Public SONDE19 As Double = 0
        Public SONDE20 As Double = 0
    End Class

    Public Class RL
        Public id As String = ""
        Public value As Integer = 0
    End Class

    Public Class [IN]
        Public id As String = ""
        Public value As Integer = 0
    End Class

    Public Class AD
        Public id As String = ""
        Public value As Integer = 0
    End Class

    Public Class VS
        Public id As String = ""
        Public value As Integer = 0
    End Class

    Public Class form
        Public RL() As RL
        Public [IN]() As [IN]
        Public AD() As AD
        Public VS() As VS
    End Class

    Public Class impulsion
        Public PULSE1 As Double = 0
        Public INDEX1 As Double = 0
        Public PULSE2 As Double = 0
        Public INDEX2 As Double = 0
        Public PULSE3 As Double = 0
        Public INDEX3 As Double = 0
        Public PULSE4 As Double = 0
        Public INDEX4 As Double = 0
    End Class

End Class
