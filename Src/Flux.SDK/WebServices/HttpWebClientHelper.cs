using Flux.SDK.DataTableAPI.DatatableTypes;
using Flux.Logger;
using Flux.SDK.Properties;
using Flux.SDK.Types;
using Flux.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Flux.SDK.WebServices
{
    internal static class HttpWebClientHelper
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK|HttpWebClient");

        private const int CHUNK_SIZE = 256;

        private const string OPT_HEADER = "Flux-Options";
        private const string AUX_HEADER = "Flux-Auxiliary-Return";

        public static HttpWebRequest CreateRequest(SDKMetadata sdkMetadata, string relativeUrl, List<FluxCookie> cookies)
        {
            log.Info("HttpWebRequest creation: {0}", relativeUrl);

            var requestUri = new Uri(sdkMetadata.BaseUri, relativeUrl);
            var request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json, application/xml, text/json, text/x-json, text/javascript, text/xml";
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;

            //adding headers
            foreach (var data in sdkMetadata.FluxHeaders)
                request.Headers.Add(data.Key, data.Value);

            request.CookieContainer = new CookieContainer();
            //adding user cookies
            if (cookies != null)
            {
                foreach (var cookie in cookies)
                    request.CookieContainer.Add(new Cookie(cookie.CookieName, cookie.CookieValue) { Domain = cookie.CookieDomain });

                //adding user-specified flux request token
                var authToken = cookies.First(el => el.CookieName == FluxApiData.CookiesTokenName);
                if (authToken != null)
                    request.Headers.Add(FluxApiData.HeadersRequestToken, authToken.CookieValue);
            }

            return request;
        }

        public static HttpWebResponse GetResponse(HttpWebRequest request)
        {
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebResponse = (HttpWebResponse)request.GetResponse();
                if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.OK)
                    log.Info("Request executed successfully with code {0}.", httpWebResponse.StatusCode);
            }
            catch (WebException ex)
            {
                HandleWebException(ex);
            }

            return httpWebResponse;
        }

        public static async Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request)
        {
            HttpWebResponse httpWebResponse = null;
            try
            {
                var response = await request.GetResponseAsync();
                httpWebResponse = (HttpWebResponse)response;
                if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.OK)
                    log.Info("Request executed successfully with code {0}.", httpWebResponse.StatusCode);
            }
            catch (WebException ex)
            {
                HandleWebException(ex);
            }

            return httpWebResponse;
        }

        public static async Task<string> GetResponseStringAsync(HttpWebResponse response)
        {
            string responseStr = string.Empty;
            if (response != null)
            {
                using (var stream = StreamUtils.GetDecompressedResponseStream(response))
                {
                    var reader = new StreamReader(stream);
                    responseStr = await reader.ReadToEndAsync();
                }
            }

            return responseStr;
        }

        public static string GetResponseString(HttpWebResponse response)
        {
            string responseStr = string.Empty;
            if (response != null)
            {
                using (var stream = StreamUtils.GetDecompressedResponseStream(response))
                {
                    var reader = new StreamReader(stream);
                    responseStr = reader.ReadToEnd();
                }
            }

            return responseStr;
        }

        internal static async Task WriteToRequestAsync(HttpWebRequest request, ProgressStream progressStream)
        {
            log.Info("Writing data to request asynchronously.");

            if (progressStream == null)
                return;
            try
            {
                log.Info("Reporting progress requested.");
                long length = progressStream.Length;
                var cancellationToken = new CancellationToken();
                var buffer = new byte[CHUNK_SIZE];

                using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    progressStream.Position = 0;

                    int read = 0;
                    for (int i = 0; i < length; i += read)
                    {
                        read = await progressStream.ReadAsync(buffer, 0, CHUNK_SIZE, cancellationToken).ConfigureAwait(false);
                        await requestStream.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                        await requestStream.FlushAsync(cancellationToken).ConfigureAwait(false); // flushing is required or else we jump to 100% very fast

                        progressStream.Position += read;
                    }
                }
            }
            catch (WebException ex)
            {
                HandleWebException(ex);
            }
        }

        internal static void WriteToRequest(HttpWebRequest request, ProgressStream progressStream)
        {
            log.Info("Writing data to request synchronously.");

            if (progressStream == null)
                return;

            try
            {
                log.Info("Reporting progress requested.");

                var buffer = new byte[CHUNK_SIZE];
                using (var req = request.GetRequestStream())
                {
                    progressStream.Position = 0;

                    int read = 0;
                    for (int i = 0; i < progressStream.Length; i += read)
                    {
                        read = progressStream.Read(buffer, 0, CHUNK_SIZE);
                        req.Write(buffer, 0, read);
                        req.Flush(); // flushing is required or else we jump to 100% very fast
                    }

                    progressStream.ReportMaxValue();
                }
            }
            catch (WebException ex)
            {
                HandleWebException(ex);
            }
        }

        internal static HttpWebRequest UpdateRequestWithOpt(HttpWebRequest request, ClientInfo clientInfo, Dictionary<string, object> optParams = null)
        {
            log.Debug("Updating request with opt parameters.");
            var jsonObj = new OptHeader(optParams, clientInfo);
            request.Headers.Add(OPT_HEADER, Utils.EncodeHeader(jsonObj));
            return request;
        }

        internal static T ExtractAuxParams<T>(HttpWebResponse response)
        {
            log.Debug("Extracting aux parameters.");
            var auxHeader = response.GetResponseHeader(AUX_HEADER);
            var auxJsonStr = string.IsNullOrEmpty(auxHeader) ? null : Utils.DecodeHeader(auxHeader);
            var auxObj = DataSerializer.Deserialize<T>(auxJsonStr);
            return auxObj;
        }

        private static void HandleWebException(WebException ex)
        {
            var httpResponse = (HttpWebResponse)ex.Response;
            if (httpResponse == null)
            {
                if (ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.NameResolutionFailure ||
                    ex.Status == WebExceptionStatus.ConnectFailure)
                    throw new Exceptions.ConnectionFailureException(ex);
                else
                    throw ex;
            }

            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                var content = new StreamReader(StreamUtils.GetDecompressedResponseStream(httpResponse)).ReadToEnd();
                log.Error(content);
                log.Error("Unable to get data from server. Server returns {0}", httpResponse.StatusCode);

                switch (httpResponse.StatusCode)
                {
                    case 0:
                        break;
                    case HttpStatusCode.NoContent:
                        break; //204 return null for empty response
                    case HttpStatusCode.Unauthorized: //401
                        {
                            throw new Exceptions.UnathorizedException();
                        }
                    case HttpStatusCode.Forbidden: //403
                        {
                            throw new Exceptions.ForbiddenException();
                        }
                    case HttpStatusCode.NotFound: //404
                        {
                            throw new Exceptions.NotFoundException();
                        }
                    case HttpStatusCode.ServiceUnavailable: //503
                    case HttpStatusCode.InternalServerError: //500
                        {
                            throw new Exceptions.ServerUnavailableException(ex, (int)httpResponse.StatusCode);
                        }
                    case HttpStatusCode.RequestTimeout: //408
                        {
                            throw new Exceptions.ConnectionFailureException(ex, (int)httpResponse.StatusCode);
                        }
                    default: // other errors
                        {
                            throw ex;
                        }
                }

                log.Error(httpResponse.StatusDescription);
            }
        }
    }
}