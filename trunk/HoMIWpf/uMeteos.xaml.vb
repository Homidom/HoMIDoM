Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Partial Public Class uMeteos
#Region "Data"
    ' Used when manually scrolling.
    Private scrollTarget As Point
    Private scrollStartPoint As Point
    Private scrollStartOffset As Point
    Private previousPoint As Point
    Private imgStackPnl As New StackPanel()
#End Region

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        With imgStackPnl
            .HorizontalAlignment = HorizontalAlignment.Left
            .VerticalAlignment = VerticalAlignment.Center
            .Orientation = Orientation.Horizontal
        End With

        'Creation  du menu
        Try
            If IsConnect Then
                For Each ObjMeteo As HoMIDom.HoMIDom.TemplateDevice In myService.GetAllDevices(IdSrv)
                    If ObjMeteo.Type = HoMIDom.HoMIDom.Device.ListeDevices.METEO And ObjMeteo.Enable = True Then
                        NewBtnMnu(ObjMeteo.ID)
                    End If
                Next
            End If
        Catch ex As Exception
            MessageBox.Show("Error updating meteo: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        ScrollViewer1.Content = imgStackPnl
    End Sub

    Private Sub NewBtnMnu(ByVal id As String)
        Try
            'Creation  du menu
            Dim ctrl As New uMeteo(id)
            imgStackPnl.Children.Add(ctrl)

        Catch ex As Exception
            MessageBox.Show("Erreur NewBtnMnu: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub ScrollViewer1_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ScrollViewer1.PreviewMouseDown
        scrollStartPoint = e.GetPosition(Me)
        scrollStartOffset.X = ScrollViewer1.HorizontalOffset
    End Sub

    Private Sub ScrollViewer1_PreviewMouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ScrollViewer1.PreviewMouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim currentPoint As Point = e.GetPosition(Me)
            Dim delta As New Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y)
            scrollTarget.X = scrollStartOffset.X + delta.X
            ScrollToPosition(ScrollViewer1, scrollTarget.X, currentPoint.Y, m_SpeedTouch)
        End If
    End Sub
End Class
