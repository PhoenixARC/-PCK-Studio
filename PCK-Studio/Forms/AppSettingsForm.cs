using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Properties;

namespace PckStudio.Forms
{
    public partial class AppSettingsForm : MetroForm
    {
        public AppSettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void autoSaveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.AutoSaveChanges = autoSaveCheckBox.Checked;
        }

        private void endianCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UseLittleEndianAsDefault = endianCheckBox.Checked;
        }

        private void autoLoadPckCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.LoadSubPcks = autoLoadPckCheckBox.Checked;
        }

        private void showPresenceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ShowRichPresence = showPresenceCheckBox.Checked;
        }

        private void autoUpdateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            usePrereleaseCheckBox.Visible = Settings.Default.AutoUpdate = autoUpdateCheckBox.Checked;
        }

        private void grf_paramKeyComboBoxCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UseComboBoxForGRFParameter = grf_paramKeyComboBoxCheckBox.Checked;
        }

        private void usePrereleaseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UsePrerelease = usePrereleaseCheckBox.Checked;
        }

        private void validateSkinDimenssionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ValidateImageDimension = validateSkinDimenssionCheckBox.Checked;
        }

        private void LoadCheckboxState(CheckBox checkBox, EventHandler eventHandler, bool state)
        {
            checkBox.CheckedChanged -= eventHandler;
            checkBox.Checked = state;
            checkBox.CheckedChanged += eventHandler;
        }

        private void LoadSettings()
        {
            LoadCheckboxState(autoSaveCheckBox, autoSaveCheckBox_CheckedChanged, Settings.Default.AutoSaveChanges);
            LoadCheckboxState(endianCheckBox, endianCheckBox_CheckedChanged, Settings.Default.UseLittleEndianAsDefault);
            LoadCheckboxState(autoLoadPckCheckBox, autoLoadPckCheckBox_CheckedChanged, Settings.Default.LoadSubPcks);
            LoadCheckboxState(showPresenceCheckBox, showPresenceCheckBox_CheckedChanged, Settings.Default.ShowRichPresence);
            LoadCheckboxState(autoUpdateCheckBox, autoUpdateCheckBox_CheckedChanged, Settings.Default.AutoUpdate);
            LoadCheckboxState(usePrereleaseCheckBox, usePrereleaseCheckBox_CheckedChanged, Settings.Default.UsePrerelease);
            LoadCheckboxState(grf_paramKeyComboBoxCheckBox, grf_paramKeyComboBoxCheckBox_CheckedChanged, Settings.Default.UseComboBoxForGRFParameter);
            LoadCheckboxState(validateSkinDimenssionCheckBox, validateSkinDimenssionCheckBox_CheckedChanged, Settings.Default.ValidateImageDimension);
        }

        private void AppBehaviorSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
