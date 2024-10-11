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
    internal class ModelMetaDataPart
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ModelMetaDataPart[] Children { get; set; } = Array.Empty<ModelMetaDataPart>();
    }

    internal class JsonModelMetaData
    {
        [JsonProperty("textureLocations", Required = Required.Always)]
        public string[] TextureLocations { get; set; }

        [JsonProperty("parts", NullValueHandling = NullValueHandling.Ignore)]
        public ModelMetaDataPart[] RootParts { get; set; } = Array.Empty<ModelMetaDataPart>();
    }
}
