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
        private Image _previewImage;
        public Image PreviewImage => _previewImage;

        private PckFileData _file;
        private Random rng;

        private BindingSource skinPartListBindingSource;
        private BindingSource skinOffsetListBindingSource;

        private static GraphicsConfig _graphicsConfig = new GraphicsConfig()
        {
            InterpolationMode = InterpolationMode.NearestNeighbor,
            PixelOffsetMode = PixelOffsetMode.HighQuality,
        };

        public CustomSkinEditor(PckFileData file)
        {
            InitializeComponent();
            _file = file;
            rng = new Random();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            renderer3D1.InitializeGL();
            renderer3D1.OutlineColor = Color.DarkSlateBlue;
            if (_file.Size > 0)
            {
                renderer3D1.Texture = _file.GetTexture();
            }
            LoadModelData(_file.Properties);
        }

        private void LoadModelData(PckFileProperties properties)
        {
            renderer3D1.ANIM = properties.GetPropertyValue("ANIM", SkinANIM.FromString);
            var boxProperties = properties.GetProperties("BOX");
            var offsetProperties = properties.GetProperties("OFFSET");

            skinNameLabel.Text = properties.HasProperty("DISPLAYNAME") ? properties.GetPropertyValue("DISPLAYNAME") : "";

            Array.ForEach(boxProperties, kv => renderer3D1.ModelData.Add(SkinBOX.FromString(kv.Value)));
            Array.ForEach(offsetProperties, kv => renderer3D1.SetPartOffset(SkinPartOffset.FromString(kv.Value)));

            skinPartListBindingSource = new BindingSource(renderer3D1.ModelData, null);
            skinPartListBox.DataSource = skinPartListBindingSource;
            skinPartListBox.DisplayMember = "Type";

            skinOffsetListBindingSource = new BindingSource(renderer3D1, null);
            offsetListBox.DataSource = skinOffsetListBindingSource;
        }

        private void GenerateUVTextureMap(SkinBOX skinBox)
        {
            using (Graphics graphics = Graphics.FromImage(uvPictureBox.BackgroundImage))
            {
                graphics.ApplyConfig(_graphicsConfig);
                int argb = rng.Next(unchecked((int)0xFF000000), -1);
                var color = Color.FromArgb(argb);
                Brush brush = new SolidBrush(color);
                graphics.FillPath(brush, skinBox.GetUVGraphicsPath());
            }
            uvPictureBox.Invalidate();
            renderer3D1.Texture = uvPictureBox.BackgroundImage;
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var boxEditor = new BoxEditor(SkinBOX.Empty, false);
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
                uvPictureBox.BackgroundImage.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void buttonIMPORT_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG Image Files | *.png";
            openFileDialog.Title = "Select Skin Texture";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var img = Image.FromFile(openFileDialog.FileName))
				{
                    renderer3D1.Texture = img;
                }
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            _file.Properties.RemoveAll(kv => kv.Key == "BOX");
            foreach (var part in renderer3D1.ModelData)
            {
                _file.Properties.Add("BOX", part);
            }
            _previewImage = renderer3D1.GetThumbnail();

            DialogResult = DialogResult.OK;
        }

        private void buttonExportModel_Click(object sender, EventArgs e)
        {
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "Custom Skin Model File | *.CSM";
            //if (saveFileDialog.ShowDialog() != DialogResult.OK)
            //    return;
            //string contents = "";
            //foreach (ListViewItem listViewItem in listViewBoxes.Items)
            //{
            //    string str = "";
            //    foreach (ListViewItem.ListViewSubItem subItem in listViewItem.SubItems)
            //    {
            //        if (subItem.Text != "unchecked")
            //            str = str + subItem.Text + Environment.NewLine;
            //    }
            //    contents += (listViewItem.Text + Environment.NewLine + listViewItem.Tag) + Environment.NewLine + str;
            //}

            //File.WriteAllText(saveFileDialog.FileName, contents);
        }

        [Obsolete("Kept for backwards compatibility, remove later.")]
        private void importCustomSkinButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Custom Skin Model File (*.csm,*.CSM)|*.csm;*.CSM|Custom Skin Model Binary File (*.csmb)|*.csmb|JSON Model File(*.json)|*.JSON;*.json";
            openFileDialog.Title = "Select Model File";
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

        private void generateModel_FormClosing(object sender, FormClosingEventArgs e)
        { 
            
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
                MessageBox.Show("Invalid image dimensions.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            generateTextureCheckBox.Checked = false;
            uvPictureBox.BackgroundImage = img;
        }

        private void skinPartListBox_DoubleClick(object sender, EventArgs e)
        {
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                var boxEditor = new BoxEditor(box, false);
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
                uvPictureBox.Image = new Bitmap(uvPictureBox.BackgroundImage.Width * scale, uvPictureBox.BackgroundImage.Height * scale);
                using (Graphics g = Graphics.FromImage(uvPictureBox.Image))
                {
                    float lineWidth = ((uvPictureBox.BackgroundImage.Width / renderer3D1.TextureSize.Width) + (uvPictureBox.BackgroundImage.Height / renderer3D1.TextureSize.Height)) / 2f;
                    GraphicsPath graphicsPath = box.GetUVGraphicsPath(
                        new System.Numerics.Vector2(
                            scale * renderer3D1.TillingFactor.X * uvPictureBox.BackgroundImage.Width,
                            scale * renderer3D1.TillingFactor.Y * uvPictureBox.BackgroundImage.Height
                            )
                        );
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