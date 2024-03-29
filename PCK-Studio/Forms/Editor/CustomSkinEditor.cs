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
using PckStudio.IO.PSM;
using PckStudio.Internal.FileFormats;
using System.Linq;
using PckStudio.Forms.Additional_Popups;

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

        private struct FileDialogOption
        {
            public readonly string FilterDescription;
            public readonly string FilterPattern;
            public string Filter => $"{FilterDescription}|{FilterPattern}";

            public FileDialogOption(string filterDescription, string filterPattern)
            {
                FilterDescription = filterDescription;
                FilterPattern = filterPattern;
            }

            public override string ToString()
            {
                return Filter;
            }
        }

        private readonly FileDialogOption[] fileFilters = 
        [
            new ("Pck skin model(*.psm)", "*.psm")
        ];

        private string skinModelFileFilters => string.Join("|", fileFilters);

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
            skinNameLabel.Text = skin.Name;
            var boxProperties = skin.AdditionalBoxes;
            var offsetProperties = skin.PartOffsets;
            
            renderer3D1.ANIM = skin.ANIM;

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
            if (skin.Texture is not null)
            {
                renderer3D1.Texture = skin.Texture;
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
            using (Graphics graphics = Graphics.FromImage(_skin.Texture))
            {
                graphics.ApplyConfig(_graphicsConfig);
                int argb = rng.Next(unchecked((int)0xFF000000), -1);
                var color = Color.FromArgb(argb);
                Brush brush = new SolidBrush(color);
                graphics.FillPath(brush, skinBox.GetUVGraphicsPath());
            }
            renderer3D1.Texture = _skin.Texture;
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
                _skin.Texture.Save(saveFileDialog.FileName, ImageFormat.Png);
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
            _skin.AdditionalBoxes.Clear();
            _skin.AdditionalBoxes.AddRange(renderer3D1.ModelData);
            _skin.PartOffsets.Clear();
            _skin.PartOffsets.AddRange(renderer3D1.GetOffsets());
            // just in case they're not the same instance
            _skin.ANIM = renderer3D1.ANIM;
            DialogResult = DialogResult.OK;
        }

        private void exportSkinButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Model File";
            saveFileDialog.Filter = skinModelFileFilters;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string fileExtension = Path.GetExtension(saveFileDialog.FileName);
            switch (fileExtension)
            {
                case ".psm":
                    var writer = new PSMFileWriter(PSMFile.FromSkin(_skin));
                    writer.WriteToFile(saveFileDialog.FileName);
                    break;
                default:
                    break;
            }
        }

        private void importSkinButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Model File";
            openFileDialog.Filter = skinModelFileFilters;
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileExtension = Path.GetExtension(openFileDialog.FileName);
                switch (fileExtension)
                {
                    case ".psm":
                        var reader = new PSMFileReader();
                        PSMFile csmbFile = reader.FromFile(openFileDialog.FileName);
                        _skin.ANIM = csmbFile.SkinANIM;
                        _skin.AdditionalBoxes.Clear();
                        _skin.PartOffsets.Clear();
                        _skin.AdditionalBoxes.AddRange(csmbFile.Parts);
                        _skin.PartOffsets.AddRange(csmbFile.Offsets);
                        LoadModelData(_skin);
                        break;
                    default:
                        break;
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
            uvPictureBox.Image = _skin.Texture = img;
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
                Size scaleSize = new Size(_skin.Texture.Width * scale, _skin.Texture.Height * scale);
                uvPictureBox.Image = new Bitmap(scaleSize.Width, scaleSize.Height);
                using (Graphics g = Graphics.FromImage(uvPictureBox.Image))
                {
                    float lineWidth = ((_skin.Texture.Width / renderer3D1.TextureSize.Width) + (_skin.Texture.Height / renderer3D1.TextureSize.Height)) / 2f;
                    GraphicsPath graphicsPath = box.GetUVGraphicsPath(new System.Numerics.Vector2(scaleSize.Width * renderer3D1.TillingFactor.X, scaleSize.Height * renderer3D1.TillingFactor.Y));
                    g.ApplyConfig(_graphicsConfig);
                    g.DrawImage(_skin.Texture, new Rectangle(Point.Empty, scaleSize), new Rectangle(Point.Empty, _skin.Texture.Size), GraphicsUnit.Pixel);
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
            if (e.KeyCode == Keys.Delete)
                deleteToolStripMenuItem_Click(sender, e);
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
    }
}