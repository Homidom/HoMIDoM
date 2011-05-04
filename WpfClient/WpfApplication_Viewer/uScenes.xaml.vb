Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Threading
Imports System.Diagnostics
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Partial Public Class uScenes
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
        ' Create StackPanel and set child elements to horizontal orientation
        imgStackPnl.HorizontalAlignment = HorizontalAlignment.Left
        imgStackPnl.VerticalAlignment = VerticalAlignment.Top
        imgStackPnl.Orientation = Orientation.Horizontal

        'Creation  du menu
        Try
            Dim myxml As clsXML = New clsXML("C:\ehome\config\wehome_config.xml")
            Dim list As XmlNodeList = myxml.SelectNodes("/wehome/scene/element")
            Dim i As Integer
            For i = 0 To list.Count - 1
                NewBtnMnu(list(i).Attributes.Item(0).Value, list(i).Attributes.Item(4).Value, list(i).Attributes.Item(1).Value, list(i).Attributes.Item(2).Value, list(i).Attributes.Item(3).Value)
            Next
        Catch ex As Exception
            MsgBox("Error updating devices: " & ex.Message)
        End Try

        ScrollViewer1.Content = imgStackPnl

    End Sub

    Private Sub NewBtnMnu(ByVal Label As String, ByVal Icon As String, ByVal Script As String, ByVal Fonction As String, ByVal Command As String)
        'Creation  du menu
        Dim ctrl As uScene = New uScene
        ctrl.Height = 350
        ctrl.Width = 300
        ctrl.Label = Label
        ctrl.Script = Script
        ctrl.Fonction = Fonction
        ctrl.Command = Command
        ctrl.Icon = Icon
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
            ScrollToPosition(ScrollViewer1, scrollTarget.X, currentPoint.Y, 600)
        End If
    End Sub
End Class
