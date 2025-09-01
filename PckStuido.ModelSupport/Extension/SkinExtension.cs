using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Model;
using PckStudio.Core.Skin;
using PckStudio.ModelSupport;

namespace PckStuido.ModelSupport.Extension
{
    public static class SkinExtension
    {
        public static SkinModelInfo GetModelInfo(this Skin skin) => new SkinModelInfo(skin.Texture, skin.Anim, skin.Model);

        public static void SetModelInfo(this Skin skin, SkinModelInfo modelInfo)
        {
            skin.Texture = modelInfo.Texture;
            skin.Anim = modelInfo.Anim;
            skin.Model = modelInfo.Model;
        }
    }
}
