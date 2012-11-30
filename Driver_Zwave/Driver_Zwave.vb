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
        <NonSerialized()> Private Shared m_nodeList As New List(Of Node)



        Private m_nodesReady As Boolean = False
        Private m_homeId As UInt32 = 0

        'parametres avancés
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

        ' Defini le niveau des messages d'erreurs/informations de la log OpenZWave
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

        ' Denition d'un noeud Zwave 
        <Serializable()> Public Class Node

            Dim m_id As Byte = 0
            Dim m_homeId As UInt32 = 0
            Dim m_name As String = ""
            Dim m_location As String = ""
            Dim m_label As String = ""
            Dim m_manufacturer As String = ""
            Dim m_product As String = ""
            Dim m_values As New List(Of ZWValueID)


            Public Property ID() As Byte
                Get
                    Return m_id
                End Get
                Set(ByVal value As Byte)
                    m_id = value
                End Set
            End Property

            Public Property HomeID() As UInt32
                Get
                    Return m_homeId
                End Get
                Set(ByVal value As UInt32)
                    m_homeId = value
                End Set
            End Property

            Public Property Name() As String
                Get
                    Return m_name
                End Get
                Set(ByVal value As String)
                    m_name = value
                End Set
            End Property

            Public Property Location() As String
                Get
                    Return m_location
                End Get
                Set(ByVal value As String)
                    m_location = value
                End Set
            End Property

            Public Property Label() As String
                Get
                    Return m_label
                End Get
                Set(ByVal value As String)
                    m_label = value
                End Set
            End Property

            Public Property Manufacturer() As String
                Get
                    Return m_manufacturer
                End Get
                Set(ByVal value As String)
                    m_manufacturer = value
                End Set
            End Property

            Public Property Product() As String
                Get
                    Return m_product
                End Get
                Set(ByVal value As String)
                    m_product = value
                End Set
            End Property

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

            Sub AddValue(ByVal valueID As ZWValueID)
                m_values.Add(valueID)
            End Sub

            Sub RemoveValue(ByVal valueID As ZWValueID)
                m_values.Remove(valueID)
            End Sub

             Sub SetValue(ByVal valueID As ZWValueID)
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
        End Class


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

        ''' <summary>Execute une commande avancée</summary>
        ''' <param name="MyDevice">Objet représentant le Device </param>
        ''' <param name="Command">Nom de la commande avancée à éxécuter</param>
        ''' <param name="Param">tableau de paramétres</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
            Dim retour As Boolean = False
            Dim NodeTemp As New Node
            ' Permet de construire le message à afficher dans la console
            Dim texteCommande As String

            Try
                If MyDevice IsNot Nothing Then
                    'Pas de commande demandée donc erreur
                    If Command = "" Then
                        Return False
                    Else
                        texteCommande = UCase(Command)

                        ' Traitement de la commande 
                        Select Case UCase(Command)

                            Case "ALL_LIGHT_ON"
                                m_manager.SwitchAllOn(m_homeId)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Passage par la commande ALL_LIGHT_ON ")

                            Case "ALL_LIGHT_OFF"
                                m_manager.SwitchAllOff(m_homeId)
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Passage par la commande ALL_LIGHT_OFF")

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
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Passage par la commande REQUESTNODESTATE = " & NodeTemp.Name)

                            Case "SETCONFIGPARAM"
                                NodeTemp = GetNode(m_homeId, MyDevice.Adresse1)
                                m_manager.SetConfigParam(m_homeId, NodeTemp.ID, Param(0), Param(1))
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Passage par la commande SETCONFIGPARAM pour le noeud " & NodeTemp.Name)

                            Case Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "La commande " & texteCommande & " n'existe pas")
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
                _AFFICHELOG = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave", "Erreur dans les paramétres avancés. utilisation des valeurs par défaut" & ex.Message)
            End Try

            'ouverture du port suivant le Port Com
            Try
                If _Com <> "" Then
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "Demarrage du pilote, ceci peut prendre plusieurs secondes")
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
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "Port " & _Com & " ouvert ")

                        ' Affichage des Informations Controleur du controleur
                        Dim NodeControler As Byte = m_manager.GetControllerNodeId(m_homeId)
                        Dim IsPrimSUC As String = ""
                        If m_manager.IsPrimaryController(m_homeId) Then IsPrimSUC = "Primaire" Else IsPrimSUC = "Secondaire"
                        If m_manager.IsStaticUpdateController(m_homeId) Then IsPrimSUC = IsPrimSUC & "(SUC)" Else IsPrimSUC = IsPrimSUC & " (SIS)"
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "    * Home ID : 0x" & Convert.ToString(m_homeId, 16).ToUpper & " Nombre de noeuds : " & m_nodeList.Count.ToString)
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "    * Mode    : " & IsPrimSUC)
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "    * ID  Controleur  : " & NodeControler & vbTab & "Nom Controleur :" & m_manager.GetNodeName(m_homeId, NodeControler))
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "    * Marque/modele   : " & m_manager.GetNodeManufacturerName(m_homeId, NodeControler) & vbTab & m_manager.GetNodeProductName(m_homeId, NodeControler))
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "    * Type Controleur : " & m_manager.GetLibraryTypeName(m_homeId) & vbTab & " Biblio Version : " & m_manager.GetLibraryVersion(m_homeId))
                        ' Si le controleur a des noeuds d'associés
                        If m_nodeList.Count Then
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "    * Noeuds: ")
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "          Id:Nom " & vbTab & vbTab & "type" & vbTab & vbTab & "Constr./Modèle" & vbTab & "Version" & vbTab & " Endormi ? ")
                            ' Affichages des informations de chaque noeud 
                            Dim IsSleeping As String = ""
                            Dim NodeTempID As Byte
                            For Each NodeTemp As Node In m_nodeList
                                NodeTempID = NodeTemp.ID
                                If m_manager.IsNodeListeningDevice(m_homeId, NodeTempID) Then IsSleeping = "à l'écoute" Else IsSleeping = "Endormi"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Z-Wave", "      -> " & NodeTempID & " : " & NodeTemp.Name & vbTab & m_manager.GetNodeType(m_homeId, NodeTempID) & vbTab & NodeTemp.Manufacturer & "/" & NodeTemp.Product & vbTab & m_manager.GetNodeVersion(m_homeId, NodeTemp.ID) & vbTab & IsSleeping)
                            Next
                        End If
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

            Dim NodeTemp As Node
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

                ' Recuperation du noeud a partir de son ID contenu dans Adresse1
                NodeTemp = GetNode(m_homeId, Objet.Adresse1)
                ' Envoi d'une commande de recuperation de l'etat du noeud 
                m_manager.RequestNodeState(m_homeId, NodeTemp.ID)

                ' Traitement du type de Données à recuperer 
                Dim LabelInfo As String = Objet.Adresse2 ' L'info contient le nom du label.
                If (LabelInfo = "") Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Read", "Erreur: le label du device (Adresse2) " & Objet.Adresse2 & " n'est pas défini.")
                    Exit Sub
                End If

                'Si le noeud n'existe pas dans le controleur
                If (IsNothing(NodeTemp)) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ZWave Read", "Erreur: l'adresse du device (Adresse1) " & Objet.Adresse1 & " n'existe pas")
                Else
                    If Objet IsNot Nothing Then
                        ValeurTemp = GetValueID(NodeTemp, LabelInfo.ToString)
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
                            ' Affichage d'information pour le debugage
                            If _DEBUG Then
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
                            ' Suppression de la valeur lue
                            NodeTemp.Values.Remove(ValeurTemp)
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
                    texteCommande = UCase(Commande)

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
                _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
                _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
                _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
                _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
                _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
                _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
                _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
                _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
                _DeviceSupport.Add(ListeDevices.TELECOMMANDE.ToString)
                _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
                _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)

                'Paramétres avancés
                Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
                Add_ParamAvance("AfficheLog", "Afficher Log OpenZwave à l'écran (True/False)", True)

                'ajout des commandes avancées pour les devices
                Add_DeviceCommande("ALL_LIGHT_ON", "", 0)
                Add_DeviceCommande("ALL_LIGHT_OFF", "", 0)
                Add_DeviceCommande("SetName", "Nom du composant", 0)
                Add_DeviceCommande("GetName", "Nom du composant", 0)
                ' Add_DeviceCommande("RequestNodeState", "Nom du composant", 0)
                Add_DeviceCommande("SetConfigParam", "paramètre de configuration - Par1 : Index - Par2 : Valeur", 2)

                'Libellé Driver
                Add_LibelleDriver("HELP", "Aide...", "Ce module permet de recuperer les informations delivrées par la controleur Z-Wave ")

                'Libellé Device
                Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant de Z-Wave")
                Add_LibelleDevice("ADRESSE2", "Label de la donnée", "Temperature, Relative Humidity, Battery Level")
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

                            m_options.Create(MyRep & "\Drivers\Zwave\", MyRep & "\Drivers\Zwave\", "")  ' repertoire de sauvegarde de la log et config 
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
                            m_options.AddOptionBool("AppendLogFile", False)                      ' Remplace le fichier de log (pas d'append)   
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
                        _IsConnect = False
                        Try
                            If m_nodeList.Count Then
                                For Each node As Node In m_nodeList : m_nodeList.Remove(node)
                                Next
                            End If
                        Catch ex As UnauthorizedAccessException
                            Return ("Probleme lors de la suppression des noeuds" & m_nodeList.Count)
                        End Try
                        m_manager.RemoveDriver("\\.\" & _Com)
                        m_manager.Destroy()
                        m_options.Destroy()
                        port.Dispose()
                        port.Close()
                        Return ("Port " & _Com & " fermé")
                    Else
                        Return ("Port " & _Com & " n'existe pas")
                    End If
                Else
                    Return ("Port " & _Com & "  est déjà fermé (port_ouvert=false)")
                End If
            Catch ex As UnauthorizedAccessException
                Return ("ERR: Port " & _Com & " IGNORE")
            Catch ex As Exception
                Return ("ERR: Port " & _Com & " IGNORE: " & ex.Message)
            End Try
        End Function

        ''' <summary>Reset le controleur Z-Wave avec les parametres d'Usine</summary>
        ''' <remarks></remarks>
        Sub ResetControler()
            Try
                'Dim RetBox As Byte = MsgBox("Est vous sur de vouloir reseter votre controleur ?" & vbLf & "Ceci va supprimer toute votre configuration", vbQuestion + vbSystemModal + vbYesNo, "Reset Usine du controleur")
                'If RetBox = 6 Then m_manager.ResetController(m_homeId)
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " ResetControler ", "Reset du controleur, ceci va supprimer toute votre configuration")
                m_manager.ResetController(m_homeId)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ResetControler ", "Exception: " & ex.Message)
            End Try
        End Sub

        ' Sub GetNodeNeighbors()
        'Dim Retour(29) As Byte
        '   m_manager.GetNodeNeighbors(m_homeId, 1, Retour(0))
        ' Console.WriteLine("Retour de get Neighbors : " & Retour.ToString)
        'End Sub

        '<summary>Handler lors d'une reception d'une trame</summary>
        '<param name="m_notification"></param>
        Sub NotificationHandler(ByVal m_notification As ZWNotification)
            Try
                If m_notification Is Nothing Then Exit Sub
                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", "Une notification a été reçue:")

                Select Case m_notification.[GetType]()
                    Case ZWNotification.Type.ValueAdded
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - ValueAdded")
                        Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                        If Not IsNothing(node) Then node.AddValue(m_notification.GetValueID())

                    Case ZWNotification.Type.ValueRemoved
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - ValueRemoved")
                        Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                        If Not IsNothing(node) Then node.RemoveValue(m_notification.GetValueID())

                    Case ZWNotification.Type.ValueChanged
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - ValueChanged")
                        Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                        If IsNothing(node) Then node.SetValue(m_notification.GetValueID())

                    Case ZWNotification.Type.Group
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - Group")

                    Case ZWNotification.Type.NodeAdded
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - NodeAdded")
                        ' Ajoute une nouveau noeud à notre liste
                        Dim node As New Node()
                        node.ID = m_notification.GetNodeId()
                        ' Si ce n'est pas le controleur 
                        If node.ID <> m_manager.GetControllerNodeId(m_homeId) Then
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", "Ajout d'un nouveau noeud")
                            node.HomeID = m_notification.GetHomeId()
                            m_nodeList.Add(node)
                            If _DEBUG Then
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - ID: " & node.ID & ", Nom: " & node.Name & ", Manufacturer: " & node.Manufacturer)
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - Produit: " & node.Product & ", Label : " & node.Label)
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - Nb noeuds: " & m_nodeList.Count & ", Location: " & node.Location & ", Nb values: " & node.Values.Count)
                            End If
                        End If

                    Case ZWNotification.Type.NodeRemoved
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - NodeRemoved")
                        For Each node As Node In m_nodeList
                            If node.ID = m_notification.GetNodeId() Then
                                m_nodeList.Remove(node)
                                Exit For
                            End If
                        Next

                    Case ZWNotification.Type.NodeProtocolInfo
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - NodeProtocolInfo")
                        Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                        If Not IsNothing(node) Then
                            node.Label = m_manager.GetNodeType(m_homeId, node.ID)
                        End If

                    Case ZWNotification.Type.NodeNaming
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - NodeNaming")
                        Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                        If Not IsNothing(node) Then
                            node.Manufacturer = m_manager.GetNodeManufacturerName(m_homeId, node.ID)
                            node.Product = m_manager.GetNodeProductName(m_homeId, node.ID)
                            node.Location = m_manager.GetNodeLocation(m_homeId, node.ID)
                            node.Name = m_manager.GetNodeName(m_homeId, node.ID)
                        End If

                    Case ZWNotification.Type.NodeEvent
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - NodeEvent")

                    Case ZWNotification.Type.PollingDisabled
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - PollingDisabled")

                    Case ZWNotification.Type.PollingEnabled
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - PollingEnabled")

                    Case ZWNotification.Type.DriverReady
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - DriverReady")
                        m_homeId = m_notification.GetHomeId()

                    Case ZWNotification.Type.NodeQueriesComplete
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - NodeQueriesComplete")

                    Case ZWNotification.Type.AllNodesQueried
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - AllNodesQueried")
                        ' Pour simulation de noeuds
                        ' Dim nodeTempAdd As New Node
                        ' nodeTempAdd.ID = 2
                        ' nodeTempAdd.HomeID = 21816633
                        ' nodeTempAdd.Manufacturer = "Everspring"
                        ' nodeTempAdd.Product = "ST814 Temperature and Humidity Sensor"
                        ' nodeTempAdd.Label = "Temperature and Humidity Sensor"
                        ' nodeTempAdd.Name = "Capteur Chambre"
                        ' m_nodeList.Add(nodeTempAdd)

                        ' Dim nodeTempAdd2 As New Node
                        ' nodeTempAdd2.ID = 3
                        ' nodeTempAdd2.HomeID = 21816633
                        ' nodeTempAdd2.Manufacturer = "Everspring-DEFRE"
                        ' nodeTempAdd2.Product = "ST99 Temperature"
                        ' nodeTempAdd2.Label = "Temperature and Humidity Sensor"
                        'nodeTempAdd2.Name = "Capteur bureau"
                        ' m_nodeList.Add(nodeTempAdd2)
                        m_nodesReady = True

                    Case ZWNotification.Type.AwakeNodesQueried
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", " - AwakeNodesQueried")

                End Select
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " NotificationHandler ", "Exception: " & ex.Message)
            End Try
        End Sub

        '<summary>Gets a node based on the homeId and the nodeId</summary>
        '<param name="homeId"></param>
        '<param name="nodeId"></param>
        '<returns>Node</returns>
        Private Shared Function GetNode(ByVal homeId As UInt32, ByVal nodeId As Byte) As Node
            Try
                For Each node As Node In m_nodeList
                    If (node.ID = nodeId) And (node.HomeID = homeId) Then Return node
                Next
            Catch ex As Exception
                '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Z-Wave GetNode ", "Exception: " & ex.Message)
            End Try
            Return Nothing
        End Function

        '<summary>Gets a Node value ID</summary>
        '<param name="node"></param>
        '<param name="valueLabel"></param>
        '<returns>ValueId</returns>
        Function GetValueID(ByVal node As Node, ByVal valueLabel As String) As ZWValueID
            Try
                For Each valueID As ZWValueID In node.Values
                    If (valueID.GetNodeId = node.ID) And (m_manager.GetValueLabel(valueID) = valueLabel) Then Return valueID
                Next
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GetValueID ", "Exception: " & ex.Message)
            End Try
            Return Nothing
        End Function

#End Region

    End Class

End Class
