using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    public class FrameBuffer
    {
        private int _id;
        private FramebufferErrorCode status;

        public FramebufferErrorCode Status => status;
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

        internal void CheckStatus()
        {
            status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer); 
        }
    }
}
