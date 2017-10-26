Imports NLog
Imports System.Collections.ObjectModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher

Public Class NlogServiceBehaviour : Implements IServiceBehavior

    Public Sub Validate(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase) Implements IServiceBehavior.Validate
        ' Not used
    End Sub

    Public Sub AddBindingParameters(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase, endpoints As Collection(Of ServiceEndpoint), bindingParameters As BindingParameterCollection) Implements IServiceBehavior.AddBindingParameters
        ' Not used
    End Sub

    Public Sub ApplyDispatchBehavior(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase) Implements IServiceBehavior.ApplyDispatchBehavior
        Dim handler As IErrorHandler = New NlogErrorHandler()

        For Each dispatcher As ChannelDispatcher In serviceHostBase.ChannelDispatchers
            dispatcher.ErrorHandlers.Add(handler)
        Next

    End Sub

End Class
