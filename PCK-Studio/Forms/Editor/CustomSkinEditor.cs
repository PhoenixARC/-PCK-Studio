using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using MetroFramework.Forms;

using PckStudio.Internal;
using PckStudio.Extensions;
using PckStudio.Internal.Skin;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Properties;
using System.Collections.Generic;
using PckStudio.Internal.App;

namespace PckStudio.Forms.Editor
{
    public partial class CustomSkinEditor : MetroForm
    {
        public Skin ResultSkin => _skin;
        private Skin _skin;
        private Random rng;
        private bool _inflateOverlayParts;
        private bool _allowInflate;

        private BindingSource skinPartListBindingSource;
        private BindingSource skinOffsetListBindingSource;

        private SettingsManager _settingsManager;

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
            _settingsManager = SettingsManager.CreateSettings();
            _settingsManager.AddSetting("shouldAnimate"  , true , "Animate skin"                   , state => renderer3D1.Animate = state);
            _settingsManager.AddSetting("lockMouse"      , true , "Lock mouse when paning/rotating", state => renderer3D1.LockMousePosition = state);
            _settingsManager.AddSetting("showGuidelines" , false, "Show guidelines"                , state => renderer3D1.ShowGuideLines = state);
            _settingsManager.AddSetting("showArmor"      , false, "Show Armor"                     , state => renderer3D1.ShowArmor = state);
            _settingsManager.AddSetting("showBoundingBox", false, "Show Bounding Box"              , state => renderer3D1.ShowBoundingBox = state);
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
            renderer3D1.GuideLineColor = Color.LightCoral;
            framerateSlider_ValueChanged(this, EventArgs.Empty);
            skinNameLabel.Text = _skin.MetaData.Name;
            if (_skin.HasCape)
                renderer3D1.CapeTexture = _skin.CapeTexture;
            LoadModelData();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.A)
            {
                using var animeditor = new ANIMEditor(_skin.ANIM);
                if (animeditor.ShowDialog() == DialogResult.OK)
                {
                    renderer3D1.ANIM = _skin.ANIM = animeditor.ResultAnim;
                    skinPartListBox_SelectedIndexChanged(this, EventArgs.Empty);
                }
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void LoadModelData()
        {
            SkinModel modelInfo = _skin.Model;

            List<SkinBOX> boxProperties = modelInfo.AdditionalBoxes;
            List<SkinPartOffset> offsetProperties = modelInfo.PartOffsets;
            
            renderer3D1.ANIM = _skin.ANIM;

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

            if (_skin.Texture is not null)
            {
                renderer3D1.Texture = _skin.Texture;
            }

            if (_skin.Texture is null && renderer3D1.Texture is not null)
            {
                _skin.Texture = renderer3D1.Texture;
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
            if (_skin?.Texture is null)
            {
                Trace.TraceWarning($"[{nameof(CustomSkinEditor)}@{nameof(GenerateUVTextureMap)}] Failed to generate uv for {skinBox}. Reason: Model.Texture was null");
                return;
            }
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
            var boxEditor = new BoxEditor(SkinBOX.DefaultHead, _allowInflate);
            if (boxEditor.ShowDialog() == DialogResult.OK)
            {
                SkinBOX newBox = boxEditor.Result;
                renderer3D1.ModelData.Add(newBox);
                _skin.Model.AdditionalBoxes.Add(newBox);
                skinPartListBindingSource.ResetBindings(false);
                if (generateTextureCheckBox.Checked)
                    GenerateUVTextureMap(newBox);
            }
        }

        private void exportTextureButton_Click(object sender, EventArgs e)
        {
            if (_skin?.Texture is null)
            {
                Trace.TraceWarning($"[{nameof(CustomSkinEditor)}@{nameof(exportTextureButton_Click)}] Failed to export texture. Reason: skin.Model.Texture was null");
                return;
            }
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
            _skin.Model.AdditionalBoxes.Clear();
            _skin.Model.AdditionalBoxes.AddRange(renderer3D1.ModelData);
            _skin.Model.PartOffsets.Clear();
            _skin.Model.PartOffsets.AddRange(renderer3D1.GetOffsets());
            // just in case they're not the same instance
            _skin.ANIM = renderer3D1.ANIM;
            DialogResult = DialogResult.OK;
        }

        private void exportSkinButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Model File";
            saveFileDialog.Filter = SkinModelImporter.Default.SupportedModelFileFormatsFilter;
            saveFileDialog.FileName = _skin.MetaData.Name.TrimEnd(new char[] { '\n', '\r' }).Replace(' ', '_');
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                SkinModelImporter.Default.Export(saveFileDialog.FileName, _skin.GetModelInfo());
        }

        private void importSkinButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Model File";
            openFileDialog.Filter = SkinModelImporter.Default.SupportedModelFileFormatsFilter;
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SkinModelInfo modelInfo = SkinModelImporter.Default.Import(openFileDialog.FileName);
                if (modelInfo is not null)
                {
                    _skin.SetModelInfo(modelInfo);
                    LoadModelData();
                }
            }
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                SkinBOX clone = box;
                renderer3D1.ModelData.Add(clone);
                _skin.Model.AdditionalBoxes.Add(clone);
                skinPartListBindingSource.ResetBindings(false);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                renderer3D1.ModelData.Remove(box);
                _skin.Model.AdditionalBoxes.Remove(box);
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
                renderer3D1.GuideLineColor = colorDialog.Color;
                skinPartListBox_SelectedIndexChanged(sender, e);
            }
        }

        private void renderer3D1_TextureChanging(object sender, Rendering.TextureChangingEventArgs e)
        {
            Image texture = e.NewTexture;
            // Skins can only be a 1:1 ratio (base 64x64) or a 2:1 ratio (base 64x32)
            if (Settings.Default.ValidateImageDimension && texture.Width != texture.Height && texture.Height != texture.Width / 2)
            {
                e.Cancel = true;
                MessageBox.Show("The selected image does not suit a skin texture.", "Invalid image dimensions.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            uvPictureBox.Image = _skin.Texture = texture;
            textureSizeLabel.Text = $"{texture.Width}x{texture.Height}";
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

        // TODO: fixed outline rendering
        private void skinPartListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int scale = 1;
            renderer3D1.SelectedIndices = skinPartListBox.SelectedIndices.Cast<int>().ToArray();
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                uvLabel.Text = $"UV: {box.UV}";
                sizeLabel.Text = $"Size: {box.Size}";
                positionLabel.Text = $"Position: {box.Pos}";

                Image uvArea = _skin.Texture.GetArea(Rectangle.Truncate(new RectangleF(box.UV.X, box.UV.Y, box.Size.X * 2 + box.Size.Z * 2, box.Size.Z + box.Size.Y)));

                Bitmap refImg = new Bitmap(1, 1);

                using (var g = Graphics.FromImage(refImg))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(uvArea, new Rectangle(0, 0, 1, 1));
                }

                Color avgColor = refImg.GetPixel(0, 0);
                renderer3D1.HighlightlingColor = avgColor.Inversed();

                Size scaleSize = new Size(_skin.Texture.Width * scale, _skin.Texture.Height * scale);
                uvPictureBox.Image = new Bitmap(scaleSize.Width, scaleSize.Height);
                using (Graphics g = Graphics.FromImage(uvPictureBox.Image))
                {
                    GraphicsPath graphicsPath = box.GetUVGraphicsPath(new System.Numerics.Vector2(scaleSize.Width * renderer3D1.TillingFactor.X, scaleSize.Height * renderer3D1.TillingFactor.Y));
                    var brush = new SolidBrush(Color.FromArgb(127, avgColor.GreyScaled()));
                    g.ApplyConfig(_graphicsConfig);
                    g.DrawImage(_skin.Texture, new Rectangle(Point.Empty, scaleSize), new Rectangle(Point.Empty, _skin.Texture.Size), GraphicsUnit.Pixel);
                    g.FillPath(brush, graphicsPath);
                }
                uvPictureBox.Invalidate();
            }
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
            uvPictureBox.Image = _skin.Texture;
        }


        private void centerSelectionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            renderer3D1.CenterOnSelect = centerSelectionCheckbox.Checked;
        }

        private void generateUvTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skinPartListBox.SelectedItem is SkinBOX skinBox)
            {
                GenerateUVTextureMap(skinBox);
            }
        }

        private void framerateSlider_ValueChanged(object sender, EventArgs e)
        {
            renderer3D1.RefreshRate = framerateSlider.Value * 30 + 30;
            framerateLabel.Text = "FPS: " + renderer3D1.RefreshRate.ToString();
        }

        private void renderSettingsButton_Click(object sender, EventArgs e)
        {
            using AppSettingsForm settingsForm = new AppSettingsForm("Render Settings", _settingsManager.GetSettings());
            settingsForm.ShowDialog();
        }

        private void exportTemplateButton_Click(object sender, EventArgs e)
        {
            Image templateTexture = Resources.classic_template;
            string templateFilename = "template";
            SkinAnimMask templateAnimMask = SkinAnimMask.RESOLUTION_64x64;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Template Model";
            saveFileDialog.Filter = SkinModelImporter.Default.SupportedModelFileFormatsFilter;
            saveFileDialog.FileName = templateFilename.TrimEnd(new char[] { '\n', '\r' }).Replace(' ', '_');
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SkinModelInfo modelInfo = new SkinModelInfo(templateTexture, templateAnimMask, new SkinModel());
                SkinModelImporter.Default.Export(saveFileDialog.FileName, modelInfo);
            }
        }

        private void animEditorButton_Click(object sender, EventArgs e)
        {
            ProcessDialogKey(Keys.A);
        }
    }
}