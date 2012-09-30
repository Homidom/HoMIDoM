Imports System.Text


'
' * Parses data from a CurrentCost meter
' *  
' *  Note that this is provided to help get you started, but it lacks careful 
' *   error-handling or proper documentation.
' *  
' *  If you have any questions about it, or even just find it useful, please do 
' *   let me know - dale.lane@gmail.com
' *  
' *  
' *   Dale Lane (http://dalelane.co.uk/blog)
' 


Namespace CurrentCost
    Class CurrentCostProgram
        Private Shared Sub Main(ByVal args As String())
            ' define the connection to the CurrentCost meter 

            Dim meter As New SerialPortAccess()
            meter.ComPort = "COM21"

            ' get a single update from the CurrentCost meter 

            Dim ccUpdate As CurrentCostUpdate = meter.GetSingleUpdate()

            ' create somewhere to store the CurrentCost data                

            ' this contains Dictionary objects that, in a real application, 

            '  you will want to persist between running the app, rather than

            '  creating an empty collection like this every time            

            Dim historyStore As New CurrentCostHistory()

            ' store the update in the history collection             

            historyStore.UpdateData(ccUpdate)

            ' print out the contents to check it worked 

            Console.WriteLine(historyStore)
        End Sub
    End Class
End Namespace
