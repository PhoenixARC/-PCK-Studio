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
#define USE_FRAMEBUFFER
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PckStudio.Properties;
using PckStudio.Rendering.Camera;
using PckStudio.Rendering.Shader;
using PckStudio.Rendering.Texture;

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

        protected virtual void OnUpdate(object sender, TimeSpan timestep)
        {
            SwapBuffers();
        }

        private int refreshRate = 120;
        private Timer timer;
        private Stopwatch stopwatch;
        private bool isInitialized;

        private ShaderProgram colorShader;
        private DrawContext boundingBoxDrawContext;

        private long _lastTick = 0L;

#if USE_FRAMEBUFFER
        private FrameBuffer framebuffer;
        private Texture2D framebufferTexture;
        private ShaderProgram framebufferShader;
        private VertexArray framebufferVAO;
        private int framebufferRenderBuffer;
        private bool _started = false;

        private static Vector4[] rectVertices = new Vector4[]
        {
            new Vector4( 1.0f, -1.0f, 1.0f, 0.0f),
            new Vector4(-1.0f, -1.0f, 0.0f, 0.0f),
            new Vector4(-1.0f,  1.0f, 0.0f, 1.0f),
            new Vector4( 1.0f,  1.0f, 1.0f, 1.0f),
            new Vector4( 1.0f, -1.0f, 1.0f, 0.0f),
            new Vector4(-1.0f,  1.0f, 0.0f, 1.0f),
        };
#endif

        protected void Initialize()
        {
            if (isInitialized)
            {
                Debug.Fail("Already Initialized.");
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

            _meshTypeVertexArray = new Dictionary<Type, VertexArray>();
            _meshIndexBuffer = new IndexBuffer();

#if USE_FRAMEBUFFER
            InitializeFramebuffer();
            // Framebuffer shader
            {
                framebufferShader = ShaderProgram.Create(Resources.framebufferVertexShader, Resources.framebufferFragmentShader);
                framebufferShader.Bind();
                framebufferShader.SetUniform1("screenTexture", 0);
                framebufferShader.Validate();
            
                GLErrorCheck();
            }
#endif

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
            foreach (VertexArray va in _meshTypeVertexArray.Values)
            {
                va.Dispose();
            }
            _meshIndexBuffer.Dispose();
            boundingBoxDrawContext.IndexBuffer.Dispose();
            boundingBoxDrawContext.VertexArray.Dispose();
            colorShader.Dispose();
            isInitialized = false;
            base.Dispose(disposing);
        }

        private IndexBuffer _meshIndexBuffer;
        private Dictionary<Type, VertexArray> _meshTypeVertexArray;

        protected void DrawMesh<T>(GenericMesh<T> mesh, ShaderProgram shader) where T : struct
        {
            if (!_meshTypeVertexArray.ContainsKey(typeof(T)))
            {
                VertexArray vertexArray = new VertexArray();
                vertexArray.AddNewBuffer(mesh.VertexLayout);
                _meshTypeVertexArray.Add(typeof(T), vertexArray);
            }
            T[] vertices = mesh.GetVertices().ToArray();
            _meshTypeVertexArray[typeof(T)].GetBuffer(0).SetData(vertices);
            int[] indices = mesh.GetIndices().ToArray();
            _meshIndexBuffer.SetIndicies(indices);
            var drawContext = new DrawContext(_meshTypeVertexArray[typeof(T)], _meshIndexBuffer, mesh.DrawType);
            Renderer.Draw(shader, drawContext);
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


        [Conditional("DEBUG")]
        protected void GLErrorCheck()
        {
            ErrorCode error = GL.GetError();
            Debug.Assert(error == ErrorCode.NoError, error.ToString());
        }

        [Conditional("USE_FRAMEBUFFER")]
        private void InitializeFramebuffer()
        {
#if USE_FRAMEBUFFER
            framebuffer = new FrameBuffer();
            framebuffer.Bind();
            framebufferTexture = new Texture2D(0);
            framebufferTexture.PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgb;
            framebufferTexture.InternalPixelFormat = PixelInternalFormat.Rgb;
            framebufferTexture.SetSize(Size);
            framebufferTexture.WrapS = TextureWrapMode.ClampToEdge;
            framebufferTexture.WrapT = TextureWrapMode.ClampToEdge;
            framebufferTexture.MinFilter = TextureMinFilter.Nearest;
            framebufferTexture.MagFilter = TextureMagFilter.Nearest;

            framebufferTexture.AttachToFramebuffer(framebuffer, FramebufferAttachment.ColorAttachment0);

            framebufferRenderBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, framebufferRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.Width, Size.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, framebufferRenderBuffer);

            framebufferVAO = new VertexArray();
            VertexBuffer vertexBuffer = new VertexBuffer();
            vertexBuffer.SetData(rectVertices);
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.Add(ShaderDataType.Float4);
            framebufferVAO.AddBuffer(vertexBuffer, layout);
            framebuffer.CheckStatus();

            if (framebuffer.Status != FramebufferErrorCode.FramebufferComplete)
            {
                Debug.Fail($"Framebuffer status: '{framebuffer.Status}'");
            }

            framebuffer.Unbind();
#endif
        }

        [Conditional("USE_FRAMEBUFFER")]
        protected void FramebufferBegin()
        {
#if USE_FRAMEBUFFER
            if (_started)
                Debug.Fail("FramebufferBegin: already begun.");
            _started = true;
            framebuffer.Bind();
#endif
        }

        [Conditional("USE_FRAMEBUFFER")]
        protected void FramebufferEnd()
        {
#if USE_FRAMEBUFFER
            if (!_started)
                Debug.Fail("FramebufferEnd: framebuffer didn't start yet.");
            framebuffer.Unbind();
            GL.Disable(EnableCap.DepthTest);
            framebufferShader.Bind();
            framebufferVAO.Bind();
            framebufferTexture.Bind();

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            framebufferTexture.Unbind();
            _started = false;
#endif
        }

#if USE_FRAMEBUFFER
        [Conditional("USE_FRAMEBUFFER")]
        private void SetFramebufferSize(Size size)
        {
            MakeCurrent();
            if (framebuffer is not null)
            {
                framebuffer.Bind();

                framebufferTexture.Bind();
                framebufferTexture.SetSize(size);

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, framebufferRenderBuffer);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, size.Width, size.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, framebufferRenderBuffer);

                FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                if (status != FramebufferErrorCode.FramebufferComplete)
                {
                    Debug.Fail($"Framebuffer status: '{framebuffer.Status}'");
                }
                framebuffer.Unbind();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!IsHandleCreated || DesignMode)
                return;
            SetFramebufferSize(Size);
        }
#endif

        private void TimerTick(object sender, EventArgs e)
        {
            long tick = DateTime.UtcNow.Ticks - _lastTick;
            Refresh();
            _lastTick = DateTime.UtcNow.Ticks;
            OnUpdate(sender, TimeSpan.FromTicks(tick));
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
