using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering.Texture
{
    internal class Texture2D : Texture
    {
        public Texture2D(string filepath, int slot)
            : this(Image.FromFile(filepath), slot)
        {

        }

        public OpenTK.Graphics.OpenGL.PixelFormat PixelFormat { get; set; }
        public PixelInternalFormat InternalPixelFormat { get; set; }

        public Texture2D(Image image, int slot) : this(slot)
        {
            LoadImageData(image);
        }

        public Texture2D(int slot) : this()
        {
            Slot = slot;
        }
        
        private Texture2D() : base(TextureTarget.Texture2D)
        {
        }

        public void SetSize(Size size)
        {
            Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, InternalPixelFormat, size.Width, size.Height, 0, PixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
            Unbind();
        }

        public void LoadImageData(Image image)
        {
            Bind();
            var bitmap = new Bitmap(image); 
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, InternalPixelFormat, bitmap.Width, bitmap.Height, 0, PixelFormat, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            Unbind();
        }
        public void Dispose()
        {
            Unbind();
            GL.DeleteTexture(_id);
        }


        public void AttachToFramebuffer(FrameBuffer frameBuffer, FramebufferAttachment attachment)
        {
            frameBuffer.Bind();
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, Target, _id, 0);
        }
    }
}
