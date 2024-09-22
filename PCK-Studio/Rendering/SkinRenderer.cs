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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing.Imaging;
using System.IO;
using PckStudio.Rendering.Texture;
using PckStudio.Rendering.Shader;
using System.Linq;
using PckStudio.Internal.Skin;

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
        public Color GuideLineColor { get; set; }

        [Description("The Color used for highlighting selected cube")]
        [Category("Appearance")]
        public Color HighlightlingColor { get; set; } = Color.Aqua;

        public float MouseSensetivity { get; set; } = 0.01f;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                if (CenterOnSelect)
                    CenterSelectedObject();
            }
        }

        public bool CenterOnSelect { get; set; } = false;
        public bool ShowArmor { get; set; } = false;
        public bool Animate { get; set; } = true;
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

        [Description("Event that gets fired when the skin texture is changing")]
        [Category("Property Chnaged")]
        [Browsable(true)]
        public event EventHandler<TextureChangingEventArgs> TextureChanging
        {
            add => Events.AddHandler(nameof(TextureChanging), value);
            remove => Events.RemoveHandler(nameof(TextureChanging), value);
        }

        [Description("Event that gets fired when the cape texture is changing")]
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
        private int selectedIndex = -1;

        public Size TextureSize { get; private set; } = new Size(64, 64);
        public Vector2 TillingFactor => new Vector2(1f / TextureSize.Width, 1f / TextureSize.Height);
        private const float OverlayScale = 0.25f;

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

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool LockMousePosition { get; set; } = false;

        private Point PreviousMouseLocation;
        private Point CurrentMouseLocation;

        private VertexBufferLayout plainColorVertexBufferLayout;

        private SkinANIM _anim;
        private Image _skinImage;
        private Image _capeImage;
        private Texture2D skinTexture;
        private Texture2D capeTexture;
        private Texture2D armorTexture;

        private DrawContext _cubicalDrawContext;
        private DrawContext _skeletonDrawContext;
        private DrawContext _groundDrawContext;

        private DrawContext _skyboxRenderBuffer;
        private CubeTexture _skyboxTexture;

        private Dictionary<string, CubeMeshCollection> meshStorage;
        private Dictionary<string, CubeMeshCollection> offsetSpecificMeshStorage;
        
        private CubeMesh cape;
        
        private CubeMeshCollection head;
        private CubeMeshCollection body;
        private CubeMeshCollection rightArm;
        private CubeMeshCollection leftArm;
        private CubeMeshCollection rightLeg;
        private CubeMeshCollection leftLeg;

        private float animationCurrentRotationAngle;
        private float animationRotationSpeed = 16f;
        private float animationMaxAngleInDegrees = 5f;

        private bool showWireFrame = false;
        private bool autoInflateOverlayParts;

        private const float defaultArmRotation = 5f;

        private Matrix4 RightArmMatrix => Matrix4.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(defaultArmRotation));
        private Matrix4 LeftArmMatrix => Matrix4.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(-defaultArmRotation));

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

        private bool initialized = false;

        public SkinRenderer() : base()
        {
            InitializeSkinData();
            InitializeCapeData();
            meshStorage = new Dictionary<string, CubeMeshCollection>()
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

            ANIM ??= new SkinANIM(SkinAnimMask.RESOLUTION_64x64);
            ModelData = new ObservableCollection<SkinBOX>();
            ModelData.CollectionChanged += ModelData_CollectionChanged;
        }

        public void Initialize(bool inflateOverlayParts)
        {
            if (initialized)
                Debug.Fail("Already Initialized!");
            autoInflateOverlayParts = inflateOverlayParts;
            MakeCurrent();
            InitializeShaders();
            Renderer.SetClearColor(BackColor);
            GLErrorCheck();
            base.Initialize();
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
            ModelPartSpecifics.PositioningInfo headInfo = ModelPartSpecifics.GetPositioningInfo("HEAD");
            head ??= new CubeMeshCollection("Head", headInfo.Translation.ToOpenTKVector(), headInfo.Pivot.ToOpenTKVector())
            {
                FlipZMapping = true
            };
            head.AddNamed("DefaultHead", new(-4, -8, -4), new(8, 8, 8), new(0, 0));
            head.AddNamed("DefaultHeadOverlay", new(-4, -8, -4), new(8, 8, 8), new(32, 0), OverlayScale * 2);
            
            ModelPartSpecifics.PositioningInfo bodyInfo = ModelPartSpecifics.GetPositioningInfo("BODY");
            body ??= new CubeMeshCollection("Body", bodyInfo.Translation.ToOpenTKVector(), bodyInfo.Pivot.ToOpenTKVector());
            body.AddNamed("DefaultBody",new(-4, 0, -2), new(8, 12, 4), new(16, 16));
            body.AddNamed("DefaultBodyOverlay", new(-4, 0, -2), new(8, 12, 4), new(16, 32), OverlayScale);

            ModelPartSpecifics.PositioningInfo rightArmInfo = ModelPartSpecifics.GetPositioningInfo("ARM0");
            rightArm ??= new CubeMeshCollection("Right Arm", rightArmInfo.Translation.ToOpenTKVector(), rightArmInfo.Pivot.ToOpenTKVector());
            rightArm.AddNamed("DefaultRightArm",new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            rightArm.AddNamed("DefaultRightArmOverlay", new(-3, -2, -2), new(4, 12, 4), new(40, 32), OverlayScale);

            ModelPartSpecifics.PositioningInfo leftArmInfo = ModelPartSpecifics.GetPositioningInfo("ARM1");
            leftArm ??= new CubeMeshCollection("Left Arm", leftArmInfo.Translation.ToOpenTKVector(), leftArmInfo.Pivot.ToOpenTKVector());
            leftArm.AddNamed("DefaultLeftArm",new(-1, -2, -2), new(4, 12, 4), new(32, 48));
            leftArm.AddNamed("DefaultLeftArmOverlay", new(-1, -2, -2), new(4, 12, 4), new(48, 48), inflate: OverlayScale);

            ModelPartSpecifics.PositioningInfo rightLegInfo = ModelPartSpecifics.GetPositioningInfo("LEG0");
            rightLeg ??= new CubeMeshCollection("Right Leg", rightLegInfo.Translation.ToOpenTKVector(), rightLegInfo.Pivot.ToOpenTKVector());
            rightLeg.AddNamed("DefaultRightLeg",new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            rightLeg.AddNamed("DefaultRightLegOverlay", new(-2, 0, -2), new(4, 12, 4), new(0, 32), OverlayScale);

            ModelPartSpecifics.PositioningInfo leftLegInfo = ModelPartSpecifics.GetPositioningInfo("LEG1");
            leftLeg ??= new CubeMeshCollection("Left Leg", leftLegInfo.Translation.ToOpenTKVector(), leftLegInfo.Pivot.ToOpenTKVector());
            leftLeg.AddNamed("DefaultLeftLeg",new(-2, 0, -2), new(4, 12, 4), new(16, 48));
            leftLeg.AddNamed("DefaultLeftLegOverlay", new(-2, 0, -2), new(4, 12, 4), new(0, 48), OverlayScale);
        }

        private void InitializeCapeData()
        {
            cape ??= new CubeMesh(new Cube(new(-5, 0, -3), new(10, 16, 1), new(0, 0), 0f, false, false));
        }

        private void InitializeArmorData()
        {
            const float armorInflation = 0.75f;

            var helmet = new CubeMeshCollection("HELMET");
            helmet.Add(new(-4, -8, -4), new(8, 8, 8), new(0, 0), inflate: armorInflation);

            var chest = new CubeMeshCollection("CHEST");
            chest.Add(new(-4, 0, -2), new(8, 12, 4), new(16, 16), inflate: armorInflation + 0.01f);

            var shoulder0 = new CubeMeshCollection("SHOULDER0", rightArm.Translation, rightArm.Pivot);
            shoulder0.Add(new(-3, -2, -2), new(4, 12, 4), new(40, 16), inflate: armorInflation);

            var shoulder1 = new CubeMeshCollection("SHOULDER1", leftArm.Translation, leftArm.Pivot);
            shoulder1.Add(new(-1, -2, -2), new(4, 12, 4), new(40, 16), inflate: armorInflation, mirrorTexture: true);
            
            var waist = new CubeMeshCollection("WAIST");
            waist.Add(new(-4, 0, -2), new(8, 12, 4), new(16, 48), inflate: armorInflation);

            var pants0 = new CubeMeshCollection("PANTS0", rightLeg.Translation, rightLeg.Pivot);
            pants0.Add(new(-2, 0, -2), new(4, 12, 4), new(0, 48), inflate: armorInflation);
            
            var pants1 = new CubeMeshCollection("PANTS1", leftLeg.Translation, leftLeg.Pivot);
            pants1.Add(new(-2, 0, -2), new(4, 12, 4), new(0, 48), inflate: armorInflation, mirrorTexture: true);

            var boot0     = new CubeMeshCollection("BOOT0", rightLeg.Translation, rightLeg.Pivot);
            boot0.Add(new(-2, 0, -2), new(4, 12, 4), new(0, 16), inflate: armorInflation + 0.25f);
            
            var boot1     = new CubeMeshCollection("BOOT1", leftLeg.Translation, leftLeg.Pivot);
            boot1.Add(new(-2, 0, -2), new(4, 12, 4), new(0, 16), inflate: armorInflation + 0.25f, mirrorTexture: true);

            offsetSpecificMeshStorage = new Dictionary<string, CubeMeshCollection>
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

            
            plainColorVertexBufferLayout = new VertexBufferLayout();
            plainColorVertexBufferLayout.Add(ShaderDataType.Float3);
            plainColorVertexBufferLayout.Add(ShaderDataType.Float4);
            
            // Skin shader
            {
                var cubeShader = ShaderProgram.Create(
                    new ShaderSource(ShaderType.VertexShader, Resources.texturedCubeVertexShader),
                    new ShaderSource(ShaderType.FragmentShader, Resources.texturedCubeFragmentShader),
                    new ShaderSource(ShaderType.GeometryShader, Resources.texturedCubeGeometryShader)
                    );
                cubeShader.Bind();
                cubeShader.SetUniform1("u_Texture", 0);
                cubeShader.Validate();
                AddShader("CubeShader", cubeShader);
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
                AddShader("SkyboxShader", skyboxShader);

                _skyboxTexture = new CubeTexture(1);
                _skyboxTexture.InternalPixelFormat = PixelInternalFormat.Rgb8;
                _skyboxTexture.PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                _skyboxTexture.MinFilter = TextureMinFilter.Linear;
                _skyboxTexture.MagFilter = TextureMagFilter.Linear;

                _skyboxTexture.WrapS = TextureWrapMode.ClampToEdge;
                _skyboxTexture.WrapT = TextureWrapMode.ClampToEdge;
                _skyboxTexture.WrapR = TextureWrapMode.ClampToEdge;

                string customSkyboxFilepath = Path.Combine(Program.AppData, "skybox.png");
                using Image skyboxImage = File.Exists(customSkyboxFilepath)
                    ? Image.FromFile(customSkyboxFilepath)
                    : Resources.DefaultSkyTexture;
                _skyboxTexture.SetTexture(skyboxImage);
                GLErrorCheck();
            }

            // Plain color shader
            {
                var lineShader = ShaderProgram.Create(Resources.plainColorVertexShader, Resources.plainColorFragmentShader);
                lineShader.Bind();
                lineShader.SetUniform4("baseColor", Color.WhiteSmoke);
                lineShader.SetUniform1("intensity", 0.5f);
                lineShader.Validate();
                AddShader("PlainColorShader", lineShader);

                // Cubical draw context
                {
                    VertexArray lineVAO = new VertexArray();

                    void AddOutline(BoundingBox boundingBox, ref List<ColorVertex> vertices, ref List<int> indices)
                    {
                        int offset = vertices.Count;
                        vertices.AddRange(boundingBox.GetVertices());
                        indices.AddRange(BoundingBox.GetIndecies().Select(i => i + offset));
                    }

                    List<ColorVertex> vertices = new List<ColorVertex>(8 * 6);
                    List<int> indices = new List<int>(24 * 6);
                    AddOutline(head.GetCubeBoundingBox(0), ref vertices, ref indices);
                    AddOutline(body.GetCubeBoundingBox(0), ref vertices, ref indices);
                    AddOutline(rightArm.GetCubeBoundingBox(0), ref vertices, ref indices);
                    AddOutline(leftArm.GetCubeBoundingBox(0), ref vertices, ref indices);
                    AddOutline(rightLeg.GetCubeBoundingBox(0), ref vertices, ref indices);
                    AddOutline(leftLeg.GetCubeBoundingBox(0), ref vertices, ref indices);
                    VertexBuffer buffer = new VertexBuffer();
                    buffer.SetData(vertices.ToArray());
                    lineVAO.AddBuffer(buffer, plainColorVertexBufferLayout);
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
                        new ColorVertex(head.GetFaceCenter(0, Cube.Face.Top)),
                        new ColorVertex(bodyCenterBottom),
                    
                        new ColorVertex(rightArm.GetFaceCenter(0, Cube.Face.Bottom)),
                        new ColorVertex(rightArm.GetFaceCenter(0, Cube.Face.Top)),
                        new ColorVertex(rightArm.GetFaceCenter(0, Cube.Face.Top)),
                        new ColorVertex(leftArm.GetFaceCenter(0, Cube.Face.Top)),

                        new ColorVertex(leftArm.GetFaceCenter(0, Cube.Face.Bottom)),
                        new ColorVertex(leftArm.GetFaceCenter(0, Cube.Face.Top)),

                        new ColorVertex(rightLeg.GetFaceCenter(0, Cube.Face.Bottom)),
                        new ColorVertex(rightLeg.GetFaceCenter(0, Cube.Face.Top)),
                        new ColorVertex(rightLeg.GetFaceCenter(0, Cube.Face.Top)),
                        new ColorVertex(leftLeg.GetFaceCenter(0, Cube.Face.Top)),
                        
                        new ColorVertex(leftLeg.GetFaceCenter(0, Cube.Face.Bottom)),
                        new ColorVertex(leftLeg.GetFaceCenter(0, Cube.Face.Top)),
                    ];
                    VertexBuffer buffer = new VertexBuffer();
                    buffer.SetData(data);
                    lineVAO.AddBuffer(buffer, plainColorVertexBufferLayout);
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
                    planeVAO.AddBuffer(buffer, plainColorVertexBufferLayout);

                    _groundDrawContext = new DrawContext(planeVAO, buffer.GenIndexBuffer(), PrimitiveType.Quads);
                }

                GLErrorCheck();
            }

            InitializeDebugShaders();
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

        public void SetPartOffset(SkinPartOffset offset)
        {
            SetPartOffset(offset.Type, offset.Value);
        }

        public void SetPartOffset(string name, float value)
        {
            if (!SkinPartOffset.ValidModelOffsetTypes.Contains(name))
            {
                Trace.TraceWarning($"{name} is not a valid offset.");
                return;
            }
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

        internal IEnumerable<SkinPartOffset> GetOffsets()
        {
            foreach (KeyValuePair<string, CubeMeshCollection> mesh in meshStorage)
            {
                if (SkinPartOffset.ValidModelOffsetTypes.Contains(mesh.Key) && mesh.Value.Offset.Y != 0f)
                    yield return new SkinPartOffset(mesh.Key, mesh.Value.Offset.Y);
            }
            foreach (KeyValuePair<string, CubeMeshCollection> offsetmesh in offsetSpecificMeshStorage)
            {
                if (offsetmesh.Value.Offset.Y != 0f)
                    yield return new SkinPartOffset(offsetmesh.Key, offsetmesh.Value.Offset.Y);
            }
            yield break;
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
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    ReInitialzeSkinData();
                    goto default;
                case NotifyCollectionChangedAction.Move:
                    break;
                default:
                    break;
            }
        }

        private void AddCustomModelPart(SkinBOX skinBox)
        {
            if (!meshStorage.ContainsKey(skinBox.Type))
            {
                Trace.TraceWarning("[{0}@{1}] Invalid BOX Type: '{2}'", nameof(SkinRenderer), nameof(AddCustomModelPart), skinBox.Type);
                return;
            }

            CubeMeshCollection cubeMesh = meshStorage[skinBox.Type];
            cubeMesh.AddSkinBox(skinBox, autoInflateOverlayParts && skinBox.IsOverlayPart() ? skinBox.Type == "HEADWEAR" ? OverlayScale * 2 : OverlayScale : 0f);
        }

        private void OnANIMUpdate()
        {
            head.SetVisible(0, !ANIM.GetFlag(SkinAnimFlag.HEAD_DISABLED));
            head.SetVisible(1, !ANIM.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED));
            
            body.SetVisible(0, !ANIM.GetFlag(SkinAnimFlag.BODY_DISABLED));
            rightArm.SetVisible(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED));
            leftArm.SetVisible(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED));
            rightLeg.SetVisible(0, !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED));
            leftLeg.SetVisible(0, !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED));

            bool slim = ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);

            head.FlipZMapping = true;
            if (slim || ANIM.GetFlag(SkinAnimFlag.RESOLUTION_64x64))
            {
                TextureSize = new Size(64, 64);
                body.SetVisible(1, !ANIM.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED));
                rightArm.SetVisible(1, !ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED));
                leftArm.SetVisible(1, !ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED));
                rightLeg.SetVisible(1, !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED));
                leftLeg.SetVisible(1, !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED));

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
            
            body.SetVisible(1, false);
            head.FlipZMapping = false;

            rightArm.ReplaceCube(0, new(-3, -2, -2), new(4, 12, 4), new(40, 16));
            rightArm.SetVisible(1, false);
            
            leftArm.ReplaceCube(0, new(-1, -2, -2), new(4, 12, 4), new(40, 16), mirrorTexture: true);
            leftArm.SetVisible(1, false);

            rightLeg.ReplaceCube(0, new(-2, 0, -2), new(4, 12, 4), new(0, 16));
            rightLeg.SetVisible(1, false);
            leftLeg.ReplaceCube (0, new(-2, 0, -2), new(4, 12, 4), new(0, 16), mirrorTexture: true);
            leftLeg.SetVisible(1, false);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
#if DEBUG
                case Keys.Escape:
                    ReleaseMouse();
                    var point = new Point(Parent.Location.X + Location.X, Parent.Location.Y + Location.Y);
                    debugContextMenuStrip1.Show(point);
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
                case Keys.C:
                    CenterSelectedObject();
                    break;
            }
            return base.ProcessDialogKey(keyData);
        }

        internal void CenterSelectedObject()
        {
            if (ModelData.IndexInRange(SelectedIndex))
            {
                SkinBOX skinBox = ModelData[SelectedIndex];
                if (!meshStorage.ContainsKey(skinBox.Type))
                {
                    Trace.TraceWarning("[{0}@{1}] Invalid BOX Type: '{2}'", nameof(SkinRenderer), nameof(CenterSelectedObject), skinBox.Type);
                    return;
                }

                CubeMeshCollection cubeMesh = meshStorage[skinBox.Type];
                Vector3 center = skinBox.ToCube().Center;
                Matrix4 camMat = (Matrix4.CreateTranslation(cubeMesh.Translation) * Matrix4.CreateTranslation(center + cubeMesh.Offset) * Matrix4.CreateScale(-1, 1, 1));
                Vector3 camPos = camMat.ExtractTranslation();
                Camera.FocalPoint = camPos;
                Camera.Distance = skinBox.Size.Length() * 2;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (DesignMode)
            {
                return;
            }

            MakeCurrent();

            FramebufferBegin();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.Enable(EnableCap.DepthTest); // Enable correct Z Drawings
            GL.Enable(EnableCap.LineSmooth);
            Matrix4 viewProjection = Camera.GetViewProjection();

            // Render Skybox
            {
                GL.DepthFunc(DepthFunction.Lequal);
                GL.DepthMask(false);
                ShaderProgram skyboxShader = GetShader("SkyboxShader");
                skyboxShader.Bind();
                _skyboxTexture.Bind();

                Matrix4 view = new Matrix4(new Matrix3(Matrix4.LookAt(Camera.WorldPosition, Camera.WorldPosition + Camera.Orientation, Camera.Up)))
                    * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Camera.Yaw))
                    * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Camera.Pitch));
                Matrix4 viewproj = view * Camera.GetProjection();
                skyboxShader.SetUniformMat4("ViewProjection", ref viewproj);
                Renderer.Draw(skyboxShader, _skyboxRenderBuffer);
                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);
            }
            
            ShaderProgram lineShader = GetShader("PlainColorShader");

            // Render (custom) skin
            {
                GL.Enable(EnableCap.Texture2D); // Enable textures

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                GL.Enable(EnableCap.AlphaTest); // Enable transparent
                GL.AlphaFunc(AlphaFunction.Greater, 0.0f);
                GL.DepthFunc(DepthFunction.Lequal);
                
                if (showWireFrame)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

                Matrix4 transform = Matrix4.Identity;

                ShaderProgram cubeShader = GetShader("CubeShader");
                cubeShader.Bind();
                cubeShader.SetUniformMat4("u_ViewProjection", ref viewProjection);
                cubeShader.SetUniform2("u_TexSize", TextureSize);

                skinTexture.Bind();

                Matrix4 legRightMatrix = Matrix4.Identity;
                Matrix4 legLeftMatrix = Matrix4.Identity;
                Matrix4 armRightMatrix = Matrix4.Identity;
                Matrix4 armLeftMatrix = Matrix4.Identity;
                if (Animate)
                {
                    if (ANIM.GetFlag(SkinAnimFlag.DINNERBONE))
                    {
                        transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-180f));
                        transform = transform.Pivoted(head.GetFaceCenter(0, Cube.Face.Top), Vector3.UnitY * 12f);
                    }

                    if (!ANIM.GetFlag(SkinAnimFlag.STATIC_ARMS))
                    {
                        armRightMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(animationCurrentRotationAngle));
                        armLeftMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians((ANIM.GetFlag(SkinAnimFlag.SYNCED_ARMS) ? 1f : -1f) * animationCurrentRotationAngle));
                        if (ANIM.GetFlag(SkinAnimFlag.STATUE_OF_LIBERTY))
                        {
                            armRightMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-180f));
                            armLeftMatrix = Matrix4.CreateRotationX(0f);
                        }
                        if (ANIM.GetFlag(SkinAnimFlag.ZOMBIE_ARMS))
                        {
                            var rotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90f));
                            armRightMatrix = rotation;
                            armLeftMatrix = rotation;
                        }
                    }

                    if (!ANIM.GetFlag(SkinAnimFlag.STATIC_LEGS))
                    {
                        legRightMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians((ANIM.GetFlag(SkinAnimFlag.SYNCED_LEGS) ? 1f : -1f) * animationCurrentRotationAngle));
                        legLeftMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(animationCurrentRotationAngle));
                    }
                    armRightMatrix = RightArmMatrix * armRightMatrix;
                    armLeftMatrix = LeftArmMatrix * armLeftMatrix;
                }

                RenderBodyPart(cubeShader, Matrix4.Identity, transform, "HEAD", "HEADWEAR");
                RenderBodyPart(cubeShader, Matrix4.Identity, transform, "BODY", "JACKET");
                RenderBodyPart(cubeShader, armRightMatrix, transform, "ARM0", "SLEEVE0");
                RenderBodyPart(cubeShader, armLeftMatrix, transform, "ARM1", "SLEEVE1");
                RenderBodyPart(cubeShader, legRightMatrix, transform, "LEG0", "PANTS0");
                RenderBodyPart(cubeShader, legLeftMatrix, transform, "LEG1", "PANTS1");

                if (_capeImage is not null)
                {
                    cubeShader.SetUniform2("u_TexSize", new Vector2(64, 32));
                    capeTexture.Bind();
                    // Defines minimum Angle(in Degrees) of the cape
                    float capeMinimumRotationAngle = 7.5f;
                    // Controls how much of an angle is applied
                    float capeRotationFactor = 0.4f;
                    // Low value = slow movement
                    float capeRotationSpeed = 0.02f;
                    float capeRotation = ((float)MathHelper.RadiansToDegrees(Math.Sin(Math.Abs(animationCurrentRotationAngle) * capeRotationSpeed) * capeRotationFactor)) + capeMinimumRotationAngle;
                    Matrix4 partMatrix = 
                        Matrix4.CreateRotationY(MathHelper.DegreesToRadians(180f)) *
                        Matrix4.CreateRotationX(MathHelper.DegreesToRadians(capeRotation));
                    RenderPart(cubeShader, cape, partMatrix, transform);
                }

                // Armor rendering
                if (ShowArmor && !ANIM.GetFlag(SkinAnimFlag.ALL_ARMOR_DISABLED))
                {
                    armorTexture.Bind();
                    cubeShader.SetUniform2("u_TexSize", new Vector2(64, 64));
                    if (!ANIM.GetFlag(SkinAnimFlag.HEAD_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_HEAD_ARMOR))
                        RenderPart(cubeShader, offsetSpecificMeshStorage["HELMET"], Matrix4.Identity, transform);
                    
                    if (!ANIM.GetFlag(SkinAnimFlag.BODY_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_BODY_ARMOR))
                        RenderPart(cubeShader, offsetSpecificMeshStorage["CHEST"], Matrix4.Identity, transform);
                    
                    if (!ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_RIGHT_ARM_ARMOR))
                        RenderPart(cubeShader, offsetSpecificMeshStorage["SHOULDER0"], RightArmMatrix * armRightMatrix, transform);
                    
                    if (!ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_LEFT_ARM_ARMOR))
                        RenderPart(cubeShader, offsetSpecificMeshStorage["SHOULDER1"], LeftArmMatrix * armLeftMatrix, transform);

                    bool showRightLegArmor = !ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_RIGHT_LEG_ARMOR);
                    if (showRightLegArmor)
                    {
                        RenderPart(cubeShader, offsetSpecificMeshStorage["PANTS0"], legRightMatrix, transform);
                        RenderPart(cubeShader, offsetSpecificMeshStorage["BOOT0"], legRightMatrix, transform);
                    }

                    bool showLeftLegArmor = !ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED) || ANIM.GetFlag(SkinAnimFlag.FORCE_LEFT_LEG_ARMOR);
                    if (showLeftLegArmor)
                    {
                        RenderPart(cubeShader, offsetSpecificMeshStorage["PANTS1"], legLeftMatrix, transform);
                        RenderPart(cubeShader, offsetSpecificMeshStorage["BOOT1"], legLeftMatrix, transform);
                    }
                    
                    if (showRightLegArmor && showLeftLegArmor)
                        RenderPart(cubeShader, offsetSpecificMeshStorage["WAIST"], Matrix4.Identity, transform);
                }

                if (showWireFrame)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                if (ShowGuideLines)
                {
                    GL.DepthFunc(DepthFunction.Always);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    lineShader.Bind();
                    lineShader.SetUniformMat4("ViewProjection", ref viewProjection);
                    lineShader.SetUniformMat4("Transform", ref transform);
                    lineShader.SetUniform1("intensity", 1f);
                    lineShader.SetUniform4("baseColor", GuideLineColor);
                    Renderer.SetLineWidth(2.5f);
                    Renderer.Draw(lineShader, GetGuidelineDrawContext());
                    Renderer.SetLineWidth(1f);
                    GL.DepthFunc(DepthFunction.Less);
                }

                if (ModelData.IndexInRange(SelectedIndex))
                {
                    SkinBOX box = ModelData[SelectedIndex];

                    float inflate = autoInflateOverlayParts && box.IsOverlayPart() ? box.Type == "HEADWEAR" ? OverlayScale * 2 : OverlayScale : 0f;
                    Cube cube = box.ToCube(inflate);

                    BoundingBox cubeBoundingBox = cube.GetBoundingBox();

                    if (meshStorage.ContainsKey(box.Type))
                    {
                        CubeMeshCollection cubeMesh = meshStorage[box.Type];

                        Matrix4 GetGroupTransform(string type)
                        {
                            switch (type)
                            {
                                case "ARM0":
                                case "SLEEVE0":
                                    return armRightMatrix;
                                case "ARM1":
                                case "SLEEVE1":
                                    return armLeftMatrix;
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

                        Matrix4 bbTransform = GetGroupTransform(box.Type);
                        bbTransform *= cubeMesh.Transform;
                        bbTransform *= transform;
                        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.SrcColor);
                        DrawBoundingBox(bbTransform, cubeBoundingBox, HighlightlingColor);
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
            FramebufferEnd();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            float mouseSensetivity = LockMousePosition ? MouseSensetivity : 64f * 3 * (1f/56f) * MouseSensetivity;

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
            ReleaseMouse();
            base.OnMouseUp(e);
        }

        private void ReleaseMouse()
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

        private void RenderBodyPart(ShaderProgram shader, Matrix4 partsMatrix, Matrix4 globalMatrix, params string[] partNames)
        {
            foreach (var partName in partNames)
            {
                RenderPart(shader, meshStorage[partName], partsMatrix, globalMatrix);
            }
        }

        private void RenderPart<T>(ShaderProgram shader, GenericMesh<T> mesh, Matrix4 partMatrix, Matrix4 globalMatrix) where T : struct
        {
            Matrix4 transform = partMatrix * mesh.Transform * globalMatrix;
            shader.SetUniformMat4("u_Transform", ref transform);
            DrawMesh(mesh, shader);
        }

        protected override void OnUpdate(object sender, TimeSpan timestep)
        {
            base.OnUpdate(sender, timestep);
            double delta = timestep.TotalSeconds;
            if (!Animate)
                return;

            animationCurrentRotationAngle += (float)delta * animationRotationSpeed;
            animationCurrentRotationAngle = MathHelper.Clamp(animationCurrentRotationAngle, -animationMaxAngleInDegrees, animationMaxAngleInDegrees);
            if (animationCurrentRotationAngle >= animationMaxAngleInDegrees || animationCurrentRotationAngle <= -animationMaxAngleInDegrees)
                animationRotationSpeed = -animationRotationSpeed;
        }

        private void ReInitialzeSkinData()
        {
            foreach (CubeMeshCollection mesh in meshStorage.Values)
            {
                mesh.Clear();
            }

            InitializeSkinData();
            UpdateModelData();
            OnANIMUpdate();
        }

        private void UpdateModelData()
        {
            foreach (SkinBOX item in ModelData)
            {
                AddCustomModelPart(item);
            }
        }

        [Conditional("DEBUG")]
        private void InitializeDebugShaders()
        {
#if DEBUG
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
            ShaderProgram colorShader = GetShader("PlainColorShader");
            Matrix4 viewProjection = Camera.GetViewProjection();
            colorShader.SetUniformMat4("ViewProjection", ref viewProjection);
            if (d_showFocalPoint)
            {
                GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.DepthFunc(DepthFunction.Always);
                GL.DepthMask(false);
                GL.Enable(EnableCap.PointSmooth);
                colorShader.Bind();
                Matrix4 transform = Matrix4.CreateTranslation(Camera.FocalPoint).Inverted();
                colorShader.SetUniformMat4("Transform", ref transform);
                colorShader.SetUniform1("intensity", 0.75f);
                colorShader.SetUniform4("baseColor", Color.DeepPink);
                GL.PointSize(5f);
                Renderer.Draw(colorShader, d_debugPointDrawContext);
                GL.PointSize(1f);
                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }
            if (d_showDirectionArrows)
            {
                GL.BlendFunc(BlendingFactor.DstAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.DepthFunc(DepthFunction.Always);
                GL.DepthMask(false);
                GL.Enable(EnableCap.LineSmooth);
                colorShader.Bind();

                Matrix4 transform = Matrix4.CreateScale(1, -1, -1);
                transform *= Matrix4.CreateTranslation(Vector3.Zero);
                transform *= Matrix4.CreateScale(Camera.Distance / 4f).Inverted();
                transform.Invert();
                colorShader.SetUniformMat4("Transform", ref transform);
                colorShader.SetUniform1("intensity", 0.75f);
                colorShader.SetUniform4("baseColor", Color.White);

                Renderer.SetLineWidth(2f);

                Renderer.Draw(colorShader, d_debugLineDrawContext);

                Renderer.SetLineWidth(1f);

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
            debugContextMenuStrip1 = new ContextMenuStrip(this.components);
            guidelineModeToolStripMenuItem = new ToolStripMenuItem();
            debugContextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            debugContextMenuStrip1.Items.AddRange(new ToolStripItem[] {
            reToolStripMenuItem,
            guidelineModeToolStripMenuItem});
            debugContextMenuStrip1.Name = "contextMenuStrip1";
            debugContextMenuStrip1.Size = new Size(159, 48);
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
        private ToolStripMenuItem reToolStripMenuItem;
        private ContextMenuStrip debugContextMenuStrip1;
        private ToolStripMenuItem guidelineModeToolStripMenuItem;

        private void reInitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReInitialzeSkinData();
            MakeCurrent();
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