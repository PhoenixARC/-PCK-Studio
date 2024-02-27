/* Copyright (c) 2024-present miku-666
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
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using PckStudio.Rendering.Shader;

namespace PckStudio.Rendering
{
    internal class GenericMesh<T> where T : struct, IVertexLayout
    {
        internal string Name { get; }

        internal readonly int SizeInBytes = Marshal.SizeOf<T>();
        
        protected List<T> vertices;
        protected List<int> indices;
        protected int indicesOffset;

        private VertexArray vertexArray;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private readonly VertexBufferLayout _layout;
        private readonly PrimitiveType drawType;
        private DrawContext drawContext;

        protected GenericMesh(string name, PrimitiveType type)
        {
            Name = name;
            drawType = type;
            indicesOffset = 0;
            vertices = new List<T>(10);
            indices = new List<int>(10);
            _layout = new T().GetLayout();
        }

        internal void Initialize()
        {
            vertexArray = new VertexArray();
            vertexBuffer = new VertexBuffer();
            indexBuffer = new IndexBuffer();
            vertexArray.AddBuffer(vertexBuffer, _layout);
            drawContext = new DrawContext(vertexArray, indexBuffer, drawType);
        }

        protected void ResetBuffers()
        {
            indicesOffset = 0;
            vertices.Clear();
            indices.Clear();
        }

        protected void Submit()
        {
            vertexBuffer.SetData(vertices.ToArray());
            indexBuffer.SetIndicies(indices.ToArray());
        }

        public void Draw(ShaderProgram shader)
        {
            if (drawContext == null)
            {
                Trace.TraceError($"[{nameof(GenericMesh<T>)}] Field: 'drawContext' is null.");
                return;
            }
            if (shader == null)
            {
                Trace.TraceError($"[{nameof(GenericMesh<T>)}] Parameter: 'shader' is null.");
                return;
            }
            Renderer.Draw(shader, drawContext);
        }
    }
}