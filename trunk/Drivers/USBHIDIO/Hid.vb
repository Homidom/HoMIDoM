Option Strict On
Option Explicit On 
Imports System.Runtime.InteropServices

Friend Class Hid

    'For communicating with HID-class devices.

    'Used in error messages.

    Const ModuleName As String = "Hid"

    Friend Capabilities As HIDP_CAPS
    Friend DeviceAttributes As HIDD_ATTRIBUTES

    'For viewing results of API calls in debug.write statements:

    Shared MyDebugging As New Debugging()

    Friend MustInherit Class DeviceReport

        'For reports that the device sends to the host.

        Friend HIDHandle As Integer
        Friend MyDeviceDetected As Boolean
        Friend Result As Integer
        Friend ReadHandle As Integer

        'Each DeviceReport class defines a ProtectedRead method for reading a type of report.
        'ProtectedRead and Read are declared as Subs rather than as Functions because 
        'asynchronous reads use a callback method that can access parameters passed by ByRef 
        'but not Function return values.

        Protected MustOverride Sub ProtectedRead _
            (ByVal readHandle As Integer, _
            ByVal hidHandle As Integer, _
            ByVal writeHandle As Integer, _
            ByRef myDeviceDetected As Boolean, _
            ByRef readBuffer() As Byte, _
            ByRef success As Boolean)


        Friend Sub Read _
             (ByVal readHandle As Integer, _
             ByVal hidHandle As Integer, _
             ByVal writeHandle As Integer, _
             ByRef myDeviceDetected As Boolean, _
             ByRef readBuffer() As Byte, _
             ByRef success As Boolean)


            'Purpose    : Calls the overridden ProtectedRead routine.
            '             Enables other classes to override ProtectedRead
            '             while limiting access as Friend.
            '             (Directly declaring Write as Friend MustOverride causes the 
            '             compiler warning : "Other languages may permit Friend 
            '             Overridable members to be overridden.")

            'Accepts    : readHandle - a handle for reading from the device.
            '             hidHandle - a handle for other device communications.  
            '             myDeviceDetected - tells whether the device is currently 
            '             attached and communicating.
            '             readBuffer - a byte array to hold the report ID and report data.  
            '             success - read success

            Try
                'Request the report.

                ProtectedRead _
                    (readHandle, _
                    hidHandle, _
                    writeHandle, _
                    myDeviceDetected, _
                    readBuffer, _
                    success)

            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Sub

    End Class


    Friend Class InFeatureReport
        Inherits DeviceReport

        'For reading Feature reports.

        Protected Overrides Sub ProtectedRead _
            (ByVal readHandle As Integer, _
            ByVal hidHandle As Integer, _
            ByVal writeHandle As Integer, _
            ByRef myDeviceDetected As Boolean, _
            ByRef inFeatureReportBuffer() As Byte, _
            ByRef success As Boolean)

            'Purpose    : reads a Feature report from the device.

            'Accepts    : readHandle - the handle for reading from the device.
            '             hidHandle - the handle for other device communications.
            '             myDeviceDetected - tells whether the device is currently attached.
            '             readBuffer - contains the requested report.
            '             success - read success

            Try

                '***
                'API function: HidD_GetFeature
                'Attempts to read a Feature report from the device.

                'Requires:
                'A handle to a HID
                'A pointer to a buffer containing the report ID and report
                'The size of the buffer. 

                'Returns: true on success, false on failure.
                '***

                success = HidD_GetFeature _
                   (hidHandle, _
                   inFeatureReportBuffer(0), _
                   UBound(inFeatureReportBuffer) + 1)

                Debug.WriteLine(MyDebugging.ResultOfAPICall("HidD_GetFeature"))
                Debug.WriteLine("")

            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Sub
    End Class


    Friend Class InputReport
        Inherits DeviceReport

        'For reading Input reports.

        Dim ReadyForOverlappedTransfer As Boolean ' initialize to false


        Friend Sub CancelTransfer _
            (ByVal readHandle As Integer, _
            ByVal hidHandle As Integer, _
            ByVal writeHandle As Integer)

            'Purpose    : closes open handles to a device.

            'Accepts    : ReadHandle - the handle for reading from the device.
            '             HIDHandle - the handle for other device communications.

            Try

                '***
                'API function: CancelIo

                'Purpose: Cancels a call to ReadFile

                'Accepts: the device handle.

                'Returns: True on success, False on failure.
                '***

                CancelIo(readHandle)


                Debug.WriteLine("************ReadFile error*************")
                Debug.WriteLine(MyDebugging.ResultOfAPICall("CancelIo"))
                Debug.WriteLine("")

                'The failure may have been because the device was removed,
                'so close any open handles and
                'set myDeviceDetected=False to cause the application to
                'look for the device on the next attempt.

                If (hidHandle <> 0) Then
                    CloseHandle(hidHandle)

                    Debug.WriteLine(MyDebugging.ResultOfAPICall("CloseHandle (HIDHandle)"))
                    Debug.WriteLine("")
                End If

                If (readHandle <> 0) Then
                    CloseHandle(readHandle)
                    Debug.WriteLine(MyDebugging.ResultOfAPICall("CloseHandle (ReadHandle)"))
                End If

                If (writeHandle <> 0) Then
                    CloseHandle(writeHandle)
                    Debug.WriteLine(MyDebugging.ResultOfAPICall("CloseHandle (WriteHandle)"))
                End If
            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Sub


        Friend Sub PrepareForOverlappedTransfer _
            (ByRef hidOverlapped As OVERLAPPED, _
            ByRef eventObject As Integer)

            'Purpose    : Creates an event object for the overlapped structure used with 
            '           : ReadFile.
            '           ; Called before the first call to ReadFile.

            Dim Security As SECURITY_ATTRIBUTES

            Try

                'Values for the SECURITY_ATTRIBUTES structure:

                Security.lpSecurityDescriptor = 0
                Security.bInheritHandle = CInt(True)
                Security.nLength = Len(Security)

                '***
                'API function: CreateEvent

                'Purpose: Creates an event object for the overlapped structure used with ReadFile.

                'Accepts:
                'A security attributes structure.
                'Manual Reset = False (The system automatically resets the state to nonsignaled 
                'after a waiting thread has been released.)
                'Initial state = True (signaled)
                'An event object name (optional)

                'Returns: a handle to the event object
                '***

                eventObject = CreateEvent(Security, CInt(False), CInt(True), "")

                Debug.WriteLine(MyDebugging.ResultOfAPICall("CreateEvent"))
                Debug.WriteLine("")

                'Set the members of the overlapped structure.

                hidOverlapped.Offset = 0
                hidOverlapped.OffsetHigh = 0
                hidOverlapped.hEvent = eventObject
                ReadyForOverlappedTransfer = True

            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Sub


        Protected Overrides Sub ProtectedRead _
            (ByVal readHandle As Integer, _
              ByVal hidHandle As Integer, _
              ByVal writeHandle As Integer, _
              ByRef myDeviceDetected As Boolean, _
              ByRef inputReportBuffer() As Byte, _
              ByRef success As Boolean)

            'Purpose    : reads an Input report from the device using interrupt transfers.

            'Accepts    : readHandle - the handle for reading from the device.
            '             hidHandle - the handle for other device communications.
            '             myDeviceDetected - tells whether the device is currently attached.
            '             readBuffer - contains the requested report.
            '             success - read success

            Dim EventObject As Integer
            Dim HIDOverlapped As OVERLAPPED
            Dim NumberOfBytesRead As Integer
            Dim Result As Integer

            Try

                'If it's the first attempt to read, set up the overlapped structure for ReadFile.

                If ReadyForOverlappedTransfer = False Then
                    Call PrepareForOverlappedTransfer(HIDOverlapped, EventObject)
                End If

                '***
                'API function: ReadFile
                'Purpose: Attempts to read an Input report from the device.

                'Accepts:
                'A device handle returned by CreateFile
                '(for overlapped I/O, CreateFile must have been called with FILE_FLAG_OVERLAPPED),
                'A pointer to a buffer for storing the report.
                'The Input report length in bytes returned by HidP_GetCaps,
                'A pointer to a variable that will hold the number of bytes read. 
                'An overlapped structure whose hEvent member is set to an event object.

                'Returns: the report in ReadBuffer.

                'The overlapped call returns immediately, even if the data hasn't been received yet.

                'To read multiple reports with one ReadFile, increase the size of ReadBuffer
                'and use NumberOfBytesRead to determine how many reports were returned.
                'Use a larger buffer if the application can't keep up with reading each report
                'individually. 
                '***
                Debug.Write("input report length = " & UBound(inputReportBuffer) + 1)

                Result = ReadFile _
                    (readHandle, _
                    inputReportBuffer(0), _
                    UBound(inputReportBuffer) + 1, _
                    NumberOfBytesRead, _
                    HIDOverlapped)

                'Result = ReadFile _
                '    (readHandle, _
                '    inputReportBuffer(0), _
                '    UBound(inputReportBuffer) + 1, _
                '    NumberOfBytesRead, _
                '    Nothing)

                'Dim i As Integer = UBound(inputReportBuffer)
                'While (i > 0) AndAlso (inputReportBuffer(i) = 0)
                '    i -= 1
                'End While
                'NumberOfBytesRead = i + 1

                'ReDim Preserve inputReportBuffer(NumberOfBytesRead - 1)

                Debug.WriteLine(MyDebugging.ResultOfAPICall("ReadFile"))
                Debug.WriteLine("")
                Debug.WriteLine("waiting for ReadFile")

                'API function: WaitForSingleObject

                'Purpose: waits for at least one report or a timeout.
                'Used with overlapped ReadFile.

                'Accepts:
                'An event object created with CreateEvent
                'A timeout value in milliseconds.

                'Returns: A result code.

                Result = WaitForSingleObject(EventObject, 60000)
                Debug.WriteLine(MyDebugging.ResultOfAPICall("WaitForSingleObject"))
                Debug.WriteLine("")

                'Find out if ReadFile completed or timeout.

                Select Case Result
                    Case WAIT_OBJECT_0

                        'ReadFile has completed

                        Debug.WriteLine("")
                        success = True
                        Debug.WriteLine("ReadFile completed successfully.")
                    Case WAIT_TIMEOUT

                        'Cancel the operation on timeout

                        Call CancelTransfer(readHandle, hidHandle, writeHandle)
                        Debug.WriteLine("Readfile timeout")
                        Debug.WriteLine("")
                        success = False
                        myDeviceDetected = False
                    Case Else

                        'Cancel the operation on other error.

                        Call CancelTransfer(readHandle, hidHandle, writeHandle)
                        Debug.WriteLine("")
                        Debug.WriteLine("Readfile undefined error")
                        success = False
                        myDeviceDetected = False
                End Select

                If Result = 0 Then
                    success = True
                Else
                    success = False
                End If

            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Sub
    End Class


    Friend Class InputReportViaControlTransfer
        Inherits DeviceReport

        Protected Overrides Sub ProtectedRead _
            (ByVal readHandle As Integer, _
            ByVal hidHandle As Integer, _
            ByVal writeHandle As Integer, _
            ByRef myDeviceDetected As Boolean, _
            ByRef inputReportBuffer() As Byte, _
            ByRef success As Boolean)

            'Purpose    : reads an Input report from the device using a control transfer.

            'Accepts    : readHandle - the handle for reading from the device.
            '             hidHandle - the handle for other device communications.
            '             myDeviceDetected - tells whether the device is currently attached.
            '             readBuffer - contains the requested report.
            '             success - read success

            Try

                '***
                'API function: HidD_GetInputReport

                'Purpose: Attempts to read an Input report from the device using a control transfer.
                'Supported under Windows XP and later only.

                'Requires:
                'A handle to a HID
                'A pointer to a buffer containing the report ID and report
                'The size of the buffer. 

                'Returns: true on success, false on failure.
                '***

                success = HidD_GetInputReport _
                   (hidHandle, _
                   inputReportBuffer(0), _
                   UBound(inputReportBuffer) + 1)

                Debug.WriteLine(MyDebugging.ResultOfAPICall("ReadFile"))
                Debug.WriteLine("")

            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Sub

    End Class


    Friend MustInherit Class HostReport

        'For reports the host sends to the device.

        'Each report class defines a ProtectedWrite method for writing a type of report.

        Protected MustOverride Function ProtectedWrite _
            (ByVal deviceHandle As Integer, _
            ByVal reportBuffer() As Byte) _
            As Boolean


        Friend Function Write _
            (ByVal reportBuffer() As Byte, _
            ByVal deviceHandle As Integer) _
            As Boolean

            Dim Success As Boolean

            'Purpose    : Calls the overridden ProtectedWrite routine.
            '           : This method enables other classes to override ProtectedWrite 
            '           : while limiting access as Friend.
            '           : (Directly declaring Write as Friend MustOverride causes the 
            '           : compiler(warning) "Other languages may permit Friend 
            '           : Overridable members to be overridden.")

            'Accepts    : reportBuffer - contains the report ID and report data.
            '           : deviceHandle - handle to the device.             '             

            'Returns    : True on success. False on failure.

            Try
                Success = ProtectedWrite _
                    (deviceHandle, _
                    reportBuffer)

                Return Success

            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Function
    End Class


    Class OutFeatureReport
        Inherits HostReport

        'For Feature reports the host sends to the device.

        Protected Overrides Function ProtectedWrite _
            (ByVal hidHandle As Integer, _
            ByVal outFeatureReportBuffer() As Byte) _
            As Boolean

            'Purpose    : writes a Feature report to the device.

            'Accepts    : hidHandle - a handle to the device.
            '             featureReportBuffer - contains the report ID and report to send.

            'Returns    : True on success. False on failure.

            Dim Count As Short
            Dim Success As Boolean

            Try
                '***
                'API function: HidD_SetFeature

                'Purpose: Attempts to send a Feature report to the device.

                'Accepts:
                'A handle to a HID
                'A pointer to a buffer containing the report ID and report
                'The size of the buffer. 

                'Returns: true on success, false on failure.
                '***

                Success = HidD_SetFeature _
                    (hidHandle, _
                    outFeatureReportBuffer(0), _
                    UBound(outFeatureReportBuffer) + 1)


                Debug.WriteLine(MyDebugging.ResultOfAPICall("Hidd_SetFeature"))
                Debug.WriteLine("")
                Debug.WriteLine(" FeatureReportByteLength = " & UBound(OutFeatureReportBuffer) + 1)
                Debug.WriteLine(" Report ID: " & OutFeatureReportBuffer(0))
                Debug.WriteLine(" Report Data:")

                For Count = 1 To CShort(UBound(OutFeatureReportBuffer))
                    Debug.WriteLine(" " & Hex(OutFeatureReportBuffer(Count)))
                Next Count

                Return Success

            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Function

    End Class


    Class OutputReport
        Inherits HostReport

        'For Output reports the host sends to the device.
        'Uses interrupt or control transfers depending on the device and OS.

        Protected Overrides Function ProtectedWrite _
            (ByVal writeHandle As Integer, _
            ByVal outputReportBuffer() As Byte) _
            As Boolean

            'Purpose    : writes an Output report to the device.

            'Accepts    : writeHandle - a handle to the device.
            '             OutputReportBuffer - contains the report ID and report to send.

            'Returns    : True on success. False on failure.

            Dim Count As Short
            Dim NumberOfBytesWritten As Integer
            Dim Result As Boolean
          
            Try
                'The host will use an interrupt transfer if the the HID has an interrupt OUT
                'endpoint (requires USB 1.1 or later) AND the OS is NOT Windows 98 Gold (original version). 
                'Otherwise the the host will use a control transfer.
                'The application doesn't have to know or care which type of transfer is used.

                NumberOfBytesWritten = 0

                '***
                'API function: WriteFile

                'Purpose: writes an Output report to the device.

                'Accepts:
                'A handle returned by CreateFile
                'The output report byte length returned by HidP_GetCaps.
                'An integer to hold the number of bytes written.

                'Returns: True on success, False on failure.
                '***

                Result = WriteFile _
                    (writeHandle, _
                    outputReportBuffer(0), _
                    UBound(outputReportBuffer) + 1, _
                    NumberOfBytesWritten, _
                    0)

                Debug.WriteLine(MyDebugging.ResultOfAPICall("WriteFile"))
                Debug.WriteLine("")
                Debug.WriteLine(" OutputReportByteLength = " & (UBound(outputReportBuffer) + 1))
                Debug.WriteLine(" NumberOfBytesWritten = " & NumberOfBytesWritten)

                If (Result = True) Then

                    'On success, display the data written.

                    Debug.WriteLine(" Report ID: " & outputReportBuffer(0))
                    Debug.WriteLine(" Report Data:")

                    For Count = 1 To CShort(UBound(outputReportBuffer))
                        Debug.WriteLine(" " & Hex(outputReportBuffer(Count)))
                    Next Count
                Else
                    'On failure, close the handles.

                    If (writeHandle <> 0) Then
                        CloseHandle(writeHandle)
                        Debug.WriteLine(MyDebugging.ResultOfAPICall("CloseHandle (WriteHandle)"))
                    End If
                End If

                'Return True on success, False on failure.

                Return CBool(Result)

            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Function

    End Class


    Class OutputReportViaControlTransfer
        Inherits HostReport

        Protected Overrides Function ProtectedWrite _
                (ByVal hidHandle As Integer, _
                ByVal outputReportBuffer() As Byte) _
                As Boolean

            'Purpose    : writes an Output report to the device using a control transfer.

            'Accepts    : hidHandle - a handle to the device.
            '             outputReportBuffer - contains the report ID and report to send.

            'Returns    : True on success. False on failure.

            Dim Success As Boolean
            Dim Count As Short

            Try
                '***
                'API function: HidD_SetOutputReport

                'Purpose: 
                'Attempts to send an Output report to the device using a control transfer.
                'Requires Windows XP or later.

                'Accepts:
                'A handle to a HID
                'A pointer to a buffer containing the report ID and report
                'The size of the buffer. 

                'Returns: true on success, false on failure.
                '***

                Success = HidD_SetOutputReport _
                    (hidHandle, _
                    outputReportBuffer(0), _
                    UBound(outputReportBuffer) + 1)


                Debug.WriteLine(MyDebugging.ResultOfAPICall("Hidd_SetFeature"))
                Debug.WriteLine("")
                Debug.WriteLine(" OutputReportByteLength = " & UBound(OutputReportBuffer) + 1)
                Debug.WriteLine(" Report ID: " & OutputReportBuffer(0))
                Debug.WriteLine(" Report Data:")

                For Count = 1 To CShort(UBound(OutputReportBuffer))
                    Debug.WriteLine(" " & Hex(OutputReportBuffer(Count)))
                Next Count

                Return Success

            Catch ex As Exception
                Call HandleException(ModuleName, ex)
            End Try

        End Function

    End Class


    Friend Function FlushQueue _
        (ByVal hidHandle As Integer) _
        As Boolean

        'Purpose    : Remove any Input reports waiting in the buffer.

        'Accepts    : hidHandle - a handle to a device.           

        'Returns    : True on success, False on failure.

        Dim Result As Boolean

        Try
            '***
            'API function: HidD_FlushQueue

            'Purpose: Removes any Input reports waiting in the buffer.

            'Accepts: a handle to the device.

            'Returns: True on success, False on failure.
            '***

            Result = HidD_FlushQueue(HIDHandle)

            Debug.WriteLine(MyDebugging.ResultOfAPICall("HidD_FlushQueue, ReadHandle"))
            Debug.WriteLine("Result = " & Result)

            Return Result

        Catch ex As Exception
            Call HandleException(ModuleName, ex)
        End Try

    End Function


    Friend Function GetDeviceCapabilities _
        (ByVal hidHandle As Integer) _
        As HIDP_CAPS

        'Purpose    : Retrieves a structure with information about a device's capabilities. 

        'Accepts    : HIDHandle - a handle to a device.      

        'Returns    : An HIDP_CAPS structure.

        Dim PreparsedDataBytes(29) As Byte
        Dim PreparsedDataString As String
        Dim PreparsedDataPointer As IntPtr
        Dim Result As Integer
        Dim Success As Boolean
        Dim ValueCaps(1023) As Byte '(the array size is a guess)

        Try

            '***
            'API function: HidD_GetPreparsedData

            'Purpose: retrieves a pointer to a buffer containing information about the device's capabilities.
            'HidP_GetCaps and other API functions require a pointer to the buffer.

            'Requires: 
            'A handle returned by CreateFile.
            'A pointer to a buffer.

            'Returns:
            'True on success, False on failure.
            '***

            Success = HidD_GetPreparsedData(HIDHandle, PreparsedDataPointer)

            Debug.WriteLine(MyDebugging.ResultOfAPICall("HidD_GetPreparsedData"))
            Debug.WriteLine("")

            'Copy the data at PreparsedDataPointer into a byte array.

            PreparsedDataString = System.Convert.ToBase64String(PreparsedDataBytes)


            '***
            'API function: HidP_GetCaps

            'Purpose: find out a device's capabilities.
            'For standard devices such as joysticks, you can find out the specific
            'capabilities of the device.
            'For a custom device where the software knows what the device is capable of,
            'this call may be unneeded.

            'Accepts:
            'A pointer returned by HidD_GetPreparsedData
            'A pointer to a HIDP_CAPS structure.

            'Returns: True on success, False on failure.
            '***

            Result = HidP_GetCaps(PreparsedDataPointer, Capabilities)
            If (Result <> 0) Then
                ' Debug data:
                Debug.WriteLine(MyDebugging.ResultOfAPICall("HidP_GetCaps"))

                Debug.WriteLine("")

                Debug.WriteLine("  Usage: " & Hex(Capabilities.Usage))
                Debug.WriteLine("  Usage Page: " & Hex(Capabilities.UsagePage))
                Debug.WriteLine("  Input Report Byte Length: " & Capabilities.InputReportByteLength)
                Debug.WriteLine("  Output Report Byte Length: " & Capabilities.OutputReportByteLength)
                Debug.WriteLine("  Feature Report Byte Length: " & Capabilities.FeatureReportByteLength)
                Debug.WriteLine("  Number of Link Collection Nodes: " & Capabilities.NumberLinkCollectionNodes)
                Debug.WriteLine("  Number of Input Button Caps: " & Capabilities.NumberInputButtonCaps)
                Debug.WriteLine("  Number of Input Value Caps: " & Capabilities.NumberInputValueCaps)
                Debug.WriteLine("  Number of Input Data Indices: " & Capabilities.NumberInputDataIndices)
                Debug.WriteLine("  Number of Output Button Caps: " & Capabilities.NumberOutputButtonCaps)
                Debug.WriteLine("  Number of Output Value Caps: " & Capabilities.NumberOutputValueCaps)
                Debug.WriteLine("  Number of Output Data Indices: " & Capabilities.NumberOutputDataIndices)
                Debug.WriteLine("  Number of Feature Button Caps: " & Capabilities.NumberFeatureButtonCaps)
                Debug.WriteLine("  Number of Feature Value Caps: " & Capabilities.NumberFeatureValueCaps)
                Debug.WriteLine("  Number of Feature Data Indices: " & Capabilities.NumberFeatureDataIndices)

                '***
                'API function: HidP_GetValueCaps

                'Purpose: retrieves a buffer containing an array of HidP_ValueCaps structures.
                'Each structure defines the capabilities of one value.
                'This application doesn't use this data.

                'Accepts:
                'A report type enumerator from hidpi.h,
                'A pointer to a buffer for the returned array,
                'The NumberInputValueCaps member of the device's HidP_Caps structure,
                'A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.

                'Returns: True on success, False on failure.
                '***

                Result = HidP_GetValueCaps _
                    (HidP_Input, _
                    ValueCaps(0), _
                    Capabilities.NumberInputValueCaps, _
                    PreparsedDataPointer)


                Debug.WriteLine(MyDebugging.ResultOfAPICall("HidP_GetValueCaps"))
                Debug.WriteLine("")

                '(To use this data, copy the ValueCaps byte array into an array of structures.)


                '***
                'API function: HidD_FreePreparsedData

                'Purpose: frees the buffer reserved by HidD_GetPreparsedData.

                'Accepts: A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.

                'Returns: True on success, False on failure.
                '***

                Success = HidD_FreePreparsedData(PreparsedDataPointer)


                Debug.WriteLine(MyDebugging.ResultOfAPICall("HidD_FreePreparsedData"))
                Debug.WriteLine("")

            End If

        Catch ex As Exception
            Call HandleException(ModuleName, ex)
        End Try

        Return Capabilities

    End Function


    Friend Function GetHIDUsage _
        (ByVal MyCapabilities As HIDP_CAPS) _
        As String

        'Purpose    : Creates a 32-bit Usage from the Usage Page and Usage ID. 
        '           : Determines whether the Usage is a system mouse or keyboard.
        '           : Can be modified to detect other Usages.

        'Accepts    : MyCapabilities - a HIDP_CAPS structure retrieved with HidP_GetCaps.      

        'Returns    : A string describing the Usage.

        Dim Usage As Integer
        Dim UsageDescription As String = ""

        Try

            'Create32-bit Usage from Usage Page and Usage ID.

            Usage = MyCapabilities.UsagePage * 256 + MyCapabilities.Usage

            If Usage = CInt(&H102) Then UsageDescription = "mouse"
            If Usage = CInt(&H106) Then UsageDescription = "keyboard"

        Catch ex As Exception
            Call HandleException(ModuleName, ex)
        End Try

        Return UsageDescription

    End Function


    Friend Function GetNumberOfInputBuffers _
      (ByVal hidDeviceObject As Integer, _
      ByRef numberOfInputBuffers As Integer) _
      As Boolean

        'Purpose    : Retrieves the number of Input reports the host can store.

        'Accepts    : hidDeviceObject - a handle to a device 
        '           : numberBuffers - an integer to hold the returned value.          

        'Returns    : True on success, False on failure.

        Dim Success As Boolean

        Try

            If Not (IsWindows98Gold()) Then
                '***
                'API function: HidD_GetNumInputBuffers

                'Purpose: retrieves the number of Input reports the host can store.
                'Not supported by Windows 98 Gold.
                'If the buffer is full and another report arrives, the host drops the 
                'oldest report.

                'Accepts: a handle to a device and an integer to hold the number of buffers. 

                'Returns: True on success, False on failure.
                '***

                Success = HidD_GetNumInputBuffers _
                     (hidDeviceObject, _
                     numberOfInputBuffers)

            Else

                'Under Windows 98 Gold, the number of buffers is fixed at 2.

                numberOfInputBuffers = 2
                Success = True
            End If

            Return Success

        Catch ex As Exception
            Call HandleException(ModuleName, ex)
        End Try

    End Function


    Friend Function SetNumberOfInputBuffers _
         (ByVal hidDeviceObject As Integer, _
         ByVal numberBuffers As Integer) _
         As Boolean

        'Purpose    : sets the number of input reports the host will store.
        '           : Requires Windows XP or later.

        'Accepts    : hidDeviceObject - a handle to the device.
        '           : numberBuffers - the requested number of input reports. 

        'Returns    : True on success. False on failure.

        Dim Success As Boolean

        Try

            If Not (IsWindows98Gold()) Then

                '***
                'API function: HidD_SetNumInputBuffers

                'Purpose: Sets the number of Input reports the host can store.
                'If the buffer is full and another report arrives, the host drops the 
                'oldest report.

                'Requires:
                'A handle to a HID
                'An integer to hold the number of buffers. 

                'Returns: true on success, false on failure.
                '***

                HidD_SetNumInputBuffers _
                     (hidDeviceObject, _
                     numberBuffers)

                Return Success

            Else

                'Not supported under Windows 98 Gold.

                Return False
            End If

        Catch ex As Exception
            Call HandleException(ModuleName, ex)
        End Try

    End Function


    Friend Function IsWindowsXpOrLater() As Boolean

        'Find out if the current operating system is Windows XP or later.
        '(Windows XP or later is required for HidD_GetInputReport and HidD_SetInputReport.)

        Try
            Dim MyEnvironment As OperatingSystem = Environment.OSVersion

            'Windows XP is version 5.1.
            Dim VersionXP As New System.Version(5, 1)

            If (Version.op_GreaterThanOrEqual(MyEnvironment.Version, VersionXP) = True) Then
                Debug.Write("The OS is Windows XP or later.")
                Return True
            Else
                Debug.Write("The OS is earlier than Windows XP.")
                Return False
            End If

        Catch ex As Exception
            Call HandleException(ModuleName, ex)
        End Try

    End Function

    Friend Function IsWindows98Gold() As Boolean

        'Find out if the current operating system is Windows 98 Gold (original version).
        'Windows 98 Gold does not support the following:
        '  Interrupt OUT transfers (WriteFile uses control transfers and Set_Report).
        '  HidD_GetNumInputBuffers and HidD_SetNumInputBuffers

        '(Not yet tested on a Windows 98 Gold system.)

        Try
            Dim MyEnvironment As OperatingSystem = Environment.OSVersion

            'Windows 98 Gold is version 4.10 with a build number less than 2183.

            Dim Version98SE As New System.Version(4, 10, 2183)

            If (Version.op_LessThan(MyEnvironment.Version, Version98SE) = True) Then
                Debug.Write("The OS is Windows 98 Gold.")
                Return True
            Else
                Debug.Write("The OS is more recent than Windows 98 Gold.")
                Return False
            End If

        Catch ex As Exception
            Call HandleException(ModuleName, ex)
        End Try

    End Function

    Shared Sub HandleException(ByVal moduleName As String, ByVal e As Exception)

        'Purpose    : Provides a central mechanism for exception handling.
        '           : Displays a message box that describes the exception.

        'Accepts    : moduleName - the module where the exception occurred.
        '           : e - the exception

        Dim Message As String
        Dim Caption As String

        Try
            'Create an error message.

            Message = "Exception: " & e.Message & ControlChars.CrLf & _
            "Module: " & moduleName & ControlChars.CrLf & _
             "Method: " & e.TargetSite.Name

            'Specify a caption.

            Caption = "Unexpected Exception"

            'Display the message in a message box.

            'MessageBox.Show(Message, Caption, MessageBoxButtons.OK)
            Debug.Write(Message)
        Finally
        End Try
    End Sub

End Class
