using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal sealed class DrawContext
    {
        internal readonly VertexArray VertexArray;
        internal readonly IndexBuffer IndexBuffer;
        internal readonly PrimitiveType PrimitiveType;

        public DrawContext(VertexArray vertexArray, IndexBuffer indexBuffer, PrimitiveType primitiveType)
        {
            VertexArray = vertexArray;
            IndexBuffer = indexBuffer;
            PrimitiveType = primitiveType;
        }
    }
}
