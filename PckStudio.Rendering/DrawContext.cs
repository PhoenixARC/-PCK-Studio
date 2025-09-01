using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    public sealed class DrawContext
    {
        public readonly VertexArray VertexArray;
        public readonly IndexBuffer IndexBuffer;
        public readonly PrimitiveType PrimitiveType;

        public DrawContext(VertexArray vertexArray, IndexBuffer indexBuffer, PrimitiveType primitiveType)
        {
            VertexArray = vertexArray;
            IndexBuffer = indexBuffer;
            PrimitiveType = primitiveType;
        }
    }
}
