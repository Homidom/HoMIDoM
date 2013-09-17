<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LogsForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LogsForm))
        Me.ErrorsTextBox = New System.Windows.Forms.TextBox()
        Me.LogsTextBox = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'ErrorsTextBox
        '
        Me.ErrorsTextBox.BackColor = System.Drawing.SystemColors.GrayText
        Me.ErrorsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ErrorsTextBox.Location = New System.Drawing.Point(3, 211)
        Me.ErrorsTextBox.Multiline = True
        Me.ErrorsTextBox.Name = "ErrorsTextBox"
        Me.ErrorsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal
        Me.ErrorsTextBox.Size = New System.Drawing.Size(737, 160)
        Me.ErrorsTextBox.TabIndex = 0
        Me.ErrorsTextBox.Text = "Pas d'erreurs"
        Me.ErrorsTextBox.WordWrap = False
        '
        'LogsTextBox
        '
        Me.LogsTextBox.BackColor = System.Drawing.SystemColors.GrayText
        Me.LogsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LogsTextBox.Location = New System.Drawing.Point(3, 22)
        Me.LogsTextBox.Multiline = True
        Me.LogsTextBox.Name = "LogsTextBox"
        Me.LogsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal
        Me.LogsTextBox.Size = New System.Drawing.Size(737, 160)
        Me.LogsTextBox.TabIndex = 0
        Me.LogsTextBox.Text = "Pas de logs"
        Me.LogsTextBox.WordWrap = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(3, 7)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Derniers Logs"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(3, 195)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(105, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Dernières Erreurs"
        '
        'LogsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(99, Byte), Integer), CType(CType(96, Byte), Integer), CType(CType(96, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(742, 376)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.LogsTextBox)
        Me.Controls.Add(Me.ErrorsTextBox)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "LogsForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "HoMIGuI - Logs"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ErrorsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents LogsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
