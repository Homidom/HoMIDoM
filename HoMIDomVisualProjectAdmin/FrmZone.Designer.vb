<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmZone
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
        Me.TxtName = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.CbDeviceDispo = New System.Windows.Forms.ComboBox
        Me.BtnAjoutDevice = New System.Windows.Forms.Button
        Me.ListDeviceSelect = New System.Windows.Forms.ListBox
        Me.BtnDelDevice = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.TxtImage = New System.Windows.Forms.TextBox
        Me.BtnLoadImage = New System.Windows.Forms.Button
        Me.MyImage = New System.Windows.Forms.PictureBox
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.BtnOK = New System.Windows.Forms.Button
        Me.BtnCancel = New System.Windows.Forms.Button
        Me.Label6 = New System.Windows.Forms.Label
        CType(Me.MyImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(13, 67)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Zone:"
        '
        'TxtName
        '
        Me.TxtName.BackColor = System.Drawing.Color.Silver
        Me.TxtName.Enabled = False
        Me.TxtName.Location = New System.Drawing.Point(125, 64)
        Me.TxtName.Name = "TxtName"
        Me.TxtName.Size = New System.Drawing.Size(223, 20)
        Me.TxtName.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(13, 125)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(106, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Devices Disponibles:"
        '
        'CbDeviceDispo
        '
        Me.CbDeviceDispo.BackColor = System.Drawing.Color.Silver
        Me.CbDeviceDispo.FormattingEnabled = True
        Me.CbDeviceDispo.Location = New System.Drawing.Point(125, 122)
        Me.CbDeviceDispo.Name = "CbDeviceDispo"
        Me.CbDeviceDispo.Size = New System.Drawing.Size(223, 21)
        Me.CbDeviceDispo.TabIndex = 3
        '
        'BtnAjoutDevice
        '
        Me.BtnAjoutDevice.Location = New System.Drawing.Point(354, 119)
        Me.BtnAjoutDevice.Name = "BtnAjoutDevice"
        Me.BtnAjoutDevice.Size = New System.Drawing.Size(25, 25)
        Me.BtnAjoutDevice.TabIndex = 4
        Me.BtnAjoutDevice.Text = "+"
        Me.BtnAjoutDevice.UseVisualStyleBackColor = True
        '
        'ListDeviceSelect
        '
        Me.ListDeviceSelect.BackColor = System.Drawing.Color.Silver
        Me.ListDeviceSelect.FormattingEnabled = True
        Me.ListDeviceSelect.Location = New System.Drawing.Point(13, 175)
        Me.ListDeviceSelect.Name = "ListDeviceSelect"
        Me.ListDeviceSelect.Size = New System.Drawing.Size(374, 212)
        Me.ListDeviceSelect.TabIndex = 5
        '
        'BtnDelDevice
        '
        Me.BtnDelDevice.Location = New System.Drawing.Point(393, 175)
        Me.BtnDelDevice.Name = "BtnDelDevice"
        Me.BtnDelDevice.Size = New System.Drawing.Size(25, 25)
        Me.BtnDelDevice.TabIndex = 6
        Me.BtnDelDevice.Text = "-"
        Me.BtnDelDevice.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(13, 156)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(144, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Liste des devices de la zone:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(13, 96)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(39, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Image:"
        '
        'TxtImage
        '
        Me.TxtImage.BackColor = System.Drawing.Color.Silver
        Me.TxtImage.Location = New System.Drawing.Point(125, 93)
        Me.TxtImage.Name = "TxtImage"
        Me.TxtImage.Size = New System.Drawing.Size(223, 20)
        Me.TxtImage.TabIndex = 9
        '
        'BtnLoadImage
        '
        Me.BtnLoadImage.Location = New System.Drawing.Point(354, 90)
        Me.BtnLoadImage.Name = "BtnLoadImage"
        Me.BtnLoadImage.Size = New System.Drawing.Size(25, 25)
        Me.BtnLoadImage.TabIndex = 10
        Me.BtnLoadImage.Text = "..."
        Me.BtnLoadImage.UseVisualStyleBackColor = True
        '
        'MyImage
        '
        Me.MyImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.MyImage.Location = New System.Drawing.Point(443, 135)
        Me.MyImage.Name = "MyImage"
        Me.MyImage.Size = New System.Drawing.Size(52, 52)
        Me.MyImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.MyImage.TabIndex = 11
        Me.MyImage.TabStop = False
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'BtnOK
        '
        Me.BtnOK.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnOK.Location = New System.Drawing.Point(421, 59)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(85, 28)
        Me.BtnOK.TabIndex = 12
        Me.BtnOK.Text = "Ok"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCancel.Location = New System.Drawing.Point(421, 93)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(85, 28)
        Me.BtnCancel.TabIndex = 13
        Me.BtnCancel.Text = "Annuler"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Lucida Sans Unicode", 20.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Orange
        Me.Label6.Location = New System.Drawing.Point(208, 9)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(84, 34)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "Zone"
        '
        'FrmZone
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.ClientSize = New System.Drawing.Size(518, 399)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.BtnCancel)
        Me.Controls.Add(Me.BtnOK)
        Me.Controls.Add(Me.MyImage)
        Me.Controls.Add(Me.BtnLoadImage)
        Me.Controls.Add(Me.TxtImage)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.BtnDelDevice)
        Me.Controls.Add(Me.ListDeviceSelect)
        Me.Controls.Add(Me.BtnAjoutDevice)
        Me.Controls.Add(Me.CbDeviceDispo)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TxtName)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FrmZone"
        Me.Text = "Zones"
        CType(Me.MyImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TxtName As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents CbDeviceDispo As System.Windows.Forms.ComboBox
    Friend WithEvents BtnAjoutDevice As System.Windows.Forms.Button
    Friend WithEvents ListDeviceSelect As System.Windows.Forms.ListBox
    Friend WithEvents BtnDelDevice As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TxtImage As System.Windows.Forms.TextBox
    Friend WithEvents BtnLoadImage As System.Windows.Forms.Button
    Friend WithEvents MyImage As System.Windows.Forms.PictureBox
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents BtnOK As System.Windows.Forms.Button
    Friend WithEvents BtnCancel As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
End Class
