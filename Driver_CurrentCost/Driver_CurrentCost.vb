Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.IO.Ports
Imports System.Math
Imports System.Net.Sockets
Imports System.Threading
Imports System.Globalization

Public Class Driver_CurrentCost
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "DACF8CE4-0AD2-11E2-A9F7-43F86188709B"
    Dim _Nom As String = "CurrentCost"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Driver CurrentCost"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "COM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "v1"
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
#End Region

#Region "Variables Internes"
    Private serialPortObj As SerialPort
    Dim _BAUD As Integer = 57600
    Dim _RCVERROR As Boolean = True
    Dim first As Boolean = False
#End Region

#Region "Déclaration"
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
                    If Value = " " Then retour = "l'adresse est obligatoire"
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
            If Not _IsConnect Then
                Dim trv As Boolean = False
                Dim _ports As String = "<AUCUN>"

                'récupération des paramétres avancés
                Try
                    _BAUD = _Parametres.Item(0).Valeur
                    _RCVERROR = _Parametres.Item(1).Valeur
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
                    _BAUD = 57600
                    _RCVERROR = True
                End Try

                If _Com = "" Or _Com = " " Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Le port COM est vide veuillez le renseigner")
                    Exit Sub
                End If

                Dim portNames As String() = SerialPort.GetPortNames()
                Array.Sort(portNames)
                For Each serialPortName As String In portNames
                    _ports &= serialPortName & " "
                    If UCase(serialPortName) = UCase(_Com) Then
                        trv = True
                    End If
                Next

                If trv = False Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.nom & " Start", "Le port COM " & _Com & " n'existe pas, seuls les ports " & _ports & " existe(s)!")
                    Exit Sub
                End If

                serialPortObj = New SerialPort()
                serialPortObj.PortName = _Com
                serialPortObj.BaudRate = _BAUD
                serialPortObj.Parity = Parity.None
                serialPortObj.DataBits = 8
                serialPortObj.StopBits = 1
                serialPortObj.ReadTimeout = 50000

                If _RCVERROR Then AddHandler serialPortObj.ErrorReceived, New SerialErrorReceivedEventHandler(AddressOf serialPortObj_ErrorReceived)
                AddHandler serialPortObj.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)

                If serialPortObj.IsOpen Then
                    serialPortObj.Close()
                End If

                serialPortObj.Open()
                serialPortObj.DiscardInBuffer()
                _IsConnect = True
                first = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Port " & _Com & " ouvert")
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Port " & _Com & " déjà ouvert")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", ex.ToString)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            If _IsConnect Then
                serialPortObj.Close()
                RemoveHandler serialPortObj.ErrorReceived, New SerialErrorReceivedEventHandler(AddressOf serialPortObj_ErrorReceived)
                RemoveHandler serialPortObj.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                _IsConnect = False
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.nom & " Stop", ex.Message)
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
            If _IsConnect = False Then Exit Sub

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.nom & " Read", ex.Message)
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
            If _IsConnect = False Then Exit Sub


        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.nom & " Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.nom & " DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.nom & " NewDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>ajout des commandes avancées pour les devices</summary>
    ''' <remarks></remarks>
    Private Sub add_devicecommande(ByVal nom As String, ByVal description As String, ByVal nbparam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = nom
            x.DescriptionCommand = description
            x.CountParam = nbparam
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
    Private Sub add_paramavance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
        Try
            Dim x As New HoMIDom.HoMIDom.Driver.Parametre
            x.Nom = nom
            x.Description = description
            x.Valeur = valeur
            _Parametres.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_paramavance", "ERR: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Parametres avancés
            add_paramavance("BaudRate", "Vitesse du port COM (57600 ou 9600)", 57600)
            add_paramavance("ErrorReceived", "Gérer les erreurs de réception (True=Activé, False=Désactivé)", True)

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse de la variable", "Adresse de la variable (ex: ch1)")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("REFRESH", "Refresh", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.nom & " New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

#End Region

#Region "Fonctions internes"
    Private Sub serialPortObj_ErrorReceived(ByVal sender As Object, ByVal e As SerialErrorReceivedEventArgs)
        Select Case e.EventType
            Case SerialError.Frame
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: Le matériel a détecté une erreur de trame")
            Case SerialError.Overrun
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: Un dépassement de mémoire tampon de caractères s'est produit.Le caractère suivant est perdu")
            Case SerialError.RXOver
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: Un dépassement de la mémoire tampon d'entrée s'est produit.Il n'y a plus de place dans la mémoire tampon d'entrée ou un caractère a été reçu après le caractère de fin de fichier")
                serialPortObj.DiscardInBuffer()
            Case SerialError.RXParity
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: Le matériel a détecté une erreur de parité")
            Case SerialError.TXFull
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: L'application a essayé de transmettre un caractère, mais la mémoire tampon de sortie était pleine")
            Case Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Erreur inconnue, le driver va tenter de traiter les données")
                Dim line As String = serialPortObj.ReadLine()
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Données reçues: " & line)

                Dim update As New CurrentCostUpdate(line)

                If update.ValidUpdate Then
                    traitement("tmpr", update.Temperature)
                    traitement("ch1", update.Channel1Watts)
                    traitement("ch2", update.Channel2Watts)
                    traitement("ch3", update.Channel3Watts)
                    traitement("time", update.Time)
                Else
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Update non réussi")
                End If
        End Select
    End Sub

    ''' <summary>
    ''' Traite les infos reçus
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        Try
            If first Then
                first = False
            Else
                'on attend d'avoir le reste
                System.Threading.Thread.Sleep(1500)

                Dim line As String = serialPortObj.ReadExisting
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DataReceived", "Données reçues: " & line)

                Dim update As New CurrentCostUpdate(line)

                If update.ValidUpdate Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DataReceived", "tmpr: " & update.Temperature)
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DataReceived", "time: " & update.Time)
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DataReceived", "ch1: " & update.Channel1Watts)
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DataReceived", "ch2: " & update.Channel2Watts)
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DataReceived", "ch3: " & update.Channel3Watts)
                    traitement("tmpr", update.Temperature)
                    traitement("ch1", update.Channel1Watts)
                    traitement("ch2", update.Channel2Watts)
                    traitement("ch3", update.Channel3Watts)
                    traitement("time", update.Time)
                Else
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DataReceived", "Update non réussi")
                End If
            End If
        Catch Ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Datareceived", "Erreur:" & Ex.ToString)
        End Try
    End Sub


    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal adresse As String, ByVal data As Object)
        Try
            'Recherche si un device affecté
            Dim listedevices As New ArrayList

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, "", Me._ID, True)
            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                'correction valeur pour correspondre au type de value
                listedevices.Item(0).Value = data

            ElseIf (listedevices.Count > 1) Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Plusieurs devices correspondent à : " & adresse)
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Device non trouvé : " & adresse & ":" & adresse)

            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & data)
        End Try
    End Sub

#End Region

End Class
