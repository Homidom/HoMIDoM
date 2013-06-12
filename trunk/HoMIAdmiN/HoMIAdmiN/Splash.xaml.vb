Public Class Window2

    Public Sub New()
        Try
            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Label2.Content = My.Application.Info.Title
            Label1.Content = "Version:" & My.Application.Info.Version.ToString & vbCrLf & My.Application.Info.Copyright
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub New Splash() : " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
