using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OMI.Formats.Pck;

namespace PckStudio.Internal
{
    public sealed class Skin
    {
        public string Name { get; set; }
        public string Theme { get; set; }
        public int Id { get; set; }
        public Image Texture { get; set; }
        public Image CapeTexture { get; set; }
        public bool HasCape => CapeTexture is not null;
        public SkinANIM ANIM { get; set; }
        public readonly List<SkinBOX> AdditionalBoxes;
        public readonly List<SkinPartOffset> PartOffsets;

        public Skin(string name, int id, Image texture, SkinANIM anim, IEnumerable<SkinBOX> additionalBoxes, IEnumerable<SkinPartOffset> partOffsets)
        {
            Name = name;
            Id = id;
            Texture = texture;
            ANIM = anim;
            AdditionalBoxes = new List<SkinBOX>(additionalBoxes);
            PartOffsets = new List<SkinPartOffset>(partOffsets);
        }
    }
}
