Partial Public Class uCalc
    Dim Result As Double
    Dim Mem1 As Double
    Dim a As String
    Dim op As Integer '1=- 2=+ 3=* 4=/
    Dim Flag As Boolean

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim bmpImagebk As New BitmapImage()
        bmpImagebk.BeginInit()
        bmpImagebk.UriSource = New Uri("C:\ehome\images\stocks_undocked_1_1_4.png", UriKind.Absolute)
        bmpImagebk.EndInit()
        Imagebk.Source = bmpImagebk

        Dim bmpImage As New BitmapImage()
        bmpImage.BeginInit()
        bmpImage.UriSource = New Uri("C:\ehome\images\k0.png", UriKind.Absolute)
        bmpImage.EndInit()
        Btn0.Source = bmpImage
        Dim bmpImage1 As New BitmapImage()
        bmpImage1.BeginInit()
        bmpImage1.UriSource = New Uri("C:\ehome\images\k1.png", UriKind.Absolute)
        bmpImage1.EndInit()
        Btn1.Source = bmpImage1
        Dim bmpImage2 As New BitmapImage()
        bmpImage2.BeginInit()
        bmpImage2.UriSource = New Uri("C:\ehome\images\k2.png", UriKind.Absolute)
        bmpImage2.EndInit()
        Btn2.Source = bmpImage2
        Dim bmpImage3 As New BitmapImage()
        bmpImage3.BeginInit()
        bmpImage3.UriSource = New Uri("C:\ehome\images\k3.png", UriKind.Absolute)
        bmpImage3.EndInit()
        Btn3.Source = bmpImage3
        Dim bmpImage4 As New BitmapImage()
        bmpImage4.BeginInit()
        bmpImage4.UriSource = New Uri("C:\ehome\images\k4.png", UriKind.Absolute)
        bmpImage4.EndInit()
        Btn4.Source = bmpImage4
        Dim bmpImage5 As New BitmapImage()
        bmpImage5.BeginInit()
        bmpImage5.UriSource = New Uri("C:\ehome\images\k5.png", UriKind.Absolute)
        bmpImage5.EndInit()
        Btn5.Source = bmpImage5
        Dim bmpImage6 As New BitmapImage()
        bmpImage6.BeginInit()
        bmpImage6.UriSource = New Uri("C:\ehome\images\k6.png", UriKind.Absolute)
        bmpImage6.EndInit()
        Btn6.Source = bmpImage6
        Dim bmpImage7 As New BitmapImage()
        bmpImage7.BeginInit()
        bmpImage7.UriSource = New Uri("C:\ehome\images\k7.png", UriKind.Absolute)
        bmpImage7.EndInit()
        Btn7.Source = bmpImage7
        Dim bmpImage8 As New BitmapImage()
        bmpImage8.BeginInit()
        bmpImage8.UriSource = New Uri("C:\ehome\images\k8.png", UriKind.Absolute)
        bmpImage8.EndInit()
        Btn8.Source = bmpImage8
        Dim bmpImage9 As New BitmapImage()
        bmpImage9.BeginInit()
        bmpImage9.UriSource = New Uri("C:\ehome\images\k9.png", UriKind.Absolute)
        bmpImage9.EndInit()
        Btn9.Source = bmpImage9
        Dim bmpImage10 As New BitmapImage()
        bmpImage10.BeginInit()
        bmpImage10.UriSource = New Uri("C:\ehome\images\kega.png", UriKind.Absolute)
        bmpImage10.EndInit()
        BtnEgal.Source = bmpImage10
        Dim bmpImage11 As New BitmapImage()
        bmpImage11.BeginInit()
        bmpImage11.UriSource = New Uri("C:\ehome\images\kvir.png", UriKind.Absolute)
        bmpImage11.EndInit()
        BtnVirg.Source = bmpImage11
        Dim bmpImage12 As New BitmapImage()
        bmpImage12.BeginInit()
        bmpImage12.UriSource = New Uri("C:\ehome\images\kc.png", UriKind.Absolute)
        bmpImage12.EndInit()
        BtnClear.Source = bmpImage12
        Dim bmpImage13 As New BitmapImage()
        bmpImage13.BeginInit()
        bmpImage13.UriSource = New Uri("C:\ehome\images\kdiv.png", UriKind.Absolute)
        bmpImage13.EndInit()
        BtnDiv.Source = bmpImage13
        Dim bmpImage14 As New BitmapImage()
        bmpImage14.BeginInit()
        bmpImage14.UriSource = New Uri("C:\ehome\images\kmul.png", UriKind.Absolute)
        bmpImage14.EndInit()
        BtnMul.Source = bmpImage14
        Dim bmpImage15 As New BitmapImage()
        bmpImage15.BeginInit()
        bmpImage15.UriSource = New Uri("C:\ehome\images\kplus.png", UriKind.Absolute)
        bmpImage15.EndInit()
        BtnPlus.Source = bmpImage15
        Dim bmpImage16 As New BitmapImage()
        bmpImage16.BeginInit()
        bmpImage16.UriSource = New Uri("C:\ehome\images\kmoi.png", UriKind.Absolute)
        bmpImage16.EndInit()
        BtnMoins.Source = bmpImage16


    End Sub

    Private Sub Num_Click(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Btn0.MouseDown, BtnVirg.MouseDown, Btn1.MouseDown, Btn2.MouseDown, Btn3.MouseDown, Btn6.MouseDown, Btn5.MouseDown, Btn4.MouseDown, Btn7.MouseDown, Btn8.MouseDown, Btn9.MouseDown
        If Lbl.Content = "0" Then Lbl.Content = ""
        If sender.tag = "," And InStr(Lbl.Content, ",") > 1 Then Exit Sub
        If Flag = True Then
            Lbl.Content = sender.tag
            Flag = False
        Else
            Lbl.Content &= sender.tag
        End If

    End Sub

    Private Sub BtnClear_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnClear.MouseDown
        Lbl.Content = "0"
        Result = 0
        Mem1 = 0
    End Sub

    Private Sub BtnMoins_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnMoins.MouseDown
        Traite()
        op = 1
        Mem1 = CDbl(Lbl.Content)
        Flag = True
    End Sub

    Private Sub BtnPlus_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPlus.MouseDown
        Traite()
        op = 2
        Mem1 = CDbl(Lbl.Content)
        Flag = True
    End Sub

    Private Sub BtnMul_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnMul.MouseDown
        Traite()
        op = 3
        Mem1 = CDbl(Lbl.Content)
        Flag = True
    End Sub

    Private Sub BtnDiv_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnDiv.MouseDown
        Traite()
        op = 4
        Mem1 = CDbl(Lbl.Content)
        Flag = True
    End Sub

    Private Sub Traite()
        If Mem1 <> "0" And op <> 0 Then
            Select Case op
                Case 1
                    Result = Mem1 - CDbl(Lbl.Content)
                Case 2
                    Result = Mem1 + CDbl(Lbl.Content)
                Case 3
                    Result = Mem1 * CDbl(Lbl.Content)
                Case 4
                    Result = Mem1 / CDbl(Lbl.Content)
            End Select
            Lbl.Content = Result
        End If
    End Sub

    Private Sub BtnEgal_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnEgal.MouseDown
        Select Case op
            Case 1
                Result = Mem1 - CDbl(Lbl.Content)
            Case 2
                Result = Mem1 + CDbl(Lbl.Content)
            Case 3
                Result = Mem1 * CDbl(Lbl.Content)
            Case 4
                Result = Mem1 / CDbl(Lbl.Content)
        End Select
        Lbl.Content = Result
        Mem1 = 0
    End Sub
End Class
