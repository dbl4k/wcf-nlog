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
        For Each endpoint In endpoints
            For Each operation In endpoint.Contract.Operations
                operation.OperationBehaviors.Add(New AuthorizedAttributeOperation())
            Next
        Next
    End Sub

#End Region

End Class


Public Class AuthorizedAttributeOperation
    Implements IOperationBehavior

    Public Sub Validate(operationDescription As OperationDescription) Implements IOperationBehavior.Validate
        ' not used
    End Sub

    Public Sub ApplyDispatchBehavior(operationDescription As OperationDescription, dispatchOperation As DispatchOperation) Implements IOperationBehavior.ApplyDispatchBehavior
        ' not used
    End Sub

    Public Sub ApplyClientBehavior(operationDescription As OperationDescription, clientOperation As ClientOperation) Implements IOperationBehavior.ApplyClientBehavior
        ' not used
    End Sub

    Public Sub AddBindingParameters(operationDescription As OperationDescription, bindingParameters As BindingParameterCollection) Implements IOperationBehavior.AddBindingParameters
        ' not used
    End Sub
End Class