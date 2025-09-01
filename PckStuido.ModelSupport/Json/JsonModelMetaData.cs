using System;
using System.Numerics;
using Newtonsoft.Json;

namespace PckStudio.Core.Json
{
    public class ModelMetaDataPart
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ModelMetaDataPart[] Children { get; set; } = Array.Empty<ModelMetaDataPart>();

        [JsonConstructor]
        public ModelMetaDataPart()
        {
        }

        public ModelMetaDataPart(string name)
            : this(name, Array.Empty<ModelMetaDataPart>())
        {
        }

        public ModelMetaDataPart(string name, params ModelMetaDataPart[] children)
        {
            Name = name;
            Children = children;
        }
    }

    public class JsonModelMetaData
    {
        [JsonProperty("textureLocations", Required = Required.Always)]
        public string[] TextureLocations { get; set; }

        [JsonProperty("materialName", NullValueHandling = NullValueHandling.Ignore)]
        public string MaterialName { get; set; } = string.Empty;

        [JsonProperty("uv_offsets", NullValueHandling = NullValueHandling.Ignore)]
        public Vector2[] UvOffsets { get; set; } = Array.Empty<Vector2>();

        [JsonProperty("parts", NullValueHandling = NullValueHandling.Ignore)]
        public ModelMetaDataPart[] RootParts { get; set; } = Array.Empty<ModelMetaDataPart>();
    }
}
