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

        public OpenTK.Matrix4 GetTransform()
        {
            return OpenTK.Matrix4.CreateScale(Volume) * OpenTK.Matrix4.CreateTranslation(Start);
        }

        private static readonly ColorVertex[] _vertices = [
                new ColorVertex(new OpenTK.Vector3(0f, 1f, 1f)),
                new ColorVertex(new OpenTK.Vector3(1f, 1f, 1f)),
                new ColorVertex(new OpenTK.Vector3(1f, 0f, 1f)),
                new ColorVertex(new OpenTK.Vector3(0f, 0f, 1f)),
                new ColorVertex(new OpenTK.Vector3(0f, 1f, 0f)),
                new ColorVertex(new OpenTK.Vector3(1f, 1f, 0f)),
                new ColorVertex(new OpenTK.Vector3(1f, 0f, 0f)),
                new ColorVertex(new OpenTK.Vector3(0f, 0f, 0f)),
            ];

        private static readonly int[] _indecies = [
            0, 1,
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

        public static ColorVertex[] GetVertices()
        {
            return _vertices;
            //OpenTK.Vector3 s = Start; // 0, 0, 0
            //OpenTK.Vector3 e = End;   // 1, 1, 1
            //return [
            //    new ColorVertex(new OpenTK.Vector3(s.X, e.Y, e.Z)),
            //    new ColorVertex(new OpenTK.Vector3(e.X, e.Y, e.Z)),
            //    new ColorVertex(new OpenTK.Vector3(e.X, s.Y, e.Z)),
            //    new ColorVertex(new OpenTK.Vector3(s.X, s.Y, e.Z)),
            //    new ColorVertex(new OpenTK.Vector3(s.X, e.Y, s.Z)),
            //    new ColorVertex(new OpenTK.Vector3(e.X, e.Y, s.Z)),
            //    new ColorVertex(new OpenTK.Vector3(e.X, s.Y, s.Z)),
            //    new ColorVertex(new OpenTK.Vector3(s.X, s.Y, s.Z)),
            //];
        }

        public static int[] GetIndecies() => _indecies;
    }
}
