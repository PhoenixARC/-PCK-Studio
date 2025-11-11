using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PckStudio.Core.Extensions;

namespace PckStudio.Core
{
    public struct BoundingBox
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
    }
}
