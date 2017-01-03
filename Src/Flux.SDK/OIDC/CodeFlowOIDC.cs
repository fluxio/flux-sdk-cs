using System;
using System.Net;
using System.Text;
using Flux.Logger;

namespace Flux.SDK.OIDC
{
    internal class CodeFlowOIDC
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK.OIC");

        private Token token;
        private Browser defaultBrowser;
        private string pluginInfoUrl;

        public Action OnOIDCCallbackReceived;

        public CodeFlowOIDC(string pluginInfoUrl)
        {
            this.pluginInfoUrl = pluginInfoUrl;
        }

        internal void LoginViaOIC(string clientSecret, SDKMetadata sdkMetadata)
        {
            try
            {
                token = new Token(clientSecret, sdkMetadata);

                //get default browser
                defaultBrowser = new Browser();
                defaultBrowser.OpenLinkInNewWindow(token.GetAuthorizationUrl());

                //init http listener
                var listener = new HttpListener();
                listener.Prefixes.Add(token.RedirectUrl);
                listener.Start();

                //start waiting for requests
                listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                var listener = (HttpListener)result.AsyncState;
                // Call EndGetContext to complete the asynchronous operation.
                HttpListenerContext context = listener.EndGetContext(result);

                HttpListenerRequest request = context.Request;
                var url = request.Url;

                if (defaultBrowser.NewWindowWasOpened)
                {
                    //stop listening to redirect uri port
                    listener.Stop();

                    defaultBrowser.CloseLoginWindow();
                    defaultBrowser.OpenLinkInNewTab(pluginInfoUrl);
                }
                else
                {
                    //navigate to plugin info url
                    string redirectHtml = "<!DOCTYPE html> <html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">" +
                                          "<head> <meta charset=\"utf-8\" />  <title></title> <script type=\"text/javascript\">" +
                                          "function getUrl() { window.location.href = '" + pluginInfoUrl +
                                          "'; } window.onload = getUrl; </script>" +
                                          "</head><body></body></html>";

                    var response = context.Response;
                    byte[] bytes = Encoding.ASCII.GetBytes(redirectHtml);
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.OutputStream.Close();

                    //stop listening to redirect uri port
                    listener.Stop();
                }

                //get token from response
                token.ObtainToken(url);

                //notify that OIDC was finished
                if (OnOIDCCallbackReceived != null)
                    OnOIDCCallbackReceived();
            }
            catch (Exceptions.FluxException)
            {
                log.Debug("Rethrow FluxException.");
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }
        }
    }
}
