 #Struct CellEvent

Represents information about the cell event.

**Namespace: **Flux.SDK.DataTableApi

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class CellEvent`

## Properties

* `ClientId` \(string\): Represent id of the client.
* `ClientInfo` \([ClientInfo](../Types/ClientInfo.md)\): Represents the client information.
* `Size`\(long\): Represents the size of the value stored in this cell.
* `Time`\(double\): Represents the time when event occurred. Use `GetDate` to convert value to DateTime.
* `Type`\([NotificationType](./Notification.md)\): Type of event.

## Methods

* `GetDate()`: Converts Java timestamp to DateTime.

  ####Returns
  
  (DateTime) Time as DateTime structure.