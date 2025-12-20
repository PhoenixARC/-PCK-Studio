using System.Numerics;
using Newtonsoft.Json;

namespace PckStudio.Core
{
    public class Box(Vector3 position, Vector3 size, Vector2 uv, float inflate, bool mirror)
    {
        
        [JsonProperty("pos", Required = Required.Always)]
        public Vector3 Position { get; } = position;
        
        [JsonProperty("size", Required = Required.Always)]
        public Vector3 Size { get; } = size;
        
        [JsonProperty("uv", Required = Required.Always)]
        public Vector2 Uv { get; } = uv;

        [JsonProperty("inflate", NullValueHandling = NullValueHandling.Ignore)]
        public float Inflate { get; } = inflate;

        [JsonProperty("mirror", NullValueHandling = NullValueHandling.Ignore)]
        public bool Mirror { get; } = mirror;
    }
}
