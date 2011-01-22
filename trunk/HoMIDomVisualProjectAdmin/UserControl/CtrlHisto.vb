'Imports HoMIDom.HoMIDom

'Public Class CtrlHisto
'    Dim _Id As String
'    Dim _Enable As Boolean
'    Dim _DeviceId As String
'    Dim _PropertyDevice As String
'    Dim _listProperty As New ArrayList

'    Public Event Delete(ByVal sender As Object)

'    Public Property ID() As String
'        Get
'            Return _Id
'        End Get
'        Set(ByVal value As String)
'            _Id = value
'        End Set
'    End Property

'    Public Property Enable() As Boolean
'        Get
'            Return _Enable
'        End Get
'        Set(ByVal value As Boolean)
'            _Enable = value
'            cEnable.Checked = value
'        End Set
'    End Property

'    Public Property DeviceId() As String
'        Get
'            Return _DeviceId
'        End Get
'        Set(ByVal value As String)
'            _DeviceId = value
'            For i As Integer = 0 To CbDevice.Items.Count - 1
'                If CbDevice.Items(i).id = value Then
'                    CbDevice.SelectedIndex = i
'                    Exit For
'                End If
'            Next
'            CbProperty.Items.Clear()
'            Dim o As Object
'            o = CbDevice.SelectedItem
'            If o IsNot Nothing Then
'                _listProperty = ListProperty(o)
'                For i As Integer = 0 To _listProperty.Count - 1
'                    Dim a() As String
'                    a = Split(_listProperty.Item(i), "|")
'                    CbProperty.Items.Add(a(0))
'                Next
'            End If
'        End Set
'    End Property

'    Public Property PropertyDevice() As String
'        Get
'            Return CbProperty.Text
'        End Get
'        Set(ByVal value As String)
'            _PropertyDevice = value
'            CbProperty.Text = value
'        End Set
'    End Property

'    Private Sub CtrlHisto_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

'    End Sub

'    Private Sub CbDevice_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CbDevice.TextChanged
'        CbProperty.Items.Clear()
'        _listProperty = ListProperty(CbDevice.SelectedItem)
'        For i As Integer = 0 To _listProperty.Count - 1
'            Dim a() As String
'            a = Split(_listProperty.Item(i), "|")
'            CbProperty.Items.Add(a(0))
'        Next
'        _DeviceId = CbDevice.Items(CbDevice.SelectedIndex).id
'    End Sub

'    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelete.Click
'        RaiseEvent Delete(Me)
'    End Sub

'    Private Sub CbProperty_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CbProperty.TextChanged
'        _PropertyDevice = CbProperty.Text
'    End Sub

'    Private Sub cEnable_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cEnable.Click
'        _Enable = cEnable.Checked
'    End Sub

'    Public Sub New()

'        ' Cet appel est requis par le Concepteur Windows Form.
'        InitializeComponent()

'        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
'        'Chargement de la liste des drivers
'        For i As Integer = 0 To FRMMere.Obj.Devices.Count - 1
'            CbDevice.Items.Add(FRMMere.Obj.Devices.Item(i))
'        Next
'        CbDevice.DisplayMember = "Name"
'    End Sub
'End Class
