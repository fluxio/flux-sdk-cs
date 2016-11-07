# Class Project

Represents the user project.

**Namespace: **Flux.SDK.Types

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class Project`

## Properties

* `CreatedDate` \(System.DateTime\): Represents creation date of the project.
* `Creator` \(string\): Represents creator of the project.
* `DataTable` \([DataTable](../DataTable/DataTable.md)\): Returns datatable for the project.
* `Id`\(string\): Represents  project id.
* `IsReadOnly` \(bool\): Represents whether project is readonly.
* `Kind`\([ProjectKind](#enum-projectkind)\): Represents the project kind.
* `Name` \(string\): Represents the project name.
* `UpdatedDate` \(System.DateTime\): Represents the project last update date.
* `UserRole` \([UserRoleType](#enum-userroletype)\): Represents the user role on the project.

## Methods

* `ConvertBrep(content, sourceFormat, targetFormat)`: Converts brep to specified format.

  #### Arguments

  1.`content`\(string\):  Brep to convert \(base64 encoded string\).

  2.`sourceFormat`\(string\): Source format of brep.

  3.`targetFormat`\(string\): Target format of brep.

  ##### Returns

  \(string\) Converted brep \(base64 encoded string\).

  ##### [Exceptions](./Exceptions.md)

  1.`ConnectionFailureException`: Throws if network connection is down.

  2.`UnathorizedException`: Throws if provided cookies were obsolete.

  3.`ServerUnavailableException`: Throws if Flux server is down.

  4.`InternalSDKException`: Throws for unhandled SDK exceptions.


* `TessellateBrep(content, sourceFormat, lod, unit)`: Tessellates brep.

  ##### Arguments

  `content`\(string\): Brep to convert \(base64 encoded string\).

  `sourceFormat`\(string\): Source format of brep.

  `lod`\(object\): Level of detail of the brep.

  `unit`\(object\): Units of the brep.

  ##### Returns

  \(string\) Tessellated brep \(base64 encoded string\).

  ##### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.

  2. `UnathorizedException`: Throws if provided cookies were obsolete.

  3. `ServerUnavailableException`: Throws if Flux server is down.

  4. `InternalSDKException`: Throws for unhandled SDK exceptions.



## Enum ProjectKind

Represents the kind of the project.

**Namespace: **Flux.SDK.Types.Project

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

### Syntax

`public enum ProjectKind`

### Enumerator list

* `None`: Project kind not set
* `Readonly`: Represents readonly access to project
* `Full`: Represents full access to project.

## Enum UserRoleType

Represents the user role on the project.

**Namespace: **Flux.SDK.Types.Project

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

### Syntax

`public enum UserRoleType`

### Enumerator list

* `None`: Project kind not set
* `Viewer`: Set if user is viewer for project
* `Collaborator`: Set if user is collaborator for project
* `Owner`: Set if user is owner for project.

