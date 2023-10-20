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
        public static void Draw(Shader shader, RenderBuffer renderBuffer)
        {
            shader.Bind();
            renderBuffer.VertexArray.Bind();
            renderBuffer.IndexBuffer.Bind();
            GL.DrawElements(renderBuffer.PrimitiveType, renderBuffer.IndexBuffer.GetCount(), DrawElementsType.UnsignedInt, 0);
        }
    }
}
