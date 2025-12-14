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

        [JsonProperty("required")]
        public bool Reqired { get; set; } = default;

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

    public class JsonModelMetaLayer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uv")]
        public Vector2 Uv { get; set; }
        
        [JsonProperty("allowedColors", NullValueHandling = NullValueHandling.Ignore)]
        public string[] AllowedColors { get; set; } = Array.Empty<string>();
    }

    public class JsonModelMetaData
    {
        [JsonProperty("textureLocations", Required = Required.Always)]
        public string[] TextureLocations { get; set; }

        [JsonProperty("materialName", NullValueHandling = NullValueHandling.Ignore)]
        public string MaterialName { get; set; } = string.Empty;

        [JsonProperty("layers", NullValueHandling = NullValueHandling.Ignore)]
        public JsonModelMetaLayer[] Layers { get; set; } =  [new JsonModelMetaLayer() { Name = "base", Uv = Vector2.Zero}];

        [JsonProperty("parts", NullValueHandling = NullValueHandling.Ignore)]
        public ModelMetaDataPart[] RootParts { get; set; } = Array.Empty<ModelMetaDataPart>();
    }
}
