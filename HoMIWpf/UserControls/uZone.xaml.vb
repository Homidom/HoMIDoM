Imports HoMIDom.HoMIDom
'Imports System.Windows
'Imports System.Windows.Controls
'Imports System.Windows.Controls.Primitives
'Imports WpfApplication1.Designer.ResizeRotateAdorner

Public Class uZone
    Public IDZone As String
    Dim _Zone As Zone
    Dim _Design As Boolean = False

    Private Sub uZone_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    End Sub

    Public Sub New(ByVal IdZone As String, ByVal Height As Double, ByVal Width As Double)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()
        IdZone = IdZone
        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim mystyles As New ResourceDictionary()
        mystyles.Source = New Uri("/HoMIDomWPFClient;component/Resources/DesignerItem.xaml",
                UriKind.RelativeOrAbsolute)
        Dim mybuttonstyle As Style = mystyles("DesignerItemStyle")

        Me.Width = Width
        Me.Height = Height
        ImgBackGround.Width = Width
        ImgBackGround.Height = Height
        DesignerCanvas.Width = Width
        DesignerCanvas.Height = Height

        If IsConnect = True Then
            _Zone = myService.ReturnZoneByID(IdSrv, IdZone)
            For i As Integer = 0 To _Zone.ListElement.Count - 1
                Dim z As Zone.Element_Zone = myService.ReturnZoneByID(IdSrv, IdZone).ListElement.Item(i)
                If z.Visible = True Then
                    Dim w As HoMIDom.HoMIDom.TemplateDevice = myService.ReturnDeviceByID(IdSrv, z.ElementID)
                    If w IsNot Nothing Then

                        'Ajouter un nouveau Control
                        Dim x As New ContentControl
                        x.Width = 75
                        x.Height = 30
                        x.Style = mybuttonstyle
                        Dim y As New Label
                        y.Content = w.Name
                        y.IsHitTestVisible = False 'True:bouge pas False:Bouge
                        x.Content = y
                        DesignerCanvas.Children.Add(x)
                        DesignerCanvas.SetLeft(x, (100 * i))
                        DesignerCanvas.SetTop(x, 100)

                    End If
                End If
            Next
        End If

    End Sub

    Private Sub DesignerCanvas_MouseRightButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles DesignerCanvas.MouseRightButtonDown
        'If _Design = False Then
        '    _Design = True
        'Else
        '    _Design = False
        'End If

        'If _Design = True Then
        '    For Each child As Control In DesignerCanvas.Children
        '        Selector.SetIsSelected(child, True)
        '    Next
        'Else
        '    Dim a As String = ""
        '    For Each child As Control In DesignerCanvas.Children
        '        Selector.SetIsSelected(child, False)
        '        a &= child.Name & " : Left=" & DesignerCanvas.GetLeft(child) & " Top=" & DesignerCanvas.GetTop(child) & " Width=" & child.Width & " Height=" & child.Height & " Angle=" & child.RenderTransform.GetValue(RotateTransform.AngleProperty) & vbCrLf
        '    Next
        '    MsgBox(a)
        'End If
    End Sub
End Class
