using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using PckStudio.Classes.Utils;
using PckStudio.Forms.Additional_Popups;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms.Utilities.Skins
{
	public partial class ANIMEditor : ThemeForm
	{
		public bool saved = false;
		readonly SkinANIM initialANIM;
		public string outANIM => animValue.Text;
		SkinANIM anim = new SkinANIM();

		void processCheckBoxes(bool set_all = false, bool value = false)
		{
			#region processes every single checkbox with the correct ANIM flags
			helmetCheckBox.Enabled = set_all ? value : anim.GetFlag(ANIM_EFFECTS.HEAD_DISABLED);
			chestplateCheckBox.Enabled = set_all ? value : anim.GetFlag(ANIM_EFFECTS.BODY_DISABLED);
			leftArmorCheckBox.Enabled = set_all ? value : anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_DISABLED);
			rightArmorCheckBox.Enabled = set_all ? value : anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_DISABLED);
			leftLeggingCheckBox.Enabled = set_all ? value : anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_DISABLED);
			rightLeggingCheckBox.Enabled = set_all ? value : anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_DISABLED);

			bobbingCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.HEAD_BOBBING_DISABLED);
			bodyCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.BODY_DISABLED);
			bodyOCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.BODY_OVERLAY_DISABLED);
			chestplateCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.FORCE_BODY_ARMOR);

			classicCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.RESOLUTION_64x64);
			crouchCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.DO_BACKWARDS_CROUCH);
			dinnerboneCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.DINNERBONE);
			headCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.HEAD_DISABLED);

			headOCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.HEAD_OVERLAY_DISABLED);
			helmetCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.FORCE_HEAD_ARMOR);
			leftArmCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_DISABLED);
			leftArmOCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED);

			leftArmorCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.FORCE_LEFT_ARM_ARMOR);
			leftLegCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_DISABLED);
			leftLeggingCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.FORCE_LEFT_LEG_ARMOR);
			leftLegOCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED);

			noArmorCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.ALL_ARMOR_DISABLED);
			rightArmCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_DISABLED);
			rightArmOCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED);
			rightArmorCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.FORCE_RIGHT_ARM_ARMOR);

			rightLegCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_DISABLED);
			rightLeggingCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.FORCE_RIGHT_LEG_ARMOR);
			rightLegOCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED);
			santaCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.BAD_SANTA);

			slimCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.SLIM_MODEL);
			staticArmsCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.STATIC_ARMS);
			staticLegsCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.STATIC_LEGS);
			statueCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.STATUE_OF_LIBERTY);

			syncArmsCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.SYNCED_ARMS);
			syncLegsCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.SYNCED_LEGS);
			unknownCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.__BIT_4);
			zombieCheckBox.Checked = set_all ? value : anim.GetFlag(ANIM_EFFECTS.ZOMBIE_ARMS);
			#endregion
		}

		public ANIMEditor(string ANIM)
		{
			InitializeComponent();
			if (!SkinANIM.IsValidANIM(ANIM))
			{
				DialogResult = DialogResult.Abort;
				Close();
			}
            initialANIM = anim = new SkinANIM(ANIM);

			#region Event definitions, since the designer can't parse lambda experessions
			bobbingCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.HEAD_BOBBING_DISABLED); };
			bodyCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.BODY_DISABLED); };
			bodyOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.BODY_OVERLAY_DISABLED); };
			chestplateCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_BODY_ARMOR); };

			classicCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.RESOLUTION_64x64); };
			crouchCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.DO_BACKWARDS_CROUCH); };
			dinnerboneCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.DINNERBONE); };
			headCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.HEAD_DISABLED); };

			headOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.HEAD_OVERLAY_DISABLED); };
			helmetCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_HEAD_ARMOR); };
			leftArmCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.LEFT_ARM_DISABLED); };
			leftArmOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED); };

			leftArmorCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_LEFT_ARM_ARMOR); };
			leftLegCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.LEFT_LEG_DISABLED); };
			leftLeggingCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_LEFT_LEG_ARMOR); };
			leftLegOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED); };

			noArmorCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.ALL_ARMOR_DISABLED); };
			rightArmCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.RIGHT_ARM_DISABLED); };
			rightArmOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED); };
			rightArmorCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_RIGHT_ARM_ARMOR); };

			rightLegCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.RIGHT_LEG_DISABLED); };
			rightLeggingCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_RIGHT_LEG_ARMOR); };
			rightLegOCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED); };
			santaCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.BAD_SANTA); };

			slimCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.SLIM_MODEL); };
			staticArmsCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.STATIC_ARMS); };
			staticLegsCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.STATIC_LEGS); };
			statueCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.STATUE_OF_LIBERTY); };

			syncArmsCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.SYNCED_ARMS); };
			syncLegsCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.SYNCED_LEGS); };
			unknownCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.__BIT_4); };
			zombieCheckBox.CheckedChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.ZOMBIE_ARMS); };

			helmetCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_HEAD_ARMOR); };
			chestplateCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_BODY_ARMOR); };
			rightArmorCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_RIGHT_ARM_ARMOR); };
			leftArmorCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_LEFT_ARM_ARMOR); };
			rightLeggingCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_RIGHT_LEG_ARMOR); };
			leftLeggingCheckBox.EnabledChanged += (sender, EventArgs) => { flagChanged(sender, EventArgs, ANIM_EFFECTS.FORCE_LEFT_LEG_ARMOR); };
			#endregion
            processCheckBoxes();
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			saved = true;
			Close();
		}

		private void flagChanged(object sender, EventArgs e, ANIM_EFFECTS flag)
		{
			// Set value
			anim.SetFlag(flag, ((CheckBox)sender).Checked && ((CheckBox)sender).Enabled);

			// Armor flags don't work if the respective parts are not enabled
			helmetCheckBox.Enabled = anim.GetFlag(ANIM_EFFECTS.HEAD_DISABLED);
			chestplateCheckBox.Enabled = anim.GetFlag(ANIM_EFFECTS.BODY_DISABLED);
			rightArmorCheckBox.Enabled = anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_DISABLED);
			leftArmorCheckBox.Enabled = anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_DISABLED);
			rightLeggingCheckBox.Enabled = anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_DISABLED);
			leftLeggingCheckBox.Enabled = anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_DISABLED);

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
				if (!first) MessageBox.Show($"The following value \"{new_value}\" is not valid. Please try again.");
				RenamePrompt diag = new RenamePrompt(new_value);
				diag.TextLabel.Text = "ANIM";
				diag.RenameButton.Text = "Ok";
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
			bool isSlim = anim.GetFlag(ANIM_EFFECTS.SLIM_MODEL);
			bool isClassic64 = anim.GetFlag(ANIM_EFFECTS.RESOLUTION_64x64);
			bool isClassic32 = !isSlim && !isClassic64;

			Image skin = isSlim ? Properties.Resources.slim_template : Properties.Resources.classic_template;

			#region Image processing code for generating the skin templates based on the input ANIM value
			Bitmap nb = new Bitmap(64, (!isSlim && !isClassic64) ? 32 : 64);
			using (Graphics g = Graphics.FromImage(nb))
			{
				g.DrawImage(skin, new Rectangle(0, 0, 64, isClassic32 ? 32 : 64), new Rectangle(0, 0, 64, isClassic32 ? 32 : 64), GraphicsUnit.Pixel);
				if (anim.GetFlag(ANIM_EFFECTS.HEAD_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(32, 0, 32, 16));
				if (anim.GetFlag(ANIM_EFFECTS.HEAD_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(0, 0, 32, 16));
				if (anim.GetFlag(ANIM_EFFECTS.BODY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(16, 16, 24, 16));
				if (nb.Height == 64)
				{
					if (anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(40, 16, 16, 16));
					if (anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(0, 16, 16, 16));
					if (anim.GetFlag(ANIM_EFFECTS.BODY_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(16, 32, 24, 16));
					if (anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(40, 32, 16, 16));
					if (anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(0, 32, 16, 16));
					if (anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(0, 48, 16, 16));
					if (anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(16, 48, 16, 16));
					if (anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(32, 48, 16, 16));
					if (anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED)) g.FillRectangle(Brushes.Magenta, new Rectangle(48, 48, 16, 16));
				}
				else
				{ 
					// Since both classic 32 arms and legs use the same texture, removing the texture would remove both limbs instead of just one.
					// So both must be disabled by the user before they're removed from the texture;
					if (anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_DISABLED) && anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_DISABLED)) 
						g.FillRectangle(Brushes.Magenta, new Rectangle(40, 16, 16, 16));
					if (anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_DISABLED) && anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_DISABLED)) 
						g.FillRectangle(Brushes.Magenta, new Rectangle(0, 16, 16, 16));
				}
				nb.MakeTransparent(Color.Magenta);
				skin = nb;
			}
			#endregion

			skin.Save(saveFileDialog.FileName);
		}

		private void resetButton_Click(object sender, EventArgs e)
		{
			anim = initialANIM;
			processCheckBoxes();
		}

		static readonly Dictionary<string, ANIM_EFFECTS> Templates = new Dictionary<string, ANIM_EFFECTS>()
		{
				{ "Steve (64x32)",           ANIM_EFFECTS.NONE },
				{ "Steve (64x64)",           ANIM_EFFECTS.RESOLUTION_64x64 },
				{ "Alex (64x64)",            ANIM_EFFECTS.SLIM_MODEL },
				{ "Zombie Skins",            ANIM_EFFECTS.ZOMBIE_ARMS },
				{ "Cetacean Skins",          ANIM_EFFECTS.SYNCED_ARMS | ANIM_EFFECTS.SYNCED_LEGS },
				{ "Ski Skins",               ANIM_EFFECTS.SYNCED_ARMS | ANIM_EFFECTS.STATIC_LEGS },
				{ "Ghost Skins",             ANIM_EFFECTS.STATIC_LEGS | ANIM_EFFECTS.ZOMBIE_ARMS },
				{ "Medusa (Greek Myth.)",    ANIM_EFFECTS.SYNCED_LEGS },
				{ "Librarian (Halo)",        ANIM_EFFECTS.STATIC_LEGS },
				{ "Grim Reaper (Halloween)", ANIM_EFFECTS.STATIC_LEGS | ANIM_EFFECTS.STATIC_ARMS }
		};

		private void templateButton_Click(object sender, EventArgs e)
		{
			var diag = new ItemSelectionPopUp(Templates.Keys.ToArray());
			diag.label2.Text = "Presets";
			//diag.button1.Text = "Load";
			//MNL or PhoenixARC or Miku, here is one problem. I removed the old button (button1) and relpaced it with the 'AddButton' but for osme reason, it does not work here. 
			// - EternalModz

			if (diag.ShowDialog() != DialogResult.OK) return;

			var templateANIM = Templates[diag.SelectedItem];
			DialogResult prompt = MessageBox.Show(this, "Would you like to add this preset's effects to your current ANIM? Otherwise all of your effects will be cleared. Either choice can be undone by pressing \"Restore ANIM\".", "", MessageBoxButtons.YesNo);
			if (prompt == DialogResult.Yes) anim |= templateANIM;
			else anim = templateANIM;
			SkinANIM backup = anim;
			processCheckBoxes();
			anim = backup;
		}
	}
}
