using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.Internal.Json
{
    internal class JsonTileInfo
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("internalName")]
        public string InternalName { get; set; }

        [JsonProperty("hasColourEntry", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool HasColourEntry { get; set; }

        [JsonProperty("colourEntry", DefaultValueHandling = DefaultValueHandling.Populate)]
        public JsonColorEntry ColourEntry { get; set; }

        public JsonTileInfo(string displayName, string internalName)
        {
            DisplayName = displayName;
            InternalName = internalName;
        }
    }
}
