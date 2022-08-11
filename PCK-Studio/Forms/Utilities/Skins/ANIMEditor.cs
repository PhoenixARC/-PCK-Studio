using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using PckStudio.Classes.Utils;

namespace PckStudio.Forms.Utilities.Skins
{
	public partial class ANIMEditor : MetroFramework.Forms.MetroForm
	{
		public bool saved = false;
		public string outANIM => animValue.Text;
		SkinANIM anim = new SkinANIM();

		void processCheckBoxes(bool set_all = false, bool value = false)
		{
			bobbingCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.HEAD_BOBBING_DISABLED);
			bodyCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.BODY_DISABLED);
			bodyOCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.BODY_OVERLAY_DISABLED);
			chestplateCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.FORCE_BODY_ARMOR);

			classicCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.RESOLUTION_64x64);
			crouchCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.DO_BACKWARDS_CROUCH);
			dinnerboneCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.DINNERBONE);
			headCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.HEAD_DISABLED);

			headOCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.HEAD_OVERLAY_DISABLED);
			helmetCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.FORCE_HEAD_ARMOR);
			leftArmCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.LEFT_ARM_DISABLED);
			leftArmOCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED);

			leftArmorCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.FORCE_LEFT_ARM_ARMOR);
			leftLegCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.LEFT_LEG_DISABLED);
			leftLeggingCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.FORCE_LEFT_LEG_ARMOR);
			leftLegOCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED);

			noArmorCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.ALL_ARMOR_DISABLED);
			rightArmCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_ARM_DISABLED);
			rightArmOCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED);
			rightArmorCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.FORCE_RIGHT_ARM_ARMOR);

			rightLegCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_LEG_DISABLED);
			rightLeggingCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.FORCE_RIGHT_LEG_ARMOR);
			rightLegOCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED);
			santaCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.BAD_SANTA);

			slimCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.SLIM_MODEL);
			staticArmsCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.STATIC_ARMS);
			staticLegsCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.STATIC_LEGS);
			statueCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.STATUE_OF_LIBERTY);

			syncArmsCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.SYNCED_ARMS);
			syncLegsCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.SYNCED_LEGS);
			unknownCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.unk_BIT4);
			zombieCheckBox.Checked = set_all ? value : anim.GetANIMFlag(eANIM_EFFECTS.ZOMBIE_ARMS);
		}

		public ANIMEditor(string ANIM)
		{
			InitializeComponent();
			if (!SkinANIM.IsValidANIM(ANIM))
			{
				DialogResult = DialogResult.Abort;
				Close();
			}
			anim = new SkinANIM(ANIM);
			
			bobbingCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.HEAD_BOBBING_DISABLED); };
			bodyCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.BODY_DISABLED); };
			bodyOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.BODY_OVERLAY_DISABLED); };
			chestplateCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_BODY_ARMOR); };

			classicCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.RESOLUTION_64x64); };
			crouchCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.DO_BACKWARDS_CROUCH); };
			dinnerboneCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.DINNERBONE); };
			headCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.HEAD_DISABLED); };

			headOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.HEAD_OVERLAY_DISABLED); };
			helmetCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_HEAD_ARMOR); };
			leftArmCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.LEFT_ARM_DISABLED); };
			leftArmOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED); };

			leftArmorCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_LEFT_ARM_ARMOR); };
			leftLegCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.LEFT_LEG_DISABLED); };
			leftLeggingCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_LEFT_LEG_ARMOR); };
			leftLegOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED); };

			noArmorCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.ALL_ARMOR_DISABLED); };
			rightArmCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.RIGHT_ARM_DISABLED); };
			rightArmOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED); };
			rightArmorCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_RIGHT_ARM_ARMOR); };

			rightLegCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.RIGHT_LEG_DISABLED); };
			rightLeggingCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_RIGHT_LEG_ARMOR); };
			rightLegOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED); };
			santaCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.BAD_SANTA); };

			slimCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.SLIM_MODEL); };
			staticArmsCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.STATIC_ARMS); };
			staticLegsCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.STATIC_LEGS); };
			statueCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.STATUE_OF_LIBERTY); };

			syncArmsCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.SYNCED_ARMS); };
			syncLegsCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.SYNCED_LEGS); };
			unknownCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.unk_BIT4); };
			zombieCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.ZOMBIE_ARMS); };

			helmetCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_HEAD_ARMOR); };
			chestplateCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_BODY_ARMOR); };
			rightArmorCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_RIGHT_ARM_ARMOR); };
			leftArmorCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_LEFT_ARM_ARMOR); };
			rightLeggingCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_RIGHT_LEG_ARMOR); };
			leftLeggingCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, eANIM_EFFECTS.FORCE_LEFT_LEG_ARMOR); };

			processCheckBoxes();
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			saved = true;
			Close();
		}

		private void flagChanged(object sender, EventArgs e, eANIM_EFFECTS flag)
		{
			// Set value
			anim.SetANIMFlag(flag, ((CheckBox)sender).Checked && ((CheckBox)sender).Enabled);

			// Armor flags don't work if the respective parts are not enabled
			helmetCheckBox.Enabled = anim.GetANIMFlag(eANIM_EFFECTS.HEAD_DISABLED);
			chestplateCheckBox.Enabled = anim.GetANIMFlag(eANIM_EFFECTS.BODY_DISABLED);
			rightArmorCheckBox.Enabled = anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_ARM_DISABLED);
			leftArmorCheckBox.Enabled = anim.GetANIMFlag(eANIM_EFFECTS.LEFT_ARM_DISABLED);
			rightLeggingCheckBox.Enabled = anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_LEG_DISABLED);
			leftLeggingCheckBox.Enabled = anim.GetANIMFlag(eANIM_EFFECTS.LEFT_LEG_DISABLED);

			animValue.Text = anim.ToString();
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(animValue.Text);
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			string new_value = "";

			bool first = true;
			while (!SkinANIM.IsValidANIM(new_value))
			{
				if (!first) MessageBox.Show("The following value \"" + new_value + "\" is not valid. Please try again.");
				RenamePrompt diag = new RenamePrompt(new_value);
				diag.TextLabel.Text = "ANIM";
				diag.OKButton.Text = "Ok";
				if (diag.ShowDialog() == DialogResult.OK)
				{
					new_value = diag.NewText;
				}
				else return;
				first = false;
			}
			anim = new SkinANIM(new_value);
			processCheckBoxes();
		}

		private void uncheckButton_Click(object sender, EventArgs e)
		{
			processCheckBoxes(true);
		}

		private void checkButton_Click(object sender, EventArgs e)
		{
			processCheckBoxes(true, true);
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = animValue.Text + ".png";
			saveFileDialog.Filter = "Skin textures|*.png";
			if (saveFileDialog.ShowDialog() != DialogResult.OK ||
				string.IsNullOrWhiteSpace(Path.GetDirectoryName(saveFileDialog.FileName))) return;
			bool isSlim = anim.GetANIMFlag(eANIM_EFFECTS.SLIM_MODEL);
			bool isClassic64 = anim.GetANIMFlag(eANIM_EFFECTS.RESOLUTION_64x64);
			bool isClassic32 = !isSlim && !isClassic64;

			Image skin = isSlim ? Properties.Resources.slim_template : Properties.Resources.classic_template;

			Bitmap nb = new Bitmap(64, (!isSlim && !isClassic64) ? 32 : 64);
			using (Graphics g = Graphics.FromImage(nb))
			{
				g.DrawImage(skin, new Rectangle(0, 0, 64, isClassic32 ? 32 : 64), new Rectangle(0, 0, 64, isClassic32 ? 32 : 64), GraphicsUnit.Pixel);
				if (anim.GetANIMFlag(eANIM_EFFECTS.HEAD_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(32, 0, 32, 16));
				if (anim.GetANIMFlag(eANIM_EFFECTS.HEAD_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(0, 0, 32, 16));
				if (anim.GetANIMFlag(eANIM_EFFECTS.BODY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(16, 16, 24, 16));
				if (nb.Height == 64)
				{
					if (anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_ARM_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(40, 16, 16, 16));
					if (anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_LEG_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(0, 16, 16, 16));
					if (anim.GetANIMFlag(eANIM_EFFECTS.BODY_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(16, 32, 24, 16));
					if (anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(40, 32, 16, 16));
					if (anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(0, 32, 16, 16));
					if (anim.GetANIMFlag(eANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(0, 48, 16, 16));
					if (anim.GetANIMFlag(eANIM_EFFECTS.LEFT_LEG_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(16, 48, 16, 16));
					if (anim.GetANIMFlag(eANIM_EFFECTS.LEFT_ARM_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(32, 48, 16, 16));
					if (anim.GetANIMFlag(eANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(48, 48, 16, 16));
				}
				else
				{ 
					// Since both classic 32 arms and legs use the same texture, removing the texture would remove both limbs instead of just one.
					// So both must be disabled by the user before they're removed from the texture;
					if (anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_ARM_DISABLED) && anim.GetANIMFlag(eANIM_EFFECTS.LEFT_ARM_DISABLED)) 
						g.FillRectangle(Brushes.Magenta, new Rectangle(40, 16, 16, 16));
					if (anim.GetANIMFlag(eANIM_EFFECTS.RIGHT_LEG_DISABLED) && anim.GetANIMFlag(eANIM_EFFECTS.LEFT_LEG_DISABLED)) 
						g.FillRectangle(Brushes.Magenta, new Rectangle(0, 16, 16, 16));
				}
				nb.MakeTransparent(Color.Magenta);
				skin = nb;
			}

			skin.Save(saveFileDialog.FileName);
		}
	}
}
