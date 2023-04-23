using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PckStudio.Conversion.Common.JsonDefinitions;

namespace PckStudio.Classes.Conversion.Legacy.JsonDefinitions
{
    internal class BlockbenchBedrockModel
    {
        internal class BedrockModelDescription
        {
            public Size TextureSize => new Size(TextureWidth, TextureHeight);

            [JsonProperty("identifier")]
            public string Identifier { get; set; }

			[JsonProperty("texture_width")]
            public int TextureWidth { get; set; }
            
            [JsonProperty("texture_height")]
            public int TextureHeight { get; set; }

            [JsonProperty("visible_bounds_width")]
            public int VisibleBoundsWidth { get; set; }
            
            [JsonProperty("visible_bounds_height")]
            public int VisibleBoundsHeight { get; set; }
            
            [JsonProperty("visible_bounds_offset")]
            public float[] VisibleBoundsOffset { get; set; }
        }

        internal class BedrockModel
        {
            [JsonProperty("description")]
            public BedrockModelDescription Description { get; set; }

            [JsonProperty("bones")]
            public GeometryBone[] Bones { get; set; }
        }

        [JsonProperty("format_version")]
        public string FormatVersion { get; set; }

        [JsonProperty("minecraft:geometry")]
        public BedrockModel[] Models { get; set; }
    }

}
