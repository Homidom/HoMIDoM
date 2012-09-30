Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.IO.Ports
Imports System.Text


' 
' * Creates a serial connection to a CurrentCost meter, and uses it
' *  to get a single update.
' * 
' * Uses the CurrentCostUpdate class to parse the data and represent
' *  it as a .NET object.
' *  
' * Dale Lane (http://dalelane.co.uk/blog)
' *  
' 


Namespace CurrentCost
    Public Class SerialPortAccess
        Public ComPort As String = "COM20"
        Public BaudRate As Integer = 9600
        Public Timeout As Integer = 12000

        Private serialPortObj As SerialPort

        Public Function GetSingleUpdate() As CurrentCostUpdate
            Try
                serialPortObj = New SerialPort()
                serialPortObj.PortName = ComPort
                serialPortObj.BaudRate = BaudRate
                serialPortObj.ReadTimeout = Timeout
                serialPortObj.DtrEnable = True
                serialPortObj.Open()

                While True
                    Dim update As New CurrentCostUpdate(serialPortObj.ReadLine())

                    If update.ValidUpdate Then
                        serialPortObj.Close()
                        Return update
                    End If
                End While
            Catch exc As TimeoutException
                Throw New ConnectTimeoutException(exc)
            Catch exc As UnauthorizedAccessException
                Throw New ConnectFailException(exc)
            Catch exc As Exception
                Throw New ConnectFailException(exc)
            End Try
        End Function


        Public Class ConnectFailException
            Inherits Exception
            Public ImplException As Exception = Nothing

            Public Sub New(ByVal exc As Exception)
                ImplException = exc
            End Sub
        End Class
        Public Class ConnectTimeoutException
            Inherits Exception
            Public ImplException As Exception = Nothing

            Public Sub New(ByVal exc As Exception)
                ImplException = exc
            End Sub
        End Class
    End Class
End Namespace
