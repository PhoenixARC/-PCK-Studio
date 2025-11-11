using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Extensions;
using PckStudio.Core.Skin;

namespace PckStudio.Rendering.Extension
{
    public static class SkinBoxExtension
    {
        public static Cube ToCube(this SkinBOX skinBOX) => skinBOX.ToCube(0f);

        public static Cube ToCube(this SkinBOX skinBOX, float inflate, bool flipZMapping = false)
            => new Cube(skinBOX.Pos.ToOpenTKVector(), skinBOX.Size.ToOpenTKVector(), skinBOX.UV.ToOpenTKVector(), skinBOX.Scale + inflate, skinBOX.Mirror, flipZMapping);
    }
}
