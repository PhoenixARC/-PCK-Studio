using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMI.Formats.Material;
using OMI.Formats.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PckStudio.Extensions;
using PckStudio.Interfaces;
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
        private List<GenericMesh<TextureVertex>> _rootCollection;
        private struct HighlightInfo
        {
            public static readonly HighlightInfo Empty = new HighlightInfo(Vector3.Zero, Vector3.Zero, BoundingBox.Empty);
            public bool IsEmpty => BoundingBox.Volume.LengthSquared <= 0f;
            public BoundingBox BoundingBox { get; }
            public Vector3 Translation { get; }
            public Vector3 Rotation { get; }

            public HighlightInfo(System.Numerics.Vector3 translation, System.Numerics.Vector3 rotation, BoundingBox boundingBox)
                : this(translation.ToOpenTKVector(), rotation.ToOpenTKVector(), boundingBox)
            {
            }

            public HighlightInfo(Vector3 translation, Vector3 rotation, BoundingBox boundingBox)
            {
                Translation = translation;
                Rotation = rotation;
                BoundingBox = boundingBox;
            }
        }
        private HighlightInfo _highlightingInfo;

        private readonly Vector3 modelOffset = Vector3.UnitY * 24f;

        public ModelRenderer() : base(fov: 60f)
        {
            InitializeComponent();
            _rootCollection = new List<GenericMesh<TextureVertex>>(5);
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
            
            if (!TryGetModelMetaData(model, out JsonModelMetaData modelMetaData))
            {
                Trace.TraceError($"[{nameof(ModelRenderer)}@{nameof(LoadModel)}] Failed to get meta data for model: '{model.Name}'");
                return;
            }

            _rootCollection.AddRange(BuildModelMesh(modelMetaData.RootParts, Vector3.Zero, Vector3.Zero, Vector3.Zero, TryGet<string, ModelPart>.FromDelegate(model.TryGetPart)));

            if (Context.IsCurrent)
            {
                ShaderProgram shader = GetShader("CubeShader");
                shader.Bind();
                shader.SetUniform2("TexSize", model.TextureSize);

                // Reset "Material"
                GL.Disable(EnableCap.AlphaTest);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
            }
            _currentModelName = model.Name;
        }

        private bool TryGetModelMetaData(Model model, out JsonModelMetaData modelMetaData)
        {
            if (!GameModelImporter.ModelMetaData.TryGetValue(model.Name, out modelMetaData))
            {
                Trace.TraceError($"[{nameof(ModelRenderer)}@{nameof(TryGetModelMetaData)}] Couldn't get meta data for model: '{model.Name}'");
                return false;
            }

            if (modelMetaData.RootParts.Length == 0)
            {
                modelMetaData.RootParts = model.GetParts().Select(p => new ModelMetaDataPart(p.Name)).ToArray();
            }
            return true;
        }

        public override void ResetCamera(Vector3 offset)
        {
            Vector3 center = (_maxBounds.Start + _maxBounds.End) / 2f;
            Camera.FocalPoint = Vector3.TransformPosition(center - modelOffset + offset, Matrix4.CreateScale(-1f, 1f, 1f));
            Camera.Distance = _maxBounds.Volume.Length;
            Camera.Yaw = 45f;
            Camera.Pitch = 25f;
        }

        private List<GenericMesh<TextureVertex>> BuildModelMesh(ModelMetaDataPart[] metaDataParts, Vector3 parentTranslation, Vector3 parentRotation, Vector3 parentPivot, ITryGet<string, ModelPart> model)
        {
            List<GenericMesh<TextureVertex>> meshes = new List<GenericMesh<TextureVertex>>();
            foreach (ModelMetaDataPart metaDataPart in metaDataParts)
            {
                if (!model.TryGet(metaDataPart.Name, out ModelPart modelPart))
                {
                    Trace.TraceError($"[{nameof(ModelRenderer)}@{nameof(BuildModelMesh)}] : Failed to find part: '{metaDataPart.Name}'");
                    continue;
                }
                Vector3 translation = modelPart.Translation.ToOpenTKVector();
                Vector3 pivot = translation * -1 - parentPivot;
                Vector3 rotation = (modelPart.Rotation.ToOpenTKVector() + modelPart.AdditionalRotation.ToOpenTKVector());
                CubeMeshCollection cubeCollection = new CubeMeshCollection(modelPart.Name, translation - parentTranslation, pivot, rotation - parentRotation);
                cubeCollection.FlipZMapping = true;
                foreach (ModelBox boxes in modelPart.GetBoxes())
                {
                    cubeCollection.AddNamed(modelPart.Name, boxes.Position.ToOpenTKVector(), boxes.Size.ToOpenTKVector(), boxes.Uv.ToOpenTKVector(), boxes.Inflate, boxes.Mirror);
                }
                meshes.Add(cubeCollection);
                BuildModelMesh(metaDataPart.Children, translation, rotation, pivot, model).ForEach(cubeCollection.Add);
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

            GL.DepthFunc(DepthFunction.Lequal);

            ShaderProgram shader = GetShader("CubeShader");

            _modelRenderTexture.Bind(slot: 0);

            Vector3 scaleVector = new Vector3(1f, -1f, -1f);
            Matrix4 renderTransform = Matrix4.CreateScale(scaleVector) * Matrix4.CreateTranslation(modelOffset);

            foreach (GenericMesh<TextureVertex> item in _rootCollection)
            {
                DrawMesh(item, shader, item.GetTransform() * renderTransform);
            }
            _modelRenderTexture.Unbind();

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            if (!_highlightingInfo.IsEmpty)
            {
                Matrix4 highlightMatrix = Matrix4.Identity;
                highlightMatrix        *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_highlightingInfo.Rotation.X));
                highlightMatrix        *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_highlightingInfo.Rotation.Y));
                highlightMatrix        *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_highlightingInfo.Rotation.Z));
                highlightMatrix         = Matrix4.CreateTranslation(_highlightingInfo.Translation) * highlightMatrix.Pivoted(_highlightingInfo.Translation * -1);

                highlightMatrix *= renderTransform;
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

        internal void SetModelMaterial(MaterialContainer.Material entityMaterial)
        {
            if (entityMaterial is null)
                return;
            switch (entityMaterial.Type)
            {
                // todo
                //case "entity":
                //    break;
                //case "entity_change_color":
                //    break;
                case "entity_alphatest":
                    GL.Enable(EnableCap.AlphaTest);
                    break;
                case "entity_alphatest_change_color":
                    GL.Enable(EnableCap.AlphaTest);
                    goto default;
                case "entity_emissive_alpha":
                    GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
                    break;
                case "entity_emissive_alpha_only":
                    GL.BlendFuncSeparate(BlendingFactorSrc.One, BlendingFactorDest.One, BlendingFactorSrc.One, BlendingFactorDest.Zero);
                    break;
                default:
                    Debug.WriteLine($"[{nameof(ModelRenderer)}@{nameof(SetModelMaterial)}]: Unhandled entity material(type: {entityMaterial.Type})");
                    break;
            }
        }
    }
}
