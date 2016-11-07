# Interface ILogger

Provides interfaces to work with Logger..

**Namespace: **Flux.Logger

**Assembly: **Flux.Logger \(in Flux.SDK.dll\)

## Syntax

`public interface ILogger`

## Methods

* `Trace(message)`: Writes specified message at the Trace level.

  #### Arguments

  1.`message`\(string\): Log message

  ##### Returns

  Void.

* `Trace<T>(value)`: Writes the diagnostic message at the Trace level.

  #### Type Parameters:
  
  1. `T`: Type of the value.

  #### Arguments

  1.`value`\(T\): The value to be written.

  ##### Returns

  Void.

* `Trace(message, args)`: Writes the diagnostic message at the Trace level using the specified parameters.

  #### Arguments

  1. `message`\(string\): A string containing format items.
  2. `args`\(object[]\): Arguments to format.

 ##### Returns

 Void.

* `Debug(message)`: Writes specified message at the Debug level.

  #### Arguments

  1.`message`\(string\): Log message

  ##### Returns

  Void.

* `Debug<T>(value)`: Writes the diagnostic message at the Debug level.

  #### Type Parameters:

1. `T`: Type of the value.

  #### Arguments

  1.`value`\(T\): The value to be written.

  ##### Returns

  Void.

* `Debug(message, args)`: Writes the diagnostic message at the Debug level using the specified parameters.

  #### Arguments

  1. `message`\(string\): A string containing format items.

  2. `args`\(object[]\): Arguments to format.

  ##### Returns

  Void. 


* `Info(message)`: Writes specified message at the Info level.

  #### Arguments

  1.`message`\(string\): Log message

  ##### Returns

  Void.

* `Info<T>(value)`: Writes the diagnostic message at the Info level.

  #### Type Parameters:

  1. `T`: Type of the value.

  #### Arguments

  1.`value`\(T\): The value to be written.

  ##### Returns

  Void.

* `Info(message, args)`: Writes the diagnostic message at the Info level using the specified parameters.

  #### Arguments

  1. `message`\(string\): A string containing format items.

  2. `args`\(object[]\): Arguments to format.

 ##### Returns

 Void. 



* `Warn(message)`: Writes specified message at the Warn level.

  #### Arguments

  1.`message`\(string\): Log message

  ##### Returns

  Void.

* `Warn<T>(value)`: Writes the diagnostic message at the Warn level.

  #### Type Parameters:

  1. `T`: Type of the value.

  #### Arguments

  1.`value`\(T\): The value to be written.

  ##### Returns

  Void.

* `Warn(message, args)`: Writes the diagnostic message at the Warn level using the specified parameters.

  #### Arguments

  1. `message`\(string\): A string containing format items.

  2. `args`\(object[]\): Arguments to format.

 ##### Returns

 Void.


* `Error(message)`: Writes specified message at the Warn level.

  #### Arguments

  1.`message`\(string\): Log message

  ##### Returns

  Void.

* `Error<T>(value)`: Writes the diagnostic message at the Error level.

  #### Type Parameters:

  1. `T`: Type of the value.

  #### Arguments

  1.`value`\(T\): The value to be written.

  ##### Returns

  Void.

* `Error(message, args)`: Writes the diagnostic message at the Error level using the specified parameters.

  #### Arguments

  1. `message`\(string\): A string containing format items.

  2. `args`\(object[]\): Arguments to format.

 ##### Returns

 Void.

* `Error(message)`: Writes specified message at the Error level.

  #### Arguments

  1.`message`\(string\): Log message

  ##### Returns

  Void.

* `Warn<T>(value)`: Writes the diagnostic message at the Warn level.

  #### Type Parameters:

  1. `T`: Type of the value.

  #### Arguments

  1.`value`\(T\): The value to be written.

  ##### Returns

  Void.

* `Warn(message, args)`: Writes the diagnostic message at the Warn level using the specified parameters.

  #### Arguments

  1. `message`\(string\): A string containing format items.

  2. `args`\(object[]\): Arguments to format.

 ##### Returns

 Void.

* `Warn(message)`: Writes specified message at the Fatal level.

  #### Arguments

  1.`message`\(string\): Log message

  ##### Returns

  Void.

* `Warn<T>(value)`: Writes the diagnostic message at the Fatal level.

  #### Type Parameters:

  1. `T`: Type of the value.

  #### Arguments

  1.`value`\(T\): The value to be written.

  ##### Returns

  Void.

* `Fatal(message, args)`: Writes the diagnostic message at the Fatal level using the specified parameters.

  #### Arguments

  1. `message`\(string\): A string containing format items.

  2. `args`\(object[]\): Arguments to format.

 ##### Returns

 Void.