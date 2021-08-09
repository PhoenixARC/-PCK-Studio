using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;

namespace stonevox
{
    public class SmoothBackground : Appearence
    {

        static Shader quadInterpolation;
        static Camera camera;
        static SmoothBackground()
        {
           quadInterpolation = ShaderUtil.GetShader("quad_interpolation");
            camera = Singleton<Camera>.INSTANCE;
        }

        public Color4[] colors = new Color4[4];

        int arrayID;
        int bufferID;
        float[] data;

        public override void Initialize()
        {
            // bottom left
            colors[0] = Color4.Black;

            //bottom riht
            colors[1] = new Color4(.5f, 0, 0, 1);

            // top right
            colors[2] = Color4.Red;

            // top left
            colors[3] = Color4.White;

            data = new float[28];

            bufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * data.Length), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            arrayID = GL.GenVertexArray();
            GL.BindVertexArray(arrayID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(quadInterpolation.getartributelocation("position"), 2, VertexAttribPointerType.Float, false, sizeof(float) * 7, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(quadInterpolation.getartributelocation("in_uv"), 2, VertexAttribPointerType.Float, false, sizeof(float) * 7, sizeof(float) * 2);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(quadInterpolation.getartributelocation("in_color"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 7, sizeof(float) * 4);

            GL.BindVertexArray(0);
        }

        public override void Render(float x, float y, float width, float height)
        {
            quadInterpolation.UseShader();
            quadInterpolation.WriteUniform("mvp", Matrix4.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, -1, 1));

            data[0] = x;
            data[1] = y;
            data[2] = 0;
            data[3] = 1;
            data[4] = colors[0].R;
            data[5] = colors[0].G;
            data[6] = colors[0].B;

            data[7] = x + width;
            data[8] = y;
            data[9] = 1;
            data[10] = 1;
            data[11] = colors[1].R;
            data[12] = colors[1].G;
            data[13] = colors[1].B;

            data[14] = x + width;
            data[15] = y + height;
            data[16] = 1;
            data[17] = 0;
            data[18] = colors[2].R;
            data[19] = colors[2].G;
            data[20] = colors[2].B;

            data[21] = x;
            data[22] = y + height;
            data[23] = 0;
            data[24] = 0;
            data[25] = colors[3].R;
            data[26] = colors[3].G;
            data[27] = colors[3].B;

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) *data.Length), data, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(arrayID);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.BindVertexArray(0);


            ShaderUtil.ResetShader();
        }

        public void SetColor(Color4 color)
        {
            colors[2] = color;

            double h, s, v;
            ColorConversion.ColorToHSV(color.ToSystemDrawingColor(), out h, out s, out v);

            colors[1] = color;
        }

        public override Appearence FromData(AppearenceData data)
        {
            return null;
        }
        public override AppearenceData ToData()
        {
            return null;
        }
    }
}
