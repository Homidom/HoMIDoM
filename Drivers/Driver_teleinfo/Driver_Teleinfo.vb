Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO.Ports
Imports VB = Microsoft.VisualBasic


Public Class Driver_Teleinfo

    ' Auteur : Laurent
    ' Date : 11/01/2012  Creation du driver
    ' Date : 06/04/203   Ajout de la gestion des MultiCompteurs

    ''' <summary>Class Driver_Teleinfo, permet de communiquer avec le module USB de A DAUGUET</summary>
    ''' <remarks>Nécessite l'installation des pilotes fournis sur le site </remarks>
    <Serializable()> Public Class Driver_Teleinfo
        Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
        '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
        'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
        Dim _ID As String = "3BB1F870-3A41-11E1-B86C-0800200C9A66"
        Dim _Nom As String = "Teleinfo"
        Dim _Enable As Boolean = False
        Dim _Description As String = "Adaptateur USB DAUGUET"
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
        Dim _Picture As String = "compteur-monophase.png"
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

		'Ajoutés dans les ppt avancés dans New()

        Dim _DEBUG As Boolean = False
        Dim _SecondPort As String = ""
        Dim _BaudRate As Integer = 1200

		#End Region

#Region "Variables Internes"

        ' Definition de tous les labels Teleinfo possibles 
        Public Enum Labels As SByte
            Unknow = -1
            ADCO = 0
            OPTARIF
            ISOUSC
            HCHC
            HCHP
            BASE
            PTEC
            PEJP
            IMAX
            PAPP
            HHPHC
            IINST
            MOTDETAT
            IINST1
            IINST2
            IINST3
            IMAX1
            IMAX2
            IMAX3
            PMAX
            PPOT
        End Enum

        Dim TabCom As String() = {"COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "NOTHING"}

        Dim TabLabels([Enum].GetValues(GetType(Labels)).Length) As String

        ' Definition d'une Classe Compteur TeleInfo 
        <Serializable()> Public Class TeleInfo
            Private m_port_name As String
            Private m_DebutTrame As Boolean = False
            Private m_bytecnt As Integer = 0
            Private m_messcnt As Integer = 0

            Private m_InfoTrame As New List(Of String)
            Private m_recbuf As New List(Of Byte)

            Private m_mess As Boolean = False
            Private m_trame As Boolean = False
            Private m_IsConnectPort As Boolean = False
            Private WithEvents m_SerialPort As New System.IO.Ports.SerialPort
            Dim m_TabLabels As New List(Of String)

            '  ([Enum].GetValues(GetType(Labels)).Length))  

            Public Property port_name() As String
                Get
                    Return m_port_name
                End Get
                Set(ByVal value As String)
                    m_port_name = value
                End Set
            End Property

            Public Property DebutTrame() As Boolean
                Get
                    Return m_DebutTrame
                End Get
                Set(ByVal value As Boolean)
                    m_DebutTrame = value
                End Set
            End Property

            Public Property bytecnt() As Integer
                Get
                    Return m_bytecnt
                End Get
                Set(ByVal value As Integer)
                    m_bytecnt = value
                End Set
            End Property

            Public Property messcnt() As Integer
                Get
                    Return m_messcnt
                End Get
                Set(ByVal value As Integer)
                    m_messcnt = value
                End Set

            End Property

            Public Property recbuf As List(Of Byte)
                Get
                    Return m_recbuf
                End Get
                Set(ByVal value As List(Of Byte))
                    m_recbuf = value
                End Set
            End Property

            Public Property mess() As Boolean
                Get
                    Return m_mess
                End Get
                Set(ByVal value As Boolean)
                    m_mess = value
                End Set
            End Property

            Public Property InfoTrame As List(Of String)
                Get
                    Return m_InfoTrame
                End Get
                Set(ByVal value As List(Of String))
                    m_InfoTrame = value
                End Set
            End Property

            Public Property trame() As Boolean
                Get
                    Return m_trame
                End Get
                Set(ByVal value As Boolean)
                    m_trame = value
                End Set
            End Property

            Public Property IsConnectPort() As Boolean
                Get
                    Return m_IsConnectPort
                End Get
                Set(ByVal value As Boolean)
                    m_IsConnectPort = value
                End Set
            End Property

            Public Property SerialPort() As System.IO.Ports.SerialPort
                Get
                    Return m_SerialPort
                End Get
                Set(ByVal value As System.IO.Ports.SerialPort)
                    m_SerialPort = value
                End Set

            End Property

            Public Property TabLabels As List(Of String)
                Get
                    Return m_TabLabels
                End Get
                Set(ByVal value As List(Of String))
                    m_TabLabels = value
                End Set
            End Property
            Sub New()
                For Cpt = 0 To ([Enum].GetValues(GetType(Labels)).Length)
                    TabLabels.Add("")
                Next
            End Sub

        End Class

        ' Tableau contenant les differents compteurs 
        Dim TabCompteur(0) As TeleInfo

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
        ''' 
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
            Dim retour2 As String = ""

            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _SecondPort = _Parametres.Item(1).Valeur.ToString.ToUpper
                _BaudRate = _Parametres.Item(2).Valeur.ToString.ToUpper

                If TabCom.Contains(_SecondPort) Then
                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start ", "Valeur de SecondPort valide  : " & _SecondPort.ToUpper)
                Else
                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start ", "Valeur de SecondPort invalide  : " & _SecondPort.ToUpper & vbCrLf & "Valeur positionnée à NOTHING")
                    _SecondPort = "NOTHING"
                End If

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
            End Try

            Dim TempCompteur As New TeleInfo
            Dim TempCompteur2 As New TeleInfo

            'ouverture du port suivant le Port Com
            Try
                If _Com <> "" Then
                    TempCompteur.port_name = _Com.ToUpper
                    TabCompteur(0) = TempCompteur
                    retour = ouvrir(TabCompteur(0))
                Else
                    retour = "ERR: Port Com non défini. Impossible d'ouvrir le port !"
                End If
                'ouverture du port si un Second port est defini
                If _SecondPort.ToLower <> "nothing" Then
                    If _SecondPort.ToUpper = _Com.ToUpper Then
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Le second Port " & _SecondPort.ToUpper & " ne doit pas etre identique avec le port Principal.")
                        _SecondPort = "nothing"
                    Else
                        TempCompteur2.port_name = _SecondPort.ToUpper
                        ReDim Preserve TabCompteur(TabCompteur.Count)
                        TabCompteur(1) = TempCompteur2
                        retour2 = ouvrir(TabCompteur(1))
                    End If

                End If

                'traitement des messages de retour
                If (STRGS.Left(retour, 4) = "ERR:") Or (STRGS.Left(retour2, 4) = "ERR:") Then
                    If (STRGS.Left(retour, 4) = "ERR:") Then
                        TabCompteur(0).IsConnectPort = False
                        _IsConnect = False
                    End If
                    retour = STRGS.Right(retour, retour.Length - 5)
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo", "Driver non démarré : " & retour)

                    If _SecondPort.ToLower <> "nothing" Then
                        TabCompteur(1).IsConnectPort = False
                        retour2 = STRGS.Right(retour, retour.Length - 5)
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo", "Driver non démarré : " & retour2)
                    End If
                Else
                    _IsConnect = True
                    TabCompteur(0).IsConnectPort = True
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "TeleInfo", retour)
                    If _SecondPort.ToLower <> "nothing" Then
                        TabCompteur(1).IsConnectPort = True
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "TeleInfo", retour2)
                    End If

                End If
            Catch ex As Exception
                _IsConnect = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Start", ex.Message)
                TabCompteur(0).IsConnectPort = False
                If _SecondPort.ToLower <> "nothing" Then TabCompteur(1).IsConnectPort = False
            End Try

        End Sub

        ''' <summary>Arrêter le du driver</summary>
        ''' <remarks></remarks>
        Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
            Dim retour As String

            ' Recherche de tous les ports ouverts
            Try
                Dim index As Byte = 0
                For Each TeleInfoCpt In TabCompteur
                    If TeleInfoCpt.IsConnectPort Then
                        retour = fermer(TeleInfoCpt)
                        Array.Clear(TabCompteur, index, 1)
                        index += 1
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "TeleInfo Stop", retour)
                    Else
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "TeleInfo Stop", "Port " & _Com & " est déjà fermé")
                    End If
                Next
                ' Effacement du tableau des compteurs
                Array.Resize(TabCompteur, 1)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Stop", ex.Message)
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
                        Case "ENERGIETOTALE", "ENERGIEINSTANTANEE"
                            Dim retour As Double = Val(Sauve_temp_teleinfo(LTrim(Objet.Adresse1), Objet.Adresse2))
                            If retour <> 9999 Then Objet.Value = retour

                        Case "GENERIQUESTRING"
                            Dim retour As String = Sauve_temp_teleinfo(LTrim(Objet.Adresse1), Objet.Adresse2)
                            Objet.Value = retour

                        Case "GENERIQUEVALUE"
                            Dim retour As Double = Val(Sauve_temp_teleinfo(LTrim(Objet.Adresse1), Objet.Adresse2))
                            Objet.Value = retour

                        Case Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Read", "Erreur du type du composant de " & Objet.Adresse1)

                    End Select

                End If

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Read", ex.Message)
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
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Write", ex.Message)
            End Try
        End Sub

        ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
        ''' <param name="DeviceId">Objet représetant le device à interroger</param>
        ''' <remarks></remarks>
        Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo DeleteDevice", ex.Message)
            End Try
        End Sub

        ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
        ''' <param name="DeviceId">Objet représetant le device à interroger</param>
        ''' <remarks></remarks>
        Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo NewDevice", ex.Message)
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
                _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

                ' liste des devices compatibles
                _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
                _DeviceSupport.Add(ListeDevices.ENERGIETOTALE.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)

                'Paramétres avancés
                Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
                Add_ParamAvance("SecondPort", "Second Teleinfo Port Com (COM2,COM3,COM4...nothing)", "nothing")
                Add_ParamAvance("BaudRate", "Vitesse de transfert à utiliser ", 0)

                Add_LibelleDevice("ADRESSE1", "Adresse", "")
                Add_LibelleDevice("ADRESSE2", "Adresse Second TeleInfo", "La valeur peut etre COM1, COM2, COM3... ou nothing", "nothing")
                Add_LibelleDevice("SOLO", "@", "")
                Add_LibelleDevice("MODELE", "@", "")
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo New", ex.Message)
            End Try
        End Sub

        ''' <summary>Si refresh >0 gestion du timer</summary>
        ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
        Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

        End Sub

#End Region

#Region "Fonctions internes"
        ''' <summary>Ouvrir le port Teleinfo</summary>
        ''' <param name="CptPort">Controleur du port COM:</param>
        ''' <remarks></remarks>
        Private Function ouvrir(ByVal CptPort As TeleInfo) As String
            Try
                'ouverture du port
                If Not CptPort.IsConnectPort And Not (CptPort.SerialPort.IsOpen()) Then
                    ' Test d'ouveture du port Com du controleur 
                    CptPort.SerialPort.PortName = CptPort.port_name 'nom du port : COM1,COM2, COM3...   
                    CptPort.SerialPort.Open()
                    ' Le port existe ==> le controleur est present
                    If CptPort.SerialPort.IsOpen() Then
                        CptPort.SerialPort.Close()
                    Else
                        Return ("ERR: Port " & CptPort.port_name & " impossible à ouvrir")
                        Exit Function
                    End If
                    If Not (IsNumeric(_BaudRate)) Then
                        _BaudRate = 1200
                    End If
                    CptPort.SerialPort.BaudRate = _BaudRate  'vitesse du port 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200
                    CptPort.SerialPort.Parity = IO.Ports.Parity.Even ' parité paire
                    CptPort.SerialPort.StopBits = IO.Ports.StopBits.One 'un bit d'arrêt par octet
                    CptPort.SerialPort.DataBits = 7 'nombre de bit par octet
                    'port.Handshake = Handshake.None
                    CptPort.SerialPort.ReadTimeout = 3000
                    CptPort.SerialPort.WriteTimeout = 5000
                    CptPort.SerialPort.RtsEnable = True  'ligne Rts désactivé
                    CptPort.SerialPort.DtrEnable = True  'ligne Dtr désactivé
                    CptPort.SerialPort.Open()
                    AddHandler CptPort.SerialPort.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)

                    Return ("Port " & CptPort.port_name & " ouvert")
                Else
                    Return ("Port " & CptPort.port_name & " dejà ouvert")
                End If
            Catch ex As Exception
                Return ("ERR: " & ex.Message)
            End Try
        End Function

        ''' <summary>Fermer le port TeleInfo</summary>
        ''' <remarks></remarks>
        Private Function fermer(ByVal CptPort As TeleInfo) As String
            Try
                If CptPort.IsConnectPort Then
                    RemoveHandler CptPort.SerialPort.DataReceived, AddressOf DataReceived

                    If (Not (CptPort.SerialPort Is Nothing)) Then ' The COM port exists.
                        If CptPort.SerialPort.IsOpen Then
                            CptPort.SerialPort.DiscardInBuffer()
                            CptPort.SerialPort.Close()
                            CptPort.SerialPort.Dispose()
                            CptPort.IsConnectPort = False

                            _IsConnect = False
                            Return ("Port " & CptPort.port_name & " fermé")
                        Else
                            Return ("Port " & CptPort.port_name & "  est déjà fermé")
                        End If
                    Else
                        Return ("Port " & CptPort.port_name & " n'existe pas")
                    End If
                Else
                    Return ("Port " & CptPort.port_name & "  est déjà fermé (port_ouvert=false)")
                End If
            Catch ex As UnauthorizedAccessException
                Return ("ERR: Port " & CptPort.port_name & " IGNORE")
                ' The port may have been removed. Ignore.
            End Try
            Return True
        End Function

        ''' <summary>Fonction lancée sur reception de données sur le port COM</summary>
        ''' <remarks></remarks>
        Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
            Try

                Dim BufferIn(8192) As Byte
                Dim count As Integer = 0

                ' Recherche du port du compteur Teleinfo
                Dim sp As New SerialPort
                Dim CptPort As New TeleInfo
                Try
                    sp = CType(sender, SerialPort)
                    For Each TeleInfoCpt In TabCompteur
                        If TeleInfoCpt.port_name = sp.PortName Then
                            CptPort = TeleInfoCpt
                        End If
                    Next

                Catch ex As Exception
                    Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " DataReceived : Erreur recherche CptPort ", "Exception : " & ex.Message)
                End Try

                ' Si le port est connecté - Reception des caracteres contenus dans le Buffer
                If CptPort.IsConnectPort Then
                    count = CptPort.SerialPort.BytesToRead
                    Try
                        If count Then
                            CptPort.SerialPort.Read(BufferIn, 0, count)
                            For i As Integer = 0 To count - 1
                                ProcessReceivedChar(CptPort, BufferIn(i))
                            Next
                        Else
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " DataReceived", "Pas de donnée recue")
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " DataReceived ", "Exception : " & ex.Message)
                    End Try
                End If



            Catch Ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Datareceived", "Exception : " & Ex.Message)
            End Try
        End Sub

        ''' <summary>Rassemble un message complet pour ensuite l'envoyer à displaymess</summary>
        ''' <param name="temp">Byte recu</param>
        ''' <remarks></remarks>
        Private Sub ProcessReceivedChar(ByVal PortCom As TeleInfo, ByVal temp As Byte)
            Dim TeleInfo_adresse As String = ""
            Dim data1 As String = ""
            Dim InfoRec As String = ""

            Dim charSeparators() As Char = {" "c}
            Dim result() As String

            Try
                ' Analyse de chaque caractere pour detecter 
                '           Le debut d'une trame
                '           Le debut d'un message
                '           La fin   d'un message
                '           La fin   d'une trame 
                If (temp = 2) Then ' Debut de trame recu 
                    PortCom.DebutTrame = True
                    PortCom.bytecnt = 0
                    PortCom.messcnt = 0
                    PortCom.mess = False
                    PortCom.trame = False
                ElseIf (PortCom.DebutTrame And temp = 3) Then 'Fin de trame recue
                    PortCom.trame = True

                ElseIf (PortCom.DebutTrame And temp = 10) Then ' debut d'info recu
                    PortCom.mess = False
                    PortCom.bytecnt = 0
                ElseIf (PortCom.DebutTrame And temp = 13) Then ' Fin d'info rec
                    PortCom.mess = True
                Else 'Recuperation de l'info
                    PortCom.recbuf.Add(temp)
                    PortCom.bytecnt += 1
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ProcessReceivedChar", ex.Message)
            End Try

            Try
                ' Une trame complete a été recue 
                If PortCom.trame Then
                    For Each InfoRec In PortCom.InfoTrame   ' Traitement de chaque message
                        Try
                            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Traitement Trame ", "ligne recue : " & InfoRec)
                            ' Separation des infos de la ligne 
                            result = InfoRec.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries)

                            If result.Count > 1 Then
                                TeleInfo_adresse = result(0)
                                data1 = result(1)
                                If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Traitement Trame : Extract data", "Result : " & TeleInfo_adresse & ": " & data1)
                            Else
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Traitement Trame", " Get data Error trame incorrecte")
                            End If
                        Catch ex As Exception
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Traitement Trame ", TeleInfo_adresse & " avec la valeur : " & data1 & "Exception :  " & ex.Message)
                        End Try

                        Try
                            ' Recherche de l'index du champs
                            Dim Etiquette As Labels
                            [Enum].TryParse(Of Labels)(LTrim(UCase(TeleInfo_adresse)), Etiquette)

                            If Not [Enum].IsDefined(GetType(Labels), Etiquette) Then
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Traitement Label ", "Parametre non reconnu adresse : " & TeleInfo_adresse)
                            Else
                                Dim Index As Int16 = CType(Etiquette, Labels)
                                PortCom.TabLabels(Index) = data1
                                If _DEBUG Then Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Traitement Label ", "Traitement de l'adresse " & Etiquette.ToString & " avec l'index : " & Index)
                            End If


                        Catch ex As Exception
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Traitement Label  : ", "Exception : " & ex.Message)
                        End Try
                    Next
                    PortCom.InfoTrame.Clear()

                ElseIf PortCom.mess Then ' Un message est recu ==> on le stocke
                    Dim xxx As String = ""
                    For i As Integer = 0 To PortCom.bytecnt - 1
                        xxx = xxx & (ChrW(PortCom.recbuf(i)))
                    Next
                    PortCom.recbuf.Clear()
                    PortCom.InfoTrame.Add(xxx.ToString)

                    If _DEBUG Then _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Traitement Message - Information recue", xxx.ToString)
                    PortCom.messcnt += 1
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Traitement Label - Traite Message", ex.Message)
            End Try
        End Sub

        Private Function Sauve_temp_teleinfo(ByVal adresse As String, Optional ByVal Adresse2 As String = Nothing) As String
            Dim retour As String = ""
            Dim CptPort As New TeleInfo

            Try
                If Adresse2.ToLower = "nothing" Or Adresse2 = "" Then
                    Adresse2 = _Com
                End If

                For Each TeleInfoCpt In TabCompteur
                    If TeleInfoCpt.port_name = Adresse2 Then
                        CptPort = TeleInfoCpt
                    End If
                Next

                ' Recherche de l'index du champs
                Dim Etiquette As Labels
                If Not [Enum].TryParse(Of Labels)(LTrim(UCase(adresse)), Etiquette) Then
                    retour = ""
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Sauve_temp_teleinfo : Case Teleinfo_adresse ", "Parametre non reconnu adresse : " & adresse)
                Else
                    Dim Index As Int16 = CType(Etiquette, Labels)
                    retour = CptPort.TabLabels(Index)
                    Server.Log(TypeLog.INFO, TypeSource.DRIVER, "TeleInfo Sauve_temp_teleinfo ", "Traitement de l'adresse " & Etiquette.ToString & " avec l'index : " & Index)
                End If

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Process : Traitement Exception ", "Exception : " & ex.Message)
            End Try
            Return retour
        End Function

#End Region

    End Class
End Class
