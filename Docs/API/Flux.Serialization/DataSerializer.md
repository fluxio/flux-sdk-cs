# Static class DataSerializer

Contains methods for data serialization\/deserialization.

**Namespace: **Flux.Serialization

**Assembly: **Flux.Serialization \(in Flux.SDK.dll\)

## Syntax

`public static class DataSerializer`

## Methods

* `Serialize(value)` : Serializes data using Newtonsoft.Json library

  #### Arguments

  1. `value` \(object\): Data to serialize.

  #### Returns

  \(string\)Json with serialized data.

* `Deserialize<T>(data)` : Deserializes data using Newtonsoft.Json library to T.

  #### Type Parameters
  
  1. `T` : result data type.

  #### Arguments

  1. `data` \(string\): Json string to deserialize.


  #### Returns
  \(T\)Deserialized object.

* `Deserialize<T>(jsonStream)` : Deserializes steam using Newtonsoft.Json library to T.

  #### Type Parameters

  1. `T` : result data type.

  #### Arguments

  1. `jsonStream` \(System.IO.Stream\): Json stream to deserialize.

  #### Returns

 \(T\)Deserialized object.

* `DynamicDeserialize(data)` : Deserializes data using Newtonsoft.Json library to dynamic object.

  #### Arguments

  1. `data` \(string\): Json string to deserialize.

  #### Returns

 \(dynamic\)Dynamic object represents Json.


* `Deserialize(jsonStream)` : Deserializes stream using Newtonsoft.Json library to dynamic object.

  #### Arguments

  1. `jsonStream` \(System.IO.Stream\): Json stream to deserialize.
 
 #### Returns

 \(dynamic\)Dynamic object represents Json.





