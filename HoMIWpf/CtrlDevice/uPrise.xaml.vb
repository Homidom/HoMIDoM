Public Class uPrise
    Dim _val As Boolean

    Public Property Value As Boolean
        Get
            Return _val
        End Get
        Set(value As Boolean)
            Try
                _val = value
                If value Then
                    ImgON.Visibility = Windows.Visibility.Visible
                    ImgOFF.Visibility = Windows.Visibility.Hidden
                Else
                    ImgON.Visibility = Windows.Visibility.Hidden
                    ImgOFF.Visibility = Windows.Visibility.Visible
                End If
            Catch ex As Exception
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uPrise.Value: " & ex.ToString & vbCrLf & "Value=" & _val, "Erreur", "uPrise.Value")
            End Try
        End Set
    End Property
End Class
