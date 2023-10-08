using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal class IndexBuffer : IDisposable
    {
        private int _id;
        private int _count;

        public IndexBuffer(params uint[] indecies)
        {
            _id = GL.GenBuffer();
            Bind();
            _count = indecies.Length;
            GL.BufferData(BufferTarget.ElementArrayBuffer, indecies.Length * sizeof(uint), indecies, BufferUsageHint.StaticDraw);
        }

        public int GetCount() => _count;

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _id);
        }

        [Conditional("DEBUG")]
        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_id);
        }
    }
}
