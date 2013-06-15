Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports Oregon

'************************************************
'INFOS 
'************************************************
'Le driver fonctionne de la manière suivante:
' 
'ETAPE1: Le driver est créé par Homidom --> lancement de la fonction Sub (pour récupérer/définir les paramètres avancés)
'ETAPE2: Le driver est lancé par Homidom --> lancement de la fonction Start (communication, ajout des évènements, config des pins...)
'ETAPE3: l'utilisateur demande la lecture d'un device --> lancement de la fonction read
'ETAPE4: le driver est arrêté par Homidom --> lancement de la fonction stop
'************************************************

Public Class Driver_OregonS

    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "D68AD962-8A53-11E2-A373-2F456288709B"
    Dim _Nom As String = "Oregon Scientific"
    Dim _Enable As Boolean = False
    Dim _Description As String = "WMR100"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "USB"
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
    Dim WithEvents MyTimer As New Timers.Timer
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False
    Dim _DEBUG As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 60000
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True

#End Region

#Region "Declaration"

    Dim WithEvents wmr100 As wmr100

    Const FRAMEKEY_DATETIME As Integer = 96
    Const FRAMEKEY_RAIN As Integer = 65
    Const FRAMEKEY_TEMPERATURE As Integer = 66
    Const FRAMEKEY_WIND As Integer = 72
    Const FRAMEKEY_BAROMETER As Integer = 70

    Const FRAMELENGTH_DATETIME As Integer = 11
    Const FRAMELENGTH_RAIN As Integer = 16
    Const FRAMELENGTH_TEMPERATURE As Integer = 11
    Const FRAMELENGTH_WIND As Integer = 10
    Const FRAMELENGTH_BAROMETER As Integer = 7

    Private oLastReception As New Collection

    Private GlobalInputReportBuffer(1000) As Byte
    Private GlobalInputReportBufferLength As Integer = 0

    Private Delegate Sub MarshalToForm _
        (ByVal action As String, _
        ByVal textToAdd As String)

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

    Public Function GetCommandPlus() As List(Of DeviceCommande)
        Return _DeviceCommandPlus
    End Function

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
                    If IsNumeric(Value) = False Then
                        retour = "veuillez saisir une adresse numérique est positive entre 2 et 12"
                    Else
                        If Value < 0 And Value > 10 Then
                            retour = "veuillez saisir une adresse entre 0 et 10"
                        End If
                    End If
            End Select
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

        If Enable = True Then
            'récupération des paramétres avancés
            Try
                wmr100 = New wmr100

                wmr100._ProductID = _Parametres.Item(0).Valeur
                wmr100._VendorID = _Parametres.Item(1).Valeur
                _DEBUG = _Parametres.Item(2).Valeur
                'wmr100._ProductID = "CA01" 
                'wmr100._VendorID = "0FDE" 


            Catch ex As Exception
                'wmr100._ProductID = "CA01" 
                'wmr100._VendorID = "0FDE" 
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)

            End Try

            Try
                wmr100.Start()
                'On ajoute à l'objet les évènements générés par la carte (fournis par la dll)
                AddHandler wmr100.Translate, AddressOf Translate 'Evènement lors d'un changement de port Binaire
                AddHandler wmr100.Info, AddressOf InfoReceieved 'Evènement lorsque la carte envoi sa version
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Driver démarré")

                _IsConnect = True
                MyTimer.Interval = 60000
                MyTimer.Enabled = True
                'System.Threading.Thread.Sleep(3000)
                'wmr100.SendToHID("20 00 08 01 00 00 00 00") 'init
                'System.Threading.Thread.Sleep(3000)
                'wmr100.SendToHID("01 0D 08 01 00 00 00 00") 'pc ready
                ' wmr100.SendToHID("01 DF 08 01 00 00 00 00") 'pc not ready

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur lors de la connexion au port USB: " & ex.ToString)
                _IsConnect = False
            End Try
        End If

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
            'On désaffecte les évènements pouvant être produit par l'objet (Arduino)
            RemoveHandler wmr100.Translate, AddressOf Translate
            RemoveHandler wmr100.Info, AddressOf InfoReceieved

            MyTimer.Stop()
            'Deconnexion usb
            wmr100.Shutdown()

            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver arrêté")
            _IsConnect = False
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

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write

    End Sub

    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

    End Sub

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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_LibelleDriver", "Exception : " & ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_LibelleDevice", "Exception : " & ex.Message)
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

        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Devices supportés par le driver
            _DeviceSupport.Add(ListeDevices.BAROMETRE.ToString)
            _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT.ToString)
            _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIECOURANT.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIETOTAL.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
            _DeviceSupport.Add(ListeDevices.UV.ToString)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)

            'Parametres avancés
            Add_ParamAvance("ProductID", "le ProductID de votre appareil USB", "CA01")
            Add_ParamAvance("VendorID", "le VendorID de votre appareil USB", "0FDE")
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            'add_devicecommande("PRESETDIM", "permet de paramétrer le DIM : param1=niveau, param2=timer", 2)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant.")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("REFRESH", "@", "")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "OregonS New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles MyTimer.Elapsed
        wmr100.Timer1_Tick(source, e)
    End Sub

#End Region

#Region "Fonctions propres au driver wmr100"

    ''' <summary>
    ''' Retourne les infos lues (fonction fournie par la dll)
    ''' </summary>
    ''' <param name="info">version majeure</param>
    ''' <remarks></remarks>

    Private Sub InfoReceieved(ByVal info As String)
        Try
            MyMarshalToForm("Log2", info)

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, "InfoReceieved : " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Message envoyé lorsqu'une valeur a changée
    ''' </summary>
    ''' <param name="data">buffer lu</param>
    ''' <remarks></remarks>

    Private Sub Translate(ByVal data As String)
        Try
            MyMarshalToForm("Log1", data)

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, "Translate erreur : " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Va écrire les valeurs dans Homidom
    ''' </summary>
    ''' <param name="valeur">valeur de l'equipement</param>
    ''' <param name="adresse">adresse de l'equipement</param>
    ''' <param name="type">type d'info</param>
    ''' <remarks></remarks>
    ''' 

    Private Sub traitement(ByVal valeur As Object, ByVal adresse As String, ByVal type As Integer)
        Try
            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            Dim _Type As String = ""

            Select Case type
                Case 0
                    _Type = "BAROMETRE"
                Case 1
                    _Type = "BATTERIE"
                Case 2
                    _Type = "DIRECTIONVENT"
                Case 3
                    _Type = "HUMIDITE"
                Case 4
                    _Type = "PLUIECOURANT"
                Case 5
                    _Type = "PLUIETOTAL"
                Case 6
                    _Type = "TEMPERATURE"
                Case 7
                    _Type = "UV"
                Case 8
                    _Type = "VITESSEVENT"
                Case 9
                    _Type = "GENERIQUESTRING"

                Case Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Le type de device n'appartient pas à ce driver: " & type)
                    Exit Sub
            End Select

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, _Type, Me._ID, True)

            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then

                ' If listedevices.Item(0).Value <> valeur Then
                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " " & listedevices.Item(0).type, listedevices.Item(0).adresse1 & " , " & listedevices.Item(0).Value & " <> " & CDbl(valeur))
                listedevices.Item(0).Value = valeur
                'End If


            ElseIf (listedevices.Count > 1) Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Plusieurs devices correspondent à : " & adresse & ":" & valeur)
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " traitement", "Le device à l'adresse : " & adresse & " de type " & _Type & " nexiste pas")
                'Le Device n'existe pas dans Homidom
            End If

            RaiseEvent DriverEvent(_Nom, type, valeur)

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & valeur)
        End Try
    End Sub

    Private Sub MyMarshalToForm _
       (ByVal action As String, _
       ByVal textToDisplay As String)
        Try
            'Purpose    : Enables accessing a form's controls from another thread 

            'Accepts    : action - a string that names the action to perform on the form
            '           : formText - text that the form displays or the code uses for 
            '           : another purpose. Actions that don't use text ignore this parameter.  

            Dim args() As Object = {action, textToDisplay}
            Dim MarshalToFormDelegate As MarshalToForm

            ' The AccessForm routine contains the code that accesses the form.

            MarshalToFormDelegate = New MarshalToForm(AddressOf AccessForm)

            ' Execute AccessForm, passing the parameters in args.

            'MyBase.Invoke(MarshalToFormDelegate, args)
            MarshalToFormDelegate.Invoke(action, textToDisplay)

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, "MyMarshalToForm : " & ex.Message)
        End Try

    End Sub

    Private Sub AccessForm(ByVal action As String, ByVal formText As String)

        'Purpose    : In asynchronous ReadFiles, the callback function GetInputReportData  
        '           : uses this routine to access the application's Form, which runs in 
        '           : a different thread.
        '           : The routine performs various application-specific functions that
        '           : involve accessing the application's form.

        'Accepts    : action - a string that names the action to perform on the form
        '           : formText - text that the form displays or the code uses for 
        '           : another purpose. Actions that don't use text ignore this parameter.  
        Dim etape As String

        Dim tmp1 As String = ""
        Dim tmp2 As String = ""
        Dim tmp3 As String = ""

        Try

            ' Select an action to perform on the form:

            Select Case action
                Case "Log1"
                    Dim byteValue As String
                    Dim s As String = ""
                    Dim tempdata As String = formText
                    Dim InputReportBuffer(9) As Byte
                    Dim InputReportBufferlenght As Integer

                    etape = "0"

                    Try

                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " etat meteo", "tempdata= " & tempdata)

                        While tempdata <> ""
                            Dim w As Integer = InStr(tempdata, ";")
                            InputReportBuffer(InputReportBufferlenght) = CByte(Strings.Left(tempdata, w - 1))
                            tmp2 = tmp2 & CByte(Strings.Left(tempdata, w - 1)) & " / "
                            tempdata = Strings.Right(tempdata, tempdata.Length - w)
                            InputReportBufferlenght += 1
                        End While

                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " etat meteo", "InputReportBuffer= " & tmp2)

                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " etat meteo", "InputReportBufferlenght= " & InputReportBufferlenght)

                        etape = "1"

                        For i As Integer = 2 To 1 + InputReportBuffer(1) 'UBound(InputReportBuffer)
                            GlobalInputReportBuffer(GlobalInputReportBufferLength + i - 2) = InputReportBuffer(i)
                        Next i

                        etape = "1-1"

                        GlobalInputReportBufferLength += 1 + InputReportBuffer(1) - 1 'UBound(InputReportBuffer) - 1

                        etape = "1-2"

                        For i As Integer = 0 To UBound(InputReportBuffer)
                            InputReportBuffer(i) = 0
                        Next i

                        etape = "1-3"

                        For count As Integer = GlobalInputReportBufferLength - 1 To 1 Step -1
                            If GlobalInputReportBuffer(count) <> 0 Or s <> "" Then

                                etape = "1-4"

                                'Add a leading zero to values from 0 to F.
                                If Len(Hex(GlobalInputReportBuffer(count))) < 2 Then
                                    byteValue = "0" & Hex(GlobalInputReportBuffer(count))
                                Else
                                    byteValue = Hex(GlobalInputReportBuffer(count))
                                End If

                                etape = "1-5"

                                If s <> "" Then s = " " & s
                                s = byteValue & s
                            End If
                        Next count

                        etape = "2"

                        Dim frameKeys As Integer() = {FRAMEKEY_DATETIME, _
                                                      FRAMEKEY_RAIN, _
                                                      FRAMEKEY_TEMPERATURE, _
                                                      FRAMEKEY_WIND, _
                                                      FRAMEKEY_BAROMETER}
                        Dim frameLengths As Integer() = {FRAMELENGTH_DATETIME, _
                                                      FRAMELENGTH_RAIN, _
                                                      FRAMELENGTH_TEMPERATURE, _
                                                      FRAMELENGTH_WIND, _
                                                      FRAMELENGTH_BAROMETER}

                        Dim iMaxFrame As Integer = 0

                        Dim f As Integer
                        Dim j As Integer
                        Dim checkSum As Long

                        etape = "3"

                        For i = 0 To GlobalInputReportBufferLength - 1
                            For f = 0 To frameKeys.Length - 1
                                If GlobalInputReportBuffer(i + 1) = frameKeys(f) Then
                                    checkSum = 0
                                    If (i + frameLengths(f) < GlobalInputReportBufferLength) Then
                                        For j = i To i + frameLengths(f) - 2
                                            checkSum += GlobalInputReportBuffer(j)
                                            tmp1 = tmp1 & GlobalInputReportBuffer(j) & " / "
                                        Next j
                                        checkSum = checkSum

                                        etape = "4"

                                        If (GlobalInputReportBuffer(i + frameLengths(f) - 1) + 256 * GlobalInputReportBuffer(i + frameLengths(f)) = checkSum) Then
                                            'If HEXA_LOG_MODE = 2 Then
                                            '    MyMarshalToForm("Log1", Format(Now, "HH:mm:ss") & " : Frame start in buffer : " & i & vbCrLf)
                                            'End If
                                            'Frame Found and CheckSum OK
                                            Dim bytes(frameLengths(f)) As Byte
                                            For j = i To i + frameLengths(f)
                                                bytes(j - i) = GlobalInputReportBuffer(j)
                                            Next j

                                            ''''

                                            Try
                                                'For j = 0 To bytes.Length - 3
                                                'checkSum += bytes(j)
                                                'Next j
                                                'checkSum = checkSum Mod 256

                                                Dim bCS As Boolean = False

                                                'Console.Write(Format(Now, "HH:mm:ss") & " : " & "[ " & s & " ]")
                                                'MyMarshalToForm("Log2", Format(Now, "HH:mm:ss") & " : [ " & s & " ]" & vbCrLf)
                                                If (checkSum = bytes(bytes.Length - 2) + 256 * bytes(bytes.Length - 1)) Then
                                                    'Console.WriteLine(" CheckSum OK")
                                                    bCS = True
                                                Else
                                                    'Console.WriteLine()
                                                End If

                                                If bCS Then
                                                    s = "OK : "
                                                Else
                                                    s = "     : "
                                                End If

                                                Dim sTime As String = ""
                                                Dim lr As Date = Date.FromOADate(0)
                                                If bCS Then
                                                    Dim sKey As String = Hex(bytes(1))
                                                    If bytes(1) = FRAMEKEY_TEMPERATURE Then

                                                        Dim iSensor As Integer = bytes(2) Mod 16
                                                        sKey &= iSensor
                                                    End If
                                                    Try
                                                        lr = CDate(oLastReception(sKey))
                                                    Catch ex As Exception
                                                    End Try

                                                    If (lr.ToOADate = 0) Then
                                                        sTime = " "
                                                    Else
                                                        sTime = Int((Now.ToOADate - lr.ToOADate) * 24 * 60 * 60) & "s"
                                                    End If

                                                    Try
                                                        oLastReception.Remove(sKey)
                                                    Catch ex As Exception
                                                    End Try
                                                    oLastReception.Add(Now, sKey)
                                                End If

                                                s &= " " & sTime & " : "

                                                If (bytes(1) = FRAMEKEY_WIND) Or (bytes(1) = FRAMEKEY_DATETIME) Or (bytes(1) = FRAMEKEY_RAIN) Then
                                                    Dim iPower As Integer = bytes(0) \ 16
                                                    Dim bPower As Boolean = (iPower Mod 8) < 4
                                                    Dim bWithoutSector As Boolean = iPower >= 8

                                                    If bWithoutSector Then
                                                        s &= "Secteur débranché ; "
                                                    End If
                                                    If bPower Then
                                                        s &= "Piles chargées ; "
                                                    Else
                                                        s &= "Piles vides ; "
                                                    End If
                                                End If

                                                'Anémomètre
                                                If (bytes(1) = FRAMEKEY_WIND) Then
                                                    Dim dWindGust As Double = CDbl(bytes(4) + 256 * (bytes(5) Mod 16)) * 0.1
                                                    Dim dWindAverage As Double = CDbl(bytes(5) \ 16 + bytes(6) * 16) * 0.1
                                                    Dim dWindDirection As Double = (CDbl(bytes(2)) Mod 16) * 360 / 16


                                                    's &= "Wind : "
                                                    's &= "(G) " & dWindGust & " m/s ; "
                                                    's &= "G " & CDbl(Bytes(4)) * 0.1 & " m/s ; "
                                                    's &= "(M) " & dWindAverage & " m/s ; "
                                                    's &= dWindDirection & "°"

                                                    'TableMeteo.VitVent = Bytes(4) + 256 * (Bytes(5) Mod 16)

                                                    'TableMeteo.DirVent = (Bytes(2)) Mod 16

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Vitesse du vent", "Vitesse du vent= " & bytes(4) + 256 * (bytes(5) Mod 16))
                                                        traitement(dWindGust, 1, 8) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Vitesse du vent", "Erreur : " & ex.ToString)
                                                    End Try

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Vitesse du vent", "Vitesse du vent= " & bytes(4) + 256 * (bytes(5) Mod 16))
                                                        traitement(dWindAverage, 0, 8) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Vitesse du vent", "Erreur : " & ex.ToString)
                                                    End Try

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Direction du vent", "Direction du vent= " & (bytes(2)) Mod 16)
                                                        traitement(dWindDirection, 0, 2) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Direction du vent", "Erreur : " & ex.ToString)
                                                    End Try

                                                End If
                                                'Thermomètre/Hygromètre
                                                If bytes(1) = FRAMEKEY_TEMPERATURE Then

                                                    Dim iSensor As Integer = bytes(2) Mod 16
                                                    Dim dTemperature As Double = CDbl(bytes(3) + bytes(4) * 16 * 16) / 10
                                                    Dim dHumidity As Double = CDbl(bytes(5))
                                                    Dim dDewPoint As Double = CDbl(bytes(6) + bytes(7) * 16 * 16) / 10
                                                    Dim iConfort As Integer = bytes(2) \ 16
                                                    Dim iTempVariation As Integer = bytes(0) \ 16

                                                    s &= "Sensor " & iSensor & " : "
                                                    s &= dTemperature & "°C "
                                                    Select Case iTempVariation
                                                        Case 0 : s &= "(=) ; "
                                                        Case 1 : s &= "(+) ; "
                                                        Case 2 : s &= "(-) ; "
                                                        Case Else : s &= "(" & iTempVariation & ") ; "
                                                    End Select
                                                    s &= dHumidity & "% ; "
                                                    s &= dDewPoint & "°C ; "
                                                    s &= "Confort(" & iConfort & ") "
                                                    Select Case iConfort
                                                        Case 8 : s &= "Non convenable"
                                                        Case 12 : s &= "Neutre"
                                                        Case Else : s &= "?"
                                                    End Select

                                                    Dim temp As Int32
                                                    If (bytes(3) + bytes(4) * 16 * 16) > 32467 Then
                                                        temp = CInt(((bytes(3) + bytes(4) * 16 * 16) - 32767) * (-1)) / 10
                                                    Else
                                                        temp = CInt(bytes(3) + bytes(4) * 16 * 16) / 10
                                                    End If

                                                    'TableTemperature.TH(iSensor) = temp
                                                    'TableTemperature.HG(iSensor) = Bytes(5)

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Temperature", "Température " & iSensor & " = " & temp)
                                                        traitement(temp, iSensor, 6) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Temperature", "Erreur : " & ex.ToString)
                                                    End Try

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Humidité", "Humidité " & iSensor & " = " & bytes(5))
                                                        traitement(bytes(5), iSensor, 3) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Humidité", "Erreur : " & ex.ToString)
                                                    End Try

                                                End If

                                                'Baromètre
                                                If (bytes(1) = FRAMEKEY_BAROMETER) Then
                                                    Dim dPressionRelative As Double = CDbl(bytes(4) + (bytes(5) Mod 16) * 16 * 16)
                                                    Dim dPressionAbsolue As Double = CDbl(bytes(2) + (bytes(3) Mod 16) * 16 * 16)

                                                    s &= "Baro : "
                                                    s &= "PR=" & dPressionRelative & "mb ; "
                                                    s &= "PA=" & dPressionAbsolue & "mb ; "

                                                    Dim p As Integer
                                                    Dim s1 As String = ""
                                                    Dim s2 As String = ""

                                                    p = bytes(5) \ 16
                                                    Select Case p
                                                        Case 0 : s1 &= "Partiellement nuageux"
                                                        Case 1 : s1 &= "Pluvieux"
                                                        Case 2 : s1 &= "Nuageux"
                                                        Case 3 : s1 &= "Ensoleillé"
                                                        Case 4 : s1 &= "Neigeux"
                                                        Case Else : s &= "?"
                                                    End Select

                                                    s &= " => "

                                                    p = bytes(3) \ 16
                                                    Select Case p
                                                        Case 0 : s2 = "Partiellement nuageux"
                                                        Case 1 : s2 = "Pluvieux"
                                                        Case 2 : s2 = "Nuageux"
                                                        Case 3 : s2 = "Ensoleillé"
                                                        Case 4 : s2 = "Neigeux"
                                                        Case Else : s = "?"
                                                    End Select


                                                    'TableMeteo.Pression = (Bytes(2) + (Bytes(3) Mod 16) * 16 * 16)

                                                    'TableMeteo.Prevision = s

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Barometre", "Barometre = " & (bytes(2) + (bytes(3) Mod 16) * 16 * 16))
                                                        traitement(dPressionRelative, 0, 0) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Barometre", "Erreur : " & ex.ToString)
                                                    End Try

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Prévision", "Prévision = " & s)
                                                        traitement(s1, 0, 9) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Prévision", "Erreur : " & ex.ToString)
                                                    End Try

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Barometre", "Barometre = " & (bytes(2) + (bytes(3) Mod 16) * 16 * 16))
                                                        traitement(dPressionAbsolue, 1, 0) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Barometre", "Erreur : " & ex.ToString)
                                                    End Try

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Prévision", "Prévision = " & s)
                                                        traitement(s2, 1, 9) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Prévision", "Erreur : " & ex.ToString)
                                                    End Try

                                                End If

                                                'Pluviomètre
                                                If (bytes(1) = FRAMEKEY_RAIN) Then
                                                    Dim dRainRate As Double = CDbl(bytes(2) + bytes(3) * 16 * 16) / 100 * 25.4
                                                    Dim dRain1h As Double = CDbl(bytes(4) + bytes(5) * 16 * 16) / 100 * 25.4
                                                    Dim dRain24h As Double = CDbl(bytes(6) + bytes(7) * 16 * 16) / 100 * 25.4
                                                    Dim dRainAccum As Double = CDbl(bytes(8) + bytes(9) * 16 * 16) / 100 * 25.4

                                                    s &= "Rain : "

                                                    s &= "Rate = " & dRainRate & " mm/h ; "
                                                    s &= "1h = " & dRain1h & " mm ; "
                                                    s &= "24h  = " & dRain24h & " mm ; "
                                                    s &= "Accum = " & dRainAccum & " mm "

                                                    s &= "since " & Format(bytes(12), "00") & "/" & Format(bytes(13), "00") & "/" & Format(bytes(14), "00") & " "
                                                    s &= Format(bytes(11), "00") & ":" & Format(bytes(10), "00")


                                                    'TableMeteo.Pluie24h = CInt(Bytes(6) + Bytes(7) * 16 * 16)

                                                    'TableMeteo.PluieAccu = Bytes(8) + Bytes(9) * 16 * 16

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Pluie 24h", "Pluie 24h = " & CInt(bytes(6) + bytes(7) * 16 * 16))
                                                        traitement(dRainRate, 0, 5) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Pluie immediat", "Erreur : " & ex.ToString)
                                                    End Try

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Pluie 24h", "Pluie 24h = " & CInt(bytes(6) + bytes(7) * 16 * 16))
                                                        traitement(dRain1h, 1, 5) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Pluie 1h", "Erreur : " & ex.ToString)
                                                    End Try


                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Pluie 24h", "Pluie 24h = " & CInt(bytes(6) + bytes(7) * 16 * 16))
                                                        traitement(dRain24h, 2, 5) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Pluie 24h", "Erreur : " & ex.ToString)
                                                    End Try

                                                    Try
                                                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Pluie Accumulée", "Pluie Accumulée = " & bytes(8) + bytes(9) * 16 * 16)
                                                        traitement(dRainAccum, 0, 4) 'aller écrire la valeur dans le device --> Homidom
                                                    Catch ex As Exception
                                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Pluie Accumulée", "Erreur : " & ex.ToString)
                                                    End Try

                                                End If

                                                'Date/Heure
                                                If (bytes(1) = FRAMEKEY_DATETIME) Then
                                                    Dim iRadioFreq As Integer = (bytes(0) \ 16) Mod 4

                                                    s &= "Date/Time : "

                                                    s &= Format(bytes(6), "00") & "/" & Format(bytes(7), "00") & "/" & Format(bytes(8), "00") & " "
                                                    s &= Format(bytes(5), "00") & ":" & Format(bytes(4), "00") & " ; "
                                                    s &= "GMT "
                                                    If bytes(9) < 128 Then
                                                        s &= "+"
                                                    Else
                                                        s &= "-"
                                                    End If
                                                    s &= Format(bytes(9) Mod 128, "00") & " ; "
                                                    Select Case iRadioFreq
                                                        Case 0 : s &= " RadioFreq désactivée"
                                                        Case 1 : s &= " RadioFreq en recherche ou signal faible"
                                                        Case 2 : s &= " RadioFreq signal moyen"
                                                        Case 3 : s &= " RadioFreq signal fort"
                                                    End Select

                                                End If


                                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Meteo ", " Translate OK ")

                                            Catch ex As Exception
                                                'restart_ex = True
                                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Translate Meteo ", " Erreur : " & ex.ToString)
                                            End Try

                                            iMaxFrame = i + frameLengths(f)
                                        Else
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " etat meteo", "Checksum= " & GlobalInputReportBuffer(i + frameLengths(f) - 1) + 256 * GlobalInputReportBuffer(i + frameLengths(f)) & " - " & checkSum)
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " etat meteo", "GlobalInputReportBuffer= " & tmp1)
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " etat meteo", "framekeys(f)= " & frameKeys(f))
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " etat meteo", "frameLengths(f)= " & frameLengths(f))
                                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " etat meteo", "i= " & i)

                                        End If
                                    End If
                                End If
                            Next f
                        Next i

                        etape = "13"

                        If iMaxFrame > 0 Then
                            For i = iMaxFrame + 1 To GlobalInputReportBufferLength
                                GlobalInputReportBuffer(i - iMaxFrame - 1) = GlobalInputReportBuffer(i)
                            Next i
                            GlobalInputReportBufferLength -= iMaxFrame + 1
                            'GlobalInputReportBufferLength = 0
                            'For i = GlobalInputReportBufferLength To UBound(GlobalInputReportBuffer)
                            'GlobalInputReportBuffer(i) = 0
                            'Next i

                        End If

                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Translate Meteo ", " Erreur : " & etape & " / " & ex.ToString)
                    End Try
                Case "Log2"
                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom, formText)

                    If InStr(formText, "Device has been detected") <> 0 Then
                        _IsConnect = True
                    End If

                    If InStr(formText, "Device not detected") <> 0 Then
                        _IsConnect = False
                        'System.Threading.Thread.Sleep(3000)
                        'wmr100.SendToHID("20 00 08 01 00 00 00 00") 'init
                        'System.Threading.Thread.Sleep(3000)
                        'wmr100.SendToHID("01 0D 08 01 00 00 00 00") 'pc ready
                        'Restart()
                    End If

                    If InStr(formText, "Device has been lost") <> 0 Then
                        _IsConnect = False
                        'System.Threading.Thread.Sleep(3000)
                        'wmr100.SendToHID("20 00 08 01 00 00 00 00") 'init
                        'System.Threading.Thread.Sleep(3000)
                        'wmr100.SendToHID("01 0D 08 01 00 00 00 00") 'pc ready
                        'Restart()
                    End If

                    If InStr(formText, "Device has been removed") <> 0 Then
                        _IsConnect = False
                    End If

            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Translate Meteo ", " Erreur AcessForm ")
        End Try

    End Sub
#End Region

End Class
