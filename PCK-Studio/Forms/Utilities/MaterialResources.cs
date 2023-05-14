using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using PckStudio.Extensions;
using OMI.Formats.Pck;
using OMI.Formats.Material;
using OMI.Workers.Material;
using System;

namespace PckStudio.Forms.Utilities
{
    public static class MaterialResources
    {
        public static byte[] MaterialsFileInitializer()
        {
            using var stream = new MemoryStream();
            var matFile = new MaterialContainer
            {
                new MaterialContainer.Material("bat", "entity_alphatest")
            };
            var writer = new MaterialFileWriter(matFile);
            writer.WriteToStream(stream);
            return stream.ToArray();
        }
    }
}
