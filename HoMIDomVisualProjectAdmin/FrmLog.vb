Imports System.IO

Public Class FrmLog

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        'FRMMere.Obj.LogMaxSize = TxtSize.Text
        Me.Close()
    End Sub

    Private Sub RefreshLog()
        TxtLog.LoadFile("C:\ehome\log\Servicehome.log", RichTextBoxStreamType.PlainText)
    End Sub

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        RefreshLog()
    End Sub

    Private Sub BtnRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnRefresh.Click
        TxtLog.Text = Nothing
        RefreshLog()
    End Sub

    Private Sub FrmLog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '  TxtSize.Text = FRMMere.Obj.LogMaxSize
    End Sub

    Private Sub TxtSize_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtSize.TextChanged
        If IsNumeric(TxtSize.Text) = False Then
            '  TxtSize.Text = FRMMere.Obj.LogMaxSize
        End If
    End Sub
End Class