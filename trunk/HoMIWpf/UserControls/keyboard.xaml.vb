Imports System.Windows.Threading

Public Class keyboard
    Dim FlagBtn As Boolean
    Private oskProcess As Process
    Dim dt As DispatcherTimer

    Private Sub Reduce()
        dt.Stop()

        Try
            If oskProcess IsNot Nothing Then
                If Not Me.oskProcess.HasExited Then
                    'CloseMainWindow would generally be preferred but the OSK doesn't respond.
                    Me.oskProcess.Kill()
                End If
                oskProcess.Close()
                oskProcess = Nothing
            ElseIf oskProcess Is Nothing OrElse Me.oskProcess.HasExited Then
                If Me.oskProcess IsNot Nothing AndAlso Me.oskProcess.HasExited Then
                    oskProcess.Close()
                End If
                If String.IsNullOrEmpty(frmMere.KeyboardPath) = False Then oskProcess = Process.Start(frmMere.KeyboardPath)
                'BtnClose.Content = "▼"
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur keyboard.Reduce: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        dt.Start()
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        Reduce()
    End Sub

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()
        BtnClose.Content = "▒"

        dt = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 2)
        dt.Start()

        Me.Height = 30
    End Sub

    Private Sub dispatcherTimer_Tick()
        Try
            Dim flag As Boolean = False

            For Each Process In Diagnostics.Process.GetProcesses()
                'Si un des noms de processus correspond
                If Process.ProcessName = "osk" Then
                    oskProcess = Process
                    flag = True
                End If
            Next
            If flag = False Then oskProcess = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur keyboard.    Private Sub dispatcherTimer_Tick(): " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        dt.Stop()
        dt = Nothing
        MyBase.Finalize()
    End Sub
End Class
