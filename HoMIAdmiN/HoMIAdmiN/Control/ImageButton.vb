Public Class ImageButton
    Inherits Image

    Dim _Row As Integer = -1
    Dim _Column As Integer = -1
    Dim _Commande As String

    Public Event Delete(ByVal Sender As Object)

    Public Property Row As Integer
        Get
            Return _Row
        End Get
        Set(ByVal value As Integer)
            _Row = value
        End Set
    End Property

    Public Property Column As Integer
        Get
            Return _Column
        End Get
        Set(ByVal value As Integer)
            _Column = value
        End Set
    End Property

    Public Property Command As String
        Get
            Return _Commande
        End Get
        Set(ByVal value As String)
            _Commande = value
            Me.ToolTip = _Commande
        End Set
    End Property

    Public Sub New()
        Try
            Dim MyContextMenu As New ContextMenu

            Dim MyContextMenuFille As New MenuItem
            MyContextMenuFille.Header = "Supprimer"
            AddHandler MyContextMenuFille.Click, AddressOf _Delete

            MyContextMenu.Items.Add(MyContextMenuFille)
            Me.ContextMenu = MyContextMenu
        Catch ex As Exception
            MessageBox.Show("Erreur ImageButton New: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub _Delete()
        RaiseEvent Delete(Me)
    End Sub
End Class
