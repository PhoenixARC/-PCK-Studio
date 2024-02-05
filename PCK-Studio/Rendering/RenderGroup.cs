/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal class RenderGroup<T> where T : struct, IVertexLayout
    {
        internal string Name { get; }

        internal readonly int SizeInBytes = Marshal.SizeOf<T>();
        
        protected List<T> vertices;
        protected List<int> indices;
        protected int indicesOffset;

        private VertexArray vertexArray;
        private VertexBuffer<T> vertexBuffer;
        private IndexBuffer indexBuffer;
        private readonly VertexBufferLayout _layout;
        private readonly PrimitiveType drawType;

        internal RenderGroup(string name, PrimitiveType type)
        {
            Name = name;
            drawType = type;
            indicesOffset = 0;
            vertices = new List<T>(10);
            indices = new List<int>(10);
            _layout = new T().GetLayout();
        }

        protected void ResetBuffers()
        {
            indicesOffset = 0;
            vertices.Clear();
            indices.Clear();
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