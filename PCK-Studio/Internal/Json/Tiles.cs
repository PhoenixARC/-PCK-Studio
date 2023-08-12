using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.Internal.Json
{
    internal class JsonTiles
    {
        [JsonProperty("blocks")]
        public List<JsonTileInfo> Blocks { get; set; }
        [JsonProperty("items")]
        public List<JsonTileInfo> Items { get; set; }
    }
}
