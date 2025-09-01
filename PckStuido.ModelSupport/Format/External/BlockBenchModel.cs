using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PckStudio.ModelSupport.Format.External
{
    internal static class BlockBenchFormatInfos
    {
        internal static readonly string FormatVersion = "4.5";

        internal static BlockBenchFormatInfo Free { get; } = new BlockBenchFormatInfo(FormatVersion, "free", true);
        internal static BlockBenchFormatInfo BedrockEntity { get; } = new BlockBenchFormatInfo(FormatVersion, "bedrock", true);
    }

    internal sealed class BlockBenchFormatInfo
    {
        [JsonProperty("format_version")]
        internal string FormatVersion { get; }
       
        [JsonProperty("model_format")]
        internal string ModelFormat { get; }
       
        [JsonProperty("box_uv")]
        internal bool UseBoxUv { get; set; }

        [JsonConstructor]
        private BlockBenchFormatInfo() { }

        internal BlockBenchFormatInfo(string formatVersion, string modelFormat, bool useBoxUv)
        {
            FormatVersion = formatVersion;
            ModelFormat = modelFormat;
            UseBoxUv = useBoxUv;
        }
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

        [DefaultValue(true)]
        [JsonProperty("export", DefaultValueHandling = DefaultValueHandling.Ignore)]
        internal bool Export { get; } = true;

        [JsonProperty("inflate")]
        internal float Inflate;
        
        [JsonProperty("origin", NullValueHandling = NullValueHandling.Ignore)]
        private float[] origin;

        [JsonProperty("from")]
        private float[] from;

        [JsonProperty("to")]
        private float[] to;

        [JsonProperty("uv_offset")]
        private int[] uv_offset;

        [JsonProperty("rotation", NullValueHandling = NullValueHandling.Ignore)]
        private float[] rotation;

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

        [JsonIgnore()]
        internal Vector3 Rotation
        {
            get
            {
                return new Vector3(rotation?[0] ?? 0, rotation?[1] ?? 0, rotation?[2] ?? 0);
            }
            set
            {
                if (rotation is null || rotation.Length < 3)
                    rotation = new float[3];
                rotation[0] = value.X;
                rotation[1] = value.Y;
                rotation[2] = value.Z;
            }
        }

        [JsonProperty("type")]
        internal string Type;

        [JsonProperty("uuid")]
        internal Guid Uuid;

        internal static Element CreateCube(string name, Vector2 uvOffset, Vector3 pos, Vector3 size, float inflate, bool mirror)
        {
            return new Element
            {
                Name = name,
                UseBoxUv = true,
                Locked = false,
                Rescale = false,
                Type = "cube",
                Uuid = Guid.NewGuid(),
                UvOffset = uvOffset,
                MirrorUv = mirror,
                Inflate = inflate,
                From = pos,
                To = pos + size
            };
        }

    }

    internal class Texture
    {
        public static implicit operator Image(Texture texture) => texture.GetImage();
        public static implicit operator Texture(Image image) => new Texture(image);
        public static implicit operator Texture(NamedTexture namedTexture) => new Texture(namedTexture.Name, namedTexture.Texture);
        
        private const string _TEXTUREDATAHEAD = "data:image/png;base64,";

        [JsonConstructor]
        private Texture()
        {
        }
        
        internal Texture(string name, Image image)
            : this(image)
        {
            Name = name;
        }

        internal Texture(Image image)
        {
            if (image is not null)
            {
                SetImage(image);
                return;
            }
            Debug.WriteLine($"param: {nameof(image)} is null");
        }

        [JsonProperty("name")]
        internal string Name { get; set; }
        
        [JsonProperty("source")]
        internal string TextureSource { get; private set; }

        private Image GetImage()
        {
            string data = TextureSource;
            if (data.StartsWith(_TEXTUREDATAHEAD))
            {
                byte[] encodedData = Convert.FromBase64String(data.Substring(_TEXTUREDATAHEAD.Length));
                using var ms = new MemoryStream(encodedData);
                return Image.FromStream(ms);
            }
            return null;
        }

        private void SetImage(Image image)
        {
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            TextureSource = _TEXTUREDATAHEAD + Convert.ToBase64String(ms.ToArray());
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

        [JsonProperty("rotation")]
        private float[] rotation;

        [JsonIgnore]
        public Vector3 Rotation
        {
            get => new Vector3(rotation?[0] ?? 0, rotation?[1] ?? 0, rotation?[2] ?? 0);
            set
            {
                if (rotation is null || rotation.Length < 3)
                    rotation = new float[3];
                rotation[0] = value.X;
                rotation[1] = value.Y;
                rotation[2] = value.Z;
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
        internal BlockBenchFormatInfo Format;

        [JsonProperty("model_identifier")]
        internal string ModelIdentifier { get; set; } = "";

        [JsonProperty("resolution")]
        internal TextureRes TextureResolution;
        
        [JsonProperty("elements")]
        internal Element[] Elements;
        
        [JsonProperty("outliner")]
        internal JArray Outliner;
        
        [JsonProperty("textures")]
        internal Texture[] Textures;

        internal static BlockBenchModel Create(BlockBenchFormatInfo formatInfo, string name, Size textureResolution, IEnumerable<Texture> textures)
        {
            return new BlockBenchModel()
            {
                Name = name,
                Textures = textures.ToArray(),
                TextureResolution = textureResolution,
                ModelIdentifier = "",
                Format = formatInfo,
            };
        }
    }
}
