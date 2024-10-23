using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PckStudio.Extensions;

namespace PckStudio.Rendering
{
    internal struct BoundingBox
    {
        public static BoundingBox Empty = new BoundingBox(OpenTK.Vector3.Zero, OpenTK.Vector3.Zero);

        public readonly OpenTK.Vector3 Start;
        public readonly OpenTK.Vector3 End;
        public readonly OpenTK.Vector3 Center;
        public readonly OpenTK.Vector3 Volume;

        public BoundingBox(OpenTK.Vector3 start, OpenTK.Vector3 end)
        {
            Start = start;
            End = end;
            OpenTK.Vector3 size = End - Start;
            Volume = OpenTKExtensions.Abs(size);
            Center = start + Volume / 2;
        }

        public BoundingBox(System.Numerics.Vector3 start, System.Numerics.Vector3 end)
            : this(start.ToOpenTKVector(), end.ToOpenTKVector())
        {
        }


        public ColorVertex[] GetVertices()
        {
            OpenTK.Vector3 s = Start;
            OpenTK.Vector3 e = End;
            return [
                new ColorVertex(new OpenTK.Vector3(s.X, e.Y, e.Z)),
                new ColorVertex(new OpenTK.Vector3(e.X, e.Y, e.Z)),
                new ColorVertex(new OpenTK.Vector3(e.X, s.Y, e.Z)),
                new ColorVertex(new OpenTK.Vector3(s.X, s.Y, e.Z)),
                new ColorVertex(new OpenTK.Vector3(s.X, e.Y, s.Z)),
                new ColorVertex(new OpenTK.Vector3(e.X, e.Y, s.Z)),
                new ColorVertex(new OpenTK.Vector3(e.X, s.Y, s.Z)),
                new ColorVertex(new OpenTK.Vector3(s.X, s.Y, s.Z)),
            ];
        }

        public static int[] GetIndecies()
        {
            return [0, 1,
                    1, 2,
                    2, 3,
                    3, 0,

                    4, 5,
                    5, 6,
                    6, 7,
                    7, 4,

                    0, 4,
                    1, 5,
                    2, 6,
                    3, 7
            ];
        }
    
        public static BoundingBox GetEnclosingBoundingBox(IEnumerable<BoundingBox> boundingBoxes)
        {
            return boundingBoxes.DefaultIfEmpty().Aggregate((a, b) => new BoundingBox(OpenTK.Vector3.ComponentMin(a.Start, b.Start), OpenTK.Vector3.ComponentMax(a.End, b.End)));
        }
    }
}
