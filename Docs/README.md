# Introduction

* [Before You Begin](#before-you-begin)
* [Software License](#software-license)
* [Troubleshooting and Help](#troubleshooting-and-help)

&nbsp;

Welcome Flux developer! The [Flux SDK](https://github.com/fluxio/flux-sdk-cs) connects your app to popular design tools like SketchUp, Revit, Excel, Google Sheets, Grasshopper, Dynamo, 3ds Max and AutoCAD. You can also exchange data with other 3rd party apps that use the Flux SDK.

By connecting to Flux, your app will have direct access to projects and data created by Flux users (once they've logged in with their Flux credentials and authorized your app). This tutorial is designed to teach you how to do exactly that. We're going to start simple, and work up to a fully-functional app chapter by chapter. By the end of the tutorial, you'll have a clear understanding of how to leverage the most powerful features of Flux.

## Before You Begin

To get started, you will need to register your own Flux account, and create an app in the [App Manager](https://flux.io/developer/apps).

Each app you create will be assigned a unique client ID and secret. The client ID is used to identify API requests made by your app. It is important that each app has its own client ID, since your users will be prompted to authorize access to their data. The client secret is used to sign API requests made by your app. You will only be required to use your client secret in certain cases. For example, server to server communications.

You can use your client ID with the tutorial code, or skip ahead to writing your own code by grabbing the SDK from our [GitHub repo](https://github.com/fluxio/flux-sdk-cs).

## Software License

You own your code. You're also welcome to redistribute and modify Flux SDK libraries in your own apps under the MIT software license, which is further explained in [LICENSE.md](https://github.com/fluxio/flux-sdk-cs/blob/master/LICENSE.md).

## Troubleshooting and Help

As you become more familiar with the sample code, have a look at [API Reference](./API/README.md) for more in-depth detail. If you run into trouble, need some friendly advice, or have something to share: head over to [https://community.flux.io/developers](https://community.flux.io/developers). For urgent matters, like server outages or security concerns please contact [sdk@flux.io](mailto:sdk@flux.io). Thanks in advance for being a great citizen, and know that we're always striving to improve. Good luck!
