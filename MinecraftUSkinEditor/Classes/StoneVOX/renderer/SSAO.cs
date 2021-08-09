using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Windows.Forms;

namespace stonevox
{
    public class SSAO : Singleton<SSAO>, IRenderer
    {
        class IOBuffer
        {
            public int fboID;
            public int texture;
            public int depth;

            public IOBuffer(int width, int height, bool depthBuffer, PixelInternalFormat format)
            {
                fboID = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboID);

                PixelFormat pixelFormat = PixelFormat.Rgb;

                switch (format)
                {
                    case PixelInternalFormat.Rgb32f:
                        pixelFormat = PixelFormat.Rgb;
                        break;
                    case PixelInternalFormat.R32f:
                        pixelFormat = PixelFormat.Red;
                        break;
                }

                texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, format, width, height, 0, pixelFormat, PixelType.Float, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);
                //DrawBuffersEnum[] DrawBuffers = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 };
                //GL.DrawBuffers(DrawBuffers.Length, DrawBuffers);

                if (depthBuffer)
                {
                    depth = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, depth);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
                    GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depth, 0);
                }

                var errorcode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

                if (errorcode != FramebufferErrorCode.FramebufferComplete)
                {
                    MessageBox.Show("StoneVox Error", string.Format("Framebuffer failed : {0}", errorcode.ToString()));
                }

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

            public void BindFBOWritting()
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboID);
            }

            public void BindFBOReading(TextureUnit unit)
            {
                GL.ActiveTexture(unit);
                GL.BindTexture(TextureTarget.Texture2D, texture);
            }
        }

        IOBuffer geometryBuffer;
        IOBuffer SSAOBuffer;

        Shader ssao_geometryDepth;
        Shader ssao;
        Shader ssao_voxel;

        int quadBuffer;

        float[] quadData = new float[]
        {
            -1.0f, -1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
            1.0f, 1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f,
        };

        private Camera camera;
        private Random random;

        public SSAO(int width, int height, Camera camera)
            :base()
        {
            this.camera = camera;

            random = new Random();

            geometryBuffer = new IOBuffer(width, height, true, PixelInternalFormat.Rgb32f);
            SSAOBuffer = new IOBuffer(width, height, false, PixelInternalFormat.R32f);

            ssao_geometryDepth = ShaderUtil.CreateShader("ssao_geometryDepth", "./data/shaders/SSAO_geometry.vs", "./data/shaders/SSAO_geometry.fs");
            ssao= ShaderUtil.CreateShader("ssao", "./data/shaders/SSAO_pass.vs", "./data/shaders/SSAO_pass.fs");
            ssao_voxel = ShaderUtil.CreateShader("qb", "./data/shaders/SSAO_voxel.vs", "./data/shaders/SSAO_voxel.fs");

            ssao.WriteUniform("gSampleRad", 1.25f);
            ssao.WriteUniform("gProj", camera.projection);
            ssao.WriteUniform("gPositionMap", 1);

            ssao_voxel.WriteUniform("gScreenSize", new Vector2(width, height));
            ssao_voxel.WriteUniform("gAOMap", 2);

            Vector3[] kernel = new Vector3[8];

            for (uint i = 0; i < kernel.Length; i++)
            {
                float scale = (float)i / (float)(kernel.Length);
                Vector3 v;
                v.X = 2.0f * (float)random.Next(0x7fff) / 0x7fff - 1.0f;
                v.Y = 2.0f * (float)random.Next(0x7fff) / 0x7fff - 1.0f;
                v.Z = 2.0f * (float)random.Next(0x7fff) / 0x7fff - 1.0f;
                v *= (0.1f + 0.9f * scale * scale);

                kernel[i] = v;
            }

            unsafe
            {
                fixed (float* pointer = &kernel[0].X)
                {
                    ssao.WriteUniformArray("gKernel", kernel.Length, pointer);
                }
            }

            quadBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadBuffer);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * quadData.Length), quadData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Render(QbModel model)
        {
            ssao_geometryDepth.UseShader();
            geometryBuffer.BindFBOWritting();
            ssao_geometryDepth.WriteUniform("gWVP", camera.modelviewprojection);
            ssao_geometryDepth.WriteUniform("gWV", camera.view);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            model.Render();

            ssao.UseShader();
            geometryBuffer.BindFBOReading(TextureUnit.Texture1);
            SSAOBuffer.BindFBOWritting();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadBuffer);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            ssao_voxel.UseShader();
            SSAOBuffer.BindFBOReading(TextureUnit.Texture2);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ssao_voxel.WriteUniform("modelview", camera.modelviewprojection);
            model.Render(ssao_voxel);
        }
    }
}
