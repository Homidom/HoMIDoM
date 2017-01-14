
Imports UsbUirt
Imports UsbUirt.Enums
Imports UsbUirt.EventArgs

Module modUSBuirt
    Public startMasterThread As New Threading.Thread(AddressOf masterThread)

    Public _RecCode As String
    Public _IncomingCode As Boolean

    Public _TransmitComplete As Boolean
    Public _TransmitError As String

    Public _LearnedCode As String
    Public _LearnComplete As Boolean
    Public _LearnProgress As String
    Public _SignalQuality As String
    Public _SignalFreq As String

    Public _abortAll As Boolean

    Enum curProcess
        Receive = 1
        Learn = 2
        Transmit = 3
    End Enum

    Public aktProcess As curProcess

    Private Sub masterThread()
        Try
            Do Until _abortAll = True
                If aktProcess = curProcess.Receive Then
                    Using driver__1 = New Driver()
                        Dim receiver = New Receiver(driver__1)
                        receiver.GenerateLegacyCodes = False
                        AddHandler receiver.Received, AddressOf OnReceive

                        Do Until _abortAll = True Or aktProcess <> curProcess.Receive
                            Threading.Thread.Sleep(1)
                        Loop
                    End Using
                End If

                If aktProcess = curProcess.Learn Then
                    Using driver__1 = New Driver()
                        Dim learner = New Learner(driver__1)
                        AddHandler learner.Learning, AddressOf OnLearning
                        _LearnComplete = False
                        Dim result = learner.Learn()
                        _LearnedCode = Convert.ToString(result)
                        _LearnComplete = True
                        aktProcess = curProcess.Receive

                        Do Until _abortAll = True Or _LearnComplete = True Or aktProcess <> curProcess.Learn
                            Threading.Thread.Sleep(1)
                        Loop
                    End Using
                End If

                If aktProcess = curProcess.Transmit Then
                    Using driver__1 = New Driver()
                        _TransmitComplete = False
                        Dim transmitter = New Transmitter(driver__1)
                        AddHandler transmitter.TransmitCompleted, AddressOf OnTransmitComplete
                        transmitter.TransmitAsync(_LearnedCode, emitter:=Emitter.Internal)

                        Do Until _abortAll = True Or _TransmitError <> "" Or _TransmitComplete = True Or aktProcess <> curProcess.Transmit
                            Threading.Thread.Sleep(1)
                        Loop
                    End Using
                End If
            Loop
        Catch
            _TransmitError = Err.Description
        End Try
    End Sub

    Public Sub instantTransmit(Code As String)
        Try
            Using driver__1 = New Driver()
                _TransmitComplete = False
                Dim transmitter = New Transmitter(driver__1)
                AddHandler transmitter.TransmitCompleted, AddressOf OnTransmitComplete
                transmitter.TransmitAsync(Code, emitter:=Emitter.All)

                Do Until _abortAll = True Or _TransmitError <> "" Or _TransmitComplete = True Or aktProcess <> curProcess.Transmit
                    Threading.Thread.Sleep(1)
                Loop
            End Using
        Catch
            _TransmitError = Err.Description
        End Try
    End Sub

    Private Sub OnReceive(sender As Object, e As ReceivedEventArgs)
        _RecCode = Convert.ToString(e.IRCode)
        _IncomingCode = True
    End Sub

    Private Sub OnTransmitComplete(sender As Object, e As TransmitCompletedEventArgs)
        If e.[Error] Is Nothing Then
            _TransmitComplete = True
            aktProcess = curProcess.Receive
        Else
            _TransmitError = e.Error.ToString
        End If
    End Sub

    Private Sub OnLearning(sender As Object, e As LearningEventArgs)
        'Console.WriteLine("Learning: {0}% freq={1} quality={2}", e.Progress, e.CarrierFrequency, e.SignalQuality)
        _LearnProgress = e.Progress
        _SignalFreq = e.CarrierFrequency
        _SignalQuality = e.SignalQuality
    End Sub
End Module
