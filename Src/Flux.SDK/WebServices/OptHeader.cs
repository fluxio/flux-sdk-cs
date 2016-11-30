using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flux.SDK.Types;
using System.Runtime.Serialization;

namespace Flux.SDK.WebServices
{
    internal class OptHeader
    {
        //[JsonExtensionData]
        internal Dictionary<string, object> OptParams;
        [DataMember(Name = "ClientInfo")]
        internal ClientInfo ClientInfo;

        public OptHeader(Dictionary<string, object> optParams, ClientInfo clientInfo)
        {
            OptParams = optParams;
            ClientInfo = clientInfo;
        }

        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(string.Format("\"{0}\":{1}","ClientInfo", Flux.Serialization.DataSerializer.Serialize(ClientInfo)));
            if (OptParams !=null)
            {
                foreach (var item in OptParams)
                {
                    sb.Append(string.Format(", \"{0}\":{1}", item.Key, Flux.Serialization.DataSerializer.Serialize(item.Value)));
                }
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
