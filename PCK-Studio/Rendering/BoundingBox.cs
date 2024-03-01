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

        public BoundingBox(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }
    }
}
