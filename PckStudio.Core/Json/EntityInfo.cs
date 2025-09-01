using System;
using Newtonsoft.Json;

namespace PckStudio.Core.Json
{
    public class EntityInfo
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("internalName")]
        public string InternalName { get; set; }

        public EntityInfo(string displayName, string internalName)
        {
            DisplayName = displayName;
            InternalName = internalName;
        }
    }
}
