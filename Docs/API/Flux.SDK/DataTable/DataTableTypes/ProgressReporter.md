# Class ProgressReporter

Reports progress to the specified UpperLimit.

**Namespace: **Flux.SDK.DataTableAPI.DataTableTypes

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class ProgressReporter`

## Constructors

* `ProgressReporter(upperLimit)`

  ##### Arguments

  1. `upperLimit` \(int\): The max value of the Progress..


## Fields

* `OnProgressChanged` \(Action&lt;int&gt;\): Reports progress change.

## Properties

* `UpperLimit` \(int\): The max value of the Progress..


## Methods

* `Report(value)` : Reports a progress update.

  #### Arguments
  
  1.`value` \(int\): The value of the updated progress.


* `Report(position, length)` : Reports a progress update.

  #### Arguments
 
  1. `position` \(long\): The current position in the progress stream.
  2. `length` \(long\): The length of the progress stream.