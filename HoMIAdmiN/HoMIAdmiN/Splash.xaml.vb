Public Class Window2

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Label2.Content = My.Application.Info.Title
        Label1.Content = "Version:" & My.Application.Info.Version.ToString & vbCrLf & My.Application.Info.Copyright
    End Sub
End Class
