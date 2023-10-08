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
                TextureScaleValue = new Vector2d(1d / value.Width, 1d / value.Height);
                _texture = value;
            }
        }

        private bool _showhead = true;
        /// <summary>
        /// Show the head or not
        /// </summary>
        /// <returns>Show the head or not</returns>
        [Description("Shows the head layer or not")]
        [Category("Appearance")]
        public bool ShowHead
        {
            set
            {
                Refresh();
                _showhead = value;
            }
            get
            {
                return _showhead;
            }
        }
        private bool _showHeadOverlay = true;
        /// <summary>
        /// Show the second head layer or not
        /// </summary>
        /// <returns>Show the second head layer or not</returns>
        [Description("Shows the second head layer or not")]
        [Category("Appearance")]
        public bool ShowHeadOverlay
        {
            set
            {
                Refresh();
                _showHeadOverlay = value;
            }
            get
            {
                return _showHeadOverlay;
            }
        }
        private bool _showbody = true;
        /// <summary>
        /// Show the body or not
        /// </summary>
        /// <returns>Show the body or not</returns>
        [Description("Shows the body layer or not")]
        [Category("Appearance")]
        public bool ShowBody
        {
            set
            {
                Refresh();
                _showbody = value;
            }
            get
            {
                return _showbody;
            }
        }
        private bool _showBodyOverlay = true;
        /// <summary>
        /// Show the second body layer or not
        /// </summary>
        /// <returns>Show the second body layer or not</returns>
        [Description("Shows the second body layer or not")]
        [Category("Appearance")]
        public bool ShowBodyOverlay
        {
            set
            {
                Refresh();
                _showBodyOverlay = value;
            }
            get
            {
                return _showBodyOverlay;
            }
        }
        private bool _showRightArm = true;
        /// <summary>
        /// Show the right arm or not
        /// </summary>
        /// <returns>Show the right arm or not</returns>
        [Description("Shows the right arm layer or not")]
        [Category("Appearance")]
        public bool ShowRightArm
        {
            set
            {
                Refresh();
                _showRightArm = value;
            }
            get
            {
                return _showRightArm;
            }
        }
        private bool _showRightArmOverlay = true;
        /// <summary>
        /// Show the second right arm layer or not
        /// </summary>
        /// <returns>Show the second right arm layer or not</returns>
        [Description("Shows the second right arm layer or not")]
        [Category("Appearance")]
        public bool ShowRightArmOverlay
        {
            set
            {
                Refresh();
                _showRightArmOverlay = value;
            }
            get
            {
                return _showRightArmOverlay;
            }
        }
        private bool _showLeftArm = true;
        /// <summary>
        /// Show the left arm or not
        /// </summary>
        /// <returns>Show the leftht arm or not</returns>
        [Description("Shows the left arm layer or not")]
        [Category("Appearance")]
        public bool ShowLeftArm
        {
            set
            {
                Refresh();
                _showLeftArm = value;
            }
            get
            {
                return _showLeftArm;
            }
        }
        private bool _showLeftArmOverlay = true;
        /// <summary>
        /// Show the second left arm layer or not
        /// </summary>
        /// <returns>Show the second left arm layer or not</returns>
        [Description("Shows the second left arm layer or not")]
        [Category("Appearance")]
        public bool ShowLeftArmOverlay
        {
            set
            {
                Refresh();
                _showLeftArmOverlay = value;
            }
            get
            {
                return _showLeftArmOverlay;
            }
        }
        private bool _showRightLeg = true;
        /// <summary>
        /// Show the right leg or not
        /// </summary>
        /// <returns>Show the right leg or not</returns>
        [Description("Shows the right leg layer or not")]
        [Category("Appearance")]
        public bool ShowRightLeg
        {
            set
            {
                Refresh();
                _showRightLeg = value;
            }
            get
            {
                return _showRightLeg;
            }
        }
        private bool _showRightLegOverlay = true;
        /// <summary>
        /// Show the second right leg layer or not
        /// </summary>
        /// <returns>Show the second right leg layer or not</returns>
        [Description("Shows the second right leg layer or not")]
        [Category("Appearance")]
        public bool ShowRightLegOverlay
        {
            set
            {
                Refresh();
                _showRightLegOverlay = value;
            }
            get
            {
                return _showRightLegOverlay;
            }
        }
        private bool _showLeftLeg = true;
        /// <summary>
        /// Show the left leg or not
        /// </summary>
        /// <returns>Show the leftht leg or not</returns>
        [Description("Shows the left leg layer or not")]
        [Category("Appearance")]
        public bool ShowLeftLeg
        {
            set
            {
                Refresh();
                _showLeftLeg = value;
            }
            get
            {
                return _showLeftLeg;
            }
        }
        private bool _showLeftLegOverlay = true;
        /// <summary>
        /// Show the second left leg layer or not
        /// </summary>
        /// <returns>Show the second left leg layer or not</returns>
        [Description("Shows the second left leg layer or not")]
        [Category("Appearance")]
        public bool ShowLeftLegOverlay
        {
            set
            {
                Refresh();
                _showLeftLegOverlay = value;
            }
            get
            {
                return _showLeftLegOverlay;
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
        [Description("The rotation of the model")]
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

        private double _fieldOfView = MinFOV;
        private const double MinFOV = 30d;
        private const double MaxFOV = 90d;

        [Description("The zoom value")]
        [Category("Appearance")]
        public double FieldOfView
        {
            get => _fieldOfView;
            set => _fieldOfView = MathHelper.Clamp(value, MinFOV, MaxFOV);
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

        public float CameraDistance { get; set; } = 36f;

        private Matrix4 projection;

        private Matrix4 view;

        private Vector2d TextureScaleValue = new Vector2d(1d / 64d);

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

            shader.SetUniform4("u_Color", Color.Blue);

            var data = new float[]
            {
                 0.0f,  0.0f, 0.0f, 0.0f, 1.0f,
                10.0f,  0.0f, 0.0f, 1.0f, 1.0f,
                10.0f, 10.0f, 10.0f, 1.0f, 0.0f,
                 0.0f, 10.0f, 0.0f, 0.0f, 0.0f,
            };

            vertexArray = new VertexArray();

            var buffer = new VertexBuffer<float>(data, data.Length * sizeof(float));
            var layout = new VertexBufferLayout();
            layout.Add<float>(3);
            layout.Add<float>(2);

            vertexArray.AddBuffer(buffer, layout);

            indexBuffer = new IndexBuffer(
                0, 1, 2,
                2, 3, 0
                );


            var error = GL.GetError();
            while (error != ErrorCode.NoError)
            {
                Debug.WriteLine(error);
                error = GL.GetError();
            }
        }

        private void UpdateView()
        {
            var camera = new Vector3(LookAngle) { Z = CameraDistance };
            
            var target = new Vector3(LookAngle);

            var up = Vector3.UnitY + Vector3.UnitZ;

            view = Matrix4.LookAt(camera, target, up);
        }

        private void UpdateProjection()
        {
            projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.Pow(FieldOfView, -1), Width / (float)Height, 0.1f, 1000f);
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

            var rot = Matrix4.CreateFromAxisAngle(new Vector3(-0.5f, 0f, 0f), Rotation.X)
                    + Matrix4.CreateFromAxisAngle(new Vector3(0f, 0.5f, 0f), Rotation.Y);

            mvp = view * projection * rot;

#if DEBUG
            debugLabel.Text = $"Rotation: {Rotation}\nZoom: {_fieldOfView}\nLookAt:\n{view}\nProjection:\n{projection}\nMVP:\n{mvp}";
#endif

            shader.SetUniformMat4("u_MVP", ref mvp);

            GL.ClearColor(BackColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Renderer.Draw(vertexArray, indexBuffer, shader);

            SwapBuffers();
            return;

            GL.PushMatrix();

            GL.ClearColor(BackColor);
            // First Clear Buffers
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            
            // Basic Setup for viewing
            UpdateProjection();
            UpdateView();
            GL.MatrixMode(MatrixMode.Projection); // Load Perspective
            GL.LoadIdentity();
            GL.LoadMatrix(ref projection);
            GL.MatrixMode(MatrixMode.Modelview); // Load Camera
            GL.LoadIdentity();
            GL.LoadMatrix(ref view);
            GL.Viewport(0, 0, Width, Height); // Size of window
            GL.Enable(EnableCap.DepthTest); // Enable correct Z Drawings
            GL.Enable(EnableCap.Texture2D); // Enable textures
            GL.DepthFunc(DepthFunction.Lequal); // Enable correct Z Drawings
                                              // GL.Disable(EnableCap.Blend) 'Disable transparent
            GL.Disable(EnableCap.AlphaTest); // Disable transparent
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
#if DEBUG
            switch (keyData)
            {
                case Keys.Escape:
                    debugLabel.Visible = !debugLabel.Visible;
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
#endif
            return base.ProcessDialogKey(keyData);
        }

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
                float deltaX = -(Cursor.Position.X - MouseLoc.X) * 0.5f / (float)FieldOfView;
                float deltaY = (Cursor.Position.Y - MouseLoc.Y) * 0.5f / (float)FieldOfView;
                LookAngle += new Vector2(deltaX, deltaY);
                Refresh();
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                MouseLoc = Cursor.Position;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            FieldOfView += e.Delta * 0.005d;
            Refresh();
            base.OnMouseWheel(e);
        }

        private Vector3 GetCameraPosition(Matrix4 modelview)
        {
            return Matrix4.Invert(modelview).ExtractTranslation();
        }

        private List<Point> MousePoints = new List<Point>();
        private List<Point> tmpMousePoints = new List<Point>();

        public event TextureChangedEventHandler TextureChanged;

        public delegate void TextureChangedEventHandler(object sender, bool IsLeft);

        public Point Get2DFrom3D(Vector3 Vector, ref Vector3 XUp, ref Vector3 YUp)
        {
            var Result = default(Point);
            if (Vector.X < 4f && Vector.X > -4 && Vector.Y < 16f && Vector.Y > 8f && Vector.Z == 4f)
            {
                // ZHead
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 4f + 8f)), (int)Math.Round(Conversion.Int(-Vector.Y + 16f + 8f)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < 4f && Vector.X > -4 && Vector.Y < 16f && Vector.Y > 8f && Vector.Z == -4)
            {
                // ZHead
                Result = new Point((int)Math.Round(Conversion.Int(-Vector.X + 4f + 24f)), (int)Math.Round(Conversion.Int(-Vector.Y + 16f + 8f)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < 4f && Vector.X > -4 && Vector.Y < 8f && Vector.Y > -4 && Vector.Z == 2f)
            {
                // ZBody
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 4f + 20f)), (int)Math.Round(Conversion.Int(-Vector.Y + 8f + 20f)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < 4f && Vector.X > -4 && Vector.Y < 8f && Vector.Y > -4 && Vector.Z == -2)
            {
                // ZBody
                Result = new Point((int)Math.Round(Conversion.Int(-Vector.X + 4f + 32f)), (int)Math.Round(Conversion.Int(-Vector.Y + 8f + 20f)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < -4 && Vector.X > -8 && Vector.Y < 8f && Vector.Y > -4 && Vector.Z == 2f)
            {
                // ZArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 4f + 48f)), (int)Math.Round(Conversion.Int(-Vector.Y + 8f + 20f)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 4f + 47f)), (int)Math.Round(Conversion.Int(-Vector.Y + 8f + 20f)));
                }
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < -4 && Vector.X > -8 && Vector.Y < 8f && Vector.Y > -4 && Vector.Z == -2)
            {
                // ZArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(-Vector.X + 4f + 44f)), (int)Math.Round(Conversion.Int(-Vector.Y + 8f + 20f)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(-Vector.X + 4f + 43f)), (int)Math.Round(Conversion.Int(-Vector.Y + 8f + 20f)));
                }
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < 8f && Vector.X > 4f && Vector.Y < 8f && Vector.Y > -4 && Vector.Z == 2f)
            {
                // ZArms
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 4f + 28f)), (int)Math.Round(Conversion.Int(-Vector.Y + 8f + 52f)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < 8f && Vector.X > 4f && Vector.Y < 8f && Vector.Y > -4 && Vector.Z == -2)
            {
                // ZArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(-Vector.X + 4f + 48f)), (int)Math.Round(Conversion.Int(-Vector.Y + 8f + 52f)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(-Vector.X + 4f + 46f)), (int)Math.Round(Conversion.Int(-Vector.Y + 8f + 52f)));
                }
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < 0f && Vector.X > -4 && Vector.Y < -4 && Vector.Y > -16 && Vector.Z == 2f)
            {
                // ZLegs
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 4f + 4f)), (int)Math.Round(Conversion.Int(-Vector.Y + 16f)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < 0f && Vector.X > -4 && Vector.Y < -4 && Vector.Y > -16 && Vector.Z == -2)
            {
                // ZLegs
                Result = new Point((int)Math.Round(Conversion.Int(-Vector.X + 4f + 8f)), (int)Math.Round(Conversion.Int(-Vector.Y + 16f)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < 4f && Vector.X > 0f && Vector.Y < -4 && Vector.Y > -16 && Vector.Z == 2f)
            {
                // ZLegs
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 4f + 16f)), (int)Math.Round(Conversion.Int(-Vector.Y + 48f)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.X < 4f && Vector.X > 0f && Vector.Y < -4 && Vector.Y > -16 && Vector.Z == -2)
            {
                // ZLegs
                Result = new Point((int)Math.Round(Conversion.Int(-Vector.X + 4f + 28f)), (int)Math.Round(Conversion.Int(-Vector.Y + 48f)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 4f && Vector.Z > -4 && Vector.Y < 16f && Vector.Y > 8f && Vector.X == 4f)
            {
                // XHead
                Result = new Point((int)Math.Round(Conversion.Int(-Vector.Z + 20f)), (int)Math.Round(Conversion.Int(-Vector.Y + 24f)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 4f && Vector.Z > -4 && Vector.Y < 16f && Vector.Y > 8f && Vector.X == -4)
            {
                // XHead
                Result = new Point((int)Math.Round(Conversion.Int(Vector.Z + 4f)), (int)Math.Round(Conversion.Int(-Vector.Y + 24f)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.Y < 8f && Vector.Y > -4 && Vector.X == 4f)
            {
                // XBody
                if (ShowBody && !ShowLeftArm)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(-Vector.Z + 30f)), (int)Math.Round(Conversion.Int(-Vector.Y + 28f)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.Z + 34f)), (int)Math.Round(Conversion.Int(-Vector.Y + 60f)));
                }
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.Y < 8f && Vector.Y > -4 && Vector.X == -4)
            {
                // XBody
                if (ShowBody && !ShowRightArm)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.Z + 18f)), (int)Math.Round(Conversion.Int(-Vector.Y + 28f)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(-Vector.Z + 50f)), (int)Math.Round(Conversion.Int(-Vector.Y + 28f)));
                }
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.Y < 8f && Vector.Y > -4 && Vector.X == 8f)
            {
                // XArms
                Result = new Point((int)Math.Round(Conversion.Int(-Vector.Z + 42f)), (int)Math.Round(Conversion.Int(-Vector.Y + 60f)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.Y < 8f && Vector.Y > -4 && Vector.X == -8)
            {
                // XArms
                Result = new Point((int)Math.Round(Conversion.Int(Vector.Z + 42f)), (int)Math.Round(Conversion.Int(-Vector.Y + 28f)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.Y < 8f && Vector.Y > -4 && Vector.X == 7f)
            {
                // XArms
                Result = new Point((int)Math.Round(Conversion.Int(-Vector.Z + 41f)), (int)Math.Round(Conversion.Int(-Vector.Y + 60f)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.Y < 8f && Vector.Y > -4 && Vector.X == -7)
            {
                // XArms
                Result = new Point((int)Math.Round(Conversion.Int(Vector.Z + 42f)), (int)Math.Round(Conversion.Int(-Vector.Y + 28f)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.Y < -4 && Vector.Y > -16 && Vector.X == 4f)
            {
                // XLeg
                Result = new Point((int)Math.Round(Conversion.Int(-Vector.Z + 26f)), (int)Math.Round(Conversion.Int(-Vector.Y + 48f)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.Y < -4 && Vector.Y > -16 && Vector.X == -4)
            {
                // XLeg
                Result = new Point((int)Math.Round(Conversion.Int(Vector.Z + 2f)), (int)Math.Round(Conversion.Int(-Vector.Y + 16f)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.Y < -4 && Vector.Y > -16 && Vector.X == 0f)
            {
                // XLeg
                if (ShowLeftLeg)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.Z + 18f)), (int)Math.Round(Conversion.Int(-Vector.Y + 48f)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(-Vector.Z + 10f)), (int)Math.Round(Conversion.Int(-Vector.Y + 16f)));
                }
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (Vector.Z < 4f && Vector.Z > -4 && Vector.X < 4f && Vector.X > -4 && Vector.Y == 16f)
            {
                // YHead
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 12f)), (int)Math.Round(Conversion.Int(Vector.Z + 4f)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowHead && Vector.Z < 4f && Vector.Z > -4 && Vector.X < 4f && Vector.X > -4 && Vector.Y == 8f)
            {
                // YHead
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 20f)), (int)Math.Round(Conversion.Int(Vector.Z + 4f)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.X < 4f && Vector.X > -4 && Vector.Y == 8f)
            {
                // YBody
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 24f)), (int)Math.Round(Conversion.Int(Vector.Z + 18f)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowBody && (!ShowLeftLeg || !ShowRightLeg) && Vector.Z < 2f && Vector.Z > -2 && Vector.X < 4f && Vector.X > -4 && Vector.Y == -4)
            {
                // YBody
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 32f)), (int)Math.Round(Conversion.Int(Vector.Z + 18f)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.X < 8f && Vector.X > 4f && Vector.Y == 8f)
            {
                // YArms
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 32f)), (int)Math.Round(Conversion.Int(Vector.Z + 50f)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.X < 8f && Vector.X > 4f && Vector.Y == -4)
            {
                // YArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 36f)), (int)Math.Round(Conversion.Int(Vector.Z + 50f)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 35f)), (int)Math.Round(Conversion.Int(Vector.Z + 50f)));
                }
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.X < -4 && Vector.X > -8 && Vector.Y == 8f)
            {
                // YArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 52f)), (int)Math.Round(Conversion.Int(Vector.Z + 18f)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 51f)), (int)Math.Round(Conversion.Int(Vector.Z + 18f)));
                }
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.X < -4 && Vector.X > -8 && Vector.Y == -4)
            {
                // YArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 56f)), (int)Math.Round(Conversion.Int(Vector.Z + 18f)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 54f)), (int)Math.Round(Conversion.Int(Vector.Z + 18f)));
                }
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowLeftLeg && !ShowBody && Vector.Z < 2f && Vector.Z > -2 && Vector.X < 4f && Vector.X > 0f && Vector.Y == -4)
            {
                // YLeg
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 20f)), (int)Math.Round(Conversion.Int(Vector.Z + 50f)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowRightLeg && !ShowBody && Vector.Z < 2f && Vector.Z > -2 && Vector.X < 0f && Vector.X > -4 && Vector.Y == -4)
            {
                // YLeg
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 8f)), (int)Math.Round(Conversion.Int(Vector.Z + 18f)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.X < 4f && Vector.X > 0f && Vector.Y == -16)
            {
                // YLeg
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 24f)), (int)Math.Round(Conversion.Int(Vector.Z + 50f)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (Vector.Z < 2f && Vector.Z > -2 && Vector.X < 0f && Vector.X > -4 && Vector.Y == -16)
            {
                // YLeg
                Result = new Point((int)Math.Round(Conversion.Int(Vector.X + 12f)), (int)Math.Round(Conversion.Int(Vector.Z + 18f)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }

            return Result;
        }

        public Point Get2nd2DFrom3D(Vector3 Vector, ref Vector3 XUp, ref Vector3 YUp)
        {
            var Result = default(Point);
            if (ShowHeadOverlay && Vector.X < 4.24d && Vector.X > -4.24d && Vector.Y < 16.24d && Vector.Y > 7.76d && Vector.Z == 4.24f)
            {
                // ZHead
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 4.24d) / 1.06d + 40d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 16.24d) / 1.06d + 8d)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowHeadOverlay && Vector.X < 4.24d && Vector.X > -4.24d && Vector.Y < 16.24d && Vector.Y > 7.76d && Vector.Z == -4.24f)
            {
                // ZHead
                Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.X + 4.24d) / 1.06d + 56d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 16.24d) / 1.06d + 8d)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowBodyOverlay && Vector.X < 4.24d && Vector.X > -4.24d && Vector.Y < 8.36d && Vector.Y > -4.36d && Vector.Z == 2.12f)
            {
                // ZBody
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 4.24d) / 1.06d + 20d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowBodyOverlay && Vector.X < 4.24d && Vector.X > -4.24d && Vector.Y < 8.36d && Vector.Y > -4.36d && Vector.Z == -2.12f)
            {
                // ZBody
                Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.X + 4.24d) / 1.06d + 32d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowRightArmOverlay && Vector.X < -3.88d && Vector.X > -8.12d && Vector.Y < 8.36d && Vector.Y > -4.36d && Vector.Z == 2.12f)
            {
                // ZArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 3.88d) / 1.06d + 48d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 3.88d) / 1.06d + 47d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                }
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowRightArmOverlay && Vector.X < -3.88d && Vector.X > -8.12d && Vector.Y < 8.36d && Vector.Y > -4.36d && Vector.Z == -2.12f)
            {
                // ZArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.X + 3.88d) / 1.06d + 45d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.X + 3.88d) / 1.06d + 44d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                }
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowLeftArmOverlay && Vector.X < 8.12d && Vector.X > 3.88d && Vector.Y < 8.36d && Vector.Y > -4.36d && Vector.Z == 2.12f)
            {
                // ZArms
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 3.88d) / 1.06d + 45d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 52d)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowLeftArmOverlay && Vector.X < 8.12d && Vector.X > 3.88d && Vector.Y < 8.36d && Vector.Y > -4.36d && Vector.Z == -2.12f)
            {
                // ZArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.X + 3.88d) / 1.06d + 64d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 52d)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.X + 3.88d) / 1.06d + 62d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 52d)));
                }
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowRightLegOverlay && Vector.X < 0.12d && Vector.X > -4.12d && Vector.Y < -3.64d && Vector.Y > -16.36d && Vector.Z == 2.12f)
            {
                // ZLegs
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 4.12d) / 1.06d + 4d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y - 16.36d) / 1.06d + 48d)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowRightLegOverlay && Vector.X < 0.12d && Vector.X > -4.12d && Vector.Y < -3.64d && Vector.Y > -16.36d && Vector.Z == -2.12f)
            {
                // ZLegs
                Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.X + 4.12d) / 1.06d + 8d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y - 16.36d) / 1.06d + 48d)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowLeftLegOverlay && Vector.X < 4.12d && Vector.X > -0.12d && Vector.Y < -3.64d && Vector.Y > -16.36d && Vector.Z == 2.12f)
            {
                // ZLegs
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 0.12d) / 1.06d + 4d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y - 3.64d) / 1.06d + 52d)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowLeftLegOverlay && Vector.X < 4.12d && Vector.X > -0.12d && Vector.Y < -3.64d && Vector.Y > -16.36d && Vector.Z == -2.12f)
            {
                // ZLegs
                Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.X + 0.12d) / 1.06d + 16d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y - 3.64d) / 1.06d + 52d)));
                XUp.X = 1f;
                YUp.Y = 1f;
            }
            else if (ShowHeadOverlay && Vector.Z < 4.24d && Vector.Z > -4.24d && Vector.Y < 16.24d && Vector.Y > 7.76d && Vector.X == 4.24f)
            {
                // XHead
                Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.Z + 4.24d) / 1.06d + 48d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 16.24d) / 1.06d + 8d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowHeadOverlay && Vector.Z < 4.24d && Vector.Z > -4.24d && Vector.Y < 16.24d && Vector.Y > 7.76d && Vector.X == -4.24f)
            {
                // XHead
                Result = new Point((int)Math.Round(Conversion.Int((Vector.Z + 4.24d) / 1.06d + 32d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 16.24d) / 1.06d + 8d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowBodyOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < 8.36d && Vector.Y > -4.36d && Vector.X == 4.24f)
            {
                // XBody
                Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.Z + 2.12d) / 1.06d + 28d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowBodyOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < 8.36d && Vector.Y > -4.36d && Vector.X == -4.24f)
            {
                // XBody
                Result = new Point((int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 16d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowLeftArmOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < 8.36d && Vector.Y > -4.36d && (Vector.X == 8.12f || Vector.X == 7.09f))
            {
                // XArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.Z + 2.12d) / 1.06d + 56d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 52d)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.Z + 2.12d) / 1.06d + 55d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 52d)));
                }
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowRightArmOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < 8.36d && Vector.Y > -4.36d && (Vector.X == -8.12f || Vector.X == -7.09f))
            {
                // XArms
                Result = new Point((int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 40d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowLeftArmOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < 8.36d && Vector.Y > -4.36d && (Vector.X == 3.88f || Vector.X == 3.91f))
            {
                // XArms
                Result = new Point((int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 48d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 52d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowRightArmOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < 8.36d && Vector.Y > -4.36d && (Vector.X == -3.88f || Vector.X == -3.91f))
            {
                // XArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.Z + 2.12d) / 1.06d + 48d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.Z + 2.12d) / 1.06d + 47d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y + 8.36d) / 1.06d + 36d)));
                }
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowLeftLegOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < -3.64d && Vector.Y > -16.36d && Vector.X == 4.24f)
            {
                // XLeg
                Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.Z + 2.12d) / 1.06d + 8d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y - 3.64d) / 1.06d + 52d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowRightLegOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < -3.64d && Vector.Y > -16.36d && Vector.X == -4.24f)
            {
                // XLeg
                Result = new Point((int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y - 3.64d) / 1.06d + 36d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowRightLegOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < -3.64d && Vector.Y > -16.36d && Vector.X == 0.12f)
            {
                // XLeg
                Result = new Point((int)Math.Round(Conversion.Int(((double)-Vector.Z + 2.12d) / 1.06d + 8d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y - 3.64d) / 1.06d + 36d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowLeftLegOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.Y < -3.64d && Vector.Y > -16.36d && Vector.X == -0.12f)
            {
                // XLeg
                Result = new Point((int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d)), (int)Math.Round(Conversion.Int(((double)-Vector.Y - 3.64d) / 1.06d + 52d)));
                XUp.Z = 1f;
                YUp.Y = 1f;
            }
            else if (ShowHeadOverlay && Vector.Z < 4.24d && Vector.Z > -4.24d && Vector.X < 4.24d && Vector.X > -4.24d && Vector.Y == 16.24f)
            {
                // YHead
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 4.24d) / 1.06d + 40d)), (int)Math.Round(Conversion.Int((Vector.Z + 4.24d) / 1.06d)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowHeadOverlay && Vector.Z < 4.24d && Vector.Z > -4.24d && Vector.X < 4.24d && Vector.X > -4.24d && Vector.Y == 7.76f)
            {
                // YHead
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 4.24d) / 1.06d + 48d)), (int)Math.Round(Conversion.Int((Vector.Z + 4.24d) / 1.06d)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowBodyOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < 4.24d && Vector.X > -4.24d && Vector.Y == 8.36f)
            {
                // YBody
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 4.24d) / 1.06d + 20d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 32d)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowBodyOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < 4.24d && Vector.X > -4.24d && Vector.Y == -4.36f)
            {
                // YBody
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 4.24d) / 1.06d + 28d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 32d)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowLeftArmOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < 8.12d && Vector.X > 3.88d && Vector.Y == 8.36f)
            {
                // YArms
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X - 3.88d) / 1.06d + 52d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 48d)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowLeftArmOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < 8.12d && Vector.X > 3.88d && Vector.Y == -4.36f)
            {
                // YArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int((Vector.X - 3.88d) / 1.06d + 56d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 48d)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int((Vector.X - 3.88d) / 1.06d + 55d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 48d)));
                }
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowRightArmOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < -3.88d && Vector.X > -8.12d && Vector.Y == 8.36f)
            {
                // YArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 8.12d) / 1.06d + 44d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 32d)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 8.12d) / 1.06d + 43d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 32d)));
                }
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowRightArmOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < -3.88d && Vector.X > -8.12d && Vector.Y == -4.36f)
            {
                // YArms
                if (Model == Models.Steve)
                {
                    Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 8.12d) / 1.06d + 48d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 32d)));
                }
                else
                {
                    Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 8.12d) / 1.06d + 46d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 32d)));
                }
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowLeftLegOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < 4.24d && Vector.X > -0.12d && Vector.Y == -3.64f)
            {
                // YLeg
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 0.12d) / 1.06d + 4d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 48d)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowRightLegOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < 0.12d && Vector.X > -4.24d && Vector.Y == -3.64f)
            {
                // YLeg
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 4.24d) / 1.06d + 4d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 32d)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowLeftLegOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < 4.24d && Vector.X > -0.12d && Vector.Y == -16.36f)
            {
                // YLeg
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 0.12d) / 1.06d + 8d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 48d)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }
            else if (ShowRightLegOverlay && Vector.Z < 2.12d && Vector.Z > -2.12d && Vector.X < 0.12d && Vector.X > -4.24d && Vector.Y == -16.36f)
            {
                // YLeg
                Result = new Point((int)Math.Round(Conversion.Int((Vector.X + 4.24d) / 1.06d + 8d)), (int)Math.Round(Conversion.Int((Vector.Z + 2.12d) / 1.06d + 32d)));
                XUp.X = 1f;
                YUp.Z = 1f;
            }

            return Result;
        }

        public object Get2DFrom3D(Vector3 Vector)
        {
            var argXUp = new Vector3();
            var argYUp = new Vector3();
            return Get2DFrom3D(Vector, ref argXUp, ref argYUp);
        }
        public object Get2nd2DFrom3D(Vector3 Vector)
        {
            var argXUp = new Vector3();
            var argYUp = new Vector3();
            return Get2nd2DFrom3D(Vector, ref argXUp, ref argYUp);
        }

        public enum SkinPlace
        {
            Head,
            Body,
            RArm,
            LArm,
            RLeg,
            LLeg,
            Head2,
            Body2,
            RArm2,
            LArm2,
            RLeg2,
            LLeg2
        }
    }
}

