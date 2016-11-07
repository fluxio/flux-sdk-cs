# Class DataTable

Represents data and operations with cells

**Namespace: **Flux.SDK.DataTableApi

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class DataTable`

## Properties

* `Capability` \([Capability](./Capability.md)\): Returns Capability of the datatable
* `Cells` \(List&lt;[CellInfo](./CellInfo.md)&gt;\): Returns datatable cells list
* `ProgressTracker`\([ProgressReporter](./DataTableTypes/ProgressReporter.md)\): Provides ability to track the progress.


## Methods

* `CreateCell(valueSteam, clientMetadata)`: Creates new cell.

  ##### Arguments

  1. `valueSteam` \(System.IO.Stream\): Stream contains value for new cell.
  2. `clientMetadata`\([ClientMetadata](./ClientMetadata.md)\): ClientMetadata to associate with the cell \(optional\).

  ##### Returns

  \([CellInfo](./CellInfo.md)\) CellInfo of the newly created cell.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported.
  2. `ConnectionFailureException`: Throws if network connection is down.
  3. `UnathorizedException`: Throws if provided cookies were obsolete.
  4. `ForbiddenException`:Throws if this project is readonly.
  5. `ServerUnavailableException`: Throws if Flux server is down. 
  6. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `CreateCell(json, clientMetadata)`: Creates new cell.

  ##### Arguments

  1. `json`\(string\): Json representation of the value.
  2. `clientMetadata`\([ClientMetadata](./ClientMetadata.md)\): ClientMetadata to associate with the cell \(optional\).

  ##### Returns

  \([CellInfo](./CellInfo.md)\) CellInfo of the newly created cell.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported.
  2. `ConnectionFailureException`: Throws if network connection is down.
  3. `UnathorizedException`: Throws if provided cookies were obsolete.
  4. `ForbiddenException`:Throws if this project is readonly.
  5. `ServerUnavailableException`: Throws if Flux server is down.
  6. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `CreateCellAsync(valueSteam, clientMetadata)`: Creates new cell asynchronously .

  ##### Arguments

  1. `valueSteam` \(System.IO.Stream\): Stream contains value for new cell.
  2. `clientMetadata`\([ClientMetadata](./ClientMetadata.md)\): ClientMetadata to associate with the cell \(optional\).

  ##### Returns

  \(Task&lt;[CellInfo&gt;](./CellInfo.md)\) CellInfo of the newly created cell.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported.
  2. `ConnectionFailureException`: Throws if network connection is down.
  3. `UnathorizedException`: Throws if provided cookies were obsolete.
  4. `ForbiddenException`:Throws if this project is readonly.
  5. `ServerUnavailableException`: Throws if Flux server is down.
  6. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `CreateCellAsync(json, clientMetadata)`:  Creates new cell asynchronously.

  ##### Arguments

  1. `json`\(string\): Json representation of the value.
  2. `clientMetadata`\([ClientMetadata](./ClientMetadata.md)\): ClientMetadata to associate with the cell \(optional\).

  ##### Returns

  \(Task&lt;[CellInfo&gt;](./CellInfo.md)\) CellInfo of the newly created cell.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported
  2. `ConnectionFailureException`: Throws if network connection is down.
  3. `UnathorizedException`: Throws if provided cookies were obsolete.
  4. `ForbiddenException`:Throws if this project is readonly.
  5. `ServerUnavailableException`: Throws if Flux server is down.
  6. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `DeleteCell(cellId)`: Deletes cell by id.

  ##### Arguments

  1. `cellId`\(string\): Id of the cell to be deleted.

  ##### Returns

  \([CellInfo](./CellInfo.md)\) CellInfo of the deleted cell.

  ##### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.
  2. `UnathorizedException`: Throws if provided cookies were obsolete.
  3. `ForbiddenException`:Throws if this project is readonly.
  4. `ServerUnavailableException`: Throws if Flux server is down.
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `DeleteCellAsync(cellId)`: Deletes cell by id asynchronously .

  ##### Arguments

  1. `cellId` \(string\): Id of the cell to be deleted.

  ##### Returns

  \(Task&lt;[CellInfo&gt;](./CellInfo.md)\) CellInfo of the deleted cell.

  ##### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.
  2. `UnathorizedException`: Throws if provided cookies were obsolete.
  3. `ForbiddenException`:Throws if this project is readonly.
  4. `ServerUnavailableException`: Throws if Flux server is down.
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `DereferenceCellValueRef(valueRef, extractValueMetadata, extractCellMetadata, extractClientMetadata)`: Dereference a permanent reference to value.

  ##### Arguments

  1. `valueRef` \(string\): Cell reference to release
  2. `extractValueMetadata`\(bool\): If set to true then metadata associated with the value will be extracted \(optional\)
  3. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optional\)
  4. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optional\).

  ##### Returns

  \([CellReleaseRefResult](./DataTableTypes/CellReleaseRefResult.md)\) CellReleaseRefResult with dereferenced permanent cell reference to value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if Capability.VALUE\_REFERENCE is not supported
  2. `ConnectionFailureException`: Throws if network connection is down.
  3. `UnathorizedException`: Throws if provided cookies were obsolete.
  4. `ServerUnavailableException`: Throws if Flux server is down.
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `DereferenceCellValueRefAsync(valueRef, extractValueMetadata, extractCellMetadata, extractClientMetadata)`: Dereference a permanent reference to value asynchronously.

  ##### Arguments

  1. `valueRef` \(string\): Cell reference to release
  2. `extractValueMetadata`\(bool\): If set to true then metadata associated with the value will be extracted \(optional\)
  3. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optional\)
  4. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optional\)

  ##### Returns

  \(Task&lt;[CellReleaseRefResult&gt;](./DataTableTypes/CellReleaseRefResult.md)\) CellReleaseRefResult with dereferenced permanent cell reference to value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.VALUE\_REFERENCE](./Capability.md) is not supported.
  2. `ConnectionFailureException`: Throws if network connection is down.
  3. `UnathorizedException`: Throws if provided cookies were obsolete.
  4. `ServerUnavailableException`: Throws if Flux server is down.
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `GetCell(cellId, extractCellMetadata, extractClientMetadata)`: Gets cell value stream by id.

  ##### Arguments

  1. `cellId` \(string\):  Id of the cell.
  2. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optianal\)
  3. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optianal\)

  ##### Returns

  \([Cell](./DataTableTypes/Cell.md)\) Stream with the cell value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported
  2. `ConnectionFailureException`: Throws if network connection is down
  3. `UnathorizedException`: Throws if provided cookies were obsolete
  4. `ServerUnavailableException`: Throws if Flux server is down
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `GetCell(cell, extractCellMetadata, extractClientMetadata)`: Get cell value by cells instance.

  ##### Arguments

  1. `cell` \([CellInfo](./CellInfo.md)\): Cell info.
  2. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optional\)
  3. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optional\).

  ##### Returns

  \([Cell](./DataTableTypes/Cell.md)\) Stream with the cell value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported
  2. `ConnectionFailureException`: Throws if network connection is down
  3. `UnathorizedException`: Throws if provided cookies were obsolet
  4. `ServerUnavailableException`: Throws if Flux server is down
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `GetCellAsync(cellId, extractCellMetadata, extractClientMetadata)`: Gets cell value stream by id asynchronously.

  ##### Arguments

  1. `cellId` \(string\): Id of the cell.
  2. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optianal\)
  3. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optional\).

  ##### Returns

  \(Task&lt;[Cell&gt;](./DataTableTypes/Cell.md)\) Stream with the cell value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported
  2. `ConnectionFailureException`: Throws if network connection is down
  3. `UnathorizedException`: Throws if provided cookies were obsolete
  4. `ServerUnavailableException`: Throws if Flux server is down
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `GetCellAsync(cell, extractCellMetadata, extractClientMetadata)`: Get cell value by cells instance  asynchronously.

  ##### Arguments

  1. `cell` \([CellInfo](./CellInfo.md)\): Cell info.
  2. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optianal\)
  3. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optianal\).

  ##### Returns

  \(Task&lt;[Cell&gt;](./DataTableTypes/Cell.md)\) Stream with the cell value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported
  2. `ConnectionFailureException`: Throws if network connection is down
  3. `UnathorizedException`: Throws if provided cookies were obsolete
  4. `ServerUnavailableException`: Throws if Flux server is down
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `GetCellValueReference(cellId, extractCellMetadata, extractClientMetadata)`: Provides a permanent reference to value.

  ##### Arguments

  1. `cellId` \(string\): Id of the cell.
  2. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optianal\)
  3. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optianal\).

  ##### Returns

  \([CellRefResult](./DataTableTypes/CellRefResult.md)\) Permanent reference to value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.VALUE\_REFERENCE](./Capability.md)is not supported.
  2. `ConnectionFailureException`: Throws if network connection is down
  3. `UnathorizedException`: Throws if provided cookies were obsolete
  4. `ServerUnavailableException`: Throws if Flux server is down
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `GetCellValueReference(cell, extractCellMetadata, extractClientMetadata)`: Provides a permanent reference to value.

  ##### Arguments

  1. `cell` \([CellInfo](./CellInfo.md)\): Cell info.
  2. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optional\
  3. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optional\).

  ##### Returns

  \([CellRefResult](./DataTableTypes/CellRefResult.md)\) Permanent reference to value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.VALUE\_REFERENCE](./Capability.md) is not supported
  2. `ConnectionFailureException`: Throws if network connection is down
  3. `UnathorizedException`: Throws if provided cookies were obsolet
  4. `ServerUnavailableException`: Throws if Flux server is down
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `GetCellValueReferenceAsync(cellId, extractCellMetadata, extractClientMetadata)`: Provides a permanent reference to value  asynchronously.

  ##### Arguments

  1. `cellId` \(string\): Id of the cell.
  2. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optianal\).

  3. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optianal\).

  ##### Returns

  \(Task&lt;[CellRefResult&gt;](./DataTableTypes/CellRefResult.md)\) Permanent reference to value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if Capability.VALUE\_REFERENCE is not supported
  2. `ConnectionFailureException`: Throws if network connection is down
  3. `UnathorizedException`: Throws if provided cookies were obsolet
  4. `ServerUnavailableException`: Throws if Flux server is down
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `GetCellValueReferenceAsync(cell, extractCellMetadata, extractClientMetadata)`: Provides a permanent reference to value  asynchronously.

  ##### Arguments

  1. `cell` \([CellInfo](./CellInfo.md)\): Cell info.
  2. `extractCellMetadata`\(bool\): If set to true then metadata associated with the cell will be extracted \(optional\).
  3. `extractClientMetadata`\(bool\): If set to true then client metadata associated with the cell will be extracted \(optional\).

  ##### Returns

  \(Task&lt;[CellRefResult&gt;](./DataTableTypes/CellRefResult.md)\) Permanent reference to value.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.VALUE\_REFERENCE](./Capability.md) is not supported
  2. `ConnectionFailureException`: Throws if network connection is down
  3. `UnathorizedException`: Throws if provided cookies were obsolete
  4. `ServerUnavailableException`: Throws if Flux server is down
  5. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `SetCell(cellId, valueStream, clientMetadata, ignoreValue)`: Updates cell value.

  ##### Arguments

  1. `cellId` \(string\): Id of the cell to be updated
  2. `valueSteam` \(System.IO.Stream\): Stream contains value for cell
  3. `clientMetadata`\([ClientMetadata](./ClientMetadata.md)\): ClientMetadata to associate with the cell \(optional\)
  4. `ignoreValue`\(bool\): If set to true then passed value is ignored.

  ##### Returns

  \([CellInfo](./CellInfo.md)\) CellInfo of the updated cell.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported.

  2. `ConnectionFailureException`: Throws if network connection is down.

  3. `UnathorizedException`: Throws if provided cookies were obsolete

  4. `ForbiddenException`:Throws if project\/cell is readonly or deleted.

  5. `ServerUnavailableException`: Throws if Flux server is down.

  6. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `SetCell(cell, json, clientMetadata, ignorevalue)`:  Updates cell value.

  ##### Arguments

  1. `cell` \( [CellInfo](./CellInfo.md) \): Cell to be updated.
  2. `json`\(string\): Json representation of the value.
  3. `clientMetadata`\([ClientMetadata](./ClientMetadata.md)\): ClientMetadata to associate with the cell \(optional\).
  4. `ignoreValue`\(bool\): If set to true then passed value is ignored.

  ##### Returns

  \([CellInfo](./CellInfo.md)\) CellInfo of the updated cell.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported.

  2. `ConnectionFailureException`: Throws if network connection is down.

  3. `UnathorizedException`: Throws if provided cookies were obsolete

  4. `ForbiddenException`: Throws if project\/cell is readonly or deleted.

  5. `ServerUnavailableException`: Throws if Flux server is down.

  6. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `SetCellAsync(cellId, valueStream, clientMetadata, ignoreValue)`: Updates cell value  asynchronously .

  ##### Arguments

  1. `cellId` \(string\): Id of the cell to be updated.
  2. `valueSteam` \(System.IO.Stream\): Stream contains value for cell.
  3. `clientMetadata`\([ClientMetadata](./ClientMetadata.md)\): ClientMetadata to associate with the cell \(optional\).
  4. `ignoreValue`\(bool\): If set to true then passed value is ignored.

  ##### Returns

  \(Task&lt;[CellInfo&gt;](./CellInfo.md)\) CellInfo of the updated cell.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported.

  2. `ConnectionFailureException`: Throws if network connection is down.

  3. `UnathorizedException`: Throws if provided cookies were obsolete

  4. `ForbiddenException`:Throws if project\/cell is readonly or deleted.

  5. `ServerUnavailableException`: Throws if Flux server is down.

  6. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `SetCellAsync(cell, json, clientMetadata, ignoreValue)`: Updates cell value  asynchronously.

  ##### Arguments

  1. `cell` \( [CellInfo](./CellInfo.md) \): Cell to be updated.
  2. `json`\(string\): Json representation of the value.
  3. `clientMetadata`\([ClientMetadata](./ClientMetadata.md)\): ClientMetadata to associate with the cell \(optional\).
  4. `ignoreValue`\(bool\): If set to true then passed value is ignored.

  ##### Returns

  \(Task&lt;[CellInfo&gt;](./CellInfo.md)\) CellInfo of the updated cell.

  ##### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if [Capability.CLIENT\_METADATA](./Capability.md) is not supported.

  2. `ConnectionFailureException`: Throws if network connection is down.

  3. `UnathorizedException`: Throws if provided cookies were obsolete

  4. `ForbiddenException`: Throws if project\/cell is readonly or deleted.

  5. `ServerUnavailableException`: Throws if Flux server is down.

  6. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `Subcribe(notificationTypes)`: Subscribe to cell events.

  ##### Arguments

  1. `notificationTypes`\(NotificationType\): Types of notification to subscribe for.

  ###### Returns

  Void.

  ###### [Exceptions](./Exceptions.md)

  1. `UnsupportedCapabilityException`: Throws if  [Capability.NOTIFICATION](./Capability.md) is not supported.


* `UnSubcribe(notificationTypes)`: Unsubscribes from datatable notifications.

  ##### Arguments
  1. `notificationTypes`\(NotificationType\): Types of notification to unsubscribe from.

  ##### Returns

  Void.


* `UpdateCells()`: Updates cells for current project.

  ##### Returns

  Void.

  ##### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.

  2. `UnathorizedException`: Throws if provided cookies were obsolete.

  3. `ServerUnavailableException`: Throws if Flux server is down.

  4. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `UpdateCellsAsync()`: Updates cells for current project  asynchronously.

  ##### Returns

  Void.

  ##### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.

  2. `UnathorizedException`: Throws if provided cookies were obsolete

  3. `ServerUnavailableException`: Throws if Flux server is down.

  4. `InternalSDKException`: Throws for unhandled SDK exceptions.

## Events



* `OnError`: Occurs when error message received.

 ##### Handler

 [`ErrorEventHandler`](./Error.md): Represents the method that will handle the OnError event.


* `OnNotification`: Occurs when notification is received.

 ##### Handler

 [`NotificationEventHandler`](./Notification.md): A delegate type for hooking up User logged out notifications.



