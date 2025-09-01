using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    public struct VertexBuffer : IDisposable
    {
        private int _id;
        private int _count;
        private int _size;

        public VertexBuffer()
        {
            _id = GL.GenBuffer();
            _size = 0;
        }

        public VertexBuffer(int size) : this()
        {
            _size = size;
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, BufferUsageHint.StaticDraw);
            Unbind();
        }

        public void SetData<T>(T[] data) where T : struct
        {
            int sizeofT = Marshal.SizeOf<T>();
            _count = data.Length;
            Bind();
            int size = sizeofT * _count;
            if (_size < size)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, size, data, BufferUsageHint.StaticDraw);
                _size = size;
                return;
            }
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, size, data);
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

        public IndexBuffer GenIndexBuffer()
        {
            return IndexBuffer.Create(Enumerable.Range(0, _count).ToArray());
        }
    }
}
