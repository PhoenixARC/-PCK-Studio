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
    public struct ColorVertex
    {
        public ColorVertex(Vector3 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
        
        public ColorVertex(Vector3 position)
            : this(position, System.Drawing.Color.White)
        {
        }

        public static implicit operator ColorVertex(Vector3 vector3) => new ColorVertex(vector3);

        public Vector3 Position { get; set; }
        public Color4 Color { get; set; }
    }
}
