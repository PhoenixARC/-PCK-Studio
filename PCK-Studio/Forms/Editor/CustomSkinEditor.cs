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
using PckStudio.External.Format;
using Newtonsoft.Json;
using System.Numerics;
using PckStudio.Rendering;
using System.Diagnostics;

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

        private struct FileDialogFilter
        {
            public readonly string Description;
            public readonly string Pattern;

            public FileDialogFilter(string description, string pattern)
            {
                Description = description;
                Pattern = pattern;
            }

            public override string ToString()
            {
                return $"{Description}|{Pattern}";
            }
        }

        private readonly FileDialogFilter[] fileFilters = 
        [
            new ("Pck skin model(*.psm)", "*.psm"),
            new ("Block bench model(*.bbmodel)", "*.bbmodel")
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
                    case ".bbmodel":
                        BlockBenchModel blockBenchModel = JsonConvert.DeserializeObject<BlockBenchModel>(File.ReadAllText(openFileDialog.FileName));
                        _skin.AdditionalBoxes.Clear();
                        _skin.PartOffsets.Clear();

                        if (blockBenchModel.Textures.IndexInRange(0))
                            _skin.Texture = blockBenchModel.Textures[0].GetTexture();

                        // TODO: clean this up -miku
                        _skin.ANIM.SetFlag(SkinAnimFlag.RESOLUTION_64x64, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.SLIM_MODEL, false);
                        _skin.ANIM.SetFlag(SkinAnimFlag.HEAD_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.BODY_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.LEFT_ARM_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.LEFT_LEG_DISABLED, true);
                        _skin.ANIM.SetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED, true);

                        foreach (JToken token in blockBenchModel.Outliner)
                        {
                            if (token.Type == JTokenType.String && Guid.TryParse((string)token, out Guid tokenGuid))
                            {
                                Element element = blockBenchModel.Elements.First(e => e.Uuid.Equals(tokenGuid));
                                if (!SkinBOX.IsValidType(element.Name))
                                    continue;
                                LoadElement(element.Name, element);
                                continue;
                            }
                            if (token.Type == JTokenType.Object)
                            {
                                Outline outline = token.ToObject<Outline>();
                            string type = outline.Name;
                            if (!SkinBOX.IsValidType(type))
                                continue;
                                ReadOutliner(token, type, blockBenchModel.Elements);
                            }
                        }

                        LoadModelData(_skin);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ReadOutliner(JToken token, string type, IReadOnlyCollection<Element> elements)
        {
            if (TryReadElement(token, type, elements))
                return;

            if (token.Type == JTokenType.Object)
            {
                Outline outline = token.ToObject<Outline>();
                foreach (JToken childToken in outline.Children)
                {
                    ReadOutliner(childToken, type, elements);
                }
            }
        }

        private bool TryReadElement(JToken token, string type, IReadOnlyCollection<Element> elements)
        {
            if (token.Type == JTokenType.String && Guid.TryParse((string)token, out Guid tokenGuid))
            {
                Element element = elements.First(e => e.Uuid.Equals(tokenGuid));
                LoadElement(type, element);
                return true;
            }
            return false;
        }

        private bool LoadElement(string boxType, Element element)
                            {
            if (!element.UseBoxUv || !element.IsVisibile)
                return false;

                                //Debug.WriteLine($"{type} {element.Name}({element.Uuid})");
                                BoundingBox boundingBox = new BoundingBox(element.From.ToOpenTKVector(), element.To.ToOpenTKVector());
                                Vector3 pos = boundingBox.Start.ToNumericsVector();
                                Vector3 size = boundingBox.Volume.ToNumericsVector();
                                Vector2 uv = element.UvOffset;
            pos = TranslatePosition(boxType, pos, size, new Vector3(1, 1, 0));
            //Debug.WriteLine(pos);

            // IMPROVMENT: detect default body parts and toggle anim flag instead of adding box data -miku

            var box = new SkinBOX(boxType, pos, size, uv);
            if (box.IsBasePart() && ((boxType == "HEAD" && element.Inflate == 0.5f) || (element.Inflate == 0.25f)))
                box.Type = box.GetOverlayType();

            _skin.AdditionalBoxes.Add(box);
            return true;
        }

        /// <summary>
        /// Translates coordinate unit system into our coordinate system
        /// </summary>
        /// <param name="boxType">See <see cref="SkinBOX.BaseTypes"/> and <see cref="SkinBOX.OverlayTypes"/>.</param>
        /// <param name="origin">Position/Origin of the Object(Cube).</param>
        /// <param name="size">The Size of the Object(Cube).</param>
        /// <param name="translationUnit">Describes what axises need translation.</param>
        /// <returns>The translated position</returns>
        private Vector3 TranslatePosition(string boxType, Vector3 origin, Vector3 size, Vector3 translationUnit)
        {
            // The translation unit describes what axises needd to be swap
            // Example:
            //      translation unit = (1, 0, 0) => This translation unit would swap ONLY the X axis
            translationUnit = Vector3.Clamp(translationUnit, Vector3.Zero, Vector3.One);
            // To better understand see:
            // https://sharplab.io/#v2:C4LgTgrgdgNAJiA1AHwAICYCMBYAUKgBgAJVMA6AOQgFsBTMASwGMBnAbj1QGYT0iBhIgG88RMb3SjxI3OLlEAbgEMwRBlAAOEYEQC8RKLQDuRAGq0mwAPZguACkwwijogQCUHWfLHLVtAB4aFsC0cHoGxmbBNvYAtC7xTpgeUt6+RGC0LOEAKmBKUCwAYjbU/FY2cOpKISx26lrAKV7epACcdpkszd5i7Z1ZevoBQZahPeIAvqlEM9wkmABsUZYxRHkFxaXlldW1duartmqa2m4zMr2KKhmD+ofWtmT8ADZK1Br1p8BODzFkAC16FZftEngB5QwTbxdIgAKn06E8V1hsXuYK4ZEhtGRvVQAHYiLEurixNNcJMgA
            Vector3 transformUnit = -((translationUnit * 2) - Vector3.One);

            Vector3 pos = origin;
            // The next line essentialy does uses the fomular below just on all axis.
            // x = -(pos.x + size.x)
                                pos *= transformUnit;
            pos -= size * translationUnit;
            // Skin Renderer (and Game) specific offset value.
                                pos.Y += 24f;

            Vector3 translation = renderer3D1.GetTranslation(boxType).ToNumericsVector();
            Vector3 pivot = renderer3D1.GetPivot(boxType).ToNumericsVector();
                                
            // This will cancel out the part specific translation and pivot.
                                pos += translation * -Vector3.UnitX - pivot * Vector3.UnitY;

            return pos;
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