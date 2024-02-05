﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal class Shader : IDisposable
    {
        private int _programId;
        private Dictionary<string, int> locationCache = new Dictionary<string, int>();

        private Shader(int programId)
        {
            _programId = programId;
        }

        public void Bind()
        {
            GL.UseProgram(_programId);
        }

        [Conditional("DEBUG")]
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

        public static Shader Create(string vertexSource, string fragmentSource)
        {
            return Create(
                new ShaderSource(ShaderType.VertexShader, vertexSource),
                new ShaderSource(ShaderType.FragmentShader, fragmentSource)
                );
        }

        private bool Link()
        {
            GL.LinkProgram(_programId);
            GL.GetProgram(_programId, GetProgramParameterName.LinkStatus, out int status);
            bool success = status != 0;
            if (!success)
                Debug.WriteLine(GL.GetProgramInfoLog(_programId), category: nameof(Shader));
            return success;
        }

        public bool Validate()
        {
#if DEBUG
            GL.ValidateProgram(_programId);
            GL.GetProgram(_programId, GetProgramParameterName.ValidateStatus, out int status);
            bool success = status != 0;
            if (!success)
                Debug.WriteLine(GL.GetProgramInfoLog(_programId), category: nameof(Shader));
            return success;
#else
            return true;
#endif
        }

        public static Shader Create(params ShaderSource[] shaderSources)
        {
            int programId = GL.CreateProgram();

            var shaderIds = new List<int>(shaderSources.Length);

            foreach (var shaderSource in shaderSources)
            {
                int shaderId = CompileShader(shaderSource.Type, shaderSource.Source);
                GL.AttachShader(programId, shaderId);
                shaderIds.Add(shaderId);
            }

            var shader = new Shader(programId);
            bool success = shader.Link();
            Debug.Assert(success, "Shader Program linking failed.");
            
            foreach (var shaderId in shaderIds)
            {
                GL.DeleteShader(shaderId);
            }
            return shader;
        }

    }
}
