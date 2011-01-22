<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CtrlCondition
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CtrlCondition))
        Me.Label1 = New System.Windows.Forms.Label
        Me.CMode = New System.Windows.Forms.ComboBox
        Me.CItem = New System.Windows.Forms.ComboBox
        Me.CProperty = New System.Windows.Forms.ComboBox
        Me.COpe = New System.Windows.Forms.ComboBox
        Me.CValue = New System.Windows.Forms.ComboBox
        Me.BtnDown = New System.Windows.Forms.Button
        Me.BtnUp = New System.Windows.Forms.Button
        Me.BtnDel = New System.Windows.Forms.PictureBox
        CType(Me.BtnDel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(2, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(37, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Mode:"
        '
        'CMode
        '
        Me.CMode.FormattingEnabled = True
        Me.CMode.Items.AddRange(New Object() {"TEMPS", "DEVICE"})
        Me.CMode.Location = New System.Drawing.Point(45, 8)
        Me.CMode.Name = "CMode"
        Me.CMode.Size = New System.Drawing.Size(66, 21)
        Me.CMode.TabIndex = 7
        '
        'CItem
        '
        Me.CItem.AllowDrop = True
        Me.CItem.FormattingEnabled = True
        Me.CItem.Location = New System.Drawing.Point(117, 8)
        Me.CItem.Name = "CItem"
        Me.CItem.Size = New System.Drawing.Size(107, 21)
        Me.CItem.TabIndex = 8
        '
        'CProperty
        '
        Me.CProperty.FormattingEnabled = True
        Me.CProperty.Location = New System.Drawing.Point(230, 8)
        Me.CProperty.Name = "CProperty"
        Me.CProperty.Size = New System.Drawing.Size(101, 21)
        Me.CProperty.TabIndex = 9
        '
        'COpe
        '
        Me.COpe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.COpe.FormattingEnabled = True
        Me.COpe.Items.AddRange(New Object() {"=", ">", ">=", "<", "<=", "<>"})
        Me.COpe.Location = New System.Drawing.Point(337, 8)
        Me.COpe.Name = "COpe"
        Me.COpe.Size = New System.Drawing.Size(45, 21)
        Me.COpe.TabIndex = 10
        '
        'CValue
        '
        Me.CValue.FormattingEnabled = True
        Me.CValue.Location = New System.Drawing.Point(388, 8)
        Me.CValue.Name = "CValue"
        Me.CValue.Size = New System.Drawing.Size(112, 21)
        Me.CValue.TabIndex = 13
        '
        'BtnDown
        '
        Me.BtnDown.Image = CType(resources.GetObject("BtnDown.Image"), System.Drawing.Image)
        Me.BtnDown.Location = New System.Drawing.Point(534, 9)
        Me.BtnDown.Name = "BtnDown"
        Me.BtnDown.Size = New System.Drawing.Size(25, 25)
        Me.BtnDown.TabIndex = 14
        Me.BtnDown.UseVisualStyleBackColor = True
        '
        'BtnUp
        '
        Me.BtnUp.Image = CType(resources.GetObject("BtnUp.Image"), System.Drawing.Image)
        Me.BtnUp.Location = New System.Drawing.Point(561, 9)
        Me.BtnUp.Name = "BtnUp"
        Me.BtnUp.Size = New System.Drawing.Size(25, 25)
        Me.BtnUp.TabIndex = 15
        Me.BtnUp.UseVisualStyleBackColor = True
        '
        'BtnDel
        '
        Me.BtnDel.Image = CType(resources.GetObject("BtnDel.Image"), System.Drawing.Image)
        Me.BtnDel.Location = New System.Drawing.Point(588, 9)
        Me.BtnDel.Name = "BtnDel"
        Me.BtnDel.Size = New System.Drawing.Size(25, 25)
        Me.BtnDel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnDel.TabIndex = 16
        Me.BtnDel.TabStop = False
        '
        'CtrlCondition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.BtnDel)
        Me.Controls.Add(Me.BtnUp)
        Me.Controls.Add(Me.BtnDown)
        Me.Controls.Add(Me.CValue)
        Me.Controls.Add(Me.COpe)
        Me.Controls.Add(Me.CProperty)
        Me.Controls.Add(Me.CItem)
        Me.Controls.Add(Me.CMode)
        Me.Controls.Add(Me.Label1)
        Me.Name = "CtrlCondition"
        Me.Size = New System.Drawing.Size(625, 42)
        CType(Me.BtnDel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents CMode As System.Windows.Forms.ComboBox
    Public WithEvents CItem As System.Windows.Forms.ComboBox
    Public WithEvents CProperty As System.Windows.Forms.ComboBox
    Public WithEvents COpe As System.Windows.Forms.ComboBox
    Public WithEvents CValue As System.Windows.Forms.ComboBox
    Friend WithEvents BtnDown As System.Windows.Forms.Button
    Friend WithEvents BtnUp As System.Windows.Forms.Button
    Friend WithEvents BtnDel As System.Windows.Forms.PictureBox

End Class
