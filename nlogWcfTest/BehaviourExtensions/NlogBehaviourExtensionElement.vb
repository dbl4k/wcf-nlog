Imports System.ServiceModel.Configuration

Public Class NlogBehaviourExtensionElement
    Inherits BehaviorExtensionElement

    Public Overrides ReadOnly Property BehaviorType As Type
        Get
            Return GetType(NlogServiceBehaviour)
        End Get
    End Property

    Protected Overrides Function CreateBehavior() As Object
        Return New NlogServiceBehaviour
    End Function

End Class
