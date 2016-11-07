# Working with Projects and Cells

Once user was logged in we can get access to all projects available for the current user using using `Projects` property

Lets print all available projects for current user:

```c#
for (var project in SDK.CurrentUser.Projects)
  Console.WriteLine("Project id = {0}, name = '{1}'", project.Id, project.Name);
```
SDK provides ability to manipulate with user project: crete new project or remove existing. Note: if project was removed by SDK it can't be restored.

Lets create new project for tests:
```c#
Types.Project testProject = SDK.CurrentUser.CreateNewProject(<put new project name here>);
```
Now we have empty `testProject` which will be used in our following examples.

Next step is to get access to Cells in our project. All projects contains`DataTable` property which provides access to operation with cells

Lets create new cell with name "Test Cell" and value "Test data" in the test project:

```c#
 var clientMetadata = new ClientMetadata
    {
       Label = "Test Cell",
       Description = "Created by my app",
       Locked = false
     };

DataTableAPI.CellInfo TestCell = testProject.DataTable.CreateCell(value, clientMetadata);
```

Now lets check what is stored in our new cell:

```c#
var value = project.DataTable.GetCell(TestCell).Value;
Console.WriteLine("Cell value: {0}", value.AsString());

```

This code will get access to the cell, retrieves CellValue which contains Stream with value and reads it into the string variable.