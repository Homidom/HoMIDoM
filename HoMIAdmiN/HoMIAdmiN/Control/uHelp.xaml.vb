Imports System.Data
Imports System.IO
Imports System.Collections.ObjectModel

Partial Public Class uHelp
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New()
        Try

            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Ttitre.Content = My.Application.Info.Title
            TVersion.Content = "Version:" & My.Application.Info.Version.ToString & vbCrLf & My.Application.Info.Copyright
        Catch ex As Exception
            MessageBox.Show("Erreur lors sur la fonction New de uHelp: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub


End Class
