using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Rendering.Shader
{
    internal sealed class ShaderLibrary
    {
        private Dictionary<string, ShaderProgram> _shaderStorage = new Dictionary<string, ShaderProgram>();

        public void AddShader(string name, ShaderProgram shader) => _shaderStorage.Add(name, shader);

        public bool HasShader(string name) => _shaderStorage.TryGetValue(name, out _);

        public bool HasShader(string name, out ShaderProgram shader) => _shaderStorage.TryGetValue(name, out shader);
        
        public ShaderProgram GetShader(string name) => _shaderStorage[name];

        public void RemoveShader(string name) => _shaderStorage.Remove(name);
    }
}
