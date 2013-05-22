Option Strict On
Option Explicit On 
Imports System.Runtime.InteropServices

Module FileIOApiDeclarations

    'API declarations relating to file I/O.

    '******************************************************************************
    'API constants
    '******************************************************************************

    Public Const FILE_FLAG_OVERLAPPED As Integer = &H40000000
    Public Const FILE_SHARE_READ As Short = &H1S
    Public Const FILE_SHARE_WRITE As Short = &H2S
    Public Const GENERIC_READ As Integer = &H80000000
    Public Const GENERIC_WRITE As Integer = &H40000000
    Public Const INVALID_HANDLE_VALUE As Integer = -1
    Public Const OPEN_EXISTING As Short = 3
    Public Const WAIT_TIMEOUT As Integer = &H102
    Public Const WAIT_OBJECT_0 As Short = 0

    '******************************************************************************
    'Structures and classes for API calls, listed alphabetically
    '******************************************************************************

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure OVERLAPPED
        Dim Internal As Integer
        Dim InternalHigh As Integer
        Dim Offset As Integer
        Dim OffsetHigh As Integer
        Dim hEvent As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure SECURITY_ATTRIBUTES
        Dim nLength As Integer
        Dim lpSecurityDescriptor As Integer
        Dim bInheritHandle As Integer
    End Structure

    '******************************************************************************
    'API functions, listed alphabetically
    '******************************************************************************

    <DllImport("kernel32.dll")> Function CancelIo _
        (ByVal hFile As Integer) _
            As Integer
    End Function

    <DllImport("kernel32.dll")> Function CloseHandle _
        (ByVal hObject As Integer) _
        As Boolean
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto)> Function CreateEvent _
        (ByRef SecurityAttributes As SECURITY_ATTRIBUTES, _
        ByVal bManualReset As Integer, _
        ByVal bInitialState As Integer, _
        ByVal lpName As String) _
        As Integer
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto)> Function CreateFile _
        (ByVal lpFileName As String, _
        ByVal dwDesiredAccess As Integer, _
        ByVal dwShareMode As Integer, _
        ByRef lpSecurityAttributes As SECURITY_ATTRIBUTES, _
        ByVal dwCreationDisposition As Integer, _
        ByVal dwFlagsAndAttributes As Integer, _
        ByVal hTemplateFile As Integer) _
        As Integer
    End Function

    <DllImport("kernel32.dll")> Function ReadFile _
      (ByVal hFile As Integer, _
      ByRef lpBuffer As Byte, _
      ByVal nNumberOfBytesToRead As Integer, _
      ByRef lpNumberOfBytesRead As Integer, _
      ByRef lpOverlapped As OVERLAPPED) _
      As Integer
    End Function

    <DllImport("kernel32.dll")> Function WaitForSingleObject _
     (ByVal hHandle As Integer, _
     ByVal dwMilliseconds As Integer) _
     As Integer
    End Function

    <DllImport("kernel32.dll")> Function WriteFile _
        (ByVal hFile As Integer, _
        ByRef lpBuffer As Byte, _
        ByVal nNumberOfBytesToWrite As Integer, _
        ByRef lpNumberOfBytesWritten As Integer, _
        ByVal lpOverlapped As Integer) _
        As Boolean
    End Function

    <DllImport("kernel32.dll")> Function GetOverlappedResult _
        (ByVal hFile As Integer, _
          ByVal lpOverlapped As OVERLAPPED, _
          ByRef lpNumberOfBytesTransferred As Integer, _
          ByVal bWait As Boolean) _
        As Boolean
    End Function

End Module
