using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering.Texture
{
    internal class Texture
    {
        protected readonly int _id;

        protected readonly TextureTarget Target;

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

        public int Slot
        {
            get => _slot;
            set
            {
                Debug.Assert(value >= 0 || value < 32, "Slot is out of range");
                _slot = MathHelper.Clamp(value, 0, 31);
            }
        }

        private int _slot = 0;  
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

        public void Bind()
        {
            GL.ActiveTexture(TextureUnit.Texture0 + Slot);
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
