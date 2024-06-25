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
using PckStudio.Internal.IO._3DST;
using PckStudio.Properties;
using PckStudio.Forms;
using PckStudio.Extensions;
using System.Linq;
using System.Diagnostics;
using PckStudio.Internal.Skin;

namespace PckStudio.Popups
{
    public partial class AddNewSkin : MetroFramework.Forms.MetroForm
    {
        public Skin NewSkin => newSkin;

        private Skin newSkin;
        private Random rng = new Random();

        public AddNewSkin()
        {
            InitializeComponent();
            newSkin = new Skin("", Resources.classic_template);
        }

        private void SetNewTexture(Image img)
        {
            if (img is null)
            {
                Debug.Assert(false, "Image is null.");
            }
            if (img.Width != img.Height && img.Height != img.Width / 2)
            {
                MessageBox.Show("The selected image does not suit a skin texture.", "Invalid image dimensions.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            newSkin.Model.ANIM.SetFlag(SkinAnimFlag.RESOLUTION_64x64, img.Width == img.Height);

            skinPictureBox.Image = newSkin.Model.Texture = img;
            labelSelectTexture.Visible = false;
            //capePictureBox.Visible = true;
            //buttonCape.Visible = true;
            //capeLabel.Visible = true;
            //buttonDone.Enabled = true;
            //buttonAnimGen.Enabled = true;
        }

        private void DrawModel()
		{
            bool isSlim = newSkin.Model.ANIM.GetFlag(SkinAnimFlag.SLIM_MODEL);
            Pen outlineColor = Pens.LightGray;
            Brush fillColor = Brushes.Gray;
            Image previewTexture = new Bitmap(displayBox.Width, displayBox.Height);
            using (Graphics g = Graphics.FromImage(previewTexture))
            {
                if(!newSkin.Model.ANIM.GetFlag(SkinAnimFlag.HEAD_DISABLED))
				{
                    //Head
                    g.DrawRectangle(outlineColor, 70, 15, 40, 40);
                    g.FillRectangle(fillColor, 71, 16, 39, 39);
                }
                if (!newSkin.Model.ANIM.GetFlag(SkinAnimFlag.BODY_DISABLED))
                {
                    //Body
                    g.DrawRectangle(outlineColor, 70, 55, 40, 60);
                    g.FillRectangle(fillColor, 71, 56, 39, 59);
                }
                if (!newSkin.Model.ANIM.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED))
                {
                    //Arm0
                    g.DrawRectangle(outlineColor, isSlim ? 55 : 50, 55, isSlim ? 15 : 20, 60);
                    g.FillRectangle(fillColor   , isSlim ? 56 : 51, 56, isSlim ? 14 : 19, 59);
                }
                if (!newSkin.Model.ANIM.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED))
                {
                    //Arm1
                    g.DrawRectangle(outlineColor, 110, 55, isSlim ? 15 : 20, 60);
                    g.FillRectangle(fillColor, 111, 56, isSlim ? 14 : 19, 59);
                }
                if (!newSkin.Model.ANIM.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED))
                {
                    //Leg0
                    g.DrawRectangle(outlineColor, 70, 115, 20, 60);
                    g.FillRectangle(fillColor, 71, 116, 19, 59);
                }
                if (!newSkin.Model.ANIM.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED))
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
            contextMenuSkin.Show(this, buttonSkin.Location.X + 2, buttonSkin.Location.Y + buttonSkin.Size.Height);
        }

        private void buttonCape_Click(object sender, EventArgs e)
        {
            contextMenuCape.Show(this, buttonCape.Location.X + 2, buttonCape.Location.Y + buttonCape.Size.Height);
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SetNewTexture(Image.FromFile(ofd.FileName).ReleaseFromFile());
            }
        }

        private void skinPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
                return;

            if (e.Button == MouseButtons.Right)
            {
                contextMenuSkin.Show(
                    this,
                    x: skinPictureBox.Location.X,
                    y: skinPictureBox.Location.Y + skinPictureBox.Size.Height
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
                            SetNewTexture(reader.FromStream(fs));
                        }
                        textSkinName.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
                        return;
                    }

                    var img = Image.FromFile(ofd.FileName).ReleaseFromFile();
                    SetNewTexture(img);
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
                    this,
                    x: capePictureBox.Location.X,
                    y: capePictureBox.Location.Y + capePictureBox.Size.Height
                    );
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Cape File|*.png";
                ofd.Title = "Select a PNG File";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var img = Image.FromFile(ofd.FileName).ReleaseFromFile();
                    if (img.RawFormat != ImageFormat.Png && img.Width != img.Height * 2)
                    {
                        MessageBox.Show(this, "Not a Valid Cape File");
                        return;
                    }
                    newSkin.CapeTexture = capePictureBox.Image = img;
                    contextMenuCape.Items[0].Text = "Replace";
                    capeLabel.Visible = false;
                    contextMenuCape.Visible = true;
                }
            }
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (radioButtonManual.Checked)
            {
                if (!int.TryParse(textSkinID.Text, out int _skinId))
                {
                    MessageBox.Show("The Skin Id must be a unique 8 digit number that is not already in use", "Invalid Skin Id", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                newSkin.MetaData.Id = _skinId;
            }
            newSkin.MetaData.Name = textSkinName.Text;
            newSkin.MetaData.Theme = textThemeName.Text;
            DialogResult = DialogResult.OK;
        }

        private void textSkinID_TextChanged(object sender, EventArgs e)
        {
            bool validSkinId = int.TryParse(textSkinID.Text, out _);
            textSkinID.ForeColor = validSkinId ? Color.Green : Color.Red;
        }

        private void CreateCustomModel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Create your own custom skin model?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;

            newSkin.MetaData.Name = textSkinName.Text;
            newSkin.MetaData.Theme = textThemeName.Text;

            using CustomSkinEditor customSkinEditor = new CustomSkinEditor(newSkin);

            if (customSkinEditor.ShowDialog() == DialogResult.OK)
            {
                skinPictureBox.Image = customSkinEditor.ResultSkin.Model.Texture;
                newSkin = customSkinEditor.ResultSkin;
                buttonDone.Enabled = true;
                labelSelectTexture.Visible = false;
                DrawModel();
            }
        }

        private void radioButtonAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAuto.Checked)
            {
                newSkin.MetaData.Id = rng.Next(100000, 99999999);
                textSkinID.Text = newSkin.MetaData.Id.ToString();
                textSkinID.Enabled = false;
            }
        }

        private void radioButtonManual_CheckedChanged(object sender, EventArgs e)
        {
            textSkinID.Enabled = radioButtonManual.Checked;
        }

		private void buttonAnimGen_Click(object sender, EventArgs e)
		{
            using ANIMEditor diag = new ANIMEditor(newSkin.Model.ANIM);
            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                newSkin.Model.ANIM = diag.ResultAnim;
                DrawModel();
            }
        }
    }
}