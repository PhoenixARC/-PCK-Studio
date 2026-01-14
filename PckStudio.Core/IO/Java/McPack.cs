using System.Drawing;
using Newtonsoft.Json;

namespace PckStudio.Core.IO.Java
{
    public struct McPack
    {
        [JsonIgnore]
        public Image Icon;

        [JsonProperty("pack_format")]
        public int Format;

        [JsonProperty("description")]
        public string Description;
    }
}

