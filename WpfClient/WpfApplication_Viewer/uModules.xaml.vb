Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Threading
Imports System.Diagnostics
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Partial Public Class uModules
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
        imgStackPnl.VerticalAlignment = VerticalAlignment.Top
        imgStackPnl.Orientation = Orientation.Horizontal

        If IsHSConnect = True Then
            'Creation  du menu
            '    Try
            '        Dim en As Scheduler.clsDeviceEnumeration
            '        en = hs.GetDeviceEnumerator()
            '        Dim dv As Scheduler.Classes.DeviceClass
            '        Dim hcuc As String
            '        Dim myxml As clsXML = New clsXML("C:\ehome\config\wehome_config.xml")
            '        Dim list As XmlNodeList = myxml.SelectNodes("/wehome/module/element")
            '        Dim status As String() = {"0", "1", "On", "Off", "Dim", "Dim", "", "", "", "", "", "", "", "", "", "", "", "Unknown"}

            '        Do While Not en.Finished
            '            dv = en.GetNext
            '            hcuc = dv.hc & dv.dc
            '            Try
            '                Dim i As Integer
            '                For i = 0 To list.Count - 1
            '                    If hcuc = (list(i).Attributes.Item(0).Value) Then 'Adresse
            '                        NewBtnMnu(dv.Name, list(i).Attributes.Item(1).Value, list(i).Attributes.Item(2).Value, list(i).Attributes.Item(3).Value, hs.DeviceString(hcuc), list(i).Attributes.Item(4).Value, hs.DeviceValue(hcuc), hs.DeviceStatus(hcuc), hcuc) 'a += dv.Name & " "
            '                    End If
            '                Next
            '            Catch ex As Exception
            '            End Try
            '        Loop
            '    Catch ex As Exception
            '        MsgBox("Error updating devices: " & ex.Message)
            '    End Try
            'Else
            '    MessageBox.Show("Impossible d'afficher les modules, vous n'êtes pas connecté à HomeSeer")
        End If
        ScrollViewer1.Content = imgStackPnl

    End Sub

    Private Sub NewBtnMnu(ByVal Label As String, ByVal TypeDevice As uModule.eTypeDevice, ByVal Icon As String, ByVal HasString As Boolean, ByVal DeviceString As String, ByVal HasValue As Boolean, ByVal DeviceValue As String, ByVal Status As Integer, ByVal Adresse As String)
        'Creation  du menu
        Dim ctrl As uModule = New uModule
        ctrl.Height = 350
        ctrl.Width = 300
        ctrl.Label = Label
        ctrl.Adresse = Adresse
        ctrl.Icon = Icon
        ctrl.HasString = HasString
        ctrl.HasValue = HasValue
        ctrl.DeviceString = DeviceString
        ctrl.DeviceValue = DeviceValue
        ctrl.TypeDevice = TypeDevice
        ctrl.Status = Status
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
