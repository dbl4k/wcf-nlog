Imports System.Net
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher

Public Class AuthorizedAttributeInspector : Inherits SimpleMessageInspector

    Private channelDispatcher As ChannelDispatcher
    Private endpointDispatcher As EndpointDispatcher

    Public Sub New(channelDispatcher As ChannelDispatcher, endpointDispatcher As EndpointDispatcher)
        Me.channelDispatcher = channelDispatcher
        Me.endpointDispatcher = endpointDispatcher
    End Sub

    Public Overrides Sub ModifyBody(ByRef body As String, originalMessage As Message, isRequest As Boolean)
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Sub InspectRequest(headers As WebHeaderCollection, body As String, originalMessage As Message)

        Dim properties As MessageProperties = originalMessage.Properties
        If properties.ContainsKey("UriMatched") Then
            Dim match As UriTemplateMatch = properties("UriTemplateMatchResults")
            Dim methodName As String = match.Data

        End If

    End Sub

    Public Overrides Sub InspectResponse(responseCode As HttpStatusCode, body As String, originalMessage As Message)
        'Throw New NotImplementedException()
    End Sub
End Class
