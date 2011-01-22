<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmSchedule
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
        Me.BtnAnnuler = New System.Windows.Forms.Button
        Me.BtnOK = New System.Windows.Forms.Button
        Me.cEnable = New System.Windows.Forms.CheckBox
        Me.TxtNom = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.PanelOffset = New System.Windows.Forms.Panel
        Me.R1 = New System.Windows.Forms.RadioButton
        Me.R0 = New System.Windows.Forms.RadioButton
        Me.OffSetTime = New System.Windows.Forms.DateTimePicker
        Me.LblOffset = New System.Windows.Forms.Label
        Me.cTgType = New System.Windows.Forms.ComboBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.BtnCalEnd = New System.Windows.Forms.Button
        Me.LblTimeEnd = New System.Windows.Forms.Label
        Me.RE1 = New System.Windows.Forms.RadioButton
        Me.RE0 = New System.Windows.Forms.RadioButton
        Me.BtnCalStart = New System.Windows.Forms.Button
        Me.LblTimeStart = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.PanelRJ = New System.Windows.Forms.Panel
        Me.CJ6 = New System.Windows.Forms.CheckBox
        Me.CJ5 = New System.Windows.Forms.CheckBox
        Me.CJ4 = New System.Windows.Forms.CheckBox
        Me.CJ3 = New System.Windows.Forms.CheckBox
        Me.CJ2 = New System.Windows.Forms.CheckBox
        Me.CJ1 = New System.Windows.Forms.CheckBox
        Me.CJ0 = New System.Windows.Forms.CheckBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.PanelRC = New System.Windows.Forms.Panel
        Me.RC3 = New System.Windows.Forms.RadioButton
        Me.RC2 = New System.Windows.Forms.RadioButton
        Me.RC1 = New System.Windows.Forms.RadioButton
        Me.RC0 = New System.Windows.Forms.RadioButton
        Me.GroupBox4 = New System.Windows.Forms.GroupBox
        Me.vTime = New System.Windows.Forms.DomainUpDown
        Me.BtnCalOk = New System.Windows.Forms.Button
        Me.BtnCalAnnuler = New System.Windows.Forms.Button
        Me.LblCalendarTime = New System.Windows.Forms.Label
        Me.MonthCalendar1 = New System.Windows.Forms.MonthCalendar
        Me.GroupBox5 = New System.Windows.Forms.GroupBox
        Me.BtnDown = New System.Windows.Forms.Button
        Me.BtnUp = New System.Windows.Forms.Button
        Me.BtnDelAll = New System.Windows.Forms.Button
        Me.BtnUnSelect = New System.Windows.Forms.Button
        Me.BtnSelect = New System.Windows.Forms.Button
        Me.ListBoxScriptDispo = New System.Windows.Forms.ListBox
        Me.ListBoxScriptSelect = New System.Windows.Forms.ListBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.PanelOffset.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.PanelRJ.SuspendLayout()
        Me.PanelRC.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.BtnAnnuler)
        Me.GroupBox1.Controls.Add(Me.BtnOK)
        Me.GroupBox1.Controls.Add(Me.cEnable)
        Me.GroupBox1.Controls.Add(Me.TxtNom)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox1.Location = New System.Drawing.Point(7, 56)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(564, 89)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Schedule Details"
        '
        'BtnAnnuler
        '
        Me.BtnAnnuler.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnAnnuler.ForeColor = System.Drawing.Color.Black
        Me.BtnAnnuler.Location = New System.Drawing.Point(456, 49)
        Me.BtnAnnuler.Name = "BtnAnnuler"
        Me.BtnAnnuler.Size = New System.Drawing.Size(85, 28)
        Me.BtnAnnuler.TabIndex = 6
        Me.BtnAnnuler.Text = "Annuler"
        Me.BtnAnnuler.UseVisualStyleBackColor = True
        '
        'BtnOK
        '
        Me.BtnOK.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnOK.ForeColor = System.Drawing.Color.Black
        Me.BtnOK.Location = New System.Drawing.Point(456, 16)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(85, 28)
        Me.BtnOK.TabIndex = 5
        Me.BtnOK.Text = "Ok"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'cEnable
        '
        Me.cEnable.AutoSize = True
        Me.cEnable.Checked = True
        Me.cEnable.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cEnable.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cEnable.ForeColor = System.Drawing.Color.White
        Me.cEnable.Location = New System.Drawing.Point(284, 23)
        Me.cEnable.Name = "cEnable"
        Me.cEnable.Size = New System.Drawing.Size(59, 17)
        Me.cEnable.TabIndex = 3
        Me.cEnable.Text = "Enable"
        Me.cEnable.UseVisualStyleBackColor = True
        '
        'TxtNom
        '
        Me.TxtNom.BackColor = System.Drawing.Color.Silver
        Me.TxtNom.Location = New System.Drawing.Point(57, 23)
        Me.TxtNom.Name = "TxtNom"
        Me.TxtNom.Size = New System.Drawing.Size(211, 20)
        Me.TxtNom.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(13, 26)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Nom:"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.PanelOffset)
        Me.GroupBox2.Controls.Add(Me.cTgType)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox2.Location = New System.Drawing.Point(8, 151)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(564, 67)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Trigger Detail"
        '
        'PanelOffset
        '
        Me.PanelOffset.Controls.Add(Me.R1)
        Me.PanelOffset.Controls.Add(Me.R0)
        Me.PanelOffset.Controls.Add(Me.OffSetTime)
        Me.PanelOffset.Controls.Add(Me.LblOffset)
        Me.PanelOffset.Location = New System.Drawing.Point(295, 18)
        Me.PanelOffset.Name = "PanelOffset"
        Me.PanelOffset.Size = New System.Drawing.Size(247, 43)
        Me.PanelOffset.TabIndex = 2
        Me.PanelOffset.Visible = False
        '
        'R1
        '
        Me.R1.AutoSize = True
        Me.R1.ForeColor = System.Drawing.Color.White
        Me.R1.Location = New System.Drawing.Point(125, 23)
        Me.R1.Name = "R1"
        Me.R1.Size = New System.Drawing.Size(57, 17)
        Me.R1.TabIndex = 7
        Me.R1.Text = "Après"
        Me.R1.UseVisualStyleBackColor = True
        '
        'R0
        '
        Me.R0.AutoSize = True
        Me.R0.Checked = True
        Me.R0.ForeColor = System.Drawing.Color.White
        Me.R0.Location = New System.Drawing.Point(125, 3)
        Me.R0.Name = "R0"
        Me.R0.Size = New System.Drawing.Size(58, 17)
        Me.R0.TabIndex = 6
        Me.R0.TabStop = True
        Me.R0.Text = "Avant"
        Me.R0.UseVisualStyleBackColor = True
        '
        'OffSetTime
        '
        Me.OffSetTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.OffSetTime.Location = New System.Drawing.Point(43, 7)
        Me.OffSetTime.Name = "OffSetTime"
        Me.OffSetTime.Size = New System.Drawing.Size(53, 20)
        Me.OffSetTime.TabIndex = 5
        '
        'LblOffset
        '
        Me.LblOffset.AutoSize = True
        Me.LblOffset.ForeColor = System.Drawing.Color.White
        Me.LblOffset.Location = New System.Drawing.Point(1, 9)
        Me.LblOffset.Name = "LblOffset"
        Me.LblOffset.Size = New System.Drawing.Size(41, 13)
        Me.LblOffset.TabIndex = 4
        Me.LblOffset.Text = "Offset"
        '
        'cTgType
        '
        Me.cTgType.BackColor = System.Drawing.Color.Silver
        Me.cTgType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cTgType.FormattingEnabled = True
        Me.cTgType.Items.AddRange(New Object() {"Custom", "SunRise", "SunSet"})
        Me.cTgType.Location = New System.Drawing.Point(88, 23)
        Me.cTgType.Name = "cTgType"
        Me.cTgType.Size = New System.Drawing.Size(178, 21)
        Me.cTgType.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(13, 25)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(70, 13)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Trigger Type:"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Panel1)
        Me.GroupBox3.Controls.Add(Me.BtnCalStart)
        Me.GroupBox3.Controls.Add(Me.LblTimeStart)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Controls.Add(Me.PanelRJ)
        Me.GroupBox3.Controls.Add(Me.PanelRC)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox3.Location = New System.Drawing.Point(8, 224)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(563, 223)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Reccurence"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.BtnCalEnd)
        Me.Panel1.Controls.Add(Me.LblTimeEnd)
        Me.Panel1.Controls.Add(Me.RE1)
        Me.Panel1.Controls.Add(Me.RE0)
        Me.Panel1.Location = New System.Drawing.Point(47, 163)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(408, 51)
        Me.Panel1.TabIndex = 7
        '
        'BtnCalEnd
        '
        Me.BtnCalEnd.Location = New System.Drawing.Point(338, 29)
        Me.BtnCalEnd.Name = "BtnCalEnd"
        Me.BtnCalEnd.Size = New System.Drawing.Size(23, 18)
        Me.BtnCalEnd.TabIndex = 3
        Me.BtnCalEnd.Text = "..."
        Me.BtnCalEnd.UseVisualStyleBackColor = True
        '
        'LblTimeEnd
        '
        Me.LblTimeEnd.ForeColor = System.Drawing.Color.White
        Me.LblTimeEnd.Location = New System.Drawing.Point(64, 32)
        Me.LblTimeEnd.Name = "LblTimeEnd"
        Me.LblTimeEnd.Size = New System.Drawing.Size(268, 15)
        Me.LblTimeEnd.TabIndex = 2
        Me.LblTimeEnd.Text = "##"
        '
        'RE1
        '
        Me.RE1.AutoSize = True
        Me.RE1.ForeColor = System.Drawing.Color.White
        Me.RE1.Location = New System.Drawing.Point(9, 31)
        Me.RE1.Name = "RE1"
        Me.RE1.Size = New System.Drawing.Size(60, 17)
        Me.RE1.TabIndex = 1
        Me.RE1.Text = "Fin le:"
        Me.RE1.UseVisualStyleBackColor = True
        '
        'RE0
        '
        Me.RE0.AutoSize = True
        Me.RE0.Checked = True
        Me.RE0.ForeColor = System.Drawing.Color.White
        Me.RE0.Location = New System.Drawing.Point(9, 8)
        Me.RE0.Name = "RE0"
        Me.RE0.Size = New System.Drawing.Size(82, 17)
        Me.RE0.TabIndex = 0
        Me.RE0.TabStop = True
        Me.RE0.Text = "Pas de fin"
        Me.RE0.UseVisualStyleBackColor = True
        '
        'BtnCalStart
        '
        Me.BtnCalStart.Location = New System.Drawing.Point(350, 136)
        Me.BtnCalStart.Name = "BtnCalStart"
        Me.BtnCalStart.Size = New System.Drawing.Size(29, 28)
        Me.BtnCalStart.TabIndex = 6
        Me.BtnCalStart.Text = "..."
        Me.BtnCalStart.UseVisualStyleBackColor = True
        '
        'LblTimeStart
        '
        Me.LblTimeStart.ForeColor = System.Drawing.Color.White
        Me.LblTimeStart.Location = New System.Drawing.Point(62, 140)
        Me.LblTimeStart.Name = "LblTimeStart"
        Me.LblTimeStart.Size = New System.Drawing.Size(279, 12)
        Me.LblTimeStart.TabIndex = 5
        Me.LblTimeStart.Text = "##"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Location = New System.Drawing.Point(14, 171)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(28, 13)
        Me.Label6.TabIndex = 4
        Me.Label6.Text = "Fin:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(14, 140)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(38, 13)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "Start:"
        '
        'PanelRJ
        '
        Me.PanelRJ.Controls.Add(Me.CJ6)
        Me.PanelRJ.Controls.Add(Me.CJ5)
        Me.PanelRJ.Controls.Add(Me.CJ4)
        Me.PanelRJ.Controls.Add(Me.CJ3)
        Me.PanelRJ.Controls.Add(Me.CJ2)
        Me.PanelRJ.Controls.Add(Me.CJ1)
        Me.PanelRJ.Controls.Add(Me.CJ0)
        Me.PanelRJ.Controls.Add(Me.Label4)
        Me.PanelRJ.Location = New System.Drawing.Point(121, 20)
        Me.PanelRJ.Name = "PanelRJ"
        Me.PanelRJ.Size = New System.Drawing.Size(419, 103)
        Me.PanelRJ.TabIndex = 1
        '
        'CJ6
        '
        Me.CJ6.AutoSize = True
        Me.CJ6.ForeColor = System.Drawing.Color.White
        Me.CJ6.Location = New System.Drawing.Point(238, 33)
        Me.CJ6.Name = "CJ6"
        Me.CJ6.Size = New System.Drawing.Size(35, 17)
        Me.CJ6.TabIndex = 9
        Me.CJ6.Text = "D"
        Me.CJ6.UseVisualStyleBackColor = True
        '
        'CJ5
        '
        Me.CJ5.AutoSize = True
        Me.CJ5.ForeColor = System.Drawing.Color.White
        Me.CJ5.Location = New System.Drawing.Point(200, 33)
        Me.CJ5.Name = "CJ5"
        Me.CJ5.Size = New System.Drawing.Size(34, 17)
        Me.CJ5.TabIndex = 8
        Me.CJ5.Text = "S"
        Me.CJ5.UseVisualStyleBackColor = True
        '
        'CJ4
        '
        Me.CJ4.AutoSize = True
        Me.CJ4.ForeColor = System.Drawing.Color.White
        Me.CJ4.Location = New System.Drawing.Point(162, 33)
        Me.CJ4.Name = "CJ4"
        Me.CJ4.Size = New System.Drawing.Size(34, 17)
        Me.CJ4.TabIndex = 7
        Me.CJ4.Text = "V"
        Me.CJ4.UseVisualStyleBackColor = True
        '
        'CJ3
        '
        Me.CJ3.AutoSize = True
        Me.CJ3.ForeColor = System.Drawing.Color.White
        Me.CJ3.Location = New System.Drawing.Point(129, 33)
        Me.CJ3.Name = "CJ3"
        Me.CJ3.Size = New System.Drawing.Size(32, 17)
        Me.CJ3.TabIndex = 6
        Me.CJ3.Text = "J"
        Me.CJ3.UseVisualStyleBackColor = True
        '
        'CJ2
        '
        Me.CJ2.AutoSize = True
        Me.CJ2.ForeColor = System.Drawing.Color.White
        Me.CJ2.Location = New System.Drawing.Point(91, 33)
        Me.CJ2.Name = "CJ2"
        Me.CJ2.Size = New System.Drawing.Size(36, 17)
        Me.CJ2.TabIndex = 5
        Me.CJ2.Text = "M"
        Me.CJ2.UseVisualStyleBackColor = True
        '
        'CJ1
        '
        Me.CJ1.AutoSize = True
        Me.CJ1.ForeColor = System.Drawing.Color.White
        Me.CJ1.Location = New System.Drawing.Point(53, 33)
        Me.CJ1.Name = "CJ1"
        Me.CJ1.Size = New System.Drawing.Size(36, 17)
        Me.CJ1.TabIndex = 4
        Me.CJ1.Text = "M"
        Me.CJ1.UseVisualStyleBackColor = True
        '
        'CJ0
        '
        Me.CJ0.AutoSize = True
        Me.CJ0.ForeColor = System.Drawing.Color.White
        Me.CJ0.Location = New System.Drawing.Point(15, 33)
        Me.CJ0.Name = "CJ0"
        Me.CJ0.Size = New System.Drawing.Size(33, 17)
        Me.CJ0.TabIndex = 3
        Me.CJ0.Text = "L"
        Me.CJ0.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(12, 7)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(45, 13)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "Jour(s)"
        '
        'PanelRC
        '
        Me.PanelRC.Controls.Add(Me.RC3)
        Me.PanelRC.Controls.Add(Me.RC2)
        Me.PanelRC.Controls.Add(Me.RC1)
        Me.PanelRC.Controls.Add(Me.RC0)
        Me.PanelRC.Location = New System.Drawing.Point(12, 19)
        Me.PanelRC.Name = "PanelRC"
        Me.PanelRC.Size = New System.Drawing.Size(92, 105)
        Me.PanelRC.TabIndex = 0
        '
        'RC3
        '
        Me.RC3.AutoSize = True
        Me.RC3.ForeColor = System.Drawing.Color.White
        Me.RC3.Location = New System.Drawing.Point(9, 75)
        Me.RC3.Name = "RC3"
        Me.RC3.Size = New System.Drawing.Size(61, 17)
        Me.RC3.TabIndex = 3
        Me.RC3.Text = "Année"
        Me.RC3.UseVisualStyleBackColor = True
        '
        'RC2
        '
        Me.RC2.AutoSize = True
        Me.RC2.ForeColor = System.Drawing.Color.White
        Me.RC2.Location = New System.Drawing.Point(9, 52)
        Me.RC2.Name = "RC2"
        Me.RC2.Size = New System.Drawing.Size(51, 17)
        Me.RC2.TabIndex = 2
        Me.RC2.Text = "Mois"
        Me.RC2.UseVisualStyleBackColor = True
        '
        'RC1
        '
        Me.RC1.AutoSize = True
        Me.RC1.ForeColor = System.Drawing.Color.White
        Me.RC1.Location = New System.Drawing.Point(9, 29)
        Me.RC1.Name = "RC1"
        Me.RC1.Size = New System.Drawing.Size(73, 17)
        Me.RC1.TabIndex = 1
        Me.RC1.Text = "Semaine"
        Me.RC1.UseVisualStyleBackColor = True
        '
        'RC0
        '
        Me.RC0.AutoSize = True
        Me.RC0.Checked = True
        Me.RC0.ForeColor = System.Drawing.Color.White
        Me.RC0.Location = New System.Drawing.Point(9, 6)
        Me.RC0.Name = "RC0"
        Me.RC0.Size = New System.Drawing.Size(49, 17)
        Me.RC0.TabIndex = 0
        Me.RC0.TabStop = True
        Me.RC0.Text = "Jour"
        Me.RC0.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.vTime)
        Me.GroupBox4.Controls.Add(Me.BtnCalOk)
        Me.GroupBox4.Controls.Add(Me.BtnCalAnnuler)
        Me.GroupBox4.Controls.Add(Me.LblCalendarTime)
        Me.GroupBox4.Controls.Add(Me.MonthCalendar1)
        Me.GroupBox4.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox4.Location = New System.Drawing.Point(8, 576)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(559, 220)
        Me.GroupBox4.TabIndex = 3
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Date et Heure"
        Me.GroupBox4.Visible = False
        '
        'vTime
        '
        Me.vTime.Location = New System.Drawing.Point(235, 29)
        Me.vTime.Name = "vTime"
        Me.vTime.Size = New System.Drawing.Size(91, 20)
        Me.vTime.TabIndex = 5
        Me.vTime.Text = "DomainUpDown1"
        '
        'BtnCalOk
        '
        Me.BtnCalOk.Location = New System.Drawing.Point(334, 185)
        Me.BtnCalOk.Name = "BtnCalOk"
        Me.BtnCalOk.Size = New System.Drawing.Size(101, 25)
        Me.BtnCalOk.TabIndex = 4
        Me.BtnCalOk.Text = "Ok"
        Me.BtnCalOk.UseVisualStyleBackColor = True
        '
        'BtnCalAnnuler
        '
        Me.BtnCalAnnuler.Location = New System.Drawing.Point(437, 185)
        Me.BtnCalAnnuler.Name = "BtnCalAnnuler"
        Me.BtnCalAnnuler.Size = New System.Drawing.Size(101, 25)
        Me.BtnCalAnnuler.TabIndex = 3
        Me.BtnCalAnnuler.Text = "Annuler"
        Me.BtnCalAnnuler.UseVisualStyleBackColor = True
        '
        'LblCalendarTime
        '
        Me.LblCalendarTime.AutoSize = True
        Me.LblCalendarTime.ForeColor = System.Drawing.Color.White
        Me.LblCalendarTime.Location = New System.Drawing.Point(232, 62)
        Me.LblCalendarTime.Name = "LblCalendarTime"
        Me.LblCalendarTime.Size = New System.Drawing.Size(42, 13)
        Me.LblCalendarTime.TabIndex = 2
        Me.LblCalendarTime.Text = "#####"
        '
        'MonthCalendar1
        '
        Me.MonthCalendar1.Location = New System.Drawing.Point(13, 28)
        Me.MonthCalendar1.Name = "MonthCalendar1"
        Me.MonthCalendar1.TabIndex = 0
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.BtnDown)
        Me.GroupBox5.Controls.Add(Me.BtnUp)
        Me.GroupBox5.Controls.Add(Me.BtnDelAll)
        Me.GroupBox5.Controls.Add(Me.BtnUnSelect)
        Me.GroupBox5.Controls.Add(Me.BtnSelect)
        Me.GroupBox5.Controls.Add(Me.ListBoxScriptDispo)
        Me.GroupBox5.Controls.Add(Me.ListBoxScriptSelect)
        Me.GroupBox5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox5.ForeColor = System.Drawing.Color.Orange
        Me.GroupBox5.Location = New System.Drawing.Point(8, 453)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(592, 187)
        Me.GroupBox5.TabIndex = 4
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Script Details"
        '
        'BtnDown
        '
        Me.BtnDown.ForeColor = System.Drawing.Color.Black
        Me.BtnDown.Location = New System.Drawing.Point(483, 80)
        Me.BtnDown.Name = "BtnDown"
        Me.BtnDown.Size = New System.Drawing.Size(85, 28)
        Me.BtnDown.TabIndex = 6
        Me.BtnDown.Text = "Descendre"
        Me.BtnDown.UseVisualStyleBackColor = True
        '
        'BtnUp
        '
        Me.BtnUp.ForeColor = System.Drawing.Color.Black
        Me.BtnUp.Location = New System.Drawing.Point(483, 50)
        Me.BtnUp.Name = "BtnUp"
        Me.BtnUp.Size = New System.Drawing.Size(85, 28)
        Me.BtnUp.TabIndex = 5
        Me.BtnUp.Text = "Monter"
        Me.BtnUp.UseVisualStyleBackColor = True
        '
        'BtnDelAll
        '
        Me.BtnDelAll.ForeColor = System.Drawing.Color.Black
        Me.BtnDelAll.Location = New System.Drawing.Point(483, 20)
        Me.BtnDelAll.Name = "BtnDelAll"
        Me.BtnDelAll.Size = New System.Drawing.Size(85, 28)
        Me.BtnDelAll.TabIndex = 4
        Me.BtnDelAll.Text = "Effacer Tous"
        Me.BtnDelAll.UseVisualStyleBackColor = True
        '
        'BtnUnSelect
        '
        Me.BtnUnSelect.ForeColor = System.Drawing.Color.Black
        Me.BtnUnSelect.Location = New System.Drawing.Point(483, 110)
        Me.BtnUnSelect.Name = "BtnUnSelect"
        Me.BtnUnSelect.Size = New System.Drawing.Size(85, 28)
        Me.BtnUnSelect.TabIndex = 3
        Me.BtnUnSelect.Text = "Supprimer"
        Me.BtnUnSelect.UseVisualStyleBackColor = True
        '
        'BtnSelect
        '
        Me.BtnSelect.Location = New System.Drawing.Point(237, 20)
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
        Me.ListBoxScriptDispo.Location = New System.Drawing.Point(19, 20)
        Me.ListBoxScriptDispo.Name = "ListBoxScriptDispo"
        Me.ListBoxScriptDispo.Size = New System.Drawing.Size(210, 160)
        Me.ListBoxScriptDispo.TabIndex = 1
        '
        'ListBoxScriptSelect
        '
        Me.ListBoxScriptSelect.BackColor = System.Drawing.Color.DarkGray
        Me.ListBoxScriptSelect.ForeColor = System.Drawing.Color.White
        Me.ListBoxScriptSelect.FormattingEnabled = True
        Me.ListBoxScriptSelect.Location = New System.Drawing.Point(268, 20)
        Me.ListBoxScriptSelect.Name = "ListBoxScriptSelect"
        Me.ListBoxScriptSelect.Size = New System.Drawing.Size(200, 160)
        Me.ListBoxScriptSelect.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Lucida Sans Unicode", 20.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Orange
        Me.Label2.Location = New System.Drawing.Point(252, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(141, 34)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Schedule"
        '
        'FrmSchedule
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Gray
        Me.ClientSize = New System.Drawing.Size(613, 652)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FrmSchedule"
        Me.Text = "Schedule"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.PanelOffset.ResumeLayout(False)
        Me.PanelOffset.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.PanelRJ.ResumeLayout(False)
        Me.PanelRJ.PerformLayout()
        Me.PanelRC.ResumeLayout(False)
        Me.PanelRC.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents cEnable As System.Windows.Forms.CheckBox
    Friend WithEvents TxtNom As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents cTgType As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents PanelRC As System.Windows.Forms.Panel
    Friend WithEvents RC3 As System.Windows.Forms.RadioButton
    Friend WithEvents RC2 As System.Windows.Forms.RadioButton
    Friend WithEvents RC1 As System.Windows.Forms.RadioButton
    Friend WithEvents RC0 As System.Windows.Forms.RadioButton
    Friend WithEvents PanelRJ As System.Windows.Forms.Panel
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents RE0 As System.Windows.Forms.RadioButton
    Friend WithEvents BtnCalStart As System.Windows.Forms.Button
    Friend WithEvents LblTimeStart As System.Windows.Forms.Label
    Friend WithEvents LblTimeEnd As System.Windows.Forms.Label
    Friend WithEvents RE1 As System.Windows.Forms.RadioButton
    Friend WithEvents BtnCalEnd As System.Windows.Forms.Button
    Friend WithEvents PanelOffset As System.Windows.Forms.Panel
    Friend WithEvents R1 As System.Windows.Forms.RadioButton
    Friend WithEvents R0 As System.Windows.Forms.RadioButton
    Friend WithEvents OffSetTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents LblOffset As System.Windows.Forms.Label
    Friend WithEvents BtnAnnuler As System.Windows.Forms.Button
    Friend WithEvents BtnOK As System.Windows.Forms.Button
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents BtnCalOk As System.Windows.Forms.Button
    Friend WithEvents BtnCalAnnuler As System.Windows.Forms.Button
    Friend WithEvents LblCalendarTime As System.Windows.Forms.Label
    Friend WithEvents MonthCalendar1 As System.Windows.Forms.MonthCalendar
    Friend WithEvents CJ6 As System.Windows.Forms.CheckBox
    Friend WithEvents CJ5 As System.Windows.Forms.CheckBox
    Friend WithEvents CJ4 As System.Windows.Forms.CheckBox
    Friend WithEvents CJ3 As System.Windows.Forms.CheckBox
    Friend WithEvents CJ2 As System.Windows.Forms.CheckBox
    Friend WithEvents CJ1 As System.Windows.Forms.CheckBox
    Friend WithEvents CJ0 As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents BtnDown As System.Windows.Forms.Button
    Friend WithEvents BtnUp As System.Windows.Forms.Button
    Friend WithEvents BtnDelAll As System.Windows.Forms.Button
    Friend WithEvents BtnUnSelect As System.Windows.Forms.Button
    Friend WithEvents BtnSelect As System.Windows.Forms.Button
    Friend WithEvents ListBoxScriptDispo As System.Windows.Forms.ListBox
    Friend WithEvents ListBoxScriptSelect As System.Windows.Forms.ListBox
    Friend WithEvents vTime As System.Windows.Forms.DomainUpDown
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
