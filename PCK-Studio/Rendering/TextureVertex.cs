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
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 20)]
    internal struct TextureVertex : IVertexLayout
    {
        internal Vector3 Position { get; set; }
        internal Vector2 TexPosition { get; set; }
        internal float Scale { get; set; }

        public TextureVertex(Vector3 position, Vector2 texPosition)
            : this(position, texPosition, 1f)
        {
        }
        
        public TextureVertex(Vector3 position, Vector2 texPosition, float scale)
        {
            Position = position;
            TexPosition = texPosition;
            Scale = scale;
        } 

        public VertexBufferLayout GetLayout()
        {
            var layout = new VertexBufferLayout();
            layout.Add<float>(3);
            layout.Add<float>(2);
            layout.Add<float>(1);
            return layout;
        }
    }
}
