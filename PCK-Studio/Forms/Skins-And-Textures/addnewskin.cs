using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using PckStudio.Classes.FileTypes;
using System.Drawing.Imaging;
using PckStudio.Classes.Utils;
using PckStudio.Classes._3ds.Utils;
using PckStudio.ToolboxItems;

namespace PckStudio
{
    public partial class AddNewSkin : ThemeForm
    {

        public PCKFile.FileData SkinFile => skin;
        public PCKFile.FileData CapeFile => cape;
        public bool HasCape = false;

        LOCFile currentLoc;
        PCKFile.FileData skin = new PCKFile.FileData("dlcskinXYXYXYXY", PCKFile.FileData.FileType.SkinFile);
        PCKFile.FileData cape = new PCKFile.FileData("dlccapeXYXYXYXY", PCKFile.FileData.FileType.CapeFile);
        SkinANIM anim = new SkinANIM(eANIM_EFFECTS.NONE);

        eSkinType skinType;
        PCKProperties generatedModel = new PCKProperties();

        enum eSkinType : int
        {
            Invalid = -1,
            _64x64,
            _64x32,
            _64x64HD,
            _64x32HD,
            Custom,
        }

        public AddNewSkin(LOCFile loc)
        {
            InitializeComponent();
            currentLoc = loc;
        }

        private void CheckImage(Image img)
        {
            //Checks image dimensions and sets things accordingly
            switch (img.Height) // 64x64
            {
                case 64:
                    anim.SetANIMFlag(eANIM_EFFECTS.RESOLUTION_64x64, true);
                    MessageBox.Show("64x64 Skin Detected");
                    skinPictureBoxTexture.Width = skinPictureBoxTexture.Height;
                    if (skinType != eSkinType._64x64 && skinType != eSkinType._64x64HD)
                    {
                        buttonSkin.Location = new Point(buttonSkin.Location.X - skinPictureBoxTexture.Width, buttonSkin.Location.Y);
                    }
                    //comboBoxSkinType.Text = "Steve (64x64)";
                    skinType = eSkinType._64x64;
                    break;
                case 32:
                    anim.SetANIMFlag(eANIM_EFFECTS.RESOLUTION_64x64, false);
                    anim.SetANIMFlag(eANIM_EFFECTS.SLIM_MODEL, false);
                    MessageBox.Show("64x32 Skin Detected");
                    skinPictureBoxTexture.Width = skinPictureBoxTexture.Height * 2;
                    if (skinType == eSkinType._64x64)
                    {
                        buttonSkin.Location = new Point(buttonSkin.Location.X + skinPictureBoxTexture.Width / 2, buttonSkin.Location.Y);
                    }
                    if (skinType == eSkinType._64x64HD)
                    {
                        buttonSkin.Location = new Point(buttonSkin.Location.X + skinPictureBoxTexture.Width / 2, buttonSkin.Location.Y);
                    }
                    //comboBoxSkinType.Text = "Default (64x32)";
                    skinType = eSkinType._64x32;
                    break;
                default:
                    if (img.Width == img.Height) // 64x64 HD
                    {
                        anim.SetANIMFlag(eANIM_EFFECTS.RESOLUTION_64x64, true);
                        MessageBox.Show("64x64 HD Skin Detected");
                        skinPictureBoxTexture.Width = skinPictureBoxTexture.Height;
                        if (skinType != eSkinType._64x64 && skinType != eSkinType._64x64HD)
                        {
                            buttonSkin.Location = new Point(buttonSkin.Location.X - skinPictureBoxTexture.Width, buttonSkin.Location.Y);
                        }
                        //comboBoxSkinType.Text = "Steve (64x64)";
                        skinType = eSkinType._64x64HD;
                    }
                    else if (img.Width == img.Height / 2) // 64x32 HD
                    {
                        anim.SetANIMFlag(eANIM_EFFECTS.RESOLUTION_64x64, false);
                        anim.SetANIMFlag(eANIM_EFFECTS.SLIM_MODEL, false);
                        MessageBox.Show("64x32 HD Skin Detected");
                        skinPictureBoxTexture.Width = skinPictureBoxTexture.Height * 2;
                        if (skinType == eSkinType._64x64)
                        {
                            buttonSkin.Location = new Point(buttonSkin.Location.X + skinPictureBoxTexture.Width / 2, buttonSkin.Location.Y);
                        }
                        if (skinType == eSkinType._64x64HD)
                        {
                            buttonSkin.Location = new Point(buttonSkin.Location.X + skinPictureBoxTexture.Width / 2, buttonSkin.Location.Y);
                        }
                        //comboBoxSkinType.Text = "Default (64x32)";
                        skinType = eSkinType._64x32HD;
                    }
                    else //If dimensions don't fit any skin type //Invalid
                    {
                        MessageBox.Show("Not a Valid Skin File");
                        skinType = eSkinType.Invalid;
                        return;
                    }
                    break;
            }

            skinPictureBoxTexture.Image = img;
            buttonDone.Enabled = true;
            labelSelectTexture.Visible = false;
        }

        private void DrawModel()
		{
            Bitmap bmp = new Bitmap(displayBox.Width, displayBox.Height);
            bool isSlim = anim.GetANIMFlag(eANIM_EFFECTS.SLIM_MODEL);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                if(!anim.GetANIMFlag(eANIM_EFFECTS.HEAD_DISABLED))
				{
                    //Head
                    g.DrawRectangle(Pens.Black, 70, 15, 40, 40);
                    g.FillRectangle(Brushes.Gray, 71, 16, 39, 39);
                }
                if (!anim.GetANIMFlag(eANIM_EFFECTS.BODY_DISABLED))
                {
                    //Body
                    g.DrawRectangle(Pens.Black, 70, 55, 40, 60);
                    g.FillRectangle(Brushes.Gray, 71, 56, 39, 59);
                }
                if (!anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_ARM_DISABLED))
                {
                    //Arm0
                    g.DrawRectangle(Pens.Black  , isSlim ? 55 : 50, 55, isSlim ? 15 : 20, 60);
                    g.FillRectangle(Brushes.Gray, isSlim ? 56 : 51, 56, isSlim ? 14 : 19, 59);
                }
                if (!anim.GetANIMFlag(eANIM_EFFECTS.LEFT_ARM_DISABLED))
                {
                    //Arm1
                    g.DrawRectangle(Pens.Black, 110, 55, isSlim ? 15 : 20, 60);
                    g.FillRectangle(Brushes.Gray, 111, 56, isSlim ? 14 : 19, 59);
                }
                if (!anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_LEG_DISABLED))
                {
                    //Leg0
                    g.DrawRectangle(Pens.Black, 70, 115, 20, 60);
                    g.FillRectangle(Brushes.Gray, 71, 116, 19, 59);
                }
                if (!anim.GetANIMFlag(eANIM_EFFECTS.LEFT_LEG_DISABLED))
                {
                    //Leg1
                    g.DrawRectangle(Pens.Black, 90, 115, 20, 60);
                    g.FillRectangle(Brushes.Gray, 91, 116, 19, 59);
                }
            }
            displayBox.Image = bmp;
        }

        private void addnewskin_Load(object sender, EventArgs e)
        {
            DrawModel();
        }

        private void buttonSkin_Click(object sender, EventArgs e)
        {
            contextMenuSkin.Show(ActiveForm.Location.X + buttonSkin.Location.X + 2, ActiveForm.Location.Y + buttonSkin.Location.Y + 23);
        }

        private void buttonCape_Click(object sender, EventArgs e)
        {
            contextMenuCape.Show(ActiveForm.Location.X + buttonCape.Location.X + 2, ActiveForm.Location.Y + buttonCape.Location.Y + 23);
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CheckImage(Image.FromFile(ofd.FileName));
            }
        }

        private void replaceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (var ofd1 = new OpenFileDialog())
            {
                ofd1.Filter = "PNG Files|*.png";
                ofd1.Title = "Select a PNG File";
                if (ofd1.ShowDialog() == DialogResult.OK)
                {
                    var img = Image.FromFile(ofd1.FileName);
                    if (img.Width == img.Height * 2)
                    {
                        HasCape = true;
                        capePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        capePictureBox.InterpolationMode = InterpolationMode.NearestNeighbor;
                        capePictureBox.Image = Image.FromFile(ofd1.FileName);
                        cape.SetData(File.ReadAllBytes(ofd1.FileName));
                        contextMenuCape.Items[0].Text = "Replace";
                    }
                    else
                    {
                        MessageBox.Show("Not a Valid Cape File");
                    }
                }
            }
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            int _skinId = -1;
            if (!int.TryParse(textSkinID.Text, out _skinId))
            {
                MessageBox.Show("The Skin ID Must be a Unique 8 Digit Number Thats Not Already in Use", "Invalid Skin ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string skinId = _skinId.ToString("d08");
            skin.filepath = $"dlcskin{skinId}.png";
            string skinDisplayNameLocKey = $"IDS_dlcskin{skinId}_DISPLAYNAME";
            currentLoc.AddLocKey(skinDisplayNameLocKey, textSkinName.Text);
            skin.properties.Add(("DISPLAYNAME", textSkinName.Text));
            skin.properties.Add(("DISPLAYNAMEID", skinDisplayNameLocKey));
            if (!string.IsNullOrEmpty(textThemeName.Text))
            {
                skin.properties.Add(("THEMENAME", textThemeName.Text));
                skin.properties.Add(("THEMENAMEID", $"IDS_dlcskin{skinId}_THEMENAME"));
                currentLoc.AddLocKey($"IDS_dlcskin{skinId}_THEMENAME", textThemeName.Text);
            }
            skin.properties.Add(("ANIM", anim.ToString()));
            skin.properties.Add(("GAME_FLAGS", "0x18"));
            skin.properties.Add(("FREE", "1"));

            if (HasCape)
            {
                try
                {
                    cape.filepath = $"dlccape{skinId}.png";
                    skin.properties.Add(("CAPEPATH", cape.filepath));
                }
                catch (Exception)
                {
                    MessageBox.Show("Cape could not be added.");
                }
            }
            using (var stream = new MemoryStream())
            {
                skinPictureBoxTexture.Image.Save(stream, ImageFormat.Png);
                skin.SetData(stream.ToArray());
            }

            //if (generatedModel != null)
            //{
            //    foreach (var item in generatedModel)
            //    {
            //        skin.properties.Add(item);
            //    }

            //    generatedModel.Clear();
            //}


            DialogResult = DialogResult.OK;
            Close();
        }

        private void textSkinID_TextChanged(object sender, EventArgs e)
        {
            bool validSkinId = int.TryParse(textSkinID.Text, out _);
            textSkinID.ForeColor = validSkinId ? Color.Green : Color.Red;
        }

        private void CreateCustomModel_Click(object sender, EventArgs e)
        {
            //Prompt for skin model generator
            if (MessageBox.Show("Create your own custom skin model?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            
            PictureBox preview = new PictureBox(); //Creates new picture for generated model preview
            GenerateModel generate = new GenerateModel(generatedModel, preview);

            if (generate.ShowDialog() == DialogResult.OK) //Opens Model Generator Dialog
            {
                //comboBoxSkinType.Items.Add("Custom"); //Adds skin preset to combobox
                //comboBoxSkinType.Text = "Custom"; //Sets combo to custom preset
                displayBox.Image = preview.Image; //Sets displayBox to created model preview
                try
                {
                    using (FileStream stream = File.OpenRead(Application.StartupPath + "\\temp.png"))
                    {
                        skinPictureBoxTexture.Image = Image.FromStream(stream);
                    }
                    buttonDone.Enabled = true;
                    labelSelectTexture.Visible = false;
                    if (skinType != eSkinType._64x64 && skinType != eSkinType._64x64HD)
                    {
                        buttonSkin.Location = new Point(buttonSkin.Location.X - skinPictureBoxTexture.Width, buttonSkin.Location.Y);
                        skinType = eSkinType._64x64;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAUTO.Checked)
            {
                try
                {
                    Random random = new Random();
                    int num = random.Next(100000, 99999999);
                    textSkinID.Text = num.ToString();
                    textSkinID.Enabled = false;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void radioLOCAL_CheckedChanged(object sender, EventArgs e)
        {
            textSkinID.Enabled = radioLOCAL.Checked;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (var ofdd = new OpenFileDialog())
            {
                ofdd.Filter = "PNG Files|*.png|3DS Texture|*.3dst";
                ofdd.Title = "Select a Skin Texture File";
                if (ofdd.ShowDialog() == DialogResult.OK)
                {
                    if (ofdd.FileName.EndsWith(".3dst"))
                    {
                        using (var fs = File.OpenRead(ofdd.FileName))
                        {
                            CheckImage(_3DSUtil.GetImageFrom3DST(fs));
                            textSkinName.Text = Path.GetFileNameWithoutExtension(ofdd.FileName);
                        }
                        return;
                    }
                    CheckImage(Image.FromFile(ofdd.FileName));
                }
            }
        }

        private void radioSERVER_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSERVER.Checked)
            {
            }
        }

		private void buttonAnimGen_Click(object sender, EventArgs e)
		{
            using Forms.Utilities.Skins.ANIMEditor diag = new Forms.Utilities.Skins.ANIMEditor(anim.ToString());
            if (diag.ShowDialog(this) == DialogResult.OK && diag.saved)
            {
                anim = new SkinANIM(diag.outANIM);
                DrawModel();
            }
        }
	}
}