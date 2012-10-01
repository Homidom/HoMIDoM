Imports System.Collections.Generic
Imports System.Text
Imports System.Xml

'
' * Represents the data contained in a single update from a CurrentCost meter
' *
' * It's a Java object representation of the XML - as described here:
' * http://cumbers.wordpress.com/2008/05/07/breakdown-of-currentcost-xml-output/
' *
' * Class includes a constructor to create an update object from a line of 
' * CurrentCost XML.
' * 
' *  Dale Lane (http://dalelane.co.uk/blog)
' 


Public Class CurrentCostUpdate
    Public ValidUpdate As Boolean = False

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
        Dim settings As New XmlReaderSettings()
        settings.ConformanceLevel = ConformanceLevel.Fragment
        settings.IgnoreComments = True
        settings.IgnoreWhitespace = True

        Dim reader As XmlReader = XmlReader.Create(New System.IO.StringReader(xmloutput))

        Try
            reader.Read()

            reader.ReadStartElement("ch1")

            reader.ReadStartElement("watts")
            Channel1Watts = reader.ReadContentAsInt()
            reader.ReadEndElement()

            reader.ReadEndElement()
            ' end of ch1
            reader.ReadStartElement("ch2")

            reader.ReadStartElement("watts")
            Channel2Watts = reader.ReadContentAsInt()
            reader.ReadEndElement()

            reader.ReadEndElement()
            ' end of ch2
            reader.ReadStartElement("ch3")

            reader.ReadStartElement("watts")
            Channel3Watts = reader.ReadContentAsInt()
            reader.ReadEndElement()

            reader.ReadEndElement()
            ' end of ch3

            reader.ReadStartElement("tmpr")
            Temperature = reader.ReadContentAsString()
            reader.ReadEndElement()

            reader.ReadStartElement("time")
            Time = reader.ReadContentAsString()
            reader.ReadEndElement()

           
            ValidUpdate = True
        Catch exc As XmlException

            ' partial updates from CurrentCost meters are not uncommon 

            '  so this is unlikely to be a severe error                

            ' the normal solution is just to try again with another    

            '  line of data from the CurrentCost meter                 

            ' however, it might be good to count how many times we end 

            '  up in here, and put a limit on how many times we try?   

            ValidUpdate = False
        Finally
            reader.Close()
        End Try
    End Sub
End Class

