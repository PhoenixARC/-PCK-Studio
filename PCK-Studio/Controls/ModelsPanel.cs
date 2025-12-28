using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMI.Formats.Model;
using OMI.Formats.Pck;
using OMI.Workers.Model;
using PckStudio.Core;
using PckStudio.Core.Extensions;
using PckStudio.Interfaces;
using PckStudio.ModelSupport;
using PckStudio.Rendering.Texture;

namespace PckStudio.Controls
{
    public partial class ModelsPanel : ViewPanel
    {
        private readonly ITryGet<string, Image> _textures;
        private readonly ImageList _imageList = new ImageList()
        {
            ColorDepth = ColorDepth.Depth32Bit,
            ImageSize = new Size(32, 32)
        };
        private ModelContainer _models;

        public ModelsPanel(ITryGet<string, Image> tryGetTexture)
        {
            InitializeComponent();
            _textures = tryGetTexture;
            textureTreeView.ImageList = _imageList;
        }

        public override void LoadAsset(PckAsset asset, Action onChange)
        {
            _models = asset.GetData(new ModelFileReader());
            modelTreeView.Nodes.AddRange(_models.Select(m => new TreeNode(m.Name)).ToArray());
            if (modelTreeView.Nodes.Count > 0)
            {
                modelTreeView.SelectedNode = modelTreeView.Nodes[0];
                modelRenderer.Visible = true;
                modelRenderer.MakeCurrent();
            }
        }

        public override void Reset()
        {
            modelTreeView.Nodes.Clear();
            textureTreeView.Nodes.Clear();
            modelRenderer.Visible = false;
            _models = null;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string modelName = e?.Node?.Text;
            modelRenderer.Visible = _models.ContainsModel(modelName);
            if (!modelRenderer.Visible)
                return;

            NamedData<Image>[] textures = GetModelTextures(modelName, _textures).ToArray();

            _imageList.Images.Clear();
            textureTreeView.Nodes.Clear();

            foreach ((int i, NamedData<Image> item) in textures.Enumerate())
            {
                _imageList.Images.Add(item.Value);
                textureTreeView.Nodes.Add(new NamedTextureTreeNode(item) { ImageIndex = i, SelectedImageIndex = i });
            }
            if (textures.Length > 0)
            {
                modelRenderer.Texture = textures[0].Value;
                modelRenderer.LoadModel(_models.GetModelByName(modelName));
            }
        }

        private static IEnumerable<NamedData<Image>> GetModelTextures(string modelName, ITryGet<string, Image> tryGet)
        {
            if (!GameModelImporter.EntityModelMetaData.ContainsKey(modelName) || GameModelImporter.EntityModelMetaData[modelName]?.TextureLocations?.Length <= 0)
                yield break;
            foreach (var textureLocation in GameModelImporter.EntityModelMetaData[modelName].TextureLocations)
            {
                if (tryGet.TryGet(textureLocation, out Image img))
                    yield return new NamedData<Image>(Path.GetFileName(textureLocation), img);
            }
            yield break;
        }

        private void textureTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is NamedTextureTreeNode namedTextureNode)
                modelRenderer.Texture = namedTextureNode.GetTexture();
        }

        private void showBoundsCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            modelRenderer.RenderModelBounds = showBoundsCheckBox.Checked;
        }
    }
}
