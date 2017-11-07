' NOTE: You can use the "Rename" command on the context menu to change the class name "RegularSoapService" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select RegularSoapService.svc or RegularSoapService.svc.vb at the Solution Explorer and start debugging.
Imports nlogWcfTest

Public Class RegularSoapService
    Inherits NlogWcfService
    Implements IRegularSoapService

    Public Sub New()
    End Sub

    Public Function GetData(ByVal value As Integer) As String Implements IRegularSoapService.GetData
        Return String.Format("You entered: {0}", value)
    End Function

    Public Function GetDataUsingDataContract(ByVal composite As CompositeType) As CompositeType Implements IRegularSoapService.GetDataUsingDataContract
        If composite Is Nothing Then
            ' Throw New ArgumentNullException("composite")
        End If
        If composite.BoolValue Then
            composite.StringValue &= "Suffix"
        End If
        Logger.Log(NLog.LogLevel.Info, "logging is working?")
        Throw New Exception("intentional, don't panic")
        Return composite
    End Function

    Public Function DoSomethingDatelike(value As Date) As String Implements IRegularSoapService.DoSomethingDatelike
        Return "Oh, nice!"
    End Function
End Class
