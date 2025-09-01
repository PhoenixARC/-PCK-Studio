using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core.Extensions
{
    public static class BoundingBoxExtensions
    {
        public static BoundingBox GetEnclosingBoundingBox(this IEnumerable<BoundingBox> boundingBoxes)
        {
            return boundingBoxes.DefaultIfEmpty().Aggregate((a, b) => new BoundingBox(OpenTK.Vector3.ComponentMin(a.Start, b.Start), OpenTK.Vector3.ComponentMax(a.End, b.End)));
        }
    }
}
