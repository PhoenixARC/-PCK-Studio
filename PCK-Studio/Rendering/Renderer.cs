using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    internal static class Renderer
    {
        public static void Draw(Shader shader, VertexArray va, IndexBuffer ib, PrimitiveType type)
        {
            shader.Bind();
            va.Bind();
            ib.Bind();
            GL.DrawElements(type, ib.GetCount(), DrawElementsType.UnsignedInt, 0);
        }
    }
}
