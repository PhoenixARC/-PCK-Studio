using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using MetroFramework.Forms;
using OMI.Formats.Pck;
using PckStudio.Internal;
using PckStudio.Extensions;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Text;

namespace PckStudio.Forms
{
    public partial class generateModel : MetroForm
    {
        private Image _previewImage;
        public Image PreviewImage => _previewImage;

        private PckFileData _file;

        private static Color _backgroundColor = Color.FromArgb(0xff, 0x50, 0x50, 0x50);
        private static GraphicsConfig _graphicsConfig = new GraphicsConfig()
        {
            InterpolationMode = InterpolationMode.NearestNeighbor,
            PixelOffsetMode = PixelOffsetMode.HighQuality,
        };

        private static readonly string[] ValidModelBoxTypes = new string[]
        {
            // Base 64x32 Parts
            "HEAD",
            "BODY",
            "ARM0",
            "ARM1",
            "LEG0",
            "LEG1",

            // 64x64 Overlay Parts
            "HEADWEAR",
            "JACKET",
            "SLEEVE0",
            "SLEEVE1",
            "WAIST",
            "PANTS0",
            "PANTS1",

            // Armor Parts
            "BODYARMOR",
            "ARMARMOR0",
            "ARMARMOR1",
            "BELT",
            "LEGGING0",
            "LEGGING1",
            "SOCK0",
            "SOCK1",
            "BOOT0",
            "BOOT1"
        };

        private static readonly string[] ValidModelOffsetTypes = new string[]
        {
            // Body Offsets
            "HEAD",
            "BODY",
            "ARM0",
            "ARM1",
            "LEG0",
            "LEG1",

            // Armor Offsets
            "HELMET",
            "CHEST", "BODYARMOR",
            "SHOULDER0", "ARMARMOR0",
            "SHOULDER1", "ARMARMOR0",
            "BELT",
            "LEGGING0",
            "LEGGING1",
            "SOCK0", "BOOT0",
            "SOCK1", "BOOT1",

            "TOOL0",
            "TOOL1",
        };

        List<SkinBOX> modelBoxes = new List<SkinBOX>();
        List<ModelOffset> modelOffsets = new List<ModelOffset>();

        private class ModelOffset
        {
            public string Name;
            public float YOffset;

            public ModelOffset(string name, float yOffset)
            {
                Name = name;
                YOffset = yOffset;
            }
            public (string, string) ToProperty()
            {
                string value = $"{Name} Y {YOffset}";
                return ("OFFSET", value.Replace(',','.'));
            }
        }

        public generateModel(PckFileData file)
        {
            InitializeComponent();

            _file = file;
            if (_file.Size > 0)
            {
                uvPictureBox.Image = renderer3D1.Texture = _file.GetTexture();
            }
            comboParent.Items.Clear();
            comboParent.Items.AddRange(ValidModelBoxTypes);
            LoadModelData(file.Properties);
        }

        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        private void LoadModelData(PckFileProperties properties)
        {
            renderer3D1.ANIM = properties.GetPropertyValue("ANIM", SkinANIM.FromString);
            var boxProperties = properties.GetProperties("BOX");
            var data = boxProperties.Select(kv => SkinBOX.FromString(kv.Value));
            listViewBoxes.Items.AddRange(data.Select(box => new ListViewItem(box.ToString())).ToArray());
            renderer3D1.ModelData.AddRange(data);
            renderer3D1.UploadModelData();

            properties.GetProperties("OFFSET").All(kv => {
                string[] offset = ReplaceWhitespace(kv.Value, ",").TrimEnd('\n', '\r', ' ').Split(',');
                if (offset.Length < 3)
                    return false;
                string name = offset[0];
                if (offset[1] != "Y")
                    return false;
                float value = float.Parse(offset[2]);
                if (ValidModelOffsetTypes.Contains(name))
                {
                    modelOffsets.Add(new ModelOffset(name, value));
                    return true;
                }
                return false;
            });

            UpdateListView();
        }

        //Rename model part/item
        private void listView1_DoubleClick_1(object sender, EventArgs e)
        {
            listViewBoxes.SelectedItems[0].BeginEdit();
        }

        private void GenerateUVTextureMap()
        {
            Random rng = new Random();
            using (Graphics graphics = Graphics.FromImage(uvPictureBox.Image))
            {
                graphics.ApplyConfig(_graphicsConfig);
                foreach (var part in modelBoxes)
                {
                    float width = part.Size.X * 2;
                    float height = part.Size.Y * 2;
                    float length = part.Size.Z * 2;
                    float u = part.UV.X * 2;
                    float v = part.UV.Y * 2;
                    int argb = rng.Next(-16777216, -1); // 0xFF000000 - 0xFFFFFFFF
                    var color = Color.FromArgb(argb);
                    Brush brush = new SolidBrush(color);
                    graphics.FillRectangle(brush, u + length, v, width, length);
                    graphics.FillRectangle(brush, u + length + width, v, width, length);
                    graphics.FillRectangle(brush, u, length + v, length, height);
                    graphics.FillRectangle(brush, u + length, v + length, width, height);
                    graphics.FillRectangle(brush, u + length + width, v + length, width, height);
                    graphics.FillRectangle(brush, u + length + width * 2, v + length, length, height);
                }
            }
            uvPictureBox.Invalidate();
        }

        private void DrawGuideLines(Graphics g)
        {
            throw new NotImplementedException();
            //Point center = new Point(displayBox.Height / 2, displayBox.Width / 2);
            //int headbodyY = center.Y + 25; //25
            //int legY = center.Y + 85; // - 80;
            //bool isSide = direction == ViewDirection.left || direction == ViewDirection.right;
            //if (!isSide)
            //{
            //    g.DrawLine(Pens.Red, 0, headbodyY + float.Parse(offsetHead.Text) * 5, displayBox.Width, headbodyY + float.Parse(offsetHead.Text) * 5);
            //    g.DrawLine(Pens.Green, 0, headbodyY + float.Parse(offsetBody.Text) * 5, displayBox.Width, headbodyY + float.Parse(offsetBody.Text) * 5);
            //    g.DrawLine(Pens.Blue, 0, headbodyY + float.Parse(offsetArms.Text) * 5, displayBox.Width, headbodyY + float.Parse(offsetArms.Text) * 5);
            //    g.DrawLine(Pens.Purple, 0, legY + float.Parse(offsetLegs.Text) * 5, displayBox.Width, legY + float.Parse(offsetLegs.Text) * 5);
            //}
            //g.DrawLine(Pens.Red, center.X, 0, center.X, displayBox.Height);
            //g.DrawLine(Pens.Blue, center.X + 30, 0, center.X + 30, displayBox.Height);
            //g.DrawLine(Pens.Blue, center.X - 30, 0, center.X - 30, displayBox.Height);
            //g.DrawLine(Pens.Purple, center.X - 10, 0, center.X - 10, displayBox.Height);
            //g.DrawLine(Pens.Purple, center.X + 10, 0, center .X + 10, displayBox.Height);
        }

        private void DrawArmorOffsets(Graphics g)
        {
            throw new NotImplementedException();
            //int centerPointHeight = displayBox.Height / 2;
            //int centerPointWidth = displayBox.Width / 2;
            //int headbodyY = centerPointHeight + 25; //25
            //int armY = centerPointHeight + 35; // - 60;
            //int legY = centerPointHeight + 85; // - 80;
            //SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(80, 50, 50, 75));
            //g.FillRectangle(semiTransBrush, centerPointWidth, (float)(headbodyY - 40 /*+ offsetHelmet.Value * 5*/), 40, 40); // Helmet
            //bool isSide = direction == ViewDirection.left || direction == ViewDirection.right;
            //if (isSide)
            //{
            //    g.FillRectangle(semiTransBrush, centerPointWidth - 10, headbodyY, 20, 60); // Chest
            //    g.FillRectangle(semiTransBrush, centerPointWidth - 10, (float)(legY + 40 /*+ offsetBoots.Value * 5*/), 20, 20); // Boots
            //    g.FillRectangle(semiTransBrush, centerPointWidth - 10, (float)(legY /*+ offsetPants.Value * 5*/), 20, 40); // Pants
            //    g.FillRectangle(semiTransBrush, centerPointWidth - 5, (float)(armY + 45 /*+ offsetTool.Value * 5*/), 10, 10); // Tools
            //}
            //else
            //{
            //    g.FillRectangle(semiTransBrush, centerPointWidth - 20, headbodyY, 40, 60); // Chest
            //    g.FillRectangle(semiTransBrush, centerPointWidth - 35, (float)(armY + 45 /*+ offsetTool.Value * 5*/), 10, 10); // Tool0
            //    g.FillRectangle(semiTransBrush, centerPointWidth + 25, (float)(armY + 45 /*+ offsetTool.Value * 5*/), 10, 10); // Tool1
            //    g.FillRectangle(semiTransBrush, centerPointWidth - 20, (float)(legY /*+ offsetPants.Value * 5*/), 20, 40); // Pants0
            //    g.FillRectangle(semiTransBrush, centerPointWidth, (float)(legY /*+ offsetPants.Value * 5*/), 20, 40); // Pants1
            //    g.FillRectangle(semiTransBrush, centerPointWidth - 20, (float)(legY + 40 /*+ offsetBoots.Value * 5*/), 20, 20); // Boot0
            //    g.FillRectangle(semiTransBrush, centerPointWidth, (float)(legY + 40 /*+ offsetBoots.Value * 5*/), 20, 20); // Boot1
            //}

        }

        private void generateModel_Load(object sender, EventArgs e)
        {
            if (Screen.PrimaryScreen.Bounds.Height >= 780 && Screen.PrimaryScreen.Bounds.Width >= 1080) {
                return;
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modelBoxes.Add(SkinBOX.Empty);
            UpdateListView();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeColorToolStripMenuItem.Visible = false;
            if (listViewBoxes.SelectedItems.Count != 0 && listViewBoxes.SelectedItems[0].Tag is SkinBOX)
            {
                changeColorToolStripMenuItem.Visible = true;
                var part = listViewBoxes.SelectedItems[0].Tag as SkinBOX;
                //graphics.DrawRectangle(Pens.Yellow, x + (float)double.Parse(this.selected.SubItems[3].Text) * 5 - 1, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5 - 1, (float)double.Parse(this.selected.SubItems[6].Text) * 5 + 2, (float)double.Parse(this.selected.SubItems[5].Text) * 5 + 2);
                //graphics.DrawRectangle(Pens.Black, x + (float)double.Parse(this.selected.SubItems[3].Text) * 5, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5, (float)double.Parse(this.selected.SubItems[6].Text) * 5, (float)double.Parse(this.selected.SubItems[5].Text) * 5);
                comboParent.Text = part.Type;
                PosXUpDown.Value = (decimal)part.Pos.X;
                PosYUpDown.Value = (decimal)part.Pos.Y;
                PosZUpDown.Value = (decimal)part.Pos.Z;
                SizeXUpDown.Value = (decimal)part.Size.X;
                SizeYUpDown.Value = (decimal)part.Size.Y;
                SizeZUpDown.Value = (decimal)part.Size.Z;
                TextureXUpDown.Value = (decimal)part.UV.X;
                TextureYUpDown.Value = (decimal)part.UV.Y;
            }
        }

        //Changes Item Model Class
        private void comboParent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Type = comboParent.Text;
                buttonIMPORT.Enabled = true;
                buttonEXPORT.Enabled = true;
                SizeXUpDown.Enabled = true;
                SizeYUpDown.Enabled = true;
                SizeZUpDown.Enabled = true;
                PosXUpDown.Enabled = true;
                PosYUpDown.Enabled = true;
                PosZUpDown.Enabled = true;
                TextureXUpDown.Enabled = true;
                TextureYUpDown.Enabled = true;
            }
        }

        private void SizeXUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Size.X = (float)SizeXUpDown.Value;
            }
            UpdateListView();
        }

        private void SizeYUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Size.Y = (float)SizeYUpDown.Value;
            }
            UpdateListView();
        }

        private void SizeZUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Size.Z = (float)SizeZUpDown.Value;
            }
            UpdateListView();
        }

        private void PosXUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Pos.X = (float)PosXUpDown.Value;
            }
            UpdateListView();
        }

        private void PosYUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Pos.Y = (float)PosYUpDown.Value;
            }
            UpdateListView();
        }

        private void PosZUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Pos.Z = (float)PosZUpDown.Value;
            }
            UpdateListView();
        }

        //Sets Texture X-Offset
        private void TextureXUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.UV.X = (int)TextureXUpDown.Value;
            }
            UpdateListView();
        }

        //Sets texture Y-Offset
        private void TextureYUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.UV.Y = (int)TextureYUpDown.Value;
            }
            UpdateListView();
        }

        //Export Current Skin Texture
        private void buttonEXPORT_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(uvPictureBox.Image, 64, 64);
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image Files | *.png";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        //Imports Skin Texture
        private void buttonIMPORT_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG Image Files | *.png";
            openFileDialog.Title = "Select Skin Texture";

            if (openFileDialog.ShowDialog() == DialogResult.OK) // skins can only be a 1:1 ratio (base 64x64) or a 2:1 ratio (base 64x32)
            {
                using (var img = Image.FromFile(openFileDialog.FileName))
				{
                    if ((img.Width == img.Height || img.Height == img.Width / 2))
                    {
                        generateTextureCheckBox.Checked = false;
                        using (Graphics graphics = Graphics.FromImage(uvPictureBox.Image))
                        {
                            graphics.ApplyConfig(_graphicsConfig);
                            graphics.DrawImage(img, 0, 0, img.Width, img.Height);
                        }
                        uvPictureBox.Invalidate();
                    }
                    else
					{
                        MessageBox.Show(this, "Not a valid skin file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Creates Model Data and Finalizes
        private void buttonDone_Click(object sender, EventArgs e)
        {
            foreach (var part in modelBoxes)
            {
                _file.Properties.Add("BOX", part);
            }
            var img = new Bitmap(renderer3D1.Size.Width, renderer3D1.Size.Height);
            renderer3D1.DrawToBitmap(img, renderer3D1.Bounds);
            _previewImage = img;
            DialogResult = DialogResult.OK;
        }

        // Trigger Dialog to select model part/item color
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                listViewBoxes.SelectedItems[0].ForeColor = colorDialog.Color;
        }

        //Re-renders head with updated x-offset
        private void offsetHead_TextChanged(object sender, EventArgs e)
        {
            
        }

        //Re-renders body with updated x-offset
        private void offsetBody_TextAlignChanged(object sender, EventArgs e)
        {

        }

        //Loads in model template(Steve)
        private void buttonTemplate_Click(object sender, EventArgs e)
        {
            modelBoxes.Add(SkinBOX.FromString("HEAD -4 -8 -4 8 8 8 0 0 0 0 0"));
            modelBoxes.Add(SkinBOX.FromString("BODY -4 0 -2 8 12 4 16 16 0 0 0"));
            modelBoxes.Add(SkinBOX.FromString("ARM0 -3 -2 -2 4 12 4 40 16 0 0 0"));
            modelBoxes.Add(SkinBOX.FromString("ARM1 -1 -2 -2 4 12 4 40 16 0 1 0"));
            modelBoxes.Add(SkinBOX.FromString("LEG0 -2 0 -2 4 12 4 0 16 0 0 0"));
            modelBoxes.Add(SkinBOX.FromString("LEG1 -2 0 -2 4 12 4 0 16 0 1 0"));
            comboParent.Enabled = true;
            UpdateListView();
        }

        private void UpdateListView()
        {
            listViewBoxes.Items.Clear();
            foreach (var part in modelBoxes)
            {
                ListViewItem listViewItem = new ListViewItem(part.Type);
                listViewItem.Tag = part;
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Pos.X.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Pos.Y.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Pos.Z.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Size.X.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Size.Y.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Size.Z.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.UV.X.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.UV.Y.ToString()));
                listViewBoxes.Items.Add(listViewItem);
            }
        }

        // Exports model as csm file
        private void buttonExportModel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Custom Skin Model File | *.CSM";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string contents = "";
            foreach (ListViewItem listViewItem in listViewBoxes.Items)
            {
                string str = "";
                foreach (ListViewItem.ListViewSubItem subItem in listViewItem.SubItems)
                {
                    if (subItem.Text != "unchecked")
                        str = str + subItem.Text + Environment.NewLine;
                }
                contents += (listViewItem.Text + Environment.NewLine + listViewItem.Tag) + Environment.NewLine + str;
            }

            File.WriteAllText(saveFileDialog.FileName, contents);
        }

        // Imports model from csm file
        private void buttonImportModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Custom Skin Model File | *.CSM";
            openFileDialog.Title = "Select Custom Skin Model File";
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                listViewBoxes.Items.Clear();
                modelBoxes.Clear();
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
                    modelBoxes.Add(SkinBOX.FromString($"{part} {PosX} {PosY} {PosZ} {SizeX} {SizeY} {SizeZ} {UvX} {UvY}"));
                }

            }
            comboParent.Enabled = true;
            UpdateListView();
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem listViewItem = new ListViewItem();
                var selected = listViewBoxes.SelectedItems[0];
                listViewItem.Text = selected.Text;
                listViewItem.Tag = selected.Tag;
                int num = 0;
                foreach (ListViewItem.ListViewSubItem subItem in selected.SubItems)
                {
                    if (num > 0)
                        listViewItem.SubItems.Add(subItem.Text);
                    ++num;
                }
                listViewBoxes.Items.Add(listViewItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Please Select a Part");
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems[0] == null)
                return;
            listViewBoxes.SelectedItems[0].Remove();
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                listViewBoxes.SelectedItems[0].ForeColor = colorDialog.Color;
        }

        //Re-renders tool with updated x-offset
        private void offsetTool_TextChanged(object sender, EventArgs e)
        {
            
        }

        //Re-renders helmet with updated x-offset
        private void offsetHelmet_TextChanged(object sender, EventArgs e)
        {
            
        }

        //Re-renders pants with updated x-offset
        private void offsetPants_TextChanged(object sender, EventArgs e)
        {
            
        }

        //Re-renders leggings with updated x-offset
        private void offsetLeggings_TextChanged(object sender, EventArgs e)
        {
            
        }

        //Re-renders boots with updated x-offset
        private void offsetBoots_TextChanged(object sender, EventArgs e)
        {
            
        }

        //Item Selection
        private void listView1_Click(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 && listViewBoxes.SelectedItems[0] != null &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                comboParent.Text = part.Type;
                PosXUpDown.Value = (decimal)part.Pos.X;
                PosYUpDown.Value = (decimal)part.Pos.Y;
                PosZUpDown.Value = (decimal)part.Pos.Z;
                SizeXUpDown.Value = (decimal)part.Size.X;
                SizeYUpDown.Value = (decimal)part.Size.Y;
                SizeZUpDown.Value = (decimal)part.Size.Z;
                TextureXUpDown.Value = (decimal)part.UV.X;
                TextureYUpDown.Value = (decimal)part.UV.Y;
                SizeXUpDown.Enabled = true;
                SizeYUpDown.Enabled = true;
                SizeZUpDown.Enabled = true;
                PosXUpDown.Enabled = true;
                PosYUpDown.Enabled = true;
                PosZUpDown.Enabled = true;
                TextureXUpDown.Enabled = true;
                TextureYUpDown.Enabled = true;
                comboParent.Enabled = true;
                return;
            }
            SizeXUpDown.Enabled = false;
            SizeYUpDown.Enabled = false;
            SizeZUpDown.Enabled = false;
            PosXUpDown.Enabled = false;
            PosYUpDown.Enabled = false;
            PosZUpDown.Enabled = false;
            TextureXUpDown.Enabled = false;
            TextureYUpDown.Enabled = false;
            comboParent.Enabled = false;
            
        }

        //currently scrapped
        private void generateModel_FormClosing(object sender, FormClosingEventArgs e)
        { 
            /*if (MessageBox.Show("You done here?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = false;*/
        }

        //Del stuff using key
        private void delStuffUsingDelKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                if (modelBoxes.Remove(part))
                    listViewBoxes.SelectedItems[0].Remove();
                
            }
        }

        private void generateModel_SizeChanged(object sender, EventArgs e)
        {
            
        }

        // TODO
        private void OpenJSONButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Model File | *.JSON";
            openFileDialog.Title = "Select JSON Model File";
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                listViewBoxes.Items.Clear();
                string str1 = JSONToCSM(openFileDialog.FileName);
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
                                listViewBoxes.Items.Add(listViewItem);
                            }
                        }
                }
            }
            
        }

        [Obsolete("Just whyyyyy")]
        public string JSONToCSM(string InputFilePath)
        {
            CSMJObject jsonDe = JsonConvert.DeserializeObject<CSMJObject>(File.ReadAllText(InputFilePath));
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
            uvPictureBox.Image = e.NewTexture;
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