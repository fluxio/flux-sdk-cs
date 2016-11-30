namespace Flux.Logger
{
    /// <summary>Provides interfaces to work with Logger.</summary>
    public interface ILogger
    {
        /// <summary>Logs specified message at the Info level.</summary>
        /// <param name="message">Log message</param>
        void Info(string message);
        /// <summary>Writes the diagnostic message at the Info level using the specified parameters.</summary>
        /// <param name="message">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Info(string message, params object[] args);
        /// <summary>Writes the diagnostic message at the Info level.</summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        void Info<T>(T value);

        /// <summary>Logs specified message at the Trace level.</summary>
        /// <param name="message">Log message</param>
        void Trace(string message);
        /// <summary>Writes the diagnostic message at the Trace level using the specified parameters.</summary>
        /// <param name="message">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Trace(string message, params object[] args);
        /// <summary>Writes the diagnostic message at the Trace level.</summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        void Trace<T>(T value);

        /// <summary>Logs specified message at the Debug level.</summary>
        /// <param name="message">Log message</param>
        void Debug(string message);
        /// <summary>Writes the diagnostic message at the Debug level using the specified parameters.</summary>
        /// <param name="message">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Debug(string message, params object[] args);
        /// <summary>Writes the diagnostic message at the Debug level.</summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        void Debug<T>(T value);

        /// <summary>Logs specified message at the Warn level.</summary>
        /// <param name="message">Log message</param>
        void Warn(string message);
        /// <summary>Writes the diagnostic message at the Warn level using the specified parameters.</summary>
        /// <param name="message">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Warn(string message, params object[] args);
        /// <summary>Writes the diagnostic message at the Warn level.</summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        void Warn<T>(T value);

        /// <summary>Logs specified message at the Error level.</summary>
        /// <param name="message">Log message</param>
        void Error(string message);
        /// <summary>Writes the diagnostic message at the Error level using the specified parameters.</summary>
        /// <param name="message">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Error(string message, params object[] args);
        /// <summary>Writes the diagnostic message at the Error level.</summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        void Error<T>(T value);

        /// <summary>Logs specified message at the Fatal level.</summary>
        /// <param name="message">Log message</param>
        void Fatal(string message);
        /// <summary>Writes the diagnostic message at the Fatal level using the specified parameters.</summary>
        /// <param name="message">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Fatal(string message, params object[] args);
        /// <summary>Writes the diagnostic message at the Fatal level.</summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        void Fatal<T>(T value);
    }
}
