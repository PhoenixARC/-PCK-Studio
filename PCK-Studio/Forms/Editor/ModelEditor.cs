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
using PckStudio.Internal.App;

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

            modelTreeView.ImageList = new ImageList();
            modelTreeView.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            modelTreeView.ImageList.ImageSize = new Size(32, 32);
            ApplicationScope.EntityImages.ToList().ForEach(modelTreeView.ImageList.Images.Add);
        }

        // TODO: move to json file. -miku
        private static int GetModelImageIndex(string modelName)
        {
            return modelName switch
            {
                "bat"              => 3,
                "blaze"            => 4,
                "boat"             => 5,
                "cat"              => 6,
                "spider"           => 107,
                "chicken"          => 9,
                "cod"              => 10,
                "cow"              => 12,
                "creeper"          => 13,
                "creeper_head"     => 13,
                "dolphin"          => 14,
                "horse.v2"         => 110,
                "guardian"         => 109,
                "bed"              => 108,
                "dragon"           => 21,
                "dragon_head"      => 21,
                "enderman"         => 23,
                "ghast"            => 34,
                "irongolem"        => 40,
                "lavaslime"        => 46,
                "llama"            => 44,
                "llamaspit"        => 45,
                "minecart"         => 47,
                "ocelot"           => 50,
                "parrot"           => 53,
                "phantom"          => 54,
                "pig"              => 55,
                "pigzombie"        => 94,
                "polarbear"        => 57,
                "rabbit"           => 60,
                "sheep"            => 63,
                "sheep.sheared"    => 113,
                "shulker"          => 64,
                "silverfish"       => 66,
                "skeleton"         => 67,
                "skeleton_head"    => 67,
                "skeleton.stray"   => 77,
                "skeleton.wither"  => 89,
                "skeleton_wither_head" => 89,
                "slime"            => 115,
                "slime.armor"      => 116,

                "snowgolem"        => 71,
                "squid"            => 76,
                "trident"          => 80,
                "turtle"           => 82,
                "villager"         => 84,
                "villager.witch"   => 87,

                "vex"              => 83,
                "evoker"           => 25,
                "vindicator"       => 25,
                "witherBoss"       => 88,
                "wolf"             => 91,
                "zombie"           => 92,
                "zombie_head"      => 92,
                "zombie.husk"      => 39,
                "zombie.villager"  => 95,
                "zombie.drowned"   => 17,
                "endermite"        => 24,
                "pufferfish.small" => 111,
                "pufferfish.mid"   => 112,
                "pufferfish.large" => 59,
                "salmon"           => 62,
                "stray.armor"      => 118,
                "stray_armor"      => 118,
                "tropicalfish_a"   => 81,
                "tropicalfish_b"   => 81,
                "mooshroom"        => 48,
                "witherBoss.armor" => 90,

                "panda"            => 52,
                "ravager"          => 61,
                "pillager"         => 56,
                "villager_v2"      => 101,
                "zombie.villager_v2" => 102,

                _ => 127
            };
        }

        private class ModelNode : TreeNode
        {
            private Model _model;
            public Model Model => _model;

            public ModelNode(Model model)
                : base(model.Name)
            {
                _model = model;
                ImageIndex = GetModelImageIndex(model.Name);
                SelectedImageIndex = GetModelImageIndex(model.Name);
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
                ImageIndex = 126;
                SelectedImageIndex = 126;
                Nodes.AddRange(GetModelPartNodeChildren(part.GetBoxes()).ToArray());
            }
            private static IEnumerable<TreeNode> GetModelPartNodeChildren(IEnumerable<ModelBox> boxes)
                => boxes.Select(box => new ModelBoxNode(box));
            }

        private class ModelBoxNode : TreeNode
        {
            private ModelBox _modelBox;
            public ModelBoxNode(ModelBox modelBox)
                : base($"Box: pos:{modelBox.Position} size:{modelBox.Size}")
            {
                ImageIndex = 126;
                SelectedImageIndex = 126;
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
