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
using PckStudio.Extensions;

namespace PckStudio.Popups
{
    public partial class AddNewSkin : MetroFramework.Forms.MetroForm
    {
        public PckFileData SkinFile => skin;
        public PckFileData CapeFile => cape;
        public bool HasCape => cape is not null;

        private LOCFile currentLoc;
        private PckFileData skin = new PckFileData("dlcskinXYXYXYXY", PckFileType.SkinFile);
        private PckFileData cape;
        private SkinANIM anim = new SkinANIM();
        private Random rng = new Random();

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
            switch (img.Height)
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
                        break;
                    }

                    if (img.Height == img.Width / 2)
                    {
                        anim.SetFlag(SkinAnimFlag.RESOLUTION_64x64 | SkinAnimFlag.SLIM_MODEL, false);
                        MessageBox.Show("64x32 HD Skin Detected");
                        skinType = eSkinType._64x32HD;
                        break;
                    }
                    
                    MessageBox.Show("Not a Valid Skin File");
                    skinType = eSkinType.Invalid;
                    return;
            }

            skinPictureBox.Image = img;
            capePictureBox.Visible = true;
            buttonCape.Visible = true;
            capeLabel.Visible = true;
            buttonDone.Enabled = true;
            buttonAnimGen.Enabled = true;
            labelSelectTexture.Visible = false;
        }

        private void DrawModel()
		{
            bool isSlim = anim.GetFlag(SkinAnimFlag.SLIM_MODEL);
            Pen outlineColor = Pens.LightGray;
            Brush fillColor = Brushes.Gray;
            Image previewTexture = new Bitmap(displayBox.Width, displayBox.Height);
            using (Graphics g = Graphics.FromImage(previewTexture))
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
            displayBox.Image = previewTexture;
        }

        private void AddNewSkin_Load(object sender, EventArgs e)
        {
            DrawModel();
        }

        private void buttonSkin_Click(object sender, EventArgs e)
        {
            contextMenuSkin.Show(Location.X + buttonSkin.Location.X + 2, Location.Y + buttonSkin.Location.Y + buttonSkin.Size.Height);
        }

        private void buttonCape_Click(object sender, EventArgs e)
        {
            contextMenuCape.Show(Location.X + buttonCape.Location.X + 2, Location.Y + buttonCape.Location.Y + buttonCape.Size.Height);
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CheckImage(Image.FromFile(ofd.FileName));
            }
        }

        private void skinPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
                return;

            if (e.Button == MouseButtons.Right)
            {
                contextMenuSkin.Show(
                    x: Location.X + skinPictureBox.Location.X,
                    y: Location.Y + skinPictureBox.Location.Y + skinPictureBox.Size.Height
                    );
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Skin File|*.png|3DS Texture|*.3dst";
                ofd.Title = "Select a Skin Texture File";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ofd.FileName.EndsWith(".3dst"))
                    {
                        using (var fs = File.OpenRead(ofd.FileName))
                        {
                            var reader = new _3DSTextureReader();
                            CheckImage(reader.FromStream(fs));
                        }
                        textSkinName.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
                        return;
                    }
                    CheckImage(Image.FromFile(ofd.FileName));
                }
            }
        }

        private void capePictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
                return;

            if (e.Button == MouseButtons.Right)
            {
                contextMenuCape.Show(
                    x: Location.X + capePictureBox.Location.X,
                    y: Location.Y + capePictureBox.Location.Y + capePictureBox.Size.Height
                    );
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Cape File|*.png";
                ofd.Title = "Select a PNG File";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var img = Image.FromFile(ofd.FileName);
                    if (img.RawFormat != ImageFormat.Png && img.Width != img.Height * 2)
                    {
                        MessageBox.Show("Not a Valid Cape File");
                        return;
                    }
                    capePictureBox.Image = Image.FromFile(ofd.FileName);
                    cape ??= new PckFileData("dlccapeXYXYXYXY", PckFileType.CapeFile);
                    cape.SetData(File.ReadAllBytes(ofd.FileName));
                    contextMenuCape.Items[0].Text = "Replace";
                    capeLabel.Visible = false;
                    contextMenuCape.Visible = true;
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
                cape.Filename = $"dlccape{skinId}.png";
                skin.Properties.Add("CAPEPATH", cape.Filename);
            }
            skin.SetData(skinPictureBox.Image, ImageFormat.Png);
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

            skin.SetData(Resources.classic_template, ImageFormat.Png);

            using CustomModelEditor generate = new CustomModelEditor(skin);

            if (generate.ShowDialog() == DialogResult.OK)
            {
                displayBox.Image = generate.PreviewImage;
                buttonDone.Enabled = true;
                labelSelectTexture.Visible = false;
                if (skinType != eSkinType._64x64 && skinType != eSkinType._64x64HD)
                {
                    buttonSkin.Location = new Point(buttonSkin.Location.X - skinPictureBox.Width, buttonSkin.Location.Y);
                    skinType = eSkinType._64x64;
                }
            }
        }

        private void radioButtonAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAuto.Checked)
            {
                int num = rng.Next(100000, 99999999);
                textSkinID.Text = num.ToString();
                textSkinID.Enabled = false;
            }
        }

        private void radioButtonManual_CheckedChanged(object sender, EventArgs e)
        {
            textSkinID.Enabled = radioButtonManual.Checked;
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