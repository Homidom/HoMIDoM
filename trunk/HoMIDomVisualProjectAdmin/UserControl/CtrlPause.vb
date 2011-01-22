Public Class CtrlPause
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

    Public Property Heure() As Integer
        Get
            Return cHeure.Text
        End Get
        Set(ByVal value As Integer)
            cHeure.Text = value
        End Set
    End Property

    Public Property Minute() As Integer
        Get
            Return cMinute.Text
        End Get
        Set(ByVal value As Integer)
            cMinute.Text = value
        End Set
    End Property

    Public Property Seconde() As Integer
        Get
            Return cSeconde.Text
        End Get
        Set(ByVal value As Integer)
            cSeconde.Text = value
        End Set
    End Property

    Public Property MilliSeconde() As Integer
        Get
            Return cMSeconde.Text
        End Get
        Set(ByVal value As Integer)
            cMSeconde.Text = value
        End Set
    End Property


    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        cHeure.Items.Clear()
        cMinute.Items.Clear()
        cSeconde.Items.Clear()

        For i As Integer = 0 To 23
            cHeure.Items.Add(i)
        Next
        For i As Integer = 0 To 59
            cMinute.Items.Add(i)
            cSeconde.Items.Add(i)
        Next
        For i As Integer = 0 To 950 Step 50
            cMSeconde.Items.Add(i)
        Next

        cHeure.SelectedIndex = 0
        cMinute.SelectedIndex = 0
        cSeconde.SelectedIndex = 0
        cMSeconde.SelectedIndex = 0
    End Sub
End Class
