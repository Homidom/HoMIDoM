Option Strict On
Option Explicit On 
Imports System.Runtime.InteropServices

Module HidApiDeclarations

    'API Declarations for communicating with HID-class devices.

    '******************************************************************************
    'API constants
    '******************************************************************************

    'from hidpi.h
    'Typedef enum defines a set of integer constants for HidP_Report_Type
    Public Const HidP_Input As Short = 0
    Public Const HidP_Output As Short = 1
    Public Const HidP_Feature As Short = 2

    '******************************************************************************
    'Structures and classes for API calls, listed alphabetically
    '******************************************************************************

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure HIDD_ATTRIBUTES
        Dim Size As Integer
        Dim VendorID As Short
        Dim ProductID As Short
        Dim VersionNumber As Short
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure HIDP_CAPS
        Dim Usage As Short
        Dim UsagePage As Short
        Dim InputReportByteLength As Short
        Dim OutputReportByteLength As Short
        Dim FeatureReportByteLength As Short
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=17)> Dim Reserved() As Short
        Dim NumberLinkCollectionNodes As Short
        Dim NumberInputButtonCaps As Short
        Dim NumberInputValueCaps As Short
        Dim NumberInputDataIndices As Short
        Dim NumberOutputButtonCaps As Short
        Dim NumberOutputValueCaps As Short
        Dim NumberOutputDataIndices As Short
        Dim NumberFeatureButtonCaps As Short
        Dim NumberFeatureValueCaps As Short
        Dim NumberFeatureDataIndices As Short

    End Structure

    'If IsRange is false, UsageMin is the Usage and UsageMax is unused.
    'If IsStringRange is false, StringMin is the string index and StringMax is unused.
    'If IsDesignatorRange is false, DesignatorMin is the designator index and DesignatorMax is unused.

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure HidP_Value_Caps
        Dim UsagePage As Short
        Dim ReportID As Byte
        Dim IsAlias As Integer
        Dim BitField As Short
        Dim LinkCollection As Short
        Dim LinkUsage As Short
        Dim LinkUsagePage As Short
        Dim IsRange As Integer
        Dim IsStringRange As Integer
        Dim IsDesignatorRange As Integer
        Dim IsAbsolute As Integer
        Dim HasNull As Integer
        Dim Reserved As Byte
        Dim BitSize As Short
        Dim ReportCount As Short
        Dim Reserved2 As Short
        Dim Reserved3 As Short
        Dim Reserved4 As Short
        Dim Reserved5 As Short
        Dim Reserved6 As Short
        Dim LogicalMin As Integer
        Dim LogicalMax As Integer
        Dim PhysicalMin As Integer
        Dim PhysicalMax As Integer
        Dim UsageMin As Short
        Dim UsageMax As Short
        Dim StringMin As Short
        Dim StringMax As Short
        Dim DesignatorMin As Short
        Dim DesignatorMax As Short
        Dim DataIndexMin As Short
        Dim DataIndexMax As Short
    End Structure

    '******************************************************************************
    'API functions, listed alphabetically
    '******************************************************************************

    <DllImport("hid.dll")> Function HidD_FlushQueue _
        (ByVal HidDeviceObject As Integer) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Function HidD_FreePreparsedData _
        (ByRef PreparsedData As IntPtr) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Function HidD_GetAttributes _
        (ByVal HidDeviceObject As Integer, _
        ByRef Attributes As HIDD_ATTRIBUTES) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Function HidD_GetFeature _
        (ByVal HidDeviceObject As Integer, _
        ByRef lpReportBuffer As Byte, _
        ByVal ReportBufferLength As Integer) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Function HidD_GetInputReport _
        (ByVal HidDeviceObject As Integer, _
        ByRef lpReportBuffer As Byte, _
        ByVal ReportBufferLength As Integer) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Sub HidD_GetHidGuid _
        (ByRef HidGuid As System.Guid)
    End Sub

    <DllImport("hid.dll")> Function HidD_GetNumInputBuffers _
        (ByVal HidDeviceObject As Integer, _
        ByRef NumberBuffers As Integer) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Function HidD_GetPreparsedData _
        (ByVal HidDeviceObject As Integer, _
        ByRef PreparsedData As IntPtr) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Function HidD_SetFeature _
        (ByVal HidDeviceObject As Integer, _
        ByRef lpReportBuffer As Byte, _
        ByVal ReportBufferLength As Integer) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Function HidD_SetNumInputBuffers _
        (ByVal HidDeviceObject As Integer, _
        ByVal NumberBuffers As Integer) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Function HidD_SetOutputReport _
        (ByVal HidDeviceObject As Integer, _
        ByRef lpReportBuffer As Byte, _
        ByVal ReportBufferLength As Integer) _
        As Boolean
    End Function

    <DllImport("hid.dll")> Function HidP_GetCaps _
        (ByVal PreparsedData As IntPtr, _
        ByRef Capabilities As HIDP_CAPS) _
        As Integer
    End Function

    <DllImport("hid.dll")> Function HidP_GetValueCaps _
        (ByVal ReportType As Short, _
        ByRef ValueCaps As Byte, _
        ByRef ValueCapsLength As Short, _
        ByVal PreparsedData As IntPtr) _
        As Integer
    End Function

End Module
