using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;

namespace PckStudio.Interfaces
{
    internal interface IPckAssetDeserializer<T>
    {
        public T Deserialize(PckAsset file);
    }
}