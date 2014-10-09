Imports HoMIDom.HoMIDom
Imports System.Data

Module Variables
    Public IdSrv As String
    Public FlagChange As Boolean = False
    Public IsConnect As Boolean = False 'True si connecté au serveur
    Public myService As HoMIDom.HoMIDom.IHoMIDom
    Public NewDevice As HoMIDom.HoMIDom.NewDevice = Nothing
    Public MyPort As String = ""
    Public myadress As String = ""
    Public _ListeDevices As New List(Of TemplateDevice)
    Public _ListeZones As New List(Of Zone)
    'Public _DevicesAsHisto As New Dictionary(Of String, Long)
    Public _ListeDrivers As New List(Of TemplateDriver)
    Public _TableDBHisto As DataTable = Nothing

    Public Enum EAction
        Nouveau
        Modifier
    End Enum
End Module
