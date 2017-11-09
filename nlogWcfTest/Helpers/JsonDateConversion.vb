Imports Runtime.Remoting.Metadata.W3cXsd2001

Public Class JsonDateConversion

    Public Class Patterns
        Public Const ISO8601 As String = "(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})"
        Public Const JSON_VALUE As String = "(\/Date\((\d+)(?:[-+](\d+))?\)\/)"
    End Class

    Public Class Reference
        Public Shared UnixEpoch As DateTime = New DateTime(1970, 1, 1)
    End Class

    Public Class Format
        Public Const F_JSONDATEVALUE As String = "/Date({0})/"
        Friend Const F_ISO8601 As String = "yyyy-MM-ddTHH:mm:ss"
    End Class

    Public Shared Function ConvertIso8601ToDate(isodate As String) As Date
        Dim result As Date

        result = Runtime.Remoting.Metadata.W3cXsd2001.SoapDateTime.Parse(isodate)

        Return result
    End Function

    Public Shared Function ConvertDateToJsonDateValue(dateTime As Date) As String
        Dim result As String = Nothing

        Dim value As Long = (dateTime - Reference.UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond

        result = String.Format(Format.F_JSONDATEVALUE, value)

        Return result
    End Function

    Friend Shared Function ConvertJsonDateValueToDate(value As String) As Date
        Dim result As Date

        Dim timeValue As Long = Convert.ToInt64(Regex.Match(value, Patterns.JSON_VALUE).Groups(2).Value)
        Dim offsetValue As Long = Convert.ToInt64(Regex.Match(value, Patterns.JSON_VALUE).Groups(3).Value)

        result = Reference.UnixEpoch.AddTicks(TimeSpan.TicksPerMillisecond * timeValue)

        Return result
    End Function

    Friend Shared Function ConvertDateToIso8601String(dateTime As Date) As String
        Dim result As String = Nothing

        result = dateTime.ToString(Format.F_ISO8601)

        Return result
    End Function

End Class
