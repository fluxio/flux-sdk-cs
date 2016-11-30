using Flux.Logger;
using Flux.SDK.Properties;
using Flux.SDK.DataTableAPI;
using Flux.SDK.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using Flux.SDK.WebServices;
using Flux.Serialization;
using System.Runtime.Serialization;

namespace Flux.SDK
{
    //TODO: Create a self monitoring/buffering/reconnecting websocket
    internal class WebSocket
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK|Websocket");

        private const int MAX_DELAY = 60000;
        private const int MIN_DELAY = 3000;
        private const string reason = "unified";
      
        private Timer connectionTimer;
        private Timer delayTimer;
        private volatile bool requestStop = false;
        private int pingTimeout = 30000; // 10 seconds
        private int reconnectDelay = MIN_DELAY; //1 second

        private WebSocketSharp.WebSocket webSocket;

        private List<string> msgBuffer;
        private bool isConnecting = false;
        private SDKMetadata sdkMetadata;

        private FluxWebSocketStates webSocketState;
        private List<FluxCookie> userCookies;
        internal List<FluxCookie> UserCookies
        {
            get
            {
                return userCookies;
            }
            set
            {
                userCookies = value;

                if (userCookies == null)
                {
                    Close();
                    return;
                }

                if (webSocket != null)
                    Reconnect();
            }
        }

        public string ProjectId { get; private set; }

        //websocket events
        public delegate void OnOpenHandler();
        public delegate void OnCloseHandler();
        public delegate void OnReconnectedHandler();

        public event OnOpenHandler OnOpen;
        public event OnCloseHandler OnClose;
        public event OnReconnectedHandler OnReconnected;

        public Action<Notification> OnNotificationMessage;
        public Action<Error> OnErrorMessage;

        internal WebSocket(string projectId, List<FluxCookie> userCookies, SDKMetadata sdkMetadata)
        {
            log.Debug("WebSocket creation for the project {0}.", projectId);

            msgBuffer = new List<string>();
            ProjectId = projectId;
            webSocketState = FluxWebSocketStates.NotInitialized;

            this.userCookies = userCookies;
            this.sdkMetadata = sdkMetadata;

            //create and connect the websocket
            Reconnect();
        }

        public void Send(string message)
        {
            if (webSocket != null && webSocket.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                webSocket.SendAsync(message, Completed);
            }
            else
            {
                log.Debug("Websocket not yet connected. Buffering message {0}.", message);
                msgBuffer.Add(message);
            }
        }

        private void Completed(bool b)
        {
            log.Trace("Sending message task is completed.");
        }

        public void Close()
        {
            if (webSocket != null)
            {
                try
                {
                    requestStop = true;
                    webSocket.CloseAsync();

                    Stop();
                    connectionTimer = null;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            isConnecting = false;
            reconnectDelay = MIN_DELAY;
            log.Debug("Connection has been established for {0}.", ProjectId);

            //send buffered data
            foreach (var msg in msgBuffer)
                Send(msg);

            msgBuffer.Clear();

            if (webSocketState != FluxWebSocketStates.NotInitialized)
            {
                webSocketState = FluxWebSocketStates.Reconnected;

                if (OnReconnected != null)
                    OnReconnected();
            }

            if (OnOpen != null)
                OnOpen();
        }

        private void WebSocket_Closed(object sender, WebSocketSharp.CloseEventArgs e)
        {
            log.Warn("WebSocket error: {0}, {1}. Trying to reconnect...", e.Reason, e.Code);
            isConnecting = false;
            webSocketState = FluxWebSocketStates.Closed;
            if (OnClose != null)
                OnClose();
        }

        private void WebSocket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            log.Trace("WebSocket error: {0}. Trying to reconnect...", e.Message);
            isConnecting = false;
            RestartConnectionTimer();
        }

        private void WebSocket_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            //connection alive -> restart timer
            if (webSocketState != FluxWebSocketStates.Reconnecting)
                RestartConnectionTimer();

            try
            {
                //ToDo: fix
                dynamic msgObj = DataSerializer.DynamicDeserialize(e.Data);

                //playing ping-pong with server
                if (msgObj.type == "PING")
                {
                    Send("{ \"type\": \"PONG\" }");
                    return;
                }

                if (msgObj.type == "DATATABLE")
                {
                    if (msgObj.body.Type == "NOTIFICATION")
                    {
                        if (OnNotificationMessage != null)
                        {
                            var notificationStr = msgObj.body.Data.ToString();
                            OnNotificationMessage(DataSerializer.Deserialize<Notification>(notificationStr));
                        }
                    }
                    else if (msgObj.body.Type == "ERROR")
                    {
                        if (OnErrorMessage != null)
                        {
                            dynamic errMsg = msgObj.body.Data;
                            var errorMsgStr = errMsg.ToString();
                            OnErrorMessage(new Error(errorMsgStr));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void ConnectionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            log.Debug("Houston, we have a problem! Trying to reconnect...");
            Reconnect();

            if (!requestStop)
                connectionTimer.Start();
        }

        private void Stop()
        {
            requestStop = true;
            if (connectionTimer != null)
                connectionTimer.Stop();
        }

        private void Start()
        {
            requestStop = false;
            if (connectionTimer != null)
                connectionTimer.Start();
        }

        private void Reconnect()
        {
            log.Debug("Trying to reconnect websocket.");

            if (isConnecting)
                return;

            Stop();
            //close current connection

            log.Debug("Trying to close old connection.");
            Close();
            
            isConnecting = true;

            log.Debug("Start getting request to obtain ws address.");
            var request = HttpWebClientHelper.CreateRequest(sdkMetadata, string.Format(FluxApiData.WebSocketUrl, ProjectId, reason), UserCookies);
            request.Method = "GET";
            request.Headers.Add("projectId", ProjectId);

            try
            {
                var response = HttpWebClientHelper.GetResponse(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var wsUrl = DataSerializer.Deserialize<WebSocketAddress>(StreamUtils.GetDecompressedResponseStream(response));
                    log.Debug("Connecting to Flux wss: {0}", wsUrl.Address);
                    webSocket = new WebSocketSharp.WebSocket(wsUrl.Address);
                    webSocket.WaitTime = TimeSpan.FromHours(12);
                    foreach (var cookie in UserCookies)
                    {
                        WebSocketSharp.Net.Cookie wsCookie = new WebSocketSharp.Net.Cookie(cookie.CookieName, cookie.CookieValue, cookie.CookieDomain);
                        webSocket.SetCookie(wsCookie);
                    }

                    webSocket.OnMessage += WebSocket_OnMessage;
                    webSocket.OnClose += WebSocket_Closed;
                    webSocket.OnOpen += WebSocket_Opened;
                    webSocket.OnError += WebSocket_OnError;
                    webSocket.ConnectAsync();

                    //start timer
                    RestartConnectionTimer();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                webSocketState = FluxWebSocketStates.Reconnecting;

                if (ex is WebException || ex is Exceptions.ConnectionFailureException || ex is Exceptions.ServerUnavailableException)
                {
                    var webException = ex as WebException;
                    if (webException != null)
                    {
                        var webResponse = (HttpWebResponse)webException.Response;
                        if (webResponse != null)
                        {
                            Close();
                            return;
                        }
                    }

                    log.Error("Error initializing WebSocket.");
                    isConnecting = false;

                    if (reconnectDelay * 2 < MAX_DELAY)
                        reconnectDelay *= 2;
                    else
                        reconnectDelay = MAX_DELAY;

                    RetryReconnect();
                }
                else
                {
                    log.Error("Unsupported exception was caught. Websocket will be closed.");
                    Close();
                }
            }
        }

        private void RetryReconnect()
        {
            isConnecting = false;
            log.Debug("Trying to reconnect...");

            delayTimer = new Timer(reconnectDelay);
            delayTimer.Elapsed += DelayTimer_Elapsed;
            delayTimer.Start();
        }

        private void DelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            delayTimer.Stop();
            RestartConnectionTimer();
        }

        private void RestartConnectionTimer(bool reInit = false)
        {
            if (connectionTimer == null || reInit)
            {
                connectionTimer = new Timer(pingTimeout);
                connectionTimer.AutoReset = false;
                connectionTimer.Elapsed += ConnectionTimer_Elapsed;
                connectionTimer.Disposed += ConnectionTimerOnDisposed;
            }

            Stop();
            Start();
        }

        private void ConnectionTimerOnDisposed(object sender, EventArgs eventArgs)
        {
            if (!requestStop)
                RestartConnectionTimer(true);
        }

        [DataContract]
        private struct WebSocketAddress
        {
            [DataMember(Name = "wsAddr")]
            public string Address { get; set; }
        }

        internal enum FluxWebSocketStates
        {
            NotInitialized,
            Reconnecting,
            Reconnected,
            Closed
        }
    }
}