using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK;

namespace PckStudio.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 20)]
    public struct TextureVertex
    {
        public Vector3 Position { get; set; }
        public Vector2 TexPosition { get; set; }
        
        public TextureVertex(Vector3 position, Vector2 texPosition)
        {
            Position = position;
            TexPosition = texPosition;
        }
    }
}
