Imports System.Net
Imports System.Xml
Imports System.Xml.XmlReader
Imports System.IO
Imports System.ServiceModel.Syndication
Imports System.Threading

Public Class uRSS
    Dim _Uri As String
    Private feedItems As ArrayList = Nothing

    Public Property URIRss As String
        Get
            Return _Uri
        End Get
        Set(ByVal value As String)
            _Uri = value
            If String.IsNullOrEmpty(_Uri) = False Then RefreshChannel()
        End Set
    End Property

    Private Sub lbItems_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Lstb.SelectionChanged
        If Lstb.Items.Count > 0 Then
            Dim currentItem As RSSItem

            currentItem = feedItems(Lstb.SelectedIndex)
            WebBrowser1.NavigateToString(currentItem.Description)
        End If
    End Sub

    Private Sub RefreshChannel()
        If My.Computer.Network.IsAvailable = False Then
            Exit Sub
        End If

            Dim channel As New RSSChannel(_Uri)

            feedItems = channel.GetChannelItems()
            For i As Integer = 0 To feedItems.Count - 1
                Lstb.Items.Add(feedItems.Item(i).Title)
            Next

        channel = Nothing
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

    End Sub

    Private Sub uRSS_Unloaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Unloaded
        Lstb.Items.Clear()
    End Sub
End Class

Public Class RSSItem
    Private m_Title As String
    Private m_Link As String
    Private m_Description As String

#Region "Properties"
    Public Property Title() As String
        Get
            Return m_Title
        End Get
        Set(ByVal value As String)
            m_Title = value
        End Set
    End Property

    Public Property Link() As String
        Get
            Return m_Link
        End Get
        Set(ByVal value As String)
            m_Link = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return m_Description
        End Get
        Set(ByVal value As String)
            m_Description = value
        End Set
    End Property
#End Region

    Public Sub New()
        Title = ""
        Link = ""
        Description = ""
    End Sub
End Class

Public Class RSSChannel
    Private m_FeedURL As String
    Private m_Title As String
    Private m_Link As String
    Private m_Description As String

#Region "Properties"
    Public Property FeedURL() As String
        Get
            Return m_FeedURL
        End Get
        Set(ByVal value As String)
            m_FeedURL = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return m_Title
        End Get
        Set(ByVal value As String)
            m_Title = value
        End Set
    End Property

    Public Property Link() As String
        Get
            Return m_Link
        End Get
        Set(ByVal value As String)
            m_Link = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return m_Description
        End Get
        Set(ByVal value As String)
            m_Description = value
        End Set
    End Property
#End Region

#Region "Methods"
    Public Sub New(ByVal url As String)
        FeedURL = url
        Title = ""
        Link = ""
        Description = ""
        Dim x As New Thread(AddressOf GetChannelInfo)
        x.Start()
    End Sub

    Private Function GetXMLDoc(ByVal node As String) As XmlNodeList
        Try

            Dim tempNodeList As System.Xml.XmlNodeList = Nothing

            Dim request As WebRequest = WebRequest.Create(Me.FeedURL)
            Dim response As WebResponse = request.GetResponse()
            Dim rssStream As Stream = response.GetResponseStream()
            Dim rssDoc As XmlDocument = New XmlDocument()
            rssDoc.Load(rssStream)
            tempNodeList = rssDoc.SelectNodes(node)

            Return tempNodeList

            tempNodeList = Nothing
            request = Nothing
            response = Nothing
            rssStream = Nothing
            rssDoc = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur GetXMLDoc: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            Return Nothing
        End Try
    End Function

    Private Sub GetChannelInfo()
        Try

            Dim rss As XmlNodeList = GetXMLDoc("rss/channel")
            Title = rss(0).SelectSingleNode("title").InnerText
            Link = rss(0).SelectSingleNode("link").InnerText
            Description = rss(0).SelectSingleNode("description").InnerText

            rss = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur GetChannelInfo: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Function GetChannelItems() As ArrayList
        Try
            Dim tempArrayList As New ArrayList

            Dim rssItems As XmlNodeList = GetXMLDoc("rss/channel/item")
            Dim item As XmlNode
            For Each item In rssItems
                Dim newItem As New RSSItem
                With newItem
                    .Title = item.SelectSingleNode("title").InnerText
                    .Link = item.SelectSingleNode("link").InnerText
                    .Description = item.SelectSingleNode("description").InnerText
                End With
                tempArrayList.Add(newItem)
            Next

            Return tempArrayList

            tempArrayList = Nothing
            rssItems = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur GetChannelItems: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            Return Nothing
        End Try
    End Function
#End Region
End Class