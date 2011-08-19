Module Variables
    Public myService As HoMIDom.HoMIDom.IHoMIDom
    Public IsConnect As Boolean

    'Variables HomeSeer
    Public hs As Scheduler.hsapplication
    Public hsapp As HomeSeer2.application
    Public frmMere As Window

    'Connecté à HomeSeer
    Public IsHSConnect As Boolean

    'Ecriture dans le fichier log
    Public _Debug As Boolean

    'Animation des ScrollViewer
    Public m_friction As Double = 0.75
    Public m_SpeedTouch As Double = 600
End Module
