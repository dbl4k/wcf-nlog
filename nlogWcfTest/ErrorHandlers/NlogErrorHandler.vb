Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher
Imports NLog

Public Class NlogErrorHandler : Implements IErrorHandler

    Private Shared mLogger As Logger = LogManager.GetLogger("NLogErrorHandler")

    Public Sub ProvideFault([error] As Exception, version As MessageVersion, ByRef fault As Message) Implements IErrorHandler.ProvideFault
        ' Not used
    End Sub

    Public Function HandleError([error] As Exception) As Boolean Implements IErrorHandler.HandleError
        mLogger.Log(LogLevel.Error, [error])

        Return False ' Don't break exception handling.
    End Function

    Private Function getContextMessage(context As OperationContext) As String
        Dim result As New StringBuilder(String.Empty)

        With result
            .AppendLine(context.RequestContext.RequestMessage.ToString)
        End With


        Return result.ToString()
    End Function

    Public Function getCurrentContext() As OperationContext
        Dim result As OperationContext = Nothing

        result = OperationContext.Current

        Return result
    End Function

End Class
