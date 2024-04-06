using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        [JsonProperty("visibility")]
        internal bool Visibility { get; set; } = true;

        [JsonProperty("rescale")]
        internal bool Rescale;

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
        internal Vector3 Origin => new Vector3(origin?[0] ?? 0, origin?[1] ?? 0, origin?[2] ?? 0);
        
        [JsonIgnore()]
        internal Vector3 From => new Vector3(from?[0] ?? 0, from?[1] ?? 0, from?[2] ?? 0);

        [JsonIgnore()]
        internal Vector3 To => new Vector3(to?[0] ?? 0, to?[1] ?? 0, to?[2] ?? 0);
        
        [JsonIgnore()]
        internal Vector2 UvOffset => new Vector2(uv_offset?[0] ?? 0, uv_offset?[1] ?? 0);

        [JsonProperty("type")]
        internal string Type;

        [JsonProperty("uuid")]
        internal Guid Uuid;
    }

    internal class Texture
    {
        [JsonProperty("name")]
        internal string Name;
        
        [JsonProperty("source")]
        internal Uri TextureSource;

        internal Image GetTexture()
        {
            string data = TextureSource.AbsolutePath;
            const string dataHead = "image/png;base64,";
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
        internal float[] Origin;
        
        [JsonProperty("uuid")]
        internal Guid Uuid;
        
        [JsonProperty("children")]
        internal Guid[] Children;
    }

    internal class TextureRes
    {
        [JsonProperty("width")]
        internal int Width;

        [JsonProperty("height")]
        internal int Height;

        public static implicit operator Size(TextureRes res) => new Size(res.Width, res.Height);
    }

    internal class BlockBenchModel
    {   
        [JsonProperty("name")]
        internal string Name;

        [JsonProperty("meta")]
        internal Meta Metadata;
        
        [JsonProperty("model_identifier")]
        internal string ModelIdentifier;

        [JsonProperty("visible_box")]
        internal int[] VisibleBox;

        [JsonProperty("resolution")]
        internal TextureRes TextureResolution;
        
        [JsonProperty("elements")]
        internal Element[] Elements;
        
        [JsonProperty("outliner")]
        internal Outline[] Outliner;
        
        [JsonProperty("textures")]
        internal Texture[] Textures;
        
    }
}
