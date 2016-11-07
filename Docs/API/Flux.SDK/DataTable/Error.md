# Class Error

Represents error occured on datatable.

**Namespace: **Flux.SDK.DataTableAPI

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class Error`

## Constructors

* `Error(message)`

 ##### Arguments
 1. `message` \(string\): The error message.

## Properties

* `Message` \(Dictionary&lt;string,string&gt;\): Represents the error message.

# Class ErrorEventArgs

Represents the class that contain data for the Error occured event.

**Namespace: **Flux.SDK.DataTableAPI

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class ErrorEventArgs:EventArgs`

## Constructors

* `Error(error, projectId)`

  ##### Arguments

  1. `error` \([Error](./Error.md#class-error)\): The occured error.
  2. `projectId`\(string\): The project id.



## Properties

* `Error` \([Error](./Error.md#class-error)\): Represents the occued error.
* `ProjectId`\(string): The project id.