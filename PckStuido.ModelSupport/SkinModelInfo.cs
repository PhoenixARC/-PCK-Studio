using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Skin;

namespace PckStudio.ModelSupport
{
    public sealed class SkinModelInfo
    {
        public SkinModel Model { get; }
        public SkinANIM Anim { get; }
        public Image Texture { get; }

        public SkinModelInfo(Image texture, SkinANIM anim, SkinModel model)
        {
            Texture = texture;
            Anim = anim;
            Model = model;
        }
    }
}
