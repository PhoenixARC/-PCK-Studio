/*
 * 
 *The MIT License (MIT)

Copyright (c) 2016 Kareem Morsy

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
https://github.com/KareemMAX/Minecraft-Skiner
https://github.com/KareemMAX/Minecraft-Skiner/blob/master/src/Minecraft%20skiner/UserControls/Renderer3D.vb
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PckStudio.Properties;
using PckStudio.Rendering.Camera;
using PckStudio.Rendering.Shader;

namespace PckStudio.Rendering
{
    internal class SceneViewport : GLControl
    {
        /// <summary>
        /// Refresh rate at which the frame is updated. Default is 50(Hz)
        /// </summary>
        public int RefreshRate
        {
            get => refreshRate;
            set
            {
                refreshRate = Math.Max(value, 1);
                timer.Interval = TimeSpan.FromSeconds(1d / refreshRate).Milliseconds;
            }
        }

        protected PerspectiveCamera Camera { get; }
        protected EventHandler OnTimerTick { get; set; }

        private int refreshRate = 60;
        private Timer timer;

        private VertexArray VAO;
        private VertexBuffer VBO;
        private IndexBuffer IBO;
        private ShaderProgram colorShader;
        private bool isInitialized;

        protected void Init()
        {
            if (isInitialized)
            {
                Debug.Fail("Already Initializted.");
                return;
            }
            colorShader = ShaderProgram.Create(Resources.plainColorVertexShader, Resources.plainColorFragmentShader);
            VAO = new VertexArray();
            VBO = new VertexBuffer();
            IBO = IndexBuffer.Create(
                    0, 1,
                    1, 2,
                    2, 3,
                    3, 0,

                    4, 5,
                    5, 6,
                    6, 7,
                    7, 4,

                    0, 4,
                    1, 5,
                    2, 6,
                    3, 7);
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.Add(ShaderDataType.Float3);
            layout.Add(ShaderDataType.Float4);
            VAO.AddBuffer(VBO, layout);
            isInitialized = true;
        }

        public SceneViewport() : base()
        {
            timer = new Timer();
            RefreshRate = refreshRate;
            timer.Tick += TimerTick;
            timer.Start();
            timer.Enabled = !DesignMode;
            Camera = new PerspectiveCamera(60f, new Vector3(0f, 0f, 0f));
            VSync = true;
            isInitialized = false;
        }


        protected void DrawBoundingBox(Matrix4 transform, BoundingBox boundingBox, Color color)
        {
            colorShader.Bind();
            Matrix4 viewProjection = Camera.GetViewProjection();
            colorShader.SetUniformMat4("ViewProjection", ref viewProjection);
            colorShader.SetUniformMat4("Transform", ref transform);
            colorShader.SetUniform4("baseColor", color);
            colorShader.SetUniform1("intensity", 0.6f);

            GL.Enable(EnableCap.LineSmooth);

            GL.DepthFunc(DepthFunction.Always);

            Renderer.SetLineWidth(2f);

            Vector3 s = boundingBox.Start;
            Vector3 e = boundingBox.End;

            ColorVertex[] vertices = [
                new ColorVertex(new Vector3(s.X, e.Y, e.Z)),
                new ColorVertex(new Vector3(e.X, e.Y, e.Z)),
                new ColorVertex(new Vector3(e.X, s.Y, e.Z)),
                new ColorVertex(new Vector3(s.X, s.Y, e.Z)),
                new ColorVertex(new Vector3(s.X, e.Y, s.Z)),
                new ColorVertex(new Vector3(e.X, e.Y, s.Z)),
                new ColorVertex(new Vector3(e.X, s.Y, s.Z)),
                new ColorVertex(new Vector3(s.X, s.Y, s.Z)),
            ];

            VAO.Bind();
            VBO.SetData(vertices);
            IBO.Bind();
            GL.DrawElements(PrimitiveType.Lines, IBO.GetCount(), DrawElementsType.UnsignedInt, 0);
            GL.DepthFunc(DepthFunction.Less);
            Renderer.SetLineWidth(1f);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            OnTimerTick?.Invoke(sender, e);
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (DesignMode)
                return;
            MakeCurrent();
            if (Camera is not null)
            {
                Camera.ViewportSize = ClientSize;
            }
            Renderer.SetViewportSize(Camera.ViewportSize);
        }
    }
}  
