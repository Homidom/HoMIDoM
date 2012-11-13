Imports System
Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports System.Linq

Namespace HoMIDom

    <ServiceContract(Namespace:="http://HoMIDom/")> Public Interface IFileServer
        <OperationContract()> Function Download(ByVal Fichier As RequestFileData) As FileData

    End Interface

    <Serializable()> Public Class FileServer
        Implements IFileServer

        Public Function Download(ByVal Fichier As RequestFileData) As FileData Implements IFileServer.Download
            Try
                If System.IO.File.Exists(Fichier.Name) Then
                    Dim Stream As System.IO.FileStream = New System.IO.FileStream(Fichier.Name, IO.FileMode.Open)
                    Return New FileData(Stream)
                End If
                Return Nothing
            Catch ex As Exception

                Return Nothing
            End Try
        End Function

    End Class

    ''' <summary>
    ''' Classe de transfert de fichier en streaming
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> <MessageContract()> Public Class FileData
        Dim _Stream As System.IO.Stream
        <MessageBodyMember()> Public Property Stream() As System.IO.Stream
            Get
                Return _Stream
            End Get
            Set(ByVal value As System.IO.Stream)
                _Stream = value
            End Set
        End Property

        Public Sub New(ByVal S As System.IO.Stream)
            Stream = S
        End Sub
    End Class


    ''' <summary>
    ''' Classe de demande de fichier
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> <MessageContract()> Public Class RequestFileData
        Dim _Name As String
        <MessageBodyMember()> Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        'Public Sub New(Optional ByVal S As String = "")
        '    Name = S
        'End Sub
    End Class
End Namespace