using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Rendering.Addon
{
    public struct BoundingBox
    {
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
        }

        public static int[] GetIndecies() => _indecies;
    }
}
