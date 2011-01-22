<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmConfigProperty
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.TxtLong = New System.Windows.Forms.TextBox
        Me.TxtLat = New System.Windows.Forms.TextBox
        Me.BtnCancel = New System.Windows.Forms.Button
        Me.BtnOk = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.HCL = New System.Windows.Forms.TextBox
        Me.HCC = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(7, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(57, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Longitude:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(7, 35)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(48, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Latitude:"
        '
        'TxtLong
        '
        Me.TxtLong.Location = New System.Drawing.Point(67, 11)
        Me.TxtLong.Name = "TxtLong"
        Me.TxtLong.Size = New System.Drawing.Size(172, 20)
        Me.TxtLong.TabIndex = 2
        '
        'TxtLat
        '
        Me.TxtLat.Location = New System.Drawing.Point(67, 35)
        Me.TxtLat.Name = "TxtLat"
        Me.TxtLat.Size = New System.Drawing.Size(172, 20)
        Me.TxtLat.TabIndex = 3
        '
        'BtnCancel
        '
        Me.BtnCancel.Location = New System.Drawing.Point(156, 144)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(91, 23)
        Me.BtnCancel.TabIndex = 4
        Me.BtnCancel.Text = "Annuler"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnOk
        '
        Me.BtnOk.Location = New System.Drawing.Point(59, 144)
        Me.BtnOk.Name = "BtnOk"
        Me.BtnOk.Size = New System.Drawing.Size(91, 23)
        Me.BtnOk.TabIndex = 5
        Me.BtnOk.Text = "Ok"
        Me.BtnOk.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(9, 66)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(148, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Correction Heure Lever Soleil:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(12, 90)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(161, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Correction Heure Coucher Soleil:"
        '
        'HCL
        '
        Me.HCL.Location = New System.Drawing.Point(179, 61)
        Me.HCL.Name = "HCL"
        Me.HCL.Size = New System.Drawing.Size(33, 20)
        Me.HCL.TabIndex = 8
        Me.HCL.Text = "0"
        '
        'HCC
        '
        Me.HCC.Location = New System.Drawing.Point(179, 83)
        Me.HCC.Name = "HCC"
        Me.HCC.Size = New System.Drawing.Size(33, 20)
        Me.HCC.TabIndex = 9
        Me.HCC.Text = "0"
        '
        'FrmConfigProperty
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.ClientSize = New System.Drawing.Size(256, 177)
        Me.Controls.Add(Me.HCC)
        Me.Controls.Add(Me.HCL)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.BtnOk)
        Me.Controls.Add(Me.BtnCancel)
        Me.Controls.Add(Me.TxtLat)
        Me.Controls.Add(Me.TxtLong)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "FrmConfigProperty"
        Me.Text = "Configuration Propriétés"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents BtnCancel As System.Windows.Forms.Button
    Friend WithEvents BtnOk As System.Windows.Forms.Button
    Public WithEvents TxtLong As System.Windows.Forms.TextBox
    Public WithEvents TxtLat As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents HCL As System.Windows.Forms.TextBox
    Friend WithEvents HCC As System.Windows.Forms.TextBox
End Class
