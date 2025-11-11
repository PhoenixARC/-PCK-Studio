using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;
using PckStudio.Core.Extensions;
using PckStudio.Interfaces;

namespace PckStudio.Core.Deserializer
{
    public sealed class AtlasDeserializer : IPckAssetDeserializer<Atlas>
    {
        private readonly ResourceLocation _resourceLocation;

        public AtlasDeserializer(ResourceLocation resourceLocation)
        {
            _resourceLocation = resourceLocation;
        }

        public Atlas Deserialize(PckAsset asset) => Atlas.FromResourceLocation(asset.GetTexture(), _resourceLocation);
    }
}
