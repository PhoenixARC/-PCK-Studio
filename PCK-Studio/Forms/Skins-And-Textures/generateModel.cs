using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using MetroFramework.Forms;
using PckStudio.Classes.FileTypes;
using System.Text.RegularExpressions;

namespace PckStudio
{
    public partial class generateModel : MetroForm
    {
        PictureBox skinPreview;

        eViewDirection direction = eViewDirection.front;

        enum eViewDirection
        {
            front,
            back,
            left,
            right
        }

        PCKProperties boxes;

        Color backgroundColor = Color.FromArgb(0xff, 0x50, 0x50, 0x50);

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
            "SHOULDER0",
            "SHOULDER1",
            "SLEEVE0",
            "SLEEVE1",
            "PANTS0",
            "PANTS1",
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
            "CHEST",
            "BOOT0",
            "BOOT1",
            "WAIST",
            "PANTS0",
            "PANTS1",

            "TOOL0",
            "TOOL1",
        };

        List<ModelPart> modelParts = new List<ModelPart>();
        List<ModelOffset> modelOffsets = new List<ModelOffset>();

        class ModelPart
        {
            public string Type;
            public float X, Y, Z;
            public float Width;
            public float Height;
            public float Length;
            public int U, V;

            public ModelPart(string type, float x, float y, float z, float width, float height, float length, int u, int v)
            {
                Type = type;
                X = x;
                Y = y;
                Z = z;
                Width = width;
                Height = height;
                Length = length;
                U = u;
                V = v;
            }

            public ValueTuple<string, string> ToProperty()
            {
                string value = $"{Type} {X} {Y} {Z} {Width} {Height} {Length} {U} {V}";
                return new ValueTuple<string, string>("BOX", value.Replace(',', '.'));
            }

        }

        class ModelOffset
        {
            public string Name;
            public float YOffset;

            public ModelOffset(string name, float yOffset)
            {
                Name = name;
                YOffset = yOffset;
            }
            public ValueTuple<string, string> ToProperty()
            {
                string value = $"{Name} Y {YOffset}";
                return new ValueTuple<string, string>("OFFSET", value.Replace(',','.'));
            }
        }


        public generateModel(PCKProperties skinProperties, PictureBox preview)
        {
            InitializeComponent();
            boxes = skinProperties;
            skinPreview = preview;
            if (texturePreview.Image == null)
                texturePreview.Image = new Bitmap(64, 64);
            loadData();
        }
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        //loads data from mode list
        private void loadData()
        {
            foreach (var property in boxes)
            {
                switch (property.Item1)
                {
                    case "BOX":
                        {
                            string[] Format = ReplaceWhitespace(property.Item2, ",").TrimEnd('\n', '\r', ' ').Split(',');
                            if (Format.Length < 9)
                            {
                                Console.WriteLine($"'{property.Item1}' property has too few arguments: {property.Item2}");
                                continue;
                            }
                            string name = Format[0];
                            if (ValidModelBoxTypes.Contains(name))
                            {
                                // %10ls = name
                                // %f
                                // %f
                                // %f
                                // %f
                                // %f
                                // %f
                                // %f
                                // %f
                                // %d
                                // %d
                                // %f
                                try
                                {
                                    float x = float.Parse(Format[1]);
                                    float y = float.Parse(Format[2]);
                                    float z = float.Parse(Format[3]);
                                    float sizeX = float.Parse(Format[4]);
                                    float sizeY = float.Parse(Format[5]);
                                    float sizeZ = float.Parse(Format[6]);
                                    int u = int.Parse(Format[7]);
                                    int v = int.Parse(Format[8]);
                                    modelParts.Add(new ModelPart(name, x, y, z, sizeX, sizeY, sizeZ, u, v));
                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    MessageBox.Show("A Format Exception was thrown\nFailed to parse BOX value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                catch (OverflowException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    MessageBox.Show("An Overflow Exception was thrown\nFailed to parse BOX value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            if (Format.Length >= 11)
                            {
                                string unk1 = Format[9];
                                string unk2 = Format[10];
                                Console.WriteLine($"{unk1} | {unk2}");
                            }
                            comboParent.Enabled = true;
                            break;
                        }

                    case "OFFSET":
                        {
                            string[] offset = ReplaceWhitespace(property.Item2, ",").TrimEnd('\n', '\r', ' ').Split(',');
                            if (offset.Length < 3) continue;
                            string name = offset[0];
                            string dimension = offset[1]; // "Y"
                            if (dimension != "Y") continue;
                            float value = float.Parse(offset[2]);
                            if (ValidModelOffsetTypes.Contains(name))
                                modelOffsets.Add(new ModelOffset(name, value));
                            break;
                        }
                    case "ANIM":
                        {
                            try
                            {
                                //ANIM = (eANIMFlags)int.Parse(property.Item2, System.Globalization.NumberStyles.HexNumber);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            break;
                        }
                }
            }
            updateListView();
            render();
        }

        //Rename model part/item
        private void listView1_DoubleClick_1(object sender, EventArgs e)
        {
            listViewBoxes.SelectedItems[0].BeginEdit();
        }

        // Graphic Rendering
        // Builds an image based on the view
        private void render(object sender = null, EventArgs e = null)
        {
            //buttonTemplate.Enabled = listViewBoxes.Items.Count == 0;
            //setZ(); //Organizes Z layers
            Bitmap bitmapModelPreview = new Bitmap(displayBox.Width, displayBox.Height); //Creates Model Display layer
            using (Graphics graphics = Graphics.FromImage(bitmapModelPreview))
            {
                graphics.Clear(backgroundColor);
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                // makes sure it reders/draws the full pixel in top left corner
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                float headbodyY = (displayBox.Height / 2) + 25; //  25
                float armY = (displayBox.Height / 2) + 35; // -60;
                float legY = (displayBox.Height / 2) + 85; // -80;
                float groundLevel = (displayBox.Height / 2) + 145;
                graphics.DrawLine(Pens.White, 0, groundLevel, displayBox.Width, groundLevel);
                // Chooses Render settings based on current direction
                foreach (ListViewItem listViewItem in listViewBoxes.Items)
                {
                    if (!(listViewItem.Tag is ModelPart)) continue;
                    ModelPart part = listViewItem.Tag as ModelPart;
                    float x = displayBox.Width / 2;
                    float y = 0;
                    switch (direction)
                    {
                        case eViewDirection.front:
                            {
                                //Sets X & Y based on model part class
                                // listViewItem.Text -> part.Type
                                // listViewItem.SubItems[1] -> part.X
                                // listViewItem.SubItems[2] -> part.Y
                                // listViewItem.SubItems[3] -> part.Z
                                // listViewItem.SubItems[4] -> part.Width
                                // listViewItem.SubItems[5] -> part.Height
                                // listViewItem.SubItems[6] -> part.Length
                                // listViewItem.SubItems[7] -> part.U
                                // listViewItem.SubItems[8] -> part.V
                                switch (part.Type)
                                {
                                    case "HEAD":
                                        y = headbodyY + int.Parse(offsetHead.Text) * 5;
                                        break;
                                    case "BODY":
                                        y = headbodyY + int.Parse(offsetBody.Text) * 5;
                                        break;

                                    case "ARM0":
                                        x -= 25;
                                        y = armY + int.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "ARM1":
                                        x += 25;
                                        y = armY + int.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "LEG0":
                                        x -= 10;
                                        y = legY + int.Parse(offsetLegs.Text) * 5;
                                        break;

                                    case "LEG1":
                                        x += 10;
                                        y = legY + int.Parse(offsetLegs.Text) * 5;
                                        break;
                                }

                                //Maps imported Texture if auto texture is disabled
                                if (!checkTextureGenerate.Checked)
                                {
                                    RectangleF destRect = new RectangleF(
                                        x + part.X * 5,
                                        y + part.Y * 5,
                                        part.Width * 5,
                                        part.Height * 5);
                                    RectangleF srcRect = new RectangleF(
                                        part.U + part.Length,
                                        part.V + part.Length,
                                        part.Width,
                                        part.Height);
                                    graphics.DrawImage(texturePreview.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    graphics.FillRectangle(new SolidBrush(listViewItem.ForeColor), x + part.X * 5, y + part.Y * 5, part.Width * 5, part.Height * 5);
                                }

                                break;
                            }

                        case eViewDirection.left:
                            {
                                //Sets X & Y based on model part class
                                switch (part.Type)
                                {
                                    case "HEAD":
                                        y = headbodyY + float.Parse(offsetHead.Text) * 5;
                                        break;

                                    case "BODY":
                                        y = headbodyY + float.Parse(offsetBody.Text) * 5;
                                        break;

                                    case "ARM0":
                                        y = armY + float.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "ARM1":
                                        y = armY + float.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "LEG0":
                                        y = legY + float.Parse(offsetLegs.Text) * 5;
                                        break;

                                    case "LEG1":
                                        y = legY + float.Parse(offsetLegs.Text) * 5;
                                        break;
                                }

                                //Maps imported Texture if auto texture is disabled
                                if (!checkTextureGenerate.Checked)
                                {
                                    RectangleF destRect = new RectangleF(
                                        x + part.Z * 5,
                                        y + part.Y * 5,
                                        part.Length * 5,
                                        part.Height * 5);
                                    RectangleF srcRect = new RectangleF(
                                        part.U + part.Length + part.Width,
                                        part.V + part.Length,
                                        part.Length,
                                        part.Height);
                                    graphics.DrawImage(texturePreview.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    //Draws Part
                                    graphics.FillRectangle(new SolidBrush(listViewItem.ForeColor), x + part.Z * 5, y + part.Y * 5, part.Length * 5, part.Height * 5);
                                }
                                bitmapModelPreview.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                break;
                            }

                        case eViewDirection.back:
                            {
                                //Sets X & Y based on model part class
                                switch (part.Type)
                                {
                                    case "HEAD":
                                        y = headbodyY + float.Parse(offsetHead.Text) * 5;
                                        break;
                                    case "BODY":
                                        y = headbodyY + float.Parse(offsetBody.Text) * 5;
                                        break;
                                    case "ARM0":
                                        x -= 25;
                                        y = armY + float.Parse(offsetArms.Text) * 5;
                                        break;
                                    case "ARM1":
                                        x += 25;
                                        y = armY + float.Parse(offsetArms.Text) * 5;
                                        break;
                                    case "LEG0":
                                        x -= 10;
                                        y = legY + float.Parse(offsetLegs.Text) * 5;
                                        break;
                                    case "LEG1":
                                        x += 10;
                                        y = legY + float.Parse(offsetLegs.Text) * 5;
                                        break;
                                }

                                //Maps imported Texture if auto texture is disabled
                                if (!checkTextureGenerate.Checked)
                                {
                                    RectangleF destRect = new RectangleF(
                                        x + part.X * 5,
                                        y + part.Y * 5,
                                        part.Width * 5,
                                        part.Height * 5);
                                    RectangleF srcRect = new RectangleF(
                                        part.U + part.Length * 2 + part.Width,
                                        part.V + part.Length,
                                        part.Width,
                                        part.Height);
                                    graphics.DrawImage(texturePreview.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    //Draws Part
                                    graphics.FillRectangle(new SolidBrush(listViewItem.ForeColor), x + part.X * 5, y + part.Y * 5, part.Width * 5, part.Height * 5);
                                }
                                bitmapModelPreview.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                break;
                            }

                        case eViewDirection.right:
                            //Sets X & Y based on model part class
                            switch (part.Type)
                            {
                                case "HEAD":
                                    y = headbodyY + float.Parse(offsetHead.Text) * 5;
                                    break;
                                case "BODY":
                                    y = headbodyY + float.Parse(offsetBody.Text) * 5;
                                    break;

                                case "ARM0":
                                    y = armY + float.Parse(offsetArms.Text) * 5;
                                    break;
                                case "ARM1":
                                    y = armY + float.Parse(offsetArms.Text) * 5;
                                    break;

                                case "LEG0":
                                    y = legY + float.Parse(offsetLegs.Text) * 5;
                                    break;

                                case "LEG1":
                                    y = legY + float.Parse(offsetLegs.Text) * 5;
                                    break;
                            }
                            //Maps imported Texture if auto texture is disabled
                            if (!checkTextureGenerate.Checked)
                            {
                                RectangleF destRect = new RectangleF(
                                    x + part.Z * 5,
                                    y + part.Y * 5,
                                    part.Length * 5,
                                    part.Height * 5);
                                RectangleF srcRect = new RectangleF(
                                    part.U + part.Length + part.Width,
                                    part.V + part.Length,
                                    part.Length,
                                    part.Height);
                                graphics.DrawImage(texturePreview.Image, destRect, srcRect, GraphicsUnit.Pixel);
                            }
                            else
                            {
                                //Draws Part
                                graphics.FillRectangle(new SolidBrush(listViewItem.ForeColor), x + part.Z * 5, y + part.Y * 5, part.Length * 5, part.Height * 5);
                            }
                            break;
                    }
                }

                if (checkBoxArmor.Checked)
                    DrawArmorOffsets(graphics);
                // draw last to be on top
                if (checkGuide.Checked)
                    DrawGuideLines(graphics);
            }
            displayBox.Image = bitmapModelPreview; // Sets created preview graphics to display box
        }

        private void MapTexture()
        {
            if (checkTextureGenerate.Checked)
            {
                Bitmap bitmapAutoTexture = new Bitmap(texturePreview.Width, texturePreview.Height);
                using (Graphics graphics = Graphics.FromImage(bitmapAutoTexture))
                {
                    foreach (var part in modelParts)
                    {
                        float width = part.Width * 2;
                        float height = part.Height * 2;
                        float length = part.Length * 2;
                        int u = part.U * 2;
                        int v = part.V * 2;
                        Random r = new Random();
                        Brush brush = new SolidBrush(Color.FromArgb(r.Next(int.MinValue, int.MaxValue)));
                        graphics.FillRectangle(brush, u + length, v, width, length);
                        graphics.FillRectangle(brush, u + length + width, v, width, length);
                        graphics.FillRectangle(brush, u, length + v, length, height);
                        graphics.FillRectangle(brush, u + length, v + length, width, height);
                        graphics.FillRectangle(brush, u + length + width, v + length, width, height);
                        graphics.FillRectangle(brush, u + length + width * 2, v + length, length, height);
                    }
                }
                texturePreview.Image = bitmapAutoTexture;
            }
        }

        //Checks and sets Z layering
        private void setZ()
        {
            foreach (ListViewItem listViewItem in listViewBoxes.Items)
                listViewItem.SubItems.Add("unchecked");

            if (direction == eViewDirection.front)
            {
                int checkedItems = 0;
                do
                {
                    foreach (ListViewItem listViewItemCurrent in listViewBoxes.Items)
                    {
                        if (listViewItemCurrent.SubItems[9].Text == "unchecked")
                        {
                            float x = 0;
                            if (listViewItemCurrent.Tag.ToString() == "HEAD")
                                x = displayBox.Width / 2;
                            else if (listViewItemCurrent.Tag.ToString() == "BODY")
                                x = displayBox.Width / 2;
                            else if (listViewItemCurrent.Tag.ToString() == "ARM0")
                                x = 178;
                            else if (listViewItemCurrent.Tag.ToString() == "ARM1")
                                x = 228;
                            else if (listViewItemCurrent.Tag.ToString() == "LEG0")
                                x = 193;
                            else if (listViewItemCurrent.Tag.ToString() == "LEG1")
                                x = 213;
                            bool flag = false;
                            int index = listViewItemCurrent.Index;
                            foreach (ListViewItem listViewItemComparing in listViewBoxes.Items)
                            {
                                if (listViewItemComparing.SubItems[9].Text == "unchecked" && (int)double.Parse(listViewItemCurrent.SubItems[3].Text) + (int)double.Parse(listViewItemCurrent.SubItems[6].Text) < (int)double.Parse(listViewItemComparing.SubItems[3].Text) + (int)double.Parse(listViewItemComparing.SubItems[6].Text))
                                {
                                    if (listViewItemComparing.Index < listViewBoxes.Items.Count + 1)
                                    {
                                        index = listViewItemComparing.Index + 1;
                                        flag = true;
                                    }
                                }
                            }
                            listViewItemCurrent.SubItems[9].Text = "checked";
                            checkedItems += 1;
                            if (flag == true)
                            {
                                ListViewItem listViewItem2 = (ListViewItem)listViewItemCurrent.Clone();
                                listViewBoxes.Items.Insert(index, listViewItem2);
                                listViewItemCurrent.Remove();
                            }
                        }
                        else
                        {
                            checkedItems += 1;
                        }
                    }
                } while (checkedItems < listViewBoxes.Items.Count);
            }
            else if (direction == eViewDirection.left)
            {
                int checkedItems = 0;
                do
                {
                    foreach (ListViewItem listViewItem1 in listViewBoxes.Items)
                    {
                        if (listViewItem1.SubItems[listViewItem1.SubItems.Count - 1].Text == "unchecked")
                        {
                            float x = 0;
                            if (listViewItem1.Tag.ToString() == "HEAD")
                                x = displayBox.Width / 2;
                            else if (listViewItem1.Tag.ToString() == "BODY")
                                x = displayBox.Width / 2;
                            else if (listViewItem1.Tag.ToString() == "ARM0")
                                x = 178;
                            else if (listViewItem1.Tag.ToString() == "ARM1")
                                x = 228;
                            else if (listViewItem1.Tag.ToString() == "LEG0")
                                x = 193;
                            else if (listViewItem1.Tag.ToString() == "LEG1")
                                x = 213;
                            bool flag = false;
                            int index = listViewItem1.Index;
                            foreach (ListViewItem listViewItem2 in listViewBoxes.Items)
                            {
                                if (listViewItem2.SubItems[9].Text == "unchecked")
                                {
                                    int y = 0;
                                    if (listViewItem2.Tag.ToString() == "HEAD")
                                        y = displayBox.Width / 2;
                                    else if (listViewItem2.Tag.ToString() == "BODY")
                                        y = displayBox.Width / 2;
                                    else if (listViewItem2.Tag.ToString() == "ARM0")
                                        y = 178;
                                    else if (listViewItem2.Tag.ToString() == "ARM1")
                                        y = 228;
                                    else if (listViewItem2.Tag.ToString() == "LEG0")
                                        y = 193;
                                    else if (listViewItem2.Tag.ToString() == "LEG1")
                                        y = 213;
                                    if ((int)double.Parse(listViewItem1.SubItems[1].Text) + (int)double.Parse(listViewItem1.SubItems[4].Text) + x < (int)double.Parse(listViewItem2.SubItems[1].Text) + (int)double.Parse(listViewItem2.SubItems[4].Text) + y && listViewItem2.Index + 1 < this.listViewBoxes.Items.Count + 1)
                                    {
                                        index = listViewItem2.Index + 1;
                                        flag = true;
                                    }
                                }
                            }
                            listViewItem1.SubItems[9].Text = "checked";
                            checkedItems += 1;
                            if (flag == true)
                            {
                                ListViewItem listViewItem2 = (ListViewItem)listViewItem1.Clone();
                                listViewBoxes.Items.Insert(index, listViewItem2);
                                if (listViewBoxes.SelectedItems.Count != 0)
                                {
                                    //if (selected.Index == listViewItem1.Index)
                                    //{
                                    //    selected = listViewItem2;
                                    //}
                                }
                                listViewItem1.Remove();
                            }
                        }
                        else
                        {
                            checkedItems += 1;
                        }
                    }
                } while (checkedItems < listViewBoxes.Items.Count);
            }
            else if (direction == eViewDirection.back)
            {
                int checkedItems = 0;
                do
                {
                    foreach (ListViewItem listViewItemCurrent in this.listViewBoxes.Items)
                    {
                        if (listViewItemCurrent.SubItems[listViewItemCurrent.SubItems.Count - 1].Text == "unchecked")
                        {
                            bool flag = false;
                            int index = listViewItemCurrent.Index;
                            foreach (ListViewItem listViewItemComparing in this.listViewBoxes.Items)
                            {
                                if (listViewItemComparing.SubItems[9].Text == "unchecked" && (int)double.Parse(listViewItemCurrent.SubItems[3].Text) + (int)double.Parse(listViewItemCurrent.SubItems[6].Text) > (int)double.Parse(listViewItemComparing.SubItems[3].Text) + (int)double.Parse(listViewItemComparing.SubItems[6].Text))
                                {
                                    if (listViewItemComparing.Index < this.listViewBoxes.Items.Count + 1)
                                    {
                                        index = listViewItemComparing.Index + 1;
                                        flag = true;
                                    }
                                }
                            }
                            listViewItemCurrent.SubItems[9].Text = "checked";
                            checkedItems += 1;
                            if (flag == true)
                            {
                                ListViewItem listViewItem2 = (ListViewItem)listViewItemCurrent.Clone();
                                listViewBoxes.Items.Insert(index, listViewItem2);
                                if (listViewBoxes.SelectedItems.Count != 0)
                                {
                                    //if (selected.Index == listViewItemCurrent.Index)
                                    //{
                                    //    selected = listViewItem2;
                                    //}
                                }
                                listViewItemCurrent.Remove();
                            }
                        }
                        else
                        {
                            checkedItems += 1;
                        }
                    }
                } while (checkedItems < listViewBoxes.Items.Count);
            }
            else if (direction == eViewDirection.right)
            {
                int checkedItems = 0;
                do
                {
                    foreach (ListViewItem listViewItem1 in listViewBoxes.Items)
                    {
                        if (listViewItem1.SubItems[listViewItem1.SubItems.Count - 1].Text == "unchecked")
                        {
                            float x = 0;
                            if (listViewItem1.Tag.ToString() == "HEAD")
                                x = displayBox.Width / 2;
                            else if (listViewItem1.Tag.ToString() == "BODY")
                                x = displayBox.Width / 2;
                            else if (listViewItem1.Tag.ToString() == "ARM0")
                                x = 178;
                            else if (listViewItem1.Tag.ToString() == "ARM1")
                                x = 228;
                            else if (listViewItem1.Tag.ToString() == "LEG0")
                                x = 193;
                            else if (listViewItem1.Tag.ToString() == "LEG1")
                                x = 213;
                            bool flag = false;
                            int index = listViewItem1.Index;
                            foreach (ListViewItem listViewItem2 in listViewBoxes.Items)
                            {
                                if (listViewItem2.SubItems[9].Text == "unchecked")
                                {
                                    int y = 0;
                                    if (listViewItem2.Tag.ToString() == "HEAD")
                                        y = displayBox.Width / 2;
                                    else if (listViewItem2.Tag.ToString() == "BODY")
                                        y = displayBox.Width / 2;
                                    else if (listViewItem2.Tag.ToString() == "ARM0")
                                        y = 178;
                                    else if (listViewItem2.Tag.ToString() == "ARM1")
                                        y = 228;
                                    else if (listViewItem2.Tag.ToString() == "LEG0")
                                        y = 193;
                                    else if (listViewItem2.Tag.ToString() == "LEG1")
                                        y = 213;
                                    if ((int)double.Parse(listViewItem1.SubItems[1].Text) + (int)double.Parse(listViewItem1.SubItems[4].Text) - x > (int)double.Parse(listViewItem2.SubItems[1].Text) + (int)double.Parse(listViewItem2.SubItems[4].Text) + y && listViewItem2.Index + 1 < this.listViewBoxes.Items.Count + 1)
                                    {
                                        index = listViewItem2.Index + 1;
                                        flag = true;
                                    }
                                }
                            }
                            listViewItem1.SubItems[9].Text = "checked";
                            checkedItems += 1;
                            if (flag == true)
                            {
                                ListViewItem listViewItem2 = (ListViewItem)listViewItem1.Clone();
                                listViewBoxes.Items.Insert(index, listViewItem2);
                                if (listViewBoxes.SelectedItems.Count != 0)
                                {
                                    //if (selected.Index == listViewItem1.Index)
                                    //{
                                    //    selected = listViewItem2;
                                    //}
                                }
                                listViewItem1.Remove();
                            }
                        }
                        else
                        {
                            checkedItems += 1;
                        }
                    }
                } while (checkedItems < listViewBoxes.Items.Count);
            }
        }

        private void DrawGuideLines(Graphics g)
        {
            int centerHeightPoint = displayBox.Height / 2;
            int centerWidthPoint = displayBox.Width / 2;
            int headbodyY = centerHeightPoint + 25; //25
            int legY = centerHeightPoint + 85; // - 80;
            bool isSide = direction == eViewDirection.left || direction == eViewDirection.right;
            if (!isSide)
            {
                g.DrawLine(Pens.Red, 0, headbodyY + float.Parse(offsetHead.Text) * 5, displayBox.Width, headbodyY + float.Parse(offsetHead.Text) * 5);
                g.DrawLine(Pens.Green, 0, headbodyY + float.Parse(offsetBody.Text) * 5, displayBox.Width, headbodyY + float.Parse(offsetBody.Text) * 5);
                g.DrawLine(Pens.Blue, 0, headbodyY + float.Parse(offsetArms.Text) * 5, displayBox.Width, headbodyY + float.Parse(offsetArms.Text) * 5);
                g.DrawLine(Pens.Purple, 0, legY + float.Parse(offsetLegs.Text) * 5, displayBox.Width, legY + float.Parse(offsetLegs.Text) * 5);
            }
            g.DrawLine(Pens.Red, centerWidthPoint, 0, centerWidthPoint, displayBox.Height);
            g.DrawLine(Pens.Blue, centerWidthPoint + 30, 0, centerWidthPoint + 30, displayBox.Height);
            g.DrawLine(Pens.Blue, centerWidthPoint - 30, 0, centerWidthPoint - 30, displayBox.Height);
            g.DrawLine(Pens.Purple, centerWidthPoint - 10, 0, centerWidthPoint - 10, displayBox.Height);
            g.DrawLine(Pens.Purple, centerWidthPoint + 10, 0, centerWidthPoint + 10, displayBox.Height);
        }

        private void DrawArmorOffsets(Graphics g)
        {
            int centerPointHeight = displayBox.Height / 2;
            int centerPointWidth = displayBox.Width / 2;
            int headbodyY = centerPointHeight + 25; //25
            int armY = centerPointHeight + 35; // - 60;
            int legY = centerPointHeight + 85; // - 80;
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(80, 50, 50, 75));
            g.FillRectangle(semiTransBrush, centerPointWidth, (float)(headbodyY - 40 /*+ offsetHelmet.Value * 5*/), 40, 40); // Helmet
            bool isSide = direction == eViewDirection.left || direction == eViewDirection.right;
            if (isSide)
            {
                g.FillRectangle(semiTransBrush, centerPointWidth - 10, headbodyY, 20, 60); // Chest
                g.FillRectangle(semiTransBrush, centerPointWidth - 10, (float)(legY + 40 /*+ offsetBoots.Value * 5*/), 20, 20); // Boots
                g.FillRectangle(semiTransBrush, centerPointWidth - 10, (float)(legY /*+ offsetPants.Value * 5*/), 20, 40); // Pants
                g.FillRectangle(semiTransBrush, centerPointWidth - 5, (float)(armY + 45 /*+ offsetTool.Value * 5*/), 10, 10); // Tools
            }
            else
            {
                g.FillRectangle(semiTransBrush, centerPointWidth - 20, headbodyY, 40, 60); // Chest
                g.FillRectangle(semiTransBrush, centerPointWidth - 35, (float)(armY + 45 /*+ offsetTool.Value * 5*/), 10, 10); // Tool0
                g.FillRectangle(semiTransBrush, centerPointWidth + 25, (float)(armY + 45 /*+ offsetTool.Value * 5*/), 10, 10); // Tool1
                g.FillRectangle(semiTransBrush, centerPointWidth - 20, (float)(legY /*+ offsetPants.Value * 5*/), 20, 40); // Pants0
                g.FillRectangle(semiTransBrush, centerPointWidth, (float)(legY /*+ offsetPants.Value * 5*/), 20, 40); // Pants1
                g.FillRectangle(semiTransBrush, centerPointWidth - 20, (float)(legY + 40 /*+ offsetBoots.Value * 5*/), 20, 20); // Boot0
                g.FillRectangle(semiTransBrush, centerPointWidth, (float)(legY + 40 /*+ offsetBoots.Value * 5*/), 20, 20); // Boot1
            }

        }


        //Loads Columns
        private void generateModel_Load(object sender, EventArgs e)
        {
            if (Screen.PrimaryScreen.Bounds.Height >= 780 && Screen.PrimaryScreen.Bounds.Width >= 1080) {
                return;
            }
            render();
        }


        //Creates Item
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modelParts.Add(new ModelPart("New Part", 0, 0, 0, 0, 0, 0, 0, 0));
            updateListView();
            render();
        }


        //Manages the selection of a item
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeColorToolStripMenuItem.Visible = false;
            if (listViewBoxes.SelectedItems.Count != 0 && listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                changeColorToolStripMenuItem.Visible = true;
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                //graphics.DrawRectangle(Pens.Yellow, x + (float)double.Parse(this.selected.SubItems[3].Text) * 5 - 1, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5 - 1, (float)double.Parse(this.selected.SubItems[6].Text) * 5 + 2, (float)double.Parse(this.selected.SubItems[5].Text) * 5 + 2);
                //graphics.DrawRectangle(Pens.Black, x + (float)double.Parse(this.selected.SubItems[3].Text) * 5, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5, (float)double.Parse(this.selected.SubItems[6].Text) * 5, (float)double.Parse(this.selected.SubItems[5].Text) * 5);
                comboParent.Text = part.Type;
                PosXUpDown.Value = (decimal)part.X;
                PosYUpDown.Value = (decimal)part.Y;
                PosZUpDown.Value = (decimal)part.Z;
                SizeXUpDown.Value = (decimal)part.Width;
                SizeYUpDown.Value = (decimal)part.Height;
                SizeZUpDown.Value = (decimal)part.Length;
                TextureXUpDown.Value = part.U;
                TextureYUpDown.Value = part.V;
                render();
            }
        }


        //Changes Item Model Class
        private void comboParent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
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
            //render();
        }

        private void SizeXUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                part.Width = (float)SizeXUpDown.Value;
            }
            updateListView();
            render();
        }

        private void SizeYUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                part.Height = (float)SizeYUpDown.Value;
            }
            updateListView();
            render();
        }

        private void SizeZUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                part.Length = (float)SizeZUpDown.Value;
            }
            updateListView();
            render();
        }

        private void PosXUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                part.X = (float)PosXUpDown.Value;
            }
            updateListView();
            render();
        }


        private void PosYUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                part.Y = (float)PosYUpDown.Value;
            }
            updateListView();
            render();
        }


        private void PosZUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                part.Z = (float)PosZUpDown.Value;
            }
            updateListView();
            render();
        }

        private void rotateRightBtn_Click(object sender, EventArgs e)
        {
            if (direction == eViewDirection.front)
                direction = eViewDirection.left;
            else if (direction == eViewDirection.left)
                direction = eViewDirection.back;
            else if (direction == eViewDirection.back)
                direction = eViewDirection.right;
            else if (direction == eViewDirection.right)
                direction = eViewDirection.front;
            labelView.Text = $"View: {direction}";
            render();
        }

        private void rotateLeftBtn_Click(object sender, EventArgs e)
        {
            if (direction == eViewDirection.front)
                direction = eViewDirection.right;
            else if (direction == eViewDirection.right)
                direction = eViewDirection.back;
            else if (direction == eViewDirection.back)
                direction = eViewDirection.left;
            else if (direction == eViewDirection.left)
                direction = eViewDirection.front;
            labelView.Text = $"View: {direction}";
            render();
        }


        //Sets Texture X-Offset
        private void TextureXUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                part.U = (int)TextureXUpDown.Value;
            }
            updateListView();
            render();
        }


        //Sets texture Y-Offset
        private void TextureYUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                part.V = (int)TextureXUpDown.Value;
            }
            updateListView();
            render();
        }


        //Export Current Skin Texture
        private void buttonEXPORT_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(texturePreview.Image, 64, 64);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image Files | *.png";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
            saveFileDialog.Dispose();
        }


        //Imports Skin Texture
        private void buttonIMPORT_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG Image Files | *.png";
            openFileDialog.Title = "Select Skin Texture";
            if (openFileDialog.ShowDialog() == DialogResult.OK && Image.FromFile(openFileDialog.FileName).Width == Image.FromFile(openFileDialog.FileName).Height)
            {
                checkTextureGenerate.Checked = false;
                Bitmap bitmap = new Bitmap(64, 64);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.DrawImage(Image.FromFile(openFileDialog.FileName), 0, 0, 64, 64);
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                }
                texturePreview.Image = bitmap;
            }
            render();
        }


        //Creates Model Data and Finalizes
        private void buttonDone_Click(object sender, EventArgs e)
        {
            Bitmap bitmap1 = new Bitmap(displayBox.Width, displayBox.Height);
            foreach (var part in modelParts)
            {
                boxes.Add(part.ToProperty());
            }

            Bitmap bitmap2 = new Bitmap(64, 64);
            using (Graphics graphics = Graphics.FromImage(bitmap2))
            {
                graphics.DrawImage(texturePreview.Image, 0, 0, 64, 64);
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
            skinPreview.Image = bitmap1;
            texturePreview.Image.Save(Application.StartupPath + "\\temp.png");
            Close();
        }

        //Renders model after texture change
        private void texturePreview_BackgroundImageChanged(object sender, EventArgs e)
        {
            render();
        }

        //Trigger Dialog to select model part/item color
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                listViewBoxes.SelectedItems[0].ForeColor = colorDialog.Color;
            render();
        }


        //Re-renders head with updated x-offset
        private void offsetHead_TextChanged(object sender, EventArgs e)
        {
            render();
        }


        //Re-renders body with updated x-offset
        private void offsetBody_TextAlignChanged(object sender, EventArgs e)
        {
            render();
        }


        //Loads in model template(Steve)
        private void buttonTemplate_Click(object sender, EventArgs e)
        {
            modelParts.Add(new ModelPart("HEAD", -4, -8, -4, 8, 8, 8, 0, 0));
            modelParts.Add(new ModelPart("BODY", -4, 0, -2, 8, 12, 4, 16, 16));
            modelParts.Add(new ModelPart("ARM0", -3, -2, -2, 4, 12, 4, 40, 16));
            modelParts.Add(new ModelPart("ARM1", -1, -2, -2, 4, 12, 4, 40, 16));
            modelParts.Add(new ModelPart("LEG0", -2, 0, -2, 4, 12, 4, 0, 16));
            modelParts.Add(new ModelPart("LEG1", -2, 0, -2, 4, 12, 4, 0, 16));
            comboParent.Enabled = true;
            updateListView();
            render();
        }

        private void updateListView()
        {
            listViewBoxes.Items.Clear();
            foreach (var part in modelParts)
            {
                ListViewItem listViewItem = new ListViewItem(part.Type);
                listViewItem.Tag = part;
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.X.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Y.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Z.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Width.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Height.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.Length.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.U.ToString()));
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, part.V.ToString()));
                listViewBoxes.Items.Add(listViewItem);
            }
        }

        //Exports model (int)doubleo reusable project file
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


        //Imports model from reusable project file
        private void buttonImportModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Custom Skin Model File | *.CSM";
            openFileDialog.Title = "Select Custom Skin Model File";
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                listViewBoxes.Items.Clear();
                string str1 = File.ReadAllText(openFileDialog.FileName);

                modelParts.Clear();
                List<string> lines = str1.Split(new[] { "\n\r", "\n" }, StringSplitOptions.None).ToList();
                if (string.IsNullOrEmpty(lines[lines.Count - 1]))
                    lines.RemoveAt(lines.Count - 1);
                int currentLine = 0;
                int passedlines = 0;
                for (int i = 0; i < lines.Count;)
                {
                    string name = lines[0 + passedlines];
                    string parent = lines[1 + passedlines];
                    float PosX = float.Parse(lines[3 + passedlines]);
                    float PosY = float.Parse(lines[4 + passedlines]);
                    float PosZ = float.Parse(lines[5 + passedlines]);
                    float SizeX = float.Parse(lines[6 + passedlines]);
                    float SizeY = float.Parse(lines[7 + passedlines]);
                    float SizeZ = float.Parse(lines[8 + passedlines]);
                    int UvX = int.Parse(lines[9 + passedlines]);
                    int UvY = int.Parse(lines[10 + passedlines]);
                    passedlines += 11;
                    i += 11;
                    modelParts.Add(new ModelPart(parent, PosX, PosY, PosZ, SizeX, SizeY, SizeZ, UvX, UvY));
                }
            }
            comboParent.Enabled = true;
            updateListView();
            render();
        }


        //Clones Item
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


        //Deletes Item
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems[0] == null)
                return;
            listViewBoxes.SelectedItems[0].Remove();
            render();
        }

        //Changes item color
        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                listViewBoxes.SelectedItems[0].ForeColor = colorDialog.Color;
            render();
        }

        //Re-renders tool with updated x-offset
        private void offsetTool_TextChanged(object sender, EventArgs e)
        {
            render();
        }

        //Re-renders helmet with updated x-offset
        private void offsetHelmet_TextChanged(object sender, EventArgs e)
        {
            render();
        }

        //Re-renders pants with updated x-offset
        private void offsetPants_TextChanged(object sender, EventArgs e)
        {
            render();
        }

        //Re-renders leggings with updated x-offset
        private void offsetLeggings_TextChanged(object sender, EventArgs e)
        {
            render();
        }

        //Re-renders boots with updated x-offset
        private void offsetBoots_TextChanged(object sender, EventArgs e)
        {
            render();
        }

        //Item Selection
        private void listView1_Click(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 && listViewBoxes.SelectedItems[0] != null &&
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                var part = listViewBoxes.SelectedItems[0].Tag as ModelPart;
                comboParent.Text = part.Type;
                PosXUpDown.Value = (decimal)part.X;
                PosYUpDown.Value = (decimal)part.Y;
                PosZUpDown.Value = (decimal)part.Z;
                SizeXUpDown.Value = (decimal)part.Width;
                SizeYUpDown.Value = (decimal)part.Height;
                SizeZUpDown.Value = (decimal)part.Length;
                TextureXUpDown.Value = part.U;
                TextureYUpDown.Value = part.V;
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
            render();
        }

        //currently scrapped
        private void generateModel_FormClosing(object sender, FormClosingEventArgs e)
        {/*
            if (MessageBox.Show("You done here?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
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
                listViewBoxes.SelectedItems[0].Tag is ModelPart)
            {
                if (modelParts.Remove(listViewBoxes.SelectedItems[0].Tag as ModelPart))
                    listViewBoxes.SelectedItems[0].Remove();
                render();
            }
        }

        private void generateModel_SizeChanged(object sender, EventArgs e)
        {
            render();
        }


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
                IEnumerator enumerator = listView.Items.GetEnumerator();
                try
                {
                label_33:
                    if (enumerator.MoveNext())
                    {
                        ListViewItem current = (ListViewItem)enumerator.Current;
                        ListViewItem listViewItem = new ListViewItem();
                        int num4 = 0;
                        do
                        {
                            foreach (string text in str1.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            {
                                ++num4;
                                if (num4 == 1 + 11 * current.Index)
                                    listViewItem.Text = text;
                                else if (num4 == 2 + 11 * current.Index)
                                    listViewItem.Tag = (object)text;
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
                                    this.listViewBoxes.Items.Add(listViewItem);
                                }
                            }
                        }
                        while (num4 < x);
                        goto label_33;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }
            }
            render();
        }

        public string JSONToCSM(string InputFilePath)
        {
            dynamic jsonDe = JsonConvert.DeserializeObject<CSMJObject>(File.ReadAllText(InputFilePath));
            string CSMData = "";
            foreach (CSMJObjectGroup group in jsonDe.groups)
            {
                string PARENT = group.name;
                foreach (int i in group.children)
                {
                    string name = jsonDe.elements[i].name;
                    float PosX = jsonDe.elements[i].from[0] + group.origin[0];
                    float PosY = jsonDe.elements[i].from[1] + group.origin[1];
                    float PosZ = jsonDe.elements[i].from[2] + group.origin[2];
                    float SizeX = jsonDe.elements[i].to[0] - jsonDe.elements[i].from[0];
                    float SizeY = jsonDe.elements[i].to[1] - jsonDe.elements[i].from[1];
                    float SizeZ = jsonDe.elements[i].to[2] - jsonDe.elements[i].from[2];
                    float U = 0;
                    float V = 0;

                    CSMData += name + "\n" + PARENT + "\n" + name + "\n" + PosX + "\n" + PosY + "\n" + PosZ + "\n" + SizeX + "\n" + SizeY + "\n" + SizeZ + "\n" + U + "\n" + V + "\n";
                }
            }
            return CSMData;
        }
    }

    class CSMJObject
    {
        public string credit;
        public int[] texture_size;
        public CSMJObjectElement[] elements;
        public CSMJObjectGroup[] groups;
    }
    class CSMJObjectElement
    {
        public string name;
        public float[] from;
        public float[] to;
    }
    class CSMJObjectGroup
    {
        public string name;
        public float[] origin;
        public int[] children;
    }
}