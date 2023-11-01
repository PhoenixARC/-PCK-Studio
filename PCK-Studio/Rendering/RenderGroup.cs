using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal class RenderGroup<T> where T : struct
    {
        internal string Name { get; }

        internal static int SizeInBytes = Marshal.SizeOf<T>();
        
        protected List<T> vertices;
        protected List<uint> indices;
        protected uint indicesOffset;


        private VertexArray vertexArray;
        private VertexBuffer<T> vertexBuffer;
        private IndexBuffer indexBuffer;
        private readonly VertexBufferLayout _layout;
        private readonly PrimitiveType drawType;

        internal RenderGroup(string name, VertexBufferLayout layout, PrimitiveType type)
        {
            Name = name;
            drawType = type;
            indicesOffset = 0;
            vertices = new List<T>(10);
            indices = new List<uint>(10);
            _layout = layout;
        }

        internal RenderBuffer GetRenderBuffer()
        {
            indexBuffer?.Dispose();
            vertexBuffer.Dispose();
            vertexArray ??= new VertexArray();

            var vertexData = vertices.ToArray();
            vertexBuffer = new VertexBuffer<T>(vertexData, vertexData.Length * SizeInBytes);

            vertexArray.AddBuffer(vertexBuffer, _layout);

            indexBuffer = IndexBuffer.Create(indices.ToArray());

            return new RenderBuffer(vertexArray, indexBuffer, drawType);
        }
    }
}