using System;
using OMI.Formats.Model;
using System.Numerics;

namespace PckStudio.Core.Extensions
{
    public static class ModelBoxExtension
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
