using System.Collections.Generic;

namespace Flux.Logger
{
    /// <summary>Creates and manages instances of Logger objects.</summary>
    public static class LogHelper
    {
        private static Dictionary<string, ILogger> _logCache = new Dictionary<string, ILogger>();
        /// <summary>Gets the specified named logger.</summary>
        /// <param name="name">Name of the logger.</param>
        /// <returns>Flux.Logger.IFluxLogger that contains logger reference. Multiple calls to GetLogger with the same argument aren't guaranteed to return the same logger reference.</returns>
        public static ILogger GetLogger(string name)
        {
            if (!_logCache.ContainsKey(name))
                _logCache.Add(name, new FluxLogger(name));
            return _logCache[name];
        }
    }
}