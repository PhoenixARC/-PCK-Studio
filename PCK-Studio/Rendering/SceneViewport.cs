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
            get => _refreshRate;
            set
            {
                _refreshRate = Math.Max(value, 1);
                _timer.Interval = TimeSpan.FromSeconds(1d / _refreshRate).Milliseconds;
            }
        }

        protected PerspectiveCamera Camera { get; }

        protected virtual void OnUpdate(object sender, TimeSpan timestep)
        {
            SwapBuffers();
        }

        private int _refreshRate = 120;
        private Timer _timer;
        private Stopwatch _stopwatch;
        private bool _initialized;
        private ShaderLibrary _shaderLibrary;

        private DrawContext _boundingBoxDrawContext;

        private long _lastTick = 0L;

        private IndexBuffer _meshIndexBuffer;
        private Dictionary<Type, VertexArray> _meshTypeVertexArray;

#if USE_FRAMEBUFFER
        private FrameBuffer _framebuffer;
        private Texture2D _framebufferTexture;
        private VertexArray _framebufferVAO;
        private int _framebufferRenderBuffer;
        private bool _framebufferStarted = false;

        private static Vector4[] _rectVertices = new Vector4[]
        {
            new Vector4( 1.0f, -1.0f, 1.0f, 0.0f),
            new Vector4(-1.0f, -1.0f, 0.0f, 0.0f),
            new Vector4(-1.0f,  1.0f, 0.0f, 1.0f),
            new Vector4( 1.0f,  1.0f, 1.0f, 1.0f),
            new Vector4( 1.0f, -1.0f, 1.0f, 0.0f),
            new Vector4(-1.0f,  1.0f, 0.0f, 1.0f),
        };
#endif

        public SceneViewport() : base()
        {
            VSync = true;
            RefreshRate = _refreshRate;
            _stopwatch = new Stopwatch();
            _timer = new Timer();
            _timer.Tick += TimerTick;
            if (!DesignMode)
            {
                _timer.Start();
                _stopwatch.Start();
            }

            Camera = new PerspectiveCamera(60f, new Vector3(0f, 0f, 0f));
            _shaderLibrary = new ShaderLibrary();
            _initialized = false;
        }

        protected void Initialize()
        {
            if (_initialized)
            {
                Debug.Fail("Already Initialized.");
                return;
            }
            MakeCurrent();
            AddShader("Internal_colorShader", ShaderProgram.Create(Resources.plainColorVertexShader, Resources.plainColorFragmentShader));
            var vao = new VertexArray();
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.Add(ShaderDataType.Float3);
            layout.Add(ShaderDataType.Float4);
            vao.AddNewBuffer(layout);
            var ibo = IndexBuffer.Create(BoundingBox.GetIndecies());
            _boundingBoxDrawContext = new DrawContext(vao, ibo, PrimitiveType.Lines);

            _meshTypeVertexArray = new Dictionary<Type, VertexArray>();
            _meshIndexBuffer = new IndexBuffer();

#if USE_FRAMEBUFFER
            InitializeFramebuffer();
            // Framebuffer shader
            {
                var framebufferShader = ShaderProgram.Create(Resources.framebufferVertexShader, Resources.framebufferFragmentShader);
                framebufferShader.Bind();
                framebufferShader.SetUniform1("screenTexture", 0);
                framebufferShader.Validate();
                AddShader("Internal_framebufferShader", framebufferShader);
            
                GLErrorCheck();
            }
#endif

            _initialized = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Stop();
                _stopwatch.Stop();
                _timer.Dispose();
            }
            MakeCurrent();
            foreach (VertexArray va in _meshTypeVertexArray.Values)
            {
                va.Dispose();
            }
            _meshIndexBuffer.Dispose();
            _boundingBoxDrawContext.IndexBuffer.Dispose();
            _boundingBoxDrawContext.VertexArray.Dispose();
            _shaderLibrary.Dispose();
            _initialized = false;
            base.Dispose(disposing);
        }

        protected void AddShader(string shaderName, ShaderProgram shader) => _shaderLibrary.AddShader(shaderName, shader);

        protected void AddShader(string shaderName, string vertexSource, string fragmentSource) => AddShader(shaderName, ShaderProgram.Create(vertexSource, fragmentSource));

        protected ShaderProgram GetShader(string shaderName) => _shaderLibrary.GetShader(shaderName);

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
            ShaderProgram colorShader = _shaderLibrary.GetShader("Internal_colorShader");
            colorShader.Bind();
            Matrix4 viewProjection = Camera.GetViewProjection();
            colorShader.SetUniformMat4("ViewProjection", ref viewProjection);
            colorShader.SetUniformMat4("Transform", ref transform);
            colorShader.SetUniform4("baseColor", color);
            colorShader.SetUniform1("intensity", 0.6f);

            GL.Enable(EnableCap.LineSmooth);

            GL.DepthFunc(DepthFunction.Always);

            Renderer.SetLineWidth(2f);
            _boundingBoxDrawContext.VertexArray.GetBuffer(0).SetData(boundingBox.GetVertices());
            Renderer.Draw(colorShader, _boundingBoxDrawContext);

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
            _framebuffer = new FrameBuffer();
            _framebuffer.Bind();
            _framebufferTexture = new Texture2D(0);
            _framebufferTexture.PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgb;
            _framebufferTexture.InternalPixelFormat = PixelInternalFormat.Rgb;
            _framebufferTexture.SetSize(Size);
            _framebufferTexture.WrapS = TextureWrapMode.ClampToEdge;
            _framebufferTexture.WrapT = TextureWrapMode.ClampToEdge;
            _framebufferTexture.MinFilter = TextureMinFilter.Nearest;
            _framebufferTexture.MagFilter = TextureMagFilter.Nearest;

            _framebufferTexture.AttachToFramebuffer(_framebuffer, FramebufferAttachment.ColorAttachment0);

            _framebufferRenderBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _framebufferRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.Width, Size.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _framebufferRenderBuffer);

            _framebufferVAO = new VertexArray();
            VertexBuffer vertexBuffer = new VertexBuffer();
            vertexBuffer.SetData(_rectVertices);
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.Add(ShaderDataType.Float4);
            _framebufferVAO.AddBuffer(vertexBuffer, layout);
            _framebuffer.CheckStatus();

            if (_framebuffer.Status != FramebufferErrorCode.FramebufferComplete)
            {
                Debug.Fail($"Framebuffer status: '{_framebuffer.Status}'");
            }

            _framebuffer.Unbind();
#endif
        }

        [Conditional("USE_FRAMEBUFFER")]
        protected void FramebufferBegin()
        {
#if USE_FRAMEBUFFER
            if (_framebufferStarted)
                Debug.Fail("FramebufferBegin: already begun.");
            _framebufferStarted = true;
            _framebuffer.Bind();
#endif
        }

        [Conditional("USE_FRAMEBUFFER")]
        protected void FramebufferEnd()
        {
#if USE_FRAMEBUFFER
            if (!_framebufferStarted)
                Debug.Fail("FramebufferEnd: framebuffer didn't start yet.");
            _framebuffer.Unbind();
            GL.Disable(EnableCap.DepthTest);
            ShaderProgram framebufferShader = GetShader("Internal_framebufferShader");
            framebufferShader.Bind();
            _framebufferVAO.Bind();
            _framebufferTexture.Bind();

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            _framebufferTexture.Unbind();
            _framebufferStarted = false;
#endif
        }

#if USE_FRAMEBUFFER
        [Conditional("USE_FRAMEBUFFER")]
        private void SetFramebufferSize(Size size)
        {
            MakeCurrent();
            if (_framebuffer is not null)
            {
                _framebuffer.Bind();

                _framebufferTexture.Bind();
                _framebufferTexture.SetSize(size);

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _framebufferRenderBuffer);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, size.Width, size.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _framebufferRenderBuffer);

                FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                if (status != FramebufferErrorCode.FramebufferComplete)
                {
                    Debug.Fail($"Framebuffer status: '{_framebuffer.Status}'");
                }
                _framebuffer.Unbind();
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
