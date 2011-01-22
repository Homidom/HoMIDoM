Public Class CtrlMail
    Dim _Expand As Boolean = False
    Dim _index As Integer

    Public Event Down(ByVal sender As Object)
    Public Event Up(ByVal sender As Object)
    Public Event Delete(ByVal sender As Object)

    Public Property Index() As Integer
        Get
            Return _index
        End Get
        Set(ByVal value As Integer)
            _index = value
        End Set
    End Property

    Private Sub BtnUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnUp.Click
        RaiseEvent Up(Me)
    End Sub

    Private Sub BtnDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDown.Click
        RaiseEvent Down(Me)
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelete.Click
        RaiseEvent Delete(Me)
    End Sub

    Public Property ServeurSMTP() As String
        Get
            Return TxtSMTP.Text
        End Get
        Set(ByVal value As String)
            TxtSMTP.Text = value
        End Set
    End Property

    Public Property CheckIdentification() As Boolean
        Get
            Return ChkIdent.Checked
        End Get
        Set(ByVal value As Boolean)
            ChkIdent.Checked = value
        End Set
    End Property

    Public Property Login() As String
        Get
            Return TxtLogin.Text
        End Get
        Set(ByVal value As String)
            TxtLogin.Text = value
        End Set
    End Property

    Public Property Password() As String
        Get
            Return TxtPassword.Text
        End Get
        Set(ByVal value As String)
            TxtPassword.Text = value
        End Set
    End Property

    Public Property De() As String
        Get
            Return TxtDe.Text
        End Get
        Set(ByVal value As String)
            TxtDe.Text = value
        End Set
    End Property

    Public Property A() As String
        Get
            Return TxtA.Text
        End Get
        Set(ByVal value As String)
            TxtA.Text = value
        End Set
    End Property

    Public Property Sujet() As String
        Get
            Return TxtSujet.Text
        End Get
        Set(ByVal value As String)
            TxtSujet.Text = value
        End Set
    End Property

    Public Property Message() As String
        Get
            Return TxtMessage.Text
        End Get
        Set(ByVal value As String)
            TxtMessage.Text = value
        End Set
    End Property

    Private Sub BtnExpand_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExpand.Click
        If _Expand = False Then
            Me.Height = 363
            _Expand = True
            BtnExpand.Text = "-"
        Else
            Me.Height = 50
            _Expand = False
            BtnExpand.Text = "+"
        End If
    End Sub
End Class
