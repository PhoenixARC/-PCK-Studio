using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using PckStudio.Controls;
using PckStudio.Properties;
using PckStudio.Forms.Additional_Popups;

using PckStudio.Core.Skin;
using PckStudio.Core.Extensions;
using PckStudio.Interfaces;
using PckStudio.ModelSupport;
using PckStuido.ModelSupport.Extension;

namespace PckStudio.Forms.Editor
{
    public partial class CustomSkinEditor : EditorForm<Skin>
    {
        private const float MAX_OFFSET = 100_000f;
        private Random _rng;
        private readonly Image _cape;
        private bool _inflateOverlayParts;
        private bool _allowInflate;

        private BindingSource _skinPartListBindingSource;
        private BindingSource _skinOffsetListBindingSource;

        private Core.App.SettingsManager _settingsManager;

        private static GraphicsConfig _graphicsConfig = GraphicsConfig.PixelPerfect();

        private CustomSkinEditor() : this(null, null, null)
        { }

        public CustomSkinEditor(Skin skin, Image cape, ISaveContext<Skin> saveContext, bool inflateOverlayParts = false, bool allowInflate = false)
            : base(skin, saveContext)
        {
            InitializeComponent();
            InitializeRenderSettings();
            _rng = new Random();
            _skinPartListBindingSource = new BindingSource(renderer3D1.ModelData, null);
            skinPartListBox.DataSource = _skinPartListBindingSource;
            skinPartListBox.DisplayMember = "Type";
            _allowInflate = allowInflate;
            _cape = cape;
            _inflateOverlayParts = inflateOverlayParts;
        }

        private void InitializeRenderSettings()
        {
            _settingsManager = Core.App.SettingsManager.CreateSettings();
            _settingsManager.AddSetting("shouldAnimate"  , true , "Animate skin"                   , state => renderer3D1.Animate = state);
            _settingsManager.AddSetting("lockMouse"      , true , "Lock mouse when paning/rotating", state => renderer3D1.LockMousePosition = state);
            _settingsManager.AddSetting("showGuidelines" , false, "Show guidelines"                , state => renderer3D1.ShowGuideLines = state);
            _settingsManager.AddSetting("showBoundingBox", false, "Show Bounding Box"              , state => renderer3D1.ShowBoundingBox = state);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;
            renderer3D1.GuideLineColor = Color.LightCoral;
            skinNameLabel.Text = EditorValue.MetaData.Name;
            if (EditorValue.HasCape)
                renderer3D1.CapeTexture = _cape;
            LoadModelData();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.A)
            {
                using var animeditor = new ANIMEditor(EditorValue.Anim);
                if (animeditor.ShowDialog() == DialogResult.OK)
                {
                    renderer3D1.ANIM = EditorValue.Anim = animeditor.ResultAnim;
                    skinPartListBox_SelectedIndexChanged(this, EventArgs.Empty);
                }
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void LoadModelData()
        {
            renderer3D1?.LoadSkin(EditorValue);

            if (EditorValue.Texture is not null)
            {
                renderer3D1.Texture = EditorValue.Texture;
            }

            if (EditorValue.Texture is null && renderer3D1.Texture is not null)
            {
                EditorValue.Texture = renderer3D1.Texture;
            }

            _skinOffsetListBindingSource = new BindingSource(renderer3D1.GetOffsets().ToArray(), null);
            offsetListBox.DataSource = _skinOffsetListBindingSource;
            offsetListBox.DisplayMember = "Type";
            offsetListBox.ValueMember = "Value";

            _skinPartListBindingSource.ResetBindings(false);
            _skinOffsetListBindingSource.ResetBindings(false);
        }

        private void GenerateUVTextureMap(SkinBOX skinBox)
        {
            if (EditorValue?.Texture is null)
            {
                Trace.TraceWarning($"[{nameof(CustomSkinEditor)}@{nameof(GenerateUVTextureMap)}] Failed to generate uv for {skinBox}. Reason: Model.Texture was null");
                return;
            }
            using (Graphics graphics = Graphics.FromImage(EditorValue.Texture))
            {
                graphics.ApplyConfig(_graphicsConfig);
                int argb = _rng.Next(unchecked((int)0xFF000000), -1);
                var color = Color.FromArgb(argb);
                Brush brush = new SolidBrush(color);
                graphics.FillPath(brush, skinBox.GetUVGraphicsPath());
            }
            renderer3D1.Texture = EditorValue.Texture;
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var boxEditor = new BoxEditor(SkinBOX.DefaultHead, _allowInflate);
            if (boxEditor.ShowDialog() == DialogResult.OK)
            {
                SkinBOX newBox = boxEditor.Result;
                renderer3D1.ModelData.Add(newBox);
                EditorValue.Model.AdditionalBoxes.Add(newBox);
                _skinPartListBindingSource.ResetBindings(false);
                if (generateTextureCheckBox.Checked)
                    GenerateUVTextureMap(newBox);
            }
        }

        private void exportTextureButton_Click(object sender, EventArgs e)
        {
            if (EditorValue?.Texture is null)
            {
                Trace.TraceWarning($"[{nameof(CustomSkinEditor)}@{nameof(exportTextureButton_Click)}] Failed to export texture. Reason: skin.Model.Texture was null");
                return;
            }
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image Files | *.png";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                EditorValue.Texture.Save(saveFileDialog.FileName, ImageFormat.Png);
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
            EditorValue.Model.AdditionalBoxes.Clear();
            EditorValue.Model.AdditionalBoxes.AddRange(renderer3D1.ModelData);
            EditorValue.Model.PartOffsets.Clear();
            EditorValue.Model.PartOffsets.AddRange(renderer3D1.GetOffsets());
            // just in case they're not the same instance
            EditorValue.Anim = renderer3D1.ANIM;
            DialogResult = DialogResult.OK;
        }

        private void exportSkinButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Model File";
            saveFileDialog.Filter = SkinModelImporter.Default.SupportedModelFileFormatsFilter;
            saveFileDialog.FileName = EditorValue.MetaData.Name.TrimEnd(new char[] { '\n', '\r' }).Replace(' ', '_');
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                SkinModelImporter.Default.Export(saveFileDialog.FileName, EditorValue.GetModelInfo());
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
                    EditorValue.SetModelInfo(modelInfo);
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
                EditorValue.Model.AdditionalBoxes.Add(clone);
                _skinPartListBindingSource.ResetBindings(false);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                renderer3D1.ModelData.Remove(box);
                EditorValue.Model.AdditionalBoxes.Remove(box);
                _skinPartListBindingSource.ResetBindings(false);
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
            uvPictureBox.Image = EditorValue.Texture = texture;
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
                    _skinPartListBindingSource.ResetItem(skinPartListBox.SelectedIndex);
                }
            }
        }

        // TODO: fixed outline rendering
        private void skinPartListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int scale = 1;
            renderer3D1.SelectedIndices = skinPartListBox.SelectedIndices.Cast<int>().ToArray();
            StringBuilder uv_sb = new StringBuilder();
            StringBuilder size_sb = new StringBuilder();
            StringBuilder pos_sb = new StringBuilder();
            foreach (SkinBOX b in skinPartListBox.SelectedItems.Cast<SkinBOX>())
            {
                uv_sb.Append(b.Uv);
                uv_sb.Append(", ");
                size_sb.Append(b.Size);
                size_sb.Append(", ");
                pos_sb.Append(b.Position);
                pos_sb.Append(", ");
            }

            uvLabel.Text = $"UV: {uv_sb}";
            sizeLabel.Text = $"Size: {size_sb}";
            positionLabel.Text = $"Position: {pos_sb}";

            // TODO: highlight all selected boxes
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {

                Image uvArea = EditorValue.Texture.GetArea(Rectangle.Truncate(new RectangleF(box.Uv.X, box.Uv.Y, box.Size.X * 2 + box.Size.Z * 2, box.Size.Z + box.Size.Y)));

                Bitmap refImg = new Bitmap(1, 1);

                using (var g = Graphics.FromImage(refImg))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(uvArea, new Rectangle(0, 0, 1, 1));
                }

                Color avgColor = refImg.GetPixel(0, 0);
                renderer3D1.HighlightlingColor = avgColor.Inversed();

                Size scaleSize = new Size(EditorValue.Texture.Width * scale, EditorValue.Texture.Height * scale);
                uvPictureBox.Image = new Bitmap(scaleSize.Width, scaleSize.Height);
                using (Graphics g = Graphics.FromImage(uvPictureBox.Image))
                {
                    GraphicsPath graphicsPath = box.GetUVGraphicsPath(new System.Numerics.Vector2(scaleSize.Width * renderer3D1.TillingFactor.X, scaleSize.Height * renderer3D1.TillingFactor.Y));
                    var brush = new SolidBrush(Color.FromArgb(127, avgColor.GreyScaled()));
                    g.ApplyConfig(_graphicsConfig);
                    g.DrawImage(EditorValue.Texture, new Rectangle(Point.Empty, scaleSize), new Rectangle(Point.Empty, EditorValue.Texture.Size), GraphicsUnit.Pixel);
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
            _skinOffsetListBindingSource = new BindingSource(renderer3D1.GetOffsets().ToArray(), null);
            offsetListBox.DataSource = _skinOffsetListBindingSource;
            _skinOffsetListBindingSource.ResetBindings(false);
        }

        private void addOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var offsets = renderer3D1.GetOffsets().Select(offset => offset.Type).ToList();
            string[] available = SkinPartOffset.ValidModelOffsetTypes.Where(s => !offsets.Contains(s)).ToArray();
            using ItemSelectionPopUp typeSelection = new ItemSelectionPopUp(available);
            using NumericPrompt valuePrompt = new NumericPrompt(0f, -MAX_OFFSET, MAX_OFFSET);
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

            using NumericPrompt valuePrompt = new NumericPrompt(offset.Value, -MAX_OFFSET, MAX_OFFSET);
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
            uvPictureBox.Image = EditorValue.Texture;
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

        private void renderSettingsButton_Click(object sender, EventArgs e)
        {
            using AppSettingsForm settingsForm = new AppSettingsForm("Render Settings", _settingsManager.GetSettings());
            settingsForm.ShowDialog();
        }

        private string SanitizeModelFilename(in string modelFilename)
        {
            return string.IsNullOrWhiteSpace(modelFilename) ? "template" : modelFilename.TrimEnd(new char[] { '\n', '\r' }).Replace(' ', '_');
        }

        private void exportTemplateButton_Click(object sender, EventArgs e)
        {
            Image templateTexture = Resources.classic_template;
            SkinAnimMask templateAnimMask = SkinAnimMask.RESOLUTION_64x64;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Template Model";
            saveFileDialog.Filter = SkinModelImporter.Default.SupportedModelFileFormatsFilter;
            saveFileDialog.FileName = SanitizeModelFilename(EditorValue.MetaData.Name);
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