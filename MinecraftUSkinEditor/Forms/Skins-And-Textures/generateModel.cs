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
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Newtonsoft.Json;


namespace PckStudio
{
    public partial class generateModel : MetroFramework.Forms.MetroForm
    {
        PictureBox skinPreview;
        Bitmap bg;

        string direction;

        List<object[]> boxes;

        ListView storeData = new ListView();

        bool autoTexture = true;

        Color backgroundColor = Color.Black;

        ListViewItem selected;

        //Checks if an item is selected
        private void checkSelect()
        {
            //Deciphers wether to enable/disable things based on wether an item is selected or not
            if (listViewBoxes.SelectedItems.Count != 0 && listViewBoxes.SelectedItems[0] != null)
            {
                textXc.Enabled = true;
                textYc.Enabled = true;
                textZc.Enabled = true;
                textXf.Enabled = true;
                textYf.Enabled = true;
                textZf.Enabled = true;
                textTextureX.Enabled = true;
                textTextureY.Enabled = true;
                buttonXcminus.Enabled = true;
                buttonYcminus.Enabled = true;
                buttonZcminus.Enabled = true;
                buttonXcplus.Enabled = true;
                buttonYcplus.Enabled = true;
                buttonZcplus.Enabled = true;
                buttonXfminus.Enabled = true;
                buttonYfminus.Enabled = true;
                buttonZfminus.Enabled = true;
                buttonXfplus.Enabled = true;
                buttonYfplus.Enabled = true;
                buttonZfplus.Enabled = true;
                comboParent.Enabled = true;
                return;
            }
            textXc.Enabled = false;
            textYc.Enabled = false;
            textZc.Enabled = false;
            textXf.Enabled = false;
            textYf.Enabled = false;
            textZf.Enabled = false;
            textTextureX.Enabled = false;
            textTextureY.Enabled = false;
            buttonXcminus.Enabled = false;
            buttonYcminus.Enabled = false;
            buttonZcminus.Enabled = false;
            buttonXcplus.Enabled = false;
            buttonYcplus.Enabled = false;
            buttonZcplus.Enabled = false;
            buttonXfminus.Enabled = false;
            buttonYfminus.Enabled = false;
            buttonZfminus.Enabled = false;
            buttonXfplus.Enabled = false;
            buttonYfplus.Enabled = false;
            buttonZfplus.Enabled = false;
            comboParent.Enabled = false;
        }


        //Initialization
        public generateModel(List<object[]> boxesIn, PictureBox preview)
        {
            InitializeComponent();
            boxes = new List<object[]>();
            boxes = boxesIn;
            skinPreview = preview;
            direction = "front";
            bg = new Bitmap(this.displayBox.Image);
            buttonIMPORT.Enabled = false;
            buttonEXPORT.Enabled = false;
            textXc.Enabled = false;
            textYc.Enabled = false;
            textZc.Enabled = false;
            textXf.Enabled = false;
            textYf.Enabled = false;
            textZf.Enabled = false;
            textTextureX.Enabled = false;
            textTextureY.Enabled = false;
            buttonXcminus.Enabled = false;
            buttonYcminus.Enabled = false;
            buttonZcminus.Enabled = false;
            buttonXcplus.Enabled = false;
            buttonYcplus.Enabled = false;
            buttonZcplus.Enabled = false;
            buttonXfminus.Enabled = false;
            buttonYfminus.Enabled = false;
            buttonZfminus.Enabled = false;
            buttonXfplus.Enabled = false;
            buttonYfplus.Enabled = false;
            buttonZfplus.Enabled = false;
            comboParent.Enabled = false;

            loadData();
        }

        //loads data from mode list
        private void loadData()
        {
            foreach (object[] box in boxes)
            {
                if (box[0].ToString() == "BOX")
                {
                    int space = 0;
                    string modelClass = "";
                    string x = "";
                    string y = "";
                    string z = "";
                    string xF = "";
                    string yF = "";
                    string zF = "";
                    string xO = "";
                    string yO = "";

                    foreach (char letter in box[1].ToString())
                    {
                        if (letter.ToString() == " ")
                        {
                            space += 1;
                        }
                        else if (space == 0 && letter.ToString() != " ")
                        {
                            modelClass += letter;
                        }
                        else if (space == 1 && letter.ToString() != " ")
                        {
                            x += letter.ToString();
                        }
                        else if (space == 2 && letter.ToString() != " ")
                        {
                            y += letter.ToString();
                        }
                        else if (space == 3 && letter.ToString() != " ")
                        {
                            z += letter.ToString();
                        }
                        else if (space == 4 && letter.ToString() != " ")
                        {
                            xF += letter.ToString();
                        }
                        else if (space == 5 && letter.ToString() != " ")
                        {
                            yF += letter.ToString();
                        }
                        else if (space == 6 && letter.ToString() != " ")
                        {
                            zF += letter.ToString();
                        }
                        else if (space == 7 && letter.ToString() != " ")
                        {
                            xO += letter.ToString();
                        }
                        else if (space == 8 && letter.ToString() != " ")
                        {
                            yO += letter.ToString();
                        }
                    }

                    ListViewItem part = new ListViewItem();
                    part.Text = "BOX";
                    part.SubItems.Add(new ListViewItem.ListViewSubItem(part, x));
                    part.SubItems.Add(new ListViewItem.ListViewSubItem(part, y));
                    part.SubItems.Add(new ListViewItem.ListViewSubItem(part, z));
                    part.SubItems.Add(new ListViewItem.ListViewSubItem(part, xF));
                    part.SubItems.Add(new ListViewItem.ListViewSubItem(part, yF));
                    part.SubItems.Add(new ListViewItem.ListViewSubItem(part, zF));
                    part.SubItems.Add(new ListViewItem.ListViewSubItem(part, xO));
                    part.SubItems.Add(new ListViewItem.ListViewSubItem(part, yO));
                    part.Tag = modelClass;
                    listViewBoxes.Items.Add(part);
                    comboParent.Enabled = true;
                }
            }
            render();
        }

        //Rename model part/item
        private void listView1_DoubleClick_1(object sender, EventArgs e)
        {
            listViewBoxes.SelectedItems[0].BeginEdit();
        }


        //Graphic Rendering
        private void render()
        {
            //Disables template option if model parts exist
            buttonTemplate.Enabled = listViewBoxes.Items.Count == 0;

            setZ(); //Organizes Z layers

            labelView.Text = "View: " + this.direction; //Updates Current Direction label

            try
            {
                Bitmap bitmapModelPreview = new Bitmap(this.displayBox.Width, this.displayBox.Height); //Creates Model Display layer
                using (Graphics graphics = Graphics.FromImage((Image)bitmapModelPreview))
                {
                    graphics.Clear(backgroundColor);
                    int headbodyY = (displayBox.Height / 2) + 25;//25
                    int armY = (displayBox.Height / 2) + 35;// - 60;
                    int legY = (displayBox.Height / 2) + 85;// - 80;
                    int groundLevel = (displayBox.Height / 2) + 145;
                    graphics.DrawLine(Pens.White, 0, groundLevel, displayBox.Width, groundLevel);

                    //Chooses Render settings based on current direction
                    if (this.direction == "front")
                    {
                        //Generates Guidelines if enabled
                        if (this.checkGuide.Checked)
                        {
                            try
                            {
                                graphics.DrawLine(Pens.Red, displayBox.Width / 2, 0, displayBox.Width / 2, displayBox.Height);
                                graphics.DrawLine(Pens.Blue, (displayBox.Width / 2) + 30, 0, (displayBox.Width / 2) + 30, displayBox.Height);
                                graphics.DrawLine(Pens.Blue, (displayBox.Width / 2) - 30, 0, (displayBox.Width / 2) - 30, displayBox.Height);
                                graphics.DrawLine(Pens.Purple, (displayBox.Width / 2) - 10, 0, (displayBox.Width / 2) - 10, displayBox.Height);
                                graphics.DrawLine(Pens.Purple, (displayBox.Width / 2) + 10, 0, (displayBox.Width / 2) + 10, displayBox.Height);
                                graphics.DrawLine(Pens.Red, 0, headbodyY + (float)double.Parse(this.offsetHead.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetHead.Text) * 5);
                                graphics.DrawLine(Pens.Green, 0, headbodyY + (float)double.Parse(this.offsetBody.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetBody.Text) * 5);
                                graphics.DrawLine(Pens.Blue, 0, headbodyY + (float)double.Parse(this.offsetArms.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetArms.Text) * 5);
                                graphics.DrawLine(Pens.Purple, 0, legY + (float)double.Parse(this.offsetLegs.Text) * 5, displayBox.Width, legY + (float)double.Parse(this.offsetLegs.Text) * 5);
                            }
                            catch (Exception)
                            {

                            }
                        }
                        foreach (ListViewItem listViewItem in this.listViewBoxes.Items) //Individually draws each model part/item
                        {
                            int x = 0;
                            int y = 0;
                            try
                            {
                                //Sets X & Y based on model part class
                                if (listViewItem.Tag.ToString() == "HEAD")
                                {
                                    x = displayBox.Width / 2;
                                    y = headbodyY + int.Parse(this.offsetHead.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "BODY")
                                {
                                    x = displayBox.Width / 2;
                                    y = headbodyY + int.Parse(this.offsetBody.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "ARM0")
                                {
                                    x = (displayBox.Width / 2) - 25;
                                    y = armY + int.Parse(this.offsetArms.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "ARM1")
                                {
                                    x = (displayBox.Width / 2) + 25;
                                    y = armY + int.Parse(this.offsetArms.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "LEG0")
                                {
                                    x = (displayBox.Width / 2) - 10;
                                    y = legY + int.Parse(this.offsetLegs.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "LEG1")
                                {
                                    x = (displayBox.Width / 2) + 10;
                                    y = legY + int.Parse(this.offsetLegs.Text) * 5;
                                }
                                //Maps imported Texture if auto texture is disabled
                                if (autoTexture != true)
                                {
                                    RectangleF destRect = new RectangleF(
                                        (float)(x + (float)double.Parse(listViewItem.SubItems[1].Text) * 5),
                                        (float)(y + (float)double.Parse(listViewItem.SubItems[2].Text) * 5),
                                        (float)((float)double.Parse(listViewItem.SubItems[4].Text) * 5),
                                        (float)((float)double.Parse(listViewItem.SubItems[5].Text) * 5));
                                    RectangleF srcRect = new RectangleF(
                                        (float)((float)double.Parse(listViewItem.SubItems[7].Text) +
                                                 (float)double.Parse(listViewItem.SubItems[6].Text)),
                                        (float)((float)double.Parse(listViewItem.SubItems[8].Text) +
                                                 (float)double.Parse(listViewItem.SubItems[6].Text)),
                                        (float)(float)double.Parse(listViewItem.SubItems[4].Text),
                                        (float)(float)double.Parse(listViewItem.SubItems[5].Text));
                                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                    graphics.DrawImage(texturePreview.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    //Draws Part
                                    graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), x + (float)double.Parse(listViewItem.SubItems[1].Text) * 5, y + (float)double.Parse(listViewItem.SubItems[2].Text) * 5, (float)double.Parse(listViewItem.SubItems[4].Text) * 5, (float)double.Parse(listViewItem.SubItems[5].Text) * 5);
                                }

                                //Draws Armor Offsets
                                if (checkBoxArmor.Checked == true)
                                {
                                    SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(80, 50, 50, 75));
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 20, headbodyY - 40 + (float)double.Parse(this.offsetHelmet.Text) * 5, 40, 40);//Helmet
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 35, armY + 45 + (float)double.Parse(this.offsetTool.Text) * 5, 10, 10);//Tool0
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) + 25, armY + 45 + (float)double.Parse(this.offsetTool.Text) * 5, 10, 10);//Tool1
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 20, legY + (float)double.Parse(this.offsetPants.Text) * 5, 20, 40);//Pants0
                                    graphics.FillRectangle(semiTransBrush, displayBox.Width / 2, legY + (float)double.Parse(this.offsetPants.Text) * 5, 20, 40);//Pants1
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 20, legY + 40 + (float)double.Parse(this.offsetBoots.Text) * 5, 20, 20);//Boot0
                                    graphics.FillRectangle(semiTransBrush, displayBox.Width / 2, legY + 40 + (float)double.Parse(this.offsetBoots.Text) * 5, 20, 20);//Boot1
                                }

                                //Highlights selected item shape in preview window
                                if (listViewItem.Index == selected.Index)
                                {
                                    graphics.DrawRectangle(Pens.Yellow, x + (float)double.Parse(this.selected.SubItems[1].Text) * 5 - 1, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5 - 1, (float)double.Parse(this.selected.SubItems[4].Text) * 5 + 2, (float)double.Parse(this.selected.SubItems[5].Text) * 5 + 2);
                                    graphics.DrawRectangle(Pens.Black, x + (float)double.Parse(this.selected.SubItems[1].Text) * 5, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5, (float)double.Parse(this.selected.SubItems[4].Text) * 5, (float)double.Parse(this.selected.SubItems[5].Text) * 5);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    else if (this.direction == "left")
                    {
                        //Generates Guidelines if enabled
                        if (this.checkGuide.Checked)
                        {
                            try
                            {
                                graphics.DrawLine(Pens.Red, displayBox.Width / 2, 0, displayBox.Width / 2, displayBox.Height);
                                graphics.DrawLine(Pens.Red, 0, headbodyY + (float)double.Parse(this.offsetHead.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetHead.Text) * 5);
                                graphics.DrawLine(Pens.Green, 0, headbodyY + (float)double.Parse(this.offsetBody.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetBody.Text) * 5);
                                graphics.DrawLine(Pens.Blue, 0, headbodyY + (float)double.Parse(this.offsetArms.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetArms.Text) * 5);
                                graphics.DrawLine(Pens.Purple, 0, legY + (float)double.Parse(this.offsetLegs.Text) * 5, displayBox.Width, legY + (float)double.Parse(this.offsetLegs.Text) * 5);
                            }
                            catch (Exception)
                            {

                            }
                        }
                        foreach (ListViewItem listViewItem in this.listViewBoxes.Items)
                        {
                            int x = 0;
                            int y = 0;
                            try
                            {
                                //Sets X & Y based on model part class
                                if (listViewItem.Tag.ToString() == "HEAD")
                                {
                                    x = displayBox.Width / 2;
                                    y = headbodyY + int.Parse(this.offsetHead.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "BODY")
                                {
                                    x = displayBox.Width / 2;
                                    y = headbodyY + int.Parse(this.offsetBody.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "ARM0")
                                {
                                    x = displayBox.Width / 2;
                                    y = armY + int.Parse(this.offsetArms.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "ARM1")
                                {
                                    x = displayBox.Width / 2;
                                    y = armY + int.Parse(this.offsetArms.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "LEG0")
                                {
                                    x = displayBox.Width / 2;
                                    y = legY + int.Parse(this.offsetLegs.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "LEG1")
                                {
                                    x = displayBox.Width / 2;
                                    y = legY + int.Parse(this.offsetLegs.Text) * 5;
                                }
                                //Maps imported Texture if auto texture is disabled
                                if (autoTexture != true)
                                {
                                    RectangleF destRect = new RectangleF(
                                        (float)(x + double.Parse(listViewItem.SubItems[3].Text) * 5),
                                        (float)(y + double.Parse(listViewItem.SubItems[2].Text) * 5),
                                        (float)(double.Parse(listViewItem.SubItems[6].Text) * 5),
                                        (float)(double.Parse(listViewItem.SubItems[5].Text) * 5));
                                    RectangleF srcRect = new RectangleF((float)double.Parse(listViewItem.SubItems[7].Text) + (float)double.Parse(listViewItem.SubItems[6].Text) + (float)double.Parse(listViewItem.SubItems[4].Text),
                                        (float)(double.Parse(listViewItem.SubItems[8].Text) + double.Parse(listViewItem.SubItems[6].Text)),
                                        (float)double.Parse(listViewItem.SubItems[6].Text),
                                        (float)double.Parse(listViewItem.SubItems[5].Text));
                                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                    graphics.DrawImage(texturePreview.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    //Draws Part
                                    graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), x + (float)double.Parse(listViewItem.SubItems[3].Text) * 5, y + (float)double.Parse(listViewItem.SubItems[2].Text) * 5, (float)double.Parse(listViewItem.SubItems[6].Text) * 5, (float)double.Parse(listViewItem.SubItems[5].Text) * 5);
                                }

                                //Draws Armor Offsets
                                if (checkBoxArmor.Checked == true)
                                {
                                    SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(80, 50, 50, 75));
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 20, headbodyY - 40 + (float)double.Parse(this.offsetHelmet.Text) * 5, 40, 40);//Helmet
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 5, armY + 45 + (float)double.Parse(this.offsetTool.Text) * 5, 10, 10);//Tools
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 10, legY + (float)double.Parse(this.offsetPants.Text) * 5, 20, 40);//Pants
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 10, legY + 40 + (float)double.Parse(this.offsetBoots.Text) * 5, 20, 20);//Boots
                                }

                                //Highlights selected item shape in preview window
                                if (listViewItem.Index == this.selected.Index)
                                {
                                    graphics.DrawRectangle(Pens.Yellow, x + (float)double.Parse(this.selected.SubItems[3].Text) * 5 - 1, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5 - 1, (float)double.Parse(this.selected.SubItems[6].Text) * 5 + 2, (float)double.Parse(this.selected.SubItems[5].Text) * 5 + 2);
                                    graphics.DrawRectangle(Pens.Black, x + (float)double.Parse(this.selected.SubItems[3].Text) * 5, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5, (float)double.Parse(this.selected.SubItems[6].Text) * 5, (float)double.Parse(this.selected.SubItems[5].Text) * 5);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        bitmapModelPreview.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (this.direction == "back")
                    {
                        //Generates Guidelines if enabled
                        if (this.checkGuide.Checked)
                        {
                            try
                            {
                                graphics.DrawLine(Pens.Red, displayBox.Width / 2, 0, displayBox.Width / 2, displayBox.Height);
                                graphics.DrawLine(Pens.Blue, (displayBox.Width / 2) + 30, 0, (displayBox.Width / 2) + 30, displayBox.Height);
                                graphics.DrawLine(Pens.Blue, (displayBox.Width / 2) - 30, 0, (displayBox.Width / 2) - 30, displayBox.Height);
                                graphics.DrawLine(Pens.Purple, (displayBox.Width / 2) - 10, 0, (displayBox.Width / 2) - 10, displayBox.Height);
                                graphics.DrawLine(Pens.Purple, (displayBox.Width / 2) + 10, 0, (displayBox.Width / 2) + 10, displayBox.Height);
                                graphics.DrawLine(Pens.Red, 0, headbodyY + (float)double.Parse(this.offsetHead.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetHead.Text) * 5);
                                graphics.DrawLine(Pens.Green, 0, headbodyY + (float)double.Parse(this.offsetBody.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetBody.Text) * 5);
                                graphics.DrawLine(Pens.Blue, 0, headbodyY + (float)double.Parse(this.offsetArms.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetArms.Text) * 5);
                                graphics.DrawLine(Pens.Purple, 0, legY + (float)double.Parse(this.offsetLegs.Text) * 5, displayBox.Width, legY + (float)double.Parse(this.offsetLegs.Text) * 5);
                            }
                            catch (Exception)
                            {

                            }
                        }
                        foreach (ListViewItem listViewItem in this.listViewBoxes.Items)
                        {
                            int x = 0;
                            int y = 0;
                            try
                            {
                                //Sets X & Y based on model part class
                                if (listViewItem.Tag.ToString() == "HEAD")
                                {
                                    x = displayBox.Width / 2;
                                    y = headbodyY + int.Parse(this.offsetHead.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "BODY")
                                {
                                    x = displayBox.Width / 2;
                                    y = headbodyY + int.Parse(this.offsetBody.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "ARM0")
                                {
                                    x = (displayBox.Width / 2) - 25;
                                    y = armY + int.Parse(this.offsetArms.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "ARM1")
                                {
                                    x = (displayBox.Width / 2) + 25;
                                    y = armY + int.Parse(this.offsetArms.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "LEG0")
                                {
                                    x = (displayBox.Width / 2) - 10;
                                    y = legY + int.Parse(this.offsetLegs.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "LEG1")
                                {
                                    x = (displayBox.Width / 2) + 10;
                                    y = legY + int.Parse(this.offsetLegs.Text) * 5;
                                }

                                //Maps imported Texture if auto texture is disabled
                                if (autoTexture != true)
                                {
                                    RectangleF destRect = new RectangleF(
                                        (float)(x + (float)double.Parse(listViewItem.SubItems[1].Text) * 5),
                                        (float)(y + (float)double.Parse(listViewItem.SubItems[2].Text) * 5),
                                        (float)((float)double.Parse(listViewItem.SubItems[4].Text) * 5),
                                        (float)((float)double.Parse(listViewItem.SubItems[5].Text) * 5));
                                    RectangleF srcRect = new RectangleF(
                                        (float)((float)double.Parse(listViewItem.SubItems[7].Text) +
                                                 (float)double.Parse(listViewItem.SubItems[6].Text) +
                                                 (float)double.Parse(listViewItem.SubItems[4].Text) +
                                                 (float)double.Parse(listViewItem.SubItems[6].Text)),
                                        (float)((float)double.Parse(listViewItem.SubItems[8].Text) +
                                                 (float)double.Parse(listViewItem.SubItems[6].Text)),
                                        (float)(float)double.Parse(listViewItem.SubItems[4].Text),
                                        (float)(float)double.Parse(listViewItem.SubItems[5].Text));
                                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                    graphics.DrawImage(texturePreview.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    //Draws Part
                                    graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), x + (float)double.Parse(listViewItem.SubItems[1].Text) * 5, y + (float)double.Parse(listViewItem.SubItems[2].Text) * 5, (float)double.Parse(listViewItem.SubItems[4].Text) * 5, (float)double.Parse(listViewItem.SubItems[5].Text) * 5);
                                }

                                //Draws Armor Offsets
                                if (checkBoxArmor.Checked == true)
                                {
                                    SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(80, 50, 50, 75));
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 20, headbodyY - 40 + (float)double.Parse(this.offsetHelmet.Text) * 5, 40, 40);//Helmet
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 35, armY + 45 + (float)double.Parse(this.offsetTool.Text) * 5, 10, 10);//Tool0
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) + 25, armY + 45 + (float)double.Parse(this.offsetTool.Text) * 5, 10, 10);//Tool1
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 20, legY + (float)double.Parse(this.offsetPants.Text) * 5, 20, 40);//Pants0
                                    graphics.FillRectangle(semiTransBrush, displayBox.Width / 2, legY + (float)double.Parse(this.offsetPants.Text) * 5, 20, 40);//Pants1
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 20, legY + 40 + (float)double.Parse(this.offsetBoots.Text) * 5, 20, 20);//Boot0
                                    graphics.FillRectangle(semiTransBrush, displayBox.Width / 2, legY + 40 + (float)double.Parse(this.offsetBoots.Text) * 5, 20, 20);//Boot1
                                }

                                //Highlights selected item shape in preview window
                                if (listViewItem.Index == this.selected.Index)
                                {
                                    graphics.DrawRectangle(Pens.Yellow, x + (float)double.Parse(this.selected.SubItems[1].Text) * 5 - 1, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5 - 1, (float)double.Parse(this.selected.SubItems[4].Text) * 5 + 2, (float)double.Parse(this.selected.SubItems[5].Text) * 5 + 2);
                                    graphics.DrawRectangle(Pens.Black, x + (float)double.Parse(this.selected.SubItems[1].Text) * 5, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5, (float)double.Parse(this.selected.SubItems[4].Text) * 5, (float)double.Parse(this.selected.SubItems[5].Text) * 5);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        bitmapModelPreview.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (this.direction == "right")
                    {
                        //Generates Guidelines if enabled
                        if (this.checkGuide.Checked)
                        {

                            try
                            {
                                graphics.DrawLine(Pens.Red, displayBox.Width / 2, 0, displayBox.Width / 2, displayBox.Height);
                                graphics.DrawLine(Pens.Red, 0, headbodyY + (float)double.Parse(this.offsetHead.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetHead.Text) * 5);
                                graphics.DrawLine(Pens.Green, 0, headbodyY + (float)double.Parse(this.offsetBody.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetBody.Text) * 5);
                                graphics.DrawLine(Pens.Blue, 0, headbodyY + (float)double.Parse(this.offsetArms.Text) * 5, displayBox.Width, headbodyY + (float)double.Parse(this.offsetArms.Text) * 5);
                                graphics.DrawLine(Pens.Purple, 0, legY + (float)double.Parse(this.offsetLegs.Text) * 5, displayBox.Width, legY + (float)double.Parse(this.offsetLegs.Text) * 5);
                            }
                            catch (Exception)
                            {

                            }
                        }
                        foreach (ListViewItem listViewItem in this.listViewBoxes.Items)
                        {
                            int x = 0;
                            int y = 0;
                            try
                            {
                                //Sets X & Y based on model part class
                                if (listViewItem.Tag.ToString() == "HEAD")
                                {
                                    x = displayBox.Width / 2;
                                    y = headbodyY + int.Parse(this.offsetHead.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "BODY")
                                {
                                    x = displayBox.Width / 2;
                                    y = headbodyY + int.Parse(this.offsetBody.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "ARM0")
                                {
                                    x = displayBox.Width / 2;
                                    y = armY + int.Parse(this.offsetArms.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "ARM1")
                                {
                                    x = displayBox.Width / 2;
                                    y = armY + int.Parse(this.offsetArms.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "LEG0")
                                {
                                    x = displayBox.Width / 2;
                                    y = legY + int.Parse(this.offsetLegs.Text) * 5;
                                }
                                else if (listViewItem.Tag.ToString() == "LEG1")
                                {
                                    x = displayBox.Width / 2;
                                    y = legY + int.Parse(this.offsetLegs.Text) * 5;
                                }
                                //Maps imported Texture if auto texture is disabled
                                if (autoTexture != true)
                                {
                                    RectangleF destRect = new RectangleF(
                                        (float)(x + double.Parse(listViewItem.SubItems[3].Text) * 5),
                                        (float)(y + double.Parse(listViewItem.SubItems[2].Text) * 5),
                                        (float)(double.Parse(listViewItem.SubItems[6].Text) * 5),
                                        (float)(double.Parse(listViewItem.SubItems[5].Text) * 5));
                                    RectangleF srcRect = new RectangleF(
                                        (float)(double.Parse(listViewItem.SubItems[7].Text) +
                                                 double.Parse(listViewItem.SubItems[6].Text) +
                                                 double.Parse(listViewItem.SubItems[4].Text)),
                                        (float)(double.Parse(listViewItem.SubItems[8].Text) +
                                                 double.Parse(listViewItem.SubItems[6].Text)),
                                        (float)double.Parse(listViewItem.SubItems[6].Text),
                                        (float)double.Parse(listViewItem.SubItems[5].Text));
                                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                    graphics.DrawImage(texturePreview.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    //Draws Part
                                    graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), x + (float)double.Parse(listViewItem.SubItems[3].Text) * 5, y + (float)double.Parse(listViewItem.SubItems[2].Text) * 5, (float)double.Parse(listViewItem.SubItems[6].Text) * 5, (float)double.Parse(listViewItem.SubItems[5].Text) * 5);
                                }

                                //Draws Armor Offsets
                                if (checkBoxArmor.Checked == true)
                                {
                                    SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(80, 50, 50, 75));
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 20, headbodyY - 40 + (float)double.Parse(this.offsetHelmet.Text) * 5, 40, 40);//Helmet
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 5, armY + 45 + (float)double.Parse(this.offsetTool.Text) * 5, 10, 10);//Tools
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 10, legY + (float)double.Parse(this.offsetPants.Text) * 5, 20, 40);//Pants
                                    graphics.FillRectangle(semiTransBrush, (displayBox.Width / 2) - 10, legY + 40 + (float)double.Parse(this.offsetBoots.Text) * 5, 20, 20);//Boots
                                }

                                //Highlights selected item shape in preview window
                                if (listViewItem.Index == this.selected.Index)
                                {
                                    graphics.DrawRectangle(Pens.Yellow, x + (float)double.Parse(this.selected.SubItems[3].Text) * 5 - 1, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5 - 1, (float)double.Parse(this.selected.SubItems[6].Text) * 5 + 2, (float)double.Parse(this.selected.SubItems[5].Text) * 5 + 2);
                                    graphics.DrawRectangle(Pens.Black, x + (float)double.Parse(this.selected.SubItems[3].Text) * 5, y + (float)double.Parse(this.selected.SubItems[2].Text) * 5, (float)double.Parse(this.selected.SubItems[6].Text) * 5, (float)double.Parse(this.selected.SubItems[5].Text) * 5);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    graphics.Dispose();
                }
                displayBox.Image = (Image)bitmapModelPreview;//Sets created preview graphics to display box
            }
            catch (Exception)
            {
                return;
            }
            //Auto Generates Texture if enabled
            if (autoTexture == true)
            {
                Bitmap bitmapAutoTexture = new Bitmap(texturePreview.Width, texturePreview.Height);
                using (Graphics graphics = Graphics.FromImage((Image)bitmapAutoTexture))
                {
                    foreach (ListViewItem listViewItem in this.listViewBoxes.Items)
                    {
                        try
                        {
                            double.Parse(listViewItem.SubItems[1].Text);
                            double.Parse(listViewItem.SubItems[2].Text);
                            double.Parse(listViewItem.SubItems[3].Text);
                            double width = (float)double.Parse(listViewItem.SubItems[4].Text) * 2;
                            double height = (float)double.Parse(listViewItem.SubItems[5].Text) * 2;
                            double num = (float)double.Parse(listViewItem.SubItems[6].Text) * 2;
                            double x = (float)double.Parse(listViewItem.SubItems[7].Text) * 2;
                            double y = (float)double.Parse(listViewItem.SubItems[8].Text) * 2;
                            graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), (float)(x + num), (float)y, (float)(width), (float)(num));
                            graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), (float)(x + num + width), (float)y, (float)width, (float)num);
                            graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), (float)(x), (float)(y) + (float)(num), (float)(num), (float)(height));
                            graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), (float)(x) + (float)(num), (float)(y) + (float)(num), (float)(width), (float)(height));
                            if (listViewItem.Tag.ToString() != "HEAD")
                            {
                                graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), (float)(x) + (float)(num) + (float)(width), (float)(y) + (float)(num), (float)(width), (float)(height));
                                graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), (float)(x) + (float)(num) + (float)(width) + (float)(width), (float)(y) + (float)(num), (float)(num), (float)(height));
                            }
                            else
                            {
                                graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), (float)(x) + (float)(num) + (float)(width) + (float)(width), (float)(y) + (float)(num), (float)(num), (float)(height));
                                graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), (float)(x) + (float)(num) + (float)(width), (float)(y) + (float)(num), (float)(width), (float)(height));
                            }
                        }
                        catch
                        {
                        }
                    }
                    graphics.Dispose();
                }
                texturePreview.Image = (Image)bitmapAutoTexture;
            }
            checkSelect(); //checks to see if an item is selected
            //Checks to make sure each item has been properly set up
            foreach (ListViewItem listViewItem in this.listViewBoxes.Items)
            {
                try
                {
                    if (listViewItem.Tag == null)
                        this.buttonDone.Enabled = false;
                    else
                        this.buttonDone.Enabled = true;
                }
                catch (Exception ex)
                {

                }
            }
        }


        //Checks and sets Z layering
        private void setZ()
        {
            try
            {
                foreach (ListViewItem listViewItem in this.listViewBoxes.Items)
                    listViewItem.SubItems.Add("unchecked");

                if (listViewBoxes.SelectedItems.Count != 0)
                {
                    selected = listViewBoxes.SelectedItems[0];
                }
                else
                {
                    selected = null;
                }

                if (this.direction == "front")
                {
                    int checkedItems = 0;
                    do
                    {
                        foreach (ListViewItem listViewItemCurrent in this.listViewBoxes.Items)
                        {
                            if (listViewItemCurrent.SubItems[9].Text == "unchecked")
                            {
                                int x = 0;
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
                                foreach (ListViewItem listViewItemComparing in this.listViewBoxes.Items)
                                {
                                    if (listViewItemComparing.SubItems[9].Text == "unchecked" && (int)double.Parse(listViewItemCurrent.SubItems[3].Text) + (int)double.Parse(listViewItemCurrent.SubItems[6].Text) < (int)double.Parse(listViewItemComparing.SubItems[3].Text) + (int)double.Parse(listViewItemComparing.SubItems[6].Text))
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
                                    this.listViewBoxes.Items.Insert(index, listViewItem2);
                                    if (listViewBoxes.SelectedItems.Count != 0)
                                    {
                                        if (selected.Index == listViewItemCurrent.Index)
                                        {
                                            selected = listViewItem2;
                                        }
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
                else if (this.direction == "left")
                {
                    int checkedItems = 0;
                    do
                    {
                        foreach (ListViewItem listViewItem1 in this.listViewBoxes.Items)
                        {
                            if (listViewItem1.SubItems[listViewItem1.SubItems.Count - 1].Text == "unchecked")
                            {
                                int x = 0;
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
                                foreach (ListViewItem listViewItem2 in this.listViewBoxes.Items)
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
                                    this.listViewBoxes.Items.Insert(index, listViewItem2);
                                    if (listViewBoxes.SelectedItems.Count != 0)
                                    {
                                        if (selected.Index == listViewItem1.Index)
                                        {
                                            selected = listViewItem2;
                                        }
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
                else if (this.direction == "back")
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
                                    this.listViewBoxes.Items.Insert(index, listViewItem2);
                                    if (listViewBoxes.SelectedItems.Count != 0)
                                    {
                                        if (selected.Index == listViewItemCurrent.Index)
                                        {
                                            selected = listViewItem2;
                                        }
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
                else if (this.direction == "right")
                {
                    int checkedItems = 0;
                    do
                    {
                        foreach (ListViewItem listViewItem1 in this.listViewBoxes.Items)
                        {
                            if (listViewItem1.SubItems[listViewItem1.SubItems.Count - 1].Text == "unchecked")
                            {
                                int x = 0;
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
                                foreach (ListViewItem listViewItem2 in this.listViewBoxes.Items)
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
                                    this.listViewBoxes.Items.Insert(index, listViewItem2);
                                    if (listViewBoxes.SelectedItems.Count != 0)
                                    {
                                        if (selected.Index == listViewItem1.Index)
                                        {
                                            selected = listViewItem2;
                                        }
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
                foreach (ListViewItem listViewItem in this.listViewBoxes.Items)
                    listViewItem.SubItems[9].Text = "unchecked";
            }
            catch (Exception ex)
            {

            }
        }


        //Loads Columns
        private void generateModel_Load(object sender, EventArgs e)
        {
            this.render();
            this.listViewBoxes.Columns.Add("Part", 50);
            this.listViewBoxes.Columns.Add("Xc", 25);
            this.listViewBoxes.Columns.Add("Yc", 25);
            this.listViewBoxes.Columns.Add("Zc", 25);
            this.listViewBoxes.Columns.Add("Xf", 25);
            this.listViewBoxes.Columns.Add("Yf", 25);
            this.listViewBoxes.Columns.Add("Zf", 25);
            this.listViewBoxes.Columns.Add("Xo", 25);
            this.listViewBoxes.Columns.Add("Yo", 25);
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height < 780 || System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width < 1080)
            {
                this.Size = new Size(753, 707);
            }
        }


        //Creates Item
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem part = new ListViewItem();
            part.Text = "New Part";
            part.SubItems.Add(new ListViewItem.ListViewSubItem(part, "0"));
            part.SubItems.Add(new ListViewItem.ListViewSubItem(part, "0"));
            part.SubItems.Add(new ListViewItem.ListViewSubItem(part, "0"));
            part.SubItems.Add(new ListViewItem.ListViewSubItem(part, "0"));
            part.SubItems.Add(new ListViewItem.ListViewSubItem(part, "0"));
            part.SubItems.Add(new ListViewItem.ListViewSubItem(part, "0"));
            part.SubItems.Add(new ListViewItem.ListViewSubItem(part, "0"));
            part.SubItems.Add(new ListViewItem.ListViewSubItem(part, "0"));
            listViewBoxes.Items.Add(part);
            render();
        }


        //Manages the selection of a item
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewBoxes.SelectedItems.Count != 0)
            {
                selected = listViewBoxes.SelectedItems[0];
                try
                {
                    try
                    {
                        this.comboParent.Text = this.selected.Tag.ToString();
                    }
                    catch (Exception ex)
                    {
                        this.comboParent.Text = "";
                    }
                    this.textXc.Text = this.selected.SubItems[1].Text;
                    this.textYc.Text = this.selected.SubItems[2].Text;
                    this.textZc.Text = this.selected.SubItems[3].Text;
                    this.textXf.Text = this.selected.SubItems[4].Text;
                    this.textYf.Text = this.selected.SubItems[5].Text;
                    this.textZf.Text = this.selected.SubItems[6].Text;
                    this.textTextureX.Text = this.selected.SubItems[7].Text;
                    this.textTextureY.Text = this.selected.SubItems[8].Text;
                }
                catch (Exception ex)
                {
                }
                if (this.comboParent.Text == "")
                {
                    this.comboParent.Enabled = true;
                    this.buttonIMPORT.Enabled = false;
                    this.buttonEXPORT.Enabled = false;
                    this.textXc.Enabled = false;
                    this.textYc.Enabled = false;
                    this.textZc.Enabled = false;
                    this.textXf.Enabled = false;
                    this.textYf.Enabled = false;
                    this.textZf.Enabled = false;
                    this.textTextureX.Enabled = false;
                    this.textTextureY.Enabled = false;
                    this.buttonXcminus.Enabled = false;
                    this.buttonYcminus.Enabled = false;
                    this.buttonZcminus.Enabled = false;
                    this.buttonXcplus.Enabled = false;
                    this.buttonYcplus.Enabled = false;
                    this.buttonZcplus.Enabled = false;
                    this.buttonXfminus.Enabled = false;
                    this.buttonYfminus.Enabled = false;
                    this.buttonZfminus.Enabled = false;
                    this.buttonXfplus.Enabled = false;
                    this.buttonYfplus.Enabled = false;
                    this.buttonZfplus.Enabled = false;
                }
                else
                {
                    this.buttonIMPORT.Enabled = true;
                    this.buttonEXPORT.Enabled = true;
                    this.textXc.Enabled = true;
                    this.textYc.Enabled = true;
                    this.textZc.Enabled = true;
                    this.textXf.Enabled = true;
                    this.textYf.Enabled = true;
                    this.textZf.Enabled = true;
                    this.textTextureX.Enabled = true;
                    this.textTextureY.Enabled = true;
                    this.buttonXcminus.Enabled = true;
                    this.buttonYcminus.Enabled = true;
                    this.buttonZcminus.Enabled = true;
                    this.buttonXcplus.Enabled = true;
                    this.buttonYcplus.Enabled = true;
                    this.buttonZcplus.Enabled = true;
                    this.buttonXfminus.Enabled = true;
                    this.buttonYfminus.Enabled = true;
                    this.buttonZfminus.Enabled = true;
                    this.buttonXfplus.Enabled = true;
                    this.buttonYfplus.Enabled = true;
                    this.buttonZfplus.Enabled = true;
                }
                this.render();
            }
            else
            {
            }
        }


        //Changes Item Model Class
        private void comboParent_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.selected.Tag = (object)this.comboParent.Text;
                if (this.comboParent.Text != "")
                {
                    this.buttonIMPORT.Enabled = true;
                    this.buttonEXPORT.Enabled = true;
                    this.textXc.Enabled = true;
                    this.textYc.Enabled = true;
                    this.textZc.Enabled = true;
                    this.textXf.Enabled = true;
                    this.textYf.Enabled = true;
                    this.textZf.Enabled = true;
                    this.textTextureX.Enabled = true;
                    this.textTextureY.Enabled = true;
                    this.buttonXcminus.Enabled = true;
                    this.buttonYcminus.Enabled = true;
                    this.buttonZcminus.Enabled = true;
                    this.buttonXcplus.Enabled = true;
                    this.buttonYcplus.Enabled = true;
                    this.buttonZcplus.Enabled = true;
                    this.buttonXfminus.Enabled = true;
                    this.buttonYfminus.Enabled = true;
                    this.buttonZfminus.Enabled = true;
                    this.buttonXfplus.Enabled = true;
                    this.buttonYfplus.Enabled = true;
                    this.buttonZfplus.Enabled = true;
                }
            }
            catch (Exception ex)
            {

            }
            render();
        }

        private void textBoxFailCheck(TextBox textBox)
        {
            try
            {
                textBox.Text = double.Parse(textBox.Text).ToString();
            }
            catch (Exception ex)
            {
                textBox.Text = "0";
            }
        }

        //X-Size Change
        private void textXf_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.selected.SubItems[4].Text = double.Parse(this.textXf.Text).ToString();
            }
            catch (Exception ex)
            {

            }
            render();
        }


        //Y-Size Change
        private void textYf_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.selected.SubItems[5].Text = double.Parse(this.textYf.Text).ToString();
            }
            catch (Exception ex)
            {

            }
            render();
        }


        //Z-Size Change
        private void textZf_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.selected.SubItems[6].Text = double.Parse(this.textZf.Text).ToString();
            }
            catch (Exception ex)
            {

            }
            render();
        }


        //X-Position Change
        private void textXc_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.selected.SubItems[1].Text = double.Parse(this.textXc.Text).ToString();
            }
            catch (Exception ex)
            {

            }
            render();
        }


        //Y-Position Change
        private void textYc_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.selected.SubItems[2].Text = double.Parse(this.textYc.Text).ToString();
            }
            catch (Exception ex)
            {

            }
            render();
        }


        //Z-Position Change
        private void textZc_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.selected.SubItems[3].Text = double.Parse(this.textZc.Text).ToString();
            }
            catch (Exception ex)
            {

            }
            render();
        }


        //Increases X-Size
        private void buttonXfplus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textXf.Text = ((int)double.Parse(this.textXf.Text) + 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Decreases X-Size
        private void buttonXfminus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textXf.Text = ((int)double.Parse(this.textXf.Text) - 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Increases Y-Size
        private void buttonYfplus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textYf.Text = ((int)double.Parse(this.textYf.Text) + 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Decreases Y-Size
        private void buttonYfminus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textYf.Text = ((int)double.Parse(this.textYf.Text) - 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Increases Z-Size
        private void buttonZfplus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textZf.Text = ((int)double.Parse(this.textZf.Text) + 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Decreases Z-Size
        private void buttonZfminus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textZf.Text = ((int)double.Parse(this.textZf.Text) - 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Increases X-Position
        private void buttonXcplus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textXc.Text = ((int)double.Parse(this.textXc.Text) + 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Decreases X-Position
        private void buttonXcminus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textXc.Text = ((int)double.Parse(this.textXc.Text) - 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Increases Y-Position
        private void buttonYcplus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textYc.Text = ((int)double.Parse(this.textYc.Text) + 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Decreases Y-Position
        private void buttonYcminus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textYc.Text = ((int)double.Parse(this.textYc.Text) - 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Increases Z-Position
        private void buttonZcplus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textZc.Text = ((int)double.Parse(this.textZc.Text) + 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Decreases Z-Position
        private void buttonZcminus_Click(object sender, EventArgs e)
        {
            try
            {
                this.textZc.Text = ((int)double.Parse(this.textZc.Text) - 1).ToString();
            }
            catch (Exception ex)
            {
            }
            render();
        }


        //Rotates View Right
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.direction == "front")
                this.direction = "left";
            else if (this.direction == "left")
                this.direction = "back";
            else if (this.direction == "back")
                this.direction = "right";
            else if (this.direction == "right")
                this.direction = "front";
            render();
        }


        //Rotates View Left
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.direction == "front")
                this.direction = "right";
            else if (this.direction == "right")
                this.direction = "back";
            else if (this.direction == "back")
                this.direction = "left";
            else if (this.direction == "left")
                this.direction = "front";
            render();
        }


        //Sets Texture X-Offset
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double.Parse(this.textTextureX.Text);
            }
            catch (Exception ex1)
            {
                try
                {
                    this.textTextureX.Text = this.textTextureX.Text.Remove(this.textTextureX.Text.Count<char>() - 1, 1);
                }
                catch (Exception ex2)
                {
                }
            }
            try
            {
                this.selected.SubItems[7].Text = double.Parse(this.textTextureX.Text).ToString();
            }
            catch (Exception ex)
            {
                this.selected.SubItems[7].Text = 0.ToString();
            }
            render();
        }


        //Sets texture Y-Offset
        private void textTextureY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double.Parse(this.textTextureY.Text);
            }
            catch (Exception ex1)
            {
                try
                {
                    this.textTextureY.Text = this.textTextureY.Text.Remove(this.textTextureY.Text.Count<char>() - 1, 1);
                }
                catch (Exception ex2)
                {
                }
            }
            try
            {
                this.selected.SubItems[8].Text = double.Parse(this.textTextureY.Text).ToString();
            }
            catch (Exception ex)
            {
                this.selected.SubItems[8].Text = 0.ToString();
            }
            render();
        }


        //Export Current Skin Texture
        private void buttonEXPORT_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(this.texturePreview.Image, 64, 64);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image Files | *.png";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
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
                using (Graphics graphics = Graphics.FromImage((Image)bitmap))
                {
                    graphics.DrawImage(Image.FromFile(openFileDialog.FileName), 0, 0, 64, 64);
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                }
                this.texturePreview.Image = (Image)bitmap;
            }
            render();
        }


        //Creates Model Data and Finalizes
        private void buttonDone_Click(object sender, EventArgs e)
        {
            boxes.Clear();

            Bitmap bitmap1 = new Bitmap(this.displayBox.Width, this.displayBox.Height);
            foreach (ListViewItem listViewItem in listViewBoxes.Items)
            {
                boxes.Add(new object[2] { "BOX", listViewItem.Tag.ToString() + " " + listViewItem.SubItems[1].Text + " " + listViewItem.SubItems[2].Text + " " + listViewItem.SubItems[3].Text + " " + listViewItem.SubItems[4].Text + " " + listViewItem.SubItems[5].Text + " " + listViewItem.SubItems[6].Text + " " + listViewItem.SubItems[7].Text + " " + listViewItem.SubItems[8].Text });

                //mf.entries.Add(new object[2] { (object) "BOX", new ListViewItem() { Tag = ((object) (listViewItem.Tag.ToString() + " " + listViewItem.SubItems[1].Text + " " + listViewItem.SubItems[2].Text + " " + listViewItem.SubItems[3].Text + " " + listViewItem.SubItems[4].Text + " " + listViewItem.SubItems[5].Text + " " + listViewItem.SubItems[6].Text + " " + listViewItem.SubItems[7].Text + " " + listViewItem.SubItems[8].Text)) }.Tag });
                using (Graphics graphics = Graphics.FromImage((Image)bitmap1))
                {
                    int x = 0;
                    int y = 0;
                    try
                    {
                        if (listViewItem.Tag.ToString() == "HEAD")
                        {
                            x = 80;
                            y = 16 + (int)double.Parse(this.offsetHead.Text) * 5 + 40;
                        }
                        else if (listViewItem.Tag.ToString() == "BODY")
                        {
                            x = 80;
                            y = 56 + (int)double.Parse(this.offsetBody.Text) * 5;
                        }
                        else if (listViewItem.Tag.ToString() == "ARM0")
                        {
                            x = 55;
                            y = 56 + (int)double.Parse(this.offsetArms.Text) * 5 + 10;
                        }
                        else if (listViewItem.Tag.ToString() == "ARM1")
                        {
                            x = 105;
                            y = 55 + (int)double.Parse(this.offsetArms.Text) * 5 + 10;
                        }
                        else if (listViewItem.Tag.ToString() == "LEG0")
                        {
                            x = 70;
                            y = 116 + (int)double.Parse(this.offsetLegs.Text) * 5;
                        }
                        else if (listViewItem.Tag.ToString() == "LEG1")
                        {
                            x = 90;
                            y = 116 + (int)double.Parse(this.offsetLegs.Text) * 5;
                        }
                        graphics.FillRectangle((Brush)new SolidBrush(listViewItem.ForeColor), x + (int)double.Parse(listViewItem.SubItems[1].Text) * 5, y + (int)double.Parse(listViewItem.SubItems[2].Text) * 5, (int)double.Parse(listViewItem.SubItems[4].Text) * 5, (int)double.Parse(listViewItem.SubItems[5].Text) * 5);
                        listViewItem.Remove();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            //Body Offsets
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("HEAD Y " + this.offsetHead.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("BODY Y " + this.offsetBody.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("ARM0 Y " + this.offsetArms.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("ARM1 Y " + this.offsetArms.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("LEG0 Y " + this.offsetLegs.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("LEG1 Y " + this.offsetLegs.Text)) }.Tag });
            //Armor Offsets
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("HELMET Y " + this.offsetHelmet.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("TOOL0 Y " + this.offsetTool.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("TOOL1 Y " + this.offsetTool.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("PANTS0 Y " + this.offsetPants.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("PANTS1 Y " + this.offsetPants.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("BOOTS0 Y " + this.offsetBoots.Text)) }.Tag });
            boxes.Add(new object[2] { (object)"OFFSET", new ListViewItem() { Tag = ((object)("BOOTS1 Y " + this.offsetBoots.Text)) }.Tag });

            Bitmap bitmap2 = new Bitmap(64, 64);
            using (Graphics graphics = Graphics.FromImage((Image)bitmap2))
            {
                graphics.DrawImage(texturePreview.Image, 0, 0, 64, 64);
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
            texturePreview.Image = (Image)bitmap2;
            try
            {
                using (FileStream stream = new FileStream(Application.StartupPath + "\\temp.png", FileMode.Create, FileAccess.Write))
                {
                    bitmap2.Save(stream, ImageFormat.Png);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            skinPreview.Image = (Image)bitmap1;
            Close();
        }

        //Renders model after texture change
        private void texturePreview_BackgroundImageChanged(object sender, EventArgs e)
        {
            render();
        }


        //Deciphers wether to auto-generate model texture or not
        private void checkTextureGenerate_CheckedChanged(object sender, EventArgs e)
        {
            if (this.autoTexture)
                this.autoTexture = false;
            else
                this.autoTexture = true;
        }


        //Trigger Dialog to select model part/item color
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            int num = (int)colorDialog.ShowDialog();
            this.selected.ForeColor = colorDialog.Color;
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
            ListViewItem owner = new ListViewItem();
            owner.Text = "HEAD";
            owner.SubItems.Add(new ListViewItem.ListViewSubItem(owner, "-4"));
            owner.SubItems.Add(new ListViewItem.ListViewSubItem(owner, "-8"));
            owner.SubItems.Add(new ListViewItem.ListViewSubItem(owner, "-4"));
            owner.SubItems.Add(new ListViewItem.ListViewSubItem(owner, "8"));
            owner.SubItems.Add(new ListViewItem.ListViewSubItem(owner, "8"));
            owner.SubItems.Add(new ListViewItem.ListViewSubItem(owner, "8"));
            owner.SubItems.Add(new ListViewItem.ListViewSubItem(owner, "0"));
            owner.SubItems.Add(new ListViewItem.ListViewSubItem(owner, "0"));
            owner.Tag = (object)"HEAD";
            owner.ForeColor = Color.Yellow;
            this.listViewBoxes.Items.Add(owner);
            this.listViewBoxes.Items.Add(new ListViewItem()
            {
                Text = "BODY",
                SubItems = {
          new ListViewItem.ListViewSubItem(owner, "-4"),
          new ListViewItem.ListViewSubItem(owner, "0"),
          new ListViewItem.ListViewSubItem(owner, "-2"),
          new ListViewItem.ListViewSubItem(owner, "8"),
          new ListViewItem.ListViewSubItem(owner, "12"),
          new ListViewItem.ListViewSubItem(owner, "4"),
          new ListViewItem.ListViewSubItem(owner, "16"),
          new ListViewItem.ListViewSubItem(owner, "16")
        },
                Tag = (object)"BODY",
                ForeColor = Color.Violet
            });
            this.listViewBoxes.Items.Add(new ListViewItem()
            {
                Text = "ARM0",
                SubItems = {
          new ListViewItem.ListViewSubItem(owner, "-3"),
          new ListViewItem.ListViewSubItem(owner, "-2"),
          new ListViewItem.ListViewSubItem(owner, "-2"),
          new ListViewItem.ListViewSubItem(owner, "4"),
          new ListViewItem.ListViewSubItem(owner, "12"),
          new ListViewItem.ListViewSubItem(owner, "4"),
          new ListViewItem.ListViewSubItem(owner, "40"),
          new ListViewItem.ListViewSubItem(owner, "16")
        },
                Tag = (object)"ARM0",
                ForeColor = Color.SkyBlue
            });
            this.listViewBoxes.Items.Add(new ListViewItem()
            {
                Text = "ARM1",
                SubItems = {
          new ListViewItem.ListViewSubItem(owner, "-1"),
          new ListViewItem.ListViewSubItem(owner, "-2"),
          new ListViewItem.ListViewSubItem(owner, "-2"),
          new ListViewItem.ListViewSubItem(owner, "4"),
          new ListViewItem.ListViewSubItem(owner, "12"),
          new ListViewItem.ListViewSubItem(owner, "4"),
          new ListViewItem.ListViewSubItem(owner, "40"),
          new ListViewItem.ListViewSubItem(owner, "16")
        },
                Tag = (object)"ARM1",
                ForeColor = Color.SkyBlue
            });
            this.listViewBoxes.Items.Add(new ListViewItem()
            {
                Text = "LEG0",
                SubItems = {
          new ListViewItem.ListViewSubItem(owner, "-2"),
          new ListViewItem.ListViewSubItem(owner, "0"),
          new ListViewItem.ListViewSubItem(owner, "-2"),
          new ListViewItem.ListViewSubItem(owner, "4"),
          new ListViewItem.ListViewSubItem(owner, "12"),
          new ListViewItem.ListViewSubItem(owner, "4"),
          new ListViewItem.ListViewSubItem(owner, "0"),
          new ListViewItem.ListViewSubItem(owner, "16")
        },
                Tag = (object)"LEG0",
                ForeColor = Color.SpringGreen
            });
            this.listViewBoxes.Items.Add(new ListViewItem()
            {
                Text = "LEG1",
                SubItems = {
          new ListViewItem.ListViewSubItem(owner, "-2"),
          new ListViewItem.ListViewSubItem(owner, "0"),
          new ListViewItem.ListViewSubItem(owner, "-2"),
          new ListViewItem.ListViewSubItem(owner, "4"),
          new ListViewItem.ListViewSubItem(owner, "12"),
          new ListViewItem.ListViewSubItem(owner, "4"),
          new ListViewItem.ListViewSubItem(owner, "0"),
          new ListViewItem.ListViewSubItem(owner, "16")
        },
                Tag = (object)"LEG1",
                ForeColor = Color.SpringGreen
            });
            this.comboParent.Enabled = true;
            render();
        }


        //Exports model (int)doubleo reusable project file
        private void buttonExportModel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Custom Skin Model File | *.CSM";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string contents = "";
            foreach (ListViewItem listViewItem in this.listViewBoxes.Items)
            {
                string str = "";
                foreach (ListViewItem.ListViewSubItem subItem in listViewItem.SubItems)
                {
                    if (subItem.Text != "unchecked")
                        str = str + subItem.Text + Environment.NewLine;
                }
                contents = contents + (listViewItem.Text + Environment.NewLine + listViewItem.Tag) + Environment.NewLine + str;
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
                this.listViewBoxes.Items.Clear();
                string str1 = File.ReadAllText(openFileDialog.FileName);
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


        //debug
        private void button3_Click(object sender, EventArgs e)
        {
            this.setZ();
        }


        //render with guide settings
        private void checkGuide_CheckedChanged(object sender, EventArgs e)
        {
            render();
        }


        //Clones Item
        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = this.selected.Text;
                listViewItem.Tag = this.selected.Tag;
                int num = 0;
                foreach (ListViewItem.ListViewSubItem subItem in this.selected.SubItems)
                {
                    if (num > 0)
                        listViewItem.SubItems.Add(subItem.Text);
                    ++num;
                }
                this.listViewBoxes.Items.Add(listViewItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please Select a Part");
            }
        }


        //Deletes Item
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (selected == null)
                    return;
                this.selected.Remove();
                this.render();
            }
            catch (Exception ex)
            {
            }
        }

        //Changes item color
        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ColorDialog colorDialog = new ColorDialog();
                colorDialog.ShowDialog();
                this.selected.ForeColor = colorDialog.Color;
                this.render();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please Select a Part");
            }
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

        //Toggles armor position overylay view
        private void checkBoxArmor_Click(object sender, EventArgs e)
        {
            render();
        }

        //Item Selection
        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                selected = listViewBoxes.SelectedItems[0];
            }
            catch (Exception)
            {

            }
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
            if (e.KeyValue == (char)Keys.Delete)
            {
                this.selected.Remove();
                this.render();
            }
        }

        private void generateModel_ResizeBegin(object sender, EventArgs e)
        {
        }

        private void generateModel_ResizeEnd(object sender, EventArgs e)
        {
            //ResizeWidth(this.Width);
            //ResizeHeight(this.Height);
        }

        public void ResizeWidth(int newWidth, int newHeight)
        {
            this.Width = (int)((double)newHeight * (double)((double)839 / (double)750));
            this.Height = newHeight;

            int newDisplayHeight = newHeight - 170;
            displayBox.Width = (int)((double)newDisplayHeight * (double)((double)530 / (double)580));
            displayBox.Height = newDisplayHeight;
        }

        private void generateModel_SizeChanged(object sender, EventArgs e)
        {
            ResizeWidth(this.Width, this.Height);
            render();
        }

        private void listViewBGs_ItemActivate(object sender, EventArgs e)
        {

        }

        private void listViewBGs_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void listViewBGs_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundColor = listViewBGs.SelectedItems[0].BackColor;
                render();
            }
            catch (Exception)
            {

            }
        }

        private void textXc_Leave(object sender, EventArgs e)
        {
            textBoxFailCheck((TextBox)sender);
        }


        private void button3_Click_1(object sender, EventArgs e)
        {

            string contents = "";
            foreach (ListViewItem listViewItem in this.listViewBoxes.Items)
            {
                string str = "";
                foreach (ListViewItem.ListViewSubItem subItem in listViewItem.SubItems)
                {
                    if (subItem.Text != "unchecked")
                        str = str + subItem.Text + Environment.NewLine;
                }
                contents = contents + (listViewItem.Text + Environment.NewLine + listViewItem.Tag) + Environment.NewLine + str;
            }
            Console.WriteLine(contents);
        }

        private void OpenJSONButton_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Model File | *.JSON";
            openFileDialog.Title = "Select JSON Model File";
            if (MessageBox.Show("Import custom model project file? Your current work will be lost!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.listViewBoxes.Items.Clear();
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
            dynamic jsonDe = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(InputFilePath));
            string CSMData = "";
            foreach (JObjectGroup group in jsonDe.groups)
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
                    float UvX = 0;
                    float UvY = 0;

                    CSMData += name + "\n" + PARENT + "\n" + name + "\n" + PosX + "\n" + PosY + "\n" + PosZ + "\n" + SizeX + "\n" + SizeY + "\n" + SizeZ + "\n" + UvX + "\n" + UvY + "\n";
                }
            }
            return CSMData;
        }
    }

    class JObject
    {
        public string credit;
        public int[] texture_size;
        public JObjectElement[] elements;
        public JObjectGroup[] groups;
    }
    class JObjectElement
    {
        public string name;
        public float[] from;
        public float[] to;
    }
    class JObjectGroup
    {
        public string name;
        public float[] origin;
        public int[] children;
    }
}