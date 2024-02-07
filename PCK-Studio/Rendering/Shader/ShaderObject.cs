using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering.Shader
{
    internal class ShaderObject
    {
        private int _shaderId;
        private ShaderSource _source;
        private int _compileStatus;

        internal bool CompileStatusOK => _compileStatus != 0;

        private ShaderObject(ShaderSource source)
        {
            _shaderId = GL.CreateShader(source.Type);
            _source = source;
            _compileStatus = 0;
        }

        internal static ShaderObject CreateNew(ShaderSource shaderSource)
        {
            var shaderObject = new ShaderObject(shaderSource);
            shaderObject.Compile();
            if (!shaderObject.CompileStatusOK)
            {
                shaderObject.Delete();
                return null;
            }
            return shaderObject;
        }

        internal void Delete()
        {
            GL.DeleteShader(_shaderId);
        }

        private void Compile()
        {
            GL.ShaderSource(_shaderId, _source.Source);
            GL.CompileShader(_shaderId);

            GL.GetShader(_shaderId, ShaderParameter.CompileStatus, out _compileStatus);

            if (!CompileStatusOK)
            {
                string infoLog = GL.GetShaderInfoLog(_shaderId);
                Debug.Fail(infoLog);
            }
        }

        internal void AttachToProgram(int programId)
        {
            GL.AttachShader(programId, _shaderId);
        }
    }
}
