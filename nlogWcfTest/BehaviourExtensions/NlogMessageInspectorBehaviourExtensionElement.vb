Imports System.ServiceModel.Configuration

Public Class NlogMessageInspectorBehaviourExtensionElement
    Inherits BehaviorExtensionElement

    Public Overrides ReadOnly Property BehaviorType As Type
        Get
            Return GetType(NlogMessageInspectorBehaviour)
        End Get
    End Property

    Protected Overrides Function CreateBehavior() As Object
        Return New NlogMessageInspectorBehaviour
    End Function

End Class
