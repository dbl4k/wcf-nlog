' NOTE: You can use the "Rename" command on the context menu to change the class name "RestfulJsonService" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select RestfulJsonService.svc or RestfulJsonService.svc.vb at the Solution Explorer and start debugging.
Public Class RestfulJsonService
    Inherits NlogWcfService
    Implements IRestfulJsonService

    Protected Friend Class Messages
        Public Const E_InvalidIdentity As String = "You supplied an invalid Id '{0}'."
    End Class

    Private num_entities As Integer = 10

    ' GET : http://localhost:58958/api/Entities
    <WebInvoke(Method:="GET", UriTemplate:="entities", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json)>
    Public Function Get_Entities() As Entity() Implements IRestfulJsonService.Get_Entities
        Dim result As New List(Of Entity)

        For i As Integer = 1 To num_entities
            result.Add(EntityFactory.GenerateEntity(i))
        Next

        Return result.ToArray()
    End Function

    ' GET : http://localhost:58958/api/Entities/5
    <WebInvoke(Method:="GET", UriTemplate:="entities/{id}", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json)>
    Public Function Get_Entity_ById(id As String) As Entity Implements IRestfulJsonService.Get_Entity_ById
        Dim result As Entity = Nothing

        Dim cleaned_int As Integer = Convert.ToInt32(id)

        If cleaned_int >= 1 AndAlso cleaned_int <= num_entities Then
            result = EntityFactory.GenerateEntity(cleaned_int)
        Else
            Throw New IndexOutOfRangeException(String.Format(Messages.E_InvalidIdentity, cleaned_int))
        End If

        Return result
    End Function

#Region "Mock Factories"

    Public Class EntityFactory
        Public Shared Function GenerateEntity(id As Integer) As Entity
            Return New Entity With {.Id = id, .Name = String.Concat(GetType(Entity), id)}
        End Function
    End Class

#End Region

End Class
