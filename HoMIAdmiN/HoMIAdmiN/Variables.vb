Module Variables
    Public IdSrv As String
    Public FlagChange As Boolean = False
    Public IsConnect As Boolean = False 'True si connecté au serveur
    Public myService As HoMIDom.HoMIDom.IHoMIDom

    Public Enum EAction
        Nouveau
        Modifier
    End Enum
End Module
