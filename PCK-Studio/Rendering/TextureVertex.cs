using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK;

namespace PckStudio.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 20)]
    internal struct TextureVertex : IVertexLayout
    {
        internal Vector3 Position { get; set; }
        internal Vector2 TexPosition { get; set; }
        
        public TextureVertex(Vector3 position, Vector2 texPosition)
        {
            Position = position;
            TexPosition = texPosition;
        }

        public VertexBufferLayout GetLayout()
        {
            var layout = new VertexBufferLayout();
            layout.Add(ShaderDataType.Float3);
            layout.Add(ShaderDataType.Float2);
            return layout;
        }
    }
}
