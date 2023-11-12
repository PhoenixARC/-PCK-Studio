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
﻿using System;
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
using System.Windows.Media.Imaging;
using System.Xml.Linq;

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
        public Bitmap Texture { get; set; }

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
        public Vector2 CameraTarget
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

        private Vector2 UvTranslation = new Vector2(1f / 64);
        private const float OverlayScale = 0.1f;

        private bool IsLeftMouseDown;
        private bool IsRightMouseDown;

        private Bitmap RenderTexture
        {
            set
            {
                if (HasValidContext && skinShader is not null)
                {
                    var texture = new Texture2D(value);
                    texture.Bind(0);
                    skinShader.SetUniform1("u_Texture", 0);
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
        private Point MouseLoc;

        private Shader skinShader;
        private SkinANIM _anim;

        private Dictionary<string, CubeRenderGroup> additionalModelRenderGroups;


        private CubeRenderGroup head;
        private CubeRenderGroup headOverlay;
        private CubeRenderGroup body;
        private CubeRenderGroup bodyOverlay;
        private CubeRenderGroup rightArm;
        private CubeRenderGroup rightArmOverlay;
        private CubeRenderGroup leftArm;
        private CubeRenderGroup leftArmOverlay;
        private CubeRenderGroup rightLeg;
        private CubeRenderGroup rightLegOverlay;
        private CubeRenderGroup leftLeg;
        private CubeRenderGroup leftLegOverlay;

        private float animationRot;
        private float animationRotStep = 1f;

        public SkinRenderer() : base()
        {
            InitializeComponent();
            InitializeCamera();
            InitializeSkinData();
            _anim = new SkinANIM(); // use backing field to not raise OnANIMUpdate
            additionalModelRenderGroups = new Dictionary<string, CubeRenderGroup>(6)
            {
                { "HEAD",     new CubeRenderGroup("HEAD") },
                { "BODY",     new CubeRenderGroup("BODY") },
                { "ARM0",     new CubeRenderGroup("ARM0") },
                { "ARM1",     new CubeRenderGroup("ARM1") },
                { "LEG0",     new CubeRenderGroup("LEG0") },
                { "LEG1",     new CubeRenderGroup("LEG1") },

                { "HEADWEAR", new CubeRenderGroup("HEADWEAR") },
                { "JACKET"  , new CubeRenderGroup("JACKET")},
                { "SLEEVE0" , new CubeRenderGroup("SLEEVE0")},
                { "SLEEVE1" , new CubeRenderGroup("SLEEVE1")},
                { "PANTS0"  , new CubeRenderGroup("PANTS0")},
                { "PANTS1"  , new CubeRenderGroup("PANTS1")},
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

        private void InitializeSkinData()
        {
            head = new CubeRenderGroup("Head");
            head.AddCube(new(-4, -8, -4), new(8, 8, 8), new(0, 0));

            headOverlay = new CubeRenderGroup("Head overlay");
            headOverlay.AddCube(new(-4, -8, -4), new(8, 8, 8), new(32, 0), scale: OverlayScale);

            body = new CubeRenderGroup("Body");
            body.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 16));

            bodyOverlay = new CubeRenderGroup("Body overlay");
            bodyOverlay.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 32), scale: OverlayScale);

            rightArm = new CubeRenderGroup("Right arm");
            rightArm.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 16));

            rightArmOverlay = new CubeRenderGroup("Right arm overlay");
            rightArmOverlay.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 32), scale: OverlayScale);

            leftArm = new CubeRenderGroup("Left arm");
            leftArm.AddCube(new(-1, -2, -2), new(4, 12, 4), new(32, 48));

            leftArmOverlay = new CubeRenderGroup("Left arm overlay");
            leftArmOverlay.AddCube(new(-1, -2, -2), new(4, 12, 4), new(48, 48), scale: OverlayScale);

            rightLeg = new CubeRenderGroup("Right leg");
            rightLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 16));

            rightLegOverlay = new CubeRenderGroup("Right leg overlay");
            rightLegOverlay.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 32), scale: OverlayScale);

            leftLeg = new CubeRenderGroup("Left leg");
            leftLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(16, 48));

            leftLegOverlay = new CubeRenderGroup("Left leg overlay");
            leftLegOverlay.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 48), scale: OverlayScale);
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

            Texture ??= Resources.classic_template;

            RenderTexture = Texture;

            foreach (var item in ModelData)
            {
                AddCustomModelPart(item);
            }

            GLErrorCheck();
        }

        public void Reload()
        {
            additionalModelRenderGroups.Clear();
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

            additionalModelRenderGroups[skinBox.Type].AddSkinBox(skinBox);
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
                    CameraTarget = Vector2.Zero;
                    Refresh();
                    return true;
                case Keys.F1:
                    var fileDialog = new OpenFileDialog()
                    {
                        Filter = "texture|*.png",
                    };
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        RenderTexture = Texture = Image.FromFile(fileDialog.FileName) as Bitmap;
                    }
                    return true;
                case Keys.F3:
                    foreach (var item in ModelData)
                    {
                        Debug.WriteLine(item);
                    }
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
                UvTranslation = new Vector2(1f / 64);
                rightArm.ReplaceCube(0, new(-3, -2, -2), new(slimValue, 12, 4), new(40, 16));
                rightArmOverlay.ReplaceCube(0, new(-3, -2, -2), new(slimValue, 12, 4), new(40, 32), scale: OverlayScale);
                leftArm.ReplaceCube(0, new(-1, -2, -2), new(slimValue, 12, 4), new(32, 48));
                leftArmOverlay.ReplaceCube(0, new(-1, -2, -2), new(slimValue, 12, 4), new(48, 48), scale: OverlayScale);

                rightLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
                leftLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(16, 48));
                return;
            }
            UvTranslation = new Vector2(1f / 64, 1f / 32);
            rightArm.ReplaceCube(0, new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            leftArm.ReplaceCube(0, new(-1, -2, -2), new(4, 12, 4), new(40, 16));
            rightLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            leftLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            rightArmOverlay.ReplaceCube(0, new(-3, -2, -2), new(4, 12, 4), new(40, 32), scale: OverlayScale);
            leftArmOverlay.ReplaceCube(0, new(-1, -2, -2), new(4, 12, 4), new(40, 32), scale: OverlayScale);
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
            skinShader.SetUniform2("u_TexScale", UvTranslation);

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

            Matrix4 modelMatrix =
                 Matrix4.CreateFromAxisAngle(-Vector3.UnitX, MathHelper.DegreesToRadians(GlobalModelRotation.X))
                * Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(GlobalModelRotation.Y));

            RenderSkin(modelMatrix);

#if true
            RenderAdditionalModelData("HEAD", new Vector3(0f, 0f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelMatrix);
            RenderAdditionalModelData("BODY", new Vector3(0f, -4f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelMatrix);
            //RenderAdditionalModelData("ARM0", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
            //RenderAdditionalModelData("ARM1", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
            //RenderAdditionalModelData("LEG0", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
            //RenderAdditionalModelData("LEG1", new Vector3(0f, 28f, 0f), /*Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(180f)) **/ modelRotationMatrix);
#endif
            SwapBuffers();
        }

        private void RenderSkin(Matrix4 modelMatrix)
        {
            if (!ANIM.GetFlag(SkinAnimFlag.HEAD_DISABLED))
            {
                RenderSkinPart(head.GetRenderBuffer(), new Vector3(0f, 16f, 0f), modelMatrix);
            }

            if (!ANIM.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED))
            {
                RenderSkinPart(headOverlay.GetRenderBuffer(), new Vector3(0f, 16f + OverlayScale, 0f), modelMatrix);
            }
            
            if (!ANIM.GetFlag(SkinAnimFlag.BODY_DISABLED))
            {
                RenderSkinPart(body.GetRenderBuffer(), new Vector3(0f, -4f, 0f), modelMatrix);
            }

            bool slimModel = ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);
            var extraRightRotation = Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(animationRot));
            var extraLeftRotation = Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-animationRot));
            var translationRight = new Vector3(slimModel ? -4f : -5f, -2f, 0f);
            var translationLeft = new Vector3(5f, -2f, 0f);
            if (ANIM.GetFlag(SkinAnimFlag.ZOMBIE_ARMS))
            {
                extraRightRotation *= Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-90f));
                extraLeftRotation *= Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-90f));
                translationRight.Yz = translationLeft.Yz = new Vector2(-8f, 6f);
            }

            if (!ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED))
            {
                RenderSkinPart(rightArm.GetRenderBuffer(), translationRight, extraRightRotation * modelMatrix);
            }

            if (!ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED))
            {
                RenderSkinPart(leftArm.GetRenderBuffer(), translationLeft, extraLeftRotation * modelMatrix);
            }

            if (!ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED))
            {
                RenderSkinPart(rightLeg.GetRenderBuffer(), new Vector3(-2f, -16f, 0f), modelMatrix);
            }

            if (!ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED))
            {
                RenderSkinPart(leftLeg.GetRenderBuffer(), new Vector3(2f, -16f, 0f), modelMatrix);
            }

            if (ANIM.GetFlag(SkinAnimFlag.RESOLUTION_64x64) || slimModel)
            {
                if (!ANIM.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED))
                {
                    RenderSkinPart(bodyOverlay.GetRenderBuffer(), new Vector3(0f, -4f + OverlayScale, 0f), modelMatrix);
                }

                if (!ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED))
                {
                    RenderSkinPart(rightArmOverlay.GetRenderBuffer(), translationRight, extraRightRotation * modelMatrix);
                }

                if (!ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED))
                {
                    RenderSkinPart(leftArmOverlay.GetRenderBuffer(), translationLeft, extraLeftRotation * modelMatrix);
                }

                if (!ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED))
                {
                    RenderSkinPart(rightLegOverlay.GetRenderBuffer(), new Vector3(-2f, -16f, 0f), modelMatrix);
                }

                if (!ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED))
                {
                    RenderSkinPart(leftLegOverlay.GetRenderBuffer(), new Vector3(2f, -16f, 0f), modelMatrix);
                }
            }
        }

        private void RenderSkinPart(RenderBuffer buffer, Vector3 translation, Matrix4 rotation)
        {
            var transform = Matrix4.CreateTranslation(translation);
            var model = transform * rotation;
            skinShader.SetUniformMat4("u_Model", ref model);
            Renderer.Draw(skinShader, buffer);
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
                CameraTarget += new Vector2(deltaX, deltaY);
                Refresh();
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                MouseLoc = Cursor.Position;
            }
        }
    }
}