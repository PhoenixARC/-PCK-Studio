using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using PckStudio.Internal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Windows.Media.Media3D;
using PckStudio.Extensions;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using PckStudio.Properties;

namespace PckStudio.Rendering
{
    internal partial class SkinRenderer : Renderer3D
    {
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
                if (skinShader is not null)
                {
                    var texture = new Texture2D(value);
                    texture.Bind(0);
                    skinShader.SetUniform1("u_Texture", 0);
                    Refresh();
                }
                TextureScaleValue = new Vector2(1f / value.Width, 1f / value.Height);
                _texture = value;
            }
        }

        private Vector2 _lookAngle = Vector2.Zero;
        [Description("The offset from the orignal point (for zoom)")]
        [Category("Appearance")]
        public Vector2 LookAngle
        {
            get => _lookAngle;
            set
            {
                _lookAngle = value;
                camera.LookAt(value);
            }
        }

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

        private Vector2 _modelRotation;
        private Vector2 modelRotation
        {
            get => _modelRotation;
            set
            {
                _modelRotation.X = MathHelper.Clamp(value.X, -5f, 5f);
                _modelRotation.Y = MathHelper.Clamp(value.Y, -180f, 180f);
            }
        }

        private bool IsMouseDown;
        private bool IsRightMouseDown;
        private bool IsMouseHidden;
        private Point PreviousMouseLocation;
        private Point MouseLoc;
        private Shader skinShader;

        private Dictionary<string, RenderGroup> renderGroups;

        private Bitmap _texture;

        public SkinRenderer()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;
            MakeCurrent();

            camera = new Camera(new Vector2(0f, 1f), 36f, Vector2.Zero, 60f);

            camera.MinimumFov = 30f;
            camera.MaximumFov = 90f;

            camera.LookAt(new Vector2(0f, 5f));

            GL.DebugMessageCallback(GLDebugMessage, IntPtr.Zero);

            Trace.TraceInformation(GL.GetString(StringName.Version));

            skinShader = Shader.Create(Resources.skinVertexShader, Resources.skinFragment);
            skinShader.Bind();

            Texture = Resources.slim_template;

            renderGroups = new Dictionary<string, RenderGroup>(6)
            {
                { "HEAD", new RenderGroup("HEAD") },
                { "BODY", new RenderGroup("BODY") },
                { "ARM0", new RenderGroup("ARM0") },
                { "ARM1", new RenderGroup("ARM1") },
                { "LEG0", new RenderGroup("LEG0") },
                { "LEG1", new RenderGroup("LEG1") }
            };

            var headbox = new SkinBOX("HEAD", new(-4, -8, -4), new(8,  8, 8), new( 0,  0));
            var bodybox = new SkinBOX("BODY", new(-4,  0, -2), new(8, 12, 4), new(16, 16));
            var arm0box = new SkinBOX("ARM0", new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            var arm1box = new SkinBOX("ARM1", new(-1, -2, -2), new(4, 12, 4), new(32, 48));
            var leg0box = new SkinBOX("LEG0", new(-2,  0, -2), new(4, 12, 4), new( 0, 16));
            var leg1box = new SkinBOX("LEG1", new(-2,  0, -2), new(4, 12, 4), new(16, 48));

            AddGroup(headbox, TextureScaleValue);
            AddGroup(bodybox, TextureScaleValue);
            AddGroup(arm0box, TextureScaleValue);
            AddGroup(arm1box, TextureScaleValue);
            AddGroup(leg0box, TextureScaleValue);
            AddGroup(leg1box, TextureScaleValue);

            GLErrorCheck();
        }

        private void AddGroup(SkinBOX skinBox, Vector2 textureScaleValue)
        {
            if (!renderGroups.ContainsKey(skinBox.Type))
                throw new KeyNotFoundException(skinBox.Type);

            renderGroups[skinBox.Type].AddBox(skinBox, textureScaleValue);
        }

        [Conditional("DEBUG")]
        private void GLErrorCheck()
        {
            var error = GL.GetError();
            Debug.Assert(error == ErrorCode.NoError, error.ToString());
        }

        private RenderBuffer GetBox3D(SkinBOX skinBOX)
        {
            return GetBox3D(skinBOX.Pos.ToOpenTKVector(), skinBOX.Size.ToOpenTKVector(), skinBOX.UV.ToOpenTKVector());
        }

        private RenderBuffer GetBox3D(Vector3 position, Vector3 size, Vector2 uv)
        {
            var vertexData = new TextureVertex[]
            {
                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z, uv.Y  + size.Z) * TextureScaleValue),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y + size.Z) * TextureScaleValue),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y + size.Z + size.Y) * TextureScaleValue),
                new TextureVertex(new Vector3(position.X         , position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z, uv.Y + size.Z + size.Y) * TextureScaleValue),

                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z * 2 + size.X * 2, uv.Y + size.Z) * TextureScaleValue),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z * 2 + size.X, uv.Y + size.Z) * TextureScaleValue),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, position.Z), new Vector2(uv.X + size.Z * 2 + size.X, uv.Y + size.Z + size.Y) * TextureScaleValue),
                new TextureVertex(new Vector3(position.X         , position.Y, position.Z), new Vector2(uv.X + size.Z * 2 + size.X * 2, uv.Y + size.Z + size.Y) * TextureScaleValue),

                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z), new Vector2(uv.X, uv.Y + size.Z) * TextureScaleValue),
                new TextureVertex(new Vector3(position.X         , position.Y, position.Z), new Vector2(uv.X, uv.Y + size.Z + size.Y) * TextureScaleValue),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y) * TextureScaleValue),
                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z, uv.Y) * TextureScaleValue),

                new TextureVertex(new Vector3(position.X         , position.Y, position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y) * TextureScaleValue),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, position.Z), new Vector2(uv.X + size.Z + size.X * 2, uv.Y) * TextureScaleValue),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X * 2, uv.Y  + size.Z) * TextureScaleValue),
                new TextureVertex(new Vector3(position.X         , position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y  + size.Z) * TextureScaleValue),
            };

            var indexBuffer = new IndexBuffer(
                 0, 1, 2, 3, // Face 1 (Front)
                 4, 5, 6, 7, // Face 2 (Back)
                 0, 8, 9, 3, // Face 3 (Left)
                 1, 5, 6, 2, // Face 4 (Right)
                 0, 1, 10, 11, // Face 5(Top)
                12, 13, 14, 15  // Face 6 (Bottom)
                );

            var buffer = new VertexBuffer<TextureVertex>(vertexData, vertexData.Length * TextureVertex.SizeInBytes);
            var layout = new VertexBufferLayout();
            layout.Add<float>(3);
            layout.Add<float>(4);
            layout.Add<float>(2);

            var vertexArray = new VertexArray();

            vertexArray.AddBuffer(buffer, layout);

            return new RenderBuffer(vertexArray, indexBuffer, PrimitiveType.Quads);
        }

        private Matrix4 GetModelFromSkinBOX(SkinBOX skinBox)
        {
            return Matrix4.CreateTranslation(skinBox.Pos.ToOpenTKVector()) * Matrix4.CreateScale(skinBox.Scale);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.R:
                    modelRotation = Vector2.Zero;
                    LookAngle = Vector2.Zero;
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (DesignMode)
            {
                return;
            }

            MakeCurrent();

            camera.Update(Size.Width / (float)Size.Height);

            var viewProjection = camera.GetViewProjection();
            skinShader.SetUniformMat4("u_ViewProjection", ref viewProjection);

            GL.Viewport(Size);

            GL.ClearColor(BackColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.Texture2D); // Enable textures
            GL.Enable(EnableCap.DepthTest); // Enable correct Z Drawings
            GL.DepthFunc(DepthFunction.Lequal); // Enable correct Z Drawings
            GL.Disable(EnableCap.AlphaTest); // Disable transparent

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest); // Enable transparent
            GL.AlphaFunc(AlphaFunction.Greater, 0.4f);

            Matrix4 transform = Matrix4.CreateTranslation(new Vector3(0f, 16f, 0f));
            Matrix4 rotation = Matrix4.CreateFromAxisAngle(new Vector3(-1f, 0f, 0f), MathHelper.DegreesToRadians(modelRotation.X))
                             * Matrix4.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), MathHelper.DegreesToRadians(modelRotation.Y));

            var model = transform * rotation;

            skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(skinShader, renderGroups["HEAD"].GetRenderBuffer());

            transform = Matrix4.CreateTranslation(new Vector3(0f, -4f, 0f));
            model = transform * rotation;

            skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(skinShader, renderGroups["BODY"].GetRenderBuffer());

            transform = Matrix4.CreateTranslation(new Vector3(-5f, -2f, 0f));
            model = transform * rotation;

            skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(skinShader, renderGroups["ARM0"].GetRenderBuffer());

            transform = Matrix4.CreateTranslation(new Vector3(5f, -2f, 0f));
            model = transform * rotation;

            skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(skinShader, renderGroups["ARM1"].GetRenderBuffer());

            transform = Matrix4.CreateTranslation(new Vector3(-2f, -16f, 0f));
            model = transform * rotation;

            skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(skinShader, renderGroups["LEG0"].GetRenderBuffer());

            transform = Matrix4.CreateTranslation(new Vector3(2f, -16f, 0f));
            model = transform * rotation;

            skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(skinShader, renderGroups["LEG1"].GetRenderBuffer());

            SwapBuffers();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            camera.Fov -= e.Delta / System.Windows.Input.Mouse.MouseWheelDeltaForOneLine;
            Refresh();
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
                modelRotation += new Vector2(rotationXDelta, rotationYDelta);
                Refresh();
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                MouseLoc = Cursor.Position;
                return;
            }
            // Move the model
            if (IsRightMouseDown)
            {
                float deltaX = -(Cursor.Position.X - MouseLoc.X) * 0.05f / (float)MathHelper.DegreesToRadians(camera.Fov);
                float deltaY = (Cursor.Position.Y - MouseLoc.Y) * 0.05f / (float)MathHelper.DegreesToRadians(camera.Fov);
                LookAngle += new Vector2(deltaX, deltaY);
                Refresh();
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                MouseLoc = Cursor.Position;
            }
        }
    }
}
