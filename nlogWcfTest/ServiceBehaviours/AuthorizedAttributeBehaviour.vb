Imports System.Collections.ObjectModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher

Public Class AuthorizedAttributeBehaviour
    Implements IServiceBehavior

#Region "IServiceBehavior Members"

    Public Sub ApplyDispatchBehavior(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase) Implements IServiceBehavior.ApplyDispatchBehavior
        For Each chDisp As ChannelDispatcher In serviceHostBase.ChannelDispatchers
            For Each epDisp As EndpointDispatcher In chDisp.Endpoints
                epDisp.DispatchRuntime.MessageInspectors.Add(New AuthorizedAttributeInspector())
            Next
        Next
    End Sub

    Public Sub Validate(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase) Implements IServiceBehavior.Validate
        ' Not used
    End Sub

    Private Sub AddBindingParameters(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase, endpoints As Collection(Of ServiceEndpoint), bindingParameters As BindingParameterCollection) Implements IServiceBehavior.AddBindingParameters
        ' Not used
    End Sub

#End Region

End Class