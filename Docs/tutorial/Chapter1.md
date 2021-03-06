#                                                                   Getting started with Flux SDK.

First step to start using Flux SDK in your project is to add reference to Flux.SDK.dll to your project using `Project -> Add Reference...` command in Visual Studio.

Then add Flux.SDK namespace to usings section or the project:

```c#
using Flux.SDK;
```

Flux.SDK namespace contains set of classes which provides access to general operation with Flux SDK and useful links to Flux servers.
Next step is to create instance of FluxSDK and log in to server.

```c#
FluxSDK SDK = new FluxSDK(<put your client id here>, <put your client verion here>);

```
Instruction how to get client id can be found [here](../README.md#before-you-begin).

When SDK is created it is time to login into Flux. Flux is using OpenId for application login. Login process consist of 2 parts: subscribe to `OnLogin` event and call `Login()` method. `Login` method works assincroniously and don't stop application execution. On Login call user browser with Flux login page will  be opened and user will be asked to provide permission for access data on Flux. On successfull login,`OnLogin` event will be raised.
After successfull login, user will be logged in automatically after SDK creation, so call `Login` method is not required. If user should be asked to login each time, do not forget to call `Logout` before closing application.

```c#

private void Flux_OnUserLogin(User user)
{
   Console.WriteLine("Hello {0}!", user.First name);
}

SDK.OnLogin += Flux_OnUserLogin;
SDK.Login(<put client secret here>,<put application url here>);
```

Once permissions will be granted, browser will be navigated to the application url provided as 2nd parameter of login method.
Note: no need to store instanse of user class from `OnLogin` callback. It can be accessed any time using `SDK.CurrentUser` property.