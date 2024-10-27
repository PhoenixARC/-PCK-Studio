using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMI.Formats.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PckStudio.Extensions;
using PckStudio.Internal;
using PckStudio.Internal.Json;
using PckStudio.Properties;
using PckStudio.Rendering.Shader;
using PckStudio.Rendering.Texture;

namespace PckStudio.Rendering
{
    internal partial class ModelRenderer : SceneViewport
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image Texture
        {
            get => _modelTexture;
            set
            {
                var args = new TextureChangingEventArgs(value);
                Events[nameof(ModelTextureChanging)]?.DynamicInvoke(this, args);
                OnModelTextureChanging(this, args);
                if (!args.Cancel)
                {
                    _modelTexture = value;
                }
            }
        }

        [Description("Event that gets fired when the skin texture is changing")]
        [Category("Property Chnaged")]
        [Browsable(true)]
        public event EventHandler<TextureChangingEventArgs> ModelTextureChanging
        {
            add => Events.AddHandler(nameof(ModelTextureChanging), value);
            remove => Events.RemoveHandler(nameof(ModelTextureChanging), value);
        }

        public bool RenderModelBounds { get; set; }
        public string CurrentModelName => _currentModelName;

        private BoundingBox _maxBounds;
        private string _currentModelName;
        private Image _modelTexture;
        private Texture2D _modelRenderTexture;
        private List<CubeMeshCollection> _rootCollection;
        private struct HighlightInfo
        {
            public static readonly HighlightInfo Empty = new HighlightInfo(Vector3.Zero, Vector3.Zero, BoundingBox.Empty);
            public bool IsEmpty => BoundingBox.Volume.LengthSquared <= 0f;
            public BoundingBox BoundingBox { get; }
            public Vector3 Pivot { get; }
            public Vector3 Rotation { get; }

            public HighlightInfo(System.Numerics.Vector3 pivot, System.Numerics.Vector3 rotation, BoundingBox boundingBox)
                : this(pivot.ToOpenTKVector(),rotation.ToOpenTKVector(), boundingBox)
            {
            }

            public HighlightInfo(Vector3 pivot, Vector3 rotation, BoundingBox boundingBox)
            {
                Pivot = pivot;
                Rotation = rotation;
                BoundingBox = boundingBox;
            }
        }
        private HighlightInfo _highlightingInfo;

        public ModelRenderer() : base(fov: 60f)
        {
            InitializeComponent();
            _rootCollection = new List<CubeMeshCollection>(5);
            if (DesignMode)
                return;
            InitializeShaders();
        }

        private void InitializeShaders()
        {
            Debug.Assert(Context.IsCurrent);

            // render texture
            {
                _modelRenderTexture = new Texture2D();
                _modelRenderTexture.PixelFormat = PixelFormat.Bgra;
                _modelRenderTexture.InternalPixelFormat = PixelInternalFormat.Rgba8;
                _modelRenderTexture.MinFilter = TextureMinFilter.Nearest;
                _modelRenderTexture.MagFilter = TextureMagFilter.Nearest;
                _modelRenderTexture.WrapS = TextureWrapMode.Repeat;
                _modelRenderTexture.WrapT = TextureWrapMode.Repeat;
            }
            
            // cubeShader
            {
                var cubeShader = ShaderProgram.Create(
                        new ShaderSource(ShaderType.VertexShader, Resources.texturedCubeVertexShader),
                        new ShaderSource(ShaderType.FragmentShader, Resources.texturedCubeFragmentShader),
                        new ShaderSource(ShaderType.GeometryShader, Resources.texturedCubeGeometryShader)
                        );
                cubeShader.Bind();
                cubeShader.SetUniform1("Texture", 0);
                cubeShader.Validate();
                AddShader("CubeShader", cubeShader);
            }
        }

        public void LoadModel(Model model)
        {
            ResetHighlight();
            _rootCollection?.Clear();

            _maxBounds = model.GetParts()
                .SelectMany(p => p.GetBoxes().Select(b => new BoundingBox(b.Position + p.Translation, b.Position + p.Translation + b.Size)))
                .GetEnclosingBoundingBox();
            
            if (!GameModelImporter.ModelMetaData.TryGetValue(model.Name, out JsonModelMetaData modelMetaData))
            {
                Trace.TraceError($"[{nameof(ModelRenderer)}@{nameof(LoadModel)}] : Couldn't get meta data for model: '{model.Name}'");
                return;
            }

            if (modelMetaData.RootParts.Length == 0)
            {
                modelMetaData = new JsonModelMetaData()
                {
                    TextureLocations = modelMetaData.TextureLocations,
                    RootParts = model.GetParts().Select(p => new ModelMetaDataPart() { Name = p.Name }).ToArray()
                };
            }

            foreach (ModelMetaDataPart metaDataPart in modelMetaData.RootParts)
            {
                if (!model.TryGetPart(metaDataPart.Name, out ModelPart modelPart))
                {
                    Trace.TraceError($"[{nameof(ModelRenderer)}@{nameof(LoadModel)}] : Failed to find part: '{metaDataPart.Name}'");
                }

                Vector3 translation = modelPart.Translation.ToOpenTKVector();

                var cubeMeshCollection = new CubeMeshCollection(modelPart.Name, translation, translation * -1, modelPart.Rotation.ToOpenTKVector() + modelPart.AdditionalRotation.ToOpenTKVector());
                cubeMeshCollection.FlipZMapping = true;
                foreach (ModelBox boxes in modelPart.GetBoxes())
                {
                    cubeMeshCollection.AddNamed(modelPart.Name, boxes.Position.ToOpenTKVector(), boxes.Size.ToOpenTKVector(), boxes.Uv.ToOpenTKVector(), boxes.Inflate, boxes.Mirror);
                }

                RetriveChildMeshes(metaDataParts: metaDataPart.Children, ref model).ForEach(cubeMeshCollection.Add);

                _rootCollection.Add(cubeMeshCollection);
            }

            if (Context.IsCurrent)
            {
                ShaderProgram shader = GetShader("CubeShader");
                shader.Bind();
                shader.SetUniform2("TexSize", model.TextureSize);
            }
            _currentModelName = model.Name;
        }

        public override void ResetCamera(Vector3 offset)
        {
            Vector3 center = (_maxBounds.Start + _maxBounds.End) / 2f;
            Camera.FocalPoint = Vector3.TransformPosition(center + offset, Matrix4.CreateScale(-1f, 1f, 1f));
            Camera.Distance = _maxBounds.Volume.Length;
            Camera.Yaw = 45f;
            Camera.Pitch = 25f;
        }

        private static CubeMesh ToCubeMesh(ModelBox box) => ToCubeMesh(box, Vector3.Zero);
        private static CubeMesh ToCubeMesh(ModelBox box, Vector3 translation)
            => new CubeMesh(new Cube(box.Position.ToOpenTKVector() + translation, box.Size.ToOpenTKVector(), box.Uv.ToOpenTKVector(), box.Inflate, box.Mirror, true));

        private List<GenericMesh<TextureVertex>> RetriveChildMeshes(ModelMetaDataPart[] metaDataParts, ref Model model)
        {
            List<GenericMesh<TextureVertex>> meshes = new List<GenericMesh<TextureVertex>>();
            foreach (ModelMetaDataPart metaDataPart in metaDataParts)
            {
                if (!model.TryGetPart(metaDataPart.Name, out ModelPart modelPart))
                {
                    Trace.TraceError($"[{nameof(ModelRenderer)}@{nameof(RetriveChildMeshes)}] : Failed to find part: '{metaDataPart.Name}'");
                }
                Vector3 translation = modelPart.Translation.ToOpenTKVector();
                meshes.AddRange(modelPart.GetBoxes().Select(b => ToCubeMesh(b, translation)));
                meshes.AddRange(RetriveChildMeshes(metaDataPart.Children, ref model));
            }
            return meshes;
        }

        protected virtual void OnModelTextureChanging(object sender, TextureChangingEventArgs e)
        {
            if (e.NewTexture is null)
                e.Cancel = true;

            if (e.Cancel)
                return;

            if (Context.IsCurrent)
            {
                _modelRenderTexture.SetTexture(e.NewTexture);
                GLErrorCheck();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (DesignMode)
                return;
            if (!Context.IsCurrent)
                return;

            GL.Enable(EnableCap.Texture2D); // Enable textures

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest); // Enable transparent
            GL.AlphaFunc(AlphaFunction.Greater, 0.0f);
            GL.DepthFunc(DepthFunction.Lequal);

            ShaderProgram shader = GetShader("CubeShader");

            _modelRenderTexture.Bind(slot: 0);

            Vector3 scaleVector = new Vector3(1f, -1f, -1f);
            Matrix4 renderTransform = Matrix4.CreateScale(scaleVector);

            foreach (CubeMeshCollection item in _rootCollection)
            {
                DrawMesh(item, shader, item.GetTransform() * renderTransform);
            }
            _modelRenderTexture.Unbind();

            if (!_highlightingInfo.IsEmpty)
            {
                Matrix4 highlightMatrix = Matrix4.Identity;
                highlightMatrix        *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_highlightingInfo.Rotation.X));
                highlightMatrix        *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_highlightingInfo.Rotation.Y));
                highlightMatrix        *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_highlightingInfo.Rotation.Z));
                highlightMatrix         = Matrix4.CreateTranslation(_highlightingInfo.Pivot) * highlightMatrix.Pivoted(_highlightingInfo.Pivot * -1) * renderTransform;
                DrawBoundingBox(highlightMatrix, _highlightingInfo.BoundingBox, Color.HotPink);
            }

            if (RenderModelBounds)
            {
                DrawBoundingBox(renderTransform, _maxBounds, Color.Red);
            }
        }
        internal void Highlight(ModelPart part)
        {
            BoundingBox bb = part.GetBoxes().Select(b => new BoundingBox(b.Position, b.Position + b.Size)).GetEnclosingBoundingBox();
            _highlightingInfo = new HighlightInfo(part.Translation, part.Rotation + part.AdditionalRotation, bb);
        }

        internal void ResetHighlight()
        {
            _highlightingInfo = HighlightInfo.Empty;
        }
    }
}
