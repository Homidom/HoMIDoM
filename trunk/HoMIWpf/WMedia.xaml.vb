Public Class WMedia

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        UMedia1.ShowVideo = False
        UMedia1.Filter.Add("Audio (*.mp3, *.wma)|*.mp3;*.wma")
    End Sub
End Class
