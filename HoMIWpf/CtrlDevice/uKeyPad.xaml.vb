Public Class uKeyPad

    Dim _Value As Integer = Nothing
    Dim _ShowPassWord As Boolean = True
    Dim _ClearAfterEnter As Boolean = False
    Dim _ShowClavier As Boolean = True

    Public Event KeyPadOk(ByVal Value As Integer)

    Public Property ShowPassWord As Boolean
        Get
            Return _ShowPassWord
        End Get
        Set(ByVal value As Boolean)
            _ShowPassWord = value
            If value Then
                TxtSaisie.Visibility = Windows.Visibility.Visible
                TxtSaisiePwd.Visibility = Windows.Visibility.Collapsed
            Else
                TxtSaisie.Visibility = Windows.Visibility.Collapsed
                TxtSaisiePwd.Visibility = Windows.Visibility.Visible
            End If
        End Set
    End Property

    Public Property ClearAfterEnter As Boolean
        Get
            Return _ClearAfterEnter
        End Get
        Set(ByVal value As Boolean)
            _ClearAfterEnter = value
        End Set
    End Property

    Public Property ShowClavier As Boolean
        Get
            Return _ShowClavier
        End Get
        Set(ByVal value As Boolean)
            _ShowClavier = value
            If value Then
                StkClavier.Visibility = Windows.Visibility.Visible
            Else
                StkClavier.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Private Sub Btn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Btn7.Click, Btn8.Click, Btn9.Click, Btn6.Click, Btn5.Click, Btn4.Click, Btn1.Click, Btn2.Click, Btn3.Click, Btn0.Click
        Dim x As Button = sender
        Dim rt As String = ""

        If _ShowPassWord Then
            TxtSaisie.Text &= x.Content
            rt = TxtSaisie.Text
        Else
            TxtSaisiePwd.Password &= x.Content
            rt = TxtSaisiePwd.Password
        End If
        _Value = CInt(rt)
    End Sub

    Private Sub BtnC_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnC.Click
        TxtSaisie.Text = ""
        TxtSaisiePwd.Password = ""
    End Sub

    Private Sub BtnOK_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            Dim rt As String = ""

            If _ShowPassWord Then
                rt = TxtSaisie.Text
            Else
                rt = TxtSaisiePwd.Password
            End If
            _Value = CInt(rt)

            RaiseEvent KeyPadOk(_Value)

            If _ClearAfterEnter Then
                TxtSaisie.Text = ""
                TxtSaisiePwd.Password = ""
            End If

        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Keypad.BtnOK_Click: " & ex.ToString, "Erreur", "Keypad.BtnOK_Click")
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub


   
    Private Sub TxtEnter_PreviewKeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TxtSaisie.PreviewKeyUp, TxtSaisiePwd.PreviewKeyUp
        Try
            If e.Key = Input.Key.Enter Then
                Dim rt As String = ""

                If _ShowPassWord Then
                    rt = TxtSaisie.Text
                Else
                    rt = TxtSaisiePwd.Password
                End If
                _Value = CInt(rt)

                RaiseEvent KeyPadOk(_Value)

                If _ClearAfterEnter Then
                    TxtSaisie.Text = ""
                    TxtSaisiePwd.Password = ""
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Keypad.TxtEnter_PreviewKeyUp: " & ex.ToString, "Erreur", "Keypad.TxtEnter_PreviewKeyUp")
        End Try
    End Sub

End Class
