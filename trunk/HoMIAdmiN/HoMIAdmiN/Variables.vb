Imports HoMIDom.HoMIDom
Imports System.Data
Imports System.Windows.Threading

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
    Public thdUDPServer As System.Threading.Thread
    Public WithEvents ClientUDP As HoMIDom.HoMIDom.UDPClient

    'Sequences
    Public _SequenceDriver As String 'N° de sequence en cours du dernier changement d'un driver
    Public _SequenceDevice As String 'N° de sequence en cours du dernier changement d'un device
    Public _SequenceTrigger As String 'N° de sequence en cours du dernier changement d'un trigger
    Public _SequenceZone As String 'N° de sequence en cours du dernier changement d'une zone
    Public _SequenceMacro As String 'N° de sequence en cours du dernier changement d'une macro
    Public _SequenceServer As String 'N° de sequence en cours du dernier changement du server

    'Timers
    Public Timer_Seq As DispatcherTimer = New DispatcherTimer 'Timer pour vérifier les séquences
    Public Timer_Sec As DispatcherTimer = New DispatcherTimer() 'Timer qui est exécuté à chaque secondes

    Public Enum EAction
        Nouveau
        Modifier
    End Enum
End Module
