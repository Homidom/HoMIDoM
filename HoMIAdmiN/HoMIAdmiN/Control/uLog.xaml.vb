Partial Public Class uLog
    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRefresh.Click
        TxtLog.Text = Nothing
        RefreshLog()
    End Sub

    Private Sub RefreshLog()
        If Window1.IsConnect = True Then TxtLog.Text = Window1.myService.ReturnLog()
    End Sub


    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        RefreshLog()

        If Window1.IsConnect = True Then
            TxtFile.Text = Window1.myService.GetMaxFileSizeLog
            TxtMaxLogMonth.Text = Window1.myService.GetMaxMonthLog

            Dim _list As List(Of Boolean) = Window1.myService.GetTypeLogEnable
            ChkTyp0.IsChecked = _list.Item(0)
            ChkTyp1.IsChecked = _list.Item(1)
            ChkTyp2.IsChecked = _list.Item(2)
            ChkTyp3.IsChecked = _list.Item(3)
            ChkTyp4.IsChecked = _list.Item(4)
            ChkTyp5.IsChecked = _list.Item(5)
            ChkTyp6.IsChecked = _list.Item(6)
            ChkTyp7.IsChecked = _list.Item(7)
            ChkTyp8.IsChecked = _list.Item(8)
            ChkTyp9.IsChecked = _list.Item(9)
        End If
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        If IsNumeric(TxtFile.Text) = False Or CDbl(TxtFile.Text) < 1 Then
            MessageBox.Show("Veuillez saisir un numérique et positif pour la taille max du fichier de log!", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        If Window1.IsConnect = True Then
            Window1.myService.SetMaxFileSizeLog(CDbl(TxtFile.Text))
            Window1.myService.SetMaxMonthLog(CDbl(TxtMaxLogMonth.Text))

            Dim _list As New List(Of Boolean)
            If ChkTyp0.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If
            If ChkTyp1.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If
            If ChkTyp2.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If
            If ChkTyp3.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If
            If ChkTyp4.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If
            If ChkTyp5.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If
            If ChkTyp6.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If
            If ChkTyp7.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If
            If ChkTyp8.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If
            If ChkTyp9.IsChecked Then
                _list.Add(True)
            Else
                _list.Add(False)
            End If

            Window1.myService.SetTypeLogEnable(_list)
        End If
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub TxtMaxLogMonth_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMaxLogMonth.TextChanged
        If IsNumeric(TxtMaxLogMonth.Text) = False Then
            MessageBox.Show("Veuillez saisir un chiffre comme durée de mois maximum", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            TxtMaxLogMonth.Text = Window1.myService.GetMaxMonthLog
        End If
    End Sub

    Private Sub TxtFile_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtFile.TextChanged
        If IsNumeric(TxtFile.Text) = False Then
            MessageBox.Show("Veuillez saisir un chiffre comme taille maximale", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            TxtFile.Text = Window1.myService.GetMaxFileSizeLog
        End If
    End Sub
End Class
