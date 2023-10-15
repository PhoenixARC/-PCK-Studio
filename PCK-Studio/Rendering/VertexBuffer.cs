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
            _id = GL.GenBuffer();
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, size, data, BufferUsageHint.StaticDraw);
#if DEBUG
            __data = data;
#endif
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
        }

        [Conditional("DEBUG")]
        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_id);
        }
    }
}
