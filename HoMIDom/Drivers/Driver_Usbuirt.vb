Imports UsbUirt

Namespace HoMIDom
    '***********************************************
    '** CLASS DRIVER USBUIRT
    '** version 1.0
    '** Date de création: 14/01/2011
    '** Historique (SebBergues): 14/01/2011: Création 
    '***********************************************

    <Serializable()> Public Class Driver_Usbuirt
        Inherits Server

        Dim _ID As String
        Dim _Nom As String = "USBUIRT"
        Dim _Enable As String = False
        Dim _Description As String = "Driver Infra-Rouge UsbUirt"
        Dim _StartAuto As Boolean = False
        Dim _Protocol As String = "IR"
        Dim _IsConnect As Boolean = False
        Dim _IP_TCP As String
        Dim _Port_TCP As String
        Dim _IP_UDP As String
        Dim _Port_UDP As String
        Dim _Com As String
        Dim _Refresh As Integer
        Dim _Modele As String = "usbuirt"
        Dim _Version As String = "1.0"
        Dim _Picture As String
        Dim MyTimer As New Timers.Timer

        Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object)

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

        'Identification unique du driver
        Public Property ID() As String
            Get
                Return _ID
            End Get
            Set(ByVal value As String)
                _ID = value
            End Set
        End Property

        'Libellé de driver (qui sert aussi à l'affichage)
        Public ReadOnly Property Nom() As String
            Get
                Return _Nom
            End Get
        End Property

        'Activation du Driver
        Public Property Enable() As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property

        'Description qui peut être le modèle du driver ou autre chose
        Public ReadOnly Property Description() As String
            Get
                Return _Description
            End Get
        End Property

        'True si le driver doit être activé dès le démarrage du serveur ou False s’il doit être activé manuellement
        Public Property StartAuto() As Boolean
            Get
                Return _StartAuto
            End Get
            Set(ByVal value As Boolean)
                _StartAuto = value
                If value = True Then Start()
            End Set
        End Property

        'X10, IP, PLCBUS, 1WIRE, …
        Public ReadOnly Property Protocol() As String
            Get
                Return _Protocol
            End Get
        End Property

        'True si le driver est actif
        Public ReadOnly Property IsConnect() As Boolean
            Get
                Return _IsConnect
            End Get
        End Property

        'Adresse IP (facultatif) en TCP
        Public Property IP_TCP() As String
            Get
                Return _IP_TCP
            End Get
            Set(ByVal value As String)
                _IP_TCP = value
            End Set
        End Property

        'Port IP (facultatif) en TCP
        Public Property Port_TCP()
            Get
                Return _Port_TCP
            End Get
            Set(ByVal value)
                _Port_TCP = value
            End Set
        End Property

        'Adresse IP (facultatif) en UDP
        Public Property IP_UDP() As String
            Get
                Return _IP_UDP
            End Get
            Set(ByVal value As String)
                _IP_UDP = value
            End Set
        End Property

        'Port IP (facultatif) en UDP
        Public Property Port_UDP() As String
            Get
                Return _Port_UDP
            End Get
            Set(ByVal value As String)
                _Port_UDP = value
            End Set
        End Property

        'Port Com (facultatif)
        Public Property COM() As String
            Get
                Return _Com
            End Get
            Set(ByVal value As String)
                _Com = value
            End Set
        End Property

        'Paramétre de rafraichissement ou de pooling (facultatif) en ms
        Public Property Refresh() As Integer
            Get
                Return _Refresh
            End Get
            Set(ByVal value As Integer)
                _Refresh = value
                If _Refresh > 0 Then
                    MyTimer.Interval = _Refresh
                    MyTimer.Enabled = True
                    AddHandler MyTimer.Elapsed, AddressOf TimerTick
                End If
            End Set
        End Property

        'Si refresh >0 gestion du timer
        Private Sub TimerTick()

        End Sub

        'Modèle du driver (CM11, CM15…) facultatif
        Public ReadOnly Property Modele() As String
            Get
                Return _Modele
            End Get
        End Property

        'Version du driver
        Public ReadOnly Property Version() As String
            Get
                Return _Version
            End Get
        End Property

        'Adresse de son image
        Public Property Picture() As String
            Get
                Return _Picture
            End Get
            Set(ByVal value As String)
                _Picture = value
            End Set
        End Property

        'Fonctions propores au driver

        Public Sub Start()
            Try
                Me.mc = New Controller
                'capte les events
                AddHandler mc.Received, AddressOf handler_mc_received
                _IsConnect = True
                'Log.Log(TypeLog.INFO, TypeSource.DRIVER, "Driver " & Me.Nom & " démarré")
            Catch ex As Exception
                _IsConnect = False
                'Log.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Driver " & Me.Nom & " erreur lors du démarrage: " & ex.Message)
            End Try
        End Sub

        Public Sub [Stop]()
            Me.mc = Nothing
            _IsConnect = False
            'Log.Log(TypeLog.INFO, TypeSource.DRIVER, "Driver " & Me.Nom & " arrêté")
        End Sub

        Public Sub Restart()
            [Stop]()
            Start()
        End Sub

        '*******************************************************
        'Fonctions/Sub gérées par le driver provenant des devices


        '******************************************************
        'Fonctions/Sub propres à ce driver

        'Apprendre un code IR
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
                'Log.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Driver " & _Nom & " Erreur:" & ex.Message)
                Return Nothing
            End Try

            'retourne le code
            wait_for_code.code_to_send = Me.args.Code
            wait_for_code.code_to_receive = last_received_code
            Return wait_for_code
        End Function

        '*****************************************************************************
        'emet un code infrarouge
        Public Sub SendCodeIR(ByVal ir_code As String, ByVal RepeatCount As Integer)
            Try
                mc.Transmit(ir_code, CodeFormat.Uuirt, RepeatCount, TimeSpan.Zero)
                'Log.Log(TypeLog.MESSAGE, TypeSource.DRIVER, "Driver " & _Nom & "IR envoyé: " & ir_code & " repeat: " & RepeatCount)
            Catch ex As Exception
                'Log.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Driver " & _Nom & "Problème de transmission: " & ex.Message)
            End Try
        End Sub

        '*****************************************************************************
        'handler code recu
        Private Sub handler_mc_received(ByVal sender As Object, ByVal e As ReceivedEventArgs)
            Debug.WriteLine("Code recu: " & e.IRCode)
            last_received_code = e.IRCode
            'RaiseEvent SendMessage(_Nom, eHomeApi.IeHomeServer.Reason.MESSAGE_IR, e.IRCode)
        End Sub

        '*****************************************************************************
        'handler en apprentissage
        Private Sub handler_mc_learning(ByVal sender As Object, ByVal e As LearningEventArgs)
            Try
                Debug.WriteLine("Learning: " & e.Progress & " freq=" & e.CarrierFrequency & " quality=" & e.SignalQuality)
            Catch ex As Exception
                'Debug.WriteLine("Aahhhhhhhhhhhhhhhhhhh")
            End Try
        End Sub

        '*****************************************************************************
        'handler a appris
        Private Sub handler_mc_learning_completed(ByVal sender As Object, ByVal e As LearnCompletedEventArgs)
            args = e
            Debug.WriteLine("Learning completed: " & e.Code)
            'RaiseEvent SendMessage(_Nom, eHomeApi.IeHomeServer.Reason.MESSAGE_IR, e.Code)
        End Sub
    End Class

End Namespace