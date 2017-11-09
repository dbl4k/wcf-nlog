Imports NLog
Imports nlogWcfTest

Public Class RestfulJsonService
    Inherits NlogWcfService
    Implements IRestfulJsonService

    Protected Friend Class Messages
        Public Const E_InvalidIdentity As String = "You supplied an invalid Id '{0}'."
        Public Const E_InvalidValue As String = "You supplied an invalid value '{0}', expected {1}."

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

        Dim cleaned_int As Integer

        Try
            cleaned_int = Convert.ToInt32(id)
        Catch ex As Exception
            Dim ex_wrapper As Exception = New InvalidCastException(String.Format(Messages.E_InvalidValue, id, GetType(Integer)), ex)
            Logger.Log(LogLevel.Error, ex_wrapper)
            Throw ex_wrapper
        End Try

        If cleaned_int >= 1 AndAlso cleaned_int <= num_entities Then
            result = EntityFactory.GenerateEntity(cleaned_int)
        Else
            Dim ex As Exception = New IndexOutOfRangeException(String.Format(Messages.E_InvalidIdentity, cleaned_int))
            Logger.Log(LogLevel.Error, ex)
            Throw ex
        End If

        Return result
    End Function

    ' POST : http://localhost:58958/api/Entities
    <WebInvoke(Method:="POST", UriTemplate:="entities", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json)>
    Public Function Post_Entity(entity As Entity) As Entity Implements IRestfulJsonService.Post_Entity
        Dim result As Entity = entity

        ' Just return the same object..

        Return result
    End Function

    ' POST : http://localhost:58958/api/Customer/666/FeedbackMessages
    <WebInvoke(Method:="POST", UriTemplate:="customer/{customerId}/feedbackmessages", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json)>
    Public Function Post_Customer_FeedbackMessage(customerId As String, entity As FeedbackMessage) As FeedbackMessage Implements IRestfulJsonService.Post_Customer_FeedbackMessage
        entity.Subject &= " all good!"
        'entity.ReceivedDate = New Date(2017, 6, 5, 13, 0, 0, DateTimeKind.Local)
        Return entity
    End Function

#Region "Mock Factories"

    Public Class EntityFactory
        Public Shared Function GenerateEntity(id As Integer) As Entity
            Return New Entity With {.Id = id,
                                    .Name = String.Concat(GetType(Entity).Name, id),
                                    .CreatedDate = Now.AddDays(0 - id)}
        End Function
    End Class

#End Region

End Class
