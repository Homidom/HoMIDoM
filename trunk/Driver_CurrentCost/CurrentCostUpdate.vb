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


Namespace CurrentCost
    Public Class CurrentCostUpdate
        Public ValidUpdate As Boolean = False

        Public DaysSinceBirth As Integer
        Public TimeStamp As DateTime
        Public MeterName As String
        Public MeterId As Integer
        Public MeterType As Integer
        Public MeterSoftwareVersion As String
        Public Channel1Watts As Integer
        Public Channel2Watts As Integer
        Public Channel3Watts As Integer
        Public Temperature As String
        Public kWattsHour02 As Single
        Public kWattsHour04 As Single
        Public kWattsHour06 As Single
        Public kWattsHour08 As Single
        Public kWattsHour10 As Single
        Public kWattsHour12 As Single
        Public kWattsHour14 As Single
        Public kWattsHour16 As Single
        Public kWattsHour18 As Single
        Public kWattsHour20 As Single
        Public kWattsHour22 As Single
        Public kWattsHour24 As Single
        Public kWattsHour26 As Single
        Public WattsDay01 As Integer
        Public WattsDay02 As Integer
        Public WattsDay03 As Integer
        Public WattsDay04 As Integer
        Public WattsDay05 As Integer
        Public WattsDay06 As Integer
        Public WattsDay07 As Integer
        Public WattsDay08 As Integer
        Public WattsDay09 As Integer
        Public WattsDay10 As Integer
        Public WattsDay11 As Integer
        Public WattsDay12 As Integer
        Public WattsDay13 As Integer
        Public WattsDay14 As Integer
        Public WattsDay15 As Integer
        Public WattsDay16 As Integer
        Public WattsDay17 As Integer
        Public WattsDay18 As Integer
        Public WattsDay19 As Integer
        Public WattsDay20 As Integer
        Public WattsDay21 As Integer
        Public WattsDay22 As Integer
        Public WattsDay23 As Integer
        Public WattsDay24 As Integer
        Public WattsDay25 As Integer
        Public WattsDay26 As Integer
        Public WattsDay27 As Integer
        Public WattsDay28 As Integer
        Public WattsDay29 As Integer
        Public WattsDay30 As Integer
        Public WattsDay31 As Integer
        Public WattsMonth01 As Integer
        Public WattsMonth02 As Integer
        Public WattsMonth03 As Integer
        Public WattsMonth04 As Integer
        Public WattsMonth05 As Integer
        Public WattsMonth06 As Integer
        Public WattsMonth07 As Integer
        Public WattsMonth08 As Integer
        Public WattsMonth09 As Integer
        Public WattsMonth10 As Integer
        Public WattsMonth11 As Integer
        Public WattsMonth12 As Integer
        Public WattsYear1 As Integer
        Public WattsYear2 As Integer
        Public WattsYear3 As Integer
        Public WattsYear4 As Integer


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
                reader.ReadStartElement("msg")

                reader.ReadStartElement("date")

                reader.ReadStartElement("dsb")
                DaysSinceBirth = reader.ReadContentAsInt()
                reader.ReadEndElement()

                reader.ReadStartElement("hr")
                Dim hrs As Integer = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("min")
                Dim mins As Integer = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("sec")
                Dim secs As Integer = reader.ReadContentAsInt()
                reader.ReadEndElement()
                If UseLocalTime Then
                    TimeStamp = DateTime.Now
                Else
                    TimeStamp = New DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hrs, mins, secs)
                End If
                reader.ReadEndElement()
                ' end of date
                reader.ReadStartElement("src")

                reader.ReadStartElement("name")
                MeterName = reader.ReadString()
                reader.ReadEndElement()

                reader.ReadStartElement("id")
                MeterId = reader.ReadContentAsInt()
                reader.ReadEndElement()

                reader.ReadStartElement("type")
                MeterType = reader.ReadContentAsInt()
                reader.ReadEndElement()

                reader.ReadStartElement("sver")
                MeterSoftwareVersion = reader.ReadContentAsString()
                reader.ReadEndElement()

                reader.ReadEndElement()
                ' end of src
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

                reader.ReadStartElement("hist")

                reader.ReadStartElement("hrs")

                reader.ReadStartElement("h02")
                kWattsHour02 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h04")
                kWattsHour04 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h06")
                kWattsHour06 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h08")
                kWattsHour08 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h10")
                kWattsHour10 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h12")
                kWattsHour12 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h14")
                kWattsHour14 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h16")
                kWattsHour16 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h18")
                kWattsHour18 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h20")
                kWattsHour20 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h22")
                kWattsHour22 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h24")
                kWattsHour24 = reader.ReadContentAsFloat()
                reader.ReadEndElement()
                reader.ReadStartElement("h26")
                kWattsHour26 = reader.ReadContentAsFloat()
                reader.ReadEndElement()

                reader.ReadEndElement()
                ' end of hrs
                reader.ReadStartElement("days")

                reader.ReadStartElement("d01")
                WattsDay01 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d02")
                WattsDay02 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d03")
                WattsDay03 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d04")
                WattsDay04 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d05")
                WattsDay05 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d06")
                WattsDay06 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d07")
                WattsDay07 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d08")
                WattsDay08 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d09")
                WattsDay09 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d10")
                WattsDay10 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d11")
                WattsDay11 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d12")
                WattsDay12 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d13")
                WattsDay13 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d14")
                WattsDay14 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d15")
                WattsDay15 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d16")
                WattsDay16 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d17")
                WattsDay17 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d18")
                WattsDay18 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d19")
                WattsDay19 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d20")
                WattsDay20 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d21")
                WattsDay21 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d22")
                WattsDay22 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d23")
                WattsDay23 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d24")
                WattsDay24 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d25")
                WattsDay25 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d26")
                WattsDay26 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d27")
                WattsDay27 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d28")
                WattsDay28 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d29")
                WattsDay29 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d30")
                WattsDay30 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("d31")
                WattsDay31 = reader.ReadContentAsInt()
                reader.ReadEndElement()

                reader.ReadEndElement()
                ' end of days
                reader.ReadStartElement("mths")

                reader.ReadStartElement("m01")
                WattsMonth01 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m02")
                WattsMonth02 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m03")
                WattsMonth03 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m04")
                WattsMonth04 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m05")
                WattsMonth05 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m06")
                WattsMonth06 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m07")
                WattsMonth07 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m08")
                WattsMonth08 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m09")
                WattsMonth09 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m10")
                WattsMonth10 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m11")
                WattsMonth11 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("m12")
                WattsMonth12 = reader.ReadContentAsInt()
                reader.ReadEndElement()

                reader.ReadEndElement()
                ' end of mths
                reader.ReadStartElement("yrs")

                reader.ReadStartElement("y1")
                WattsYear1 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("y2")
                WattsYear2 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("y3")
                WattsYear3 = reader.ReadContentAsInt()
                reader.ReadEndElement()
                reader.ReadStartElement("y4")
                WattsYear4 = reader.ReadContentAsInt()
                reader.ReadEndElement()

                reader.ReadEndElement()
                ' end of yrs

                reader.ReadEndElement()
                ' end of hist
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
End Namespace
