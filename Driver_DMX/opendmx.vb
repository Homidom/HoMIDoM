Option Strict Off
Option Explicit On
Module OpenDmx

    ' VB.NET Driver for the Enttec.com "Open Dmx USB" widget

    ' Multi-Threading Multiple Device Edition 
    ' Currently supports upto 4 widgets

    ' version: 2.1 (29 July 2005)
    ' author: hippy (rowanmac@optusnet.com.au)


    ' USAGE CONDITIONS...
    '  please give credit for this code
    ' OTHERWISE YOU ARE BREACHING THE USAGE CONDITIONS


    ' DISCLAIMER
    '  im not responsible for anything, or liable in any way for
    '  anything to do with anything that has anything to do with me, you, your
    '  wife or boss, any associates, pets, property, lights, investments and everything
    '  else you can think up. if you don't like that, then don't do anything ever again.


    ' Requires: ftd2xx.dll (and ftdi drivers installed)
    '           kernel32.dll (windows standard)


    ' Interface...

    ' opendmx.init(0..3)  -  connect with a device, create a thread and go
    ' *returns 1 on success

    ' opendmx.init_all() - connect with all devices, create threads and go
    ' returns number of new devices found...
    ' (call with a two second timer to auto-discover new devices)

    ' opendmx.set_dmx(n, array(511) as byte) - fill the dmx buffer with a byte array.
    ' *n = device (device which this dmx data belongs to)

    ' opendmx.done(0..3)  -  disconnect a device, stop it's thread
    ' *returns nothing

    ' opendmx.done_all() - disconnect all devices, stop all threads
    ' *returns nothing



    ' implementation...


    ' the ftd2xx.dll usb interface
    Private Declare Function FT_Open Lib "FTD2XX.DLL" (ByVal intDeviceNumber As Short, ByRef lngHandle As Integer) As Integer
    Private Declare Function FT_Close Lib "FTD2XX.DLL" (ByVal lngHandle As Integer) As Integer
    Private Declare Function FT_SetDivisor Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByVal div As Integer) As Integer
    Private Declare Function FT_Read Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByVal lpszBuffer As String, ByVal lngBufferSize As Integer, ByRef lngBytesReturned As Integer) As Integer
    Private Declare Function FT_Write Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByVal lpszBuffer As String, ByVal lngBufferSize As Integer, ByRef lngBytesWritten As Integer) As Integer
    Private Declare Function FT_SetBaudRate Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByVal lngBaudRate As Integer) As Integer
    Private Declare Function FT_SetDataCharacteristics Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByVal byWordLength As Byte, ByVal byStopBits As Byte, ByVal byParity As Byte) As Integer
    Private Declare Function FT_SetFlowControl Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByVal intFlowControl As Short, ByVal byXonChar As Byte, ByVal byXoffChar As Byte) As Integer
    Private Declare Function FT_ResetDevice Lib "FTD2XX.DLL" (ByVal lngHandle As Integer) As Integer
    Private Declare Function FT_SetDtr Lib "FTD2XX.DLL" (ByVal lngHandle As Integer) As Integer
    Private Declare Function FT_ClrDtr Lib "FTD2XX.DLL" (ByVal lngHandle As Integer) As Integer
    Private Declare Function FT_SetRts Lib "FTD2XX.DLL" (ByVal lngHandle As Integer) As Integer
    Private Declare Function FT_ClrRts Lib "FTD2XX.DLL" (ByVal lngHandle As Integer) As Integer
    Private Declare Function FT_GetModemStatus Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByRef lngModemStatus As Integer) As Integer
    Private Declare Function FT_Purge Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByVal lngMask As Integer) As Integer
    Private Declare Function FT_GetStatus Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByRef lngRxBytes As Integer, ByRef lngTxBytes As Integer, ByRef lngEventsDWord As Integer) As Integer
    Private Declare Function FT_GetQueueStatus Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByRef lngRxBytes As Integer) As Integer
    Private Declare Function FT_GetEventStatus Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByRef lngEventsDWord As Integer) As Integer
    Private Declare Function FT_SetChars Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByVal byEventChar As Byte, ByVal byEventCharEnabled As Byte, ByVal byErrorChar As Byte, ByVal byErrorCharEnabled As Byte) As Integer
    Private Declare Function FT_SetTimeouts Lib "FTD2XX.DLL" (ByVal lngHandle As Integer, ByVal lngReadTimeout As Integer, ByVal lngWriteTimeout As Integer) As Integer
    Private Declare Function FT_SetBreakOn Lib "FTD2XX.DLL" (ByVal lngHandle As Integer) As Integer
    Private Declare Function FT_SetBreakOff Lib "FTD2XX.DLL" (ByVal lngHandle As Integer) As Integer

    ' FTDI Constants
    Const FT_OK As Short = 0
    Const FT_INVALID_HANDLE As Short = 1
    Const FT_DEVICE_NOT_FOUND As Short = 2
    Const FT_DEVICE_NOT_OPENED As Short = 3
    Const FT_IO_ERROR As Short = 4
    Const FT_INSUFFICIENT_RESOURCES As Short = 5

    Const FT_BITS_8 As Short = 8 ' Word Lengths
    Const FT_STOP_BITS_2 As Short = 2 ' Stop Bits
    Const FT_PARITY_NONE As Short = 0 ' Parity
    Const FT_FLOW_NONE As Short = &H0S ' Flow Control
    Const FT_PURGE_RX As Short = 1 ' Purge rx and tx buffers
    Const FT_PURGE_TX As Short = 2

    ' sleep for x milliseconds
    Private Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Integer)

    Public Const Max_Devices As Short = 3 ' four devices 0..3

    ' Threading Support variables
    Dim thdDMXSendThread(Max_Devices) As System.Threading.Thread
    Dim ThreadStarted(Max_Devices) As Boolean

    ' Device driver information
    Dim lngHandle(Max_Devices) As Integer ' device handle

    Dim IFD(Max_Devices) As Integer ' Inter-Frame-Delay

    Public strWriteBuffer(Max_Devices) As String  ' the buffer to send
    Dim lngBytesWritten(Max_Devices) As Integer ' how much has been sent

    ' Application Interfacing...
    Public Error_String(Max_Devices) As String ' Status of devices as strings
    Public JumperID(Max_Devices) As Integer ' jumper ID of device
    Public Connected(Max_Devices) As Boolean ' is device connected
    Public StartCode(Max_Devices) As Byte ' dmx startcode


    Public Sub Initialize()  ' MUST CALL THIS FIRST!
        ' create a thread for the device
        thdDMXSendThread(0) = New System.Threading.Thread(AddressOf Send_Thread_1)
        thdDMXSendThread(1) = New System.Threading.Thread(AddressOf Send_Thread_2)
        thdDMXSendThread(2) = New System.Threading.Thread(AddressOf Send_Thread_3)
        thdDMXSendThread(3) = New System.Threading.Thread(AddressOf Send_Thread_4)
    End Sub



    Public Function Init(ByRef n As Integer) As Integer
        ' Open a FTDI device, assume it's a open dmx and create a thread and go...

        Init = 0 ' fail, unless otherwise succeded

        If n > Max_Devices Then
            MsgBox("OpenDmx: only " & Max_Devices & " devices are supported!")
            Exit Function
        End If
        JumperID(n) = n ' default jumper to 0..3 for devices with no jumpers
        Connected(n) = False ' not connected

        IFD(n) = 30 ' default 30 ms between each frame
        StartCode(n) = 0 ' default startcode of 0
        Error_String(n) = "" ' no error has happened yet

        ' ==== ATTEMPT TO OPEN DEVICE ====
        If FT_Open(n, lngHandle(n)) <> FT_OK Then
            Error_String(n) = "Not Found"
            Kill_Thread((n)) ' just incase it's alive, but it should not be
            Done((n))
            Exit Function
        End If
        ' ==== PREPARE DEVICE FOR DMX TRANSMISSION ====
        ' reset the device
        If FT_ResetDevice(lngHandle(n)) <> FT_OK Then
            Error_String(n) = "Failed To Reset Device!"
            Done((n))
            Exit Function
        End If

        ' get an ID from the widget from jumpers
        Call GetID(n)

        ' set the baud rate
        If FT_SetDivisor(lngHandle(n), 12) Then
            Error_String(n) = "Failed To Set Baud Rate!"
            Done((n))
            Exit Function
        End If
        ' shape the line
        If FT_SetDataCharacteristics(lngHandle(n), FT_BITS_8, FT_STOP_BITS_2, FT_PARITY_NONE) <> FT_OK Then
            Error_String(n) = "Failed To Set Data Characteristics!"
            Done((n))
            Exit Function
        End If
        ' no flow control
        If FT_SetFlowControl(lngHandle(n), FT_FLOW_NONE, 0, 0) <> FT_OK Then
            Error_String(n) = "Failed to set flow control!"
            Done((n))
            Exit Function
        End If
        ' set bus transiever to transmit enable
        If FT_ClrRts(lngHandle(n)) <> FT_OK Then
            Error_String(n) = "Failed to set RS485 to send!"
            Done((n))
            Exit Function
        End If
        ' Clear TX & RX buffers
        If FT_Purge(lngHandle(n), FT_PURGE_TX) <> FT_OK Then
            Error_String(n) = "Failed to purge TX buffer!"
            Done((n))
            Exit Function
        End If
        ' empty buffers
        If FT_Purge(lngHandle(n), FT_PURGE_RX) <> FT_OK Then
            Error_String(n) = "Failed to purge RX buffer!"
            Done((n))
            Exit Function
        End If
        strWriteBuffer(n) = "" ' clear the software buffer
        lngBytesWritten(n) = 0 ' clear byte counter

        ' Success
        Connected(n) = True ' device connected

        ' start/resume threads
        If thdDMXSendThread(n).ThreadState = Threading.ThreadState.Suspended Then
            thdDMXSendThread(n).Resume() ' resume the thread if it has been suspended
            Error_String(n) = "SENDING DMX"
        Else
            Error_String(n) = "Starting Thread"
            thdDMXSendThread(n).Start() ' start the thread
            thdDMXSendThread(n).Priority = Threading.ThreadPriority.AboveNormal ' highest priority
        End If

        ' if threads running
        If thdDMXSendThread(n).IsAlive Then
            ThreadStarted(n) = True
            Init = 1 ' init ok!
        Else
            MsgBox("thread " & n & " creation failed! ")
            Error_String(n) = "Thread Failure?" ' the thread has failed to start
            ThreadStarted(n) = False
        End If

    End Function ' init



    ' try to initialize all devices, and return number of devices connected
    Public Function Init_All() As Integer
        Dim a As Integer
        Dim b As Integer
        Dim d As Integer
        a = b = d = 0
        For a = 0 To Max_Devices
            b = 0
            If Not Connected(a) Then
                JumperID(a) = a '
                b = Init(a) ' init a device
            End If
            If b = 1 Then d = d + 1 ' if we succeed inc the device count
        Next a

        Init_All = d ' return number of found devices

    End Function




    ' get the status of the CTS and DSR lines
    ' try and figure if the widget has and ID
    Public Function GetID(ByRef n As Integer) As Integer
        Dim ModemStatus As Integer
        ' CTS DSR    ID
        '  0   0   =  0
        '  0   1   =  1
        '  1   0   =  2
        '  1   1   =  3

        If FT_GetModemStatus(lngHandle(n), ModemStatus) <> FT_OK Then
            Error_String(n) = "Failed To Get Modem Status"
            Done((n))
            JumperID(n) = 0
            Exit Function
        End If

        If (ModemStatus = 0) Then
            JumperID(n) = 0 ' ID 0
        End If

        If (ModemStatus = 32) Then
            JumperID(n) = 1 ' ID 1
        End If

        If (ModemStatus = 16) Then
            JumperID(n) = 2 ' id 2
        End If

        If (ModemStatus = 48) Then
            JumperID(n) = 4 ' id 4
        End If

        If Connected(0) Then If JumperID(n) = 0 Then JumperID(n) = n
        ' return the id of the device
        GetID = JumperID(n)

    End Function

    ' return number of open devices (only devices opened)
    Public Function NumberOfOpenDevices() As Integer
        Dim n As Integer
        Dim b As Integer
        b = 0
        For n = 0 To Max_Devices

            If Connected(n) Then b = b + 1
        Next n
        NumberOfOpenDevices = b
    End Function


    ' Set output beffer of device n(0..max_devices) from DmxArray(512)
    ' (hint, you can also roll-your-own and overide this by puttin dmx into 
    ' strWriteBuffer(n))
    Public Sub Set_DMX(ByRef n As Integer, ByRef DmxArray() As Byte)
        If ((n > Max_Devices) Or (n < 0)) Then Exit Sub ' invaild device!
        strWriteBuffer(n) = StrConv(System.Text.UnicodeEncoding.Unicode.GetString(DmxArray), 0)
    End Sub




    ' not recieve dmx, get dmx which is being sent
    Public Function Get_DMX(ByRef n As Integer, ByRef channel As Integer) As Integer
        Get_DMX = CInt(Mid(strWriteBuffer(n), channel, 1))
    End Function


    ' send from buffer to interface "n" (0..3)
    Private Sub Send(ByRef n As Integer)

        If ((n > Max_Devices) Or (n < 0)) Then Exit Sub ' invaild device!

        ' if not connected then exit
        If Connected(n) <> True Then
            Exit Sub
        End If

        ' break
        FT_SetBreakOn(lngHandle(n))
        FT_SetBreakOff(lngHandle(n))

        ' write start code
        If FT_Write(lngHandle(n), Chr(StartCode(n)), 1, lngBytesWritten(n)) <> FT_OK Then
            Error_String(n) = "Failed to Write Startcode!"
            Done((n))
            Kill_Thread((n))
        End If

        'n = JumperID(n) ' chose which universe from jumper settings

        ' write dmx data
        If FT_Write(lngHandle(n), strWriteBuffer(n), Len(strWriteBuffer(n)), lngBytesWritten(n)) <> FT_OK Then
            Error_String(n) = "Failed To Write DMX"
            Done((n))
            Kill_Thread((n))
        End If

    End Sub



    ' Kill all dmx sending threads
    ' pre: threads have been started
    ' post: all working threads are killed
    Public Sub Stop_Threads()
        Dim n As Integer
        For n = 0 To Max_Devices
            If ThreadStarted(n) Then Kill_Thread((n))
        Next n
    End Sub


    ' kill a single thread
    Private Sub Kill_Thread(ByRef n As Integer)
        If n > Max_Devices Then ' no such device
            Exit Sub
        End If
        If ThreadStarted(n) Then thdDMXSendThread(n).Suspend() ' suspend the thread
    End Sub



    ' close a device
    Public Sub Done(ByRef n As Integer)
        ' if not connected then exit
        If Connected(n) = False Then
            Error_String(n) = "Not Connected"
            Exit Sub
        End If
        Connected(n) = False
        ' close the ftdi port
        If FT_Close(lngHandle(n)) <> FT_OK Then
            Error_String(n) = "Close Failed"
            Exit Sub
        End If
        Error_String(n) = "Done"

    End Sub 'done


    ' close all devices
    Public Sub Done_All()
        Dim n As Integer
        ' stop the threads
        Call Stop_Threads()
        For n = 0 To Max_Devices
            'empty buffers
            strWriteBuffer(n) = ""
            Done((n))
        Next n

    End Sub

    ' these are the sender threads, seperate for each device
    Private Sub Send_Thread_1()
        Error_String(0) = "SENDING DMX"
        Do
            Call Send(0) ' send a frame to device 1
            Sleep((IFD(0))) ' inter-frame-delay
        Loop  ' forever
    End Sub

    Private Sub Send_Thread_2()
        Error_String(1) = "SENDING DMX"
        Do
            Call Send(1) ' send a frame to device 2
            Sleep((IFD(1))) ' inter-frame-delay
        Loop  ' forever
    End Sub

    Private Sub Send_Thread_3()
        Error_String(2) = "SENDING DMX"
        Do
            Call Send(2) ' send a frame to device 3
            Sleep((IFD(2))) ' inter-frame-delay
        Loop  ' forever
    End Sub

    Private Sub Send_Thread_4()
        Error_String(3) = "SENDING DMX"
        Do
            Call Send(3) ' send a frame to device 4
            Sleep((IFD(3))) ' inter-frame-delay
        Loop  ' forever
    End Sub

End Module