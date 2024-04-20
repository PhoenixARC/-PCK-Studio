using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OMI.Formats.Pck;
using PckStudio.Extensions;
using PckStudio.Interfaces;

namespace PckStudio.Internal.Serializer
{
    internal sealed class AnimationSerializer : IPckFileSerializer<Animation>
    {
        public static readonly AnimationSerializer DefaultSerializer = new AnimationSerializer();

        public void Serialize(Animation animation, ref PckFileData file)
        {
            string anim = animation.BuildAnim();
            file.SetProperty("ANIM", anim);
            var texture = animation.BuildTexture();
            file.SetTexture(texture);
        }
    }
}
