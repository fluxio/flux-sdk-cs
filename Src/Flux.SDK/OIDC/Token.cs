using Flux.Logger;
using Flux.SDK.Types;
using Flux.SDK.WebServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using Flux.SDK.Properties;
using Flux.Serialization;
using System.Runtime.Serialization;

namespace Flux.SDK.OIDC
{
    [DataContract]
    internal class Token
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK.OIDC|Token");

        private readonly string clientSecret;
        private readonly SDKMetadata sdkMetadata;
        private readonly Guid nonceId;
        private readonly Guid stateId;

        #region Serializable properties

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
        [DataMember(Name = "flux_token")]
        public string FluxToken { get; set; }
        [DataMember(Name = "id_token")]
        public string IdToken { get; set; }
        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        #endregion

        internal Token()
        {
        }

        public Token(string clientSecret, SDKMetadata sdkMetadata)
        {
            this.sdkMetadata = sdkMetadata;
            this.clientSecret = clientSecret;

            stateId = Guid.NewGuid();
            nonceId = Guid.NewGuid();
        }

        public string GetAuthorizationUrl()
        {
            return string.Format("{0}authorize?redirect_uri={1}&client_id={2}&response_type=code&scope=openid&state={3}&nonce={4}",
               sdkMetadata.BaseUri,
               RedirectUrl,
               sdkMetadata.ClientInfo.ClientId,
               stateId,
               nonceId);
        }

        private string redirectUrl;
        public string RedirectUrl
        {
            get
            {
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    int port = GetFreePort();
                    redirectUrl = "http://localhost:" + port + "/";
                }

                return redirectUrl;
            }
        }

        internal void ObtainToken(Uri uri)
        {
            try
            {
                var code = HttpUtility.ParseQueryString(uri.Query).Get("code");
                var fluxToken = HttpUtility.ParseQueryString(uri.Query).Get("flux_token");
                var returnedStateId = HttpUtility.ParseQueryString(uri.Query).Get("state");

                //we MUST verify state
                if (string.IsNullOrEmpty(returnedStateId) || returnedStateId != stateId.ToString())
                    throw new Exceptions.AuthorizationFailedException("State uuids don't match.");

                //requesting access_token 
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, FluxApiData.TokenUrl, null);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var authStr = sdkMetadata.ClientInfo.ClientId + ":" + clientSecret;
                var authStrBytes = Encoding.UTF8.GetBytes(authStr);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(authStrBytes);

                string formParams = string.Format("client_id={0}&code={1}&grant_type=authorization_code&redirect_uri={2}",
                    Uri.EscapeDataString(sdkMetadata.ClientInfo.ClientId),
                    Uri.EscapeDataString(code),
                    Uri.EscapeUriString(RedirectUrl));

                byte[] bytes = Encoding.ASCII.GetBytes(formParams);
                request.ContentLength = bytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    var token = DataSerializer.Deserialize<Token>(StreamUtils.GetDecompressedResponseStream(response));

                    if (token != null && !string.IsNullOrEmpty(token.AccessToken) && !string.IsNullOrEmpty(fluxToken) && !string.IsNullOrEmpty(token.IdToken))
                    {
                        //verify nonce uuid
                        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(token.IdToken);
                        var returnedNonceId = jwtSecurityToken.Payload.Nonce;
                        if (string.IsNullOrEmpty(returnedNonceId) || returnedNonceId != nonceId.ToString())
                            throw new Exceptions.AuthorizationFailedException("Nonce uuids don't match.");

                        token.FluxToken = fluxToken;
                        Init(token);

                        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Flux\fluxData.bin");
                        if (!string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(FluxToken))
                        {
                            var cookies = new List<FluxCookie>();
                            var authCook = new FluxCookie()
                            {
                                CookieName = "auth",
                                CookieValue = Uri.UnescapeDataString(AccessToken),
                                CookieDomain = sdkMetadata.BaseUri.Host
                            };
                            cookies.Add(authCook);

                            var fluxCook = new FluxCookie()
                            {
                                CookieName = "flux_token",
                                CookieValue = Uri.UnescapeDataString(FluxToken),
                                CookieDomain = sdkMetadata.BaseUri.Host
                            };
                            cookies.Add(fluxCook);

                            Utils.StoreOICCookies(path, cookies, sdkMetadata.ClientInfo.ClientId);
                        }
                    }
                    else
                    {
                        log.Debug("Unable to receive token.");
                        throw new Exceptions.AuthorizationFailedException(
                            "Authentification has been failed. See log for more details.");
                    }
                }
            }
            catch (Exceptions.AuthorizationFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.AuthorizationFailedException(ex.Message);
            }
        }

        private static int GetFreePort()
        {
            int startPortNumber = 3000;
            int maxPortNumber = 10000;

            IPGlobalProperties ipGP = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endpoints = ipGP.GetActiveTcpListeners();

            for (int port = startPortNumber; port < maxPortNumber; port += 10)
            {
                if (endpoints.All(el => el.Port != port))
                    return port;
            }

            //In any case, let's try to use port 3000
            return startPortNumber;
        }

        private void Init(Token token)
        {
            this.AccessToken = token.AccessToken;
            this.ExpiresIn = token.ExpiresIn;
            this.FluxToken = token.FluxToken;
            this.IdToken = token.IdToken;
            this.RefreshToken = token.RefreshToken;
        }
    }
}