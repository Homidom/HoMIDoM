'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports VB = Microsoft.VisualBasic
Imports System.IO.Ports
Imports System.Math
Imports System.Net.Sockets
Imports System.Threading
Imports System.Globalization
Imports System.Text
Imports System.IO
Imports System.Media

' Auteur : David
' Date : 17/09/2012
'-------------------------------------------------------------------------------------
'                                                                     
'                     Software License Agreement                      
'                                                                     
' A part of this code is owned by RFXCOM, and is protected under applicable copyright laws.
' 
' It is not allowed to use this code or any part of it in an exclusive or patented
' product without the express prior written permission of RFXCOM.
' It is not allowed to use this software or any part of it for non-RFXCOM products.
'
' Any use in violation of the foregoing restrictions may subject the  
' user to criminal sanctions under applicable laws, as well as to     
' civil liability for the breach of the terms and conditions of this license.                                                             
'                                                                      
'-------------------------------------------------------------------------------------

''' <summary>Class Driver_RFXTrx, permet de communiquer avec le RFXtrx Ethernet/COM</summary>
''' <remarks>Pour la version USB, necessite l'installation du driver USB RFXCOM</remarks>
<Serializable()> Public Class Driver_RFXtrx
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "3D9D5D42-475B-11E1-B117-64314824019B"
    Dim _Nom As String = "RFXtrx"
    Dim _Enable As Boolean = False
    Dim _Description As String = "RFXtrx USB/Ethernet Interface"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RF"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = ""
    Dim _Port_TCP As String = ""
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "COM2"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "@"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Dim _LabelsDevice As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _PARAMMODE As String = "20011111111111111011111111"

#End Region

#Region "Variables Internes"

    Enum ICMD As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        cmnd = 4
        msg1 = 5
        msg2 = 6
        msg3 = 7
        msg4 = 8
        msg5 = 9
        msg6 = 10
        msg7 = 11
        msg8 = 12
        msg9 = 13
        size = 13

        'Interface Control
        pType = &H0
        sType = &H0

        'Interface commands
        RESET = &H0 ' reset the receiver/transceiver
        STATUS = &H2 ' request firmware versions and configuration of the interface
        SETMODE = &H3 ' set the configuration of the interface
        ENABLEALL = &H4 ' enable all receiving modes of the receiver/transceiver
        UNDECODED = &H5 ' display UNDECODEDoded messages
        SAVE = &H6 ' save receiving modes of the receiver/transceiver in non-volatile memory
        DISX10 = &H10 ' disable receiving of X10
        DISARC = &H11 ' disable receiving of ARC
        DISAC = &H12 ' disable receiving of AC
        DISHEU = &H13 ' disable receiving of HomeEasy EU
        DISKOP = &H14 ' disable receiving of Ikea-Koppla
        DISOREGON = &H15 ' disable receiving of Oregon Scientific
        DISATI = &H16 ' disable receiving of ATI Remote Wonder
        DISVISONIC = &H17 ' disable receiving of Visonic
        DISMERTIK = &H18 ' disable receiving of Mertik
        DISAD = &H19 ' disable receiving of AD
        DISHID = &H1A ' disable receiving of Hideki
        DISLCROS = &H1B ' disable receiving of La Crosse
        DISFS20 = &H1C ' disable receiving of FS20
        'DISNOVAT = &H1D ' disable receiving of Novatis

        sel310 = &H50 ' select 310MHz in the 310/315 transceiver
        sel315 = &H51 ' select 315MHz in the 310/315 transceiver
        sel800 = &H55 ' select 868.00MHz ASK in the 868 transceiver
        sel800F = &H56 ' select 868.00MHz FSK in the 868 transceiver
        sel830 = &H57 ' select 868.30MHz ASK in the 868 transceiver
        sel830F = &H58 ' select 868.30MHz FSK in the 868 transceiver
        sel835 = &H59 ' select 868.35MHz ASK in the 868 transceiver
        sel835F = &H5A ' select 868.35MHz FSK in the 868 transceiver
        sel895 = &H5B ' select 868.95MHz in the 868 transceiver
    End Enum

    Enum IRESPONSE As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        cmnd = 4
        msg1 = 5
        msg2 = 6
        msg3 = 7
        msg4 = 8
        msg5 = 9
        msg6 = 10
        msg7 = 11
        msg8 = 12
        msg9 = 13
        size = 13

        pType = &H1
        sType = &H0
        recType310 = &H50
        recType315 = &H51
        recType43392 = &H53
        recType86800 = &H55
        recType86800FSK = &H56
        recType86830 = &H57
        recType86830FSK = &H58
        recType86835 = &H59
        recType86835FSK = &H5A
        recType86895 = &H5B

        msg3_undec = &H80
        msg3_RFU6 = &H40
        msg3_RFU5 = &H20
        msg3_RFU4 = &H10
        msg3_RFU3 = &H8
        msg3_FINEOFFSET = &H4
        msg3_RUBICSON = &H2
        msg3_AE = &H1

        msg4_BlindsT1 = &H80
        msg4_BlindsT0 = &H40
        msg4_PROGUARD = &H20
        msg4_FS20 = &H10
        msg4_LCROS = &H8
        msg4_HID = &H4
        msg4_AD = &H2
        msg4_MERTIK = &H1

        msg5_VISONIC = &H80
        msg5_ATI = &H40
        msg5_OREGON = &H20
        msg5_MEI = &H10
        msg5_HEU = &H8
        msg5_AC = &H4
        msg5_ARC = &H2
        msg5_X10 = &H1
    End Enum

    Enum RXRESPONSE As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        msg = 4
        size = 4

        pType = &H2
        sTypeReceiverLockError = &H0
        sTypeTransmitterResponse = &H1
    End Enum

    Enum UNDECODED As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        msg1 = 4
        'msg2 to msg32 depending on RF packet length
        size = 36   'maximum size

        pType = &H3
        sTypeUac = &H0
        sTypeUarc = &H1
        sTypeUati = &H2
        sTypeUhideki = &H3
        sTypeUlacrosse = &H4
        sTypeUad = &H5
        sTypeUmertik = &H6
        sTypeUoregon1 = &H7
        sTypeUoregon2 = &H8
        sTypeUoregon3 = &H9
        sTypeUproguard = &HA
        sTypeUvisonic = &HB
        sTypeUnec = &HC
        sTypeUfs20 = &HD
        sTypeUblinds = &HF
        sTypeUrubicson = &H10
        sTypeUae = &H11
        sTypeUfineoffset = &H12
    End Enum

    Enum LIGHTING1 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        housecode = 4
        unitcode = 5
        cmnd = 6
        filler = 7 'bits 3-0
        rssi = 7   'bits 7-4
        size = 7

        pType = &H10
        sTypeX10 = &H0
        sTypeARC = &H1
        sTypeAB400D = &H2
        sTypeWaveman = &H3
        sTypeEMW200 = &H4
        sTypeIMPULS = &H5
        sTypeRisingSun = &H6
        sTypePhilips = &H7

        sOff = 0
        sOn = 1
        sDim = 2
        sBright = 3
        sAllOff = 5
        sAllOn = 6
        sChime = 7
    End Enum

    Enum LIGHTING2 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        id3 = 6
        id4 = 7
        unitcode = 8
        cmnd = 9
        level = 10
        filler = 11 'bits 3-0
        rssi = 11   'bits 7-4
        size = 11

        pType = &H11
        sTypeAC = &H0
        sTypeHEU = &H1
        sTypeANSLUT = &H2

        sOff = 0
        sOn = 1
        sSetLevel = 2
        sGroupOff = 3
        sGroupOn = 4
        sSetGroupLevel = 5
    End Enum

    Enum LIGHTING3 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        system = 4
        channel8_1 = 5
        channel10_9 = 6
        cmnd = 7
        filler = 8  'bits 3-0
        rssi = 8    'bits 7-4
        size = 8

        pType = &H12
        sTypeKoppla = &H0

        sBright = &H0
        sDim = &H8
        sOn = &H10
        sLevel1 = &H11
        sLevel2 = &H12
        sLevel3 = &H13
        sLevel4 = &H14
        sLevel5 = &H15
        sLevel6 = &H16
        sLevel7 = &H17
        sLevel8 = &H18
        sLevel9 = &H19
        sOff = &H1A
        sProgram = &H1C
    End Enum

    Enum LIGHTING4 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        cmd1 = 4
        cmd2 = 5
        cmd3 = 6
        pulsehigh = 7
        pulselow = 8
        filler = 9
        size = 9

        pType = &H13
        sTypePT2262 = &H0
    End Enum

    Enum LIGHTING5 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        id3 = 6
        unitcode = 7
        cmnd = 8
        level = 9
        filler = 10 'bits 3-0
        rssi = 10   'bits 7-4
        size = 10

        pType = &H14
        sTypeLightwaveRF = &H0
        sTypeEMW100 = &H1

        sOff = 0
        sOn = 1
        sGroupOff = 2
        sLearn = 2
        sMood1 = 3
        sMood2 = 4
        sMood3 = 5
        sMood4 = 6
        sMood5 = 7
        sUnlock = 10
        sLock = 11
        sAllLock = 12
        sClose = 13
        sStop = 14
        sOpen = 15
        sSetLevel = 16
    End Enum

    Enum LIGHTING6 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        groupcode = 6
        unitcode = 7
        cmnd = 8
        cmndseqnbr = 9
        rfu = 10
        filler = 11 'bits 3-0
        rssi = 11   'bits 7-4
        size = 11

        pType = &H15
        sTypeBlyss = &H0

        sOn = 0
        sOff = 1
        sGroupOn = 2
        sGroupOff = 3
    End Enum

    Enum CURTAIN1 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        housecode = 4
        unitcode = 5
        cmnd = 6
        filler = 7
        size = 7

        'types for Curtain
        pType = &H18
        Harrison = &H0

        sOpen = 0
        sClose = 1
        sStop = 2
        sProgram = 3
    End Enum

    Enum BLINDS1 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        id3 = 6
        unitcode = 7
        cmnd = 8
        filler = 9 'bits 3-0
        rssi = 9   'bits 7-4
        size = 9

        'types for Blindss
        pType = &H19
        BlindsT0 = &H0    'RollerTrol or Hasta new
        BlindsT1 = &H1    'Hasta old

        sOpen = 0
        sClose = 1
        sStop = 2
        sConfirm = 3
        sLimit = 4
    End Enum

    Enum SECURITY1 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        id3 = 6
        status = 7
        battery_level = 8   'bits 3-0
        rssi = 8            'bits 7-4
        filler = 8
        size = 8

        'Security
        pType = &H20
        SecX10 = &H0
        SecX10M = &H1
        SecX10R = &H2
        KD101 = &H3
        PowercodeSensor = &H4
        PowercodeMotion = &H5
        Codesecure = &H6
        PowercodeAux = &H7
        Meiantech = &H8

        'status security
        sStatusNormal = &H0
        sStatusNormalDelayed = &H1
        sStatusAlarm = &H2
        sStatusAlarmDelayed = &H3
        sStatusMotion = &H4
        sStatusNoMotion = &H5
        sStatusPanic = &H6
        sStatusPanicOff = &H7
        sStatusArmAway = &H9
        sStatusArmAwayDelayed = &HA
        sStatusArmHome = &HB
        sStatusArmHomeDelayed = &HC
        sStatusDisarm = &HD
        sStatusLightOff = &H10
        sStatusLightOn = &H11
        sStatusLIGHTING2Off = &H12
        sStatusLIGHTING2On = &H13
        sStatusDark = &H14
        sStatusLight = &H15
        sStatusBatLow = &H16
        sStatusPairKD101 = &H17
        sStatusNormalTamper = &H80
        sStatusNormalDelayedTamper = &H81
        sStatusAlarmTamper = &H82
        sStatusAlarmDelayedTamper = &H83
        sStatusMotionTamper = &H84
        sStatusNoMotionTamper = &H85
    End Enum

    Enum CAMERA1 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        housecode = 4
        cmnd = 5
        filler = 6 'bits 3-0
        rssi = 6   'bits 7-4
        size = 6

        'Camera1
        pType = &H28
        Ninja = &H0

        sLeft = 0
        sRight = 1
        sUp = 2
        sDown = 3
        sPosition1 = 4
        sProgramPosition1 = 5
        sPosition2 = 6
        sProgramPosition2 = 7
        sPosition3 = 8
        sProgramPosition3 = 9
        sPosition4 = 10
        sProgramPosition4 = 11
        sCenter = 12
        sProgramCenterPosition = 13
        sSweep = 14
        sProgramSweep = 15
    End Enum

    Enum REMOTE As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id = 4
        cmnd = 5
        toggle = 6       'bit 0
        cmndtype = 6       'bits 3-1
        rssi = 6         'bits 7-4
        size = 6

        'Remotes
        pType = &H30
        ATI = &H0
        ATIplus = &H1
        Medion = &H2
        PCremote = &H3
        ATIrw2 = &H4
    End Enum

    Enum THERMOSTAT1 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        temperature = 6
        set_point = 7
        status = 8  'bits 1-0
        filler = 8  'bits 6-2
        mode = 8    'bit 7
        battery_level = 9   'bits 3-0
        rssi = 9            'bits 7-4
        size = 9

        'Thermostat1
        pType = &H40
        Digimax = &H0    'Digimax with long packet 
        DigimaxShort = &H1   'Digimax with short packet (no set point)
    End Enum

    Enum THERMOSTAT2 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        unitcode = 4
        cmnd = 5
        filler = 6  'bits 3-0
        rssi = 6    'bits 7-4
        size = 6

        'Thermostat2
        pType = &H41
        HE105 = &H0  'HE105
        RTS10 = &H1  'RTS10

        sOff = 0
        sOn = 1
        sProgram = 2
    End Enum

    Enum THERMOSTAT3 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        unitcode1 = 4
        unitcode2 = 5
        unitcode3 = 6
        cmnd = 7
        filler = 9   'bits 3-0
        rssi = 9     'bits 7-4
        size = 9

        'Thermostat3
        pType = &H42
        MertikG6RH4T1 = &H0  'Mertik G6R-H4T1
        MertikG6RH4TB = &H1  'Mertik G6R-H4TB

        sOff = 0
        sOn = 1
        sUp = 2
        sDown = 3
        sRunUp = 4
        Off2nd = 4
        sRunDown = 5
        On2nd = 5
        sStop = 6
    End Enum

    Enum TEMP As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        temperatureh = 6    'bits 6-0
        tempsign = 6        'bit 7
        temperaturel = 7
        battery_level = 8   'bits 3-0
        rssi = 8            'bits 7-4
        size = 8

        'Temperature
        pType = &H50
        TEMP1 = &H1  'THR128/138, THC138
        TEMP2 = &H2  'THC238/268,THN132,THWR288,THRN122,THN122,AW129/131
        TEMP3 = &H3  'THWR800
        TEMP4 = &H4  'RTHN318
        TEMP5 = &H5  'LaCrosse TX3
        TEMP6 = &H6  'TS15C
        TEMP7 = &H7  'Viking 02811
        TEMP8 = &H8  'WS2300
        TEMP9 = &H9  'RUBiCSON
        TEMP10 = &HA   'TFA 30.3133
    End Enum

    Enum HUM As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        humidity = 6
        humidity_status = 7
        battery_level = 8  'bits 3-0
        rssi = 8           'bits 7-4
        size = 8

        'humidity
        pType = &H51
        HUM1 = &H1  'LaCrosse TX3
        HUM2 = &H2  'LaCrosse WS2300
    End Enum

    Enum TEMP_HUM As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        temperatureh = 6    'bits 6-0
        tempsign = 6        'bit 7
        temperaturel = 7
        humidity = 8
        humidity_status = 9
        battery_level = 10  'bits 3-0
        rssi = 10           'bits 7-4
        size = 10

        'temperature+humidity
        pType = &H52
        TH1 = &H1    'THGN122/123,/THGN132,THGR122/228/238/268
        TH2 = &H2    'THGR810/THGN800
        TH3 = &H3    'RTGR328
        TH4 = &H4    'THGR328
        TH5 = &H5    'WTGR800
        TH6 = &H6    'THGR918,THGRN228,THGN500
        TH7 = &H7    'TFA TS34C, Cresta
        TH8 = &H8    'Esic
        TH9 = &H9    'viking 02038
    End Enum

    Enum BARO As Byte
        'barometric
        pType = &H53  'not used
    End Enum

    Enum TEMP_HUM_BARO As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        temperatureh = 6    'bits 6-0
        tempsign = 6        'bit 7
        temperaturel = 7
        humidity = 8
        humidity_status = 9
        baroh = 10
        barol = 11
        forecast = 12
        battery_level = 13  'bits 3-0
        rssi = 13           'bits 7-4
        size = 13

        'temperature+humidity+baro
        pType = &H54
        THB1 = &H1   'BTHR918
        THB2 = &H2   'BTHR918N,BTHR968
    End Enum

    Enum RAIN As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        rainrateh = 6
        rainratel = 7
        raintotal1 = 8
        raintotal2 = 9
        raintotal3 = 10
        battery_level = 11  'bits 3-0
        rssi = 11           'bits 7-4
        size = 11

        'rain
        pType = &H55
        RAIN1 = &H1   'RGR126/682/918
        RAIN2 = &H2   'PCR800
        RAIN3 = &H3   'TFA
        RAIN4 = &H4   'UPM
        RAIN5 = &H5   'WS2300
    End Enum

    Enum WIND As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        directionh = 6
        directionl = 7
        av_speedh = 8
        av_speedl = 9
        gusth = 10
        gustl = 11
        temperatureh = 12    'bits 6-0
        tempsign = 12        'bit 7
        temperaturel = 13
        chillh = 14    'bits 6-0
        chillsign = 14        'bit 7
        chilll = 15
        battery_level = 16  'bits 3-0
        rssi = 16           'bits 7-4
        size = 16

        'wind
        pType = &H56
        WIND1 = &H1   'WTGR800
        WIND2 = &H2   'WGR800
        WIND3 = &H3   'STR918,WGR918
        WIND4 = &H4   'TFA
        WIND5 = &H5   'UPM
        WIND6 = &H6   'WS2300
    End Enum

    Enum UV As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        uv = 6
        temperatureh = 7    'bits 6-0
        tempsign = 7        'bit 7
        temperaturel = 8
        battery_level = 9   'bits 3-0
        rssi = 9            'bits 7-4
        size = 9

        'uv
        pType = &H57
        UV1 = &H1   'UVN128,UV138
        UV2 = &H2   'UVN800
        UV3 = &H3   'TFA
    End Enum

    Enum DT As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        yy = 6
        mm = 7
        dd = 8
        dow = 9
        hr = 10
        min = 11
        sec = 12
        battery_level = 13  'bits 3-0
        rssi = 13           'bits 7-4
        size = 13

        'date & time
        pType = &H58
        DT1 = &H1   'RTGR328N
    End Enum

    Enum CURRENT As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        count = 6
        ch1h = 7
        ch1l = 8
        ch2h = 9
        ch2l = 10
        ch3h = 11
        ch3l = 12
        battery_level = 13  'bits 3-0
        rssi = 13           'bits 7-4
        size = 13

        'current
        pType = &H59
        ELEC1 = &H1   'CM113,Electrisave
    End Enum

    Enum ENERGY As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        count = 6
        instant1 = 7
        instant2 = 8
        instant3 = 9
        instant4 = 10
        total1 = 11
        total2 = 12
        total3 = 13
        total4 = 14
        total5 = 15
        total6 = 16
        battery_level = 17  'bits 3-0
        rssi = 17           'bits 7-4
        size = 17

        'energy
        pType = &H5A
        ELEC2 = &H1   'CM119/160
    End Enum

    Enum GAS As Byte
        'gas
        pType = &H5B  'not used
    End Enum

    Enum WATER As Byte
        'water
        pType = &H5C  'not used
    End Enum

    Enum WEIGHT As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        weighthigh = 6
        weightlow = 7
        filler = 8   'bits 3-0
        rssi = 8            'bits 7-4
        size = 8

        'weight scales
        pType = &H5D
        WEIGHT1 = &H1   'BWR102
        WEIGHT2 = &H2   'GR101
    End Enum

    Enum RFXSENSOR As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id = 4
        msg1 = 5
        msg2 = 6
        filler = 7  'bits 3-0
        rssi = 7    'bits 7-4
        size = 7

        'RFXSensor
        pType = &H70
        Temp = &H0
        AD = &H1
        Volt = &H2
        Message = &H3
    End Enum

    Enum RFXMETER As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        id1 = 4
        id2 = 5
        count1 = 6
        count2 = 7
        count3 = 8
        count4 = 9
        filler = 10 'bits 3-0
        rssi = 10   'bits 7-4
        size = 10

        'RFXMeter
        pType = &H71
        Count = &H0
        Interval = &H1
        Calib = &H2
        Addr = &H3
        CounterReset = &H4
        CounterSet = &HB
        SetInterval = &HC
        SetCalib = &HD
        SetAddr = &HE
        Ident = &HF
    End Enum

    Enum FS20 As Byte
        packetlength = 0
        packettype = 1
        subtype = 2
        seqnbr = 3
        hc1 = 4
        hc2 = 5
        addr = 6
        cmd1 = 7
        cmd2 = 8
        filler = 9 'bits 3-0
        rssi = 9   'bits 7-4
        size = 9

        'FS20
        pType = &H72
        sTypeFS20 = &H0
        sTypeFHT8V = &H1
        sTypeFHT80 = &H2
    End Enum

    'liste des variables de base
    Dim WithEvents RS232Port As New SerialPort
    Private gRecComPortEnabled As Boolean = False
    'Private Resettimer As Integer = 0
    Private trxType As Integer = 0

    Private recbuf(40), recbytes As Byte
    Private bytecnt As Integer = 0
    Private message As String
    Private bytSeqNbr As Byte = 0
    Private bytRemoteToggle As Byte = 0
    Private bytFWversion As Byte
    Private bytCmndSeqNbr As Byte = 0

    Private client As TcpClient
    Private stream As NetworkStream
    Private tcp As Boolean
    Private maxticks As Byte = 0
    Private LogFile As StreamWriter
    Private LogActive As Boolean = False
    Private TCPData(1024) As Byte

    Private port_name As String = ""
    Private dateheurelancement As DateTime
    Dim adressetoint() As String = {"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F", "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF", "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF", "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF", "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF", "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF", "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"}
    Dim adressetoint2() As String = {"0", "1", "2", "3"}
    Dim unittoint() As String = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16"}
    Dim messagerecu As String
    'Private WithEvents tmrRead As New System.Timers.Timer

    'old
    'Private WithEvents tmrRead As New System.Timers.Timer
    'Private messagetemp, messagelast, adresselast, valeurlast, recbuf_last As String
    'Private nblast As Integer = 0
    'Private BufferIn(8192) As Byte
    'Const GETSW As Byte = &H30
    'Const MODEBLK As Byte = &H31
    'Const PING As Byte = &H32
    'Const MODERBRB48 As Byte = &H33
    'Const MODECONT As Byte = &H35
    'Const MODEBRB48 As Byte = &H37
    'Private protocolsynchro As Integer = MODEBRB48
    'Private ack As Boolean = False
    'Private ack_ok As Boolean = True

#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
        End Set
    End Property

    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property
    Public ReadOnly Property DeviceSupport() As ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
        Get
            Return _DeviceSupport
        End Get
    End Property
    Public Property Parametres() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.Parametres
        Get
            Return _Parametres
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _Parametres = value
        End Set
    End Property

    Public Property LabelsDriver() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDriver
        Get
            Return _LabelsDriver
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDriver = value
        End Set
    End Property
    Public Property LabelsDevice() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDevice
        Get
            Return _LabelsDevice
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDevice = value
        End Set
    End Property

    Public Property COM() As String Implements HoMIDom.HoMIDom.IDriver.COM
        Get
            Return _Com
        End Get
        Set(ByVal value As String)
            _Com = value
        End Set
    End Property
    Public ReadOnly Property Description() As String Implements HoMIDom.HoMIDom.IDriver.Description
        Get
            Return _Description
        End Get
    End Property
    Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements HoMIDom.HoMIDom.IDriver.DriverEvent
    Public Property Enable() As Boolean Implements HoMIDom.HoMIDom.IDriver.Enable
        Get
            Return _Enable
        End Get
        Set(ByVal value As Boolean)
            _Enable = value
        End Set
    End Property
    Public ReadOnly Property ID() As String Implements HoMIDom.HoMIDom.IDriver.ID
        Get
            Return _ID
        End Get
    End Property
    Public Property IP_TCP() As String Implements HoMIDom.HoMIDom.IDriver.IP_TCP
        Get
            Return _IP_TCP
        End Get
        Set(ByVal value As String)
            _IP_TCP = value
        End Set
    End Property
    Public Property IP_UDP() As String Implements HoMIDom.HoMIDom.IDriver.IP_UDP
        Get
            Return _IP_UDP
        End Get
        Set(ByVal value As String)
            _IP_UDP = value
        End Set
    End Property
    Public ReadOnly Property IsConnect() As Boolean Implements HoMIDom.HoMIDom.IDriver.IsConnect
        Get
            Return _IsConnect
        End Get
    End Property
    Public Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
        Get
            Return _Modele
        End Get
        Set(ByVal value As String)
            _Modele = value
        End Set
    End Property
    Public ReadOnly Property Nom() As String Implements HoMIDom.HoMIDom.IDriver.Nom
        Get
            Return _Nom
        End Get
    End Property
    Public Property Picture() As String Implements HoMIDom.HoMIDom.IDriver.Picture
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property
    Public Property Port_TCP() As String Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
            _Port_TCP = value
        End Set
    End Property
    Public Property Port_UDP() As String Implements HoMIDom.HoMIDom.IDriver.Port_UDP
        Get
            Return _Port_UDP
        End Get
        Set(ByVal value As String)
            _Port_UDP = value
        End Set
    End Property
    Public ReadOnly Property Protocol() As String Implements HoMIDom.HoMIDom.IDriver.Protocol
        Get
            Return _Protocol
        End Get
    End Property
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
            If _Refresh > 0 Then
                MyTimer.Interval = _Refresh
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick
            End If
        End Set
    End Property
    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property
    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
        End Get
    End Property
    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property
#End Region

#Region "Fonctions génériques"
    ''' <summary>
    ''' Retourne la liste des Commandes avancées
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCommandPlus() As List(Of DeviceCommande)
        Return _DeviceCommandPlus
    End Function

    ''' <summary>Execute une commande avancée</summary>
    ''' <param name="Command"></param>
    ''' <param name="Param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False
        Try
            If MyDevice IsNot Nothing Then
                'Pas de commande demandée donc erreur
                If Command = "" Then
                    Return False
                Else
                    Write(MyDevice, Command, Param(0), Param(1))
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            WriteLog("ERR: ExecuteCommand exception : " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>Permet de vérifier si un champ est valide</summary>
    ''' <param name="Champ"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)


            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        '_IsConnect = True
        Dim retour As String

        'récupération des paramétres avancés
        Try
            _DEBUG = _Parametres.Item(0).Valeur
            _PARAMMODE = _Parametres.Item(1).Valeur
        Catch ex As Exception
            WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
        End Try

        'ouverture du port suivant le Port Com ou IP
        Try
            If _Com <> "" Then
                retour = ouvrir(_Com)
            ElseIf _IP_TCP <> "" Then
                retour = ouvrir(_IP_TCP)
            Else
                retour = "ERR: Port Com ou IP_TCP non défini. Impossible d'ouvrir le port !"
            End If
            'traitement du message de retour
            If STRGS.Left(retour, 4) = "ERR:" Then
                retour = STRGS.Right(retour, retour.Length - 5)
                WriteLog("ERR: Driver non démarré : " & retour)
            Else
                'le driver est démarré, on log puis on lance les handlers
                WriteLog("Driver démarré : " & retour)
                retour = lancer()
                If STRGS.Left(retour, 4) = "ERR:" Then
                    WriteLog("ERR: Start driver non lancé, arrêt du driver")
                    [Stop]()
                Else
                    WriteLog(retour)
                    'les handlers sont lancés, on configure le RFXtrx
                    retour = configurer()
                    If STRGS.Left(retour, 4) = "ERR:" Then
                        retour = STRGS.Right(retour, retour.Length - 5)
                        WriteLog("ERR: Start " & retour)
                        [Stop]()
                    Else
                        WriteLog(retour)
                    End If
                End If
            End If
        Catch ex As Exception
            WriteLog("ERR: Start Exception " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Dim retour As String
        Try
            retour = fermer()
            If STRGS.Left(retour, 4) = "ERR:" Then
                retour = STRGS.Right(retour, retour.Length - 5)
                WriteLog("Stop " & retour)
            Else
                WriteLog("Stop " & retour)
            End If
        Catch ex As Exception
            WriteLog("ERR: Stop Exception " & ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    ''' <summary>Intérroger un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <remarks>pas utilisé</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        'pas utilisé
        If _Enable = False Then Exit Sub
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then Exit Sub
            If _DEBUG Then WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)
            'suivant le protocole, on lance la bonne fonction
            'AC / ACEU / ANSLUT / ARC / BLYSS / ELROAB400D / EMW100 / EMW200 / IMPULS / LIGHTWAVERF / PHILIPS / WAVEMAN / X10

            Select Case UCase(Objet.Modele)
                Case "AC" 'AC : Chacon...
                    If IsNothing(parametre1) Then
                        send_Lighting2(Objet.Adresse1, Command, LIGHTING2.sTypeAC)
                    Else
                        If IsNumeric(parametre1) Then
                            send_Lighting2(Objet.Adresse1, Command, LIGHTING2.sTypeAC, CInt(parametre1))
                        Else
                            WriteLog("ERR: WRITE Le parametre " & CStr(Parametre1) & " n'est pas un entier")
                        End If
                    End If
                Case "ACEU" 'AC norme Europe
                    If IsNothing(Parametre1) Then
                        send_Lighting2(Objet.Adresse1, Command, LIGHTING2.sTypeHEU)
                    Else
                        If IsNumeric(Parametre1) Then
                            send_Lighting2(Objet.Adresse1, Command, LIGHTING2.sTypeHEU, CInt(Parametre1))
                        Else
                            WriteLog("ERR: WRITE Le parametre " & CStr(Parametre1) & " n'est pas un entier")
                        End If
                    End If
                Case "ANSLUT"
                    If IsNothing(Parametre1) Then
                        send_Lighting2(Objet.Adresse1, Command, LIGHTING2.sTypeANSLUT)
                    Else
                        If IsNumeric(Parametre1) Then
                            send_Lighting2(Objet.Adresse1, Command, LIGHTING2.sTypeANSLUT, CInt(Parametre1))
                        Else
                            WriteLog("ERR: WRITE Le parametre " & CStr(Parametre1) & " n'est pas un entier")
                        End If
                    End If
                Case "ARC"
                        send_lighting1(Objet.Adresse1, Command, LIGHTING1.sTypeARC)
                Case "BLYSS"
                        send_lighting6(Objet.Adresse1, Command, LIGHTING6.sTypeBlyss)
                Case "ELROAB400D"
                        send_lighting1(Objet.Adresse1, Command, LIGHTING1.sTypeAB400D)
                Case "EMW100"
                    If Not IsNothing(Parametre1) Then
                        If IsNumeric(Parametre1) Then
                            send_lighting5(Objet.Adresse1, Command, LIGHTING5.sTypeEMW100, CInt(Parametre1))
                        Else
                            WriteLog("ERR: WRITE Le parametre " & CStr(Parametre1) & " n'est pas un entier")
                        End If
                    Else
                        WriteLog("ERR: WRITE Il manque un parametre")
                    End If

                Case "EMW200"
                    send_lighting1(Objet.Adresse1, Command, LIGHTING1.sTypeEMW200)
                Case "IMPULS"
                    send_lighting1(Objet.Adresse1, Command, LIGHTING1.sTypeIMPULS)
                Case "LIGHTWAVERF"
                    If Not IsNothing(Parametre1) Then
                        If IsNumeric(Parametre1) Then
                            send_lighting5(Objet.Adresse1, Command, LIGHTING5.sTypeLightwaveRF, CInt(Parametre1))
                        Else
                            WriteLog("ERR: WRITE Le parametre " & CStr(Parametre1) & " n'est pas un entier")
                        End If
                    Else
                        WriteLog("ERR: WRITE Il manque un parametre")
                    End If
                Case "PHILIPS"
                    send_lighting1(Objet.Adresse1, Command, LIGHTING1.sTypePhilips)
                Case "WAVEMAN"
                    send_lighting1(Objet.Adresse1, Command, LIGHTING1.sTypeWaveman)
                Case "X10"
                    send_lighting1(Objet.Adresse1, Command, LIGHTING1.sTypeX10)
                Case "aucun"
                    WriteLog("ERR: WRITE Pas de protocole d'emission pour " & Objet.Name)
                Case ""
                    WriteLog("ERR: WRITE Pas de protocole d'emission pour " & Objet.Name)
                Case Else
                    WriteLog("ERR: WRITE Protocole non géré : " & Objet.Modele.ToString.ToUpper)
            End Select
        Catch ex As Exception
            WriteLog("ERR: WRITE" & ex.ToString)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

    End Sub

    ''' <summary>ajout des commandes avancées pour les devices</summary>
    ''' <remarks></remarks>
    Private Sub add_devicecommande(ByVal nom As String, ByVal description As String, ByVal nbparam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = nom
            x.DescriptionCommand = description
            x.CountParam = nbparam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            WriteLog("ERR: add_devicecommande Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout Libellé pour le Driver</summary>
    ''' <param name="nom">Nom du champ : HELP</param>
    ''' <param name="labelchamp">Nom à afficher : Aide</param>
    ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
    ''' <remarks></remarks>
    Private Sub Add_LibelleDriver(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
        Try
            Dim y0 As New HoMIDom.HoMIDom.Driver.cLabels
            y0.LabelChamp = Labelchamp
            y0.NomChamp = UCase(Nom)
            y0.Tooltip = Tooltip
            y0.Parametre = Parametre
            _LabelsDriver.Add(y0)
        Catch ex As Exception
            WriteLog("ERR: Add_LibelleDriver Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Ajout Libellé pour les Devices</summary>
    ''' <param name="nom">Nom du champ : HELP</param>
    ''' <param name="labelchamp">Nom à afficher : Aide, si = "@" alors le champ ne sera pas affiché</param>
    ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
    ''' <remarks></remarks>
    Private Sub Add_LibelleDevice(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
        Try
            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            ld0.LabelChamp = Labelchamp
            ld0.NomChamp = UCase(Nom)
            ld0.Tooltip = Tooltip
            ld0.Parametre = Parametre
            _LabelsDevice.Add(ld0)
        Catch ex As Exception
            WriteLog("ERR: Add_LibelleDevice Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout de parametre avancés</summary>
    ''' <param name="nom">Nom du parametre (sans espace)</param>
    ''' <param name="description">Description du parametre</param>
    ''' <param name="valeur">Sa valeur</param>
    ''' <remarks></remarks>
    Private Sub add_paramavance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
        Try
            Dim x As New HoMIDom.HoMIDom.Driver.Parametre
            x.Nom = nom
            x.Description = description
            x.Valeur = valeur
            _Parametres.Add(x)
        Catch ex As Exception
            WriteLog("ERR: add_devicecommande Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Parametres avancés
            add_paramavance("Debug", "Activer le Debug complet (True/False)", False)
            add_paramavance("ParamMode", "Paramétres (ex: 20011111111111111011111111)", "20011111111111111011111111")

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.BAROMETRE.ToString)
            _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
            _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT.ToString)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
            _DeviceSupport.Add(ListeDevices.ENERGIETOTALE.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIECOURANT.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIETOTAL.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.TELECOMMANDE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)
            _DeviceSupport.Add(ListeDevices.UV.ToString)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT.ToString)
            _DeviceSupport.Add(ListeDevices.VOLET.ToString)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            add_devicecommande("GROUP_ON", "Protocole AC/ACEU/ARC/BLYSS : ON sur le groupe du composant", 2)
            add_devicecommande("GROUP_OFF", "Protocole AC/ACEU/ARC/BLYSS : OFF sur le groupe du composant", 2)
            add_devicecommande("GROUP_DIM", "Protocole AC/ACEU : DIM sur le groupe du composant", 2)
            add_devicecommande("BRIGHT", "Protocole X10 : Bright", 2)
            add_devicecommande("ALL_LIGHT_ON", "Protocole X10/EMW200/PHILIPS : ALL_LIGHT_ON", 2)
            add_devicecommande("ALL_LIGHT_OFF", "Protocole X10/EMW200/PHILIPS : ALL_LIGHT_OFF", 2)
            add_devicecommande("CHIME", "Protocole ARC : Chime", 2)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant. Le format dépend du protocole")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "Protocole", "Nom du protocole à utiliser : aucun/AC/ACEU/ANSLUT/ARC/BLYSS/ELROAB400D/EMW100/EMW200/LIGHTWAVERF/IMPULS/PHILIPS/WAVEMAN/X10", "aucun|AC|ACEU|ANSLUT|ARC|BLYSS|ELROAB400D|EMW100|EMW200|IMPULS|LIGHTWAVERF|PHILIPS|WAVEMAN|X10")
            Add_LibelleDevice("REFRESH", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            WriteLog("ERR: New Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

#End Region

#Region "Fonctions Internes"

    ''' <summary>Ouvrir le port COM/ETHERNET</summary>
    ''' <param name="numero">Nom/Numero du port COM/Adresse IP: COM2</param>
    ''' <remarks></remarks>
    Private Function ouvrir(ByVal numero As String) As String
        'Forcer le . 
        Try
            'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            'My.Application.ChangeCulture("en-US")
            If Not _IsConnect Then
                port_name = numero 'pour se rapeller du nom du port
                If VB.Left(numero, 3) <> "COM" Then
                    'RFXtrx est un modele ethernet
                    tcp = True
                    client = New TcpClient(numero, CInt(_Port_TCP))
                    _IsConnect = True
                    Return ("Port IP " & port_name & ":" & _Port_TCP & " ouvert")
                Else
                    'RFXtrx est un modele usb
                    tcp = False
                    RS232Port.PortName = port_name 'nom du port : COM1
                    RS232Port.BaudRate = 38400 'vitesse du port 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200
                    RS232Port.Parity = Parity.None 'pas de parité
                    RS232Port.StopBits = StopBits.One '1 bit d'arrêt par octet
                    RS232Port.DataBits = 8 'nombre de bit par octet
                    'RS232Port.Encoding = System.Text.Encoding.GetEncoding(1252)  'Extended ASCII (8-bits)
                    RS232Port.Handshake = Handshake.None
                    RS232Port.ReadBufferSize = CInt(4096)
                    'RS232Port.ReceivedBytesThreshold = 1
                    RS232Port.ReadTimeout = 100
                    RS232Port.WriteTimeout = 500
                    RS232Port.Open()
                    _IsConnect = True
                    If RS232Port.IsOpen Then
                        GC.SuppressFinalize(RS232Port.BaseStream)
                        RS232Port.DtrEnable = True
                        RS232Port.RtsEnable = True
                        RS232Port.DiscardInBuffer()
                    End If
                    gRecComPortEnabled = True
                    Return ("Port " & port_name & " ouvert")
                End If
            Else
                Return ("Port " & port_name & " dejà ouvert")
            End If
        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try
    End Function

    ''' <summary>Lances les handlers sur le port</summary>
    ''' <remarks></remarks>
    Private Function lancer() As String
        Try
            'lancer les handlers
            If tcp Then
                Try
                    stream = client.GetStream()
                    stream.BeginRead(TCPData, 0, 1024, AddressOf TCPDataReceived, Nothing)
                    Return "Handler IP OK"
                Catch ex As Exception
                    WriteLog("ERR: LANCER GETSTREAM Exception : " & ex.Message)
                    Return "ERR: Handler IP"
                End Try
            Else
                Try
                    AddHandler RS232Port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                    AddHandler RS232Port.ErrorReceived, New SerialErrorReceivedEventHandler(AddressOf ReadErrorEvent)
                    Return "Handler COM OK"
                Catch ex As Exception
                    WriteLog("ERR: LANCER Serial Exception : " & ex.Message)
                    Return "ERR: Handler COM"
                End Try
            End If
            recbuf(0) = 0
            maxticks = 0

            ''tmrRead.Enabled = True
            'If tmrRead.Enabled Then MyTimer.Stop()
            'tmrRead.Interval = 100
            'tmrRead.Start()
        Catch ex As Exception
            WriteLog("ERR: LANCER Serial Exception : " & ex.Message)
            Return "ERR: Exception"
        End Try
    End Function

    ''' <summary>Configurer le RFXtrx</summary>
    ''' <remarks></remarks>
    Private Function configurer() As String
        'configurer le RFXtrx
        Try
            'get firmware version
            SendCommand(ICMD.RESET, "Reset receiver/transceiver:")
            System.Threading.Thread.Sleep(2000)
            'configure Transceiver mode
            SetMode(_PARAMMODE)
            System.Threading.Thread.Sleep(2000)

            dateheurelancement = DateTime.Now

            Return "Configuration OK"
        Catch ex As Exception
            WriteLog("ERR: LANCER Configuration Exception : " & ex.Message)
            Return "ERR: Configuration NOK"
        End Try
    End Function

    ''' <summary>Ferme la connexion au port</summary>
    ''' <remarks></remarks>
    Private Function fermer() As String
        Try
            If _IsConnect Then
                'fermeture des ports
                _IsConnect = False
                'If tmrRead.Enabled Then tmrRead.Stop()
                If tcp Then
                    client.Close()
                    stream.Close()
                    Return ("Port IP fermé")
                Else
                    'suppression de l'attente de données à lire
                    gRecComPortEnabled = False
                    RemoveHandler RS232Port.DataReceived, AddressOf DataReceived
                    RemoveHandler RS232Port.ErrorReceived, AddressOf ReadErrorEvent
                    If (Not (RS232Port Is Nothing)) Then ' The COM port exists.
                        If RS232Port.IsOpen Then
                            Dim limite As Integer = 0
                            'vidage des tampons
                            RS232Port.DiscardInBuffer()
                            RS232Port.DiscardOutBuffer()
                            'au cas on verifie si encore quelque chose à lire
                            Do While (RS232Port.BytesToWrite > 0 And limite < 100) ' Wait for the transmit buffer to empty.
                                limite = limite + 1
                            Loop
                            limite = 0
                            Do While (RS232Port.BytesToRead > 0 And limite < 100) ' Wait for the receipt buffer to empty.
                                limite = limite + 1
                            Loop

                            GC.ReRegisterForFinalize(RS232Port.BaseStream)
                            RS232Port.Close()
                            RS232Port.Dispose()
                            Return ("Port " & port_name & " fermé")
                        End If
                        Return ("Port " & port_name & "  est déjà fermé")
                    End If
                    Return ("Port " & port_name & " n'existe pas")
                End If
                tcp = False
            End If
        Catch ex As UnauthorizedAccessException
            Return ("ERR: Port " & port_name & " IGNORE") ' The port may have been removed. Ignore.
        Catch ex As Exception
            Return ("ERR: Port " & port_name & " : " & ex.Message)
        End Try
        Return "ERR: Not defined"
    End Function

    ''' <summary>ecrire sur le port</summary>
    ''' <param name="commande">premier paquet à envoyer</param>
    ''' <remarks></remarks>
    Private Function ecrire(ByVal commande() As Byte) As String
        Try
            If tcp Then
                stream.Write(commande, 0, commande.Length)
                bytSeqNbr = CByte(bytSeqNbr + 1)
            Else
                RS232Port.Write(commande, 0, commande.Length)
                bytSeqNbr = CByte(bytSeqNbr + 1)
            End If
            Return ""
        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try
    End Function

    Private Sub SendCommand(ByVal command As Byte, ByRef message As String)
        Try

            Dim kar(ICMD.size) As Byte
            kar(ICMD.packetlength) = ICMD.size
            kar(ICMD.packettype) = ICMD.pType
            kar(ICMD.subtype) = ICMD.sType
            kar(ICMD.seqnbr) = bytSeqNbr
            kar(ICMD.cmnd) = command
            kar(ICMD.msg1) = 0
            kar(ICMD.msg2) = 0
            kar(ICMD.msg3) = 0
            kar(ICMD.msg4) = 0
            kar(ICMD.msg5) = 0
            kar(ICMD.msg6) = 0
            kar(ICMD.msg7) = 0
            kar(ICMD.msg8) = 0
            kar(ICMD.msg9) = 0

            For Each bt As Byte In kar
                message = message + VB.Right("0" & Hex(bt), 2)
            Next
            WriteLog("SendCommand : " & message)

            Try
                ecrire(kar)
            Catch exc As Exception
                WriteLog("ERR: SendCommand: Unable to write to port")
            End Try
            kar = Nothing
        Catch ex As Exception
            WriteLog("ERR: SendCommand Exception : " & ex.Message)
        End Try
    End Sub
    'Private Sub ecrirecommande(ByVal kar As Byte())
    '    Dim message As String
    '    Try
    '        Dim temp, tcpdata(0) As Byte
    '        Dim ok(0) As Byte
    '        Dim intIndex, intEnd As Integer
    '        Dim Finish As Double
    '        ack_ok = True

    '        Finish = VB.DateAndTime.Timer + 3.0   ' wait for ACK, max 3-seconds

    '        Do While (ack = False)
    '            If VB.DateAndTime.Timer > Finish Then
    '                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXtrx", "No ACK received witin 3 seconds !")
    '                ack_ok = False
    '                Exit Do
    '            End If

    '            If tcp = True Then
    '                ' As long as there is information, read one byte at a time and output it.
    '                While stream.DataAvailable
    '                    stream.Read(tcpdata, 0, 1)
    '                    temp = tcpdata(0)
    '                    If temp = protocolsynchro Then
    '                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXtrx", "ACK  => " & VB.Right("0" & Hex(temp), 2))
    '                    ElseIf temp = &H5A Then
    '                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXtrx", "NAK  => " & VB.Right("0" & Hex(temp), 2))
    '                    End If
    '                    mess = True
    '                End While
    '            Else
    '                Try
    '                    ' As long as there is information, read one byte at a time and 
    '                    '   output it.
    '                    While (port.BytesToRead() > 0)
    '                        ' Write the output to the screen.
    '                        temp = port.ReadByte()
    '                        If temp = protocolsynchro Then
    '                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXtrx", "ACK  => " & VB.Right("0" & Hex(temp), 2))
    '                        ElseIf temp = &H5A Then
    '                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXtrx", "NAK  => " & VB.Right("0" & Hex(temp), 2))
    '                        End If
    '                        mess = True
    '                    End While
    '                Catch exc As Exception
    '                    ' An exception is raised when there is no information to read : Don't do anything here, just let the exception go.
    '                End Try
    '            End If

    '            If mess Then
    '                ack = True
    '                mess = False
    '            End If
    '        Loop

    '        ack = False

    '        ' Write a user specified Command to the Port.
    '        Try
    '            ecrire(kar)
    '        Catch exc As Exception
    '            ' Warn the user.
    '            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXtrx", "Unable to write to port")
    '            ack_ok = False
    '        Finally

    '        End Try

    '        message = VB.Right("0" & Hex(kar(0)), 2)
    '        intEnd = ((kar(0) And &HF8) / 8)
    '        If (kar(0) And &H7) <> 0 Then
    '            intEnd += 1
    '        End If
    '        For intIndex = 1 To intEnd
    '            message = message + VB.Right("0" & Hex(kar(intIndex)), 2)
    '        Next
    '        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXtrx Ecrirecommande", message)
    '        ack = False
    '    Catch ex As Exception
    '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXtrx ecrirecommande", ex.Message)
    '    End Try
    'End Sub

    ''' <summary>Executer lors de la reception d'une donnée sur le port</summary>
    ''' <remarks></remarks>
    Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        Try
            While gRecComPortEnabled And RS232Port.BytesToRead() > 0
                ProcessReceivedChar(CByte(RS232Port.ReadByte()))
            End While
        Catch Ex As Exception
            WriteLog("ERR: Datareceived Exception : " & Ex.Message)
        End Try
    End Sub

    ''' <summary>Executer lors de la reception d'une erreur sur le port</summary>
    ''' <remarks></remarks>
    Private Sub ReadErrorEvent(ByVal sender As Object, ByVal ev As SerialErrorReceivedEventArgs)
        Try
            While gRecComPortEnabled And RS232Port.BytesToRead() > 0
                ProcessReceivedChar(CByte(RS232Port.ReadByte()))
            End While
        Catch Ex As Exception
            WriteLog("ERR: ReadErrorEvent Exception : " & Ex.Message)
        End Try
    End Sub

    ''' <summary>Executer lors de la reception d'une donnée sur le port IP</summary>
    ''' <remarks></remarks>
    Private Sub TCPDataReceived(ByVal ar As IAsyncResult)
        Dim intCount As Integer
        Try
            If _IsConnect Then
                intCount = stream.EndRead(ar)
                ProcessNewTCPData(TCPData, 0, intCount)
                stream.BeginRead(TCPData, 0, 1024, AddressOf TCPDataReceived, Nothing)
            End If
        Catch Ex As Exception
            WriteLog("ERR: TCPDatareceived Exception : " & Ex.Message)
        End Try
    End Sub

    ''' <summary>Traite les données IP recu</summary>
    ''' <remarks></remarks>
    Private Sub ProcessNewTCPData(ByVal Bytes() As Byte, ByVal offset As Integer, ByVal count As Integer)
        Dim intIndex As Integer
        Try
            For intIndex = offset To offset + count - 1
                ProcessReceivedChar(Bytes(intIndex))
            Next
        Catch ex As Exception
            WriteLog("ERR: ProcessNewTCPData Exception : " & ex.Message)
        End Try
    End Sub

    ' ''' <summary>xxx</summary>
    ' ''' <remarks></remarks>
    'Private Sub tmrRead_Elapsed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrRead.Elapsed
    '    Try
    '        If Resettimer <= 0 Then
    '            If recbytes <> 0 Then 'one or more bytes received
    '                maxticks += 1
    '                If maxticks > 3 Then 'flush buffer due to 400ms timeout
    '                    maxticks = 0
    '                    recbytes = 0
    '                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXtrx", "Buffer flushed due to timeout")
    '                End If
    '            End If
    '        Else
    '            Resettimer = Resettimer - 1    ' decrement resettimer
    '            If Resettimer = 0 Then
    '                If gRecComPortEnabled Then
    '                    RS232Port.DiscardInBuffer()
    '                Else
    '                    'stream.Flush() 'flush not yet supported
    '                End If
    '                SendCommand(ICMD.STATUS, "Get Status:")
    '                maxticks = 0
    '            End If
    '        End If
    '    Catch ex As Exception
    '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXtrx tmrRead_Elapsed", ex.Message)
    '    End Try
    'End Sub

    ''' <summary>Traite chaque byte reçu</summary>
    ''' <param name="sComChar">Byte recu</param>
    ''' <remarks></remarks>
    Private Sub ProcessReceivedChar(ByVal sComChar As Byte)
        Try
            'If Resettimer <> 0 Then
            '    Exit Sub 'ignore received characters after a reset cmd until resettimer = 0
            'End If

            maxticks = 0    'reset receive timeout

            If recbytes = 0 Then    '1st char of a packet received
                If sComChar <> 0 Then
                    If LogActive Then
                        LogFile.WriteLine()
                    End If
                Else
                    Return  'ignore 1st char if 00
                End If
            End If

            recbuf(recbytes) = sComChar 'store received char
            recbytes += 1               'increment char counter

            If recbytes > recbuf(0) Then 'all bytes of the packet received?
                'Write the output to the screen for DEBUG
                messagerecu = messagerecu & VB.Right("0" & Hex(sComChar), 2)
                If _DEBUG Then WriteLog("DBG: Message Reçu : " & messagerecu)
                messagerecu = ""

                decode_messages()  'decode message
                recbytes = 0    'set to zero to receive next message
            Else
                messagerecu = messagerecu & VB.Right("0" & Hex(sComChar), 2) 'get message recu for debug
            End If

        Catch ex As Exception
            WriteLog("ERR: ProcessReceivedChar Exception : " & ex.Message)
        End Try
    End Sub

#End Region

#Region "Decode messages"
    Private Sub decode_messages()
        Try
            Select Case recbuf(1)
                Case IRESPONSE.pType : decode_InterfaceMessage()
                Case RXRESPONSE.pType : decode_RecXmitMessage()
                Case UNDECODED.pType : decode_UNDECODED()
                Case LIGHTING1.pType : decode_Lighting1()
                Case LIGHTING2.pType : decode_Lighting2()
                Case LIGHTING3.pType : decode_Lighting3()
                Case LIGHTING4.pType : decode_Lighting4()
                Case LIGHTING5.pType : decode_Lighting5()
                Case LIGHTING6.pType : decode_Lighting6()
                Case SECURITY1.pType : decode_Security1()
                Case CAMERA1.pType : decode_Camera1()
                Case BLINDS1.pType : decode_BLINDS1()
                Case REMOTE.pType : decode_Remote()
                Case THERMOSTAT1.pType : decode_Thermostat1()
                Case THERMOSTAT2.pType : decode_Thermostat2()
                Case THERMOSTAT3.pType : decode_Thermostat3()
                Case TEMP.pType : decode_Temp()
                Case HUM.pType : decode_Hum()
                Case TEMP_HUM.pType : decode_TempHum()
                Case BARO.pType : decode_Baro()
                Case TEMP_HUM_BARO.pType : decode_TempHumBaro()
                Case RAIN.pType : decode_Rain()
                Case WIND.pType : decode_Wind()
                Case UV.pType : decode_UV()
                Case DT.pType : decode_DateTime()
                Case CURRENT.pType : decode_Current()
                Case ENERGY.pType : decode_Energy()
                Case GAS.pType : decode_Gas()
                Case WATER.pType : decode_Water()
                Case WEIGHT.pType : decode_Weight()
                Case RFXSENSOR.pType : decode_RFXSensor()
                Case RFXMETER.pType : decode_RFXMeter()
                Case FS20.pType : decode_FS20()
                Case Else : _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXtrx decode_messages", "ERROR: Unknown Packet type:" & Hex(recbuf(1)))
            End Select
        Catch ex As Exception
            WriteLog("ERR: decode_messages : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_InterfaceMessage()
        Try
            Dim messagelog As String = ""
            Select Case recbuf(IRESPONSE.subtype)
                Case IRESPONSE.sType
                    '        WriteMessage("subtype           = Interface Response")
                    '        WriteMessage("Sequence nbr      = " & recbuf(IRESPONSE.seqnbr).ToString)
                    Select Case recbuf(IRESPONSE.cmnd)
                        Case ICMD.STATUS, ICMD.SETMODE, ICMD.sel310, ICMD.sel315, ICMD.sel800, ICMD.sel800F, ICMD.sel830, ICMD.sel830F, ICMD.sel835, ICMD.sel835F, ICMD.sel895
                            messagelog = "Interface: Command="
                            Select Case recbuf(IRESPONSE.cmnd)
                                Case ICMD.STATUS : messagelog &= "Get Status"
                                Case ICMD.SETMODE : messagelog &= "Set Mode"
                                Case ICMD.sel310 : messagelog &= "Select 310MHz"
                                Case ICMD.sel315 : messagelog &= "Select 315MHz"
                                Case ICMD.sel800 : messagelog &= "Select 868.00MHz"
                                Case ICMD.sel800F : messagelog &= "Select 868.00MHz FSK"
                                Case ICMD.sel830 : messagelog &= "Select 868.30MHz"
                                Case ICMD.sel830F : messagelog &= "Select 868.30MHz FSK"
                                Case ICMD.sel835 : messagelog &= "Select 868.35MHz"
                                Case ICMD.sel835F : messagelog &= "Select 868.35MHz FSK"
                                Case ICMD.sel895 : messagelog &= "Select 868.95MHz"
                                Case Else : messagelog &= "Error unknown response"
                            End Select
                            messagelog &= ", Type="
                            Select Case recbuf(IRESPONSE.msg1)
                                Case IRESPONSE.recType310 : messagelog &= "Transceiver 310MHz"
                                Case IRESPONSE.recType315 : messagelog &= "Receiver 315MHz"
                                Case IRESPONSE.recType43392 : messagelog &= "Transceiver 433.92MHz"
                                Case IRESPONSE.recType86800 : messagelog &= "Receiver 868.00MHz"
                                Case IRESPONSE.recType86800FSK : messagelog &= "Receiver 868.00MHz FSK"
                                Case IRESPONSE.recType86830 : messagelog &= "Receiver 868.30MHz"
                                Case IRESPONSE.recType86830FSK : messagelog &= "Receiver 868.30MHz FSK"
                                Case IRESPONSE.recType86835 : messagelog &= "Receiver 868.35MHz"
                                Case IRESPONSE.recType86835FSK : messagelog &= "Receiver 868.35MHz FSK"
                                Case IRESPONSE.recType86895 : messagelog &= "Receiver 868.95MHz"
                                Case Else : messagelog &= "Receiver unknown"
                            End Select
                            trxType = recbuf(IRESPONSE.msg1)
                            messagelog &= ", Firmware=" & recbuf(IRESPONSE.msg2)
                            messagelog &= ", Hardware=" & recbuf(IRESPONSE.msg6) & "." & recbuf(IRESPONSE.msg7)
                            WriteLog(messagelog)
                            bytFWversion = recbuf(IRESPONSE.msg2)

                            messagelog = "Protocole: "
                            'If (recbuf(IRESPONSE.msg3) And &H80) <> 0 Then messagelog &= "Undec=on" Else messagelog &= "Undec=off"
                            'If (recbuf(IRESPONSE.msg5) And &H1) <> 0 Then messagelog &= ", X10=on" Else messagelog &= ", X10=off"
                            'If (recbuf(IRESPONSE.msg5) And &H2) <> 0 Then messagelog &= ", ARC=on" Else messagelog &= ", ARC=off"
                            'If (recbuf(IRESPONSE.msg5) And &H4) <> 0 Then messagelog &= ", AC=on" Else messagelog &= ", AC=off"
                            'If (recbuf(IRESPONSE.msg5) And &H8) <> 0 Then messagelog &= ", HomeEasyEU=on" Else messagelog &= ", HomeEasyEU=off"
                            'If (recbuf(IRESPONSE.msg5) And &H10) <> 0 Then messagelog &= ", Meiantech=on" Else messagelog &= ", Meiantech=off"
                            'If (recbuf(IRESPONSE.msg5) And &H20) <> 0 Then messagelog &= ", OregonScientific=on" Else messagelog &= ", OregonScientific=off"
                            'If (recbuf(IRESPONSE.msg5) And &H40) <> 0 Then messagelog &= ", ATI=on" Else messagelog &= ", ATI=off"
                            'If (recbuf(IRESPONSE.msg5) And &H80) <> 0 Then messagelog &= ", Visonic=on" Else messagelog &= ", Visonic=off"
                            'If (recbuf(IRESPONSE.msg4) And &H1) <> 0 Then messagelog &= ", Mertik=on" Else messagelog &= ", Mertik=off"
                            'If (recbuf(IRESPONSE.msg4) And &H2) <> 0 Then messagelog &= ", LightwaveRF=on" Else messagelog &= ", LightwaveRF=off"
                            'If (recbuf(IRESPONSE.msg4) And &H4) <> 0 Then messagelog &= ", Hideki=on" Else messagelog &= ", Hideki=off"
                            'If (recbuf(IRESPONSE.msg4) And &H8) <> 0 Then messagelog &= ", LaCrosse=on" Else messagelog &= ", LaCrosse=off"
                            'If (recbuf(IRESPONSE.msg4) And &H10) <> 0 Then messagelog &= ", FS20=on" Else messagelog &= ", FS20=off"
                            'If (recbuf(IRESPONSE.msg4) And &H20) <> 0 Then messagelog &= ", ProGuard=on" Else messagelog &= ", ProGuard=off"
                            'If (recbuf(IRESPONSE.msg4) And &H40) <> 0 Then messagelog &= ", Novatis=on" Else messagelog &= ", Novatis=off"
                            'If (recbuf(IRESPONSE.msg4) And &H80) <> 0 Then messagelog &= ", RFUprotocol7=on" Else messagelog &= ", RFUprotocol7=off"

                            If (recbuf(IRESPONSE.msg3) And IRESPONSE.msg3_undec) <> 0 Then messagelog &= "Undec=on" Else messagelog &= "Undec=off"
                            If (recbuf(IRESPONSE.msg5) And IRESPONSE.msg5_X10) <> 0 Then messagelog &= ", X10=on" Else messagelog &= ", X10=off"
                            If (recbuf(IRESPONSE.msg5) And IRESPONSE.msg5_ARC) <> 0 Then messagelog &= ", ARC=on" Else messagelog &= ", ARC=off"
                            If (recbuf(IRESPONSE.msg5) And IRESPONSE.msg5_AC) <> 0 Then messagelog &= ", AC=on" Else messagelog &= ", AC=off"
                            If (recbuf(IRESPONSE.msg5) And IRESPONSE.msg5_HEU) <> 0 Then messagelog &= ", HomeEasyEU=on" Else messagelog &= ", HomeEasyEU=off"
                            If (recbuf(IRESPONSE.msg5) And IRESPONSE.msg5_MEI) <> 0 Then messagelog &= ", Meiantech=on" Else messagelog &= ", Meiantech=off"
                            If (recbuf(IRESPONSE.msg5) And IRESPONSE.msg5_OREGON) <> 0 Then messagelog &= ", OregonScientific=on" Else messagelog &= ", OregonScientific=off"
                            If (recbuf(IRESPONSE.msg5) And IRESPONSE.msg5_ATI) <> 0 Then messagelog &= ", ATI=on" Else messagelog &= ", ATI=off"
                            If (recbuf(IRESPONSE.msg5) And IRESPONSE.msg5_VISONIC) <> 0 Then messagelog &= ", Visonic=on" Else messagelog &= ", Visonic=off"
                            If (recbuf(IRESPONSE.msg4) And IRESPONSE.msg4_MERTIK) <> 0 Then messagelog &= ", Mertik=on" Else messagelog &= ", Mertik=off"
                            If (recbuf(IRESPONSE.msg4) And IRESPONSE.msg4_AD) <> 0 Then messagelog &= ", AD=on" Else messagelog &= ", AD=off"
                            If (recbuf(IRESPONSE.msg4) And IRESPONSE.msg4_HID) <> 0 Then messagelog &= ", Hideki=on" Else messagelog &= ", Hideki=off"
                            If (recbuf(IRESPONSE.msg4) And IRESPONSE.msg4_LCROS) <> 0 Then messagelog &= ", LaCrosse=on" Else messagelog &= ", LaCrosse=off"
                            If (recbuf(IRESPONSE.msg4) And IRESPONSE.msg4_FS20) <> 0 Then messagelog &= ", FS20=on" Else messagelog &= ", FS20=off"
                            If (recbuf(IRESPONSE.msg4) And IRESPONSE.msg4_PROGUARD) <> 0 Then messagelog &= ", ProGuard=on" Else messagelog &= ", ProGuard=off"
                            If (recbuf(IRESPONSE.msg4) And IRESPONSE.msg4_BlindsT0) <> 0 Then messagelog &= ", BlindsT0=on" Else messagelog &= ", BlindsT0=off"
                            If (recbuf(IRESPONSE.msg4) And IRESPONSE.msg4_BlindsT1) <> 0 Then messagelog &= ", BlindsT1=on" Else messagelog &= ", BlindsT1=off"
                            If (recbuf(IRESPONSE.msg3) And IRESPONSE.msg3_AE) <> 0 Then messagelog &= ", AE=on" Else messagelog &= ", AE=off"
                            If (recbuf(IRESPONSE.msg3) And IRESPONSE.msg3_RUBICSON) <> 0 Then messagelog &= ", RUBiCSON=on" Else messagelog &= ", RUBiCSON=off"
                            If (recbuf(IRESPONSE.msg3) And IRESPONSE.msg3_FINEOFFSET) <> 0 Then messagelog &= ", FineOffset=on" Else messagelog &= ", FineOffset=off"
                            If (recbuf(IRESPONSE.msg3) And IRESPONSE.msg3_RFU3) <> 0 Then messagelog &= ", RFUprotocol3=on" Else messagelog &= ", RFUprotocol3=off"
                            If (recbuf(IRESPONSE.msg3) And IRESPONSE.msg3_RFU4) <> 0 Then messagelog &= ", RFUprotocol4=on" Else messagelog &= ", RFUprotocol4=off"
                            If (recbuf(IRESPONSE.msg3) And IRESPONSE.msg3_RFU5) <> 0 Then messagelog &= ", RFUprotocol5=on" Else messagelog &= ", RFUprotocol5=off"
                            If (recbuf(IRESPONSE.msg3) And IRESPONSE.msg3_RFU6) <> 0 Then messagelog &= ", RFUprotocol6=on" Else messagelog &= ", RFUprotocol6=off"

                            WriteLog(messagelog)
                        Case ICMD.ENABLEALL : WriteLog("Réponse à : Enable All RF")
                        Case ICMD.UNDECODED : WriteLog("Réponse à : UNDECODED on")
                        Case ICMD.SAVE : WriteLog("Réponse à : Save")
                        Case ICMD.DISX10 : WriteLog("Réponse à : Disable X10 RF")
                        Case ICMD.DISARC : WriteLog("Réponse à : Disable ARC RF")
                        Case ICMD.DISAC : WriteLog("Réponse à : Disable AC RF")
                        Case ICMD.DISHEU : WriteLog("Réponse à : Disable HomeEasy EU RF")
                        Case ICMD.DISKOP : WriteLog("Réponse à : Disable Meiantech RF")
                        Case ICMD.DISOREGON : WriteLog("Réponse à : Disable Oregon Scientific RF")
                        Case ICMD.DISATI : WriteLog("Réponse à : Disable ATI remote RF")
                        Case ICMD.DISVISONIC : WriteLog("Réponse à : Disable Visonic RF")
                        Case ICMD.DISMERTIK : WriteLog("Réponse à : Disable Mertik RF")
                        Case ICMD.DISAD : WriteLog("Réponse à : Disable AD RF")
                        Case ICMD.DISHID : WriteLog("Réponse à : Disable Hideki RF")
                        Case ICMD.DISLCROS : WriteLog("Réponse à : Disable La Crosse RF")
                            'Case ICMD.DISNOVAT : WriteLog("Réponse à : Disable Novatis RF")
                        Case Else : WriteLog("ERR: decode_InterfaceMessage : Données incorrectes reçues : type=" & Hex(recbuf(IRESPONSE.packettype)) & ", Sub type=" & Hex(recbuf(IRESPONSE.subtype)) & " cmnd=" & Hex(recbuf(IRESPONSE.cmnd)))
                    End Select
            End Select
            messagelog = Nothing
        Catch ex As Exception
            WriteLog("ERR: decode_InterfaceMessage : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_RecXmitMessage()
        Try
            Select recbuf(RXRESPONSE.subtype)
                Case RXRESPONSE.sTypeReceiverLockError
                    If _DEBUG Then WriteLog("ERR: Receiver lock error")
                Case RXRESPONSE.sTypeTransmitterResponse
                    Select Case recbuf(RXRESPONSE.msg)
                        Case &H0 : If _DEBUG Then WriteLog("Transmitter Response : ACK, data correct transmitted")
                        Case &H1 : If _DEBUG Then WriteLog("Transmitter Response : ACK, but transmit started after 6 seconds delay anyway with RF receive data detected")
                        Case &H2 : If _DEBUG Then WriteLog("ERR: Transmitter Response : NAK, transmitter did not lock on the requested transmit frequency")
                        Case &H3 : If _DEBUG Then WriteLog("ERR: Transmitter Response : NAK, AC address zero in id1-id4 not allowed")
                        Case Else : If _DEBUG Then WriteLog("ERR: decode_RecXmitMessage : Type de message reçu incorrect : type=" & Hex(recbuf(RXRESPONSE.msg)))
                    End Select
                Case Else : If _DEBUG Then WriteLog("ERR: decode_RecXmitMessage : Données incorrectes reçues : type=" & Hex(recbuf(RXRESPONSE.packettype)) & ", Sub type=" & Hex(recbuf(RXRESPONSE.subtype)))
            End Select
        Catch ex As Exception
            WriteLog("ERR: decode_RecXmitMessage : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_UNDECODED()
        Try
            Dim messagelog As String = ""
            messagelog = "UNDECODED "
            Select Case recbuf(UNDECODED.subtype)
                Case UNDECODED.sTypeUac : messagelog &= "AC:"
                Case UNDECODED.sTypeUarc : messagelog &= "ARC:"
                Case UNDECODED.sTypeUati : messagelog &= "ATI:"
                Case UNDECODED.sTypeUhideki : messagelog &= "HIDEKI:"
                Case UNDECODED.sTypeUlacrosse : messagelog &= "LACROSSE:"
                Case UNDECODED.sTypeUad : messagelog &= "AD:"
                Case UNDECODED.sTypeUmertik : messagelog &= "MERTIK:"
                Case UNDECODED.sTypeUoregon1 : messagelog &= "OREGON1:"
                Case UNDECODED.sTypeUoregon2 : messagelog &= "OREGON2:"
                Case UNDECODED.sTypeUoregon3 : messagelog &= "OREGON3:"
                Case UNDECODED.sTypeUproguard : messagelog &= "PROGUARD:"
                Case UNDECODED.sTypeUvisonic : messagelog &= "VISONIC:"
                Case UNDECODED.sTypeUnec : messagelog &= "NEC:"
                Case UNDECODED.sTypeUfs20 : messagelog &= "FS20:"
                Case UNDECODED.sTypeUblinds : messagelog &= "Blinds:"
                Case UNDECODED.sTypeUrubicson : messagelog &= "RUBICSON:"
                Case UNDECODED.sTypeUae : messagelog &= "AE:"
                Case UNDECODED.sTypeUfineoffset : messagelog &= "FineOffset:"
                Case Else : messagelog = "ERR: UNDECODED Unknown Sub type for Packet type=" & Hex(recbuf(UNDECODED.packettype)) & ": " & Hex(recbuf(UNDECODED.subtype))
            End Select
            For i = 0 To recbuf(UNDECODED.packetlength) - UNDECODED.msg1
                messagelog &= VB.Right("0" & Hex(recbuf(UNDECODED.msg1 + i)), 2)
            Next
            WriteLog(messagelog)
        Catch ex As Exception
            WriteLog("ERR: decode_UNDECODED Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Lighting1()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Select Case recbuf(LIGHTING1.subtype)
                Case LIGHTING1.sTypeX10
                    '        WriteMessage("subtype       = X10")
                    '        WriteMessage("Sequence nbr  = " & recbuf(LIGHTING1.seqnbr).ToString)
                    adresse = Chr(recbuf(LIGHTING1.housecode)) & recbuf(LIGHTING1.unitcode).ToString
                    Select Case recbuf(LIGHTING1.cmnd)
                        Case LIGHTING1.sOff : valeur = "OFF"
                        Case LIGHTING1.sOn : valeur = "ON"
                        Case LIGHTING1.sDim : valeur = "DIM"
                        Case LIGHTING1.sBright : valeur = "BRIGHT"
                        Case LIGHTING1.sAllOn : valeur = "ALL_ON"
                        Case LIGHTING1.sAllOff : valeur = "ALL_OFF"
                        Case LIGHTING1.sChime : valeur = "CHIME"
                        Case Else : valeur = "UNKNOWN"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case LIGHTING1.sTypeARC
                    '        WriteMessage("subtype       = ARC")
                    '        WriteMessage("Sequence nbr  = " & recbuf(LIGHTING1.seqnbr).ToString)
                    adresse = Chr(recbuf(LIGHTING1.housecode)) & recbuf(LIGHTING1.unitcode).ToString
                    Select Case recbuf(LIGHTING1.cmnd)
                        Case LIGHTING1.sOff : valeur = "OFF"
                        Case LIGHTING1.sOn : valeur = "ON"
                            'Case &H2 : valeur = "DIM"
                            'Case &H3 : valeur = "BRIGHT"
                        Case LIGHTING1.sAllOn : valeur = "ALL_ON"
                        Case LIGHTING1.sAllOff : valeur = "ALL_OFF"
                        Case Else : valeur = "UNKNOWN"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case LIGHTING1.sTypeAB400D
                    '        WriteMessage("subtype       = ELRO AB400")
                    '        WriteMessage("Sequence nbr  = " & recbuf(LIGHTING1.seqnbr).ToString)
                    adresse = Chr(recbuf(LIGHTING1.housecode)) & recbuf(LIGHTING1.unitcode).ToString
                    Select Case recbuf(LIGHTING1.cmnd)
                        Case LIGHTING1.sOff : valeur = "OFF"
                        Case LIGHTING1.sOn : valeur = "ON"
                        Case Else : valeur = "UNKNOWN"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case Else
                    WriteLog("ERR: decode_Lighting1 : Unknown Sub type for Packet type=" & Hex(recbuf(LIGHTING1.packettype)) & ": " & Hex(recbuf(LIGHTING1.subtype)))
            End Select
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(LIGHTING1.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Lighting1 Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Lighting2()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Select Case recbuf(LIGHTING2.subtype)
                Case LIGHTING2.sTypeAC, LIGHTING2.sTypeHEU, LIGHTING2.sTypeANSLUT
                    '        Select Case recbuf(LIGHTING2.subtype)
                    '            Case LIGHTING2.sTypeAC
                    '                WriteMessage("subtype       = AC")
                    '            Case LIGHTING2.sTypeHEU
                    '                WriteMessage("subtype       = HomeEasy EU")
                    '            Case LIGHTING2.sTypeANSLUT
                    '                WriteMessage("subtype       = ANSLUT")
                    '        End Select
                    '        WriteMessage("Sequence nbr  = " & recbuf(LIGHTING2.seqnbr).ToString)
                    adresse = Hex(recbuf(LIGHTING2.id1)) & VB.Right("0" & Hex(recbuf(LIGHTING2.id2)), 2) & VB.Right("0" & Hex(recbuf(LIGHTING2.id3)), 2) & VB.Right("0" & Hex(recbuf(LIGHTING2.id4)), 2) & "-" & recbuf(LIGHTING2.unitcode).ToString
                    Select Case recbuf(LIGHTING2.cmnd)
                        Case LIGHTING2.sOff : valeur = "OFF"
                        Case LIGHTING2.sOn : valeur = "ON"
                        Case LIGHTING2.sSetLevel : valeur = "SET_LEVEL:" & recbuf(LIGHTING2.level).ToString
                        Case LIGHTING2.sGroupOff : valeur = "GROUP_OFF"
                        Case LIGHTING2.sGroupOn : valeur = "GROUP_ON"
                        Case LIGHTING2.sSetGroupLevel : valeur = "SET_GROUP_LEVEL:" & recbuf(LIGHTING2.level).ToString
                        Case Else : valeur = "UNKNOWN"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case Else : WriteLog("ERR: decode_Lighting2 : Unknown Sub type for Packet type=" & Hex(recbuf(LIGHTING2.packettype)) & ": " & Hex(recbuf(LIGHTING2.subtype)))
            End Select
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(LIGHTING2.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Lighting2 Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Lighting3()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Select Case recbuf(LIGHTING3.subtype)
                Case LIGHTING3.sTypeKoppla
                    '        WriteMessage("subtype       = Ikea Koppla")
                    '        WriteMessage("Sequence nbr  = " & recbuf(LIGHTING3.seqnbr).ToString)
                    '        WriteMessage("Command       = "
                    adresse = "IKEAKOPPLA"
                    Select Case recbuf(LIGHTING3.cmnd)
                        Case &H0 : valeur = "OFF"
                        Case &H1 : valeur = "ON"
                        Case &H20 : valeur = "SET_LEVEL:" & recbuf(6).ToString
                        Case &H21 : valeur = "PROGRAM"
                        Case Else
                            If recbuf(LIGHTING3.cmnd) >= &H10 And recbuf(LIGHTING3.cmnd) < &H18 Then
                                valeur = "DIM"
                            ElseIf recbuf(LIGHTING3.cmnd) >= &H18 And recbuf(LIGHTING3.cmnd) < &H20 Then
                                valeur = "BRIGHT"
                            Else
                                valeur = "UNKNOWN"
                            End If
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case Else : WriteLog("ERR: decode_Lighting3 : Unknown Sub type for Packet type=" & Hex(recbuf(LIGHTING3.packettype)) & ": " & Hex(recbuf(LIGHTING3.subtype)))
            End Select
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(LIGHTING3.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Lighting3 Exception : " & ex.Message)
        End Try
    End Sub
    'Not implemented
    Private Sub decode_Lighting4()
        Try
            'WriteMessage("Not implemented")
        Catch ex As Exception
            WriteLog("ERR: decode_Lighting4 Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Lighting5()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Select Case recbuf(LIGHTING5.subtype)
                Case LIGHTING5.sTypeLightwaveRF
                    'WriteMessage("subtype       = LightwaveRF")
                    'WriteMessage("Sequence nbr  = " & recbuf(LIGHTING5.seqnbr).ToString)
                    adresse = VB.Right("0" & Hex(recbuf(LIGHTING5.id1)), 2) & VB.Right("0" & Hex(recbuf(LIGHTING5.id2)), 2) & VB.Right("0" & Hex(recbuf(LIGHTING5.id3)), 2) & "-" & recbuf(LIGHTING5.unitcode).ToString
                    Select Case recbuf(LIGHTING5.cmnd)
                        Case LIGHTING5.sOff : valeur = "OFF"
                        Case LIGHTING5.sOn : valeur = "ON"
                        Case LIGHTING5.sGroupOff : valeur = "GROUP_OFF"
                        Case LIGHTING5.sMood1 : valeur = "GROUP_Mood_1"
                        Case LIGHTING5.sMood2 : valeur = "GROUP_Mood_2"
                        Case LIGHTING5.sMood3 : valeur = "GROUP_Mood_3"
                        Case LIGHTING5.sMood4 : valeur = "GROUP_Mood_4"
                        Case LIGHTING5.sMood5 : valeur = "GROUP_Mood_5"
                        Case LIGHTING5.sUnlock : valeur = "UNLOCK"
                        Case LIGHTING5.sLock : valeur = "LOCK"
                        Case LIGHTING5.sAllLock : valeur = "ALL_LOCK"
                        Case LIGHTING5.sClose : valeur = "CLOSE_INLINE_RELAY"
                        Case LIGHTING5.sStop : valeur = "STOP_INLINE_RELAY"
                        Case LIGHTING5.sOpen : valeur = "OPEN_INLINE_RELAY"
                        Case LIGHTING5.sSetLevel : valeur = CInt((recbuf(LIGHTING5.level) * 3.2)).ToString 'Dim level "Set dim level to: " & CInt((recbuf(LIGHTING5.level) * 3.2)).ToString & "%"
                        Case Else : valeur = "UNKNOWN"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case LIGHTING5.sTypeEMW100
                    'WriteMessage("subtype       = EMW100")
                    'WriteMessage("Sequence nbr  = " & recbuf(LIGHTING5.seqnbr).ToString)
                    adresse = VB.Right("0" & Hex(recbuf(LIGHTING5.id2)), 2) & VB.Right("0" & Hex(recbuf(LIGHTING5.id3)), 2)
                    'WriteMessage("Unit          = " & recbuf(LIGHTING5.unitcode).ToString)
                    'WriteMessage("Command       = ", False)
                    Select Case recbuf(LIGHTING5.cmnd)
                        Case LIGHTING5.sOff : valeur = "OFF"
                        Case LIGHTING5.sOn : valeur = "ON"
                        Case LIGHTING5.sLearn : valeur = "LEARN"
                        Case Else : valeur = "UNKNOWN"
                    End Select

                Case Else : WriteLog("ERR: decode_Lighting5 : Unknown Sub type for Packet type=" & Hex(recbuf(LIGHTING5.packettype)) & ": " & Hex(recbuf(LIGHTING5.subtype)))
            End Select
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(LIGHTING5.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Lighting5 Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Lighting6()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Select Case recbuf(LIGHTING6.subtype)
                Case LIGHTING6.sTypeBlyss
                    'WriteMessage("subtype       = BLYSS")
                    'WriteMessage("Sequence nbr  = " & recbuf(LIGHTING6.seqnbr).ToString)
                    adresse = VB.Right("0" & Hex(recbuf(LIGHTING6.id1)), 2) & VB.Right("0" & Hex(recbuf(LIGHTING6.id2)), 2)
                    'WriteMessage("groupcode     = " & Chr(recbuf(LIGHTING6.groupcode)))
                    'WriteMessage("unitcode      = " & recbuf(LIGHTING6.unitcode).ToString)
                    Select Case recbuf(LIGHTING6.cmnd)
                        Case &H0 : valeur = "OFF"
                        Case &H1 : valeur = "ON"
                        Case Else : valeur = "UNKNOWN"
                    End Select
                    Select Case recbuf(LIGHTING6.cmnd)
                        Case LIGHTING6.sOff : valeur = "OFF"
                        Case LIGHTING6.sOn : valeur = "ON"
                        Case LIGHTING6.sGroupOff : valeur = "GROUP_OFF"
                        Case LIGHTING6.sGroupOn : valeur = "GROUP_ON"
                        Case Else : valeur = "UNKNOWN"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case Else : WriteLog("ERR: decode_Lighting6 : Unknown Sub type for Packet type=" & Hex(recbuf(LIGHTING6.packettype)) & ": " & Hex(recbuf(LIGHTING6.subtype)))
            End Select
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(LIGHTING6.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Lighting6 Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Security1()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            'Select Case recbuf(SECURITY1.subtype)
            '    Case SECURITY1.SecX10
            '        WriteMessage("subtype       = X10 security")
            '    Case SECURITY1.SecX10M
            '        WriteMessage("subtype       = X10 security motion")
            '    Case SECURITY1.SecX10R
            '        WriteMessage("subtype       = X10 security remote")
            '    Case SECURITY1.KD101
            '        WriteMessage("subtype       = KD101 smoke detector")
            '    Case SECURITY1.PowercodeSensor
            '        WriteMessage("subtype       = Visonic PowerCode sensor - primary contact")
            '    Case SECURITY1.PowercodeMotion
            '        WriteMessage("subtype       = Visonic PowerCode motion")
            '    Case SECURITY1.Codesecure
            '        WriteMessage("subtype       = Visonic CodeSecure")
            '    Case SECURITY1.PowercodeAux
            '        WriteMessage("subtype       = Visonic PowerCode sensor - auxiliary contact")
            '   Case SECURITY1.Meiantech
            '        WriteMessage("subtype       = Meiantech")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(SECURITY1.packettype)) & ": " & Hex(recbuf(SECURITY1.subtype)))
            'End Select

            adresse = VB.Right("0" & Hex(recbuf(SECURITY1.id1)), 2) & VB.Right("0" & Hex(recbuf(SECURITY1.id2)), 2) & VB.Right("0" & Hex(recbuf(SECURITY1.id3)), 2)
            Select Case recbuf(SECURITY1.status)
                Case SECURITY1.sStatusNormal : valeur = "Normal"
                Case SECURITY1.sStatusNormalDelayed : valeur = "Normal Delayed"
                Case SECURITY1.sStatusAlarm : valeur = "Alarm"
                Case SECURITY1.sStatusAlarmDelayed : valeur = "Alarm Delayed"
                Case SECURITY1.sStatusMotion : valeur = "Motion"
                Case SECURITY1.sStatusNoMotion : valeur = "No Motion"
                Case SECURITY1.sStatusPanic : valeur = "Panic"
                Case SECURITY1.sStatusPanicOff : valeur = "Panic End"
                Case SECURITY1.sStatusArmAway : valeur = "Arm Away"
                Case SECURITY1.sStatusArmAwayDelayed : valeur = "Arm Away Delayed"
                Case SECURITY1.sStatusArmHome : valeur = "Arm Home"
                Case SECURITY1.sStatusArmHomeDelayed : valeur = "Arm Home Delayed"
                Case SECURITY1.sStatusDisarm : valeur = "Disarm"
                Case SECURITY1.sStatusLightOff : valeur = "Light Off"
                Case SECURITY1.sStatusLightOn : valeur = "Light On"
                Case SECURITY1.sStatusLIGHTING2Off : valeur = "Light 2 Off"
                Case SECURITY1.sStatusLIGHTING2On : valeur = "Light 2 On"
                Case SECURITY1.sStatusDark : valeur = "Dark detected"
                Case SECURITY1.sStatusLight : valeur = "Light Detected"
                Case SECURITY1.sStatusBatLow : valeur = "Battery low MS10 or XX18 sensor"
                Case SECURITY1.sStatusPairKD101 : valeur = "Pair KD101"
                Case SECURITY1.sStatusNormalTamper : valeur = "Normal + Tamper"
                Case SECURITY1.sStatusNormalDelayedTamper : valeur = "Normal Delayed + Tamper"
                Case SECURITY1.sStatusAlarmTamper : valeur = "Alarm + Tamper"
                Case SECURITY1.sStatusAlarmDelayedTamper : valeur = "Alarm Delayed + Tamper"
                Case SECURITY1.sStatusMotionTamper : valeur = "Motion + Tamper"
                Case SECURITY1.sStatusNoMotionTamper : valeur = "No Motion + Tamper"
            End Select
            WriteRetour(adresse, "", valeur)
            If recbuf(SECURITY1.subtype) <> SECURITY1.KD101 Then    'KD101 does not support battery low indication
                If (recbuf(SECURITY1.battery_level) And &HF) = 0 Then WriteBattery(adresse) 'battery low
            End If
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(SECURITY1.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Security1 Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Camera1()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Select Case recbuf(CAMERA1.subtype)
                Case CAMERA1.Ninja 'X10 Ninja/Robocam
                    Select Case recbuf(CAMERA1.cmnd)
                        Case CAMERA1.sLeft : valeur = "Left"
                        Case CAMERA1.sRight : valeur = "Right"
                        Case CAMERA1.sUp : valeur = "Up"
                        Case CAMERA1.sDown : valeur = "Down"
                        Case CAMERA1.sPosition1 : valeur = "Position 1"
                        Case CAMERA1.sProgramPosition1 : valeur = "Position 1 program"
                        Case CAMERA1.sPosition2 : valeur = ("Position 2")
                        Case CAMERA1.sProgramPosition2 : valeur = "Position 2 program"
                        Case CAMERA1.sPosition3 : valeur = "Position 3"
                        Case CAMERA1.sProgramPosition3 : valeur = "Position 3 program"
                        Case CAMERA1.sPosition4 : valeur = "Position 4"
                        Case CAMERA1.sProgramPosition4 : valeur = "Position 4 program"
                        Case CAMERA1.sCenter : valeur = "Center"
                        Case CAMERA1.sProgramCenterPosition : valeur = "Center program"
                        Case CAMERA1.sSweep : valeur = "Sweep"
                        Case CAMERA1.sProgramSweep : valeur = "Sweep program"
                        Case Else : valeur = "UNKNOWN"
                    End Select
                    adresse = Chr(recbuf(CAMERA1.housecode))
                    WriteRetour(adresse, "", valeur)
                    If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(CAMERA1.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
                Case Else : WriteLog("ERR: decode_Camera1 : Unknown Sub type for Packet type=" & Hex(recbuf(CAMERA1.packettype)) & ": " & Hex(recbuf(CAMERA1.subtype)))
            End Select
        Catch ex As Exception
            WriteLog("ERR: decode_Camera1 Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_BLINDS1()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            'Select Case recbuf(BLINDS1.subtype)
            '    Case BLINDS1.BlindsT0
            '        WriteMessage("subtype       = Safy / RollerTrol / Hasta new")
            '    Case BLINDS1.BlindsT1
            '        WriteMessage("subtype       = Hasta old")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(BLINDS1.packettype)) & ": " & Hex(recbuf(BLINDS1.subtype)))
            'End Select
            'WriteMessage("Sequence nbr  = " & recbuf(BLINDS1.seqnbr).ToString)
            adresse = VB.Right("0" & Hex(recbuf(BLINDS1.id1)), 2) & VB.Right("0" & Hex(recbuf(BLINDS1.id2)), 2) & VB.Right("0" & Hex(recbuf(BLINDS1.id3)), 2)
            'If recbuf(BLINDS1.unitcode) = 15 Then
            '    WriteMessage("Unit          = All")
            'Else
            '    WriteMessage("Unit          = " & recbuf(BLINDS1.unitcode).ToString)
            'End If

            Select Case recbuf(BLINDS1.cmnd)
                Case BLINDS1.sOpen : valeur = "OPEN"
                Case BLINDS1.sStop : valeur = "STOP"
                Case BLINDS1.sClose : valeur = "CLOSE"
                Case BLINDS1.sConfirm : valeur = "CONFIRM"
                Case BLINDS1.sLimit : valeur = "Set Limit"
                Case Else : valeur = "UNKNOWN"
            End Select
            WriteRetour(adresse, "", valeur)
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(BLINDS1.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_BLINDS1 Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Remote()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""

            Select recbuf(REMOTE.subtype)
                Case REMOTE.ATI 'ATI Remote Wonder
                    adresse = recbuf(REMOTE.id).ToString
                    Select Case recbuf(REMOTE.cmnd)
                        Case &H0 : valeur = "A"
                        Case &H1 : valeur = "B"
                        Case &H2 : valeur = "power"
                        Case &H3 : valeur = "TV"
                        Case &H4 : valeur = "DVD"
                        Case &H5 : valeur = "?"
                        Case &H6 : valeur = "Guide"
                        Case &H7 : valeur = "Drag"
                        Case &H8 : valeur = "VOL+"
                        Case &H9 : valeur = "VOL-"
                        Case &HA : valeur = "MUTE"
                        Case &HB : valeur = "CHAN+"
                        Case &HC : valeur = "CHAN-"
                        Case &HD : valeur = "1"
                        Case &HE : valeur = "2"
                        Case &HF : valeur = "3"
                        Case &H10 : valeur = "4"
                        Case &H11 : valeur = "5"
                        Case &H12 : valeur = "6"
                        Case &H13 : valeur = "7"
                        Case &H14 : valeur = "8"
                        Case &H15 : valeur = "9"
                        Case &H16 : valeur = "txt"
                        Case &H17 : valeur = "0"
                        Case &H18 : valeur = "snapshot ESC"
                        Case &H19 : valeur = "C"
                        Case &H1A : valeur = "^"
                        Case &H1B : valeur = "D"
                        Case &H1C : valeur = "TV/RADIO"
                        Case &H1D : valeur = "<"
                        Case &H1E : valeur = "OK"
                        Case &H1F : valeur = ">"
                        Case &H20 : valeur = "<-"
                        Case &H21 : valeur = "E"
                        Case &H22 : valeur = "v"
                        Case &H23 : valeur = "F"
                        Case &H24 : valeur = "Rewind"
                        Case &H25 : valeur = "Play"
                        Case &H26 : valeur = "Fast forward"
                        Case &H27 : valeur = "Record"
                        Case &H28 : valeur = "Stop"
                        Case &H29 : valeur = "Pause"
                        Case &H2C : valeur = "TV"
                        Case &H2D : valeur = "VCR"
                        Case &H2E : valeur = "RADIO"
                        Case &H2F : valeur = "TV Preview"
                        Case &H30 : valeur = "Channel list"
                        Case &H31 : valeur = "Video Desktop"
                        Case &H32 : valeur = "red"
                        Case &H33 : valeur = "green"
                        Case &H34 : valeur = "yellow"
                        Case &H35 : valeur = "blue"
                        Case &H36 : valeur = "rename TAB"
                        Case &H37 : valeur = "Acquire image"
                        Case &H38 : valeur = "edit image"
                        Case &H39 : valeur = "Full screen"
                        Case &H3A : valeur = "DVD Audio"
                        Case &H70 : valeur = "Cursor-left"
                        Case &H71 : valeur = "Cursor-right"
                        Case &H72 : valeur = "Cursor-up"
                        Case &H73 : valeur = "Cursor-down"
                        Case &H74 : valeur = "Cursor-up-left"
                        Case &H75 : valeur = "Cursor-up-right"
                        Case &H76 : valeur = "Cursor-down-right"
                        Case &H77 : valeur = "Cursor-down-left"
                        Case &H78 : valeur = "V"
                        Case &H79 : valeur = "V-End"
                        Case &H7C : valeur = "X"
                        Case &H7D : valeur = "X-End"
                        Case Else : valeur = "unknown"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case REMOTE.ATIplus 'ATI Remote Wonder Plus
                    adresse = recbuf(REMOTE.id).ToString
                    Select Case recbuf(REMOTE.cmnd)
                        Case &H0 : valeur = "A"
                        Case &H1 : valeur = "B"
                        Case &H2 : valeur = "power"
                        Case &H3 : valeur = "TV"
                        Case &H4 : valeur = "DVD"
                        Case &H5 : valeur = "?"
                        Case &H6 : valeur = "Guide"
                        Case &H7 : valeur = "Drag"
                        Case &H8 : valeur = "VOL+"
                        Case &H9 : valeur = "VOL-"
                        Case &HA : valeur = "MUTE"
                        Case &HB : valeur = "CHAN+"
                        Case &HC : valeur = "CHAN-"
                        Case &HD : valeur = "1"
                        Case &HE : valeur = "2"
                        Case &HF : valeur = "3"
                        Case &H10 : valeur = "4"
                        Case &H11 : valeur = "5"
                        Case &H12 : valeur = "6"
                        Case &H13 : valeur = "7"
                        Case &H14 : valeur = "8"
                        Case &H15 : valeur = "9"
                        Case &H16 : valeur = "txt"
                        Case &H17 : valeur = "0"
                        Case &H18 : valeur = "Open Setup Menu"
                        Case &H19 : valeur = "C"
                        Case &H1A : valeur = "^"
                        Case &H1B : valeur = "D"
                        Case &H1C : valeur = "FM"
                        Case &H1D : valeur = "<"
                        Case &H1E : valeur = "OK"
                        Case &H1F : valeur = ">"
                        Case &H20 : valeur = "Max/Restore window"
                        Case &H21 : valeur = "E"
                        Case &H22 : valeur = "v"
                        Case &H23 : valeur = "F"
                        Case &H24 : valeur = "Rewind"
                        Case &H25 : valeur = "Play"
                        Case &H26 : valeur = "Fast forward"
                        Case &H27 : valeur = "Record"
                        Case &H28 : valeur = "Stop"
                        Case &H29 : valeur = "Pause"
                        Case &H2A : valeur = "TV2"
                        Case &H2B : valeur = "Clock"
                        Case &H2C : valeur = "i"
                        Case &H2D : valeur = "ATI"
                        Case &H2E : valeur = "RADIO"
                        Case &H2F : valeur = "TV Preview"
                        Case &H30 : valeur = "Channel list"
                        Case &H31 : valeur = "Video Desktop"
                        Case &H32 : valeur = "red"
                        Case &H33 : valeur = "green"
                        Case &H34 : valeur = "yellow"
                        Case &H35 : valeur = "blue"
                        Case &H36 : valeur = "rename TAB"
                        Case &H37 : valeur = "Acquire image"
                        Case &H38 : valeur = "edit image"
                        Case &H39 : valeur = "Full screen"
                        Case &H3A : valeur = "DVD Audio"
                        Case &H70 : valeur = "Cursor-left"
                        Case &H71 : valeur = "Cursor-right"
                        Case &H72 : valeur = "Cursor-up"
                        Case &H73 : valeur = "Cursor-down"
                        Case &H74 : valeur = "Cursor-up-left"
                        Case &H75 : valeur = "Cursor-up-right"
                        Case &H76 : valeur = "Cursor-down-right"
                        Case &H77 : valeur = "Cursor-down-left"
                        Case &H78 : valeur = "Left Mouse Button"
                        Case &H79 : valeur = "V-End"
                        Case &H7C : valeur = "Right Mouse Button"
                        Case &H7D : valeur = "X-End"
                        Case Else : valeur = "unknown"
                    End Select
                    '        If (recbuf(REMOTE.toggle) And &H1) = &H1 Then
                    '            WriteMessage("  (button press = odd)")
                    '        Else
                    '            WriteMessage("  (button press = even)")
                    '        End If
                    WriteRetour(adresse, "", valeur)
                Case REMOTE.ATIrw2 'ATI Remote Wonder II
                    adresse = recbuf(REMOTE.id).ToString
                    'Select Case recbuf(REMOTE.cmndtype) And &HE
                    '    Case &H0 : WriteMessage("PC")
                    '    Case &H2 : WriteMessage("AUX1")
                    '    Case &H4 : WriteMessage("AUX2")
                    '    Case &H6 : WriteMessage("AUX3")
                    '    Case &H8 : WriteMessage("AUX4")
                    '    Case Else : WriteMessage("unknown")
                    'End Select
                    Select Case recbuf(REMOTE.cmnd)
                        Case &H0 : valeur = "A"
                        Case &H1 : valeur = "B"
                        Case &H2 : valeur = "power"
                        Case &H3 : valeur = "TV"
                        Case &H4 : valeur = "DVD"
                        Case &H5 : valeur = "?"
                        Case &H6 : valeur = "Guide"
                        Case &H7 : valeur = "Drag"
                        Case &H8 : valeur = "VOL+"
                        Case &H9 : valeur = "VOL-"
                        Case &HA : valeur = "MUTE"
                        Case &HB : valeur = "CHAN+"
                        Case &HC : valeur = "CHAN-"
                        Case &HD : valeur = "1"
                        Case &HE : valeur = "2"
                        Case &HF : valeur = "3"
                        Case &H10 : valeur = "4"
                        Case &H11 : valeur = "5"
                        Case &H12 : valeur = "6"
                        Case &H13 : valeur = "7"
                        Case &H14 : valeur = "8"
                        Case &H15 : valeur = "9"
                        Case &H16 : valeur = "txt"
                        Case &H17 : valeur = "0"
                        Case &H18 : valeur = "Open Setup Menu"
                        Case &H19 : valeur = "C"
                        Case &H1A : valeur = "^"
                        Case &H1B : valeur = "D"
                        Case &H1C : valeur = "FM"
                        Case &H1D : valeur = "<"
                        Case &H1E : valeur = "OK"
                        Case &H1F : valeur = ">"
                        Case &H20 : valeur = "Max/Restore window"
                        Case &H21 : valeur = "E"
                        Case &H22 : valeur = "v"
                        Case &H23 : valeur = "F"
                        Case &H24 : valeur = "Rewind"
                        Case &H25 : valeur = "Play"
                        Case &H26 : valeur = "Fast forward"
                        Case &H27 : valeur = "Record"
                        Case &H28 : valeur = "Stop"
                        Case &H29 : valeur = "Pause"
                        Case &H2C : valeur = "i"
                        Case &H2D : valeur = "ATI"
                        Case &H3B : valeur = "PC"
                        Case &H3C : valeur = "AUX1"
                        Case &H3D : valeur = "AUX2"
                        Case &H3E : valeur = "AUX3"
                        Case &H3F : valeur = "AUX4"
                        Case &H70 : valeur = "Cursor-left"
                        Case &H71 : valeur = "Cursor-right"
                        Case &H72 : valeur = "Cursor-up"
                        Case &H73 : valeur = "Cursor-down"
                        Case &H74 : valeur = "Cursor-up-left"
                        Case &H75 : valeur = "Cursor-up-right"
                        Case &H76 : valeur = "Cursor-down-right"
                        Case &H77 : valeur = "Cursor-down-left"
                        Case &H78 : valeur = "Left Mouse Button"
                        Case &H7C : valeur = "Right Mouse Button"
                        Case Else : valeur = "unknown"
                    End Select
                    '        If (recbuf(REMOTE.toggle) And &H1) = &H1 Then
                    '            WriteMessage("  (button press = odd)")
                    '        Else
                    '            WriteMessage("  (button press = even)")
                    '        End If
                    WriteRetour(adresse, "", valeur)
                Case REMOTE.Medion 'Medion Remote
                    adresse = recbuf(REMOTE.id).ToString
                    Select Case recbuf(REMOTE.cmnd)
                        Case &H0 : valeur = "Mute"
                        Case &H1 : valeur = "B"
                        Case &H2 : valeur = "power"
                        Case &H3 : valeur = "TV"
                        Case &H4 : valeur = "DVD"
                        Case &H5 : valeur = "Photo"
                        Case &H6 : valeur = "Music"
                        Case &H7 : valeur = "Drag"
                        Case &H8 : valeur = "VOL-"
                        Case &H9 : valeur = "VOL+"
                        Case &HA : valeur = "MUTE"
                        Case &HB : valeur = "CHAN+"
                        Case &HC : valeur = "CHAN-"
                        Case &HD : valeur = "1"
                        Case &HE : valeur = "2"
                        Case &HF : valeur = "3"
                        Case &H10 : valeur = "4"
                        Case &H11 : valeur = "5"
                        Case &H12 : valeur = "6"
                        Case &H13 : valeur = "7"
                        Case &H14 : valeur = "8"
                        Case &H15 : valeur = "9"
                        Case &H16 : valeur = "txt"
                        Case &H17 : valeur = "0"
                        Case &H18 : valeur = "snapshot ESC"
                        Case &H19 : valeur = "DVD MENU"
                        Case &H1A : valeur = "^"
                        Case &H1B : valeur = "Setup"
                        Case &H1C : valeur = "TV/RADIO"
                        Case &H1D : valeur = "<"
                        Case &H1E : valeur = "OK"
                        Case &H1F : valeur = ">"
                        Case &H20 : valeur = "<-"
                        Case &H21 : valeur = "E"
                        Case &H22 : valeur = "v"
                        Case &H23 : valeur = "F"
                        Case &H24 : valeur = "Rewind"
                        Case &H25 : valeur = "Play"
                        Case &H26 : valeur = "Fast forward"
                        Case &H27 : valeur = "Record"
                        Case &H28 : valeur = "Stop"
                        Case &H29 : valeur = "Pause"
                        Case &H2C : valeur = "V"
                        Case &H2D : valeur = "VCR"
                        Case &H2E : valeur = "RADIO"
                        Case &H2F : valeur = "TV Preview"
                        Case &H30 : valeur = "Channel list"
                        Case &H31 : valeur = "Video Desktop"
                        Case &H32 : valeur = "red"
                        Case &H33 : valeur = "green"
                        Case &H34 : valeur = "yellow"
                        Case &H35 : valeur = "blue"
                        Case &H36 : valeur = "rename TAB"
                        Case &H37 : valeur = "Acquire image"
                        Case &H38 : valeur = "edit image"
                        Case &H39 : valeur = "Full screen"
                        Case &H3A : valeur = "DVD Audio"
                        Case &H70 : valeur = "Cursor-left"
                        Case &H71 : valeur = "Cursor-right"
                        Case &H72 : valeur = "Cursor-up"
                        Case &H73 : valeur = "Cursor-down"
                        Case &H74 : valeur = "Cursor-up-left"
                        Case &H75 : valeur = "Cursor-up-right"
                        Case &H76 : valeur = "Cursor-down-right"
                        Case &H77 : valeur = "Cursor-down-left"
                        Case &H78 : valeur = "V"
                        Case &H79 : valeur = "V-End"
                        Case &H7C : valeur = "X"
                        Case &H7D : valeur = "X-End"
                        Case Else : valeur = "unknown"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case REMOTE.PCremote 'PC Remote
                    adresse = recbuf(REMOTE.id).ToString
                    Select Case recbuf(REMOTE.cmnd)
                        Case &H2 : valeur = "0"
                        Case &H82 : valeur = "1"
                        Case &HD1 : valeur = "MP3"
                        Case &H42 : valeur = "2"
                        Case &HD2 : valeur = "DVD"
                        Case &HC2 : valeur = "3"
                        Case &HD3 : valeur = "CD"
                        Case &H22 : valeur = "4"
                        Case &HD4 : valeur = "PC or SHIFT-4"
                        Case &HA2 : valeur = "5"
                        Case &HD5 : valeur = "SHIFT-5"
                        Case &H62 : valeur = "6"
                        Case &HE2 : valeur = "7"
                        Case &H12 : valeur = "8"
                        Case &H92 : valeur = "9"
                        Case &HC0 : valeur = "CH-"
                        Case &H40 : valeur = "CH+"
                        Case &HE0 : valeur = "VOL-"
                        Case &H60 : valeur = "VOL+"
                        Case &HA0 : valeur = "MUTE"
                        Case &H3A : valeur = "INFO"
                        Case &H38 : valeur = "REW"
                        Case &HB8 : valeur = "FF"
                        Case &HB0 : valeur = "PLAY"
                        Case &H64 : valeur = "PAUSE"
                        Case &H63 : valeur = "STOP"
                        Case &HB6 : valeur = "MENU"
                        Case &HFF : valeur = "REC"
                        Case &HC9 : valeur = "EXIT"
                        Case &HD8 : valeur = "TEXT"
                        Case &HD9 : valeur = "SHIFT-TEXT"
                        Case &HF2 : valeur = "TELETEXT"
                        Case &HD7 : valeur = "SHIFT-TELETEXT"
                        Case &HBA : valeur = "A+B"
                        Case &H52 : valeur = "ENT"
                        Case &HD6 : valeur = "SHIFT-ENT"
                        Case &H70 : valeur = "Cursor-left"
                        Case &H71 : valeur = "Cursor-right"
                        Case &H72 : valeur = "Cursor-up"
                        Case &H73 : valeur = "Cursor-down"
                        Case &H74 : valeur = "Cursor-up-left"
                        Case &H75 : valeur = "Cursor-up-right"
                        Case &H76 : valeur = "Cursor-down-right"
                        Case &H77 : valeur = "Cursor-down-left"
                        Case &H78 : valeur = "Left mouse"
                        Case &H79 : valeur = "Left mouse-End"
                        Case &H7B : valeur = "Drag"
                        Case &H7C : valeur = "Right mouse"
                        Case &H7D : valeur = "Right mouse-End"
                        Case Else : valeur = "unknown"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case Else : WriteLog("ERR: decode_Remote : Unknown Sub type for Packet type=" & Hex(recbuf(REMOTE.packettype)) & ": " & Hex(recbuf(REMOTE.subtype)))
            End Select
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(REMOTE.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Remote Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Thermostat1()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            'Select Case recbuf(THERMOSTAT1.subtype)
            '    Case THERMOSTAT1.Digimax
            '        WriteMessage("subtype       = Digimax")
            '    Case THERMOSTAT1.DigimaxShort
            '        WriteMessage("subtype       = Digimax with short format")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(THERMOSTAT1.packettype)) & ":" & Hex(recbuf(THERMOSTAT1.subtype)))
            'End Select
            adresse = ((recbuf(THERMOSTAT1.id1) * 256 + recbuf(THERMOSTAT1.id2))).ToString

            valeur = recbuf(THERMOSTAT1.temperature).ToString '°C
            WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)

            If recbuf(THERMOSTAT1.subtype) = THERMOSTAT1.Digimax Then
                valeur = recbuf(THERMOSTAT1.set_point).ToString '°C
                WriteRetour(adresse, ListeDevices.TEMPERATURECONSIGNE.ToString, valeur)

                '    If (recbuf(THERMOSTAT1.mode) And &H80) = 0 Then
                '        WriteMessage("Mode          = heating")
                '    Else
                '        WriteMessage("Mode          = Cooling")
                '    End If
                '    Select Case (recbuf(THERMOSTAT1.status) And &H3)
                '        Case 0
                '            WriteMessage("Status        = no status available")
                '        Case 1
                '            WriteMessage("Status        = demand")
                '        Case 2
                '            WriteMessage("Status        = no demand")
                '        Case 3
                '            WriteMessage("Status        = initializing")
                '    End Select
            End If
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(THERMOSTAT1.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Thermostat1 Exception : " & ex.Message)
        End Try
    End Sub
    'Not implemented
    Private Sub decode_Thermostat2()
        'WriteMessage("Not implemented")
    End Sub

    Private Sub decode_Thermostat3()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            'Select Case recbuf(THERMOSTAT3.subtype)
            '    Case THERMOSTAT3.MertikG6RH4T1
            '        WriteMessage("subtype       = Mertik G6R-H4T1")
            '    Case THERMOSTAT3.MertikG6RH4TB
            '        WriteMessage("subtype       = Mertik G6R-H4TB")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(THERMOSTAT3.packettype)) & ":" & Hex(recbuf(THERMOSTAT3.subtype)))
            'End Select
            adresse = VB.Right("0" & Hex(recbuf(THERMOSTAT3.unitcode1)), 2) & VB.Right("0" & Hex(recbuf(THERMOSTAT3.unitcode2)), 2) & VB.Right("0" & Hex(recbuf(THERMOSTAT3.unitcode3)), 2)

            Select Case recbuf(THERMOSTAT3.cmnd)
                Case 0 : valeur = "OFF"
                Case 1 : valeur = "ON"
                Case 2 : valeur = "UP"
                Case 3 : valeur = "DOWN"
                Case 4 : If recbuf(THERMOSTAT3.subtype) = THERMOSTAT3.MertikG6RH4T1 Then valeur = "RUN_UP" Else valeur = "2ND_OFF"
                Case 5 : If recbuf(THERMOSTAT3.subtype) = THERMOSTAT3.MertikG6RH4T1 Then valeur = "RUN_DOWN" Else valeur = "2ND_ON"
                Case 6 : If recbuf(THERMOSTAT3.subtype) = THERMOSTAT3.MertikG6RH4T1 Then valeur = "STOP" Else valeur = "UNKNOWN"
                Case Else : valeur = "UNKNOWN"
            End Select
            WriteRetour(adresse, "", valeur)

            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(THERMOSTAT3.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Thermostat3 Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Temp()
        Try
            'Select Case recbuf(TEMP.subtype)
            '    Case TEMP.TEMP1
            '        WriteMessage("subtype       = TEMP1 - THR128/138, THC138")
            '    Case TEMP.TEMP2
            '        WriteMessage("subtype       = TEMP2 - THC238/268,THN132,THWR288,THRN122,THN122,AW129/131")
            '    Case TEMP.TEMP3
            '        WriteMessage("subtype       = TEMP3 - THWR800")
            '    Case TEMP.TEMP4
            '        WriteMessage("subtype       = TEMP4 - RTHN318")
            '    Case TEMP.TEMP5
            '        WriteMessage("subtype       = TEMP5 - LaCrosse TX3, TX4, TX17")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(TEMP.packettype)) & ":" & Hex(recbuf(TEMP.subtype)))
            'End Select
            'WriteMessage("Sequence nbr  = " & recbuf(TEMP.seqnbr).ToString)
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(TEMP.id1) * 256 + recbuf(TEMP.id2)).ToString
            If (recbuf(TEMP.tempsign) And &H80) = 0 Then
                valeur = Math.Round((recbuf(TEMP.temperatureh) * 256 + recbuf(TEMP.temperaturel)) / 10, 2).ToString
            Else
                valeur = Math.Round(((recbuf(TEMP.temperatureh) And &H7F) * 256 + recbuf(TEMP.temperaturel)) / 10, 2).ToString
            End If
            WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
            If (recbuf(TEMP.battery_level) And &HF) = 0 Then WriteBattery(adresse) 'battery low
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(TEMP.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Temp Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Hum()
        Try
            'Select Case recbuf(HUM.subtype)
            '    Case HUM.HUM1
            '        WriteMessage("subtype       = HUM1 - LaCrosse TX3")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(HUM.packettype)) & ":" & Hex(recbuf(HUM.subtype)))
            'End Select
            'WriteMessage("Sequence nbr  = " & recbuf(HUM.seqnbr).ToString)
            'Select Case recbuf(HUM.humidity_status)
            '    Case &H0
            '        WriteMessage("Status        = Normal")
            '    Case &H1
            '        WriteMessage("Status        = Comfortable")
            '    Case &H2
            '        WriteMessage("Status        = Dry")
            '    Case &H3
            '        WriteMessage("Status        = Wet")
            'End Select
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(HUM.id1) * 256 + recbuf(HUM.id2)).ToString
            valeur = recbuf(HUM.humidity).ToString
            WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)
            If (recbuf(HUM.battery_level) And &HF) = 0 Then WriteBattery(adresse) 'battery low
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(HUM.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Hum Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_TempHum()
        Try
            '   Case TEMP_HUM.TH1
            '    WriteMessage("subtype       = TH1 - THGN122/123,/THGN132,THGR122/228/238/268")
            '    WriteMessage("                channel " & recbuf(TEMP_HUM.id2).ToString)
            '    Case TEMP_HUM.TH2
            '    WriteMessage("subtype       = TH2 - THGR810,THGN800")
            '    WriteMessage("                channel " & recbuf(TEMP_HUM.id2).ToString)
            '    Case TEMP_HUM.TH3
            '    WriteMessage("subtype       = TH3 - RTGR328")
            '    WriteMessage("                channel " & recbuf(TEMP_HUM.id2).ToString)
            '    Case TEMP_HUM.TH4
            '    WriteMessage("subtype       = TH4 - THGR328")
            '    WriteMessage("                channel " & recbuf(TEMP_HUM.id2).ToString)
            '    Case TEMP_HUM.TH5
            '    WriteMessage("subtype       = TH5 - WTGR800")
            '    Case TEMP_HUM.TH6
            '    WriteMessage("subtype       = TH6 - THGR918,THGRN228,THGN500")
            '    WriteMessage("                channel " & recbuf(TEMP_HUM.id2).ToString)
            '    Case TEMP_HUM.TH7
            '    WriteMessage("subtype       = TH7 - Cresta, TFA TS34C")
            '    If recbuf(TEMP_HUM.id1) < &H40 Then
            '        WriteMessage("                channel 1")
            '    ElseIf recbuf(TEMP_HUM.id1) < &H60 Then
            '        WriteMessage("                channel 2")
            '    ElseIf recbuf(TEMP_HUM.id1) < &H80 Then
            '        WriteMessage("                channel 3")
            '    ElseIf recbuf(TEMP_HUM.id1) > &H9F And TEMP_HUM.id1 < &HC0 Then
            '        WriteMessage("                channel 4")
            '    ElseIf recbuf(TEMP_HUM.id1) < &HE0 Then
            '        WriteMessage("                channel 5")
            '    Else
            '        WriteMessage("                channel ??")
            '    End If
            '    Case TEMP_HUM.TH8
            '    WriteMessage("subtype       = TH8 - WT260,WT260H,WT440H,WT450,WT450H")
            '    WriteMessage("                channel " & recbuf(TEMP_HUM.id2).ToString)
            '    Case TEMP_HUM.TH9
            '    WriteMessage("subtype       = TH9 - Viking 02038, 02035 (02035 has no humidity)")
            '    Case Else
            '    WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(TEMP_HUM.packettype)) & ":" & Hex(recbuf(TEMP_HUM.subtype)))
            'End Select
            'WriteMessage("Sequence nbr  = " & recbuf(TEMP_HUM.seqnbr).ToString)
            'Select Case recbuf(TEMP_HUM.humidity_status)
            '    Case &H0
            '        WriteMessage("Status        = Normal")
            '    Case &H1
            '        WriteMessage("Status        = Comfortable")
            '    Case &H2
            '        WriteMessage("Status        = Dry")
            '    Case &H3
            '        WriteMessage("Status        = Wet")
            'End Select
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(TEMP_HUM.id1) * 256 + recbuf(TEMP_HUM.id2)).ToString
            If (recbuf(TEMP_HUM.tempsign) And &H80) = 0 Then
                valeur = Math.Round((recbuf(TEMP_HUM.temperatureh) * 256 + recbuf(TEMP_HUM.temperaturel)) / 10, 2).ToString
            Else
                valeur = Math.Round(((recbuf(TEMP_HUM.temperatureh) And &H7F) * 256 + recbuf(TEMP_HUM.temperaturel)) / 10, 2).ToString
            End If
            WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
            valeur = recbuf(TEMP_HUM.humidity).ToString
            WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)
            If recbuf(TEMP_HUM.subtype) = TEMP_HUM.TH6 Then
                If recbuf(TEMP_HUM.battery_level) = 0 Then WriteBattery(adresse) 'battery low < 10% (1=20% 2=30%....9=100%)
            Else
                If (recbuf(TEMP_HUM.battery_level) And &HF) = 0 Then WriteBattery(adresse) 'battery low
            End If
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(TEMP_HUM.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_TempHum Exception : " & ex.Message)
        End Try
    End Sub
    'Not implemented
    Private Sub decode_Baro()
        'WriteMessage("Not implemented")
    End Sub

    Private Sub decode_TempHumBaro()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(TEMP_HUM_BARO.id1) * 256 + recbuf(TEMP_HUM_BARO.id2)).ToString
            'Select Case recbuf(TEMP_HUM_BARO.subtype)
            '    Case TEMP_HUM_BARO.THB1
            '        WriteMessage("subtype       = THB1 - BTHR918")
            '    Case TEMP_HUM_BARO.THB2
            '        WriteMessage("subtype       = THB2 - BTHR918N, BTHR968")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(TEMP_HUM_BARO.packettype)) & ":" & Hex(recbuf(TEMP_HUM_BARO.subtype)))
            'End Select

            If (TEMP_HUM_BARO.tempsign And &H80) = 0 Then
                valeur = Math.Round((recbuf(TEMP_HUM_BARO.temperatureh) * 256 + recbuf(TEMP_HUM_BARO.temperaturel)) / 10, 2).ToString '°C
            Else
                valeur = Math.Round(((recbuf(TEMP_HUM_BARO.temperatureh) And &H7F) * 256 + recbuf(TEMP_HUM_BARO.temperaturel)) / 10, 2).ToString '°C
            End If
            WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)

            valeur = recbuf(TEMP_HUM_BARO.humidity).ToString
            WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)

            'Select Case recbuf(TEMP_HUM_BARO.humidity_status)
            '    Case &H0
            '        WriteMessage("Status        = Normal")
            '    Case &H1
            '        WriteMessage("Status        = Comfortable")
            '    Case &H2
            '        WriteMessage("Status        = Dry")
            '    Case &H3
            '        WriteMessage("Status        = Wet")
            'End Select

            valeur = (recbuf(TEMP_HUM_BARO.baroh) * 256 + recbuf(TEMP_HUM_BARO.barol)).ToString
            WriteRetour(adresse, ListeDevices.BAROMETRE.ToString, valeur)

            'Select Case recbuf(TEMP_HUM_BARO.forecast)
            '    Case &H0
            '        WriteMessage("Forecast      = No information available")
            '    Case &H1
            '        WriteMessage("Forecast      = Sunny")
            '    Case &H2
            '        WriteMessage("Forecast      = Partly Cloudy")
            '    Case &H3
            '        WriteMessage("Forecast      = Cloudy")
            '    Case &H4
            '        WriteMessage("Forecast      = Rain")
            'End Select

            If (recbuf(TEMP_HUM_BARO.battery_level) And &HF) = 0 Then WriteBattery(adresse) 'battery low
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(TEMP_HUM_BARO.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_TempHumBaro Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Rain()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(RAIN.id1) * 256 + recbuf(RAIN.id2)).ToString
            'Select Case recbuf(RAIN.subtype)
            '    Case RAIN.RAIN1
            '        WriteMessage("subtype       = RAIN1 - RGR126/682/918")
            '    Case RAIN.RAIN2
            '        WriteMessage("subtype       = RAIN2 - PCR800")
            '    Case RAIN.RAIN3
            '        WriteMessage("subtype       = RAIN3 - TFA")
            '    Case RAIN.RAIN4
            '        WriteMessage("subtype       = RAIN4 - UPM RG700")
            '    Case RAIN.RAIN5
            '        WriteMessage("subtype       = RAIN5 - LaCrosse WS2300")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(RAIN.packettype)) & ":" & Hex(recbuf(RAIN.subtype)))
            'End Select
            If recbuf(RAIN.subtype) = RAIN.RAIN1 Then
                valeur = ((recbuf(RAIN.rainrateh) * 256) + recbuf(RAIN.rainratel)).ToString 'mm/h
                WriteRetour(adresse, ListeDevices.PLUIECOURANT.ToString, valeur)
            ElseIf recbuf(RAIN.subtype) = RAIN.RAIN2 Then
                valeur = (((recbuf(RAIN.rainrateh) * 256) + recbuf(RAIN.rainratel)) / 100).ToString ' mm/h
                WriteRetour(adresse, ListeDevices.PLUIECOURANT.ToString, valeur)
            End If


            valeur = ((recbuf(RAIN.raintotal1) * 65535) + recbuf(RAIN.raintotal2) * 256 + recbuf(RAIN.raintotal3)).ToString 'mm
            WriteRetour(adresse, ListeDevices.PLUIETOTAL.ToString, valeur)
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(RAIN.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
            If (recbuf(RAIN.battery_level) And &HF) = 0 Then WriteBattery(adresse) 'battery low
        Catch ex As Exception
            WriteLog("ERR: decode_Rain Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Wind()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(WIND.id1) * 256 + recbuf(WIND.id2)).ToString
            'Select Case recbuf(WIND.subtype)
            '    Case WIND.WIND1
            '        WriteMessage("subtype       = WIND1 - WTGR800")
            '    Case WIND.WIND2
            '        WriteMessage("subtype       = WIND2 - WGR800")
            '    Case WIND.WIND3
            '        WriteMessage("subtype       = WIND3 - STR918, WGR918")
            '    Case WIND.WIND4
            '        WriteMessage("subtype       = WIND4 - TFA")
            '    Case WIND.WIND5
            '        WriteMessage("subtype       = WIND5 - UPM WDS500")
            '    Case WIND.WIND6
            '        WriteMessage("subtype       = WIND6 - LaCrosse WS2300")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(WIND.packettype)) & ":" & Hex(recbuf(WIND.subtype)))
            'End Select

            valeur = ((recbuf(WIND.directionh) * 256) + recbuf(WIND.directionl)).ToString 'direction en degré
            WriteRetour(adresse, ListeDevices.DIRECTIONVENT.ToString, valeur)
            If recbuf(WIND.subtype) <> WIND.WIND5 Then
                'valeur = (((recbuf(WIND.av_speedh) * 256) + recbuf(WIND.av_speedl)) / 10).ToString 'Vitesse en mtr/sec
                valeur = (((recbuf(WIND.av_speedh) * 256) + recbuf(WIND.av_speedl)) * 0.36).ToString 'Vitesse en km/hr
                WriteRetour(adresse, ListeDevices.VITESSEVENT.ToString, valeur)
            End If

            'WriteMessage("Wind gust     = " & (((recbuf(WIND.gusth) * 256) + recbuf(WIND.gustl)) / 10).ToString & " mtr/sec")

            If recbuf(WIND.subtype) = WIND.WIND4 Then
                If (WIND.tempsign And &H80) = 0 Then
                    valeur = Math.Round((recbuf(WIND.temperatureh) * 256 + recbuf(WIND.temperaturel)) / 10, 2).ToString '°C
                Else
                    valeur = Math.Round(((recbuf(WIND.temperatureh) And &H7F) * 256 + recbuf(WIND.temperaturel)) / 10, 2).ToString '°C
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)

                If (WIND.chillsign And &H80) = 0 Then
                    '        WriteMessage("Chill         = " & Math.Round((recbuf(WIND.chillh) * 256 + recbuf(WIND.chilll)) / 10, 2).ToString & " °C")
                Else
                    '        WriteMessage("Chill         = -" & Math.Round(((recbuf(WIND.chillh) And &H7F) * 256 + recbuf(WIND.chilll)) / 10, 2).ToString & " °C")
                End If
            End If

            If recbuf(WIND.subtype) = WIND.WIND3 Then
                If recbuf(WIND.battery_level) = 0 Then WriteBattery(adresse) 'battery low < 10% (1=20% 2=30%....9=100%)
            Else
                If (recbuf(WIND.battery_level) And &HF) = 0 Then WriteBattery(adresse) 'battery low
            End If
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(WIND.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Wind Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_UV()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(UV.id1) * 256 + recbuf(UV.id2)).ToString
            'Select Case recbuf(UV.subtype)
            '    Case UV.UV1
            '        WriteMessage("Subtype       = UV1 - UVN128, UV138")
            '    Case UV.UV2
            '        WriteMessage("Subtype       = UV2 - UVN800")
            '    Case UV.UV3
            '        WriteMessage("Subtype       = UV3 - TFA")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(UV.packettype) & ":" & Hex(recbuf(UV.subtype)))
            'End Select

            valeur = (recbuf(UV.uv) / 10).ToString
            WriteRetour(adresse, ListeDevices.UV.ToString, valeur)

            If recbuf(UV.subtype) = UV.UV3 Then
                If (UV.tempsign And &H80) = 0 Then
                    valeur = Math.Round((recbuf(UV.temperatureh) * 256 + recbuf(UV.temperaturel)) / 10, 2).ToString '°C
                Else
                    valeur = Math.Round(((recbuf(UV.temperatureh) And &H7F) * 256 + recbuf(UV.temperaturel)) / 10, 2).ToString '°C
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
            End If

            'If recbuf(UV.uv) < 3 Then
            '    WriteMessage("Description = Low")
            'ElseIf recbuf(UV.uv) < 6 Then
            '    WriteMessage("Description = Medium")
            'ElseIf recbuf(UV.uv) < 8 Then
            '    WriteMessage("Description = High")
            'ElseIf recbuf(UV.uv) < 11 Then
            '    WriteMessage("Description = Very high")
            'Else
            '    WriteMessage("Description = Dangerous")
            'End If

            If (recbuf(UV.battery_level) And &HF) = 0 Then WriteBattery(adresse) 'battery low
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(UV.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_UV Exception : " & ex.Message)
        End Try
    End Sub
    'Not implemented
    Private Sub decode_DateTime()
        Try

        Catch ex As Exception
            WriteLog("ERR: decode_DateTime Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Current()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(CURRENT.id1) * 256 + recbuf(CURRENT.id2)).ToString
            'Select Case recbuf(CURRENT.subtype)
            '    Case CURRENT.ELEC1
            '        WriteMessage("subtype       = ELEC1 - OWL CM113, Electrisave, cent-a-meter")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(CURRENT.packettype)) & ":" & Hex(recbuf(CURRENT.subtype)))
            'End Select
            'WriteMessage("Count         = " & recbuf(5).ToString)

            valeur = ((recbuf(CURRENT.ch1h) * 256 + recbuf(CURRENT.ch1l)) / 10).ToString 'ampere channel 1
            WriteRetour(adresse & "-1", "", valeur)
            valeur = ((recbuf(CURRENT.ch2h) * 256 + recbuf(CURRENT.ch2h)) / 10).ToString 'ampere channel 2
            WriteRetour(adresse & "-2", "", valeur)
            valeur = ((recbuf(CURRENT.ch3l) * 256 + recbuf(CURRENT.ch3l)) / 10).ToString 'ampere channel 3
            WriteRetour(adresse & "-3", "", valeur)

            If (recbuf(CURRENT.battery_level) And &HF) = 0 Then WriteBattery(adresse & "-1") 'battery low
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(CURRENT.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Current Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Energy()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(ENERGY.id1) * 256 + recbuf(ENERGY.id2)).ToString
            'Select Case recbuf(ENERGY.subtype)
            '    Case ENERGY.ELEC2
            '        WriteMessage("subtype       = ELEC2 - OWL CM119, CM160")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(ENERGY.packettype)) & ":" & Hex(recbuf(ENERGY.subtype)))
            'End Select

            'WriteMessage("Count         = " & recbuf(ENERGY.count).ToString)

            valeur = (CLng(recbuf(ENERGY.instant1)) * &H1000000 + recbuf(ENERGY.instant2) * &H10000 + recbuf(ENERGY.instant3) * &H100 + recbuf(ENERGY.instant4)).ToString 'Watt
            WriteRetour(adresse, ListeDevices.ENERGIEINSTANTANEE.ToString, valeur)
            valeur = ((CDbl(recbuf(ENERGY.total1)) * &H10000000000 + CDbl(recbuf(ENERGY.total2)) * &H100000000 + CDbl(recbuf(ENERGY.total3)) * &H1000000 + recbuf(ENERGY.total4) * &H10000 + recbuf(ENERGY.total5) * &H100 + recbuf(ENERGY.total6)) / 223.666).ToString 'Watt / h
            WriteRetour(adresse, ListeDevices.ENERGIETOTALE.ToString, valeur)

            If (recbuf(ENERGY.battery_level) And &HF) = 0 Then WriteBattery(adresse) 'battery low
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(ENERGY.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Energy Exception : " & ex.Message)
        End Try
    End Sub
    'Not implemented
    Private Sub decode_Gas()
        Try
            'WriteMessage("Not implemented")
        Catch ex As Exception
            WriteLog("ERR: decode_Gas Exception : " & ex.Message)
        End Try
    End Sub
    'Not implemented
    Private Sub decode_Water()
        Try
            'WriteMessage("Not implemented")
        Catch ex As Exception
            WriteLog("ERR: decode_Water Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_Weight()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            adresse = (recbuf(WEIGHT.id1) * 256 + recbuf(WEIGHT.id2)).ToString
            'Select Case recbuf(WEIGHT.subtype)
            '    Case WEIGHT.WEIGHT1
            '        WriteMessage("subtype       = BWR102")
            '    Case WEIGHT.WEIGHT2
            '        WriteMessage("subtype       = GR101")
            '    Case Else
            '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(WEIGHT.packettype)) & ":" & Hex(recbuf(WEIGHT.subtype)))
            'End Select

            valeur = ((recbuf(WEIGHT.weighthigh) * 25.6) + recbuf(WEIGHT.weightlow) / 10).ToString 'kg
            WriteRetour(adresse, "", valeur)

            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(WEIGHT.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_Weight Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_RFXSensor()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Select recbuf(RFXSENSOR.subtype)
                Case RFXSENSOR.Temp 'Temperature
                    adresse = recbuf(RFXSENSOR.id).ToString
                    If (recbuf(RFXSENSOR.msg1) And &H80) = 0 Then 'positive temperature?
                        valeur = Math.Round(((recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)) / 100), 2).ToString '°C
                    Else
                        valeur = Math.Round((0 - ((recbuf(RFXSENSOR.msg1) And &H7F) * 256 + recbuf(RFXSENSOR.msg2)) / 100), 2).ToString '°C
                    End If
                    WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                Case RFXSENSOR.AD 'A/D
                    adresse = recbuf(RFXSENSOR.id).ToString
                    valeur = (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString ' mV
                    WriteRetour(adresse, "", valeur)
                Case RFXSENSOR.Volt 'Voltage
                    adresse = recbuf(RFXSENSOR.id).ToString
                    valeur = (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString ' mV
                    WriteRetour(adresse, "", valeur)
                Case RFXSENSOR.Message 'Message
                    adresse = recbuf(RFXSENSOR.id).ToString
                    Select Case recbuf(RFXSENSOR.msg2)
                        Case &H1 : valeur = "sensor addresses incremented"
                        Case &H2 : valeur = "battery low detected"
                        Case &H81 : valeur = "no 1-wire device connected"
                        Case &H82 : valeur = "1-Wire ROM CRC error"
                        Case &H83 : valeur = "1-Wire device connected is not a DS18B20 or DS2438"
                        Case &H84 : valeur = "no end of read signal received from 1-Wire device"
                        Case &H85 : valeur = "1-Wire scratchpad CRC error"
                        Case Else : valeur = "ERROR: unknown message"
                    End Select
                    WriteLog("decode_RFXSensor Message : " & valeur & " de " & adresse)
                    valeur = (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString
                    WriteRetour(adresse, "", valeur)
                Case Else : WriteLog("ERR: decode_RFXSensor : Unknown Sub type for Packet type=" & Hex(recbuf(RFXSENSOR.packettype)) & ": " & Hex(recbuf(RFXSENSOR.subtype)))
            End Select
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(RFXSENSOR.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_RFXSensor Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_RFXMeter()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Dim counter As Long

            Select Case recbuf(RFXMETER.subtype)
                Case RFXMETER.Count
                    'WriteMessage("subtype       = RFXMeter counter")
                    'WriteMessage("Sequence nbr  = " & recbuf(RFXMETER.seqnbr).ToString)
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    counter = (CLng(recbuf(RFXMETER.count1)) << 24) + (CLng(recbuf(RFXMETER.count2)) << 16) + (CLng(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4)
                    valeur = counter.ToString 'WriteMessage("if RFXPwr     = " & (counter / 1000).ToString & " kWh")
                    WriteRetour(adresse, "", valeur)
                Case RFXMETER.Interval
                    'WriteMessage("subtype       = RFXMeter new interval time set")
                    'WriteMessage("Sequence nbr  = " & recbuf(RFXMETER.seqnbr).ToString)
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    Select Case recbuf(RFXMETER.count3)
                        Case &H1 : valeur = "Interval: 30 sec"
                        Case &H2 : valeur = "Interval: 1 min"
                        Case &H4 : valeur = "Interval: 6 min"
                        Case &H8 : valeur = "Interval: 12 min"
                        Case &H10 : valeur = "Interval: 15 min"
                        Case &H20 : valeur = "Interval: 30 min"
                        Case &H40 : valeur = "Interval: 45 min"
                        Case &H80 : valeur = "Interval: 60 min"
                        Case Else : valeur = "Interval: illegal value"
                    End Select
                    WriteRetour(adresse, "", "CFG: " & valeur)
                Case RFXMETER.Calib
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    counter = CLng(((CLng(recbuf(RFXMETER.count2) And &H3F) << 16) + (CLng(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4)) / 1000)
                    Select Case (recbuf(RFXMETER.count2) And &HC0)
                        Case &H0 : WriteRetour(adresse, "", "CFG: Calibrate mode for channel 1 : " & counter.ToString & " msec - RFXPwr        = " & Convert.ToString(Round(1 / ((16 * counter) / (3600000 / 62.5)), 3)) & " kW")
                        Case &H40 : WriteRetour(adresse, "", "CFG: Calibrate mode for channel 2 : " & counter.ToString & " msec - RFXPwr        = " & Convert.ToString(Round(1 / ((16 * counter) / (3600000 / 62.5)), 3)) & " kW")
                        Case &H80 : WriteRetour(adresse, "", "CFG: Calibrate mode for channel 3 : " & counter.ToString & " msec - RFXPwr        = " & Convert.ToString(Round(1 / ((16 * counter) / (3600000 / 62.5)), 3)) & " kW")
                    End Select
                Case RFXMETER.Addr
                    'WriteMessage("Sequence nbr  = " & recbuf(RFXMETER.seqnbr).ToString)
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    WriteRetour(adresse, "", "CFG: New address set, push button for next address")
                Case RFXMETER.CounterReset
                    'WriteMessage("Sequence nbr  = " & recbuf(RFXMETER.seqnbr).ToString)
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    Select Case (recbuf(RFXMETER.count2) And &HC0)
                        Case &H0 : WriteRetour(adresse, "", "CFG: Push the button for next mode within 5 seconds or else RESET COUNTER channel 1 will be executed")
                        Case &H40 : WriteRetour(adresse, "", "CFG: Push the button for next mode within 5 seconds or else RESET COUNTER channel 2 will be executed")
                        Case &H80 : WriteRetour(adresse, "", "CFG: Push the button for next mode within 5 seconds or else RESET COUNTER channel 3 will be executed")
                    End Select
                Case RFXMETER.CounterSet
                    'WriteMessage("Sequence nbr  = " & recbuf(RFXMETER.seqnbr).ToString)
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    valeur = ((CLng(recbuf(RFXMETER.count1)) << 24) + (CLng(recbuf(RFXMETER.count2)) << 16) + (CLng(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4)).ToString
                    Select Case (recbuf(RFXMETER.count2) And &HC0)
                        Case &H0 : WriteRetour(adresse, "", "CFG: Counter channel 1 is reset to zero - Valeur:" & valeur)
                        Case &H40 : WriteRetour(adresse, "", "CFG: Counter channel 2 is reset to zero - Valeur:" & valeur)
                        Case &H80 : WriteRetour(adresse, "", "CFG: Counter channel 3 is reset to zero - Valeur:" & valeur)
                    End Select
                Case RFXMETER.SetInterval
                    'WriteMessage("Sequence nbr  = " & recbuf(RFXMETER.seqnbr).ToString)
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    WriteRetour(adresse, "", "CFG: Push the button for next mode within 5 seconds or else SET INTERVAL MODE will be entered")
                Case RFXMETER.SetCalib
                    'WriteMessage("Sequence nbr  = " & recbuf(RFXMETER.seqnbr).ToString)
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    Select Case (recbuf(RFXMETER.count2) And &HC0)
                        Case &H0 : WriteRetour(adresse, "", "CFG: Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 1 will be executed")
                        Case &H40 : WriteRetour(adresse, "", "CFG: Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 2 will be executed")
                        Case &H80 : WriteRetour(adresse, "", "CFG: Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 3 will be executed")
                    End Select
                Case RFXMETER.SetAddr
                    'WriteMessage("Sequence nbr  = " & recbuf(RFXMETER.seqnbr).ToString)
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    WriteRetour(adresse, "", "CFG: Push the button for next mode within 5 seconds or else SET ADDRESS MODE will be entered")
                Case RFXMETER.Ident
                    'WriteMessage("subtype       = RFXMeter identification")
                    'WriteMessage("Sequence nbr  = " & recbuf(RFXMETER.seqnbr).ToString)
                    adresse = (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString
                    WriteRetour(adresse, "", "CFG: FW version" & Hex(recbuf(RFXMETER.count3)))
                    Select Case recbuf(RFXMETER.count4)
                        Case &H1 : valeur = "Interval: 30 sec"
                        Case &H2 : valeur = "Interval: 1 min"
                        Case &H4 : valeur = "Interval: 6 min"
                        Case &H8 : valeur = "Interval: 12 min"
                        Case &H10 : valeur = "Interval: 15 min"
                        Case &H20 : valeur = "Interval: 30 min"
                        Case &H40 : valeur = "Interval: 45 min"
                        Case &H80 : valeur = "Interval: 60 min"
                        Case Else : valeur = "Interval: illegal value"
                    End Select
                    WriteRetour(adresse, "", "CFG: " & valeur)
                Case Else
                    '        WriteMessage("ERROR: Unknown Sub type for Packet type=" & Hex(recbuf(RFXMETER.packettype)) & ":" & Hex(recbuf(RFXMETER.subtype)))
            End Select
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(RFXMETER.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_RFXMeter Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub decode_FS20()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Select Case recbuf(FS20.subtype)
                Case FS20.sTypeFS20
                    'WriteMessage("subtype       = FS20")
                    'WriteMessage("Sequence nbr  = " & recbuf(FS20.seqnbr).ToString)
                    'WriteMessage("House code    = " & VB.Right("0" & Hex(recbuf(FS20.hc1)), 2) & VB.Right("0" & Hex(recbuf(FS20.hc2)), 2))
                    'WriteMessage("Address       = " & VB.Right("0" & Hex(recbuf(FS20.addr)), 2))
                    adresse = VB.Right("0" & Hex(recbuf(FS20.hc1)), 2) & VB.Right("0" & Hex(recbuf(FS20.hc2)), 2) & VB.Right("0" & Hex(recbuf(FS20.addr)), 2)
                    Select Case (recbuf(FS20.cmd1) And &H1F)
                        Case &H0 : valeur = "0"
                        Case &H1 : valeur = "6.25"
                        Case &H2 : valeur = "12.5"
                        Case &H3 : valeur = "18.75"
                        Case &H4 : valeur = "2"
                        Case &H5 : valeur = "31.25"
                        Case &H6 : valeur = "37.5"
                        Case &H7 : valeur = "43.75"
                        Case &H8 : valeur = "50"
                        Case &H9 : valeur = "56.25"
                        Case &HA : valeur = "62.5"
                        Case &HB : valeur = "68.75"
                        Case &HC : valeur = "75"
                        Case &HD : valeur = "81.25"
                        Case &HE : valeur = "87.5"
                        Case &HF : valeur = "93.75"
                        Case &H10 : valeur = "100"
                        Case &H11 : valeur = "ON" 'On ( at last dim level set)"
                        Case &H12 : valeur = "TOGGLE" '"Toggle between Off and On (last dim level set)"
                        Case &H13 : valeur = "BRIGHT" '"Bright one step"
                        Case &H14 : valeur = "DIM" 'Dim one step"
                        Case &H15 : valeur = "START_DIM_CYCLE" 'Start dim cycle"
                        Case &H16 : valeur = "Program(Timer)"
                        Case &H17 : valeur = "Request status from a bidirectional device"
                        Case &H18 : valeur = "OFF for timer period"
                        Case &H19 : valeur = "ON (100%) for timer period"
                        Case &H1A : valeur = "ON ( at last dim level set) for timer period"
                        Case &H1B : valeur = "RESET"
                        Case Else : valeur = "ERROR: Unknown"
                    End Select
                    'If (recbuf(FS20.cmd1) And &H80) = 0 Then
                    '    WriteMessage("                command to receiver")
                    'Else
                    '    WriteMessage("                response from receiver")
                    'End If
                    'If (recbuf(FS20.cmd1) And &H40) = 0 Then
                    '    WriteMessage("                unidirectional command")
                    'Else
                    '    WriteMessage("                bidirectional command")
                    'End If
                    'If (recbuf(FS20.cmd1) And &H20) = 0 Then
                    '    WriteMessage("                additional cmd2 byte not present")
                    'Else
                    '    WriteMessage("                additional cmd2 byte present")
                    'End If
                    'If (recbuf(FS20.cmd1) And &H20) <> 0 Then
                    '    WriteMessage("Cmd2          = " & VB.Right("0" & Hex(recbuf(FS20.cmd2)), 2))
                    'End If
                    WriteRetour(adresse, "", valeur)
                Case FS20.sTypeFHT8V
                    'WriteMessage("subtype       = FHT 8V valve")
                    'WriteMessage("Sequence nbr  = " & recbuf(FS20.seqnbr).ToString)
                    'WriteMessage("House code    = " & VB.Right("0" & Hex(recbuf(FS20.hc1)), 2) & VB.Right("0" & Hex(recbuf(FS20.hc2)), 2))
                    'WriteMessage("Address       = " & VB.Right("0" & Hex(recbuf(FS20.addr)), 2))
                    adresse = VB.Right("0" & Hex(recbuf(FS20.hc1)), 2) & VB.Right("0" & Hex(recbuf(FS20.hc2)), 2) & VB.Right("0" & Hex(recbuf(FS20.addr)), 2)
                    'If (recbuf(FS20.cmd1) And &H80) = 0 Then
                    '    WriteMessage("new command")
                    'Else
                    '    WriteMessage("repeated command")
                    'End If
                    'If (recbuf(FS20.cmd1) And &H40) = 0 Then
                    '    WriteMessage("                unidirectional command")
                    'Else
                    '    WriteMessage("                bidirectional command")
                    'End If
                    'If (recbuf(FS20.cmd1) And &H20) = 0 Then
                    '    WriteMessage("                additional cmd2 byte not present")
                    'Else
                    '    WriteMessage("                additional cmd2 byte present")
                    'End If
                    'If (recbuf(FS20.cmd1) And &H10) = 0 Then
                    '    WriteMessage("                battery empty beep not enabled")
                    'Else
                    '    WriteMessage("                enable battery empty beep")
                    'End If
                    Select Case (recbuf(FS20.cmd1) And &HF)
                        Case &H0 : valeur = "Synchronize now : valve position: " & VB.Right("0" & Hex(recbuf(FS20.cmd2)), 2) & " is " & (CInt(recbuf(FS20.cmd2) / 2.55)).ToString & "%"
                        Case &H1 : valeur = "open valve"
                        Case &H2 : valeur = "close valve"
                        Case &H6 : valeur = "open valve at percentage level : valve position: " & VB.Right("0" & Hex(recbuf(FS20.cmd2)), 2) & " is " & (CInt(recbuf(FS20.cmd2) / 2.55)).ToString & "%"
                        Case &H8 : valeur = "relative offset (cmd2 bit 7=direction, bit 5-0 offset value)"
                        Case &HA : valeur = "decalcification cycle : valve position: " & VB.Right("0" & Hex(recbuf(FS20.cmd2)), 2) & " is " & (CInt(recbuf(FS20.cmd2) / 2.55)).ToString & "%"
                        Case &HC : valeur = "synchronization active : count down is " & (recbuf(FS20.cmd2) >> 1).ToString & " seconds"
                        Case &HE : valeur = "test, drive valve and produce an audible signal"
                        Case &HF : valeur = "pair valve (cmd2 bit 7-1 is count down in seconds, bit 0=1) : count down is " & CStr(recbuf(FS20.cmd2) >> 1) & " seconds"
                        Case Else : valeur = "ERROR: Unknown"
                    End Select
                    WriteRetour(adresse, "", valeur)
                Case FS20.sTypeFHT80
                    'WriteMessage("subtype       = FHT80 door/window sensor")
                    'WriteMessage("Sequence nbr  = " & recbuf(FS20.seqnbr).ToString)
                    'WriteMessage("House code    = " & VB.Right("0" & Hex(recbuf(FS20.hc1)), 2) & VB.Right("0" & Hex(recbuf(FS20.hc2)), 2))
                    'WriteMessage("Address       = " & VB.Right("0" & Hex(recbuf(FS20.addr)), 2))
                    adresse = VB.Right("0" & Hex(recbuf(FS20.hc1)), 2) & VB.Right("0" & Hex(recbuf(FS20.hc2)), 2) & VB.Right("0" & Hex(recbuf(FS20.addr)), 2)
                    Select Case (recbuf(FS20.cmd1) And &HF)
                        Case &H1 : valeur = "OPEN"
                        Case &H2 : valeur = "CLOSED"
                        Case &HC : valeur = "CFG: synchronization active"
                        Case Else : valeur = "ERROR: Unknown command"
                    End Select
                    'If (recbuf(FS20.cmd1) And &H80) = 0 Then
                    '    WriteMessage("                new command")
                    'Else
                    '    WriteMessage("                repeated command")
                    'End If
                    WriteRetour(adresse, "", valeur)
                Case Else : WriteLog("ERR: Unknown Sub type for Packet type=" & Hex(recbuf(FS20.packettype)) & ":" & Hex(recbuf(FS20.subtype)))
            End Select
            If _DEBUG Then WriteLog("DBG: Signal Level : " & (recbuf(FS20.rssi) >> 4).ToString & " (Adresse:" & adresse & ")")
        Catch ex As Exception
            WriteLog("ERR: decode_FS20 Exception : " & ex.Message)
        End Try
    End Sub
#End Region

#Region "Send messages"

    Private Sub SetMode(ByVal paramMode As String)
        Try
            Dim temp As String = ""
            'paramMode 20011111111111111011111111  
            '1 : type frequence (310, 315, 433, 868.30, 868.30 FSK, 868.35, 868.35 FSK, 868.95)
            '2 : UNDEC
            '3 : novatis --> NOT USED ANYMORE 200
            '4 : proguard
            '5 : FS20
            '6 : Lacrosse
            '7 : Hideki
            '8 : AD
            '9 : Mertik 111111
            '10 : Visonic
            '11 : ATI
            '12 : Oregon
            '13 : Meiantech
            '14 : HEEU
            '15 : AC
            '16 : ARC
            '17 : X10 11111111

            '18 : BlindsT0
            '19 : RFU6
            '20 : RFU5
            '21 : RFU4
            '22 : RFU3
            '23 : FINEOFFSET
            '24 : RUBICSON
            '25 : AE
            '26 : BlindsT1

            If paramMode.Length <> 26 Then
                WriteLog("ERR: Setmode : ParamMode incorrect : " & paramMode & " (valeur par défaut utilisée : 20011111111111111011111111)")
                paramMode = "20011111111111111011111111"
            Else
                WriteLog("DBG: Setmode : ParamMode utilisé : " & paramMode)
            End If


            Dim kar(ICMD.size) As Byte
            kar(ICMD.packetlength) = ICMD.size
            kar(ICMD.packettype) = ICMD.pType
            kar(ICMD.subtype) = ICMD.sType
            kar(ICMD.seqnbr) = bytSeqNbr
            kar(ICMD.cmnd) = ICMD.SETMODE

            'type frequence
            Select Case CInt(paramMode.Substring(0, 1))
                Case 0 : kar(ICMD.msg1) = IRESPONSE.recType310
                Case 1 : kar(ICMD.msg1) = IRESPONSE.recType315
                Case 2 : kar(ICMD.msg1) = IRESPONSE.recType43392
                Case 3 : kar(ICMD.msg1) = IRESPONSE.recType86830
                Case 4 : kar(ICMD.msg1) = IRESPONSE.recType86830FSK
                Case 5 : kar(ICMD.msg1) = IRESPONSE.recType86835
                Case 6 : kar(ICMD.msg1) = IRESPONSE.recType86835FSK
                Case 7 : kar(ICMD.msg1) = IRESPONSE.recType86895
            End Select

            'UNDEC
            If CInt(paramMode.Substring(1, 1)) = 1 Then kar(ICMD.msg3) = &H80 Else kar(ICMD.msg3) = 0

            If CInt(paramMode.Substring(17, 1)) = 1 Then
                'All other protocol receiving is disabled if BlindsT0 enabled, BlindsT0 receive is only used to read the address code of the remote
                kar(ICMD.msg4) = 0
                kar(ICMD.msg4) = kar(ICMD.msg4) Or IRESPONSE.msg4_BlindsT0
                kar(ICMD.msg5) = 0
            Else
                If CInt(paramMode.Substring(18, 1)) = 1 Then kar(ICMD.msg3) = kar(ICMD.msg3) Or IRESPONSE.msg3_RFU6
                If CInt(paramMode.Substring(19, 1)) = 1 Then kar(ICMD.msg3) = kar(ICMD.msg3) Or IRESPONSE.msg3_RFU5
                If CInt(paramMode.Substring(20, 1)) = 1 Then kar(ICMD.msg3) = kar(ICMD.msg3) Or IRESPONSE.msg3_RFU4
                If CInt(paramMode.Substring(21, 1)) = 1 Then kar(ICMD.msg3) = kar(ICMD.msg3) Or IRESPONSE.msg3_RFU3
                If CInt(paramMode.Substring(22, 1)) = 1 Then kar(ICMD.msg3) = kar(ICMD.msg3) Or IRESPONSE.msg3_FINEOFFSET
                If CInt(paramMode.Substring(23, 1)) = 1 Then kar(ICMD.msg3) = kar(ICMD.msg3) Or IRESPONSE.msg3_RUBICSON
                If CInt(paramMode.Substring(24, 1)) = 1 Then kar(ICMD.msg3) = kar(ICMD.msg3) Or IRESPONSE.msg3_AE

                kar(ICMD.msg4) = 0
                If CInt(paramMode.Substring(25, 1)) = 1 Then kar(ICMD.msg4) = kar(ICMD.msg4) Or IRESPONSE.msg4_BlindsT1
                If CInt(paramMode.Substring(3, 1)) = 1 Then kar(ICMD.msg4) = kar(ICMD.msg4) Or IRESPONSE.msg4_PROGUARD
                If CInt(paramMode.Substring(4, 1)) = 1 Then kar(ICMD.msg4) = kar(ICMD.msg4) Or IRESPONSE.msg4_FS20
                If CInt(paramMode.Substring(5, 1)) = 1 Then kar(ICMD.msg4) = kar(ICMD.msg4) Or IRESPONSE.msg4_LCROS
                If CInt(paramMode.Substring(6, 1)) = 1 Then kar(ICMD.msg4) = kar(ICMD.msg4) Or IRESPONSE.msg4_HID
                If CInt(paramMode.Substring(7, 1)) = 1 Then kar(ICMD.msg4) = kar(ICMD.msg4) Or IRESPONSE.msg4_AD
                If CInt(paramMode.Substring(8, 1)) = 1 Then kar(ICMD.msg4) = kar(ICMD.msg4) Or IRESPONSE.msg4_MERTIK

                kar(ICMD.msg5) = 0
                If CInt(paramMode.Substring(9, 1)) = 1 Then kar(ICMD.msg5) = kar(ICMD.msg5) Or IRESPONSE.msg5_VISONIC
                If CInt(paramMode.Substring(10, 1)) = 1 Then kar(ICMD.msg5) = kar(ICMD.msg5) Or IRESPONSE.msg5_ATI
                If CInt(paramMode.Substring(11, 1)) = 1 Then kar(ICMD.msg5) = kar(ICMD.msg5) Or IRESPONSE.msg5_OREGON
                If CInt(paramMode.Substring(12, 1)) = 1 Then kar(ICMD.msg5) = kar(ICMD.msg5) Or IRESPONSE.msg5_MEI
                If CInt(paramMode.Substring(13, 1)) = 1 Then kar(ICMD.msg5) = kar(ICMD.msg5) Or IRESPONSE.msg5_HEU
                If CInt(paramMode.Substring(14, 1)) = 1 Then kar(ICMD.msg5) = kar(ICMD.msg5) Or IRESPONSE.msg5_AC
                If CInt(paramMode.Substring(15, 1)) = 1 Then kar(ICMD.msg5) = kar(ICMD.msg5) Or IRESPONSE.msg5_ARC
                If CInt(paramMode.Substring(16, 1)) = 1 Then kar(ICMD.msg5) = kar(ICMD.msg5) Or IRESPONSE.msg5_X10

            End If
            If _DEBUG Then
                For Each bt As Byte In kar
                    temp = temp & VB.Right("0" & Hex(bt), 2) & " "
                Next
                WriteLog("DBG: Setmode : Commande envoyée : " & temp)
            End If
            ecrire(kar)
        Catch ex As Exception
            WriteLog("ERR: SetMode Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Converti un house de type A, B... en byte</summary>
    ''' <param name="housecode">HouseCode du type A (de A1)</param>
    ''' <returns>Byte représentant le housecode</returns>
    Private Function convert_housecode(ByVal housecode As String) As Byte
        Try
            Dim temp As Byte
            Select Case housecode
                Case "A" : temp = 0 + &H41
                Case "B" : temp = 1 + &H41
                Case "C" : temp = 2 + &H41
                Case "D" : temp = 3 + &H41
                Case "E" : temp = 4 + &H41
                Case "F" : temp = 5 + &H41
                Case "G" : temp = 6 + &H41
                Case "H" : temp = 7 + &H41
                Case "I" : temp = 8 + &H41
                Case "J" : temp = 9 + &H41
                Case "K" : temp = 10 + &H41
                Case "L" : temp = 11 + &H41
                Case "M" : temp = 12 + &H41
                Case "N" : temp = 13 + &H41
                Case "O" : temp = 14 + &H41
                Case "P" : temp = 15 + &H41
                Case Else : WriteLog("ERR: convert_housecode HouseCode Incorrect : " & housecode)
            End Select
            Return temp
        Catch ex As Exception
            WriteLog("ERR: convert_housecode Exception : " & ex.Message)
            Return 0 + &H41
        End Try
    End Function

    ''' <summary>Gestion du protocole X10/ARC/ELRO AB400D/Waveman/EMW200/Impuls/RisingSun/Philips SBC</summary>
    ''' <param name="adresse">Adresse du type A1</param>
    ''' <param name="commande">commande ON, OFF, BRIGHT, DIM, ALL_LIGHT_ON, ALL_LIGHT_OFF</param>
    ''' <param name="type">0=X10, 1=ARC, 2=ELRO AB400D, 3=Waveman, 4=EMW200, 5=Impuls, 6=RisingSun, 7=Philips SBC</param>
    ''' <remarks></remarks>
    Private Sub send_lighting1(ByVal adresse As String, ByVal commande As String, ByVal type As Integer)
        Try
            Dim kar(LIGHTING1.size) As Byte
            Dim temp As String = ""

            'verification format adresse
            If Not (adresse.Length = 2) Then
                WriteLog("ERR: Send lighting1 : Adresse invalide : " & adresse)
                Exit Sub
            End If
            Select Case type
                Case LIGHTING1.sTypeX10, LIGHTING1.sTypeARC, LIGHTING1.sTypeWaveman
                    If CInt(adresse.Substring(1, 1)) > 16 Or (adresse.Substring(0, 1) <> "A" And adresse.Substring(0, 1) <> "B" And adresse.Substring(0, 1) <> "C" And adresse.Substring(0, 1) <> "D" And adresse.Substring(0, 1) <> "E" And adresse.Substring(0, 1) <> "F" And adresse.Substring(0, 1) <> "G" And adresse.Substring(0, 1) <> "H" And adresse.Substring(0, 1) <> "I" And adresse.Substring(0, 1) <> "J" And adresse.Substring(0, 1) <> "K" And adresse.Substring(0, 1) <> "L" And adresse.Substring(0, 1) <> "M" And adresse.Substring(0, 1) <> "N" And adresse.Substring(0, 1) <> "O" And adresse.Substring(0, 1) <> "P") Then
                        WriteLog("ERR: Send lighting1 : Adresse (X10, ARC, Waveman) invalide : " & adresse)
                        Exit Sub
                    End If
                Case LIGHTING1.sTypeAB400D, LIGHTING1.sTypeIMPULS
                    If CInt(adresse.Substring(1, 1)) > 64 Or (adresse.Substring(0, 1) <> "A" And adresse.Substring(0, 1) <> "B" And adresse.Substring(0, 1) <> "C" And adresse.Substring(0, 1) <> "D" And adresse.Substring(0, 1) <> "E" And adresse.Substring(0, 1) <> "F" And adresse.Substring(0, 1) <> "G" And adresse.Substring(0, 1) <> "H" And adresse.Substring(0, 1) <> "I" And adresse.Substring(0, 1) <> "J" And adresse.Substring(0, 1) <> "K" And adresse.Substring(0, 1) <> "L" And adresse.Substring(0, 1) <> "M" And adresse.Substring(0, 1) <> "N" And adresse.Substring(0, 1) <> "O" And adresse.Substring(0, 1) <> "P") Then
                        WriteLog("ERR: Send lighting1 : Adresse (ELRO AB400D, Impuls) invalide : " & adresse)
                        Exit Sub
                    End If
                Case LIGHTING1.sTypeEMW200
                    If CInt(adresse.Substring(1, 1)) > 4 Or (adresse.Substring(0, 1) <> "A" And adresse.Substring(0, 1) <> "B" And adresse.Substring(0, 1) <> "C") Then
                        WriteLog("ERR: Send lighting1 : Adresse (EMW200) invalide : " & adresse)
                        Exit Sub
                    End If
                Case LIGHTING1.sTypePhilips
                    If CInt(adresse.Substring(1, 1)) > 8 Or (adresse.Substring(0, 1) <> "A" And adresse.Substring(0, 1) <> "B" And adresse.Substring(0, 1) <> "C" And adresse.Substring(0, 1) <> "D" And adresse.Substring(0, 1) <> "E" And adresse.Substring(0, 1) <> "F" And adresse.Substring(0, 1) <> "G" And adresse.Substring(0, 1) <> "H" And adresse.Substring(0, 1) <> "I" And adresse.Substring(0, 1) <> "J" And adresse.Substring(0, 1) <> "K" And adresse.Substring(0, 1) <> "L" And adresse.Substring(0, 1) <> "M" And adresse.Substring(0, 1) <> "N" And adresse.Substring(0, 1) <> "O" And adresse.Substring(0, 1) <> "P") Then
                        WriteLog("ERR: Send lighting1 : Adresse (ELRO AB400D, Impuls) invalide : " & adresse)
                        Exit Sub
                    End If
            End Select

            kar(LIGHTING1.packetlength) = LIGHTING1.size
            kar(LIGHTING1.packettype) = LIGHTING1.pType
            kar(LIGHTING1.subtype) = CByte(type)
            kar(LIGHTING1.seqnbr) = bytSeqNbr
            kar(LIGHTING1.housecode) = convert_housecode(adresse.Substring(0, 1))
            kar(LIGHTING1.unitcode) = CByte(adresse.Substring(1, 1))

            Select Case commande
                Case "OFF"
                    kar(LIGHTING1.cmnd) = LIGHTING1.sOff
                    WriteRetourSend(adresse, "", "OFF")
                Case "ON"
                    kar(LIGHTING1.cmnd) = LIGHTING1.sOn
                    WriteRetourSend(adresse, "", "ON")
                Case "ALL_LIGHT_OFF"
                    If type = LIGHTING1.sTypeX10 Or type = LIGHTING1.sTypeEMW200 Or type = LIGHTING1.sTypePhilips Or type = LIGHTING1.sTypeARC Then
                        kar(LIGHTING1.cmnd) = LIGHTING1.sAllOff
                        WriteRetourSend(adresse, "", "OFF")
                        'traiter toutes les lights


                    Else
                        WriteLog("ERR: Send lighting1 : Commande ALL_LIGHT_OFF indisponible pour ce protocole")
                        Exit Sub
                    End If
                Case "ALL_LIGHT_ON"
                    If type = LIGHTING1.sTypeX10 Or type = LIGHTING1.sTypeEMW200 Or type = LIGHTING1.sTypePhilips Or type = LIGHTING1.sTypeARC Then
                        kar(LIGHTING1.cmnd) = LIGHTING1.sAllOn
                        WriteRetourSend(adresse, "", "ON")
                        'traiter toutes les lights


                    Else
                        WriteLog("ERR: Send lighting1 : Commande ALL_LIGHT_ON indisponible pour ce protocole")
                        Exit Sub
                    End If
                Case "BRIGHT"
                    If type = LIGHTING1.sTypeX10 Then
                        kar(LIGHTING1.cmnd) = LIGHTING1.sBright
                        WriteRetourSend(adresse, "", "ON")
                    Else
                        WriteLog("ERR: Send lighting1 : Commande BRIGHT indisponible pour ce protocole")
                        Exit Sub
                    End If
                Case "DIM"
                    If type = LIGHTING1.sTypeX10 Then
                        kar(LIGHTING1.cmnd) = LIGHTING1.sDim
                        WriteRetourSend(adresse, "", "ON")
                    Else
                        WriteLog("ERR: Send lighting1 : Commande DIM indisponible pour ce protocole")
                        Exit Sub
                    End If
                Case "CHIME"
                    If type = LIGHTING1.sTypeARC Then
                        kar(LIGHTING1.cmnd) = LIGHTING1.sChime
                        kar(LIGHTING1.unitcode) = 8
                        WriteRetourSend(adresse, "", "CHIME")
                    Else
                        WriteLog("ERR: Send lighting1 : Commande CHIME indisponible pour ce protocole")
                        Exit Sub
                    End If
                Case Else
                    WriteLog("ERR: Send lighting1 : Commande invalide : " & commande)
                    Exit Sub
            End Select
            kar(LIGHTING1.filler) = 0
            ecrire(kar)
            If _DEBUG Then
                For Each bt As Byte In kar
                    temp = temp & VB.Right("0" & Hex(bt), 2) & " "
                Next
                WriteLog("DBG: Send lighting1 : commande envoyée : " & temp)
            End If
        Catch ex As Exception
            WriteLog("ERR: Send lighting1 Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Gestion du protocole AC / HEEU / ANSLUT</summary>
    ''' <param name="adresse">Adresse du type 02F4416-1</param>
    ''' <param name="commande">commande ON, OFF, DIM, GROUP_OFF, GROUP_ON, GROUP_DIM</param>
    ''' <param name="type">0=AC / 1=HEEU / 2=ANSLUT</param>
    ''' <param name="dimlevel">Level pour Dim de 1 à 16</param>
    ''' <remarks></remarks>
    Private Sub send_Lighting2(ByVal adresse As String, ByVal commande As String, ByVal type As Integer, Optional ByVal dimlevel As Integer = 1)
        Try
            Dim kar(LIGHTING2.size) As Byte
            Dim temp As String = ""
            If Not (adresse.Length = 9) Then
                WriteLog("ERR: Send Lighting2 : Adresse invalide : " & adresse)
                Exit Sub
            End If
            kar(LIGHTING2.packetlength) = LIGHTING2.size
            kar(LIGHTING2.packettype) = LIGHTING2.pType
            kar(LIGHTING2.seqnbr) = bytSeqNbr
            kar(LIGHTING2.subtype) = CByte(type)
            Select Case commande
                Case "OFF"
                    kar(LIGHTING2.cmnd) = 0
                    WriteRetourSend(adresse, "", "OFF")
                Case "ON"
                    kar(LIGHTING2.cmnd) = 1
                    WriteRetourSend(adresse, "", "ON")
                Case "GROUP_OFF"
                    kar(LIGHTING2.cmnd) = 3
                    WriteRetourSend(adresse, "", "OFF")
                    'traiter toutes le groupe



                Case "GROUP_ON"
                    kar(LIGHTING2.cmnd) = 4
                    WriteRetourSend(adresse, "", "ON")
                    'traiter toutes le groupe



                Case "GROUP_DIM"
                    kar(LIGHTING2.cmnd) = 5
                    WriteRetourSend(adresse, "", CStr(dimlevel))
                    'traiter toutes le groupe



                Case "DIM"
                    kar(LIGHTING2.cmnd) = 2
                    WriteRetourSend(adresse, "", CStr(dimlevel))
                Case Else
                    WriteLog("ERR: Send AC : Commande invalide : " & commande)
                    Exit Sub
            End Select
            Try
                Dim adressetab As String() = adresse.Split(CChar("-"))
                kar(LIGHTING2.unitcode) = CByte(adressetab(1))
                kar(LIGHTING2.id1) = CByte(adressetab(0).Substring(0, 1))
                kar(LIGHTING2.id2) = CByte(Array.IndexOf(adressetoint, adressetab(0).Substring(1, 2)))
                kar(LIGHTING2.id3) = CByte(Array.IndexOf(adressetoint, adressetab(0).Substring(3, 2)))
                kar(LIGHTING2.id4) = CByte(Array.IndexOf(adressetoint, adressetab(0).Substring(5, 2)))
            Catch ex As Exception
                WriteLog("ERR: Send Lighting2 Exception : Adresse incorrecte")
            End Try

            If dimlevel = 0 Then
                dimlevel = 0
            ElseIf dimlevel < 7 Then
                dimlevel = 1
            ElseIf dimlevel < 14 Then
                dimlevel = 2
            ElseIf dimlevel < 21 Then
                dimlevel = 3
            ElseIf dimlevel < 28 Then
                dimlevel = 4
            ElseIf dimlevel < 34 Then
                dimlevel = 5
            ElseIf dimlevel < 40 Then
                dimlevel = 6
            ElseIf dimlevel < 46 Then
                dimlevel = 7
            ElseIf dimlevel < 53 Then
                dimlevel = 8
            ElseIf dimlevel < 60 Then
                dimlevel = 9
            ElseIf dimlevel < 67 Then
                dimlevel = 10
            ElseIf dimlevel < 74 Then
                dimlevel = 11
            ElseIf dimlevel < 81 Then
                dimlevel = 12
            ElseIf dimlevel < 88 Then
                dimlevel = 13
            ElseIf dimlevel < 95 Then
                dimlevel = 14
            Else
                dimlevel = 15
            End If
            kar(LIGHTING2.level) = CByte(dimlevel)
            kar(LIGHTING2.filler) = 0

            ecrire(kar)
            If _DEBUG Then
                For Each bt As Byte In kar
                    temp = temp & VB.Right("0" & Hex(bt), 2) & " "
                Next
                WriteLog("DBG: Send Lighting2 : commande envoyée : " & temp)
            End If
        Catch ex As Exception
            WriteLog("ERR: Send Lighting2 Exception : " & ex.ToString)
        End Try
    End Sub

    ''' <summary>Gestion du protocole EMW100 / LIGHTWAVERF</summary>
    ''' <param name="adresse">Adresse du type FFFFFF-1</param>
    ''' <param name="commande">commande ON, OFF, ALL_LIGHT_ON, ALL_LIGHT_OFF</param>
    ''' <param name="type">0=EMW100 / 1=LIGHTWAVERF</param>
    ''' <param name="dimlevel">Level pour Dim de 1 à 31</param>
    ''' <remarks></remarks>
    Private Sub send_lighting5(ByVal adresse As String, ByVal commande As String, ByVal type As Integer, Optional ByVal dimlevel As Integer = 1)
        Try
            Dim kar(LIGHTING5.size) As Byte
            Dim temp As String = ""

            If Not (adresse.Length = 8) Then
                WriteLog("ERR: Send lighting5 : Adresse invalide : " & adresse)
                Exit Sub
            End If

            kar(LIGHTING5.packetlength) = LIGHTING5.size
            kar(LIGHTING5.packettype) = LIGHTING5.pType
            kar(LIGHTING5.subtype) = CByte(type) '0=EMW100 / 1=LIGHTWAVERF
            kar(LIGHTING2.seqnbr) = bytSeqNbr
            Try
                Dim adressetab As String() = adresse.Split(CChar("-"))
                kar(LIGHTING5.unitcode) = CByte(adressetab(1))
                kar(LIGHTING5.id1) = CByte(adressetab(0).Substring(0, 2))
                kar(LIGHTING5.id2) = CByte(Array.IndexOf(adressetoint, adressetab(0).Substring(2, 2)))
                kar(LIGHTING5.id3) = CByte(Array.IndexOf(adressetoint, adressetab(0).Substring(4, 2)))
            Catch ex As Exception
                WriteLog("ERR: Send lighting5 Exception : Adresse incorrecte")
            End Try

            Select Case commande
                Case "OFF"
                    kar(LIGHTING5.cmnd) = LIGHTING5.sOff
                    WriteRetourSend(adresse, "", "OFF")
                Case "ON"
                    kar(LIGHTING5.cmnd) = LIGHTING5.sOn
                    WriteRetourSend(adresse, "", "ON")
                Case "GROUP_OFF"
                    kar(LIGHTING5.cmnd) = LIGHTING5.sGroupOff
                    WriteRetourSend(adresse, "", "OFF")
                    'traiter tout le groupe

                    'commandes restantes à dev pour EMW100 : Mood1/Mood2/Mood3/Mood4/Mood5/Unlock/Lock/All Lock/Close relay/Stop relay/Open relay/Set Level
                    'commandes restantes à dev pour LIGHTWAVERF : Learn

                Case Else
                    WriteLog("ERR: Send lighting5 : Commande invalide : " & commande)
                    Exit Sub
            End Select

            If kar(LIGHTING5.cmnd) = LIGHTING5.sSetLevel Then
                If dimlevel = 0 Then
                    dimlevel = 0
                ElseIf dimlevel < 7 Then
                    dimlevel = 1
                ElseIf dimlevel < 14 Then
                    dimlevel = 2
                ElseIf dimlevel < 21 Then
                    dimlevel = 3
                ElseIf dimlevel < 28 Then
                    dimlevel = 4
                ElseIf dimlevel < 34 Then
                    dimlevel = 5
                ElseIf dimlevel < 40 Then
                    dimlevel = 6
                ElseIf dimlevel < 46 Then
                    dimlevel = 7
                ElseIf dimlevel < 53 Then
                    dimlevel = 8
                ElseIf dimlevel < 60 Then
                    dimlevel = 9
                ElseIf dimlevel < 67 Then
                    dimlevel = 10
                ElseIf dimlevel < 74 Then
                    dimlevel = 11
                ElseIf dimlevel < 81 Then
                    dimlevel = 12
                ElseIf dimlevel < 88 Then
                    dimlevel = 13
                ElseIf dimlevel < 95 Then
                    dimlevel = 14
                Else
                    dimlevel = 15
                End If
                kar(LIGHTING5.level) = CByte(dimlevel)
            Else
                kar(LIGHTING5.level) = 0
            End If
            kar(LIGHTING5.filler) = 0
            If kar(LIGHTING5.cmnd) = 8 Or kar(LIGHTING5.cmnd) = 9 Then 'not used commands
                Exit Sub
            End If
            If kar(LIGHTING5.id1) = 0 And kar(LIGHTING5.id2) = 0 And kar(LIGHTING5.id3) = 0 Then
                WriteLog("ERR: Send lighting5 : Adresse invalide : " & adresse)
                Exit Sub
            End If

            ecrire(kar)
            If _DEBUG Then
                For Each bt As Byte In kar
                    temp = temp & VB.Right("0" & Hex(bt), 2) & " "
                Next
                WriteLog("DBG: Send lighting5 : commande envoyée : " & temp)
            End If
        Catch ex As Exception
            WriteLog("ERR: Send lighting5 Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Gestion du protocole BLYSS</summary>
    ''' <param name="adresse">Adresse du type FFFF-A1  avec A1 A/D et 1/4</param>
    ''' <param name="commande">commande ON, OFF</param>
    ''' <param name="type">0=BLYSS</param>
    ''' <remarks></remarks>
    Private Sub send_lighting6(ByVal adresse As String, ByVal commande As String, ByVal type As Integer)
        Try
            Dim kar(LIGHTING6.size) As Byte
            Dim temp As String = ""

            If Not (adresse.Length = 7) Then
                WriteLog("ERR: Send lighting6 : Adresse invalide : " & adresse)
                Exit Sub
            End If

            kar(LIGHTING6.packetlength) = LIGHTING6.size
            kar(LIGHTING6.packettype) = LIGHTING6.pType
            kar(LIGHTING6.subtype) = 0 '0=BLYSS
            kar(LIGHTING6.seqnbr) = bytSeqNbr
            Try
                Dim adressetab As String() = adresse.Split(CChar("-"))
                kar(LIGHTING6.groupcode) = convert_housecode(adressetab(1).Substring(0, 1))
                kar(LIGHTING6.unitcode) = CByte(adressetab(1).Substring(1, 1))
                kar(LIGHTING6.id1) = CByte(adressetab(0).Substring(0, 2))
                kar(LIGHTING6.id2) = CByte(Array.IndexOf(adressetoint, adressetab(0).Substring(2, 2)))
            Catch ex As Exception
                WriteLog("ERR: Send lighting6 Exception : Adresse incorrecte")
            End Try

            Select Case commande
                Case "OFF"
                    kar(LIGHTING6.cmnd) = LIGHTING6.sOff
                    WriteRetourSend(adresse, "", "OFF")
                Case "ON"
                    kar(LIGHTING6.cmnd) = LIGHTING6.sOn
                    WriteRetourSend(adresse, "", "ON")
                Case "GROUP_OFF"
                    kar(LIGHTING6.cmnd) = LIGHTING6.sGroupOff
                    WriteRetourSend(adresse, "", "OFF")
                Case "GROUP_ON"
                    kar(LIGHTING6.cmnd) = LIGHTING6.sGroupOff
                    WriteRetourSend(adresse, "", "OFF")
                Case Else
                    WriteLog("ERR: Send lighting6 : Commande invalide : " & commande)
                    Exit Sub
            End Select

            kar(LIGHTING6.cmndseqnbr) = bytCmndSeqNbr
            bytCmndSeqNbr += 1
            If bytCmndSeqNbr > 4 Then
                bytCmndSeqNbr = 0
            End If
            kar(LIGHTING6.rfu) = 0
            kar(LIGHTING6.filler) = 0

            If kar(LIGHTING6.id1) = 0 And kar(LIGHTING6.id2) = 0 Then
                WriteLog("ERR: Send lighting6 : Adresse invalide : " & adresse)
                Exit Sub
            End If

            ecrire(kar)
            If _DEBUG Then
                For Each bt As Byte In kar
                    temp = temp & VB.Right("0" & Hex(bt), 2) & " "
                Next
                WriteLog("DBG: Send lighting6 : commande envoyée : " & temp)
            End If
        Catch ex As Exception
            WriteLog("ERR: Send lighting6 Exception : " & ex.Message)
        End Try
    End Sub

    'OLD
    ' ''' <summary>Gestion du protocole X10 RF</summary>
    ' ''' <param name="adresse">Adresse du type A1</param>
    ' ''' <param name="commande">commande ON, OFF, BRIGHT, DIM, ALL_LIGHT_ON, ALL_LIGHT_OFF</param>
    ' ''' <remarks></remarks>
    'Private Sub send_x10(ByVal adresse As String, ByVal commande As String)
    '    Try
    '        Dim kar(LIGHTING1.size) As Byte
    '        Dim temp As String = ""

    '        If Not (adresse.Length = 2) Then
    '            WriteLog("ERR: Send X10 : Adresse invalide : " & adresse)
    '            Exit Sub
    '        End If
    '        kar(LIGHTING1.packetlength) = LIGHTING1.size
    '        kar(LIGHTING1.packettype) = LIGHTING1.pType
    '        kar(LIGHTING1.subtype) = 0  '0=X10, 1=ARC, 2=ELRO AB400D, 3=Waveman, 4=EMW200, 5=Impuls, 6=RisingSun, 7=Philips SBC
    '        kar(LIGHTING1.seqnbr) = bytSeqNbr
    '        kar(LIGHTING1.housecode) = convert_housecode(adresse.Substring(0, 1))
    '        kar(LIGHTING1.unitcode) = adresse.Substring(1, 1)
    '        Select Case commande
    '            Case "OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOff
    '                WriteRetourSend(adresse, "", "OFF")
    '            Case "ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOn
    '                WriteRetourSend(adresse, "", "ON")
    '            Case "ALL_LIGHT_OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sAllOff
    '                WriteRetourSend(adresse, "", "OFF")
    '                'traiter toutes les lights



    '            Case "ALL_LIGHT_ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sAllOn
    '                WriteRetourSend(adresse, "", "ON")
    '                'traiter toutes les lights




    '            Case "BRIGHT"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sBright
    '                WriteRetourSend(adresse, "", "ON")
    '            Case "DIM"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sDim
    '                WriteRetourSend(adresse, "", "ON")
    '            Case "CHIME"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sChime
    '                kar(LIGHTING1.unitcode) = 8
    '                WriteRetourSend(adresse, "", "CHIME")
    '            Case Else
    '                WriteLog("ERR: Send X10 : Commande invalide : " & commande)
    '                Exit Sub
    '        End Select
    '        kar(LIGHTING1.filler) = 0
    '        ecrire(kar)
    '        If _DEBUG Then
    '            For Each bt As Byte In kar
    '                temp = temp & VB.Right("0" & Hex(bt), 2) & " "
    '            Next
    '            WriteLog("DBG: Send X10 : commande envoyée : " & temp)
    '        End If
    '    Catch ex As Exception
    '        WriteLog("ERR: Send X10 Exception : " & ex.Message)
    '    End Try
    'End Sub

    ' ''' <summary>Gestion du protocole ARC</summary>
    ' ''' <param name="adresse">Adresse du type A1</param>
    ' ''' <param name="commande">commande ON, OFF, GROUP_ON, GROUP_OFF, CHIME</param>
    ' ''' <remarks></remarks>
    'Private Sub send_arc(ByVal adresse As String, ByVal commande As String)
    '    Try
    '        Dim kar(LIGHTING1.size) As Byte
    '        Dim temp As String = ""

    '        If Not (adresse.Length = 2) Then
    '            WriteLog("ERR: Send ARC : Adresse invalide : " & adresse)
    '            Exit Sub
    '        End If
    '        kar(LIGHTING1.packetlength) = LIGHTING1.size
    '        kar(LIGHTING1.packettype) = LIGHTING1.pType
    '        kar(LIGHTING1.subtype) = 1  '0=X10, 1=ARC, 2=ELRO AB400D, 3=Waveman, 4=EMW200, 5=Impuls, 6=RisingSun, 7=Philips SBC
    '        kar(LIGHTING1.seqnbr) = bytSeqNbr
    '        kar(LIGHTING1.housecode) = convert_housecode(adresse.Substring(0, 1))
    '        kar(LIGHTING1.unitcode) = adresse.Substring(1, 1)
    '        Select Case commande
    '            Case "OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOff
    '                WriteRetourSend(adresse, "", "OFF")
    '            Case "ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOn
    '                WriteRetourSend(adresse, "", "ON")
    '            Case "GROUP_ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sAllOn
    '                WriteRetourSend(adresse, "", "ON")
    '                'traiter toutes le groupe



    '            Case "GROUP_OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sAllOff
    '                WriteRetourSend(adresse, "", "OFF")
    '                'traiter toutes le groupe



    '            Case "CHIME"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sChime
    '                kar(LIGHTING1.unitcode) = 8
    '            Case Else
    '                WriteLog("ERR: Send ARC : Commande invalide : " & commande)
    '                Exit Sub
    '        End Select
    '        kar(LIGHTING1.filler) = 0
    '        ecrire(kar)
    '        If _DEBUG Then
    '            For Each bt As Byte In kar
    '                temp = temp & VB.Right("0" & Hex(bt), 2) & " "
    '            Next
    '            WriteLog("DBG: Send ARC : commande envoyée : " & temp)
    '        End If
    '    Catch ex As Exception
    '        WriteLog("ERR: Send ARC Exception : " & ex.Message)
    '    End Try
    'End Sub

    ' ''' <summary>Gestion du protocole ELRO AB400D</summary>
    ' ''' <param name="adresse">Adresse du type A1</param>
    ' ''' <param name="commande">commande ON, OFF</param>
    ' ''' <remarks></remarks>
    'Private Sub send_ELROAB400D(ByVal adresse As String, ByVal commande As String)
    '    Try
    '        Dim kar(LIGHTING1.size) As Byte
    '        Dim temp As String = ""

    '        If Not (adresse.Length = 2) Then
    '            WriteLog("ERR: Send ELRO AB400D : Adresse invalide : " & adresse)
    '            Exit Sub
    '        End If
    '        kar(LIGHTING1.housecode) = convert_housecode(adresse.Substring(0, 1))
    '        kar(LIGHTING1.unitcode) = adresse.Substring(1, 1)
    '        Select Case commande
    '            Case "OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOff
    '                WriteRetourSend(adresse, "", "OFF")
    '            Case "ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOn
    '                WriteRetourSend(adresse, "", "ON")
    '            Case Else
    '                WriteLog("ERR: Send ELRO AB400D : Commande invalide : " & commande)
    '                Exit Sub
    '        End Select
    '        kar(LIGHTING1.packetlength) = LIGHTING1.size
    '        kar(LIGHTING1.packettype) = LIGHTING1.pType
    '        kar(LIGHTING1.subtype) = 2  '0=X10, 1=ARC, 2=ELRO AB400D, 3=Waveman, 4=EMW200, 5=Impuls, 6=RisingSun, 7=Philips SBC
    '        kar(LIGHTING1.seqnbr) = bytSeqNbr
    '        kar(LIGHTING1.filler) = 0
    '        ecrire(kar)
    '        If _DEBUG Then
    '            For Each bt As Byte In kar
    '                temp = temp & VB.Right("0" & Hex(bt), 2) & " "
    '            Next
    '            WriteLog("DBG: Send ELRO AB400D : commande envoyée : " & temp)
    '        End If
    '    Catch ex As Exception
    '        WriteLog("ERR: Send ELRO AB400D Exception : " & ex.Message)
    '    End Try
    'End Sub

    ' ''' <summary>Gestion du protocole WAVEMAN</summary>
    ' ''' <param name="adresse">Adresse du type A1</param>
    ' ''' <param name="commande">commande ON, OFF</param>
    ' ''' <remarks></remarks>
    'Private Sub send_WAVEMAN(ByVal adresse As String, ByVal commande As String)
    '    Try
    '        Dim kar(LIGHTING1.size) As Byte
    '        Dim temp As String = ""

    '        If Not (adresse.Length = 2) Then
    '            WriteLog("ERR: Send WAVEMAN : Adresse invalide : " & adresse)
    '            Exit Sub
    '        End If
    '        kar(LIGHTING1.packetlength) = LIGHTING1.size
    '        kar(LIGHTING1.packettype) = LIGHTING1.pType
    '        kar(LIGHTING1.subtype) = 3  '0=X10, 1=ARC, 2=ELRO AB400D, 3=Waveman, 4=EMW200, 5=Impuls, 6=RisingSun, 7=Philips SBC
    '        kar(LIGHTING1.seqnbr) = bytSeqNbr
    '        kar(LIGHTING1.housecode) = convert_housecode(adresse.Substring(0, 1))
    '        kar(LIGHTING1.unitcode) = adresse.Substring(1, 1)
    '        Select Case commande
    '            Case "OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOff
    '                WriteRetourSend(adresse, "", "OFF")
    '            Case "ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOn
    '                WriteRetourSend(adresse, "", "ON")
    '            Case Else
    '                WriteLog("ERR: Send WAVEMAN : Commande invalide : " & commande)
    '                Exit Sub
    '        End Select
    '        kar(LIGHTING1.filler) = 0
    '        ecrire(kar)
    '        If _DEBUG Then
    '            For Each bt As Byte In kar
    '                temp = temp & VB.Right("0" & Hex(bt), 2) & " "
    '            Next
    '            WriteLog("DBG: Send WAVEMAN : commande envoyée : " & temp)
    '        End If
    '    Catch ex As Exception
    '        WriteLog("ERR: Send WAVEMAN Exception : " & ex.Message)
    '    End Try
    'End Sub

    ' ''' <summary>Gestion du protocole EMW200</summary>
    ' ''' <param name="adresse">Adresse du type A1 avec A/B/C et 1/2/3/4</param>
    ' ''' <param name="commande">commande ON, OFF, ALL_LIGHT_ON, ALL_LIGHT_OFF</param>
    ' ''' <remarks></remarks>
    'Private Sub send_EMW200(ByVal adresse As String, ByVal commande As String)
    '    Try
    '        Dim kar(LIGHTING1.size) As Byte
    '        Dim temp As String = ""

    '        If Not (adresse.Length = 2) Or CInt(adresse.Substring(1, 1)) > 4 Or (adresse.Substring(0, 1) <> "A" And adresse.Substring(0, 1) <> "B" And adresse.Substring(0, 1) <> "C") Then
    '            WriteLog("ERR: Send EMW200 : Adresse invalide : " & adresse)
    '            Exit Sub
    '        End If
    '        kar(LIGHTING1.housecode) = convert_housecode(adresse.Substring(0, 1))
    '        kar(LIGHTING1.unitcode) = adresse.Substring(1, 1)
    '        Select Case commande
    '            Case "OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOff
    '                WriteRetourSend(adresse, "", "OFF")
    '            Case "ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOn
    '                WriteRetourSend(adresse, "", "ON")
    '            Case "ALL_LIGHT_ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sAllOn
    '                WriteRetourSend(adresse, "", "ON")
    '            Case "ALL_LIGHT_OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sAllOff
    '                WriteRetourSend(adresse, "", "OFF")
    '            Case Else
    '                WriteLog("ERR: Send EMW200 : Commande invalide : " & commande)
    '                Exit Sub
    '        End Select
    '        kar(LIGHTING1.packetlength) = LIGHTING1.size
    '        kar(LIGHTING1.packettype) = LIGHTING1.pType
    '        kar(LIGHTING1.subtype) = 4  '0=X10, 1=ARC, 2=ELRO AB400D, 3=Waveman, 4=EMW200, 5=Impuls, 6=RisingSun, 7=Philips SBC
    '        kar(LIGHTING1.seqnbr) = bytSeqNbr
    '        kar(LIGHTING1.filler) = 0
    '        ecrire(kar)
    '        If _DEBUG Then
    '            For Each bt As Byte In kar
    '                temp = temp & VB.Right("0" & Hex(bt), 2) & " "
    '            Next
    '            WriteLog("DBG: Send EMW200 : commande envoyée : " & temp)
    '        End If
    '    Catch ex As Exception
    '        WriteLog("ERR: Send EMW200 Exception : " & ex.Message)
    '    End Try
    'End Sub

    ' ''' <summary>Gestion du protocole IMPULS</summary>
    ' ''' <param name="adresse">Adresse du type A1</param>
    ' ''' <param name="commande">commande ON, OFF</param>
    ' ''' <remarks></remarks>
    'Private Sub send_IMPULS(ByVal adresse As String, ByVal commande As String)
    '    Try
    '        Dim kar(LIGHTING1.size) As Byte
    '        Dim temp As String = ""

    '        If Not (adresse.Length = 2) Then
    '            WriteLog("ERR: Send IMPULS : Adresse invalide : " & adresse)
    '            Exit Sub
    '        End If
    '        kar(LIGHTING1.housecode) = convert_housecode(adresse.Substring(0, 1))
    '        kar(LIGHTING1.unitcode) = adresse.Substring(1, 1)
    '        Select Case commande
    '            Case "OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOff
    '                WriteRetourSend(adresse, "", "OFF")
    '            Case "ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOn
    '                WriteRetourSend(adresse, "", "ON")
    '            Case Else
    '                WriteLog("ERR: Send IMPULS : Commande invalide : " & commande)
    '                Exit Sub
    '        End Select
    '        kar(LIGHTING1.packetlength) = LIGHTING1.size
    '        kar(LIGHTING1.packettype) = LIGHTING1.pType
    '        kar(LIGHTING1.subtype) = 5  '0=X10, 1=ARC, 2=ELRO AB400D, 3=Waveman, 4=EMW200, 5=Impuls, 6=RisingSun, 7=Philips SBC
    '        kar(LIGHTING1.seqnbr) = bytSeqNbr
    '        kar(LIGHTING1.filler) = 0
    '        ecrire(kar)
    '        If _DEBUG Then
    '            For Each bt As Byte In kar
    '                temp = temp & VB.Right("0" & Hex(bt), 2) & " "
    '            Next
    '            WriteLog("DBG: Send IMPULS : commande envoyée : " & temp)
    '        End If
    '    Catch ex As Exception
    '        WriteLog("ERR: Send IMPULS Exception : " & ex.Message)
    '    End Try
    'End Sub

    ' ''' <summary>Gestion du protocole Philips</summary>
    ' ''' <param name="adresse">Adresse du type A1 avec A-P et 1-8</param>
    ' ''' <param name="commande">commande ON, OFF, ALL_LIGHT_ON, ALL_LIGHT_OFF</param>
    ' ''' <remarks></remarks>
    'Private Sub send_PHILIPS(ByVal adresse As String, ByVal commande As String)
    '    Try
    '        Dim kar(LIGHTING1.size) As Byte
    '        Dim temp As String = ""

    '        If Not (adresse.Length = 2) Or CInt(adresse.Substring(1, 1)) > 8 Or (adresse.Substring(0, 1) <> "A" And adresse.Substring(0, 1) <> "B" And adresse.Substring(0, 1) <> "C" And adresse.Substring(0, 1) <> "D" And adresse.Substring(0, 1) <> "E" And adresse.Substring(0, 1) <> "F" And adresse.Substring(0, 1) <> "G" And adresse.Substring(0, 1) <> "H" And adresse.Substring(0, 1) <> "I" And adresse.Substring(0, 1) <> "J" And adresse.Substring(0, 1) <> "K" And adresse.Substring(0, 1) <> "L" And adresse.Substring(0, 1) <> "M" And adresse.Substring(0, 1) <> "N" And adresse.Substring(0, 1) <> "O" And adresse.Substring(0, 1) <> "P") Then
    '            WriteLog("ERR: Send PHILIPS : Adresse invalide : " & adresse)
    '            Exit Sub
    '        End If
    '        kar(LIGHTING1.housecode) = convert_housecode(adresse.Substring(0, 1))
    '        kar(LIGHTING1.unitcode) = adresse.Substring(1, 1)
    '        Select Case commande
    '            Case "OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOff
    '                WriteRetourSend(adresse, "", "OFF")
    '            Case "ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sOn
    '                WriteRetourSend(adresse, "", "ON")
    '            Case "ALL_LIGHT_ON"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sAllOn
    '                WriteRetourSend(adresse, "", "ON")
    '            Case "ALL_LIGHT_OFF"
    '                kar(LIGHTING1.cmnd) = LIGHTING1.sAllOff
    '                WriteRetourSend(adresse, "", "OFF")
    '            Case Else
    '                WriteLog("ERR: Send PHILIPS : Commande invalide : " & commande)
    '                Exit Sub
    '        End Select
    '        kar(LIGHTING1.packetlength) = LIGHTING1.size
    '        kar(LIGHTING1.packettype) = LIGHTING1.pType
    '        kar(LIGHTING1.subtype) = 7 '0=X10, 1=ARC, 2=ELRO AB400D, 3=Waveman, 4=EMW200, 5=Impuls, 6=RisingSun, 7=Philips SBC
    '        kar(LIGHTING1.seqnbr) = bytSeqNbr
    '        kar(LIGHTING1.filler) = 0
    '        ecrire(kar)
    '        If _DEBUG Then
    '            For Each bt As Byte In kar
    '                temp = temp & VB.Right("0" & Hex(bt), 2) & " "
    '            Next
    '            WriteLog("DBG: Send PHILIPS : commande envoyée : " & temp)
    '        End If
    '    Catch ex As Exception
    '        WriteLog("ERR: Send PHILIPS Exception : " & ex.Message)
    '    End Try
    'End Sub

#End Region

#Region "Write"

    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "RFXtrx", STRGS.Right(message, message.Length - 5))
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXtrx", STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXtrx", message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXtrx WriteLog", ex.Message)
        End Try
    End Sub

    Private Sub WriteBattery(ByVal adresse As String)
        Try
            'Forcer le . 
            'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            'My.Application.ChangeCulture("en-US")

            'log tous les paquets en mode debug
            If _DEBUG Then WriteLog("DBG: WriteBattery : receive from " & adresse)

            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte
            If DateTime.Now < DateAdd(DateInterval.Second, 10, dateheurelancement) Then Exit Sub 'on ne traite rien pendant les 10 premieres secondes

            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, "", Me._ID, True)
            If (listedevices.Count >= 1) Then
                'on a trouvé un ou plusieurs composants avec cette adresse, on prend le premier
                WriteLog(listedevices.Item(0).Name & " (" & adresse & ") : Battery Empty")
            Else
                'device pas trouvé
                WriteLog("ERR: Device non trouvé : " & adresse & ": Battery Empty")

                'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)

            End If
            listedevices = Nothing
        Catch ex As Exception
            WriteLog("ERR: WriteBattery Exception : " & ex.Message & " --> " & adresse)
        End Try
    End Sub

    Private Sub WriteRetourSend(ByVal adresse As String, ByVal type As String, ByVal valeur As String)
        Try
            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte

            'Forcer le . 
            'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            'My.Application.ChangeCulture("en-US")

            'log tous les paquets en mode debug
            If _DEBUG Then WriteLog("DBG: WriteRetour send to " & adresse & " (" & type & ") -> " & valeur)

            'on ne traite rien pendant les 6 premieres secondes
            If DateTime.Now > DateAdd(DateInterval.Second, 6, dateheurelancement) Then
                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, type, Me._ID, True)
                If (listedevices.Count = 1) Then
                    'un device trouvé on maj la value
                    If valeur = "ON" Then
                        If TypeOf listedevices.Item(0).Value Is Boolean Then
                            listedevices.Item(0).Value = True
                        ElseIf TypeOf listedevices.Item(0).Value Is Long Or TypeOf listedevices.Item(0).Value Is Integer Then
                            listedevices.Item(0).Value = 100
                        Else
                            listedevices.Item(0).Value = "ON"
                        End If
                    ElseIf valeur = "OFF" Then
                        If TypeOf listedevices.Item(0).Value Is Boolean Then
                            listedevices.Item(0).Value = False
                        ElseIf TypeOf listedevices.Item(0).Value Is Long Or TypeOf listedevices.Item(0).Value Is Integer Then
                            listedevices.Item(0).Value = 0
                        Else
                            listedevices.Item(0).Value = "OFF"
                        End If
                    Else
                        listedevices.Item(0).Value = valeur
                    End If
                ElseIf (listedevices.Count > 1) Then
                    WriteLog("ERR: Plusieurs devices correspondent à : " & type & " " & adresse & ":" & valeur)
                Else
                    WriteLog("ERR: Device non trouvé : " & type & " " & adresse & ":" & valeur)
                End If
                listedevices = Nothing
            End If

        Catch ex As Exception
            WriteLog("ERR: Writeretour Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub WriteRetour(ByVal adresse As String, ByVal type As String, ByVal valeur As String)
        Try
            'Forcer le . 
            'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            'My.Application.ChangeCulture("en-US")

            'log tous les paquets en mode debug
            If _DEBUG Then WriteLog("DBG: WriteRetour : receive from " & adresse & " (" & type & ") -> " & valeur)

            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte
            If DateTime.Now < DateAdd(DateInterval.Second, 10, dateheurelancement) Then Exit Sub 'on ne traite rien pendant les 10 premieres secondes

            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, type, Me._ID, True)
            If (listedevices.Count = 1) Then
                'un device trouvé 
                If STRGS.InStr(valeur, "CFG:") > 0 Then
                    'c'est un message de config, on log juste
                    WriteLog(listedevices.Item(0).name & " : " & valeur)
                Else
                    'on maj la value si la durée entre les deux receptions est > à 1.5s
                    If (DateTime.Now - Date.Parse(listedevices.Item(0).LastChange)).TotalMilliseconds > 1500 Then
                        If valeur = "ON" Then
                            If TypeOf listedevices.Item(0).Value Is Boolean Then
                                listedevices.Item(0).Value = True
                            ElseIf TypeOf listedevices.Item(0).Value Is Long Or TypeOf listedevices.Item(0).Value Is Integer Then
                                listedevices.Item(0).Value = 100
                            Else
                                listedevices.Item(0).Value = "ON"
                            End If
                        ElseIf valeur = "OFF" Then
                            If TypeOf listedevices.Item(0).Value Is Boolean Then
                                listedevices.Item(0).Value = False
                            ElseIf TypeOf listedevices.Item(0).Value Is Long Or TypeOf listedevices.Item(0).Value Is Integer Then
                                listedevices.Item(0).Value = 0
                            Else
                                listedevices.Item(0).Value = "OFF"
                            End If
                        Else
                            listedevices.Item(0).Value = valeur
                        End If
                    Else
                        WriteLog("DBG: Reception < 1.5s de deux valeurs pour le meme composant : " & listedevices.Item(0).name & ":" & valeur)
                    End If
                End If
            ElseIf (listedevices.Count > 1) Then
                WriteLog("ERR: Plusieurs devices correspondent à : " & type & " " & adresse & ":" & valeur)
            Else

                WriteLog("ERR: Device non trouvé : " & type & " " & adresse & ":" & valeur)

                'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)

            End If
            listedevices = Nothing
        Catch ex As Exception
            WriteLog("ERR: Writeretour Exception : " & ex.Message)
        End Try
    End Sub

#End Region

End Class