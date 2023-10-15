using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace PckStudio.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct Vertex
    {
        public Vertex(Vector3 position, Color4 color, Vector2 texPosition)
        {
            Position = position;
            Color = color;
            TexPosition = texPosition;
        }

        internal Vector3 Position { get; set; }
        internal Color4 Color { get; set; }
        internal Vector2 TexPosition { get; set; }
    }
}
