using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using Newtonsoft.Json;
using MetroFramework.Forms;

using OMI.Formats.Pck;

using PckStudio.Internal;
using PckStudio.Extensions;
using PckStudio.IO.CSMB;
using PckStudio.FileFormats;

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
        //private BindingSource skinOffsetListBindingSource;

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
            if (_skin.Texture is not null)
            {
                renderer3D1.Texture = _skin.Texture;
            }
            renderer3D1.ANIM = _skin.ANIM;
            renderer3D1.OutlineColor = Color.DarkSlateBlue;
            LoadModelData();
        }

        private void LoadModelData()
        {
            skinNameLabel.Text = _skin.Name;
            var boxProperties = _skin.AdditionalBoxes;
            var offsetProperties = _skin.PartOffsets;

            if (_skin.HasCape)
                renderer3D1.CapeTexture = _skin.CapeTexture;

            foreach (SkinBOX box in boxProperties)
            {
                renderer3D1.ModelData.Add(box);
            }
            foreach (SkinPartOffset offset in offsetProperties)
            {
                renderer3D1.SetPartOffset(offset);
            }

            //skinOffsetListBindingSource = new BindingSource(renderer3D1.offsetSpecificMeshStorage, null);
            //offsetListBox.DataSource = skinOffsetListBindingSource;
            //offsetListBox.DisplayMember = "Value";

            skinPartListBindingSource.ResetBindings(false);
            //skinOffsetListBindingSource.ResetBindings(false);
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

        private void buttonEXPORT_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image Files | *.png";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _skin.Texture.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void buttonIMPORT_Click(object sender, EventArgs e)
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
            //Debug.Fail("TODO: Implement");
            _skin.AdditionalBoxes.Clear();
            _skin.AdditionalBoxes.AddRange(renderer3D1.ModelData);

            // TODO: Get part offset list/IEnumerable from renderer
            //_skin.PartOffsets.Clear();
            //_skin.PartOffsets.AddRange();

            //_previewImage = renderer3D1.GetThumbnail();

            DialogResult = DialogResult.OK;
        }

        // TODO
        private void buttonExportModel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Model File";
            saveFileDialog.Filter = "Custom Skin Model File (*.csm,*.CSM)|*.csm;*.CSM|" +
                                    "Custom Skin Model Binary File (*.csmb)|*.csmb|";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
        }

        [Obsolete("Kept for backwards compatibility.")]
        private void importCustomSkinButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Model File";
            openFileDialog.Filter = "Custom Skin Model File (*.csm,*.CSM)|*.csm;*.CSM|" +
                                    "Custom Skin Model Binary File (*.csmb)|*.csmb|";
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileExtension = Path.GetExtension(openFileDialog.FileName);
                if (fileExtension == ".csmb")
                {
                    //var reader = new CSMBFileReader();
                    //CSMBFile csmbFile = reader.FromFile(openFileDialog.FileName);
                    //LoadCsmb(csmbFile);
                }
            }
        }

        private void LoadCsmb(CSMBFile csmbFile)
        {
            //renderer3D1.ModelData.Clear();
            //foreach (var part in csmbFile.Parts)
            //{
            //    renderer3D1.ModelData.Add(part);
            //}

            //renderer3D1.ResetOffsets();
            //foreach (var offset in csmbFile.Offsets)
            //{
            //    renderer3D1.SetPartOffset(offset);
            //}
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

        [Obsolete("Will be removed")]
        public string JSONToCSM(string inputJson)
        {
            CSMJObject jsonDe = JsonConvert.DeserializeObject<CSMJObject>(inputJson);
            StringBuilder sb = new StringBuilder();
            foreach (CSMJObjectGroup group in jsonDe.Groups)
            {
                string PARENT = group.Name;
                foreach (int i in group.children)
                {
                    CSMJObjectElement element = jsonDe.Elements[i];
                    string name = element.Name;
                    float PosX = element.from[0] + group.origin[0];
                    float PosY = element.from[1] + group.origin[1];
                    float PosZ = element.from[2] + group.origin[2];
                    float SizeX = element.to[0] - element.from[0];
                    float SizeY = element.to[1] - element.from[1];
                    float SizeZ = element.to[2] - element.from[2];
                    float U = 0;
                    float V = 0;

                    sb.AppendLine(name + "\n" + PARENT + "\n" + name + "\n" + PosX + "\n" + PosY + "\n" + PosZ + "\n" + SizeX + "\n" + SizeY + "\n" + SizeZ + "\n" + U + "\n" + V);
                }
            }
            return sb.ToString();
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

        private void skinPartListBox_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                deleteToolStripMenuItem_Click(sender, e);
        }
    }

    class CSMJObject
    {
        [JsonProperty("credit")]
        public string Credit { get; set; }

        [JsonProperty("texture_size")]
        public int[] TextureSize;

        [JsonProperty("elements")]
        public CSMJObjectElement[] Elements;

        [JsonProperty("groups")]
        public CSMJObjectGroup[] Groups;
    }
    
    class CSMJObjectElement
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public float[] from;
        public float[] to;
    }

    class CSMJObjectGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public float[] origin;
        public int[] children;
    }
}