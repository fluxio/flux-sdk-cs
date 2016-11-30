using System;
using System.Net;

namespace Flux.SDK.Types
{
    /// <summary>Represents the user cookies</summary>
    [Serializable]
    internal class FluxCookie
    {
        //Hotfix for compatibility with SDK 1.*. Property name can't be changed (including capitalization style).
        private string Name;
        private string Domain;
        private string Value;

        /// <summary>
        /// Initializes new FluxCookie instance
        /// </summary>
        public FluxCookie() { }

        /// <summary>
        /// Initializes new FluxCookie instance
        /// </summary>
        /// <param name="cookie">Cookie to initialize the cuurent instance from</param>
        public FluxCookie(Cookie cookie)
        {
            Name = cookie.Name;
            Value = cookie.Value;
            Domain = cookie.Domain;
        }

        /// <summary>The name of the cookie</summary>
        public string CookieName { get { return Name; } set { Name = value; } }

        /// <summary>The domain of the cookie</summary>
        public string CookieDomain { get { return Domain; } set { Domain = value; } }

        /// <summary>The value of the cookie</summary>
        public string CookieValue { get { return Value; } set { this.Value = value; } }
    }
}
