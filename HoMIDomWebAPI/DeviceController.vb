
Imports System.Web.Http
Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports System.Text.RegularExpressions
Imports System.Net.Http

Public Class DeviceController
    Inherits BaseHoMIdomController

    Public Function [Get]() As List(Of TemplateDevice)
        Return HoMIDomAPI.CurrentServer.GetAllDevices(Me.ServerKey)
    End Function

    Public Function [Get](id As String) As TemplateDevice
        Return HoMIDomAPI.CurrentServer.ReturnDeviceByID(Me.ServerKey, id)
    End Function

    <HttpGet()>
    Public Function ExecuteCommand(id As String, command As String) As Boolean
        Me.ExecuteCommandWithParams(id, command, Me.Request.RequestUri.ParseQueryString())
        ' HoMIDomAPI.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, id, New DeviceAction() With {.Nom = command})
        Return True
    End Function

    <HttpGet()>
    Public Function GetValue(id As String, field As String) As Object
        Return Me.GetField(Me.Get(id), field)
    End Function

    ' ''' <summary>
    ' ''' Permet d'executer une action avec des paramètres.
    ' ''' </summary>
    ' ''' <param name="id">GUID du composant (device)</param>
    ' ''' <param name="command">Nom de l'action a exécuter</param>
    ' ''' <param name="parameters">Listes de paramètres à passer à l'action. param1.nom:param1.value;param2.nom:param2.value</param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<HttpGet()>
    'Public Function ExecuteCommandWithParams(id As String, command As String, parameters As String) As Boolean

    '    Dim action As DeviceAction
    '    action = New DeviceAction() With {.Nom = command}

    '    If Not String.IsNullOrEmpty(parameters) Then

    '        ' récupération du composant
    '        Dim tplDevice As TemplateDevice = HoMIDomAPI.CurrentServer.ReturnDeviceByID(Me.ServerKey, id)
    '        If tplDevice Is Nothing Then Throw New ArgumentException("Composant non trouvé !", "id")

    '        ' recherche de l'action invoquée
    '        Dim devAction As DeviceAction = tplDevice.DeviceAction.Where(Function(t) t.Nom = command).FirstOrDefault()
    '        If devAction Is Nothing Then Throw New ArgumentException("Action non trouvée pour ce composant !", "command")

    '        ' extraction des paramètre reçu
    '        Dim params As String()
    '        params = Regex.Split(parameters, ";")

    '        ' assignation des paramètres à l'action
    '        For Each param As String In params

    '            Dim paramKV As String()
    '            paramKV = Regex.Split(param, ":")

    '            ' vérification de la présente d'une valeur pour la paramètre 
    '            If paramKV.Length > 1 Then
    '                Dim devActionParamater As DeviceAction.Parametre = devAction.Parametres.Where(Function(t) t.Nom = paramKV(0)).FirstOrDefault()
    '                If Not devActionParamater Is Nothing Then
    '                    Select Case devActionParamater.Type.ToLower()
    '                        Case "int32"
    '                            Dim intVal As Integer
    '                            If Integer.TryParse(paramKV(1), intVal) Then devActionParamater.Value = intVal
    '                    End Select

    '                    If Not devActionParamater.Value Is Nothing Then
    '                        action.Parametres.Add(devActionParamater)
    '                    End If
    '                End If
    '            End If

    '        Next

    '    End If

    '    HoMIDomAPI.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, id, action)
    '    Return True
    'End Function




    Private Function ExecuteCommandWithParams(id As String, command As String, parameters As System.Collections.Specialized.NameValueCollection) As Boolean
        Try
            Dim action As DeviceAction = New DeviceAction() With {.Nom = command}

            If Not parameters Is Nothing And parameters.Count > 0 Then

                ' récupération du composant
                Dim tplDevice As TemplateDevice = HoMIDomAPI.CurrentServer.ReturnDeviceByID(Me.ServerKey, id)
                If tplDevice Is Nothing Then Throw New ArgumentException("Composant non trouvé !", "id")

                ' recherche de l'action invoquée
                Dim devAction As DeviceAction = tplDevice.DeviceAction.Where(Function(t) t.Nom = command).FirstOrDefault()
                If devAction Is Nothing Then Throw New ArgumentException("Action non trouvée pour ce composant !", "command")

                For Each pKey In parameters.AllKeys
                    Dim parameterKey = pKey 'Utile pour l'expression lambda plus bas
                    Dim parameterValue = parameters(parameterKey)
                    ' vérification de la présente d'une valeur pour la paramètre 
                    If Not String.IsNullOrEmpty(parameterValue) Then
                        Dim devActionParameter As DeviceAction.Parametre = devAction.Parametres.Where(Function(t) t.Nom = parameterKey).FirstOrDefault()
                        If Not devActionParameter Is Nothing Then
                            Select Case devActionParameter.Type.ToLower()
                                Case "int32"
                                    Dim intVal As Integer
                                    If Integer.TryParse(parameterValue, intVal) Then devActionParameter.Value = intVal

                                Case "system.string"
                                    devActionParameter.Value = parameterValue.ToString
                                Case Else
                                    Dim intVal As Integer
                                    If Integer.TryParse(parameterValue, intVal) Then devActionParameter.Value = intVal
                                    Dim intdbl As Double
                                    If Double.TryParse(parameterValue, intdbl) Then devActionParameter.Value = intdbl
                                    Dim intlng As Long
                                    If Long.TryParse(parameterValue, intlng) Then devActionParameter.Value = intlng
                            End Select

                            If Not devActionParameter.Value Is Nothing Then
                                action.Parametres.Add(devActionParameter)
                            End If
                        End If
                    End If

                Next

            End If

            HoMIDomAPI.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, id, action)
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

End Class
