
Imports System.Web.Http
Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports System.Text.RegularExpressions

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
        HoMIDomAPI.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, id, New DeviceAction() With {.Nom = command})
        Return True
    End Function

    <HttpGet()>
    Public Function GetValue(id As String, field As String) As Object
        Return Me.GetField(Me.Get(id), field)
    End Function

    ''' <summary>
    ''' Permet d'executer une action avec des paramètres.
    ''' </summary>
    ''' <param name="id">GUID du composant (device)</param>
    ''' <param name="command">Nom de l'action a exécuter</param>
    ''' <param name="parameters">Listes de paramètres à passer à l'action. param1.nom:param1.value;param2.nom:param2.value</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    Public Function ExecuteCommandWithParams(id As String, command As String, parameters As String) As Boolean

        Dim action As DeviceAction
        action = New DeviceAction() With {.Nom = command}

        If Not String.IsNullOrEmpty(parameters) Then

            ' récupération du composant
            Dim tplDevice As TemplateDevice = HoMIDomAPI.CurrentServer.ReturnDeviceByID(Me.ServerKey, id)
            If tplDevice Is Nothing Then Throw New ArgumentException("Composant non trouvé !", "id")

            ' recherche de l'action invoquée
            Dim devAction As DeviceAction = tplDevice.DeviceAction.Where(Function(t) t.Nom = command).FirstOrDefault()
            If devAction Is Nothing Then Throw New ArgumentException("Action non trouvée pour ce composant !", "command")

            ' extraction des paramètre reçu
            Dim params As String()
            params = Regex.Split(parameters, ";")

            ' assignation des paramètres à l'action
            For Each param As String In params

                Dim paramKV As String()
                paramKV = Regex.Split(param, ":")

                ' vérification de la présente d'une valeur pour la paramètre 
                If paramKV.Length > 1 Then
                    Dim devActionParamater As DeviceAction.Parametre = devAction.Parametres.Where(Function(t) t.Nom = paramKV(0)).FirstOrDefault()
                    If Not devActionParamater Is Nothing Then
                        Select Case devActionParamater.Type.ToLower()
                            Case "int32"
                                Dim intVal As Integer
                                If Integer.TryParse(paramKV(1), intVal) Then devActionParamater.Value = intVal
                        End Select

                        If Not devActionParamater.Value Is Nothing Then
                            action.Parametres.Add(devActionParamater)
                        End If
                    End If
                End If

            Next

        End If

        HoMIDomAPI.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, id, action)
        Return True
    End Function


End Class
