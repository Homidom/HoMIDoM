<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmTrigger
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.TxtDevice = New System.Windows.Forms.ComboBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.cEnable = New System.Windows.Forms.CheckBox
        Me.TxtNom = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.CbCondition = New System.Windows.Forms.ComboBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.CbValue = New System.Windows.Forms.ComboBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.CbStatus = New System.Windows.Forms.ComboBox
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.BtnDown = New System.Windows.Forms.Button
        Me.BtnUp = New System.Windows.Forms.Button
        Me.BtnDelAll = New System.Windows.Forms.Button
        Me.BtnUnSelect = New System.Windows.Forms.Button
        Me.BtnSelect = New System.Windows.Forms.Button
        Me.ListBoxScriptDispo = New System.Windows.Forms.ListBox
        Me.ListBoxScriptSelect = New System.Windows.Forms.ListBox
        Me.BtnOK = New System.Windows.Forms.Button
        Me.BtnAnnuler = New System.Windows.Forms.Button
        Me.Label6 = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TxtDevice)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.cEnable)
        Me.GroupBox1.Controls.Add(Me.TxtNom)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox1.Location = New System.Drawing.Point(12, 62)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(401, 85)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Trigger Details"
        '
        'TxtDevice
        '
        Me.TxtDevice.BackColor = System.Drawing.Color.Silver
        Me.TxtDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TxtDevice.FormattingEnabled = True
        Me.TxtDevice.Location = New System.Drawing.Point(66, 53)
        Me.TxtDevice.Name = "TxtDevice"
        Me.TxtDevice.Size = New System.Drawing.Size(245, 21)
        Me.TxtDevice.TabIndex = 6
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(15, 57)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(44, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Device:"
        '
        'cEnable
        '
        Me.cEnable.AutoSize = True
        Me.cEnable.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cEnable.ForeColor = System.Drawing.Color.White
        Me.cEnable.Location = New System.Drawing.Point(330, 20)
        Me.cEnable.Name = "cEnable"
        Me.cEnable.Size = New System.Drawing.Size(59, 17)
        Me.cEnable.TabIndex = 2
        Me.cEnable.Text = "Enable"
        Me.cEnable.UseVisualStyleBackColor = True
        '
        'TxtNom
        '
        Me.TxtNom.BackColor = System.Drawing.Color.Silver
        Me.TxtNom.Location = New System.Drawing.Point(67, 19)
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
        Me.Label1.Location = New System.Drawing.Point(15, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Nom:"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.CbCondition)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.CbValue)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.CbStatus)
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox2.Location = New System.Drawing.Point(12, 153)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(593, 81)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Condition Details"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(233, 21)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(54, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Condition:"
        '
        'CbCondition
        '
        Me.CbCondition.BackColor = System.Drawing.Color.Silver
        Me.CbCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbCondition.FormattingEnabled = True
        Me.CbCondition.Items.AddRange(New Object() {"=", ">", "<", ">=", "<=", "<>"})
        Me.CbCondition.Location = New System.Drawing.Point(236, 42)
        Me.CbCondition.Name = "CbCondition"
        Me.CbCondition.Size = New System.Drawing.Size(76, 21)
        Me.CbCondition.TabIndex = 10
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(315, 21)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(40, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Valeur:"
        '
        'CbValue
        '
        Me.CbValue.BackColor = System.Drawing.Color.Silver
        Me.CbValue.FormattingEnabled = True
        Me.CbValue.Location = New System.Drawing.Point(318, 42)
        Me.CbValue.Name = "CbValue"
        Me.CbValue.Size = New System.Drawing.Size(196, 21)
        Me.CbValue.TabIndex = 8
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(15, 21)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(40, 13)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Status:"
        '
        'CbStatus
        '
        Me.CbStatus.BackColor = System.Drawing.Color.Silver
        Me.CbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbStatus.FormattingEnabled = True
        Me.CbStatus.Location = New System.Drawing.Point(18, 42)
        Me.CbStatus.Name = "CbStatus"
        Me.CbStatus.Size = New System.Drawing.Size(210, 21)
        Me.CbStatus.TabIndex = 0
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.BtnDown)
        Me.GroupBox3.Controls.Add(Me.BtnUp)
        Me.GroupBox3.Controls.Add(Me.BtnDelAll)
        Me.GroupBox3.Controls.Add(Me.BtnUnSelect)
        Me.GroupBox3.Controls.Add(Me.BtnSelect)
        Me.GroupBox3.Controls.Add(Me.ListBoxScriptDispo)
        Me.GroupBox3.Controls.Add(Me.ListBoxScriptSelect)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox3.Location = New System.Drawing.Point(11, 240)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(592, 190)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Script Details"
        '
        'BtnDown
        '
        Me.BtnDown.ForeColor = System.Drawing.Color.Black
        Me.BtnDown.Location = New System.Drawing.Point(474, 83)
        Me.BtnDown.Name = "BtnDown"
        Me.BtnDown.Size = New System.Drawing.Size(85, 28)
        Me.BtnDown.TabIndex = 6
        Me.BtnDown.Text = "Descendre"
        Me.BtnDown.UseVisualStyleBackColor = True
        '
        'BtnUp
        '
        Me.BtnUp.ForeColor = System.Drawing.Color.Black
        Me.BtnUp.Location = New System.Drawing.Point(474, 51)
        Me.BtnUp.Name = "BtnUp"
        Me.BtnUp.Size = New System.Drawing.Size(85, 28)
        Me.BtnUp.TabIndex = 5
        Me.BtnUp.Text = "Monter"
        Me.BtnUp.UseVisualStyleBackColor = True
        '
        'BtnDelAll
        '
        Me.BtnDelAll.ForeColor = System.Drawing.Color.Black
        Me.BtnDelAll.Location = New System.Drawing.Point(474, 19)
        Me.BtnDelAll.Name = "BtnDelAll"
        Me.BtnDelAll.Size = New System.Drawing.Size(85, 28)
        Me.BtnDelAll.TabIndex = 4
        Me.BtnDelAll.Text = "Effacer Tous"
        Me.BtnDelAll.UseVisualStyleBackColor = True
        '
        'BtnUnSelect
        '
        Me.BtnUnSelect.ForeColor = System.Drawing.Color.Black
        Me.BtnUnSelect.Location = New System.Drawing.Point(474, 115)
        Me.BtnUnSelect.Name = "BtnUnSelect"
        Me.BtnUnSelect.Size = New System.Drawing.Size(85, 28)
        Me.BtnUnSelect.TabIndex = 3
        Me.BtnUnSelect.Text = "Supprimer"
        Me.BtnUnSelect.UseVisualStyleBackColor = True
        '
        'BtnSelect
        '
        Me.BtnSelect.Location = New System.Drawing.Point(237, 21)
        Me.BtnSelect.Name = "BtnSelect"
        Me.BtnSelect.Size = New System.Drawing.Size(24, 23)
        Me.BtnSelect.TabIndex = 2
        Me.BtnSelect.Text = ">>"
        Me.BtnSelect.UseVisualStyleBackColor = True
        '
        'ListBoxScriptDispo
        '
        Me.ListBoxScriptDispo.BackColor = System.Drawing.Color.DarkGray
        Me.ListBoxScriptDispo.ForeColor = System.Drawing.Color.White
        Me.ListBoxScriptDispo.FormattingEnabled = True
        Me.ListBoxScriptDispo.Location = New System.Drawing.Point(19, 21)
        Me.ListBoxScriptDispo.Name = "ListBoxScriptDispo"
        Me.ListBoxScriptDispo.Size = New System.Drawing.Size(210, 160)
        Me.ListBoxScriptDispo.TabIndex = 1
        '
        'ListBoxScriptSelect
        '
        Me.ListBoxScriptSelect.BackColor = System.Drawing.Color.DarkGray
        Me.ListBoxScriptSelect.ForeColor = System.Drawing.Color.White
        Me.ListBoxScriptSelect.FormattingEnabled = True
        Me.ListBoxScriptSelect.Location = New System.Drawing.Point(268, 21)
        Me.ListBoxScriptSelect.Name = "ListBoxScriptSelect"
        Me.ListBoxScriptSelect.Size = New System.Drawing.Size(200, 160)
        Me.ListBoxScriptSelect.TabIndex = 0
        '
        'BtnOK
        '
        Me.BtnOK.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnOK.Location = New System.Drawing.Point(518, 62)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(85, 28)
        Me.BtnOK.TabIndex = 3
        Me.BtnOK.Text = "Ok"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnAnnuler
        '
        Me.BtnAnnuler.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnAnnuler.Location = New System.Drawing.Point(518, 96)
        Me.BtnAnnuler.Name = "BtnAnnuler"
        Me.BtnAnnuler.Size = New System.Drawing.Size(85, 28)
        Me.BtnAnnuler.TabIndex = 4
        Me.BtnAnnuler.Text = "Annuler"
        Me.BtnAnnuler.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Lucida Sans Unicode", 20.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Orange
        Me.Label6.Location = New System.Drawing.Point(245, 9)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(118, 34)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "Trigger"
        '
        'FrmTrigger
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.ClientSize = New System.Drawing.Size(617, 442)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.BtnAnnuler)
        Me.Controls.Add(Me.BtnOK)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FrmTrigger"
        Me.Text = "Trigger"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents cEnable As System.Windows.Forms.CheckBox
    Friend WithEvents TxtNom As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents CbStatus As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CbValue As System.Windows.Forms.ComboBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents BtnOK As System.Windows.Forms.Button
    Friend WithEvents BtnAnnuler As System.Windows.Forms.Button
    Friend WithEvents CbCondition As System.Windows.Forms.ComboBox
    Friend WithEvents TxtDevice As System.Windows.Forms.ComboBox
    Friend WithEvents ListBoxScriptSelect As System.Windows.Forms.ListBox
    Friend WithEvents BtnDown As System.Windows.Forms.Button
    Friend WithEvents BtnUp As System.Windows.Forms.Button
    Friend WithEvents BtnDelAll As System.Windows.Forms.Button
    Friend WithEvents BtnUnSelect As System.Windows.Forms.Button
    Friend WithEvents BtnSelect As System.Windows.Forms.Button
    Friend WithEvents ListBoxScriptDispo As System.Windows.Forms.ListBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
End Class
