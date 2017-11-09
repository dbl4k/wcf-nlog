Imports System.IO
Imports System.Reflection
Imports System.Runtime.Serialization.Json
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher
Imports System.Xml
Imports Newtonsoft.Json
Imports NLog

Public Class AuthorizedAttributeInspector
    Implements IDispatchMessageInspector

    Private Const BINARY As String = "Binary"

    Private Const BODY_SEPARATOR As String = "---"

    Private mLogger As Logger = LogManager.GetLogger("AuthorizedAttributeInspector")

    Public Sub BeforeSendReply(ByRef reply As Message, correlationState As Object) Implements IDispatchMessageInspector.BeforeSendReply
        ' Not required
    End Sub

    Public Function AfterReceiveRequest(ByRef request As Message, channel As IClientChannel, instanceContext As InstanceContext) As Object Implements IDispatchMessageInspector.AfterReceiveRequest

        If Me.GetMessageContentFormat(request) = WebContentFormat.Json Then
            ' TODO : Check if urltemplate matched method has <AuthorizedAttribute>
        End If

        Return Nothing
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
        Dim result As Message = Message.CreateMessage(reader, Integer.MaxValue, message.Version)

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
        Dim newMessage As Message = Message.CreateMessage(reader, Integer.MaxValue, message.Version)
        newMessage.Properties.CopyProperties(message.Properties)
        message = newMessage

        Return messageBody
    End Function

End Class
