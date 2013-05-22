Option Strict On
Option Explicit On 

Imports System.Runtime.InteropServices

Module DebuggingDeclarations

    'API declarations relating to Windows error messages.

    '******************************************************************************
    'API constants
    '******************************************************************************

    Public Const FORMAT_MESSAGE_FROM_SYSTEM As Short = &H1000S

    '******************************************************************************
    'API functions, listed alphabetically
    '******************************************************************************

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto)> Function FormatMessage _
            (ByVal dwFlags As Integer, _
            ByRef lpSource As Long, _
            ByVal dwMessageId As Integer, _
            ByVal dwLanguageZId As Integer, _
            ByVal lpBuffer As String, _
            ByVal nSize As Integer, _
            ByVal Arguments As Integer) _
            As Integer
    End Function

End Module
