# wcf-nlog

A sample implementation of the fantastic nLog in a boilerplate WCF project.

Nothing too advanced here, more of a proof of concept.

## Current Features

* an inheritable base class with an instantiated NLog logger for manual logging control.
* a simple custom set of NLog BehaviorExtensionElement, ServiceBehavior and ErrorHandler that will catch unhandled exceptions.
* behaviour configuration in config.


## Extra Features

Just some stuff i'm hacking around with, including:

* Using a MessageInspector within a ServiceBehavior (WCF Extensibility) to read and write [iso8601](https://en.wikipedia.org/wiki/ISO_8601), by converting to the ugly epoch-based "/Date(n)/" in transit for WCF serializions to occur.
* Adding an <Authorized> attribute to add in custom behaviour - I don't plan to expand on this, just exhibit the basic idea.

## Example Outputs

When pushing an invalid value, like a "?" character into an expected Date param:

```
2017-10-29 09:03:46.0171 ERROR System.ServiceModel.Dispatcher.NetDispatcherFaultException: The formatter threw an exception while trying to deserialize the message: Error in deserializing body of request message for operation 'DoSomethingDatelike'. The value '?' cannot be parsed as the type 'DateTime'. ---> System.Xml.XmlException: The value '?' cannot be parsed as the type 'DateTime'. ---> System.FormatException: The string '?' is not a valid AllXsd value.
   at System.Xml.Schema.XsdDateTime..ctor(String text, XsdDateTimeFlags kinds)
   at System.Xml.XmlConvert.ToDateTime(String s, XmlDateTimeSerializationMode dateTimeOption)
   at System.Xml.XmlConverter.ToDateTime(String value)
   --- End of inner exception stack trace ---
   at System.Xml.XmlConverter.ToDateTime(String value)
   at System.Xml.XmlConverter.ToDateTime(Byte[] buffer, Int32 offset, Int32 count)
   at System.Xml.ValueHandle.ToDateTime()
   at System.Xml.XmlBaseReader.ReadContentAsDateTime()
   at System.Xml.XmlDictionaryReader.ReadElementContentAsDateTime()
   at System.ServiceModel.Dispatcher.PrimitiveOperationFormatter.PartInfo.ReadValue(XmlDictionaryReader reader)
   at System.ServiceModel.Dispatcher.PrimitiveOperationFormatter.DeserializeParameter(XmlDictionaryReader reader, PartInfo part)
   at System.ServiceModel.Dispatcher.PrimitiveOperationFormatter.DeserializeParameters(XmlDictionaryReader reader, PartInfo[] parts, Object[] parameters)
   at System.ServiceModel.Dispatcher.PrimitiveOperationFormatter.DeserializeRequest(XmlDictionaryReader reader, Object[] parameters)
   at System.ServiceModel.Dispatcher.PrimitiveOperationFormatter.DeserializeRequest(Message message, Object[] parameters)
   --- End of inner exception stack trace ---
   at System.ServiceModel.Dispatcher.PrimitiveOperationFormatter.DeserializeRequest(Message message, Object[] parameters)
   at System.ServiceModel.Dispatcher.DispatchOperationRuntime.DeserializeInputs(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage41(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage4(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage31(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage3(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage2(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage11(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage1(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)
```

When using in invalid SOAPAction header value, such as "DoSomethingDatelike2" that doesn't resolve to a method.

```
2017-10-29 09:21:18.0081 ERROR System.ServiceModel.FaultException: The message with Action 'http://tempuri.org/IService1/DoSomethingDatelike2' cannot be processed at the receiver, due to a ContractFilter mismatch at the EndpointDispatcher. This may be because of either a contract mismatch (mismatched Actions between sender and receiver) or a binding/security mismatch between the sender and the receiver.  Check that sender and receiver have the same contract and the same binding (including security requirements, e.g. Message, Transport, None).
   at System.ServiceModel.Dispatcher.ErrorBehavior.ThrowAndCatch(Exception e, Message message)
```

When using an incorrect element name to refer to a method, such as "DoSomethingDatelike2" in the SOAP envelope:

```
2017-10-29 09:22:37.4227 ERROR System.ServiceModel.CommunicationException: Error in deserializing body of request message for operation 'DoSomethingDatelike'. OperationFormatter encountered an invalid Message body. Expected to find node type 'Element' with name 'DoSomethingDatelike' and namespace 'http://tempuri.org/'. Found node type 'Element' with name 'tem:DoSomethingDatelike2' and namespace 'http://tempuri.org/' ---> System.Runtime.Serialization.SerializationException: OperationFormatter encountered an invalid Message body. Expected to find node type 'Element' with name 'DoSomethingDatelike' and namespace 'http://tempuri.org/'. Found node type 'Element' with name 'tem:DoSomethingDatelike2' and namespace 'http://tempuri.org/'
   at System.ServiceModel.Dispatcher.PrimitiveOperationFormatter.DeserializeRequest(XmlDictionaryReader reader, Object[] parameters)
   at System.ServiceModel.Dispatcher.PrimitiveOperationFormatter.DeserializeRequest(Message message, Object[] parameters)
   --- End of inner exception stack trace ---
   at System.ServiceModel.Dispatcher.PrimitiveOperationFormatter.DeserializeRequest(Message message, Object[] parameters)
   at System.ServiceModel.Dispatcher.DispatchOperationRuntime.DeserializeInputs(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage41(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage4(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage31(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage3(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage2(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage11(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage1(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)
```

If you hit an unhandled exception within your code.

```
2017-10-29 09:44:17.3573 ERROR System.Exception: intentional, don't panic
   at nlogWcfTest.Service1.GetDataUsingDataContract(CompositeType composite) in C:\Users\andre\documents\visual studio 2017\Projects\nlogWcfTest\nlogWcfTest\Service1.svc.vb:line 24
   at SyncInvokeGetDataUsingDataContract(Object , Object[] , Object[] )
   at System.ServiceModel.Dispatcher.SyncMethodInvoker.Invoke(Object instance, Object[] inputs, Object[]& outputs)
   at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage41(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage4(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage31(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage3(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage2(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage11(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage1(MessageRpc& rpc)
   at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)
```

Enjoy!

- dbl4k
