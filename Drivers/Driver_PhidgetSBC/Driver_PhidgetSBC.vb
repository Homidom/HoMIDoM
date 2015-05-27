Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports Phidgets
Imports STRGS = Microsoft.VisualBasic.Strings

Imports System.Text.RegularExpressions

' Auteur : jphomi sur une base HoMIDoM phidgetinterface
' Date : 01/03/2015

''' <summary>Driver Phidget SBC pour piloter et recevoir des données des Phidgets SBC 8/8/8</summary>
''' <remarks>Nécessite l'installation des pilotes fournis sur le site http://www.phidgets.com/</remarks>


<Serializable()> Public Class Driver_PhidgetSBC
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "183368F2-722E-11E4-A1BA-7D491E5D46B0"
    Dim _Nom As String = "Phidget SBC"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Phidget SBCx 8/8/8"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "TCP"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "192.168.0.1"
    Dim _Port_TCP As String = "5001"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Phidget SBCx 8/8/8"
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
    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _NumeroUnit As Integer = 123456
#End Region

#Region "Variables Internes"
    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
    Dim _SerialInterface As Integer = 0

    'Dim WithEvents phidgetMan As Phidgets.Manager
    Dim WithEvents phidgetIFK As Phidgets.InterfaceKit
#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
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
    Public ReadOnly Property DeviceSupport() As ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
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
    ''' <param name="Command"></param>
    ''' <param name="Param"></param>
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

    ''' <summary>Permet de vérifier si un champ est valide</summary>
    ''' <param name="Champ"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1"
                    If Value IsNot Nothing Then
                        If String.IsNullOrEmpty(Value) Or IsNumeric(Value) Or InStr(Value, ":") = 0 Then
                            retour = "Veuillez saisir l'adresse de lecture sous la forme A:1 pour Analog entree1, D:3 pour Digital entree3 "
                        End If
                    End If
                Case "ADRESSE2"
                    If Value IsNot Nothing Then
                        If String.IsNullOrEmpty(Value) Or Not IsNumeric(Value) Then
                            retour = "Veuillez saisir l'adresse d'écriture sous la forme 1 pour Digital sortie1 "
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
        'récupération des paramétres avancés 

        Try
            _DEBUG = _Parametres.Item(0).Valeur
            _NumeroUnit = _Parametres.Item(1).Valeur
        Catch ex As Exception
            _DEBUG = False
            _Parametres.Item(0).Valeur = False
            WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
        End Try

        'ouverture du port suivant le Port Com ou IP
        Try
            'cree l'objet
            WriteLog("DBG: ServeurID demande : " & IP_TCP & ":" & Port_TCP & " / " & _NumeroUnit)
            phidgetIFK = New Phidgets.InterfaceKit()
            phidgetIFK.open(_NumeroUnit, IP_TCP, CInt(Port_TCP))
            phidgetIFK.waitForAttachment(5000)
            If phidgetIFK.Attached Then
                WriteLog("SBC connecte : " & IP_TCP & ":" & Port_TCP & " -> " & _NumeroUnit)
                _IsConnect = True
            End If
            WriteLog("DBG: Nom: " & phidgetIFK.Name)
            WriteLog("DBG: Nombre d'entrees: " & phidgetIFK.inputs.Count.ToString())
            WriteLog("DBG: Nombre de sorties digitales: " & phidgetIFK.outputs.Count.ToString())
            WriteLog("DBG: Nombre de capteurs analogiques: " & phidgetIFK.sensors.Count.ToString())
        Catch ex As Exception
            WriteLog("ERR: Start Exception " & ex.Message)
            _IsConnect = False
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            RemoveHandler phidgetIFK.Error, AddressOf phidgetIFK_Error
            RemoveHandler phidgetIFK.InputChange, AddressOf phidgetIFK_InputChange
            RemoveHandler phidgetIFK.OutputChange, AddressOf phidgetIFK_DIOutputChange
            RemoveHandler phidgetIFK.SensorChange, AddressOf phidgetIFK_ANAOutputChange

            phidgetIFK.close()
            WriteLog("Driver arrêté")
            _IsConnect = False
        Catch ex As Exception
            WriteLog("ERR: Erreur lors de l'arret du driver: " & ex.ToString)
            _IsConnect = False
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

            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            Dim typadr As String
            Dim adr1 As Integer

            typadr = Mid(Objet.Adresse1, 1, 1)
            adr1 = Mid(Objet.Adresse1, InStr(Objet.Adresse1, ":") + 1, 1)
            WriteLog("DBG: Read Composant " & Objet.Name & " --> " & Objet.Adresse1 & " / " & Objet.type)

            If typadr = "A" Then
                Select Case Objet.Type
                    Case "HUMIDITE"   'sensor 1107, 1125H
                        Objet.Value = Regex.Replace(CStr((phidgetIFK.sensors(adr1).Value * 0.1906) - 40.2), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Case "TEMPERATURE"  ' sensor 1124, 1125T
                        Objet.Value = Regex.Replace(CStr((phidgetIFK.sensors(adr1).Value * 0.2222) - 61.11), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Case "BAROMETRE"   'sensor 1115
                        Objet.Value = Regex.Replace(CStr((phidgetIFK.sensors(adr1).Value / 4) + 10), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Case "VOLTAGE"   'sensor 1117, 1123
                        Objet.Value = Regex.Replace(CStr((phidgetIFK.sensors(adr1).Value * 0.06) - 30), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Case Else
                        Objet.Value = phidgetIFK.sensors(adr1).Value
                End Select
            Else
                Select Case Objet.Type
                    Case "PLUIECOURANT" ' 1mm par impulsion
                        If phidgetIFK.inputs(adr1) = True Then
                            Objet.Value = 1
                        Else
                            Objet.Value = 2
                        End If
                    Case Else
                        Objet.Value = phidgetIFK.sensors(adr1).Value
                End Select
            End If
        Catch ex As Exception
            WriteLog("ERR: READ, " & ex.ToString)
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
            If _IsConnect = False Then
                WriteLog("ERR: WRITE, Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

            WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)

            Dim adr2 As Integer
            adr2 = Mid(Objet.Adresse2, InStr(Objet.Adresse2, ":") + 1, 1)

            Try
                Select Case Objet.Type
                    Case "APPAREIL", "SWITCH"
                        If Command = "ON" Then
                            phidgetIFK.outputs(adr2) = True
                            Objet.value = True
                            If _DEBUG Then WriteLog("DBG: Write " & Objet.Type & " Adr : " & adr2 & " -> ON")
                        End If
                        If Command = "OFF" Then
                            phidgetIFK.outputs(adr2) = False
                            Objet.value = False
                            If _DEBUG Then WriteLog("DBG: Write " & Objet.Type & " Adr : " & adr2 & " -> OFF")
                        End If
                    Case Else
                        WriteLog("ERR: WRITE Erreur Write Type de composant non géré : " & Objet.Type.ToString)
                End Select
            Catch ex As Exception
                WriteLog("ERR: WRITE Erreur commande " & Command & " : " & ex.ToString)
            End Try
        Catch ex As Exception
            WriteLog("ERR: WRITE " & ex.ToString)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

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
            WriteLog("ERR: add_devicecommande Exception : " & ex.Message)
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
            WriteLog("ERR: Add_LibelleDriver Exception : " & ex.Message)
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
            WriteLog("ERR: Add_LibelleDevice Exception : " & ex.Message)
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
            WriteLog("ERR: add_devicecommande Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString
            
            'Liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.BAROMETRE.ToString)
            _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
            _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.TELECOMMANDE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)
            _DeviceSupport.Add(ListeDevices.UV.ToString)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT.ToString)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)

            'Parametres avancés
            add_paramavance("Debug", "Activer le Debug complet (True/False)", False)
            add_paramavance("Numéro Unit", "Numéro d'identification de l'unité à interroger", 123456)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellés Device
            Add_LibelleDevice("ADRESSE1", "Adresse Lecture", "Adresse de lecture (D:x pour digital entrée x, A:x pour analog entrée x) ou -1 si pas de lecture", "-1")
            Add_LibelleDevice("ADRESSE2", "Adresse Ecriture", "Adresse d'écriture ou -1 si pas de écriture", "-1")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "@", "")

        Catch ex As Exception
            WriteLog("ERR: New Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

#End Region

#Region "Fonctions propres au driver"
    Private Sub phidgetIFK_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetIFK.Error
        WriteLog("ERR: Erreur: " & e.Description)
    End Sub

    'digital input change event handler... here we check or uncheck the corresponding input checkbox based on the index of
    'the digital input that generated the event
    Private Sub phidgetIFK_InputChange(ByVal sender As Object, ByVal e As Phidgets.Events.InputChangeEventArgs) Handles phidgetIFK.InputChange
        WriteLog("Valeur change, Entree Num: " & e.Index & " Value: " & e.Value)
        traitement(e.Value, e.Index)
    End Sub

    ''digital output change event handler... here we check or uncheck the corresponding output checkbox based on the index of
    ''the output that generated the event
    Private Sub phidgetIFK_DIOutputChange(ByVal sender As Object, ByVal e As Phidgets.Events.OutputChangeEventArgs) Handles phidgetIFK.OutputChange
        WriteLog("DBG: DigitalOutputChange Sortie Num: " & e.Index & " Value: " & e.Value)
    End Sub
    ''analog output change event handler... here we check or uncheck the corresponding output checkbox based on the index of
    ''the output that generated the event
    Private Sub phidgetIFK_ANAOutputChange(ByVal sender As Object, ByVal e As Phidgets.Events.SensorChangeEventArgs) Handles phidgetIFK.SensorChange
        WriteLog("DBG: AnalogicInputChange Sortie Num: " & e.Index & " Value: " & e.Value)
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As Boolean, ByVal adresse As String)
        Try
            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            Dim typedevice As String
            adresse = "D:" + adresse

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, "", Me._ID, True)

            'quitte si pas de device
            If (listedevices.Count = 0) Then Exit Sub

            typedevice = Trim(Mid(listedevices.Item(0).ToString, InStr(listedevices.Item(0).ToString, "+") + 1, Len(listedevices.Item(0).ToString)))

            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                'correction valeur pour correspondre au type de value
                If TypeOf listedevices.Item(0).Value Is Integer Then
                    If valeur = True Then
                        valeur = 100
                    ElseIf valeur = False Then
                        valeur = 0
                    End If
                End If

                listedevices.Item(0).Value = valeur
                WriteLog("Nouvelle valeur: " & typedevice & "/" & adresse & "->" & valeur)
            ElseIf (listedevices.Count > 1) Then
                WriteLog("ERR: traitement Process: Plusieurs composants correspondent à : " & adresse & ":" & valeur)
            Else
                WriteLog("ERR: traitement Process: Composant non trouvé : " & adresse & ":" & valeur)
                'si autodiscover = true ou modedecouverte du serveur actif alors on crée le composant sinon on logue
                If _AutoDiscover Or _Server.GetModeDecouverte Then
                    WriteLog("DBG: traitement Process: Composant non trouvé, AutoCreation du composant : " & adresse & ":" & valeur)
                    _Server.AddDetectNewDevice(adresse, _ID, "", "", valeur)
                Else
                    WriteLog("ERR: traitement Process: Composant non trouvé : " & adresse & ":" & valeur)
                End If
            End If
        Catch ex As Exception
            WriteLog("ERR: traitement Exception : " & ex.Message & " --> " & adresse & " : " & valeur)
            Exit Sub
        End Try
    End Sub

    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                If _DEBUG Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Phidget SBC", STRGS.Right(message, message.Length - 5))
                End If
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget SBC", STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget SBC", message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget SBC WriteLog", ex.Message)
        End Try
    End Sub
#End Region
End Class
