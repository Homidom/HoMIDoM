Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports OpenZWaveDotNet


Public Class Driver_ZWave

    ' Auteur : Laurent
    ' Date : 28/02/2012

    ''' <summary>Class Driver_ZWave, permet de communiquer avec le controleur Z-Wave</summary>
    ''' <remarks>Nécessite l'installation des pilotes fournis sur le site </remarks>
    <Serializable()> Public Class Driver_ZWave
        Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
        '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
        'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
        Dim _ID As String = "57BCAA20-5CD2-11E1-AA83-88244824019B"
        Dim _Nom As String = "Z-Wave"
        Dim _Enable As String = False
        Dim _Description As String = "Controleur Z-Wave"
        Dim _StartAuto As Boolean = False
        Dim _Protocol As String = "COM"
        Dim _IsConnect As Boolean = False
        Dim _IP_TCP As String = "@"
        Dim _Port_TCP As String = "@"
        Dim _IP_UDP As String = "@"
        Dim _Port_UDP As String = "@"
        Dim _Com As String = "COM1"
        Dim _Refresh As Integer = 0
        Dim _Modele As String = ""
        Dim _Version As String = "1.0"
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

        Private m_manager As New ZWManager
        Private m_options As New ZWOptions
        Private m_notification As ZWNotification

        Private m_nodesReady As Boolean = False
        Private m_homeId As UInt32 = 0

        'param avancé
        Dim _DEBUG As Boolean = False

        'Ajoutés dans les ppt avancés dans New()


#End Region

#Region "Variables Internes"
        ' Variables de gestion du port COM
        Private WithEvents port As New System.IO.Ports.SerialPort
        Private port_name As String = ""
        Private BufferIn(8192) As Byte
        Private DebutTrame As Boolean = False
        Private DebutInfo As Boolean = False

        Private bytecnt As Integer = 0
        Private messcnt As Integer = 0
        <NonSerialized()> Dim TimerSecond As New Timers.Timer 'Timer à la seconde
        Private recbuf(300), recbytes, recbits As Byte
        Private InfoTrame() As String

        Private mess As Boolean = False
        Private trame As Boolean = False

        ' -----------------   Ajout des declarations pour OpenZWave
        Private Structure NodeInfo
            Dim m_homeId As UInteger
            Dim m_nodeId As Byte
            Dim m_polled As Boolean
            Dim name As String
            Dim location As String
            'Dim m_values As  List()
        End Structure

        Private g_nodes() As NodeInfo
        Private g_homeId As UInteger = 0
        Private g_initFailed As Boolean = False
        Private g_nodesQueried As Boolean = False
        '     Private d_CriticalSection As CRITICAL_SECTION

        Enum LogLevel
            LogLevel_None
            LogLevel_Always
            LogLevel_Fatal
            LogLevel_Error
            LogLevel_Warning
            LogLevel_Alert
            LogLevel_Info
            LogLevel_Detail
            LogLevel_Debug
            LogLevel_Internal
        End Enum


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
                If value >= 1 Then
                    _Refresh = value

                End If
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
                        ' Pour le Debug MyDevice,
                        Console.WriteLine(Command, Param(0), Param(1))
                        Select Case UCase(Command)
                            Case "SEARCHNODES"
                                Console.WriteLine("Passage par la commande de recherche d'un device")

                            Case "DIM"
                                Console.WriteLine("Passage par la commande Dumming avec le % = " & Val(Param(0)))

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


                End Select
                Return retour
            Catch ex As Exception
                Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
            End Try
        End Function

        ''' <summary>Démarrer le du driver</summary>
        ''' <remarks></remarks>
        Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
            Dim retour As String = ""
            Dim CptBoucle As Byte

            'ouverture du port suivant le Port Com
            Try
                If _Com <> "" Then
                    ' retour = ouvrir(_Com)

                    Console.WriteLine("Ouverture du port du controleur Z-Wave : " & _Com)
                    m_options.Create(".\Config\Zwave\config\", "LogZWave.log", "")
                    m_options.AddOptionInt("SaveLogLevel", LogLevel.LogLevel_Error)
                    m_options.AddOptionInt("QueueLogLevel", LogLevel.LogLevel_Error)
                    m_options.AddOptionInt("DumpTrigger", LogLevel.LogLevel_Error)
                    m_options.AddOptionInt("PollInterval", 500)
                    m_options.AddOptionBool("IntervalBetweenPolls", True)
                    m_options.AddOptionBool("ValidateValueChanges", True)
                    m_options.Lock()

                    m_manager.Create()
                    m_manager.OnNotification = (NotificationHandler(m_notification))
                    ' Add an event handler for all the Z-Wave notifications

                    ' Add a driver, this will start up the z-wave network
                    m_manager.AddDriver("\\.\" & _Com)
                    ' ZWave network is started, and our control of hardware can begin once all the nodes have reported in
                    While (m_nodesReady = False And CptBoucle < 3)
                        Console.WriteLine("Waiting loop " & CptBoucle & "  - The nodes all need to report in")
                        System.Threading.Thread.Sleep(3000) ' Attente de 3 sec  - Info OpenZWave Wait since this process can take around 20seconds within my network
                        CptBoucle = CptBoucle + 1
                    End While

                    ' Recuperation de la variable HomeId du controleur
                    ' m_manager.GetControllerNodeId(g_homeId)
                    Console.WriteLine("Configuration détectée avec le HomeId = " & g_homeId)

                    'traitement du message de retour
                    If g_initFailed Then
                        _IsConnect = False

                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave", "Driver non démarré : " & retour)
                    Else
                        _IsConnect = True
                        retour = "Port " & _Com & " ouvert"
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", retour)
                    End If
                Else
                    retour = "ERR: Port Com non défini. Impossible d'ouvrir le port !"
                End If


            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave Start", ex.Message)
                _IsConnect = False

            End Try



        End Sub

        ''' <summary>Arrêter le du driver</summary>
        ''' <remarks></remarks>
        Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave Stop", ex.Message)
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

                If Objet IsNot Nothing Then
                    Select Case Objet.Type

                        Case Else

                    End Select

                End If


            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Read", ex.Message)
            End Try
        End Sub

        ''' <summary>Commander un device</summary>
        ''' <param name="Objet">Objet représetant le device à interroger</param>
        ''' <param name="Commande">La commande à passer</param>
        ''' <param name="Parametre1"></param>
        ''' <param name="Parametre2"></param>
        ''' <remarks></remarks>
        Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
            Try

                If _Enable = False Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Write", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                    Exit Sub
                End If

                If _IsConnect = False Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Write", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté à la carte")
                    Exit Sub
                End If
                If IsNumeric(Objet.Adresse1) = False Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Write", "Erreur: l'adresse du device (Adresse1) " & Objet.Adresse1 & " n'est pas une valeur numérique")
                    Exit Sub
                End If

                Dim adr As Long = Objet.Adresse1

                If Objet.Type = "SWITCH" Or Objet.Type = "APPAREIL" Then
                    If Commande = "ON" Then

                    End If
                    If Commande = "OFF" Then

                    End If
                    'If Commande = "DIM" Then
                    ' Console.WriteLine("Passage par la commande DIM de Write")
                    ' If Parametre1 > 100 Then Parametre1 = 100
                    ' If Parametre1 < 0 Then Parametre1 = 0

                    'SendBasicSwitch(Objet.Adresse1, Val(Parametre1))
                    ' End If
                End If

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave Write", ex.Message)
            End Try
        End Sub

        ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
        ''' <param name="DeviceId">Objet représetant le device à interroger</param>
        ''' <remarks></remarks>
        Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave DeleteDevice", ex.Message)
            End Try
        End Sub

        ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
        ''' <param name="DeviceId">Objet représetant le device à interroger</param>
        ''' <remarks></remarks>
        Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave NewDevice", ex.Message)
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
                ''liste des devices compatibles
                _DeviceSupport.Add(ListeDevices.SWITCH) 'SORTIE
                _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN) 'ENTREE
                _DeviceSupport.Add(ListeDevices.APPAREIL) 'SORTIE
                'Paramétres avancés

                'ajout des commandes avancées pour les devices
                Add_DeviceCommande("DIM", "Valeur en % d'intensité", 1)
                Add_DeviceCommande("SEARCHNODES", "trouver les noeuds du réseau", 0)

                'Libellé Driver
                Add_LibelleDriver("HELP", "Aide...", "Ce module permet de recuperer les informations delivrées par la controleur Z-Wave ")

                'Libellé Device
                Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant de Z-Wave")



            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave New", ex.Message)
            End Try
        End Sub

        ''' <summary>Si refresh >0 gestion du timer</summary>
        ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
        Private Sub TimerTick()

        End Sub

#End Region

#Region "Fonctions internes"
        ''' <summary>Ouvrir le port Z-Wave</summary>
        ''' <param name="numero">Nom/Numero du port COM: COM2</param>
        ''' <remarks></remarks>
        Private Function ouvrir(ByVal numero As String) As String
            Try
                'ouverture du port
                If Not _IsConnect Then


                    'AddHandler port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                    mess = False

                    Return ("Port " & port_name & " ouvert")
                Else
                    Return ("Port " & port_name & " dejà ouvert")
                End If
            Catch ex As Exception
                Return ("ERR: " & ex.Message)
            End Try

        End Function

        ''' <summary>Fermer le port Z-Wave</summary>
        ''' <remarks></remarks>
        Private Function fermer() As String
            Try
                If _IsConnect Then
                    If (Not (port Is Nothing)) Then ' The COM port exists.
                        If port.IsOpen Then
                            port.DiscardOutBuffer()
                            port.Close()
                            port.Dispose()
                            _IsConnect = False
                            Return ("Port " & port_name & " fermé")
                        Else
                            Return ("Port " & port_name & "  est déjà fermé")
                        End If
                    Else
                        Return ("Port " & port_name & " n'existe pas")
                    End If
                Else
                    Return ("Port " & port_name & "  est déjà fermé (port_ouvert=false)")
                End If
            Catch ex As UnauthorizedAccessException
                Return ("ERR: Port " & port_name & " IGNORE")
                ' The port may have been removed. Ignore.
            End Try

            Return True
        End Function




        Private Function GetNode(ByRef homeId As UInt32, ByRef nodeId As Byte) As NodeInfo
            Dim localnode As New NodeInfo

            For Each localnode In g_nodes
                If ((localnode.m_nodeId = nodeId) & (localnode.m_homeId = homeId)) Then

                    Return localnode
                Else
                    Return Nothing
                End If
            Next
            Return localnode
        End Function


        ' Method which handles the events raised by the ZWave network
        ' </summary>
        ' <param name="m_notification"></param>
        Private Function NotificationHandler(ByRef m_notification As ZWNotification)

            Console.WriteLine("Passage par la procedure de gestion des notifications")

            If IsNothing(m_notification) Then
                Console.WriteLine("error parametre vide")
                Return Nothing
            End If

            Select Case (m_notification.GetType())


                Case ZWNotification.Type.ValueAdded
                    Console.WriteLine("Passage par ValueAdded")
                    Dim node As New NodeInfo
                    node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                    If IsNothing(node) Then

                        'node.AddValue(m_notification.GetValueID())
                        Console.WriteLine("Node ValueAdded, " + m_manager.GetValueLabel(m_notification.GetValueID()))
                    End If

            End Select

            Return Nothing

        End Function


#End Region


    End Class

End Class
