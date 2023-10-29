using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using PckStudio.Internal;
using PckStudio.Extensions;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using PckStudio.Properties;
using PckStudio.Forms.Editor;

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
                if (value is null)
                    return;
                
                if (HasValidContext && skinShader is not null)
                {
                    var texture = new Texture2D(value);
                    texture.Bind(0);
                    skinShader.SetUniform1("u_Texture", 0);
                    Refresh();
                }
                _texture = value;
            }
        }

        [Description("Anim flags for special effects applied to the skin")]
        [Category("Appearance")]
        public SkinANIM ANIM
        {
            get => _anim;
            set
            {
                _anim = value;
                OnANIMUpdate();
            }
        }

        [Description("Additional model data")]
        [Category("Appearance")]
        public List<SkinBOX> ModelData { get; } = new List<SkinBOX>();

        [Description("The offset from the orignal point (for zoom)")]
        [Category("Appearance")]
        public Vector2 LookAngle
        {
            get => camera.Position;
            set => camera.LookAt(value);
        }


        private void GLDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
            Debug.WriteLine(source);
            Debug.WriteLine(type);
            Debug.WriteLine(severity);
            Debug.WriteLine(id);
            Debug.WriteLine(msg);
        }

        private Vector2 _globalModelRotation;
        private Vector2 GlobalModelRotation
        {
            get => _globalModelRotation;
            set
            {
                _globalModelRotation.X = MathHelper.Clamp(value.X, -90f, 90f);
                _globalModelRotation.Y = MathHelper.Clamp(value.Y, -180f, 180f);
            }
        }

        private Bitmap _texture;
        private Vector2 TextureScaleValue = new Vector2(1f / 64);
        private const float OverlayScale = 0.1f;

        private bool IsLeftMouseDown;
        private bool IsRightMouseDown;
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
        private Point PreviousMouseLocation;
        private Point MouseLoc;

        private Shader skinShader;
        private SkinANIM _anim;

        private Dictionary<string, RenderGroup> defaultRenderGroups;
        private Dictionary<string, RenderGroup> additionalModelRenderGroups;


        private float animationRot;
        private float animationRotStep = 1f;

        public SkinRenderer()
        {
            InitializeComponent();
            InitializeCamera();
            _anim = new SkinANIM(); // use backing field to not raise OnANIMUpdate
            defaultRenderGroups = new Dictionary<string, RenderGroup>(6)
            {
                { "HEAD",      new RenderGroup("HEAD") },
                { "BODY",      new RenderGroup("BODY") },
                { "ARM0",      new RenderGroup("ARM0") },
                { "ARM1",      new RenderGroup("ARM1") },
                { "LEG0",      new RenderGroup("LEG0") },
                { "LEG1",      new RenderGroup("LEG1") },
                
                { "HEADWEAR",  new RenderGroup("HEADWEAR") },
                { "JACKET",    new RenderGroup("JACKET")},
                { "SLEEVE0",   new RenderGroup("SLEEVE0")},
                { "SLEEVE1",   new RenderGroup("SLEEVE1")},
                { "LEGGING0",  new RenderGroup("LEGGING0")},
                { "LEGGING1",  new RenderGroup("LEGGING1")},
                
                //{ "PANTS0",    new RenderGroup("PANTS0")},
                //{ "PANTS1",    new RenderGroup("PANTS1")},
                //{ "WAIST",     new RenderGroup("WAIST")},
                //{ "SOCK0",     new RenderGroup("SOCK0")},
                //{ "SOCK1",     new RenderGroup("SOCK1")},
                //{ "BOOT0",     new RenderGroup("BOOT0")},
                //{ "BOOT1",     new RenderGroup("BOOT1")},
                //{ "ARMARMOR1", new RenderGroup("ARMARMOR1")},
                //{ "ARMARMOR0", new RenderGroup("ARMARMOR0")},
                //{ "BODYARMOR", new RenderGroup("BODYARMOR")},
                //{ "BELT",      new RenderGroup("BELT")},
            };
            additionalModelRenderGroups = new Dictionary<string, RenderGroup>(6)
            {
                { "HEAD",      new RenderGroup("HEAD") },
                { "BODY",      new RenderGroup("BODY") },
                { "ARM0",      new RenderGroup("ARM0") },
                { "ARM1",      new RenderGroup("ARM1") },
                { "LEG0",      new RenderGroup("LEG0") },
                { "LEG1",      new RenderGroup("LEG1") },

                { "HEADWEAR",  new RenderGroup("HEADWEAR") },
                { "JACKET",    new RenderGroup("JACKET")},
                { "SLEEVE0",   new RenderGroup("SLEEVE0")},
                { "SLEEVE1",   new RenderGroup("SLEEVE1")},
                { "LEGGING0",  new RenderGroup("LEGGING0")},
                { "LEGGING1",  new RenderGroup("LEGGING1")},
            };
        }

        private void InitializeCamera()
        {
            const float distance = 36f;
            camera = new PerspectiveCamera(new Vector2(0f, 5f), distance, Vector2.Zero, 60f)
            {
                MinimumFov = 30f,
                MaximumFov = 90f,
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;
            MakeCurrent();

            Trace.TraceInformation(GL.GetString(StringName.Version));

            GL.DebugMessageCallback(GLDebugMessage, IntPtr.Zero);

            skinShader = Shader.Create(Resources.skinVertexShader, Resources.skinFragment);
            skinShader.Bind();

            if (_texture is null)
            {
                Texture = Resources.classic_template;
                if (Texture.Size == new Size(64, 64))
                {
                    ANIM.SetFlag(SkinAnimFlag.RESOLUTION_64x64, true);
                }
            }

            var headbox        = new SkinBOX("HEAD", new(-4, -8, -4), new(8,  8, 8), new( 0,  0));
            var headoverlaybox = new SkinBOX("HEADWEAR", new(-4, -8, -4), new(8,  8, 8), new(32,  0), scale: OverlayScale);
            var bodybox        = new SkinBOX("BODY", new(-4,  0, -2), new(8, 12, 4), new(16, 16));
            var bodyoverlaybox = new SkinBOX("JACKET", new(-4,  0, -2), new(8, 12, 4), new(16, 32), scale: OverlayScale);
            var arm0box        = new SkinBOX("ARM0", new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            var arm1box        = new SkinBOX("ARM1", new(-1, -2, -2), new(4, 12, 4), new(32, 48));
            var arm0overlaybox = new SkinBOX("SLEEVE0", new(-3, -2, -2), new(4, 12, 4), new(40, 32), scale: OverlayScale);
            var arm1overlaybox = new SkinBOX("SLEEVE1", new(-1, -2, -2), new(4, 12, 4), new(48, 48), scale: OverlayScale);
            var leg0box        = new SkinBOX("LEG0", new(-2,  0, -2), new(4, 12, 4), new( 0, 16));
            var leg0overlaybox = new SkinBOX("LEGGING0", new(-2,  0, -2), new(4, 12, 4), new( 0, 32), scale: OverlayScale);
            var leg1box        = new SkinBOX("LEG1", new(-2,  0, -2), new(4, 12, 4), new(16, 48));
            var leg1overlaybox = new SkinBOX("LEGGING1", new(-2,  0, -2), new(4, 12, 4), new( 0, 48), scale: OverlayScale);

            AddToGroup(headbox);
            AddToGroup(headoverlaybox);
            AddToGroup(bodybox);
            AddToGroup(bodyoverlaybox);
            AddToGroup(arm0box);
            AddToGroup(arm0overlaybox);
            AddToGroup(arm1box);
            AddToGroup(arm1overlaybox);
            AddToGroup(leg0box);
            AddToGroup(leg0overlaybox);
            AddToGroup(leg1box);
            AddToGroup(leg1overlaybox);

            foreach (var item in ModelData)
            {
                AddCustomModelPart(item);
            }

            GLErrorCheck();
        }

        private void AddCustomModelPart(SkinBOX skinBox)
        {
            if (!additionalModelRenderGroups.ContainsKey(skinBox.Type))
                throw new KeyNotFoundException(skinBox.Type);

            additionalModelRenderGroups[skinBox.Type].AddBox(skinBox);
        }

        private void AddToGroup(SkinBOX skinBox)
        {
            if (!defaultRenderGroups.ContainsKey(skinBox.Type))
                throw new KeyNotFoundException(skinBox.Type);

            defaultRenderGroups[skinBox.Type].AddBox(skinBox);
        }

        [Conditional("DEBUG")]
        private void GLErrorCheck()
        {
            var error = GL.GetError();
            Debug.Assert(error == ErrorCode.NoError, error.ToString());
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    if (IsMouseHidden || IsLeftMouseDown || IsRightMouseDown)
                    {
                        IsMouseHidden = IsRightMouseDown = IsLeftMouseDown = false;
                        Cursor.Position = PreviousMouseLocation;
                    }
                    break;
                case Keys.R:
                    GlobalModelRotation = Vector2.Zero;
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
                case Keys.F3:

                    return true;
                case Keys.A:
                    {
                        using var animeditor = new ANIMEditor(ANIM);
                        if (animeditor.ShowDialog() == DialogResult.OK)
                        {
                            ANIM = animeditor.ResultAnim;
                            Refresh();
                        }
                    }
                    return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void OnANIMUpdate()
        {
            bool slim = ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);
            if (slim || ANIM.GetFlag(SkinAnimFlag.RESOLUTION_64x64))
            {
                int slimValue = slim ? 3 : 4;
                TextureScaleValue = new Vector2(1f / 64);
                defaultRenderGroups["ARM0"].ReplaceBox(0, new(-3, -2, -2), new(slimValue, 12, 4), new(40, 16));
                defaultRenderGroups["ARM1"].ReplaceBox(0, new(-1, -2, -2), new(slimValue, 12, 4), new(32, 48));
                defaultRenderGroups["SLEEVE0"].ReplaceBox(0, new(-3, -2, -2), new(slimValue, 12, 4), new(40, 32), scale: OverlayScale);
                defaultRenderGroups["SLEEVE1"].ReplaceBox(0, new(-1, -2, -2), new(slimValue, 12, 4), new(48, 48), scale: OverlayScale);

                defaultRenderGroups["LEG0"].ReplaceBox(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
                defaultRenderGroups["LEG1"].ReplaceBox(0, new(-2, 0, -2), new(4, 12, 4), new(16, 48));
                return;
            }
            TextureScaleValue = new Vector2(1f / 64, 1f / 32);
            defaultRenderGroups["ARM0"].ReplaceBox(0, new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            defaultRenderGroups["ARM1"].ReplaceBox(0, new(-1, -2, -2), new(4, 12, 4), new(40, 16));
            defaultRenderGroups["LEG0"].ReplaceBox(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            defaultRenderGroups["LEG1"].ReplaceBox(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            defaultRenderGroups["SLEEVE0"].ReplaceBox(0, new(-3, -2, -2), new(4, 12, 4), new(40, 32), scale: OverlayScale);
            defaultRenderGroups["SLEEVE1"].ReplaceBox(0, new(-1, -2, -2), new(4, 12, 4), new(40, 32), scale: OverlayScale);
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
            skinShader.SetUniform2("u_TexScale", TextureScaleValue);

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

            Matrix4 modelRotationMatrix = Matrix4.CreateFromAxisAngle(new Vector3(-1f, 0f, 0f), MathHelper.DegreesToRadians(GlobalModelRotation.X))
                             * Matrix4.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), MathHelper.DegreesToRadians(GlobalModelRotation.Y));
            
            
            RenderSkinPartIf("HEAD"    , !ANIM.GetFlag(SkinAnimFlag.HEAD_DISABLED), new Vector3(0f, 16f, 0f), modelRotationMatrix);
            RenderSkinPartIf("HEADWEAR", !ANIM.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED), new Vector3(0f, 16f + OverlayScale, 0f), modelRotationMatrix);

            RenderSkinPartIf("BODY"  , !ANIM.GetFlag(SkinAnimFlag.BODY_DISABLED), new Vector3(0f, -4f, 0f), modelRotationMatrix);
            RenderSkinPartIf("JACKET", !ANIM.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED), new Vector3(0f, -4f + OverlayScale, 0f), modelRotationMatrix);

            var extraRightRotation = Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(animationRot));
            var extraLeftRotation  = Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-animationRot));
            var translationRight   = new Vector3(ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL) ? -4f : -5f, -2f, 0f);
            var translationLeft    = new Vector3(5f, -2f, 0f);
            if (ANIM.GetFlag(SkinAnimFlag.ZOMBIE_ARMS))
            {
                extraRightRotation *= Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-90f));
                extraLeftRotation *= Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-90f));
                translationRight.Yz = translationLeft.Yz = new Vector2(-8f, 6f);
            }

            RenderSkinPartIf("ARM0"   , !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED)        , translationRight, extraRightRotation * modelRotationMatrix);
            RenderSkinPartIf("SLEEVE0", !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED), translationRight, extraRightRotation * modelRotationMatrix);

            RenderSkinPartIf("ARM1"   , !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED)         , translationLeft , extraLeftRotation * modelRotationMatrix);
            RenderSkinPartIf("SLEEVE1", !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED) , translationLeft , extraLeftRotation * modelRotationMatrix);
            
            RenderSkinPartIf("LEG0"    , !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED)        , new Vector3(-2f, -16f, 0f), modelRotationMatrix);
            RenderSkinPartIf("LEGGING0", !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED), new Vector3(-2f, -16f, 0f), modelRotationMatrix);

            RenderSkinPartIf("LEG1"    , !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED)         , new Vector3( 2f, -16f, 0f), modelRotationMatrix);
            RenderSkinPartIf("LEGGING1", !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED) , new Vector3( 2f, -16f, 0f), modelRotationMatrix);

#if flase
            RenderAdditionalModelData("HEAD", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
            RenderAdditionalModelData("BODY", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
            RenderAdditionalModelData("ARM0", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
            RenderAdditionalModelData("ARM1", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
            RenderAdditionalModelData("LEG0", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
            RenderAdditionalModelData("LEG1", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
#endif
            SwapBuffers();
        }

        private void RenderSkinPartIf(string name, bool condition, Vector3 translation, Matrix4 rotation)
        {
            if (condition)
            {
                RenderSkinPart(name, translation, rotation);
            }
        }

        private void RenderSkinPart(string name, Vector3 translation, Matrix4 rotation)
        {
            var transform = Matrix4.CreateTranslation(translation);
            var model = transform * rotation;
            skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(skinShader, defaultRenderGroups[name].GetRenderBuffer());
        }

        private void RenderAdditionalModelData(string name, Vector3 translation, Matrix4 rotation)
        {
            var transform = Matrix4.CreateTranslation(translation);
            var model = transform * rotation;
            skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(skinShader, additionalModelRenderGroups[name].GetRenderBuffer());
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            camera.Fov -= e.Delta / System.Windows.Input.Mouse.MouseWheelDeltaForOneLine;
            Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!IsLeftMouseDown && e.Button == MouseButtons.Left)
            {
                // If the ray didn't hit the model then rotate the model
                PreviousMouseLocation = Cursor.Position; // Store the old mouse position to reset it when the action is over
                if (!IsMouseHidden) // Hide the mouse
                {
                    IsMouseHidden = true;
                }
                MouseLoc = Cursor.Position; // Store the current mouse position to use it for the rotate action
                IsLeftMouseDown = true;
            }
            else if (!IsRightMouseDown && e.Button == MouseButtons.Right)
            {
                PreviousMouseLocation = Cursor.Position; // Store the old mouse position to reset it when the action is over 
                if (!IsMouseHidden) // Hide the mouse
                {
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
                IsMouseHidden = IsLeftMouseDown = IsRightMouseDown = false;
            }
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            if (!Focused)
                return;

            const float angle = 2f;

            animationRot += animationRotStep;
            if (animationRot > angle || animationRot < -angle)
                animationRotStep = -animationRotStep;
            Refresh();
        }

        private void moveTimer_Tick(object sender, EventArgs e)
        {
            if (!Focused)
            {
                return;
            }

            // Rotate the model
            if (IsLeftMouseDown)
            {
                float rotationYDelta = (float)Math.Round((Cursor.Position.X - MouseLoc.X) * 0.5f);
                float rotationXDelta = (float)Math.Round(-(Cursor.Position.Y - MouseLoc.Y) * 0.5f);
                GlobalModelRotation += new Vector2(rotationXDelta, rotationYDelta);
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