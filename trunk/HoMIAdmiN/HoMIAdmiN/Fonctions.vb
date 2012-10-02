Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Module Fonctions
    Public Function ConvertArrayToImage(ByVal value As Object) As Object
        Dim ImgSource As BitmapImage = Nothing
        Dim array As Byte() = TryCast(value, Byte())

        If array IsNot Nothing Then
            ImgSource = New BitmapImage()
            ImgSource.BeginInit()
            ImgSource.StreamSource = New MemoryStream(array)
            ImgSource.EndInit()
        End If
        Return ImgSource
    End Function

    Public Function UrlIsValid(ByVal Host As String) As Boolean
        Dim bValid As Boolean = False
        Try
            Dim url As New Uri(Host)
            Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
            Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)
            bValid = True
        Catch ex As Exception
            bValid = False
        End Try
        Return bValid
    End Function

    Public Sub SaveRealTime()
        If IsConnect And My.Settings.SaveRealTime Then
            Dim retour As String = myService.SaveConfig(IdSrv)
            If retour = "0" Then FlagChange = False
        End If
    End Sub
End Module
