Public Class Arduino
    Private WithEvents serialPort1 As System.IO.Ports.SerialPort
    Private CommandBuffer As String
    Private Receiving As Boolean
    Private BufferPointer As Integer
    Private CBuffer(30) As Byte

    'Serial default values
    Private _ComPort As String = "COM2"
    Private _BaudRate As Integer = 9600

    'default set of pins from Duemilanove
    Private _DigitalCount As Integer = 14
    Private _AnalogCount As Integer = 6
    Private _ServoCount As Integer = 2
    Private _PWMPorts As Integer() = {3, 5, 6, 9, 10, 11}
    Private _ServoPorts As Integer() = {9, 10}
    Private _MotorNrs As Integer() = {1, 2, 3, 4}

    'Private WithEvents DataCheckTimer As System.Timers.Timer
    Private WithEvents WatchDogTimer As System.Timers.Timer

    Public Enum DigitalDirection
        Input = 0
        DigitalOutput = 1
        PWMOutput = 2
    End Enum

    Public Enum MotorDirection
        Forward = 1
        Backward = 2
        Release = 4
    End Enum

    ''' <summary>
    ''' Gives the value of a digital port
    ''' </summary>
    ''' <param name="DPortNr">The digital portnr. that has the data</param>
    ''' <param name="Value">The new value of the port</param>
    ''' <remarks>This event is fired when a trigger is enabled on a digital port 
    ''' and the data changes or as a reaction on the GetDigitalValue command</remarks>
    Public Event DigitalDataReceived(ByVal DPortNr As Integer, ByVal Value As Integer)

    ''' <summary>
    ''' Gives the value of an analog port
    ''' </summary>
    ''' <param name="APortNr">The analog portnumber that has the data</param>
    ''' <param name="Value">The new value of the port</param>
    ''' <remarks>This event is fired when a trigger is enabled on an analog port 
    ''' and the data changes more than the set threshold, or as a reaction on the 
    ''' GetAnalogValue command</remarks>
    Public Event AnalogDataReceived(ByVal APortNr As Integer, ByVal Value As Integer)

    ''' <summary>
    ''' Gives a system message from the Arduino
    ''' </summary>
    ''' <param name="Message">The message</param>
    ''' <remarks></remarks>
    Public Event LogMessageReceived(ByVal Message As String)

    ''' <summary>
    ''' Fires when a watchdog message (heartbeat) is received
    ''' </summary>
    ''' <remarks></remarks>
    Public Event WatchdogReceived()

    ''' <summary>
    ''' Fires when there has not been a watchdog message for 5 seconds
    ''' </summary>
    ''' <remarks></remarks>
    Public Event ConnectionLost()

    ''' <summary>
    ''' Set the comport where the Arduino is connected to
    ''' </summary>
    ''' <value>"COM1", "COM2", etc...</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ComPort() As String
        Get
            Return _ComPort
        End Get
        Set(ByVal value As String)
            _ComPort = value
        End Set
    End Property

    ''' <summary>
    ''' Set the baudrate of the comport
    ''' </summary>
    ''' <value>9600, 19200</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BaudRate() As String
        Get
            Return _BaudRate
        End Get
        Set(ByVal value As String)
            If (value = 9600 Or value = 19200) Then
                _BaudRate = value
            Else
                _BaudRate = 9600
            End If
        End Set
    End Property

    ''' <summary>
    ''' Set the number of digital pins for the used Arduino model
    ''' </summary>
    ''' <value>Number of digital pins</value>
    ''' <returns>Number of digital pins</returns>
    ''' <remarks>Default = 14 (Duemilanove)</remarks>
    Public Property DigitalCount() As Integer
        Get
            Return _DigitalCount
        End Get
        Set(ByVal value As Integer)
            _DigitalCount = value
        End Set
    End Property

    ''' <summary>
    ''' Set the number of analog pins for the used Arduino model
    ''' </summary>
    ''' <value>Number of analog pins</value>
    ''' <returns>Number of analog pins</returns>
    ''' <remarks>Default = 6 (Duemilanove)</remarks>
    Public Property AnalogCount() As Integer
        Get
            Return _AnalogCount
        End Get
        Set(ByVal value As Integer)
            _AnalogCount = value
        End Set
    End Property

    ''' <summary>
    ''' Set the zero-based pinnumbers of the digital PWM lines for the used Arduino model
    ''' </summary>
    ''' <value>Array of zero-based integers with PWM portnumbers</value>
    ''' <returns>Array of zero-based integers with PWM portnumbers</returns>
    ''' <remarks>Default = {3, 5, 6, 9, 10, 11} (Duemilanove)</remarks>
    Public Property PWMPorts() As Integer()
        Get
            Return _PWMPorts
        End Get
        Set(ByVal value As Integer())
            _PWMPorts = value
        End Set
    End Property

    ''' <summary>
    ''' Set the zero-based pinnumbers of the servo lines for the used Arduino model
    ''' </summary>
    ''' <value>Array of zero-based integers with servo portnumbers</value>
    ''' <returns>Array of zero-based integers with servo portnumbers</returns>
    ''' <remarks>Default = {9, 10}</remarks>
    Public Property ServoPorts() As Integer()
        Get
            Return _ServoPorts
        End Get
        Set(ByVal value As Integer())
            _ServoPorts = value
        End Set
    End Property

    ''' <summary>
    ''' Creates new instance of this class
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

    End Sub

    ''' <summary>
    ''' Creates new instance of this class
    ''' </summary>
    ''' <param name="PortName">The comport the Arduino is connected to</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal PortName As String)
        Me._ComPort = PortName.Trim
    End Sub

    ''' <summary>
    ''' Creates new instance of this class
    ''' </summary>
    ''' <param name="PortName">The serial port the Arduino is connected to</param>
    ''' <param name="BaudRate">The baudrate of the serial port</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal PortName As String, ByVal BaudRate As String)
        Me._ComPort = PortName.Trim
        Me._BaudRate = BaudRate.Trim
    End Sub

    ''' <summary>
    ''' Start the serial communication to the Arduino on the set comport
    ''' </summary>
    ''' <returns>True when connected</returns>
    ''' <remarks></remarks>
    Public Function StartCommunication() As Boolean
        Dim _ReturnValue As Boolean = False
        Try
            Dim components As System.ComponentModel.IContainer = New System.ComponentModel.Container()
            serialPort1 = New System.IO.Ports.SerialPort(components)
            serialPort1.PortName = _ComPort
            serialPort1.BaudRate = _BaudRate
            serialPort1.ReceivedBytesThreshold = 1

            serialPort1.Open()
            If Not serialPort1.IsOpen Then
                WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Unable to open comport...")
                _ReturnValue = False
                Exit Function
            Else
                serialPort1.DtrEnable = True
                WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Serial port is open")
                System.Threading.Thread.Sleep(1000)
                Dim Command As Byte() = {40, 0, 0, 0, 41, 0}
                'send some empty commands to clear all buffers on Arduino
                Me.SendCommand(Command)
                Me.SendCommand(Command)
                WatchDogTimer = New System.Timers.Timer(5000)
                WatchDogTimer.Enabled = True
                WatchDogTimer.Start()
                _ReturnValue = True
            End If

            ' callback for text coming back from the arduino
            AddHandler serialPort1.DataReceived, AddressOf OnReceived
        Catch ex As Exception
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Error opening comport...")
            _ReturnValue = False
        End Try
        Return _ReturnValue
    End Function

    ''' <summary>
    ''' Set the direction of digital port
    ''' </summary>
    ''' <param name="Port">Digital portnumber</param>
    ''' <param name="Direction">DigitalInput, DigitalOutput, PWMoutput</param>
    ''' <remarks></remarks>
    Public Sub SetDigitalDirection(ByVal Port As Integer, ByVal Direction As DigitalDirection)
        If Port < _DigitalCount Then
            If Direction <> DigitalDirection.PWMOutput Then
                Dim Command1 As Byte() = {40, 82, Port, Direction, 41, 0}
                Me.SendCommand(Command1)
            Else
                'is the port able to do PWM?
                If Array.IndexOf(_PWMPorts, Port) >= 0 Then
                    Dim Command1 As Byte() = {40, 82, Port, Direction, 41, 0}
                    Me.SendCommand(Command1)
                Else
                    WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Unable to set port " + Port.ToString + " to PWM")
                End If
            End If
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " is not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Set the value of the digital (Output) port
    ''' </summary>
    ''' <param name="Port">Digital portnumber</param>
    ''' <param name="Value">Value to write to port</param>
    ''' <remarks>The value can be 0/1 if the port is set to Output or 0-255 if the port is set to PWM</remarks>
    Public Sub SetDigitalValue(ByVal Port As Integer, ByVal Value As Integer)
        If Port < _DigitalCount Then
            'is the port able to do PWM?
            If Array.IndexOf(_PWMPorts, Port) >= 0 Then
                If Value < 256 Then
                    Dim Command1 As Byte() = {40, 80, Port, Value, 41, 0}
                    Me.SendCommand(Command1)
                Else
                    WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " value " + Value.ToString + " not allowed")
                End If
            Else
                If Value < 2 Then
                    Dim Command1 As Byte() = {40, 80, Port, Value, 41, 0}
                    Me.SendCommand(Command1)
                Else
                    WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " value " + Value.ToString + " not allowed")
                End If
            End If
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " is not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Enable digital port
    ''' </summary>
    ''' <param name="Port">Digital portnumber</param>
    ''' <param name="Enable">0 = Disable, 1 = Enable</param>
    ''' <remarks></remarks>
    Public Sub EnableDigitalPort(ByVal Port As Integer, ByVal Enable As Boolean)
        If Port < _DigitalCount Then
            If Enable Then
                Dim Command1 As Byte() = {40, 68, Port, 1, 41, 0}
                Me.SendCommand(Command1)
            Else
                Dim Command1 As Byte() = {40, 68, Port, 0, 41, 0}
                Me.SendCommand(Command1)
            End If
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " is not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Enable analog port
    ''' </summary>
    ''' <param name="Port">Analog portnumber</param>
    ''' <param name="Enable">0 = Disable, 1 = Enable</param>
    ''' <remarks></remarks>
    Public Sub EnableAnalogPort(ByVal Port As Integer, ByVal Enable As Boolean)
        If Port < _AnalogCount Then
            If Enable Then
                Dim Command1 As Byte() = {40, 65, Port, 1, 41, 0}
                Me.SendCommand(Command1)
            Else
                Dim Command1 As Byte() = {40, 65, Port, 0, 41, 0}
                Me.SendCommand(Command1)
            End If
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " is not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Enable servo port
    ''' </summary>
    ''' <param name="Port">Servo portnumber</param>
    ''' <param name="Enable">0 = Disable, 1 = Enable</param>
    ''' <remarks></remarks>
    Public Sub EnableServoPort(ByVal Port As Integer, ByVal Enable As Boolean)
        'is the port able to do Servo?
        If Array.IndexOf(_ServoPorts, Port) >= 0 Then
            If Enable Then
                Dim Command1 As Byte() = {40, 81, Port, 1, 41, 0}
                Me.SendCommand(Command1)
            Else
                Dim Command1 As Byte() = {40, 81, Port, 0, 41, 0}
                Me.SendCommand(Command1)
            End If
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " is not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Set the value of the servo port
    ''' </summary>
    ''' <param name="Port">Servo portnumber</param>
    ''' <param name="Value">Value to write to port</param>
    ''' <remarks>The value can be between 0 and 179 (in degrees)</remarks>
    Public Sub SetServoValue(ByVal Port As Integer, ByVal Value As Integer)
        If Array.IndexOf(_ServoPorts, Port) >= 0 Then
            If (Value >= 0) And (Value < 180) Then
                Dim Command1 As Byte() = {40, 79, Port, Value, 41, 0}
                Me.SendCommand(Command1)
            Else
                WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " value " + Value.ToString + " not allowed")
            End If
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " value " + Value.ToString + " not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Set the speed of a motor
    ''' </summary>
    ''' <param name="MotorNr">Motor number</param>
    ''' <param name="Value">Value to write to motor</param>
    ''' <remarks>The value can be between 0 and 255</remarks>
    Public Sub SetMotorSpeed(ByVal MotorNr As Integer, ByVal Value As Integer)
        If Array.IndexOf(_MotorNrs, MotorNr) >= 0 Then
            If (Value >= 0) And (Value <= 255) Then
                Dim Command1 As Byte() = {40, 70, MotorNr, Value, 41, 0}
                Me.SendCommand(Command1)
            Else
                WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Motor " + MotorNr.ToString + " value " + Value.ToString + " not allowed")
            End If
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Motor " + MotorNr.ToString + " value " + Value.ToString + " not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Set the direction of a motor
    ''' </summary>
    ''' <param name="MotorNr">Motor number</param>
    ''' <param name="Value">Value to write to motor</param>
    ''' <remarks>The value can be 1, 2 or 4</remarks>
    Public Sub SetMotorDir(ByVal MotorNr As Integer, ByVal Value As MotorDirection)
        If Array.IndexOf(_MotorNrs, MotorNr) >= 0 Then
            Dim Command1 As Byte() = {40, 77, MotorNr, Value, 41, 0}
            Me.SendCommand(Command1)
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Motor " + MotorNr.ToString + " value " + Value.ToString + " not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Enable trigger on digital port
    ''' </summary>
    ''' <param name="Port">Digital portnumber</param>
    ''' <param name="Enable">0 = Disable, 1 = Enable</param>
    ''' <remarks>The trigger will fire an event when the value of the digital (input) port is changed</remarks>
    Public Sub EnableDigitalTrigger(ByVal Port As Integer, ByVal Enable As Boolean)
        If Port < _DigitalCount Then
            If Enable Then
                Dim Command1 As Byte() = {40, 84, Port, 1, 41, 0}
                Me.SendCommand(Command1)
            Else
                Dim Command1 As Byte() = {40, 84, Port, 0, 41, 0}
                Me.SendCommand(Command1)
            End If
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " is not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Enable threshold on analog port
    ''' </summary>
    ''' <param name="Port">Analog portnumber</param>
    ''' <param name="Threshold">Threshold value (0-255)</param>
    ''' <remarks>The trigger will fire an event when the valuechange of the analog port will exceed this threshold</remarks>
    Public Sub EnableAnalogTrigger(ByVal Port As Integer, ByVal Threshold As Integer)
        If Port < _AnalogCount Then
            If Threshold < 256 Then
                Dim Command1 As Byte() = {40, 83, Port, Threshold, 41, 0}
                Me.SendCommand(Command1)
            Else
                WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " value " + Threshold.ToString + " not allowed")
            End If
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " is not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Get value of digital (output) port
    ''' </summary>
    ''' <param name="Port">Digital portnumber</param>
    ''' <remarks></remarks>
    Public Sub GetDigitalValue(ByVal Port As Integer)
        If Port < _DigitalCount Then
            Dim Command1 As Byte() = {40, 86, Port, 0, 41, 0}
            Me.SendCommand(Command1)
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " is not allowed")
        End If
    End Sub

    ''' <summary>
    ''' Get value of analog port
    ''' </summary>
    ''' <param name="Port">Analog portnumber</param>
    ''' <remarks></remarks>
    Public Sub GetAnalogValue(ByVal Port As Integer)
        If Port < _AnalogCount Then
            Dim Command1 As Byte() = {40, 87, Port, 0, 41, 0}
            Me.SendCommand(Command1)
        Else
            WriteDebug(System.Reflection.MethodBase.GetCurrentMethod.Name + ": Port " + Port.ToString + " is not allowed")
        End If
    End Sub

    Private Sub SendCommand(ByVal Command As Byte())
        Try
            If Command.Length > 0 Then
                If serialPort1.IsOpen Then
                    serialPort1.Write(Command, 0, Command.Length - 1)
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs)
        If Receiving = False Then
            Receiving = True
            ProcessSerialData()
            Receiving = False
        End If
    End Sub

    Private Sub CheckTimerElapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
        If Receiving = False Then
            Receiving = True
            ProcessSerialData()
            Receiving = False
        End If
    End Sub

    Private Sub ProcessSerialData()
        'disable further receive events
        'the entire serial buffer is processed with this event
        Try
            Dim BytesRead As Integer
            While serialPort1.BytesToRead > 0
                BytesRead = serialPort1.Read(CBuffer, BufferPointer, serialPort1.BytesToRead)
                BufferPointer += BytesRead
                While BufferPointer > 0
                    'search start of new command in buffer
                    Dim CommandStart As Integer = -1
                    For i As Short = 0 To BufferPointer
                        If CBuffer(i) = CByte(40) Then
                            CommandStart = i
                            Exit For
                        End If
                    Next
                    If CommandStart = -1 Then
                        'no begin of command found. this is no valid situation
                        ClearCBuffer()
                    End If
                    If CommandStart > 0 Then
                        'begin of command found somewhere in buffer. dismiss al bytes before startcharacter
                        LeftShiftCBuffer(CommandStart)
                    End If

                    'at this point we should have a clean commandbuffer

                    'search end of command in buffer
                    Dim Commandend As Integer = 0
                    For i As Short = 0 To BufferPointer
                        If CBuffer(i) = CByte(41) Then
                            Commandend = i
                            Exit For
                        End If
                    Next
                    If Commandend > 0 Then
                        'command-end found. there should be a complete command in the buffer
                        Dim CommandBytes(Commandend) As Byte
                        For i As Integer = 0 To Commandend
                            CommandBytes(i) = CBuffer(i)
                        Next
                        'execute command
                        ProcessCommand(CommandBytes)
                        'reset pointer
                        LeftShiftCBuffer(Commandend + 1)
                    Else
                        Exit While
                    End If
                End While
            End While
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ClearCBuffer()
        For i As Integer = 0 To CBuffer.Length - 1
            CBuffer(i) = 0
        Next
        BufferPointer = 0
    End Sub

    Private Sub LeftShiftCBuffer(ByVal NrOfPlaces As Integer)
        For i As Integer = NrOfPlaces To CBuffer.Length - 1
            CBuffer(i - NrOfPlaces) = CBuffer(i)
        Next
        For i As Integer = CBuffer.Length - NrOfPlaces To CBuffer.Length - 1
            CBuffer(i) = 0
        Next
        BufferPointer -= NrOfPlaces
    End Sub

    Private Sub ProcessCommand(ByVal CommandBytes As Byte())
        Try
            If ((CommandBytes(0) = CByte(40)) And (CommandBytes(CommandBytes.Length - 1) = CByte(41))) Then
                Dim PType As Char = ChrW(CommandBytes(1))
                Dim PNumber As Integer
                Dim Value As Integer
                Dim CFound As Boolean = False
                Select Case PType
                    Case "D"
                        PNumber = CInt(CommandBytes(2))
                        Value = CInt(CommandBytes(3))
                        RaiseEvent DigitalDataReceived(PNumber, Value)
                    Case "A"
                        PNumber = CInt(CommandBytes(2))
                        Value = (CInt(CommandBytes(3)) * 255) + CInt(CommandBytes(4))
                        RaiseEvent AnalogDataReceived(PNumber, Value)
                    Case "H"
                        PNumber = 0
                        Value = 0
                        RaiseEvent WatchdogReceived()
                        If Not IsNothing(WatchDogTimer) Then
                            WatchDogTimer.Stop()
                            WatchDogTimer.Dispose()
                        End If
                        WatchDogTimer = New System.Timers.Timer(5000)
                        WatchDogTimer.Enabled = True
                        WatchDogTimer.Start()
                    Case "Q"
                        Dim CommandString As String = String.Empty
                        For i As Integer = 0 To CommandBytes.Length - 1
                            CommandString += CommandBytes(i).ToString + " "
                        Next
                        WriteDebug("Arduino Q msg: " + CommandString)
                    Case Else
                        Dim CommandString As String = String.Empty
                        For i As Integer = 0 To CommandBytes.Length - 1
                            CommandString += CommandBytes(i).ToString + " "
                        Next
                        WriteDebug("Arduino msg: " + CommandString)
                End Select
            Else
                Dim CommandString As String = String.Empty
                For i As Integer = 1 To CommandBytes.Length - 1
                    CommandString += CommandBytes(i).ToString
                Next
                WriteDebug("Error:bad commandformat received: " + CommandString)
            End If
        Catch ex As Exception
            WriteDebug("Error:bad commandformat received")
        End Try
    End Sub

    Private Sub WatchdogTimerElaped(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles WatchDogTimer.Elapsed
        RaiseEvent ConnectionLost()
        If Not IsNothing(WatchDogTimer) Then
            WatchDogTimer.Stop()
            WatchDogTimer.Dispose()
        End If
    End Sub

    Private Sub WriteDebug(ByVal Message As String)
        RaiseEvent LogMessageReceived(Message)
    End Sub
End Class
