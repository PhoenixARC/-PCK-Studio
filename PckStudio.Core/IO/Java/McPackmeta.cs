using System.Drawing;
using Newtonsoft.Json;

namespace PckStudio.Core.IO.Java
{
    public struct McPackmeta
    {
        [JsonProperty("pack")]
        public McPack Pack;
        public struct McPack
        {
            [JsonIgnore]
            public Image Icon;
            
            [JsonIgnore]
            public int Format => _format;
            
            [JsonProperty("description")]
            public string Description;

            [JsonProperty("pack_format")]
            private int _format;
        }
    }
}
