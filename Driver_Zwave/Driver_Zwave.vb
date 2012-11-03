Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports OpenZWaveDotNet
Imports System.ComponentModel


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
        Dim _IdSrv As String
        Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

        Private m_manager As New ZWManager
        Private m_options As New ZWOptions
        Private m_notification As ZWNotification
        Private Shared m_nodeList As New BindingList(Of Node)()

        Private m_nodesReady As Boolean = False
        Private m_homeId As UInt32 = 0

        'param avancé
        Dim _DEBUG As Boolean = False
        Dim _AFFICHELOG As Boolean = False

        'Ajoutés dans les ppt avancés dans New()


#End Region

#Region "Variables Internes"
        ' Variables de gestion du port COM
        Private WithEvents port As New System.IO.Ports.SerialPort
        Private port_name As String = ""
        Dim MyRep As String = System.Environment.CurrentDirectory

        ' -----------------   Ajout des declarations pour OpenZWave
        Private g_initFailed As Boolean = False

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


        Public Structure Node

            Shared m_id As Byte = 0
            Public Property ID() As Byte
                Get
                    Return m_id
                End Get
                Set(ByVal value As Byte)
                    m_id = value
                End Set
            End Property

            Shared m_homeId As UInt32 = 0
            Public Property HomeID() As UInt32
                Get
                    Return m_homeId
                End Get
                Set(ByVal value As UInt32)
                    m_homeId = value
                End Set
            End Property

            Shared m_name As String = ""
            Public Property Name() As String
                Get
                    Return m_name
                End Get
                Set(ByVal value As String)
                    m_name = value
                End Set
            End Property

            Shared m_location As String = ""
            Public Property Location() As String
                Get
                    Return m_location
                End Get
                Set(ByVal value As String)
                    m_location = value
                End Set
            End Property

            Shared m_label As String = ""
            Public Property Label() As String
                Get
                    Return m_label
                End Get
                Set(ByVal value As String)
                    m_label = value
                End Set
            End Property

            Shared m_manufacturer As String = ""
            Public Property Manufacturer() As String
                Get
                    Return m_manufacturer
                End Get
                Set(ByVal value As String)
                    m_manufacturer = value
                End Set
            End Property

            Shared m_product As String = ""
            Public Property Product() As String
                Get
                    Return m_product
                End Get
                Set(ByVal value As String)
                    m_product = value
                End Set
            End Property

            Shared m_values As New List(Of ZWValueID)
            Public Property Values() As List(Of ZWValueID)
                Get
                    Return m_values
                End Get
                Set(value As List(Of ZWValueID))
                    m_values = value
                End Set
            End Property


            Shared Sub New()

            End Sub

            Shared Sub AddValue(ByVal valueID As ZWValueID)
                m_values.Add(valueID)
            End Sub

            Shared Sub RemoveValue(ByVal valueID As ZWValueID)
                m_values.Remove(valueID)
            End Sub

            Shared Sub SetValue(ByVal valueID As ZWValueID)
                Dim valueIndex As Integer = 0
                Dim index As Integer = 0

                While index < m_values.Count
                    If m_values(index).GetId() = valueID.GetId() Then
                        valueIndex = index
                        Exit While
                    End If
                    System.Math.Max(System.Threading.Interlocked.Increment(index), index - 1)
                End While

                If valueIndex >= 0 Then
                    m_values(valueIndex) = valueID
                Else
                    AddValue(valueID)
                End If
            End Sub
        End Structure


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

        Public Function Toto()
            Return Nothing
        End Function

        ''' <summary>Execute une commande avancée</summary>
        ''' <param name="MyDevice">Objet représentant le Device </param>
        ''' <param name="Command">Nom de la commande avancée à éxécuter</param>
        ''' <param name="Param">tableau de paramétres</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
            Dim retour As Boolean = False
            Dim NodeTemp As New Node
            Dim texteCommande As String

            Try
                If MyDevice IsNot Nothing Then
                    'Pas de commande demandée donc erreur
                    If Command = "" Then
                        Return False
                    Else
                        texteCommande = UCase(Command)

                        Select Case UCase(Command)

                            Case "ALL_LIGHT_ON"
                                m_manager.SwitchAllOn(m_homeId)

                            Case "ALL_LIGHT_OFF"
                                m_manager.SwitchAllOff(m_homeId)

                            Case "SETNAME"
                                NodeTemp = GetNode(m_homeId, MyDevice.Adresse1)
                                m_manager.SetNodeName(m_homeId, NodeTemp.ID, MyDevice.Name)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Passage par la commande Set Name avec le nom = " & MyDevice.Name)

                            Case "GETNAME"
                                Dim TempName As String
                                NodeTemp = GetNode(m_homeId, MyDevice.Adresse1)
                                TempName = m_manager.GetNodeName(m_homeId, NodeTemp.ID)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Passage par la commande Get Name avec le nom = " & TempName)

                            Case "REQUESTNODESTATE"
                                NodeTemp = GetNode(m_homeId, MyDevice.Adresse1)
                                m_manager.RequestNodeState(m_homeId, NodeTemp.ID)

                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Passage par la commande RequestNodeState")

                            Case Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "La commande " & UCase(Command) & " n'existe pas")
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

            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _AFFICHELOG = Parametres.Item(1).Valeur

            Catch ex As Exception
                _DEBUG = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave Start", "Erreur dans les paramétres avancés. utilisation des valeurs par défaut" & ex.Message)
            End Try

            'ouverture du port suivant le Port Com
            Try
                If _Com <> "" Then
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Demarrage du pilote Z-Wave, ceci peut prendre plusieurs secondes", retour)
                    retour = ouvrir(_Com)
                    ' ZWave network is started, and our control of hardware can begin once all the nodes have reported in
                    While (m_nodesReady = False And CptBoucle < 10)
                        System.Threading.Thread.Sleep(3000) ' Attente de 3 sec  - Info OpenZWave Wait since this process can take around 20seconds within some network
                        CptBoucle = CptBoucle + 1
                    End While

                    'traitement du message de retour
                    If Not (g_initFailed) Then ' le demarrage le controleur a échoué
                        _IsConnect = False
                        retour = "ERR: Port Com non défini. Impossible d'ouvrir le port !"
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave", "Driver non démarré : " & retour)
                    Else
                        _IsConnect = True
                        retour = "Port " & _Com & " ouvert "
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", retour)
                        ' Debug Information Controleur
                        Dim NodeControler As Byte = m_manager.GetControllerNodeId(m_homeId)

                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "Home ID : 0x" & Convert.ToString(m_homeId, 16).ToUpper)
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "ID Node Controleur : " & NodeControler & " Nom Controleur :" & m_nodeList(NodeControler).Name)
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "Controleur : " & m_manager.GetNodeManufacturerName(m_homeId, NodeControler) & " " & m_manager.GetNodeProductName(m_homeId, NodeControler))
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "Type Controleur : " & m_manager.GetLibraryTypeName(m_homeId) & " Biblio Version : " & m_manager.GetLibraryVersion(m_homeId))

                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "Adresse Node " & vbTab & "  Basic type" & vbTab & vbTab & "  label  ")
                        For i = NodeControler To m_nodeList.Count - 1
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", m_nodeList(i).ID & " - " & m_nodeList(i).Name & vbTab & m_manager.GetNodeType(m_homeId, NodeControler) & " " & m_nodeList(i).Manufacturer & m_nodeList(i).Product & vbTab & vbTab & m_nodeList(i).Label)
                        Next
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
            Dim retour As String
            Try
                If _IsConnect Then
                    retour = fermer()
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave Stop", retour)
                Else
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave Stop", "Port " & _Com & " est déjà fermé")
                End If

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

            Dim NodeTemp As New Node
            Dim IndexTemp As Byte
            Dim ValeurTemp As ZWValueID
            Try
                If _Enable = False Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Read", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                    Exit Sub
                End If

                If _IsConnect = False Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Read", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté")
                    Exit Sub
                End If

                NodeTemp = GetNode(m_homeId, Objet.Adresse1)
                m_manager.RequestNodeState(m_homeId, NodeTemp.ID)
                Console.WriteLine("Valeur de l'info Adresse2 : " & Objet.Adresse2)
                Dim TypeInfo As String = Objet.Adresse2
                If (TypeInfo = "") Then TypeInfo = "Basic"
                Console.WriteLine("Valeur de l'info Adresse2 : " & Objet.Adresse2 & "-" & TypeInfo)

                If IsNothing(NodeTemp) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Read", "Erreur: l'adresse du device (Adresse1) " & Objet.Adresse1 & " n'existe pas")
                Else
                    If Objet IsNot Nothing Then
                        ValeurTemp = GetValueID(NodeTemp, TypeInfo.ToString)
                        If Not (IsNothing(ValeurTemp)) Then   ' Il y a des valeurs pour ce noeud
                            IndexTemp = ValeurTemp.GetIndex()
                            Select Case ValeurTemp.GetType()
                                Case 0 : m_manager.GetValueAsBool(ValeurTemp, Objet.value)
                                Case 1 : m_manager.GetValueAsByte(ValeurTemp, Objet.value)
                                Case 2 : m_manager.GetValueAsDecimal(ValeurTemp, Objet.value)
                                Case 3 : m_manager.GetValueAsInt(ValeurTemp, Objet.value)
                                    '  Case 4 : m_manager.GetValueListItems(NodeTemp.Values(IndexTemp), Objet.value) ; A voir + tard
                                Case 6 : m_manager.GetValueAsShort(ValeurTemp, Objet.value)
                                Case 7 : m_manager.GetValueAsString(ValeurTemp, Objet.value)
                            End Select
                            If _DEBUG Then
                                ' DEBUGLA
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Le nombre de valeurs est de : " & NodeTemp.Values.Count)
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "La valeur n°" & IndexTemp & " a pour Id : " & ValeurTemp.GetId().ToString)
                                Console.WriteLine("NodeID         :" & ValeurTemp.GetNodeId())
                                Console.WriteLine("Node Version   :" & m_manager.GetNodeVersion(m_homeId, NodeTemp.ID))
                                Console.WriteLine("ValueGenre     :" & ValeurTemp.GetGenre())
                                Console.WriteLine("CommandClassId :" & ValeurTemp.GetCommandClassId())
                                Console.WriteLine("ValueIdex      :" & ValeurTemp.GetIndex())
                                Console.WriteLine("ValueType      :" & ValeurTemp.GetType())
                                Console.WriteLine("ValueLabel     :" & m_manager.GetValueLabel(ValeurTemp))
                                Console.WriteLine("ValueUnit      :" & m_manager.GetValueUnits(ValeurTemp))
                                Console.WriteLine("Value relevée :" & Objet.value & " de type " & Objet.GetType.Name)
                            End If
                            NodeTemp.Values.RemoveAt(IndexTemp)
                        Else
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Pas de valeur trouvée")
                            Exit Sub
                        End If

                    End If
                End If


            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave Read", ex.Message)
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
                Dim NodeTemp As New Node
                Dim texteCommande As String

                NodeTemp = GetNode(m_homeId, Objet.Adresse1)

            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Write", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Write", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté")
                Exit Sub
            End If

            If IsNumeric(Objet.Adresse1) = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Write", "Erreur: l'adresse du device (Adresse1) " & Objet.Adresse1 & " n'est pas une valeur numérique")
                Exit Sub
            End If

                If Objet.Type = "LAMPE" Or Objet.Type = "APPAREIL" Then
                    TexteCommande = UCase(Commande)
                    Select Case UCase(Commande)

                        Case "ON"
                            m_manager.SetNodeOn(m_homeId, NodeTemp.ID)

                        Case "OFF"
                            m_manager.SetNodeOff(m_homeId, NodeTemp.ID)

                        Case "DIM"
                            If Not (IsNothing(Parametre1)) Then
                                Dim ValDimmer As Byte = Math.Round(Parametre1 * 2.55) ' Reformate la valeur entre 0 : OFF  et 255 :ON 
                                m_manager.SetNodeLevel(m_homeId, NodeTemp.ID, ValDimmer)
                                texteCommande = texteCommande & " avec le % = " & Val(Parametre1) & " - " & ValDimmer
                            End If

                    End Select
                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & "ZWave Write", "Passage par la commande " & texteCommande)
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Write", "Erreur: Le type " & Objet.Type.ToString & " à l'adresse " & Objet.Adresse1 & " n'est pas compatible")
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
                _DeviceSupport.Add(ListeDevices.APPAREIL) 'SORTIE     
                _DeviceSupport.Add(ListeDevices.LAMPE) 'SORTIE     
                _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN) 'ENTREE
                _DeviceSupport.Add(ListeDevices.GENERIQUESTRING) 'ENTREE
                _DeviceSupport.Add(ListeDevices.SWITCH) 'ENTREE
                _DeviceSupport.Add(ListeDevices.CONTACT) 'ENTREE   

                'Paramétres avancés
                Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
                Add_ParamAvance("AfficheLog", "Afficher Log à l'écran (True/False)", True)

                'ajout des commandes avancées pour les devices
                Add_DeviceCommande("ALL_LIGHT_ON", "", 0)
                Add_DeviceCommande("ALL_LIGHT_OFF", "", 0)
                Add_DeviceCommande("SetName", "Nom du composant", 0)
                Add_DeviceCommande("GetName", "Nom du composant", 0)
                Add_DeviceCommande("RequestNodeState", "Nom du composant", 0)

                'Libellé Driver
                Add_LibelleDriver("HELP", "Aide...", "Ce module permet de recuperer les informations delivrées par la controleur Z-Wave ")

                'Libellé Device
                Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant de Z-Wave")
                Add_LibelleDevice("ADRESSE2", "Type de la donnée", "Basic, User, Config, System, Count")
                Add_LibelleDevice("SOLO", "@", "")
                Add_LibelleDevice("MODELE", "@", "")
                Add_LibelleDevice("REFRESH", "Refresh", "")
                Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")


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

        '-----------------------------------------------------------------------------
        ''' <summary>Ouvrir le port Z-Wave</summary>
        ''' <param name="numero">Nom/Numero du port COM: COM2</param>
        ''' <remarks></remarks>
        Private Function ouvrir(ByVal numero As String) As String
            Try
                'ouverture du port
                If Not _IsConnect Then
                    Try
                        ' Test d'ouveture du port Com du controleur 
                        port.PortName = numero
                        port.Open()
                        ' Le port existe ==> le controleur est present
                        If port.IsOpen() Then
                            port.Close()
                            ' Creation du controleur ZWave
                            m_options = New ZWOptions()
                            m_manager = New ZWManager()

                            m_options.Create(MyRep & "\Drivers\Zwave\", MyRep & "\Drivers\Zwave\", "")   ' MyRep & "\Config\Zwave\"
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Ouvrir ", "Le nom du repertoire de config est : " & MyRep & "\drivers\Zwave\")

                            If _DEBUG Then
                                m_options.AddOptionInt("SaveLogLevel", LogLevel.LogLevel_Internal)      ' Configure le niveau de sauvegarde des messages (Disque)
                                m_options.AddOptionInt("QueueLogLevel", LogLevel.LogLevel_Internal)    ' Configure le niveau de  sauvegarde des messages (RAM)
                                m_options.AddOptionInt("DumpTrigger", LogLevel.LogLevel_Internal)
                            Else
                                m_options.AddOptionInt("SaveLogLevel", OpenZWaveDotNet.ZWLogLevel.Info)      ' Configure le niveau de sauvegarde des messages (Disque)
                                m_options.AddOptionInt("QueueLogLevel", OpenZWaveDotNet.ZWLogLevel.Warning)     ' Configure le niveau de  sauvegarde des messages (RAM)
                                m_options.AddOptionInt("DumpTrigger", OpenZWaveDotNet.ZWLogLevel.Info)       ' Internal niveau de debug
                            End If


                            m_options.AddOptionBool("ConsoleOutput", _AFFICHELOG)
                            m_options.AddOptionBool("AppendLogFile", False)                      ' Remplace le fichier (pas d'append)   
                            m_options.AddOptionInt("PollInterval", 500)
                            m_options.AddOptionBool("IntervalBetweenPolls", True)
                            m_options.AddOptionBool("ValidateValueChanges", True)

                            m_options.Lock()
                            m_manager.Create()


                            ' Ajout d'un gestionnaire d'evenements()
                            m_manager.OnNotification = New ManagedNotificationsHandler(AddressOf NotificationHandler)
                            ' Creation du driver - Ouverture du port du controleur
                            g_initFailed = m_manager.AddDriver("\\.\" & numero) ' Return True if driver create
                            Return ("Port " & port_name & " ouvert")
                        Else
                            ' Le port n'existe pas ==> le controleur n'est pas present
                            Return ("Port " & port_name & " fermé")
                        End If


            Catch ex As Exception
                Return ("Port " & port_name & " n'existe pas")
                Exit Function
            End Try

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
                        ' Sauvegarde de la configuration du réseau
                        m_manager.WriteConfig(m_homeId)
                        ' Fermeture du port du controleur 
                        m_manager.RemoveDriver("\\.\" & _Com)
                        m_manager.Destroy()
                        m_options.Destroy()
                        m_nodeList = Nothing

                        port.Dispose()
                        port.Close()

                        _IsConnect = False
                        Return ("Port " & _Com & " fermé")
                        
                    Else
                        Return ("Port " & _Com & " n'existe pas")
                    End If
                Else
                    Return ("Port " & _Com & "  est déjà fermé (port_ouvert=false)")
                End If
            Catch ex As UnauthorizedAccessException
                Return ("ERR: Port " & _Com & " IGNORE")
            End Try

            Return True
        End Function

        Sub tata()

        End Sub

        Sub NotificationHandler(ByVal m_notification As ZWNotification)

            If m_notification Is Nothing Then
                Return
            End If

            Select Case m_notification.[GetType]()

                Case ZWNotification.Type.ValueAdded
                    Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                    If Not IsNothing(node) Then
                        node.AddValue(m_notification.GetValueID())
                    End If

                Case ZWNotification.Type.ValueRemoved
                    Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                    If Not IsNothing(node) Then
                        node.RemoveValue(m_notification.GetValueID())
                    End If



                Case ZWNotification.Type.ValueChanged
                    Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                    If IsNothing(node) Then
                        node.SetValue(m_notification.GetValueID())
                    End If


                Case ZWNotification.Type.Group




                Case ZWNotification.Type.NodeAdded
                    ' Add the new node to our list
                    Dim node As New Node()
                    node.ID = m_notification.GetNodeId()
                    node.HomeID = m_notification.GetHomeId()
                    m_nodeList.Add(node)

                Case ZWNotification.Type.NodeRemoved
                    For Each node As Node In m_nodeList
                        If node.ID = m_notification.GetNodeId() Then
                            m_nodeList.Remove(node)
                            Exit For
                        End If
                    Next

                Case ZWNotification.Type.NodeProtocolInfo
                    Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                    If Not IsNothing(node) Then
                        node.Label = m_manager.GetNodeType(m_homeId, node.ID)
                    End If

                Case ZWNotification.Type.NodeNaming
                    Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                    If Not IsNothing(node) Then
                        node.Manufacturer = m_manager.GetNodeManufacturerName(m_homeId, node.ID)
                        node.Product = m_manager.GetNodeProductName(m_homeId, node.ID)
                        node.Location = m_manager.GetNodeLocation(m_homeId, node.ID)
                        node.Name = m_manager.GetNodeName(m_homeId, node.ID)
                    End If


                Case ZWNotification.Type.NodeEvent


                Case ZWNotification.Type.PollingDisabled


                Case ZWNotification.Type.PollingEnabled

                Case ZWNotification.Type.DriverReady
                    m_homeId = m_notification.GetHomeId()

                Case ZWNotification.Type.NodeQueriesComplete

                Case ZWNotification.Type.AllNodesQueried
                    m_nodesReady = True

                Case ZWNotification.Type.AwakeNodesQueried

            End Select
        End Sub

        '<summary>

        'Gets a node based on the homeId and the nodeId
        '</summary>
        '<param name="homeId"></param>
        '<param name="nodeId"></param>
        '<returns></returns>
        Private Shared Function GetNode(ByVal homeId As UInt32, ByVal nodeId As Byte) As Node

            For Each node As Node In m_nodeList
                If (node.ID = nodeId) AndAlso (node.HomeID = homeId) Then
                    Return node
                End If
            Next
            Return Nothing
        End Function



        Function GetValueID(ByVal node As Node, ByVal valueLabel As String) As ZWValueID

            For Each valueID As ZWValueID In node.Values
                If m_manager.GetValueLabel(valueID) = valueLabel Then
                    Return valueID
                End If
            Next
            Return Nothing
        End Function

#End Region


    End Class

End Class
