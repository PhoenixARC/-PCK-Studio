using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.Windows.Media;
using System.Windows;
using OMI.Formats.Model;
using PckStudio.Rendering;
using System.Numerics;

namespace PckStudio.Extensions
{
    static class ModelBoxExtension
    {

        public static BoundingBox GetBoundingBox(this ModelBox modelBox)
        {
            Vector3 halfSize = modelBox.Size / 2f;
            Vector3 halfSizeInflated = new Vector3(modelBox.Inflate) + halfSize;
            Vector3 transformedCenter = modelBox.Position + halfSize;
            Vector3 start = transformedCenter - halfSizeInflated;
            Vector3 end = transformedCenter + halfSizeInflated;
            return new BoundingBox(start, end);
        }

    }
}
