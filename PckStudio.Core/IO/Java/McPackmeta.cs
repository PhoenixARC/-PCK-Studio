using Newtonsoft.Json;

namespace PckStudio.Core.IO.Java
{
    struct McPackmeta
    {
        [JsonProperty("pack")]
        public McPack Pack;
        public struct McPack
        {
            [JsonProperty("pack_format")]
            public int Format;
            [JsonProperty("description")]
            public string Description;
        }
    }
}
