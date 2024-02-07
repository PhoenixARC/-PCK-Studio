using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal class VertexArray : IDisposable
    {
        private int _id;

        public VertexArray()
        {
            _id = GL.GenVertexArray();
        }

        public void AddBuffer<T>(VertexBuffer<T> buffer, VertexBufferLayout layout) where T : struct
        {
            Bind();
            buffer.Bind();
            var elements = layout.GetElements();
            int offset = 0;
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                GL.EnableVertexAttribArray(i);
                GL.VertexAttribPointer(i, element.Count, element.Type, element.Normalize, layout.GetStride(), offset);
                offset += element.Count * VertexBufferElement.GetStrideSize(element.Type);
            }
            Unbind();
        }

        public void Bind()
        {
            GL.BindVertexArray(_id);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            Unbind();
            GL.DeleteVertexArray(_id);
        }
    }
}
