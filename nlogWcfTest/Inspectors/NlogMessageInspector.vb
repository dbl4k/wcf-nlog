Imports System.Reflection
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher
Imports NLog

Public Class NlogMessageInspector
    Implements IDispatchMessageInspector

    Private Const REQUEST_RECEIVED As String = "REQUEST_RECEIVED"
    Private Const RESPONSE_SENT As String = "RESPONSE_SENT"

    Private mLogger As Logger = LogManager.GetLogger("MessageInspector")

    Public Sub BeforeSendReply(ByRef reply As Message, correlationState As Object) Implements IDispatchMessageInspector.BeforeSendReply
        mLogger.Log(LogLevel.Debug, GetInspectorMessage(MethodBase.GetCurrentMethod(), reply, RESPONSE_SENT))
    End Sub

    Public Function AfterReceiveRequest(ByRef request As Message, channel As IClientChannel, instanceContext As InstanceContext) As Object Implements IDispatchMessageInspector.AfterReceiveRequest
        mLogger.Log(LogLevel.Debug, GetInspectorMessage(MethodBase.GetCurrentMethod(), request, REQUEST_RECEIVED))

        Return Nothing
    End Function

    Private Function GetInspectorMessage(method As MethodBase, message As Message, operation As String) As String
        Dim result As New StringBuilder(String.Empty)

        With result
            .AppendLine(operation)
            .Append(message)
        End With

        Return result.ToString
    End Function
End Class
