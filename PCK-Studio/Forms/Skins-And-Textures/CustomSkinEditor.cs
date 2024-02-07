﻿using System;
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
using PckStudio.Forms.Editor;

namespace PckStudio.Forms
{
    public partial class CustomSkinEditor : MetroForm
    {
        private Image _previewImage;
        public Image PreviewImage => _previewImage;

        private PckFileData _file;

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
            if (_file.Size > 0)
            {
                uvPictureBox.BackgroundImage = renderer3D1.Texture = _file.GetTexture();
            }
            //comboParent.Items.AddRange(ValidModelBoxTypes);
            LoadModelData(file.Properties);
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

            skinOffsetListBindingSource = new BindingSource(renderer3D1.PartOffsets, null);
            offsetListBox.DataSource = skinOffsetListBindingSource;
            //offsetListBox.DisplayMember = ""
        }

        // TODO
        private void GenerateUVTextureMap()
        {
            Random rng = new Random();
            using (Graphics graphics = Graphics.FromImage(uvPictureBox.Image))
            {
                graphics.ApplyConfig(_graphicsConfig);
                //foreach (var part in modelBoxes)
                //{
                //    float width = part.Size.X * 2;
                //    float height = part.Size.Y * 2;
                //    float length = part.Size.Z * 2;
                //    float u = part.UV.X * 2;
                //    float v = part.UV.Y * 2;
                //    int argb = rng.Next(-16777216, -1); // 0xFF000000 - 0xFFFFFFFF
                //    var color = Color.FromArgb(argb);
                //    Brush brush = new SolidBrush(color);
                //    graphics.FillRectangle(brush, u + length, v, width, length);
                //    graphics.FillRectangle(brush, u + length + width, v, width, length);
                //    graphics.FillRectangle(brush, u, length + v, length, height);
                //    graphics.FillRectangle(brush, u + length, v + length, width, height);
                //    graphics.FillRectangle(brush, u + length + width, v + length, width, height);
                //    graphics.FillRectangle(brush, u + length + width * 2, v + length, length, height);
                //}
            }
            uvPictureBox.Invalidate();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var boxEditor = new BoxEditor(SkinBOX.Empty, false);
            if (boxEditor.ShowDialog() == DialogResult.OK)
            {
                renderer3D1.ModelData.Add(boxEditor.Result);
                skinPartListBindingSource.ResetBindings(false);
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

            if (openFileDialog.ShowDialog() == DialogResult.OK) // skins can only be a 1:1 ratio (base 64x64) or a 2:1 ratio (base 64x32)
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
        private void buttonImportModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Custom Skin Model File | *.CSM";
            openFileDialog.Title = "Select Custom Skin Model File";
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                renderer3D1.ModelData.Clear();
                StreamReader reader = new StreamReader(openFileDialog.FileName);
                while (!reader.EndOfStream)
                {
                    reader.ReadLine();
                    string part = reader.ReadLine();
                    reader.ReadLine();
                    var PosX = reader.ReadLine();
                    var PosY = reader.ReadLine();
                    var PosZ = reader.ReadLine();
                    var SizeX = reader.ReadLine();
                    var SizeY = reader.ReadLine();
                    var SizeZ = reader.ReadLine();
                    var UvX = reader.ReadLine();
                    var UvY = reader.ReadLine();
                    renderer3D1.ModelData.Add(SkinBOX.FromString($"{part} {PosX} {PosY} {PosZ} {SizeX} {SizeY} {SizeZ} {UvX} {UvY}"));
                }
                skinPartListBindingSource.ResetBindings(false);
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

        private void generateModel_FormClosing(object sender, FormClosingEventArgs e)
        { 
            /*if (MessageBox.Show("You done here?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = false;*/
        }

        private void OpenJSONButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Model File | *.JSON";
            openFileDialog.Title = "Select JSON Model File";
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //listViewBoxes.Items.Clear();
                string str1 = JSONToCSM(File.ReadAllText(openFileDialog.FileName));
                int x = 0;
                foreach (string str2 in str1.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    ++x;
                int y = x / 11;
                ListView listView = new ListView();
                int num3 = 0;
                do
                {
                    listView.Items.Add("BOX");
                    ++num3;
                }
                while (num3 < y);


                foreach (ListViewItem current in listView.Items)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    int num4 = 0;
                    foreach (string text in str1.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        ++num4;
                        if (num4 == 1 + 11 * current.Index)
                            listViewItem.Text = text;
                        else if (num4 == 2 + 11 * current.Index)
                            listViewItem.Tag = text;
                        else if (num4 == 4 + 11 * current.Index)
                            listViewItem.SubItems.Add(text);
                        else if (num4 == 5 + 11 * current.Index)
                            listViewItem.SubItems.Add(text);
                        else if (num4 == 6 + 11 * current.Index)
                            listViewItem.SubItems.Add(text);
                        else if (num4 == 7 + 11 * current.Index)
                            listViewItem.SubItems.Add(text);
                        else if (num4 == 8 + 11 * current.Index)
                            listViewItem.SubItems.Add(text);
                        else if (num4 == 9 + 11 * current.Index)
                            listViewItem.SubItems.Add(text);
                        else if (num4 == 10 + 11 * current.Index)
                            listViewItem.SubItems.Add(text);
                        else if (num4 == 11 + 11 * current.Index)
                        {
                            listViewItem.SubItems.Add(text);
                        }
                    }
                }
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
                    string name = jsonDe.Elements[i].Name;
                    float PosX = jsonDe.Elements[i].from[0] + group.origin[0];
                    float PosY = jsonDe.Elements[i].from[1] + group.origin[1];
                    float PosZ = jsonDe.Elements[i].from[2] + group.origin[2];
                    float SizeX = jsonDe.Elements[i].to[0] - jsonDe.Elements[i].from[0];
                    float SizeY = jsonDe.Elements[i].to[1] - jsonDe.Elements[i].from[1];
                    float SizeZ = jsonDe.Elements[i].to[2] - jsonDe.Elements[i].from[2];
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
            if (skinPartListBox.SelectedItem is SkinBOX box)
            {
                int scale = 3;
                uvPictureBox.Image = new Bitmap(uvPictureBox.BackgroundImage.Width * scale, uvPictureBox.BackgroundImage.Height * scale);
                using (Graphics g = Graphics.FromImage(uvPictureBox.Image))
                {
                    float penWidth = uvPictureBox.BackgroundImage.Width / renderer3D1.TextureSize.Width + uvPictureBox.BackgroundImage.Height / renderer3D1.TextureSize.Height / 2f;
                    GraphicsPath graphicsPath = box.GetUVGraphicsPath(
                        new System.Numerics.Vector2(
                            scale * (1f / renderer3D1.TextureSize.Width) * uvPictureBox.BackgroundImage.Width,
                            scale * (1f / renderer3D1.TextureSize.Height) * uvPictureBox.BackgroundImage.Height
                            )
                        );
                    g.DrawPath(new Pen(Color.HotPink, penWidth), graphicsPath);
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