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
    [Obsolete]
    public partial class generateModel : MetroForm
    {
        [Obsolete("We don't need a full control to get an image")]
        private PictureBox skinPreview = new PictureBox();

        private Image _previewImage;
        public Image PreviewImage => _previewImage;

        private ViewDirection direction = ViewDirection.front;

        private enum ViewDirection
        {
            front,
            right,
            back,
            left,
        }

        private PckAsset _file;
        private SkinANIM _ANIM;

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

        public generateModel(PckAsset file)
        {
            MessageBox.Show(this, "This feature is now considered deprecated and will no longer recieve updates. A better alternative is currently under development. Use at your own risk.", "Deprecated Feature", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            InitializeComponent();

            _file = file;
            if (file.Size > 0)
            {
                using (var ms = new MemoryStream(file.Data))
                {
                    uvPictureBox.Image = Image.FromStream(ms);
                }
            }
            comboParent.Items.Clear();
            comboParent.Items.AddRange(ValidModelBoxTypes);
            LoadData(file);
        }
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        private void LoadData(PckAsset file)
        {
            comboParent.Enabled = file.GetMultipleProperties("BOX").All(kv => {
                var box = SkinBOX.FromString(kv.Value);
                if (ValidModelBoxTypes.Contains(box.Type))
                {
                    modelBoxes.Add(box);
                    return true;
                }
                return false;
            });
            file.GetMultipleProperties("OFFSET").All(kv => {
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

            _ANIM = file.GetProperty("ANIM", SkinANIM.FromString);
            UpdateListView();
            Rerender();
        }

        //Rename model part/item
        private void listView1_DoubleClick_1(object sender, EventArgs e)
        {
            listViewBoxes.SelectedItems[0].BeginEdit();
        }

        private void Rerender([CallerMemberName] string caller = default!)
        {
            Debug.WriteLine($"Call from {caller}", category: nameof(Rerender));
            Render(this, EventArgs.Empty);
            if (generateTextureCheckBox.Checked)
                GenerateUVTextureMap();
        }

        // Graphic Rendering
        // Builds an image based on the view
        private void Render(object sender, EventArgs e)
        {
            buttonTemplate.Enabled = listViewBoxes.Items.Count == 0;
            OrganizesZLayer();
            Bitmap bitmapModelPreview = new Bitmap(displayBox.Width, displayBox.Height); // Creates Model Display layer
            using (Graphics graphics = Graphics.FromImage(bitmapModelPreview))
            {
                graphics.ApplyConfig(_graphicsConfig);
                graphics.Clear(_backgroundColor);

                float headbodyY = (displayBox.Height / 2) + 25; //  25
                float armY = (displayBox.Height / 2) + 35; // -60;
                float legY = (displayBox.Height / 2) + 85; // -80;
                float groundLevel = (displayBox.Height / 2) + 145;
                graphics.DrawLine(Pens.White, 0, groundLevel, displayBox.Width, groundLevel);
                float renderScale = uvPictureBox.Image.Width / 64; // used for displaying larger graphics properly; 64 is the base skin width for all models

                // Chooses Render settings based on current direction
                foreach (ListViewItem listViewItem in listViewBoxes.Items)
                {
                    if (!(listViewItem.Tag is SkinBOX part))
                        continue;
                    float x = displayBox.Width / 2;
                    float y = 0;

                    switch (direction)
                    {
                        case ViewDirection.front:
                            {
                                //Sets X & Y based on model part class
                                // listViewItem.Text -> part.Type
                                // listViewItem.SubItems[1] -> part.Pos.X
                                // listViewItem.SubItems[2] -> part.Pos.Y
                                // listViewItem.SubItems[3] -> part.Pos.Z
                                // listViewItem.SubItems[4] -> part.Size.X
                                // listViewItem.SubItems[5] -> part.Size.Y
                                // listViewItem.SubItems[6] -> part.Size.Z
                                // listViewItem.SubItems[7] -> part.U
                                // listViewItem.SubItems[8] -> part.V
                                switch (part.Type)
                                {
                                    case "HEAD":
                                    case "HEADWEAR":
                                    case "HELMET":
                                        y = headbodyY + int.Parse(offsetHead.Text) * 5;
                                        break;
                                    case "BODY":
                                    case "JACKET":
                                    case "CHEST":
                                    case "BODYARMOR":
                                    case "BELT":
                                    case "WAIST":
                                        y = headbodyY + int.Parse(offsetBody.Text) * 5;
                                        break;

                                    case "ARM0":
                                    case "ARMARMOR0":
                                    case "SLEEVE0":
                                    case "SHOULDER0":
                                        x -= 25;
                                        y = armY + int.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "ARM1":
                                    case "ARMARMOR1":
                                    case "SLEEVE1":
                                    case "SHOULDER1":
                                        x += 25;
                                        y = armY + int.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "LEG0":
                                    case "PANTS0":
                                    case "SOCK0":
                                    case "LEGGING0":
                                    case "BOOT0":
                                        x -= 10;
                                        y = legY + int.Parse(offsetLegs.Text) * 5;
                                        break;

                                    case "LEG1":
                                    case "PANTS1":
                                    case "SOCK1":
                                    case "LEGGING1":
                                    case "BOOT1":
                                        x += 10;
                                        y = legY + int.Parse(offsetLegs.Text) * 5;
                                        break;
                                }

                                // Maps imported Texture if texture generation is disabled
                                if (!generateTextureCheckBox.Checked)
                                {
                                    RectangleF destRect = new RectangleF(
                                        x + part.Pos.X * 5,
                                        y + part.Pos.Y * 5,
                                        part.Size.X * 5,
                                        part.Size.Y * 5);
                                    RectangleF srcRect = new RectangleF(
                                        (part.UV.X + part.Size.Z) * renderScale,
                                        (part.UV.Y + part.Size.Z) * renderScale,
                                        part.Size.X * renderScale,
                                        part.Size.Y * renderScale);
                                    graphics.DrawImage(uvPictureBox.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    graphics.FillRectangle(new SolidBrush(listViewItem.ForeColor), x + part.Pos.X * 5, y + part.Pos.Y * 5, part.Size.X * 5, part.Size.Y * 5);
                                }

                                break;
                            }

                        case ViewDirection.left:
                            {
                                //Sets X & Y based on model part class
                                switch (part.Type)
                                {
                                    case "HEAD":
                                    case "HEADWEAR":
                                    case "HELMET":
                                        y = headbodyY + int.Parse(offsetHead.Text) * 5;
                                        break;
                                    case "BODY":
                                    case "JACKET":
                                    case "CHEST":
                                    case "BODYARMOR":
                                    case "BELT":
                                    case "WAIST":
                                        y = headbodyY + int.Parse(offsetBody.Text) * 5;
                                        break;

                                    case "ARM0":
                                    case "ARMARMOR0":
                                    case "SLEEVE0":
                                    case "SHOULDER0":
                                        y = armY + int.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "ARM1":
                                    case "ARMARMOR1":
                                    case "SLEEVE1":
                                    case "SHOULDER1":
                                        y = armY + int.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "LEG0":
                                    case "PANTS0":
                                    case "SOCK0":
                                    case "LEGGING0":
                                    case "BOOT0":
                                        y = legY + int.Parse(offsetLegs.Text) * 5;
                                        break;

                                    case "LEG1":
                                    case "PANTS1":
                                    case "SOCK1":
                                    case "LEGGING1":
                                    case "BOOT1":
                                        y = legY + int.Parse(offsetLegs.Text) * 5;
                                        break;
                                }

                                // Maps imported Texture if auto texture is disabled
                                if (!generateTextureCheckBox.Checked)
                                {
                                    RectangleF destRect = new RectangleF(
                                        x + part.Pos.Z * 5,
                                        y + part.Pos.Y * 5,
                                        part.Size.Z * 5,
                                        part.Size.Y * 5);
                                    RectangleF srcRect = new RectangleF(
                                        (part.UV.X + part.Size.Z + part.Size.X) * renderScale,
                                        (part.UV.Y + part.Size.Z) * renderScale,
                                        part.Size.Z * renderScale,
                                        part.Size.Y * renderScale);
                                    graphics.DrawImage(uvPictureBox.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    //Draws Part
                                    graphics.FillRectangle(new SolidBrush(listViewItem.ForeColor), x + part.Pos.Z * 5, y + part.Pos.Y * 5, part.Size.Z * 5, part.Size.Y * 5);
                                }
                                bitmapModelPreview.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                break;
                            }

                        case ViewDirection.back:
                            {
                                //Sets X & Y based on model part class
                                switch (part.Type)
                                {
                                    case "HEAD":
                                    case "HEADWEAR":
                                    case "HELMET":
                                        y = headbodyY + int.Parse(offsetHead.Text) * 5;
                                        break;
                                    case "BODY":
                                    case "JACKET":
                                    case "CHEST":
                                    case "BODYARMOR":
                                    case "BELT":
                                    case "WAIST":
                                        y = headbodyY + int.Parse(offsetBody.Text) * 5;
                                        break;

                                    case "ARM0":
                                    case "ARMARMOR0":
                                    case "SLEEVE0":
                                    case "SHOULDER0":
                                        x -= 25;
                                        y = armY + int.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "ARM1":
                                    case "ARMARMOR1":
                                    case "SLEEVE1":
                                    case "SHOULDER1":
                                        x += 25;
                                        y = armY + int.Parse(offsetArms.Text) * 5;
                                        break;

                                    case "LEG0":
                                    case "PANTS0":
                                    case "SOCK0":
                                    case "LEGGING0":
                                    case "BOOT0":
                                        x -= 10;
                                        y = legY + int.Parse(offsetLegs.Text) * 5;
                                        break;

                                    case "LEG1":
                                    case "PANTS1":
                                    case "SOCK1":
                                    case "LEGGING1":
                                    case "BOOT1":
                                        x += 10;
                                        y = legY + int.Parse(offsetLegs.Text) * 5;
                                        break;
                                }

                                //Maps imported Texture if auto texture is disabled
                                if (!generateTextureCheckBox.Checked)
                                {
                                    RectangleF destRect = new RectangleF(
                                        x + part.Pos.X * 5,
                                        y + part.Pos.Y * 5,
                                        part.Size.X * 5,
                                        part.Size.Y * 5);
                                    RectangleF srcRect = new RectangleF(
                                        (part.UV.X + part.Size.Z * 2 + part.Size.X) * renderScale,
                                        (part.UV.Y + part.Size.Z) * renderScale,
                                        part.Size.X * renderScale,
                                        part.Size.Y * renderScale);
                                    graphics.DrawImage(uvPictureBox.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    //Draws Part
                                    graphics.FillRectangle(new SolidBrush(listViewItem.ForeColor), x + part.Pos.X * 5, y + part.Pos.Y * 5, part.Size.X * 5, part.Size.Y * 5);
                                }
                                bitmapModelPreview.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                break;
                            }

                        case ViewDirection.right:
                            //Sets X & Y based on model part class
                            switch (part.Type)
                            {
                                case "HEAD":
                                case "HEADWEAR":
                                case "HELMET":
                                    y = headbodyY + int.Parse(offsetHead.Text) * 5;
                                    break;
                                case "BODY":
                                case "JACKET":
                                case "CHEST":
                                case "BODYARMOR":
                                case "BELT":
                                case "WAIST":
                                    y = headbodyY + int.Parse(offsetBody.Text) * 5;
                                    break;

                                case "ARM0":
                                case "ARMARMOR0":
                                case "SLEEVE0":
                                case "SHOULDER0":
                                    y = armY + int.Parse(offsetArms.Text) * 5;
                                    break;

                                case "ARM1":
                                case "ARMARMOR1":
                                case "SLEEVE1":
                                case "SHOULDER1":
                                    y = armY + int.Parse(offsetArms.Text) * 5;
                                    break;

                                case "LEG0":
                                case "PANTS0":
                                case "SOCK0":
                                case "LEGGING0":
                                case "BOOT0":
                                    y = legY + int.Parse(offsetLegs.Text) * 5;
                                    break;

                                case "LEG1":
                                case "PANTS1":
                                case "SOCK1":
                                case "LEGGING1":
                                case "BOOT1":
                                    y = legY + int.Parse(offsetLegs.Text) * 5;
                                    break;
                            }
                            //Maps imported Texture if auto texture is disabled
                            if (!generateTextureCheckBox.Checked)
                            {
                                RectangleF destRect = new RectangleF(
                                    x + part.Pos.Z * 5,
                                    y + part.Pos.Y * 5,
                                    part.Size.Z * 5,
                                    part.Size.Y * 5);
                                RectangleF srcRect = new RectangleF(
                                    (part.UV.X + part.Size.Z + part.Size.X) * renderScale,
                                    (part.UV.Y + part.Size.Z) * renderScale,
                                    part.Size.Z * renderScale,
                                    part.Size.Y * renderScale);
                                graphics.DrawImage(uvPictureBox.Image, destRect, srcRect, GraphicsUnit.Pixel);
                            }
                            else
                            {
                                //Draws Part
                                graphics.FillRectangle(new SolidBrush(listViewItem.ForeColor), x + part.Pos.Z * 5, y + part.Pos.Y * 5, part.Size.Z * 5, part.Size.Y * 5);
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
            displayBox.Image = bitmapModelPreview;
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

        // Checks and sets Z layering
        private void OrganizesZLayer()
        {
            foreach (ListViewItem listViewItem in listViewBoxes.Items)
                listViewItem.SubItems.Add("unchecked");

            float surfaceCenter = displayBox.Width / 2;

            switch (direction)
            {
                case ViewDirection.front:
                    {
                        foreach (ListViewItem listViewItemCurrent in listViewBoxes.Items)
                        {
                            if (listViewItemCurrent.SubItems[9].Text == "unchecked")
                            {
                                float x = 0;
                                if (listViewItemCurrent.Text == "HEAD")
                                    x = surfaceCenter;
                                else if (listViewItemCurrent.Text == "BODY")
                                    x = surfaceCenter;
                                else if (listViewItemCurrent.Text == "ARM0")
                                    x = 178;
                                else if (listViewItemCurrent.Text == "ARM1")
                                    x = 228;
                                else if (listViewItemCurrent.Text == "LEG0")
                                    x = 193;
                                else if (listViewItemCurrent.Text == "LEG1")
                                    x = 213;

                                bool flag = false;
                                int index = listViewItemCurrent.Index;
                                foreach (ListViewItem listViewItemComparing in listViewBoxes.Items)
                                {
                                    var val1 = double.Parse(listViewItemCurrent.SubItems[3].Text) + double.Parse(listViewItemCurrent.SubItems[6].Text);
                                    var val2 = double.Parse(listViewItemComparing.SubItems[3].Text) + double.Parse(listViewItemComparing.SubItems[6].Text);
                                    if (listViewItemComparing.SubItems[9].Text == "unchecked" &&
                                        val1 < val2)
                                    {
                                        if (listViewItemComparing.Index < listViewBoxes.Items.Count + 1)
                                        {
                                            index = listViewItemComparing.Index + 1;
                                            flag = true;
                                        }
                                    }
                                }
                                listViewItemCurrent.SubItems[9].Text = "checked";
                                if (flag)
                                {
                                    ListViewItem listViewItem2 = (ListViewItem)listViewItemCurrent.Clone();
                                    listViewBoxes.Items.Insert(index, listViewItem2);
                                    listViewItemCurrent.Remove();
                                }
                            }
                        }
                    }
                    break;
                case ViewDirection.right:
                    {
                        int checkedItems = 0;
                        do
                        {
                            foreach (ListViewItem listViewItemCurrent in listViewBoxes.Items)
                            {
                                if (listViewItemCurrent.SubItems[listViewItemCurrent.SubItems.Count - 1].Text == "unchecked")
                                {
                                    float x = 0;
                                    if (listViewItemCurrent.Text == "HEAD")
                                        x = surfaceCenter;
                                    else if (listViewItemCurrent.Text == "BODY")
                                        x = surfaceCenter;
                                    else if (listViewItemCurrent.Text == "ARM0")
                                        x = 178;
                                    else if (listViewItemCurrent.Text == "ARM1")
                                        x = 228;
                                    else if (listViewItemCurrent.Text == "LEG0")
                                        x = 193;
                                    else if (listViewItemCurrent.Text == "LEG1")
                                        x = 213;
                                    bool flag = false;
                                    int index = listViewItemCurrent.Index;
                                    foreach (ListViewItem listViewItem2 in listViewBoxes.Items)
                                    {
                                        if (listViewItem2.SubItems[9].Text == "unchecked")
                                        {
                                            int y = 0;
                                            if (listViewItem2.Text == "HEAD")
                                                y = (int)surfaceCenter;
                                            else if (listViewItem2.Text == "BODY")
                                                y = (int)surfaceCenter;
                                            else if (listViewItem2.Text == "ARM0")
                                                y = 178;
                                            else if (listViewItem2.Text == "ARM1")
                                                y = 228;
                                            else if (listViewItem2.Text == "LEG0")
                                                y = 193;
                                            else if (listViewItem2.Text == "LEG1")
                                                y = 213;
                                            if ((int)double.Parse(listViewItemCurrent.SubItems[1].Text) + (int)double.Parse(listViewItemCurrent.SubItems[4].Text) - x > (int)double.Parse(listViewItem2.SubItems[1].Text) + (int)double.Parse(listViewItem2.SubItems[4].Text) + y && listViewItem2.Index + 1 < this.listViewBoxes.Items.Count + 1)
                                            {
                                                index = listViewItem2.Index + 1;
                                                flag = true;
                                            }
                                        }
                                    }
                                    listViewItemCurrent.SubItems[9].Text = "checked";
                                    checkedItems += 1;
                                    if (flag)
                                    {
                                        ListViewItem listViewItem2 = (ListViewItem)listViewItemCurrent.Clone();
                                        listViewBoxes.Items.Insert(index, listViewItem2);
                                        if (listViewBoxes.SelectedItems.Count != 0)
                                        {
                                            //if (selected.Index == listViewItem1.Index)
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
                    break;
                case ViewDirection.back:
                    {
                        int checkedItems = 0;
                        do
                        {
                            foreach (ListViewItem listViewItemCurrent in listViewBoxes.Items)
                            {
                                if (listViewItemCurrent.SubItems[listViewItemCurrent.SubItems.Count - 1].Text == "unchecked")
                                {
                                    bool flag = false;
                                    int index = listViewItemCurrent.Index;
                                    foreach (ListViewItem listViewItemComparing in listViewBoxes.Items)
                                    {
                                        if (listViewItemComparing.SubItems[9].Text == "unchecked" && (int)double.Parse(listViewItemCurrent.SubItems[3].Text) + (int)double.Parse(listViewItemCurrent.SubItems[6].Text) > (int)double.Parse(listViewItemComparing.SubItems[3].Text) + (int)double.Parse(listViewItemComparing.SubItems[6].Text))
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
                                    if (flag)
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
                    break;
                case ViewDirection.left:
                    {
                        int checkedItems = 0;
                        do
                        {
                            foreach (ListViewItem listViewItemCurrent in listViewBoxes.Items)
                            {
                                if (listViewItemCurrent.SubItems[listViewItemCurrent.SubItems.Count - 1].Text == "unchecked")
                                {
                                    float x = 0;
                                    if (listViewItemCurrent.Text == "HEAD")
                                        x = surfaceCenter;
                                    else if (listViewItemCurrent.Text == "BODY")
                                        x = surfaceCenter;
                                    else if (listViewItemCurrent.Text == "ARM0")
                                        x = 178;
                                    else if (listViewItemCurrent.Text == "ARM1")
                                        x = 228;
                                    else if (listViewItemCurrent.Text == "LEG0")
                                        x = 193;
                                    else if (listViewItemCurrent.Text == "LEG1")
                                        x = 213;
                                    bool flag = false;
                                    int index = listViewItemCurrent.Index;
                                    foreach (ListViewItem listViewItem2 in listViewBoxes.Items)
                                    {
                                        if (listViewItem2.SubItems[9].Text == "unchecked")
                                        {
                                            int y = 0;
                                            if (listViewItem2.Text == "HEAD")
                                                y = (int)surfaceCenter;
                                            else if (listViewItem2.Text == "BODY")
                                                y = (int)surfaceCenter;
                                            else if (listViewItem2.Text == "ARM0")
                                                y = 178;
                                            else if (listViewItem2.Text == "ARM1")
                                                y = 228;
                                            else if (listViewItem2.Text == "LEG0")
                                                y = 193;
                                            else if (listViewItem2.Text == "LEG1")
                                                y = 213;
                                            if ((int)double.Parse(listViewItemCurrent.SubItems[1].Text) + (int)double.Parse(listViewItemCurrent.SubItems[4].Text) + x < (int)double.Parse(listViewItem2.SubItems[1].Text) + (int)double.Parse(listViewItem2.SubItems[4].Text) + y && listViewItem2.Index + 1 < this.listViewBoxes.Items.Count + 1)
                                            {
                                                index = listViewItem2.Index + 1;
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
                                            //if (selected.Index == listViewItem1.Index)
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
                    break;
                default:
                    break;
            }
        }

        private void DrawGuideLines(Graphics g)
        {
            Point center = new Point(displayBox.Height / 2, displayBox.Width / 2);
            int headbodyY = center.Y + 25; //25
            int legY = center.Y + 85; // - 80;
            bool isSide = direction == ViewDirection.left || direction == ViewDirection.right;
            if (!isSide)
            {
                g.DrawLine(Pens.Red, 0, headbodyY + float.Parse(offsetHead.Text) * 5, displayBox.Width, headbodyY + float.Parse(offsetHead.Text) * 5);
                g.DrawLine(Pens.Green, 0, headbodyY + float.Parse(offsetBody.Text) * 5, displayBox.Width, headbodyY + float.Parse(offsetBody.Text) * 5);
                g.DrawLine(Pens.Blue, 0, headbodyY + float.Parse(offsetArms.Text) * 5, displayBox.Width, headbodyY + float.Parse(offsetArms.Text) * 5);
                g.DrawLine(Pens.Purple, 0, legY + float.Parse(offsetLegs.Text) * 5, displayBox.Width, legY + float.Parse(offsetLegs.Text) * 5);
            }
            g.DrawLine(Pens.Red, center.X, 0, center.X, displayBox.Height);
            g.DrawLine(Pens.Blue, center.X + 30, 0, center.X + 30, displayBox.Height);
            g.DrawLine(Pens.Blue, center.X - 30, 0, center.X - 30, displayBox.Height);
            g.DrawLine(Pens.Purple, center.X - 10, 0, center.X - 10, displayBox.Height);
            g.DrawLine(Pens.Purple, center.X + 10, 0, center .X + 10, displayBox.Height);
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
            bool isSide = direction == ViewDirection.left || direction == ViewDirection.right;
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

        private void generateModel_Load(object sender, EventArgs e)
        {
            if (Screen.PrimaryScreen.Bounds.Height >= 780 && Screen.PrimaryScreen.Bounds.Width >= 1080) {
                return;
            }
            Rerender();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modelBoxes.Add(SkinBOX.Empty);
            UpdateListView();
            Rerender();
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
                Rerender();
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
            Rerender();
        }

        private void SizeXUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Size.X = (float)SizeXUpDown.Value;
            }
            UpdateListView();
            Rerender();
        }

        private void SizeYUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Size.Y = (float)SizeYUpDown.Value;
            }
            UpdateListView();
            Rerender();
        }

        private void SizeZUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Size.Z = (float)SizeZUpDown.Value;
            }
            UpdateListView();
            Rerender();
        }

        private void PosXUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Pos.X = (float)PosXUpDown.Value;
            }
            UpdateListView();
            Rerender();
        }


        private void PosYUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Pos.Y = (float)PosYUpDown.Value;
            }
            UpdateListView();
            Rerender();
        }


        private void PosZUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0 &&
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                part.Pos.Z = (float)PosZUpDown.Value;
            }
            UpdateListView();
            Rerender();
        }

        private void rotateRightBtn_Click(object sender, EventArgs e)
        {
            if (direction == ViewDirection.front)
                direction = ViewDirection.left;
            else
                --direction;
            labelView.Text = $"View: {direction}";
            Rerender();
        }

        private void rotateLeftBtn_Click(object sender, EventArgs e)
        {
            if (direction == ViewDirection.left)
                direction = ViewDirection.front;
            else
                ++direction;
            labelView.Text = $"View: {direction}";
            Rerender();
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
            Rerender();
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
            Rerender();
        }


        //Export Current Skin Texture
        private void buttonEXPORT_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(uvPictureBox.Image, 64, 64);
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image Files | *.png";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
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

            if (openFileDialog.ShowDialog(this) == DialogResult.OK) // skins can only be a 1:1 ratio (base 64x64) or a 2:1 ratio (base 64x32)
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
                        Rerender();
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
                _file.AddProperty("BOX", part);
            }

            //Bitmap bitmap2 = new Bitmap(64, 64);
            //using (Graphics graphics = Graphics.FromImage(bitmap2))
            //{
            //    graphics.ApplyConfig(_graphicsConfig);
            //    graphics.DrawImage(uvPictureBox.Image, 0, 0, 64, 64);
            //}
            _previewImage = new Bitmap(displayBox.Width, displayBox.Height);
            Close();
        }

        // Renders model after texture change
        private void texturePreview_BackgroundImageChanged(object sender, EventArgs e)
        {
            Rerender();
        }

        // Trigger Dialog to select model part/item color
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                listViewBoxes.SelectedItems[0].ForeColor = colorDialog.Color;
            Rerender();
        }


        //Re-renders head with updated x-offset
        private void offsetHead_TextChanged(object sender, EventArgs e)
        {
            Rerender();
        }


        //Re-renders body with updated x-offset
        private void offsetBody_TextAlignChanged(object sender, EventArgs e)
        {
            Rerender();
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
            Rerender();
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
            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
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
            if (MessageBox.Show(this, "Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog(this) == DialogResult.OK)
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
            Rerender();
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
                MessageBox.Show(this, "Please Select a Part");
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems[0] == null)
                return;
            listViewBoxes.SelectedItems[0].Remove();
            Rerender();
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                listViewBoxes.SelectedItems[0].ForeColor = colorDialog.Color;
            Rerender();
        }

        //Re-renders tool with updated x-offset
        private void offsetTool_TextChanged(object sender, EventArgs e)
        {
            Rerender();
        }

        //Re-renders helmet with updated x-offset
        private void offsetHelmet_TextChanged(object sender, EventArgs e)
        {
            Rerender();
        }

        //Re-renders pants with updated x-offset
        private void offsetPants_TextChanged(object sender, EventArgs e)
        {
            Rerender();
        }

        //Re-renders leggings with updated x-offset
        private void offsetLeggings_TextChanged(object sender, EventArgs e)
        {
            Rerender();
        }

        //Re-renders boots with updated x-offset
        private void offsetBoots_TextChanged(object sender, EventArgs e)
        {
            Rerender();
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
            Rerender();
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
                listViewBoxes.SelectedItems[0].Tag is SkinBOX part)
            {
                if (modelBoxes.Remove(part))
                    listViewBoxes.SelectedItems[0].Remove();
                Rerender();
            }
        }

        private void generateModel_SizeChanged(object sender, EventArgs e)
        {
            Rerender();
        }

        // TODO
        private void OpenJSONButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Model File | *.JSON";
            openFileDialog.Title = "Select JSON Model File";
            if (MessageBox.Show(this, "Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog(this) == DialogResult.OK)
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
            Rerender();
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