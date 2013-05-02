Imports System.Text.RegularExpressions
Public Class KNXLogMessage

    Private _message As String
    Public Property Message() As String
        Get
            Return _message
        End Get
        Set(ByVal value As String)
            _message = value

            'Lecture : 
            ' LPDU: BC 00 00 0D 0B F1 00 00 B4 :L_Data low from 0.0.0 to 1/5/11 hops: 07 T_DATA_XXX_REQ A_GroupValue_Read
            ' LPDU: BC 00 00 0D 0B F1 00 41 F5 :L_Data low from 0.0.0 to 1/5/11 hops: 07 T_DATA_XXX_REQ A_GroupValue_Response (small) 01
            ' LPDU: BC 10 46 0D 0B F1 00 41 A3 :L_Data low from 1.0.70 to 1/5/11 hops: 07 T_DATA_XXX_REQ A_GroupValue_Response (small) 01
            ' LPDU: BC 10 02 0D 0B F1 00 41 E7 :L_Data low from 1.0.2 to 1/5/11 hops: 07 T_DATA_XXX_REQ A_GroupValue_Response (small) 01

            ' Hourly Trigger
            ' LPDU: BC 00 00 09 C8 F4 00 80 57 1A 2D 96 :L_Data low from 0.0.0 to 1/1/200 hops: 07 T_DATA_XXX_REQ A_GroupValue_Write 57 1A 2D
            ' LPDU: BC 00 00 09 C9 F4 00 80 1E 04 0D E0 :L_Data low from 0.0.0 to 1/1/201 hops: 07 T_DATA_XXX_REQ A_GroupValue_Write 1E 04 0D

            ' Write ON/OFF
            ' LPDU: BC 00 00 0D 0A F1 00 81 34 :L_Data low from 0.0.0 to 1/5/10 hops: 07 T_DATA_XXX_REQ A_GroupValue_Write (small) 01
            ' LPDU: BC 00 00 0D 0A F1 00 80 35 :L_Data low from 0.0.0 to 1/5/10 hops: 07 T_DATA_XXX_REQ A_GroupValue_Write (small) 00
            ' LPDU: BC 10 02 0D 0B E1 00 80 36 :L_Data low from 1.0.2 to 1/5/11 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 00
            ' LPDU: BC 00 00 0D 0A F1 00 81 34 :L_Data low from 0.0.0 to 1/5/10 hops: 07 T_DATA_XXX_REQ A_GroupValue_Write (small) 01
            ' LPDU: BC 10 02 0D 0B E1 00 81 37 :L_Data low from 1.0.2 to 1/5/11 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 01
            ' LPDU: BC 10 01 0B 15 E1 00 80 2D :L_Data low from 1.0.1 to 1/3/21 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 00
            ' LPDU: BC 10 01 09 0B E1 00 81 30 :L_Data low from 1.0.1 to 1/1/11 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 01 
            ' LPDU: BC 10 14 09 0A E1 00 80 25 :L_Data low from 1.0.20 to 1/1/10 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 00
            ' LPDU: BC 10 01 09 0B E1 00 80 31 :L_Data low from 1.0.1 to 1/1/11 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 00 
            ' LPDU: BC 10 02 0F 15 E1 00 81 2B :L_Data low from 1.0.2 to 1/7/21 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 01
            ' LPDU: BC 10 01 09 0B E1 00 81 30 :L_Data low from 1.0.1 to 1/1/11 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 01

            ' DIM
            ' LPDU: BC 10 03 0E 1F E1 00 81 21 :L_Data low from 1.0.3 to 1/6/31 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 01
            ' LPDU: BC 10 03 0E 21 E2 00 80 8A 97 :L_Data low from 1.0.3 to 1/6/33 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write 8A
            ' LPDU: BC 10 03 0E 0B E1 00 81 35 :L_Data low from 1.0.3 to 1/6/11 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write (small) 01
            ' LPDU: BC 10 03 0E 0D E2 00 80 FF CE :L_Data low from 1.0.3 to 1/6/13 hops: 06 T_DATA_XXX_REQ A_GroupValue_Write FF

            _from = ExtractStringBetweenTwoStrings(_message, " from ", " to ").Trim()
            _to = ExtractStringBetweenTwoStrings(_message, " to ", " hops:").Trim()

            Dim tmpstr As String
            tmpstr = _message.Substring(_message.IndexOf("hops:")).Trim()
            _action = Regex.Split(tmpstr, " ")(3).Trim()
            _value = tmpstr.Substring(tmpstr.IndexOf(_action) + _action.Length).Replace(Chr(0), "").Trim()

        End Set
    End Property

    Private _from As String
    Public ReadOnly Property From() As String
        Get
            Return _from
        End Get
    End Property

    Private _to As String
    Public ReadOnly Property [To]() As String
        Get
            Return _to
        End Get
    End Property

    Private _action As String
    Public ReadOnly Property Action() As String
        Get
            Return _action
        End Get
    End Property

    Private _value As String
    Public ReadOnly Property Value() As String
        Get
            Return _value
        End Get
    End Property


    Private Function ExtractStringBetweenTwoStrings(ByVal fromString As String, ByVal string1 As String, ByVal string2 As String) As String

        Dim tmpstr As String
        tmpstr = fromString.Substring(fromString.IndexOf(string1) + string1.Length)
        tmpstr = tmpstr.Substring(0, tmpstr.IndexOf(string2))
        Return tmpstr

    End Function

End Class
