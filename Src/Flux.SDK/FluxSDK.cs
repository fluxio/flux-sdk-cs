using Flux.Logger;
using Flux.SDK.Properties;
using Flux.SDK.Types;
using Flux.SDK.WebServices;
using Flux.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Flux.SDK
{
    /// <summary> A delegate type for hooking up User logged in notifications. </summary>
    public delegate void OnUserLoggedInHandler(User user);

    /// <summary> A delegate type for hooking up User logged out notifications. </summary>
    public delegate void OnUserLoggedOutHandler();

    /// <summary>Provides access to work with Flux.</summary>
    public class FluxSDK
    {        
        private static readonly ILogger log = LogHelper.GetLogger("SDK");

        private SDKMetadata sdkMetadata;
        private User currentUser;
        
        #region Constructors
        /// <summary>Creates instanse of Flux SDK</summary>
        /// <param name="clientId">The client ID provided to you when you created your app.</param>        
        /// <param name="clientVersion">Version of the your app (optional).</param>
        /// <param name="additionalClientData">HostProgramVersion and HostProgramMainFile are required.</param>
        /// <param name="fluxUrl">Use to override the base Flux URL, e.g., for testing purposes. (optional)</param>
        public FluxSDK(string clientId, string clientVersion = null, Dictionary<string, string> additionalClientData = null, Uri fluxUrl = null)
        {
            log.Info("Creating SDK instance. SDK version: {0}", Constants.ASSEMBLY_VERSION);

            if (string.IsNullOrEmpty(clientId))
            {
                log.Fatal("Unable to create Flux SDK. ClientId can't be null or empty");
                throw new Exceptions.InternalSDKException("ClientId can't be null or empty.");
            }

            log.Info("Getting Flux Server URL");
            Uri serverUri;
            if (fluxUrl == null)
                serverUri = Utils.GetServerUri();
            else
            {
                log.Info("Flux URL is set by constructor to {0}", fluxUrl);
                serverUri = fluxUrl;
            }

            log.Info("Initializing ClientInfo and PluginMetadata");
            sdkMetadata = new SDKMetadata(serverUri, new ClientInfo(clientId, clientVersion, additionalClientData));

            //try to init user from cookies
            InitUserFromCookies(clientId);
            log.Info("SDK instance successfully created.");
        }
        #endregion

        #region Properties
        /// <summary>Returns currently logged in user information.</summary>
        public User CurrentUser
        {
            get { return currentUser; }
            internal set
            {
                if (value == null)
                    currentUser.Cookies = null;

                currentUser = value;

                if (currentUser != null)
                {
                    if (onUserLogin != null)
                        onUserLogin(currentUser);
                }
                else
                {
                    if (OnUserLogout != null)
                        OnUserLogout();
                }
            }
        }

        /// <summary>Base server uri.</summary>
        public Uri FluxUri
        {
            get
            {
                return sdkMetadata.BaseUri;
            }
        }

        /// <summary>Headers to be sent with all sdk requests to the server.</summary>
        public Dictionary<string, string> FluxHeaders
        {
            get { return sdkMetadata.FluxHeaders; }
            set { sdkMetadata.FluxHeaders = value; }
        }

        /// <summary>Allows plugin to set up file host name before pushing data.</summary>
        public Dictionary<string, string> AdditionalClientData
        {
            get { return sdkMetadata.ClientInfo.AdditionalClientData; }
            set
            {
                sdkMetadata.ClientInfo.AdditionalClientData = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>Log in user using OIDC code flow.</summary>
        /// <param name="clientSecret">ClientSecret to be used to request a token.</param>
        /// <param name="pluginInfoUrl">Info url to navigate to after login.</param>
        /// <exception cref="Exceptions.AuthorizationFailedException">Throws if OIC authorization process failed.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public void Login(string clientSecret, string pluginInfoUrl)
        {
            log.Info("Trying to login.");

            //get current user
            if (currentUser != null)
            {
                log.Info("Already logged in as {0}.", currentUser.FullName);
                return;
            }

            var oic = new OIC.CodeFlowOIC(pluginInfoUrl);
            oic.OnOICCallbackReceived += OnOICCallbackReceived;
            oic.LoginViaOIC(clientSecret, sdkMetadata);
        }

        /// <summary>Logout the current user, clears saved cookies.</summary>
        public void Logout()
        {
            log.Info("Logged out from flux.");

            if (CurrentUser == null)
                return;

            CurrentUser = null;

            //remove OIC cookies
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                FluxApiData.FluxCookiesData);

            Utils.RemoveOICCookies(path, sdkMetadata.ClientInfo.ClientId);

            if (OnUserLogout != null)
                OnUserLogout();
        }

        /// <summary>Obtain plugin versions supported by Flux</summary>
        /// <returns>List of products and versions currently supported by Flux.</returns>
        /// <exception cref="Exceptions.FluxException">Throws for internal SDK exceptions (Network is down, etc.).</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public PluginVersions GetSupportedVersions()
        {
            log.Info("Getting Supported Versions data");
            var request = HttpWebClientHelper.CreateRequest(sdkMetadata, FluxApiData.EnvUrl, null);
            request.Method = "GET";

            try
            {
                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    return DataSerializer.Deserialize<PluginVersions>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }
        }

        #endregion

        #region Events
        private OnUserLoggedInHandler onUserLogin;
        /// <summary>Occurs when user is logged in to Flux.</summary>
        public event OnUserLoggedInHandler OnUserLogin
        {
            add
            {
                if (value != null)
                {
                    onUserLogin += value;
                    if (currentUser != null && onUserLogin.GetInvocationList().Length == 1)
                        value(currentUser);
                }
            }
            remove
            {
                if (onUserLogin != null)
                    onUserLogin -= value;
            }
        }

        
        /// <summary>Occurs when user is logged out.</summary>
        public event OnUserLoggedOutHandler OnUserLogout;

        #endregion

        #region Private
        /// <summary>Trying to init user from cookies.</summary>
        /// <param name="clientId">ClientId of the application.</param>
        private void InitUserFromCookies(string clientId)
        {
            try
            {
                log.Info("Trying to load cookies.");
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    FluxApiData.FluxCookiesData);
                var cookies = Utils.LoadOICCookies(path, clientId);

                if (cookies != null)
                    CurrentUser = User.GetWhoAmI(cookies, sdkMetadata);
            }
            catch
            {
                currentUser = null;
            }
        }

        private void OnOICCallbackReceived()
        {
            log.Info("OIC Callback Received.");
            InitUserFromCookies(sdkMetadata.ClientInfo.ClientId);
        }
        #endregion
    }
}