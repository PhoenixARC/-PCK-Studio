/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing.Imaging;

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
        public Image Texture
        {
            get => _texture;
            set
            {
                var args = new TextureChangingEventArgs(value);
                Events[nameof(TextureChanging)]?.DynamicInvoke(this, args);
                if (!args.Cancel)
                {
                    RenderTexture = _texture = value; 
                }
            }
        }

        public bool ClampModel { get; set; } = false;

        [Description("Event that gets fired when the Texture is changing")]
        [Category("Property Chnaged")]
        [Browsable(true)]
        public event EventHandler<TextureChangingEventArgs> TextureChanging
        {
            add => Events.AddHandler(nameof(TextureChanging), value);
            remove => Events.RemoveHandler(nameof(TextureChanging), value);
        }


        [Description("Anim flags for special effects applied to the skin")]
        [Category("Appearance")]
        [Browsable(true)]
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
        public ObservableCollection<SkinBOX> ModelData { get; }

        [Description("The offset from the origin point")]
        [Category("Appearance")]
        public Vector2 CameraTarget
        {
            get => camera.Position;
            set
            {
                if (ClampModel)
                value = Vector2.Clamp(value, new Vector2(camera.Distance / 2f * -1), new Vector2(camera.Distance / 2f));
                camera.LookAt(value);
        }
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

        private Vector2 UvTranslation = new Vector2(1f / 64);
        private Size TextureSize = new Size(64, 64);
        private const float OverlayScale = 1.12f;

        private bool _isLeftMouseDown;
        private bool _isRightMouseDown;

        private Image RenderTexture
        {
            set
            {
                if (HasValidContext && _skinShader is not null)
                {
                    TextureSize = value.Width == value.Height ? new Size(64, 64) : new Size(64, 32);
                    UvTranslation = value.Width == value.Height ? new Vector2(1f / 64) : new Vector2(1f / 64, 1f / 32);
                    var texture = new Texture2D(value);
                    texture.Bind(0);
                    _skinShader.SetUniform1("u_Texture", 0);
                    Refresh();
                }
            }
        }

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
        private Point CurrentMouseLocation;

        private Shader _skinShader;
        private SkinANIM _anim;
        private Image _texture;

        private Dictionary<string, CubeRenderGroup> additionalModelRenderGroups;

        private CubeRenderGroup head;
        private CubeRenderGroup body;
        private CubeRenderGroup rightArm;
        private CubeRenderGroup leftArm;
        private CubeRenderGroup rightLeg;
        private CubeRenderGroup leftLeg;

        private float animationRot;
        private float animationRotStep = 1f;

#if DEBUG
        private bool showWireFrame = false;
#endif

        public SkinRenderer() : base()
        {
            InitializeComponent();
            InitializeCamera();
            InitializeSkinData();
            ModelData = new ObservableCollection<SkinBOX>();
            ModelData.CollectionChanged += ModelData_CollectionChanged;
            additionalModelRenderGroups = new Dictionary<string, CubeRenderGroup>(6)
            {
                { "HEAD",     new CubeRenderGroup("HEAD") },
                { "BODY",     new CubeRenderGroup("BODY") },
                { "BODYARMOR",new CubeRenderGroup("BODYARMOR") },
                { "ARM0",     new CubeRenderGroup("ARM0") },
                { "ARM1",     new CubeRenderGroup("ARM1") },
                { "BELT",     new CubeRenderGroup("BELT") },
                { "LEG0",     new CubeRenderGroup("LEG0") },
                { "LEG1",     new CubeRenderGroup("LEG1") },

                { "HEADWEAR", new CubeRenderGroup("HEADWEAR") },
                { "JACKET"  , new CubeRenderGroup("JACKET") },
                { "SLEEVE0" , new CubeRenderGroup("SLEEVE0") },
                { "SLEEVE1" , new CubeRenderGroup("SLEEVE1") },
                { "PANTS0"  , new CubeRenderGroup("PANTS0") },
                { "PANTS1"  , new CubeRenderGroup("PANTS1") },
            };
        }

        private void ModelData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move &&
                e.Action != NotifyCollectionChangedAction.Reset)
            {
                UpdateModelData();
            }
        }

        private const float DefaultCameraDistance = 36f;
        private void InitializeCamera()
        {
            camera = new PerspectiveCamera(new Vector2(0f, 5f), DefaultCameraDistance, Vector2.Zero, 60f)
            {
                MinimumFov = 30f,
                MaximumFov = 90f,
            };
        }

        private void InitializeSkinData()
        {
            head ??= new CubeRenderGroup("Head");
            head.AddCube(new(-4, -8, -4), new(8, 8, 8), new(0, 0), flipZMapping: true);
            head.AddCube(new(-4, -8, -4), new(8, 8, 8), new(32, 0), flipZMapping: true, scale: OverlayScale);
            head.Submit();

            body ??= new CubeRenderGroup("Body");
            body.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 16));
            body.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 32), scale: OverlayScale);
            body.Submit();

            rightArm ??= new CubeRenderGroup("Right arm");
            rightArm.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            rightArm.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 32), scale: OverlayScale);
            rightArm.Submit();
            
            leftArm ??= new CubeRenderGroup("Left arm");
            leftArm.AddCube(new(-1, -2, -2), new(4, 12, 4), new(32, 48));
            leftArm.AddCube(new(-1, -2, -2), new(4, 12, 4), new(48, 48), scale: OverlayScale);
            leftArm.Submit();

            rightLeg ??= new CubeRenderGroup("Right leg");
            rightLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            rightLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 32), scale: OverlayScale);
            rightLeg.Submit();

            leftLeg ??= new CubeRenderGroup("Left leg");
            leftLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(16, 48));
            leftLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 48), scale: OverlayScale);
            leftLeg.Submit();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;

            // use backing field to not raise OnANIMUpdate
            _anim ??= new SkinANIM();
            MakeCurrent();

            Trace.TraceInformation(GL.GetString(StringName.Version));

            _skinShader = Shader.Create(Resources.skinVertexShader, Resources.skinFragmentShader);
            _skinShader.Bind();

            Texture ??= Resources.classic_template;

            RenderTexture = Texture;

            GLErrorCheck();
        }

        public void UpdateModelData()
        {
            foreach (var group in additionalModelRenderGroups.Values)
            {
                group.Clear();
            }
            foreach (var item in ModelData)
            {
                AddCustomModelPart(item);
            }
            Refresh();
        }

        private void AddCustomModelPart(SkinBOX skinBox)
        {
            if (!additionalModelRenderGroups.ContainsKey(skinBox.Type))
                throw new KeyNotFoundException(skinBox.Type);

            CubeRenderGroup group = additionalModelRenderGroups[skinBox.Type];
            group.AddSkinBox(skinBox);
            group.Validate(TextureSize);
            group.Submit();
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
                    ReleaseMouse();
                    var point = new Point(Parent.Location.X + Location.X, Parent.Location.Y + Location.Y);
                    contextMenuStrip1.Show(point);
                    return true;
#if DEBUG
                case Keys.W:
                    GL.PolygonMode(MaterialFace.FrontAndBack, showWireFrame ? PolygonMode.Line : PolygonMode.Fill);
                    Refresh();
                    showWireFrame = !showWireFrame;
                    return true;
                case Keys.F1:
                    var fileDialog = new OpenFileDialog()
                    {
                        Filter = "texture|*.png",
                    };
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Texture = Image.FromFile(fileDialog.FileName);
                    }
                    return true;
#endif
                case Keys.R:
                    GlobalModelRotation = Vector2.Zero;
                    CameraTarget = Vector2.Zero;
                    camera.Distance = DefaultCameraDistance;
                    Refresh();
                    return true;
                case Keys.A:
                    ReleaseMouse();
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

        private void ReleaseMouse()
        {
            if (IsMouseHidden || _isLeftMouseDown || _isRightMouseDown)
            {
                IsMouseHidden = _isRightMouseDown = _isLeftMouseDown = false;
                Cursor.Position = PreviousMouseLocation;
            }
        }

        private void OnANIMUpdate()
        {
            head.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.HEAD_DISABLED));
            head.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED));
            
            body.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.BODY_DISABLED));
            rightArm.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED));
            leftArm.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED));
            rightLeg.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED));
            leftLeg.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED));

            bool slim = ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);
            if (slim || ANIM.GetFlag(SkinAnimFlag.RESOLUTION_64x64))
            {
                UvTranslation = new Vector2(1f / 64);
                body.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED));
                rightArm.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED));
                leftArm.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED));
                rightLeg.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED));
                leftLeg.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED));

                int slimValue = slim ? 3 : 4;
                rightArm.ReplaceCube(0, new(-3, -2, -2), new(slimValue, 12, 4), new(40, 16));
                rightArm.ReplaceCube(1, new(-3, -2, -2), new(slimValue, 12, 4), new(40, 32), scale: OverlayScale);
                leftArm.ReplaceCube(0, new(-1, -2, -2), new(slimValue, 12, 4), new(32, 48));
                leftArm.ReplaceCube(1, new(-1, -2, -2), new(slimValue, 12, 4), new(48, 48), scale: OverlayScale);

                rightLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
                leftLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(16, 48));
                return;
            }
            UvTranslation = new Vector2(1f / 64, 1f / 32);
            rightArm.ReplaceCube(0, new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            leftArm.ReplaceCube(0, new(-1, -2, -2), new(4, 12, 4), new(40, 16), mirrorTexture: true);
            rightLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            leftLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16), mirrorTexture: true);
            
            rightArm.SetEnabled(1, false);
            leftArm.SetEnabled(1, false);
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
            _skinShader.SetUniformMat4("u_ViewProjection", ref viewProjection);
            _skinShader.SetUniform2("u_TexScale", UvTranslation);

            GL.Viewport(Size);

            GL.ClearColor(BackColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.Texture2D); // Enable textures
            GL.Enable(EnableCap.DepthTest); // Enable correct Z Drawings
            GL.DepthFunc(DepthFunction.Lequal); // Enable correct Z Drawings

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest); // Enable transparent
            GL.AlphaFunc(AlphaFunction.Greater, 0.4f);

            Matrix4 modelMatrix = Matrix4.CreateTranslation(0f, 4f, 0f) // <- model rotation pivot point
                * Matrix4.CreateFromAxisAngle(-Vector3.UnitX, MathHelper.DegreesToRadians(GlobalModelRotation.X))
                * Matrix4.CreateFromAxisAngle( Vector3.UnitY, MathHelper.DegreesToRadians(GlobalModelRotation.Y));

            bool slimModel = ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);

            const float rotationAngle = 2.5f;
            var extraLegRightRotation = Matrix4.Identity;
            var extraLegLeftRotation = Matrix4.Identity;
            var extraArmRightRotation = Matrix4.Identity;
            var extraArmLeftRotation = Matrix4.Identity;
            
            if (!ANIM.GetFlag(SkinAnimFlag.STATIC_ARMS))
            {
                extraArmRightRotation = Matrix4.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(-rotationAngle));
                extraArmLeftRotation  = Matrix4.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(rotationAngle));
            }

            if (!ANIM.GetFlag(SkinAnimFlag.STATIC_LEGS))
            {
                extraLegRightRotation = Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-rotationAngle));
                extraLegLeftRotation  = Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(rotationAngle));
            }

            if (ANIM.GetFlag(SkinAnimFlag.ZOMBIE_ARMS))
            {
                var rotation = Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-90f));
                extraArmRightRotation *= rotation;
                extraArmLeftRotation  *= rotation;
            }

            if (ANIM.GetFlag(SkinAnimFlag.STATUE_OF_LIBERTY))
            {
                extraArmRightRotation = Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-180f));
            }

            RenderBodyPart(head.GetRenderBuffer()    , Vector3.Zero, Vector3.Zero, modelMatrix, "HEAD", "HEADWEAR");
            RenderBodyPart(body.GetRenderBuffer()    , Vector3.Zero, Vector3.Zero, modelMatrix, "BODY", "JACKET");
            RenderBodyPart(rightArm.GetRenderBuffer(), new Vector3(0f, 0f, -2f), new Vector3(slimModel ? -4f : -5f, -2f, 2f), extraArmRightRotation * modelMatrix, "ARM0", "SLEEVE0");
            RenderBodyPart(leftArm.GetRenderBuffer() , new Vector3(0f, 0f, -2f), new Vector3(                   5f, -2f, 2f), extraArmLeftRotation  * modelMatrix, "ARM1", "SLEEVE1");
            RenderBodyPart(rightLeg.GetRenderBuffer(), new Vector3(0f, 0f, 0f), new Vector3(-2f, -12f, 0f), extraLegRightRotation * modelMatrix, "LEG0", "PANTS0");
            RenderBodyPart(leftLeg.GetRenderBuffer() , new Vector3(0f, 0f, 0f), new Vector3( 2f, -12f, 0f), extraLegLeftRotation  * modelMatrix, "LEG1", "PANTS1");
            
            SwapBuffers();
        }

        private void RenderBodyPart(RenderBuffer baseBuffer, Vector3 pivot, Vector3 translation, Matrix4 rotation, params string[] additionalData)
        {
            RenderPart(baseBuffer, pivot, translation, rotation);
            foreach (var data in additionalData)
            {
                RenderAdditionalModelData(data, pivot, translation, rotation);
            }
        }

        private void RenderPart(RenderBuffer buffer, Vector3 pivot, Vector3 translation, Matrix4 rotation)
        {
            var model = Matrix4.CreateTranslation(translation) * Matrix4.CreateTranslation(pivot);
            model *= rotation;
            model *= Matrix4.CreateTranslation(pivot).Inverted();
            _skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(_skinShader, buffer);
        }

        private void RenderAdditionalModelData(string name, Vector3 pivot, Vector3 translation, Matrix4 rotation)
        {
            RenderPart(additionalModelRenderGroups[name].GetRenderBuffer(), pivot, translation, rotation);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            camera.Distance -= e.Delta / System.Windows.Input.Mouse.MouseWheelDeltaForOneLine;
            Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!_isLeftMouseDown && e.Button == MouseButtons.Left)
            {
                // If the ray didn't hit the model then rotate the model
                PreviousMouseLocation = Cursor.Position; // Store the old mouse position to reset it when the action is over
                if (!IsMouseHidden) // Hide the mouse
                {
                    IsMouseHidden = true;
                }
                CurrentMouseLocation = Cursor.Position; // Store the current mouse position to use it for the rotate action
                _isLeftMouseDown = true;
            }
            else if (!_isRightMouseDown && e.Button == MouseButtons.Right)
            {
                PreviousMouseLocation = Cursor.Position; // Store the old mouse position to reset it when the action is over 
                if (!IsMouseHidden) // Hide the mouse
                {
                    IsMouseHidden = true;
                }
                CurrentMouseLocation = Cursor.Position; // Store the current mouse position to use it for the move action
                _isRightMouseDown = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            ReleaseMouse();
            base.OnMouseUp(e);
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
            if (_isLeftMouseDown)
            {
                float rotationYDelta = (float)Math.Round((Cursor.Position.X - CurrentMouseLocation.X) * 0.5f);
                float rotationXDelta = (float)Math.Round(-(Cursor.Position.Y - CurrentMouseLocation.Y) * 0.5f);
                GlobalModelRotation += new Vector2(rotationXDelta, rotationYDelta);
                Refresh();
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                CurrentMouseLocation = Cursor.Position;
                return;
            }
            // Move the model
            if (_isRightMouseDown)
            {
                float deltaX = -(Cursor.Position.X - CurrentMouseLocation.X) * 0.05f / (float)MathHelper.DegreesToRadians(camera.Fov);
                float deltaY = (Cursor.Position.Y - CurrentMouseLocation.Y) * 0.05f / (float)MathHelper.DegreesToRadians(camera.Fov);
                CameraTarget += new Vector2(deltaX, deltaY);
                Refresh();
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                CurrentMouseLocation = Cursor.Position;
            }
        }

        private void reInitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            head.Clear();
            body.Clear();
            rightArm.Clear();
            leftArm.Clear();
            rightLeg.Clear();
            leftLeg.Clear();

            InitializeSkinData();

            OnANIMUpdate();
            Refresh();
        }

        /// <summary>
        /// Captures the currently displayed frame
        /// </summary>
        /// <returns>Thumbnail of the cameras current view space</returns>
        public Image GetThumbnail()
        {
            Bitmap bmp = new Bitmap(Width, Height);
            BitmapData data = bmp.LockBits(ClientRectangle, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            MakeCurrent();
            GL.Finish();
            GL.ReadPixels(0, 0, Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }
    }
}