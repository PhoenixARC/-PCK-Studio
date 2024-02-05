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
        public Texture2D(string filepath)
            : this(Image.FromFile(filepath))
        {

        }

        public Texture2D(Image image)
            : this(new Bitmap(image))
        {

        }

        public Texture2D(Image image, int slot)
            : this(new Bitmap(image), slot)
        {

        }

        private Texture2D(Bitmap bitmap, int slot = 0)
            : base(TextureTarget.Texture2D)
        {
            Bind(slot);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            SetTexParameter(TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            SetTexParameter(TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
           
            SetTexParameter(TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            SetTexParameter(TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
        }

        public void Dispose()
        {
            Unbind();
            GL.DeleteTexture(_id);
        }

        public static implicit operator Texture2D(Bitmap image) => new Texture2D(image);
    }
}
