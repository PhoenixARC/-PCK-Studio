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
        
        public Image CapeTexture { get; set; }

        public bool HasCape => CapeTexture is not null;
    
        public Skin(string name, Image texture)
        {
            MetaData = new SkinMetaData(name, string.Empty);
            Texture = texture;
            Model = new SkinModel();
        }
        
        public Skin(string name, Image texture, Image capeTexture)
            : this(name, texture)
        {
            CapeTexture = capeTexture;
        }    

        public Skin(string name, SkinANIM anim, Image texture, IEnumerable<SkinBOX> additionalBoxes, IEnumerable<SkinPartOffset> partOffsets)
            : this(name, texture)
        {
            Model.AdditionalBoxes.AddRange(additionalBoxes);
            Model.PartOffsets.AddRange(partOffsets);
            Anim = anim;
        }

        public Skin(string name, int id, Image texture, SkinANIM anim, IEnumerable<SkinBOX> additionalBoxes, IEnumerable<SkinPartOffset> partOffsets)
            : this(name, anim, texture, additionalBoxes, partOffsets)
        {
            Identifier = new(id);
        }
    }
}
