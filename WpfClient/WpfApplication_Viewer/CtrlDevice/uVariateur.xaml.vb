Public Class uVariateur
    Dim _Value As Integer

    Public Event ValueChange(ByVal Value As Integer)

    Public Property Value As Integer
        Get
            Return _Value
        End Get
        Set(ByVal value2 As Integer)
            _Value = value2
        End Set
    End Property


    Private Sub BtnUp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnUp.Click
        _Value += 10
        If _Value > 100 Then _Value = 100
        ProgressBar1.Value = _Value
        RaiseEvent ValueChange(_Value)
    End Sub

    Private Sub BtnDown_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDown.Click
        _Value -= 10
        If _Value < 0 Then _Value = 0
        ProgressBar1.Value = _Value
        RaiseEvent ValueChange(_Value)
    End Sub
End Class
