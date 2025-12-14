using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.Core.Json
{
    public class JsonColorEntry
    {
        [JsonProperty("defaultName", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string DefaultName { get; set; }

        [JsonProperty("isWaterColour", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsWaterColour { get; set; }

        [JsonProperty("variants", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Variants { get; set; } = Array.Empty<string>();
    }
}
