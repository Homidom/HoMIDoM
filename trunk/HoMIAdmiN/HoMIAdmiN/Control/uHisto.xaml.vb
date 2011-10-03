Imports ZedGraph

Public Class uHisto
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim x As New ZedGraphControl
        x.ScrollGrace = 0
        x.ScrollMaxX = 0
        x.ScrollMaxY = 0
        x.ScrollMaxY2 = 0
        x.ScrollMinX = 0
        x.ScrollMinY = 0
        x.ScrollMinY2 = 0
        x.Name = "Graph"
        x.Width = 300
        x.Height = 300
        x.Size = New System.Drawing.Size(506, 429)

        Dim y As New GraphPane
        y.Title.Text = "Test"

        x.GraphPane = y
        'x.GraphPane.Title.Text = "Test"
        host.Child = x


    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub
End Class
