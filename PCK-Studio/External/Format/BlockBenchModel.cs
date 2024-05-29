using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PckStudio.External.Format
{
    internal class Meta
    {
        [JsonProperty("format_version")]
        internal string FormatVersion;
       
        [JsonProperty("model_format")]
        internal string ModelFormat;
       
        [JsonProperty("box_uv")]
        internal bool UseBoxUv;
    }

    internal class Element
    {
        [JsonProperty("name")]
        internal string Name;

        [JsonProperty("box_uv")]
        internal bool UseBoxUv;

        [JsonProperty("visibility", DefaultValueHandling = DefaultValueHandling.Ignore)]
        internal bool IsVisibile { get; set; } = true;

        [JsonProperty("rescale")]
        internal bool Rescale;

        [JsonProperty("mirror_uv")]
        internal bool MirrorUv;

        [JsonProperty("locked")]
        internal bool Locked;

        [JsonProperty("inflate")]
        internal float Inflate;
        
        [JsonProperty("origin")]
        private float[] origin;

        [JsonProperty("from")]
        private float[] from;

        [JsonProperty("to")]
        private float[] to;

        [JsonProperty("uv_offset")]
        private int[] uv_offset;

        [JsonIgnore()]
        internal Vector3 Origin
        {
            get
            {
                return new Vector3(origin?[0] ?? 0, origin?[1] ?? 0, origin?[2] ?? 0);
            }
            set
            {
                if (origin is null || origin.Length < 3)
                    origin = new float[3];
                origin[0] = value.X;
                origin[1] = value.Y;
                origin[2] = value.Z;
            }
        }

        [JsonIgnore()]
        internal Vector3 From
        {
            get
            {
                return new Vector3(from?[0] ?? 0, from?[1] ?? 0, from?[2] ?? 0);
            }
            set
            {
                if (from is null || from.Length < 3)
                    from = new float[3];
                from[0] = value.X;
                from[1] = value.Y;
                from[2] = value.Z;
            }
        }

        [JsonIgnore()]
        internal Vector3 To
        {
            get
            {
                return new Vector3(to?[0] ?? 0, to?[1] ?? 0, to?[2] ?? 0);
            }
            set
            {
                if (to is null || to.Length < 3)
                    to = new float[3];
                to[0] = value.X;
                to[1] = value.Y;
                to[2] = value.Z;
            }
        }

        [JsonIgnore()]
        internal Vector2 UvOffset
        {
            get
            {
                return new Vector2(uv_offset?[0] ?? 0, uv_offset?[1] ?? 0);
            }
            set
            {
                if (uv_offset is null || uv_offset.Length < 2)
                    uv_offset = new int[2];
                uv_offset[0] = (int)value.X;
                uv_offset[1] = (int)value.Y;
            }
        }

        [JsonProperty("type")]
        internal string Type;

        [JsonProperty("uuid")]
        internal Guid Uuid;
    }

    internal class Texture
    {
        public static implicit operator Texture(Image image) => new Texture(image);
        public static implicit operator Image(Texture texture) => texture.GetImage();
        
        private Texture() { }

        internal Texture(Image image)
        {
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            TextureSource = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
        }

        [JsonProperty("name")]
        internal string Name;
        
        [JsonProperty("source")]
        internal string TextureSource;

        private Image GetImage()
        {
            string data = TextureSource;
            const string dataHead = "data:image/png;base64,";
            if (data.StartsWith(dataHead))
            {
                byte[] encodedData = Convert.FromBase64String(data.Substring(dataHead.Length));
                using (var ms = new MemoryStream(encodedData))
                {
                    return Image.FromStream(ms);
                }
            }
            return null;
        }
    }

    internal class Outline
    {
        [JsonProperty("name")]
        internal string Name;

        [JsonProperty("origin")]
        private float[] origin;

        [JsonIgnore]
        public Vector3 Origin
        {
            get => new Vector3(origin?[0] ?? 0, origin?[1] ?? 0, origin?[2] ?? 0);
            set
            {
                if (origin is null || origin.Length < 3)
                    origin = new float[3];
                origin[0] = value.X;
                origin[1] = value.Y;
                origin[2] = value.Z;
            }
        }

        [JsonProperty("uuid")]
        internal Guid Uuid;
        
        [JsonProperty("children")]
        internal JArray Children;

        public Outline(string name)
        {
            Name = name;
            origin = new float[3];
            Uuid = Guid.NewGuid();
            Children = new JArray();
        }
    }

    internal class TextureRes
    {
        [JsonProperty("width")]
        internal int Width { get; set; }

        [JsonProperty("height")]
        internal int Height { get; set; }

        public TextureRes(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static implicit operator Size(TextureRes res) => new Size(res.Width, res.Height);
        public static implicit operator TextureRes(Size size) => new TextureRes(size.Width, size.Height);
    }

    internal class BlockBenchModel
    {   
        [JsonProperty("name")]
        internal string Name;

        [JsonProperty("meta")]
        internal Meta Metadata;

        [JsonProperty("model_identifier")]
        internal string ModelIdentifier { get; set; } = "";

        [JsonProperty("visible_box")]
        internal int[] VisibleBox;

        [JsonProperty("resolution")]
        internal TextureRes TextureResolution;
        
        [JsonProperty("elements")]
        internal Element[] Elements;
        
        [JsonProperty("outliner")]
        internal JArray Outliner;
        
        [JsonProperty("textures")]
        internal Texture[] Textures;
        
    }
}
