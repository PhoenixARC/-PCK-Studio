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
        /// Refresh rate at which the frame is updated. Default is 60(Hz)
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

        protected class TimestepEventArgs : EventArgs
        {
            public readonly float Delta;

            public TimestepEventArgs(double seconds)
            {
                Delta = (float)seconds;
            }
        }

        protected virtual void OnUpdate(object sender, TimestepEventArgs e)
        {

        }

        private int refreshRate = 120;
        private Timer timer;
        private Stopwatch stopwatch;
        private bool isInitialized;

        private ShaderProgram colorShader;
        private DrawContext boundingBoxDrawContext;

        protected void Init()
        {
            if (isInitialized)
            {
                Debug.Fail("Already Initializted.");
                return;
            }
            MakeCurrent();
            colorShader = ShaderProgram.Create(Resources.plainColorVertexShader, Resources.plainColorFragmentShader);
            var vao = new VertexArray();
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.Add(ShaderDataType.Float3);
            layout.Add(ShaderDataType.Float4);
            vao.AddNewBuffer(layout);
            var ibo = IndexBuffer.Create(BoundingBox.GetIndecies());
            boundingBoxDrawContext = new DrawContext(vao, ibo, PrimitiveType.Lines);
            isInitialized = true;
        }

        public SceneViewport() : base()
        {
            timer = new Timer();
            stopwatch = new Stopwatch();
            RefreshRate = refreshRate;
            timer.Tick += TimerTick;
            if (!DesignMode)
            {
                timer.Start();
                stopwatch.Start();
            }

            Camera = new PerspectiveCamera(60f, new Vector3(0f, 0f, 0f));
            VSync = true;
            isInitialized = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Stop();
                stopwatch.Stop();
                timer.Dispose();
            }
            MakeCurrent();
            boundingBoxDrawContext.Dispose();
            colorShader.Dispose();
            isInitialized = false;
            base.Dispose(disposing);
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
            boundingBoxDrawContext.VertexArray.GetBuffer(0).SetData(boundingBox.GetVertices());
            Renderer.Draw(colorShader, boundingBoxDrawContext);

            GL.DepthFunc(DepthFunction.Less);
            Renderer.SetLineWidth(1f);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            stopwatch.Stop();
            double deltaTime = stopwatch.Elapsed.TotalMilliseconds;
            OnUpdate(sender, new TimestepEventArgs(deltaTime));
            stopwatch.Restart();
            Refresh();
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
