using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal class FrameBuffer
    {
        private int _id;

        public FrameBuffer()
        {
            _id = GL.GenFramebuffer();
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
