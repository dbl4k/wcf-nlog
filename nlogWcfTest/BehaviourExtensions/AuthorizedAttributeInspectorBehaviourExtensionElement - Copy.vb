Imports System.ServiceModel.Configuration

Public Class AuthorizedAttributeBehaviourExtensionElement
    Inherits BehaviorExtensionElement

    Public Overrides ReadOnly Property BehaviorType As Type
        Get
            Return GetType(AuthorizedAttributeBehaviour)
        End Get
    End Property

    Protected Overrides Function CreateBehavior() As Object
        Return New AuthorizedAttributeBehaviour
    End Function

End Class
