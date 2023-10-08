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
        public static void Draw(VertexArray va, IndexBuffer ib, Shader shader)
        {
            shader.Bind();
            va.Bind();
            ib.Bind();
            GL.DrawElements(PrimitiveType.Triangles, ib.GetCount(), DrawElementsType.UnsignedInt, 0);
        }
        
        public static void Draw(VertexArray va, Shader shader)
        {
            shader.Bind();
            va.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}
