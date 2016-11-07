# Working with data on Flux

In previous chapter we recieved string value from newly created cell. In this chapter we'll work with symple geometry objects: send it to Flux, check data we send and receive it back.
First of all, lets get access to test project and test key:

```c#
Project testProject = SDK.CurrentUser.FirstOrDefault(p => p.Name == "testProject");
CellInfo testCell = project.DataTable.Cells.FirstOrDefault(c => c.ClientMetadata.Label == "test Cell");
```

Now lets create class which will represents geometry object (line) on Flux.

```c#
public class Line
{
   public string primitive 
   {
      get { return "Line"; }
   }
  public List<double> start { get; set; }
  public List<double> end { get; set; }
}
```

It is time to create line instance and send it to Flux. First step for sending data to Flux is to serialize object to string. In this example DataSerializer is used. But it can be replaced by Json serilizer you like. Next step is to generate stream from string and call `SetCell`.

```c#
  Line ln = new Line();
  ln.start = new List<double>() { 10, 0, 0 };
  ln.end = new List<double>() { 0, 10, 0 };

  //in this exemple . It can be replaced by any Json
  string serializedValueStr = Flux.Serialization.DataSerializer.Serialize(ln);
  Stream stream = StreamUtils.GenerateStreamFromString(serializedValueStr);
  CellInfo updatingKey = project.DataTable.SetCell(key.CellId, stream, key.ClientMetadata);
```

Now data was sent to Flux can be checked on https://flux.io.

Next example shows how to retrieve geometry data from Flux.

```C#
Cell cellData = project.DataTable.GetCell(testCell, true, true);
string jsonStr = StreamUtils.GetStringFromStream(cellData.Value.Stream, cellData .Value.StreamLength);
Line receivedData = Flux.Serialization.DataSerializer.Deserilize<Line>(jsonStr);
```

This example describes steps to retrieved data from the cell. First step is to call `GetCell` to get data stream. Next step is to read `Value.Stream` into variable and deserialize data using DataSerializer or other Json serializer.