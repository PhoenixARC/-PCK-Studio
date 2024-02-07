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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering.Shader
{
    internal class ShaderProgram : IDisposable
    {
        private int _programId;
        private Dictionary<string, int> locationCache = new Dictionary<string, int>();

        private ShaderProgram(int programId)
        {
            _programId = programId;
        }

        public void Bind()
        {
            GL.UseProgram(_programId);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        public void Dispose()
        {
            Unbind();
            GL.DeleteProgram(_programId);
        }

        public void SetUniform1(string name, int value)
        {
            int location = GetUniformLocation(name);
            GL.Uniform1(location, value);
        }

        public void SetUniform1(string name, float value)
        {
            int location = GetUniformLocation(name);
            GL.Uniform1(location, value);
        }

        public void SetUniform2(string name, Vector2 value)
        {
            int location = GetUniformLocation(name);
            GL.Uniform2(location, value);
        }

        public void SetUniform4(string name, Vector4 value)
        {
            int location = GetUniformLocation(name);
            GL.Uniform4(location, value);
        }

        public void SetUniform4(string name, Color color)
        {
            int location = GetUniformLocation(name);
            GL.Uniform4(location, color);
        }

        public void SetUniformMat4(string name, ref Matrix4 matrix)
        {
            int location = GetUniformLocation(name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        private int GetUniformLocation(string name)
        {
            if (locationCache.ContainsKey(name))
                return locationCache[name];
            int location = GL.GetUniformLocation(_programId, name);
            locationCache.Add(name, location);
            return location;
        }

        private static int CompileShader(ShaderType type, string shaderSource)
        {
            int shaderId = GL.CreateShader(type);
            GL.ShaderSource(shaderId, shaderSource);
            GL.CompileShader(shaderId);

            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int status);

            if (status == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderId);
                GL.DeleteShader(shaderId);
                Trace.Fail(infoLog);
                return 0;
            }
            return shaderId;
        }

        private bool Link()
        {
            GL.LinkProgram(_programId);
            GL.GetProgram(_programId, GetProgramParameterName.LinkStatus, out int status);
            bool success = status != 0;
            if (!success)
                Debug.WriteLine(GL.GetProgramInfoLog(_programId), category: nameof(ShaderProgram));
            return success;
        }

        public bool Validate()
        {
#if DEBUG
            GL.ValidateProgram(_programId);
            GL.GetProgram(_programId, GetProgramParameterName.ValidateStatus, out int status);
            bool success = status != 0;
            if (!success)
                Debug.WriteLine(GL.GetProgramInfoLog(_programId), category: nameof(ShaderProgram));
            return success;
#else
            return true;
#endif
        }

        public static ShaderProgram Create(string vertexSource, string fragmentSource)
        {
            return Create(
                new ShaderSource(ShaderType.VertexShader, vertexSource),
                new ShaderSource(ShaderType.FragmentShader, fragmentSource)
                );
        }

        public static ShaderProgram Create(params ShaderSource[] shaderSources)
        {
            int programId = GL.CreateProgram();

            var shaderObjects = new List<ShaderObject>(shaderSources.Length);

            foreach (var shaderSource in shaderSources)
            {
                ShaderObject shaderObject = ShaderObject.CreateNew(shaderSource);
                shaderObject.AttachToProgram(programId);
                shaderObjects.Add(shaderObject);
            }

            var shader = new ShaderProgram(programId);
            bool success = shader.Link();
            Debug.Assert(success, "Shader Program linking failed.");
            
            foreach (var shaderObject in shaderObjects)
            {
                shaderObject.Delete();
            }
            return shader;
        }

    }
}
