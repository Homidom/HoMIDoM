<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CtrlHisto
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CtrlHisto))
        Me.cEnable = New System.Windows.Forms.CheckBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.CbDevice = New System.Windows.Forms.ComboBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.CbProperty = New System.Windows.Forms.ComboBox
        Me.BtnDelete = New System.Windows.Forms.PictureBox
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cEnable
        '
        Me.cEnable.AutoSize = True
        Me.cEnable.ForeColor = System.Drawing.Color.White
        Me.cEnable.Location = New System.Drawing.Point(3, 12)
        Me.cEnable.Name = "cEnable"
        Me.cEnable.Size = New System.Drawing.Size(59, 17)
        Me.cEnable.TabIndex = 0
        Me.cEnable.Text = "Enable"
        Me.cEnable.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(68, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(44, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Device:"
        '
        'CbDevice
        '
        Me.CbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbDevice.FormattingEnabled = True
        Me.CbDevice.Location = New System.Drawing.Point(109, 11)
        Me.CbDevice.Name = "CbDevice"
        Me.CbDevice.Size = New System.Drawing.Size(121, 21)
        Me.CbDevice.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(232, 13)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(49, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Property:"
        '
        'CbProperty
        '
        Me.CbProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbProperty.FormattingEnabled = True
        Me.CbProperty.Location = New System.Drawing.Point(283, 10)
        Me.CbProperty.Name = "CbProperty"
        Me.CbProperty.Size = New System.Drawing.Size(121, 21)
        Me.CbProperty.TabIndex = 4
        '
        'BtnDelete
        '
        Me.BtnDelete.Image = CType(resources.GetObject("BtnDelete.Image"), System.Drawing.Image)
        Me.BtnDelete.Location = New System.Drawing.Point(431, 10)
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.Size = New System.Drawing.Size(25, 25)
        Me.BtnDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnDelete.TabIndex = 5
        Me.BtnDelete.TabStop = False
        '
        'CtrlHisto
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DarkGray
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.CbProperty)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.CbDevice)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cEnable)
        Me.Name = "CtrlHisto"
        Me.Size = New System.Drawing.Size(470, 47)
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cEnable As System.Windows.Forms.CheckBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CbDevice As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents CbProperty As System.Windows.Forms.ComboBox
    Friend WithEvents BtnDelete As System.Windows.Forms.PictureBox

End Class
