using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordRPC;
using OMI.Formats.Pck;
using OMI.Workers.Pck;
using PckStudio.Core.DLC;
using PckStudio.Core.Extensions;
using PckStudio.Core.Skin;

namespace PckStudio.Controls
{
    public partial class SkinsPanel : ViewPanel
    {
        readonly PckFileReader _reader;
        readonly ImageList _imageList;
        public SkinsPanel(OMI.ByteOrder byteOrder) : this()
        {
            _reader = new PckFileReader(byteOrder);
            _imageList = new ImageList()
            {
                ImageSize = new Size(32, 32),
                ColorDepth = ColorDepth.Depth32Bit,
            };
            treeView1.ImageList = _imageList;
        }

        public SkinsPanel(DLCSkinPackage skinPackage) : this()
        {
            Reset();
            LoadTreeview(skinPackage.GetSkins());
        }

        public override void LoadAsset(PckAsset asset, Action onChange)
        {
            Reset();
            LoadTreeview(asset.GetData(_reader).GetAssetsByType(PckAssetType.SkinFile).Select(PckAssetExtensions.GetSkin));
        }

        private void LoadTreeview(IEnumerable<Skin> skins)
        {
            treeView1.Nodes.AddRange(
                skins
                .Select((skin, i) =>
                {
                    _imageList.Images.Add(skin.GetPreviewImage(_imageList.ImageSize));
                    return new TreeNode(skin.MetaData.Name) { ImageIndex = i, SelectedImageIndex = i, Tag = skin };
                })
                .ToArray()
                );
        }

        private SkinsPanel()
        {
            InitializeComponent();
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
