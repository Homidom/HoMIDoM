Namespace HoMIDom
    Module ManagerSequences
        Dim _SequenceDriver As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'un driver
        Dim _SequenceDevice As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'un device
        Dim _SequenceTrigger As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'un trigger
        Dim _SequenceZone As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'une zone
        Dim _SequenceMacro As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'une macro
        Dim _SequenceServer As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement du server

        Dim _MaxSequences As Integer = 32 'nombre max de séquences dans la liste
        Dim _Sequences As New List(Of Sequence) 'liste des dernières séquences

        Public ReadOnly Property Sequences As List(Of Sequence)
            Get
                Return _Sequences
            End Get
        End Property

        Public Property MaxSequences As Integer
            Get
                Return _MaxSequences
            End Get
            Set(value As Integer)
                If value < 0 Then value = 0
                If value > 128 Then value = 128
                _MaxSequences = 128
            End Set
        End Property

        Public Property SequenceDriver As String
            Get
                Return _SequenceDriver
            End Get
            Set(value As String)
                _SequenceDriver = value
            End Set
        End Property

        Public Property SequenceDevice As String
            Get
                Return _SequenceDevice
            End Get
            Set(value As String)
                _SequenceDevice = value
            End Set
        End Property

        Public Property SequenceTrigger As String
            Get
                Return _SequenceTrigger
            End Get
            Set(value As String)
                _SequenceTrigger = value
            End Set
        End Property

        Public Property SequenceZone As String
            Get
                Return _SequenceZone
            End Get
            Set(value As String)
                _SequenceZone = value
            End Set
        End Property

        Public Property SequenceMacro As String
            Get
                Return _SequenceMacro
            End Get
            Set(value As String)
                _SequenceMacro = value
            End Set
        End Property

        Public Property SequenceServer As String
            Get
                Return _SequenceServer
            End Get
            Set(value As String)
                _SequenceServer = value
            End Set
        End Property

        ''' <summary>
        ''' Ajouter une séquence
        ''' </summary>
        ''' <param name="Type"></param>
        ''' <param name="IDElement"></param>
        ''' <param name="Property"></param>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        Public Sub AddSequences(Type As Sequence.TypeOfSequence, IDElement As String, [Property] As String, Value As Object)
            Dim x As New Sequence
            Dim ID As String = Api.GenerateGUID

            With x
                .Numero = ID
                .ID = IDElement
                .DateTime = Now.ToLocalTime
                .Property = [Property]
                .Value = Value
                .SequenceType = Type
            End With

            Select Case Type
                Case Sequence.TypeOfSequence.Device
                    _SequenceDevice = ID
                Case Sequence.TypeOfSequence.DeviceAdd
                    _SequenceDevice = ID
                Case Sequence.TypeOfSequence.DeviceChange
                    _SequenceDevice = ID
                Case Sequence.TypeOfSequence.DeviceDelete
                    _SequenceDevice = ID
                Case Sequence.TypeOfSequence.Driver
                    _SequenceDriver = ID
                Case Sequence.TypeOfSequence.DriverAdd
                    _SequenceDriver = ID
                Case Sequence.TypeOfSequence.DriverChange
                    _SequenceDriver = ID
                Case Sequence.TypeOfSequence.DriverDelete
                    _SequenceDriver = ID
                Case Sequence.TypeOfSequence.Macro
                    _SequenceMacro = ID
                Case Sequence.TypeOfSequence.MacroAdd
                    _SequenceMacro = ID
                Case Sequence.TypeOfSequence.MacroChange
                    _SequenceMacro = ID
                Case Sequence.TypeOfSequence.MacroDelete
                    _SequenceMacro = ID
                Case Sequence.TypeOfSequence.Message
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.Notification
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.Server
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.ServerShutDown
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.ServerStart
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.HistoryChange
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.Log
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.Trigger
                    _SequenceTrigger = ID
                Case Sequence.TypeOfSequence.TriggerAdd
                    _SequenceTrigger = ID
                Case Sequence.TypeOfSequence.TriggerChange
                    _SequenceTrigger = ID
                Case Sequence.TypeOfSequence.TriggerDelete
                    _SequenceTrigger = ID
                Case Sequence.TypeOfSequence.User
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.UserAdd
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.UserChange
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.UserDelete
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.Variable
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.VariableAdd
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.VariableChange
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.VariableDelete
                    _SequenceServer = ID
                Case Sequence.TypeOfSequence.Zone
                    _SequenceZone = ID
                Case Sequence.TypeOfSequence.ZoneAdd
                    _SequenceZone = ID
                Case Sequence.TypeOfSequence.ZoneChange
                    _SequenceZone = ID
                Case Sequence.TypeOfSequence.ZoneDelete
                    _SequenceZone = ID
            End Select

            _Sequences.Add(x)

            If Server.Instance.Etat_server Then UDP.SendMessageToClient(Type & "|" & ID)

            If _Sequences.Count > _MaxSequences Then
                _Sequences.RemoveAt(0)
            End If
        End Sub


        ''' <summary>
        ''' Retourne une séquence à partir de son numéro d'identifiant
        ''' </summary>
        ''' <param name="Numero"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnSequenceFromNumero(Numero As String) As Sequence
            Dim retour As Sequence = Nothing

            If _Sequences IsNot Nothing Then
                For Each seq In _Sequences
                    If seq.Numero = Numero Then
                        Return seq
                    End If
                Next
            End If

            Return retour
        End Function
    End Module
End Namespace