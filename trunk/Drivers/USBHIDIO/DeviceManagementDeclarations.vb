Option Strict On
Option Explicit On 
Imports System.Runtime.InteropServices

Module DeviceManagementApiDeclarations

    'API declarations relating to device management (SetupDixxx and 
    'RegisterDeviceNotification functions).

    '******************************************************************************
    'API constants
    '******************************************************************************

    'from dbt.h
    Public Const DBT_DEVNODES_CHANGED As Long = &H7
    Public Const DBT_DEVICEARRIVAL As Integer = &H8000
    Public Const DBT_DEVICEREMOVECOMPLETE As Integer = &H8004
    Public Const DBT_DEVTYP_DEVICEINTERFACE As Integer = 5
    Public Const DBT_DEVTYP_HANDLE As Integer = 6
    Public Const DEVICE_NOTIFY_ALL_INTERFACE_CLASSES As Integer = 4
    Public Const DEVICE_NOTIFY_SERVICE_HANDLE As Integer = 1
    Public Const DEVICE_NOTIFY_WINDOW_HANDLE As Integer = 0
    Public Const WM_DEVICECHANGE As Integer = &H219

    'from setupapi.h
    Public Const DIGCF_PRESENT As Short = &H2S
    Public Const DIGCF_DEVICEINTERFACE As Short = &H10S

    '******************************************************************************
    'Structures and classes for API calls, listed alphabetically
    '******************************************************************************

    'There are two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.

    'Use this in the call to RegisterDeviceNotification() and
    'in checking dbch_devicetype in a DEV_BROADCAST_HDR structure.
    <StructLayout(LayoutKind.Sequential)> _
    Public Class DEV_BROADCAST_DEVICEINTERFACE
        Public dbcc_size As Integer
        Public dbcc_devicetype As Integer
        Public dbcc_reserved As Integer
        Public dbcc_classguid As Guid
        Public dbcc_name As Short
    End Class

    'Use this to read the dbcc_name string and classguid.
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
       Public Class DEV_BROADCAST_DEVICEINTERFACE_1
        Public dbcc_size As Integer
        Public dbcc_devicetype As Integer
        Public dbcc_reserved As Integer
        <MarshalAs(UnmanagedType.ByValArray, ArraySubType:=UnmanagedType.U1, SizeConst:=16)> _
        Public dbcc_classguid() As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=255)> _
        Public dbcc_name() As Char
    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Public Class DEV_BROADCAST_HANDLE
        Public dbch_size As Integer
        Public dbch_devicetype As Integer
        Public dbch_reserved As Integer
        Public dbch_handle As Integer
        Public dbch_hdevnotify As Integer
    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Public Class DEV_BROADCAST_HDR
        Public dbch_size As Integer
        Public dbch_devicetype As Integer
        Public dbch_reserved As Integer
    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure SP_DEVICE_INTERFACE_DATA
        Dim cbSize As Integer
        Dim InterfaceClassGuid As System.Guid
        Dim Flags As Integer
        Dim Reserved As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure SP_DEVICE_INTERFACE_DETAIL_DATA
        Dim cbSize As Integer
        Dim DevicePath As String
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure SP_DEVINFO_DATA
        Dim cbSize As Integer
        Dim ClassGuid As System.Guid
        Dim DevInst As Integer
        Dim Reserved As Integer
    End Structure

    '******************************************************************************
    'API functions, listed alphabetically
    '******************************************************************************

    <DllImport("user32.dll", CharSet:=CharSet.Auto)> Function RegisterDeviceNotification _
        (ByVal hRecipient As IntPtr, _
        ByVal NotificationFilter As IntPtr, _
        ByVal Flags As Int32) _
        As IntPtr
    End Function

    <DllImport("setupapi.dll")> Function SetupDiCreateDeviceInfoList _
        (ByRef ClassGuid As System.Guid, _
        ByVal hwndParent As Integer) _
        As Integer
    End Function

    <DllImport("setupapi.dll")> Function SetupDiDestroyDeviceInfoList _
        (ByVal DeviceInfoSet As IntPtr) _
        As Integer
    End Function

    <DllImport("setupapi.dll")> Function SetupDiEnumDeviceInterfaces _
        (ByVal DeviceInfoSet As IntPtr, _
        ByVal DeviceInfoData As Integer, _
        ByRef InterfaceClassGuid As System.Guid, _
        ByVal MemberIndex As Integer, _
        ByRef DeviceInterfaceData As SP_DEVICE_INTERFACE_DATA) _
        As Boolean
    End Function

    <DllImport("setupapi.dll", CharSet:=CharSet.Auto)> Function SetupDiGetClassDevs _
        (ByRef ClassGuid As System.Guid, _
        ByVal Enumerator As String, _
        ByVal hwndParent As Integer, _
        ByVal Flags As Integer) _
        As IntPtr
    End Function

    <DllImport("setupapi.dll", CharSet:=CharSet.Auto)> Function SetupDiGetDeviceInterfaceDetail _
        (ByVal DeviceInfoSet As IntPtr, _
        ByRef DeviceInterfaceData As SP_DEVICE_INTERFACE_DATA, _
        ByVal DeviceInterfaceDetailData As IntPtr, _
        ByVal DeviceInterfaceDetailDataSize As Integer, _
        ByRef RequiredSize As Integer, _
        ByVal DeviceInfoData As IntPtr) _
        As Boolean
    End Function

    <DllImport("user32.dll")> Function UnregisterDeviceNotification _
        (ByVal Handle As IntPtr) _
    As Boolean
    End Function

End Module
