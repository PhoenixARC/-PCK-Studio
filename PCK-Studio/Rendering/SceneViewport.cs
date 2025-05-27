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
//#define USE_FRAMEBUFFER
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PckStudio.Extensions;
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
            get => _refreshRate;
            set
            {
                _refreshRate = Math.Max(value, 1);
                _timer.Interval = TimeSpan.FromSeconds(1d / _refreshRate).Milliseconds;
            }
        }

        public new Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                if (!DesignMode)
                {
                    Renderer.SetClearColor(value);
                }
            }
        }

        protected new bool DesignMode => base.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        protected PerspectiveCamera Camera { get; }

        protected virtual void OnUpdate(object sender, TimeSpan timestep) { }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool LockMousePosition { get; set; } = false;

        public float MouseSensetivity { get; set; } = 0.01f;
        private Point PreviousMouseLocation;
        private Point CurrentMouseLocation;

        private int _refreshRate = 60;
        private Timer _timer;
        private bool _initialized;
        private ShaderLibrary _shaderLibrary;

        private DrawContext _boundingBoxDrawContext;

        private long _lastTick = 0L;

        private IndexBuffer _meshIndexBuffer;
        private Dictionary<Type, VertexArray> _meshTypeVertexArray;

        private bool IsMouseHidden
        {
            get => !Cursor.IsVisible();
            set
            {
                if (value)
                {
                    Cursor.Hide();
                    return;
                }
                Cursor.Show();
            }
        }

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

        private SceneViewport()
        {

        }

        public SceneViewport(float fov, Vector3 camareaPosition = default)
#if DEBUG
            : base(GraphicsMode.Default, 4, 6, GraphicsContextFlags.Debug)
#else
            : base()
#endif
        {
            VSync = true;
            _timer = new Timer();
            _timer.Tick += TimerTick;

            RefreshRate = _refreshRate;
            Camera = new PerspectiveCamera(fov, camareaPosition);
            _shaderLibrary = new ShaderLibrary();

            if (!DesignMode)
            {
                _timer.Start();
                InitializeInternal();
            }
            _initialized = false;
        }

        private void InitializeInternal()
        {
            if (_initialized)
            {
                Debug.Fail("Already Initialized.");
                return;
            }
            MakeCurrent();
            Trace.TraceInformation(GL.GetString(StringName.Version));
            GL.DebugMessageCallback(DebugProc, this.Handle);
            AddShader("Internal_colorShader", Resources.plainColorVertexShader, Resources.plainColorFragmentShader);
            var vao = new VertexArray();
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.Add(ShaderDataType.Float3);
            layout.Add(ShaderDataType.Float4);
            int id = vao.AddNewBuffer(layout);
            vao.GetBuffer(id).SetData(BoundingBox.GetVertices());
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
            InitializeDebugComponents();
            InitializeDebugShaders();
            _initialized = true;
        }

        public void ResetCamera() => ResetCamera(Vector3.Zero);
        public virtual void ResetCamera(Vector3 defaultPosition)
        {
            Camera.FocalPoint = defaultPosition;
            Camera.Yaw = 0f;
            Camera.Pitch = 0f;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
#if DEBUG
                case Keys.Escape:
                    ReleaseMouse();
                    debugContextMenuStrip1.Show(this, Point.Empty);
                    return true;
#endif
            }
            return base.ProcessDialogKey(keyData);
        }

        protected override void Dispose(bool disposing)
        {
            if (DesignMode)
                return;
            if (disposing)
            {
                _timer.Stop();
                _timer.Dispose();
                foreach (VertexArray va in _meshTypeVertexArray.Values)
                {
                    va.Dispose();
                }
                _meshIndexBuffer.Dispose();
                _boundingBoxDrawContext.IndexBuffer.Dispose();
                _boundingBoxDrawContext.VertexArray.Dispose();
                _shaderLibrary.Dispose();
            }
            _initialized = false;
            base.Dispose(disposing);
        }

        protected void AddShader(string shaderName, ShaderProgram shader) => _shaderLibrary.AddShader(shaderName, shader);

        protected void AddShader(string shaderName, string vertexSource, string fragmentSource) => AddShader(shaderName, ShaderProgram.Create(vertexSource, fragmentSource));

        protected ShaderProgram GetShader(string shaderName) => _shaderLibrary.GetShader(shaderName);

        protected void DrawMesh<T>(GenericMesh<T> mesh, ShaderProgram shader, Matrix4 transform) where T : struct
        {
            Matrix4 viewProjection = Camera.GetViewProjection();
            shader.Bind();
            shader.SetUniformMat4("ViewProjection", ref viewProjection);
            shader.SetUniformMat4("Transform", ref transform);
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
            transform = boundingBox.GetTransform() * transform;
            colorShader.SetUniformMat4("Transform", ref transform);
            colorShader.SetUniform4("BlendColor", color);
            colorShader.SetUniform1("Intensity", 0.6f);

            GL.Enable(EnableCap.LineSmooth);

            GL.DepthFunc(DepthFunction.Always);

            Renderer.SetLineWidth(2f);
            Renderer.Draw(colorShader, _boundingBoxDrawContext);

            GL.DepthFunc(DepthFunction.Less);
            Renderer.SetLineWidth(1f);
        }

        static void DebugProc(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr instanceHandle)
        {
            string dbgMessage = Marshal.PtrToStringAnsi(message, length);
            Debug.WriteLine($"{source}:{id} {type} {severity}: {dbgMessage}");
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
            _framebufferTexture = new Texture2D();
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
            _framebufferTexture.Bind(slot: 0);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            _framebufferTexture.Unbind();
            _framebufferStarted = false;
#endif
        }

#if USE_FRAMEBUFFER
        [Conditional("USE_FRAMEBUFFER")]
        private void SetFramebufferSize(Size size)
        {
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
            _lastTick = DateTime.UtcNow.Ticks;
            OnUpdate(sender, TimeSpan.FromTicks(tick));
            Refresh();
            RenderDebug();
            if (IsHandleCreated && !IsDisposed)
                SwapBuffers();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (DesignMode)
                return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest); // Enable correct Z Drawings
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (DesignMode)
                return;
            if (Camera is not null)
            {
                Camera.ViewportSize = ClientSize;
            }
            Renderer.SetViewportSize(Camera.ViewportSize);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            float mouseSensetivity = LockMousePosition ? MouseSensetivity : 64f * 3 * (1f / 56f) * MouseSensetivity;

            float deltaX = (Cursor.Position.X - CurrentMouseLocation.X) * mouseSensetivity;
            float deltaY = (Cursor.Position.Y - CurrentMouseLocation.Y) * mouseSensetivity;

            switch (e.Button)
            {
                case MouseButtons.None:
                case MouseButtons.Middle:
                case MouseButtons.XButton1:
                case MouseButtons.XButton2:
                    break;
                case MouseButtons.Left:
                    Camera.Rotate(deltaX, deltaY);
                    goto default;
                case MouseButtons.Right:
                    Camera.Pan(deltaX, deltaY);
                    goto default;
                default:
                    if (LockMousePosition)
                        Cursor.Position = PointToScreen(new Point((int)Math.Round(Bounds.Width / 2d), (int)Math.Round(Bounds.Height / 2d)));
                    CurrentMouseLocation = Cursor.Position;
                    break;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Camera.Distance -= e.Delta / System.Windows.Input.Mouse.MouseWheelDeltaForOneLine;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            ReleaseMouse();
        }

        protected void ReleaseMouse()
        {
            if (LockMousePosition)
            {
                Cursor.Position = PreviousMouseLocation;
                if (IsMouseHidden)
                    IsMouseHidden = false;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
            {
                CurrentMouseLocation = PreviousMouseLocation = Cursor.Position;
                IsMouseHidden = LockMousePosition;
            }
        }

        [Conditional("DEBUG")]
        private void InitializeDebugShaders()
        {
#if DEBUG
            var plainColorVertexBufferLayout = new VertexBufferLayout();
            plainColorVertexBufferLayout.Add(ShaderDataType.Float3);
            plainColorVertexBufferLayout.Add(ShaderDataType.Float4);
            // Debug point render
            {
                ColorVertex[] vertices = [
                    new ColorVertex(Vector3.Zero, Color.White),
                ];
                VertexArray vao = new VertexArray();
                var debugVBO = new VertexBuffer();
                debugVBO.SetData(vertices);
                vao.AddBuffer(debugVBO, plainColorVertexBufferLayout);
                d_debugPointDrawContext = new DrawContext(vao, debugVBO.GenIndexBuffer(), PrimitiveType.Points);
            }
            // Debug line render
            {
                ColorVertex[] vertices = [
                    new ColorVertex(Vector3.Zero, Color.Red)  , new ColorVertex(Vector3.UnitX, Color.Red),
                    new ColorVertex(Vector3.Zero, Color.Green), new ColorVertex(Vector3.UnitY, Color.Green),
                    new ColorVertex(Vector3.Zero, Color.Blue) , new ColorVertex(Vector3.UnitZ, Color.Blue),
                ];
                VertexArray vao = new VertexArray();
                var debugVBO = new VertexBuffer();
                debugVBO.SetData(vertices);
                vao.AddBuffer(debugVBO, plainColorVertexBufferLayout);
                d_debugLineDrawContext = new DrawContext(vao, debugVBO.GenIndexBuffer(), PrimitiveType.Lines);
            }
#endif
        }

        [Conditional("DEBUG")]
        private void RenderDebug()
        {
#if DEBUG
            d_debugLabel.Text = Camera.ToString();
            GL.Disable(EnableCap.Blend);
            GL.DepthMask(false);
            GL.DepthFunc(DepthFunction.Always);
            GL.Enable(EnableCap.PointSmooth);
            GL.Enable(EnableCap.LineSmooth);
            ShaderProgram colorShader = GetShader("Internal_colorShader");
            colorShader.Bind();
            Matrix4 viewProjection = Camera.GetViewProjection();
            colorShader.SetUniformMat4("ViewProjection", ref viewProjection);
            if (d_showFocalPoint)
            {
                Matrix4 transform = Matrix4.CreateTranslation(Camera.FocalPoint).Inverted();
                colorShader.SetUniformMat4("Transform", ref transform);
                colorShader.SetUniform1("Intensity", 0.75f);
                colorShader.SetUniform4("BlendColor", Color.DeepPink);
                GL.PointSize(5f);
                Renderer.Draw(colorShader, d_debugPointDrawContext);
                GL.PointSize(1f);
            }
            if (d_showDirectionArrows)
            {
                Matrix4 transform = Matrix4.CreateScale(1, -1, -1);
                transform *= Matrix4.CreateTranslation(Vector3.Zero);
                transform *= Matrix4.CreateScale(Camera.Distance / 4f).Inverted();
                transform.Invert();
                colorShader.SetUniformMat4("Transform", ref transform);
                colorShader.SetUniform1("Intensity", 0.75f);
                colorShader.SetUniform4("BlendColor", Color.White);

                Renderer.SetLineWidth(2f);

                Renderer.Draw(colorShader, d_debugLineDrawContext);

                Renderer.SetLineWidth(1f);

            }
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.Blend);
#endif
        }

        [Conditional("DEBUG")]
        private void InitializeDebugComponents()
        {
#if DEBUG
            debugContextMenuStrip1 = new ContextMenuStrip();
            debugContextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            debugContextMenuStrip1.Items.AddRange(new ToolStripItem[] {});
            debugContextMenuStrip1.Name = "contextMenuStrip1";
            debugContextMenuStrip1.Size = new Size(159, 48);
            // 
            // debugLabel
            // 
            d_debugLabel = new Label();
            d_debugLabel.AutoSize = true;
            d_debugLabel.Visible = false;
            d_debugLabel.BackColor = Color.Transparent;
            d_debugLabel.ForeColor = SystemColors.ControlLight;
            d_debugLabel.Location = new Point(3, 4);
            d_debugLabel.Name = "debugLabel";
            d_debugLabel.Size = new Size(37, 13);
            d_debugLabel.TabIndex = 2;
            d_debugLabel.Text = "debug";
            var debugCameraToolStripMenuItem = new ToolStripMenuItem("Show Camera debug information");
            debugCameraToolStripMenuItem.CheckOnClick = true;
            debugCameraToolStripMenuItem.Click += (s, e) => d_debugLabel.Visible = debugCameraToolStripMenuItem.Checked;
            debugContextMenuStrip1.Items.Add(debugCameraToolStripMenuItem);

            var debugShowFocalPointToolStripMenuItem = new ToolStripMenuItem("Show Camera Focal point");
            debugShowFocalPointToolStripMenuItem.CheckOnClick = true;
            debugShowFocalPointToolStripMenuItem.Click += (s, e) => d_showFocalPoint = debugShowFocalPointToolStripMenuItem.Checked;
            debugContextMenuStrip1.Items.Add(debugShowFocalPointToolStripMenuItem);

            var debugShowDirectionArrows = new ToolStripMenuItem("Show Direction Arrows");
            debugShowDirectionArrows.CheckOnClick = true;
            debugShowDirectionArrows.Click += (s, e) => d_showDirectionArrows = debugShowDirectionArrows.Checked;
            debugContextMenuStrip1.Items.Add(debugShowDirectionArrows);

            Controls.Add(d_debugLabel);

            this.debugContextMenuStrip1.ResumeLayout(false);
#endif
        }

#if DEBUG
        private bool d_showFocalPoint;
        private bool d_showDirectionArrows;
        private DrawContext d_debugPointDrawContext;
        private DrawContext d_debugLineDrawContext;
        private Label d_debugLabel;
        private ContextMenuStrip debugContextMenuStrip1;
#endif

    }
}  
