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
        public Model Model
        {
            get => _model;
            set
            {
                _model = value;
                InitModelRender(_model);
            }
        }
        
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

        public bool RenderModelBounds { get; set; }

        [Description("Event that gets fired when the skin texture is changing")]
        [Category("Property Chnaged")]
        [Browsable(true)]
        public event EventHandler<TextureChangingEventArgs> ModelTextureChanging
        {
            add => Events.AddHandler(nameof(ModelTextureChanging), value);
            remove => Events.RemoveHandler(nameof(ModelTextureChanging), value);
        }

        private BoundingBox _maxBounds;
        private Model _model;
        private Image _modelTexture;
        private Texture2D _modelRenderTexture;
        private List<CubeMeshCollection> _rootCollection;

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
            if (!Context.IsCurrent)
                MakeCurrent();

            // render texture
            {
                _modelRenderTexture = new Texture2D(0);
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

        private void InitModelRender(Model model)
        {
            _rootCollection?.Clear();

            IEnumerable<BoundingBox> allBoxes = model.GetParts()
                .SelectMany(p => p.GetBoxes()
                    .Select(b => new ModelBox(b.Position + p.Translation, b.Size, System.Numerics.Vector2.Zero, 0f, false)))
                .Select(b => new BoundingBox(b.Position, b.Position + b.Size));
            
            _maxBounds = GetBounds(allBoxes);

            Vector3 center = (_maxBounds.Start + _maxBounds.End) / 2f;

            Camera.FocalPoint = center;
            Camera.Distance = _maxBounds.Volume.Length * 1.3f;
            Camera.Yaw   = 45f;
            Camera.Pitch = 25f;

            if (!GameModelImporter.ModelMetaData.TryGetValue(model.Name, out JsonModelMetaData modelMetaData))
            {
                Trace.TraceError($"[{nameof(ModelRenderer)}@{nameof(InitModelRender)}] : Couldn't get meta data for model: '{model.Name}'");
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
                    Trace.TraceError($"[{nameof(ModelRenderer)}@{nameof(InitModelRender)}] : Failed to find part: '{metaDataPart.Name}'");
                }

                Vector3 translation = modelPart.Translation.ToOpenTKVector();

                var cubeMeshCollection = new CubeMeshCollection(modelPart.Name, translation, translation, modelPart.Rotation.ToOpenTKVector() + modelPart.AdditionalRotation.ToOpenTKVector());
                cubeMeshCollection.FlipZMapping = true;
                foreach (ModelBox boxes in modelPart.GetBoxes())
                {
                    cubeMeshCollection.AddNamed(modelPart.Name, boxes.Position.ToOpenTKVector() /*+ modelPart.Translation.ToOpenTKVector()*/, boxes.Size.ToOpenTKVector(), boxes.Uv.ToOpenTKVector(), boxes.Inflate, boxes.Mirror);
                }

                RetriveChildMeshes(metaDataParts: metaDataPart.Children).ForEach(cubeMeshCollection.Add);

                _rootCollection.Add(cubeMeshCollection);
            }

            MakeCurrent();
            ShaderProgram shader = GetShader("CubeShader");

            shader.SetUniform2("TexSize", model.TextureSize);
        }

        private static CubeMesh ToCubeMesh(ModelBox box) => ToCubeMesh(box, Vector3.Zero);
        private static CubeMesh ToCubeMesh(ModelBox box, Vector3 translation)
            => new CubeMesh(new Cube(translation + box.Position.ToOpenTKVector(), box.Size.ToOpenTKVector(), box.Uv.ToOpenTKVector(), box.Inflate, box.Mirror, true));

        private List<GenericMesh<TextureVertex>> RetriveChildMeshes(ModelMetaDataPart[] metaDataParts)
        {
            List<GenericMesh<TextureVertex>> meshes = new List<GenericMesh<TextureVertex>>();
            foreach (ModelMetaDataPart metaDataPart in metaDataParts)
            {
                if (!Model.TryGetPart(metaDataPart.Name, out ModelPart modelPart))
                {
                    Trace.TraceError($"[{nameof(ModelRenderer)}@{nameof(RetriveChildMeshes)}] : Failed to find part: '{metaDataPart.Name}'");
                }
                Vector3 translation = modelPart.Translation.ToOpenTKVector();
                meshes.AddRange(modelPart.GetBoxes().Select(b => ToCubeMesh(b, translation)));
                meshes.AddRange(RetriveChildMeshes(metaDataPart.Children));
            }
            return meshes;
        }

        protected virtual void OnModelTextureChanging(object sender, TextureChangingEventArgs e)
        {
            if (e.NewTexture is null)
                e.Cancel = true;

            if (e.Cancel)
                return;
            if (!Context.IsCurrent)
                MakeCurrent();
            _modelRenderTexture.SetTexture(e.NewTexture);
            GLErrorCheck();
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

            _modelRenderTexture.Bind();

            if (RenderModelBounds)
            {
                DrawBoundingBox(Matrix4.CreateScale(1f, -1f, -1f), _maxBounds, Color.Red);
            }

            foreach (CubeMeshCollection item in _rootCollection)
            {
                DrawMesh(item, shader, item.Transform * Matrix4.CreateScale(1f, -1f, -1f));
            }
        }
    }
}
