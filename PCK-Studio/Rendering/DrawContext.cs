using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal class DrawContext : IDisposable
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

        public void Dispose()
        {
            VertexArray.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
