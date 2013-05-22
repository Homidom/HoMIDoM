Option Strict On
Option Explicit On
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Friend Class DeviceManagement

    'For detecting devices and receiving device notifications.

    'Used in error messages:

    Const moduleName As String = "DeviceManagement"

    'For viewing results of API calls in debug.write statements:

    Dim MyDebugging As New Debugging()


    Friend Function DeviceNameMatch _
        (ByVal m As Message, _
        ByVal mydevicePathName As String) _
        As Boolean

        'Purpose    : Compares two device path names. Used to find out if the device name 
        '           : of a recently attached or removed device matches the name of a 
        '           : device the application is communicating with.

        'Accepts    : m - a WM_DEVICECHANGE message. A call to RegisterDeviceNotification
        '           : causes WM_DEVICECHANGE messages to be passed to an OnDeviceChange routine.
        '           : mydevicePathName - a device pathname returned by SetupDiGetDeviceInterfaceDetail
        '           : in an SP_DEVICE_INTERFACE_DETAIL_DATA structure.              

        'Returns    : True if the names match, False if not.

        Try
            Dim DevBroadcastDeviceInterface As New DEV_BROADCAST_DEVICEINTERFACE_1()
            Dim DevBroadcastHeader As New DEV_BROADCAST_HDR()

            'The LParam parameter of Message is a pointer to a DEV_BROADCAST_HDR structure.

            Marshal.PtrToStructure(m.LParam, DevBroadcastHeader)

            If (DevBroadcastHeader.dbch_devicetype = DBT_DEVTYP_DEVICEINTERFACE) Then

                'The dbch_devicetype parameter indicates that the event applies to a device interface.
                'So the structure in LParam is actually a DEV_BROADCAST_INTERFACE structure, 
                'which begins with a DEV_BROADCAST_HDR.

                'Obtain the number of characters in dbch_name by subtracting the 32 bytes
                'in the strucutre that are not part of dbch_name and dividing by 2 because there are 
                '2 bytes per character.

                Dim StringSize As Integer = CInt((DevBroadcastHeader.dbch_size - 32) / 2)

                'The dbcc_name parameter of DevBroadcastDeviceInterface contains the device name. 
                'Trim dbcc_name to match the size of the string.

                ReDim DevBroadcastDeviceInterface.dbcc_name(StringSize)

                'Marshal data from the unmanaged block pointed to by m.LParam 
                'to the managed object DevBroadcastDeviceInterface.

                Marshal.PtrToStructure(m.LParam, DevBroadcastDeviceInterface)

                'Store the device name in a String.

                Dim DeviceNameString As New String(DevBroadcastDeviceInterface.dbcc_name, 0, StringSize)

                Debug.WriteLine("Device Name =      " & DeviceNameString)
                Debug.WriteLine("")
                Debug.WriteLine("myDevicePathName = " & mydevicePathName)
                Debug.WriteLine("")

                'Compare the name of the newly attached device with the name of the device 
                'the application is accessing (mydevicePathName).
                'Set ignorecase True.

                If (String.Compare(DeviceNameString, mydevicePathName, True) = 0) Then

                    'The name matches.

                    Return True
                Else

                    'It's a different device.

                    Return False
                End If
            End If

        Catch ex As Exception
            Call HandleException(moduleName, ex)
        End Try

    End Function


    Friend Function FindDeviceFromGuid _
        (ByVal myGuid As System.Guid, _
        ByRef devicePathName() As String) _
        As Boolean

        'Purpose    : Uses SetupDi API functions to retrieve the device path name of an
        '           : attached device that belongs to an interface class.

        'Accepts    : myGuid - an interface class GUID.
        '           : devicePathName - a pointer to an array of strings that will contain 
        '           : the device path names of attached devices.  

        'Returns    : True if at least one device is found, False if not. 

        Dim DeviceFound As Boolean
        Dim DeviceInfoSet As IntPtr
        Dim LastDevice As Boolean
        Dim BufferSize As Integer
        Dim MemberIndex As Integer
        Dim MyDeviceInterfaceDetailData As SP_DEVICE_INTERFACE_DETAIL_DATA
        Dim MyDeviceInterfaceData As SP_DEVICE_INTERFACE_DATA
        Dim Result As Boolean
        Dim SingledevicePathName As String
        Dim Success As Boolean

        Try
            '***
            'API function: SetupDiGetClassDevs

            'Purpose: 
            'Retrieves a device information set for a specified group of devices.
            'SetupDiEnumDeviceInterfaces uses the device information set.

            'Accepts: 
            'An interface class GUID
            'Null to retrieve information for all device instances
            'An optional handle to a top-level window (unused here)
            'Flags to limit the returned information to currently present devices 
            'and devices that expose interfaces in the class specified by the GUID.

            'Returns:
            'A handle to a device information set for the devices.
            '***

            DeviceInfoSet = SetupDiGetClassDevs _
                (myGuid, _
                vbNullString, _
                0, _
                DIGCF_PRESENT Or DIGCF_DEVICEINTERFACE)

            Debug.WriteLine(MyDebugging.ResultOfAPICall("SetupDiClassDevs"))

            DeviceFound = False
            MemberIndex = 0

            Do

                'Begin with 0 and increment through the device information set until
                'no more devices are available.

                'The cbSize element of the MyDeviceInterfaceData structure must be set to
                'the structure's size in bytes. The size is 28 bytes.
                MyDeviceInterfaceData.cbSize = Marshal.SizeOf(MyDeviceInterfaceData)

                '***
                'API function: 
                'SetupDiEnumDeviceInterfaces()

                'Purpose: Retrieves a handle to a SP_DEVICE_INTERFACE_DATA 
                'structure for a device.
                'On return, MyDeviceInterfaceData contains the handle to a
                'SP_DEVICE_INTERFACE_DATA structure for a detected device.

                'Accepts:
                'A DeviceInfoSet returned by SetupDiGetClassDevs.
                'An interface class GUID.
                'An index to specify a device in a device information set.
                'A pointer to a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.

                'Returns:
                'Non-zero on success, zero on True.
                '***

                Result = SetupDiEnumDeviceInterfaces _
                    (DeviceInfoSet, _
                    0, _
                    myGuid, _
                    MemberIndex, _
                    MyDeviceInterfaceData)

                Debug.WriteLine(MyDebugging.ResultOfAPICall("SetupDiEnumDeviceInterfaces"))

                'Find out if a device information set was retrieved.

                If (Result = False) Then
                    LastDevice = True

                Else
                    'A device is present.

                    Debug.WriteLine("  DeviceInfoSet for device #" & CStr(MemberIndex) & ": ")
                    Debug.WriteLine("  cbSize = " & CStr(MyDeviceInterfaceData.cbSize))
                    Debug.WriteLine("  InterfaceclassGuid = " & MyDeviceInterfaceData.InterfaceClassGuid.ToString)
                    Debug.WriteLine("  Flags = " & Hex(MyDeviceInterfaceData.Flags))

                    '***
                    'API function: 
                    'SetupDiGetDeviceInterfaceDetail()

                    'Purpose:
                    'Retrieves an SP_DEVICE_INTERFACE_DETAIL_DATA structure
                    'containing information about a device.
                    'To retrieve the information, call this function twice.
                    'The first time returns the size of the structure.
                    'The second time returns a pointer to the data.

                    'Accepts:
                    'A DeviceInfoSet returned by SetupDiGetClassDevs
                    'An SP_DEVICE_INTERFACE_DATA structure returned by SetupDiEnumDeviceInterfaces
                    'A pointer to an SP_DEVICE_INTERFACE_DETAIL_DATA structure to receive information 
                    'about the specified interface.
                    'The size of the SP_DEVICE_INTERFACE_DETAIL_DATA structure.
                    'A pointer to a variable that will receive the returned required size of the 
                    'SP_DEVICE_INTERFACE_DETAIL_DATA structure.
                    'A pointer to an SP_DEVINFO_DATA structure to receive information about the device.

                    'Returns:
                    'Non-zero on success, zero on failure.
                    '***

                    MyDeviceInterfaceDetailData = Nothing

                    Success = SetupDiGetDeviceInterfaceDetail _
                        (DeviceInfoSet, _
                        MyDeviceInterfaceData, _
                        IntPtr.Zero, _
                        0, _
                        BufferSize, _
                        IntPtr.Zero)

                    Debug.WriteLine(MyDebugging.ResultOfAPICall("SetupDiGetDeviceInterfaceDetail"))
                    Debug.WriteLine("  (OK to say too small)")
                    Debug.WriteLine("  Required buffer size for the data: " & BufferSize)

                    'Store the structure's size.

                    MyDeviceInterfaceDetailData.cbSize = Marshal.SizeOf(MyDeviceInterfaceDetailData)

                    'Allocate memory for the MyDeviceInterfaceDetailData Structure using the returned buffer size.

                    Dim DetailDataBuffer As IntPtr = Marshal.AllocHGlobal(BufferSize)

                    'Store cbSize in the first 4 bytes of the array

                    Marshal.WriteInt32(DetailDataBuffer, 4 + Marshal.SystemDefaultCharSize)
                    Debug.WriteLine("cbsize = " & MyDeviceInterfaceDetailData.cbSize)

                    'Call SetupDiGetDeviceInterfaceDetail again.
                    'This time, pass a pointer to DetailDataBuffer
                    'and the returned required buffer size.

                    Success = SetupDiGetDeviceInterfaceDetail _
                        (DeviceInfoSet, _
                        MyDeviceInterfaceData, _
                        DetailDataBuffer, _
                        BufferSize, _
                        BufferSize, _
                        IntPtr.Zero)


                    Debug.WriteLine(MyDebugging.ResultOfAPICall(" Result of second call: "))
                    Debug.WriteLine("  MyDeviceInterfaceDetailData.cbSize: " & CStr(MyDeviceInterfaceDetailData.cbSize))

                    'Skip over cbsize (4 bytes) to get the address of the devicePathName.

                    Dim pdevicePathName As IntPtr = New IntPtr(DetailDataBuffer.ToInt32 + 4)

                    'Get the String containing the devicePathName.

                    SingledevicePathName = Marshal.PtrToStringAuto(pdevicePathName)
                    devicePathName(MemberIndex) = SingledevicePathName


                    Debug.WriteLine("Device Path = " & devicePathName(MemberIndex))
                    Debug.WriteLine("Device Path Length= " & Len(devicePathName(MemberIndex)))

                    'Free the memory allocated previously by AllocHGlobal.

                    Marshal.FreeHGlobal(DetailDataBuffer)
                    DeviceFound = True
                End If
                MemberIndex = MemberIndex + 1
            Loop Until (LastDevice = True)

            'Trim the array to the number of devices found.

            ReDim Preserve devicePathName(MemberIndex - 1)

            Debug.WriteLine("Number of HIDs found = " & MemberIndex - 1)

            '***
            'API function:
            'SetupDiDestroyDeviceInfoList

            'Purpose:
            'Frees the memory reserved for the DeviceInfoSet returned by SetupDiGetClassDevs.

            'Accepts:
            'A DeviceInfoSet returned by SetupDiGetClassDevs.

            'Returns:
            'True on success, False on failure.
            '***

            SetupDiDestroyDeviceInfoList _
                (DeviceInfoSet)

            Debug.WriteLine(MyDebugging.ResultOfAPICall("DestroyDeviceInfoList"))

            Return DeviceFound

        Catch ex As Exception
            Call HandleException(moduleName, ex)
        End Try

    End Function


    Friend Function RegisterForDeviceNotifications _
        (ByVal devicePathName As String, _
        ByVal formHandle As IntPtr, _
        ByVal classGuid As Guid, _
        ByRef deviceNotificationHandle As IntPtr) _
        As Boolean

        'Purpose    : Request to receive a notification when a device is attached or removed.

        'Accepts    : devicePathName - a handle to a device.
        '           : formHandle - a handle to the window that will receive device events.              
        '           : classGuid - an interface class GUID.  
        '             

        'Returns    : True on success, False on failure.

        'A DEV_BROADCAST_DEVICEINTERFACE header holds information about the request.

        Dim DevBroadcastDeviceInterface As DEV_BROADCAST_DEVICEINTERFACE = _
            New DEV_BROADCAST_DEVICEINTERFACE()
        Dim DevBroadcastDeviceInterfaceBuffer As IntPtr

        Dim size As Integer

        Try
            'Set the parameters in the DEV_BROADCAST_DEVICEINTERFACE structure.

            'Set the size.

            size = Marshal.SizeOf(DevBroadcastDeviceInterface)
            DevBroadcastDeviceInterface.dbcc_size = size

            'Request to receive notifications about a class of devices.

            DevBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE

            DevBroadcastDeviceInterface.dbcc_reserved = 0

            'Specify the interface class to receive notifications about.

            DevBroadcastDeviceInterface.dbcc_classguid = classGuid

            'Allocate memory for the buffer that holds the DEV_BROADCAST_DEVICEINTERFACE structure.

            DevBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(size)

            'Copy the DEV_BROADCAST_DEVICEINTERFACE structure to the buffer.
            'Set fDeleteOld True to prevent memory leaks.

            Marshal.StructureToPtr _
                (DevBroadcastDeviceInterface, DevBroadcastDeviceInterfaceBuffer, True)

            '***
            'API function: 
            'RegisterDeviceNotification

            'Purpose:
            'Request to receive notification messages when a device in an interface class
            'is attached or removed.

            'Accepts: 
            'Aa handle to the window that will receive device events
            'A pointer to a DEV_BROADCAST_DEVICEINTERFACE to specify the type of 
            'device to send notifications for,
            'DEVICE_NOTIFY_WINDOW_HANDLE to indicate that Handle is a window handle.

            'Returns:
            'A device notification handle or NULL on failure.
            '***

            deviceNotificationHandle = RegisterDeviceNotification _
                (formHandle, _
                DevBroadcastDeviceInterfaceBuffer, _
                DEVICE_NOTIFY_WINDOW_HANDLE)

            'Marshal data from the unmanaged block DevBroadcastDeviceInterfaceBuffer to
            'the managed object DevBroadcastDeviceInterface

            Marshal.PtrToStructure(DevBroadcastDeviceInterfaceBuffer, DevBroadcastDeviceInterface)

            'Free the memory allocated previously by AllocHGlobal.

            Marshal.FreeHGlobal(DevBroadcastDeviceInterfaceBuffer)

            'Find out if RegisterDeviceNotification was successful.

            If (deviceNotificationHandle.ToInt32 = IntPtr.Zero.ToInt32) Then
                Debug.WriteLine("RegisterDeviceNotification error")
                Return False
            Else
                Return True
            End If

        Catch ex As Exception
            Call HandleException(moduleName, ex)
        End Try
    End Function


    Friend Sub StopReceivingDeviceNotifications _
        (ByVal deviceNotificationHandle As IntPtr)

        'Purpose    : Requests to stop receiving notification messages when a device in an 
        '             interface class is attached or removed.

        'Accepts    : deviceNotificationHandle - a handle returned previously by
        '             RegisterDeviceNotification  

        Try

            '***
            'API function: UnregisterDeviceNotification

            'Purpose: Stop receiving notification messages.

            'Accepts: a handle returned previously by RegisterDeviceNotification  

            'Returns: True on success, False on failure.
            '***

            'Ignore failures.

            UnregisterDeviceNotification(deviceNotificationHandle)

        Catch ex As Exception
            Call HandleException(moduleName, ex)
        End Try

    End Sub

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
