/* Copyright (c) 2024-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
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

        internal static ShaderObject Create(ShaderSource shaderSource)
        {
            var shaderObject = new ShaderObject(shaderSource);
            if (!shaderObject.Compile())
            {
                string infoLog = shaderObject.GetShaderInfoLog();
                Debug.Fail(infoLog);
                shaderObject.Delete();
                return null;
            }
            return shaderObject;
        }

        private string GetShaderInfoLog()
        {
            return GL.GetShaderInfoLog(_shaderId);
        }

        internal void Delete()
        {
            GL.DeleteShader(_shaderId);
        }

        private bool Compile()
        {
            GL.ShaderSource(_shaderId, _source.Source);
            GL.CompileShader(_shaderId);

            GL.GetShader(_shaderId, ShaderParameter.CompileStatus, out _compileStatus);

            return CompileStatusOK;
        }

        internal void AttachToProgram(int programId)
        {
            GL.AttachShader(programId, _shaderId);
        }
    }
}
