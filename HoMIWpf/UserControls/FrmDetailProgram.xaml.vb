Public Class FrmDetailProgram

    Private Sub BtnOk_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOk.Click
        DialogResult = True
    End Sub

    Public Sub New(ByVal Text As String)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Lbl.Text = Text
    End Sub
End Class
