
Public Class keyboard
    Dim FlagBtn As Boolean
    Dim BtnState As Integer = 0 '0= lettre 1=chiffre
    Dim ActualHeight As Double = 0

    Private Sub Reduce()
        If FlagBtn = False Then
            BtnClose.Content = "▒"
            Me.Height = 30
            FlagBtn = True
        Else
            BtnClose.Content = "▼"
            Me.Height = ActualHeight
            FlagBtn = False
        End If
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        Reduce()
    End Sub

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        ActualHeight = Me.Height
        Reduce()
    End Sub
End Class
