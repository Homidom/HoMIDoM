Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports System.Xml.XPath


Public Class CurrentCostUpdate
    Public ValidUpdate As Boolean = False
    Public Historique As Boolean = False

    Public Time As String
    Public Channel1Watts As Integer
    Public Channel2Watts As Integer
    Public Channel3Watts As Integer
    Public Temperature As String


    Public Sub New()
        ' invalid by default
        ValidUpdate = False
    End Sub

    Public Sub New(ByVal xmloutput As String)
        Me.New(xmloutput, False)
    End Sub

    Public Sub New(ByVal xmloutput As String, ByVal UseLocalTime As Boolean)
        Try
            Dim doc As XPathDocument
            Dim xmlNI As XPathNodeIterator
            Dim xmlNav As XPathNavigator
            Dim myStringReader As XmlReader = XmlReader.Create(New System.IO.StringReader(xmloutput))

            doc = New XPathDocument(myStringReader)
            xmlNav = doc.CreateNavigator()

            'Test si historique on ne prend pas en compte les valeurs
            xmlNI = xmlNav.Select("/msg/hist")
            If xmlNI.Count >= 1 Then
                Historique = True
            Else
                ' time
                xmlNI = xmlNav.Select("/msg/time")
                If xmlNI.Count = 1 Then
                    xmlNI.MoveNext()
                    Time = xmlNI.Current.Value
                End If

                ' tmpr
                xmlNI = xmlNav.Select("/msg/tmpr")
                If xmlNI.Count = 1 Then
                    xmlNI.MoveNext()
                    Temperature = xmlNI.Current.Value
                End If

                ' ch1
                xmlNI = xmlNav.Select("/msg/ch1/watts")
                If xmlNI.Count = 1 Then
                    xmlNI.MoveNext()
                    Channel1Watts = xmlNI.Current.Value
                End If

                ' ch2
                xmlNI = xmlNav.Select("/msg/ch2/watts")
                If xmlNI.Count = 1 Then
                    xmlNI.MoveNext()
                    Channel2Watts = xmlNI.Current.Value
                End If

                ' ch3
                xmlNI = xmlNav.Select("/msg/ch3/watts")
                If xmlNI.Count = 1 Then
                    xmlNI.MoveNext()
                    Channel3Watts = xmlNI.Current.Value
                End If
                ValidUpdate = True
            End If
            
        Catch exc As XmlException
            ValidUpdate = False
        End Try
    End Sub
End Class

