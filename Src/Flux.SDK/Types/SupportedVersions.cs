using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flux.SDK.Types
{
    /// <summary>Versions supported by Flux</summary>
    [DataContract]
    public struct SupportedVersions
    {
        /// <summary>The minimal version supported by Flux</summary>
        [DataMember(Name = "Minimum")]
        //[Newtonsoft.Json.JsonConverterAttribute(typeof(Newtonsoft.Json.Converters.VersionConverter))]
        public Version Min;

        /// <summary>The latest version supported by Flux</summary>
        [DataMember(Name = "Latest")]
        //[Newtonsoft.Json.JsonConverterAttribute(typeof(Newtonsoft.Json.Converters.VersionConverter))]
        public Version Latest;
    }

    /// <summary>Represents set of plugins with versions supported by Flux.</summary>
    [DataContract]
    public class PluginVersions
    {
        /// <summary>Set of supported plugins with versions.</summary>
        [DataMember(Name = "PluginVersions")]
        public Dictionary<string, SupportedVersions> Versions = null;
    }
}