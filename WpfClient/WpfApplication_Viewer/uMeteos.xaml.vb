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
        imgStackPnl.HorizontalAlignment = HorizontalAlignment.Left
        imgStackPnl.VerticalAlignment = VerticalAlignment.Center
        imgStackPnl.Orientation = Orientation.Horizontal

        'Creation  du menu
        Try
            Dim myxml As clsXML = New clsXML("C:\ehome\config\wehome_config.xml")
            Dim list As XmlNodeList = myxml.SelectNodes("/wehome/meteo/element")
            For i As Integer = 0 To list.Count - 1
                NewBtnMnu(list(i).Attributes.Item(0).Value, list(i).Attributes.Item(1).Value)
            Next
        Catch ex As Exception
            MsgBox("Error updating meteo: " & ex.Message)
        End Try

        ScrollViewer1.Content = imgStackPnl
    End Sub

    Private Sub NewBtnMnu(ByVal Nom As String, ByVal Code As String)
        'Creation  du menu
        Dim ctrl As New uMeteo(Nom, Code)
        imgStackPnl.Children.Add(ctrl)
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
