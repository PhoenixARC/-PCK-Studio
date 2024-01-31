using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering.Texture
{
    internal class Texture
    {
        protected readonly int _id;

        protected TextureTarget Target;

        protected Texture(TextureTarget target)
        {
            _id = GL.GenTexture();
            Target = target;
        }

        public void Bind()
        {
            GL.BindTexture(Target, _id);
        }

        public void Bind(int slot)
        {
            Debug.Assert(slot >= 0 || slot < 32, "Slot is out of range");
            slot = MathHelper.Clamp(slot, 0, 31);
            Bind();
            GL.ActiveTexture(TextureUnit.Texture0 + slot);
        }

        public void Unbind()
        {
            GL.BindTexture(Target, 0);
        }

        public void SetTexParameter(TextureParameterName parameterName, int value)
        {
            GL.TexParameter(Target, parameterName, value);
            Debug.WriteLineIf(GL.GetError() != ErrorCode.NoError, $"{Target}: {parameterName} = {value}");
        }
    }
}
