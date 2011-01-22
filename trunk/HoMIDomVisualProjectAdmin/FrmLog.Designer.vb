<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmLog
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
        Me.TxtLog = New System.Windows.Forms.RichTextBox
        Me.BtnClose = New System.Windows.Forms.Button
        Me.BtnRefresh = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.TxtSize = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'TxtLog
        '
        Me.TxtLog.Location = New System.Drawing.Point(10, 39)
        Me.TxtLog.Name = "TxtLog"
        Me.TxtLog.Size = New System.Drawing.Size(530, 359)
        Me.TxtLog.TabIndex = 0
        Me.TxtLog.Text = ""
        '
        'BtnClose
        '
        Me.BtnClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.Location = New System.Drawing.Point(459, 6)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(81, 27)
        Me.BtnClose.TabIndex = 1
        Me.BtnClose.Text = "Quitter"
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'BtnRefresh
        '
        Me.BtnRefresh.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnRefresh.Location = New System.Drawing.Point(372, 6)
        Me.BtnRefresh.Name = "BtnRefresh"
        Me.BtnRefresh.Size = New System.Drawing.Size(81, 27)
        Me.BtnRefresh.TabIndex = 2
        Me.BtnRefresh.Text = "Rafraichir"
        Me.BtnRefresh.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(15, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(155, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Taille Max du fichier log (bytes):"
        '
        'TxtSize
        '
        Me.TxtSize.Location = New System.Drawing.Point(177, 7)
        Me.TxtSize.Name = "TxtSize"
        Me.TxtSize.Size = New System.Drawing.Size(168, 20)
        Me.TxtSize.TabIndex = 4
        '
        'FrmLog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.ClientSize = New System.Drawing.Size(550, 410)
        Me.Controls.Add(Me.TxtSize)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnRefresh)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.TxtLog)
        Me.Name = "FrmLog"
        Me.Text = "Log"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TxtLog As System.Windows.Forms.RichTextBox
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents BtnRefresh As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TxtSize As System.Windows.Forms.TextBox
End Class
