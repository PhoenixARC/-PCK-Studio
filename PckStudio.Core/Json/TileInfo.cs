using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PckStudio.Json;

namespace PckStudio.Core.Json
{
    public sealed class JsonTileInfo
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("internalName")]
        public string InternalName { get; set; }

        [JsonProperty("allowCustomColour", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AllowCustomColour { get; set; }

        [JsonProperty("colorKey", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ColorKey { get; set; } = string.Empty;

        [JsonIgnore]
        public bool HasColourEntry => Tiles.ColorEntries.ContainsKey(ColorKey);

        [JsonIgnore]
        public JsonColorEntry ColorEntry => Tiles.ColorEntries[ColorKey];

        public JsonTileInfo(string displayName, string internalName)
        {
            DisplayName = displayName;
            InternalName = internalName;
        }
    }
}
