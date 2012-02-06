Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports Driver_Arduino.Arduino
Imports Firmata

Public Class Driver_Arduino
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "958D5812-390B-11E1-A6F7-D4CA4724019B"
    Dim _Nom As String = "Arduino"
    Dim _Enable As String = False
    Dim _Description As String = "Arduino"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RS232"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "COM2"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Arduino"
    Dim _Version As String = "1.0"
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

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    'Private WithEvents Arduino As Arduino
    Dim WithEvents ArduinoVB As New Firmata.FirmataVB
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

    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then Exit Sub
            If _IsConnect = False Then Exit Sub

            Dim _type As Integer
            If Objet.Type = "CONTACT" Then _type = 0
            If Objet.Type = "APPAREIL" Then _type = 1
            If Objet.Type = "GENERIQUEVALUE" Then _type = 2

            traitement(ArduinoVB.DigitalRead(Objet.Adresse1), Objet.Adresse1, _type)
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
                    If IsNumeric(Value) = False Then
                        retour = "veuillez saisir une adresse numérique est positive entre 2 et 12"
                    Else
                        If Value < 2 And Value > 12 Then
                            retour = "veuillez saisir une adresse entre 2 et 12"
                        End If
                    End If
            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        'cree l'objet
        Try
            'Arduino = New Arduino(_Com, "9600")
            'Arduino.DigitalCount = 14
            'Arduino.AnalogCount = 6
            'Arduino.PWMPorts = New Integer() {3, 5, 6, 9, 10, 11}

            'AddHandler Arduino.DigitalDataReceived, AddressOf ArduinoDigitalData
            'AddHandler Arduino.AnalogDataReceived, AddressOf ArduinoAnalogData
            'AddHandler Arduino.LogMessageReceived, AddressOf WriteLog
            'AddHandler Arduino.ConnectionLost, AddressOf ConnectionLost

            'If Arduino.StartCommunication() = True Then
            '    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Carte connectée")
            '    _IsConnect = True

            '    For i As Integer = 0 To 6
            '        Arduino.SetDigitalDirection(i, Arduino.DigitalDirection.Input)
            '        Arduino.EnableDigitalPort(i, True)
            '        Arduino.EnableDigitalTrigger(i, True)
            '    Next
            '    For i As Integer = 7 To 13
            '        Arduino.SetDigitalDirection(i, Arduino.DigitalDirection.DigitalOutput)
            '        Arduino.EnableDigitalPort(i, True)
            '    Next
            'Else
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Le driver n'a pas réussit à se connecter à la carte ")
            '    _IsConnect = False
            'End If
            If Enable = True And ArduinoVB.PortOpen = False Then
                ArduinoVB.Connect(_Com, Firmata.FirmataVB.DEFAULT_BAUD_RATE)

                Threading.Thread.Sleep(1000)

                If ArduinoVB.PortOpen = True Then
                    ArduinoVB.QueryVersion()
                    _IsConnect = True
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Carte connectée sur le port:" & ArduinoVB.PortName & " Baud:" & ArduinoVB.Baud)
                    ArduinoVB.DigitalPortReport(0, 1) 'Activer le port0
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start", "Activation du port 0 effectué")
                    ArduinoVB.DigitalPortReport(1, 1) 'Activer le port1
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start", "Activation du port 1 effectué")
                    'Pin0 à 6 définie en entrée
                    For i As Integer = 2 To 6
                        ArduinoVB.PinMode(i, Firmata.FirmataVB.INPUT)
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start", "Pin" & i & " définie en entrée")
                    Next
                    'Pin 7 à 12 définie en sortie
                    For i As Integer = 7 To 12
                        ArduinoVB.PinMode(i, Firmata.FirmataVB.OUTUPT)
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start", "Pin" & i & " définie en sortie")
                    Next
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Le driver n'a pas réussit à se connecter à la carte")
                    _IsConnect = False
                    Exit Sub
                End If
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

    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        'cree l'objet
        Try
            'RemoveHandler Arduino.DigitalDataReceived, AddressOf ArduinoDigitalData
            'RemoveHandler Arduino.AnalogDataReceived, AddressOf ArduinoAnalogData
            'RemoveHandler Arduino.LogMessageReceived, AddressOf WriteLog
            'RemoveHandler Arduino.ConnectionLost, AddressOf ConnectionLost
            ArduinoVB.Disconnect()
            'Arduino = Nothing
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

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        If _Enable = False Then Exit Sub
        If _IsConnect = False Then Exit Sub
        Try
            If Objet.type = "APPAREIL" Then
                If Commande = "ON" Then
                    ArduinoVB.DigitalWrite(Objet.Adresse1, 1)
                End If
                If Commande = "OFF" Then
                    ArduinoVB.DigitalWrite(Objet.Adresse1, 0)
                End If
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Write", "Impossible d'écrire sur un device autre que de type APPAREIL")
            End If
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
    Private Sub Add_LibelleDriver(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String)
        Try
            Dim y0 As New HoMIDom.HoMIDom.Driver.cLabels
            y0.LabelChamp = Labelchamp
            y0.NomChamp = UCase(Nom)
            y0.Tooltip = Tooltip
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
    Private Sub Add_LibelleDevice(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String)
        Try
            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            ld0.LabelChamp = Labelchamp
            ld0.NomChamp = UCase(Nom)
            ld0.Tooltip = Tooltip
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

    Public Sub New()
        _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
        _DeviceSupport.Add(ListeDevices.CONTACT)
        _DeviceSupport.Add(ListeDevices.APPAREIL)

        'ajout des commandes avancées pour les devices
        'Ci-dessous un exemple
        'Dim x As New DeviceCommande
        'x.NameCommand = "Test"
        'x.DescriptionCommand = "Ceci est une commande avancée de test"
        'x.CountParam = 1
        '_DeviceCommandPlus.Add(x)
    End Sub
#End Region

#Region "Fonctions propres au driver"
    'Reception pin digital a changé
    Private Sub FirmataVB1_DigitalMessageReceieved(ByVal portNumber As Integer, ByVal portData As Integer) Handles ArduinoVB.DigitalMessageReceieved
        Try
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DigitalMessageRecu", "PortNumber:" & portNumber & " Value:" & portData)
            Select Case portNumber
                Case 0 'Normal sur le port 0 les pins 2 à 6 sont en entrées
                    For i As Integer = 2 To 6
                        traitement(ArduinoVB.DigitalRead(i), i, 0)
                    Next
                Case 1 'Pas Normal sur le port 1 les pins 7 à 12 sont en sorties
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DigitalMessageRecu", "Le port 1 est paramétré en sortie donc rien à traiter")
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " DigitalMessageReceieved", "Erreur : " & ex.ToString)
        End Try
    End Sub

    'Reception de la version
    Private Sub FirmataVB1_VersionInfoReceieved(ByVal majorVersion As Integer, ByVal minorVersion As Integer) Handles ArduinoVB.VersionInfoReceieved
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Version", "Version:" & majorVersion & "." & minorVersion)
    End Sub

    'Reception pin analogique a changé
    Private Sub FirmataVB1_AnalogMessageReceieved(ByVal pin As Integer, ByVal value As Integer) Handles ArduinoVB.AnalogMessageReceieved
        Try
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " AnalogMessageReceieved", "Pin:" & pin & " Value:" & value)
            traitement(value, pin, 2)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " AnalogMessageReceieved", "Erreur : " & ex.ToString)
        End Try
    End Sub

    Delegate Sub ArduinoDigitalDataCallback(ByVal PortNr As Integer, ByVal Value As Integer)
    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As Integer, ByVal adresse As String, ByVal type As Integer)
        Try
            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            Dim _Type As String = ""

            Select Case type
                Case 0 'CONTACT
                    _Type = "CONTACT"
                Case 1 'APPAREIL
                    _Type = "APPAREIL"
                Case 2 'GENERIQUE VALUE
                    _Type = "GENERIQUEVALUE"
                Case Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Le type de device n'appartient pas à ce driver: " & type)
                    Exit Sub
            End Select

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, _Type, Me._ID, True)

            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                'correction valeur pour correspondre au type de value
                'If TypeOf listedevices.Item(0).Value Is Integer Then
                '    If valeur = 1 Then
                '        valeur = 100
                '    ElseIf valeur = 0 Then
                '        valeur = 0
                '    End If
                'End If

                listedevices.Item(0).Value = valeur

            ElseIf (listedevices.Count > 1) Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Plusieurs devices correspondent à : " & adresse & ":" & valeur)
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Device non trouvé : " & adresse & ":" & valeur)

            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & valeur)
        End Try
    End Sub

#End Region


End Class
