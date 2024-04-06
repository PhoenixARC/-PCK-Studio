using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace PckStudio.Rendering
{
    internal struct BoundingBox
    {
        public readonly Vector3 Start;
        public readonly Vector3 End;
        public readonly Vector3 Volume;

        public BoundingBox(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
            Vector3 size = End - Start;
            Volume = new Vector3(Math.Abs(size.X), Math.Abs(size.Y), Math.Abs(size.Z));
        }

        public ColorVertex[] GetVertices()
        {
            Vector3 s = Start;
            Vector3 e = End;
            return [
                new ColorVertex(new Vector3(s.X, e.Y, e.Z)),
                new ColorVertex(new Vector3(e.X, e.Y, e.Z)),
                new ColorVertex(new Vector3(e.X, s.Y, e.Z)),
                new ColorVertex(new Vector3(s.X, s.Y, e.Z)),
                new ColorVertex(new Vector3(s.X, e.Y, s.Z)),
                new ColorVertex(new Vector3(e.X, e.Y, s.Z)),
                new ColorVertex(new Vector3(e.X, s.Y, s.Z)),
                new ColorVertex(new Vector3(s.X, s.Y, s.Z)),
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
    }
}
