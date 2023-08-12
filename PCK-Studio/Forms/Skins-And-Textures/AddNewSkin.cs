using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using PckStudio.Internal;
using PckStudio.Forms.Editor;
using PckStudio.IO._3DST;
using PckStudio.Properties;
using PckStudio.Forms;

namespace PckStudio.Popups
{
    public partial class AddNewSkin : MetroFramework.Forms.MetroForm
    {
        public PckFile.FileData SkinFile => skin;
        public PckFile.FileData CapeFile => cape;
        public bool HasCape { get; private set; } = false;

        private LOCFile currentLoc;
        private PckFile.FileData skin = new PckFile.FileData("dlcskinXYXYXYXY", PckFile.FileData.FileType.SkinFile);
        private PckFile.FileData cape = new PckFile.FileData("dlccapeXYXYXYXY", PckFile.FileData.FileType.CapeFile);
        private SkinANIM anim = new SkinANIM();

        private eSkinType skinType;

        private enum eSkinType
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
                    anim.SetFlag(SkinAnimFlag.RESOLUTION_64x64, true);
                    MessageBox.Show("64x64 Skin Detected");
                    skinType = eSkinType._64x64;
                    break;
                case 32:
                    anim.SetFlag(SkinAnimFlag.RESOLUTION_64x64 | SkinAnimFlag.SLIM_MODEL, false);
                    MessageBox.Show("64x32 Skin Detected");
                    skinType = eSkinType._64x32;
                    break;
                default:
                    if (img.Width == img.Height)
                    {
                        anim.SetFlag(SkinAnimFlag.RESOLUTION_64x64, true);
                        MessageBox.Show("64x64 HD Skin Detected");
                        skinType = eSkinType._64x64HD;
                        return;
                    }

                    if (img.Height == img.Width / 2)
                    {
                        anim.SetFlag(SkinAnimFlag.RESOLUTION_64x64 | SkinAnimFlag.SLIM_MODEL, false);
                        MessageBox.Show("64x32 HD Skin Detected");
                        skinType = eSkinType._64x32HD;
                        return;
                    }
                    
                    MessageBox.Show("Not a Valid Skin File");
                    skinType = eSkinType.Invalid;
                    return;
            }

            skinPictureBoxTexture.Image = img;
            buttonDone.Enabled = true;
            labelSelectTexture.Visible = false;
        }

        private void DrawModel()
		{
            bool isSlim = anim.GetFlag(SkinAnimFlag.SLIM_MODEL);
            Pen outlineColor = Pens.Black;
            Brush fillColor = Brushes.Gray;
            using (Graphics g = Graphics.FromImage(displayBox.Image))
            {
                if(!anim.GetFlag(SkinAnimFlag.HEAD_DISABLED))
				{
                    //Head
                    g.DrawRectangle(outlineColor, 70, 15, 40, 40);
                    g.FillRectangle(fillColor, 71, 16, 39, 39);
                }
                if (!anim.GetFlag(SkinAnimFlag.BODY_DISABLED))
                {
                    //Body
                    g.DrawRectangle(outlineColor, 70, 55, 40, 60);
                    g.FillRectangle(fillColor, 71, 56, 39, 59);
                }
                if (!anim.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED))
                {
                    //Arm0
                    g.DrawRectangle(outlineColor, isSlim ? 55 : 50, 55, isSlim ? 15 : 20, 60);
                    g.FillRectangle(fillColor   , isSlim ? 56 : 51, 56, isSlim ? 14 : 19, 59);
                }
                if (!anim.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED))
                {
                    //Arm1
                    g.DrawRectangle(outlineColor, 110, 55, isSlim ? 15 : 20, 60);
                    g.FillRectangle(fillColor, 111, 56, isSlim ? 14 : 19, 59);
                }
                if (!anim.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED))
                {
                    //Leg0
                    g.DrawRectangle(outlineColor, 70, 115, 20, 60);
                    g.FillRectangle(fillColor, 71, 116, 19, 59);
                }
                if (!anim.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED))
                {
                    //Leg1
                    g.DrawRectangle(outlineColor, 90, 115, 20, 60);
                    g.FillRectangle(fillColor, 91, 116, 19, 59);
                }
            }
            displayBox.Invalidate();
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

        private void capePictureBox_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "PNG Files|*.png";
                ofd.Title = "Select a PNG File";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var img = Image.FromFile(ofd.FileName);
                    if (img.Width == img.Height * 2)
                    {
                        HasCape = true;
                        capePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        capePictureBox.InterpolationMode = InterpolationMode.NearestNeighbor;
                        capePictureBox.Image = Image.FromFile(ofd.FileName);
                        cape.SetData(File.ReadAllBytes(ofd.FileName));
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
            if (!int.TryParse(textSkinID.Text, out int _skinId))
            {
                MessageBox.Show("The Skin ID Must be a Unique 8 Digit Number Thats Not Already in Use", "Invalid Skin ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string skinId = _skinId.ToString("d08");
            skin.Filename = $"dlcskin{skinId}.png";
            string skinDisplayNameLocKey = $"IDS_dlcskin{skinId}_DISPLAYNAME";
            currentLoc.AddLocKey(skinDisplayNameLocKey, textSkinName.Text);
            skin.Properties.Add("DISPLAYNAME", textSkinName.Text);
            skin.Properties.Add("DISPLAYNAMEID", skinDisplayNameLocKey);
            if (!string.IsNullOrEmpty(textThemeName.Text))
            {
                skin.Properties.Add("THEMENAME", textThemeName.Text);
                skin.Properties.Add("THEMENAMEID", $"IDS_dlcskin{skinId}_THEMENAME");
                currentLoc.AddLocKey($"IDS_dlcskin{skinId}_THEMENAME", textThemeName.Text);
            }
            skin.Properties.Add("ANIM", anim);
            skin.Properties.Add("GAME_FLAGS", "0x18");
            skin.Properties.Add("FREE", "1");

            if (HasCape)
            {
                try
                {
                    cape.Filename = $"dlccape{skinId}.png";
                    skin.Properties.Add("CAPEPATH", cape.Filename);
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

            using var ms = new MemoryStream();
            Resources.classic_template.Save(ms, ImageFormat.Png);
            skin.SetData(ms.ToArray());

            generateModel generate = new generateModel(skin);

            if (generate.ShowDialog() == DialogResult.OK)
            {
                displayBox.Image = generate.PreviewImage;
                buttonDone.Enabled = true;
                labelSelectTexture.Visible = false;
                if (skinType != eSkinType._64x64 && skinType != eSkinType._64x64HD)
                {
                    buttonSkin.Location = new Point(buttonSkin.Location.X - skinPictureBoxTexture.Width, buttonSkin.Location.Y);
                    skinType = eSkinType._64x64;
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

        private void skinPictureBoxTexture_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "PNG Files|*.png|3DS Texture|*.3dst";
                ofd.Title = "Select a Skin Texture File";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ofd.FileName.EndsWith(".3dst"))
                    {
                        using (var fs = File.OpenRead(ofd.FileName))
                        {
                            var reader = new _3DSTextureReader();
                            CheckImage(reader.FromStream(fs));
                            textSkinName.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
                        }
                        return;
                    }
                    CheckImage(Image.FromFile(ofd.FileName));
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
            using ANIMEditor diag = new ANIMEditor(anim.ToString());
            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                anim = diag.ResultAnim;
                DrawModel();
            }
        }
	}
}