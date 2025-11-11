using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OMI.Formats.Pck;

namespace PckStudio.Core.Skin
{
    public sealed class SkinModel
    {
        public readonly List<SkinBOX> AdditionalBoxes;
        public readonly List<SkinPartOffset> PartOffsets;

        public SkinModel()
        {
            AdditionalBoxes = new List<SkinBOX>();
            PartOffsets = new List<SkinPartOffset>(5);
        }

        public SkinModel(IEnumerable<SkinBOX> additionalBoxes, IEnumerable<SkinPartOffset> partOffsets)
        {
            AdditionalBoxes = new List<SkinBOX>(additionalBoxes);
            PartOffsets = new List<SkinPartOffset>(partOffsets);
        }
    }
}
