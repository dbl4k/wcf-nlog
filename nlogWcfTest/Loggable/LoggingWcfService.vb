Imports NLog

Public Class LoggingWcfService
    Public logger As Logger = LogManager.GetLogger(Me.GetType.ToString)
End Class
