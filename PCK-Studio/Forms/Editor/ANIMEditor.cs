using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using PckStudio.Classes.Utils;
using PckStudio.Forms.Additional_Popups;

namespace PckStudio.Forms.Editor
{
    public partial class ANIMEditor : MetroFramework.Forms.MetroForm
    {
        public SkinANIM ResultAnim => ruleset.Value;

        private readonly SkinANIM initialANIM;
        private ANIMRuleSet ruleset;

        sealed class ANIMRuleSet
        {
            public SkinANIM Value => anim;
            public Action<SkinANIM> OnCheckboxChanged;

            private class Bictionary<T1, T2> : Dictionary<T1, T2>
            {
                public Bictionary(int capacity)
                    : base(capacity)
                { }

                public T1 this[T2 index]
                {
                    get
                    {
                        if (!this.Any(x => x.Value.Equals(index)))
                            throw new KeyNotFoundException();
                        return this.First(x => x.Value.Equals(index)).Key;
                    }
                }

                internal void AddRange(IEnumerable<(T1, T2)> range)
                {
                    foreach (var (key, value) in range)
                    {
                        Add(key, value);
                    }
                }
            }
            private Bictionary<CheckBox, ANIM_EFFECTS> checkBoxLinkage;
            private SkinANIM anim;
            private bool ignoreCheckChanged = false;

            public ANIMRuleSet(params (CheckBox, ANIM_EFFECTS)[] linkage)
            {
                checkBoxLinkage = new Bictionary<CheckBox, ANIM_EFFECTS>(32);
                if (linkage.Length < 32)
                    Debug.WriteLine($"Not all {nameof(ANIM_EFFECTS)} are mapped to a given checkbox.");

                checkBoxLinkage.AddRange(linkage);
                foreach (var (checkbox, _) in linkage)
                {
                    checkbox.CheckedChanged += checkedChanged;
                }
            }

            internal void SetAll(bool state)
            {
                foreach (var item in checkBoxLinkage)
                {
                    IgnoreAndDo(item.Key, checkbox =>
                    {
                        anim.SetFlag(item.Value, state);
                        checkbox.Checked = state;
                        checkbox.Enabled = true;
                    });
                }
            }

            internal void ApplyAnim(SkinANIM anim)
            {
                this.anim = anim;
                foreach (var item in checkBoxLinkage)
                    item.Key.Enabled = true;
                foreach (var item in checkBoxLinkage)
                {
                    item.Key.Checked = anim.GetFlag(item.Value);
                }
            }

            private void checkedChanged(object sender, EventArgs e)
            {
                if (!ignoreCheckChanged && sender is CheckBox checkBox && checkBoxLinkage.ContainsKey(checkBox))
                {
                    switch (checkBoxLinkage[checkBox])
                    {
                        case ANIM_EFFECTS.HEAD_DISABLED:
                            checkBoxLinkage[ANIM_EFFECTS.FORCE_HEAD_ARMOR].Enabled = !checkBox.Checked;
                            break;
                        case ANIM_EFFECTS.BODY_DISABLED:
                            checkBoxLinkage[ANIM_EFFECTS.FORCE_BODY_ARMOR].Enabled = !checkBox.Checked;
                            break;
                        case ANIM_EFFECTS.LEFT_LEG_DISABLED:
                            checkBoxLinkage[ANIM_EFFECTS.FORCE_LEFT_LEG_ARMOR].Enabled = !checkBox.Checked;
                            break;
                        case ANIM_EFFECTS.RIGHT_LEG_DISABLED:
                            checkBoxLinkage[ANIM_EFFECTS.FORCE_RIGHT_LEG_ARMOR].Enabled = !checkBox.Checked;
                            break;
                        case ANIM_EFFECTS.LEFT_ARM_DISABLED:
                            checkBoxLinkage[ANIM_EFFECTS.FORCE_LEFT_ARM_ARMOR].Enabled = !checkBox.Checked;
                            break;
                        case ANIM_EFFECTS.RIGHT_ARM_DISABLED:
                            checkBoxLinkage[ANIM_EFFECTS.FORCE_RIGHT_ARM_ARMOR].Enabled = !checkBox.Checked;
                            break;
                        
                        case ANIM_EFFECTS.RESOLUTION_64x64:
                            Uncheck(checkBoxLinkage[ANIM_EFFECTS.SLIM_MODEL]);
                            checkBoxLinkage[ANIM_EFFECTS.SLIM_MODEL].Enabled = !checkBox.Checked;
                            break;

                        case ANIM_EFFECTS.SLIM_MODEL:
                            Uncheck(checkBoxLinkage[ANIM_EFFECTS.RESOLUTION_64x64]);
                            checkBoxLinkage[ANIM_EFFECTS.RESOLUTION_64x64].Enabled = !checkBox.Checked;
                            break;
                        default:
                            break;
                    }
                    anim.SetFlag(checkBoxLinkage[checkBox], checkBox.Checked && checkBox.Enabled);
                    OnCheckboxChanged?.Invoke(anim);
                }
            }

            private void Uncheck(CheckBox checkBox)
            {
                checkBox.Checked = false;
            }

            private void IgnoreAndDo(CheckBox checkBox, Action<CheckBox> action)
            {
                ignoreCheckChanged = true;
                action.Invoke(checkBox);
                ignoreCheckChanged = false;
            }
        }

        public ANIMEditor(string ANIM)
        {
            InitializeComponent();
            if (!SkinANIM.IsValidANIM(ANIM))
            {
                DialogResult = DialogResult.Abort;
                Close();
            }
            var anim = initialANIM = SkinANIM.FromString(ANIM);
            ruleset = new ANIMRuleSet(
                (bobbingCheckBox, ANIM_EFFECTS.HEAD_BOBBING_DISABLED),
                (bodyCheckBox, ANIM_EFFECTS.BODY_DISABLED),
                (bodyOCheckBox, ANIM_EFFECTS.BODY_OVERLAY_DISABLED),
                (chestplateCheckBox, ANIM_EFFECTS.FORCE_BODY_ARMOR),
                (classicCheckBox, ANIM_EFFECTS.RESOLUTION_64x64),
                (crouchCheckBox, ANIM_EFFECTS.DO_BACKWARDS_CROUCH),
                (dinnerboneCheckBox, ANIM_EFFECTS.DINNERBONE),
                (headCheckBox, ANIM_EFFECTS.HEAD_DISABLED),
                (headOCheckBox, ANIM_EFFECTS.HEAD_OVERLAY_DISABLED),
                (helmetCheckBox, ANIM_EFFECTS.FORCE_HEAD_ARMOR),
                (leftArmCheckBox, ANIM_EFFECTS.LEFT_ARM_DISABLED),
                (leftArmOCheckBox, ANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED),
                (leftArmorCheckBox, ANIM_EFFECTS.FORCE_LEFT_ARM_ARMOR),
                (leftLegCheckBox, ANIM_EFFECTS.LEFT_LEG_DISABLED),
                (leftLeggingCheckBox, ANIM_EFFECTS.FORCE_LEFT_LEG_ARMOR),
                (leftLegOCheckBox, ANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED),
                (noArmorCheckBox, ANIM_EFFECTS.ALL_ARMOR_DISABLED),
                (rightArmCheckBox, ANIM_EFFECTS.RIGHT_ARM_DISABLED),
                (rightArmOCheckBox, ANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED),
                (rightArmorCheckBox, ANIM_EFFECTS.FORCE_RIGHT_ARM_ARMOR),
                (rightLegCheckBox, ANIM_EFFECTS.RIGHT_LEG_DISABLED),
                (rightLeggingCheckBox, ANIM_EFFECTS.FORCE_RIGHT_LEG_ARMOR),
                (rightLegOCheckBox, ANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED),
                (santaCheckBox, ANIM_EFFECTS.BAD_SANTA),
                (slimCheckBox, ANIM_EFFECTS.SLIM_MODEL),
                (staticArmsCheckBox, ANIM_EFFECTS.STATIC_ARMS),
                (staticLegsCheckBox, ANIM_EFFECTS.STATIC_LEGS),
                (statueCheckBox, ANIM_EFFECTS.STATUE_OF_LIBERTY),
                (syncArmsCheckBox, ANIM_EFFECTS.SYNCED_ARMS),
                (syncLegsCheckBox, ANIM_EFFECTS.SYNCED_LEGS),
                (unknownCheckBox, ANIM_EFFECTS.__BIT_4),
                (zombieCheckBox, ANIM_EFFECTS.ZOMBIE_ARMS)
            );
            ruleset.OnCheckboxChanged = setDisplayAnim;
            setDisplayAnim(anim);
            ruleset.ApplyAnim(anim);
        }

        private void setDisplayAnim(SkinANIM anim)
        {
            animValue.Text = anim.ToString();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(animValue.Text);
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            string value = string.Empty;
            while (!SkinANIM.IsValidANIM(value))
            {
                if (!string.IsNullOrWhiteSpace(value)) MessageBox.Show($"The following value \"{value}\" is not valid. Please try again.");
                RenamePrompt diag = new RenamePrompt(value);
                diag.TextLabel.Text = "ANIM";
                diag.OKButton.Text = "Ok";
                if (diag.ShowDialog() == DialogResult.OK)
                {
                    value = diag.NewText;
                }
                else return;
            }
            ruleset.ApplyAnim(SkinANIM.FromString(value));
        }

        private void uncheckAllButton_Click(object sender, EventArgs e)
        {
            ruleset.SetAll(false);
        }

        private void checkAllButton_Click(object sender, EventArgs e)
        {
            ruleset.SetAll(true);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = animValue.Text + ".png",
                Filter = "Skin textures|*.png"
            };
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            bool isSlim = ruleset.Value.GetFlag(ANIM_EFFECTS.SLIM_MODEL);
            bool is64x64 = ruleset.Value.GetFlag(ANIM_EFFECTS.RESOLUTION_64x64);
            bool isClassic32 = !isSlim && !is64x64;

            Image skin = isSlim ? Properties.Resources.slim_template : Properties.Resources.classic_template;

            Bitmap img = new Bitmap(64, isClassic32 ? 32 : 64);
            using (Graphics graphic = Graphics.FromImage(img))
            {
                graphic.DrawImage(skin, new Rectangle(0, 0, 64, isClassic32 ? 32 : 64), new Rectangle(0, 0, 64, isClassic32 ? 32 : 64), GraphicsUnit.Pixel);
                if (ruleset.Value.GetFlag(ANIM_EFFECTS.HEAD_OVERLAY_DISABLED))
                    graphic.FillRectangle(Brushes.Magenta, new Rectangle(32, 0, 32, 16));
                if (ruleset.Value.GetFlag(ANIM_EFFECTS.HEAD_DISABLED))
                    graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 0, 32, 16));
                if (ruleset.Value.GetFlag(ANIM_EFFECTS.BODY_DISABLED))
                    graphic.FillRectangle(Brushes.Magenta, new Rectangle(16, 16, 24, 16));
                if (img.Height == 64)
                {
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.RIGHT_ARM_DISABLED)) graphic.FillRectangle(Brushes.Magenta, new Rectangle(40, 16, 16, 16));
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.RIGHT_LEG_DISABLED)) graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 16, 16, 16));
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.BODY_OVERLAY_DISABLED)) graphic.FillRectangle(Brushes.Magenta, new Rectangle(16, 32, 24, 16));
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED)) graphic.FillRectangle(Brushes.Magenta, new Rectangle(40, 32, 16, 16));
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED)) graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 32, 16, 16));
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED)) graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 48, 16, 16));
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.LEFT_LEG_DISABLED)) graphic.FillRectangle(Brushes.Magenta, new Rectangle(16, 48, 16, 16));
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.LEFT_ARM_DISABLED)) graphic.FillRectangle(Brushes.Magenta, new Rectangle(32, 48, 16, 16));
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED)) graphic.FillRectangle(Brushes.Magenta, new Rectangle(48, 48, 16, 16));
                }
                else
                {
                    // Since both classic 32 arms and legs use the same texture, removing the texture would remove both limbs instead of just one.
                    // So both must be disabled by the user before they're removed from the texture;
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.RIGHT_ARM_DISABLED) && ruleset.Value.GetFlag(ANIM_EFFECTS.LEFT_ARM_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(40, 16, 16, 16));
                    if (ruleset.Value.GetFlag(ANIM_EFFECTS.RIGHT_LEG_DISABLED) && ruleset.Value.GetFlag(ANIM_EFFECTS.LEFT_LEG_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 16, 16, 16));
                }
                img.MakeTransparent(Color.Magenta);
                skin = img;
            }
            skin.Save(saveFileDialog.FileName);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            ruleset.ApplyAnim((SkinANIM)initialANIM.Clone());
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
            diag.okBtn.Text = "Load";

            if (diag.ShowDialog() != DialogResult.OK)
                return;

            var templateANIM = new SkinANIM(Templates[diag.SelectedItem]);
            DialogResult prompt = MessageBox.Show(this, "Would you like to add this preset's effects to your current ANIM? Otherwise all of your effects will be cleared. Either choice can be undone by pressing \"Restore ANIM\".", "", MessageBoxButtons.YesNo);
            if (prompt == DialogResult.Yes)
                templateANIM = ruleset.Value | templateANIM;
            ruleset.ApplyAnim(templateANIM);
        }
    }
}
