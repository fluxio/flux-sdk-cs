# Class FluxSDK

Provides access to work with Flux.

**Namespace: **Flux.SDK

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class FluxSDK`

## Constructors

* `FluxSDK(clientId, clientVersion, additionalClientData, fluxUrl)`

  ##### Arguments

  1. `clientId` \(string\): The client ID provided to you when you created your app.
  2. `clientVersion` \(string\): Version of the your app \(optional\).
  3. `additionalClientData` \(Dictionary&lt;string,string&gt;\): HostProgramVersion and HostProgramMainFile are required.
  4. `FluxUrl` \(Uri\): Use to override the base Flux URL, e.g., for testing purposes (optional)



## Properties

* `AdditionalClientData` \(Dictionary&lt;string,string&gt;\): Allows plugin to get\/set up file host name before pushing data.
* `FluxHeaders` \( _Dictionary&lt;string, string&gt;_ \):   Gets\/sets headers to be sent with all sdk requests to the server.
* `CurrentUser` \([User](../Flux.SDK/Types/User.md)\): Returns currently logged in user information.
* `FluxUri`\( Uri \): - Returns Flux server Url.

## Methods

* `Login(clientSecret, pluginInfoUrl)`: Login user using OIDC code flow.

  ##### Arguments

  1. `clientSecret`\(string\): ClientSecret to be used to request a token.
  2. `pluginInfoUrl`\(string\): Info url to navigate to after login.

  ##### Returns

  Void.

  ##### [Exceptions](./Exceptions.md)

  1. `AuthorizationFailedException`: Throws if OIC authorization process failed.
  2. `ConnectionFailureException`: Throws if network connection is down.
  3. `ServerUnavailableException`: Throws if Flux server is down.
  4. `InternalSDKException`: Throws for unhandled SDK exceptions.


* `Logout()` - Logout the current user, clears saved cookies.

  ##### Returns

  Void.

* `GetSupportedVersions()`: Obtain plugin versions supported by Flux

  ##### Returns

  \([PluginVersions](./Types/SupportedVersions.md)\) List of products and versions currently supported by Flux.

  ##### [Exceptions](./Exceptions.md)

  1. `FluxException`: Throws for internal SDK exceptions \(Network is down, etc.\).
  2. `ConnectionFailureException`: Throws if network connection is down.
  3. `ServerUnavailableException`: Throws if Flux server is down.
  4. `InternalSDKException`: Throws for unhandled SDK exceptions.


## Events

* `OnUserLogin`: Occurs when user is logged in to Flux.

  ##### Handler

  `OnUserLoggedInHandler`: A delegate type for hooking up User logged in notifications. Provides information about logged in user.

* `OnUserLogout:`Occurs when user is logged out.

  ##### Handler

  `OnUserLoggedInHandler`: A delegate type for hooking up User logged out notifications.


