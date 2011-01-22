<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CtrlIf
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CtrlIf))
        Me.BtnUp = New System.Windows.Forms.Button
        Me.BtnDown = New System.Windows.Forms.Button
        Me.Lbl = New System.Windows.Forms.Label
        Me.BtnExpand = New System.Windows.Forms.Label
        Me.LblElse = New System.Windows.Forms.Label
        Me.TLPIf = New System.Windows.Forms.TableLayoutPanel
        Me.Label1 = New System.Windows.Forms.Label
        Me.TLPThen = New System.Windows.Forms.TableLayoutPanel
        Me.TLPElse = New System.Windows.Forms.TableLayoutPanel
        Me.BtnNewCondition = New System.Windows.Forms.Button
        Me.BtnNewActionSpeek_T = New System.Windows.Forms.PictureBox
        Me.BtnNewActionMail_T = New System.Windows.Forms.PictureBox
        Me.BtnNewActionExit_T = New System.Windows.Forms.PictureBox
        Me.BtnNewActionPause_T = New System.Windows.Forms.PictureBox
        Me.BtnNewActionDevice_T = New System.Windows.Forms.PictureBox
        Me.BtnNewActionSpeek_E = New System.Windows.Forms.PictureBox
        Me.BtnNewActionMail_E = New System.Windows.Forms.PictureBox
        Me.BtnNewActionExit_E = New System.Windows.Forms.PictureBox
        Me.BtnNewActionPause_E = New System.Windows.Forms.PictureBox
        Me.BtnNewActionDevice_E = New System.Windows.Forms.PictureBox
        Me.BtnDelete = New System.Windows.Forms.PictureBox
        CType(Me.BtnNewActionSpeek_T, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionMail_T, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionExit_T, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionPause_T, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionDevice_T, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionSpeek_E, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionMail_E, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionExit_E, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionPause_E, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewActionDevice_E, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnUp
        '
        Me.BtnUp.Image = CType(resources.GetObject("BtnUp.Image"), System.Drawing.Image)
        Me.BtnUp.Location = New System.Drawing.Point(534, 9)
        Me.BtnUp.Name = "BtnUp"
        Me.BtnUp.Size = New System.Drawing.Size(25, 25)
        Me.BtnUp.TabIndex = 4
        Me.BtnUp.UseVisualStyleBackColor = True
        '
        'BtnDown
        '
        Me.BtnDown.Image = CType(resources.GetObject("BtnDown.Image"), System.Drawing.Image)
        Me.BtnDown.Location = New System.Drawing.Point(561, 9)
        Me.BtnDown.Name = "BtnDown"
        Me.BtnDown.Size = New System.Drawing.Size(25, 25)
        Me.BtnDown.TabIndex = 3
        Me.BtnDown.UseVisualStyleBackColor = True
        '
        'Lbl
        '
        Me.Lbl.AutoSize = True
        Me.Lbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Lbl.ForeColor = System.Drawing.Color.White
        Me.Lbl.Location = New System.Drawing.Point(31, 12)
        Me.Lbl.Name = "Lbl"
        Me.Lbl.Size = New System.Drawing.Size(14, 16)
        Me.Lbl.TabIndex = 1
        Me.Lbl.Text = "If"
        '
        'BtnExpand
        '
        Me.BtnExpand.AutoSize = True
        Me.BtnExpand.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnExpand.ForeColor = System.Drawing.Color.White
        Me.BtnExpand.Location = New System.Drawing.Point(4, 7)
        Me.BtnExpand.Name = "BtnExpand"
        Me.BtnExpand.Size = New System.Drawing.Size(21, 24)
        Me.BtnExpand.TabIndex = 6
        Me.BtnExpand.Text = "+"
        '
        'LblElse
        '
        Me.LblElse.AutoSize = True
        Me.LblElse.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblElse.ForeColor = System.Drawing.Color.White
        Me.LblElse.Location = New System.Drawing.Point(3, 184)
        Me.LblElse.Name = "LblElse"
        Me.LblElse.Size = New System.Drawing.Size(39, 16)
        Me.LblElse.TabIndex = 11
        Me.LblElse.Text = "Then"
        '
        'TLPIf
        '
        Me.TLPIf.AutoScroll = True
        Me.TLPIf.BackColor = System.Drawing.Color.DarkGray
        Me.TLPIf.ColumnCount = 1
        Me.TLPIf.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPIf.Location = New System.Drawing.Point(4, 37)
        Me.TLPIf.Name = "TLPIf"
        Me.TLPIf.RowCount = 1
        Me.TLPIf.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TLPIf.Size = New System.Drawing.Size(718, 147)
        Me.TLPIf.TabIndex = 12
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(3, 375)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 16)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Else"
        '
        'TLPThen
        '
        Me.TLPThen.AutoScroll = True
        Me.TLPThen.BackColor = System.Drawing.Color.DarkGray
        Me.TLPThen.ColumnCount = 1
        Me.TLPThen.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPThen.Location = New System.Drawing.Point(4, 220)
        Me.TLPThen.Name = "TLPThen"
        Me.TLPThen.RowCount = 1
        Me.TLPThen.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TLPThen.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152.0!))
        Me.TLPThen.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152.0!))
        Me.TLPThen.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152.0!))
        Me.TLPThen.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152.0!))
        Me.TLPThen.Size = New System.Drawing.Size(718, 152)
        Me.TLPThen.TabIndex = 13
        '
        'TLPElse
        '
        Me.TLPElse.AutoScroll = True
        Me.TLPElse.BackColor = System.Drawing.Color.DarkGray
        Me.TLPElse.ColumnCount = 1
        Me.TLPElse.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPElse.Location = New System.Drawing.Point(4, 403)
        Me.TLPElse.Name = "TLPElse"
        Me.TLPElse.RowCount = 1
        Me.TLPElse.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TLPElse.Size = New System.Drawing.Size(718, 152)
        Me.TLPElse.TabIndex = 13
        '
        'BtnNewCondition
        '
        Me.BtnNewCondition.Location = New System.Drawing.Point(67, 3)
        Me.BtnNewCondition.Name = "BtnNewCondition"
        Me.BtnNewCondition.Size = New System.Drawing.Size(115, 28)
        Me.BtnNewCondition.TabIndex = 14
        Me.BtnNewCondition.Text = "Nouvelle Condition"
        Me.BtnNewCondition.UseVisualStyleBackColor = True
        '
        'BtnNewActionSpeek_T
        '
        Me.BtnNewActionSpeek_T.Image = CType(resources.GetObject("BtnNewActionSpeek_T.Image"), System.Drawing.Image)
        Me.BtnNewActionSpeek_T.Location = New System.Drawing.Point(205, 184)
        Me.BtnNewActionSpeek_T.Name = "BtnNewActionSpeek_T"
        Me.BtnNewActionSpeek_T.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionSpeek_T.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionSpeek_T.TabIndex = 31
        Me.BtnNewActionSpeek_T.TabStop = False
        '
        'BtnNewActionMail_T
        '
        Me.BtnNewActionMail_T.Image = CType(resources.GetObject("BtnNewActionMail_T.Image"), System.Drawing.Image)
        Me.BtnNewActionMail_T.Location = New System.Drawing.Point(168, 184)
        Me.BtnNewActionMail_T.Name = "BtnNewActionMail_T"
        Me.BtnNewActionMail_T.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionMail_T.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionMail_T.TabIndex = 30
        Me.BtnNewActionMail_T.TabStop = False
        '
        'BtnNewActionExit_T
        '
        Me.BtnNewActionExit_T.Image = CType(resources.GetObject("BtnNewActionExit_T.Image"), System.Drawing.Image)
        Me.BtnNewActionExit_T.Location = New System.Drawing.Point(131, 184)
        Me.BtnNewActionExit_T.Name = "BtnNewActionExit_T"
        Me.BtnNewActionExit_T.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionExit_T.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionExit_T.TabIndex = 29
        Me.BtnNewActionExit_T.TabStop = False
        '
        'BtnNewActionPause_T
        '
        Me.BtnNewActionPause_T.Image = CType(resources.GetObject("BtnNewActionPause_T.Image"), System.Drawing.Image)
        Me.BtnNewActionPause_T.Location = New System.Drawing.Point(94, 184)
        Me.BtnNewActionPause_T.Name = "BtnNewActionPause_T"
        Me.BtnNewActionPause_T.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionPause_T.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionPause_T.TabIndex = 28
        Me.BtnNewActionPause_T.TabStop = False
        '
        'BtnNewActionDevice_T
        '
        Me.BtnNewActionDevice_T.Image = CType(resources.GetObject("BtnNewActionDevice_T.Image"), System.Drawing.Image)
        Me.BtnNewActionDevice_T.Location = New System.Drawing.Point(57, 184)
        Me.BtnNewActionDevice_T.Name = "BtnNewActionDevice_T"
        Me.BtnNewActionDevice_T.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionDevice_T.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionDevice_T.TabIndex = 27
        Me.BtnNewActionDevice_T.TabStop = False
        '
        'BtnNewActionSpeek_E
        '
        Me.BtnNewActionSpeek_E.Image = CType(resources.GetObject("BtnNewActionSpeek_E.Image"), System.Drawing.Image)
        Me.BtnNewActionSpeek_E.Location = New System.Drawing.Point(205, 373)
        Me.BtnNewActionSpeek_E.Name = "BtnNewActionSpeek_E"
        Me.BtnNewActionSpeek_E.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionSpeek_E.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionSpeek_E.TabIndex = 36
        Me.BtnNewActionSpeek_E.TabStop = False
        '
        'BtnNewActionMail_E
        '
        Me.BtnNewActionMail_E.Image = CType(resources.GetObject("BtnNewActionMail_E.Image"), System.Drawing.Image)
        Me.BtnNewActionMail_E.Location = New System.Drawing.Point(168, 373)
        Me.BtnNewActionMail_E.Name = "BtnNewActionMail_E"
        Me.BtnNewActionMail_E.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionMail_E.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionMail_E.TabIndex = 35
        Me.BtnNewActionMail_E.TabStop = False
        '
        'BtnNewActionExit_E
        '
        Me.BtnNewActionExit_E.Image = CType(resources.GetObject("BtnNewActionExit_E.Image"), System.Drawing.Image)
        Me.BtnNewActionExit_E.Location = New System.Drawing.Point(131, 373)
        Me.BtnNewActionExit_E.Name = "BtnNewActionExit_E"
        Me.BtnNewActionExit_E.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionExit_E.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionExit_E.TabIndex = 34
        Me.BtnNewActionExit_E.TabStop = False
        '
        'BtnNewActionPause_E
        '
        Me.BtnNewActionPause_E.Image = CType(resources.GetObject("BtnNewActionPause_E.Image"), System.Drawing.Image)
        Me.BtnNewActionPause_E.Location = New System.Drawing.Point(94, 373)
        Me.BtnNewActionPause_E.Name = "BtnNewActionPause_E"
        Me.BtnNewActionPause_E.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionPause_E.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionPause_E.TabIndex = 33
        Me.BtnNewActionPause_E.TabStop = False
        '
        'BtnNewActionDevice_E
        '
        Me.BtnNewActionDevice_E.Image = CType(resources.GetObject("BtnNewActionDevice_E.Image"), System.Drawing.Image)
        Me.BtnNewActionDevice_E.Location = New System.Drawing.Point(57, 373)
        Me.BtnNewActionDevice_E.Name = "BtnNewActionDevice_E"
        Me.BtnNewActionDevice_E.Size = New System.Drawing.Size(30, 30)
        Me.BtnNewActionDevice_E.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnNewActionDevice_E.TabIndex = 32
        Me.BtnNewActionDevice_E.TabStop = False
        '
        'BtnDelete
        '
        Me.BtnDelete.Image = CType(resources.GetObject("BtnDelete.Image"), System.Drawing.Image)
        Me.BtnDelete.Location = New System.Drawing.Point(588, 9)
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.Size = New System.Drawing.Size(25, 25)
        Me.BtnDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnDelete.TabIndex = 37
        Me.BtnDelete.TabStop = False
        '
        'CtrlIf
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.BtnNewActionSpeek_E)
        Me.Controls.Add(Me.BtnNewActionMail_E)
        Me.Controls.Add(Me.BtnNewActionExit_E)
        Me.Controls.Add(Me.BtnNewActionPause_E)
        Me.Controls.Add(Me.BtnNewActionDevice_E)
        Me.Controls.Add(Me.BtnNewActionSpeek_T)
        Me.Controls.Add(Me.BtnNewActionMail_T)
        Me.Controls.Add(Me.BtnNewActionExit_T)
        Me.Controls.Add(Me.BtnNewActionPause_T)
        Me.Controls.Add(Me.BtnNewActionDevice_T)
        Me.Controls.Add(Me.BtnNewCondition)
        Me.Controls.Add(Me.TLPElse)
        Me.Controls.Add(Me.TLPThen)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TLPIf)
        Me.Controls.Add(Me.LblElse)
        Me.Controls.Add(Me.BtnExpand)
        Me.Controls.Add(Me.BtnUp)
        Me.Controls.Add(Me.BtnDown)
        Me.Controls.Add(Me.Lbl)
        Me.Name = "CtrlIf"
        Me.Size = New System.Drawing.Size(733, 42)
        CType(Me.BtnNewActionSpeek_T, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionMail_T, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionExit_T, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionPause_T, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionDevice_T, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionSpeek_E, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionMail_E, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionExit_E, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionPause_E, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewActionDevice_E, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnUp As System.Windows.Forms.Button
    Friend WithEvents BtnDown As System.Windows.Forms.Button
    Friend WithEvents Lbl As System.Windows.Forms.Label
    Friend WithEvents BtnExpand As System.Windows.Forms.Label
    Friend WithEvents LblElse As System.Windows.Forms.Label
    Friend WithEvents TLPIf As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TLPThen As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TLPElse As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents BtnNewCondition As System.Windows.Forms.Button
    Friend WithEvents BtnNewActionSpeek_T As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionMail_T As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionExit_T As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionPause_T As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionDevice_T As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionSpeek_E As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionMail_E As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionExit_E As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionPause_E As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewActionDevice_E As System.Windows.Forms.PictureBox
    Friend WithEvents BtnDelete As System.Windows.Forms.PictureBox

End Class
