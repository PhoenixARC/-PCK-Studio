using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering.Shader
{
    public readonly struct ShaderSource
    {
        public readonly ShaderType Type;
        public readonly string Source;

        public ShaderSource(ShaderType type, string source)
        {
            Type = type;
            Source = source;
        }
    }
}
