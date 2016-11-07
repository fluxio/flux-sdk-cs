 #Struct CellValue

Represents the cell value

**Namespace: **Flux.SDK.DataTableApi

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class CellValue`

## Properties

* `Stream` \(System.IO.Stream\): Stream value of the cell.
* `StreamLength ` \(long\): Length of the Stream cell value.

## Methods

* `AsInt32()`: Converts Stream value of the cell to Int32.

  #### Returns

  (int) The cell value as Int32.

  #### [Exceptions](./Exceptions.md)

  1. `InternalSDKException`: Throw InternalSDKException if data can't be read.

 
* `AsInt64()`: Converts Stream value of the cell to Int64 .

  #### Returns

  (System.Int64) The cell value as Int64.

  #### [Exceptions](./Exceptions.md)

  1. `InternalSDKException`: Throw InternalSDKException if data can't be read.


* `AsString()`: Converts Stream value of the cell to string.

  #### Returns

  (string) The cell value as string.

  #### [Exceptions](./Exceptions.md)

  1. `InternalSDKException`: Throw InternalSDKException if data can't be read.


