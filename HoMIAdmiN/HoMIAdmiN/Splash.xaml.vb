Public Class Window2

    Public Sub New()
        Try
            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Label2.Content = My.Application.Info.Title
            Label1.Content = "Version:" & My.Application.Info.Version.ToString & vbCrLf & My.Application.Info.Copyright
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub New Splash() : " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
