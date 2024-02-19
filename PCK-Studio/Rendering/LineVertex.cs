using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace PckStudio.Rendering
{
    [StructLayout(LayoutKind.Sequential, Size = 28)]
    internal struct LineVertex
    {
        public LineVertex(Vector3 position, Color4 color)
        {
            Position = position;
            Color = color;
        }

        public Vector3 Position { get; set; }
        public Color4 Color { get; set; }
    }
}
