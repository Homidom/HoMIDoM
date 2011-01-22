<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmDevice
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
        Me.LDriver = New System.Windows.Forms.ComboBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.TxtAdress = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.TxtImg = New System.Windows.Forms.TextBox
        Me.cEnable = New System.Windows.Forms.CheckBox
        Me.BtnAnnuler = New System.Windows.Forms.Button
        Me.BtnOk = New System.Windows.Forms.Button
        Me.Label5 = New System.Windows.Forms.Label
        Me.TxtName = New System.Windows.Forms.TextBox
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.Grp14 = New System.Windows.Forms.GroupBox
        Me.BtnLearn = New System.Windows.Forms.Button
        Me.BtnTstCmd = New System.Windows.Forms.Button
        Me.BtnDelCmd = New System.Windows.Forms.Button
        Me.BtnSaveCmd = New System.Windows.Forms.Button
        Me.BtnNewCmd = New System.Windows.Forms.Button
        Me.TxtCmdData = New System.Windows.Forms.TextBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.TxtCmdRepeat = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.TxtCmdName = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.ListCmd = New System.Windows.Forms.ListBox
        Me.CbTypeClass = New System.Windows.Forms.ComboBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.BtnLoadImg = New System.Windows.Forms.Button
        Me.MyImage = New System.Windows.Forms.PictureBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Grp14.SuspendLayout()
        CType(Me.MyImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(8, 80)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Driver:"
        '
        'LDriver
        '
        Me.LDriver.BackColor = System.Drawing.Color.Silver
        Me.LDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.LDriver.FormattingEnabled = True
        Me.LDriver.Location = New System.Drawing.Point(66, 79)
        Me.LDriver.Name = "LDriver"
        Me.LDriver.Size = New System.Drawing.Size(237, 21)
        Me.LDriver.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(8, 136)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(56, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Adresse:"
        '
        'TxtAdress
        '
        Me.TxtAdress.BackColor = System.Drawing.Color.Silver
        Me.TxtAdress.Location = New System.Drawing.Point(66, 137)
        Me.TxtAdress.Name = "TxtAdress"
        Me.TxtAdress.Size = New System.Drawing.Size(237, 20)
        Me.TxtAdress.TabIndex = 4
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(8, 164)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(45, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Image:"
        '
        'TxtImg
        '
        Me.TxtImg.BackColor = System.Drawing.Color.Silver
        Me.TxtImg.Location = New System.Drawing.Point(66, 165)
        Me.TxtImg.Name = "TxtImg"
        Me.TxtImg.Size = New System.Drawing.Size(237, 20)
        Me.TxtImg.TabIndex = 5
        '
        'cEnable
        '
        Me.cEnable.AutoSize = True
        Me.cEnable.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cEnable.ForeColor = System.Drawing.Color.White
        Me.cEnable.Location = New System.Drawing.Point(326, 53)
        Me.cEnable.Name = "cEnable"
        Me.cEnable.Size = New System.Drawing.Size(65, 17)
        Me.cEnable.TabIndex = 6
        Me.cEnable.Text = "Enable"
        Me.cEnable.UseVisualStyleBackColor = True
        '
        'BtnAnnuler
        '
        Me.BtnAnnuler.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnAnnuler.Location = New System.Drawing.Point(463, 83)
        Me.BtnAnnuler.Name = "BtnAnnuler"
        Me.BtnAnnuler.Size = New System.Drawing.Size(85, 28)
        Me.BtnAnnuler.TabIndex = 8
        Me.BtnAnnuler.Text = "Annuler"
        Me.BtnAnnuler.UseVisualStyleBackColor = True
        '
        'BtnOk
        '
        Me.BtnOk.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnOk.Location = New System.Drawing.Point(463, 51)
        Me.BtnOk.Name = "BtnOk"
        Me.BtnOk.Size = New System.Drawing.Size(85, 28)
        Me.BtnOk.TabIndex = 7
        Me.BtnOk.Text = "Ok"
        Me.BtnOk.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(8, 52)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(36, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Nom:"
        '
        'TxtName
        '
        Me.TxtName.BackColor = System.Drawing.Color.Silver
        Me.TxtName.Location = New System.Drawing.Point(66, 51)
        Me.TxtName.Name = "TxtName"
        Me.TxtName.Size = New System.Drawing.Size(235, 20)
        Me.TxtName.TabIndex = 1
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'Grp14
        '
        Me.Grp14.Controls.Add(Me.BtnLearn)
        Me.Grp14.Controls.Add(Me.BtnTstCmd)
        Me.Grp14.Controls.Add(Me.BtnDelCmd)
        Me.Grp14.Controls.Add(Me.BtnSaveCmd)
        Me.Grp14.Controls.Add(Me.BtnNewCmd)
        Me.Grp14.Controls.Add(Me.TxtCmdData)
        Me.Grp14.Controls.Add(Me.Label9)
        Me.Grp14.Controls.Add(Me.TxtCmdRepeat)
        Me.Grp14.Controls.Add(Me.Label8)
        Me.Grp14.Controls.Add(Me.TxtCmdName)
        Me.Grp14.Controls.Add(Me.Label7)
        Me.Grp14.Controls.Add(Me.ListCmd)
        Me.Grp14.Location = New System.Drawing.Point(5, 197)
        Me.Grp14.Name = "Grp14"
        Me.Grp14.Size = New System.Drawing.Size(550, 195)
        Me.Grp14.TabIndex = 16
        Me.Grp14.TabStop = False
        Me.Grp14.Text = "Commandes"
        Me.Grp14.Visible = False
        '
        'BtnLearn
        '
        Me.BtnLearn.Location = New System.Drawing.Point(448, 139)
        Me.BtnLearn.Name = "BtnLearn"
        Me.BtnLearn.Size = New System.Drawing.Size(69, 23)
        Me.BtnLearn.TabIndex = 11
        Me.BtnLearn.Text = "Apprendre"
        Me.BtnLearn.UseVisualStyleBackColor = True
        '
        'BtnTstCmd
        '
        Me.BtnTstCmd.Location = New System.Drawing.Point(373, 139)
        Me.BtnTstCmd.Name = "BtnTstCmd"
        Me.BtnTstCmd.Size = New System.Drawing.Size(69, 23)
        Me.BtnTstCmd.TabIndex = 10
        Me.BtnTstCmd.Text = "Tester"
        Me.BtnTstCmd.UseVisualStyleBackColor = True
        '
        'BtnDelCmd
        '
        Me.BtnDelCmd.Location = New System.Drawing.Point(303, 139)
        Me.BtnDelCmd.Name = "BtnDelCmd"
        Me.BtnDelCmd.Size = New System.Drawing.Size(64, 23)
        Me.BtnDelCmd.TabIndex = 9
        Me.BtnDelCmd.Text = "Supprimer"
        Me.BtnDelCmd.UseVisualStyleBackColor = True
        '
        'BtnSaveCmd
        '
        Me.BtnSaveCmd.Location = New System.Drawing.Point(229, 139)
        Me.BtnSaveCmd.Name = "BtnSaveCmd"
        Me.BtnSaveCmd.Size = New System.Drawing.Size(68, 23)
        Me.BtnSaveCmd.TabIndex = 8
        Me.BtnSaveCmd.Text = "Enregistrer"
        Me.BtnSaveCmd.UseVisualStyleBackColor = True
        '
        'BtnNewCmd
        '
        Me.BtnNewCmd.Location = New System.Drawing.Point(158, 139)
        Me.BtnNewCmd.Name = "BtnNewCmd"
        Me.BtnNewCmd.Size = New System.Drawing.Size(65, 23)
        Me.BtnNewCmd.TabIndex = 7
        Me.BtnNewCmd.Text = "Nouveau"
        Me.BtnNewCmd.UseVisualStyleBackColor = True
        '
        'TxtCmdData
        '
        Me.TxtCmdData.Location = New System.Drawing.Point(208, 77)
        Me.TxtCmdData.Multiline = True
        Me.TxtCmdData.Name = "TxtCmdData"
        Me.TxtCmdData.Size = New System.Drawing.Size(334, 56)
        Me.TxtCmdData.TabIndex = 6
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.ForeColor = System.Drawing.Color.White
        Me.Label9.Location = New System.Drawing.Point(171, 80)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(33, 13)
        Me.Label9.TabIndex = 5
        Me.Label9.Text = "Data:"
        '
        'TxtCmdRepeat
        '
        Me.TxtCmdRepeat.Location = New System.Drawing.Point(208, 51)
        Me.TxtCmdRepeat.Name = "TxtCmdRepeat"
        Me.TxtCmdRepeat.Size = New System.Drawing.Size(50, 20)
        Me.TxtCmdRepeat.TabIndex = 4
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.ForeColor = System.Drawing.Color.White
        Me.Label8.Location = New System.Drawing.Point(155, 54)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(48, 13)
        Me.Label8.TabIndex = 3
        Me.Label8.Text = "Répéter:"
        '
        'TxtCmdName
        '
        Me.TxtCmdName.Location = New System.Drawing.Point(208, 23)
        Me.TxtCmdName.Name = "TxtCmdName"
        Me.TxtCmdName.Size = New System.Drawing.Size(294, 20)
        Me.TxtCmdName.TabIndex = 2
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.ForeColor = System.Drawing.Color.White
        Me.Label7.Location = New System.Drawing.Point(171, 26)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(32, 13)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "Nom:"
        '
        'ListCmd
        '
        Me.ListCmd.FormattingEnabled = True
        Me.ListCmd.Location = New System.Drawing.Point(6, 25)
        Me.ListCmd.Name = "ListCmd"
        Me.ListCmd.Size = New System.Drawing.Size(143, 160)
        Me.ListCmd.TabIndex = 0
        '
        'CbTypeClass
        '
        Me.CbTypeClass.BackColor = System.Drawing.Color.Silver
        Me.CbTypeClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbTypeClass.FormattingEnabled = True
        Me.CbTypeClass.Items.AddRange(New Object() {"audiozone", "contact", "freebox", "lightingdimmer", "lightingswitch", "meteo", "temperature", "tv", "web"})
        Me.CbTypeClass.Location = New System.Drawing.Point(66, 108)
        Me.CbTypeClass.Name = "CbTypeClass"
        Me.CbTypeClass.Size = New System.Drawing.Size(236, 21)
        Me.CbTypeClass.Sorted = True
        Me.CbTypeClass.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(8, 108)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(39, 13)
        Me.Label2.TabIndex = 18
        Me.Label2.Text = "Type:"
        '
        'BtnLoadImg
        '
        Me.BtnLoadImg.Location = New System.Drawing.Point(309, 162)
        Me.BtnLoadImg.Name = "BtnLoadImg"
        Me.BtnLoadImg.Size = New System.Drawing.Size(25, 25)
        Me.BtnLoadImg.TabIndex = 19
        Me.BtnLoadImg.Text = "..."
        Me.BtnLoadImg.UseVisualStyleBackColor = True
        '
        'MyImage
        '
        Me.MyImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.MyImage.Location = New System.Drawing.Point(465, 120)
        Me.MyImage.Name = "MyImage"
        Me.MyImage.Size = New System.Drawing.Size(65, 65)
        Me.MyImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.MyImage.TabIndex = 20
        Me.MyImage.TabStop = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Lucida Sans Unicode", 20.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Orange
        Me.Label6.Location = New System.Drawing.Point(228, -1)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(107, 34)
        Me.Label6.TabIndex = 21
        Me.Label6.Text = "Device"
        '
        'FrmDevice
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(558, 203)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.MyImage)
        Me.Controls.Add(Me.BtnLoadImg)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.CbTypeClass)
        Me.Controls.Add(Me.Grp14)
        Me.Controls.Add(Me.TxtName)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.BtnOk)
        Me.Controls.Add(Me.BtnAnnuler)
        Me.Controls.Add(Me.cEnable)
        Me.Controls.Add(Me.TxtImg)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TxtAdress)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.LDriver)
        Me.Controls.Add(Me.Label1)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FrmDevice"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Device"
        Me.Grp14.ResumeLayout(False)
        Me.Grp14.PerformLayout()
        CType(Me.MyImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents LDriver As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TxtAdress As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TxtImg As System.Windows.Forms.TextBox
    Friend WithEvents cEnable As System.Windows.Forms.CheckBox
    Friend WithEvents BtnAnnuler As System.Windows.Forms.Button
    Friend WithEvents BtnOk As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TxtName As System.Windows.Forms.TextBox
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents Grp14 As System.Windows.Forms.GroupBox
    Friend WithEvents ListCmd As System.Windows.Forms.ListBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TxtCmdRepeat As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents TxtCmdName As System.Windows.Forms.TextBox
    Friend WithEvents TxtCmdData As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents BtnLearn As System.Windows.Forms.Button
    Friend WithEvents BtnTstCmd As System.Windows.Forms.Button
    Friend WithEvents BtnDelCmd As System.Windows.Forms.Button
    Friend WithEvents BtnSaveCmd As System.Windows.Forms.Button
    Friend WithEvents BtnNewCmd As System.Windows.Forms.Button
    Friend WithEvents CbTypeClass As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents BtnLoadImg As System.Windows.Forms.Button
    Friend WithEvents MyImage As System.Windows.Forms.PictureBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
End Class
