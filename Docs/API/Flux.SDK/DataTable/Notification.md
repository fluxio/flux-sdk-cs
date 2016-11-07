# Class Notification

Contains the cell notification data.

**Namespace: **Flux.SDK.DataTableAPI

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class Notification`

## Properties

* `CellEvent` \([CellEvent](./CellEvent.md)\): Represents the cell event information
* `CellInfo`\([CellInfo](./CellInfo.md)\): The cell information.



# Enum NotificationType

Contains available notification types.

**Namespace: **Flux.SDK.DataTableApi

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

### Syntax

`public enum NotificationType`

### Enumerator list

* `_NONE_`: Notification type not set.

* `CELL_CREATED`: Cell created notification.

* `CELL_CLIENT_METADATA_MODIFIED`: Cell metadata modified notification.

* `CELL_MODIFIED`: Cell modified notification.

* `CELL_DELETED`: Cell deleted notification.

* `_ALL`: All notification listed above are set.


# Class NotificationEventArgs

Represents the class that contain data for the Error occured event.

**Namespace: **Flux.SDK.DataTableAPI

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class NotificationEventArgs:EventArgs`

## Constructors

* `NotificationEventArgs(notification, projectId)`

 ##### Arguments

 1. `notification` \([Notification](./Notification.md#class-notification)\): The type of notification.

 2. `projectId`\(string\): The project id.

## Properties

* `Notification` \([Notification](./Notification.md#class-notification)\):  Represents the type of notification.

* `ProjectId`\(string): The project id.
