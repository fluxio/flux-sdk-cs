using Flux.Logger;
using Flux.SDK.Properties;
using System;
using System.Collections.Generic;

namespace Flux.SDK.Types
{
    /// <summary>Contains information about the client.</summary>
    public class ClientInfo
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK|ClientInfo");

        //Platform
        /// <summary>The current client OS</summary>
        public string OS { get; private set; }
        
        /// <summary>The SDK name</summary>
        public string SDKName { get; set; }
        /// <summary>The SDK version</summary>
        public string SDKVersion { get; set; }
        
        /// <summary>The user id</summary>
        public string UserId { get; set; }
        /// <summary>The user name</summary>
        public string UserName { get; set; }
        
        /// <summary>The developer id</summary>
        public string DeveloperId { get; set; }
        /// <summary>The developer name</summary>
        public string DeveloperName { get; set; }
        
        /// <summary>The client id</summary>
        public string ClientId { get; private set; }
        /// <summary>The client name</summary>
        public string ClientName { get; set; }
        /// <summary>The client version</summary>
        public string ClientVersion { get; private set; }

        /// <summary>Additional client data</summary>
        /// <remarks>HostProgramVersion and HostProgramMainFile are required, but may become optional in future</remarks>
        public Dictionary<string, string> AdditionalClientData { get; internal set; }

        private ClientInfo()
        {
            OS = string.Format("windows/Microsoft Windows [Version {0}.{1}.{2}]", Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor, Environment.OSVersion.Version.Build);
            log.Info("Current OS: {0}", OS);
            SDKName = Constants.DESCRIPTION;
            SDKVersion = Constants.ASSEMBLY_VERSION;
        }

        /// <summary>
        /// Initializes new ClientInfo instance
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <param name="clientVersion">The client version</param>
        /// <param name="additionalClientData">Additional client data</param>
        /// <remarks>Relevant if METADATA capability is supported</remarks>
        public ClientInfo(string clientId, string clientVersion, Dictionary<string, string> additionalClientData) : this()
        {
            ClientId = clientId;
            ClientVersion = clientVersion;
            AdditionalClientData = additionalClientData;

            if (additionalClientData != null && additionalClientData.ContainsKey(FluxApiData.HostProgramMainFile))
                ClientName = additionalClientData[FluxApiData.HostProgramMainFile];
        }
    }
}