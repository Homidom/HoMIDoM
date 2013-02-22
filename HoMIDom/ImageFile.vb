Namespace HoMIDom

    <Serializable()> Public Class ImageFile
        Dim _Path As String = ""
        Dim _FileName As String = ""

        Public Property Path As String
            Get
                Return _Path
            End Get
            Set(ByVal value As String)
                _Path = value
            End Set
        End Property

        Public Property FileName As String
            Get
                Return _FileName
            End Get
            Set(ByVal value As String)
                _FileName = value
            End Set
        End Property
    End Class

End Namespace
