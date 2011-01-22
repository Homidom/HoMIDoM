<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CtrlMail
    Inherits System.Windows.Forms.UserControl

    'UserControl remplace la méthode Dispose pour nettoyer la liste des composants.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CtrlMail))
        Me.Label1 = New System.Windows.Forms.Label
        Me.TxtSMTP = New System.Windows.Forms.TextBox
        Me.ChkIdent = New System.Windows.Forms.CheckBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.TxtLogin = New System.Windows.Forms.TextBox
        Me.TxtPassword = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.TxtDe = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.TxtA = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.TxtSujet = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.TxtMessage = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.BtnUp = New System.Windows.Forms.Button
        Me.BtnDown = New System.Windows.Forms.Button
        Me.BtnExpand = New System.Windows.Forms.Label
        Me.BtnDelete = New System.Windows.Forms.PictureBox
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(5, 60)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Serveur SMTP:"
        '
        'TxtSMTP
        '
        Me.TxtSMTP.Location = New System.Drawing.Point(101, 57)
        Me.TxtSMTP.Name = "TxtSMTP"
        Me.TxtSMTP.Size = New System.Drawing.Size(219, 20)
        Me.TxtSMTP.TabIndex = 8
        '
        'ChkIdent
        '
        Me.ChkIdent.AutoSize = True
        Me.ChkIdent.ForeColor = System.Drawing.Color.White
        Me.ChkIdent.Location = New System.Drawing.Point(8, 83)
        Me.ChkIdent.Name = "ChkIdent"
        Me.ChkIdent.Size = New System.Drawing.Size(136, 17)
        Me.ChkIdent.TabIndex = 9
        Me.ChkIdent.Text = "Authentification requise"
        Me.ChkIdent.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(386, 85)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(36, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "Login:"
        '
        'TxtLogin
        '
        Me.TxtLogin.Location = New System.Drawing.Point(428, 83)
        Me.TxtLogin.Name = "TxtLogin"
        Me.TxtLogin.Size = New System.Drawing.Size(168, 20)
        Me.TxtLogin.TabIndex = 11
        '
        'TxtPassword
        '
        Me.TxtPassword.Location = New System.Drawing.Point(211, 83)
        Me.TxtPassword.Name = "TxtPassword"
        Me.TxtPassword.Size = New System.Drawing.Size(168, 20)
        Me.TxtPassword.TabIndex = 13
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(148, 85)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(56, 13)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "Password:"
        '
        'TxtDe
        '
        Me.TxtDe.Location = New System.Drawing.Point(63, 109)
        Me.TxtDe.Name = "TxtDe"
        Me.TxtDe.Size = New System.Drawing.Size(370, 20)
        Me.TxtDe.TabIndex = 15
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(5, 111)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(24, 13)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "De:"
        '
        'TxtA
        '
        Me.TxtA.Location = New System.Drawing.Point(63, 137)
        Me.TxtA.Name = "TxtA"
        Me.TxtA.Size = New System.Drawing.Size(370, 20)
        Me.TxtA.TabIndex = 17
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(5, 139)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(17, 13)
        Me.Label5.TabIndex = 16
        Me.Label5.Text = "A:"
        '
        'TxtSujet
        '
        Me.TxtSujet.Location = New System.Drawing.Point(63, 166)
        Me.TxtSujet.Name = "TxtSujet"
        Me.TxtSujet.Size = New System.Drawing.Size(370, 20)
        Me.TxtSujet.TabIndex = 19
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Location = New System.Drawing.Point(5, 168)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(34, 13)
        Me.Label6.TabIndex = 18
        Me.Label6.Text = "Sujet:"
        '
        'TxtMessage
        '
        Me.TxtMessage.Location = New System.Drawing.Point(63, 194)
        Me.TxtMessage.Multiline = True
        Me.TxtMessage.Name = "TxtMessage"
        Me.TxtMessage.Size = New System.Drawing.Size(370, 163)
        Me.TxtMessage.TabIndex = 21
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.ForeColor = System.Drawing.Color.White
        Me.Label7.Location = New System.Drawing.Point(5, 196)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(53, 13)
        Me.Label7.TabIndex = 20
        Me.Label7.Text = "Message:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.ForeColor = System.Drawing.Color.White
        Me.Label8.Location = New System.Drawing.Point(60, 17)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(26, 13)
        Me.Label8.TabIndex = 23
        Me.Label8.Text = "Mail"
        '
        'BtnUp
        '
        Me.BtnUp.Image = CType(resources.GetObject("BtnUp.Image"), System.Drawing.Image)
        Me.BtnUp.Location = New System.Drawing.Point(534, 9)
        Me.BtnUp.Name = "BtnUp"
        Me.BtnUp.Size = New System.Drawing.Size(25, 25)
        Me.BtnUp.TabIndex = 26
        Me.BtnUp.UseVisualStyleBackColor = True
        '
        'BtnDown
        '
        Me.BtnDown.Image = CType(resources.GetObject("BtnDown.Image"), System.Drawing.Image)
        Me.BtnDown.Location = New System.Drawing.Point(561, 9)
        Me.BtnDown.Name = "BtnDown"
        Me.BtnDown.Size = New System.Drawing.Size(25, 25)
        Me.BtnDown.TabIndex = 25
        Me.BtnDown.UseVisualStyleBackColor = True
        '
        'BtnExpand
        '
        Me.BtnExpand.AutoSize = True
        Me.BtnExpand.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnExpand.ForeColor = System.Drawing.Color.White
        Me.BtnExpand.Location = New System.Drawing.Point(8, 9)
        Me.BtnExpand.Name = "BtnExpand"
        Me.BtnExpand.Size = New System.Drawing.Size(21, 24)
        Me.BtnExpand.TabIndex = 27
        Me.BtnExpand.Text = "+"
        '
        'BtnDelete
        '
        Me.BtnDelete.Image = CType(resources.GetObject("BtnDelete.Image"), System.Drawing.Image)
        Me.BtnDelete.Location = New System.Drawing.Point(588, 9)
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.Size = New System.Drawing.Size(25, 25)
        Me.BtnDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnDelete.TabIndex = 28
        Me.BtnDelete.TabStop = False
        '
        'CtrlMail
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.BtnExpand)
        Me.Controls.Add(Me.BtnUp)
        Me.Controls.Add(Me.BtnDown)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.TxtMessage)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.TxtSujet)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.TxtA)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.TxtDe)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TxtPassword)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxtLogin)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ChkIdent)
        Me.Controls.Add(Me.TxtSMTP)
        Me.Controls.Add(Me.Label1)
        Me.Name = "CtrlMail"
        Me.Size = New System.Drawing.Size(625, 42)
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TxtSMTP As System.Windows.Forms.TextBox
    Friend WithEvents ChkIdent As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TxtLogin As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TxtDe As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TxtA As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TxtSujet As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TxtMessage As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Private WithEvents TxtPassword As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents BtnUp As System.Windows.Forms.Button
    Friend WithEvents BtnDown As System.Windows.Forms.Button
    Friend WithEvents BtnExpand As System.Windows.Forms.Label
    Friend WithEvents BtnDelete As System.Windows.Forms.PictureBox

End Class
