Imports System.IO
Imports System.Reflection
Imports System.Runtime.Serialization.Json
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher
Imports System.Xml
Imports Newtonsoft.Json
Imports NLog

Public Class NlogMessageInspector
    Implements IDispatchMessageInspector

    Private Const BINARY As String = "Binary"

    Private Const REQUEST_RECEIVED As String = "REQUEST_RECEIVED"
    Private Const RESPONSE_SENT As String = "RESPONSE_SENT"

    Private Const BODY_SEPARATOR As String = "---"

    Private mLogger As Logger = LogManager.GetLogger("MessageInspector")

    Public Sub BeforeSendReply(ByRef reply As Message, correlationState As Object) Implements IDispatchMessageInspector.BeforeSendReply
        Dim builder As New StringBuilder()

        If Me.GetMessageContentFormat(reply) = WebContentFormat.Json Then
            reply = ConvertISO8601Dates(reply, isRequest:=False)
        End If

        Using writer As StringWriter = New StringWriter(builder)

            If reply.Properties.ContainsKey(HttpResponseMessageProperty.Name) Then
                Dim response As HttpResponseMessageProperty =
                    DirectCast(reply.Properties(HttpResponseMessageProperty.Name), HttpResponseMessageProperty)

                writer.WriteLine("{0} {1}", CInt(response.StatusCode), response.StatusCode)
            End If

            If Not reply.IsEmpty Then
                writer.WriteLine()
                writer.WriteLine(Me.GetMessageAsString(reply))
            End If

        End Using

        mLogger.Log(LogLevel.Debug, GetInspectorMessage(MethodBase.GetCurrentMethod(), builder.ToString, RESPONSE_SENT))
    End Sub

    Public Function AfterReceiveRequest(ByRef request As Message, channel As IClientChannel, instanceContext As InstanceContext) As Object Implements IDispatchMessageInspector.AfterReceiveRequest

        If Me.GetMessageContentFormat(request) = WebContentFormat.Json Then
            request = ConvertISO8601Dates(request, isRequest:=True)
        End If


        Dim requestUri As Uri = request.Headers.[To]
        Dim builder As New StringBuilder()
        Using writer As StringWriter = New StringWriter(builder)
            Dim httpReq As HttpRequestMessageProperty = DirectCast(request.Properties(HttpRequestMessageProperty.Name), HttpRequestMessageProperty)

            writer.WriteLine("{0} {1}", httpReq.Method, requestUri)
            For Each header As String In httpReq.Headers.AllKeys
                writer.WriteLine("{0}: {1}", header, httpReq.Headers(header))
            Next

            If Not request.IsEmpty Then
                writer.WriteLine()
                writer.WriteLine(Me.GetMessageAsString(request))
            End If
        End Using

        mLogger.Log(LogLevel.Debug, GetInspectorMessage(MethodBase.GetCurrentMethod(), builder.ToString, REQUEST_RECEIVED))

        Return Nothing
    End Function

    Private Function GetInspectorMessage(method As MethodBase, message As String, operation As String) As String
        Dim result As New StringBuilder(String.Empty)

        With result
            .AppendLine(operation)
            .AppendLine(BODY_SEPARATOR)
            .AppendLine(message)
            .Append(BODY_SEPARATOR)
        End With

        Return result.ToString
    End Function

    Private Function GetMessageContentFormat(message As Message) As WebContentFormat
        Dim format As WebContentFormat = WebContentFormat.Default

        If message.Properties.ContainsKey(WebBodyFormatMessageProperty.Name) Then
            Dim bodyFormat As WebBodyFormatMessageProperty
            bodyFormat = DirectCast(message.Properties(WebBodyFormatMessageProperty.Name), WebBodyFormatMessageProperty)
            format = bodyFormat.Format
        End If

        Return format
    End Function

    Private Function GetMessageAsString(ByRef message As Message) As String
        Dim format As WebContentFormat = Me.GetMessageContentFormat(message)

        Dim stream As New MemoryStream()
        Dim writer As XmlDictionaryWriter = Nothing

        Select Case format
            Case WebContentFormat.[Default], WebContentFormat.Xml
                writer = XmlDictionaryWriter.CreateTextWriter(stream)
            Case WebContentFormat.Json
                writer = JsonReaderWriterFactory.CreateJsonWriter(stream)
            Case WebContentFormat.Raw
                Return Me.ReadRawBody(message)
        End Select

        message.WriteMessage(writer)
        writer.Flush()

        Dim body As String = Encoding.UTF8.GetString(stream.ToArray())

        ResetStreamPosition(stream)

        Dim reader As XmlDictionaryReader
        Select Case format
            Case WebContentFormat.Json
                reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max)
            Case Else
                reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max)
        End Select

        message = CloneMessage(reader, message)

        Return body
    End Function

    Private Function CloneMessage(reader As XmlDictionaryReader, message As Message) As Message
        Dim result As Message = message.CreateMessage(reader, Integer.MaxValue, message.Version)

        result.Properties.CopyProperties(message.Properties)

        Return result
    End Function

    Private Sub ResetStreamPosition(stream As MemoryStream)
        stream.Position = 0
    End Sub

    Private Function ReadRawBody(ByRef message As Message) As String
        Dim bodyReader As XmlDictionaryReader = message.GetReaderAtBodyContents()
        bodyReader.ReadStartElement(BINARY)

        Dim body As Byte() = bodyReader.ReadContentAsBase64()
        Dim messageBody As String = Encoding.UTF8.GetString(body)
        Dim ms As New MemoryStream()
        Dim writer As XmlDictionaryWriter = XmlDictionaryWriter.CreateBinaryWriter(ms)

        With writer
            .WriteStartElement(BINARY)
            .WriteBase64(body, 0, body.Length)
            .WriteEndElement()
            .Flush()
        End With

        ms.Position = 0
        Dim reader As XmlDictionaryReader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max)
        Dim newMessage As Message = message.CreateMessage(reader, Integer.MaxValue, message.Version)
        newMessage.Properties.CopyProperties(message.Properties)
        message = newMessage

        Return messageBody
    End Function


    Public Function ChangeString(oldMessage As Message, from As String, [to] As String) As Message
        Dim ms As New MemoryStream()
        Dim xw As XmlWriter = XmlWriter.Create(ms)
        oldMessage.WriteMessage(xw)
        xw.Flush()
        Dim body As String = Encoding.UTF8.GetString(ms.ToArray())
        xw.Close()

        body = body.Replace(from, [to])

        ms = New MemoryStream(Encoding.UTF8.GetBytes(body))
        Dim xdr As XmlDictionaryReader = XmlDictionaryReader.CreateTextReader(ms, New XmlDictionaryReaderQuotas())
        Dim newMessage As Message = Message.CreateMessage(xdr, Integer.MaxValue, oldMessage.Version)
        newMessage.Properties.CopyProperties(oldMessage.Properties)
        Return newMessage
    End Function


    Public Function ConvertISO8601Dates(oldMessage As Message, isRequest As Boolean) As Message
        Dim ms As New MemoryStream()
        Dim xw As XmlWriter = XmlWriter.Create(ms)
        oldMessage.WriteMessage(xw)
        xw.Flush()
        Dim body As String = Encoding.UTF8.GetString(ms.ToArray())
        xw.Close()

        If isRequest Then
            ' "2012-04-21T18:25:43-05:00" -> "\/Date(1509739291460+0000)\/"
            Dim regex As New Regex(JsonDateConversion.Patterns.ISO8601)
            For Each match As Match In regex.Matches(body)
                Dim dateTime As Date = JsonDateConversion.ConvertIso8601ToDate(match.Groups(0).Value)
                Dim jsonValue As String = JsonDateConversion.ConvertDateToJsonDateValue(dateTime)

                body = body.Replace(match.Groups(0).Value, jsonValue)
            Next
        Else
            ' "\/Date(1509739291460+0000)\/" -> "2012-04-21T18:25:43-05:00"
            Dim regex As New Regex(JsonDateConversion.Patterns.JSON_VALUE)
            For Each match As Match In regex.Matches(body)
                Dim dateTime As Date = JsonDateConversion.ConvertJsonDateValueToDate(match.Groups(0).Value)
                Dim iso8601Value As String = JsonDateConversion.ConvertDateToIso8601String(dateTime)

                body = body.Replace(match.Groups(0).Value, iso8601Value)
            Next
        End If

        ms = New MemoryStream(Encoding.UTF8.GetBytes(body))
        Dim xdr As XmlDictionaryReader = XmlDictionaryReader.CreateTextReader(ms, New XmlDictionaryReaderQuotas())
        Dim newMessage As Message = Message.CreateMessage(xdr, Integer.MaxValue, oldMessage.Version)
        newMessage.Properties.CopyProperties(oldMessage.Properties)
        Return newMessage
    End Function

End Class
