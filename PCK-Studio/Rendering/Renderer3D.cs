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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Classes.Utils;
using Microsoft.VisualBasic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using PckStudio.Properties;
using System.Runtime.InteropServices;
using PckStudio.Extensions;
using Newtonsoft.Json.Linq;

namespace PckStudio.Rendering
{
    public partial class Renderer3D : GLControl
    {
        private Bitmap _texture;
        /// <summary>
        /// The visible Texture on the renderer
        /// </summary>
        /// <returns>The visible Texture</returns>
        [Description("The current Texture")]
        [Category("Appearance")]
        public Bitmap Texture
        {
            get => _texture;
            set
            {
                if (shader is not null)
                {
                    var texture = new Texture2D(value);
                    texture.Bind(1);
                    shader.SetUniform1("u_Texture", 1);
                    Refresh();
                }
                TextureScaleValue = new Vector2(1f / value.Width, 1f / value.Height);
                _texture = value;
            }
        }

        public enum Models
        {
            Steve,
            Alex
        }
        /// <summary>
        /// The rendered model
        /// </summary>
        /// <returns>The rendered model</returns>
        [Description("The current player model")]
        [Category("Appearance")]
        public Models Model { get; set; }


        private Vector2 _rotation = new Vector2();
        /// <summary>
        /// Rotation
        /// </summary>
        /// <returns>Rotation</returns>
        [Description("The rotation of the camera")]
        [Category("Appearance")]
        public Vector2 Rotation
        {
            get => _rotation;
            set
            {
                value.X = MathHelper.Clamp(value.X, -90f, 90f);
                value.Y = MathHelper.Clamp(value.Y, -180f, 180f);
                _rotation = value;
            }
        }

        private double _fieldOfViewRadians = MinFOV;

        private const double MinFOVDegrees = 30d;
        private const double MaxFOVDegrees = 90d;

        private static double MinFOV = MathHelper.DegreesToRadians(MinFOVDegrees);
        private static double MaxFOV = MathHelper.DegreesToRadians(MaxFOVDegrees);

        [Description("The Field of View value (in Radians)")]
        private double FieldOfViewRadians
        {
            get => _fieldOfViewRadians;
            set => _fieldOfViewRadians = MathHelper.Clamp(value, MinFOV, MaxFOV);
        }

        [Description("The Field of View (in Degrees)")]
        [Category("Appearance")]
        public double FieldOfView
        {
            get => MathHelper.RadiansToDegrees(_fieldOfViewRadians);
            set => FieldOfViewRadians = MathHelper.DegreesToRadians(MathHelper.Clamp(value, MinFOVDegrees, MaxFOVDegrees));
        }

        private Vector2 _lookAngle = Vector2.Zero;
        [Description("The offset from the orignal point (for zoom)")]
        [Category("Appearance")]
        public Vector2 LookAngle
        {
            get => _lookAngle;
            set
            {
                value.X = MathHelper.Clamp(value.X, -8f, 8f);
                value.Y = MathHelper.Clamp(value.Y, -16f, 16f);
                _lookAngle = value;
            }
        }

        public float CameraDistance { get; set; } = 18f;

        private Matrix4 projection;

        private Matrix4 view;

        private Vector2 TextureScaleValue = new Vector2(1f / 64);

        private void GLDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
            Debug.WriteLine(source);
            Debug.WriteLine(type);
            Debug.WriteLine(severity);
            Debug.WriteLine(id);
            Debug.WriteLine(msg);
        }

        private Shader shader;

        private VertexArray vertexArray;
        private IndexBuffer indexBuffer;
        private Matrix4 mvp;

        private bool IsMouseDown;
        private bool IsRightMouseDown;
        private bool IsMouseHidden;
        private Point PreviousMouseLocation;
        private Point MouseLoc;

        private Ray LastRay;
        private Vector3 GlobalCameraPos;

        public Renderer3D()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;

            MakeCurrent();

            GL.DebugMessageCallback(GLDebugMessage, IntPtr.Zero);

            Trace.TraceInformation(GL.GetString(StringName.Version));

            shader = Shader.Create(Resources.vertexShader, Resources.fragmentShader);
            shader.Bind();

            Texture = Resources.slim_template;
            var bodyVertexData = new Vertex[]
            {
                // Face 1
                new Vertex(new Vector3(-4,  8,  2), Color.Red, new Vector2(TextureScaleValue.X * 20f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3( 4,  8,  2), Color.Red, new Vector2(TextureScaleValue.X * 28f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3( 4, -4,  2), Color.Red, new Vector2(TextureScaleValue.X * 28f, TextureScaleValue.Y * 32f)),
                new Vertex(new Vector3(-4, -4,  2), Color.Red, new Vector2(TextureScaleValue.X * 20f, TextureScaleValue.Y * 32f)),
                
                // Face 2
                new Vertex(new Vector3(-4,  8, -2), Color.Red, new Vector2(TextureScaleValue.X * 40f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3( 4,  8, -2), Color.Red, new Vector2(TextureScaleValue.X * 32f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3( 4, -4, -2), Color.Red, new Vector2(TextureScaleValue.X * 32f, TextureScaleValue.Y * 32f)),
                new Vertex(new Vector3(-4, -4, -2), Color.Red, new Vector2(TextureScaleValue.X * 40f, TextureScaleValue.Y * 32f)),
                
                // Face 3
                new Vertex(new Vector3(-4,  8,  2), Color.Red, new Vector2(TextureScaleValue.X * 20f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3(-4,  8, -2), Color.Red, new Vector2(TextureScaleValue.X * 16f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3(-4, -4, -2), Color.Red, new Vector2(TextureScaleValue.X * 16f, TextureScaleValue.Y * 32f)),
                new Vertex(new Vector3(-4, -4,  2), Color.Red, new Vector2(TextureScaleValue.X * 20f, TextureScaleValue.Y * 32f)),
                
                // Face 4
                new Vertex(new Vector3( 4,  8,  2), Color.Red, new Vector2(TextureScaleValue.X * 28f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3( 4,  8, -2), Color.Red, new Vector2(TextureScaleValue.X * 32f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3( 4, -4, -2), Color.Red, new Vector2(TextureScaleValue.X * 32f, TextureScaleValue.Y * 32f)),
                new Vertex(new Vector3( 4, -4,  2), Color.Red, new Vector2(TextureScaleValue.X * 28f, TextureScaleValue.Y * 32f)),
                
                // Face 5
                new Vertex(new Vector3(-4,  8,  2), Color.Red, new Vector2(TextureScaleValue.X * 20f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3( 4,  8,  2), Color.Red, new Vector2(TextureScaleValue.X * 28f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3( 4,  8, -2), Color.Red, new Vector2(TextureScaleValue.X * 28f, TextureScaleValue.Y * 16f)),
                new Vertex(new Vector3(-4,  8, -2), Color.Red, new Vector2(TextureScaleValue.X * 20f, TextureScaleValue.Y * 16f)),
                
                // Face 6
                new Vertex(new Vector3(-4, -4, -2), Color.Red, new Vector2(TextureScaleValue.X * 28f, TextureScaleValue.Y * 16f)),
                new Vertex(new Vector3( 4, -4, -2), Color.Red, new Vector2(TextureScaleValue.X * 36f, TextureScaleValue.Y * 16f)),
                new Vertex(new Vector3( 4, -4,  2), Color.Red, new Vector2(TextureScaleValue.X * 36f, TextureScaleValue.Y * 20f)),
                new Vertex(new Vector3(-4, -4,  2), Color.Red, new Vector2(TextureScaleValue.X * 28f, TextureScaleValue.Y * 20f)),
            };

            indexBuffer = new IndexBuffer(
                 0,  1,  2,  3,
                 4,  5,  6,  7,
                 8,  9, 10, 11,
                12, 13, 14, 15,
                16, 17, 18, 19,
                20, 21, 22, 23
                );

            vertexArray = new VertexArray();

            int vertexSize = Marshal.SizeOf(typeof(Vertex));

            var buffer = new VertexBuffer<Vertex>(bodyVertexData, bodyVertexData.Length * vertexSize);
            var layout = new VertexBufferLayout();
            layout.Add<float>(3);
            layout.Add<float>(4);
            layout.Add<float>(2);

            vertexArray.AddBuffer(buffer, layout);

            GLErrorCheck();
        }

        [Conditional("DEBUG")]
        private void GLErrorCheck()
        {
            var error = GL.GetError();
            Debug.Assert(error == ErrorCode.NoError, error.ToString());
        }

        private void UpdateView()
        {
            var camera = new Vector3(LookAngle) { Z = CameraDistance };
            
            var target = new Vector3(LookAngle);

            var up = new Vector3(0, 1f, 1f);

            view = Matrix4.LookAt(camera, target, up);
        }

        private void UpdateProjection()
        {
            projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.Pow(FieldOfViewRadians, -1), Width / (float)Height, 1f, 100f);
        }

        private void DrawBox(Vector3 scale, Vector3 position, Vector2 uv)
        {
            float[] Corner1 = { position.X,          position.Y,          position.Z };
            float[] Corner2 = { position.X + scale.X, position.Y,          position.Z };
            float[] Corner3 = { position.X,          position.Y + scale.Y, position.Z };
            float[] Corner4 = { position.X,          position.Y,          position.Z + scale.Z };
            float[] Corner5 = { position.X + scale.X, position.Y + scale.Y, position.Z };
            float[] Corner6 = { position.X,          position.Y + scale.Y, position.Z + scale.Z };
            float[] Corner7 = { position.X + scale.X, position.Y,          position.Z + scale.Z };
            float[] Corner8 = { position.X + scale.X, position.Y + scale.Y, position.Z + scale.Z };

            GL.Color3(Color.Red);
            // Face 1
            GL.Vertex3(Corner1);
            GL.Vertex3(Corner3);
            GL.Vertex3(Corner5);
            GL.Vertex3(Corner2);
            // Face 2
            GL.Vertex3(Corner1);
            GL.Vertex3(Corner4);
            GL.Vertex3(Corner7);
            GL.Vertex3(Corner2);
            // Face 3
            GL.Vertex3(Corner1);
            GL.Vertex3(Corner4);
            GL.Vertex3(Corner6);
            GL.Vertex3(Corner3);
            // Face 4
            GL.TexCoord2(uv.X, TextureScaleValue.Y * 8d);
            GL.Vertex3(Corner4);
            GL.TexCoord2(TextureScaleValue.X * 16d, TextureScaleValue.Y * 8d);
            GL.Vertex3(Corner7);
            GL.TexCoord2(TextureScaleValue.X * 16d, TextureScaleValue.Y * 16d);
            GL.Vertex3(Corner8);
            GL.TexCoord2(TextureScaleValue.X * 8d, TextureScaleValue.Y * 16d);
            GL.Vertex3(Corner6);
            // Face 5
            GL.Vertex3(Corner8);
            GL.Vertex3(Corner6);
            GL.Vertex3(Corner3);
            GL.Vertex3(Corner5);
            // Face 6
            GL.Vertex3(Corner2);
            GL.Vertex3(Corner7);
            GL.Vertex3(Corner8);
            GL.Vertex3(Corner5);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                base.OnPaint(e);
                return;
            }

            UpdateProjection();
            UpdateView();

            MakeCurrent();
            GL.Viewport(Size);

            Matrix4 transform = Matrix4.Identity;
            Matrix4 scale = Matrix4.Identity;
            Matrix4 rot = Matrix4.CreateFromAxisAngle(new Vector3(-1f, 0f, 0f), MathHelper.DegreesToRadians(Rotation.X))
                    + Matrix4.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), MathHelper.DegreesToRadians(Rotation.Y));

            var model = transform * rot * scale;

            mvp = model * view * projection;

#if DEBUG
            debugLabel.Text = $"Rotation: {Rotation}\nFOV: {FieldOfView}\nView:\n{view}\nProjection:\n{projection}\nMVP:\n{mvp}";
#endif

            shader.SetUniformMat4("u_MVP", ref mvp);

            GL.Enable(EnableCap.Texture2D); // Enable textures
            //GL.Enable(EnableCap.DepthTest); // Enable correct Z Drawings
            //GL.DepthFunc(DepthFunction.Less); // Enable correct Z Drawings
            GL.Disable(EnableCap.Blend); // Disable transparent
            GL.Disable(EnableCap.AlphaTest); // Disable transparent

            GL.ClearColor(BackColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //GL.Enable(EnableCap.AlphaTest); // Enable transparent
            //GL.AlphaFunc(AlphaFunction.Greater, 0.4f);

            Renderer.Draw(shader, vertexArray, indexBuffer, PrimitiveType.Quads);

            SwapBuffers();
            return;
            
            GL.Disable(EnableCap.AlphaTest); // Disable transparent
        }

#if DEBUG
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    debugLabel.Visible = !debugLabel.Visible;
                    return true;
                case Keys.R:
                    Rotation = Vector2.Zero;
                    Refresh();
                    return true;
                case Keys.F1:
                    var fileDialog = new OpenFileDialog()
                    {
                        Filter = "texture|*.png",
                    };
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Texture = Image.FromFile(fileDialog.FileName) as Bitmap;
                    }
                    return true;
            }
            return base.ProcessDialogKey(keyData);
        }
#endif

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!IsMouseDown && e.Button == MouseButtons.Left)
            {
                // If the ray didn't hit the model then rotate the model
                PreviousMouseLocation = Cursor.Position; // Store the old mouse position to reset it when the action is over
                if (!IsMouseHidden) // Hide the mouse
                {
                    Cursor.Hide();
                    IsMouseHidden = true;
                }
                MouseLoc = Cursor.Position; // Store the current mouse position to use it for the rotate action
                IsMouseDown = true;
            }
            else if (!IsRightMouseDown && e.Button == MouseButtons.Right)
            {
                PreviousMouseLocation = Cursor.Position; // Store the old mouse position to reset it when the action is over 
                if (!IsMouseHidden) // Hide the mouse
                {
                    Cursor.Hide();
                    IsMouseHidden = true;
                }
                MouseLoc = Cursor.Position; // Store the current mouse position to use it for the move action
                IsRightMouseDown = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (IsMouseHidden)
            {
                Cursor.Position = PreviousMouseLocation;
                IsMouseDown = IsMouseHidden = IsRightMouseDown = false;
                Cursor.Show();
            }
        }

        private void Move_Tick(object sender, EventArgs e)
        {
            // Rotate the model
            if (IsMouseDown)
            {
                float rotationYDelta = (float)Math.Round((Cursor.Position.X - MouseLoc.X) * 0.5f);
                float rotationXDelta = (float)Math.Round(-(Cursor.Position.Y - MouseLoc.Y) * 0.5f);
                Rotation += new Vector2(rotationXDelta, rotationYDelta);
                Refresh();
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                MouseLoc = Cursor.Position;
                return;
            }
            // Move the model
            if (IsRightMouseDown)
            {
                float deltaX = -(Cursor.Position.X - MouseLoc.X) * 0.05f / (float)FieldOfViewRadians;
                float deltaY = (Cursor.Position.Y - MouseLoc.Y) * 0.05f / (float)FieldOfViewRadians;
                LookAngle += new Vector2(deltaX, deltaY);
                Refresh();
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                MouseLoc = Cursor.Position;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            FieldOfViewRadians += e.Delta * 0.005d;
            Refresh();
            base.OnMouseWheel(e);
        }

        private Vector3 GetCameraPosition(Matrix4 view)
        {
            return Matrix4.Invert(view).ExtractTranslation();
        }

        private List<Point> MousePoints = new List<Point>();
        private List<Point> tmpMousePoints = new List<Point>();

        public event TextureChangedEventHandler TextureChanged;

        public delegate void TextureChangedEventHandler(object sender, bool IsLeft);
    }
}

