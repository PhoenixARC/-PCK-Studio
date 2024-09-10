using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using OMI.Formats.Model;
using MetroFramework.Forms;

using PckStudio.Internal;

namespace PckStudio.Forms.Editor
{
    public partial class ModelEditor : MetroForm
    {
        private readonly ModelContainer _models;
        private readonly TryGetTextureDelegate _tryGetTexture;
        private readonly TrySetTextureDelegate _trySetTexture;


        public delegate bool TryGetTextureDelegate(string path, out Image img);
        public delegate bool TrySetTextureDelegate(string path, Image img);

        public ModelEditor(ModelContainer models, TryGetTextureDelegate tryGetTexture, TrySetTextureDelegate trySetTexture)
        {
            InitializeComponent();
            _models = models;
            _tryGetTexture = tryGetTexture;
            _trySetTexture = trySetTexture;
        }

        private class ModelNode : TreeNode
        {
            private Model _model;
            public Model Model => _model;

            public ModelNode(Model model)
                : base(model.Name)
            {
                _model = model;
                Nodes.AddRange(GetModelNodes(_model.GetParts()).ToArray());
            }
            private static IEnumerable<TreeNode> GetModelNodes(IEnumerable<ModelPart> parts)
            {
                return parts.Select(part => new ModelPartNode(part));
            }
        }

        private class ModelPartNode : TreeNode
        {
            private ModelPart _part;

            public ModelPart Part => _part;

            public ModelPartNode(ModelPart part)
                : base(part.Name)
            {
                _part = part;
                Nodes.AddRange(GetModelPartNodeChildren(part.GetBoxes()).ToArray());
            }
            private static IEnumerable<TreeNode> GetModelPartNodeChildren(IEnumerable<ModelBox> boxes)
            {
                return boxes.Select(box => new ModelBoxNode(box));
            }
        }

        private class ModelBoxNode : TreeNode
        {
            private ModelBox _modelBox;
            public ModelBoxNode(ModelBox modelBox)
                : base($"Box: pos:{modelBox.Position} size:{modelBox.Size}")
            {
                _modelBox = modelBox;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            modelTreeView.Nodes.AddRange(_models.Select(model => new ModelNode(model)).ToArray());
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (modelTreeView.SelectedNode is ModelNode modelNode)
            {
                Model model = modelNode.Model;
                Debug.Write(model.Name + "; ");
                Debug.WriteLine(model.TextureSize);

                GameModelImporter.Default.ExportSettings.CreateModelOutline =
                    MessageBox.Show(
                        $"Do you wish to have all model parts contained in a group called '{model.Name}'?",
                        "Group model parts", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                using SaveFileDialog openFileDialog = new SaveFileDialog();
                openFileDialog.FileName = model.Name;
                openFileDialog.Filter = GameModelImporter.Default.SupportedModelFileFormatsFilter;

                IEnumerable<NamedTexture> GetModelTextures(string modelName)
                {
                    if (!GameModelImporter.ModelMetaData.ContainsKey(modelName) || GameModelImporter.ModelMetaData[modelName]?.TextureLocations?.Length <= 0)
                        yield break;
                    foreach (var textureLocation in GameModelImporter.ModelMetaData[modelName].TextureLocations)
                    {
                        if (_tryGetTexture(textureLocation, out Image img))
                            yield return new NamedTexture(Path.GetFileName(textureLocation), img);
                    }
                    yield break;
                }

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    IEnumerable<NamedTexture> textures = GetModelTextures(model.Name);
                    var modelInfo = new GameModelInfo(model, textures);
                    GameModelImporter.Default.Export(openFileDialog.FileName, modelInfo);
                }
            }
        }

        private void modelTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            exportToolStripMenuItem.Visible = e.Node is ModelNode;
            editToolStripMenuItem.Visible = e.Node is ModelBoxNode;
            removeToolStripMenuItem.Visible = e.Node is ModelPartNode || e.Node is ModelBoxNode;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
