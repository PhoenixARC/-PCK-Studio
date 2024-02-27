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
        private int _size;
        private int[] _indicies;

        public IndexBuffer()
        {
            _id = GL.GenBuffer();
            _size = 0;
        }

        /// <summary>
        /// Creates and attaches created index buffer
        /// </summary>
        /// <param name="indicies"></param>
        /// <returns></returns>
        public static IndexBuffer Create(params int[] indicies)
        {
            var ib = new IndexBuffer();
            ib.SetIndicies(indicies);
            return ib;
        }

        public void SetIndicies(int[] indicies)
        {
            Bind();
            int size = indicies.Length * sizeof(int);
            _indicies = indicies;
            if (_size < size)
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer, size, indicies, BufferUsageHint.StaticDraw);
                return;
            }
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, size, indicies);
        }

        public int GetCount() => _indicies.Length;

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
            Unbind();
            GL.DeleteBuffer(_id);
        }
    }
}
