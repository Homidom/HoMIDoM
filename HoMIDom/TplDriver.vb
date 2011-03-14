''' <summary>
''' Template de Classe de type Driver pour le service web
''' </summary>
''' <remarks></remarks>
<Serializable()> Public Class TplDriver
    Public ID As String = "" 'Identification unique du driver
    Public Nom As String = ""
    Public Enable As Boolean = False
    Public Description As String = ""
    Public StartAuto As Boolean = False
    Public Protocol As String = ""
    Public IsConnect As Boolean = False
    Public IP_TCP As String = ""
    Public Port_TCP As String = ""
    Public IP_UDP As String = ""
    Public Port_UDP As String = ""
    Public COM As String = ""
    Public Refresh As Integer = 0
    Public Modele As String = ""
    Public Version As String = ""
    Public Picture As String = ""
    Public DeviceSupport As New List(Of String)
End Class
