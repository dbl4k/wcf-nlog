Imports NLog

' https://github.com/NLog/NLog/wiki/Tutorial#expose-logger-to-sub-classes

Public Class NlogWcfService

    Private mLogger As Logger = Nothing

    Public Sub New()
        Logger = LogManager.GetLogger(Me.GetType.FullName)
    End Sub

    Protected Property Logger As Logger
        Get
            Return mLogger
        End Get
        Set(value As Logger)
            mLogger = value
        End Set
    End Property

End Class
