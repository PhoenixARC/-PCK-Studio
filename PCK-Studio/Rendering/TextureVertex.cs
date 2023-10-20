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
    internal struct TextureVertex
    {
        public static int SizeInBytes = Marshal.SizeOf(typeof(TextureVertex));

        public TextureVertex(Vector3 position, Vector2 texPosition)
        {
            Position = position;
            TexPosition = texPosition;
        }

        internal Vector3 Position { get; set; }
        internal Vector2 TexPosition { get; set; }
    }
}
