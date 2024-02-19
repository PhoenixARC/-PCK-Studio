/* Copyright (c) 2024-present miku-666
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
//#define USE_FRAMEBUFFER
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
using PckStudio.Properties;
using PckStudio.Forms.Editor;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing.Imaging;
using System.IO;
using PckStudio.Rendering.Camera;
using PckStudio.Rendering.Texture;
using PckStudio.Rendering.Shader;
using System.Linq;

namespace PckStudio.Rendering
{
    internal partial class SkinRenderer : SceneViewport
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
                OnTextureChanging(this, args);
                if (!args.Cancel)
                {
                    _texture = value;
                    TextureSize = value.Width == value.Height ? new Size(64, 64) : new Size(64, 32);
                }
            }
        }

        public bool ClampModel { get; set; } = false;
        public bool ShowGuideLines
        {
            get => guidelineMode != GuidelineMode.None;
            set
            {
                if (value)
                {
                    guidelineMode = GuidelineMode.Cubical;
                    return;
                }
                guidelineMode = GuidelineMode.None;
            }
        }

        [Description("Event that gets fired when the Texture is changing")]
        [Category("Property Chnaged")]
        [Browsable(true)]
        public event EventHandler<TextureChangingEventArgs> TextureChanging
        {
            add => Events.AddHandler(nameof(TextureChanging), value);
            remove => Events.RemoveHandler(nameof(TextureChanging), value);
        }

        [Browsable(false)]
        public SkinANIM ANIM
        {
            get => _anim;
            set
            {
                _anim = value;
                OnANIMUpdate();
                if (initialized)
                {
                    MakeCurrent();
                    UploadMeshData();
                }
            }
        }

        public ObservableCollection<SkinBOX> ModelData { get; }
    
        /// <summary>
        /// Captures the currently displayed frame
        /// </summary>
        /// <returns>Image of the cameras current view</returns>
        // TODO: add thumbnail size argument
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


        private enum GuidelineMode
        {
            None = -1,
            Cubical,
            Skeleton
        };

        private GuidelineMode guidelineMode { get; set; } = GuidelineMode.None;

        private Vector2 _globalModelRotation;
        private Vector2 GlobalModelRotation
        {
            get => _globalModelRotation;
            set
            {
                _globalModelRotation.X = MathHelper.Clamp(value.X, -60f, 60f);
                _globalModelRotation.Y = value.Y % 360f;
            }
        }

        public Size TextureSize { get; private set; } = new Size(64, 64);
        public Vector2 TillingFactor => new Vector2(1f / TextureSize.Width, 1f / TextureSize.Height);
        private const float OverlayScale = 1.12f;

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

        private ShaderLibrary _shaders;
        private SkinANIM _anim;
        private Image _texture;
        private Texture2D skinTexture;
#if USE_FRAMEBUFFER
        private FrameBuffer framebuffer;
        private Texture2D framebufferTexture;
        private ShaderProgram framebufferShader;
        private VertexArray framebufferVAO;
#endif
        private DrawContext _cubicalDrawContext;
        private DrawContext _skeletonDrawContext;

        private DrawContext _skyboxRenderBuffer;
        private CubeTexture _skyboxTexture;
        private float skyboxRotation = 0f;
        private float skyboxRotationStep = 0.5f;

        private Dictionary<string, CubeBatchMesh> meshStorage;
        private Dictionary<string, float> partOffset;
        
        private CubeBatchMesh head;
        private CubeBatchMesh body;
        private CubeBatchMesh rightArm;
        private CubeBatchMesh leftArm;
        private CubeBatchMesh rightLeg;
        private CubeBatchMesh leftLeg;

        private CubeBatchMesh headOverlay;
        private CubeBatchMesh bodyOverlay;
        private CubeBatchMesh rightArmOverlay;
        private CubeBatchMesh leftArmOverlay;
        private CubeBatchMesh rightLegOverlay;
        private CubeBatchMesh leftLegOverlay;

        private float animationCurrentRotationAngle;
        private float animationRotationStep = 0.5f;
        private float animationMaxAngleInDegrees = 5f;

        private bool showWireFrame = false;

        private Matrix4 RightArmMatrix { get; set; } = Matrix4.CreateFromAxisAngle(Vector3.UnitZ,  25f);
        private Matrix4 LeftArmMatrix  { get; set; } = Matrix4.CreateFromAxisAngle(Vector3.UnitZ, -25f);

        private static Vector3[] cubeVertices = new Vector3[]
        {
            // front
            new Vector3(-1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f,  1.0f,  1.0f),
            new Vector3(-1.0f,  1.0f,  1.0f),
            // back
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3( 1.0f, -1.0f, -1.0f),
            new Vector3( 1.0f,  1.0f, -1.0f),
            new Vector3(-1.0f,  1.0f, -1.0f)
        };

        private static Vector4[] rectVertices = new Vector4[]
        {
            new Vector4( 1.0f, -1.0f, 1.0f, 0.0f),
            new Vector4(-1.0f, -1.0f, 0.0f, 0.0f),
            new Vector4(-1.0f,  1.0f, 0.0f, 1.0f),
            new Vector4( 1.0f,  1.0f, 1.0f, 1.0f),
            new Vector4( 1.0f, -1.0f, 1.0f, 0.0f),
            new Vector4(-1.0f,  1.0f, 0.0f, 1.0f),
        };
        private bool initialized = false;

        public SkinRenderer() : base()
        {
            InitializeSkinData();
            meshStorage = new Dictionary<string, CubeBatchMesh>()
            {
                { "HEAD", head },
                { "BODY", body },
                { "ARM0", rightArm },
                { "ARM1", leftArm },
                { "LEG0", rightLeg },
                { "LEG1", leftLeg },

                { "HEADWEAR", headOverlay },
                { "JACKET"  , bodyOverlay },
                { "SLEEVE0" , rightArmOverlay },
                { "SLEEVE1" , leftArmOverlay },
                { "PANTS0"  , rightLegOverlay },
                { "PANTS1"  , leftLegOverlay },

                { "BODYARMOR", new CubeBatchMesh("BODYARMOR") },
                { "BELT",      new CubeBatchMesh("BELT") },
                { "ARMARMOR0",      new CubeBatchMesh("ARMARMOR0") },
                { "ARMARMOR1",      new CubeBatchMesh("ARMARMOR1") },
            };
            partOffset = new Dictionary<string, float>()
            {
                { "HEAD", 0f },
                { "BODY", 0f },
                { "ARM0", 0f },
                { "ARM1", 0f },
                { "LEG0", 0f },
                { "LEG1", 0f },

                { "HEADWEAR" , 0f },
                { "JACKET"   , 0f },
                { "SLEEVE0"  , 0f },
                { "SLEEVE1"  , 0f },
                { "PANTS0"   , 0f },
                { "PANTS1"   , 0f },

                { "BODYARMOR", 0f },
                { "BELT"     , 0f },

                { "HELMET"     , 0f },

                { "BOOT0"     , 0f },
                { "BOOT1"     , 0f },

                { "TOOL0"     , 0f },
                { "TOOL1"     , 0f },
            };

            InitializeCamera();
            InitializeComponent();

            _shaders = new ShaderLibrary();
            ANIM ??= new SkinANIM(SkinAnimMask.RESOLUTION_64x64);
            OnTimerTick = AnimationTick;
            ModelData = new ObservableCollection<SkinBOX>();
            ModelData.CollectionChanged += ModelData_CollectionChanged;
        }

        public void InitializeGL()
        {
            MakeCurrent();
            InitializeShaders();
            InitializeFramebuffer();
            UploadMeshData();
            Renderer.SetClearColor(BackColor);
            initialized = true;
        }

        private const float DefaultCameraDistance = 64f;
        private void InitializeCamera()
        {
            Camera.Distance = DefaultCameraDistance;
        }

        private void InitializeSkinData()
        {
            head ??= new CubeBatchMesh("Head");
            head.AddCube(new(-4, -8, -4), new(8, 8, 8), new(0, 0), flipZMapping: true);
            
            headOverlay ??= new CubeBatchMesh("Head Overlay", OverlayScale);
            headOverlay.AddCube(new(-4, -8, -4), new(8, 8, 8), new(32, 0), flipZMapping: true, scale: OverlayScale);

            body ??= new CubeBatchMesh("Body");
            body.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 16));
            
            bodyOverlay ??= new CubeBatchMesh("Body Overlay", OverlayScale);
            bodyOverlay.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 32), scale: OverlayScale);

            rightArm ??= new CubeBatchMesh("Right Arm");
            rightArm.Pivot = new Vector3(4f, 2f, 0f);
            rightArm.Translation = new Vector3(-5f, -2f, 0f);
            rightArm.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            
            rightArmOverlay ??= new CubeBatchMesh("Right Arm Overlay", OverlayScale);
            rightArmOverlay.Pivot = new Vector3(4f, 2f, 0f);
            rightArmOverlay.Translation = new Vector3(-5f, -2f, 0f);
            rightArmOverlay.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 32), scale: OverlayScale);
            
            leftArm ??= new CubeBatchMesh("Left Arm");
            leftArm.Pivot = new Vector3(-4f, 2f, 0f);
            leftArm.Translation = new Vector3(5f, -2f, 0f);
            leftArm.AddCube(new(-1, -2, -2), new(4, 12, 4), new(32, 48));

            leftArmOverlay ??= new CubeBatchMesh("Left Arm Overlay", OverlayScale);
            leftArmOverlay.Pivot = new Vector3(-4f, 2f, 0f);
            leftArmOverlay.Translation = new Vector3(5f, -2f, 0f);
            leftArmOverlay.AddCube(new(-1, -2, -2), new(4, 12, 4), new(48, 48), scale: OverlayScale);

            rightLeg ??= new CubeBatchMesh("Right Leg");
            rightLeg.Pivot = new Vector3(0f, 12f, 0f);
            rightLeg.Translation = new Vector3(-2f, -12f, 0f);
            rightLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 16));

            rightLegOverlay ??= new CubeBatchMesh("Right Leg Overlay", OverlayScale);
            rightLegOverlay.Pivot = new Vector3(0f, 12f, 0f);
            rightLegOverlay.Translation = new Vector3(-2f, -12f, 0f);
            rightLegOverlay.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 32), scale: OverlayScale);

            leftLeg ??= new CubeBatchMesh("Left Leg");
            leftLeg.Pivot = new Vector3(0f, 12f, 0f);
            leftLeg.Translation = new Vector3(2f, -12f, 0f);
            leftLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(16, 48));

            leftLegOverlay ??= new CubeBatchMesh("Left Leg Overlay", OverlayScale);
            leftLegOverlay.Pivot = new Vector3(0f, 12f, 0f);
            leftLegOverlay.Translation = new Vector3(2f, -12f, 0f);
            leftLegOverlay.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 48), scale: OverlayScale);
        }

        private void InitializeShaders()
        {
            MakeCurrent();

            Trace.TraceInformation(GL.GetString(StringName.Version));

            // Skin shader
            {
                var skinShader = ShaderProgram.Create(
                    new ShaderSource(ShaderType.VertexShader, Resources.skinVertexShader),
                    new ShaderSource(ShaderType.FragmentShader, Resources.skinFragmentShader),
                    new ShaderSource(ShaderType.GeometryShader, Resources.skinGeometryShader)
                    );
                skinShader.Bind();
                skinShader.SetUniform1("u_Texture", 0);
                skinShader.Validate();
                _shaders.AddShader("SkinShader", skinShader);
                GLErrorCheck();

                skinTexture = new Texture2D(0);
                skinTexture.PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                skinTexture.InternalPixelFormat = PixelInternalFormat.Rgba8;
                skinTexture.MinFilter = TextureMinFilter.Nearest;
                skinTexture.MagFilter = TextureMagFilter.Nearest;
                skinTexture.WrapS = TextureWrapMode.Repeat;
                skinTexture.WrapT = TextureWrapMode.Repeat;
                
                Texture ??= Resources.classic_template;

                skinShader.Unbind();
                GLErrorCheck();
            }

            // Skybox shader
            {
                var skyboxVAO = new VertexArray();
                var skyboxVBO = new VertexBuffer();
                skyboxVBO.SetData(cubeVertices);
                var vboLayout = new VertexBufferLayout();
                vboLayout.Add<float>(3);
                skyboxVAO.AddBuffer(skyboxVBO, vboLayout);
                var skybocIBO = IndexBuffer.Create(
                    // front
                    0, 1, 2,
                    2, 3, 0,
                    // right
                    1, 5, 6,
                    6, 2, 1,
                    // back
                    7, 6, 5,
                    5, 4, 7,
                    // left
                    4, 0, 3,
                    3, 7, 4,
                    // bottom
                    4, 5, 1,
                    1, 0, 4,
                    // top
                    3, 2, 6,
                    6, 7, 3);

                _skyboxRenderBuffer = new DrawContext(skyboxVAO, skybocIBO, PrimitiveType.Triangles);

                skyboxVAO.Unbind();
                skybocIBO.Unbind();

                var skyboxShader = ShaderProgram.Create(Resources.skyboxVertexShader, Resources.skyboxFragmentShader);
                skyboxShader.Bind();
                skyboxShader.SetUniform1("skybox", 1);
                skyboxShader.SetUniform1("brightness", 1f);
                skyboxShader.Validate();
                _shaders.AddShader("SkyboxShader", skyboxShader);

                string customSkyboxFilepath = Path.Combine(Program.AppData, "cubemap.png");
                Image skyboxImage = File.Exists(customSkyboxFilepath)
                    ? Image.FromFile(customSkyboxFilepath)
                    : Resources.DefaultSkyTexture;

                _skyboxTexture = new CubeTexture(skyboxImage, 1);
                _skyboxTexture.MinFilter = TextureMinFilter.Linear;
                _skyboxTexture.MagFilter = TextureMagFilter.Linear;

                _skyboxTexture.WrapS = TextureWrapMode.ClampToEdge;
                _skyboxTexture.WrapT = TextureWrapMode.ClampToEdge;
                _skyboxTexture.WrapR = TextureWrapMode.ClampToEdge;
                _skyboxTexture.Unbind();
                skyboxShader.Unbind();

                GLErrorCheck();
            }
#if USE_FRAMEBUFFER
            // Framebuffer shader
            {
                framebufferShader = ShaderProgram.Create(Resources.framebufferVertexShader, Resources.framebufferFragmentShader);
                framebufferShader.Bind();
                framebufferShader.SetUniform1("screenTexture", 0);
                framebufferShader.Validate();
            
                GLErrorCheck();
            }
#endif
            // Line Shader
            {
                var lineShader = ShaderProgram.Create(Resources.lineVertexShader, Resources.lineFragmentShader);
                lineShader.Bind();
                lineShader.SetUniform4("baseColor", Color.WhiteSmoke);
                lineShader.Validate();
                _shaders.AddShader("LineShader", lineShader);

                Color lineColor = Color.Aquamarine;

                // Cubical draw context
                {
                    VertexArray lineVAO = new VertexArray();
                    List<LineVertex> vertices = new List<LineVertex>(24 * 6);
                    vertices.AddRange(head.GetCubical(0).Select(pos => new LineVertex(pos, lineColor)));
                    vertices.AddRange(body.GetCubical(0).Select(pos => new LineVertex(pos, lineColor)));
                    vertices.AddRange(rightArm.GetCubical(0).Select(pos => new LineVertex(pos, lineColor)));
                    vertices.AddRange(leftArm.GetCubical(0).Select(pos => new LineVertex(pos, lineColor)));
                    vertices.AddRange(rightLeg.GetCubical(0).Select(pos => new LineVertex(pos, lineColor)));
                    vertices.AddRange(leftLeg.GetCubical(0).Select(pos => new LineVertex(pos, lineColor)));
                    VertexBuffer buffer = new VertexBuffer();
                    buffer.SetData(vertices.ToArray());
                    VertexBufferLayout layout = new VertexBufferLayout();
                    layout.Add<float>(3);
                    layout.Add<float>(4);
                    lineVAO.AddBuffer(buffer, layout);
                    lineVAO.Bind();

                    _cubicalDrawContext = new DrawContext(lineVAO, buffer.GenIndexBuffer(), PrimitiveType.Lines);
                }

                GLErrorCheck();

                // Skeleton draw context
                {
                    VertexArray lineVAO = new VertexArray();
                    Vector3 bodyCenterTop = body.GetFaceCenter(0, CubeData.CubeFace.Top);
                    Vector3 bodyCenterBottom = body.GetFaceCenter(0, CubeData.CubeFace.Bottom);
                    LineVertex[] data = [
                        new LineVertex(head.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new LineVertex(bodyCenterTop, lineColor),
                    
                        new LineVertex(rightArm.GetFaceCenter(0, CubeData.CubeFace.Bottom), lineColor),
                        new LineVertex(rightArm.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new LineVertex(rightArm.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new LineVertex(bodyCenterTop, lineColor),

                        new LineVertex(leftArm.GetFaceCenter(0, CubeData.CubeFace.Bottom), lineColor),
                        new LineVertex(leftArm.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new LineVertex(leftArm.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new LineVertex(bodyCenterTop, lineColor),

                        new LineVertex(rightLeg.GetFaceCenter(0, CubeData.CubeFace.Bottom), lineColor),
                        new LineVertex(rightLeg.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new LineVertex(rightLeg.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new LineVertex(bodyCenterBottom, lineColor),
                        new LineVertex(bodyCenterBottom, lineColor),
                        new LineVertex(bodyCenterTop, lineColor),

                        new LineVertex(leftLeg.GetFaceCenter(0, CubeData.CubeFace.Bottom), lineColor),
                        new LineVertex(leftLeg.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new LineVertex(leftLeg.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new LineVertex(bodyCenterBottom, lineColor),
                        new LineVertex(bodyCenterBottom, lineColor),
                        new LineVertex(bodyCenterTop, lineColor),
                    ];
                    VertexBuffer buffer = new VertexBuffer();
                    buffer.SetData(data);
                    VertexBufferLayout layout = new VertexBufferLayout();
                    layout.Add<float>(3);
                    layout.Add<float>(4);
                    lineVAO.AddBuffer(buffer, layout);
                    lineVAO.Bind();

                    _skeletonDrawContext = new DrawContext(lineVAO, buffer.GenIndexBuffer(), PrimitiveType.Lines);
                }

                GLErrorCheck();
            }
        }

        private DrawContext GetGuidelineDrawContext()
        {
            return guidelineMode == GuidelineMode.Skeleton ? _skeletonDrawContext : _cubicalDrawContext;
        }

        protected virtual void OnTextureChanging(object sender, TextureChangingEventArgs e)
        {
            e.Cancel = e.NewTexture is null;
            if (!e.Cancel)
            {
                skinTexture.LoadImageData(e.NewTexture);
                GLErrorCheck();
            }
        }

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

            int rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.Width, Size.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

            framebufferVAO = new VertexArray();
            VertexBuffer vertexBuffer = new VertexBuffer();
            vertexBuffer.SetData(rectVertices);
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.Add<float>(4);
            framebufferVAO.AddBuffer(vertexBuffer, layout);
            framebuffer.CheckStatus();

            if (framebuffer.Status != FramebufferErrorCode.FramebufferComplete)
            {
                Debug.Fail($"Framebuffer status: '{framebuffer.Status}'");
            }

            framebuffer.Unbind();
#endif
        }

        private void UploadMeshData()
        {
            foreach (var cubeMesh in meshStorage?.Values)
            {
                cubeMesh?.UploadData();
            }
        }

        public void SetPartOffset(SkinPartOffset offset)
        {
            SetPartOffset(offset.Type, offset.Value);
        }

        public void SetPartOffset(string name, float value)
        {
            if (!partOffset.ContainsKey(name))
            {
                Trace.TraceInformation($"[{nameof(SetPartOffset)}]: '{name}' is not inside {nameof(partOffset)}");
                return;
            }
            partOffset[name] = value;
        }

        private float GetOffset(string name)
        {
            return partOffset.ContainsKey(name) ? partOffset[name] : 0f;
        }

        internal void ResetOffsets()
        {
            foreach (var key in partOffset.Keys.ToList())
            {
                partOffset[key] = 0f;
            }
        }

        private void ModelData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO: dont re-initialize everytime..
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    ReInitialzeSkinData();
                    goto default;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    MakeCurrent();
                    UploadMeshData();
                    break;
            }
        }

        private void AddCustomModelPart(SkinBOX skinBox)
        {
            if (!meshStorage.ContainsKey(skinBox.Type))
                throw new KeyNotFoundException(skinBox.Type);

            CubeBatchMesh cubeMesh = meshStorage[skinBox.Type];
            cubeMesh.AddSkinBox(skinBox);
        }

        [Conditional("DEBUG")]
        private void GLErrorCheck()
        {
            var error = GL.GetError();
            Debug.Assert(error == ErrorCode.NoError, error.ToString());
        }

        private void ReleaseMouse()
        {
            if (IsMouseHidden)
            {
                IsMouseHidden = false;
                Cursor.Position = PreviousMouseLocation;
            }
        }

        private void OnANIMUpdate()
        {
            head.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.HEAD_DISABLED));
            headOverlay.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED));
            
            body.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.BODY_DISABLED));
            rightArm.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED));
            leftArm.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED));
            rightLeg.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED));
            leftLeg.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED));

            bool slim = ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);
            if (slim || ANIM.GetFlag(SkinAnimFlag.RESOLUTION_64x64))
            {
                TextureSize = new Size(64, 64);
                bodyOverlay.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED));
                rightArmOverlay.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED));
                leftArmOverlay.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED));
                rightLegOverlay.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED));
                leftLegOverlay.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED));

                int slimValue = slim ? 3 : 4;
                rightArm.ReplaceCube(0, new(-3, -2, -2), new(slimValue, 12, 4), new(40, 16));
                rightArmOverlay.ReplaceCube(0, new(-3, -2, -2), new(slimValue, 12, 4), new(40, 32), scale: OverlayScale);
                leftArm.ReplaceCube(0, new(-1, -2, -2), new(slimValue, 12, 4), new(32, 48));
                leftArmOverlay.ReplaceCube(0, new(-1, -2, -2), new(slimValue, 12, 4), new(48, 48), scale: OverlayScale);

                rightLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
                leftLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(16, 48));
                return;
            }
            
            TextureSize = new Size(64, 32);
            
            bodyOverlay.SetEnabled(0, false);

            rightArm.ReplaceCube(0, new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            rightArmOverlay.SetEnabled(0, false);
            
            leftArm.ReplaceCube(0, new(-1, -2, -2), new(4, 12, 4), new(40, 16), mirrorTexture: true);
            leftArmOverlay.SetEnabled(0, false);

            rightLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            rightLegOverlay.SetEnabled(0, false);
            leftLeg.ReplaceCube (0, new(-2, 0, -2), new(4, 12, 4), new(0, 16), mirrorTexture: true);
            leftLegOverlay.SetEnabled(0, false);
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
                case Keys.F3:
                    showWireFrame = !showWireFrame;
                    return true;
                case Keys.R:
                    GlobalModelRotation = Vector2.Zero;
                    Camera.Distance = DefaultCameraDistance;
                    Camera.FocalPoint = Vector3.Zero;
                    return true;
                case Keys.A:
                    ReleaseMouse();
                    {
                        using var animeditor = new ANIMEditor(ANIM);
                        if (animeditor.ShowDialog() == DialogResult.OK)
                        {
                            ANIM = animeditor.ResultAnim;
                        }
                    }
                    return true;
            }
            return base.ProcessDialogKey(keyData);
        }

#if USE_FRAMEBUFFER
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!IsHandleCreated || DesignMode)
                return;
            MakeCurrent();
            if (framebuffer is not null)
            {
                framebuffer.Bind();

                framebufferTexture.Bind();
                framebufferTexture.SetSize(Size);
                framebufferTexture.Unbind();

                int rbo = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.Width, Size.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

                FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                if (status != FramebufferErrorCode.FramebufferComplete)
                {
                    Debug.Fail("");
                }
                framebuffer.Unbind();
            }

        }
#endif

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (DesignMode)
            {
                return;
            }

            MakeCurrent();

#if USE_FRAMEBUFFER
            framebuffer.Bind();
#endif
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.Enable(EnableCap.DepthTest); // Enable correct Z Drawings
            GL.Enable(EnableCap.LineSmooth);
            Matrix4 viewProjection = Camera.GetViewProjection();

            // Render Skybox
            {
                GL.DepthFunc(DepthFunction.Lequal);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                var skyboxShader = _shaders.GetShader("SkyboxShader");
                skyboxShader.Bind();
                _skyboxTexture.Bind();

                var view = new Matrix4(new Matrix3(Matrix4.LookAt(Camera.WorldPosition, Camera.WorldPosition + Camera.Orientation, Camera.Up)))
                    * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(skyboxRotation));
                var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.Fov), AspectRatio, 1f, 1000f);
                var viewproj = view * proj;
                skyboxShader.SetUniformMat4("ViewProjection", ref viewproj);
                Renderer.Draw(skyboxShader, _skyboxRenderBuffer);
                GL.DepthFunc(DepthFunction.Less);
            }

            // Render (custom) skin
            {
                var skinShader = _shaders.GetShader("SkinShader");
                skinShader.Bind();
                skinShader.SetUniformMat4("u_ViewProjection", ref viewProjection);
                skinShader.SetUniform2("u_TexSize", new Vector2(TextureSize.Width, TextureSize.Height));

                skinTexture.Bind();

                GL.Enable(EnableCap.Texture2D); // Enable textures

                GL.DepthFunc(DepthFunction.Lequal); // Enable correct Z Drawings
                GL.DepthMask(true);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                GL.Enable(EnableCap.AlphaTest); // Enable transparent
                GL.AlphaFunc(AlphaFunction.Greater, 0.4f);

                GL.PolygonMode(MaterialFace.FrontAndBack, showWireFrame ? PolygonMode.Line : PolygonMode.Fill);

                Matrix4 modelMatrix = Matrix4.CreateTranslation(0f, 4f, 0f) * // <- model rotation pivot point
                    Matrix4.CreateFromAxisAngle(-Vector3.UnitX, MathHelper.DegreesToRadians(GlobalModelRotation.X)) *
                    Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(GlobalModelRotation.Y));

                if (ANIM.GetFlag(SkinAnimFlag.DINNERBONE))
                {
                    modelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-180f));
                }

                var legRightMatrix = Matrix4.Identity;
                var legLeftMatrix = Matrix4.Identity;
                var armRightMatrix = Matrix4.Identity;
                var armLeftMatrix = Matrix4.Identity;

                if (!ANIM.GetFlag(SkinAnimFlag.STATIC_ARMS))
                {
                    armRightMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(animationCurrentRotationAngle));
                    armLeftMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians((ANIM.GetFlag(SkinAnimFlag.SYNCED_ARMS) ? 1f : -1f) * animationCurrentRotationAngle));
                }


                if (!ANIM.GetFlag(SkinAnimFlag.STATIC_LEGS))
                {
                    legRightMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians((ANIM.GetFlag(SkinAnimFlag.SYNCED_LEGS) ? 1f : -1f) * animationCurrentRotationAngle));
                    legLeftMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(animationCurrentRotationAngle));
                }

                if (ANIM.GetFlag(SkinAnimFlag.ZOMBIE_ARMS))
                {
                    var rotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90f));
                    armRightMatrix = rotation;
                    armLeftMatrix = rotation;
                }

                if (ANIM.GetFlag(SkinAnimFlag.STATUE_OF_LIBERTY))
                {
                    armRightMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-180f));
                    armLeftMatrix = Matrix4.CreateRotationX(0f);
                }

                bool slimModel = ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);
                rightArm.Translation = rightArmOverlay.Translation = new Vector3(slimModel ? -4f : -5f, -2f, 0f);
                

                RenderBodyPart(skinShader, Matrix4.Identity, modelMatrix, "HEAD", "HEADWEAR");
                RenderBodyPart(skinShader, Matrix4.Identity, modelMatrix, "BODY", "JACKET");
                RenderBodyPart(skinShader, RightArmMatrix * armRightMatrix, modelMatrix, "ARM0", "SLEEVE0");
                RenderBodyPart(skinShader, LeftArmMatrix * armLeftMatrix, modelMatrix, "ARM1", "SLEEVE1");
                RenderBodyPart(skinShader, legRightMatrix, modelMatrix, "LEG0", "PANTS0");
                RenderBodyPart(skinShader, legLeftMatrix, modelMatrix, "LEG1", "PANTS1");

                // render lines
                if (ShowGuideLines)
                {
                    GL.DepthFunc(DepthFunction.Always);
                    var shader = _shaders.GetShader("LineShader");
                    shader.Bind();
                    shader.SetUniformMat4("ViewProjection", ref viewProjection);
                    shader.SetUniformMat4("Transform", ref modelMatrix);
                    Renderer.SetLineWidth(2.5f);
                    Renderer.Draw(shader, GetGuidelineDrawContext());
                    //GL.DepthFunc(DepthFunction.Less);
                    Renderer.SetLineWidth(1f);
                }
            }

#if USE_FRAMEBUFFER
            framebuffer.Unbind();
            GL.Disable(EnableCap.DepthTest);
            framebufferShader.Bind();
            framebufferVAO.Bind();
            framebufferTexture.Bind();

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            framebufferTexture.Unbind();
#endif
            SwapBuffers();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Rotate the model
            if (e.Button == MouseButtons.Left)
            {
                float deltaX = (Cursor.Position.X - CurrentMouseLocation.X) * 0.5f;
                float deltaY = (Cursor.Position.Y - CurrentMouseLocation.Y) * 0.5f;
                GlobalModelRotation += new Vector2(-deltaY, deltaX) * Camera.Distance * 0.015f;
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                CurrentMouseLocation = Cursor.Position;
                return;
            }
            // Move the model
            if (e.Button == MouseButtons.Right)
            {
                float deltaX = (Cursor.Position.X - CurrentMouseLocation.X) * 0.05f;
                float deltaY = (Cursor.Position.Y - CurrentMouseLocation.Y) * 0.05f;
                Camera.Pan(deltaX, deltaY);
                Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
                CurrentMouseLocation = Cursor.Position;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Camera.Distance -= e.Delta / System.Windows.Input.Mouse.MouseWheelDeltaForOneLine;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
            {
                if (!IsMouseHidden)
                {
                    IsMouseHidden = true;
                }
                PreviousMouseLocation = Cursor.Position;
                CurrentMouseLocation = Cursor.Position;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            ReleaseMouse();
            base.OnMouseUp(e);
        }

        private void RenderBodyPart(ShaderProgram shader, Matrix4 partMatrix, Matrix4 globalMatrix, params string[] additionalData)
        {
            foreach (var data in additionalData)
            {
                RenderPart(shader, data, partMatrix, globalMatrix);
            }
        }

        private void RenderPart(ShaderProgram shader, string name, Matrix4 partMatrix, Matrix4 globalMatrix)
        {
            CubeBatchMesh cubeMesh = meshStorage[name];
            Vector3 translation = cubeMesh.Translation;
            Vector3 pivot = cubeMesh.Pivot;
            float yOffset = GetOffset(name);
            translation.Y -= yOffset;
            pivot.Y += yOffset;
            Matrix4 transform = Matrix4.CreateScale(cubeMesh.Scale);
            transform *= Pivot(translation, pivot, partMatrix);
            transform *= globalMatrix;
            shader.SetUniformMat4("u_Transform", ref transform);
            cubeMesh.Draw(shader);
        }

        private static Matrix4 Pivot(Vector3 translation, Vector3 pivot, Matrix4 target)
        {
            var model = Matrix4.CreateTranslation(translation);
            model *= Matrix4.CreateTranslation(pivot);
            model *= target;
            model *= Matrix4.CreateTranslation(pivot).Inverted();
            return model;
        }
        
        private void AnimationTick(object sender, EventArgs e)
        {
            skyboxRotation += skyboxRotationStep;
            skyboxRotation %= 360f;
            animationCurrentRotationAngle += animationRotationStep;
            if (animationCurrentRotationAngle >= animationMaxAngleInDegrees || animationCurrentRotationAngle <= -animationMaxAngleInDegrees)
                animationRotationStep = -animationRotationStep;
        }

        private void ReInitialzeSkinData()
        {
            foreach (var mesh in meshStorage.Values)
            {
                mesh.ClearData();
            }

            InitializeSkinData();
            UpdateModelData();
            OnANIMUpdate();
        }

        private void UpdateModelData()
        {
            foreach (var item in ModelData)
            {
                AddCustomModelPart(item);
            }
        }

        private void reInitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReInitialzeSkinData();
            MakeCurrent();
            UploadMeshData();
        }

        private void guidelineModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Enum.IsDefined(typeof(GuidelineMode), ++guidelineMode))
            {
                guidelineMode = GuidelineMode.None;
            }
            guidelineModeToolStripMenuItem.Text = $"Guideline Mode: {guidelineMode}";
        }
    }
}