using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PckStudio.Internal.Json
{
    internal class JsonModelMetaData
    {
        [JsonProperty("textureLocations", Required = Required.Always)]
        public string[] TextureLocations { get; set; }

        [JsonProperty("root", NullValueHandling = NullValueHandling.Ignore)]
        public ReadOnlyDictionary<string, JArray> RootParts { get; set; } = new ReadOnlyDictionary<string, JArray>(new Dictionary<string, JArray>());
    }
}
