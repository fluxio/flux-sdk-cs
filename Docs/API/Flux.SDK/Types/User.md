# Class User

Represents the Flux user.

**Namespace: **Flux.SDK.Types

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class User`

## Properties

* `Email` \(string\): Represents the user email.
* `FirstName` \(string\): Represents the user first name.
* `LastName`\(string\): Represents the user last name.
* `FullName` \(string\): Represents the user full name.
* `Id`\(string\): Represents the user id.
* `Kind` \(string\): Represents the user kind.
* `Projects` \(List&lt;[Project](./Project.md)&gt;\): Represents the user projects.

## Methods

* `CreateNewProject(projectName)`: Creates new project.

  ##### Arguments

  1. `projectName`\(string\): Name of the new project.

  ##### Returns

  \([Project](./Project.md)\) Project instance if project was created successfully..

  ##### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.

  2. `UnathorizedException`: Throws if provided cookies were obsolete.

  3. `ServerUnavailableException`: Throws if Flux server is down.

  4. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `CreateNewProjectAsync(projectName)`: Creates new project assincroniously.

  ##### Arguments

  1. `projectName`\(string\): Name of the new project.

  ###### Returns

  \(Task&lt;[Project&gt;](./Project.md)\) Project instance if project was created successfully.

  ###### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.

  2. `UnathorizedException`: Throws if provided cookies were obsolete.

  3. `ServerUnavailableException`: Throws if Flux server is down.

  4. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `DeleteProject(projectID)`: Deletes project by id.

  ##### Arguments

  1. `projectId`\(string\): Id of the project to be deleted.

  ##### Returns

  \(bool\) true if project was deleted successfully.

  ###### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.

  2. `UnathorizedException`: Throws if provided cookies were obsolete.

  3. `ForbiddenException`: Throws if Flux server is down.

  4. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `DeleteProjectAsync(projectID)`: Deletes project by id assincroniously .

  ##### Arguments

  1. `projectId`\(string\): Id of the project to be deleted.

  ##### Returns

  \(bool\) true if project was deleted successfully.

  ###### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.

  2. `UnathorizedException`: Throws if provided cookies were obsolete.

  3. `ForbiddenException`: Throws if Flux server is down.

  4. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `UpdateProjects()`: Updates list of projects for the current user assincroniously.

  ##### Returns

  Void.

  ###### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.

  2. `UnathorizedException`: Throws if provided cookies were obsolete.

  3. `ServerUnavailableException`: Throws if Flux server is down.

  4. `InternalSDKException`: Throws for unhandled SDK exceptions.



* `UpdateProjectsAsync()`: Creates new project.

  ##### Returns

  Void.

  ###### [Exceptions](./Exceptions.md)

  1. `ConnectionFailureException`: Throws if network connection is down.

  2. `UnathorizedException`: Throws if provided cookies were obsolete.

  3. `ServerUnavailableException`: Throws if Flux server is down.

  4. `InternalSDKException`: Throws for unhandled SDK exceptions.



