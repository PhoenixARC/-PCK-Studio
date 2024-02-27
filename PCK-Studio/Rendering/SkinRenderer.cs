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

        [Description("The Color used for outlines")]
        [Category("Appearance")]
        public Color OutlineColor
        {
            get => _outlineColor;
            set
            {
                if (value == _outlineColor)
                    return;
                _outlineColor = value;
            }
        }

        public float MouseSensetivity { get; set; } = 0.01f;

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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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


        private Color _outlineColor;

        private enum GuidelineMode
        {
            None = -1,
            Cubical,
            Skeleton
        };

        private GuidelineMode guidelineMode { get; set; } = GuidelineMode.None;

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
        private DrawContext _groundDrawContext;

        private DrawContext _skyboxRenderBuffer;
        private CubeTexture _skyboxTexture;
        private float skyboxRotation = 0f;
        private float skyboxRotationStep = 0.5f;

        private Dictionary<string, CubeGroupMesh> meshStorage;
        
        private CubeGroupMesh head;
        private CubeGroupMesh body;
        private CubeGroupMesh rightArm;
        private CubeGroupMesh leftArm;
        private CubeGroupMesh rightLeg;
        private CubeGroupMesh leftLeg;

        private CubeGroupMesh headOverlay;
        private CubeGroupMesh bodyOverlay;
        private CubeGroupMesh rightArmOverlay;
        private CubeGroupMesh leftArmOverlay;
        private CubeGroupMesh rightLegOverlay;
        private CubeGroupMesh leftLegOverlay;

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

#if DEBUG
        private DrawContext debugDrawContext;
#endif

        public SkinRenderer() : base()
        {
            InitializeSkinData();
            meshStorage = new Dictionary<string, CubeGroupMesh>()
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

                { "HELMET"   , new CubeGroupMesh("HELMET") },
                { "BODYARMOR", new CubeGroupMesh("BODYARMOR") },
                
                { "BELT"     , new CubeGroupMesh("BELT") },
                
                { "ARMARMOR0", new CubeGroupMesh("ARMARMOR0") },
                { "ARMARMOR1", new CubeGroupMesh("ARMARMOR1") },
                
                { "BOOT0"    , new CubeGroupMesh("BOOT0") },
                { "BOOT1"    , new CubeGroupMesh("BOOT1") },

                { "TOOL0"    , new CubeGroupMesh("TOOL0") },
                { "TOOL1"    , new CubeGroupMesh("TOOL1") },
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
            Renderer.SetClearColor(BackColor);
            foreach (var item in meshStorage)
            {
                item.Value.Initialize();
            }
            UploadMeshData();
            GLErrorCheck();
            initialized = true;
        }

        private const float DefaultCameraDistance = 64f;
        private void InitializeCamera()
        {
            Camera.Distance = DefaultCameraDistance;
            Camera.FocalPoint = head.GetCenter(0);
        }

        private void InitializeSkinData()
        {
            head ??= new CubeGroupMesh("Head");
            head.AddCube(new(-4, -8, -4), new(8, 8, 8), new(0, 0), flipZMapping: true);
            
            headOverlay ??= new CubeGroupMesh("Head Overlay", OverlayScale);
            headOverlay.AddCube(new(-4, -8, -4), new(8, 8, 8), new(32, 0), flipZMapping: true);

            body ??= new CubeGroupMesh("Body");
            body.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 16));
            
            bodyOverlay ??= new CubeGroupMesh("Body Overlay", OverlayScale);
            bodyOverlay.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 32));

            rightArm ??= new CubeGroupMesh("Right Arm");
            rightArm.Pivot = new Vector3(4f, 2f, 0f);
            rightArm.Translation = new Vector3(-5f, -2f, 0f);
            rightArm.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            
            rightArmOverlay ??= new CubeGroupMesh("Right Arm Overlay", OverlayScale);
            rightArmOverlay.Pivot = new Vector3(4f, 2f, 0f);
            rightArmOverlay.Translation = new Vector3(-5f, -2f, 0f);
            rightArmOverlay.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 32));
            
            leftArm ??= new CubeGroupMesh("Left Arm");
            leftArm.Pivot = new Vector3(-4f, 2f, 0f);
            leftArm.Translation = new Vector3(5f, -2f, 0f);
            leftArm.AddCube(new(-1, -2, -2), new(4, 12, 4), new(32, 48));

            leftArmOverlay ??= new CubeGroupMesh("Left Arm Overlay", OverlayScale);
            leftArmOverlay.Pivot = new Vector3(-4f, 2f, 0f);
            leftArmOverlay.Translation = new Vector3(5f, -2f, 0f);
            leftArmOverlay.AddCube(new(-1, -2, -2), new(4, 12, 4), new(48, 48));

            rightLeg ??= new CubeGroupMesh("Right Leg");
            rightLeg.Pivot = new Vector3(0f, 12f, 0f);
            rightLeg.Translation = new Vector3(-2f, -12f, 0f);
            rightLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 16));

            rightLegOverlay ??= new CubeGroupMesh("Right Leg Overlay", OverlayScale);
            rightLegOverlay.Pivot = new Vector3(0f, 12f, 0f);
            rightLegOverlay.Translation = new Vector3(-2f, -12f, 0f);
            rightLegOverlay.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 32));

            leftLeg ??= new CubeGroupMesh("Left Leg");
            leftLeg.Pivot = new Vector3(0f, 12f, 0f);
            leftLeg.Translation = new Vector3(2f, -12f, 0f);
            leftLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(16, 48));

            leftLegOverlay ??= new CubeGroupMesh("Left Leg Overlay", OverlayScale);
            leftLegOverlay.Pivot = new Vector3(0f, 12f, 0f);
            leftLegOverlay.Translation = new Vector3(2f, -12f, 0f);
            leftLegOverlay.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 48));
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

                GLErrorCheck();
            }

            // Skybox shader
            {
                var skyboxVAO = new VertexArray();
                var skyboxVBO = new VertexBuffer();
                skyboxVBO.SetData(cubeVertices);
                var vboLayout = new VertexBufferLayout();
                vboLayout.Add(ShaderDataType.Float3);
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

                var skyboxShader = ShaderProgram.Create(Resources.skyboxVertexShader, Resources.skyboxFragmentShader);
                skyboxShader.Bind();
                skyboxShader.SetUniform1("skybox", 1);
                skyboxShader.SetUniform1("brightness", 0.8f);
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
            // Plain color shader
            {
                var lineShader = ShaderProgram.Create(Resources.plainColorVertexShader, Resources.plainColorFragmentShader);
                lineShader.Bind();
                lineShader.SetUniform4("baseColor", Color.WhiteSmoke);
                lineShader.SetUniform1("intensity", 0.5f);
                lineShader.Validate();
                _shaders.AddShader("PlainColorShader", lineShader);

                Color lineColor = Color.White;

                // Cubical draw context
                {
                    VertexArray lineVAO = new VertexArray();

                    void AddOutline(OutlineDefinition outline, ref List<ColorVertex> vertices, ref List<int> indices)
                    {
                        int offset = vertices.Count;
                        vertices.AddRange(outline.verticies.Select(pos => new ColorVertex(pos, lineColor)));
                        indices.AddRange(outline.indicies.Select(i => i + offset));
                    }

                    List<ColorVertex> vertices = new List<ColorVertex>(8 * 6);
                    List<int> indices = new List<int>(24 * 6);
                    AddOutline(head.GetOutline(0), ref vertices, ref indices);
                    AddOutline(body.GetOutline(0), ref vertices, ref indices);
                    AddOutline(rightArm.GetOutline(0), ref vertices, ref indices);
                    AddOutline(leftArm.GetOutline(0), ref vertices, ref indices);
                    AddOutline(rightLeg.GetOutline(0), ref vertices, ref indices);
                    AddOutline(leftLeg.GetOutline(0), ref vertices, ref indices);
                    VertexBuffer buffer = new VertexBuffer();
                    buffer.SetData(vertices.ToArray());
                    VertexBufferLayout layout = new VertexBufferLayout();
                    layout.Add(ShaderDataType.Float3);
                    layout.Add(ShaderDataType.Float4);
                    lineVAO.AddBuffer(buffer, layout);
                    lineVAO.Bind();

                    _cubicalDrawContext = new DrawContext(lineVAO, IndexBuffer.Create(indices.ToArray()), PrimitiveType.Lines);
                }

                GLErrorCheck();

                // Skeleton draw context
                {
                    VertexArray lineVAO = new VertexArray();
                    Vector3 bodyCenterTop = body.GetFaceCenter(0, CubeData.CubeFace.Top);
                    Vector3 bodyCenterBottom = body.GetFaceCenter(0, CubeData.CubeFace.Bottom);
                    ColorVertex[] data = [
                        new ColorVertex(head.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new ColorVertex(bodyCenterBottom, lineColor),
                    
                        new ColorVertex(rightArm.GetFaceCenter(0, CubeData.CubeFace.Bottom), lineColor),
                        new ColorVertex(rightArm.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new ColorVertex(rightArm.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new ColorVertex(leftArm.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),

                        new ColorVertex(leftArm.GetFaceCenter(0, CubeData.CubeFace.Bottom), lineColor),
                        new ColorVertex(leftArm.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),

                        new ColorVertex(rightLeg.GetFaceCenter(0, CubeData.CubeFace.Bottom), lineColor),
                        new ColorVertex(rightLeg.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new ColorVertex(rightLeg.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        new ColorVertex(leftLeg.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                        
                        new ColorVertex(leftLeg.GetFaceCenter(0, CubeData.CubeFace.Bottom), lineColor),
                        new ColorVertex(leftLeg.GetFaceCenter(0, CubeData.CubeFace.Top), lineColor),
                    ];
                    VertexBuffer buffer = new VertexBuffer();
                    buffer.SetData(data);
                    VertexBufferLayout layout = new VertexBufferLayout();
                    layout.Add(ShaderDataType.Float3);
                    layout.Add(ShaderDataType.Float4);
                    lineVAO.AddBuffer(buffer, layout);
                    lineVAO.Bind();

                    _skeletonDrawContext = new DrawContext(lineVAO, buffer.GenIndexBuffer(), PrimitiveType.Lines);
                }

                // Ground plane draw context
                {
                    Vector3 center = Vector3.Zero;
                    Color planeColor = Color.CadetBlue;

                    ColorVertex[] vertices = [
                        new ColorVertex(new Vector3(center.X + 1f, 0f, center.Z + 1f), planeColor),
                        new ColorVertex(new Vector3(center.X - 1f, 0f, center.Z + 1f), planeColor),
                        new ColorVertex(new Vector3(center.X - 1f, 0f, center.Z - 1f), planeColor),
                        new ColorVertex(new Vector3(center.X + 1f, 0f, center.Z - 1f), planeColor),
                        ];
                    var planeVAO = new VertexArray();
                    VertexBuffer buffer = new VertexBuffer();
                    buffer.SetData(vertices);

                    VertexBufferLayout layout = new VertexBufferLayout();
                    layout.Add(ShaderDataType.Float3);
                    layout.Add(ShaderDataType.Float4);
                    planeVAO.AddBuffer(buffer, layout);

                    _groundDrawContext = new DrawContext(planeVAO, buffer.GenIndexBuffer(), PrimitiveType.Quads);
                }

                GLErrorCheck();
            }

#if DEBUG
            // Debug render
            {
                ColorVertex[] vertices = [
                    new ColorVertex(Vector3.Zero, Color.White)
                ];
                VertexArray vao = new VertexArray();
                var debugVBO = new VertexBuffer();
                debugVBO.SetData(vertices);
                VertexBufferLayout layout = new VertexBufferLayout();
                layout.Add(ShaderDataType.Float3);
                layout.Add(ShaderDataType.Float4);
                vao.AddBuffer(debugVBO, layout);
                debugDrawContext = new DrawContext(vao, debugVBO.GenIndexBuffer(), PrimitiveType.Points);
            }
#endif

        }

        private DrawContext GetGuidelineDrawContext()
        {
            return guidelineMode == GuidelineMode.Skeleton ? _skeletonDrawContext : _cubicalDrawContext;
        }

        protected virtual void OnTextureChanging(object sender, TextureChangingEventArgs e)
        {
            if (e.NewTexture is null)
                e.Cancel = true;
            
            if (e.Cancel)
                return;
            skinTexture.LoadImageData(e.NewTexture);
            GLErrorCheck();
            
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
            if (!meshStorage.ContainsKey(name))
            {
                Trace.TraceError($"[{nameof(SetPartOffset)}]: '{name}' is not inside {nameof(meshStorage)}");
                return;
            }
            meshStorage[name].Offset = Vector3.UnitY * value;
        }

        internal void ResetOffsets()
        {
            foreach (var key in meshStorage.Keys.ToList())
            {
                meshStorage[key].Offset = Vector3.Zero;
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

            CubeGroupMesh cubeMesh = meshStorage[skinBox.Type];
            cubeMesh.AddSkinBox(skinBox);
        }

        [Conditional("DEBUG")]
        private void GLErrorCheck()
        {
            var error = GL.GetError();
            Debug.Assert(error == ErrorCode.NoError, error.ToString());
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
                    Camera.Distance = DefaultCameraDistance;
                    Camera.FocalPoint = head.GetCenter(0);
                    Camera.Yaw = 0f;
                    Camera.Pitch = 0f;
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
                GL.DepthMask(false);
                var skyboxShader = _shaders.GetShader("SkyboxShader");
                skyboxShader.Bind();
                _skyboxTexture.Bind();

                var view = new Matrix4(new Matrix3(Matrix4.LookAt(Camera.WorldPosition, Camera.WorldPosition + Camera.Orientation, Camera.Up)))
                    * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Camera.Yaw))
                    * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Camera.Pitch));
                var viewproj = view * Camera.GetProjection();
                skyboxShader.SetUniformMat4("ViewProjection", ref viewproj);
                Renderer.Draw(skyboxShader, _skyboxRenderBuffer);
                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);
            }
            
            ShaderProgram lineShader = _shaders.GetShader("PlainColorShader");

            // Render (custom) skin
            {
                GL.Enable(EnableCap.Texture2D); // Enable textures

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                GL.Enable(EnableCap.AlphaTest); // Enable transparent
                GL.AlphaFunc(AlphaFunction.Greater, 0.4f);

                if (showWireFrame)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

                Matrix4 transform = Matrix4.CreateTranslation(0f, 0f, 0f);

                var skinShader = _shaders.GetShader("SkinShader");
                skinShader.Bind();
                skinShader.SetUniformMat4("u_ViewProjection", ref viewProjection);
                skinShader.SetUniform2("u_TexSize", new Vector2(TextureSize.Width, TextureSize.Height));

                skinTexture.Bind();

                if (ANIM.GetFlag(SkinAnimFlag.DINNERBONE))
                {
                    transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-180f));
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

                if (!ANIM.GetFlag(SkinAnimFlag.STATIC_LEGS))
                {
                    legRightMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians((ANIM.GetFlag(SkinAnimFlag.SYNCED_LEGS) ? 1f : -1f) * animationCurrentRotationAngle));
                    legLeftMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(animationCurrentRotationAngle));
                }

                // TODO: only apply Translation to the base arm
                bool slimModel = ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);
                rightArm.Translation = rightArmOverlay.Translation = new Vector3(slimModel ? -4f : -5f, -2f, 0f);

                RenderBodyPart(skinShader, Matrix4.Identity, transform, "HEAD", "HEADWEAR");
                RenderBodyPart(skinShader, Matrix4.Identity, transform, "BODY", "JACKET");
                RenderBodyPart(skinShader, RightArmMatrix * armRightMatrix, transform, "ARM0", "SLEEVE0");
                RenderBodyPart(skinShader, LeftArmMatrix * armLeftMatrix, transform, "ARM1", "SLEEVE1");
                RenderBodyPart(skinShader, legRightMatrix, transform, "LEG0", "PANTS0");
                RenderBodyPart(skinShader, legLeftMatrix, transform, "LEG1", "PANTS1");

                if (showWireFrame)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                if (ShowGuideLines)
                {
                    GL.DepthFunc(DepthFunction.Always);
                    GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                    lineShader.Bind();
                    lineShader.SetUniformMat4("ViewProjection", ref viewProjection);
                    lineShader.SetUniformMat4("Transform", ref transform);
                    lineShader.SetUniform1("intensity", 1f);
                    lineShader.SetUniform4("baseColor", _outlineColor);
                    Renderer.SetLineWidth(2.5f);
                    Renderer.Draw(lineShader, GetGuidelineDrawContext());
                    Renderer.SetLineWidth(1f);
                    GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GL.DepthFunc(DepthFunction.Less);
                }
            }

            // Ground plane
            {
                GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                lineShader.Bind();
                lineShader.SetUniformMat4("ViewProjection", ref viewProjection);
                lineShader.SetUniform1("intensity", 0.5f);
                lineShader.SetUniform4("baseColor", Color.AntiqueWhite);
                Matrix4 transform = Matrix4.CreateScale(25f) * Matrix4.CreateTranslation(new Vector3(0f, -24f, 0f));
                lineShader.SetUniformMat4("Transform", ref transform);
                Renderer.Draw(lineShader, _groundDrawContext);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }

#if DEBUG
            // Debug
            {
                GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.DepthFunc(DepthFunction.Always);
                GL.DepthMask(false);
                GL.Enable(EnableCap.PointSmooth);
                lineShader.Bind();
                var transform = Matrix4.CreateTranslation(Camera.FocalPoint).Inverted();
                lineShader.SetUniformMat4("Transform", ref transform);
                lineShader.SetUniformMat4("ViewProjection", ref viewProjection);
                lineShader.SetUniform1("intensity", 0.75f);
                lineShader.SetUniform4("baseColor", Color.DeepPink);
                GL.PointSize(5f);
                Renderer.Draw(lineShader, debugDrawContext);
                GL.PointSize(1f);
                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }
#endif

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

#if DEBUG
            debugLabel.Text = Camera.ToString();
#endif
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            float deltaX = (Cursor.Position.X - CurrentMouseLocation.X) * MouseSensetivity;
            float deltaY = (Cursor.Position.Y - CurrentMouseLocation.Y) * MouseSensetivity;

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
                    Cursor.Position = new Point((int)Math.Round(Screen.PrimaryScreen.Bounds.Width / 2d), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height / 2d));
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
            ReleaseMouse();
            base.OnMouseUp(e);
        }

        private void ReleaseMouse()
        {
            if (IsMouseHidden)
            {
                IsMouseHidden = false;
                Cursor.Position = PreviousMouseLocation;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
            {
                IsMouseHidden = true;
                CurrentMouseLocation = PreviousMouseLocation = Cursor.Position;
            }
        }

        private void RenderBodyPart(ShaderProgram shader, Matrix4 partsMatrix, Matrix4 globalMatrix, params string[] partNames)
        {
            foreach (var partName in partNames)
            {
                RenderPart(shader, partName, partsMatrix, globalMatrix);
            }
        }

        private void RenderPart(ShaderProgram shader, string name, Matrix4 partMatrix, Matrix4 globalMatrix)
        {
            CubeGroupMesh cubeMesh = meshStorage[name];
            Vector3 translation = cubeMesh.Translation - cubeMesh.Offset;
            Vector3 pivot = cubeMesh.Pivot + cubeMesh.Offset;
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