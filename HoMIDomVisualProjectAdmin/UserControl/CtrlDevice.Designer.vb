<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CtrlDevice
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CtrlDevice))
        Me.LblDevice = New System.Windows.Forms.Label
        Me.cDevice = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.cFonction = New System.Windows.Forms.ComboBox
        Me.cValeur = New System.Windows.Forms.ComboBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.BtnDown = New System.Windows.Forms.Button
        Me.BtnUp = New System.Windows.Forms.Button
        Me.BtnExpand = New System.Windows.Forms.Label
        Me.BtnDelete = New System.Windows.Forms.PictureBox
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LblDevice
        '
        Me.LblDevice.AllowDrop = True
        Me.LblDevice.AutoSize = True
        Me.LblDevice.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblDevice.ForeColor = System.Drawing.Color.White
        Me.LblDevice.Location = New System.Drawing.Point(37, 12)
        Me.LblDevice.Name = "LblDevice"
        Me.LblDevice.Size = New System.Drawing.Size(44, 13)
        Me.LblDevice.TabIndex = 1
        Me.LblDevice.Text = "Device:"
        '
        'cDevice
        '
        Me.cDevice.BackColor = System.Drawing.Color.DimGray
        Me.cDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cDevice.ForeColor = System.Drawing.Color.White
        Me.cDevice.FormattingEnabled = True
        Me.cDevice.Location = New System.Drawing.Point(87, 9)
        Me.cDevice.Name = "cDevice"
        Me.cDevice.Size = New System.Drawing.Size(137, 21)
        Me.cDevice.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(230, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(58, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Parametre:"
        '
        'cFonction
        '
        Me.cFonction.BackColor = System.Drawing.Color.DimGray
        Me.cFonction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cFonction.FormattingEnabled = True
        Me.cFonction.Location = New System.Drawing.Point(294, 9)
        Me.cFonction.Name = "cFonction"
        Me.cFonction.Size = New System.Drawing.Size(117, 21)
        Me.cFonction.TabIndex = 7
        '
        'cValeur
        '
        Me.cValeur.BackColor = System.Drawing.Color.DimGray
        Me.cValeur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cValeur.FormattingEnabled = True
        Me.cValeur.Location = New System.Drawing.Point(275, 59)
        Me.cValeur.Name = "cValeur"
        Me.cValeur.Size = New System.Drawing.Size(137, 21)
        Me.cValeur.TabIndex = 9
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(229, 59)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(40, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Valeur:"
        '
        'BtnDown
        '
        Me.BtnDown.Image = CType(resources.GetObject("BtnDown.Image"), System.Drawing.Image)
        Me.BtnDown.Location = New System.Drawing.Point(561, 9)
        Me.BtnDown.Name = "BtnDown"
        Me.BtnDown.Size = New System.Drawing.Size(25, 25)
        Me.BtnDown.TabIndex = 12
        Me.BtnDown.UseVisualStyleBackColor = True
        '
        'BtnUp
        '
        Me.BtnUp.Image = CType(resources.GetObject("BtnUp.Image"), System.Drawing.Image)
        Me.BtnUp.Location = New System.Drawing.Point(534, 9)
        Me.BtnUp.Name = "BtnUp"
        Me.BtnUp.Size = New System.Drawing.Size(25, 25)
        Me.BtnUp.TabIndex = 13
        Me.BtnUp.UseVisualStyleBackColor = True
        '
        'BtnExpand
        '
        Me.BtnExpand.AutoSize = True
        Me.BtnExpand.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnExpand.ForeColor = System.Drawing.Color.White
        Me.BtnExpand.Location = New System.Drawing.Point(3, 6)
        Me.BtnExpand.Name = "BtnExpand"
        Me.BtnExpand.Size = New System.Drawing.Size(21, 24)
        Me.BtnExpand.TabIndex = 14
        Me.BtnExpand.Text = "+"
        '
        'BtnDelete
        '
        Me.BtnDelete.Image = CType(resources.GetObject("BtnDelete.Image"), System.Drawing.Image)
        Me.BtnDelete.Location = New System.Drawing.Point(588, 9)
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.Size = New System.Drawing.Size(25, 25)
        Me.BtnDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnDelete.TabIndex = 15
        Me.BtnDelete.TabStop = False
        '
        'CtrlDevice
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.BtnExpand)
        Me.Controls.Add(Me.BtnUp)
        Me.Controls.Add(Me.BtnDown)
        Me.Controls.Add(Me.cValeur)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cFonction)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cDevice)
        Me.Controls.Add(Me.LblDevice)
        Me.Name = "CtrlDevice"
        Me.Size = New System.Drawing.Size(625, 42)
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LblDevice As System.Windows.Forms.Label
    Friend WithEvents cDevice As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cFonction As System.Windows.Forms.ComboBox
    Friend WithEvents cValeur As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents BtnDown As System.Windows.Forms.Button
    Friend WithEvents BtnUp As System.Windows.Forms.Button
    Friend WithEvents BtnExpand As System.Windows.Forms.Label
    Friend WithEvents BtnDelete As System.Windows.Forms.PictureBox

End Class
