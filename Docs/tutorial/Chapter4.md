# Working with notifications

Flux.SDK allows to receive notification from Flux about Cell events. Following notifications are supported: cell created, cell deleted, cell modified and cell client data modified.

Lets create event hadler for notifications on cell data modified (CELL_MODIFIED) notification.

```c#
private void OnNotification(object sender, NotificationEventArgs args)
{
  //ignore all notificaitons but CELL_MODIFIED
  if (args.Notification.CellEvent.Type != NotificationType.CELL_MODIFIED)
    return;
  //lets get updated value for cell:
  Cell cellData = project.DataTable.GetCell(testCell, true, true);
  string jsonStr = StreamUtils.GetStringFromStream(cellData.Value.Stream, cellData.Value.StreamLength);
  Console.Writeline("Cell {0} was updated. New value is {1}, args.CellInfo.ClientMetadata.Label, jsonStr);
}
```

Next step is to subscribe for notificaitons.

```c#
testProject.DataTable.OnNotification += OnNotification;
project.DataTable.Subscribe(NotificationType.CELL_MODIFIED);
```

Now we can update cell data on Flux of using SDK and check that handler is called and cell lable and updated value is displayed.

To unsubscrive from notification event handler can be removed:

```C#
testProject.DataTable.OnNotification -= OnNotification;
```