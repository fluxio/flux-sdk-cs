# Exceptions

Contains set of SDK specific exceptions.

**Namespace**: Flux.SDK

**Assembly**: Flux.SDK \(in Flux.SDK.dll\)

* ### Abstract class FluxException

  Basic class for informative SDK Exceptions \(Network is down, etc.\).

  #### Syntax

  `abstract class FluxException : Exception`

  #### Constructors

  * `protected FluxException(message, InnerException)`

    ##### Arguments

    1.`message` \(string\): The error message that explains the reason for the exception.
    2. `InnerException`\(Exception\): The exception that is the cause of the current exception.

  * `protected FluxException(message, InnerException)`

    ##### Arguments

    1.`message` \(string\): The error message that explains the reason for the exception



* ### Abstract class InternalException

  Basic class for fatal SDK Exceptions.

  #### Syntax

  `abstract class InternalException: Exception`

  #### Constructors

  * `protected InternalException(message)`

    ##### Arguments

    1. `message` \(string\): The error message that explains the reason for the exception.



* ### Abstract Class NetworkException

  Throws if server returns error code.

  #### Syntax

  `class NetworkException: FluxException`

  #### Properties

  * `ErrorCode`\(int\): Represents Error code of Network Exception.


* ### Class InternalSDKException

  Throws for unhandled SDK exceptions.

  #### Syntax

  `class InternalSDKException: InternalException`


* ### Class UnsupportedCapabilityException

  Throws if required Capability is not supported.

  #### Syntax

  `class UnsupportedCapabilityException: FluxException`

  ### 

* ### Class AuthorizationFailedException

  Throws if OIC authorization process failed.

  #### Syntax

  `class AuthorizationFailedException: FluxException`


* ### Class ConnectionFailureException

  Throws if network connection is down.

  #### Syntax

  `class ConnectionFailureException: NetworkException`


* ### Class UnathorizedException

  Throws if provided cookies were obsolete.

  #### Syntax

  `class UnathorizedException: NetworkException`


* ### Class ForbiddenException

  Throws if the project\/cell is readonly.

  #### Syntax

  `class ForbiddenException: NetworkException`


* ### Class NotFoundException

  Throws if the requested data was moved or deleted.

  #### Syntax

  `class NotFoundException: NetworkException`


* ### Class ServerUnavailableException

  Throws if Flux server is down.

  #### Syntax

  `class ServerUnavailableException: NetworkException`


