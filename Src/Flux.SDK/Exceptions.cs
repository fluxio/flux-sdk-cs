using System;
using System.Net.Cache;
using Flux.SDK.DataTableAPI;

namespace Flux.SDK
{
    /// <summary>Provides SDK specific exceptions</summary>
    public class Exceptions
    {
        /// <summary>
        /// Basic class for informative SDK Exceptions (Network is down, etc.).
        /// </summary>
        public abstract class FluxException : Exception
        {
            /// <summary>Initializes a new instance of the FluxException class with a specified error message.</summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            protected FluxException(string message)
                : base(message)
            {
            }
            /// <summary>Initializes a new instance of the FluxException class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
            /// /// <param name="message">The error message that explains the reason for the exception.</param>
            /// <param name="innerException">The exception that is the cause of the current exception</param>
            protected FluxException(string message, Exception innerException)
                : base(message, innerException)
            { }
        }

        /// <summary>
        /// Basic class for fatal SDK Exceptions.
        /// </summary>
        public abstract class InternalException : Exception
        {
            /// <summary>Initializes a new instance of the InternalException class with a specified error message.</summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            protected InternalException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// Throws for unhandled SDK exceptions.
        /// </summary>
        public class InternalSDKException : InternalException
        {
            internal InternalSDKException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// Throws if required Capability is not supported.
        /// </summary>
        public class UnsupportedCapabilityException : FluxException
        {
            internal UnsupportedCapabilityException(Capability capability)
                : base(string.Format("Unsupported capability: {0}.", capability))
            {
            }
        }

        /// <summary>
        /// Throws if server returns error code.
        /// </summary>
        public abstract class NetworkException : FluxException
        {
            /// <summary>Represents Error code of Network Exception.</summary>
            public int ErrorCode { get; private set; }

            internal NetworkException(string message, int errorCode)
                : base(message)
            {
                ErrorCode = errorCode;
            }

            internal NetworkException(string message, Exception innerException)
                : base(message, innerException)
            {
                ErrorCode = innerException.HResult;
            }

            internal NetworkException(string message, Exception innerException, int errorCode)
                : base(message, innerException)
            {
                ErrorCode = errorCode;
            }
        }

        /// <summary>
        /// Throws if OIC authorization process failed.
        /// </summary>
        public class AuthorizationFailedException : FluxException
        {
            internal AuthorizationFailedException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// Throws if network connection is down.
        /// </summary>
        public class ConnectionFailureException : NetworkException
        {

            internal ConnectionFailureException(Exception innerException)
                : base("Could not connect to Flux. Please check your internet connection and try again.", innerException)
            {
            }

            internal ConnectionFailureException(Exception innerException, int errorCode)
                : base("Could not connect to Flux. Please check your internet connection and try again.", innerException, errorCode)
            {
            }
        }

        /// <summary>
        /// Throws if provided cookies were obsolete.
        /// </summary>
        public class UnathorizedException : NetworkException
        {
            internal UnathorizedException()
                : base("Authentification is required to get access to the resource. Please log in (relogin) to get access.", 401)
            {
            }

            internal UnathorizedException(string message)
                : base(message, 401)
            {
            }
        }

        /// <summary>
        /// Throws if the project/cell is readonly.
        /// </summary>
        public class ForbiddenException : NetworkException
        {
            internal ForbiddenException()
                : base("User doesn't have permission to access the resource.", 403)
            {
            }

            internal ForbiddenException(string message)
                : base(message, 403)
            {
            }
        }

        /// <summary>
        /// Throws if the requested data was moved or deleted.
        /// </summary>
        public class NotFoundException : NetworkException
        {
            internal NotFoundException()
                : base("Requested data might be moved or deleted.", 404)
            {
            }

            internal NotFoundException(string message)
                : base(message, 404)
            {
            }
        }

        /// <summary>
        /// Throws if Flux server is down.
        /// </summary>
        public class ServerUnavailableException : NetworkException
        {
            internal ServerUnavailableException()
                : base("Server is temporary down. Please try later.", 500)
            {
            }

            internal ServerUnavailableException(Exception innerException, int errorCode)
                : base(innerException.Message, innerException, errorCode)
            {
            }
        }
    }
}
