Namespace HoMIDom


    ''' <summary>
    ''' Permet de regrouper un ensemble de devices du même type
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Groupes
        Dim _Id As String
        Dim _Nom As String
        Dim _ListDevice As New List(Of String)
        Dim _Picture As String
        Dim _Description As String
        Dim _DeviceAction As New List(Of DeviceAction)
        <NonSerialized()> Dim _Server As Server

        Public Property ID As String
            Get
                Return _Id
            End Get
            Set(ByVal value As String)
                _Id = value
            End Set
        End Property

        Public Property Nom As String
            Get
                Return _Nom
            End Get
            Set(ByVal value As String)
                _Nom = value
            End Set
        End Property

        Public Property ListDevice As List(Of String)
            Get
                Return _ListDevice
            End Get
            Set(ByVal value As List(Of String))
                _ListDevice = value
            End Set
        End Property

        Public Property Picture As String
            Get
                Return _Picture
            End Get
            Set(ByVal value As String)
                _Picture = value
            End Set
        End Property

        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        Sub New(ByRef Server As Server)
            _Server = Server
        End Sub

        ''' <summary>
        ''' List des actions associées au Device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DeviceAction() As List(Of DeviceAction)
            Get
                Try
                    If _ListDevice IsNot Nothing Then
                        Dim _listact As New List(Of String)
                        _listact = _Server.ListMethod(_ListDevice.Item(0))
                        _listact = _listact
                        If _listact.Count > 0 Then
                            For n As Integer = 0 To _listact.Count - 1
                                Dim a() As String = _listact.Item(n).Split("|")
                                Dim p As New DeviceAction
                                With p
                                    .Nom = a(0)
                                    If a.Length > 1 Then
                                        For t As Integer = 1 To a.Length - 1
                                            Dim pr As New DeviceAction.Parametre
                                            Dim b() As String = a(t).Split(":")
                                            With pr
                                                .Nom = b(0)
                                                .Type = b(1)
                                            End With
                                            p.Parametres.Add(pr)
                                        Next
                                    End If
                                End With
                                _DeviceAction.Add(p)
                            Next
                        End If
                    End If
                    Return _DeviceAction
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Groupes:DeviceAction", "Exception : " & ex.ToString)
                    Return Nothing
                End Try
            End Get
        End Property
    End Class
End Namespace