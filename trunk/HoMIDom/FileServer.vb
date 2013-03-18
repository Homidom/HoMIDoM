Imports System
Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports System.Linq
Imports System.Drawing

Namespace HoMIDom

    <ServiceContract(Namespace:="http://HoMIDom/")> Public Interface IFileServer
        <OperationContract()> Function Download(ByVal Fichier As RequestFileData) As FileData
        <OperationContract()> Function Upload(ByVal Fichier As UploadFileData) As UploadFileDataResult

    End Interface

    <Serializable()> Public Class FileServer
        Implements IFileServer

        Private BasePath = "Fichiers"

        Public Function Download(ByVal Fichier As RequestFileData) As FileData Implements IFileServer.Download
            Try
                If Not Server.Instance.VerifIdSrv(Fichier.IdServ) Then
                    Return Nothing
                End If

                Dim path = Me.getPath(Fichier.Name)
                If System.IO.File.Exists(path) Then
                    Dim Stream As System.IO.FileStream = New System.IO.FileStream(path, IO.FileMode.Open)
                    Return New FileData(Stream)
                End If
                Return Nothing
            Catch ex As Exception

                Return Nothing
            End Try
        End Function


        Public Function Upload(ByVal Fichier As UploadFileData) As UploadFileDataResult Implements IFileServer.Upload

            Dim result = New UploadFileDataResult()
            result.Success = False

            If Not Server.Instance.VerifIdSrv(Fichier.IdServ) Then
                Return result
            End If

            Dim path = Me.getPath(Fichier.FilePath)

            If System.IO.File.Exists(path) And Not Fichier.Overwrite Then
                Return result
            End If

            Dim fileInfo = New System.IO.FileInfo(path)
            If Not fileInfo.Directory.Exists Then
                fileInfo.Directory.Create()
            End If

            Dim fileStream = Fichier.Stream
            Using outputStream = New System.IO.FileInfo(path).OpenWrite()
                Fichier.Stream.CopyTo(outputStream)
            End Using

            result.Success = True
            Return result
        End Function

        Private Function getPath(ByVal file As String)
            Dim basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Me.BasePath)
            Dim fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(basePath, file))
            If fullPath.StartsWith(basePath) Then
                Return fullPath
            End If

            Throw New ArgumentException("Invalid path")
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


        ''' <summary>
        ''' Id du serveur
        ''' </summary>
        ''' <remarks></remarks>
        <MessageHeader(MustUnderstand:=True)> Public IdServ As String

        ''' <summary>
        ''' Nom et chemin du fichier a télécharger. P.ex: /Musique/xyz.mp3
        ''' </summary>
        ''' <remarks></remarks>
        <MessageBodyMember()> Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property
    End Class


    ''' <summary>
    ''' Classe d'envoi de fichier
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> <MessageContract()> Public Class UploadFileData
        Implements IDisposable

        ''' <summary>
        ''' Id du serveur
        ''' </summary>
        ''' <remarks></remarks>
        <MessageHeader(MustUnderstand:=True)> Public IdServ As String

        ''' <summary>
        ''' Nom du fichier et chemin où il doit être stocké. P.ex: /Musique/xyz.mp3
        ''' </summary>
        ''' <remarks></remarks>
        <MessageHeader(MustUnderstand:=True)> Public FilePath As String

        ''' <summary>
        ''' Longueur du fichier en byte
        ''' </summary>
        ''' <remarks></remarks>
        <MessageHeader(MustUnderstand:=True)> Public Length As Long

        ''' <summary>
        ''' Spécifier si il faut écraser le fichier si celui-ci existe déjà
        ''' </summary>
        ''' <remarks></remarks>
        <MessageHeader(MustUnderstand:=True)> Public Overwrite As Boolean

        ''' <summary>
        ''' Flux de donnée du fichier
        ''' </summary>
        ''' <remarks></remarks>
        <MessageBodyMember()> Public Stream As System.IO.Stream

        Public Sub Dispose() Implements System.IDisposable.Dispose
            If Stream IsNot Nothing Then
                Stream.Close()
                Stream = Nothing
            End If
        End Sub
    End Class


    ''' <summary>
    ''' Classe de retour d'information sur l'envoi du fichier
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> <MessageContract()> Public Class UploadFileDataResult

        ''' <summary>
        ''' Retourne si l'upload s'est correctement déroulé
        ''' </summary>
        ''' <remarks></remarks>
        <MessageBodyMember()> Public Success As Boolean

    End Class
End Namespace