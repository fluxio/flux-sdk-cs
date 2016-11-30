using NLog;
using NLog.Config;
using System.IO;
using System.Reflection;

namespace Flux.Logger
{
    internal class FluxLogger : ILogger
    {
        private static bool _isInitialized = false;
        private NLog.Logger _logger = null;

        internal FluxLogger(string name)
        {
            _logger = GetLogger(name);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void Info<T>(T value)
        {
            _logger.Info<T>(value);
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Trace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }

        public void Trace<T>(T value)
        {
            _logger.Trace<T>(value);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void Debug<T>(T value)
        {
            _logger.Debug<T>(value);
        }

        private static NLog.Logger GetLogger(string name)
        {
            if (!_isInitialized)
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "Flux.config");
                if (File.Exists(path))
                {
                    try
                    {
                        LogManager.Configuration = new XmlLoggingConfiguration(path, true);
                    }
                    catch
                    {
                        LogManager.Configuration = new LoggingConfiguration();
                    }
                }
                else
                {
                    LogManager.Configuration = new LoggingConfiguration();
                }
                _isInitialized = true;
            }
            return LogManager.GetLogger(name);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Warn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        public void Warn<T>(T value)
        {
            _logger.Warn<T>(value);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void Error<T>(T value)
        {
            _logger.Error<T>(value);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }

        public void Fatal<T>(T value)
        {
            _logger.Fatal<T>(value);
        }
    }
}

