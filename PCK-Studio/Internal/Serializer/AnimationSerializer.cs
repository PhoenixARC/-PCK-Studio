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
    internal sealed class AnimationSerializer : IPckAssetSerializer<Animation>
    {
        public static readonly AnimationSerializer DefaultSerializer = new AnimationSerializer();

        public void Serialize(Animation animation, ref PckAsset asset)
        {
            string anim = animation.BuildAnim();
            asset.SetProperty("ANIM", anim);
            var texture = animation.BuildTexture();
            asset.SetTexture(texture);
        }
    }
}
