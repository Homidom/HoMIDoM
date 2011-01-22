<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CtrlPause
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CtrlPause))
        Me.Label = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.cHeure = New System.Windows.Forms.ComboBox
        Me.cMinute = New System.Windows.Forms.ComboBox
        Me.cSeconde = New System.Windows.Forms.ComboBox
        Me.cMSeconde = New System.Windows.Forms.ComboBox
        Me.BtnUp = New System.Windows.Forms.Button
        Me.BtnDown = New System.Windows.Forms.Button
        Me.BtnDelete = New System.Windows.Forms.PictureBox
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label
        '
        resources.ApplyResources(Me.Label, "Label")
        Me.Label.ForeColor = System.Drawing.Color.White
        Me.Label.Name = "Label"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Name = "Label2"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Name = "Label3"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Name = "Label4"
        '
        'cHeure
        '
        Me.cHeure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cHeure.FormattingEnabled = True
        resources.ApplyResources(Me.cHeure, "cHeure")
        Me.cHeure.Name = "cHeure"
        '
        'cMinute
        '
        Me.cMinute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cMinute.FormattingEnabled = True
        resources.ApplyResources(Me.cMinute, "cMinute")
        Me.cMinute.Name = "cMinute"
        '
        'cSeconde
        '
        Me.cSeconde.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cSeconde.FormattingEnabled = True
        resources.ApplyResources(Me.cSeconde, "cSeconde")
        Me.cSeconde.Name = "cSeconde"
        '
        'cMSeconde
        '
        Me.cMSeconde.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cMSeconde.FormattingEnabled = True
        resources.ApplyResources(Me.cMSeconde, "cMSeconde")
        Me.cMSeconde.Name = "cMSeconde"
        '
        'BtnUp
        '
        resources.ApplyResources(Me.BtnUp, "BtnUp")
        Me.BtnUp.Name = "BtnUp"
        Me.BtnUp.UseVisualStyleBackColor = True
        '
        'BtnDown
        '
        resources.ApplyResources(Me.BtnDown, "BtnDown")
        Me.BtnDown.Name = "BtnDown"
        Me.BtnDown.UseVisualStyleBackColor = True
        '
        'BtnDelete
        '
        resources.ApplyResources(Me.BtnDelete, "BtnDelete")
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.TabStop = False
        '
        'CtrlPause
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.BtnUp)
        Me.Controls.Add(Me.BtnDown)
        Me.Controls.Add(Me.cMSeconde)
        Me.Controls.Add(Me.cSeconde)
        Me.Controls.Add(Me.cMinute)
        Me.Controls.Add(Me.cHeure)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label)
        Me.Name = "CtrlPause"
        CType(Me.BtnDelete, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents cHeure As System.Windows.Forms.ComboBox
    Friend WithEvents cMinute As System.Windows.Forms.ComboBox
    Friend WithEvents cSeconde As System.Windows.Forms.ComboBox
    Friend WithEvents cMSeconde As System.Windows.Forms.ComboBox
    Friend WithEvents BtnUp As System.Windows.Forms.Button
    Friend WithEvents BtnDown As System.Windows.Forms.Button
    Friend WithEvents BtnDelete As System.Windows.Forms.PictureBox

End Class
