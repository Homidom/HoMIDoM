Public Class FrmConfigChaine

    Private Sub BtnOk_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOk.Click
        For i As Integer = 0 To MyChaine.Count - 1
            Dim chk As New CheckBox
            chk = MyListBox.Items.Item(i)
            If chk.IsChecked = True Then
                EnableChaine(MyChaine.Item(i).ID, True)
            Else
                EnableChaine(MyChaine.Item(i).ID, False)
            End If
        Next
        ChargeChaineFromDB()

        DialogResult = True
    End Sub

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        For i As Integer = 0 To MyChaine.Count - 1
            Dim chk As New CheckBox
            chk.IsChecked = MyChaine.Item(i).Enable
            chk.Content = ConvertHtmlToText(MyChaine.Item(i).Nom)
            MyListBox.Items.Add(chk)
        Next
    End Sub

    Private Sub BtnSearch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSearch.Click
        Dim Thread1 As New System.Threading.Thread(AddressOf ChaineFromXMLToDB)
        Thread1.Start()
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        DialogResult = False
    End Sub
End Class
