using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using MetroFramework.Forms;
using PckStudio.Internal;
using PckStudio.Extensions;
using PckStudio.Internal.IO.PSM;
using PckStudio.Internal.FileFormats;
using System.Linq;
using PckStudio.Forms.Additional_Popups;
using PckStudio.External.Format;
using Newtonsoft.Json;
using System.Numerics;
using PckStudio.Rendering;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using PckStudio.Properties;

namespace PckStudio.Forms.Editor
{
    public partial class CustomSkinEditor : MetroForm
    {
        public Image PreviewImage => _previewImage;

        public Skin ResultSkin => _skin;

        private Image _previewImage;
        private Skin _skin;
        private Random rng;
        private bool _inflateOverlayParts;
        private bool _allowInflate;

        private BindingSource skinPartListBindingSource;
        private BindingSource skinOffsetListBindingSource;

        private static GraphicsConfig _graphicsConfig = new GraphicsConfig()
        {
            InterpolationMode = InterpolationMode.NearestNeighbor,
            PixelOffsetMode = PixelOffsetMode.HighQuality,
        };

        private CustomSkinEditor()
        {
            InitializeComponent();
            rng = new Random();
            skinPartListBindingSource = new BindingSource(renderer3D1.ModelData, null);
            skinPartListBox.DataSource = skinPartListBindingSource;
            skinPartListBox.DisplayMember = "Type";
        }

        public CustomSkinEditor(Skin skin, bool inflateOverlayParts = false, bool allowInflate = false) : this()
        {
            _skin = skin;
            _allowInflate = allowInflate;
            _inflateOverlayParts = inflateOverlayParts;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            renderer3D1.Initialize(_inflateOverlayParts);
            renderer3D1.OutlineColor = Color.DarkSlateBlue;
            LoadModelData(_skin);
        }

        private void LoadModelData(Skin skin)
        {
            skinNameLabel.Text = skin.MetaData.Name;
            var boxProperties = skin.Model.AdditionalBoxes;
            var offsetProperties = skin.Model.PartOffsets;
            
            renderer3D1.ANIM = skin.Model.ANIM;

            if (skin.HasCape)
                renderer3D1.CapeTexture = skin.CapeTexture;

            renderer3D1.ModelData.Clear();
            foreach (SkinBOX box in boxProperties)
            {
                renderer3D1.ModelData.Add(box);
            }
            renderer3D1.ResetOffsets();
            foreach (SkinPartOffset offset in offsetProperties)
            {
                renderer3D1.SetPartOffset(offset);
            }

            if (skin.Model.Texture is not null)
            {
                renderer3D1.Texture = skin.Model.Texture;
            }

            if (skin.Model.Texture is null && renderer3D1.Texture is not null)
            {
                skin.Model.Texture = renderer3D1.Texture;
            }

            skinOffsetListBindingSource = new BindingSource(renderer3D1.GetOffsets().ToArray(), null);
            offsetListBox.DataSource = skinOffsetListBindingSource;
            offsetListBox.DisplayMember = "Type";
            offsetListBox.ValueMember = "Value";

            skinPartListBindingSource.ResetBindings(false);
            skinOffsetListBindingSource.ResetBindings(false);
        }

        private void GenerateUVTextureMap(SkinBOX skinBox)
        {
            using (Graphics graphics = Graphics.FromImage(_skin.Model.Texture))
            {
                graphics.ApplyConfig(_graphicsConfig);
                int argb = rng.Next(unchecked((int)0xFF000000), -1);
                var color = Color.FromArgb(argb);
                Brush brush = new SolidBrush(color);
                graphics.FillPath(brush, skinBox.GetUVGraphicsPath());
            }
            renderer3D1.Texture = _skin.Model.Texture;
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var boxEditor = new BoxEditor(SkinBOX.Empty, _allowInflate);
            if (boxEditor.ShowDialog() == DialogResult.OK)
            {
                var newBox = boxEditor.Result;
                renderer3D1.ModelData.Add(newBox);
                skinPartListBindingSource.ResetBindings(false);
                if (generateTextureCheckBox.Checked)
                    GenerateUVTextureMap(newBox);
            }
        }

        private void exportTextureButton_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image Files | *.png";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _skin.Model.Texture.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void importTextureButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG Image Files | *.png";
            openFileDialog.Title = "Select Skin Texture";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                generateTextureCheckBox.Checked = false;
                renderer3D1.Texture = Image.FromFile(openFileDialog.FileName).ReleaseFromFile(); 
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            _skin.Model.AdditionalBoxes.Clear();
            _skin.Model.AdditionalBoxes.AddRange(renderer3D1.ModelData);
            _skin.Model.PartOffsets.Clear();
            _skin.Model.PartOffsets.AddRange(renderer3D1.GetOffsets());
            // just in case they're not the same instance
            _skin.Model.ANIM = renderer3D1.ANIM;
            DialogResult = DialogResult.OK;
        }

        private void exportSkinButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Model File";
            saveFileDialog.Filter = ModelImporter.SupportedModelFileFormatsFilter;
            saveFileDialog.FileName = _skin.MetaData.Name;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                ModelImporter.Export(saveFileDialog.FileName, _skin.Model);
        }

        private void importSkinButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Model File";
            openFileDialog.Filter = ModelImporter.SupportedModelFileFormatsFilter;
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SkinModelInfo modelInfo = ModelImporter.Import(openFileDialog.FileName);
                if (modelInfo is not null)
                {
                    _skin.Model = modelInfo;
                    LoadModelData(_skin);
                }
            }
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                renderer3D1.ModelData.Add((SkinBOX)box.Clone());
                skinPartListBindingSource.ResetBindings(false);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                renderer3D1.ModelData.Remove(box);
                skinPartListBindingSource.ResetBindings(false);
            }
        }

        private void CustomSkinEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer3D1.Dispose();
        }

        private void outlineColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.SolidColorOnly = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                renderer3D1.OutlineColor = colorDialog.Color;
                skinPartListBox_SelectedIndexChanged(sender, e);
            }
        }

        private void renderer3D1_TextureChanging(object sender, Rendering.TextureChangingEventArgs e)
        {
            var img = e.NewTexture;
            // Skins can only be a 1:1 ratio (base 64x64) or a 2:1 ratio (base 64x32)
            if (img.Width != img.Height && img.Height != img.Width / 2)
            {
                e.Cancel = true;
                MessageBox.Show("The selected image does not suit a skin texture.", "Invalid image dimensions.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            uvPictureBox.Image = _skin.Model.Texture = img;
        }

        private void skinPartListBox_DoubleClick(object sender, EventArgs e)
        {
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                var boxEditor = new BoxEditor(box, _allowInflate);
                if (boxEditor.ShowDialog() == DialogResult.OK)
                {
                    renderer3D1.ModelData[skinPartListBox.SelectedIndex] = boxEditor.Result;
                    skinPartListBindingSource.ResetItem(skinPartListBox.SelectedIndex);
                }
            }
        }

        private void skinPartListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int scale = 4;
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                renderer3D1.SelectedIndex = skinPartListBox.SelectedIndex;
                Size scaleSize = new Size(_skin.Model.Texture.Width * scale, _skin.Model.Texture.Height * scale);
                uvPictureBox.Image = new Bitmap(scaleSize.Width, scaleSize.Height);
                using (Graphics g = Graphics.FromImage(uvPictureBox.Image))
                {
                    float lineWidth = ((_skin.Model.Texture.Width / renderer3D1.TextureSize.Width) + (_skin.Model.Texture.Height / renderer3D1.TextureSize.Height)) / 2f;
                    GraphicsPath graphicsPath = box.GetUVGraphicsPath(new System.Numerics.Vector2(scaleSize.Width * renderer3D1.TillingFactor.X, scaleSize.Height * renderer3D1.TillingFactor.Y));
                    g.ApplyConfig(_graphicsConfig);
                    g.DrawImage(_skin.Model.Texture, new Rectangle(Point.Empty, scaleSize), new Rectangle(Point.Empty, _skin.Model.Texture.Size), GraphicsUnit.Pixel);
                    g.DrawPath(new Pen(renderer3D1.OutlineColor, lineWidth), graphicsPath);
                }
                uvPictureBox.Invalidate();
            }
        }

        private void clampToViewCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            renderer3D1.ClampModel = clampToViewCheckbox.Checked;
        }

        private void captureScreenshotButton_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "PNG|*.png"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                renderer3D1.GetThumbnail().Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void checkGuide_CheckedChanged(object sender, EventArgs e)
        {
            outlineColorButton.Visible = renderer3D1.ShowGuideLines = checkGuide.Checked;
        }

        private void showArmorCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            renderer3D1.ShowArmor = showArmorCheckbox.Checked;
        }

        private void skinPartListBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    deleteToolStripMenuItem_Click(sender, e);
                    break;
                case Keys.Escape:
                    ClearSelection();
                    break;
                default:
                    break;
            }
        }

        private void ReloadOffsetList()
        {
            skinOffsetListBindingSource = new BindingSource(renderer3D1.GetOffsets().ToArray(), null);
            offsetListBox.DataSource = skinOffsetListBindingSource;
            skinOffsetListBindingSource.ResetBindings(false);
        }

        private void addOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var offsets = renderer3D1.GetOffsets().Select(offset => offset.Type).ToList();
            string[] available = SkinPartOffset.ValidModelOffsetTypes.Where(s => !offsets.Contains(s)).ToArray();
            using ItemSelectionPopUp typeSelection = new ItemSelectionPopUp(available);
            using NumericPrompt valuePrompt = new NumericPrompt(0f, -100_000f, 100_000f);
            valuePrompt.DecimalPlaces = 1;
            valuePrompt.ValueStep = (decimal)0.1f;
            if (typeSelection.ShowDialog() == DialogResult.OK && valuePrompt.ShowDialog() == DialogResult.OK)
            {
                renderer3D1.SetPartOffset(typeSelection.SelectedItem, (float)valuePrompt.SelectedValue);
                ReloadOffsetList();
            }
        }

        private void removeOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (offsetListBox.SelectedItem is not SkinPartOffset offset)
                return;
            renderer3D1.SetPartOffset(offset.Type, 0f);
            ReloadOffsetList();
        }

        private void offsetListBox_DoubleClick(object sender, EventArgs e)
        {
            if (offsetListBox.SelectedItem is not SkinPartOffset offset)
                return;

            using NumericPrompt valuePrompt = new NumericPrompt(offset.Value, -100_000f, 100_000f);
            valuePrompt.ToolTipText = "Set new Value for " + offset.Type;
            valuePrompt.DecimalPlaces = 1;
            valuePrompt.ValueStep = (decimal)0.1f;
            if (valuePrompt.ShowDialog() == DialogResult.OK)
            {
                renderer3D1.SetPartOffset(offset.Type, (float)valuePrompt.SelectedValue);
                ReloadOffsetList();
            }
        }

        private void skinPartListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (skinPartListBox.IndexFromPoint(e.X, e.Y) == -1)
                ClearSelection();
        }

        private void ClearSelection()
        {
            skinPartListBox.ClearSelected();
            renderer3D1.SelectedIndex = skinPartListBox.SelectedIndex;
        }

        private void skinAnimateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            renderer3D1.Animate = skinAnimateCheckBox.Checked;
        }
    }
}