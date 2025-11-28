using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core.Skin
{
    public sealed class Skin
    {
        public SkinMetaData MetaData { get; set; }
        
        public SkinIdentifier Identifier { get; set; }
        
        public SkinANIM Anim { get; set; }

        public SkinModel Model { get; set; }
        
        public Image Texture { get; set; }

        public int CapeId { get; set; } = -1;

        public bool HasCape => CapeId != -1;
    
        public Skin(string name, Image texture) : this(name, "", texture) { }

        public Skin(string name, string theme, Image texture, SkinANIM anim = default)
        {
            MetaData = new SkinMetaData(name, theme);
            Texture = texture;
            Model = new SkinModel();
            Anim = anim ?? new SkinANIM(0);
        }
        
        public Skin(string name, Image texture, int capeId) : this(name, "", texture, capeId) { }
        public Skin(string name, string theme, Image texture, int capeId) : this(name, theme, texture)
        {
            CapeId = capeId;
        }    

        public Skin(string name, SkinANIM anim, Image texture, IEnumerable<SkinBOX> additionalBoxes, IEnumerable<SkinPartOffset> partOffsets)
            : this(name, "", anim, texture, additionalBoxes, partOffsets) { }

        public Skin(string name, string theme, SkinANIM anim, Image texture, IEnumerable<SkinBOX> additionalBoxes, IEnumerable<SkinPartOffset> partOffsets)
            : this(name, theme, texture, anim)
        {
            Model.AdditionalBoxes.AddRange(additionalBoxes);
            Model.PartOffsets.AddRange(partOffsets);
        }

        public Skin(string name, string theme, int id, Image texture, SkinANIM anim, IEnumerable<SkinBOX> additionalBoxes, IEnumerable<SkinPartOffset> partOffsets)
            : this(name, theme, anim, texture, additionalBoxes, partOffsets)
        {
            Identifier = new(id);
        }
    }
}
