Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports Microsoft.VisualBasic.Strings
Imports System.Net.Sockets
Imports System.Threading
Imports System.Globalization
Imports com

Public Class Driver_onewire
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "6DAA2DE2-0545-11E1-B580-3CCD4824019B"
    Dim _Nom As String = "1-Wire"
    Dim _Enable As String = False
    Dim _Description As String = "Adaptateur 1-wire USB / COM"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "COM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "USB1"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "{DS9490}"
    Dim _Version As String = "1.0"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String
#End Region

#Region "Variables Internes"
    Dim wir_adapter As com.dalsemi.onewire.adapter.DSPortAdapter
    Public adapter_present = 0 '=1 si adapteur présent sinon =0
#End Region

#Region "Déclaration"

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
    Public Property Port_TCP() As Object Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As Object)
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

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            If Not _IsConnect Then
        'Initialisation de la cle USB 1-WIRE
                '_Modele = "{DS9490B}"
                ' _Com = "USB2"
                Try
                    If (_Modele = "" Or _Modele = " ") And (_Com = "" Or _Com = " ") Then
                        wir_adapter = dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter
                    Else
                        wir_adapter = dalsemi.onewire.OneWireAccessProvider.getAdapter(_Modele, _Com)
                    End If
                    adapter_present = 1
                    _IsConnect = True
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "1-Wire Start", "Adapter " & wir_adapter.getAdapterName & " " & wir_adapter.getPortName)
                Catch ex As Exception
                    adapter_present = 0
                    _IsConnect = False
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "1-Wire Start", "ERR: Initialisation : " & ex.ToString & " - " & _Modele & " - " & _Com)

                End Try
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Start", "Driver déjà connecté")
            End If
        Catch ex As Exception
            _IsConnect = False
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            If _IsConnect Then
                _IsConnect = False
               wir_adapter.freePort()
                adapter_present = 0
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "1-Wire Stop", "Port " & _Com & " fermé")
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "1-Wire Stop", "Port " & _Com & " est déjà fermé")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Stop", ex.Message)
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
            If _Enable = False Then Exit Sub
            If _IsConnect = False Then Exit Sub

            If Objet IsNot Nothing Then
                Select Case Objet.Type
                    Case "TEMPERATURE"
                        Dim retour As Integer = temp_get_save(Objet.Adresse1)
                        If retour <> 9999 Then Objet.Value = retour
                    Case "SWITCH"
                        Dim retour As Integer = switch_get(Objet.Adresse1)
                        If retour <> 9999 Then
                            If retour = 1 Then
                                Objet.Value = True
                            Else
                                Objet.Value = False
                            End If
                        End If
                    Case "CONTACT"
                        Dim retour As Integer = switch_get(Objet.Adresse1)
                        If retour <> 9999 Then
                            If retour = 1 Then
                                Objet.Value = True
                            Else
                                Objet.Value = False
                            End If
                        End If
                    Case "DETECTEUR"
                        Dim retour As Integer = switch_get(Objet.Adresse1)
                        If retour <> 9999 Then
                            If retour = 1 Then
                                Objet.Value = True
                            Else
                                Objet.Value = False
                            End If
                        End If
                    Case "GENERIQUEVALUE"
                        Dim retour As Integer = switch_get(Objet.Adresse1)
                        If retour <> 9999 Then
                            Objet.Value = retour
                        End If
                End Select
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Read", ex.Message)
        End Try
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
            If _IsConnect = False Then Exit Sub
            Select Case UCase(Command)
                Case "ON"

                Case "OFF"
                Case "DIM"
                Case "OUVERTURE"
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire NewDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick()

    End Sub

#End Region

#Region "Fonctions propres au driver"
    Public Function temp_get_save(ByVal adresse As String) As String

        ' Renvoi la temperature du capteur X
        Dim resolution As Double = 0.1 'resolution de la temperature : 0.1 ou 0.5
        Dim retour As String = ""
        Dim state As Object
        Dim tc As com.dalsemi.onewire.container.TemperatureContainer
        Dim owd As com.dalsemi.onewire.container.OneWireContainer

        If _IsConnect = True Then
            'demande l'acces exclusif au reseau
            Try
                wir_adapter.beginExclusive(False)
                owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant

                If owd.isPresent() Then
                    Try
                        tc = DirectCast(owd, com.dalsemi.onewire.container.TemperatureContainer) 'creer la connexion
                        state = tc.readDevice 'lit le capteur
                        tc.setTemperatureResolution(resolution, state) 'modifie la resolution à 0.1 degré (0.5 par défaut)
                        tc.doTemperatureConvert(state) 'converti la valeur obtenu en temperature
                        state = tc.readDevice 'lit la conversion
                        retour = Math.Round(tc.getTemperature(state), 1)
                    Catch ex As Exception
                        retour = 9999
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire GetTemp", ex.ToString)
                    End Try
                Else
                    retour = 9999
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire GetTemp", "Capteur à l'adresse " & adresse & " Non présent")
                End If

                wir_adapter.endExclusive()
            Catch ex As Exception
                retour = 9999
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire GetTemp", ex.ToString)
            End Try
        Else
            retour = 9999
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire GetTemp", "Erreur Adaptateur non présent")
        End If
        Return retour
    End Function

    Public Function temp_get(ByVal adresse As String, ByVal resolution As Double) As String
        ' Renvoi la temperature du capteur X
        'resolution = Domos.WIR_res --> resolution de la temperature : 0.1 ou 0.5
        Dim retour As String = ""
        Dim state As Object
        Dim result As Boolean = False
        Dim tc As com.dalsemi.onewire.container.TemperatureContainer
        Dim owd As com.dalsemi.onewire.container.OneWireContainer

        If adapter_present Then
            'demande l'acces exclusif au reseau
            Try
                result = wir_adapter.beginExclusive(False)
                If result Then
                    'wir_adapter.reset()
                    owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant
                    tc = DirectCast(owd, com.dalsemi.onewire.container.TemperatureContainer) 'creer la connexion
                    If owd.isPresent() Then
                        state = tc.readDevice() 'lit le capteur
                        tc.setTemperatureResolution(resolution, state) 'modifie la resolution à 0.1 degré (0.5 par défaut)
                        tc.doTemperatureConvert(state)
                        state = tc.readDevice 'lit la conversion
                        retour = Math.Round(tc.getTemperature(state), 1)
                    Else
                        retour = "ERR: temp_get : Capteur non présent"
                    End If
                    wir_adapter.endExclusive()
                Else
                    retour = "ERR: temp_get : Acces Exclusif refusé"
                End If
            Catch ex As Exception
                retour = "ERR: temp_get : " & ex.ToString
            End Try
        Else
            retour = "ERR: temp_get : Adaptateur non présent"
        End If
        Return retour
    End Function

    Public Function switchs_get(ByVal adresse As String) As String
        ' Récupere l'etat et activité d'un multiswitch
        Dim retour As String = ""
        Dim state As Object
        Dim owd As com.dalsemi.onewire.container.OneWireContainer
        Dim tc As com.dalsemi.onewire.container.SwitchContainer
        Dim switch_activity, switch_state
        Try
            If adapter_present Then
                If wir_adapter.isPresent(adresse) Then
                    wir_adapter.beginExclusive(True)
                    owd = wir_adapter.getDeviceContainer(adresse)
                    tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                    state = tc.readDevice()
                    Dim number_of_switches = tc.getNumberChannels(state)
                    For i = 0 To (number_of_switches - 1)
                        switch_state = tc.getLatchState(i, state) 'recup l'etat du switch
                        switch_activity = tc.getSensedActivity(i, state) 'recup l'activité du switch
                        If i <> 0 Then retour = retour & "-"
                        If switch_state Then retour = retour & "1" Else retour = retour & "0"
                        If switch_activity Then retour = retour & "1" Else retour = retour & "0"
                    Next
                    wir_adapter.endExclusive()
                Else
                    retour = "ERR: switchs_get : Capteur non présent"
                End If
            Else
                retour = "ERR: switchs_get : Adaptateur non présent"
            End If
        Catch ex As Exception
            retour = "ERR: switchs_get : " & ex.ToString
        End Try
        Return retour
    End Function

    Public Function switch_switchstate(ByVal adresse As String) As String
        ' Change l'etat d'un switch et renvoi le nouveau etat (ex :0 ==> Off)
        Dim retour As String = ""
        Dim state As Object
        Dim owd As com.dalsemi.onewire.container.OneWireContainer
        Dim tc As com.dalsemi.onewire.container.SwitchContainer
        Dim switch_state
        Try
            If adapter_present Then
                If wir_adapter.isPresent(adresse) Then
                    wir_adapter.beginExclusive(True)
                    owd = wir_adapter.getDeviceContainer(adresse)
                    tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                    state = tc.readDevice()
                    switch_state = tc.getLatchState(0, state)
                    If switch_state Then retour = "0" Else retour = "1"
                    tc.setLatchState(0, Not switch_state, False, state)
                    tc.writeDevice(state)
                    wir_adapter.endExclusive()
                Else
                    retour = "ERR: switch_switchstate : Capteur non présent"
                End If
            Else
                retour = "ERR: switch_switchstate : Adaptateur non présent"
            End If
        Catch ex As Exception
            retour = "ERR: switch_switchstate : " & ex.ToString
        End Try
        Return retour
    End Function

    Public Function switchs_switchstate(ByVal adresse As String, ByVal channel As Integer) As String
        ' Change l'etat d'un switch
        Dim retour As String = ""
        Dim state As Object
        Dim owd As com.dalsemi.onewire.container.OneWireContainer
        Dim tc As com.dalsemi.onewire.container.SwitchContainer
        Dim switch_state
        Try
            If adapter_present Then
                If wir_adapter.isPresent(adresse) Then
                    wir_adapter.beginExclusive(True)
                    owd = wir_adapter.getDeviceContainer(adresse)
                    tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                    state = tc.readDevice()
                    Dim number_of_switches = tc.getNumberChannels(state)
                    For i = 0 To (number_of_switches - 1)
                        If i = channel Then
                            switch_state = tc.getLatchState(i, state)
                            If i <> 0 Then retour = retour & "-"
                            If switch_state Then retour = retour & "0" Else retour = retour & "1"
                            tc.setLatchState(i, Not switch_state, False, state)
                        End If
                    Next
                    tc.writeDevice(state)
                    wir_adapter.endExclusive()
                Else
                    retour = "ERR: switchs_switchstate : Capteur non présent"
                End If
            Else
                retour = "ERR: switchs_switchstate : Adaptateur non présent"
            End If
        Catch ex As Exception
            retour = "ERR: switchs_switchstate : " & ex.ToString
        End Try
        Return retour
    End Function

    Public Function switch_get(ByVal adresse As String) As String
        ' Renvoie l'etat du switch 0=fermé, 1=ouvert, 2=fermé mais ouvert entre temps
        Dim retour As String = ""
        Dim state As Object
        Dim owd As com.dalsemi.onewire.container.OneWireContainer12
        Dim tc As com.dalsemi.onewire.container.SwitchContainer
        Dim switch_state, switch_activity
        Try
            If adapter_present Then
                If wir_adapter.isPresent(adresse) Then
                    wir_adapter.beginExclusive(True) 'demande l'acces exclusif au reseau
                    owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant
                    tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer) 'creer la connexion
                    state = tc.readDevice()  'lit les infos du composant
                    switch_state = tc.getLatchState(0, state) 'recup l'etat du switch
                    switch_activity = tc.getSensedActivity(0, state) 'recup l'activité du switch
                    If switch_state Then retour = "1" Else 
                    If switch_activity Then retour = "2" Else retour = "0"
                    tc.clearActivity()
                    tc.readDevice()
                    wir_adapter.endExclusive() 'rend l'accés au reseau
                Else
                    retour = 9999
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switch_get", "Capteur à l'adresse " & adresse & " Non présent")
                End If
            Else
                retour = 9999
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switch_get", "Adaptateur non présent")
            End If
        Catch ex As Exception
            retour = 9999
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switch_get", "Erreur: " & ex.ToString)
        End Try
        Return retour
    End Function

    Public Function switch_setstate(ByVal adresse As String, ByVal etat As Boolean) As String
        ' Change l'etat d'un switch et renvoi le nouvel etat
        Dim retour As String = ""
        Dim state As Object
        Dim owd As com.dalsemi.onewire.container.OneWireContainer
        Dim tc As com.dalsemi.onewire.container.SwitchContainer
        Dim switch_state
        Try
            If adapter_present Then
                If wir_adapter.isPresent(adresse) Then
                    wir_adapter.beginExclusive(True)
                    owd = wir_adapter.getDeviceContainer(adresse)
                    tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                    state = tc.readDevice()
                    switch_state = tc.getLatchState(0, state)
                    If etat Then retour = "1" Else retour = "0"
                    tc.setLatchState(0, etat, False, state)
                    tc.writeDevice(state)
                    wir_adapter.endExclusive()
                Else
                    retour = "ERR: switch_setstate : Capteur non présent"
                End If
            Else
                retour = "ERR: switch_setstate : Adaptateur non présent"
            End If
        Catch ex As Exception
            retour = "ERR: switch_setstate : " & ex.ToString
        End Try
        Return retour
    End Function

    Public Function switchs_setstate(ByVal adresse As String, ByVal channel As Integer, ByVal etat As Boolean) As String
        ' Change l'etat du channel x du switch Y et renvoi le nouvel etat
        Dim retour As String = ""
        Dim state As Object
        Dim owd As com.dalsemi.onewire.container.OneWireContainer
        Dim tc As com.dalsemi.onewire.container.SwitchContainer
        Dim switch_state
        Try
            If adapter_present Then
                If wir_adapter.isPresent(adresse) Then
                    wir_adapter.beginExclusive(True)
                    owd = wir_adapter.getDeviceContainer(adresse)
                    tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                    state = tc.readDevice()
                    Dim number_of_switches = tc.getNumberChannels(state)
                    For i = 0 To (number_of_switches - 1)
                        If i = channel Then
                            switch_state = tc.getLatchState(i, state)
                            If etat Then retour = "1" Else retour = "0"
                            tc.setLatchState(i, etat, False, state)
                        End If
                    Next
                    tc.writeDevice(state)
                    wir_adapter.endExclusive()
                Else
                    retour = "ERR: switchs_setstate : Capteur non présent"
                End If
            Else
                retour = "ERR: switchs_setstate : Adaptateur non présent"
            End If
        Catch ex As Exception
            retour = "ERR: switchs_setstate : " & ex.ToString
        End Try
        Return retour
    End Function

    Public Function switch_clearactivity(ByVal adresse As String) As String
        ' Récupere l'etat et activité d'un switch
        Dim retour As String = ""
        Dim state As Object
        Dim owd As com.dalsemi.onewire.container.OneWireContainer
        Dim tc As com.dalsemi.onewire.container.SwitchContainer
        Dim switch_activity, switch_state
        Try
            If adapter_present Then
                If wir_adapter.isPresent(adresse) Then
                    wir_adapter.beginExclusive(True)
                    owd = wir_adapter.getDeviceContainer(adresse)
                    tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                    state = tc.readDevice()
                    Dim number_of_switches = tc.getNumberChannels(state)
                    For i = 0 To (number_of_switches - 1)
                        switch_state = tc.getLatchState(i, state)
                        switch_activity = tc.getSensedActivity(i, state)
                        If Not switch_state Then
                            retour = "Switch " & i & " => Activité " & switch_activity & " à False"
                            'retour = "0"
                            tc.clearActivity()
                            tc.readDevice()
                        Else
                            retour = "1"
                        End If
                    Next
                    wir_adapter.endExclusive()
                Else
                    retour = "ERR: switch_clearactivity : Capteur non présent"
                End If
            Else
                retour = "ERR: switch_clearactivity : Adaptateur non présent"
            End If
        Catch ex As Exception
            retour = "ERR: switch_clearactivity : " & ex.ToString
        End Try
        Return retour
    End Function

    Public Function counter(ByVal adresse As String, ByVal countera As Boolean) As String
        'recupere la valeur du compteur A (true) ou B (false)
        Dim CounterContainer As com.dalsemi.onewire.container.OneWireContainer1D
        Dim owd As com.dalsemi.onewire.container.OneWireContainer
        Dim retour As String = ""
        Dim counterstate As Long
        Try
            If adapter_present Then
                wir_adapter.beginExclusive(True)
                'owd = wir_adapter.getDeviceContainer(adresse)
                'CounterContainer = New com.dalsemi.onewire.container.OneWireContainer1D(wir_adapter, adresse)
                'If countera Then
                '    counterstate = CounterContainer.readCounter(14)
                'Else
                '    counterstate = CounterContainer.readCounter(15)
                'End If
                owd = wir_adapter.getDeviceContainer(adresse)
                CounterContainer = DirectCast(owd, com.dalsemi.onewire.container.OneWireContainer1D)
                If countera Then
                    counterstate = CounterContainer.readCounter(14)
                Else
                    counterstate = CounterContainer.readCounter(15)
                End If
                wir_adapter.endExclusive()
                retour = counterstate.ToString
            Else
                retour = "ERR: counter : Adaptateur non présent"
            End If
        Catch ex As Exception
            retour = "ERR: counter : " & ex.ToString
        End Try
        Return retour
    End Function

#End Region

    Public Sub New()
        Try
            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
            _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire New", ex.Message)
        End Try
    End Sub


End Class

