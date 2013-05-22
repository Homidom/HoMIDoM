Option Strict On
Option Explicit On
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Class WMR100

    Inherits System.Windows.Forms.Form

    Public Event Translate(ByVal data As String)
    Public Event Info(ByVal data As String)

    Public Property _VendorID As String

    Public Property _ProductID As String

    Const HEXA_LOG_MODE As Integer = 2

    Const MSG_CI1 As String = "20 00 08 01 00 C7 F5 C8" 'CLASS INTERFACE
    Const MSG_CI2 As String = "07 A1 01 00 20 A1 01 00" 'CLASS INTERFACE
    Const MSG_CI3 As String = "01 20 01 00 20 A1 01 00" 'CLASS INTERFACE
    Const MSG_CT1 As String = "21 09 00 02 00 00 08 00" 'CONTROL TRANSFER

    'Const MSG_CT1 As String = "21 0A 00 00 00 00 00 00" 'CONTROL TRANSFER
    'Const MSG_PCR As String = "01 D0 08 01 00 00 00 00"
    Dim nNotDetectedMessage As Integer = 0

    'Private WithEvents oForm1 As FormMain = Nothing

    Dim oLastReception As New Collection

    Dim status As Integer = 0

    Dim bReading As Boolean = False
    Dim bRestart1 As Boolean = False

    Dim DeviceNotificationHandle As IntPtr
    Dim ExclusiveAccess As Boolean
    Dim HIDHandle As Integer
    Dim HIDUsage As String
    Dim MyDeviceDetected As Boolean
    Dim MyDevicePathName As String
    Dim MyHID As New Hid()
    Dim ReadHandle As Integer
    Dim WriteHandle As Integer

    Dim MyDeviceManagement As New DeviceManagement()

    Private Delegate Sub ReadInputReportDelegate _
        (ByVal readHandle As Integer, _
        ByVal hidHandle As Integer, _
        ByVal writeHandle As Integer, _
        ByRef myDeviceDetected As Boolean, _
        ByRef readBuffer() As Byte, _
        ByRef success As Boolean)

    Private Delegate Sub MarshalToForm _
        (ByVal action As String, _
        ByVal textToAdd As String)


    Private Sub GetVendorAndProductIDs(ByRef myVendorID As Short, ByRef myProductID As Short)

        'Purpose    : Retrieves a Vendor ID and Product ID in hexadecimal 
        '           : from the form's text boxes and converts the text to Shorts.

        'Accepts    : myVendorID - the Vendor ID as a Short.
        '           : myProductID - the Product ID as a Short.                    

        Try
            myVendorID = CShort(Val("&h" & _VendorID))
            myProductID = CShort(Val("&h" & _ProductID))

        Catch ex As Exception
            MsgBox("Erreur GetVendorAndProductIDs")
        End Try
    End Sub

    Private Function FindTheHid() As Boolean

        'Purpose    : Uses a series of API calls to locate a HID-class device
        '           ; by its Vendor ID and Product ID.

        'Returns    : True if the device is detected, False if not detected.

        Dim DeviceFound As Boolean
        Dim DevicePathName(127) As String
        Dim GUIDString As String
        Dim HidGuid As System.Guid
        Dim LastDevice As Boolean
        Dim MemberIndex As Integer
        Dim MyProductID As Short
        Dim MyVendorID As Short
        Dim Result As Boolean
        Dim Security As SECURITY_ATTRIBUTES
        Dim Success As Boolean


        Try

            HidGuid = Guid.Empty
            LastDevice = False
            MyDeviceDetected = False

            'Values for the SECURITY_ATTRIBUTES structure:

            Security.lpSecurityDescriptor = 0
            Security.bInheritHandle = CInt(True)
            Security.nLength = Len(Security)

            'Get the device's Vendor ID and Product ID from the form's text boxes.

            GetVendorAndProductIDs(MyVendorID, MyProductID)

            '***
            'API function: 'HidD_GetHidGuid

            'Purpose: Retrieves the interface class GUID for the HID class.

            'Accepts: 'A System.Guid object for storing the GUID.
            '***

            HidD_GetHidGuid(HidGuid)

            'Display the GUID.

            GUIDString = HidGuid.ToString

            'Fill an array with the device path names of all attached HIDs.

            DeviceFound = MyDeviceManagement.FindDeviceFromGuid _
                (HidGuid, _
                DevicePathName)

            'If there is at least one HID, attempt to read the Vendor ID and Product ID
            'of each device until there is a match or all devices have been examined.

            If DeviceFound = True Then
                MemberIndex = 0
                Do
                    '***
                    'API function:
                    'CreateFile

                    'Purpose:
                    'Retrieves a handle to a device.

                    'Accepts:
                    'A device path name returned by SetupDiGetDeviceInterfaceDetail
                    'The type of access requested (read/write).
                    'FILE_SHARE attributes to allow other processes to access the device while this handle is open.
                    'A Security structure. Using Null for this may cause problems under Windows XP.
                    'A creation disposition value. Use OPEN_EXISTING for devices.
                    'Flags and attributes for files. Not used for devices.
                    'Handle to a template file. Not used.

                    'Returns: a handle without read or write access.
                    'This enables obtaining information about all HIDs, even system
                    'keyboards and mice. 
                    'Separate handles are used for reading and writing.
                    '***

                    HIDHandle = CreateFile _
                        (DevicePathName(MemberIndex), _
                        0, _
                        FILE_SHARE_READ Or FILE_SHARE_WRITE, _
                        Security, _
                        OPEN_EXISTING, _
                        0, _
                        0)

                    If (HIDHandle <> INVALID_HANDLE_VALUE) Then

                        'The returned handle is valid, 
                        'so find out if this is the device we're looking for.

                        'Set the Size property of DeviceAttributes to the number of bytes in the structure.

                        MyHID.DeviceAttributes.Size = Marshal.SizeOf(MyHID.DeviceAttributes)

                        '***
                        'API function:
                        'HidD_GetAttributes

                        'Purpose:
                        'Retrieves a HIDD_ATTRIBUTES structure containing the Vendor ID, 
                        'Product ID, and Product Version Number for a device.

                        'Accepts:
                        'A handle returned by CreateFile.
                        'A pointer to receive a HIDD_ATTRIBUTES structure.

                        'Returns:
                        'True on success, False on failure.
                        '***

                        Result = HidD_GetAttributes(HIDHandle, MyHID.DeviceAttributes)


                        If Result Then
                            'Find out if the device matches the one we're looking for.

                            If (MyHID.DeviceAttributes.VendorID = MyVendorID) And _
                                (MyHID.DeviceAttributes.ProductID = MyProductID) Then

                                'It's the desired device.

                                'Display the information in form's list box.

                                MyDeviceDetected = True

                                'Save the DevicePathName so OnDeviceChange() knows which name is my device.

                                MyDevicePathName = DevicePathName(MemberIndex)
                            Else

                                'It's not a match, so close the handle.

                                MyDeviceDetected = False

                                Result = CloseHandle(HIDHandle)
                            End If
                        Else
                            'There was a problem in retrieving the information.

                            MyDeviceDetected = False
                            Result = CloseHandle(HIDHandle)
                        End If

                    End If


                    'Keep looking until we find the device or there are no more left to examine.

                    MemberIndex = MemberIndex + 1

                Loop Until ((MyDeviceDetected = True) Or _
                    (MemberIndex = UBound(DevicePathName) + 1))
            End If

            If MyDeviceDetected Then

                'The device was detected.
                'Register to receive notifications if the device is removed or attached.

                Success = MyDeviceManagement.RegisterForDeviceNotifications _
                    (MyDevicePathName, _
                    Me.Handle, _
                    HidGuid, _
                    DeviceNotificationHandle)

                Debug.WriteLine("RegisterForDeviceNotifications = " & Success)

                'Learn the capabilities of the device.

                MyHID.Capabilities = MyHID.GetDeviceCapabilities _
                    (HIDHandle)

                If Success Then

                    'Find out if the device is a system mouse or keyboard.

                    HIDUsage = MyHID.GetHIDUsage(MyHID.Capabilities)

                    'Get and display the Input report buffer size.

                    'Get handles to use in requesting Input and Output reports.

                    ReadHandle = CreateFile _
                        (MyDevicePathName, _
                        GENERIC_READ, _
                        FILE_SHARE_READ Or FILE_SHARE_WRITE, _
                        Security, _
                        OPEN_EXISTING, _
                        FILE_FLAG_OVERLAPPED, _
                        0)
                    'ReadHandle = CreateFile _
                    '    (MyDevicePathName, _
                    '    GENERIC_READ, _
                    '    FILE_SHARE_READ Or FILE_SHARE_WRITE, _
                    '    Security, _
                    '    OPEN_EXISTING, _
                    '    0, _
                    '    0)

                    If (ReadHandle = INVALID_HANDLE_VALUE) Then

                        ExclusiveAccess = True


                    Else

                        'WriteHandle = CreateFile _
                        '     (MyDevicePathName, _
                        '     GENERIC_WRITE, _
                        '     FILE_SHARE_READ Or FILE_SHARE_WRITE, _
                        '     Security, _
                        '     OPEN_EXISTING, _
                        '     0, _
                        '     0)
                        WriteHandle = CreateFile _
                             (MyDevicePathName, _
                             GENERIC_WRITE, _
                             FILE_SHARE_READ Or FILE_SHARE_WRITE, _
                             Security, _
                             OPEN_EXISTING, _
                             FILE_FLAG_OVERLAPPED, _
                             0)

                        '(optional)
                        'Flush any waiting reports in the input buffer.

                        'MyHID.FlushQueue(ReadHandle)

                    End If

                End If

                nNotDetectedMessage = 0
                'MyMarshalToForm("Log2", Format(Now, "HH:mm:ss") & " : Device has been detected" & vbCrLf)
                RaiseEvent Info(Format(Now, "HH:mm:ss") & " : Device has been detected" & vbCrLf)

            Else

                'The device wasn't detected.
                'MyMarshalToForm("Log2", Format(Now, "HH:mm:ss") & " : Device not detected" & vbCrLf)
                RaiseEvent Info(Format(Now, "HH:mm:ss") & " : Device not detected" & vbCrLf)
                nNotDetectedMessage += 1
            End If
            Return MyDeviceDetected

        Catch ex As Exception
            MsgBox("Erreur FindTheHid")
        End Try
    End Function

    Public Sub Shutdown()

        'Purpose    : Perform actions that must execute when the program ends.

        Try
            'Close open handles to the device.

            If (HIDHandle <> 0) Then
                CloseHandle(HIDHandle)
            End If

            If (ReadHandle <> 0) Then
                CloseHandle(ReadHandle)
            End If

            If (WriteHandle <> 0) Then
                CloseHandle(WriteHandle)
            End If

            'Stop receiving notifications.

            Call MyDeviceManagement.StopReceivingDeviceNotifications(DeviceNotificationHandle)

        Catch ex As Exception
            MsgBox("Erreur Shutdown")
        End Try

    End Sub

    Private Sub GetInputReportData(ByVal ar As IAsyncResult)

        'Purpose    : Retrieves Input report data and status information.
        '           : This routine is called automatically when myInputReport.Read
        '           : returns.
        '           : Calls several marshaling routines to access the main form.

        'Accepts    : ar - an object containing status information about 
        '           : the asynchronous operation.    


        Dim InputReportBuffer As Byte()
        Dim Success As Boolean

        Try

            InputReportBuffer = Nothing

            'Define a delegate using the IAsyncResult object.

            Dim deleg As ReadInputReportDelegate = _
                DirectCast(ar.AsyncState, ReadInputReportDelegate)

            'Get the IAsyncResult object and the values of other paramaters that the
            'BeginInvoke method passed ByRef.

            deleg.EndInvoke(MyDeviceDetected, InputReportBuffer, Success, ar)

            If (Not MyDeviceDetected) Then
                nNotDetectedMessage = 0
                ' MyMarshalToForm("Log2", Format(Now, "HH:mm:ss") & " : Device has been lost" & vbCrLf)
                RaiseEvent Info(Format(Now, "HH:mm:ss") & " : Device has been lost" & vbCrLf)
                Shutdown()

            End If

            'Display the received report data in the form's list box.

            If (ar.IsCompleted And Success) Then
                OnDataRead(InputReportBuffer)
            End If

        Catch ex As Exception
            MsgBox("Erreur GetInputReportData")
        End Try

    End Sub

    Public Function SendToHID(ByVal sMSG As String) As Boolean
        Try
            If (MyDeviceDetected = False) Then
                MyDeviceDetected = FindTheHid()
            End If

            Dim Success As Boolean
            Dim OutputReportBuffer() As Byte
            If MyHID.Capabilities.OutputReportByteLength > 0 Then
                ReDim OutputReportBuffer(MyHID.Capabilities.OutputReportByteLength - 1)
                OutputReportBuffer(0) = 0

                Dim values As String() = sMSG.Split(" "c)
                Dim value As String
                Dim i As Integer = 1
                For Each value In values
                    OutputReportBuffer(i) = Byte.Parse(value, Globalization.NumberStyles.HexNumber)
                    i += 1
                Next

                'Use a control transfer to send the report,
                'even if the HID has an interrupt OUT endpoint.

                Dim myOutputReport As New Hid.OutputReport
                Success = myOutputReport.Write(OutputReportBuffer, WriteHandle)

                'Console.WriteLine(Format(Now, "HH:mm:ss") & " : " & "Send " & sMSG)
                If HEXA_LOG_MODE = 1 Then
                    'MyMarshalToForm("Log2", Format(Now, "HH:mm:ss") & " : Sent " & sMSG & vbCrLf)
                    RaiseEvent Info(Format(Now, "HH:mm:ss") & " : Sent " & sMSG & vbCrLf)
                End If

                'Application.DoEvents()

                Return Success
            Else : Return True
            End If

        Catch ex As Exception
            MsgBox("Erreur SendToHid")
        End Try
    End Function


    Public Sub Start() 'ByVal bRestart As Boolean)
        Try
            Dim InputReportBuffer() As Byte
            'Dim Success As Boolean

            If (MyDeviceDetected = False) Then
                MyDeviceDetected = FindTheHid()
            End If

            If MyDeviceDetected Then
                'ReDim GlobalInputReportBuffer(1000)
                'GlobalInputReportBufferLength = 0

                SendToHID(MSG_CI1)
                status = 2

                ReDim InputReportBuffer(MyHID.Capabilities.InputReportByteLength - 1)

                WaitRead(InputReportBuffer)
            End If

        Catch ex As Exception
            MsgBox("Erreur Start")
        End Try
    End Sub

    Private Function WaitRead(ByRef InputReportBuffer() As Byte) As Integer
        Try
            If (MyDeviceDetected = False) Then
                MyDeviceDetected = FindTheHid()
            End If

            Dim Success As Boolean

            Dim ar As IAsyncResult
            Dim myInputReport As New Hid.InputReport

            Dim count As Integer
            For count = 0 To UBound(InputReportBuffer)
                InputReportBuffer(count) = 0
            Next count

            'Define a delegate for the Read method of myInputReport.

            Dim MyReadInputReportDelegate As _
                New ReadInputReportDelegate(AddressOf myInputReport.Read)

            'The BeginInvoke method calls myInputReport.Read to attempt to read a report.
            'The method has the same parameters as the Read function,
            'plus two additional parameters:
            'GetInputReportData is the callback procedure that executes when the Read function returns.
            'MyReadInputReportDelegate is the asynchronous delegate object.
            'The last parameter can optionally be an object passed to the callback.

            ar = MyReadInputReportDelegate.BeginInvoke _
                (ReadHandle, _
                HIDHandle, _
                WriteHandle, _
                MyDeviceDetected, _
                InputReportBuffer, _
                Success, _
                New AsyncCallback(AddressOf GetInputReportData), _
                MyReadInputReportDelegate)

        Catch ex As Exception
            MsgBox("Erreur WaitRead")
        End Try
    End Function

    Private Function GetHex(ByRef bytes() As Byte, Optional ByVal n As Integer = -1) As String
        Dim s As String = ""
        Try
            Dim i As Integer
            Dim strByte As String

            If n = -1 Then
                n = bytes.Length - 1
            End If

            For i = 0 To n
                strByte = Hex(bytes(i))
                If (Len(strByte) < 2) Then strByte = "0" & strByte
                If s <> "" Then s &= " "
                s &= strByte
            Next i

        Catch ex As Exception
            MsgBox("Erreur GetHex")
        End Try
        Return s
    End Function

    Private Function OnDataRead(ByRef InputReportBuffer() As Byte) As Integer
        Try
            If bReading Then Exit Function

            bReading = True
            If status > 0 Then
                If status = 2 Then
                    SendToHID(MSG_CT1)
                    status = 3
                ElseIf status = 3 Then
                    SendToHID(MSG_CI2)
                    status = 4
                ElseIf status = 4 Then
                    SendToHID(MSG_CT1)
                    status = 5
                ElseIf status = 5 Then
                    SendToHID(MSG_CI3)
                    status = 6
                ElseIf status = 6 Then
                    SendToHID(MSG_CT1)
                    status = 0
                End If
            Else

                Debug.WriteLine(Format(Now, "HH:mm:ss") & " : " & "(I) " & GetHex(InputReportBuffer))


                's = GetHex(bytes)
                Dim s As String = ""
                For k As Integer = 0 To InputReportBuffer.Length - 1
                    s = s & CStr(InputReportBuffer(k)) & ";"
                Next

                If HEXA_LOG_MODE = 2 Then
                    RaiseEvent Translate(s)
                    'MyMarshalToForm("Log2", Format(Now, "HH:mm:ss") & " : " & s & vbCrLf)
                    'MyMarshalToForm("Log1", s)
                End If

                For i As Integer = 0 To UBound(InputReportBuffer)
                    InputReportBuffer(i) = 0
                Next i
                status = 0 '4
            End If

            System.Threading.Thread.Sleep(100)
            'Application.DoEvents()

            bReading = False

            WaitRead(InputReportBuffer)

        Catch ex As Exception
            MsgBox("Erreur OnDataRead")
        End Try
        Return 0
    End Function

    Public Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs)
        If Not MyDeviceDetected Then Start()
    End Sub

    Sub New()

        MyBase.new()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)

        'Purpose    : Overrides WndProc to enable checking for and handling
        '           : WM_DEVICECHANGE(messages)

        'Accepts    : m - a Windows Message                   

        Try
            'The OnDeviceChange routine processes WM_DEVICECHANGE messages.

            If m.Msg = WM_DEVICECHANGE Then
                OnDeviceChange(m)
            End If

            'Let the base form process the message.

            MyBase.WndProc(m)

        Catch ex As Exception
            MsgBox("Erreur WndProc")
        End Try

    End Sub

    Friend Sub OnDeviceChange(ByVal m As Message)

        'Purpose    : Called when a WM_DEVICECHANGE message has arrived,
        '           : indicating that a device has been attached or removed.

        'Accepts    : m - a message with information about the device

        Debug.WriteLine("WM_DEVICECHANGE")

        Try
            If (m.WParam.ToInt32 = DBT_DEVICEARRIVAL) Then

                'If WParam contains DBT_DEVICEARRIVAL, a device has been attached.

                'Find out if it's the device we're communicating with.

                If MyDeviceManagement.DeviceNameMatch(m, MyDevicePathName) Then
                    Start() 'True)
                End If

            ElseIf (m.WParam.ToInt32 = DBT_DEVICEREMOVECOMPLETE) Then

                'If WParam contains DBT_DEVICEREMOVAL, a device has been removed.

                Debug.WriteLine("A device has been removed.")

                'Find out if it's the device we're communicating with.

                If MyDeviceManagement.DeviceNameMatch(m, MyDevicePathName) Then

                    nNotDetectedMessage = 0
                    'MyMarshalToForm("Log2", Format(Now, "HH:mm:ss") & " : Device has been removed" & vbCrLf)


                    Shutdown()

                    'Set MyDeviceDetected False so on the next data-transfer attempt,
                    'FindTheHid() will be called to look for the device 
                    'and get a new handle.

                    MyDeviceDetected = False
                End If
            End If
        Catch ex As Exception
            MsgBox("Erreur OnDeviceChange")
        End Try
    End Sub


    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'FormMain
        '
        Me.ClientSize = New System.Drawing.Size(116, 0)
        Me.Name = "FormMain"
        Me.ResumeLayout(False)

    End Sub
End Class