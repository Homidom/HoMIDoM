Public Class CtrlScript

    Public Property Script() As String
        Get
            Return TxtScript.Text
        End Get
        Set(ByVal value As String)
            TxtScript.Text = value
        End Set
    End Property
End Class
