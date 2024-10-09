using System;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering.Texture
{
    internal class Texture
    {
        protected readonly int _id;

        protected readonly TextureTarget Target;

        public PixelFormat PixelFormat { get; set; }
        public PixelInternalFormat InternalPixelFormat { get; set; }

        public TextureMinFilter MinFilter
        {
            get => minFilter;
            set
            {
                minFilter = value;
                SetTexParameter(TextureParameterName.TextureMinFilter, (int)value);
            }
        }

        public TextureMagFilter MagFilter
        {
            get => magFilter;
            set
            {
                magFilter = value;
                SetTexParameter(TextureParameterName.TextureMagFilter, (int)value);
            }
        }

        public TextureWrapMode WrapS
        {
            get => wrapS;
            set
            {
                wrapS = value;
                SetTexParameter(TextureParameterName.TextureWrapS, (int)value);
            }
        }

        public TextureWrapMode WrapT
        {
            get => wrapT;
            set
            {
                wrapT = value;
                SetTexParameter(TextureParameterName.TextureWrapT, (int)value);
            }
        }

        public TextureWrapMode WrapR
        {
            get => wrapR;
            set
            {
                wrapR = value;
                SetTexParameter(TextureParameterName.TextureWrapR, (int)value);
            }
        }

        private TextureMinFilter minFilter;
        private TextureMagFilter magFilter;
        private TextureWrapMode wrapS;
        private TextureWrapMode wrapT;
        private TextureWrapMode wrapR;

        protected Texture(TextureTarget target)
        {
            _id = GL.GenTexture();
            Target = target;
        }

        public virtual void SetTexture(Image image)
        {
            throw new NotImplementedException();
        }

        public void Bind(int slot = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + slot);
            GL.BindTexture(Target, _id);
        }

        public void Unbind()
        {
            GL.BindTexture(Target, 0);
        }

        private void SetTexParameter(TextureParameterName parameterName, int value)
        {
            Bind();
            GL.TexParameter(Target, parameterName, value);
            Debug.WriteLineIf(GL.GetError() != ErrorCode.NoError, $"{Target}: {parameterName} = {value}");
        }
    }
}
