using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.Internal.Json
{
    internal class DefaultModel
    {
        [JsonProperty("textureSize", Required = Required.Always)]
        public Vector2 TextureSize { get; set; }

        [JsonProperty("parts", Required = Required.Always)]
        public DefaultPart[] Parts { get; set; } = Array.Empty<DefaultPart>();
    }

    internal class DefaultPart
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("translation")]
        public Vector3 Translation { get; set; } = Vector3.Zero;

        [JsonProperty("rotation")]
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        [JsonProperty("boxes")]
        public ModelDefaultBox[] Boxes { get; set; }
    }

    internal class ModelDefaultBox
    {
        [JsonProperty("pos")]
        public Vector3 Position { get; set; }

        [JsonProperty("size")]
        public Vector3 Size { get; set; }

        [JsonProperty("uv")]
        public Vector2 Uv { get; set; }

        [JsonProperty("mirror")]
        public bool Mirror { get; set; } = false;

        [JsonProperty("inflate")]
        public float Inflate { get; set; } = 0f;
    }
}
