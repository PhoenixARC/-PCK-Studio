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
        [Description("The current skin texture")]
        [Category("Appearance")]
        public Image Texture
        {
            get => _skinImage;
            set
            {
                var args = new TextureChangingEventArgs(value);
                Events[nameof(TextureChanging)]?.DynamicInvoke(this, args);
                OnTextureChanging(this, args);
                if (!args.Cancel)
                {
                    _skinImage = value;
                    TextureSize = value.Width == value.Height ? new Size(64, 64) : new Size(64, 32);
                }
            }
        }

        [Description("The current cape texture")]
        [Category("Appearance")]
        public Image CapeTexture
        {
            get => _capeImage;
            set
            {
                var args = new TextureChangingEventArgs(value);
                Events[nameof(CapeTextureChanging)]?.DynamicInvoke(this, args);
                OnCapeTextureChanging(this, args);
                if (!args.Cancel)
                {
                    _capeImage = value;
                }
            }
        }

        [Description("The Color used for outlines")]
        [Category("Appearance")]
        public Color OutlineColor { get; set; }

        public float MouseSensetivity { get; set; } = 0.01f;
        public int SelectedIndex { get; set; } = -1;

        public bool ClampModel { get; set; } = false;
        public bool ShowArmor { get; set; } = false;
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

        [Description("Event that gets fired when the Texture is changing")]
        [Category("Property Chnaged")]
        [Browsable(true)]
        public event EventHandler<TextureChangingEventArgs> CapeTextureChanging
        {
            add => Events.AddHandler(nameof(CapeTextureChanging), value);
            remove => Events.RemoveHandler(nameof(CapeTextureChanging), value);
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

        private enum GuidelineMode
        {
            None = -1,
            Cubical,
            Skeleton
        };

        private GuidelineMode guidelineMode { get; set; } = GuidelineMode.None;

        public Size TextureSize { get; private set; } = new Size(64, 64);
        public Vector2 TillingFactor => new Vector2(1f / TextureSize.Width, 1f / TextureSize.Height);
        private const float OverlayScale = 0.5f;

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
        private Image _skinImage;
        private Image _capeImage;
        private Texture2D skinTexture;
        private Texture2D capeTexture;
        private Texture2D armorTexture;

#if USE_FRAMEBUFFER
        private FrameBuffer framebuffer;
        private Texture2D framebufferTexture;
        private ShaderProgram framebufferShader;
        private VertexArray framebufferVAO;
        private int framebufferRenderBuffer;
#endif

        private DrawContext _cubicalDrawContext;
        private DrawContext _skeletonDrawContext;
        private DrawContext _groundDrawContext;

        private DrawContext _skyboxRenderBuffer;
        private CubeTexture _skyboxTexture;

        private Dictionary<string, CubeGroupMesh> meshStorage;
        private Dictionary<string, CubeGroupMesh> offsetSpecificMeshStorage;
        
        private CubeGroupMesh cape;
        
        private CubeGroupMesh head;
        private CubeGroupMesh body;
        private CubeGroupMesh rightArm;
        private CubeGroupMesh leftArm;
        private CubeGroupMesh rightLeg;
        private CubeGroupMesh leftLeg;

        private float animationCurrentRotationAngle;
        private float animationRotationStep = 0.5f;
        private float animationMaxAngleInDegrees = 5f;

        private bool showWireFrame = false;
        private bool autoInflateOverlayParts;

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
            InitializeCapeData();
            meshStorage = new Dictionary<string, CubeGroupMesh>()
            {
                { "HEAD", head },
                { "BODY", body },
                { "ARM0", rightArm },
                { "ARM1", leftArm },
                { "LEG0", rightLeg },
                { "LEG1", leftLeg },

                { "HEADWEAR", head },
                { "JACKET"  , body },
                { "SLEEVE0" , rightArm },
                { "SLEEVE1" , leftArm },
                { "PANTS0"  , rightLeg },
                { "PANTS1"  , leftLeg },

            };
            InitializeArmorData();
            InitializeCamera();
            InitializeComponent();
            InitializeDebug();

            _shaders = new ShaderLibrary();
            ANIM ??= new SkinANIM(SkinAnimMask.RESOLUTION_64x64);
            OnTimerTick = AnimationTick;
            ModelData = new ObservableCollection<SkinBOX>();
            ModelData.CollectionChanged += ModelData_CollectionChanged;
        }

        public void InitializeGL(bool inflateOverlayParts)
        {
            if (initialized)
                Debug.Fail("Already Initialized!");
            autoInflateOverlayParts = inflateOverlayParts;
            MakeCurrent();
            InitializeShaders();
            InitializeFramebuffer();
            Renderer.SetClearColor(BackColor);
            var layout = CubeGroupMesh.GetLayout();
            foreach (var item in meshStorage)
            {
                item.Value.Initialize(layout);
            }
            UploadMeshData();
            foreach (var cubeMesh in offsetSpecificMeshStorage?.Values)
            {
                cubeMesh.Initialize(layout);
                cubeMesh.UploadData();
            }
            cape.Initialize(layout);
            cape.UploadData();
            GLErrorCheck();
            base.Init();
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
            head ??= new CubeGroupMesh("Head", flipZMapping: true);
            head.AddCube(new(-4, -8, -4), new(8, 8, 8), new(0, 0));
            head.AddCube(new(-4, -8, -4), new(8, 8, 8), new(32, 0), OverlayScale);
            
            body ??= new CubeGroupMesh("Body");
            body.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 16));
            body.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 32), OverlayScale);
            
            rightArm ??= new CubeGroupMesh("Right Arm");
            rightArm.Pivot = new Vector3(4f, 2f, 0f);
            rightArm.Translation = new Vector3(-5f, -2f, 0f);
            rightArm.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            rightArm.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 32), OverlayScale);
            
            leftArm ??= new CubeGroupMesh("Left Arm");
            leftArm.Pivot = new Vector3(-4f, 2f, 0f);
            leftArm.Translation = new Vector3(5f, -2f, 0f);
            leftArm.AddCube(new(-1, -2, -2), new(4, 12, 4), new(32, 48));
            leftArm.AddCube(new(-1, -2, -2), new(4, 12, 4), new(48, 48), inflate: OverlayScale);

            rightLeg ??= new CubeGroupMesh("Right Leg");
            rightLeg.Pivot = new Vector3(0f, 12f, 0f);
            rightLeg.Translation = new Vector3(-2f, -12f, 0f);
            rightLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            rightLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 32), OverlayScale);

            leftLeg ??= new CubeGroupMesh("Left Leg");
            leftLeg.Pivot = new Vector3(0f, 12f, 0f);
            leftLeg.Translation = new Vector3(2f, -12f, 0f);
            leftLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(16, 48));
            leftLeg.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 48), OverlayScale);
        }

        private void InitializeCapeData()
        {
            cape ??= new CubeGroupMesh("Cape");
            cape.AddCube(new(-5, 0, -3), new(10, 16, 1), new(0, 0));
        }

        private void InitializeArmorData()
        {
            const float armorInflation = 0.75f;

            var helmet    = new CubeGroupMesh("HELMET");
            helmet.AddCube(new(-4, -8, -4), new(8, 8, 8), new(0, 0), inflate: armorInflation);

            var chest = new CubeGroupMesh("CHEST");
            chest.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 16), inflate: armorInflation + 0.01f);

            var shoulder0 = new CubeGroupMesh("SHOULDER0");
            shoulder0.Pivot = new Vector3(4f, 2f, 0f);
            shoulder0.Translation = new Vector3(-5f, -2f, 0f);
            shoulder0.AddCube(new(-3, -2, -2), new(4, 12, 4), new(40, 16), inflate: armorInflation);

            var shoulder1 = new CubeGroupMesh("SHOULDER1");
            shoulder1.Pivot = new Vector3(-4f, 2f, 0f);
            shoulder1.Translation = new Vector3(5f, -2f, 0f);
            shoulder1.AddCube(new(-1, -2, -2), new(4, 12, 4), new(40, 16), inflate: armorInflation, mirrorTexture: true);
            
            var waist = new CubeGroupMesh("WAIST");
            waist.AddCube(new(-4, 0, -2), new(8, 12, 4), new(16, 48), inflate: armorInflation);

            var pants0 = new CubeGroupMesh("PANTS0");
            pants0.Pivot = new Vector3(0f, 12f, 0f);
            pants0.Translation = new Vector3(-2f, -12f, 0f);
            pants0.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 48), inflate: armorInflation);
            
            var pants1 = new CubeGroupMesh("PANTS1");
            pants1.Pivot = new Vector3(0f, 12f, 0f);
            pants1.Translation = new Vector3(2f, -12f, 0f);
            pants1.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 48), inflate: armorInflation, mirrorTexture: true);

            var boot0     = new CubeGroupMesh("BOOT0");
            boot0.Pivot = new Vector3(0f, 12f, 0f);
            boot0.Translation = new Vector3(-2f, -12f, 0f);
            boot0.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 16), inflate: armorInflation + 0.25f);
            
            var boot1     = new CubeGroupMesh("BOOT1");
            boot1.Pivot = new Vector3(0f, 12f, 0f);
            boot1.Translation = new Vector3(2f, -12f, 0f);
            boot1.AddCube(new(-2, 0, -2), new(4, 12, 4), new(0, 16), inflate: armorInflation + 0.25f, mirrorTexture: true);

            offsetSpecificMeshStorage = new Dictionary<string, CubeGroupMesh>
            {
                { helmet.Name, helmet },
                { chest.Name, chest },
                { shoulder0.Name, shoulder0 },
                { shoulder1.Name, shoulder1 },
                { waist.Name, waist },
                { pants0.Name, pants0 },
                { pants1.Name, pants1 },
                { boot0.Name, boot0 },
                { boot1.Name, boot1 }
            };

            //// TODO
            //{ "TOOL0"    , new CubeGroupMesh("TOOL0") },
            //{ "TOOL1"    , new CubeGroupMesh("TOOL1") },
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

                armorTexture = new Texture2D(0);
                armorTexture.PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                armorTexture.InternalPixelFormat = PixelInternalFormat.Rgba8;
                armorTexture.MinFilter = TextureMinFilter.Nearest;
                armorTexture.MagFilter = TextureMagFilter.Nearest;
                armorTexture.WrapS = TextureWrapMode.Repeat;
                armorTexture.WrapT = TextureWrapMode.Repeat;
                armorTexture.SetTexture(Resources.armor);
                GLErrorCheck();

                capeTexture = new Texture2D(0);
                capeTexture.PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                capeTexture.InternalPixelFormat = PixelInternalFormat.Rgba8;
                capeTexture.MinFilter = TextureMinFilter.Nearest;
                capeTexture.MagFilter = TextureMagFilter.Nearest;
                capeTexture.WrapS = TextureWrapMode.Repeat;
                capeTexture.WrapT = TextureWrapMode.Repeat;
                GLErrorCheck();

                skinTexture = new Texture2D(0);
                skinTexture.PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                skinTexture.InternalPixelFormat = PixelInternalFormat.Rgba8;
                skinTexture.MinFilter = TextureMinFilter.Nearest;
                skinTexture.MagFilter = TextureMagFilter.Nearest;
                skinTexture.WrapS = TextureWrapMode.Repeat;
                skinTexture.WrapT = TextureWrapMode.Repeat;
                
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

                string customSkyboxFilepath = Path.Combine(Program.AppData, "skybox.png");
                Image skyboxImage = File.Exists(customSkyboxFilepath)
                    ? Image.FromFile(customSkyboxFilepath)
                    : Resources.DefaultSkyTexture;

                _skyboxTexture = new CubeTexture(1);
                _skyboxTexture.InternalPixelFormat = PixelInternalFormat.Rgb8;
                _skyboxTexture.PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                _skyboxTexture.MinFilter = TextureMinFilter.Linear;
                _skyboxTexture.MagFilter = TextureMagFilter.Linear;

                _skyboxTexture.WrapS = TextureWrapMode.ClampToEdge;
                _skyboxTexture.WrapT = TextureWrapMode.ClampToEdge;
                _skyboxTexture.WrapR = TextureWrapMode.ClampToEdge;
                _skyboxTexture.SetTexture(skyboxImage);
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
                    Vector3 bodyCenterTop = body.GetFaceCenter(0, Cube.Face.Top);
                    Vector3 bodyCenterBottom = body.GetFaceCenter(0, Cube.Face.Bottom);
                    ColorVertex[] data = [
                        new ColorVertex(head.GetFaceCenter(0, Cube.Face.Top), lineColor),
                        new ColorVertex(bodyCenterBottom, lineColor),
                    
                        new ColorVertex(rightArm.GetFaceCenter(0, Cube.Face.Bottom), lineColor),
                        new ColorVertex(rightArm.GetFaceCenter(0, Cube.Face.Top), lineColor),
                        new ColorVertex(rightArm.GetFaceCenter(0, Cube.Face.Top), lineColor),
                        new ColorVertex(leftArm.GetFaceCenter(0, Cube.Face.Top), lineColor),

                        new ColorVertex(leftArm.GetFaceCenter(0, Cube.Face.Bottom), lineColor),
                        new ColorVertex(leftArm.GetFaceCenter(0, Cube.Face.Top), lineColor),

                        new ColorVertex(rightLeg.GetFaceCenter(0, Cube.Face.Bottom), lineColor),
                        new ColorVertex(rightLeg.GetFaceCenter(0, Cube.Face.Top), lineColor),
                        new ColorVertex(rightLeg.GetFaceCenter(0, Cube.Face.Top), lineColor),
                        new ColorVertex(leftLeg.GetFaceCenter(0, Cube.Face.Top), lineColor),
                        
                        new ColorVertex(leftLeg.GetFaceCenter(0, Cube.Face.Bottom), lineColor),
                        new ColorVertex(leftLeg.GetFaceCenter(0, Cube.Face.Top), lineColor),
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
                d_debugDrawContext = new DrawContext(vao, debugVBO.GenIndexBuffer(), PrimitiveType.Points);
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
            skinTexture.SetTexture(e.NewTexture);
            GLErrorCheck();
        }

        protected virtual void OnCapeTextureChanging(object sender, TextureChangingEventArgs e)
        {
            if (e.NewTexture is null)
                e.Cancel = true;
            
            if (e.Cancel)
                return;
            capeTexture.SetTexture(e.NewTexture);
            GLErrorCheck();
        }

        [Conditional("USE_FRAMEBUFFER")]
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

            framebufferRenderBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, framebufferRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.Width, Size.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, framebufferRenderBuffer);

            framebufferVAO = new VertexArray();
            VertexBuffer vertexBuffer = new VertexBuffer();
            vertexBuffer.SetData(rectVertices);
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.Add(ShaderDataType.Float4);
            framebufferVAO.AddBuffer(vertexBuffer, layout);
            framebuffer.CheckStatus();

            if (framebuffer.Status != FramebufferErrorCode.FramebufferComplete)
            {
                Debug.Fail($"Framebuffer status: '{framebuffer.Status}'");
            }

            framebuffer.Unbind();
#endif
        }

        private void UpdateMesh(string name)
        {
            if (!meshStorage.ContainsKey(name))
                return;
            meshStorage[name]?.UploadData();
        }

        private void UploadMeshData()
        {
            foreach (var cubeMeshName in meshStorage?.Keys)
            {
                UpdateMesh(cubeMeshName);
            }
        }

        public void SetPartOffset(SkinPartOffset offset)
        {
            SetPartOffset(offset.Type, offset.Value);
        }

        public void SetPartOffset(string name, float value)
        {
            bool offsetSpecific = offsetSpecificMeshStorage.ContainsKey(name);
            if (!meshStorage.ContainsKey(name) && !offsetSpecific)
            {
                Trace.TraceError($"[{nameof(SetPartOffset)}]: '{name}' is not inside {nameof(meshStorage)} or {nameof(offsetSpecificMeshStorage)}");
                return;
            }
            if (offsetSpecific)
            {
                offsetSpecificMeshStorage[name].Offset = Vector3.UnitY * value;
                return;
            }
            meshStorage[name].Offset = Vector3.UnitY * value;
        }

        internal void ResetOffsets()
        {
            foreach (var key in meshStorage.Keys.ToArray())
            {
                meshStorage[key].Offset = Vector3.Zero;
            }
            foreach (var key in offsetSpecificMeshStorage.Keys.ToArray())
            {
                offsetSpecificMeshStorage[key].Offset = Vector3.Zero;
        }
        }

        private void ModelData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO: dont re-initialize everytime..
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems[0] is SkinBOX addedBox)
                    {
                        AddCustomModelPart(addedBox);
                        UpdateMesh(addedBox.Type);
                    }
                    break;
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
            cubeMesh.AddSkinBox(skinBox, autoInflateOverlayParts && skinBox.IsOverlayPart() ? OverlayScale : 0f);
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
            head.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED));
            
            body.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.BODY_DISABLED));
            rightArm.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED));
            leftArm.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED));
            rightLeg.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED));
            leftLeg.SetEnabled(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED));

            bool slim = ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);
            if (slim || ANIM.GetFlag(SkinAnimFlag.RESOLUTION_64x64))
            {
                TextureSize = new Size(64, 64);
                body.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED));
                rightArm.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED));
                leftArm.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED));
                rightLeg.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED));
                leftLeg.SetEnabled(1, !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED));

                int slimValue = slim ? 3 : 4;
                rightArm.ReplaceCube(0, new(slim ? -2 : -3, -2, -2), new(slimValue, 12, 4), new(40, 16));
                rightArm.ReplaceCube(1, new(slim ? -2 : -3, -2, -2), new(slimValue, 12, 4), new(40, 32), inflate: OverlayScale);

                leftArm.ReplaceCube(0, new(-1, -2, -2), new(slimValue, 12, 4), new(32, 48));
                leftArm.ReplaceCube(1, new(-1, -2, -2), new(slimValue, 12, 4), new(48, 48), inflate: OverlayScale);

                rightLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
                leftLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(16, 48));
                return;
            }
            
            TextureSize = new Size(64, 32);
            
            body.SetEnabled(1, false);

            rightArm.ReplaceCube(0, new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            rightArm.SetEnabled(1, false);
            
            leftArm.ReplaceCube(0, new(-1, -2, -2), new(4, 12, 4), new(40, 16), mirrorTexture: true);
            leftArm.SetEnabled(1, false);

            rightLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            rightLeg.SetEnabled(1, false);
            leftLeg.ReplaceCube (0, new(-2, 0, -2), new(4, 12, 4), new(0, 16), mirrorTexture: true);
            leftLeg.SetEnabled(1, false);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
#if DEBUG
                case Keys.Escape:
                    ReleaseMouse();
                    var point = new Point(Parent.Location.X + Location.X, Parent.Location.Y + Location.Y);
                    contextMenuStrip1.Show(point);
                    return true;
#endif
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
        [Conditional("USE_FRAMEBUFFER")]
        private void SetFramebufferSize(Size size)
        {
            MakeCurrent();
            if (framebuffer is not null)
            {
                framebuffer.Bind();

                framebufferTexture.Bind();
                framebufferTexture.SetSize(size);

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, framebufferRenderBuffer);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, size.Width, size.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, framebufferRenderBuffer);

                FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                if (status != FramebufferErrorCode.FramebufferComplete)
                {
                    Debug.Fail($"Framebuffer status: '{framebuffer.Status}'");
                }
                framebuffer.Unbind();
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
                GL.DepthFunc(DepthFunction.Lequal);

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
                    transform = Pivot(head.GetFaceCenter(0, Cube.Face.Top), Vector3.UnitY * 12f, transform * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-180f)));
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

                RenderBodyPart(skinShader, Matrix4.Identity, transform, "HEAD", "HEADWEAR");
                RenderBodyPart(skinShader, Matrix4.Identity, transform, "BODY", "JACKET");
                RenderBodyPart(skinShader, RightArmMatrix * armRightMatrix, transform, "ARM0", "SLEEVE0");
                RenderBodyPart(skinShader, LeftArmMatrix * armLeftMatrix, transform, "ARM1", "SLEEVE1");
                RenderBodyPart(skinShader, legRightMatrix, transform, "LEG0", "PANTS0");
                RenderBodyPart(skinShader, legLeftMatrix, transform, "LEG1", "PANTS1");

                if (_capeImage is not null)
                {
                    skinShader.SetUniform2("u_TexSize", new Vector2(64, 32));
                    capeTexture.Bind();
                    Matrix4 partMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(180f)) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians((float)Math.Sin(Math.Abs(animationCurrentRotationAngle) * 0.25f) * 10f));
                    RenderPart(skinShader, cape, partMatrix, transform);
                }

                // Armor rendering
                if (ShowArmor)
                {
                    armorTexture.Bind();
                    //skinShader.SetUniform4("u_Color", Color.FromArgb(0xff << 24 | Color.White.ToArgb() - _outlineColor.ToArgb()));
                    skinShader.SetUniform2("u_TexSize", new Vector2(64, 64));
                    if (!ANIM.GetFlag(SkinAnimFlag.HEAD_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_HEAD_ARMOR))
                        RenderPart(skinShader, offsetSpecificMeshStorage["HELMET"], Matrix4.Identity, transform);
                    
                    if (!ANIM.GetFlag(SkinAnimFlag.BODY_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_BODY_ARMOR))
                        RenderPart(skinShader, offsetSpecificMeshStorage["CHEST"], Matrix4.Identity, transform);
                    
                    if (!ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_RIGHT_ARM_ARMOR))
                        RenderPart(skinShader, offsetSpecificMeshStorage["SHOULDER0"], RightArmMatrix * armRightMatrix, transform);
                    
                    if (!ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_LEFT_ARM_ARMOR))
                        RenderPart(skinShader, offsetSpecificMeshStorage["SHOULDER1"], LeftArmMatrix * armLeftMatrix, transform);

                    bool showRightLegArmor = !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_RIGHT_LEG_ARMOR);
                    if (showRightLegArmor)
                    {
                        RenderPart(skinShader, offsetSpecificMeshStorage["PANTS0"], legRightMatrix, transform);
                        RenderPart(skinShader, offsetSpecificMeshStorage["BOOT0"], legRightMatrix, transform);
                    }

                    bool showLeftLegArmor = !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_LEFT_LEG_ARMOR);
                    if (showLeftLegArmor)
                    {
                        RenderPart(skinShader, offsetSpecificMeshStorage["PANTS1"], legLeftMatrix, transform);
                        RenderPart(skinShader, offsetSpecificMeshStorage["BOOT1"], legLeftMatrix, transform);
                    }
                    
                    if (showRightLegArmor && showLeftLegArmor)
                        RenderPart(skinShader, offsetSpecificMeshStorage["WAIST"], Matrix4.Identity, transform);
                }

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
                    lineShader.SetUniform4("baseColor", OutlineColor);
                    Renderer.SetLineWidth(2.5f);
                    Renderer.Draw(lineShader, GetGuidelineDrawContext());
                    Renderer.SetLineWidth(1f);
                    GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GL.DepthFunc(DepthFunction.Less);
                }

                if (ModelData.IndexInRange(SelectedIndex))
                {
                    SkinBOX box = ModelData[SelectedIndex];
                    var cubeBoundingBox = Cube.FromSkinBox(box).GetBoundingBox();

                    if (meshStorage.ContainsKey(box.Type))
                    {
                        var cubeMesh = meshStorage[box.Type];

                        Matrix4 GetGroupTransform(string type)
                        {
                            switch (type)
                            {
                                case "ARM0":
                                case "SLEEVE0":
                                    return RightArmMatrix * armRightMatrix;
                                case "ARM1":
                                case "SLEEVE1":
                                    return LeftArmMatrix * armLeftMatrix;
                                case "LEG0":
                                case "PANTS0":
                                    return legRightMatrix;
                                case "LEG1":
                                case "PANTS1":
                                    return legLeftMatrix;
                                default:
                                    return Matrix4.Identity;
                            }
                        }

                        transform *= GetGroupTransform(box.Type);
                        Vector3 translation = cubeMesh.Translation - cubeMesh.Offset;
                        Vector3 pivot = cubeMesh.Pivot + cubeMesh.Offset;
                        transform = Pivot(translation, pivot, transform);
                    GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                        DrawBoundingBox(transform, cubeBoundingBox, OutlineColor);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                }
            }
            }


            // Ground plane
            {
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.AlphaTest); // Enable transparent
                GL.AlphaFunc(AlphaFunction.Always, 0.0f);
                GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                lineShader.Bind();
                lineShader.SetUniformMat4("ViewProjection", ref viewProjection);
                lineShader.SetUniform1("intensity", 0.5f);
                lineShader.SetUniform4("baseColor", Color.AntiqueWhite);
                Matrix4 transform = Matrix4.CreateScale(25f) * Matrix4.CreateTranslation(new Vector3(0f, -24.1f, 0f));
                lineShader.SetUniformMat4("Transform", ref transform);
                Renderer.Draw(lineShader, _groundDrawContext);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }

            // Debug
            RenderDebug();

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
                RenderPart(shader, meshStorage[partName], partsMatrix, globalMatrix);
            }
        }

        private void RenderPart(ShaderProgram shader, CubeGroupMesh cubeMesh, Matrix4 partMatrix, Matrix4 globalMatrix)
        {
            Vector3 translation = cubeMesh.Translation - cubeMesh.Offset;
            Vector3 pivot = cubeMesh.Pivot + cubeMesh.Offset;
            Matrix4 transform = Pivot(translation, pivot, partMatrix);
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

        [Conditional("DEBUG")]
        private void RenderDebug()
        {
#if DEBUG
            var colorShader = _shaders.GetShader("PlainColorShader");
            if (d_showFocalPoint)
            {
                GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.DepthFunc(DepthFunction.Always);
                GL.DepthMask(false);
                GL.Enable(EnableCap.PointSmooth);
                colorShader.Bind();
                var viewProjection = Camera.GetViewProjection();
                var transform = Matrix4.CreateTranslation(Camera.FocalPoint).Inverted();
                colorShader.SetUniformMat4("Transform", ref transform);
                colorShader.SetUniformMat4("ViewProjection", ref viewProjection);
                colorShader.SetUniform1("intensity", 0.75f);
                colorShader.SetUniform4("baseColor", Color.DeepPink);
                GL.PointSize(5f);
                Renderer.Draw(colorShader, d_debugDrawContext);
                GL.PointSize(1f);
                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }
            d_debugLabel.Text = Camera.ToString();
#endif
        }

        [Conditional("DEBUG")]
        private void InitializeDebug()
        {
#if DEBUG
            reToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1 = new ContextMenuStrip(this.components);
            guidelineModeToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
            reToolStripMenuItem,
            guidelineModeToolStripMenuItem});
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(159, 48);
            // 
            // reToolStripMenuItem
            // 
            reToolStripMenuItem.Name = "reToolStripMenuItem";
            reToolStripMenuItem.Size = new Size(158, 22);
            reToolStripMenuItem.Text = "Re-Init";
            reToolStripMenuItem.Click += new EventHandler(this.reInitToolStripMenuItem_Click);
            // 
            // guidelineModeToolStripMenuItem
            // 
            guidelineModeToolStripMenuItem.Name = "guidelineModeToolStripMenuItem";
            guidelineModeToolStripMenuItem.Size = new Size(158, 22);
            guidelineModeToolStripMenuItem.Text = "Guideline Mode";
            guidelineModeToolStripMenuItem.Click += new EventHandler(this.guidelineModeToolStripMenuItem_Click);
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
            contextMenuStrip1.Items.Add(debugCameraToolStripMenuItem);

            var debugShowFocalPointToolStripMenuItem = new ToolStripMenuItem("Show Camera Focal point");
            debugShowFocalPointToolStripMenuItem.CheckOnClick = true;
            debugShowFocalPointToolStripMenuItem.Click += (s, e) => d_showFocalPoint = debugShowFocalPointToolStripMenuItem.Checked;
            contextMenuStrip1.Items.Add(debugShowFocalPointToolStripMenuItem);

            Controls.Add(d_debugLabel);

            this.contextMenuStrip1.ResumeLayout(false);
#endif
        }

#if DEBUG
        private bool d_showFocalPoint;
        private DrawContext d_debugDrawContext;
        private Label d_debugLabel;
        private ToolStripMenuItem reToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem guidelineModeToolStripMenuItem;

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
#endif
    }
}