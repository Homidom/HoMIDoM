﻿Imports System.Reflection
Imports System.Windows.Threading
Imports System.Threading

Partial Public Class uInternet
    Dim _Url As String 'Url internet de la page à afficher 

    Private Sub uInternet_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            If UrlIsValid(_Url) = False Then Exit Sub
            If _Url.ToLower().StartsWith("www.") Then _Url = "http://" & _Url
            If My.Computer.Network.IsAvailable = True And String.IsNullOrEmpty(_Url) = False Then
                x.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, DirectCast(Sub() x.Navigate(New Uri(_Url)), ThreadStart))
            End If
            x.Height = Me.Height - 30
        Catch ex As Exception
            MessageBox.Show("Erreur uInternet.uInternet_Loaded: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Sub New(ByVal Url As String)
        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _Url = Url
    End Sub

    Private Sub Image1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image1.MouseDown
        x.GoBack()
    End Sub

    Private Sub Image2_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image2.MouseDown
        x.GoForward()
    End Sub

    Sub SuppressScriptErrors(ByVal wb As Controls.WebBrowser, ByVal Hide As Boolean)
        Try
            Dim fiComWebBrowser As FieldInfo = GetType(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance Or BindingFlags.NonPublic)
            If fiComWebBrowser Is Nothing Then
                Return
            End If
            Dim objComWebBrowser As Object = fiComWebBrowser.GetValue(wb)
            If objComWebBrowser Is Nothing Then
                Return
            End If
            objComWebBrowser.[GetType]().InvokeMember("Silent", BindingFlags.SetProperty, Nothing, objComWebBrowser, New Object() {Hide})
        Catch ex As Exception
            MessageBox.Show("Erreur uInternet.SuppressScriptErrors: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub x_Navigated(ByVal sender As Object, ByVal e As System.Windows.Navigation.NavigationEventArgs) Handles x.Navigated
        SuppressScriptErrors(x, True)
    End Sub

    Private Sub x_Navigating(ByVal sender As Object, ByVal e As System.Windows.Navigation.NavigatingCancelEventArgs) Handles x.Navigating
        SuppressScriptErrors(x, True)
    End Sub
End Class
