Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports WpfApplication1.Designer.ResizeRotateAdorner

Public Class WDesigner

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim mystyles As New ResourceDictionary()
        mystyles.Source = New Uri("/WpfApplicationTestGUI;component/Resources/DesignerItem.xaml",
                UriKind.RelativeOrAbsolute)
        Dim mybuttonstyle As Style = mystyles("DesignerItemStyle")

        'Ajouter un nouveau Control
        Dim x As New ContentControl
        x.Width = 75
        x.Height = 75
        x.Style = mybuttonstyle
        Dim y As New Button
        y.Content = "Bonjour"
        y.IsHitTestVisible = False 'True:bouge pas False:Bouge
        x.Content = y
        DesignerCanvas.Children.Add(x)
        DesignerCanvas.SetLeft(x, 100)
        DesignerCanvas.SetTop(x, 100)
    End Sub

    Private Sub Chk1_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Chk1.Click
        Dim selectionCheckBox As CheckBox = TryCast(sender, CheckBox)
        If selectionCheckBox IsNot Nothing AndAlso selectionCheckBox.IsChecked = True Then
            For Each child As Control In DesignerCanvas.Children
                Selector.SetIsSelected(child, True)
            Next
        Else
            Dim a As String = ""
            For Each child As Control In DesignerCanvas.Children
                Selector.SetIsSelected(child, False)
                a &= child.Name & " : Left=" & DesignerCanvas.GetLeft(child) & " Top=" & DesignerCanvas.GetTop(child) & " Width=" & child.Width & " Height=" & child.Height & " Angle=" & child.RenderTransform.GetValue(RotateTransform.AngleProperty) & vbCrLf
            Next
            MsgBox(a)
        End If
    End Sub

    Private Sub DesignerCanvas_MouseRightButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles DesignerCanvas.MouseRightButtonDown

    End Sub
End Class
