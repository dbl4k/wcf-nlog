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

        Return False ' Don't break excepton handling
    End Function
End Class
