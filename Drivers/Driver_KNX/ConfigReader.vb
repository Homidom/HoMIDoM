Public Class ConfigReader

    Private _configFilePath As String
    Public Property ConfigFilePath() As String
        Get
            Return _configFilePath
        End Get
        Set(ByVal value As String)
            _configFilePath = value

            Me.LoadConfigFile(_configFilePath)
        End Set
    End Property

    Private _hmdObjects As List(Of HMDObject)
    Public Property HMDObjects() As List(Of HMDObject)
        Get
            Return _hmdObjects
        End Get
        Set(ByVal value As List(Of HMDObject))
            _hmdObjects = value
        End Set
    End Property


    Public Sub New()
        _hmdObjects = New List(Of HMDObject)
    End Sub

    Private Sub LoadConfigFile(configFilePath As String)

        Dim doc As XDocument
        doc = XDocument.Load(configFilePath)

        For Each xHmdObj As XElement In doc.Descendants("HMDObject")

            Dim hmdObj As HMDObject
            hmdObj = New HMDObject()

            hmdObj.Id = xHmdObj.Attribute("id").Value
            hmdObj.Name = xHmdObj.Attribute("name").Value
            hmdObj.Type = xHmdObj.Attribute("type").Value

            For Each xRead As XElement In xHmdObj.Descendants("read")
                hmdObj.ReadObjects.Add(xRead.Attribute("id").Value)
            Next

            For Each xWrite As XElement In xHmdObj.Descendants("write")
                hmdObj.WriteObjects.Add(xWrite.Attribute("id").Value)
            Next

            _hmdObjects.Add(hmdObj)

        Next
    End Sub

End Class
