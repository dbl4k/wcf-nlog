﻿' NOTE: You can use the "Rename" command on the context menu to change the interface name "IRegularSoapService" in both code and config file together.
<ServiceContract()>
Public Interface IRegularSoapService

    <OperationContract()>
    Function GetData(ByVal value As Integer) As String

    <OperationContract()>
    Function DoSomethingDatelike(ByVal value As Date) As String

    <OperationContract()>
    Function GetDataUsingDataContract(ByVal composite As CompositeType) As CompositeType

    ' TODO: Add your service operations here

End Interface

' Use a data contract as illustrated in the sample below to add composite types to service operations.

<DataContract()>
Public Class CompositeType

    <DataMember()>
    Public Property BoolValue() As Boolean

    <DataMember()>
    Public Property StringValue() As String

End Class