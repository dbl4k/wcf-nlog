Imports System.ServiceModel

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IRestfulJsonService" in both code and config file together.
<ServiceContract()>
Public Interface IRestfulJsonService

    <OperationContract()>
    Function Get_Entities() As Entity()

    <OperationContract()>
    Function Get_Entity_ById(id As String) As Entity

End Interface

<DataContract()>
Public Class Entity
    <DataMember()>
    Public Property Id As Integer

    <DataMember()>
    Public Property Name As String
End Class
