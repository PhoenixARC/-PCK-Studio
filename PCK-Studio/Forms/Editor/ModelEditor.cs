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
using PckStudio.Internal.Json;
using PckStudio.Internal.App;
using PckStudio.Extensions;

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

            modelTreeView.ImageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(32, 32)
            };
            modelTreeView.ImageList.Images.AddRange(ApplicationScope.EntityImages);
        }

        private const int InvalidImageIndex = 127;
        // TODO: move to json file. -miku
        private static Dictionary<string, int> ModelImageIndex = new Dictionary<string, int>()
        {
                ["bat"]              = 3,
                ["blaze"]            = 4,
                ["boat"]             = 5,
                ["cat"]              = 6,
                ["spider"]           = 107,
                ["chicken"]          = 9,
                ["cod"]              = 10,
                ["cow"]              = 12,
                ["creeper"]          = 13,
                ["creeper_head"]     = 13,
                ["dolphin"]          = 14,
                ["horse.v2"]         = 110,
                ["guardian"]         = 109,
                ["bed"]              = 108,
                ["dragon"]           = 21,
                ["dragon_head"]      = 21,
                ["enderman"]         = 23,
                ["ghast"]            = 34,
                ["irongolem"]        = 40,
                ["lavaslime"]        = 46,
                ["llama"]            = 44,
                ["llamaspit"]        = 45,
                ["minecart"]         = 47,
                ["ocelot"]           = 50,
                ["parrot"]           = 53,
                ["phantom"]          = 54,
                ["pig"]              = 55,
                ["pigzombie"]        = 94,
                ["polarbear"]        = 57,
                ["rabbit"]           = 60,
                ["sheep"]            = 63,
                ["sheep.sheared"]    = 113,
                ["shulker"]          = 64,
                ["silverfish"]       = 66,
                ["skeleton"]         = 67,
                ["skeleton_head"]    = 67,
                ["skeleton.stray"]   = 77,
                ["skeleton.wither"]  = 89,
                ["skeleton_wither_head"] = 89,
                ["slime"]            = 115,
                ["slime.armor"]      = 116,

                ["snowgolem"]        = 71,
                ["squid"]            = 76,
                ["trident"]          = 80,
                ["turtle"]           = 82,
                ["villager"]         = 84,
                ["villager.witch"]   = 87,

                ["vex"]              = 83,
                ["evoker"]           = 25,
                ["vindicator"]       = 25,
                ["witherBoss"]       = 88,
                ["wolf"]             = 91,
                ["zombie"]           = 92,
                ["zombie_head"]      = 92,
                ["zombie.husk"]      = 39,
                ["zombie.villager"]  = 95,
                ["zombie.drowned"]   = 17,
                ["endermite"]        = 24,
                ["pufferfish.small"] = 111,
                ["pufferfish.mid"]   = 112,
                ["pufferfish.large"] = 59,
                ["salmon"]           = 62,
                ["stray.armor"]      = 118,
                ["stray_armor"]      = 118,
                ["tropicalfish_a"]   = 81,
                ["tropicalfish_b"]   = 81,
                ["mooshroom"]        = 48,
                ["witherBoss.armor"] = 90,

                ["panda"]              = 52,
                ["ravager"]            = 61,
                ["pillager"]           = 56,
                ["villager_v2"]        = 101,
                ["zombie.villager_v2"] = 102,
        };

        private static int GetModelImageIndex(string name) => ModelImageIndex.TryGetValue(name, out int index) ? index : InvalidImageIndex;

        private class ModelNode : TreeNode
        {
            private Model _model;
            public Model Model => _model;

            private ModelNode(Model model)
                : base(model.Name)
            {
                _model = model;
                ImageIndex = GetModelImageIndex(model.Name);
                SelectedImageIndex = GetModelImageIndex(model.Name);
                Nodes.AddRange(GetModelPartNodes(_model.GetParts()).ToArray());
            }
            private static IEnumerable<TreeNode> GetModelPartNodes(IEnumerable<ModelPart> parts) => parts.Select(ModelPartNode.Create);

            internal static ModelNode Create(Model model) => new ModelNode(model);
        }

        private class ModelPartNode : TreeNode
        {
            private ModelPart _part;

            public ModelPart Part => _part;

            private ModelPartNode(ModelPart part)
                : base(part.Name)
            {
                _part = part;
                ImageIndex = 126;
                SelectedImageIndex = 126;
                Nodes.AddRange(GetModelBoxNodes(part.GetBoxes()).ToArray());
            }
            private static IEnumerable<TreeNode> GetModelBoxNodes(IEnumerable<ModelBox> boxes) => boxes.Select(ModelBoxNode.Create);

            internal static ModelPartNode Create(ModelPart part) => new ModelPartNode(part);
        }

        private class ModelBoxNode : TreeNode
        {
            private ModelBox _modelBox;
            private ModelBoxNode(ModelBox modelBox)
                : base($"Box: pos:{modelBox.Position} size:{modelBox.Size}")
            {
                ImageIndex = 126;
                SelectedImageIndex = 126;
                _modelBox = modelBox;
            }

            internal static ModelBoxNode Create(ModelBox modelBox) => new ModelBoxNode(modelBox);
        }

        private class NamedTextureTreeNode : TreeNode
        {
            private readonly NamedTexture _namedTexture;

            public NamedTextureTreeNode(NamedTexture namedTexture)
                : base(namedTexture.Name)
            {
                Tag = namedTexture;
                _namedTexture = namedTexture;
            }

            public Image GetTexture() => _namedTexture.Texture;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            modelTreeView.Nodes.AddRange(_models.Select(ModelNode.Create).ToArray());
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
            if (e.Node is ModelNode modelNode)
            {
                NamedTexture[] textures = GetModelTextures(modelNode.Model.Name).ToArray();
                
                textureImageList.Images.Clear();
                namedTexturesTreeView.Nodes.Clear();

                foreach ((int i, NamedTexture item) in textures.enumerate())
                {
                    textureImageList.Images.Add(item.Texture);
                    namedTexturesTreeView.Nodes.Add(new NamedTextureTreeNode(item) { ImageIndex = i, SelectedImageIndex = i });
                }
                if (textures.Length != 0)
                    modelViewport.Texture = textures[0].Texture;

                modelViewport.LoadModel(modelNode.Model);
                modelViewport.ResetCamera();
            }
        }

        private IEnumerable<NamedTexture> GetModelTextures(string modelName)
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

        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = GameModelImporter.Default.SupportedModelFileFormatsFilter;
            fileDialog.Title = "Select model";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                GameModelInfo modelInfo = GameModelImporter.Default.Import(fileDialog.FileName);
                if (modelInfo is null)
                {
                    MessageBox.Show("Import failed.", ProductName);
                    return;
                }

                if (!GameModelImporter.ModelMetaData.TryGetValue(modelInfo.Model.Name, out JsonModelMetaData modelMetaData))
                {
                    MessageBox.Show($"Couldn't get model meta data for: '{modelInfo.Model.Name}'.", ProductName);
                    return;
                }

                //if (models.Version < modelInfo.ModelVersion)
                //{
                //	MessageBox.Show("Model container version does not match with the model version.", ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //	return;
                //}

                _models.Add(modelInfo.Model);

                foreach (NamedTexture texture in modelInfo.Textures)
                {
                    _trySetTexture(texture.Name, texture.Texture);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void namedTexturesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (namedTexturesTreeView.SelectedNode is NamedTextureTreeNode namedTextureNode)
                modelViewport.Texture = namedTextureNode.GetTexture();
        }

        private void showModelBoundsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            modelViewport.RenderModelBounds = showModelBoundsToolStripMenuItem.Checked;
        }
    }
}
