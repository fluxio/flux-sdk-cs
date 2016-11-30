using System;
using System.Collections.Generic;
using Flux.SDK.Types;
using Flux.SDK.Properties;

namespace Flux.SDK
{
    internal class SDKMetadata
    {
        public Uri BaseUri { get; private set; }
        public ClientInfo ClientInfo { get; private set; }
        public Dictionary<string, string> FluxHeaders { get; set; }

        internal SDKMetadata(Uri uri, ClientInfo clientInfo)
        {
            BaseUri = uri;
            ClientInfo = clientInfo;
            InitHeaders();
        }

        private void InitHeaders()
        {
            FluxHeaders = new Dictionary<string, string>();
            FluxHeaders[FluxApiData.HeadersRequestMarker] = "1";
            FluxHeaders[FluxApiData.HeaderAcceptEncoding] = "gzip,deflate";

            if (ClientInfo != null)
            {
                FluxHeaders[FluxApiData.HeadersPluginPlatform] = ClientInfo.OS;
                FluxHeaders[FluxApiData.HeadersHostName] = ClientInfo.ClientId;

                if (ClientInfo.AdditionalClientData != null && ClientInfo.AdditionalClientData.ContainsKey(FluxApiData.HostProgramVersion))
                    FluxHeaders[FluxApiData.HeadersHostVersion] = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ClientInfo.AdditionalClientData[FluxApiData.HostProgramVersion]));

                FluxHeaders[FluxApiData.HeadersPluginVersion] = ClientInfo.ClientVersion;
            }
        }
    }
}