using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using MySql.Data.MySqlClient;
using System.Net;
using PckStudio;
using PckStudio.Classes.FileTypes;

namespace PckStudio
{
    public partial class addnewskin : MetroFramework.Forms.MetroForm
    {
        PCKFile currentPCK;
        DataTable tbl;
        LOC currentLoc;
        PCKFile.FileData mf = null;
        PCKFile.FileData mfc = null;
        TreeView treeView1;
        string skinId = "";
        TreeNode skin = new TreeNode();
        TreeNode cape = new TreeNode();
        TreeNode skinName = new TreeNode();
        TreeNode displayNameId = new TreeNode();
        TreeNode themeName = new TreeNode();
        TreeNode themeNameId = new TreeNode();
        TreeNode anim = new TreeNode();
        TreeNode free = new TreeNode();
        TreeNode theme = new TreeNode();
        TreeNode capePath = new TreeNode();
        string skinType = "";
        string ofd;
        bool useCape = false;
        string capeID;
        string localID;
        string serverID;
        string skinid;
        List<object[]> generatedModel = new List<object[]>();

        public addnewskin(PCKFile currentPCKIn, TreeView treeView1In, string tempIDIn, LOC loc)
        {
            InitializeComponent();
            
            mf = new PCKFile.FileData(0);
            mfc = new PCKFile.FileData(0);
            currentLoc = loc;
            tbl = new DataTable();
            tbl.Columns.Add(new DataColumn("Language") { ReadOnly = true });
            tbl.Columns.Add("Display Name");

            currentPCK = currentPCKIn;
            treeView1 = treeView1In;

            localID = tempIDIn;

            textSkinID.Text = localID;

            FormBorderStyle = FormBorderStyle.None;

            buttonDone.Enabled = false;
        }

        private void checkImage()
        {
            //Checks image dimensions and sets things accordingly
            var img = Image.FromFile(ofd);
            if (img.Height == 64) //If skins is 64x64
            {
                MessageBox.Show("64x64 Skin Detected");
                pictureBoxTexture.Width = pictureBoxTexture.Height;
                if (skinType != "64x64" && skinType != "64x64HD")
                {
                    buttonSkin.Location = new Point(buttonSkin.Location.X - pictureBoxTexture.Width, buttonSkin.Location.Y);
                }
                comboBoxSkinType.Text = "Steve (64x64)";
                comboBoxSkinType.Enabled = true;
                if (comboBoxSkinType.Items.Count == 3)
                {
                    comboBoxSkinType.Items.RemoveAt(0);
                }
                skinType = "64x64";
            }
            else if (img.Height == 32)//If skins is 64x32
            {
                MessageBox.Show("64x32 Skin Detected");
                pictureBoxTexture.Width = pictureBoxTexture.Height * 2;
                if (skinType == "64x64")
                {
                    buttonSkin.Location = new Point(buttonSkin.Location.X + pictureBoxTexture.Width / 2, buttonSkin.Location.Y);
                }
                if (skinType == "64x64HD")
                {
                    buttonSkin.Location = new Point(buttonSkin.Location.X + pictureBoxTexture.Width / 2, buttonSkin.Location.Y);
                }
                comboBoxSkinType.Text = "Default (64x32)";
                comboBoxSkinType.Enabled = false;
                skinType = "64x32";
            }
            else if (img.Width == img.Height / 1)//If skins is 64x64 HD
            {
                MessageBox.Show("64x64 HD Skin Detected");
                pictureBoxTexture.Width = pictureBoxTexture.Height;
                if (skinType != "64x64" && skinType != "64x64HD")
                {
                    buttonSkin.Location = new Point(buttonSkin.Location.X - pictureBoxTexture.Width, buttonSkin.Location.Y);
                }
                comboBoxSkinType.Text = "Steve (64x64)";
                comboBoxSkinType.Enabled = true;
                if (comboBoxSkinType.Items.Count == 3)
                {
                    comboBoxSkinType.Items.RemoveAt(0);
                }
                skinType = "64x64";
            }
            else if (img.Width == img.Height / 2)//If skins is 64x32 HD
            {
                MessageBox.Show("64x32 HD Skin Detected");
                pictureBoxTexture.Width = pictureBoxTexture.Height * 2;
                if (skinType == "64x64")
                {
                    buttonSkin.Location = new Point(buttonSkin.Location.X + pictureBoxTexture.Width / 2, buttonSkin.Location.Y);
                }
                if (skinType == "64x64HD")
                {
                    buttonSkin.Location = new Point(buttonSkin.Location.X + pictureBoxTexture.Width / 2, buttonSkin.Location.Y);
                }
                comboBoxSkinType.Text = "Default (64x32)";
                comboBoxSkinType.Enabled = false;
                skinType = "64x32";
            }
            else //If dimensions don't fit any skin type //Invalid
            {
                MessageBox.Show("Not a Valid Skin File");
                skinType = "unusable";
                return;
            }
            
            pictureBoxTexture.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxTexture.InterpolationMode = InterpolationMode.NearestNeighbor;
            pictureBoxTexture.Image = Image.FromFile(ofd);

            buttonDone.Enabled = true;
            labelSelectTexture.Visible = false;

            mf.data = File.ReadAllBytes(ofd);
        }

        public class displayId
        {
            public string id;
            public string defaultName;
        }

        private void textSkinName_TextChanged(object sender, EventArgs e)
        {
            skinName.Text = "DISPLAYNAME";
            skinName.Tag = textSkinName.Text;
        }

        private void textSkinID_TextChanged(object sender, EventArgs e)
        {
            skinId = textSkinID.Text;

            displayNameId.Text = "DISPLAYNAMEID";
            displayNameId.Tag = "IDS_dlcskin" + textSkinID.Text + "_DISPLAYNAME";

            themeName.Text = "THEMENAME";
            themeName.Tag = "dlcskin" + textSkinID.Text;
        }

        private void radioSteveModel_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Skin Model Set to Steve Model");
        }

        private void radioAlexModel_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Skin Model Set to Alex Model");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Must be an 8 digit Number");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is the Skins Name You'll See In-Game");
        }

        private void addnewskin_Load(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Application.StartupPath + "\\temp.png"))
                {
                    File.Delete(Application.StartupPath + "\\temp.png");
                }
            }catch (Exception)
            {

            }
            if (skinType == "unusable")
            {
                this.Close();
            }
            else if (skinType == "64x64")
            {
                comboBoxSkinType.Text = "Steve (64x64)";
            }
            else if (skinType == "64x64HD")
            {
                comboBoxSkinType.Text = "Steve (64x64)";
            }
            else if (skinType == "64x32")
            {
                comboBoxSkinType.Text = "Steve (64x32)";
            }
            else if (skinType == "64x32HD")
            {
                comboBoxSkinType.Text = "Steve (64x32)";
            }

            Bitmap bmp = new Bitmap(displayBox.Width, displayBox.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                //Head
                g.DrawRectangle(Pens.Black, 70, 15, 40, 40);
                g.FillRectangle(Brushes.Gray, 71, 16, 39, 39);
                //Body
                g.DrawRectangle(Pens.Black, 70, 55, 40, 60);
                g.FillRectangle(Brushes.Gray, 71, 56, 39, 59);
                //Arm0
                g.DrawRectangle(Pens.Black, 50, 55, 20, 60);
                g.FillRectangle(Brushes.Gray, 51, 56, 19, 59);
                //Arm1
                g.DrawRectangle(Pens.Black, 110, 55, 20, 60);
                g.FillRectangle(Brushes.Gray, 111, 56, 19, 59);
                //Leg0
                g.DrawRectangle(Pens.Black, 70, 115, 20, 60);
                g.FillRectangle(Brushes.Gray, 71, 116, 19, 59);
                //Leg1
                g.DrawRectangle(Pens.Black, 90, 115, 20, 60);
                g.FillRectangle(Brushes.Gray, 91, 116, 19, 59);
            }
            displayBox.Image = bmp;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            themeName.Text = "THEMENAME";
            themeName.Tag = textTheme.Text;
        }

        private void buttonSkin_Click(object sender, EventArgs e)
        {
            contextMenuSkin.Show(System.Windows.Forms.Form.ActiveForm.Location.X + buttonSkin.Location.X + 2, System.Windows.Forms.Form.ActiveForm.Location.Y + buttonSkin.Location.Y + 23);
        }

        private void buttonCape_Click(object sender, EventArgs e)
        {
            contextMenuCape.Show(System.Windows.Forms.Form.ActiveForm.Location.X + buttonCape.Location.X + 2, System.Windows.Forms.Form.ActiveForm.Location.Y + buttonCape.Location.Y + 23);
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofD = new OpenFileDialog();
            if (ofD.ShowDialog() == DialogResult.OK)
            {
                ofd = ofD.FileName;
                checkImage();
            }
        }

        private void replaceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (var ofd1 = new OpenFileDialog())
            {
                if (ofd1.ShowDialog() == DialogResult.OK)
                {
                    ofd1.Filter = "PNG Files | *.png";
                    ofd1.Title = "Select a PNG File";

                    var img = Image.FromFile(ofd1.FileName);
                    if (img.Width == img.Height * 2)
                    {
                        useCape = true;
                        pictureBoxWithInterpolationMode1.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBoxWithInterpolationMode1.InterpolationMode = InterpolationMode.NearestNeighbor;
                        pictureBoxWithInterpolationMode1.Image = Image.FromFile(ofd1.FileName);

                        mfc.data = File.ReadAllBytes(ofd1.FileName);

                        contextMenuCape.Items[0].Text = "Replace";
                    }
                    else
                    {
                        MessageBox.Show("Not a Valid Cape File");
                    }
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (textSkinID.Text.Length / 8 == 1)
                {
                    bool mashupStructure = false;
                    int skinsFolder = 0;

                    foreach (TreeNode item in treeView1.Nodes)
                    {
                        if (item.Text == "Skins")
                        {
                            mashupStructure = true;
                            skinsFolder = item.Index;
                        }
                    }

                    if (useCape == true)
                    {
                        try
                        {
                            capePath.Text = "CAPEPATH";
                            capePath.Tag = "dlccape" + textSkinID.Text + ".png";

                            mf.properties.Add(capePath.Text, capePath.Tag.ToString());

                            currentPCK.file_entries.Add(mfc);

                            mfc.size = mf.data.Length; if (mashupStructure == true)
                            {
                                mfc.name = "Skins/" + "dlccape" + textSkinID.Text + ".png";
                            }
                            else
                            {
                                mfc.name = "dlccape" + textSkinID.Text + ".png";
                            }

                            //mfc.type = 1;

                            cape.Text = "dlccape" + textSkinID.Text + ".png";
                            cape.Tag = mfc;

                            cape.ImageIndex = 2;
                            cape.SelectedImageIndex = 2;

                            if (mashupStructure == true)
                            {
                                treeView1.Nodes[skinsFolder].Nodes.Add(cape);
                            }
                            else
                            {
                                treeView1.Nodes.Add(cape);
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Cape Could Not be Added");
                        }
                    }

                    currentPCK.file_entries.Add(mf);
                    free.Text = "FREE";
                    free.Tag = "1";
                    themeName.Text = "THEMENAME";
                    themeName.Tag = textThemeName.Text;
                    displayNameId.Text = "DISPLAYNAMEID";
                    displayNameId.Tag = "IDS_dlcskin" + textSkinID.Text + "_DISPLAYNAME";
                    skinName.Text = "DISPLAYNAME";
                    skinName.Tag = textSkinName.Text;
                    anim.Text = "ANIM";

                    mf.properties.Add(skinName.Text, textSkinName.Text);

                    mf.properties.Add(displayNameId.Text, "IDS_dlcskin" + textSkinID.Text + "_DISPLAYNAME");


                    if (comboBoxSkinType.Text == "Default (64x32)")
                    {

                    }
                    else if (comboBoxSkinType.Text == "Alex (64x64)" && skinType != "64x32")
                    {
                        anim.Tag = "0x80000";

                        object[] ANIM = { anim.Text, anim.Tag };
                        mf.properties.Add("ANIM", "0x80000");
                    }
                    else if (comboBoxSkinType.Text == "Steve (64x64)" && skinType != "64x32")
                    {
                        mf.properties.Add("ANIM", "0x40000");
                    }
                    else if (comboBoxSkinType.Text == "Custom")
                    {
                        //mf.entries.Add(new object[2] { (object)"BOX", new ListViewItem() { Tag = ((object)(listViewItem.Tag.ToString() + " " + listViewItem.SubItems[1].Text + " " + listViewItem.SubItems[2].Text + " " + listViewItem.SubItems[3].Text + " " + listViewItem.SubItems[4].Text + " " + listViewItem.SubItems[5].Text + " " + listViewItem.SubItems[6].Text + " " + listViewItem.SubItems[7].Text + " " + listViewItem.SubItems[8].Text)) }.Tag });
                        //foreach (object[] item in generatedModel)
                        //{
                        //    mf.properties.Add((object[])item);
                        //}
                        mf.properties.Add("ANIM", "0x7ff5fc10");
                    }
                    if (generatedModel != null)
                    {
                        generatedModel.Clear();
                    }

                    if (themeName.Tag.ToString() != "")
                    {
                        mf.properties.Add(themeName.Text, themeName.Tag.ToString());
                    }

                    mf.properties.Add("GAME_FLAGS", "0x18");
                    mf.properties.Add("FREE", "1");

                    mf.size = mf.data.Length;
                    if (mashupStructure == true)
                    {
                        mf.name = "Skins/" + "dlcskin" + textSkinID.Text + ".png";
                    }
                    else
                    {
                        mf.name = "dlcskin" + textSkinID.Text + ".png";
                    }
                    //mf.type = 0;

                    skin.Text = "dlcskin" + textSkinID.Text + ".png";
                    skin.Tag = mf;

                    skin.ImageIndex = 2;
                    skin.SelectedImageIndex = 2;

                    if (mashupStructure == true)
                    {
                        treeView1.Nodes[skinsFolder].Nodes.Add(skin);
                    }
                    else
                    {
                        treeView1.Nodes.Add(skin);
                    }

                    displayId d = new displayId();
                    d.id = "IDS_dlcskin" + textSkinID.Text + "_DISPLAYNAME";
                    d.defaultName = textSkinName.Text;

                    currentLoc.ids.names.Add(d.id);

                    foreach (LOC.Language l in currentLoc.langs)
                        l.names.Add(d.defaultName);

                    displayId b = new displayId();
                    b.id = "IDS_dlcskin" + textSkinID.Text + "_THEMENAME";
                    b.defaultName = textThemeName.Text;

                    currentLoc.ids.names.Add(b.id);

                    foreach (LOC.Language l in currentLoc.langs)
                        l.names.Add(b.defaultName);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("The Skin ID Must be a Unique 8 Digit Number Thats Not Already in Use");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show("The Skin ID Must be a Unique 8 Digit Number Thats Not Already in Use");
            }
        }

        private void textSkinID_TextChanged_1(object sender, EventArgs e)
        {
            bool valid = true;
            if (textSkinID.Text.Length == 8)
            {
                try
                {
                    int CHECK = int.Parse(textSkinID.Text);
                }
                catch
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }

            if (valid == false)
            {
                textSkinID.ForeColor = Color.Red;
            }
            else if (valid == true)
            {
                textSkinID.ForeColor = Color.Green;
            }
        }

        private void textSkinName_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void textThemeName_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void textThemeName_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //Prompt for skin model generator
            if (MessageBox.Show("Create your own custom skin model?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            
            PictureBox preview = new PictureBox();//Creates new picture for generated model preview
            generateModel generate = new generateModel(generatedModel, preview);

            if (generate.ShowDialog() == DialogResult.OK)//Opens Model Generator Dialog
            {
                comboBoxSkinType.Items.Add((object)"Custom");//Adds skin preset to combobox
                comboBoxSkinType.Text = "Custom";//Sets combo to custom preset
                displayBox.Image = preview.Image;//Sets displayBox to created model preview
                try
                {
                    using (FileStream stream = new FileStream(Application.StartupPath + "\\temp.png", FileMode.Open, FileAccess.Read))
                    {
                        pictureBoxTexture.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBoxTexture.InterpolationMode = InterpolationMode.NearestNeighbor;
                        pictureBoxTexture.Image = Image.FromStream(stream);
                        stream.Close();
                        stream.Dispose();
                    }
                    ofd = Application.StartupPath + "\\temp.png";
                    //Sets texture box
                    pictureBoxTexture.Width = pictureBoxTexture.Height;
                    buttonDone.Enabled = true;
                    labelSelectTexture.Visible = false;
                    if (skinType != "64x64" && skinType != "64x64HD")
                    {
                        buttonSkin.Location = new Point(buttonSkin.Location.X - pictureBoxTexture.Width, buttonSkin.Location.Y);
                        skinType = "64x64";
                    }

                    mf.data = File.ReadAllBytes(ofd);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Feature not Available in Beta");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAUTO.Checked == true)
            {
                try
                {
                        Random random = new Random();
                        int num = random.Next(10000000, 99999999);
                        textSkinID.Text = num.ToString();
                        textSkinID.Enabled = false;
                    
                }
                catch
                {
                }
            }
        }

        private void radioLOCAL_CheckedChanged(object sender, EventArgs e)
        {
            if (radioLOCAL.Checked == true)
            {
                textSkinID.Text = localID;
                textSkinID.Enabled = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (var ofdd = new OpenFileDialog())
            {
                ofdd.Filter = "PNG Files | *.png";
                ofdd.Title = "Select a PNG File";
                if (ofdd.ShowDialog() == DialogResult.OK)
                {
                    ofd = ofdd.FileName;
                    checkImage();
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            using (var ofdd = new OpenFileDialog())
            {
                ofdd.Filter = "PNG Files | *.png";
                ofdd.Title = "Select a PNG File";
                if (ofdd.ShowDialog() == DialogResult.OK)
                {
                    ofd = ofdd.FileName;
                    checkImage();
                }
            }
        }

        private void radioSERVER_CheckedChanged(object sender, EventArgs e)
        {

            if (radioSERVER.Checked == true)
            {
            }
        }
    }
}

