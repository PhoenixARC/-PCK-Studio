using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.External.Format
{
    internal class BedrockModel
    {
        [JsonProperty("format_version")]
        public string FormatVersion { get; set; }

        [JsonProperty("minecraft:geometry")]
        public List<Geometry> Models;
    }

    internal class Geometry
    {
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public GeometryDescription Description { get; set; }

        [JsonProperty("bones")]
        public List<Bone> Bones;
    }

    internal class GeometryDescription
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("texture_width")]
        private int TextureWidth;

        [JsonProperty("texture_height")]
        private int TextureHeight;

        [JsonIgnore]
        public Size TextureSize
        {
            get => new Size(TextureWidth, TextureHeight);
            set
            {
                TextureWidth = value.Width;
                TextureHeight = value.Height;
            }
        }
    }

    internal class Bone
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public Bone(string name)
        {
            Name = name;
            Cubes = new List<Cube>();
        }

        [JsonIgnore]
        public Vector3 Pivot
        {
            get => pivot.Length < 3 ? Vector3.Zero : new Vector3(pivot[0], pivot[1], pivot[2]);
            set
            {
                if (pivot.Length < 3)
                    pivot = new float[3];
                pivot[0] = value.X;
                pivot[1] = value.Y;
                pivot[2] = value.Z;
            }
        }

        [JsonProperty("cubes")]
        public List<Cube> Cubes;

        [JsonProperty("pivot")]
        private float[] pivot { get; set; } = new float[3];
    }

    internal class Cube
    {
        [JsonProperty("origin")]
        private float[] origin { get; set; } = new float[3];
        [JsonIgnore]
        public Vector3 Origin
        {
            get => origin.Length < 3 ? Vector3.Zero : new Vector3(origin[0], origin[1], origin[2]);
            set
            {
                if (origin.Length < 3)
                    origin = new float[3];
                origin[0] = value.X;
                origin[1] = value.Y;
                origin[2] = value.Z;
            }
        }

        [JsonProperty("size")]
        private float[] size { get; set; } = new float[3];
        [JsonIgnore]
        public Vector3 Size
        {
            get => size.Length < 3 ? Vector3.Zero : new Vector3(size[0], size[1], size[2]);
            set
            {
                if (size.Length < 3)
                    size = new float[3];
                size[0] = value.X;
                size[1] = value.Y;
                size[2] = value.Z;
            }
        }

        [JsonProperty("uv")]
        private float[] uv { get; set; } = new float[2];
        [JsonIgnore]
        public Vector2 Uv
        {
            get => uv.Length < 2 ? Vector2.Zero : new Vector2(uv[0], uv[1]);
            set
            {
                if (uv.Length < 2)
                    uv = new float[2];
                uv[0] = value.X;
                uv[1] = value.Y;
            }
        }

        [JsonProperty("inflate")]
        public float Inflate { get; set; } = 0f;

        [JsonProperty("mirror", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Mirror;
    }
}
