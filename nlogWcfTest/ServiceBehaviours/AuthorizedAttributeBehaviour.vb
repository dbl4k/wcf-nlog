Imports System.Collections.ObjectModel
Imports System.Reflection
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher

Public Class AuthorizedAttributeBehaviour
    Implements IServiceBehavior, IEndpointBehavior

#Region "IServiceBehavior Members"

    Public Sub ApplyDispatchBehavior(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase) Implements IServiceBehavior.ApplyDispatchBehavior
        For Each chDisp As ChannelDispatcher In serviceHostBase.ChannelDispatchers
            For Each epDisp As EndpointDispatcher In chDisp.Endpoints
                epDisp.DispatchRuntime.MessageInspectors.Add(New AuthorizedAttributeInspector(chDisp, epDisp))
            Next
        Next
    End Sub

    Public Sub ApplyDispatchBehavior(endpoint As ServiceEndpoint, endpointDispatcher As EndpointDispatcher) Implements IEndpointBehavior.ApplyDispatchBehavior
        For Each operation In endpoint.Contract.Operations
            Dim declaringContract As ContractDescription = operation.DeclaringContract
            Dim operationBehaviour As New AuthorizedAttributeOperation(endpoint, operation)

            With operation.OperationBehaviors

                .Add(operationBehaviour)

            End With

        Next
    End Sub

    Public Sub Validate(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase) Implements IServiceBehavior.Validate
        ' Not used
    End Sub

    Public Sub Validate(endpoint As ServiceEndpoint) Implements IEndpointBehavior.Validate
        '  Throw New NotImplementedException()
    End Sub

    Public Sub AddBindingParameters(endpoint As ServiceEndpoint, bindingParameters As BindingParameterCollection) Implements IEndpointBehavior.AddBindingParameters
        ' Throw New NotImplementedException()
    End Sub

    Public Sub ApplyClientBehavior(endpoint As ServiceEndpoint, clientRuntime As ClientRuntime) Implements IEndpointBehavior.ApplyClientBehavior
        'Throw New NotImplementedException()
    End Sub

    Private Sub AddBindingParameters(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase, endpoints As Collection(Of ServiceEndpoint), bindingParameters As BindingParameterCollection) Implements IServiceBehavior.AddBindingParameters
        '
    End Sub

#End Region

End Class


Public Class AuthorizedAttributeOperation
    Implements IOperationBehavior

    Private endpoint As ServiceEndpoint
    Private operation As OperationDescription

    Public Sub New(endpoint As ServiceEndpoint, operation As OperationDescription)
        Me.endpoint = endpoint
        Me.operation = operation
    End Sub

    Public Sub Validate(operationDescription As OperationDescription) Implements IOperationBehavior.Validate
        ' not used
    End Sub

    Public Sub ApplyDispatchBehavior(operationDescription As OperationDescription, dispatchOperation As DispatchOperation) Implements IOperationBehavior.ApplyDispatchBehavior
        Dim method As MethodInfo = operationDescription.SyncMethod

        Dim hasAttribute As Boolean = method.
            GetCustomAttributes.ToList.
            Select(Function(n) n.GetType Is GetType(AuthorizationStoreRoleProvider)).Any

        If hasAttribute Then
            ' Throw New HttpException(401, "Unauthorized access")
        End If

    End Sub

    Public Sub ApplyClientBehavior(operationDescription As OperationDescription, clientOperation As ClientOperation) Implements IOperationBehavior.ApplyClientBehavior
        ' not used
    End Sub

    Public Sub AddBindingParameters(operationDescription As OperationDescription, bindingParameters As BindingParameterCollection) Implements IOperationBehavior.AddBindingParameters
        ' not used
    End Sub
End Class