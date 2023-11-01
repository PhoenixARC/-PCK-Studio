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
        private List<uint> _indecies;

        public IndexBuffer(params uint[] indecies)
        {
            _indecies = new List<uint>(indecies);
        }

        /// <summary>
        /// Creates and attaches created index buffer
        /// </summary>
        /// <param name="indecies"></param>
        /// <returns></returns>
        public static IndexBuffer Create(params uint[] indecies)
        {
            var ib = new IndexBuffer(indecies);
            ib.Attach();
            return ib;
        }

        public void Attach()
        {
            _id = GL.GenBuffer();
            Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indecies.Count * sizeof(uint), _indecies.ToArray(), BufferUsageHint.StaticDraw);
        }

        public int GetCount() => _indecies.Count;

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
