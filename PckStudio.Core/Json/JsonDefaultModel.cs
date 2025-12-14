using System;
using System.Numerics;
using Newtonsoft.Json;

namespace PckStudio.Core.Json
{
    public class DefaultModel
    {
        [JsonProperty("textureSize", Required = Required.Always)]
        public Vector2 TextureSize { get; set; }

        [JsonProperty("parts", Required = Required.Always)]
        public DefaultPart[] Parts { get; set; } = Array.Empty<DefaultPart>();
    }

    public class DefaultPart
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("translation")]
        public Vector3 Translation { get; set; } = Vector3.Zero;

        [JsonProperty("rotation")]
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        [JsonProperty("boxes")]
        public Box[] Boxes { get; set; }
    }
}
