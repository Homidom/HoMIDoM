Imports System.Net
Imports System.Xml
Imports System.Xml.XmlReader
Imports System.IO
Imports System.ServiceModel.Syndication

Public Class uRSS
    Dim _Uri As String
    Private feedItems As ArrayList

    Public Property URIRss As String
        Get
            Return _Uri
        End Get
        Set(ByVal value As String)
            _Uri = value
            RefreshChannel()
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
        If _Uri <> "" Then
            Dim channel As New RSSChannel(_Uri)

            feedItems = channel.GetChannelItems()
            For i As Integer = 0 To feedItems.Count - 1
                Lstb.Items.Add(feedItems.Item(i).Title)
            Next
        End If
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
        GetChannelInfo()
    End Sub

    Private Function GetXMLDoc(ByVal node As String) As XmlNodeList
        Dim tempNodeList As System.Xml.XmlNodeList = Nothing

        Dim request As WebRequest = WebRequest.Create(Me.FeedURL)
        Dim response As WebResponse = request.GetResponse()
        Dim rssStream As Stream = response.GetResponseStream()
        Dim rssDoc As XmlDocument = New XmlDocument()
        rssDoc.Load(rssStream)
        tempNodeList = rssDoc.SelectNodes(node)

        Return tempNodeList
    End Function

    Private Sub GetChannelInfo()
        Dim rss As XmlNodeList = GetXMLDoc("rss/channel")
        Title = rss(0).SelectSingleNode("title").InnerText
        Link = rss(0).SelectSingleNode("link").InnerText
        Description = rss(0).SelectSingleNode("description").InnerText
    End Sub

    Public Function GetChannelItems() As ArrayList
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
    End Function
#End Region
End Class