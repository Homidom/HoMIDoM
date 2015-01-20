Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server

Namespace HoMIDom

    Public Class ManagerData

#Region "Variables"
        Dim _ListDevice As New List(Of TemplateDevice)
        Dim _ListDriver As List(Of TemplateDriver)
        Dim _ListZone As New List(Of Zone)
        Dim _ListMacro As New List(Of Macro)
        Dim _ListUser As New List(Of Users.User)
        Dim _ListTrigger As New List(Of Trigger)
        Dim _ListWidgets As New List(Of Object)
        Dim _DBHisto As DataTable = Nothing
        Dim _ListVariable As New List(Of Variable)

        Dim _SequenceDriver As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'un driver
        Dim _SequenceDevice As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'un device
        Dim _SequenceTrigger As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'un trigger
        Dim _SequenceZone As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'une zone
        Dim _SequenceMacro As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement d'une macro
        Dim _SequenceServer As String = Api.GenerateGUID 'N° de sequence en cours du dernier changement du server

        Dim _serveur As IHoMIDom = Nothing
        Dim _idsrv As String = Nothing
        Dim _ClientUDP As UDPClient = Nothing
#End Region

#Region "Event"
        Public Event DataChange(e As Sequence.TypeOfSequence, Data As String)
#End Region

        Public Sub New(Server As IHoMIDom, Idsrv As String, ClientUDP As UDPClient)
            _serveur = Server
            _idsrv = Idsrv
            _ClientUDP = ClientUDP
            Reset()

        End Sub

        Public Sub Reset()
            Try
                _ListDevice.Clear()
                _ListDriver.Clear()
                _ListZone.Clear()
                _ListMacro.Clear()
                _ListUser.Clear()
                _ListTrigger.Clear()
                _ListWidgets.Clear()
                _ListVariable.Clear()
                _DBHisto = Nothing

                If _ClientUDP IsNot Nothing Then
                    RemoveHandler _ClientUDP.OnMessageReceive, AddressOf OnMessageReceiveUDP
                    RemoveHandler _ClientUDP.EvtDevice, AddressOf EvtDevice
                    RemoveHandler _ClientUDP.EvtDeviceAdd, AddressOf EvtDeviceAdd
                    RemoveHandler _ClientUDP.EvtDeviceChange, AddressOf EvtDeviceChange
                    RemoveHandler _ClientUDP.EvtDeviceDelete, AddressOf EvtDeviceDelete
                    RemoveHandler _ClientUDP.EvtDriver, AddressOf EvtDriver
                    RemoveHandler _ClientUDP.EvtDriverAdd, AddressOf EvtDriverAdd
                    RemoveHandler _ClientUDP.EvtDriverChange, AddressOf EvtDriverChange
                    RemoveHandler _ClientUDP.EvtDeviceDelete, AddressOf EvtDeviceDelete
                    RemoveHandler _ClientUDP.EvtHistoryChange, AddressOf EvtHistoryChange
                    RemoveHandler _ClientUDP.EvtLog, AddressOf EvtLog
                    RemoveHandler _ClientUDP.EvtMacro, AddressOf EvtMacro
                    RemoveHandler _ClientUDP.EvtMacroAdd, AddressOf EvtMacroAdd
                    RemoveHandler _ClientUDP.EvtMacroChange, AddressOf EvtMacroChange
                    RemoveHandler _ClientUDP.EvtMacroDelete, AddressOf EvtMacroDelete
                    RemoveHandler _ClientUDP.EvtMessage, AddressOf EvtMessage
                    RemoveHandler _ClientUDP.EvtNotification, AddressOf EvtNotification
                    RemoveHandler _ClientUDP.EvtServer, AddressOf EvtServer
                    RemoveHandler _ClientUDP.EvtServerStart, AddressOf EvtServerStart
                    RemoveHandler _ClientUDP.EvtServerShutDown, AddressOf EvtServerShutDown
                    RemoveHandler _ClientUDP.EvtTrigger, AddressOf EvtTrigger
                    RemoveHandler _ClientUDP.EvtTriggerAdd, AddressOf EvtTriggerAdd
                    RemoveHandler _ClientUDP.EvtTriggerChange, AddressOf EvtTriggerChange
                    RemoveHandler _ClientUDP.EvtTriggerDelete, AddressOf EvtTriggerDelete
                    RemoveHandler _ClientUDP.EvtUser, AddressOf EvtUser
                    RemoveHandler _ClientUDP.EvtUserAdd, AddressOf EvtUserAdd
                    RemoveHandler _ClientUDP.EvtUserChange, AddressOf EvtUserChange
                    RemoveHandler _ClientUDP.EvtUserDelete, AddressOf EvtUserDelete
                    RemoveHandler _ClientUDP.EvtVariable, AddressOf EvtVariable
                    RemoveHandler _ClientUDP.EvtVariableAdd, AddressOf EvtVariableAdd
                    RemoveHandler _ClientUDP.EvtVariableChange, AddressOf EvtVariableChange
                    RemoveHandler _ClientUDP.EvtVariableDelete, AddressOf EvtVariableDelete
                    RemoveHandler _ClientUDP.EvtZone, AddressOf EvtZone
                    RemoveHandler _ClientUDP.EvtZoneAdd, AddressOf EvtZoneAdd
                    RemoveHandler _ClientUDP.EvtZoneChange, AddressOf EvtZoneChange
                    RemoveHandler _ClientUDP.EvtZoneDelete, AddressOf EvtZoneDelete
                End If

                If _ClientUDP IsNot Nothing Then
                    AddHandler _ClientUDP.OnMessageReceive, AddressOf OnMessageReceiveUDP
                    AddHandler _ClientUDP.EvtDevice, AddressOf EvtDevice
                    AddHandler _ClientUDP.EvtDeviceAdd, AddressOf EvtDeviceAdd
                    AddHandler _ClientUDP.EvtDeviceChange, AddressOf EvtDeviceChange
                    AddHandler _ClientUDP.EvtDeviceDelete, AddressOf EvtDeviceDelete
                    AddHandler _ClientUDP.EvtDriver, AddressOf EvtDriver
                    AddHandler _ClientUDP.EvtDriverAdd, AddressOf EvtDriverAdd
                    AddHandler _ClientUDP.EvtDriverChange, AddressOf EvtDriverChange
                    AddHandler _ClientUDP.EvtDeviceDelete, AddressOf EvtDeviceDelete
                    AddHandler _ClientUDP.EvtHistoryChange, AddressOf EvtHistoryChange
                    AddHandler _ClientUDP.EvtLog, AddressOf EvtLog
                    AddHandler _ClientUDP.EvtMacro, AddressOf EvtMacro
                    AddHandler _ClientUDP.EvtMacroAdd, AddressOf EvtMacroAdd
                    AddHandler _ClientUDP.EvtMacroChange, AddressOf EvtMacroChange
                    AddHandler _ClientUDP.EvtMacroDelete, AddressOf EvtMacroDelete
                    AddHandler _ClientUDP.EvtMessage, AddressOf EvtMessage
                    AddHandler _ClientUDP.EvtNotification, AddressOf EvtNotification
                    AddHandler _ClientUDP.EvtServer, AddressOf EvtServer
                    AddHandler _ClientUDP.EvtServerStart, AddressOf EvtServerStart
                    AddHandler _ClientUDP.EvtServerShutDown, AddressOf EvtServerShutDown
                    AddHandler _ClientUDP.EvtTrigger, AddressOf EvtTrigger
                    AddHandler _ClientUDP.EvtTriggerAdd, AddressOf EvtTriggerAdd
                    AddHandler _ClientUDP.EvtTriggerChange, AddressOf EvtTriggerChange
                    AddHandler _ClientUDP.EvtTriggerDelete, AddressOf EvtTriggerDelete
                    AddHandler _ClientUDP.EvtUser, AddressOf EvtUser
                    AddHandler _ClientUDP.EvtUserAdd, AddressOf EvtUserAdd
                    AddHandler _ClientUDP.EvtUserChange, AddressOf EvtUserChange
                    AddHandler _ClientUDP.EvtUserDelete, AddressOf EvtUserDelete
                    AddHandler _ClientUDP.EvtVariable, AddressOf EvtVariable
                    AddHandler _ClientUDP.EvtVariableAdd, AddressOf EvtVariableAdd
                    AddHandler _ClientUDP.EvtVariableChange, AddressOf EvtVariableChange
                    AddHandler _ClientUDP.EvtVariableDelete, AddressOf EvtVariableDelete
                    AddHandler _ClientUDP.EvtZone, AddressOf EvtZone
                    AddHandler _ClientUDP.EvtZoneAdd, AddressOf EvtZoneAdd
                    AddHandler _ClientUDP.EvtZoneChange, AddressOf EvtZoneChange
                    AddHandler _ClientUDP.EvtZoneDelete, AddressOf EvtZoneDelete
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "Reset", "Erreur:" & ex.Message)
            End Try
        End Sub

#Region "Load"
        Public Sub LoadAll()
            Try
                LoadDevices()
                LoadDrivers()
                LoadMacros()
                LoadTriggers()
                LoadUsers()
                LoadZones()
                LoadVariables()
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "LoadAll", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Sub LoadDevices()
            Try
                If _serveur IsNot Nothing Then
                    _ListDevice = _serveur.GetAllDevices(_idsrv)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "LoadDevices", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Sub LoadDrivers()
            Try
                If _serveur IsNot Nothing Then
                    _ListDriver = _serveur.GetAllDrivers(_idsrv)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "LoadDrivers", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Sub LoadMacros()
            Try
                If _serveur IsNot Nothing Then
                    _ListMacro = _serveur.GetAllMacros(_idsrv)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "LoadMacros", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Sub LoadZones()
            Try
                If _serveur IsNot Nothing Then
                    _ListZone = _serveur.GetAllZones(_idsrv)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "LoadZones", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Sub LoadTriggers()
            Try
                If _serveur IsNot Nothing Then
                    _ListTrigger = _serveur.GetAllTriggers(_idsrv)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "LoadTriggers", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Sub LoadUsers()
            Try
                If _serveur IsNot Nothing Then
                    _ListUser = _serveur.GetAllUsers(_idsrv)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "LoadUsers", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Sub LoadVariables()
            Try
                If _serveur IsNot Nothing Then
                    _ListVariable = _serveur.GetAllVariables(_idsrv)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "LoadUsers", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Sub LoadHisto()
            Try
                If _serveur IsNot Nothing Then
                    _DBHisto = _serveur.GetTableDBHisto(_idsrv)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "LoadUsers", "Erreur:" & ex.Message)
            End Try
        End Sub
#End Region

#Region "Property/Sub"

        Public Property ListDevice As List(Of TemplateDevice)
            Get
                Return _ListDevice
            End Get
            Set(value As List(Of TemplateDevice))
                _ListDevice = value
            End Set
        End Property

        Public Sub ChangeDevice(ID As String)
            Try
                For Each _dev In _ListDevice
                    If _dev.ID = ID Then
                        _dev = _serveur.ReturnDeviceByID(_idsrv, ID)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ChangeDevice", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Function ReturnDeviceByID(ID As String) As TemplateDevice
            Try
                Dim retour As TemplateDevice = Nothing

                For Each v In _ListDevice
                    If v.ID = ID Then
                        retour = v
                        Exit For
                    End If
                Next

                Return retour
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ReturnDeviceByID", "Erreur:" & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Property ListDriver As List(Of TemplateDriver)
            Get
                Return _ListDriver
            End Get
            Set(value As List(Of TemplateDriver))
                _ListDriver = value
            End Set
        End Property

        Public Sub ChangeDriver(ID As String)
            Try
                For Each _drv In _ListDriver
                    If _drv.ID = ID Then
                        _drv = _serveur.ReturnDriverByID(_idsrv, ID)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ChangeDriver", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Function ReturnDriverByID(ID As String) As TemplateDriver
            Try
                Dim retour As TemplateDriver = Nothing

                For Each v In _ListDriver
                    If v.ID = ID Then
                        retour = v
                        Exit For
                    End If
                Next

                Return retour
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ReturnDriverByID", "Erreur:" & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Property ListZone As List(Of Zone)
            Get
                Return _ListZone
            End Get
            Set(value As List(Of Zone))
                _ListZone = value
            End Set
        End Property

        Public Sub ChangeZone(ID As String)
            Try
                For Each _zon In _ListZone
                    If _zon.ID = ID Then
                        _zon = _serveur.ReturnZoneByID(_idsrv, ID)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ChangeZone", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Function ReturnZoneByID(ID As String) As Zone
            Try
                Dim retour As Zone = Nothing

                For Each v In _ListZone
                    If v.ID = ID Then
                        retour = v
                        Exit For
                    End If
                Next

                Return retour
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ReturnZoneByID", "Erreur:" & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Property ListMacro As List(Of Macro)
            Get
                Return _ListMacro
            End Get
            Set(value As List(Of Macro))
                _ListMacro = value
            End Set
        End Property

        Public Sub ChangeMacro(ID As String)
            Try
                For Each _mac In _ListMacro
                    If _mac.ID = ID Then
                        _mac = _serveur.ReturnMacroById(_idsrv, ID)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ChangeMacro", "Erreur:" & ex.Message)
            End Try
        End Sub


        Public Function ReturnMacroByID(ID As String) As Macro
            Try
                Dim retour As Macro = Nothing

                For Each v In _ListMacro
                    If v.ID = ID Then
                        retour = v
                        Exit For
                    End If
                Next

                Return retour
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ReturnMacroByID", "Erreur:" & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Property ListUser As List(Of Users.User)
            Get
                Return _ListUser
            End Get
            Set(value As List(Of Users.User))
                _ListUser = value
            End Set
        End Property

        Public Sub ChangeUser(ID As String)
            Try
                For Each _usr In _ListUser
                    If _usr.ID = ID Then
                        _usr = _serveur.ReturnUserById(_idsrv, ID)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ChangeUser", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Function ReturnUserByID(ID As String) As Users.User
            Try
                Dim retour As Users.User = Nothing

                For Each v In _ListUser
                    If v.ID = ID Then
                        retour = v
                        Exit For
                    End If
                Next

                Return retour
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ReturnUserByID", "Erreur:" & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Property ListTrigger As List(Of Trigger)
            Get
                Return _ListTrigger
            End Get
            Set(value As List(Of Trigger))
                _ListTrigger = value
            End Set
        End Property

        Public Sub ChangeTrigger(ID As String)
            Try
                For Each _trg In _ListTrigger
                    If _trg.ID = ID Then
                        _trg = _serveur.ReturnTriggerById(_idsrv, ID)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ChangeTrigger", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Function ReturnTriggerByID(ID As String) As Trigger
            Try
                Dim retour As Trigger = Nothing

                For Each v In _ListTrigger
                    If v.ID = ID Then
                        retour = v
                        Exit For
                    End If
                Next

                Return retour
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ReturnTriggerByID", "Erreur:" & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Property ListWidgets As List(Of Object)
            Get
                Return _ListWidgets
            End Get
            Set(value As List(Of Object))
                _ListWidgets = value
            End Set
        End Property

        Public Sub ChangeWidget(ID As String, Widget As Object)
            Try
                For Each _wid In _ListWidgets
                    If _wid.ID = ID Then
                        _wid = Widget
                        Exit For
                    End If
                Next
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ChangeWidget", "Erreur:" & ex.Message)
            End Try
        End Sub

        Public Function ReturnWidgetByID(ID As String) As Object
            Try
                Dim retour As Object = Nothing

                For Each v In _ListWidgets
                    If v.ID = ID Then
                        retour = v
                        Exit For
                    End If
                Next

                Return retour
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ReturnWidgetByID", "Erreur:" & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Property ListVariable As List(Of Variable)
            Get
                Return _ListVariable
            End Get
            Set(value As List(Of Variable))
                _ListVariable = value
            End Set
        End Property

        Public Sub ChangeVariable(ID As String)
            Try
                LoadVariables()
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "ChangeWidget", "Erreur:" & ex.Message)
            End Try
        End Sub
#End Region

#Region "Sequences"

        Public Property SequenceDevice As String
            Get
                Return _SequenceDevice
            End Get
            Set(value As String)
                _SequenceDevice = value
            End Set
        End Property

        Public Property SequenceDriver As String
            Get
                Return _SequenceDriver
            End Get
            Set(value As String)
                _SequenceDriver = value
            End Set
        End Property

        Public Property SequenceTrigger As String
            Get
                Return _SequenceTrigger
            End Get
            Set(value As String)
                _SequenceTrigger = value
            End Set
        End Property

        Public Property SequenceZone As String
            Get
                Return _SequenceZone
            End Get
            Set(value As String)
                _SequenceZone = value
            End Set
        End Property

        Public Property SequenceMacro As String
            Get
                Return _SequenceMacro
            End Get
            Set(value As String)
                _SequenceMacro = value
            End Set
        End Property

        Public Property SequenceServer As String
            Get
                Return _SequenceServer
            End Get
            Set(value As String)
                _SequenceServer = value
            End Set
        End Property

        Public Function IsNewSequenceDriver(IDSequence As String) As Boolean
            Try
                If IDSequence = _serveur.GetSequenceDriver(_idsrv) Then
                    Return True
                Else
                    Return False
                End If

            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur IsNewSequenceDriver: " & ex.ToString, "Erreur", "IsNewSequenceDriver")
                Return False
            End Try
        End Function

        Public Function IsNewSequenceDevice(IDSequence As String) As Boolean
            Try
                If IDSequence = _serveur.GetSequenceDevice(_idsrv) Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur IsNewSequenceDevice: " & ex.ToString, "Erreur", "IsNewSequenceDevice")
                Return False
            End Try
        End Function

        Public Function IsNewSequenceTrigger(IDSequence As String) As Boolean
            Try
                If IDSequence = _serveur.GetSequenceTrigger(_idsrv) Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur IsNewSequenceTrigger: " & ex.ToString, "Erreur", "IsNewSequenceTrigger")
                Return False
            End Try
        End Function

        Public Function IsNewSequenceZone(IDSequence As String) As Boolean
            Try
                If IDSequence = _serveur.GetSequenceZone(_idsrv) Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur IsNewSequenceZone: " & ex.ToString, "Erreur", "IsNewSequenceZone")
                Return False
            End Try
        End Function

        Public Function IsNewSequenceMacro(IDSequence As String) As Boolean
            Try
                If IDSequence = _serveur.GetSequenceMacro(_idsrv) Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur IsNewSequenceMacro: " & ex.ToString, "Erreur", "IsNewSequenceMacro")
                Return False
            End Try
        End Function

        Public Function IsNewSequenceServer(IDSequence As String) As Boolean
            Try
                If IDSequence = _serveur.GetSequenceServer(_idsrv) Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur IsNewSequenceServer: " & ex.ToString, "Erreur", "IsNewSequenceServer")
                Return False
            End Try
        End Function
#End Region

#Region "Gestion Event"
        ''' <summary>
        ''' Message UDP reçu
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        Private Sub OnMessageReceiveUDP(Message As String)
            Try
                If String.IsNullOrEmpty(Message) = False Then
                    Dim a() As String
                    a = Message.Split("|")
                    If a.Length > 0 Then
                        Select Case a(0).ToUpper

                        End Select
                    End If

                End If
            Catch ex As Exception
                _serveur.Log(TypeLog.ERREUR, "Erreur OnMessageReceiveUDP: " & ex.ToString, "Erreur", " OnMessageReceiveUDP")
            End Try
        End Sub

        Private Sub EvtDevice(e As Sequence)
            Try
                If e.Numero <> SequenceDevice Then
                    SequenceDevice = e.Numero
                    ChangeDevice(e.ID)
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Device, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtDevice", "Erreur" & ex.ToString, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtDeviceAdd(e As Sequence)
            Try
                If e.Numero <> SequenceDevice Then
                    SequenceDevice = e.Numero
                    LoadDevices()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.DeviceAdd, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtDeviceAdd", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtDeviceChange(e As Sequence)
            Try
                If e.Numero <> SequenceDevice Then
                    SequenceDevice = e.Numero
                    ChangeDevice(e.ID)
                    RaiseEvent DataChange(Sequence.TypeOfSequence.DeviceChange, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtDeviceChange", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtDeviceDelete(e As Sequence)
            Try
                If e.Numero <> SequenceDevice Then
                    SequenceDevice = e.Numero
                    LoadDevices()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.DeviceDelete, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtDeviceDelete", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtDriver(e As Sequence)
            Try
                If e.Numero <> SequenceDriver Then
                    SequenceDriver = e.Numero
                    LoadDrivers()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Driver, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtDriver", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtDriverChange(e As Sequence)
            Try
                If e.Numero <> SequenceDriver Then
                    SequenceDriver = e.Numero
                    ChangeDriver(e.ID)
                    RaiseEvent DataChange(Sequence.TypeOfSequence.DriverChange, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtDriverChange", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtDriverAdd(e As Sequence)
            Try
                If e.Numero <> SequenceDriver Then
                    SequenceDriver = e.Numero
                    LoadDrivers()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.DriverAdd, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtDriverAdd", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtHistoryChange(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    RaiseEvent DataChange(Sequence.TypeOfSequence.HistoryChange, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtHistoryChange", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtLog(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Log, e.Numero)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtLog", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtMacro(e As Sequence)
            Try
                If e.Numero <> SequenceMacro Then
                    SequenceMacro = e.Numero
                    LoadMacros()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Macro, e.Numero)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtMacro", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtMacroDelete(e As Sequence)
            Try
                If e.Numero <> SequenceMacro Then
                    SequenceMacro = e.Numero
                    LoadMacros()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.MacroDelete, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtMacroDelete", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtMacroChange(e As Sequence)
            Try
                If e.Numero <> SequenceMacro Then
                    SequenceMacro = e.Numero
                    ChangeMacro(e.ID)
                    RaiseEvent DataChange(Sequence.TypeOfSequence.MacroChange, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtMacroChange", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtNotification(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Notification, e.Value)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtNotification", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtMessage(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Message, e.Value)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtMessage", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtMacroAdd(e As Sequence)
            Try
                If e.Numero <> SequenceMacro Then
                    SequenceMacro = e.Numero
                    LoadMacros()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.MacroAdd, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtMacroAdd", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtServer(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Server, e.Numero)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtServer", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtServerStart(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    RaiseEvent DataChange(Sequence.TypeOfSequence.ServerStart, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtServerStart", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtServerShutDown(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    RaiseEvent DataChange(Sequence.TypeOfSequence.ServerShutDown, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtServerStart", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtTrigger(e As Sequence)
            Try
                If e.Numero <> SequenceTrigger Then
                    SequenceTrigger = e.Numero
                    LoadTriggers()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Trigger, e.Numero)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtTrigger", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtTriggerAdd(e As Sequence)
            Try
                If e.Numero <> SequenceTrigger Then
                    SequenceTrigger = e.Numero
                    LoadTriggers()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.TriggerAdd, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtTriggerAdd", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtTriggerChange(e As Sequence)
            Try
                If e.Numero <> SequenceTrigger Then
                    SequenceTrigger = e.Numero
                    ChangeTrigger(e.ID)
                    RaiseEvent DataChange(Sequence.TypeOfSequence.TriggerChange, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtTriggerChange", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtTriggerDelete(e As Sequence)
            Try
                If e.Numero <> SequenceTrigger Then
                    SequenceTrigger = e.Numero
                    LoadTriggers()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.TriggerDelete, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtTriggerDelete", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtUserAdd(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    LoadUsers()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.UserAdd, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtUserAdd", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtUser(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    LoadUsers()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.User, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtUser", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtUserChange(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    ChangeUser(e.ID)
                    RaiseEvent DataChange(Sequence.TypeOfSequence.UserChange, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtUserChange", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtUserDelete(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    LoadUsers()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.UserDelete, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtUserDelete", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtVariableAdd(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    LoadVariables()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.VariableAdd, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtVariableAdd", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtVariable(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    LoadVariables()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Variable, e.Numero)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtVariable", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtVariableChange(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    LoadVariables()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.VariableChange, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtVariableChange", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtVariableDelete(e As Sequence)
            Try
                If e.Numero <> SequenceServer Then
                    SequenceServer = e.Numero
                    LoadVariables()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.VariableDelete, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtVariableDelete", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtZone(e As Sequence)
            Try
                If e.Numero <> SequenceZone Then
                    SequenceZone = e.Numero
                    LoadZones()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.Zone, e.Numero)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtZone", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtZoneAdd(e As Sequence)
            Try
                If e.Numero <> SequenceZone Then
                    SequenceZone = e.Numero
                    LoadZones()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.ZoneAdd, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtZoneAdd", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtZoneChange(e As Sequence)
            Try
                If e.Numero <> SequenceZone Then
                    SequenceZone = e.Numero
                    ChangeZone(e.ID)
                    RaiseEvent DataChange(Sequence.TypeOfSequence.ZoneChange, e.ID)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtZoneChange", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

        Private Sub EvtZoneDelete(e As Sequence)
            Try
                If e.Numero <> SequenceZone Then
                    SequenceZone = e.Numero
                    LoadZones()
                    RaiseEvent DataChange(Sequence.TypeOfSequence.ZoneDelete, String.Empty)
                End If
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, "Erreur Event EvtZoneDelete", "Erreur" & ex.Message, " Event DeviceChanged")
            End Try
        End Sub

#End Region

    End Class

End Namespace
