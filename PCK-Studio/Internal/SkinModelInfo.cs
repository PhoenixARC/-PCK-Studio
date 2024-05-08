using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OMI.Formats.Pck;

namespace PckStudio.Internal
{
    public sealed class SkinModelInfo
    {
        public Image Texture { get; set; }
        public SkinANIM ANIM { get; set; }
        public readonly List<SkinBOX> AdditionalBoxes;
        public readonly List<SkinPartOffset> PartOffsets;
        
        public SkinModelInfo() : this(null)
        {
        }

        public SkinModelInfo(Image texture)
            : this(texture, new SkinANIM())
        {
            Texture = texture;
        }

        public SkinModelInfo(Image texture, SkinANIM anim)
        {
            Texture = texture;
            ANIM = anim;
            AdditionalBoxes = new List<SkinBOX>();
            PartOffsets = new List<SkinPartOffset>(5);
        }

        public SkinModelInfo(Image texture, SkinANIM anim, IEnumerable<SkinBOX> additionalBoxes, IEnumerable<SkinPartOffset> partOffsets)
        {
            Texture = texture;
            ANIM = anim;
            AdditionalBoxes = new List<SkinBOX>(additionalBoxes);
            PartOffsets = new List<SkinPartOffset>(partOffsets);
        }
    }
}
