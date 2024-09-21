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
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal abstract class GenericMesh<T> where T : struct
    {
        internal string Name { get; }
        internal PrimitiveType DrawType { get; }
        internal VertexBufferLayout VertexLayout { get; }

        public abstract Matrix4 Transform { get; }

        protected GenericMesh(string name, PrimitiveType type, VertexBufferLayout vertexLayout)
        {
            Name = name;
            DrawType = type;
            VertexLayout = vertexLayout;
        }

        internal abstract IEnumerable<T> GetVertices();
        internal abstract IEnumerable<int> GetIndices();

        public override string ToString()
        {
            return $"Name: {Name} T={typeof(T)}";
        }
    }
}