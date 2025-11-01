using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.Core.Json
{
    public sealed class JsonTileInfo
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("internalName")]
        public string InternalName { get; set; }

        [JsonProperty("width")]
        public int TileWidth { get; set; } = 1;

        [JsonProperty("height")]
        public int TileHeight { get; set; } = 1;

        [JsonIgnore]
        public bool HasColourEntry => ColourEntry != null;

        [JsonProperty("colourEntry", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonColorEntry ColourEntry { get; set; }

        [JsonProperty("allowCustomColour", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AllowCustomColour { get; set; }

        public JsonTileInfo(string displayName, string internalName)
        {
            DisplayName = displayName;
            InternalName = internalName;
        }
    }
}
