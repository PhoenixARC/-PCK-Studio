using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using PckStudio.Internal;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Properties;

namespace PckStudio.Forms.Editor
{
    public partial class ANIMEditor : MetroFramework.Forms.MetroForm
    {
        public SkinANIM ResultAnim => ruleset.Value;

        private readonly SkinANIM initialANIM;
        private ANIMRuleSet ruleset;

        sealed class ANIMRuleSet
        {
            public SkinANIM Value => _anim;
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
                    foreach ((T1 key, T2 value) in range)
                    {
                        Add(key, value);
                    }
                }
            }
            private Bictionary<CheckBox, SkinAnimFlag> checkBoxLinkage;
            private SkinANIM _anim;
            private bool _ignoreCheckChanged = false;

            public ANIMRuleSet(params (CheckBox, SkinAnimFlag)[] linkage)
            {
                checkBoxLinkage = new Bictionary<CheckBox, SkinAnimFlag>(32);
                if (linkage.Length < 32)
                    Debug.WriteLine($"Not all {nameof(SkinAnimFlag)} are mapped to a given checkbox.");

                checkBoxLinkage.AddRange(linkage);
                foreach ((CheckBox checkbox, SkinAnimFlag _) in linkage)
                {
                    checkbox.CheckedChanged += checkedChanged;
                }
            }

            internal void SetAll(bool state)
            {
                foreach (KeyValuePair<CheckBox, SkinAnimFlag> item in checkBoxLinkage)
                {
                    IgnoreAndDo(item.Key, checkbox =>
                    {
                        checkbox.Enabled = true; // fix for checkboxes being stuck as disabled
                        checkbox.Checked = state;
                        switch(checkBoxLinkage[checkbox])
						{
                            case SkinAnimFlag.FORCE_HEAD_ARMOR:
                            case SkinAnimFlag.FORCE_BODY_ARMOR:
                            case SkinAnimFlag.FORCE_LEFT_ARM_ARMOR:
                            case SkinAnimFlag.FORCE_RIGHT_ARM_ARMOR:
                            case SkinAnimFlag.FORCE_LEFT_LEG_ARMOR:
                            case SkinAnimFlag.FORCE_RIGHT_LEG_ARMOR:
                                checkbox.Enabled = state;
                                break;
                            case SkinAnimFlag.RESOLUTION_64x64:
                                checkbox.Enabled = !state;
                                // Prioritize slim model > classic model, LCE would
                                if(state)
                                    checkbox.Checked = false;
                                break;
						}
                        _anim = _anim.SetFlag(item.Value, checkbox.Checked);
                    });
                }
                OnCheckboxChanged?.Invoke(_anim);
            }

            internal void ApplyAnim(SkinANIM anim)
            {
                this._anim = anim;
                foreach (KeyValuePair<CheckBox, SkinAnimFlag> item in checkBoxLinkage)
                {
                    /*
                     * not the best way to do this but whatever lol
                     * fix for both model flags being unset when both are set to true, with slim model prioritized of course
                     */
                    if (item.Value == SkinAnimFlag.RESOLUTION_64x64 && anim.GetFlag(SkinAnimFlag.SLIM_MODEL))
                        continue;

                    item.Key.Checked = anim.GetFlag(item.Value);
                }
            }

            private void checkedChanged(object sender, EventArgs e)
            {
                if (!_ignoreCheckChanged && sender is CheckBox checkBox && checkBoxLinkage.ContainsKey(checkBox))
                {
                    switch (checkBoxLinkage[checkBox])
                    {
                        case SkinAnimFlag.HEAD_DISABLED:
                            checkBoxLinkage[SkinAnimFlag.FORCE_HEAD_ARMOR].Enabled = checkBox.Checked;
                            Uncheck(checkBoxLinkage[SkinAnimFlag.FORCE_HEAD_ARMOR]);
                            break;
                        case SkinAnimFlag.BODY_DISABLED:
                            checkBoxLinkage[SkinAnimFlag.FORCE_BODY_ARMOR].Enabled = checkBox.Checked;
                            Uncheck(checkBoxLinkage[SkinAnimFlag.FORCE_BODY_ARMOR]);
                            break;
                        case SkinAnimFlag.LEFT_LEG_DISABLED:
                            checkBoxLinkage[SkinAnimFlag.FORCE_LEFT_LEG_ARMOR].Enabled = checkBox.Checked;
                            Uncheck(checkBoxLinkage[SkinAnimFlag.FORCE_LEFT_LEG_ARMOR]);
                            break;
                        case SkinAnimFlag.RIGHT_LEG_DISABLED:
                            checkBoxLinkage[SkinAnimFlag.FORCE_RIGHT_LEG_ARMOR].Enabled = checkBox.Checked;
                            Uncheck(checkBoxLinkage[SkinAnimFlag.FORCE_RIGHT_LEG_ARMOR]);
                            break;
                        case SkinAnimFlag.LEFT_ARM_DISABLED:
                            checkBoxLinkage[SkinAnimFlag.FORCE_LEFT_ARM_ARMOR].Enabled = checkBox.Checked;
                            Uncheck(checkBoxLinkage[SkinAnimFlag.FORCE_LEFT_ARM_ARMOR]);
                            break;
                        case SkinAnimFlag.RIGHT_ARM_DISABLED:
                            checkBoxLinkage[SkinAnimFlag.FORCE_RIGHT_ARM_ARMOR].Enabled = checkBox.Checked;
                            Uncheck(checkBoxLinkage[SkinAnimFlag.FORCE_RIGHT_ARM_ARMOR]);
                            break;
                        
                        case SkinAnimFlag.RESOLUTION_64x64:
                            Uncheck(checkBoxLinkage[SkinAnimFlag.SLIM_MODEL]);
                            checkBoxLinkage[SkinAnimFlag.SLIM_MODEL].Enabled = !checkBox.Checked;
                            break;

                        case SkinAnimFlag.SLIM_MODEL:
                            Uncheck(checkBoxLinkage[SkinAnimFlag.RESOLUTION_64x64]);
                            checkBoxLinkage[SkinAnimFlag.RESOLUTION_64x64].Enabled = !checkBox.Checked;
                            break;
                        default:
                            break;
                    }
                    _anim = _anim.SetFlag(checkBoxLinkage[checkBox], checkBox.Checked && checkBox.Enabled);
                    OnCheckboxChanged?.Invoke(_anim);
                }
            }

            private void Uncheck(CheckBox checkBox)
            {
                checkBox.Checked = false;
            }

            private void IgnoreAndDo(CheckBox checkBox, Action<CheckBox> action)
            {
                _ignoreCheckChanged = true;
                action.Invoke(checkBox);
                _ignoreCheckChanged = false;
            }
        }

        private ANIMEditor()
        {
            InitializeComponent();
            InitializeRuleSet();
            saveButton.Visible = !Settings.Default.AutoSaveChanges;
        }

        public ANIMEditor(SkinANIM skinANIM) : this()
        {
            initialANIM = skinANIM;
            setDisplayAnim(skinANIM);
            ruleset.ApplyAnim(skinANIM);
        }

        private void InitializeRuleSet()
        {
            ruleset = new ANIMRuleSet(
                (bobbingCheckBox, SkinAnimFlag.HEAD_BOBBING_DISABLED),
                (bodyCheckBox, SkinAnimFlag.BODY_DISABLED),
                (bodyOCheckBox, SkinAnimFlag.BODY_OVERLAY_DISABLED),
                (chestplateCheckBox, SkinAnimFlag.FORCE_BODY_ARMOR),
                (classicCheckBox, SkinAnimFlag.RESOLUTION_64x64),
                (crouchCheckBox, SkinAnimFlag.DO_BACKWARDS_CROUCH),
                (dinnerboneCheckBox, SkinAnimFlag.DINNERBONE),
                (headCheckBox, SkinAnimFlag.HEAD_DISABLED),
                (headOCheckBox, SkinAnimFlag.HEAD_OVERLAY_DISABLED),
                (helmetCheckBox, SkinAnimFlag.FORCE_HEAD_ARMOR),
                (leftArmCheckBox, SkinAnimFlag.LEFT_ARM_DISABLED),
                (leftArmOCheckBox, SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED),
                (leftArmorCheckBox, SkinAnimFlag.FORCE_LEFT_ARM_ARMOR),
                (leftLegCheckBox, SkinAnimFlag.LEFT_LEG_DISABLED),
                (leftLeggingCheckBox, SkinAnimFlag.FORCE_LEFT_LEG_ARMOR),
                (leftLegOCheckBox, SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED),
                (noArmorCheckBox, SkinAnimFlag.ALL_ARMOR_DISABLED),
                (rightArmCheckBox, SkinAnimFlag.RIGHT_ARM_DISABLED),
                (rightArmOCheckBox, SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED),
                (rightArmorCheckBox, SkinAnimFlag.FORCE_RIGHT_ARM_ARMOR),
                (rightLegCheckBox, SkinAnimFlag.RIGHT_LEG_DISABLED),
                (rightLeggingCheckBox, SkinAnimFlag.FORCE_RIGHT_LEG_ARMOR),
                (rightLegOCheckBox, SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED),
                (santaCheckBox, SkinAnimFlag.BAD_SANTA),
                (slimCheckBox, SkinAnimFlag.SLIM_MODEL),
                (staticArmsCheckBox, SkinAnimFlag.STATIC_ARMS),
                (staticLegsCheckBox, SkinAnimFlag.STATIC_LEGS),
                (statueCheckBox, SkinAnimFlag.STATUE_OF_LIBERTY),
                (syncArmsCheckBox, SkinAnimFlag.SYNCED_ARMS),
                (syncLegsCheckBox, SkinAnimFlag.SYNCED_LEGS),
                (unknownCheckBox, SkinAnimFlag.__BIT_4),
                (zombieCheckBox, SkinAnimFlag.ZOMBIE_ARMS)
            );
            ruleset.OnCheckboxChanged = setDisplayAnim;
        }

        private void setDisplayAnim(SkinANIM anim)
        {
            animValue.Text = anim.ToString();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
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
                if (!string.IsNullOrWhiteSpace(value))
                    MessageBox.Show(this, $"The following value \"{value}\" is not valid. Please try again.");
                TextPrompt diag = new TextPrompt(value);
                diag.LabelText = "ANIM";
                diag.OKButtonText = "Ok";
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    value = diag.NewText;
                }
                else
                    return;
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
            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                return;
            bool isSlim = ruleset.Value.GetFlag(SkinAnimFlag.SLIM_MODEL);
            bool is64x64 = ruleset.Value.GetFlag(SkinAnimFlag.RESOLUTION_64x64);
            bool isClassic32 = !isSlim && !is64x64;

            Image skin = isSlim ? Properties.Resources.slim_template : Properties.Resources.classic_template;

            Size imgSize = new Size(64, isClassic32 ? 32 : 64);

            Bitmap img = new Bitmap(imgSize.Width, imgSize.Height);
            using (Graphics graphic = Graphics.FromImage(img))
            {
                graphic.DrawImage(skin, new Rectangle(Point.Empty, imgSize), new Rectangle(Point.Empty, imgSize), GraphicsUnit.Pixel);
                if (ruleset.Value.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED))
                    graphic.FillRectangle(Brushes.Magenta, new Rectangle(32, 0, 32, 16));
                if (ruleset.Value.GetFlag(SkinAnimFlag.HEAD_DISABLED))
                    graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 0, 32, 16));
                if (ruleset.Value.GetFlag(SkinAnimFlag.BODY_DISABLED))
                    graphic.FillRectangle(Brushes.Magenta, new Rectangle(16, 16, 24, 16));
                if (img.Height == 64)
                {
                    if (ruleset.Value.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(40, 16, 16, 16));
                    if (ruleset.Value.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 16, 16, 16));
                    if (ruleset.Value.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(16, 32, 24, 16));
                    if (ruleset.Value.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(40, 32, 16, 16));
                    if (ruleset.Value.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 32, 16, 16));
                    if (ruleset.Value.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 48, 16, 16));
                    if (ruleset.Value.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(16, 48, 16, 16));
                    if (ruleset.Value.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(32, 48, 16, 16));
                    if (ruleset.Value.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(48, 48, 16, 16));
                }
                else
                {
                    // Since both classic 32 arms and legs use the same texture, removing the texture would remove both limbs instead of just one.
                    // So both must be disabled by the user before they're removed from the texture;
                    if (ruleset.Value.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED) && ruleset.Value.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(40, 16, 16, 16));
                    if (ruleset.Value.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED) && ruleset.Value.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED))
                        graphic.FillRectangle(Brushes.Magenta, new Rectangle(0, 16, 16, 16));
                }
                img.MakeTransparent(Color.Magenta);
                skin = img;
            }
            skin.Save(saveFileDialog.FileName);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            ruleset.ApplyAnim(initialANIM);
            setDisplayAnim(initialANIM);
        }

        static readonly Dictionary<string, SkinAnimMask> Templates = new Dictionary<string, SkinAnimMask>()
        {
                { "Steve (64x32)",           SkinAnimMask.NONE },
                { "Steve (64x64)",           SkinAnimMask.RESOLUTION_64x64 },
                { "Alex (64x64)",            SkinAnimMask.SLIM_MODEL },
                { "Zombie Skins",            SkinAnimMask.ZOMBIE_ARMS },
                { "Cetacean Skins",          SkinAnimMask.SYNCED_ARMS | SkinAnimMask.SYNCED_LEGS },
                { "Ski Skins",               SkinAnimMask.SYNCED_ARMS | SkinAnimMask.STATIC_LEGS },
                { "Ghost Skins",             SkinAnimMask.STATIC_LEGS | SkinAnimMask.ZOMBIE_ARMS },
                { "Medusa (Greek Myth.)",    SkinAnimMask.SYNCED_LEGS },
                { "Librarian (Halo)",        SkinAnimMask.STATIC_LEGS },
                { "Grim Reaper (Halloween)", SkinAnimMask.STATIC_LEGS | SkinAnimMask.STATIC_ARMS }
        };

        private void templateButton_Click(object sender, EventArgs e)
        {
            var diag = new ItemSelectionPopUp(Templates.Keys.ToArray());
            diag.ButtonText = "Presets";
            diag.ButtonText = "Load";

            if (diag.ShowDialog(this) != DialogResult.OK)
                return;

            SkinANIM templateANIM = SkinANIM.Empty.SetMask(Templates[diag.SelectedItem]);
            DialogResult prompt = MessageBox.Show(this, "Would you like to add this preset's effects to your current ANIM? Otherwise all of your effects will be cleared. Either choice can be undone by pressing \"Restore ANIM\".", "", MessageBoxButtons.YesNo);
            if (prompt == DialogResult.Yes)
                templateANIM |= ruleset.Value;
            ruleset.ApplyAnim(templateANIM);
        }

        private void ANIMEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.Default.AutoSaveChanges)
            {
                saveButton_Click(sender, EventArgs.Empty);
            }
        }
    }
}
