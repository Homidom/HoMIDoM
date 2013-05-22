Option Strict On
Option Explicit On

Public Class Debugging

    'Used only in Debug.Write statements.

    Friend Function ResultOfAPICall _
        (ByRef FunctionName As String) _
        As String

        'Purpose    : Get text that describes the result of an API call.

        'Accepts    : FunctionName - the name of the API function.

        'Returns    : The text.

        Dim Bytes As Integer
        Dim ResultCode As Integer
        Dim ResultString As String = ""

        Try
            ResultString = New String(Chr(0), 129)

            'Returns the result code for the last API call.

            ResultCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error

            'Get the result message that corresponds to the code.

            Bytes = FormatMessage _
                (FORMAT_MESSAGE_FROM_SYSTEM, _
                0, _
                ResultCode, _
                0, _
                ResultString, _
                128, _
                0)

            'Subtract two characters from the message to strip the CR and LF.

            If Bytes > 2 Then
                ResultString = ResultString.Remove(Bytes - 2, 2)
            End If

            'Create the string to return.

            ResultString = vbCrLf & FunctionName & vbCrLf & _
                "Result = " & ResultString & vbCrLf

        Catch ex As Exception

        End Try

        Return ResultString

    End Function
End Class
