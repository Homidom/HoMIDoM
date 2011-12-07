Imports HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Server
Imports UsbUirt

' Auteur : Seb
' Date : 10/02/2011

''' <summary>Class Driver_USBUIRT, permet d'apprendre des codes IR et de les restituer par la suite </summary>
''' <remarks>Nécessite la dll usbuirtmanagedwrapper</remarks>
<Serializable()> Public Class Driver_USBUIRT
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "74FD4E7C-34ED-11E0-8AC4-70CEDED72085"
    Dim _Nom As String = "USBuirt"
    Dim _Enable As String = False
    Dim _Description As String = "Emetteur/Récepteur Infrarouge sur port USB"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "IR"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = ""
    Dim _Port_TCP As String = ""
    Dim _IP_UDP As String = ""
    Dim _Port_UDP As String = ""
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "USBuirt"
    Dim _Version As String = "1.0"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Variables internes"
    'variables propres à ce driver
    <NonSerialized()> Dim mc As Controller 'var pour l'usb uirt
    <NonSerialized()> Private learn_code_modifier As LearnCodeModifier = LearnCodeModifier.ForceStruct
    <NonSerialized()> Private code_format As CodeFormat = CodeFormat.Uuirt
    <NonSerialized()> Dim args As LearnCompletedEventArgs = Nothing     'arguments récup lors de l'apprentissage

    Private last_received_code As String        'dernier code recu

    Public Structure ircodeinfo
        Public code_to_send As String
        Public code_to_receive As String
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
    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property
    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
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

    ''' <summary>
    ''' Execute une commande avancée
    ''' </summary>
    ''' <param name="Command"></param>
    ''' <param name="Param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False

        If Command = "" Then
            Return False
            Exit Function
        End If

        Select Case UCase(Command)
            Case ""
            Case Else
        End Select

        Return retour
    End Function

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            Me.mc = New Controller
            'capte les events
            AddHandler mc.Received, AddressOf handler_mc_received
            _IsConnect = True
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "USBUIRT", "Driver démarré")
        Catch ex As Exception
            _IsConnect = False
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt Start", "Driver erreur lors du démarrage: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            Me.mc = Nothing
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "USBUIRT", "Driver arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt Stop", ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le driver</summary>
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
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt Read", ex.Message)
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
            If _Enable = False Then Exit Sub
            If Objet.type = "MULTIMEDIA" Then
                If Commande = "SendCodeIR" Then
                    SendCodeIR(Parametre1, Parametre2)
                Else
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "La commande " & Commande & " est inconnue pour ce driver")
                End If
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUIRT", "Impossible d'envoyer un code IR pour un type de device autre que MULTIMEDIA")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt NewDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.MULTIMEDIA)

            'ajout des commandes avancées pour les devices
            'Ci-dessous un exemple
            'Dim x As New DeviceCommande
            'x.NameCommand = "Test"
            'x.DescriptionCommand = "Ceci est une commande avancée de test"
            'x.CountParam = 1
            '_DeviceCommandPlus.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick()

    End Sub

#End Region

#Region "Fonctions internes"
    ''' <summary>Apprendre un code IR</summary>
    ''' <returns>Retourne le code IR</returns>
    ''' <remarks></remarks>
    Public Function LearnCodeIR() As String
        If _IsConnect = False Then
            Return "Impossible d'apprendre le code IR le driver n'est pas connecté"
        Else
            Dim x As ircodeinfo
            x = wait_for_code()
            Return x.code_to_send
        End If
    End Function

    'boucle qui attend kon recoive
    <System.STAThread()> _
    Private Function wait_for_code() As ircodeinfo

        'handler
        AddHandler mc.Learning, AddressOf handler_mc_learning
        AddHandler mc.LearnCompleted, AddressOf handler_mc_learning_completed
        Me.args = Nothing

        'lance l'apprentissage
        Try
            Try
                Me.mc.LearnAsync(Me.code_format, Me.learn_code_modifier, Me.args)
                'Me.mc.Learn(Me.code_format, Me.learn_code_modifier, TimeSpan.Zero)
            Catch ex As Exception
                'MsgBox(ex.Message)
                Return Nothing
            End Try

            'attend que ce soit appris
            Do While IsNothing(Me.args)
                'Application.DoEvents()          !!!!!!!!MODIFIE A CAUSE DOEVENTS
            Loop

            'c appris !!!
            RemoveHandler mc.Learning, AddressOf handler_mc_learning
            RemoveHandler mc.LearnCompleted, AddressOf handler_mc_learning_completed

        Catch ex As Exception
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "Erreur lors de l'apprentissage:" & ex.Message)
            Return Nothing
        End Try

        'retourne le code
        wait_for_code.code_to_send = Me.args.Code
        wait_for_code.code_to_receive = last_received_code
        Return wait_for_code
    End Function

    ''' <summary>Emet un code infrarouge</summary>
    ''' <param name="ir_code"></param>
    ''' <param name="RepeatCount"></param>
    ''' <remarks></remarks>
    Public Sub SendCodeIR(ByVal ir_code As String, ByVal RepeatCount As Integer)
        Try
            mc.Transmit(ir_code, CodeFormat.Uuirt, RepeatCount, TimeSpan.Zero)
            _Server.Log(TypeLog.MESSAGE, TypeSource.DRIVER, "USBUIRT", "Code IR envoyé: " & ir_code & " repeat: " & RepeatCount)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUIRT", "Problème de transmission: " & ex.Message)
        End Try
    End Sub

    'handler code recu
    Private Sub handler_mc_received(ByVal sender As Object, ByVal e As ReceivedEventArgs)
        _Server.Log(TypeLog.MESSAGE, TypeSource.DRIVER, "USBUIRT", "Code IR reçu: " & e.IRCode)
        Debug.WriteLine("Code recu: " & e.IRCode)
        last_received_code = e.IRCode
        RaiseEvent DriverEvent(_Nom, "CODE_RECU", e.IRCode)
    End Sub

    'handler en apprentissage
    Private Sub handler_mc_learning(ByVal sender As Object, ByVal e As LearningEventArgs)
        Try
            'Debug.WriteLine("Learning: " & e.Progress & " freq=" & e.CarrierFrequency & " quality=" & e.SignalQuality)
        Catch ex As Exception

        End Try
    End Sub

    'handler a appris
    Private Sub handler_mc_learning_completed(ByVal sender As Object, ByVal e As LearnCompletedEventArgs)
        args = e
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "Learning completed: " & e.Code)
        RaiseEvent DriverEvent(_Nom, "LEARN_TERMINE", e.Code)
    End Sub
#End Region

End Class
