using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;
using PckStudio.Core.Extensions;
using PckStudio.Interfaces;

namespace PckStudio.Core.Serializer
{
    internal sealed class AtlasSerializer : IPckAssetSerializer<Atlas>
    {
        public void Serialize(Atlas atlas, ref PckAsset asset)
        {
            asset.SetTexture(atlas);
        }
    }
}
