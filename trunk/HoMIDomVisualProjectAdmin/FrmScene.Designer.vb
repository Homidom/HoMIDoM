<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmScene
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmScene))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.BtnTest = New System.Windows.Forms.Button
        Me.BtnAnnuler = New System.Windows.Forms.Button
        Me.cEnable = New System.Windows.Forms.CheckBox
        Me.BtnOK = New System.Windows.Forms.Button
        Me.TxtNom = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.BtnDeleteAll = New System.Windows.Forms.PictureBox
        Me.BtnNewActionIf = New System.Windows.Forms.PictureBox
        Me.BtnNewActionSpeek = New System.Windows.Forms.PictureBox
        Me.BtnNewActionMail = New System.Windows.Forms.PictureBox
        Me.BtnNewActionExit = New System.Windows.Forms.PictureBox
        Me.BtnNewActionPause = New System.Windows.Forms.PictureBox
        Me.BtnNewActionDevice = New System.Windows.Forms.PictureBox
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.Label6 = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.BtnDeleteAll, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionIf, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionSpeek, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionMail, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionExit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionPause, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionDevice, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.BtnTest)
        Me.GroupBox1.Controls.Add(Me.BtnAnnuler)
        Me.GroupBox1.Controls.Add(Me.cEnable)
        Me.GroupBox1.Controls.Add(Me.BtnOK)
        Me.GroupBox1.Controls.Add(Me.TxtNom)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox1.Location = New System.Drawing.Point(5, 61)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(763, 55)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Script Details"
        '
        'BtnTest
        '
        Me.BtnTest.ForeColor = System.Drawing.Color.Black
        Me.BtnTest.Location = New System.Drawing.Point(373, 14)
        Me.BtnTest.Name = "BtnTest"
        Me.BtnTest.Size = New System.Drawing.Size(80, 27)
        Me.BtnTest.TabIndex = 5
        Me.BtnTest.Text = "Tester"
        Me.BtnTest.UseVisualStyleBackColor = True
        '
        'BtnAnnuler
        '
        Me.BtnAnnuler.ForeColor = System.Drawing.Color.Black
        Me.BtnAnnuler.Location = New System.Drawing.Point(662, 14)
        Me.BtnAnnuler.Name = "BtnAnnuler"
        Me.BtnAnnuler.Size = New System.Drawing.Size(80, 27)
        Me.BtnAnnuler.TabIndex = 4
        Me.BtnAnnuler.Text = "Fermer"
        Me.BtnAnnuler.UseVisualStyleBackColor = True
        '
        'cEnable
        '
        Me.cEnable.AutoSize = True
        Me.cEnable.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cEnable.ForeColor = System.Drawing.Color.White
        Me.cEnable.Location = New System.Drawing.Point(308, 20)
        Me.cEnable.Name = "cEnable"
        Me.cEnable.Size = New System.Drawing.Size(59, 17)
        Me.cEnable.TabIndex = 2
        Me.cEnable.Text = "Enable"
        Me.cEnable.UseVisualStyleBackColor = True
        '
        'BtnOK
        '
        Me.BtnOK.ForeColor = System.Drawing.Color.Black
        Me.BtnOK.Location = New System.Drawing.Point(576, 14)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(80, 27)
        Me.BtnOK.TabIndex = 3
        Me.BtnOK.Text = "OK"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'TxtNom
        '
        Me.TxtNom.BackColor = System.Drawing.Color.Silver
        Me.TxtNom.Location = New System.Drawing.Point(53, 18)
        Me.TxtNom.MaxLength = 64
        Me.TxtNom.Name = "TxtNom"
        Me.TxtNom.Size = New System.Drawing.Size(245, 20)
        Me.TxtNom.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(15, 21)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Nom:"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.BtnDeleteAll)
        Me.GroupBox3.Controls.Add(Me.BtnNewActionIf)
        Me.GroupBox3.Controls.Add(Me.BtnNewActionSpeek)
        Me.GroupBox3.Controls.Add(Me.BtnNewActionMail)
        Me.GroupBox3.Controls.Add(Me.BtnNewActionExit)
        Me.GroupBox3.Controls.Add(Me.BtnNewActionPause)
        Me.GroupBox3.Controls.Add(Me.BtnNewActionDevice)
        Me.GroupBox3.Controls.Add(Me.TableLayoutPanel1)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox3.Location = New System.Drawing.Point(5, 122)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(787, 525)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Script Actions"
        '
        'BtnDeleteAll
        '
        Me.BtnDeleteAll.Image = CType(resources.GetObject("BtnDeleteAll.Image"), System.Drawing.Image)
        Me.BtnDeleteAll.Location = New System.Drawing.Point(748, 22)
        Me.BtnDeleteAll.Name = "BtnDeleteAll"
        Me.BtnDeleteAll.Size = New System.Drawing.Size(30, 30)
        Me.BtnDeleteAll.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnDeleteAll.TabIndex = 22
        Me.BtnDeleteAll.TabStop = False
        '
        'BtnNewActionIf
        '
        Me.BtnNewActionIf.Image = CType(resources.GetObject("BtnNewActionIf.Image"), System.Drawing.Image)
        Me.BtnNewActionIf.Location = New System.Drawing.Point(192, 22)
        Me.BtnNewActionIf.Name = "BtnNewActionIf"
        Me.BtnNewActionIf.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionIf.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionIf.TabIndex = 21
        Me.BtnNewActionIf.TabStop = False
        '
        'BtnNewActionSpeek
        '
        Me.BtnNewActionSpeek.Image = CType(resources.GetObject("BtnNewActionSpeek.Image"), System.Drawing.Image)
        Me.BtnNewActionSpeek.Location = New System.Drawing.Point(155, 22)
        Me.BtnNewActionSpeek.Name = "BtnNewActionSpeek"
        Me.BtnNewActionSpeek.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionSpeek.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionSpeek.TabIndex = 20
        Me.BtnNewActionSpeek.TabStop = False
        '
        'BtnNewActionMail
        '
        Me.BtnNewActionMail.Image = CType(resources.GetObject("BtnNewActionMail.Image"), System.Drawing.Image)
        Me.BtnNewActionMail.Location = New System.Drawing.Point(118, 22)
        Me.BtnNewActionMail.Name = "BtnNewActionMail"
        Me.BtnNewActionMail.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionMail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionMail.TabIndex = 19
        Me.BtnNewActionMail.TabStop = False
        '
        'BtnNewActionExit
        '
        Me.BtnNewActionExit.Image = CType(resources.GetObject("BtnNewActionExit.Image"), System.Drawing.Image)
        Me.BtnNewActionExit.Location = New System.Drawing.Point(81, 22)
        Me.BtnNewActionExit.Name = "BtnNewActionExit"
        Me.BtnNewActionExit.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionExit.TabIndex = 18
        Me.BtnNewActionExit.TabStop = False
        '
        'BtnNewActionPause
        '
        Me.BtnNewActionPause.Image = CType(resources.GetObject("BtnNewActionPause.Image"), System.Drawing.Image)
        Me.BtnNewActionPause.Location = New System.Drawing.Point(44, 22)
        Me.BtnNewActionPause.Name = "BtnNewActionPause"
        Me.BtnNewActionPause.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionPause.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionPause.TabIndex = 17
        Me.BtnNewActionPause.TabStop = False
        '
        'BtnNewActionDevice
        '
        Me.BtnNewActionDevice.Image = CType(resources.GetObject("BtnNewActionDevice.Image"), System.Drawing.Image)
        Me.BtnNewActionDevice.Location = New System.Drawing.Point(7, 22)
        Me.BtnNewActionDevice.Name = "BtnNewActionDevice"
        Me.BtnNewActionDevice.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionDevice.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionDevice.TabIndex = 16
        Me.BtnNewActionDevice.TabStop = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoScroll = True
        Me.TableLayoutPanel1.BackColor = System.Drawing.Color.Gray
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(7, 62)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(774, 456)
        Me.TableLayoutPanel1.TabIndex = 14
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Lucida Sans Unicode", 20.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Orange
        Me.Label6.Location = New System.Drawing.Point(282, 9)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(96, 34)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "Script"
        '
        'FrmScene
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.ClientSize = New System.Drawing.Size(794, 681)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FrmScene"
        Me.Text = "Scene"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        CType(Me.BtnDeleteAll, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionIf, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionSpeek, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionMail, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionExit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionPause, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionDevice, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents cEnable As System.Windows.Forms.CheckBox
    Friend WithEvents TxtNom As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents BtnOK As System.Windows.Forms.Button
    Friend WithEvents BtnAnnuler As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents BtnNewActionDevice As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionIf As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionSpeek As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionMail As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionExit As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionPause As System.Windows.Forms.PictureBox
    Friend WithEvents BtnDeleteAll As System.Windows.Forms.PictureBox
    Friend WithEvents BtnTest As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
End Class
