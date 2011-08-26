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
        imgStackPnl.VerticalAlignment = VerticalAlignment.Center
        imgStackPnl.Orientation = Orientation.Horizontal

        If IsHSConnect = True Then
            'Creation  du menu
            Try
                Dim en As Scheduler.clsDeviceEnumeration
                en = hs.GetDeviceEnumerator()
                Dim dv As Scheduler.Classes.DeviceClass
                Dim hcuc As String
                Dim myxml As clsXML = New clsXML("C:\ehome\config\wehome_config.xml")
                Dim list As XmlNodeList = myxml.SelectNodes("/wehome/module/element")
                Dim status As String() = {"0", "1", "On", "Off", "Dim", "Dim", "", "", "", "", "", "", "", "", "", "", "", "Unknown"}

                Do While Not en.Finished
                    dv = en.GetNext
                    hcuc = dv.hc & dv.dc
                    Try
                        For i As Integer = 0 To list.Count - 1
                            If hcuc = (list(i).Attributes.Item(0).Value) Then 'Adresse
                                'Creation  du menu
                                Dim ctrl As uModule = New uModule
                                ctrl.Label = dv.Name
                                ctrl.Adresse = hcuc
                                ctrl.Icon = list(i).Attributes.Item(2).Value
                                ctrl.HasString = list(i).Attributes.Item(3).Value
                                ctrl.HasValue = list(i).Attributes.Item(4).Value
                                ctrl.DeviceString = hs.DeviceString(hcuc)
                                ctrl.DeviceValue = hs.DeviceValue(hcuc)
                                ctrl.TypeDevice = list(i).Attributes.Item(1).Value
                                ctrl.Status = hs.DeviceStatus(hcuc)
                                imgStackPnl.Children.Add(ctrl)
                            End If
                        Next
                    Catch ex As Exception
                        MsgBox("Error chargement des macros: " & ex.ToString)
                    End Try
                Loop
                list = Nothing

                'Creation  du menu
                Try
                    myxml = New clsXML("C:\ehome\config\wehome_config.xml")
                    list = myxml.SelectNodes("/wehome/scene/element")
                    For i As Integer = 0 To list.Count - 1
                        'Creation  du menu
                        Dim ctrl As uModule = New uModule
                        ctrl.Label = list(i).Attributes.Item(0).Value
                        ctrl.Script = list(i).Attributes.Item(1).Value
                        ctrl.Fonction = list(i).Attributes.Item(2).Value
                        ctrl.Command = list(i).Attributes.Item(3).Value
                        ctrl.Icon = list(i).Attributes.Item(4).Value
                        ctrl.TypeDevice = uModule.eTypeDevice.Macro
                        imgStackPnl.Children.Add(ctrl)
                    Next
                Catch ex As Exception
                    MsgBox("Error chargement des macros: " & ex.ToString)
                End Try
            Catch ex As Exception
                MsgBox("Error chargement des macros et devices: " & ex.ToString)
            End Try
        Else
            MessageBox.Show("Impossible d'afficher les modules, vous n'êtes pas connecté à HomeSeer")
        End If
        ScrollViewer1.Content = imgStackPnl

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
