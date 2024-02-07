using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal struct VertexBuffer<T> : IDisposable where T : struct
    {
        private int _id;

#if DEBUG
        private T[] __data;
#endif

        public VertexBuffer(T[] data, int size)
        {
#if DEBUG
            __data = data;
#endif
            _id = GL.GenBuffer();
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, size, data, BufferUsageHint.StaticDraw);
            Unbind();
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            Unbind();
            GL.DeleteBuffer(_id);
        }
    }
}
