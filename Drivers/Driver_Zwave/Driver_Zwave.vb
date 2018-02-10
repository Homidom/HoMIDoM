'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports OpenZWaveDotNet
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO



Public Class Driver_ZWave

    ' Auteur : Laurent
    ' Date : 28/02/2012

    ''' <summary>Class Driver_ZWave, permet de communiquer avec le controleur Z-Wave</summary>
    ''' <remarks>Nécessite l'installation des pilotes fournis sur le site </remarks>
    <Serializable()> Public Class Driver_ZWave
        Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
        '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
        'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
        Dim _ID As String = "57BCAA20-5CD2-11E1-AA83-88244824019B"
        Dim _Nom As String = "Z-Wave"
        Dim _Enable As Boolean = False
        Dim _Description As String = "Controleur Z-Wave"
        Dim _StartAuto As Boolean = False
        Dim _Protocol As String = "COM"
        Dim _IsConnect As Boolean = False
        Dim _IP_TCP As String = "@"
        Dim _Port_TCP As String = "@"
        Dim _IP_UDP As String = "@"
        Dim _Port_UDP As String = "@"
        Dim _Com As String = "COM1"
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
        Dim _AutoDiscover As Boolean = False

        Private m_manager As New ZWManager
        Private m_options As New ZWOptions
        Private m_notification As ZWNotification
        <NonSerialized()> Private Shared m_nodeList As New List(Of Node)

        Private m_nodesReady As Boolean = False
        Private m_homeId As UInt32 = 0
        Private dateheurelancement As DateTime

        'parametres avancés
        Dim _DEBUG As Boolean = False
        Dim _AFFICHELOG As Boolean = False
        Dim _STARTIDLETIME As Integer = 10

        'Ajoutés dans les ppt avancés dans New()


#End Region

#Region "Variables Internes"
        ' Variables de gestion du port COM
        Dim _networkkey As String = "0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F,0x10"
        Dim MyRep As String = System.IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)

        ' variables port serie
        Private WithEvents port As New System.IO.Ports.SerialPort
        Private port_name As String = ""
        Dim _baudspeed As Integer = 9600
        Dim _nbrebit As Integer = 8
        Dim _parity As IO.Ports.Parity = IO.Ports.Parity.None
        Dim _nbrebitstop As IO.Ports.StopBits = IO.Ports.StopBits.One
		
        Dim _NomFileConfigZWave As String = ""
        Dim _Adr1Txt As New ArrayList()
        Dim _Getconfig As Boolean = False

        ' -----------------   Ajout des declarations pour OpenZWave
        Private g_initFailed As Boolean = False

        ' Defini le niveau des messages d'erreurs/informations de la log OpenZWave
        Enum LogLevel
            LogLevel_None
            LogLevel_Always
            LogLevel_Fatal
            LogLevel_Error
            LogLevel_Warning
            LogLevel_Alert
            LogLevel_Info
            LogLevel_Detail
            LogLevel_Debug
            LogLevel_Internal
        End Enum

        Enum CommandClass As Byte
            COMMAND_CLASS_NO_OPERATION = 0                        ' 0x00
            COMMAND_CLASS_BASIC = 32                              ' 0x20
            COMMAND_CLASS_BASIC_V2 = 32                           ' 0x20
            COMMAND_CLASS_CONTROLLER_REPLICATION = 33             ' 0x21
            COMMAND_CLASS_APPLICATION_STATUS = 34                 ' 0x22
            COMMAND_CLASS_ZIP_SERVICES = 35                       ' 0x23
            COMMAND_CLASS_ZIP = 35                                ' 0x23
            COMMAND_CLASS_ZIP_V2 = 35                             ' 0x23
            COMMAND_CLASS_ZIP_SERVER = 36                         ' 0x24
            COMMAND_CLASS_SECURITY_PANEL_MODE = 36                ' 0x24
            COMMAND_CLASS_SWITCH_BINARY = 37                      ' 0x25
            COMMAND_CLASS_SWITCH_BINARY_V2 = 37                   ' 0x25
            COMMAND_CLASS_SWITCH_MULTILEVEL = 38                  ' 0x26
            COMMAND_CLASS_SWITCH_MULTILEVEL_V2 = 38               ' 0x26
            COMMAND_CLASS_SWITCH_MULTILEVEL_V3 = 38               ' 0x26
            COMMAND_CLASS_SWITCH_MULTILEVEL_V4 = 38               ' 0x26
            COMMAND_CLASS_SWITCH_ALL = 39                         ' 0x27
            COMMAND_CLASS_SWITCH_TOGGLE_BINARY = 40               ' 0x28
            COMMAND_CLASS_SWITCH_TOGGLE_MULTILEVEL = 41           ' 0x29
            COMMAND_CLASS_CHIMNEY_FAN = 42                        ' 0x2A
            COMMAND_CLASS_SCENE_ACTIVATION = 43                   ' 0x2B
            COMMAND_CLASS_SCENE_ACTUATOR_CONF = 44                ' 0x2C
            COMMAND_CLASS_SCENE_CONTROLLER_CONF = 45              ' 0x2D
            COMMAND_CLASS_ZIP_CLIENT = 46                         ' 0x2E
            COMMAND_CLASS_SECURITY_PANEL_ZONE = 46                ' 0x2E
            COMMAND_CLASS_ZIP_ADV_SERVICES = 47                   ' 0x2F
            COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR = 47         ' 0x2F
            COMMAND_CLASS_SENSOR_BINARY = 48                      ' 0x30
            COMMAND_CLASS_SENSOR_BINARY_V2 = 48                   ' 0x30
            COMMAND_CLASS_SENSOR_MULTILEVEL = 49                  ' 0x31
            COMMAND_CLASS_SENSOR_MULTILEVEL_V2 = 49               ' 0x31
            COMMAND_CLASS_SENSOR_MULTILEVEL_V3 = 49               ' 0x31
            COMMAND_CLASS_SENSOR_MULTILEVEL_V4 = 49               ' 0x31
            COMMAND_CLASS_SENSOR_MULTILEVEL_V5 = 49               ' 0x31
            COMMAND_CLASS_SENSOR_MULTILEVEL_V6 = 49               ' 0x31
            COMMAND_CLASS_SENSOR_MULTILEVEL_V7 = 49               ' 0x31
            COMMAND_CLASS_METER = 50                              ' 0x32
            COMMAND_CLASS_METER_V2 = 50                           ' 0x32
            COMMAND_CLASS_METER_V3 = 50                           ' 0x32
            COMMAND_CLASS_METER_V4 = 50                           ' 0x32
            COMMAND_CLASS_COLOR = 51                              ' 0x33
            COMMAND_CLASS_COLOR_CONTROL = 51                      ' 0x33
            COMMAND_CLASS_ZIP_ADV_CLIENT  = 52                    ' 0x34
            COMMAND_CLASS_METER_PULSE = 53                        ' 0x35
            COMMAND_CLASS_BASIC_TARIFF_INFO = 54                  ' 0x36
            COMMAND_CLASS_HRV_STATUS = 55                         ' 0x37
            COMMAND_CLASS_THERMOSTAT_HEATING = 56                 ' 0x38
            COMMAND_CLASS_HRV_CONTROL = 57                        ' 0x39
            COMMAND_CLASS_DCP_CONFIG = 58                         ' 0x3A
            COMMAND_CLASS_DCP_MONITOR = 59                        ' 0x3B
            COMMAND_CLASS_METER_TBL_CONFIG = 60                   ' 0x3C
            COMMAND_CLASS_METER_TBL_MONITOR = 61                  ' 0x3D
            COMMAND_CLASS_METER_TBL_MONITOR_V2 = 61               ' 0x3D
            COMMAND_CLASS_METER_TBL_PUSH = 62                     ' 0x3E
            COMMAND_CLASS_PREPAYMENT = 63                         ' 0x3F
            COMMAND_CLASS_THERMOSTAT_MODE = 64                    ' 0x40
            COMMAND_CLASS_THERMOSTAT_MODE_V2 = 64                 ' 0x40
            COMMAND_CLASS_THERMOSTAT_MODE_V3 = 64                 ' 0x40
            COMMAND_CLASS_PREPAYMENT_ENCAPSULATION = 65           ' 0x41
            COMMAND_CLASS_THERMOSTAT_OPERATING_STATE = 66         ' 0x42
            COMMAND_CLASS_THERMOSTAT_OPERATING_STATE_V2 = 66      ' 0x42
            COMMAND_CLASS_THERMOSTAT_SETPOINT = 67                ' 0x43
            COMMAND_CLASS_THERMOSTAT_SETPOINT_V2 = 67             ' 0x43
            COMMAND_CLASS_THERMOSTAT_SETPOINT_V3 = 67             ' 0x43
            COMMAND_CLASS_THERMOSTAT_FAN_MODE = 68                ' 0x44
            COMMAND_CLASS_THERMOSTAT_FAN_MODE_V2 = 68             ' 0x44
            COMMAND_CLASS_THERMOSTAT_FAN_MODE_V3 = 68             ' 0x44
            COMMAND_CLASS_THERMOSTAT_FAN_MODE_V4 = 68             ' 0x44
            COMMAND_CLASS_THERMOSTAT_FAN_STATE = 69               ' 0x45
            COMMAND_CLASS_THERMOSTAT_FAN_STATE_V2 = 69            ' 0x45
            COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE = 70           ' 0x46
            COMMAND_CLASS_THERMOSTAT_SETBACK = 71                 ' 0x47
            COMMAND_CLASS_NOTIFICATION = 71                       ' 0x47
            COMMAND_CLASS_NOTIFICATION_V2 = 71                    ' 0x47
            COMMAND_CLASS_NOTIFICATION_V3 = 71                    ' 0x47
            COMMAND_CLASS_NOTIFICATION_V4 = 71                    ' 0x47
            COMMAND_CLASS_NOTIFICATION_V5 = 71                    ' 0x47
            COMMAND_CLASS_NOTIFICATION_V6 = 71                    ' 0x47
            COMMAND_CLASS_NOTIFICATION_V7 = 71                    ' 0x47
            COMMAND_CLASS_NOTIFICATION_V8 = 71                    ' 0x47
            COMMAND_CLASS_RATE_TBL_CONFIG = 72                    ' 0x48
            COMMAND_CLASS_RATE_TBL_MONITOR = 73                   ' 0x49
            COMMAND_CLASS_TARIFF_CONFIG = 74                      ' 0x4A
            COMMAND_CLASS_TARIFF_TBL_MONITOR = 75                 ' 0x4B
            COMMAND_CLASS_DOOR_LOCK_LOGGING = 76                  ' 0x4C
            COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC = 77           ' 0x4D
            COMMAND_CLASS_SCHEDULE_ENTRY_LOCK = 78                ' 0x4E
            COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V2 = 78             ' 0x4E
            COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3 = 78             ' 0x4E
            COMMAND_CLASS_ZIP_6LOWPAN = 79                        ' 0x4F
            COMMAND_CLASS_BASIC_WINDOW_COVERING = 80              ' 0x50
            COMMAND_CLASS_MTP_WINDOW_COVERING = 81                ' 0x51
            COMMAND_CLASS_NETWORK_MANAGEMENT_PROXY = 82           ' 0x52
            COMMAND_CLASS_SCHEDULE = 83                           ' 0x53
            COMMAND_CLASS_NETWORK_MANAGEMENT_PRIMARY = 84         ' 0x54
            COMMAND_CLASS_TRANSPORT_SERVICE = 85                  ' 0x55
            COMMAND_CLASS_TRANSPORT_SERVICE_V2 = 85               ' 0x55
            COMMAND_CLASS_CRC_16_ENCAP = 86                       ' 0x56
            COMMAND_CLASS_APPLICATION_CAPABILITY = 86             ' 0x57
            COMMAND_CLASS_ZIP_ND = 88                             ' 0x58
            COMMAND_CLASS_ASSOCIATION_GRP_INFO = 89               ' 0x59
            COMMAND_CLASS_ASSOCIATION_GRP_INFO_V2 = 89            ' 0x59
            COMMAND_CLASS_DEVICE_RESET_LOCALLY = 90               ' 0x5A
            COMMAND_CLASS_CENTRAL_SCENE = 91                      ' 0x5B
            COMMAND_CLASS_CENTRAL_SCENE_V2 = 91                   ' 0x5B
            COMMAND_CLASS_IP_ASSOCIATION = 92                     ' 0x5C
            COMMAND_CLASS_ANTITHEFT = 93                          ' 0x5D
            COMMAND_CLASS_ANTITHEFT_V2 = 93                       ' 0x5D
            COMMAND_CLASS_ZWAVE_PLUS_INFO = 94                    ' 0x5E
            COMMAND_CLASS_ZWAVE_PLUS_INFO_V2 = 94                 ' 0x5E
            COMMAND_CLASS_ZIP_GATEWAY = 95                        ' 0x5F
            COMMAND_CLASS_MULTI_CHANNEL = 96                      ' 0x60
            COMMAND_CLASS_MULTI_CHANNEL_V2 = 96                   ' 0x60
            COMMAND_CLASS_MULTI_CHANNEL_V3 = 96                   ' 0x60
            COMMAND_CLASS_MULTI_CHANNEL_V4 = 96                   ' 0x60
            COMMAND_CLASS_MULTI_INSTANCE = 96                     ' 0x60
            COMMAND_CLASS_MULTI_INSTANCE_V2 = 96                  ' 0x60
            COMMAND_CLASS_MULTI_INSTANCE_V3 = 96                  ' 0x60
            COMMAND_CLASS_MULTI_INSTANCE_V4 = 96                  ' 0x60
            COMMAND_CLASS_ZIP_PORTAL = 97                         ' 0x61
            COMMAND_CLASS_DOOR_LOCK = 98                          ' 0x62
            COMMAND_CLASS_DOOR_LOCK_V2 = 98                       ' 0x62
            COMMAND_CLASS_DOOR_LOCK_V3 = 98                       ' 0x62
            COMMAND_CLASS_USER_CODE = 99                          ' 0x63
            COMMAND_CLASS_APPLIANCE = 100                         ' 0x64
            COMMAND_CLASS_DMX = 101                               ' 0x65
            COMMAND_CLASS_BARRIER_OPERATOR = 102                  ' 0x66
            COMMAND_CLASS_ZIP_NAMING = 104                        ' 0x68
            COMMAND_CLASS_WINDOW_COVERING = 105                   ' 0x69
            COMMAND_CLASS_CONFIGURATION = 112                     ' 0x70
            COMMAND_CLASS_CONFIGURATION_V2 = 112                  ' 0x70
            COMMAND_CLASS_CONFIGURATION_V3 = 112                  ' 0x70
            COMMAND_CLASS_ALARM = 113                             ' 0x71
            COMMAND_CLASS_ALARM_V2 = 113                          ' 0x71
            COMMAND_CLASS_ALARM_V3 = 113                          ' 0x71
            COMMAND_CLASS_ALARM_V4 = 113                          ' 0x71
            COMMAND_CLASS_ALARM_V5 = 113                          ' 0x71
            COMMAND_CLASS_MANUFACTURER_SPECIFIC = 114             ' 0x72
            COMMAND_CLASS_MANUFACTURER_SPECIFIC_V2 = 114          ' 0x72
            COMMAND_CLASS_POWERLEVEL = 115                        ' 0x73
            '            COMMAND_CLASS_ = 116                                  ' 0x74
            COMMAND_CLASS_PROTECTION = 117                        ' 0x75
            COMMAND_CLASS_PROTECTION_V2 = 117                     ' 0x75
            COMMAND_CLASS_LOCK = 118                              ' 0x76
            COMMAND_CLASS_NODE_NAMING = 119                       ' 0x77
            COMMAND_CLASS_FIRMWARE_UPDATE_MD = 122                ' 0x7A
            COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2 = 122             ' 0x7A
            COMMAND_CLASS_FIRMWARE_UPDATE_MD_V3 = 122             ' 0x7A
            COMMAND_CLASS_FIRMWARE_UPDATE_MD_V4 = 122             ' 0x7A
            COMMAND_CLASS_GROUPING_NAME = 123                     ' 0x7B
            COMMAND_CLASS_REMOTE_ASSOCIATION_ACTIVATE = 124       ' 0x7C
            COMMAND_CLASS_REMOTE_ASSOCIATION = 125                ' 0x7D
            COMMAND_CLASS_BATTERY = 128                           ' 0x80
            COMMAND_CLASS_CLOCK = 129                             ' 0x81
            COMMAND_CLASS_HAIL = 130                              ' 0x82
            COMMAND_CLASS_WAKE_UP = 132                           ' 0x84
            COMMAND_CLASS_WAKE_UP_V2 = 132                        ' 0x84
            COMMAND_CLASS_ASSOCIATION = 133                       ' 0x85
            COMMAND_CLASS_ASSOCIATION_V2 = 133                    ' 0x85
            COMMAND_CLASS_VERSION = 134                           ' 0x86
            COMMAND_CLASS_VERSION_V2 = 134                        ' 0x86
            COMMAND_CLASS_INDICATOR = 135                         ' 0x87
            COMMAND_CLASS_PROPRIETARY = 136                       ' 0x88
            COMMAND_CLASS_LANGUAGE = 137                          ' 0x89
            COMMAND_CLASS_TIME = 138                              ' 0x8A
            COMMAND_CLASS_TIME_V2 = 138                           ' 0x8A
            COMMAND_CLASS_TIME_PARAMETERS = 139                   ' 0x8B
            COMMAND_CLASS_GEOGRAPHIC_LOCATION = 140               ' 0x8C
            COMMAND_CLASS_COMPOSITE = 141                         ' 0x8D
            COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION = 142         ' 0x8E
            COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V2 = 142      ' 0x8E
            COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3 = 142      ' 0x8E
            COMMAND_CLASS_MULTI_INSTANCE_ASSOCIATION = 142        ' 0x8E
            COMMAND_CLASS_MULTI_INSTANCE_ASSOCIATION_V2 = 142     ' 0x8E
            COMMAND_CLASS_MULTI_INSTANCE_ASSOCIATION_V3 = 142     ' 0x8E
            COMMAND_CLASS_MULTI_CMD = 143                         ' 0x8F
            COMMAND_CLASS_ENERGY_PRODUCTION = 144                 ' 0x90
            COMMAND_CLASS_MANUFACTURER_PROPRIETARY = 145          ' 0x91
            COMMAND_CLASS_SCREEN_MD = 146                         ' 0x92
            COMMAND_CLASS_SCREEN_MD_V2 = 146                      ' 0x92
            COMMAND_CLASS_SCREEN_ATTRIBUTES = 147                 ' 0x93
            COMMAND_CLASS_SCREEN_ATTRIBUTES_V2 = 147              ' 0x93
            COMMAND_CLASS_SIMPLE_AV_CONTROL = 148                 ' 0x94
            COMMAND_CLASS_AV_CONTENT_DIRECTORY_MD = 149           ' 0x95
            COMMAND_CLASS_AV_RENDERER_STATUS = 150                ' 0x96
            COMMAND_CLASS_AV_CONTENT_SEARCH_MD = 151              ' 0x97
            COMMAND_CLASS_SECURITY = 152                          ' 0x98
            COMMAND_CLASS_AV_TAGGING_MD = 153                     ' 0x99
            COMMAND_CLASS_IP_CONFIGURATION = 154                  ' 0x9A
            COMMAND_CLASS_ASSOCIATION_COMMAND_CONFIGURATION = 155 ' 0x9B
            COMMAND_CLASS_SENSOR_ALARM = 156                      ' 0x9C
            COMMAND_CLASS_SILENCE_ALARM = 157                     ' 0x9D
            COMMAND_CLASS_SENSOR_CONFIGURATION = 158              ' 0x9E
            COMMAND_CLASS_MARK = 239                              ' 0xEF
            COMMAND_CLASS_NON_INTEROPERABLE = 240                 ' 0xF0
        End Enum

        ' Definition d'un noeud Zwave 
        <Serializable()> Public Class Node

            Dim m_id As Byte = 0
            Dim m_homeId As UInt32 = 0
            Dim m_name As String = ""
            Dim m_location As String = ""
            Dim m_label As String = ""
            Dim m_manufacturer As String = ""
            Dim m_product As String = ""
            Dim m_values As New List(Of ZWValueID)
            Dim m_commandClass As New List(Of CommandClass)
            Dim m_groups As New List(Of Byte)

            Public Property ID() As Byte
                Get
                    Return m_id
                End Get
                Set(ByVal value As Byte)
                    m_id = value
                End Set
            End Property

            Public Property HomeID() As UInt32
                Get
                    Return m_homeId
                End Get
                Set(ByVal value As UInt32)
                    m_homeId = value
                End Set
            End Property

            Public Property Name() As String
                Get
                    Return m_name
                End Get
                Set(ByVal value As String)
                    m_name = value
                End Set
            End Property

            Public Property Location() As String
                Get
                    Return m_location
                End Get
                Set(ByVal value As String)
                    m_location = value
                End Set
            End Property

            Public Property Label() As String
                Get
                    Return m_label
                End Get
                Set(ByVal value As String)
                    m_label = value
                End Set
            End Property

            Public Property Manufacturer() As String
                Get
                    Return m_manufacturer
                End Get
                Set(ByVal value As String)
                    m_manufacturer = value
                End Set
            End Property

            Public Property Product() As String
                Get
                    Return m_product
                End Get
                Set(ByVal value As String)
                    m_product = value
                End Set
            End Property

            Public Property Values() As List(Of ZWValueID)
                Get
                    Return m_values
                End Get
                Set(value As List(Of ZWValueID))
                    m_values = value
                End Set
            End Property

            Public Property CommandClass() As List(Of CommandClass)
                Get
                    Return m_commandClass
                End Get
                Set(ByVal value As List(Of CommandClass))
                    m_CommandClass = value
                End Set
            End Property
            Public Property Groups() As List(Of Byte)
                Get
                    Return m_groups
                End Get
                Set(ByVal value As List(Of Byte))
                    m_groups = value
                End Set
            End Property

            Shared Sub New()

            End Sub

            Sub AddValue(ByVal valueID As ZWValueID)
                m_values.Add(valueID)
            End Sub

            Sub RemoveValue(ByVal valueID As ZWValueID)
                m_values.Remove(valueID)
            End Sub

            Sub SetValue(ByVal valueID As ZWValueID)
                Dim valueIndex As Integer = 0
                Dim index As Integer = 0

                While index < m_values.Count
                    If m_values(index).GetId() = valueID.GetId() Then
                        valueIndex = index
                        Exit While
                    End If
                    System.Math.Max(System.Threading.Interlocked.Increment(index), index - 1)
                End While

                If valueIndex >= 0 Then
                    m_values(valueIndex) = valueID
                Else
                    AddValue(valueID)
                End If
            End Sub
        End Class


#End Region

#Region "Propriétés génériques"
        Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
            Set(ByVal value As String)
                _IdSrv = value
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
        Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
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
                If value >= 1 Then
                    _Refresh = value

                End If
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
        Public Property AutoDiscover() As Boolean Implements HoMIDom.HoMIDom.IDriver.AutoDiscover
            Get
                Return _AutoDiscover
            End Get
            Set(ByVal value As Boolean)
                _AutoDiscover = value
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
        ''' <param name="MyDevice">Objet représentant le Device </param>
        ''' <param name="Command">Nom de la commande avancée à éxécuter</param>
        ''' <param name="Param">tableau de paramétres</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
            Dim retour As Boolean = False
            Dim NodeTemp As New Node
            ' Permet de construire le message à afficher dans la console
            '            Dim texteCommande As String

            Try
                If MyDevice IsNot Nothing Then
                    'Pas de commande demandée donc erreur
                    If Command = "" Then
                        Return False
                    Else
                        '                        texteCommande = UCase(Command)
                        'permet de gerer la compatibilité du driver avec listbox
                        Dim adr1 As String = ""
                        If InStr(MyDevice.Adresse1, "#") > 0 Then
                            adr1 = Trim(Left(MyDevice.Adresse1, InStr(MyDevice.Adresse1, "#") - 1))
                        Else
                            adr1 = MyDevice.Adresse1
                        End If

                        WriteLog("DBG: ExecuteCommand, Passage de la commande " & UCase(Command) & " Param(0) -> " & Param(0) & " / Param(1) -> " & Param(1))

                        ' Traitement de la commande 
                        Select Case UCase(Command)

                            Case "ALL_LIGHT_ON"
                                m_manager.SwitchAllOn(m_homeId)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande ALL_LIGHT_ON")

                            Case "ALL_LIGHT_OFF"
                                m_manager.SwitchAllOff(m_homeId)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande ALL_LIGHT_OFF")

                            Case "SETNAME"
                                NodeTemp = GetNode(m_homeId, adr1)
                                m_manager.SetNodeName(m_homeId, NodeTemp.ID, MyDevice.Name)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande Set Name avec le nom = " & MyDevice.Name)

                            Case "GETNAME"
                                Dim TempName As String
                                NodeTemp = GetNode(m_homeId, adr1)
                                TempName = m_manager.GetNodeName(m_homeId, NodeTemp.ID)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande Get Name avec le nom = " & TempName)

                            Case "SETCONFIGPARAM"
                                Dim RetourSet As Boolean
                                NodeTemp = GetNode(m_homeId, adr1)
                                RetourSet = m_manager.SetConfigParam(m_homeId, NodeTemp.ID, Param(0), Param(1))
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande SETCONFIGPARAM pour le noeud " & NodeTemp.Name)
                                If RetourSet Then
                                    WriteLog("ExecuteCommand, Parametre " & Param(0) & " modifié avec succès sur le noeud " & NodeTemp.ID)
                                Else
                                    WriteLog("ERR: " & "ExecuteCommand, Parametre " & Param(0) & " erreur de la modification sur le noeud " & NodeTemp.ID)
                                End If

                            Case "GETCONFIGPARAM"
                                NodeTemp = GetNode(m_homeId, adr1)
                                m_manager.RequestConfigParam(m_homeId, NodeTemp.ID, Param(0))
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande SETCONFIGPARAM pour le noeud " & NodeTemp.Name)

                            Case "GETALLCONFIGPARAMS"
                                NodeTemp = GetNode(m_homeId, adr1)
                                m_manager.RequestAllConfigParams(m_homeId, NodeTemp.ID)
                                WriteLog("ExecuteCommand, Passage par la commande GETALLCONFIGPARAMS -> le noeud " & NodeTemp.ID)

                            Case "REQUESTNODESTATE"
                                NodeTemp = GetNode(m_homeId, adr1)
                                m_manager.RequestNodeState(m_homeId, NodeTemp.ID)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande REQUESTNODESTATE = " & NodeTemp.Name)

                            Case "REQUESTNODEDYNAMIC"
                                NodeTemp = GetNode(m_homeId, adr1)
                                If m_manager.RequestNodeDynamic(m_homeId, NodeTemp.ID) Then
                                    WriteLog("ExecuteCommand, Passage par la commande REQUESTNODEDYNAMIC -> le noeud " & NodeTemp.ID & " OK")
                                Else
                                    WriteLog("ERR: ExecuteCommand, Passage par la commande REQUESTNODEDYNAMIC -> le noeud " & NodeTemp.ID & " Impossible")
                                End If

                            Case "TESTNETWORKNODE"
                                NodeTemp = GetNode(m_homeId, adr1)
                                m_manager.TestNetworkNode(m_homeId, NodeTemp.ID, 1)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande TESTNETWORKNODE = " & NodeTemp.Name)

                            Case "REQUESTNETWORKUPDATE"
                                NodeTemp = GetNode(m_homeId, adr1)
                                m_manager.RequestNetworkUpdate(m_homeId, NodeTemp.ID)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande REQUESTNETWORKUPDATE = " & NodeTemp.Name)

                            Case "REQUESTNODENEIGHBORUPDATE"
                                NodeTemp = GetNode(m_homeId, adr1)
                                m_manager.RequestNodeNeighborUpdate(m_homeId, NodeTemp.ID)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande REQUESTNODENEIGHBORUPDATE = " & NodeTemp.Name)

                            Case "GETNUMGROUPS"
                                NodeTemp = GetNode(m_homeId, adr1)
                                Dim NumGroup As Byte = m_manager.GetNumGroups(m_homeId, NodeTemp.ID)
                                If NumGroup Then
                                    WriteLog("ExecuteCommand, Passage par la commande GETNUMGROUPS = " & NumGroup & " pour le noeud " & NodeTemp.ID)
                                Else
                                    WriteLog("ERR: " & "ExecuteCommand, Erreur dans la commande GETNUMGROUPS  pour le noeud " & NodeTemp.ID)
                                End If

                            Case "ADD_ASSOCIATION"
                                NodeTemp = GetNode(m_homeId, adr1)             'Group Idx   NodeId
                                m_manager.AddAssociation(m_homeId, NodeTemp.ID, Param(0), Param(1))
                                WriteLog("ExecuteCommand, Passage par la commande ADD_ASSOCIATION -> Groupe Id :" & Param(0) & ", Noeud : " & Param(1) & " sur le noeud " & NodeTemp.ID)
                                m_manager.RefreshNodeInfo(m_homeId, NodeTemp.ID)

                            Case "REMOVE_ASSOCIATION"
                                NodeTemp = GetNode(m_homeId, adr1)                'Group Idx   NodeId
                                m_manager.RemoveAssociation(m_homeId, NodeTemp.ID, Param(0), Param(1))
                                WriteLog("ExecuteCommand, Passage par la commande REMOVE_ASSOCIATION -> Groupe Id :" & Param(0) & ", Noeud : " & Param(1) & " sur le noeud " & NodeTemp.ID)
                                m_manager.RefreshNodeInfo(m_homeId, NodeTemp.ID)

                            Case "GET_ASSOCIATION"
                                Dim NumGroup As Integer
                                Dim NumAssociation As Integer
                                Dim Association() As Byte = Nothing

                                NodeTemp = GetNode(m_homeId, adr1)
                                NumGroup = m_manager.GetNumGroups(m_homeId, NodeTemp.ID)
                                If (Param(0) >= 1 And Param(0) <= NumGroup) Then                     'Group Idx
                                    NumAssociation = m_manager.GetAssociations(m_homeId, NodeTemp.ID, Param(0), Association)
                                    If NumAssociation Then
                                        WriteLog("ExecuteCommand, GET_ASSOCIATION -> Des groupes ont été trouvés avec : " & NumAssociation.ToString & " Groupe Id :" & Param(0) & " sur le noeud " & NodeTemp.ID)
                                        For i As Integer = 0 To NumAssociation - 1
                                            WriteLog("Association -> Node : " & Association(i).ToString)
                                        Next
                                    Else
                                        WriteLog("ERR :ExecuteCommand, GET_ASSOCIATION -> Association non retournée sur le groupe : " & Param(0))
                                    End If
                                Else
                                    WriteLog("ExecuteCommand, Passage par la commande GET_ASSOCIATION -> nome, Groupe Id :" & Param(0) & " sur le noeud " & NodeTemp.ID)
                                End If
                                Association = Nothing

                            Case "GET_GROUPMAX"
                                Dim NumGroup As Integer
                                Dim Nombre As Integer

                                NodeTemp = GetNode(m_homeId, adr1)
                                NumGroup = m_manager.GetNumGroups(m_homeId, NodeTemp.ID)
                                If (Param(0) >= 1 And Param(0) <= NumGroup) Then
                                    Nombre = m_manager.GetMaxAssociations(m_homeId, NodeTemp.ID, Param(0))
                                    WriteLog("ExecuteCommand, Passage par la commande GET_GROUPMAX -> " & Nombre & ", Groupe Id :" & Param(0) & " sur le noeud " & NodeTemp.ID)
                                Else
                                    WriteLog("ExecuteCommand, Passage par la commande GET_GROUPMAX -> Nome, Groupe Id :" & Param(0) & " sur le noeud " & NodeTemp.ID)
                                End If

                            Case "REFRESHNODEINFO"
                                NodeTemp = GetNode(m_homeId, adr1)
                                If m_manager.RefreshNodeInfo(m_homeId, adr1) Then
                                    WriteLog("ExecuteCommand, Passage par la commande REFRESHNODEINFO -> sur le noeud " & NodeTemp.ID)
                                Else
                                    WriteLog("ExecuteCommand, Passage par la commande REFRESHNODEINFO -> sur le noeud " & NodeTemp.ID & " Impossible")
                                End If

                            Case "GET_NODE_SECURITE"
                                Dim Securite As Byte = 0

                                NodeTemp = GetNode(m_homeId, adr1)
                                If m_manager.IsNodeSecurityDevice(m_homeId, adr1) Then
                                    WriteLog("ExecuteCommand, Passage par la commande GET_NODE_SECURITE -> le noeud " & NodeTemp.ID & " prend en charge la sécurité")
                                Else
                                    WriteLog("ExecuteCommand, Passage par la commande GET_NODE_SECURITE -> noeud " & NodeTemp.ID & " ne prend pas en charge la sécurité")
                                End If
                                Securite = m_manager.GetNodeSecurity(m_homeId, NodeTemp.ID)
                                WriteLog("ExecuteCommand, Passage par la commande GET_NODE_SECURITE -> Code : " & Securite & " sur le noeud " & NodeTemp.ID)

                            Case "IS_NODE_AWAKE"
                                NodeTemp = GetNode(m_homeId, adr1)
                                If m_manager.IsNodeAwake(m_homeId, NodeTemp.ID) Then
                                    WriteLog("ExecuteCommand, Passage par la commande IS_NODE_AWAKE -> le noeud " & NodeTemp.ID & " est éveillé")
                                Else
                                    WriteLog("ExecuteCommand, Passage par la commande IS_NODE_AWAKE -> le noeud " & NodeTemp.ID & " est endormi")
                                End If

                            Case "PRESSBOUTON"
                                Dim RetourSet As Boolean
                                Dim ValueTemp As ZWValueID = Nothing
                                Dim ParaAdr2 = Split(MyDevice.Adresse2, ":")
                                NodeTemp = GetNode(m_homeId, adr1)
                                ValueTemp = GetValeur(NodeTemp, Trim(ParaAdr2(0)), Trim(ParaAdr2(1)))
                                '                                If ValueTemp.GetType() = 5 Then        ' Uniquement Type Button
                                If ValueTemp.GetType() = ZWValueID.ValueType.Button Then   ' Uniquement Type Button
                                    If Param(0) = 1 Then
                                        RetourSet = m_manager.PressButton(ValueTemp)
                                    ElseIf Param(0) = 0 Then
                                        RetourSet = m_manager.ReleaseButton(ValueTemp)
                                    End If
                                    If RetourSet Then
                                        WriteLog(Me.Nom & " ExecuteCommand, Parametre " & Param(0) & " modifié avec succès sur le noeud " & NodeTemp.ID)
                                    Else
                                        WriteLog("ERR: " & Me.Nom & " ExecuteCommand, Parametre " & Param(0) & " erreur de la modification sur le noeud " & NodeTemp.ID)
                                    End If
                                Else
                                    WriteLog("ERR: " & Me.Nom & " ExecuteCommand, Parametre " & Param(0) & " erreur : Uniquement pour type Button " & NodeTemp.ID)
                                End If
                                WriteLog("DBG: " & Me.Nom & " ExecuteCommand, Passage par la commande PressBouton ")

                            Case "SETLIST"
                                Dim RetourSet As Boolean
                                Dim ValueTemp As ZWValueID = Nothing
                                Dim ParaAdr2 = Split(MyDevice.Adresse2, ":")
                                NodeTemp = GetNode(m_homeId, adr1)
                                ValueTemp = GetValeur(NodeTemp, Trim(ParaAdr2(0)), Trim(ParaAdr2(1)))
                                ' If ValueTemp.GetType() = 4 Then        ' Uniquement Type list
                                If ValueTemp.GetType() = ZWValueID.ValueType.List Then   ' Uniquement Type list
                                    RetourSet = m_manager.SetValueListSelection(ValueTemp, Param(0))
                                    If RetourSet Then
                                        WriteLog("ExecuteCommand, Parametre " & Param(0) & " modifié avec succès sur le noeud " & NodeTemp.ID)
                                    Else
                                        WriteLog("ERR: ExecuteCommand, Parametre " & Param(0) & " erreur de la modification sur le noeud " & NodeTemp.ID)
                                    End If
                                Else
                                    WriteLog("ERR: ExecuteCommand,Parametre " & Param(0) & " erreur : Uniquement pour type LIST " & NodeTemp.ID)
                                End If
                                WriteLog("DBG: " & Me.Nom & " ExecuteCommand, Passage par la commande SetList ")

                            Case "SETPOINT"
                                Write(MyDevice, "SETPOINT", Param(0))

                            Case "RED"
                                Write(MyDevice, "RED", Param(0))
                            Case "GREEN"
                                Write(MyDevice, "GREEN", Param(0))
                            Case "BLUE"
                                Write(MyDevice, "BLUE", Param(0))

                                WriteLog("DBG: " & Me.Nom & " ExecuteCommand, commande " & UCase(Command) & "passée")

                            Case Else
                                WriteLog("ERR: " & "ExecuteCommand, La commande " & UCase(Command) & " n'existe pas")
                        End Select
                        Return True
                    End If
                Else
                    Return False
                End If
            Catch ex As Exception
                WriteLog("ERR: " & "ExecuteCommand, exception : " & ex.Message)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Permet de vérifier si un champ est valide
        ''' </summary>
        ''' <param name="Champ"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
            Try
                Dim retour As String = "0"

                Select Case UCase(Champ)
                    Case "ADRESSE1"
                        'verification que numero de noeud existe
                        If _IsConnect Then
                            'permet de gerer la compatibilité du driver avec listbox
                            Dim adr1 As String = ""
                            If InStr(Value, "#") <> 0 Then
                                adr1 = Trim(Left(Value, InStr(Value, "#") - 1))
                            Else
                                adr1 = Value
                            End If
                            Try
                                Dim ValExist As Boolean = False
                                Dim i As Integer
                                For i = 0 To m_nodeList.Count - 1
                                    If m_nodeList.ElementAt(i).ID = adr1 Then
                                        ValExist = True
                                        Exit For
                                    End If
                                Next
                                If Not ValExist Then Return "Le noeud " & Value & " n'existe pas" & vbCrLf & "Vérifiez votre configuration"
                            Catch ex As Exception
                                Return ("ERR: VerifChamp, Probleme lors de la recherche de la liste des noeuds, Noeud " & " => " & adr1)
                            End Try
                        End If
                    Case "ADRESSE2"
                        ' Suppression des espaces inutiles
                        If InStr(Value, ":") Then
                            Dim ParaAdr2 = Split(Value, ":")
                            Value = Trim(ParaAdr2(0)) & ":" & Trim(ParaAdr2(1))
                        End If

                End Select
                Return retour
            Catch ex As Exception
                Return "ERR: VerifChamp, Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
            End Try
        End Function

        ''' <summary>Démarrer le driver</summary>
        ''' <remarks></remarks>
        Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
            Dim retour As String = ""
            Dim CptBoucle As Byte

            'récupération des paramétres avancés
            Try
                _DEBUG = Parametres.Item(0).Valeur
                _AFFICHELOG = Parametres.Item(1).Valeur
                _STARTIDLETIME = Parametres.Item(2).Valeur

                'parametrage port serie
                Dim valuetmp As String
                valuetmp = Parametres.Item(3).Valeur
                Select Case Right(valuetmp, 1)
                    Case "0"
                        _nbrebitstop = IO.Ports.StopBits.None
                    Case "1"
                        _nbrebitstop = IO.Ports.StopBits.One
                    Case "2"
                        _nbrebitstop = IO.Ports.StopBits.Two
                End Select
                valuetmp = Left(valuetmp, Len(valuetmp) - 1)
                Select Case Right(valuetmp, 1)
                    Case "N"
                        _parity = IO.Ports.Parity.None
                    Case "E"
                        _parity = IO.Ports.Parity.Even
                    Case "O"
                        _parity = IO.Ports.Parity.Odd
                    Case "M"
                        _parity = IO.Ports.Parity.Mark
                    Case "S"
                        _parity = IO.Ports.Parity.Space
                End Select
                valuetmp = Left(valuetmp, Len(valuetmp) - 1)
                _nbrebit = Right(valuetmp, 1)
                _baudspeed = Left(valuetmp, Len(valuetmp) - 1)

                _networkkey = Parametres.Item(4).Valeur
                _Getconfig = Parametres.Item(5).Valeur
            Catch ex As Exception
                _DEBUG = False
                _AFFICHELOG = False
                _STARTIDLETIME = 10
                WriteLog("ERR: " & "Erreur dans les paramétres avancés. utilisation des valeurs par défaut " & ex.Message)
            End Try

            'ouverture du port suivant le Port Com
            Try
                If _Com <> "" Then
                    WriteLog("Demarrage du pilote, ceci peut prendre plusieurs secondes")
                    retour = ouvrir(_Com)
                    ' ZWave network is started, and our control of hardware can begin once all the nodes have reported in
                    While (m_nodesReady = False And CptBoucle < 10)
                        System.Threading.Thread.Sleep(3000) ' Attente de 3 sec  - Info OpenZWave Wait since this process can take around 20seconds within some network
                        CptBoucle = CptBoucle + 1
                    End While

                    'traitement du message de retour
                    If Not (g_initFailed) Or (m_homeId = 0) Then ' le demarrage le controleur a échoué
                        _IsConnect = False
                        retour = "ERR: Port Com non défini. Impossible d'ouvrir le port !"
                        WriteLog("ERR: " & "Driver non démarré : " & retour)
                    Else
                        _IsConnect = True
                        dateheurelancement = DateTime.Now
                        WriteLog("Port " & _Com & " ouvert ")

                        ' Affichage des Informations Controleur du controleur
                        Dim NodeControler As Byte = m_manager.GetControllerNodeId(m_homeId)
                        Dim IsPrimSUC As String = ""
                        If m_manager.IsPrimaryController(m_homeId) Then IsPrimSUC = "Primaire" Else IsPrimSUC = "Secondaire"
                        If m_manager.IsStaticUpdateController(m_homeId) Then IsPrimSUC = IsPrimSUC & "(SUC)" Else IsPrimSUC = IsPrimSUC & " (SIS)"
                        WriteLog("    * Home ID : 0x" & Convert.ToString(m_homeId, 16).ToUpper & " Nombre de noeuds : " & m_nodeList.Count.ToString)
                        WriteLog("    * Mode    : " & IsPrimSUC)
                        WriteLog("    * ID  Controleur  : " & NodeControler & "   Nom Controleur :" & m_manager.GetNodeName(m_homeId, NodeControler))
                        WriteLog("    * Marque/modele   : " & m_manager.GetNodeManufacturerName(m_homeId, NodeControler) & m_manager.GetNodeProductName(m_homeId, NodeControler))
                        WriteLog("    * Type Controleur : " & m_manager.GetLibraryTypeName(m_homeId) & " Biblio Version : " & m_manager.GetLibraryVersion(m_homeId))
                        ' Si le controleur a des noeuds d'associés
                        If m_nodeList.Count Then
                            WriteLog("* Noeuds: ")
                            WriteLog(String.Format("{0,-15}{1,-70}{2,30}", "Id:Nom", Space(35 - Len("Constr./Modele   Version") / 2) & "Constr./Modele   Version" & Space(35 - Len("Constr./Modele   Version") / 2), String.Format("{0,40}", "Endormi ?")))
                            ' Affichages des informations de chaque noeud 
                            Dim IsSleeping As String = ""
                            Dim NodeTempID As Byte
                            For Each NodeTemp As Node In m_nodeList
                                NodeTempID = NodeTemp.ID
                                If m_manager.IsNodeListeningDevice(m_homeId, NodeTempID) Then IsSleeping = "à l'écoute" Else IsSleeping = "Endormi"
                                WriteLog(String.Format("{0,-15}{1,-70}{2,30}", NodeTempID & ":" & NodeTemp.Name, NodeTemp.Manufacturer & "/" & NodeTemp.Product & "     V." & m_manager.GetNodeVersion(m_homeId, NodeTemp.ID), IsSleeping))
                            Next
                        End If

                        _NomFileConfigZWave = Convert.ToString(m_homeId, 16).ToString
                        'rajoute des 0 si nécessaire devant l'id du controleur pour avoir une chaine de 8 de long
                        _NomFileConfigZWave = _NomFileConfigZWave.PadLeft(8, "0")
                        _NomFileConfigZWave = My.Application.Info.DirectoryPath & "\drivers\zwave\zwcfg_0x" & _NomFileConfigZWave & ".xml"
                        WriteLog("DBG: Start,  _NomFileConfigZWave " & _NomFileConfigZWave)

                        ' Sauvegarde de la configuration 
                        WriteLog("Start,  Sauvegarde de la config Zwave")
                        If My.Computer.FileSystem.FileExists(_NomFileConfigZWave) Then
                            Dim DateModif As String = FileDateTime(_NomFileConfigZWave)
                            m_manager.WriteConfig(m_homeId)
                            'boucle tant la date du fichier n'est pas modifiée
                            While DateModif = FileDateTime(_NomFileConfigZWave)
                            End While
                        Else
                            m_manager.WriteConfig(m_homeId)
                        End If
                        If _Getconfig Then
                            Get_Config(_NomFileConfigZWave)
                            'recharge la config toutes les 10 mn
                            MyTimer.Interval = 600 * 1000
                            MyTimer.Enabled = True
                            AddHandler MyTimer.Elapsed, AddressOf TimerTick
                        End If

                        End If
                Else
                        retour = "ERR: Port Com non défini. Impossible d'ouvrir le port !"
                End If
                If (_STARTIDLETIME > 0) Then
                    WriteLog("Les messages ne seront pas traité pendant " & _STARTIDLETIME & " secondes.")
                End If
            Catch ex As Exception
                WriteLog("ERR: " & "Start, " & ex.Message)
                _IsConnect = False
            End Try
        End Sub

        ''' <summary>Arrêter le driver</summary>
        ''' <remarks></remarks>
        Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
            Dim retour As String
            Try
                If _IsConnect Then
                    retour = fermer()
                    WriteLog("Stop, " & retour)
                Else
                    WriteLog("Stop, Port " & _Com & " est déjà fermé")
                End If
                MyTimer.Enabled = False

            Catch ex As Exception
                WriteLog("ERR: " & "Stop, " & ex.Message)
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

            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _AFFICHELOG = Parametres.Item(1).Valeur

                If _Enable = False Then
                    WriteLog("ERR: " & "Read, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Not Enable)")
                    Exit Sub
                End If

                If _IsConnect = False Then
                    WriteLog("ERR: " & "Read, Erreur: Impossible de traiter la commande car le driver n'est pas connecté")
                    Exit Sub
                End If

                If Not IsNothing(Objet) Then
                    Dim NodeTemp As Node
                    'permet de gerer la compatibilité du driver avec listbox
                    Dim adr1 As String = ""
                    If InStr(Objet.Adresse1, "#") > 0 Then
                        adr1 = Trim(Left(Objet.Adresse1, InStr(Objet.Adresse1, "#") - 1))
                    Else
                        adr1 = Objet.Adresse1
                    End If
                    NodeTemp = GetNode(m_homeId, adr1)
                    m_manager.RequestNodeState(m_homeId, NodeTemp.ID)
                Else
                    WriteLog("ERR: " & "Read, Erreur: Impossible de traiter la commande car l'objet " & Objet.Adresse1 & " est vide")
                End If


            Catch ex As Exception
                WriteLog("ERR: " & "Read, " & ex.Message)
            End Try
        End Sub

        ''' <summary>Commander un device</summary>
        ''' <param name="Objet">Objet représetant le device à interroger</param>
        ''' <param name="Commande">La commande à passer</param>
        ''' <param name="Parametre1"></param>
        ''' <param name="Parametre2"></param>
        ''' <remarks></remarks>
        Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write

            Try
                Dim NodeTemp As New Node
                Dim texteCommande As String
                Dim ValueTemp As ZWValueID = Nothing
                Dim TempVersion As Byte = 0
                Dim IsMultiLevel As Boolean = False
                _DEBUG = _Parametres.Item(0).Valeur
                _AFFICHELOG = Parametres.Item(1).Valeur

                If _Enable = False Then
                    WriteLog("ERR: " & "Write, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                    Exit Sub
                End If

                If _IsConnect = False Then
                    WriteLog("ERR: " & "Write, Erreur: Impossible de traiter la commande car le driver n'est pas connecté")
                    Exit Sub
                End If

                'permet de gerer la compatibilité du driver avec listbox
                Dim adr1 As String = ""
                If InStr(Objet.Adresse1, "#") > 0 Then
                    adr1 = Trim(Left(Objet.Adresse1, InStr(Objet.Adresse1, "#") - 1))
                Else
                    adr1 = Objet.Adresse1
                End If

                NodeTemp = GetNode(m_homeId, adr1)
                WriteLog("DBG: " & "Write, Commande recue :" & Commande & " sur le noeud " & adr1 & " de type " & Objet.Adresse2)

                If IsNothing(NodeTemp) Then
                    WriteLog("ERR: " & "Write, Noeud non trouvé avec l'adresse : " & adr1)
                Else
                    WriteLog("DBG: " & "Write, Noeud  trouvé avec l'adresse : " & adr1)
                    If NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_MULTILEVEL) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_MULTILEVEL_V2) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_MULTILEVEL_V3) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_MULTILEVEL_V4) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_BINARY) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_BINARY_V2) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_DOOR_LOCK) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_DOOR_LOCK_V2) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_DOOR_LOCK_V3) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_WAKE_UP) Or
                        NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_WAKE_UP_V2) Then
                        WriteLog("DBG: " & "Write, Recherche de : dans l'adresse2 " & Objet.Adresse2)
                        If InStr(Objet.Adresse2, ":") Then
                            Dim ParaAdr2 = Split(Objet.Adresse2, ":")
                            ValueTemp = GetValeur(NodeTemp, Trim(ParaAdr2(0)), Trim(ParaAdr2(1)))
                            If IsNothing(ValueTemp) Then
                                WriteLog("ERR: " & "Write, Valeur non trouvée avec l'adresse : " & Objet.Adresse1 & " et " & Objet.Adresse2)
                                Exit Sub
                            Else
                                WriteLog("DBG: " & "Write, Valeur trouvée avec l'adresse : " & adr1 & " et " & Objet.Adresse2)
                                IsMultiLevel = True
                            End If
                        Else
                            WriteLog("ERR: " & "Write, Erreur dans la definition du label et de l'instance")
                        End If
                    End If

                    If IsMultiLevel Then
                        WriteLog("DBG: " & "Write, Composant Multilevel")
                    Else
                        WriteLog("DBG: " & "Write, Composant Classique")
                    End If
                    WriteLog("Write, Commande à exécuter " & Commande & " sur le noeud " & adr1 & " de type " & Objet.Adresse2 & " avec param1 -> " & Parametre1 & " / param2 -> " & Parametre2)

                    Select Case True
                        '                        Case (Objet.Type = "LAMPE" Or Objet.Type = "LAMPERGBW" Or Objet.Type = "APPAREIL" Or Objet.Type = "SWITCH")
                        Case (Objet.Type = "LAMPE" Or Objet.Type = "LAMPERGBW" Or Objet.Type = "APPAREIL" Or Objet.Type = "SWITCH" Or Objet.Type = "VOLET")
                            texteCommande = UCase(Commande)

                            Select Case True
                                Case UCase(Commande) = "ON"
                                    If IsMultiLevel Then
                                        If InStr(Objet.Adresse2, "Level:") Then
                                            Dim OnValue As Byte = Objet.ValueMax - 1   ' n'accepte que 99 max
                                            m_manager.SetValue(ValueTemp, OnValue)
                                        Else
                                            If NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_BINARY) Or
                                                NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_BINARY_V2) Or
                                                NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_DOOR_LOCK) Or
                                                NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_DOOR_LOCK_V2) Or
                                                NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_DOOR_LOCK_V3) Then
                                                m_manager.SetValue(ValueTemp, True)
                                            End If
                                        End If
                                    Else
                                        m_manager.SetNodeOn(m_homeId, NodeTemp.ID)
                                    End If

                                Case UCase(Commande) = "OFF"
                                    If IsMultiLevel Then
                                        If InStr(Objet.Adresse2, "Level:") Then
                                            Dim OffValue As Byte = Objet.ValueMin
                                            m_manager.SetValue(ValueTemp, OffValue)
                                        Else
                                            If NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_BINARY) Or
                                                NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_SWITCH_BINARY_V2) Or
                                                NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_DOOR_LOCK) Or
                                                NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_DOOR_LOCK_V2) Or
                                                NodeTemp.CommandClass.Contains(CommandClass.COMMAND_CLASS_DOOR_LOCK_V3) Then
                                                m_manager.SetValue(ValueTemp, False)
                                            End If
                                        End If
                                    Else
                                        m_manager.SetNodeOff(m_homeId, NodeTemp.ID)
                                    End If

                                Case UCase(Commande) = "DIM" Or UCase(Commande) = "OUVERTURE"
                                    If Not (IsNothing(Parametre1)) Then
                                        ' Dim ValDimmer As Byte = Math.Round(Parametre1 * 2.55) ' Reformate la valeur entre 0 : OFF  et 255 :ON 
                                        Dim ValDimmer As Byte = Parametre1
                                        texteCommande = texteCommande & " avec le % = " & Val(Parametre1) & " - " & ValDimmer
                                        If IsMultiLevel Then
                                            m_manager.SetValue(ValueTemp, ValDimmer)
                                        Else
                                            m_manager.SetNodeLevel(m_homeId, NodeTemp.ID, ValDimmer)
                                        End If
                                    End If

                                    '                                Case (UCase(Commande) = "SETNEWVAL" Or UCase(Commande) = "RED" Or UCase(Commande) = "GREEN" Or UCase(Commande) = "BLUE")  'Ecrire une valeur vers le device physique
                                Case (UCase(Commande) = "SETPOINT" Or UCase(Commande) = "RED" Or UCase(Commande) = "GREEN" Or UCase(Commande) = "BLUE")  'Ecrire une valeur vers le device physique
                                    If Not IsNothing(Parametre1) Then
                                        Dim ValDimmer As Single
                                        If IsNumeric(Parametre1) Then ValDimmer = Parametre1
                                        Select Case True  ' gestion des modes de chauffage
                                            Case (UCase(Parametre1) = "CONFORT" Or UCase(Parametre1) = "CONF")
                                                ValDimmer = 95
                                            Case (UCase(Parametre1) = "CONFORT-1" Or UCase(Parametre1) = "CONF-1")
                                                ValDimmer = 45
                                            Case (UCase(Parametre1) = "CONFORT-2" Or UCase(Parametre1) = "CONF-2")
                                                ValDimmer = 35
                                            Case (UCase(Parametre1) = "ECO" Or UCase(Parametre1) = "EC")
                                                ValDimmer = 25
                                            Case (UCase(Parametre1) = "HORSGEL" Or UCase(Parametre1) = "HG")
                                                ValDimmer = 15
                                            Case UCase(Parametre1) = "ARRET"
                                                ValDimmer = 5
                                        End Select

                                        Select Case True
                                            Case (InStr(Objet.Adresse2, "Color:") > 0 And InStr(Parametre1, "#") = 0)  'code couleur RGB
                                                '                                                ValDimmer = Parametre1
                                                Dim HexR, HexB, HexG As String
                                                If UCase(Commande) = "RED" Then
                                                    HexR = Hex(ValDimmer)
                                                Else
                                                    HexR = Hex(Objet.red)
                                                End If
                                                If Len(HexR) < 2 Then HexR = "0" & HexR
                                                If UCase(Commande) = "GREEN" Then
                                                    HexG = Hex(ValDimmer)
                                                Else
                                                    HexG = Hex(Objet.green)
                                                End If
                                                If Len(HexG) < 2 Then HexG = "0" & HexG
                                                If UCase(Commande) = "BLUE" Then
                                                    HexB = Hex(ValDimmer)
                                                Else
                                                    HexB = Hex(Objet.blue)
                                                End If
                                                If Len(HexB) < 2 Then HexB = "0" & HexB
                                                Dim color_str As String = "#" & HexR & HexG & HexB
                                                m_manager.SetValue(ValueTemp, color_str)
                                            Case (InStr(Objet.Adresse2, "Color:") > 0 And InStr(Parametre1, "#") > 0)  'code couleur hexadecimal
                                                m_manager.SetValue(ValueTemp, Parametre1)
                                            Case InStr(Objet.Adresse2, "Level:")  'puissance de la lampe, mode chauffage
                                                '                                               ValDimmer = Parametre1
                                                If IsMultiLevel Then
                                                    m_manager.SetValue(ValueTemp, CByte(ValDimmer))
                                                Else
                                                    m_manager.SetNodeLevel(m_homeId, NodeTemp.ID, CByte(ValDimmer))
                                                End If
                                            Case Else
                                                '                                                ValDimmer = Parametre1
                                                m_manager.SetNodeLevel(m_homeId, NodeTemp.ID, CByte(ValDimmer))
                                        End Select
                                    End If
                            End Select
                        Case (Objet.Type = "TEMPERATURECONSIGNE" Or Objet.Type = "GENERIQUEVALUE")    '    Or Objet.Type = "TEMPERATURE") Then
                            texteCommande = UCase(Commande)

                            Select Case UCase(Commande)
                                Case "SETPOINT"     'Ecrire une valeur vers le device physique
                                    If Not (IsNothing(Parametre1)) Then
                                        Dim ValDimmer As Single
                                        If IsNumeric(Parametre1) Then ValDimmer = Parametre1
                                        Select Case True  ' gestion des modes de chauffage
                                            Case (UCase(Parametre1) = "CONFORT" Or UCase(Parametre1) = "CONF")
                                                ValDimmer = 95
                                            Case (UCase(Parametre1) = "CONFORT-1" Or UCase(Parametre1) = "CONF-1")
                                                ValDimmer = 45
                                            Case (UCase(Parametre1) = "CONFORT-2" Or UCase(Parametre1) = "CONF-2")
                                                ValDimmer = 35
                                            Case (UCase(Parametre1) = "ECO" Or UCase(Parametre1) = "EC")
                                                ValDimmer = 25
                                            Case (UCase(Parametre1) = "HORSGEL" Or UCase(Parametre1) = "HG")
                                                ValDimmer = 15
                                            Case UCase(Parametre1) = "ARRET"
                                                ValDimmer = 5
                                        End Select
                                        If InStr(Objet.Adresse2, "Wake-up Interval:") > 0 Then
                                            If ValDimmer < 60 Then    'Pour Wake Up Interval
                                                ValDimmer = 60
                                            ElseIf ValDimmer > 86400 Then
                                                ValDimmer = 86400
                                            End If
                                        End If
                                        texteCommande = texteCommande & " avec valeur = " & ValDimmer

                                        If IsMultiLevel Then
                                            Select Case True
                                                Case InStr(Objet.Adresse2, "Wake-up Interval:") > 0
                                                    m_manager.SetValue(ValueTemp, CInt(ValDimmer))
                                                Case InStr(Objet.Adresse2, "Heating 1:") > 0
                                                    m_manager.SetValue(ValueTemp, ValDimmer)
                                                Case InStr(Objet.Adresse2, "Basic:") > 0
                                                    m_manager.SetValue(ValueTemp, CByte(ValDimmer))
                                                Case InStr(Objet.Adresse2, "Cooling 1:") > 0
                                                    m_manager.SetValue(ValueTemp, CInt(ValDimmer))
                                                Case InStr(Objet.Adresse2, "Dry Air:") > 0
                                                    m_manager.SetValue(ValueTemp, CInt(ValDimmer))
                                                Case InStr(Objet.Adresse2, "Auto Changeover:") > 0
                                                Case Else
                                                    m_manager.SetValue(ValueTemp, ValDimmer)
                                            End Select
                                        End If
                                    End If
                            End Select
                        Case Else
                            WriteLog("ERR: " & "Write, Erreur: Le type " & Objet.Type.ToString & " à l'adresse " & Objet.Adresse1 & " n'est pas compatible")
                    End Select
                End If
            Catch ex As Exception
                WriteLog("ERR: " & "Write, Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
        ''' <param name="DeviceId">Objet représetant le device à interroger</param>
        ''' <remarks></remarks>
        Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
            Try

            Catch ex As Exception
                WriteLog("ERR: " & "DeleteDevice, Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
        ''' <param name="DeviceId">Objet représetant le device à interroger</param>
        ''' <remarks></remarks>
        Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
            Try

            Catch ex As Exception
                WriteLog("ERR: " & "NewDevice, Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>ajout des commandes avancées pour les devices</summary>
        ''' <param name="nom">Nom de la commande avancée</param>
        ''' <param name="description">Description qui sera affichée dans l'admin</param>
        ''' <param name="nbparam">Nombre de parametres attendus</param>
        ''' <remarks></remarks>
        Private Sub Add_DeviceCommande(ByVal Nom As String, ByVal Description As String, ByVal NbParam As Integer)
            Try
                Dim x As New DeviceCommande
                x.NameCommand = Nom
                x.DescriptionCommand = Description
                x.CountParam = NbParam
                _DeviceCommandPlus.Add(x)
            Catch ex As Exception
                WriteLog("ERR: " & "Add_DeviceCommande, Exception : " & ex.Message)
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
                WriteLog("ERR: " & "Add_LibelleDriver, Exception : " & ex.Message)
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
                WriteLog("ERR: " & "Add_LibelleDevice, Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>ajout de parametre avancés</summary>
        ''' <param name="nom">Nom du parametre (sans espace)</param>
        ''' <param name="description">Description du parametre</param>
        ''' <param name="valeur">Sa valeur</param>
        ''' <remarks></remarks>
        Private Sub Add_ParamAvance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
            Try
                Dim x As New HoMIDom.HoMIDom.Driver.Parametre
                x.Nom = nom
                x.Description = description
                x.Valeur = valeur
                _Parametres.Add(x)
            Catch ex As Exception
                WriteLog("ERR: " & "Add_ParamAvance, Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Creation d'un objet de type</summary>
        ''' <remarks></remarks>
        Public Sub New()
            Try

                'liste des devices compatibles
                _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
                _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
                _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)
                _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
                _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
                _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
                _DeviceSupport.Add(ListeDevices.ENERGIETOTALE.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
                _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
                _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
                _DeviceSupport.Add(ListeDevices.LAMPERGBW.ToString)
                _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
                _DeviceSupport.Add(ListeDevices.TELECOMMANDE.ToString)
                _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
                _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)
                _DeviceSupport.Add(ListeDevices.VOLET.ToString)

                'Paramétres avancés
                Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
                Add_ParamAvance("AfficheLog", "Afficher Log OpenZwave à l'écran (True/False)", True)
                Add_ParamAvance("StartIdleTime", "Durée durant laquelle le driver ne traite aucun message lors de son démarrage (en secondes).", 10)
                Add_ParamAvance("BaudRate", "Vitesse,Nbre bits, Parité, Nbre bit stop ( défaut 96008N1 )", "96008N1")
                Add_ParamAvance("NetworkKey", "Clef pour réseau sécurisé", "0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F,0x10")
                Add_ParamAvance("GetConfig", "Permet d'avoir la configuration du réseau ZWave au paramétrage (True/False)", True)
                Add_ParamAvance("ActiveSauvegarde", "Active la sauvegarde automatique de la configuration du réseau ZWave (True/False)", True)
                Add_ParamAvance("AfficheSysteme", "Affiche Eléments Systèmes du résau (True/False)", False)

                'ajout des commandes avancées pour les devices
                Add_DeviceCommande("SetPoint", "Nouvelle consigne", 1)
                Add_DeviceCommande("ALL_LIGHT_ON", "", 0)
                Add_DeviceCommande("ALL_LIGHT_OFF", "", 0)
                Add_DeviceCommande("SetName", "Nom du composant", 0)
                Add_DeviceCommande("SetList", "Valeur de la liste : ", 1)
                Add_DeviceCommande("PressBouton", "Valeur : Byte, Integer, Decimal : ", 1)
                Add_DeviceCommande("GetName", "Nom du composant", 0)
                Add_DeviceCommande("SetConfigParam", "paramètre de configuration - Par1 : Index - Par2 : Valeur", 2)
                Add_DeviceCommande("GetConfigParam", "paramètre de configuration - Par1 : Index", 1)
                Add_DeviceCommande("RequestNodeState", "Nom du composant", 0)
                Add_DeviceCommande("RequestNodeDynamic", "Nom du composant", 0)
                Add_DeviceCommande("TestNetworkNode", "Nom du composant", 0)
                Add_DeviceCommande("RequestNetworkUpdate", "Nom du composant", 0)
                Add_DeviceCommande("RequestNodeNeighborUpdate", "Nom du composant", 0)
                Add_DeviceCommande("GetNumGroups", "Nom du composant", 0)
                Add_DeviceCommande("Add_Association", "Parametres de configuration - Par1 : Groupe Idx - Par2 : Cible Noeud Id", 2)
                Add_DeviceCommande("Remove_Association", "Parametres de configuration - Par1 : Groupe Idx - Par2 : Cible Noeud Id", 2)
                Add_DeviceCommande("Get_Association", "Parametres de configuration - Par1 : Groupe Idx", 1)
                Add_DeviceCommande("Get_GroupMax", "Parametres de configuration - Par1 : Groupe Idx", 1)
                Add_DeviceCommande("Get_Node_Securite", "Lecture code Sécurité", 0)
                Add_DeviceCommande("RefreshNodeInfo", "Lance le rafraichissement du noeud", 0)
                Add_DeviceCommande("Is_Node_Awake", "Vérifie si le noeud est éveillé", 0)
                Add_DeviceCommande("blue", "Imperihome", 1) 'compatibilité imperihome et lampe rgbw
                Add_DeviceCommande("red", "Imperihome", 1) 'compatibilité imperihome et lampe rgbw
                Add_DeviceCommande("green", "imperihome", 1) 'compatibilité imperihome et lampe rgbw

                'Libellé Driver
                Add_LibelleDriver("HELP", "Aide...", "Ce module permet de recuperer les informations delivrées par un contrôleur Z-Wave ")

                'Libellé Device
                Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant de Z-Wave")
                Add_LibelleDevice("ADRESSE2", "Label de la donnée:Index", "'Temperature', 'Relative Humidity', 'Battery Level' suivi de l'index (si necessaire)")
                Add_LibelleDevice("SOLO", "@", "")
                Add_LibelleDevice("MODELE", "@", "")

            Catch ex As Exception
                WriteLog("ERR: " & "New - " & ex.Message)
            End Try
        End Sub

        ''' <summary>Si refresh >0 gestion du timer</summary>
        ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
        Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
            Try
                'recharge la config
                If _Getconfig Then
                    If Parametres.Item(6).Valeur = True Then    'Si sauvegarde activÃ
                    Dim DateModif As String = FileDateTime(_NomFileConfigZWave)
                    WriteLog("Timer,  Sauvegarde de la config Zwave")
                    m_manager.WriteConfig(m_homeId)
                    'boucle tant la date du fichier n'est pas modifiée
                    While DateModif = FileDateTime(_NomFileConfigZWave)
                    End While
                    End If
                    Get_Config(_NomFileConfigZWave)
                End If
            Catch ex As Exception
                WriteLog("ERR: " & "TimerTick - " & ex.Message)
            End Try
        End Sub
#End Region

#Region "Fonctions internes"

        '-----------------------------------------------------------------------------
        ''' <summary>Ouvrir le port Z-Wave</summary>
        ''' <param name="numero">Nom/Numero du port COM: COM2</param>
        ''' <remarks></remarks>
        Private Function ouvrir(ByVal numero As String) As String
            Try
                'ouverture du port
                If Not _IsConnect Then
                    Try
                        ' Test d'ouveture du port Com du controleur 
                        port.PortName = numero
                        port.BaudRate = _baudspeed
                        port.DataBits = _nbrebit
                        port.Parity = _parity
                        port.StopBits = _nbrebitstop
                        WriteLog("Ouvrir - Ouverture du port " & port.PortName & " à la vitesse " & port.BaudRate & port.DataBits & port.Parity.ToString & port.StopBits.ToString)
                        port.Open()
                        ' Le port existe ==> le controleur est present
                        If port.IsOpen() Then
                            port.Close()

                            ' Creation du controleur ZWave
                            m_options = New ZWOptions()
                            m_manager = New ZWManager()

                            m_options.Create(System.IO.Path.Combine(MyRep, "Zwave\"), System.IO.Path.Combine(MyRep, "Zwave\"), "")  ' repertoire de sauvegarde de la log et config 
                            WriteLog("Ouvrir - Le nom du repertoire de config est : " & System.IO.Path.Combine(MyRep, "Zwave\"))

                            If _DEBUG Then
                                m_options.AddOptionInt("SaveLogLevel", LogLevel.LogLevel_Internal)      ' Configure le niveau de sauvegarde des messages (Disque)
                                m_options.AddOptionInt("QueueLogLevel", LogLevel.LogLevel_Internal)    ' Configure le niveau de  sauvegarde des messages (RAM)
                                m_options.AddOptionInt("DumpTrigger", LogLevel.LogLevel_Internal)
                            Else
                                m_options.AddOptionInt("SaveLogLevel", OpenZWaveDotNet.ZWLogLevel.Info)      ' Configure le niveau de sauvegarde des messages (Disque)
                                m_options.AddOptionInt("QueueLogLevel", OpenZWaveDotNet.ZWLogLevel.Warning)     ' Configure le niveau de  sauvegarde des messages (RAM)
                                m_options.AddOptionInt("DumpTrigger", OpenZWaveDotNet.ZWLogLevel.Info)       ' Internal niveau de debug
                            End If

                            m_options.AddOptionBool("ConsoleOutput", _AFFICHELOG)
                            m_options.AddOptionBool("AppendLogFile", False)                      ' Remplace le fichier de log (pas d'append)   
                            m_options.AddOptionInt("PollInterval", 500)
                            m_options.AddOptionBool("IntervalBetweenPolls", True)
                            m_options.AddOptionBool("ValidateValueChanges", True)

                            m_options.AddOptionString("NetworkKey", _networkkey, False)
                            m_options.AddOptionBool("AssumeAwake", True)
                            m_options.AddOptionBool("SuppressValueRefresh", True)
                            m_options.AddOptionBool("PerformReturnRoutes", True)
                            m_options.AddOptionBool("SaveConfiguration", True)

                            m_options.Lock()
                            m_manager.Create()

                            ' Ajout d'un gestionnaire d'evenements()
                            m_manager.OnControllerStateChanged = New ManagedControllerStateChangedHandler(AddressOf ManagedControllerStateChangedHandler)
                            m_manager.OnNotification = New ManagedNotificationsHandler(AddressOf NotificationHandler)
                            ' Creation du driver - Ouverture du port du controleur
                            g_initFailed = m_manager.AddDriver("\\.\" & numero) ' Return True if driver create
                            Return ("Port " & port_name & " ouvert")
                        Else
                            ' Le port n'existe pas ==> le controleur n'est pas present
                            Return ("Port " & port_name & " fermé")
                        End If
                    Catch ex As Exception
                        Return ("Port " & port_name & " n'existe pas")
                        Exit Function
                    End Try
                Else
                    Return ("Port " & port_name & " dejà ouvert")
                End If
            Catch ex As Exception
                Return ("ERR: " & ex.Message)
            End Try
        End Function

        ''' <summary>Fermer le port Z-Wave</summary>
        ''' <remarks></remarks>
        Private Function fermer() As String
            Try
                If _IsConnect Then
                    If (Not (port Is Nothing)) Then ' The COM port exists.
                        ' Sauvegarde de la configuration du réseau
                        m_manager.WriteConfig(m_homeId)
                        WriteLog("Close, sauvegarde de la config Zwave")
                        ' Fermeture du port du controleur 
                        Try
                            WriteLog("Close, nbr de noeud à retirer => " & m_nodeList.Count)
                            Dim node As Node
                            While m_nodeList.Count > 0
                                WriteLog("DBG: " & "Close, noeud " & m_nodeList.ElementAt(m_nodeList.Count - 1).ID & " / " & m_nodeList.ElementAt(m_nodeList.Count - 1).Label & " retiré")
                                node = GetNode(m_homeId, m_nodeList.ElementAt(m_nodeList.Count - 1).ID)
                                m_nodeList.Remove(node)
                            End While
                        Catch ex As UnauthorizedAccessException
                            Return ("ERR: Probleme lors de la suppression des noeuds" & m_nodeList.Count)
                        End Try

                        m_manager.RemoveDriver("\\.\" & _Com)
                        m_manager.Destroy()
                        m_options.Destroy()
                        port.Dispose()
                        port.Close()
                        m_homeId = Nothing
                        m_nodesReady = Nothing
                        _IsConnect = False
                        Return ("Port " & _Com & " fermé")
                    Else
                        _IsConnect = False
                        Return ("Port " & _Com & " n'existe pas")
                    End If
                Else
                    _IsConnect = False
                    Return ("Port " & _Com & "  est déjà fermé (port_ouvert=false)")
                End If
                ' Catch ex As UnauthorizedAccessException
                '     Return ("ERR: Port " & _Com & " IGNORE")
            Catch ex As Exception
                Return ("ERR: Port " & _Com & " IGNORE: " & ex.Message)
            End Try
        End Function


        ''' <summary>Reset le controleur Z-Wave avec les parametres d'Usine, hard reset</summary>
        ''' <remarks></remarks>
        Sub ResetControler()
            Try
                WriteLog("ResetControler - Reset du controleur, ceci va supprimer toute votre configuration")
                m_manager.ResetController(m_homeId)
            Catch ex As Exception
                WriteLog("ERR: " & "ResetControler - Exception: " & ex.Message)
            End Try
        End Sub

        ''' <summary>Reset le controleur Z-Wave sans effacer les parametres de configuration, soft reset </summary>
        ''' <remarks></remarks>
        Sub SoftReset()
            Try
                WriteLog("SoftReset - Reset du controleur, sans effacer les parametres de configuration")
                m_manager.SoftReset(m_homeId)
            Catch ex As Exception
                WriteLog("ERR: " & "SoftReset - Exception: " & ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Sauve la config du réseau zwave
        ''' </summary>
        ''' <remarks></remarks>
        Sub SauverConfigZwave()
            Try
                If _IsConnect Then
                    Dim DateModif As String = FileDateTime(_NomFileConfigZWave)
                    WriteLog("Sauvegarde de la config Zwave")
                    m_manager.WriteConfig(m_homeId)
                    'boucle tant la date du fichier n'est pas modifiée
                    While DateModif = FileDateTime(_NomFileConfigZWave)
                    End While
                    If _Getconfig Then
                        WriteLog("SauverConfigue,  Chargement de la config Zwave")
                        Get_Config(_NomFileConfigZWave)
                    End If
                End If
            Catch ex As Exception
                WriteLog("ERR: " & "SauverConfigueZwave - " & ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Place le controller en mode "inclusion"
        ''' </summary>
        ''' <remarks></remarks>
        Sub CheckStateNode()
            WriteLog("CheckStateNode, Nombre de noeud à analyser : " & m_nodeList.Count)
            Try
                Dim node As Node
                Dim i As Integer

                For i = 0 To m_nodeList.Count - 1
                    node = GetNode(m_homeId, m_nodeList.ElementAt(i).ID)
                    If (m_manager.IsNodeFailed(m_homeId, node.ID)) Then
                        WriteLog("Noeud Mort : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product & " -> " & m_nodeList.ElementAt(i).Label)
                    Else
                        WriteLog("Noeud Actif : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product & " -> " & m_nodeList.ElementAt(i).Label)
                    End If
                Next
            Catch ex As Exception
                WriteLog("ERR: AffichedNode, Probleme lors de la suppression des noeuds")
            End Try
        End Sub

        ''' <summary>
        ''' Place le controller en mode "inclusion"
        ''' </summary>
        ''' <remarks></remarks>
        Sub ReplaceFailNode(Optional ByVal NumNode As Byte = Nothing, Optional ByVal Securite As Boolean = False)
            Try
                Dim node As Node
                Dim i As Integer
                Dim Trouve As Boolean = False

                If _IsConnect Then
                    If NumNode = Nothing Then NumNode = 0
                    If Securite = Nothing Then Securite = False
                    If NumNode > 1 Then
                        For i = 0 To m_nodeList.Count - 1
                            node = GetNode(m_homeId, m_nodeList.ElementAt(i).ID)
                            If NumNode = node.ID Then
                                Trouve = True
                                If m_manager.HasNodeFailed(m_homeId, NumNode) Then
                                    If m_manager.ReplaceFailedNode(m_homeId, node.ID) Then
                                        WriteLog("ReplaceFailedNode réussi sur Noeud : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product & " -> " & m_nodeList.ElementAt(i).Product)
                                        If m_manager.AddNode(m_homeId, Securite) Then
                                            WriteLog("Début de la séquence d'inclusion.")
                                        Else
                                            WriteLog("ERR: Impossible d'exécuter la séquence d'inclusion.")
                                        End If
                                    Else
                                        WriteLog("ERR: ReplaceFailedNode échoué : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product & " -> " & m_nodeList.ElementAt(i).Label)
                                    End If
                                    Exit For
                                Else
                                    WriteLog("ERR: ReplaceFailedNode, impossible sur un Noeud actif  : " & NumNode.ToString)
                                End If
                            End If
                        Next
                        If Trouve = False Then
                            WriteLog("ERR: ReplaceFailedNode, Numéro du noeud incorrect : " & NumNode.ToString)
                        End If
                    Else
                        WriteLog("ReplaceFailedNode, Le numéro de noeud n'est pas correct")
                    End If
                End If
            Catch ex As Exception
                WriteLog("ERR: ReplaceFailedNode, Probleme lors du replacement du noeud")
            End Try
        End Sub

        ''' <summary>
        ''' Place le controller en mode "inclusion"
        ''' </summary>
        ''' <remarks></remarks>
        Sub StartInclusionMode()
            If m_manager.AddNode(m_homeId, False) Then
                WriteLog("Début de la séquence d'Inclusion.")
            Else
                WriteLog("ERR: Impossible d'envoyer une séquence d'Inclusion.")
            End If
        End Sub

        ''' <summary>
        ''' Place le controller en mode "inclusion securisée"
        ''' </summary>
        ''' <remarks></remarks>
        Sub StartSecureInclusionMode()
            'WriteLog("Début de la séquence d'association sécurisée.")
            RemoveFailedNode(0)  'preferable avant une inclusion sécurisée
            If m_manager.AddNode(m_homeId, True) Then
                WriteLog("Début de la séquence d'Inclusion sécurisée.")
            Else
                WriteLog("ERR: Impossible d'envoyer une séquence d'Inclusion.")
            End If
        End Sub

        ''' <summary>
        ''' Place le controller en mode "exclusion"
        ''' </summary>
        ''' <remarks></remarks>
        Sub StartExclusionMode()
            ' WriteLog("Début de la séquence désassociation.")
            If m_manager.RemoveNode(m_homeId) Then
                WriteLog("Début de la séquence d'Exclusion.")
            Else
                WriteLog("ERR: Impossible d'envoyer une séquence de d'Exclusion.")
            End If
        End Sub

        ''' <summary>
        ''' Annule la commande en cours : permet de sortir du mode "inclusion/exclusion" *** experimental ***
        ''' </summary>
        ''' <remarks></remarks>
        Sub StopAssociation()
            If m_manager.CancelControllerCommand(m_homeId) Then
                WriteLog("Annule la commande en cours.")
            Else
                WriteLog("ERR: Impossible d'envoyer la commande d'annulation d'association.")
            End If
        End Sub

        ''' <summary>
        ''' Suppresion des noeuds morts
        ''' </summary>
        ''' <remarks></remarks>
        Sub RemoveFailedNode(Optional ByVal NumNode As Byte = Nothing)
            WriteLog("RemoveFailedNode, Nombre de noeud à analyser : " & m_nodeList.Count)
            Try
                Dim node As Node
                Dim i As Integer
                Dim Trouve As Boolean = False

                If NumNode = Nothing Then NumNode = 0
                'Supprime tous les noeuds Morts
                If NumNode = 0 Then
                    For i = 0 To m_nodeList.Count - 1
                        node = GetNode(m_homeId, m_nodeList.ElementAt(i).ID)
                        If (m_manager.IsNodeFailed(m_homeId, node.ID)) Then
                            WriteLog("Noeud Mort : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product & " -> " & m_nodeList.ElementAt(i).Label)
                            If m_manager.RemoveFailedNode(m_homeId, node.ID) Then
                                WriteLog("Noeud Supprimé : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product)
                            Else
                                WriteLog("ERR: Probleme lors de la suppression du noeud : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product & " -> " & m_nodeList.ElementAt(i).Label)
                            End If
                        Else
                            WriteLog("Noeud actif : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product & " -> " & m_nodeList.ElementAt(i).Label)
                        End If
                    Next
                End If

                'Supprime le noeud passé en paramètre
                If NumNode > 1 Then
                    For i = 0 To m_nodeList.Count - 1
                        node = GetNode(m_homeId, m_nodeList.ElementAt(i).ID)
                        If NumNode = node.ID Then
                            Trouve = True
                            If m_manager.RemoveFailedNode(m_homeId, node.ID) Then
                                WriteLog("Noeud Supprimé : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product & " -> " & m_nodeList.ElementAt(i).Product)
                            Else
                                WriteLog("ERR: Probleme lors de la suppression du noeud : " & m_nodeList.ElementAt(i).ID & " / " & m_nodeList.ElementAt(i).Product & " -> " & m_nodeList.ElementAt(i).Label)
                            End If
                            Exit For
                        End If
                    Next
                    If Trouve = False Then
                        WriteLog("ERR: RemoveFailedNode, Numéro du noeud incorrect : " & NumNode.ToString)
                    End If
                End If
                ' WriteLog("RemoveFailedNode, Nombre de noeud présent aprés analyse :" & m_nodeList.Count)
            Catch ex As Exception
                WriteLog("ERR: RemoveFailedNode, Probleme lors de la suppression des noeuds")
            End Try
        End Sub

        Sub ManagedControllerStateChangedHandler(ByVal m_controllerState As ZWControllerState)
            WriteLog("Controller State Change," & m_controllerState.ToString())
        End Sub

        ''' <summary>Handler lors d'une reception d'une trame</summary>
        ''' <param name="m_notification"></param>
        Sub NotificationHandler(ByVal m_notification As ZWNotification)
            Try
                If m_notification Is Nothing Then Exit Sub
                Select Case m_notification.[GetType]()
                    Case ZWNotification.Type.ValueAdded
                        If m_notification.GetNodeId() <> m_manager.GetControllerNodeId(m_homeId) Then
                            WriteLog("DBG: " & "NotificationHandler - ValueAdded sur node " & m_notification.GetNodeId())
                            Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                            If Not IsNothing(node) Then
                                Dim ValueId As ZWValueID = m_notification.GetValueID()
                                WriteLog("DBG: " & "NotificationHandler valeurId : " & ValueId.GetId() & " de type : " & ValueId.GetType().ToString & " Instance : " & ValueId.GetInstance())
                                WriteLog("DBG: " & "NotificationHandler d'index : " & ValueId.GetIndex() & " de classe : " & ValueId.GetCommandClassId().ToString & " de label : " & m_manager.GetValueLabel(ValueId))
                                node.AddValue(m_notification.GetValueID())

                                If Not (node.CommandClass.Contains(ValueId.GetCommandClassId())) Then
                                    node.CommandClass.Add(ValueId.GetCommandClassId())
                                    WriteLog("DBG: " & "NotificationHandler - NodeAdded sur node " & m_notification.GetNodeId() & " Classe : " & node.CommandClass.IndexOf(ValueId.GetCommandClassId()).ToString & " trouvée - ajout à la liste")
                                End If

                            Else
                                ' Message si le noeud n'est pas le controleur
                                If (node.ID <> m_manager.GetControllerNodeId(m_homeId)) Then WriteLog("DBG: " & "NotificationHandler - Erreur dans ValueAdded : node " & m_notification.GetNodeId() & " non trouvé")
                            End If
                        End If

                    Case ZWNotification.Type.ValueRemoved
                        WriteLog("DBG: " & "NotificationHandler - ValueRemoved sur node " & m_notification.GetNodeId())
                        Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                        If Not IsNothing(node) Then node.RemoveValue(m_notification.GetValueID())

                    Case ZWNotification.Type.ValueChanged
                        WriteLog("DBG: " & "NotificationHandler - ValueChanged sur node " & m_notification.GetNodeId())
                        Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                        If Not IsNothing(node) Then
                            traiteValeur(m_notification)
                        Else
                            ' Message si le noeud n'est pas le controleur
                            If (node.ID <> m_manager.GetControllerNodeId(m_homeId)) Then WriteLog("ERR: " & "NotificationHandler - Erreur dans ValueChanged : node " & m_notification.GetNodeId() & " non trouvé")
                        End If


                    Case ZWNotification.Type.ValueRefreshed
                        WriteLog("DBG: " & "NotificationHandler - Refreshed sur node " & m_notification.GetNodeId())

                    Case ZWNotification.Type.Group
                        Dim cpt As Byte = Nothing
                        Dim RetGet As UInt32 = Nothing

                        WriteLog("DBG: " & "NotificationHandler - Group " & m_notification.GetGroupIdx() & " sur node " & m_notification.GetNodeId())
                        Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                        If Not IsNothing(node) Then
                            Dim Assoc() As Byte = Nothing
                            RetGet = m_manager.GetAssociations(m_homeId, node.ID, m_notification.GetGroupIdx(), Assoc)
                            If RetGet Then
                                WriteLog("DBG: " & "NotificationHandler - Des groupes ont été trouvés avec : " & RetGet & " elements")
                                For Each Tempval As Byte In Assoc
                                    WriteLog("DBG: " & "NotificationHandler - Les elements du groupe n° " & cpt & " sont " & Tempval.ToString)
                                Next
                            End If
                        End If

                    Case ZWNotification.Type.NodeAdded
                        ' Ajoute une nouveau noeud à notre liste
                        Dim node As New Node
                        ' Si ce n'est pas le controleur 
                        If m_notification.GetNodeId() <> m_manager.GetControllerNodeId(m_homeId) Then
                            WriteLog("NotificationHandler - Ajout d'un nouveau noeud " & m_notification.GetNodeId())
                            node.ID = m_notification.GetNodeId()
                            node.HomeID = m_notification.GetHomeId()
                            m_nodeList.Add(node)
                        End If

                    Case ZWNotification.Type.NodeRemoved
                        If m_notification.GetNodeId() <> m_manager.GetControllerNodeId(m_homeId) Then
                            WriteLog("NotificationHandler - Suppression du noeud " & m_notification.GetNodeId())
                            For Each node As Node In m_nodeList
                                If node.ID = m_notification.GetNodeId() Then
                                    m_nodeList.Remove(node)
                                    Exit For
                                End If
                            Next
                            If _Getconfig Then Get_Config(_NomFileConfigZWave)
                        End If

                    Case ZWNotification.Type.NodeProtocolInfo
                        If m_notification.GetNodeId() <> m_manager.GetControllerNodeId(m_homeId) Then
                            WriteLog("DBG: " & "NotificationHandler - NodeProtocolInfo sur node " & m_notification.GetNodeId())
                            Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                            If Not IsNothing(node) Then
                                node.Label = m_manager.GetNodeType(m_homeId, node.ID)
                            Else
                                WriteLog("ERR: " & "NotificationHandler - Erreur dans NodeProtocolInfo : node " & m_notification.GetNodeId() & " non trouvé")
                            End If
                        End If


                    Case ZWNotification.Type.NodeNaming
                        If m_notification.GetNodeId() <> m_manager.GetControllerNodeId(m_homeId) Then
                            WriteLog("DBG: " & "NotificationHandler - NodeNaming sur node " & m_notification.GetNodeId())
                            Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                            If Not IsNothing(node) Then
                                node.Manufacturer = m_manager.GetNodeManufacturerName(m_homeId, node.ID)
                                node.Product = m_manager.GetNodeProductName(m_homeId, node.ID)
                                node.Location = m_manager.GetNodeLocation(m_homeId, node.ID)
                                node.Name = m_manager.GetNodeName(m_homeId, node.ID)
                                WriteLog("DBG: " & "NotificationHandler - ID: " & node.ID & ", Nom: " & node.Name & ", Manufacturer: " & node.Manufacturer)
                                WriteLog("DBG: " & "NotificationHandler - Produit: " & node.Product & ", Label : " & node.Label)
                                WriteLog("DBG: " & "NotificationHandler - Nb noeuds: " & m_nodeList.Count & ", Location: " & node.Location & ", Nb values: " & node.Values.Count)
                            Else
                                ' Message si le noeud n'est pas le controleur
                                WriteLog("ERR: " & "NotificationHandler - Erreur dans NodeNaming  : node " & m_notification.GetNodeId() & " non trouvé")
                            End If
                        End If


                    Case ZWNotification.Type.NodeEvent
                        WriteLog("DBG: " & "NotificationHandler - NodeEvent sur node " & m_notification.GetNodeId())
                        Dim node As Node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId())
                        If Not IsNothing(node) Then traiteValeur(m_notification)

                    Case ZWNotification.Type.PollingDisabled
                        WriteLog("DBG: " & "NotificationHandler - PollingDisabled sur node " & m_notification.GetNodeId())

                    Case ZWNotification.Type.PollingEnabled
                        WriteLog("DBG: " & "NotificationHandler - PollingEnabled sur node " & m_notification.GetNodeId())

                    Case ZWNotification.Type.SceneEvent
                        WriteLog("DBG: " & "NotificationHandler - SceneEvent " & m_notification.GetNodeId())

                    Case ZWNotification.Type.CreateButton
                        WriteLog("DBG: " & "NotificationHandler - CreateButton " & m_notification.GetNodeId())

                    Case ZWNotification.Type.DeleteButton
                        WriteLog("DBG: " & "NotificationHandler - DeleteButton " & m_notification.GetNodeId())

                    Case ZWNotification.Type.ButtonOn
                        WriteLog("DBG: " & "NotificationHandler - ButtonOn " & m_notification.GetNodeId())

                    Case ZWNotification.Type.ButtonOff
                        WriteLog("DBG: " & "NotificationHandler - ButtonOff " & m_notification.GetNodeId())

                    Case ZWNotification.Type.DriverReady
                        m_homeId = m_notification.GetHomeId()
                        WriteLog("DBG: " & "NotificationHandler - DriverReady")

                    Case ZWNotification.Type.DriverReset
                        m_homeId = m_notification.GetHomeId()
                        WriteLog("DBG: " & "NotificationHandler - DriverReset")

                    Case ZWNotification.Type.EssentialNodeQueriesComplete
                        WriteLog("DBG: " & "NotificationHandler - EssentialNodeQueriesComplete")

                    Case ZWNotification.Type.NodeQueriesComplete
                        WriteLog("DBG: " & "NotificationHandler - NodeQueriesComplete")


                    Case ZWNotification.Type.AllNodesQueried
                        WriteLog("DBG: " & "NotificationHandler - AllNodesQueried")
                        m_nodesReady = True
                        ' Simulation de noeuds 
                        ' SimulNode()

                    Case ZWNotification.Type.AwakeNodesQueried
                        WriteLog("DBG: " & "NotificationHandler - AwakeNodesQueried")

                    Case ZWNotification.Type.AllNodesQueriedSomeDead
                        WriteLog("DBG: " & Me.Nom & " NotificationHandler - AllNodesQueriedSomeDead")

                    Case ZWNotification.Type.ControllerCommand
                        WriteLog("DBG: " & "NotificationHandler - ControllerCommand sur le noeud : " & m_notification.GetNodeId())

                    Case ZWNotification.Type.NodeReset
                        WriteLog("DBG: " & "NotificationHandler - NodeReset : " & m_notification.GetNodeId())

                    Case Else
                        WriteLog("DBG: " & "NotificationHandler - Une notification a été reçue : " & m_notification.[GetType]())
                End Select
            Catch ex As Exception
                WriteLog("ERR: " & "NotificationHandler Exception: " & ex.Message)
            End Try
        End Sub

        ''' <summary>Gets a node based on the homeId and the nodeId</summary>
        ''' <param name="homeId"></param>
        ''' <param name="nodeId"></param>
        ''' <returns>Node</returns>
        Private Function GetNode(ByVal homeId As UInt32, ByVal nodeId As Byte) As Node
            Try
                For Each node As Node In m_nodeList
                    If (node.ID = nodeId) And (node.HomeID = homeId) Then Return node
                Next
            Catch ex As Exception
                WriteLog("ERR: " & "GetNode Exception: " & ex.Message)
            End Try
            Return Nothing
        End Function

        ''' <summary>Gets a Node value ID</summary>
        ''' <param name="node"></param>
        ''' <param name="valueLabel"></param>
        ''' <returns>ValueId</returns>
        Private Function GetValeur(ByVal node As Node, ByVal valueLabel As String, Optional ByVal ValueInstanc As String = "") As ZWValueID
            Try
                WriteLog("DBG: " & "GetValueID, Receive from node:" & node.ID & ":" & " Label:" & valueLabel & " Instance:" & ValueInstanc)

                For Each valueID As ZWValueID In node.Values
                    WriteLog("DBG: " & "GetValueID, Value from node:" & valueID.GetNodeId() & "-" & valueID.GetId() & ":" & "Label:" & m_manager.GetValueLabel(valueID).ToString & " Instance:" & valueID.GetInstance)
                    If (valueID.GetNodeId() = node.ID) And (m_manager.GetValueLabel(valueID).ToLower = valueLabel.ToLower) Then
                        If ValueInstanc <> "" Then
                            Dim ValueInstance As Byte = 0
                            If (InStr(ValueInstanc, ".") > 0) Then
                                ValueInstance = Mid(ValueInstanc, 1, InStr(ValueInstanc, ".") - 1)
                            Else
                                ValueInstance = Mid(ValueInstanc, 1, Len(ValueInstanc))
                            End If
                            If valueID.GetInstance() = ValueInstance Then
                                WriteLog("DBG: " & "GetValueID, Valeur trouvée  Instance:" & ValueInstanc)
                                If (InStr(ValueInstanc, ".") > 0) Then
                                    Dim ValueIndex As Byte = Mid(ValueInstanc, InStr(ValueInstanc, ".") + 1, Len(ValueInstanc))
                                    If valueID.GetIndex() = ValueIndex Then
                                        WriteLog("DBG: " & "GetValueID, Valeur trouvée  Index:" & ValueIndex)
                                        Return valueID
                                    End If
                                Else
                                    Return valueID
                                End If
                            End If
                        Else
                            Return valueID
                        End If
                    End If
                Next
            Catch ex As Exception
                WriteLog("ERR: " & "GetValueID Exception: " & ex.Message)
            End Try
            Return Nothing
        End Function

        ''' <summary>processing the received value</summary>
        ''' <param name="m_notification"></param>
        Private Sub traiteValeur(ByVal m_notification As ZWNotification) ', Optional ByVal FromEvent As Boolean = False)


            _DEBUG = _Parametres.Item(0).Valeur

            Dim m_valueLabel As String = ""
            Dim m_instance As Integer = 0
            Dim m_nodeId As Integer = 0
            Dim m_valueID As ZWValueID
            Dim m_valueString As String = ""
            Dim m_tempString As String = ""
            Dim m_devices As New ArrayList()
            Dim m_index As Integer

            m_valueLabel = m_manager.GetValueLabel(m_notification.GetValueID())
            m_nodeId = m_notification.GetNodeId()
            m_instance = m_notification.GetValueID.GetInstance()
            m_index = m_notification.GetValueID.GetIndex()
            m_valueID = m_notification.GetValueID()
            m_manager.GetValueAsString(m_valueID, m_valueString)

            Dim ValeurRecue As Object = Nothing

            Try
                ' WriteLog("DBG: " & "Receive from " & m_nodeId & ":" & m_instance & " -> " & m_valueLabel & "=" & m_valueString)
                If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte

                If (_STARTIDLETIME > 0) Then
                    If DateTime.Now < DateAdd(DateInterval.Second, _STARTIDLETIME, dateheurelancement) Then Exit Sub 'on ne traite rien pendant les 10 premieres secondes
                End If

                ' Log tous les informations en mode debug
                WriteLog("DBG: " & "Receive from " & m_nodeId & ":" & m_instance & "." & m_index & " -> " & m_valueLabel & "=" & m_valueString)

                'Recherche si un device affecté
                m_devices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, m_nodeId, "", Me._ID, True)
                WriteLog("DBG: NODE : " & m_nodeId & " -> Nb Device " & m_devices.Count)
                '               Else
                'Recherche si un device affecté
                ' compatibilité avec combobox si device pas trouve avec ancienne appellation
                'permet de gerer la compatibilité du driver avec listbox (ex: "3 # ZMNHAA2 Flush 1 Relay") ds adresse1
                Dim adr1 As String = ""
                For i As Integer = 0 To _Adr1Txt.Count - 1
                    If InStr(1, _Adr1Txt(i), m_nodeId & " # ") > 0 Then
                        adr1 = _Adr1Txt(i)
                        Exit For
                    End If
                Next

                Dim m_devicestmp As New ArrayList()
                m_devicestmp = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adr1, "", Me._ID, True)
                If m_devicestmp.Count > 0 Then
                    WriteLog("DBG: ADR1 : " & adr1 & " -> Nb Device " & m_devicestmp.Count)
                    m_devices.AddRange(m_devicestmp)
                End If
                WriteLog("DBG: m_devices total -> Nb Device " & m_devices.Count)

                ' Pas de composant => ajout automatique dans la liste des nouveaux composants
                If (m_devices.Count = 0 And _AutoDiscover) Then


                    ' Ne traite pas les notifications COMMAND_CLASS_CONFIGURATION & COMMAND_CLASS_VERSION
                    '                    If (m_valueID.GetCommandClassId() = 132 Or m_valueID.GetCommandClassId() = 134) Then Exit Sub
                    If (m_valueID.GetCommandClassId() = 112 Or m_valueID.GetCommandClassId() = 134) Then Exit Sub

                    ' Contrôles
                    Dim m_manufacturerName As String = m_manager.GetNodeManufacturerName(m_notification.GetHomeId(), m_nodeId)
                    Dim m_productName As String = m_manager.GetNodeProductName(m_notification.GetHomeId(), m_nodeId)

                    If (String.IsNullOrEmpty(m_manufacturerName) And String.IsNullOrEmpty(m_productName)) Then
                        WriteLog("ERR: TraiteValeur " & "Impossible de déterminer le nom et le fabriquant de l'équipement !")
                    End If
                    If (String.IsNullOrEmpty(m_manufacturerName)) Then m_manufacturerName = "Unknown"
                    If (String.IsNullOrEmpty(m_productName)) Then m_productName = "Unknown"


                    'Génération du nom : ZWaveDevice (<controllerId>:<nodeId>:<instance>) <manufacturerName> <productName> - <valueLabel> (<commandClassId>.<index>)
                    'Ex. ZWaveDevice (1:7:1) Everspring ST814 Temperature and Humidity Sensor - Battery Level (128.0)
                    '    ZWaveDevice (1:12:1) Aeon Labs Multi Sensor - Temperature (49.1)
                    '    ZWaveDevice (1:2:1) Everspring SM103 Door/Window Sensor - Sensor (48.0)
                    Dim m_deviceName As String = String.Format("ZWaveDevice ({0}:{1}:{2}.{3}) {4} {5} - {6} ({7} {8})",
                                                               m_manager.GetControllerNodeId(m_notification.GetHomeId()),
                                                               m_nodeId,
                                                               m_instance,
                                                               m_index,
                                                               m_manufacturerName,
                                                               m_productName,
                                                               m_valueLabel, m_valueID.GetCommandClassId, m_valueID.GetIndex())

                    ' Vérifie que le composant n'a pas déja été ajouté à la liste des nouveaux composants.
                    Dim m_newDevice As HoMIDom.HoMIDom.NewDevice =
                        (From dev In _Server.GetAllNewDevice() _
                            Where dev.IdDriver = Me.ID _
                            And dev.Adresse1 = m_nodeId _
                            And dev.Adresse2 = m_valueLabel).FirstOrDefault()

                    ' Non trouvé :Création du composant
                    If (m_newDevice Is Nothing) Then

                        Dim m_device As New HoMIDom.HoMIDom.NewDevice
                        m_device.ID = System.Guid.NewGuid.ToString() 'génération d'un nouveau GUID
                        m_device.Adresse1 = m_nodeId 'ID du node
                        m_device.Adresse2 = m_valueLabel & "." & m_index 'type de composant
                        m_device.IdDriver = Me.ID
                        m_device.Type = String.Empty 'TEMPERATURE, HUMIDITE, ... 
                        m_device.Ignore = False
                        m_device.DateTetect = Now
                        m_device.Value = m_valueString
                        m_device.Name = m_deviceName

                        Select Case m_device.Adresse2.ToLower().Trim()
                            Case "Battery Level".ToLower()
                                m_device.Type = ListeDevices.BATTERIE.ToString()
                            Case "Temperature".ToLower()
                                m_device.Type = ListeDevices.TEMPERATURE.ToString()
                            Case "Sensor".ToLower()
                                m_device.Type = ListeDevices.CONTACT.ToString()
                            Case "Relative Humidity".ToString()
                                m_device.Type = ListeDevices.HUMIDITE.ToString()
                            Case "Moist Air".ToString()
                                m_device.Type = ListeDevices.HUMIDITE.ToString()
                            Case "Energy".ToLower()
                                m_device.Type = ListeDevices.ENERGIETOTALE.ToString()
                            Case "Previous Reading".ToLower()
                                m_device.Type = ListeDevices.ENERGIETOTALE.ToString()
                            Case "Power".ToLower()
                                m_device.Type = ListeDevices.ENERGIEINSTANTANEE.ToString()
                            Case "Cooling 1".ToLower()
                                m_device.Type = ListeDevices.TEMPERATURECONSIGNE.ToString()
                            Case "Auto Changeover".ToLower()
                                m_device.Type = ListeDevices.TEMPERATURECONSIGNE.ToString()
                            Case "Heating 1".ToLower()
                                m_device.Type = ListeDevices.TEMPERATURECONSIGNE.ToString()
                            Case "Dry Air".ToLower()
                                m_device.Type = ListeDevices.TEMPERATURECONSIGNE.ToString()
                        End Select

                        'Vérification que le driver supporte bien ce type de composant
                        If (Not _DeviceSupport.Contains(m_device.Adresse2)) Then
                            m_device.Type = String.Empty
                        End If

                        WriteLog("NewDevice " & String.Format("{0} (CommandClassId={1},Genre={2},Index={3},Type={4}, Units={5})", _
                                               m_deviceName, _
                                               m_valueID.GetCommandClassId(), m_valueID.GetGenre(), m_valueID.GetIndex(), m_valueID.GetType(), m_manager.GetValueUnits(m_valueID)))

                        _Server.GetAllNewDevice().Add(m_device)

                    Else
                        'Composant déja détecté, maj de la valeur.
                        ' *** TODO ***
                    End If
                End If

                Dim TEMP(10) As String   'a vérifier si 10 est suffisant ?
                Dim Posit As Integer

                ' Il existe au moins un composant utilisé avec cet Id
                If m_devices.Count > 0 Then

                    ' Recherche du composant en fonction de l'adresse1
                    For Each LocalDevice As Object In m_devices

                        ' Le numéro du noeud est passé en Adresse1.
                        ' L'adresse2 correspond au label de la valeur recherchée
                        ' L'adresse2 peut aussi être vide (pour les notifications de noeud sans label, p.ex. détecteur de mouvement)
                        ' En option, l'adresse2 peut contenir l'instance sous le format label:instance
                        ' En option, l'adresse2 peut contenir l'index sous le format label:instance.index
                        'If (IsNothing(LocalDevice.adresse2) Or (LocalDevice.adresse2 = m_valueLabel) Or (LocalDevice.adresse2 = m_valueLabel & ":" & m_instance)) Then 'Then

                        If (IsNothing(LocalDevice.adresse2) Or (LocalDevice.adresse2 = m_valueLabel) Or (LocalDevice.adresse2 = m_valueLabel & ":" & m_instance) Or (LocalDevice.adresse2 = m_valueLabel & ":" & m_instance & "." & m_index)) Then
                            'on maj la value si la durée entre les deux receptions est > à 1.5s
                            If (DateTime.Now - Date.Parse(LocalDevice.LastChange)).TotalMilliseconds > 1500 Then
                                ' Recuperation de la valeur en fonction du type
                                Select Case m_valueID.GetType()
                                    Case 0 : m_manager.GetValueAsBool(m_valueID, ValeurRecue) 'm_manager.GetValueAsBool(TempValeur, LocalDevice.value)
                                    Case 1 : m_manager.GetValueAsByte(m_valueID, ValeurRecue) ' GetValueAsByte(TempValeur, LocalDevice.value)
                                    Case 2 : m_manager.GetValueAsString(m_valueID, ValeurRecue) ' GetValueAsDecimal(TempValeur, LocalDevice.value)
                                        ValeurRecue = Regex.Replace(CStr(ValeurRecue), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                        '   Case 2 : m_manager.GetValueAsDecimal(m_valueID, ValeurRecue) ' GetValueAsDecimal(TempValeur, LocalDevice.value)
                                        '       ValeurRecue = Regex.Replace(CStr(ValeurRecue), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                    Case 3 : m_manager.GetValueAsInt(m_valueID, ValeurRecue) ' m_manager.GetValueAsInt(TempValeur, LocalDevice.value)
                                        '  Case 4 : m_manager.GetValueListItems(NodeTemp.Values(IndexTemp), LocalDevice.value) ; A voir + tard
                                    Case 4 : m_manager.GetValueListItems(m_valueID, TEMP)
                                        m_manager.GetValueListSelection(m_valueID, Posit)
                                        If TEMP(0) = "Monday" Then  'Correction lecture du jour Damfoss (tableau commençant à 1 ??)
                                            Posit = Posit - 1
                                        End If
                                        ValeurRecue = TEMP(Posit)
                                    Case 6 : m_manager.GetValueAsShort(m_valueID, ValeurRecue) ' m_manager.GetValueAsShort(TempValeur, LocalDevice.value)
                                    Case 7 : m_manager.GetValueAsString(m_valueID, ValeurRecue) ' m_manager.GetValueAsString(TempValeur, LocalDevice.value)
                                    Case Else
                                        WriteLog("ERR: " & " traiteValeur" & " Type non traité Exception : " & CStr(m_valueID.GetType()))
                                End Select

                                ' Traitement particulier pour les temperatures
                                If UCase(LocalDevice.type) = "TEMPERATURE" Or UCase(LocalDevice.type) = "TEMPERATURECONSIGNE" Then
                                    Select Case UCase(m_manager.GetValueUnits(m_valueID))
                                        Case "F"
                                            If LocalDevice.unit = "°C" Then
                                                Dim myValue As Single = Math.Round(CDec(Int((ValeurRecue - 32) * 5) / 9), 1)
                                                ValeurRecue = myValue.ToString
                                            End If
                                        Case "C"
                                            If LocalDevice.unit = "°F" Then
                                                Dim myValue As Single = Math.Round(CDec(Int((ValeurRecue * 9) / 5) + 32), 1)
                                                ValeurRecue = myValue.ToString
                                            End If
                                        Case Else
                                            If m_manager.GetValueUnits(m_valueID) <> "" Then WriteLog("ERR: " & "traiteValeur, " & UCase(LocalDevice.type) & " Unité non traitée " & m_manager.GetValueUnits(m_valueID))
                                    End Select
                                End If

                                If UCase(m_valueLabel) = "COLOR" Then
                                    'transforme code couleur hexa en numerique
                                    If InStr(ValeurRecue, "#") > 0 Then
                                        ValeurRecue = Mid(ValeurRecue, InStr(ValeurRecue, "#") + 1, Len(ValeurRecue))
                                        ValeurRecue = RGB(CLng("&H" & Mid(ValeurRecue, 1, 2)), CLng("&H" & Mid(ValeurRecue, 3, 2)), CLng("&H" & Mid(ValeurRecue, 5, 2)))
                                    End If
                                End If

                                LocalDevice.value = ValeurRecue

                                'gestion de l'information de Batterie
                                If UCase(m_valueLabel) = "BATTERY LEVEL" Then
                                    If m_valueString <= 10 Then WriteLog("ERR: " & LocalDevice.nom & " : Battery vide")
                                End If


                                WriteLog("DBG: " & "Z-Wave NodeID: " & m_nodeId)
                                WriteLog("DBG: " & "Z-Wave Label: " & m_valueLabel)
                                WriteLog("DBG: " & "Z-Wave ValueUnit: " & m_manager.GetValueUnits(m_notification.GetValueID()))
                                WriteLog("DBG: " & "Valeur Homidom relevée: " & LocalDevice.value & " de type " & LocalDevice.GetType.Name)
                            Else
                                WriteLog("DBG: " & "Reception < 1.5s de deux valeurs pour le meme composant : " & m_nodeId & ":" & m_instance & " -> " & m_valueLabel & "=" & m_valueString)
                            End If
                        End If

                    Next
                    WriteLog("DBG: " & "Mise à jour de la valeur à l'adresse : " & m_nodeId & ":" & m_instance & " -> " & m_valueLabel & "=" & m_valueString)

                Else
                    ' Une valeur recue sans correspondance 
                    WriteLog("DBG: " & "Receive from " & m_nodeId & ":" & m_instance & " -> " & m_valueLabel & "=" & m_valueString & " without correspondance")
                End If

                m_devices = Nothing
            Catch ex As Exception
                WriteLog("ERR: " & "traiteValeur, Exception : " & ex.Message)
            End Try
        End Sub

        Function Get_Config(nomfileconfig As String) As Boolean
            ' recupere les configurations des equipements 

            Try
                'recherche des equipements
                If m_nodeList.Count Then
                    If Not My.Computer.FileSystem.FileExists(nomfileconfig) Then
                        Return False
                        WriteLog("ERR: " & "GET_Config, fichier" & nomfileconfig & " inexistant")
                        Exit Function
                    End If

                    _Adr1Txt.Clear()
                    Dim _libelleadr1 As String = ""
                    Dim _libelleadr2 As String = ""
                    Dim response As String = ""

                    For Each NodeTemp As Node In m_nodeList
                        _libelleadr1 += NodeTemp.ID & " # " & NodeTemp.Product & "|"
                        _Adr1Txt.Add(NodeTemp.ID & " # " & NodeTemp.Product)
                        WriteLog("DBG: _libelleadr1, " & _libelleadr1)

                        'recherche des parametres de l'equipement
                        response = ""
                        response = LectureNoeudConfigXml(nomfileconfig, NodeTemp.ID)
                        If response <> "" Then
                            _libelleadr2 += response
                            WriteLog("DBG: _libelleadr2, " & response)
                        End If
                    Next


                    ' evite les doublons 
                    Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
                    For i As Integer = 0 To _LabelsDevice.Count - 1
                        ld0 = _LabelsDevice(i)
                        Select Case ld0.NomChamp
                            Case "ADRESSE1"
                                _libelleadr1 = Mid(_libelleadr1, 1, Len(_libelleadr1) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                                ld0.Parametre = _libelleadr1
                                _LabelsDevice(i) = ld0
                            Case "ADRESSE2"
                                _libelleadr2 = Mid(_libelleadr2, 1, Len(_libelleadr2) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                                ld0.Parametre = _libelleadr2
                                _LabelsDevice(i) = ld0
                        End Select
                    Next


                End If
                Return True
            Catch ex As Exception
                WriteLog("ERR: " & "GET_Config, " & ex.Message)
                Return False
            End Try
        End Function

        Function LectureNoeudConfigXml(fichierxml As String, nodeid As String) As String
            Try
                If My.Computer.FileSystem.FileExists(fichierxml) Then
                    WriteLog("DBG: Recherche dans le fichier " & fichierxml & " de la valeur " & nodeid)

                    Dim valnode As String = ""
                    Dim reader As XmlReader
                    Dim readertmp As XmlReader
                    Dim str As String = ""
                    Dim strtmp As String = ""

                    Dim xmldoc As XPathDocument = New XPathDocument(fichierxml)
                    Dim nav As XPathNavigator = xmldoc.CreateNavigator()

                    nav.MoveToChild("Driver", "http://code.google.com/p/open-zwave/")
                    Dim Nodes As XPathNodeIterator = nav.SelectChildren("", "http://code.google.com/p/open-zwave/")

                    While Nodes.MoveNext()
                        valnode = Nodes.Current.OuterXml
                        ' recherche de l'id demandé
                        If InStr(valnode, "Node id=""" & nodeid & """") > 0 Then
                            reader = XmlReader.Create(New StringReader(valnode))
                            reader.ReadToDescendant("CommandClasses")
                            While reader.Read()
                                Select Case reader.NodeType
                                    Case XmlNodeType.Element
                                        valnode = reader.ReadOuterXml
                                        'recherche des value user
                                        If InStr(valnode, "genre=""user""") > 0 Then
                                            readertmp = XmlReader.Create(New StringReader(valnode))
                                            While readertmp.Read()
                                                Select Case readertmp.NodeType
                                                    Case XmlNodeType.Element
                                                        If readertmp.Name = "Value" Then
                                                            strtmp = readertmp.Item("label") & ":" & readertmp.Item("instance") & "." & readertmp.Item("index")
                                                            ' ne rajoute que si existe pas. Evite les doublons
                                                            If InStr(1, str, strtmp) = 0 Then
                                                                str += nodeid & " #; " & strtmp & "|"
                                                                WriteLog("DBG: Str : " & str)
                                                            End If
                                                        End If
                                                End Select
                                            End While
                                            readertmp = Nothing
                                        End If
                                End Select
                            End While
                            Exit While
                            reader = Nothing
                        End If
                    End While
                    Nodes = Nothing
                    nav = Nothing
                    xmldoc = Nothing
                    Return str
                Else
                    WriteLog("ERR: " & "LectureNoeudConfigXml, fichier " & fichierxml & " introuvable")
                    Return ""
                End If

            Catch ex As Exception
                WriteLog("ERR: " & "LectureNoeudConfigXml, " & ex.Message)
                Return ""
            End Try
        End Function

        ''' <summary>Simulate a Node</summary>
        Private Sub SimulNode()
            Try
                ' Pour simulation de noeuds
                'Dim nodeTempAdd As New Node
                'nodeTempAdd.ID = 2
                'nodeTempAdd.HomeID = 21816633
                'nodeTempAdd.Manufacturer = "Everspring"
                'nodeTempAdd.Product = "ST814 Temperature and Humidity Sensor"
                'nodeTempAdd.Label = "Temperature and Humidity Sensor"
                'nodeTempAdd.Name = "Capteur Chambre"
                'nodeTempAdd.CommandClass.Add(32)
                'nodeTempAdd.CommandClass.Add(0)
                'm_nodeList.Add(nodeTempAdd)

                Dim nodeTempAdd2 As New Node
                nodeTempAdd2.ID = 4
                nodeTempAdd2.HomeID = 21816633
                nodeTempAdd2.Manufacturer = "Fibaro"
                nodeTempAdd2.Product = "FGS-221"
                nodeTempAdd2.Label = "Binary Power Switch"
                nodeTempAdd2.Name = "Rad salon Switch 1"
                nodeTempAdd2.CommandClass.Add(CommandClass.COMMAND_CLASS_SWITCH_BINARY)
                nodeTempAdd2.CommandClass.Add(CommandClass.COMMAND_CLASS_SWITCH_MULTILEVEL)
                nodeTempAdd2.CommandClass.Add(CommandClass.COMMAND_CLASS_BASIC)
                m_nodeList.Add(nodeTempAdd2)
                Dim NodeTempValue As New ZWValueID(21816633, 4, ZWValueID.ValueGenre.Basic, CommandClass.COMMAND_CLASS_SWITCH_BINARY, 1, 1, ZWValueID.ValueType.Byte, 0)
                nodeTempAdd2.Values.Add(NodeTempValue)
                Dim NodeTempValue2 As New ZWValueID(21816633, 4, ZWValueID.ValueGenre.Basic, CommandClass.COMMAND_CLASS_SWITCH_BINARY, 1, 2, ZWValueID.ValueType.Byte, 0)
                nodeTempAdd2.Values.Add(NodeTempValue2)
                ' Dim node As Node = GetNode(m_notification.GetHomeId(), 2)
                'Dim present As Boolean = node.CommandClass.Contains(CommandClass.COMMAND_CLASS_BASIC)
                'If present Then
                ' Console.WriteLine("Classe trouvée")
                ' Else
                ' Console.WriteLine("Classe non trouvée")
                ' End If
            Catch ex As Exception
                WriteLog("ERR: " & " SimulateNode Exception : " & ex.Message)
            End Try
        End Sub

        Private Sub WriteLog(ByVal message As String)
            Try
                'utilise la fonction de base pour loguer un event
                If STRGS.InStr(message, "DBG:") > 0 Then
                    If _DEBUG Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
                    End If
                ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
                Else
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, message)
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " WriteLog", ex.Message)
            End Try
        End Sub

#End Region

    End Class

End Class
