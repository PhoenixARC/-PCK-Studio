using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMI.Formats.Pck;
using OMI.Workers.Pck;
using PckStudio.Core.Extensions;
using PckStudio.Core.Skin;

namespace PckStudio.Controls
{
    public partial class SkinsPanel : ViewPanel
    {
        readonly PckFileReader _reader;
        readonly ImageList _imageList;
        public SkinsPanel(OMI.ByteOrder byteOrder)
        {
            InitializeComponent();
            _reader = new PckFileReader(byteOrder);
            _imageList = new ImageList()
            {
                ImageSize = new Size(32, 32),
                ColorDepth = ColorDepth.Depth32Bit,
            };
            treeView1.ImageList = _imageList;
        }

        public override void LoadAsset(PckAsset asset, Action onChange)
        {
            Reset();
            treeView1.Nodes.AddRange(
                asset.GetData(_reader).GetAssetsByType(PckAssetType.SkinFile)
                .Select(PckAssetExtensions.GetSkin)
                .enumerate()
                .Select(a =>
                {
                    _imageList.Images.Add(a.value.GetPreviewImage(_imageList.ImageSize));
                    return new TreeNode(a.value.MetaData.Name) { ImageIndex = a.index, SelectedImageIndex = a.index, Tag = a.value };
                })
                .ToArray()
                );
        }

        public override void Reset()
        {
            _imageList.Images.Clear();
            treeView1.Nodes.Clear();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e?.Node.Tag is Skin skin)
            {
                skinRenderer1.LoadSkin(skin);
            }
        }
    }
}
