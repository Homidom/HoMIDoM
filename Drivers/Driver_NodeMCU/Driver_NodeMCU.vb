Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Xml
Imports System.Net
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
'************************************************
'INFOS 
'************************************************
'Le driver fonctionne de la manière suivante:
' 
'ETAPE1: Le driver est créé par Homidom --> lancement de la fonction Sub (pour récupérer/définir les paramètres avancés)
'ETAPE2: Le driver est lancé par Homidom --> lancement de la fonction Start (communication, ajout des évènements, config des pins...)
'ETAPE3:
'          - une pin (ana ou binaire) change sur la carte --> déclenchement des fonctions DigitalMessageReceieved ou AnalogMessageReceieved
'          - l'utilisateur active un device (ON/OFF) --> lancement de la fonction write
'          - l'utilisateur demande la lecture d'un device --> lancement de la fonction read
'ETAPE4: le driver est arrêté par Homidom --> lancement de la fonction stop
'************************************************

Public Class Driver_NodeMCU
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "58C8EC76-E913-11E6-A71B-251C349F4EBC"
    Dim _Nom As String = "NodeMCU"
    Dim _Enable As Boolean = False
    Dim _Description As String = "NodeMCU"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "HTTP"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "@"
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
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    Dim NodesMCU As New List(Of NodeMCU)
    Dim MinRefresh As Integer = 3
#End Region

#Region "Fonctions génériques"

    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _idsrv = value
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

    Public Property AutoDiscover() As Boolean Implements HoMIDom.HoMIDom.IDriver.AutoDiscover
        Get
            Return _AutoDiscover
        End Get
        Set(ByVal value As Boolean)
            _AutoDiscover = value
        End Set
    End Property

    ''' <summary>
    ''' Aller lire une entrée
    ''' </summary>
    ''' <param name="Objet">Device</param>
    ''' <remarks></remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car le driver n'est pas Activé")
                Exit Sub
            End If
            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car le driver n'est pas connecté à la carte")
                Exit Sub
            End If

            'Dim IP As String = Objet.Adresse1
            'Dim Exist As Boolean = False
            'Dim mynode As NodeMCU = Nothing

            'For Each node In NodesMCU
            '    If node.IP = IP Then
            '        Exist = True
            '        mynode = node
            '        Exit For
            '    End If
            'Next

            'If Exist = False Then
            '    Dim node As New NodeMCU(IP)
            '    NodesMCU.Add(node)
            '    mynode = node
            '    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Read", "Ajout du NodeMCU à l'adresse " & IP)
            'Else
            '    If mynode.LastRefresh.AddMinutes(MinRefresh) <= Now Then
            '        Dim retour As String = mynode.Refresh()
            '        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Read", "Refresh du NodeMCU à l'adresse " & IP)
            '        If String.IsNullOrEmpty(retour) = False Then
            '            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur lors du Refresh du NodeMCU à l'adresse " & IP & ":" & retour)
            '        End If
            '    End If
            'End If

            'Dim Param As String = Objet.Adresse2
            'Select Case Param
            '    Case "temperature" : Objet.Value = mynode.Temperature
            '    Case "humidity" : Objet.Value = mynode.Humidity
            '    Case "motion" : Objet.Value = mynode.Motion
            '    Case "co2" : Objet.Value = mynode.CO2
            'End Select

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur : " & ex.ToString)
        End Try
    End Sub

    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
            If _Refresh < 60 Then _Refresh = 60
        End Set
    End Property

    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property

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
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>
    ''' Démarrer le driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            If Enable = True Then
                'récupération des paramétres avancés
                Try
                    MinRefresh = _Parametres.Item(0).Valeur
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, " Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
                    MinRefresh = 3
                End Try

                Dim Listdev As ArrayList = Server.ReturnDeviceByDriver(_idsrv, Me.ID, True)
                NodesMCU.Clear()

                For Each dev In Listdev
                    Dim IP As String = dev.Adresse1
                    Dim Exist As Boolean = False

                    For Each node In NodesMCU
                        If IP = node.IP Then
                            Exist = True
                        End If
                    Next

                    If Exist = False And String.IsNullOrEmpty(IP) = False Then
                        Dim x As New NodeMCU(IP)
                        NodesMCU.Add(x)
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Ajout du NodeMCU à l'adresse " & IP)
                    End If
                Next

                MyTimer.Interval = Refresh * 1000
                MyTimer.Start()
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Driver démarré ")
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Le driver n'est pas activé ou la carte est déjà connectée ")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur lors du démarrage du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property

    ''' <summary>
    ''' Arrêter le driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        'cree l'objet
        Try
            MyTimer.Stop()
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", "Erreur lors de l'arrêt du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

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

    ''' <summary>
    ''' Activer une sortie
    ''' </summary>
    ''' <param name="Objet"></param>
    ''' <param name="Commande"></param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        If _Enable = False Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible de traiter cette commande car le driver n'est pas activé")
            Exit Sub
        End If
        If _IsConnect = False Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible de traiter cette commande car le driver n'est connecté")
            Exit Sub
        End If
        Try
            'If Objet.type = "APPAREIL" Then
            '    'If Commande = "ON" Then
            '    '    ArduinoVB.DigitalWrite(Objet.Adresse1, 1)
            '    '    Objet.Value = True
            '    '    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Activation de la pin:" & Objet.Adresse1)
            '    'End If
            '    'If Commande = "OFF" Then
            '    '    ArduinoVB.DigitalWrite(Objet.Adresse1, 0)
            '    '    Objet.Value = False
            '    '    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Désactivation de la pin:" & Objet.Adresse1)
            '    'End If
            'Else
            '    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Write", "Impossible d'écrire sur un device autre que de type APPAREIL")
            'End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: " & ex.ToString)
        End Try
    End Sub

    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

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

    ''' <summary>
    ''' Déclaration du driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

        'Devices supportés par le driver
        _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
        _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)
        _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
        _DeviceSupport.Add(ListeDevices.APPAREIL)
        _DeviceSupport.Add(ListeDevices.CONTACT)
        _DeviceSupport.Add(ListeDevices.DETECTEUR)
        _DeviceSupport.Add(ListeDevices.HUMIDITE)
        _DeviceSupport.Add(ListeDevices.TEMPERATURE)
        _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE)
        _DeviceSupport.Add(ListeDevices.UV)
        _DeviceSupport.Add(ListeDevices.VITESSEVENT)
        _DeviceSupport.Add(ListeDevices.VOLET)


        'Paramétres avancés pouvant être définit côté Admin sur le driver
        Add_ParamAvance("MinRfshMin", "Reresh Mini Min", 1)

        Add_LibelleDevice("ADRESSE1", "Adresse IP", "")
        Add_LibelleDevice("ADRESSE2", "Nom parametre", "")
        Add_LibelleDevice("SOLO", "@", "")
        Add_LibelleDevice("MODELE", "@", "")

        AddHandler MyTimer.Elapsed, AddressOf MyTimer_Tick

    End Sub


#End Region

#Region "Fonctions propres au driver"
    Sub MyTimer_Tick()
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " MyTimer_Tick", "Impossible d'effectuer un Read car le driver n'est pas Activé")
                Exit Sub
            End If
            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " MyTimer_Tick", "Impossible d'effectuer un Read car le driver n'est pas connecté à la carte")
                Exit Sub
            End If

            Dim Listdev As ArrayList = Server.ReturnDeviceByDriver(_idsrv, Me.ID, True)
            Dim _LastIP As String = ""

            For Each node In NodesMCU
                Dim retour As String = node.Refresh()
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " MyTimer_Tick", "Refresh du NodeMCU à l'adresse " & node.IP)
                If String.IsNullOrEmpty(retour) = False Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " MyTimer_Tick", "Erreur lors du Refresh du NodeMCU à l'adresse " & node.IP & ":" & retour)
                End If
                Threading.Thread.Sleep(1000)
            Next

            For Each dev In Listdev
                Dim IP As String = dev.Adresse1
                Dim Exist As Boolean = False

                For Each node In NodesMCU
                    If node.IP = IP Then
                        Dim Param As String = dev.Adresse2
                        Dim RealDev As Object = Server.ReturnRealDeviceById(dev.id)
                        If RealDev IsNot Nothing Then
                            Select Case Param
                                Case "temperature" : RealDev.Value = node.Temperature
                                Case "humidity" : RealDev.Value = node.Humidity
                                Case "motion" : RealDev.Value = node.Motion
                                Case "co2" : RealDev.Value = node.CO2
                            End Select
                        End If
                    End If
                Next
            Next

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " MyTimer_Tick", "Exception : " & ex.Message)
        End Try
    End Sub
#End Region


End Class

Public Class NodeMCU
    Dim _ip As String
    Dim _temp As Double
    Dim _hum As Double
    Dim _co2 As Double
    Dim _motion As Boolean
    Dim _LastRefresh As Date

    Public ReadOnly Property LastRefresh As Date
        Get
            Return _LastRefresh
        End Get
    End Property

    Public Property IP As String
        Get
            Return _ip
        End Get
        Set(value As String)
            _ip = value
        End Set
    End Property

    Public Property Temperature As Double
        Get
            Return _temp
        End Get
        Set(value As Double)
            _temp = value
        End Set
    End Property

    Public Property Humidity As Double
        Get
            Return _hum
        End Get
        Set(value As Double)
            _hum = value
        End Set
    End Property

    Public Property CO2 As Double
        Get
            Return _co2
        End Get
        Set(value As Double)
            _co2 = value
        End Set
    End Property

    Public Property Motion As Boolean
        Get
            Return _motion
        End Get
        Set(value As Boolean)
            _motion = value
        End Set
    End Property

    Public Sub New(AdressIP As String)
        IP = AdressIP
        Refresh()
    End Sub

    Public Function Refresh() As String
        Try
            Dim url As New Uri("http://" & IP & "/status.json")
            Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
            Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)
            Dim SR As New StreamReader(response.GetResponseStream)
            Dim json As String = SR.ReadToEnd
            SR.Close()
            SR = Nothing
            Dim result = JsonConvert.DeserializeObject(Of ArrayList)(json)
            Dim token As JToken
            Dim rel
            Dim href

            For Each elmt As Object In result
                token = JObject.Parse(elmt.ToString())
                rel = token.SelectToken("name")
                href = token.SelectToken("value")
                If rel.ToString.ToLower = "temperature" Then
                    Temperature = CDbl(href.ToString.Replace(".", ","))
                End If
                If rel.ToString.ToLower = "humidity" Then
                    Humidity = CDbl(href.ToString.Replace(".", ","))
                End If
                If rel.ToString.ToLower = "motion" Then
                    Motion = href
                End If
                If rel.ToString.ToLower = "co2" Then
                    href = token.SelectToken("value")
                    CO2 = CDbl(href.ToString.Replace(".", ","))
                End If
            Next

            url = Nothing
            Request = Nothing
            response = Nothing
            json = String.Empty
            result = Nothing
            token = Nothing

            _LastRefresh = Now
            Return Nothing
        Catch ex As Exception
            Return ex.ToString
        End Try
    End Function
End Class