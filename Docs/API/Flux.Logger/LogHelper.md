# Class LogHelper

Creates and manages instances of Logger objects.

**Namespace: **Flux.Logger

**Assembly: **Flux.Logger \(in Flux.SDK.dll\)

## Syntax

`public class Project`

## Methods
* `GetLogger(name)`: Gets the specified named logger.
  #### Arguments

  1.`name`\(string\): Name of the logger..  

  ##### Returns
  \([ILogger](./ILogger.md)\) IFluxLogger that contains logger reference. Multiple calls to GetLogger with the same argument aren't guaranteed to return the same logger reference.