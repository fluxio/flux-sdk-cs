# Static Class StreamUtils

Contains useful methods for Stream.

**Namespace: **Flux.SDK

**Assembly: **Flux.SDK \(in Flux.SDK.dll\)

## Methods

* `static GetDecompressedResponseStream(response)`: Decompresses response stream.

  ##### Arguments

  1. `response` \(HttpWebResponse\): ClientSecret to be used to request a token.

  ##### Returns

  \(Stream\) Decompressed response stream.

* `static GenerateStreamFromString(source)`: Generates stream from the specified string


  ##### Arguments

  1.  `source` \(String\): The string to generate stream from.

  ##### Returns

  \(Stream\) Generated stream.

* `static GetStringFromStream(source, length)`: Generates string from the specified stream.

  ##### Arguments

  1.  `source` \(String\): The string to generate stream from.
  2.  `length` \(int\): The stream length \(optional\).

  ##### Returns

  \(string\) Generated string.

* `static GetStringFromStreamAsync(source, length)`: Generates string from the specified stream asynchronously.

  ##### Arguments

  1.  `source` \(String\): The string to generate stream from.
  2.  `length` \(int\): The stream length \(optional\).


  ##### Returns

  \(Task&lt;string&gt;\) Generated string.


