# Class ProgressStream

Provides a wrapper for System.IO.Stream which reports reading/writing progress.

**Namespace: **Flux.SDK.DataTableAPI.DataTableTypes

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class ProgressStream`

## Constructors

* `ProgressStream(sourceStream, length)`

  ##### Arguments

  1. `sourceStream` \(System.IO.Stream\): Stream to report progress for. Note: stream should support length property to report valid progress.
  2. `length` \(long\): Stream full length.



* `ProgressStream(sourceStream)`

  ##### Arguments

  1. `sourceStream` \(System.IO.Stream\): Stream to report progress for. Note: stream should support length property to report valid progress.


## Events

* `OnProgressChanged`: Reports progress when read/write operation on strem performed. Returns number of read/write bytes and stream length..

##### Handler

 [`ProgressEventHandler`](./ProgressStream.md#class-progresseventargs): Represents the method that will handle the ProgressChanged event.


# Class ProgressEventArgs

Represents the class that contain data for the Error occured event.

**Namespace: **Flux.SDK.DataTableAPI.DataTableTypes

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Syntax

`public class ProgressEventArgs:EventArgs`

## Constructors

* `ProgressEventArgs(position, length)`

  ##### Arguments

  1. `position` \(long\): Current position in steam.

  2. `length`\(long\): Stream length.

## Properties

* `Position` \(long\): Returns current position in steam.

* `Length`\(long\): Returns stream length.